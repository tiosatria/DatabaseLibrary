﻿
namespace DatabaseLibrary.Interfaces
{
    public interface IDbUpdateAsync : IDbEntity
    {
        Dictionary<string, object?> UpdateData { get; }
        string updateCondition { get; }
    }
}
