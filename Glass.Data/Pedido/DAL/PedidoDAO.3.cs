using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Web;
using Glass.Data.Exceptions;
using System.Linq;
using Glass.Data.RelDAL;
using Glass.Configuracoes;
using Glass.Global;
using Colosoft;
using Glass.Data.Helper.Calculos;

namespace Glass.Data.DAL
{
    public sealed partial class PedidoDAO : BaseCadastroDAO<Pedido, PedidoDAO>
    {
        #region Retorna o total do Pedido

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna o total do Pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public decimal GetTotal(uint idPedido)
        {
            return GetTotal(null, idPedido);
        }

        /// <summary>
        /// Retorna o total do Pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public decimal GetTotal(GDASession sessao, uint idPedido)
        {
            string sql = "Select Coalesce(total, 0) from pedido Where idPedido=" + idPedido;
            return ExecuteScalar<decimal>(sessao, sql);
        }

        /// <summary>
        /// Retorna a comissão do comissionado do Pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public decimal GetComissao(uint idPedido)
        {
            string sql = "Select Coalesce(ValorComissao, 0) from pedido Where idPedido=" + idPedido;
            return ExecuteScalar<decimal>(sql);
        }

        public decimal GetTotalProdutos(uint idPedido)
        {
            // Atualiza total e custo do pedido
            string sql = "Select Coalesce(Sum(Total + coalesce(valorBenef, 0)), 0) From produtos_pedido Where IdPedido=" + idPedido +
                " and (InvisivelPedido=false or InvisivelPedido is null)";

            return ExecuteScalar<decimal>(sql);
        }

        public Pedido GetForTotalBruto(uint idPedido)
        {
            Pedido p = new Pedido();
            p.IdPedido = idPedido;
            p.Total = GetTotal(idPedido);
            p.ValorComissao = GetComissao(idPedido);
            p.TaxaPrazo = ObtemValorCampo<float>("taxaPrazo", "idPedido=" + idPedido);

            return p;
        }

        #endregion

        #region Listagem de Comissão

        #region SQLs para cálculo da comissão do pedido        

        /// <summary>
        /// Retorna o SQL usado para retornar o valor da comissão pago a um funcionário/comissionado.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="tipoFunc"></param>
        /// <param name="idInstalador"></param>
        /// <returns></returns>
        internal string SqlTotalComissaoPago(string idPedido, Pedido.TipoComissao tipoFunc, uint idInstalador)
        {
            string campo = tipoFunc == Pedido.TipoComissao.Funcionario ? "c.idFunc" :
                tipoFunc == Pedido.TipoComissao.Comissionado ? "c.idComissionado" :
                tipoFunc == Pedido.TipoComissao.Instalador ? "c.idInstalador" :
                tipoFunc == Pedido.TipoComissao.Gerente ? "c.idGerente" : "";

            string where = campo != "" ? campo + " is not null" : "1";
            if (tipoFunc == Pedido.TipoComissao.Instalador && idInstalador > 0)
                where += " and c.idInstalador=" + idInstalador;

            if (!String.IsNullOrEmpty(idPedido))
                where += " and cp.idPedido in (" + idPedido + ")";

            string sql = @"
                select cp.idPedido, " + campo + @", coalesce(sum(coalesce(cp.Valor, 0)), 0) as valor
                from comissao_pedido cp
                    left join comissao c on (cp.idcomissao=c.idComissao)
                where " + where + @"
                group by cp.idPedido, " + campo;

            return sql;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="tipoFunc"></param>
        /// <param name="idInstalador"></param>
        /// <returns></returns>
        internal string SqlTotalBaseCalcComissaoPago(string idPedido, Pedido.TipoComissao tipoFunc, uint idInstalador)
        {
            string campo = tipoFunc == Pedido.TipoComissao.Funcionario ? "c.idFunc" :
                tipoFunc == Pedido.TipoComissao.Comissionado ? "c.idComissionado" :
                tipoFunc == Pedido.TipoComissao.Instalador ? "c.idInstalador" : "";

            string where = campo != "" ? campo.Trim(',') + " is not null" : "1";
            if (tipoFunc == Pedido.TipoComissao.Instalador && idInstalador > 0)
                where += " and c.idInstalador=" + idInstalador;

            if (!String.IsNullOrEmpty(idPedido))
                where += " and cp.idPedido in (" + idPedido + ")";

            string sql = @"
                select cp.idPedido, " + (campo.IsNullOrEmpty() ? "" : campo + ", ") + @" coalesce(sum(coalesce(cp.BaseCalcComissao, 0)), 0) as valor
                from comissao_pedido cp
                    left join comissao c on (cp.idcomissao=c.idComissao)
                where " + where + @"
                group by cp.idPedido" + (campo.IsNullOrEmpty() ? "" : ", " + campo.Trim(','));

            return sql;
        }

        public string SqlComissao(string idComissao, string idsPedidos, uint idPedido, Pedido.TipoComissao tipoFunc, uint idFunc,
            string dataIni, string dataFim, bool semComissao, bool incluirComissaoPaga, string campoIds, uint idLoja, string tiposvenda = "")
        {
            return SqlComissao((GDASession)null, idComissao, idsPedidos, idPedido, tipoFunc, idFunc, dataIni, dataFim,
                semComissao, incluirComissaoPaga, campoIds, idLoja, tiposvenda);
        }

        private string SqlComissao(GDASession session, string idComissao, string idsPedidos, uint idPedido, Pedido.TipoComissao tipoFunc, uint idFunc,
            string dataIni, string dataFim, bool semComissao, bool incluirComissaoPaga, string campoIds, uint idLoja, string tiposVenda = "")
        {
            string TOLERANCIA_VALORES_PAGAR_PAGO = "0.01";
            bool selecaoIds = !String.IsNullOrEmpty(campoIds);

            string total = SqlCampoTotalLiberacao(!selecaoIds && PedidoConfig.LiberarPedido, "total", "p", "pe", "ap", "plp");

            string campos = !selecaoIds ? "p.idPedido, p.idLoja, p.idFunc, p.idCli, p.idFormaPagto, p.idOrcamento, " + total + @", p.prazoEntrega, 
                p.tipoEntrega, p.tipoVenda, p.dataEntrega, p.valorEntrega, p.situacao, p.valorEntrada, p.dataCad, p.usuCad, p.numParc, p.total as totalReal,
                p.desconto, p.obs, p.custoPedido, p.dataConf, p.usuConf, p.dataCanc, p.usuCanc, p.enderecoObra, p.bairroObra, p.cidadeObra, 
                p.localObra, p.idFormaPagto2, p.idTipoCartao, p.idTipoCartao2, p.codCliente, p.numAutConstrucard, p.idComissionado, p.percComissao, 
                p.valorComissao, p.idPedidoAnterior, p.fastDelivery, p.dataPedido, p.valorIcms, p.aliquotaIcms, p.idObra, p.idMedidor, p.taxaPrazo, 
                p.tipoPedido, p.tipoDesconto, p.acrescimo, p.tipoAcrescimo, p.taxaFastDelivery, p.temperaFora, p.rotaExterna, p.clienteExterno,
                p.situacaoProducao, p.idFuncVenda, p.dataEntregaOriginal, p.peso, p.totM, p.geradoParceiro, p.aliquotaIpi, p.valorIpi, p.idParcela, p.pedCliExterno,
                p.celCliExterno, p.cepObra, p.idSinal, p.idPagamentoAntecipado, p.valorPagamentoAntecipado, p.dataPronto, p.obsLiberacao, p.idProjeto, p.idLiberarPedido,
                p.PercentualComissao, " + ClienteDAO.Instance.GetNomeCliente("c") + @" as NomeCliente, f.Nome as NomeFunc, '$$$' as Criterio, l.NomeFantasia as nomeLoja, 
                cast(" + (int)tipoFunc + @" as signed) as ComissaoFuncionario, p.valorCreditoAoConfirmar, p.creditoGeradoConfirmar, p.idFuncDesc,
                p.dataDesc, p.importado, p.creditoUtilizadoConfirmar, p.deveTransferir, p.dataFin, p.usuFin" + (PedidoConfig.LiberarPedido ? ", lp.dataLiberacao" : "") :
                "distinct " + campoIds + " as id";

            if (!selecaoIds)
            {
                if (tipoFunc == Pedido.TipoComissao.Todos || tipoFunc == Pedido.TipoComissao.Comissionado)
                    campos += ", com.nome as nomeComissionado";

                if (tipoFunc == Pedido.TipoComissao.Todos || tipoFunc == Pedido.TipoComissao.Instalador)
                    campos += @", fe.idFunc as idInstalador, fe.idEquipe as idEquipe, fi.nome as nomeInst,
                    (select count(*) from func_equipe where idEquipe=fe.idEquipe) as numeroIntegrantesEquipe, i.dataFinal as dataFinalizacaoInst";

                campos += @"
                    , (SELECT GROUP_CONCAT(nf.numeroNfe) as numeroNfe
                    FROM pedidos_nota_fiscal pnf
	                    INNER JOIN nota_fiscal nf ON (pnf.idNf = nf.idNf)
                    WHERE pnf.idPedido = p.idPedido) as NfeAssociada";
            }

            string sql = @"
                Select " + campos + @"
                From pedido p
                    Left Join pedido_espelho pe On (p.idPedido=pe.idPedido)
                    Inner Join cliente c On (p.idCli=c.id_Cli)
                    Inner Join funcionario f On (p.IdFunc=f.IdFunc)
                    Inner Join loja l On (p.IdLoja = l.IdLoja)";

            if (PedidoConfig.LiberarPedido)
                sql += @"
                    Inner Join produtos_pedido pp On (pp.idPedido=p.idPedido)
                    Left Join ambiente_pedido ap On (pp.idAmbientePedido=ap.idAmbientePedido)
                    Left Join produtos_liberar_pedido plp on (pp.idProdPed=plp.idProdPed)
                    Left Join liberarpedido lp on (lp.idLiberarPedido=plp.idLiberarPedido)";

            if (tipoFunc == Pedido.TipoComissao.Todos || tipoFunc == Pedido.TipoComissao.Comissionado)
                sql += @"
                    Left Join comissionado com On (p.idComissionado=com.idComissionado)";

            if (tipoFunc == Pedido.TipoComissao.Todos || tipoFunc == Pedido.TipoComissao.Instalador)
            {
                sql += @"
                    Left Join instalacao i on (p.idPedido=i.idPedido)
                    Left Join equipe_instalacao ei On (i.idInstalacao=ei.idInstalacao)
                    Left Join func_equipe fe on (ei.idEquipe=fe.idEquipe)
                    Left Join funcionario fi on (fe.idFunc=fi.idFunc)";
            }

            sql += " Where COALESCE(p.IgnorarComissao, 0) = 0";

            string filtro = " and p.Situacao in (" + (int)Pedido.SituacaoPedido.Confirmado + "," + (int)Pedido.SituacaoPedido.LiberadoParcialmente + @")
                And p.TipoVenda Not In (" + (int)Pedido.TipoVendaPedido.Garantia + "," + (int)Pedido.TipoVendaPedido.Reposição + @")
                And p.tipoPedido<>" + (int)Pedido.TipoPedidoEnum.Producao;

            if (!string.IsNullOrEmpty(tiposVenda))
                sql += string.Format(" and p.TipoVenda IN({0})", tiposVenda);

            // Não inclui este filtro na variável filtro, pois no sql um pouco abaixo este filtro será feito de forma diferente
            if (PedidoConfig.LiberarPedido)
                sql += " and lp.situacao=" + (int)LiberarPedido.SituacaoLiberarPedido.Liberado;

            if (tipoFunc == Pedido.TipoComissao.Instalador)
                filtro += " and ei.idOrdemInstalacao=i.idOrdemInstalacao";

            if (!String.IsNullOrEmpty(idComissao) && idComissao != "0")
                filtro += " and p.idPedido in (select idPedido from comissao_pedido where idComissao=" + idComissao + ")";
            else if (idComissao == "0")
                filtro += " and false";

            if (tipoFunc == Pedido.TipoComissao.Instalador)
            {
                var sitInstalacao = ((int)Instalacao.SituacaoInst.Finalizada).ToString();

                /* Chamado 52921.
                 * A customização feita para o cálculo de comissão por produto instalado está incompleta, quando o pedido possui comissão Continuada e Finalizada o total dele é multiplicado pela
                 * quantidade de instalações, fazendo com que o valor fique duplicado, triplicado etc. Portanto, será possível gerar comissão somente de instalações Finalizadas mesmo que a instalação
                 * seja feita por produto. Teremos que rever este controle e alterá-lo de forma que as comissões de instalações Continuadas e Finalizadas sejam geradas corretamente.
                if (Configuracoes.ComissaoConfig.UsarComissaoPorProdutoInstalado)
                    sitInstalacao += ", " + (int)Instalacao.SituacaoInst.Continuada;*/

                filtro += " and i.situacao IN (" + sitInstalacao + @") 
                    and i.tipoInstalacao <> " + (int)Instalacao.TipoInst.Entrega;
            }

            if (idLoja > 0)
                filtro += " AND p.idLoja=" + idLoja;

            string filtroFunc = "";

            switch (tipoFunc)
            {
                case Pedido.TipoComissao.Funcionario:
                    filtroFunc = " And p.IdFunc" + (idFunc > 0 ? "=" + idFunc : " is not null");
                    break;
                case Pedido.TipoComissao.Comissionado:
                    filtroFunc = " And p.idComissionado" + (idFunc > 0 ? "=" + idFunc : " is not null");
                    break;
                case Pedido.TipoComissao.Instalador:
                    filtroFunc = idFunc > 0 || idPedido > 0 ? " and fe.idFunc=" + idFunc :
                        " and p.idPedido in (select idPedido from instalacao where " + (Configuracoes.ComissaoConfig.UsarComissaoPorProdutoInstalado ? "DataInstalacao" : "dataFinal") + @">=?dataIni 
                            and " + (Configuracoes.ComissaoConfig.UsarComissaoPorProdutoInstalado ? "DataInstalacao" : "dataFinal") + @"<=?dataFim)";
                    break;
            }

            var idsLiberacao = String.Empty;

            // Ao invés de filtrar pela data da liberação, recupera os ids da mesma para que a consulta fique mais rápida
            if (tipoFunc != Pedido.TipoComissao.Instalador && PedidoConfig.LiberarPedido)
            {
                var filtroLib = String.Empty;
                var lstParam = new List<GDAParameter>();

                if (!String.IsNullOrEmpty(dataIni))
                {
                    filtroLib += " and DataLiberacao>=?dataIni";
                    lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni)));
                }

