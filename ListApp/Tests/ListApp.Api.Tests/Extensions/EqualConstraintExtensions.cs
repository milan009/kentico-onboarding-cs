using System.Collections.Generic;
using NUnit.Framework.Constraints;
using ListApp.Contracts.Models;

namespace ListApp.Api.Tests.Extensions
{
    public static class EqualConstraintExtensions
    {
        public static EqualConstraint UsingListItemComparer(this EqualConstraint constr) => constr.Using(ListItemEqualityComparer.Instance);

        private class ListItemEqualityComparer : IEqualityComparer<ListItem>
        {
            private ListItemEqualityComparer() { }

            public static ListItemEqualityComparer Instance { get; }
                = new ListItemEqualityComparer();

            public bool Equals(ListItem x, ListItem y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x == null || y == null) return false;
                return x.Id == y.Id && x.Text == y.Text;
            }

            public int GetHashCode(ListItem obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
