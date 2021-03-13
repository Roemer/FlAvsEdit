using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace AviSynthCsharpWrapper
{
    public class AviSynthClip : IDisposable
    {
        private IntPtr _avs;
        private Native.AVSDLLVideoInfo _vi;
        private AviSynthColorspace _colorSpace;
        private AudioSampleType _sampleType;
        private bool disposedValue;

        /// <summary>
        /// Audio samples per second.
        /// </summary>
        public int AudioSampleRate => _vi.audio_samples_per_second;

        /// <summary>
        /// Total samples count.
        /// </summary>
        public long SamplesCount => _vi.num_audio_samples;

        /// <summary>
        /// The number of samples per frame.
        /// </summary>
        public int SamplesPerFrame => (int)(SamplesCount / TotalFrames);

        /// <summary>
        /// The sample type.
        /// </summary>
        public AudioSampleType SampleType => _vi.sample_type;

        /// <summary>
        /// Number of audio channels.
        /// </summary>
        public short ChannelsCount => (short)_vi.nchannels;

        /// <summary>
        /// Bits per sample.
        /// </summary>
        public short BitsPerSample => (short)(BytesPerSample * 8);

        /// <summary>
        /// Bytes per sample.
        /// </summary>
        public short BytesPerSample
        {
            get
            {
                switch (SampleType)
                {
                    case AudioSampleType.INT8:
                        return 1;
                    case AudioSampleType.INT16:
                        return 2;
                    case AudioSampleType.INT24:
                        return 3;
                    case AudioSampleType.INT32:
                        return 4;
                    case AudioSampleType.FLOAT:
                        return 4;
                    default:
                        throw new ArgumentException(SampleType.ToString());
                }
            }
        }

        /// <summary>
        /// The average audio bytes per second.
        /// </summary>
        public int AvgBytesPerSec
        {
            get
            {
                return AudioSampleRate * ChannelsCount * BytesPerSample;
            }
        }

        /// <summary>
        /// The total size in bytes of the audio.
        /// </summary>
        public long AudioSizeInBytes
        {
            get
            {
                return (SamplesCount > 0 ? SamplesCount : 0) * ChannelsCount * BytesPerSample;
            }
        }

        /// <summary>
        /// The width of the video.
        /// </summary>
        public int VideoWidth => _vi.width;

        /// <summary>
        /// The height of the video.
        /// </summary>
        public int VideoHeight => _vi.height;

        /// <summary>
        /// The framerate of the video.
        /// </summary>
        public double FrameRate => (double)_vi.raten / _vi.rated;

        /// <summary>
        /// The total number of video frames.
        /// </summary>
        public int TotalFrames => _vi.num_frames;

        /// <summary>
        /// Initializes a new AviSynth clip with the given function and argument.
        /// </summary>
        /// <param name="function">The function to execute in AviSynth.</param>
        /// <param name="argument">The parameter for the function.</param>
        /// <param name="forceColorspace">A colorspace that the clip should be converted to.</param>
        public AviSynthClip(string function, string argument, AviSynthColorspace forceColorspace)
        {
            _vi = new Native.AVSDLLVideoInfo();
            _avs = new IntPtr(0);
            _colorSpace = AviSynthColorspace.Unknown;
            _sampleType = AudioSampleType.Unknown;
            bool success = false;

            try
            {
                if (0 == Native.avswrapper_init_3(ref _avs, function, argument, ref _vi, ref _colorSpace, ref _sampleType, forceColorspace.ToString()))
                {
                    success = true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            if (success == false)
            {
                string errorText = GetLastError();
                Dispose(false);
                throw new AviSynthException(errorText);
            }
        }

        public void ReadAudioRaw(IntPtr addr, long offset, int count)
        {
            if (0 != Native.avswrapper_getaframe(_avs, addr, offset, count))
            {
                throw new AviSynthException(GetLastError());
            }
        }

        public void ReadAudioRaw(byte[] buffer, long offset, int count)
        {
            var h = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                ReadAudioRaw(h.AddrOfPinnedObject(), offset, count);
            }
            finally
            {
                h.Free();
            }
        }

        public byte[] ReadAudio(long offset, int count)
        {
            var buffer = new byte[count * ChannelsCount * BytesPerSample];
            ReadAudioRaw(buffer, offset, count);
            return buffer;
        }

        public void ReadFrameRaw(IntPtr addr, int stride, int frame)
        {
            if (0 != Native.avswrapper_getvframe(_avs, addr, stride, frame))
            {
                throw new AviSynthException(GetLastError());
            }
        }

        public Bitmap ReadFrame(int frame)
        {
            // Prepare the bitmap
            var bmp = new Bitmap(VideoWidth, VideoHeight, PixelFormat.Format24bppRgb);
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);
            try
            {
                // Get the address of the first line
                IntPtr ptr = bmpData.Scan0;
                // Read data
                ReadFrameRaw(ptr, bmpData.Stride, frame);
            }
            finally
            {
                // Unlock the bits.
                bmp.UnlockBits(bmpData);
            }
            bmp.RotateFlip(RotateFlipType.Rotate180FlipX);
            return bmp;
        }

        private string GetLastError()
        {
            const int errlen = 1024;
            var sb = new StringBuilder(errlen);
            sb.Length = Native.avswrapper_getlasterror(_avs, sb, errlen);
            return sb.ToString();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects)
                }

                // Free unmanaged resources (unmanaged objects)
                Native.avswrapper_destroy(ref _avs);
                _avs = IntPtr.Zero;

                disposedValue = true;
            }
        }

        ~AviSynthClip()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
