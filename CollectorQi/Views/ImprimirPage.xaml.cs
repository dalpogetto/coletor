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
        public ObservableCollection<InventarioViewModel> ObsInventario { get; }
        public InventarioVO pInventarioVO { get; }
        public string localizacaoRetorno { get; set; }

        public ImprimirPage()
        {
            InitializeComponent();

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
                    IdInventario = pInventarioVO.InventarioId,
                    CodEstabel = pInventarioVO.CodEstabel,
                    CodDepos = pInventarioVO.CodDepos //,
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

        async void OnClick_Proximo(object sender, EventArgs e)
        {

            //var parametersFichasUsuario = new ParametersFichasUsuarioService();
            //var lstInventarioVO = await parametersFichasUsuario.GetObterFichasUsuarioAsync();

            ////var lstInventarioVOFiltro = lstInventarioVO.param.Resultparam.Where(x => x.Localizacao == localizacaoRetorno);

            //foreach (var item in lstInventarioVO.param.Resultparam.Where(x => x.Localizacao == localizacaoRetorno))
            //{
            //    InventarioItemVO inventarioItem = new InventarioItemVO();
            //    inventarioItem.InventarioId = item.IdInventario;
            //    inventarioItem.CodLote = item.Lote;
            //    inventarioItem.CodLocaliz = item.Localizacao;
            //    inventarioItem.CodRefer = item.CodItem;
            //    inventarioItem.NrFicha = item.Quantidade;

            //    InventarioItemDB.InserirInventarioItem(inventarioItem);
            //}

            //Application.Current.MainPage = new NavigationPage(new InventarioListaItemPage(pInventarioVO, null));
        } 

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new InventarioListaPage());

            return true;
        }

        async void BtnItem_Clicked(object sender, EventArgs e)
        {
            var inventario = new Inventario()
            { 
                CodEstabel = "101",
                CodDepos = "DAT",
                QuantidadeDigitada = 1                
            };

            //"CodEstabel": "101",
            //"CodDeposito": "DAT",
            //"CodItem": "65.111.06998-2",
            //"Quantidade": 1,
            //"QtdeEtiqueta": 1

            var imprimir = new ParametersImprimirEtiquetaService();
            await imprimir.SendImpressaoAsync(inventario);
        }

        async void BtnLocalizacao_Clicked(object sender, EventArgs e)
        {
            var inventario = new Inventario()
            {
                CodEstabel = "101",
                CodDepos = "DAT",
                QuantidadeDigitada = 1
            };

            var imprimir = new ParametersImprimirEtiquetaService();
            await imprimir.SendImpressaoAsync(inventario);
        }

        async void BtnReparo_Clicked(object sender, EventArgs e)
        {
            var inventario = new Inventario()
            {
                CodEstabel = "101",
                CodDepos = "DAT",
                QuantidadeDigitada = 1
            };

            var imprimir = new ParametersImprimirEtiquetaService();
            await imprimir.SendImpressaoAsync(inventario);
        }
    }
}