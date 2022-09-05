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
using static CollectorQi.Services.ESCL002.ParametersService;
using static CollectorQi.Services.ESCL002.RepairService;
using System.ComponentModel;
using System.Collections.ObjectModel;

/*using Rg.Plugins.Popup.Services;
*/

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConferenciaFisicaReparosPage : ContentPage, INotifyPropertyChanged
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

        public ObservableCollection<ResultRepair> ListaReparos
        {
            get { return _listaReparos; }
            set
            {
                _listaReparos = value;
                OnPropertyChanged("ListaReparos");
            }

        }
        public ObservableCollection<ResultRepair> _listaReparos = new ObservableCollection<ResultRepair>();
        public ParametrosResult parametrosResult = new ParametrosResult();

        public ConferenciaFisicaReparosPage()
        {
        }

        public ConferenciaFisicaReparosPage(ParametrosResult _parametrosResult)
        {
            InitializeComponent();

            parametrosResult = _parametrosResult;

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
                    Limpar(false);
                    //if (RecebimentoPage.Item_VO != null)
                    //{
                    //    Fill(RecebimentoPage.Item_VO);
                    //}
                }
            }


            cvListaReparos.BindingContext = this;
            //this.Title = "Conferência de depósito";

        }

        void OnClick_Sair(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new PrincipalPage());
        }               
       
        void OnClick_Scan(object sender, EventArgs e)
        {   
            var repair = new ResultRepair()
            {
                CodBarras = txtScan.Text,
                CodEstabel = txtEst.Text,
                CodFilial = txtFil.Text,
                NumRR = txtRR.Text
            };

            ListaReparos.Add(repair);

            OnPropertyChanged("ListaReparos");

            txtScan.Text = string.Empty;
            txtEst.Text = string.Empty;
            txtFil.Text = string.Empty;
            txtRR.Text = string.Empty;
        }

        async void OnClick_Avancar(object sender, EventArgs e)
        {
            List<ResultRepair> resultRepairList = new List<ResultRepair>();
            ParametersService ps = new ParametersService();

            // Chamar validar reparo
            RepairService repair = new RepairService();

            //List<Repair> repairs

            List<Repair> rep = new List<Repair>();

            foreach (var row in this.ListaReparos){

                rep.Add(new Repair
                {
                    CodBarras = row.CodBarras,
                    CodEstabel = row.CodEstabel,
                    NumRR = row.NumRR,
                    CodFilial = row.CodFilial
                });
            }
            
            /*
            rep.Add(new Repair
            {
                CodBarras = "1010749593280"
            });

            rep.Add(new Repair
            {
                CodBarras = "91010799620000"
            });*/

            // 
            var retornoRep = await repair.ValidateRepairAsync("super", "super", rep);

            // Encaminho Lista Reparos
            // ps.Va

            foreach (var item in retornoRep)
            {
                ResultRepair resultRepair = new ResultRepair();
                resultRepair.CodItem = item.CodItem;
                resultRepair.NumRR = item.NumRR;
                resultRepair.Mensagem = item.Mensagem;
                resultRepair.RowId = item.RowId;
                resultRepair.DescItem = item.DescItem;

                resultRepairList.Add(resultRepair); 

            }

            /*
            foreach (var item in ps.GetListRepair(parametrosResult, ListaReparos))
            {
                ResultRepair resultRepair = new ResultRepair();
                resultRepair.CodItem = item.CodItem;
                resultRepair.NumRR = item.NumRR;
                resultRepair.Mensagem = item.Mensagem;
                resultRepair.RowId = item.RowId;

                resultRepairList.Add(resultRepair);
            }*/

            Application.Current.MainPage = new NavigationPage(new ConferenciaFisicaConfirmarPage(parametrosResult, resultRepairList) { Title = "Conferência Física de Reparos" });
            return;
        }

        void OnClick_Voltar(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new ConferenciaFisicaParametrosPage() { Title = "Conferência Física de Reparos" });
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

        async void OnClick_BuscaItem(object sender, System.EventArgs e)
        {
            BtnBuscaItem.IsEnabled = false;
            try
            {
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
                BtnBuscaItem.IsEnabled = true;
            }
        }


        private void VerifyProd(string strQr)
        {
            try
            {
                
                if (strQr == null)
                    return;


                txtScan.Text = strQr;

                OnClick_Scan(new object(), new EventArgs());


                /*
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
                    edtQuantidade.Focus();
                    _blnClickQr = true;

                    //OnClick_DepositoSaida(new object(), new EventArgs());
                
                }*/
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
            Application.Current.MainPage = new NavigationPage(new ConferenciaFisicaParametrosPage() { Title = "Conferência Física de Reparos" });

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

    public class DecimalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception(value.ToString());
            if (value is decimal)
                return value.ToString();
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            decimal dec;
            if (decimal.TryParse(value as string, out dec))
                return dec;
            return value;
        }
    }

}