
namespace DatabaseLibrary.Interfaces
{
    public interface IDbDeleteAsync : IDbEntity
    {
        (string DeleteCondition, object? DeleteParam) DeleteTuple { get; } 
    }
}
