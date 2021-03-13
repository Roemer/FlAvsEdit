using System;

namespace AviSynthCsharpWrapper
{
    public static class AviSynthWrapper
    {
        /// <summary>
        /// Gets the AviSynth interface version of the AviSynthWrapper.dll.
        /// </summary>
        /// <returns>The version number of the interface.</returns>
        public static int GetAviSynthWrapperInterfaceVersion()
        {
            int iVersion = 0;
            try
            {
                int iResult = Native.avswrapper_getinterfaceversion(ref iVersion);
            }
            catch (Exception) { }
            return iVersion;
        }

        public static AviSynthClip CreateClipFromText(string aviSynthScriptText)
        {
            return new AviSynthClip("Eval", aviSynthScriptText, AviSynthColorspace.RGB24);
        }

        public static AviSynthClip CreateClipFromFile(string aviSynthFilePath)
        {
            return new AviSynthClip("Import", aviSynthFilePath, AviSynthColorspace.RGB24);
        }
    }
}
