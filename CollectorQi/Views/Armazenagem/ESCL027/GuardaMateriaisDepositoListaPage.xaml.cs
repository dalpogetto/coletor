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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GuardaMateriaisDepositoListaPage : ContentPage, INotifyPropertyChanged
    {
        #region Property

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

        public ObservableCollection<GuardaMateriaisDepositoViewModel> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                OnPropertyChanged("Items");
            }
        }

        private ObservableCollection<GuardaMateriaisDepositoViewModel> _Items;
        private ObservableCollection<GuardaMateriaisDepositoViewModel> _ItemsFiltered;
        private ObservableCollection<GuardaMateriaisDepositoViewModel> _ItemsUnfiltered;


        /*
        public ObservableCollection<GuardaMateriaisDepositoViewModel> ObsDepositosGuardaMaterial { get; set; }
        public List<DepositosGuardaMaterial> ListaDepositosGuardaMaterial { get; set; }
        public DepositosGuardaMaterial DepositosGuardaMaterial { get; set; }
        */

        public GuardaMateriaisDepositoListaPage()
        {
            InitializeComponent();
            //ObsDepositosGuardaMaterial = new ObservableCollection<GuardaMateriaisDepositoViewModel>();
            //ListaDepositosGuardaMaterial = new List<DepositosGuardaMaterial>();
            //ListaDepositosGuardaMaterial = listadepositosGuardaMaterial;

            lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.Estabelecimento;

            /*
            if (listadepositosGuardaMaterial != null)
            {
                foreach (var item in listadepositosGuardaMaterial)
                {
                    if (item.CodDepos != "DEC")
                        continue;

                    var modelView = Mapper.Map<DepositosGuardaMaterial, GuardaMateriaisDepositoViewModel>(item);
                    ObsDepositosGuardaMaterial.Add(modelView);
                }
            }*/

            cvDepositosGuardaMaterial.BindingContext = this;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            //ObsDepositosGuardaMaterial = new ObservableCollection<GuardaMateriaisDepositoViewModel>();
            Items = new ObservableCollection<GuardaMateriaisDepositoViewModel>();


            CarregaListView();
        }

        private async void CarregaListView()
        {
            //var lstInventario = await ParametersInventarioService.SendParametersAsync();
            var pageProgress = new ProgressBarPopUp("Carregando Guarda de Materias, aguarde...");

            // ObsDepositosGuardaMaterial.Clear();

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                var listadepositosGuardaMaterial = await DepositosGuardaMaterialService.SendGuardaMaterialAsync();

                if (listadepositosGuardaMaterial != null && listadepositosGuardaMaterial.Param != null && listadepositosGuardaMaterial.Param.ParamResult != null)
                {
                    foreach (var item in listadepositosGuardaMaterial.Param.ParamResult)
                    {
                      // if (item.CodDepos != "DEC")
                      //     continue;

                        var modelView = Mapper.Map<DepositosGuardaMaterial, GuardaMateriaisDepositoViewModel>(item);
                        Items.Add(modelView);
                    }
                }
                
                SearchBarCodDepos.Focus();

                _ItemsUnfiltered = Items;

                OnPropertyChanged("Items");

                /*
                ObsInventario.Clear();

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                //var lstInventario = new ObservableCollection<InventarioLocalizacaoViewModel>();

                //var lstLocalizacoesVO = await ParametersObterLocalizacaoUsuarioService.GetObterLocalizacoesUsuarioAsync(_inventarioVO.InventarioId, this);
                var lstInventario = await ParametersInventarioService.SendParametersAsync(this);

                foreach (var row in lstInventario)
                {
                    var modelView = Mapper.Map<InventarioVO, InventarioViewModel>(row);

                    ObsInventario.Add(modelView);
                }

                OnPropertyChanged("ObsInventario"); */

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

        public async void ConfirmaLocalizacao(string pLocalizacao, DepositosGuardaMaterial current)
        {
            string codigoBarras = pLocalizacao;

            var dLeituraEtiqueta = new LeituraEtiquetaLocalizaGuardaMaterialService();
            var dadosLeituraLocalizaGuardaMaterial = new DadosLeituraLocalizaGuardaMaterial()
            { CodEstabel = SecurityAuxiliar.GetCodEstabel(), CodigoBarras = codigoBarras };

            // Leitura Localizacação - /api/integracao/coletores/v1/escl021api/LeituraEtiquetaLocaliza
            //var dDepositoItemRetorno = await dLeituraEtiqueta.SendLeituraEtiquetaLocalizaAsync(dadosLeituraLocalizaGuardaMaterial);         

            // recarrega a lista da API
            var dadosLeituraItemGuardaMaterial = new DadosLeituraItemGuardaMaterial()
            { CodEstabel = SecurityAuxiliar.GetCodEstabel(), CodDepos = current.CodDepos, CodigoBarras = codigoBarras };

            dadosLeituraItemGuardaMaterial.CodLocaliza = pLocalizacao;
            dadosLeituraItemGuardaMaterial.Transacao   = 1;
            dadosLeituraItemGuardaMaterial.SemSaldo    = 0;

            // /api/integracao/coletores/v1/escl027api/LeituraEtiquetaLocaliza
          //  var dLeituraEtiquetaLerLocaliza = new LeituraEtiquetaLerLocalizaGuardaMaterialService();
            var dRetorno = await LeituraEtiquetaLerLocalizaGuardaMaterialService.SendLeituraEtiquetaAsync(dadosLeituraItemGuardaMaterial);

            System.Diagnostics.Debug.Write(dRetorno);

            if (dRetorno.Retorno.Contains("Error"))
            {
                await DisplayAlert("ERRO", dRetorno.Resultparam[0].ErrorHelp, "OK");
            }
            else
            {
                Application.Current.MainPage = new NavigationPage(new GuardaMateriaisDepositoItemListaPage(dRetorno.paramRetorno,
                                                                                                           codigoBarras,
                                                                                                           current.CodDepos,1)); 
            }
        }

        async void cvDepositosGuardaMaterial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                cvDepositosGuardaMaterial.IsEnabled = false;

                if (cvDepositosGuardaMaterial.SelectedItem == null)
                    return;

                var current = (cvDepositosGuardaMaterial.SelectedItem as DepositosGuardaMaterial);

                //var pageProgress = new ProgressBarPopUp("Carregando...");
                
                var page = new GuardaMateriasDepositoLocalizPopUp(null, current);
                page._confirmaLocalizacao = ConfirmaLocalizacao;
                await PopupNavigation.Instance.PushAsync(page);
                //await pageProgress.OnClose();

                cvDepositosGuardaMaterial.SelectedItem = null;
            }
            catch (Exception ex)
            {
                await DisplayAlert("ERRO", ex.Message, "OK");
            }
            finally
            {
                cvDepositosGuardaMaterial.IsEnabled = true;
            }


            /*
            
            var current = (cvDepositosGuardaMaterial.SelectedItem as DepositosGuardaMaterial);

            string codigoBarras = "10[COR1800000";

            var dLeituraEtiqueta = new LeituraEtiquetaLocalizaGuardaMaterialService();
            var dadosLeituraLocalizaGuardaMaterial = new DadosLeituraLocalizaGuardaMaterial() 
                { CodEstabel = SecurityAuxiliar.GetCodEstabel(), CodigoBarras = codigoBarras };

            // Leitura Localizacação - /api/integracao/coletores/v1/escl021api/LeituraEtiquetaLocaliza
            //var dDepositoItemRetorno = await dLeituraEtiqueta.SendLeituraEtiquetaLocalizaAsync(dadosLeituraLocalizaGuardaMaterial);         

            // recarrega a lista da API
            var dadosLeituraItemGuardaMaterial = new DadosLeituraItemGuardaMaterial()
            { CodEstabel = SecurityAuxiliar.GetCodEstabel(), CodDepos = current.CodDepos, CodigoBarras = codigoBarras };

            dadosLeituraItemGuardaMaterial.CodLocaliza = "COR1800000";
            dadosLeituraItemGuardaMaterial.Transacao = 1;
            dadosLeituraItemGuardaMaterial.SemSaldo = 0;

            // /api/integracao/coletores/v1/escl027api/LeituraEtiquetaLocaliza
            var dLeituraEtiquetaLerLocaliza = new LeituraEtiquetaLerLocalizaGuardaMaterialService();
            var dRetorno = await dLeituraEtiquetaLerLocaliza.SendLeituraEtiquetaAsync(dadosLeituraItemGuardaMaterial);

            Application.Current.MainPage = new NavigationPage(new GuardaMateriaisTipoMovimento(dRetorno.Param.ParamResult, 
                                                                                               codigoBarras, 
                                                                                               current.CodDepos));
            */
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new ArmazenagemPage());

            return true;
        }

        async public void PerformSearch()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchBarCodDepos.Text))
                    Items = _ItemsUnfiltered;
                else
                {
                    /* Victor Alves - 31/10/2019 - Melhoria de performance Item */
                    _ItemsFiltered = new ObservableCollection<GuardaMateriaisDepositoViewModel>(_ItemsUnfiltered.Where(i =>
                   (i is GuardaMateriaisDepositoViewModel && (((GuardaMateriaisDepositoViewModel)i).CodDeposNome.ToLower().Contains(SearchBarCodDepos.Text.ToLower())))
                   ));

                    Items = _ItemsFiltered;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancel");
            }
            finally
            {
                //cvInventarioItem.IsEnabled = true;
            }
        }

        /*
        private async void SearchBarCdDepos_Unfocused(object sender, FocusEventArgs e)
        {
            if (!String.IsNullOrEmpty(SearchBarCodDepos.Text))
            {
                var current = _ItemsUnfiltered.FirstOrDefault(p => p.CodDeposNome == SearchBarItCodigo.Text);

                if (current == null)
                {
                    if (SearchBarItCodigo.Text.Contains(';'))
                    {

                        var textItCodigo = SearchBarItCodigo.Text.Split(';');
                        if (textItCodigo.Length > 1)
                        {
                            current = _ItemsUnfiltered.FirstOrDefault(p => p.CodItem == textItCodigo[1]);
                        }
                    }
                }

                if (current != null)
                {
                    OpenPagePopUp(current);

                }
            }
        }
        */

        private CancellationTokenSource throttleCts = new CancellationTokenSource();

        async void Handle_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                //await Task.Run(() => PerformSearch());
                //PerformSearch();
                /* Victor Alves - 31/10/2019 - Processo para cancelar thread se digita varias vezes o item e trava  */
                Interlocked.Exchange(ref this.throttleCts, new CancellationTokenSource()).Cancel();
                await Task.Delay(TimeSpan.FromMilliseconds(1500), this.throttleCts.Token) // if no keystroke occurs, carry on after 500ms
                    .ContinueWith(
                        delegate { PerformSearch(); }, // Pass the changed text to the PerformSearch function
                        CancellationToken.None,
                        TaskContinuationOptions.OnlyOnRanToCompletion,
                        TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Erro!", ex.Message, "Cancel");
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

    }    

    public class GuardaMateriaisDepositoViewModel : DepositosGuardaMaterial, INotifyPropertyChanged
    {
        public string CodDeposNome
        {
            get { return CodDepos + " - " + Nome; }
        }
        //public string Image
        //{
        //    get
        //    {
        //        if (this.ItensRestantes)
        //            return "intSucessoMed.png";
        //        else
        //            return "intPendenteMed.png";
        //    }
        //}
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}