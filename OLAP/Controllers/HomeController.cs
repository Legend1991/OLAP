using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Text;
using OLAP.Models;
using System.Data.SqlServerCe;
using System.Data;

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

            DataBase db = olapDB.DataBases.Single(d => d.Name == baseName);
            SqlCeConnection myConnection = new SqlCeConnection();
            DataSet ds;
            String selTablesSQL = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES";
            string dataSource = Path.Combine(Server.MapPath("~/Files"), db.FileName);
            myConnection.ConnectionString = @"Data Source="+dataSource+";Persist Security Info=False;";
            myConnection.Open();
            using (SqlCeCommand comm = new SqlCeCommand(selTablesSQL, myConnection))
            {
                using (SqlCeDataAdapter da = new SqlCeDataAdapter(comm))
                {
                    ds = new DataSet();

                    try
                    {
                        da.Fill(ds);
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        myConnection.Close();
                    }
                }
            }

            addTabels(dims, ds);

            return Json(dims);
        }

        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ShowMeasures(string dimName)
        {
            var meas = SelectMeasures(olapDB.Measures.ToList(), dimName);

            Dimension dim = olapDB.Dimensions.Single(d => d.Name == dimName);
            SqlCeConnection myConnection = new SqlCeConnection();
            DataSet ds;
            String selColumnsSQL = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='"  +dim.TableName + "'";
            string dataSource = Path.Combine(Server.MapPath("~/Files"), dim.DataBase.FileName);
            myConnection.ConnectionString = @"Data Source=" + dataSource + ";Persist Security Info=False;";
            myConnection.Open();
            using (SqlCeCommand comm = new SqlCeCommand(selColumnsSQL, myConnection))
            {
                using (SqlCeDataAdapter da = new SqlCeDataAdapter(comm))
                {
                    ds = new DataSet();

                    try
                    {
                        da.Fill(ds);
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        myConnection.Close();
                    }
                }
            }

            addRows(meas, ds);

            return Json(meas);
        }

        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ShowFacts(string baseName)
        {
            var dims = SelectFacts(olapDB.Facts.ToList(), baseName);

            return Json(dims);
        }

        [AllowAnonymous]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ShowFactsRows(string baseName, string tableName)
        {
            List<FactJson> facts = new List<FactJson>();

            DataBase db = olapDB.DataBases.Single(d => d.Name == baseName);

            SqlCeConnection myConnection = new SqlCeConnection();
            DataSet ds;
            String selColumnsSQL = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='" + tableName + "'";
            string dataSource = Path.Combine(Server.MapPath("~/Files"), db.FileName);
            myConnection.ConnectionString = @"Data Source=" + dataSource + ";Persist Security Info=False;";
            myConnection.Open();
            using (SqlCeCommand comm = new SqlCeCommand(selColumnsSQL, myConnection))
            {
                using (SqlCeDataAdapter da = new SqlCeDataAdapter(comm))
                {
                    ds = new DataSet();

                    try
                    {
                        da.Fill(ds);
                    }
                    catch (Exception ex)
                    {
                    }
                    finally
                    {
                        myConnection.Close();
                    }
                }
            }

            addFactsRows(facts, ds);

            return Json(facts);
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
            olapDB.SaveChanges();
            ViewData["dataBases"] = olapDB.DataBases.ToList();
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
        public ActionResult AddMeasure(string name, string columnName, string dimName, string baseName)
        {
            name = name.Trim();
            Dimension dim = olapDB.Dimensions.Single(d => d.Name == dimName);
            int priority = 0;
            if (olapDB.Measures.ToList().Count != 0)
            {
                priority = olapDB.Measures.ToList().Last().Priority + 1;
            }
            Measure meas = new Measure
            {
                DimensionId = dim.Id,
                ColumnName = columnName,
                Name = name,
                Priority = priority
            };

            olapDB.Measures.Add(meas);
            olapDB.SaveChanges();
            ViewData["dataBases"] = olapDB.DataBases.ToList();
            ViewData["dimensions"] = SelectDimensions(olapDB.Dimensions.ToList(), baseName);
            ViewData["measuresList"] = SelectMeasures(olapDB.Measures.ToList(), dimName);
            return PartialView("Manage");
        }

        [HttpGet]
        public ActionResult DeleteMeasure(string name)
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
                        TableName = ""
                    });
                }
            }
            return result;
        }

        private List<MeasureJson> SelectMeasures(List<Measure> meas, string dimName)
        {
            List<MeasureJson> result = new List<MeasureJson>();
            //Dimension dim = olapDB.Dimensions.Single(d => d.Name == dimName);
            foreach (Measure m in meas)
            {
                if (m.Dimension.Name == dimName)
                {
                    result.Add(new MeasureJson
                    {
                        Name = m.Name,
                        ColumnName = ""
                    });
                }
            }
            return result;
        }

        private List<FactJson> SelectFacts(List<Fact> fact, string baseName)
        {
            List<FactJson> result = new List<FactJson>();
            foreach (Fact f in fact)
            {
                if (f.DataBase.Name == baseName)
                {
                    result.Add(new FactJson
                    {
                        Name = f.Name,
                        RowName = "",
                        TableName = ""
                    });
                }
            }
            return result;
        }

        private void addTabels(List<DimensionJson> dims, DataSet ds)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                dims.Add(new DimensionJson { Name = "", TableName = (string)row[0] });
            }
        }

        private void addRows(List<MeasureJson> meas, DataSet ds)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                meas.Add(new MeasureJson { Name = "", ColumnName = (string)row[0] });
            }
        }

        private void addFactsRows(List<FactJson> meas, DataSet ds)
        {
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                meas.Add(new FactJson { Name = "", RowName = (string)row[0], TableName="" });
            }
        }
    }
}
