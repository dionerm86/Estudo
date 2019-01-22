using System;
using System.Linq;
using System.Collections.Generic;
using Glass.Data.RelModel;
using GDA;
using Glass.Data.DAL;
using Glass.Data.Helper;

namespace Glass.Data.RelDAL
{
    public sealed class PlanoContasDAO : BaseDAO<PlanoContas, PlanoContasDAO>
    {
        //private PlanoContasDAO() { }

        #region Sql Geral

        private string SqlCamposVenc(string dataIni, string dataFim, bool ajustado, bool filtro, bool agruparMes)
        {
            string sql = "";

            if ((ajustado || filtro) && !String.IsNullOrEmpty(dataIni) && !String.IsNullOrEmpty(dataFim))
            {
                string sqlVencNaoPagas = @"
                    select sum(coalesce(valorVenc,0)) as valor
                    from contas_pagar
                    where (dataPagto is null or date(dataPagto)>date(?dataFim))
                        and date(dataVenc)>=date(?dataIni) and date(dataVenc)<=date(?dataFim) and idConta=plano_contas" + (filtro ? "1" : "") + @".idConta";

                string sqlVencPassadoPagas = @"
                    select sum(coalesce(valorPago,0)) as valor
                    from contas_pagar
                    where paga=true and date(dataVenc)<date(?dataIni) and date(dataPagto)>=date(?dataIni)
                        and date(dataPagto)<=date(?dataFim) and idConta=plano_contas" + (filtro ? "1" : "") + @".idConta";

                if (agruparMes)
                {
                    sqlVencNaoPagas += " and month(dataVenc)=Mes and year(dataVenc)=Ano";
                    sqlVencPassadoPagas += " and month(dataPagto)=Mes and year(dataPagto)=Ano";
                }

                sql = ", cast((" + sqlVencNaoPagas + ") as decimal(12,2)) as VencPeriodoNaoPagas, cast((" +
                    sqlVencPassadoPagas + ") as decimal(12,2)) as VencPassadoPagasPeriodo";
            }

            return sql;
        }

