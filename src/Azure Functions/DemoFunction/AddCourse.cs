using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Data.SqlClient;
using System.Data;

namespace DemoFunction
{
    public static class AddCourse
    {
        [FunctionName("AddCourse")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Course data = JsonConvert.DeserializeObject<Course>(requestBody);

            //string connectionString = @"Server=tcp:SERVER-NAME.database.windows.net,1433;Initial Catalog=DB-NAME;Persist Security Info=False;User ID=USERNAME;Password=PASSWORD;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

            string connectionString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_SqlConnectionString");

            string statement = @"INSERT INTO Course(CourseID,CourseName,Rating) VALUES(@param1,@param2,@param3)";

            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (SqlCommand command = new SqlCommand(statement, connection))
            {
                command.Parameters.Add("@param1", SqlDbType.Int).Value = data.CourseID;
                command.Parameters.Add("@param2", SqlDbType.VarChar).Value = data.CourseName;
                command.Parameters.Add("@param3", SqlDbType.Decimal).Value = data.Rating;

                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }
            connection.Close();

            return new OkObjectResult("Course Added");
        }
    }
}
