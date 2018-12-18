using Chromium;
using Chromium.Event;
using Neutronium.Core;
using System;
using System.IO;
using System.Windows.Resources;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    public class PackUriResourceHandler : CfxResourceHandler
    {
        private readonly Uri _Uri;
        private readonly StreamResourceInfo _StreamResourceInfo;

        public PackUriResourceHandler(Uri packUri, IWebSessionLogger logger)
        {
            _Uri = packUri;
            try
            {
                _StreamResourceInfo = System.Windows.Application.GetResourceStream(packUri);
            }
            catch (Exception exception)
            {
                logger?.Error(() => $"Unable to find pack ressource:{packUri} exception:{exception}");
            }

            GetResponseHeaders += PackUriResourceHandler_GetResponseHeaders;
            ReadResponse += PackUriResourceHandler_ReadResponse;
            ProcessRequest += PackUriResourceHandler_ProcessRequest;
            CanGetCookie += PackUriResourceHandler_CanGetCookie;
            CanSetCookie += PackUriResourceHandler_CanSetCookie;
        }

        private void PackUriResourceHandler_GetResponseHeaders(object sender, CfxGetResponseHeadersEventArgs responeHeader)
        {
            var response = responeHeader.Response;
            if (_StreamResourceInfo == null)
            {
                response.Status = 404;
                response.StatusText = "Not Found";
                return;
            }

            responeHeader.ResponseLength = _StreamResourceInfo.Stream.Length;
            response.MimeType = GetMineType();
            response.Status = 200;
            response.StatusText = "OK";
        }

        private void PackUriResourceHandler_ReadResponse(object sender, CfxReadResponseEventArgs readResponse)
        {
            if (_StreamResourceInfo == null)
            {
                readResponse.SetReturnValue(false);
                return;
            }

            var buffer = new byte[readResponse.BytesToRead];
            var bytesRead = _StreamResourceInfo.Stream.Read(buffer, 0, readResponse.BytesToRead);
            System.Runtime.InteropServices.Marshal.Copy(buffer, 0, readResponse.DataOut, bytesRead);
            readResponse.BytesRead = bytesRead;
            readResponse.SetReturnValue(bytesRead > 0);
        }

        private void PackUriResourceHandler_ProcessRequest(object sender, CfxProcessRequestEventArgs processRequest)
        {
            processRequest.Callback.Continue();
            processRequest.SetReturnValue(true);
        }
        private void PackUriResourceHandler_CanSetCookie(object sender, CfxResourceHandlerCanSetCookieEventArgs e)
        {
            e.SetReturnValue(true);
        }

        private void PackUriResourceHandler_CanGetCookie(object sender, CfxCanGetCookieEventArgs e)
        {
            e.SetReturnValue(true);
        }

        public override string ToString()
        {
            return _Uri.ToString();
        }

        private string GetMineType()
        {
            var extension = Path.GetExtension(_Uri.AbsoluteUri).Replace(".", "");
            return CfxRuntime.GetMimeType(extension);
        }
    }
}