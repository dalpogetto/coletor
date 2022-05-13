using CollectorQi.Models;
using CollectorQi.ViewModels;
using CollectorQi.Resources.DataBaseHelper;
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

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RequisicaoDevListaItemPage : ContentPage, INotifyPropertyChanged
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

        //public ObservableCollection<InventarioItemVO> ObsInventarioItem { get; }

        public ObservableCollection<RequisicaoItemViewModel> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                OnPropertyChanged("Items");
            }
        }

        private ObservableCollection<RequisicaoItemViewModel> _Items;
        private ObservableCollection<RequisicaoItemViewModel> _ItemsFiltered;
        private ObservableCollection<RequisicaoItemViewModel> _ItemsUnfiltered;

        private RequisicaoItemViewModel _currentClick;

        private RequisicaoVO _requisicao;
        private bool _isDevolucao = false;

        public RequisicaoDevListaItemPage(RequisicaoVO pRequisicaoVO, bool pIsDevolucao)
        {
            InitializeComponent();

            _requisicao = pRequisicaoVO;
            _isDevolucao = pIsDevolucao;

            this.Title = "Requisição (" + pRequisicaoVO.NrRequisicao + ")";

            cvRequisicaoItem.BindingContext = this; 
        }

        private async void AtualizaCv()
        {
            var pageProgress = new ProgressBarPopUp("");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                var lstRequisicaoItemVO = new ObservableCollection<RequisicaoItemVO>(await RequisicaoItemDB.GetRequisicaoItemByRequisicao(_requisicao.NrRequisicao, _isDevolucao));

                Items = new ObservableCollection<RequisicaoItemViewModel>();

                for (int i = 0; i < lstRequisicaoItemVO.Count; i++)
                {
                    var modelView = Mapper.Map<RequisicaoItemVO, RequisicaoItemViewModel>(lstRequisicaoItemVO[i]);

                    Items.Add(modelView);
                }

                _ItemsUnfiltered = Items;
            }
            finally
            {
                await pageProgress.OnClose();
            }
        }

        protected async override void OnAppearing()
        {
            AtualizaCv();
        }

        async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var current = (cvRequisicaoItem.SelectedItem as RequisicaoItemViewModel);

            if (current != null)
            {
                _currentClick = current;

                // Executa QR Code
                var customScanPage = new ZXingScannerPage();

                customScanPage.SetResultAction(VerifyProd);

                customScanPage.OpenBuscaItem = BuscaItem;

                await Navigation.PushModalAsync(customScanPage);
            }
            cvRequisicaoItem.SelectedItem = null;
        }

        public async void BuscaItem()
        {
            var result = await DisplayAlert("Item Requisição", "Deseja informar o item manualmente?", "Sim", "Não");

            if (result)
            {
                var page = new ItemPopUp(null, null, null, null, VerifyProd2);

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page);
            }
        }

        private async void VerifyProd2(string pItCodigo)
        {
            try
            {
                if (pItCodigo == _currentClick.ItCodigo)
                {

                    /*
                    // Abre tela para digitar item após confirmar o ITEM
                    _currentClick.QtDevolvida = _currentClick.QtaDevolver;

                    //RequisicaoItemVO teste = (RequisicaoItemVO)_currentClick as RequisicaoItemVO;

                    //var requisicaoItem = _currentClick.Base



                    RequisicaoItemVO modelView = Mapper.Map<RequisicaoItemViewModel, RequisicaoItemVO>(_currentClick);

                    await RequisicaoItemDB.AtualizaRequisicaoItem(modelView); 

                    AtualizaCv(); */
                    // Abre tela para digitar item após confirmar o ITEM
                    await Navigation.PushModalAsync(new RequisicaoListaItemSaldoEstoqPage(_currentClick, _isDevolucao));

                    AtualizaCv();

                }
                else
                {
                    await DisplayAlert("Erro!", "Item diferente da requisição!", "Cancelar");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
        }

        private void VerifyProd(string strQr)
        {
            try
            {
                ModelEtiqueta _mdlEtiqueta = null;

                if (strQr == null)
                    return;

                _mdlEtiqueta = csAuxiliar.GetEtiqueta(strQr);

                if (_mdlEtiqueta != null)
                {
                    if (_mdlEtiqueta.itCodigo == _currentClick.ItCodigo)
                    {
                        Navigation.PushModalAsync(new RequisicaoListaItemSaldoEstoqPage(_currentClick, _isDevolucao));
                    }
                    else
                    {
                        DisplayAlert("Erro!", "Item diferente da requisição!", "Cancelar");
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
        }

        async void OnClick_DevolveRequisicao(object sender, System.EventArgs e)
        {
            var action = await DisplayAlert("Devolução de Requisição", "Tem certeza que deseja devolver as requisições?", "Sim", "Não");

            if (action.ToString() != "True")
                return;

            var pageProgress = new ProgressBarPopUp("Devolvendo requisição...");

            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

            try
            {
                var tplRet = await IntegracaoOnlineBatch.AtendeDevolveRequisicao(_requisicao.NrRequisicao, _isDevolucao);

                if (tplRet.Item1 == TipoIntegracao.IntegracaoOnline)
                    OnBackButtonPressed();
                else
                {
                    throw new Exception(tplRet.Item2);
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

        protected override bool OnBackButtonPressed()
        {

            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new RequisicaoListaPage(_isDevolucao)); 

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
                if (string.IsNullOrWhiteSpace(SearchBarItCodigo.Text))
                    Items = _ItemsUnfiltered;
                else
                {
                    _ItemsFiltered = new ObservableCollection<RequisicaoItemViewModel>(_ItemsUnfiltered.Where(i =>
                    (i is RequisicaoItemViewModel && (((RequisicaoItemViewModel)i).ItCodigo.ToLower().Contains(SearchBarItCodigo.Text.ToLower()))) ||
                    (i is RequisicaoItemViewModel && (((RequisicaoItemViewModel)i).__item__.DescItem.ToLower().Contains(SearchBarItCodigo.Text.ToLower())))
                    ));

                    Items = _ItemsFiltered;

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancel");
            }
        }
    }


    public class RequisicaoItemViewModel : RequisicaoItemVO, INotifyPropertyChanged
    {

        /*private decimal? _deQtdDevolve = null;


        public Decimal sdoQtdDevolve
        {
            get
            {
                decimal deQtdDevolve = 0;

                if (_deQtdDevolve == null)
                {
                    RequisicaoItemSaldoEstoqDB.GetRequisicaoItemSaldoEstoq(this.NrRequisicao, this.Sequencia, this.ItCodigo, true);
                }
            }

            [Ignore]
            public ItemVO __item__
            {
                get
                {
                    if (item == null)
                    {
                        item = ItemDB.GetItem(itCodigo);
                    }
                    return item;
                }
            }

        }
        */


        public string Image
        {
            get
            {
                if (this.QtDevolvidaMobile > 0) 
                    return "intSucessoMed.png";

                return string.Empty;   
            }
        }


        public string StatusItemDevolucao
        {
            get
            {
                if (this.QtDevolvidaMobile > 0)
                    return "Devolver (" + this.QtDevolvidaMobile + ")";

                return string.Empty;
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