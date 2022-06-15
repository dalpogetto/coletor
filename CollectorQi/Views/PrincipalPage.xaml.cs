using CollectorQi.Models;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Resources.DataBaseHelper.Batch;
using CollectorQi.ViewModels;
using CollectorQi.VO;
using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PrincipalPage : ContentPage
    {
        private bool _blnLoginPage;

        public PrincipalPage()
        {
            InitializeComponent();

            listView.ItemSelected += OnSelection;

        }

        private async Task AtualizaListView()
        {
            try
            {
                var security =  await SecurityDB.GetSecurityAsync();

                if (security != null && security.Autenticado)
                {
                    SecurityAuxiliar.Autenticado = true;
                    SecurityAuxiliar.CodUsuario = security.CodUsuario;
                    SecurityAuxiliar.CodSenha = security.CodSenha;
                }

                // Victor - apenas apresentacao (09/04/2020)

                //await SecurityDB.AtualizarSecurityIntegracao();

                if (security != null)
                {

                    if (CrossConnectivity.Current.IsConnected)
                    {
                        Task.Run(() => Controller.ConectColetorAsync(SecurityAuxiliar.CodUsuario, SecurityAuxiliar.CodSenha, null));
                    }

                    lblMensagemErro.Text = String.Empty;

                    footerCodUsuario.Text = SecurityAuxiliar.CodUsuario;
                    footerVersion.Text = "v" + VersionTracking.CurrentVersion;

                    /* Valida integração de cadastros */
                    if (security.DtUltIntegracao.AddDays(30) < DateTime.Today.Date)
                    {
                        lblMensagemErro.Text = "Atenção! Ultima integração efetuada dia (" + security.DtUltIntegracao.Date.ToString("dd/MM/yyyy") + ") acesse a internet pelo dispositivo para efetuar a integração e continuar usando o sistema.";
                    }

                    ErroIntegracaoTransferencia();
                    ErroIntegracaoInventario();

                    //string[] imagem = new string[] { "almoxarifado.png", "inventario.png", "expedicao.png", "logoTotvs.png", "logout.png" };
                    //string[] titulo = new string[] { "Almoxarifado", "Inventário", "Estabelecimento", "Integração TOTVS", "Logoff" };
                    //string[] subTitulo = new string[] { "Opções do Almoxarifado" ,
                    //                                "Inventariar produtos",
                    //                                "Escolher o estabelecimento",
                    //                                "Ultima Integração (" + security.DtUltIntegracao + ")",
                    //                                "Sair da conta: " + SecurityAuxiliar.CodUsuario };

                    string[] imagem = new string[] { "security.png", "fisica.png", "expedicao.png", "logoTotvs.png", "logout.png" };
                    string[] titulo = new string[] { "Armazenagem", "Recebimento", "Estabelecimento", "Integração TOTVS", "Logoff" }; 

                    string[] subTitulo = new string[] { "Armazenagem", "Conferência Física", "Escolher o estabelecimento", "Última Integração",
                                                    "Sair da conta: " + SecurityAuxiliar.CodUsuario };

                    List<MenuItemDetail> menuItemDetails = new List<MenuItemDetail>();
                    MenuItemDetail menuItemDetail;

                    for (int i = 0; i < imagem.Count(); i++)
                    {
                        menuItemDetail = new MenuItemDetail { MenuItemDatailId = i + 1, Name = titulo[i], SubTitle = subTitulo[i], Image = imagem[i] };

                        if (!String.IsNullOrEmpty(lblMensagemErro.Text))
                        {
                            /* Trava o acesso apenas do almoxarifado, inventario e expedição */
                            if (i < 3)
                            {
                                menuItemDetail.Image = "closeMini.png";
                            }
                        }

                        menuItemDetails.Add(menuItemDetail);
                    }

                    listView.ItemsSource = menuItemDetails;

                    if (!String.IsNullOrEmpty(lblMensagemErro.Text))
                    {
                        frameCadastros.IsVisible = true;
                        frameEstab.IsVisible = false;
                    }
                    else
                    {
                        frameCadastros.IsVisible = false;

                        if (!String.IsNullOrEmpty(SecurityAuxiliar.Estabelecimento))
                        {
                            lblCodEstabel.Text = SecurityAuxiliar.Estabelecimento;
                            frameEstab.IsVisible = true;
                        }
                        else
                        {
                            lblCodEstabel.Text = String.Empty;
                            frameEstab.IsVisible = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", "Erro para atualizar as ultimas integrações com o sistema, se o erro persistir, reinicie a aplicação. " + Environment.NewLine + ex.Message, "CANCELAR");
            }
        }

        private bool ErroIntegracaoInventario()
        {
            /* Valida integração inventario */
            DateTime dtLastIntegracaoInventario = DateTime.MaxValue;

            var lstBatchInventarioErro = BatchInventarioDB.GetBatchInventarioByStatus(eStatusIntegracao.ErroIntegracao).
                OrderByDescending(p => p.DtIntegracao).ToList();

            var lstBatchInventarioPend = BatchInventarioDB.GetBatchInventarioByStatus(eStatusIntegracao.PendenteIntegracao).
                OrderByDescending(p => p.DtIntegracao).ToList();

            if (lstBatchInventarioErro != null && lstBatchInventarioErro.Count > 0)
            {
                dtLastIntegracaoInventario = lstBatchInventarioErro[0].DtIntegracao;
            }

            if (lstBatchInventarioPend != null && lstBatchInventarioPend.Count > 0)
            {
                if (lstBatchInventarioPend[0].DtIntegracao < dtLastIntegracaoInventario)
                    dtLastIntegracaoInventario = lstBatchInventarioPend[0].DtIntegracao;
            }

            if (dtLastIntegracaoInventario < DateTime.Today.Date)
            {
                if (!String.IsNullOrEmpty(lblMensagemErro.Text))
                    lblMensagemErro.Text = lblMensagemErro.Text + Environment.NewLine;

                lblMensagemErro.Text = lblMensagemErro.Text + "Atenção! Existe integração de inventário pendente desde o dia  (" + dtLastIntegracaoInventario.ToString("dd/MM/yyyy") + ") efetue a correção da integração para continuar usando o sistema.";

                return true;
            }
            return false;
        }


        private bool ErroIntegracaoTransferencia()
        {
            /* Valida integração Transferencia */


            DateTime dtLastIntegracaoTransf = DateTime.MaxValue;

            var lstBatchTransferenciaErro = BatchDepositoTransfereDB.GetBatchDepositoTransfereByStatus(eStatusIntegracao.ErroIntegracao).OrderByDescending
                (p => p.DtTransferencia).ToList();
            var lstBatchTransferenciaPend = BatchDepositoTransfereDB.GetBatchDepositoTransfereByStatus(eStatusIntegracao.PendenteIntegracao).OrderByDescending
                (p => p.DtTransferencia).ToList();  

            if (lstBatchTransferenciaErro != null && lstBatchTransferenciaErro.Count > 0)
            {
                dtLastIntegracaoTransf = lstBatchTransferenciaErro[0].DtTransferencia;

            }

            if (lstBatchTransferenciaPend != null && lstBatchTransferenciaPend.Count > 0)
            {
                if (lstBatchTransferenciaPend[0].DtTransferencia < dtLastIntegracaoTransf)
                        dtLastIntegracaoTransf = lstBatchTransferenciaPend[0].DtTransferencia;

            }

            if (dtLastIntegracaoTransf < DateTime.Today.Date)
            {
                if (!String.IsNullOrEmpty(lblMensagemErro.Text))
                    lblMensagemErro.Text = lblMensagemErro.Text + Environment.NewLine;

                lblMensagemErro.Text = lblMensagemErro.Text + "Atenção! Existe integração de transferencia pendente desde o dia  (" + dtLastIntegracaoTransf.ToString("dd/MM/yyyy") + ") efetue a correção da integração para continuar usando o sistema.";
                //_blnErroIntegracao = true;

                return true;
            }

            return false;
        }

        private async Task<string> SelectEstab()
        {
            var estabelec = await EstabelecDB.GetEstabelec();
            if (estabelec != null)
            {
                string[] arrayDep = new string[estabelec.Count];
                for (int i = 0; i < estabelec.Count; i++)
                {
                    arrayDep[i] = estabelec[i].CodEstabel + " (" + estabelec[i].Nome.Trim() + ")";
                }

                var action = await DisplayActionSheet("Escolha o Estabelecimento?", "Cancelar", null, arrayDep);

                if (action == "Cancelar" && action != null)
                {
                    SecurityAuxiliar.Estabelecimento = action;
                    //

                    lblCodEstabel.Text   = action;
                    frameEstab.IsVisible = true;

                    return SecurityAuxiliar.Estabelecimento;
                }
                else
                {
                    SecurityAuxiliar.Estabelecimento = String.Empty;
                    lblCodEstabel.Text               = String.Empty;
                    frameEstab.IsVisible             = false;

                }
            }
            else
                await DisplayAlert("Erro!", "Nenhum estabelecimento encontrado.", "Cancelar");

            return String.Empty;
        }

        async public Task<bool> Logoff()
        {
            var result = await DisplayAlert("Autenticação!", "Deseja sair do sistema?", "Sim", "Não");

            if (result.ToString() == "True")
            {
                await SecurityDB.AtualizarSecurityLogin(new SecurityVO
                {
                    CodUsuario = String.Empty,
                    CodSenha = String.Empty,
                    Autenticado = false
                });

                //await Navigation.PushModalAsync(new LoginPage(/*AtualizaListView */ ));
                ShowModalLogin();
            }

            return true;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            SecurityVO security = await SecurityDB.GetSecurityAsync();

            if (security != null && security.Autenticado)
            {
                SecurityAuxiliar.Autenticado = true;
                SecurityAuxiliar.CodUsuario = security.CodUsuario;
                SecurityAuxiliar.CodSenha = security.CodSenha;
            }

            if (SecurityAuxiliar.Autenticado == false)
            {
                if (!_blnLoginPage)
                {
                    _blnLoginPage = true;
                    //await Navigation.PushModalAsync(new LoginPage(/*AtualizaListView*/));
                    ShowModalLogin();
                }
            }
            else
            {
                _blnLoginPage = false;

                var page = new ProgressBarPopUp("");

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page);

                await AtualizaListView();

                await page.OnClose();

            }
        }

        LoginPage _myLoginPage;
        private async void ShowModalLogin()
        {
            // When you want to show the modal page, just call this method
            // add the event handler for to listen for the modal popping event:
            App.Current.ModalPopping += HandleModalPopping;

            _myLoginPage = new LoginPage();
            await Navigation.PushModalAsync(_myLoginPage);
        }

        private void HandleModalPopping(object sender, ModalPoppingEventArgs e)
        {
            if (e.Modal == _myLoginPage)
            {
                // now we can retrieve that phone number:
                AtualizaListView();
                _myLoginPage = null;

                // remember to remove the event handler:
                App.Current.ModalPopping -= HandleModalPopping;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            var result = Logoff();

            return true;
        }

        async void OnTapped_FrameEstab(object sender, EventArgs e)
        {
            await SelectEstab();
        }

        async void OnTapped_FrameCadastros(object sender, EventArgs e)
        {
            if (!CrossConnectivity.Current.IsConnected)
            {
                await DisplayAlert("Erro!", "Para atualizar os cadastros no sistema o dispositivo deve estar conectado.", "CANCELAR");

                return;
            }

            var page = new ProgressBarPopUp("Carregando Cadastros...");

            await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page);

            try
            {
                var result = await Models.Controller.ConectColetorAsync(SecurityAuxiliar.CodUsuario, SecurityAuxiliar.CodSenha, page);

                if (result == "OK")
                {
                    await SecurityDB.AtualizarSecurityIntegracao();

                    await AtualizaListView();

                    //AtualizaListView();
                }

            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");

            }
            finally
            {
                await page.OnClose();
            }
        }


        async void OnSelection (object sender, SelectedItemChangedEventArgs e)
        {
            try
            {
                if (e.SelectedItem == null)
                {
                    return;
                }

                MenuItemDetail menuItemDetail = new MenuItemDetail();
                menuItemDetail = (MenuItemDetail)e.SelectedItem;

                //((ListView)sender).SelectedItem = null;

                switch (menuItemDetail.Name)
                {
                    case "Armazenagem":

                        if (!String.IsNullOrEmpty(lblMensagemErro.Text))
                        {
                            await DisplayAlert("Erro!", lblMensagemErro.Text, "OK");
                            return;
                        }
                        
                        //if (String.IsNullOrEmpty(SecurityAuxiliar.Estabelecimento))
                        //{
                        //    var strEstab = await SelectEstab();
                        //    if (strEstab == "Cancelar" || String.IsNullOrEmpty(strEstab))
                        //        return;
                        //}

                        // Victor Alves - 08/01/2020 - Busca inventarios ativos, se tiver inventario ativo, não entra na tela
                        //var lstInventario = InventarioDB.GetInventarioAtivoByEstab(SecurityAuxiliar.GetCodEstabel()).ToList();
                        //if (lstInventario != null && lstInventario.Count > 0)
                        //{
                        //    await DisplayAlert("Erro!", "Existe inventário ativo para o estabelecimento (" + SecurityAuxiliar.GetCodEstabel() + ") e não é possivel acessar a tela de Almoxarifado.", "OK");
                        //    return;
                        //}

                        //if (!String.IsNullOrEmpty(SecurityAuxiliar.Estabelecimento))
                        //{
                        //    RecebimentoPage.InicialPage = menuItemDetail.MenuItemDatailId;
                        //    Application.Current.MainPage = new NavigationPage(new ArmazenagemPage());
                        //}

                        ArmazenagemPage.InicialPage = menuItemDetail.MenuItemDatailId;
                        Application.Current.MainPage = new NavigationPage(new ArmazenagemPage());

                        break;

                    case "Recebimento":

                        if (!String.IsNullOrEmpty(lblMensagemErro.Text))
                        {
                            await DisplayAlert("Erro!", lblMensagemErro.Text, "OK");
                            return;
                        }

                        //if (String.IsNullOrEmpty(SecurityAuxiliar.Estabelecimento))
                        //{
                        //    var strEstab = await SelectEstab();
                        //    if (strEstab == "Cancelar" || String.IsNullOrEmpty(strEstab))
                        //        return;
                        //}

                        //if (!String.IsNullOrEmpty(SecurityAuxiliar.Estabelecimento))
                        //{
                        //    RecebimentoPage.InicialPage = menuItemDetail.MenuItemDatailId;
                        //    Application.Current.MainPage = new NavigationPage(new RecebimentoPage());
                        //}

                        RecebimentoPage.InicialPage = menuItemDetail.MenuItemDatailId;
                        Application.Current.MainPage = new NavigationPage(new RecebimentoPage());

                        break;

                    case "Estabelecimento":

                        if (!String.IsNullOrEmpty(lblMensagemErro.Text))
                        {
                            await DisplayAlert("Erro!", lblMensagemErro.Text, "OK");
                            return;
                        }

                        await SelectEstab();

                        break;

                    case "Integração TOTVS":

                        Application.Current.MainPage = new NavigationPage(new IntegracaoMovtoPage() { Title = menuItemDetail.Name.Trim() });
                        //Application.Current.MainPage = new NavigationPage(new IntegracaoPage());
                        break;

                    case "Logoff":
                        await Logoff();
                        break;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "CANCELAR");
            }
            finally
            {
                ((ListView)sender).SelectedItem = null;
                listView.IsEnabled = true;
            }
        }
    }
}



