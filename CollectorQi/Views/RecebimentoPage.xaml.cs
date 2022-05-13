using CollectorQi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using CollectorQi.VO;


namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecebimentoPage : ContentPage
    {
        private static int inicialPage = 0;
        public static int InicialPage { get => inicialPage; set => inicialPage = value; }

        private static int menuId = 0;
        private static string menuDesc = "";
        private static ItemVO item_VO = new ItemVO();

        public static int MenuId { get => menuId; set => menuId = value; }
        public static string MenuDesc { get => menuDesc; set => menuDesc = value; }
        public static ItemVO Item_VO { get => item_VO; set => item_VO = value; }

        public RecebimentoPage()
        {
            InitializeComponent();


            DisplayAlert("Erro", "Abrindo tela RecebimentoPage", "OK");

            if (SecurityAuxiliar.Autenticado == false)
            {
                DisplayAlert("Autenticação", "Sinto muito!!! Precisa estar autenticado na página de controle", "OK");
                Application.Current.MainPage = new NavigationPage(new PrincipalPage());
            }
            else
            {
                switchFlash.Toggled += switcher_ToggledAsync;
            }
        }

        void EntryCodigo_Completed(object sender, EventArgs e)
        {
            if (codigo.Text.Trim() != "")
            {
                VerifyProd(codigo.Text.Trim());
            }
        }
        async void switcher_ToggledAsync(object sender, ToggledEventArgs e)
        {
            if (switchFlash.IsToggled)
            {
                await Flashlight.TurnOnAsync();
            }
            else
            {
                await Flashlight.TurnOnAsync();
            }            
        }

        void Voltar_Clicked(object sender, System.EventArgs e)
        {
            ItemVO i = new ItemVO();
            RecebimentoPage.Item_VO = i;
            GoMenu(menuId);
        }


        async void OpenScanner_Clicked(object sender, System.EventArgs e)
        {
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();

            ZXing.Mobile.MobileBarcodeScanningOptions scanningOptions = new ZXing.Mobile.MobileBarcodeScanningOptions();


            scanningOptions.UseFrontCameraIfAvailable =  switchCameraFrontal.IsToggled;

            scanningOptions.PossibleFormats.Add(ZXing.BarcodeFormat.CODE_128);
            scanningOptions.PossibleFormats.Add(ZXing.BarcodeFormat.CODE_39);
            scanningOptions.PossibleFormats.Add(ZXing.BarcodeFormat.CODABAR);
            scanningOptions.PossibleFormats.Add(ZXing.BarcodeFormat.QR_CODE);

            var result = await scanner.Scan(scanningOptions);


            if (result != null)
            {
                VerifyProd(result.Text.ToString().Trim());
            } 
        }

        void VerifyProd(string byCodigo)
        {
            ItemVO itemVO = new ItemVO();
            itemVO = ItemDB.GetItem(byCodigo);
            Item_VO = itemVO;
            if (itemVO != null)
            {
                codigo.Text = itemVO.ItCodigo;
                descricao.Text = itemVO.DescItem;
                GoMenu(MenuId);
            }
            else
            {
                DisplayAlert("Valor", $"NÃO ENCONTRADO - Codigo: {byCodigo}", "OK");
                codigo.Text = byCodigo;
                descricao.Text = "";
            }
        }

        void GoMenu(int byMenu)
        {
            switch (byMenu)
            {
                case 1:
                case 2:
                    ConferenciaPage.Volta = true;
                    Application.Current.MainPage = new NavigationPage(new ConferenciaPage());
                    break;
                case 11:
                    InventarioItemPage.Volta = true;
                    Application.Current.MainPage = new NavigationPage(new InventarioItemPage());
                    break;
            }
        }
    }
}
 
 