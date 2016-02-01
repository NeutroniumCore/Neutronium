using MVVM.HTML.Core.V8JavascriptObject;


namespace MVVM.HTML.Core.HTMLBinding
{
    public interface IJavascriptMapper
    {
        void MapFirst(IJavascriptObject iRoot);

        void Map(IJavascriptObject iFather, string att, IJavascriptObject iChild);

        void MapCollection(IJavascriptObject iFather, string att, int index, IJavascriptObject iChild);

        void EndMapping(IJavascriptObject iRoot);
    }
}
