using CollectorQi.Models.ESCL018;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL006;
using CollectorQi.Services.ESCL018;
using CollectorQi.VO;
using CollectorQi.VO.ESCL018;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ArmazenagemImprimirPage : ContentPage, INotifyPropertyChanged
    {
        public ObservableCollection<InventarioFisicoViewModel> ObsInventario { get; }
        public string localizacaoRetorno { get; set; }
        private InventarioVO _inventario;
        public string localizacao;

        public ArmazenagemImprimirPage()
        {
            InitializeComponent();

            //_inventario = pInventarioVO;
            //localizacao = _localizacao;
            lblCodEstabel.Text = SecurityAuxiliar.GetCodEstabel();

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
                    IdInventario = _inventario.IdInventario,
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

        async void BtnFilial_Clicked(object sender, EventArgs e)
        {
            await PopupNavigation.Instance.PushAsync(new ArmazenagemImprimirFilial());
        }
    }
}