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

        public ObservableCollection<InventarioViewModel> ObsInventario { get; }
        public InventarioVO pInventarioVO { get; }
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

            lblCodEstabel.Text = SecurityAuxiliar.GetCodEstabel() + " - PROCOMP INDUSTRIA ELETRONICA LTDA";

            //_ = InventarioItemDB.DeletarInventarioByInventarioId(inventarioVO.InventarioId);

            pInventarioVO = inventarioVO;
            //BtnProximo.IsEnabled = false;

            this.Title = "Buscar a localização (Depósito: " + inventarioVO.CodDepos;

            cvInventario.BindingContext = this;

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
                    CodigoBarras = txtEtiqueta.Text
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

        protected override void OnAppearing()
        {
            base.OnAppearing();

            LocalizacaoInventario = new ObservableCollection<InventarioViewModel>();

            LocalizacaoInventario.Add(new InventarioViewModel
            {
                CodEstabel = "F01GV03101"
            });

            LocalizacaoInventario.Add(new InventarioViewModel
            {
                CodEstabel = "F01GV05701"
            });

            OnPropertyChanged("LocalizacaoInventario");
        }

        async void OnClick_Proximo(object sender, System.EventArgs e)
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

            Application.Current.MainPage = new NavigationPage(new InventarioListaItemPage(pInventarioVO, lstInventarioListViewModel, localizacaoRetorno));
        } 

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new InventarioListaPage());

            return true;
        }
    }
}