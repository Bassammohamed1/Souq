// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System;
using System.Threading.Tasks;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace PresentationLayer.Areas.Identity.Pages.Account.Manage
{
    public class PersonalDataModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<PersonalDataModel> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public PersonalDataModel(
            UserManager<AppUser> userManager,
            ILogger<PersonalDataModel> logger,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> OnGet()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();
            ViewData["Departments"] = departments;

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return Page();
        }
    }
}
