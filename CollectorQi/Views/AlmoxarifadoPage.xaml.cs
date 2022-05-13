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
    public partial class AlmoxarifadoPage : ContentPage
    {
        public AlmoxarifadoPage()
        {
            InitializeComponent();
            if (SecurityAuxiliar.Autenticado == false)
            {
                DisplayAlert("Autenticação", "Sinto muito!!! Precisa estar autenticado na página de controle", "OK");
                Application.Current.MainPage = new NavigationPage(new PrincipalPage());
            }
            else
            {
              
                //string[] imagem = new string[]    {  "deposito.png"          , "retirada.png"             , "voltar.png"               };
                //string[] titulo = new string[]    {  "Depósito"              , "Requisição"               , "Devolver Requisição"      };
                //string[] subTitulo = new string[] { "Transferência Depósito" , "Atendimento de Requisição", "Devolução de Requisição"  };

                string[] imagem = new string[] { "deposito.png" };
                string[] titulo = new string[] { "Coletor"};
                string[] subTitulo = new string[] { "Conferência" };


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
               case "Depósito":
                    ConferenciaPage.MenuId = 2;
                    ConferenciaPage.MenuDesc = "Conferência de depósito";
                    Application.Current.MainPage = new NavigationPage(new ConferenciaPage() { Title = "Conferência de depósito" }); 
            
                    break;

                case "Requisição":
                    //RequisicaoListaPage.MenuId = 2;
                    //RequisicaoListaPage.MenuDesc = "Transferência de depósito";
                    Application.Current.MainPage = new NavigationPage(new RequisicaoListaPage(false) { Title = "Atendimento de Requisição" });

                    break;

                case "Devolver Requisição":
                    //RequisicaoListaPage.MenuId = 2;
                    //RequisicaoListaPage.MenuDesc = "Transferência de depósito";
                    Application.Current.MainPage = new NavigationPage(new RequisicaoListaPage(true) { Title = "Devolver Requisição" });

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