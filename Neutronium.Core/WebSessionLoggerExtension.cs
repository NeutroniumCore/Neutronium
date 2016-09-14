using Neutronium.Core.Infra;

namespace Neutronium.Core
{
    public static class WebSessionLoggerExtension
    {
        public static IWebSessionLogger Add(this IWebSessionLogger logger1, IWebSessionLogger logger2)
        {
            return new AddLoger(logger1, logger2);
        }
    }
}
