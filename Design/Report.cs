using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Design
{
    using System.Diagnostics;
    class Report
    {
        public void CreatePdf(string filePath, string text)
        {
            // Create a new document
            iTextSharp.text.Document document = new iTextSharp.text.Document();

            try
            {
                // Create a writer to generate the PDF file
                PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));

                // Open the document
                document.Open();

                // Add a new paragraph with the specified text
                document.Add(new iTextSharp.text.Paragraph(text));
            }
            catch (Exception ex)
            {
                Debug.WriteLine("An error occurred while creating the PDF file: " + ex.Message);
            }
            finally
            {
                // Close the document
                document.Close();
            }
        }
    }
}
