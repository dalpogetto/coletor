using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectorQi.Models
{
    public interface ICallNotification
    {
        bool callNotification(eTpNotificacao byTpNotificacao, string byStrMensagem, bool byErro = false);
    }
}
