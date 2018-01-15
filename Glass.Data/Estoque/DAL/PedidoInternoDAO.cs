using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class PedidoInternoDAO : BaseDAO<PedidoInterno, PedidoInternoDAO>
    {
        //private PedidoInternoDAO() { }

        #region Busca padrão

        private string Sql(uint idPedidoInterno, uint idFunc, uint idFuncAut, string dataIni, string dataFim, string situacao, bool selecionar)
        {
            return Sql(null, idPedidoInterno, idFunc, idFuncAut, dataIni, dataFim, situacao, selecionar);
        }

        private string Sql(GDASession session, uint idPedidoInterno, uint idFunc, uint idFuncAut, string dataIni, string dataFim, string situacao, bool selecionar)
        {
            string criterio = "";
            string campos = selecionar ? "pi.*, fAut.nome as nomeFuncAut, fCad.nome as nomeFuncCad, fConf.nome as nomeFuncConf, " +
                "coalesce(l.nomeFantasia, l.razaoSocial) as nomeLoja, '$$$' as criterio" : "count(*)";

            string sql = @"
                select " + campos + @"
                from pedido_interno pi
                    left join funcionario fCad on (pi.idFuncCad=fCad.idFunc)
                    left join funcionario fConf on (pi.usuConf=fConf.idFunc)
                    left join funcionario fAut on (pi.idFuncAut=fAut.idFunc)
                    left join loja l on (pi.idLoja=l.idLoja)
                where 1";

            if (idPedidoInterno > 0)
            {
                sql += " and pi.idPedidoInterno=" + idPedidoInterno;
                criterio += "Pedido Interno: " + idPedidoInterno + "    ";
            }

            if (idFunc > 0)
            {
                sql += " and pi.idFuncCad=" + idFunc;
                criterio += "Funcionário: " + FuncionarioDAO.Instance.GetNome(session, idFunc) + "    ";
            }

            if (idFuncAut > 0)
            {
                sql += " and pi.idFuncAut=" + idFuncAut;
                criterio += "Funcionário Autorizador: " + FuncionarioDAO.Instance.GetNome(session, idFuncAut) + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " and pi.dataPedido>=?dataIni";
                criterio += "Data Inicial: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " and pi.dataPedido<=?dataFim";
                criterio += "Data Inicial: " + dataIni + "    ";
            }

            if (!String.IsNullOrEmpty(situacao))
            {
                sql += " and pi.situacao in (" + situacao + ")";

                PedidoInterno temp = new PedidoInterno();

                criterio += "Situação: ";
                foreach (int s in Array.ConvertAll(situacao.Split(','), x => Glass.Conversoes.StrParaInt(x)))
                {
                    if (s == 0) continue;
                    temp.Situacao = s;
                    criterio += temp.DescrSituacao + ", ";
                }

                criterio = criterio.TrimEnd(' ', ',') + "    ";
            }

            sql += " order by pi.idPedidoInterno desc";
            sql = sql.Replace("$$$", criterio);
            return sql;
        }

        public IList<PedidoInterno> GetForRpt(uint idPedidoInterno, uint idFunc, uint idFuncAut, string dataIni, string dataFim)
        {
            return objPersistence.LoadData(Sql(idPedidoInterno, idFunc, idFuncAut, dataIni, dataFim, null, true), GetParam(dataIni, dataFim)).ToList();
        }

        public IList<PedidoInterno> GetList(uint idPedidoInterno, uint idFunc, uint idFuncAut, string dataIni, string dataFim, 
            string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idPedidoInterno, idFunc, idFuncAut, dataIni, dataFim, null, true), sortExpression, startRow, 
                pageSize, GetParam(dataIni, dataFim));
        }

        public int GetCount(uint idPedidoInterno, uint idFunc, uint idFuncAut, string dataIni, string dataFim)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idPedidoInterno, idFunc, idFuncAut, dataIni, dataFim, null, false), 
                GetParam(dataIni, dataFim));
        }

        public PedidoInterno GetElement(uint idPedidoInterno)
        {
            return GetElement(null, idPedidoInterno);
        }

        public PedidoInterno GetElement(GDASession session, uint idPedidoInterno)
        {
            return objPersistence.LoadOneData(session, Sql(session, idPedidoInterno, 0, 0, null, null, null, true));
        }

        private GDAParameter[] GetParam(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Finalizar pedido

        /// <summary>
        /// Finaliza o pedido.
        /// </summary>
        /// <param name="idPedidoInterno"></param>
        public void Finalizar(uint idPedidoInterno)
        {
            var situacao = ObtemValorCampo<int>("situacao", "idPedidoInterno=" + idPedidoInterno);

            if (ProdutoPedidoInternoDAO.Instance.GetCountReal(idPedidoInterno) == 0)
                throw new Exception("Cadastre pelo menos um produto para finalizar o pedido.");

            // Caso o pedido interno esteja confirmado ou autorizado então não pode ser finalizado.
            if (situacao == (int)PedidoInterno.SituacaoPedidoInt.Autorizado ||
                situacao == (int)PedidoInterno.SituacaoPedidoInt.ConfirmadoParcialmente ||
                situacao == (int)PedidoInterno.SituacaoPedidoInt.Confirmado)
                throw new Exception("Falha ao finalizar o pedido interno. O pedido está confirmado/autorizado.");

            objPersistence.ExecuteCommand("update pedido_interno set situacao=" + (int)PedidoInterno.SituacaoPedidoInt.Finalizado +
                " where idPedidoInterno=" + idPedidoInterno);

            if (Config.PossuiPermissao(Config.FuncaoMenuEstoque.AutorizarPedidoInterno))
            {
                var centroCusto = ObtemIdCentroCusto((int)idPedidoInterno).GetValueOrDefault(0);

                if (FiscalConfig.UsarControleCentroCusto && CentroCustoDAO.Instance.GetCountReal() > 0 && centroCusto == 0)
                    return;

                PedidoInternoDAO.Instance.AutorizarPedidoInterno(idPedidoInterno, centroCusto);
            }
        }

        /// <summary>
        /// Reabre um pedido finalizado.
        /// </summary>
        /// <param name="idPedidoInterno"></param>
        public void Reabrir(uint idPedidoInterno)
        {
            var situacao = ObtemValorCampo<int>("situacao", "idPedidoInterno=" + idPedidoInterno);

            // Caso o pedido interno esteja confirmado então não pode ser finalizado.
            if (situacao == (int)PedidoInterno.SituacaoPedidoInt.ConfirmadoParcialmente ||
                situacao == (int)PedidoInterno.SituacaoPedidoInt.Confirmado)
                throw new Exception("Falha ao finalizar o pedido interno. O pedido está confirmado/autorizado.");

            objPersistence.ExecuteCommand("update pedido_interno set situacao=" + (int)PedidoInterno.SituacaoPedidoInt.Aberto +
                ", idFuncAut = null, dataAut = null where idPedidoInterno=" + idPedidoInterno);

            #region Centro de Custo

            var idCentroCusto = ObtemIdCentroCusto((int)idPedidoInterno);
            if (idCentroCusto.GetValueOrDefault(0) > 0)
            {
                CentroCustoAssociadoDAO.Instance.DeleteByPedidoInterno((int)idPedidoInterno);
                MovEstoqueCentroCustoDAO.Instance.DeleteByPedidoInterno((int)idPedidoInterno);
            }

            #endregion
        }

        #endregion

        #region Cancelar pedido

        /// <summary>
        /// Cancela o pedido.
        /// </summary>
        /// <param name="idPedidoInterno"></param>
        public void Cancelar(uint idPedidoInterno)
        {
            PedidoInterno pedido = GetElement(idPedidoInterno);

            var produtos = ProdutoPedidoInternoDAO.Instance.GetByPedidoInterno(pedido.IdPedidoInterno);

            if (pedido.Situacao == (int)PedidoInterno.SituacaoPedidoInt.Confirmado)
            {
                foreach(ProdutoPedidoInterno p in produtos)
                {
                    float estoque = ProdutoLojaDAO.Instance.GetEstoque(null, pedido.IdLoja, p.IdProd, null, false, false, false);

                    ProdutoPedidoInternoDAO.Instance.ExtornaProdutosPedidoInterno(pedido.IdLoja, p.IdProd, estoque + p.QtdeConfirmada); 
                }
            }

            objPersistence.ExecuteCommand("update pedido_interno set situacao=" + (int)PedidoInterno.SituacaoPedidoInt.Cancelado +
                " where idPedidoInterno=" + idPedidoInterno);

            #region Centro de Custo

            var idCentroCusto = ObtemIdCentroCusto((int)idPedidoInterno);
            if (idCentroCusto.GetValueOrDefault(0) > 0)
            {
                CentroCustoAssociadoDAO.Instance.DeleteByPedidoInterno((int)idPedidoInterno);
                MovEstoqueCentroCustoDAO.Instance.DeleteByPedidoInterno((int)idPedidoInterno);
            }

            #endregion
        }

        #endregion

        #region Confirmar pedido

        /// <summary>
        /// Verifica se um pedido pode ser confirmado.
        /// </summary>
        public bool PodeConfirmar(GDASession session, uint idPedidoInterno)
        {
            return objPersistence.ExecuteSqlQueryCount(session, "select count(*) from pedido_interno where idPedidoInterno=" + idPedidoInterno +
                " and situacao in (" + (int)PedidoInterno.SituacaoPedidoInt.Autorizado + "," + (int)PedidoInterno.SituacaoPedidoInt.ConfirmadoParcialmente + ")") > 0;
        }
 
        public IList<PedidoInterno> ObtemParaConfirmacao()
        {
            string sql = Sql(0, 0, 0, null, null, (int)PedidoInterno.SituacaoPedidoInt.Autorizado + "," + (int)PedidoInterno.SituacaoPedidoInt.ConfirmadoParcialmente, true);
            return objPersistence.LoadData(sql).ToList();
        }

        private static object ConfirmarPedidoInternoLock = new object();

        /// <summary>
        /// Confirma um pedido finalizado.
        /// </summary>
        /// <param name="qtdeProdutos">As quantidades dos produtos que serão liberados, com código e quantidade.</param>
        public void Confirmar(uint idPedidoInterno, Dictionary<uint, float> qtdeProdutos)
        {
            lock(ConfirmarPedidoInternoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        if (!PodeConfirmar(transaction, idPedidoInterno))
                            throw new Exception("O pedido deve estar autorizado para ser confirmado.");

                        PedidoInterno pedido = GetElement(transaction, idPedidoInterno);

                        int prodConfirmados = 0;
                        var produtos = ProdutoPedidoInternoDAO.Instance.GetByPedidoInterno(transaction, idPedidoInterno);

                        // Baixa o estoque dos produtos
                        foreach (ProdutoPedidoInterno p in produtos)
                        {
                            // Só deixa confirmar até o máximo permitido para o produto
                            qtdeProdutos[p.IdProdPedInterno] = Math.Min(qtdeProdutos[p.IdProdPedInterno], p.QtdeConfirmar);

                            if (qtdeProdutos[p.IdProdPedInterno] <= 0)
                                continue;

                            bool m2 = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(transaction, (int)p.IdGrupoProd, (int?)p.IdSubgrupoProd) == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 ||
                                Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(transaction, (int)p.IdGrupoProd, (int?)p.IdSubgrupoProd) == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;

                            prodConfirmados++;
                            float qtdeBaixa = p.ConfirmarQtde ? qtdeProdutos[p.IdProdPedInterno] : 1;
                            float m2Baixa = p.ConfirmarQtde ? 0 : qtdeProdutos[p.IdProdPedInterno];

                            MovEstoqueDAO.Instance.BaixaEstoquePedidoInterno(transaction, p.IdProd, pedido.IdLoja, idPedidoInterno, p.IdProdPedInterno, (decimal)(m2 ? m2Baixa : qtdeBaixa));

                            // Atualiza no produto a quantidade confirmada
                            objPersistence.ExecuteCommand(transaction, "update produto_pedido_interno set qtdeConfirmada=coalesce(qtdeConfirmada,0)+?qtde where idProdPedInterno=" + p.IdProdPedInterno,
                                new GDAParameter("?qtde", qtdeProdutos[p.IdProdPedInterno]));

                            #region Centro de Custo

                            var idCentroCusto = ObtemIdCentroCusto(transaction, (int)idPedidoInterno);

                            if (FiscalConfig.UsarControleCentroCusto && CentroCustoDAO.Instance.GetCountReal(transaction) > 0 && idCentroCusto.GetValueOrDefault(0) > 0 && !m2)
                            {
                                var idLoja = ObtemIdLoja(transaction, (int)idPedidoInterno);

                                CentroCustoAssociadoDAO.Instance.Insert(transaction, new CentroCustoAssociado()
                                {
                                    IdCentroCusto = idCentroCusto.Value,
                                    IdPedidoInterno = (int)idPedidoInterno,
                                    IdConta = MovEstoqueCentroCustoDAO.Instance.ObtemUltimoIdConta(transaction, idLoja, (int)p.IdProd).GetValueOrDefault(),
                                    Valor = (decimal)qtdeBaixa * MovEstoqueCentroCustoDAO.Instance.ObtemValorUnitarioProd(transaction, idLoja, (int)p.IdProd)
                                });

                                MovEstoqueCentroCustoDAO.Instance.BaixaEstoquePedidoInterno(transaction, (int)idPedidoInterno, (int)p.IdProd, idLoja, (decimal)qtdeBaixa);
                            }

                            #endregion
                        }

                        // Verifica se houve algum produto confirmado
                        if (prodConfirmados == 0)
                            throw new Exception("Selecione a quantidade a ser liberada de algum produto.");

                        // Altera a situação do pedido interno
                        int situacao = ExecuteScalar<bool>(transaction, "select sum(qtde>coalesce(qtdeConfirmada,0))=0 from produto_pedido_interno where idPedidoInterno=" + idPedidoInterno) ?
                            (int)PedidoInterno.SituacaoPedidoInt.Confirmado : (int)PedidoInterno.SituacaoPedidoInt.ConfirmadoParcialmente;

                        objPersistence.ExecuteCommand(transaction, "update pedido_interno set situacao=" + situacao + ", " +
                            "dataConf=now(), usuConf=" + UserInfo.GetUserInfo.CodUser + " where idPedidoInterno=" + idPedidoInterno);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw;
                    }
                }
            }
        }

        #endregion

        #region Autorizar pedido

        public IList<PedidoInterno> ObtemParaAutorizacao()
        {
            string sql = Sql(0, 0, 0, null, null, (int)PedidoInterno.SituacaoPedidoInt.Finalizado + "", true);
            return objPersistence.LoadData(sql).ToList();
        }

        public void AutorizarPedidoInterno(uint idPedidoInterno, int idCentroCusto)
        {
            var idCentroCustoOriginal = ObtemIdCentroCusto((int)idPedidoInterno).GetValueOrDefault(0);

            if (FiscalConfig.UsarControleCentroCusto && CentroCustoDAO.Instance.GetCountReal() > 0 && idCentroCustoOriginal == 0 && idCentroCusto == 0)
                throw new Exception("Nenhum centro de custo foi informado para o pedido interno: " + idPedidoInterno);

            if (idCentroCusto > 0 && idCentroCusto != idCentroCustoOriginal)
                AtualizarCentroCusto((int)idPedidoInterno, idCentroCusto);

            objPersistence.ExecuteCommand(@"update pedido_interno set idFuncAut=?func, dataAut=?data, 
                situacao=?sit where idPedidoInterno=" + idPedidoInterno,
                new GDAParameter("?func", UserInfo.GetUserInfo.CodUser),
                new GDAParameter("?data", DateTime.Now),
                new GDAParameter("?sit", (int)PedidoInterno.SituacaoPedidoInt.Autorizado));
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(PedidoInterno objInsert)
        {
            objInsert.DataPedido = DateTime.Now;
            
            if (objInsert.IdFuncCad == 0)
                objInsert.IdFuncCad = UserInfo.GetUserInfo.CodUser;            

            var idPedido = base.Insert(objInsert);            

            return idPedido;
        }

        public override int Update(PedidoInterno objUpdate)
        {
            var situacao = ObtemValorCampo<int>("situacao", "idPedidoInterno=" + objUpdate.IdPedidoInterno);

            // Caso o pedido interno esteja confirmado ou autorizado então não pode ser atualizado.
            if (situacao == (int)PedidoInterno.SituacaoPedidoInt.Autorizado ||
                situacao == (int)PedidoInterno.SituacaoPedidoInt.ConfirmadoParcialmente ||
                situacao == (int)PedidoInterno.SituacaoPedidoInt.Confirmado)
                throw new Exception("Falha ao finalizar o pedido interno. O pedido está confirmado/autorizado.");

            objUpdate.DataPedido = DateTime.Now;

            if (objUpdate.IdFuncCad == 0)
                objUpdate.IdFuncCad = UserInfo.GetUserInfo.CodUser;

            return base.Update(objUpdate);
        }

        #endregion     

        #region Obtem dados do pedido inteno

        /// <summary>
        /// Recupera o loja do pedido interno
        /// </summary>
        public int ObtemIdLoja(GDASession session, int idPedidoInterno)
        {
            return ObtemValorCampo<int>(session, "idLoja", "idPedidoInterno = " + idPedidoInterno);
        }

        /// <summary>
        /// Obtem o id do centro de custo
        /// </summary>
        public int? ObtemIdCentroCusto(int idPedidoInterno)
        {
            return ObtemIdCentroCusto(null, idPedidoInterno);
        }

        /// <summary>
        /// Obtem o id do centro de custo
        /// </summary>
        public int? ObtemIdCentroCusto(GDASession session, int idPedidoInterno)
        {
            return ObtemValorCampo<int?>(session, "IdCentroCusto", "idPedidoInterno = " + idPedidoInterno);
        }

        public int? ObtemIdFuncAut(int idPedidoInterno)
        {
            return ObtemValorCampo<int?>("IdFuncAut", "idPedidoInterno = " + idPedidoInterno);
        }

        #endregion

        #region Atualiza dados do centro de custo

        /// <summary>
        /// Atualiza dados do centro de custo
        /// </summary>
        /// <param name="idPedidoInterno"></param>
        /// <param name="idCentroCusto"></param>
        private void AtualizarCentroCusto(int idPedidoInterno, int idCentroCusto)
        {
            objPersistence.ExecuteCommand("UPDATE pedido_interno set idCentroCusto = " + idCentroCusto + " WHERE idPedidoInterno = " + idPedidoInterno);
        }

        #endregion
    }
}
