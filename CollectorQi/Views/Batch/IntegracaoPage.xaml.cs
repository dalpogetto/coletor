using CollectorQi.Models;
using CollectorQi.Resources;
using CollectorQi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class IntegracaoPage : ContentPage
    {
        public IntegracaoPage()
        {
            InitializeComponent();
            if (SecurityAuxiliar.Autenticado == false)
            {
                DisplayAlert("Autenticação", "Sinto muito!!! Precisa estar autenticado na página de controle", "OK");
                Application.Current.MainPage = new NavigationPage(new PrincipalPage());
            }
            else
            {
                string[] imagem = new string[]    { "deposito.png"   , };
                string[] titulo = new string[]    { "Movimentações"  , };
                string[] subTitulo = new string[] { "",  };

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
        void OnSelection(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }
            MenuItemDetail menuItemDetail = new MenuItemDetail();
            menuItemDetail = (MenuItemDetail)e.SelectedItem;

            switch (menuItemDetail.Name)
            {
                case "Cadastros":
                    FisicaPage.MenuId = 1;
                    FisicaPage.MenuDesc = menuItemDetail.Name.Trim();
                    Application.Current.MainPage = new NavigationPage(new FisicaPage() { Title = menuItemDetail.Name.Trim() } );
                    break;
                case "Movimentações":

                  //  IntegracaoMovtoPage.MenuId = 2;
                  //  IntegracaoMovtoPage.MenuDesc = menuItemDetail.Name.Trim();
                    Application.Current.MainPage = new NavigationPage(new IntegracaoMovtoPage() { Title = menuItemDetail.Name.Trim() });
                    break;
                case "Abastecimento":
                    break;
                case "Retirada":
                    break;
                case "Voltar":
                    Application.Current.MainPage = new NavigationPage(new PrincipalPage());
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
    }
}