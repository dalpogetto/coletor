using CollectorQi.Models;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        MainPage RootPage { get => Application.Current.MainPage as MainPage; }
        List<HomeMenuItem> menuItems;
        public MenuPage()
        {
            InitializeComponent();
            menuItems = new List<HomeMenuItem>
            {
                new HomeMenuItem {Id = MenuItemType.Cadastros, Title="Cadastros" },
                new HomeMenuItem {Id = MenuItemType.Recebimento, Title="Recebimento de mercadorias" },
                new HomeMenuItem {Id = MenuItemType.Estoque, Title="Estoque - Movimentação" },
                new HomeMenuItem {Id = MenuItemType.Coletor, Title="Coletor" },
                new HomeMenuItem {Id = MenuItemType.Controle, Title="Controle - Login" },
                new HomeMenuItem {Id = MenuItemType.QualiIT, Title="QualiIT" }
            };

            ListViewMenu.ItemsSource = menuItems;

            ListViewMenu.SelectedItem = menuItems[0];
            ListViewMenu.ItemSelected += async (sender, e) =>
            {
                if (e.SelectedItem == null)
                    return;

                var id = (int)((HomeMenuItem)e.SelectedItem).Id;
                await RootPage.NavigateFromMenu(id);
            };
        }
    }
}