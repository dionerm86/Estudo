using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.DAL;
using GDA;
using Glass.Data.RelModel;
using Glass.Data.Helper;

namespace Glass.Data.RelDAL
{
    public sealed class ItemApuracaoDAO : BaseDAO<ItemApuracao, ItemApuracaoDAO>
    {
        //private ItemApuracaoDAO() { }

        private GDAParameter[] GetParams(int mes, int ano)
        {
            List<GDAParameter> lstParams = new List<GDAParameter>();

            DateTime dataIni = new DateTime(ano, mes, 1, 0, 0, 0);
            DateTime dataFim = new DateTime(ano, mes, DateTime.DaysInMonth(ano, mes), 23, 59, 059);

            lstParams.Add(new GDAParameter("?dataIni", dataIni));

            lstParams.Add(new GDAParameter("?dataFim", dataFim));

            return lstParams.ToArray();
        }

        private List<ItemApuracao> ObtemItens(uint idLoja, int mes, int ano, string tipoImposto, bool incluirSt, bool exibeIpi, LoginUsuario login)
        {
            List<ItemApuracao> retorno = new List<ItemApuracao>(),
                entrada = new List<ItemApuracao>(),
                saida = new List<ItemApuracao>();

            #region Entrada

            var itensEntrada = ItemEntradaDAO.Instance.ObterItensEntrada(idLoja, mes, ano, true, exibeIpi, login);

            // Conversão para o objeto de retorno
            entrada.AddRange(from i in itensEntrada
                             where i.TipoImposto == tipoImposto
                             select new ItemApuracao()
                             {
                                 AliquotaICMS = i.Aliquota,
                                 BaseCalculo = i.BaseCalculo,
                                 Contabil = i.CodigoContabil,
                                 Estado = i.UFOrigem,
                                 Fiscal = i.CFOP,
                                 Imposto = i.ImpostoCreditado,
                                 IdNF = i.IdNF,
                                 IsentasNaoTributadas = i.IsentasNaoTributadas,
                                 Operacao = 1,
                                 ValorContabil = i.ValorContabil,
                                 ExibirNoRelatorio = true
                             });

            // Atribui aos itens de entrada o ICMS ST
            if (incluirSt)
                entrada.ForEach(x =>
                {
                    if (x.IdNF == 0)
                        return;

                    x.ImpostoST = itensEntrada.
                        Where(y => y.IdNF == x.IdNF && y.TipoImposto == "ICMS ST").
                        Sum(y => y.ImpostoCreditado);
                });

            // Inclui um item vazio para que a página seja exibida no relatório
            if (entrada.Count == 0)
                entrada.Add(new ItemApuracao()
                {
                    Operacao = 1,
                    ExibirNoRelatorio = false
                });

            // Coloca os itens de entrada na lista de retorno
            retorno.AddRange(entrada);

            #endregion

            #region Saída

            var itensSaida = ItemSaidaDAO.Instance.ObterItensSaida(idLoja, mes, ano);

            // Conversão para o objeto de retorno
            saida.AddRange(from i in itensSaida
                           where i.TipoImposto == tipoImposto
                           select new ItemApuracao()
                           {
                               AliquotaICMS = i.Aliquota,
                               BaseCalculo = i.BaseCalculo,
                               Contabil = i.CodigoContabil,
                               Estado = i.UFDestinatario,
                               Fiscal = i.CodigoFiscal,
                               Imposto = i.ImpostoDebitado,
                               IdNF = i.IdNF,
                               IsentasNaoTributadas = i.IsentasNaoTributadas,
                               Operacao = 2,
                               ValorContabil = i.ValorContabil,
                               ExibirNoRelatorio = true
                           });

            // Atribui aos itens de saída o ICMS ST
            if (incluirSt)
                saida.ForEach(x =>
                {
                    if (x.IdNF == 0)
                        return;

                    x.ImpostoST = itensSaida.
                        Where(y => y.IdNF == x.IdNF && y.TipoImposto == "ST").
                        Sum(y => y.SubTributaria);
                });

            // Inclui um item vazio para que a página seja exibida no relatório
            if (saida.Count == 0)
                saida.Add(new ItemApuracao()
                {
                    Operacao = 2,
                    ExibirNoRelatorio = false
                });

            // Coloca os itens de saída na lista de retorno
            retorno.AddRange(saida);

            #endregion

            // Agrupamento pelo tipo de CFOP e operação, ordenando pelo CFOP crescente
            retorno = (from r in retorno
                       group r by new { r.Fiscal, r.Operacao } into g
                       select new ItemApuracao()
                       {
                           AliquotaICMS = g.Sum(x => x.AliquotaICMS),
                           BaseCalculo = g.Sum(x => x.BaseCalculo),
                           Contabil = g.First().Contabil,
                           Estado = g.First().Estado,
                           Fiscal = g.Key.Fiscal,
                           Imposto = g.Sum(x => x.Imposto),
                           ImpostoST = g.Sum(x => x.ImpostoST),
                           IsentasNaoTributadas = g.Sum(x => x.IsentasNaoTributadas),
                           Operacao = g.Key.Operacao,
                           ValorContabil = g.Sum(x => x.ValorContabil),
                           ExibirNoRelatorio = g.First().ExibirNoRelatorio
                       }).OrderBy(x => x.Operacao).ThenBy(x => x.Fiscal).ToList();

            // Calcula os campos "Outras" e "Folha"
            int numeroItem = 0, offset = 0;
            bool alterou = false;
            retorno.ForEach(x =>
            {
                if (!alterou && x.Operacao == 2)
                {
                    if (numeroItem % 31 != 0)
                        offset = 1;

                    alterou = true;
                }

                x.Outras = x.ValorContabil - x.BaseCalculo - x.IsentasNaoTributadas;
                x.Folha = (++numeroItem / 31) + 2 + offset;
            });

            return retorno;
        }

