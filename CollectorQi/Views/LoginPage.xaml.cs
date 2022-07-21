using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CollectorQi.Resources;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.Models;
using System.Threading.Tasks;
using CollectorQi.Services.ESCL002;
//using Android.Widget;

namespace CollectorQi.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        //public Action<Task> ActAtualizaListView;

        private int _qtdClick;
        private DateTime _ultClick;

        public LoginPage(/*Action<Task> byAtualizaListView*/)
        {
            InitializeComponent();
            BtnLogin.Clicked += BtnLogin_Clicked;

            //ActAtualizaListView = byAtualizaListView;

            BindingContext = this;

            //usuario.Text = "consultoria";
            //senha.Text = "mudar@123";

           
        }

        async void OnTapped_Img(object sender, EventArgs e)
        {
            _qtdClick++;

            /*TimeSpan difference = DateTime.Now - dtSqlConnect;

            if (difference >= TimeSpan.FromSeconds(120) && sqliteConnect != null)
            {
                sqliteConnect.CloseAsync();
                sqliteConnect = null;
            }*/

            if (_qtdClick == 10)
            {
                //Toast.MakeText(Android.App.Application.Context, App.CallProcedureWithToken.GetWSUrl(), Android.Widget.ToastLength.Long).Show();

                //await DisplayAlert("Conexão Mobile", App.CallProcedureWithToken.GetWSUrl(), "CANCELAR");
                _qtdClick = 0;
            }
        }

        protected override bool OnBackButtonPressed()
        {
            DisplayAlert("Autenticação", "Usuário/Senha invalida!", "Cancelar");

            return true;
        }

        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            var page = new ProgressBarPopUp("Carregando Cadastros...");

            try
            {
                BtnLogin.IsEnabled = false;

                //if (ItemDB.GetItemCount()             <= 0 ||
                //    SaldoEstoqDB.GetSaldoEstoqCount() <= 0)
                //{
                //    await DisplayAlert("Atenção!", "Primeira conexão com o Sistema, o processo de Integração pode demorar.", "OK");
                //}

                await Rg.Plugins.Popup.Services.PopupNavigation.Instance.PushAsync(page);

                string logon = String.Empty;

                try
                {
                    //logon = await Models.Controller.ConectColetorAsync(usuario.Text, senha.Text, page);

                    var t = new Cadastros();

                    logon = await t.ObterListaFiliais(usuario.Text, senha.Text);

                    if (logon != "OK")
                    {

                        logon = await t.ObterListaFiliais(usuario.Text + "@DIEBOLD_MASTER", senha.Text);
                    }


                }
                catch (Exception ex)
                {
                    await DisplayAlert("Erro!?", ex.Message, "Cancelar");
                    logon = "Erro";
                    //logon = "OK";

                }

                if (logon == "OK")
                {
                    await SecurityDB.DeleteSecurity();

                    VO.SecurityVO security = new VO.SecurityVO();

                    security.Autenticado = true;
                    security.CodUsuario = usuario.Text.Trim();
                    security.CodSenha = senha.Text.Trim();

                    await SecurityDB.AtualizarSecurityLogin(security);
                    await SecurityDB.AtualizarSecurityIntegracao();

                    SecurityAuxiliar.Autenticado = true;
                    SecurityAuxiliar.CodUsuario = usuario.Text.Trim();
                    SecurityAuxiliar.CodSenha = senha.Text.Trim();

                    await Navigation.PopModalAsync(true);

                    //ActAtualizaListView();

                }
                else if (logon != "Erro")
                {   
                    await DisplayAlert("Autenticação", "Usuário/Senha invalida!", "Cancelar");
                    senha.TextColor = Color.Red;   
                    usuario.Focus();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro!", ex.Message, "Cancelar");
            }
            finally
            {
                await page.OnClose();
                BtnLogin.IsEnabled = true;
            }
        }
    }

}