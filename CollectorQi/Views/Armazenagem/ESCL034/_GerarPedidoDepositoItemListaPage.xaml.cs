using AutoMapper;
using CollectorQi.Models.ESCL021;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL021;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GerarPedidoDepositoItemListaPage : ContentPage, INotifyPropertyChanged
    {  
        public ObservableCollection<DepositosGuardaMaterialItemViewModel> ObsGuardaMateriaisDepositoItem { get; set; }
        public List<DepositosGuardaMaterialItem> ListaDepositosGuardaMaterialItem { get; set; }
        public string _local { get; set; }
        public string _localCodigoBarras { get; set; }
        public string _codDepos { get; set; }
        public int? _tipoMovimento { get; set; }
        public int SemSaldo { get; set; }

        private List<DepositosGuardaMaterialItem> _listaDepositosGuardaMaterialItem;
        private string _codigoBarrasLocalizacao;

        private DepositosGuardaMaterial _depositoGuardaMaterial;

        public GerarPedidoDepositoItemListaPage(List<DepositosGuardaMaterialItem> listaDepositosGuardaMaterialItem, string local, string codDepos, int? tipoMovimento, DepositosGuardaMaterial pDepositoGuardaMaterial)
        {
            InitializeComponent();

            _codigoBarrasLocalizacao = local;

            if (local.Length > 0)
            {
                _localCodigoBarras = local;
                local              = local.Replace("10[", "").Replace("10;", "").Replace("10]", "");
            }

            BtnTipoMovimento.Text = "Depósito: " + codDepos + "   /   Localização: " + local;

            ObsGuardaMateriaisDepositoItem    = new ObservableCollection<DepositosGuardaMaterialItemViewModel>();
            _local                            = local;
            _tipoMovimento                    = tipoMovimento;
            _codDepos                         = codDepos;
            _listaDepositosGuardaMaterialItem = listaDepositosGuardaMaterialItem;
            _depositoGuardaMaterial = pDepositoGuardaMaterial;

            cvGuardaMateriaisDepositoItem.BindingContext = this;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new GuardaMateriaisDepositoListaPage(_codDepos));

            return true;
        }

        private async void CarregaListViewPopUp(string pLocalizacao)
        {
            var pageProgress = new ProgressBarPopUp("Carregando Itens, aguarde...");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                _codigoBarrasLocalizacao = pLocalizacao;

                if (pLocalizacao.Length > 0)
                {
                    _localCodigoBarras = pLocalizacao;
                    pLocalizacao = pLocalizacao.Replace("10[", "").Replace("10;", "").Replace("10]", "");
                }

                BtnTipoMovimento.Text = "Depósito: " + _codDepos + "   /   Localização: " + pLocalizacao;

                _local = pLocalizacao;

                var dLeituraEtiqueta = new LeituraEtiquetaLocalizaGuardaMaterialService();
                var dadosLeituraLocalizaGuardaMaterial = new DadosLeituraLocalizaGuardaMaterial()
                { CodEstabel = SecurityAuxiliar.GetCodEstabel(), CodigoBarras = _localCodigoBarras };

                // recarrega a lista da API
                var dadosLeituraItemGuardaMaterial = new DadosLeituraItemGuardaMaterial()
                {
                    CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                    CodDepos = _codDepos,
                    CodigoBarras = _localCodigoBarras
                };

                dadosLeituraItemGuardaMaterial.CodLocaliza = _codigoBarrasLocalizacao;
                dadosLeituraItemGuardaMaterial.Transacao = 1;

                if ((SwtSaldo).On)
                {
                    dadosLeituraItemGuardaMaterial.SemSaldo = 1;
                }
                else
                {
                    dadosLeituraItemGuardaMaterial.SemSaldo = 0;
                }

                var dRetorno = await LeituraEtiquetaLerLocalizaGuardaMaterialService.SendLeituraEtiquetaAsync(dadosLeituraItemGuardaMaterial);

                ObsGuardaMateriaisDepositoItem.Clear();

                if (dRetorno.Retorno.Contains("Error"))
                {
                    await DisplayAlert("ERRO", dRetorno.Resultparam[0].ErrorHelp, "OK");
                    OnPropertyChanged("ObsGuardaMateriaisDepositoItem");
                }
                else
                {
                    // Chamar a API novamente passando sem saldo 0 
                    if (dRetorno != null && dRetorno.paramRetorno != null)
                    {
                        foreach (var item in dRetorno.paramRetorno)
                        {
                            var modelView = Mapper.Map<DepositosGuardaMaterialItem, DepositosGuardaMaterialItemViewModel>(item);
                            ObsGuardaMateriaisDepositoItem.Add(modelView);
                        }
                    }
                    OnPropertyChanged("ObsGuardaMateriaisDepositoItem");
                }

                if (_pagePopUpLocaliz != null)
                {
                    _pagePopUpLocaliz.PopUpClose();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                await pageProgress.OnClose();
            }

            //CarregaListView();
        }

        private async void CarregaListView()
        {
            var pageProgress = new ProgressBarPopUp("Carregando Itens, aguarde...");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);
                
                ObsGuardaMateriaisDepositoItem.Clear();

                if (_listaDepositosGuardaMaterialItem != null)
                {
                    // Retirar .Where(x => x.SaldoInfo != 0)
                    foreach (var item in _listaDepositosGuardaMaterialItem.Where(x => x.SaldoInfo != 0))
                    {
                        var modelView = Mapper.Map<DepositosGuardaMaterialItem, DepositosGuardaMaterialItemViewModel>(item);
                        ObsGuardaMateriaisDepositoItem.Add(modelView);
                    }

                    ListaDepositosGuardaMaterialItem = _listaDepositosGuardaMaterialItem;
                }
                else
                    ListaDepositosGuardaMaterialItem = new List<DepositosGuardaMaterialItem>();

                OnPropertyChanged("ObsGuardaMateriaisDepositoItem");

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                await pageProgress.OnClose();
            }

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            CarregaListView();

        }
        private void BtnVoltarLeituraEtiqueta_Clicked(object sender, System.EventArgs e)
        {
            OnBackButtonPressed();
        }
        public async void CodigoBarras(string pCodigoBarras, bool origemPopUp, string pQuantidade)
        {
            var pageProgress = new ProgressBarPopUp("Efetivando Item, aguarde...");

            try
            {

                // Solicitou informar a quantidade e digita a caixa 
                if (SwtQuantidade.On && !origemPopUp)
                {
                    string strItCodigo = "";                    
                    
                    if (pCodigoBarras.Contains(";"))
                    {
                        strItCodigo = pCodigoBarras.Split(';')[1];
                    }
                    else
                    {
                        strItCodigo = pCodigoBarras;
                    }

                    var item = ObsGuardaMateriaisDepositoItem.FirstOrDefault(x => x.CodigoItem == strItCodigo);

                    if (item != null)
                    {
                        await OpenPopUpQuantidade(item, pCodigoBarras);
                        return;
                    }
                    else
                    {
                        await DisplayAlert("Erro", "Item não encontrado", "OK");
                    }
                }
                
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                ObsGuardaMateriaisDepositoItem  = new ObservableCollection<DepositosGuardaMaterialItemViewModel>();
                var depositosGuardaMaterialItem = new DepositosGuardaMaterialItem();

                // altera quantidade no codigo de barras
                if (SwtQuantidade.On && pQuantidade != null)
                {
                    /*
                    var splCodigoBarras = pCodigoBarras.Split(';');

                    /*
                    if (splCodigoBarras != null)
                    {
                        splCodigoBarras[3] = pQuantidade.ToString();
                    }
                    


                    pCodigoBarras = "";
                    for (int i = 0; i < splCodigoBarras.Length; i++)
                    {
                        if (pCodigoBarras == "")
                        {
                            pCodigoBarras = splCodigoBarras[i];
                        }
                        else
                        {
                            pCodigoBarras = pCodigoBarras + ';' + splCodigoBarras[i];
                        }
                    }
                    */

                    
                }
                else
                {
                    pQuantidade = "-1";
                }

                var dadosLeituraItemGuardaMaterial = new DadosLeituraItemGuardaMaterial()
                {
                    CodEstabel   = SecurityAuxiliar.GetCodEstabel(),
                    CodDepos     = _codDepos,
                    CodLocaliza  = _local,
                    Transacao    = _tipoMovimento,
                    SemSaldo     = SemSaldo,
                    CodigoBarras = CleanInput(pCodigoBarras),
                    Quantidade   = pQuantidade,
                    
                };
            
                var resultService = await LeituraEtiquetaGuardaMaterialService.SendLeituraEtiquetaAsync(dadosLeituraItemGuardaMaterial);

                if (resultService != null && resultService.Retorno != null)
                {
                    if (resultService.Retorno == "OK")
                    {
                        await DisplayAlert("Guarda de Materiais", "Guarda de Materias efetivada com sucesso", "OK");

                        // Envia o codigo de barras da localizacao para buscar os itens da localizacao
                        dadosLeituraItemGuardaMaterial.CodigoBarras = _codigoBarrasLocalizacao;

                        // Listando guarda de materias webservice;
                        var dRetorno = await LeituraEtiquetaLerLocalizaGuardaMaterialService.SendLeituraEtiquetaAsync(dadosLeituraItemGuardaMaterial);

                        System.Diagnostics.Debug.Write(dRetorno);

                        if (dRetorno.Retorno.Contains("Error"))
                        {
                            await DisplayAlert("ERRO", "Lista guarda de materias " + dRetorno.Resultparam[0].ErrorHelp, "OK");
                        }
                        else
                        {
                            if (_pagePopUp != null)
                            {
                               _pagePopUp.PopUpClose();
                                // await PopupNavigation.Instance.PushAsync(_pagePopUp);
                            }
                            _listaDepositosGuardaMaterialItem = dRetorno.paramRetorno;
                            CarregaListView();
                        }
                    }
                    else if (resultService.Retorno == "IntegracaoBatch")
                    {
                        // await DisplayAlert("Atenção!", "Erro de conexão com ERP, a atualização do item será integrado de forma Offline", "OK");
                        //
                        // _inventarioItemVO.StatusIntegracao = eStatusInventarioItem.ErroIntegracao;
                        // _actRefreshPage(_inventarioItemVO);
                        //
                        // await pageProgress.OnClose();
                        // OnBackButtonPressed();
                    }
                    else
                    {
                        if (resultService.Resultparam != null && resultService.Resultparam.Count > 0)
                        {
                            await DisplayAlert("Erro!", resultService.Resultparam[0].ErrorDescription + " - " + resultService.Resultparam[0].ErrorHelp, "Cancelar");
                        }
                        else
                        {
                            await DisplayAlert("Erro!", "Erro na efetivação do inventário", "Cancelar");
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Erro!", "Erro na efetivação do inventário", "Cancelar");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                await pageProgress.OnClose();
            }
        }

        async void BtnEfetivarItem_Clicked(object sender, System.EventArgs e)
        {
            BtnEfetivarItem.IsEnabled = false;
            try
            {
                cvGuardaMateriaisDepositoItem.IsEnabled = false;

                var page = new GuardaMateriaisDepositoItemListaPagePopUp();
                page._confirmaItem = CodigoBarras;
                await PopupNavigation.Instance.PushAsync(page);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "OK");
            }
            finally
            {
                BtnEfetivarItem.IsEnabled = true;
            }
        }

        private void SwitchCellEntradaSaida_OnChanged(object sender, ToggledEventArgs e)
        {
            if (((SwitchCell)sender).On)
            {
                SwtEntradaSaida.Text = "Saída";
                _tipoMovimento = 2;
            }
            else
            {
                SwtEntradaSaida.Text = "Entrada";
                _tipoMovimento = 1;
            }
        }

        private void SwitchCellQuantidade_OnChanged(object sender, ToggledEventArgs e)
        {
            if (!((SwitchCell)sender).On)
            {
                SwtQuantidade.Text = "Quantidade Caixa";
            }
            else
            {
                SwtQuantidade.Text = "Informar a quantidade";
            }
        }

        private async void SwitchCell_OnChanged(object sender, ToggledEventArgs e)
        {

            SwtSaldo.IsEnabled = false;
            var pageProgress = new ProgressBarPopUp("Carregando Itens, aguarde...");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                var dLeituraEtiqueta = new LeituraEtiquetaLocalizaGuardaMaterialService();
                var dadosLeituraLocalizaGuardaMaterial = new DadosLeituraLocalizaGuardaMaterial()
                { CodEstabel = SecurityAuxiliar.GetCodEstabel(), CodigoBarras = _localCodigoBarras };

                // recarrega a lista da API
                var dadosLeituraItemGuardaMaterial = new DadosLeituraItemGuardaMaterial()
                {
                    CodEstabel   = SecurityAuxiliar.GetCodEstabel(),
                    CodDepos     = _codDepos,
                    CodigoBarras = _localCodigoBarras
                };

                dadosLeituraItemGuardaMaterial.CodLocaliza = _codigoBarrasLocalizacao;
                dadosLeituraItemGuardaMaterial.Transacao = 1;

                if (((SwitchCell)sender).On)
                {
                    dadosLeituraItemGuardaMaterial.SemSaldo = 1;
                }
                else
                {
                    dadosLeituraItemGuardaMaterial.SemSaldo = 0;
                }

                var dRetorno = await LeituraEtiquetaLerLocalizaGuardaMaterialService.SendLeituraEtiquetaAsync(dadosLeituraItemGuardaMaterial);

                if (dRetorno.Retorno.Contains("Error"))
                {
                    await DisplayAlert("ERRO", dRetorno.Resultparam[0].ErrorHelp, "OK");
                }
                else
                {

                    ObsGuardaMateriaisDepositoItem.Clear();

                    // Chamar a API novamente passando sem saldo 0 
                    if (dRetorno != null && dRetorno.paramRetorno != null)
                    {
                        foreach (var item in dRetorno.paramRetorno)
                        {
                            var modelView = Mapper.Map<DepositosGuardaMaterialItem, DepositosGuardaMaterialItemViewModel>(item);
                            ObsGuardaMateriaisDepositoItem.Add(modelView);
                        }
                    }

                    OnPropertyChanged("ObsGuardaMateriaisDepositoItem");

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("ERRO!", ex.Message, "OK");
            }
            finally
            {
                await pageProgress.OnClose();
                SwtSaldo.IsEnabled = true;
            }
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                ToolBarPrint.IsEnabled = false;

                var pageProgress = new ProgressBarPopUp("Carregando...");
                var page = new ArmazenagemPrintPopUp(null, null);
                await PopupNavigation.Instance.PushAsync(page);
                await pageProgress.OnClose();
            }
            finally
            {
                ToolBarPrint.IsEnabled = true;
            }
        }

        private async void cvGuardaMateriaisDepositoItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cvGuardaMateriaisDepositoItem.IsEnabled = false;

            if (cvGuardaMateriaisDepositoItem.SelectedItem == null)
                return;

            cvGuardaMateriaisDepositoItem.SelectedItem = null;

            var current = (cvGuardaMateriaisDepositoItem.SelectedItem as DepositosGuardaMaterialItemViewModel);

            SwtQuantidade.On = true;

            await OpenPopUpQuantidade(current, "");
        }

        private async Task OpenPopUpQuantidade(DepositosGuardaMaterialItemViewModel current, string pCodBarras)
        {
            try
            {
                /*
                cvGuardaMateriaisDepositoItem.IsEnabled = false;

                if (cvGuardaMateriaisDepositoItem.SelectedItem == null)
                    return;

                cvGuardaMateriaisDepositoItem.SelectedItem = null;

                var current = (cvGuardaMateriaisDepositoItem.SelectedItem as DepositosGuardaMaterialItemViewModel);
                */

                // Saida
                if (current.SaldoInfo <= 0 && SwtEntradaSaida.On)
                {
                    await DisplayAlert("ERRO!", "Saída de Materias não disponivel para itens com quantidade (0).", "OK");
                    return;
                }

                string tipoTransacao = String.Empty;

                if (SwtEntradaSaida.On)
                {
                    tipoTransacao = "Saída";
                }
                else
                {
                    tipoTransacao = "Entrada";
                }

                var page = new GuardaMateriasConfirmaQuantidadePopUp(_codDepos, _local, current.CodigoItem, tipoTransacao, pCodBarras);
                page._actConfirmaGuardaMateriais = CodigoBarras;
                _pagePopUp = page;
                await PopupNavigation.Instance.PushAsync(page);

            }
            catch (Exception ex)
            {
                await DisplayAlert("ERRO", ex.Message, "OK");
            }
            finally
            {
                cvGuardaMateriaisDepositoItem.IsEnabled = true;
            }
        }


        GuardaMateriasConfirmaQuantidadePopUp _pagePopUp = null;
        GuardaMateriasDepositoLocalizPopUp _pagePopUpLocaliz = null;

        private async void BtnTipoMovimento_Clicked(object sender, EventArgs e)
        {
            try
            {
                BtnTipoMovimento.IsEnabled = false;

                //var pageProgress = new ProgressBarPopUp("Carregando...");

                _pagePopUpLocaliz = new GuardaMateriasDepositoLocalizPopUp(null, _depositoGuardaMaterial);
                _pagePopUpLocaliz._confirmaLocalizacaoItem = CarregaListViewPopUp;
                await PopupNavigation.Instance.PushAsync(_pagePopUpLocaliz);
                //await pageProgress.OnClose();

               // cvDepositosGuardaMaterial.SelectedItem = null;
            }
            catch (Exception ex)
            {
                await DisplayAlert("ERRO", ex.Message, "OK");
            }
            finally
            {
                BtnTipoMovimento.IsEnabled = true;
            } 
        }
        static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, @"[^0-9a-zA-Z;./-]+", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters,
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }
    }



    public class DepositosGerarPedidoItemViewModel : DepositosGuardaMaterialItem
    {
        public string StrSaldo
        {
            get
            {
                return SaldoInfo.ToString().Replace(".", ",");
            }
        }
    }
}