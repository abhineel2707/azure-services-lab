using DockerDemo_ConnectingToSSMSOnContainer.Models;
using DockerDemo_ConnectingToSSMSOnContainer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace DockerDemo_ConnectingToSSMSOnContainer.Controllers
{
    public class CourseController : Controller
    {
        private readonly CourseService courseService;

        public CourseController(CourseService courseService)
        {
            this.courseService = courseService;
        }
        public IActionResult Index()
        {
            IEnumerable<Course> courses = courseService.GetCourses();
            return View(courses);
        }
    }
}
