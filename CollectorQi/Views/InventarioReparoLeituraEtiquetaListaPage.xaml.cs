using AutoMapper;
using CollectorQi.Models.ESCL017;
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
        //public List<NotaFiscalVO> ListNotaFiscalVO { get; set; }
        public List<LeituraEtiquetaInventarioReparo> ListaLeituraEtiquetaInventarioReparo { get; set; }
        public List<DepositosInventarioReparo> ListaDepositosInventarioReparo { get; set; }

        //public InventarioReparoDepositoListaPage(List<NotaFiscalVO> listNotaFiscalVO, List<ListaDocumentosNotaFiscal> listaDocumentosNotaFiscal)
        public InventarioReparoLeituraEtiquetaListaPage(List<LeituraEtiquetaInventarioReparo> listaLeituraEtiquetaInventarioReparo, List<DepositosInventarioReparo> listaDepositosInventarioReparo)
        {
            InitializeComponent();

            ListaLeituraEtiquetaInventarioReparo = new List<LeituraEtiquetaInventarioReparo>();
            ListaLeituraEtiquetaInventarioReparo = listaLeituraEtiquetaInventarioReparo;
            ListaDepositosInventarioReparo = new List<DepositosInventarioReparo>();
            ListaDepositosInventarioReparo = listaDepositosInventarioReparo;

            ObsInventarioReparoLeituraEtiqueta = new ObservableCollection<InventarioReparoLeituraEtiquetaViewModel>();

            //foreach (var item in ListNotaFiscalVO)
            //{
            //    lblCodEstabel.Text = "Estabelecimento: " + item.CodEstabel;
            //    Conferidos = item.Conferido;
            //}

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
            Application.Current.MainPage = new NavigationPage(new InventarioReparoDepositoListaPage(ListaDepositosInventarioReparo));

            return true;
        }       

        private void BtnVoltarLeituraEtiqueta_Clicked(object sender, System.EventArgs e)
        {
            OnBackButtonPressed();
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