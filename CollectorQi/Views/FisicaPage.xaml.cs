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

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FisicaPage : ContentPage
    {
        private static int menuId = 0;
        private static string menuDesc = "";
        private static bool volta = false;

        public static int MenuId { get => menuId; set => menuId = value; }
        public static string MenuDesc { get => menuDesc; set => menuDesc = value; }
        public static bool Volta { get => volta; set => volta = value; }

        public FisicaPage()
        {
            InitializeComponent();
            if (SecurityAuxiliar.Autenticado == false)
            {
                DisplayAlert("Autenticação", "Sinto muito!!! Precisa estar autenticado na página de controle", "OK");
                Application.Current.MainPage = new NavigationPage(new PrincipalPage());
            }
            else
            {
                footerCodUsuario.Text = SecurityAuxiliar.CodUsuario;

                tipoConEst.Toggled += tipoConEst_Toggled;
                edtItCodigo.Focus();

                if (Volta)
                {
                    Volta = false;
                    Limpar();
                    if (RecebimentoPage.Item_VO != null)
                    {
                        Fill(RecebimentoPage.Item_VO);
                    }
                }
            }
        }

        void Fill(ItemVO byItemVO)
        {

            edtItCodigo.Text = byItemVO.ItCodigo;
            edtDescItem.Text = byItemVO.DescItem;
            Unidade.Text = byItemVO.Un;
        }
        void tipoConEst_Verifica()
        {
            double opacity = 0.3;

            tipoConEstDesc.Text = (tipoConEst.IsToggled ? "Lote" : "Serial");

            switch (tipoConEst.IsToggled)
            {
                case true: // Lote
                    LoteE.Opacity = opacity;
                    /*LoteS.Opacity = opacity;*/
                    /* Sequencia.Opacity = opacity; */
                    Quantidade.Opacity = opacity;
                    DepositoEntrada.Opacity = 1.0;

                    LoteE.IsEnabled = false;
              /*      LoteS.IsEnabled = false;*/
                 /*   Sequencia.IsEnabled = false; */
                    Quantidade.IsEnabled = false;
                    DepositoEntrada.IsEnabled = true;

                    break;
                case false:
                    LoteE.Opacity = 1;
             /*       LoteS.Opacity = 1;*/
              /*      Sequencia.Opacity = 1; */
                    Quantidade.Opacity = 1;
                    DepositoEntrada.Opacity = opacity;

                    LoteE.IsEnabled = true;
               /*     LoteS.IsEnabled = true; */
              /*      Sequencia.IsEnabled = true; */
                    Quantidade.IsEnabled = true;
                    DepositoEntrada.IsEnabled = false;

                    break;
            }
        }
        void tipoConEst_Toggled(object sender, ToggledEventArgs e)
        {
            tipoConEst_Verifica();
        }

        void OnClick_Sair(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new AlmoxarifadoPage());
        }

        void OnClick_Limpar(object sender, EventArgs e)
        {
            Limpar();
        }
        void OnClick_QR(object sender, EventArgs e)
        {
            RecebimentoPage.MenuId = MenuId;
            RecebimentoPage.MenuDesc = MenuDesc;
            Application.Current.MainPage = new NavigationPage(new RecebimentoPage() { Title = "Leitor: " + MenuDesc } );
        }

        void Limpar()
        {
            edtItCodigo.Text = "";
            edtDescItem.Text = "";
            tipoConEst.IsToggled = false;
            tipoConEst_Verifica();
            Unidade.Text = "";
         /*   LoteS.Text = ""; */
            /*Sequencia.Text = ""; */
            DepositoEntrada.Text = "";
           /* DepositoSaida.Text = ""; */
            LoteE.Text = "";
          /*  LoteS.Text = ""; */
            Saldo.Text = "";
            Quantidade.Text = "";
        }
    }
}