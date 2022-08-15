using AutoMapper;
using CollectorQi.Models.ESCL021;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL021;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GuardaMateriaisDepositoListaPage : ContentPage, INotifyPropertyChanged
    {  
        public ObservableCollection<GuardaMateriaisDepositoViewModel> ObsDepositosGuardaMaterial { get; set; }
        public List<DepositosGuardaMaterial> ListaDepositosGuardaMaterial { get; set; }
        public DepositosGuardaMaterial DepositosGuardaMaterial { get; set; }

        public GuardaMateriaisDepositoListaPage(List<DepositosGuardaMaterial> listadepositosGuardaMaterial)
        {
            InitializeComponent();
            ObsDepositosGuardaMaterial = new ObservableCollection<GuardaMateriaisDepositoViewModel>();
            ListaDepositosGuardaMaterial = new List<DepositosGuardaMaterial>();
            ListaDepositosGuardaMaterial = listadepositosGuardaMaterial;

            lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.GetCodEstabel();

            if (listadepositosGuardaMaterial != null)
            {
                foreach (var item in listadepositosGuardaMaterial)
                {
                    var modelView = Mapper.Map<DepositosGuardaMaterial, GuardaMateriaisDepositoViewModel>(item);
                    ObsDepositosGuardaMaterial.Add(modelView);
                }
            }

            cvDepositosGuardaMaterial.BindingContext = this;
        }  

        async void cvDepositosGuardaMaterial_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cvDepositosGuardaMaterial.SelectedItem == null)
                return;

            var current = (cvDepositosGuardaMaterial.SelectedItem as DepositosGuardaMaterial);

            Application.Current.MainPage = new NavigationPage(new GuardaMateriaisDepositoItemListaPage(null));
        }
    }

    public class GuardaMateriaisDepositoViewModel : DepositosGuardaMaterial
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