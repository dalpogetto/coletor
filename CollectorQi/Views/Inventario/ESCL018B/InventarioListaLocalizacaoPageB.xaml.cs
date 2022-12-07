using CollectorQi.Models;
using CollectorQi.ViewModels;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Resources.DataBaseHelper.Batch;
using CollectorQi.Resources;
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
using CollectorQi.ViewModels.Interface;
using CollectorQi.VO.ESCL018;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventarioListaLocalizacaoPageB : ContentPage, INotifyPropertyChanged
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

        public InventarioListaLocalizacaoPageB(InventarioVO inventarioVO)
        {
            InitializeComponent();

            _inventarioVO = inventarioVO;

            this.Title = $"Busca Localização";

            lblCodDepos.Text = $"Depósito: {inventarioVO.CodDepos} - {inventarioVO.DescDepos}";

            cvLeituraEtiqueta.BindingContext = this;
        }

        async void OnClick_BuscaEtiqueta(object sender, System.EventArgs e)
        {
           //var pageProgress = new ProgressBarPopUp("Carregando Consulta de Etiqueta...");
           //await PopupNavigation.Instance.PushAsync(pageProgress);
           //
           //try
           //{
           //    var inventario = new Inventario()
           //    {
           //        IdInventario = _inventarioVO.InventarioId,
           //        CodEstabel = _inventarioVO.CodEstabel,
           //        CodDepos = _inventarioVO.CodDepos,
           //      //  CodigoBarras = txtEtiqueta.Text,
           //    };
           //
           //    var localizacao = new ParametersLocalizacaoLeituraEtiquetaService();
           //    var localizacaoResult = await localizacao.SendInventarioAsync(inventario);
           //  //  txtEtiqueta.Text = localizacaoRetorno = localizacaoResult.Resultparam.Localizacao;
           //}
           //catch (Exception ex)
           //{
           //    var pageProgressErro = new ProgressBarPopUp("Erro: " + ex.Message);
           //    await PopupNavigation.Instance.PushAsync(pageProgressErro);
           //    await pageProgressErro.OnClose();
           //}
           //finally
           //{
           //    await pageProgress.OnClose();
           //    //BtnProximo.IsEnabled = true;
           //}
        }


        private async void CarregaListView()
        {
            var pageProgress = new ProgressBarPopUp("Carregando Localização, aguarde...");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                Items = new ObservableCollection<InventarioLocalizacaoViewModel>();

                var lstLocalizacoesVO = await ParametersObterLocalizacaoUsuarioService.GetObterLocalizacoesUsuarioAsync(_inventarioVO.IdInventario, this);

                foreach (var localizacao in lstLocalizacoesVO)
                {
                    var modelView = Mapper.Map<InventarioLocalizacaoVO, InventarioLocalizacaoViewModel>(localizacao);

                    Items.Add(modelView);
                }

                SearchLocalizacao.Focus();

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

        protected override void OnAppearing()
        {
            base.OnAppearing();

            CarregaListView();
        }

        private CancellationTokenSource throttleCts = new CancellationTokenSource();
        private bool lCodBarras = false;
        async void Handle_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                System.Diagnostics.Debug.Write(e);

                //System.Diagnostics.Debug.Write(e);
                bool lCodBarras = false;
                if (String.IsNullOrEmpty(e.OldTextValue) && e.NewTextValue.Length > 5)
                {
                    lCodBarras = true;
                }

                /* Victor Alves - 31/10/2019 - Processo para cancelar thread se digita varias vezes o item e trava  */
                Interlocked.Exchange(ref this.throttleCts, new CancellationTokenSource()).Cancel();
                await Task.Delay(TimeSpan.FromMilliseconds(1200), this.throttleCts.Token) // if no keystroke occurs, carry on after 500ms
                    .ContinueWith(
                        delegate { PerformSearch(lCodBarras); }, // Pass the changed text to the PerformSearch function
                        CancellationToken.None,
                        TaskContinuationOptions.OnlyOnRanToCompletion,
                        TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
            }
        }

        async public void PerformSearch(bool plCodBarras)
        {
            try
            {
                //cvInventarioItem.IsEnabled = false;

                string strCodLocalizacao = string.Empty;

                if (SearchLocalizacao.Text.IndexOf(";") >= 1)
                {
                    var splItem = SearchLocalizacao.Text.Split(';');

                    if (splItem[0] == "10")
                    {
                        strCodLocalizacao = splItem[1].Trim();
                    }
                }
                else
                {
                    strCodLocalizacao = SearchLocalizacao.Text.Trim();
                }

                if (string.IsNullOrWhiteSpace(strCodLocalizacao))
                    Items = _ItemsUnfiltered;
                else
                {
                    /* Victor Alves - 31/10/2019 - Melhoria de performance Item */
                    _ItemsFiltered = new ObservableCollection<InventarioLocalizacaoViewModel>(_ItemsUnfiltered.Where(i =>
                   (i is InventarioLocalizacaoViewModel && (((InventarioLocalizacaoViewModel)i).Localizacao.ToLower().Contains(strCodLocalizacao.ToLower())))
                   ));

                    Items = _ItemsFiltered;
                }

                if (plCodBarras)
                {
                    SearchLocalizacao.Unfocus();
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

        private async void cvLeituraEtiqueta_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cvLeituraEtiqueta.SelectedItem == null)
                return;

            var current = (cvLeituraEtiqueta.SelectedItem as InventarioLocalizacaoVO);

            if (current != null)
            {
                // Criar();

                /*
                var lstInventarioItem = await ParametersFichasUsuarioService.GetObterFichasUsuarioAsync(_inventarioVO.InventarioId, current.CodLocaliz);

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
                }
                */

                Application.Current.MainPage = new NavigationPage(new InventarioListaItemPageB(_inventarioVO, current.Localizacao));
            }

            cvLeituraEtiqueta.SelectedItem = null;
        }
        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new InventarioListaPageB());

            return true;
        }
        public class InventarioLocalizacaoViewModel : InventarioLocalizacaoVO, INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
        }

        private void SearchLocalizacao_Unfocused(object sender, FocusEventArgs e)
        {
            if (!String.IsNullOrEmpty(SearchLocalizacao.Text))
            {
                var current = Items.FirstOrDefault(p => p.Localizacao.ToLower() == SearchLocalizacao.Text.ToLower());

                if (current == null)
                {
                    string strLocalizacao = SearchLocalizacao.Text.Replace("10;", "").Trim().ToLower();

                    current = _ItemsUnfiltered.FirstOrDefault(p => p.Localizacao.ToLower() == strLocalizacao);
                }

                if (current != null)
                {
                    Application.Current.MainPage = new NavigationPage(new InventarioListaItemPageB(_inventarioVO, SearchLocalizacao.Text.Replace("10;", "").Trim()));
                }
            }

            /*

            if (!String.IsNullOrEmpty(SearchBarItCodigo.Text))
            {
                var current = _ItemsUnfiltered.FirstOrDefault(p => p.CodItem == SearchBarItCodigo.Text);

                if (current == null)
                {
                    if (SearchBarItCodigo.Text.Contains(';'))
                    {

                        var textItCodigo = SearchBarItCodigo.Text.Split(';');
                        if (textItCodigo.Length > 1)
                        {
                            current = _ItemsUnfiltered.FirstOrDefault(p => p.CodItem == textItCodigo[1]);

                            if (current != null)
                            {
                                current.CodigoBarras = SearchBarItCodigo.Text;
                            }
                        }
                    }
                }

                if (current != null)
                {
                    OpenPagePopUp(current);

                }
            }*/
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                ToolBarPrint.IsEnabled = false;

                var pageProgress = new ProgressBarPopUp("Carregando...");
                var page = new InventarioPrintPopUp(_inventarioVO.CodDepos, null, null, null);
                await PopupNavigation.Instance.PushAsync(page);
                //Thread.Sleep(1000);
                await pageProgress.OnClose();
            }
            finally
            {
                ToolBarPrint.IsEnabled = true;
            }
        }

    }
}