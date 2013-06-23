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
            var dataBases = olapDB.DataBases.ToList();

            return View(dataBases);
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
    }
}
