using AutoMapper;
using CollectorQi.Models.ESCL017;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL017;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventarioReparoLeituraEtiquetaListaPage : ContentPage, INotifyPropertyChanged
    {  
        public ObservableCollection<InventarioReparoLeituraEtiquetaViewModel> ObsInventarioReparoLeituraEtiqueta { get; set; }
        public List<LeituraEtiquetaInventarioReparo> ListaLeituraEtiquetaInventarioReparo { get; set; }
        public ParametrosInventarioReparo parametrosInventarioReparo { get; set; }

        public InventarioReparoLeituraEtiquetaListaPage(List<LeituraEtiquetaInventarioReparo> listaLeituraEtiquetaInventarioReparo,
            ParametrosInventarioReparo _parametrosInventarioReparo)
        {
            InitializeComponent();

            lblCodEstabel.Text = "Estab: " + SecurityAuxiliar.GetCodEstabel() + "  Técnico: " + _parametrosInventarioReparo.CodEstabel +
                "  Depós: " + _parametrosInventarioReparo.CodDepos + "  Dt Inventário: " + _parametrosInventarioReparo.DtInventario;

            ListaLeituraEtiquetaInventarioReparo = new List<LeituraEtiquetaInventarioReparo>();
            ListaLeituraEtiquetaInventarioReparo = listaLeituraEtiquetaInventarioReparo;

            parametrosInventarioReparo = new ParametrosInventarioReparo();
            parametrosInventarioReparo = _parametrosInventarioReparo;

            ObsInventarioReparoLeituraEtiqueta = new ObservableCollection<InventarioReparoLeituraEtiquetaViewModel>();          

            if (listaLeituraEtiquetaInventarioReparo != null)
            {
                foreach (var item in listaLeituraEtiquetaInventarioReparo)
                {
                    var modelView = Mapper.Map<LeituraEtiquetaInventarioReparo, InventarioReparoLeituraEtiquetaViewModel>(item);
                    ObsInventarioReparoLeituraEtiqueta.Add(modelView);
                }
            }

            cvInventarioReparoLeituraEtiqueta.BindingContext = this;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new InventarioReparoListaPage("", null));

            return true;
        }

        private void BtnVoltarLeituraEtiqueta_Clicked(object sender, System.EventArgs e)
        {
            OnBackButtonPressed();
        }

        async void BtnLeituraEtiqueta_Clicked(object sender, System.EventArgs e)
        {
            var leituraReparo = new LeituraEtiquetaInventarioReparo() { CodBarras = "" };

            var pLeituraEtiqueta = new LeituraEtiquetaInventarioReparoService();
            var listaLeituraEtiqueta =  await pLeituraEtiqueta.SendParametersAsync(parametrosInventarioReparo, leituraReparo);
            ObsInventarioReparoLeituraEtiqueta = new ObservableCollection<InventarioReparoLeituraEtiquetaViewModel>();

            foreach (var item in listaLeituraEtiqueta.Param.Resultparam)
            {
                string[] msg = item.Mensagem.Split(':');

                if (msg[0] == "ERRO")
                    Application.Current.MainPage = new NavigationPage(new InventarioReparoLeituraEtiquetaManual(listaLeituraEtiqueta.Param.Resultparam, parametrosInventarioReparo));
                else
                    _ = DisplayAlert("", "Leitura de etiqueta efetuado com sucesso!!!", "OK");

                var modelView = Mapper.Map<LeituraEtiquetaInventarioReparo, InventarioReparoLeituraEtiquetaViewModel>(item);
                ObsInventarioReparoLeituraEtiqueta.Add(modelView);
            }

            cvInventarioReparoLeituraEtiqueta.BindingContext = this;
        }
    }

    public class InventarioReparoLeituraEtiquetaViewModel : LeituraEtiquetaInventarioReparo
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