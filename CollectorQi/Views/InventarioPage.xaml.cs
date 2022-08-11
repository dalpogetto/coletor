﻿using CollectorQi.Resources;
using CollectorQi.ViewModels;
using System.Collections.Generic;
using System.Linq;
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
                string[] imagem = new string[] { "inventario.png", "inventario.png" };
                string[] titulo = new string[] { "Inventário Físico", "Inventário de Reparos" };
                //string[] subTitulo = new string[] { "Processo já desenvolvido anteriormente", "Processo proposto nesse documento" };

                List<MenuItemDetail> menuItemDetails = new List<MenuItemDetail>();
                MenuItemDetail menuItemDetail;

                for (int i = 0; i < titulo.Count(); i++)
                {
                    //menuItemDetail = new MenuItemDetail { MenuItemDatailId = i + 1, Name = titulo[i], SubTitle = subTitulo[i] , Image = imagem[i] };
                    menuItemDetail = new MenuItemDetail { MenuItemDatailId = i + 1, Name = titulo[i], Image = imagem[i] };

                    menuItemDetails.Add(menuItemDetail);
                }

                listView.ItemsSource = menuItemDetails;
                listView.ItemSelected += OnSelection; 
            }
        }
       
        void Voltar_Clicked(object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new PrincipalPage());
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
                case "Inventário Físico":
                    Application.Current.MainPage = new NavigationPage(new InventarioFisicoListaPage());
                    break;

                case "Inventário de Reparos":
                    Application.Current.MainPage = new NavigationPage(new InventarioReparoListaPage(null));
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