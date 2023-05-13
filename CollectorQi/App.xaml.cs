using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CollectorQi.Views;
using CollectorQi.Models;
using CollectorQi.Services;
using Matcha.BackgroundService;
using CollectorQi.VO.Batch;
using CollectorQi.VO;
using AutoMapper;
using System.Threading.Tasks;
using System.Threading;
using Xamarin.Essentials;
using CollectorQi.Resources.DataBaseHelper;
using Rg.Plugins.Popup.Services;
using CollectorQi.Resources;
using ESCL = CollectorQi.Models.ESCL018;
using CollectorQi.VO.ESCL018;
using CollectorQi.VO.Batch.ESCL018;
using System.Globalization;
using CollectorQi.Services.ESCL000;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace CollectorQi
{
    public partial class App : Application
    {
        private event EventHandler Starting = delegate { };

        public bool isNotification;

        public App()
        {
            try
            {
                InitializeComponent();

                //Cultura BR
                ServiceCommon.SetarAmbienteCulturaBrasil();

                if (String.IsNullOrEmpty(csAuxiliar.idNotify))
                {
                    AutoMapper.Mapper.Initialize(Load());

                    /* Atualiza banco de dados */
                    DataBase db = new DataBase();
                    db.CriarBancoDeDados();

                    //Criar Tabelas Temporarias
                    db.CriarTabelasTemporarias();

                    // Code to run on the main thread
                    MainPage = new NavigationPage(new PrincipalPage());
                }
                else
                {

                    if (csAuxiliar.idNotify == eTpNotificacao.Transferencia.ToString())
                        MainPage = new NavigationPage(new IntegracaoMovtoTransferenciaPage());

                    else if (csAuxiliar.idNotify == eTpNotificacao.Inventario.ToString())
                        MainPage = new NavigationPage(new IntegracaoMovtoInventarioPage());

                } 

                VersionTracking.Track();
            }
            catch (Exception ex)
            {
                Application.Current.MainPage.DisplayAlert("Erro Aplicação!", ex.ToString(), "OK");
            }

            // First time ever launched application
            var firstLaunch = VersionTracking.IsFirstLaunchEver;

            // First time launching current version
            var firstLaunchCurrent = VersionTracking.IsFirstLaunchForCurrentVersion;

            // First time launching current build
            var firstLaunchBuild = VersionTracking.IsFirstLaunchForCurrentBuild;

            // Current app version (2.0.0)
            var currentVersion = VersionTracking.CurrentVersion;

            // Current build (2)
            var currentBuild = VersionTracking.CurrentBuild;

            // Previous app version (1.0.0)
            var previousVersion = VersionTracking.PreviousVersion;

            // Previous app build (1)
            var previousBuild = VersionTracking.PreviousBuild;

            // First version of app installed (1.0.0)
            var firstVersion = VersionTracking.FirstInstalledVersion;

            // First build of app installed (1)
            var firstBuild = VersionTracking.FirstInstalledBuild;

            // List of versions installed (1.0.0, 2.0.0)
            var versionHistory = VersionTracking.VersionHistory;

            // List of builds installed (1, 2)
            var buildHistory = VersionTracking.BuildHistory;
        }

        protected async override void OnStart()
        {

            await Task.Run(() => StartBackgroundService());

            await Task.Run(async () => {

                SecurityAuxiliar.ItemAll = await ItemDB.GetItemsAsync();
            }); 
        }

        public static Action<AutoMapper.IMapperConfigurationExpression> Load()
        {
            return _ =>
            {
                _.CreateMap<BatchDepositoTransfereVO, BatchDepositoTransfereViewModel>();
                _.CreateMap<InventarioVO, BatchInventarioItemVO>();
                _.CreateMap<BatchInventarioItemVO, BatchInventarioViewModel>();
                _.CreateMap<InventarioItemVO, InventarioItemViewModel>();
                _.CreateMap<InventarioVO, InventarioFisicoViewModel>();
                _.CreateMap<RequisicaoItemVO, RequisicaoItemViewModel>();
                _.CreateMap<RequisicaoItemViewModel, RequisicaoItemVO>();

                // Nova atualizacao
                _.CreateMap<InventarioItemVO, BatchInventarioItemVO>();
                _.CreateMap<BatchInventarioItemVO, InventarioItemVO>();
                _.CreateMap<NotaFiscalViewModel, NotaFiscalVO>();
                //_.CreateMap<ESCL.Parametros, InventarioViewModel>();
                //_.CreateMap<SaldoEstoqVO, RequisicaoSaldoEstoqViewModel>();
                //_.CreateMap add ma s configuração.

                //   var modelView = Mapper.Map<RequisicaoItemViewModel, RequisicaoItemVO>(_currentClick);

            };
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private static void StartBackgroundService()
        {
            //Atualiza a cada 5 Minutos
            BackgroundAggregatorService.Add(() => new IntegracaoServiceMovto(8));

            // Atualiza a cada 60 minutos os cadastros TOTVS
            //BackgroundAggregatorService.Add(() => new IntegracaoServiceCad(60));

            //Inicia o Background Service
            BackgroundAggregatorService.StartBackgroundService();
        }

        private static ICallProcedureWithToken _CallProcedureWithToken;
        public static ICallProcedureWithToken CallProcedureWithToken
        {
            get
            {
                if (_CallProcedureWithToken == null)
                {
                    _CallProcedureWithToken = DependencyService.Get<ICallProcedureWithToken>();
                }

                return _CallProcedureWithToken;
            }
        }

        private static ICallNotification _CallNotification;
        public static ICallNotification CallNotification
        {
            get
            {
                if (_CallNotification == null)
                {
                    _CallNotification = DependencyService.Get<ICallNotification>();
                }

                return _CallNotification;
            }
        }

    }
}
