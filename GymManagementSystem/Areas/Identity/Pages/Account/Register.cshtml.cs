// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.Areas.Identity.Pages.Account
{
    // Kayıt sayfasını temsil eden PageModel sınıfı
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager; // ROL YÖNETİCİSİ EKLENDİ

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager) // ROL YÖNETİCİSİ CONSTRUCTOR'A EKLENDİ
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager; // ROL YÖNETİCİSİ ATAMASI
        }

        // Giriş verilerini tutan özellik
        [BindProperty]
        public InputModel Input { get; set; }

        // Geri dönüş URL'si
        public string ReturnUrl { get; set; }

        // Harici Giriş Şemaları
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        // Kullanıcı Giriş Modelini temsil eden iç sınıf
        public class InputModel
        {
            [Required(ErrorMessage = "E-posta adresi zorunludur.")]
            [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
            [Display(Name = "E-posta Adresi")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Şifre zorunludur.")]
            [StringLength(100, ErrorMessage = "Şifre en az {2} ve en fazla {1} karakter uzunluğunda olmalıdır.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Şifre")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Şifreyi Onayla")]
            [Compare("Password", ErrorMessage = "Şifre ile onay şifresi eşleşmiyor.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var user = CreateUser();

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                // Kullanıcıyı oluştur
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("Kullanıcı şifre ile yeni bir hesap oluşturdu.");

                    // **********************************************
                    // 🚨 ÖNEMLİ EKLEME: Varsayılan Üye Rolü Atama
                    // **********************************************
                    var defaultRole = "Member";

                    // Rolün mevcut olup olmadığını kontrol et
                    if (!await _roleManager.RoleExistsAsync(defaultRole))
                    {
                        // Rol yoksa oluştur (Bu işlem genellikle SeedData'da yapılır)
                        await _roleManager.CreateAsync(new IdentityRole(defaultRole));
                    }

                    // Yeni kullanıcıya varsayılan rolü ata
                    await _userManager.AddToRoleAsync(user, defaultRole);
                    _logger.LogInformation($"{user.UserName} kullanıcısına '{defaultRole}' rolü başarıyla atandı.");
                    // **********************************************

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Hesabınızı Onaylayın",
                        $"Hesabınızı onaylamak için lütfen <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>buraya tıklayınız</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        // E-posta onayı gerekiyorsa onay sayfasına yönlendir
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        // E-posta onayı gerekmiyorsa kullanıcıyı doğrudan oturum açtır
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    // Identity hatalarını ModelState'e ekle
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Hata oluşursa formu tekrar göster
            return Page();
        }

        // Yeni IdentityUser nesnesi oluşturur
        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                // Hata oluşursa özel bir istisna fırlat
                throw new InvalidOperationException($"'{nameof(IdentityUser)}' örneği oluşturulamıyor. " +
                    $"'{nameof(IdentityUser)}' soyut bir sınıf olmadığından ve parametresiz bir kurucuya sahip olduğundan emin olun, veya alternatif olarak " +
                    $"/Areas/Identity/Pages/Account/Register.cshtml adresindeki kayıt sayfasını override edin.");
            }
        }

        // Kullanıcı e-posta deposunu döndürür
        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("Varsayılan UI, e-posta desteği olan bir kullanıcı deposu gerektirir.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}