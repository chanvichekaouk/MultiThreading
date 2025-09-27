using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadSafeUsingSimpleLock
{
    class MyBankABC
    {
        // This balance field will be our shared resource used by multiple worker threads.
        // We need to protect it to prevent race conditions, where two or more threads update it simultaneously.
        // For example, without thread safety, if thread #1 and thread #2 both try to deposit $50 at the same time,
        // they might both read the initial balance as $0 and each set it to $50.
        // In this case, we “lose” $50 because we expected the balance to be $100.
        // This is why it’s important to protect shared resources accessed by multiple threads.
        // Using a lock ensures that only one thread can access the resource at a time.
        // Once a thread finishes updating, it releases the lock so another thread can safely access the resource.
        private decimal balance; 

        private readonly object _lock = new object();
        public MyBankABC(decimal balance)
        {
            this.balance = balance;
        }

        public decimal GetBalance()
        {
            lock(_lock)
            {
                return balance;
            }
        }

        public void Deposite(decimal amount)
        {
            lock (_lock)
            {
                balance += amount;
            }
        }
    }

    class Program
    {


        static async Task Main(string[] args)
        {
            MyBankABC vicBank = new MyBankABC(0);

            Task t1 = Task.Run(() => { vicBank.Deposite(50); });
            Task t2 = Task.Run(() => { vicBank.Deposite(50); });

            await Task.WhenAll(t1, t2); 

            Debug.Assert(vicBank.GetBalance() == 100);
        }
    }
}
