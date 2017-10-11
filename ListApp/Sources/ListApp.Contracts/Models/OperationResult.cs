using System;

namespace ListApp.Contracts.Models
{
    public class OperationResult
    {
        public static readonly OperationResult Failed = new OperationResult(false, null);

        public static OperationResult CreateSuccessfulResult(ListItem item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return new OperationResult(true, item);
        }

        public bool Found { get; }
        public ListItem Item { get; }

        private OperationResult(bool found, ListItem item)
        {
            Found = found;
            Item = item;
        }
    }
}
