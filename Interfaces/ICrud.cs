using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLibrary.Interfaces
{

    public interface ICrud
    {
        public string TableName { get; }
        public Dictionary<string, object> Columns { get; }

        public static Task<bool> InsertAsync()
        {
            throw new NotImplementedException();
        }

        public static Task<int> InsertReturnIdAsync()
        {
            throw new NotImplementedException();
        }

        public static bool Insert()
        {
            throw new NotImplementedException();
        }

        public static Task<bool> UpdateAsync()
        {
            throw new NotImplementedException();
        }

        public static bool Update()
        {
            throw new NotImplementedException();
        }

        public static Task<bool> DeleteAsync()
        {
            throw new NotImplementedException();
        }

        public static bool Delete()
        {
            throw new NotImplementedException();
        }

    }
}
