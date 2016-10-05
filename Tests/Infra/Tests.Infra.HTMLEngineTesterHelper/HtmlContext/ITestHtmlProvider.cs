namespace Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext
{
    public interface ITestHtmlProvider 
    {
        string GetHtmlPath(TestContext context);

        string GetRelativeHtmlPath(TestContext context);
    }
}