        private string ObterSqlGeralPlanoConta(int idCategoriaConta, int idGrupoConta, int[] idsPlanoConta, int idLoja, string dataIni, string dataFim, int tipoMov, int tipoConta,
            string filtroGrupos, bool detalhado, bool sintetico, bool ajustado, bool agruparMes, bool selecionar)
        {
            var sqlGeralPlanoConta = string.Empty;
            var camposPlanoConta = string.Empty;
            var filtro = string.Empty;
            var sqlSintetico = SqlGeral((uint)idCategoriaConta, (uint)idGrupoConta, idsPlanoConta?.Select(f => (uint)f)?.ToArray(), (uint)idLoja, dataIni, dataFim, tipoMov, tipoConta, false, false,
                false, true, true, false, true, false, false);

            if (selecionar)
            {
                camposPlanoConta = @"p.IdConta,
                    p.Descricao AS PlanoConta,
                    g.Descricao AS GrupoConta,
                    cat.IdCategoriaConta,
                    cat.Descricao AS DescrCategoria,
                    0 AS Valor,
                    1 AS TipoMov,
                    NULL AS DataVenc,
                    g.NumSeq AS NumSeqGrupo,
                    cat.NumSeq AS NumSeqCateg,
                    cat.Tipo AS TipoCategoria,
                    NULL AS NomeCliente,
                    NULL AS NomeFornecedor,
                    NULL AS Data,
                    NULL AS IdsPagto,
                    NULL AS IdPagto,
                    NULL AS IdCompra,
                    NULL AS IdDeposito,
                    NULL AS IdPedido,
                    NULL AS IdAcerto,
                    NULL AS IdLiberarPedido,
                    NULL AS Obs";

                if (agruparMes)
                {
                    camposPlanoConta += ", NULL AS Mes, NULL AS Ano";
                }
            }
            else if (sintetico)
            {
                camposPlanoConta = "p.IdConta, 0 AS Valor";
            }
            else
            {
                camposPlanoConta = "COUNT(*) AS Cont";
            }

            sqlGeralPlanoConta += $@" UNION ALL

                (SELECT { camposPlanoConta } FROM plano_contas p
                    LEFT JOIN grupo_conta g ON (p.IdGrupo=g.IdGrupo)
                    LEFT JOIN categoria_conta cat ON (g.IdCategoriaConta=cat.IdCategoriaConta)
                WHERE p.IdConta NOT IN
                    (SELECT IdConta FROM ({ sqlSintetico }
                    HAVING (VencPeriodoNaoPagas=0 OR VencPeriodoNaoPagas IS NULL)
                        AND (VencPassadoPagasPeriodo=0 OR VencPassadoPagasPeriodo IS NULL)) AS tbl2)
                    { filtroGrupos }";

            if (idCategoriaConta > 0)
            {
                filtro += $" AND g.IdCategoriaConta={ idCategoriaConta }";
            }

            if (idGrupoConta > 0)
            {
                filtro += $" AND g.IdGrupo={ idGrupoConta }";
            }

            if (idsPlanoConta.Any())
            {
                filtro += $" AND p.IdConta IN ({ string.Join(",", idsPlanoConta) })";
            }

            sqlGeralPlanoConta += $"{ filtro } GROUP  BY p.IdConta)";

            return sqlGeralPlanoConta;
        }

        private string ObterSqlGeralContaBanco(int idCategoriaConta, int idGrupoConta, int[] idsPlanoConta, int idLoja, string dataIni, string dataFim, int tipoMov, int tipoConta,
            string filtroGrupos, bool detalhado, bool sintetico, bool agruparMes, bool selecionar)
        {
            var sqlContaBanco = string.Empty;
            var camposContaBanco = string.Empty;
            var filtro = string.Empty;

            if (selecionar)
            {
                var campoValor = string.Empty;
                var campoTipoMov = string.Empty;
                var campoIdPagto = string.Empty;
                var campoIdsPagto = string.Empty;
                var camposAgruparMes = string.Empty;

                if (detalhado)
                {
                    campoValor = "m.ValorMov AS Valor,";
                    campoTipoMov = "m.TipoMov,";
                    campoIdPagto = "m.IdPagto,";
                    campoIdsPagto = "CAST(m.IdPagto AS CHAR) AS IdsPagto,";
                }
                else
                {
                    campoValor = "ABS(SUM(m.ValorMov * IF(m.TipoMov = 1, 1, -1))) AS Valor,";
                    campoTipoMov = "IF(SUM(m.ValorMov * IF(m.TipoMov = 1, 1, -1)) < 0, 2, 1) AS TipoMov,";
                    campoIdPagto = "NULL AS IdPagto,";
                    campoIdsPagto = "GROUP_CONCAT(DISTINCT(m.IdPagto)) AS IdsPagto,";
                }

                if (agruparMes)
                {
                    camposAgruparMes = @", MONTH(m.DataMov) AS Mes,
                        YEAR(m.DataMov) AS Ano";
                }

                camposContaBanco = $@"p.IdConta,
                    p.Descricao AS PlanoConta,
                    g.Descricao AS GrupoConta,
                    cat.IdCategoriaConta,
                    cat.Descricao AS DescrCategoria,
                    { campoValor }
                    { campoTipoMov }
                    NULL AS DataVenc,
                    g.NumSeq AS NumSeqGrupo,
                    cat.NumSeq AS NumSeqCateg,
                    cat.Tipo AS TipoCategoria,
                    cli.Nome AS NomeCliente,
                    COALESCE(f.RazaoSocial, f.NomeFantasia) AS NomeFornecedor,
                    CAST(DATE_FORMAT(m.DataMov, '%Y/%m/%d') AS Date) AS Data,
                    { campoIdsPagto }
                    { campoIdPagto }
                    NULL AS IdCompra,
                    m.IdDeposito,
                    m.IdPedido,
                    m.IdAcerto,
                    m.IdLiberarPedido,
                    CONVERT(m.Obs USING UTF8) AS Obs
                    {camposAgruparMes}";
            }
            else if (sintetico)
            {
                camposContaBanco = "m.IdConta, (m.ValorMov * IF(m.TipoMov = 1, 1, -1)) AS Valor";
            }
            else
            {
                camposContaBanco = "COUNT(*) AS Cont";
            }

            sqlContaBanco = $@"SELECT { camposContaBanco } FROM mov_banco m
                    LEFT JOIN funcionario func ON (m.UsuCad=func.IdFunc)
                    LEFT JOIN plano_contas p ON (m.IdConta=p.IdConta)
                    LEFT JOIN grupo_conta g ON (p.IdGrupo=g.IdGrupo)
                    LEFT JOIN pedido ped ON (m.IdPedido=ped.IdPedido)
                    LEFT JOIN categoria_conta cat ON (g.IdCategoriaConta=cat.IdCategoriaConta)
                    LEFT JOIN cliente cli ON (m.IdCliente=cli.Id_Cli)
                    LEFT JOIN fornecedor f ON (m.IdFornec=f.IdFornec)
                    LEFT JOIN conta_banco cb on (m.IdContaBanco = cb.IdContaBanco)
                WHERE 1 { filtroGrupos }";

            if (tipoConta == 1)
            {
                filtro += " AND 0=1";
            }
            else
            {
                if (idCategoriaConta > 0)
                {
                    filtro += $" AND g.IdCategoriaConta = { idCategoriaConta }";
                }

                if (idGrupoConta > 0)
                {
                    filtro += $" AND g.IdGrupo = { idGrupoConta }";
                }

                if (idsPlanoConta.Any())
                {
                    filtro += $" AND p.IdConta IN ({ string.Join(",", idsPlanoConta) })";
                }

                if (idLoja > 0)
                {
                    filtro += $" AND cb.IdLoja = {idLoja}";
                }

                if (!string.IsNullOrWhiteSpace(dataIni))
                {
                    filtro += " AND m.DataMov >= ?dataIni";
                }

                if (!string.IsNullOrWhiteSpace(dataFim))
                {
                    filtro += " AND m.DataMov <= ?dataFim";
                }

                if (tipoMov > 0)
                {
                    filtro += $" AND m.TipoMov = { tipoMov }";
                }
            }

            sqlContaBanco += filtro;

            if (agruparMes)
            {
                sqlContaBanco += " GROUP BY MONTH(m.DataMov), YEAR(m.DataMov), m.IdConta";
            }
            else if (sintetico)
            {
                sqlContaBanco += " GROUP BY m.IdConta";
            }

            return sqlContaBanco;
        }

        private string ObterSqlGeralCaixaGeral(int idCategoriaConta, int idGrupoConta, int[] idsPlanoConta, int idLoja, string dataIni, string dataFim, int tipoMov, int tipoConta,
            string filtroGrupos, bool detalhado, bool sintetico, bool agruparMes, bool selecionar)
        {
            var sqlCaixaGeral = string.Empty;
            var camposCaixaGeral = string.Empty;
            var filtro = string.Empty;

            if (selecionar)
            {
                var campoJuros = Configuracoes.FinanceiroConfig.SubtrairJurosDRE ? "-c.Juros" : string.Empty;
                var campoValor = string.Empty;
                var campoTipoMov = string.Empty;
                var campoIdPagto = string.Empty;
                var campoIdsPagto = string.Empty;
                var camposAgruparMes = string.Empty;

                if (detalhado)
                {
                    campoValor = $"c.ValorMov{ campoJuros } AS Valor,";
                    campoTipoMov = "c.TipoMov,";
                    campoIdPagto = "c.IdPagto,";
                    campoIdsPagto = "CAST(c.IdPagto AS CHAR) AS IdsPagto,";
                }
                else
                {
                    campoValor = $"ABS(SUM(c.ValorMov * IF(c.TipoMov = 1, 1, -1))){ campoJuros } AS Valor,";
                    campoTipoMov = "IF(SUM(c.ValorMov * IF(c.TipoMov = 1, 1, -1)) < 0, 2, 1) AS TipoMov,";
                    campoIdPagto = "NULL AS IdPagto,";
                    campoIdsPagto = "GROUP_CONCAT(DISTINCT(c.IdPagto)) AS IdsPagto,";
                }

                if (agruparMes)
                {
                    camposAgruparMes = @", MONTH(COALESCE(c.DataMovBanco, c.DataMov)) AS Mes,
                        YEAR(COALESCE(c.DataMovBanco, c.DataMov)) AS Ano";
                }

                camposCaixaGeral = $@"p.IdConta,
                    p.Descricao AS PlanoConta,
                    g.Descricao AS GrupoConta,
                    cat.IdCategoriaConta,
                    cat.Descricao AS DescrCategoria,
                    { campoValor }
                    { campoTipoMov }
                    NULL AS DataVenc,
                    g.NumSeq AS NumSeqGrupo,
                    cat.NumSeq AS NumSeqCateg,
                    cat.Tipo AS TipoCategoria,
                    cli.Nome AS NomeCliente,
                    COALESCE(f.RazaoSocial, f.NomeFantasia) AS NomeFornecedor,
                    CAST(DATE_FORMAT(COALESCE(c.DataMovBanco, c.DataMov), '%Y/%m/%d') AS Date) AS Data,
                    { campoIdsPagto }
                    { campoIdPagto }
                    c.IdCompra,
                    NULL AS IdDeposito,
                    c.IdPedido,
                    c.IdAcerto,
                    c.IdLiberarPedido,
                    CONVERT(c.Obs USING UTF8) AS Obs
                    { camposAgruparMes }";
            }
            else if (sintetico)
            {
                camposCaixaGeral = "c.IdConta, (c.ValorMov * IF(c.TipoMov = 1, 1, -1)) AS Valor";
            }
            else
            {
                camposCaixaGeral = "COUNT(*) AS Cont";
            }

            sqlCaixaGeral = $@"SELECT { camposCaixaGeral } FROM caixa_geral c
                    LEFT JOIN plano_contas p ON (c.IdConta=p.IdConta)
                    LEFT JOIN grupo_conta g ON (p.IdGrupo=g.IdGrupo)
                    LEFT JOIN pedido ped ON (c.IdPedido=ped.IdPedido)
                    LEFT JOIN compra comp ON (c.IdCompra=comp.IdCompra)
                    LEFT JOIN categoria_conta cat ON (g.IdCategoriaConta=cat.IdCategoriaConta)
                    LEFT JOIN cliente cli ON (c.IdCliente=cli.Id_Cli)
                    LEFT JOIN fornecedor f ON (c.IdFornec=f.IdFornec)
                WHERE c.IdConta NOT IN ({ UtilsPlanoConta.PlanosContaDesconsiderarCxGeral })
                    { filtroGrupos }";

            if (tipoConta == 1)
            {
                filtro += " AND 0=1";
            }
            else
            {
                if (idCategoriaConta > 0)
                {
                    filtro += $" AND g.IdCategoriaConta = { idCategoriaConta }";
                }

                if (idGrupoConta > 0)
                {
                    filtro += $" AND g.IdGrupo = { idGrupoConta }";
                }

                if (idsPlanoConta.Any())
                {
                    filtro += $" AND p.IdConta IN ({ string.Join(",", idsPlanoConta) })";
                }

                if (idLoja > 0)
                {
                    filtro += $" AND c.IdLoja={ idLoja }";
                }

                if (!string.IsNullOrWhiteSpace(dataIni))
                {
                    filtro += " AND COALESCE(c.DataMovBanco, c.DataMov) >= ?dataIni";
                }

                if (!string.IsNullOrWhiteSpace(dataFim))
                {
                    filtro += " AND COALESCE(c.DataMovBanco, c.DataMov) <= ?dataFim";
                }

                if (tipoMov > 0)
                {
                    filtro += $" AND c.TipoMov = { tipoMov }";
                }
            }

            sqlCaixaGeral += filtro;

            if (agruparMes)
            {
                sqlCaixaGeral += " GROUP BY MONTH(COALESCE(c.DataMovBanco, c.DataMov)), YEAR(COALESCE(c.DataMovBanco, c.DataMov)), c.IdConta";
            }
            else if (sintetico)
            {
                sqlCaixaGeral += " GROUP BY c.IdConta";
            }

            return sqlCaixaGeral;
        }

        private string ObterSqlGeralCaixaDiario(int idCategoriaConta, int idGrupoConta, int[] idsPlanoConta, int idLoja, string dataIni, string dataFim, int tipoMov, int tipoConta,
            string filtroGrupos, bool detalhado, bool sintetico, bool agruparMes, bool selecionar)
        {
            var sqlCaixaDiario = string.Empty;
            var camposCaixaDiario = string.Empty;
            var filtro = string.Empty;

            if (selecionar)
            {
                var campoJuros = Configuracoes.FinanceiroConfig.SubtrairJurosDRE ? "-c.Juros" : string.Empty;
                var campoValor = string.Empty;
                var campoTipoMov = string.Empty;
                var camposAgruparMes = string.Empty;

                if (detalhado)
                {
                    campoValor = $"c.Valor{ campoJuros } AS Valor,";
                    campoTipoMov = "c.TipoMov,";
                }
                else
                {
                    campoValor = $"ABS(SUM(c.Valor * IF(c.TipoMov = 1, 1, -1))){ campoJuros } AS Valor,";
                    campoTipoMov = "IF(SUM(c.Valor * IF(c.TipoMov = 1, 1, -1)) < 0, 2, 1) AS TipoMov,";
                }

                if (agruparMes)
                {
                    camposAgruparMes = @", MONTH(c.DataCad) AS Mes,
                        YEAR(c.DataCad) AS Ano";
                }

                camposCaixaDiario = $@"p.IdConta,
                    p.Descricao AS PlanoConta,
                    g.Descricao AS GrupoConta,
                    cat.IdCategoriaConta,
                    cat.Descricao AS DescrCategoria,
                    { campoValor }
                    { campoTipoMov }
                    NULL AS DataVenc,
                    g.NumSeq AS NumSeqGrupo,
                    cat.NumSeq AS NumSeqCateg,
                    cat.Tipo AS TipoCategoria,
                    cli.Nome AS NomeCliente,
                    NULL AS NomeFornecedor,
                    c.DataCad AS Data,
                    NULL AS IdsPagto,
                    NULL AS IdPagto,
                    NULL AS IdCompra,
                    NULL AS IdDeposito,
                    c.IdPedido,
                    c.IdAcerto,
                    c.IdLiberarPedido,
                    CONVERT(c.Obs USING UTF8) AS Obs
                    { camposAgruparMes }";
            }
            else if (sintetico)
            {
                camposCaixaDiario = "c.IdConta, ABS(SUM(c.Valor * IF(c.TipoMov=1, 1, -1))) AS Valor";
            }
            else
            {
                camposCaixaDiario = "COUNT(*) AS Cont";
            }

            sqlCaixaDiario = $@"SELECT { camposCaixaDiario } FROM caixa_diario c
                    LEFT JOIN plano_contas p ON (c.IdConta=p.IdConta)
                    LEFT JOIN grupo_conta g ON (p.IdGrupo=g.IdGrupo)
                    LEFT JOIN categoria_conta cat ON (g.IdCategoriaConta=cat.IdCategoriaConta)
                    LEFT JOIN cliente cli ON (c.IdCliente=cli.Id_Cli)
                WHERE c.IdConta NOT IN ({ UtilsPlanoConta.PlanosContaDesconsiderarCxGeral })
                    { filtroGrupos }";

            if (tipoConta == 1)
            {
                filtro += " AND 0=1";
            }
            else
            {
                if (idCategoriaConta > 0)
                {
                    filtro += $" AND g.IdCategoriaConta = { idCategoriaConta }";
                }

                if (idGrupoConta > 0)
                {
                    filtro += $" AND g.IdGrupo = { idGrupoConta }";
                }

                if (idsPlanoConta.Any())
                {
                    filtro += $" AND p.IdConta IN ({ string.Join(",", idsPlanoConta) })";
                }

                if (idLoja > 0)
                {
                    filtro += $" AND c.IdLoja = { idLoja }";
                }

                if (!string.IsNullOrWhiteSpace(dataIni))
                {
                    filtro += " AND c.DataCad >= ?dataIni";
                }

                if (!string.IsNullOrWhiteSpace(dataFim))
                {
                    filtro += " AND c.DataCad <= ?dataFim";
                }

                if (tipoMov > 0)
                {
                    filtro += $" AND c.TipoMov = { tipoMov }";
                }
            }

            sqlCaixaDiario += filtro;

            if (agruparMes)
            {
                sqlCaixaDiario += " GROUP BY MONTH(c.DataCad), YEAR(c.DataCad), c.IdConta";
            }
            else if (sintetico)
            {
                sqlCaixaDiario += " GROUP BY c.IdConta";
            }

            return sqlCaixaDiario;
        }

        private string ObterSqlGeralContaPaga(int idCategoriaConta, int idGrupoConta, int[] idsPlanoConta, int idLoja, string dataIni, string dataFim, int tipoMov, int tipoConta,
            string filtroGrupos, bool detalhado, bool sintetico, bool agruparMes, bool selecionar)
        {
            var sqlContaPaga = string.Empty;
            var camposContaPaga = string.Empty;
            var filtro = string.Empty;

            if (selecionar)
            {
                var campoValor = string.Empty;
                var campoIdsPagto = string.Empty;
                var camposAgruparMes = string.Empty;

                if (detalhado)
                {
                    campoValor = "cp.ValorPago AS Valor,";
                    campoIdsPagto = "CAST(cp.IdPagto AS CHAR) AS IdsPagto,";
                }
                else
                {
                    campoValor = "SUM(cp.ValorPago) AS Valor,";
                    campoIdsPagto = "GROUP_CONCAT(DISTINCT(cp.IdPagto)) AS IdsPagto,";
                }

                if (agruparMes)
                {
                    camposAgruparMes = @", MONTH(IF(cp.IdChequePagto IS NOT NULL, ch.DataReceb, cp.DataPagto)) AS Mes,
                        YEAR(IF(cp.IdChequePagto IS NOT NULL, ch.DataReceb, cp.DataPagto)) AS Ano";
                }

                camposContaPaga = $@"p.IdConta,
                    p.Descricao AS PlanoConta,
                    g.Descricao AS GrupoConta,
                    cat.IdCategoriaConta,
                    cat.Descricao AS DescrCategoria,
                    { campoValor }
                    NULL AS TipoMov,
                    cp.DataVenc,
                    g.NumSeq AS NumSeqGrupo,
                    cat.NumSeq AS NumSeqCateg,
                    cat.Tipo As TipoCategoria,
                    NULL AS NomeCliente,
                    COALESCE(f.RazaoSocial, f.NomeFantasia) AS NomeFornecedor,
                    IF(cp.IdChequePagto IS NOT NULL, ch.DataReceb, cp.DataPagto) AS Data,
                    { campoIdsPagto }
                    NULL AS IdPagto,
                    cp.IdCompra,
                    NULL AS IdDeposito,
                    NULL AS IdPedido,
                    NULL AS IdAcerto,
                    NULL AS IdLiberarPedido,
                    CONVERT(CONCAT('[', COALESCE(cp.Obs, ''), '];[', COALESCE(pag.Obs, ''), '];[', COALESCE(imps.Obs, ''),']') USING UTF8) AS Obs
                    { camposAgruparMes }";
            }
            else if (sintetico)
            {
                camposContaPaga = "cp.IdConta, cp.ValorPago as Valor";
            }
            else
            {
                camposContaPaga = "COUNT(*) AS Cont";
            }

            sqlContaPaga = $@"SELECT { camposContaPaga } FROM contas_pagar cp
                    LEFT JOIN imposto_serv imps ON (cp.IdImpostoServ=imps.IdImpostoServ)
                    LEFT JOIN pagto pag ON (cp.IdPagto=pag.IdPagto)
                    LEFT JOIN plano_contas p ON (cp.IdConta=p.IdConta)
                    LEFT JOIN grupo_conta g ON (p.IdGrupo=g.IdGrupo)
                    LEFT JOIN compra comp ON (cp.IdCompra=comp.IdCompra)
                    LEFT JOIN cheques ch ON (cp.IdChequePagto=ch.IdCheque)
                    LEFT JOIN categoria_conta cat ON (g.IdCategoriaConta=cat.IdCategoriaConta)
                    LEFT JOIN fornecedor f ON (cp.IdFornec=f.IdFornec)
                WHERE cp.Paga=1
                    AND (Renegociada IS NULL OR Renegociada=0 OR cp.ValorPago=cp.ValorVenc)
                    AND cp.ValorPago > 0
                    AND (cp.IdChequePagto IS NULL OR ch.Situacao IN ({ (int)Model.Cheques.SituacaoCheque.Compensado },{ (int)Model.Cheques.SituacaoCheque.Quitado }))
                    { filtroGrupos }";

            if (tipoMov == 1)
            {
                filtro += " AND 0=1";
            }
            else
            {
                if (idCategoriaConta > 0)
                {
                    filtro += $" AND g.IdCategoriaConta = { idCategoriaConta }";
                }

                if (idGrupoConta > 0)
                {
                    filtro += $" AND g.IdGrupo = { idGrupoConta }";
                }

                if (idsPlanoConta.Any())
                {
                    filtro += $" AND p.IdConta IN ({ string.Join(",", idsPlanoConta) })";
                }

                if (idLoja > 0)
                {
                    filtro += $" AND cp.IdLoja = { idLoja }";
                }

                if (!string.IsNullOrWhiteSpace(dataIni))
                {
                    filtro += " AND IF(cp.IdChequePagto IS NULL, cp.DataPagto >= ?dataIni, ch.DataReceb >= ?dataIni)";
                }

                if (!string.IsNullOrWhiteSpace(dataFim))
                {
                    filtro += " AND IF(cp.IdChequePagto IS NULL, cp.DataPagto <= ?dataFim, ch.DataReceb <= ?dataFim)";
                }

                switch (tipoConta)
                {
                    case 1:
                        filtro += " AND cp.Contabil = 1";
                        break;
                    case 2:
                        filtro += " AND (cp.Contabil = 0 OR cp.Contabil IS NULL)";
                        break;
                }
            }

            sqlContaPaga += filtro;

            if (agruparMes)
            {
                sqlContaPaga += " GROUP BY MONTH(IF(cp.IdChequePagto IS NOT NULL, ch.DataReceb, cp.DataPagto)), YEAR(IF(cp.IdChequePagto IS NOT NULL, ch.DataReceb, cp.DataPagto)), cp.IdConta";
            }
            else if (sintetico)
            {
                sqlContaPaga += " GROUP BY cp.IdConta";
            }

            return sqlContaPaga;
        }

        private string SqlGeral(uint idCategoriaConta, uint idGrupoConta, uint[] idsPlanoConta, uint idLoja, string dataIni, string dataFim, int tipoMov, int tipoConta, bool ajustado,
            bool exibirChequeDevolvido, bool agruparMes, bool filtro, bool sintetico, bool detalhado, bool groupBySeparado, bool aumentarLimiteGroupConcat, bool selecionar)
        {
            var filtroGrupos = string.Empty;
            var campoGeralAnalitico = string.Empty;
            var campoGeralSintetico = string.Empty;
            var limiteGroupConcat = string.Empty;
            var campoIdPagto = string.Empty;
            var camposVenc = SqlCamposVenc(dataIni, dataFim, ajustado, filtro, agruparMes);
            var campoAgruparMes = agruparMes ? ", Mes, Ano" : string.Empty;
            var idsContaChequeDevolvido = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ChequeDevolvido);
            var idsContaCheque = $"{ UtilsPlanoConta.ContasChequeDev() },{ UtilsPlanoConta.ListaEstornosChequeDev() }, { idsContaChequeDevolvido }";
            var grupoSubtotal = $@", (SELECT Descricao FROM categoria_conta
                WHERE NumSeq > plano_contas.NumSeqCateg AND Tipo={ (int)Model.TipoCategoriaConta.Subtotal }
                ORDER BY NumSeq LIMIT 1) AS GrupoSubtotal,

