using AutoMapper;
using CollectorQi.Models.ESCL017;
using CollectorQi.Models.ESCL021;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL017;
using CollectorQi.Services.ESCL021;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GuardaMateriaisDepositoItemListaPage : ContentPage, INotifyPropertyChanged
    {  
        public ObservableCollection<DepositosGuardaMaterialItemViewModel> ObsGuardaMateriaisDepositoItem { get; set; }
        public List<DepositosGuardaMaterialItem> ListaDepositosGuardaMaterialItem { get; set; }
        //public ParametrosInventarioReparo parametrosInventarioReparo { get; set; }
        //public GuardaMateriaisDepositoItemListaPage(List<LeituraEtiquetaInventarioReparo> listaLeituraEtiquetaInventarioReparo,
        //    ParametrosInventarioReparo _parametrosInventarioReparo)
        public GuardaMateriaisDepositoItemListaPage(List<DepositosGuardaMaterialItem> listaDepositosGuardaMaterialItem)
        {
            InitializeComponent();

            //lblCodEstabel.Text = "Estab: " + SecurityAuxiliar.GetCodEstabel() + "  Técnico: " + _parametrosInventarioReparo.CodEstabel +
            //    "  Depós: " + _parametrosInventarioReparo.CodDepos + "  Dt Inventário: " + _parametrosInventarioReparo.DtInventario;

            //ListaLeituraEtiquetaInventarioReparo = listaLeituraEtiquetaInventarioReparo;

            //parametrosInventarioReparo = new ParametrosInventarioReparo();
            //parametrosInventarioReparo = _parametrosInventarioReparo;

            ListaDepositosGuardaMaterialItem = new List<DepositosGuardaMaterialItem>();
            ObsGuardaMateriaisDepositoItem = new ObservableCollection<DepositosGuardaMaterialItemViewModel>();

            if (listaDepositosGuardaMaterialItem != null)
            {  
                foreach (var item in listaDepositosGuardaMaterialItem)
                {
                    var modelView = Mapper.Map<DepositosGuardaMaterialItem, DepositosGuardaMaterialItemViewModel>(item);
                    ObsGuardaMateriaisDepositoItem.Add(modelView);
                }
            }

            cvGuardaMateriaisDepositoItem.BindingContext = this;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new GuardaMateriaisDepositoListaPage(null));

            return true;
        }

        private void BtnVoltarLeituraEtiqueta_Clicked(object sender, System.EventArgs e)
        {
            OnBackButtonPressed();
        }

        async void BtnLeituraEtiqueta_Clicked(object sender, System.EventArgs e)
        {
            var dLeituraEtiqueta = new LeituraEtiquetaGuardaMaterialService();
            var dDepositoItemRetorno = await dLeituraEtiqueta.SendLeituraEtiquetaAsync();

            foreach (var item in dDepositoItemRetorno.Param.ParamResult)
            {
                if (dDepositoItemRetorno.Retorno == "ERRO")
                    _ = DisplayAlert("", "Erro da Leitura de etiqueta !!!", "OK");
                else
                {  
                    var modelView = Mapper.Map<DepositosGuardaMaterialItem, DepositosGuardaMaterialItemViewModel>(item);
                    ObsGuardaMateriaisDepositoItem.Add(modelView);

                    _ = DisplayAlert("", "Leitura de etiqueta efetuado com sucesso!!!", "OK");
                }
            }

            cvGuardaMateriaisDepositoItem.BindingContext = this;
        }
    }

    public class DepositosGuardaMaterialItemViewModel : DepositosGuardaMaterialItem
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