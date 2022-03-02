using AppInsightsDemo.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AppInsightsDemo.Services
{
    public class CourseService
    {
        private readonly IConfiguration configuration;

        public CourseService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        private SqlConnection GetConnection()
        {
            string connectionString = this.configuration.GetConnectionString("sqlConnection");
            return new SqlConnection(connectionString);
        }

        public IEnumerable<Course> GetCourses()
        {
            List<Course> courses = new List<Course>();

            string statement = "SELECT CourseID,CourseName,Rating FROM Course";

            SqlConnection connection = this.GetConnection();
            connection.Open();

            SqlCommand command = new SqlCommand(statement, connection);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Course course = new Course()
                    {
                        CourseID = Convert.ToInt32(reader[0]),
                        CourseName = Convert.ToString(reader[1]),
                        Rating = Convert.ToDecimal(reader[2])
                    };

                    courses.Add(course);
                }
            }

            return courses;
        }

        public void UpdateCourse(Course updatedCourse)
        {
            StringBuilder sb = new StringBuilder("UPDATE Course SET Rating=");
            sb.Append(updatedCourse.Rating);
            sb.Append(" WHERE CourseID=");
            sb.Append(updatedCourse.CourseID);

            SqlConnection connection = this.GetConnection();
            connection.Open();

            SqlCommand command = new SqlCommand(sb.ToString(), connection);
            command.ExecuteNonQuery();
        }

        public Course GetCourse(string id)
        {
            IEnumerable<Course> courses = this.GetCourses();
            Course course = courses.FirstOrDefault(c => c.CourseID == Convert.ToInt32(id));

            return course;
        }
    }
}
