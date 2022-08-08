using AutoMapper;
using CollectorQi.Models.ESCL017;
using CollectorQi.Services.ESCL017;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventarioReparoDepositoListaPage : ContentPage, INotifyPropertyChanged
    {  
        public ObservableCollection<DepositosInventarioReparoViewModel> ObsInventarioReparoDeposito { get; set; }
        List<DepositosInventarioReparo> ListaDepositosInventarioReparo;

        public InventarioReparoDepositoListaPage(List<DepositosInventarioReparo> listaDepositosInventarioReparo)
        {
            InitializeComponent();
            ObsInventarioReparoDeposito = new ObservableCollection<DepositosInventarioReparoViewModel>();
            ListaDepositosInventarioReparo = new List<DepositosInventarioReparo>();
            ListaDepositosInventarioReparo = listaDepositosInventarioReparo;

            //foreach (var item in ListNotaFiscalVO)
            //{
            //    lblCodEstabel.Text = "Estabelecimento: " + item.CodEstabel;
            //    Conferidos = item.Conferido;
            //}

            if (listaDepositosInventarioReparo != null)
            {
                foreach (var item in listaDepositosInventarioReparo)
                {
                    var modelView = Mapper.Map<DepositosInventarioReparo, DepositosInventarioReparoViewModel>(item);
                    ObsInventarioReparoDeposito.Add(modelView);
                }
            }

            cvInventarioReparoDeposito.BindingContext = this;
        }     

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new InventarioReparoListaPage());

            return true;
        }

        private void BtnVoltarNotaFiscal_Clicked(object sender, System.EventArgs e)
        {
            OnBackButtonPressed();
        }

        async void BtnApuracaoItens_Clicked(object sender, System.EventArgs e)
        {
            var pLeituraEtiqueta = new LeituraEtiquetaInventarioReparoService();

            var leituraEtiquetaInventarioReparo = new LeituraEtiquetaInventarioReparo()
            {
               
            };

            var LeituraEtiqueta = await pLeituraEtiqueta.SendParametersAsync();

            //if (parametrosRetorno.Retorno == "OK")
            //{
            //    var dInventario = new DepositoInventarioReparoService();
            //    var dInventarioRetorno = await dInventario.SendParametersAsync();

            //    Application.Current.MainPage = new NavigationPage(new InventarioReparoDepositoListaPage(dInventarioRetorno.Param.Resultparam));
            //}
            //else
            //    await DisplayAlert("", "Erro no retorno do envio !!!", "OK");

            Application.Current.MainPage = new NavigationPage(new InventarioReparoLeituraEtiquetaListaPage(LeituraEtiqueta.Param.Resultparam, ListaDepositosInventarioReparo));
        }
    }

    public class DepositosInventarioReparoViewModel : DepositosInventarioReparo
    {
        //public string Image
        //{
        //    get
        //    {
        //        if (this.ItensRestantes)
        //            return "intSucessoMed.png";
        //        else
        //            return "intPendenteMed.png";
        //    }
        //}
    }
}