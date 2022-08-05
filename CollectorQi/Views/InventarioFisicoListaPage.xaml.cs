using AutoMapper;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Services.ESCL018;
using CollectorQi.VO;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventarioFisicoListaPage : ContentPage, INotifyPropertyChanged
    {
        #region Property

        //public event PropertyChangedEventHandler PropertyChanged;

        //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        //protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        //{
        //    if (EqualityComparer<T>.Default.Equals(storage, value))
        //    {
        //        return false;
        //    }

        //    storage = value;
        //    OnPropertyChanged(propertyName);
        //    return true;
        //}

        #endregion

        public ObservableCollection<InventarioFisicoViewModel> ObsInventario { get; }

        public InventarioFisicoListaPage()
        {
            InitializeComponent();

            ObsInventario = new ObservableCollection<InventarioFisicoViewModel>();

            lblCodEstabel.Text = SecurityAuxiliar.GetCodEstabel();

            var lstInventario = InventarioDB.GetInventarioAtivoByEstab(SecurityAuxiliar.GetCodEstabel()).OrderBy(p => p.CodDepos).OrderBy(p => p.DtInventario).ToList();

            for (int i = 0; i < lstInventario.Count(); i++)
            {
                //_ = InventarioDB.DeletarInventario(lstInventario[i]);

                var modelView = Mapper.Map<InventarioVO, InventarioFisicoViewModel>(lstInventario[i]);
                lblCodEstabel.Text = lstInventario[i].CodEstabel + " - " + lstInventario[i].DescEstabel;

                ObsInventario.Add(modelView);
            }

            cvInventario.BindingContext = this;
        }

        async void Parametros()
        {
            var parametersFichasUsuario = new ParametersFichasUsuarioService();
            var lstInventarioVO = await parametersFichasUsuario.GetObterFichasUsuarioAsync();

            //var lstInventarioVOFiltro = lstInventarioVO.param.Resultparam.Where(x => x.Localizacao == localizacaoRetorno);

            var lstInventarioVOFiltro = lstInventarioVO.param.Resultparam.GroupBy(x => x.Localizacao);
            var listFichasUsuarioVO = new List<FichasUsuarioVO>();

            foreach (var item in lstInventarioVOFiltro.Select(g => g.Key))
            {
                var usuarioVO = new FichasUsuarioVO()
                {
                    Localizacao = item.ToString()
                };

                FichasUsuarioDB.InserirFichasUsuarioItem(usuarioVO);
            }
        }

        public void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cvInventario.SelectedItem == null)
                return;
            
            var current = (cvInventario.SelectedItem as VO.InventarioVO);

            //if (current != null)
            //{            
            //    var batchInventario = BatchInventarioDB.GetBatchInventario(current.InventarioId);

            //    // Inventário efetivado
            //    if (batchInventario != null &&
            //        batchInventario.StatusIntegracao != eStatusIntegracao.EnviadoIntegracao)
            //    {
            //        var result = await DisplayAlert("Inventário Efetivado", "Inventário está efetivado para integração com sistema, deseja habilitar alteração no inventário? Habilitando a alteração será cancelado a integração com o sistema", "Sim", "Não");

            //        if (result.ToString() == "True")
            //        {
            //            // Valida novamente se no meio da efetivação nao foi enviado para o TOTVS
            //            batchInventario = BatchInventarioDB.GetBatchInventario(current.InventarioId);

            //            if (batchInventario.StatusIntegracao != eStatusIntegracao.EnviadoIntegracao)
            //            {
            //                IntegracaoOnlineBatch.CancelaInventarioMobile(current.InventarioId);
            //                Application.Current.MainPage = new NavigationPage(new InventarioListaItemPage(current));
            //            }
            //            else
            //                await DisplayAlert("Erro!", "Inventário integrado com o sistema, não pode ser alterado.", "Cancelar");

            //        }
            //    }
            //    // Inventario efetivado e integrado com TOTVS
            //    else if (batchInventario != null)
            //    {
            //        await DisplayAlert("Erro!", "Inventário integrado com o sistema, não pode ser alterado.", "Cancelar");
            //    }
            //    else
            //    {
            //        Application.Current.MainPage = new NavigationPage(new InventarioListaItemPage(current));
            //    }
            //}
            //

            if (current != null)
            {
                Parametros();               

                Application.Current.MainPage = new NavigationPage(new LeituraEtiquetaLocaliza(current));
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

                    var modelView = Mapper.Map<InventarioVO, InventarioFisicoViewModel>(inventarioVO);
                    ObsInventario.Add(modelView);
                }

                //var parametersFichasUsuario = new ParametersFichasUsuarioService();
                //var lstInventarioVO = await parametersFichasUsuario.GetObterFichasUsuarioAsync();

                //foreach (var item in lstInventarioVO.param.Resultparam)
                //{
                //    InventarioItemVO inventarioItem = new InventarioItemVO();
                //    inventarioItem.InventarioId = item.IdInventario;
                //    inventarioItem.CodLote = item.Lote;
                //    inventarioItem.CodLocaliz = item.Localizacao;
                //    inventarioItem.CodRefer = item.CodItem;                     
                //    inventarioItem.NrFicha = item.Quantidade;                    

                //    InventarioItemDB.InserirInventarioItem(inventarioItempageProgress);
                //}  

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
            Application.Current.MainPage = new NavigationPage(new InventarioPage());

            return true;
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