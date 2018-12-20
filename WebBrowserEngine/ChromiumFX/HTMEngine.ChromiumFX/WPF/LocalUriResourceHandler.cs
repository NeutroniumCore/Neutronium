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
    internal class LocalUriResourceHandler : CfxResourceHandler
    {
        private const string Prefix = @"pack://application:,,,/";
        private static readonly Regex _LocalUrl = new Regex(@"^local:\/\/", RegexOptions.Compiled);
        private static readonly ConcurrentDictionary<ulong, LocalUriResourceHandler> _PackUriResourceHandlers = new ConcurrentDictionary<ulong, LocalUriResourceHandler>();

        private string Url => _Request.Url;
        private bool IsPrefetch => _Request.ResourceType == CfxResourceType.Prefetch;
        private string MineType
        {
            get
            {
                var extension = Path.GetExtension(Url).Replace(".", "");
                return CfxRuntime.GetMimeType(extension);
            }
        }

        private readonly StreamResourceInfo _StreamResourceInfo;
        private readonly IWebSessionLogger _Logger;
        private readonly CfxRequest _Request;

        internal static LocalUriResourceHandler FromPackUrl(CfxRequest request, IWebSessionLogger logger) 
        {
            return new LocalUriResourceHandler(request, logger);
        }

        internal static LocalUriResourceHandler FromLocalUrl(CfxRequest request, IWebSessionLogger logger)
        {
            return new LocalUriResourceHandler(request, logger, UpdateUrl(request.Url));
        }

        private LocalUriResourceHandler(CfxRequest request, IWebSessionLogger logger, Uri uriUrl = null)
        {
            _Request = request;
            _Logger = logger;
            var uri = uriUrl ?? new Uri(Url);
            _StreamResourceInfo = GetStreamResourceInfo(uri);

            GetResponseHeaders += PackUriResourceHandler_GetResponseHeaders;
            ReadResponse += PackUriResourceHandler_ReadResponse;
            ProcessRequest += PackUriResourceHandler_ProcessRequest;
            _PackUriResourceHandlers.TryAdd(_Request.Identifier, this);
        }

        private StreamResourceInfo GetStreamResourceInfo(Uri uri)
        {
            if (IsPrefetch)
            {
                return null;
            }
            try 
            {
               return System.Windows.Application.GetResourceStream(uri);
            }
            catch
            {
                _Logger?.Warning(() => $"Unable to find pack ressource:{Url}");
            }
            return null;
        }

        private static Uri UpdateUrl(string url)
        {
            var newUrl = _LocalUrl.Replace(url, Prefix);
            return new Uri(newUrl);
        }

        private void PackUriResourceHandler_GetResponseHeaders(object sender, CfxGetResponseHeadersEventArgs responseHeader)
        {
            var response = responseHeader.Response;
            if (IsPrefetch)
            {
                _Logger?.Info($"Loading ignored (prefetch): {Url}");
                responseHeader.ResponseLength = 0;
                SetSuccess(response);
                return;
            }

            if (_StreamResourceInfo == null)
            {
                response.Status = 404;
                response.StatusText = "Not Found";
                return;
            }

            responseHeader.ResponseLength = _StreamResourceInfo.Stream.Length;
            SetSuccess(response);
        }

        private void SetSuccess(CfxResponse response)
        {
            response.MimeType = MineType;
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
            _Logger?.Info($"Loaded: {Url}");
        }

        private void Clean()
        {
            _PackUriResourceHandlers.TryRemove(_Request.Identifier, out _);
            _StreamResourceInfo?.Stream.Dispose();
        }

        private void PackUriResourceHandler_ProcessRequest(object sender, CfxProcessRequestEventArgs processRequest)
        {
            processRequest.Callback.Continue();
            processRequest.SetReturnValue(true);
        }

        public override string ToString()
        {
            return Url.ToString();
        }
    }
}