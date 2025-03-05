using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleHelpers;
using Dapper;
using Microsoft.Data.SqlClient;

namespace VehicleRentalSystem
{
    internal class User
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }


        static string connectionString = "";// connection string
        public User Login(string username, string password)
        {
            using var connection = new SqlConnection(connectionString);

            return connection.QueryFirstOrDefault<User>("SELECT * FROM CarsTableUsers WHERE Username = @Username AND Password = @Password",
                new { Username = username, Password = password });
        }

        public void Register(string username, string password)
        {
            using var connection = new SqlConnection(connectionString);

            connection.Execute("INSERT INTO CarsTableUsers (Username, Password) VALUES (@Username, @Password)", new
            {
                Username = username,
                Password = password
            });
        }

        public bool IsUserExist(string username)
        {
            using var connection = new SqlConnection(connectionString);

            var existingUser = connection.QueryFirstOrDefault<User>("SELECT * FROM CarsTableUsers WHERE Username = @Username", new
            {
                Username = username
            });

            return existingUser != null;
        }


        public Car GetCarById(int carId)
        {
            using var connection = new SqlConnection(connectionString);

            return connection.QueryFirstOrDefault<Car>("SELECT * FROM CarsTable WHERE Id = @Id ", new
            {
                Id = carId,
            });
        }
    }
}
