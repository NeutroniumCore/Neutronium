namespace Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext
{
    public interface ITestHtmlProvider 
    {
        string GetHtlmPath(TestContext context);

        string GetRelativeHtlmPath(TestContext context);
    }
}
