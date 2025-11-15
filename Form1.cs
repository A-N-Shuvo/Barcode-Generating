//এইটা শুধু পিডিএফ সেভ করে, প্রিন্ট করে না

//using System;
//using System.Data;
//using System.Data.SqlClient;
//using System.Drawing;
//using System.IO;
//using System.Windows.Forms;
//using iTextSharp.text;
//using iTextSharp.text.pdf;
//using System.Configuration;

//namespace MultipleBarcode
//{
//    public partial class Form1 : Form
//    {
//        private string connectionString;

//        public Form1()
//        {
//            try
//            {
//                var cs = ConfigurationManager.ConnectionStrings["DefaultConnection"];
//                if (cs == null)
//                {
//                    MessageBox.Show("Connection string 'DefaultConnection' not found in App.config!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                    return; // ফর্ম আর load হবে না
//                }

//                connectionString = cs.ConnectionString;

//                InitializeComponent();
//                InitializeGrid();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error while loading Form1:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        // Initialize DataGridView
//        private void InitializeGrid()
//        {
//            gvReports.AllowUserToAddRows = false;
//            gvReports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
//            gvReports.Columns.Clear();

//            gvReports.Columns.Add("ReportId", "Report ID");
//            gvReports.Columns.Add("ReportCode", "Report Code");
//            gvReports.Columns.Add("ReportName", "Report Name");
//            gvReports.Columns.Add("Barcode", "Barcode");

//            DataGridViewButtonColumn printButton = new DataGridViewButtonColumn();
//            printButton.Name = "Print";
//            printButton.HeaderText = "Action";
//            printButton.Text = "Print";
//            printButton.UseColumnTextForButtonValue = true;
//            gvReports.Columns.Add(printButton);
//        }

//        // Load data from database
//        private void btnSearch_Click(object sender, EventArgs e)
//        {
//            string reportId = txtReportId.Text.Trim();
//            gvReports.Rows.Clear();

//            using (SqlConnection con = new SqlConnection(connectionString))
//            {
//                string query = "SELECT ReportId, ReportCode, ReportName, Barcode FROM ReportsBarcodes WHERE (@ReportId = '' OR ReportId = @ReportId)";
//                SqlCommand cmd = new SqlCommand(query, con);
//                cmd.Parameters.AddWithValue("@ReportId", reportId);
//                con.Open();

//                SqlDataReader dr = cmd.ExecuteReader();
//                while (dr.Read())
//                {
//                    gvReports.Rows.Add(
//                        dr["ReportId"].ToString(),
//                        dr["ReportCode"].ToString(),
//                        dr["ReportName"].ToString(),
//                        dr["Barcode"].ToString()
//                    );
//                }
//            }

//            if (gvReports.Rows.Count == 0)
//                MessageBox.Show("No data found for the given Report ID.", "Info");
//        }

//        // Single print
//        private void gvReports_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {
//            if (e.ColumnIndex == gvReports.Columns["Print"].Index && e.RowIndex >= 0)
//            {
//                string reportId = gvReports.Rows[e.RowIndex].Cells["ReportId"].Value.ToString();
//                string reportCode = gvReports.Rows[e.RowIndex].Cells["ReportCode"].Value.ToString();
//                string reportName = gvReports.Rows[e.RowIndex].Cells["ReportName"].Value.ToString();

//                GenerateBarcodePdf(reportId, reportCode, reportName);
//            }
//        }

//        // ✅ Print All (with Save As dialog)
//        private void btnPrintAll_Click(object sender, EventArgs e)
//        {
//            if (gvReports.Rows.Count == 0)
//            {
//                MessageBox.Show("No data to print.");
//                return;
//            }

//            string defaultFileName = $"AllBarcodes_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";

//            using (SaveFileDialog saveDialog = new SaveFileDialog())
//            {
//                saveDialog.Title = "Save All Barcodes PDF";
//                saveDialog.Filter = "PDF files (*.pdf)|*.pdf";
//                saveDialog.FileName = defaultFileName;
//                saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

