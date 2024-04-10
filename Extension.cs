using System.Dynamic;
using Dapper.Transaction;
using DatabaseLibrary.Helper;
using DatabaseLibrary.Interfaces;
using StringUtilLib;

namespace DatabaseLibrary
{
    public static class Extension
    {

        private static object MakeAnonymousObject(Dictionary<string, object?> objects)
        {
            var obj = new ExpandoObject() as IDictionary<string, object?>;
            foreach (var o in objects)
            {
                obj.Add(o.Key, o.Value);
            }
            return obj;
        }

        public static async Task<bool> InsertAsync(this IDbInsertAsync obj)
        {
            var keys = obj.InsertData.Select(e => e.Key).ToString();
            var parameters = obj.InsertData.Select(e => e.Key).ToString(prefixer: "@");
            var sql = $"INSERT INTO {obj.TableName} ({keys}) VALUES ({parameters});";
            await using var t = await new Transaction().InitAsync();
            var inserted = await t.TransCTX.ExecuteAsync(sql, MakeAnonymousObject(obj.InsertData)) > 0;
            return await t.FinishTransactionAsync(inserted);
        }

        public static async Task<bool> UpdateAsync(this IDbUpdateAsync obj)
        {
            var updates = obj.UpdateData.Select(e => $"{e.Key} = @{e.Key}").ToString();
            var sql = $"UPDATE {obj.TableName} SET {updates} WHERE {obj.updateCondition};";
            await using var t = await new Transaction().InitAsync();
            var updated = await t.TransCTX.ExecuteAsync(sql, MakeAnonymousObject(obj.UpdateData)) > 0;
            return await t.FinishTransactionAsync(updated);
        }

        public static async Task<bool> DeleteAsync(this IDbDeleteAsync obj)
        {
            var sql = $"DELETE FROM {obj.TableName} WHERE {obj.Condition.deleteCondition}";
            await using var t = await new Transaction().InitAsync();
            var deleted = await t.TransCTX.ExecuteAsync(sql, obj.Condition.deleteParam) > 0;
            return await t.FinishTransactionAsync(deleted);
        }

        public static async Task<dynamic?> FetchSingleAsync(this IDbFetchSingleAsync obj) => await CommonDbHelper.GetObjectAsync<object>(obj.TableName, obj.FetchQuery.cols,
            obj.FetchQuery.condition + " LIMIT 1", obj.FetchQuery.param);

        public static async Task<IEnumerable<dynamic>?> FetchListAsync(this IDbFetchListAsync obj) => await CommonDbHelper.GetMultipleObjectAsync<object>(obj.TableName, obj.FetchQuery.cols,
            obj.FetchQuery.condition != null && obj.Limit > 0 ? $"{obj.FetchQuery.condition} LIMIT {obj.Limit}" : obj.FetchQuery.condition, obj.FetchQuery.param);

    }
}
