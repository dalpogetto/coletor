﻿using AutoMapper;
using CollectorQi.Models.ESCL018;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Services.ESCL018;
using CollectorQi.ViewModels;
using CollectorQi.VO;
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
        public InventarioPrintPopUp()        
        {
            try
            {
                InitializeComponent();

                string[] imagem = new string[] { "product.png", "forklift.png", "repair.png" };
                string[] titulo = new string[] {  "Item", "Localização", "Reparo"};

                List<MenuItemDetail> menuItemDetails = new List<MenuItemDetail>();
                MenuItemDetail menuItemDetail;

                for (int i = 0; i < imagem.Count(); i++)
                {
                    menuItemDetail = new MenuItemDetail { MenuItemDatailId = i + 1, Name = titulo[i], Image = imagem[i] };
                    menuItemDetails.Add(menuItemDetail);
                }

                listView.ItemsSource = menuItemDetails;
                listView.ItemSelected += OnSelection;

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

        public void SetResultDigita(Action<VO.InventarioItemVO, bool> dp)
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

                        var pageItem = new InventarioPrintItem();
                        await PopupNavigation.Instance.PushAsync(pageItem);

                        break;

                    case "Localização":

                        var pageLocalizacao = new InventarioPrintLocalizacao();
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