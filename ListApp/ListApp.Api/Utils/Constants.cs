using System;
using System.Collections.Generic;
using ListApp.Api.Models;

namespace ListApp.Api.Utils
{
    public static class Constants
    {
        public static Guid Guid1 = Guid.Parse("00000000-0000-0000-0000-000000000000");
        public static Guid Guid2 = Guid.Parse("00000000-0000-0000-0000-000000000001");
        public static Guid Guid3 = Guid.Parse("00000000-0000-0000-0000-000000000002");
        public static Guid NonExistingItemGuid = Guid.Parse("00000000-0000-0000-0000-000000000003");

        public static IEnumerable<ListItem> MockListItems => new List<ListItem>
        {
            new ListItem {Id = Guid1, Text = "Stretch correctly"},
            new ListItem {Id = Guid2, Text = "Make a coffey"},
            new ListItem {Id = Guid3, Text = "Take over the world"}
        };
    }
}