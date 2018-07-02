using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.RelModel;
using Glass.Data.DAL;

namespace Glass.Data.RelDAL
{
    public sealed class ItemSaidaDAO : BaseDAO<ItemSaida, ItemSaidaDAO>
    {
        //private ItemSaidaDAO() { }

        private GDAParameter[] GetParams(int mes, int ano)
        {
            List<GDAParameter> lstParams = new List<GDAParameter>();

            DateTime dataIni = new DateTime(ano, mes, 1, 0, 0, 0);
            DateTime dataFim = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes), 23, 59, 059);

            lstParams.Add(new GDAParameter("?dataIni", dataIni));

            lstParams.Add(new GDAParameter("?dataFim", dataFim));

            return lstParams.ToArray();
        }

        public List<ItemSaida> ObterItensSaida(uint idLoja, int mes, int ano)
        {
            string sql = @"SET @i = 0;
                            SELECT 
                                FLOOR((@i := @i + 1)/30) + 2 AS NumeroPagina,
                                e.Especie, 
                                e.SerieSubSerie, 
                                e.NumeroNota, 
                                e.Dia, 
                                e.UFDestinatario, 
                                IF(Situacao = 2, e.ValorContabil,0) AS ValorContabil,
                                e.CodigoContabil,
                                e.CodigoFiscal,
                                e.CodValorFiscal,
                                IF(Situacao = 2, e.IcmsIpi,'') AS IcmsIpi,
                                IF(Situacao = 2, e.BaseCalculo,0) AS BaseCalculo,
                                IF(Situacao = 2, e.Aliquota,0) AS Aliquota,
                                IF(Situacao = 2, e.ImpostoDebitado,0) AS ImpostoDebitado,
                                IF(Situacao = 2, e.IsentasNaoTributadas,0) AS IsentasNaoTributadas,
                                IF(Situacao = 2, e.Outras,0) AS Outras,
                                IF(Situacao = 2, e.SubTributaria,0) AS SubTributaria,
                                IF(Situacao = 2, e.BaseCalculoST,0) AS BaseCalculoST,
                                e.Observacao,
                                e.IdNF,
                                e.Situacao,
                                e.CST,
                                e.IdProdNF
                            FROM(
                                    SELECT 
                                        IF(n.Transporte = 1, 'CTRC', 'NF') AS Especie,
                                        Cast(CONCAT_WS('-', n.Serie,n.SubSerie) as char) AS SerieSubSerie,
                                        n.NumeroNFE AS NumeroNota,
                                        Cast(DATE_FORMAT(n.DataEmissao, '%d') as char) AS Dia,
                                        c.NomeUf AS UFDestinatario, 
                                        SUM(
                                            (pnf.Total + 
           	                                ((pnf.Total / n.TotalProd) * n.ValorFrete) + 
         	                                ((pnf.Total / n.TotalProd) * n.ValorSeguro) + 
           	                                ((pnf.Total / n.TotalProd) * n.OutrasDespesas) + 
           	                                (pnf.ValorICMSST + pnf.ValorIPI)) - 
           	                                ((n.Desconto / n.TotalProd) * pnf.Total)
                                        ) AS ValorContabil,
                                        p.IdContaContabil AS CodigoContabil,	
                                        cfop.CodInterno AS CodigoFiscal,
                                        'ICMS' AS IcmsIpi,
                                        pnf.CodValorFiscal AS CodValorFiscal,
                                        CAST(SUM(IF(pnf.CodValorFiscal = 1, pnf.BCICMS,0)) AS DECIMAL(11,2)) AS BaseCalculo,
                                        CAST((IF(pnf.CodValorFiscal = 1, SUM(pnf.ValorICMS)/SUM(pnf.BCICMS),0)) AS DECIMAL(11,2))*100 AS Aliquota, 
                                        CAST(SUM(IF(pnf.CodValorFiscal = 1, pnf.ValorICMS,0)) AS DECIMAL(11,2)) AS ImpostoDebitado, 
                                        CAST(SUM(IF(pnf.CodValorFiscal = 2 , 
                                            (pnf.Total + 
                                            ((pnf.Total / n.TotalProd) * n.ValorFrete) + 
                                            ((pnf.Total / n.TotalProd) * n.ValorSeguro) + 
                                            ((pnf.Total / n.TotalProd) * n.OutrasDespesas) + 
                                            (pnf.ValorICMSST + pnf.ValorIPI)) - 
                                            ((n.Desconto / n.TotalProd) * pnf.Total),0)) AS DECIMAL(11,2)) AS IsentasNaoTributadas,
                                        CAST(SUM(IF(pnf.CodValorFiscal = 3 , 
                                            (pnf.Total + 
                                            ((pnf.Total / n.TotalProd) * n.ValorFrete) + 
                                            ((pnf.Total / n.TotalProd) * n.ValorSeguro) + 
                                            ((pnf.Total / n.TotalProd) * n.OutrasDespesas) + 
                                            (pnf.ValorICMSST + pnf.ValorIPI)) - 
                                            ((n.Desconto / n.TotalProd) * pnf.Total),0)) AS DECIMAL(11,2)) AS Outras,
                                        IF(n.Situacao = 2, '','NFE Cancelada') As Observacao,
                                        0 AS SubTributaria, 
                                        0 AS BaseCalculoST, 
                                        n.IdNF,
                                        n.TipoDocumento,
                                        n.Situacao,
                                        pnf.CST,
                                        pnf.IDPRODNF
                                    FROM nota_fiscal n
                                        LEFT JOIN cliente cli ON(cli.Id_Cli = n.IdCliente)
                                        LEFT JOIN fornecedor f ON(f.IdFornec = n.IdFornec)
                                        LEFT JOIN cidade c ON(IFNULL(cli.IdCidade,f.IdCidade)=c.IdCidade)
                                        LEFT JOIN produtos_nf pnf ON(n.IdNF = pnf.IdNf)
                                        LEFT JOIN produto p ON(pnf.IdProd=p.IdProd)
                                        LEFT JOIN natureza_operacao no ON (coalesce(pnf.idNaturezaOperacao, n.idNaturezaOperacao)=no.idNaturezaOperacao)
                                        LEFT JOIN cfop ON(no.IdCfop=cfop.IdCfop)
                                     WHERE n.idLoja=" + idLoja + @" AND n.DataEmissao >= ?dataIni AND n.DataEmissao <= ?dataFim AND n.TipoDocumento = 2 AND n.Situacao IN(2)
                                        GROUP BY n.IdNF, p.IdContaContabil, cfop.CodInterno, pnf.CST -- , pnf.CodValorFiscal 
                                        -- CAST(IF(pnf.CodValorFiscal = 1, pnf.ALIQICMS,0) AS DECIMAL(11,2))                           
                            UNION ALL
                                    SELECT 
                                        IF(n.Transporte = 1, 'CTRC', 'NF') AS Especie,
                                        CONCAT_WS('-', n.Serie,n.SubSerie) AS SerieSubSerie,
                                        n.NumeroNFE AS NumeroNota,
                                        Cast(DATE_FORMAT(n.DataEmissao, '%d') as char) AS Dia,
                                        c.NomeUf AS UFDestinatario, 
                                        SUM(
                                            (pnf.Total + 
           	                                ((pnf.Total / n.TotalProd) * n.ValorFrete) + 
         	                                ((pnf.Total / n.TotalProd) * n.ValorSeguro) + 
           	                                ((pnf.Total / n.TotalProd) * n.OutrasDespesas) + 
           	                                (pnf.ValorICMSST + pnf.ValorIPI)) - 
           	                                ((n.Desconto / n.TotalProd) * pnf.Total)
                                        ) AS ValorContabil,
                                        p.IdContaContabil AS CodigoContabil,	
                                        cfop.CodInterno AS CodigoFiscal,
                                        'IPI' AS IcmsIpi,
                                        pnf.CodValorFiscalIPI AS CodValorFiscal,
                                        CAST(SUM(IF(pnf.CodValorFiscalIPI = 1, pnf.Total,0)) AS DECIMAL(11,2)) AS BaseCalculo,
                                        CAST((IF(pnf.CodValorFiscalIPI = 1, SUM(pnf.ValorIPI)/SUM(pnf.Total),0)) AS DECIMAL(11,2))*100 AS Aliquota, 
                                        CAST(SUM(IF(pnf.CodValorFiscalIPI = 1, pnf.ValorIPI,0)) AS DECIMAL(11,2)) AS ImpostoDebitado, 
                                        CAST(SUM(IF(pnf.CodValorFiscalIPI = 2 , 
                                            (pnf.Total + 
                                            ((pnf.Total / n.TotalProd) * n.ValorFrete) + 
                                            ((pnf.Total / n.TotalProd) * n.ValorSeguro) + 
                                            ((pnf.Total / n.TotalProd) * n.OutrasDespesas) + 
                                            (pnf.ValorICMSST + pnf.ValorIPI)) - 
                                            ((n.Desconto / n.TotalProd) * pnf.Total),0)) AS DECIMAL(11,2)) AS IsentasNaoTributadas,
                                        CAST(SUM(IF(pnf.CodValorFiscalIPI = 3 , 
                                            (pnf.Total + 
                                            ((pnf.Total / n.TotalProd) * n.ValorFrete) + 
                                            ((pnf.Total / n.TotalProd) * n.ValorSeguro) + 
                                            ((pnf.Total / n.TotalProd) * n.OutrasDespesas) + 
                                            (pnf.ValorICMSST + pnf.ValorIPI)) - 
                                            ((n.Desconto / n.TotalProd) * pnf.Total),0)) AS DECIMAL(11,2)) AS Outras,
                                        '' AS Observacao, 
                                        0 AS SubTributaria, 
                                        0 AS BaseCalculoST, 
                                        n.IdNF,
                                        n.TipoDocumento,
                                        n.Situacao,
                                        pnf.CST,
                                        pnf.IDPRODNF
                                    FROM nota_fiscal n
                                        LEFT JOIN cliente cli ON(cli.Id_Cli = n.IdCliente)
                                        LEFT JOIN fornecedor f ON(f.IdFornec = n.IdFornec)
                                        LEFT JOIN cidade c ON(IFNULL(cli.IdCidade,f.IdCidade)=c.IdCidade)
                                        INNER JOIN produtos_nf pnf ON(n.IdNF = pnf.IdNf)
                                        INNER JOIN produto p ON(pnf.IdProd=p.IdProd)
                                        INNER JOIN natureza_operacao no ON(coalesce(pnf.idNaturezaOperacao, n.idNaturezaOperacao)=no.idNaturezaOperacao)
                                        INNER JOIN cfop ON(no.IdCfop=cfop.IdCfop)
                                     WHERE n.idLoja=" + idLoja + @" AND n.DataEmissao >= ?dataIni AND n.DataEmissao <= ?dataFim AND n.TipoDocumento = 2 AND n.Situacao IN(2) -- AND pnf.ValorIPI > 0 
                                        GROUP BY n.IdNF, p.IdContaContabil, cfop.CodInterno, pnf.CST -- , pnf.CodValorFiscalIPI 
                                        -- CAST(IF(pnf.CodValorFiscalIPI = 1, pnf.ALIQIPI,0) AS DECIMAL(11,2))  
                            UNION ALL
                                SELECT 
                                        '' AS Especie,
                                        '' AS SerieSubSerie,
                                        n.NumeroNFE AS NumeroNota,
                                        '0' AS Dia,
                                        '' UFDestinatario, 
                                        0 AS ValorContabil,
                                        p.IdContaContabil AS CodigoContabil,	
                                        cfop.CodInterno AS CodigoFiscal,
                                        'ST' AS IcmsIpi,
                                        pnf.CodValorFiscal AS CodValorFiscal,
                                        0 AS BaseCalculo,
                                        0 AS Aliquota, 
                                        0 AS ImpostoDebitado, 
                                        0 AS IsentasNaoTributadas,
                                        0 AS Outras,
                                        '' AS Observacao, 
                                        SUM(pnf.ValorICMSST) AS SubTributaria, 
                                        SUM(pnf.BCICMSST) AS BaseCalculoST, 
                                        n.IdNF,
                                        n.TipoDocumento,
                                        n.Situacao,
                                        pnf.CST,
                                        pnf.IDPRODNF
                                    FROM nota_fiscal n
                                        LEFT JOIN cliente cli ON(cli.Id_Cli = n.IdCliente)
                                        LEFT JOIN fornecedor f ON(f.IdFornec = n.IdFornec)
                                        LEFT JOIN cidade c ON(IFNULL(cli.IdCidade,f.IdCidade)=c.IdCidade)
                                        INNER JOIN produtos_nf pnf ON(n.IdNF = pnf.IdNf)
                                        INNER JOIN produto p ON(pnf.IdProd=p.IdProd)
                                        INNER JOIN natureza_operacao no ON(coalesce(pnf.idNaturezaOperacao, n.idNaturezaOperacao)=no.idNaturezaOperacao)
                                        INNER JOIN cfop ON(no.IdCfop=cfop.IdCfop)
                                     WHERE n.idLoja=" + idLoja + @" AND n.DataEmissao >= ?dataIni AND n.DataEmissao <= ?dataFim AND n.TipoDocumento = 2 AND n.Situacao IN(2)
                                        GROUP BY n.IdNF, p.IdContaContabil, cfop.CodInterno, pnf.CST -- , pnf.CodValorFiscal 
                                        -- CAST(IF(pnf.CodValorFiscal = 1, pnf.ALIQICMS,0) AS DECIMAL(11,2))
                                )
                            AS e ORDER BY NumeroNota, CodigoFiscal, IcmsIpi, Dia ASC";

            return objPersistence.LoadData(sql, GetParams(mes, ano));
        }
    }
}
