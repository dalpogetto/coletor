﻿using AutoMapper;
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
    public partial class ConsultaLocalizacaoItemListaPage : ContentPage, INotifyPropertyChanged
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

        public ObservableCollection<ItemViewModel> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                OnPropertyChanged("Items");
            }
        }

        private ObservableCollection<ItemViewModel> _Items;
        private ObservableCollection<ItemViewModel> _ItemsFiltered;
        private ObservableCollection<ItemViewModel> _ItemsUnfiltered;

        public string _deposito { get; set; }
        public string _codLocaliz { get; set; }

        public ConsultaLocalizacaoItemListaPage(string pDeposito, string pCodLocaliz)
        {
            InitializeComponent();

            lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.Estabelecimento;

            _deposito = pDeposito;
            _codLocaliz = pCodLocaliz;

            cvItem.BindingContext = this;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            Items = new ObservableCollection<ItemViewModel>();

            CarregaListView(false);
        }

        private async void CarregaListView(bool pSemSaldo)
        {
            //var lstInventario = await ParametersInventarioService.SendParametersAsync();
            var pageProgress = new ProgressBarPopUp("Carregando Itens, aguarde...");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                var lstItem = await Item.ObterLocalizDoItemPorEstabDep(_deposito, _codLocaliz, pSemSaldo);

                if (lstItem != null && lstItem.param != null && lstItem.param.Resultparam != null && lstItem.param.Resultparam.Count > 0)
                {
                    foreach (var row in lstItem.param.Resultparam)
                    {
                        Items.Add(new ItemViewModel { 
                            CodigoItem = row.CodigoItem, 
                            SaldoInfo  = row.SaldoInfo.ToString() 
                        }) ;
                    }
                }

                SearchBarCodDepos.Focus();

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
        
        async void cvItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            try
            {
                /*
                cvItem.IsEnabled = false;

                if (cvItem.SelectedItem == null)
                    return;

                var current = (cvItem.SelectedItem as ItemViewModel);

                var page = new ConsultaLocalizacaoListaPage(current);
                await PopupNavigation.Instance.PushAsync(page);

                cvItem.SelectedItem = null; 
                */
            }
            catch (Exception ex)
            {
                await DisplayAlert("ERRO", ex.Message, "OK");
            }
            finally
            {
                cvItem.IsEnabled = true;
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
                    _ItemsFiltered = new ObservableCollection<ItemViewModel>(_ItemsUnfiltered.Where(i =>
                   (i is ItemViewModel && (((ItemViewModel)i).CodigoItem.ToLower().Contains(SearchBarCodDepos.Text.ToLower())))
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

    public class ItemViewModel :  INotifyPropertyChanged
    {
        public string CodigoItem { get; set; }
        public string SaldoInfo { get; set; }
   
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