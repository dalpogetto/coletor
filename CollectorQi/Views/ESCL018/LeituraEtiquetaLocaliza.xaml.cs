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
        public InventarioVO _inventarioVO { get; set; }
        public string localizacaoRetorno { get; set; }

        public LeituraEtiquetaLocaliza(InventarioVO inventarioVO)
        {
            InitializeComponent();

            _inventarioVO = inventarioVO;

            cvLeituraEtiqueta.BindingContext = this;

            /*
            try
            {
              
            }
            catch (Exception ex)
            {
                throw;
            } */
        }

        async void OnClick_BuscaEtiqueta(object sender, System.EventArgs e)
        {
            var pageProgress = new ProgressBarPopUp("Carregando Consulta de Etiqueta...");
            await PopupNavigation.Instance.PushAsync(pageProgress);

            try
            {
                var inventario = new Inventario()
                {
                    IdInventario = _inventarioVO.InventarioId,
                    CodEstabel = _inventarioVO.CodEstabel,
                    CodDepos = _inventarioVO.CodDepos,
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

            /*
            var parametersFichasUsuario = new ParametersFichasUsuarioService();
            var lstInventarioVO = await parametersFichasUsuario.GetObterFichasUsuarioAsync(_inventarioVO.InventarioId);

            //var lstInventarioVOFiltro = lstInventarioVO.param.Resultparam.Where(x => x.Localizacao == localizacaoRetorno);

            foreach (var item in lstInventarioVO.param.Resultparam.Where(x => x.Localizacao == localizacaoRetorno))
            {
                InventarioItemVO inventarioItem = new InventarioItemVO();
                inventarioItem.InventarioId = item.IdInventario;
                inventarioItem.CodLote = item.Lote;
                inventarioItem.CodLocaliz = item.Localizacao;
                inventarioItem.CodRefer = item.CodItem;
               // inventarioItem.NrFicha = item.Quantidade;

                InventarioItemDB.InserirInventarioItem(inventarioItem);
            } */
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var pageProgress = new ProgressBarPopUp("Carregando inventário, aguarde...");

            try
            {

                /*
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                var parametersFichasUsuario = new ParametersFichasUsuarioService();
                var lstInventarioVO = await parametersFichasUsuario.GetObterFichasUsuarioAsync(_inventarioVO.InventarioId);
                var lstInventarioVOFiltro = lstInventarioVO.param.Resultparam.GroupBy(x => x.Localizacao);
                var listFichasUsuarioVO = new List<FichasUsuarioVO>();

                await FichaUsuarioItem.DeleteFichaUsuarioItem();

                foreach (var item in lstInventarioVOFiltro)
                {
                    var usuarioVO = new FichasUsuarioVO()
                    {
                        Localizacao = item.ToString()
                    };

                    FichaUsuarioItem.InserirFichasUsuarioItem(usuarioVO);
                }

                //pInventarioVO = new InventarioVO();
                //pInventarioVO = inventarioVO;

                Items = new ObservableCollection<FichasUsuarioVO>();

                var lstFichasUsuarioVO = new ObservableCollection<FichasUsuarioVO>(FichaUsuarioItem.GetFichasUsuarioBy().OrderBy(p => p.Localizacao).ToList());

                for (int i = 0; i < lstFichasUsuarioVO.Count; i++)
                {
                    var modelView = Mapper.Map<FichasUsuarioVO>(lstFichasUsuarioVO[i]);
                    Items.Add(modelView);
                }

                OnPropertyChanged("Items");

                //BtnProximo.IsEnabled = false;
                

                //if (lstFichasUsuarioVO.Count != 0)


                //_ = InventarioItemDB.DeletarInventarioByInventarioId(inventarioVO.InventarioId);
                //
                */
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

        async void OnClick_Proximo(object sender, System.EventArgs e)
        {
            Criar();
          //  Application.Current.MainPage = new NavigationPage(new InventarioListaItemPage(_inventarioVO));
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
              //
              //  Application.Current.MainPage = new NavigationPage(new InventarioListaItemPage(_inventarioVO));
            }
        }
    }
}