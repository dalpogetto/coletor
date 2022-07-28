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

        public NotaFiscalConferenciaReparosListaPage()
        {
            InitializeComponent();

            ObsNotaFiscal = new ObservableCollection<NotaFiscalViewModel>();

            //var lstNotaFiscal = NotaFiscalDB.GetNotaFiscalAtivoByEstab(SecurityAuxiliar.GetCodEstabel()).OrderBy(p => p.RowId).ToList();
            var lstNotaFiscal = NotaFiscalDB.GetNotaFiscalAtivoByEstab("126").OrderBy(p => p.RowId).ToList();

            for (int i = 0; i < lstNotaFiscal.Count(); i++)
            { 
                var modelView = Mapper.Map<NotaFiscalVO, NotaFiscalViewModel>(lstNotaFiscal[i]);
                lblCodEstabel.Text = lstNotaFiscal[i].CodEstabel; // + " - " + lstNotaFiscal[i].DescEstabel;

                ObsNotaFiscal.Add(modelView);
            }

            cvNotaFiscal.BindingContext = this;
        }       

        async void OnClick_CarregaNotaFiscal(object sender, EventArgs e)
        {
            var current = (cvNotaFiscal.SelectedItem as VO.NotaFiscalVO);

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
                var lstNotaFiscal = await parametersNotaFiscal.SendParametersAsync();
                var listNotaFiscal = new List<ModelNotaFiscal>();

                foreach (var item in lstNotaFiscal.param.Resultparam)
                {
                    var notaFiscal = new ModelNotaFiscal();
                    notaFiscal.rowId = item.RowId;
                    notaFiscal.codEstabel = item.CodEstabel;
                    notaFiscal.codItem = item.CodItem;
                    notaFiscal.localizacao = item.Localizacao;
                    notaFiscal.descricaoItem = item.DescricaoItem;
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
                    notaFiscalVO.DescricaoItem = item.DescricaoItem;
                    notaFiscalVO.NroDocto = item.NroDocto;
                    notaFiscalVO.NumRR = item.NumRR;
                    notaFiscalVO.Conferido = item.Conferido;
                    notaFiscalVO.Relaciona = item.Relaciona;
                    notaFiscalVO.CodFilial = item.CodFilial;

                    var modelView = Mapper.Map<NotaFiscalVO, NotaFiscalViewModel>(notaFiscalVO);
                    ObsNotaFiscal.Add(modelView);
                }               

                Models.Controller.CriaNotaFiscal(listNotaFiscal);
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

            var lstNotaFiscalRetorno = NotaFiscalDB.GetNotaFiscalByEstab("126").OrderBy(p => p.RowId).ToList();

            for (int i = 0; i < lstNotaFiscalRetorno.Count(); i++)
            {
                var modelView = Mapper.Map<NotaFiscalVO, NotaFiscalViewModel>(lstNotaFiscalRetorno[i]);
                ObsNotaFiscal.Add(modelView);
            }

            cvNotaFiscal.BindingContext = this;
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