                (SELECT Descricao FROM categoria_conta
                WHERE NumSeq > plano_contas.NumSeqCateg AND Tipo={ (int)Model.TipoCategoriaConta.SubtotalAgregado }
                ORDER BY NumSeq LIMIT 1) AS GrupoSubtotalAgregado";

            if (detalhado)
            {
                campoIdPagto = "IdPagto, CAST(IdPagto AS CHAR) AS IdsPagto";
            }
            else
            {
                campoIdPagto = "NULL AS IdPagto, GROUP_CONCAT(DISTINCT(IdPagto)) AS IdsPagto";
            }

            if (exibirChequeDevolvido)
            {
                filtroGrupos = $" AND (p.ExibirDre OR p.IdConta IN ({ idsContaChequeDevolvido }))";
            }
            else
            {
                filtroGrupos = $" AND p.ExibirDre AND p.IdConta NOT IN ({ idsContaCheque })";
            }

            if (selecionar)
            {
                campoGeralSintetico = $@"IdConta, PlanoConta, GrupoConta, NumSeqGrupo, DescrCategoria, NumSeqCateg, TipoCategoria,
                    CAST(IF(SUM(Valor * IF(TipoMov=1, 1, -1)) < 0, 2, 1) AS SIGNED) AS TipoMov,
                    CAST(ABS(SUM(Valor * IF(TipoMov=1, 1, -1))) AS DECIMAL(12,2)) AS Valor, IdsPagto{ camposVenc }{ grupoSubtotal }{ campoAgruparMes }";

                campoGeralAnalitico = $@"IdConta, PlanoConta, GrupoConta, NumSeqGrupo, DescrCategoria, NumSeqCateg, TipoCategoria, NomeCliente, NomeFornecedor, TipoMov,
                    CAST(Valor AS DECIMAL(12,2)) AS Valor, Data, IdCompra, { campoIdPagto }, IdDeposito, IdPedido, IdAcerto, IdLiberarPedido, Obs{ camposVenc }{ grupoSubtotal }";
            }
            else
            {
                campoGeralSintetico = $"IdConta, CAST(ABS(SUM(Valor)) AS DECIMAL(12,2)) AS Valor{ camposVenc }";
                campoGeralAnalitico = "SUM(Cont)";
            }

