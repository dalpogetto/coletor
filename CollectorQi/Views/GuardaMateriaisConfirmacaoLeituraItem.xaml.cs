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
    public partial class GuardaMateriaisConfirmacaoLeituraItem : ContentPage, INotifyPropertyChanged
    {  
        public ObservableCollection<DepositosGuardaMaterialItemViewModel> ObsGuardaMateriaisDepositoItem { get; set; }
        public List<DepositosGuardaMaterialItem> ListaDepositosGuardaMaterialItem { get; set; }
        public string Local { get; set; }
        public string CodDepos { get; set; }
        public string TipoMovimento { get; set; }
        public GuardaMateriaisConfirmacaoLeituraItem(string codigoBarras)
        {
            InitializeComponent();

            //CodDepos = codDepos;
            //BtnTipoMovimento.Text = "Depósito: " + codDepos + "   /   Localização: " + local + "   /   Tipo Movimento: " + tipoMovimento;

            //ListaDepositosGuardaMaterialItem = new List<DepositosGuardaMaterialItem>();
            //ObsGuardaMateriaisDepositoItem = new ObservableCollection<DepositosGuardaMaterialItemViewModel>();
            //Local = local;
            //TipoMovimento = tipoMovimento;

            //if (listaDepositosGuardaMaterialItem != null)
            //{
            //    foreach (var item in listaDepositosGuardaMaterialItem)
            //    {
            //        var modelView = Mapper.Map<DepositosGuardaMaterialItem, DepositosGuardaMaterialItemViewModel>(item);
            //        ObsGuardaMateriaisDepositoItem.Add(modelView);
            //    }
            //}

            //cvGuardaMateriaisDepositoItem.BindingContext = this;
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

        private void BtnConfirmaLeitura_Clicked(object sender, System.EventArgs e)
        {
            //semSaldo = 0;  
        }

        private void BtnZerarSaldoItem_Clicked(object sender, System.EventArgs e)
        {
            //semSaldo = 1;
        }
    }

    public class GuardaMateriaisConfirmacaoLeituraItemViewModel : DepositosGuardaMaterialItem
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