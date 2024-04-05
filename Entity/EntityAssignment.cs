using Dapper.Transaction;
using MySql.Data.MySqlClient;

namespace DatabaseLibrary.Entity
{
    public class EntityAssignment
    {
        private (string col, object? val) _sourceEntity;
        private (string col, object? val) _targetEntity;
        private (string col, object? val)? _assignee;
        private readonly string _table;
        private MySqlTransaction? _transaction;

        public EntityAssignment(string table)
        {
            _table = table;
        }

        public EntityAssignment(string table, (string col, object? val) sourceEntity, (string col, object? val) targetEntity, (string col, object? val)? assignee)
        {
            _table = table;
            _sourceEntity = sourceEntity;
            _targetEntity = targetEntity;
            _assignee = assignee;
        }

        public EntityAssignment SetSource((string col, object? val) sourceEntity)
        {
            _sourceEntity = sourceEntity;
            return this;
        }

        public EntityAssignment SetTarget((string col, object? val) targetEntity)
        {
            _targetEntity = targetEntity;
            return this;
        }

        public EntityAssignment SetOperator((string col, object? val)? assignee)
        {
            _assignee = assignee;
            return this;
        }

        public EntityAssignment SetTransaction(MySqlTransaction transaction)
        {
            _transaction = transaction;
            return this;
        }

        public async Task<bool> AssignAsync(object? param = null)
        {
            var q = new QueryBuilder(_transaction,QueryBuilder.QueryTypeEnums.INSERT, _table)
                .AddInsertValue
                (
                    (_sourceEntity.col, _sourceEntity.val),
                    (_targetEntity.col, _targetEntity.val)
                ).AddParams(param);
            if (_assignee != null) q.AddInsertValue((_assignee.Value.col, _assignee.Value.val));
            return await q.ExecuteDMLAsync() > 0;
        }

        public async Task<bool> DeassignAsync(object? param = null)
        {
            return await new QueryBuilder(QueryBuilder.QueryTypeEnums.DELETE, _table)
                .AddWhere((_targetEntity.col, _targetEntity.val))
                .SetObjectParams(param)
                .ExecuteDMLAsync() > 0;
        }

        public async Task<bool> ClearAssignmentAsync(object? param = null)
        {
            return await new QueryBuilder(_transaction, QueryBuilder.QueryTypeEnums.DELETE, _table)
                .AddWhere((_sourceEntity.col, _sourceEntity.val))
                .AddParams(param)
                .ExecuteDMLAsync() > 0;
        }

    }
}
