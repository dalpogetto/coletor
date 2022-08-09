using AutoMapper;
using CollectorQi.Models.ESCL018;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Services.ESCL018;
using CollectorQi.VO;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
//using Android.Graphics;

namespace CollectorQi.Views
{
    public partial class InventarioCaixaIncompletaPopUp : PopupPage
    {
        public ObservableCollection<InventarioItemViewModel> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                OnPropertyChanged("Items");
            }
        }

        //private int             _inventarioId = 0;
        private VO.InventarioVO _inventarioVO;
        private VO.InventarioItemVO _inventarioItemVO;
        private ObservableCollection<InventarioItemViewModel> _Items;
        public Action<VO.InventarioItemVO,bool> ResultAction;
        private string localizacao;

        public Action<InventarioItemVO> _actDeleteRow;

        public InventarioCaixaIncompletaPopUp(int pInventarioId, InventarioItemVO inventarioItem)        
        {
            try
            {
                InitializeComponent();

                _inventarioItemVO = inventarioItem;

                //Items = _Items;
                _inventarioVO = InventarioDB.GetInventario(pInventarioId).Result;

                edtItCodigo.Text = inventarioItem.ItCodigo.ToString();
                edtCodEstabelecimento.Text = _inventarioVO.CodEstabel;
                edtCodDeposito.Text = _inventarioVO.CodDepos;
                edtCodigoBarras.Text = inventarioItem.CodigoBarras;
                edtLote.Text = inventarioItem.CodLote;

                if (String.IsNullOrEmpty(inventarioItem.CodLote)) ;
                {
                    FrameLote.IsVisible = false;
                }
                
                //edtDtSaldo.Text = _inventarioVO.DtInventario.ToString("dd/MM/yy");
                //localizacao = _localizacao;


                //if (pInventarioItemId > 0)
                //{
                //    _inventarioItemVO = InventarioItemDB.GetInventarioItem(pInventarioItemId);

                //    if (_inventarioItemVO != null)
                //    {
                //        edtItCodigo.Text = _inventarioItemVO.CodRefer;
                //        edtDescItem.Text = _inventarioItemVO.CodLocaliz;
                //    }

                //    //if (_inventarioItemVO != null)
                //    //{
                //    //    edtItCodigo.Text = _inventarioItemVO.ItCodigo;
                //    //    edtDescItem.Text = _inventarioItemVO.__item__.DescItem;
                //    //    edtUnidade.Text = _inventarioItemVO.__item__.Un;
                //    //    edtTipoConEst.Text = _inventarioItemVO.__item__.__TipoConEst__;
                //    //    edtLote.Text = _inventarioItemVO.CodLote;
                //    //    edtDtValiLote.Text = _inventarioItemVO.DtUltEntr.HasValue ? _inventarioItemVO.DtUltEntr.Value.ToString("dd/MM/yyyy") : String.Empty;
                //    //    edtQuantidade.Text = _inventarioItemVO.QtdDigitada ? _inventarioItemVO.ValApurado.ToString() : String.Empty;
                //    //    edtQuantidade.Focus();
                //    //}
                //}
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void SetNovoItemInventario(string pItCodigo, string pDescItem, string pUnidade, string pTipoConEst, string pLote, string pDtValiLote)
        {
            //edtItCodigo.Text = pItCodigo;
            //edtDescItem.Text = pDescItem;
            //edtUnidade.Text = pUnidade;
            //edtTipoConEst.Text = pTipoConEst;
            edtLote.Text = pLote;
            edtDtValiLote.Text = pDtValiLote;
        }

        protected async override void OnAppearing()
        {
            await Task.Run(async () =>
            { 
                await Task.Delay(100);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    txtQuantidade.Focus();
                });
            });
        }

        public void SetResultDigita(Action<VO.InventarioItemVO, bool> dp)
        {
            ResultAction = dp;
        }
        
        async void BtnVoltar_Clicked(object sender, EventArgs e)
        {
            OnBackButtonPressed();
           
        }

        void BtnLimpar_Clicked(object sender, EventArgs e)
        {
            txtQuantidade.Text = string.Empty;
            edtCodigoBarras.Text = string.Empty;

        }
        protected override bool OnBackButtonPressed()
        {            
            PopupNavigation.Instance.PopAsync();
            return true;
        }

        private void BtnQuantidadeSum1_Clicked(object sender, EventArgs e)
        {
            SumButton(txtQuantidade, 1);
        }
        private void BtnQuantidadeSum10_Clicked(object sender, EventArgs e)
        {
            SumButton(txtQuantidade, 10);
        }
        private void BtnQuantidadeSum100_Clicked(object sender, EventArgs e)
        {
            SumButton(txtQuantidade, 100);
        }

        private void SumButton(CustomEntry entry, int value)
        {
            if (String.IsNullOrEmpty(entry.Text)) { entry.Text = "0"; }

            entry.Text = (int.Parse(entry.Text) + value).ToString();
        }



        async void OnClick_Efetivar(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtQuantidade.Text))
            {
                await DisplayAlert("Erro!", "Informe a quantidade do produto disponivel para a contagem do inventário", "Cancelar");
                return;
            }


            if (string.IsNullOrEmpty(edtCodigoBarras.Text))
            {
                await DisplayAlert("Erro!", "Informe o código de barras", "Cancelar");
                return;
            }

            var result = await DisplayAlert("Confirmação!", $"Deseja concluír a digitação com a quantidade de {txtQuantidade.Text} produto?", "Sim", "Não");

            var pageProgress = new ProgressBarPopUp("Carregando...");
            try
            {

                BtnEfetivar.IsEnabled = false;
                if (result.ToString() == "True")
                {
                    if (!string.IsNullOrEmpty(txtQuantidade.Text))
                    {
                        /*
                        var inventario = new InventarioItem()
                        {
                            IdInventario = _inventarioVO.InventarioId,
                            Lote = _inventarioItemVO.CodLote,
                            Localizacao = _inventarioItemVO.CodLocaliz,
                            CodItem = _inventarioItemVO.ItCodigo,
                            CodDepos = _inventarioVO.CodDepos,
                            Quantidade = int.Parse(txtQuantidade.Text),
                            CodEmp = "1",
                            Contagem = 1,
                            CodEstabel = SecurityAuxiliar.GetCodEstabel(),


                        };
                        */

                        var inventarioBarra = new InventarioItemBarra()
                        {
                            IdInventario = _inventarioVO.InventarioId,
                            Lote = _inventarioItemVO.CodLote.Trim(),
                            Localizacao = _inventarioItemVO.CodLocaliz.Trim(),
                            CodItem = _inventarioItemVO.ItCodigo.Trim(),
                            CodDepos = _inventarioVO.CodDepos.Trim(),
                            QuantidadeDigitada = int.Parse(txtQuantidade.Text),
                            CodEmp = "1",
                            Contagem = 1,
                            CodEstabel = SecurityAuxiliar.GetCodEstabel().Trim(),
                            CodigoBarras = edtCodigoBarras.Text.Trim()


                        };

                        var resultService = await ParametersLeituraEtiquetaService.SendInventarioAsync(inventarioBarra);

                        if (resultService != null && resultService.Retorno != null)
                        {
                            if (resultService.Retorno == "OK")
                            {
                                //       var _inventarioItem = await ParametersGravarFichasUsuarioService.SendGravarFichasUsuarioAsync(inventario);

                                //     pageProgress = new ProgressBarPopUp(_inventarioItem.paramConteudo.Ok);
                                //Thread.Sleep(2000);

                                _actDeleteRow(_inventarioItemVO);

                                await pageProgress.OnClose();
                                OnBackButtonPressed();
                            }
                            else
                            {

                                
                                if (resultService.Resultparam != null && resultService.Resultparam.Count > 0)
                                {

                                    await DisplayAlert("Erro!", resultService.Resultparam[0].ErrorDescription + " - " + resultService.Resultparam[0].ErrorHelp, "Cancelar");
                                }
                                else 
                                {
                                    await DisplayAlert("Erro!", "Erro na efetivação do inventário", "Cancelar");
                                }
                                // await DisplayAlert("Erro!", resultService.Retorno, "Cancelar");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Erro!", "Erro na efetivação do inventário", "Cancelar");
                        }
                        //
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                BtnEfetivar.IsEnabled = true;
                await pageProgress.OnClose();
            }


            /*else
                pageProgress = new ProgressBarPopUp("Digite uma quantidade !"); */


            //if (String.IsNullOrEmpty(edtQuantidade.Text))
            //{
            //    await DisplayAlert("Erro!", "Quantidade digitada inválida", "Cancelar");

            //    edtQuantidade.Focus();

            //    return;
            //}

            //var param = new ParametersItemLeituraEtiquetaService();

            //var inventario = new Inventario()
            //{
            //    IdInventario = _inventarioVO
            //    .InventarioId,
            //    CodEstabel = _inventarioVO.CodEstabel,
            //    CodDepos = _inventarioVO.CodDepos,
            //    Localizacao = localizacao,
            //    Lote = "",
            //    QuantidadeDigitada = int.Parse(edtQuantidade.Text),
            //    CodigoBarras = "02[65.116.00709-1[1[2[3[4[5[6[1[8"  // receber do leitor
            //};

            //var _inventarioItem = await param.SendInventarioAsync(inventario);

            //var filtroReturn = Items.FirstOrDefault(x => x.CodRefer == _inventarioItem.paramConteudo.Resultparam[0].CodItem);
            //filtroReturn.Quantidade += int.Parse(edtQuantidade.Text);

            //foreach (var item in Items)
            //{
            //    if (item.CodRefer == _inventarioItem.paramConteudo.Resultparam[0].CodItem)
            //    {
            //        Items.Remove(item);
            //        break;
            //    }
            //}

            //var modelView = Mapper.Map<InventarioItemVO, InventarioItemViewModel>(filtroReturn);
            //Items.Add(modelView);

            ////await PopupNavigation.Instance.PushAsync(new ProgressBarPopUp("Carregando..."));
            //var pageProgress = new ProgressBarPopUp("Carregando... !");
            //Application.Current.MainPage = new NavigationPage(new InventarioListaItemPage(_inventarioVO));
            //await pageProgress.OnClose();  

            //OnBackButtonPressed();                       
        }
    }
}