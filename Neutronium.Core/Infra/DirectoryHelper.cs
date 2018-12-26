using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Neutronium.Core.Infra
{
    public static class DirectoryHelper
    {
        private static readonly Regex _Path = new Regex(@"\\bin(\\x86|\\x64)?\\(Debug|Release)$", RegexOptions.Compiled);

        public static string GetCurrentDirectory()
        {
            return _Path.Replace(Directory.GetCurrentDirectory(), String.Empty);
        }
    }
}
