namespace IntegratedTest.JavascriptUIFramework
{
    public interface ITestHtmlProvider 
    {
        string GetHtlmPath(TestContext context, bool allowInitialScriptInjection);
    }
}
