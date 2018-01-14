using Neutronium.Core.Binding.GlueObject;

namespace Neutronium.Core.Extension
{
    public static class JsGlueExtension
    {
        public static string AsCircularVersionedJson(this IJsCsGlue glue, int version = 2)
        {
            var descriptionBuilder = GetConventionedBuilder();
            glue.BuilString(descriptionBuilder);
            if (glue.Type == JsCsGlueType.Object)
                descriptionBuilder.Prepend($@"{(descriptionBuilder.StringLength > 2 ? "," : "")}""version"":{version}");
            return descriptionBuilder.BuildString();
        }

        public static string AsCircularJson(this IJsCsGlue glue)
        {
            var descriptionBuilder = GetConventionedBuilder();
            glue.BuilString(descriptionBuilder);
            return descriptionBuilder.BuildString();
        }

        private static DescriptionBuilder GetConventionedBuilder() => new DescriptionBuilder("cmd({0})");
    }
}
