using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollectorQi.Droid
{
    public interface ICustomer
    {
        string Name { get; set; }
        string Id { get; set; }
    }

    public interface ICustomerSoapService
    {
        Task<List<ICustomer>> GetAllCustomers(string criteria = null);
    }
}
