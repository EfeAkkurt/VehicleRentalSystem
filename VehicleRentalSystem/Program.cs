using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Channels;
using System.Speech.Synthesis;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Win32;
using VehicleRentalSystem;

namespace VehicleRentalSystem
{
    internal class Program 
    {
        static string connectionString = "";

        static void Main(string[] args)
        {
            User user = new User();
            User userService = new User();

            void MainMenu()
            {
                while (true)
                {
                    Console.WriteLine("1. Login");
                    Console.WriteLine("2. Register");
                    Console.WriteLine("9. Log Out");

                    int infoChoice = int.Parse(Console.ReadLine());

                    if (infoChoice == 1)
                    {
                        Login();
                    }
                    else if (infoChoice == 2)
                    {
                        Register();
                    }
                    else if (infoChoice == 9)
                    {
                        Console.WriteLine("Good Bye!");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("You have made an incorrect selection. Press the button to return to the menu");
                        Console.ReadKey();
                        MainMenu();
                    }
                }
            }

            void Login()
            {
                Console.Clear();

                Console.WriteLine("Username:");
                string username = Console.ReadLine();

                Console.WriteLine("Password");
                string password = Console.ReadLine();

                var currentUser = userService.Login(username, password);

                if (currentUser == null)
                {
                    Console.WriteLine("You logged in incorrectly or incompletely, please try again!");
                    Console.WriteLine("Press any key to return to the menu");
                    Helper.PlayRetryMessage();
                    Thread.Sleep(1000);
                    Console.ReadKey();
                    Console.Clear();
                }
                else
                {
                    Console.WriteLine($"Welcome {currentUser.Username}");
                    Helper.PlayMozartIntro();
                    ListUserMenu(currentUser);
                }
            }

            void Register()
            {
                Console.Clear();

                while (true)
                {
                    Console.WriteLine("User Name: ");
                    string username = Console.ReadLine();

                    if (userService.IsUserExist(username))
                    {
                        Console.WriteLine("This username uses");
                        continue;
                    }

                    Console.WriteLine("Password: ");
                    string password = Console.ReadLine();

                    userService.Register(username, password);
                    Console.WriteLine("Registration successful. Press any key for the login screen");
                    Console.ReadKey();
                    Login();
                    break;
                }
            }

            void ListUserMenu(User currentUser)
            {
                Console.Clear();
                int inputChoice = Helper.MakeChoice(
                    new[] { "list available vehicles", "All List Cars", "Add Vehicles", "Edit Vehicle", "Rent a Car", "Return The Vehicle", "Send The Car For Maintenance", "Delete Car", "Exit" },
                    "Car automation\n".ToUpper()
                );

                switch (inputChoice)
                {
                    case 1:
                        ListAvaibleCars(currentUser);
                        break;
                    case 2:
                        AllListCars(currentUser);
                        break;
                    case 3:
                        AddCar(currentUser);
                        break;
                    case 4:
                        EditVehicle(currentUser);
                        break;
                    case 5:
                        RentCar(currentUser);
                        break;
                    case 6:
                        ReturnVehicle(currentUser);
                        break;
                    case 7:
                        Maintenance(currentUser);
                        break;
                    case 8:
                        DeleteCar(currentUser);
                        break;
                    case 9:
                        Exit();
                        break;
                }
            }

            void BackMenu(User currentUser)
            {
                Console.WriteLine("\nPress a key to return to the menu...");
                Console.ReadKey();
                Console.Clear();
                ListUserMenu(currentUser);
                return;
            }

            void ListAvaibleCars(User currentUser)
            {
                Console.Clear();

                using var connection = new SqlConnection(connectionString);

                var CarsStatus = connection.Query<Car>(
                    @"SELECT c.Id, c.BrandName, c.ModelName, c.Price, c.Category  
                    FROM CarsStatus cs
                    LEFT JOIN CarsTable c ON cs.Id = c.Id 
                    WHERE cs.Status = @Status",
                    new { Status = "Avaible" }).ToList();

                foreach (var car in CarsStatus)
                {
                    Console.WriteLine($"{car.Id} - {car.BrandName} - {car.ModelName} - {car.Price} $ - {car.Category}");
                }

                Console.WriteLine("\nIf you want to rent a car, press '1'");
                Console.WriteLine("If you want to Add car, press '2'");
                Console.WriteLine("If you want to go back, press '0'");
                Console.WriteLine("If you want to exit, press '9'");
                Console.Write("Your choice: ");

                string inputKey = Console.ReadLine();

                if (inputKey =="1")
                {
                    RentCar(currentUser);
                }
                else if (inputKey == "2")
                {
                    AddCar(currentUser);
                }
                else if (inputKey == "0")
                {
                    BackMenu(currentUser);
                }
                else if (inputKey == "9")
                {
                    Exit();
                }
                else
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                }

                BackMenu(user);
                return;
            }

