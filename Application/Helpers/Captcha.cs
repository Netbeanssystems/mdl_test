using Microsoft.AspNetCore.Http;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace Application.Helpers
{
    public static class Captcha
    {
        const string Letters = "2346789ABCDEFGHIJKLMNOPQRSTUVWXYZ$#@&";

        public static string GenerateCaptchaCode()
        {
            var rand = new Random();
            var maxRand = Letters.Length - 1;
            var sb = new StringBuilder();

            for (var i = 0; i < 6; i++)
            {
                var index = rand.Next(maxRand);
                sb.Append(Letters[index]);
            }

            return sb.ToString();
        }

        public static bool ValidateCaptchaCode(string userInputCaptcha, HttpContext context)
        {
            var isValid = userInputCaptcha == context.Session.GetString("CaptchaCode");
            context.Session.Remove("CaptchaCode");
            return isValid;
        }

        public static CaptchaResult GenerateCaptchaImage(int width, int height, string captchaCode)
        {
            using var baseMap = new Bitmap(width, height);
            using var graph = Graphics.FromImage(baseMap);
            var rand = new Random();

            //graph.Clear(GetRandomLightColor());
            graph.Clear(Color.LightGray);

            DrawCaptchaCode();
            DrawDisorderLine();
            AdjustRippleEffect();

            var ms = new MemoryStream();

            baseMap.Save(ms, ImageFormat.Png);

            return new CaptchaResult { CaptchaCode = captchaCode, CaptchaByteData = ms.ToArray(), Timestamp = DateTime.Now };

            int GetFontSize(int imageWidth, int captchCodeCount)
            {
                var averageSize = imageWidth / captchCodeCount;

                return Convert.ToInt32(averageSize);
            }

            Color GetRandomDeepColor()
            {
                int redlow = 160, greenLow = 100, blueLow = 160;
                return Color.FromArgb(rand.Next(redlow), rand.Next(greenLow), rand.Next(blueLow));
            }

            //Color GetRandomLightColor()
            //{
            //    int low = 180, high = 255;
            //    var nRend = rand.Next(high) % (high - low) + low;
            //    var nGreen = rand.Next(high) % (high - low) + low;
            //    var nBlue = rand.Next(high) % (high - low) + low;
            //    return Color.FromArgb(nRend, nGreen, nBlue);
            //}

            void DrawCaptchaCode()
            {
                var fontBrush = new SolidBrush(Color.Black);
                var fontSize = GetFontSize(width, captchaCode.Length);
                var font = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
                for (var i = 0; i < captchaCode.Length; i++)
                {
                    //fontBrush.Color = GetRandomDeepColor();
                    fontBrush.Color = Color.Black;

                    var shiftPx = fontSize / 6;

                    float x = i * fontSize + rand.Next(-shiftPx, shiftPx) + rand.Next(-shiftPx, shiftPx);
                    var maxY = height - fontSize;
                    if (maxY < 0) maxY = 0;
                    float y = rand.Next(0, maxY);

                    graph.DrawString(captchaCode[i].ToString(), font, fontBrush, x, y);
                }
            }

            void DrawDisorderLine()
            {
                var linePen = new Pen(new SolidBrush(Color.Black), 1);
                for (var i = 0; i < rand.Next(3, 5); i++)
                {
                    linePen.Color = GetRandomDeepColor();

                    var startPoint = new Point(rand.Next(0, width), rand.Next(0, height));
                    var endPoint = new Point(rand.Next(0, width), rand.Next(0, height));
                    graph.DrawLine(linePen, startPoint, endPoint);

                    //Point bezierPoint1 = new Point(rand.Next(0, width), rand.Next(0, height));
                    //Point bezierPoint2 = new Point(rand.Next(0, width), rand.Next(0, height));
                    //graph.DrawBezier(linePen, startPoint, bezierPoint1, bezierPoint2, endPoint);
                }
            }

            void AdjustRippleEffect()
            {
                short nWave = 6;
                var nWidth = baseMap.Width;
                var nHeight = baseMap.Height;

                var pt = new Point[nWidth, nHeight];

                for (var x = 0; x < nWidth; ++x)
                {
                    for (var y = 0; y < nHeight; ++y)
                    {
                        var xo = nWave * Math.Sin(2.0 * 3.1415 * y / 128.0);
                        var yo = nWave * Math.Cos(2.0 * 3.1415 * x / 128.0);

                        var newX = x + xo;
                        var newY = y + yo;

                        if (newX > 0 && newX < nWidth)
                        {
                            pt[x, y].X = (int)newX;
                        }
                        else
                        {
                            pt[x, y].X = 0;
                        }


                        if (newY > 0 && newY < nHeight)
                        {
                            pt[x, y].Y = (int)newY;
                        }
                        else
                        {
                            pt[x, y].Y = 0;
                        }
                    }
                }

                var bSrc = (Bitmap)baseMap.Clone();

                var bitmapData = baseMap.LockBits(new Rectangle(0, 0, baseMap.Width, baseMap.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
                var bmSrc = bSrc.LockBits(new Rectangle(0, 0, bSrc.Width, bSrc.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

                //var scanline = bitmapData.Stride;
                //var scan0 = bitmapData.Scan0;
                //var srcScan0 = bmSrc.Scan0;
                //unsafe
                //{
                //    byte* p = (byte*)(void*)scan0;
                //    byte* pSrc = (byte*)(void*)srcScan0;
                //    int nOffset = bitmapData.Stride - baseMap.Width * 3;
                //    for (int y = 0; y < nHeight; ++y)
                //    {
                //        for (int x = 0; x < nWidth; ++x)
                //        {
                //            var xOffset = pt[x, y].X;
                //            var yOffset = pt[x, y].Y;
                //            if (yOffset >= 0 && yOffset < nHeight && xOffset >= 0 && xOffset < nWidth)
                //            {
                //                if (pSrc != null)
                //                {
                //                    p[0] = pSrc[yOffset * scanline + xOffset * 3];
                //                    p[1] = pSrc[yOffset * scanline + xOffset * 3 + 1];
                //                    p[2] = pSrc[yOffset * scanline + xOffset * 3 + 2];
                //                }
                //            }
                //            p += 3;
                //        }
                //        p += nOffset;
                //    }
                //}
                baseMap.UnlockBits(bitmapData);
                bSrc.UnlockBits(bmSrc);
                bSrc.Dispose();
            }
        }
    }
    public static class MathEquation
    {
        private static bool isValid;

        public static bool ValidateMathEquationForms(int userInputAnswer, HttpContext context)
        {

            try
            {
                var equation = context.Session.GetString("MathEquForms");
                // Use DataTable.Compute to evaluate the expression
                DataTable table = new DataTable();
                var result = table.Compute(equation, "");
                int actualAnswer = Convert.ToInt32(result);

                // Validate the answer
                isValid = userInputAnswer == actualAnswer;
                context.Session.Remove("MathEquForms");
                return isValid;

            }
            catch (Exception)
            {

                return isValid = false;
                // return false;

            }
        }
    }
}
