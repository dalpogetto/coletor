using CollectorQi.Models;
using CollectorQi.ViewModels;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Resources;
using CollectorQi.VO;
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
    public partial class InventarioPage : ContentPage
    {
        public InventarioPage()
        {
            InitializeComponent();

            if (SecurityAuxiliar.Autenticado == false)
            {
                DisplayAlert("Autenticação", "Sinto muito!!! Precisa estar autenticado na página de controle", "OK");
                Application.Current.MainPage = new NavigationPage(new PrincipalPage());
            }
            else
            {
             
                string[] imagem = new string[] { /*"almoxarifado.png", */ "inventario.png",  };
                string[] titulo = new string[] { /*"Novo Inventário" , */ "Lista de inventário" };
                string[] subTitulo = new string[] { /* "Digitação de inventário" , */ "Inventários em andamento" };

                List<MenuItemDetail> menuItemDetails = new List<MenuItemDetail>();
                MenuItemDetail menuItemDetail;

                for (int i = 0; i < titulo.Count(); i++)
                {
                    menuItemDetail = new MenuItemDetail { MenuItemDatailId = i + 1, Name = titulo[i], SubTitle = subTitulo[i] , Image = imagem[i] };

                    menuItemDetails.Add(menuItemDetail);
                }

                listView.ItemsSource = menuItemDetails;
                listView.ItemSelected += OnSelection; 
            }
        }

        void Limpar()
        {
            /*
            DateTime dt = DateTime.Now;
            selDataPicker.MinimumDate = dt.AddDays(-7);
            selDataPicker.MaximumDate = dt.AddDays(7);
            selDataPicker.Date = dt;
            edtCodDepos.Text = "";
            edtContagem.Text = ""; */
        }
        void Voltar_Clicked(object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new PrincipalPage());
        }

        void Limpar_Clicked(object sender, System.EventArgs e)
        {
            Limpar();
        }

        async void OnSelection(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return;
            }

            MenuItemDetail menuItemDetail = new MenuItemDetail();
            menuItemDetail = (MenuItemDetail)e.SelectedItem;

            ((ListView)sender).SelectedItem = null;

            switch (menuItemDetail.Name)
            {
                case "Novo Inventário":

                    var page = new InventarioNovoPopUp();

                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page);

                    break;
                case "Lista de inventário":
                    Application.Current.MainPage = new NavigationPage(new InventarioListaPage());
                    break;                
            }
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new PrincipalPage());

            return true;
        }     
    }
}