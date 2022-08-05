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
    public partial class ImprimirEtiquetaLocalizacao : PopupPage
    {
        private VO.InventarioVO _inventarioVO;
        private VO.InventarioItemVO _inventarioItemVO;
        private ObservableCollection<InventarioItemViewModel> _Items;
        public Action<VO.InventarioItemVO,bool> ResultAction;
        private string localizacao;

        public ImprimirEtiquetaLocalizacao(InventarioVO pInventarioVO, string localizacao)        
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
            /*
            var param = new ParametersImprimirEtiquetaService();

            var impressaoLocalizacao = new ImpressaoLocalizacao()
            {
                CodEstabel = txtCodEstabelecimento.Text,
                CodDeposito = txtCodDeposito.Text,
                Localizacao = txtLocalizacao.Text,
                AreaIni = txtAreaInicio.Text,
                AreaFim = txtAreaFim.Text,
                RuaIni = txtRuaInicio.Text,
                RuaFim = txtRuaFim.Text
            };

            var _impressaoLocalizacao = await param.SendImpressaoAsync(null, impressaoLocalizacao, null, 2);

            var pageProgress = new ProgressBarPopUp(_impressaoLocalizacao.Resultparam.OK);
            Thread.Sleep(2000);
            await pageProgress.OnClose();

            OnBackButtonPressed();      
            */
        }

        private void BtnVoltar_Clicked(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }
    }
}