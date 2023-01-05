using CollectorQi.Models.ESCL017;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL017;
using System;
using System.ComponentModel;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventarioReparoListaPage : ContentPage, INotifyPropertyChanged
    {

        private string _codDeposHidden;

        public InventarioReparoListaPage(ParametrosInventarioReparo parametrosInventarioReparo)
        {
            InitializeComponent();

            lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.GetCodEstabel();

            if (parametrosInventarioReparo != null)
            {
                if(!string.IsNullOrEmpty(parametrosInventarioReparo.CodTecnico.ToString()))
                    txtTecnico.Text = parametrosInventarioReparo.CodTecnico.ToString();

                txtDeposito.Text = parametrosInventarioReparo.CodDepos;
                txtSenha.Text    = parametrosInventarioReparo.Senha;
                txtData.Text     = parametrosInventarioReparo.DtInventario;
            }                
        }               
        
        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new InventarioPage());

            return true;
        }

        async void BtnIniciarReparoInventario_Clicked(object sender, System.EventArgs e)
        {
            BtnIniciarReparoInventario.IsEnabled = false;

            try
            {
                var parametrosIR = new ParametrosInventarioReparo();
                parametrosIR.CodEstabel = SecurityAuxiliar.GetCodEstabel();

                if (string.IsNullOrEmpty(txtTecnico.Text) || string.IsNullOrEmpty(txtSenha.Text) ||
                   string.IsNullOrEmpty(txtDeposito.Text) || string.IsNullOrEmpty(txtData.Text))
                    await DisplayAlert("", "Todos os campos são obrigatórios !", "OK");
                else
                {
                    DateTime valor;

                    if (DateTime.TryParseExact(txtData.Text, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out valor))
                    {
                        parametrosIR.CodTecnico   = int.Parse(txtTecnico.Text);
                        parametrosIR.Senha        = txtSenha.Text;
                        parametrosIR.CodDepos     = _codDeposHidden;
                        parametrosIR.DtInventario = txtData.Text;

                        var parametrosRetorno = await ParametersInventarioReparoService.SendParametersAsync(parametrosIR);

                        if (parametrosRetorno.Retorno == "OK")
                            Application.Current.MainPage = new NavigationPage(new InventarioReparoLeituraEtiquetaListaPage(parametrosIR));
                        else
                        {
                            if (parametrosRetorno.Resultparam != null && parametrosRetorno.Resultparam.Count > 0)
                            {
                                await DisplayAlert("ERRO!", parametrosRetorno.Resultparam[0].ErrorHelp, "OK");
                            } 
                            else {
                                await DisplayAlert("ERRO!", "Erro no retorno do envio !", "OK");
                            }
                        }
                    }
                    else
                        await DisplayAlert("", "Data inválida !", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("ERRO!", ex.Message, "OK");
            }
            finally
            {
                BtnIniciarReparoInventario.IsEnabled = true;
            }
        }

        async void BtnBuscaDeposito_Clicked(object sender, System.EventArgs e)
        {
            BtnBuscaDeposito.IsEnabled = false;

            try
            {
                var page = new InventarioReparoListaDepositoPopUp(txtDeposito);

                page._setDepos = SetDepos;

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page);

                BtnBuscaDeposito.IsEnabled = true;

            }
            catch (Exception)
            {

            }
            finally
            {
                BtnBuscaDeposito.IsEnabled = true;
            }
        }

        public void SetDepos (string pCodDepos)
        {
            _codDeposHidden = pCodDepos;

        }
    }
}