using AutoMapper;
using CollectorQi.Models.ESCL028;
using CollectorQi.Services.ESCL028;
using CollectorQi.VO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NotaFiscalFinalizarItemConferenciaListaPage : ContentPage, INotifyPropertyChanged
    {
        public ObservableCollection<NotaFiscalViewModel> ObsNotaFiscalItem { get; set; }
        public List<NotaFiscalVO> ListNotaFiscalVO { get; set; }
        public ListaDocumentosNotaFiscal ListaDocumentosNotaFiscal { get; set; }

        public NotaFiscalFinalizarItemConferenciaListaPage(List<NotaFiscalVO> listNotaFiscalVO, ListaDocumentosNotaFiscal listaDocumentosNotaFiscal)
        {
            InitializeComponent();
            ListNotaFiscalVO = listNotaFiscalVO;
            ListaDocumentosNotaFiscal = listaDocumentosNotaFiscal;
            ObsNotaFiscalItem = new ObservableCollection<NotaFiscalViewModel>();

            foreach (var item in ListNotaFiscalVO)
            {
                lblCodEstabel.Text = "Estabelecimento: " + item.CodEstabel;
                var modelView = Mapper.Map<NotaFiscalVO, NotaFiscalViewModel>(item);
                ObsNotaFiscalItem.Add(modelView);
            }

            cvNotaFiscalItem.BindingContext = this;
        }     

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new NotaFiscalFinalizarConferenciaListaPage(ListNotaFiscalVO, ListaDocumentosNotaFiscal));

            return true;
        }

        private void BtnVoltarNotaFiscal_Clicked(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }
    }

    public class NotaFiscalItemViewModel : NotaFiscalVO
    {
        public string Image
        {
            get
            {
                if (this.Conferido)
                    return "intSucessoMed.png";
                else
                    return "intPendenteMed.png";
            }
        }
    }
}