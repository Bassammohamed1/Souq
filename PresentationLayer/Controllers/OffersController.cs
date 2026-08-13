using ApplicationLayer.DTOs;
using ApplicationLayer.Interfaces.ServicesInterfaces;
using DomainLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PresentationLayer.ViewModels;

namespace PresentationLayer.Controllers
{
    [Authorize(Roles = "Admin")]
    public class OffersController : Controller
    {
        private readonly IOffersService _offers;
        private readonly IDepartmentsService _departments;

        private async Task CreateDepartmentsItemsViewBags(IEnumerable<Department> departments)
        {
            foreach (var department in departments)
            {
                if (department.Name == "Appliances")
                {
                    var appliancesItems = await _departments.GetDepartmentItems(department);
                    ViewBag.Appliances = appliancesItems;
                }
                else if (department.Name == "Electronics")
                {
                    var electronicsItems = await _departments.GetDepartmentItems(department);
                    ViewBag.Electronics = electronicsItems;
                }
                else if (department.Name == "Mobile Phones")
                {
                    var mobilePhonesItems = await _departments.GetDepartmentItems(department);
                    ViewBag.Phones = mobilePhonesItems;
                }
                else if (department.Name == "Video Games")
                {
                    var videoGamesItems = await _departments.GetDepartmentItems(department);
                    ViewBag.Games = videoGamesItems;
                }
            }
        }

        public OffersController(IOffersService offers, IDepartmentsService departments)
        {
            _offers = offers;
            _departments = departments;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Offers()
        {
            var offers = await _offers.GetAllOffers();

            var offersVM = offers.
                Select(o => new OfferViewModel
                {
                    ID = o.ID,
                    DepartmentName = o.DepartmentName,
                    CategoryName = o.CategoryName,
                    ItemID = o.ItemID,
                    OfferType = o.OfferType,
                    FixedDiscountValue = o.FixedDiscountValue,
                    PercentDiscount = o.PercentDiscount,
                    ItemOneID = o.ItemOneID,
                    ItemTwoID = o.ItemTwoID,
                    PromoCode = o.PromoCode,
                    PromoDiscountValue = o.PromoDiscountValue,
                    PromoDiscountType = o.PromoDiscountType,
                    ImageSrc = o.ImageSrc
                });

            return View(offersVM);
        }

        public async Task<IActionResult> CreateOffer()
        {
            var result = await _offers.GetOfferWithRelatedData();

            await CreateDepartmentsItemsViewBags(await _departments.GetDepartments());

            var offerVM = new OfferViewModel()
            {
                Departments = result.Departments,
                Categories = result.Categories,
                Items = result.Items
            };

            return View(offerVM);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> CreateOffer(OfferDTO offer)
        {
            if (ModelState.IsValid && offer.ClientFile is not null)
            {
                var result = await _offers.CreateOffer(offer);

                return result.Success ? RedirectToAction("Offers") : View();
            }

            return View();
        }

        public async Task<IActionResult> Update(int id)
        {
            if (id != null)
            {
                await CreateDepartmentsItemsViewBags(await _departments.GetDepartments());

                var result = await _offers.GetOfferWithRelatedData(id);

                var offerVM = new OfferViewModel()
                {
                    ID = result.ID,
                    Departments = result.Departments,
                    Categories = result.Categories,
                    Items = result.Items,
                    OfferType = result.OfferType,
                    DepartmentName = result.DepartmentName,
                    CategoryName = result.CategoryName,
                    ItemID = result.ItemID,
                    FixedDiscountValue = result.FixedDiscountValue,
                    PercentDiscount = result.PercentDiscount,
                    ItemOneID = result.ItemOneID,
                    ItemTwoID = result.ItemTwoID,
                    PromoCode = result.PromoCode,
                    PromoDiscountType = result.PromoDiscountType,
                    PromoDiscountValue = result.PromoDiscountValue
                };

                return View(offerVM);
            }

            return RedirectToAction("Offers");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Update(OfferDTO offer)
        {
            if (ModelState.IsValid && offer.ClientFile is not null)
            {
                var result = await _offers.UpdateOffer(offer);

                return result.Success ? RedirectToAction("Offers") : View();
            }

            return View();
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id != null)
            {
                var offer = await _offers.GetOffer(id);

                return View(offer);
            }

            return RedirectToAction("Offers");
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Delete(Offer offer)
        {
            if (ModelState.IsValid && offer.ClientFile is not null)
            {
                var result = await _offers.DeleteOffer(offer.ID);

                return result.Success ? RedirectToAction("Offers") : View();
            }

            return View();
        }
    }
}
