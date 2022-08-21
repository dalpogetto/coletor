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
        public ObservableCollection<GuardaMateriaisConfirmacaoLeituraItemViewModel> ObsGuardaMateriaisConfirmacaoLeituraItem { get; set; }
        public List<DepositosGuardaMaterialItem> ListaDepositosGuardaMaterialItem { get; set; }
        public string Local { get; set; }
        public string CodDepos { get; set; }
        public int? TipoMovimento { get; set; }
        public string CodigoBarras { get; set; }

        public GuardaMateriaisConfirmacaoLeituraItem(List<DepositosGuardaMaterialItem> listaDepositosGuardaMaterialItem, string local, string codDepos, int? tipoMovimento, string codigoBarras)
        {
            InitializeComponent();

            if(tipoMovimento == 1)
                BtnTipoMovimento.Text = "Depósito: " + codDepos + "   /   Localização: " + local + "   /   Tipo Movimento: Entrada";
            else
                BtnTipoMovimento.Text = "Depósito: " + codDepos + "   /   Localização: " + local + "   /   Tipo Movimento: Saída";

            ListaDepositosGuardaMaterialItem = listaDepositosGuardaMaterialItem;
            ObsGuardaMateriaisConfirmacaoLeituraItem = new ObservableCollection<GuardaMateriaisConfirmacaoLeituraItemViewModel>();
            Local = local;
            CodDepos = codDepos;
            TipoMovimento = tipoMovimento;
            CodigoBarras = codigoBarras;

            if (codigoBarras != "")
            {
                var item = new DadosLeituraLocalizaGuardaMaterial() { CodigoBarras = codigoBarras };
                var modelView = Mapper.Map<DadosLeituraLocalizaGuardaMaterial, GuardaMateriaisConfirmacaoLeituraItemViewModel>(item);
                ObsGuardaMateriaisConfirmacaoLeituraItem.Add(modelView);
            }

            cvGuardaMateriaisConfirmacaoLeituraItem.BindingContext = this;
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

        async void BtnConfirmaLeitura_Clicked(object sender, System.EventArgs e)
        {
            var dLeituraEtiqueta = new LeituraEtiquetaGuardaMaterialService();

            var dadosLeituraItemGuardaMaterial = new DadosLeituraItemGuardaMaterial()
            {
                CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                CodDepos = CodDepos,
                CodLocaliza = Local,
                Transacao = TipoMovimento,
                SemSaldo = 0,
                CodigoBarras = CodigoBarras
            };

            var dDepositoItemRetorno = await dLeituraEtiqueta.SendLeituraEtiquetaAsync(dadosLeituraItemGuardaMaterial);

            foreach (var item in dDepositoItemRetorno.Param.ParamResult)
            {
                if (dDepositoItemRetorno.Retorno == "ERRO")
                    _ = DisplayAlert("", "Erro da Leitura de etiqueta !!!", "OK");
                else
                {
                    ListaDepositosGuardaMaterialItem.Add(item);
                    //_ = DisplayAlert("", "Leitura de etiqueta efetuado com sucesso!!!", "OK");
                }
            }

            Application.Current.MainPage = new NavigationPage(new GuardaMateriaisDepositoItemListaPage(ListaDepositosGuardaMaterialItem, Local, CodDepos, TipoMovimento));
        }

        async void BtnZerarSaldoItem_Clicked(object sender, System.EventArgs e)
        {
            var dLeituraEtiqueta = new LeituraEtiquetaGuardaMaterialService();

            var dadosLeituraItemGuardaMaterial = new DadosLeituraItemGuardaMaterial()
            {
                CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                CodDepos = CodDepos,
                CodLocaliza = Local,
                Transacao = TipoMovimento,
                SemSaldo = 1,
                CodigoBarras = CodigoBarras
            };

            var dDepositoItemRetorno = await dLeituraEtiqueta.SendLeituraEtiquetaAsync(dadosLeituraItemGuardaMaterial);

            foreach (var item in dDepositoItemRetorno.Param.ParamResult)
            {
                if (dDepositoItemRetorno.Retorno == "ERRO")
                    _ = DisplayAlert("", "Erro da Leitura de etiqueta !!!", "OK");
                else
                {
                    ListaDepositosGuardaMaterialItem.Add(item);
                    //_ = DisplayAlert("", "Leitura de etiqueta efetuado com sucesso!!!", "OK");
                }
            }

            Application.Current.MainPage = new NavigationPage(new GuardaMateriaisDepositoItemListaPage(ListaDepositosGuardaMaterialItem, Local, CodDepos, TipoMovimento));
        }
    }

    public class GuardaMateriaisConfirmacaoLeituraItemViewModel : DadosLeituraLocalizaGuardaMaterial
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