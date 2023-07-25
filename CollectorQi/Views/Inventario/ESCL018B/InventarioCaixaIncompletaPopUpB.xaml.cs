using Android.OS;
using AutoMapper;
using CollectorQi.Models.ESCL018;
using CollectorQi.Resources;
using CollectorQi.Resources.Batch;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Resources.DataBaseHelper.ESCL018;
using CollectorQi.Services.ESCL000;
using CollectorQi.Services.ESCL018;
using CollectorQi.Services.ESCL018B;
using CollectorQi.VO.ESCL018;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
//using Android.Graphics;

namespace CollectorQi.Views
{
    public partial class InventarioCaixaIncompletaPopUpB : PopupPage
    {
        private InventarioVO            _inventario { get; set; }
        private InventarioItemVO        _inventarioItemVO;

        public Action<InventarioItemVO> _actDeleteRow;
        public Action<InventarioItemVO, bool, string> _actRefreshPage;

        public InventarioCaixaIncompletaPopUpB(InventarioVO pInventario, InventarioItemVO pInventarioItem)        
        {
            try
            {
                InitializeComponent();
                
                _inventarioItemVO = pInventarioItem;
                _inventario     = pInventario;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task OnCloseCustom()
        {
            await PopupNavigation.Instance.PopAsync();
        }
        protected async override void OnAppearing()
        {
            edtItCodigo.Text           = _inventarioItemVO.CodItem.ToString();
            edtCodEstabelecimento.Text = _inventario.CodEstabel + " - " + _inventario.DescEstabel;
            edtCodDeposito.Text        = _inventario.CodDepos + " - " + _inventario.DescDepos;
            edtLote.Text               = _inventarioItemVO.Lote;

            AtualizaQtdAcumulada();

            if (String.IsNullOrEmpty(_inventarioItemVO.Lote))
            {
                FrameLote.IsVisible = false;
            }
        }

        private void AtualizaQtdAcumulada()
        {
            var result = InventarioItemCodigoBarrasDB.GetByInventarioItemKey(_inventarioItemVO.InventarioItemKey);

            if (result != null)
            {
                txtQuantidadeAcumulada.Text = result.Sum(x => x.Quantidade).ToString();
            }
        }
       
        private async void BtnLimpar_Clicked(object sender, EventArgs e)
        {
            var pageProgress = new ProgressBarPopUp("Carregando...");

            BtnLimpar.IsEnabled = false;
            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                var result = InventarioItemCodigoBarrasDB.DeleteByItemId(_inventarioItemVO.InventarioItemKey);

                edtCodigoBarras.Text = "";
                txtQuantidade.Text = "";
                txtQuantidadeAcumulada.Text = "";

                
                _inventarioItemVO.StatusIntegracao = eStatusInventarioItem.NaoIniciado;
                _actRefreshPage(_inventarioItemVO, false, null);


                /*
                // if (!String.IsNullOrEmpty(edtCodigoBarras.Text))
                // {
                    var inventarioBarra = new InventarioItemBarra()
                    {
                        IdInventario = _inventarioItemVO.InventarioId,
                        Lote         = _inventarioItemVO.Lote.Trim(),
                        Localizacao  = _inventarioItemVO.Localizacao.Trim(),
                        // CodItem = _inventarioItemVO.CodItem.Trim(),
                        CodDepos     = _inventarioItemVO.CodDepos.Trim(),
                        QuantidadeDigitada = 0,
                        CodEmp       = SecurityAuxiliar.GetCodEmpresa(),
                        Contagem     = _inventarioItemVO.Contagem,
                        CodEstabel   = _inventarioItemVO.CodEstabel,
                        CodigoBarras = _inventarioItemVO.CodItem.Trim()
                    };

                    var resultService = await ParametersLimparLeituraEtiquetaServiceB.SendInventarioAsync(inventarioBarra);

                    if (resultService != null && resultService.Retorno != null)
                    {
                        if (resultService.Retorno == "OK")
                        {
                            _inventarioItemVO.QuantidadeAcum = 0;
                            txtQuantidadeAcumulada.Text      = _inventarioItemVO.QuantidadeAcum.ToString();

                            txtQuantidade.Text   = string.Empty;
                            edtCodigoBarras.Text = string.Empty;

                            _actRefreshPage(_inventarioItemVO, false);

                            edtCodigoBarras.Focus();
                        }
                        else
                        {
                            txtQuantidade.Text   = string.Empty;
                            edtCodigoBarras.Text = string.Empty;

                            edtCodigoBarras.Focus();

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
              //  }
              //  else
              //  {
                   
                    /* var inventarioBarra = new InventarioItemBarra()
                    {
                        IdInventario = _inventarioItemVO.InventarioId,
                        Lote = _inventarioItemVO.Lote.Trim(),
                        Localizacao = _inventarioItemVO.Localizacao.Trim(),
                        // CodItem = _inventarioItemVO.CodItem.Trim(),
                        CodDepos = _inventarioItemVO.CodDepos.Trim(),
                        QuantidadeDigitada = 0,
                        CodEmp = SecurityAuxiliar.GetCodEmpresa(),
                        Contagem = _inventarioItemVO.Contagem,
                        CodEstabel = _inventarioItemVO.CodEstabel,
                        CodigoBarras = _inventarioItemVO.CodItem.Trim()
                    };

                    var resultService = await ParametersLimparLeituraEtiquetaServiceB.SendInventarioAsync(inventarioBarra);


                    if (resultService != null && resultService.Retorno != null)
                    {
                        if (resultService.Retorno == "OK")
                        {
                            _inventarioItemVO.QuantidadeAcum = 0;
                            txtQuantidadeAcumulada.Text = _inventarioItemVO.QuantidadeAcum.ToString();

                            txtQuantidade.Text = string.Empty;
                            edtCodigoBarras.Text = string.Empty;

                            _actRefreshPage(_inventarioItemVO);

                            edtCodigoBarras.Focus();
                        }
                        else
                        {
                            txtQuantidade.Text = string.Empty;
                            edtCodigoBarras.Text = string.Empty;

                            edtCodigoBarras.Focus();

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
                    }*/


                //  }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                BtnLimpar.IsEnabled = true;
                await pageProgress.OnClose();
            }
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
            if (!String.IsNullOrEmpty(edtCodigoBarras.Text))
            {
                await ClickButtonEfetivar(false, null);
            }
            else if (!String.IsNullOrEmpty(txtQuantidade.Text))
            {
                var result = await ClickButtonEfetivar(false, null);

               // if (result)
               // {
               //     _actEfetivaInventirio(true);
               // }
            }
            else
            {
                if (String.IsNullOrEmpty(txtQuantidade.Text) && String.IsNullOrEmpty(edtCodigoBarras.Text))
                {
                    _actRefreshPage(null, false, null);
                    OnBackButtonPressed();
                }
            }
        }

        private async Task<bool> ClickButtonEfetivar(bool blnCodBarras, DadosLeituraEtiquetaPAM convertPAM)
        {
            if (string.IsNullOrEmpty(txtQuantidade.Text) && !blnCodBarras)
            {
                await DisplayAlert("Erro!", "Informe a quantidade do produto disponivel para a contagem do inventário", "Cancelar");
                return false;
            }

            decimal decQuantidade = 0;

            if (!blnCodBarras)
            {
                decQuantidade = decimal.Parse(txtQuantidade.Text, ServiceCommon.ObterCulturaBrasil);

                /*
                if (string.IsNullOrEmpty(edtCodigoBarras.Text) && decQuantidade > 0)
                {
                    await DisplayAlert("Erro!", "Informe o código de barras", "Cancelar");
                    return;
                }
                else
                {*/
                if (decQuantidade == 0)
                {
                    textChangeZera = true;
                    edtCodigoBarras.Text = $"02;{_inventarioItemVO.CodItem.Trim()};1;1;1;0;1;1;1;1";
                }
            }
            else
            {
                if (convertPAM != null)
                {
                    txtQuantidade.Text = convertPAM.QtdUnitaria.ToString();
                    decQuantidade = convertPAM.QtdUnitaria;
                }
            }
            // }
            
            var pageProgress = new ProgressBarPopUp("Carregando...");
            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress, false);

                BtnEfetivar.IsEnabled = false;

                if (!string.IsNullOrEmpty(txtQuantidade.Text) || blnCodBarras)
                {
                    string strCodBarras = CleanInput(edtCodigoBarras.Text);

                    if (strCodBarras == null) strCodBarras = String.Empty;

                    var inventarioBarra = new InventarioItemBarra()
                    {
                        IdInventario = _inventarioItemVO.InventarioId,
                        Lote = _inventarioItemVO.Lote.Trim(),
                        Localizacao = _inventarioItemVO.Localizacao.Trim(),
                        CodItem = _inventarioItemVO.CodItem.Trim(),
                        CodDepos = _inventarioItemVO.__inventario__.CodDepos.Trim(),
                        QuantidadeDigitada = decQuantidade,
                        CodEmp = SecurityAuxiliar.GetCodEmpresa(),
                        Contagem = _inventarioItemVO.Contagem,
                        CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                        CodigoBarras = strCodBarras
                    };

                    if (string.IsNullOrEmpty(txtQuantidade.Text))
                    {
                        txtQuantidade.Text = "0";
                    }

                    _inventarioItemVO.Quantidade   = decimal.Parse(txtQuantidade.Text, ServiceCommon.ObterCulturaBrasil);
                    _inventarioItemVO.CodigoBarras = strCodBarras;

                    if (!String.IsNullOrEmpty(_inventarioItemVO.CodigoBarras))
                    {
                        _inventarioItemVO.CodigoBarras = _inventarioItemVO.CodigoBarras.Replace(";", "[");
                        inventarioBarra.CodigoBarras = inventarioBarra.CodigoBarras.Replace(";", "[");
                    }

                    decimal quantidadeAcumulada = 0;

                    var resultService = await ParametersLeituraEtiquetaServiceB.SendInventarioAsync(inventarioBarra, _inventarioItemVO, 0, this, quantidadeAcumulada, blnCodBarras, convertPAM);

                    if (resultService)
                    {
                        AtualizaQtdAcumulada();

                        _inventarioItemVO.StatusIntegracao = eStatusInventarioItem.IntegracaoCX;
                            
                        if (blnCodBarras)
                        {
                            txtQuantidade.Text = string.Empty;
                            edtCodigoBarras.Text = string.Empty;

                            _actRefreshPage(_inventarioItemVO, false,null);
                            //  edtCodigoBarras.Focus();
                            edtCodigoBarras.Focus();
                        }
                        else
                        {
                            txtQuantidade.Text   = string.Empty;
                            edtCodigoBarras.Text = string.Empty;

                            if (!String.IsNullOrEmpty(edtCodigoBarras.Text))
                            {
                                _actRefreshPage(_inventarioItemVO, false , null);
                                edtCodigoBarras.Focus();
                                OnBackButtonPressed();
                            }
                            else
                            {
                                await pageProgress.OnClose();
                                await this.OnCloseCustom();

                                _actRefreshPage(_inventarioItemVO, true, null);
                            }
                        }

                        return true;
                        
                    }
                    else
                    {
                        await DisplayAlert("Erro!", "Erro na efetivação do inventário", "Cancelar");
                        return false;
                    }
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
                return false;
            }
            finally
            {
                BtnEfetivar.IsEnabled = true;

                if (pageProgress != null && pageProgress.IsVisible)
                {
                    await pageProgress.OnClose();
                }
            }

            return false;
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
            catch (Exception e)
            {
                return String.Empty;
            }
        }

