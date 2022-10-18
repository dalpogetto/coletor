﻿using CollectorQi.Models.ESCL017;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL017;
using System;
using System.ComponentModel;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TransferenciaDepositoListaPage : ContentPage, INotifyPropertyChanged
    {
        public TransferenciaDepositoListaPage(ParametrosInventarioReparo parametrosInventarioReparo)
        {
            InitializeComponent();

            lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.GetCodEstabel();

            if (parametrosInventarioReparo != null)
            {
                txtDepositoSaida.Text = parametrosInventarioReparo.CodDepos;
                //...
            }                
        }               

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new InventarioPage());

            return true;
        }       

        async void BtnBuscaDepositoSaida_Clicked(object sender, System.EventArgs e)
        {
            var parametrosIR = new ParametrosInventarioReparo();
            parametrosIR.CodEstabel = SecurityAuxiliar.GetCodEstabel();

            //if(!string.IsNullOrEmpty(txtTecnico.Text))
            //    parametrosIR.CodTecnico = int.Parse(txtTecnico.Text);

            //parametrosIR.Senha = txtSenha.Text;
            //parametrosIR.CodDepos = txtDeposito.Text;
            //parametrosIR.DtInventario = txtData.Text;            

            //  var dInventario = new DepositoInventarioReparoService();
            var dInventarioRetorno = await DepositoInventarioReparoService.SendParametersAsync();

            Application.Current.MainPage = new NavigationPage(new InventarioReparoDepositoListaPage(dInventarioRetorno.Param.Resultparam, parametrosIR, "TransferenciaDeposito"));            
        }

        async void BtnBuscaDepositoEntrada_Clicked(object sender, System.EventArgs e)
        {
            var parametrosIR = new ParametrosInventarioReparo();
            parametrosIR.CodEstabel = SecurityAuxiliar.GetCodEstabel();

            //if(!string.IsNullOrEmpty(txtTecnico.Text))
            //    parametrosIR.CodTecnico = int.Parse(txtTecnico.Text);

            //parametrosIR.Senha = txtSenha.Text;
            //parametrosIR.CodDepos = txtDeposito.Text;
            //parametrosIR.DtInventario = txtData.Text;            

            //var dInventario = new DepositoInventarioReparoService();
            var dInventarioRetorno = await DepositoInventarioReparoService.SendParametersAsync();

            Application.Current.MainPage = new NavigationPage(new InventarioReparoDepositoListaPage(dInventarioRetorno.Param.Resultparam, parametrosIR, "TransferenciaDeposito"));            
        }


    }
}