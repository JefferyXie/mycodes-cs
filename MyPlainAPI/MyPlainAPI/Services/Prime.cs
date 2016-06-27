using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace MyPlainAPI.Services
{
    public class Prime
    {
        public async Task<int[]> GetAllPrimesAsync(int upperbound)
        {
            var results = await Task.Run(() =>
            {
                return GetAllPrimes(0, upperbound);
            });
            Global.Log.Write("[" + Thread.CurrentThread.ManagedThreadId + "] after await Task.Run");
            return results;
        }
        public Task<int[]> GetAllPrimes(int upperbound)
        {
            var t = Task.Run(() => GetAllPrimes(0, upperbound));
            Global.Log.Write("[" + Thread.CurrentThread.ManagedThreadId + "] after Task.Run");
            return t;
        }
        public int[] GetAllPrimes(int lowerbound, int upperbound)
        {
            Global.Log.Write("["+Thread.CurrentThread.ManagedThreadId+"] entering GetAllPrimes(int, int)");
            List<int> primes = new List<int>();
            for (int i = lowerbound; i < upperbound; ++i)
            {
                if (IsPrime(i)) primes.Add(i);
            }
            Global.Log.Write("["+Thread.CurrentThread.ManagedThreadId+"] finished GetAllPrimes(int, int)");
            return primes.ToArray();
        }
        public bool IsPrime(int n)
        {
            for (int i = 2; i <= Math.Sqrt(Math.Abs(n)); ++i)
            {
                if (n % i == 0) return false;
            }
            return true;
        }
    }
}