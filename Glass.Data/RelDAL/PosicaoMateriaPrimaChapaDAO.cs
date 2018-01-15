using Glass.Data.DAL;
using Glass.Data.Model;
using Glass.Data.RelModel;
using System.Collections.Generic;

namespace Glass.Data.RelDAL
{
    public sealed class PosicaoMateriaPrimaChapaDAO : BaseDAO<PosicaoMateriaPrimaChapa, PosicaoMateriaPrimaChapaDAO>
    {
        #region Busca as chapas pela cor e espessura

        /// <summary>
        /// Busca as chapas do subgrupo Chapas de Vidro através da sua cor e espessura.
        /// </summary>
        /// <param name="idCorVidro"></param>
        /// <param name="espessura"></param>
        /// <returns></returns>
        public IList<PosicaoMateriaPrimaChapa> GetChapaByCorEsp(uint idCorVidro, float espessura, string idsCorVidro, string espessuras)
        {
            var sql = @"
                SELECT p.IdProdBase, p.IdCorVidro, p.Espessura, p.CodInterno, p.Descricao, p.Altura, p.largura,
                    CAST((pnf.totM / ROUND(pnf.TotM/(p.Altura*p.Largura/1000000), 0)) * SUM(pnf.Qtde - COALESCE(corte.Qtde, 0) - COALESCE(perda.Qtde, 0))  AS decimal(12, 2)) AS totalM2Chapa,
                    SUM(pnf.Qtde - COALESCE(corte.Qtde, 0) - COALESCE(perda.Qtde, 0)) AS qtdeChapa,
                    CAST(CONCAT(f.idFornec, ' - ', " + FornecedorDAO.Instance.GetNomeFornecedor("f") + @") as char) as nomeFornecedor
                FROM produtos_nf pnf
                    INNER JOIN nota_fiscal nf ON (pnf.idNf = nf.idNf)
                    INNER JOIN fornecedor f ON (nf.idFornec = f.idFornec)
	                INNER JOIN produto p ON (pnf.idProd = p.idProd)
	                LEFT JOIN subgrupo_prod s ON (p.idSubgrupoProd = s.idSubgrupoProd)

                    LEFT JOIN
    				(
    					SELECT IdProdNf, COUNT(*) as Qtde
        				FROM
        				(
        					SELECT pi.IdProdNf
    						FROM produto_impressao pi
        						INNER JOIN chapa_corte_peca ccp ON (ccp.idProdimpressaochapa = pi.IdProdImpressao)
        					WHERE (pi.Cancelado IS NULL OR pi.Cancelado=0) AND pi.IdProdNf IS NOT NULL
                                AND pi.IdProdImpressao NOT IN (SELECT IdProdImpressao FROM perda_chapa_vidro WHERE (Cancelado IS NULL OR Cancelado=0))
        					GROUP BY pi.IdProdImpressao
        				) as tmp
        				GROUP BY IdProdNf
    				) as corte ON (pnf.IdProdNf = corte.IdProdNf)
                    
    				LEFT JOIN
    				(
    					SELECT IdProdNf, COUNT(*) as Qtde
        				FROM
        				(
        					SELECT pi.IdProdNf
    						FROM produto_impressao pi
        						INNER JOIN perda_chapa_vidro pcv ON (pi.IdProdImpressao = pcv.IdProdImpressao AND !pcv.Cancelado)
        					WHERE (pi.Cancelado IS NULL OR pi.Cancelado=0) AND pi.IdProdNf IS NOT NULL
        					GROUP BY pi.IdProdImpressao
        				) as tmp
        				GROUP BY IdProdNf
    				) as perda ON (pnf.IdProdNf = perda.IdProdNf)

                WHERE s.tipoSubgrupo IN(" + (int)TipoSubgrupoProd.ChapasVidro + "," + (int)TipoSubgrupoProd.ChapasVidroLaminado + @")
                    AND nf.tipoDocumento in (" + (int)NotaFiscal.TipoDoc.EntradaTerceiros + "," + (int)NotaFiscal.TipoDoc.NotaCliente + "," + (int)NotaFiscal.TipoDoc.Entrada + @")
                    AND nf.situacao IN(" + (int)NotaFiscal.SituacaoEnum.FinalizadaTerceiros + "," + (int)NotaFiscal.SituacaoEnum.Autorizada + @")
                    AND (nf.GerarEtiqueta IS NULL OR nf.GerarEtiqueta=1)
                    {0}
                GROUP BY p.idProd, f.idFornec
                HAVING qtdeChapa > 0";

            string filtro = "";

            if (idCorVidro > 0)
                filtro += string.Format(" AND (p.IdCorVidro IS NOT NULL AND p.IdCorVidro={0})", idCorVidro);
            else if (!string.IsNullOrEmpty(idsCorVidro))
                filtro += string.Format(" AND (p.IdCorVidro IS NOT NULL AND p.IdCorVidro IN ({0}))", idsCorVidro);

            if (espessura > 0)
                filtro += string.Format(" AND (p.Espessura IS NOT NULL AND p.Espessura={0})", espessura);
            else if (!string.IsNullOrEmpty(espessuras))
                filtro += string.Format(" AND (p.Espessura IS NOT NULL AND CAST(p.Espessura AS Decimal) IN ({0}))", espessuras);

            sql = string.Format(sql, filtro);

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion
    }
}
