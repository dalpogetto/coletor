using AutoMapper;
using CollectorQi.Models.ESCL021;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL021;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GuardaMateriaisDepositoItemListaPage : ContentPage, INotifyPropertyChanged
    {  
        public ObservableCollection<DepositosGuardaMaterialItemViewModel> ObsGuardaMateriaisDepositoItem { get; set; }
        public List<DepositosGuardaMaterialItem> ListaDepositosGuardaMaterialItem { get; set; }
        public string Local { get; set; }
        public string CodDepos { get; set; }
        public int? TipoMovimento { get; set; }
        public int? SemSaldo { get; set; }

        public GuardaMateriaisDepositoItemListaPage(List<DepositosGuardaMaterialItem> listaDepositosGuardaMaterialItem, string local, string codDepos, int? tipoMovimento)
        {
            InitializeComponent();

            if (tipoMovimento == 1)
                BtnTipoMovimento.Text = "Depósito: " + codDepos + "   /   Localização: " + local + "   /   Tipo Movimento: Entrada";
            else
                BtnTipoMovimento.Text = "Depósito: " + codDepos + "   /   Localização: " + local + "   /   Tipo Movimento: Saída";

            ObsGuardaMateriaisDepositoItem = new ObservableCollection<DepositosGuardaMaterialItemViewModel>();
            Local = local;
            TipoMovimento = tipoMovimento;
            CodDepos = codDepos;

            if (listaDepositosGuardaMaterialItem != null)
            {
                //foreach (var item in listaDepositosGuardaMaterialItem)
                foreach (var item in listaDepositosGuardaMaterialItem.Where(x => x.SaldoInfo != 0))
                {
                    var modelView = Mapper.Map<DepositosGuardaMaterialItem, DepositosGuardaMaterialItemViewModel>(item);
                    ObsGuardaMateriaisDepositoItem.Add(modelView);
                }

                ListaDepositosGuardaMaterialItem = listaDepositosGuardaMaterialItem;
            }
            else            
                ListaDepositosGuardaMaterialItem = new List<DepositosGuardaMaterialItem>();     

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
        public string CodigoBarras()
        {
            return "02[85.150.00285-7B[DESCRICAO[4[5[1[7[8[ABC"; 
        }

        protected void BtnLeituraEtiqueta_Clicked(object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new GuardaMateriaisConfirmacaoLeituraItem(Local, CodDepos, TipoMovimento, CodigoBarras()));
        }

        protected void BtnTipoMovimento_Clicked(object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new GuardaMateriaisTipoMovimento(ListaDepositosGuardaMaterialItem, Local, CodDepos));
        }

        private void SwitchCell_OnChanged(object sender, ToggledEventArgs e)
        {
            cvGuardaMateriaisDepositoItem.BindingContext = null;
            ObsGuardaMateriaisDepositoItem = new ObservableCollection<DepositosGuardaMaterialItemViewModel>();

            var listaDepositosGuardaMaterialItem = new List<DepositosGuardaMaterialItem>();

            if (((SwitchCell)sender).On)
                listaDepositosGuardaMaterialItem.AddRange(ListaDepositosGuardaMaterialItem);
            else
                listaDepositosGuardaMaterialItem.AddRange(ListaDepositosGuardaMaterialItem.Where(x => x.SaldoInfo != 0));            

            // Chamar a API novamente passando sem saldo 0 
            foreach (var item in listaDepositosGuardaMaterialItem)
            {
                var modelView = Mapper.Map<DepositosGuardaMaterialItem, DepositosGuardaMaterialItemViewModel>(item);
                ObsGuardaMateriaisDepositoItem.Add(modelView);
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