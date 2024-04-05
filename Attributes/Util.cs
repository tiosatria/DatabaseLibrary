using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLibrary.Attributes
{
    public static class Util
    {
        public static T GetCustomPropValue<T, CustomAttr>(object obj) 
        {
            var type = obj.GetType();

            var member = type.GetMembers(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
                .FirstOrDefault(m => m.GetCustomAttributes(typeof(CustomAttr), true).Any());
            /*var member = type.GetMembers(BindingFlags.NonPublic | BindingFlags.Instance)
                .FirstOrDefault(m => m.GetCustomAttribute(typeof(CustomAttr), true) != null);*/
            return member == null
                ? throw new InvalidOperationException("No field or property with specified custom attribute found.")
                : member switch
                {
                    FieldInfo field => (T)field.GetValue(obj)!,
                    PropertyInfo property => (T)property.GetValue(obj)!,
                    _ => throw new InvalidOperationException("No field or property with specified custom attribute found.")
                };
        }

        public static (string, object)[] GetCustomPropValueTuples<CustomAttr>(object obj)
        {
            var type = obj.GetType();

            var members = type.GetMembers(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
                .Where(m => m.GetCustomAttributes(typeof(CustomAttr), true).Any())
                .ToArray();

            if (members.Length == 0)
            {
                throw new InvalidOperationException("No field or property with the specified custom attribute found.");
            }

            var values = members.Select(member =>
            {
                return member switch
                {
                    FieldInfo field => (field.Name, field.GetValue(obj)),
                    PropertyInfo property => (property.Name, property.GetValue(obj)),
                    _ => throw new InvalidOperationException("Unexpected member type.")
                };
            }).ToArray();

            return values;
        }

        public static (string, object)[] GetCustomPropParamsTuples<CustomAttr>(object obj)
        {
            var type = obj.GetType();

            var members = type.GetMembers(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.GetField | BindingFlags.SetField )
                .Where(m => m.GetCustomAttributes(typeof(CustomAttr), true).Any())
                .ToArray();

            if (members.Length == 0)
            {
                throw new InvalidOperationException("No field or property with the specified custom attribute found.");
            }

            var values = members.Select(member =>
            {
                return member switch
                {
                    FieldInfo field => (field.Name, $"@{field.Name}"),
                    PropertyInfo property => (property.Name, (object) $"@{property.Name}"),
                    _ => throw new InvalidOperationException("Unexpected member type.")
                };
            }).ToArray();

            return values;
        }

        public static T[] GetCustomPropValueArray<T, CustomAttr>(object obj)
        {
            var type = obj.GetType();

            var members = type.GetMembers(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public)
                .Where(m => m.GetCustomAttributes(typeof(CustomAttr), true).Any())
                .ToArray();

            if (members.Length == 0)
            {
                throw new InvalidOperationException("No field or property with the specified custom attribute found.");
            }

            var values = members.Select(member =>
            {
                return member switch
                {
                    FieldInfo field => (T)field.GetValue(obj)!,
                    PropertyInfo property => (T)property.GetValue(obj)!,
                    _ => throw new InvalidOperationException("Unexpected member type.")
                };
            }).ToArray();

            return values;
        }

    }
}
