using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using GDA;
using Glass.Data.Helper;
using Glass.Data.Model;

namespace Glass.Data.RelDAL
{
    public sealed class MovimentacoesFinanceirasDAO : BaseDAO<MovimentacoesFinanceiras, MovimentacoesFinanceirasDAO>
    {
        //private MovimentacoesFinanceirasDAO() { }

        private string Sql(string dataIni, string dataFim, bool detalhado, bool selecionar)
        {
            string planosContaDinheiroEntrada = UtilsPlanoConta.GetLstEntradaByFormaPagto(Glass.Data.Model.Pagto.FormaPagto.Dinheiro, 0, false);
            string planosContaChequeEntrada = UtilsPlanoConta.GetLstEntradaByFormaPagto(Glass.Data.Model.Pagto.FormaPagto.ChequeProprio, 0, false);
            string planosContaConstrucardEntrada = UtilsPlanoConta.GetLstEntradaByFormaPagto(Glass.Data.Model.Pagto.FormaPagto.Construcard, 0, false);

            var lstPlanosContaCartao = new Dictionary<uint, string>();
            foreach (var c in UtilsPlanoConta.ContasCartoes)
                lstPlanosContaCartao.Add((uint)c.IdTipoCartao, UtilsPlanoConta.GetLstEntradaByFormaPagto(Glass.Data.Model.Pagto.FormaPagto.Cartao, (uint)c.IdTipoCartao, false));

            string planosContaDinheiroSaida = UtilsPlanoConta.GetLstSaidaByFormaPagto(Glass.Data.Model.Pagto.FormaPagto.Dinheiro, 0, 1);
            string planosContaChequeSaida = UtilsPlanoConta.GetLstSaidaByFormaPagto(Glass.Data.Model.Pagto.FormaPagto.ChequeProprio, 0, 1);
            string planosContaConstrucardSaida = UtilsPlanoConta.GetLstSaidaByFormaPagto(Glass.Data.Model.Pagto.FormaPagto.Construcard, 0, 1);

            string planosContaDinheiroEstorno = UtilsPlanoConta.GetLstEstornoSaidaByFormaPagto(Glass.Data.Model.Pagto.FormaPagto.Dinheiro, 0);
            string planosContaConstrucardEstorno = UtilsPlanoConta.GetLstEstornoSaidaByFormaPagto(Glass.Data.Model.Pagto.FormaPagto.Construcard, 0);

            if (string.IsNullOrEmpty(planosContaConstrucardEntrada))
                planosContaConstrucardEntrada = "0";

            if (string.IsNullOrEmpty(planosContaConstrucardSaida))
                planosContaConstrucardSaida = "0";

            if (string.IsNullOrEmpty(planosContaConstrucardEstorno))
                planosContaConstrucardEstorno = "0";

            string dataInicioMes = "date(date_sub(?dataIni, interval day(?dataIni)-1 day))";
            string dataFimMes = "date_sub(date(date_add(date_sub(?dataFim, interval day(?dataFim)-1 day), interval 1 month)), interval 1 second)";

            #region SQL para caixa geral

            string camposCaixaGeral = !selecionar ? "count(*), cg.idConta" : 
                @"2 as tipoMov, '{0}' as nomeMov, cg.idConta, (
                        select (
                                select sum(valorMov)
                                from caixa_geral
                                where (idConta in ({1},{5}) {2})
                                    and tipoMov=1
                                    and dataMov<?dataIni
                                    " + (detalhado ? "and idConta=cg.idConta" : "") + @"
                            )-(
                                select sum(valorMov)
                                from caixa_geral
                                where (idConta in ({3}) {4})
                                    and tipoMov=2
                                    and dataMov<?dataIni
                                    " + (detalhado ? "and idConta=cg.idConta" : "") + @"
                            )
                    ) as saldoAnteriorDia, sum(if(cg.tipoMov=1 and (cg.idConta in ({1}) {2}), cg.valorMov, 0)) as entradasDia,
                    sum(if(cg.tipoMov=2 and (cg.idConta in ({3}) {4}), cg.valorMov, 0)) as saidasDia, (
                        select (
                                select sum(valorMov)
                                from caixa_geral
                                where (idConta in ({1},{5}) {2})
                                    and tipoMov=1
                                    and dataMov<" + dataInicioMes + @"
                                    " + (detalhado ? "and idConta=cg.idConta" : "") + @"
                            )-(
                                select sum(valorMov)
                                from caixa_geral
                                where (idConta in ({3}) {4})
                                    and tipoMov=2
                                    and dataMov<" + dataInicioMes + @"
                                    " + (detalhado ? "and idConta=cg.idConta" : "") + @"
                            )
                    )  as saldoAnteriorMes, mes.entradas as entradasMes, mes.saidas as saidasMes";

