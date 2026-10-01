using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing.QrCode;

namespace Application.Helpers
{
    public static class QRCodeGenerator
    {
        public static byte[] GenerateQR(string content, int Height = 100, int Width = 100, int Margin = 0)
        {
            var qrWriter = new ZXing.BarcodeWriterPixelData
            {
                Format = ZXing.BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions { Height = Height, Width = Width, Margin = Margin }
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
            var bitmapBytes = ms.ToArray();
            return bitmapBytes;
        }
    }
}
