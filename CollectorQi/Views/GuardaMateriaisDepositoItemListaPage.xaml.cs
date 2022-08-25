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
        public int SemSaldo { get; set; }

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
                // Retirar .Where(x => x.SaldoInfo != 0)
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

        async void BtnLeituraEtiqueta_Clicked(object sender, System.EventArgs e)
        {           
            var dLeituraEtiqueta = new LeituraEtiquetaGuardaMaterialService();
            ObsGuardaMateriaisDepositoItem = new ObservableCollection<DepositosGuardaMaterialItemViewModel>();
            var depositosGuardaMaterialItem = new DepositosGuardaMaterialItem();

            var dadosLeituraItemGuardaMaterial = new DadosLeituraItemGuardaMaterial()
            {
                CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                CodDepos = CodDepos,
                CodLocaliza = Local,
                Transacao = TipoMovimento,
                SemSaldo = SemSaldo,
                CodigoBarras = CodigoBarras()
            };

            var dDepositoItemRetorno = await dLeituraEtiqueta.SendLeituraEtiquetaAsync(dadosLeituraItemGuardaMaterial);

            foreach (var item in dDepositoItemRetorno.Param.ParamResult)
            {
                depositosGuardaMaterialItem = item;

                if (dDepositoItemRetorno.Retorno == "ERRO")
                    _ = DisplayAlert("", "Erro da Leitura de etiqueta !!!", "OK");
                else
                    _ = DisplayAlert("", "Leitura de etiqueta efetuado com sucesso!!!", "OK");
            }

            var dLeituraEtiquetaLerLocaliza = new LeituraEtiquetaLerLocalizaGuardaMaterialService();
            var dRetorno = await dLeituraEtiquetaLerLocaliza.SendLeituraEtiquetaAsync(dadosLeituraItemGuardaMaterial);            

            foreach (var item in dRetorno.Param.ParamResult)
            {
                var modelView = Mapper.Map<DepositosGuardaMaterialItem, DepositosGuardaMaterialItemViewModel>(item);

                if (modelView.CodigoItem == depositosGuardaMaterialItem.CodigoItem)
                {
                    _ = DisplayAlert("", "Etiqueta já existe !", "OK");
                    break;
                }

                ObsGuardaMateriaisDepositoItem.Add(modelView);
            }

            cvGuardaMateriaisDepositoItem.BindingContext = this;
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
            {
                listaDepositosGuardaMaterialItem.AddRange(ListaDepositosGuardaMaterialItem);
                SemSaldo = 1;
            }
            else
            {
                // // Retirar .Where(x => x.SaldoInfo != 0)
                listaDepositosGuardaMaterialItem.AddRange(ListaDepositosGuardaMaterialItem.Where(x => x.SaldoInfo != 0));
                SemSaldo = 0;
            }

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