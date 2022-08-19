using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GuardaMateriaisTipoMovimento : ContentPage, INotifyPropertyChanged
    {
        public ObservableCollection<InventarioFisicoViewModel> ObsInventario { get; } 
        public string Local { get; set; }
        public string CodDepos { get; set; }

        public GuardaMateriaisTipoMovimento(string local, string codDepos)
        {
            InitializeComponent();

            Local = local;
            CodDepos = codDepos;
            lblDescricao.Text = "Depósito / Localização: " + codDepos + " / " + local;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new GuardaMateriaisDepositoListaPage(null));

            return true;
        }

        protected void BtnEntrada_Clicked(object sender, EventArgs e)
        {  
            Application.Current.MainPage = new NavigationPage(new GuardaMateriaisDepositoItemListaPage(null, Local, CodDepos, "Entrada"));
        }

        protected void BtnSaida_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new GuardaMateriaisDepositoItemListaPage(null, Local, CodDepos, "Saida"));
        }
    }
}