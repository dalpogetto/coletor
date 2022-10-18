using AutoMapper;
using CollectorQi.Models.ESCL018;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Services.ESCL018;
using CollectorQi.ViewModels;
using CollectorQi.VO.ESCL018;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Xamarin.Forms;
//using Android.Graphics;

namespace CollectorQi.Views
{
    public partial class ArmazenagemPrintPopUp : PopupPage
    {
        private string _codDepos { get; set; }
        private string _codLocaliz { get; set; }
        

        public ArmazenagemPrintPopUp(string pCodDepos, string pCodLocaliz)        
        {
            try
            {
                InitializeComponent();

                _codDepos = pCodDepos;
                _codLocaliz = pCodLocaliz;

                string[] imagem = new string[] { "product.png", "forklift.png", "repair.png" /* ,   "estabelecimento.png"*/ };
                string[] titulo = new string[] {  "Item", "Localização", "Reparo" /*, "Filial" */};

                List<MenuItemDetail> menuItemDetails = new List<MenuItemDetail>();
                MenuItemDetail menuItemDetail;

                for (int i = 0; i < imagem.Count(); i++)
                {
                    menuItemDetail = new MenuItemDetail { MenuItemDatailId = i + 1, Name = titulo[i], Image = imagem[i] };
                    menuItemDetails.Add(menuItemDetail);
                }

                CvPrint.ItemsSource = menuItemDetails;
              //  listView.SelectedItem += OnSelection;

            }
            catch (Exception ex)
            {
                throw;
            } 
        }

        public void SetNovoItemInventario(string pItCodigo, string pDescItem, string pUnidade, string pTipoConEst, string pLote, string pDtValiLote)
        {
            //edtItCodigo.Text = pItCodigo;
            //edtDescItem.Text = pDescItem;
            //edtUnidade.Text = pUnidade;
            //edtTipoConEst.Text = pTipoConEst;
        //    edtLote.Text = pLote;
        //    edtDtValiLote.Text = pDtValiLote;
        }

        public void SetResultDigita(Action<InventarioItemVO, bool> dp)
        {
        }
        
        async void BtnVoltar_Clicked(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }

        protected override bool OnBackButtonPressed()
        {            
            PopupNavigation.Instance.PopAsync();
            return true;
        }

        async void OnClick_Efetivar(object sender, EventArgs e)
        {
        }

        async void OnSelection(object sender, SelectedItemChangedEventArgs e)
        {
            /*
            try
            {
                if (e.SelectedItem == null)
                {
                    return;
                }

                MenuItemDetail menuItemDetail = new MenuItemDetail();
                menuItemDetail = (MenuItemDetail)e.SelectedItem;

                switch (menuItemDetail.Name)
                {
                    case "Item":

                        var pageItem = new InventarioPrintItem(_codDepos);
                        await PopupNavigation.Instance.PushAsync(pageItem);

                        break;

                    case "Localização":

                        var pageLocalizacao = new InventarioPrintLocalizacao(_codDepos, _codLocaliz);
                        await PopupNavigation.Instance.PushAsync(pageLocalizacao);

                        break;


                    case "Reparo":

                        var pageReparo = new InventarioPrintReparo();
                        await PopupNavigation.Instance.PushAsync(pageReparo);

                        break;
                
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "CANCELAR");
            }
            finally
            {
                ((ListView)sender).SelectedItem = null;
                listView.IsEnabled = true;
            } */
        }

        private async void CvPrint_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            if (CvPrint.SelectedItem == null)
                return;

            var menuItemDetail = (CvPrint.SelectedItem as MenuItemDetail);

            if (menuItemDetail != null)
            {
                try
                {

                    switch (menuItemDetail.Name)
                    {
                        case "Item":

                            var pageItem = new InventarioPrintItem(_codDepos);
                            await PopupNavigation.Instance.PushAsync(pageItem);

                            break;

                        case "Localização":

                            var pageLocalizacao = new InventarioPrintLocalizacao(_codDepos, _codLocaliz);
                            await PopupNavigation.Instance.PushAsync(pageLocalizacao);

                            break;


                        case "Reparo":

                            var pageReparo = new InventarioPrintReparo();
                            await PopupNavigation.Instance.PushAsync(pageReparo);

                            break;

                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erro!", ex.Message, "CANCELAR");
                }
                finally
                {
                    CvPrint.SelectedItem = null;
                    CvPrint.IsEnabled = true;
                }
            }
        }
    }
}