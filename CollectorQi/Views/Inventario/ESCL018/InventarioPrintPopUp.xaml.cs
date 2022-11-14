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
    public partial class InventarioPrintPopUp : PopupPage
    {
        private string _codDepos { get; set; }
        private string _codLocaliz { get; set; }
        private string _itCodigo { get; set; }

        private List<string> _lstItem { get; set; }
        
        public InventarioPrintPopUp(string pCodDepos, string pCodLocaliz, string pItCodigo, List<string> pLstItem)        
        {
            try
            {
                InitializeComponent();

                _codDepos   = pCodDepos;
                _codLocaliz = pCodLocaliz;
                _itCodigo   = pItCodigo;
                _lstItem = pLstItem;

                string[] imagem = new string[] { "product.png", "forklift.png", "repair.png" };
                string[] titulo = new string[] {  "Item", "Localização", "Reparo"};

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

                           
                            if (_lstItem != null && _lstItem.Count > 0)
                            {
                                
                                string[] arrayItem = new string[_lstItem.Count + 1];

                                arrayItem[0] = "Nenhum item da lista...";

                                int iCont = 0;
                                foreach (var item in _lstItem)
                                {
                                    iCont++;
                                    arrayItem[iCont] = item.ToString();
                                }

                                var action = await DisplayActionSheet("Escolha o Item?", "Cancelar", null, arrayItem);

                                if (action != "Cancelar")
                                {
                                    if (action == "Nenhum item da lista...")
                                    {
                                        _itCodigo = "";
                                    }
                                    else
                                    {
                                        _itCodigo = action;
                                    }
                                }
                                else
                                    return;
                            }

                            var pageItem = new InventarioPrintItem(_codDepos, _itCodigo);
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

        /*else
            pageProgress = new ProgressBarPopUp("Digite uma quantidade !"); */


        //if (String.IsNullOrEmpty(edtQuantidade.Text))
        //{
        //    await DisplayAlert("Erro!", "Quantidade digitada inválida", "Cancelar");

        //    edtQuantidade.Focus();

        //    return;
        //}

        //var param = new ParametersItemLeituraEtiquetaService();

        //var inventario = new Inventario()
        //{
        //    IdInventario = _inventarioVO
        //    .InventarioId,
        //    CodEstabel = _inventarioVO.CodEstabel,
        //    CodDepos = _inventarioVO.CodDepos,
        //    Localizacao = localizacao,
        //    Lote = "",
        //    QuantidadeDigitada = int.Parse(edtQuantidade.Text),
        //    CodigoBarras = "02[65.116.00709-1[1[2[3[4[5[6[1[8"  // receber do leitor
        //};

        //var _inventarioItem = await param.SendInventarioAsync(inventario);

        //var filtroReturn = Items.FirstOrDefault(x => x.CodRefer == _inventarioItem.paramConteudo.Resultparam[0].CodItem);
        //filtroReturn.Quantidade += int.Parse(edtQuantidade.Text);

        //foreach (var item in Items)
        //{
        //    if (item.CodRefer == _inventarioItem.paramConteudo.Resultparam[0].CodItem)
        //    {
        //        Items.Remove(item);
        //        break;
        //    }
        //}

        //var modelView = Mapper.Map<InventarioItemVO, InventarioItemViewModel>(filtroReturn);
        //Items.Add(modelView);

        ////await PopupNavigation.Instance.PushAsync(new ProgressBarPopUp("Carregando..."));
        //var pageProgress = new ProgressBarPopUp("Carregando... !");
        //Application.Current.MainPage = new NavigationPage(new InventarioListaItemPage(_inventarioVO));
        //await pageProgress.OnClose();  

        //OnBackButtonPressed();                       

    }
}