using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.CEFGlue.Binding.HTMLBinding.V8JavascriptObject
{
    public interface IJavascriptObject : IDisposable
    {
        bool HasRelevantId();

        uint GetID();

        bool IsUndefined { get;  }

        bool IsNull { get; }

        bool IsObject { get; }

        bool IsArray { get;  }

        bool IsString { get; }

        bool IsNumber { get; }

        bool IsDate { get; }

        bool IsBool { get; }

        int GetArrayLength();

        bool HasValue(string attributename);

        IJavascriptObject Invoke(string iFunctionName, IWebView iContext, params IJavascriptObject[] iparam);

        Task<IJavascriptObject> InvokeAsync(string iFunctionName, IWebView iContext, params IJavascriptObject[] iparam);

        void Bind(string iFunctionName, IWebView iContext, Action<string, IJavascriptObject, IJavascriptObject[]> action);

        void SetValue(int position, IJavascriptObject element);

        void SetValue(string AttributeName, IJavascriptObject element, CreationOption ioption = CreationOption.None);

        string GetStringValue();

        double GetDoubleValue();

        bool GetBoolValue();

        DateTime GetDateValue();

        int GetIntValue();

        IJavascriptObject ExecuteFunction();

        IJavascriptObject GetValue(string ivalue);

        IJavascriptObject GetValue(int ivalue);

        IJavascriptObject[] GetArrayElements();
    }
}
