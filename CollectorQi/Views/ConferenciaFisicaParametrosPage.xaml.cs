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
using static CollectorQi.Services.ESCL002.ParametersService;

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
        private static string usuario = "";
        public ParametersService.ParametrosResult parametersService = new ParametersService.ParametrosResult();

        public ConferenciaFisicaParametrosPage()
        {
            InitializeComponent();

            // Nao utilizar request nesse local, o serviço deve ser executado como async
            // Chamada ESCL002 - ObterParametros
            // A busca é somente enviado o usuario para buscar os parametros padrao
            // O retorno sempre será usuario, estabelecimento e data que deve ser populado os campos em questao
            ParametersService ps = new ParametersService();
            var parameters = ps.GetParametersAsync("","");
            usuario = parameters.UsuarioTotvs;
            txtEstabelecimento.Text = parameters.CodEstabel;
            txtDataEntrada.Text = parameters.DtEntrada;

            if(parameters.CodEmitente.ToString() != "0")
                txtCodEmitente.Text = parameters.CodEmitente.ToString();

            //txtCodEmitenteDesc.Text
            if (string.IsNullOrEmpty(parameters.NFRet.ToString()))
                txtNFRet.Text = parameters.NFRet;

            if(string.IsNullOrEmpty(parameters.Serie.ToString()))
                txtSerie.Text = parameters.Serie;

            if (string.IsNullOrEmpty(parameters.QtdeItem.ToString()))
                txtQtdItens.Text = parameters.QtdeItem.ToString();

            if (parameters.ValorTotal.ToString() != "0")
                txtValorTotal.Text = parameters.ValorTotal.ToString();

            if (parameters.DiasXML.ToString() != "0")
                txtQtdDiasXMl.Text = parameters.DiasXML.ToString();

            parametersService = parameters;        

            if (SecurityAuxiliar.Autenticado == false)
            {
                DisplayAlert("Autenticação", "Sinto muito!!! Precisa estar autenticado na página de controle", "OK");
                Application.Current.MainPage = new NavigationPage(new PrincipalPage());
            }
            else
            {                

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

        void OnClick_Sair(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new PrincipalPage());
        }              

        void OnClick_Avancar(object sender, EventArgs e)
        {
            // Chamada ESCL002 - EnviarParametros
            // Enviar no metodo abaixo, os campos que foram digitados na tela após o retorno da API (Busca Parametro)
            // Com o retorno desse método, pode incluir na lista (browse) que mostra todos os reparos disponiveis
            ParametersService parametersService = new ParametersService();

            var parametrosResult = new ParametrosResult();

            parametrosResult.UsuarioTotvs = usuario;
            parametrosResult.CodEstabel = txtEstabelecimento.Text;

            if (!string.IsNullOrEmpty(txtCodEmitente.Text))
                parametrosResult.CodEmitente = int.Parse(txtCodEmitente.Text);

            parametrosResult.DtEntrada = txtDataEntrada.Text;
            parametrosResult.NFRet = txtNFRet.Text;
            parametrosResult.Serie = txtSerie.Text;

            if (!string.IsNullOrEmpty(txtQtdItens.Text))
                parametrosResult.QtdeItem = decimal.Parse(txtQtdItens.Text);

            if (!string.IsNullOrEmpty(txtValorTotal.Text))
                parametrosResult.ValorTotal = decimal.Parse(txtValorTotal.Text);

            if (!string.IsNullOrEmpty(txtQtdDiasXMl.Text))
                parametrosResult.DiasXML = int.Parse(txtQtdDiasXMl.Text);

            parametersService.SendParametersAsync(parametrosResult);

            Application.Current.MainPage = new NavigationPage(new ConferenciaFisicaReparosPage(parametrosResult) { Title = "Conferência Física de Reparos" });
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