using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data.Repository.IRepository;
using Farmer.Extensions;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;
using Utility;

namespace Farmer.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private CartViewModel cartVM;

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            cartVM = new CartViewModel()
            {
                OrderHeader = new Models.OrderHeader(),
                ServiceList = new List<Service>()
            };
        }
        public IActionResult Index()
        {
            if(HttpContext.Session.GetObject<List<int>>(SD.SessionCart)!=null)
            {
                var sessionList = new List<int>();
                sessionList = HttpContext.Session.GetObject<List<int>>(SD.SessionCart);
               
                foreach (var item in sessionList)
                {
                    cartVM.ServiceList.Add(_unitOfWork.Service.GetFirstOrDefault(u => u.Id == item, includeProperties: "Frequency,Category"));
                }
            }
            return View(cartVM);
        }
        public IActionResult Summary()
        {
            if (HttpContext.Session.GetObject<List<int>>(SD.SessionCart) != null)
            {
                var sessionList = new List<int>();
                sessionList = HttpContext.Session.GetObject<List<int>>(SD.SessionCart);
                foreach (var item in sessionList)
                {
                    cartVM.ServiceList.Add(_unitOfWork.Service.GetFirstOrDefault(filter:u => u.Id == item, includeProperties: "Frequency,Category"));
                }
            }
            return View(cartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public IActionResult SummaryPOST(OrderHeader orderHeader)
        {
            if (HttpContext.Session.GetObject<List<int>>(SD.SessionCart) != null)
            {
                var sessionList = new List<int>();
                sessionList = HttpContext.Session.GetObject<List<int>>(SD.SessionCart);
                cartVM.ServiceList = new List<Service>();
                cartVM.OrderHeader = orderHeader;
                foreach (var itemId in sessionList)
                {
                    cartVM.ServiceList.Add(_unitOfWork.Service.Get(itemId));
                }
            }
            if(!ModelState.IsValid)
            {
                return View(cartVM);
            }
            else
            {
                cartVM.OrderHeader.OrderDate = DateTime.Now;
                cartVM.OrderHeader.Status = SD.StatusSubmitted;
                cartVM.OrderHeader.ServiceCount = cartVM.ServiceList.Count;
                _unitOfWork.OrderHeader.Add(cartVM.OrderHeader);
                _unitOfWork.Save();
                foreach (var item in cartVM.ServiceList)
                {
                    OrderDetails details = new OrderDetails
                    {
                        ServiceId = item.Id,
                        OrderHeaderId=cartVM.OrderHeader.Id,
                        ServiceName=item.Name,
                        Price=item.Price
                    };
                    _unitOfWork.OrderDetails.Add(details);
                    
                }
                _unitOfWork.Save();
                HttpContext.Session.SetObject(SD.SessionCart, new List<int>());
                return RedirectToAction("OrderConfirmation","Cart",new { id = cartVM.OrderHeader.Id});
            }
        }
        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }
        public IActionResult RemoveItem(int serviceId)
        {
            var sessionList = new List<int>();
            sessionList = HttpContext.Session.GetObject<List<int>>(SD.SessionCart);
            sessionList.Remove(serviceId);
            HttpContext.Session.SetObject(SD.SessionCart, sessionList);
            return RedirectToAction(nameof(Index));
        }
    }
}