            var sqlGeral =
                $@"SELECT { (sintetico ? campoGeralSintetico : campoGeralAnalitico) } FROM (

                    ({ ObterSqlGeralCaixaDiario((int)idCategoriaConta, (int)idGrupoConta, idsPlanoConta?.Where(f => f > 0)?.Select(f => (int)f)?.ToArray(), (int)idLoja, dataIni, dataFim, tipoMov,
                        tipoConta, filtroGrupos, detalhado, sintetico, agruparMes, selecionar) })

                    UNION ALL

                    ({ ObterSqlGeralContaPaga((int)idCategoriaConta, (int)idGrupoConta, idsPlanoConta?.Where(f => f > 0).Select(f => (int)f)?.ToArray(), (int)idLoja, dataIni, dataFim, tipoMov,
                        tipoConta, filtroGrupos, detalhado, sintetico, agruparMes, selecionar) })

                    UNION ALL

                    ({ ObterSqlGeralCaixaGeral((int)idCategoriaConta, (int)idGrupoConta, idsPlanoConta?.Where(f => f > 0).Select(f => (int)f)?.ToArray(), (int)idLoja, dataIni, dataFim, tipoMov,
                        tipoConta, filtroGrupos, detalhado, sintetico, agruparMes, selecionar) })

                    UNION ALL

