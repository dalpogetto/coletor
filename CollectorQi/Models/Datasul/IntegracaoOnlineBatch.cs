using System;
using System.Collections.Generic;
using Plugin.Connectivity;
using CollectorQi.Resources.DataBaseHelper.Batch;
using CollectorQi.Resources.DataBaseHelper;
using CollectorQi.VO.Batch;
using CollectorQi.VO;
using AutoMapper;
using System.Threading.Tasks;

namespace CollectorQi.Models.Datasul
{
    public static class IntegracaoOnlineBatch
    {

        public static Tuple<TipoIntegracao, string> DepositoTransfere(List<ModelDepositoTransfere> lstDepositoTransfere)
        {
            try
            {
                if (CrossConnectivity.Current.IsConnected)
                {
                    /* Integracao Online */

                    var tplRetorno = Controller.SetDepositoTransfere(lstDepositoTransfere);

                    return tplRetorno;
                }
                else
                {
                    /* Integracao Offline */
                    List<VO.Batch.BatchDepositoTransfereVO> transfereDB = new List<VO.Batch.BatchDepositoTransfereVO>();

                    for (int i = 0; i < lstDepositoTransfere.Count; i++)
                    {
                        transfereDB.Add(new VO.Batch.BatchDepositoTransfereVO
                        {
                            CodEstabel = lstDepositoTransfere[i].codEstabel,
                            NroDocto = lstDepositoTransfere[i].nroDocto,
                            ItCodigo = lstDepositoTransfere[i].itCodigo,
                            CodDeposSaida = lstDepositoTransfere[i].codDeposSaida,
                            CodLocaliz = lstDepositoTransfere[i].codLocaliz,
                            CodRefer = lstDepositoTransfere[i].codRefer,
                            CodLote = lstDepositoTransfere[i].codLote,
                            DtValiLote = lstDepositoTransfere[i].dtValiLote,
                            CodDeposEntrada = lstDepositoTransfere[i].codDeposEntrada,
                            QtidadeTransf = lstDepositoTransfere[i].qtidadeTransf,
                            CodUsuario = lstDepositoTransfere[i].codUsuario,

                            DtTransferencia = DateTime.Now,
                            DtIntegracao = DateTime.MinValue,
                            MsgIntegracao = String.Empty,
                            StatusIntegracao = eStatusIntegracao.PendenteIntegracao /* Pendente */
                        });
                    }

                    ///var batchDepositoTransfere = new BatchDepositoTransfereDB();
                    Controller.MovtoEstoqMobile(true, lstDepositoTransfere);
                    BatchDepositoTransfereDB.InserirBatchDepositoTransfere(transfereDB);

                    return Tuple.Create<TipoIntegracao, string>(TipoIntegracao.IntegracaoBatch,
                                                                "Conexão não encontrada, transferencia será confirmada quando habilitar a conexão local.");

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Tuple<TipoIntegracao, string> DepositoTransfereOffiline(List<VO.Batch.BatchDepositoTransfereVO> lstDepositoTransfereVO)
        {
            try
            {
                int iContSucesso = 0;
                int iContErro = 0;
                for (int i = 0; i < lstDepositoTransfereVO.Count; i++)
                {
                    List<ModelDepositoTransfere> models = new List<ModelDepositoTransfere>();

                    models.Add(new ModelDepositoTransfere
                    {

                        codEstabel = lstDepositoTransfereVO[i].CodEstabel,
                        nroDocto = lstDepositoTransfereVO[i].NroDocto,
                        itCodigo = lstDepositoTransfereVO[i].ItCodigo,
                        codDeposSaida = lstDepositoTransfereVO[i].CodDeposSaida,
                        codLocaliz = lstDepositoTransfereVO[i].CodLocaliz,
                        codRefer = lstDepositoTransfereVO[i].CodRefer,
                        codLote = lstDepositoTransfereVO[i].CodLote,
                        dtValiLote = lstDepositoTransfereVO[i].DtValiLote,
                        codDeposEntrada = lstDepositoTransfereVO[i].CodDeposEntrada,
                        qtidadeTransf = lstDepositoTransfereVO[i].QtidadeTransf,
                        codUsuario = lstDepositoTransfereVO[i].CodUsuario,

                    });

                    var tplRetorno = Controller.SetDepositoTransfere(models);

                    lstDepositoTransfereVO[i].DtIntegracao  = DateTime.Now;
                    lstDepositoTransfereVO[i].MsgIntegracao = tplRetorno.Item2;

                    if (tplRetorno.Item1.ToString() == TipoIntegracao.IntegracaoOnline.ToString())
                    {
                        /* Integração efetuada com sucesso */
                        lstDepositoTransfereVO[i].StatusIntegracao = eStatusIntegracao.EnviadoIntegracao;
                        iContSucesso++;
                    }
                    else
                    {
                        /* Integração efetuada com erro */
                        lstDepositoTransfereVO[i].StatusIntegracao = eStatusIntegracao.ErroIntegracao;
                        iContErro++;
                    }

                    //BatchDepositoTransfereDB db = new BatchDepositoTransfereDB();

                    BatchDepositoTransfereDB.AtualizarBatchDepositoTransfereIntegracaoById(lstDepositoTransfereVO[i]);
                }

                return Tuple.Create<TipoIntegracao, string>(TipoIntegracao.IntegracaoOnline,
                                                            "Transferencia com sucesso " + iContSucesso.ToString() + " e com erro " + iContErro.ToString());

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Tuple<TipoIntegracao, string> InventarioNovo(VO.InventarioVO pInventarioVO)
        {
            try
            {
                /*
                if (CrossConnectivity.Current.IsConnected)
                {
                    /* Integracao Online 

                    var tplRetorno = Controller.SetDepositoTransfere(lstDepositoTransfere);

                    return tplRetorno;
                }
                else*/

                //InventarioDB db = new InventarioDB();

                InventarioDB.InserirInventario(pInventarioVO);

                return Tuple.Create<TipoIntegracao, string>(TipoIntegracao.IntegracaoBatch, "Inventário criado com sucesso");


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void CancelaTransferenciaMobile(BatchDepositoTransfereVO pBatchDepositoTransfere)
        {
            var lstDepositoTransfere = new List<ModelDepositoTransfere>();

            // Atenção
            // A chamada da transferencia deve ser a inversão do que está salvo na tabela de integraçào TOTVS
            // 1- Deposito de saída na integração TOTVS deve ser considerado como entrada
            // 2- Deposito de entrada na integração TOTVS deve ser considerado como saída
            lstDepositoTransfere.Add(new ModelDepositoTransfere
            {
                codEstabel      = pBatchDepositoTransfere.CodEstabel,
                itCodigo        = pBatchDepositoTransfere.ItCodigo,
                codDeposSaida   = pBatchDepositoTransfere.CodDeposEntrada,
                codLocaliz      = pBatchDepositoTransfere.CodLocaliz,
                codRefer        = pBatchDepositoTransfere.CodRefer,
                codLote         = pBatchDepositoTransfere.CodLote,
                dtValiLote      = pBatchDepositoTransfere.DtValiLote,
                codDeposEntrada = pBatchDepositoTransfere.CodDeposSaida,
                qtidadeTransf   = pBatchDepositoTransfere.QtidadeTransf,
                codUsuario      = pBatchDepositoTransfere.CodUsuario
            });

            var result = Controller.MovtoEstoqMobile(true, lstDepositoTransfere);

            if (!result)
                return;

            // Elimina movimento de integração
            //BatchDepositoTransfereDB batchDepositoTransfereDB = new BatchDepositoTransfereDB();

            BatchDepositoTransfereDB.DeletarBatchDepositoTransfere(pBatchDepositoTransfere.IntTransferenciaId);
                      
        }

        public static void CancelaInventarioMobile(int pInventarioId)
        {
            try
            {
                //InventarioDB invDb = new InventarioDB();

                var inventario = InventarioDB.EfetivaInventarioMobile(pInventarioId, eStatusInventario.IniciadoMobile);

                var batchInventario = Mapper.Map<InventarioVO, BatchInventarioVO>(inventario);

                BatchInventarioDB.DeletarBatchInventario(batchInventario);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static Tuple<TipoIntegracao,string>  EfetivaInventarioSistemaOnline(InventarioVO pInventarioVO)
        {
            List<ModelInventario> lstModelInventario = new List<ModelInventario>();

            /* Integracao Online */
            var lstInventarioItemAsync = InventarioItemDB.GetInventarioItemByInventario(pInventarioVO.InventarioId);

            var lstInventarioItem = lstInventarioItemAsync.FindAll(p => p.QtdDigitada);

            for (int iItem = 0; iItem < lstInventarioItem.Count; iItem++)
                {
                    lstModelInventario.Add(new ModelInventario
                    {
                        dtSaldo          = pInventarioVO.DtInventario.ToString("dd/MM/yyyy"),
                        itCodigo         = lstInventarioItem[iItem].ItCodigo,
                        codEstabel       = pInventarioVO.CodEstabel,
                        codDepos         = pInventarioVO.CodDepos,
                        codLocaliz       = lstInventarioItem[iItem].CodLocaliz,
                        lote             = lstInventarioItem[iItem].CodLote,
                        dtUltSaida       = "",
                        dtUltEntr        = lstInventarioItem[iItem].DtUltEntr.HasValue ? lstInventarioItem[iItem].DtUltEntr.Value.ToString("dd/MM/yyyy") : String.Empty,
                        situacao         = "",
                        nrFicha          = lstInventarioItem[iItem].NrFicha,
                        valApurado       = lstInventarioItem[iItem].ValApurado,
                        codRefer         = lstInventarioItem[iItem].CodRefer,
                        NumContagem      = pInventarioVO.Contagem

                    });
                }
            

            var result = Controller.SetInventario(lstModelInventario);

            if (result.Item1 == TipoIntegracao.IntegracaoOnline)
            {
                BatchInventarioDB.AtualizaStatusIntegracao(pInventarioVO.InventarioId, eStatusIntegracao.EnviadoIntegracao);
            }

            return result;
        }

        public static Tuple<TipoIntegracao,string> EfetivaInventarioMobile(int pInventarioId)
        {
            try
            {

                if (InventarioItemDB.GetInventarioItemDigitadoByInventarioId(pInventarioId).Count <= 0)
                    throw new Exception("Inventário não pode ser efetivado, não foi informado nenhuma quantidade para os itens relacionados ao inventário");


                var inventario = InventarioDB.EfetivaInventarioMobile(pInventarioId, eStatusInventario.EncerradoMobile);
       
                var batchInventario = Mapper.Map<InventarioVO, BatchInventarioVO>(inventario);

                batchInventario.DtEfetivacao     = DateTime.Now;
                batchInventario.DtIntegracao     = DateTime.Now;
                batchInventario.StatusIntegracao = eStatusIntegracao.PendenteIntegracao;

                var blnOk = BatchInventarioDB.InserirBatchInventario(batchInventario);

                if (CrossConnectivity.Current.IsConnected)
                {
                    return IntegracaoOnlineBatch.EfetivaInventarioSistemaOnline(inventario);
                }
                else
                {
                    return Tuple.Create<TipoIntegracao, string>(TipoIntegracao.IntegracaoBatch, "Inventário efetivado, mas não integrado com o sistema. Dispositivo não conectado a internet.");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async static Task<Tuple<TipoIntegracao, string>> AtendeDevolveRequisicao(int pNrRequisicao, bool lDevolucao)
        {
            try
            {
                var lstRequisicaoItemSaldoEstoq = await RequisicaoItemSaldoEstoqDB.GetRequisicaoItemSaldoEstoq(pNrRequisicao, lDevolucao);

                Tuple<TipoIntegracao, string> tplRet;

                if (lDevolucao)
                {
                    //var lstRequisicaoDev;


                    lstRequisicaoItemSaldoEstoq.RemoveAll(p => p.QtdDevolver <= 0);

                    tplRet = Controller.SetDevolucao(lstRequisicaoItemSaldoEstoq);
                    /*
                    foreach (var row in lstRequisicaoItemSaldoEstoq)
                    {

                    } */
                        
                }
                else
                {


                    tplRet = Controller.SetRequisicao(lstRequisicaoItemSaldoEstoq);
                }

                if (tplRet.Item1 == TipoIntegracao.IntegracaoOnline)
                {
                    var lstReqOk = await RequisicaoItemSaldoEstoqDB.GetRequisicaoItemSaldoEstoq(pNrRequisicao, lDevolucao);
                    for (int i = 0; i < lstReqOk.Count; i++)
                    {
                        if (lDevolucao)
                        {
                            // Se for devolucao e a quantidade a atender é 0
                            // Quer dizer que nao existe quantidade a atender e pode eliminar
                            if (lstReqOk[i].QtdAtender == 0)
                            {
                                await RequisicaoItemSaldoEstoqDB.DeletarRequisicaoItemSaldoEstoq(lstReqOk[i]);
                            }
                            else
                            {
                                // Atualiza quantidade a devolver para 0
                                lstReqOk[i].QtdDevolver = 0;
                                await RequisicaoItemSaldoEstoqDB.AtualizaRequisicaoItemSaldoEstoq(lstReqOk[i]);
                            }
                        }
                        else
                        {
                            // Se for atendimento e a quantidade a devolver é 0
                            // Quer dizer que nao existe quantidade a devolver e pode eliminar
                            if (lstReqOk[i].QtdDevolver == 0)
                            {
                                await RequisicaoItemSaldoEstoqDB.DeletarRequisicaoItemSaldoEstoq(lstReqOk[i]);
                            }
                            else
                            {
                                // Atualiza quantidade a atender para 0
                                lstReqOk[i].QtdAtender = 0;
                                await RequisicaoItemSaldoEstoqDB.AtualizaRequisicaoItemSaldoEstoq(lstReqOk[i]);
                            }
                        }
                    } 
                }
                 
                return tplRet; 


            }
            catch (Exception ex)
            {
                throw ex;
            } 


        }
         
    }

    public enum TipoIntegracao
    {
        IntegracaoOnline,
        IntegracaoOnlineErro,
        IntegracaoBatch
    }
}
