using AutoMapper;
using CollectorQi.Models.ESCL028;
using CollectorQi.Services.ESCL028;
using CollectorQi.VO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotaFiscalFinalizarConferenciaListaPage : ContentPage, INotifyPropertyChanged
    {  
        public ObservableCollection<NotaFiscalCabecalhoViewModel> ObsCabecalhoNotaFiscal { get; set; }
        public List<NotaFiscalVO> ListNotaFiscalVO { get; set; }
        public ListaDocumentosNotaFiscal ListaDocumentosNotaFiscal { get; set; }

        public NotaFiscalFinalizarConferenciaListaPage(List<NotaFiscalVO> listNotaFiscalVO, ListaDocumentosNotaFiscal listaDocumentosNotaFiscal)
        {
            InitializeComponent();
            ListNotaFiscalVO = listNotaFiscalVO;
            ListaDocumentosNotaFiscal = listaDocumentosNotaFiscal;
            ObsCabecalhoNotaFiscal = new ObservableCollection<NotaFiscalCabecalhoViewModel>();

            if (listaDocumentosNotaFiscal != null)
            {
                var modelView = Mapper.Map<ListaDocumentosNotaFiscal, NotaFiscalCabecalhoViewModel>(listaDocumentosNotaFiscal);
                ObsCabecalhoNotaFiscal.Add(modelView);
            }

            foreach (var item in ListNotaFiscalVO)
                lblCodEstabel.Text = "Estabelecimento: " + item.CodEstabel;

            cvCabecalhoNotaFiscal.BindingContext = this;
        }     

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new NotaFiscalConferenciaReparosListaPage());

            return true;
        }

        async void BtnFinalizarNotaFiscal_Clicked(object sender, EventArgs e)
        {
            var parametersNotaFiscalFinalizar = new FinalizarConferenciaNotaFiscalService();
            var finalizarConferenciaNotaFiscal = new FinalizarConferenciaNotaFiscal();
            var lstNotaFiscalFinalizar = await parametersNotaFiscalFinalizar.SendFinalizarConferenciaAsync(finalizarConferenciaNotaFiscal);

            ObsCabecalhoNotaFiscal = new ObservableCollection<NotaFiscalCabecalhoViewModel>();            

            if (lstNotaFiscalFinalizar.Retorno == "OK")            
                await DisplayAlert("OK!", lstNotaFiscalFinalizar.Param.Mensagem, "OK");
            else
                await DisplayAlert("Erro!", lstNotaFiscalFinalizar.Param.Mensagem, "OK");         

            cvCabecalhoNotaFiscal.BindingContext = this;
        }

        private void BtnVoltarNotaFiscal_Clicked(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }

        private void cvCabecalhoNotaFiscal_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var current = (cvCabecalhoNotaFiscal.SelectedItem as ListaDocumentosNotaFiscal);

            var listItemNotaFiscal = ListNotaFiscalVO.Where(x => x.NroDocto == current.Docto && x.Conferido == false).ToList();

            Application.Current.MainPage = new NavigationPage(new NotaFiscalFinalizarItemConferenciaListaPage(listItemNotaFiscal, ListaDocumentosNotaFiscal));
        }
    }

    public class NotaFiscalCabecalhoViewModel : ListaDocumentosNotaFiscal
    {
        public string Image
        {
            get
            {
                if (this.Atualizar)
                    return "intSucessoMed.png";
                else
                    return "intPendenteMed.png";
            }
        }
    }
}