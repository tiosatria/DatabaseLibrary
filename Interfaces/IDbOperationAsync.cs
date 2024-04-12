
namespace DatabaseLibrary.Interfaces
{
    public interface IDbOperationAsync
    {
        Task<bool> CreateAsync(IDbInsert insert);
        Task<bool> DeleteAsync(IDbDelete delete);
        Task<bool> UpdateAsync(IDbUpdate update);
    }
}
