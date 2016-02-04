using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using MVVM.HTML.Core.Infra;

namespace HTML_WPF.Component
{
    /// <summary>
    /// IHTMLEngineFactory implementation
    /// </summary>
    public class HTMLEngineFactory : IHTMLEngineFactory
    {
        private IDictionary<string, IWPFWebWindowFactory> _Dic = new Dictionary<string, IWPFWebWindowFactory>();

        /// <summary>
        /// Access the singleton IHTMLEngineFactory
        /// </summary>
        public static IHTMLEngineFactory Engine
        {
            get; internal set;
        }

        static HTMLEngineFactory()
        {
            Engine = new HTMLEngineFactory();
        }


        public IWPFWebWindowFactory Resolve(string EngineName)
        {
            if (_Dic.Count == 1)
            {
                var res = _Dic.First().Value;
                if (!string.IsNullOrEmpty(EngineName) && (res.Name != EngineName))
                {
                    Trace.WriteLine(string.Format("Name mismatch in IWPFWebWindowFactory resoluction {0} vs {1}", EngineName, res.Name));
                }
                return res;
            }

            IWPFWebWindowFactory myres = null;
            _Dic.TryGetValue(EngineName, out myres);
            return myres;
        }

        public void Register(IWPFWebWindowFactory iWPFWebWindowFactory)
        {
            _Dic.Add(iWPFWebWindowFactory.Name, iWPFWebWindowFactory);
        }

        public void Dispose()
        {
            _Dic.Values.ForEach(fact => fact.Dispose());
            _Dic.Clear();
        }
    }
}
