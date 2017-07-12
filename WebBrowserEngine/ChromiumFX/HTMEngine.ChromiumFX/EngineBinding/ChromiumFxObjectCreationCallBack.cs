using Chromium.Remote;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding
{
    internal class ChromiumFxObjectCreationCallBack
    {
        private List<CfrV8Value[]> _Result = new List<CfrV8Value[]>();

        internal CfrV8Handler Handler { get; }

        internal ChromiumFxObjectCreationCallBack()
        {
            Handler = new CfrV8Handler();
            Handler.Execute += (_, e) =>
            {
                _Result.Add(e.Arguments);
            };
        }

        public CfrV8Value[] GetLastArguments()
        {
            var result = (_Result.Count == 1)? _Result[0] : _Result.SelectMany(item => item).ToArray();
            _Result.Clear();
            return result;
        }
    }
}
