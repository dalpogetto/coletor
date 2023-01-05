using CollectorQi.Models.ESCL017;
using CollectorQi.Models.ESCL029;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL029;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/*using Rg.Plugins.Popup.Services;
*/

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MovimentoReparosLeituraEtiqueta : ContentPage
    {
        public List<OpcoesTransferenciaMovimentoReparo> _listaOpcoesTransferenciaMovimentoReparo { get; set; }
        public string _rowId { get; set; }
        public string _descOpcao { get; set; }
        public int _opcao { get; set; }
        public ParametrosInventarioReparo _parametrosReparo { get; set; }

        public MovimentoReparosLeituraEtiqueta(List<OpcoesTransferenciaMovimentoReparo> listaOpcoesTransferenciaMovimentoReparo, ParametrosInventarioReparo parametros, string descOpcao, int pOpcao)
        {
            InitializeComponent();

            _listaOpcoesTransferenciaMovimentoReparo = listaOpcoesTransferenciaMovimentoReparo;

            _parametrosReparo = parametros;
            _descOpcao = descOpcao;
            _opcao = pOpcao;

            //Parametros = parametros;

            ChangeSwt();

            //edtScan.Focus();
        }
        protected async override void OnAppearing()
        {
            
            await Task.Run(async () =>
            {
                await Task.Delay(400);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    edtScan.Focus();
               });
            });
        }
        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new MovimentoReparosOpcoesTransferenciaPage(_listaOpcoesTransferenciaMovimentoReparo, _parametrosReparo));

            return true;
        }

        private void Limpar()
        {
            edtScan.Text = "";
            edtFilial.Text = "";
            edtRR.Text = "";
            edtDigito.Text = "";

            _rowId = "";
            edtItem.Text = "";
            edtDescricao.Text = "";
            edtLocalizacao.Text = "";
       //     edtMsg.Text = "";

            edtFilial.Text = "";
            edtRR.Text = "";
            edtDigito.Text = "";

            frameLocalizacao.IsVisible = false;

            ChangeSwt();

            if (SwtCodigoBarras.On)
            {
                edtFilial.Focus();
            }
            else
            {
                edtScan.Focus();
            }
        }
        
        private void BtnLimpar_Clicked(object sender, System.EventArgs e)
        {
            Limpar();
        }

        async void BtnScan_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                BtnScan.IsEnabled = false;

                if (!string.IsNullOrWhiteSpace(edtScan.Text))
                {
                    var parametrosMovimentoReparo = new ParametrosMovimentoReparo()
                    {
                        CodEstabel = "",
                        Opcao = _opcao,
                        CodBarras = edtScan.Text
                    };

                    Leitura(parametrosMovimentoReparo);
                }
            }
            finally
            {
                BtnScan.IsEnabled = true;
            }
        }

        async void BtnLeituraDigitacao_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                BtnLeituraDigitacao.IsEnabled = false;

                if (string.IsNullOrWhiteSpace(edtFilial.Text) && string.IsNullOrWhiteSpace(edtRR.Text) && string.IsNullOrWhiteSpace(edtDigito.Text))
                    await DisplayAlert("Erro", "Digite uma Filial, RR e Dígito !", "OK");
                else
                {
                    var parametrosMovimentoReparo = new ParametrosMovimentoReparo()
                    {
                        CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                        CodFilial = edtFilial.Text,
                        NumRR = edtRR.Text,
                        Digito = edtDigito.Text,
                        Opcao = _opcao,
                        CodBarras = edtScan.Text
                    };

                    Leitura(parametrosMovimentoReparo);
                }
            }
            finally
            {
                BtnLeituraDigitacao.IsEnabled = true;
            }
        }

        async void Leitura(ParametrosMovimentoReparo parametrosMovimentoReparo)
        {
            var pageProgress = new ProgressBarPopUp("Carregando Reparo, aguarde...");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                if (String.IsNullOrEmpty(parametrosMovimentoReparo.CodFilial))
                    parametrosMovimentoReparo.CodFilial = String.Empty;

                if (String.IsNullOrEmpty(parametrosMovimentoReparo.NumRR))
                    parametrosMovimentoReparo.NumRR = String.Empty;
                else
                    parametrosMovimentoReparo.NumRR = parametrosMovimentoReparo.NumRR.Replace(".", ",");

                if (String.IsNullOrEmpty(parametrosMovimentoReparo.Digito))
                    parametrosMovimentoReparo.Digito = String.Empty;
                
                // Conforme alinhado com Kawano, utilizar no código de barras o codigo do estabelecimento local
                // Futuramente alterar API para corrigir atualização
                if (String.IsNullOrEmpty(parametrosMovimentoReparo.CodBarras))
                    parametrosMovimentoReparo.CodBarras = String.Empty;
                else
                    parametrosMovimentoReparo.CodBarras =  SecurityAuxiliar.GetCodEstabel() + parametrosMovimentoReparo.CodBarras.Substring(3);

                var result = await LeituraEtiquetaArmazenagemService.SendLeituraEtiquetaAsync(parametrosMovimentoReparo);

                if (result != null && result.ParamReparo != null && result.ParamReparo.ParamLeitura != null && result.Retorno == "OK")
                {
                    _rowId            = result.ParamReparo.ParamLeitura.RowId;
                    edtItem.Text      = result.ParamReparo.ParamLeitura.CodItem;
                    edtDescricao.Text = result.ParamReparo.ParamLeitura.DescItem;
                    edtLocalizacao.Text = result.ParamReparo.ParamLeitura.Localiza;
                    //edtMsg.Text       = result.ParamReparo.ParamLeitura.Mensagem;
                    edtFilial.Text    = result.ParamReparo.ParamLeitura.CodFilial;
                    edtRR.Text        = result.ParamReparo.ParamLeitura.NumRR.ToString();
                    edtDigito.Text    = result.ParamReparo.ParamLeitura.Digito.ToString();

                    if (!SwtCodigoBarras.On)
                    {
                        frame2.IsVisible = true;
                        BtnLeituraDigitacao.IsVisible = false;
                        frameBuscaReparo.IsVisible = false;
                    }

                    if (String.IsNullOrEmpty(edtLocalizacao.Text))
                    {
                        frameLocalizacao.IsVisible = false;
                    }
                    else
                    {
                        frameLocalizacao.IsVisible = true;
                    }

                    edtScan.IsReadOnly   = true;
                    edtRR.IsReadOnly     = true;
                    edtFilial.IsReadOnly = true;
                    edtDigito.IsReadOnly = true;
                    BtnLeituraDigitacao.IsEnabled = false;

                    // Click Efetivar
                    BtnEfetivar_Clicked(BtnEfetivar, new EventArgs());
                }
                else
                {
                    if (result != null && result.Resultparam != null && result.Resultparam.Count > 0)
                    {
                        await DisplayAlert("Erro", result.Resultparam[0].ErrorHelp, "OK");
                    }
                    else
                    {
                        await DisplayAlert("Erro", "Erro na confirmação do reparo", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
            finally
            {
                await pageProgress.OnClose();
            }
        }

        async void BtnEfetivar_Clicked(object sender, System.EventArgs e)
        {
            var pageProgress = new ProgressBarPopUp("Carregando Reparo, aguarde...");

            try
            {
                BtnEfetivar.IsEnabled = false;
                if (string.IsNullOrWhiteSpace(edtScan.Text) && (string.IsNullOrWhiteSpace(edtFilial.Text) || string.IsNullOrWhiteSpace(edtRR.Text) || string.IsNullOrWhiteSpace(edtDigito.Text)))
                { 
                    await DisplayAlert("Erro", "Faça a leitura da etiqueta ou Digite uma filial, RR e dígito !", "OK");
                    return;
                }

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                var efetivarEtiqueta = new EfetivarArmazenagemService();

                var efetivaReparo = new EfetivaReparo()
                {
                    RowId      = _rowId,
                    Opcao      = _opcao,
                    CodTecnico = _parametrosReparo.CodTecnico,
                    CodEstabel = SecurityAuxiliar.GetCodEstabel()
                };

                var efetivarEtiquetaRetorno = await efetivarEtiqueta.SendParametersAsync(efetivaReparo);

                if (efetivarEtiquetaRetorno != null && efetivarEtiquetaRetorno.ParamConteudo != null)
                {
                    await DisplayAlert("Erro", efetivarEtiquetaRetorno.ParamConteudo[0].ErrorHelp, "OK");
                }
                else
                {
                    //  await DisplayAlert("Movimentação de Reparo", "Movimentação de Reparo efetuado com sucesso","OK");
                    
                    Limpar();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
            finally
            {
                BtnEfetivar.IsEnabled = true;
                await pageProgress.OnClose();
            }
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                ToolBarPrint.IsEnabled = false;

                var pageProgress = new ProgressBarPopUp("Carregando...");
                var page         = new ArmazenagemPrintPopUp(null, null);
                await PopupNavigation.Instance.PushAsync(page);
                await pageProgress.OnClose();
            }
            finally
            {
                ToolBarPrint.IsEnabled = true;
            }
        }


        private void ToolbarLimpar_Clicked(object sender, EventArgs e)
        {
            Limpar();
        }

        private void ChangeSwt()
        {
            if (SwtCodigoBarras.On)
            {
                SwtCodigoBarras.Text          = "Digita Reparo";
                frame1.IsVisible              = false;
                BtnLeituraDigitacao.IsVisible = true;
                frameBuscaReparo.IsVisible    = true;

                if (String.IsNullOrEmpty(_rowId))
                {
                    BtnLeituraDigitacao.IsEnabled = true;
                    edtFilial.IsReadOnly          = false;
                    edtRR.IsReadOnly              = false;
                    edtDigito.IsReadOnly          = false;
                }
            }
            else
            {
                SwtCodigoBarras.Text          = "Código de Barras";
                frame1.IsVisible              = true;
                BtnLeituraDigitacao.IsVisible = false;
                frameBuscaReparo.IsVisible    = false;
                edtFilial.IsReadOnly          = true;
                edtRR.IsReadOnly              = true;
                edtDigito.IsReadOnly          = true;

                if (String.IsNullOrEmpty(_rowId))
                {
                    edtScan.IsReadOnly = false;
                }
            }
        }

        private void SwtCodigoBarras_OnChanged(object sender, ToggledEventArgs e)
        {
            ChangeSwt();
        }

        private void edtScan_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.OldTextValue) && e.NewTextValue.Length > 6)
            {
                var parametrosMovimentoReparo = new ParametrosMovimentoReparo()
                {
                   // CodEstabel = "",
                   /* CodFilial  = edtFilial.Text,
                    NumRR      = edtRR.Text,
                    Digito     = edtDigito.Text,  */
                    Opcao      = _opcao,
                    CodBarras  = edtScan.Text
                };

                Leitura(parametrosMovimentoReparo);
            }
        }
    }
}