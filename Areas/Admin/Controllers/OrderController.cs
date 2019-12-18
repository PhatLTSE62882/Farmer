using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using Utility;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Farmer.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            var orderVM = new OrderViewModel { 
            OrderHeader = _unitOfWork.OrderHeader.Get(id),
            OrderDetails = _unitOfWork.OrderDetails.GetAll(filter: f=>f.OrderHeaderId==id)           
            };       
            return View(orderVM);
        }
        public IActionResult Approve(int id)
        {         
            var objFromDb = _unitOfWork.OrderHeader.Get(id);
            if(objFromDb != null)
            {
                objFromDb.Status = SD.StatusApproved;
                _unitOfWork.Save();
            }          
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Reject(int id)
        {
            var objFromDb = _unitOfWork.OrderHeader.Get(id);
            if (objFromDb != null)
            {
                objFromDb.Status = SD.StatusRejected;
                _unitOfWork.Save();
            }
            return RedirectToAction(nameof(Index));
        }
        #region API
        public IActionResult GetAllOrders()
        {
            return Json(new { data = _unitOfWork.OrderHeader.GetAll() });
        }
        public IActionResult GetAllPendingOrders()
        {
            return Json(new { data = _unitOfWork.OrderHeader.GetAll(filter:f=>f.Status==SD.StatusSubmitted) });
        }
        public IActionResult GetAllApprovedOrders()
        {
            return Json(new { data = _unitOfWork.OrderHeader.GetAll(filter: f => f.Status == SD.StatusApproved) });
        }
        #endregion
    }
}
