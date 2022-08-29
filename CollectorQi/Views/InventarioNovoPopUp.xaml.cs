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
    public partial class InventarioNovoPopUp : PopupPage, INotifyPropertyChanged
    {
        /*

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
        }*/

        

        /*
        private Editor _edtItCodigo;
        private Editor _edtDescItem;
        private Editor _edtUn;
        private Editor _edtTipoConEst;
        private ObservableCollection<ModelItem> _Items;
        private ObservableCollection<ModelItem> _ItemsFiltered;
        private ObservableCollection<ModelItem> _ItemsUnfiltered;
        */

        public InventarioNovoPopUp()
        {
            InitializeComponent();

            /*
            _edtItCodigo = pEdtItCodigo;
            _edtDescItem = pEdtDescItem;
            _edtUn = pEdtUn;
            _edtTipoConEst = pEdtTipoConEst;

            var itemDB = new ItemDB();

            var items = itemDB.GetItems();

            Items = new ObservableCollection<ModelItem>();

            for (int i = 0; i < items.Count; i++)
            {
                Items.Add(new ModelItem
                {
                    itCodigo = items[i].ItCodigo,
                    descItem = items[i].DescItem,
                    un = items[i].Un,
                    tipoConEst = items[i].TipoConEst
                });
            }

            _ItemsUnfiltered = Items;

            cvItem.BindingContext = this;*/

        }

        async void OnClick_Deposito(object sender, EventArgs e)
        {
            BtnDeposito.IsEnabled = false;

            var deposito = DepositoDB.GetDeposito();
            if (deposito != null)
            {
                string[] arrayDep = new string[deposito.Count];
                for (int i = 0; i < deposito.Count; i++)
                {
                    arrayDep[i] = deposito[i].CodDepos + " (" + deposito[i].Nome.Trim() + ")";
                }

                var action = await DisplayActionSheet("Escolha o Depósito?", "Cancelar", null, arrayDep);

                if (action != "Cancelar")
                {
                    edtDeposito.Text = action.ToString();
                }
            }
            else
                await DisplayAlert("Erro!", "Nenhum depósito encontrado.", "OK");

            BtnDeposito.IsEnabled = true; 
        }


        async void OnClick_Contagem(object sender, EventArgs e)
        {

            BtnContagem.IsEnabled = false;

            string[] arrayContagem = new string[3];

            arrayContagem[0] = "1";
            arrayContagem[1] = "2";
            arrayContagem[2] = "3";

            var action = await DisplayActionSheet("Contagem do inventário?", "Cancelar", null, arrayContagem);

            if (action != "Cancelar")
            {
                edtContagem.Text = action.ToString();
            }

            /*var depDataBase = new DepositoDB();

            var deposito = depDataBase.GetDeposito();
            if (deposito != null)
            {
                string[] arrayDep = new string[deposito.Count];
                for (int i = 0; i < deposito.Count; i++)
                {
                    arrayDep[i] = deposito[i].CodDepos + " (" + deposito[i].Nome.Trim() + ")";
                }

                var action = await DisplayActionSheet("Escolha o Depósito?", "Cancelar", null, arrayDep);

                if (action != "Cancel")
                {
                    edtDeposito.Text = action.ToString();
                }
            }
            else
                await DisplayAlert("Erro!", "Nenhum depósito encontrado.", "OK");
                */

            BtnContagem.IsEnabled = true;
        }

        async void OnClick_Efetivar(object sender, EventArgs e)
        {
            try
            {
               

                var tpl = Models.Datasul.IntegracaoOnlineBatch.InventarioNovo(new VO.ESCL018.InventarioVO
                {
                    CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                    CodDepos = edtDeposito.Text,
                    //DtInventario = selDtInventario.Date,
                    //Contagem = int.Parse(edtContagem.Text)
                });

                await DisplayAlert("Geração inventário", tpl.Item2, "OK");

                Application.Current.MainPage = new NavigationPage(new InventarioFisicoListaPage());

                await PopupNavigation.Instance.PopAsync();

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
        }

        async void OnClick_Limpar(object sender, EventArgs e)
        {

        }

        /*

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

        void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var current = (cvItem.SelectedItem as ModelItem);

            string strTipoConEst = String.Empty;

            _edtItCodigo.Text = current.itCodigo;
            _edtDescItem.Text = current.descItem;
            _edtUn.Text = current.un;

            if (current.tipoConEst == 1 )
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

            PopupNavigation.Instance.PopAsync();
        }*/
    }
}
