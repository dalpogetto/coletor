using Android.App;
using Android.Views;
using AutoMapper;
using CollectorQi.Models.ESCL021;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper.ESCL018;
using CollectorQi.Services.ESCL021;
using CollectorQi.VO.ESCL018;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    public partial class InventarioListaCodigoBarrasPopUp : PopupPage  , INotifyPropertyChanged 
    {

        #region Property

        new public event PropertyChangedEventHandler PropertyChanged;

        new protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
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

        public ObservableCollection<InventarioItemCodigoBarrasVO> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                OnPropertyChanged("Items");
            }
        }

        private int QtdItem
        {
            get
            {
                if (Items != null & Items.Count > 0)
                {
                    return Items.Count;
                }

                return 0;
            }
        }

        private decimal QtdVolume
        {
            get
            {
                if (Items != null && Items.Count > 0)
                {
                    decimal qtdVolume = 0;
                    foreach(var row in Items)
                    {
                        qtdVolume += row.Quantidade;
                    }
                    return qtdVolume;
                }
                return 0;
            }
        }

        private ObservableCollection<InventarioItemCodigoBarrasVO> _Items;

        private List<InventarioItemViewModel> _lstInventarioItem;

        public Action<InventarioItemVO, bool, string> _actRefreshPage;


        public InventarioListaCodigoBarrasPopUp(List<InventarioItemViewModel> byLstInventarioitem)
        {
            InitializeComponent();

            _lstInventarioItem = byLstInventarioitem;

            cvCodigoBarras.BindingContext = this;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

           // Items = new ObservableCollection<InventarioItemCodigoBarrasVO>();

            CarregaListView();
        }

        private async void CarregaListView()
        {
            try
            {
                //var lstInventarioItem = InventarioItemCodigoBarrasDB.GetByLocalizacao(_idInventario);

                List<InventarioItemCodigoBarrasVO> lstCurrentBarras = new List<InventarioItemCodigoBarrasVO>();
                if (_lstInventarioItem != null && _lstInventarioItem.Count > 0)
                {
                    foreach (var row in _lstInventarioItem)
                    {
                        var resultBarras = InventarioItemCodigoBarrasDB.GetByInventarioItemKey(row.InventarioItemKey);

                        if (resultBarras != null)
                        {
                            lstCurrentBarras.AddRange(resultBarras);
                        }
                    }
                }

                Items = new ObservableCollection<InventarioItemCodigoBarrasVO>(lstCurrentBarras);

                OnPropertyChangeCustom();
            }
            finally
            {
            }
        }

        private void OnPropertyChangeCustom()
        {
            OnPropertyChanged("Items");
     
            if (Items != null & Items.Count > 0)
            {
                lblQtdItemValue.Text = (from x in Items select x.InventarioItemKey).Distinct().Count().ToString();
            }
            else
            {
                lblQtdItemValue.Text = "0";
            }

            if (Items != null && Items.Count > 0)
            {
                lblQtdVolumeValue.Text = Items.Sum(x => x.Quantidade).ToString();
            }
            else
            {
                lblQtdVolumeValue.Text = "0";
            }
        }

        protected override bool OnBackButtonPressed()
        {
            PopupNavigation.Instance.PopAsync();
            
            return true;
        }

        private async void cvCodigoBarras_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cvCodigoBarras.SelectedItem == null)
                return;

            try
            {
                var current = (cvCodigoBarras.SelectedItem as InventarioItemCodigoBarrasVO);

                if (current != null)
                {
                    var resultDisplay = await DisplayAlert("Confirmação!", "Deseja eliminar a contagem (" + current.CodigoBarras + ")", "Sim", "Não");
                    if (resultDisplay.ToString() == "True")
                    {
                        var resultDel = InventarioItemCodigoBarrasDB.DeleteCodigoBarras(current.InventarioItemCodigoBarrasId);

                        if (resultDel)
                        {
                            _actRefreshPage(null, false, current.InventarioItemKey);

                            Items.Remove(current);

                            OnPropertyChangeCustom();
                        }
                    }
                }
            }
            finally
            {
                cvCodigoBarras.SelectedItem = null;
            }
        }
    }

    public class InventarioItemCodigoBarrasView : InventarioItemCodigoBarrasVO
    {
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
    }
}