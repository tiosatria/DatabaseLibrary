using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLibrary.Interfaces
{
    public interface IDbInsert : IDbEntity
    {
        Dictionary<string, object?> InsertData { get; }
    }
}
