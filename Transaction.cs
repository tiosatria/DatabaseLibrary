using MySql.Data.MySqlClient;
using  static DatabaseLibrary.Helper.Helper;

namespace DatabaseLibrary
{
    public sealed class Transaction : IDisposable, IAsyncDisposable
    {
        private MySqlTransaction? _transaction;
        private MySqlConnection? _connection;

        public bool IsSuccess { get; set; } = false;

        public delegate bool DelDoAct(MySqlTransaction t);
        public delegate void DelDoActVoid(MySqlTransaction t);
        public delegate T DelGetVal<out T>(MySqlTransaction t);

        public delegate Task<bool> DelDoActAsync(MySqlTransaction t);
        public delegate Task DelDoActVoidAsync(MySqlTransaction t);
        public delegate Task<T> DelGetValAsync<T>(MySqlTransaction t);
        
        public static async Task<bool> StartStaticTransactionAsync(DelDoActAsync del)
        {
            await using var t = await new Transaction().InitAsync();
            try
            {
                var success = await del.Invoke(t.TransCTX);
                return await t.FinishTransactionAsync(success);
            }
            catch (Exception e)
            {
                HandleError(e);
                return await t.FinishTransactionAsync();
            }
        }

        public static bool StartTransaction(DelDoAct del)
        {
            using var t = new Transaction().Init();
            try
            {
                var success = del.Invoke(t.TransCTX);
                return t.FinishTransaction(success);
            }
            catch (Exception e)
            {
                HandleError(e);
                return t.FinishTransaction();
            }
        }

        public static async Task<T> StartStaticTransactionAsync<T>(DelGetValAsync<T> del, bool shouldCommit = false)
        {
            var t = new Transaction();
            try
            {
                await t.BeginTransactionAsync();
                var x = await del.Invoke(t.TransCTX);
                await t.FinishTransactionAsync(shouldCommit);
                return x;
            }
            catch (Exception e)
            {
                HandleError(e);
                await t.FinishTransactionAsync();
                throw;
            }
        }

        public static T? StartTransaction<T>(DelGetVal<T> del, bool shouldCommit = false)
        {
            try
            {
                using var t = new Transaction().Init();
                var x = del.Invoke(t.TransCTX);
                t.FinishTransaction(shouldCommit);
                return x;
            }
            catch (Exception e)
            {
                HandleError(e);
                return default;
            }
        }

        public Transaction(MySqlConnection conn, MySqlTransaction trans)
        {
            _transaction=trans;
            _connection=conn;
        }

        public Transaction()
        {

        }

        private async Task InitConnectionAsync()
        {
            if (_transaction is null || _connection is null)
            {
                var (conn, trans) = await Database.UseTransactionAsync();
                _connection = conn;
                _transaction = trans;
            }
        }

        private void InitConnection()
        {
            if (_transaction is null || _connection is null)
            {
                var (conn, trans) = Database.UseTransaction();
                _connection = conn;
                _transaction = trans;
            }
        }

        public async Task<Transaction> InitAsync()
        {
            await this.BeginTransactionAsync();
            return this;
        }

        public Transaction Init()
        {
            InitConnection();
            return this;
        }

        public MySqlTransaction TransCTX => _transaction ?? throw new InvalidOperationException("Please initialise the connection. Call begin transaction first.");

        // manual
        public async Task BeginTransactionAsync()
        {
            await InitConnectionAsync();
        }

        public void BeginTransaction()
        {
            InitConnection();
        }

        // manual
        public async Task<bool> FinishTransactionAsync(bool shouldCommit = false)
        {
            if (shouldCommit)
            {
                await _transaction!.CommitAsync();
            }
            else
            {
                await _transaction!.RollbackAsync();
            }
            await _connection!.DisposeAsync();
            await _transaction.DisposeAsync();
            return shouldCommit;
        }

        public bool FinishTransaction(bool shouldCommit = false)
        {
            if (shouldCommit)
            {
                _transaction!.Commit();
            }
            else
            {
                _transaction!.Rollback();
            }
            _connection!.Dispose();
            _transaction!.Dispose();
            return shouldCommit;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _connection?.Dispose();
        }

        public async ValueTask DisposeAsync()
        {
            await _transaction!.DisposeAsync();
            await _connection!.DisposeAsync();
        }
    }
}
