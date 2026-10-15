using Application.Dtos;
using Application.Extensions;
using Application.ServiceInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]/[action]")]
    public class VigilanceController : Controller
    {
        private readonly IDataService _dataService;

        public VigilanceController(IDataService dataService)
        {
            _dataService = dataService;
        }

        // ── Existing: Create — UNCHANGED ──
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] VigilanceFormDTO argModelDto)
        {
            if (argModelDto == null) return BadRequest("Input not valid or null");
            if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());

            var modelDto = await _dataService.vigilance.Create(argModelDto).ConfigureAwait(false);
            if (modelDto == null)
                return BadRequest("Submission failed. Please ensure email is verified and 24-hour limit is respected.");

            return Ok(modelDto);
        }

        // ═══════════════════════════════════════════════════════════
        //  NEW #1: SubmitEmail — OTP response mein return karo
        // ═══════════════════════════════════════════════════════════
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SubmitEmail([FromBody] VigilanceOtpRequest req)
        {
            if (req == null)
                return Ok(new { success = false, message = "Request body is required.", otp = (string)null, resendLeft = (int?)null });

            var (success, message, otp, resendLeft) = await _dataService.vigilance
                .SubmitEmail(req.Email)
                .ConfigureAwait(false);

            return Ok(new { success, message, otp, resendLeft });
        }

        // ═══════════════════════════════════════════════════════════
        //  NEW #2: Verify OTP
        // ═══════════════════════════════════════════════════════════
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> VerifyOtp([FromBody] VigilanceVerifyOtpRequest req)
        {
            if (req == null)
                return Ok(new { success = false, message = "Request body is required.", attemptsLeft = (int?)null });

            var (success, message, attemptsLeft) = await _dataService.vigilance
                .VerifyOtp(req.Email, req.Otp).ConfigureAwait(false);

            return Ok(new { success, message, attemptsLeft });
        }

        // ═══════════════════════════════════════════════════════════
        //  NEW #3: Resend OTP — OTP response mein return karo
        // ═══════════════════════════════════════════════════════════
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResendOtp([FromBody] VigilanceOtpRequest req)
        {
            if (req == null)
                return Ok(new { success = false, message = "Request body is required.", otp = (string)null, resendLeft = (int?)null });

            var (success, message, otp, resendLeft) = await _dataService.vigilance
                .ResendOtp(req.Email).ConfigureAwait(false);

            return Ok(new { success, message, otp, resendLeft });
        }
    }
}