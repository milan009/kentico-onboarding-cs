using System;
using ListApp.Api.Models;

namespace ListApp.Api.Utils
{
    public static class Constants
    {
        private static readonly Guid Guid1 = Guid.Parse("00000000-0000-0000-0000-000000000000");
        private static readonly Guid Guid2 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        private static readonly Guid Guid3 = Guid.Parse("00000000-0000-0000-0000-000000000002");
        public static readonly Guid NonExistingItemGuid = Guid.Parse("00000000-0000-0000-0000-000000000003");

        public static ListItem[] MockListItems => new []
        {
            new ListItem {Id = Guid1, Text = "Stretch correctly"},
            new ListItem {Id = Guid2, Text = "Make a coffey"},
            new ListItem {Id = Guid3, Text = "Take over the world"}
        };

        public static ListItem CreatedListItem => new ListItem
        {
            Id = NonExistingItemGuid,
            Text = "Create another ListItem item!"
        };
    }
}