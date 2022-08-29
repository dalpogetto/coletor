﻿using AutoMapper;
using CollectorQi.Models.ESCL018;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper.ESCL018;
using CollectorQi.Services.ESCL018;
using CollectorQi.VO.ESCL018;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
//using Android.Graphics;

namespace CollectorQi.Views
{
    public partial class InventarioCaixaIncompletaPopUp : PopupPage
    {
        private int                     _inventarioId { get; set; }
        private InventarioItemVO        _inventarioItemVO;

        public Action<InventarioItemVO> _actDeleteRow;
        public Action<InventarioItemVO> _actRefreshPage;

        public InventarioCaixaIncompletaPopUp(int pInventarioId, InventarioItemVO pInventarioItem)        
        {
            try
            {
                InitializeComponent();

                _inventarioItemVO = pInventarioItem;
                _inventarioId     = pInventarioId;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        protected async override void OnAppearing()
        {
            var inventario = await InventarioDB.GetInventario(_inventarioId);

            edtItCodigo.Text           = _inventarioItemVO.CodItem.ToString();
            edtCodEstabelecimento.Text = _inventarioItemVO.CodEstabel + " - " + inventario.DescEstabel;
            edtCodDeposito.Text        = _inventarioItemVO.CodDepos + " - " + inventario.DescDepos;
            edtCodigoBarras.Text       = _inventarioItemVO.CodigoBarras;
            edtLote.Text               = _inventarioItemVO.Lote;

            if (_inventarioItemVO.Quantidade > 0)
            {
                txtQuantidade.Text = _inventarioItemVO.Quantidade.ToString();
            }

            if (String.IsNullOrEmpty(_inventarioItemVO.Lote))
            {
                FrameLote.IsVisible = false;
            }

            await Task.Run(async () =>
            { 
                await Task.Delay(100);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (String.IsNullOrEmpty(edtCodigoBarras.Text))
                    {
                        edtCodigoBarras.Focus();
                    }
                    else
                    {
                        txtQuantidade.Focus();
                    }
                });
            });
        }
       
        void BtnLimpar_Clicked(object sender, EventArgs e)
        {
            txtQuantidade.Text = string.Empty;
            edtCodigoBarras.Text = string.Empty;
        }
        protected override bool OnBackButtonPressed()
        {            
            PopupNavigation.Instance.PopAsync();
            return true;
        }

        private void BtnQuantidadeSum1_Clicked(object sender, EventArgs e)
        {
            SumButton(txtQuantidade, 1);
        }
        private void BtnQuantidadeSum10_Clicked(object sender, EventArgs e)
        {
            SumButton(txtQuantidade, 10);
        }
        private void BtnQuantidadeSum100_Clicked(object sender, EventArgs e)
        {
            SumButton(txtQuantidade, 100);
        }

        private void SumButton(CustomEntry entry, int value)
        {
            if (String.IsNullOrEmpty(entry.Text)) { entry.Text = "0"; }

            entry.Text = (int.Parse(entry.Text) + value).ToString();
        }

        async void OnClick_Efetivar(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtQuantidade.Text))
            {
                await DisplayAlert("Erro!", "Informe a quantidade do produto disponivel para a contagem do inventário", "Cancelar");
                return;
            }


            if (string.IsNullOrEmpty(edtCodigoBarras.Text))
            {
                await DisplayAlert("Erro!", "Informe o código de barras", "Cancelar");
                return;
            }

            var result = await DisplayAlert("Confirmação!", $"Deseja concluír a digitação com a quantidade de {txtQuantidade.Text} produto?", "Sim", "Não");

            var pageProgress = new ProgressBarPopUp("Carregando...");
            try
            {
                BtnEfetivar.IsEnabled = false;
                if (result.ToString() == "True")
                {
                    if (!string.IsNullOrEmpty(txtQuantidade.Text))
                    {

                        var inventarioBarra = new InventarioItemBarra()
                        {
                            IdInventario       = _inventarioItemVO.InventarioId,
                            Lote               = _inventarioItemVO.Lote.Trim(),
                            Localizacao        = _inventarioItemVO.Localizacao.Trim(),
                            CodItem            = _inventarioItemVO.CodItem.Trim(),
                            CodDepos           = _inventarioItemVO.CodDepos.Trim(),
                            QuantidadeDigitada = int.Parse(txtQuantidade.Text),
                            CodEmp             = "1",
                            Contagem           = _inventarioItemVO.Contagem,
                            CodEstabel         = _inventarioItemVO.CodEstabel,
                            CodigoBarras       = edtCodigoBarras.Text.Trim()
                        };

                        _inventarioItemVO.Quantidade = int.Parse(txtQuantidade.Text);
                        _inventarioItemVO.CodigoBarras = edtCodigoBarras.Text;

                        var resultService = await ParametersLeituraEtiquetaService.SendInventarioAsync(inventarioBarra, _inventarioItemVO, 0 , this);

                        if (resultService != null && resultService.Retorno != null)
                        {
                            if (resultService.Retorno == "OK")
                            {
                                _actDeleteRow(_inventarioItemVO);

                                await pageProgress.OnClose();
                                OnBackButtonPressed();
                            }
                            else if (resultService.Retorno == "IntegracaoBatch")
                            {
                                await DisplayAlert("Atenção!", "Erro de conexão com ERP, a atualização do item será integrado de forma Offline", "OK");
                                
                                _inventarioItemVO.StatusIntegracao = eStatusInventarioItem.ErroIntegracao;
                                _actRefreshPage(_inventarioItemVO);

                                await pageProgress.OnClose();
                                OnBackButtonPressed();
                            }
                            else {
                                if (resultService.Resultparam != null && resultService.Resultparam.Count > 0)
                                {
                                    await DisplayAlert("Erro!", resultService.Resultparam[0].ErrorDescription + " - " + resultService.Resultparam[0].ErrorHelp, "Cancelar");
                                }
                                else 
                                {
                                    await DisplayAlert("Erro!", "Erro na efetivação do inventário", "Cancelar");
                                }
                            }
                        }
                        else
                        {
                            await DisplayAlert("Erro!", "Erro na efetivação do inventário", "Cancelar");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                BtnEfetivar.IsEnabled = true;
                await pageProgress.OnClose();
            }
        }
    }
}