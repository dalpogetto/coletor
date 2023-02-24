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
using CollectorQi.Services.ESCL000;
//using Android.Graphics;

namespace CollectorQi.Views
{
    public partial class GerarPedidoEmitentePopUp : PopupPage
    {
        public Action<GerarPedidoEmitente> _confirmaEmitente;

        public GerarPedidoEmitentePopUp(GerarPedidoEmitente pEmitente)        
        {
            try
            {
                InitializeComponent();

                if (pEmitente != null)
                {
                    edtCodEmitente.Text = pEmitente.CodEmitente.ToString();
                    edtNomeAbrev.Text = pEmitente.NomeAbrev;
                    edtNomeEmitente.Text = pEmitente.Nome;
                    edtCidadeEmitente.Text = pEmitente.Cidade;
                    edtEstadoEmitente.Text = pEmitente.Estado;
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

                    edtCodEmitente.Focus();
                    /* if (String.IsNullOrEmpty(edtCodigoBarras.Text))
                    {
                        edtCodigoBarras.Focus();
                    }
                    else
                    {
                        txtQuantidade.Focus();
                    }
                    */
                });
            });
        }
       
        void BtnLimpar_Clicked(object sender, EventArgs e)
        {
            Limpar();

            edtCodEmitente.Text = String.Empty;
            edtCodEmitente.Focus();
        }

        public void Limpar()
        {
            edtNomeAbrev.Text      = string.Empty;
            edtNomeEmitente.Text   = string.Empty;
            edtEndereco.Text       = string.Empty;
            edtEstadoEmitente.Text = string.Empty;
            edtCidadeEmitente.Text = string.Empty;
        }

        protected override bool OnBackButtonPressed()
        {            
            PopupNavigation.Instance.PopAsync();
            return true;
        }

        private void BtnQuantidadeSum1_Clicked(object sender, EventArgs e)
        {
            //SumButton(txtQuantidade, 1);
        }
        private void BtnQuantidadeSum10_Clicked(object sender, EventArgs e)
        {
            //SumButton(txtQuantidade, 10);
        }
        private void BtnQuantidadeSum100_Clicked(object sender, EventArgs e)
        {
            //SumButton(txtQuantidade, 100);
        }

        private void SumButton(CustomEntry entry, int value)
        {
            /*
            if (String.IsNullOrEmpty(entry.Text)) { entry.Text = "0"; }

            entry.Text = (int.Parse(entry.Text) + value).ToString();
            */
        }

        public async void PopUpClose()
        {
            await PopupNavigation.Instance.PopAsync();
        }

        async void OnClick_Efetivar(object sender, EventArgs e)
        {
            try
            {
                BtnEfetivar.IsEnabled = false;

                _confirmaEmitente(new GerarPedidoEmitente
                {
                    CodEmitente = int.Parse(edtCodEmitente.Text),
                    Nome        = edtNomeEmitente.Text,
                    NomeAbrev   = edtNomeAbrev.Text,
                    Endereco    = edtEndereco.Text,
                    Cidade      = edtCidadeEmitente.Text,
                    Estado      = edtEstadoEmitente.Text
                });

                OnBackButtonPressed();

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                BtnEfetivar.IsEnabled = true;
            }
        }

        private void lblEmitente_Unfocused(object sender, FocusEventArgs e)
        {

        }

        private async void edtCodEmitente_Unfocused(object sender, FocusEventArgs e)
        {
            var pageProgress = new ProgressBarPopUp("Carregando...");

            try
            {
                await PopupNavigation.Instance.PushAsync(pageProgress);

                var emitente = await ObterEmitenteService.ObterEmitente(int.Parse(edtCodEmitente.Text));

                if (emitente != null)
                {
                    edtNomeAbrev.Text      = emitente.NomeAbrev;
                    edtNomeEmitente.Text   = emitente.Nome;
                    edtEndereco.Text       = emitente.Endereco;
                    edtEstadoEmitente.Text = emitente.Estado;
                    edtCidadeEmitente.Text = emitente.Cidade;
                    edtCodTransp.Text      = emitente.CodTransp;
                    edtNomeTransp.Text     = emitente.NomeTransp;
                }
                else
                {
                    Limpar();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("ERRO", ex.Message, "OK");
            }
            finally
            {
                await pageProgress.OnClose();
            }
        }

        private void edtCodTransp_Unfocused(object sender, FocusEventArgs e)
        {

        }
    }
}