                if (!String.IsNullOrEmpty(dataFim))
                {
                    filtroLib += " and DataLiberacao<=?dataFim";
                    lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));
                }

                if (!String.IsNullOrEmpty(filtroLib))
                {
                    idsLiberacao = String.Join(",", ExecuteMultipleScalar<string>(session,
                        "Select Cast(idLiberarPedido as char) From liberarPedido Where 1" + filtroLib, lstParam.ToArray()).ToArray());

                    dataIni = null;
                    dataFim = null;
                }
            }

            if (!semComissao)
            {
                if (!incluirComissaoPaga)
                {
                    // Se o tipo de filtro for por instalado ou todos ou o id do func,comiss,inst, não tiver sido informado, não otimiza o sql.
                    if (tipoFunc == Pedido.TipoComissao.Instalador || tipoFunc == Pedido.TipoComissao.Todos || idFunc == 0)
                    {
                        string filtroFuncPed = "";

                        switch (tipoFunc)
                        {
                            case Pedido.TipoComissao.Funcionario:
                                filtroFuncPed = " And p.IdFunc" + (idFunc > 0 ? "=" + idFunc : " is not null");
                                break;
                            case Pedido.TipoComissao.Comissionado:
                                filtroFuncPed = " And p.idComissionado" + (idFunc > 0 ? "=" + idFunc : " is not null");
                                break;
                            case Pedido.TipoComissao.Instalador:
                                filtroFuncPed = idFunc > 0 || idPedido > 0 ? " and fe.idFunc=" + idFunc :
                                    " and p.idPedido in (select idPedido from instalacao where " + (Configuracoes.ComissaoConfig.UsarComissaoPorProdutoInstalado ? "DataInstalacao" : "dataFinal") + @">=?dataIni 
                                            and " + (Configuracoes.ComissaoConfig.UsarComissaoPorProdutoInstalado ? "DataInstalacao" : "dataFinal") + "<=?dataFim)";
                                break;
                        }

                        string filtroComissionado =
                            tipoFunc == Pedido.TipoComissao.Funcionario ?
                                " And pc.IdFunc" + (idFunc > 0 ? "=" + idFunc : " is not null") :
                            tipoFunc == Pedido.TipoComissao.Comissionado ?
                                " And pc.idComissionado" + (idFunc > 0 ? "=" + idFunc : " is not null") :
                            tipoFunc == Pedido.TipoComissao.Instalador ?
                                idFunc > 0 || idPedido > 0 ? " and fe.idFunc=" + idFunc :
                                   @" and p.idPedido in (select idPedido from instalacao where " + (Configuracoes.ComissaoConfig.UsarComissaoPorProdutoInstalado ? "DataInstalacao" : "dataFinal") + @">=?dataIni 
                                    and " + (Configuracoes.ComissaoConfig.UsarComissaoPorProdutoInstalado ? "DataInstalacao" : "dataFinal") + "<=?dataFim)" : "";

                        if (string.IsNullOrEmpty(idsPedidos))
                        {
                            var sqlIdsPedidos = @"
                                select distinct pc.idPedido
                                from pedido_comissao pc 
                                    inner join pedido p On (p.idPedido=pc.idPedido)" +
                                    (tipoFunc ==
                                    Pedido.TipoComissao.Instalador
                                        ? @"
                                    Left Join instalacao i on (p.idPedido=i.idPedido)
                                    Left Join equipe_instalacao ei On (i.idInstalacao=ei.idInstalacao)
                                    Left Join func_equipe fe on (ei.idEquipe=fe.idEquipe)"
                                    : "") + @"

                                WHERE ((pc.ValorPagar=pc.ValorPago) OR (pc.ValorPagar-" + TOLERANCIA_VALORES_PAGAR_PAGO + @">pc.ValorPago) 
                                    OR (pc.ValorPagar+" + TOLERANCIA_VALORES_PAGAR_PAGO + @"<pc.ValorPago)) AND pc.ValorPagar > 0" +
                                    (tipoFunc !=
                                    Pedido.TipoComissao.Todos
                                        ? filtroComissionado
                                        : "");

                            var idsPedido = ExecuteMultipleScalar<string>(session, sqlIdsPedidos, GetParamComissao(dataIni, dataFim));

                            if (idsPedido.Count > 0)
                                filtro += " And p.idPedido in (" + string.Join(",", idsPedido.ToArray()) + ")";
                            else
                                filtro += " And false";
                        }
                        else
                            filtro += string.Format(" AND p.IdPedido IN ({0})", idsPedidos);
                    }
                }

                string filtroComissaoPaga = @" 
                    select distinct p.idPedido 
                    from pedido p 
                        inner join pedido_comissao pc On (p.idPedido=pc.idPedido)
                        left join produtos_liberar_pedido plp on (p.idPedido=plp.idPedido)
                        left join liberarpedido lp on (plp.idLiberarPedido=lp.idLiberarPedido)";

                if (tipoFunc == Pedido.TipoComissao.Todos || tipoFunc == Pedido.TipoComissao.Instalador)
                    filtroComissaoPaga += @"
                        Left Join instalacao i on (p.idPedido=i.idPedido)
                        Left Join equipe_instalacao ei On (i.idInstalacao=ei.idInstalacao)
                        Left Join func_equipe fe on (ei.idEquipe=fe.idEquipe)";

                filtroComissaoPaga += " Where ((pc.ValorPagar=pc.ValorPago) OR (pc.ValorPagar-" + TOLERANCIA_VALORES_PAGAR_PAGO + @">pc.ValorPago)
                    OR (pc.ValorPagar+" + TOLERANCIA_VALORES_PAGAR_PAGO + @"<pc.ValorPago)) AND pc.ValorPagar > 0";

                if (!String.IsNullOrEmpty(dataIni))
                {
                    if (tipoFunc == Pedido.TipoComissao.Instalador)
                        filtroComissaoPaga += " and i." + (Configuracoes.ComissaoConfig.UsarComissaoPorProdutoInstalado ? "DataInstalacao" : "dataFinal") + ">=?dataIni";
                    else if (!PedidoConfig.LiberarPedido)
                        filtroComissaoPaga += " and p.DataConf>=?dataIni";
                }

                if (!String.IsNullOrEmpty(dataFim))
                {
                    if (tipoFunc == Pedido.TipoComissao.Instalador)
                        filtroComissaoPaga += " and i." + (Configuracoes.ComissaoConfig.UsarComissaoPorProdutoInstalado ? "DataInstalacao" : "dataFinal") + "<=?dataFim";
                    else if (!PedidoConfig.LiberarPedido)
                        filtroComissaoPaga += " and p.DataConf<=?dataFim";
                }

                if (!String.IsNullOrEmpty(idsLiberacao))
                    filtroComissaoPaga += " and lp.idLiberarPedido in (" + idsLiberacao + ")";

                if (tipoFunc == Pedido.TipoComissao.Funcionario)
                    filtroComissaoPaga += " And pc.IdFunc" + (idFunc > 0 ? "=" + idFunc : " is not null");

                // Busca pelos pedidos
                if (!String.IsNullOrEmpty(idsPedidos))
                    filtroComissaoPaga += " and p.idPedido in (" + idsPedidos + ")";
                else if (idPedido > 0)
                    filtroComissaoPaga += " and p.idPedido=" + idPedido;

                // Inclui os filtros passados por parâmetro neste sub-sql, exceto os referentes à liberação, os quais serão 
                // tratado logo abaixo
                filtroComissaoPaga += filtroFunc + " " + filtro.ToLower();

                if (PedidoConfig.LiberarPedido)
                    filtroComissaoPaga += @" And lp.situacao=" + (int)LiberarPedido.SituacaoLiberarPedido.Liberado;

                // Substitui os pedidos indicados pelos pedidos encontrados
                idsPedidos = GetValoresCampo(session, filtroComissaoPaga, "idPedido", GetParamComissao(dataIni, dataFim));
                /* Chamado 26319. */
                idsPedidos = string.IsNullOrEmpty(idsPedidos) || string.IsNullOrWhiteSpace(idsPedidos) ? "0" : idsPedidos;
            }
            else
            {
                string whereComissaoConfig = @"faixaUm < p.total or faixaDois < p.total 
                    or faixaTres < p.total or faixaQuatro < p.total or faixaCinco < p.total";

                switch (tipoFunc)
                {
                    case Pedido.TipoComissao.Funcionario:
                        /* Chamado 36378. */
                        if (PedidoConfig.Comissao.PerComissaoPedido)
                            filtro += @" AND (p.PercentualComissao>0 OR
                                p.IdFunc IN (SELECT cg1.IdFunc FROM comissao_config cg1 WHERE cg1.PercFaixaUm>0) OR
                                (SELECT COUNT(*) FROM comissao_config cg1 WHERE cg1.IdFunc IS NULL AND cg1.PercFaixaUm>0) > 0)";
                        else
                            filtro += string.Format(@" AND (p.IdFunc IN (SELECT IdFunc FROM comissao_config WHERE {0}) OR
                                (SELECT COUNT(*) FROM comissao_config WHERE IdFunc IS NULL AND ({0})) > 0 OR
                                (p.IdFunc=c.IdFunc AND c.PercComissaoFunc > 0))", whereComissaoConfig);
                        break;

                    case Pedido.TipoComissao.Comissionado:
                        filtro += " and p.valorComissao > 0";
                        break;

                    case Pedido.TipoComissao.Instalador:
                        {
                            filtro += string.Format(@" AND (fe.idFunc IN (select idFunc from comissao_config where {0}) 
                                    OR (select count(*) from comissao_config where idFunc is null and({0})) > 0)", whereComissaoConfig);

                            if (!Configuracoes.ComissaoConfig.UsarComissaoPorProdutoInstalado)
                                filtro += " and " + InstalacaoDAO.Instance.SqlFinalizadaByPedido(session, "p.idPedido", false) + @" ";

                            break;
                        }
                }

                if (!String.IsNullOrEmpty(dataIni))
                {
                    if (tipoFunc == Pedido.TipoComissao.Instalador)
                        filtro += " and i." + (Configuracoes.ComissaoConfig.UsarComissaoPorProdutoInstalado ? "DataInstalacao" : "dataFinal") + ">=?dataIni";
                    else if (!PedidoConfig.LiberarPedido)
                        filtro += " and p.DataConf>=?dataIni";
                }

                if (!String.IsNullOrEmpty(dataFim))
                {
                    if (tipoFunc == Pedido.TipoComissao.Instalador)
                        filtro += " and i." + (Configuracoes.ComissaoConfig.UsarComissaoPorProdutoInstalado ? "DataInstalacao" : "dataFinal") + "<=?dataFim";
                    else if (!PedidoConfig.LiberarPedido)
                        filtro += " and p.DataConf<=?dataFim";
                }

                if (!String.IsNullOrEmpty(idsLiberacao))
                    filtro += " and lp.idLiberarPedido in (" + idsLiberacao + ")";
            }

            if (!String.IsNullOrEmpty(idsPedidos))
                filtro += " and p.idPedido in (" + idsPedidos + ")";
            else if (idPedido > 0)
                filtro += " and p.idPedido=" + idPedido;

            // Estes dois filtros por data de liberação devem ficar do lado de fora do filtro que retorna os ids dos pedidos,
            // o motivo disso é buscar somente o valor liberado no período que se deseja pagar a comissão, caso sejam retirados,
            // ao gerar a comissão de outubro de um pedido liberado metade em outubro e metade em novembro por exemplo, a BC de comissão
            // buscará tudo que foi liberado deste pedido, inclusive de novembro.
            if (!String.IsNullOrEmpty(idsLiberacao))
                sql += " and lp.idLiberarPedido in (" + idsLiberacao + ")";

            if (tipoFunc == Pedido.TipoComissao.Gerente)
                filtro += string.Format(" AND p.IdLoja IN (SELECT IdLoja FROM comissao_config_gerente WHERE IdFuncionario = {0})", idFunc);

            sql += filtro;

            string groupBy = (PedidoConfig.LiberarPedido && !selecaoIds) || (!selecaoIds && tipoFunc == Pedido.TipoComissao.Instalador) ? " Group By p.idPedido" : "";
            string orderBy = " Order By " + (tipoFunc == Pedido.TipoComissao.Instalador ? "fe.idFunc, " : "") + "p.DataConf";

            sql += filtroFunc + groupBy + orderBy;
            return sql;
        }

        /// <summary>
        /// Retorna o sql para recuperar a base de calculo da comissão de instalações efetuadas
        /// </summary>
        private string SqlTotalBaseCalcComissaoInstalacao(GDASession session, string idsPedidos)
        {
            if (string.IsNullOrWhiteSpace(idsPedidos))
                return string.Empty;

            var sql = string.Empty;

            if (!PedidoConfig.RatearDescontoProdutos)
            {
                var sqls = new List<string>();
                sql = @"SELECT pi.IdPedido, (SUM(QtdeInstalada) * (((pp.Total + pp.ValorBenef) / pp.Qtde) - (((pp.Total + pp.ValorBenef) / pp.Qtde) * {1}))) AS BaseCalc
                    FROM produtos_instalacao pi
	                    INNER JOIN produtos_pedido pp ON (pi.IdProdPed = pp.IdProdPed)
                    WHERE pi.IdPedido = {0} AND pp.IdProdPedParent IS NULL
                    GROUP BY pi.IdProdPed";

                foreach (var idPedido in idsPedidos.Split(',').Select(f => f.StrParaUint()))
                {
                    var usarEspelho = PedidoEspelhoDAO.Instance.ExisteEspelho(session, idPedido);
                    var descontoPedido = usarEspelho ? PedidoEspelhoDAO.Instance.GetDescontoPedido(session, idPedido) : GetDescontoPedido(session, idPedido);
                    var fastDelivery = (decimal)ObtemTaxaFastDelivery(session, idPedido);
                    fastDelivery = fastDelivery > 0 ? fastDelivery : 1;
                    var totalSemDesconto = usarEspelho ? PedidoEspelhoDAO.Instance.GetTotalSemDesconto(session, idPedido, PedidoEspelhoDAO.Instance.GetTotal(session, idPedido) / fastDelivery) :
                        GetTotalSemDesconto(session, idPedido, GetTotal(session, idPedido) / fastDelivery);

                    sqls.Add(string.Format(sql, idPedido, (descontoPedido / totalSemDesconto).ToString().Replace(",", ".")));
                }

                sql = string.Format("SELECT CAST(CONCAT(IdPedido, ';', SUM(BaseCalc)) AS CHAR) FROM ({0}) AS tmp GROUP BY IdPedido", string.Join(" UNION ALL ", sqls));
            }
            else
                sql = string.Format(@"SELECT CAST(CONCAT(IdPedido, ';', SUM(BaseCalc)) AS CHAR)
                    FROM (SELECT pi.IdPedido, (SUM(QtdeInstalada) * ((pp.Total + pp.ValorBenef) / pp.Qtde)) AS BaseCalc
                        FROM produtos_instalacao pi
	                        INNER JOIN produtos_pedido pp ON (pi.IdProdPed = pp.IdProdPed)
                        WHERE pi.IdPedido IN ({0}) AND pp.IdProdPedParent IS NULL
                        GROUP BY pi.IdProdPed
                    ) AS tmp
                    GROUP BY IdPedido", idsPedidos);

            return sql;
        }

        /// <summary>
        /// Atualiza a data de entrefa do pedido
        /// </summary>
        /// <param name="session"></param>
        /// <param name="pedido"></param>
        /// <param name="idPedido"></param>
        public void AtualizarDataEntregaCalculada(GDASession session, Pedido pedido, uint idPedido)
        {
            DateTime dataEntrega, dataFastDelivery;
            var desabilitarCampo = false;

            // Calcula a data de entrega mínima.
            if (GetDataEntregaMinima(session, pedido.IdCli, pedido.IdPedido, pedido.TipoPedido, pedido.TipoEntrega,
                pedido.DataPedido, out dataEntrega, out dataFastDelivery, out desabilitarCampo) || !pedido.DataEntrega.HasValue)
            {
                /* Chamado 49811. */
                pedido.DataEntrega = pedido.FastDelivery ? dataFastDelivery : pedido.IdProjeto > 0 ? dataEntrega : pedido.DataEntrega.HasValue ? pedido.DataEntrega : dataEntrega;
                objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido SET DataEntrega=?dataEntrega WHERE IdPedido={0}",
                    pedido.IdPedido), new GDAParameter("?dataEntrega", pedido.DataEntrega));

                // Caso a data entrega do pedido tenha sido alterada na inserção do pedido, salva log da alteração.
                if (pedido.DataEntrega.Value.Date != dataEntrega.Date)
                {
                    var logData = new LogAlteracao();
                    logData.Tabela = (int)LogAlteracao.TabelaAlteracao.Pedido;
                    logData.IdRegistroAlt = (int)idPedido;
                    logData.NumEvento = LogAlteracaoDAO.Instance.GetNumEvento(session, LogAlteracao.TabelaAlteracao.Pedido, (int)pedido.IdPedido);
                    logData.Campo = "Data Entrega";
                    logData.DataAlt = DateTime.Now;
                    logData.IdFuncAlt = UserInfo.GetUserInfo != null ? UserInfo.GetUserInfo.CodUser : 0;
                    logData.ValorAnterior = dataEntrega != null ? dataEntrega.ToString() : null;
                    logData.ValorAtual = pedido.DataEntrega != null ? pedido.DataEntrega.ToString() : null;
                    logData.Referencia = LogAlteracao.GetReferencia(session, (int)LogAlteracao.TabelaAlteracao.Pedido, pedido.IdPedido);

                    LogAlteracaoDAO.Instance.Insert(session, logData);
                }
            }
        }

        #endregion

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="tipoFunc"></param>
        /// <param name="idComissao"></param>
        /// <param name="ped"></param>
        private void GetCamposComissao(Pedido.TipoComissao tipoFunc, uint idComissao, ref List<Pedido> ped)
        {
            GetCamposComissao(null, tipoFunc, idComissao, ref ped);
        }

        private void GetCamposComissao(GDASession session, Pedido.TipoComissao tipoFunc, uint idComissao, ref List<Pedido> ped)
        {
            if (ped.Count == 0)
                return;

            List<string> pedidos = new List<string>(), inst = new List<string>();

            string idsPedidos = String.Join(",", Array.ConvertAll(ped.ToArray(), x => x.IdPedido.ToString()));
            var recebidas = ContasReceberDAO.Instance.ExisteRecebida(session, idsPedidos, true);

            var dicFuncPedido = new Dictionary<uint, Glass.Data.Model.ComissaoConfig>();

            // Salva os ids dos pedidos e dos instaladores em 2 listas
            foreach (Pedido p in ped)
            {
                if (tipoFunc == Pedido.TipoComissao.Funcionario)
                {
                    if (!dicFuncPedido.ContainsKey(p.IdFunc))
                        dicFuncPedido.Add(p.IdFunc, ComissaoConfigDAO.Instance.GetComissaoConfig(session, p.IdFunc));

                    p.ComissaoConfig = dicFuncPedido[p.IdFunc];
                }

                if (!pedidos.Contains(p.IdPedido.ToString()))
                {
                    p.TemRecebimento = (recebidas.ContainsKey(p.IdPedido) && recebidas[p.IdPedido]) ||
                        p.IdPagamentoAntecipado > 0 || p.IdSinal > 0 ||
                        (p.IdLiberarPedido > 0 && LiberarPedidoDAO.Instance.ObtemTipoPagto(session, p.IdLiberarPedido.Value) == (int)LiberarPedido.TipoPagtoEnum.AVista);

                    /* if (Config.LiberarPedido && !p.TemRecebimento && (p.Situacao == Pedido.SituacaoPedido.LiberadoParcialmente ||
                        p.Situacao == Pedido.SituacaoPedido.Confirmado))
                    {
                        foreach (var idLib in LiberarPedidoDAO.Instance.GetIdsLiberacaoByPedido(sessao, p.IdPedido))
                            if (ContasReceberDAO.Instance.ObtemValorCampo<int>(sessao, "Count(*)", "idLiberarPedido=" + idLib + " And Coalesce(Recebida, False)") > 0)
                            {
                                p.TemRecebimento = true;
                                break;
                            }
                    } */

                    pedidos.Add(p.IdPedido.ToString());
                }

                if (p.IdInstalador > 0 && !inst.Contains(p.IdInstalador.ToString()))
                    inst.Add(p.IdInstalador.ToString());
            }

            // Junta os ids dos pedidos em uma string
            string ids = String.Join(",", pedidos.ToArray());

            Dictionary<uint, decimal> parc = new Dictionary<uint, decimal>();
            Dictionary<uint, decimal> crf = new Dictionary<uint, decimal>();
            Dictionary<uint, decimal> crc = new Dictionary<uint, decimal>();
            Dictionary<uint, string> nomeCom = new Dictionary<uint, string>();
            Dictionary<KeyValuePair<uint, uint>, decimal> cri = new Dictionary<KeyValuePair<uint, uint>, decimal>();
            Dictionary<uint, decimal> vpc = new Dictionary<uint, decimal>();
            var valorTotalInstalado = new Dictionary<uint, decimal>();

            string result;

            //Busca os valores da base de calculo quando a comissao e gerada por produto instalado
            if (Configuracoes.ComissaoConfig.UsarComissaoPorProdutoInstalado)
            {
                var pedValores = ExecuteMultipleScalar<string>(session, SqlTotalBaseCalcComissaoInstalacao(session, ids));

                valorTotalInstalado = pedValores
                    .Where(f => !string.IsNullOrEmpty(f))
                    .Select(f => new
                    {
                        idPedido = f.Split(';')[0].StrParaUint(),
                        BaseCalc = f.Split(';')[1].StrParaDecimal()
                    })
                    .ToDictionary(f => f.idPedido, f => f.BaseCalc);
            }

            // Busca o valor já pago ao funcionário de cada pedido
            if (tipoFunc == Pedido.TipoComissao.Todos || tipoFunc == Pedido.TipoComissao.Funcionario)
            {
                result = GetValoresCampo(session, SqlTotalComissaoPago(ids, Pedido.TipoComissao.Funcionario, 0),
                    "cast(concat(idPedido, '|', valor) as char)");

                foreach (string s in (result != null ? result : "").Split(','))
                {
                    if (String.IsNullOrEmpty(s))
                        continue;

                    string[] d = s.Split('|');
                    crf.Add(Glass.Conversoes.StrParaUint(d[0]), Glass.Conversoes.StrParaDecimal(d[1]));
                }
            }

            // Busca o valor já pago os comissionados de cada pedido
            if (tipoFunc == Pedido.TipoComissao.Todos || tipoFunc == Pedido.TipoComissao.Comissionado)
            {
                result = GetValoresCampo(session, SqlTotalComissaoPago(ids, Pedido.TipoComissao.Comissionado, 0),
                    "cast(concat(idPedido, '|', valor) as char)");

                foreach (string s in (result != null ? result : "").Split(','))
                {
                    if (String.IsNullOrEmpty(s))
                        continue;

                    string[] d = s.Split('|');
                    crc.Add(Glass.Conversoes.StrParaUint(d[0]), Glass.Conversoes.StrParaDecimal(d[1]));
                }
            }

            // Busca o valor já pago aos instaladores de cada pedido
            if (tipoFunc == Pedido.TipoComissao.Todos || tipoFunc == Pedido.TipoComissao.Instalador)
                foreach (string i in inst)
                {
                    uint idInst = Glass.Conversoes.StrParaUint(i);
                    if (idInst == 0)
                        continue;

                    result = GetValoresCampo(session, SqlTotalComissaoPago(ids, Pedido.TipoComissao.Instalador, idInst),
                        "cast(concat(idPedido, '|', valor) as char)");

                    foreach (string s in (result != null ? result : "").Split(','))
                    {
                        if (String.IsNullOrEmpty(s))
                            continue;

                        string[] d = s.Split('|');
                        cri.Add(new KeyValuePair<uint, uint>(Glass.Conversoes.StrParaUint(d[0]), idInst), Glass.Conversoes.StrParaDecimal(d[1]));
                    }
                }

            // Busca o valor pago de uma comissão para cada pedido
            string sqlVpc = @"select idPedido, sum(valor) as valor
                from comissao_pedido
                where idPedido in (" + ids + ")";

            if (idComissao > 0)
                sqlVpc += " and idComissao=" + idComissao;
            else if (tipoFunc != Pedido.TipoComissao.Todos)
            {
                string campo = tipoFunc == Pedido.TipoComissao.Funcionario ? "idFunc" :
                    tipoFunc == Pedido.TipoComissao.Comissionado ? "idComissionado" :
                    tipoFunc == Pedido.TipoComissao.Instalador ? "idInstalador" :
                    tipoFunc == Pedido.TipoComissao.Gerente ? "idGerente" : "";

                sqlVpc += " and idComissao in (select idComissao from comissao where " + campo + " is not null)";
            }

            sqlVpc += " group by idPedido";

            result = GetValoresCampo(session, sqlVpc, "cast(concat(idPedido, '|', valor) as char)");

            foreach (string s in (result != null ? result : "").Split(','))
            {
                if (String.IsNullOrEmpty(s))
                    continue;

                string[] d = s.Split('|');
                vpc.Add(Glass.Conversoes.StrParaUint(d[0]), Glass.Conversoes.StrParaDecimal(d[1]));
            }

            // Preenche os pedidos com os dados
            foreach (Pedido p in ped)
            {
                if (parc.ContainsKey(p.IdPedido))
                    p.TotalParcelasRecebidas = parc[p.IdPedido];

                if (crf.Count > 0 && crf.ContainsKey(p.IdPedido))
                    p.ValorComissaoRecebidaFunc = crf[p.IdPedido];

                if (crc.Count > 0 && crc.ContainsKey(p.IdPedido))
                    p.ValorComissaoRecebidaComissionado = crc[p.IdPedido];

                if (cri.Count > 0)
                    foreach (string i in inst)
                    {
                        KeyValuePair<uint, uint> chave = new KeyValuePair<uint, uint>(p.IdPedido, p.IdInstalador.GetValueOrDefault());
                        if (cri.ContainsKey(chave))
                            p.ValorComissaoRecebidaInstalador = cri[chave];
                    }

                if (vpc.Count > 0 && vpc.ContainsKey(p.IdPedido))
                    p.ValorPagoComissao = vpc[p.IdPedido];

                if (valorTotalInstalado.ContainsKey(p.IdPedido))
                    p.TotalParaComissaoProdutoInstalado = valorTotalInstalado[p.IdPedido];
            }
        }

        internal Pedido GetElementComissao(uint idPedido, Pedido.TipoComissao comissaoFuncionario)
        {
            return GetElementComissao(null, idPedido, comissaoFuncionario);
        }

        internal Pedido GetElementComissao(GDASession session, uint idPedido, Pedido.TipoComissao comissaoFuncionario)
        {
            List<Pedido> item = objPersistence.LoadData(session, SqlComissao(session, null, null, idPedido, comissaoFuncionario, 0, null, null, true, true, null, 0));
            GetCamposComissao(session, comissaoFuncionario, 0, ref item);
            return item.Count > 0 ? item[0] : null;
        }

        /// <summary>
        /// Retorna os IDs dos funcionarios/comissionados/instaladores para comissão.
        /// </summary>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        public string GetPedidosIdForComissao(Pedido.TipoComissao tipoFunc, uint idFunc, string dataIni, string dataFim)
        {
            string campo = tipoFunc == Pedido.TipoComissao.Funcionario || tipoFunc == Pedido.TipoComissao.Gerente ? "p.idFunc" :
                tipoFunc == Pedido.TipoComissao.Comissionado ? "p.idComissionado" :
                tipoFunc == Pedido.TipoComissao.Instalador ? "fe.idFunc" : "";

            string retorno = GetValoresCampo(SqlComissao(null, null, 0, tipoFunc, idFunc, dataIni,
                dataFim, true, false, campo, 0), "id", GetParamComissao(dataIni, dataFim));

            return retorno != String.Empty ? retorno : "0";
        }

        /// <summary>
        /// Busca pedidos que ainda não foi pago a comissão
        /// </summary>
        /// <param name="tipoFunc">0-Funcionário, 1-Comissionado, 2-Instalador</param>
        /// <param name="idFunc"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public Pedido[] GetPedidosForComissao(Pedido.TipoComissao tipoFunc, uint idFunc, string dataIni, string dataFim, bool isRelatorio, uint idLoja, bool? comRecebimento, string tiposVenda)
        {
            return GetPedidosForComissao(tipoFunc, idFunc, dataIni, dataFim, isRelatorio, "0", idLoja, comRecebimento, tiposVenda);
        }

        /// <summary>
        /// Busca pedidos que ainda não foi pago a comissão
        /// </summary>
        /// <param name="tipoFunc">0-Funcionário, 1-Comissionado, 2-Instalador</param>
        /// <param name="idFunc"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <param name="tiposPedidos"></param>
        /// <returns></returns>
        public Pedido[] GetPedidosForComissao(Pedido.TipoComissao tipoFunc, uint idFunc, string dataIni, string dataFim, bool isRelatorio, string tiposPedidos, string tiposVenda)
        {
            return GetPedidosForComissao(tipoFunc, idFunc, dataIni, dataFim, isRelatorio, tiposPedidos, 0, null, tiposVenda);
        }

        /// <summary>
        /// Busca pedidos para o relatório de comissão
        /// </summary>
        /// <param name="tipoFunc">0-Funcionário, 1-Comissionado, 2-Instalador</param>
        /// <param name="idFunc"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <param name="tiposPedidos"></param>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public Pedido[] GetPedidosForComissao(Pedido.TipoComissao tipoFunc, uint idFunc, string dataIni, string dataFim, bool isRelatorio,
            string tiposPedidos, uint idLoja, bool? comRecebimento, string tiposVenda)
        {
            /* Chamado 47577. */
            if (idFunc == 0 && !isRelatorio)
                return new List<Pedido>().ToArray();

            List<string> tipoPed = new List<string>((tiposPedidos != null ? tiposPedidos : "").Split(','));

            // Atualiza o valor da comissão para os pedidos, se necessário
            if (tipoPed.Contains("0"))
                CriaComissaoFuncionario(tipoFunc, idFunc, dataIni, dataFim, tiposVenda);

            string criterio = "Período: " + dataIni + " a " + dataFim + "    Tipo: " + (tipoFunc == Pedido.TipoComissao.Funcionario ? "Funcionário" :
                tipoFunc == Pedido.TipoComissao.Comissionado ? "Comissionado" : "Instalador");

            if (idFunc > 0)
            {
                criterio += "    Nome: " + (tipoFunc == Pedido.TipoComissao.Funcionario || tipoFunc == Pedido.TipoComissao.Instalador ?
                    FuncionarioDAO.Instance.GetNome((uint)idFunc) : ComissionadoDAO.Instance.GetNome((uint)idFunc));
            }

            if (idLoja > 0)
                criterio += "     Loja: " + LojaDAO.Instance.GetNome(idLoja);

            string sql = SqlComissao(null, null, 0, tipoFunc, idFunc, dataIni, dataFim, false, tipoPed.Contains("1"), null, idLoja, tiposVenda).Replace("$$$", criterio);

            List<Pedido> retorno = objPersistence.LoadData(sql, GetParamComissao(dataIni, dataFim)).ToList();
            GetCamposComissao(tipoFunc, 0, ref retorno);

            if (tipoFunc == Pedido.TipoComissao.Gerente)
            {
                for (int i = retorno.Count - 1; i >= 0; i--)
                {
                    var comissaoPedido = PedidoComissaoDAO.Instance.GetByPedidoFunc(null, retorno[i].IdPedido, 3, idFunc, true);

                    if (comissaoPedido.ValorPago == comissaoPedido.ValorPagar)
                    {
                        retorno.RemoveAt(i);
                    }
                    else
                    {
                        retorno[i].TemRecebimento = (comissaoPedido.ValorPago > 0);
                        retorno[i].ValorComissaoGerentePagar = comissaoPedido.ValorPagar;
                        retorno[i].ValorComissaoGerentePago = comissaoPedido.ValorPago;
                    }
                }
            }
            else
                retorno.RemoveAll(f =>
                    (!tipoPed.Contains("1") && !f.ComissaoAPagar) ||
                    (!tipoPed.Contains("0") && f.ComissaoAPagar));

            if (comRecebimento.HasValue)
                retorno = retorno.Where(f => f.TemRecebimento == comRecebimento.Value).ToList();

            return retorno.ToArray();
        }

        public void CriaComissaoFuncionario(Pedido.TipoComissao tipoFunc, uint idFunc, string dataIni, string dataFim, string tiposVenda)
        {
            var semComissao = objPersistence.LoadData(SqlComissao(null, null, 0, tipoFunc, idFunc, dataIni,
                dataFim, true, false, null, 0, tiposVenda), GetParamComissao(dataIni, dataFim)).ToList();

            GetCamposComissao(tipoFunc, 0, ref semComissao);

            /* Chamado 48565. */
            if (tipoFunc == Pedido.TipoComissao.Funcionario)
                PedidoComissaoDAO.Instance.CriarPedidoComissaoPorPedidosEFuncionario(null, semComissao, (int)idFunc, tipoFunc);
            else
                PedidoComissaoDAO.Instance.Create(semComissao, tipoFunc);
        }

        /// <summary>
        /// Busca pedidos de uma comissão.
        /// </summary>
        /// <param name="idComissao"></param>
        /// <param name="tipoFunc"></param>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        public Pedido[] GetPedidosByComissao(uint idComissao, Pedido.TipoComissao tipoFunc, uint idFunc)
        {
            string sql = SqlComissao(idComissao.ToString(), null, 0, tipoFunc, idFunc, null, null, true, false, null, 0);
            List<Pedido> ped = objPersistence.LoadData(sql);

            GetCamposComissao(tipoFunc, idComissao, ref ped);
            return ped.ToArray();
        }

        /// <summary>
        /// Busca os pedidos de uma comissão.
        /// </summary>
        /// <param name="idComissao"></param>
        /// <param name="tipoFunc"></param>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        public Pedido GetPedidosByComissao(uint idPedido, uint idComissao, Pedido.TipoComissao tipoFunc, uint idFunc)
        {
            string sql = SqlComissao(idComissao.ToString(), null, idPedido, tipoFunc, idFunc, null, null, true, false, null, 0);
            List<Pedido> ped = objPersistence.LoadData(sql);

            GetCamposComissao(tipoFunc, idComissao, ref ped);
            return ped.Count > 0 ? ped[0] : null;
        }

        internal GDAParameter[] GetParamComissao(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Reposição

        public uint? IdReposicao(uint idPedido)
        {
            var sql = string.Format("SELECT IdPedido FROM pedido WHERE IdPedidoAnterior={0}", idPedido);
            var retorno = objPersistence.ExecuteScalar(sql);

            return retorno != null ? retorno.ToString().StrParaUintNullable() : null;
        }
        /// <summary>
        /// Verifica se o pedido Possui IdPedidoAnterior e se o pedido não está cancelado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public uint? ObterIdPedidoAnterior(uint idPedido)
        {
            var sql = string.Format("SELECT IdPedido FROM pedido WHERE IdPedidoAnterior={0} AND situacao <> {1}", idPedido, (int)Pedido.SituacaoPedido.Cancelado);
            return ExecuteScalar<uint?>(sql);
        }

        /// <summary>
        /// Verifica se entre os pedidos passados existe algum de reposição
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool ContemPedidoReposicao(GDASession sessao, string idPedido)
        {
            if (String.IsNullOrEmpty(idPedido))
                return false;

            string sql = "select count(*) from pedido where idPedido In (" + idPedido.TrimEnd(',') + ") and tipoVenda=" + (int)Pedido.TipoVendaPedido.Reposição;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Verifica se os pedidos passados são de reposição
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsPedidoReposicao(string idPedido)
        {
            return IsPedidoReposicao(null, idPedido);
        }

        /// <summary>
        /// Verifica se os pedidos passados são de reposição
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsPedidoReposicao(GDASession sessao, string idPedido)
        {
            if (String.IsNullOrEmpty(idPedido))
                return false;

            string sql = "select count(*) from pedido where idPedido In (" + idPedido.TrimEnd(',') + ") and tipoVenda=" + (int)Pedido.TipoVendaPedido.Reposição;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) == idPedido.TrimEnd(',').Split(',').Length;
        }

        /// <summary>
        /// Verifica se este pedido possui alguma reposição
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsPedidoReposto(GDASession sessao, uint idPedido)
        {
            if (idPedido == 0)
                return false;

            string sql = "select count(*) from pedido where idPedidoAnterior=" + idPedido;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Verifica se o pedido está marcado para gerar pedido de produção de corte
        /// </summary>
        public bool GerarPedidoProducaoCorte(GDASession sessao, uint idPedido)
        {
            if (idPedido == 0)
                return false;

            string sql = "select GerarPedidoProducaoCorte from pedido where idPedido=" + idPedido;

            return (bool)objPersistence.ExecuteScalar(sessao, sql);
        }

        /// <summary>
        /// Verifica se o pedido infomrado e um pedido de produção para corte
        /// </summary>
        public bool IsPedidoProducaoCorte(GDASession sessao, uint idPedido)
        {
            if (idPedido == 0)
                return false;

            string sql = "SELECT COUNT(*) FROM pedido WHERE IdPedidoRevenda IS NOT NULL AND TipoPedido = " + (int)Glass.Data.Model.Pedido.TipoPedidoEnum.Producao + " AND IdPedido=" + idPedido;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Verifica se este pedido possui alguma reposição
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsPedidoReposto(uint idPedido)
        {
            return IsPedidoReposto(null, idPedido);
        }

        /// <summary>
        /// Verifica se no pedido foi expedido box
        /// </summary>
        public bool IsPedidoExpedicaoBox(uint idPedido)
        {
            return IsPedidoExpedicaoBox(null, idPedido);
        }

        /// <summary>
        /// Verifica se no pedido foi expedido box
        /// </summary>
        public bool IsPedidoExpedicaoBox(GDASession session, uint idPedido)
        {
            if (idPedido == 0)
                return false;

            var sql =
                string.Format(
                    @"SELECT * FROM
                        (SELECT COUNT(*) FROM produto_pedido_producao WHERE IdPedidoExpedicao={0})
                    AS temp;", idPedido);

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        /// <summary>
        /// Verifica se os pedidos passados são de garantia
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsPedidoGarantia(string idPedido)
        {
            return IsPedidoGarantia(null, idPedido);
        }

        /// <summary>
        /// Verifica se os pedidos passados são de garantia
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsPedidoGarantia(GDASession session, string idPedido)
        {
            if (String.IsNullOrEmpty(idPedido))
                return false;

            string sql = "select count(*) from pedido where idPedido In (" + idPedido.TrimEnd(',') + ") and tipoVenda=" + (int)Pedido.TipoVendaPedido.Garantia;

            return objPersistence.ExecuteSqlQueryCount(session, sql) == idPedido.TrimEnd(',').Split(',').Length;
        }

        /// <summary>
        /// Verifica se os pedidos passados são vendas para funcionários
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsPedidoFuncionario(string idPedido)
        {
            if (String.IsNullOrEmpty(idPedido))
                return false;

            string sql = "select count(*) from pedido where idPedido In (" + idPedido.TrimEnd(',') + ") and tipoVenda=" + (int)Pedido.TipoVendaPedido.Funcionario;

            return objPersistence.ExecuteSqlQueryCount(sql) == idPedido.TrimEnd(',').Split(',').Length;
        }

        #endregion

        #region Atualiza o peso dos produtos e do pedido

        /// <summary>
        /// Atualiza o peso dos produtos e do pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        public void AtualizaPeso(uint idPedido)
        {
            AtualizaPeso(null, idPedido);
        }

        /// <summary>
        /// Atualiza o peso dos produtos e do pedido.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        public void AtualizaPeso(GDASession sessao, uint idPedido)
        {
            string sql = @"
                UPDATE produtos_pedido pp
                    LEFT JOIN 
                    (
                        " + Utils.SqlCalcPeso(Utils.TipoCalcPeso.ProdutoPedido, idPedido, false, false, false) + @"
                    ) as peso on (pp.idProdPed=peso.id)
                    INNER JOIN produto prod ON (pp.idProd = prod.idProd)
                    LEFT JOIN subgrupo_prod sgp ON (prod.idSubGrupoProd = sgp.idSubGrupoProd)
                    LEFT JOIN 
                    (
                        SELECT pp1.IdProdPedParent, sum(pp1.peso) as peso
                        FROM produtos_pedido pp1
                        GROUP BY pp1.IdProdPedParent
                    ) as pesoFilhos ON (pp.IdProdPed = pesoFilhos.IdProdPedParent)
                SET pp.peso = coalesce(IF(sgp.TipoSubgrupo IN (" + (int)TipoSubgrupoProd.VidroDuplo + "," + (int)TipoSubgrupoProd.VidroLaminado + @"), pesoFilhos.peso * pp.Qtde, peso.peso), 0)
                WHERE pp.idPedido={0};

                UPDATE pedido 
                SET peso = coalesce((SELECT sum(peso) FROM produtos_pedido WHERE coalesce(IdProdPedParent, 0) = 0 AND idPedido={0} and !coalesce(invisivelPedido, false)), 0) 
                WHERE idPedido = {0}";

            objPersistence.ExecuteCommand(sessao, String.Format(sql, idPedido));
        }

        #endregion

        #region Atualiza a data de entrega do pedido

        /// <summary>
        /// Atualiza a data de entrega do pedido.
        /// </summary>
        public void AtualizarDataEntrega(int idPedido, DateTime dataEntrega)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    AtualizarDataEntrega(transaction, idPedido, dataEntrega);

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        public void AtualizarDataEntrega(GDASession sessao, int idPedido, DateTime dataEntrega)
        {
            var pedidoAtual = GetElementByPrimaryKey(sessao, idPedido);

            objPersistence.ExecuteCommand(sessao, string.Format("UPDATE pedido SET DataEntrega=?dataEntrega WHERE IdPedido={0}", idPedido), new GDAParameter("?dataEntrega", dataEntrega));

            LogAlteracaoDAO.Instance.LogPedido(sessao, pedidoAtual, GetElementByPrimaryKey(sessao, idPedido), LogAlteracaoDAO.SequenciaObjeto.Atual);
        }

        #endregion

        #region Atualizar valor total, custo e comissão do pedido

        // Variável de controle do método UpdateTotalPedido
        private static Dictionary<uint, bool> _atualizando = new Dictionary<uint, bool>();

        /// <summary>
        /// Atualiza o valor total do pedido, somando os totais dos produtos relacionados à ele
        /// </summary>
        public void UpdateTotalPedido(uint idPedido)
        {
            UpdateTotalPedido(null, idPedido);
        }

        /// <summary>
        /// Atualiza o valor total do pedido, somando os totais dos produtos relacionados à ele
        /// </summary>
        public void UpdateTotalPedido(Pedido pedido)
        {
            UpdateTotalPedido(null, pedido);
        }

        /// <summary>
        /// Atualiza o valor total do pedido, somando os totais dos produtos relacionados à ele
        /// </summary>
        internal void UpdateTotalPedido(GDASession sessao, uint idPedido)
        {
            var pedido = GetElementByPrimaryKey(sessao, idPedido);
            UpdateTotalPedido(sessao, pedido, false, false, false, true);
        }

        /// <summary>
        /// Atualiza o valor total do pedido, somando os totais dos produtos relacionados à ele
        /// </summary>
        internal void UpdateTotalPedido(GDASession sessao, Pedido pedido)
        {
            UpdateTotalPedido(sessao, pedido, false, false, false, true);
        }


        /// <summary>
        /// Atualiza o percentual de comissão do pedido
        /// </summary>
        private void AtualizarPercentualComissao(GDASession sessao, Pedido pedido, IEnumerable<ProdutosPedido> produtosPedido)
        {
            decimal percComissao = 0;

            if (Glass.Configuracoes.PedidoConfig.Comissao.UsarComissaoPorProduto)
                if (pedido.Total > 0)
                    foreach (var prod in produtosPedido)
                        percComissao += ((prod.Total * 100) / pedido.Total) * (prod.PercComissao / 100);

            var parametros = new List<GDAParameter>();
            parametros.Add(new GDAParameter("?idPedido", pedido.IdPedido));
            parametros.Add(new GDAParameter("?percComissao", Math.Round(percComissao, 2)));
            objPersistence.ExecuteCommand(sessao, "UPDATE pedido SET PercentualComissao=?percComissao WHERE IdPedido=?idPedido", parametros.ToArray());
        }

        /// <summary>
        /// Atualiza o valor total do pedido, somando os totais dos produtos relacionados à ele
        /// </summary>
        internal void UpdateTotalPedido(GDASession sessao, Pedido pedido, bool liberando, bool forcarAtualizacao, bool alterouDesconto,
            bool criarLogDeAlteracao)
        {
            // Verifica se o usuário está atualizando o total
            if (!_atualizando.ContainsKey(UserInfo.GetUserInfo.CodUser))
                _atualizando.Add(UserInfo.GetUserInfo.CodUser, false);

            if (!forcarAtualizacao && _atualizando[UserInfo.GetUserInfo.CodUser])
                return;

            try
            {
                // Define que o usuário está atualizando o total
                _atualizando[UserInfo.GetUserInfo.CodUser] = true;

                // Atualiza o custo do pedido
                UpdateCustoPedido(sessao, pedido.IdPedido);

                // Atualiza total do pedido
                string sql = "update pedido p set Total=(Select Sum(Total + coalesce(valorBenef, 0)) From produtos_pedido Where " +
                    "IdPedido=p.IdPedido and (InvisivelPedido = false or InvisivelPedido is null) AND IdProdPedParent IS NULL) where p.idPedido=" + pedido.IdPedido;

                objPersistence.ExecuteCommand(sessao, sql);

                if (!PedidoConfig.RatearDescontoProdutos)
                {
                    // Atualiza total do pedido
                    sql = @"
                        Update pedido p set Total=Round(
                            Total-if(p.TipoDesconto=1, (p.Total * (p.Desconto / 100)), p.Desconto)-
                            coalesce((
                                Select sum(if(tipoDesconto=1, ((
                                    Select sum(total + coalesce(valorBenef,0)) 
                                    From produtos_pedido 
                                    Where (invisivelPedido=false or invisivelPedido is null) 
                                        And idAmbientePedido=a.idAmbientePedido
                                        AND IdProdPedParent IS NULL) * (desconto / 100)), desconto)) 
                                From ambiente_pedido a 
                                Where idPedido=p.idPedido),0), 2) " +
                        "Where IdPedido=" + pedido.IdPedido;

                    objPersistence.ExecuteCommand(sessao, sql);
                }

                // Verifica se o desconto dado no pedido é permitido, se não for, zera o desconto
                if (!liberando)
                {
                    if (!DescontoPermitido(sessao, pedido))
                        RemoveDescontoNaoPermitido(sessao, pedido);
                    else if (alterouDesconto)
                    {
                        decimal percDesconto = pedido.Desconto;

                        uint idFuncDesc = ObtemIdFuncDesc(sessao, pedido.IdPedido) ?? UserInfo.GetUserInfo.CodUser;

                        if (pedido.TipoDesconto == 2)
                        {
                            percDesconto = Pedido.GetValorPerc(1, pedido.TipoDesconto, percDesconto,
                                GetTotalSemDesconto(sessao, pedido.IdPedido, pedido.Total));
                        }

                        if (percDesconto > (decimal)PedidoConfig.Desconto.GetDescontoMaximoPedido(idFuncDesc, pedido.TipoVenda ?? 0, (int?)pedido.IdParcela))
                            Email.EnviaEmailDescontoMaior(sessao, pedido.IdPedido, 0, idFuncDesc, (float)percDesconto, PedidoConfig.Desconto.GetDescontoMaximoPedido(idFuncDesc, pedido.TipoVenda ?? 0, (int?)pedido.IdParcela));
                    }
                }

                float percFastDelivery = 1;

                // Verifica se há taxa de urgência para o pedido
                if (PedidoConfig.Pedido_FastDelivery.FastDelivery && pedido.FastDelivery)
                {
                    percFastDelivery = 1 + (PedidoConfig.Pedido_FastDelivery.TaxaFastDelivery / 100);
                    sql = "update pedido set taxaFastDelivery=?taxa, Total=Round(Total * " + percFastDelivery.ToString().Replace(',', '.') + ", 2) where IdPedido=" + pedido.IdPedido;

                    objPersistence.ExecuteCommand(sessao, sql, new GDAParameter("?taxa", PedidoConfig.Pedido_FastDelivery.TaxaFastDelivery));
                }

                string descontoRateadoImpostos = "0";
                pedido.Total = GetTotal(sessao, pedido.IdPedido);

                if (!PedidoConfig.RatearDescontoProdutos)
                {
                    var dadosAmbientes = (pedido as IContainerCalculo).Ambientes.Obter(true)
                        .Cast<AmbientePedido>()
                        .Select(x => new { x.IdAmbientePedido, TotalProdutos = x.TotalProdutos + x.ValorDescontoAtual });

                    var formata = new Func<decimal, string>(x => x.ToString().Replace(".", "").Replace(",", "."));

                    decimal totalSemDesconto = GetTotalSemDesconto(sessao, pedido.IdPedido, (pedido.Total / (decimal)percFastDelivery));
                    string selectAmbientes = dadosAmbientes.Count() == 0 ? "select null as idAmbientePedido, 1 as total" :
                        String.Join(" union all ", dadosAmbientes.Select(x =>
                            String.Format("select {0} as idAmbientePedido, {1} as total",
                            x.IdAmbientePedido, formata(x.TotalProdutos))).ToArray());

                    descontoRateadoImpostos = @"(
                        if(coalesce(ped.desconto, 0)=0, 0, if(ped.tipoDesconto=1, ped.desconto / 100, ped.desconto / Greatest(" + formata(totalSemDesconto) + @", 1)) * (pp.total + coalesce(pp.valorBenef, 0)))) - (
                        if(coalesce(ap.desconto, 0)=0, 0, if(ap.tipoDesconto=1, ap.desconto / 100, ap.desconto / (select Greatest(total, 1) from (" + selectAmbientes + @") as amb 
                        where idAmbientePedido=ap.idAmbientePedido)) * (pp.total + coalesce(pp.valorBenef, 0))))";
                }

                // Calcula o valor do ICMS do pedido, utiliza o percentual do fast delivery no cálcul o para que quando for gerar a NF, calcule corretamente
                if (LojaDAO.Instance.ObtemCalculaIcmsPedido(sessao, pedido.IdLoja) && ClienteDAO.Instance.IsCobrarIcmsSt(sessao, pedido.IdCli))
                {
                    var calcIcmsSt = CalculoIcmsStFactory.ObtemInstancia(sessao, (int)pedido.IdLoja, (int)pedido.IdCli, null, null, null, null);

                    string idProd = "pp.idProd";
                    string total = "pp.Total + Coalesce(pp.ValorBenef, 0)";
                    string aliquotaIcmsSt = "pp.AliqIcms";

                    sql = @"
                        Update produtos_pedido pp
                            inner join pedido ped on (pp.idPedido=ped.idPedido)
                            left join ambiente_pedido ap on (pp.idAmbientePedido=ap.idAmbientePedido)
                        {0}
                        Where pp.idPedido=" + pedido.IdPedido + " and (pp.InvisivelPedido = false or pp.InvisivelPedido is null) AND pp.IdProdPedParent IS NULL";

                    // Atualiza a Alíquota ICMSST somada ao FCPST com o ajuste do MVA e do IPI. Necessário porque na tela é recuperado e salvo o valor sem FCPST.
                    objPersistence.ExecuteCommand(sessao, string.Format(sql,
                        "set pp.AliqIcms=(" + calcIcmsSt.ObtemSqlAliquotaInternaIcmsSt(sessao, idProd, total, descontoRateadoImpostos, aliquotaIcmsSt, percFastDelivery.ToString().Replace(',', '.')) + @")"));
                    // Atualiza o valor do ICMSST calculado com a Alíquota recuperada anteriormente.
                    objPersistence.ExecuteCommand(sessao, string.Format(sql,
                        "set pp.ValorIcms=(" + calcIcmsSt.ObtemSqlValorIcmsSt(total, descontoRateadoImpostos, aliquotaIcmsSt, percFastDelivery.ToString().Replace(',', '.')) + @")"));

                    sql = "update pedido set AliquotaIcms=(select sum(coalesce(AliqIcms, 0)) from produtos_pedido where idPedido=" + pedido.IdPedido + " and (InvisivelPedido = false or InvisivelPedido is null) AND IdProdPedParent IS NULL) / (select Greatest(count(*), 1) from produtos_pedido where idPedido=" + pedido.IdPedido + " and AliqIcms>0 and (InvisivelPedido = false or InvisivelPedido is null) AND IdProdPedParent IS NULL) where idPedido=" + pedido.IdPedido;
                    objPersistence.ExecuteCommand(sessao, sql);

                    sql = "update pedido set ValorIcms=(select sum(coalesce(ValorIcms, 0)) from produtos_pedido where IdPedido=" + pedido.IdPedido + " and (InvisivelPedido = false or InvisivelPedido is null) AND IdProdPedParent IS NULL), Total=(Total + ValorIcms) where idPedido=" + pedido.IdPedido;
                    objPersistence.ExecuteCommand(sessao, sql);
                }
                else
                {
                    sql = "update produtos_pedido pp set AliqIcms=0, ValorIcms=0 where idPedido=" + pedido.IdPedido + " and (InvisivelPedido = false or InvisivelPedido is null) AND IdProdPedParent IS NULL";
                    objPersistence.ExecuteCommand(sessao, sql);

                    sql = "update pedido set AliquotaIcms=0, ValorIcms=0 where idPedido=" + pedido.IdPedido;
                    objPersistence.ExecuteCommand(sessao, sql);
                }

                // Calcula o valor do IPI do pedido
                if (LojaDAO.Instance.ObtemCalculaIpiPedido(sessao, pedido.IdLoja) && ClienteDAO.Instance.IsCobrarIpi(sessao, pedido.IdCli))
                {
                    sql = @"
                        Update produtos_pedido pp
                            inner join pedido ped on (pp.idPedido=ped.idPedido)
                            left join ambiente_pedido ap on (pp.idAmbientePedido=ap.idAmbientePedido) 
                        {0}
                        Where pp.idPedido=" + pedido.IdPedido + " and (pp.InvisivelPedido = false or pp.InvisivelPedido is null) AND pp.IdProdPedParent IS NULL";

                    objPersistence.ExecuteCommand(sessao, string.Format(sql,
                        "SET pp.AliquotaIpi=Round((select aliqIpi from produto where idProd=pp.idProd), 2)"));

                    objPersistence.ExecuteCommand(sessao, string.Format(sql,
                        "SET pp.ValorIpi=(((pp.Total + Coalesce(pp.ValorBenef, 0) - " + descontoRateadoImpostos + @") * " + percFastDelivery.ToString().Replace(',', '.') + @")  * (Coalesce(pp.AliquotaIpi, 0) / 100))"));

                    sql = "update pedido set AliquotaIpi=Round((select sum(coalesce(AliquotaIpi, 0)) from produtos_pedido where idPedido=" + pedido.IdPedido + " and (InvisivelPedido = false or InvisivelPedido is null) AND IdProdPedParent IS NULL) / (select Greatest(count(*), 1) from produtos_pedido where idPedido=" + pedido.IdPedido + " and AliquotaIpi>0 and (InvisivelPedido = false or InvisivelPedido is null) AND IdProdPedParent IS NULL), 2) where idPedido=" + pedido.IdPedido;
                    objPersistence.ExecuteCommand(sessao, sql);

                    sql = "update pedido set ValorIpi=Round((select sum(coalesce(ValorIpi, 0)) from produtos_pedido where IdPedido=" + pedido.IdPedido + " and (InvisivelPedido = false or InvisivelPedido is null) AND IdProdPedParent IS NULL), 2), Total=(Total + ValorIpi) where idPedido=" + pedido.IdPedido;
                    objPersistence.ExecuteCommand(sessao, sql);
                }
                else
                {
                    sql = "update produtos_pedido pp set AliquotaIpi=0, ValorIpi=0 where idPedido=" + pedido.IdPedido + " and (InvisivelPedido = false or InvisivelPedido is null) AND IdProdPedParent IS NULL";
                    objPersistence.ExecuteCommand(sessao, sql);

                    sql = "update pedido set AliquotaIpi=0, ValorIpi=0 where idPedido=" + pedido.IdPedido;
                    objPersistence.ExecuteCommand(sessao, sql);
                }

                // Calcula os impostos dos produtos do pedido
                var impostos = CalculadoraImpostoHelper.ObterCalculadora<Model.Pedido>()
                    .Calcular(sessao, pedido);

                // Salva os dados dos impostos calculados
                impostos.Salvar(sessao);

                // Atualiza o campo ValorComissao
                sql = @"update pedido set valorComissao=total*coalesce(percComissao,0)/100 where idPedido=" + pedido.IdPedido;
                objPersistence.ExecuteCommand(sessao, sql);

                // Atualiza peso e total de m²
                AtualizaTotM(sessao, pedido.IdPedido, false);
                AtualizaPeso(sessao, pedido.IdPedido);

                // Aplica taxa de juros no pedido
                string taxaPrazo = "0";
                objPersistence.ExecuteCommand(sessao, "update pedido set taxaPrazo=" + taxaPrazo + ", Total=Round(Total*(1+(taxaPrazo/100)), 2) where IdPedido=" + pedido.IdPedido);

                //Aplica o frete no pedido
                objPersistence.ExecuteCommand(sessao, "UPDATE pedido SET Total = COALESCE(Total, 0) + ValorEntrega WHERE IdPedido=" + pedido.IdPedido);

                // Se for parceiro, gera parcelas do pedido
                if (pedido.GeradoParceiro)
                {
                    Pedido ped = GetElementByPrimaryKey(sessao, pedido.IdPedido);
                    if (ped.IdCli > 0 && ped.IdSinal == null)
                    {
                        decimal percSinalMinimo = ClienteDAO.Instance.ObtemValorCampo<decimal>(sessao, "percSinalMin", "id_cli=" + ped.IdCli, null);
                        if (percSinalMinimo > 0)
                        {
                            decimal valEntrada = ped.Total * Math.Round(percSinalMinimo / 100, 2);
                            if (valEntrada != ped.ValorEntrada)
                                PedidoDAO.Instance.UpdateParceiro(sessao, ped.IdPedido, ped.CodCliente, valEntrada.ToString().Replace(',', '.'), ped.Obs, ped.ObsLiberacao, ped.IdTransportador);
                        }
                    }
                    GeraParcelaParceiro(sessao, ref ped);
                }

                var rentabilidade = RentabilidadeHelper.ObterCalculadora<Pedido>().Calcular(sessao, pedido);
                if (rentabilidade.Executado)
                    rentabilidade.Salvar(sessao);

                if (criarLogDeAlteracao)
                    LogAlteracaoDAO.Instance.LogPedido(sessao, pedido, GetElementByPrimaryKey(sessao, pedido.IdPedido), LogAlteracaoDAO.SequenciaObjeto.Atual);
            }
            finally
            {
                // Indica que a atualização já acabou
                _atualizando[UserInfo.GetUserInfo.CodUser] = false;
            }
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Atualizar valor do custo do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        public void UpdateCustoPedido(uint idPedido)
        {
            UpdateCustoPedido(null, idPedido);
        }

        /// <summary>
        /// Atualizar valor do custo do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        public void UpdateCustoPedido(GDASession sessao, uint idPedido)
        {
            // Atualiza valor do custo do pedido
            string sql = "update pedido p set " +
                "CustoPedido=(Select Round(Sum(custoProd), 2) From produtos_pedido Where IdPedido=p.IdPedido and (InvisivelPedido = false or InvisivelPedido is null) AND IdProdPedParent IS NULL) " +
                "Where IdPedido=" + idPedido;

            objPersistence.ExecuteCommand(sessao, sql);
        }

        #endregion

        #region Atualiza a observação do pedido

        /// <summary>
        /// Atualiza a observação do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        public void AtualizaObs(uint idPedido, string obs)
        {
            string sql = "update pedido set obs=?obs Where idpedido=" + idPedido;

            objPersistence.ExecuteCommand(sql, new GDAParameter("?obs", obs));
        }

        #endregion

        #region Atualiza a loja do pedido

        /// <summary>
        /// Atualiza a loja do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        public void AtualizaLoja(uint idPedido, uint idLoja)
        {
            string sql = "update pedido set idLoja=?idLoja Where idpedido=" + idPedido;

            objPersistence.ExecuteCommand(sql, new GDAParameter("?idLoja", idLoja));
        }

        #endregion

        #region Recupera pedidos de uma lista

        /// <summary>
        /// Retorna pedidos a partir de uma string com os IDs.
        /// </summary>
        public Pedido[] ObterPedidosPorIdsPedidoParaImpressaoPcp(GDASession sessao, string idsPedido)
        {
            var sql = string.Format(@"SELECT p.*, {0} AS NomeCliente, c.Revenda AS CliRevenda, f.Nome AS NomeFunc, c.IdFunc AS IdFuncCliente, c.Tel_Cont AS RptTelCont, c.Tel_Res AS RptTelCont,
                    c.Tel_Cel AS RptTelCel, c.Tel_Res AS RptTelRes, c.Endereco AS Endereco, c.Numero AS Numero, c.Compl AS Compl, c.Bairro AS Bairro, cid.NomeCidade AS Cidade, cid.NomeUf AS Uf,
                    c.Cep AS Cep, c.Cpf_Cnpj, c.Rg_EscInst, CAST(CONCAT(r.CodInterno, ' - ', r.Descricao) AS CHAR) AS RptRotaCliente, med.Nome as NomeMedidor
                FROM pedido p
                    INNER JOIN cliente c ON (p.IdCli=c.Id_Cli)
                    LEFT JOIN rota_cliente rc ON (c.Id_Cli=rc.IdCliente)
                    LEFT JOIN rota r ON (rc.IdRota=r.IdRota)
                    LEFT JOIN funcionario f ON (p.IdFunc=f.IdFunc)
                    LEFT JOIN funcionario med On (p.IdMedidor=med.IdFunc)                  
                    LEFT JOIN cidade cid ON (c.IdCidade=cid.IdCidade)
                WHERE p.IdPedido IN ({2})", ClienteDAO.Instance.GetNomeCliente("c"), (FinanceiroConfig.PermitirConfirmacaoPedidoPeloFinanceiro || FinanceiroConfig.PermitirFinalizacaoPedidoPeloFinanceiro),
                idsPedido);

            return objPersistence.LoadData(sessao, sql).ToArray();
        }

        /// <summary>
        /// Retorna pedidos a partir de uma string com os IDs.
        /// </summary>
        /// <param name="idsPedido"></param>
        /// <returns></returns>
        public Pedido[] GetByString(GDASession sessao, string idsPedido)
        {
            bool temFiltro;
            string filtroAdicional;

            return objPersistence.LoadData(sessao, Sql(0, 0, idsPedido, null, 0, 0, null, 0, null, 0, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, 0, true, false, 0, 0, 0, 0, 0, null, 0, 0, 0, "", true, out filtroAdicional, out temFiltro).
                Replace("?filtroAdicional?", filtroAdicional)).ToArray();
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna pedidos para comissão a partir de uma string com os IDs.
        /// </summary>
        /// <param name="idsPedido"></param>
        /// <returns></returns>
        public Pedido[] GetByString(string idsPedido, uint idFunc, Pedido.TipoComissao tipoFunc, string dataIni, string dataFim)
        {
            return GetByString(null, idsPedido, idFunc, tipoFunc, dataIni, dataFim);
        }

        /// <summary>
        /// Retorna pedidos para comissão a partir de uma string com os IDs.
        /// </summary>
        /// <param name="idsPedido"></param>
        /// <returns></returns>
        public Pedido[] GetByString(GDASession sessao, string idsPedido, uint idFunc, Pedido.TipoComissao tipoFunc, string dataIni, string dataFim)
        {
            List<Pedido> retorno = objPersistence.LoadData(sessao, SqlComissao(null, idsPedido, 0, tipoFunc, idFunc, dataIni, dataFim, false, false, null, 0),
                GetParamComissao(dataIni, dataFim));

            if (tipoFunc != Pedido.TipoComissao.Todos)
            {
                retorno = retorno.FindAll(new Predicate<Pedido>(delegate (Pedido x)
                {
                    if (tipoFunc != Pedido.TipoComissao.Comissionado)
                        return x.ComissaoFuncionario == tipoFunc;
                    else
                    {
                        x.ComissaoFuncionario = tipoFunc;
                        return true;
                    }

                }));
            }

            GetCamposComissao(sessao, tipoFunc, 0, ref retorno);
            return retorno.ToArray();
        }

        #endregion

        #region Comissão, Acréscimo e Desconto

        #region Comissão

        #region Aplica a comissão no valor dos produtos

        /// <summary>
        /// Aplica um percentual de comissão sobre o valor dos produtos do pedido.
        /// </summary>
        internal bool AplicarComissao(GDASession sessao, Pedido pedido, float percComissao,
            IEnumerable<ProdutosPedido> produtosPedido)
        {
            if (!PedidoConfig.Comissao.ComissaoAlteraValor)
                return false;

            return DescontoAcrescimo.Instance.AplicarComissao(sessao, pedido, percComissao, produtosPedido);
        }

        #endregion

        #region Remove a comissão no valor dos produtos

        /// <summary>
        /// Remove comissão no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        internal bool RemoverComissao(GDASession sessao, Pedido pedido, IEnumerable<ProdutosPedido> produtosPedido)
        {
            return DescontoAcrescimo.Instance.RemoverComissao(sessao, pedido, produtosPedido);
        }

        #endregion

        #region Recupera o valor da comissão

        public decimal GetComissaoPedido(uint idPedido)
        {
            string sql = "select coalesce(sum(coalesce(valorComissao,0)),0) from produtos_pedido where idPedido=" + idPedido;
            return decimal.Parse(objPersistence.ExecuteScalar(sql).ToString());
        }

        #endregion

        #region Métodos de suporte

        /// <summary>
        /// Recupera o percentual de comissão de um pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public float RecuperaPercComissao(uint idPedido)
        {
            return RecuperaPercComissao(null, idPedido);
        }

        /// <summary>
        /// Recupera o percentual de comissão de um pedido.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public float RecuperaPercComissao(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<float>(sessao, "percComissao", "idPedido=" + idPedido);
        }

        private string SqlPedidosIgnoradosComissao(uint idPedido, string motivo, bool selecionar)
        {
            var campos = selecionar ? "p.*, " + ClienteDAO.Instance.GetNomeCliente("c") + " as NomeCliente, f.Nome as NomeFunc, l.NomeFantasia as nomeLoja" : "COUNT(*)";

            var sql = string.Format(@"
                SELECT {0} 
                FROM pedido p
                    INNER JOIN cliente c ON (p.IdCli = c.Id_Cli)
                    LEFT JOIN loja l ON (p.IdLoja = l.IdLoja)
                    LEFT JOIN funcionario f ON (p.IdFunc = f.IdFunc)
                WHERE COALESCE(IgnorarComissao, 0) = 1", campos);

            if (idPedido > 0)
                sql += " AND p.IdPedido = " + idPedido;

            if (!string.IsNullOrWhiteSpace(motivo))
                sql += string.Format(" AND p.MotivoIgnorarComissao like '%{0}%'", motivo);

            return sql;
        }

        /// <summary>
        /// Busca os pedidos que serão ignorados ao gerar comissão
        /// </summary>
        /// <returns></returns>
        public List<Pedido> ObterPedidosIgnorarComissao(uint idPedido, string motivo, string sortExpression, int startRow, int pageSize)
        {
            if (ObterPedidosIgnorarComissaoCountReal(idPedido, motivo) == 0)
                return new List<Pedido>() { new Pedido() };

            var sql = SqlPedidosIgnoradosComissao(idPedido, motivo, true);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize).ToList();
        }

        /// <summary>
        /// Busca os pedidos que serão ignorados ao gerar comissão
        /// </summary>
        /// <returns></returns>
        public int ObterPedidosIgnorarComissaoCountReal(uint idPedido, string motivo)
        {
            var sql = SqlPedidosIgnoradosComissao(idPedido, motivo, false);

            return objPersistence.ExecuteSqlQueryCount(sql);
        }

        /// <summary>
        /// Busca os pedidos que serão ignorados ao gerar comissão
        /// </summary>
        /// <returns></returns>
        public int ObterPedidosIgnorarComissaoCount(uint idPedido, string motivo)
        {
            var count = ObterPedidosIgnorarComissaoCountReal(idPedido, motivo);

            return count == 0 ? 1 : count;
        }

        /// <summary>
        /// Altera se o pedido deve gerar comissão ou não
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="motivo"></param>
        /// <param name="ignorar"></param>
        public void IgnorarComissaoPedido(uint idPedido, string motivo, bool ignorar)
        {
            if (idPedido == 0)
                throw new Exception("Informe o pedido");

            if (ignorar && string.IsNullOrWhiteSpace(motivo))
                throw new Exception("Informe um motivo");

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var pedido = Instance.GetElement(transaction, idPedido);

                    if (pedido == null)
                        throw new Exception("Pedido não encontrado.");

                    pedido.IgnorarComissao = ignorar;
                    pedido.MotivoIgnorarComissao = motivo;

                    Instance.UpdateBase(transaction, pedido);

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw ex;
                }
            }
        }

        #endregion

        #endregion

        #region Acréscimo

        #region Aplica acréscimo no valor dos produtos

        /// <summary>
        /// Aplica acréscimo no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        internal bool AplicarAcrescimo(GDASession sessao, Pedido pedido, int tipoAcrescimo, decimal acrescimo,
            IEnumerable<ProdutosPedido> produtosPedido)
        {
            return DescontoAcrescimo.Instance.AplicarAcrescimo(
                sessao,
                pedido,
                tipoAcrescimo,
                acrescimo,
                produtosPedido
            );
        }

        #endregion

        #region Remove acréscimo no valor dos produtos

        /// <summary>
        /// Remove acréscimo no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        internal bool RemoverAcrescimo(GDASession sessao, Pedido pedido, IEnumerable<ProdutosPedido> produtosPedido)
        {
            return DescontoAcrescimo.Instance.RemoverAcrescimo(sessao, pedido, produtosPedido);
        }

        #endregion

        #region Recupera o valor do acréscimo

        public decimal GetAcrescimoProdutos(uint idPedido)
        {
            string sql = @"select (
                    select coalesce(sum(coalesce(valorAcrescimoProd,0)+coalesce(valorAcrescimoCliente,0)),0)
                    from produtos_pedido where idPedido={0} and coalesce(invisivelpedido, false)=false
                )+(
                    select coalesce(sum(coalesce(valorAcrescimoProd,0)),0)
                    from produto_pedido_benef where idProdPed in (select * from (
                        select idProdPed from produtos_pedido where idPedido={0}
                    ) as temp)
                )";

            return ExecuteScalar<decimal>(String.Format(sql, idPedido));
        }

        public decimal GetAcrescimoPedido(uint idPedido)
        {
            string sql = @"select (
                    select coalesce(sum(coalesce(valorAcrescimo,0)),0)
                    from produtos_pedido where idPedido={0} and coalesce(invisivelpedido, false)=false
                )+(
                    select coalesce(sum(coalesce(valorAcrescimo,0)),0)
                    from produto_pedido_benef where idProdPed in (select * from (
                        select idProdPed from produtos_pedido where idPedido={0}
                    ) as temp)
                )";

            return ExecuteScalar<decimal>(String.Format(sql, idPedido));
        }

        #endregion

        #endregion

        #region Desconto

        #region Aplica desconto no valor dos produtos

        /// <summary>
        /// Aplica desconto no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        internal bool AplicarDesconto(GDASession sessao, Pedido pedido, int tipoDesconto, decimal desconto,
            IEnumerable<ProdutosPedido> produtosPedido)
        {
            return DescontoAcrescimo.Instance.AplicarDesconto(sessao, pedido, tipoDesconto, desconto, produtosPedido);
        }

        #endregion

        #region Remove desconto no valor dos produtos

        /// <summary>
        /// Remove acréscimo no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        internal bool RemoverDesconto(GDASession sessao, Pedido pedido, IEnumerable<ProdutosPedido> produtosPedido)
        {
            return DescontoAcrescimo.Instance.RemoverDesconto(sessao, pedido, produtosPedido);
        }

        #endregion

        #region Recupera o valor do desconto

        /// <summary>
        /// Calcula o desconto por quantidade e o desconto por ambiente contido nos produtos do pedido e nos seus beneficiamentos
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public decimal GetDescontoProdutos(uint idPedido)
        {
            return GetDescontoProdutos(null, idPedido);
        }

        /// <summary>
        /// Calcula o desconto por quantidade e o desconto por ambiente contido nos produtos do pedido e nos seus beneficiamentos
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public decimal GetDescontoProdutos(GDASession sessao, uint idPedido)
        {
            string sql;

            if (PedidoConfig.RatearDescontoProdutos)
            {
                sql = @"select (
                        select coalesce(sum(coalesce(valorDescontoProd,0)+coalesce(valorDescontoQtde,0){1}),0)
                        from produtos_pedido where idPedido={0} and (InvisivelPedido IS NULL OR InvisivelPedido=false)
                    )+(
                        select coalesce(sum(coalesce(valorDescontoProd,0)),0)
                        from produto_pedido_benef where idProdPed in (select * from (
                            select idProdPed from produtos_pedido where idPedido={0} and (InvisivelPedido IS NULL OR InvisivelPedido=false)
                        ) as temp)
                    )";
            }
            else
            {
                sql = @"select (
                        select coalesce(sum(coalesce(pp.total/a.totalProd*a.desconto,0)+coalesce(pp.valorDescontoQtde,0){1}),0)
                        from produtos_pedido pp
                            left join (
                                select a.idAmbientePedido, sum(pp.total+coalesce(pp.valorBenef,0)) as totalProd, 
                                    a.desconto*if(a.tipoDesconto=1, sum(pp.total+coalesce(pp.valorBenef,0))/100, 1) as desconto
                                from produtos_pedido pp
                                    inner join ambiente_pedido a on (pp.idAmbientePedido=a.idAmbientePedido)
                                where pp.idPedido={0} and (InvisivelPedido IS NULL OR InvisivelPedido=false)
                                group by a.idAmbientePedido
                            ) as a on (pp.idAmbientePedido=a.idAmbientePedido)
                        where pp.idPedido={0} and (InvisivelPedido IS NULL OR InvisivelPedido=false)
                    )+(
                        select coalesce(sum(coalesce(ppb.valor/a.totalProd*a.desconto,0)),0)
                        from produto_pedido_benef ppb
                            inner join produtos_pedido pp on (ppb.idProdPed=pp.idProdPed)
                            left join (
                                select a.idAmbientePedido, sum(pp.total+coalesce(pp.valorBenef,0)) as totalProd, 
                                    a.desconto*if(a.tipoDesconto=1, sum(pp.total+coalesce(pp.valorBenef,0))/100, 1) as desconto
                                from produtos_pedido pp
                                    inner join ambiente_pedido a on (pp.idAmbientePedido=a.idAmbientePedido)
                                where pp.idPedido={0} and (InvisivelPedido IS NULL OR InvisivelPedido=false)
                                group by a.idAmbientePedido
                            ) as a on (pp.idAmbientePedido=a.idAmbientePedido)
                        where pp.idPedido={0} and (InvisivelPedido IS NULL OR InvisivelPedido=false)
                    )";
            }

            return ExecuteScalar<decimal>(sessao, String.Format(sql, idPedido, PedidoConfig.ConsiderarDescontoClienteDescontoTotalPedido ? "+coalesce(valorDescontoCliente,0)" : ""));
        }

        public decimal GetDescontoPedido(uint idPedido)
        {
            return GetDescontoPedido(null, idPedido);
        }

        public decimal GetDescontoPedido(GDASession sessao, uint idPedido)
        {
            string sql;

            if (PedidoConfig.RatearDescontoProdutos)
            {
                sql = @"select (
                        select coalesce(sum(coalesce(valorDesconto,0)),0)
                        from produtos_pedido where idPedido={0} and coalesce(invisivelPedido,false)=false
                    )+(
                        select coalesce(sum(coalesce(valorDesconto,0)),0)
                        from produto_pedido_benef where idProdPed in (select * from (
                            select idProdPed from produtos_pedido where idPedido={0} and coalesce(invisivelPedido,false)=false
                        ) as temp)
                    )";
            }
            else
            {
                decimal desconto = 0;
                var descontoPedido = ObterDesconto(sessao, (int)idPedido);
                var tipoDescontoPedido = ObterTipoDesconto(sessao, (int)idPedido);
                var totalPedido = GetTotal(sessao, idPedido);
                var valorIcmsPedido = ObtemValorIcms(sessao, idPedido);
                var valorIpiPedido = ObtemValorIpi(sessao, idPedido);
                var valorEntrega = ObtemValorCampo<decimal>("ValorEntrega", "IdPedido=" + idPedido);

                if (descontoPedido == 100 && tipoDescontoPedido == 1)
                {
                    if (totalPedido > 0)
                        desconto = totalPedido;
                    else
                    {
                        if (!PedidoEspelhoDAO.Instance.ExisteEspelho(sessao, idPedido))
                            desconto = Conversoes.StrParaDecimal(ProdutosPedidoDAO.Instance.GetTotalByPedido(sessao, idPedido).Replace(".", ","));
                        else
                            desconto = ProdutosPedidoDAO.Instance.GetTotalByPedidoFluxo(sessao, idPedido);
                    }
                }
                else
                {
                    if (tipoDescontoPedido == 2)
                        desconto = descontoPedido;
                    else
                    {
                        var taxaFastDelivery = ObtemTaxaFastDelivery(sessao, idPedido);

                        //Remove o IPI, ICMS e valorEntrega
                        var total = totalPedido - (decimal)valorIcmsPedido - valorIpiPedido - valorEntrega;

                        //Remove FastDelivery se houver
                        total = taxaFastDelivery > 0 ? total / (1 + ((decimal)taxaFastDelivery / 100)) : total;

                        //Calcula o desconto
                        desconto = total / (1 - (descontoPedido / 100)) * (descontoPedido / 100);
                    }
                }

                return desconto;
            }

            return ExecuteScalar<decimal>(sessao, String.Format(sql, idPedido));
        }

        #endregion

        #endregion

        #region Finalizar

        internal void FinalizarAplicacaoComissaoAcrescimoDesconto(GDASession sessao, Pedido pedido,
            IEnumerable<ProdutosPedido> produtosPedido, bool atualizar, bool manterFuncDesc = false)
        {
            if (atualizar)
            {
                foreach (var produtoPedido in produtosPedido)
                {
                    ProdutosPedidoDAO.Instance.Update(sessao, produtoPedido, pedido, false, false, false);
                    ProdutosPedidoDAO.Instance.AtualizaBenef(sessao, produtoPedido.IdProdPed, produtoPedido.Beneficiamentos, pedido);
                }
            }

            // A data do desconto não pode ser alterada caso o pedido esteja sendo gerado pelo orçamento.
            if (!manterFuncDesc)
            {
                var dataDesc = DateTime.Now;

                objPersistence.ExecuteCommand(
                    sessao,
                    "update pedido set idFuncDesc=?f, dataDesc=?d where idPedido=" + pedido.IdPedido,
                    new GDAParameter("?f", UserInfo.GetUserInfo.CodUser),
                    new GDAParameter("?d", dataDesc)
                );

                pedido.IdFuncDesc = UserInfo.GetUserInfo.CodUser;
                //pedido.DataDesc = dataDesc;  -- comentado porque não está mapeado
            }
        }

        #endregion

        #endregion

        #region Verifica a situação do pedido com relação à produção

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica a situação do pedido com relação à produção
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsLiberadoEntregaFinanc(uint idPedido)
        {
            return IsLiberadoEntregaFinanc(null, idPedido);
        }

        /// <summary>
        /// Verifica a situação do pedido com relação à produção
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsLiberadoEntregaFinanc(GDASession sessao, uint idPedido)
        {
            string sql = "Select Count(*) From pedido Where liberadoFinanc=true And idPedido=" + idPedido;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Verifica se todas as peças do pedido passado já passaram do tipo de setor passado
        /// </summary>
        public bool PosicaoProducao(GDASession sessao, uint idPedido, SituacaoProdutoProducao situacaoProducao)
        {
            #region Posição produção pedido revenda corte produção

            /* Chamado 47267.
             * A situação de produção do pedido de revenda deve verificar se existem peças pendentes e depois
             * verificar se existem peças entregues, exatamente nessa sequência. Dessa forma,
             * caso uma das peças esteja pendente o pedido fica pendente, senão, caso uma das peças
             * esteja entregue, marca o pedido como entregue. */
            if (GerarPedidoProducaoCorte(sessao, idPedido) && situacaoProducao == SituacaoProdutoProducao.Entregue)
            {
                var sqlPecaPendente = string.Format(@"SELECT COUNT(*) FROM pedido ped
	                    INNER JOIN produtos_pedido_espelho pp ON (ped.IdPedido = pp.IdPedido)
                        LEFT JOIN produto_pedido_producao ppp ON (pp.IdProdPed = ppp.IdProdPed)
                    WHERE ped.IdPedidoRevenda = {0} AND ppp.SituacaoProducao <> {1} AND ppp.Situacao={2};",
                    idPedido, (int)SituacaoProdutoProducao.Entregue, (int)ProdutoPedidoProducao.SituacaoEnum.Producao);

                var retornoPecaPendente = ExecuteScalar<int>(sessao, sqlPecaPendente);

                if (retornoPecaPendente > 0)
                    return false;

                var sqlPecaEntregue = string.Format(@"SELECT COUNT(*) FROM pedido ped
	                    INNER JOIN produtos_pedido_espelho pp ON (ped.IdPedido = pp.IdPedido)
                        LEFT JOIN produto_pedido_producao ppp ON (pp.IdProdPed = ppp.IdProdPed)
                    WHERE ped.IdPedidoRevenda = {0} AND ppp.SituacaoProducao = {1} AND ppp.Situacao={2};",
                    idPedido, (int)SituacaoProdutoProducao.Entregue, (int)ProdutoPedidoProducao.SituacaoEnum.Producao);

                var retornoPecaEntregue = ExecuteScalar<int>(sessao, sqlPecaEntregue);

                if (retornoPecaEntregue > 0)
                    return true;

                return false;
            }

            #endregion

            var sqlBase = @"
                select coalesce(count(ppp.idProdPedProducao){1},0)
                from pedido ped
                    Inner Join produtos_pedido_espelho pp On (ped.idPedido=pp.idPedido)
                    Left Join produto_pedido_producao ppp On (pp.idProdPed=ppp.idProdPed) 
                Where {0}=" + idPedido + @"
                    And ped.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado + @"
                    And ppp.Situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao;

            if (situacaoProducao == SituacaoProdutoProducao.Entregue)
                sqlBase += " AND ppp.IdProdPedProducaoParent IS NULL";

            var sqlProdImpressao = @"
                SELECT COALESCE(COUNT(*))
                FROM produto_impressao
                WHERE idPedidoExpedicao=" + idPedido;

            var complSql = "0";

            var sql = "select (({0})+({1})+({2})+({3}))";
            sql = string.Format(sql,
                string.Format(sqlBase, "ped.idPedido", "{0}"),
                string.Format(complSql, "ped.idPedido"),
                string.Format(sqlBase, "ppp.idPedidoExpedicao", "{0}"),
                sqlProdImpressao);

            /* Chamado 23697. */
            var sqlQuantidadePerda =
                string.Format(
                    @"SELECT * FROM
                        (SELECT COUNT(*) FROM produto_pedido_producao ppp
                        WHERE ppp.DataPerda IS NOT NULL
                            AND ppp.Situacao <> {0}
                            AND (ppp.NumEtiqueta LIKE '{1}-%' {2})) AS temp",
                    (int)ProdutoPedidoProducao.SituacaoEnum.Producao, idPedido,
                    Instance.IsPedidoExpedicaoBox(sessao, idPedido) ?
                        string.Format("OR ppp.IdPedidoExpedicao = {0}", idPedido) : string.Empty);

            var quantidadePerda = ExecuteScalar<int>(sessao, sqlQuantidadePerda);

            int retorno;

            // Garante que há etiquetas na produção
            if ((ExecuteScalar<int>(sessao, string.Format(sql, "")) == 0 || quantidadePerda > 0) &&
                !(ProducaoConfig.TipoControleReposicao == DataSources.TipoReposicaoEnum.Peca && IsMaoDeObra(idPedido)))
            {
                // Se este pedido tiver sido reposto e se não houver peças na produção, pode ser que todas as peças
                // em produção deste pedido sejam perdas, se for o caso, verifica se o pedido que foi reposto está pronto/entregue,
                // se ele estiver pronto, não quer dizer também que o pedido esteja todo pronto, porém se a peça reposta estiver pendente
                // então o pedido estará pendente.
                if (IsPedidoReposto(sessao, idPedido))
                {
                    var sqlRep = "select ({0})";
                    sqlRep = string.Format(sqlRep, string.Format(sqlBase, "ped.idPedidoAnterior", "{0}"));

                    retorno = ExecuteScalar<int>(sessao, string.Format(sqlRep, "=sum(ppp.situacaoProducao>=" + (int)situacaoProducao + ")"));

                    if (retorno == 0)
                        return false;
                }
                else
                    return false;
            }

            // Se retornar 1, quer dizer que todas as peças em produção do pedido passou totalmente do tipo Pronto/Entregue
            retorno = ExecuteScalar<int>(sessao, string.Format(sql, "=sum(ppp.situacaoProducao>=" + (int)situacaoProducao + ")"));
            var prontoEntregue = retorno != 0;

            if (prontoEntregue && IsPedidoReposto(sessao, idPedido))
            {
                sql = "select ({0})";
                sql = string.Format(sql, string.Format(sqlBase, "ped.idPedidoAnterior", "{0}"));

                retorno = ExecuteScalar<int>(sessao, string.Format(sql, "=sum(ppp.situacaoProducao>=" + (int)situacaoProducao + ")"));
                prontoEntregue = retorno != 0;
            }

            // Se estiver entregue mas o pedido for de revenda, é necessário verificar se todas as peças de produção foram expedidas.
            if (prontoEntregue && situacaoProducao == SituacaoProdutoProducao.Entregue &&
                Instance.GetTipoPedido(sessao, idPedido) == Pedido.TipoPedidoEnum.Revenda &&
                ObtemQtdVidrosProducao(sessao, idPedido) != ProdutoPedidoProducaoDAO.Instance.ObtemQtdVidroEstoqueEntreguePorPedido(sessao, idPedido))
                prontoEntregue = false;

            return prontoEntregue;
        }

        /// <summary>
        /// Retorna a descrição da etapa da produção do pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public string GetSituacaoProducaoByPedido(uint idPedido)
        {
            int situacao = Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar("select coalesce(situacaoProducao, 1) from pedido where idPedido=" + idPedido).ToString());
            int tipoEntrega = Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar("select coalesce(tipoEntrega, 1) from pedido where idPedido=" + idPedido).ToString());
            return Pedido.GetDescrSituacaoProducao((int)GetTipoPedido(idPedido), situacao, tipoEntrega, UserInfo.GetUserInfo);
        }

        /// <summary>
        /// Atualiza a situação da produção do pedido.
        /// </summary>
        public void AtualizaSituacaoProducao(GDASession sessao, uint idPedido, SituacaoProdutoProducao? situacaoProducao, DateTime dataLeitura, bool finalizandoInstalacao = false)
        {
            // Variáveis de suporte
            Pedido.SituacaoProducaoEnum situacao = Pedido.SituacaoProducaoEnum.NaoEntregue;
            bool alterado = false;
            bool enviarEmail = false;

            DateTime? dataPronto = ObtemValorCampo<DateTime?>(sessao, "dataPronto", "idPedido=" + idPedido);

            try
            {
                var pedidoAntigo = PedidoDAO.Instance.GetElement(sessao, idPedido);

                // Verifica a situação na produção
                if (PCPConfig.ControlarProducao)
                {
                    if ((situacaoProducao == SituacaoProdutoProducao.Entregue || situacaoProducao.GetValueOrDefault() == 0) && PosicaoProducao(sessao, idPedido, SituacaoProdutoProducao.Entregue))
                    {
                        // Verifica se pedido possui peças que ainda não foram impressas
                        if (!ProdutosPedidoEspelhoDAO.Instance.PossuiPecaASerImpressa(sessao, idPedido))
                        {
                            dataPronto = dataPronto.GetValueOrDefault(dataLeitura);
                            situacao = Pedido.SituacaoProducaoEnum.Entregue;
                            alterado = true;
                        }
                    }
                    else if ((situacaoProducao == SituacaoProdutoProducao.Pronto || situacaoProducao.GetValueOrDefault() == 0) && PosicaoProducao(sessao, idPedido, SituacaoProdutoProducao.Pronto))
                    {
                        // Verifica se pedido possui peças que ainda não foram impressas
                        if (!ProdutosPedidoEspelhoDAO.Instance.PossuiPecaASerImpressa(sessao, idPedido))
                        {
                            dataPronto = dataPronto.GetValueOrDefault(dataLeitura);
                            situacao = Pedido.SituacaoProducaoEnum.Pronto;
                            alterado = true;
                            enviarEmail = true;
                        }
                    }
                    // Chamado 17859: Se tiver lendo em um setor entregue tenha peças prontas, sai do método
                    else if (situacaoProducao == SituacaoProdutoProducao.Entregue && PosicaoProducao(sessao, idPedido, SituacaoProdutoProducao.Pronto))
                        return;
                }

                if (!alterado)
                    dataPronto = null;

                // Verifica a situação da instalação
                if ((!alterado || finalizandoInstalacao) && Geral.ControleInstalacao)
                {
                    if (InstalacaoDAO.Instance.IsFinalizadaByPedido(sessao, idPedido) || finalizandoInstalacao)
                    {
                        situacao = Pedido.SituacaoProducaoEnum.Instalado;
                        alterado = true;
                    }
                }

                // Se for controlar produção, a situação ainda não foi recuperada e possui etiquetas
                if (!alterado && PCPConfig.ControlarProducao)
                {
                    //LogArquivo.InsereLogSitProdPedido("Não Alterado");

                    if (PedidoEspelhoDAO.Instance.ExisteEspelho(sessao, idPedido))
                    {
                        var situacaoEspelho = PedidoEspelhoDAO.Instance.ObtemSituacao(sessao, idPedido);

                        if (situacaoEspelho == PedidoEspelho.SituacaoPedido.Aberto || situacaoEspelho == PedidoEspelho.SituacaoPedido.Cancelado ||
                           situacaoEspelho == PedidoEspelho.SituacaoPedido.Processando)
                            situacao = Pedido.SituacaoProducaoEnum.NaoEntregue;
                        else
                            situacao = Pedido.SituacaoProducaoEnum.Pendente;
                    }
                    else
                        situacao = Pedido.SituacaoProducaoEnum.NaoEntregue;

                    objPersistence.ExecuteCommand(sessao, "update pedido set dataPronto=null where idPedido=" + idPedido);
                }

                // Atualiza a situação da produção
                objPersistence.ExecuteCommand(sessao, "update pedido set dataPronto=?pronto, situacaoProducao=" + (int)situacao + " where idPedido=" + idPedido,
                    new GDAParameter("?pronto", dataPronto));

                /* Chamado 37934. */
                if (IsProducao(sessao, idPedido))
                {
                    if ((situacao == Pedido.SituacaoProducaoEnum.Pronto || situacao == Pedido.SituacaoProducaoEnum.Entregue) &&
                        ObtemSituacao(sessao, idPedido) != Pedido.SituacaoPedido.Confirmado)
                    {
                        AlteraSituacao(sessao, idPedido, Pedido.SituacaoPedido.Confirmado);
                    }
                    else if (ObtemSituacao(sessao, idPedido) != Pedido.SituacaoPedido.ConfirmadoLiberacao
                             && situacao == Pedido.SituacaoProducaoEnum.Pendente)
                    {
                        AlteraSituacao(sessao, idPedido, Pedido.SituacaoPedido.ConfirmadoLiberacao);
                    }
                }

                LogAlteracaoDAO.Instance.LogPedido(sessao, pedidoAntigo, PedidoDAO.Instance.GetElement(sessao, idPedido), LogAlteracaoDAO.SequenciaObjeto.Atual);

                // Atualiza a situação do pedido original (no caso de reposição)
                object retorno = objPersistence.ExecuteScalar(sessao, "select idPedidoAnterior from pedido where idPedido=" + idPedido);
                uint? idPedidoAnterior = retorno != null && retorno.ToString() != "" && retorno != DBNull.Value ? (uint?)Glass.Conversoes.StrParaUint(retorno.ToString()) : null;
                if (idPedidoAnterior != null)
                {
                    AtualizaSituacaoProducao(sessao, idPedidoAnterior.Value, 0, dataLeitura);
                }
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("AtualizaSituacaoProducao - IdPedido: " + idPedido + " Situacao: " + situacao +
                    " SituacaoProducao: " + (situacaoProducao != null ? situacaoProducao.ToString() : "null") + " DataLeitura: " + dataLeitura +
                    " DataPronto: " + (dataPronto.HasValue ? dataPronto.Value.ToLongDateString() : "null") + " Alterado: " + alterado.ToString(), ex);
                throw ex;
            }

            if (enviarEmail)
            {
                try
                {
                    // Envia email/SMS para o cliente indicando que o pedido está pronto, desde que não seja cliente de rota
                    if (!HttpContext.Current.Request.Url.ToString().Contains("localhost:"))
                    {
                        int tipoVenda = ObtemTipoVenda(sessao, idPedido);

                        if (ObtemSituacaoProducao(sessao, idPedido) == (int)Pedido.SituacaoProducaoEnum.Pronto &&
                            tipoVenda != (uint)Pedido.TipoVendaPedido.Reposição &&
                            tipoVenda != (uint)Pedido.TipoVendaPedido.Garantia)
                        {
                            if (PCPConfig.EmailSMS.EnviarEmailPedidoPronto)
                                Email.EnviaEmailPedidoPronto(sessao, idPedido);

                            if (PCPConfig.EmailSMS.EnviarSMSPedidoPronto)
                            {
                                var idClientePedido = ObtemIdCliente(idPedido);

                                if (!(IsPedidoImportado(idPedido) && Geral.NaoEnviarEmailPedidoProntoPedidoImportado.Contains(idClientePedido)))
                                    SMS.EnviaSMSPedidoPronto(sessao, idPedido);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ErroDAO.Instance.InserirFromException("AtualizaSituacaoProducao - EnvioEmail. IdPedido:" + idPedido, ex);
                }

                try
                {
                    if (IsPedidoImportado(sessao, idPedido))
                    {
                        Cliente cliente = ClienteDAO.Instance.GetByPedido(sessao, idPedido);

                        if (!string.IsNullOrEmpty(cliente.UrlSistema))
                        {
                            var urlService = string.Format("{0}{1}", cliente.UrlSistema.ToLower().Substring(0, cliente.UrlSistema.ToLower().LastIndexOf("/webglass")).TrimEnd('/'),
                                "/service/wsexportacaopedido.asmx");

                            Loja loja = LojaDAO.Instance.GetElement(sessao, UserInfo.GetUserInfo.IdLoja);

                            object[] parametros = new object[] { loja.Cnpj, 2, Glass.Conversoes.StrParaUint(ObtemValorCampo<string>(sessao, "codCliente", "idPedido=" + idPedido)) };

                            object retornoWS = WebService.ChamarWebService(urlService, "SyncService", "MarcarPedidoPronto", parametros);

                            //string[] dados = retornoWS as string[];

                            //if (dados[0] == "1")
                            //{
                            //    throw new Exception("Ocorreu um erro e não foi possível avisar ao cliente que o pedido está pronto: " + dados[1] + ".");
                            //}
                        }
                        //else
                        //{
                        //    throw new Exception("Atenção: Para pedidos importados é necessário o preenchimento da URL do sistema do cliente no cadastro do mesmo.");
                        //}
                    }
                }
                catch (Exception ex)
                {
                    ErroDAO.Instance.InserirFromException("AtualizaSituacaoProducao - EnvioEmail. IdPedido:" + idPedido, ex);
                }
            }
        }

        /// <summary>
        /// Atualiza a situação da produção do pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="situacaoProducao">Pode ser null para verificar todos as situações.</param>
        /// <param name="dataLeitura"></param>
        public void AtualizaSituacaoProducao(uint idPedido, SituacaoProdutoProducao? situacaoProducao, DateTime dataLeitura)
        {
            AtualizaSituacaoProducao(null, idPedido, situacaoProducao, dataLeitura);
        }

        #endregion

        #region Verifica se o pedido está confirmado

        /// <summary>
        /// Verifica se o pedido está confirmado ou liberado parcialmente.
        /// </summary>
        public bool IsPedidoLiberado(uint idPedido)
        {
            return IsPedidoLiberado(null, idPedido);
        }

        /// <summary>
        /// Verifica se o pedido está confirmado ou liberado parcialmente.
        /// </summary>
        public bool IsPedidoLiberado(GDASession session, uint idPedido)
        {
            return objPersistence.ExecuteSqlQueryCount(session, "select count(*) from pedido where idPedido=" + idPedido + " and situacao in (" +
                (int)Pedido.SituacaoPedido.Confirmado + ")") > 0;
        }

        /// <summary>
        /// Verifica se o pedido está confirmado ou liberado parcialmente.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsPedidoConfirmado(uint idPedido)
        {
            return IsPedidoConfirmado(null, idPedido);
        }

        /// <summary>
        /// Verifica se o pedido está confirmado ou liberado parcialmente.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsPedidoConfirmado(GDASession session, uint idPedido)
        {
            int situacao = PedidoConfig.LiberarPedido ? (int)Pedido.SituacaoPedido.ConfirmadoLiberacao : (int)Pedido.SituacaoPedido.Confirmado;
            return objPersistence.ExecuteSqlQueryCount(session, "select count(*) from pedido where idPedido=" + idPedido + " and situacao in (" +
                situacao + ", " + (int)Pedido.SituacaoPedido.LiberadoParcialmente + ")") > 0;
        }

        /// <summary>
        /// Verifica se o pedido está confirmado, liberado ou liberado parcialmente.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsPedidoConfirmadoLiberado(uint idPedido)
        {
            return IsPedidoConfirmadoLiberado(idPedido, false);
        }

        /// <summary>
        /// Verifica se o pedido está confirmado, liberado ou liberado parcialmente.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsPedidoConfirmadoLiberado(uint idPedido, bool nf)
        {
            string situacoes = (!nf || !(PedidoConfig.LiberarPedido && FiscalConfig.BloquearEmissaoNFeApenasPedidosLiberados) ?
                (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + ", " : "") +
                (int)Pedido.SituacaoPedido.Confirmado + ", " +
                (int)Pedido.SituacaoPedido.LiberadoParcialmente;

            if (FiscalConfig.PermitirGerarNotaPedidoConferido)
                situacoes += ", " + (int)Pedido.SituacaoPedido.Conferido;

            return objPersistence.ExecuteSqlQueryCount("select count(*) from pedido where idPedido=" + idPedido + " and situacao in (" + situacoes + ")") > 0;
        }

        #endregion

        #region Verifica se o pedido tem sinal a receber

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se o pedido tem sinal a receber.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool TemSinalReceber(uint idPedido)
        {
            return TemSinalReceber(null, idPedido);
        }

        /// <summary>
        /// Verifica se o pedido tem sinal a receber.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool TemSinalReceber(GDASession sessao, uint idPedido)
        {
            var sql = @"Select Count(*) From pedido p Where p.valorEntrada > 0 And p.idSinal Is Null And p.idPagamentoAntecipado is null And Coalesce(p.valorPagamentoAntecipado, 0) < p.total And 
                p.idPedido=" + idPedido + " And p.tipoVenda In (" + (int)Pedido.TipoVendaPedido.APrazo + "," + (int)Pedido.TipoVendaPedido.AVista + ")";

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Verifica se o pedido tem pagamento antecipado a receber/recebido

        /// <summary>
        /// Verifica se o pedido tem sinal a receber.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool TemPagamentoAntecipadoReceber(GDASession sessao, uint idPedido)
        {
            string sql = @"select count(*) from pedido where idCli in (select id_Cli from cliente where 
                pagamentoAntesProducao=true) and tipoVenda in (" + (int)Pedido.TipoVendaPedido.APrazo + "," +
                (int)Pedido.TipoVendaPedido.AVista + ") and idPagamentoAntecipado is null and TipoPedido<>" + (int)Pedido.TipoPedidoEnum.Producao + " and idPedido=" + idPedido;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Verifica se o pedido tem pagamento antecipado recebido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool TemPagamentoAntecipadoRecebido(GDASession sessao, uint idPedido)
        {
            string sql = @"select count(*) from pedido where idCli in (select id_Cli from cliente where 
                pagamentoAntesProducao=true) and tipoVenda in (" + (int)Pedido.TipoVendaPedido.APrazo + "," +
                (int)Pedido.TipoVendaPedido.AVista + ") and idPagamentoAntecipado is not null and idPedido=" + idPedido;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Verifica se o(s) pedido(s) possuem ICMS ST

        /// <summary>
        /// Verifica se o(s) pedido(s) possuem
        /// </summary>
        /// <param name="idsPedido"></param>
        /// <returns></returns>
        public bool PedidosPossuemST(string idsPedido)
        {
            foreach (var idPedido in idsPedido.TrimEnd(',').Split(','))
            {
                var idLojaPedido = ObtemIdLoja(Conversoes.StrParaUint(idPedido));
                if (!LojaDAO.Instance.ObtemCalculaIcmsPedido(idLojaPedido))
                    return false;
            }

            string sql = "Select Count(*) From pedido Where valorIcms>0 and idPedido In (" + idsPedido.TrimEnd(',') + ")";

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Verifica se no pedido possui cálculos de projeto

        /// <summary>
        /// Verifica se no pedido possui cálculos de projeto
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiCalculoProjeto(uint idPedido)
        {
            string sql = "Select Count(*) From ambiente_pedido Where idItemProjeto>0 And idPedido=" + idPedido;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Clona item projeto para o pedido passado

        /// <summary>
        /// Clona item projeto para o pedido passado
        /// </summary>
        public uint ClonaItemProjeto(uint idItemProjeto, uint idPedido)
        {
            return ClonaItemProjeto(null, idItemProjeto, idPedido);
        }

        /// <summary>
        /// Clona item projeto para o pedido passado
        /// </summary>
        public uint ClonaItemProjeto(GDASession session, uint idItemProjeto, uint idPedido)
        {
            var pedido = GetElementByPrimaryKey(session, idPedido);
            uint idItemProjetoPed = 0;

            // Clona item projeto
            ItemProjeto itemProj = ItemProjetoDAO.Instance.GetElement(session, idItemProjeto);
            itemProj.IdOrcamento = null;
            itemProj.IdProjeto = null;
            itemProj.IdPedido = idPedido;
            itemProj.IdPedidoEspelho = null;
            idItemProjetoPed = ItemProjetoDAO.Instance.Insert(session, itemProj);

            // Clona medidas
            MedidaItemProjetoDAO.Instance.DeleteByItemProjeto(session, idItemProjetoPed);
            foreach (MedidaItemProjeto mip in MedidaItemProjetoDAO.Instance.GetListByItemProjeto(session, idItemProjeto))
            {
                mip.IdMedidaItemProjeto = 0;
                mip.IdItemProjeto = idItemProjetoPed;
                MedidaItemProjetoDAO.Instance.Insert(session, mip);
            }

            // Busca materiais
            var lstMateriais = MaterialItemProjetoDAO.Instance.GetByItemProjeto(session, idItemProjeto, false);
            var lstPeca = PecaItemProjetoDAO.Instance.GetByItemProjeto(session, idItemProjeto, itemProj.IdProjetoModelo);

            // Clona peças e materiais
            foreach (PecaItemProjeto pip in lstPeca)
            {
                // Clona as peças
                uint idPecaItemProjOld = pip.IdPecaItemProj;

                pip.Beneficiamentos = pip.Beneficiamentos;
                pip.IdPecaItemProj = 0;
                pip.IdItemProjeto = idItemProjetoPed;
                uint idPecaItemProj = PecaItemProjetoDAO.Instance.Insert(session, pip);

                foreach (FiguraPecaItemProjeto fig in FiguraPecaItemProjetoDAO.Instance.GetForClone(session, idPecaItemProjOld))
                {
                    fig.IdPecaItemProj = idPecaItemProj;
                    FiguraPecaItemProjetoDAO.Instance.Insert(session, fig);
                }

                // Busca o material pela peça e clona ele também
                MaterialItemProjeto mip = lstMateriais.Find(f => f.IdPecaItemProj == idPecaItemProjOld);

                // O material pode ser nulo caso o usuário tenha inserido projeto de medidas exatas e tenha informado quantidade 0
                // ou uma quantidade menor que o padrão da peça
                if (mip != null)
                {
                    uint idMaterialOrig = mip.IdMaterItemProj;

                    mip.Beneficiamentos = mip.Beneficiamentos;
                    mip.IdMaterItemProj = 0;
                    mip.IdItemProjeto = idItemProjetoPed;
                    mip.IdPecaItemProj = idPecaItemProj;
                    uint idMaterial = MaterialItemProjetoDAO.Instance.InsertBase(session, mip, pedido);

                    MaterialItemProjetoDAO.Instance.SetIdMaterItemProjOrig(session, idMaterialOrig, idMaterial);
                }
            }

            // Clona os materiais que não foram clonados acima (os que não possuem referência de peça)
            foreach (MaterialItemProjeto mip in lstMateriais.FindAll(f => f.IdPecaItemProj.GetValueOrDefault() == 0))
            {
                uint idMaterialOrig = mip.IdMaterItemProj;
                mip.Beneficiamentos = mip.Beneficiamentos;
                mip.IdMaterItemProj = 0;
                mip.IdItemProjeto = idItemProjetoPed;

                uint idMaterial = MaterialItemProjetoDAO.Instance.InsertBase(session, mip, pedido);

                // Salva o id do material original no material clonado
                MaterialItemProjetoDAO.Instance.SetIdMaterItemProjOrig(session, idMaterialOrig, idMaterial);
            }

            // Marca que o projeto foi conferido, pois ao gerar pedido de orçamento o projeto já estava conferido.
            ItemProjetoDAO.Instance.CalculoConferido(session, idItemProjetoPed);

            return idItemProjetoPed;
        }

        #endregion

        #region Altera a data de entrega dos pedidos

        /// <summary>
        /// Altera a data de entrega dos pedidos informados, salvando a data original.
        /// </summary>
        public void AlteraDataEntrega(string idsPedidos, DateTime novaDataEntrega)
        {
            AlteraDataEntrega(idsPedidos, novaDataEntrega, false);
        }

        /// <summary>
        /// Altera a data de entrega dos pedidos informados, salvando a data original.
        /// </summary>
        public void AlteraDataEntrega(string idsPedidos, DateTime novaDataEntrega, bool alteraDataFabrica)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    if (novaDataEntrega.Date < DateTime.Now.Date)
                        throw new Exception(string.Format("A data selecionada não pode ser inferior a {0}.", DateTime.Now.ToShortDateString()));

                    var ped = objPersistence.LoadData(transaction, "select * from pedido where idPedido in (" + idsPedidos + ")").ToArray();

                    objPersistence.ExecuteCommand(transaction, "update pedido set dataEntregaOriginal=dataEntrega, dataEntrega=?entrega where idPedido in (" + idsPedidos + ")",
                        new GDAParameter("?entrega", novaDataEntrega));

                    foreach (Pedido p in ped)
                        LogAlteracaoDAO.Instance.LogPedido(transaction, p, GetElementByPrimaryKey(transaction, p.IdPedido), LogAlteracaoDAO.SequenciaObjeto.Atual);

                    if (alteraDataFabrica)
                    {
                        int dias = PCPConfig.Etiqueta.DiasDataFabrica;
                        while (dias > 0)
                        {
                            novaDataEntrega = novaDataEntrega.AddDays(-1);
                            while (!novaDataEntrega.DiaUtil())
                                novaDataEntrega = novaDataEntrega.AddDays(-1);

                            dias--;
                        }

                        PedidoEspelhoDAO.Instance.AlterarDataFabrica(transaction, idsPedidos, novaDataEntrega);
                    }

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        #endregion

        #region Obtém valor de campos do pedido

        public bool ObtemOrdemCargaParcial(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<bool>("OrdemCargaParcial", "idPedido=" + idPedido);
        }

        public bool ObtemDeveTransferir(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<bool>("deveTransferir", "idPedido=" + idPedido);
        }

        public uint? ObtemFormaPagto(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<uint?>(sessao, "idFormaPagto", "idPedido=" + idPedido);
        }

        public uint? ObtemTipoCartao(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<uint?>(sessao, "idTipoCartao", "idPedido=" + idPedido);
        }

        public List<int> ObtemFormaPagto(string idsPedidos)
        {
            return ExecuteMultipleScalar<int>("SELECT idFormaPagto FROM pedido WHERE idPedido IN (" + idsPedidos + ")");
        }

        public uint? ObtemFormaPagto(uint idPedido)
        {
            return ObtemValorCampo<uint?>("IdFormaPagto", "IdPedido=" + idPedido);
        }

        /// <summary>
        /// Verifica se o ICMS ST foi calculado no pedido
        /// </summary>
        public bool CobrouICMSST(uint idPedido)
        {
            return CobrouICMSST(null, idPedido);
        }

        /// <summary>
        /// Verifica se o ICMS ST foi calculado no pedido
        /// </summary>
        public bool CobrouICMSST(GDASession session, uint idPedido)
        {
            return ObtemValorCampo<decimal>(session, "valorIcms", "idPedido=" + idPedido) > 0;
        }

        /// <summary>
        /// Verifica se o IPI foi calculado no pedido
        /// </summary>
        public bool CobrouIPI(uint idPedido)
        {
            return CobrouIPI(null, idPedido);
        }

        /// <summary>
        /// Verifica se o IPI foi calculado no pedido
        /// </summary>
        public bool CobrouIPI(GDASession session, uint idPedido)
        {
            return ObtemValorCampo<decimal>(session, "valorIpi", "idPedido=" + idPedido) > 0;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém a data de entrega do pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public DateTime? ObtemDataEntrega(uint idPedido)
        {
            return ObtemDataEntrega(null, idPedido);
        }

        /// <summary>
        /// Obtém a data de entrega do pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public DateTime? ObtemDataEntrega(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<DateTime?>(sessao, "dataEntrega", "idPedido=" + idPedido);
        }

        /// <summary>
        /// Obtém a data de entrega do pedido.
        /// </summary>
        public DateTime ObtemDataPedido(uint idPedido)
        {
            return ObtemDataPedido(null, idPedido);
        }

        /// <summary>
        /// Obtém a data de entrega do pedido.
        /// </summary>
        public DateTime ObtemDataPedido(GDASession session, uint idPedido)
        {
            return ObtemValorCampo<DateTime>(session, "dataPedido", "idPedido=" + idPedido);
        }

        /// <summary>
        /// Obtém os três primeiros ids relacionados ao sinal
        /// </summary>
        /// <param name="idAcerto"></param>
        /// <returns></returns>
        public string ObtemIdsPeloSinal(uint idSinal)
        {
            // Foi retirada a opção idAcertoParcial para otimizar o comando
            string ids = ExecuteScalar<string>(@"select cast(group_concat(distinct idPedido separator ',') as char) 
                from pedido where idSinal=" + idSinal);

            if (ids == null)
                return "";

            string[] vetIds = ids.Split(',');

            string[] retorno = new string[Math.Min(3, vetIds.Length)];
            Array.Copy(vetIds, retorno, retorno.Length);

            return String.Join(", ", retorno) + (vetIds.Length > 3 ? "..." : "");
        }

        /// <summary>
        /// Obtém os ids dos pedidos no período de confirmação do pedido informado.
        /// </summary>
        /// <param name="dataIniConfPed">Data inicial da confirmação do pedido</param>
        /// <param name="dataFimConfPed">Data final da confirmação do pedido</param>
        /// <returns></returns>
        public string ObtemIdsPelaDataConf(DateTime? dataIniConf, DateTime? dataFimConf)
        {
            var sql = "Select idPedido From pedido Where 1";

            if (dataIniConf != null)
                sql += " And dataConf>=?dataIniConf";

            if (dataFimConf != null)
                sql += " And dataConf<=?dataFimConf";

            var idsPedido = ExecuteMultipleScalar<string>(sql, new GDAParameter("?dataIniConf", dataIniConf), new GDAParameter("?dataFimConf", dataFimConf));

            if (idsPedido == null)
                return "";

            return String.Join(", ", idsPedido);
        }

        /// <summary>
        /// Obtém o id do proejto do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public uint? ObtemIdProjeto(uint idPedido)
        {
            return ObtemValorCampo<uint?>("idProjeto", "idPedido=" + idPedido);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém o idSinal do pedido passado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public uint? ObtemIdSinal(uint idPedido)
        {
            return ObtemIdSinal(null, idPedido);
        }

        /// <summary>
        /// Obtém o idSinal do pedido passado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public uint? ObtemIdSinal(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<uint?>(sessao, "idSinal", "idPedido=" + idPedido);
        }

        /// <summary>
        /// Obtém o idSinal ou pagamento antecipado do pedido passado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public uint? ObtemIdSinalOuPagtoAntecipado(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<uint?>(sessao, "Coalesce(idSinal, IdPagamentoAntecipado) AS IdSinal", "idPedido=" + idPedido);
        }

        /// <summary>
        /// Obtém o idParcela do pedido passado.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns>ID da parcela.</returns>
        public uint? ObtemIdParcela(uint idPedido)
        {
            return ObtemIdParcela(null, idPedido);
        }

        /// <summary>
        /// Obtém o idParcela do pedido passado.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns>ID da parcela.</returns>
        public uint? ObtemIdParcela(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<uint?>(sessao, "idParcela", "idPedido=" + idPedido);
        }

        /// <summary>
        /// Obtém o idPagamentoAntecipado do pedido passado.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns>ID do pagamento antecipado.</returns>
        public uint? ObtemIdPagamentoAntecipado(uint idPedido)
        {
            return ObtemIdPagamentoAntecipado(null, idPedido);
        }

        /// <summary>
        /// Obtém o idPagamentoAntecipado do pedido passado.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idPedido"></param>
        /// <returns>ID do pagamento antecipado.</returns>
        public uint? ObtemIdPagamentoAntecipado(GDASession session, uint idPedido)
        {
            return ObtemValorCampo<uint?>(session, "idPagamentoAntecipado", "idPedido=" + idPedido);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém o cliente do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public uint ObtemIdCliente(uint idPedido)
        {
            return ObtemIdCliente(null, idPedido);
        }

        /// <summary>
        /// Obtém o cliente do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public uint ObtemIdCliente(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<uint>(sessao, "idCli", "idPedido=" + idPedido);
        }

        /// <summary>
        /// Obtem o vendedor do pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public uint ObtemIdFunc(uint idPedido)
        {
            return ObtemIdFunc(null, idPedido);
        }

        /// <summary>
        /// Obtem o vendedor do pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public uint ObtemIdFunc(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<uint>(sessao, "idFunc", "idPedido=" + idPedido);
        }

        public uint? ObtemIdFuncVenda(GDASession session, uint idPedido)
        {
            return ObtemValorCampo<uint?>(session, "idFuncVenda", "idPedido=" + idPedido);
        }

        /// <summary>
        /// Obtem o vendedor que cadastrou o pedido.
        /// </summary>
        public uint ObtemIdFuncCad(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<uint>(sessao, "UsuCad", "idPedido=" + idPedido);
        }

        /// <summary>
        /// Obtem o funcionário que colocou o desconto no pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public uint? ObtemIdFuncDesc(uint idPedido)
        {
            return ObtemIdFuncDesc(null, idPedido);
        }

        /// <summary>
        /// Obtem o funcionário que colocou o desconto no pedido.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public uint? ObtemIdFuncDesc(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<uint?>(sessao, "idFuncDesc", "idPedido=" + idPedido);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém a loja do pedido
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public uint ObtemIdLoja(uint idPedido)
        {
            return ObtemIdLoja(null, idPedido);
        }

        /// <summary>
        /// Obtém a loja do pedido
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public uint ObtemIdLoja(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<uint>(sessao, "idLoja", "idPedido=" + idPedido);
        }

        /// <summary>
        /// Obtém as lojas dos pedidos
        /// </summary>
        public string ObtemIdsLojas(string idsPedidos)
        {
            if (string.IsNullOrWhiteSpace(idsPedidos))
                return string.Empty;

            var sql = string.Format("Select Distinct IdLoja from Pedido Where idPedido in ({0})", idsPedidos);

            var resultado = string.Empty;

            foreach (var record in this.CurrentPersistenceObject.LoadResult(sql, null))
            {
                resultado += record["IdLoja"].ToString() + ",";
            }

            return resultado.TrimEnd(',');
        }

        /// <summary>
        /// Obtém o cliente do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public string ObtemPedCli(uint idPedido)
        {
            return ObtemValorCampo<string>("codCliente", "idPedido=" + idPedido);
        }

        /// <summary>
        /// Obtém o comissionado do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public uint ObtemIdComissionado(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<uint>(sessao, "idComissionado", "idPedido=" + idPedido);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém a situação do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public Pedido.SituacaoPedido ObtemSituacao(uint idPedido)
        {
            return ObtemSituacao(null, idPedido);
        }

        /// <summary>
        /// Obtém a situação do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public Pedido.SituacaoPedido ObtemSituacao(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<Pedido.SituacaoPedido>(sessao, "situacao", "idPedido=" + idPedido);
        }

        /// <summary>
        /// Retorna o celular do cliente do pedido importado
        /// </summary>
        public string ObtemCelCliExterno(uint idPedido)
        {
            return ObtemCelCliExterno(null, idPedido);
        }

        /// <summary>
        /// Retorna o celular do cliente do pedido importado
        /// </summary>
        public string ObtemCelCliExterno(GDASession session, uint idPedido)
        {
            return ObtemValorCampo<string>(session, "celCliExterno", "idPedido=" + idPedido);
        }

        /// <summary>
        /// Obtém a situação da produção do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int ObtemSituacaoProducao(GDASession sessao, uint idPedido)
        {
            string sql = "Select situacaoProducao From pedido Where idPedido=" + idPedido;

            object obj = objPersistence.ExecuteScalar(sessao, sql);

            return obj == null || obj.ToString().Trim() == String.Empty ? 0 : Glass.Conversoes.StrParaInt(obj.ToString());
        }

        /// <summary>
        /// Obtém a situação da produção do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int ObtemSituacaoProducao(uint idPedido)
        {
            return ObtemSituacaoProducao(null, idPedido);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna o tipo de venda de um pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int ObtemTipoVenda(uint idPedido)
        {
            return ObtemTipoVenda(null, idPedido);
        }

        /// <summary>
        /// Retorna o tipo de venda de um pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int ObtemTipoVenda(GDASession sessao, uint idPedido)
        {
            string sql = "Select tipoVenda From pedido Where idPedido=" + idPedido;

            object obj = objPersistence.ExecuteScalar(sessao, sql);

            return obj == null || obj.ToString().Trim() == String.Empty ? 0 : Glass.Conversoes.StrParaInt(obj.ToString());
        }

        /// <summary>
        /// Retorna o tipo de venda de um pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public Pedido.TipoPedidoEnum ObterTipoPedido(GDASession sessao, uint idPedido)
        {
            string sql = "SELECT TipoPedido FROM pedido WHERE idPedido=" + idPedido;

            return ExecuteScalar<Pedido.TipoPedidoEnum>(sessao, sql);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Obtém o tipo de entrega do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int ObtemTipoEntrega(uint idPedido)
        {
            return ObtemTipoEntrega(null, idPedido);
        }

        /// <summary>
        /// Obtém o tipo de entrega do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int ObtemTipoEntrega(GDASession sessao, uint idPedido)
        {
            string sql = "Select tipoEntrega From pedido Where idPedido=" + idPedido;

            object obj = objPersistence.ExecuteScalar(sessao, sql);

            return obj == null || obj.ToString().Trim() == String.Empty ? 0 : Glass.Conversoes.StrParaInt(obj.ToString());
        }

        /// <summary>
        /// Obtém o funcionário responsável pelo pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public string ObtemNomeFuncResp(GDASession session, uint idPedido)
        {
            return ExecuteScalar<string>(session, @"
                Select f.nome 
                From pedido p 
                    Inner Join funcionario f On (p.idFunc=f.idFunc) 
                Where idpedido=" + idPedido);
        }

        /// <summary>
        /// Obtém o funcionário que comprou o pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public string ObtemNomeFuncVenda(uint idPedido)
        {
            string sql = "Select nome From funcionario Where idFunc in " +
                "(Select idFuncVenda From pedido Where idpedido=" + idPedido + ")";

            object obj = objPersistence.ExecuteScalar(sql);

            return obj == null ? String.Empty : obj.ToString();
        }

        /// <summary>
        /// Obtém o valor do ICMS do pedido
        /// </summary>
        public float ObtemValorIcms(GDASession session, uint idPedido)
        {
            string sql = "Select valorIcms From pedido Where idPedido=" + idPedido;

            object obj = objPersistence.ExecuteScalar(session, sql);

            return obj == null || obj.ToString().Trim() == String.Empty ? 0 : Glass.Conversoes.StrParaFloat(obj.ToString());
        }

        /// <summary>
        /// Obtém o valor do ICMS do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns>Retorna o valor do IPI do pedido.</returns>
        public decimal ObtemValorIpi(GDASession session, uint idPedido)
        {
            string sql = "Select valorIpi From pedido Where idPedido=" + idPedido;

            object obj = objPersistence.ExecuteScalar(session, sql);

            return obj == null || obj.ToString().Trim() == String.Empty ? 0 : Glass.Conversoes.StrParaDecimal(obj.ToString());
        }

        public DateTime GetDataPedido(uint idPedido)
        {
            return GetDataPedido(null, idPedido);
        }

        public DateTime GetDataPedido(GDASession session, uint idPedido)
        {
            string sql = "Select dataPedido From pedido Where idPedido=" + idPedido;

            object obj = objPersistence.ExecuteScalar(session, sql);

            return obj == null || obj.ToString().Trim() == String.Empty ? DateTime.Now : DateTime.Parse(obj.ToString());
        }

        public string ObtemObs(GDASession sessao, uint idPedido)
        {
            string sql = "Select obs From pedido Where idPedido=" + idPedido;

            object obj = objPersistence.ExecuteScalar(sessao, sql);

            return obj == null ? String.Empty : obj.ToString();
        }

        public string ObtemObsLiberacao(uint idPedido)
        {
            string sql = "Select ObsLiberacao From pedido Where idPedido=" + idPedido;

            object obj = objPersistence.ExecuteScalar(sql);

            return obj == null ? String.Empty : obj.ToString();
        }

        public void SalvarObsLiberacao(uint idPedido, string obLiberacao)
        {
            objPersistence.ExecuteCommand("update pedido set ObsLiberacao=?obsLiberacao where idPedido=" + idPedido,
                    new GDAParameter("?obsLiberacao", obLiberacao));
        }

        public float ObtemTaxaFastDelivery(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<float>(sessao, "taxaFastDelivery", "idPedido=" + idPedido);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public decimal ObtemValorEntrada(uint idPedido)
        {
            return ObtemValorEntrada(null, idPedido);
        }

        public decimal ObtemValorEntrada(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<decimal>(sessao, "valorEntrada", "idPedido=" + idPedido);
        }

        public decimal ObtemValorPagtoAntecipado(uint idPedido)
        {
            return ObtemValorPagtoAntecipado(null, idPedido);
        }

        public decimal ObtemValorPagtoAntecipado(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<decimal>(sessao, "valorPagamentoAntecipado", "idPedido=" + idPedido);
        }

        public decimal ObtemDescontoCalculado(GDASession sessao, uint idPedido)
        {
            var sql = @"
                SELECT IF(tipoDesconto = 1, desconto, Coalesce(round(desconto / (total +
                                                                   (SELECT sum(coalesce(valorDescontoQtde, 0))
                                                                    FROM produtos_pedido
                                                                    WHERE idPedido = p.idPedido) + desconto), 2), 0) * 100)
                FROM pedido p
                WHERE idPedido = " + idPedido;

            return ExecuteScalar<decimal>(sessao, sql);
        }

        #region Comissão/Desconto/Acréscimo

        /// <summary>
        /// Obtém o percentual de comissao do pedido
        /// </summary>
        public float ObterPercentualComissao(GDASession session, int idPedido)
        {
            var sql = string.Format("Select percComissao From pedido Where idPedido={0}", idPedido);

            object obj = objPersistence.ExecuteScalar(session, sql);

            return obj == null || obj.ToString().Trim() == String.Empty ? 0 : Glass.Conversoes.StrParaFloat(obj.ToString());
        }

        public int ObterTipoAcrescimo(GDASession session, int idPedido)
        {
            return ObtemValorCampo<int>(session, "TipoAcrescimo", string.Format("IdPedido={0}", idPedido));
        }

        public decimal ObterAcrescimo(GDASession session, int idPedido)
        {
            return ObtemValorCampo<decimal>(session, "Acrescimo", string.Format("IdPedido={0}", idPedido));
        }

        public int ObterTipoDesconto(GDASession session, int idPedido)
        {
            return ObtemValorCampo<int>(session, "TipoDesconto", string.Format("IdPedido={0}", idPedido));
        }

        public decimal ObterDesconto(GDASession session, int idPedido)
        {
            return ObtemValorCampo<decimal>(session, "Desconto", string.Format("IdPedido={0}", idPedido));
        }

        #endregion

        public string ObtemEnderecoObra(uint idPedido)
        {
            return ObtemValorCampo<string>("enderecoObra", "idPedido=" + idPedido);
        }

        public string ObtemBairroObra(uint idPedido)
        {
            return ObtemValorCampo<string>("bairroObra", "idPedido=" + idPedido);
        }

        public string ObtemCidadeObra(uint idPedido)
        {
            return ObtemValorCampo<string>("cidadeObra", "idPedido=" + idPedido);
        }

        public void ObtemDadosComissaoDescontoAcrescimo(uint idPedido, out int tipoDesconto,
            out decimal desconto, out int tipoAcrescimo, out decimal acrescimo, out float percComissao,
            out uint? idComissionado)
        {
            ObtemDadosComissaoDescontoAcrescimo(null, idPedido, out tipoDesconto, out desconto, out tipoAcrescimo, out acrescimo,
                out percComissao, out idComissionado);
        }

        public void ObtemDadosComissaoDescontoAcrescimo(GDASession sessao, uint idPedido, out int tipoDesconto,
            out decimal desconto, out int tipoAcrescimo, out decimal acrescimo, out float percComissao,
            out uint? idComissionado)
        {
            string where = "idPedido=" + idPedido;
            tipoDesconto = ObtemValorCampo<int>(sessao, "tipoDesconto", where);
            desconto = ObtemValorCampo<decimal>(sessao, "desconto", where);
            tipoAcrescimo = ObtemValorCampo<int>(sessao, "tipoAcrescimo", where);
            acrescimo = ObtemValorCampo<decimal>(sessao, "acrescimo", where);
            percComissao = RecuperaPercComissao(sessao, idPedido);
            idComissionado = ObtemValorCampo<uint?>(sessao, "idComissionado", where);
        }

        public bool IsPedidoImportado(uint idPedido)
        {
            return IsPedidoImportado(null, idPedido);
        }

        public bool IsPedidoImportado(GDASession session, uint idPedido)
        {
            return ObtemValorCampo<bool>(session, "importado", "idPedido=" + idPedido);
        }

        public string ObtemCancelados(string idsPedido)
        {
            return ObtemCancelados(null, idsPedido);
        }

        public string ObtemCancelados(GDASession session, string idsPedido)
        {
            return ObtemValorCampo<string>(session, "cast(group_concat(idPedido) as char)", "idPedido in (" + idsPedido + ") And situacao=" +
                (int)Pedido.SituacaoPedido.Cancelado);
        }

        internal decimal GetTotalSemDesconto(uint idPedido, decimal total)
        {
            return GetTotalSemDesconto(null, idPedido, total);
        }

        internal decimal GetTotalSemDesconto(GDASession sessao, uint idPedido, decimal total)
        {
            return total + GetDescontoPedido(sessao, idPedido) + GetDescontoProdutos(sessao, idPedido);
        }

        internal decimal GetTotalSemAcrescimo(uint idPedido, decimal total)
        {
            return total - GetAcrescimoPedido(idPedido) - GetAcrescimoProdutos(idPedido);
        }

        internal decimal GetTotalSemComissao(uint idPedido, decimal total)
        {
            return total - GetComissaoPedido(idPedido);
        }

        internal decimal GetTotalParaLiberacao(uint idPedido)
        {
            return GetTotalParaLiberacao(null, idPedido);
        }

        internal decimal GetTotalParaLiberacao(GDASession sessao, uint idPedido)
        {
            decimal total = PedidoEspelhoDAO.Instance.ObtemValorCampo<decimal>(sessao, "total", "idPedido=" + idPedido);
            total = PCPConfig.UsarConferenciaFluxo && total > 0 ? total : GetTotal(sessao, idPedido);
            float taxaPrazo = ObtemValorCampo<float>(sessao, "taxaPrazo", "idPedido=" + idPedido);

            return GetTotalSemDesconto(sessao, idPedido, total);
        }

        internal decimal GetTotalBruto(uint idPedido, bool espelho)
        {
            decimal total = !espelho ? GetTotal(idPedido) : PedidoEspelhoDAO.Instance.ObtemValorCampo<decimal>("total", "idPedido=" + idPedido);
            float taxaPrazo = ObtemValorCampo<float>("taxaPrazo", "idPedido=" + idPedido);

            decimal acrescimo = total - GetTotalSemAcrescimo(idPedido, total);
            decimal desconto = GetTotalSemDesconto(idPedido, total) - total;
            decimal comissao = total - GetTotalSemComissao(idPedido, total);
            return total - acrescimo + desconto - comissao;
        }

        public int ObtemQtdPedidosFinanceiro()
        {
            return ObtemValorCampo<int>("COUNT(*)", "Situacao in (" + (int)Pedido.SituacaoPedido.AguardandoConfirmacaoFinanceiro
                + "," + (int)Pedido.SituacaoPedido.AguardandoFinalizacaoFinanceiro + ")");
        }

        public uint ObtemIdClienteExterno(GDASession session, uint idPedido)
        {
            return ObtemValorCampo<uint>(session, "IdClienteExterno", "idPedido=" + idPedido);
        }

        public string ObtemClienteExterno(GDASession session, uint idPedido)
        {
            return ObtemValorCampo<string>(session, "ClienteExterno", "idPedido=" + idPedido);
        }

        public uint ObtemIdPedidoExterno(GDASession session, uint idPedido)
        {
            return ObtemValorCampo<uint>(session, "IdPedidoExterno", "idPedido=" + idPedido);
        }

        /// <summary>
        /// Recupera a referência do pedido de revenda que gerou o pedido passado.
        /// </summary>
        public int? ObterIdPedidoRevenda(GDASession session, int idPedido)
        {
            return ObtemValorCampo<int>(session, "IdPedidoRevenda", string.Format("IdPedido={0}", idPedido));
        }

        /// <summary>
        /// Recupera a referência do pedido de produção associado ao pedido de revenda informado por parâmetro.
        /// </summary>
        public List<int> ObterIdsPedidoProducaoPeloIdPedidoRevenda(GDASession session, int idPedido)
        {
            return ExecuteMultipleScalar<int>(session, string.Format("SELECT IdPedido FROM pedido WHERE IdPedidoRevenda={0}", idPedido));
        }

        /// <summary>
        /// Retorna o ID do último pedido inserido no sistema.
        /// </summary>
        public int? ObterUltimoIdPedidoInserido()
        {
            return ObtemValorCampo<int?>("MAX(IdPedido)", "1");
        }

        public float ObtemPercComissaoAdmin(uint idPedido)
        {
            return ObtemValorCampo<float>("PercentualComissao", "IdPedido=" + idPedido);
        }

        public int ObtemQuantidadePecas(GDASession session, uint idPedido)
        {

            var invisivel = PCPConfig.UsarConferenciaFluxo ? "Fluxo" : "Pedido";

            var sql = $@"SELECT CAST(SUM(COALESCE(Qtde, 0)) AS SIGNED INTEGER) FROM produtos_pedido pp 
                    LEFT JOIN produto p ON (pp.IdProd=p.IdProd)
                WHERE IdPedido=?id AND (Invisivel{invisivel} IS NULL OR Invisivel{invisivel}=0) AND p.IdGrupoProd={(int)NomeGrupoProd.Vidro}";

            return ExecuteScalar<int>(session, sql, new GDAParameter("?id", idPedido));

        }

        /// <summary>
        /// Retorna a descrição das situações passadas
        /// </summary>
        public string GetSituacaoProdPedido(string situacao)
        {
            return GetSituacaoProdPedido(situacao, UserInfo.GetUserInfo);
        }

        /// <summary>
        /// Retorna a descrição das situações passadas
        /// </summary>
        public string GetSituacaoProdPedido(string situacao, LoginUsuario login)
        {
            string descr = String.Empty;

            foreach (string sit in situacao.Split(','))
                descr += Pedido.GetDescrSituacaoProducao(0, sit.StrParaInt(), 0, login) + "/";

            return descr.TrimEnd('/');
        }

        /// <summary>
        /// Retorna a descrição das situações passadas
        /// </summary>
        /// <param name="situacao"></param>
        /// <returns></returns>
        public string GetSituacaoPedido(string situacao)
        {
            string descr = String.Empty;

            foreach (string sit in situacao.Split(','))
                descr += GetSituacaoPedido(Glass.Conversoes.StrParaInt(sit)) + "/";

            return descr.TrimEnd('/');
        }

        /// <summary>
        /// Retorna a descrição da situação do pedido
        /// </summary>
        /// <param name="situacao"></param>
        /// <returns></returns>
        public string GetSituacaoPedido(int situacao)
        {
            switch (situacao)
            {
                case (int)Glass.Data.Model.Pedido.SituacaoPedido.Ativo:
                    return "Ativo";
                case (int)Glass.Data.Model.Pedido.SituacaoPedido.AtivoConferencia:
                    return "Ativo/Em Conferência";
                case (int)Glass.Data.Model.Pedido.SituacaoPedido.EmConferencia:
                    return "Em Conferência";
                case (int)Glass.Data.Model.Pedido.SituacaoPedido.Conferido:
                    return !PedidoConfig.LiberarPedido ? "Conferido" : "Conferido COM";
                case (int)Glass.Data.Model.Pedido.SituacaoPedido.Confirmado:
                    return !PedidoConfig.LiberarPedido ? "Confirmado" : "Liberado";
                case (int)Glass.Data.Model.Pedido.SituacaoPedido.ConfirmadoLiberacao:
                    return "Confirmado PCP";
                case (int)Glass.Data.Model.Pedido.SituacaoPedido.Cancelado:
                    return "Cancelado";
                case (int)Glass.Data.Model.Pedido.SituacaoPedido.LiberadoParcialmente:
                    return "Liberado Parcialmente";
                case (int)Glass.Data.Model.Pedido.SituacaoPedido.AguardandoFinalizacaoFinanceiro:
                    return "Aguardando Finalização Financeiro";
                case (int)Glass.Data.Model.Pedido.SituacaoPedido.AguardandoConfirmacaoFinanceiro:
                    return "Aguardando Confirmação Financeiro";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Retorna a descrição da situação de produção do pedido.
        /// </summary>
        /// <param name="situacaoProducao"></param>
        /// <returns></returns>
        public string GetSituacaoProducaoPedido(int situacaoProducao)
        {
            switch (situacaoProducao)
            {
                case (int)Glass.Data.Model.Pedido.SituacaoProducaoEnum.Entregue:
                    return "Entregue";
                case (int)Glass.Data.Model.Pedido.SituacaoProducaoEnum.Instalado:
                    return "Instalado";
                case (int)Glass.Data.Model.Pedido.SituacaoProducaoEnum.NaoEntregue:
                    return "Não Entregue";
                case (int)Glass.Data.Model.Pedido.SituacaoProducaoEnum.Pendente:
                    return "Pendente";
                case (int)Glass.Data.Model.Pedido.SituacaoProducaoEnum.Pronto:
                    return "Pronto";
                default:
                    return "Etiqueta não impressa";
            }
        }

        /// <summary>
        /// Verifica se o pedido possui alguma impressão de box.
        /// </summary>
        public bool PossuiImpressaoBox(GDASession session, uint idPedido)
        {
            return objPersistence.ExecuteSqlQueryCount(session,
                string.Format(
                    @"SELECT COUNT(*) FROM produtos_pedido pp
                    WHERE pp.QtdeBoxImpresso IS NOT NULL AND pp.QtdeBoxImpresso > 0
                        AND pp.IdPedido = {0};", idPedido)) > 0;
        }

        public bool VerificarPedidoPossuiIcmsEDesconto(string idsPedidos)
        {
            var sql = string.Format("select COUNT(*) from pedido where ValorIcms>0 AND Desconto>0 AND IdPedido IN({0})", idsPedidos);

            return ExecuteScalar<bool>(sql);
        }

        /// <summary>
        /// Busca os ids dos pedido que possuem pedido de produção vinculado e que o pedido de produção ainda não tenha sido confirmado PCP ou liberaodo.
        /// </summary>
        public string ObterIdsPedidoRevendaSemProducaoConfirmadaLiberada(GDASession session, string idsPedido)
        {
            var sql = @"SELECT group_concat(IdPedido SEPARATOR ', ')
                        FROM 
                        (
	                        SELECT p.IdPedido, SUM(IF(pProd.Situacao IN (" + (int)Pedido.SituacaoPedido.Confirmado +
                            ", " + (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + ", " + (int)Pedido.SituacaoPedido.LiberadoParcialmente + @"), 1, 0)) as qtdePedProducao
	                        FROM pedido p
		                        LEFT JOIN pedido pProd ON (p.IdPedido = pProd.IdPedidoRevenda)
	                        WHERE p.IdPedido IN (" + idsPedido + @")
                                AND p.GerarPedidoProducaoCorte = 1
	                        GROUP BY p.IdPedido
	                        HAVING qtdePedProducao = 0
                        ) as tmp";

            var ids = ExecuteScalar<string>(session, sql);
            return ids;
        }

        #endregion
    }
}