            string sqlCaixaGeral = @"
                select " + camposCaixaGeral + @"
                from caixa_geral cg, (
                        select idConta, sum(entradas) as entradas, sum(saidas) as saidas
                        from (
                            select idConta, sum(valorMov) as entradas, 0 as saidas
                            from caixa_geral
                            where (idConta in ({1}) {2})
                                and tipoMov=1
                                and dataMov>=" + dataInicioMes + @"
                                and dataMov<=" + dataFimMes + @"
                                " + (detalhado ? "group by idConta" : "") + @"
                            
                            union select idConta, 0 as entradas, sum(valorMov) as saidas
                            from caixa_geral
                            where (idConta in ({3}) {4})
                                and tipoMov=2
                                and dataMov>=" + dataInicioMes + @"
                                and dataMov<=" + dataFimMes + @"
                                " + (detalhado ? "group by idConta" : "") + @"
                        ) as temp
                        " + (detalhado ? "group by idConta" : "") + @"
                    ) as mes
                where (cg.idConta in ({1},{3},{5}) {2} {4})
                    and cg.dataMov>=?dataIni
                    and cg.dataMov<=?dataFim
                    " + (detalhado ? "and cg.idConta=mes.idConta" : "") + @"
                    " + (detalhado ? "group by cg.idConta" : "");

            #endregion

            #region SQL para caixa diário

            string camposCaixaDiario = !selecionar ? "count(*), cd.idConta" :
                @"2 as tipoMov, '{0}' as nomeMov, cd.idConta, (
                        select (
                                select sum(valor)
                                from caixa_diario
                                where (idConta in ({1},{5}) {2})
                                    and tipoMov=1
                                    and dataCad<?dataIni
                                    " + (detalhado ? "and idConta=cd.idConta" : "") + @"
                            )-(
                                select sum(valor)
                                from caixa_diario
                                where (idConta in ({3}) {4})
                                    and tipoMov=2
                                    and dataCad<?dataIni
                                    " + (detalhado ? "and idConta=cd.idConta" : "") + @"
                            )
                    ) as saldoAnteriorDia, sum(if(cd.tipoMov=1 and (cd.idConta in ({1}) {2}), cd.valor, 0)) as entradasDia,
                    sum(if(cd.tipoMov=2 and (cd.idConta in ({3}) {4}), cd.valor, 0)) as saidasDia, (
                        select (
                                select sum(valor)
                                from caixa_diario
                                where (idConta in ({1},{5}) {2})
                                    and tipoMov=1
                                    and dataCad<" + dataInicioMes + @"
                                    " + (detalhado ? "and idConta=cd.idConta" : "") + @"
                            )-(
                                select sum(valor)
                                from caixa_diario
                                where (idConta in ({3}) {4})
                                    and tipoMov=2
                                    and dataCad<" + dataInicioMes + @"
                                    " + (detalhado ? "and idConta=cd.idConta" : "") + @"
                            )
                    )  as saldoAnteriorMes, mes.entradas as entradasMes, mes.saidas as saidasMes";

