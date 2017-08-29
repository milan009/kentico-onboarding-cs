using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ListApp.Api.Models
{
    public class ListItem
    {
        public Guid Id { get;set; }
        public string Text { get; set; }
    }
}
 
 