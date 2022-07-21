using CollectorQi.Models;
using CollectorQi.ViewModels;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Resources.DataBaseHelper.Batch;
using CollectorQi.Resources;
using CollectorQi.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CollectorQi.Models.Datasul;
using AutoMapper;
using System.Globalization;
using Plugin.Connectivity;
using CollectorQi.Services.ESCL018;
using ESCL = CollectorQi.Models.ESCL018;
using CollectorQi.Models.ESCL018;
using Rg.Plugins.Popup.Services;
using System.Threading;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventarioListaLocalizacaoPage : ContentPage, INotifyPropertyChanged
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

        public ObservableCollection<InventarioLocalizacaoViewModel> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                OnPropertyChanged("Items");
            }
        }

        private ObservableCollection<InventarioLocalizacaoViewModel> _Items;

        private ObservableCollection<InventarioLocalizacaoViewModel> _ItemsFiltered;

        private ObservableCollection<InventarioLocalizacaoViewModel> _ItemsUnfiltered;

        private InventarioVO _inventarioVO { get; set; }

        private List<InventarioItemVO> _lstInventarioItemVO { get; set; }

        //public ObservableCollection<InventarioViewModel> ObsInventario { get; }

        //public string localizacaoRetorno { get; set; }

        public InventarioListaLocalizacaoPage(InventarioVO inventarioVO)
        {
            InitializeComponent();

            _inventarioVO = inventarioVO;

            this.Title = $"Busca Localização";

            lblCodEstabel.Text = $"Depósito: {inventarioVO.CodDepos} - {inventarioVO.DescDepos}";

            cvLeituraEtiqueta.BindingContext = this;
        }

        async void OnClick_BuscaEtiqueta(object sender, System.EventArgs e)
        {
            var pageProgress = new ProgressBarPopUp("Carregando Consulta de Etiqueta...");
            await PopupNavigation.Instance.PushAsync(pageProgress);

            try
            {
                var inventario = new Inventario()
                {
                    IdInventario = _inventarioVO.InventarioId,
                    CodEstabel = _inventarioVO.CodEstabel,
                    CodDepos = _inventarioVO.CodDepos,
                  //  CodigoBarras = txtEtiqueta.Text,
                };

                var localizacao = new ParametersLocalizacaoLeituraEtiquetaService();
                var localizacaoResult = await localizacao.SendInventarioAsync(inventario);
              //  txtEtiqueta.Text = localizacaoRetorno = localizacaoResult.Resultparam.Localizacao;
            }
            catch (Exception ex)
            {
                var pageProgressErro = new ProgressBarPopUp("Erro: " + ex.Message);
                await PopupNavigation.Instance.PushAsync(pageProgressErro);
                await pageProgressErro.OnClose();
            }
            finally
            {
                await pageProgress.OnClose();
                //BtnProximo.IsEnabled = true;
            }
        }


        async void Criar()
        {

            /*var parametersFichasUsuario = new ParametersFichasUsuarioService();
            var lstInventarioVO = await parametersFichasUsuario.GetObterFichasUsuarioAsync(_inventarioVO.InventarioId);

            //var lstInventarioVOFiltro = lstInventarioVO.param.Resultparam.Where(x => x.Localizacao == localizacaoRetorno);

            foreach (var item in lstInventarioVO.param.Resultparam.Where(x => x.Localizacao == localizacaoRetorno))
            {
                InventarioItemVO inventarioItem = new InventarioItemVO();
                inventarioItem.InventarioId = item.IdInventario;
                inventarioItem.CodLote = item.Lote;
                inventarioItem.CodLocaliz = item.Localizacao;
                inventarioItem.CodRefer = item.CodItem;
               // inventarioItem.NrFicha = item.Quantidade;

                InventarioItemDB.InserirInventarioItem(inventarioItem);
            } */
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var pageProgress = new ProgressBarPopUp("Carregando Localização, aguarde...");

            try
            {

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                Items = new ObservableCollection<InventarioLocalizacaoViewModel>();


                var lstInventarioItem = await ParametersFichasUsuarioService.GetObterFichasUsuarioAsync(_inventarioVO.InventarioId);


                List<InventarioLocalizacaoVO> lstInventarioLocalizacaoVO = new List<InventarioLocalizacaoVO>();
                _lstInventarioItemVO = new List<InventarioItemVO>();

                foreach (var inventarioItem in lstInventarioItem.param.Resultparam)
                {
                    InventarioItemVO inventarioItemVO = new InventarioItemVO();
                    inventarioItemVO.InventarioId = inventarioItem.IdInventario;
                    inventarioItemVO.CodLocaliz = inventarioItem.Localizacao;
                    inventarioItemVO.CodLote = inventarioItem.Lote;
                        // CodRefer = inventarioItem.Cod
                    inventarioItemVO.ItCodigo = inventarioItem.CodItem;

                    InventarioItemDB.InserirInventarioItem(inventarioItemVO);

                    _lstInventarioItemVO.Add(inventarioItemVO);

                    if (Items.FirstOrDefault(p => p.CodLocaliz == inventarioItem.Localizacao) == null)
                    {
                        InventarioLocalizacaoVO inventarioLocalizacaoVO = new InventarioLocalizacaoVO();
                        inventarioLocalizacaoVO.CodLocaliz = inventarioItem.Localizacao;


                        InventarioLocalizacaoDB.InserirInventarioLocalizacao(inventarioLocalizacaoVO);

                        var modelView = Mapper.Map<InventarioLocalizacaoVO, InventarioLocalizacaoViewModel>(inventarioLocalizacaoVO);

                        Items.Add(modelView);
                    }
                }

                _ItemsUnfiltered = Items;

                OnPropertyChanged("Items");

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


        private CancellationTokenSource throttleCts = new CancellationTokenSource();

        async void Handle_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.Write(e);

                //await Task.Run(() => PerformSearch());
                //PerformSearch();
                /* Victor Alves - 31/10/2019 - Processo para cancelar thread se digita varias vezes o item e trava  */
                Interlocked.Exchange(ref this.throttleCts, new CancellationTokenSource()).Cancel();
                await Task.Delay(TimeSpan.FromMilliseconds(500), this.throttleCts.Token) // if no keystroke occurs, carry on after 500ms
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

        async public void PerformSearch()
        {
            try
            {
                //cvInventarioItem.IsEnabled = false;

                if (string.IsNullOrWhiteSpace(SearchCodLocaliz.Text))
                    Items = _ItemsUnfiltered;
                else
                {
                    /*
                    _ItemsFiltered = new ObservableCollection<InventarioItemViewModel>(_ItemsUnfiltered.Where(i =>
                    (i is InventarioItemViewModel && (((InventarioItemViewModel)i).ItCodigo.ToLower().Contains(SearchBarItCodigo.Text.ToLower())))  ||
                    (i is InventarioItemViewModel && (((InventarioItemViewModel)i).__item__.DescItem.ToLower().Contains(SearchBarItCodigo.Text.ToLower())))
                    ));*/

                    /* Victor Alves - 31/10/2019 - Melhoria de performance Item */
                    _ItemsFiltered = new ObservableCollection<InventarioLocalizacaoViewModel>(_ItemsUnfiltered.Where(i =>
                   (i is InventarioLocalizacaoViewModel && (((InventarioLocalizacaoViewModel)i).CodLocaliz.ToLower().Contains(SearchCodLocaliz.Text.ToLower())))
                   ));

                    /*
                    _ItemsFiltered = new ObservableCollection<InventarioItemViewModel>(_ItemsUnfiltered.Where(i =>
                    (i is InventarioItemViewModel && (((InventarioItemViewModel)i).__item__.ItCodigo.ToLower().Contains(SearchBarItCodigo.Text.ToLower()))) ||
                     i is InventarioItemViewModel && (((InventarioItemViewModel)i).__item__.DescItem.ToLower().Contains(SearchBarItCodigo.Text.ToLower())))
                    );*/

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


        async void OnClick_Proximo(object sender, System.EventArgs e)
        {
            Criar();
        //    Application.Current.MainPage = new NavigationPage(new InventarioListaItemPage(_inventarioVO));
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new InventarioListaPage());

            return true;
        }

        private void cvLeituraEtiqueta_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cvLeituraEtiqueta.SelectedItem == null)
                return;
            
            var current = (cvLeituraEtiqueta.SelectedItem as InventarioLocalizacaoVO);

            if (current != null)
            {
               // Criar();
                Application.Current.MainPage = new NavigationPage(new InventarioListaItemPage(_inventarioVO, _lstInventarioItemVO.FindAll(p => p.CodLocaliz == current.CodLocaliz)));
            }

            cvLeituraEtiqueta.SelectedItem = null;
        }

        public class InventarioLocalizacaoViewModel : InventarioLocalizacaoVO, INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
        }

        private void SearchCodLocaliz_Unfocused(object sender, FocusEventArgs e)
        {
            if (!String.IsNullOrEmpty(SearchCodLocaliz.Text))
            {
                var current = Items.FirstOrDefault(p => p.CodLocaliz == SearchCodLocaliz.Text);

                if (current == null)
                {
                    _ItemsUnfiltered.FirstOrDefault(p => p.CodLocaliz == SearchCodLocaliz.Text.Replace("10;",""));
                }

                if (current != null)
                {
                    Application.Current.MainPage = new NavigationPage(new InventarioListaItemPage(_inventarioVO, _lstInventarioItemVO.FindAll(p => p.CodLocaliz == current.CodLocaliz)));
                }
            }
        }
    }
}