namespace CollectorQi.Models
{
    public interface ICallNotification
    {
        bool callNotification(eTpNotificacao byTpNotificacao, string byStrMensagem, bool byErro = false);
    }
}
