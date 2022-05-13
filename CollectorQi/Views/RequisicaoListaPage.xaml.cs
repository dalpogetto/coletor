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

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RequisicaoListaPage : ContentPage, INotifyPropertyChanged
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

        public ObservableCollection<RequisicaoVO> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                OnPropertyChanged("Items");
            }
        }

        private ObservableCollection<RequisicaoVO> _Items;
        private ObservableCollection<RequisicaoVO> _ItemsFiltered;
        private ObservableCollection<RequisicaoVO> _ItemsUnfiltered;

        private bool _isDevolucao;

        public RequisicaoListaPage(bool pIsDevolucao)
        {
            InitializeComponent();

            Items = new ObservableCollection<RequisicaoVO>();

            //lblCodEstabel.Text = SecurityAuxiliar.Estabelecimento;

            _isDevolucao = pIsDevolucao;

            AtualizaCv();

            cvRequisicao.BindingContext = this;
        }

        private async void AtualizaCv()
        {
            await CarregaRequisicao();

            var pageProgress = new ProgressBarPopUp("");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                //ObsRequisicao.Clear();

                var lstRequisicaoAsync = await RequisicaoDB.GetRequisicao(_isDevolucao);

                var lstRequisicao = lstRequisicaoAsync.OrderBy(p => p.NrRequisicao).ToList();

                Items.Clear();

                for (int i = 0; i < lstRequisicao.Count(); i++)
                {
                    Items.Add(lstRequisicao[i]);
                }
                _ItemsUnfiltered = Items;
            }
            finally
            {
                await pageProgress.OnClose();
            }
        }

        void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var current = (cvRequisicao.SelectedItem as VO.RequisicaoVO);

            if (current != null)
            {
                if (_isDevolucao)
                    Application.Current.MainPage = new NavigationPage(new RequisicaoDevListaItemPage(current, _isDevolucao));
                else
                    Application.Current.MainPage = new NavigationPage(new RequisicaoListaItemPage(current, _isDevolucao));
            }

            cvRequisicao.SelectedItem = null; 
        }

        async Task CarregaRequisicao()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                BtnCarregaRequisicao.IsEnabled = false;

                var pageProgress = new ProgressBarPopUp("Carregando requisição, aguarde...");

                try
                {
                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                    var tplLstRequisicaoERP = Models.Controller.GetRequisicao();

                    await Models.Controller.CriaRequisicao(tplLstRequisicaoERP.Item1, tplLstRequisicaoERP.Item2, tplLstRequisicaoERP.Item3); 

                    var lstRequisicaoAsync = await RequisicaoDB.GetRequisicao(_isDevolucao);

                    var lstRequisicao = lstRequisicaoAsync.OrderBy(p => p.NrRequisicao).ToList();

                    Items.Clear();

                    for (int i = 0; i < lstRequisicao.Count(); i++)
                    {
                        //    var modelView = Mapper.Map<RequisicaoVO, RequisicaoViewModel>(lstRequisicao[i]);

                        Items.Add(lstRequisicao[i]);
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erro!", ex.Message, "Cancelar");
                }
                finally
                {
                    BtnCarregaRequisicao.IsEnabled = true;
                    await pageProgress.OnClose();
                }
            }
        }

        async void OnClick_CarregaRequisicao(object sender, System.EventArgs e)
        {

            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Erro!", "Para buscar as requisiçẽos no sistema o dispositivo deve estar conectado.", "CANCELAR");

                return;
            }

            await CarregaRequisicao();   
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new AlmoxarifadoPage());

            return true;
        }

        async void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            await Task.Run(() => PerformSearch());
        }

        async public void PerformSearch()
        {

            try
            {
                if (string.IsNullOrWhiteSpace(SearchNrRequisicao.Text))
                    Items = _ItemsUnfiltered;
                else
                {
                    _ItemsFiltered = new ObservableCollection<RequisicaoVO>(_ItemsUnfiltered.Where(i =>
                    (i is RequisicaoVO && (((RequisicaoVO)i).NrRequisicao.ToString().ToLower().Contains(SearchNrRequisicao.Text.ToLower())))));

                    Items = _ItemsFiltered;

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancel");
            }
        }
    }
}