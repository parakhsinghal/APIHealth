using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIHealthCheck.Model
{
    public class Author
    {
        public int AuthorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public byte[] RowId { get; set; }

    }
}
