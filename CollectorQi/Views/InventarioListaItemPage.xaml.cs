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
using System.Threading;
using Rg.Plugins.Popup.Services;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventarioListaItemPage : ContentPage, INotifyPropertyChanged
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

        public ObservableCollection<InventarioItemViewModel> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                OnPropertyChanged("Items");
            }
        }

        private ObservableCollection<InventarioItemViewModel> _Items;
        private ObservableCollection<InventarioItemViewModel> _ItemsFiltered;
        private ObservableCollection<InventarioItemViewModel> _ItemsUnfiltered;

        private InventarioVO _inventario;

        public InventarioListaItemPage(InventarioVO pInventarioVO)
        {
            InitializeComponent();

            _inventario = pInventarioVO;

            this.Title = "Inventário (" + pInventarioVO.__deposito__.Nome + ")";

            var lstInventarioVO = new ObservableCollection<InventarioItemVO>(InventarioItemDB.GetInventarioItemByInventario(_inventario.InventarioId).OrderBy(p => p.ItCodigo).ToList()); ;

            Items = new ObservableCollection<InventarioItemViewModel>();

            for (int i = 0; i < lstInventarioVO.Count; i++)
            {
                var modelView = Mapper.Map<InventarioItemVO, InventarioItemViewModel>(lstInventarioVO[i]);

                Items.Add(modelView);

                /* Adiciona descricao do item para busca */
                /*
                var item = SecurityAuxiliar.ItemAll.Find(p => p.ItCodigo == Items[i].ItCodigo);
                Items[i].__itemDesc__ = item.ItCodigo + item.DescItem; */
            }      

            _ItemsUnfiltered = Items;

            Task.Run(CarregaDescricao);            
                        
            cvInventarioItem.BindingContext = this;                        

        }

        public void CarregaDescricao()
        {
            foreach (var i in _ItemsUnfiltered)
            {
                var item = SecurityAuxiliar.ItemAll.Find(p => p.ItCodigo == i.ItCodigo);
                i.__itemDesc__ = item.ItCodigo + item.DescItem;
            }
        }

        async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cvInventarioItem.IsEnabled = false;
            var pageProgress = new ProgressBarPopUp("Carregando Item..");
            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

            try
            {
                var current = (cvInventarioItem.SelectedItem as InventarioItemViewModel);
                if (current != null)
                {
                    var  page = new InventarioUpdateItemPopUp(_inventario.InventarioId, current.InventarioItemId);

                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page);

                    page.SetResultDigita(resultDigita);
                    
                }                
            }
            catch (Exception  ex)
            {
                await DisplayAlert("Erro!", ex.Message, "OK");
            }
            finally
            {
                cvInventarioItem.IsEnabled = true;                
                await pageProgress.OnClose();
            }
        }

        public void resultDigita (InventarioItemVO byCurrent, bool byQtdDigita)
        {            
            var idxCurrent = Items.Where(p => p.InventarioItemId == byCurrent.InventarioItemId);
            var idxCurrentUnfiltred = _ItemsUnfiltered.Where(p => p.InventarioItemId == byCurrent.InventarioItemId);

            /* Victor Alves - 31/10/2019 - Se os itens nao estiver na lista, filtra pois vai mostrar o qual ele digitou */
            byCurrent.QtdDigitada = byQtdDigita;

            /* Victor Alves - 31/10/2019 - Atualiza itens filtrados */
            if (idxCurrent.Count() > 0)
            {
                idxCurrent.FirstOrDefault().QtdDigitada = byQtdDigita;
                OnPropertyChanged("Items");
            }
            else
            {
                Items.Add(Mapper.Map<InventarioItemVO, InventarioItemViewModel>(byCurrent));
                OnPropertyChanged("Items");
            }

            /* Victor Alves - 31/10/2019 - Atualiza itens não filtrados, se o usuário digitou filtro */
            /* E clicou para aparecer na tela, quando retorna tem que atualizar os dois              */
            if (idxCurrentUnfiltred.Count() > 0)
            {
                idxCurrentUnfiltred.FirstOrDefault().QtdDigitada = byQtdDigita;
                OnPropertyChanged("Items");
            }
            else
            {
                Items.Add(Mapper.Map<InventarioItemVO, InventarioItemViewModel>(byCurrent));
                OnPropertyChanged("Items");
            }
            
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        async void OnClick_NovoItem(object sender, System.EventArgs e)
        {
            /*var page = new InventarioUpdateItemPopUp(_inventarioId, 0);

            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page); */
        }

        async void OnClick_BuscaItem(object sender, System.EventArgs e)
        {
            BtnBuscaItem.IsEnabled = false;
            try
            {
                var customScanPage = new ZXingScannerPage();

                customScanPage.SetResultAction(VerifyProd);

                await Navigation.PushModalAsync(customScanPage);

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "OK");
            }
            finally
            {
                BtnBuscaItem.IsEnabled = true;
            }
        }


        private async void VerifyProd(string strQr)
        {
            if (strQr == null)
                return;

            try
            {
                var mdlEtiqueta = csAuxiliar.GetEtiqueta(strQr);

                var inventarioItem = InventarioItemDB.GetInventarioItemByQr(_inventario.InventarioId, mdlEtiqueta.itCodigo, mdlEtiqueta.lote).FirstOrDefault();

                if (inventarioItem != null)
                {
                    var page = new InventarioUpdateItemPopUp(_inventario.InventarioId, inventarioItem.InventarioItemId);

                    page.SetResultDigita(resultDigita);

                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page);
                }
                else
                {
                    if (_inventario.Contagem == 1)
                    {
                        var result = await DisplayAlert("Atenção!", "Item (" + mdlEtiqueta.itCodigo + ") não encontrado no inventário, deseja adicionar para a digitação?", "Sim", "Não");

                        if (result.ToString() == "True")
                        {
                            var page = new InventarioUpdateItemPopUp(_inventario.InventarioId, 0);

                            page.SetResultDigita(resultDigita);

                            var item = ItemDB.GetItem(mdlEtiqueta.itCodigo);

                            if (item == null)
                                throw new Exception("Item (" + mdlEtiqueta.itCodigo + ") não encontrado.");

                            page.SetNovoItemInventario(mdlEtiqueta.itCodigo, item.DescItem, item.Un, item.__TipoConEst__, mdlEtiqueta.lote, mdlEtiqueta.dtValiLote);

                            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page);
                        }
                    }
                    else
                    {
                        throw new Exception("Item (" + mdlEtiqueta.itCodigo + ") não encontrado no inventário." + Environment.NewLine + 
                                            "Nova ficha (item) no inventário só pode ser criado na contagem (1) e não é permitido a criação de uma nova ficha (item) no inventário (" + _inventario.Contagem.ToString() + ")." );
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "OK");
            } 
        }

        async void OnClick_EfetivaInventario(object sender, System.EventArgs e)
        {
            var action = await DisplayAlert("Efetivar Inventário", "Tem certeza que deseja efetivar o inventário? O inventário será integrado com o TOTVS e não será possivel a alteração.", "Sim", "Não");

            if (action.ToString() != "True")
                return;

            var pageProgress = new ProgressBarPopUp("Efetivando inventário...");

            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

            try
            {
                IntegracaoOnlineBatch.EfetivaInventarioMobile(_inventario.InventarioId);

                OnBackButtonPressed();

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
            Application.Current.MainPage = new NavigationPage(new InventarioListaPage());

            return true;
        }

        private CancellationTokenSource throttleCts = new CancellationTokenSource();

        async void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            try
            {
                //await Task.Run(() => PerformSearch());
                //PerformSearch();
                /* Victor Alves - 31/10/2019 - Processo para cancelar thread se digita varias vezes o item e trava  */
                Interlocked.Exchange(ref this.throttleCts, new CancellationTokenSource()).Cancel();
                await Task.Delay(TimeSpan.FromMilliseconds(100), this.throttleCts.Token) // if no keystroke occurs, carry on after 500ms
                    .ContinueWith(
                        delegate { PerformSearch(); }, // Pass the changed text to the PerformSearch function
                        CancellationToken.None,
                        TaskContinuationOptions.OnlyOnRanToCompletion,
                        TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Erro!", ex.Message, "Cancel");
            }
        }

        async public void PerformSearch()
        {
            try
            {
                //cvInventarioItem.IsEnabled = false;

                if (string.IsNullOrWhiteSpace(SearchBarItCodigo.Text))
                    Items = _ItemsUnfiltered;
                else
                {
                    /*
                    _ItemsFiltered = new ObservableCollection<InventarioItemViewModel>(_ItemsUnfiltered.Where(i =>
                    (i is InventarioItemViewModel && (((InventarioItemViewModel)i).ItCodigo.ToLower().Contains(SearchBarItCodigo.Text.ToLower())))  ||
                    (i is InventarioItemViewModel && (((InventarioItemViewModel)i).__item__.DescItem.ToLower().Contains(SearchBarItCodigo.Text.ToLower())))
                    ));*/

                    /* Victor Alves - 31/10/2019 - Melhoria de performance Item */
                     _ItemsFiltered = new ObservableCollection<InventarioItemViewModel>(_ItemsUnfiltered.Where(i =>
                    (i is InventarioItemViewModel && (((InventarioItemViewModel)i).__itemDesc__.ToLower().Contains(SearchBarItCodigo.Text.ToLower())))
                    )); 

                    /*
                    _ItemsFiltered = new ObservableCollection<InventarioItemViewModel>(_ItemsUnfiltered.Where(i =>
                    (i is InventarioItemViewModel && (((InventarioItemViewModel)i).__item__.ItCodigo.ToLower().Contains(SearchBarItCodigo.Text.ToLower()))) ||
                     i is InventarioItemViewModel && (((InventarioItemViewModel)i).__item__.DescItem.ToLower().Contains(SearchBarItCodigo.Text.ToLower())))
                    );*/

                    Items = _ItemsFiltered;
                }                
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancel");
            }
            finally
            {
                //cvInventarioItem.IsEnabled = true;
            }
        }        
    }

    public class InventarioItemViewModel : InventarioItemVO, INotifyPropertyChanged
    {
        public string Image
        {
            get
            {
                if (!this.QtdDigitada)
                {
                    return "intPendenteMed.png";
                }
                else
                {
                    return "intSucessoMed.png";

                }            

            }
        }

        public bool QtdDigitada
        {
            get
            {
                return base.QtdDigitada;
            }
            set
            {
                base.QtdDigitada = value;
                OnPropertyChanged("Image");
                OnPropertyChanged("StatusItemInventario");
            }
        }

        public string StatusItemInventario
        {
            get
            {
                if (!this.QtdDigitada)
                {
                    return "Não Digitado";
                }
                else
                {
                    return "Digitado";
                }
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
