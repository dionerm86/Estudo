using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using GDA;

namespace Glass.Data.RelDAL
{
    public sealed class ItemLivroRegistroEntradaDAO : BaseDAO<ItemLivroRegistroEntrada, ItemLivroRegistroEntradaDAO>
    {
        //private ItemLivroRegistroEntradaDAO() { }

        private GDAParameter[] GetParams(string dataIni, string dataFim, int tipoDocumento)
        {
            List<GDAParameter> lstParams = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParams.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParams.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            if (tipoDocumento > 0)
                lstParams.Add(new GDAParameter("?tipoDocumento", tipoDocumento));

            return lstParams.ToArray();
        }

        public List<ItemLivroRegistroEntrada> ObterItensLivroRegistroEntrada(uint idLoja, string dataIni, string dataFim, int tipoDocumento)
        {
            string campoData = LivroRegistroDAO.Instance.SqlCampoDataEntrada("n", true);

            string sql = "SELECT " + campoData + @" AS DataEntrada, CASE WHEN n.Transporte = 1 THEN 'CTRC' ELSE 'NF' END AS Especie,
                            COALESCE(n.Serie, n.SubSerie) AS SerieSubSerie, n.NumeroNFE AS NumeroNota,
                            n.DataEmissao AS DataDocumento, f.CpfCnpj AS CodigoEmitente, c.NomeUf AS UFOrigem, n.TotalNota AS ValorContabil,
                            p.IdContaContabil AS CodigoContabil, cfop.CodInterno AS CodigoFiscal, CASE WHEN n.ValorICMS >0 THEN 'ICMS' END AS TipoImposto,
                            0 AS CodTipoImposto, pnf.BCICMS AS BaseCalculo, pnf.ALIQICMS AS Aliquota, 0.00 AS ImpostoCreditado, 
                            CASE WHEN n.Transporte = 1 THEN t.Nome ELSE f.NomeFantasia END AS Observacao
                            FROM nota_fiscal n
                            LEFT JOIN fornecedor f ON(n.IdFornec = f.IdFornec)
                            LEFT JOIN transportador t ON(n.IdTransportador = t.IdTransportador)
                            LEFT JOIN cidade c ON(n.IdCidade=c.IdCidade)
                            INNER JOIN produtos_nf pnf ON(n.IdNF = pnf.IdNf)
                            INNER JOIN produto p ON(pnf.IdProd=p.IdProd)
                            INNER JOIN natureza_operacao no ON(coalesce(pnf.idNaturezaOperacao, n.idNaturezaOperacao)=no.idNaturezaOperacao)
                            INNER JOIN cfop ON(no.IdCfop=cfop.IdCfop)
	                            WHERE n.idLoja=" + idLoja + @" AND " + campoData + @" >= ?dataIni AND " + campoData + @" <= ?dataFim AND n.TipoDocumento = ?tipoDocumento   
                            UNION ALL
                            SELECT " + campoData + @" AS DataEntrada, CASE WHEN n.Transporte = 1 THEN 'CTRC' ELSE 'NF' END AS Especie,
                            COALESCE(n.Serie, n.SubSerie) AS SerieSubSerie, n.NumeroNFE AS NumeroNota,
                            n.DataEmissao AS DataDocumento, f.CpfCnpj AS CodigoEmitente, c.NomeUf AS UFOrigem, n.TotalNota AS ValorContabil,
                            p.IdContaContabil AS CodigoContabil, cfop.CodInterno AS CodigoFiscal, CASE WHEN n.ValorIPI >0 THEN 'IPI' END AS TipoImposto,
                            0 AS CodTipoImposto, pnf.BCICMS AS BaseCalculo, pnf.ALIQICMS AS Aliquota, 0.00 AS ImpostoCreditado, 
                            CASE WHEN n.Transporte = 1 THEN t.Nome ELSE f.NomeFantasia END AS Observacao
                            FROM nota_fiscal n
                            LEFT JOIN fornecedor f ON(n.IdFornec = f.IdFornec)
                            LEFT JOIN transportador t ON(n.IdTransportador = t.IdTransportador)
                            LEFT JOIN cidade c ON(n.IdCidade=c.IdCidade)
                            INNER JOIN produtos_nf pnf ON(n.IdNF = pnf.IdNf)
                            INNER JOIN produto p ON(pnf.IdProd=p.IdProd)
                            INNER JOIN natureza_operacao no ON(coalesce(pnf.idNaturezaOperacao, n.idNaturezaOperacao)=no.idNaturezaOperacao)
                            INNER JOIN cfop ON(no.IdCfop=cfop.IdCfop)
	                            WHERE n.idLoja=" + idLoja + @" AND " + campoData + @" >= ?dataIni AND " + campoData + @" <= ?dataFim AND n.TipoDocumento = ?tipoDocumento";

            return objPersistence.LoadData(sql, GetParams(dataIni, dataFim, tipoDocumento));
        }
    }
}
