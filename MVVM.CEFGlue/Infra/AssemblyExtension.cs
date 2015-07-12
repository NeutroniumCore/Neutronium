using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MVVM.CEFGlue.Infra
{
    public static class AssemblyExtension
    {
        public static string GetPath(this Assembly @this)
        {
            var lCodeBase = @this.CodeBase;
            var lUri = new UriBuilder(lCodeBase);
            var lPath = Uri.UnescapeDataString(lUri.Path);
            return System.IO.Path.GetDirectoryName(lPath);
        }
    }
}
