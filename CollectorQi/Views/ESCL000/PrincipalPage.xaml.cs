﻿using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Resources.DataBaseHelper.Batch;
using CollectorQi.Resources.DataBaseHelper.Batch.ESCL018;
using CollectorQi.Services;
using CollectorQi.Services.ESCL000;
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
                var versaoSistema    = new ValidaVersaoSistema();
                
                // Validar
                var existeNovaVersao = await versaoSistema.ExisteNovaVersao();

                if (!existeNovaVersao.LoginValidado)
                {
                    await DisplayAlert("Nova Versao", existeNovaVersao.Mensagem , "OK");
                    Uri urlVersao = new Uri(existeNovaVersao.LinkVersao);
                    await Launcher.OpenAsync(urlVersao);

                    Application.Current.Quit();
                    System.Diagnostics.Process.GetCurrentProcess().Kill();
                    System.Environment.Exit(0);
                    return;
                }

                var security         = await SecurityDB.GetSecurityAsync();
                
                if (security != null && security.Autenticado && existeNovaVersao.LoginValidado)
                {
                    SecurityAuxiliar.Autenticado = true;
                    SecurityAuxiliar.CodUsuario  = security.CodUsuario;
                    SecurityAuxiliar.CodSenha    = security.CodSenha;
                }

                // Victor Alves - Verifica ultima atualizacao Banco
                await SecurityDB.AtualizarSecurityIntegracao();

                if (SecurityAuxiliar.EmpresaAll != null && SecurityAuxiliar.EmpresaAll.Count > 0)
                {
                    if (SecurityAuxiliar.EmpresaAll.Count == 1)
                    {
                        var empCurrent = SecurityAuxiliar.EmpresaAll.FirstOrDefault();
                        SecurityAuxiliar.CodEmpresa = empCurrent.codEmpresa + " (" + empCurrent.nomEmpresa.Trim() + ")";
                    }
                    else
                    {
                        if (String.IsNullOrEmpty(SecurityAuxiliar.CodEmpresa))
                        {
                            await SelectEmpresa();
                        }
                    }
                }
                else
                {
                    throw new Exception("Nenhuma empresa cadastrar para o usuário");
                }

                if (security != null && existeNovaVersao.LoginValidado)
                {
                    if (CrossConnectivity.Current.IsConnected)
                    {
                        Task.Run(() => Services.ESCL000.ConnectService.ConnectColetorAsync(SecurityAuxiliar.CodUsuario, SecurityAuxiliar.CodSenha, null));
                    }

                    lblMensagemErro.Text = String.Empty;

                    footerCodUsuario.Text = SecurityAuxiliar.CodUsuario;
                    footerVersion.Text    = "v" + VersionTracking.CurrentVersion;

                    /* Valida integração de cadastros */
                    //if (security.DtUltIntegracao.AddDays(30) < DateTime.Today.Date)
                    //{
                    //    lblMensagemErro.Text = "Atenção! Ultima integração efetuada dia (" + security.DtUltIntegracao.Date.ToString("dd/MM/yyyy") + ") acesse a internet pelo dispositivo para efetuar a integração e continuar usando o sistema.";
                    //}

                    //ErroIntegracaoTransferencia();
                    //ErroIntegracaoInventario();

                    //string[] imagem = new string[] { "almoxarifado.png", "inventario.png", "expedicao.png", "logoTotvs.png", "logout.png" };
                    //string[] titulo = new string[] { "Almoxarifado", "Inventário", "Estabelecimento", "Integração TOTVS", "Logoff" };
                    //string[] subTitulo = new string[] { "Opções do Almoxarifado" ,
                    //                                "Inventariar produtos",
                    //                                "Escolher o estabelecimento",
                    //                                "Ultima Integração (" + security.DtUltIntegracaoerro par  + ")",
                    //                                "Sair da conta: " + SecurityAuxiliar.CodUsuario };

                    string[] imagem = new string[] { "armazenagem.png"    , 
                                                     "conferencia.png"    , 
                                                     "inventario.png"     ,
                                                     "estabelecimento.png",
                                                     "integration.png"    , 
                                                     "logout.png" };

                    string[] titulo = new string[] { "Armazenagem"     , 
                                                     "Recebimento"     ,
                                                     "Inventário"      ,
                                                     "Estabelecimento" , 
                                                     "Integração TOTVS", "Logoff" };

                    string[] subTitulo = new string[] { "Armazenagens em Depósito", 
                                                        "Conferência Física, Recebimento e Reparos", 
                                                        "Inventário Físico e Reparos", 
                                                        "Escolher o estabelecimento", 
                                                        "Última Integração",
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
                        frameEstab.IsVisible = true;
                    }
                    else
                    {
                        frameCadastros.IsVisible = false;

                        if (!String.IsNullOrEmpty(SecurityAuxiliar.CodEmpresa))
                        {
                            lblCodEmpresa.Text = SecurityAuxiliar.CodEmpresa;
                            frameEmpresa.IsVisible = true;
                        }
                        else
                        {
                            lblCodEmpresa.Text = String.Empty;
                            frameEmpresa.IsVisible = true;
                        }

                        if (!String.IsNullOrEmpty(SecurityAuxiliar.Estabelecimento))
                        {
                            lblCodEstabel.Text = SecurityAuxiliar.Estabelecimento;
                            frameEstab.IsVisible = true;
                        }
                        else
                        {
                            lblCodEstabel.Text = String.Empty;
                            frameEstab.IsVisible = true;
                        }
                    }

                    if (String.IsNullOrEmpty(lblCodEstabel.Text))
                    {
                        lblCodEstabel.Text = "Escolha o Estabelecimento";
                    }

                    if (String.IsNullOrEmpty(lblCodEmpresa.Text))
                    {
                        lblCodEmpresa.Text = "Escolha a Empresa";
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

            var lstBatchInventarioErro = BatchInventarioItemDB.GetBatchInventarioByStatus(eStatusIntegracao.ErroIntegracao).
                OrderByDescending(p => p.DtIntegracao).ToList();

            var lstBatchInventarioPend = BatchInventarioItemDB.GetBatchInventarioByStatus(eStatusIntegracao.PendenteIntegracao).
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

        private async Task<string> SelectEmpresa()
        {

            try
            {
                var empresas = SecurityAuxiliar.EmpresaAll;

                if (empresas != null)
                {
                    string[] arrayDep = new string[empresas.Count];
                    for (int i = 0; i < empresas.Count; i++)
                    {
                        arrayDep[i] = empresas[i].codEmpresa + " (" + empresas[i].nomEmpresa.Trim() + ")";
                    }


                    if (empresas.Count == 1)
                    {
                        SecurityAuxiliar.CodEmpresa = empresas[0].codEmpresa + " (" + empresas[0].nomEmpresa.Trim() + ")";

                        /*lblCodEstabel.Text = SecurityAuxiliar.Estabelecimento;
                        frameEstab.IsVisible = true;
                        */

                        await ConnectService.CarregarListaFilial();

                        SecurityAuxiliar.Estabelecimento = "";
                        lblCodEstabel.Text = "Escolha o Estabelecimento";


                        return SecurityAuxiliar.CodEmpresa;
                    }
                    else
                    {
                        var action = await DisplayActionSheet("Escolha a Empresa?", "Cancelar", null, arrayDep);

                        if (action != "Cancelar" && action != null)
                        {
                            SecurityAuxiliar.CodEmpresa = action;

                            lblCodEmpresa.Text = action;
                            frameEmpresa.IsVisible = true;

                            await ConnectService.CarregarListaFilial();


                            SecurityAuxiliar.Estabelecimento = "";
                            lblCodEstabel.Text = "Escolha o Estabelecimento";
                            /*
                            SecurityAuxiliar.Estabelecimento = action;
                            lblCodEstabel.Text = action;
                            frameEstab.IsVisible = true;
                            */

                            return SecurityAuxiliar.CodEmpresa;
                        }
                        else
                        {
                            SecurityAuxiliar.CodEmpresa = String.Empty;
                            lblCodEmpresa.Text = String.Empty;
                            frameEmpresa.IsVisible = false;
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Erro!", "Nenhum empresa encontrada.", "Cancelar");
                }

                System.Diagnostics.Debug.Write(empresas);

            }
            catch (Exception e)
            {
                await DisplayAlert("Erro!", "Não foi possivel carregar os estabelecimentos da empresa " + e.Message, "OK");
            }
            return String.Empty;
        }

        private async Task<string> SelectEstab()
        {
            var estabelec = await EstabelecDB.GetEstabelec();
            if (estabelec != null && estabelec.Count > 0)
            {
                string[] arrayDep = new string[estabelec.Count];
                for (int i = 0; i < estabelec.Count; i++)
                {
                    arrayDep[i] = estabelec[i].CodEstabel + " (" + estabelec[i].Nome.Trim() + ")";
                }

                if (estabelec.Count == 1)
                {
                    SecurityAuxiliar.Estabelecimento = estabelec[0].CodEstabel + " (" + estabelec[0].Nome.Trim() + ")";

                    lblCodEstabel.Text = SecurityAuxiliar.Estabelecimento;
                    frameEstab.IsVisible = true;

                    return SecurityAuxiliar.Estabelecimento;
                }
                else
                {

                    var action = await DisplayActionSheet("Escolha o Estabelecimento?", "Cancelar", null, arrayDep);

                    if (action != "Cancelar" && action != null)
                    {
                        SecurityAuxiliar.Estabelecimento = action;
                        lblCodEstabel.Text = action;
                        frameEstab.IsVisible = true;

                        return SecurityAuxiliar.Estabelecimento;
                    }
                    else
                    {
                        SecurityAuxiliar.Estabelecimento = String.Empty;
                        lblCodEstabel.Text = String.Empty;
                        frameEstab.IsVisible = false;
                    }
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

                SecurityAuxiliar.CodUsuario = "";
                SecurityAuxiliar.CodSenha = "";

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
            //await SelectEmpresa();
            await SelectEstab();
        }

        async void OnTapped_FrameEmpresa(object sender, EventArgs e)
        {
            await SelectEmpresa();
            //await SelectEstab();
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
                var result = await Models.ConnectService.ConectColetorAsync(SecurityAuxiliar.CodUsuario, SecurityAuxiliar.CodSenha, page);

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

        async void OnSelection(object sender, SelectedItemChangedEventArgs e)
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

                        if (String.IsNullOrEmpty(SecurityAuxiliar.CodEmpresa))
                        {
                            var strEmpresa = await SelectEmpresa();
                            if (strEmpresa == "Cancelar" || String.IsNullOrEmpty(strEmpresa))
                            {
                                return;
                            }
                        }

                        if (String.IsNullOrEmpty(SecurityAuxiliar.Estabelecimento))
                        {
                          

                            var strEstab = await SelectEstab();
                            if (strEstab == "Cancelar" || String.IsNullOrEmpty(strEstab))
                                return;
                        }


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

                        if (String.IsNullOrEmpty(SecurityAuxiliar.CodEmpresa))
                        {
                            var strEmpresa = await SelectEmpresa();
                            if (strEmpresa == "Cancelar" || String.IsNullOrEmpty(strEmpresa))
                            {
                                return;
                            }
                        }

                        if (String.IsNullOrEmpty(SecurityAuxiliar.Estabelecimento))
                        {

                            var strEstab = await SelectEstab();
                            if (strEstab == "Cancelar" || String.IsNullOrEmpty(strEstab))
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


                    case "Inventário":

                        if (!String.IsNullOrEmpty(lblMensagemErro.Text))
                        {
                            await DisplayAlert("Erro!", lblMensagemErro.Text, "OK");
                            return;
                        }

                        if (String.IsNullOrEmpty(SecurityAuxiliar.CodEmpresa))
                        {
                            var strEmpresa = await SelectEmpresa();
                            if (strEmpresa == "Cancelar" || String.IsNullOrEmpty(strEmpresa))
                                return;
                        }

                        if (String.IsNullOrEmpty(SecurityAuxiliar.Estabelecimento))
                        {
                            var strEstab = await SelectEstab();
                            if (strEstab == "Cancelar" || String.IsNullOrEmpty(strEstab))
                                return;
                        }

                        //  if (!String.IsNullOrEmpty(SecurityAuxiliar.Estabelecimento))
                        //  {
                        //      RecebimentoPage.InicialPage = menuItemDetail.MenuItemDatailId;
                        //      Application.Current.MainPage = new NavigationPage(new RecebimentoPage());
                        //  }

                        //  Application.Current.MainPage = new NavigationPage(new InventarioListaPage() { Title = menuItemDetail.Name.Trim() });

                        Application.Current.MainPage = new NavigationPage(new InventarioPage() { Title = menuItemDetail.Name.Trim() });
                        break;

                    case "Expedição":

                        if (!String.IsNullOrEmpty(lblMensagemErro.Text))
                        {
                            await DisplayAlert("Erro!", lblMensagemErro.Text, "OK");
                            return;
                        }

                        if (String.IsNullOrEmpty(SecurityAuxiliar.CodEmpresa))
                        {
                            var strEmpresa = await SelectEmpresa();
                            if (strEmpresa == "Cancelar" || String.IsNullOrEmpty(strEmpresa))
                                return;
                        }

                        if (String.IsNullOrEmpty(SecurityAuxiliar.Estabelecimento))
                        {
                            var strEstab = await SelectEstab();
                            if (strEstab == "Cancelar" || String.IsNullOrEmpty(strEstab))
                                return;
                        }

                        //  if (!String.IsNullOrEmpty(SecurityAuxiliar.Estabelecimento))
                        //  {
                        //      RecebimentoPage.InicialPage = menuItemDetail.MenuItemDatailId;
                        //      Application.Current.MainPage = new NavigationPage(new RecebimentoPage());
                        //  }

                        //  Application.Current.MainPage = new NavigationPage(new InventarioListaPage() { Title = menuItemDetail.Name.Trim() });

                       // Application.Current.MainPage = new NavigationPage(new GerarPedidoDepositoViewModel() { Title = menuItemDetail.Name.Trim() });
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



