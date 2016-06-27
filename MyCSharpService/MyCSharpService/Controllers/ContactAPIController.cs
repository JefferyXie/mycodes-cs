using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MyCSharpService.Models;
using MyCSharpService.Services;

namespace MyCSharpService.Controllers
{
    public class ContactAPIController : ApiController
    {
        private ContactRepository contactRepository;
        public ContactAPIController()
        {
            this.contactRepository = new ContactRepository();
        }
        public Contact[] Get()
        {
            return this.contactRepository.GetAllContacts();
        }

        public HttpResponseMessage Post(Contact contact)
        {
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
    }
}
