using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseLibrary.Interfaces;

namespace DatabaseLibrary
{
    public class PaginationResult<T>
    {
        
        public IEnumerable<T> Data { get; set; }
        public PaginationData? PaginationData { get; set; }
    }

    public record PaginationData : IPagination
    {

        public delegate Task<int> GetCountTaskDelegate();
        public delegate int GetCountDelegate();

        public async Task<PaginationData> InitAsync()
        {
            
            return this;
        }

        public int totalData { get; set; }
        public int pageSize { get; set; }
        public int totalPage { get; set; }
        public int currentPage { get; set; }
        public string cacheKey { get; init; }



    }

}
