using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.EFD;
using GDA;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class CentroCustoDAO : BaseDAO<CentroCusto, CentroCustoDAO>
    {
        //private CentroCustoDAO() { }

        #region Busca padrão

        private string Sql(uint idCentroCusto, uint idLoja, bool selecionar)
        {
            string campos = selecionar ? "cc.*, coalesce(l.nomeFantasia, l.razaoSocial) as nomeLoja" : "count(*)";
            string sql = "select " + campos + @"
                from centro_custo cc
                    left join loja l on (cc.idLoja=l.idLoja)
                where 1";

            if (idCentroCusto > 0)
                sql += " and cc.idCentroCusto=" + idCentroCusto;

            if (idLoja > 0)
                sql += " and cc.idLoja=" + idLoja;

            return sql;
        }

        public IList<CentroCusto> GetList(string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal() == 0)
                return new CentroCusto[] { new CentroCusto() };

            return LoadDataWithSortExpression(Sql(0, 0, true), sortExpression, startRow, pageSize);
        }

        public int GetCount()
        {
            int retorno = GetCountReal();
            return retorno > 0 ? retorno : 1;
        }

        public int GetCountReal()
        {
            return GetCountReal(null);
        }

        public int GetCountReal(GDASession session)
        {
            return objPersistence.ExecuteSqlQueryCount(session, Sql(0, 0, false));
        }

        #endregion

        #region Busca por loja

        public CentroCusto[] GetByLoja(uint idLoja)
        {
            List<CentroCusto> retorno = objPersistence.LoadData(Sql(0, idLoja, true));
            if (retorno.Count == 0)
                foreach (GenericModel g in DataSourcesEFD.Instance.GetTipoCentroCusto(idLoja))
                {
                    CentroCusto c = new CentroCusto();
                    c.IdCentroCusto = (int)g.Id.Value;
                    c.Descricao = g.Descr;
                    retorno.Add(c);
                }

            return retorno.ToArray();
        }

        #endregion

        #region Recupera os centros de custos cadastrados

        public IList<CentroCusto> GetSorted()
        {
            string sql = "select * from centro_custo order by descricao";
            return objPersistence.LoadData(sql).ToList();
        }

        public IList<CentroCusto> ObtemParaSelecao(int idLoja, bool buscarEstoque)
        {
            string sql = @"
                SELECT * 
                FROM centro_custo 
                WHERE 1";

            if (idLoja > 0)
                sql += " AND idLoja = " + idLoja;

            if (!buscarEstoque)
                sql += " AND COALESCE(tipo, 0) <> " + (int)TipoCentroCusto.Estoque;

            sql += " ORDER BY descricao";

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Obtem a descrição de um centro de custo
        /// </summary>
        /// <param name="idCentroCusto"></param>
        /// <returns></returns>
        public string ObtemDescricao(int idCentroCusto)
        {
            return ObtemValorCampo<string>("Descricao", "IdCentroCusto = " + idCentroCusto);
        }

        #endregion

        #region Busca para EFD

        /// <summary>
        /// Busca para EFD.
        /// </summary>
        /// <returns></returns>
        public CentroCusto[] GetForEFD(DateTime inicio)
        {
            List<CentroCusto> retorno = new List<CentroCusto>(GetAll());
            
            List<string> idLoja = new List<string>();
            foreach (CentroCusto c in retorno)
                if (!idLoja.Contains(c.IdLoja.ToString()))
                    idLoja.Add(c.IdLoja.ToString());

            string idsLojas = String.Join(",", idLoja.ToArray());
            if (idsLojas == String.Empty)
                idsLojas = "0";

            idsLojas = ExecuteScalar<string>("select coalesce(cast(group_concat(idLoja) as char), '') from loja where idLoja not in (" + idsLojas + ")");

            if (!String.IsNullOrEmpty(idsLojas))
            {
                foreach (string l in idsLojas.Split(','))
                {
                    uint id;
                    if (!uint.TryParse(l, out id))
                        continue;

                    CentroCusto[] itens = GetByLoja(id);

                    foreach (CentroCusto c in itens)
                        if (retorno.Find(x => x.IdCentroCusto == c.IdCentroCusto) == null)
                        {
                            c.ForEfd = true;
                            c.DataCad = inicio;
                            retorno.Add(c);
                        }
                }
            }

            return retorno.ToArray();
        }

        #endregion

        #region Busca para Relatorio

        private string SqlForRelatorioPorMes(int idLoja, int ano)
        {
            string sql = @"
            SELECT cc.*, tmp.Total, tmp.Mes
            FROM centro_custo cc
	            LEFT JOIN
                (
    	            SELECT cca.IdCentroCusto, sum(cca.Valor) as Total, MONTH(COALESCE(c.DataFinalizada, i.DataCad, nf.DataEmissao, pg.dataCad, pi.DataPedido)) as Mes
                    FROM centro_custo_associado cca
        	            LEFT JOIN compra c ON (cca.IdCompra = c.IdCompra)
                        LEFT JOIN imposto_serv i ON (cca.IdImpostoServ = i.IdImpostoServ)
                        LEFT JOIN nota_fiscal nf ON (cca.IdNf = nf.IdNf)
                        LEFT JOIN contas_pagar pg ON (cca.IdContaPg = pg.IdContaPg)
                        LEFT JOIN pedido_interno pi ON (cca.IdPedidoInterno = pi.IdPedidoInterno)
                    WHERE (c.Situacao IN (" + (int)Compra.SituacaoEnum.Finalizada + "," + (int)Compra.SituacaoEnum.AguardandoEntrega + @")
                        OR i.Situacao =" + (int)ImpostoServ.SituacaoEnum.Finalizado + @"
                        OR nf.Situacao = "+(int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros+ @"
                        OR pi.Situacao NOT IN (" + (int)PedidoInterno.SituacaoPedidoInt.Aberto + "," + (int)PedidoInterno.SituacaoPedidoInt.Cancelado + @")
                        OR pg.IdCustoFixo is not null)
                        AND COALESCE(c.IdLoja, i.IdLoja, nf.idLoja, pg.IdLoja, pi.IdLoja) = " + idLoja + @"
                        {0}
                    GROUP BY cca.IdCentroCusto, MONTH(COALESCE(c.DataFinalizada, i.DataCad, nf.DataEmissao, pg.dataCad, pi.DataPedido))
                ) as tmp ON (cc.IdCentroCusto = tmp.IdCentroCusto)
            WHERE COALESCE(cc.Tipo, 0) <>" + (int)TipoCentroCusto.Estoque;

            sql = string.Format(sql, " AND YEAR(COALESCE(c.DataFinalizada, i.DataCad, nf.DataEmissao, pg.dataCad, pi.DataPedido)) = " + ano);

            return sql;
        }

        private string SqlForRelatorioPorPlanoConta(int idLoja, string dataInicial, string dataFinal)
        {
            string sql = @"
            SELECT cc.*, tmp.Total, tmp.IdConta as IdPlanoConta, tmp.Descricao as DescrPlanoConta
            FROM centro_custo cc
	            LEFT JOIN
                (
    	            SELECT cca.IdCentroCusto, sum(cca.Valor) as Total, pc.IdConta, pc.Descricao
                    FROM centro_custo_associado cca
        	            LEFT JOIN compra c ON (cca.IdCompra = c.IdCompra)
                        LEFT JOIN imposto_serv i ON (cca.IdImpostoServ = i.IdImpostoServ)
                        LEFT JOIN nota_fiscal nf ON (cca.IdNf = nf.IdNf)
                        LEFT JOIN contas_pagar pg ON (cca.IdContaPg = pg.IdContaPg)
                        LEFT JOIN pedido_interno pi ON (cca.IdPedidoInterno = pi.IdPedidoInterno)
                        INNER JOIN plano_contas pc ON (cca.IdConta = pc.IdConta)
                    WHERE (c.Situacao IN (" + (int)Compra.SituacaoEnum.Finalizada + "," + (int)Compra.SituacaoEnum.AguardandoEntrega + @")
                        OR i.Situacao = " + (int)ImpostoServ.SituacaoEnum.Finalizado + @"
                        OR nf.Situacao = " + (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros + @"
                        OR pi.Situacao NOT IN (" + (int)PedidoInterno.SituacaoPedidoInt.Aberto + "," + (int)PedidoInterno.SituacaoPedidoInt.Cancelado + @")
                        OR pg.IdCustoFixo is not null)
                        AND COALESCE(c.DataFinalizada, i.DataCad, nf.DataEmissao, pg.DataCad, pi.DataPedido) >= ?dtIni 
                        AND COALESCE(c.DataFinalizada, i.DataCad, nf.DataEmissao, pg.DataCad, pi.DataPedido) <= ?dtFim
                        AND COALESCE(c.IdLoja, i.IdLoja, nf.idLoja, pg.IdLoja, pi.IdLoja) = " + idLoja + @"
                    GROUP BY cca.IdCentroCusto, pc.IdConta
                ) as tmp ON (cc.IdCentroCusto = tmp.IdCentroCusto)
            WHERE COALESCE(cc.Tipo, 0) <>" + (int)TipoCentroCusto.Estoque;

            return sql;
        }

        /// <summary>
        /// Recupera os centro de custos para o relatorio por mês
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="ano"></param>
        /// <returns></returns>
        public IList<CentroCusto> GetForRelCentroCustoMes(int idLoja, int ano)
        {
            var aux = objPersistence.LoadData(SqlForRelatorioPorMes(idLoja, ano)).ToList();

            var retorno = new List<CentroCusto>();

            foreach (var item in aux.GroupBy(f => f.IdCentroCusto))
            {
                for (int i = 1; i <= 12; i++)
                {
                    var centroCusto = aux.Where(f => f.IdCentroCusto == item.Key && f.Mes == i).FirstOrDefault();

                    if (centroCusto == null)
                    {
                        retorno.Add(new CentroCusto()
                        {
                            IdCentroCusto = item.Key,
                            Mes = i,
                            Descricao = item.Select(x => x.Descricao).FirstOrDefault(),
                            Total = 0
                        });
                    }
                    else
                    {
                        retorno.Add(centroCusto);
                    }
                }
            }

            return retorno;
        }

        /// <summary>
        /// Recupera os centro de custos para o relatorio por plano de contas
        /// </summary>
        /// <param name="idLoja"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <returns></returns>
        public IList<CentroCusto> GetForRelCentroCustoPlanoConta(int idLoja, string dataInicial, string dataFinal)
        {
            var aux = objPersistence.LoadData(SqlForRelatorioPorPlanoConta(idLoja, dataInicial, dataFinal), GetParams(dataInicial, dataFinal)).ToList();

            var retorno = new List<CentroCusto>();

            foreach (var item in aux.GroupBy(f => f.IdCentroCusto))
            {
                foreach (var planoConta in aux.GroupBy(f => f.IdPlanoConta))
                {
                    var centroCusto = aux.Where(f => f.IdCentroCusto == item.Key && f.IdPlanoConta == planoConta.Key).FirstOrDefault();

                    if (centroCusto == null)
                    {
                        retorno.Add(new CentroCusto()
                        {
                            IdCentroCusto = item.Key,
                            Mes = planoConta.Key,
                            Descricao = item.Select(x => x.Descricao).FirstOrDefault(),
                            DescrPlanoConta = planoConta.Select(x => x.DescrPlanoConta).FirstOrDefault(),
                            Total = 0
                        });
                    }
                    else
                    {
                        retorno.Add(centroCusto);
                    }
                }
            }

            return retorno;
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParams = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParams.Add(new GDAParameter("?dtIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParams.Add(new GDAParameter("?dtFim", DateTime.Parse(dataFim + " 23:59:59")));

            return lstParams.ToArray();
        }

        #endregion

        #region Recupera dados do centro de custo

        /// <summary>
        /// Obtem o centro de custo do tipo estoque
        /// </summary>
        public CentroCusto ObtemCentroCustoEstoque(int idLoja)
        {
            return ObtemCentroCustoEstoque(null, idLoja);
        }

        /// <summary>
        /// Obtem o centro de custo do tipo estoque
        /// </summary>
        public CentroCusto ObtemCentroCustoEstoque(GDASession session, int idLoja)
        {
            return objPersistence.LoadData(session, @"
                SELECT *
                FROM centro_custo
                WHERE tipo=?tipo
                AND IdLoja = ?idLoja", new GDAParameter("?tipo", TipoCentroCusto.Estoque), new GDAParameter("?idLoja", idLoja))
                                     .ToList().FirstOrDefault();
        }

        #endregion

        #region Métodos sobrescritos

        public override uint Insert(CentroCusto objInsert)
        {
            if (objInsert.Tipo == TipoCentroCusto.Estoque && ObtemCentroCustoEstoque(objInsert.IdLoja) != null)
                throw new Exception("Já existe um centro de custo estoque cadastrado para essa loja.");

            objInsert.DataCad = DateTime.Now;

            CentroCusto vazio = new CentroCusto();
            vazio.IdCentroCusto = (int)base.Insert(objInsert);
            LogAlteracaoDAO.Instance.LogCentroCusto(vazio, LogAlteracaoDAO.SequenciaObjeto.Atual);

            return (uint)vazio.IdCentroCusto;
        }

        public override int Update(CentroCusto objUpdate)
        {
            if (objUpdate.Tipo == TipoCentroCusto.Estoque)
            {
                var estoque = ObtemCentroCustoEstoque(objUpdate.IdLoja);

                if (estoque != null && estoque.IdCentroCusto != objUpdate.IdCentroCusto)
                    throw new Exception("Já existe um centro de custo estoque cadastrado para essa loja.");
            }

            LogAlteracaoDAO.Instance.LogCentroCusto(objUpdate, LogAlteracaoDAO.SequenciaObjeto.Novo);
            return base.Update(objUpdate);
        }

        public override int Delete(CentroCusto objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdCentroCusto);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            if (objPersistence.ExecuteSqlQueryCount(@"select count(*) from centro_custo") > 0)
                throw new Exception("Não é possível apagar esse centro de custo porque ele está em uso.");

            return base.DeleteByPrimaryKey(Key);
        }

        #endregion
    }
}
