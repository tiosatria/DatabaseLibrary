using System.Dynamic;
using Dapper.Transaction;
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
            var sql = $"INSERT INTO {obj.Table} ({keys}) VALUES ({parameters});";
            await using var t = await new Transaction().InitAsync();
            var inserted = await t.TransCTX.ExecuteAsync(sql, MakeAnonymousObject(obj.InsertData)) > 0;
            return await t.FinishTransactionAsync(inserted);
        }

        public static async Task<bool> UpdateAsync(this IDbUpdateAsync obj)
        {
            var updates = obj.UpdateData.Select(e => $"{e.Key} = @{e.Key}").ToString();
            var sql = $"UPDATE {obj.Table} SET {updates} WHERE {obj.WhereCondition};";
            await using var t = await new Transaction().InitAsync();
            var updated = await t.TransCTX.ExecuteAsync(sql, MakeAnonymousObject(obj.UpdateData)) > 0;
            return await t.FinishTransactionAsync(updated);
        }

        public static async Task<bool> DeleteAsync(this IDbDeleteAsync obj)
        {
            var sql = $"DELETE FROM {obj.Table} WHERE {obj.DeleteTuple.DeleteCondition}";
            await using var t = await new Transaction().InitAsync();
            var deleted = await t.TransCTX.ExecuteAsync(sql, obj.DeleteTuple.DeleteParam) > 0;
            return await t.FinishTransactionAsync(deleted);
        }

        public static async Task<IDbFetchSingleAsync> FetchSingleAsync(this IDbFetchSingleAsync obj)
        {
            throw new NotImplementedException();
            
        } 


    }
}
