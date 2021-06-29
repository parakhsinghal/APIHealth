using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIHealthCheck.Model
{
    public class Library
    {
        public int LibraryId { get; set; }
        public IEnumerable<Book> Books { get; set; }
    }
}
