using System.Linq;
using JetBrains.Annotations;
using JetBrains.Metadata.Reader.API;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.CSharp.Tree;

namespace Roflcopter.Plugin.Utilities
{
    public static class AttributeExtensions
    {
        [Pure]
        public static bool IsAttributeOrDerivedFrom(this IAttribute attribute, [ItemNotNull] params IClrTypeName[] typeNamesToTest)
        {
            if (attribute.TypeReference != null)
            {
                if (attribute.TypeReference.Resolve().DeclaredElement is ITypeElement typeElement)
                {
                    return typeNamesToTest.Any(typeNameToTest =>
                    {
#if RS20183
                        var interfaceTypeToTest = TypeFactory.CreateTypeByCLRName(typeNameToTest, attribute.PsiModule);
#else
                        var interfaceTypeToTest = TypeFactory.CreateTypeByCLRName(typeNameToTest, NullableAnnotation.Unknown, attribute.PsiModule);
#endif

                        return typeElement.IsDescendantOf(interfaceTypeToTest.GetTypeElement());
                    });
                }
            }

            return false;
        }
    }
}
