using System;
using System.ComponentModel.DataAnnotations;

namespace ListApp.Api.Models
{
    public class ListItem
    {
        [Required]
        public Guid Id { get;set; }
        [Required]
        public string Text { get; set; }
    }
}
 
 