//                if (saveDialog.ShowDialog() != DialogResult.OK)
//                    return;

//                string filePath = saveDialog.FileName;

//                float outerWidthPt = 1.5f * 72f;
//                float outerHeightPt = 1.0f * 72f;
//                float innerWidthPt = 1.45f * 72f;
//                float innerHeightPt = 0.95f * 72f;
//                float marginX = (outerWidthPt - innerWidthPt) / 2;
//                float marginY = (outerHeightPt - innerHeightPt) / 2;

//                using (FileStream fs = new FileStream(filePath, FileMode.Create))
//                {
//                    Document doc = new Document(new iTextSharp.text.Rectangle(outerWidthPt, outerHeightPt), 0, 0, 0, 0);
//                    PdfWriter writer = PdfWriter.GetInstance(doc, fs);
//                    doc.Open();

//                    PdfContentByte cb = writer.DirectContent;
//                    BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);

//                    bool firstPage = true;

//                    foreach (DataGridViewRow row in gvReports.Rows)
//                    {
//                        if (row.IsNewRow) continue;

//                        string reportId = row.Cells["ReportId"].Value.ToString();
//                        string reportCode = row.Cells["ReportCode"].Value.ToString();
//                        string reportName = row.Cells["ReportName"].Value.ToString();

//                        if (!firstPage)
//                            doc.NewPage();
//                        firstPage = false;

//                        Barcode128 code128 = new Barcode128
//                        {
//                            Code = reportCode.Length > 12 ? reportCode.Substring(0, 12) : reportCode,
//                            BarHeight = innerHeightPt * 0.6f,
//                            X = 1f,
//                            StartStopText = false,
//                            Font = null
//                        };

//                        iTextSharp.text.Image barcodeImage = code128.CreateImageWithBarcode(cb, null, null);
//                        barcodeImage.ScaleAbsolute(innerWidthPt, innerHeightPt * 0.6f);
//                        barcodeImage.SetAbsolutePosition(marginX, outerHeightPt - marginY - (innerHeightPt * 0.6f));
//                        doc.Add(barcodeImage);

//                        // নিচের টেক্সট
//                        cb.BeginText();
//                        cb.SetFontAndSize(bf, 8);

//                        float textAreaHeight = innerHeightPt * 0.4f;
//                        float textStartY = marginY + textAreaHeight - 10;
//                        float lineSpacing = textAreaHeight / 3.5f;
//                        float centerX = outerWidthPt / 2;

//                        cb.ShowTextAligned(Element.ALIGN_CENTER, reportId, centerX, textStartY, 0);
//                        cb.ShowTextAligned(Element.ALIGN_CENTER, reportCode, centerX, textStartY - lineSpacing, 0);
//                        cb.ShowTextAligned(Element.ALIGN_CENTER, reportName, centerX, textStartY - 2 * lineSpacing, 0);

//                        cb.EndText();
//                    }

//                    doc.Close();
//                }

//                MessageBox.Show($"All barcodes saved successfully at:\n{filePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
//            }
//        }

//        // ✅ Single print (with Save As dialog)
//        private void GenerateBarcodePdf(string reportId, string reportCode, string reportName)
//        {
//            string defaultFileName = $"{reportCode}_{DateTime.Now:yyyyMMdd_HHmmss}_Barcode.pdf";

//            using (SaveFileDialog saveDialog = new SaveFileDialog())
//            {
//                saveDialog.Title = "Save Barcode PDF";
//                saveDialog.Filter = "PDF files (*.pdf)|*.pdf";
//                saveDialog.FileName = defaultFileName;
//                saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

//                if (saveDialog.ShowDialog() != DialogResult.OK)
//                    return;

//                string filePath = saveDialog.FileName;

