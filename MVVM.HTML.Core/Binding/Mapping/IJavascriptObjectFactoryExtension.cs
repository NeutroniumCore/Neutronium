using MVVM.HTML.Core.V8JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.Exceptions;

namespace MVVM.HTML.Core.Binding
{
    public static class IJavascriptObjectFactoryExtension
    {
        public static IJavascriptObject CreateEnum(this IJavascriptObjectFactory @this, Enum ienum)
        {
            try
            {
                IJavascriptObject res = @this.CreateObject(string.Format("new Enum('{0}',{1},'{2}','{3}')",
                    ienum.GetType().Name, Convert.ToInt32(ienum), ienum.ToString(), ienum.GetDescription()));

                if ((res == null) || (!res.IsObject))
                    throw ExceptionHelper.NoKoExtension();

                return res;
            }
            catch
            {
                throw ExceptionHelper.NoKoExtension();
            }
        }
    }
}
