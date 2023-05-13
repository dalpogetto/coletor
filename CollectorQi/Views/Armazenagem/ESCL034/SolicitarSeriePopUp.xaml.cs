using AutoMapper;
using CollectorQi.Models.ESCL018;
using CollectorQi.Models.ESCL021;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Services.ESCL018;
using CollectorQi.Services.ESCL034;
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
    public partial class SolicitarSeriePopUp : PopupPage
    {
        private ObservableCollection<GerarPedidoViewModel> _items;
        private string _rowIdReparoCorrente;

        public Action _refreshRow;

        public SolicitarSeriePopUp(ObservableCollection<GerarPedidoViewModel> pItems, string pRowId)        
        {
            try
            {
                InitializeComponent();

                _items    = pItems;
                _rowIdReparoCorrente = pRowId;
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
                await Task.Delay(300);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    //edtVolume.Focus();
                });
            });
        }
        void BtnSairSerie_Clicked(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }

        protected override bool OnBackButtonPressed()
        {            
            PopupNavigation.Instance.PopAsync();
            return true;
        }

        public async void PopUpClose()
        {
            await PopupNavigation.Instance.PopAsync();
        }

        async void SearchSerie_Unfocused(object sender, FocusEventArgs e)
        {
            if (String.IsNullOrEmpty(SearchSerie.Text)) { return; }

            //Formatar serie
            if (SearchSerie.Text.Length < 12)
                SearchSerie.Text = SearchSerie.Text.PadLeft(12, '0');


            var pageProgress = new ProgressBarPopUp("Validando Número de Série, aguarde...");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                //Chamar o Servico Validar Reparo
                var resultService = await ValidarSerieReparoService.ValidarReparo(_rowIdReparoCorrente, SearchSerie.Text);
                if (resultService.Retorno == "OK")
                {

                    if (string.IsNullOrEmpty(resultService.Conteudo.Mensagem))
                    {

                        //Atualizar o Item com Informações da Serie
                        //E popular o registro no Grid
                        var registroCorrente = _items.Last(x => x.RowId == _rowIdReparoCorrente);
                        _items.Remove(registroCorrente);

                        registroCorrente.Serie = SearchSerie.Text;
                        _items.Add(registroCorrente);
                    }
                    else
                    {
                        await DisplayAlert("Erro!", resultService.Conteudo.Mensagem, "OK");
                        SearchSerie.Text = string.Empty;
                        // var registroCorrente = Items.Last(x => x.RowId == _RowIdReparoCorrente);
                        // Items.Remove(registroCorrente);

                    }
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancel");

            }
            finally
            {

                await pageProgress.OnClose();
            }
        }

    }
}