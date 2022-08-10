using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace ConsoleBlob
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter root directory and press enter");

            string line = Console.ReadLine();  //Pause to check command window for results; hit any key to close window 
            //string PathVar = "c:\\temp\\";
            string PathVar = line;
            string[] filesindirectory = Directory.GetFiles(PathVar, "*.*", SearchOption.AllDirectories);
            List<String> images = new List<string>(filesindirectory.Count());

            foreach (string item in filesindirectory)
            {
                if (item.Contains(".pdf") || item.Contains(".png") || item.Contains(".jpg"))
                //if (item.Contains(".png"))
                {
                    string s = StoreFiles(item);

                    Console.WriteLine(item);
                    Console.ReadLine();
                }
            }

        }

        private static string StoreFiles(string pstrPath)
        {
            Stream theStream = null;

            Boolean FileOK = false;
            const int BUFFER_SIZE = 2555555;
            Byte[] lstrBytes = new Byte[BUFFER_SIZE];
            Boolean FileSaved = false;

            lstrBytes = File.ReadAllBytes(pstrPath);

            //if (Upload.HasFile)
            //{
            //    String FileExtension = Path.GetExtension(Upload.FileName.ToString()).ToLower();
            //    lstrBytes = Upload.FileBytes;
            //    String[] allowedExtensions = { ".pdf", ".jpg" };
            //    for (int i = 0; i < allowedExtensions.Length; i++)
            //    {
            //        if (FileExtension == allowedExtensions[i])
            //        {
            //            FileOK = true;
            //        }
            //    }
            //}


            try
            {
                StringBuilder strUploadedContent = new StringBuilder("");
                StringBuilder strErrorContent = new StringBuilder("");

                StreamReader sr = new StreamReader(pstrPath);

                //*****************

                theStream = sr.BaseStream; // Upload.PostedFile.InputStream;

                string lstrFilename = Path.GetFileName(pstrPath);
                DateTime ldModDate = File.GetLastWriteTime(pstrPath);
                
                ////string FilecontentType = Upload.PostedFile.ContentType;

                //using (Stream s = Upload.PostedFile.InputStream)
                using (Stream s = theStream)
                {
                    using (BinaryReader br = new BinaryReader(s))
                    {
                        byte[] Databytes = br.ReadBytes((Int32)s.Length);

                        //fetch connection string from the web.config file  
                        string ConnectionStrings = "Server=localhost\\SQLEXPRESS;Database=Blobtest;Trusted_Connection=True;";
                        //create a database connection object  
                        using (SqlConnection con = new SqlConnection(ConnectionStrings))
                        {
                            string query = "INSERT INTO [Blob] VALUES (@FileName, @BlobData, @Description, @ModDate)";
                            //create an object for SQL command class  
                            using (SqlCommand cmd = new SqlCommand(query))
                            {
                                cmd.Connection = con;
                                cmd.Parameters.AddWithValue("@FileName", lstrFilename.Trim());
                                cmd.Parameters.AddWithValue("@BlobData", Databytes);
                                cmd.Parameters.AddWithValue("@Description", "");
                                cmd.Parameters.AddWithValue("@ModDate", ldModDate);
                                con.Open();
                                cmd.ExecuteNonQuery();
                                con.Close();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                //lblError.Text = "File could not be uploaded." + ex.Message.ToString();
                //lblError.Visible = true;
                //FileSaved = false;
                //lblErrors.ForeColor = System.Drawing.Color.Red;
                //lblErrorTitle.Visible = false;
                //pnlErr.Visible = true;
                //btnBack.Visible = true;
                //pnlMain.Visible = false;
                //return;
            }

            return "success";
        }
    }
}
