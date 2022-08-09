﻿using CollectorQi.Models;
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
using CollectorQi.Services.ESCL018;
using CollectorQi.Models.ESCL018;

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
        /*

        public void CarregaDescricao()
        {
            foreach (var i in _ItemsUnfiltered)
            {
                var item = SecurityAuxiliar.ItemAll.Find(p => p.ItCodigo == i.ItCodigo);
              //  i.__itemDesc__ = item.ItCodigo + item.DescItem;
            }
        } */

        //async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    cvInventarioItem.IsEnabled = false;
        //    //var pageProgress = new ProgressBarPopUp("Carregando Item..");
        //    //await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

        //    try
        //    {
        //        var current = (cvInventarioItem.SelectedItem as InventarioItemViewModel);
        //        if (current != null)
        //        {
        //            var page = new InventarioUpdateItemPopUp(_inventario.InventarioId, current.InventarioItemId);

        //            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page);

        //            page.SetResultDigita(resultDigita);

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await DisplayAlert("Erro!", ex.Message, "OK");
        //    }
        //    finally
        //    {
        //        cvInventarioItem.IsEnabled = true;               
        //    }
        //}

        async void OnClick_CaixaIncompleta(object sender, EventArgs e)
        {
            var pageProgress = new ProgressBarPopUp("Carregando...");
            var page = new InventarioCaixaIncompletaPopUp(_inventario.InventarioId, null);
            await PopupNavigation.Instance.PushAsync(page);
            Thread.Sleep(1000);
            await pageProgress.OnClose();


            //cvInventarioItem.IsEnabled = false;
            //var pageProgress = new ProgressBarPopUp("Carregando Item..");
            //await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

            //try
            //{
            //    var current = (cvInventarioItem.SelectedItem as InventarioItemViewModel);
            //    if (current != null)
            //    {
            //        var page = new InventarioUpdateItemPopUp(_inventario.InventarioId, current.InventarioItemId);

            //        await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page);

            //        page.SetResultDigita(resultDigita);

            //    }
            //}
            //catch (Exception ex)
            //{
            //    await DisplayAlert("Erro!", ex.Message, "OK");
            //}
            //finally
            //{
            //    cvInventarioItem.IsEnabled = true;
            //}
        }

        public void resultDigita(InventarioItemVO byCurrent, bool byQtdDigita)
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

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var pageProgress = new ProgressBarPopUp("Carregando Itens, aguarde...");

            try
            {

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                Items = new ObservableCollection<InventarioItemViewModel>();

                /*
                foreach (var row in _lstInventarioItemVO)
                {
                    var modelView = Mapper.Map<InventarioItemVO, InventarioItemViewModel>(row);
                    //localizacao = modelView.CodLocaliz;

                    Items.Add(modelView);
                }
                */

                var lstInventarioItem = await ParametersFichasUsuarioService.GetObterFichasUsuarioAsync(_inventario.InventarioId, _codLocaliz);

                List<InventarioLocalizacaoVO> lstInventarioLocalizacaoVO = new List<InventarioLocalizacaoVO>();
                _lstInventarioItemVO = new List<InventarioItemVO>();

                foreach (var inventarioItem in lstInventarioItem.param.Resultparam)
                {
                    
                    InventarioItemVO inventarioItemVO = new InventarioItemVO();
                    inventarioItemVO.InventarioId = inventarioItem.IdInventario;
                    inventarioItemVO.CodLocaliz = inventarioItem.Localizacao;
                    inventarioItemVO.CodLote = inventarioItem.Lote;
                    // CodRefer = inventarioItem.Cod
                    inventarioItemVO.ItCodigo = inventarioItem.CodItem;

                    var modelView = Mapper.Map<InventarioItemVO, InventarioItemViewModel>(inventarioItemVO);

                    Items.Add(modelView);
                }

                _ItemsUnfiltered = Items;

                OnPropertyChanged("Items");

                SearchBarItCodigo.Focus();

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                //await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                await pageProgress.OnClose();
            }
        }

        async void OnClick_NovoItem(object sender, EventArgs e)
        {
            /*var page = new InventarioUpdateItemPopUp(_inventarioId, 0);

            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page); */
        }

        async void OnClick_BuscaItem(object sender, EventArgs e)
        {
            var param = new ParametersItemLeituraEtiquetaService();

            var inventario = new Inventario()
            {
                IdInventario = _inventario.InventarioId,
                CodEstabel = _inventario.CodEstabel,
                CodDepos = _inventario.CodDepos,
               // Localizacao = localizacao,
                Lote = "",
                QuantidadeDigitada = 0,
                CodigoBarras = "02[65.116.00709-1[1[2[3[4[5[6[1[8"  // receber do leitor
            };

            var _inventarioItem = await param.SendInventarioAsync(inventario);

            var filtroReturn = Items.FirstOrDefault(x => x.CodRefer == _inventarioItem.paramConteudo.Resultparam[0].CodItem);
            filtroReturn.Quantidade += _inventarioItem.paramConteudo.Resultparam[0].Quantidade;

            foreach (var item in Items)
            {
                if (item.CodRefer == _inventarioItem.paramConteudo.Resultparam[0].CodItem)
                {
                    Items.Remove(item);
                    break;
                }
            }

            var modelView = Mapper.Map<InventarioItemVO, InventarioItemViewModel>(filtroReturn);
            Items.Add(modelView);

            cvInventarioItem.BindingContext = this;

            var pageProgress = new ProgressBarPopUp("Leitura realizada com sucesso !!!");
            await PopupNavigation.Instance.PushAsync(pageProgress);
            Thread.Sleep(1000);
            await pageProgress.OnClose();

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
                    //var page = new InventarioUpdateItemPopUp(_inventario.InventarioId, inventarioItem.InventarioItemId);
                    var page = new InventarioCaixaIncompletaPopUp(_inventario.InventarioId, null);
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
                            var page = new InventarioCaixaIncompletaPopUp(_inventario.InventarioId, null);

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
                                            "Nova ficha (item) no inventário só pode ser criado na contagem (1) e não é permitido a criação de uma nova ficha (item) no inventário (" + _inventario.Contagem.ToString() + ").");
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

            // Criar();
            Application.Current.MainPage = new NavigationPage(new InventarioListaLocalizacaoPage(_inventario));

            return true;
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
                   (i is InventarioItemViewModel && (((InventarioItemViewModel)i).ItCodigo.ToLower().Contains(SearchBarItCodigo.Text.ToLower())))
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

        async void BtnZerar_Clicked(object sender, EventArgs e)
        {
            var param = new ParametersZerarLeituraEtiquetaService();

            var inventario = new Inventario()
            {
                IdInventario = _inventario.InventarioId,
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

        private async void SearchBarItCodigo_Unfocused(object sender, FocusEventArgs e)
        {
            if (!String.IsNullOrEmpty(SearchBarItCodigo.Text))
            {
                var current = _ItemsUnfiltered.FirstOrDefault(p => p.ItCodigo == SearchBarItCodigo.Text);

                if (current == null)
                {
                    if (SearchBarItCodigo.Text.Contains(';'))
                    {

                        var textItCodigo = SearchBarItCodigo.Text.Split(';');
                        if (textItCodigo.Length > 1)
                        {
                            current = _ItemsUnfiltered.FirstOrDefault(p => p.ItCodigo == textItCodigo[1]);
                        }
                    }
                }

                if (current != null)
                {
                    var pageProgress = new ProgressBarPopUp("Carregando...");

                    current.CodigoBarras = SearchBarItCodigo.Text.Replace(";","[");
                    var page = new InventarioCaixaIncompletaPopUp(_inventario.InventarioId, current);
                    page._actDeleteRow = DeleteRowInventarioItem;
                    await PopupNavigation.Instance.PushAsync(page);
                    //Thread.Sleep(1000);
                    await pageProgress.OnClose();

                    cvInventarioItem.SelectedItem = null;
                    
                }
            }
        }

        async void BtnLimpar_Clicked(object sender, EventArgs e)
        {
           /* var param = new ParametersLimparLeituraEtiquetaService();

            var inventario = new Inventario()
            {
                IdInventario = _inventario.InventarioId,
                CodEstabel = _inventario.CodEstabel,
                CodDepos = _inventario.CodDepos,
                Localizacao = localizacao,
                Lote = "",
                QuantidadeDigitada = 0,
                CodigoBarras = "02[65.116.00709-1[1[2[3[4[5[6[1[8"  // receber do leitor
            };

            var _inventarioItemLimpar = await param.SendInventarioAsync(inventario);

            var pageProgress = new ProgressBarPopUp(_inventarioItemLimpar.Resultparam.LimparLeitura);
            await PopupNavigation.Instance.PushAsync(pageProgress);
            Thread.Sleep(1000);
            await pageProgress.OnClose(); */
        }

        private void BtnImprimir_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new ImprimirPage(_inventario));
        }

        async void cvInventarioItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var current = (cvInventarioItem.SelectedItem as InventarioItemViewModel);

            if (cvInventarioItem.SelectedItem == null)
                return;

            var pageProgress = new ProgressBarPopUp("Carregando...");

            var page = new InventarioCaixaIncompletaPopUp(_inventario.InventarioId, current);
            page._actDeleteRow = DeleteRowInventarioItem;
            await PopupNavigation.Instance.PushAsync(page);
            //Thread.Sleep(1000);
            await pageProgress.OnClose();

            cvInventarioItem.SelectedItem = null;
        }

        public async void DeleteRowInventarioItem(InventarioItemVO pInventario)
        {
            var current = Items.FirstOrDefault(p => p.InventarioItemId == pInventario.InventarioItemId);

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

                var pageProgress = new ProgressBarPopUp("Carregando...");
                var page = new InventarioPrintPopUp(_inventario.CodDepos, _codLocaliz);
                await PopupNavigation.Instance.PushAsync(page);
                //Thread.Sleep(1000);
                await pageProgress.OnClose();
            }
            finally
            {
                ToolBarPrint.IsEnabled = true;
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
