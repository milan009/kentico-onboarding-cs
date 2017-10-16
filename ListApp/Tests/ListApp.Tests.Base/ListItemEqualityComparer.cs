using System.Collections.Generic;
using ListApp.Contracts.Models;

namespace ListApp.Tests.Base
{
    public class ListItemEqualityComparer : IEqualityComparer<ListItem>
    {
        private ListItemEqualityComparer() { }

        public static ListItemEqualityComparer Instance { get; }
            = new ListItemEqualityComparer();

        public bool Equals(ListItem x, ListItem y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            return IsListItemContentEqueal(x, y);
        }

        private bool IsListItemContentEqueal(ListItem a, ListItem b)
        {
            return a.Id == b.Id 
                && a.Text == b.Text
                && a.LastModified == b.LastModified
                && a.Created == b.Created;
        }

        public int GetHashCode(ListItem obj)
        {
            return obj.GetHashCode();
        }
    }
}