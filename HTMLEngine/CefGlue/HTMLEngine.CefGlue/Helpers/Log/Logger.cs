namespace HTMLEngine.CefGlue.Helpers.Log
{
    public static class Logger
    {
        public static ILogger Log { get; set; }

        static Logger()
        {
            Log = new NoLogger();
        }
    }
}
