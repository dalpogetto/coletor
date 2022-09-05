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

            //var lstNotaFiscal = NotaFiscalDB.GetNotaFiscalAtivoByEstab(SecurityAuxiliar.GetCodEstabel()).OrderBy(p => p.RowId).ToList();
           // var lstNotaFiscal = listNotaFiscalVO = NotaFiscalDB.GetNotaFiscalByEstab("126").OrderBy(p => p.RowId).ToList();

            lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.Estabelecimento;

          //  for (int i = 0; i < lstNotaFiscal.Count(); i++)
          //  { 
          //      var modelView = Mapper.Map<NotaFiscalVO, NotaFiscalViewModel>(lstNotaFiscal[i]);
          //   //   lblCodEstabel.Text = "Estabelecimento: " + lstNotaFiscal[i].CodEstabel;
          //      Estabelecimento = lstNotaFiscal[i].CodEstabel;
          //
          //      ObsNotaFiscal.Add(modelView);
          //  }

            cvNotaFiscal.BindingContext = this;
        }

        private async void CarregaListView()
        {
            // var parametersInventario = new ParametersInventarioService();
            //var lstInventario = await ParametersInventarioService.SendParametersAsync();

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

                //var parametersNotaFiscal = new ParametersNotaFiscalService();
                var listNotaFiscal = new List<ModelNotaFiscal>();
                listaDocumentosNotaFiscal = new List<ListaDocumentosNotaFiscal>();

                var lstNotaFiscal = await ParametersNotaFiscalService.SendParametersAsync(SecurityAuxiliar.GetCodEstabel());

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
                //    lblCodEstabel.Text = "Estabelecimento: " + notaFiscalVO.CodEstabel;
                }

                ConnectService.CriaNotaFiscal(listNotaFiscal);

                for (int i = 0; i < lstNotaFiscal.param.ListaDocumentos.Count(); i++)
                {
                    bool existeItensRestantes = listNotaFiscalVO.Any(x => x.NroDocto == lstNotaFiscal.param.ListaDocumentos[i].Docto && x.Conferido == false);

                    var documentosNotaFiscal = new ListaDocumentosNotaFiscal();
                    documentosNotaFiscal.Atualizar = lstNotaFiscal.param.ListaDocumentos[i].Atualizar;
                    documentosNotaFiscal.Bloqueado = lstNotaFiscal.param.ListaDocumentos[i].Bloqueado;
                    documentosNotaFiscal.CodEmitente = lstNotaFiscal.param.ListaDocumentos[i].CodEmitente;
                    documentosNotaFiscal.CodEstabel = lstNotaFiscal.param.ListaDocumentos[i].CodEstabel;
                    documentosNotaFiscal.Docto = lstNotaFiscal.param.ListaDocumentos[i].Docto;
                    documentosNotaFiscal.Marca = lstNotaFiscal.param.ListaDocumentos[i].Marca;
                    documentosNotaFiscal.NatOperacao = lstNotaFiscal.param.ListaDocumentos[i].NatOperacao;
                    documentosNotaFiscal.NrProcesso = lstNotaFiscal.param.ListaDocumentos[i].NrProcesso;
                    documentosNotaFiscal.Usuario = lstNotaFiscal.param.ListaDocumentos[i].Usuario;
                    documentosNotaFiscal.SerieDocto = lstNotaFiscal.param.ListaDocumentos[i].SerieDocto;
                    documentosNotaFiscal.Relaciona = lstNotaFiscal.param.ListaDocumentos[i].Relaciona;

                    if (existeItensRestantes)
                        documentosNotaFiscal.ItensRestantes = false;
                    else
                        documentosNotaFiscal.ItensRestantes = true;

                    listaDocumentosNotaFiscal.Add(documentosNotaFiscal);
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

            /*

            var pageProgress = new ProgressBarPopUp("Carregando inventário, aguarde...");

            try
            {

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                ObsInventario = new ObservableCollection<InventarioViewModel>();

                lblCodEstabel.Text = SecurityAuxiliar.Estabelecimento;

                var lstInventario = InventarioDB.GetInventarioAtivoByEstab(SecurityAuxiliar.GetCodEstabel()).OrderBy(p => p.CodDepos).OrderBy(p => p.DtInventario).ToList();

                for (int i = 0; i < lstInventario.Count(); i++)
                {
                    var modelView = Mapper.Map<InventarioVO, InventarioViewModel>(lstInventario[i]);
                    ObsInventario.Add(modelView);
                }

                if (ObsInventario.Count <= 0)
                {
                    await pageProgress.OnClose();
                    OnClick_CarregaInventario(new object(), new EventArgs());
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

            OnPropertyChanged("ObsInventario");
            */
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
                //cvGuardaMateriaisDepositoItem.IsEnabled = false;

                var page = new NotaFiscalConferenciaReparosListaPopUp();
                //page._confirmaItem = CodigoBarras;
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

            /*
            var parametersNotaFiscal = new ValidarReparosNotaFiscalService();
            var validarReparosNotaFiscal = new ValidarReparosNotaFiscal() { CodBarras = "" };           

            var lstNotaFiscal = await parametersNotaFiscal.SendValidarReparosAsync(validarReparosNotaFiscal);  

            foreach (var item in lstNotaFiscal.Resultparam)            
                Models.ConnectService.AtualizaNotaFiscal(item);

            var lstNotaFiscalRetorno = NotaFiscalDB.GetNotaFiscalByEstab(Estabelecimento).OrderBy(p => p.RowId).ToList();

            for (int i = 0; i < lstNotaFiscalRetorno.Count(); i++)
            {
                var modelView = Mapper.Map<NotaFiscalVO, NotaFiscalViewModel>(lstNotaFiscalRetorno[i]);
                ObsNotaFiscal.Add(modelView);
            }

            cvNotaFiscal.BindingContext = this; */
        }

        /*
        private void BtnAtualizarNotaFiscal_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new NotaFiscalFinalizarConferenciaListaPage(listNotaFiscalVO, listaDocumentosNotaFiscal));
        } */

        public async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cvNotaFiscal.SelectedItem == null)
                return;

            var current = (cvNotaFiscal.SelectedItem as NotaFiscalViewModel);

            if (current != null)
            {
                //Application.Current.MainPage = new NavigationPage(new InventarioListaLocalizacaoPage(current));
                Application.Current.MainPage = new NavigationPage(new NotaFiscalFinalizarConferenciaListaPage(listNotaFiscalVO, listaDocumentosNotaFiscal));
            }

            cvNotaFiscal.SelectedItem = null;
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