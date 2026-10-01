using Application.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Speech.Synthesis;

namespace WebSite.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult GetCaptchaImage()
        {
            int width = 100;
            int height = 36;
            var captchaCode = Captcha.GenerateCaptchaCode();
            var result = Captcha.GenerateCaptchaImage(width, height, captchaCode);
            HttpContext.Session.SetString("CaptchaCode", result.CaptchaCode);
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");

        }

        [HttpGet]
        public IActionResult GetCaptchaCode()
        {
            var code = HttpContext.Session.GetString("CaptchaCode");
            return Json(new { captcha = code });
        }
        //[HttpGet]
        //public IActionResult Audio()
        //{
        //    var captchaText = HttpContext.Session.GetString("CaptchaCode") ?? "1234";

        //    using var synth = new SpeechSynthesizer();
        //    using var stream = new MemoryStream();
        //    synth.SetOutputToWaveStream(stream);
        //    synth.Speak(captchaText);

        //    stream.Position = 0;
        //    return File(stream.ToArray(), "audio/wav");
        //}
    }
}
