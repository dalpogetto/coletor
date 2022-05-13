using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using CollectorQi.Models;
using CollectorQi.ViewModels;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemDetailPage : ContentPage
    {
        ItemDetailViewModel viewModel;

        public ItemDetailPage(ItemDetailViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = this.viewModel = viewModel;
        }

        public ItemDetailPage()
        {
            InitializeComponent();

            var item = new Item
            {
                Text = "Código do item",
                Description = "Descrição"
            };

            viewModel = new ItemDetailViewModel(item);
            BindingContext = viewModel;
        }
    }
}