                    ({ ObterSqlGeralContaBanco((int)idCategoriaConta, (int)idGrupoConta, idsPlanoConta?.Where(f => f > 0).Select(f => (int)f)?.ToArray(), (int)idLoja, dataIni, dataFim, tipoMov,
                        tipoConta, filtroGrupos, detalhado, sintetico, agruparMes, selecionar) })

                    { (ajustado && !agruparMes ?
                        ObterSqlGeralPlanoConta((int)idCategoriaConta, (int)idGrupoConta, idsPlanoConta?.Where(f => f > 0).Select(f => (int)f)?.ToArray(), (int)idLoja, dataIni, dataFim, tipoMov,
                            tipoConta, filtroGrupos, detalhado, sintetico, ajustado, agruparMes, selecionar) : string.Empty) }";

            var whereMes = string.Empty;

            if (agruparMes)
            {
                var mesInicio = DateTime.Parse(dataIni).Month;
                var anoInicio = DateTime.Parse(dataIni).Year;
                var mesFim = DateTime.Parse(dataFim).Month;
                var anoFim = DateTime.Parse(dataFim).Year;

                whereMes = $" WHERE ((Mes >= { mesInicio } AND Ano = { anoInicio }) OR Ano > { anoInicio }) AND ((Mes <= { mesFim } AND Ano={ anoFim }) OR Ano < { anoFim })";
            }

            sqlGeral += $@") AS plano_contas{ (filtro ? "1" : string.Empty) }{ whereMes }
                { (sintetico ? (string.Format(" GROUP {0}BY IdConta", groupBySeparado ? " " : string.Empty)) + campoAgruparMes : string.Empty) }";

            if (!filtro && sintetico)
            {
                if (!selecionar)
                {
                    sqlGeral = $"SELECT COUNT(*) FROM ({ sqlGeral }) AS temp";
                }
            }

            if (!detalhado && aumentarLimiteGroupConcat)
            {
                limiteGroupConcat = "SET GROUP_CONCAT_MAX_LEN = 4096;";
            }

            return $"{ limiteGroupConcat }{ sqlGeral }";
        }

        #endregion

        #region Parâmetros

