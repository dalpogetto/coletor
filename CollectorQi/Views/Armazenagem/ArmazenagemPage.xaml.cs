using CollectorQi.Models.ESCL017;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL017;
using CollectorQi.Services.ESCL021;
using CollectorQi.ViewModels;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ArmazenagemPage : ContentPage
    {
        private static int inicialPage = 0;
        public static int InicialPage { get => inicialPage; set => inicialPage = value; }

        public ArmazenagemPage()
        {
            InitializeComponent();
            if (SecurityAuxiliar.Autenticado == false)
            {
                DisplayAlert("Autenticação", "Sinto muito!!! Precisa estar autenticado na página de controle", "OK");
                Application.Current.MainPage = new NavigationPage(new PrincipalPage());
            }
            else
            {
                string[] imagem = new string[]    { "guardaMaterias.png"           , "transferenciaDeposito.png"          , "movto_repair3.png"                 , "consulta_localizacao.png"           , "saldo_virtual.png"                   , "expedicao.png"                  /* , "print.png"*/  };
                string[] titulo = new string[]    { "Guarda de Materiais"          , "Transferência de Depósito"          , "Movimentação de Reparo"            , "Consulta de Localização"            , "Manutenção Saldo Virtual"            , "Gerar Pedido"                   /* , "Impressão de Etiquetas de Identificação" */ };
                string[] subTitulo = new string[] { "Guarda de Materiais (ESCL027)", "Transferência de Depósito (ESCL021)", "Movimentação de Reparo (ESCL029)"  , "Consulta de Localização (ESCL025)"  , "Manutenção Saldo Virtual (ESCL026)"  , "Expedição de Pedidos (ESCL034)" /* , "Impressão (ESCL010/ESCL013/ESCL020)" ,*/  };  

                List<MenuItemDetail> menuItemDetails = new List<MenuItemDetail>();
                MenuItemDetail menuItemDetail;
                for (int i = 0; i < imagem.Count(); i++)
                {
                    menuItemDetail = new MenuItemDetail { MenuItemDatailId = i + 1, Name = titulo[i], SubTitle = subTitulo[i], Image = imagem[i] };

                    menuItemDetails.Add(menuItemDetail);
                }

                listView.ItemsSource = menuItemDetails;
                listView.ItemSelected += OnSelection;
            }
        }
        async void OnSelection(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }
            MenuItemDetail menuItemDetail = new MenuItemDetail();
            menuItemDetail = (MenuItemDetail)e.SelectedItem;

            switch (menuItemDetail.Name)
            {
               case "Guarda de Materiais":
                    ConferenciaPage.MenuId = 1;
                    ConferenciaPage.MenuDesc = "Guarda de Materiais";

                    Application.Current.MainPage = new NavigationPage(new GuardaMateriaisDepositoListaPage(null));

                    break;

                case "Transferência de Depósito":
                    ConferenciaPage.MenuId = 2;
                    ConferenciaPage.MenuDesc = "Transferência de Depósito";
                    Application.Current.MainPage = new NavigationPage(new TransferenciaDepositoPage(null) { Title = "Transferência de Depósito" });
                    break;


                case "Movimentação de Reparo":
                    ConferenciaPage.MenuId = 2;
                    ConferenciaPage.MenuDesc = "Movimentação de Reparo";
                    Application.Current.MainPage = new NavigationPage(new ArmazenagemMovimentoReparoPage(null) { Title = "Movimentação de Reparo" });
                    break;


                case "Consulta de Localização":
                    ConferenciaPage.MenuId = 2;
                    ConferenciaPage.MenuDesc = "Consulta de Localização";
                    Application.Current.MainPage = new NavigationPage(new ConsultaLocalizacaoDepositoListaPage(null) { Title = "Consulta de Localização" });
                    break;

                case "Manutenção Saldo Virtual":
                    ConferenciaPage.MenuId = 2;
                    ConferenciaPage.MenuDesc = "Manutenção Saldo Virtual";
                    Application.Current.MainPage = new NavigationPage(new SaldoVirtualDepositoListaPage(null) { Title = "Manutenção Saldo Virtual" });
                    break;

                //case "MovimentoReparos":
                //    ConferenciaPage.MenuId = 3;
                //    ConferenciaPage.MenuDesc = "Movimento de Reparos";
                //    //Application.Current.MainPage = new NavigationPage(new ConferenciaPage() { Title = "Movimento de Reparos" });
                //    break;

                case "Gerar Pedido":
                    ConferenciaPage.MenuId = 2;
                    ConferenciaPage.MenuDesc = "Gerar Pedido";
                    Application.Current.MainPage = new NavigationPage(new GerarPedidoEnviarParametroPage(null) { Title = "Gerar Pedido" });
                    break;

            }

            ((ListView)sender).SelectedItem = null;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new PrincipalPage());

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
    }
}