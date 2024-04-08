using Dapper;
using Dapper.Transaction;
using MySql.Data.MySqlClient;

namespace DatabaseLibrary;

public class QueryBuilder
{
    #region Enums

    public enum QueryTypeEnums
    {
        SELECT,
        INSERT,
        UPDATE,
        DELETE,
        MANUAL
    }

    public enum OrderQueryTypeEnum
    {
        TRUE,
        ASC,
        DESC
    }

    public enum ExecuteTypeEnum
    {
        QUERY,
        QUERYSINGLE,
        SCALAR,
        DML,
        EXECUTE
    }

    #endregion

    private (OrderQueryTypeEnum orderBy, string cols)? _orderByTuple = null;
    private readonly List<(string key, object? val)> _whereTuples = new();
    private readonly List<JoinBuilder> _joins = new();
    private int _limit = 0;
    private string? _wheres;
    private readonly List<string> _groubList = new();
    private readonly List<string> _selectCols = new();
    private readonly List<(string col, object? val)> _updateTuples = new();
    private readonly List<(string col, object? val)> _insertTuples = new();
    private readonly List<QueryBuilder> _unionList = new();
    private bool _withLastInsertId = false;
    private string? _additionalCommand;
    private DynamicParameters _parameters = new();
    private MySqlTransaction? _transaction;
    public ExecuteTypeEnum ExecuteType = ExecuteTypeEnum.DML;
    private string? _appendWhere;

    private readonly QueryTypeEnums _queryType;
    private string _table;

    #region Setter


    public QueryBuilder(MySqlTransaction trans)
    {
        _transaction=trans;
    }

    public QueryBuilder(MySqlTransaction trans, QueryTypeEnums queryType, string table)
    {
        _transaction=trans;
        _queryType = queryType;
        _table = table;
    }

    public QueryBuilder(MySqlTransaction t, QueryTypeEnums type)
    {
        _transaction=t;
        _queryType=type;
    }

    public QueryBuilder(QueryTypeEnums queryType, string table)
    {
        _queryType = queryType;
        _table = table;
    }

    public QueryBuilder SetTransaction(MySqlTransaction trans)
    {
        this._transaction=trans;
        return this;
    }

    public QueryBuilder SetExecuteType(ExecuteTypeEnum executeType)
    {
        this.ExecuteType = executeType;
        return this;
    }

    public QueryBuilder ReturnLastInsertID()
    {
        _withLastInsertId = true;
        return this;
    }

    public QueryBuilder ChangeTable(string tableName)
    {
        _table = tableName;
        return this;
    }

    public QueryBuilder SetObjectParams<T>(T param)
    {
        var parameter = Utilities.GetDynamicParams<T>(param);
        _parameters = parameter;
        return this;
    }

    public QueryBuilder AddParamSingle(string name, object? param)
    {
        _parameters.Add(name, param);
        return this;
    }

    public QueryBuilder AddParams(object param)
    {
        _parameters.AddDynamicParams(param);
        return this;
    }

    public QueryBuilder AddUpdateValue(params (string col, object? val)[] updateTuples)
    {
        _updateTuples.AddRange(updateTuples);
        return this;
    }

    public QueryBuilder OrderBy((OrderQueryTypeEnum orderBy, string cols) orderTuple)
    {
        _orderByTuple = orderTuple;
        return this;
    }

    public QueryBuilder AddSelects(params string[] col)
    {
        _selectCols.AddRange(col); 
        return this;
    }

    public QueryBuilder AddInsertValue(params (string col, object? val)[] insertTuples)
    {
        _insertTuples.AddRange(insertTuples); 
        return this;
    }

    public QueryBuilder SetLimit(int limit)
    {
        _limit = limit;
        return this;
    }

    public QueryBuilder Where(string wheres)
    {
        _wheres = wheres;
        return this;
    }

    public QueryBuilder AddWheres(params (string key, object? val)[] whereTuples)
    {
        _whereTuples.AddRange(whereTuples);
        return this;
    }

    public QueryBuilder AddJoins(params JoinBuilder[] joins)
    {
        _joins.AddRange(joins);
        return this;
    }

    public QueryBuilder AddJoin(JoinBuilder join)
    {
        _joins.Add(join);
        return this;
    }

    public QueryBuilder AddWhere((string key, object? val) tuple)
    {
        _whereTuples.Add(tuple);
        return this;
    }

    public QueryBuilder AddGroups(params string[] groups)
    {
        _groubList.AddRange(groups);
        return this;
    }

    public QueryBuilder AddLikes(params (string col, object? val)[] tuple)
    {
        _updateTuples.AddRange(tuple);
        return this;
    }

    public QueryBuilder Union(params QueryBuilder[] builders)
    {
        _unionList.AddRange(builders);
        return this;
    }

    public QueryBuilder OnDuplicateUpdate(params (string col, object? val)[] updateTuples)
    {
        _updateTuples.AddRange(updateTuples);
        return this;
    }

    public QueryBuilder AppendWhere(string str)
    {
        _appendWhere = str;
        return this;
    }

    #region Execute

