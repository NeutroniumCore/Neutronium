using System;
using Neutronium.Core.Exceptions;
using Neutronium.Core.Infra;
using Neutronium.Core.JavascriptEngine.JavascriptObject;

namespace Neutronium.Core.Extension
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
                throw ExceptionHelper.Get($"Unexpected Error creating enum: {ienum}");
            }
        }
    }
}
