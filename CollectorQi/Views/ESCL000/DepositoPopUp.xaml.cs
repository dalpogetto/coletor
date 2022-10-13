using AutoMapper;
using CollectorQi.Models.ESCL018;
using CollectorQi.Models.ESCL021;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Services.ESCL000;
using CollectorQi.Services.ESCL018;
using CollectorQi.ViewModels;
using CollectorQi.VO.ESCL018;
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
//using Android.Graphics;

namespace CollectorQi.Views
{
    public partial class DepositoPopUp : PopupPage, INotifyPropertyChanged
    {
        //public Action<string> _confirmaItem { get; set; }

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

        public ObservableCollection<DepositoViewModel> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                OnPropertyChanged("Items");
            }
        }

        public ObservableCollection<DepositoViewModel> _Items;

        CustomEntry _edtDeposito;

        public DepositoPopUp(CustomEntry edtDeposito)        
        {
            try
            {
                InitializeComponent();

                _edtDeposito = edtDeposito;

                cvDepositos.BindingContext = this;

            }
            catch (Exception ex)
            {
                throw;
            } 
        }

        void OnClick_Cancelar(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }
        protected async override void OnAppearing()
        {

            var lstDeposito = await CadastrosDeposito.ObterListaDepositos();


            //Items = new ObservableCollection<Models.ESCL000.Deposito>(lstDeposito);

            Items = new ObservableCollection<DepositoViewModel>();

            foreach (var row in lstDeposito)
            {
                Items.Add(new DepositoViewModel
                {
                    CodDepos = row.CodDepos,
                    NomeDepos = row.NomeDepos
                });
            }

            OnPropertyChanged("Items");

            //  try
            //  {
            //
            //  }
            // await Task.Run(async () =>
            // {
            //     await Task.Delay(100);
            //     Device.BeginInvokeOnMainThread(async () =>
            //     {
            //         edtCodigoBarras.Focus();
            //     });
            // });
        }
        protected override bool OnBackButtonPressed()
        {            
            PopupNavigation.Instance.PopAsync();
            return true;
        }

        async void OnClick_Confirmar(object sender, EventArgs e)
        {
           // try
           // {
           //     BtnEfetivar.IsEnabled = false;
           //     await PopupNavigation.Instance.PopAsync();
           //     _confirmaItem(edtCodigoBarras.Text);
           // }
           // finally
           // {
           //     BtnEfetivar.IsEnabled = true;
           // }
        }

        private async void cvDepositos_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                cvDepositos.IsEnabled = false;

                if (cvDepositos.SelectedItem == null)
                    return;

                var current = (cvDepositos.SelectedItem as DepositoViewModel);

                _edtDeposito.Text = current.CodDepos;

                await PopupNavigation.Instance.PopAsync();

                //var pageProgress = new ProgressBarPopUp("Carregando...");

                //var page = new GuardaMateriasDepositoLocalizPopUp(null, current);
                //page._confirmaLocalizacao = ConfirmaLocalizacao;
                //await PopupNavigation.Instance.PushAsync(page);
                ////await pageProgress.OnClose();
                //
                //cvDepositosGuardaMaterial.SelectedItem = null;
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
    }

    public class DepositoViewModel : Models.ESCL000.Deposito, INotifyPropertyChanged
    {
        public string CodDeposNome
        {
            get { return CodDepos + " - " + NomeDepos; }
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