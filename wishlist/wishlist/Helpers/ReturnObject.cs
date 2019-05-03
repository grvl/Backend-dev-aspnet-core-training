using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace wishlist.Helpers
{
    public class ReturnObject<T>
    {
        public string Message { get; set; }
        public T Value { get; set; }
        public IQueryable<T> Values { get; set; }

        public bool HasMessage()
        {
            return (Message != null && Message != "");
        }
    }
}
