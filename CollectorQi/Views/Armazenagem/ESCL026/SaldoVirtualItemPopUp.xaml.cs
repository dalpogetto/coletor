using CollectorQi.Resources;
using CollectorQi.Services.ESCL027;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using static CollectorQi.Services.ESCL027.SaldoVirtual;
//using Android.Graphics;

namespace CollectorQi.Views
{
    public partial class SaldoVirtualItemPopUp : PopupPage
    {
        public Action<string, string, string> _actRefresh;
        public string _codDepos { get; set; }
        public string _codLocaliz { get; set; }
        public string _codItem { get; set; }
        public string _saldo { get; set; }
        public SaldoVirtualItemPopUp(string pCodDepos, string pCodLocaliz, string pCodItem, string pSaldo /* int pInventarioId, InventarioItemVO pInventarioItem*/ )        
        {
            try
            {
                InitializeComponent();

                _codDepos   = pCodDepos;
                _codLocaliz = pCodLocaliz;
                _codItem    = pCodItem;
                _saldo      = pSaldo;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected async override void OnAppearing()
        {
            edtCodDepos.Text   = _codDepos;
            edtCodLocaliz.Text = _codLocaliz;
            edtCodItem.Text    = _codItem;
            txtQuantidade.Text = _saldo.ToString();
          
            await Task.Run(async () =>
            { 
                await Task.Delay(100);
                Device.BeginInvokeOnMainThread(async () =>
                {
                  
                   txtQuantidade.Focus();
                   txtQuantidade.CursorPosition = txtQuantidade.Text.Length;
                });
            });
        }
       
        void BtnLimpar_Clicked(object sender, EventArgs e)
        {
            txtQuantidade.Text = string.Empty;
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

            //entry.Text = (decimal.Parse(entry.Text.Replace(",", ".")) + value).ToString();

            decimal qtd = decimal.Parse(entry.Text) + value;

            entry.Text = qtd.ToString();
        }
        async void OnClick_Efetivar(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtQuantidade.Text))
            {
                await DisplayAlert("Erro!", "Informe a quantidade do produto disponivel para a contagem do inventário", "Cancelar");
                return;
            }

            decimal decQuantidade = decimal.Parse(txtQuantidade.Text);

            var result = await DisplayAlert("Confirmação!", $"Deseja concluír a digitação com a quantidade de {txtQuantidade.Text} produto?", "Sim", "Não");

            var pageProgress = new ProgressBarPopUp("Carregando...");
            try
            {
                BtnEfetivar.IsEnabled = false;
                if (result.ToString() == "True")
                {
                    if (!string.IsNullOrEmpty(txtQuantidade.Text))
                    {
                        var saldoVirtual = new AtualizarSaldoVirtualSend()
                        {
                            CodEstabel   = SecurityAuxiliar.GetCodEstabel(),
                            CodigoItem   = _codItem,
                            CodLocaliza  = _codLocaliz,
                            CodDepos     = _codDepos,
                            Lote         = String.Empty,
                            SaldoInfo    = decQuantidade
                        };

                        var resultService = await SaldoVirtual.AtualizarSaldoItemLocaliz(saldoVirtual);

                        if (resultService != null && resultService.Retorno != null)
                        {
                            if (resultService.Retorno == "OK")
                            {
                                _actRefresh(_codItem, _codLocaliz, decQuantidade.ToString());

                                await pageProgress.OnClose();
                                OnBackButtonPressed();
                            }
                            else
                            {
                                await DisplayAlert("Erro!", "Erro na efetivação", "Cancelar");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Erro!", "Erro na efetivação", "Cancelar");
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
    }
}