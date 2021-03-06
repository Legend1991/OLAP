﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace OLAP.Models
{
    [Bind(Exclude = "Id")]
    public class Fact
    {
        [Key]
        public int Id { get; set; }
        public int DataBaseId { get; set; }
        public string TableName { get; set; }
        public string RowName { get; set; }
        public string Name { get; set; }

        public virtual DataBase DataBase { get; set; }
    }
}