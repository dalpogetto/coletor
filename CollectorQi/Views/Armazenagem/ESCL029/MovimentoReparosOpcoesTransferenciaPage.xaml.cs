using AutoMapper;
using CollectorQi.Models.ESCL017;
using CollectorQi.Models.ESCL029;
using CollectorQi.Resources;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MovimentoReparosOpcoesTransferenciaPage : ContentPage
    {
        public List<OpcoesTransferenciaMovimentoReparo> ListaOpcoesTransferenciaMovimentoReparo;
        public ObservableCollection<OpcoesTransferenciaMovimentoReparoViewModel> ObsOpcoesTransferenciaMovimentoReparo { get; set; }
        public ParametrosInventarioReparo Parametros { get; set; }

        

        public MovimentoReparosOpcoesTransferenciaPage(List<OpcoesTransferenciaMovimentoReparo> listaOpcoesTransferenciaMovimentoReparo, ParametrosInventarioReparo parametros)
        {
            InitializeComponent();

            lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.GetCodEstabel();
            ListaOpcoesTransferenciaMovimentoReparo = listaOpcoesTransferenciaMovimentoReparo;
            ObsOpcoesTransferenciaMovimentoReparo = new ObservableCollection<OpcoesTransferenciaMovimentoReparoViewModel>();
            Parametros = parametros;

            if (listaOpcoesTransferenciaMovimentoReparo != null)
            {
                foreach (var item in listaOpcoesTransferenciaMovimentoReparo)
                {
                    var modelView = Mapper.Map<OpcoesTransferenciaMovimentoReparo, OpcoesTransferenciaMovimentoReparoViewModel>(item);
                    ObsOpcoesTransferenciaMovimentoReparo.Add(modelView);
                }
            }

            cvOpcoesTransferenciaMovimentoReparo.BindingContext = this;
        }

        private void cvOpcoesTransferencia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var current = (cvOpcoesTransferenciaMovimentoReparo.SelectedItem as OpcoesTransferenciaMovimentoReparo);
            

            Application.Current.MainPage = new NavigationPage(new MovimentoReparosLeituraEtiqueta(ListaOpcoesTransferenciaMovimentoReparo, Parametros, current.DescOpcao));
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new ArmazenagemMovimentoReparoPage(null));

            return true;
        }

        public class OpcoesTransferenciaMovimentoReparoViewModel : OpcoesTransferenciaMovimentoReparo
        {

        }
    }
}