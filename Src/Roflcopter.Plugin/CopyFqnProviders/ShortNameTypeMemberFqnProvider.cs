using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.DataContext;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Features.Environment.CopyFqn;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.DataContext;
#if RESHARPER20171
using JetBrains.UI.Avalon.TreeListView;

#else
using JetBrains.UI.Controls.TreeListView;

#endif

namespace Roflcopter.Plugin.CopyFqnProviders
{
    /// <summary>
    /// A provider for <see cref="CopyFqnAction"/> which returns <c>[type name].[member]</c>.
    /// </summary>
    [SolutionComponent]
    public class ShortNameTypeMemberFqnProvider : IFqnProvider
    {
        public bool IsApplicable([NotNull] IDataContext dataContext)
        {
            // Will be called in the CopyFqnAction to determine if _any_ provider has sth. to provide.

            var typeMembers = GetTypeMembers(dataContext);

            return typeMembers.Any();
        }

        public IEnumerable<PresentableFqn> GetSortedFqns([NotNull] IDataContext dataContext)
        {
            var typeMembers = GetTypeMembers(dataContext);

            foreach (var typeMember in typeMembers)
            {
                var containingType = typeMember.GetContainingType();

                if (containingType != null)
                {
                    var containingTypePath = containingType.PathName(x => x.ShortName, x => x.GetContainingType());

                    yield return new PresentableFqn($"{containingTypePath}.{typeMember.ShortName}");
                }
            }
        }

        public int Priority => -10; // The lower the value, the _higher_ it is ranked. Yes, really.

        private static IEnumerable<ITypeMember> GetTypeMembers(IDataContext dataContext)
        {
            var data = dataContext.GetData(PsiDataConstants.DECLARED_ELEMENTS);

            if (data == null)
                return new ITypeMember[0];

            return data.OfType<ITypeMember>();
        }
    }
}
