using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace hackatlon
{
    /*
     * INITIAL CONSIDERATIONS
     * 1) the maximum distance should be parameterized and requested at the beginning
     * 2) the toll calculation is done in a method that returns a double
     * 3) parameterize all waiting times with constants but without requesting them
     * the switch statement with 3 conditions takes half the time than if statement --> https://code-maze.com/csharp-comparing-performance-of-the-switch-and-if-else-statements/
    */


    internal class Program
    {
        static string logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FileDiLog.txt");
        static Random r = new Random();
        static Mutex IsFree = new Mutex();
        static int numCarsEntered = 0;
        static int maxCarsAllowed;
        static int MaxKm;

        #region Costanti di attesa      
        static int Telepass = 3000;
        static int Cash = 5000;
        static int HevyVehicle = 8000;
        #endregion

        #region Costanti per pagamenti
        static double BasePrice = 0.50;
        #endregion

        /// <summary>
        /// The entry point of the program. Initializes the simulation by requesting user input for the number of toll gates, 
        /// the maximum number of cars allowed, and the maximum distance. Creates and starts threads to simulate toll gate operations.
        /// </summary>
        /// <param name="args">Command-line arguments (not used in this program).</param>
        static void Main(string[] args)
        {
            if (File.Exists(logFilePath))
                File.Delete(logFilePath);

            List<Thread> TollGates = new List<Thread>();
            Console.Write("How many toll gates do you want to add? ");
            int numTollGates;
            int.TryParse(Console.ReadLine(), out numTollGates);

            Console.Write("How many cars do you want to let pass? ");
            int.TryParse(Console.ReadLine(), out maxCarsAllowed);


            Console.Write("What is the maximum distance that can be traveled? ");
            int.TryParse(Console.ReadLine(), out MaxKm);

            for (int i = 0; i < numTollGates; i++)
            {
                int index = i;
                TollGates.Add(new Thread(() => TollGate(index)));
            }

            foreach (Thread thread in TollGates)
            {
                thread.Start();
            }
        }

        /// <summary>
        /// Calculates the toll fee for a vehicle based on the distance traveled and the type of vehicle.
        /// </summary>
        /// <param name="numKm">The number of kilometers traveled by the vehicle.</param>
        /// <param name="vehicleType">The type of vehicle, represented as an integer.</param>
        /// <returns>The calculated toll fee as a double.</returns>
        static double CalcoloPedaggio(int numKm, int vehicleType)
        {
            return BasePrice + (BasePrice * vehicleType) + (BasePrice * (numKm / 10));
        }


        /// <summary>
        /// Returns the waiting time in milliseconds based on the type of vehicle.
        /// </summary>
        /// <param name="vehicleType">The type of vehicle: 1 for cash payment, 2 for Telepass, 3 for heavy vehicles.</param>
        /// <returns>The waiting time in milliseconds for the specified vehicle type.</returns>
        static int ReturnTempo(int vehicleType)
        {
            switch (vehicleType)
            {
                case 1:
                    return Cash;
                case 2:
                    return Telepass;
                case 3:
                    return HevyVehicle;
                default:
                    return 0;
            }
        }


        /// <summary>
        /// Writes a given string to the log file in a thread-safe manner using a mutex.
        /// </summary>
        /// <param name="s">The string to be written to the log file.</param>
        static void Writer(string s)
        {
            IsFree.WaitOne();
            StreamWriter writer = new StreamWriter("FileDiLog.txt", true);
            writer.WriteLine(s);
            writer.Close();
            IsFree.ReleaseMutex();
        }


        /// <summary>
        /// Simulates the operation of a toll gate, processing vehicles as they pass through.
        /// It calculates the toll fee, logs the vehicle's entry and exit, and tracks the time taken.
        /// </summary>
        /// <param name="tollGateNumber">The identifier of the toll gate being simulated.</param>
        static void TollGate(int tollGateNumber)
        {
            Stopwatch cronometro = new Stopwatch();
            while (true)
            {
                if (numCarsEntered >= maxCarsAllowed)
                    return;
                cronometro.Reset();
                cronometro.Start();
                IsFree.WaitOne();
                numCarsEntered++;
                IsFree.ReleaseMutex();

                int vehicleType = r.Next(1, 4);
                int paymentType;

                if (vehicleType == 3)
                    paymentType = 3;
                else
                    paymentType = r.Next(1, 3);
                int kmTraveled = r.Next(10, MaxKm);
                string s = string.Format("TIME {1} --> vehicle entered toll gate {0}", tollGateNumber, DateTime.Now.ToString("h:mm:ss.ff t"));
                Console.WriteLine(s);
                Writer(s);

                //pagamento

                Thread.Sleep(ReturnTempo(paymentType));
                string st = string.Format("TOLL GATE {0} TIME {4} --> vehicle type: {1} km traveled {2} price paid {3} euros",
                    tollGateNumber, vehicleType, kmTraveled, CalcoloPedaggio(vehicleType, kmTraveled), DateTime.Now.ToString("h:mm:ss.ff t"));
                
                Console.WriteLine(st);
                Writer(st);

                cronometro.Stop();
                string elapsedTime = cronometro.ElapsedMilliseconds.ToString();

                string finalString = string.Format("TOLL GATE {0} TIME {1} --> the vehicle has cleared the toll gate. Its time was {2} milliseconds", tollGateNumber, DateTime.Now.ToString("h:mm:ss.ff t"), elapsedTime);
                Console.WriteLine(finalString);
                Writer(finalString);
            }
        }
    }
}