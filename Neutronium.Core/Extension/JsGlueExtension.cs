using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Extension
{
    public static class JsGlueExtension
    {
        public static string AsCircularJson(this IJsCsGlue glue)
        {
            var descriptionBuilder = new DescriptionBuilder("cmd({0})");
            glue.BuilString(descriptionBuilder);
            if (glue.Type == JsCsGlueType.Object)
                descriptionBuilder.Prepend($@"{(descriptionBuilder.StringLength > 2 ? "," : "")}""version"":2");
            return descriptionBuilder.BuildString();
        }
    }
}