//                float outerWidthPt = 1.5f * 72f;
//                float outerHeightPt = 1.0f * 72f;
//                float innerWidthPt = 1.45f * 72f;
//                float innerHeightPt = 0.95f * 72f;
//                float marginX = (outerWidthPt - innerWidthPt) / 2;
//                float marginY = (outerHeightPt - innerHeightPt) / 2;

//                using (FileStream fs = new FileStream(filePath, FileMode.Create))
//                {
//                    Document doc = new Document(new iTextSharp.text.Rectangle(outerWidthPt, outerHeightPt), 0, 0, 0, 0);
//                    PdfWriter writer = PdfWriter.GetInstance(doc, fs);
//                    doc.Open();

//                    PdfContentByte cb = writer.DirectContent;

//                    Barcode128 code128 = new Barcode128
//                    {
//                        Code = reportCode.Length > 12 ? reportCode.Substring(0, 12) : reportCode,
//                        BarHeight = innerHeightPt * 0.6f,
//                        X = 1f,
//                        StartStopText = false,
//                        Font = null
//                    };

//                    iTextSharp.text.Image barcodeImage = code128.CreateImageWithBarcode(cb, null, null);
//                    barcodeImage.ScaleAbsolute(innerWidthPt, innerHeightPt * 0.6f);
//                    barcodeImage.SetAbsolutePosition(marginX, outerHeightPt - marginY - (innerHeightPt * 0.6f));
//                    doc.Add(barcodeImage);

//                    BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, false);
//                    cb.BeginText();
//                    cb.SetFontAndSize(bf, 8);

//                    float textAreaHeight = innerHeightPt * 0.4f;
//                    float textStartY = marginY + textAreaHeight - 10;
//                    float lineSpacing = textAreaHeight / 3.5f;
//                    float centerX = outerWidthPt / 2;

//                    cb.ShowTextAligned(Element.ALIGN_CENTER, reportId, centerX, textStartY, 0);
//                    cb.ShowTextAligned(Element.ALIGN_CENTER, reportCode, centerX, textStartY - lineSpacing, 0);
//                    cb.ShowTextAligned(Element.ALIGN_CENTER, reportName, centerX, textStartY - 2 * lineSpacing, 0);

//                    cb.EndText();
//                    doc.Close();
//                }

//                MessageBox.Show($"Barcode PDF saved successfully at:\n{filePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
//            }
//        }
//    }
//}








// এইটা পিডিএফ সেভ করার পরিবর্তে প্রিন্ট প্রিভিউ দেখাবে এবং প্রিন্টার থেকে সরাসরি প্রিন্ট করার অপশন দেবে।
// এইটা MSSQL থেকে ডাটা নিয়ে বারকোড প্রিন্ট করার জন্য।

//using iTextSharp.text.pdf;
//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Data.SqlClient;
//using System.Drawing;
//using System.Drawing.Printing;
//using System.Windows.Forms;

//namespace MultipleBarcode
//{
//    public partial class Form1 : Form
//    {
//        private string connectionString;

//        public Form1()
//        {
//            try
//            {
//                var cs = ConfigurationManager.ConnectionStrings["DefaultConnection"];
//                if (cs == null)
//                {
//                    MessageBox.Show("Connection string 'DefaultConnection' not found in App.config!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//                    return;
//                }

//                connectionString = cs.ConnectionString;

//                InitializeComponent();
//                InitializeGrid();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Error while loading Form1:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }

//        // Initialize DataGridView
//        private void InitializeGrid()
//        {
//            gvReports.AllowUserToAddRows = false;
//            gvReports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
//            gvReports.Columns.Clear();

//            gvReports.Columns.Add("ReportId", "Report ID");
//            gvReports.Columns.Add("ReportCode", "Report Code");
//            gvReports.Columns.Add("ReportName", "Report Name");
//            gvReports.Columns.Add("Barcode", "Barcode");

