using Dapper.Transaction;
using DatabaseLibrary.Attributes;
using DatabaseLibrary.Helper;
using static DatabaseLibrary.Helper.Helper;
using MySql.Data.MySqlClient;

namespace DatabaseLibrary.Entity
{
    public abstract class EntityBase 
    {

        public event EventHandler? OnInserted;
        public event EventHandler? OnUpdated;
        public event EventHandler? OnDeleted;
        public event EventHandler? OnChanged;

        protected virtual void OnInsertCompleted()
        {
            OnInserted?.Invoke(this, EventArgs.Empty);
            OnChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnUpdateCompleted()
        {
            OnUpdated?.Invoke(this, EventArgs.Empty);
            OnChanged?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnDeleteCompleted()
        {
            OnDeleted?.Invoke(this, EventArgs.Empty);
            OnChanged?.Invoke(this, EventArgs.Empty);
        }

        private MySqlTransaction? _transaction;
        protected bool disableInsert { get; init; } = false;
        protected bool disableUpdate { get; init; } = false;
        protected bool disableDelete { get; init; } = false;

        private string tableName => TableNameAttribute.GetTableName(this);
        private (string,object)[] paramInsert => Util.GetCustomPropParamsTuples<InsertableAttribute>(this);
        private (string, object)[] paramUpdate => Util.GetCustomPropParamsTuples<UpdateableAttribute>(this);
        private (string, object)[] paramKeys => Util.GetCustomPropParamsTuples<KeyIdentifierAttribute>(this);
        private bool useSoftDelete => SoftDeleteAttribute.WhetherUseSoftDelete(this);
        private string softDeleteCol => SoftDeleteAttribute.GetColumnName(this);

        private QueryBuilder QInsert => new QueryBuilder(QueryBuilder.QueryTypeEnums.INSERT, tableName)
            .AddInsertValue(paramInsert!)
            .AddParams(this);

        private QueryBuilder QUpdate => new QueryBuilder(QueryBuilder.QueryTypeEnums.UPDATE, tableName)
        .AddUpdateValue(paramUpdate!)
        .AddParams(this)
        .AddWheres(paramKeys!);

        private QueryBuilder QSoftDelete => new QueryBuilder(QueryBuilder.QueryTypeEnums.UPDATE, tableName)
            .AddUpdateValue((softDeleteCol, 1))
            .AddParams(this)
        .AddWheres(paramKeys!);

        private QueryBuilder QDelete => new QueryBuilder(QueryBuilder.QueryTypeEnums.DELETE, tableName)
            .AddWheres(paramKeys!)
            .AddParams(this);

        public void SetTransaction(MySqlTransaction trans)
        {
            this._transaction = trans;
        }

        public virtual async Task<bool> InsertAsync()
        {
            if (disableInsert) throw new InvalidOperationException("insert operation is not permitted");
            if (_transaction != null) return await QInsert.SetTransaction(_transaction).ExecuteDMLAsync() > 0;
            var (conn, trans) = await Database.UseTransactionAsync();
            await using (conn)
            await using (trans)
                try
                {
                    var success = await QInsert.SetTransaction(trans).ExecuteDMLAsync() > 0;
                    if(success) await trans.CommitAsync();
                    return success;
                }
                catch (Exception e)
                {
                    HandleError(e);
                    await trans.RollbackAsync();
                    return false;
                }
        }

        public virtual async Task<int> InsertWithReturnIDAsync()
        {
            if (disableInsert) throw new InvalidOperationException("insert operation is not permitted");
            if (_transaction != null) return await QInsert
                .SetTransaction(_transaction)
                .ReturnLastInsertID()
                .ExecuteScalarAsync<int>();
            var (conn, trans) = await Database.UseTransactionAsync();
            await using (conn)
            await using (trans)
                try
                {
                    var data = await QInsert.SetTransaction(trans).ExecuteScalarAsync<int>();
                    if (data>0) await trans.CommitAsync();
                    return data;
                }
                catch (Exception e)
                {
                    HandleError(e);
                    await trans.RollbackAsync();
                    return -1;
                }
        }

        public virtual async Task<bool> UpdateAsync()
        {
            if (disableUpdate) throw new InvalidOperationException("update operation is not permitted");
            if (_transaction != null) return await QInsert.SetTransaction(_transaction).ExecuteDMLAsync() > 0;
            var (conn, trans) = await Database.UseTransactionAsync();
            await using (conn)
            await using (trans)
                try
                {
                    var success = await QUpdate.SetTransaction(trans).ExecuteDMLAsync() > 0;
                    if(success) await trans.CommitAsync();
                    return success;
                }
                catch (Exception e)
                {
                    HandleError(e);
                    await trans.RollbackAsync();
                    return false;
                }
        }

        public virtual async Task<bool> DeleteAsync()
        {
            if (disableDelete) throw new InvalidOperationException("delete operation is not permitted");
            if (_transaction != null) return useSoftDelete ? await QSoftDelete.SetTransaction(_transaction).ExecuteDMLAsync() > 0 : await QDelete.SetTransaction(_transaction).ExecuteDMLAsync() > 0;
            var (conn, trans) = await Database.UseTransactionAsync();
            await using (conn)
            await using (trans)
                try
                {
                    var success = useSoftDelete 
                        ? await QSoftDelete.SetTransaction(trans).ExecuteDMLAsync() > 0 
                        : await QDelete.SetTransaction(trans).ExecuteDMLAsync() > 0;
                    if(success) await trans.CommitAsync();
                    return success;
                }
                catch (Exception e)
                {
                    HandleError(e);
                    await trans.RollbackAsync();
                    return false;
                }
        }


        public virtual async Task<bool> InsertAsync(MySqlTransaction trans)
        {
            if (disableInsert) throw new InvalidOperationException("insert operation is not permitted");
            _transaction = trans;
            if (_transaction != null) return await QInsert.SetTransaction(_transaction).ExecuteDMLAsync() > 0;
            return false;
        }

        public virtual async Task<int> InsertWithReturnIDAsync(MySqlTransaction trans)
        {
            if (disableInsert) throw new InvalidOperationException("insert operation is not permitted");
            _transaction = trans;
            if (_transaction != null) return await QInsert.ReturnLastInsertID().SetTransaction(_transaction).ExecuteScalarAsync<int>();
            return -1;
        }

        public virtual async Task<uint> InsertWithReturnID_UINT_Async(MySqlTransaction trans)
        {
            if (disableInsert) throw new InvalidOperationException("insert operation is not permitted");
            _transaction = trans;
            if (_transaction != null) return await QInsert.ReturnLastInsertID().SetTransaction(_transaction).ExecuteScalarAsync<uint>();
            return 0;
        }

        public virtual async Task<bool> UpdateAsync(MySqlTransaction trans)
        {
            if (disableUpdate) throw new InvalidOperationException("update operation is not permitted");
            _transaction = trans;
            if (_transaction != null) return await QUpdate.SetTransaction(_transaction).ExecuteDMLAsync() > 0;
            return false;
        }

        public virtual async Task<bool> DeleteAsync(MySqlTransaction trans)
        {
            if (disableDelete) throw new InvalidOperationException("delete operation is not permitted");
            _transaction = trans;
            if (_transaction != null) return useSoftDelete ? await QSoftDelete.SetTransaction(_transaction).ExecuteDMLAsync() > 0 : await QDelete.SetTransaction(_transaction).ExecuteDMLAsync() > 0;
            return false;
        }

        public virtual async Task<bool> InsertOnDuplicateKeyUpdateAsync(MySqlTransaction trans)
        {
            throw new NotImplementedException();
            if(disableInsert) throw new InvalidOperationException("insert operation is not permitted");
            return await trans.ExecuteAsync($"INSERT INTO {tableName} VALUES ()")
                   > 0;
        }



    }
}
