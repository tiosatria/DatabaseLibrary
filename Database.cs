using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace DatabaseLibrary
{
    public static class Database
    {
        #region Const
        //34.142.246.158
        private const string connString = "Server=localhost;Database=bokemium_db;Uid=superadmin;Pwd=eYLmqpFhuS*1iWlm;Pooling=true;MinPoolSize=5;MaxPoolSize=100;ConnectionLifetime=60;ConnectionReset=true;";
        #endregion

        private static IEnumerable<string> GetParams(string input)
        {
            // Use regular expression to match parameter names inside parentheses
            var regex = new Regex(@"@[\w\d_]+");
            var matches = regex.Matches(input);

            // Convert matches to string array
            var output = matches.Cast<Match>().Select(match => match.Value).ToArray();

            return output;
        }

        public static async Task<MySqlConnection> InitConnectionAsync()
        {
            var conn = new MySqlConnection(connString);
            await conn.OpenAsync();
            return conn;
        }

        public static MySqlConnection InitConnection()
        {
            var conn = new MySqlConnection(connString);
            conn.Open();
            return conn;
        }

        public static (MySqlConnection, MySqlTransaction) UseTransaction()
        {
            var conn = InitConnection();
            var trans = conn.BeginTransaction();
            return (conn, trans);
        }

        public static async Task<(MySqlConnection, MySqlTransaction)> UseTransactionAsync()
        {
            var conn = await Database.InitConnectionAsync();
            var trans = await conn.BeginTransactionAsync();
            return (conn, trans);
        }

    }
}
 