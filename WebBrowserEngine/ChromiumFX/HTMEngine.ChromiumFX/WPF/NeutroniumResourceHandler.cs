using Chromium;
using Chromium.Event;
using Neutronium.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Resources;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    internal class NeutroniumResourceHandler : CfxResourceHandler
    {
        private const string SecureScheme = "https";
        private const string Host = "application";
        private const string PackPrefix = @"pack://application:,,,/";
        private static readonly Regex _HttpsUrl = new Regex(@"^https:\/\/application\/", RegexOptions.Compiled);
        private static readonly ConcurrentDictionary<ulong, NeutroniumResourceHandler> _PackUriResourceHandlers = new ConcurrentDictionary<ulong, NeutroniumResourceHandler>();

        private string Url => _Request.Url;
        private string LocalPath => _Uri.LocalPath;
        private bool IsPrefetch => _Request.ResourceType == CfxResourceType.Prefetch;
        private bool _Done = false;

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
        private readonly Uri _Uri;

        public static CfxResourceHandler FromHttpsUrl(CfxRequest request, IWebSessionLogger logger)
        {
            return new NeutroniumResourceHandler(request, logger, UpdateHttpsUrl(request.Url));
        }

        private static Uri UpdateHttpsUrl(string url)
        {
            var newUrl = _HttpsUrl.Replace(url, PackPrefix);
            return new Uri(newUrl);
        }

        public static string UpdatePackUrl(Uri path)
        {
            var newUri = new UriBuilder(path)
            {
                Host = Host,
                Scheme = SecureScheme
            };
            return newUri.ToString();
        }

        private NeutroniumResourceHandler(CfxRequest request, IWebSessionLogger logger, Uri uriUrl = null)
        {
            _Request = request;
            _Logger = logger;
            _Uri = uriUrl ?? new Uri(Url);
            _StreamResourceInfo = GetStreamResourceInfo(_Uri);

            GetResponseHeaders += PackUriResourceHandler_GetResponseHeaders;
            Read += PackUriResourceHandler_ReadResponse;
            Open += NeutroniumResourceHandler_Open;
            _PackUriResourceHandlers.TryAdd(_Request.Identifier, this);
        }

        private void NeutroniumResourceHandler_Open(object sender, CfxOpenEventArgs e)
        {
            e.HandleRequest = true;
            e.SetReturnValue(true);
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
                _Logger?.Warning(NotFound);
            }
            return null;
        }

        private void PackUriResourceHandler_GetResponseHeaders(object sender, CfxGetResponseHeadersEventArgs responseHeader)
        {
            var response = responseHeader.Response;
            if (IsPrefetch)
            {
                _Logger?.Info(Prefetch);
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
            response.SetHeaderMap(new List<string[]>
            {
                new[] { "Access-Control-Allow-Origin", "*"}
            });
        }

        private void PackUriResourceHandler_ReadResponse(object sender, CfxResourceHandlerReadEventArgs readResponse)
        {
            if (_StreamResourceInfo == null)
            {
                readResponse.SetReturnValue(false);
                return;
            }

            if (_Done)
            {
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
            _Logger?.Info(Loaded);
        }

        private string Loaded() => $"Loaded: {LocalPath}";
        private string Prefetch() => $"Loading ignored (prefetch): {LocalPath}";
        private string NotFound() => $"Unable to find pack resource:{LocalPath}";

        private void Clean()
        {
            _PackUriResourceHandlers.TryRemove(_Request.Identifier, out _);
            _StreamResourceInfo?.Stream.Dispose();
            _Done = true;
        }

        private void PackUriResourceHandler_ProcessRequest(object sender, CfxProcessRequestEventArgs processRequest)
        {
            processRequest.SetReturnValue(true);
            processRequest.Callback.Continue();
        }

        public override string ToString() => Url;
    }
}