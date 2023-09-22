using Android.Database;
using AutoMapper;
using CollectorQi.Models.ESCL021;
using CollectorQi.Resources;
using CollectorQi.Services.ESCL021;
using CollectorQi.Services.ESCL034;
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
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GerarPedidoListaPage : ContentPage, INotifyPropertyChanged
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

        public ObservableCollection<GerarPedidoViewModel> Items
        {
            get { return _Items; }
            set
            {
                _Items = value;
                lblReparosLidos.Text = $"Reparos lidos: {_Items.Count}";
                OnPropertyChanged("Items");
            }
        }
        
        private ObservableCollection<GerarPedidoViewModel> _Items;
        private ObservableCollection<GerarPedidoViewModel> _ItemsFiltered;
        private ObservableCollection<GerarPedidoViewModel> _ItemsUnfiltered;

        private GerarPedidoEmitente _emitente;
        private GerarPedidoTecnico _tecnico;
        private string _agrupamento = String.Empty;
        private string _RowIdReparoCorrente = string.Empty;
        public int _ContadorReparos = 0;

        public GerarPedidoListaPage(GerarPedidoTecnico tecnico)
        {
            InitializeComponent();

            // lblCodEstabel.Text = "Estabelecimento: " + SecurityAuxiliar.Estabelecimento;

            cvReparo.BindingContext = this;

            Items = new ObservableCollection<GerarPedidoViewModel>();
            
            _tecnico = tecnico;
            stackLayoutSerie.IsVisible = false;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            if (_emitente == null)
            {
                var page = new GerarPedidoEmitentePopUp(null);
                page._confirmaEmitente = ConfirmaEmitente;
                await PopupNavigation.Instance.PushAsync(page);
            }
            
        }

        public void ConfirmaEmitente(GerarPedidoEmitente emitente)
        {
            _emitente = emitente;

            SearchReparo.Focus();
        }

        private async void CarregaListView()
        {
        }

        async void cvReparo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                cvReparo.IsEnabled = false;

                if (cvReparo.SelectedItem == null)
                    return;

                var current = (cvReparo.SelectedItem as GerarPedidoViewModel);

                var result = await DisplayAlert("Confirmação!", $"Deseja retirar o item (Reparo) do pedido?", "Sim", "Não");

                if (result)
                {
                    Items.Remove(current);
                }

                cvReparo.SelectedItem = null;
            }
            catch (Exception ex)
            {
                await DisplayAlert("ERRO", ex.Message, "OK");
            }
            finally
            {
                cvReparo.IsEnabled = true;
                prepararTelaInicial();
            }
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            Application.Current.MainPage = new NavigationPage(new GerarPedidoEnviarParametroPage(_tecnico));
            return true;
        }

        async public void PerformSearch()
        {
            try
            {
                /*
                if (string.IsNullOrWhiteSpace(SearchReparo.Text))
                    Items = _ItemsUnfiltered;
                else
                {
                    /* Victor Alves - 31/10/2019 - Melhoria de performance Item 
                    _ItemsFiltered = new ObservableCollection<GuardaMateriaisDepositoViewModel>(_ItemsUnfiltered.Where(i =>
                   (i is GuardaMateriaisDepositoViewModel && (((GuardaMateriaisDepositoViewModel)i).CodDeposNome.ToLower().Contains(SearchReparo.Text.ToLower())))
                   ));

                    Items = _ItemsFiltered;
                }*/
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancel");
            }
            finally
            {
                //cvInventarioItem.IsEnabled = true;
            }
        }

        /*
        private async void SearchBarCdDepos_Unfocused(object sender, FocusEventArgs e)
        {
            if (!String.IsNullOrEmpty(SearchBarCodDepos.Text))
            {
                var current = _ItemsUnfiltered.FirstOrDefault(p => p.CodDeposNome == SearchBarItCodigo.Text);

                if (current == null)
                {
                    if (SearchBarItCodigo.Text.Contains(';'))
                    {

                        var textItCodigo = SearchBarItCodigo.Text.Split(';');
                        if (textItCodigo.Length > 1)
                        {
                            current = _ItemsUnfiltered.FirstOrDefault(p => p.CodItem == textItCodigo[1]);
                        }
                    }
                }

                if (current != null)
                {
                    OpenPagePopUp(current);

                }
            }
        }
        */

        private CancellationTokenSource throttleCts = new CancellationTokenSource();

        //async void SearchReparo_Unfocused(object sender, FocusEventArgs e)
        async void SearchReparo_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(SearchReparo.Text)) { return; }

            var pageProgress = new ProgressBarPopUp("Lendo Etiqueta, aguarde...");
            stackLayoutSerie.IsVisible = false;

            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);
                if (String.IsNullOrEmpty(e.OldTextValue) && e.NewTextValue.Length >= 5)
                {
                    var param = new GerarPedidoEtiqueta.GerarPedidoParametros
                    {
                        CodEstabel = SecurityAuxiliar.GetCodEstabel(),
                        CodTecnico = _tecnico.CodTecnico,
                        CodDepos = _tecnico.CodDepos,
                        CodEmitente = _emitente.CodEmitente,
                        CodTransporte = "1",
                        Agrupamento = ""
                    };

                    var reparos = new List<GerarPedidoEtiqueta.GerarPedidoReparo>();

                    reparos.Add(new GerarPedidoEtiqueta.GerarPedidoReparo
                    {
                        CodBarras = SearchReparo.Text,
                        CodEstabel = SecurityAuxiliar.GetCodEstabel()

                    });

                    var resultService = await GerarPedidoEtiqueta.SendLeituraEtiquetaAsync(param, reparos);

                    // Leitura Etiqueta;
                    if (resultService != null && resultService.Retorno != null)
                    {
                        if (resultService.Retorno == "OK")
                        {
                            if (String.IsNullOrEmpty(resultService.ParamReparo.ParamLeitura[0].Mensagem))
                            {
                                if (Items.FirstOrDefault(x => x.NumRR == resultService.ParamReparo.ParamLeitura[0].NumRR) != null)
                                {
                                    await DisplayAlert("Erro!", "Reparo já se encontra na lista", "Cancelar");
                                    SearchReparo.Focus();
                                    return;
                                }


                                var serie = resultService.ParamReparo.ParamLeitura[0].Serie;

                                //09-05-2023: Valter
                                //TODO: Caso o reparo nao possua numero de serie solicitar em tela
                                if (String.IsNullOrEmpty(serie))
                                {

                                    SearchSerie.Text = string.Empty;
                                    stackLayoutSerie.IsVisible = true;
                                    stackLayoutReparo.IsVisible = false;
                                    lblReparo.Text = resultService.ParamReparo.ParamLeitura[0].NumRR;
                                    SearchSerie.Focus();

                                }

                                Items.Add(new GerarPedidoViewModel
                                {
                                    CodProduto = resultService.ParamReparo.ParamLeitura[0].CodProduto,
                                    RowId = resultService.ParamReparo.ParamLeitura[0].RowId,
                                    CodEstabel = resultService.ParamReparo.ParamLeitura[0].CodEstabel,
                                    Digito = resultService.ParamReparo.ParamLeitura[0].Digito,
                                    NumRR = resultService.ParamReparo.ParamLeitura[0].NumRR,
                                    DescProduto = resultService.ParamReparo.ParamLeitura[0].DescProduto,
                                    CodFilial = resultService.ParamReparo.ParamLeitura[0].CodFilial,
                                    VlOrcado = resultService.ParamReparo.ParamLeitura[0].VlOrcado,
                                    Mensagem = resultService.ParamReparo.ParamLeitura[0].Mensagem,
                                    CodBarras = resultService.ParamReparo.ParamLeitura[0].CodBarras,
                                    Serie = resultService.ParamReparo.ParamLeitura[0].Serie
                                });

                                _RowIdReparoCorrente = resultService.ParamReparo.ParamLeitura[0].RowId;
                                lblReparosLidos.Text = $"Reparos lidos: {Items.Count}";

                                // lblCodEstabel.Text = "Agrupamento: " + resultService.ParamReparo.Agrupamento;
                            }
                            else
                            {
                                await DisplayAlert("Erro!", resultService.ParamReparo.ParamLeitura[0].Mensagem, "Cancelar");
                            }
                        }
                        else if (resultService.Resultparam != null && resultService.Resultparam.Count > 0)
                        {
                            await DisplayAlert("Erro!", resultService.Resultparam[0].ErrorDescription + " - " + resultService.Resultparam[0].ErrorHelp, "Cancelar");
                        }
                        else
                        {
                            await DisplayAlert("Erro!", "Erro na confirmação do reparo", "Cancelar");
                        }

                        SearchReparo.Text = String.Empty;
                    }
                    else
                    {
                        await DisplayAlert("Erro!", "Erro na confirmação do reparo", "Cancelar");
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancel");

            }
            finally
            {
                await pageProgress.OnClose();
                SearchReparo.Text = String.Empty;
                OnPropertyChanged("Items");
                
            }
        }

       // async void SearchSerie_Unfocused(object sender, FocusEventArgs e)
        async void SearchSerie_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(SearchSerie.Text)) { return; }

            var pageProgress = new ProgressBarPopUp("Validando Número de Série, aguarde...");
 
            try
            {
                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(pageProgress);
                if (String.IsNullOrEmpty(e.OldTextValue) && e.NewTextValue.Length >= 5)
                {

                    //Formatar serie
                    if (SearchSerie.Text.Length < 12)
                        SearchSerie.Text = SearchSerie.Text.PadLeft(12, '0');

                    if (Items.FirstOrDefault(x => x.Serie == SearchSerie.Text) != null)
                    {
                        await DisplayAlert("Erro!", "Série já utilizada na lista", "Cancelar");
                        SearchReparo.Text = string.Empty;
                        SearchReparo.Focus();
                        return;
                    }

                    //Chamar o Servico Validar Reparo
                    var resultService = await ValidarSerieReparoService.ValidarReparo(_RowIdReparoCorrente, SearchSerie.Text);
                    if (resultService.Retorno == "OK")
                    {

                        if (string.IsNullOrEmpty(resultService.Conteudo.Mensagem))
                        {

                            var registroCorrente = Items.Last(x => x.RowId == _RowIdReparoCorrente);
                            Items.Remove(registroCorrente);

                            registroCorrente.Serie = SearchSerie.Text;
                            Items.Add(registroCorrente);

                            prepararTelaInicial();

                        }
                        else
                        {
                            await DisplayAlert("Erro!", resultService.Conteudo.Mensagem, "OK");
                            SearchSerie.Text = string.Empty;
                            SearchSerie.Focus();

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancel");

            }
            finally
            {
                OnPropertyChanged("Items");
                
                await pageProgress.OnClose();
            }
        }

        private void prepararTelaInicial()
        {
            //Setar Tela
            stackLayoutSerie.IsVisible = false;
            stackLayoutReparo.IsVisible = true;
            lblReparo.Text = string.Empty;
            SearchSerie.Text = string.Empty;
            lblReparosLidos.Text = $"Reparos lidos: {Items.Count}";
            OnPropertyChanged("Items");
            SearchReparo.Focus();
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            try
            {
                ToolBarPrint.IsEnabled = false;
                var pageProgress = new ProgressBarPopUp("Carregando...");
                var page = new ArmazenagemPrintPopUp(null, null);
                await PopupNavigation.Instance.PushAsync(page);
                await pageProgress.OnClose();
            }
            finally
            {
                ToolBarPrint.IsEnabled = true;
                OnPropertyChanged("Items");
            }
        }

        private async void ToolbarEmitente_Clicked(object sender, EventArgs e)
        {
            try
            {

                ToolBarEmitente.IsEnabled = false;

                Boolean result = true;

                if (Items.Count > 0)
                {
                    result = await DisplayAlert("Confirmação!", $"Deseja altera o Cliente? Será eliminado todos os reparos da lista.", "Sim", "Não");
                }

                if (result.ToString() == "True")
                {
                    Items.Clear();
                    prepararTelaInicial();

                    var page = new GerarPedidoEmitentePopUp(_emitente);
                    page._confirmaEmitente = ConfirmaEmitente;
                    await PopupNavigation.Instance.PushAsync(page);
                }
            }
            finally
            {
                ToolBarEmitente.IsEnabled = true;
            }
        }

        private async void RefreshGrid()
        {
            Items.Clear();
            OnPropertyChanged("Items");
        }

        private async void BtnEfetivarPedido_Clicked(object sender, EventArgs e)
        {
            try
            {
                /*10-05-2023: Valter: Nao permitir efetivar sem numero de serie
                
                //Obter a lista de reparos sem serie
                var listaReparosSemSerie = Items.Where(x => x.Serie == "" || x.Serie == null);
                var reparosSemLeitura = string.Empty;

                //Montar a lista de reparos sem serie para visualizacao
                listaReparosSemSerie.ForEach(x => reparosSemLeitura += $"{x.NumRR}, ");

                Mensagem na Tela - Retirada para possibilitar a leitura de series em branco
                if (!String.IsNullOrEmpty(reparosSemLeitura))
                {
                    await DisplayAlert("Erro", $"Reparo(s): {reparosSemLeitura} sem número de série, refaça a leitura.", "OK");
                    if (SearchReparo.IsVisible)
                        SearchReparo.Focus();
                    else
                        SearchSerie.Focus();

                    return;
                }
                */


                //Items.Clear();

                if (Items != null && Items.Count <= 0)
                {
                    await DisplayAlert("Erro", "Nenhum reparo registrado para geração do pedido.", "OK");
                    return;
                }
                
                var pagePesoVolume = new GerarPedidoPesoVolumePopUp(Items, _emitente, _tecnico);
                pagePesoVolume._refreshRow = RefreshGrid;
                await PopupNavigation.Instance.PushAsync(pagePesoVolume);
                OnPropertyChanged("Items");

                lblReparosLidos.Text = $"Reparos lidos: {0}";

            }
            catch (Exception ex)
            {
                await DisplayAlert("ERRO", ex.Message, "OK");
            }
        }

        private void BtnLimparPedido_Clicked(object sender, EventArgs e)
        {
            Items.Clear();
            prepararTelaInicial();
            
        }

        private void ToolBarVoltar_Clicked(object sender, EventArgs e)
        {
            base.OnBackButtonPressed();
            Xamarin.Forms.Application.Current.MainPage = new NavigationPage(new ArmazenagemPage());
        }

        private void BtnSairSerie_Clicked(object sender, EventArgs e)
        {
            var registroCorrente = Items.Last(x => x.RowId == _RowIdReparoCorrente);
            if (registroCorrente != null)
            {
                Items.Remove(registroCorrente);
            }
            
            prepararTelaInicial();
        }

        private void BtnSerieEmBranco_Clicked(object sender, EventArgs e)
        {
            prepararTelaInicial();
        }

    }

    public class GerarPedidoTecnico
    {
        public int CodTecnico { get; set; }
        public string Senha { get; set; }
        public string CodDepos { get; set; }
    }

    public class GerarPedidoEmitente
    {
        public int CodEmitente { get; set; }
        public string NomeAbrev { get; set; }
        public string Nome { get; set; }
        public string Endereco { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
    }

    public class GerarPedidoViewModel : INotifyPropertyChanged
    {
        public string CodProduto { get; set; }
        public string RowId { get; set; }
        public string CodEstabel { get; set; }
        public string Digito { get; set; }
        public string NumRR { get; set; }
        public string DescProduto { get; set; }
        public string CodFilial { get; set; }
        public string VlOrcado { get; set; }
        public string Mensagem { get; set; }
        public string CodBarras { get; set; }
        public string Serie { get; set; }

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