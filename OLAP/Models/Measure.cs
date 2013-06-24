using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OLAP.Models
{
    [Bind(Exclude = "Id")]
    public class Measure
    {
        [Key]
        public int Id { get; set; }
        public int DimensionId { get; set; }
        public string ColumnName { get; set; }
        public string Name { get; set; }
        public int Priority { get; set; }

        public virtual Dimension Dimension { get; set; }
    }
}