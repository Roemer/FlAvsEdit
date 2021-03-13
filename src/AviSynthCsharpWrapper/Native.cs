using System;
using System.Runtime.InteropServices;
using System.Text;

namespace AviSynthCsharpWrapper
{
    /// <summary>
    /// Contains the native methods and objects needed for the wrapper.
    /// </summary>
    public static class Native
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct AVSDLLVideoInfo
        {
            public int width;
            public int height;
            public int raten;
            public int rated;
            public int aspectn;
            public int aspectd;
            public int interlaced_frame;
            public int top_field_first;
            public int num_frames;
            public AviSynthColorspace pixel_type;

            // Audio
            public int audio_samples_per_second;
            public AudioSampleType sample_type;
            public int nchannels;
            public int num_audio_frames;
            public long num_audio_samples;
        }

        // getinterfaceversion
        [DllImport(@"x64/AviSynthCppWrapper.dll", EntryPoint = "avswrapper_getinterfaceversion", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int avswrapper_getinterfaceversion_64(ref int val);
        [DllImport(@"x86/AviSynthCppWrapper.dll", EntryPoint = "avswrapper_getinterfaceversion", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int avswrapper_getinterfaceversion_32(ref int val);

        // init_3
        [DllImport(@"x64/AviSynthCppWrapper.dll", EntryPoint = "avswrapper_init_3", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int avswrapper_init_3_64(ref IntPtr avs, string func, string arg, ref AVSDLLVideoInfo vi, ref AviSynthColorspace originalColorspace, ref AudioSampleType originalSampleType, string cs);
        [DllImport(@"x86/AviSynthCppWrapper.dll", EntryPoint = "avswrapper_init_3", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int avswrapper_init_3_32(ref IntPtr avs, string func, string arg, ref AVSDLLVideoInfo vi, ref AviSynthColorspace originalColorspace, ref AudioSampleType originalSampleType, string cs);

        // destroy
        [DllImport(@"x64/AviSynthCppWrapper.dll", EntryPoint = "avswrapper_destroy", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int avswrapper_destroy_64(ref IntPtr avs);
        [DllImport(@"x86/AviSynthCppWrapper.dll", EntryPoint = "avswrapper_destroy", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int avswrapper_destroy_32(ref IntPtr avs);

        // getlasterror
        [DllImport(@"x64/AviSynthCppWrapper.dll", EntryPoint = "avswrapper_getlasterror", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int avswrapper_getlasterror_64(IntPtr avs, [MarshalAs(UnmanagedType.LPStr)] StringBuilder sb, int len);
        [DllImport(@"x86/AviSynthCppWrapper.dll", EntryPoint = "avswrapper_getlasterror", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int avswrapper_getlasterror_32(IntPtr avs, [MarshalAs(UnmanagedType.LPStr)] StringBuilder sb, int len);

        // getaframe
        [DllImport("x64/AviSynthCppWrapper.dll", EntryPoint = "avswrapper_getaframe", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int avswrapper_getaframe_64(IntPtr avs, IntPtr buf, long sampleNo, long sampleCount);
        [DllImport("x86/AviSynthCppWrapper.dll", EntryPoint = "avswrapper_getaframe", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int avswrapper_getaframe_32(IntPtr avs, IntPtr buf, long sampleNo, long sampleCount);

        // getvframe
        [DllImport("x64/AviSynthCppWrapper.dll", EntryPoint = "avswrapper_getvframe", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int avswrapper_getvframe_64(IntPtr avs, IntPtr buf, int stride, int frm);
        [DllImport("x86/AviSynthCppWrapper.dll", EntryPoint = "avswrapper_getvframe", ExactSpelling = true, SetLastError = false, CharSet = CharSet.Ansi)]
        private static extern int avswrapper_getvframe_32(IntPtr avs, IntPtr buf, int stride, int frm);

        public static int avswrapper_getinterfaceversion(ref int val)
        {
            if (Marshal.SizeOf(typeof(IntPtr)) == 4)
            {
                return avswrapper_getinterfaceversion_32(ref val);
            }
            else
            {
                return avswrapper_getinterfaceversion_64(ref val);
            }
        }

        public static int avswrapper_init_3(ref IntPtr avs, string func, string arg, ref AVSDLLVideoInfo vi, ref AviSynthColorspace originalColorspace, ref AudioSampleType originalSampleType, string cs)
        {
            if (Marshal.SizeOf(typeof(IntPtr)) == 4)
            {
                return avswrapper_init_3_32(ref avs, func, arg, ref vi, ref originalColorspace, ref originalSampleType, cs);
            }
            else
            {
                return avswrapper_init_3_64(ref avs, func, arg, ref vi, ref originalColorspace, ref originalSampleType, cs);
            }
        }

        public static int avswrapper_destroy(ref IntPtr avs)
        {
            if (Marshal.SizeOf(typeof(IntPtr)) == 4)
            {
                return avswrapper_destroy_32(ref avs);
            }
            else
            {
                return avswrapper_destroy_64(ref avs);
            }
        }

        public static int avswrapper_getlasterror(IntPtr avs, [MarshalAs(UnmanagedType.LPStr)] StringBuilder sb, int len)
        {
            if (Marshal.SizeOf(typeof(IntPtr)) == 4)
            {
                return avswrapper_getlasterror_32(avs, sb, len);
            }
            else
            {
                return avswrapper_getlasterror_64(avs, sb, len);
            }
        }

        public static int avswrapper_getaframe(IntPtr avs, IntPtr buf, long sampleNo, long sampleCount)
        {
            if (Marshal.SizeOf(typeof(IntPtr)) == 4)
            {
                return avswrapper_getaframe_32(avs, buf, sampleNo, sampleCount);
            }
            else
            {
                return avswrapper_getaframe_64(avs, buf, sampleNo, sampleCount);
            }
        }

        public static int avswrapper_getvframe(IntPtr avs, IntPtr buf, int stride, int frm)
        {
            if (Marshal.SizeOf(typeof(IntPtr)) == 4)
            {
                return avswrapper_getvframe_32(avs, buf, stride, frm);
            }
            else
            {
                return avswrapper_getvframe_64(avs, buf, stride, frm);
            }
        }
    }
}
