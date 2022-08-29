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

                //var lstInventario = new ObservableCollection<InventarioLocalizacaoViewModel>();

                //var lstLocalizacoesVO = await ParametersObterLocalizacaoUsuarioService.GetObterLocalizacoesUsuarioAsync(_inventarioVO.InventarioId, this);
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
           /*
            var current = (cvInventario.SelectedItem as InventarioVO);

            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Erro!", "Para buscar os inventários no sistema o dispositivo deve estar conectado.", "CANCELAR");

                return;
            }

            BtnCarregaInventario.IsEnabled = false;

            var pageProgress = new ProgressBarPopUp("Carregando inventário, aguarde...");

            try
            {

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                ObsInventario.Clear();

                var parametersInventario = new ParametersInventarioService();
                var lstInventario = await parametersInventario.SendParametersAsync();
                var listInventario = new List<ModelInventario>();

                foreach (var item in lstInventario.param.Resultparam)
                {
                    var inventario = new ModelInventario();
                    inventario.dtSaldo = item.DtSaldo;
                    inventario.codEstabel = item.CodEstabel;
                    inventario.codDepos = item.CodDeposito;
                    inventario.idInventario = item.IdInventario;
                    inventario.DescEstabel = item.DescEstabel;
                    inventario.DescDepos = item.DescDepos;
                    listInventario.Add(inventario);

                    var inventarioVO = new InventarioVO();
                    inventarioVO.DtInventario = DateTime.ParseExact(item.DtSaldo, "dd/MM/yy", CultureInfo.InvariantCulture);
                    inventarioVO.CodEstabel = item.CodEstabel;
                    inventarioVO.CodDepos = item.CodDeposito;
                    inventarioVO.InventarioId = item.IdInventario;
                    inventarioVO.DescEstabel = item.DescEstabel;
                    inventarioVO.DescDepos = item.DescDepos;

                    var modelView = Mapper.Map<InventarioVO, InventarioViewModel>(inventarioVO);
                    ObsInventario.Add(modelView);
                }

                await Models.ConnectService.CriaInventario(listInventario);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                BtnCarregaInventario.IsEnabled = true;
                await pageProgress.OnClose();
            }
           */
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            ObsInventario = new ObservableCollection<InventarioViewModel>();

            CarregaListView();

            /*

            var pageProgress = new ProgressBarPopUp("Carregando inventário, aguarde...");

            try
            {

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                ObsInventario = new ObservableCollection<InventarioViewModel>();

                lblCodEstabel.Text = SecurityAuxiliar.Estabelecimento;

                var lstInventario = InventarioDB.GetInventarioAtivoByEstab(SecurityAuxiliar.GetCodEstabel()).OrderBy(p => p.CodDepos).OrderBy(p => p.DtInventario).ToList();

                for (int i = 0; i < lstInventario.Count(); i++)
                {
                    var modelView = Mapper.Map<InventarioVO, InventarioViewModel>(lstInventario[i]);
                    ObsInventario.Add(modelView);
                }

                if (ObsInventario.Count <= 0)
                {
                    await pageProgress.OnClose();
                    OnClick_CarregaInventario(new object(), new EventArgs());
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                await pageProgress.OnClose();
            }

            OnPropertyChanged("ObsInventario");
            */
        }


        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new PrincipalPage());

            return true;
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                ToolBarPrint.IsEnabled = false;

                var pageProgress = new ProgressBarPopUp("Carregando...");
                var page = new InventarioPrintPopUp(null,null);
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