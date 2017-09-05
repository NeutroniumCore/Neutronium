using System;
using Neutronium.Core.Exceptions;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Extension
{
    public static class JavascriptObjectFactoryExtension
    {
        public static IJavascriptObject CreateEnum(this IJavascriptObjectFactory @this, Enum @enum)
        {
            try
            {
                var res = @this.CreateObject($"new Enum('{@enum.GetType().Name}',{Convert.ToInt32(@enum)},'{@enum.ToString()}','{@enum.GetDescription()}')");

                if ((res == null) || (!res.IsObject))
                    throw ExceptionHelper.GetUnexpected();

                return res;
            }
            catch
            {
                throw ExceptionHelper.Get($"Unexpected Error creating enum: {@enum}");
            }
        }
    }
}
