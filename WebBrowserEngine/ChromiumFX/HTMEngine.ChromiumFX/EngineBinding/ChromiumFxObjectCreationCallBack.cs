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

        public IEnumerable<CfrV8Value> GetLastArguments() 
        {
            var result = GetFromList(_Result); 
            _Result.Clear();
            return result;
        }

        private static IEnumerable<CfrV8Value> GetFromList(List<CfrV8Value[]> lists) 
        {
            if (lists.Count == 1)
                return lists[0];

            var copiedList = new List<CfrV8Value[]>(lists);
            return copiedList.SelectMany(l => l);
        }
    }
}
