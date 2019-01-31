using System;
using System.Collections.Generic;
using System.Text;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class InventarioEstoqueDAO : BaseCadastroDAO<InventarioEstoque, InventarioEstoqueDAO>
    {
        //private InventarioEstoqueDAO() { }
 
        private static readonly object _confirmarLock = new object();
        private static readonly object _finalizarLock = new object();
        private static readonly object _iniciarLock = new object();

        private string Sql(uint idLoja, uint idGrupoProd, uint idSubgrupoProd, InventarioEstoque.SituacaoEnum? situacao, 
            bool selecionar, out string filtroAdicional, out bool temFiltro)
        {
            temFiltro = false;
            StringBuilder sql = new StringBuilder("select ");

            sql.Append(selecionar ? "ie.*" : "count(*)");

            sql.AppendFormat(@"
                from inventario_estoque ie
                where 1 {0}", FILTRO_ADICIONAL);

            StringBuilder fa = new StringBuilder();

            if (idLoja > 0)
                fa.AppendFormat(" and ie.idLoja={0}", idLoja);

            if (idGrupoProd > 0)
                fa.AppendFormat(" and ie.idGrupoProd={0}", idGrupoProd);

            if (idSubgrupoProd > 0)
                fa.AppendFormat(" and ie.idSubgrupoProd={0}", idSubgrupoProd);

            if (situacao != null)
                fa.AppendFormat(" and ie.situacao={0}", (int)situacao);

            filtroAdicional = fa.ToString();
            return sql.ToString();
        }

        public IList<InventarioEstoque> ObtemItens(uint idLoja, uint idGrupoProd, uint idSubgrupoProd, InventarioEstoque.SituacaoEnum? situacao, 
            string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional, sql = Sql(idLoja, idGrupoProd, idSubgrupoProd, situacao, true, out filtroAdicional, out temFiltro);

            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "ie.idInventarioEstoque desc";

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional);
        }

        public int ObtemNumeroRegistros(uint idLoja, uint idGrupoProd, uint idSubgrupoProd, InventarioEstoque.SituacaoEnum? situacao)
        {
            bool temFiltro;
            string filtroAdicional, sql = Sql(idLoja, idGrupoProd, idSubgrupoProd, situacao, true, out filtroAdicional, out temFiltro);

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional);
        }

        public void AlterarSituacao(GDASession sessao, uint idInventarioEstoque, InventarioEstoque.SituacaoEnum situacao)
        {
            objPersistence.ExecuteCommand(sessao, "update inventario_estoque set situacao=" + 
                (int)situacao + " where idInventarioEstoque=" + idInventarioEstoque);
        }

        public void Finalizar(uint idInventarioEstoque)
        {
            lock (_finalizarLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        if (ObtemValorCampo<InventarioEstoque.SituacaoEnum>(transaction, "Situacao", "IdInventarioEstoque=" + idInventarioEstoque) == InventarioEstoque.SituacaoEnum.Finalizado)
                            throw new Exception("Este inventário já foi finalizado.");

                        AlterarSituacao(transaction, idInventarioEstoque, InventarioEstoque.SituacaoEnum.Finalizado);

                        objPersistence.ExecuteCommand(transaction, "update inventario_estoque set idFuncFin=" +
                            UserInfo.GetUserInfo.CodUser + ", dataFin=?data where idInventarioEstoque=" + idInventarioEstoque,
                            new GDAParameter("?data", DateTime.Now));

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

        public void Confirmar(uint idInventarioEstoque)
        {
            lock (_confirmarLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var inventario = GetElementByPrimaryKey(transaction, idInventarioEstoque);

                        if (inventario.Situacao == InventarioEstoque.SituacaoEnum.Confirmado)
                            throw new Exception("Este inventário já foi confirmado.");

                        var produtosInventario = ProdutoInventarioEstoqueDAO.Instance.ObtemPorInventarioEstoque(transaction, idInventarioEstoque)
                            .Where(f => f.QtdeFim != null);

                        MovEstoqueDAO.Instance.CreditaEstoqueInventario(
                            transaction,
                            inventario.IdLoja,
                            produtosInventario
                                .Where(f => f.QtdeFim > f.QtdeIni));

                        MovEstoqueDAO.Instance.BaixaEstoqueInventario(
                            transaction,
                            inventario.IdLoja,
                            produtosInventario
                                .Where(f => f.QtdeFim < f.QtdeIni));

                        AlterarSituacao(transaction, idInventarioEstoque, InventarioEstoque.SituacaoEnum.Confirmado);

                        objPersistence.ExecuteCommand(transaction, "update inventario_estoque set idFuncConf=" +
                            UserInfo.GetUserInfo.CodUser + ", dataConf=?data where idInventarioEstoque=" + idInventarioEstoque,
                            new GDAParameter("?data", DateTime.Now));

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

        public void Iniciar(uint idInventarioEstoque)
        {
            lock (_iniciarLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        if (ObtemValorCampo<InventarioEstoque.SituacaoEnum>(transaction, "Situacao", "IdInventarioEstoque=" + idInventarioEstoque) == InventarioEstoque.SituacaoEnum.EmContagem)
                            throw new Exception("Este inventário já está em contagem.");

                        AlterarSituacao(transaction, idInventarioEstoque, InventarioEstoque.SituacaoEnum.EmContagem);

                        InserirProdutos(transaction, idInventarioEstoque);

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

        public void InserirProdutos(GDASession session, uint idInventarioEstoque)
        {
            var idLoja = ObtemIdLojaPeloIdInventario(session, idInventarioEstoque);

            // Insere os produtos na tabela produto_loja caso ainda não exista associação.
            objPersistence.ExecuteCommand(session, @"Insert Into produto_loja (idLoja,idProd,qtdEstoque,estMinimo,reserva,m2,estoqueFiscal, liberacao)
                    (Select " + idLoja + @" As idLoja, idProd, 0 As qtdEstoque, 0 As estMinimo, 0 As reserva, 0 As m2, 0 As EstoqueFiscal, 0 As Liberacao From produto 
                    Where idProd Not In (Select idProd From produto_loja Where idLoja="+ idLoja + "));");

            // Insere os produtos na tabela
            objPersistence.ExecuteCommand(session, @"insert into produto_inventario_estoque (idInventarioEstoque, idProd, qtdeIni, m2Ini)
                select ie.idInventarioEstoque, pl.idProd, pl.qtdEstoque, pl.m2
                from produto_loja pl
                    inner join inventario_estoque ie on (pl.idLoja=ie.idLoja)
                    inner join produto p on (pl.idProd=p.idProd and p.idGrupoProd=ie.idGrupoProd
                        and coalesce(ie.idSubgrupoProd, p.idSubgrupoProd, 0) = coalesce(p.idSubgrupoProd, 0))
                where ie.idInventarioEstoque=" + idInventarioEstoque + " and ie.situacao=" + (int)InventarioEstoque.SituacaoEnum.EmContagem + @"
                    and p.situacao=" + (int)Situacao.Ativo);
        }

        #region Obtem Valor dos Campos

        public uint ObtemIdLojaPeloIdInventario(GDASession session, uint idInventarioEstoque)
        {
            return ObtemValorCampo<uint>(session, "IdLoja", "IdInventarioEstoque=" + idInventarioEstoque);
        }

        #endregion
    }
}
