using Chromium;
using Neutronium.Core;
using System;
using System.IO;
using System.Windows.Resources;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    public class PackUriResourceHandler : CfxResourceHandler
    {
        public PackUriResourceHandler(Uri packUri, IWebSessionLogger logger)
        {
            var resInfo = default(StreamResourceInfo);
            try
            {
                resInfo = System.Windows.Application.GetResourceStream(packUri);
            }
            catch (Exception exception)
            {
                logger?.Error(() => $"Unable to find pack ressource:{packUri} exception:{exception}");
            }

            GetResponseHeaders += (_, responeHeader) =>
            {
                var response = responeHeader.Response;
                if (resInfo == null)
                {
                    response.Status = 404;
                    response.StatusText = "Not Found";
                    return;
                }

                responeHeader.ResponseLength = resInfo.Stream.Length;         
                response.MimeType = GetMineType(packUri);
                response.Status = 200;
                response.StatusText = "OK";
            };

            ReadResponse += (_, readResponse) =>
            {
                if (resInfo == null)
                {
                    readResponse.SetReturnValue(false);
                    return;
                }

                var buffer = new byte[readResponse.BytesToRead];
                var bytesRead = resInfo.Stream.Read(buffer, 0, readResponse.BytesToRead);
                System.Runtime.InteropServices.Marshal.Copy(buffer, 0, readResponse.DataOut, bytesRead);
                readResponse.BytesRead = bytesRead;
                readResponse.SetReturnValue(true);
            };

            ProcessRequest += (_, processRequest) =>
            {
                processRequest.Callback.Continue();
                processRequest.SetReturnValue(true);
            };
        }

        private static string GetMineType(Uri uri)
        {
            var extension = Path.GetExtension(uri.AbsoluteUri).Replace(".", "");
            return CfxRuntime.GetMimeType(extension);
        }
    }
}