using NUnit.Framework.Constraints;

namespace ListApp.Tests.Base
{
    internal static class EqualConstraintExtensions
    {
        public static EqualConstraint UsingListItemComparer(this EqualConstraint constraint) 
            => constraint.Using(ListItemEqualityComparer.Instance);
    }
}