//            DataGridViewButtonColumn printButton = new DataGridViewButtonColumn
//            {
//                Name = "Print",
//                HeaderText = "Action",
//                Text = "Print",
//                UseColumnTextForButtonValue = true
//            };
//            gvReports.Columns.Add(printButton);
//        }

//        // Load data from database
//        private void btnSearch_Click(object sender, EventArgs e)
//        {
//            string reportId = txtReportId.Text.Trim();
//            gvReports.Rows.Clear();

//            using (SqlConnection con = new SqlConnection(connectionString))
//            {
//                string query = "SELECT ReportId, ReportCode, ReportName, Barcode FROM ReportsBarcodes WHERE (@ReportId = '' OR ReportId = @ReportId)";
//                SqlCommand cmd = new SqlCommand(query, con);
//                cmd.Parameters.AddWithValue("@ReportId", reportId);
//                con.Open();

//                SqlDataReader dr = cmd.ExecuteReader();
//                while (dr.Read())
//                {
//                    gvReports.Rows.Add(
//                        dr["ReportId"].ToString(),
//                        dr["ReportCode"].ToString(),
//                        dr["ReportName"].ToString(),
//                        dr["Barcode"].ToString()
//                    );
//                }
//            }

//            if (gvReports.Rows.Count == 0)
//                MessageBox.Show("No data found for the given Report ID.", "Info");
//        }

//        // Single Print Button
//        private void gvReports_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {
//            if (e.ColumnIndex == gvReports.Columns["Print"].Index && e.RowIndex >= 0)
//            {
//                string reportId = gvReports.Rows[e.RowIndex].Cells["ReportId"].Value.ToString();
//                string reportCode = gvReports.Rows[e.RowIndex].Cells["ReportCode"].Value.ToString();
//                string reportName = gvReports.Rows[e.RowIndex].Cells["ReportName"].Value.ToString();

//                var barcode = new BarcodeData
//                {
//                    ReportId = reportId,
//                    ReportCode = reportCode,
//                    ReportName = reportName
//                };

//                ShowBarcodePreview(new BarcodeData[] { barcode });
//            }
//        }

//        // Print All Button
//        private void btnPrintAll_Click(object sender, EventArgs e)
//        {
//            if (gvReports.Rows.Count == 0)
//            {
//                MessageBox.Show("No data to print.");
//                return;
//            }

//            var allBarcodes = new List<BarcodeData>();
//            foreach (DataGridViewRow row in gvReports.Rows)
//            {
//                if (row.IsNewRow) continue;
//                allBarcodes.Add(new BarcodeData
//                {
//                    ReportId = row.Cells["ReportId"].Value.ToString(),
//                    ReportCode = row.Cells["ReportCode"].Value.ToString(),
//                    ReportName = row.Cells["ReportName"].Value.ToString()
//                });
//            }

//            ShowBarcodePreview(allBarcodes.ToArray());
//        }

//        // Generate Bitmaps for barcodes
//        private List<Bitmap> GenerateBarcodeBitmaps(BarcodeData[] barcodes)
//        {
//            float dpi = 300f;
//            int width = (int)(1.5f * dpi);
//            int height = (int)(1.0f * dpi);
//            List<Bitmap> pages = new List<Bitmap>();

//            foreach (var b in barcodes)
//            {
//                Bitmap bmp = new Bitmap(width, height);
//                bmp.SetResolution(dpi, dpi);

//                using (Graphics g = Graphics.FromImage(bmp))
//                {
//                    g.Clear(Color.White);
//                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
//                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

//                    // Barcode
//                    Barcode128 code128 = new Barcode128
//                    {
//                        Code = b.ReportCode.Length > 12 ? b.ReportCode.Substring(0, 12) : b.ReportCode,
//                        BarHeight = height * 0.55f,
//                        X = 1f,
//                        StartStopText = false,
//                        Font = null
//                    };
//                    Image barcodeImg = code128.CreateDrawingImage(Color.Black, Color.White);

