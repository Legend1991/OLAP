using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OLAP.Models
{
    public class Measure
    {
        public int Id { get; set; }
        public int DimensionsId { get; set; }
        public string ColumnName { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }

        public virtual Dimension Dimension { get; set; }
    }
}