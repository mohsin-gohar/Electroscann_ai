using ElectroScanAI.Models.Entities;
using ElectroScanAI.Models.Enums;
using Electroscann_ai.Data;
using Electroscann_ai.Models;
using Electroscann_ai.Models.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Electroscann_ai.Controllers
{
    public class AccountController : Controller
    {
        private readonly ElectroscannDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountController(ElectroscannDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        // ========== LOGIN GET ==========
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // ========== LOGIN POST ==========
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .Include(u => u.ElectricianProfile)
                    .Include(u => u.CompanyProfile)
                    .FirstOrDefaultAsync(u => u.Email == model.Email && !u.IsDeleted);

                if (user != null && VerifyPassword(user.PasswordHash, model.Password))
                {
                    // Update last login
                    user.LastLoginAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync();

                    // Create claims
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name, user.FullName),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role.ToString()),
                        new Claim("UserId", user.Id.ToString()),
                        new Claim("FullName", user.FullName)
                    };

                    // Add role-specific claims
                    if (user.Role == UserRole.Electrician && user.ElectricianProfile != null)
                    {
                        claims.Add(new Claim("ElectricianId", user.ElectricianProfile.Id.ToString()));
                        claims.Add(new Claim("IsVerified", user.ElectricianProfile.IsVerified.ToString()));
                    }
                    else if (user.Role == UserRole.Company && user.CompanyProfile != null)
                    {
                        claims.Add(new Claim("CompanyId", user.CompanyProfile.Id.ToString()));
                        claims.Add(new Claim("CompanyName", user.CompanyProfile.CompanyName));
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    // Redirect to appropriate dashboard based on role
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction(GetDashboardAction(user.Role), GetDashboardController(user.Role));
                }

                ModelState.AddModelError(string.Empty, "Invalid email or password.");
            }

            return View(model);
        }

        // ========== REGISTER GET ==========
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // ========== REGISTER POST ==========
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (model.Role == UserRole.Electrician)
            {
                if (string.IsNullOrWhiteSpace(model.LicenseNumber))
                {
                    ModelState.AddModelError(nameof(RegisterViewModel.LicenseNumber), "License number is required for electricians.");
                }

                if (string.IsNullOrWhiteSpace(model.Specialization))
                {
                    ModelState.AddModelError(nameof(RegisterViewModel.Specialization), "Specialization is required for electricians.");
                }
            }
            else if (model.Role == UserRole.Company)
            {
                if (string.IsNullOrWhiteSpace(model.CompanyName))
                {
                    ModelState.AddModelError(nameof(RegisterViewModel.CompanyName), "Company name is required for companies.");
                }

                if (string.IsNullOrWhiteSpace(model.NTNNumber))
                {
                    ModelState.AddModelError(nameof(RegisterViewModel.NTNNumber), "NTN number is required for companies.");
                }

                if (string.IsNullOrWhiteSpace(model.Industry))
                {
                    ModelState.AddModelError(nameof(RegisterViewModel.Industry), "Industry is required for companies.");
                }
            }

            var yearsOfExperienceValue = model.YearsOfExperience?.Trim();
            if (!string.IsNullOrWhiteSpace(yearsOfExperienceValue))
            {
                if (!int.TryParse(yearsOfExperienceValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedYears) || parsedYears < 0 || parsedYears > 50)
                {
                    ModelState.AddModelError(nameof(RegisterViewModel.YearsOfExperience), "Years of experience must be a whole number between 0 and 50.");
                }
            }

            if (ModelState.IsValid)
            {
                // Check if email already exists
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email already registered.");
                    return View(model);
                }

                var yearsOfExperience = 0;
                if (int.TryParse(yearsOfExperienceValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsedValue))
                {
                    yearsOfExperience = parsedValue;
                }

                // Create new user
                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    City = model.City ?? "",
                    Role = model.Role,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false,
                    EmailVerified = false
                };

                user.PasswordHash = _passwordHasher.HashPassword(user, model.Password);

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    // Create role-specific profile
                    if (model.Role == UserRole.Electrician)
                    {
                        var electrician = new Electrician
                        {
                            UserId = user.Id,
                            LicenseNumber = model.LicenseNumber ?? string.Empty,
                            Specialization = model.Specialization ?? string.Empty,
                            YearsOfExperience = yearsOfExperience,
                            IsVerified = false,
                            CreatedAt = DateTime.UtcNow,
                            Rating = 0,
                            TotalReviews = 0,
                            CompletedJobs = 0
                        };
                        _context.Electricians.Add(electrician);
                    }
                    else if (model.Role == UserRole.Company)
                    {
                        var company = new Company
                        {
                            UserId = user.Id,
                            CompanyName = model.CompanyName ?? string.Empty,
                            NTNNumber = model.NTNNumber ?? string.Empty,
                            Industry = model.Industry ?? string.Empty,
                            IsVerified = false,
                            CreatedAt = DateTime.UtcNow
                        };
                        _context.Companies.Add(company);
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    TempData["Success"] = "Registration successful! Please login.";
                    return RedirectToAction(nameof(Login));
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    ModelState.AddModelError(string.Empty, "Registration failed. Please try again.");
                    Console.WriteLine($"Registration error: {ex.Message}");
                }
            }

            return View(model);
        }

        // ========== LOGOUT ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // ========== FORGOT PASSWORD GET ==========
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        // ========== FORGOT PASSWORD POST ==========
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user != null)
                {
                    // Generate password reset token (simplified)
                    var token = Guid.NewGuid().ToString();
                    // Store token in database or send email
                    TempData["Success"] = "Password reset link has been sent to your email.";
                    return RedirectToAction(nameof(Login));
                }
                TempData["Success"] = "If an account exists, a reset link has been sent.";
            }
            return View(model);
        }

        // ========== ACCESS DENIED ==========
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        // ========== HELPER METHODS ==========
        private bool VerifyPassword(string hashedPassword, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(null, hashedPassword, password);
            return result == PasswordVerificationResult.Success;
        }

        private string GetDashboardAction(UserRole role)
        {
            return role switch
            {
                UserRole.Admin => "Index",
                UserRole.Electrician => "Index",
                UserRole.Company => "Index",
                UserRole.Client => "Index",
                _ => "Index"
            };
        }

        private string GetDashboardController(UserRole role)
        {
            return role switch
            {
                UserRole.Admin => "Dashboard",
                UserRole.Electrician => "ElectricianDashboard",
                UserRole.Company => "CompanyDashboard",
                UserRole.Client => "ClientDashboard",
                _ => "Home"
            };
        }
    }
}