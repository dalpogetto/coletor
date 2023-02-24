using AutoMapper;
using CollectorQi.Models.ESCL028;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Services.ESCL018;
using CollectorQi.Services.ESCL028;
using CollectorQi.VO;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CollectorQi.Models;
using System.Runtime.CompilerServices;
using Rg.Plugins.Popup.Services;
using ESCL = CollectorQi.Models.ESCL028;
using System.Threading;
using System.Threading.Tasks;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotaFiscalConferenciaReparosListaPage : ContentPage, INotifyPropertyChanged
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

        public ObservableCollection<NotaFiscalViewModel> ObsNotaFiscal { set; get; }
        public List<NotaFiscalVO> listNotaFiscalVO { get; }
        public List<ListaDocumentosNotaFiscal> listaDocumentosNotaFiscal { get; set; }
        public string Estabelecimento { get; }
        public bool Conferido { get; set; } = true;

        public NotaFiscalConferenciaReparosListaPage()
        {
            InitializeComponent();

            ObsNotaFiscal = new ObservableCollection<NotaFiscalViewModel>();
            listNotaFiscalVO = new List<NotaFiscalVO>();

            //lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.Estabelecimento;

            //SearchBar.En += (sender, e) => edtCodigoBarras_Completed(sender, e);

            cvNotaFiscal.BindingContext = this;
        }

        private async void CarregaListView()
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Erro!", "Para buscar as Notas Fiscal no sistema o dispositivo deve estar conectado.", "CANCELAR");

                return;
            }

            BtnCarregaNotaFiscal.IsEnabled = false;

            var pageProgress = new ProgressBarPopUp("Carregando Notas Fiscal, aguarde...");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                ObsNotaFiscal.Clear();

                var listNotaFiscal = new List<ModelNotaFiscal>();
                listaDocumentosNotaFiscal = new List<ListaDocumentosNotaFiscal>();

                var lstNotaFiscal = await ParametersNotaFiscalService.SendParametersAsync(SecurityAuxiliar.GetCodEstabel());

                if (lstNotaFiscal != null && lstNotaFiscal.param != null && lstNotaFiscal.param.Resultparam != null)
                {

                    foreach (var item in lstNotaFiscal.param.Resultparam)
                    {
                        var notaFiscal = new ModelNotaFiscal();
                        notaFiscal.rowId = item.RowId;
                        notaFiscal.codEstabel = item.CodEstabel;
                        notaFiscal.codItem = item.CodItem;
                        notaFiscal.localizacao = item.Localizacao;
                        string[] descricaoItem = item.DescricaoItem.Split(' ');

                        foreach (string descricao in descricaoItem)
                        {
                            if (descricao != "")
                                notaFiscal.descricaoItem += descricao + " ";
                        }

                        notaFiscal.descricaoItem = notaFiscal.descricaoItem.TrimEnd();
                        notaFiscal.nroDocto = item.NroDocto;
                        notaFiscal.numRR = item.NumRR;
                        notaFiscal.conferido = item.Conferido;
                        notaFiscal.relaciona = item.Relaciona;
                        notaFiscal.codFilial = item.CodFilial;
                        listNotaFiscal.Add(notaFiscal);

                        var notaFiscalVO = new NotaFiscalVO();
                        notaFiscalVO.RowId = item.RowId;
                        notaFiscalVO.CodEstabel = item.CodEstabel;
                        notaFiscalVO.CodItem = item.CodItem;
                        notaFiscalVO.Localizacao = item.Localizacao;
                        string[] descricaoItems = item.DescricaoItem.Split(' ');

                        foreach (string descricao in descricaoItems)
                        {
                            if (descricao != "")
                                notaFiscalVO.DescricaoItem += descricao + " ";
                        }

                        notaFiscalVO.DescricaoItem = notaFiscalVO.DescricaoItem.TrimEnd();
                        notaFiscalVO.NroDocto = item.NroDocto;
                        notaFiscalVO.NumRR = item.NumRR;
                        notaFiscalVO.Conferido = item.Conferido;
                        notaFiscalVO.Relaciona = item.Relaciona;
                        notaFiscalVO.CodFilial = item.CodFilial;

                        var modelView = Mapper.Map<NotaFiscalVO, NotaFiscalViewModel>(notaFiscalVO);
                        ObsNotaFiscal.Add(modelView);

                        
                    }
                }

                OnPropertyChanged("ObsNotaFiscal");

                if (ObsNotaFiscal != null && ObsNotaFiscal.Count > 0)
                {
                    SearchBarItCodigo.Focus();
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                BtnCarregaNotaFiscal.IsEnabled = true;
                await pageProgress.OnClose();
            }

        }


        private async void SearchBarItCodigo_Unfocused(object sender, FocusEventArgs e)
        {

            // await ConfirmaCodigoBarras();
        }

        private async Task ConfirmaCodigoBarras()
        {
            if (!String.IsNullOrEmpty(SearchBarItCodigo.Text))
            {
                var pageProgress = new ProgressBarPopUp("Efetivando Reparo, aguarde...");
                // BtnEfetivar.IsEnabled = false;

                try
                {
                    /*
                    if (String.IsNullOrEmpty(SearchBarItCodigo.Text))
                    {
                        await DisplayAlert("Erro Leitura da Etiqueta", "Nenhuma etiqueta efetuada a leitura, favor realizar a leitura da etiqueta para seguir com a atualização da nota fiscal.", "OK");
                        return;
                    } */

                    // Alinhado com Kawano/Valter - encaminhar estabelecimento do tecnico
                    //string codEstabOrigem = SearchBarItCodigo.Text.Substring(0, 3);
                    string codEstabOrigem = SecurityAuxiliar.GetCodEstabel();
                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                    var dRetornoNota = await ValidarReparosNotaFiscalService.SendValidarReparosAsync(new ValidarReparosNotaFiscal
                    {
                        CodBarras = SearchBarItCodigo.Text

                    }, codEstabOrigem);

                    //  await pageProgress.OnClose();
                    if (dRetornoNota != null && dRetornoNota.Resultparam != null && dRetornoNota.Resultparam.Count > 0)
                    {
                        var v = dRetornoNota.Resultparam.FirstOrDefault();

                        SearchBarItCodigo.Text = "";

                        if (v.Mensagem.Contains("ERRO:"))
                        {
                            await DisplayAlert("ERRO!", v.Mensagem, "OK");
                            return;
                        }
                        else
                        {
                            CarregaListView();

                            
                            //  Acr.UserDialogs.UserDialogs.Instance.Toast("Efetivado código de barras com sucesso", new TimeSpan(10));
                        }
                    }
                }
                catch (Exception ex)
                {
                    // await pageProgress.OnClose();
                    await DisplayAlert("ERRO", ex.Message, "OK");
                }
                finally
                {
                    // BtnEfetivar.IsEnabled = true;
                    await pageProgress.OnClose();
                }

                /*
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

                } */
            }
        }
        private void SearchBarItCodigo_SearchButtonPressed(object sender, EventArgs e)
        {
            ConfirmaCodigoBarras();
        }


        async void OnClick_CarregaNotaFiscal(object sender, EventArgs e)
        {
            CarregaListView();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            ObsNotaFiscal = new ObservableCollection<NotaFiscalViewModel>();

            CarregaListView();
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new PrincipalPage());

            return true;
        }

        /*
        async void BtnScan_Clicked(object sender, EventArgs e)
        {

            BtnScan.IsEnabled = false;
            try
            {
                if (ObsNotaFiscal == null && ObsNotaFiscal.Count <= 0)
                {
                    await DisplayAlert("Erro!", "Nenhuma nota fiscal relacionada para confência.", "OK");
                }

                var page = new NotaFiscalConferenciaReparosListaPopUp();
                page._refreshListView = CarregaListView;
                await PopupNavigation.Instance.PushAsync(page);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "OK");
            }
            finally
            {
                BtnScan.IsEnabled = true;
            }
        }
        */

        private CancellationTokenSource throttleCts = new CancellationTokenSource();

        async void Handle_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {

                if (String.IsNullOrEmpty(e.OldTextValue) && e.NewTextValue.Length >= 5)
                {
                    await ConfirmaCodigoBarras();
                }

                /*

                //System.Diagnostics.Debug.Write(e);
                bool lCodBarras = false;
                if (String.IsNullOrEmpty(e.OldTextValue) && e.NewTextValue.Length > 5)
                {
                    lCodBarras = true;
                }


                //await Task.Run(() => PerformSearch());
                //PerformSearch();
                /* Victor Alves - 31/10/2019 - Processo para cancelar thread se digita varias vezes o item e trava  
                Interlocked.Exchange(ref this.throttleCts, new CancellationTokenSource()).Cancel();
                await Task.Delay(TimeSpan.FromMilliseconds(1500), this.throttleCts.Token) // if no keystroke occurs, carry on after 500ms
                    .ContinueWith(
                        delegate { PerformSearch(lCodBarras); }, // Pass the changed text to the PerformSearch function
                        CancellationToken.None,
                        TaskContinuationOptions.OnlyOnRanToCompletion,
                        TaskScheduler.FromCurrentSynchronizationContext());

                */
            }
            catch (Exception ex)
            {
                //await DisplayAlert("Erro!", ex.Message, "Cancel");
            }
        }

        public async void CodigoBarras(string pCodBarras, NotaFiscalViewModel currentNF)
        {

            try
            {
                if (String.IsNullOrEmpty(pCodBarras))
                {
                    await DisplayAlert("Erro Leitura da Etiqueta", "Nenhuma etiqueta efetuada a leitura, favor realizar a leitura da etiqueta para seguir com a atualização da nota fiscal.", "OK");
                    return;
                }

                // Alinhado com Kawano/Valter - encaminhar estabelecimento do tecnico
                //string codEstabOrigem = SearchBarItCodigo.Text.Substring(0, 3);
                string codEstabOrigem = SecurityAuxiliar.GetCodEstabel();

                var dRetornoNota = await ValidarReparosNotaFiscalService.SendValidarReparosAsync(new ValidarReparosNotaFiscal
                {
                    CodBarras = pCodBarras
                   
                }, codEstabOrigem);

              //  await pageProgress.OnClose();
                if (dRetornoNota != null && dRetornoNota.Resultparam != null && dRetornoNota.Resultparam.Count > 0)
                {
                    var v = dRetornoNota.Resultparam.FirstOrDefault();

                    if (v.Mensagem.Contains("ERRO:"))
                    {
                        await DisplayAlert("ERRO!", v.Mensagem, "OK");
                        return;
                    }
                    else
                    {
                        CarregaListView();
                    }
                }
            }
            catch (Exception e)
            {
               // await pageProgress.OnClose();
                await DisplayAlert("ERRO", e.Message, "OK");
            }
        }

        public async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cvNotaFiscal.SelectedItem == null)
                return;

            var current = (cvNotaFiscal.SelectedItem as NotaFiscalViewModel);

            //BtnEfetivarItem.IsEnabled = false;
            try
            {
                cvNotaFiscal.SelectedItem = null;
                cvNotaFiscal.IsEnabled = false;
                //cvGuardaMateriaisDepositoItem.IsEnabled = false;

                var page = new NotaFiscalConferenciaReparosDetPopUp(current.CodItem, current.DescricaoItem, current.NroDocto, current.NumRR, current.Conferido, current.Relaciona, current.CodFilial, current);

                page._actConfirmaConferencia = CodigoBarras;

                await PopupNavigation.Instance.PushAsync(page);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "OK");
            }
            finally
            {
                cvNotaFiscal.IsEnabled = true;
                //BtnEfetivarItem.IsEnabled = true;
            }
        }

        private async void BtnFinalizaConferencia_Clicked(object sender, EventArgs e)
        {
            BtnFinalizaConferencia.IsEnabled = false;

            var pageProgress = new ProgressBarPopUp("Efetivando Documento, aguarde...");

            try
            {
                var result = await DisplayAlert("Confirmar Nota?", "Deseja finalizar a conferencia das notas fiscais?", "Sim", "Nao");

                if (result.ToString() == "True")
                {
                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                    var lstNotaConferencia = new CollectorQi.Services.ESCL028.FinalizarConferenciaNotaFiscalService.RequestNotaFiscalJson();

                    //lstNotaConferencia.
                    lstNotaConferencia.Parametros = new FinalizarConferenciaNotaFiscalService.Param();

                    lstNotaConferencia.Parametros.CodEstabel = SecurityAuxiliar.GetCodEstabel();

                    lstNotaConferencia.ListaReparosConferidos = new List<FinalizarConferenciaReparosConferidos>();
                    lstNotaConferencia.ListaConferenciaDocumentos = new List<FinalizarConferenciaDocumentos>();

                    var lstNotaFiscal = await ParametersNotaFiscalService.SendParametersAsync(SecurityAuxiliar.GetCodEstabel());

                    if (lstNotaFiscal != null)
                    {

                        foreach (var item in ObsNotaFiscal.Where(x => x.Conferido))
                        {
                            lstNotaConferencia.ListaReparosConferidos.Add(new FinalizarConferenciaReparosConferidos { RowId = item.RowId });

                            var docto = lstNotaFiscal.param.ListaDocumentos.FirstOrDefault(x => x.Relaciona == item.Relaciona);

                            if (docto != null && lstNotaConferencia.ListaConferenciaDocumentos.FirstOrDefault(x => x.Relaciona == item.Relaciona) == null)
                            {
                                lstNotaConferencia.ListaConferenciaDocumentos.Add(new FinalizarConferenciaDocumentos
                                {
                                    CodEmitente = docto.CodEmitente,
                                    Docto = item.NroDocto,
                                    CodEstabel = item.CodEstabel,
                                    Usuario = SecurityAuxiliar.GetCodEstabel(),
                                    Atualizar = true,
                                    NatOperacao = docto.NatOperacao,
                                    SerieDocto = docto.SerieDocto,
                                    Relaciona = item.Relaciona,
                                });
                            }
                        }
                    }

                    var parametrosRetorno = await FinalizarConferenciaNotaFiscalService.SendFinalizarConferenciaAsync(lstNotaConferencia);

                    if (parametrosRetorno.Retorno == "OK")
                    {
                        await DisplayAlert("Sucesso", "Conferencia efetuada com sucesso", "OK");

                        CarregaListView();
                    }
                    else
                    {
                        if (parametrosRetorno.Resultparam != null && parametrosRetorno.Resultparam.Count > 0)
                        {
                            await DisplayAlert("ERRO!", parametrosRetorno.Resultparam[0].ErrorDescription + " - " + parametrosRetorno.Resultparam[0].ErrorHelp, "OK");
                        }
                        else
                        {
                            await DisplayAlert("ERRO!", "Erro no retorno do envio !", "OK");
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("ERRO!", ex.Message, "OK");
            }
            finally
            {
                await pageProgress.OnClose();
                BtnFinalizaConferencia.IsEnabled = true;
            }
        }
    }

    public class NotaFiscalViewModel : NotaFiscalVO, INotifyPropertyChanged
    {
        public string Image
        {
            get
            {
                if(this.Conferido)
                    return "intSucessoMed.png";
                else
                    return "intPendenteMed.png";
            }
        }


        #region Property

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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

    }
}