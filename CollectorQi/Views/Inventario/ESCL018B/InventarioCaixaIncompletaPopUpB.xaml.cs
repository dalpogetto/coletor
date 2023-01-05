using AutoMapper;
using CollectorQi.Models.ESCL018;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Resources.DataBaseHelper.ESCL018;
using CollectorQi.Services.ESCL018;
using CollectorQi.Services.ESCL018B;
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
    public partial class InventarioCaixaIncompletaPopUpB : PopupPage
    {
        private int                     _inventarioId { get; set; }
        private InventarioItemVO        _inventarioItemVO;

        public Action<InventarioItemVO> _actDeleteRow;
        public Action<InventarioItemVO> _actRefreshPage;

        public InventarioCaixaIncompletaPopUpB(int pInventarioId, InventarioItemVO pInventarioItem)        
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
            var inventario = await InventarioDB.GetInventario(_inventarioId);

            edtItCodigo.Text           = _inventarioItemVO.CodItem.ToString();
            edtCodEstabelecimento.Text = _inventarioItemVO.CodEstabel + " - " + inventario.DescEstabel;
            edtCodDeposito.Text        = _inventarioItemVO.CodDepos + " - " + inventario.DescDepos;
            //edtCodigoBarras.Text       = _inventarioItemVO.CodigoBarras;
            edtLote.Text               = _inventarioItemVO.Lote;

           // if (_inventarioItemVO.Quantidade > 0)
           // {
           //     txtQuantidade.Text = _inventarioItemVO.Quantidade.ToString();
           // }

            if (_inventarioItemVO.QuantidadeAcum > 0)
            {
                txtQuantidadeAcumulada.Text = _inventarioItemVO.QuantidadeAcum.ToString();
            }

            if (String.IsNullOrEmpty(_inventarioItemVO.Lote))
            {
                FrameLote.IsVisible = false;
            }

            var security = await SecurityDB.GetSecurityAsync();

            if (security != null && !security.CxCompleta)
            {
                SwtCxCompleta.On = false;
            }
            {
                SwtCxCompleta.On = true;
            }

            await Task.Run(async () =>
            { 
                await Task.Delay(100);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (SwtCxCompleta.On)
                    {
                        edtCodigoBarras.Focus();
                    }
                    else
                    {
                        txtQuantidade.Focus();
                    }

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

        private void SumButton(CustomEntry entry, int value)
        {
            if (String.IsNullOrEmpty(entry.Text)) { entry.Text = "0"; }

            entry.Text = (int.Parse(entry.Text) + value).ToString();
        }

        async void OnClick_Efetivar(object sender, EventArgs e)
        {
            await ClickButtonEfetivar(false);
        }

        private async Task ClickButtonEfetivar(bool blnCodBarras)
        {

            if (String.IsNullOrEmpty(txtQuantidade.Text) && String.IsNullOrEmpty(edtCodigoBarras.Text) && blnCodBarras == false)
            {
                _actRefreshPage(null);
                OnBackButtonPressed();
            }
            else
            {

                if (string.IsNullOrEmpty(txtQuantidade.Text))
                {
                    await DisplayAlert("Erro!", "Informe a quantidade do produto disponivel para a contagem do inventário", "Cancelar");
                    return;
                }

                int decQuantidade = int.Parse(txtQuantidade.Text);

                if (string.IsNullOrEmpty(edtCodigoBarras.Text) && decQuantidade > 0)
                {
                    await DisplayAlert("Erro!", "Informe o código de barras", "Cancelar");
                    return;
                }
                else
                {
                    if (decQuantidade == 0)
                    {

                        textChangeZera = true;
                        edtCodigoBarras.Text = $"02;{_inventarioItemVO.CodItem.Trim()};1;1;1;0;1;1;1;1";
                    }
                }

                var pageProgress = new ProgressBarPopUp("Carregando...");
                try
                {
                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                    BtnEfetivar.IsEnabled = false;

                    if (!string.IsNullOrEmpty(txtQuantidade.Text))
                    {

                        var inventarioBarra = new InventarioItemBarra()
                        {
                            IdInventario = _inventarioItemVO.InventarioId,
                            Lote = _inventarioItemVO.Lote.Trim(),
                            Localizacao = _inventarioItemVO.Localizacao.Trim(),
                            CodItem = _inventarioItemVO.CodItem.Trim(),
                            CodDepos = _inventarioItemVO.CodDepos.Trim(),
                            QuantidadeDigitada = decQuantidade,
                            CodEmp = SecurityAuxiliar.GetCodEmpresa(),
                            Contagem = _inventarioItemVO.Contagem,
                            CodEstabel = _inventarioItemVO.CodEstabel,
                            CodigoBarras = CleanInput(edtCodigoBarras.Text.Trim())
                        };

                        _inventarioItemVO.Quantidade = int.Parse(txtQuantidade.Text);
                        _inventarioItemVO.CodigoBarras = CleanInput(edtCodigoBarras.Text);
                        _inventarioItemVO.CodigoBarras = _inventarioItemVO.CodigoBarras.Replace(";", "[");
                        inventarioBarra.CodigoBarras = inventarioBarra.CodigoBarras.Replace(";", "[");

                        decimal quantidadeAcumulada = 0;

                        if (txtQuantidadeAcumulada.Text != null)
                        {
                            quantidadeAcumulada += int.Parse(txtQuantidadeAcumulada.Text);
                        }

                        if (txtQuantidade.Text != null)
                        {
                            quantidadeAcumulada += int.Parse(txtQuantidade.Text);
                        }

                        var resultService = await ParametersLeituraEtiquetaServiceB.SendInventarioAsync(inventarioBarra, _inventarioItemVO, 0, this, quantidadeAcumulada);

                        if (resultService != null && resultService.Retorno != null)
                        {
                            if (resultService.Retorno == "OK")
                            {
                                _inventarioItemVO.QuantidadeAcum = quantidadeAcumulada;
                                txtQuantidadeAcumulada.Text = _inventarioItemVO.QuantidadeAcum.ToString();

                                if (blnCodBarras)
                                {
                                    txtQuantidade.Text = string.Empty;
                                    edtCodigoBarras.Text = string.Empty;

                                    _actRefreshPage(_inventarioItemVO);
                                  //  edtCodigoBarras.Focus();
                                }
                                else
                                {
                                    txtQuantidade.Text   = string.Empty;
                                    edtCodigoBarras.Text = string.Empty;

                                    _actDeleteRow(_inventarioItemVO);
                                    OnBackButtonPressed();
                                }

                                edtCodigoBarras.Focus();
                            }
                            else if (resultService.Retorno == "IntegracaoBatch")
                            {
                                await DisplayAlert("Atenção!", "Erro de conexão com ERP, a atualização do item será integrado de forma Offline", "OK");

                                _inventarioItemVO.StatusIntegracao = eStatusInventarioItem.ErroIntegracao;
                                _actRefreshPage(_inventarioItemVO);

                                await pageProgress.OnClose();
                                OnBackButtonPressed();
                            }
                            else if (resultService.Retorno == "IntegracaoBatchErrorLeituraEtiqueta")
                            {
                                txtQuantidade.Text = string.Empty;
                                edtCodigoBarras.Text = string.Empty;

                                edtCodigoBarras.Focus();

                                await DisplayAlert("Erro!", $"{resultService.Localizacao}/{resultService.Item} está pendente de efetivação! Atualize antes de uma nova leitura!", "OK");
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

                   // if (pageProgress != null && pageProgress.IsVisible)
                   // {
                        await pageProgress.OnClose();
                   // }
                }
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
                    if (SwtCxCompleta.On)
                    {
                        var result = await ParametersEtiquetaPAM.ObterEtiqueta(edtCodigoBarras.Text);

                        if (result != null && result.Qtde > 0)
                        {
                            txtQuantidade.Text = result.Qtde.ToString();
                        }
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
                await SecurityDB.AtualizarSecurityParametros(SwtCxCompleta.On);
            }
            catch { }
        }

        private async void edtCodigoBarras_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {
                if (String.IsNullOrEmpty(e.OldTextValue) && e.NewTextValue.Length >= 10 && !textChangeZera)
                {
                    var pageProgress = new ProgressBarPopUp("Carregando...");

                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                    await edtCodigoBarrasEtiqueta();

                    await pageProgress.OnClose();

                    if (txtQuantidade.Text != null && txtQuantidade.Text.Length > 0 && txtQuantidade.Text != "0")
                    {
                        await ClickButtonEfetivar(true);
                    }
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