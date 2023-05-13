using AutoMapper;
using CollectorQi.Models.Datasul;
using CollectorQi.Models.ESCL018;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper.ESCL018;
using CollectorQi.Services.ESCL018;
using CollectorQi.Services.ESCL018B;
using CollectorQi.VO.ESCL018;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventarioListaItemPageB : ContentPage, INotifyPropertyChanged
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

        public InventarioListaItemPageB(InventarioVO pInventarioVO , string pCodLocaliz)
        {
            InitializeComponent();

            _codLocaliz = pCodLocaliz;
            _inventario = pInventarioVO;
            cvInventarioItem.BindingContext = this;

            this.Title = "Digitação de Item";

            lblCodLocalizacao.Text = $"Depósito / Localização: {_inventario.CodDepos} / {_codLocaliz}";

        }
                
        private async void CarregaListView()
        {
            var pageProgress = new ProgressBarPopUp("Carregando Itens do Inventário, aguarde...");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                Items = new ObservableCollection<InventarioItemViewModel>();

                var lstInventarioItemVO = await ObterLocalizPorEstabDepService.GetObterFichasUsuarioAsync(_inventario.IdInventario, _codLocaliz, this);

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
            Application.Current.MainPage = new NavigationPage(new InventarioListaLocalizacaoPageB(_inventario));

            return true;
        }

        private CancellationTokenSource throttleCts = new CancellationTokenSource();

        async void Handle_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                bool lCodBarras = false;
                if (String.IsNullOrEmpty(e.OldTextValue) && e.NewTextValue.Length > 5){
                    lCodBarras = true;
                }
                
                /* Victor Alves - 31/10/2019 - Processo para cancelar thread se digita varias vezes o item e trava  */
                Interlocked.Exchange(ref this.throttleCts, new CancellationTokenSource()).Cancel();
                await Task.Delay(TimeSpan.FromMilliseconds(1500), this.throttleCts.Token) // if no keystroke occurs, carry on after 500ms
                    .ContinueWith(
                        delegate { PerformSearch(lCodBarras); }, // Pass the changed text to the PerformSearch function
                        CancellationToken.None,
                        TaskContinuationOptions.OnlyOnRanToCompletion,
                        TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch
            {
            }
        }

        async public void PerformSearch(bool plCodBarras)
        {
            try
            {
                //cvInventarioItem.IsEnabled = false;
                string strCodItemBusca = string.Empty;

                if (SearchBarItCodigo.Text.IndexOf(";") >= 1)
                {
                    var splItem = SearchBarItCodigo.Text.Split(';');

                    if (splItem[0] == "03" || splItem[0] == "02")
                    {
                        strCodItemBusca = splItem[1].Trim();
                    }
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
                    SearchBarItCodigo.Unfocus();
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

        static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, @"[^0-9a-zA-Z;./-]+", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters,
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }

        void OnClick_EfetivarContagem(object sender, EventArgs e)
        {
            EfetivarContagem(false);
        }

        public async void EfetivarContagem(bool blnEfetivaPopUp)
        {
            var result = true;

            if (!blnEfetivaPopUp)
            {
                result = await DisplayAlert("Confirmação!", $"Deseja efetivar a contagem?", "Sim", "Não");
            }

            var pageProgress = new ProgressBarPopUp("Carregando...");
            try
            {
                BtnEfetivarContagem.IsEnabled = false;

                if (result.ToString() == "True")
                {
                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

              
                    // Busca todos itens digitados da Lista
                    List<ListaItemInventarioPAM> lstItensDigitados = new List<ListaItemInventarioPAM>();
                    foreach (var row in Items)
                    {
                        var resultDigitacao = InventarioItemCodigoBarrasDB.GetByInventarioItemKey(row.InventarioItemKey); 
                        if (resultDigitacao != null && resultDigitacao.Count > 0)
                        {
                            foreach(var rowDig in resultDigitacao)
                            {
                                ListaItemInventarioPAM registroItemDigitado;
                                //if (String.IsNullOrEmpty(rowDig.CodigoBarras))
                                //{
                                    registroItemDigitado = lstItensDigitados.FirstOrDefault(x => x.CodItem == row.CodItem);

                                    if (registroItemDigitado == null)
                                    {
                                        registroItemDigitado = new ListaItemInventarioPAM
                                        {
                                            CodItem = row.CodItem,
                                            CodigoBarras = "",
                                            QtdeDigitada = rowDig.Quantidade
                                        };

                                        lstItensDigitados.Add(registroItemDigitado);
                                    }
                                    else
                                    {
                                        registroItemDigitado.QtdeDigitada += rowDig.Quantidade; 
                                    }


                                    
                                //}
                                //else
                                //{
                                //    registroItemDigitado = lstItensDigitados.FirstOrDefault(x => x.CodigoBarras == row.CodigoBarras);
                                //
                                //    if (registroItemDigitado == null)
                                //    {
                                //        registroItemDigitado = new ListaItemInventarioPAM
                                //        {
                                //            CodItem      = row.CodItem,
                                //            CodigoBarras = rowDig.CodigoBarras,
                                //            QtdeDigitada = rowDig.Quantidade
                                //        };
                                //    }
                                //    else
                                //    {
                                //        registroItemDigitado.QtdeDigitada += rowDig.Quantidade;
                                //    }
                                //}

                            }
                        }
                    }

                    if (lstItensDigitados.Count == 0)
                    {
                        await DisplayAlert("Erro!", "Não há itens pendentes de efetivação de contagem", "Ok");
                        return;
                    }

                    var resultService = await ParametersEfetivarContagem.SendInventarioAsync(_inventario, lstItensDigitados, _codLocaliz);

                    //var _inventarioItemVO = lstInventarioItem.FirstOrDefault();
                    //
                    //var resultDigitacao = InventarioItemCodigoBarrasDB.GetByInventarioItemKey(_inventarioItemVO.InventarioItemKey);
                    //decimal qtdAcumulada = 0;
                    //if (resultDigitacao != null)
                    //{
                    //    qtdAcumulada = resultDigitacao.Sum(x => x.Quantidade);
                    //}
                    //
                    //var inventarioBarra = new InventarioItemBarra()
                    //{
                    //    IdInventario = _inventarioItemVO.InventarioId,
                    //    Lote = _inventarioItemVO.Lote.Trim(),
                    //    Localizacao = _inventarioItemVO.Localizacao.Trim(),
                    //    CodItem = _inventarioItemVO.CodItem.Trim(),
                    //    CodDepos = _inventarioItemVO.__inventario__.CodDepos.Trim(),
                    //    QuantidadeDigitada = int.Parse(qtdAcumulada.ToString()),
                    //    CodEmp = SecurityAuxiliar.GetCodEmpresa(),
                    //    Contagem = _inventarioItemVO.Contagem,
                    //    CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                    //    CodigoBarras = CleanInput(_inventarioItemVO.CodigoBarras)
                    //};

                    //_inventarioItemVO.Quantidade = int.Parse(txtQuantidade.Text);

                    //if (_inventarioItemVO.Quantidade > 0)
                    //{
                    //    _inventarioItemVO.CodigoBarras = CleanInput(_inventarioItemVO.CodigoBarras);
                    //
                    //    _inventarioItemVO.CodigoBarras = _inventarioItemVO.CodigoBarras.Replace(";", "[");
                    //    inventarioBarra.CodigoBarras = inventarioBarra.CodigoBarras.Replace(";", "[");
                    //
                    //}
                    //else
                    //{
                    //    _inventarioItemVO.CodigoBarras = $"02;{_inventarioItemVO.CodItem.Trim()};1;1;1;0;1;1;1;1";
                    //    _inventarioItemVO.CodigoBarras = _inventarioItemVO.CodigoBarras.Replace(";", "[");
                    //    inventarioBarra.CodigoBarras = _inventarioItemVO.CodigoBarras;
                    //}
                    //
                    //var resultService = await ParametersEfetivarContagem.SendInventarioAsync(inventarioBarra, _inventarioItemVO, 0, this);

                    if (resultService != null && resultService.Retorno != null)
                    {
                        if (resultService.Retorno == "OK")
                        {
                            string strErro = "";
                            string strSucesso = "";
                            List<string> lstCodItemEfetivado = new List<string>();
                            if (resultService.ConteudoSuccess != null && resultService.ConteudoSuccess.ListaItens != null)
                            {
                                var resultadoErro = resultService.ConteudoSuccess.ListaItens.Exists(x => x.Mensagem.Contains("Erro:"));

                                if (resultadoErro != null)
                                {
                                    foreach (var rowErro in resultService.ConteudoSuccess.ListaItens.Where(x => x.Mensagem.Contains("Erro:")))
                                    {
                                        if (strErro == "")
                                        {
                                            strErro = $"({rowErro.CodItem} - {rowErro.Mensagem}";
                                        }
                                        else
                                        {
                                            strErro = strErro + " / " + $"({rowErro.CodItem} - {rowErro.Mensagem}";
                                        }
                                    }
                                }

                                var resultSucesso = resultService.ConteudoSuccess.ListaItens.Exists(x => x.Mensagem.Contains("OK:"));

                                if (resultSucesso != null)
                                {
                                    foreach (var rowSucucesso in resultService.ConteudoSuccess.ListaItens.Where(x => x.Mensagem.Contains("OK:")))
                                    {
                                        if (strSucesso == "")
                                        {
                                            strSucesso = $"({rowSucucesso.CodItem} - {rowSucucesso.Mensagem}";
                                        }
                                        else
                                        {
                                            strSucesso = strSucesso + " / " + $"({rowSucucesso.CodItem} - {rowSucucesso.Mensagem}";
                                        }

                                        lstCodItemEfetivado.Add(rowSucucesso.CodigoBarras);
                                    }
                                }
                            }

                            bool lExistErro = false;
                            if (String.IsNullOrEmpty(strErro) && !String.IsNullOrEmpty(strSucesso)) {
                                await DisplayAlert("Sucesso!", $"{resultService.Localizacao} Efetivação concluída com sucesso !", "OK");
                            }
                            else if (!String.IsNullOrEmpty(strErro) && (!String.IsNullOrEmpty(strSucesso) || strSucesso == ""))
                            {
                                await DisplayAlert("Atenção!", $"{resultService.Localizacao} Efetivação concluída com sucesso ! Porem ocorreu erro de efetivação nos items ({strErro}, favor revisar a atualização", "OK");
                                lExistErro = true;
                            }

                            if (lstCodItemEfetivado.Count > 0)
                            {
                                foreach (var rowObs in Items) {
                                    
                                    if (lstCodItemEfetivado.Exists(x => x == rowObs.CodItem))
                                    {
                                        InventarioItemCodigoBarrasDB.DeleteByItemId(rowObs.InventarioItemKey);
                                    }
                                }
                            }

                            await pageProgress.OnClose();

                            if (!lExistErro) {
                                if (!blnEfetivaPopUp)
                                {
                                    OnBackButtonPressed();
                                }
                                else
                                {
                                    if (_pagePopUp != null) { _pagePopUp.OnCloseCustom(); }
                                    OnBackButtonPressed();
                                }
                            }
                            else
                            {
                                CarregaListView();
                            }

                            
                        }
                       // else if (resultService.Retorno == "IntegracaoBatch")
                       // {
                       //     await DisplayAlert("Atenção!", "Erro de conexão com ERP, a atualização do item será integrado de forma Offline", "OK");
                       //
                       //     _inventarioItemVO.StatusIntegracao = eStatusInventarioItem.ErroIntegracao;
                       //     //_actRefreshPage(_inventarioItemVO);
                       //
                       //     await pageProgress.OnClose();
                       //     OnBackButtonPressed();
                       // }
                        else
                        {
                            if (resultService.Resultparam != null && resultService.Resultparam.Count > 0)
                            {
                                await DisplayAlert("Erro!", resultService.Resultparam[0].ErrorDescription + " - " + resultService.Resultparam[0].ErrorHelp, "Cancelar");
                            }
                            else
                            {
                                await DisplayAlert("Erro!", "Erro na efetivação do inventário", "Cancelar");
                            }
                        }
                    }
                    else
                    {
                        await DisplayAlert("Erro!", "Erro na efetivação do inventário", "Cancelar");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                BtnEfetivarContagem.IsEnabled = true;
                await pageProgress.OnClose();
            }
        }
                
        private async void SearchBarItCodigo_Unfocused(object sender, FocusEventArgs e)
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

        InventarioCaixaIncompletaPopUpB _pagePopUp;

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
            _pagePopUp = new InventarioCaixaIncompletaPopUpB(_inventario, current);
            _pagePopUp._actDeleteRow = DeleteRowInventarioItem;
            _pagePopUp._actRefreshPage = RefreshRowInventarioItem;
            //_pagePopUp._actEfetivaInventirio = EfetivarContagem;

            await PopupNavigation.Instance.PushAsync(_pagePopUp);
           // await pageProgress.OnClose();

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

        public async void RefreshRowInventarioItem(InventarioItemVO pInventarioItemVO, bool pBlnClose, string byInventarioItemKey)
        {
            try
            {
                if (pInventarioItemVO != null)
                {
                    var current = Items.FirstOrDefault(p => p.InventarioItemKey == pInventarioItemVO.InventarioItemKey);

                    if (current != null)
                    {
                        if (!pBlnClose)
                        {
                            if (!String.IsNullOrEmpty(current.CodItem))
                            {
                                Items[Items.IndexOf(current)] = Mapper.Map<InventarioItemVO, InventarioItemViewModel>(pInventarioItemVO);
                            }
                            else
                            {
                                Items[Items.IndexOf(current)] = Items[Items.IndexOf(current)];
                            }
                        }
                        else
                        {
                            Items[Items.IndexOf(current)] = Items[Items.IndexOf(current)];
                        }
                        OnPropertyChanged("Items");
                    }
                }
                else
                {
                    if (byInventarioItemKey != null)
                    {
                        var current = Items.FirstOrDefault(p => p.InventarioItemKey == byInventarioItemKey);

                        if (current != null)
                        {
                            Items[Items.IndexOf(current)] = Items[Items.IndexOf(current)];
                        }
                    }

                    OnPropertyChanged("Items");
                }
            }
            catch
            {
                return;
            }
        }

        public async void DeleteRowInventarioItem(InventarioItemVO pInventarioItemVO)
        {

            OnPropertyChanged("Items");

            var newItem = Items.FirstOrDefault(x => x.CodItem == pInventarioItemVO.CodItem && x.InventarioItemKey != pInventarioItemVO.InventarioItemKey);
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

        private async void ToolBarEtiqueta_Clicked(object sender, EventArgs e)
        {
            var page = new InventarioListaCodigoBarrasPopUp(Items.ToList());

            await PopupNavigation.Instance.PushAsync(page);

            page._actRefreshPage = RefreshRowInventarioItem;

        }
    }

    public class InventarioItemViewModelB : InventarioItemVO, INotifyPropertyChanged
    {
        
        public string Image
        {
            get
            {
                if (this.StatusIntegracao ==  eStatusInventarioItem.NaoIniciado)
                {
                    return "intPendenteMed.png";
                }
                else if(this.StatusIntegracao == eStatusInventarioItem.IntegracaoCX)
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
        

        
        public string StatusItemInventario
        {
            get
            {
                if (this.StatusIntegracao == eStatusInventarioItem.NaoIniciado)
                {
                    return "Não Digitado";
                }
                else
                {
                    return "Int. Pendente";
                }
            }
        }*/
        
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

    public class ListaItemInventarioPAM
    {
        public string CodigoBarras { get; set; }
        public string CodItem { get; set; }
        public decimal QtdeDigitada { get; set; }
    }
}
