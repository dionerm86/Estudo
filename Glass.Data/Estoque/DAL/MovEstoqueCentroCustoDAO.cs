using GDA;
using Glass.Data.Helper;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class MovEstoqueCentroCustoDAO : BaseDAO<MovEstoqueCentroCusto, MovEstoqueCentroCustoDAO>
    {
        #region Credita o estoque

        /// <summary>
        /// Credita o estoque do centro de custo ao associar o mesmo a compra
        /// </summary>
        public void CreditaEstoqueCompra(GDASession session, int idCompra)
        {
            DeleteByCompra(session, idCompra);

            var idLoja = CompraDAO.Instance.ObtemIdLoja(session, (uint)idCompra);
            var idConta = CompraDAO.Instance.ObtemIdConta(session, idCompra);
            var produtos = ProdutosCompraDAO.Instance.GetByCompra(session, (uint)idCompra);

            foreach (var p in produtos)
                MovimentaEstoque(session, (int)p.IdProd, MovEstoqueCentroCusto.TipoMovEnum.Entrada,
                    (int)idLoja, idCompra, null, (decimal)p.Qtde, p.Total, idConta);
        }

        #endregion

        #region Baixa o estoque

        /// <summary>
        /// Baixa o estoque do centro de custo ao fazer um pedido interno
        /// </summary>
        public void BaixaEstoquePedidoInterno(GDASession session, int idPedidoInterno, int idProd, int idLoja, decimal qtde)
        {
            MovimentaEstoque(session, idProd, MovEstoqueCentroCusto.TipoMovEnum.Saida, idLoja, null, idPedidoInterno, qtde, ObtemValorUnitarioProd(session, idLoja, idProd) * qtde, null);
        }

        #endregion

        #region Movimenta o estoque

        /// <summary>
        /// Faz a movimentação do estoque
        /// </summary>
        private void MovimentaEstoque(GDASession session, int idProd, MovEstoqueCentroCusto.TipoMovEnum tipoMov, int idLoja, int? idCompra, int? idPedidoInterno,
            decimal qtdeMov, decimal valorMov, int? idConta)
        {
            var mov = new MovEstoqueCentroCusto();

            mov.IdProd = idProd;
            mov.TipoMov = tipoMov;
            mov.IdLoja = idLoja;
            mov.IdCompra = idCompra;
            mov.IdPedidoInterno = idPedidoInterno;
            mov.IdConta = idConta != null ? idConta : ObtemUltimoIdConta(session, idLoja, idProd);
            mov.QtdeMov = qtdeMov;
            mov.ValorMov = valorMov;

            mov.SaldoQtdeMov = ObtemUltimoSaldoQtde(session, idLoja, idProd) + (tipoMov == MovEstoqueCentroCusto.TipoMovEnum.Entrada ? qtdeMov : -qtdeMov);
            mov.SaldoValorMov = (valorMov / qtdeMov) * mov.SaldoQtdeMov;

            mov.IdFunc = (int)UserInfo.GetUserInfo.CodUser;
            mov.DataMov = DateTime.Now;

            mov.IdMovEstoqueCentroCusto = (int)Insert(session, mov);
        }

        #endregion

        #region Obtem dados da movimentação

        /// <summary>
        /// Obtem o ultimo saldo de quantidade de um produto
        /// </summary>
        public decimal ObtemUltimoSaldoQtde(GDASession session, int idLoja, int idProd)
        {
            var sql = @"
                SELECT COALESCE(SaldoQtdeMov, 0)
                FROM mov_estoque_centro_custo
                WHERE IdLoja = " + idLoja + " AND IdProd = " + idProd+@"
                ORDER BY IdMovEstoqueCentroCusto Desc";

            return ExecuteScalar<decimal>(session, sql);
        }

        /// <summary>
        /// Obtem o valor unitario de um produto
        /// </summary>
        public decimal ObtemValorUnitarioProd(GDASession session, int idLoja, int idProd)
        {
            var sql = @"
                SELECT COALESCE(SaldoValorMov / SaldoQtdeMov, 0)
                FROM mov_estoque_centro_custo
                WHERE IdLoja = " + idLoja + " AND IdProd = " + idProd + @"
                ORDER BY IdMovEstoqueCentroCusto Desc";

            return ExecuteScalar<decimal>(session, sql);
        }

        /// <summary>
        /// Obtem o saldo do centro de custo estoque de uma loja
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public decimal ObtemSaldoEstoque(int idLoja)
        {
            var sql = @"
                SELECT sum(saldo) AS saldo
                    FROM
                      ( SELECT idLoja,
                               idProd,

                         (SELECT SaldoValorMov
                          FROM mov_estoque_centro_custo mecc1
                          WHERE mecc1.idProd = mecc.IdProd
                            AND mecc1.idLoja = " + idLoja + @"
                          ORDER BY IdMovEstoqueCentroCusto DESC LIMIT 1) AS saldo
                       FROM mov_estoque_centro_custo mecc
                        WHERE mecc.idLoja = " + idLoja + @"
                       GROUP BY mecc.idprod) AS tmp";

            return ExecuteScalar<decimal>(sql);
        }

        /// <summary>
        /// Obtem o ultimo plano de contas de uma movimentação de compra
        /// </summary>
        public int? ObtemUltimoIdConta(GDASession session, int idLoja, int idProd)
        {
            var sql = @"
                SELECT IdConta
                FROM mov_estoque_centro_custo
                WHERE IdConta is not null AND IdLoja = " + idLoja + " AND IdProd = " + idProd + @"
                ORDER BY IdMovEstoqueCentroCusto Desc";

            return ExecuteScalar<int?>(session, sql);
        }

        #endregion

        #region Atualiza o saldos (Qtde e Valor)

        /// <summary>
        /// Atualiza o saldos (Qtde e Valor)
        /// </summary>
        private void AtualizaSaldos(GDASession session, int idMovEstoqueCentroCusto, int idProd, int idLoja)
        {
            var sql = @"
                SET @saldoQtde := COALESCE((SELECT SaldoQtdeMov
                FROM mov_estoque_centro_custo
                WHERE idLoja = ?idLoja AND idProd = ?idProd
	                AND IdMovEstoqueCentroCusto < ?idMov
                ORDER BY IdMovEstoqueCentroCusto LIMIT 1), 0);

                UPDATE mov_estoque_centro_custo 
                SET SaldoQtdeMov = (@saldoQtde := @saldoQtde + if(tipoMov=1, QtdeMov, -QtdeMov)),
                SaldoValorMov = ((ValorMov / QtdeMov) * @saldoQtde)
                WHERE idLoja = ?idLoja AND idProd = ?idProd
	                AND IdMovEstoqueCentroCusto > ?idMov
                ORDER BY IdMovEstoqueCentroCusto;";

            objPersistence.ExecuteCommand(session, sql, new GDAParameter("?idMov", idMovEstoqueCentroCusto),
                new GDAParameter("?idProd", idProd), new GDAParameter("?idLoja", idLoja));
        }

        #endregion

        #region Obtem movimentações

        /// <summary>
        /// Recupera as movimentações de uma compra
        /// </summary>
        private List<MovEstoqueCentroCusto> ObtemMovimentacoes(GDASession session, int idCompra, int idPedidoInterno)
        {
            var sql = @"
                SELECT *
                FROM mov_estoque_centro_custo
                WHERE 1";

            if (idCompra > 0)
                sql += " AND idCompra = " + idCompra;

            if (idPedidoInterno > 0)
                sql += " AND IdPedidoInterno = " + idPedidoInterno;

            sql += " ORDER BY idMovEstoqueCentroCusto";

            return objPersistence.LoadData(session, sql).ToList();
        }

        #endregion

        #region Apaga movimentações

        /// <summary>
        /// Remove as movimentações de uma compra
        /// </summary>
        public void DeleteByCompra(int idCompra)
        {
            DeleteByCompra(null, idCompra);
        }

        /// <summary>
        /// Remove as movimentações de uma compra
        /// </summary>
        public void DeleteByCompra(GDASession session, int idCompra)
        {
            var movAnteriores = ObtemMovimentacoes(session, idCompra, 0);

            if (movAnteriores.Count() > 0)
            {
                var movAnterior = movAnteriores.OrderBy(f => f.IdMovEstoqueCentroCusto).FirstOrDefault();
                
                objPersistence.ExecuteCommand(session, "DELETE FROM mov_estoque_centro_custo WHERE idCompra = " + idCompra);

                AtualizaSaldos(session, movAnterior.IdMovEstoqueCentroCusto, movAnterior.IdProd, movAnterior.IdLoja);
            }
        }

        /// <summary>
        /// Remove as movimentações de um pedido interno
        /// </summary>
        /// <param name="idPedidoInterno"></param>
        public void DeleteByPedidoInterno(int idPedidoInterno)
        {
            var movAnteriores = ObtemMovimentacoes(null, 0, idPedidoInterno);

            if (movAnteriores.Count() > 0)
            {
                var movAnterior = movAnteriores.OrderBy(f => f.IdMovEstoqueCentroCusto).FirstOrDefault();

                objPersistence.ExecuteCommand("DELETE FROM mov_estoque_centro_custo WHERE idPedidoInterno = " + idPedidoInterno);

                AtualizaSaldos(null, movAnterior.IdMovEstoqueCentroCusto, movAnterior.IdProd, movAnterior.IdLoja);
            }
        }

        #endregion
    }
}