//                    float barcodeWidth = width * 0.85f;
//                    float barcodeHeight = height * 0.55f;
//                    float barcodeX = (width - barcodeWidth) / 2;
//                    float barcodeY = height * 0.05f;

//                    g.DrawImage(barcodeImg, barcodeX, barcodeY, barcodeWidth, barcodeHeight);

//                    // Text in bottom 40%
//                    float textAreaY = barcodeY + barcodeHeight;
//                    float textAreaHeight = height * 0.40f;

//                    using (Font font = new Font("Arial", 8, FontStyle.Bold))
//                    using (StringFormat sf = new StringFormat
//                    {
//                        Alignment = StringAlignment.Center,
//                        LineAlignment = StringAlignment.Near
//                    })
//                    {
//                        float lineHeight = textAreaHeight / 3f;
//                        g.DrawString(b.ReportId, font, Brushes.Black, new RectangleF(0, textAreaY, width, lineHeight), sf);
//                        g.DrawString(b.ReportCode, font, Brushes.Black, new RectangleF(0, textAreaY + lineHeight, width, lineHeight), sf);
//                        g.DrawString(b.ReportName, font, Brushes.Black, new RectangleF(0, textAreaY + 2 * lineHeight, width, lineHeight), sf);
//                    }
//                }

//                pages.Add(bmp);
//            }

//            return pages;
//        }

//        // Show Print Preview
//        private void ShowBarcodePreview(BarcodeData[] barcodes)
//        {
//            var pages = GenerateBarcodeBitmaps(barcodes);
//            if (pages.Count == 0) return;

//            PrintDocument printDoc = new PrintDocument();
//            int pageIndex = 0;

//            PaperSize customSize = new PaperSize("Label_1.5x1", (int)(1.5 * 100), (int)(1.0 * 100));
//            printDoc.DefaultPageSettings.PaperSize = customSize;
//            printDoc.DefaultPageSettings.Landscape = false;

//            printDoc.PrintPage += (s, e) =>
//            {
//                if (pageIndex < pages.Count)
//                {
//                    Bitmap bmp = pages[pageIndex];

//                    float scaleX = e.PageBounds.Width / (float)bmp.Width;
//                    float scaleY = e.PageBounds.Height / (float)bmp.Height;
//                    float scale = Math.Min(scaleX, scaleY);

//                    float imgWidth = bmp.Width * scale;
//                    float imgHeight = bmp.Height * scale;

//                    float offsetX = (e.PageBounds.Width - imgWidth) / 2;
//                    float offsetY = (e.PageBounds.Height - imgHeight) / 2;

//                    e.Graphics.DrawImage(bmp, offsetX, offsetY, imgWidth, imgHeight);

//                    pageIndex++;
//                    e.HasMorePages = pageIndex < pages.Count;
//                }
//            };

//            // ✅ Print Preview
//            PrintPreviewDialog preview = new PrintPreviewDialog
//            {
//                Document = printDoc,
//                WindowState = FormWindowState.Maximized
//            };
//            preview.ShowDialog();

//            // Optional: From preview, user can press Print button to print via default printer
//        }

//        public class BarcodeData
//        {
//            public string ReportId { get; set; }
//            public string ReportCode { get; set; }
//            public string ReportName { get; set; }
//        }
//    }
//}





// এইটা পিডিএফ সেভ করার পরিবর্তে প্রিন্ট প্রিভিউ দেখাবে এবং প্রিন্টার থেকে সরাসরি প্রিন্ট করার অপশন দেবে।
// এইটা Oeacle Database থেকে ডাটা নিয়ে বারকোড প্রিন্ট করার জন্য।

using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;  // ✅ Oracle Driver

namespace MultipleBarcode
{
    public partial class Form1 : Form
    {
        private string connectionString;

