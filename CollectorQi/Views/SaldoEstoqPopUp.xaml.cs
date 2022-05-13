using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Rg.Plugins.Popup.Pages;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CollectorQi.Resources.DataBaseHelper;
using Rg.Plugins.Popup.Services;
using CollectorQi;
using CollectorQi.Resources;

namespace CollectorQi.Views
{


    public partial class SaldoEstoqPopUp : PopupPage, INotifyPropertyChanged 
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

        private int _rating;
        public int Rating
        {
            get { return _rating; }
            set => SetProperty(ref _rating, value);
        }

        #endregion

        public ObservableCollection<VO.SaldoEstoqVO> ObsSaldoEstoq { get; }

        CustomEntryText _edtDepSaida;
        CustomEntryText _edtCodLocaliz;
        CustomEntryText _edtLote;
        //DatePicker _selDtValiLote;
        CustomEntryText _edtDtValiLote;
        CustomEntryText _edtSaldo;
        CustomEntryText _edtSaldoMobile;

        public SaldoEstoqPopUp(string pCodEstabel, string pItCodigo, string pDescItem, CustomEntryText pEdtDepSaida, CustomEntryText pEdtCodLocaliz, CustomEntryText pEdtLote, /*DatePicker pSelDtValiLote*/ CustomEntryText pEdtDtValilote, CustomEntryText pEdtSaldo, CustomEntryText pEdtSaldoMobile, bool pBlnClickQr = false)
        {
            InitializeComponent();

            lblItCodigo.Text = pItCodigo;
            lblDescItem.Text = pDescItem;
            lblCodEstabel.Text = SecurityAuxiliar.Estabelecimento;

            _edtDepSaida = pEdtDepSaida;
            _edtCodLocaliz = pEdtCodLocaliz;
            _edtLote = pEdtLote;
            _edtDtValiLote = pEdtDtValilote;
            _edtSaldo = pEdtSaldo;
            _edtSaldoMobile = pEdtSaldoMobile;

            ObsSaldoEstoq = new ObservableCollection<VO.SaldoEstoqVO>();

            ObsSaldoEstoq = new ObservableCollection<VO.SaldoEstoqVO>(SaldoEstoqDB.GetSaldoEstoqByItemAndEstab(pItCodigo, pCodEstabel));

            // Quando trazer pelo QRCODE busca só o lote que passou no scanner
            if (pBlnClickQr)
            {
                List<VO.SaldoEstoqVO> lstRemove = new List<VO.SaldoEstoqVO>();

                for (int i = 0; i < ObsSaldoEstoq.Count;  i++)
                {
                    if (ObsSaldoEstoq[i].CodLote.Trim() != _edtLote.Text.Trim())
                        lstRemove.Add(ObsSaldoEstoq[i]);
                }

                for (int i = 0; i < lstRemove.Count; i++)
                {
                    ObsSaldoEstoq.Remove(lstRemove[i]);
                }
            }

            cvSaldoEstoq.BindingContext = this;

        }

        void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var current = (cvSaldoEstoq.SelectedItem as VO.SaldoEstoqVO);

            if (current != null)
            {

                var deposito = DepositoDB.GetDeposito(current.CodDepos);

                if (deposito != null && !String.IsNullOrEmpty(deposito.CodDepos))
                {
                    _edtDepSaida.Text = current.CodDepos + " (" + deposito.Nome + ")";
                }
                else
                    _edtDepSaida.Text = current.CodDepos;

                _edtLote.Text        = current.CodLote;
                _edtDtValiLote.Text  = current.DtValiLote.HasValue ? current.DtValiLote.Value.ToString("dd/MM/yyyy") : string.Empty;
                _edtCodLocaliz.Text  = current.CodLocaliz;
                _edtSaldo.Text       = current.QtidadeAtu.ToString();
                _edtSaldoMobile.Text = current.QtidadeMobile.ToString();
            }

            PopupNavigation.Instance.PopAsync();
        }
    }
}
