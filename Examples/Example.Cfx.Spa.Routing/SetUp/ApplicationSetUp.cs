using System;

namespace Example.Cfx.Spa.Routing.SetUp
{
    public class ApplicationSetUp
    {
        public bool Debug => Mode != ApplicationMode.Production;
        public ApplicationMode Mode { get; }
        public Uri Uri { get; }

        public ApplicationSetUp(ApplicationMode mode, Uri uri)
        {
            Mode = mode;
            Uri = uri;
        }
    }
}
