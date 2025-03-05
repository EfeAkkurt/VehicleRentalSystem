using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using ConsoleHelpers;
using Dapper;
using Microsoft.Data.SqlClient;

namespace VehicleRentalSystem
{
    internal class Car
    {
        public int Id { get; set; }
        public string BrandName { get; set; }
        public string ModelName { get; set; }
        public int Price { get; set; }
        public string LicensePlate { get; set; }
        public bool? Status { get; set; }
        public string Category { get; set; }
        public DateTime RentalTime { get; set; }
        public DateTime ReturnDate { get; set; }
    }
    internal static class Helper
    {
        public static int MakeChoice(string[] options, string msj = "Please make a choice")
        {
            Console.WriteLine(msj);
            for (int i = 0; i < options.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {options[i]}");
            }

            Console.Write("Choice:\n ");
            string inputSecim = Console.ReadKey(true).KeyChar.ToString();
            if (int.TryParse(inputSecim, out int secim))
            {
                if (secim > 0 && secim <= options.Length)
                {
                    return secim;
                }
            }

            Wrong("You must make a valid choice!\nTry again by checking the options.\n");
            return MakeChoice(options, msj);
        }

        public static int GetId(string Question)
        {
            string inputNum = GetInfo(Question);
            if (int.TryParse(inputNum, out int num))
            {
                return num;
            }
            Wrong("Input numbers only!");
            return GetId(Question);
        }

        public static void Wrong(string msj)
        {
            ColorfulMessageShow(msj, ConsoleColor.Red);
        }

        public static void ColorfulMessageShow(string msj, ConsoleColor renk)
        {
            Console.ForegroundColor = renk;
            Console.WriteLine(msj);
            Console.ResetColor();
        }

        public static void Successful(string msj)
        {
            ColorfulMessageShow(msj, ConsoleColor.Green);
        }

        public static string GetInfo(string Question)
        {
            Console.Write($"{Question}: ");
            return Console.ReadLine();
        }

        public static bool GetBool(string question)
        {
            Console.WriteLine(question);
            string inputBool = Console.ReadLine().ToLower();

            if (inputBool == "true")
            {
                return true;
            }
            else if (inputBool == "false")
            {
                return false;
            }

            Console.WriteLine("Please enter ('true/Renting' or 'false/Not Renting')!");
            return GetBool(question);
        }

        public static void PlayAddCarSound()
        {
            //fun
            Console.Beep(700, 200);
            Console.Beep(900, 200);
            Console.Beep(1100, 300);
            Console.Beep(900, 200);
        }

        public static void PlayUpdateCarSound()
        {
            // Update
            Console.Beep(500, 300);
            Console.Beep(600, 200);
            Console.Beep(800, 200);
            Console.Beep(600, 200);
            Console.Beep(500, 300);
        }

        public static void PlayRentCarSound()
        {
            // enthusiastic
            Console.Beep(800, 200);
            Console.Beep(1000, 200);
            Console.Beep(1200, 300);
            Console.Beep(1400, 200);
            Console.Beep(1600, 400);
        }

        public static void PlayReturnCarSound()
        {
            // Sad
            Console.Beep(500, 400);
            Console.Beep(400, 300);
            Console.Beep(300, 500);
            Console.Beep(400, 300);
            Console.Beep(350, 400);
        }

        public static void PlayMaintenanceSound()
        {
            Console.Beep(600, 150);
            Console.Beep(700, 150);
            Console.Beep(800, 150);
            Console.Beep(700, 150);
            Console.Beep(600, 300);
        }

        public static void PlayMozartIntro()
        {
            Console.Beep(659, 300); 
            Console.Beep(622, 300); 
            Console.Beep(659, 300); 
            Console.Beep(622, 300); 
            Console.Beep(659, 300); 
            Console.Beep(494, 300); 
            Console.Beep(587, 300); 
            Console.Beep(523, 300); 
            Console.Beep(440, 600); 

            Console.Beep(440, 300); 
            Console.Beep(494, 300); 
            Console.Beep(523, 300); 
            Console.Beep(587, 300); 
            Console.Beep(659, 600); 
            Console.Beep(622, 300); 
            Console.Beep(659, 300); 
            Console.Beep(622, 300); 
            Console.Beep(659, 300); 
            Console.Beep(494, 300); 
            Console.Beep(587, 300); 
            Console.Beep(523, 300); 
            Console.Beep(440, 600); 

            Console.Beep(659, 300); 
            Console.Beep(523, 300); 
            Console.Beep(587, 300); 
            Console.Beep(494, 300); 
            Console.Beep(440, 300); 
            Console.Beep(494, 300); 
            Console.Beep(523, 300); 
            Console.Beep(440, 600);
        }

        public static void PlayRetryMessage()
        {
            using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
            {
                synthesizer.Volume = 100;
                synthesizer.Rate = 0;

                synthesizer.Speak("Please Try Again.");
            }
        }
    }
} 


