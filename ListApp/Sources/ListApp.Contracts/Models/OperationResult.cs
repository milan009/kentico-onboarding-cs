namespace ListApp.Contracts.Models
{
    public class OperationResult
    {
        public bool Found { get; }
        public ListItem Item { get; }

        public OperationResult(bool found, ListItem item)
        {
            Found = found;
            Item = item;
        }
    }
}
