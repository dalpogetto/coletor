using AutoMapper;
using CollectorQi.Models.ESCL018;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper.ESCL018;
using CollectorQi.Services.ESCL018;
using CollectorQi.VO.ESCL018;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
//using Android.Graphics;

namespace CollectorQi.Views
{
    public partial class GuardaMateriasConfirmaQuantidadePopUp : PopupPage
    {
        public Action<string, bool, string> _actConfirmaGuardaMateriais;

        public GuardaMateriasConfirmaQuantidadePopUp( string pCodDepos, string pCodLocalizacao, string pCodItem, string pTipoTransacao, string pCodBarras /* int pInventarioId, InventarioItemVO pInventarioItem */ )        
        {
            try
            {
                InitializeComponent();

                edtDeposito.Text      = pCodDepos;
                edtLocalizacao.Text   = pCodLocalizacao;
                edtItem.Text          = pCodItem;
                edtTipoTransacao.Text = pTipoTransacao;

                if (!String.IsNullOrEmpty(pCodBarras))
                {
                    edtCodigoBarras.Text = pCodBarras;
                }
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
                    if (String.IsNullOrEmpty(edtCodigoBarras.Text))
                    {
                        edtCodigoBarras.Focus();
                    }
                    else
                    {
                        txtQuantidade.Focus();
                    }
                });
            });
        }
       
        void BtnLimpar_Clicked(object sender, EventArgs e)
        {
            txtQuantidade.Text = string.Empty;
            edtCodigoBarras.Text = string.Empty;
        }
        protected override bool OnBackButtonPressed()
        {            
            PopupNavigation.Instance.PopAsync();
            return true;
        }

        private void BtnQuantidadeSum1_Clicked(object sender, EventArgs e)
        {
            SumButton(txtQuantidade, 1);
        }
        private void BtnQuantidadeSum10_Clicked(object sender, EventArgs e)
        {
            SumButton(txtQuantidade, 10);
        }
        private void BtnQuantidadeSum100_Clicked(object sender, EventArgs e)
        {
            SumButton(txtQuantidade, 100);
        }

        private void SumButton(CustomEntry entry, int value)
        {
            if (String.IsNullOrEmpty(entry.Text)) { entry.Text = "0"; }

            entry.Text = (int.Parse(entry.Text) + value).ToString();
        }

        public async void PopUpClose()
        {
            await PopupNavigation.Instance.PopAsync();
        }

        async void OnClick_Efetivar(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtQuantidade.Text))
            {
                await DisplayAlert("Erro!", "Informe a quantidade do produto disponivel para a contagem do inventário", "Cancelar");
                return;
            }


            if (string.IsNullOrEmpty(edtCodigoBarras.Text))
            {
                await DisplayAlert("Erro!", "Informe o código de barras", "Cancelar");
                return;
            }

            var result = await DisplayAlert("Confirmação!", $"Deseja concluír a digitação com a quantidade de {txtQuantidade.Text} produto?", "Sim", "Não");

            var pageProgress = new ProgressBarPopUp("Carregando...");
            try
            {
                BtnEfetivar.IsEnabled = false;
                if (result.ToString() == "True")
                {
                    _actConfirmaGuardaMateriais(edtCodigoBarras.Text, true, txtQuantidade.Text) ;
                    await pageProgress.OnClose();
                    OnBackButtonPressed();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                BtnEfetivar.IsEnabled = true;
                await pageProgress.OnClose();
            }
        }
    }
}