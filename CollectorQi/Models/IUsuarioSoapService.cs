using System.Collections.Generic;
using System.Threading.Tasks;

namespace CollectorQi.Models
{
    public interface IUsuarioSoapService
    {
        Task<List<IUsuario>> GetAllCustomers(string criteria = null);
    }
}