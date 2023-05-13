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
    public partial class GerarPedidoPesoVolumePopUp : PopupPage
    {
        private GerarPedidoEmitente                        _emitente;
        private GerarPedidoTecnico                         _tecnico;
        private ObservableCollection<GerarPedidoViewModel> _items;
        public Action _refreshRow;

        public GerarPedidoPesoVolumePopUp(ObservableCollection<GerarPedidoViewModel> pItems, GerarPedidoEmitente pEmitente, GerarPedidoTecnico pTecnico)        
        {
            try
            {
                InitializeComponent();

                _emitente = pEmitente;
                _tecnico  = pTecnico;
                _items    = pItems;
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
                    edtVolume.Focus();
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

        public async void PopUpClose()
        {
            await PopupNavigation.Instance.PopAsync();
        }

        public async Task EfetivarPedido()
        {
            var pageProgress = new ProgressBarPopUp("Gerando pedidos...");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                var result = true;

                if (result.ToString() == "True")
                {
                    var lstReparos = new List<GerarPedidoService.GerarPedidoListaReparos>();

                    foreach (var row in _items)
                    {
                        lstReparos.Add(new GerarPedidoService.GerarPedidoListaReparos
                        {
                            RowId = row.RowId,
                            VlOrcado = row.VlOrcado,
                            Serie = row.Serie
                        });
                    }

                    var param = new GerarPedidoService.GerarPedidoParam
                    {
                        CodEstabel    = SecurityAuxiliar.GetCodEstabel(),
                        CodTecnico    = _tecnico.CodTecnico,
                        CodDepos      = _tecnico.CodDepos,
                        CodEmitente   = _emitente.CodEmitente,
                        CodTransporte = "1",
                        Peso          = edtPeso.Text,
                        Volumes       = edtVolume.Text
                    };

                    var resultService = await GerarPedidoService.GerarPedido(param, lstReparos);

                    if (resultService != null && resultService.Retorno != null)
                    {
                        if (resultService.Retorno == "OK")
                        {
                            string strPedcli = "";

                            if (resultService.ConteudoPedido != null && resultService.ConteudoPedido != null)
                                strPedcli = resultService.ConteudoPedido.Pedido.NrPedCli;

                            await DisplayAlert("Reparos/Pedidos", "Pedido " + strPedcli + " Gerado com sucesso", "OK");

                            _refreshRow();

                            await pageProgress.OnClose();
                            await PopupNavigation.Instance.PopAsync();
                        }
                        else if (resultService.Resultparam != null && resultService.Resultparam.Count > 0)
                        {
                            await DisplayAlert("Erro!", resultService.Resultparam[0].ErrorDescription + " - " + resultService.Resultparam[0].ErrorHelp, "Cancelar");
                        }
                        else
                        {
                            await DisplayAlert("Erro!", "Erro na confirmação do reparo", "Cancelar");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
            finally
            {
                await pageProgress.OnClose();
            }
        }

        async void OnClick_Confirmar(object sender, EventArgs e)
        {
            try
            {
                BtnEfetivar.IsEnabled = false;

                await EfetivarPedido();
            }
            finally
            {
                BtnEfetivar.IsEnabled = true;
            }
        }

        private void edtLocalizacao_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(e.OldTextValue) && e.NewTextValue.Length > 5)
                {
                    BtnEfetivar.IsEnabled = false;

                    PopupNavigation.Instance.PopAsync();
                }
            }
            finally
            {
                BtnEfetivar.IsEnabled = true;
            }
        }
    }
}