            string sqlCaixaDiario = @"
                select " + camposCaixaDiario + @"
                from caixa_diario cd, (
                        select idConta, sum(entradas) as entradas, sum(saidas) as saidas
                        from (
                            select idConta, sum(valor) as entradas, 0 as saidas
                            from caixa_diario
                            where (idConta in ({1}) {2})
                                and tipoMov=1
                                and dataCad>=" + dataInicioMes + @"
                                and dataCad<=" + dataFimMes + @"
                                " + (detalhado ? "group by idConta" : "") + @"
                            
                            union select idConta, 0 as entradas, sum(valor) as saidas
                            from caixa_diario
                            where (idConta in ({3}) {4})
                                and tipoMov=2
                                and dataCad>=" + dataInicioMes + @"
                                and dataCad<=" + dataFimMes + @"
                                " + (detalhado ? "group by idConta" : "") + @"
                        ) as temp
                        " + (detalhado ? "group by idConta" : "") + @"
                    ) as mes
                where (cd.idConta in ({1},{3},{5}) {2} {4})
                    and cd.dataCad>=?dataIni
                    and cd.dataCad<=?dataFim
                    " + (detalhado ? "and cd.idConta=mes.idConta" : "") + @"
                    " + (detalhado ? "group by cd.idConta" : "");

            #endregion

            #region SQL para cheques

            string dataCompCheque = "coalesce((select dataCad from caixa_geral where idCheque=c1.idCheque and idConta in ({3}) limit 1), " +
                "(select dataCad from mov_banco where idCheque=c1.idCheque and idConta in ({4}) limit 1))";

