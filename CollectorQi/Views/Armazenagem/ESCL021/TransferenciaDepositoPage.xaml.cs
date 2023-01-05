using CollectorQi.Models.ESCL021;
using CollectorQi.Models.ESCL027;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL021;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/*using Rg.Plugins.Popup.Services;
*/

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TransferenciaDepositoPage : ContentPage
    {
        //private static int itemListaColetor = 0;
        //private static ItemColetor itemColetor;

        //private static int menuId = 0;
        //private static string menuDesc = "";
        //private static bool volta = false;
        //private bool _blnClickQr { get; set; }

        //public static int MenuId { get => menuId; set => menuId = value; }
        //public static string MenuDesc { get => menuDesc; set => menuDesc = value; }
        //public static bool Volta { get => volta; set => volta = value; }

        public ParametrosDepositosGuardaMaterial ParametrosDepositosGuardaMaterial;
        public bool LocalizacaoStatus { get; set; } = true;

        string _codDeposSaidaHidden;
        string _codDeposEntradaHidden;

        public TransferenciaDepositoPage(ParametrosDepositosGuardaMaterial parametrosDepositosGuardaMaterial)
        {
            InitializeComponent();

            lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.Estabelecimento;
            ParametrosDepositosGuardaMaterial = parametrosDepositosGuardaMaterial;

            edtItCodigo.Completed += (sender, e) => edtItCodigo_Completed(sender, e);

            if (parametrosDepositosGuardaMaterial != null)
            {
                if (parametrosDepositosGuardaMaterial.LocalizacaoStatus)
                {
                    BtnLocalizacaoSaida.IsEnabled = true;
                    BtnLocalizacaoEntrada.IsEnabled = true;
                }
                else
                    LocalizacaoStatus = false;

                edtDepositoEntrada.Text = parametrosDepositosGuardaMaterial.DepositoEntrada;
                edtDepositoSaida.Text = parametrosDepositosGuardaMaterial.DepositoSaida;
                edtLocalizacaoSaida.Text = parametrosDepositosGuardaMaterial.LocalizacaoSaida;
                edtLocalizacaoEntrada.Text = parametrosDepositosGuardaMaterial.LocalizacaoEntrada;
                edtItCodigo.Text = parametrosDepositosGuardaMaterial.CodItem;
             //   edtDescItem.Text = parametrosDepositosGuardaMaterial.DescItem;
             //   edtUnidade.Text = parametrosDepositosGuardaMaterial.Un;
             //   edtTipoConta.Text = parametrosDepositosGuardaMaterial.Conta;
                edtNroDocto.Text = parametrosDepositosGuardaMaterial.NF;
                edtSerie.Text = parametrosDepositosGuardaMaterial.Serie;
                edtLote.Text = parametrosDepositosGuardaMaterial.Lote;
               // edtSaldo.Text = parametrosDepositosGuardaMaterial.Saldo;
                edtQuantidade.Text = parametrosDepositosGuardaMaterial.Quantidade;
            }
        }

        //void Fill(ItemVO byItemVO)
        //{
        //    //edtItCodigo.Text = byItemVO.ItCodigo;
        //    //edtDescItem.Text = byItemVO.DescItem;
        //    //edtUnidade.Text = byItemVO.Un;
        //}

        void OnClick_Sair(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new AlmoxarifadoPage());
        }

        void OnClick_Limpar(object sender, EventArgs e)
        {
            Limpar(true);
        }

        async Task DepositoSaida(string tipoTransferencia)
        {
            //var parametrosDepositosGuardaMaterialLocal = new ParametrosDepositosGuardaMaterial();
            //
            //if (ParametrosDepositosGuardaMaterial == null)
            //    ParametrosDepositosGuardaMaterial = new ParametrosDepositosGuardaMaterial();
            //
            //parametrosDepositosGuardaMaterialLocal = ParametrosDepositosGuardaMaterial;
            //parametrosDepositosGuardaMaterialLocal.TipoTransferencia = tipoTransferencia;
            //
            //await BuscaDeposito(parametrosDepositosGuardaMaterialLocal);
        }

        async void OnClick_DepositoSaida(object sender, EventArgs e)
        {
            await BuscaDeposito("Saida");
        }

        async void OnClick_DepositoEntrada(object sender, EventArgs e)
        {
            await BuscaDeposito("Entrada");
        }
        public void SetDepos (string pCodDepos, string pTipoTransacao, bool depLab)
        {
            if (pTipoTransacao == "Entrada")
            {
                _codDeposEntradaHidden = pCodDepos;

                if (depLab)
                {
                    frmLocalizacaoEntrada.IsVisible = true;
                }
                else
                {
                    frmLocalizacaoEntrada.IsVisible = false;
                }
            }
            else
            {
                _codDeposSaidaHidden = pCodDepos;

                if (depLab)
                {
                    frmLocalizacaoSaida.IsVisible = true;
                }
                else
                {
                    frmLocalizacaoSaida.IsVisible = false;
                }
            }
        }

        async Task BuscaDeposito(string pTipoTransacao)
        {
            BtnDepositoEntrada.IsEnabled = false;
            BtnDepositoSaida.IsEnabled = false;

            try
            {
                var edtCustomText = pTipoTransacao == "Entrada" ? edtDepositoEntrada : edtDepositoSaida;

                var page = new DepositosUsuarioPorTransacaoListaPopUp(edtCustomText, pTipoTransacao);

                page._setDepos = SetDepos;

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page);

            }
            catch (Exception)
            {

            }
            finally
            {
                BtnDepositoEntrada.IsEnabled = true;
                BtnDepositoSaida.IsEnabled = true;
            }

        }

        async void BtnLocalizacaoSaida_Clicked(object sender, EventArgs e)
        {
            // Ler codigo de barras
            string codigoBarras = "";

            var dLeituraEtiquetaLocaliza = new LeituraEtiquetaLocalizaTransferenciaDepositoService();

          //  var dadosLeituraLocalizaGuardaMaterial = new DadosLeituraLocalizaGuardaMaterial() { CodEstabel = SecurityAuxiliar.GetCodEstabel(), CodigoBarras = codigoBarras };
          //
          //  var dGuardaMaterialRetorno = await dLeituraEtiquetaLocaliza.SendLeituraEtiquetaLocalizaAsync(dadosLeituraLocalizaGuardaMaterial);
          //
          //  edtLocalizacaoSaida.Text = dGuardaMaterialRetorno.Result.Local;
        }

        async void BtnLocalizacaoEntrada_Clicked(object sender, EventArgs e)
        {
            // Ler codigo de barras
            string codigoBarras = "";

            var dLeituraEtiquetaLocaliza = new LeituraEtiquetaLocalizaTransferenciaDepositoService();

        //    var dadosLeituraLocalizaGuardaMaterial = new DadosLeituraLocalizaGuardaMaterial() { CodEstabel = SecurityAuxiliar.GetCodEstabel(), CodigoBarras = codigoBarras };
        //
        //    var dGuardaMaterialRetorno = await dLeituraEtiquetaLocaliza.SendLeituraEtiquetaLocalizaAsync(dadosLeituraLocalizaGuardaMaterial);
        //
        //    edtLocalizacaoEntrada.Text = dGuardaMaterialRetorno.Result.Local;
        }


        async void OnClick_QR(object sender, EventArgs e)
        {
            /*
            var iTemLeituraEtiquetaService = new LeituraEtiquetaTransferenciaDepositoService();

            var dadosLeituraItemTransferenciaDeposito = new DadosLeituraItemTransferenciaDeposito()
            {
                CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                CodDeposOrigem = edtDepositoSaida.Text,
                CodLocalizaOrigem = edtLocalizacaoSaida.Text,
                CodigoBarras = ""
            };

            var iTemLeituraEtiquetaServiceRetorno = await iTemLeituraEtiquetaService.SendLeituraEtiquetaAsync(dadosLeituraItemTransferenciaDeposito);

            foreach (var item in iTemLeituraEtiquetaServiceRetorno.Param.ParamResult)
            {
                edtItCodigo.Text = item.CodItem;
                /* edtDescItem.Text = item.DescItem;
                edtUnidade.Text = item.Un;
                edtTipoConta.Text = item.Conta; 
                edtNroDocto.Text = item.NF;
                edtSerie.Text = item.Serie;
                edtLote.Text = item.Lote;
               // edtSaldo.Text = item.Saldo.ToString();
                edtQuantidade.Text = item.Quantidade.ToString();
            } */
        }

        async void OnClick_Efetivar(object sender, EventArgs e)
        {
            BtnEfetivar.IsEnabled = false;

            var pageProgress = new ProgressBarPopUp("Efetivando transferencia, aguarde...");

            var result = await DisplayAlert("Confirmação!", $"Deseja efetivar a transferência {edtQuantidade.Text} produto?", "Sim", "Não");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                if (result.ToString() == "True")
                {

                    //if (LocalizacaoStatus != false)
                    //{
                    //    if (String.IsNullOrWhiteSpace(edtLocalizacaoSaida.Text))
                    //    {
                    //        edtLocalizacaoSaida.Focus();
                    //        await DisplayAlert("", "Localização Saída não encontrada", "OK");
                    //        return;
                    //    }
                    //    else if (String.IsNullOrWhiteSpace(edtLocalizacaoEntrada.Text))
                    //    {
                    //        edtLocalizacaoEntrada.Focus();
                    //        await DisplayAlert("", "Localização Entrada não encontrada", "OK");
                    //        return;
                    //    }
                    //}

                    if (String.IsNullOrWhiteSpace(edtDepositoSaida.Text))
                    {
                        edtDepositoSaida.Focus();
                        await DisplayAlert("", "Deposito Saída não encontrada", "OK");
                    }
                    else if (String.IsNullOrWhiteSpace(edtDepositoEntrada.Text))
                    {
                        edtDepositoEntrada.Focus();
                        await DisplayAlert("", "Deposito Entrada não encontrada", "OK");
                    }
                    else if (String.IsNullOrWhiteSpace(edtItCodigo.Text))
                    {
                        edtItCodigo.Focus();
                        await DisplayAlert("", "Item não encontrado", "OK");
                    }
                    else if (String.IsNullOrWhiteSpace(edtQuantidade.Text))
                    {
                        edtQuantidade.Focus();
                        await DisplayAlert("", "Quantidade não encontrada", "OK");
                    }
                    else
                    {
                        var efetivarTransferencia = new EfetivarTransferenciaDepositoService();

                        var dadosLeituraDadosItemTransferenciaDeposito = new DadosLeituraDadosItemTransferenciaDeposito()
                        {
                            CodEstabel        = SecurityAuxiliar.GetCodEstabel(),
                            CodDeposOrigem    = _codDeposSaidaHidden,
                            CodDeposDest      = _codDeposEntradaHidden,
                            CodLocalizaOrigem = edtLocalizacaoSaida.Text ?? String.Empty,
                            CodLocalizaDest   = edtLocalizacaoEntrada.Text ?? String.Empty,
                            NF                = edtNroDocto.Text ?? String.Empty,
                            Serie             = edtSerie.Text ?? String.Empty,
                            CodItem           = edtItCodigo.Text ?? String.Empty,
                            Lote              = edtLote.Text ?? String.Empty,
                            Quantidade        = edtQuantidade.Text
                        };

                        var efetivarTransferenciaRetorno = await efetivarTransferencia.SendTransferenciaDepositoAsync(dadosLeituraDadosItemTransferenciaDeposito);

                        //await DisplayAlert("", efetivarTransferenciaRetorno.Param.OK, "OK");

                        await pageProgress.OnClose();

                        if (efetivarTransferenciaRetorno != null && efetivarTransferenciaRetorno.Retorno == "OK")
                        {
                            await DisplayAlert("Sucesso!", "Transfência efetuada com sucesso!", "OK");
                            Limpar(false);
                            // OnBackButtonPressed();
                        }
                        else
                        {
                            if (efetivarTransferenciaRetorno != null && efetivarTransferenciaRetorno.Resultparam != null && efetivarTransferenciaRetorno.Resultparam.Count > 0)
                            {
                                await DisplayAlert("Erro!", efetivarTransferenciaRetorno.Resultparam[0].ErrorHelp, "Cancelar");
                            }
                            else
                            {
                                await DisplayAlert("Erro!", "Erro ao efetivar transferencia", "Cancelar");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                BtnEfetivar.IsEnabled = true;
                await pageProgress.OnClose();
            }


            //ListaItensColetor.ListaItens.Add(itemColetor);

            //Application.Current.MainPage = new NavigationPage(new ConferenciaPage());
            //return;

            //BtnEfetivar.IsEnabled = false;
            //bool blnProgress = false;
            //var pageProgress = new ProgressBarPopUp("Efetuando Transferência...");

            //try
            //{

            //    if (String.IsNullOrEmpty(edtItCodigo.Text))
            //    {
            //        edtItCodigo.Focus();
            //        throw new Exception("Item não encontrado");
            //    }

            //    if (String.IsNullOrEmpty(edtDepSaida.Text))
            //    {
            //        edtDepSaida.Focus();
            //        throw new Exception("Depósito Saída não encontrado");
            //    }

            //    if (String.IsNullOrEmpty(edtDepEntrada.Text))
            //    {
            //        edtDepEntrada.Focus();
            //        throw new Exception("Depósito Entrada não encontrado");
            //    }

            //    if (String.IsNullOrEmpty(edtQuantidade.Text) ||
            //        edtQuantidade.Text == "0")
            //    {
            //        edtQuantidade.Focus();
            //        throw new Exception("Quantidade invalida");
            //    }

            //    if (edtDepEntrada.Text == edtDepSaida.Text)
            //    {
            //        edtDepEntrada.Focus();
            //        throw new Exception("Depósito Entrada deve ser diferente do atual");
            //    }

            //    if (decimal.Parse(edtSaldoMobile.Text) - decimal.Parse(edtQuantidade.Text) < 0)
            //    {
            //        edtQuantidade.Focus();
            //        throw new Exception("Movimentação não atualizada, saldo do depósito " + edtDepSaida.Text + " não pode ser negativo. ");
            //    }

            //    bool blnAlert = await DisplayAlert("Transferência de depósito?", "Confirma a transferência de depósito?", "Sim", "Não");

            //    blnProgress = true;

            //    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

            //    if (!blnAlert)
            //        return;

            //    string strCodEstabel = SecurityAuxiliar.GetCodEstabel();
            //    string strCodDepSaida = edtDepSaida.Text;
            //    string strCodDepEntrada = edtDepEntrada.Text;

            //    if (strCodDepSaida.IndexOf('(') > 0)
            //        strCodDepSaida = strCodDepSaida.Remove(strCodDepSaida.IndexOf('('));

            //    if (strCodDepEntrada.IndexOf('(') > 0)
            //        strCodDepEntrada = strCodDepEntrada.Remove(strCodDepEntrada.IndexOf('('));

            //    List<ModelDepositoTransfere> lstDepositoTransfere = new List<ModelDepositoTransfere>();


            //    ModelDepositoTransfere mDepositoTransfer = new ModelDepositoTransfere
            //    {
            //        codEstabel = strCodEstabel.Trim(),
            //        itCodigo = edtItCodigo.Text.Trim(),
            //        nroDocto = edtNroDocto.Text == null ? String.Empty : edtNroDocto.Text.Trim(),
            //        codDeposSaida = strCodDepSaida.Trim(),
            //        codLote = edtLote.Text.Trim(),
            //        codLocaliz = edtCodLocaliz.Text.Trim(),
            //        /*  dtValiLote =  DateTime(int.Parse(edtDtValiLote.Text.Substring(6)),
            //                                                                                int.Parse(edtDtValiLote.Text.Substring(3, 2)),
            //                                                                                int.Parse(edtDtValiLote.Text.Substring(0, 2))) */
            //        codDeposEntrada = strCodDepEntrada.Trim(),
            //        qtidadeTransf = Decimal.Parse(edtQuantidade.Text.Trim()),
            //        codUsuario = SecurityAuxiliar.CodUsuario.Trim()

            //    };

            //    if (!String.IsNullOrEmpty(edtDtValiLote.Text))
            //        mDepositoTransfer.dtValiLote = new DateTime(int.Parse(edtDtValiLote.Text.Substring(6)),
            //                                                    int.Parse(edtDtValiLote.Text.Substring(3, 2)),
            //                                                    int.Parse(edtDtValiLote.Text.Substring(0, 2)));

            //    lstDepositoTransfere.Add(mDepositoTransfer);

            //    var tplRetorno = Models.Datasul.IntegracaoOnlineBatch.DepositoTransfere(lstDepositoTransfere);

            //    await DisplayAlert("Transferência de depósitos", tplRetorno.Item2, "OK");

            //    if (tplRetorno.Item1 != Models.Datasul.TipoIntegracao.IntegracaoOnlineErro)
            //    {
            //        Limpar(false);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    await DisplayAlert("Erro!", ex.Message, "Cancelar");
            //}
            //finally
            //{
            //    BtnEfetivar.IsEnabled = true;

            //    if (blnProgress)
            //        await pageProgress.OnClose();
            //}
        }

        //private void VerifyProd(string strQr)
        //{
        //    try
        //    {
        //        if (strQr == null)
        //            return;

        //        var mdlEtiqueta = csAuxiliar.GetEtiqueta(strQr);

        //        if (mdlEtiqueta != null)
        //        {
        //            //edtItCodigo.Text = mdlEtiqueta.itCodigo;
        //            //edtLote.Text = mdlEtiqueta.lote;
        //            //edtDtValiLote.Text = mdlEtiqueta.dtValiLote;

        //            //edtDescItem.Text = mdlEtiqueta.descItem;

        //            //edtItCodigo.Focus();
        //            //edtItCodigo.Unfocus();

        //            itemColetor = new ItemColetor
        //            {
        //                itCodigo = mdlEtiqueta.itCodigo,
        //                descItem = mdlEtiqueta.descItem,
        //                itOrigem = mdlEtiqueta.itOrigem,
        //                itNF = mdlEtiqueta.itNF,
        //                itDataRecebimento = mdlEtiqueta.itDataRecebimento,
        //                itRIQ = mdlEtiqueta.itRIQ,
        //                itVol = mdlEtiqueta.itVol,
        //                itExtra = mdlEtiqueta.itExtra
        //            };




        //            /*
        //            edtQuantidade.Focus();*/
        //            _blnClickQr = true;

        //            //OnClick_DepositoSaida(new object(), new EventArgs());
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        DisplayAlert("Erro!", ex.Message, "Cancelar");
        //    }
        //}

        private async void edtItCodigo_Completed(object sender, EventArgs e)
        {
          
        }

        async void Limpar(bool pBlnQuestion)
        {
            if (pBlnQuestion)
            {
                bool blnAlert = await DisplayAlert("Limpar Registros?", "Deseja limpar todos os campos ?", "Sim", "Não");

                if (!blnAlert)
                    return;
            }


            if (pBlnQuestion)
            {
              //  edtDepositoSaida.Text = "";
              //  edtDepositoEntrada.Text = "";
            }

            edtLocalizacaoSaida.Text = "";
            edtLocalizacaoEntrada.Text = "";
            edtItCodigo.Text = "";
        //   edtDescItem.Text = "";
        //   edtUnidade.Text = "";
        //   edtTipoConta.Text = "";
            edtNroDocto.Text = "";
            edtSerie.Text = "";
            edtLote.Text = "";
            //edtSaldo.Text = "";
            edtQuantidade.Text = "";
            edtSaldo.Text = "";
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new ArmazenagemPage());

            return true;
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

        private async void edtItCodigo_TextChanged(object sender, TextChangedEventArgs e)
        {
            var pageProgress = new ProgressBarPopUp("Carregando...");

            try
            {

                
                if (String.IsNullOrEmpty(e.OldTextValue) && edtItCodigo.Text != null && edtItCodigo.Text.Length >= 10)
                {
                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                    //  var iTemLeituraEtiquetaService = new LeituraEtiquetaTransferenciaDepositoService();

                    if (String.IsNullOrEmpty(edtDepositoSaida.Text))
                    {
                        throw new Exception("Informar o depósito de saída");
                    }

                    if (String.IsNullOrEmpty(edtDepositoEntrada.Text))
                    {
                        throw new Exception("Informar o depósito de entrada");
                    }

                    var dadosLeituraItemTransferenciaDeposito = new DadosLeituraItemTransferenciaDeposito()
                    {
                        CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                        CodDeposOrigem = _codDeposEntradaHidden,
                        CodLocalizaOrigem = edtLocalizacaoEntrada.Text ?? String.Empty,
                        CodigoBarras = edtItCodigo.Text
                    };

                    var itemLeituraEtiquetaServiceRetorno = await LeituraEtiquetaTransferenciaDepositoService.SendLeituraEtiquetaAsync(dadosLeituraItemTransferenciaDeposito);

                    if (itemLeituraEtiquetaServiceRetorno != null)
                    {
                        
                        foreach (var item in itemLeituraEtiquetaServiceRetorno.Param.ParamResult)
                        {
                            edtItCodigo.Text = item.CodItem;
                            /* edtDescItem.Text = item.DescItem;
                            edtUnidade.Text = item.Un;
                            edtTipoConta.Text = item.Conta; */
                            edtNroDocto.Text = item.NF;
                            edtSerie.Text = item.Serie;
                            edtLote.Text = item.Lote;
                            try
                            {
                                edtSaldo.Text = int.Parse(item.Saldo).ToString();
                                
                            }
                            catch
                            {
                                edtSaldo.Text = item.Saldo;
                            }

                            if (edtSaldo != null)
                                edtSaldo.Text = edtSaldo.Text.Replace(".0", "").Trim();

                            try
                            {

                                edtQuantidade.Text = int.Parse(item.Quantidade).ToString();
                                
                            }
                            catch
                            {
                                edtQuantidade.Text = item.Quantidade;
                            }

                            if (edtQuantidade.Text != null)
                                edtQuantidade.Text = edtQuantidade.Text.Replace(".0", "").Trim();

                        }
                    }

                    /*
                    await edtCodigoBarrasEtiqueta();

                    await pageProgress.OnClose();

                    if (txtQuantidade.Text.Length > 0 && txtQuantidade.Text != "0")
                    {
                        await ClickButtonEfetivar(true);
                    } */
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Erro leitura etiqueta " + ex.Message, "OK");
                edtItCodigo.Text = String.Empty;
            }
            finally
            {
                await pageProgress.OnClose();
            }
        }

        //void edtItCodigo_Unfocused(object sender, Xamarin.Forms.FocusEventArgs e)
        //{
        //    //string strTipoConEst = String.Empty;

        //    //var item = ItemDB.GetItem(edtItCodigo.Text);

        //    //if (item != null)
        //    //{

        //    //    if (item.TipoConEst == 1)
        //    //    {
        //    //        strTipoConEst = "Serial";
        //    //    }
        //    //    else if (item.TipoConEst == 2)
        //    //    {
        //    //        strTipoConEst = "Número Série";
        //    //    }
        //    //    else if (item.TipoConEst == 3)
        //    //    {
        //    //        strTipoConEst = "Lote";
        //    //    }
        //    //    else if (item.TipoConEst == 4)
        //    //    {
        //    //        strTipoConEst = "Referência";
        //    //    }

        //    //    edtDescItem.Text = item.DescItem;
        //    //    edtUnidade.Text = item.Un;
        //    //    edtTipoConEst.Text = strTipoConEst;
        //    //}
        //    //else
        //    //{
        //    //    edtDescItem.Text = "";
        //    //    edtUnidade.Text = "";
        //    //    edtTipoConEst.Text = "";
        //    //}
        //}


        //void Handle_Completed(object sender, System.EventArgs e)
        //{
        //    //edtItCodigo.Unfocus();
        //}
        //

    }

    //public class DecimalConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        throw new Exception(value.ToString());
    //        if (value is decimal)
    //            return value.ToString();
    //        return value;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    //    {
    //        decimal dec;
    //        if (decimal.TryParse(value as string, out dec))
    //            return dec;
    //        return value;
    //    }
    //}
}