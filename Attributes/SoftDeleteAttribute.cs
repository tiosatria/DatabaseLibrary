using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLibrary.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SoftDeleteAttribute : Attribute
    {
        public SoftDeleteAttribute(string col)
        {
            this.column=col;

        }
        public string column { get; set; }

        public static bool WhetherUseSoftDelete(object obj)
        {
            var type = obj.GetType();
            return Attribute.IsDefined(type, typeof(SoftDeleteAttribute));
        }

        public static string GetColumnName(object obj)
        {
            var type = obj.GetType();
            return ((SoftDeleteAttribute)Attribute.GetCustomAttribute(type, typeof(SoftDeleteAttribute))!).column;
        }

    }
}
