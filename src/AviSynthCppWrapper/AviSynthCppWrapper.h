#pragma once

// The maximum length of the error message
#define ERRMSG_LEN 1024

/// <summary>
/// Struct that contains the whole environment for AviSynth.
/// </summary>
typedef struct AvsWrapperStruct
{
	char err[ERRMSG_LEN];
	IScriptEnvironment* env;
	AVSValue* res;
	PClip clp;
	HMODULE dll;
};

/// <summary>
/// Struct that contains the information about the clip.
/// </summary>
typedef struct AVSDLLVideoInfo {
	// Video
	int width;
	int height;
	int raten;
	int rated;
	int aspectn;
	int aspectd;
	int interlaced_frame;
	int top_field_first;
	int num_frames;
	int pixel_type;

	// Audio
	int audio_samples_per_second;
	int sample_type;
	int nchannels;
	int num_audio_frames;
	int64_t num_audio_samples;
};
