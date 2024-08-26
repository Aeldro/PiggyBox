// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using WildPay.Models.Entities;

namespace WildPay.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ResendEmailConfirmationModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Un email de vérification a été envoyé.");
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);

            string emailTitle = $"🐷 PiggyBox : Validez votre adresse email";
            string emailContent = $"<h1>🐷 Bienvenue sur PiggyBox 🐷</h1><h3>Vous venez de créer un compte sur PiggyBox. Avant de pouvoir vous connecter, vous devez confirmer votre adresse email.</h3><a style=\"display: inline-block; font-weight: 400; text-align: center; vertical-align: middle; user-select: none; border: 1px solid transparent; line-height: 1.5; transition: color 0.15s ease-in-out, background-color 0.15s ease-in-out, border-color 0.15s ease-in-out, box-shadow 0.15s ease-in-out; cursor: pointer; text-decoration: none; padding: 0.5rem 1rem; font-size: 1.25rem; line-height: 1.5; border-radius: 0.3rem; background-color: rgba(74, 163, 162, 1); color: rgba(245, 245, 245); border-color: rgba(74, 163, 162, 1); margin-top: 1em;\" href=\"{HtmlEncoder.Default.Encode(callbackUrl)}\">Je confirme mon adresse email</a>";
            await _emailSender.SendEmailAsync(
                Input.Email,
                emailTitle,
                emailContent);

            ModelState.AddModelError(string.Empty, "Un email de vérification a été envoyé.");
            return Page();
        }
    }
}
