using CollectorQi.Resources;
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
        public async Task<ValidaAplicativoInfo> ExisteNovaVersao()
        {
            var novaVersao = false;
            
            /*var readPhoneState = DependencyService
                .Get<IReadPhoneState>()
                .GetPhoneIMEI();

            if (string.IsNullOrEmpty(readPhoneState)) return false;
            */

            var modelVersao = new ValidaAplicativoRequest
            {
                Modelo = $"{DeviceInfo.Manufacturer}-{DeviceInfo.Model}",
                Versao = VersionTracking.CurrentVersion,
                VersaoAndroid = DeviceInfo.Version.ToString(),
                Imei = "" //readPhoneState
            };

            var controleVersao = new ControleVersaoService();
            var resultData = await controleVersao.ValidaAplicativoAsync(modelVersao);

            if (resultData != null && resultData.ListaEmpresas != null)
            {
                SecurityAuxiliar.EmpresaAll = resultData.ListaEmpresas.ToList().Where(x => x.codEmpresa != "0").ToList();
            }
            else
            {
                SecurityAuxiliar.EmpresaAll = null;
            }

            /*
            if (resultData == null)
            {
                throw new Exception("401");
            }*/

            if (resultData != null)
            {

                if (resultData.Retorno != null && resultData.Retorno == "Error")
                {
                    throw new Exception("Versão nao cadastrada");
                }
                else
                {
                    var rVersao = resultData.APKInfo.FirstOrDefault();

                    System.Diagnostics.Debug.Write(resultData);
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
                            return rVersao;
                           // await Launcher.OpenAsync(new Uri(rVersao.LinkVersao));
                           // novaVersao = true;
                        }

                        if (rVersao.Versao != VersionTracking.CurrentVersion)
                        {
                            //var result = await DisplayAlert("Alerta",
                            //     "Existe uma nova versão disponivel. Deseja atualizar agora?",
                            //     "Sim", "Não");

                            var result = true;

                            if (result)
                            {
                           //     await Launcher.OpenAsync(new Uri(rVersao.LinkVersao));
                           //     novaVersao = true;
                            }
                        }
                    }
                    return rVersao;
                }
                return null;
            }
            return null;
        }
    }
}