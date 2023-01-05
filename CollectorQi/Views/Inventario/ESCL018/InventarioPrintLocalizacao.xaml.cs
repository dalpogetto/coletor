using CollectorQi.Models.ESCL018;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Services.ESCL018;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms;

namespace CollectorQi.Views
{
    public partial class InventarioPrintLocalizacao : PopupPage
    {
        private string _codDepos { get; set; }
        private string _codLocaliz { get; set; }
        public InventarioPrintLocalizacao(string pCodDepos = null, string pCodLocaliz = null)        
        {    
            InitializeComponent();

            txtCodEstabelecimento.Text = SecurityAuxiliar.GetCodEstabel();
            txtDescEstabelecimento.Text = SecurityAuxiliar.GetDescEstabel();

            _codDepos = pCodDepos;
            _codLocaliz = pCodLocaliz;

            if (pCodDepos != null)
            {
                txtCodDeposito.Text = pCodDepos;
            }

            if (pCodLocaliz != null)
            {
                txtLocalizacao.Text = pCodLocaliz;
            }
        }

        protected override bool OnBackButtonPressed()
        {            
            PopupNavigation.Instance.PopAsync();
            return true;
        }

        async void OnClick_Imprimir(object sender, EventArgs e)
        {
            
            BtnImprimir.IsEnabled = false;

            var pageProgress = new ProgressBarPopUp("Imprimindo Etiqueta, aguarde...");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                if (String.IsNullOrEmpty(txtCodEstabelecimento.Text))
                {
                    await DisplayAlert("Erro!", "Informe o estabelecimento de impressão!", "Cancelar");
                    txtCodEstabelecimento.Focus();
                    return;
                }

                if (String.IsNullOrEmpty(txtCodDeposito.Text))
                {
                    await DisplayAlert("Erro!", "Informe o depósito para impressão!", "Cancelar");
                    txtCodDeposito.Focus();
                    return;
                }

                if (String.IsNullOrEmpty(txtLocalizacao.Text))
                {
                    await DisplayAlert("Erro!", "Informe localização para impressão!", "Cancelar");
                    txtLocalizacao.Focus();
                    return;
                }

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


                var result = await ParametersImprimirEtiquetaService.SendImpressaoAsync(null, impressaoLocalizacao, null, 2);

                await pageProgress.OnClose();

                if (result != null && result.Retorno == "OK")
                {
                    OnBackButtonPressed();
                }
                else
                {
                    if (result.Resultparam != null && result.Resultparam.Count > 0)
                    {
                        await DisplayAlert("Erro!", result.Resultparam[0].ErrorDescription, "Cancelar");
                    }
                    else
                    {
                        await DisplayAlert("Erro!", "Erro ao imprimir etiqueta", "Cancelar");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                BtnImprimir.IsEnabled = true;
                await pageProgress.OnClose();
            }
            
        }

        private void BtnVoltar_Clicked(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }


        private void BtnLimpar_Clicked(object sender, EventArgs e)
        {
            txtCodEstabelecimento.Text = String.Empty;
            txtDescEstabelecimento.Text = String.Empty;
            txtCodDeposito.Text = String.Empty;
            txtLocalizacao.Text = String.Empty;
            txtRuaInicio.Text = String.Empty;
            txtRuaFim.Text = "ZZZ";
            txtAreaInicio.Text = String.Empty;
            txtAreaFim.Text = "ZZZZ";
        }

        private async void BtnBuscaEstab_Clicked(object sender, EventArgs e)
        {
            var estabelec = await EstabelecDB.GetEstabelec();
            if (estabelec != null)
            {
                string[] arrayDep = new string[estabelec.Count];
                for (int i = 0; i < estabelec.Count; i++)
                {
                    arrayDep[i] = estabelec[i].CodEstabel + " (" + estabelec[i].Nome.Trim() + ")";
                }

                var action = await DisplayActionSheet("Escolha o Estabelecimento?", "Cancelar", null, arrayDep);

                if (action != "Cancelar" && action != null)
                {
                    txtCodEstabelecimento.Text = action.Remove(action.IndexOf('(')).Trim();
                    txtDescEstabelecimento.Text = action.Remove(0, action.IndexOf('(')).Replace("(", "").Replace(")", "").Trim();
                }
            }
        }
        private async void BtnBuscaDeposito_Clicked(object sender, EventArgs e)
        {
            var estabelec = await EstabelecDB.GetEstabelec();
            if (estabelec != null)
            {
                string[] arrayDep = new string[estabelec.Count];
                for (int i = 0; i < estabelec.Count; i++)
                {
                    arrayDep[i] = estabelec[i].CodEstabel + " (" + estabelec[i].Nome.Trim() + ")";
                }

                var action = await DisplayActionSheet("Escolha o Estabelecimento?", "Cancelar", null, arrayDep);

                if (action != "Cancelar" && action != null)
                {
                    txtCodEstabelecimento.Text = action.Remove(action.IndexOf('(')).Trim();
                    txtDescEstabelecimento.Text = action.Remove(0, action.IndexOf('(')).Replace("(", "").Replace(")", "").Trim();
                }
            }
        }

        private async void BtnBuscaItem_Clicked(object sender, EventArgs e)
        {
            var estabelec = await EstabelecDB.GetEstabelec();
            if (estabelec != null)
            {
                string[] arrayDep = new string[estabelec.Count];
                for (int i = 0; i < estabelec.Count; i++)
                {
                    arrayDep[i] = estabelec[i].CodEstabel + " (" + estabelec[i].Nome.Trim() + ")";
                }

                var action = await DisplayActionSheet("Escolha o Estabelecimento?", "Cancelar", null, arrayDep);

                if (action != "Cancelar" && action != null)
                {
                    txtCodEstabelecimento.Text = action.Remove(action.IndexOf('(')).Trim();
                    txtDescEstabelecimento.Text = action.Remove(0, action.IndexOf('(')).Replace("(", "").Replace(")", "").Trim();
                }
            }
        }
        private void txtCodEstabelecimento_Unfocused(object sender, FocusEventArgs e)
        {
            var estabelec = EstabelecDB.GetEstabelec(txtCodEstabelecimento.Text);

            if (estabelec != null)
            {
                txtDescEstabelecimento.Text = estabelec.Nome;
            }
            else
            {
                txtDescEstabelecimento.Text = String.Empty;
            }
        }

        private void txtLocalizacao_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.OldTextValue) && e.NewTextValue.Length > 5)
            {
                txtLocalizacao.Text = txtLocalizacao.Text.Replace("10;", "").Trim();
            }
        }


        private void txtDeposito_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                (sender as Entry).Text = e.NewTextValue.ToUpperInvariant();
            }
            catch
            {

            }
        }
    }
}