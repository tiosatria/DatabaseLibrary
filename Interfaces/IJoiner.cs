using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLibrary.Interfaces
{
    public interface IJoinProvider
    {

        static abstract JoinBuilder UseJoin(JoinBuilder.JoinType typeJoin, string similarities, string? alias = null);

    }
}
