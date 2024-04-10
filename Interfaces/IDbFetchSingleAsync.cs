
namespace DatabaseLibrary.Interfaces
{
    public record struct FetchQuery(string? cols, string? condition, object? param);
    public interface IDbFetchSingleAsync : IDbEntity
    {
        FetchQuery FetchQuery { get; }
    }
}
