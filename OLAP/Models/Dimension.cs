using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OLAP.Models
{
    public class Dimension
    {
        public int Id { get; set; }
        public int DataBaseId { get; set; }
        public string TableName { get; set; }
        public string Name { get; set; }

        public virtual DataBase DataBase { get; set; }
        public List<Measure> Measures { get; set; }
    }
}