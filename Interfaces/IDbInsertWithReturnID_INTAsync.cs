using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLibrary.Interfaces
{
    public interface IDbInsertWithReturnID_INTAsync : IDbEntity
    {
        Dictionary<string,object> InsertData { get; }

    }
}
