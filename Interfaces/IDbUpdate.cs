
namespace DatabaseLibrary.Interfaces
{
    public interface IDbUpdate : IDbEntity
    {
        Dictionary<string, object?> UpdateData { get; }
        string updateCondition { get;  }
    }
}
