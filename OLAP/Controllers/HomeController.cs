using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using OLAP.Models;

namespace OLAP.Controllers
{
    public class HomeController : Controller
    {
        OLAPEntities olapDB = new OLAPEntities();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Manage()
        {
            ViewData["dataBases"] = olapDB.DataBases.ToList();
            ViewData["dimensions"] = olapDB.Dimensions.ToList();
            //ViewData["measures"] = olapDB.Measures.ToList();

            return View();
        }

        [HttpPost]
        public ActionResult AddDataBase(HttpPostedFileBase filePath, string name)
        {
            name = name.Trim();
            string path = Server.MapPath("~/Files");
            if (filePath != null)
            {
                var serverName = new StringBuilder(Guid.NewGuid().ToString());
                serverName.Append(filePath.FileName, filePath.FileName.Length - 4, 4);
                filePath.SaveAs(Path.Combine(path, serverName.ToString()));

                DataBase db = new DataBase
                {
                    FileName = serverName.ToString(),
                    Name = name,
                };

                olapDB.DataBases.Add(db);
                olapDB.SaveChanges();
            }
            return RedirectToAction("Manage");
        }

        [HttpGet]
        public ActionResult DeleteDataBase(string name)
        {
            DataBase db = olapDB.DataBases.Single(d => d.Name == name);
            var filePath = Path.Combine(Server.MapPath("~/Files"), db.FileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);

                olapDB.DataBases.Remove(db);
                olapDB.SaveChanges();
            }
            return RedirectToAction("Manage");
        }

        [HttpPost]
        public ActionResult AddDimension(string name, string tableName, string baseName)
        {
            name = name.Trim();
            DataBase db = olapDB.DataBases.Single(b => b.Name == baseName);
            Dimension d = new Dimension
            {
                DataBaseId = db.Id,
                TableName = tableName,
                Name = name,
            };

            olapDB.Dimensions.Add(d);
            olapDB.SaveChanges();
            return RedirectToAction("Manage");
        }

        [HttpGet]
        public ActionResult DeleteDimension(string name)
        {
            Dimension dim = olapDB.Dimensions.Single(d => d.Name == name);
            olapDB.Dimensions.Remove(dim);
            olapDB.SaveChanges();
            return RedirectToAction("Manage");
        }

        [HttpPost]
        public ActionResult AddMeasure(string name, string columnName, string dimName)
        {
            name = name.Trim();
            Dimension dim = olapDB.Dimensions.Single(d => d.Name == dimName);
            Measure m = new Measure
            {
                DimensionsId = dim.Id,
                ColumnName = columnName,
                Name = name,
                Priority = olapDB.Measures.ToList().Last().Priority + 1,
            };

            olapDB.Measures.Add(m);
            olapDB.SaveChanges();
            return RedirectToAction("Manage");
        }

        [HttpGet]
        public ActionResult DeleteAddMeasure(string name)
        {
            Measure meas= olapDB.Measures.Single(m => m.Name == name);
            olapDB.Measures.Remove(meas);
            olapDB.SaveChanges();
            return RedirectToAction("Manage");
        }
    }
}
