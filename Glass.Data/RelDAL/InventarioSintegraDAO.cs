using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using Glass.Data.DAL;
using GDA;
using Glass.Data.Model;

namespace Glass.Data.RelDAL
{
    public sealed class InventarioSintegraDAO : BaseDAO<InventarioSintegra, InventarioSintegraDAO>
    {
        //private InventarioSintegraDAO() { }

        /// <summary>
        /// Obtem o invetario do ano anterior ao passado por parâmento
        /// </summary>
        /// <param name="dataAtual">Data que o registro do sintegra está sendo gerado</param>
        /// <param name="idLoja">Loja que será obtido o inventário</param>
        /// <returns></returns>
        public IList<InventarioSintegra> ObtemInvetario(DateTime dataAtual, uint idLoja)
        {
            string sql = @"
                Select p.codInterno, pl.estoqueFiscal - Coalesce(pnf.qtde, 0) As Qtd, 
	                Cast(If(pnf.total Is Not Null, pnf.total - ((pnf.total / pl.estoqueFiscal) * pnf.qtde), Coalesce(p.valorFiscal, p.custoCompra, p.custoFabBase) * pl.estoqueFiscal) As Decimal(12,2)) As Total 
                From produto p
	                Left Join 
	                (
                        Select * From (
		                    Select Coalesce(pbef.idProdBaixa, p.idProd) As idProd, Sum(If(nf.tipoDocumento = 2, 
                                (Coalesce(pbef.qtde, 1) * If(totM > 0, totM, pnf.qtde)) * -1, (Coalesce(pbef.qtde, 1) * If(totM > 0, totM, pnf.qtde)))) As qtde,
                                Cast(Sum(total) As Decimal(12,2)) As total
	                        From produtos_nf pnf
                                Left Join produto_baixa_estoque_fiscal pbef ON (pnf.idProd=pbef.idProd) 
    		                    Inner Join nota_fiscal nf ON (pnf.idNf=nf.idNf)
                                Inner Join produto p ON (pnf.idProd=p.idProd)
                                Inner Join grupo_prod g ON (p.idGrupoProd=g.idGrupoProd)
                            Where nf.dataEmissao>=?data And nf.idloja=" + idLoja + @" 
                                And g.tipoGrupo<>" + (int)TipoGrupoProd.UsoConsumo + @"
    	                    Group By Coalesce(pbef.idProdBaixa, p.idProd)
                        ) As temp
	                ) pnf ON (p.idProd=pnf.idProd)
	                Inner Join produto_loja pl ON (p.idProd=pl.idProd)
                    Inner Join grupo_prod g ON (p.idGrupoProd=g.idGrupoProd)
                Where (pl.estoqueFiscal - Coalesce(pnf.qtde, 0)) > 0 And pl.idloja=" + idLoja + @"
                    And g.tipoGrupo<>" + (int)TipoGrupoProd.UsoConsumo;

            return objPersistence.LoadData(sql, new GDAParameter("?data", DateTime.Parse("01/01/" + dataAtual.Year))).ToList();
        }
    }
}