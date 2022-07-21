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
using CollectorQi.Models.ESCL018;
using Rg.Plugins.Popup.Services;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeituraEtiquetaLocaliza : ContentPage, INotifyPropertyChanged
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

        public ObservableCollection<FichasUsuarioVO> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                OnPropertyChanged("Items");
            }
        }

        private ObservableCollection<FichasUsuarioVO> _Items;
        public ObservableCollection<InventarioViewModel> ObsInventario { get; }
        public InventarioVO pInventarioVO { get; set; }
        public string localizacaoRetorno { get; set; }

        public ObservableCollection<InventarioViewModel> _localizacaoInventario;

        public ObservableCollection<InventarioViewModel> LocalizacaoInventario
        {
            get { return _localizacaoInventario; }
            set
            {
                _localizacaoInventario = value;
                OnPropertyChanged("LocalizacaoInventario");
            }
        }


        // public ObservableCollection<InventarioViewModel> LocalizacaoInventario = new ObservableCollection<InventarioViewModel>();

        public LeituraEtiquetaLocaliza(InventarioVO inventarioVO)
        {
            InitializeComponent();
          
            try
            {
                if (!string.IsNullOrEmpty(inventarioVO.CodEstabel) && !string.IsNullOrEmpty(inventarioVO.CodEstabel))
                lblCodEstabel.Text = inventarioVO.CodEstabel + " - " + inventarioVO.DescEstabel;

                pInventarioVO = new InventarioVO();
                pInventarioVO = inventarioVO;  
               
                Items = new ObservableCollection<FichasUsuarioVO>();

                var lstFichasUsuarioVO = new ObservableCollection<FichasUsuarioVO>(FichasUsuarioDB.GetFichasUsuarioBy().OrderBy(p => p.Localizacao).ToList());

                for (int i = 0; i < lstFichasUsuarioVO.Count; i++)
                {
                    var modelView = Mapper.Map<FichasUsuarioVO>(lstFichasUsuarioVO[i]);
                    Items.Add(modelView);
                }

                BtnProximo.IsEnabled = false;

                if(lstFichasUsuarioVO.Count != 0)
                    cvLeituraEtiqueta.BindingContext = this;

                //_ = InventarioItemDB.DeletarInventarioByInventarioId(inventarioVO.InventarioId);    
            }
            catch (Exception ex)
            {
                throw;
            }  
        }       

        async void OnClick_BuscaEtiqueta(object sender, System.EventArgs e)
        {
            var pageProgress = new ProgressBarPopUp("Carregando Consulta de Etiqueta...");
            await PopupNavigation.Instance.PushAsync(pageProgress);

            try
            {                
                var inventario = new Inventario()
                {
                    IdInventario = pInventarioVO.InventarioId,
                    CodEstabel = pInventarioVO.CodEstabel,
                    CodDepos = pInventarioVO.CodDepos,
                    CodigoBarras = txtEtiqueta.Text,
                };                

                var localizacao = new ParametersLocalizacaoLeituraEtiquetaService();
                var localizacaoResult = await localizacao.SendInventarioAsync(inventario);
                txtEtiqueta.Text = localizacaoRetorno = localizacaoResult.Resultparam.Localizacao;
            }
            catch (Exception ex)
            {
                var pageProgressErro = new ProgressBarPopUp("Erro: " + ex.Message);
                await PopupNavigation.Instance.PushAsync(pageProgressErro);
                await pageProgressErro.OnClose();
            }
            finally
            {
                await pageProgress.OnClose();
                BtnProximo.IsEnabled = true;
            }           
        }

        async void Criar()
        {
            var parametersFichasUsuario = new ParametersFichasUsuarioService();
            var lstInventarioVO = await parametersFichasUsuario.GetObterFichasUsuarioAsync();

            var lstInventarioVOSend = new List<InventarioItemVO>();

            //var lstInventarioVOFiltro = lstInventarioVO.param.Resultparam.Where(x => x.Localizacao == localizacaoRetorno);

            ObservableCollection<InventarioItemViewModel> lstInventarioListViewModel = new ObservableCollection<InventarioItemViewModel>();

            foreach (var item in lstInventarioVO.param.Resultparam /*.Where(x => x.Localizacao == localizacaoRetorno)*/ )
            {
                InventarioItemVO inventarioItem = new InventarioItemVO();
                inventarioItem.InventarioId = item.IdInventario;
                inventarioItem.CodLote = item.Lote;
                inventarioItem.CodLocaliz = item.Localizacao;
                inventarioItem.CodRefer = item.CodItem;
                inventarioItem.ItCodigo = item.CodItem;
               // inventarioItem.NrFicha = item.Quantidade;

                InventarioItemDB.InserirInventarioItem(inventarioItem);
                lstInventarioVOSend.Add(inventarioItem);

                lstInventarioListViewModel.Add(new InventarioItemViewModel
                {
                    ItCodigo = item.CodItem
                });
            }
        }

        async void OnClick_Proximo(object sender, System.EventArgs e)
        {
            Criar();
            Application.Current.MainPage = new NavigationPage(new InventarioListaItemPage(pInventarioVO));
        } 

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new InventarioListaPage());

            return true;
        }

        private void cvLeituraEtiqueta_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var current = (cvLeituraEtiqueta.SelectedItem as FichasUsuarioVO);

            if (current != null)
            {
                Criar();
                Application.Current.MainPage = new NavigationPage(new InventarioListaItemPage(pInventarioVO));
            }
        }
    }
}