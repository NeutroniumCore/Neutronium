using Chromium;
using Chromium.Event;
using Neutronium.Core;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Resources;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    public class PackUriResourceHandler : CfxResourceHandler
    {
        private static readonly Regex _PackUrl = new Regex(@"^pack:\/\/", RegexOptions.Compiled);
        private static readonly Regex _OldPackUrl = new Regex(@"^pack:\/\/application:,,,\/", RegexOptions.Compiled);
        private const string Prefix = @"pack://application:,,,/";
        private static readonly ConcurrentDictionary<ulong,PackUriResourceHandler> _PackUriResourceHandlers = new ConcurrentDictionary<ulong, PackUriResourceHandler>();

        private readonly string _Url;
        private readonly ulong _Id;
        private readonly StreamResourceInfo _StreamResourceInfo;
        private readonly IWebSessionLogger _Logger;

        public PackUriResourceHandler(CfxRequest request, IWebSessionLogger logger)
        {
            _Url = request.Url;
            _Id = request.Identifier;
            _Logger = logger;
            var uri = UpdateUrl(_Url);
            try
            {
                _StreamResourceInfo = System.Windows.Application.GetResourceStream(uri);
            }
            catch (Exception exception)
            {
                _Logger?.Error(() => $"Unable to find pack ressource:{_Url} exception:{exception}");
            }

            GetResponseHeaders += PackUriResourceHandler_GetResponseHeaders;
            ReadResponse += PackUriResourceHandler_ReadResponse;
            ProcessRequest += PackUriResourceHandler_ProcessRequest;
            _PackUriResourceHandlers.TryAdd(_Id, this);
        }

        internal static string UpdateLoadUrl(string url)
        {
            return _OldPackUrl.Replace(url, "pack://");
        }

        private static Uri UpdateUrl(string url)
        {
            var newUrl = _PackUrl.Replace(url, Prefix);
            return new Uri(newUrl);
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

            var stream = _StreamResourceInfo.Stream;
            var buffer = new byte[readResponse.BytesToRead];
            var bytesRead = stream.Read(buffer, 0, readResponse.BytesToRead);
            System.Runtime.InteropServices.Marshal.Copy(buffer, 0, readResponse.DataOut, bytesRead);
            readResponse.BytesRead = bytesRead;
            readResponse.SetReturnValue(true);

            if (stream.Position != stream.Length)
                return;

            Clean();
            _Logger?.Info($"Loaded: {_Url}");
        }

        private void Clean()
        {
            _PackUriResourceHandlers.TryRemove(_Id, out _);
            _StreamResourceInfo?.Stream.Dispose();
        }

        private void PackUriResourceHandler_ProcessRequest(object sender, CfxProcessRequestEventArgs processRequest)
        {
            processRequest.Callback.Continue();
            processRequest.SetReturnValue(true);
        }

        public override string ToString()
        {
            return _Url.ToString();
        }

        private string GetMineType()
        {
            var extension = Path.GetExtension(_Url).Replace(".", "");
            return CfxRuntime.GetMimeType(extension);
        }
    }
}