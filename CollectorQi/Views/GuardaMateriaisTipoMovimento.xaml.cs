using CollectorQi.Models.ESCL021;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GuardaMateriaisTipoMovimento : ContentPage, INotifyPropertyChanged
    {
        public List<DepositosGuardaMaterialItem> ListaDepositosGuardaMaterialItem { get; set; }
        public ObservableCollection<InventarioFisicoViewModel> ObsInventario { get; } 
        public string Local { get; set; }
        public string CodDepos { get; set; }
        public string CodigoBarras { get; set; }

        public GuardaMateriaisTipoMovimento(List<DepositosGuardaMaterialItem> listaDepositosGuardaMaterialItem, string local, string codDepos, string codigoBarras)
        {
            InitializeComponent();

            ListaDepositosGuardaMaterialItem = listaDepositosGuardaMaterialItem;
            Local = local;
            CodDepos = codDepos;
            CodigoBarras = codigoBarras;

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
            Application.Current.MainPage = new NavigationPage(new GuardaMateriaisDepositoItemListaPage(ListaDepositosGuardaMaterialItem, Local, CodDepos, CodigoBarras, 1));
        }

        protected void BtnSaida_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new GuardaMateriaisDepositoItemListaPage(ListaDepositosGuardaMaterialItem, Local, CodDepos, CodigoBarras, 0));
        }
    }
}