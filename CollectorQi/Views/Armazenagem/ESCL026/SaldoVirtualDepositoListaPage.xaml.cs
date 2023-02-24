using AutoMapper;
using CollectorQi.Models.ESCL000;
using CollectorQi.Models.ESCL021;
using CollectorQi.Models.ESCL025;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL000;
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
    public partial class SaldoVirtualDepositoListaPage : ContentPage, INotifyPropertyChanged
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

        public ObservableCollection<DepositosViewModel> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                OnPropertyChanged("Items");
            }
        }

        private ObservableCollection<DepositosViewModel> _Items;
        private ObservableCollection<DepositosViewModel> _ItemsFiltered;
        private ObservableCollection<DepositosViewModel> _ItemsUnfiltered;

        private string _codDepos;
        public SaldoVirtualDepositoListaPage(string pCodDepos)
        {
            InitializeComponent();

            lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.Estabelecimento;

            cvDepositos.BindingContext = this;

            _codDepos = pCodDepos;
        }
        protected async override void OnAppearing()
        {
            base.OnAppearing();

            Items = new ObservableCollection<DepositosViewModel>();

            CarregaListView();
        }
        private async void CarregaListView()
        {
            var pageProgress = new ProgressBarPopUp("Carregando Depósitos, aguarde...");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                var lstDeposito = await DepositosGuardaMaterialService.SendGuardaMaterialAsync( /* parametrosDepositosGuardaMaterial.TipoTransferencia */ );

                Items.Clear();

                if (lstDeposito != null && lstDeposito.Param != null && lstDeposito.Param.ParamResult != null)
                {
                    foreach (var item in lstDeposito.Param.ParamResult)
                    {
                        Items.Add(new DepositosViewModel
                        {
                            CodDepos = item.CodDepos,
                            NomeDepos = item.Nome
                        });
                    }
                }

                SearchBarCodDepos.Focus();

                _ItemsUnfiltered = Items;

                OnPropertyChanged("Items");

                
                if (!String.IsNullOrEmpty(_codDepos) && Items != null)
                {
                    var currentDepos = Items.FirstOrDefault(x => x.CodDepos == _codDepos);

                    cvDepositos.SelectedItem = currentDepos;
                }

                if (Items != null && Items.Count == 1)
                {
                    var currentDepos = Items[0];

                    cvDepositos.SelectedItem = currentDepos;
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

        SaldoVirtualLocalizPopUp pageLocaliz = null;

        async void cvDepositos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                cvDepositos.IsEnabled = false;

                if (cvDepositos.SelectedItem == null)
                    return;

                var current = (cvDepositos.SelectedItem as DepositosViewModel);

                pageLocaliz = new SaldoVirtualLocalizPopUp(current.CodDepos);
                pageLocaliz._confirmaLocalizacaoItem = CodigoBarras;
                await PopupNavigation.Instance.PushAsync(pageLocaliz);

                cvDepositos.SelectedItem = null;
            }
            catch (Exception ex)
            {
                await DisplayAlert("ERRO", ex.Message, "OK");
            }
            finally
            {
                cvDepositos.IsEnabled = true;
            }
        }

        public void CodigoBarras(string pCodDepos, string pCodBarras)
        {
            if (pageLocaliz != null && pageLocaliz.IsVisible)
            {
                pageLocaliz.OnClose();
            }

            if (pCodBarras.Substring(0, 3) == "10;")
            {
                Application.Current.MainPage = new NavigationPage(new SaldoVirtualItemListaPage(pCodDepos, pCodBarras.Replace("10;", "").Trim()));
            }
            else
            {
                var splItem = pCodBarras.Split(';');
                string strCodItem;

                if (splItem.Length > 1)
                {
                    strCodItem = splItem[1];
                }
                else
                {
                    strCodItem = pCodBarras;
                }

                if (!String.IsNullOrEmpty(strCodItem))
                {
                    Application.Current.MainPage = new NavigationPage(new SaldoVirtualLocalizacaoListaPage(pCodDepos, strCodItem));
                }
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
                if (string.IsNullOrWhiteSpace(SearchBarCodDepos.Text))
                    Items = _ItemsUnfiltered;
                else
                {
                    /* Victor Alves - 31/10/2019 - Melhoria de performance Item */
                    _ItemsFiltered = new ObservableCollection<DepositosViewModel>(_ItemsUnfiltered.Where(i =>
                   (i is DepositosViewModel && (((DepositosViewModel)i).CodDeposNome.ToLower().Contains(SearchBarCodDepos.Text.ToLower())))
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
        private void ToolBarVoltar_Clicked(object sender, EventArgs e)
        {
            base.OnBackButtonPressed();
            Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new ArmazenagemPage());
        }

    }    
    public class DepositosSaldoViewModel : Deposito, INotifyPropertyChanged
    {
        public string CodDeposNome
        {
            get { return CodDepos + " - " + NomeDepos; }
        }
   
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