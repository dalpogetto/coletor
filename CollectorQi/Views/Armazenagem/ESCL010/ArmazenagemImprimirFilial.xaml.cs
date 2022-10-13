using CollectorQi.Resources;
using CollectorQi.Services.ESCL006;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;

namespace CollectorQi.Views
{
    public partial class ArmazenagemImprimirFilial : PopupPage
    {
        public ArmazenagemImprimirFilial()
        {
            InitializeComponent();
            lblCodEstabel.Text = SecurityAuxiliar.GetCodEstabel();
        }

        protected override bool OnBackButtonPressed()
        {
            PopupNavigation.Instance.PopAsync();
            return true;
        }

        async void BtnImprimir_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtFilial.Text))
            {
                var imprimir = new ArmazenagemImprimirEtiquetaService();
                var imprimirResult = await imprimir.SendImpressaoAsync(txtFilial.Text);

                _ = DisplayAlert("", imprimirResult.Conteudo, "OK");
            }
            else
                _ = DisplayAlert("", "Digite uma Filial", "OK");
        }
    }
}