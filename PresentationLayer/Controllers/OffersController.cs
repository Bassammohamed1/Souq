using DomainLayer.DTOs;
using DomainLayer.Interfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ViewModels;
using Souq.Models;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OffersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender emailSender;
        private readonly UserManager<AppUser> userManager;

        private async Task CreateDepartmentsItemsViewBags(IEnumerable<Department> departments)
        {
            foreach (var department in departments)
            {
                if (department.Name == "Appliances")
                {
                    var appliancesItems = await _unitOfWork.Departments.GetDepartmentItems(department);
                    ViewBag.Appliances = appliancesItems;
                }
                else if (department.Name == "Electronics")
                {
                    var electronicsItems = await _unitOfWork.Departments.GetDepartmentItems(department);
                    ViewBag.Electronics = electronicsItems;
                }
                else if (department.Name == "Mobile Phones")
                {
                    var mobilePhonesItems = await _unitOfWork.Departments.GetDepartmentItems(department);
                    ViewBag.Phones = mobilePhonesItems;
                }
                else if (department.Name == "Video Games")
                {
                    var videoGamesItems = await _unitOfWork.Departments.GetDepartmentItems(department);
                    ViewBag.Games = videoGamesItems;
                }
            }
        }

        public OffersController(IUnitOfWork unitOfWork, IEmailSender emailSender, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            this.emailSender = emailSender;
            this.userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Offers()
        {
            var offers = await _unitOfWork.Offers.GetAllOffers();

            return View(offers);
        }

        public async Task<IActionResult> CreateOffer()
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();

            var categories = await _unitOfWork.Departments.GetAllDepartmentsCategories(departments);

            var items = await _unitOfWork.Items.GetAll(1, int.MaxValue);

            await CreateDepartmentsItemsViewBags(departments);

            var offerVM = new OfferViewModel()
            {
                Departments = departments.OrderBy(d => d.Name).ToList(),
                Categories = categories.OrderBy(c => c.Name).ToList(),
                Items = items.OrderBy(i => i.Name).ToList()
            };

            return View(offerVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CreateOffer(OfferDTO offer)
        {
            if (ModelState.IsValid && offer.ClientFile is not null)
            {
                await _unitOfWork.Offers.CreateOffer(offer);

                foreach (var user in userManager.Users)
                {
                    await emailSender.SendEmailAsync(user.Email, "Check our new offer!!", @$"
                            <div style=""font-family: Amiri, serif; max-width: 600px; margin: auto; padding: 20px; 
                                border: 1px solid #e0e0e0; border-radius: 10px; background-color: #fdfdfd;"">
    
                              <h2 style=""color: #e67e22;"">🔥 Hot New Offer Just for You!</h2>
  
                              <p style=""font-size: 16px; color: #555;"">
                                Hello {user.UserName}, we've just launched an exciting new offer you won't want to miss!
                              </p>

                               <div style='margin: 20px 0; text-align: center;'>
                                     <img src='{offer.ImageSrc}' alt='Offer Banner' 
                                     style='width: 100%; max-height: 300px; border-radius: 8px; object-fit: cover;' />
                              </div>

                              <p style=""font-size: 15px; color: #666;"">
                                <strong>{offer.OfferType}</strong>
                              </p>

                              <div style=""text-align: center; margin: 30px 0;"">
                                <a href=""https://yourdomain.com"" 
                                   style=""padding: 12px 25px; background-color: #e67e22; color: #fff; text-decoration: none; border-radius: 5px;"">
                                  View Offer
                                </a>
                              </div>

                              <p style=""font-size: 14px; color: #999;"">
                                This offer is available for a limited time, so act fast!
                              </p>

                              <p style=""font-size: 14px; color: #555; margin-top: 30px;"">
                                Stay tuned for more exclusive deals!<br />
                                <strong>Souq.eg</strong>
                              </p>
                            </div>");
                }

                return RedirectToAction("Offers");
            }

            return View();
        }

        public async Task<IActionResult> Update(int id)
        {
            var departments = await _unitOfWork.Departments.GetAllWithoutPagination();

            if (id != null)
            {
                var categories = await _unitOfWork.Departments.GetAllDepartmentsCategories(departments);

                var items = await _unitOfWork.Items.GetAll(1, int.MaxValue);

                await CreateDepartmentsItemsViewBags(departments);

                var offer = await _unitOfWork.Offers.FindOfferByID(id);

                var offerVM = new OfferViewModel()
                {
                    ID = offer.ID,
                    Departments = departments.OrderBy(d => d.Name).ToList(),
                    Categories = categories.OrderBy(c => c.Name).ToList(),
                    Items = items.OrderBy(i => i.Name).ToList(),
                    OfferType = offer.OfferType,
                    DepartmentName = offer.DepartmentName,
                    CategoryName = offer.CategoryName,
                    ItemID = offer.ItemID,
                    FixedDiscountValue = offer.FixedDiscountValue,
                    PercentDiscount = offer.PercentDiscount,
                    ItemOneID = offer.ItemOneID,
                    ItemTwoID = offer.ItemTwoID,
                    PromoCode = offer.PromoCode,
                    PromoDiscountType = offer.PromoDiscountType,
                    PromoDiscountValue = offer.PromoDiscountValue
                };

                return View(offerVM);
            }

            return RedirectToAction("Offers");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(OfferDTO offer)
        {
            await _unitOfWork.Offers.UpdateOffer(offer);
            await _unitOfWork.Commit();

            return RedirectToAction("Offers");
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id != null)
            {
                var offer = await _unitOfWork.Offers.FindOfferByID(id);

                return View(offer);
            }

            return RedirectToAction("Offers");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(Offer offer)
        {
            await _unitOfWork.Offers.DeleteOffer(offer.ID);
            await _unitOfWork.Commit();

            return RedirectToAction("Offers");
        }
    }
}
