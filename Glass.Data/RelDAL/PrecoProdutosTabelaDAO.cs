using Glass.Data.DAL;
using Glass.Data.RelModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Data.RelDAL
{
    public class PrecoProdutosTabelaDAO : BaseDAO<PrecoProdutosTabela, PrecoProdutosTabelaDAO>
    {
        private string SqlPrecosTabelaProdutos(uint idTabelaDescontoAcrescimo, string codInterno,
            string descrProduto, uint idGrupoProd, string idsSubgrupoProd, uint tipoValor, decimal alturaInicio,
            decimal alturaFim, decimal larguraInicio, decimal larguraFim, bool produtoDesconto)
        {
            string criterio = "";

            var select = @"p.CODINTERNO AS CodInterno,
                    p.DESCRICAO AS DescProduto,
                    g.DESCRICAO AS DescGrupo,
                    s.DESCRICAO AS DescSubgrupo,
                    p.Altura,
                    p.Largura,
                    tdac.ValorOriginalUtilizado AS ValorOriginal,
                    ROUND((tdac.ValorOriginalUtilizado * IF(tdac.Acrescimo > 0 OR tdac.Desconto > 0, IF(tdac.Acrescimo > tdac.Desconto, (1 + ((tdac.Acrescimo - tdac.Desconto) / 100)), (1 - ((tdac.Desconto - tdac.Acrescimo) / 100))), 1)), 2) AS ValorTabela,
                    IF(tdac.Acrescimo > 0 OR tdac.Desconto > 0, IF(tdac.Acrescimo > tdac.Desconto, (1 + ((tdac.Acrescimo - tdac.Desconto) / 100)), (1 - ((tdac.Desconto - tdac.Acrescimo) / 100))), 1) AS PercDescAcrescimo,
                    '$$$' as criterio";

            var sql = "SELECT " + select +
                $@" from produto p
                        LEFT JOIN
                    grupo_prod g ON p.IDGRUPOPROD = g.IDGRUPOPROD
                        LEFT JOIN
                    subgrupo_prod s ON p.IDSUBGRUPOPROD = s.IDSUBGRUPOPROD
                        LEFT JOIN
                    (SELECT
                        p.IDPROD, 
                        dac.ACRESCIMO, 
                        dac.DESCONTO, 
                        (CASE {tipoValor} WHEN 1 THEN p.ValorAtacado WHEN 2 THEN p.ValorBalcao ELSE p.ValorObra END) ValorOriginalUtilizado
                    from produto p
                    LEFT JOIN desconto_acrescimo_cliente dac ON dac.IDCLIENTE IS NULL
                        AND((p.IDPROD = dac.IDPROD)
                        OR(p.IdGrupoProd = dac.IDGRUPOPROD
                        AND p.IDSUBGRUPOPROD = dac.IDSUBGRUPOPROD
                        AND dac.IDPROD IS NULL))
                    WHERE
                        dac.IDTABELADESCONTO={idTabelaDescontoAcrescimo}) tdac ON p.IDPROD = tdac.IDPROD
                    WHERE p.situacao=" + (int)Glass.Situacao.Ativo;

            if (!String.IsNullOrEmpty(codInterno))
            {
                sql += " and p.codInterno='" + codInterno + "'";
                criterio += "Produto: " + ProdutoDAO.Instance.GetDescrProduto(codInterno) + "    ";
            }
            else if (!String.IsNullOrEmpty(descrProduto))
            {
                string ids = ProdutoDAO.Instance.ObtemIds(null, descrProduto);
                sql += " And p.idProd In (" + ids + ")";
                criterio += "Produto: " + descrProduto + "    ";
            }

            if (idGrupoProd > 0)
            {
                sql += " and p.idGrupoProd=" + idGrupoProd;
                criterio += "Grupo: " + GrupoProdDAO.Instance.GetDescricao((int)idGrupoProd) + "    ";
            }

            if (!String.IsNullOrEmpty(idsSubgrupoProd) && !new List<string>(idsSubgrupoProd.Split(',')).Contains("0"))
            {
                sql += " and p.idSubgrupoProd in (" + idsSubgrupoProd + ")";
                criterio += "Subgrupos: " + SubgrupoProdDAO.Instance.GetDescricao(idsSubgrupoProd) + "    ";
            }

            if (alturaInicio > 0 || alturaFim > 0)
            {
                sql += " and p.altura >= " + alturaInicio +
                    (alturaFim > 0 ? " AND p.altura <= " + alturaFim : "");
                criterio += "Altura: " + alturaInicio + "Até" + alturaFim;
            }

            if (larguraInicio > 0 || larguraFim > 0)
            {
                sql += " and p.largura >= " + larguraInicio +
                    (larguraFim > 0 ? " AND p.largura <= " + larguraFim : "");
                criterio += "Largura: " + larguraInicio + "Até" + larguraFim;
            }      
            
            if(produtoDesconto)
                sql += @" AND (tdac.Acrescimo > 0 OR tdac.Desconto > 0) ";

            return sql.Replace("$$$", criterio);
        }

        public IList<PrecoProdutosTabela> GetPrecosTabelaProdutos(uint idTabelaDescontoAcrescimo, string codInterno,
            string descrProduto, uint idGrupoProd, string idsSubgrupoProd, uint tipoValor, decimal alturaInicio,
            decimal alturaFim, decimal larguraInicio, decimal larguraFim, int ordenacao, bool produtoDesconto, 
            string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !string.IsNullOrEmpty(sortExpression) ? sortExpression :
                ordenacao == 0 ? "p.codInterno" :
                ordenacao == 1 ? "p.descricao, p.espessura, p.idcorvidro, p.idcoraluminio, p.idcorferragem" :
                ordenacao == 2 ? "g.descricao, s.descricao" :
                "s.descricao";

            var sql = SqlPrecosTabelaProdutos(idTabelaDescontoAcrescimo, codInterno,
                descrProduto, idGrupoProd, idsSubgrupoProd, tipoValor, alturaInicio,
                alturaFim, larguraInicio, larguraFim, produtoDesconto);

            var retorno = LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, null); 

            return retorno;
        }

        public int GetPrecosTabelaProdutosCount(uint idTabelaDescontoAcrescimo, string codInterno, 
            string descrProduto, uint idGrupoProd, string idsSubgrupoProd, uint tipoValor, 
            decimal alturaInicio, decimal alturaFim, decimal larguraInicio, decimal larguraFim, int ordenacao, bool produtoDesconto)
        {
            var sql = SqlPrecosTabelaProdutos(idTabelaDescontoAcrescimo, codInterno,
                descrProduto, idGrupoProd, idsSubgrupoProd, tipoValor, alturaInicio,
                alturaFim, larguraInicio, larguraFim, produtoDesconto);

            var retorno = objPersistence.LoadData(sql);

            return retorno.ToList().Count;
        }


        public IList<PrecoProdutosTabela> GetPrecosTabelaProdutosRpt(uint idTabelaDescontoAcrescimo, string codInterno,
            string descrProduto, uint idGrupoProd, string idsSubgrupoProd, uint tipoValor, decimal alturaInicio,
            decimal alturaFim, decimal larguraInicio, decimal larguraFim, int ordenacao, bool produtoDesconto)
        {
            string sortExpression =
                ordenacao == 0 ? "p.codInterno" :
                ordenacao == 1 ? "p.descricao, p.espessura, p.idcorvidro, p.idcoraluminio, p.idcorferragem" :
                ordenacao == 2 ? "g.descricao, s.descricao" :
                "s.descricao";

            var sql = SqlPrecosTabelaProdutos(idTabelaDescontoAcrescimo, codInterno,
                descrProduto, idGrupoProd, idsSubgrupoProd, tipoValor, alturaInicio,
                alturaFim, larguraInicio, larguraFim, produtoDesconto);

            sql += " order by " + sortExpression;

            var retorno = objPersistence.LoadData(sql);

            return retorno.ToList();
        }        
    }
}
