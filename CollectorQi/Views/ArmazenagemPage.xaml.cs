using CollectorQi.Models.ESCL017;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL017;
using CollectorQi.Services.ESCL021;
using CollectorQi.ViewModels;
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
                string[] imagem = new string[] { "security.png", "security.png"};
                string[] titulo = new string[] { "GuardaMateriais", "TransferenciaDeposito" };
                string[] subTitulo = new string[] { "Guarda de Materiais", "Transferência de Depósito" };  

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
               case "GuardaMateriais":
                    ConferenciaPage.MenuId = 1;
                    ConferenciaPage.MenuDesc = "Guarda de Materiais";

                    var dDeposito = new DepositosGuardaMaterialService();
                    var dDepositoRetorno = await dDeposito.SendGuardaMaterialAsync();
                    Application.Current.MainPage = new NavigationPage(new GuardaMateriaisDepositoListaPage(dDepositoRetorno.Param.ParamResult));

                    break;

                case "TransferenciaDeposito":
                    ConferenciaPage.MenuId = 2;
                    ConferenciaPage.MenuDesc = "Transferência de Depósito";
                    Application.Current.MainPage = new NavigationPage(new TransferenciaDepositoListaPage(null) { Title = "Transferência de Depósito" });
                    break;


                //case "MovimentoReparos":
                //    ConferenciaPage.MenuId = 3;
                //    ConferenciaPage.MenuDesc = "Movimento de Reparos";
                //    //Application.Current.MainPage = new NavigationPage(new ConferenciaPage() { Title = "Movimento de Reparos" });
                //    break;
            }

            ((ListView)sender).SelectedItem = null;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new PrincipalPage());

            return true;
        }
    }
}