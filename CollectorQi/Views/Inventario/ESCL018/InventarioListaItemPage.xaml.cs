using CollectorQi.Models;
using CollectorQi.ViewModels;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Resources;
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
using CollectorQi.Services.ESCL018;
using CollectorQi.Models.ESCL018;
using CollectorQi.Resources.DataBaseHelper.ESCL018;
using CollectorQi.VO.ESCL018;

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
        private string _codLocaliz { get; set; }
        private List<InventarioItemVO> _lstInventarioItemVO;

        public InventarioListaItemPage(InventarioVO pInventarioVO , string pCodLocaliz)
        {
            InitializeComponent();

            _codLocaliz = pCodLocaliz;
            _inventario = pInventarioVO;
            cvInventarioItem.BindingContext = this;

            this.Title = "Digitação de Item";

            lblCodLocalizacao.Text = $"Depósito / Localização: {_inventario.CodDepos} / {_codLocaliz}";

        }

        async void OnClick_CaixaIncompleta(object sender, EventArgs e)
        {
            /*
            var pageProgress = new ProgressBarPopUp("Carregando...");
            var page = new InventarioCaixaIncompletaPopUp(_inventario.IdInventario, null);
            await PopupNavigation.Instance.PushAsync(page);
            await pageProgress.OnClose();
            */

        }

        public void resultDigita(InventarioItemVO byCurrent, decimal byQtdDigita)
        {
            var idxCurrent = Items.Where(p => p.InventarioItemId == byCurrent.InventarioItemId);
            var idxCurrentUnfiltred = _ItemsUnfiltered.Where(p => p.InventarioItemId == byCurrent.InventarioItemId);

            /* Victor Alves - 31/10/2019 - Se os itens nao estiver na lista, filtra pois vai mostrar o qual ele digitou */
            byCurrent.Quantidade = byQtdDigita;

            /* Victor Alves - 31/10/2019 - Atualiza itens filtrados */
            if (idxCurrent.Count() > 0)
            {
                //idxCurrent.FirstOrDefault().QtdDigitada = byQtdDigita;
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
                //idxCurrentUnfiltred.FirstOrDefault().QtdDigitada = byQtdDigita;
                OnPropertyChanged("Items");
            }
            else
            {
                Items.Add(Mapper.Map<InventarioItemVO, InventarioItemViewModel>(byCurrent));
                OnPropertyChanged("Items");
            }
        }

        private async void CarregaListView()
        {
            var pageProgress = new ProgressBarPopUp("Carregando Itens do Inventário, aguarde...");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                Items = new ObservableCollection<InventarioItemViewModel>();

                var lstInventarioItemVO = await ObterLocalizPorEstabDepService.GetObterFichasUsuarioAsync(_inventario.IdInventario, _codLocaliz, this);

                //var lstLocalizacoesVO = await ParametersObterLocalizacaoUsuarioService.GetObterLocalizacoesUsuarioAsync(_inventarioVO.IdInventario, this);

                foreach (var row in lstInventarioItemVO)
                {
                    var modelView = Mapper.Map<InventarioItemVO, InventarioItemViewModel>(row);

                    Items.Add(modelView);
                }

                SearchBarItCodigo.Focus();

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

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            CarregaListView();

          //  var pageProgress = new ProgressBarPopUp("Carregando Itens, aguarde...");
          //
          //  try
          //  {
          //
          //      await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);
          //
          //      Items = new ObservableCollection<InventarioItemViewModel>();
          //
          //      var lstInventarioItem = await ParametersFichasUsuarioService.GetObterFichasUsuarioAsync(_inventario.IdInventario, _codLocaliz);
          //
          //      List<InventarioLocalizacaoVO> lstInventarioLocalizacaoVO = new List<InventarioLocalizacaoVO>();
          //      _lstInventarioItemVO = new List<InventarioItemVO>();
          //
          //      foreach (var inventarioItem in lstInventarioItem.param.Resultparam)
          //      {
          //          
          //          InventarioItemVO inventarioItemVO = new InventarioItemVO();
          //          inventarioItemVO.InventarioId = inventarioItem.IdInventario;
          //          inventarioItemVO.CodLocaliz = inventarioItem.Localizacao;
          //          inventarioItemVO.CodLote = inventarioItem.Lote;
          //          // CodRefer = inventarioItem.Cod
          //          inventarioItemVO.ItCodigo = inventarioItem.CodItem;
          //
          //          var modelView = Mapper.Map<InventarioItemVO, InventarioItemViewModel>(inventarioItemVO);
          //
          //          Items.Add(modelView);
          //      }
          //
          //      _ItemsUnfiltered = Items;
          //
          //      OnPropertyChanged("Items");
          //
          //      SearchBarItCodigo.Focus();
          //
          //  }
          //  catch (Exception ex)
          //  {
          //      System.Diagnostics.Debug.Write(ex);
          //      //await DisplayAlert("Erro!", ex.Message, "Cancelar");
          //  }
          //  finally
          //  {
          //      await pageProgress.OnClose();
          //  }
        }

        async void OnClick_NovoItem(object sender, EventArgs e)
        {
        }

        async void OnClick_BuscaItem(object sender, EventArgs e)
        {
           // var param = new ParametersItemLeituraEtiquetaService();
           //
           // var inventario = new Inventario()
           // {
           //     IdInventario = _inventario.IdInventario,
           //     CodEstabel = _inventario.CodEstabel,
           //     CodDepos = _inventario.CodDepos,
           //    // Localizacao = localizacao,
           //     Lote = "",
           //     QuantidadeDigitada = 0,
           //     CodigoBarras = "02[65.116.00709-1[1[2[3[4[5[6[1[8"  // receber do leitor
           // };
           //
           // var _inventarioItem = await param.SendInventarioAsync(inventario);
           //
           // var filtroReturn = Items.FirstOrDefault(x => x.CodRefer == _inventarioItem.paramConteudo.Resultparam[0].CodItem);
           // filtroReturn.Quantidade += _inventarioItem.paramConteudo.Resultparam[0].Quantidade;
           //
           // foreach (var item in Items)
           // {
           //     if (item.CodRefer == _inventarioItem.paramConteudo.Resultparam[0].CodItem)
           //     {
           //         Items.Remove(item);
           //         break;
           //     }
           // }
           //
           // var modelView = Mapper.Map<InventarioItemVO, InventarioItemViewModel>(filtroReturn);
           // Items.Add(modelView);
           //
           // cvInventarioItem.BindingContext = this;
           //
           // var pageProgress = new ProgressBarPopUp("Leitura realizada com sucesso !!!");
           // await PopupNavigation.Instance.PushAsync(pageProgress);
           // await pageProgress.OnClose();

        }

        private async void VerifyProd(string strQr)
        {
            if (strQr == null)
                return;

            try
            {
                var mdlEtiqueta = csAuxiliar.GetEtiqueta(strQr);

                //var inventarioItem = InventarioItemDB.GetInventarioItemByQr(_inventario.IdInventario, mdlEtiqueta.itCodigo, mdlEtiqueta.lote).FirstOrDefault();

              // if (inventarioItem != null)
              // {
              //     //var page = new InventarioUpdateItemPopUp(_inventario.InventarioId, inventarioItem.InventarioItemId);
              //     var page = new InventarioCaixaIncompletaPopUp(_inventario.IdInventario, null);
              //     //page.SetResultDigita(resultDigita);
              //
              //     await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page);
              // }
              // else
              // {
              //     //if (_inventario.Contagem == 1)
              //     //{
              //     //    var result = await DisplayAlert("Atenção!", "Item (" + mdlEtiqueta.itCodigo + ") não encontrado no inventário, deseja adicionar para a digitação?", "Sim", "Não");
              //     //
              //     //    if (result.ToString() == "True")
              //     //    {
              //     //        var page = new InventarioCaixaIncompletaPopUp(_inventario.InventarioId, null);
              //     //
              //     //        page.SetResultDigita(resultDigita);
              //     //
              //     //        var item = ItemDB.GetItem(mdlEtiqueta.itCodigo);
              //     //
              //     //        if (item == null)
              //     //            throw new Exception("Item (" + mdlEtiqueta.itCodigo + ") não encontrado.");
              //     //
              //     //        page.SetNovoItemInventario(mdlEtiqueta.itCodigo, item.DescItem, item.Un, item.__TipoConEst__, mdlEtiqueta.lote, mdlEtiqueta.dtValiLote);
              //     //
              //     //        await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page);
              //     //    }
              //     //}
              //     //else
              //     //{
              //     //    throw new Exception("Item (" + mdlEtiqueta.itCodigo + ") não encontrado no inventário." + Environment.NewLine +
              //     //                        "Nova ficha (item) no inventário só pode ser criado na contagem (1) e não é permitido a criação de uma nova ficha (item) no inventário (" + _inventario.Contagem.ToString() + ").");
              //     //}
              // }
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
                IntegracaoOnlineBatch.EfetivaInventarioMobile(_inventario.IdInventario);

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

            // Criar();
            Application.Current.MainPage = new NavigationPage(new InventarioListaLocalizacaoPage(_inventario));

            return true;
        }

        private CancellationTokenSource throttleCts = new CancellationTokenSource();

        async void Handle_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {


                //System.Diagnostics.Debug.Write(e);
                bool lCodBarras = false;
                if (String.IsNullOrEmpty(e.OldTextValue) && e.NewTextValue.Length > 10){
                    lCodBarras = true;
                }


                //await Task.Run(() => PerformSearch());
                //PerformSearch();
                /* Victor Alves - 31/10/2019 - Processo para cancelar thread se digita varias vezes o item e trava  */
                Interlocked.Exchange(ref this.throttleCts, new CancellationTokenSource()).Cancel();
                await Task.Delay(TimeSpan.FromMilliseconds(1100), this.throttleCts.Token) // if no keystroke occurs, carry on after 500ms
                    .ContinueWith(
                        delegate { PerformSearch(lCodBarras); }, // Pass the changed text to the PerformSearch function
                        CancellationToken.None,
                        TaskContinuationOptions.OnlyOnRanToCompletion,
                        TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Erro!", ex.Message, "Cancel");
            }
        }

        async public void PerformSearch(bool plCodBarras)
        {
            try
            {
                //cvInventarioItem.IsEnabled = false;
                string strCodItemBusca = string.Empty;

                if (!String.IsNullOrEmpty(SearchBarItCodigo.Text) && SearchBarItCodigo.Text.IndexOf(";") >= 1)
                {
                    var splItem = SearchBarItCodigo.Text.Split(';');

                    //if (splItem[0] == "03" || splItem[0] == "02")
                    //{
                        strCodItemBusca = splItem[1].Trim();
                    //}
                }
                else
                {
                    strCodItemBusca = SearchBarItCodigo.Text.Trim();
                }

                if (string.IsNullOrWhiteSpace(strCodItemBusca))
                    Items = _ItemsUnfiltered;
                else
                {
                    /* Victor Alves - 31/10/2019 - Melhoria de performance Item */
                    _ItemsFiltered = new ObservableCollection<InventarioItemViewModel>(_ItemsUnfiltered.Where(i =>
                   (i is InventarioItemViewModel && (((InventarioItemViewModel)i).CodItem.ToLower().Contains(strCodItemBusca.ToLower())))
                   ));

                    Items = _ItemsFiltered;
                }

                if (plCodBarras)
                {
                    //SearchBarItCodigo.Unfocus();
                    BuscaItemList();
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

        async void BtnZerar_Clicked(object sender, EventArgs e)
        {
            var param = new ParametersZerarLeituraEtiquetaService();

            var inventario = new Inventario()
            {
                IdInventario = _inventario.IdInventario,
                CodEstabel = _inventario.CodEstabel,
                CodDepos = _inventario.CodDepos,
              //  Localizacao = localizacao,
                Lote = "",
                QuantidadeDigitada = 0,
                CodigoBarras = "02[65.116.00709-1[1[2[3[4[5[6[1[8"  // receber do leitor
            };

            var _inventarioItemZerar = await param.SendInventarioAsync(inventario);

            var pageProgress = new ProgressBarPopUp(_inventarioItemZerar.Resultparam.Zerar);
            await PopupNavigation.Instance.PushAsync(pageProgress);
            Thread.Sleep(1000);
            await pageProgress.OnClose();
        }


        private async void BuscaItemList()
        {

            if (!String.IsNullOrEmpty(SearchBarItCodigo.Text))
            {
                var current = _ItemsUnfiltered.FirstOrDefault(p => p.CodItem == SearchBarItCodigo.Text);

                if (current == null)
                {
                    if (SearchBarItCodigo.Text.Contains(';'))
                    {

                        var textItCodigo = SearchBarItCodigo.Text.Split(';');
                        if (textItCodigo.Length > 1)
                        {
                            current = _ItemsUnfiltered.FirstOrDefault(p => p.CodItem == textItCodigo[1]);

                            if (current != null)
                            {
                                current.CodigoBarras = SearchBarItCodigo.Text;
                            }
                        }
                    }
                }

                if (current != null)
                {
                    OpenPagePopUp(current);

                }
            }
        }


        private async void SearchBarItCodigo_Unfocused(object sender, FocusEventArgs e)
        {
           // BuscaItemList();
        }

        private async void OpenPagePopUp(InventarioItemViewModel current)
        {
            var pageProgress = new ProgressBarPopUp("Carregando...");

            if (current.StatusIntegracao == eStatusInventarioItem.ErroIntegracao)
            {
                var result = await DisplayAlert("Confirmação!", $"Item {current.CodItem} pendente de Integração Batch, deseja continuar?", "Sim", "Não");

                if (result.ToString() != "True")
                {
                    return;
                }
            }

            //current.CodigoBarras = SearchBarItCodigo.Text.Replace(";","[");
            var page = new InventarioCaixaIncompletaPopUp(_inventario.IdInventario, current);
            page._actDeleteRow = DeleteRowInventarioItem;
            page._actRefreshPage = RefreshRowInventarioItem;
            await PopupNavigation.Instance.PushAsync(page);
            await pageProgress.OnClose();

            cvInventarioItem.SelectedItem = null;
        }

        async void BtnLimpar_Clicked(object sender, EventArgs e)
        {
          
        }

        private void BtnImprimir_Clicked(object sender, EventArgs e)
        {

            Application.Current.MainPage = new NavigationPage(new ImprimirPage(_inventario));
        }

        async void cvInventarioItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            cvInventarioItem.IsEnabled = false;

            try
            {

                var current = (cvInventarioItem.SelectedItem as InventarioItemViewModel);

                if (cvInventarioItem.SelectedItem == null)
                    return;

                OpenPagePopUp(current);

            }
            catch (Exception ex)
            {
                await DisplayAlert("ERRO", ex.Message, "OK");
            }
            finally
            {
                cvInventarioItem.IsEnabled = true;
            }
        }

        public async void RefreshRowInventarioItem(InventarioItemVO pInventarioItemVO)
        {
            var current = Items.FirstOrDefault(p => p.InventarioItemId == pInventarioItemVO.InventarioItemId);

            if (current != null)
            {
                Items[Items.IndexOf(current)] = Mapper.Map<InventarioItemVO, InventarioItemViewModel>(pInventarioItemVO);

                //current =
                OnPropertyChanged("Items");
            }
        }
        public async void DeleteRowInventarioItem(InventarioItemVO pInventarioItemVO)
        {
            var current = Items.FirstOrDefault(p => p.InventarioItemId == pInventarioItemVO.InventarioItemId);

            if (current != null) {
                Items.Remove(current);
                OnPropertyChanged("Items");
            }

            if (Items.Count <= 0)
            {
                await DisplayAlert("Localização Finalizada", $"Itens da localização {_codLocaliz} inventariados com sucesso", "OK");
                OnBackButtonPressed();
            }
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                ToolBarPrint.IsEnabled = false;

                string strItCodigo = null;
                List<string> lstItem = new List<string>();

                if (_Items != null)
                {
                    if (_Items.Count == 1)
                    {
                        strItCodigo = _Items.FirstOrDefault().CodItem;
                    }
                    else if (_Items.Count > 1)
                    {
                        foreach (var item in _Items)
                        {
                            lstItem.Add(item.CodItem);
                        }
                    }
                }

                var pageProgress = new ProgressBarPopUp("Carregando...");
                var page = new InventarioPrintPopUp(_inventario.CodDepos, _codLocaliz, strItCodigo, lstItem );
                await PopupNavigation.Instance.PushAsync(page);
                //Thread.Sleep(1000);
                await pageProgress.OnClose();
            }
            finally
            {
                ToolBarPrint.IsEnabled = true;
            }
        }

        private void SearchBarItCodigo_SearchButtonPressed(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.Write(e);
        }
    }

    public class InventarioItemViewModel : InventarioItemVO, INotifyPropertyChanged
    {

        public string Image
        {
            get
            {
                if (this.StatusIntegracao == eStatusInventarioItem.NaoIniciado)
                {
                    return "intPendenteMed.png";
                }
                else if (this.StatusIntegracao == eStatusInventarioItem.IntegracaoCX)
                {
                    return "intSucessoMed.png";
                }
                else
                {
                    return "closeMini.png";
                }
            }
        }

        /*
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
        */


        public string StatusItemInventario
        {
            get
            {
                if (this.StatusIntegracao == eStatusInventarioItem.NaoIniciado)
                {
                    return "Não Digitado";
                }
                else if (this.StatusIntegracao == eStatusInventarioItem.IntegracaoCX)
                {
                    if (this.QuantidadeAcum != null && this.QuantidadeAcum > 0)
                    {

                        return $"{this.QuantidadeAcum.ToString()} (CX)";
                    }
                    else
                    {
                        return "Cont (CX)";
                    }
                } 
                else
                {
                    return "Int. Pendente";
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
