
using System.Reflection;

namespace DatabaseLibrary.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class TableNameAttribute : Attribute
    {
        public static string GetTableName(object obj)
        {
            return Util.GetCustomPropValue<string, TableNameAttribute>(obj);
        }
    }



}
