using Rotativa.AspNetCore;
using System;

namespace WebApp.Helpers
{
    public static class PdfHelper<T>
    {
        /// <summary>
        /// Generates an A4 size pdf file
        /// </summary>
        /// <param name="ViewModel">View model for the view</param>
        /// <param name="ViewName">Name of the View</param>
        /// <param name="FileName">Name of the pdf file defaults to null</param>
        /// <param name="Orientation">Orientation of the pdf file defaults to null</param>
        /// <param name="AddFooter">Flag to specify if you want to show date and page numbers defaults to false</param>
        /// <returns>A4 size pdf file from the given parameters</returns>
        public static ViewAsPdf GeneratePdf(T ViewModel, string ViewName, string FileName = null, string Orientation = null, bool AddFooter = false)
        {
            var pdf = new ViewAsPdf(ViewName, ViewModel)
            {
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };

            if (AddFooter)
                pdf.CustomSwitches += "--print-media-type --footer-right \"  Generated on: " +
                                     DateTime.UtcNow.AddHours(5.5).Date.ToString("dd-MM-yyyy") +
                                     "  Page: [page]/[toPage]\"" +
                                     " --footer-line --footer-font-size \"10\" --footer-spacing 1 --footer-font-name \"Segoe UI\"";

            if (!string.IsNullOrEmpty(FileName))
                pdf.FileName = $"{FileName}.pdf";

            pdf.PageOrientation = !string.IsNullOrEmpty(Orientation) && Orientation == "Landscape"
                ? Rotativa.AspNetCore.Options.Orientation.Landscape
                : Rotativa.AspNetCore.Options.Orientation.Portrait;

            return pdf;
        }
    }
}