using CollectorQi.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : MasterDetailPage
    {
        Dictionary<int, NavigationPage> MenuPages = new Dictionary<int, NavigationPage>();
        public MainPage()
        {
            InitializeComponent();

            MasterBehavior = MasterBehavior.Popover;

            MenuPages.Add((int)MenuItemType.Controle, (NavigationPage)Detail);
        }

        public async Task NavigateFromMenu(int id)
        {
            if (!MenuPages.ContainsKey(id))
            {
                switch (id)
                {
                    case (int)MenuItemType.Cadastros:
                        MenuPages.Add(id, new NavigationPage(new CadastrosPage()));
                        break;
                    case (int)MenuItemType.Recebimento:
                        MenuPages.Add(id, new NavigationPage(new RecebimentoPage()));
                        break;
                    case (int)MenuItemType.Estoque:
                        MenuPages.Add(id, new NavigationPage(new MovimentacaoPage()));
                        break;
                    case (int)MenuItemType.Coletor:
                        MenuPages.Add(id, new NavigationPage(new ItemsPage()));
                        break;
                    case (int)MenuItemType.Controle:
                        MenuPages.Add(id, new NavigationPage(new PrincipalPage()));
                        break;
                    case (int)MenuItemType.QualiIT:
                        MenuPages.Add(id, new NavigationPage(new AboutPage()));
                        break;
                }
            }

            var newPage = MenuPages[id];

            if (newPage != null && Detail != newPage)
            {
                Detail = newPage;

                if (Device.RuntimePlatform == Device.Android)
                    await Task.Delay(100);

                IsPresented = false;
            }
        }
    }
}