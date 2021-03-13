using System;
using System.IO;
using System.Text;

namespace AviSynthCsharpWrapper
{
    public static class AviSynthHelpers
    {
        public static byte[] WrapInWav(AviSynthClip clip, byte[] audioBytes, int sampleCount)
        {
            var ms = new MemoryStream();
            var headerBytes = CreateWavHeader(clip, sampleCount);
            ms.Write(headerBytes, 0, headerBytes.Length);
            ms.Write(audioBytes, 0, audioBytes.Length);
            return ms.ToArray();
        }

        public static byte[] CreateWavHeader(AviSynthClip clip, int sampleCount)
        {
            return CreateWavHeader(clip.SampleType == AudioSampleType.FLOAT, clip.ChannelsCount, clip.BitsPerSample, clip.AudioSampleRate, sampleCount);
        }

        public static byte[] CreateWavHeader(bool isFloatingPoint, short channelCount, short bitDepth, int sampleRate, int sampleCount)
        {
            var headerSize = 12 + 24;
            var dataHeaderSize = 8;

            var headerBytes = new byte[headerSize + dataHeaderSize];
            var stream = new MemoryStream(headerBytes);

            //// RIFF header (12 bytes)
            // Chunk ID
            stream.Write(Encoding.ASCII.GetBytes("RIFF"), 0, 4);
            // Chunk size
            stream.Write(BitConverter.GetBytes(((bitDepth / 8) * sampleCount) + headerSize), 0, 4);
            // RIFF type
            stream.Write(Encoding.ASCII.GetBytes("WAVE"), 0, 4);

            //// Format header / Chunk 1 (24 bytes)
            // Chunk ID
            stream.Write(Encoding.ASCII.GetBytes("fmt "), 0, 4);
            // Chunk size
            stream.Write(BitConverter.GetBytes(16), 0, 4);
            // Format code (floating point (3) or PCM (1)). Any other format indicates compression.
            stream.Write(BitConverter.GetBytes((ushort)(isFloatingPoint ? 3 : 1)), 0, 2);
            // Number of channels
            stream.Write(BitConverter.GetBytes(channelCount), 0, 2);
            // Sample rate (blocks per second)
            stream.Write(BitConverter.GetBytes(sampleRate), 0, 4);
            // Data rate (bytes per second)
            stream.Write(BitConverter.GetBytes(sampleRate * channelCount * (bitDepth / 8)), 0, 4);
            // Block align / Data block size (bytes)
            stream.Write(BitConverter.GetBytes((ushort)channelCount * ((bitDepth + 7) / 8)), 0, 2);
            // Bits per sample
            stream.Write(BitConverter.GetBytes(bitDepth), 0, 2);

            //// Data chunk (Chunk 2) (8 bytes)
            // Chunk ID
            stream.Write(Encoding.ASCII.GetBytes("data"), 0, 4);
            // Chunk size
            stream.Write(BitConverter.GetBytes(channelCount * (bitDepth / 8) * sampleCount), 0, 4);

            return headerBytes;
        }
    }
}