        public List<ItemApuracao> ObterApuracaoICMS(uint idLoja, int mes, int ano, LoginUsuario login)
        {
            return ObtemItens(idLoja, mes, ano, "ICMS", true, false, login);

            /*
            string campoData = LivroRegistroDAO.Instance.SqlCampoDataEntrada("n", true);
            string campoDataInt = LivroRegistroDAO.Instance.SqlCampoDataEntrada("n2", true);
            string campoDataCte = LivroRegistroDAO.Instance.SqlCampoDataEntrada("ct", false);

            string sql = @"SET @i = 0;
                           SELECT FLOOR((@i := @i + 1)/10) + 2 AS NumeroPagina, 
                            temp.CodigoContabil, 
                            temp.CodigoFiscal,
                            sum(temp.ValorContabil) as valorContabil,
                            sum(temp.BaseCalculo) as baseCalculo,
                            sum(temp.Imposto) as imposto,
                            sum(temp.IsentasNaoTributadas) as isentasNaoTributadas,
                            sum(temp.ValorContabil - temp.BaseCalculo -  temp.IsentasNaoTributadas) AS Outras,
                            temp.Estado,
                            temp.Operacao,
                            sum(temp.ImpostoST) as impostoSt
                        FROM 
                        (
                            SELECT 
                                p.IdContaContabil AS CodigoContabil, 
                                cfop.CodInterno AS CodigoFiscal, 
                                CAST(SUM(Coalesce(
                                     (pnf.Total + 
                                     ((pnf.Total / n.TotalProd) * n.ValorFrete) + 
                                     ((pnf.Total / n.TotalProd) * n.ValorSeguro) + 
                                     ((pnf.Total / n.TotalProd) * n.OutrasDespesas) + 
                                     (pnf.ValorICMSST + pnf.ValorIPI)) - 
                                     ((n.Desconto / n.TotalProd) * pnf.Total),
      	                             n.totalNota)) AS DECIMAL(11,2)) AS ValorContabil,
                                CAST(SUM(If(pnf.idProdNf is not null, If(pnf.CodValorFiscal = 1, pnf.BCICMS,0), n.bcIcms)) AS DECIMAL(11,2)) AS BaseCalculo, 
                                CAST(SUM(If(pnf.idProdNf is not null, If(pnf.CodValorFiscal = 1, pnf.ValorICMS,0), n.valorIcms)) AS DECIMAL(11,2)) AS Imposto, 
                                CAST(SUM(If(pnf.CodValorFiscal = 2 , pnf.Total,if(pnf.CST = 20, ((pnf.Total + 
		                            ((pnf.Total / n.TotalProd) * n.ValorFrete) + 
		                            ((pnf.Total / n.TotalProd) * n.ValorSeguro) + 
		                            ((pnf.Total / n.TotalProd) * n.OutrasDespesas) + 
		                            (pnf.ValorICMSST + pnf.ValorIPI)) - 
		                            ((n.Desconto / n.TotalProd) * pnf.Total)) * pnf.PERCREDBCICMS / 100, 0))) AS DECIMAL(11,2)) AS IsentasNaoTributadas,
                                CAST(SUM(If(pnf.CodValorFiscal = 3 OR (pnf.Cst = 00 AND pnf.ValorICMS = 0), Coalesce(pnf.Total, n.TotalNota), 0)) AS DECIMAL(11,2)) AS Outras,
                                c.NomeUf AS Estado,
                                If(n.TipoDocumento = 2, 2,1) AS Operacao,
                                CAST(SUM(If(pnf.idProdNf is not null, If(pnf.CodValorFiscal = 1, pnf.ValorICMSST,0), n.valorIcmsSt)) AS DECIMAL(11,2)) AS ImpostoST 
                            FROM nota_fiscal n 
                                LEFT JOIN produtos_nf pnf ON(n.IdNF = pnf.IdNf)	
                                LEFT JOIN produto p ON(pnf.IdProd=p.IdProd)
                                LEFT JOIN natureza_operacao no ON (coalesce(pnf.idNaturezaOperacao, n.idNaturezaOperacao)=no.idNaturezaOperacao)
                                LEFT JOIN cfop ON(no.IdCfop=cfop.IdCfop)
                                LEFT JOIN (
    	                            select n1.idNf, c1.nomeUf
                                    from (
        	                            select idNf, case when n2.tipoDocumento=1 then n2.idCidade
            	                            when n2.tipoDocumento in (2,4) then c2.idCidade
                                            when n2.tipoDocumento=3 then f2.idCidade end as idCidade
                                        from nota_fiscal n2
            	                            left join cliente c2 on (n2.idCliente=c2.id_Cli)
                                            left join fornecedor f2 on (f2.idFornec=n2.idFornec)
                                        where If(n2.TipoDocumento = 2, n2.DataEmissao, " + campoDataInt + @") >= ?dataIni 
				                            AND If(n2.TipoDocumento = 2, n2.DataEmissao, " + campoDataInt + @") <= ?dataFim AND n2.Situacao IN (2,13)
                                    ) n1
        	                            left join cidade c1 on (n1.idCidade=c1.idCidade)
                                ) as c ON (n.idNf=c.idNf)
                            WHERE n.idLoja=" + idLoja + @" and n.tipoDocumento<>4 AND If(n.TipoDocumento = 2, n.DataEmissao, " + campoData + @") >= ?dataIni 
                                AND If(n.TipoDocumento = 2, n.DataEmissao, " + campoData + @") <= ?dataFim AND n.Situacao IN (2,13)   
                            GROUP BY cfop.CodInterno, If(n.TipoDocumento = 2, 2,1) 

                            UNION ALL SELECT 
                                efd.IdContaContabil AS CodigoContabil, 
                                cfop.CodInterno AS CodigoFiscal, 
                                CAST(SUM(ct.valorTotal) AS DECIMAL(11,2)) AS ValorContabil,
                                CAST(SUM(i.BaseCalc) AS DECIMAL(11,2)) AS BaseCalculo, 
                                CAST(SUM(i.Valor) AS DECIMAL(11,2)) AS Imposto, 
                                CAST(0 AS DECIMAL(11,2)) AS IsentasNaoTributadas,
                                CAST(0 AS DECIMAL(11,2)) AS Outras,
                                c.NomeUf AS Estado,
                                If(ct.TipoDocumentoCte = 2, 2,1) AS Operacao,
                                CAST(0 AS DECIMAL(11,2)) AS ImpostoST 
                            FROM conhecimento_transporte ct 
                                INNER JOIN efd_cte efd ON (ct.idCte=efd.idCte)
                                INNER JOIN imposto_cte i ON (ct.idCte=i.idCte and i.tipoImposto=" + (int)DataSourcesEFD.TipoImpostoEnum.Icms + @")
                                INNER JOIN participante_cte p ON (ct.idCte=p.idCte and p.tomador)
                                INNER JOIN participante_cte pl ON (ct.idCte=pl.idCte and if(ct.tipoDocumentoCte<>2, 
                                    pl.tipoParticipante=" + (int)ParticipanteCte.TipoParticipanteEnum.Destinatario + @", pl.idLoja>0))
                                LEFT JOIN natureza_operacao no ON (ct.idNaturezaOperacao=no.idNaturezaOperacao)
                                LEFT JOIN cfop ON(no.IdCfop=cfop.IdCfop)
                                LEFT JOIN fornecedor f ON (p.idFornec=f.idFornec)
                                LEFT JOIN transportador t ON (p.idTransportador=t.idTransportador)
                                LEFT JOIN cliente cli ON (p.idCliente=cli.id_Cli)
                                LEFT JOIN loja l ON (if(p.idLoja>0, p.idLoja, pl.idLoja)=l.idLoja)
                                LEFT JOIN cidade c ON (coalesce(f.idCidade, t.idCidade, cli.idCidade, l.idLoja)=c.idCidade)
                            WHERE l.idLoja=" + idLoja + @" AND " + campoDataCte + @" >= ?dataIni 
                                AND " + campoDataCte + @" <= ?dataFim AND ct.Situacao IN (2,13)
                            GROUP BY cfop.CodInterno, If(ct.TipoDocumentoCte = 2, 2,1)
                        ) as temp
                        GROUP BY temp.CodigoFiscal, temp.Operacao
                        ORDER BY temp.CodigoFiscal ASC";
            return objPersistence.LoadData(sql, GetParams(mes, ano)); */
        }

