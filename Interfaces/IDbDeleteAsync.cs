
namespace DatabaseLibrary.Interfaces
{
    public enum DeleteType
    {
        HardDelete,
        SoftDelete
    }

    public enum SoftDeleteType
    {
        Boolean,
        DateTime
    }

    public record struct DeleteConfig(DeleteType type, SoftDeleteType? softDeleteType, string? softDeleteCols);

    public record struct DeleteCondition(string deleteCondition, object? deleteParam);

    public interface IDbDeleteAsync : IDbEntity
    {
        DeleteConfig Config { get; }
        DeleteCondition Condition { get; } 
    }
}
