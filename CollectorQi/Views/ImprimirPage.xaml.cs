using CollectorQi.Models.ESCL018;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Services.ESCL018;
using CollectorQi.VO;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ImprimirPage : ContentPage, INotifyPropertyChanged
    {
        public ObservableCollection<InventarioFisicoViewModel> ObsInventario { get; }       
        public string localizacaoRetorno { get; set; }
        private InventarioVO _inventario;
        public string localizacao;  

        public ImprimirPage(InventarioVO pInventarioVO, string _localizacao)
        {
            InitializeComponent();

            _inventario = pInventarioVO;
            localizacao = _localizacao;
            //lblCodEstabel.Text = SecurityAuxiliar.GetCodEstabel();

            ////_ = InventarioItemDB.DeletarInventarioByInventarioId(inventarioVO.InventarioId);

            //pInventarioVO = inventarioVO;
            //BtnProximo.IsEnabled = false;
        }      

        async void OnClick_BuscaEtiqueta(object sender, EventArgs e)
        {
            var pageProgress = new ProgressBarPopUp("Carregando Consulta de Etiqueta...");
            await PopupNavigation.Instance.PushAsync(pageProgress);

            try
            {
                var inventario = new Inventario()
                {
                    IdInventario = _inventario.InventarioId,
                    CodEstabel = _inventario.CodEstabel,
                    CodDepos = _inventario.CodDepos //,
                    //CodigoBarras = txtEtiqueta.Text
                };

                var localizacao = new ParametersLocalizacaoLeituraEtiquetaService();
                var localizacaoResult = await localizacao.SendInventarioAsync(inventario);
                localizacaoRetorno = localizacaoResult.Resultparam.Localizacao;
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
                //BtnProximo.IsEnabled = true;
            }           
        }        

        //protected override bool OnBackButtonPressed()
        //{
        //    base.OnBackButtonPressed();
        //    //Application.Current.MainPage = new NavigationPage(new InventarioListaPage());

        //    return true;
        //}

        protected override bool OnBackButtonPressed()
        {
            PopupNavigation.Instance.PopAsync();
            return true;
        }

        async void BtnItem_Clicked(object sender, EventArgs e)
        {
            var page = new ImprimirEtiquetaItem(_inventario);
            await PopupNavigation.Instance.PushAsync(page);
        }

        async void BtnLocalizacao_Clicked(object sender, EventArgs e)
        {
            var page = new ImprimirEtiquetaLocalizacao(_inventario, localizacao);
            await PopupNavigation.Instance.PushAsync(page);
        }

        async void BtnReparo_Clicked(object sender, EventArgs e)
        {
            var page = new ImprimirEtiquetaReparo();
            await PopupNavigation.Instance.PushAsync(page);
        }

        public void BtnVoltar_Clicked(object sender, EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new InventarioListaItemPage(_inventario));
        }
    }
}