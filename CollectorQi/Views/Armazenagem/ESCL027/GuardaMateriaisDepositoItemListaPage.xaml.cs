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
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GuardaMateriaisDepositoItemListaPage : ContentPage, INotifyPropertyChanged
    {  
        public ObservableCollection<DepositosGuardaMaterialItemViewModel> ObsGuardaMateriaisDepositoItem { get; set; }
        public List<DepositosGuardaMaterialItem> ListaDepositosGuardaMaterialItem { get; set; }
        public string Local { get; set; }
        public string CodDepos { get; set; }
        public int? TipoMovimento { get; set; }
        public int SemSaldo { get; set; }

        private List<DepositosGuardaMaterialItem> _listaDepositosGuardaMaterialItem;
        private string _codigoBarrasLocalizacao;

        public GuardaMateriaisDepositoItemListaPage(List<DepositosGuardaMaterialItem> listaDepositosGuardaMaterialItem, string local, string codDepos, int? tipoMovimento)
        {
            InitializeComponent();

            _codigoBarrasLocalizacao = local;

            if (local.Length > 0)
            {
                local = local.Replace("10[", "");
            }

            if (tipoMovimento == 1)
                BtnTipoMovimento.Text = "Depósito: " + codDepos + "   /   Localização: " + local;
            else
                BtnTipoMovimento.Text = "Depósito: " + codDepos + "   /   Localização: " + local;

            ObsGuardaMateriaisDepositoItem = new ObservableCollection<DepositosGuardaMaterialItemViewModel>();
            Local                          = local;
            TipoMovimento                  = tipoMovimento;
            CodDepos                        = codDepos;
            _listaDepositosGuardaMaterialItem = listaDepositosGuardaMaterialItem;

            cvGuardaMateriaisDepositoItem.BindingContext = this;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new GuardaMateriaisDepositoListaPage());

            return true;
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
        public async void CodigoBarras(string pCodigoBarras)
        {
            var pageProgress = new ProgressBarPopUp("Efetivando Item, aguarde...");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                ObsGuardaMateriaisDepositoItem  = new ObservableCollection<DepositosGuardaMaterialItemViewModel>();
                var depositosGuardaMaterialItem = new DepositosGuardaMaterialItem();

                var dadosLeituraItemGuardaMaterial = new DadosLeituraItemGuardaMaterial()
                {
                    CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                    CodDepos = CodDepos,
                    CodLocaliza = Local,
                    Transacao = TipoMovimento,
                    SemSaldo = SemSaldo,
                    CodigoBarras = pCodigoBarras
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
            }
            else
            {
                SwtEntradaSaida.Text = "Entrada";
            }
        }

        private void SwitchCell_OnChanged(object sender, ToggledEventArgs e)
        {
            cvGuardaMateriaisDepositoItem.BindingContext = null;
            ObsGuardaMateriaisDepositoItem = new ObservableCollection<DepositosGuardaMaterialItemViewModel>();

            var listaDepositosGuardaMaterialItem = new List<DepositosGuardaMaterialItem>();

            if (((SwitchCell)sender).On)
            {
                listaDepositosGuardaMaterialItem.AddRange(ListaDepositosGuardaMaterialItem);
                SemSaldo = 1;
            }
            else
            {
                // // Retirar .Where(x => x.SaldoInfo != 0)
                listaDepositosGuardaMaterialItem.AddRange(ListaDepositosGuardaMaterialItem.Where(x => x.SaldoInfo != 0));
                SemSaldo = 0;
            }

            // Chamar a API novamente passando sem saldo 0 
            foreach (var item in listaDepositosGuardaMaterialItem)
            {
                var modelView = Mapper.Map<DepositosGuardaMaterialItem, DepositosGuardaMaterialItemViewModel>(item);
                ObsGuardaMateriaisDepositoItem.Add(modelView);
            }           

            cvGuardaMateriaisDepositoItem.BindingContext = this;
        }
    }

    public class DepositosGuardaMaterialItemViewModel : DepositosGuardaMaterialItem
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