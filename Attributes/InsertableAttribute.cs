using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLibrary.Attributes
{
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]
    public class InsertableAttribute : Attribute
    {
        public string? alias { get; set; }

        public InsertableAttribute() { }

        public InsertableAttribute(string alias)
        {
            this.alias = alias; 
        }

    }
}
