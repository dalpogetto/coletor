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

            lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.Estabelecimento;

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

        async void OnClick_CarregaNotaFiscal(object sender, EventArgs e)
        {
            //var current = (cvNotaFiscal.SelectedItem as NotaFiscalVO);
            CarregaListView();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            ObsNotaFiscal = new ObservableCollection<NotaFiscalViewModel>();

            //ObsInventario = new ObservableCollection<InventarioViewModel>();

            CarregaListView();
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new PrincipalPage());

            return true;
        }

        async void BtnScan_Clicked(object sender, EventArgs e)
        {

            //BtnEfetivarItem.IsEnabled = false;
            try
            {
                if (ObsNotaFiscal == null && ObsNotaFiscal.Count <= 0)
                {
                    await DisplayAlert("Erro!", "Nenhuma nota fiscal relacionada para confência.", "OK");
                }

                //cvGuardaMateriaisDepositoItem.IsEnabled = false;

                var page = new NotaFiscalConferenciaReparosListaPopUp();
                page._confirmaItemEtiqueta = CodigoBarras;
                await PopupNavigation.Instance.PushAsync(page);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "OK");
            }
            finally
            {
                //BtnEfetivarItem.IsEnabled = true;
            }

        }

        public async void CodigoBarras(string pCodBarras)
        {
            try
            {
                if (String.IsNullOrEmpty(pCodBarras))
                {
                    await DisplayAlert("Erro Leitura da Etiqueta", "Nenhuma etiqueta efetuada a leitura, favor realizar a leitura da etiqueta para seguir com a atualização da nota fiscal.", "OK");
                    return;
                }

                var dRetornoNota = await ValidarReparosNotaFiscalService.SendValidarReparosAsync(new ValidarReparosNotaFiscal
                {
                    CodBarras = pCodBarras
                });

                if (dRetornoNota != null && dRetornoNota.Resultparam != null && dRetornoNota.Resultparam.Count > 0)
                {
                    var v = dRetornoNota.Resultparam.FirstOrDefault();

                    if (v.Mensagem.Contains("ERRO:"))
                    {

                        await DisplayAlert("ERRO!", v.Mensagem, "OK");
                        return;

                    }
                }
            }
            catch (Exception e)
            {
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

                var page = new NotaFiscalConferenciaReparosDetPopUp(current.CodItem, current.DescricaoItem, current.NroDocto, current.NumRR, current.Conferido, current.Relaciona, current.CodFilial);

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
            try
            {
                var lstNotaConferencia = new CollectorQi.Services.ESCL028.FinalizarConferenciaNotaFiscalService.RequestNotaFiscalJson();

                //lstNotaConferencia.
                lstNotaConferencia.Parametros = new FinalizarConferenciaNotaFiscalService.Param();

                lstNotaConferencia.Parametros.CodEstabel = SecurityAuxiliar.GetCodEstabel();

                lstNotaConferencia.ListaReparosConferidos = new List<FinalizarConferenciaReparosConferidos>();
                lstNotaConferencia.ListaConferenciaDocumentos = new List<FinalizarConferenciaDocumentos>();

                foreach (var item in ObsNotaFiscal)
                {

                    lstNotaConferencia.ListaReparosConferidos.Add(new FinalizarConferenciaReparosConferidos { RowId = item.RowId });

                    lstNotaConferencia.ListaConferenciaDocumentos.Add(new FinalizarConferenciaDocumentos {
                    
                        Docto = item.NroDocto,
                        CodEstabel = item.CodEstabel,
                        Relaciona = item.Relaciona,
                        
                        
                        // Outros pontos..

                    
                    });
                }

                var parametrosRetorno = await FinalizarConferenciaNotaFiscalService.SendFinalizarConferenciaAsync(lstNotaConferencia);

                if (parametrosRetorno.Retorno == "OK")
                {
                    await DisplayAlert("Sucesso", "Conferencia efetuada com sucesso", "OK");
                }
                else
                {
                    if (parametrosRetorno.Resultparam != null && parametrosRetorno.Resultparam.Count > 0)
                    {
                        await DisplayAlert("ERRO!", parametrosRetorno.Resultparam[0].ErrorDescription, "OK");
                    }
                    else
                    {
                        await DisplayAlert("ERRO!", "Erro no retorno do envio !", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("ERRO!", ex.Message, "OK");
            }
            finally
            {
                BtnFinalizaConferencia.IsEnabled = true;
            }
        }
    }

    public class NotaFiscalViewModel : NotaFiscalVO
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
    }
}