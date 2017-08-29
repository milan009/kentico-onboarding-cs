using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ListApp.Api.Models
{
    public class ListItem : IEquatable<ListItem>
    {
        private readonly Guid _id;

        public Guid Id
        {
            get { return _id; }

            /*  A public setter is required to allow XML serialization, yet _id has to be readonly
             *  for GetHashCode() and equality comparison to work correctly and behave as expected.
             *  Thus, an empty setter has been added to accomodate for that issue. Blame Microsoft.
             *  SO link: https://stackoverflow.com/questions/37702489/webapi-v2-xmlformatter-not-serializing-get-only-properties */

            set { }
        }

        public string Text { get; set; }

        // Paramless constructor is required for model binding
        public ListItem() { }

        // Constructor with parameters is required for unit testing
        public ListItem(Guid id, string text)
        {
            _id = id;
            Text = text;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((ListItem)obj);
        }

        public bool Equals (ListItem other)
        {
            return other != null && Id.Equals(other.Id) && string.Equals(Text, other.Text);
        }
    }
}
 
 