using CollectorQi.Models.ESCL021;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL021;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
//using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TransferenciaDepositoPage : ContentPage, INotifyPropertyChanged
    {

        #region Property

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

        public bool LocalizacaoStatus { get; set; } = true;

        string _codDeposSaidaHidden;
        string _codDeposEntradaHidden;

        public List<DepositosGuardaMaterial> _lstDeposito { get; set; }
        string[] _arrayDep;

        public TransferenciaDepositoPage(ParametrosDepositosGuardaMaterial parametrosDepositosGuardaMaterial)
        {
            InitializeComponent();

            lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.Estabelecimento;

            edtItCodigo.Completed += edtItCodigo_Completed;
        }
        void OnClick_Limpar(object sender, EventArgs e)
        {
            Limpar(true);
        }
        async void  OnClick_DepositoSaida(object sender, EventArgs e)
        {
            await BuscaDeposito("Saida");
                        
        }
        async void OnClick_DepositoEntrada(object sender, EventArgs e)
        {
            await BuscaDeposito("Entrada");
        }
        public void SetDepos (string pCodDepos, string pTipoTransacao, bool depLab)
        {
            if (pTipoTransacao == "Entrada")
            {
                _codDeposEntradaHidden = pCodDepos;
                edtDepositoEntrada.Text = pCodDepos;

                if (depLab)
                {
                    frmLocalizacaoEntrada.IsVisible = true;
                }
                else
                {
                    frmLocalizacaoEntrada.IsVisible = false;
                }
            }
            else
            {
                _codDeposSaidaHidden = pCodDepos;
                edtDepositoSaida.Text = pCodDepos;
                if (depLab)
                {
                    frmLocalizacaoSaida.IsVisible = true;
                }
                else
                {
                    frmLocalizacaoSaida.IsVisible = false;
                }
            }

            edtItCodigo.Focus();
        }

        async Task BuscaDeposito(string pTipoTransacao)
        {
            BtnDepositoEntrada.IsEnabled = false;
            BtnDepositoSaida.IsEnabled = false;
            ToolBarPrint.IsEnabled = false;

            try
            {
                var action = await DisplayActionSheet("Escolha o Depósito?", "Cancelar", null, _arrayDep);

                if (action != "Cancelar" && action != null)
                {
                    var deposito = _lstDeposito.FirstOrDefault(x => x.CodDepos == action);

                    SetDepos(action, pTipoTransacao, deposito.DepLab);
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                BtnDepositoEntrada.IsEnabled = true;
                BtnDepositoSaida.IsEnabled = true;
                ToolBarPrint.IsEnabled = true;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new ArmazenagemPage());

            return true;
        }

        public void edtItCodigo_Completed(object sender, EventArgs e)
        {
            edtItCodigo.Focus();
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var result = await DepositosGuardaMaterialService.SendGuardaMaterialAsync();

            if (result != null && result.Param != null && result.Param.ParamResult != null)
            {
                _lstDeposito = result.Param.ParamResult;

                _arrayDep = new string[_lstDeposito.Count];
                for (int i = 0; i < _lstDeposito.Count; i++)
                {
                    _arrayDep[i] = _lstDeposito[i].CodDepos;
                }
            }
        }

        async void OnClick_Efetivar(object sender, EventArgs e)
        {
            BtnEfetivar.IsEnabled = false;

            var pageProgress = new ProgressBarPopUp("Efetivando transferencia, aguarde...");
          
            var result = await DisplayAlert("Confirmação!", $"Deseja efetivar a transferência {edtQuantidade.Text} produto?", "Sim", "Não");

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                if (result.ToString() == "True")
                {
                    if (String.IsNullOrWhiteSpace(edtDepositoSaida.Text))
                    {
                        //edtDepositoSaida.Focus();
                        await DisplayAlert("", "Deposito Saída não encontrada", "OK");
                    }
                    else if (String.IsNullOrWhiteSpace(edtDepositoEntrada.Text))
                    {
                        //edtDepositoEntrada.Focus();
                        await DisplayAlert("", "Deposito Entrada não encontrada", "OK");
                    }
                    else if (String.IsNullOrWhiteSpace(edtItCodigo.Text))
                    {
                       // edtItCodigo.Focus();
                        await DisplayAlert("", "Item não encontrado", "OK");
                    }
                    else if (String.IsNullOrWhiteSpace(edtQuantidade.Text))
                    {
                       // edtQuantidade.Focus();
                        await DisplayAlert("", "Quantidade não encontrada", "OK");
                    }
                    else
                    {
                        var efetivarTransferencia = new EfetivarTransferenciaDepositoService();

                        var dadosLeituraDadosItemTransferenciaDeposito = new DadosLeituraDadosItemTransferenciaDeposito()
                        {
                            CodEstabel        = SecurityAuxiliar.GetCodEstabel(),
                            CodDeposOrigem    = _codDeposSaidaHidden,
                            CodDeposDest      = _codDeposEntradaHidden,
                            CodLocalizaOrigem = edtLocalizacaoSaida.Text ?? String.Empty,
                            CodLocalizaDest   = edtLocalizacaoEntrada.Text ?? String.Empty,
                            NF                = edtNroDocto.Text ?? String.Empty,
                            Serie             = edtSerie.Text ?? String.Empty,
                            CodItem           = edtItCodigo.Text ?? String.Empty,
                            Lote              = edtLote.Text ?? String.Empty,
                            Quantidade        = edtQuantidade.Text
                        };

                        var efetivarTransferenciaRetorno = await efetivarTransferencia.SendTransferenciaDepositoAsync(dadosLeituraDadosItemTransferenciaDeposito);

                        await pageProgress.OnClose();

                        if (efetivarTransferenciaRetorno != null && efetivarTransferenciaRetorno.Retorno == "OK")
                        {
                            await DisplayAlert("Sucesso!", "Transfência efetuada com sucesso!", "OK");
                            Limpar(false);

                            edtItCodigo.Focus();
                        }
                        else
                        {
                            if (efetivarTransferenciaRetorno != null && efetivarTransferenciaRetorno.Resultparam != null && efetivarTransferenciaRetorno.Resultparam.Count > 0)
                            {
                                await DisplayAlert("Erro!", efetivarTransferenciaRetorno.Resultparam[0].ErrorHelp, "Cancelar");
                            }
                            else
                            {
                                await DisplayAlert("Erro!", "Erro ao efetivar transferencia", "Cancelar");
                            }
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
        async void Limpar(bool pBlnQuestion)
        {
            if (pBlnQuestion)
            {
        
                bool blnAlert = await DisplayAlert("Limpar Registros?", "Deseja limpar todos os campos ?", "Sim", "Não");

                if (!blnAlert)
                    return;
            }

            edtLocalizacaoSaida.Text = "";
            edtLocalizacaoEntrada.Text = "";
            edtItCodigo.Text = "";
            edtNroDocto.Text = "";
            edtSerie.Text = "";
            edtLote.Text = "";
            edtQuantidade.Text = "";
            edtSaldo.Text   = "";
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                ToolBarPrint.IsEnabled = false;

                var pageProgress = new ProgressBarPopUp("Carregando...");
                var page = new ArmazenagemPrintPopUp(null, null);
                await PopupNavigation.Instance.PushAsync(page);
                await pageProgress.OnClose();
            }
            finally
            {
                ToolBarPrint.IsEnabled = true;
            }
        }

        private async void edtItCodigo_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Leitura código de barras
            if (String.IsNullOrEmpty(e.OldTextValue) && e.NewTextValue != null && e.NewTextValue.Length >= 5)
            {
                var pageProgress = new ProgressBarPopUp("Carregando...");

                try
                {

                    await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

                    if (String.IsNullOrEmpty(edtDepositoSaida.Text))
                    {
                        throw new Exception("Informar o depósito de saída");
                    }

                    if (String.IsNullOrEmpty(edtDepositoEntrada.Text))
                    {
                        throw new Exception("Informar o depósito de entrada");
                    }

                    var dadosLeituraItemTransferenciaDeposito = new DadosLeituraItemTransferenciaDeposito()
                    {
                        CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                        CodDeposOrigem = _codDeposSaidaHidden,
                        CodLocalizaOrigem = edtLocalizacaoSaida.Text ?? String.Empty,
                        CodigoBarras = e.NewTextValue
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

                        edtItCodigo.Text = item.CodItem;

                        if (!String.IsNullOrEmpty(item.NF))    { edtNroDocto.Text = item.NF; }
                        if (!String.IsNullOrEmpty(item.Serie)) { edtSerie.Text = item.Serie; }
                        if (!String.IsNullOrEmpty(item.Lote))  { edtLote.Text = item.Lote; }

                        try
                        {
                            edtSaldo.Text = int.Parse(item.Saldo).ToString();
                        }
                        catch
                        {
                            edtSaldo.Text = item.Saldo;
                        }

                        if (edtSaldo != null)
                            edtSaldo.Text = edtSaldo.Text.Replace(".0", "").Trim();

                        try
                        {

                            edtQuantidade.Text = int.Parse(item.Quantidade).ToString();

                        }
                        catch
                        {
                            edtQuantidade.Text = item.Quantidade;
                        }

                        if (edtQuantidade.Text != null)
                            edtQuantidade.Text = edtQuantidade.Text.Replace(".0", "").Trim();

                    }

                    await Task.Delay(100);

                    await ScrollView.ScrollToAsync(0, 2000, true);

                    edtQuantidade.Focus();

                    edtQuantidade.CursorPosition = edtQuantidade.Text != null ? edtQuantidade.Text.Length : 0; 

                    try
                    {
                        DependencyService.Get<IKeyboardHelper>().HideKeyboard();
                    }
                    catch { }
                }
                catch (Exception ex)
                {
                    await pageProgress.OnClose();

                    await DisplayAlert("Erro", "Erro leitura etiqueta " + ex.Message, "OK");
                  
                    edtItCodigo.Text = String.Empty;
                    edtItCodigo.Focus();
                
                }
                finally
                {
                   await pageProgress.OnClose();
                }
            }
        }

        private void edtDepositoEntrada_TextChanged(object sender, TextChangedEventArgs e)
        {
            _codDeposEntradaHidden = edtDepositoEntrada.Text;
        }

        private void edtDepositoSaida_TextChanged(object sender, TextChangedEventArgs e)
        {
            _codDeposSaidaHidden = edtDepositoSaida.Text;
        }

        private void BtnVolta_Clicked(object sender, EventArgs e)
        {
            base.OnBackButtonPressed();
            Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new ArmazenagemPage());
        }

        private void ToolBarVoltar_Clicked(object sender, EventArgs e)
        {
            base.OnBackButtonPressed();
            Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new ArmazenagemPage());
        }
    }
    public interface IKeyboardHelper
    {
        void HideKeyboard();
    }
}