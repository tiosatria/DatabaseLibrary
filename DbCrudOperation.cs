
using DatabaseLibrary.Interfaces;
using MySql.Data.MySqlClient;

namespace DatabaseLibrary
{
    public sealed class DbCrudOperation : IDbOperationAsync
    {
        public DbCrudOperation()
        {

        }

        public DbCrudOperation(MySqlTransaction trans)
        {
            Transaction = trans;
            IsTransactionOpened=true;
        }

        private MySqlTransaction? Transaction;

        public bool IsOperationStarted { get; private set; } = false;
        public bool IsOperationFinished { get; private set; } = false;
        public bool IsOperationSuccesfull { get; private set; } = false;
        public bool IsTransactionOpened { get; private set; } = false;
        public bool IsTransactionDisposed { get; private set; } = false;

        public MySqlTransaction GetTransactionInstance() => Transaction??throw new InvalidOperationException("transaction_is_not_initialized");

        public MySqlConnection GetConnectionInstance() =>
            Transaction?.Connection ?? throw new InvalidOperationException("connection_is_not_initialized");

        private async Task InitAsync()
        {
            if (Transaction != null) return;
            var conn = await Database.InitConnectionAsync();
            Transaction = await conn.BeginTransactionAsync();
            IsTransactionOpened=true;
        }

        public async Task<bool> CreateAsync(IDbInsert insert)
        {
            try
            {
                await InitAsync();
                IsOperationStarted = true;
            }
            catch (Exception e)
            {
                Helper.Helper.HandleError(e);
                return false;
            }
        }

        public Task<bool> CreateBulkAsync(params IDbInsert[] inserts)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(IDbUpdate update)
        {
            await InitAsync();
            IsOperationStarted = true;

        }

        public async Task<bool> DeleteAsync(IDbDelete delete)
        {
            await InitAsync();
            IsOperationStarted = true;

        }

        public async Task FinishOperationAsync(bool shouldCommit)
        {
            if (Transaction?.Connection is null)
            {
                IsOperationFinished = true;
                return;
            }
            if (shouldCommit) await Transaction.CommitAsync();
            else await Transaction.RollbackAsync();
            await Transaction.DisposeAsync();
            await Transaction.Connection.DisposeAsync();
            IsOperationFinished = true;
            IsTransactionDisposed = true;
        }

    }
}