        public Form1()
        {
            try
            {
                var cs = ConfigurationManager.ConnectionStrings["DefaultConnection"];
                if (cs == null)
                {
                    MessageBox.Show("Connection string 'DefaultConnection' not found in App.config!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                connectionString = cs.ConnectionString;

                InitializeComponent();
                InitializeGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while loading Form1:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Initialize DataGridView
        private void InitializeGrid()
        {
            gvReports.AllowUserToAddRows = false;
            gvReports.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gvReports.Columns.Clear();

            gvReports.Columns.Add("ReportId", "Report ID");
            gvReports.Columns.Add("ReportCode", "Report Code");
            gvReports.Columns.Add("ReportName", "Report Name");
            gvReports.Columns.Add("Barcode", "Barcode");

            DataGridViewButtonColumn printButton = new DataGridViewButtonColumn
            {
                Name = "Print",
                HeaderText = "Action",
                Text = "Print",
                UseColumnTextForButtonValue = true
            };
            gvReports.Columns.Add(printButton);
        }

        // Load data from Oracle Database
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string reportId = txtReportId.Text.Trim();
            gvReports.Rows.Clear();

            //using (var con = new OracleConnection(connectionString))
            //{
            //    con.Open();
            //    MessageBox.Show("✅ Connected Successfully!");
            //}


            try
            {
                using (OracleConnection con = new OracleConnection(connectionString))
                {
                    // ✅ Oracle parameter syntax uses colon (:)
                    string query = @"
                        SELECT REPORTID, REPORTCODE, REPORTNAME, BARCODE 
                FROM MULTIPLEBARCODEDB.REPORTSBARCODES
                WHERE (:ReportId IS NULL OR REPORTID = :ReportId)";

                    using (OracleCommand cmd = new OracleCommand(query, con))
                    {
                        // যদি ফাঁকা থাকে, তাহলে NULL পাঠানো হবে
                        cmd.Parameters.Add(new OracleParameter("ReportId",
                            string.IsNullOrEmpty(reportId) ? (object)DBNull.Value : reportId));

                        con.Open();
                        using (OracleDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                gvReports.Rows.Add(
                                    dr["REPORTID"].ToString(),
                                    dr["REPORTCODE"].ToString(),
                                    dr["REPORTNAME"].ToString(),
                                    dr["BARCODE"].ToString()
                                );
                            }
                        }
                    }
                }

                if (gvReports.Rows.Count == 0)
                    MessageBox.Show("No data found for the given Report ID.", "Info");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while fetching data from Oracle:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Single Print Button
        private void gvReports_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == gvReports.Columns["Print"].Index && e.RowIndex >= 0)
            {
                string reportId = gvReports.Rows[e.RowIndex].Cells["ReportId"].Value.ToString();
                string reportCode = gvReports.Rows[e.RowIndex].Cells["ReportCode"].Value.ToString();
                string reportName = gvReports.Rows[e.RowIndex].Cells["ReportName"].Value.ToString();

                var barcode = new BarcodeData
                {
                    ReportId = reportId,
                    ReportCode = reportCode,
                    ReportName = reportName
                };

                ShowBarcodePreview(new BarcodeData[] { barcode });
            }
        }

        // Print All Button
        private void btnPrintAll_Click(object sender, EventArgs e)
        {
            if (gvReports.Rows.Count == 0)
            {
                MessageBox.Show("No data to print.");
                return;
            }

            var allBarcodes = new List<BarcodeData>();
            foreach (DataGridViewRow row in gvReports.Rows)
            {
                if (row.IsNewRow) continue;
                allBarcodes.Add(new BarcodeData
                {
                    ReportId = row.Cells["ReportId"].Value.ToString(),
                    ReportCode = row.Cells["ReportCode"].Value.ToString(),
                    ReportName = row.Cells["ReportName"].Value.ToString()
                });
            }

            ShowBarcodePreview(allBarcodes.ToArray());
        }

        // Generate Bitmaps for barcodes
        private List<Bitmap> GenerateBarcodeBitmaps(BarcodeData[] barcodes)
        {
            float dpi = 300f;
            int width = (int)(1.5f * dpi);
            int height = (int)(1.0f * dpi);
            List<Bitmap> pages = new List<Bitmap>();

            foreach (var b in barcodes)
            {
                Bitmap bmp = new Bitmap(width, height);
                bmp.SetResolution(dpi, dpi);

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.Clear(Color.White);
                    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                    // Barcode
                    Barcode128 code128 = new Barcode128
                    {
                        Code = b.ReportCode.Length > 12 ? b.ReportCode.Substring(0, 12) : b.ReportCode,
                        BarHeight = height * 0.55f,
                        X = 1f,
                        StartStopText = false,
                        Font = null
                    };
                    Image barcodeImg = code128.CreateDrawingImage(Color.Black, Color.White);

                    float barcodeWidth = width * 0.85f;
                    float barcodeHeight = height * 0.55f;
                    float barcodeX = (width - barcodeWidth) / 2;
                    float barcodeY = height * 0.05f;

                    g.DrawImage(barcodeImg, barcodeX, barcodeY, barcodeWidth, barcodeHeight);

                    // Text in bottom 40%
                    float textAreaY = barcodeY + barcodeHeight;
                    float textAreaHeight = height * 0.40f;

                    using (Font font = new Font("Arial", 8, FontStyle.Bold))
                    using (StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Near
                    })
                    {
                        float lineHeight = textAreaHeight / 3f;
                        g.DrawString(b.ReportId, font, Brushes.Black, new RectangleF(0, textAreaY, width, lineHeight), sf);
                        g.DrawString(b.ReportCode, font, Brushes.Black, new RectangleF(0, textAreaY + lineHeight, width, lineHeight), sf);
                        g.DrawString(b.ReportName, font, Brushes.Black, new RectangleF(0, textAreaY + 2 * lineHeight, width, lineHeight), sf);
                    }
                }

                pages.Add(bmp);
            }

            return pages;
        }

