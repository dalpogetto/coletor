﻿using AutoMapper;
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
    public partial class SaldoVirtualLocalizPopUp : PopupPage
    {
        public Action<string, string> _confirmaLocalizacaoItem { get; set; }
        private string _codDepos { get; set; }

        public SaldoVirtualLocalizPopUp(string pCodDepos)        
        {
            try
            {
                InitializeComponent();

                _codDepos = pCodDepos;
            }
            catch (Exception ex)
            {
                throw;
            } 
        }

        protected async override void OnAppearing()
        {
            await Task.Run(async () =>
            {
                await Task.Delay(100);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    edtCodBarras.Focus();
                });
            });
        }
        void OnClick_Cancelar(object sender, EventArgs e)
        {
            OnBackButtonPressed();
        }

        protected override bool OnBackButtonPressed()
        {            
            PopupNavigation.Instance.PopAsync();
            return true;
        }

        void OnClick_Confirmar(object sender, EventArgs e)
        {
            try
            {
                BtnEfetivar.IsEnabled = false;

                _confirmaLocalizacaoItem(_codDepos, edtCodBarras.Text);
                PopupNavigation.Instance.PopAsync();
            }
            finally
            {
                BtnEfetivar.IsEnabled = true;
            }
        }

        private void edtLocalizacao_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                if (String.IsNullOrEmpty(e.OldTextValue) && e.NewTextValue.Length > 5)
                {
                    BtnEfetivar.IsEnabled = false;

                    _confirmaLocalizacaoItem(_codDepos, edtCodBarras.Text);
                    PopupNavigation.Instance.PopAsync();
                }
            }
            finally
            {
                BtnEfetivar.IsEnabled = true;
            }
        }

        public void OnClose()
        {
            try
            {
                PopupNavigation.Instance.PopAsync();
            }
            catch { }
        }
    }
}