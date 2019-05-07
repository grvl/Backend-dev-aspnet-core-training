using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wishlist.Helpers
{
    public class PaginatedObject<T>
    {
        public int Total { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public List<T> Result { get; set; }
        public string Previous { get; set; }
        public string Next { get; set; }

        public PaginatedObject (String type, ObjectPagination objectPagination, List<T> list, int totalItems) 
        {   
            int totalPages = (int)Math.Ceiling(totalItems / (double)objectPagination.Size);

            Total = totalItems;
            TotalPages = totalPages;
            PageNumber = objectPagination.Page;
            PageSize = objectPagination.Size;
            Result = list;
            Previous = (objectPagination.Page > 1) ? $"{type}?size={objectPagination.Size}&page={objectPagination.Page - 1}" : "";
            Next = (objectPagination.Page<totalPages) ? $"{type}?size={objectPagination.Size}&page={objectPagination.Page + 1}" : "";
        }

    }
}