        // Show Print Preview
        private void ShowBarcodePreview(BarcodeData[] barcodes)
        {
            var pages = GenerateBarcodeBitmaps(barcodes);
            if (pages.Count == 0) return;

            PrintDocument printDoc = new PrintDocument();
            int pageIndex = 0;

            PaperSize customSize = new PaperSize("Label_1.5x1", (int)(1.5 * 100), (int)(1.0 * 100));
            printDoc.DefaultPageSettings.PaperSize = customSize;
            printDoc.DefaultPageSettings.Landscape = false;

            printDoc.PrintPage += (s, e) =>
            {
                if (pageIndex < pages.Count)
                {
                    Bitmap bmp = pages[pageIndex];

                    float scaleX = e.PageBounds.Width / (float)bmp.Width;
                    float scaleY = e.PageBounds.Height / (float)bmp.Height;
                    float scale = Math.Min(scaleX, scaleY);

                    float imgWidth = bmp.Width * scale;
                    float imgHeight = bmp.Height * scale;

                    float offsetX = (e.PageBounds.Width - imgWidth) / 2;
                    float offsetY = (e.PageBounds.Height - imgHeight) / 2;

                    e.Graphics.DrawImage(bmp, offsetX, offsetY, imgWidth, imgHeight);

                    pageIndex++;
                    e.HasMorePages = pageIndex < pages.Count;
                }
            };

            // ✅ Print Preview
            PrintPreviewDialog preview = new PrintPreviewDialog
            {
                Document = printDoc,
                WindowState = FormWindowState.Maximized
            };
            preview.ShowDialog();
        }

        // Simple class to store barcode info
        public class BarcodeData
        {
            public string ReportId { get; set; }
            public string ReportCode { get; set; }
            public string ReportName { get; set; }
        }
    }
}

