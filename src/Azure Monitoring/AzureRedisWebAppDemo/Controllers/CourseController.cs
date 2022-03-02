using AzureRedisWebAppDemo.Models;
using AzureRedisWebAppDemo.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AzureRedisWebAppDemo.Controllers
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
            IEnumerable<Course> courses = this.courseService.GetCourses();
            return View(courses);
        }
        public IActionResult Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            return View(courseService.GetCourse(id));
        }

        [HttpPost]
        public IActionResult Edit(Course course)
        {
            courseService.UpdateCourse(course);
            return RedirectToAction("Index");
        }
    }
}
