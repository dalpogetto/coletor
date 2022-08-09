using CollectorQi.Models.ESCL017;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL017;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventarioReparoLeituraEtiquetaManual : ContentPage, INotifyPropertyChanged
    {
        public List<LeituraEtiquetaInventarioReparo> ListaLeituraEtiquetaInventarioReparo { get; set; }
        public ParametrosInventarioReparo parametrosInventarioReparo { get; set; }
        public InventarioReparoLeituraEtiquetaManual(List<LeituraEtiquetaInventarioReparo> listaLeituraEtiquetaInventarioReparo,ParametrosInventarioReparo _parametrosInventarioReparo)
        {
            InitializeComponent();

            lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.GetCodEstabel();

            ListaLeituraEtiquetaInventarioReparo = new List<LeituraEtiquetaInventarioReparo>();
            ListaLeituraEtiquetaInventarioReparo = listaLeituraEtiquetaInventarioReparo;

            parametrosInventarioReparo = new ParametrosInventarioReparo();
            parametrosInventarioReparo = _parametrosInventarioReparo;
        }               

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new InventarioReparoLeituraEtiquetaListaPage(ListaLeituraEtiquetaInventarioReparo, parametrosInventarioReparo));

            return true;
        }

        async void BtnEtiquetaManualConfirmar_Clicked(object sender, System.EventArgs e)
        {
            var leituraReparo = new LeituraEtiquetaInventarioReparo();

            leituraReparo.CodEstabel = parametrosInventarioReparo.CodEstabel;
            leituraReparo.CodFilial = txtFilial.Text;

            if(!string.IsNullOrEmpty(txtNumeroRR.Text))
                leituraReparo.NumRR = int.Parse(txtNumeroRR.Text);

            if (!string.IsNullOrEmpty(txtDigito.Text))
                leituraReparo.Digito = int.Parse(txtDigito.Text);

            var pLeituraEtiqueta = new LeituraEtiquetaInventarioReparoService();
            var leituraEtiqueta = await pLeituraEtiqueta.SendParametersAsync(parametrosInventarioReparo, leituraReparo);

            if (leituraEtiqueta.Retorno == "OK")
            {   
                _ = DisplayAlert("", "Leitura de etiqueta efetuada com sucesso !!!", "OK");
                Application.Current.MainPage = new NavigationPage(new InventarioReparoLeituraEtiquetaListaPage(ListaLeituraEtiquetaInventarioReparo, parametrosInventarioReparo));
            }
            else
                _ = DisplayAlert("", "Erro ao realizar a leitura da etiqueta !!!", "OK");
        }
    }
}