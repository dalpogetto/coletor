using CollectorQi.Models.ESCL018;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Services.ESCL000;
using CollectorQi.Services.ESCL018;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms;

namespace CollectorQi.Views
{
    public partial class InventarioPrintReparo : PopupPage
    {
        public InventarioPrintReparo()        
        {    
            InitializeComponent();

            txtCodEstabelecimento.Text = SecurityAuxiliar.GetCodEstabel();
            txtDescEstabelecimento.Text = SecurityAuxiliar.GetDescEstabel();
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

                if (String.IsNullOrEmpty(txtCodBarra.Text))
                {
                    if (String.IsNullOrEmpty(txtCodEstabelecimento.Text))
                    {
                        await DisplayAlert("Erro!", "Informe o estabelecimento de impressão!", "Cancelar");
                        txtCodEstabelecimento.Focus();
                        return;
                    }

                    if (String.IsNullOrEmpty(txtCodFilial.Text))
                    {
                        await DisplayAlert("Erro!", "Informe a filial para impressão!", "Cancelar");
                        txtCodFilial.Focus();
                        return;
                    }

                    if (String.IsNullOrEmpty(txtItCodigo.Text))
                    {
                        await DisplayAlert("Erro!", "Informe o item para impressão!", "Cancelar");
                        txtItCodigo.Focus();
                        return;
                    }

                    if (String.IsNullOrEmpty(txtNumRR.Text))
                    {
                        await DisplayAlert("Erro!", "Informe o número do reparo para impressão!", "Cancelar");
                        txtNumRR.Focus();
                        return;
                    }

                    if (String.IsNullOrEmpty(txtDigito.Text))
                    {
                        await DisplayAlert("Erro!", "Informe o digito para impressão!", "Cancelar");
                        txtDigito.Focus();
                        return;
                    }
                }

                var impressaoReparo = new ImpressaoReparo()
                {
                    CodigoBarras = txtCodBarra.Text,
                    CodEstabel = txtCodEstabelecimento.Text,
                    CodFilial = txtCodFilial.Text,
                    CodItem = txtItCodigo.Text,
                    NumRR = int.Parse(txtNumRR.Text),
                    Digito = int.Parse(txtDigito.Text)
                };

                var result = await ParametersImprimirEtiquetaService.SendImpressaoAsync(null, null, impressaoReparo, 3);

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

        private void BtnScan_Clicked(object sender, EventArgs e)
        {

        }

        private void BtnLimpar_Clicked(object sender, EventArgs e)
        {
            txtCodEstabelecimento.Text = String.Empty;
            txtDescEstabelecimento.Text = String.Empty;
            txtCodFilial.Text = String.Empty;
            txtItCodigo.Text = String.Empty;
            txtDescItem.Text = String.Empty;
            txtNumRR.Text = String.Empty;
            txtDigito.Text = String.Empty;
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

        private async void BtnBuscaFilial_Clicked(object sender, EventArgs e)
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
                    txtCodFilial.Text = action.Remove(action.IndexOf('(')).Trim();
                    txtDescFilial.Text = action.Remove(0, action.IndexOf('(')).Replace("(", "").Replace(")", "").Trim();
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

        private void txtCodFilial_Unfocused(object sender, FocusEventArgs e)
        {
            var estabelec = EstabelecDB.GetEstabelec(txtCodFilial.Text);

            if (estabelec != null)
            {
                txtDescFilial.Text = estabelec.Nome;
            }
            else
            {
                txtDescFilial.Text = String.Empty;
            }
        }

        private async void txtItCodigo_Unfocused(object sender, FocusEventArgs e)
        {
            var item = await Cadastros.ObterItem(txtItCodigo.Text);

            if (item != null)
            {
                txtDescItem.Text = item;
            }
            else
            {
                txtDescItem.Text = String.Empty;
            }
        }
    }
}