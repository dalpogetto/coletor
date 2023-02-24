using CollectorQi.Models.ESCL018;
using CollectorQi.Models.ESCL029;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Services.ESCL000;
using CollectorQi.Services.ESCL018;
using CollectorQi.Services.ESCL029;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CollectorQi.Views
{
    public partial class InventarioPrintReparo : PopupPage
    {
        public InventarioPrintReparo()        
        {    
            InitializeComponent();

         //   txtCodEstabelecimento.Text = SecurityAuxiliar.GetCodEstabel();
         //   txtDescEstabelecimento.Text = SecurityAuxiliar.GetDescEstabel();
        }

        protected override bool OnBackButtonPressed()
        {            
            PopupNavigation.Instance.PopAsync();
            return true;
        }
        protected async override void OnAppearing()
        {

            await Task.Run(async () =>
            {
                await Task.Delay(400);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    edtScan.Focus();
                });
            });
        }

        async void OnClick_Imprimir(object sender, EventArgs e)
        {

            BtnImprimir.IsEnabled = false;

            var pageProgress = new ProgressBarPopUp("Imprimindo Etiqueta, aguarde...");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                if (String.IsNullOrEmpty(edtFilial.Text))
                {
                   //
                   //if (String.IsNullOrEmpty(txtCodEstabelecimento.Text))
                   //{
                   //    await DisplayAlert("Erro!", "Informe o estabelecimento de impressão!", "Cancelar");
                   //    txtCodEstabelecimento.Focus();
                   //    return;
                   //}
                   //
                    if (String.IsNullOrEmpty(edtFilial.Text))
                    {
                        await DisplayAlert("Erro!", "Informe a filial para impressão!", "Cancelar");
                        edtFilial.Focus();
                        return;
                    }

                //   if (String.IsNullOrEmpty(edtItem.Text))
                //   {
                //       await DisplayAlert("Erro!", "Informe o item para impressão!", "Cancelar");
                //       edtItem.Focus();
                //       return;
                //   }

                    if (String.IsNullOrEmpty(edtRR.Text))
                    {
                        await DisplayAlert("Erro!", "Informe o número do reparo para impressão!", "Cancelar");
                        edtRR.Focus();
                        return;
                    }

                    if (String.IsNullOrEmpty(edtDigito.Text))
                    {
                        await DisplayAlert("Erro!", "Informe o digito para impressão!", "Cancelar");
                        edtDigito.Focus();
                        return;
                    }
                }

                var impressaoReparo = new ImpressaoReparo()
                {
                    CodigoBarras = "",
                    //CodigoBarras = edtScan.Text,
                    //CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                    CodEstabel        = edtCodEstab.Text,
                    CodEstabelTecnico = SecurityAuxiliar.GetCodEstabel(),
                    //CodEstabel = 
                    //CodEstabel = edtS
                    CodFilial = edtFilial.Text,
                    CodItem = edtItem.Text,
                    NumRR = edtRR.Text.Replace(",","."),
                    Digito = int.Parse(edtDigito.Text)
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

        async void Leitura(ParametrosMovimentoReparo parametrosMovimentoReparo)
        {
            var pageProgress = new ProgressBarPopUp("Carregando Reparo, aguarde...");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                if (String.IsNullOrEmpty(parametrosMovimentoReparo.CodFilial))
                    parametrosMovimentoReparo.CodFilial = String.Empty;

                if (String.IsNullOrEmpty(parametrosMovimentoReparo.NumRR))
                    parametrosMovimentoReparo.NumRR = String.Empty;
                else
                    parametrosMovimentoReparo.NumRR = parametrosMovimentoReparo.NumRR.Replace(".", ",");

                if (String.IsNullOrEmpty(parametrosMovimentoReparo.Digito))
                    parametrosMovimentoReparo.Digito = String.Empty;

                // Conforme alinhado com Kawano, utilizar no código de barras o codigo do estabelecimento local
                // Futuramente alterar API para corrigir atualização
                /*
                 * if (String.IsNullOrEmpty(parametrosMovimentoReparo.CodBarras))
                    parametrosMovimentoReparo.CodBarras = String.Empty;
                else
                    parametrosMovimentoReparo.CodBarras = SecurityAuxiliar.GetCodEstabel() + parametrosMovimentoReparo.CodBarras.Substring(3);
                */

                var result = await LeituraEtiquetaArmazenagemService.SendLeituraEtiquetaAsync(parametrosMovimentoReparo);

                if (result != null && result.ParamReparo != null && result.ParamReparo.ParamLeitura != null && result.Retorno == "OK")
                {
                     edtItem.Text = result.ParamReparo.ParamLeitura.CodItem;
                    edtDescricao.Text = result.ParamReparo.ParamLeitura.DescItem;
                    edtLocalizacao.Text = result.ParamReparo.ParamLeitura.Localiza;
                    //edtMsg.Text       = result.ParamReparo.ParamLeitura.Mensagem;
                    edtFilial.Text = result.ParamReparo.ParamLeitura.CodFilial;
                    edtRR.Text = result.ParamReparo.ParamLeitura.NumRR.ToString();
                    edtDigito.Text = result.ParamReparo.ParamLeitura.Digito.ToString();

                    edtCodEstab.Text = result.ParamReparo.ParamLeitura.CodEstabel.ToString();

                    if (!SwtCodigoBarras.On)
                    {
                        frame2.IsVisible = true;
                        BtnLeituraDigitacao.IsVisible = false;
                        frameBuscaReparo.IsVisible = false;
                    }

                    if (String.IsNullOrEmpty(edtLocalizacao.Text))
                    {
                        frameLocalizacao.IsVisible = false;
                    }
                    else
                    {
                        frameLocalizacao.IsVisible = true;
                    }

                    edtScan.IsReadOnly = true;
                    edtRR.IsReadOnly = true;
                    edtFilial.IsReadOnly = true;
                    edtDigito.IsReadOnly = true;
                    BtnLeituraDigitacao.IsEnabled = false;

                    // Click Efetivar
                    OnClick_Imprimir(BtnImprimir, new EventArgs());

                }
                else
                {
                    if (result != null && result.Resultparam != null && result.Resultparam.Count > 0)
                    {
                        await DisplayAlert("Erro", result.Resultparam[0].ErrorHelp, "OK");

                        edtScan.Text = String.Empty;
                    }
                    else
                    {
                        await DisplayAlert("Erro", "Erro na confirmação do reparo", "OK");
                        edtScan.Text = String.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", ex.Message, "OK");
            }
            finally
            {
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
            //txtCodEstabelecimento.Text = String.Empty;
            //txtDescEstabelecimento.Text = String.Empty;
            edtFilial.Text = String.Empty;
            edtItem.Text = String.Empty;
            edtDescricao.Text = String.Empty;
            edtRR.Text = String.Empty;
            edtDigito.Text = String.Empty;
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
                    //txtCodEstabelecimento.Text = action.Remove(action.IndexOf('(')).Trim();
                    //txtDescEstabelecimento.Text = action.Remove(0, action.IndexOf('(')).Replace("(", "").Replace(")", "").Trim();
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
                   //txtCodEstabelecimento.Text = action.Remove(action.IndexOf('(')).Trim();
                   //txtDescEstabelecimento.Text = action.Remove(0, action.IndexOf('(')).Replace("(", "").Replace(")", "").Trim();
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
                    //txtCodEstabelecimento.Text = action.Remove(action.IndexOf('(')).Trim();
                    //txtDescEstabelecimento.Text = action.Remove(0, action.IndexOf('(')).Replace("(", "").Replace(")", "").Trim();
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
                    edtFilial.Text = action.Remove(action.IndexOf('(')).Trim();
                    //txtDescFilial.Text = action.Remove(0, action.IndexOf('(')).Replace("(", "").Replace(")", "").Trim();
                }
            }
        }

        private void edtFilial_Unfocused(object sender, FocusEventArgs e)
        {
            var estabelec = EstabelecDB.GetEstabelec(edtFilial.Text);

            if (estabelec != null)
            {
               // txtDescFilial.Text = estabelec.Nome;
            }
            else
            {
                //txtDescFilial.Text = String.Empty;
            }
        }

        private async void edtItem_Unfocused(object sender, FocusEventArgs e)
        {
            var item = await Cadastros.ObterItem(edtItem.Text);

            if (item != null)
            {
                edtDescricao.Text = item;
            }
            else
            {
                edtDescricao.Text = String.Empty;
            }
        }

          private void ChangeSwt()
        {
            if (SwtCodigoBarras.On)
            {
                SwtCodigoBarras.Text          = "Digita Reparo";
                frame1.IsVisible              = false;
                BtnLeituraDigitacao.IsVisible = true;
                frameBuscaReparo.IsVisible    = true;

               

                    edtFilial.IsReadOnly          = false;
                    edtRR.IsReadOnly              = false;
                    edtDigito.IsReadOnly          = false;
            }
            else
            {
                SwtCodigoBarras.Text          = "Código de Barras";
                frame1.IsVisible              = true;
                BtnLeituraDigitacao.IsVisible = false;
                frameBuscaReparo.IsVisible    = false;
                edtFilial.IsReadOnly          = true;
                edtRR.IsReadOnly              = true;
                edtDigito.IsReadOnly          = true;

                /* if (String.IsNullOrEmpty(_rowId))
                {
                    edtScan.IsReadOnly = false;
                }*/
            }
        }


        private void SwtCodigoBarras_OnChanged(object sender, ToggledEventArgs e)
        {
            ChangeSwt();
        }

        private void edtScan_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.OldTextValue) && e.NewTextValue.Length > 6)
            {

                var parametrosMovimentoReparo = new ParametrosMovimentoReparo()
                {
                    // CodEstabel = "",
                    /* CodFilial  = edtFilial.Text,
                     NumRR      = edtRR.Text,
                     Digito     = edtDigito.Text,   */
                    Opcao = 1,
                    CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                    CodBarras = edtScan.Text
                };

                Leitura(parametrosMovimentoReparo); 
            } 
        }

        async void BtnLeituraDigitacao_Clicked(object sender, System.EventArgs e)
        {
            try
            {
                BtnLeituraDigitacao.IsEnabled = false;

                if (string.IsNullOrWhiteSpace(edtFilial.Text) && string.IsNullOrWhiteSpace(edtRR.Text) && string.IsNullOrWhiteSpace(edtDigito.Text))
                    await DisplayAlert("Erro", "Digite uma Filial, RR e Dígito !", "OK");
                else
                {
                    
                    var parametrosMovimentoReparo = new ParametrosMovimentoReparo()
                    {
                        CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                        CodFilial = edtFilial.Text,
                        NumRR = edtRR.Text,
                        Digito = edtDigito.Text,
                  //      Opcao = _opcao,
                        CodBarras = edtScan.Text
                    };

                    Leitura(parametrosMovimentoReparo); 
                }
            }
            finally
            {
                BtnLeituraDigitacao.IsEnabled = true;
            }
        }
    }
}