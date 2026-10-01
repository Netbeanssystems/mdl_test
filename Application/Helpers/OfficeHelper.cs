using ClosedXML.Excel;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Spire.Doc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Application.Helpers
{
    public static class OfficeHelper
    {
        private static string[] DateTimeFormats = {
            "dd-MM-yyyy",
            "dd-MM-yyyy hh:mm:ss tt",
            "dd-MM-yyyy hh:mm:ss",
            "dd-MMM-yy hh:mm:ss",
            "dd-MMM-yyyy hh:mm:ss",
            "dd-MMM-yy h:mm tt",
            "dd-MMM-yyyy h:mm tt",
            "MM-dd-yyyy",
            "M-d-yyyy",
            "M-dd-yyyy",
            "MM-d-yyyy",
            "M-d-yyyy h:mm:ss tt",
            "MM-d-yyyy h:mm:ss tt",
            "M-d-yyyy h:mm tt",
            "MM-dd-yyyy hh:mm:ss",
            "M-d-yyyy h:mm:ss",
            "M-d-yyyy hh:mm tt",
            "M-d-yyyy hh tt",
            "M-d-yyyy h:mm",
            "M-d-yyyy h:mm",
            "MM-dd-yyyy hh:mm",
            "M-dd-yyyy hh:mm",
            "yyyy-MM-dd",
            "yyyy-MM-ddTHH:mm",
            "yyyy-MM-dd hh:mm tt",
            "yyyy-MM-dd hh:mm:ss tt",
            "MM/dd/yyyy",
            "M/d/yyyy",
            "M/dd/yyyy",
            "MM/d/yyyy",
            "M/d/yyyy h:mm:ss tt",
            "MM/d/yyyy h:mm:ss tt",
            "M/d/yyyy h:mm tt",
            "MM/dd/yyyy hh:mm:ss",
            "M/d/yyyy h:mm:ss",
            "M/d/yyyy hh:mm tt",
            "M/d/yyyy hh tt",
            "M/d/yyyy h:mm",
            "M/d/yyyy h:mm",
            "MM/dd/yyyy hh:mm",
            "M/dd/yyyy hh:mm",
            "d/M/yyyy hh:mm:ss tt",
            "d/MM/yyyy hh:mm:ss tt",
            "dd/MM/yyyy hh:mm:ss tt",
            "dd/MM/yyyy hh:mm:ss",
            "dd/MMM/yy hh:mm:ss",
            "dd/MMM/yyyy hh:mm:ss",
            "dd/MMM/yy h:mm tt",
            "dd/MMM/yyyy hh:mm tt",
            "yyyy/MM/ddTHH:mm"
        };
        /// <summary>
        /// Replaces text and images in the word (.docx) file 
        /// </summary>
        /// <param name="TextDictionary">Dictionary containing key value pairs of data to be replaced</param>
        /// <param name="relativepath">Physical path of the containing folder</param>
        /// <param name="SourceName">Name of the source template word (.docx) file name</param>
        /// <param name="DestinationName">Name of the destination word (.docx) file</param>
        /// 
        // datnet 6 --> dotnet 8 migration issue with using statement
        //public static void ReplaceTextAndImages(Dictionary<string, string> TextDictionary, string relativepath, string SourceName, string DestinationName)
        //{
        //    var sourceFullPath = System.IO.Path.Combine(relativepath, SourceName);
        //    var destinationFullPath = System.IO.Path.Combine(relativepath, $"{DestinationName}.docx");

        //    // Create a copy of the template file and open the copy
        //    File.Copy(sourceFullPath, destinationFullPath, true);

        //    using WordprocessingDocument doc = WordprocessingDocument.Open(destinationFullPath, true);

        //    //Replace text elements
        //    ReplaceTextElement(doc, TextDictionary);
        //    //Replace image placeholder with QR Code
        //    if (TextDictionary.ContainsKey("RWQRCode"))
        //        ReplaceInternalImage(doc, "RWQRCode.jpg", Convert.FromBase64String(TextDictionary["RWQRCode"]));
        //    doc.Close();
        //}

        public static void ReplaceTextAndImages(
    Dictionary<string, string> TextDictionary,
    string relativepath,
    string SourceName,
    string DestinationName)
        {
            var sourceFullPath = System.IO.Path.Combine(relativepath, SourceName);
            var destinationFullPath = System.IO.Path.Combine(relativepath, $"{DestinationName}.docx");

            File.Copy(sourceFullPath, destinationFullPath, true);

            using (WordprocessingDocument doc = WordprocessingDocument.Open(destinationFullPath, true))
            {
                ReplaceTextElement(doc, TextDictionary);

                if (TextDictionary.ContainsKey("RWQRCode"))
                {
                    ReplaceInternalImage(
                        doc,
                        "RWQRCode.jpg",
                        Convert.FromBase64String(TextDictionary["RWQRCode"])
                    );
                }
            } // ✅ Automatically closed here
        }


        /// <summary>
        /// Replaces text elements in OpenXML word documnet
        /// </summary>
        /// <param name="doc">OpenXML word documnet</param>
        /// <param name="TextDictionary">Dictionary containing key value pairs of data to be replaced</param>
        private static void ReplaceTextElement(WordprocessingDocument doc, Dictionary<string, string> TextDictionary)
        {
            var body = doc.MainDocumentPart?.Document.Body;
            var paras = body?.Elements<DocumentFormat.OpenXml.Wordprocessing.Paragraph>();
            if (paras != null)
                foreach (var para in paras)
                {
                    foreach (var run in para.Elements<DocumentFormat.OpenXml.Wordprocessing.Run>())
                    {
                        foreach (var text in run.Elements<DocumentFormat.OpenXml.Wordprocessing.Text>())
                        {
                            foreach (var item in TextDictionary)
                            {
                                if (text.Text == item.Key)
                                    text.Text = text.Text.Replace(item.Key, item.Value);
                            }
                        }
                    }
                }
        }

        /// <summary>
        /// Replaces the image in a document with the new file bytes, or removes the image if the newImageBytes parameter is null.
        /// Relies on a the image having had it's name set via the 'Selection Pane' in Word
        /// </summary>
        /// <param name="document">The OpenXML document</param>
        /// <param name="oldImagesPlaceholderText">The placeholder name for the image set via Selection in Word</param>
        /// <param name="newImageBytes">The new file. Pass null to remove the selected image from the document instead</param>
        private static void ReplaceInternalImage(WordprocessingDocument document, string oldImagesPlaceholderText, byte[] newImageBytes)
        {
            IEnumerable<Drawing> drawings = document.MainDocumentPart?.Document.Descendants<Drawing>().ToList();
            if (drawings != null)
                foreach (Drawing drawing in drawings)
                {
                    DocProperties dpr = drawing.Descendants<DocProperties>().FirstOrDefault();
                    if (dpr != null && dpr.Name == oldImagesPlaceholderText)
                    {
                        foreach (Blip b in drawing.Descendants<Blip>().ToList())
                        {
                            OpenXmlPart imagePart = document.MainDocumentPart.GetPartById(b.Embed);
                            using var writer = new BinaryWriter(imagePart.GetStream());
                            writer.Write(newImageBytes);
                        }
                    }
                }
        }

        /// <summary>
        /// Converts word (.docx) file on disk to pdf file
        /// </summary>
        /// <param name="relativepath">Physical path of the containing folder</param>
        /// <param name="DestinationName">File name without extension</param>
        /// <returns>Full physical path of the pdf file</returns>
        public static string ConvertWordToPdf(string relativepath, string DestinationName)
        {
            var WordFullPath = System.IO.Path.Combine(relativepath, $"{DestinationName}.docx");
            var PdfFullPath = System.IO.Path.Combine(relativepath, $"{DestinationName}.pdf");
            Spire.Doc.Document document = new Spire.Doc.Document();
            document.LoadFromFile(WordFullPath);
            document.SaveToFile(PdfFullPath, FileFormat.PDF);
            document.Close();
            return PdfFullPath;
        }

        /// <summary>
        /// Sets headers of the excel file for import
        /// </summary>
        /// <param name="sheet">ClosedXML worksheet</param>
        /// <param name="Headers">List of headers</param>
        public static void SetHeaders(IXLWorksheet sheet, List<string> Headers)
        {
            for (var i = 0; i < Headers.Count; i++)
            {
                sheet.Cell(1, i + 1).Value = Headers[i];
                sheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromTheme(XLThemeColor.Accent1);
                sheet.Cell(1, i + 1).Style.Font.FontColor = XLColor.FromTheme(XLThemeColor.Background1);
                sheet.Cell(1, i + 1).Style.Font.Bold = true;
            }
        }
        /// <summary>
        /// Find if given item is between start and end 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="includeStart"></param>
        /// <param name="includeEnd"></param>
        /// <returns></returns>
        public static bool IsBetween<T>(this T item, T start, T end, bool includeStart = false, bool includeEnd = false)
        {
            if (includeEnd && includeStart)
                return Comparer<T>.Default.Compare(item, start) >= 0
                       && Comparer<T>.Default.Compare(item, end) <= 0;
            if (includeStart)
                return Comparer<T>.Default.Compare(item, start) >= 0
                    && Comparer<T>.Default.Compare(item, end) < 0;
            if (includeEnd)
                return Comparer<T>.Default.Compare(item, start) > 0
                       && Comparer<T>.Default.Compare(item, end) <= 0;
            return Comparer<T>.Default.Compare(item, start) > 0
                   && Comparer<T>.Default.Compare(item, end) < 0;
        }
        /// <summary>
        /// Gets Date from String
        /// </summary>
        /// <param name="date">date as string</param>
        /// <returns>date as DateTime</returns>
        public static DateTime GetDateFromString(string date)
        {
            return DateTime.ParseExact(date, DateTimeFormats, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Gets Time from String
        /// </summary>
        /// <param name="time">time as string</param>
        /// <returns>time as DateTime</returns>
        public static DateTime GetTimeFromString(string time)
        {
            return DateTime.ParseExact(time, new[] { "h:mm tt", "hh:mm tt", "HH:mm", "H:mm" }, CultureInfo.InvariantCulture);
        }
        /// <summary>
        /// Gets DateTime from date and time strings
        /// </summary>
        /// <param name="date">date as string</param>
        /// <param name="time">time as string</param>
        /// <returns>Date with time</returns>
        public static DateTime GetDateTimeFromString(string date, string time)
        {
            var Date = DateTime.ParseExact(date, DateTimeFormats, CultureInfo.InvariantCulture);
            if (string.IsNullOrEmpty(time)) return Date;
            var Time = DateTime.ParseExact(time, new[] { "h:mm tt", "hh:mm tt", "HH:mm", "H:mm" }, CultureInfo.InvariantCulture);
            Date = Date.Add(Time.TimeOfDay);
            return Date;
        }
        /// <summary>
        /// Gets DateTime from DateTime date and time string
        /// </summary>
        /// <param name="date">date as DateTime</param>
        /// <param name="time">time as string</param>
        /// <returns>Date with time</returns>
        public static DateTime GetDateTimeFromString(DateTime? date, string time)
        {
            var Date = (DateTime)date;
            if (string.IsNullOrEmpty(time)) return Date;
            var Time = DateTime.ParseExact(time, new[] { "h:mm tt", "hh:mm tt", "HH:mm", "H:mm" }, CultureInfo.InvariantCulture);
            Date = Date.Add(Time.TimeOfDay);
            return Date;
        }

        public static string GetRandFromDate(DateTime dateTime)
        {
            var rndNo = new Random().Next(0, 1000);
            var dateTimeTicks = dateTime.TimeOfDay.Ticks;
            var orderId = $"{dateTimeTicks}{rndNo}";
            return orderId;
        }
    }
}