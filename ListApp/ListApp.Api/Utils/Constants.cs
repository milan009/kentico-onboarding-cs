using System;
using System.Collections.Generic;
using ListApp.Api.Models;

namespace ListApp.Api.Utils
{
    public static class Constants
    {
        public static IEnumerable<ListItem> MockListItems => new List<ListItem>
        {
            new ListItem {Id = Guid.Parse("00000000-0000-0000-0000-000000000000"), Text = "Stretch correctly"},
            new ListItem {Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Text = "Make a coffey"},
            new ListItem {Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Text = "Take over the world"}
        };
    }
}