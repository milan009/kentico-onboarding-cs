﻿using System.Collections.Generic;
using ListApp.Contracts.Models;
using NUnit.Framework.Constraints;

namespace ListApp.Tests.Extensions
{
    internal static class EqualConstraintExtensions
    {
        public static EqualConstraint UsingListItemComparer(this EqualConstraint constraint) => constraint.Using(ListItemEqualityComparer.Instance);
        
        internal class ListItemEqualityComparer : IEqualityComparer<ListItem>
        {
            private ListItemEqualityComparer() { }

            public static ListItemEqualityComparer Instance { get; }
                = new ListItemEqualityComparer();

            public bool Equals(ListItem x, ListItem y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (x == null || y == null) return false;
                return x.Id == y.Id && x.Text == y.Text
                       && x.LastModified == y.LastModified
                       && x.Created == y.Created;
            }

            public int GetHashCode(ListItem obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}