using System;
using MVVM.HTML.Core.Exceptions;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.Extension
{
    public static class IJavascriptObjectFactoryExtension
    {
        public static IJavascriptObject CreateEnum(this IJavascriptObjectFactory @this, Enum ienum)
        {
            try
            {
                var res = @this.CreateObject($"new Enum('{ienum.GetType().Name}',{Convert.ToInt32(ienum)},'{ienum.ToString()}','{ienum.GetDescription()}')");

                if ((res == null) || (!res.IsObject))
                    throw ExceptionHelper.GetUnexpected();

                return res;
            }
            catch
            {
                throw ExceptionHelper.GetUnexpected();
            }
        }
    }
}
