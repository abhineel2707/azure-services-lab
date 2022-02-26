using DockerDemo_ConnectingToSSMSOnContainer.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DockerDemo_ConnectingToSSMSOnContainer.Services
{
    public class CourseService
    {
        private readonly IConfiguration configuration;

        public CourseService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private SqlConnection GetSqlConnection()
        {
            return new SqlConnection(this.configuration.GetConnectionString("sqlConnection"));
        }

        public IEnumerable<Course> GetCourses()
        {
            List<Course> courses = new List<Course>();

            string query = "SELECT CourseID, CourseName, Rating FROM Course";

            SqlConnection sqlConnection = GetSqlConnection();

            sqlConnection.Open();

            SqlCommand command = new SqlCommand(query, sqlConnection);

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

            sqlConnection.Close();
            return courses;
        }
    }
}
