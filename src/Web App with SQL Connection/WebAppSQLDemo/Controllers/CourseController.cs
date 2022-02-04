using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using WebAppSQLDemo.Models;
using WebAppSQLDemo.Services;

namespace WebAppSQLDemo.Controllers
{
    public class CourseController : Controller
    {
        private readonly CourseService _courseService;
        private readonly IConfiguration _configuration;

        public CourseController(CourseService courseService, IConfiguration configuration)
        {
            this._courseService = courseService;
            this._configuration = configuration;
        }
        public IActionResult Index()
        {
            IEnumerable<Course> courses = _courseService.GetCourses(_configuration.GetConnectionString("SQLConnection"));
            return View(courses);
        }
    }
}
