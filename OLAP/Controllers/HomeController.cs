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
            ViewData["dataBases"] = olapDB.DataBases.ToList();
            return View();
        }

        public ActionResult Manage()
        {
            ViewData["dataBases"] = olapDB.DataBases.ToList();
            return View();
        }

        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ShowDimensions(string baseName)
        {
            var dims = SelectDimensions(olapDB.Dimensions.ToList(), baseName);
            return Json(dims);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ShowMeasures(string dimName)
        {
            ViewData["measuresList"] = SelectMeasures(olapDB.Measures.ToList(), dimName);
            return Json("Manage");
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
            Dimension dim = new Dimension
            {
                DataBaseId = db.Id,
                TableName = tableName,
                Name = name,
            };

            olapDB.Dimensions.Add(dim);
            //olapDB.SaveChanges();
            ViewData["dimensions"] = SelectDimensions(olapDB.Dimensions.ToList(), baseName);
            return PartialView("Manage");
        }

        [HttpGet]
        public ActionResult DeleteDimension(string name)
        {
            Dimension dim = olapDB.Dimensions.Single(d => d.Name == name);
            olapDB.Dimensions.Remove(dim);
            olapDB.SaveChanges();
            return PartialView("Manage");
        }

        [HttpPost]
        public ActionResult AddMeasure(string name, string columnName, string dimName)
        {
            name = name.Trim();
            Dimension dim = olapDB.Dimensions.Single(d => d.Name == dimName);
            Measure meas = new Measure
            {
                DimensionsId = dim.Id,
                ColumnName = columnName,
                Name = name,
                Priority = olapDB.Measures.ToList().Last().Priority + 1,
            };

            olapDB.Measures.Add(meas);
            olapDB.SaveChanges();
            ViewData["measuresList"] = SelectMeasures(olapDB.Measures.ToList(), dimName);
            return PartialView("Manage");
        }

        [HttpGet]
        public ActionResult DeleteAddMeasure(string name)
        {
            Measure meas= olapDB.Measures.Single(m => m.Name == name);
            olapDB.Measures.Remove(meas);
            olapDB.SaveChanges();
            return PartialView("Manage");
        }

        private List<DimensionJson> SelectDimensions(List<Dimension> dims, string baseName)
        {
            List<DimensionJson> result = new List<DimensionJson>();
            foreach (Dimension d in dims)
            {
                if (d.DataBase.Name == baseName)
                {
                    result.Add(new DimensionJson
                    {
                        Name = d.Name,
                        TableName = d.TableName
                    });
                }
            }
            return result;
        }

        private List<Measure> SelectMeasures(List<Measure> meas, string dimName)
        {
            List<Measure> result = new List<Measure>();
            foreach (Measure m in meas)
            {
                if (m.Dimension.Name == dimName)
                {
                    result.Add(m);
                }
            }
            return result;
        }
    }
}
