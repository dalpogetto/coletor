using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectorQi.Models
{
    public interface IUsuarioSoapService
    {
        Task<List<IUsuario>> GetAllCustomers(string criteria = null);
    }
}