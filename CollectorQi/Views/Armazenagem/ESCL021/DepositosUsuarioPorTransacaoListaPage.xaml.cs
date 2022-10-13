using AutoMapper;
using CollectorQi.Models.ESCL021;
using CollectorQi.Resources;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DepositosUsuarioPorTransacaoListaPage : ContentPage, INotifyPropertyChanged
    {
        public ObservableCollection<DepositosUsuarioPorTransacaoViewModel> ObsDepositosUsuarioTransacao { get; set; }
        public ParametrosDepositosGuardaMaterial ParametrosDepositosGuardaMaterial;

        public DepositosUsuarioPorTransacaoListaPage(List<DepositosGuardaMaterial> listaDepositosGuardaMaterial, ParametrosDepositosGuardaMaterial parametrosDepositosGuardaMaterial)
        {
            InitializeComponent();

            ObsDepositosUsuarioTransacao = new ObservableCollection<DepositosUsuarioPorTransacaoViewModel>();
            lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.Estabelecimento;
            ParametrosDepositosGuardaMaterial = new ParametrosDepositosGuardaMaterial();
            ParametrosDepositosGuardaMaterial = parametrosDepositosGuardaMaterial;

            if (listaDepositosGuardaMaterial != null)
            {
                foreach (var item in listaDepositosGuardaMaterial)
                {
                    var modelView = Mapper.Map<DepositosGuardaMaterial, DepositosUsuarioPorTransacaoViewModel>(item);
                    ObsDepositosUsuarioTransacao.Add(modelView);
                }
            }

            cvDepositosUsuarioTransacao.BindingContext = this;
        }

        private void cvDepositosUsuarioTransacao_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cvDepositosUsuarioTransacao.SelectedItem == null)
                return;

            var current = (cvDepositosUsuarioTransacao.SelectedItem as DepositosGuardaMaterial);
            //var depositosGuardaMaterial = new DepositosGuardaMaterial() { Nome = current.Nome };

            if (ParametrosDepositosGuardaMaterial.TipoTransferencia == "1")
            {
                ParametrosDepositosGuardaMaterial.DepositoEntrada = current.Nome;
                Application.Current.MainPage = new NavigationPage(new TransferenciaDepositoPage(ParametrosDepositosGuardaMaterial));
            }
            else
            {
                ParametrosDepositosGuardaMaterial.DepositoSaida = current.Nome;
                Application.Current.MainPage = new NavigationPage(new TransferenciaDepositoPage(ParametrosDepositosGuardaMaterial));
            }
        }
    }

    public class DepositosUsuarioPorTransacaoViewModel : DepositosGuardaMaterial
    {
        public string CodDeposNome
        {
            get { return CodDepos + " - " + Nome; }
        }

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