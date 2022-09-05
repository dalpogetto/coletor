using AutoMapper;
using CollectorQi.Models.ESCL018;
using CollectorQi.Models.ESCL021;
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
using System.Threading.Tasks;
using Xamarin.Forms;
//using Android.Graphics;

namespace CollectorQi.Views
{
    public partial class GuardaMateriaisDepositoItemListaPagePopUp : PopupPage
    {
        public Action<string> _confirmaItem { get; set; }

        public GuardaMateriaisDepositoItemListaPagePopUp()        
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                throw;
            } 
        }

        void OnClick_Cancelar(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }
        protected async override void OnAppearing()
        {
            await Task.Run(async () =>
            {
                await Task.Delay(100);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    edtCodigoBarras.Focus();
                });
            });
        }
        protected override bool OnBackButtonPressed()
        {            
            PopupNavigation.Instance.PopAsync();
            return true;
        }

        async void OnClick_Confirmar(object sender, EventArgs e)
        {
            try
            {
                BtnEfetivar.IsEnabled = false;
                await PopupNavigation.Instance.PopAsync();
                _confirmaItem(edtCodigoBarras.Text);
            }
            finally
            {
                BtnEfetivar.IsEnabled = true;
            }
        }
    }
}