using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyCSharpService.Models;

namespace MyCSharpService.Services
{
    public class ContactRepository
    {
        private const string CacheKey = "ContactStore";
        public ContactRepository()
        {
            var ctx = HttpContext.Current;
            if (null != ctx && null == ctx.Cache[CacheKey])
            {
                ctx.Cache[CacheKey] = new Contact[]
                {
                    new Contact
                    {
                        Id = 1,
                        Name = "Glenn Block",
                    },
                    new Contact
                    {
                        Id = 2,
                        Name = "Dan Roth",
                    },
                };
            }
        }
        public Contact[] GetAllContacts()
        {
            var ctx = HttpContext.Current;
            if (null != ctx)
            {
                return (Contact[])ctx.Cache[CacheKey];
            }
            return new Contact[]
            {
                new Contact
                {
                    Id = 0,
                    Name = "Not Ready",
                },
            };
        }
        public bool SaveContact(Contact contact)
        {
            var ctx = HttpContext.Current;
            if (null != ctx && null != contact && contact.Id > 0)
            {
                var contacts = ((Contact[])ctx.Cache[CacheKey]).ToList();
                if (!contacts.Any(ele => ele.Id.CompareTo(contact.Id) == 0))
                {
                    contacts.Add(contact);
                    ctx.Cache[CacheKey] = contacts.ToArray();
                    return true;
                }
            }
            return false;
        }
    }
}