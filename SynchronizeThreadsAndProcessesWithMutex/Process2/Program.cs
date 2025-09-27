using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Process1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var mutex = new Mutex(false, "Global\\VicMutexBtw2Processes"))
            {
                string filePath = @"c:\temp\mycounter.txt";

                for (int i = 0; i < 5; i++)
                {
                    mutex.WaitOne();

                    try
                    {
                        int counter = 0;

                        if (File.Exists(filePath))
                            counter = int.Parse(File.ReadAllText(filePath));

                        counter++;
                        File.WriteAllText(filePath, counter.ToString());

                        Console.WriteLine($"Process2 updated counter to {counter}");

                    }
                    finally
                    {
                        mutex.ReleaseMutex();
                    }

                    Thread.Sleep(500); // simulate work
                }
            }

            Console.WriteLine("Process2 done. Press any key to exit.");
            Console.ReadKey();
        }
    }
}