            string camposCheques = !selecionar ? "count(*), null as idConta" :
                @"2 as tipoMov, '{0}' as nomeMov, null as idConta, (
                        select sum(valor)
                        from cheques c1
                        where situacao in ({1})
                            and tipo=2
                            and dataCad<?dataIni
                            and if(situacao={2}, " + dataCompCheque + @">?dataIni, true)
                    ) as saldoAnteriorDia, dia.entradas as entradasDia, dia.saidas as saidasDia, (
                        select sum(valor)
                        from cheques c1
                        where situacao in ({1})
                            and tipo=2
                            and dataCad<" + dataInicioMes + @"
                            and if(situacao={2}, " + dataCompCheque + @">?dataIni, true)
                    ) as saldoAnteriorMes, mes.entradas as entradasMes, mes.saidas as saidasMes";

            string sqlCheques = @"
                select " + camposCheques + @"
                from (
                    select sum(entradas) as entradas, sum(saidas) as saidas
                    from (
                        select sum(valor) as entradas, 0 as saidas
                        from cheques c1
                        where situacao in ({1})
                            and tipo=2
                            and dataCad>=?dataIni
                            and dataCad<=?dataFim
                            and if(situacao={2}, " + dataCompCheque + @">=date_add(date(dataCad), interval 1 day), true)
                        
                        union select 0 as entradas, sum(valor) as saidas
                        from cheques c1
                        where situacao in ({2})
                            and tipo=2
                            and dataCad>=?dataIni
                            and dataCad<=?dataFim
                            and " + dataCompCheque + @">=date_add(date(dataCad), interval 1 day)
                    ) as temp
                ) as dia, (
                        select sum(entradas) as entradas, sum(saidas) as saidas
                        from (
                            select sum(valor) as entradas, 0 as saidas
                            from cheques c1
                            where situacao in ({1})
                                and tipo=2
                                and dataCad>=" + dataInicioMes + @"
                                and dataCad<=" + dataFimMes + @"
                                and if(situacao={2}, " + dataCompCheque + @">=date_add(date(dataCad), interval 1 day), true)
                            
                            union select 0 as entradas, sum(valor) as saidas
                            from cheques c1
                            where situacao in ({2})
                                and tipo=2
                                and dataCad>=" + dataInicioMes + @"
                                and dataCad<=" + dataFimMes + @"
                                and " + dataCompCheque + @">=date_add(date(dataCad), interval 1 day)
                        ) as temp
                    ) as mes";

            string sqlChequesDevolvidos = @"
                select " + camposCheques + @"
                from (
                    select sum(entradas) as entradas, sum(saidas) as saidas
                    from (
                        select sum(valor) as entradas, 0 as saidas
                        from cheques c1
                        where situacao in ({1})
                            and tipo=2
                            and dataCad>=?dataIni
                            and dataCad<=?dataFim
                            and if(situacao={2}, " + dataCompCheque + @">=date_add(date(dataCad), interval 1 day), true)
                        
                        union select 0 as entradas, sum(valor) as saidas
                        from cheques c1
                        where situacao in ({2})
                            and tipo=2
                            and dataCad<=?dataFim
                            and " + dataCompCheque + @">=?dataIni
                    ) as temp
                ) as dia, (
                        select sum(entradas) as entradas, sum(saidas) as saidas
                        from (
                            select sum(valor) as entradas, 0 as saidas
                            from cheques c1
                            where situacao in ({1})
                                and tipo=2
                                and dataCad>=" + dataInicioMes + @"
                                and dataCad<=" + dataFimMes + @"
                                and if(situacao={2}, " + dataCompCheque + @">=date_add(date(dataCad), interval 1 day), true)
                            
                            union select 0 as entradas, sum(valor) as saidas
                            from cheques c1
                            where situacao in ({2})
                                and tipo=2
                                and dataCad>=" + dataInicioMes + @"
                                and dataCad<=" + dataFimMes + @"
                                and " + dataCompCheque + @">=date_add(date(dataCad), interval 1 day)
                        ) as temp
                    ) as mes";

            #endregion

            #region SQL para cartões

            var sqlCartoesSaldoAnterior = @"
                SELECT SUM(ValorVec) AS Entradas
                FROM contas_receber
                WHERE IsParcelaCartao = 1 AND Recebida = 0 AND IdConta IN ({1})";

            var camposCartoes = !selecionar ? "COUNT(*), cr.IdConta" :
                string.Format(@"2 AS TipoMov, '{0}' AS NomeMov, cr.IdConta,
                    ({1}) AS SaldoAnteriorDia, SUM(IF(cr.Recebida = 0, cr.ValorVec, 0)) AS EntradasDia,
                    SUM(IF(cr.Recebida = 1, cr.ValorVec, 0)) AS SaidasDia,
                    ({1}) AS SaldoAnteriorMes, mes.Entradas AS EntradasMes, mes.Saidas AS SaidasMes",
                    "{0}", sqlCartoesSaldoAnterior);

            var sqlCartoesEntradaSaida =
                string.Format(@"
                    SELECT SUM(ValorVec) AS Entradas, 0 AS Saidas
                    FROM contas_receber
                    WHERE IdConta IN ({0})
                        AND IsParcelaCartao = 1
                        AND DataCad >= {1}
                        AND DataCad <= {2}
                            
                    UNION SELECT 0 AS Entradas, SUM(ValorRec) AS Saidas
                    FROM contas_receber
                    WHERE IdConta IN ({0})
                        AND IsParcelaCartao = 1
                        AND Recebida = 1
                        AND DataRec >= {1}
                        AND DataRec <= {2}", "{1}", dataInicioMes, dataFimMes);

            var sqlCartoes = string.Format(@"
                SELECT {0}
                FROM contas_receber cr, (
                        SELECT SUM(Entradas) AS Entradas, SUM(Saidas) AS Saidas
                        FROM (
                            {2}
                        ) AS temp
                    ) AS mes
                WHERE cr.IdConta IN ({1})
                    AND cr.IsParcelaCartao = 1
                    AND cr.DataVec >= ?dataIni
                    AND cr.DataVec <= ?dataFim", camposCartoes, "{1}",
                    dataIni == dataFim ? sqlCartoesSaldoAnterior.Replace("Entradas", "Entradas, 0 AS Saidas") : sqlCartoesEntradaSaida);

            #endregion

            #region Contas bancárias

            var orderBySaldoBanco = "DATE_FORMAT(DataMov, '%Y-%m-%d %H%i') DESC, IdMovBanco DESC";
            string camposBanco = !selecionar ? "count(*), m.idConta" : 
                @"1 as tipoMov, c.nome as nomeMov, m.idConta, saldoAnteriorDia.saldo as saldoAnteriorDia, 
                    dia.entradas as entradasDia, dia.saidas as saidasDia, saldoAnteriorMes.saldo as saldoAnteriorMes, 
                    mes.entradas as entradasMes, mes.saidas as saidasMes";

            string sqlBanco = @"
                select " + camposBanco + @"
                from mov_banco m
                    inner join conta_banco c on (m.idContaBanco=c.idContaBanco)
                    inner join (
                        select idContaBanco, idConta, sum(saldo) as saldo
                        from (
                            select idContaBanco, idConta, saldo
                            from (
                                select idContaBanco, idConta, saldo
                                from mov_banco 
                                where dataMov<?dataIni
                                order by " + orderBySaldoBanco + @"
                            ) as temp1
                            group by idContaBanco" + (detalhado ? ", idConta" : "") + @"
                            
                            union all select idContaBanco, null as idConta, 0 as saldo
                            from conta_banco
                        ) as temp
                        group by idContaBanco" + (detalhado ? ", idConta" : "") + @"
                    ) as saldoAnteriorDia on (saldoAnteriorDia.idContaBanco=m.idContaBanco" + (detalhado ? " and saldoAnteriorDia.idConta=m.idConta" : "") + @")
                    inner join (
                        select idContaBanco, idConta, sum(saldo) as saldo
                        from (
                            select idContaBanco, idConta, saldo
                            from (
                                select idContaBanco, idConta, saldo
                                from mov_banco
                                where dataMov<" + dataInicioMes + @"
                                order by " + orderBySaldoBanco + @"
                            ) as temp1
                            group by idContaBanco" + (detalhado ? ", idConta" : "") + @"

                            union all select idContaBanco, null as idConta, 0 as saldo
                            from conta_banco
                        ) as temp
                        group by idContaBanco" + (detalhado ? ", idConta" : "") + @"
                    ) as saldoAnteriorMes on (saldoAnteriorMes.idContaBanco=m.idContaBanco" + (detalhado ? " and saldoAnteriorMes.idConta=m.idConta" : "") + @")
                    inner join (
                        select idContaBanco, idConta, sum(entradas) as entradas, sum(saidas) as saidas
                        from (
                            select idContaBanco, idConta, sum(if(tipoMov=1, valorMov, 0)) as entradas, sum(if(tipoMov=1, 0, valorMov)) as saidas
                            from mov_banco
                            where dataMov>=?dataIni 
                                and dataMov<=?dataFim
                            group by idContaBanco" + (detalhado ? ", idConta" : "") + @"
                            
                            union all select idContaBanco, null as idConta, 0 as entradas, 0 as saidas
                            from conta_banco
                        ) as temp
                        group by idContaBanco" + (detalhado ? ", idConta" : "") + @"
                    ) as dia on (m.idContaBanco=dia.idContaBanco" + (detalhado ? " and m.idConta=dia.idConta" : "") + @")
                    inner join (
                        select idContaBanco, idConta, sum(entradas) as entradas, sum(saidas) as saidas
                        from (
                            select idContaBanco, idConta, sum(if(tipoMov=1, valorMov, 0)) as entradas, sum(if(tipoMov=1, 0, valorMov)) as saidas
                            from mov_banco
                            where dataMov>=" + dataInicioMes + @"
                                and dataMov<=" + dataFimMes + @"
                            group by idContaBanco" + (detalhado ? ", idConta" : "") + @"
                            
                            union all select idContaBanco, null as idConta, 0 as entradas, 0 as saidas
                            from conta_banco
                        ) as temp
                        group by idContaBanco" + (detalhado ? ", idConta" : "") + @"
                    ) as mes on (m.idContaBanco=mes.idContaBanco" + (detalhado ? " and m.idConta=mes.idConta" : "") + @")
                where m.idContaBanco in (select idContaBanco from conta_banco where situacao=" + (int)Glass.Situacao.Ativo + @")
                group by m.idContaBanco" + (detalhado ? ", m.idConta" : "");

            #endregion

            #region Caixa (dinheiro)

            string sqlCaixa = String.Format(sqlCaixaGeral, "Caixa (dinheiro)", planosContaDinheiroEntrada, "or formaSaida=" + (int)Glass.Data.Model.Pagto.FormaPagto.Dinheiro,
                planosContaDinheiroSaida, "or formaSaida=" + (int)Glass.Data.Model.Pagto.FormaPagto.Dinheiro, planosContaDinheiroEstorno);

            #endregion

            #region Cheque devolvido

            string sqlChequeDevolvido = String.Format(sqlChequesDevolvidos, "Cheque Devolvido", (int)Cheques.SituacaoCheque.Devolvido + "," + (int)Cheques.SituacaoCheque.Quitado, 
                (int)Cheques.SituacaoCheque.Quitado, planosContaChequeEntrada, planosContaChequeSaida);

            #endregion

            #region Nota promissória

            string camposNotaPromissoria = !selecionar ? "count(*), dia.idConta" :
                @"2 as tipoMov, 'Nota Promis.' as nomeMov, dia.idConta, (
                        select sum(valorVec)
                        from contas_receber
                        where (isParcelaCartao=false or isParcelaCartao is null)
                            and dataCad<?dataIni
                            and (
                                (recebida=false or recebida is null)
                                or (dataRec is null or dataRec>=?dataIni)
                            )
                            and (renegociada=false or renegociada is null)
                            " + (detalhado ? "and idConta=dia.idConta" : "") + @"
                    ) as saldoAnteriorDia, dia.entradas as entradasDia, dia.saidas as saidasDia, (
                        select sum(valorVec)
                        from contas_receber
                        where (isParcelaCartao=false or isParcelaCartao is null)
                            and dataCad<" + dataInicioMes + @"
                            and (
                                (recebida=false or recebida is null)
                                or (dataRec is null or dataRec>=" + dataInicioMes + @")
                            )
                            and (renegociada=false or renegociada is null)
                            " + (detalhado ? "and idConta=mes.idConta" : "") + @"
                    ) as saldoAnteriorMes, mes.entradas as entradasMes, mes.saidas as saidasMes";

            string sqlNotaPromissoria = @"
                select " + camposNotaPromissoria + @"
                from (
                    select idConta, sum(entradas) as entradas, sum(saidas) as saidas
                    from (
                        select idConta, sum(valorVec-if(dataRec>=date_add(date(dataCad), interval 1 day), 0, valorRec)) as entradas, 0 as saidas
                        from contas_receber
                        where (isParcelaCartao=false or isParcelaCartao is null)
                            and dataCad>=?dataIni
                            and dataCad<=?dataFim
                            and (
                                (recebida=false or recebida is null)
                                or (dataRec is null or dataRec>=date_add(date(dataCad), interval 1 day))
                            )
                            and (renegociada=false or renegociada is null)
                        " + (detalhado ? "group by idConta" : "") + @"
                        
                        /* 
                            Ao calcular o valor da saída, se o valorRec for 0, dá saída no valorVec, isso porque a conta foi renegociada 
                            para outra data, logo, ela deve dar saída no valorVec, para que ao dar entrada no valor restante gerado em outra 
                            conta a receber, não fique duplicado e para acertar o saldo anterior e atual, ao tirar este relatório dois dias seguidos
                        */
                        union all select idConta, 0 as entradas, sum(if(valorRec = 0 Or valorRec>(valorVec+coalesce(juros,0)), valorVec, valorRec-coalesce(juros,0))) as saidas
                        from contas_receber
                        where (isParcelaCartao=false or isParcelaCartao is null)
                            and recebida=true
                            and dataRec>=?dataIni
                            and dataRec<=?dataFim
                            and dataCad<date(dataRec)
                        " + (detalhado ? "group by idConta" : "") + @"
                    ) as temp
                    " + (detalhado ? "group by idConta" : "") + @"
                ) as dia, (
                        select idConta, sum(entradas) as entradas, sum(saidas) as saidas
                        from (
                            select idConta, sum(valorVec-if(dataRec>=date_add(date(dataCad), interval 1 day), 0, valorRec)) as entradas, 0 as saidas
                            from contas_receber
                            where (isParcelaCartao=false or isParcelaCartao is null)
                                and dataCad>=" + dataInicioMes + @"
                                and dataCad<=" + dataFimMes + @"
                                and (
                                    (recebida=false or recebida is null)
                                    or (dataRec is null or dataRec>=date_add(date(dataCad), interval 1 day))
                                )
                                and (renegociada=false or renegociada is null)
                            " + (detalhado ? "group by idConta" : "") + @"
                            
                            /* 
                                Ao calcular o valor da saída, se o valorRec for 0, dá saída no valorVec, isso porque a conta foi renegociada 
                                para outra data, logo, ela deve dar saída no valorVec, para que ao dar entrada no valor restante gerado em outra 
                                conta a receber, não fique duplicado e para acertar o saldo anterior e atual, ao tirar este relatório dois dias seguidos
                            */
                            union all select idConta, 0 as entradas, sum(if(valorRec=0 Or valorRec>(valorVec+coalesce(juros,0)), valorVec, valorRec-coalesce(juros,0))) as saidas
                            from contas_receber
                            where (isParcelaCartao=false or isParcelaCartao is null)
                                and recebida=true
                                and dataRec>=" + dataInicioMes + @"
                                and dataRec<=" + dataFimMes + @"
                                and dataCad<date(dataRec)
                            " + (detalhado ? "group by idConta" : "") + @"
                        ) as temp
                        " + (detalhado ? "group by idConta" : "") + @"
                    ) as mes
                where 1
                    " + (detalhado ? "and dia.idConta=mes.idConta" : "") + @"
                " + (detalhado ? "group by dia.idConta" : "");

            #endregion

            #region Cheque

            string sqlCheque = String.Format(sqlCheques, "Carteira de Cheque", (int)Cheques.SituacaoCheque.EmAberto + "," + (int)Cheques.SituacaoCheque.Compensado,
                (int)Cheques.SituacaoCheque.Compensado, planosContaChequeEntrada, planosContaChequeSaida);

            #endregion

            #region Cartão

            var lstSqlsCartoes = new List<string>();

            foreach (var item in lstPlanosContaCartao)
            {
                var descricao = TipoCartaoCreditoDAO.Instance.ObterDescricao(null, (int)item.Key);
                lstSqlsCartoes.Add(string.Format(sqlCartoes, descricao, item.Value));
            }

            #endregion

            #region Construcard

            var sqlConstrucard = string.Format(sqlCaixaGeral, "Construcard Cx. Geral", planosContaConstrucardEntrada, "",
                planosContaConstrucardSaida, "", planosContaConstrucardEstorno);

            var sqlConstrucardCxDiario = string.Format(sqlCaixaDiario, "Construcard Cx. Diário", planosContaConstrucardEntrada, "",
                planosContaConstrucardSaida, "", planosContaConstrucardEstorno);

            #endregion

            string criterio = "";
            if (!String.IsNullOrEmpty(dataIni) && !String.IsNullOrEmpty(dataFim) && dataIni == dataFim)
                criterio = "Data: " + dataIni + "    ";
            else if (!String.IsNullOrEmpty(dataIni) || !String.IsNullOrEmpty(dataFim))
            {
                if (!String.IsNullOrEmpty(dataIni))
                    criterio += "Data de início: " + dataIni + "    ";

                if (!String.IsNullOrEmpty(dataFim))
                    criterio += "Data de término: " + dataFim + "    ";
            }

            if (detalhado)
                criterio += "Relatório detalhado    ";

            string campos = !selecionar ? "coalesce(count(*), 0)" :
                @"cast(tipoMov as signed) as tipoMov, nomeMov, movimentacoes_financeiras.idConta, cast(saldoAnteriorDia as decimal(12,2)) as saldoAnteriorDia, 
                    cast(entradasDia as decimal(12,2)) as entradasDia, cast(saidasDia as decimal(12,2)) as saidasDia, cast(saldoAnteriorMes as decimal(12,2)) as saldoAnteriorMes, 
                    cast(entradasMes as decimal(12,2)) as entradasMes, cast(saidasMes as decimal(12,2)) as saidasMes, '$$$' as criterio, p.descricao as descrPlanoConta,
                    g.idGrupo as idGrupoConta, g.descricao as descrGrupoConta, c.idCategoriaConta, c.descricao as descrCategoriaConta";

            string sql = @"
                select " + campos + @"
                from (
                    " + sqlBanco + @"
                    union all " + sqlCaixa + @"
                    union all " + sqlChequeDevolvido + @"
                    union all " + sqlNotaPromissoria + @"
                    union all " + sqlCheque;

            foreach (var s in lstSqlsCartoes)
                sql += " UNION ALL " + s;

            sql += " union all " + sqlConstrucard + @"
                    union all " + sqlConstrucardCxDiario + @"
                ) as movimentacoes_financeiras
                    left join plano_contas p on (movimentacoes_financeiras.idConta=p.idConta)
                    left join grupo_conta g on (p.idGrupo=g.idGrupo)
                    left join categoria_conta c on (g.idCategoriaConta=c.idCategoriaConta)";

            sql = sql.Replace("$$$", criterio);
            return sql;
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lst.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lst.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            return lst.ToArray();
        }

        public IList<MovimentacoesFinanceiras> GetList(string dataIni, string dataFim, bool detalhado, string sortExpression, int startRow, int pageSize)
        {
            if (!detalhado) dataFim = dataIni;
            return LoadDataWithSortExpression(Sql(dataIni, dataFim, detalhado, true), sortExpression, startRow, pageSize, GetParams(dataIni, dataFim));
        }

        public int GetCount(string dataIni, string dataFim, bool detalhado)
        {
            if (!detalhado) dataFim = dataIni;
            return objPersistence.ExecuteSqlQueryCount(Sql(dataIni, dataFim, detalhado, false), GetParams(dataIni, dataFim));
        }

        public MovimentacoesFinanceiras[] GetForRpt(string dataIni, string dataFim, bool detalhado)
        {
            if (!detalhado) dataFim = dataIni;
            List<MovimentacoesFinanceiras> retorno = objPersistence.LoadData(Sql(dataIni, dataFim, detalhado, true), GetParams(dataIni, dataFim));

            if (detalhado)
            {
                List<uint> categorias = new List<uint>();
                foreach (MovimentacoesFinanceiras m in retorno)
                    if (!categorias.Contains(m.IdCategoriaConta))
                        categorias.Add(m.IdCategoriaConta);

                foreach (uint id in categorias)
                {
                    MovimentacoesFinanceiras categoria = new MovimentacoesFinanceiras();
                    if (id > 0)
                    {
                        categoria.IdCategoriaConta = id;
                        categoria.DescrCategoriaConta = CategoriaContaDAO.Instance.ObtemDescricao(id);
                    }

                    categoria.DescricaoCategoriaRelatorio = true;
                    retorno.Add(categoria);
                }

                MovimentacoesFinanceiras[] anteriores = GetForRpt(dataIni, dataIni, false);
                foreach (MovimentacoesFinanceiras m in anteriores)
                {
                    MovimentacoesFinanceiras anterior = new MovimentacoesFinanceiras();
                    anterior.DescrCategoriaConta = "Saldo Anterior";
                    anterior.DescrGrupoConta = "Saldo Anterior";
                    anterior.SaldoAnteriorDetalhado = true;
                    anterior.EntradasDia = m.SaldoAnteriorDia;
                    anterior.SaidasDia = 0;
                    anterior.NomeMov = m.NomeMov;
                    retorno.Add(anterior);
                }
            }

            return retorno.ToArray();
        }
    }
}
