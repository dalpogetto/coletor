using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

using CollectorQi.VO;
using CollectorQi.VO.Batch;


using Xamarin.Forms.Internals;
using Xamarin.Essentials;
using System.IO;
using CsvHelper;
using CollectorQi.VO.ESCL018;
using CollectorQi.VO.Batch.ESCL018;

namespace CollectorQi.Resources.DataBaseHelper
{
    public class DataBase
    {
        string pasta = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        string banco = "CollectorQualiIT.db";

        public DataBase()
        {
            //CriarBancoDeDados();
        }

        /*
        public SQLiteConnection get()
        {
            SQLiteConnection conexao = new SQLiteConnection(System.IO.Path.Combine(pasta, banco));
            return conexao;
        }*/

        public bool IsTableExists(string tableName, SQLiteConnection conexao)
        {
            try
            {
                var tableInfo = conexao.GetTableInfo(tableName);
                if (tableInfo.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool CriarBancoDeDados()
        {
            Version version = new Version();

            try
            {
                string teste = Path.GetFullPath(pasta);

                if (!File.Exists(Path.Combine(pasta, banco)) || VersionTracking.IsFirstLaunchForCurrentVersion)
                {
                    using (SQLiteConnection conexao = new SQLiteConnection(Path.Combine(pasta, banco)))
                    {

                        /* Se for ambiente (1.2.0), limpa o banco de dados */
                        conexao.DropTable<UsuarioVO>();
                        conexao.DropTable<ItemVO>();
                        conexao.DropTable<SecurityVO>();
                        conexao.DropTable<InventarioVO>();
                        conexao.DropTable<InventarioLocalizacaoVO>();
                        conexao.DropTable<InventarioItemVO>();
                        conexao.DropTable<DepositoVO>();
                        conexao.DropTable<EstabelecVO>();
                        conexao.DropTable<SaldoEstoqVO>();
                        conexao.DropTable<RequisicaoVO>();
                        conexao.DropTable<RequisicaoItemVO>();
                        conexao.DropTable<RequisicaoItemSaldoEstoqVO>();
                        conexao.DropTable<FichasUsuarioVO>();
                        conexao.DropTable<NotaFiscalVO>();

                        conexao.DropTable<BatchInventarioItemVO>();


                        if (!IsTableExists("UsuarioVO", conexao))
                            conexao.CreateTable<UsuarioVO>();

                        if (!IsTableExists("ItemVO", conexao))
                            conexao.CreateTable<ItemVO>();

                        if (!IsTableExists("SecurityVO", conexao))
                            conexao.CreateTable<SecurityVO>();

                        if (!IsTableExists("InventarioVO", conexao))
                            conexao.CreateTable<InventarioVO>();

                        if (!IsTableExists("InventarioLocalizacaoVO", conexao))
                        {
                            conexao.CreateTable<InventarioLocalizacaoVO>();
                        }

                        if (!IsTableExists("InventarioItemVO", conexao))
                            conexao.CreateTable<InventarioItemVO>();

                        if (!IsTableExists("DepositoVO", conexao))
                            conexao.CreateTable<DepositoVO>();

                        if (!IsTableExists("EstabelecVO", conexao))
                            conexao.CreateTable<EstabelecVO>();

                        if (!IsTableExists("SaldoEstoqVO", conexao))
                            conexao.CreateTable<SaldoEstoqVO>();

                        if (!IsTableExists("RequisicaoVO", conexao))
                            conexao.CreateTable<RequisicaoVO>();

                        if (!IsTableExists("RequisicaoItemVO", conexao))
                            conexao.CreateTable<RequisicaoItemVO>();

                        if (!IsTableExists("RequisicaoItemSaldoEstoqVO", conexao))
                            conexao.CreateTable<RequisicaoItemSaldoEstoqVO>();

                        /* Integracao BATCH  */
                        //conexao.DropTable<BatchDepositoTransfereVO>();
                        if (!IsTableExists("BatchDepositoTransfereVO", conexao))
                            conexao.CreateTable<BatchDepositoTransfereVO>();

                        if (!IsTableExists("BatchInventarioItemVO", conexao))
                        {
                            conexao.CreateTable<BatchInventarioItemVO>();
                        }

                        if (!IsTableExists("FichasUsuarioVO", conexao))
                            conexao.CreateTable<FichasUsuarioVO>();

                        if (!IsTableExists("NotaFiscalVO", conexao))
                            conexao.CreateTable<NotaFiscalVO>();
                    }
                }

                return true;  
            }
            catch (SQLiteException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                //dbAsync.Connection.CloseAsync();
            } 
                

        }
    }
}