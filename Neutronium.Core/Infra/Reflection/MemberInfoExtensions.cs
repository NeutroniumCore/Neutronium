using System;
using System.Linq;
using System.Reflection;

namespace Neutronium.Core.Infra.Reflection
{
    public static class MemberInfoExtensions
    {
        public static T GetAttribute<T>(this MemberInfo memberInfo) where T: Attribute
        {
            var attributes = memberInfo.GetCustomAttributes(typeof(T), false);
            return (T)attributes?.FirstOrDefault();
        }
    }
}
