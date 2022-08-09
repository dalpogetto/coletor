﻿using AutoMapper;
using CollectorQi.Models.ESCL017;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL017;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventarioReparoDepositoListaPage : ContentPage, INotifyPropertyChanged
    {  
        public ObservableCollection<DepositosInventarioReparoViewModel> ObsInventarioReparoDeposito { get; set; }
        public List<DepositosInventarioReparo> ListaDepositosInventarioReparo { get; set; }
        public ParametrosInventarioReparo ParametrosInventarioReparo { get; set; }

        public InventarioReparoDepositoListaPage(List<DepositosInventarioReparo> listaDepositosInventarioReparo, ParametrosInventarioReparo parametrosInventarioReparo)
        {
            InitializeComponent();
            ObsInventarioReparoDeposito = new ObservableCollection<DepositosInventarioReparoViewModel>();
            ListaDepositosInventarioReparo = new List<DepositosInventarioReparo>();
            ListaDepositosInventarioReparo = listaDepositosInventarioReparo;
            ParametrosInventarioReparo = parametrosInventarioReparo;

            lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.GetCodEstabel();

            if (listaDepositosInventarioReparo != null)
            {
                foreach (var item in listaDepositosInventarioReparo)
                {
                    var modelView = Mapper.Map<DepositosInventarioReparo, DepositosInventarioReparoViewModel>(item);
                    ObsInventarioReparoDeposito.Add(modelView);
                }
            }

            cvInventarioReparoDeposito.BindingContext = this;
        }  

        private void cvInventarioReparoDeposito_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cvInventarioReparoDeposito.SelectedItem == null)
                return;

            var current = (cvInventarioReparoDeposito.SelectedItem as DepositosInventarioReparo);

            Application.Current.MainPage = new NavigationPage(new InventarioReparoListaPage(current.Nome, ParametrosInventarioReparo));
        }
    }

    public class DepositosInventarioReparoViewModel : DepositosInventarioReparo
    {
        //public string Image
        //{
        //    get
        //    {
        //        if (this.ItensRestantes)
        //            return "intSucessoMed.png";
        //        else
        //            return "intPendenteMed.png";
        //    }
        //}
    }
}