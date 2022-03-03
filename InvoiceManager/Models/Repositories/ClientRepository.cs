using InvoiceManager.Models.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InvoiceManager.Models.Repositories
{
    public class ClientRepository
    {
        public List<Client> GetClients(string userId)
        {
            using(var dbContext = new ApplicationDbContext())
            {
                return dbContext.Clients.Where(x => x.UserId == userId).ToList();
            }
        }
    }
}