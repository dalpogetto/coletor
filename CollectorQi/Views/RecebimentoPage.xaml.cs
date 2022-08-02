using CollectorQi.Resources;
using CollectorQi.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RecebimentoPage : ContentPage
    {
        private static int inicialPage = 0;
        public static int InicialPage { get => inicialPage; set => inicialPage = value; }
        public RecebimentoPage()
        {
            InitializeComponent();
            if (SecurityAuxiliar.Autenticado == false)
            {
                DisplayAlert("Autenticação", "Sinto muito!!! Precisa estar autenticado na página de controle", "OK");
                Application.Current.MainPage = new NavigationPage(new PrincipalPage());
            }
            else
            {
                string[] imagem = new string[] { "fisica.png", "fisica.png" };
                string[] titulo = new string[] { "ConferenciaFisicaReparos", "AtualizacaoEntrada" };
                string[] subTitulo = new string[] { "Conferência Física de Reparos", "Atualização de NF de Entrada" };  

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
               case "ConferenciaFisicaReparos":
                    ConferenciaPage.MenuId = 1;
                    ConferenciaPage.MenuDesc = "Conferência Física de Reparos";
                    Application.Current.MainPage = new NavigationPage(new ConferenciaFisicaParametrosPage() { Title = "Conferência Física de Reparos" });  
                    break;

                case "AtualizacaoEntrada":
                    ConferenciaPage.MenuId = 2;
                    ConferenciaPage.MenuDesc = "Atualização de NF de Entrada";
                    Application.Current.MainPage = new NavigationPage(new NotaFiscalConferenciaReparosListaPage() { Title = "Nota Fiscal" });
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