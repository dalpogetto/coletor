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
using static CollectorQi.Services.ESCL002.ParametersService;
using System.Collections.ObjectModel;
using CollectorQi.Services.ESCL002;

/*using Rg.Plugins.Popup.Services;
*/

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConferenciaFisicaConfirmarPage : ContentPage
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
        public ObservableCollection<ResultRepair> Confirmar { get; set; }
        public ParametrosResult parametrosResult = new ParametrosResult();

        public ConferenciaFisicaConfirmarPage(ParametrosResult _parametrosResult, List<ResultRepair> listaReparos)
        {
            InitializeComponent();
            Confirmar = new ObservableCollection<ResultRepair>();
            parametrosResult = _parametrosResult;

            foreach (ResultRepair item in listaReparos)
            {
                ResultRepair resultRepair = new ResultRepair();
                resultRepair.RowId = item.RowId;
                resultRepair.CodItem = item.CodItem + " / " + item.NumRR;
                //resultRepair.NumRR = item.NumRR;                
                resultRepair.Mensagem = item.Mensagem;

                Confirmar.Add(resultRepair);                
            }

            ColConfirmar.BindingContext = this;
            ColConfirmar.ItemsLayout = new ListItemsLayout(ItemsLayoutOrientation.Vertical);

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
                    Limpar(false);
                }
            }
        }
     

        void OnClick_Voltar(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new ConferenciaFisicaReparosPage() { Title = "Conferência Física de Reparos" });
            return;
        }

        void OnClick_Sair(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new AlmoxarifadoPage());
        }

        void OnClick_Limpar(object sender, EventArgs e)
        {
            Limpar(true);
        }

        void OnClick_Excluir(object sender, EventArgs e)
        {
            ResultRepair stringInThisCell = (ResultRepair)((Button)sender).BindingContext;
            Confirmar.Remove(stringInThisCell);

            OnPropertyChanged("Confirmar");

            return;
        }
               
        void OnClick_Salvar(object sender, EventArgs e)
        {
            ParametersService ps = new ParametersService();

            List<ResultRepair> listaReparos = new List<ResultRepair>(); 

            foreach (var item in Confirmar)
            {
                ResultRepair reparos = new ResultRepair();
                reparos.RowId = item.RowId;

                listaReparos.Add(reparos);
            }

            ps.SendParametersListaReparosAsync(parametrosResult, listaReparos);

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

        async void Limpar(bool pBlnQuestion)
        {
            if (pBlnQuestion)
            {
                bool blnAlert = await DisplayAlert("Limpar Registros?", "Deseja limpar os campos informados?", "Sim", "Não");

                if (!blnAlert)
                    return;
            }

            //edtItCodigo.Text = "";
            //edtDescItem.Text = "";
            //edtTipoConEst.Text = "";
            //edtUnidade.Text = "";
            //edtLote.Text = "";
            //edtDepEntrada.Text = "";
            //edtDepSaida.Text = "";
            //edtCodLocaliz.Text = "";
            //edtSaldo.Text = "";
            //edtQuantidade.Text = "";
            //edtSaldoMobile.Text = "";
            //edtDtValiLote.Text = "";
            //edtNroDocto.Text = "";
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new ConferenciaFisicaReparosPage() { Title = "Conferência Física de Reparos" });

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
}