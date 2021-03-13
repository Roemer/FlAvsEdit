using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace FlAvsEdit.Gui.Core
{
    /// <summary>
    /// Static class for getting embedded or associated resources.
    /// </summary>
    public static class ResourceGetter
    {
        /// <summary>
        /// Gets an embedded resource as string.
        /// </summary>
        /// <example>
        /// GetEmbeddedResource("FlAvsEdit.Gui.Resources.AviSynthSyntax.xshd")
        /// </example>
        public static string GetEmbeddedResource(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            string result = reader.ReadToEnd();
            return result;
        }

        /// <summary>
        /// Gets an associated resource as string.
        /// </summary>
        /// <example>
        /// GetContentResource("/Resources/AviSynthSyntax.xshd")
        /// </example>
        public static string GetContentResource(string resourceUrl)
        {
            var resourceName = new Uri(resourceUrl, UriKind.Relative);
            using var stream = Application.GetContentStream(resourceName).Stream;
            using var reader = new StreamReader(stream);
            string result = reader.ReadToEnd();
            return result;
        }
    }
}
