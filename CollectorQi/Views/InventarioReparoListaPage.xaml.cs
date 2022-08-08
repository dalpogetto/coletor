using CollectorQi.Models.ESCL017;
using CollectorQi.Services.ESCL017;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventarioReparoListaPage : ContentPage, INotifyPropertyChanged
    {
        public InventarioReparoListaPage()
        {
            InitializeComponent();            
        }               

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new InventarioPage());

            return true;
        }

        async void BtnIniciarReparoInventario_Clicked(object sender, System.EventArgs e)
        {
            var pInventario = new ParametersInventarioReparoService();

            var parametrosInventarioReparo = new ParametrosInventarioReparo()
            {
                CodEstabel = "101",
                CodTecnico = int.Parse(txtTecnico.Text),
                Senha = txtSenha.Text,
                CodDepos = txtDeposito.Text,
                DtInventario = txtData.Text
            };

            var parametrosRetorno = await pInventario.SendParametersAsync(parametrosInventarioReparo);

            if (parametrosRetorno.Retorno == "OK")
            {
                var dInventario = new DepositoInventarioReparoService();
                var dInventarioRetorno = await dInventario.SendParametersAsync();

                Application.Current.MainPage = new NavigationPage(new InventarioReparoDepositoListaPage(dInventarioRetorno.Param.Resultparam));            
            }
            else
                await DisplayAlert("", "Erro no retorno do envio !!!", "OK");
        }
    }
}