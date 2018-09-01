using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GDA;

namespace Glass.Data.DAL
{
    public class ProdutoNfCustoDAO : BaseDAO<ProdutoNfCusto, ProdutoNfCustoDAO>
    {
        /// <summary>
        /// Recupera os custos dos produtos pela nota fiscal.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public IEnumerable<ProdutoNfCusto> ObterCustosPorNotaFiscal(GDA.GDASession sessao, uint idNf)
        {
            return objPersistence.LoadData(sessao,
                @"SELECT pnc.*, pnEntrada.AliqIpi AS AliqIpiCompra, pnEntrada.AliqIcms AS AliqIcmsCompra 
                  FROM produto_nf_custo pnc 
                  INNER JOIN produtos_nf pn ON (pnc.IdProdNf=pn.IdProdNf) 
                  LEFT JOIN produtos_nf pnEntrada ON (pnc.IdProdNfEntrada=pnEntrada.IdProdNf)
                  WHERE pn.IdNf=?id", new GDA.GDAParameter("?id", idNf)).ToList();
        }

        /// <summary>
        /// Recupera os custos do produto da nota fiscal.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdNf"></param>
        /// <returns></returns>
        public IEnumerable<ProdutoNfCusto> ObterCustosPorProdutoNotaFiscal(GDA.GDASession sessao, uint idProdNf)
        {
            return objPersistence.LoadData(sessao,
                @"SELECT pnc.*, pnEntrada.AliqIpi AS AliqIpiCompra, pnEntrada.AliqIcms AS AliqIcmsCompra  
                FROM produto_nf_custo pnc
                LEFT JOIN produtos_nf pnEntrada ON (pnc.IdProdNfEntrada=pnEntrada.IdProdNf)
                WHERE IdProdNf=?id",
                new GDA.GDAParameter("?id", idProdNf)).ToList();
        }

        /// <summary>
        /// Atualiza a rentabilidade do custo do produto da nota fiscal.
        /// </summary>
        /// <param name="idProdNfCusto"></param>
        /// <param name="percentualRentabilidade">Percentual da rentabilidade.</param>
        /// <param name="rentabilidadeFinanceira">Rentabilidade financeira.</param>
        public void AtualizarRentabilidade(GDA.GDASession sessao,
            int idProdNfCusto, decimal percentualRentabilidade, decimal rentabilidadeFinanceira)
        {
            objPersistence.ExecuteCommand(sessao, "UPDATE produto_nf_custo SET PercentualRentabilidade=?percentual, RentabilidadeFinanceira=?rentabilidade WHERE IdProdNfCusto=?id",
                new GDA.GDAParameter("?percentual", percentualRentabilidade),
                new GDA.GDAParameter("?rentabilidade", rentabilidadeFinanceira),
                new GDA.GDAParameter("?id", idProdNfCusto));
        }

        /// <summary>
        /// Apaga os registros associados com o produto da nota fiscal.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idProdNf"></param>
        public void ApagarPorProdutoNf(GDA.GDASession sessao, uint idProdNf)
        {
            // Apaga os registro de rentabilidade
            objPersistence.ExecuteCommand(sessao, 
                "DELETE FROM produto_nf_custo_rentabilidade WHERE IdProdNfCusto IN (SELECT IdProdNfCusto FROM produto_nf_custo pnc WHERE pnc.IdProdNf=?id)", 
                new GDA.GDAParameter("?id", idProdNf));

            objPersistence.ExecuteCommand(sessao, "DELETE FROM produto_nf_custo WHERE IdProdNf=?id", new GDA.GDAParameter("?id", idProdNf));
        }

        public override int Delete(GDASession session, ProdutoNfCusto objDelete)
        {
            ProdutoNfCustoRentabilidadeDAO.Instance.ApagarPorProdutoNfCusto(session, objDelete.IdProdNfCusto);
            return base.Delete(session, objDelete);
        }
    }
}