        private async void edtCodigoBarras_UnFocused(object sender, FocusEventArgs e)
        {
            await edtCodigoBarrasEtiqueta();
        }

        bool textChangeZera = false;
        private async Task edtCodigoBarrasEtiqueta()
        {
            try
            {
                if (!String.IsNullOrEmpty(edtCodigoBarras.Text) && !textChangeZera)
                {
                    var result = await ParametersEtiquetaPAM.ObterEtiqueta(edtCodigoBarras.Text);

                    if (result != null && result.Qtde > 0)
                    {
                        txtQuantidade.Text = result.Qtde.ToString();
                    }
                }

                textChangeZera = false;
            }
            catch (Exception ex) {
                throw ex;
            }
        }

        private async void SwtCxCompleta_OnChanged(object sender, ToggledEventArgs e)
        {
            try
            {
                await SecurityDB.AtualizarSecurityParametros(true);
            }
            catch { }
        }

        private async void edtCodigoBarras_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(e.OldTextValue) && e.NewTextValue.Length >= 10 && !textChangeZera)
                {

                    var convertPAM = LeituraEtiquetaInventarioPAM.ConvertePAM(e.NewTextValue, _inventarioItemVO.CodItem);

                    if (convertPAM != null)
                    {
                        var exists = InventarioItemCodigoBarrasDB.GetByCodigoBarras(_inventario.IdInventario, e.NewTextValue);

                        if (exists)
                        {
                            await DisplayAlert("ERRO!", "Etiqueta já efetuado a leitura.", "OK");

                            edtCodigoBarras.Text = "";
                            return;
                        }

                        if (convertPAM.CodItem != _inventarioItemVO.CodItem)
                        {
                            if (!_inventarioItemVO.CodItem.Contains(convertPAM.CodItem.Substring(0,13)))
                            {
                                await DisplayAlert("ERRO!", "Item não corresponde a etiqueta informada.", "OK");

                                edtCodigoBarras.Text = "";
                                return;
                            }
                        }
                    }

                    await ClickButtonEfetivar(true, convertPAM); // Efetivação
                }

                textChangeZera = false;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", "Erro leitura etiqueta " + ex.Message, "OK");
                edtCodigoBarras.Text = String.Empty;
            }
        }
    }
}