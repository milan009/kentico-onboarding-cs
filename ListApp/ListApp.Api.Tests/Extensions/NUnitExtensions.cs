﻿using ListApp.Api.Tests.Comparers;
using NUnit.Framework.Constraints;

namespace ListApp.Api.Tests.Extensions
{
    public static class NUnitExtensions
    {
        public static EqualConstraint UsingListItemComparer(this EqualConstraint constr)
        {
            return constr.Using(ListItemEqualityComparer.Instance);
        }
    }
}
