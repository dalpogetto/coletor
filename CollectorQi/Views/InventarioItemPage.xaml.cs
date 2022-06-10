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

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CollectorQi.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class InventarioItemPage : ContentPage
	{
        private static InventarioVO inventario;
        private static ItemVO item_VO;
        private static int menuId = 0;
        private static string menuDesc = "";
        private static bool volta = false;


        public static InventarioVO Inventario { get => inventario; set => inventario = value; }
        public static ItemVO Item_VO { get => item_VO; set => item_VO = value; }
        public static int MenuId { get => menuId; set => menuId = value; }
        public static string MenuDesc { get => menuDesc; set => menuDesc = value; }
        public static bool Volta { get => volta; set => volta = value; }

        public InventarioItemPage ()
		{
			InitializeComponent ();
            if (SecurityAuxiliar.Autenticado == false)
            {
                DisplayAlert("Autenticação", "Sinto muito!!! Precisa estar autenticado na página de controle", "OK");
                Application.Current.MainPage = new NavigationPage(new PrincipalPage());
            }
            else
            {
                if (Inventario != null)
                {
                    lblData.Text = String.Format("{0:dd/MM/yyyy}", inventario.DtInventario);
                    lblDeposito.Text = inventario.CodDepos;
                    lblContagem.Text = inventario.Contagem.ToString();
                }

                if (Volta)
                {
                    Volta = false;
                    //if (RecebimentoPage.Item_VO != null)
                    //{
                    //    Fill(RecebimentoPage.Item_VO);
                    //}
                }
                else
                {
                    Limpar();
                }
                footerCodUsuario.Text = SecurityAuxiliar.CodUsuario;
            }
        }

        void OnClick_QR(object sender, EventArgs e)
        {
            DisplayAlert("Sim", "Antes Clicar QR", "OK");
            //RecebimentoPage.MenuId = MenuId;
            //RecebimentoPage.MenuDesc = MenuDesc;
            Application.Current.MainPage = new NavigationPage(new RecebimentoPage() { Title = "Leitor Teste 123: " + MenuDesc });
            edtLote.Focus();
        }

        void Fill(ItemVO byItemVO)
        {
            edtItCodigo.Text = byItemVO.ItCodigo;
            edtDescItem.Text = byItemVO.DescItem;
            edtUn.Text = byItemVO.Un;
            Item_VO = byItemVO;
        }

        void Fill(InventarioItemVO byInventarioItem)
        {
         /*   edtLote.Text = byInventarioItem.Lote.Trim();
            edtQuantidade.Text = String.Format("#######0.0", byInventarioItem.Quantidade); */
        }

        void Voltar_Clicked(object sender, System.EventArgs e)
        {
            Application.Current.MainPage = new NavigationPage(new PrincipalPage());
        }

        void Limpar_Clicked(object sender, System.EventArgs e)
        {
            Limpar();
        }

        void BtnAdicionarItem_Clicked(object sender, System.EventArgs e)
        {
            //await DisplayAlert("Teste", "Adicionando Item", "Cancel");
            MenuItemDetail menuItemDetail;

            List<MenuItemDetail> menuItemDetails = new List<MenuItemDetail>();

            menuItemDetail = new MenuItemDetail { MenuItemDatailId = 1, Name = "Teste", SubTitle = "Teste2", Image = "" };

            menuItemDetails.Add(menuItemDetail);

            listView.ItemsSource = menuItemDetails;

            /*menuItemDetail = new MenuItemDetail { MenuItemDatailId = i + 1, Name = titulo[i], SubTitle = subTitulo[i], Image = imagem[i] };*/
            /*
            footerCodUsuario.Text = Security.CodUsuario;
            footerEstabelecimento.Text = Security.Estabelecimento;

            string[] imagem = new string[] { "fisica.png", "deposito.png" /*, "abastecimento.png", "retirada.png", , "voltar.png" };
            string[] titulo = new string[] { "Física", "Depósito", /*"Abastecimento", "Retirada",  "Voltar" };
            string[] subTitulo = new string[] { "Entrada Física", "Transferência Depósito" /*, "Entrada de material", "Retirada de material", , "Voltar ao menu principal" };

            List<MenuItemDetail> menuItemDetails = new List<MenuItemDetail>();
            MenuItemDetail menuItemDetail;
            for (int i = 0; i < imagem.Count(); i++)
            {
                menuItemDetail = new MenuItemDetail { MenuItemDatailId = i + 1, Name = titulo[i], SubTitle = subTitulo[i], Image = imagem[i] };

                menuItemDetails.Add(menuItemDetail);
            }

            listView.ItemsSource = menuItemDetails;
            listView.ItemSelected += OnSelection;*/

        }

        void Ok_Clicked(object sender, System.EventArgs e)
        {
            edtLote.Text = edtLote.Text.ToUpper().Trim();
            Double Qtde = 0;
            if (Double.TryParse(edtQuantidade.Text.Trim(), out Qtde))
                edtQuantidade.Text = String.Format("#######0.0", Qtde); 
            else
                DisplayAlert("Atenção!!!", "O campo QUANTIDADE precisa estar preenchido corretamente", "OK");

            if (edtLote.Text.Trim() == "")
            {
                DisplayAlert("Atenção!!!", "O campo LOTE precisa estar preenchido", "OK");
                edtLote.Focus();
            }
            else if (edtQuantidade.Text.Trim() == "")
            {
                DisplayAlert("Atenção!!!", "O campo QUANTIDADE precisa estar preenchido", "OK");
                edtQuantidade.Focus();
            }
            else if (edtItCodigo.Text.Trim() == "")
            {
                DisplayAlert("Atenção!!!", "O campo Item precisa estar preenchido", "OK");
                edtItCodigo.Focus();
            }
            else if (Qtde > 0)
            {
                DisplayAlert("Atenção!!!", "O campo QUANTIDADE precisa ser maior que zero", "OK");
                edtQuantidade.Focus();
            }
            else if (Inventario.InventarioId > 0)
            {
                DisplayAlert("Atenção!!!", "O INVENTÁRIO precisa existir", "OK");
                edtQuantidade.Focus();
            }
            else
            {
                InventarioItemVO inventarioItem = new InventarioItemVO();
                inventarioItem.InventarioId = Inventario.InventarioId;
         //       inventarioItem.ItemId = Item_VO.ItemId;
           /*     inventarioItem.CodigoItem = edtItCodigo.Text.Trim();
                inventarioItem.Lote = edtLote.Text.Trim().ToUpper();
                inventarioItem.Quantidade = Qtde;
                InventarioItemDB inventarioItemDB = new InventarioItemDB();
                inventarioItemDB.InserirInventarioItem(inventarioItem);
  //              inventarioItem = inventarioItemDB.GetInventarioItem(inventario.InventarioId, item_VO.ItemId, edtItCodigo.Text.Trim(), edtLote.Text.Trim().ToUpper());
                Fill(inventarioItem);
                ItemVO i = new ItemVO();
                ItemDB itemDB = new ItemDB();
                i = itemDB.GetItem(edtItCodigo.Text);
                Fill(i); */
            }
        }

        void Limpar()
        {
            edtItCodigo.Text = "";
            edtDescItem.Text = "";
            edtUn.Text = "";
            edtQuantidade.Text = "0.0";
            edtLote.Text = "";
            ItemVO i = new ItemVO();
            item_VO = i;
        }
    }
}