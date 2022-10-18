using AutoMapper;
using CollectorQi.Models.ESCL021;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL021;
using Rg.Plugins.Popup.Pages;
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
    public partial class DepositosUsuarioPorTransacaoListaPopUp : PopupPage, INotifyPropertyChanged
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

        public ObservableCollection<DepositosUsuarioPorTransacaoViewModel> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                OnPropertyChanged("Items");
            }
        }

        private ObservableCollection<DepositosUsuarioPorTransacaoViewModel> _Items;
        private ObservableCollection<DepositosUsuarioPorTransacaoViewModel> _ItemsFiltered;
        private ObservableCollection<DepositosUsuarioPorTransacaoViewModel> _ItemsUnfiltered;

        CustomEntryText _edtCodDepos;
        public Action<string,string, bool> _setDepos;
        string _tipoTransacao;

        public DepositosUsuarioPorTransacaoListaPopUp(CustomEntryText pCodDepos, string pTipoTransacao)
        {
            InitializeComponent();

            _edtCodDepos = pCodDepos;
            _tipoTransacao = pTipoTransacao;

            cvDepositosUsuarioTransacao.BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Items = new ObservableCollection<DepositosUsuarioPorTransacaoViewModel>();

            CarregaListView();
        }

        private async void CarregaListView()
        {
            var pageProgress = new ProgressBarPopUp("Carregando Depósito...");

            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

            var dGuardaMaterialRetorno = await DepositosGuardaMaterialService.SendGuardaMaterialAsync( /* parametrosDepositosGuardaMaterial.TipoTransferencia */ );
          
            Items.Clear();

            if (dGuardaMaterialRetorno != null && dGuardaMaterialRetorno.Param != null && dGuardaMaterialRetorno.Param.ParamResult != null)
            {
                foreach (var item in dGuardaMaterialRetorno.Param.ParamResult)
                {
                    Items.Add(new DepositosUsuarioPorTransacaoViewModel
                    {
                        CodDepos = item.CodDepos,
                        Nome = item.Nome,
                        DepLab = item.DepLab
                    });
                }
            }

            SearchBarCodDepos.Focus();

            _ItemsUnfiltered = Items;

            OnPropertyChanged("Items");

            await pageProgress.OnClose();
        }

        void cvDepositosUsuarioTransacao_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var current = (cvDepositosUsuarioTransacao.SelectedItem as DepositosUsuarioPorTransacaoViewModel);

            if (current != null)
            {
                _edtCodDepos.Text = current.CodDepos + " " + current.Nome;
                //_codDeposHidden = current.CodDepos;
                _setDepos(current.CodDepos, _tipoTransacao, current.DepLab);
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
                    _ItemsFiltered = new ObservableCollection<DepositosUsuarioPorTransacaoViewModel>(_ItemsUnfiltered.Where(i =>
                   (i is DepositosUsuarioPorTransacaoViewModel && (((DepositosUsuarioPorTransacaoViewModel)i).CodDeposNome.ToLower().Contains(SearchBarCodDepos.Text.ToLower())))
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

    public class DepositosUsuarioPorTransacaoViewModel : DepositosGuardaMaterial
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