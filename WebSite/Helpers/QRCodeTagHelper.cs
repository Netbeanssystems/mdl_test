using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing.QrCode;
namespace WebSite.Helpers
{
    [HtmlTargetElement("qrcode")]
    public class QRCodeTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var content = context.AllAttributes["content"].Value.ToString();
            var alt = context.AllAttributes["alt"].Value.ToString();
            var width = Convert.ToInt32(context.AllAttributes["width"] == null ? "130" : context.AllAttributes["width"].Value.ToString());
            var height = Convert.ToInt32(context.AllAttributes["height"] == null ? "130" : context.AllAttributes["height"].Value.ToString());
            var margin = Convert.ToInt32(context.AllAttributes["margin"] == null ? "0" : context.AllAttributes["margin"].Value.ToString());
            var qrWriter = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions { Height = height, Width = width, Margin = margin }
            };
            var pixelData = qrWriter.Write(content);
            using var bitmap = new Bitmap(pixelData.Width, pixelData.Height, PixelFormat.Format32bppRgb);
            using var ms = new MemoryStream();
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, pixelData.Width, pixelData.Height),
                ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb);
            try
            {
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
            bitmap.Save(ms, ImageFormat.Png);
            output.TagName = "img";
            output.Attributes.Clear();
            output.Attributes.Add("width", width);
            output.Attributes.Add("height", height);
            output.Attributes.Add("alt", alt);
            output.Attributes.Add("src", $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}");
        }
    }
}
