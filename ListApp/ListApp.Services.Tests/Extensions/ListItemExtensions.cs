using ListApp.Contracts.Models;

namespace ListApp.Services.Tests.Extensions
{
    internal static class ListItemExtensions
    {
        internal static bool IsEqualTo(this ListItem x, ListItem y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x == null || y == null) return false;
            return x.Id == y.Id && x.Text == y.Text
                   && x.LastModified == y.LastModified
                   && x.Created == y.Created;
        }
    }
}
