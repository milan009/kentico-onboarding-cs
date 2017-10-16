using NUnit.Framework.Constraints;

namespace ListApp.Tests.Base
{
    public static class EqualConstraintExtensions
    {
        public static EqualConstraint UsingListItemComparer(this EqualConstraint constraint) 
            => constraint.Using(ListItemEqualityComparer.Instance);
    }
}
