using System;

namespace ListApp.Contracts.Models
{
    public class ListItemDbOperationResult
    {
        public static readonly ListItemDbOperationResult Failed = new ListItemDbOperationResult(false, null);

        public static ListItemDbOperationResult CreateSuccessfulResult(ListItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return new ListItemDbOperationResult(true, item);
        }

        public bool Found { get; }
        public ListItem Item { get; }

        private ListItemDbOperationResult(bool found, ListItem item)
        {
            Found = found;
            Item = item;
        }
    }
}