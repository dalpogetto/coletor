﻿using AutoMapper;
using CollectorQi.Models.ESCL018;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Services.ESCL018;
using CollectorQi.VO;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using Xamarin.Forms;
//using Android.Graphics;

namespace CollectorQi.Views
{
    public partial class InventarioUpdateItemPopUp : PopupPage
    {
        public ObservableCollection<InventarioItemViewModel> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                OnPropertyChanged("Items");
            }
        }

        //private int             _inventarioId = 0;
        private VO.InventarioVO _inventarioVO;
        private VO.InventarioItemVO _inventarioItemVO;
        private ObservableCollection<InventarioItemViewModel> _Items;
        public Action<VO.InventarioItemVO,bool> ResultAction;
        private string localizacao;

        public InventarioUpdateItemPopUp(int pInventarioId, string _localizacao, ObservableCollection<InventarioItemViewModel> _Items)        
        {
            try
            {
                InitializeComponent();

                Items = _Items;

                _inventarioVO = InventarioDB.GetInventario(pInventarioId).Result;

                edtInventario.Text = "65.111.03319-6";
                edtCodEstabelecimento.Text = _inventarioVO.CodEstabel;
                edtCodDeposito.Text = _inventarioVO.CodDepos;
                edtDtSaldo.Text = _inventarioVO.DtInventario.ToString("dd/MM/yy");
                localizacao = _localizacao;


                //if (pInventarioItemId > 0)
                //{
                //    _inventarioItemVO = InventarioItemDB.GetInventarioItem(pInventarioItemId);

                //    if (_inventarioItemVO != null)
                //    {
                //        edtItCodigo.Text = _inventarioItemVO.CodRefer;
                //        edtDescItem.Text = _inventarioItemVO.CodLocaliz;
                //    }

                //    //if (_inventarioItemVO != null)
                //    //{
                //    //    edtItCodigo.Text = _inventarioItemVO.ItCodigo;
                //    //    edtDescItem.Text = _inventarioItemVO.__item__.DescItem;
                //    //    edtUnidade.Text = _inventarioItemVO.__item__.Un;
                //    //    edtTipoConEst.Text = _inventarioItemVO.__item__.__TipoConEst__;
                //    //    edtLote.Text = _inventarioItemVO.CodLote;
                //    //    edtDtValiLote.Text = _inventarioItemVO.DtUltEntr.HasValue ? _inventarioItemVO.DtUltEntr.Value.ToString("dd/MM/yyyy") : String.Empty;
                //    //    edtQuantidade.Text = _inventarioItemVO.QtdDigitada ? _inventarioItemVO.ValApurado.ToString() : String.Empty;
                //    //    edtQuantidade.Focus();
                //    //}
                //}
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
            edtLote.Text = pLote;
            edtDtValiLote.Text = pDtValiLote;
        }

        public void SetResultDigita(Action<VO.InventarioItemVO, bool> dp)
        {
            ResultAction = dp;
        }
        
        async void OnClick_Cancelar(object sender, EventArgs e)
        {
            //BtnCancelar.IsEnabled = false;
            //try
            //{
            //    bool execProc = false;

            //    var result = await DisplayAlert("Atenção!", "Confirma o cancelamento da quantidade no inventário? O item não será integrado com o sistema", "Sim", "Não");

            //    if (result.ToString() == "True")
            //        execProc = true;

            //    if (execProc)
            //    {
            //        if (_inventarioItemVO == null)
            //        {
            //            DateTime? dtValiLote = null;

            //            if (!String.IsNullOrEmpty(edtDtValiLote.Text))
            //                dtValiLote = new DateTime(int.Parse(edtDtValiLote.Text.Substring(6)),
            //                                          int.Parse(edtDtValiLote.Text.Substring(3, 2)),
            //                                          int.Parse(edtDtValiLote.Text.Substring(0, 2)));


            //            InventarioItemDB.InserirInventarioItem(new VO.InventarioItemVO
            //            {
            //                InventarioId = _inventarioVO.InventarioId,
            //                CodLocaliz = String.Empty,
            //                CodLote = edtLote.Text,
            //                //ItCodigo = edtItCodigo.Text,
            //                DtUltEntr = dtValiLote,
            //                CodRefer = String.Empty,
            //                NrFicha = 0,
            //                ValApurado = 0,
            //                QtdDigitada = false,
            //            });
            //        }
            //        else
            //        {

            //            InventarioItemDB.ConfirmaQuantidadeDigitada(_inventarioItemVO.InventarioItemId, false);

            //            _inventarioItemVO.ValApurado = 0;

            //            InventarioItemDB.AtualizaQuantidadeInventario(_inventarioItemVO);
            //        }

            //        // Se nao tem nada digitado, marca como Nao Inciado 
            //        if (InventarioItemDB.GetInventarioItemDigitadoByInventarioId(_inventarioVO.InventarioId).Count <= 0)
            //        {
            //            InventarioDB.EfetivaInventarioMobile(_inventarioVO.InventarioId, eStatusInventario.NaoIniciado);
            //        }

            //        ResultAction(_inventarioItemVO, false);
                    
            //        await PopupNavigation.Instance.PopAsync();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    await DisplayAlert("Erro!", ex.Message, "Cancelar");
            //}
            //finally
            //{
            //    BtnCancelar.IsEnabled = true;
            //}
        }

        protected override bool OnBackButtonPressed()
        {            
            PopupNavigation.Instance.PopAsync();
            return true;
        }

        async void OnClick_Efetivar(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(edtQuantidade.Text))
            {
                await DisplayAlert("Erro!", "Quantidade digitada inválida", "Cancelar");

                edtQuantidade.Focus();

                return;
            }

            
            var param = new ParametersItemLeituraEtiquetaService();

            var inventario = new Inventario()
            {
                IdInventario = _inventarioVO.InventarioId,
                CodEstabel = _inventarioVO.CodEstabel,
                CodDepos = _inventarioVO.CodDepos,
                Localizacao = localizacao,
                Lote = "",
                QuantidadeDigitada = int.Parse(edtQuantidade.Text),
                CodigoBarras = "02[65.116.00709-1[1[2[3[4[5[6[1[8"  // receber do leitor
            };

            var _inventarioItem = await param.SendInventarioAsync(inventario);

            var filtroReturn = Items.FirstOrDefault(x => x.CodRefer == _inventarioItem.paramConteudo.Resultparam[0].CodItem);
            filtroReturn.Quantidade += int.Parse(edtQuantidade.Text);

            foreach (var item in Items)
            {
                if (item.CodRefer == _inventarioItem.paramConteudo.Resultparam[0].CodItem)
                {
                    Items.Remove(item);
                    break;
                }
            }

            var modelView = Mapper.Map<InventarioItemVO, InventarioItemViewModel>(filtroReturn);
            Items.Add(modelView);

            //await PopupNavigation.Instance.PushAsync(new ProgressBarPopUp("Carregando..."));
            var pageProgress = new ProgressBarPopUp("Carregando... !");
            Application.Current.MainPage = new NavigationPage(new InventarioListaItemPage(_inventarioVO, Items, localizacao));
            await pageProgress.OnClose();  

            OnBackButtonPressed();


            //if (String.IsNullOrEmpty(edtQuantidade.Text))
            //{
            //    await DisplayAlert("Erro!", "Quantidade digitada inválida", "Cancelar");

            //    edtQuantidade.Focus();

            //    return; 
            //}

            //BtnEfetivar.IsEnabled = false;
            //try
            //{
            //    bool execProc = false;

            //    if (_inventarioItemVO != null && _inventarioItemVO.QtdDigitada)
            //    {
            //        var result = await DisplayAlert("Atenção!", "Item já digitado com a quantidade (" + _inventarioItemVO.ValApurado + "). Confirma a alteração do inventário? " + Environment.NewLine + "VERIFICAR SE O ITEM NÃO ESTÁ EM OUTRA LOCALIDADE", "Sim", "Não");

            //        if (result.ToString() == "True")
            //            execProc = true;
            //        else
            //            execProc = false;
            //    }
            //    else if (_inventarioVO != null)
            //        execProc = true;


            //    if (execProc)
            //    {

            //        if (decimal.Parse(edtQuantidade.Text) <= 0)
            //        {
            //            var result = await DisplayAlert("Atenção!", "Quantidade informada 0! O saldo em estoque será eliminado no sistema, deseja continuar?", "Sim", "Não");

            //            if (result.ToString() == "True")
            //                execProc = true;
            //            else
            //                execProc = false;

            //        }

            //        if (execProc)
            //        {
            //            if (_inventarioVO != null && _inventarioItemVO == null)
            //            {
            //                DateTime? dtValiLote = null;

            //                if (!String.IsNullOrEmpty(edtDtValiLote.Text))
            //                    dtValiLote = new DateTime(int.Parse(edtDtValiLote.Text.Substring(6)),
            //                                              int.Parse(edtDtValiLote.Text.Substring(3, 2)),
            //                                              int.Parse(edtDtValiLote.Text.Substring(0, 2)));


            //                _inventarioItemVO = InventarioItemDB.InserirInventarioItem(new VO.InventarioItemVO
            //                {
            //                    InventarioId = _inventarioVO.InventarioId,
            //                    CodLocaliz = String.Empty,
            //                    CodLote = edtLote.Text,
            //                    DtUltEntr = dtValiLote,
            //                    //ItCodigo = edtItCodigo.Text,
            //                    CodRefer = String.Empty,
            //                    NrFicha = 0,
            //                    ValApurado = decimal.Parse(edtQuantidade.Text.Replace(".", ",")),

            //                    QtdDigitada = true,
            //                });
            //            }
            //            else
            //            {

            //                _inventarioItemVO.ValApurado = decimal.Parse(edtQuantidade.Text.Replace(".",","));

            //                InventarioItemDB.AtualizaQuantidadeInventario(_inventarioItemVO);
            //                InventarioItemDB.ConfirmaQuantidadeDigitada(_inventarioItemVO.InventarioItemId, true);

            //                //InventarioDB.EfetivaInventarioMobile(_inventarioVO.InventarioId, eStatusInventario.IniciadoMobile);
            //            }

            //            InventarioDB.EfetivaInventarioMobile(_inventarioVO.InventarioId, eStatusInventario.IniciadoMobile);

            //            ResultAction(_inventarioItemVO, true);

            //            await DisplayAlert("Item Inventário", "Item do inventário atualizado com sucesso", "OK");


            //            await PopupNavigation.Instance.PopAsync();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    await DisplayAlert("Erro!", ex.Message, "Cancelar");
            //}
            //finally
            //{
            //    BtnEfetivar.IsEnabled = true;
            //}
        }
    }
}