using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.DataContext;
using JetBrains.Application.Settings;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Features.Environment.CopyFqn;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.DataContext;
using JetBrains.Util.Special;

namespace Roflcopter.Plugin.CopyFqnProviders
{
    /// <summary>
    /// A provider for <see cref="CopyFqnAction"/> which returns <c>[type name].[member]</c>.
    /// </summary>
    [SolutionComponent]
    public class ShortNameTypeMemberFqnProvider : IFqnProvider
    {
        private readonly ISolution _solution;

        public ShortNameTypeMemberFqnProvider(ISolution solution)
        {
            _solution = solution;
        }

        public bool IsApplicable([NotNull] IDataContext dataContext)
        {
            // Will be called in the CopyFqnAction to determine if _any_ provider has sth. to provide.

            return IsEnabled() && GetTypeMembers(dataContext).Any();
        }

        public IEnumerable<PresentableFqn> GetSortedFqns([NotNull] IDataContext dataContext)
        {
            if (IsEnabled())
            {
                var typeMembers = GetTypeMembers(dataContext);

                foreach (var typeMember in typeMembers)
                {
                    var typeMemberAndParents =
                        GeneralUtil.SelfAndParentReversedPath<IClrDeclaredElement>(typeMember, x => x.GetContainingType()).Reverse();

                    yield return new PresentableFqn(string.Join(".", typeMemberAndParents.Select(x => x.ShortName)));
                }
            }
        }

        public int Priority => -10; // The lower the value, the _higher_ it is ranked. Yes, really.

        private bool IsEnabled() => _solution.GetSettingsStore().GetValue((CopyFqnProvidersSettings s) => s.EnableShortNames);

        private static IEnumerable<ITypeMember> GetTypeMembers(IDataContext dataContext)
        {
            var data = dataContext.GetData(PsiDataConstants.DECLARED_ELEMENTS);

            if (data == null)
                return new ITypeMember[0];

            return data.OfType<ITypeMember>();
        }
    }
}
