using AutoMapper;
using CollectorQi.Models.ESCL018;
using CollectorQi.Models.ESCL021;
using CollectorQi.Models.ESCL028;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Services.ESCL018;
using CollectorQi.Services.ESCL028;
using CollectorQi.ViewModels;
using CollectorQi.VO.ESCL018;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
//using Android.Graphics;

namespace CollectorQi.Views
{
    public partial class NotaFiscalConferenciaReparosListaPopUp : PopupPage
    {
        public Action _refreshListView { get; set; }
        private DepositosGuardaMaterial _depositosGuardaMaterial { get; set; }

        public NotaFiscalConferenciaReparosListaPopUp()        
        {
            try
            {
                InitializeComponent();

              //_depositosGuardaMaterial = pDepositosGuardaMaterial;
              //
              //if (!String.IsNullOrEmpty(pLocalizacao))
              //{
              //    edtLocalizacao.Text = pLocalizacao;
              //}
            }
            catch (Exception ex)
            {
                throw;
            } 
        }

        protected async override void OnAppearing()
        {
            await Task.Run(async () =>
            {
                await Task.Delay(100);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    edtCodigoBarras.Focus();
                });
            });
        }
        void OnClick_Cancelar(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }

        protected override bool OnBackButtonPressed()
        {            
            PopupNavigation.Instance.PopAsync();
            return true;
        }

        async void OnClick_Confirmar(object sender, EventArgs e)
        {
           var pageProgress = new ProgressBarPopUp("Efetivando Reparo, aguarde...");
            BtnEfetivar.IsEnabled = false;

            try
            {
                if (String.IsNullOrEmpty(edtCodigoBarras.Text))
                {
                    await DisplayAlert("Erro Leitura da Etiqueta", "Nenhuma etiqueta efetuada a leitura, favor realizar a leitura da etiqueta para seguir com a atualização da nota fiscal.", "OK");
                    return;
                }

                string codEstabOrigem = edtCodigoBarras.Text.Substring(0, 3);
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                var dRetornoNota = await ValidarReparosNotaFiscalService.SendValidarReparosAsync(new ValidarReparosNotaFiscal
                {
                    CodBarras = edtCodigoBarras.Text

                }, codEstabOrigem);

                //  await pageProgress.OnClose();
                if (dRetornoNota != null && dRetornoNota.Resultparam != null && dRetornoNota.Resultparam.Count > 0)
                {
                    var v = dRetornoNota.Resultparam.FirstOrDefault();

                    if (v.Mensagem.Contains("ERRO:"))
                    {
                        await DisplayAlert("ERRO!", v.Mensagem, "OK");
                        return;
                    }
                    else
                    {
                        await pageProgress.OnClose();
                        OnBackButtonPressed();
                        _refreshListView();
                    }
                }
            }
            catch (Exception ex)
            {
                // await pageProgress.OnClose();
                await DisplayAlert("ERRO", ex.Message, "OK");
            }
            finally
            {
                BtnEfetivar.IsEnabled = true;
                await pageProgress.OnClose();
            }
        }
    }
}