﻿using AutoMapper;
using CollectorQi.Models.ESCL021;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL021;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GuardaMateriaisConfirmacaoLeituraItem : ContentPage, INotifyPropertyChanged
    {  
        public ObservableCollection<GuardaMateriaisConfirmacaoLeituraItemViewModel> ObsGuardaMateriaisConfirmacaoLeituraItem { get; set; }
        public string Local { get; set; }
        public string CodDepos { get; set; }
        public int? TipoMovimento { get; set; }      
        public string CodigoBarras { get; set; }

        public GuardaMateriaisConfirmacaoLeituraItem(string local, string codDepos, int? tipoMovimento, string codigoBarras)
        {
            InitializeComponent();

            if(tipoMovimento == 1)
                BtnTipoMovimento.Text = "Depósito: " + codDepos + "   /   Localização: " + local + "   /   Tipo Movimento: Entrada";
            else
                BtnTipoMovimento.Text = "Depósito: " + codDepos + "   /   Localização: " + local + "   /   Tipo Movimento: Saída";

            ObsGuardaMateriaisConfirmacaoLeituraItem = new ObservableCollection<GuardaMateriaisConfirmacaoLeituraItemViewModel>();
            Local = local;
            CodDepos = codDepos;
            TipoMovimento = tipoMovimento;
            CodigoBarras = codigoBarras;
          
            var item = new DadosLeituraLocalizaGuardaMaterial() { CodigoBarras = codigoBarras };
            var modelView = Mapper.Map<DadosLeituraLocalizaGuardaMaterial, GuardaMateriaisConfirmacaoLeituraItemViewModel>(item);
            ObsGuardaMateriaisConfirmacaoLeituraItem.Add(modelView);

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

        async Task LeituraEtiqueta(int semSaldo)
        {
            var dLeituraEtiqueta = new LeituraEtiquetaGuardaMaterialService();

            var dadosLeituraItemGuardaMaterial = new DadosLeituraItemGuardaMaterial()
            {
                CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                CodDepos = CodDepos,
                CodLocaliza = Local,
                Transacao = TipoMovimento,
                SemSaldo = semSaldo,
                CodigoBarras = CodigoBarras
            };

            var dDepositoItemRetorno = await dLeituraEtiqueta.SendLeituraEtiquetaAsync(dadosLeituraItemGuardaMaterial);

            foreach (var item in dDepositoItemRetorno.Param.ParamResult)
            {
                if (dDepositoItemRetorno.Retorno == "ERRO")
                    _ = DisplayAlert("", "Erro da Leitura de etiqueta !!!", "OK");
                else                                    
                    _ = DisplayAlert("", "Leitura de etiqueta efetuado com sucesso!!!", "OK");
            }

            var dLeituraEtiquetaLerLocaliza = new LeituraEtiquetaLerLocalizaGuardaMaterialService();
            var dRetorno = await dLeituraEtiquetaLerLocaliza.SendLeituraEtiquetaAsync(dadosLeituraItemGuardaMaterial);

            Application.Current.MainPage = new NavigationPage(new GuardaMateriaisDepositoItemListaPage(dRetorno.Param.ParamResult, Local, CodDepos, TipoMovimento));
        }

        async void BtnConfirmaLeitura_Clicked(object sender, System.EventArgs e)
        {
            await LeituraEtiqueta(0);
        }

        async void BtnZerarSaldoItem_Clicked(object sender, System.EventArgs e)
        {
            await LeituraEtiqueta(1);
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