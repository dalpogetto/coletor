using AutoMapper;
using CollectorQi.Models.ESCL017;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL017;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventarioReparoLeituraEtiquetaListaPage : ContentPage, INotifyPropertyChanged
    {  
        public ObservableCollection<InventarioReparoLeituraEtiquetaViewModel> ObsInventarioReparoLeituraEtiqueta { get; set; }
        public ParametrosInventarioReparo _parametrosInventarioReparo { get; set; }
        
        public InventarioReparoLeituraEtiquetaListaPage(ParametrosInventarioReparo parametrosInventarioReparo)
        {
            InitializeComponent();

            _parametrosInventarioReparo = parametrosInventarioReparo;
            //_listaLeituraEtiquetaInventarioReparo = listaLeituraEtiquetaInventarioReparo;

            lblCodEstabel.Text = "Estab: " + SecurityAuxiliar.GetCodEstabel() + "  Técnico: " + _parametrosInventarioReparo.CodEstabel +
                "  Depós: " + _parametrosInventarioReparo.CodDepos + "  Dt Inventário: " + _parametrosInventarioReparo.DtInventario;

            ObsInventarioReparoLeituraEtiqueta = new ObservableCollection<InventarioReparoLeituraEtiquetaViewModel>();

            //if (listaLeituraEtiquetaInventarioReparo != null)
            //{
            //    foreach (var item in listaLeituraEtiquetaInventarioReparo)
            //    {
            //        var modelView = Mapper.Map<LeituraEtiquetaInventarioReparo, InventarioReparoLeituraEtiquetaViewModel>(item);
            //        ObsInventarioReparoLeituraEtiqueta.Add(modelView);
            //    }
            //}
            //
            cvInventarioReparoLeituraEtiqueta.BindingContext = this;


           //ObsInventarioReparoLeituraEtiqueta.Add(new InventarioReparoLeituraEtiquetaViewModel
           //{
           //    //CodEstabel = leituraEtiqueta.Param.Resultparam[0].CodEstabel,
           //    CodProduto = "",
           //    CodEstabel = "",
           //    Localiza = "",
           //    Digito = 8,
           //    NumRR = 123,
           //    Situacao = "",
           //    DescProduto = "",
           //    CodFilial = "01",
           //    Mensagem = "Erro Leitura Etiqueta",
           //    CodBarras = "213213"
           //
           //});

            OnPropertyChanged("ObsInventarioReparoLeituraEtiqueta");
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new InventarioReparoListaPage(null));

            return true;
        }

        private void BtnVoltarLeituraEtiqueta_Clicked(object sender, System.EventArgs e)
        {
            OnBackButtonPressed();
        }

        async void BtnLeituraEtiqueta_Clicked(object sender, System.EventArgs e)
        {
            BtnLeituraEtiqueta.IsEnabled = false;
            try
            {
                var page = new InventarioReparoLeituraEtiquetaBarrasPopUp();
                page._confirmaLeituraEtiqueta = ConfirmaLeituraEtiqueta;
                await PopupNavigation.Instance.PushAsync(page);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "OK");
            }
            finally
            {
                BtnLeituraEtiqueta.IsEnabled = true;
            }
        }

        async void BtnDigitacaoManual_Click(object sender, System.EventArgs e)
        {

            BtnDigitacaoManual.IsEnabled = false;
            try
            {
                var page = new InventarioReparoLeituraEtiquetaManualPopUp();
                page._confirmaLeituraEtiqueta = ConfirmaLeituraEtiqueta;
                await PopupNavigation.Instance.PushAsync(page);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "OK");
            }
            finally
            {
                BtnDigitacaoManual.IsEnabled = true;
            }
        }

        public async void ConfirmaLeituraEtiqueta(LeituraEtiquetaInventarioReparo pLeituraReparo)
        {
            var leituraEtiqueta = await LeituraEtiquetaInventarioReparoService.SendParametersAsync(_parametrosInventarioReparo, pLeituraReparo);

            if (leituraEtiqueta != null && leituraEtiqueta.Param != null && leituraEtiqueta.Param.Resultparam != null && leituraEtiqueta.Param.Resultparam.Count > 0)
            {
                if (!String.IsNullOrEmpty(leituraEtiqueta.Param.Resultparam[0].Mensagem))
                {
                    await DisplayAlert("ERRO!", leituraEtiqueta.Param.Resultparam[0].Mensagem, "OK");

                    ObsInventarioReparoLeituraEtiqueta.Add(new InventarioReparoLeituraEtiquetaViewModel
                    {
                        //CodEstabel = leituraEtiqueta.Param.Resultparam[0].CodEstabel,
                        CodProduto = leituraEtiqueta.Param.Resultparam[0].CodProduto,
                        CodEstabel = leituraEtiqueta.Param.Resultparam[0].CodEstabel,
                        Localiza = leituraEtiqueta.Param.Resultparam[0].Localiza,
                        Digito = leituraEtiqueta.Param.Resultparam[0].Digito,
                        NumRR = leituraEtiqueta.Param.Resultparam[0].NumRR,
                        Situacao = leituraEtiqueta.Param.Resultparam[0].Situacao,
                        DescProduto = leituraEtiqueta.Param.Resultparam[0].DescProduto,
                        CodFilial = leituraEtiqueta.Param.Resultparam[0].CodFilial,
                        Mensagem = leituraEtiqueta.Param.Resultparam[0].Mensagem,
                        CodBarras = leituraEtiqueta.Param.Resultparam[0].CodBarras

                    });

                    OnPropertyChanged("ObsInventarioReparoLeituraEtiqueta");

                    return;
                }
                else
                {
                    ObsInventarioReparoLeituraEtiqueta.Add(new InventarioReparoLeituraEtiquetaViewModel
                    {
                        //CodEstabel = leituraEtiqueta.Param.Resultparam[0].CodEstabel,
                        CodProduto = leituraEtiqueta.Param.Resultparam[0].CodProduto,
                        CodEstabel = leituraEtiqueta.Param.Resultparam[0].CodEstabel,
                        Localiza = leituraEtiqueta.Param.Resultparam[0].Localiza,
                        Digito = leituraEtiqueta.Param.Resultparam[0].Digito,
                        NumRR = leituraEtiqueta.Param.Resultparam[0].NumRR,
                        Situacao = leituraEtiqueta.Param.Resultparam[0].Situacao,
                        DescProduto = leituraEtiqueta.Param.Resultparam[0].DescProduto,
                        CodFilial = leituraEtiqueta.Param.Resultparam[0].CodFilial,
                        Mensagem = leituraEtiqueta.Param.Resultparam[0].Mensagem,
                        CodBarras = leituraEtiqueta.Param.Resultparam[0].CodBarras

                    });

                    OnPropertyChanged("ObsInventarioReparoLeituraEtiqueta");
                }
            }
            /*
            if (leituraEtiqueta.Retorno == "OK")
            {
                if (ListaLeituraEtiquetaInventarioReparo == null)
                    ListaLeituraEtiquetaInventarioReparo = new List<LeituraEtiquetaInventarioReparo>();

                foreach (var item in leituraEtiqueta.Param.Resultparam)
                    ListaLeituraEtiquetaInventarioReparo.Add(item);

                _ = DisplayAlert("", "Leitura de etiqueta manual efetuada com sucesso !!!", "OK");
                Application.Current.MainPage = new NavigationPage(new InventarioReparoLeituraEtiquetaListaPage(ListaLeituraEtiquetaInventarioReparo, parametrosInventarioReparo));
            }
            else
                _ = DisplayAlert("", "Erro ao realizar a leitura da etiqueta manual !!!", "OK");
            */
        }
    }

    public class InventarioReparoLeituraEtiquetaViewModel : LeituraEtiquetaInventarioReparo
    {

        public bool ItemIsVisible
        {
            get
            {
                return !string.IsNullOrEmpty(CodProduto);
            }
        }

        public bool LocalizacaoIsVisible
        {
            get
            {
                return !string.IsNullOrEmpty(Localiza);
            }
        }

        public bool NumRRIsVisible
        {
            get
            {
                return NumRR == 0 ? false : true;
            }
        }

        public bool DigitoIsVisible
        {
            get
            {
                return Digito == 0 ? false : true;
            }
        }

        public bool CodFilialIsVisible
        {
            get
            {
                return !string.IsNullOrEmpty(CodFilial);
            }
        }

        public bool MensagemIsVisible
        {
            get
            {
                return !string.IsNullOrEmpty(Mensagem);
            }
        }                           
    }
}