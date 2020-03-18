using EmployeeManagementMVCCoreClient.Models;
using EmployeeManagementMVCCoreClient.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagementMVCCoreClient.Controllers
{
    [Authorize]
    //[Route("[controller]/[action]")] // si on le met ici plus besoin de le mettre dans les actions
    public class HomeController : Controller
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IHostingEnvironment _environment;

        public HomeController(IEmployeeRepository employeeRepository, IHostingEnvironment environment)
        {
            _employeeRepository = employeeRepository;
            _environment = environment;
        }

        [AllowAnonymous]
        //[Route("")]// Si on navigate to the root application
        //[Route("Home")]//Si on navigate to the root/Home             ici c le Attribute Routing qui se fait dans
        //[Route("Home/Index")]//Si on navigate to the root/Home/Index   startup.cs avec app.UseMcv()
        public ViewResult Index()
        {
            var model = _employeeRepository.GetAllEmployees();
            return View(model);
        }

        public ViewResult Details(int? id)
        {
            //throw new Exception("Error in Details view");

            Employee employee = _employeeRepository.GetEmployee(id.Value); 
            if(employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id.Value);
            }


            HomeDetailsViewModel hdvm = new HomeDetailsViewModel()
            {
                employee = employee, //_employeeRepository.GetEmployee(id ?? 1),
                pageTitle = "Employee Details"
            };

            return View(hdvm);
        }

        [HttpGet]
        [AllowAnonymous]
        public ViewResult Create()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ViewResult Edit(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);

            EmployeeEditViewModel eevm = new EmployeeEditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath
            };

            return View(eevm);
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Edit(EmployeeEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Department = model.Department;

                if (model.Photo != null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(_environment.WebRootPath, "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    employee.PhotoPath = ProcessUploadedFile(model);
                }

                _employeeRepository.Update(employee);

                return RedirectToAction("index");
            }
            return View();
        }

        private string ProcessUploadedFile(EmployeeCreateViewModel model)
        {
            string uniqueFileName = null;
            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "images"); //WebRootPath gives us the wwww folder
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }

            }

            return uniqueFileName;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Create(EmployeeCreateViewModel model) //RedirectToActionResult et ViewResult implementent IactionResult donc possible
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);              

                Employee newEmployee = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Department = model.Department,
                    PhotoPath = uniqueFileName
                };

                _employeeRepository.Add(newEmployee);

                return RedirectToAction("details", new { id = newEmployee.Id });
            }
            return View();
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            Employee employee = _employeeRepository.GetEmployee(id);

            EmployeeDeleteViewModel employeeDeleteViewModel = new EmployeeDeleteViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Department = employee.Department,
                ExistingPhotoPath = employee.PhotoPath

            };

            return View(employeeDeleteViewModel);
        }

        [HttpPost]
        public IActionResult Delete(EmployeeDeleteViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeRepository.GetEmployee(model.Id);

                if (employee != null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        //string uploadsFolder = Path.Combine(_environment.WebRootPath, "imagesSupprimees"); //WebRootPath gives us the wwww folder
                        //string uniqueFileName = Guid.NewGuid().ToString() + "_" + employee.PhotoPath;
                        //string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        
                        //using (var fileStream = new FileStream(filePath, FileMode.Create))
                        //{
                        //    System.IO.File.Copy(employee.PhotoPath, filePath2);
                        //}

                        string filePath = Path.Combine(_environment.WebRootPath, "images", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                      
                    }
                    _employeeRepository.Delete(employee.Id);
                    return RedirectToAction("index");
                }
            }
            return View();
        }
    }
}
