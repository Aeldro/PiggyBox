// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WildPay.Helpers;
using WildPay.Models.Entities;
using WildPay.Services.Interfaces;

namespace WildPay.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<IndexModel> _logger;
        private readonly ICloudinaryService _cloudinaryService;

        public IndexModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<IndexModel> logger,
            ICloudinaryService cloudinaryService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _cloudinaryService = cloudinaryService;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// 
        [Display(Name = "Identifiant")]
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

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
            [Phone]
            [Display(Name = "Téléphone")]
            public string PhoneNumber { get; set; }

            [Required]
            [MaxLength(25)]
            [Display(Name = "Prénom")]
            public string Firstname { get; set; }

            [Required]
            [MaxLength(25)]
            [Display(Name = "Nom de famille")]
            public string Lastname { get; set; }

            [Display(Name = "Photo de profil"),]
            //[Required(ErrorMessage = "Vous devez ajouter une photo de profil")]
            [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
            [MaxFileSize(2 * 1024 * 1024)]
            public IFormFile Image { get; set; }

            [BindProperty]
            public string? ImageUrl { get; set; }
        }

        private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var firstname = user.Firstname;
            var lastname = user.Lastname;
            var imageUrl = user.ImageUrl;

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                Firstname = firstname,
                Lastname = lastname,
                ImageUrl = imageUrl,
                Image = null
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            ViewData["imageUrl"] = user.ImageUrl;

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }

            user.Firstname = Input.Firstname;
            user.Lastname = Input.Lastname;

            // idéalement trouver un moyen de checker si la photo a été changée
            if (Input.Image != null && Input.Image.Length > 0)
            {
                if (!string.IsNullOrEmpty(user.ImagePublicID))
                {
                    try
                    {
                        await _cloudinaryService.DeleteImageCloudinaryAsync(user.ImagePublicID);
                    }
                    catch (Exception cloudinaryException)
                    {
                        _logger.LogInformation(cloudinaryException.Message);
                    }
                }

                try
                {
                    List<string> ImageInfos = await _cloudinaryService.UploadImageCloudinaryAsync(Input.Image);
                    user.ImageUrl = ImageInfos[0];
                    user.ImagePublicID = ImageInfos[1];
                }
                catch (Exception cloudinaryException)
                {
                    _logger.LogInformation(cloudinaryException.Message);
                }
            }

            var result = await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Le profil a bien été modifié";
            return RedirectToPage();
        }
    }
}
