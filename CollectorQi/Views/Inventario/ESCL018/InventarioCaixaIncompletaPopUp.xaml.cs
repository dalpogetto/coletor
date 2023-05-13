using AutoMapper;
using CollectorQi.Models.ESCL018;
using CollectorQi.Models.ESCL021;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Resources.DataBaseHelper.ESCL018;
using CollectorQi.Services.ESCL000;
using CollectorQi.Services.ESCL018;
using CollectorQi.Services.ESCL018B;
using CollectorQi.Services.ESCL021;
using CollectorQi.VO.ESCL018;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
//using Android.Graphics;

namespace CollectorQi.Views
{
    public partial class InventarioCaixaIncompletaPopUp : PopupPage
    {
        private int                     _inventarioId { get; set; }
        private InventarioItemVO        _inventarioItemVO;

        public Action<InventarioItemVO> _actDeleteRow;
        public Action<InventarioItemVO> _actRefreshPage;

        public InventarioCaixaIncompletaPopUp(int pInventarioId, InventarioItemVO pInventarioItem)        
        {
            try
            {
                InitializeComponent();

                _inventarioItemVO = pInventarioItem;
                _inventarioId     = pInventarioId;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected async override void OnAppearing()
        {
            var inventario = InventarioDB.GetInventario(_inventarioId);

            edtItCodigo.Text           = _inventarioItemVO.CodItem.ToString();
            edtCodEstabelecimento.Text = inventario.CodEstabel + " - " + inventario.DescEstabel;
            edtCodDeposito.Text        = inventario.CodDepos + " - " + inventario.DescDepos;
            edtCodigoBarras.Text       = _inventarioItemVO.CodigoBarras;
            edtLote.Text               = _inventarioItemVO.Lote;

           //if (_inventarioItemVO.Quantidade > 0)
           //{
           //    txtQuantidade.Text = _inventarioItemVO.Quantidade.ToString();
           //}

            if (String.IsNullOrEmpty(_inventarioItemVO.Lote))
            {
                FrameLote.IsVisible = false;
            }

            await edtLeituraEtiqueta();

            await Task.Run(async () =>
            { 
                await Task.Delay(300);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    /* if (!String.IsNullOrEmpty(edtCodigoBarras.Text))
                     {
                         txtQuantidade.Focus();
                     }
                     else if (SwtCxCompleta.On)
                     {
                         edtCodigoBarras.Focus();
                     }
                     else
                     {
                         txtQuantidade.Focus();
                     }
                    */


                    var security = await SecurityDB.GetSecurityAsync();

                    if (security != null && !security.CxCompleta)
                    {
                        SwtCxCompleta.On = false;
                    } else {
                        SwtCxCompleta.On = true;
                    }

                    txtQuantidade.Focus();
                });
            });
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

        private void SumButton(Entry entry, int value)
        {
            if (String.IsNullOrEmpty(entry.Text)) { entry.Text = "0"; }

            entry.Text = (decimal.Parse(entry.Text) + value).ToString();
        }

        async void OnClick_Efetivar(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtQuantidade.Text))
            {
                await DisplayAlert("Erro!", "Informe a quantidade do produto disponivel para a contagem do inventário", "Cancelar");
                return;
            }

            decimal decQuantidade = decimal.Parse(txtQuantidade.Text);

            if (string.IsNullOrEmpty(edtCodigoBarras.Text) && decQuantidade > 0)
            {
                await DisplayAlert("Erro!", "Informe o código de barras", "Cancelar");
                return;
            }
            else
            {
                if (decQuantidade == 0)
                {
                    edtCodigoBarras.Text = $"02;{_inventarioItemVO.CodItem.Trim()};1;1;1;0;1;1;1;1";
                }
            }

            var result = await DisplayAlert("Confirmação!", $"Deseja concluír a digitação com a quantidade de {txtQuantidade.Text} produto?", "Sim", "Não");

