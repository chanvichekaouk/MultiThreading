using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UsingAsynAwaitWithThreadPool
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // We will use Asyncronious programming here so the main thread won't be block and able to do its own work
            // (I/O bounds e.g our DoWork can be network related e.g consuming API, file IO , or db connection)
            // Also we will use worker thread (thread pool) where CLR will pick the idle thread or free thread
            // from its pool to do our work (CPU bound e.g complex computation, image procession etc.. that would
            // consume much of CPU (brain) so we use work thread to offload CPU usage on another available CPU core present)
            Task myWork = DoWorkWithWorkerThread();

            // Main thread is not blocked by myWork Task and continue to do its work here
            // This is best practice so that our application is responsive esp when dealing with UI
            // But here we use console so it is easy to demonstrate it
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"Main thread- thread ID:{Thread.CurrentThread.ManagedThreadId} doing work #{i}");
                Thread.Sleep(3000);
            }

            await myWork;

            //Expected output is sth like this: it illustrated that 
            // The main thread (#1) is not blocked while worker threads (#3 and #4) are running in parallel, 
            // pulling tasks from the thread pool and doing their work.
            /* 
            DoWorkWithWorkerThread CPU2 started.thread ID: 3
            Main thread-thread ID: 1 doing work #0
            Main thread-thread ID: 1 doing work #1
            Main thread-thread ID: 1 doing work #2
            DoWorkWithWorkerThread CPU1 started.thread ID:4
            Main thread-thread ID: 1 doing work #3
            Main thread-thread ID: 1 doing work #4
            Main thread-thread ID: 1 doing work #5
            Main thread-thread ID: 1 doing work #6
            Main thread-thread ID: 1 doing work #7
            Main thread-thread ID: 1 doing work #8
            Main thread-thread ID: 1 doing work #9
            */
        }

        private static async Task DoWorkWithWorkerThread()
        {
            await Task.Run(() => myCPUWork1());
            await Task.Run(() => myCPUWork2());
        }

        private static async Task myCPUWork2()
        {
            // simulate long computation utilizing cpu work
            Console.WriteLine($"DoWorkWithWorkerThread CPU1 started. thread ID:{Thread.CurrentThread.ManagedThreadId}");
            await Task.Delay(8000);
        }

        private static async Task myCPUWork1()
        {
            // simulate long computation utilizing cpu work
            Console.WriteLine($"DoWorkWithWorkerThread CPU2 started. thread ID: {Thread.CurrentThread.ManagedThreadId}");
            await Task.Delay(7000);
        }
    }
}