            void AllListCars(User currentUser)
            {
                using var connection = new SqlConnection(connectionString);

                var CarsTable = connection.Query<Car>("SELECT * FROM CarsTable").ToList();

                foreach (var car in CarsTable)
                {
                    Console.WriteLine($"{car.Id} - {car.BrandName} - {car.ModelName} - {car.Price} $ {car.LicensePlate} - {car.Status} - {car.Category}\n");

                    if (car.Status == true)
                    {
                        if (car.ReturnDate != null)
                        {
                            var rentalPeriod = car.ReturnDate - car.RentalTime;
                            Console.WriteLine($"Rental periods of leased vehicles: {rentalPeriod.TotalHours:F0} hours");
                        }
                        else if (car.ReturnDate == null && car.Status == true)
                        {
                            Console.WriteLine($"{car.Id} - {car.BrandName} - {car.ModelName} - {car.Price} $ {car.LicensePlate} - {car.Status} - {car.Category}\n");
                            Console.WriteLine("The vehicle is rented");
                        }
                    }
                }

                BackMenu(user);
            }

            void AddCar(User currentUser)
            {
                using var connection = new SqlConnection(connectionString);

                Console.Clear();

                var CarsTable = connection.Query<Car>("SELECT * FROM CarsTable").ToArray();

                Console.WriteLine("Add Car \n.".ToUpper());

                string inputBrandName = Helper.GetInfo("Brand Name");
                string inputModelName = Helper.GetInfo("Model Name");
                string inputPlate = Helper.GetInfo("Plate");
                string inputPrice = Helper.GetInfo("Price");
                bool inputStatus = Helper.GetBool("Status: available (false)/ not available (true)");
                string inputCategory = Helper.GetInfo("Category");

                var sql = @"INSERT INTO 
                            CarsTable (BrandName, ModelName, Plate, Price, Status, Category, RentalTime)
                            VALUES (@BrandName, @ModelName, @Plate, @Price, @Status, @Category, @RentalTime)";

                var newCar = new
                {
                    BrandName = inputBrandName,
                    ModelName = inputModelName,
                    Plate = inputPlate,
                    Price = inputPrice,
                    Status = inputStatus,
                    Category = inputCategory,
                    RentalTime = DateTime.Now
                };

                var affRows = connection.Execute(sql, newCar);

                if (affRows > 0)
                {
                    Helper.Successful("Car added.");
                    Thread.Sleep(1000);
                    Helper.PlayAddCarSound();
                }
                else
                {
                    Helper.Wrong("Could not be added!");
                    Thread.Sleep(1000);
                }

                ListUserMenu(currentUser);
            }

            void RentCar(User currentUser)
            {
                using var connection = new SqlConnection(connectionString);


                ListAvaibleCars(currentUser);

                int inputId = Helper.GetId("Choice car");

                var sql = @"UPDATE CarsTable SET Status = @Status, RentalTime = @RentalTime WHERE Id = @Id ";

                var newCar = new
                {
                    Id = inputId,
                    RentalTime = DateTime.Now,
                    Status = true
                };

                connection.Execute(sql, newCar);

                Console.WriteLine("Your vehicle is rented...");
                Thread.Sleep(1500);
                Helper.ColorfulMessageShow("Rented!", ConsoleColor.Green);
                Helper.PlayRentCarSound();
                Thread.Sleep(1000);

                BackMenu(currentUser);
            }

