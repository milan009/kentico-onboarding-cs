using System;
using ListApp.Api.Models;

namespace ListApp.Api.Utils
{
    public static class Constants
    {
        private static readonly ListItem Item1 = new ListItem { Id = Guid.Parse("00000000-0000-0000-0000-000000000000"), Text = "Stretch correctly" };
        private static readonly ListItem Item2 = new ListItem { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Text = "Make a coffey" };
        private static readonly ListItem Item3 = new ListItem { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Text = "Take over the world" };

        public static readonly Guid NonExistingItemGuid = Guid.Parse("00000000-0000-0000-0000-000000000003");

        public static readonly ListItem[] MockListItems = { Item1, Item2, Item3 };

        public static readonly ListItem CreatedListItem = new ListItem
        {
            Id = NonExistingItemGuid,
            Text = "Create another ListItem item!"
        };
    }
}