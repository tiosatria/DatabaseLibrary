using Dapper;

namespace DatabaseLibrary
{
    public static class Utilities
    {
        /*public static DynamicParameters GetDynamicParams<T>(object obj)
        {
            var parameters = new DynamicParameters();
            foreach (var property in typeof(T).GetProperties())
            {
                if (property.PropertyType == typeof(string) && property?.GetValue(obj)?.ToString()?.Length <= 0) parameters.Add(property.Name, null);
            }
            return parameters;
        }*/

        public static DynamicParameters GetDynamicParams<T>(object obj, params Type[] types)
        {
            var parameters = new DynamicParameters();
            foreach (var property in typeof(T).GetProperties())
            {
                if (property.PropertyType == typeof(string) && property?.GetValue(obj)?.ToString()?.Length <= 0) parameters.Add(property.Name, null);
                if (!types.Contains(property!.PropertyType)) parameters.Add(property.Name, null);
            }
            return parameters;
        }

    }
}
