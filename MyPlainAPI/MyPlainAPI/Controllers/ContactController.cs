using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using MyPlainAPI.Models;
using MyPlainAPI.Services;
using MyPlainAPI.Helper;
using System.Threading;

namespace MyPlainAPI.Controllers
{
    public class ContactController : ApiController
    {
        private ContactRepository contactRepository;
        public ContactController()
        {
            this.contactRepository = new ContactRepository();
        }
        [HttpGet]
        [ActionName("contacts")]
        public Contact[] GetContacts()
        {
            return this.contactRepository.GetAllContacts();
        }
        [HttpGet]
        [ActionName("contact")]
        //public Contact GetContact([FromUri]string id)
        public Contact GetContact(string id)
        {
            var reqContent = this.Request.Content.ReadAsStringAsync().Result;
            return new Contact() { Id = int.Parse(id) };
        }
        [HttpPost]
        [ActionName("contact")]
        public HttpResponseMessage PostContact([FromBody]Contact contact)
        {
            var reqContent = this.Request.Content.ReadAsStringAsync().Result;
            HttpResponseMessage response = null;
            if (this.contactRepository.SaveContact(contact))
            {
                response = Request.CreateResponse<Contact>(HttpStatusCode.Created, contact);
            }
            else
            {
                response = Request.CreateResponse<Contact>(HttpStatusCode.Conflict, contact);
            }
            return response;
        }

        [HttpPost, ActionName("raw")]
        public async Task<string> PostRawAsync()
        {
            // a way to get any raw post data from request
            var reqContent = await this.Request.Content.ReadAsStringAsync();
            return reqContent;
        }
        [HttpPost, ActionName("rawstring")]
        public string PostString([RawHttpRequestBody]string raw)
        {
            return raw;
        }
        [HttpPost, ActionName("rawbyte")]
        public byte[] PostBytes([RawHttpRequestBody]byte[] raw)
        {
            //return raw.Length + " bytes sent";
            return raw;
        }
        // http://weblog.west-wind.com/posts/2013/Dec/13/Accepting-Raw-Request-Body-Content-with-ASPNET-Web-API
        // with route setting you can call like /api/contact/rawmultiple/5/jack
        // without route setting you can call like /api/contact/rawmultiple?id=5&name=jack
        [HttpPost, ActionName("rawmultiple")]
        public string PostMultiple(int id, [RawHttpRequestBody]string body, string name)
        {
            return id + body + name;
        }
        /// <summary>
        /// this is synchronous interface and codes will be executed line by line
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        [HttpGet, ActionName("primessync")]
        public int[] GetAllPrimesSync(int id /*n*/)
        {
            int n = id;
            Global.Log.Write("[" + Thread.CurrentThread.ManagedThreadId + "] before calling GetAllPrimes(int,int)");
            Prime primer = new Prime();
            var t = primer.GetAllPrimes(0, n);
            Global.Log.Write("[" + Thread.CurrentThread.ManagedThreadId + "] after calling GetAllPrimes(int)");
            return t;
        }
        /// <summary>
        /// this is asynchronous interface, steps are -
        /// 1) run until primer.GetAllPrimes which immediately returns Task<int[]>
        /// 2) execute codes until t.Result, block current thread and wait until Task finishes
        /// 3) go ahead and execute following codes
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        [HttpGet, ActionName("primes")]
        public int[] GetAllPrimes(int id /*n*/)
        {
            int n = id;
            Global.Log.Write("[" + Thread.CurrentThread.ManagedThreadId + "] before calling GetAllPrimes(int)");
            Prime primer = new Prime();
            var t = primer.GetAllPrimes(n);
            Global.Log.Write("[" + Thread.CurrentThread.ManagedThreadId + "] after calling GetAllPrimes(int)");
            var results = t.Result;
            Global.Log.Write("[" + Thread.CurrentThread.ManagedThreadId + "] retrieved the results");
            return results;
        }
        /// <summary>
        /// this is not working as asynchronous interface, the steps are -
        /// 1) run until await primer.GetAllPrimesAsync
        /// 2) the caller of this api will get control immediately and execute its codes
        /// 3) however, since it's not clear who the caller is, this api will block current thread
        /// and wait unti calculation finishes
        /// 4) execute Global.Log and following codes after calculaiton is done
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        [HttpGet, ActionName("primesawait")]
        public async Task<int[]> GetAllPrimesAsync(int id /*n*/)
        {
            int n = id;
            Global.Log.Write("[" + Thread.CurrentThread.ManagedThreadId + "] before calling GetAllPrimesAsync(int)");
            Prime primer = new Prime();
            var tt = await primer.GetAllPrimesAsync(n);
            Global.Log.Write("[" + Thread.CurrentThread.ManagedThreadId + "] after calling GetAllPrimesAsync(int)");
            var response = tt;
            Global.Log.Write("[" + Thread.CurrentThread.ManagedThreadId + "] retrieved the results tt");
            return response;
        }
        /// <summary>
        /// this is asynchronous interface, steps are -
        /// 1) run until primer.GetAllPrimesAsync which immediately returns
        /// 2) go ahead and finish this function by returning a Task<int[]>
        /// 3) asp container will finish the Task and return result to caller
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        [HttpGet, ActionName("primestask")]
        public Task<int[]> GetAllPrimesTask(int id /*n*/)
        {
            int n = id;
            Global.Log.Write("[" + Thread.CurrentThread.ManagedThreadId + "] before calling GetAllPrimesAsync(int)");
            Prime primer = new Prime();
            var tt = primer.GetAllPrimesAsync(n);
            Global.Log.Write("[" + Thread.CurrentThread.ManagedThreadId + "] after calling GetAllPrimesAsync(int) task");
            return tt;
        }
    }
}

