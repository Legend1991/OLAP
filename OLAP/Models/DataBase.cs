using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OLAP.Models
{
    [Bind(Exclude = "Id")]
    public class DataBase
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }

        public List<Dimension> Dimensions { get; set; }
    }
}