            void EditVehicle(User currentUser)
            {
                using var connection = new SqlConnection(connectionString);
                var CarsTable = connection.Query<Car>("SELECT * FROM CarsTable").ToArray();

                Console.Clear();
                Console.WriteLine("ALL VEHICLE\n");

                foreach (var car in CarsTable)
                {
                    Console.WriteLine($"{car.Id} {car.BrandName} - {car.ModelName} - {car.Price} - {car.LicensePlate} - {car.RentalTime} - {car.ReturnDate}");
                }

                Console.WriteLine("");
                Console.WriteLine("Press the 'M' button to return to the menu!");
                Console.WriteLine("\nIf you do not want to return to the menu, press any key except 'M'\n");

                if (Console.ReadKey().Key == ConsoleKey.M)
                {
                    ListUserMenu(currentUser);
                    return;
                }

                int inputId = Helper.GetId("Select Car Number:");
                string inputBrandName = Helper.GetInfo("New Brand Name:");
                string inputModelName = Helper.GetInfo("New Model Name:");
                int inputPrice = Helper.GetId("New Price:");
                string inputLicensePlate = Helper.GetInfo("New License Plate:");
                int inputRentalTime = Helper.GetId("New Rental Time");
                int inputReturnDate = Helper.GetId("New Return Date");
                bool inputStatus = Helper.GetBool("New Status");
                string inputCategory = Helper.GetInfo("New Category");

                var sql = @"UPDATE CarsTable SET BrandName = @BrandName, ModelName = @ModelName, 
                            Price = @Price, LicensePlate = @LicensePlate, RentalTime = @RentalTime, 
                            ReturnDate = @ReturnDate, Status = @Status, Category = @Category WHERE Id = @Id";

                var newCar = new
                {
                    BrandName = inputBrandName,
                    ModelName = inputModelName,
                    Price = inputPrice,
                    LicensePlate = inputLicensePlate,
                    RentalTime = inputRentalTime,
                    ReturnDate = inputReturnDate,
                    Status = inputStatus,
                    Category = inputCategory,
                    Id = inputId
                };

                connection.Execute(sql, newCar);

                Console.WriteLine("Updated!");
                BackMenu(user);
            }

            void ReturnVehicle(User currentUser)
            {
                Console.Clear();

                using var connection = new SqlConnection(connectionString);
                var CarsStatus = connection.Query<Car>(
                    @"SELECT cs.Id, cs.Status, c.BrandName, c.ModelName 
                    FROM CarsStatus cs
                    LEFT JOIN CarsTable c ON cs.Id = c.Id 
                    WHERE cs.Status = @Status",
                    new { Status = "NotAvaible" }).ToList(); // burasını kendim yazmadım internetten bakarak tamamiyle yazdım yalan yok
                // ve yinede patlıyorm burada

                    foreach (var car in CarsStatus)
                    {
                        Console.WriteLine($"{car.Id} - {car.BrandName} - {car.ModelName}");
                    }


                int inputId = Helper.GetId("Choice Car");

                var sql = @"UPDATE CarsTable SET Status = @Status, ReturnDate = @ReturnDate WHERE Id = @Id";

                var newCar = new
                {
                    Id = inputId,
                    ReturnDate = DateTime.Now,
                    Status = false
                };

                connection.Execute(sql, newCar);

                Helper.ColorfulMessageShow("Returned", ConsoleColor.Green);
                BackMenu(currentUser);
            }

            void Maintenance(User currentUser)
            {
                    Console.Clear();

                    Console.WriteLine("Enter the car's ID to send it to maintenance:");
                    int inputId = int.Parse(Console.ReadLine());

                    using var connection = new SqlConnection(connectionString);
                    var sql = @"UPDATE CarsStatus SET Status = 'Maintenance' WHERE Id = @Id";

                    var newCar = new
                    {
                        Id = inputId,
                    };
                    connection.Execute(sql, newCar);

                    Console.WriteLine("Car is under maintenance now!");
                    BackMenu(currentUser);
            }

            void DeleteCar(User currentUser)
            {
                using var connection = new SqlConnection(connectionString);

                Console.Clear();

                Console.WriteLine("Enter the car's ID to delete:");
                int inputId = int.Parse(Console.ReadLine());

                var sql = "DELETE FROM CarsTable WHERE Id = @Id";

                connection.Execute(sql, new { Id = inputId });

                Helper.ColorfulMessageShow("Car Deleted", ConsoleColor.Red);
                BackMenu(user);
            }

            void Exit()
            {
                Console.Clear();
                Console.WriteLine("Exiting...");
            }

            MainMenu();
        }
    }
}
// IsNullOrWhiteSpace veya NullOrEmpty entere basarsa eğer yeni birşeyler girerken veya güncellerken onu boş bırakmıyor
// hata vermiyor direk eskisini yapıştırıyor tabiki whitespace kullan daha iyi space sapce ard arda basıp boşluk koymasına boş görünmesine izin vemriyor
