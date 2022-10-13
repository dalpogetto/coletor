using CollectorQi.Models.ESCL017;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL017;
using CollectorQi.Services.ESCL029;
using Rg.Plugins.Popup.Services;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ArmazenagemMovimentoReparoPage : ContentPage, INotifyPropertyChanged
    {
        public ParametrosInventarioReparo ParametrosInventarioReparo { get; set; }

        public ArmazenagemMovimentoReparoPage(ParametrosInventarioReparo parametrosInventarioReparo)
        {
            InitializeComponent();

            ParametrosInventarioReparo = parametrosInventarioReparo;
            lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.Estabelecimento;

            if (parametrosInventarioReparo != null)
            {
                if (!string.IsNullOrEmpty(parametrosInventarioReparo.CodTecnico.ToString()))
                    txtTecnico.Text = parametrosInventarioReparo.CodTecnico.ToString();

                txtDeposito.Text = parametrosInventarioReparo.CodDepos;
                txtSenha.Text = parametrosInventarioReparo.Senha;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new ArmazenagemPage();

            return true;
        }

        async void BtnIniciarReparoInventario_Clicked(object sender, System.EventArgs e)
        {
            var parametrosIR = new ParametrosInventarioReparo();
            parametrosIR.CodEstabel = SecurityAuxiliar.GetCodEstabel();

            if (string.IsNullOrEmpty(txtTecnico.Text) || string.IsNullOrEmpty(txtSenha.Text) ||
               string.IsNullOrEmpty(txtDeposito.Text))
                await DisplayAlert("", "Todos os campos são obrigatórios !", "OK");
            else
            {


             //  DateTime valor;
             //  parametrosIR.CodTecnico = int.Parse(txtTecnico.Text);
             //  parametrosIR.Senha = txtSenha.Text;
             //  parametrosIR.CodDepos = txtDeposito.Text;
             //
             //  var pInventario = new ParametersInventarioReparoService();
             //
             //  var parametrosRetorno = await pInventario.SendParametersAsync(parametrosIR);
             //
             //  if (parametrosRetorno.Retorno == "OK")
             //      Application.Current.MainPage = new NavigationPage(new InventarioReparoLeituraEtiquetaListaPage(null, parametrosIR));
             //  else
             //      await DisplayAlert("", "Erro no retorno do envio !", "OK");
            }
        }

        async void BtnBuscaDeposito_Clicked(object sender, System.EventArgs e)
        {
            BtnBuscaDeposito.IsEnabled = false;

            try
            {

                var page = new DepositoPopUp(txtDeposito);
                //page._confirmaItem = CodigoBarras;
                await PopupNavigation.Instance.PushAsync(page);

                //  var parametrosIR = new ParametrosInventarioReparo();
                //  parametrosIR.CodEstabel = SecurityAuxiliar.GetCodEstabel();
                //
                //  if (!string.IsNullOrEmpty(txtTecnico.Text))
                //      parametrosIR.CodTecnico = int.Parse(txtTecnico.Text);
                //
                //  parametrosIR.Senha = txtSenha.Text;
                //  parametrosIR.CodDepos = txtDeposito.Text;
                //
                //  var dInventario = new DepositoInventarioReparoService();
                //  var dInventarioRetorno = await dInventario.SendParametersAsync();
                //
                //  Application.Current.MainPage = new NavigationPage(new InventarioReparoDepositoListaPage(dInventarioRetorno.Param.Resultparam, parametrosIR, "MovimentoReparo"));
            }
            catch (Exception ex)
            {

            }
            finally
            {
                BtnBuscaDeposito.IsEnabled = true;
            }
        }

        async void BtnAcessar_Clicked(object sender, EventArgs e)
        {
            BtnAcessar.IsEnabled = false;

            var pageProgress = new ProgressBarPopUp("Acessando, aguarde...");

            try
            {

                if (string.IsNullOrWhiteSpace(txtTecnico.Text))
                    await DisplayAlert("", "Digite um codigo técnico !", "OK");
                else if (string.IsNullOrWhiteSpace(txtSenha.Text))
                    await DisplayAlert("", "Digite uma senha !", "OK");
                else if (string.IsNullOrWhiteSpace(txtDeposito.Text))
                    await DisplayAlert("", "Escolha um deposito!", "OK");
                else
                {
                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);


                    var parametros = new ParametrosInventarioReparo()
                    {
                        CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                        CodTecnico = int.Parse(txtTecnico.Text),
                        Senha = txtSenha.Text,
                        CodDepos = txtDeposito.Text
                    };

                    // var pArmazenagem = new ParametrosArmazenagemService();
                    var result = await ParametrosArmazenagemService.SendParametersAsync(parametros);

                    await pageProgress.OnClose();

                    //result.Retorno = "OK";

                    if (result != null && result.Retorno == "OK")
                    {
                        var opcoesTransferencia = new OpcoesTransferenciaService();
                        var oOpcoesTransferencia = await opcoesTransferencia.SendOpcoesTransferenciaAsync();

                        if (oOpcoesTransferencia != null && oOpcoesTransferencia.ResultConteudo != null && oOpcoesTransferencia.ResultConteudo.ResultParam != null)
                        {
                            if (ParametrosInventarioReparo == null)
                            {
                                ParametrosInventarioReparo = new ParametrosInventarioReparo();

                                ParametrosInventarioReparo.CodDepos = "DET";
                                ParametrosInventarioReparo.CodTecnico = 4122;

                            }
                            Application.Current.MainPage = new NavigationPage(new MovimentoReparosOpcoesTransferenciaPage(oOpcoesTransferencia.ResultConteudo.ResultParam, ParametrosInventarioReparo));
                        }
                    }
                    else
                    {
                        if (result.Resultparam != null && result.Resultparam.Count > 0)
                        {
                            await DisplayAlert("Erro!", result.Resultparam[0].ErrorHelp, "Cancelar");
                        }
                        else
                        {
                            await DisplayAlert("Erro!", "Erro ao imprimir etiqueta", "Cancelar");
                        }
                    }

                    /*
                    if (pArmazenagemRetorno.Retorno == "OK")
                    {
                        // Obter funcoes transferencia
                        var opcoesTransferencia = new OpcoesTransferenciaService();
                        var oOpcoesTransferencia = await opcoesTransferencia.SendOpcoesTransferenciaAsync();

                        Application.Current.MainPage = new NavigationPage(new MovimentoReparosOpcoesTransferenciaPage(oOpcoesTransferencia.ResultConteudo.ResultParam, ParametrosInventarioReparo));
                    }
                    else
                        await DisplayAlert("", pArmazenagemRetorno.Retorno, "OK");
                    */
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                BtnAcessar.IsEnabled = true;
                await pageProgress.OnClose();
            }
        }
    }
}   