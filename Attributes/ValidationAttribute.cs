using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLibrary.Attributes
{
    [AttributeUsage(AttributeTargets.Property|AttributeTargets.Field)]
    public class ValidationAttribute : Attribute
    {
        public int minLength { get; set; } 
        public int maxLength { get; set; } 
        public bool required { get; set; } = false;

        public ValidationAttribute(int minLength = 0, int maxLength = int.MaxValue, bool required = false)
        {
            this.minLength = minLength;
            this.maxLength = maxLength;
            this.required = required;
        }



    }
}
