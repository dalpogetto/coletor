using CollectorQi.Models.ESCL017;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL017;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    public partial class InventarioReparoLeituraEtiquetaManualPopUp : PopupPage
    {

        public List<LeituraEtiquetaInventarioReparo> ListaLeituraEtiquetaInventarioReparo { get; set; }
        public ParametrosInventarioReparo parametrosInventarioReparo { get; set; }
        public Action<LeituraEtiquetaInventarioReparo> _confirmaLeituraEtiqueta;
        public InventarioReparoLeituraEtiquetaManualPopUp()
        {
            InitializeComponent();

            lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.GetCodEstabel();
        }
        protected override bool OnBackButtonPressed()
        {
            PopupNavigation.Instance.PopAsync();
            return true;
        }

        async void BtnEtiquetaManualConfirmar_Clicked(object sender, System.EventArgs e)
        {
            BtnEtiquetaManualConfirmar.IsEnabled = false;
            try
            {
                if (string.IsNullOrEmpty(txtFilial.Text) || string.IsNullOrEmpty(txtNumeroRR.Text) || string.IsNullOrEmpty(txtDigito.Text))
                    await DisplayAlert("", "Todos os campos são obrigatórios !", "OK");
                else
                {
                    var leituraReparo = new LeituraEtiquetaInventarioReparo();

                    leituraReparo.CodEstabel = SecurityAuxiliar.GetCodEstabel();
                    leituraReparo.CodFilial = txtFilial.Text;

                    if (!string.IsNullOrEmpty(txtNumeroRR.Text))
                        leituraReparo.NumRR = int.Parse(txtNumeroRR.Text);

                    if (!string.IsNullOrEmpty(txtDigito.Text))
                        leituraReparo.Digito = int.Parse(txtDigito.Text);

                    _confirmaLeituraEtiqueta(new LeituraEtiquetaInventarioReparo
                    {
                        CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                        CodFilial = txtFilial.Text,
                        NumRR = int.Parse(txtNumeroRR.Text),
                        Digito = int.Parse(txtDigito.Text)
                    });

                    await PopupNavigation.Instance.PopAsync();

                }
            }
            finally
            {
                BtnEtiquetaManualConfirmar.IsEnabled = true;
            }
        }
    }
}