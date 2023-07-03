using ClientserachUtiliy.Models;
using ClosedXML.Excel;
using ExcelDataReader;
using OfficeOpenXml;
using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;


namespace ClientserachUtiliy.Controllers
{
    [Authorize]
    public class ClientController : Controller
    {
        // GET: Client



        public ActionResult Fresh()
        {
            return View();
        }

        [HttpPost]
        public JsonResult FreshTables(string ClientCode, string type)
        {
            if (Session["UserID"] != null)
            {
                ViewBag.type = type;
                DataSet dsDbResponse = new DataSet();

                using (SqlConnection sqlconn = new SqlConnection(ConfigurationManager.AppSettings["SPMConnectionLIVE"]))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {

                        if (ClientCode != null && type != null)
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "sp_getclientdetails_CBOS_DION";
                            cmd.Connection = sqlconn;
                            cmd.CommandTimeout = 0;

                            cmd.Parameters.AddWithValue("@p_clientcode", ClientCode);
                            cmd.Parameters.AddWithValue("@Type", type);
                        }

                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            sda.Fill(dsDbResponse);
                        }
                    }
                }

                if (dsDbResponse != null)
                {
                    if (type == "clientDetail")
                    {
                        return Json(new
                        {
                            Status = 1,
                            Message = "Success",
                            htmlcontent = ControllerExtention.RenderPartialViewToString(this, "_Freshtable", dsDbResponse)
                        });
                    }
                    else if (type == "HOLIDING")
                    {
                        return Json(new
                        {
                            Status = 1,
                            Message = "Success",
                            htmlcontent = ControllerExtention.RenderPartialViewToString(this, "_Holding", dsDbResponse)
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = 1,
                            Message = "Success",
                            htmlcontent = ControllerExtention.RenderPartialViewToString(this, "_MAPPING", dsDbResponse)
                        });
                    }

                }
                else
                {
                    return Json(new
                    {
                        Status = 0,
                        Message = "Fail",
                        HtmlContent = ""
                    });
                }
            }
            else
            {
                return Json(new
                {
                    Status = -1,
                    Message = "Fail",
                    url = ConfigurationManager.AppSettings["SessionExpired"].ToString()
                });
            }

        }


        public ActionResult GetdetailsCilent()
        {
            return View();
        }



        public ActionResult ExportToTextFile(string txtclientcode, string Exchangetype)
        {
            
            var fileName = "";
            try
            {

                if (Session["UserID"] != null)
                {
                    DataSet dsDbResponse = new DataSet();
                    using (SqlConnection sqlconn = new SqlConnection(ConfigurationManager.AppSettings["SPMconnectionli"]))
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "uspGetClientDetailForMFBSE";
                            cmd.Connection = sqlconn;


                            cmd.Parameters.AddWithValue("@vcClientCode", txtclientcode);
                            cmd.Parameters.AddWithValue("@FileType", "UCC");
                            cmd.Parameters.AddWithValue("@ExchangeType", Exchangetype);
                            cmd.Parameters.AddWithValue("@RequestedBy", "API");
                            cmd.Parameters.AddWithValue("@Filter1", "");
                            cmd.Parameters.AddWithValue("@inErrorCode", "");
                            cmd.Parameters.AddWithValue("@vcMessage", "");

                            cmd.CommandTimeout = 0;

                            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                            {
                                sda.Fill(dsDbResponse);
                            }
                        }
                    }
                    if (dsDbResponse.Tables[1].Rows.Count > 0)
                    {
                        var delimeter = "|";
                        System.Data.DataTable dtProduct = dsDbResponse.Tables[1];
                        StringBuilder sb = new StringBuilder();
                        string Columns = string.Empty;
                        foreach (DataRow datarow in dtProduct.Rows)
                        {
                            string row = string.Empty;
                            foreach (object items in datarow.ItemArray)
                            {


                                row += items.ToString() + delimeter;
                            }
                            //sb.AppendFormat(Environment.NewLine);
                            sb.Append(row.Remove(row.Length - 1, 1));
                        }
                        fileName = "UCCFILE" + ".txt";
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Response.Buffer = true;
                        Response.ContentType = "application/Text";
                        Response.AddHeader("Content-Disposition", "attachment; filename= " + fileName + ".txt;");



                        //Save the file to server temp folder
                        string fullPath = Path.Combine(Server.MapPath("~"), fileName);
                        FileStream fParameter = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                        StreamWriter m_WriterParameter = new StreamWriter(fParameter);
                        m_WriterParameter.BaseStream.Seek(0, SeekOrigin.End);
                        m_WriterParameter.Write(sb.ToString());
                        m_WriterParameter.Flush();
                        m_WriterParameter.Close();
                        return Json(new
                        {
                            Status = 1,
                            Message = "dowload File",
                            filename = fileName
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = 0,
                            Message = "No Records Enter Valide ClientCode!..."
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Status = -1,
                        Message = "Fail",
                        url = ConfigurationManager.AppSettings["SessionExpired"].ToString()
                    });
                }

            }
            catch (Exception ex)
            {
                //ErrorMaster.ErrorLog(Session["SPM_USERID"], this.ControllerContext, ex);
                return RedirectToAction("DPModification_Text", "Client");
            }


        }

        [HttpPost]
        public ActionResult ExportTextFileCDSL(string clientCodeCDSL)
        {
            var Filename = "";
            try
            {
                if (Session["UserID"] != null)
                {
                    DataSet ds = new DataSet();
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["SPMconnectionli"]))
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "GetClientDetailForMFBSECDSL_textFile_Dowload";
                            cmd.Connection = conn;
                            cmd.Parameters.AddWithValue("@ClientCode", clientCodeCDSL);
                            cmd.CommandTimeout = 0;

                            using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                            {
                                adp.Fill(ds);
                            }
                        }
                    }

                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        var delimeter = "|";
                        System.Data.DataTable dtProduct = ds.Tables[0];
                        StringBuilder sb = new StringBuilder();
                        string Columns = string.Empty;
                        foreach (DataRow datarow in dtProduct.Rows)
                        {
                            string row = string.Empty;
                            foreach (object items in datarow.ItemArray)
                            {


                                row += items.ToString() + delimeter;
                            }
                            //sb.AppendFormat(Environment.NewLine);
                            sb.Append(row.Remove(row.Length - 1, 1));
                        }
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Response.Buffer = true;
                        Response.ContentType = "application/Text";
                        Response.AddHeader("Content-Disposition", "attachment; filename= " + Filename + ".txt;");

                        Filename = "CDSLFIle" + ".txt";
                        string Fullpath = Path.Combine(Server.MapPath("~"), Filename);
                        FileStream fileStream = new FileStream(Fullpath, FileMode.Create, FileAccess.Write);
                        StreamWriter streamWriter = new StreamWriter(fileStream);
                        streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                        streamWriter.Write(sb.ToString());
                        streamWriter.Flush();
                        streamWriter.Close();
                        return Json(new
                        {
                            Status = 1,
                            Message = "DOWNLOAD CDSL FILE",
                            filename = Filename

                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = 0,
                            Message = "No Records Enter Valide ClientCode!..."

                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Status = -1,
                        Message = "Fail",
                        url = ConfigurationManager.AppSettings["SessionExpired"].ToString()
                    });
                }
            }

            catch (Exception ex)
            {
                return RedirectToAction("DPModification_Text", "Client");
            }
        }



        [HttpPost]
        public ActionResult ExporttextFileNSDL(string clientCodoNSDL)
        {
            var filename = "";
            try
            {
                if (Session["UserID"] != null)
                {
                    DataSet _dsresponse = new DataSet();
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["SPMconnectionli"]))
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "GetClientDetailForMFBSENSDL_textFile_Dowload";
                            cmd.Parameters.AddWithValue("@clientCode", clientCodoNSDL);
                            cmd.CommandTimeout = 0;
                            cmd.Connection = conn;

                            using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                            {
                                adp.Fill(_dsresponse);
                            }
                        }
                    }

                    if (_dsresponse.Tables[0].Rows.Count > 0)
                    {
                        System.Data.DataTable tb = _dsresponse.Tables[0];
                        StringBuilder sb = new StringBuilder();
                        string deliminater = "|";
                        foreach (DataRow row in tb.Rows)
                        {
                            string rows = string.Empty;
                            foreach (object items in row.ItemArray)
                            {
                                rows += items.ToString() + deliminater;
                            }
                            sb.Append(rows.Remove(rows.Length - 1, 1));
                        }
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Response.Buffer = true;
                        Response.ContentType = "application/Text";
                        Response.AddHeader("Content-Disposition", "attachment; filename= " + filename + ".txt;");

                        filename = "NOOMINEE FILE" + ".txt";

                        string fullpath = Path.Combine(Server.MapPath("~"), filename);
                        FileStream fileStream = new FileStream(fullpath, FileMode.Create, FileAccess.Write);
                        StreamWriter streamWriter = new StreamWriter(fileStream);
                        streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                        streamWriter.Write(sb.ToString());
                        streamWriter.Flush();
                        streamWriter.Close();
                        return Json(new
                        {
                            Status = 1,
                            Message = "DOWLOAD NOMINEE File",
                            filename = filename
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = 0,
                            Message = "No Records Enter Valide ClientCode!..."
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Status = -1,
                        Message = "Fail",
                        url = ConfigurationManager.AppSettings["SessionExpired"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                //ErrorMaster.ErrorLog(Session["SPM_USERID"], this.ControllerContext, ex);
                return RedirectToAction("DPModification_Text", "Client");
            }

        }

        [HttpPost]
        public ActionResult ExportBankFile(string clientCode)
        {

            var filename = "";
            try
            {
                if (Session["UserID"] != null)
                {
                    DataSet _dsresponse = new DataSet();
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["SPMConnectionLIVE"]))
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandText = "USP_GETCLIENTDETAILFORMFBSENEW_FORBANK_TEMP";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Parameters.AddWithValue("@vcClientCode", clientCode);
                            cmd.Parameters.AddWithValue("@FileType", "single");
                            cmd.CommandTimeout = 0;
                            cmd.Connection = conn;

                            using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                            {
                                adp.Fill(_dsresponse);
                            }
                        }
                    }

                    if (_dsresponse.Tables[0].Rows.Count > 0)
                    {
                        System.Data.DataTable tb = _dsresponse.Tables[0];
                        StringBuilder sb = new StringBuilder();
                        string deliminater = "|";
                        foreach (DataRow row in tb.Rows)
                        {
                            string rows = string.Empty;
                            foreach (object items in row.ItemArray)
                            {
                                rows += items.ToString() + deliminater;
                            }
                            sb.Append(rows.Remove(rows.Length - 1, 1));
                        }
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Response.Buffer = true;
                        Response.ContentType = "application/Text";
                        Response.AddHeader("Content-Disposition", "attachment; filename= " + filename + ".txt;");

                        filename = "BANK FILE" + ".txt";

                        string fullpath = Path.Combine(Server.MapPath("~"), filename);
                        FileStream fileStream = new FileStream(fullpath, FileMode.Create, FileAccess.Write);
                        StreamWriter streamWriter = new StreamWriter(fileStream);
                        streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                        streamWriter.Write(sb.ToString());
                        streamWriter.Flush();
                        streamWriter.Close();
                        return Json(new
                        {
                            Status = 1,
                            Message = "DOWLOAD BANK File",
                            filename = filename
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = 0,
                            Message = "No Records Enter Valide ClientCode!..."
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Status = -1,
                        Message = "Fail",
                        url = ConfigurationManager.AppSettings["SessionExpired"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                //ErrorMaster.ErrorLog(Session["SPM_USERID"], this.ControllerContext, ex);
                return RedirectToAction("DPModification_Text", "Client");
            }

        }


        public ActionResult GenrateFileAppMFI_MFD(string ClientCode, string Exchanangetype)
        {
            var fileName = "";
            try
            {
                if (Session["UserID"] != null)
                {
                    DataSet dsDbResponse = new DataSet();
                    using (SqlConnection sqlconn = new SqlConnection(ConfigurationManager.AppSettings["SPMConnectionLIVE"]))
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "Genrate_file_App_Name";
                            cmd.Connection = sqlconn;


                            cmd.Parameters.AddWithValue("@ClientCode", ClientCode);
                            cmd.Parameters.AddWithValue("@ExchangeType", Exchanangetype);

                            cmd.CommandTimeout = 0;

                            using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                            {
                                sda.Fill(dsDbResponse);
                            }
                        }
                    }
                    if (dsDbResponse.Tables[0].Rows.Count > 0)
                    {
                        var delimeter = "|";
                        System.Data.DataTable dtProduct = dsDbResponse.Tables[0];
                        StringBuilder sb = new StringBuilder();
                        string Columns = string.Empty;
                        foreach (DataRow datarow in dtProduct.Rows)
                        {
                            string row = string.Empty;
                            foreach (object items in datarow.ItemArray)
                            {


                                row += items.ToString() + delimeter;
                            }
                            //sb.AppendFormat(Environment.NewLine);
                            sb.Append(row.Remove(row.Length - 1, 1));
                        }
                        fileName = "GenrateAppFile" + ".txt";
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Response.Buffer = true;
                        Response.ContentType = "application/Text";
                        Response.AddHeader("Content-Disposition", "attachment; filename= " + fileName + ".txt;");



                        //Save the file to server temp folder
                        string fullPath = Path.Combine(Server.MapPath("~"), fileName);
                        FileStream fParameter = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                        StreamWriter m_WriterParameter = new StreamWriter(fParameter);
                        m_WriterParameter.BaseStream.Seek(0, SeekOrigin.End);
                        m_WriterParameter.Write(sb.ToString());
                        m_WriterParameter.Flush();
                        m_WriterParameter.Close();
                        return Json(new
                        {
                            Status = 1,
                            Message = "GenrateAppFile dowloaded",
                            filename = fileName
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = 0,
                            Message = "No Records Enter Valide ClientCod  "
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Status = -1,
                        Message = "Fail",
                        url = ConfigurationManager.AppSettings["SessionExpired"].ToString()
                    });
                }

            }
            catch (Exception ex)
            {
                //ErrorMaster.ErrorLog(Session["SPM_USERID"], this.ControllerContext, ex);
                return RedirectToAction("DPModification_Text", "Client");
            }

        }
        public ActionResult DownloadModificationText(string FileName)
        {
            try
            {
                string fileName = FileName.ToString();
                //Get the temp folder and file path in server
                string fullPath = Path.Combine(Server.MapPath("~"), fileName);
                byte[] fileByteArray = System.IO.File.ReadAllBytes(fullPath);
                System.IO.File.Delete(fullPath);
                return File(fileByteArray, "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {

                return null;
            }

        }


        public ActionResult Recancilation()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Dowload(FormCollection cl)
        {
            var data = "";

            var cl1 = cl["missingdata"];
            var cl2 = cl["missingclient"];

            if (cl1 != "")
            {
                data = "missingdata";
            }
            else
            {
                data ="missingclient";
            }
            DataSet ds = new DataSet();

            if (Session["UserId"] != null)
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["SPMConnection"]))
                {
                   
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "DowloadExcelFile";
                        cmd.Parameters.AddWithValue("@Typebtn", cl[data]);
                        cmd.Parameters.AddWithValue("@Fromdate", cl["formdate"]);
                        cmd.Parameters.AddWithValue("@Todate", cl["todate"]);

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
                    using (XLWorkbook workbook = new XLWorkbook())
                    {

                        var ws = workbook.Worksheets.Add(ds.Tables[0]);

                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";

                        using (MemoryStream stream = new MemoryStream())
                        {
                            ws.Rows().Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            ws.Columns().AdjustToContents();
                            ws.Range("A1:C200").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                            ws.Range("A1:C1").Style.Fill.BackgroundColor = XLColor.Red;
                            ws.Range("A1:C1").Style.Font.Bold = true;

                            //ws.Range("A1:C200").Style.Border.TopBorder = XLBorderStyleValues.Thin; ws.Range("A1:C200").Style.Border.InsideBorder = XLBorderStyleValues.Thin; ws.Range("A1:C200").Style.Border.OutsideBorder = XLBorderStyleValues.Thin; ws.Range("A1:D4").Style.Border.LeftBorder = XLBorderStyleValues.Thin; ws.Range("A1:C200").Style.Border.RightBorder = XLBorderStyleValues.Thin; ws.Range("A1:C200").Style.Border.TopBorder = XLBorderStyleValues.Thin;
                            workbook.SaveAs(stream);


                            return Json(Convert.ToBase64String(stream.ToArray(), 0, stream.ToArray().Length), JsonRequestBehavior.AllowGet);
                        }

                    }
                }
                else
                {
                    return Json(new { Status = "1", message = "Select only 1 year Data " }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new
            {
                Status = -1,
                Message = "Fail",
                url = ConfigurationManager.AppSettings["SessionExpired"].ToString()
            });


        }



        public ActionResult DowloadExcelFile()
        {
            return View();
        }

        
        public ActionResult UploadFile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadSPFile()
        {
            try
            {
                string userId = "";

                if (Session["UserID"] != null)
                {
                    userId = Convert.ToString(Session["UserID"]);
                }
                else
                {
                    return Json(new
                    {
                        Status = "-1",
                        Message = ConfigurationManager.AppSettings["SessionExpired"].ToString()
                    }) ;
                }

                if (Request.Files.Count == 0)
                {
                    return Json(new
                    {
                        Status = "0",
                        Message = "Please upload a file"
                    });
                }

                HttpPostedFileBase TradeFile = Request.Files[0];

                if (Path.GetExtension(TradeFile.FileName) != ".csv" && Path.GetExtension(TradeFile.FileName) != ".xlsx" && Path.GetExtension(TradeFile.FileName) != ".xls" && Path.GetExtension(TradeFile.FileName)!=".txt")
                {
                    return Json(new
                    {
                        Status = "0",
                        Message = "Please upload .xls/.xlsx/.csv file only."
                    });
                }

                string saveFolderPath = Server.MapPath("~/UploadFile/");
                string saveFilePath = "";

                if (!Directory.Exists(saveFolderPath))
                {
                    Directory.CreateDirectory(saveFolderPath);
                }

                DateTime timeNow = DateTime.Now.ToUniversalTime();
                string fileName = userId + "_" + timeNow.Year + timeNow.Month + timeNow.Day + timeNow.Hour + timeNow.Minute + timeNow.Second + timeNow.Millisecond;
                saveFilePath = saveFolderPath + fileName + Path.GetExtension(TradeFile.FileName);
                TradeFile.SaveAs(saveFilePath);

                DataTable dt = new DataTable();

                if (Path.GetExtension(TradeFile.FileName).ToUpper() == ".CSV")
                {
                    //ReadCSV
                    dt = ReadCSV(saveFilePath);
                    System.IO.File.Delete(saveFilePath);
                }
                else if (Path.GetExtension(TradeFile.FileName).ToUpper() == ".XLSX" || Path.GetExtension(TradeFile.FileName).ToUpper() == ".XLS")
                {
                    //ReadExcelsheet
                    dt = ReadExcelFileData(saveFilePath, Path.GetExtension(TradeFile.FileName).ToUpper());
                    
                    System.IO.File.Delete(saveFilePath);
                }
                else if (Path.GetExtension(TradeFile.FileName).ToUpper() == ".TXT")
                {
                    //ReadExcelsheet
                    dt = ReadtextFileData(saveFilePath, Path.GetExtension(TradeFile.FileName).ToUpper());
                    
                    System.IO.File.Delete(saveFilePath);
                }
                else
                {
                    System.IO.File.Delete(saveFilePath);
                    return Json(new
                    {
                        Status = "0",
                        Message = "Invalid file uploaded."
                    });
                }

                if (dt == null)
                {
                    return Json(new
                    {
                        Status = "0",
                        Message = "Something went wrong at runtime!"
                    });
                }

                if (dt.Rows.Count == 0)
                {
                    return Json(new
                    {
                        Status = "0",
                        Message = "Uploaded file is empty, please upload file having data."
                    });
                }
                
               
                 
                 //InsertToDB(dt);
                return Json(new
                {
                    Status = "1",
                    Message = "File uploaded successfully."

                });
               
            }
            catch (Exception ex)
            {
                // ErrorLogger.LogErrorToFile(Session["SPM_USERID"], this.ControllerContext, ex);
                
                return Json(new
                {
                    Status = "0",
                    Message = "Something went wrong at runtime!"
                });
            }
        }
       

        public DataTable ReadExcelFileData(string ext, string filePath)
        {
            DataTable dtFileData = new DataTable();
            
            string excelConnectionString = "";
            if (filePath.ToLower() == ".xls")
            {
                excelConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;IMEX=1;HDR=YES\"", ext);
            }
            else
            {
                excelConnectionString = String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 8.0;IMEX=1;HDR=YES\"", ext);
                // public static string connStr        = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=Excel 12.0;"
            }
            try
            {
                using (OleDbConnection connection = new OleDbConnection(excelConnectionString))
                {
                    connection.Open();
                    DataTable dtSchema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    String sheetName = string.Empty;
                    foreach (DataRow row in dtSchema.Rows)
                    {
                        if (!row["TABLE_NAME"].ToString().Contains("FilterDatabase"))
                        {
                            sheetName = row["TABLE_NAME"].ToString();
                            sheetName = "[" + sheetName + "]";
                            break;
                        }
                    }
                    OleDbCommand command = new OleDbCommand("Select * FROM " + sheetName, connection);
                    // Create DbDataReader to Data Worksheet 
                    using (DbDataReader dr = command.ExecuteReader())
                    {
                        dtFileData.Load(dr);
                    }
                    dtFileData=removerNulldatatable(dtFileData);
                }
                return dtFileData;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public DataTable ReadtextFileData(string ext, string filePath)
        {
            DataTable dtFileData = new DataTable();
            //dtFileData.Columns.Add("ROWID", typeof(int));
            string excelConnectionString = "";
            if (filePath.ToLower() == ".txt")
            {
                excelConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='text;HDR=YES;FMT=Delimited'", ext);
            }
            else
            {
                excelConnectionString = String.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='text;HDR=YES;FMT=Delimited'", ext);
                // public static string connStr        = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=Excel 12.0;"
            }
            try
            {
                using (OleDbConnection connection = new OleDbConnection(excelConnectionString))
                {
                    connection.Open();
                    DataTable dtSchema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    String sheetName = string.Empty;
                    foreach (DataRow row in dtSchema.Rows)
                    {
                        if (!row["TABLE_NAME"].ToString().Contains("FilterDatabase"))
                        {
                            sheetName = row["TABLE_NAME"].ToString();
                            sheetName = "[" + sheetName + "]";
                            break;
                        }
                    }
                    OleDbCommand command = new OleDbCommand("Select * FROM " + sheetName, connection);
                    // Create DbDataReader to Data Worksheet 
                    using (DbDataReader dr = command.ExecuteReader())
                    {
                        dtFileData.Load(dr);
                    }
                    dtFileData = removerNulldatatable(dtFileData);
                }
                return dtFileData;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        

        public DataTable ReadCSV(string filePath)
        {
            try
            {
                DataTable dtDataSource = new DataTable();
                string[] fileContent = System.IO.File.ReadAllLines(filePath);
                if (fileContent.Count() > 0)
                {
                    string[] columns = fileContent[0].Split(',');
                    for (int i = 0; i < columns.Count(); i++)
                    {
                        dtDataSource.Columns.Add(columns[i]);
                    }
                    for (int i = 1; i < fileContent.Count(); i++)
                    {
                        string[] rowData = fileContent[i].Split(',');
                        dtDataSource.Rows.Add(rowData);
                    }
                }
                return dtDataSource;
            }
            catch (Exception ex)
            {
                // ErrorLogger.LogErrorToFile(Session["SPM_USERID"], this.ControllerContext, ex);
                
                return null;
            }
        }

        public void InsertToDB(DataTable dtExcel)
        {
            DataSet ds = new DataSet();
            DataTable dtResponse = new DataTable();

            try
            {

                DataTable dtDbData = dtExcel;

                DataSet dsDbResponse = new DataSet();

                using (SqlConnection sqlconn = new SqlConnection(ConfigurationManager.AppSettings["SPMConnection"]))
                {

                    using (SqlCommand cmd = new SqlCommand("Insert_Customers"))
                    {
                        using (SqlDataAdapter sda = new SqlDataAdapter())
                        {
                            cmd.Connection = sqlconn;
                           
                            var tableparameter = new SqlParameter("@tblCustomers", SqlDbType.Structured);
                            tableparameter.TypeName = "dbo.ClientCodetype";
                            tableparameter.Value = dtDbData;
                            cmd.Parameters.Add(tableparameter);
                            cmd.CommandType = CommandType.StoredProcedure;
                            sda.SelectCommand = cmd;
                            using (DataTable dt = new DataTable())
                            {
                                sda.Fill(dt);

                                
                            }
                        }
                    }

                }

                
                
            }
            catch (Exception ex)
            {
                //ErrorLogger.LogErrorToFile(Session["SPM_USERID"], this.ControllerContext, ex);
                
                
            }
        }

        public DataTable removerNulldatatable(DataTable dts)
        {
            DataTable dt1 = dts;

            for (int i = dt1.Rows.Count - 1; i >= 0; i--)
            {
                if (dt1.Rows[i][0] == DBNull.Value)
                {
                    dt1.Rows[i].Delete();
                }
            }
            dt1.AcceptChanges();
            return dt1;
        }


        [HttpPost]
        public ActionResult GenrateUsstextFile()
        {

            var filename = "";
            try
            {
                if (Session["UserID"] != null)
                {
                    DataSet _dsresponse = new DataSet();
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["SPMconnectionli"]))
                    {
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandText = "uspGetClientDetailForMFBSENEW_Multiple";
                            cmd.Parameters.AddWithValue("@vcClientCode", "N-1");
                            cmd.Parameters.AddWithValue("@FileType", "UCC");
                            cmd.Parameters.AddWithValue("@ExchangeType", "MFI");
                            cmd.Parameters.AddWithValue ("@RequestedBy", "API'");
                            cmd.Parameters.AddWithValue ("@Filter1", "");
                            cmd.Parameters.AddWithValue ("@inErrorCode", "");
                            cmd.Parameters.AddWithValue ("@vcMessage", "");
                            cmd.CommandTimeout = 0;
                            cmd.Connection = conn;

                            using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                            {
                                adp.Fill(_dsresponse);
                            }
                        }
                    }

                    if (_dsresponse.Tables[0].Rows.Count > 0)
                    {
                        System.Data.DataTable tb = _dsresponse.Tables[0];
                        StringBuilder sb = new StringBuilder();
                        string deliminater = "|";
                        foreach (DataRow row in tb.Rows)
                        {
                            string rows = string.Empty;
                            foreach (object items in row.ItemArray)
                            {
                                rows += items.ToString() + deliminater;
                            }
                            sb.Append(rows.Remove(rows.Length - 1, 1));
                        }
                        Response.ClearContent();
                        Response.ClearHeaders();
                        Response.Buffer = true;
                        Response.ContentType = "application/Text";
                        Response.AddHeader("Content-Disposition", "attachment; filename= " + filename + ".txt;");

                        filename = "UCC FILE" + ".txt";

                        string fullpath = Path.Combine(Server.MapPath("~"), filename);
                        FileStream fileStream = new FileStream(fullpath, FileMode.Create, FileAccess.Write);
                        StreamWriter streamWriter = new StreamWriter(fileStream);
                        streamWriter.BaseStream.Seek(0, SeekOrigin.End);
                        streamWriter.Write(sb.ToString());
                        streamWriter.Flush();
                        streamWriter.Close();
                        return Json(new
                        {
                            Status = 1,
                            Message = "DOWLOAD UCC File",
                            filename = filename
                        });
                    }
                    else
                    {
                        return Json(new
                        {
                            Status = 0,
                            Message = "No Records Enter Valide ClientCode!..."
                        });
                    }
                }
                else
                {
                    return Json(new
                    {
                        Status = -1,
                        Message = "Fail",
                        url = ConfigurationManager.AppSettings["SessionExpired"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                //ErrorMaster.ErrorLog(Session["SPM_USERID"], this.ControllerContext, ex);
                return RedirectToAction("DPModification_Text", "Client");
            }

        }


        public ActionResult Updaterecon()
        {
            return View();
        }


        [HttpPost]
        public ActionResult UpdateRecon(string clientcode,string type,string modifytype)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["SPMconnectionli"]))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "Sp_update_recon";
                    cmd.Parameters.AddWithValue("@clientcode",clientcode);
                    cmd.Parameters.AddWithValue("@type",type);
                    cmd.Parameters.AddWithValue("@modifiye", modifytype);
                    cmd.CommandTimeout = 0;
                    cmd.Connection = conn;
                    conn.Open();
                    int a = cmd.ExecuteNonQuery();
                    if (a > 0)
                    {
                        return Json(new { status = 1, Message = "update successfully" });
                    }
                        
                    
                }
            }
            return Json(new { status = 0, Message = "Something Went Wrong" });
        }



        [HttpPost]
        public JsonResult GetdetailsRecon(string clientcode, string type)
       {
            var detail = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["SPMconnectionli"]))
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = "Sp_ClientCode_Search";
                        cmd.Parameters.AddWithValue("@clientcode", clientcode);
                        cmd.Parameters.AddWithValue("@type", type);

                        cmd.CommandTimeout = 0;
                        cmd.Connection = conn;
                        conn.Open();
                        SqlDataReader read = cmd.ExecuteReader();
                        if (!read.HasRows)
                        {
                            detail = "";
                            return Json(detail, JsonRequestBehavior.AllowGet);
                        }
                        while (read.Read())
                        {
                            detail = read[0].ToString();
                        }


                    }
                }
            }
            catch(Exception ex)
            {

            }
            return Json(detail,JsonRequestBehavior.AllowGet);
        }




        

    }
}