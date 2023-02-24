using CollectorQi.Models.ESCL021;
using CollectorQi.VO.ESCL018;
using Rg.Plugins.Popup.Services;
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
        public string _codDepos { get; set; }

        

        public GuardaMateriaisTipoMovimento(List<DepositosGuardaMaterialItem> listaDepositosGuardaMaterialItem, string local, string codDepos)
        {
            InitializeComponent();

            ListaDepositosGuardaMaterialItem = listaDepositosGuardaMaterialItem;
            Local = local;
            _codDepos = codDepos;

            lblDescricao.Text = "Depósito / Localização: " + codDepos + " / " + local;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new GuardaMateriaisDepositoListaPage(_codDepos));

            return true;
        }

        protected void BtnEntrada_Clicked(object sender, EventArgs e)
        {
            //Application.Current.MainPage = new NavigationPage(new GuardaMateriaisDepositoItemListaPage(ListaDepositosGuardaMaterialItem, Local, _codDepos, 1));
        }

        protected void BtnSaida_Clicked(object sender, EventArgs e)
        {
            //Application.Current.MainPage = new NavigationPage(new GuardaMateriaisDepositoItemListaPage(ListaDepositosGuardaMaterialItem, Local, _codDepos, 0));
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                ToolBarPrint.IsEnabled = false;

                var pageProgress = new ProgressBarPopUp("Carregando...");
                var page = new ArmazenagemPrintPopUp(null, null);
                await PopupNavigation.Instance.PushAsync(page);
                await pageProgress.OnClose();
            }
            finally
            {
                ToolBarPrint.IsEnabled = true;
            }
        }

        private void ToolBarVoltar_Clicked(object sender, EventArgs e)
        {
            base.OnBackButtonPressed();
            Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new ArmazenagemPage());
        }
    }

    public class InventarioFisicoViewModel : InventarioVO
    {
        public string Image
        {
            get
            {
                if (this.StatusInventario == eStatusInventario.NaoIniciado)
                {
                    return "intPendenteMed.png";
                }
                else if (this.StatusInventario == eStatusInventario.IniciadoMobile)
                {
                    return "intSucessoMed.png";

                }
                else if (this.StatusInventario == eStatusInventario.EncerradoMobile)
                {
                    return "intErroMed.png";
                }

                return "";
            }
        }

        public string StatusInventarioString
        {
            get
            {
                return csAuxiliar.GetDescription(StatusInventario);
            }
        }
    }
}