using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Rg.Plugins.Popup.Pages;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CollectorQi.Resources.DataBaseHelper;
using Rg.Plugins.Popup.Services;
using System.Linq;
using System.Threading.Tasks;
using CollectorQi.Resources;

namespace CollectorQi.Views
{
    public partial class ItemPopUp : PopupPage, INotifyPropertyChanged
    {

        public bool IsClick { get; set; }

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

        public ObservableCollection<ModelItem> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                OnPropertyChanged("Items");
            }
        }

        #endregion

        private CustomEntryText _edtItCodigo;
        private CustomEntryText _edtDescItem;
        private CustomEntryText _edtUn;
        private CustomEntryText _edtTipoConEst;
        private ObservableCollection<ModelItem> _Items;
        private ObservableCollection<ModelItem> _ItemsFiltered;
        private ObservableCollection<ModelItem> _ItemsUnfiltered;

        private Action<String> _actVerify;

        public ItemPopUp(CustomEntryText pEdtItCodigo, CustomEntryText pEdtDescItem, CustomEntryText pEdtUn, CustomEntryText pEdtTipoConEst, Action<String> pActVerify)
        {
            InitializeComponent();

            _edtItCodigo = pEdtItCodigo;
            _edtDescItem = pEdtDescItem;
            _edtUn = pEdtUn;
            _edtTipoConEst = pEdtTipoConEst;
            _actVerify = pActVerify;

            cvItem.BindingContext = this;

        }

        async public void PerformSearch()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SearchBarItCodigo.Text))
                    Items = _ItemsUnfiltered;
                else
                {
                     _ItemsFiltered = new ObservableCollection<ModelItem>(_ItemsUnfiltered.Where(i =>
                     (i is ModelItem && (((ModelItem)i).itCodigo.ToLower().Contains(SearchBarItCodigo.Text.ToLower()))) ||
                     (i is ModelItem && (((ModelItem)i).descItem.ToLower().Contains(SearchBarItCodigo.Text.ToLower())))
                     ));

                    Items = _ItemsFiltered;

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancel");
            }
        }

        async void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            await Task.Run(() => PerformSearch());
        }

        protected async override void OnAppearing()
        {
            //System.Diagnostics.Debug.Write(cvItem.EmptyView.);

            base.OnAppearing();

            var pageProgress = new ProgressBarPopUp("Carregando, aguarde...");

            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);

            var items = SecurityAuxiliar.ItemAll;

            Items = new ObservableCollection<ModelItem>();

            for (int i = 0; i < items.Count; i++)
            {
                Items.Add(new ModelItem
                {
                    itCodigo = items[i].ItCodigo.Trim(),
                    descItem = items[i].DescItem.Trim(),
                    un = items[i].Un,
                    tipoConEst = items[i].TipoConEst
                });
            }

            _ItemsUnfiltered = Items;

            await pageProgress.OnClose();

        }

        void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var current = (cvItem.SelectedItem as ModelItem);

            string strTipoConEst = String.Empty;

            if (_edtItCodigo != null &&
                _edtDescItem != null &&
                _edtUn != null &&
                _edtTipoConEst != null)
            {
                _edtItCodigo.Text = current.itCodigo;
                _edtDescItem.Text = current.descItem;
                _edtUn.Text = current.un;

                if (current.tipoConEst == 1)
                {
                    strTipoConEst = "Serial";
                }
                else if (current.tipoConEst == 2)
                {
                    strTipoConEst = "Número Série";
                }
                else if (current.tipoConEst == 3)
                {
                    strTipoConEst = "Lote";
                }
                else if (current.tipoConEst == 4)
                {
                    strTipoConEst = "Referência";
                }

                _edtTipoConEst.Text = strTipoConEst;
            }
            else
            {
                _actVerify?.Invoke(current.itCodigo);
            }
            PopupNavigation.Instance.PopAsync();
        }
    }
}
