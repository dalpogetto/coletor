using Android.App;
using Android.Views;
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
    public partial class DepositosUsuarioPorTransacaoListaPopUp : PopupPage  , INotifyPropertyChanged 
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

        public CustomEntryText _edtCodDepos;
        public string _tipoTransacao;
        public Action<string,string, bool> _setDepos;
        public Action _backGround;

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

            //cvDepositosUsuarioTransacao.Focus();
            //SearchBarCodDepos.IsReadOnly = false;
            //SearchBarCodDepos.IsEnabled = true;
           // SearchBarCodDepos.IsReadOnly = false;
        }

        private async void CarregaListView()
        {
            try
            {
                var dGuardaMaterialRetorno = await DepositosGuardaMaterialService.SendGuardaMaterialAsync();

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

                _ItemsUnfiltered = Items;

                OnPropertyChanged("Items");

                //await Task.Run(async () =>
                //{
                //    await Task.Delay(500);
                //    Device.BeginInvokeOnMainThread(async () =>
                //    {
                //        SearchBarCodDepos.IsEnabled = true;
                //
                //    });
                //});
            }
            finally
            {
            }
        }

        protected override bool OnBackButtonPressed()
        {
            PopupNavigation.Instance.PopAsync();
            
            return true;
        }

        void cvDepositosUsuarioTransacao_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var current = (cvDepositosUsuarioTransacao.SelectedItem as DepositosUsuarioPorTransacaoViewModel);

            if (current != null)
            {
                _edtCodDepos.Text = current.CodDepos + " " + current.Nome;
                _setDepos(current.CodDepos, _tipoTransacao, current.DepLab);
            }

            PopupNavigation.Instance.PopAsync();
        }

       // private CancellationTokenSource throttleCts = new CancellationTokenSource();
       // async void Handle_TextChanged(object sender, TextChangedEventArgs e)
       // {
       //     try
       //     {
       //       Interlocked.Exchange(ref this.throttleCts, new CancellationTokenSource()).Cancel();
       //         await Task.Delay(TimeSpan.FromMilliseconds(500), this.throttleCts.Token) // if no keystroke occurs, carry on after 500ms
       //             .ContinueWith(
       //                 delegate { PerformSearch(); }, // Pass the changed text to the PerformSearch function
       //                 CancellationToken.None,
       //                 TaskContinuationOptions.OnlyOnRanToCompletion,
       //                 TaskScheduler.FromCurrentSynchronizationContext());
       //     }
       //     catch (Exception ex)
       //     {
       //     }
       // }
        //async public void PerformSearch()
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(SearchBarCodDepos.Text))
        //            Items = _ItemsUnfiltered;
        //        else
        //        {
        //            _ItemsFiltered = new ObservableCollection<DepositosUsuarioPorTransacaoViewModel>(_ItemsUnfiltered.Where(i =>
        //           (i is DepositosUsuarioPorTransacaoViewModel && (((DepositosUsuarioPorTransacaoViewModel)i).CodDeposNome.ToLower().Contains(SearchBarCodDepos.Text.ToLower())))
        //           ));
        //
        //            Items = _ItemsFiltered;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert("Erro!", ex.Message, "Cancel");
        //    }
        //    finally
        //    {
        //    }
        //}
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
    public interface IKeyboardService
    {
        event EventHandler KeyboardIsShown;
        event EventHandler KeyboardIsHidden;
       // void showSoftKeyboard(View v);
    }
}