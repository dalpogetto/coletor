using CollectorQi.Services.ESCL000;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace CollectorQi.Services
{
    public class ValidaVersaoSistema : ContentPage
    {
        public async Task<bool> ExisteNovaVersao()
        {
            var novaVersao = false;
            var readPhoneState = DependencyService
                .Get<IReadPhoneState>()
                .GetPhoneIMEI();

            if (string.IsNullOrEmpty(readPhoneState)) return false;

            var modelVersao = new ValidaAplicativoRequest
            {
                Modelo = $"{DeviceInfo.Manufacturer}-{DeviceInfo.Model}",
                Versao = VersionTracking.CurrentVersion,
                VersaoAndroid = DeviceInfo.Version.ToString(),
                Imei = readPhoneState
            };

            var controleVersao = new ControleVersaoService();
            var resultData = await controleVersao.ValidaAplicativoAsync(modelVersao);

            if (resultData != null)
            {
                var rVersao = resultData.APKInfo.FirstOrDefault();
                if (rVersao != null)
                {
                    var isNewVersion = !rVersao.VersaoMobile.Equals(modelVersao.Versao);
                    var isValida = isNewVersion && rVersao.LoginValidado;
                    if (isValida)
                    {
                        var result = await DisplayAlert("Alerta",
                            "Existe uma nova versão disponivel. Deseja atualizar agora?",
                            "Sim", "Não");

                        if (result)
                        {
                            await Launcher.OpenAsync(new Uri(rVersao.LinkVersao));
                            novaVersao = true;
                        }
                    }

                    if (!rVersao.LoginValidado)
                    {
                        await Launcher.OpenAsync(new Uri(rVersao.LinkVersao));
                        novaVersao = true;
                    }
                }
            }

            return novaVersao;
        }
    }
}