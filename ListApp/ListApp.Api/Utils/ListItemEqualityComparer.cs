using System.Collections.Generic;
using ListApp.Api.Models;

namespace ListApp.Api.Utils
{
    /// <summary>
    /// Equality comparer used for testing equality of <see cref="ListItem"/> models
    /// It does NOT implement its own GetHashCode() method - use ONLY
    /// for comparing two <see cref="ListItem"/> models!
    /// </summary>
    public class ListItemEqualityComparer : IEqualityComparer<ListItem>
    {
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