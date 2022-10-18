using CollectorQi.Models.ESCL017;
using CollectorQi.Models.ESCL029;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL029;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/*using Rg.Plugins.Popup.Services;
*/

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MovimentoReparosLeituraEtiqueta : ContentPage
    {
        public List<OpcoesTransferenciaMovimentoReparo> ListaOpcoesTransferenciaMovimentoReparo { get; set; }
        public string RowId { get; set; }
        public ParametrosInventarioReparo Parametros { get; set; }

        public MovimentoReparosLeituraEtiqueta(List<OpcoesTransferenciaMovimentoReparo> listaOpcoesTransferenciaMovimentoReparo, ParametrosInventarioReparo parametros, string descOpcao)
        {
            InitializeComponent();

            ListaOpcoesTransferenciaMovimentoReparo = listaOpcoesTransferenciaMovimentoReparo;
            lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.Estabelecimento;
            lblDep.Text = "Dep: " + parametros.CodDepos;
            lblTec.Text = "Tec.: " + parametros.CodTecnico;
            lblDescOpcao.Text = descOpcao;

            //Parametros = parametros;
            BtnEfetivar.IsEnabled = false;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new MovimentoReparosOpcoesTransferenciaPage(ListaOpcoesTransferenciaMovimentoReparo, Parametros));

            return true;
        }

        private void BtnLimpar_Clicked(object sender, System.EventArgs e)
        {
            edtScan.Text = "";
            edtFilial.Text = "";
            edtRR.Text = "";
            edtDigito.Text = "";
            edtItem.Text = "";
            edtDescricao.Text = "";
            edtLocal.Text = "";
            edtMsg.Text = "";
        }

        async void BtnScan_Clicked(object sender, System.EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(edtScan.Text))
                await DisplayAlert("", "Faça a leitura da etiqueta !", "OK");
            else
            {
                var parametrosMovimentoReparo = new ParametrosMovimentoReparo()
                {
                    CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                    Opcao = 1,
                    CodBarras = ""
                };

                Leitura(parametrosMovimentoReparo);
            }
        }

        async void BtnLeitura_Clicked(object sender, System.EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(edtFilial.Text) && string.IsNullOrWhiteSpace(edtRR.Text) && string.IsNullOrWhiteSpace(edtDigito.Text))
                await DisplayAlert("", "Digite uma Filial, RR e Dígito !", "OK");
            else
            {
                var parametrosMovimentoReparo = new ParametrosMovimentoReparo()
                {
                    CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                    CodFilial = edtFilial.Text,
                    NumRR = edtRR.Text,
                    Digito = edtDigito.Text,
                    Opcao = 1,
                    CodBarras = ""
                };

                Leitura(parametrosMovimentoReparo);
            }
        }

        async void Leitura(ParametrosMovimentoReparo parametrosMovimentoReparo)
        {
            var leituraEtiqueta = new LeituraEtiquetaArmazenagemService();
            var leituraEtiquetaRetorno = await leituraEtiqueta.SendLeituraEtiquetaAsync(parametrosMovimentoReparo);

            RowId = leituraEtiquetaRetorno.ParamReparo.ParamLeitura.RowId;
            edtItem.Text = leituraEtiquetaRetorno.ParamReparo.ParamLeitura.CodItem;
            edtDescricao.Text = leituraEtiquetaRetorno.ParamReparo.ParamLeitura.DescItem;
            edtLocal.Text = leituraEtiquetaRetorno.ParamReparo.ParamLeitura.Localiza;
            edtMsg.Text = leituraEtiquetaRetorno.ParamReparo.ParamLeitura.Mensagem;

            BtnEfetivar.IsEnabled = true;
        }

        async void BtnEfetivar_Clicked(object sender, System.EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(edtScan.Text) && (string.IsNullOrWhiteSpace(edtFilial.Text) || string.IsNullOrWhiteSpace(edtRR.Text) || string.IsNullOrWhiteSpace(edtDigito.Text)))
                await DisplayAlert("", "Faça a leitura da etiqueta ou Digite uma filial, RR e dígito !", "OK");
            else
            {
                var efetivarEtiqueta = new EfetivarArmazenagemService();

                var leituraMovimentoReparo = new LeituraMovimentoReparo()
                {
                    RowId = RowId,
                    Opcao = 2
                };

                var efetivarEtiquetaRetorno = await efetivarEtiqueta.SendParametersAsync(leituraMovimentoReparo);

                await DisplayAlert("", efetivarEtiquetaRetorno.ParamConteudo.ParamOK[0].Mensagem, "OK");
            }
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                ToolBarPrint.IsEnabled = false;

                var pageProgress = new ProgressBarPopUp("Carregando...");
                var page = new ArmazenagemPrintPopUp(null, null);
                await PopupNavigation.Instance.PushAsync(page);
                await pageProgress.OnClose();
            }
            finally
            {
                ToolBarPrint.IsEnabled = true;
            }
        }

    }
}