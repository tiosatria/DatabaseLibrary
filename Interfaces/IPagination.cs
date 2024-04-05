using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseLibrary.Interfaces
{
    public interface IPagination
    {
        int totalData { get; set; }
        int pageSize { get; set; }
        int totalPage { get; set; }
        int currentPage { get; set; }
        string cacheKey { get; init; }
    }
}
