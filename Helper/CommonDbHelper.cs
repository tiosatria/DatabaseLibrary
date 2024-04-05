using Dapper.Transaction;
using MySql.Data.MySqlClient;

namespace DatabaseLibrary.Helper
{
    public static class CommonDbHelper
    {
        /// <summary>
        /// Return true when record exist. Else false;
        /// </summary>
        /// <param name="whereToSearch"></param>
        /// <param name="condition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static async Task<bool> IsExistAsync(string whereToSearch, string condition, object? param = null) =>
            await Transaction.StartStaticTransactionAsync<bool>(async e =>
                await e.ExecuteScalarAsync<bool>(
                    $"SELECT EXISTS(SELECT 1 FROM {whereToSearch} WHERE {condition}) scalar", param));

        /// <summary>
        /// Count * as 
        /// </summary>
        /// <param name="table">Table in where the count will take place</param>
        /// <param name="condition">Condition Statement</param>
        /// <param name="param">Parameters as new object</param>
        /// <returns></returns>
        public static async Task<int> CountAsync(string table, string condition, object? param = null) =>
            await Transaction.StartStaticTransactionAsync<int>(async e => await e.ExecuteScalarAsync<int>(
                $"SELECT COUNT(*) FROM {table} {condition}", param));

        public static async Task<decimal> SumConvertAmountAsync(string table, string colToSum, string colWithCurrency,
            string currency, string condition, object? param = null, int roundTo = 2) =>
            await Transaction.StartStaticTransactionAsync<decimal>(async t =>
                await t.ExecuteScalarAsync<decimal>(
                    $"SELECT COALESCE(ROUND(SUM({colToSum} * (crate.currency_multiplier / curr.currency_multiplier)) ,{roundTo})) result FROM {table} LEFT JOIN com_currency curr ON curr.code_currency = {colWithCurrency} LEFT JOIN com_currency crate ON crate.code_currency = '{currency}' WHERE {condition}", param));

        /// <summary>
        /// Delete record from the database using specified condition
        /// </summary>
        /// <param name="t">SQL Transaction</param>
        /// <param name="table">Table in which the record will be deleted</param>
        /// <param name="condition">Condition statement</param>
        /// <param name="param">Parameters as new object specified in condition statement</param>
        /// <returns></returns>
        public static async Task<bool> DeleteRecordAsync(string table, string condition, object? param = null) =>
            await Transaction.StartStaticTransactionAsync(async (t) =>
                await t.ExecuteAsync($"DELETE FROM {table} WHERE {condition}", param) > 0);

        /// <summary>
        /// Async Delete record from the database using specified condition
        /// </summary>
        /// <param name="t">SQL Transaction</param>
        /// <param name="table">Table in which the record will be deleted</param>
        /// <param name="condition">Condition statement</param>
        /// <param name="param">Parameters as new object specified in condition statement</param>
        /// <returns></returns>
        public static async Task<bool> DeleteRecordAsync(MySqlTransaction t, string table, string condition,
            object? param = null) =>
            await t.ExecuteAsync($"DELETE FROM {table} WHERE {condition}", param) > 0;

        /// <summary>
        /// Select Something from table, output is object, if none found return default/null
        /// This method will fire up new static transaction. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table">Specify table</param>
        /// <param name="cols">Specify columns, if not specified will select * (all) column in table</param>
        /// <param name="condition">You need to specify the reserved keyword like where or join. at the end of it will be appended LIMIT 1 to force it to object.</param>
        /// <returns>output is object specified, if none found return default/null</returns>
        public static async Task<T?> GetObjectAsync<T>(string table, string? cols = null, string ? condition = null, object? param = null) => 
            await Transaction.StartStaticTransactionAsync(async (t) => 
                await t.QuerySingleAsync<T>($"SELECT {cols ?? "*"} FROM {table} {condition ?? string.Empty} LIMIT 1;", param) ?? default);


        public static async Task<IEnumerable<T>?> GetMultipleObjectAsync<T>(string table, string? cols = null,
            string? condition = null, object? param = null) =>
            await Transaction.StartStaticTransactionAsync(async (t) =>
            {
                var sql = $"SELECT {cols ?? "*"} FROM {table} {condition ?? string.Empty};";
                return await t.QueryAsync<T>(sql, param) ?? default;
            });


        /// <summary>
        /// Select Something from table, output is object, if none found return default/null
        /// This method will fire up new static transaction. 
        /// </summary>
        /// <typeparam name="T">Result will be cast to this type</typeparam>
        /// <param name="table">Specify table</param>
        /// <param name="cols">Specify columns, if not specified will select * (all) column in table</param>
        /// <param name="condition">You need to specify the reserved keyword like where or join. at the end of it will be appended LIMIT 1 to force it to object.</param>
        /// <returns>output is object specified, if none found return default/null</returns>
        public static T? GetObject<T>(string table, string? cols = null, string? condition = null, object? param = null) =>
            Transaction.StartTransaction( (t) =>
                t.QuerySingle<T>($"SELECT {cols ?? "*"} FROM {table} {condition ?? string.Empty} LIMIT 1;", param) ?? default);

        public static IEnumerable<T>? GetMultipleObject<T>(string table, string? cols = null, string? condition = null, object? param = null) =>
            Transaction.StartTransaction((t) =>
                t.Query<T>($"SELECT {cols ?? "*"} FROM {table} {condition ?? string.Empty};", param) ?? default);


    }
}
