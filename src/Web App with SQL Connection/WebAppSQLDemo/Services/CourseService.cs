using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using WebAppSQLDemo.Models;

namespace WebAppSQLDemo.Services
{
    public class CourseService
    {
        //private static string db_source = "";
        //private static string db_user = "";
        //private static string db_password = "";
        //private static string db_database = "";

        private SqlConnection GetConnection(string connectionString)
        {
            //var _builder = new SqlConnectionStringBuilder();
            //_builder.DataSource = db_source;
            //_builder.UserID = db_user;
            //_builder.Password = db_password;
            //_builder.InitialCatalog = db_database;
            //return new SqlConnection(_builder.ConnectionString);
            return new SqlConnection(connectionString);
        }

        public IEnumerable<Course> GetCourses(string connectionString)
        {
            List<Course> courses = new List<Course>();

            string statement = "SELECT CourseID,CourseName,Rating FROM Course;";

            SqlConnection connection = GetConnection(connectionString);

            connection.Open();

            SqlCommand sqlCommand = new SqlCommand(statement, connection);

            using (var reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    Course course = new Course()
                    {
                        CourseID = reader.GetInt32(0),
                        CourseName = reader.GetString(1),
                        Rating = reader.GetDecimal(2)
                    };
                    courses.Add(course);
                }
            }

            connection.Close();

            return courses;
        }
    }
}
