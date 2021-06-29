using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIHealthCheck.Model
{
    public class Book
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public int PageCount { get; set; }
        public bool IsActive { get; set; }
        public byte[] RowId { get; set; }
    }
}