            var pageProgress = new ProgressBarPopUp("Carregando...");
            try
            {
                ServiceCommon.SetarAmbienteCulturaUSA();
                BtnEfetivar.IsEnabled = false;
                if (result.ToString() == "True")
                {
                    if (!string.IsNullOrEmpty(txtQuantidade.Text))
                    {

                        var inventarioBarra = new InventarioItemBarra()
                        {
                            IdInventario       = _inventarioItemVO.InventarioId,
                            Lote               = _inventarioItemVO.Lote.Trim(),
                            Localizacao        = _inventarioItemVO.Localizacao.Trim(),
                            CodItem            = _inventarioItemVO.CodItem.Trim(),
                            CodDepos           = _inventarioItemVO.__inventario__.CodDepos.Trim(),
                            QuantidadeDigitada = decQuantidade,
                            CodEmp             = SecurityAuxiliar.GetCodEmpresa(),
                            Contagem           = _inventarioItemVO.Contagem,
                            CodEstabel         = SecurityAuxiliar.GetCodEstabel(),
                            CodigoBarras       = CleanInput(edtCodigoBarras.Text.Trim())
                        };

                        _inventarioItemVO.Quantidade = decimal.Parse(txtQuantidade.Text);

                        _inventarioItemVO.CodigoBarras = CleanInput(edtCodigoBarras.Text);

                        _inventarioItemVO.CodigoBarras = _inventarioItemVO.CodigoBarras.Replace(";", "[");
                        inventarioBarra.CodigoBarras = inventarioBarra.CodigoBarras.Replace(";", "[");

                        var resultService = await ParametersLeituraEtiquetaService.SendInventarioAsync(inventarioBarra, _inventarioItemVO, 0 , this, false);

                        if (resultService != null && resultService.Retorno != null)
                        {
                            if (resultService.Retorno == "OK")
                            {
                                _actDeleteRow(_inventarioItemVO);

                                await pageProgress.OnClose();
                                OnBackButtonPressed();
                            }
                            else if (resultService.Retorno == "IntegracaoBatch")
                            {
                                await DisplayAlert("Atenção!", "Erro de conexão com ERP, a atualização do item será integrado de forma Offline", "OK");
                                
                                _inventarioItemVO.StatusIntegracao = eStatusInventarioItem.ErroIntegracao;
                                _actRefreshPage(_inventarioItemVO);

                                await pageProgress.OnClose();
                                OnBackButtonPressed();
                            }
                            else {
                                if (resultService.Resultparam != null && resultService.Resultparam.Count > 0)
                                {
                                    await DisplayAlert("Erro!", resultService.Resultparam[0].ErrorDescription + " - " + resultService.Resultparam[0].ErrorHelp, "Cancelar");
                                }
                                else 
                                {
                                    await DisplayAlert("Erro!", "Erro na efetivação do inventário", "Cancelar");
                                }
                            }
                        }
                        else
                        {
                            await DisplayAlert("Erro!", "Erro na efetivação do inventário", "Cancelar");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                ServiceCommon.SetarAmbienteCulturaBrasil();
                BtnEfetivar.IsEnabled = true;
                await pageProgress.OnClose();
            }
        }

        static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, @"[^0-9a-zA-Z;./-]+", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters,
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
            }
        }

        private async Task edtLeituraEtiqueta()
        {
            try
            {
                if (!String.IsNullOrEmpty(edtCodigoBarras.Text))
                {
                    if (SwtCxCompleta.On)
                    {
                        /*
                        var result = await ParametersEtiquetaPAM.ObterEtiqueta(edtCodigoBarras.Text);

                        if (result != null && result.Qtde > 0)
                        {
                            txtQuantidade.Text = result.Qtde.ToString();
                        }
                        */


                        var dadosLeituraItemTransferenciaDeposito = new DadosLeituraItemTransferenciaDeposito()
                        {
                            CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                            //CodDeposOrigem = _codDeposSaidaHidden,
                            //CodLocalizaOrigem = edtLocalizacaoSaida.Text ?? String.Empty,
                            CodigoBarras = edtCodigoBarras.Text
                        };

                        var itemLeituraEtiquetaServiceRetorno = await LeituraEtiquetaTransferenciaDepositoService.SendLeituraEtiquetaAsync(dadosLeituraItemTransferenciaDeposito);

                        if (itemLeituraEtiquetaServiceRetorno != null &&
                            itemLeituraEtiquetaServiceRetorno.Param != null &&
                            itemLeituraEtiquetaServiceRetorno.Param.ParamResult != null &&
                            itemLeituraEtiquetaServiceRetorno.Param.ParamResult.Count > 0)
                        {

                            var item = itemLeituraEtiquetaServiceRetorno.Param.ParamResult[0];

                            if (String.IsNullOrEmpty(item.CodItem))
                            {
                                throw new Exception("Não foi possivel ler a etiqueta.");
                            }

                            try
                            {
                                txtQuantidade.Text = decimal.Parse(item.Quantidade, ServiceCommon.ObterCulturaUSA).ToString();
                            }
                            catch
                            {
                                txtQuantidade.Text = item.Quantidade;
                            }

                            if (txtQuantidade != null)
                                txtQuantidade.Text = txtQuantidade.Text.Replace(".0", "").Trim();

                            // txtQuantidade.Text = item.Quantidade;

                        }
                    }
                }
            }
            catch { }
        }

        private async void edtCodigoBarras_UnFocused(object sender, FocusEventArgs e)
        {
            await edtLeituraEtiqueta();
        }

        private async void SwtCxCompleta_OnChanged(object sender, ToggledEventArgs e)
        {
            try
            {
                bool blnCx = false;
                if (SwtCxCompleta.On)
                {
                    blnCx = true;
                }
                else
                {
                    blnCx = false;
                }

                await SecurityDB.AtualizarSecurityParametros(blnCx) ;
            }
            catch { }
        }
    }
}