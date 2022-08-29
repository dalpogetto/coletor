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

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotaFiscalConferenciaReparosListaPage : ContentPage, INotifyPropertyChanged
    {  
        public ObservableCollection<NotaFiscalViewModel> ObsNotaFiscal { get; }
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
            var lstNotaFiscal = listNotaFiscalVO = NotaFiscalDB.GetNotaFiscalByEstab("126").OrderBy(p => p.RowId).ToList();       

            for (int i = 0; i < lstNotaFiscal.Count(); i++)
            { 
                var modelView = Mapper.Map<NotaFiscalVO, NotaFiscalViewModel>(lstNotaFiscal[i]);
                lblCodEstabel.Text = "Estabelecimento: " + lstNotaFiscal[i].CodEstabel;
                Estabelecimento = lstNotaFiscal[i].CodEstabel;

                ObsNotaFiscal.Add(modelView);
            }

            cvNotaFiscal.BindingContext = this;
        }       

        async void OnClick_CarregaNotaFiscal(object sender, EventArgs e)
        {
            //var current = (cvNotaFiscal.SelectedItem as NotaFiscalVO);

            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Erro!", "Para buscar as Notas Fiscal no sistema o dispositivo deve estar conectado.", "CANCELAR");

                return;
            }

            BtnCarregaNotaFiscal.IsEnabled = false;

            var pageProgress = new ProgressBarPopUp("Carregando Notas Fiscal, aguarde...");

            try
            {
                ObsNotaFiscal.Clear();

                var parametersNotaFiscal = new ParametersNotaFiscalService();
                var listNotaFiscal = new List<ModelNotaFiscal>();
                listaDocumentosNotaFiscal = new List<ListaDocumentosNotaFiscal>();

                var lstNotaFiscal = await parametersNotaFiscal.SendParametersAsync();

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
                        if(descricao != "")
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
                    lblCodEstabel.Text = "Estabelecimento: " + notaFiscalVO.CodEstabel;
                }               

                Models.Controller.CriaNotaFiscal(listNotaFiscal);

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

                    if(existeItensRestantes)
                        documentosNotaFiscal.ItensRestantes = false;
                    else
                        documentosNotaFiscal.ItensRestantes = true;

                    listaDocumentosNotaFiscal.Add(documentosNotaFiscal);
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

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new PrincipalPage());

            return true;
        }

        async void BtnScan_Clicked(object sender, EventArgs e)
        {            
            var parametersNotaFiscal = new ValidarReparosNotaFiscalService();
            var validarReparosNotaFiscal = new ValidarReparosNotaFiscal() { CodBarras = "" };           

            var lstNotaFiscal = await parametersNotaFiscal.SendValidarReparosAsync(validarReparosNotaFiscal);  

            foreach (var item in lstNotaFiscal.Resultparam)            
                Models.Controller.AtualizaNotaFiscal(item);

            var lstNotaFiscalRetorno = NotaFiscalDB.GetNotaFiscalByEstab(Estabelecimento).OrderBy(p => p.RowId).ToList();

            for (int i = 0; i < lstNotaFiscalRetorno.Count(); i++)
            {
                var modelView = Mapper.Map<NotaFiscalVO, NotaFiscalViewModel>(lstNotaFiscalRetorno[i]);
                ObsNotaFiscal.Add(modelView);
            }

            cvNotaFiscal.BindingContext = this;
        }

        private void BtnAtualizarNotaFiscal_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new NotaFiscalFinalizarConferenciaListaPage(listNotaFiscalVO, listaDocumentosNotaFiscal));
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