    public async Task<T> ExecuteQuerySingleAsync<T>()
    {
        return await _transaction.QuerySingleAsync<T>(this.Build(), _parameters);
    }

    public async Task<IEnumerable<T>> ExecuteQueryAsync<T>()
    {
        return await _transaction.QueryAsync<T>(this.Build(), _parameters);
    }

    public async Task<T> ExecuteScalarAsync<T>()
    {
        return await _transaction.ExecuteScalarAsync<T>(this.Build(), _parameters);
    }

    public async Task<int> ExecuteDMLAsync()
    {
        return await _transaction.ExecuteAsync(this.Build(), _parameters);
    }

    public async Task ExecuteAsync()
    {
        await _transaction.ExecuteAsync(this.Build(), _parameters);
    }

    #endregion



    #endregion

    #region Clauses

    private string JoinClause()
    {
        var clauses = (from obj in _joins select obj.GetJoinQuery).ToList();
        return clauses.Any() ? string.Join(" ", clauses) : string.Empty;
    }

    private string WhereClause()
    {
        var clauses = (from obj in _whereTuples where obj.val != null select $"{obj.key} = {ToVal(obj.val)}").ToList();
        var w = clauses.Any() && _wheres is null
            ? " WHERE " + string.Join(" AND ", clauses) 
            : _wheres != null ? $" WHERE {_wheres}"
                : string.Empty;
        return w + " " + _appendWhere;
    }

    private string GroupByClause()
    {
        var clauses = (from obj in _groubList select obj).ToList();
        return clauses.Any() ? " GROUP BY " + string.Join(", ", clauses) : string.Empty;
    }

    private static object? ToVal(object? obj)
    {
        var objStr = obj?.ToString();
        return obj is string
               && !objStr!.Contains('@')
               && (!objStr.Contains("()") 
                   || 
                   (!objStr.Contains('(') && !objStr.Contains(')')))
            ? $"'{obj}'"
            : obj;
    }

    #endregion

    #region Parser

    private string ParseSelectCols()
    {
        if (_selectCols.Count <= 0) return "*";
        return string.Join(", ", _selectCols);
    }

    private string ParseUpdateCols()
    {
        var updates = (from _update in _updateTuples select $"`{_update.col}` = {ToVal(_update.val)}").ToList();
        return updates.Any() ? string.Join(", ", updates) : string.Empty;
    }

    private string ParseInsert()
    {
        if (_insertTuples.Count == 0) throw new InvalidOperationException("No insert values provided.");
        var cols = string.Join(", ", _insertTuples.Select(tuple => $"`{tuple.col}`"));
        var vals = string.Join(", ", _insertTuples.Select(tuple => ToVal(tuple.val)));
        return $"({cols}) VALUES ({ToVal(vals)}) {(_updateTuples.Count > 0 ? $" ON DUPLICATE KEY UPDATE {ParseUpdateCols()}" : "")}";
    }

    private string ParseLimitResult()
    {
        return _limit > 0 ? $"LIMIT {_limit}" : string.Empty;
    }

    private string ParseOrderBy()
    {
        return _orderByTuple is null
            ? string.Empty
            : $"ORDER BY {_orderByTuple!.Value.cols} {_orderByTuple.Value.orderBy}";
    }

    private string ParseLikes()
    {
        var clausesPreJoin = (from obj in _updateTuples where obj.val != null select $"{obj.col} LIKE CONCAT('%', {ToVal(obj.val)}, '%')").ToList();
        var preclause = clausesPreJoin.Any() && _whereTuples.Count(e=>e.val != null) > 0 ? " AND " : clausesPreJoin.Any() ? " WHERE " : string.Empty;
        var clauses = string.Join(" AND ", clausesPreJoin);
        return preclause + clauses;
    }

    #endregion

    #region Constructor

    private string ConstructSelectString()
    {
        return $"SELECT {ParseSelectCols()} FROM {_table} {JoinClause()} {WhereClause()} {ParseLikes()} {GroupByClause()} {ParseOrderBy()} {ParseLimitResult()}";
    }

    private string ConstructSelectUnionString()
    {
        var result = ConstructSelectString();
        return _unionList.Aggregate(result, (current, builder) => current + " UNION " + builder.ConstructSelectString()); ;
    }

    #endregion

    private const string LAST_INSERT_ID = "SELECT LAST_INSERT_ID()";

    public string Build()
    {
        return _queryType switch
        {
            QueryTypeEnums.SELECT => $"{(_unionList.Any() ? ConstructSelectUnionString() : ConstructSelectString())};",
            QueryTypeEnums.UPDATE => $"UPDATE {_table} SET {ParseUpdateCols()} {WhereClause()};",
            QueryTypeEnums.DELETE => $"DELETE FROM {_table} {WhereClause()};",
            QueryTypeEnums.INSERT => $"INSERT INTO {_table} {ParseInsert()}; {(_withLastInsertId ? LAST_INSERT_ID : string.Empty)};",
            QueryTypeEnums.MANUAL => _table,
            _ => throw new NotSupportedException("Please specify query type")
        };
    }

}