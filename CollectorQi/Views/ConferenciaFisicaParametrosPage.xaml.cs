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
using CollectorQi.Services.ESCL002;

/*using Rg.Plugins.Popup.Services;
*/
using CollectorQi.Services.ESCL002;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConferenciaFisicaParametrosPage : ContentPage
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

        public ConferenciaFisicaParametrosPage()
        {
            InitializeComponent();

            // Nao utilizar request nesse local, o serviço deve ser executado como async
            // Chamada ESCL002 - ObterParametros
            // A busca é somente enviado o usuario para buscar os parametros padrao
            // O retorno sempre será usuario, estabelecimento e data que deve ser populado os campos em questao
            //ParametersService a = new ParametersService();
            //var teste = a.GetParametersAsync("","");
            //System.Diagnostics.Debug.Write(teste);

            // Chamada ESCL002 - EnviarParametros
            // Enviar no metodo abaixo, os campos que foram digitados na tela após o retorno da API (Busca Parametro)
            // Com o retorno desse método, pode incluir na lista (browse) que mostra todos os reparos disponiveis
            ParametersService a = new ParametersService();
            var teste = a.SendParametersAsync("super", "101", 10098, "02/05/2022", "0140390", "0", 3, 390, 15);
            System.Diagnostics.Debug.Write(teste);


            // Chamada ESCL002 - ValidaReparos
            /* 
             * RepairService a = new RepairService();
            var repairs = new List<RepairService.Repair>();

            repairs.Add(new RepairService.Repair()
            {
                CodBarras = "",
                CodEstabel = "101",
                CodFilial = "01",
                NumRR = "2449849",
                Digito = "9"
            });

            a.ValidateRepairAsync("super", "101", repairs);
            */


            // Chamada ESCL002 - ExcluirReparos
            /*
            RepairService a = new RepairService();
            var reparis = new List<RepairService.DelRepair>();

            reparis.Add(new RepairService.DelRepair()
            {
                RowId = "0x00000000315a38098"
            });

            reparis.Add(new RepairService.DelRepair()
            {
                RowId = "0x00000000315a38228"
            });

            a.DelRepairAsync("super", reparis); */

            // Chamada ESCL002 - Finalizar Conferencia

            /*
            ConferenceService a = new ConferenceService();

            ConferenceService.EndConferenceParameters conferenceParam = new ConferenceService.EndConferenceParameters()
            {
                UsuarioTotvs = "super",
                CodEstabel = "101",
                CodEmitente = 10098,
                DtEntrada = "02/05/2020",
                NfRet = "0140390",
                Serie = "0",
                QtdeItem = 3,
                ValorTotal = decimal.Parse("390.12"),
                DiasXml = 15

            };

            var repairs = new List<ConferenceService.Repair>();

            repairs.Add(new ConferenceService.Repair()
            {
                RowId = "0x0000000000c6ffb6"
            });

            repairs.Add(new ConferenceService.Repair()
            {
                RowId = "0x0000000000c70c65"
            });


            repairs.Add(new ConferenceService.Repair()
            {
                RowId = "0x0000000000c71eed"
            });

            a.EndConferenceAsync("super", "prodiebold11", conferenceParam, repairs);
            */



            if (SecurityAuxiliar.Autenticado == false)
            {
                DisplayAlert("Autenticação", "Sinto muito!!! Precisa estar autenticado na página de controle", "OK");
                Application.Current.MainPage = new NavigationPage(new PrincipalPage());
            }
            else
            {
                //footerCodUsuario.Text = SecurityAuxiliar.CodUsuario;

                //tipoConEst.Toggled += tipoConEst_Toggled;
                //edtItCodigo.Focus();

                if (Volta)
                {
                    Volta = false;
                    //Limpar(false);
                    //if (RecebimentoPage.Item_VO != null)
                    //{
                    //    Fill(RecebimentoPage.Item_VO);
                    //}
                }
            }

            //this.Title = "Conferência Física de Reparos";

        }

        void Fill(ItemVO byItemVO)
        {
            //edtItCodigo.Text = byItemVO.ItCodigo;
            //edtDescItem.Text = byItemVO.DescItem;
            //edtUnidade.Text = byItemVO.Un;
        }

        void OnClick_Sair(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new PrincipalPage());
        }

        //void OnClick_Limpar(object sender, EventArgs e)
        //{
        //    Limpar(true);
        //}

        async void OnClick_DepositoSaida(object sender, EventArgs e)
        {
            try
            {
                //BtnDepSaida.IsEnabled = false;

                var pageProgress = new ProgressBarPopUp("Carregando Cadastros...");

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                //var page = new SaldoEstoqPopUp(SecurityAuxiliar.GetCodEstabel(),
                //                               edtItCodigo.Text,
                //                               edtDescItem.Text,
                //                               edtDepSaida,
                //                               edtCodLocaliz,
                //                               edtLote,
                //                               edtDtValiLote,
                //                               edtSaldo,
                //                               edtSaldoMobile,
                //                               _blnClickQr);

                //await pageProgress.OnClose();

                //await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page);

                _blnClickQr = false;

                //BtnDepSaida.IsEnabled = true;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "OK");
            }
            finally
            {
                //BtnDepSaida.IsEnabled = true;
            }
        }



        async void OnClick_BuscaItem(object sender, EventArgs e)
        {
            try
            {
               // BtnBuscaItem.IsEnabled = false;

                //var page = new ItemPopUp(edtItCodigo, edtDescItem, edtUnidade, edtTipoConEst, null);

                //await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page);

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "OK");
            }
            finally
            {
                //BtnBuscaItem.IsEnabled = true;
            }
        }

        async void OnClick_DepositoEntrada(object sender, EventArgs e)
        {
            try
            {
                //BtnDepEntrada.IsEnabled = false;

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
                        //edtDepEntrada.Text = action.ToString();
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
                //BtnDepEntrada.IsEnabled = true;
            }

        }

        void OnClick_Avancar(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new ConferenciaFisicaReparosPage() { Title = "Conferência Física de Reparos" });
            return;
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
                //BtnQR.IsEnabled = false;

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
                //BtnQR.IsEnabled = true;
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
                    //edtItCodigo.Text = mdlEtiqueta.itCodigo;
                    //edtLote.Text = mdlEtiqueta.lote;
                    //edtDtValiLote.Text = mdlEtiqueta.dtValiLote;

                    //edtDescItem.Text = mdlEtiqueta.descItem;

                    //edtItCodigo.Focus();
                    //edtItCodigo.Unfocus();

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

        //async void Limpar(bool pBlnQuestion)
        //{
        //    if (pBlnQuestion)
        //    {
        //        bool blnAlert = await DisplayAlert("Limpar Registros?", "Deseja limpar os campos informados?", "Sim", "Não");

        //        if (!blnAlert)
        //            return;
        //    }

        //    //edtItCodigo.Text = "";
        //    //edtDescItem.Text = "";
        //    edtTipoConEst.Text = "";
        //    edtUnidade.Text = "";
        //    //edtLote.Text = "";
        //    //edtDepEntrada.Text = "";
        //    //edtDepSaida.Text = "";
        //    //edtCodLocaliz.Text = "";
        //    //edtSaldo.Text = "";
        //    //edtQuantidade.Text = "";
        //    //edtSaldoMobile.Text = "";
        //    //edtDtValiLote.Text = "";
        //    //edtNroDocto.Text = "";

        //}

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new RecebimentoPage());

            return true;
        }

        void edtItCodigo_Unfocused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            string strTipoConEst = String.Empty;

            //var item = ItemDB.GetItem(edtItCodigo.Text);

            //if (item != null)
            //{

            //    if (item.TipoConEst == 1)
            //    {
            //        strTipoConEst = "Serial";
            //    }
            //    else if (item.TipoConEst == 2)
            //    {
            //        strTipoConEst = "Número Série";
            //    }
            //    else if (item.TipoConEst == 3)
            //    {
            //        strTipoConEst = "Lote";
            //    }
            //    else if (item.TipoConEst == 4)
            //    {
            //        strTipoConEst = "Referência";
            //    }

            //    edtDescItem.Text = item.DescItem;
            //    edtUnidade.Text = item.Un;
            //    edtTipoConEst.Text = strTipoConEst;
            //}
            //else
            //{
            //    edtDescItem.Text = "";
            //    edtUnidade.Text = "";
            //    edtTipoConEst.Text = "";
            //}
        }


        void Handle_Completed(object sender, System.EventArgs e)
        {
            //edtItCodigo.Unfocus();
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