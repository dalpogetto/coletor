using AutoMapper;
using CollectorQi.Models.ESCL018;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Services.ESCL018;
using CollectorQi.VO;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Xamarin.Forms;
//using Android.Graphics;

namespace CollectorQi.Views
{
    public partial class ImprimirEtiquetaReparo : PopupPage
    {
        private VO.InventarioVO _inventarioVO;
        private VO.InventarioItemVO _inventarioItemVO;
        private ObservableCollection<InventarioItemViewModel> _Items;
        public Action<VO.InventarioItemVO,bool> ResultAction;
        private string localizacao;

        public ImprimirEtiquetaReparo()        
        {    
            InitializeComponent();

            //lblLocalizacao.Text += localizacao;
            //lblCodEstabelecimento.Text += pInventarioVO.CodEstabel;
            //lblCodDeposito.Text += pInventarioVO.CodDepos;
        }

        public void SetNovoItemInventario(string pItCodigo, string pDescItem, string pUnidade, string pTipoConEst, string pLote, string pDtValiLote)
        {
            //edtItCodigo.Text = pItCodigo;
            //edtDescItem.Text = pDescItem;
            //edtUnidade.Text = pUnidade;
            //edtTipoConEst.Text = pTipoConEst;
            //edtLote.Text = pLote;
            //edtDtValiLote.Text = pDtValiLote;
        }       

        protected override bool OnBackButtonPressed()
        {            
            PopupNavigation.Instance.PopAsync();
            return true;
        }

        async void OnClick_Imprimir(object sender, EventArgs e)
        {
            var param = new ParametersImprimirEtiquetaService();

            var impressaoReparo = new ImpressaoReparo()
            {
                CodigoBarras = txtCodBarra.Text,
                CodEstabel = txtCodEstabelecimento.Text,
                CodFilial = txtCodFilial.Text,
                CodItem = txtCodItem.Text,
                NumRR = int.Parse(txtNumRR.Text),
                Digito = int.Parse(txtDigito.Text)
            };

            var _impressaoReparo = await param.SendImpressaoAsync(null, null, impressaoReparo, 3);

            var pageProgress = new ProgressBarPopUp(_impressaoReparo.Resultparam.OK);
            Thread.Sleep(2000);
            await pageProgress.OnClose();

            OnBackButtonPressed();
        }

        private void BtnVoltar_Clicked(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }

        private void BtnScan_Clicked(object sender, EventArgs e)
        {

        }
    }
}