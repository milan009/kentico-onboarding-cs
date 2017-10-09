using System;

namespace ListApp.Contracts.Models
{
    public class ListItem
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? LastModified { get; set; }
    }
}