        private GDAParameter[] GetParams(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Listagem padrão

        /// <summary>
        /// Chamado 48448.
        /// Ajusta o valor das movimentações que foram pagas com crédito.
        /// O valor de crédito utilizado em pagamentos não deve ser exibido no DRE, pois,
        /// já existe uma movimentação no DRE referente à geração do crédito.
        /// </summary>
        public void AjustarValorMovimentacaoPagaComCreditoFornecedor(ref List<PlanoContas> movimentacoes, bool detalhado)
        {
            var retorno = new List<PlanoContas>();
            var idPagtoEPercentualReducaoCredito = new Dictionary<int, decimal>();

            #region Pagamento

            // Percorre todas as movimentações a serem exibidas no DRE para retirar os valores de crédito de fornecedor.
            foreach (var movimentacao in movimentacoes.Where(f => !string.IsNullOrWhiteSpace(f.IdsPagto)).ToList())
            {
                if (string.IsNullOrWhiteSpace(movimentacao.IdsPagto))
                    continue;

                foreach (var idPagto in movimentacao.IdsPagto.Split(',').Select(f => f.StrParaInt()))
                {
                    if (idPagto == 0)
                        continue;

                    // Recupera todos os pagamentos do pagamento.
                    var pagtosPagto = PagtoPagtoDAO.Instance.GetByPagto((uint)idPagto);
                    var valorPagtoCredito = new decimal();
                    var valorPagtoTotal = new decimal();
                    var valorPagtoTotalPorPlanoConta = new decimal();
                    var valorCreditoUtilizado = new decimal();

                    // Verifica se existe alguma forma de pagamento Crédito, associada ao pagamento.
                    // Caso o pagamento tenha sido efetuado totalmente com crédito o valor da conta ficará zerado.
                    // É muito importante que ele fique zerado ao invés de não ser exibido, pois, métodos de tela utilizam
                    // esse ajuste e, na tela, a quantidade exata do page size deve ser exibida.
                    if (pagtosPagto.Any(f => f.IdFormaPagto == (uint)Model.Pagto.FormaPagto.Credito))
                    {
                        // Recupera o valor pago em crédito e o valor total pago.
                        valorPagtoCredito = pagtosPagto.Where(f => f.IdFormaPagto == (uint)Model.Pagto.FormaPagto.Credito).Sum(f => f.ValorPagto);
                        valorPagtoTotal = pagtosPagto.Sum(f => f.ValorPagto);

                        // Calcula o total pago por plano de conta do pagto, considerando o plano de conta agrupado (se não for relatório detalhado)
                        if (!detalhado && !string.IsNullOrEmpty(movimentacao.IdsPagto))
                            valorPagtoTotalPorPlanoConta += ContasPagarDAO.Instance.GetByPagto(null, (uint)idPagto).Where(f => f.IdConta == movimentacao.IdConta).Sum(f => f.ValorPago);

                        // Verifica se o pagamento da movimentação já foi adicionado ao dicionário com o percentual de redução.
                        if (!idPagtoEPercentualReducaoCredito.ContainsKey(idPagto))
                            // Salva em um dicionário o ID do pagamento e o percentual a ser reduzido da movimentação.
                            // É interessante salvar estes dados para não ter que fazer todo esse procedimento em cada movimentação do mesmo pagamento.
                            idPagtoEPercentualReducaoCredito.Add(idPagto, valorPagtoCredito / valorPagtoTotal);
                    }
                    else
                        continue;

                    // Calcula o valor da movimentação com base no percentual de crédito de forncedor a ser deduzido.
                    if (detalhado)
                        valorCreditoUtilizado = movimentacao.Valor * idPagtoEPercentualReducaoCredito[idPagto];
                    else
                        valorCreditoUtilizado = valorPagtoTotal * idPagtoEPercentualReducaoCredito[idPagto];

                    // Soma o valor de crédito utilizado para que possa ser exibido no DRE não detalhado.
                    movimentacao.ValorCreditoUtilizado += valorCreditoUtilizado;

                    if (valorCreditoUtilizado > 0)
                    {
                        // Calcula quanto do crédito usado será reduzido de cada movimentação, pois o valor de crédito já entrou no DRE através da movimentação que o gerou.
                        var percReducaoCreditoUsado = valorPagtoCredito / valorPagtoTotal;

                        // Subtrai o crédito utilizado do valor da movimentação, pois o valor de crédito já entrou no DRE através da movimentação que o gerou.
                        if (detalhado)
                            movimentacao.Valor -= movimentacao.Valor * percReducaoCreditoUsado;
                        else // Subtrai o crédito utilizado do valor total pago, pois o valor de crédito já entrou no DRE através da movimentação que o gerou.
                            movimentacao.Valor -= (valorPagtoTotalPorPlanoConta > 0 ? valorPagtoTotalPorPlanoConta : valorPagtoTotal) * percReducaoCreditoUsado;
                    }
                }
            }

            #endregion
        }

        public IList<PlanoContas> GetList(uint idCategoriaConta, uint idGrupoConta, uint[] idsPlanoConta, uint idLoja, string dataIni,
            string dataFim, int tipoMov, int tipoConta, bool ajustado, bool exibirChequeDevolvido, int ordenar, string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? (ordenar == 1 ? "Data" : "NumSeqCateg, GrupoConta, PlanoConta") : sortExpression;

            var movimentacoes = LoadDataWithSortExpression(SqlGeral(idCategoriaConta, idGrupoConta, idsPlanoConta, idLoja, dataIni, dataFim,
             tipoMov, tipoConta, ajustado, exibirChequeDevolvido, false, false, true,
             false, false, true, true), sort, startRow, pageSize, GetParams(dataIni, dataFim)).ToList();

            AjustarValorMovimentacaoPagaComCreditoFornecedor(ref movimentacoes, false);

            return movimentacoes;
        }

        public int GetCount(uint idCategoriaConta, uint idGrupoConta, uint[] idsPlanoConta, uint idLoja, string dataIni, string dataFim,
            int tipoMov, int tipoConta, bool ajustado, bool exibirChequeDevolvido, int ordenar)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlGeral(idCategoriaConta, idGrupoConta, idsPlanoConta, idLoja, dataIni, dataFim,
                tipoMov, tipoConta, ajustado, exibirChequeDevolvido, false, false, true, false, false, true, false), GetParams(dataIni, dataFim));
        }

        public string SqlDreCompetencia(int? idLoja, string dataIni, string dataFim, int? idCategoriaConta, int? idGrupoConta,
            uint[] idsPlanoConta, bool detalhado, bool selecionar)
        {

            var campos = $@"lp.DataLiberacao AS Data, COALESCE(cr.idconta,COALESCE(cr1.IdConta, cr2.IDCONTA), cr3.IDCONTA) AS IdConta,
                                        p.TOTAL AS Valor, cli.Nome as NomeCliente, NULL as NomeFornecedor, 1 AS TipoMov";


            var sql = "Select " + campos + @" From pedido p
                        INNER JOIN produtos_liberar_pedido plp ON (p.IdPedido = plp.IdPedido)
                        INNER JOIN liberarpedido lp ON (plp.IDLIBERARPEDIDO = lp.IDLIBERARPEDIDO)
                        LEFT JOIN contas_receber cr ON (cr.IDLIBERARPEDIDO = lp.IDLIBERARPEDIDO)
                        LEFT JOIN contas_receber cr1 ON (cr1.IDSINAL = p.IDSINAL)
                        LEFT JOIN contas_receber cr2 ON (cr2.IDSINAL = p.IDPAGAMENTOANTECIPADO)
                        LEFT JOIN contas_receber cr3 ON (cr3.IDOBRA	 = p.IDOBRA)
                        LEFT JOIN cliente cli ON (lp.IdCliente = cli.Id_Cli)
                    WHERE lp.SITUACAO =" + (int)Model.LiberarPedido.SituacaoLiberarPedido.Liberado + "{0} GROUP BY p.IdPedido {1}";

            var camposUnion = $@" cp.DataCad AS Data, cp.IdConta,
                                 cp.ValorVenc AS Valor, NULL AS NomeCliente, Coalesce(f.RazaoSocial, f.NomeFantasia) as NomeFornecedor, 2 AS TipoMov";

            var unionAll = "UNION ALL Select" + camposUnion + @" FROM contas_pagar cp
                        LEFT JOIN fornecedor f ON (f.IdFornec = cp.IdFornec)
                    WHERE 1 {0}";

            var filtro = string.Empty;
            var filtroUnion = string.Empty;

            if (idLoja > 0)
            {
                filtro += $" AND cr.IdLoja={idLoja}";
                filtroUnion += $" AND cp.IdLoja={idLoja}";
            }

            if (!string.IsNullOrWhiteSpace(dataIni))
            {
                filtro += $" AND lp.DATALIBERACAO >=?dataIni";
                filtroUnion += $" AND cp.DATACAD >=?dataIni";
            }

            if (!string.IsNullOrWhiteSpace(dataFim))
            {
                filtro += $" AND lp.DATALIBERACAO <=?dataFim";
                filtroUnion += $" AND cp.DATACAD <=?dataFim";
            }

            unionAll = string.Format(unionAll, filtroUnion);
            sql = string.Format(sql, filtro, unionAll);

            var campoValor = detalhado ? "Valor" : "Sum(Valor) AS Valor";

            var selecao = $"gc.Descricao AS GrupoConta, pc.Descricao AS PlanoConta, Data, {campoValor}, NomeCliente, NomeFornecedor, TipoMov";

            var sqlPrincipal = $@"Select {selecao} from ({sql}) temp
                                LEFT JOIN plano_contas pc ON (pc.IDCONTA = temp.IDCONTA)
                                LEFT JOIN grupo_conta gc ON (gc.IDGRUPO = pc.IDGRUPO)
                                LEFT JOIN categoria_conta cc ON (cc.IDCATEGORIACONTA = gc.IDCATEGORIACONTA) WHERE 1";

            var filtroGeral = string.Empty;

            if (idCategoriaConta > 0)
                filtroGeral += $" AND cc.IdCategoriaConta={idCategoriaConta}";

            if (idGrupoConta > 0)
                filtroGeral += $" AND gc.IdGrupo={idGrupoConta}";

            if (idsPlanoConta.Any())
                filtroGeral += $" AND pc.IdConta IN ({string.Join(",", idsPlanoConta)})";

            sqlPrincipal += filtroGeral;

            sqlPrincipal += detalhado ? "" : " GROUP BY temp.IdConta";

            return selecionar ? sqlPrincipal : $"Select COUNT(*) FROM ({sqlPrincipal}) temp2";
        }

        public IList<PlanoContas> PesquisarDreCompetencia(int? idLoja, string dataIni, string dataFim, int? idCategoriaConta, int? idGrupoConta,
            uint[] idsPlanoConta, bool detalhado, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(SqlDreCompetencia(idLoja, dataIni, dataFim, idCategoriaConta, idGrupoConta, idsPlanoConta, detalhado, true),
                sortExpression, startRow, pageSize, GetParams(dataIni, dataFim)).ToList();
        }

        public int PesquisarDreCompetenciaCount(int? idLoja, string dataIni, string dataFim, int? idCategoriaConta, int? idGrupoConta, uint[] idsPlanoConta, bool detalhado)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlDreCompetencia(idLoja, dataIni, dataFim, idCategoriaConta, idGrupoConta, idsPlanoConta, detalhado, false), GetParams(dataIni, dataFim));
        }

        public PlanoContas[] PesquisarDreCompetenciaRpt(int? idLoja, string dataIni, string dataFim, int? idCategoriaConta, int? idGrupoConta, uint[] idsPlanoConta, bool detalhado)
        {
            List<PlanoContas> lstPlanoConta = objPersistence.LoadData(SqlDreCompetencia(idLoja, dataIni, dataFim, idCategoriaConta, idGrupoConta, idsPlanoConta, detalhado, true), GetParams(dataIni, dataFim)).ToList();

            string criterio = String.Empty;

            if (idGrupoConta > 0)
                criterio += "Grupo: " + GrupoContaDAO.Instance.ObtemValorCampo<string>("descricao", "idGrupo=" + idGrupoConta) + "    ";

            if (idsPlanoConta.Any())
            {
                criterio += "Planos Conta: " +
                    PlanoContasDAO.Instance.ExecuteScalar<string>("select group_concat(descricao SEPARATOR ', ') from plano_contas where idConta IN (" + string.Join(",", idsPlanoConta) + ")") + "    ";
            }

            if (idLoja > 0)
                criterio += "Loja: " + LojaDAO.Instance.GetNome((uint)idLoja) + "    ";

            if (!String.IsNullOrEmpty(dataIni))
                criterio += "Data Início: " + dataIni + "    ";

            if (!String.IsNullOrEmpty(dataFim))
                criterio += "Data Fim: " + dataFim + "    ";

            return lstPlanoConta.ToArray();
        }

        #endregion

        #region Listagem padrão detalhes

        public IList<PlanoContas> GetListDetalhes(uint idCategoriaConta, uint idGrupoConta, uint[] idsPlanoConta, uint idLoja,
            string dataIni, string dataFim, int tipoMov, int tipoConta, bool ajustado, bool exibirChequeDevolvido, int ordenar, string sortExpression, int startRow, int pageSize)
        {
            string sort = string.IsNullOrEmpty(sortExpression) ? (ordenar == 1 ? "Data" : "NumSeqCateg, GrupoConta, PlanoConta") : sortExpression;

            var movimentacoes = LoadDataWithSortExpression(SqlGeral(idCategoriaConta, idGrupoConta, idsPlanoConta, idLoja, dataIni, dataFim,
                tipoMov, tipoConta, ajustado, exibirChequeDevolvido, false, false, false, true, false, true, true), sort, startRow, pageSize, GetParams(dataIni, dataFim)).ToList();

            AjustarValorMovimentacaoPagaComCreditoFornecedor(ref movimentacoes, true);

            return movimentacoes;
        }

        public int GetDetalhesCount(uint idCategoriaConta, uint idGrupoConta, uint[] idsPlanoConta, uint idLoja, string dataIni,
            string dataFim, int tipoMov, int tipoConta, bool ajustado, bool exibirChequeDevolvido, int ordenar)
        {
            return objPersistence.ExecuteSqlQueryCount(
                SqlGeral(idCategoriaConta, idGrupoConta, idsPlanoConta, idLoja, dataIni, dataFim,
                tipoMov, tipoConta, ajustado, exibirChequeDevolvido, false, false, false, true, false, true, false), GetParams(dataIni, dataFim));
        }

        #endregion

        #region Busca para relatório

        private class ChaveRpt
        {
            public string Categoria;
            public string Grupo;
            public long Mes;
            public long Ano;
            public long Direcao;
            public int NumSeqCateg;

            public override bool Equals(object obj)
            {
                if (!(obj is ChaveRpt))
                    return false;

                ChaveRpt comp = (ChaveRpt)obj;
                return Categoria == comp.Categoria && Grupo == comp.Grupo && Mes == comp.Mes &&
                    Ano == comp.Ano && Direcao == comp.Direcao && NumSeqCateg == comp.NumSeqCateg;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }
        }

        public PlanoContas[] GetForRpt(uint idCategoriaConta, uint idGrupoConta, uint[] idsPlanoConta, uint idLoja, string dataIni,
            string dataFim, int tipoMov, int tipoConta, bool ajustado, bool exibirChequeDevolvido, bool agruparMes, int ordenar)
        {
            var sort = " ORDER BY " + (ordenar == 1 ? "Data" : "NumSeqCateg, NumSeqGrupo, GrupoConta, PlanoConta");

            List<PlanoContas> lstPlanoConta = objPersistence.LoadData(SqlGeral(idCategoriaConta, idGrupoConta, idsPlanoConta, idLoja,
                dataIni, dataFim, tipoMov, tipoConta, ajustado, exibirChequeDevolvido, agruparMes, false, true,
                false, false, true, true) + sort, GetParams(dataIni, dataFim));

            AjustarValorMovimentacaoPagaComCreditoFornecedor(ref lstPlanoConta, false);

            string criterio = String.Empty;

            if (idGrupoConta > 0)
                criterio += "Grupo: " + GrupoContaDAO.Instance.ObtemValorCampo<string>("descricao", "idGrupo=" + idGrupoConta) + "    ";

            if (idsPlanoConta.Any())
            {
                criterio += "Planos Conta: " +
                    PlanoContasDAO.Instance.ExecuteScalar<string>("select group_concat(descricao SEPARATOR ', ') from plano_contas where idConta IN (" + string.Join(",", idsPlanoConta) + ")");
            }

            if (idLoja > 0)
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";

            if (!String.IsNullOrEmpty(dataIni))
                criterio += "Data Início: " + dataIni + "    ";

            if (!String.IsNullOrEmpty(dataFim))
                criterio += "Data Fim: " + dataFim + "    ";

            switch (tipoMov)
            {
                case 1:
                    criterio += "Apenas movimentações de entrada    ";
                    break;
                case 2:
                    criterio += "Apenas movimentações de saída    ";
                    break;
            }

            switch (tipoConta)
            {
                case 1:
                    criterio += "Apenas contas contábeis    ";
                    break;
                case 2:
                    criterio += "Apenas contas não-contábeis    ";
                    break;
            }

            if (lstPlanoConta.Count > 0)
                lstPlanoConta[0].Criterio = criterio;

            if (agruparMes)
            {
                List<ChaveRpt> categorias = new List<ChaveRpt>();
                List<ChaveRpt> gruposConta = new List<ChaveRpt>();

                foreach (PlanoContas p in lstPlanoConta)
                {
                    ChaveRpt chave = new ChaveRpt();
                    chave.Categoria = p.DescrCategoria;
                    chave.Mes = p.Mes;
                    chave.Ano = p.Ano;
                    chave.Direcao = p.TipoMov;
                    chave.NumSeqCateg = p.NumSeqCateg;

                    if (!categorias.Contains(chave))
                        categorias.Add(chave);

                    chave = new ChaveRpt();
                    chave.Categoria = p.DescrCategoria;
                    chave.Mes = p.Mes;
                    chave.Ano = p.Ano;
                    chave.Grupo = p.GrupoConta;
                    chave.Direcao = p.TipoMov;
                    chave.NumSeqCateg = p.NumSeqCateg;

                    if (!gruposConta.Contains(chave))
                        gruposConta.Add(chave);
                }

                foreach (ChaveRpt c in categorias)
                {
                    PlanoContas novo = new PlanoContas();
                    novo.DescrCategoria = c.Categoria;
                    novo.Mes = c.Mes;
                    novo.Ano = c.Ano;
                    novo.TipoMov = c.Direcao;
                    novo.DescricaoCategoria = true;
                    novo.NumSeqCateg = c.NumSeqCateg;

                    lstPlanoConta.Add(novo);
                }

                foreach (ChaveRpt c in gruposConta)
                {
                    PlanoContas novo = new PlanoContas();
                    novo.DescrCategoria = c.Categoria;
                    novo.Mes = c.Mes;
                    novo.Ano = c.Ano;
                    novo.TipoMov = c.Direcao;
                    novo.GrupoConta = c.Grupo;
                    novo.DescricaoGrupo = true;
                    novo.NumSeqCateg = c.NumSeqCateg;

                    lstPlanoConta.Add(novo);
                }
            }

            return lstPlanoConta.ToArray();
        }

        public PlanoContas[] GetForRptDetalhes(uint idCategoriaConta, uint idGrupoConta, List<uint> idsPlanoConta , uint idLoja,
            string dataIni, string dataFim, int tipoMov, int tipoConta, bool ajustado, bool exibirChequeDevolvido, int ordenar)
        {
            var sort = " ORDER BY " + (ordenar == 1 ? "Data" : "NumSeqCateg, NumSeqGrupo, GrupoConta, PlanoConta");

            List<PlanoContas> lstPlanoConta = objPersistence.LoadData(SqlGeral(idCategoriaConta, idGrupoConta, idsPlanoConta.ToArray(), idLoja,
                dataIni, dataFim, tipoMov, tipoConta, ajustado, exibirChequeDevolvido, false, false, false, true, true, false, true) + sort, GetParams(dataIni, dataFim));

            AjustarValorMovimentacaoPagaComCreditoFornecedor(ref lstPlanoConta, true);

            string criterio = String.Empty;

            if (idGrupoConta > 0)
                criterio += "Grupo: " + GrupoContaDAO.Instance.ObtemValorCampo<string>("descricao", "idGrupo=" + idGrupoConta) + "    ";

            if (idsPlanoConta?.Any(f => f > 0) ?? false)
            {
                criterio += string.Format("Plano(s) Conta: {0}    ", string.Join(", ", ExecuteMultipleScalar<string>(string.Format("SELECT descricao FROM plano_contas WHERE idConta IN ({0})", string.Join(",", idsPlanoConta)))));
            }

            if (idLoja > 0)
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";

            if (!String.IsNullOrEmpty(dataIni))
                criterio += "Data Início: " + dataIni + "    ";

            if (!String.IsNullOrEmpty(dataFim))
                criterio += "Data Fim: " + dataFim + "    ";

            if (lstPlanoConta.Count > 0)
                lstPlanoConta[0].Criterio = criterio;

            return lstPlanoConta.ToArray();
        }

        #endregion
    }
}
