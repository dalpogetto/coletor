﻿using CollectorQi.Models;
using CollectorQi.ViewModels;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Resources.DataBaseHelper.Batch;
using CollectorQi.Resources;
using CollectorQi.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CollectorQi.Models.Datasul;
using AutoMapper;
using System.Globalization;
using Plugin.Connectivity;
using CollectorQi.Services.ESCL018;
using ESCL = CollectorQi.Models.ESCL018;
using CollectorQi.Models.ESCL018;
using Rg.Plugins.Popup.Services;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeituraEtiquetaLocaliza : ContentPage, INotifyPropertyChanged
    {
        #region Property

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

        public ObservableCollection<InventarioViewModel> ObsInventario { get; }
        public InventarioVO pInventarioVO { get; }
        public string localizacaoRetorno { get; set; }

        public LeituraEtiquetaLocaliza(InventarioVO inventarioVO)
        {
            InitializeComponent();

            //_ = InventarioItemDB.DeletarInventarioByInventarioId(inventarioVO.InventarioId);
            
            pInventarioVO = inventarioVO;
        }      

        async void OnClick_BuscaEtiqueta(object sender, System.EventArgs e)
        {
            var pageProgress = new ProgressBarPopUp("Carregando Consulta de Etiqueta...");
            await PopupNavigation.Instance.PushAsync(pageProgress);

            try
            {
                var inventario = new Inventario()
                {
                    IdInventario = pInventarioVO.InventarioId,
                    CodEstabel = pInventarioVO.CodEstabel,
                    CodDepos = pInventarioVO.CodDepos,
                    CodigoBarras = txtEtiqueta.Text
                };

                var localizacao = new ParametersLocalizacaoLeituraEtiquetaService();
                var localizacaoResult = await localizacao.SendInventarioAsync(inventario);
                localizacaoRetorno = localizacaoResult.Resultparam.Localizacao;
            }
            catch (Exception ex)
            {
                var pageProgressErro = new ProgressBarPopUp("Erro: " + ex.Message);
                await PopupNavigation.Instance.PushAsync(pageProgressErro);
                await pageProgressErro.OnClose();
            }
            finally
            {
                await pageProgress.OnClose();
            }           
        }

        async void OnClick_Proximo(object sender, System.EventArgs e)
        {
            var parametersFichasUsuario = new ParametersFichasUsuarioService();
            var lstInventarioVO = await parametersFichasUsuario.GetObterFichasUsuarioAsync();

            //var lstInventarioVOFiltro = lstInventarioVO.param.Resultparam.Where(x => x.Localizacao == localizacaoRetorno);

            foreach (var item in lstInventarioVO.param.Resultparam.Where(x => x.Localizacao == localizacaoRetorno))
            {
                InventarioItemVO inventarioItem = new InventarioItemVO();
                inventarioItem.InventarioId = item.IdInventario;
                inventarioItem.CodLote = item.Lote;
                inventarioItem.CodLocaliz = item.Localizacao;
                inventarioItem.CodRefer = item.CodItem;
                inventarioItem.NrFicha = item.Quantidade;

                InventarioItemDB.InserirInventarioItem(inventarioItem);
            }

            Application.Current.MainPage = new NavigationPage(new InventarioListaItemPage(pInventarioVO));
        } 

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new PrincipalPage());

            return true;
        }
    }
}