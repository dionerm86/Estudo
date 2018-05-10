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

        private string SqlGeral(uint idCategoriaConta, uint idGrupoConta, uint idPlanoConta, uint idLoja, string dataIni, string dataFim,
            int tipoMov, int tipoConta, bool ajustado, bool exibirChequeDevolvido, bool agruparMes, bool filtro, bool sintetico,
            bool detalhado, bool selecionar)
        {
            return SqlGeral(idCategoriaConta, idGrupoConta, idPlanoConta, idLoja, dataIni, dataFim,
                tipoMov, tipoConta, ajustado, exibirChequeDevolvido, agruparMes, filtro, sintetico,
                detalhado, false, selecionar);
        }

        private string SqlGeral(uint idCategoriaConta, uint idGrupoConta, uint idPlanoConta, uint idLoja, string dataIni, string dataFim,
          int tipoMov, int tipoConta, bool ajustado, bool exibirChequeDevolvido, bool agruparMes, bool filtro, bool sintetico,
          bool detalhado, bool groupBySeparado, bool selecionar)
        {
            return SqlGeral(idCategoriaConta, idGrupoConta, idPlanoConta, idLoja, dataIni, dataFim,
                tipoMov, tipoConta, ajustado, exibirChequeDevolvido, agruparMes, filtro, sintetico,
                detalhado, groupBySeparado, true, selecionar);
        }

        private string SqlGeral(uint idCategoriaConta, uint idGrupoConta, uint idPlanoConta, uint idLoja, string dataIni, string dataFim,
            int tipoMov, int tipoConta, bool ajustado, bool exibirChequeDevolvido, bool agruparMes, bool filtro, bool sintetico,
            bool detalhado, bool groupBySeparado, bool aumentarLimiteGroupConcat, bool selecionar)
        {
            return SqlGeral(idCategoriaConta, idGrupoConta, new[] { idPlanoConta }, idLoja, dataIni, dataFim,
                tipoMov, tipoConta, ajustado, exibirChequeDevolvido, agruparMes, filtro, sintetico,
                detalhado, groupBySeparado, aumentarLimiteGroupConcat, selecionar);
        }

        private string SqlGeral(uint idCategoriaConta, uint idGrupoConta, uint[] idsPlanoConta, uint idLoja, string dataIni, string dataFim,
            int tipoMov, int tipoConta, bool ajustado, bool exibirChequeDevolvido, bool agruparMes, bool filtro, bool sintetico,
            bool detalhado, bool groupBySeparado, bool aumentarLimiteGroupConcat, bool selecionar)
        {
            // Define que o juros será subtraído das movimentações
            bool subtrairJuros = Configuracoes.FinanceiroConfig.SubtrairJurosDRE;

            var campoValorCxDiario =
                string.Format("{0}{1}",
                    !detalhado ? "ABS(SUM(c.Valor * IF(c.TipoMov = 1, 1, -1)))" : "c.Valor", subtrairJuros ? "-c.Juros" : string.Empty);
            var campoValorCxGeral =
                string.Format("{0}{1}",
                    !detalhado ? "ABS(SUM(c.ValorMov * IF(c.TipoMov = 1, 1, -1)))" : "c.ValorMov", subtrairJuros ? "-c.Juros" : string.Empty);
            
            // O plano de contas "Cheque Devolvido" está associado ao grupo de ID 5, porém o mesmo deve ser mostrado caso a variável "exibirChequeDevolvido" seja verdadeira,
            // por isso recupero as movimentações que não estão incluídas no grupo de ID 5 ou as que possuem referência do plano de conta "Cheque Devolvido".
            string filtroGrupos = " AND (p.ExibirDre" +
                (exibirChequeDevolvido ? " OR p.idConta In (" + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ChequeDevolvido) + "))" : ")");

            // Se não for para exibir cheques devolvidos, esconde os recebimentos dos mesmos
            if (!exibirChequeDevolvido)
                filtroGrupos += " And p.idConta not In (" + UtilsPlanoConta.ContasChequeDev() + "," + UtilsPlanoConta.ListaEstornosChequeDev() + "," +
                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ChequeDevolvido) + ")";

            string camposVenc = SqlCamposVenc(dataIni, dataFim, ajustado, filtro, agruparMes);
            var campoIdPagto = !detalhado ? "NULL AS IdPagto, GROUP_CONCAT(DISTINCT(IdPagto)) AS IdsPagto" : "IdPagto, CAST(IdPagto AS CHAR) AS IdsPagto";

            string grupoSubtotal = ", (select descricao from categoria_conta where numSeq > plano_contas.numSeqCateg and tipo=" +
                (int)Glass.Data.Model.TipoCategoriaConta.Subtotal + @" order by numSeq limit 1) as grupoSubtotal, 
                (select descricao from categoria_conta where numSeq > plano_contas.numSeqCateg and tipo=" +
                (int)Glass.Data.Model.TipoCategoriaConta.SubtotalAgregado + " order by numSeq limit 1) as grupoSubtotalAgregado";

            string campoGeralSintetico = selecionar ? "IdConta, PlanoConta, GrupoConta, NumSeqGrupo, DescrCategoria, NumSeqCateg, TipoCategoria, cast(if(Sum(Valor*if(tipoMov=1,1,-1))<0, 2, 1) as signed) as tipoMov, " +
                "cast(abs(Sum(Valor*if(tipoMov=1,1,-1))) as decimal(12,2)) as Valor, IdsPagto" + camposVenc + grupoSubtotal + (agruparMes ? ", Mes, Ano" : "") :
                "idConta, cast(abs(Sum(Valor)) as decimal(12,2)) as Valor" + camposVenc;

            string campoGeralAnalitico = selecionar ? @"IdConta, PlanoConta, GrupoConta, NumSeqGrupo, DescrCategoria, NumSeqCateg, TipoCategoria, NomeCliente, NomeFornec, TipoMov, 
                cast(Valor as decimal(12,2)) as Valor, Data, IdCompra, " + campoIdPagto + ", IdDeposito, IdPedido, IdAcerto, IdLiberarPedido, Obs" + camposVenc + grupoSubtotal : "Sum(cont)";

            string camposCxDiario = selecionar ? @"p.idConta, p.Descricao as PlanoConta, g.Descricao as GrupoConta, cat.idCategoriaConta, 
                cat.Descricao as DescrCategoria,g.numSeq as NumSeqGrupo, cat.numSeq as NumSeqCateg, cat.Tipo As TipoCategoria, cli.Nome as NomeCliente, null as NomeFornec, c.TipoMov, 
                " + campoValorCxDiario + @" as Valor, c.DataCad as Data, null as idCompra, null as idPagto, NULL AS IdsPagto, null as IdDeposito, c.idPedido, c.idAcerto,
                c.idLiberarPedido, null as DataVenc, Convert(c.Obs Using utf8) As Obs" + (agruparMes ? ", month(c.dataCad) as Mes, year(c.dataCad) as Ano" : "") :
                (sintetico ? "c.IdConta, abs(sum(c.Valor*if(c.tipoMov=1,1,-1))) as Valor" : "Count(*) as cont");

            var camposContasPagas =
                string.Format("{0}",
                    selecionar ?
                        string.Format(@"p.idConta, p.Descricao as PlanoConta, g.Descricao as GrupoConta, cat.idCategoriaConta, 
                            cat.Descricao as DescrCategoria,g.numSeq as NumSeqGrupo, cat.numSeq as NumSeqCateg, cat.Tipo As TipoCategoria,
                            null as NomeCliente, Coalesce(f.RazaoSocial, f.NomeFantasia) as NomeFornecedor, null as TipoMov,
                            {0} as Valor, if (cp.idChequePagto is not null, ch.dataReceb, cp.DataPagto) as Data, cp.IdCompra, {2},
                            null as IdDeposito, null as IdPedido, null as IdAcerto, null as IdLiberarPedido, cp.DataVenc,
                            Convert(Concat('[', Coalesce(cp.Obs, ''), '];[', Coalesce(pag.obs, ''), '];[', Coalesce(imps.Obs, ''),']') Using utf8) As Obs {1}",
                    !detalhado ? "SUM(cp.ValorPago)" : "cp.ValorPago",
                    agruparMes ? ", month(if (cp.idChequePagto is not null, ch.dataReceb, cp.DataPagto)) as Mes, year(if (cp.idChequePagto is not null, ch.dataReceb, cp.DataPagto)) as Ano" : "",
                    !detalhado ? "NULL AS IdPagto, GROUP_CONCAT(DISTINCT(cp.IdPagto)) AS IdsPagto" : "cp.IdPagto, CAST(cp.IdPagto AS CHAR) AS IdsPagto") :
                    sintetico ? "cp.IdConta, cp.ValorPago as Valor" : "Count(*) as cont");

            // O DATE_FORMAT é usado para não buscar movimentações repetidas do caixa geral com o banco, os juros não devem ser subtraídos do valor
            // pois o valor da movimentação já é o valor recebido em determinada forma de pagamento.
            var camposCxGeral = selecionar ? @"p.idConta, p.Descricao as PlanoConta, g.Descricao as GrupoConta, cat.idCategoriaConta, 
                cat.Descricao as DescrCategoria,g.numSeq as NumSeqGrupo, cat.numSeq as NumSeqCateg, cat.Tipo As TipoCategoria, cli.Nome as NomeCliente, Coalesce(f.RazaoSocial, f.NomeFantasia) as NomeFornecedor, 
                c.TipoMov, " + campoValorCxGeral + @" as Valor, Cast(Date_Format(Coalesce(c.DataMovBanco, c.DataMov), '%Y/%m/%d') as Date) as Data, c.idCompra, " +
                (!detalhado ? "NULL AS IdPagto, GROUP_CONCAT(DISTINCT(c.IdPagto)) AS IdsPagto" : "c.IdPagto, CAST(c.IdPagto AS CHAR) AS IdsPagto") +
                @", null as idDeposito, c.idPedido, c.idAcerto, c.idLiberarPedido, null as DataVenc, Convert(c.Obs Using utf8) As Obs" +
                (agruparMes ? ", month(Coalesce(c.dataMovBanco, c.dataMov)) as Mes, year(Coalesce(c.dataMovBanco, c.dataMov)) as Ano" : "") :
                (sintetico ? "c.IdConta, c.ValorMov*if(c.tipoMov=1,1,-1) as Valor" : "Count(*) as cont");

            var camposContaBanco =
                string.Format("{0}",
                    selecionar ?
                        string.Format(@"p.idConta, p.Descricao as PlanoConta, g.Descricao as GrupoConta, cat.idCategoriaConta, 
                            cat.Descricao as DescrCategoria,g.numSeq as NumSeqGrupo, cat.numSeq as NumSeqCateg, cat.Tipo As TipoCategoria,
                            cli.Nome as NomeCliente, Coalesce(f.RazaoSocial, f.NomeFantasia) as NomeFornecedor, m.TipoMov,
                            {0} as Valor, Cast(Date_Format(m.DataMov, '%Y/%m/%d') as Date) as Data, null as idCompra, " +
                            (!detalhado ? "NULL AS IdPagto, GROUP_CONCAT(DISTINCT(m.IdPagto)) AS IdsPagto" : "m.IdPagto, CAST(m.IdPagto AS CHAR) AS IdsPagto") +
                            @", m.idDeposito, m.idPedido, m.idAcerto, m.idLiberarPedido, null as DataVenc, Convert(m.Obs Using utf8) As Obs {1}",
                            !detalhado ? "ABS(SUM(m.ValorMov * IF(m.TipoMov = 1, 1, -1)))" : "m.ValorMov",
                            (agruparMes ? ", month(m.dataMov) as Mes, year(m.dataMov) as Ano" : "")) :
                        (sintetico ? "m.IdConta, m.ValorMov*if(m.tipoMov=1,1,-1) as Valor" : "Count(*) as cont"));

            var camposPlanoConta = selecionar ? @"p.idConta, p.Descricao as PlanoConta, g.Descricao as GrupoConta, cat.idCategoriaConta,
                cat.Descricao as DescrCategoria,g.numSeq as NumSeqGrupo, cat.numSeq as NumSeqCateg, cat.Tipo As TipoCategoria, null as NomeCliente, null as NomeFornecedor, 1 as TipoMov, 
                0 as Valor, null as Data, null as idCompra, null as idPagto, NULL AS IdsPagto, null as idDeposito, null as idPedido, null as idAcerto,
                null as idLiberarPedido, null as DataVenc, null as Obs" +
                (agruparMes ? ", null as Mes, null as Ano" : "") : "p.idConta, 0 as Valor";

            string sql =
                "Select " + (sintetico ? campoGeralSintetico : campoGeralAnalitico) + @" From (
                (Select " + camposCxDiario + @" From caixa_diario c 
                Left Join plano_contas p On (c.IdConta=p.IdConta)
                Left Join grupo_conta g On (p.IdGrupo=g.IdGrupo) 
                Left Join categoria_conta cat On (g.IdCategoriaConta=cat.IdCategoriaConta) 
                Left Join cliente cli On (c.idCliente=cli.id_Cli) "
                + GetCaixaDiarioFiltro(idCategoriaConta, idGrupoConta, idsPlanoConta, idLoja, dataIni, dataFim, tipoMov, tipoConta) + filtroGrupos + 
                (agruparMes ? "GROUP BY month(c.dataCad), year(c.dataCad), c.idConta" :
                sintetico ? " GROUP BY c.IdConta" : string.Empty) +
                @") union all
                
                (Select " + camposContasPagas + @" From contas_pagar cp 
                Left Join imposto_serv imps ON (cp.IdImpostoServ = imps.IdImpostoServ)
                Left Join pagto pag On (cp.idPagto=pag.idPagto)
                Left Join plano_contas p On (cp.IdConta=p.IdConta)
                Left Join grupo_conta g On (p.IdGrupo=g.IdGrupo)
                Left Join compra comp On (cp.IdCompra=comp.IdCompra) 
                Left Join cheques ch On (cp.IdChequePagto=ch.IdCheque) 
                Left Join categoria_conta cat On (g.IdCategoriaConta=cat.IdCategoriaConta) 
                Left Join fornecedor f On (cp.idFornec=f.idFornec) "
                + GetContaPagaFiltro(idCategoriaConta, idGrupoConta, idsPlanoConta, idLoja, dataIni, dataFim, tipoMov, tipoConta) + filtroGrupos + 
                (agruparMes ? " GROUP BY month(if (cp.idChequePagto is not null, ch.dataReceb, cp.DataPagto)), year(if (cp.idChequePagto is not null, ch.dataReceb, cp.DataPagto)), cp.IdConta" :
                sintetico ? " GROUP BY cp.IdConta" : string.Empty) +
                @") union all

                (Select " + camposCxGeral + @" From caixa_geral c 
                Left Join plano_contas p On (c.IdConta=p.IdConta)
                Left Join grupo_conta g On (p.IdGrupo=g.IdGrupo)
                Left Join pedido ped On (c.IdPedido=ped.IdPedido)
                Left Join compra comp On (c.IdCompra=comp.IdCompra) 
                Left Join categoria_conta cat On (g.IdCategoriaConta=cat.IdCategoriaConta) 
                Left Join cliente cli On (c.idCliente=cli.id_Cli) 
                Left Join fornecedor f On (c.idFornec=f.idFornec) "
                + GetCaixaGeralFiltro(idCategoriaConta, idGrupoConta, idsPlanoConta, idLoja, dataIni, dataFim, tipoMov, tipoConta) + filtroGrupos + 
                (agruparMes ? " GROUP BY month(Coalesce(c.dataMovBanco, c.dataMov)), year(Coalesce(c.dataMovBanco, c.dataMov)), c.idConta" : 
                sintetico ? " GROUP BY c.IdConta" : string.Empty) +
                @") union all
                
                (Select " + camposContaBanco + @" From mov_banco m 
                Left Join funcionario func On (m.usuCad=func.idFunc)
                Left Join plano_contas p On (m.IdConta=p.IdConta)
                Left Join grupo_conta g On (p.IdGrupo=g.IdGrupo)
                Left Join pedido ped On (m.IdPedido=ped.IdPedido) 
                Left Join categoria_conta cat On (g.IdCategoriaConta=cat.IdCategoriaConta) 
                Left Join cliente cli On (m.idCliente=cli.id_Cli) 
                Left Join fornecedor f On (m.idFornec=f.idFornec) "
                + GetContaBancoFiltro(idCategoriaConta, idGrupoConta, idsPlanoConta, idLoja, dataIni, dataFim, tipoMov, tipoConta) + filtroGrupos + 
                (agruparMes ? " GROUP BY month(m.dataMov), year(m.dataMov), m.idConta" :
                sintetico ? " GROUP BY m.IdConta" : string.Empty) +
                ")";

            // O union all do caixa_geral com mov_banco deve ser usado, pois faz com que os boletos, que são gerados nos dois,
            // apareça no DRE

            if (ajustado && !agruparMes)
            {
                sql += @" union all
                    (Select " + camposPlanoConta + @" From plano_contas p
                    Left Join grupo_conta g On (p.IdGrupo=g.IdGrupo)
                    Left Join categoria_conta cat On (g.IdCategoriaConta=cat.IdCategoriaConta) "
                    + GetPlanoContaFiltro(idCategoriaConta, idGrupoConta, idsPlanoConta, idLoja, dataIni, dataFim, tipoMov, tipoConta) + filtroGrupos +
                    @" Group  By p.idConta)";
            }

            string whereMes = "";
            if (agruparMes)
            {
                int mesInicio = DateTime.Parse(dataIni).Month;
                int anoInicio = DateTime.Parse(dataIni).Year;
                int mesFim = DateTime.Parse(dataFim).Month;
                int anoFim = DateTime.Parse(dataFim).Year;

                whereMes = " Where ((Mes>=" + mesInicio + " And Ano=" + anoInicio + ") Or Ano>" + anoInicio + ") And ((Mes<=" +
                    mesFim + " And Ano=" + anoFim + ") Or Ano<" + anoFim + ")";
            }

            sql += ") as plano_contas" + (filtro ? "1" : "") + whereMes + (sintetico ? (string.Format(" Group {0}By IdConta", groupBySeparado ? " " : string.Empty)) + (agruparMes ? ", Mes, Ano" : "") : "");

            if (!filtro && sintetico)
            {
                // Chamado 25058: O sql abaixo foi comentado para que sejam buscadas corretamentes os plano de conta no DRE
                //if (!String.IsNullOrEmpty(camposVenc))
                //    sql += " Having VencPeriodoNaoPagas>0 Or VencPassadoPagasPeriodo>0 Or Valor>0";

                if (!selecionar)
                    sql = "select count(*) from (" + sql + ") as temp";
            }

            //Validação para a chamada do metodo GetPlanoContaFiltro, para não gerar duplicidade do SET GROUP_CONCAT_MAX_LEN
            return string.Format("{0}{1}", !detalhado && aumentarLimiteGroupConcat ? "SET GROUP_CONCAT_MAX_LEN = 4096;" : string.Empty, sql);
        }

        #endregion

        #region Filtros

        private string GetCaixaDiarioFiltro(uint idCategoriaConta, uint idGrupoConta, uint[] idsPlanoConta, uint idLoja, string dataIni, string dataFim, int tipoMov, int tipoConta)
        {
            string where = "Where c.idConta Not In (" + UtilsPlanoConta.PlanosContaDesconsiderarCxGeral + ")";

            if (tipoConta == 1)
                where += " and 0";
            else
            {
                if (idCategoriaConta > 0)
                    where += " And g.idCategoriaConta=" + idCategoriaConta;

                if (idGrupoConta > 0)
                    where += " And g.IdGrupo=" + idGrupoConta;

                if (idsPlanoConta.Any())
                    where += " And p.IdConta IN (" + string.Join(",", idsPlanoConta) + ")";

                if (idLoja > 0)
                    where += " And c.idLoja=" + idLoja;

                if (!String.IsNullOrEmpty(dataIni))
                    where += " And c.DataCad>=?dataIni";

                if (!String.IsNullOrEmpty(dataFim))
                    where += " And c.DataCad<=?dataFim";

                if (tipoMov > 0)
                    where += " and c.tipoMov=" + tipoMov;
            }

            return where;
        }

        private string GetCaixaGeralFiltro(uint idCategoriaConta, uint idGrupoConta, uint[] idsPlanoConta, uint idLoja, string dataIni, string dataFim, int tipoMov, int tipoConta)
        {
            string where = "Where c.idConta Not In (" + UtilsPlanoConta.PlanosContaDesconsiderarCxGeral + ")";

            if (tipoConta == 1)
                where += " and 0";
            else
            {
                if (idCategoriaConta > 0)
                    where += " And g.idCategoriaConta=" + idCategoriaConta;

                if (idGrupoConta > 0)
                    where += " And g.IdGrupo=" + idGrupoConta;

                if (idsPlanoConta.Any())
                    where += " And p.IdConta IN (" + string.Join(",", idsPlanoConta) + ")";

                if (idLoja > 0)
                    where += " And c.IdLoja=" + idLoja;

                if (!String.IsNullOrEmpty(dataIni))
                    where += " And Coalesce(c.DataMovBanco, c.DataMov)>=?dataIni";

                if (!String.IsNullOrEmpty(dataFim))
                    where += " And Coalesce(c.DataMovBanco, c.DataMov)<=?dataFim";

                if (tipoMov > 0)
                    where += " and c.tipoMov=" + tipoMov;
            }

            return where;
        }

        private string GetContaBancoFiltro(uint idCategoriaConta, uint idGrupoConta, uint[] idsPlanoConta, uint idLoja, string dataIni, string dataFim, int tipoMov, int tipoConta)
        {
            string where = "Where 1 ";

            if (tipoConta == 1)
                where += " and 0";
            else
            {
                if (idCategoriaConta > 0)
                    where += " And g.idCategoriaConta=" + idCategoriaConta;

                if (idGrupoConta > 0)
                    where += " And g.IdGrupo=" + idGrupoConta;

                if (idsPlanoConta.Any())
                    where += " And p.IdConta IN (" + string.Join(",", idsPlanoConta) + ")";

                if (idLoja > 0)
                    where += " And func.IdLoja=" + idLoja;

                if (!String.IsNullOrEmpty(dataIni))
                    where += " And m.DataMov>=?dataIni";

                if (!String.IsNullOrEmpty(dataFim))
                    where += " And m.DataMov<=?dataFim";

                if (tipoMov > 0)
                    where += " and m.tipoMov=" + tipoMov;
            }

            return where;
        }

        private string GetContaPagaFiltro(uint idCategoriaConta, uint idGrupoConta, uint[] idsPlanoConta, uint idLoja, string dataIni, string dataFim, int tipoMov, int tipoConta)
        {
            string where = @"Where cp.paga=true And (Renegociada IS NULL OR Renegociada=FALSE OR cp.ValorPago=cp.ValorVenc) And cp.valorPago > 0 And 
                (cp.idChequePagto is null Or ch.situacao In (" + (int)Glass.Data.Model.Cheques.SituacaoCheque.Compensado + 
                "," + (int)Glass.Data.Model.Cheques.SituacaoCheque.Quitado + "))";

            if (tipoMov == 1)
                where += " And 0";
            else
            {
                if (idCategoriaConta > 0)
                    where += " And g.idCategoriaConta=" + idCategoriaConta;

                if (idGrupoConta > 0)
                    where += " And g.IdGrupo=" + idGrupoConta;

                if (idsPlanoConta.Any())
                    where += " And p.IdConta IN (" + string.Join(",", idsPlanoConta) + ")";

                if (idLoja > 0)
                    where += " And cp.IdLoja=" + idLoja;

                if (!String.IsNullOrEmpty(dataIni))
                    where += " And if (cp.idChequePagto is null, cp.DataPagto>=?dataIni, ch.dataReceb>=?dataIni)";

                if (!String.IsNullOrEmpty(dataFim))
                    where += " And if (cp.idChequePagto is null, cp.DataPagto<=?dataFim, ch.dataReceb<=?dataFim)";

                switch (tipoConta)
                {
                    case 1: // Contábil
                        where += " and cp.contabil=true";
                        break;
                    case 2: // Não contábil
                        where += " and (cp.contabil=false or cp.contabil is null)";
                        break;
                }
            }

            return where;
        }

        private string GetPlanoContaFiltro(uint idCategoriaConta, uint idGrupoConta, uint[] idsPlanoConta, uint idLoja, string dataIni, string dataFim, int tipoMov, int tipoConta)
        {
            string where = "where p.idConta not in (select idConta from (" + 
                SqlGeral(idCategoriaConta, idGrupoConta, idsPlanoConta, idLoja, dataIni, dataFim, tipoMov, tipoConta, false, false, false,
                true, true, false, true, false, false) + " Having (VencPeriodoNaoPagas=0 Or VencPeriodoNaoPagas is null) And " +
                "(VencPassadoPagasPeriodo=0 Or VencPassadoPagasPeriodo is null)) as tbl2)";

            if (idCategoriaConta > 0)
                where += " And g.idCategoriaConta=" + idCategoriaConta;

            if (idGrupoConta > 0)
                where += " And g.IdGrupo=" + idGrupoConta;

            if (idsPlanoConta.Any())
                where += " And p.IdConta IN (" + string.Join(",", idsPlanoConta) + ")";

            return where;
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
        /// Ajusta o valor das movimentações que foram pagas com crédito.
        /// O valor de crédito utilizado em pagamentos não deve ser exibido no DRE, pois,
        /// já existe uma movimentação no DRE referente à geração do crédito.
        /// </summary>
        public void AjustarValorMovimentacaoPagaComCreditoFornecedor(ref List<PlanoContas> movimentacoes)
        {
            AjustarValorMovimentacaoPagaComCreditoFornecedor(ref movimentacoes, false);
        }

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
