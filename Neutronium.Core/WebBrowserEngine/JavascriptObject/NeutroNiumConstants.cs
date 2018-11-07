namespace Neutronium.Core.WebBrowserEngine.JavascriptObject
{
    public static class NeutroniumConstants
    {
        public static string ObjectId { get; } = "_MappedId";
        public static string ReadOnlyFlag { get; } = "__readonly__";
        public static string ObjectIdTemplate { get; } = $"{{{{{nameof(NeutroniumConstants)}.{nameof(ObjectId)}}}}}";
        public static string ReadOnlyFlagTemplate { get; } = $"{{{{{nameof(NeutroniumConstants)}.{nameof(ReadOnlyFlag)}}}}}";
    }
}
