using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLibrary.Attributes
{
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]
    public class UpdateableAttribute : Attribute
    {
        public string? alias { get; set; }

        public UpdateableAttribute() { }

        public UpdateableAttribute(string? alias)
        {
            this.alias = alias;
        }

    }
}
