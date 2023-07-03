using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ClientserachUtiliy.Controllers
{
    public class ExcelFileDownloadController : Controller
    {
        [HttpPost]
        public FileResult ExcelFileGenrate()
        {
            
           
                if (Session["UserID"] != null)
                {
                    DataSet ds = new DataSet();

                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["SPMConnection"]))
                    {

                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "DowloadExcelFile";
                            cmd.Parameters.AddWithValue("@Typebtn", "Mobile");
                            cmd.Parameters.AddWithValue("@Fromdate", "2018-06-25");
                            cmd.Parameters.AddWithValue("@Todate", "2019-06-25");

                            cmd.CommandTimeout = 0;
                            cmd.Connection = conn;
                            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                            {
                                sda.Fill(ds);
                            }
                        }
                    }
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            wb.Worksheets.Add(ds.Tables[0]);
                            using (MemoryStream stream = new MemoryStream())
                            {
                                wb.SaveAs(stream);
                                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "mobile.xlsx");
                            }


                        }


                    }
                }
                return null;


        }
        public FileResult DownloadModificationText(string FileName)
        {
            try
            {
                string fileName = FileName;
                //Get the temp folder and file path in server
                string fullPath = Path.Combine(Server.MapPath("~/ExcelFile"), fileName);
                byte[] fileByteArray = System.IO.File.ReadAllBytes(fullPath);
                System.IO.File.Delete(fullPath);
                return File(fileByteArray, "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {

                return null;
            }

        }
    }
}