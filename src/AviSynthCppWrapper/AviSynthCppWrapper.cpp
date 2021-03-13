#include <tchar.h>
#include <Windows.h>

// Define which interface version that is used
#define AVISYNTH_INTERFACE_BUILD_VERSION 8

// Impor the correct AviSynth header files
#if AVISYNTH_INTERFACE_BUILD_VERSION == 8
#include "avs_core_v8\avisynth.h"
#endif

#include "AviSynthCppWrapper.h"

#if AVISYNTH_INTERFACE_BUILD_VERSION > 4
const AVS_Linkage* AVS_linkage = 0; // AviSynth 2.6 only
#endif

// Methods that are exported
extern "C" {
	__declspec(dllexport) int __stdcall avswrapper_getinterfaceversion(int* result);
	__declspec(dllexport) int __stdcall avswrapper_init_3(AvsWrapperStruct** ppstr, char* func, char* arg, AVSDLLVideoInfo* vi, int* originalPixelType, int* originalSampleType, char* cs);
	__declspec(dllexport) int __stdcall avswrapper_destroy(AvsWrapperStruct** ppstr);
	__declspec(dllexport) int __stdcall avswrapper_getlasterror(AvsWrapperStruct* pstr, char* str, int len);
	__declspec(dllexport) int __stdcall avswrapper_getaframe(AvsWrapperStruct* pstr, void* buf, __int64 start, __int64 count);
	__declspec(dllexport) int __stdcall avswrapper_getvframe(AvsWrapperStruct* pstr, void* buf, int stride, int frm);
}

/// <summary>
/// Gets the interface version of AviSynth against which this wrapper was compiled.
/// </summary>
int __stdcall avswrapper_getinterfaceversion(int* result)
{
	try
	{
		*result = AVISYNTH_INTERFACE_VERSION;
		return 0;
	}
	catch (...)
	{
		return -1;
	}
}

/// <summary>
/// Initializes a ScriptEnvironment and adds a function with a parameter.
/// </summary>
/// <param name="ppstr">Pointer to the wrapper object.</param>
/// <param name="func">The function to call.</param>
/// <param name="arg">The arguments for the function to call.</param>
/// <param name="vi">The video information object.</param>
/// <param name="originalPixelType">The pixel type.</param>
/// <param name="originalSampleType">The sample type.</param>
/// <param name="cs">The color space</param>
/// <returns>0 on success, anything else otherwise.</returns>
int __stdcall avswrapper_init_3(AvsWrapperStruct** ppstr, char* func, char* arg, AVSDLLVideoInfo* vi, int* originalPixelType, int* originalSampleType, char* cs)
{
	// Initialize the wrapper object
	AvsWrapperStruct* pstr = ((AvsWrapperStruct*)malloc(sizeof(AvsWrapperStruct)));

	try
	{
		*ppstr = pstr;
		if (pstr)
		{
			memset(pstr, 0, sizeof(AvsWrapperStruct));
		}
		else
		{
			// Failed to initialize the wrapper object at all
			return -1;
		}

		// Load AviSynth
		pstr->dll = LoadLibrary(_T("avisynth.dll"));
		if (!pstr->dll)
		{
			strncpy_s(pstr->err, ERRMSG_LEN, "AviSynth installation cannot be found", _TRUNCATE);
			return 1;
		}

		// Create the script environment
		IScriptEnvironment* (*CreateScriptEnvironment)(int version) = (IScriptEnvironment * (*)(int)) GetProcAddress(pstr->dll, "CreateScriptEnvironment");
		if (!CreateScriptEnvironment)
		{
			strncpy_s(pstr->err, ERRMSG_LEN, "Cannot load CreateScriptEnvironment", _TRUNCATE);
			return 2;
		}

		pstr->env = CreateScriptEnvironment(AVISYNTH_INTERFACE_VERSION);
		if (pstr->env == NULL)
		{
#if AVISYNTH_INTERFACE_BUILD_VERSION > 4
			strncpy_s(pstr->err, ERRMSG_LEN, "AviSynth 2.6 required", _TRUNCATE);
#else
			strncpy_s(pstr->err, ERRMSG_LEN, "AviSynth 2.5 required", _TRUNCATE);
#endif
			return 3;
		}

#if AVISYNTH_INTERFACE_BUILD_VERSION > 4
		AVS_linkage = pstr->env->GetAVSLinkage(); // AviSynth 2.6 only
#endif

		// Invoke the function with the given argument
		AVSValue arg(arg);
		AVSValue res = pstr->env->Invoke(func, AVSValue(&arg, 1));
		if (!res.IsClip())
		{
			strncpy_s(pstr->err, ERRMSG_LEN, "The script's return was not a video clip.", _TRUNCATE);
			return 4;
		}

		pstr->clp = res.AsClip();
		VideoInfo inf = pstr->clp->GetVideoInfo();
		VideoInfo infh = pstr->clp->GetVideoInfo();

		if (inf.HasVideo())
		{
			*originalPixelType = inf.pixel_type;

			// Convert video only if RGB24 is required
			if (strcmp("RGB24", cs) == 0 && (!inf.IsRGB24()))
			{
				// Make sure that the clip is 8 bit
				AVSValue args[2] = { res.AsClip(), 8 };
				res = pstr->env->Invoke("ConvertBits", AVSValue(args, 2));

				// convert to RGB24
				res = pstr->env->Invoke("ConvertToRGB24", res);
				pstr->clp = res.AsClip();
				infh = pstr->clp->GetVideoInfo();
				if (!infh.IsRGB24())
				{
					strncpy_s(pstr->err, ERRMSG_LEN, "Cannot convert video to RGB24", _TRUNCATE);
					return 5;
				}
			}
		}

		// Fill the information object
		inf = pstr->clp->GetVideoInfo();
		if (vi != NULL)
		{
			vi->width = inf.width;
			vi->height = inf.height;
			vi->raten = inf.fps_numerator;
			vi->rated = inf.fps_denominator;
			vi->aspectn = 0;
			vi->aspectd = 1;
			vi->interlaced_frame = 0;
			vi->top_field_first = 0;
			vi->num_frames = inf.num_frames;
			vi->pixel_type = inf.pixel_type;

			vi->audio_samples_per_second = inf.audio_samples_per_second;
			vi->num_audio_samples = inf.num_audio_samples;
			vi->sample_type = inf.sample_type;
			vi->nchannels = inf.nchannels;
		}

		pstr->res = new AVSValue(res);

		pstr->err[0] = 0;
		return 0;
	}
	catch (AvisynthError err)
	{
		strncpy_s(pstr->err, ERRMSG_LEN, err.msg, _TRUNCATE);
		return 999;
	}
	catch (...)
	{
		strncpy_s(pstr->err, ERRMSG_LEN, "Unhandled error: avswrapper_init_3", _TRUNCATE);
		return 999;
	}
}

