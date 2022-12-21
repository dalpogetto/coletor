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
    public partial class SaldoVirtualListaPage : ContentPage, INotifyPropertyChanged
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

        public ObservableCollection<LocalizacaoViewModel> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                OnPropertyChanged("Items");
            }
        }

        private ObservableCollection<LocalizacaoViewModel> _Items;
        private ObservableCollection<LocalizacaoViewModel> _ItemsFiltered;
        private ObservableCollection<LocalizacaoViewModel> _ItemsUnfiltered;

        public Deposito _deposito { get; set; }

        public SaldoVirtualListaPage(Deposito pDeposito)
        {
            InitializeComponent();

            lblCodEstabel.Text = "Estab: " + SecurityAuxiliar.Estabelecimento + " / Depos: " + pDeposito.CodDepos ;

            _deposito = pDeposito;

            cvLocalizacoes.BindingContext = this;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            Items = new ObservableCollection<LocalizacaoViewModel>();

            CarregaListView();
        }

        private async void CarregaListView()
        {
            //var lstInventario = await ParametersInventarioService.SendParametersAsync();
            var pageProgress = new ProgressBarPopUp("Carregando Localização, aguarde...");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                var lstLocalizacao = await Localizacao.ObterLocalizPorEstabDepService(_deposito.CodDepos);

                System.Diagnostics.Debug.Write(lstLocalizacao);
                
                if (lstLocalizacao != null && lstLocalizacao.param != null && lstLocalizacao.param.Resultparam != null && lstLocalizacao.param.Resultparam.Count > 0)
                {
                    foreach (var row in lstLocalizacao.param.Resultparam)
                    {
                        Items.Add(new LocalizacaoViewModel { CodLocaliz = row.CodLocaliz });
                    }
                }

                SearchBarCodLocaliz.Focus();

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
        
        async void cvLocalizacoes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

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
            }
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
                if (string.IsNullOrWhiteSpace(SearchBarCodLocaliz.Text))
                    Items = _ItemsUnfiltered;
                else
                {
                    /* Victor Alves - 31/10/2019 - Melhoria de performance Item */
                    _ItemsFiltered = new ObservableCollection<LocalizacaoViewModel>(_ItemsUnfiltered.Where(i =>
                   (i is LocalizacaoViewModel && (((LocalizacaoViewModel)i).CodLocaliz.ToLower().Contains(SearchBarCodLocaliz.Text.ToLower())))
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

        async void Handle_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
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
            catch { }
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
    public class LocalizacaoSaldoViewModel : INotifyPropertyChanged
    {
        public string CodLocaliz { get;set; }
   
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