using CollectorQi.Models;
using CollectorQi.ViewModels;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Resources.DataBaseHelper.Batch;
using CollectorQi.Resources;
using CollectorQi.VO;
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

        public ObservableCollection<InventarioViewModel> ObsInventario { get; }

        public InventarioListaPage()
        {
            InitializeComponent();

            ObsInventario = new ObservableCollection<InventarioViewModel>();
            //lblCodEstabel.Text = SecurityAuxiliar.Estabelecimento;  
            lblCodEstabel.Text = SecurityAuxiliar.GetCodEstabel();

            var lstInventario = InventarioDB.GetInventarioAtivoByEstab(SecurityAuxiliar.GetCodEstabel()).OrderBy(p => p.CodDepos).OrderBy(p => p.DtInventario).ToList();

            for (int i = 0; i < lstInventario.Count(); i++)
            {
                var modelView = Mapper.Map<InventarioVO, InventarioViewModel>(lstInventario[i]);

                ObsInventario.Add(modelView);
            }

            cvInventario.BindingContext = this;
        }

        async void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var current = (cvInventario.SelectedItem as VO.InventarioVO);

            if (current != null)
            {            
                //var batchInventario = BatchInventarioDB.GetBatchInventario(current.InventarioId);
                var batchInventario = BatchInventarioDB.GetBatchInventario(current.CodInventario);

                // Inventário efetivado
                if (batchInventario != null &&
                    batchInventario.StatusIntegracao != eStatusIntegracao.EnviadoIntegracao)
                {
                    var result = await DisplayAlert("Inventário Efetivado", "Inventário está efetivado para integração com sistema, deseja habilitar alteração no inventário? Habilitando a alteração será cancelado a integração com o sistema", "Sim", "Não");

                    if (result.ToString() == "True")
                    {
                        // Valida novamente se no meio da efetivação nao foi enviado para o TOTVS
                        batchInventario = BatchInventarioDB.GetBatchInventario(current.InventarioId);

                        if (batchInventario.StatusIntegracao != eStatusIntegracao.EnviadoIntegracao)
                        {
                            IntegracaoOnlineBatch.CancelaInventarioMobile(current.InventarioId);
                            Application.Current.MainPage = new NavigationPage(new InventarioListaItemPage(current));
                        }
                        else
                            await DisplayAlert("Erro!", "Inventário integrado com o sistema, não pode ser alterado.", "Cancelar");

                    }
                }
                // Inventario efetivado e integrado com TOTVS
                else if (batchInventario != null)
                {
                    await DisplayAlert("Erro!", "Inventário integrado com o sistema, não pode ser alterado.", "Cancelar");
                }
                else
                {
                    Application.Current.MainPage = new NavigationPage(new InventarioListaItemPage(current));
                }
            }

            cvInventario.SelectedItem = null;
        }

        async void OnClick_CarregaInventario(object sender, System.EventArgs e)
        {
            var current = (cvInventario.SelectedItem as VO.InventarioVO);

            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Erro!", "Para buscar os inventários no sistema o dispositivo deve estar conectado.", "CANCELAR");

                return;
            }

            BtnCarregaInventario.IsEnabled = false;

            var pageProgress = new ProgressBarPopUp("Carregando inventário, aguarde...");

            try
            {
                ObsInventario.Clear();

                //await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                //var lstInventarioERP = Models.Controller.GetInventario();                 

                //var lstInventarioAsync = InventarioDB.GetInventarioAtivoByEstab(SecurityAuxiliar.GetCodEstabel());
                //var lstInventario = lstInventarioAsync.OrderBy(p => p.CodDepos).OrderBy(p => p.DtInventario).ToList();

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
                    listInventario.Add(inventario);

                    var inventarioVO = new InventarioVO();
                    inventarioVO.DtInventario = DateTime.ParseExact(item.DtSaldo, "dd/MM/yy", CultureInfo.InvariantCulture);
                    inventarioVO.CodEstabel = item.CodEstabel;
                    inventarioVO.CodDepos = item.CodDeposito;
                    inventarioVO.CodInventario = item.IdInventario;

                    var modelView = Mapper.Map<InventarioVO, InventarioViewModel>(inventarioVO);
                    ObsInventario.Add(modelView);
                }

                var parametersFichasUsuario = new ParametersFichasUsuarioService();
                var lstInventarioVO = await parametersFichasUsuario.GetObterFichasUsuarioAsync();

                foreach (var item in lstInventarioVO.param.Resultparam)
                {
                    //var itemView = new InventarioItemViewModel() { CodRefer = item.CodItem, CodLocaliz = item.Localizacao, NrFicha = item.Quantidade };
                    //var modelView = Mapper.Map<InventarioItemVO, InventarioItemViewModel>(itemView);

                    InventarioItemVO inventarioItem = new InventarioItemVO();
                    inventarioItem.InventarioId = item.IdInventario;
                    inventarioItem.CodLote = item.Lote;
                    inventarioItem.CodLocaliz = item.Localizacao;
                    inventarioItem.CodRefer = item.CodItem;                     
                    inventarioItem.NrFicha = item.Quantidade;

                    InventarioItemDB.InserirInventarioItem(inventarioItem);
                }

                //"IdInventario": 796,
                //"Lote": "",
                //"CodEstabel": "101",
                //"Localizacao": "F01GV00203",
                //"CodItem": "65.111.06998-2",
                //"Contagem": 1,
                //"Serie": "",
                //"IVL": 294674,
                //"CodEmp": "1",
                //"CodDepos": "DAT",
                //"Quantidade": 0

                //InventarioItemVO inventarioItem = new InventarioItemVO();
                //inventarioItem.InventarioId = Inventario.InventarioId;
                //inventarioItem.ItemId = Item_VO.ItemId;
                //inventarioItem.CodigoItem = edtItCodigo.Text.Trim();
                //inventarioItem.Lote = edtLote.Text.Trim().ToUpper();
                //inventarioItem.Quantidade = Qtde;


                await Models.Controller.CriaInventario(listInventario);
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
        }

        //protected async override void OnAppearing()
        //{
        //    base.OnAppearing();

        //    ObsInventario.Clear();

        //    var lstInventarioAsync = InventarioDB.GetInventarioByEstab(SecurityAuxiliar.GetCodEstabel());

        //    var lstInventario = lstInventarioAsync.OrderBy(p => p.CodDepos).OrderBy(p => p.DtInventario).ToList();

        //    for (int i = 0; i < lstInventario.Count(); i++)
        //    {
        //        var modelView = Mapper.Map<InventarioVO, InventarioViewModel>(lstInventario[i]);

        //        ObsInventario.Add(modelView);
        //    }
        //}


        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new PrincipalPage());

            return true;
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