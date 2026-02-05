using Application.Extensions;
using Application.Helpers;
using Application.ServiceInterfaces;
using Application.ViewModels;
using AspNetCoreHero.ToastNotification.Abstractions;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Pages.Admin.MessageCenter
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class IndexModel : PageModel
    {
        private readonly IHttpClientService _httpClient;
        private readonly INotyfService _notyf;
        public IndexModel(
            IHttpClientService httpClient,
            INotyfService notyf)
        {
            _httpClient = httpClient;
            _notyf = notyf;
        }
        public List<string> AllowedExtensions => new List<string> { ".xls", ".xlsx" };
        [BindProperty] public SmsVM SmsVm { get; set; }
        public async Task<IActionResult> OnGetSampleFile()
        {
            var FileName = "MobileNos.xlsx";
            using XLWorkbook workbook = new XLWorkbook();
            //Add sheet to workbook
            var sheet1 = workbook.Worksheets.Add("MobileNos");
            //Create list of Headers
            var Headers1 = new List<string>
            {
                "Mobile Number*"
            };
            //Add Headers to the sheet
            OfficeHelper.SetHeaders(sheet1, Headers1);
            //Set Editable Range
            var firstCellRow = 2;
            var firstCellColumn = 1;
            var lastCellRow = 100;
            var lastCellColumn = Headers1.Count;

            var sheet1EditableRange = sheet1.Range(firstCellRow, firstCellColumn, lastCellRow, lastCellColumn);
            sheet1EditableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            sheet1EditableRange.Style.Border.OutsideBorderColor = XLColor.Black;
            sheet1EditableRange.Style.Protection.Locked = false;
            sheet1.Protection.Protect("Sheet1#123");
            sheet1.Cell(firstCellRow, firstCellColumn).SetActive();

            //Specify column cell validations styles

            var columnNumber = firstCellColumn;
            sheet1.Column(columnNumber).Width = 15.0;
            var MobileNoRange = sheet1.Range(firstCellRow, columnNumber, lastCellRow, columnNumber);
            //MobileNoRange.DataType = XLDataType.Number;
            MobileNoRange.Style.NumberFormat.Format = "0000000000";

            var MobileNoDV = MobileNoRange.CreateDataValidation();
            MobileNoDV.TextLength.EqualTo(10);
            MobileNoDV.IgnoreBlanks = false;
            MobileNoDV.ErrorStyle = XLErrorStyle.Stop;
            MobileNoDV.ErrorTitle = "Mobile Number";
            MobileNoDV.ErrorMessage = "Please enter a valid 10 digit mobile number";
            MobileNoDV.ShowErrorMessage = true;

            workbook.LockStructure = true;
            workbook.Protect("Book1#123");
            //return the file for download
            await using var memoryStream = new MemoryStream();
            workbook.SaveAs(memoryStream);
            return File(memoryStream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", FileName);
        }
        public IActionResult OnPostImport(IFormFile UploadedFile)
        {
            if (UploadedFile?.Length > 0)
            {
                var FileExtension = Path.GetExtension(UploadedFile.FileName).ToLower();
                if (!AllowedExtensions.Contains(FileExtension) || !UploadedFile.FileName.Contains("MobileNos"))
                {
                    _notyf.Error("Only recomended excel file please");
                    return RedirectToPage();
                }

                using XLWorkbook workBook = new XLWorkbook(UploadedFile.OpenReadStream());
                //Read the first Sheet from Excel file.
                var workSheet = workBook.Worksheet(1);
                if (workSheet.Name != "MobileNos")
                {
                    _notyf.Error("Only recomended excel file please");
                    return RedirectToPage();
                }
                var rows = workSheet.Rows(2, workSheet.LastRowUsed().RowNumber());
                if (rows.Any())
                {
                    SmsVm = new SmsVM { MobileNos = new List<string>(rows.Count()) };
                    foreach (var row in rows)
                    {
                        if (row == null) continue;

                        if (!row.Cell(1).IsEmpty() && row.Cell(1).Value.ToString().Length == 10)
                            SmsVm.MobileNos.Add(row.Cell(1).Value.ToString());
                    }
                }
                else
                {
                    _notyf.Error("No data or valid data found in the sheet");
                    return RedirectToPage();
                }
            }
            else
            {
                _notyf.Information("File not selected or invalid.");
            }
            return Page();
        }
        public async Task<IActionResult> OnPostSave()
        {
            if (!ModelState.IsValid) { _notyf.Error(ModelState.GetErrorMessageString()); return Page(); }
            SmsVm.MobileNos.RemoveAll(x => string.IsNullOrEmpty(x) || x.Length < 10);
            var SmsResult = await _httpClient.PostAsync("SendSmsNotifications/SendSms", true, SmsVm);
            if (SmsResult == "unauthorized")
            {
                _notyf.Information("Please login");
                return RedirectToPage("/Account/Login");
            }
            //var SmsResponseVm = !string.IsNullOrEmpty(SmsResult) ? JsonConvert.DeserializeObject<SmsResponseVM>(SmsResult) : null;
            if (!string.IsNullOrEmpty(SmsResult))
                _notyf.Success($"{SmsResult}");
            else
                _notyf.Error("Sms(s) sending failed");
            return Page();
        }
    }
}