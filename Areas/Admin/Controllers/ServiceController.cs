using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataAccess.Data.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Farmer.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ServiceController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _hostEnvironment;
        [BindProperty]
        public ServiceVM ServVM { get; set; }
        public ServiceController(IUnitOfWork unitOfWork, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _hostEnvironment = hostEnvironment;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Upsert(int? id)
        {
            ServVM = new ServiceVM()
            {
                Service = new Models.Service(),
                CategoryList = _unitOfWork.Category.GetCategoryListForDropDown(),
                FrequencyList=_unitOfWork.Frequency.GetFrequencyListForDropDown()
            };
            if(id!=null)
            {
                ServVM.Service = _unitOfWork.Service.Get(id.GetValueOrDefault());
            }
            return View(ServVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(Service service)
        {
            if(ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if(service.Id==0)
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/services", files[0].FileName);          
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                      await files[0].CopyToAsync(stream);
                    }
                    service.ImageUrl = files[0].FileName;
                    _unitOfWork.Service.Add(service);
                }
                else
                {
                    var serviceObj = _unitOfWork.Service.Get(service.Id);
                    if (files.Count > 0)
                    {
                        var oldImage = serviceObj.ImageUrl;
                        if(oldImage != null)
                        {
                            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/services", oldImage);
                            if (System.IO.File.Exists(path))
                            {
                                System.IO.File.Delete(path);
                            }
                            path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/services", files[0].FileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await files[0].CopyToAsync(stream);
                            }
                            service.ImageUrl = files[0].FileName;
                        }  
                        else
                        {
                           var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/services", files[0].FileName);
                            using (var stream = new FileStream(path, FileMode.Create))
                            {
                                await files[0].CopyToAsync(stream);
                            }
                            service.ImageUrl = files[0].FileName;
                        }
                    }                  
                       _unitOfWork.Service.Update(service);
                }
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        #region API
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new { data = _unitOfWork.Service.GetAll(includeProperties: "Category,Frequency") });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var serviceFromDb = _unitOfWork.Service.Get(id);
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/services", serviceFromDb.ImageUrl);
           
            if(serviceFromDb == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }          
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
            _unitOfWork.Service.Remove(serviceFromDb);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Deleted successfully" });
        }
        #endregion
    }
}
