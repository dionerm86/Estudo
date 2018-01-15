using System;
using System.Linq;
using System.Text;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ProdutoFornecedorCotacaoCompraDAO : BaseDAO<ProdutoFornecedorCotacaoCompra, ProdutoFornecedorCotacaoCompraDAO>
    {
        //private ProdutoFornecedorCotacaoCompraDAO() { }

        private string Sql(uint idCotacaoCompra, uint idFornec, uint idProd, bool apenasCadastrados, bool selecionar)
        {
            StringBuilder sql = new StringBuilder("select "), where = new StringBuilder(), interno = new StringBuilder();
            
            sql.Append(selecionar ? @"pcc.idCotacaoCompra, pcc.idProd, pf.idFornec, coalesce(pfcc.custoUnit, 
                pf.custoCompra) as custoUnit, coalesce(pfcc.prazoEntregaDias, pf.prazoEntregaDias) as prazoEntregaDias,
                (pfcc.idProd is not null) as cadastrado, cast(if(pfcc.idProd is not null, pfcc.idParcela, f.tipoPagto) as unsigned) as idParcela,
                pfcc.datasPagamentos, total.custoTotal" : "count(distinct concat(pcc.idProd, ',', pf.idFornec))");

            if (idFornec > 0)
            {
                where.AppendFormat(" and pf.idFornec={0}", idFornec);
                interno.AppendFormat(" and idFornec={0}", idFornec);
            }

            if (idProd > 0)
            {
                where.AppendFormat(" and pcc.idProd={0}", idProd);
                interno.AppendFormat(" and idProd={0}", idProd);
            }
            else
            {
                string idsProd = GetValoresCampo("select idProd from produto_cotacao_compra where idCotacaoCompra=" + idCotacaoCompra, "idProd");
                interno.AppendFormat(" and idProd in ({0})", idsProd != String.Empty ? idsProd : "0");
            }

            if (apenasCadastrados)
                where.Append(" and pfcc.idProd is not null");

            sql.AppendFormat(@"
                from produto_cotacao_compra pcc
                    left join (
                        select * from (
	                        select * from produto_fornecedor
	                        where date(coalesce(dataVigencia, now()))>=date(now()) {6}
	                        order by dataVigencia is null asc, dataVigencia asc
                        ) as temp
                        group by idFornec, idProd
                    ) pf on (pcc.idProd=pf.idProd)
                    left join produto_fornecedor_cotacao_compra pfcc on (pcc.idCotacaoCompra=pfcc.idCotacaoCompra and 
                        pcc.idProd=pfcc.idProd and pf.idFornec=pfcc.idFornec)
                    left join fornecedor f on (pf.idFornec=f.idFornec)
                    left join (
                        select pcc1.idProd, pfcc1.idFornec, cast(coalesce(pfcc1.custoUnit, 0)*sum(if({2} in ({3}), pcc1.TotM, if({2} in ({4}), pcc1.qtde * pcc1.altura, 
                            if({2} in ({5}), pcc1.qtde * ((pcc1.altura + pcc1.largura) * 2), pcc1.qtde)))) as decimal(12,2)) as custoTotal
                        from produto_cotacao_compra pcc1
                            left join produto p1 on (pcc1.idProd=p1.idProd)
                            left join grupo_prod g1 on (p1.idGrupoProd=g1.idGrupoProd)
                            left join subgrupo_prod s1 on (p1.idSubgrupoProd=s1.idSubgrupoProd)
                            left join (
                                select * from (
	                                select * from produto_fornecedor
	                                where date(coalesce(dataVigencia, now()))>=date(now()) {6}
	                                order by dataVigencia is null asc, dataVigencia asc
                                ) as temp
                                group by idFornec, idProd
                            ) pf1 on (pcc1.idProd=pf1.idProd)
                            left join produto_fornecedor_cotacao_compra pfcc1 on (pcc1.idCotacaoCompra=pfcc1.idCotacaoCompra and 
                                pcc1.idProd=pfcc1.idProd and pf1.idFornec=pfcc1.idFornec)
                        where pcc1.idCotacaoCompra={0} and pfcc1.idFornec is not null
                        group by pcc1.idProd, pfcc1.idFornec
                    ) as total on (pcc.idProd=total.idProd and pfcc.idFornec=total.idFornec)
                where pcc.idCotacaoCompra={0} {7} {1}",

                idCotacaoCompra,
                FILTRO_ADICIONAL,
                String.Format("coalesce(s1.tipoCalculo, g1.tipoCalculo, {0})", (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd),
                (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto,
                (int)Glass.Data.Model.TipoCalculoGrupoProd.ML + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 + "," + 
                    (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 + "," + (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6,
                (int)Glass.Data.Model.TipoCalculoGrupoProd.Perimetro,
                interno.ToString(),
                where.ToString());

            if (selecionar)
                sql.Append(" group by pcc.idProd, pf.idFornec");

            return sql.ToString();
        }

        public ProdutoFornecedorCotacaoCompra[] ObtemProdutosFornecedorCotacao(GDASession session, uint idCotacaoCompra, uint idFornec, 
            uint idProd, bool apenasCadastrados)
        {
            string sql = Sql(idCotacaoCompra, idFornec, idProd, apenasCadastrados, true).Replace(FILTRO_ADICIONAL, "");
            return objPersistence.LoadData(session, sql).ToArray();
        }

        public ProdutoFornecedorCotacaoCompra[] ObtemProdutosFornecedorCotacao(uint idCotacaoCompra, uint idFornec, 
            uint idProd, bool apenasCadastrados, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(idCotacaoCompra, idFornec, idProd, apenasCadastrados, true), 
                sortExpression, startRow, pageSize).ToArray();
        }

        public int ObtemNumeroProdutosFornecedorCotacao(uint idCotacaoCompra, uint idFornec, uint idProd, bool apenasCadastrados)
        {
            return GetCountWithInfoPaging(Sql(idCotacaoCompra, idFornec, idProd, apenasCadastrados, true), false);
        }
    }
}
