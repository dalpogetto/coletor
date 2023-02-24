using AutoMapper;
using CollectorQi.Models.ESCL000;
using CollectorQi.Models.ESCL021;
using CollectorQi.Models.ESCL025;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL000;
using CollectorQi.Services.ESCL021;
using CollectorQi.Services.ESCL027;
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
    public partial class SaldoVirtualLocalizacaoListaPage : ContentPage, INotifyPropertyChanged
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

        public ObservableCollection<LocalizacaoSaldoViewModel> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                OnPropertyChanged("Items");
            }
        }

        private ObservableCollection<LocalizacaoSaldoViewModel> _Items;
        private ObservableCollection<LocalizacaoSaldoViewModel> _ItemsFiltered;
        private ObservableCollection<LocalizacaoSaldoViewModel> _ItemsUnfiltered;

        public string _codDepos { get; set; }
        public string _codItem { get; set; }
        public SaldoVirtualLocalizacaoListaPage(string pCodDepos, string pCodItem)
        {
            InitializeComponent();

            lblCodEstabel.Text = "Item: " + pCodItem;

            _codDepos = pCodDepos;
            _codItem = pCodItem;

            cvLocalizacoes.BindingContext = this;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            Items = new ObservableCollection<LocalizacaoSaldoViewModel>();

            CarregaListView();
        }

        private async Task CarregaListView()
        {
            lblCodEstabel.Text = "Depósito: " + _codDepos + " / " + "Item: " + _codItem;

            //var lstInventario = await ParametersInventarioService.SendParametersAsync();
            var pageProgress = new ProgressBarPopUp("Carregando Localização, aguarde...");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                Items.Clear();

                var lstLocalizacao = await ItemService.ObterLocalizacaoPorEstabDepItem(_codDepos, _codItem, true);

                System.Diagnostics.Debug.Write(lstLocalizacao);
                                
                if (lstLocalizacao != null && lstLocalizacao.param != null && lstLocalizacao.param.Resultparam != null && lstLocalizacao.param.Resultparam.Count > 0)
                {
                    foreach (var row in lstLocalizacao.param.Resultparam)
                    {
                        Items.Add(new LocalizacaoSaldoViewModel { CodLocaliz = row.CodLocaliz, SaldoInfo = row.SaldoInfo });
                    }
                }
                
                SearchBarCodLocaliz.Focus();

                _ItemsUnfiltered = Items;

                OnPropertyChanged("Items");

                if (_origChangeItem)
                {
                    SearchBarCodLocaliz.Text = "";
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
        
        async void cvLocalizacoes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            /*

            try
            {
                cvLocalizacoes.IsEnabled = false;

                if (cvLocalizacoes.SelectedItem == null)
                    return;

                var current = (cvLocalizacoes.SelectedItem as LocalizacaoViewModel);

                //Application.Current.MainPage = new NavigationPage(new ConsultaLocalizacaoItemListaPage(_deposito, current.CodLocaliz));

                cvLocalizacoes.SelectedItem = null;
            }
            catch (Exception ex)
            {
                await DisplayAlert("ERRO", ex.Message, "OK");
            }
            finally
            {
                cvLocalizacoes.IsEnabled = true;
            }*/


            try
            {
                cvLocalizacoes.IsEnabled = false;

                if (cvLocalizacoes.SelectedItem == null)
                    return;

                var current = (cvLocalizacoes.SelectedItem as LocalizacaoSaldoViewModel);

                var page = new SaldoVirtualItemPopUp(_codDepos, current.CodLocaliz, _codItem, current.SaldoInfo.ToString());

                page._actRefresh = RefreshAction;

                await PopupNavigation.Instance.PushAsync(page);

                cvLocalizacoes.SelectedItem = null;

            }
            catch (Exception ex)
            {
                await DisplayAlert("ERRO", ex.Message, "OK");
            }
            finally
            {
                cvLocalizacoes.IsEnabled = true;
            }

        }

        public void RefreshAction(string pCodItem, string pCodLocaliz, string pSaldoInfo)
        {
            if (pCodItem != null)
            {
                var itemCurrent = Items.FirstOrDefault(x => x.CodLocaliz == pCodLocaliz);

                if (itemCurrent != null)
                {
                    itemCurrent.SaldoInfo = pSaldoInfo;
                }

                var itemCurrentUn = _ItemsUnfiltered.FirstOrDefault(x => x.CodLocaliz == pCodItem);

                if (itemCurrentUn != null)
                {
                    itemCurrentUn.SaldoInfo = pSaldoInfo;
                }

                itemCurrent.OnPropertyChanged("SaldoInfo");

                OnPropertyChanged("Items");
            }
        }


        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new SaldoVirtualDepositoListaPage(_codDepos));

            return true;
        }

        async public void PerformSearch()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchBarCodLocaliz.Text))
                    Items = _ItemsUnfiltered;
                else
                {
                    /* Victor Alves - 31/10/2019 - Melhoria de performance Item */
                    _ItemsFiltered = new ObservableCollection<LocalizacaoSaldoViewModel>(_ItemsUnfiltered.Where(i =>
                   (i is LocalizacaoSaldoViewModel && (((LocalizacaoSaldoViewModel)i).CodLocaliz.ToLower().Contains(SearchBarCodLocaliz.Text.ToLower())))
                   ));

                    Items = _ItemsFiltered;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancel");
            }
        }

        private CancellationTokenSource throttleCts = new CancellationTokenSource();

        private bool _origChangeItem;
        async void Handle_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                if (String.IsNullOrEmpty(e.OldTextValue) && e.NewTextValue.Length > 5 && e.NewTextValue.Substring(0,3) != "10;")
                {
                    var splItem = e.NewTextValue.Split(';');
                    string strCodItem;

                    if (splItem.Length > 1)
                    {
                        strCodItem = splItem[1];
                    }
                    else
                    {
                        strCodItem = e.NewTextValue;
                    }


                    _codItem = strCodItem;
                    // Usuario lendo etiqueta de localização, precisa mudar a localização e refresh em tela.
                    _origChangeItem = true;
                    await CarregaListView();
                    _origChangeItem = false;
                }
                else
                {
                    /* Victor Alves - 31/10/2019 - Processo para cancelar thread se digita varias vezes o item e trava  */
                    Interlocked.Exchange(ref this.throttleCts, new CancellationTokenSource()).Cancel();
                    await Task.Delay(TimeSpan.FromMilliseconds(200), this.throttleCts.Token) // if no keystroke occurs, carry on after 500ms
                        .ContinueWith(
                            delegate { PerformSearch(); }, // Pass the changed text to the PerformSearch function
                            CancellationToken.None,
                            TaskContinuationOptions.OnlyOnRanToCompletion,
                            TaskScheduler.FromCurrentSynchronizationContext());
                }
            }
            catch (Exception ex){

                System.Diagnostics.Debug.Write(ex);
            }
            finally
            {
                _origChangeItem = false;
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

        private void ToolBarVoltar_Clicked(object sender, EventArgs e)
        {
            base.OnBackButtonPressed();
            Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new ArmazenagemPage());
        }

    }    
    public class LocalizacaoSaldoViewModel : INotifyPropertyChanged
    {
        public string CodLocaliz { get;set; }

        public string LocalizView
        {
            get
            {
                if (!String.IsNullOrEmpty(CodLocaliz) && CodLocaliz.Length == 10)
                {
                    string codLocaliz = "";

                    codLocaliz = CodLocaliz.Substring(0, 3) + " - ";
                    codLocaliz = codLocaliz + CodLocaliz.Substring(3, 2) + " - ";
                    codLocaliz = codLocaliz + CodLocaliz.Substring(5, 3) + " - ";
                    codLocaliz = codLocaliz + CodLocaliz.Substring(8, 2);

                    return codLocaliz;
                }
                else
                {
                    return CodLocaliz;
                }
            }
        }


        public string SaldoInfo { get; set; }
   
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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