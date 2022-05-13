using CollectorQi.Models;
using CollectorQi.ViewModels;
using CollectorQi.Resources.DataBaseHelper;
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

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RequisicaoListaItemSaldoEstoqPage : ContentPage, INotifyPropertyChanged
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

        private RequisicaoItemVO _requisicaoItem;
        private bool _isDevolucao;

        public RequisicaoListaItemSaldoEstoqPage(RequisicaoItemVO pRequisicaoItemVO, bool pIsDevolucao)
        {
            InitializeComponent();

            _requisicaoItem = pRequisicaoItemVO;
            _isDevolucao = pIsDevolucao;

            lblCodEstabel.Text = SecurityAuxiliar.Estabelecimento.Trim();
            lblItCodigo.Text = pRequisicaoItemVO.ItCodigo.Trim();
            lblDescItem.Text = pRequisicaoItemVO.__item__.DescItem.Trim();
            lblQtdeAtender.Text = pRequisicaoItemVO.QtaAtender.ToString();
            lblQtdeDevolver.Text = pRequisicaoItemVO.QtaDevolver.ToString();

            if (_isDevolucao)
            {
                lblQtdeAtender.IsVisible = false;
                lblQtdeAtenderLabel.IsVisible = false;
            }
            else
            {
                lblQtdeDevolver.IsVisible = false;
                lblQtdeDevolverLabel.IsVisible = false;
            }


            frameRefer.IsVisible = false;
            frameLote.IsVisible = false;

            switch (pRequisicaoItemVO.__item__.TipoConEst)
            {
                case 3: // Lote
                case 2: // Numero Serie
                    frameLote.IsVisible = true;
                    break;
                case 4: // Referencia
                    frameRefer.IsVisible = true;
                    break;

            }

            AtualizaCv();
        }

        async void AtualizaCv()
        {
            // Elimina todas as requisicoes para atender no depósito informado
            var lstReqAtual = await RequisicaoItemSaldoEstoqDB.GetRequisicaoItemSaldoEstoq(_requisicaoItem.NrRequisicao, _requisicaoItem.Sequencia, _requisicaoItem.ItCodigo, false);

            if (lstReqAtual != null && lstReqAtual.Count > 0)
            {
                edtDepEntrada.Text = lstReqAtual[0].CodDepos;
                edtLote.Text = lstReqAtual[0].CodLote;
                edtCodLocaliz.Text = lstReqAtual[0].CodLocaliz;
                edtCodRefer.Text = lstReqAtual[0].CodRefer;
                edtQuantidade.Text = _isDevolucao ? lstReqAtual[0].QtdDevolver.ToString() : lstReqAtual[0].QtdAtender.ToString();
            }

            var saldoEstoq = await SaldoEstoqDB.GetSaldoEstoqByItemAndEstabAsync(_requisicaoItem.ItCodigo, SecurityAuxiliar.GetCodEstabel());

            List<String> lstLocaliz = new List<String>();

            if (lstLocaliz != null)
            {
                foreach (var row in saldoEstoq)
                {
                    if (lstLocaliz.Find(p => p.Equals(row.CodLocaliz)) == null)
                        lstLocaliz.Add(row.CodLocaliz);
                }
            }

            if (lstLocaliz != null &&
                lstLocaliz.Count > 0)
            {
                // Se a unica localização cadastrada for vazio, então nem precisa aparecer
                if (lstLocaliz.Count == 1 && String.IsNullOrEmpty(lstLocaliz[0]))
                {
                    frameLocaliz.IsVisible = false;
                }
                else
                {
                    frameLocaliz.IsVisible = true;
                }
            }
            else
            {
                frameLocaliz.IsVisible = false;
            }
        }

        async void OnClick_EfetivaRequisicao(object sender, System.EventArgs e)
        {

            try
            { // Elimina todas as requisicoes para atender no depósito informado
                var lstReqAtual = await RequisicaoItemSaldoEstoqDB.GetRequisicaoItemSaldoEstoq(_requisicaoItem.NrRequisicao, _requisicaoItem.Sequencia, _requisicaoItem.ItCodigo, _isDevolucao);

                // Anteriormente as requisições era atendida por mais de um registro
                // Foi alterado o processo para atender apenas uma requisição por vez
                if (lstReqAtual != null)
                {
                    for (int i = 0; i < lstReqAtual.Count; i++)
                        await RequisicaoItemSaldoEstoqDB.DeletarRequisicaoItemSaldoEstoq(lstReqAtual[i]);
                }

                // Atualiza requisicao para saber se alterou STATUS
                _requisicaoItem = await RequisicaoItemDB.GetRequisicaoItemAsync(_requisicaoItem);
                List<RequisicaoItemSaldoEstoqVO> lstReqItemSaldo = new List<RequisicaoItemSaldoEstoqVO>();

                string strCodDep = edtDepEntrada.Text;

                if (strCodDep.IndexOf('(') > 0)
                    strCodDep = strCodDep.Remove(strCodDep.IndexOf('(')).Trim();

                if (String.IsNullOrEmpty(strCodDep))
                {
                    await DisplayAlert("Erro!", "Informe o depósito para o atendimento da requisição", "CANCELAR");

                    return;
                }

                // Apenas atendimento padrao passa pela validacao de saldo de estoque
                if (!_isDevolucao)
                {
                    var lstSaldoEstoq = await SaldoEstoqDB.GetSaldoEstoqByItemAndEstabAsync(_requisicaoItem.ItCodigo, SecurityAuxiliar.GetCodEstabel());

                    bool lSaldoOk = false;

                    foreach (var row in lstSaldoEstoq)
                    {
                        string strCodLocaliz = String.IsNullOrEmpty(edtCodLocaliz.Text) ? String.Empty : edtCodLocaliz.Text;
                        string strCodLote = String.IsNullOrEmpty(edtLote.Text) ? string.Empty : edtLote.Text;
                        string strCodRefer = String.IsNullOrEmpty(edtCodRefer.Text) ? string.Empty : edtCodRefer.Text;

                        if (row.CodDepos == strCodDep &&
                            row.CodLocaliz == strCodLocaliz &&
                            row.CodLote == strCodLote &&
                            row.CodRefer == strCodRefer)
                        {
                            if (row.QtidadeMobile >= Decimal.Parse(edtQuantidade.Text.Replace(".",",")))
                            {
                                lSaldoOk = true;
                            }
                        }
                    }

                    if (!lSaldoOk)
                    {
                        await DisplayAlert("Erro!", "Quantidade informada (" + edtQuantidade.Text + ") no depósito (" + strCodDep.Trim() + ") não pode ser maior que a (Quantidade Estoque Mobile)", "CANCELAR");

                        return;
                    }
                }

                decimal deQtdAtender = _isDevolucao ? 0 : Decimal.Parse(edtQuantidade.Text.Replace(".", ","));
                decimal deQtdDevolver = _isDevolucao ? Decimal.Parse(edtQuantidade.Text.Replace(".", ",")) : 0;

                lstReqItemSaldo.Add(new RequisicaoItemSaldoEstoqVO
                {
                    //RequisicaoItemSaldoEstoqId = 0,
                    NrRequisicao = _requisicaoItem.NrRequisicao,
                    Sequencia = _requisicaoItem.Sequencia,
                    ItCodigo = _requisicaoItem.ItCodigo,
                    CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                    CodDepos = strCodDep.Trim(),
                    CodRefer = String.IsNullOrEmpty(edtCodRefer.Text) ? string.Empty : edtCodRefer.Text,
                    CodLocaliz = String.IsNullOrEmpty(edtCodLocaliz.Text) ? String.Empty : edtCodLocaliz.Text,
                    CodLote = String.IsNullOrEmpty(edtLote.Text) ? string.Empty : edtLote.Text,
                    QtdAtender = deQtdAtender,
                    QtdDevolver = deQtdDevolver,
                    IsDevolucao = _isDevolucao
                });

                if (_isDevolucao)
                {
                    if (Decimal.Parse(edtQuantidade.Text.Replace(".", ",")) > _requisicaoItem.QtaDevolver)
                    {
                        await DisplayAlert("Erro!", "Quantidade informada (" + edtQuantidade.Text + ") não pode ser maior que a (Quantidade a Devolver) (" + _requisicaoItem.QtaAtender.ToString() + ")", "CANCELAR");

                        return;
                    }
                }
                else
                {
                    if (Decimal.Parse(edtQuantidade.Text.Replace(".", ",")) > _requisicaoItem.QtaAtender)
                    {
                        await DisplayAlert("Erro!", "Quantidade informada (" + edtQuantidade.Text + ") não pode ser maior que a (Quantidade a Atender) (" + _requisicaoItem.QtaAtender.ToString() + ")", "CANCELAR");

                        return;
                    }
                }

                await RequisicaoItemSaldoEstoqDB.AtualizaRequisicaoItemSaldoEstoq(lstReqItemSaldo);

                _requisicaoItem.QtDevolvidaMobile = deQtdDevolver;
                _requisicaoItem.QtAtendidaMobile = deQtdAtender;

                await RequisicaoItemDB.AtualizaRequisicaoItem(_requisicaoItem);

                await Navigation.PopModalAsync(true);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "CANCELAR");
            }

            /*
       List<RequisicaoItemSaldoEstoqVO> lstReqItemSaldo = new List<RequisicaoItemSaldoEstoqVO>();

       // Atualiza requisicao para saber se alterou STATUS
       _requisicaoItem = await RequisicaoItemDB.GetRequisicaoItemAsync(_requisicaoItem);

       decimal deQtDigitada = 0;
       for (int i = 0; i < lstSaldoEstoq.Count; i++)
       {
           deQtDigitada = deQtDigitada + ObsReqSaldoEstoq[i].QtdAtender;
       }

    /

       /*
       // Cria registros de requisicao para adicionar na tabela
       for (int i = 0; i < ObsReqSaldoEstoq.Count; i++)
       {
           if (ObsReqSaldoEstoq[i].QtdAtender <= 0)
               continue;

           if (ObsReqSaldoEstoq[i].QtdAtender > ObsReqSaldoEstoq[i].QtidadeMobile)
           {
               await DisplayAlert("Erro!", "Quantidade informada (" + ObsReqSaldoEstoq[i].QtdAtender + ") no depósito (" + ObsReqSaldoEstoq[i].__deposito__.Nome + ") não pode ser maior que a (Quantidade Estoque Mobile)", "CANCELAR");
               return;
           }

           lstReqItemSaldo.Add(new RequisicaoItemSaldoEstoqVO
           {
               RequisicaoItemSaldoEstoqId = ObsReqSaldoEstoq[i].requisicaoItemSaldoEstoqId,
               NrRequisicao = _requisicaoItem.NrRequisicao,
               Sequencia = _requisicaoItem.Sequencia,
               ItCodigo = _requisicaoItem.ItCodigo,
               CodEstabel = ObsReqSaldoEstoq[i].CodEstabel,
               CodDepos = ObsReqSaldoEstoq[i].CodDepos,
               CodRefer = ObsReqSaldoEstoq[i].CodRefer,
               CodLocaliz = ObsReqSaldoEstoq[i].CodLocaliz,
               CodLote = ObsReqSaldoEstoq[i].CodLote,
               QtdAtender = ObsReqSaldoEstoq[i].QtdAtender
           });
       }

       // Eliminar se nao existe os registros de requisicao
       var lstReqAtual = await RequisicaoItemSaldoEstoqDB.GetRequisicaoItemSaldoEstoqByItemAndEstab(_requisicaoItem.ItCodigo, SecurityAuxiliar.GetCodEstabel());

       if (lstReqAtual != null)
       {
           for (int i = 0; i < lstReqAtual.Count; i++)
           {
               var buscaSaldo = ObsReqSaldoEstoq.Where(p => p.requisicaoItemSaldoEstoqId == lstReqAtual[i].RequisicaoItemSaldoEstoqId).First();

               if (buscaSaldo == null)
               {
                   await RequisicaoItemSaldoEstoqDB.DeletarRequisicaoItemSaldoEstoq(lstReqAtual[i]);
               }
               else
               {
                   if (buscaSaldo.QtdAtender <= 0)
                   {
                       await RequisicaoItemSaldoEstoqDB.DeletarRequisicaoItemSaldoEstoq(lstReqAtual[i]);
                   }
               }
           }
       }
       */

        }

        protected override bool OnBackButtonPressed()
        {
            Navigation.PopModalAsync(true);
            return true;
        }

        void Focused_QtAtender(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            var entry = sender as CustomEntry;

            if (entry != null)
            {
                if (entry.Text == "0")
                {
                    entry.Text = String.Empty;
                }
            }
        }

        async void OnClick_Deposito(object sender, EventArgs e)
        {
            try
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

                    if (action != "Cancelar" && action != null)
                    {
                        edtDepEntrada.Text = action.ToString();
                    }
                }
                else
                    await DisplayAlert("Erro!", "Nenhum depósito encontrado.", "OK");

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "OK");
            }
            finally
            {
                BtnDeposito.IsEnabled = true;
            }

        }

        async void OnClick_Lote(object sender, EventArgs e)
        {
            try
            {
                BtnLote.IsEnabled = false;


                var saldoEstoq = await SaldoEstoqDB.GetSaldoEstoqByItemAndEstabAsync(_requisicaoItem.ItCodigo, SecurityAuxiliar.GetCodEstabel());

                List<String> lstLote = new List<String>();

                if (saldoEstoq != null)
                {
                    foreach (var row in saldoEstoq)
                    {
                        if (lstLote.Find(p => p.Equals(row.CodLote)) == null)
                            lstLote.Add(row.CodLote);
                    }
                }

                if (lstLote != null &&
                    lstLote.Count > 0)
                {
                    var action = await DisplayActionSheet("Escolha o Lote?", "Cancelar", null, lstLote.ToArray());

                    if (action != "Cancelar" && action != null)
                    {
                        edtLote.Text = action.ToString();
                    }
                }
                else
                {
                    await DisplayAlert("Erro!", "Nenhum lote encontrado.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "OK");
            }
            finally
            {
                BtnLote.IsEnabled = true;
            }
        }


        async void OnClick_CodRefer(object sender, EventArgs e)
        {
            try
            {
                BtnCodRefer.IsEnabled = false;


                var saldoEstoq = await SaldoEstoqDB.GetSaldoEstoqByItemAndEstabAsync(_requisicaoItem.ItCodigo, SecurityAuxiliar.GetCodEstabel());

                List<String> lstReferencia = new List<String>();

                if (saldoEstoq != null)
                {
                    foreach (var row in saldoEstoq)
                    {
                        if (lstReferencia.Find(p => p.Equals(row.CodRefer)) == null)
                            lstReferencia.Add(row.CodRefer);
                    }
                }

                if (lstReferencia != null &&
                    lstReferencia.Count > 0)
                {
                    var action = await DisplayActionSheet("Escolha a Referência?", "Cancelar", null, lstReferencia.ToArray());

                    if (action != "Cancelar" && action != null)
                    {
                        edtCodRefer.Text = action.ToString();
                    }
                }
                else
                {
                    await DisplayAlert("Erro!", "Nenhuma referência encontrada.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "OK");
            }
            finally
            {
                BtnCodRefer.IsEnabled = true;
            }
           
        }

        async void OnClick_CodLocaliz(object sender, EventArgs e)
        {
            try
            {
                BtnCodLocaliz.IsEnabled = false;


                var saldoEstoq = await SaldoEstoqDB.GetSaldoEstoqByItemAndEstabAsync(_requisicaoItem.ItCodigo, SecurityAuxiliar.GetCodEstabel());

                List<String> lstLocaliz = new List<String>();

                if (lstLocaliz != null)
                {
                    foreach (var row in saldoEstoq)
                    {
                        if (lstLocaliz.Find(p => p.Equals(row.CodLocaliz)) == null)
                            lstLocaliz.Add(row.CodLocaliz);
                    }
                }

                if (lstLocaliz != null &&
                    lstLocaliz.Count > 0)
                {
                    var action = await DisplayActionSheet("Escolha a Localização?", "Cancelar", null, lstLocaliz.ToArray());

                    if (action != "Cancelar" && action != null)
                    {
                        edtCodLocaliz.Text = action.ToString();
                    }
                }
                else
                {
                    await DisplayAlert("Erro!", "Nenhuma localização encontrada.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "OK");
            }
            finally
            {
                BtnCodLocaliz.IsEnabled = true;
            }
        }

        /*

        void Handle_Completed(object sender, System.EventArgs e)
        {
            edtItCodigo.Unfocus();
        }

        void edtItCodigo_Unfocused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            string strTipoConEst = String.Empty;

            var item = ItemDB.GetItem(edtItCodigo.Text);

            if (item != null)
            {

                if (item.TipoConEst == 1)
                {
                    strTipoConEst = "Serial";
                }
                else if (item.TipoConEst == 2)
                {
                    strTipoConEst = "Número Série";
                }
                else if (item.TipoConEst == 3)
                {
                    strTipoConEst = "Lote";
                }
                else if (item.TipoConEst == 4)
                {
                    strTipoConEst = "Referência";
                }

                edtDescItem.Text = item.DescItem;
                edtUnidade.Text = item.Un;
                edtTipoConEst.Text = strTipoConEst;
            }
            else
            {
                edtDescItem.Text = "";
                edtUnidade.Text = "";
                edtTipoConEst.Text = "";
            }
        } */

        /*
        async void OnClick_QR(object sender, EventArgs e)
        {
            /*
            var scanner = new ZXing.Mobile.MobileBarcodeScanner();

            ZXing.Mobile.MobileBarcodeScanningOptions scanningOptions = new ZXing.Mobile.MobileBarcodeScanningOptions();

            /*scanningOptions.UseFrontCameraIfAvailable = switchCameraFrontal.IsToggled;

            scanningOptions.PossibleFormats.Add(ZXing.BarcodeFormat.CODE_128);
            scanningOptions.PossibleFormats.Add(ZXing.BarcodeFormat.CODE_39);
            scanningOptions.PossibleFormats.Add(ZXing.BarcodeFormat.CODABAR);
            scanningOptions.PossibleFormats.Add(ZXing.BarcodeFormat.QR_CODE);

            var result = await scanner.Scan(scanningOptions);

            if (result != null)
            {
                VerifyProd(result.Text.ToString().Trim()); 
            }*/

        /*


            try
            {
                //BtnQR.IsEnabled = false;

                var customScanPage = new ZXingScannerPage();

                customScanPage.SetResultAction(VerifyProd);

                await Navigation.PushModalAsync(customScanPage);

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "OK");
            }
            finally
            {
               // BtnQR.IsEnabled = true;
            }
        } */

        /*
        private void VerifyProd(string strQr)
        {
            try
            {
                if (strQr == null)
                    return;

                var mdlEtiqueta = csAuxiliar.GetEtiqueta(strQr);

                if (mdlEtiqueta != null)
                {
                    edtItCodigo.Text = mdlEtiqueta.itCodigo;
                    edtLote.Text = mdlEtiqueta.lote;
                    edtDtValiLote.Text = mdlEtiqueta.dtValiLote;

                    edtItCodigo.Focus();
                    edtItCodigo.Unfocus();

                    /*

                    edtQuantidade.Focus();
                    //_blnClickQr = true;

                    //OnClick_DepositoSaida(new object(), new EventArgs());
                }
            }
            catch (Exception ex)
            {
                DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
        } */

        /*
        async void OnClick_BuscaItem(object sender, EventArgs e)
        {
            try
            {
                //BtnBuscaItem.IsEnabled = false;

                var page = new ItemPopUp(edtItCodigo, edtDescItem, edtUnidade, edtTipoConEst);

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page);

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "OK");
            }
            finally
            {
                //BtnBuscaItem.IsEnabled = true;
            }
        } */


    }
}
