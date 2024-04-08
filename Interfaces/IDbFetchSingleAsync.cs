
namespace DatabaseLibrary.Interfaces
{
    public interface IDbFetchSingleAsync : IDbEntity
    {
        Type objType { get; }
    }
}
