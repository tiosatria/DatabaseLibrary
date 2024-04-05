
namespace DatabaseLibrary;

public class JoinBuilder
{
    public enum JoinType
    {
        JOIN,
        LEFT_JOIN,
        RIGHT_JOIN,
    }

    public string _tableName;
    private string _columnJoiner;
    private JoinType _joinType = JoinType.JOIN;
    private string? _manualJoin;
    private string? _tableAlias;

    public JoinBuilder(string tableName, string columnJoiner, string? tableAlias = null)
    {
        this._tableName = tableName;
        this._columnJoiner = columnJoiner;
        _tableAlias = tableAlias;
    }

    public JoinBuilder(string tableName, string columnJoiner, JoinType type,string? tableAlias = null)
    {
        this._tableName = tableName;
        this._columnJoiner = columnJoiner;
        _tableAlias = tableAlias;
        _joinType=type;
    }

    public JoinBuilder()
    {

    }

    public JoinBuilder Join(JoinType type, string table)
    {
        _tableName = table;
        _joinType = type;
        return this;
    }

    public JoinBuilder On(string columnJoiner)
    {
        this._columnJoiner = columnJoiner;
        return this;
    }

    public JoinBuilder Alias(string alias)
    {
        _tableAlias = alias;
        return this;
    }

    public JoinBuilder UseManualJoin(string manual)
    {
        this._manualJoin = manual;
        return this;
    }

    public string GetJoinQuery => string.IsNullOrEmpty(_manualJoin)
        ? @$"{_joinType.ToString().Replace("_", " ")} {_tableName + " " + _tableAlias} ON {_columnJoiner}"
        : _manualJoin;

}