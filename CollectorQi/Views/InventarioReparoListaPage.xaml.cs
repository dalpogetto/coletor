using CollectorQi.Models.ESCL017;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL017;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventarioReparoListaPage : ContentPage, INotifyPropertyChanged
    {
        public InventarioReparoListaPage(string depositoNome, ParametrosInventarioReparo parametrosInventarioReparo)
        {
            InitializeComponent();

            lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.GetCodEstabel();

            if (depositoNome != "")
                txtDeposito.Text = depositoNome;
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

            if (!string.IsNullOrEmpty(txtTecnico.Text))
                parametrosIR.CodTecnico = int.Parse(txtTecnico.Text);

            parametrosIR.Senha = txtSenha.Text;
            parametrosIR.CodDepos = txtDeposito.Text;
            parametrosIR.DtInventario = txtData.Text;

            var pInventario = new ParametersInventarioReparoService();

            var parametrosRetorno = await pInventario.SendParametersAsync(parametrosIR);

            if (parametrosRetorno.Retorno == "OK")
            {
                var pLeituraEtiqueta = new LeituraEtiquetaInventarioReparoService();
                var leituraEtiquetaRetorno = await pLeituraEtiqueta.SendParametersAsync(null, null);
                Application.Current.MainPage = new NavigationPage(new InventarioReparoLeituraEtiquetaListaPage(leituraEtiquetaRetorno.Param.Resultparam, parametrosIR));
            }
            else
                await DisplayAlert("", "Erro no retorno do envio !!!", "OK");
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

            Application.Current.MainPage = new NavigationPage(new InventarioReparoDepositoListaPage(dInventarioRetorno.Param.Resultparam, parametrosIR));            
        }
    }
}