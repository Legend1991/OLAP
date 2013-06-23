using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace OLAP.Models
{
    public class OLAPEntities : DbContext
    {
        public DbSet<DataBase> DataBases { get; set; }
        public DbSet<Dimension> Dimensions { get; set; }
        public DbSet<Measure> Measures { get; set; }
    }
}