/// <summary>
/// Cleanup a script environment.
/// </summary>
/// <param name="ppstr"></param>
/// <returns></returns>
int __stdcall avswrapper_destroy(AvsWrapperStruct** ppstr)
{
	try
	{
		if (!ppstr)
		{
			return 1;
		}

		AvsWrapperStruct* pstr = *ppstr;
		if (!pstr)
		{
			return 1;
		}

		if (pstr->clp)
		{
			pstr->clp = NULL;
		}

		if (pstr->res)
		{
			delete pstr->res;
			pstr->res = NULL;
		}

		if (pstr->env)
		{
#if AVISYNTH_INTERFACE_BUILD_VERSION > 4
			pstr->env->DeleteScriptEnvironment(); // AviSynth 2.6
#else
			delete pstr->env; // AviSynth 2.5
#endif
			pstr->env = NULL;
		}

		if (pstr->dll)
		{
			FreeLibrary(pstr->dll);
		}

#if AVISYNTH_INTERFACE_BUILD_VERSION > 4
		AVS_linkage = 0;  // AviSynth 2.6 only
#endif

		free(pstr);
		*ppstr = NULL;
		return 0;
	}
	catch (...)
	{
		return -1;
	}
}

/// <summary>
/// Gets the last error text that occured.
/// </summary>
/// <param name="pstr"></param>
/// <param name="str"></param>
/// <param name="len"></param>
/// <returns></returns>
int __stdcall avswrapper_getlasterror(AvsWrapperStruct* pstr, char* str, int len)
{
	try
	{
		strncpy_s(str, len, pstr->err, len - 1);
		return (int)strlen(str);
	}
	catch (...)
	{
		strncpy_s(str, ERRMSG_LEN, "Unhandled error: avswrapper_getlasterror", _TRUNCATE);
		return (int)strlen(str);
	}
}

/// <summary>
/// Gets audio frames within the given sample range.
/// </summary>
/// <param name="pstr">Pointer to the wrapper object.</param>
/// <param name="buf">Pointer to the buffer to write the audio data to.</param>
/// <param name="start">Sample start position.</param>
/// <param name="count">Number of samples to get.</param>
/// <returns>0 on success, anything else otherwise.</returns>
int __stdcall avswrapper_getaframe(AvsWrapperStruct* pstr, void* buf, __int64 start, __int64 count)
{
	try
	{
		pstr->clp->GetAudio(buf, start, count, pstr->env);
		pstr->err[0] = 0;
		return 0;
	}
	catch (AvisynthError err)
	{
		strncpy_s(pstr->err, ERRMSG_LEN, err.msg, _TRUNCATE);
		return -1;
	}
	catch (...)
	{
		strncpy_s(pstr->err, ERRMSG_LEN, "Unhandled error: avswrapper_getaframe", _TRUNCATE);
		return -1;
	}
}

/// <summary>
/// Gets a video frame.
/// </summary>
/// <param name="pstr">Pointer to the wrapper object.</param>
/// <param name="buf">Pointer to the buffer to write the video data to.</param>
/// <param name="stride">???</param>
/// <param name="frm">The number of the frame to get.</param>
/// <returns>0 on success, anything else otherwise.</returns>
int __stdcall avswrapper_getvframe(AvsWrapperStruct* pstr, void* buf, int stride, int frm)
{
	try
	{
		PVideoFrame f = pstr->clp->GetFrame(frm, pstr->env);
		if (buf && stride)
		{
			pstr->env->BitBlt((BYTE*)buf, stride, f->GetReadPtr(), f->GetPitch(), f->GetRowSize(), f->GetHeight());
		}
		pstr->err[0] = 0;
		return 0;
	}
	catch (AvisynthError err)
	{
		strncpy_s(pstr->err, ERRMSG_LEN, err.msg, _TRUNCATE);
		return -1;
	}
	catch (...)
	{
		strncpy_s(pstr->err, ERRMSG_LEN, "Unhandled error: avswrapper_getvframe", _TRUNCATE);
		return -1;
	}
}
