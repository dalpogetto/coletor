using AutoMapper;
using CollectorQi.Models.ESCL018;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Services.ESCL000;
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
    public partial class InventarioPrintItem : PopupPage
    { 
        private string _codDepos { get; set; }
        private string _itCodigo { get; set; }
        
        public InventarioPrintItem(string pCodDepos = null, string pItCodigo = null)        
        {    
            InitializeComponent();

            txtCodEstabelecimento.Text = SecurityAuxiliar.GetCodEstabel();
            txtDescEstabelecimento.Text = SecurityAuxiliar.GetDescEstabel();

            if (pCodDepos != null)
            {
                txtCodDeposito.Text = pCodDepos;
            }

            if (pItCodigo != null)
            {
                txtItCodigo.Text = pItCodigo;
            }

            _codDepos = pCodDepos;
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

                if (String.IsNullOrEmpty(txtItCodigo.Text))
                {
                    await DisplayAlert("Erro!", "Informe o item para impressão!", "Cancelar");
                    txtItCodigo.Focus();
                    return;
                }

                if (String.IsNullOrEmpty(txtEtiqItem.Text))
                {
                    await DisplayAlert("Erro!", "Informe a etiqueta item para impressão!", "Cancelar");
                    txtEtiqItem.Focus();
                    return;
                }

                if (String.IsNullOrEmpty(txtQtdeEtiqueta.Text))
                {
                    await DisplayAlert("Erro!", "Informe a quantidade de etiqueta para impressão!", "Cancelar");
                    txtQtdeEtiqueta.Focus();
                    return;
                }

                var impressaoItem = new ImpressaoItem()
                {
                    CodEstabel = txtCodEstabelecimento.Text.Trim(),
                    CodDeposito = txtCodDeposito.Text.Trim(),
                    CodItem = txtItCodigo.Text.Trim(),
                    Quantidade = int.Parse(txtEtiqItem.Text),
                    QtdeEtiqueta = int.Parse(txtQtdeEtiqueta.Text),
                };

                var result = await ParametersImprimirEtiquetaService.SendImpressaoAsync(impressaoItem, null, null,1);

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
            txtItCodigo.Text = String.Empty;
            //txtDescItem.Text = String.Empty;
            txtEtiqItem.Text = String.Empty;
            txtQtdeEtiqueta.Text = String.Empty;
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
        private void BtnEtiqItemSum1_Clicked(object sender, EventArgs e)
        {
            SumButton(txtEtiqItem, 1);
        }
        private void BtnEtiqItemSum10_Clicked(object sender, EventArgs e)
        {
            SumButton(txtEtiqItem, 10);
        }
        private void BtnEtiqItemSum100_Clicked(object sender, EventArgs e)
        {
            SumButton(txtEtiqItem, 100);
        }

        private void BtnQtdeEtiquetaSum1_Clicked(object sender, EventArgs e)
        {
            SumButton(txtQtdeEtiqueta, 1);
        }
        private void BtnQtdeEtiquetaSum10_Clicked(object sender, EventArgs e)
        {
            SumButton(txtQtdeEtiqueta, 10);
        }
        private void BtnQtdeEtiquetaSum100_Clicked(object sender, EventArgs e)
        {
            SumButton(txtQtdeEtiqueta, 100);
        }

        private void SumButton(CustomEntryText entry, int value)
        {
            if (String.IsNullOrEmpty(entry.Text)) { entry.Text = "0"; }

            entry.Text = (int.Parse(entry.Text) + value).ToString();
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

        private async void txtItCodigo_Unfocused(object sender, FocusEventArgs e)
        {
            var item = await Cadastros.ObterItem(txtItCodigo.Text);

            if (item != null)
            {
              //  txtDescItem.Text = item;
            }
            else
            {
                //txtDescItem.Text = String.Empty;
            }
        }
    }
}