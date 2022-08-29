using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.PlatformConfiguration;
using CollectorQi.VO;
using CollectorQi.Models;

/*using Rg.Plugins.Popup.Services;
*/

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConferenciaPage : ContentPage
    {
        private static int itemListaColetor = 0;
        private static ItemColetor itemColetor;

        private static int menuId = 0;
        private static string menuDesc = "";
        private static bool volta = false;
        private bool _blnClickQr { get; set; }

        public static int MenuId { get => menuId; set => menuId = value; }
        public static string MenuDesc { get => menuDesc; set => menuDesc = value; }
        public static bool Volta { get => volta; set => volta = value; }

        public ConferenciaPage()
        {
            InitializeComponent();

            if (SecurityAuxiliar.Autenticado == false)
            {
                DisplayAlert("Autenticação", "Sinto muito!!! Precisa estar autenticado na página de controle", "OK");
                Application.Current.MainPage = new NavigationPage(new PrincipalPage());
            }
            else
            {
                //footerCodUsuario.Text = SecurityAuxiliar.CodUsuario;

                //tipoConEst.Toggled += tipoConEst_Toggled;
                edtItCodigo.Focus();

                if (Volta)
                {
                    Volta = false;
                    Limpar(false);
                    //if (RecebimentoPage.Item_VO != null)
                    //{
                    //    Fill(RecebimentoPage.Item_VO);
                    //}
                }
            }

            this.Title = "Conferência de depósito";

        }

        void Fill(ItemVO byItemVO)
        {
            edtItCodigo.Text = byItemVO.ItCodigo;
            edtDescItem.Text = byItemVO.DescItem;
            edtUnidade.Text = byItemVO.Un;
        }

        void OnClick_Sair(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new AlmoxarifadoPage());
        }

        void OnClick_Limpar(object sender, EventArgs e)
        {
            Limpar(true);
        }

        async void OnClick_DepositoSaida(object sender, EventArgs e)
        {
            try
            {
                BtnDepSaida.IsEnabled = false;

                var pageProgress = new ProgressBarPopUp("Carregando Cadastros...");

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                var page = new SaldoEstoqPopUp(SecurityAuxiliar.GetCodEstabel(),
                                               edtItCodigo.Text,
                                               edtDescItem.Text,
                                               edtDepSaida,
                                               edtCodLocaliz,
                                               edtLote,
                                               edtDtValiLote,
                                               edtSaldo,
                                               edtSaldoMobile,
                                               _blnClickQr);

                await pageProgress.OnClose();

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page);

                _blnClickQr = false;

                BtnDepSaida.IsEnabled = true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "OK");
            }
            finally
            {
                BtnDepSaida.IsEnabled = true;
            }
        }



        async void OnClick_BuscaItem(object sender, EventArgs e)
        {
            try
            {
                BtnBuscaItem.IsEnabled = false;

                var page = new ItemPopUp(edtItCodigo, edtDescItem, edtUnidade, edtTipoConEst, null);

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page);

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "OK");
            }
            finally
            {
                BtnBuscaItem.IsEnabled = true;
            }
        }

        async void OnClick_DepositoEntrada(object sender, EventArgs e)
        {
            try
            {
                BtnDepEntrada.IsEnabled = false;

                var deposito = DepositoDB.GetDeposito();
                if (deposito != null)
                {
                    string[] arrayDep = new string[deposito.Count];
                    for (int i = 0; i < deposito.Count; i++)
                    {
                        arrayDep[i] = deposito[i].CodDepos + " (" + deposito[i].Nome.Trim() + ")";
                    }

                    var action = await DisplayActionSheet("Escolha o Depósito?", "Cancelar", null, arrayDep);

                    if (action != "Cancelar" && action != null)
                    {
                        edtDepEntrada.Text = action.ToString();
                    }
                }
                else
                    await DisplayAlert("Erro!", "Nenhum depósito encontrado.", "OK");

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "OK");
            }
            finally
            {
                BtnDepEntrada.IsEnabled = true;
            }

        }

        async void OnClick_Efetivar(object sender, EventArgs e)
        {
            ListaItensColetor.ListaItens.Add(itemColetor);

            Application.Current.MainPage = new NavigationPage(new ConferenciaPage());
            return;

            BtnEfetivar.IsEnabled = false;
            bool blnProgress = false;
            var pageProgress = new ProgressBarPopUp("Efetuando Transferência...");

            try
            {

                if (String.IsNullOrEmpty(edtItCodigo.Text))
                {
                    edtItCodigo.Focus();
                    throw new Exception("Item não encontrado");
                }

                if (String.IsNullOrEmpty(edtDepSaida.Text))
                {
                    edtDepSaida.Focus();
                    throw new Exception("Depósito Saída não encontrado");
                }

                if (String.IsNullOrEmpty(edtDepEntrada.Text))
                {
                    edtDepEntrada.Focus();
                    throw new Exception("Depósito Entrada não encontrado");
                }

                if (String.IsNullOrEmpty(edtQuantidade.Text) ||
                    edtQuantidade.Text == "0")
                {
                    edtQuantidade.Focus();
                    throw new Exception("Quantidade invalida");
                }

                if (edtDepEntrada.Text == edtDepSaida.Text)
                {
                    edtDepEntrada.Focus();
                    throw new Exception("Depósito Entrada deve ser diferente do atual");
                }

                if (decimal.Parse(edtSaldoMobile.Text) - decimal.Parse(edtQuantidade.Text) < 0)
                {
                    edtQuantidade.Focus();
                    throw new Exception("Movimentação não atualizada, saldo do depósito " + edtDepSaida.Text + " não pode ser negativo. ");
                }

                bool blnAlert = await DisplayAlert("Transferência de depósito?", "Confirma a transferência de depósito?", "Sim", "Não");

                blnProgress = true;

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                if (!blnAlert)
                    return;

                string strCodEstabel = SecurityAuxiliar.GetCodEstabel();
                string strCodDepSaida = edtDepSaida.Text;
                string strCodDepEntrada = edtDepEntrada.Text;

                if (strCodDepSaida.IndexOf('(') > 0)
                    strCodDepSaida = strCodDepSaida.Remove(strCodDepSaida.IndexOf('('));

                if (strCodDepEntrada.IndexOf('(') > 0)
                    strCodDepEntrada = strCodDepEntrada.Remove(strCodDepEntrada.IndexOf('('));

                List<ModelDepositoTransfere> lstDepositoTransfere = new List<ModelDepositoTransfere>();


                ModelDepositoTransfere mDepositoTransfer = new ModelDepositoTransfere
                {
                    codEstabel = strCodEstabel.Trim(),
                    itCodigo = edtItCodigo.Text.Trim(),
                    nroDocto = edtNroDocto.Text == null ? String.Empty : edtNroDocto.Text.Trim(),
                    codDeposSaida = strCodDepSaida.Trim(),
                    codLote = edtLote.Text.Trim(),
                    codLocaliz = edtCodLocaliz.Text.Trim(),
                    /*  dtValiLote =  DateTime(int.Parse(edtDtValiLote.Text.Substring(6)),
                                                                                            int.Parse(edtDtValiLote.Text.Substring(3, 2)),
                                                                                            int.Parse(edtDtValiLote.Text.Substring(0, 2))) */
                    codDeposEntrada = strCodDepEntrada.Trim(),
                    qtidadeTransf = Decimal.Parse(edtQuantidade.Text.Trim()),
                    codUsuario = SecurityAuxiliar.CodUsuario.Trim()

                };

                if (!String.IsNullOrEmpty(edtDtValiLote.Text))
                    mDepositoTransfer.dtValiLote = new DateTime(int.Parse(edtDtValiLote.Text.Substring(6)),
                                                                int.Parse(edtDtValiLote.Text.Substring(3, 2)),
                                                                int.Parse(edtDtValiLote.Text.Substring(0, 2)));

                lstDepositoTransfere.Add(mDepositoTransfer);

                var tplRetorno = Models.Datasul.IntegracaoOnlineBatch.DepositoTransfere(lstDepositoTransfere);

                await DisplayAlert("Transferência de depósitos", tplRetorno.Item2, "OK");

                if (tplRetorno.Item1 != Models.Datasul.TipoIntegracao.IntegracaoOnlineErro)
                {
                    Limpar(false);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                BtnEfetivar.IsEnabled = true;

                if (blnProgress)
                    await pageProgress.OnClose();
            }
        }

        async void OnClick_QR(object sender, EventArgs e)
        {
            /*
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();

            ZXing.Mobile.MobileBarcodeScanningOptions scanningOptions = new ZXing.Mobile.MobileBarcodeScanningOptions();

            /*scanningOptions.UseFrontCameraIfAvailable = switchCameraFrontal.IsToggled;

            scanningOptions.PossibleFormats.Add(ZXing.BarcodeFormat.CODE_128);
            scanningOptions.PossibleFormats.Add(ZXing.BarcodeFormat.CODE_39);
            scanningOptions.PossibleFormats.Add(ZXing.BarcodeFormat.CODABAR);
            scanningOptions.PossibleFormats.Add(ZXing.BarcodeFormat.QR_CODE);

            var result = await scanner.Scan(scanningOptions);

            if (result != null)
            {
                VerifyProd(result.Text.ToString().Trim()); 
            }*/



            try
            {
                BtnQR.IsEnabled = false;

                var customScanPage = new ZXingScannerPage();

                customScanPage.SetResultAction(VerifyProd);

                await Navigation.PushModalAsync(customScanPage);

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "OK");
            }
            finally
            {
                BtnQR.IsEnabled = true;
            }
        }

        private void VerifyProd(string strQr)
        {
            try
            {
                if (strQr == null)
                    return;

                var mdlEtiqueta = csAuxiliar.GetEtiqueta(strQr);

                if (mdlEtiqueta != null)
                {
                    edtItCodigo.Text = mdlEtiqueta.itCodigo;
                    //edtLote.Text = mdlEtiqueta.lote;
                    //edtDtValiLote.Text = mdlEtiqueta.dtValiLote;

                    edtDescItem.Text = mdlEtiqueta.descItem;

                    edtItCodigo.Focus();
                    edtItCodigo.Unfocus();

                    itemColetor = new ItemColetor
                    {
                        itCodigo = mdlEtiqueta.itCodigo,
                        descItem = mdlEtiqueta.descItem,
                        itOrigem = mdlEtiqueta.itOrigem,
                        itNF = mdlEtiqueta.itNF,
                        itDataRecebimento = mdlEtiqueta.itDataRecebimento,
                        itRIQ = mdlEtiqueta.itRIQ,
                        itVol = mdlEtiqueta.itVol,
                        itExtra = mdlEtiqueta.itExtra
                    };

                    


                    /*
                    edtQuantidade.Focus();*/
                    _blnClickQr = true;

                    //OnClick_DepositoSaida(new object(), new EventArgs());
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
        }

        async void Limpar(bool pBlnQuestion)
        {
            if (pBlnQuestion)
            {
                bool blnAlert = await DisplayAlert("Limpar Registros?", "Deseja limpar os campos informados?", "Sim", "Não");

                if (!blnAlert)
                    return;
            }

            edtItCodigo.Text = "";
            edtDescItem.Text = "";
            edtTipoConEst.Text = "";
            edtUnidade.Text = "";
            edtLote.Text = "";
            edtDepEntrada.Text = "";
            edtDepSaida.Text = "";
            edtCodLocaliz.Text = "";
            edtSaldo.Text = "";
            edtQuantidade.Text = "";
            edtSaldoMobile.Text = "";
            edtDtValiLote.Text = "";
            edtNroDocto.Text = "";

        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new AlmoxarifadoPage());

            return true;
        }

        void edtItCodigo_Unfocused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            string strTipoConEst = String.Empty;

            var item = ItemDB.GetItem(edtItCodigo.Text);

            if (item != null)
            {

                if (item.TipoConEst == 1)
                {
                    strTipoConEst = "Serial";
                }
                else if (item.TipoConEst == 2)
                {
                    strTipoConEst = "Número Série";
                }
                else if (item.TipoConEst == 3)
                {
                    strTipoConEst = "Lote";
                }
                else if (item.TipoConEst == 4)
                {
                    strTipoConEst = "Referência";
                }

                edtDescItem.Text = item.DescItem;
                edtUnidade.Text = item.Un;
                edtTipoConEst.Text = strTipoConEst;
            }
            else
            {
                edtDescItem.Text = "";
                edtUnidade.Text = "";
                edtTipoConEst.Text = "";
            }
        }


        void Handle_Completed(object sender, System.EventArgs e)
        {
            edtItCodigo.Unfocus();
        }
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