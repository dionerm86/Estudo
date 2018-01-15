using System;
using System.Collections.Generic;
using Glass.Data.RelModel;
using GDA;
using Glass.Data.DAL;
using Glass.Data.Helper;
using Glass.Data.Model.Cte;
using Glass.Data.EFD;

namespace Glass.Data.RelDAL
{
    public sealed class ItemEntradaDAO : BaseDAO<ItemEntrada, ItemEntradaDAO>
    {
        //private ItemEntradaDAO() { }

        private GDAParameter[] GetParams(int mes, int ano)
        {
            List<GDAParameter> lstParams = new List<GDAParameter>();

            DateTime dataIni = new DateTime(ano, mes, 1, 0, 0, 0);
            DateTime dataFim = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes), 23, 59, 059);

            lstParams.Add(new GDAParameter("?dataIni", dataIni));

            lstParams.Add(new GDAParameter("?dataFim", dataFim));

            return lstParams.ToArray();
        }

        public List<ItemEntrada> ObterItensEntrada(uint idLoja, int mes, int ano, bool exibeST, bool exibeIPI, LoginUsuario login)
        {
            string campoData = LivroRegistroDAO.Instance.SqlCampoDataEntrada("n", true);
            string campoDataCte = LivroRegistroDAO.Instance.SqlCampoDataEntrada("ct", false);

            int crt = LojaDAO.Instance.BuscaCrtLoja(null, NotaFiscalDAO.Instance.ObtemIdLoja(login.IdLoja));
            bool simples = crt == 1 || crt == 2;

            string sqlICMS = "";
            string sqlIPI = "";
            string sqlST = "";
            string sqlNf = "";
            string sqlUnion = "";

            string sqlPrincipal = @"SET @i = 0; SELECT FLOOR((@i := @i + 1)/15) + 2 AS NumeroPagina, e.DataEntrada, e.Especie, e.SerieSubSerie, 
                                                    CAST(e.Numero as CHAR(50)) as Numero, e.DataDocumento, e.CodigoEmitente, e.UFOrigem, 
		                                            CAST(e.ValorContabil AS DECIMAL(11,2)) AS ValorContabil,e.CodigoContabil, e.Fiscal, e.IcmsIpi,
		                                            e.CodValorFiscal, CAST(e.BaseCalculo AS DECIMAL(11,2)) AS BaseCalculo, 
                                                    CAST(e.Aliquota AS DECIMAL(11,2)) AS Aliquota, CAST(e.ImpostoCreditado AS DECIMAL(11,2)) AS ImpostoCreditado,
		                                            CAST(e.IsentasNaoTributadas AS DECIMAL(11,2)) AS IsentasNaoTributadas, e.Observacao, e.NomeEmitente, 
                                                    e.CNPJEmitente, e.InscEstEmitente, e.IdNF, e.CST, e.IdProdNF, e.SubTributaria
                                                FROM ({0}) as e ORDER BY  DATE_FORMAT(DataEntrada,'%d-%m-%Y'), Numero, IcmsIpi, CodValorFiscal, Fiscal ASC";

            sqlICMS = @"SELECT " + campoData + @" AS DataEntrada, IF(n.Transporte = 1, 'CTRC', 'NF') AS Especie, CONCAT_WS('-', n.Serie,n.SubSerie) AS SerieSubSerie,
                    n.NumeroNFE AS Numero, n.DataEmissao AS DataDocumento, f.IdFornec AS CodigoEmitente,
                    c.NomeUf AS UFOrigem,  p.IdContaContabil AS CodigoContabil, cfop.CodInterno AS Fiscal, 'ICMS' AS IcmsIpi,
                    pnf.CodValorFiscal AS CodValorFiscal, CAST(IF(pnf.CodValorFiscal = 1, pnf.ALIQICMS,0) AS DECIMAL(11,2)) AS Aliquota, 
                    CAST(SUM((Round(pnf.Total, 2) + 
                        ((pnf.Total / n.TotalProd) * n.ValorFrete) + 
                        ((pnf.Total / n.TotalProd) * n.ValorSeguro) + 
                        ((pnf.Total / n.TotalProd) * n.OutrasDespesas) + 
                        (Round(pnf.ValorICMSST + pnf.ValorIPI,2))) - 
                        ((n.Desconto / n.TotalProd) * pnf.Total)) AS DECIMAL(11,2)) AS ValorContabil,

                    /* Chamado 13471, deve somar somente a BCICMS do produto e se ele tiver */
                    CAST(SUM(/*If(pnf.CodValorFiscal = 1 Or pnf.CodValorFiscal = 3, */Round(pnf.BCICMS,2)/*, 
                        (Round(pnf.Total,2) + 
                        ((pnf.Total / n.TotalProd) * n.ValorFrete) + 
                        ((pnf.Total / n.TotalProd) * n.ValorSeguro) + 
                        ((pnf.Total / n.TotalProd) * n.OutrasDespesas) + 
                        (" + (!simples ? "Round(pnf.ValorICMSST,2) + " : "") + @"Round(pnf.ValorIPI,2))) - 
                        ((n.Desconto / n.TotalProd) * pnf.Total))*/) AS DECIMAL(11,2)) AS BaseCalculo, 
                    SUM(If(pnf.CodValorFiscal = 1, pnf.ValorICMS,0)) AS ImpostoCreditado, 
                    SUM(If(pnf.CodValorFiscal = 2, 
                        (Round(pnf.Total,2) + 
                        ((pnf.Total / n.TotalProd) * n.ValorFrete) + 
                        ((pnf.Total / n.TotalProd) * n.ValorSeguro) + 
                        ((pnf.Total / n.TotalProd) * n.OutrasDespesas) + 
                        (" + (!simples ? "Round(pnf.ValorICMSST,2) + " : "") + @"Round(pnf.ValorIPI,2))) - 
                        ((n.Desconto / n.TotalProd) * pnf.Total),0)) AS IsentasNaoTributadas, " +
                        (exibeST ? "IF(pnf.ValorICMSST > 0, CONCAT('ICMS CREDITADO: ', REPLACE(CAST(TRUNCATE(SUM(pnf.ValorICMSST),2) AS CHAR), '.', ',')), '')" : "''") + @" AS Observacao,
                    COALESCE(f.razaoSocial, f.nomeFantasia, cli.nome, cli.nomeFantasia) AS NomeEmitente, Coalesce(f.CpfCnpj, cli.Cpf_Cnpj) AS CNPJEmitente, 
                    Coalesce(f.RgInscEst, cli.Rg_EscInst) As InscEstEmitente, n.IdNF, pnf.CST, pnf.IDPRODNF, 0 AS SubTributaria 
                FROM produtos_nf pnf
                    LEFT JOIN nota_fiscal n ON(n.IdNF = pnf.IdNf)	
                    LEFT JOIN produto p ON(pnf.IdProd=p.IdProd)
                    LEFT JOIN natureza_operacao no ON (coalesce(pnf.idNaturezaOperacao, n.idNaturezaOperacao)=no.idNaturezaOperacao)
                    LEFT JOIN cfop ON(no.IdCfop=cfop.IdCfop)
                    LEFT JOIN tipo_cfop tcfop on (cfop.IdTipoCfop=tcfop.IdTipoCfop)
                    LEFT JOIN fornecedor f ON(f.IdFornec=n.IdFornec)
                    LEFT JOIN cliente cli ON(cli.Id_Cli=n.IdCliente)
                    LEFT JOIN cidade c ON (c.IdCidade=Coalesce(f.IdCidade, cli.idCidade))
                WHERE n.idLoja=" + idLoja + @" AND " + campoData + @" >= ?dataIni AND " + campoData + @" <= ?dataFim AND n.Situacao IN (2,13) 
                    AND n.TipoDocumento not in (2,4)
                    AND (n.servico=false Or n.modelo=3 Or n.modelo=6 Or n.modelo=21 Or n.modelo=22)
                GROUP BY n.IdNF, p.IdContaContabil, cfop.CodInterno
 
                UNION ALL SELECT " + campoDataCte + @" as DataEntrada, 'CTRC' as Especie, ct.Serie AS SerieSubSerie,
                    ct.NumeroCte AS Numero, ct.DataEmissao AS DataDocumento, coalesce(p.IdFornec, p.idTransportador, p.idCliente) AS CodigoEmitente,
                    c.NomeUf AS UFOrigem, efd.IdContaContabil AS CodigoContabil, cfop.CodInterno AS Fiscal, 'ICMS' AS IcmsIpi,
                    If(i.cst=0 Or i.cst=10 or i.cst=20, 1, if(i.cst=60 Or i.cst=70 or i.cst=90, 3, 2)) AS CodValorFiscal, CAST(i.aliquota AS DECIMAL(11,2)) AS Aliquota, 
                    CAST(Round(ct.valorTotal, 2) AS DECIMAL(11,2)) AS ValorContabil,
                    CAST(i.baseCalc AS DECIMAL(11,2)) AS BaseCalculo, SUM(i.valor) AS ImpostoCreditado, 0 as IsentasNaoTributadas, '' AS Observacao, 
                    COALESCE(f.razaoSocial, f.nomeFantasia, t.nome, t.nomeFantasia, cli.nome, cli.nomeFantasia) AS NomeEmitente, 
                    coalesce(f.CpfCnpj, t.cpfCnpj, cli.cpf_Cnpj) AS CNPJEmitente, 
                    coalesce(f.RgInscEst, t.inscEst, cli.Rg_EscInst) As InscEstEmitente, null, i.CST, null, 0 AS SubTributaria 
                FROM conhecimento_transporte ct
                    LEFT JOIN natureza_operacao no ON (ct.idNaturezaOperacao=no.idNaturezaOperacao)
                    LEFT JOIN cfop ON(no.IdCfop=cfop.IdCfop)
                    LEFT JOIN tipo_cfop tcfop on (cfop.IdTipoCfop=tcfop.IdTipoCfop)
                    LEFT JOIN participante_cte p ON (ct.idCte=p.idCte and p.tipoParticipante=" + (int)ParticipanteCte.TipoParticipanteEnum.Emitente + @")
                    LEFT JOIN participante_cte pl ON (ct.idCte=pl.idCte and pl.tipoParticipante=" + (int)ParticipanteCte.TipoParticipanteEnum.Destinatario + @")
                    LEFT JOIN fornecedor f ON (f.IdFornec=p.IdFornec)
                    LEFT JOIN cliente cli ON (cli.Id_Cli=p.IdCliente)
                    LEFT JOIN transportador t ON (t.idTransportador=p.idTransportador)
                    LEFT JOIN cidade c ON (c.IdCidade=ct.IdCidadeCte)
                    LEFT JOIN imposto_cte i ON (ct.idCte=i.idCte and i.tipoImposto=" + (int)DataSourcesEFD.TipoImpostoEnum.Icms + @")
                    LEFT JOIN efd_cte efd ON (ct.idCte=efd.idCte)
                WHERE pl.idLoja=" + idLoja + @" AND " + campoDataCte + @" >= ?dataIni AND " + campoDataCte + @" <= ?dataFim AND 
                    ct.Situacao IN (2,13) AND ct.TipoDocumentoCte <> 2
                GROUP BY ct.IdCte, efd.IdContaContabil, cfop.CodInterno";

            sqlIPI = @"SELECT " + campoData + @" AS DataEntrada, IF(n.Transporte = 1, 'CTRC', 'NF') AS Especie, CONCAT_WS('-', n.Serie,n.SubSerie) AS SerieSubSerie,
                    n.NumeroNFE AS Numero, n.DataEmissao AS DataDocumento, f.IdFornec AS CodigoEmitente,
                    c.NomeUf AS UFOrigem, p.IdContaContabil AS CodigoContabil, cfop.CodInterno AS Fiscal, 'IPI' AS IcmsIpi,
                    pnf.CodValorFiscalIPI AS CodValorFiscal, CAST(IF(pnf.CodValorFiscalIPI = 1, pnf.ALIQIPI,0) AS DECIMAL(11,2)) AS Aliquota, 
                    CAST(SUM((Round(pnf.Total, 2) + 
                        ((pnf.Total / n.TotalProd) * n.ValorFrete) + 
                        ((pnf.Total / n.TotalProd) * n.ValorSeguro) + 
                        ((pnf.Total / n.TotalProd) * n.OutrasDespesas) + 
                        (Round(pnf.ValorICMSST + pnf.ValorIPI,2))) - 
                        ((n.Desconto / n.TotalProd) * pnf.Total)) AS DECIMAL(11,2)) AS ValorContabil,

                    /* Chamado 15663:  */
                    CAST(SUM(/*If(pnf.CodValorFiscalIPI = 1, */(pnf.ValorIPI / (pnf.AliqIPI / 100))/*,(pnf.Total + 
                        ((pnf.Total / n.TotalProd) * n.ValorFrete) + 
                        ((pnf.Total / n.TotalProd) * n.ValorSeguro) + 
                        ((pnf.Total / n.TotalProd) * n.OutrasDespesas) + 
                        (pnf.ValorICMSST + pnf.ValorIPI)) - 
                        ((n.Desconto / n.TotalProd) * pnf.Total))*/) AS DECIMAL(11,2)) AS BaseCalculo, 
                        CAST(SUM(If(pnf.CodValorFiscalIPI = 1, pnf.ValorIPI,0))  AS DECIMAL(11,2)) AS ImpostoCreditado, 
                        CAST(SUM(If(pnf.CodValorFiscalIPI = 2, pnf.ValorIPI,0))  AS DECIMAL(11,2)) AS IsentasNaoTributadas,  
                    '' AS Observacao, COALESCE(f.razaoSocial, f.nomeFantasia, cli.nome, cli.nomeFantasia) AS NomeEmitente, 
                    Coalesce(f.CpfCnpj, cli.Cpf_Cnpj) AS CNPJEmitente, Coalesce(f.RgInscEst, cli.Rg_EscInst) As InscEstEmitente, n.IdNF, pnf.CST, pnf.IDPRODNF, 0 AS SubTributaria
                FROM produtos_nf pnf
                    LEFT JOIN nota_fiscal n ON(n.IdNF = pnf.IdNf)	
                    LEFT JOIN produto p ON(pnf.IdProd=p.IdProd)
                    LEFT JOIN natureza_operacao no ON (coalesce(pnf.idNaturezaOperacao, n.idNaturezaOperacao)=no.idNaturezaOperacao)
                    LEFT JOIN cfop ON(no.IdCfop=cfop.IdCfop)
                    LEFT JOIN tipo_cfop tcfop on (cfop.IdTipoCfop=tcfop.IdTipoCfop)
                    LEFT JOIN fornecedor f ON(f.IdFornec=n.IdFornec)
                    LEFT JOIN cliente cli ON(cli.Id_Cli=n.IdCliente)
                    LEFT JOIN cidade c ON (c.IdCidade=Coalesce(f.IdCidade, cli.idCidade))
                WHERE n.idLoja=" + idLoja + @" AND " + campoData + @" >= ?dataIni AND " + campoData + @" <= ?dataFim AND n.Situacao IN (2,13) AND n.TipoDocumento not in (2, 4)
                    AND (n.servico=false Or n.modelo=3 Or n.modelo=6 Or n.modelo=21 Or n.modelo=22)
                GROUP BY n.IdNF, p.IdContaContabil, cfop.CodInterno";

            sqlST = @"SELECT " + campoData + @" AS DataEntrada,IF(n.Transporte = 1, 'CTRC', 'NF') AS Especie, CONCAT_WS('-', n.Serie,n.SubSerie) AS SerieSubSerie,
                    n.NumeroNFE AS Numero, n.DataEmissao AS DataDocumento, f.IdFornec AS CodigoEmitente,
                    c.NomeUf AS UFOrigem, p.IdContaContabil AS CodigoContabil, cfop.CodInterno AS Fiscal, 'ICMS ST' AS IcmsIpi, pnf.CodValorFiscal AS CodValorFiscal,
                    CAST(IF(pnf.CodValorFiscal = 1, pnf.ALIQICMSST, 0) AS DECIMAL(11,2)) AS Aliquota, 
                    0 AS ValorContabil,
                    CAST(SUM(pnf.ValorICMSST) AS DECIMAL(11,2)) AS BaseCalculo, 
                    If(pnf.CodValorFiscal = 1, pnf.ValorICMSST,0) AS ImpostoCreditado, 
                    SUM(If(pnf.CodValorFiscal = 2, pnf.ValorICMSST,0)) AS IsentasNaoTributadas, 
                    '' AS Observacao, COALESCE(f.razaoSocial, f.nomeFantasia, cli.nome, cli.nomeFantasia) AS NomeEmitente, 
                    Coalesce(f.CpfCnpj, cli.Cpf_Cnpj) AS CNPJEmitente, Coalesce(f.RgInscEst, cli.Rg_EscInst) As InscEstEmitente,
                    n.IdNF, pnf.CST, pnf.IDPRODNF, SUM(pnf.ValorICMSST) AS SubTributaria
                FROM produtos_nf pnf
                    LEFT JOIN nota_fiscal n ON(n.IdNF = pnf.IdNf)	
                    LEFT JOIN produto p ON(pnf.IdProd=p.IdProd)
                    LEFT JOIN natureza_operacao no ON (coalesce(pnf.idNaturezaOperacao, n.idNaturezaOperacao)=no.idNaturezaOperacao)
                    LEFT JOIN cfop ON(no.IdCfop=cfop.IdCfop)
                    LEFT JOIN tipo_cfop tcfop on (cfop.IdTipoCfop=tcfop.IdTipoCfop)
                    LEFT JOIN fornecedor f ON(f.IdFornec=n.IdFornec)
                    LEFT JOIN cliente cli ON(cli.Id_Cli=n.IdCliente)
                    LEFT JOIN cidade c ON (c.IdCidade=Coalesce(f.IdCidade, cli.idCidade))
                WHERE n.servico=false And n.idLoja=" + idLoja + @" AND " + campoData + @" >= ?dataIni AND " + campoData + @" <= ?dataFim  AND n.Situacao IN (2,13) AND n.TipoDocumento not in (2,4)
                AND pnf.ValorICMSST > 0    
                GROUP BY n.IdNF, p.IdContaContabil, cfop.CodInterno";

            sqlNf = @"SELECT " + campoData + @" AS DataEntrada, IF(n.Transporte = 1, 'CTRC', 'NF') AS Especie, CONCAT_WS('-', n.Serie,n.SubSerie) AS SerieSubSerie,
                    n.NumeroNFE AS Numero, n.DataEmissao AS DataDocumento, f.IdFornec AS CodigoEmitente,
                    c.NomeUf AS UFOrigem, p.IdContaContabil AS CodigoContabil, 
                    cfop.CodInterno AS Fiscal, 'ICMS' AS IcmsIpi, i.CodValorFiscal AS CodValorFiscal,
                    CAST(IF(i.CodValorFiscal = 1, i.ALIQICMS,0) AS DECIMAL(11,2)) AS Aliquota,  CAST(SUM(n.TotalNota) AS DECIMAL(11,2)) AS ValorContabil,
                    CAST(SUM(If(i.CodValorFiscal = 1, n.BCICMS, 0))  AS DECIMAL(11,2)) AS BaseCalculo, 
                    SUM(If(i.CodValorFiscal = 1, n.ValorICMS,0)) AS ImpostoCreditado, SUM(If(i.CodValorFiscal = 2, n.ValorICMS,0)) AS IsentasNaoTributadas, '' AS Observacao, 
                    COALESCE(f.razaoSocial, f.nomeFantasia, cli.nome, cli.nomeFantasia) AS NomeEmitente, Coalesce(f.CpfCnpj, cli.Cpf_Cnpj) AS CNPJEmitente, 
                    Coalesce(f.RgInscEst, cli.Rg_EscInst) As InscEstEmitente, n.IdNF, i.CST, NULL AS IDPRODNF, 0 AS SubTributaria
                FROM nota_fiscal n 
                    LEFT JOIN info_adicional_nf i ON(n.IdNf=i.IdNf)
                    LEFT JOIN produtos_nf pnf ON(n.IdNF = pnf.IdNf)	
                    LEFT JOIN produto p ON(pnf.IdProd=p.IdProd)
                    LEFT JOIN natureza_operacao no ON (coalesce(pnf.idNaturezaOperacao, n.idNaturezaOperacao)=no.idNaturezaOperacao)
                    LEFT JOIN cfop ON(no.IdCfop=cfop.IdCfop)
                    LEFT JOIN tipo_cfop tcfop on (cfop.IdTipoCfop=tcfop.IdTipoCfop)
                    LEFT JOIN fornecedor f ON(f.IdFornec=n.IdFornec)
                    LEFT JOIN cliente cli ON(cli.Id_Cli=n.IdCliente)
                    LEFT JOIN cidade c ON (c.IdCidade=Coalesce(f.IdCidade, cli.idCidade))
                WHERE n.idLoja=" + idLoja + @" AND pnf.IdNf IS NULL AND " + campoData + @" >= ?dataIni AND " + campoData + @" <= ?dataFim 
                    AND n.Situacao IN (2,13) AND n.TipoDocumento not in (2,4)
                    AND (n.servico=false Or n.modelo=3 Or n.modelo=6 Or n.modelo=21 Or n.modelo=22 Or n.modelo=28 Or n.modelo=29)
                GROUP BY n.IdNF, p.IdContaContabil, cfop.CodInterno";

            sqlUnion = sqlICMS + (exibeIPI ? " UNION " + sqlIPI : "") + " UNION " + sqlST + " UNION " + sqlNf;

            return objPersistence.LoadData(string.Format(sqlPrincipal, sqlUnion), GetParams(mes, ano));
         }

        public List<ItemEntrada> ObterItensEntrada1(int mes, int ano)
        {
            string sql = "call sp_livro_registro_entrada(?dataIni, ?dataFim)";

            return objPersistence.LoadData(sql, GetParams(mes, ano));
        }
    }
}
