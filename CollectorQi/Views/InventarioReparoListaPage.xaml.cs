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
        public InventarioReparoListaPage(ParametrosInventarioReparo parametrosInventarioReparo)
        {
            InitializeComponent();

            lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.GetCodEstabel();

            if (parametrosInventarioReparo != null)
            {
                if(!string.IsNullOrEmpty(parametrosInventarioReparo.CodTecnico.ToString()))
                    txtTecnico.Text = parametrosInventarioReparo.CodTecnico.ToString();

                txtDeposito.Text = parametrosInventarioReparo.CodDepos;
                txtSenha.Text = parametrosInventarioReparo.Senha;
                txtData.Text = parametrosInventarioReparo.DtInventario;
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
                    parametrosIR.CodTecnico = int.Parse(txtTecnico.Text);
                    parametrosIR.Senha = txtSenha.Text;
                    parametrosIR.CodDepos = txtDeposito.Text;
                    parametrosIR.DtInventario = txtData.Text;

                    var pInventario = new ParametersInventarioReparoService();

                    var parametrosRetorno = await pInventario.SendParametersAsync(parametrosIR);

                    if (parametrosRetorno.Retorno == "OK")
                        Application.Current.MainPage = new NavigationPage(new InventarioReparoLeituraEtiquetaListaPage(null, parametrosIR));
                    else
                        await DisplayAlert("", "Erro no retorno do envio !", "OK");
                }
                else
                    await DisplayAlert("", "Data inválida !", "OK");
            }
        }

        async void BtnBuscaDeposito_Clicked(object sender, System.EventArgs e)
        {
            var parametrosIR = new ParametrosInventarioReparo();
            parametrosIR.CodEstabel = SecurityAuxiliar.GetCodEstabel();

            if(!string.IsNullOrEmpty(txtTecnico.Text))
                parametrosIR.CodTecnico = int.Parse(txtTecnico.Text);

            parametrosIR.Senha = txtSenha.Text;
            parametrosIR.CodDepos = txtDeposito.Text;
            parametrosIR.DtInventario = txtData.Text;            

            var dInventario = new DepositoInventarioReparoService();
            var dInventarioRetorno = await dInventario.SendParametersAsync();

            Application.Current.MainPage = new NavigationPage(new InventarioReparoDepositoListaPage(dInventarioRetorno.Param.Resultparam, parametrosIR, "InventarioReparo"));            
        }
    }
}