using System;
using System.Threading.Tasks;

namespace Neutronium.WebBrowserEngine.ChromiumFx.Util
{
    internal class Resilient
    {
        private readonly Func<bool> _Do;
        private int _TimeOut = 100;

        public Resilient(Func<bool> @do)
        {
            _Do = @do;
        }

        public Resilient WithTimeOut(int timeOut)
        {
            _TimeOut = timeOut;
            return this;
        }

        public async Task StartIn(int firstTimeOut)
        {
            await Task.Delay(firstTimeOut);
            while (!_Do())
            {
                await Task.Delay(_TimeOut);
            }
        }
    }
}