        public List<ItemApuracao> ObterApuracaoIPI(uint idLoja, int mes, int ano, LoginUsuario login)
        {
            return ObtemItens(idLoja, mes, ano, "IPI", false, true, login);

            /* string campoData = LivroRegistroDAO.Instance.SqlCampoDataEntrada("n", true);
            string campoDataInt = LivroRegistroDAO.Instance.SqlCampoDataEntrada("n2", true);

            string sql = @"SET @i = 0;
                                SELECT FLOOR((@i := @i + 1)/10) + 2 AS NumeroPagina, 
                                    temp.CodigoContabil, 
                                    temp.CodigoFiscal,
                                    temp.ValorContabil,
                                    temp.BaseCalculo,
                                    temp.Imposto,
                                    temp.IsentasNaoTributadas,
                                    (temp.ValorContabil - temp.BaseCalculo -  temp.IsentasNaoTributadas) AS Outras,
                                    temp.Estado,
                                    temp.Operacao
                                    FROM 
                                (
                                SELECT 
                                    p.IdContaContabil AS CodigoContabil, 
                                    cfop.CodInterno AS CodigoFiscal, 
                                    CAST(SUM(
                                         (pnf.Total + 
                                         ((pnf.Total / n.TotalProd) * n.ValorFrete) + 
                                         ((pnf.Total / n.TotalProd) * n.ValorSeguro) + 
                                         ((pnf.Total / n.TotalProd) * n.OutrasDespesas) + 
                                         (pnf.ValorICMSST + pnf.ValorIPI)) - 
                                         ((n.Desconto / n.TotalProd) * pnf.Total)
                                        ) AS DECIMAL(11,2)) AS ValorContabil,
                                    CAST(SUM(pnf.Total) AS DECIMAL(11,2)) AS BaseCalculo, 
                                    CAST(SUM(If(pnf.CSTIPI IN(00,50), pnf.ValorIPI,0)) AS DECIMAL(11,2)) AS Imposto, 
                                    CAST(SUM(If(pnf.CSTIPI IN(02,03,52,53) , pnf.Total,0)) AS DECIMAL(11,2)) AS IsentasNaoTributadas,
                                    CAST(SUM(If(pnf.CSTIPI IN(01,04,05,49,51,54,55,99), pnf.Total ,0)) AS DECIMAL(11,2)) AS Outras,
                                    c.NomeUf AS Estado,
                                    If(n.TipoDocumento = 2, 2,1) AS Operacao
                                FROM produtos_nf pnf
                                    LEFT JOIN nota_fiscal n ON(n.IdNF = pnf.IdNf)	
                                    LEFT JOIN produto p ON(pnf.IdProd=p.IdProd)
                                    LEFT JOIN natureza_operacao no ON (coalesce(pnf.idNaturezaOperacao, n.idNaturezaOperacao)=no.idNaturezaOperacao)
                                    LEFT JOIN cfop ON(no.IdCfop=cfop.IdCfop)
                                    LEFT JOIN (
                                        select n1.idNf, c1.nomeUf
                                        from (
                                            select idNf, case when n2.tipoDocumento=1 then n2.idCidade
                                                when n2.tipoDocumento in (2,4) then c2.idCidade
                                                when n2.tipoDocumento=3 then f2.idCidade end as idCidade
                                            from nota_fiscal n2
                                                left join cliente c2 on (n2.idCliente=c2.id_Cli)
                                                left join fornecedor f2 on (f2.idFornec=n2.idFornec)
                                            where If(n2.TipoDocumento = 2, n2.DataEmissao, " + campoDataInt + @") >= ?dataIni 
			                                    AND If(n2.TipoDocumento = 2, n2.DataEmissao, " + campoDataInt + @") <= ?dataFim AND n2.Situacao IN (2,13)
                                        ) n1
                                            left join cidade c1 on (n1.idCidade=c1.idCidade)
                                    ) as c ON (n.idNf=c.idNf)
                                WHERE n.idLoja=" + idLoja + @" and n.tipoDocumento<>4 AND If(n.TipoDocumento = 2, n.DataEmissao, " + campoData + @") >= ?dataIni 
                                    AND If(n.TipoDocumento = 2, n.DataEmissao, " + campoData + @") <= ?dataFim AND n.Situacao IN (2,13)   
                                GROUP BY cfop.CodInterno, If(n.TipoDocumento = 2, 2,1) 
                                ORDER BY cfop.CodInterno ASC
                                ) as temp";

            return objPersistence.LoadData(sql, GetParams(mes, ano)); */
        }
    }
}
