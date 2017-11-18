using System;
using System.IO;
using System.Windows.Resources;
using Chromium;
using Neutronium.Core;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    public class PackUriResourceHandler : CfxResourceHandler
    {
        public PackUriResourceHandler(Uri packUri, IWebSessionLogger logger)
        {
            StreamResourceInfo resInfo = null;
            try
            {
                resInfo = System.Windows.Application.GetResourceStream(packUri);
            }
            catch (Exception exception)
            {
                logger?.Error(() => $"Unable to find pack ressource:{packUri} exception:{exception}");
            }

            GetResponseHeaders += (s1, e1) =>
            {
                if (resInfo == null)
                {
                    e1.Response.Status = 404;
                    e1.Response.StatusText = "Not Found";
                    return;
                }

                e1.ResponseLength = resInfo.Stream.Length;
                e1.Response.MimeType = GetMineType(resInfo, packUri);
                e1.Response.Status = 200;
                e1.Response.StatusText = "OK";
            };

            ReadResponse += (s2, e2) =>
            {
                if (resInfo != null)
                {
                    var buffer = new byte[e2.BytesToRead];
                    var bytesRead = resInfo.Stream.Read(buffer, 0, e2.BytesToRead);
                    System.Runtime.InteropServices.Marshal.Copy(buffer, 0, e2.DataOut, bytesRead);
                    e2.BytesRead = bytesRead;
                }
                e2.SetReturnValue(true);
            };

            ProcessRequest += (s3, e3) =>
            {
                e3.Callback.Continue();
                e3.SetReturnValue(true);
            };
        }

        private static string GetMineType(StreamResourceInfo streamResourceInfo, Uri uri)
        {
            var type = streamResourceInfo.ContentType;
            if (!string.IsNullOrEmpty(type))
                return type;

            var extension = Path.GetExtension(uri.AbsoluteUri).Replace(".", "");
            return CfxRuntime.GetMimeType(extension);
        }
    }
}