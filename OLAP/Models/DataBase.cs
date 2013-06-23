using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OLAP.Models
{
    public class DataBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }

        public List<Dimension> Dimensions { get; set; }
    }
}