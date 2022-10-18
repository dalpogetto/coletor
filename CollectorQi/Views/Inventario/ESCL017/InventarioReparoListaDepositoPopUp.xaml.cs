using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Rg.Plugins.Popup.Pages;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CollectorQi.Resources.DataBaseHelper;
using Rg.Plugins.Popup.Services;
using CollectorQi;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL017;
using CollectorQi.Models.ESCL017;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace CollectorQi.Views
{


    public partial class InventarioReparoListaDepositoPopUp : PopupPage, INotifyPropertyChanged 
    {
     
        #region Property

        new public event PropertyChangedEventHandler PropertyChanged;

        new protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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

        private int _rating;
        public int Rating
        {
            get { return _rating; }
            set => SetProperty(ref _rating, value);
        }

        #endregion

        public ObservableCollection<DepositosInventarioPopUpReparoViewModel> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                OnPropertyChanged("Items");
            }
        }

        private ObservableCollection<DepositosInventarioPopUpReparoViewModel> _Items;
        private ObservableCollection<DepositosInventarioPopUpReparoViewModel> _ItemsFiltered;
        private ObservableCollection<DepositosInventarioPopUpReparoViewModel> _ItemsUnfiltered;

        CustomEntry _edtCodDepos;
        public Action<string> _setDepos;

        public InventarioReparoListaDepositoPopUp(CustomEntry pCodDepos)
        {
            InitializeComponent();

            _edtCodDepos = pCodDepos;
            //_codDeposHidden = pCodDeposHidden;

           cvDeposito.BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Items = new ObservableCollection<DepositosInventarioPopUpReparoViewModel>();

            CarregaListView();
        }

        private async void CarregaListView()
        {
            var pageProgress = new ProgressBarPopUp("Carregando Depósito...");

            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

            var dInventarioRetorno = await DepositoInventarioReparoService.SendParametersAsync();

            Items.Clear();
            
            if (dInventarioRetorno != null && dInventarioRetorno.Param != null && dInventarioRetorno.Param.Resultparam != null)
            {
                foreach (var item in dInventarioRetorno.Param.Resultparam)
                {
                    Items.Add(new DepositosInventarioPopUpReparoViewModel
                    {
                        CodDepos = item.CodDepos,
                        Nome = item.Nome
                    });
                }
            }

            SearchBarCodDepos.Focus();

            _ItemsUnfiltered = Items;

            OnPropertyChanged("Items");

            await pageProgress.OnClose();
        }

        void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var current = (cvDeposito.SelectedItem as DepositosInventarioPopUpReparoViewModel);

            if (current != null)
            {
                _edtCodDepos.Text = current.CodDepos + " " + current.Nome;
                //_codDeposHidden = current.CodDepos;
                _setDepos(current.CodDepos);
            }
           
            PopupNavigation.Instance.PopAsync();
        }


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
            }
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
                    _ItemsFiltered = new ObservableCollection<DepositosInventarioPopUpReparoViewModel>(_ItemsUnfiltered.Where(i =>
                   (i is DepositosInventarioPopUpReparoViewModel && (((DepositosInventarioPopUpReparoViewModel)i).CodDeposNome.ToLower().Contains(SearchBarCodDepos.Text.ToLower())))
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
            }
        }

    }

    public class DepositosInventarioPopUpReparoViewModel : DepositosInventarioReparo, INotifyPropertyChanged
    {
        public string CodDeposNome
        {
            get
            {
                return CodDepos + " " + Nome;
            }
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
