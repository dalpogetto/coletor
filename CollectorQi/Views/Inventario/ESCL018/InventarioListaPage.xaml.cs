using CollectorQi.Models;
using CollectorQi.ViewModels;
using CollectorQi.Resources.DataBaseHelper.Batch;
using CollectorQi.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CollectorQi.Models.Datasul;
using AutoMapper;
using System.Globalization;
using Plugin.Connectivity;
using CollectorQi.Services.ESCL018;
using ESCL = CollectorQi.Models.ESCL018;
using Rg.Plugins.Popup.Services;
using CollectorQi.Resources.DataBaseHelper.ESCL018;
using CollectorQi.VO.ESCL018;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventarioListaPage : ContentPage, INotifyPropertyChanged
    {
        #region Property

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

        public ObservableCollection<InventarioViewModel> ObsInventario { get; set; }

        public InventarioListaPage()
        {
            InitializeComponent();

            lblCodEstabel.Text = SecurityAuxiliar.Estabelecimento;

            cvInventario.BindingContext = this;
        }

        public async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cvInventario.SelectedItem == null)
                return;

            var current = (cvInventario.SelectedItem as InventarioVO);

            if (current != null)
            {
                Application.Current.MainPage = new NavigationPage(new InventarioListaLocalizacaoPage(current));
            }

            cvInventario.SelectedItem = null;
        }

        private async void CarregaListView()
        {
            // var parametersInventario = new ParametersInventarioService();
            //var lstInventario = await ParametersInventarioService.SendParametersAsync();
            var pageProgress = new ProgressBarPopUp("Carregando Inventário, aguarde...");

            try
            {
                ObsInventario.Clear();

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                var lstInventario = await ParametersInventarioService.SendParametersAsync(this);

                foreach (var row in lstInventario)
                {
                    var modelView = Mapper.Map<InventarioVO, InventarioViewModel>(row);

                    ObsInventario.Add(modelView);
                }

                OnPropertyChanged("ObsInventario");

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                await pageProgress.OnClose();
            }
        }

        async void OnClick_CarregaInventario(object sender, System.EventArgs e)
        {
            CarregaListView();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            ObsInventario = new ObservableCollection<InventarioViewModel>();

            CarregaListView();
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new InventarioPage());

            return true;
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                ToolBarPrint.IsEnabled = false;

                var pageProgress = new ProgressBarPopUp("Carregando...");
                var page = new InventarioPrintPopUp(null,null , null, null);
                await PopupNavigation.Instance.PushAsync(page);
                await pageProgress.OnClose();
            }
            finally
            {
                ToolBarPrint.IsEnabled = true;
            }
        }

        private void cvInventario_ScrollToRequested(object sender, ScrollToRequestEventArgs e)
        {
            System.Diagnostics.Debug.Write(sender);
        }
    }

    public class InventarioViewModel : InventarioVO
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