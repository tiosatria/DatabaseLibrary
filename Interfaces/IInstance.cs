using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLibrary.Interfaces
{
    public interface IInstance
    {
        T CreateInstance<T>();
    }

    public interface IInstanceAsync
    {

    }

}
