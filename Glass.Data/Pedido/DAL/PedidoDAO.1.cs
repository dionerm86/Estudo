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
        #region Gráfico de vendas

        /// <summary>
        /// Método que retorna o SQL utilizado para a busca dos dados do gráfico de vendas.
        /// </summary>
        internal string SqlGraficoVendas(bool administrador, string dataFimSituacao, string dataInicioSituacao, bool emitirGarantiaReposicao, bool emitirPedidoFuncionario, out string filtroAdicional,
            int? idCliente, int? idFunc, int? idLoja, int? idVendAssoc, int? idRota, bool loginCliente, string nomeCliente, out bool temFiltro, string tipoPedido)
        {
            temFiltro = false;
            filtroAdicional = string.Empty;
            var criterio = string.Empty;
            var formatoCriterio = "{0} {1}    ";
            var calcularLiberados = PedidoConfig.LiberarPedido;
            var situacoes = string.Format("{0},{1}", (int)Pedido.SituacaoPedido.Confirmado, (int)Pedido.SituacaoPedido.LiberadoParcialmente);

            /* Chamado 49970. */
            var camposFluxo = string.Format(@"CAST(COALESCE({0}.Total, {1}.Total) AS DECIMAL (12,2)) AS TotalReal,
                CAST(COALESCE({0}.Total, {1}.Total) AS DECIMAL (12,2)) AS Total,
                CAST(COALESCE({0}.Desconto, {1}.Desconto) AS DECIMAL (12,2)) AS Desconto,
                CAST(COALESCE({0}.TipoDesconto, {1}.TipoDesconto) AS DECIMAL (12,2)) AS TipoDesconto,
                CAST(COALESCE({0}.Acrescimo, {1}.Acrescimo) AS DECIMAL (12,2)) AS Acrescimo,
                CAST(COALESCE({0}.TipoAcrescimo, {1}.TipoAcrescimo) AS DECIMAL (12,2)) AS TipoAcrescimo,
                CAST(COALESCE({0}.Peso, {1}.Peso) AS DECIMAL (12,2)) AS Peso,
                CAST(COALESCE({0}.TotM, {1}.TotM) AS DECIMAL (12,2)) AS TotM,
                CAST(COALESCE({0}.AliquotaIpi, {1}.AliquotaIpi) AS DECIMAL (12,2)) AS AliquotaIpi,
                CAST(COALESCE({0}.ValorIpi, {1}.ValorIpi) AS DECIMAL (12,2)) AS ValorIpi,
                CAST(COALESCE({0}.AliquotaIcms, {1}.AliquotaIcms) AS DECIMAL (12,2)) AS AliquotaIcms,
                CAST(COALESCE({0}.ValorIcms, {1}.ValorIcms) AS DECIMAL (12,2)) AS ValorIcms", PCPConfig.UsarConferenciaFluxo ? "pe" : "p", PCPConfig.UsarConferenciaFluxo ? "p" : "pe");

            var campos = string.Format(@"{0}, p.IdLoja, p.IdFunc, p.IdCli, f.Nome AS NomeFunc, c.IdFunc AS IdFuncCliente, fc.Nome AS NomeFuncCliente, l.NomeFantasia AS NomeLoja, p.TipoPedido,
                p.Situacao, p.DataConf, com.IdComissionado, com.Nome AS NomeComissionado, lp.DataLiberacao, {1} AS NomeCliente, p.Importado, rc.IdRota, r.Descricao as DescricaoRota, '$$$' AS Criterio",
                camposFluxo, ClienteDAO.Instance.GetNomeCliente("c"));

            var usarDadosVendidos = !loginCliente;

            var whereDadosVendidos = string.Format(" AND ped.Situacao IN ({0}) AND ped.TipoVenda IN ({1},{2},{3})", situacoes, (int)Pedido.TipoVendaPedido.AVista, (int)Pedido.TipoVendaPedido.APrazo,
                    (int)Pedido.TipoVendaPedido.Obra);

            var campoDadosVendidos = !usarDadosVendidos ? string.Empty : ", TRIM(dv.DadosVidrosVendidos) AS DadosVidrosVendidos";

            var dadosVendidos = usarDadosVendidos ?
                string.Format(@"LEFT JOIN (
                    SELECT IdCli, IdPedido, CONCAT(CAST(GROUP_CONCAT(CONCAT('\n* ', CodInterno, ' - ', Descricao, ': Qtde ', Qtde, '  Tot. m² ', 
                        TotM2)) AS CHAR), RPAD('', 100, ' ')) AS DadosVidrosVendidos
                    FROM (
                        SELECT ped.IdCli, ped.IdPedido, pp.IdProd, p.CodInterno, p.Descricao, 
                            REPLACE(CAST(SUM(pp.TotM) AS CHAR), '.', ',') AS TotM2, CAST(SUM(pp.Qtde) AS SIGNED) AS Qtde
                        FROM produtos_pedido pp
                            INNER JOIN pedido ped ON (pp.IdPedido=ped.IdPedido)
                            INNER JOIN cliente cli ON (ped.IdCli=cli.Id_Cli)
                            INNER JOIN produto p ON (pp.IdProd=p.IdProd)
                            LEFT JOIN rota_cliente rtc ON (cli.Id_Cli=rtc.IdCliente)
                            LEFT JOIN rota rt ON (rtc.IdRota=rt.IdRota)
                            LEFT JOIN produtos_liberar_pedido plp1 ON (pp.IdProdPed=plp1.IdProdPed)
                            LEFT JOIN liberarpedido lp1 ON (plp1.IdLiberarPedido=lp1.IdLiberarPedido)
                        WHERE p.IdGrupoProd=1 AND {0}
                            {1}
                        GROUP BY ped.IdCli, pp.IdProd
                    ) AS Temp
                    GROUP BY IdCli
                ) AS dv ON (dv.IdCli=p.IdCli)", string.Format("(Invisivel{0} IS NULL OR Invisivel{0}=0)", PCPConfig.UsarConferenciaFluxo ? "Fluxo" : "Pedido"), "{0}") : string.Empty;

            var sql = string.Format(@"
                SELECT {0}
                FROM pedido p 
                    INNER JOIN cliente c ON (p.IdCli=c.Id_Cli)
                    LEFT JOIN rota_cliente rc ON (c.Id_Cli=rc.IdCliente)
                    LEFT JOIN rota r ON (rc.IdRota=r.IdRota)
                    LEFT JOIN pedido_espelho pe ON (p.IdPedido=pe.IdPedido)
                    LEFT JOIN produtos_pedido pp ON (p.IdPedido=pp.IdPedido)
                    LEFT JOIN produtos_liberar_pedido plp ON (pp.IdProdPed=plp.IdProdPed)
                    LEFT JOIN liberarpedido lp ON (plp.IdLiberarPedido=lp.IdLiberarPedido AND lp.Situacao IS NOT NULL AND lp.Situacao=1)
                    LEFT JOIN ambiente_pedido ap ON (pp.IdAmbientePedido=ap.IdAmbientePedido)
                    LEFT JOIN funcionario fc ON (c.IdFunc=fc.IdFunc)
                    LEFT JOIN funcionario f ON (p.IdFunc=f.IdFunc) 
                    LEFT JOIN loja l ON (p.IdLoja = l.IdLoja) 
                    LEFT JOIN comissionado com ON (p.IdComissionado=com.IdComissionado)
                    {1}
                WHERE p.Situacao IN ({2}) AND p.TipoVenda IN ({3},{4},{5}) ?filtroAdicional?",
                    string.Format("{0}{1}", campos, campoDadosVendidos), dadosVendidos, situacoes, (int)Pedido.TipoVendaPedido.AVista, (int)Pedido.TipoVendaPedido.APrazo,
                    (int)Pedido.TipoVendaPedido.Obra);

            if (dataInicioSituacao != "")
            {
                criterio += string.Format(formatoCriterio, "Data Inicio :", dataInicioSituacao);
            }

            if (dataFimSituacao != "")
            {
                criterio += string.Format(formatoCriterio, "Data Fim :", dataFimSituacao);
            }

            if (idCliente > 0)
            {
                filtroAdicional += string.Format(" AND p.IdCli={0}", idCliente);
                whereDadosVendidos += string.Format(" AND ped.IdCli={0}", idCliente);
                criterio += string.Format(formatoCriterio, "Cliente :", ClienteDAO.Instance.GetNome((uint)idCliente));
            }
            else if (!string.IsNullOrEmpty(nomeCliente))
            {
                var ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);

                filtroAdicional += string.Format(" AND p.IdCli IN ({0})", ids);
                whereDadosVendidos += string.Format(" AND ped.IdCli IN ({0})", ids);
            }

            if (idLoja > 0)
            {
                filtroAdicional += string.Format(" AND p.IdLoja={0}", idLoja);
                whereDadosVendidos += string.Format(" AND ped.IdLoja={0}", idLoja);
            }

            #region Filtro por data de situação

            var filtro = FiltroDataSituacao(dataInicioSituacao, dataFimSituacao, situacoes, "?dtIniSit", "?dtFimSit", "p", "lp", " Sit.", calcularLiberados);
            sql += filtro.Sql;

            whereDadosVendidos += FiltroDataSituacao(dataInicioSituacao, dataFimSituacao, situacoes, "?dtIniSit", "?dtFimSit", "ped", "lp1", " Sit.", true).Sql;
            temFiltro = temFiltro || filtro.Sql != "";

            #endregion

            if (idFunc > 0)
            {
                filtroAdicional += string.Format(" AND p.IdFunc={0}", idFunc);
                whereDadosVendidos += string.Format(" AND ped.IdFunc={0}", idFunc);
                criterio += string.Format(formatoCriterio, "Funcionário:", FuncionarioDAO.Instance.GetNome((uint)idFunc));
            }

            if (idVendAssoc > 0)
            {
                sql += string.Format(" AND c.IdFunc={0}", idVendAssoc);
                whereDadosVendidos += string.Format(" AND cli.IdFunc={0}", idVendAssoc);
                criterio += string.Format(formatoCriterio, "Vendedor associado:", FuncionarioDAO.Instance.GetNome((uint)idVendAssoc));
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(tipoPedido))
            {
                filtroAdicional += string.Format(" AND p.TipoPedido IN ({0})", tipoPedido);
                whereDadosVendidos += string.Format(" AND ped.TipoPedido IN ({0})", tipoPedido);
            }

            if (idRota > 0)
            {
                filtroAdicional += string.Format(" AND rc.IdRota={0}", idRota);
                whereDadosVendidos += string.Format(" AND rtc.IdRota={0}", idRota);
                criterio += string.Format(formatoCriterio, "Rota:", RotaDAO.Instance.ObterDescricaoRota(null, (uint)idRota));
            }

            sql += " GROUP BY p.IdPedido";

            sql = string.Format(sql, whereDadosVendidos);

            return sql.Replace("$$$", criterio); ;
        }

        #endregion

        #region Acesso externo

        private string SqlAcessoExterno(uint idPedido, string codCliente, DateTime? dtIni, DateTime? dtFim, bool apenasAbertos,
            bool selecionar, bool countLimit, out bool temFiltro, out string filtroAdicional, LoginUsuario login)
        {
            temFiltro = false;
            filtroAdicional = "";

            var idCliente = UserInfo.GetUserInfo.IdCliente;

            if (idCliente == null || idCliente == 0)
                return null;

            var cliente = login.IsCliente;
            var administrador = login.IsAdministrador;
            var emitirGarantiaReposicao = Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoGarantiaReposicao);
            var emitirPedidoFuncionario = Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoFuncionario);

            var dataInicio = dtIni != null ? dtIni.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : "";
            var dataFinal = dtFim != null ? dtFim.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : "";

            var sql = !apenasAbertos ? SqlRptSit(idPedido, null, 0, codCliente, null, idCliente.Value.ToString(), null, 0, null, null, null,
                null, dataInicio, dataFinal, null, null, 0, 0, null, 0, 0, 0, null, null, 0, null, null, false, false, false, null, null, 0, null, null, 0, 0,
                null, null, null, null, false, 0, 0, selecionar, countLimit, false, false, out temFiltro, out filtroAdicional, 0, null, 0, false, 0, null,
                cliente, administrador, emitirGarantiaReposicao, emitirPedidoFuncionario) : "";

            // Retira os pedidos liberados/confirmados da lista de clientes
            if (!PedidoConfig.ExibirPedidosLiberadosECommerce)
                filtroAdicional += " and p.situacao<>" + (int)Pedido.SituacaoPedido.Confirmado;

            // Exibe os pedidos que não tenham sido entregues ainda
            if (PedidoConfig.ExibirPedidosNaoEntregueCommerce)
                filtroAdicional += " and (p.situacao<>" + (int)Pedido.SituacaoPedido.Confirmado +
                    " Or (p.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Venda + " and p.situacaoProducao<>" + (int)Pedido.SituacaoProducaoEnum.Entregue + "))";

            return sql;
        }

        /// <summary>
        /// Retorna pedidos para acesso externo
        /// </summary>
        public Pedido[] GetListAcessoExterno(uint idPedido, string codCliente, DateTime? dtIni, DateTime? dtFim, bool apenasAbertos,
            string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlAcessoExterno(idPedido, codCliente, dtIni, dtFim, apenasAbertos, true, false, out temFiltro,
                out filtroAdicional, UserInfo.GetUserInfo).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            sortExpression = string.IsNullOrEmpty(sortExpression) ? "idPedido desc" : string.Empty;

            var lstPedido = LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional,
                ObterParametrosFiltrosAcessoExterno(codCliente, dtFim, dtIni)).ToArray();

            foreach (var p in lstPedido)
                if (p.TemEspelho)
                    p.TotalEspelho = PedidoEspelhoDAO.Instance.ObtemValorCampo<decimal>("total", "idPedido=" + p.IdPedido);

            return lstPedido;
        }

        public int GetAcessoExternoCount(uint idPedido, string codCliente, DateTime? dtIni, DateTime? dtFim, bool apenasAbertos)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlAcessoExterno(idPedido, codCliente, dtIni, dtFim, apenasAbertos, true, true, out temFiltro,
                out filtroAdicional, UserInfo.GetUserInfo).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, ObterParametrosFiltrosAcessoExterno(codCliente, dtFim, dtIni));
        }

        #region Parâmetros

        /// <summary>
        /// Retorna os parâmetros que devem ser substituídos no SQL, com base nos filtros informados.
        /// </summary>
        private GDAParameter[] ObterParametrosFiltrosAcessoExterno(string codCliente, DateTime? dataFimPedido, DateTime? dataInicioPedido)
        {
            var parametros = new List<GDAParameter>();

            if (!string.IsNullOrWhiteSpace(codCliente))
                parametros.Add(new GDAParameter("?codCliente", "%" + codCliente + "%"));

            if (dataFimPedido.HasValue)
                parametros.Add(new GDAParameter("?dtFim", dataFimPedido.Value.Date.AddDays(1).AddSeconds(-1)));

            if (dataInicioPedido.HasValue)
                parametros.Add(new GDAParameter("?dtIni", dataInicioPedido.Value.Date));

            return parametros.Count > 0 ? parametros.ToArray() : null;
        }

        #endregion

        #endregion

        #region Relatório de Pedidos Com Lucratividade/Sem Lucratividade

        internal string SqlLucr(string idLoja, string idVendedor, int situacao, string dtIni, string dtFim, int tipoVenda, int agruparFunc,
            bool selecionar, out bool temFiltro, out string filtroAdicional)
        {
            return SqlLucr(idLoja, idVendedor, 0, 0, null, situacao, dtIni, dtFim, tipoVenda, agruparFunc,
            selecionar, out temFiltro, out filtroAdicional);
        }

        internal string SqlLucr(string idLoja, string idVendedor, int idPedido, int idCliente, string nomeCliente, int situacao, string dtIni,
            string dtFim, int tipoVenda, int agruparFunc, bool selecionar, out bool temFiltro, out string filtroAdicional)
        {
            idLoja = !string.IsNullOrEmpty(idLoja) && idLoja != "0" ? idLoja : "todas";
            var tv = tipoVenda != 0 && tipoVenda != 6 ? tipoVenda.ToString() :
                tipoVenda == 6 ? (int)Pedido.TipoVendaPedido.AVista + "," + (int)Pedido.TipoVendaPedido.APrazo : "";

            var sit = situacao != 99 && situacao != 5 ? situacao.ToString() : (int)Pedido.SituacaoPedido.Confirmado +
                "," + (int)Pedido.SituacaoPedido.LiberadoParcialmente + (situacao != 5 ? "," + (int)Pedido.SituacaoPedido.ConfirmadoLiberacao : "");

            var login = UserInfo.GetUserInfo;
            var cliente = login.IsCliente;
            var administrador = login.IsAdministrador;
            var emitirGarantiaReposicao = Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoGarantiaReposicao);
            var emitirPedidoFuncionario = Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoFuncionario);

            var sql = SqlRptSit((uint)idPedido, null, 0, null, null, idCliente > 0 ? idCliente.ToString() : null, nomeCliente, 0, idLoja, sit, dtIni, dtFim, null, null, null, null, 0,
                0, null, 0, 0, 0, null, tv, 0, null, null, false, false, false, null, null, 0, null, null, 0, 0, null, null, null, null, false, 0, 0, selecionar,
                !selecionar, false, false, out temFiltro, out filtroAdicional, 0, null, 0, true, 0, null,
                cliente, administrador, emitirGarantiaReposicao, emitirPedidoFuncionario).Replace(" as Criterio", " as Criterio1, '$$$' as Criterio");

            filtroAdicional += " and p.tipoPedido<>" + (int)Pedido.TipoPedidoEnum.Producao;
            var criterio = idLoja != "todas" ? "Loja: " + LojaDAO.Instance.GetNome(idLoja.StrParaUint()) + "    " : "Loja: Todas    ";

            // Separa o group by do sql, para fazer o filtro
            var groupBy = sql.Substring(sql.LastIndexOf("group by", StringComparison.Ordinal));
            sql = sql.Substring(0, sql.LastIndexOf("group by", StringComparison.Ordinal));

            if (idVendedor != "0")
            {
                switch (agruparFunc)
                {
                    case 0:
                        filtroAdicional += " And p.idFunc=" + idVendedor;
                        criterio += "Emissor: " + BibliotecaTexto.GetTwoFirstNames(FuncionarioDAO.Instance.GetNome(idVendedor.StrParaUint())) + "    ";
                        break;
                    case 1:
                        sql += " And c.idFunc=" + idVendedor;
                        criterio += "Vendedor: " + BibliotecaTexto.GetTwoFirstNames(FuncionarioDAO.Instance.GetNome(idVendedor.StrParaUint())) + "    ";
                        temFiltro = true;
                        break;
                    case 2:
                        filtroAdicional += " And p.idComissionado=" + idVendedor;
                        criterio += "Comissionado: " + BibliotecaTexto.GetTwoFirstNames(ComissionadoDAO.Instance.GetNome(idVendedor.StrParaUint())) + "    ";
                        break;
                }
            }
            else
            {
                switch (agruparFunc)
                {
                    case 0:
                        criterio += "Emissor: Todos    ";
                        break;
                    case 1:
                        criterio += "Vendedor: Todos    ";
                        break;
                    case 2:
                        filtroAdicional += " and p.idComissionado is not null";
                        criterio += "Comissionado: Todos    ";
                        break;
                }
            }

            if (idPedido > 0)
                criterio += string.Format("Pedido: {0}    ", idPedido);

            if (idCliente > 0)
                criterio += string.Format("Cliente: {0}    ", ClienteDAO.Instance.GetNome((uint)idCliente));
            else if (!string.IsNullOrEmpty(nomeCliente))
                criterio += string.Format("Cliente: {0}    ", nomeCliente);

            if (!string.IsNullOrEmpty(dtIni))
            {
                criterio += "Data Início " + (!PedidoConfig.LiberarPedido || situacao == (int)Pedido.SituacaoPedido.ConfirmadoLiberacao ?
                    "Conf." : situacao == 99 ? "Conf./Lib." : "Liberação") + ": " + dtIni + "    ";
            }

            if (!string.IsNullOrEmpty(dtFim))
            {
                criterio += "Data Fim " + (!PedidoConfig.LiberarPedido || situacao == (int)Pedido.SituacaoPedido.ConfirmadoLiberacao ?
                    "Conf." : situacao == 99 ? "Conf./Lib." : "Liberação") + ": " + dtFim + "    ";
            }

            if (situacao > 0)
                criterio += "Situação: " + (situacao != 99 ? PedidoDAO.Instance.GetSituacaoPedido(situacao) : "Confirmado/Liberado") + "    ";

            if (tipoVenda == 0)
                // Desconsidera pedidos de reposição e de garantia
                filtroAdicional += " And p.TipoVenda not in (" + (int)Pedido.TipoVendaPedido.Garantia + "," + (int)Pedido.TipoVendaPedido.Reposição + ")";
            else
            {
                switch (tipoVenda)
                {
                    case 1:
                        criterio += "Tipo Venda: À Vista    "; break;
                    case 2:
                        criterio += "Tipo Venda: À Prazo    "; break;
                    case 3:
                        criterio += "Tipo Venda: Reposição    "; break;
                    case 4:
                        criterio += "Tipo Venda: Garantia    "; break;
                    case 5:
                        criterio += "Tipo Venda: Obra    "; break;
                    case 6:
                        criterio += "Tipo Venda: À Vista/à prazo    "; break;
                }
            }

            return sql.Replace("$$$", criterio) + " " + groupBy;
        }

        /// <summary>
        /// Busca pedido que serão usado no relatório de vendas com e sem lucratividade
        /// </summary>
        public Pedido[] GetForRptLucr(string idLoja, string idVendedor, int situacao, string dtIni, string dtFim,
            int tipoVenda, int agruparFunc, string orderBy)
        {
            return GetForRptLucr(idLoja, idVendedor, 0, 0, null, situacao, dtIni, dtFim, tipoVenda, agruparFunc, orderBy);
        }

        /// <summary>
        /// Busca pedido que serão usado no relatório de vendas com e sem lucratividade
        /// </summary>
        public Pedido[] GetForRptLucr(string idLoja, string idVendedor, int idPedido, int idCliente, string nomeCliente,
            int situacao, string dtIni, string dtFim, int tipoVenda, int agruparFunc, string orderBy)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlLucr(idLoja, idVendedor, idPedido, idCliente, nomeCliente, situacao, dtIni, dtFim, tipoVenda,
                agruparFunc, true, out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            if (idVendedor == "0" && agruparFunc == 1)
            {
                var groupBy = sql.Substring(sql.LastIndexOf("group by", StringComparison.Ordinal));
                sql = sql.Substring(0, sql.LastIndexOf("group by", StringComparison.Ordinal));
                sql += " and c.idFunc IS NULL ";
                sql += groupBy;
            }

            switch (orderBy)
            {
                case "1":
                    sql += PedidoConfig.LiberarPedido ? " Order By lp.dataLiberacao Desc" : " Order By p.dataConf Desc"; break;
                case "2":
                    sql += " Order By c.nome"; break;
                case "3":
                    sql += " Order By p.idPedido"; break;
            }

            var retorno = objPersistence.LoadData(sql, GetParamLucr(dtIni, dtFim)).ToArray();

            if (retorno.Length > 0 && (!string.IsNullOrEmpty(dtIni) || !string.IsNullOrEmpty(dtFim)))
            {
                sql = "select count(distinct date(p.dataCad)) from pedido p inner join cliente c on (p.idCli=c.id_Cli) where 1";

                if (!string.IsNullOrEmpty(dtIni))
                    sql += " and p.dataCad>=?dtIniSit";
                if (!string.IsNullOrEmpty(dtFim))
                    sql += " and p.dataCad<=?dtFimSit";

                if (idVendedor != "0")
                    sql += " and " + (agruparFunc == 0 ? "p" : "c") + ".idFunc=" + idVendedor;

                if (idVendedor == "0" && agruparFunc == 1)
                {
                    sql += " and c.idFunc IS NULL";
                }

                retorno[0].NumDias = ExecuteScalar<int>(sql, GetParamLucr(dtIni, dtFim));
            }

            return retorno;
        }

        public Pedido[] GetForRptSemImposto(string idLoja, string idVendedor, string dtIni, string dtFim, int tipoVenda, string orderBy)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlLucr(idLoja, idVendedor, (int)Pedido.SituacaoPedido.Confirmado, dtIni, dtFim, tipoVenda, 0, true,
                out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            switch (orderBy)
            {
                case "1":
                    sql += PedidoConfig.LiberarPedido ? " Order By lp.dataLiberacao Desc" : " Order By p.dataConf Desc"; break;
                case "2":
                    sql += " Order By c.nome"; break;
                case "3":
                    sql += " Order By p.idPedido"; break;
            }

            var retorno = objPersistence.LoadData(sql, GetParamLucr(dtIni, dtFim)).ToArray();

            if (retorno.Length > 0 && (!string.IsNullOrEmpty(dtIni) || !string.IsNullOrEmpty(dtFim)))
            {
                sql = "select count(distinct date(dataCad)) from pedido where 1";

                if (!string.IsNullOrEmpty(dtIni))
                    sql += " and dataCad>=?dtIniSit";
                if (!string.IsNullOrEmpty(dtFim))
                    sql += " and dataCad<=?dtFimSit";

                if (idVendedor != "0")
                    sql += " and idFunc=" + idVendedor;

                retorno[0].NumDias = ExecuteScalar<int>(sql, GetParamLucr(dtIni, dtFim));
            }

            return retorno;
        }

        public Pedido[] GetListLucr(string idLoja, string idVendedor, string dtIni, string dtFim, int tipoVenda, string orderBy, string sortExpression, int startRow, int pageSize)
        {
            return GetListLucr(idLoja, idVendedor, 0, 0, null, dtIni, dtFim, tipoVenda, orderBy, sortExpression, startRow, pageSize);
        }

        public Pedido[] GetListLucr(string idLoja, string idVendedor, int idPedido, int idCliente, string nomeCliente, string dtIni, string dtFim,
            int tipoVenda, string orderBy, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlLucr(idLoja, idVendedor, idPedido, idCliente, nomeCliente, (int)Pedido.SituacaoPedido.Confirmado, dtIni, dtFim, tipoVenda, 0, true,
                out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            if (string.IsNullOrEmpty(sortExpression))
                switch (orderBy)
                {
                    case "1":
                        sortExpression = PedidoConfig.LiberarPedido ? "lp.dataLiberacao Desc" : "p.dataConf Desc"; break;
                    case "2":
                        sortExpression = "c.nome"; break;
                    case "3":
                        sortExpression = "p.idPedido"; break;
                }

            var retorno = LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional, GetParamLucr(dtIni, dtFim)).ToArray();

            if (retorno.Length > 0 && (!string.IsNullOrEmpty(dtIni) || !string.IsNullOrEmpty(dtFim)))
            {
                sql = "select count(distinct date(dataCad)) from pedido where 1";

                if (!string.IsNullOrEmpty(dtIni))
                    sql += " and dataCad>=?dtIniSit";
                if (!string.IsNullOrEmpty(dtFim))
                    sql += " and dataCad<=?dtFimSit";

                if (idVendedor != "0")
                    sql += " and idFunc=" + idVendedor;

                retorno[0].NumDias = ExecuteScalar<int>(sql, GetParamLucr(dtIni, dtFim));
            }

            return retorno;
        }

        public int GetCountLucr(string idLoja, string idVendedor, string dtIni, string dtFim, int tipoVenda, string orderBy)
        {
            return GetCountLucr(idLoja, idVendedor, 0, 0, null, dtIni, dtFim, tipoVenda, orderBy);
        }

        public int GetCountLucr(string idLoja, string idVendedor, int idPedido, int idCliente, string nomeCliente, string dtIni,
            string dtFim, int tipoVenda, string orderBy)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlLucr(idLoja, idVendedor, idPedido, idCliente, nomeCliente, (int)Pedido.SituacaoPedido.Confirmado, dtIni, dtFim, tipoVenda, 0, true,
                out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, GetParamLucr(dtIni, dtFim));
        }

        public Pedido[] GetListSemImposto(string idLoja, string idVendedor, string dtIni, string dtFim, int tipoVenda, string orderBy, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlLucr(idLoja, idVendedor, (int)Pedido.SituacaoPedido.Confirmado, dtIni, dtFim, tipoVenda, 0, true,
                out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            var retorno = LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro,
                filtroAdicional, GetParamLucr(dtIni, dtFim)).ToArray();

            if (retorno.Length > 0 && (!string.IsNullOrEmpty(dtIni) || !string.IsNullOrEmpty(dtFim)))
            {
                sql = "select count(distinct date(dataCad)) from pedido where 1";

                if (!string.IsNullOrEmpty(dtIni))
                    sql += " and dataCad>=?dtIniSit";
                if (!string.IsNullOrEmpty(dtFim))
                    sql += " and dataCad<=?dtFimSit";

                if (idVendedor != "0")
                    sql += " and idFunc=" + idVendedor;

                retorno[0].NumDias = ExecuteScalar<int>(sql, GetParamLucr(dtIni, dtFim));
            }

            return retorno;
        }

        public int GetCountSemImposto(string idLoja, string idVendedor, string dtIni, string dtFim, int tipoVenda, string orderBy)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlLucr(idLoja, idVendedor, (int)Pedido.SituacaoPedido.Confirmado, dtIni, dtFim, tipoVenda, 0, true,
                out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, GetParamLucr(dtIni, dtFim));
        }

        private GDAParameter[] GetParamLucr(string dtIni, string dtFim)
        {
            var lstParam = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(dtIni))
                lstParam.Add(new GDAParameter("?dtIniSit", DateTime.Parse(dtIni + " 00:00")));

            if (!string.IsNullOrEmpty(dtFim))
                lstParam.Add(new GDAParameter("?dtFimSit", DateTime.Parse(dtFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Relatórios de Sinais

        private string SqlSinaisRecebidos(uint idCli, uint idPedido, uint idFunc, string dataIniRec, string dataFimRec,
            bool recebidos, bool pagtoAntecipado, bool selecionar, out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = recebidos;

            filtroAdicional = string.Format(" And p.situacao<>{0}{1}",
                (int)Pedido.SituacaoPedido.Cancelado,
                recebidos ? "" : " And p.situacao<>" + (int)Pedido.SituacaoPedido.Confirmado);

            if (pagtoAntecipado)
                filtroAdicional += string.Format(" And p.IdPagamentoAntecipado IS {0}", recebidos ? "NOT NULL" : "NULL");
            else
                filtroAdicional += string.Format(" And p.IdSinal IS {0} And p.valorEntrada>0 And (p.valorPagamentoAntecipado<>p.total Or p.idPagamentoAntecipado is null)", recebidos ? "NOT NULL" : "NULL");

            var campos = selecionar ? "p.*, pe.total as totalEspelho, " + ClienteDAO.Instance.GetNomeCliente("c") + @" as NomeCliente, '$$$' as Criterio, s.dataCad as dataEntrada,
                s.usuCad as usuEntrada, cast(s.valorCreditoAoCriar as decimal(12,2)) as valorCreditoAoReceberSinal, cast(s.creditoGeradoCriar as decimal(12,2)) as creditoGeradoReceberSinal,
                cast(s.creditoUtilizadoCriar as decimal(12,2)) as creditoUtilizadoReceberSinal, s.isPagtoAntecipado as pagamentoAntecipado" : "Count(*)";

            var criterio = string.Empty;

            var sql = "Select " + campos + @"
                From pedido p 
                    Inner Join cliente c On (p.idCli=c.id_Cli)
                    Left Join pedido_espelho pe On (p.idPedido=pe.idPedido)
                    Left Join sinal s On (" + (pagtoAntecipado ? "p.idPagamentoAntecipado" : "p.idSinal") + @"=s.idSinal)
                Where 1 ?filtroAdicional?
                    " + (recebidos ? "And (s.isPagtoAntecipado=" + (pagtoAntecipado ? "1)" : "0 or s.isPagtoAntecipado is null)") : "");

            if (idCli > 0)
            {
                sql += " And idCli=" + idCli;
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCli) + "    ";
                temFiltro = true;
            }

            if (idPedido > 0)
            {
                sql += " And p.idPedido=" + idPedido;
                criterio += "Num. Pedido: " + idPedido + "    ";
                temFiltro = true;
            }

            if (idFunc > 0)
            {
                sql += " And s.usuCad=" + idFunc;
                criterio += "Funcionário: " + FuncionarioDAO.Instance.GetNome(idFunc) + "    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(dataIniRec))
            {
                sql += " And s.dataCad>=?dataIniRec";
                criterio += "Data Ini. Rec.: " + dataIniRec + "    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(dataFimRec))
            {
                sql += " And s.dataCad<=?dataFimRec";
                criterio += "Data Fim Rec.: " + dataFimRec + "    ";
                temFiltro = true;
            }

            return sql.Replace("$$$", criterio);
        }

        /// <summary>
        /// Retorna uma lista com os sinais a serem recebidos para o relatorio
        /// </summary>
        /// <returns></returns>
        public Pedido[] GetSinaisNaoRecebidosRpt(uint idCli, uint idPedido, bool pagtoAntecipado)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlSinaisRecebidos(idCli, idPedido, 0, null, null, false, pagtoAntecipado, true, out temFiltro, out filtroAdicional).
                Replace("?filtroAdicional?", filtroAdicional) + " order by coalesce(p.dataPedido, p.dataCad)";

            return objPersistence.LoadData(sql).ToArray();
        }

        /// <summary>
        /// Retorna uma lista com os sinais a serem recebidos
        /// </summary>
        /// <returns></returns>
        public IList<Pedido> GetSinaisNaoRecebidos(uint idCli, uint idPedido, bool pagtoAntecipado, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;

            sortExpression = !string.IsNullOrEmpty(sortExpression) ? sortExpression : "coalesce(p.dataPedido, p.dataCad)";

            var sql = SqlSinaisRecebidos(idCli, idPedido, 0, null, null, false, pagtoAntecipado, true, out temFiltro, out filtroAdicional).
                Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional);
        }

        public int GetSinaisNaoRecebidosCount(uint idCli, uint idPedido, bool pagtoAntecipado)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlSinaisRecebidos(idCli, idPedido, 0, null, null, false, pagtoAntecipado, true, out temFiltro, out filtroAdicional).
                Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional);
        }

        /// <summary>
        /// Retorna uma lista com os sinais a serem recebidos para o relatorio
        /// </summary>
        /// <returns></returns>
        public Pedido[] GetSinaisRecebidosRpt(uint idCli, uint idPedido, uint idFunc, string dataIniRec, string dataFimRec, bool pagtoAntecipado, int ordenacao)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlSinaisRecebidos(idCli, idPedido, idFunc, dataIniRec, dataFimRec, true, pagtoAntecipado, true, out temFiltro,
                out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            var filtro = string.Empty;
            switch (ordenacao)
            {
                case 0: //Nenhum
                    filtro = " ORDER BY COALESCE(p.dataPedido, p.dataCad) DESC";
                    break;
                case 1: //Pedido
                    filtro = " ORDER BY p.IdPedido";
                    break;
                case 2: //Cliente
                    filtro = " ORDER BY p.IdCli";
                    break;
                case 3: //Data Recebimento
                    filtro = " ORDER BY s.DataCad";
                    break;
                default:
                    break;
            }

            sql += filtro;

            return objPersistence.LoadData(sql, GetParamSinalRec(dataIniRec, dataFimRec)).ToArray();
        }

        /// <summary>
        /// Retorna uma lista com os sinais a serem recebidos
        /// </summary>
        /// <returns></returns>
        public IList<Pedido> GetSinaisRecebidos(uint idCli, uint idPedido, string dataIniRec, string dataFimRec, bool pagtoAntecipado,
            string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;

            sortExpression = !string.IsNullOrEmpty(sortExpression) ? sortExpression : "coalesce(p.dataPedido, p.dataCad) desc";

            var sql = SqlSinaisRecebidos(idCli, idPedido, 0, dataIniRec, dataFimRec, true, pagtoAntecipado, true, out temFiltro,
                out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional,
                GetParamSinalRec(dataIniRec, dataFimRec));
        }

        public int GetSinaisRecebidosCount(uint idCli, uint idPedido, string dataIniRec, string dataFimRec, bool pagtoAntecipado)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlSinaisRecebidos(idCli, idPedido, 0, dataIniRec, dataFimRec, true, pagtoAntecipado, true, out temFiltro,
                out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, GetParamSinalRec(dataIniRec, dataFimRec));
        }

        public GDAParameter[] GetParamSinalRec(string dataIniRec, string dataFimRec)
        {
            var lstParam = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(dataIniRec))
                lstParam.Add(new GDAParameter("?dataIniRec", (dataIniRec.Length == 10 ? DateTime.Parse(dataIniRec = dataIniRec + " 00:00") : DateTime.Parse(dataIniRec))));

            if (!string.IsNullOrEmpty(dataFimRec))
                lstParam.Add(new GDAParameter("?dataFimRec", (dataFimRec.Length == 10 ? DateTime.Parse(dataFimRec = dataFimRec + " 23:59:59") : DateTime.Parse(dataFimRec))));

            return lstParam.ToArray();
        }

        #endregion

        #region Busca ids dos pedidos para tela de recebimento de sinal

        private string SqlReceberSinal(string idsPedidos, uint idCliente, string nomeCliente, string idsPedidosRem,
            string dataIniEntrega, string dataFimEntrega, bool isSinal, bool forList, out bool temFiltro, out string filtroAdicional)
        {
            var sql = SqlSinaisRecebidos(idCliente, 0, 0, null, null, false, !isSinal, true, out temFiltro, out filtroAdicional);
            if (sql.Contains(" order by"))
                sql = sql.Remove(sql.IndexOf(" order by", StringComparison.Ordinal));

            if (forList && string.IsNullOrEmpty(idsPedidos))
                idsPedidos = "0";

            if (forList || !string.IsNullOrEmpty(idsPedidos))
                filtroAdicional += " and p.idPedido in (" + idsPedidos + ")";

            if (idCliente == 0 && !string.IsNullOrEmpty(nomeCliente))
            {
                var ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                filtroAdicional += " And p.idCli in (" + ids + ")";
            }

            if (!string.IsNullOrEmpty(dataIniEntrega))
                filtroAdicional += " and p.dataEntrega>=?dataIniEntrega";

            if (!string.IsNullOrEmpty(dataFimEntrega))
                filtroAdicional += " and p.dataEntrega<=?dataFimEntrega";

            if (!string.IsNullOrEmpty(idsPedidosRem))
                filtroAdicional += " and p.idPedido not in (" + idsPedidosRem.TrimEnd(',') + ")";

            return sql;
        }

        /// <summary>
        /// Busca ids dos pedidos para tela de recebimento de sinal.
        /// </summary>
        public string GetIdsPedidosForReceberSinal(uint idCliente, string nomeCliente, string idsPedidosRem,
            string dataIniEntrega, string dataFimEntrega, bool isSinal)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlReceberSinal(null, idCliente, nomeCliente, idsPedidosRem, dataIniEntrega, dataFimEntrega,
                isSinal, false, out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            var ids = objPersistence.LoadResult(sql, new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"),
                new GDAParameter("?dataIniEntrega", DateTime.Parse(dataIniEntrega + " 00:00")),
                new GDAParameter("?dataFimEntrega", DateTime.Parse(dataFimEntrega + " 23:59"))).Select(f => f.GetUInt32(0))
                       .ToList(); ;

            return string.Join(",", Array.ConvertAll(ids.ToArray(), (
                x => x.ToString()
                )));
        }

        public Pedido[] GetForReceberSinal(string idsPedidos, bool isSinal)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlReceberSinal(idsPedidos, 0, null, null, null, null, isSinal, true,
                out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadData(sql).ToArray();
        }

        #endregion

        #region Relatório de obra

        /// <summary>
        /// Busca pedidos relacionados à obra passada
        /// </summary>
        public Pedido[] GetForRptObra(uint idObra)
        {
            var sql = @"
                Select p.*, " + ClienteDAO.Instance.GetNomeCliente("c") + @" as NomeCliente, c.Revenda as CliRevenda, f.Nome as NomeFunc, 
                    c.Tel_Cont as rptTelCont, c.Tel_Res as rptTelRes, c.Tel_Cel as rptTelCel, 
                    l.NomeFantasia as nomeLoja, fp.Descricao as FormaPagto
                From pedido p 
                    Inner Join cliente c On (p.idCli=c.id_Cli) 
                    Inner Join funcionario f On (p.IdFunc=f.IdFunc) 
                    Inner Join loja l On (p.IdLoja = l.IdLoja) 
                    Left Join formapagto fp On (fp.IdFormaPagto=p.IdFormaPagto) 
                Where p.idObra=" + idObra + " and p.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado;

            return objPersistence.LoadData(sql).ToArray();
        }

        #endregion

        #region Busca pedidos para tela de seleção

        private string SqlSel(uint idPedido, uint idFunc, uint idCliente, string nomeCliente, int tipo, bool selecionar,
            out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = false;
            filtroAdicional = "";

            var campos = selecionar ? "p.*, " + ClienteDAO.Instance.GetNomeCliente("c") + @" as NomeCliente, f.Nome as nomeFunc" : "Count(*)";

            var sql = "Select " + campos + @" From pedido p 
                Inner Join funcionario f On (p.IdFunc=f.IdFunc) 
                Inner Join cliente c On (p.idCli=c.id_Cli) Where 1 ?filtroAdicional?";

            if (idCliente > 0)
                filtroAdicional += " And p.idCli=" + idCliente;
            else if (!string.IsNullOrEmpty(nomeCliente))
            {
                var ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                filtroAdicional += " And p.idCli in (" + ids + ")";
            }

            if (idPedido > 0)
                filtroAdicional += " And p.idPedido=" + idPedido;

            if (idFunc > 0)
                filtroAdicional += " And p.idFunc=" + idFunc;

            if (tipo == 1)
                filtroAdicional += " And p.valorEntrada>0 And p.tipoVenda=2 And p.idSinal is null And p.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado;
            else if (tipo == 2)
                filtroAdicional += " And p.situacao=" + (int)Pedido.SituacaoPedido.Conferido;
            else if (tipo == 3)
                filtroAdicional += " And p.situacao=" + (int)Pedido.SituacaoPedido.Confirmado + " And p.tipoVenda=" + (int)Pedido.TipoVendaPedido.APrazo;
            else if (tipo == 4)
                filtroAdicional += " And p.situacao in (" + (int)Pedido.SituacaoPedido.Confirmado + "," + (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + "," +
                    (int)Pedido.SituacaoPedido.LiberadoParcialmente + ")";
            else if (tipo == 5)
            {
                sql += " And c.pagamentoAntesProducao=true and p.idSinal is null and p.situacao not in (" +
                    (int)Pedido.SituacaoPedido.Cancelado + "," + (int)Pedido.SituacaoPedido.Confirmado + ")";

                temFiltro = true;
            }
            else if (tipo == 6)
                filtroAdicional += string.Format(" AND p.Situacao NOT IN ({0},{1})", (int)Pedido.SituacaoPedido.Confirmado, (int)Pedido.SituacaoPedido.Cancelado);

            return sql;
        }

        /// <summary>
        /// Retorna pedidos para tela de seleção
        /// </summary>
        public IList<Pedido> GetListSel(uint idPedido, uint idFunc, uint idCliente, string nomeCliente, int tipo, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlSel(idPedido, idFunc, idCliente, nomeCliente, tipo, true, out temFiltro, out filtroAdicional).
                Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional, GetParamSel(nomeCliente));
        }

        public int GetCountSel(uint idPedido, uint idFunc, uint idCliente, string nomeCliente, int tipo)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlSel(idPedido, idFunc, idCliente, nomeCliente, tipo, true, out temFiltro, out filtroAdicional).
                Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, GetParamSel(nomeCliente));
        }

        private GDAParameter[] GetParamSel(string nomeCliente)
        {
            var lstParam = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(nomeCliente))
                lstParam.Add(new GDAParameter("?nome", "%" + nomeCliente + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Pedido para confirmação

        /// <summary>
        /// Retorna o pedido a ser confirmado
        /// </summary>
        public Pedido GetForConfirmation(uint idPedido)
        {
            if (idPedido == 0)
                return null;

            if (!PedidoExists(idPedido))
                throw new Exception("Não foi encontrado nenhum pedido com o número informado.");

            bool temFiltro;
            string filtroAdicional;

            var pedido = objPersistence.LoadOneData(Sql(idPedido, 0, null, null, 0, 0, null, 0, null, 0, null, null, null, null, null,
                string.Empty, string.Empty, string.Empty, "2", null, null, null, null, null, 0, true, false, 0, 0, 0, 0, 0,
                null, 0, 0, 0, "", true, out filtroAdicional, out temFiltro).Replace("?filtroAdicional?", filtroAdicional));

            if (pedido.Situacao == Pedido.SituacaoPedido.Cancelado)
                throw new Exception("Este pedido foi cancelado.");

            #region Busca as parcelas do pedido

            var lstParc = ParcelasPedidoDAO.Instance.GetByPedido(idPedido).ToArray();

            var parcelas = lstParc.Length + " vez(es): ";

            pedido.ValoresParcelas = new decimal[lstParc.Length];
            pedido.DatasParcelas = new DateTime[lstParc.Length];

            for (int i = 0; i < lstParc.Length; i++)
            {
                pedido.ValoresParcelas[i] = lstParc[i].Valor;
                pedido.DatasParcelas[i] = lstParc[i].Data != null ? lstParc[i].Data.Value : new DateTime();
                parcelas += lstParc[i].Valor.ToString("c") + "-" + (lstParc[i].Data != null ? lstParc[i].Data.Value.ToString("d") : "") + ",  ";
            }

            if (lstParc.Length > 0 && pedido.TipoVenda != (int)Pedido.TipoVendaPedido.AVista)
                pedido.DescrParcelas = parcelas.TrimEnd(' ').TrimEnd(',');

            #endregion

            return pedido;
        }

        /// <summary>
        /// Retorna todos os pedidos para confirmação.
        /// </summary>
        /// <returns></returns>
        public Pedido[] GetForConfirmation(uint idPedido, uint idCli, string nomeCli, uint idFunc, string codCliente,
            string dataIni, string dataFim, bool revenda, bool liberarPedido, uint idLoja, int origemPedido, int tipoPedido, string sortExpression)
        {
            bool temFiltro;
            string filtroAdicional;

            var tipoPedidoStr = (revenda && !liberarPedido) || (revenda && liberarPedido) ?
                ((int)Pedido.TipoPedidoEnum.Revenda).ToString() : null;

            if (tipoPedido > 0)
                tipoPedidoStr = tipoPedido.ToString();

            var situacaoPedido = (int)Pedido.SituacaoPedido.Conferido;

            var sql = Sql(idPedido, 0, null, null, idLoja, idCli, nomeCli, idFunc, codCliente, 0, null, null, null, null, null, situacaoPedido.ToString(),
                String.Empty, String.Empty, null, dataIni, dataFim, null, null, null, 0, true, false, 0, 0, 0, 0, 0,
                tipoPedidoStr, 0, 0, origemPedido, "", true, out filtroAdicional, out temFiltro).Replace("?filtroAdicional?", filtroAdicional);

            sortExpression = !sortExpression.IsNullOrEmpty() ? sortExpression : "IdPedido";

            return objPersistence.LoadDataWithSortExpression(sql, new InfoSortExpression(sortExpression), new InfoPaging(0, 1500), GetParam(nomeCli, codCliente, null, null, situacaoPedido.ToString(), null, dataIni, dataFim, null, null, null)).ToArray();
        }

        /// <summary>
        /// Verifica se o Pedido existe
        /// </summary>
        public bool PedidoExists(uint idPedido)
        {
            return PedidoExists(null, idPedido);
        }

        /// <summary>
        /// Verifica se o Pedido existe
        /// </summary>
        public bool PedidoExists(GDASession session, uint idPedido)
        {
            return objPersistence.ExecuteSqlQueryCount(session, "Select Count(*) From pedido Where IdPedido=" + idPedido) > 0;
        }

        #endregion

        #region Busca pedidos para geração de espelho

        /// <summary>
        /// Retorna os pedidos confirmados para geração do pedido espelho.
        /// </summary>
        /// <returns></returns>
        public Pedido[] GetForPedidoEspelhoGerar(uint idPedido, uint idCli, string nomeCli, string codCliente, string dataIni, string dataFim)
        {
            bool temFiltro;
            string filtroAdicional;

            var buscarRevenda = (PedidoConfig.DadosPedido.BloquearItensTipoPedido || !PCPConfig.PermitirGerarConferenciaPedidoRevenda) ? "And p.tipoPedido <> 2" : "AND 1";

            var sql = Sql(idPedido, 0, null, null, 0, idCli, nomeCli, 0, codCliente, 0, null, null, null, null, null, null, null, null, null, dataIni,
                dataFim, null, null, null, 0, true, false, 0, 0, 0, 0, 0, null, 0, 0, 0, "", true, out filtroAdicional, out temFiltro).Replace("?filtroAdicional?", filtroAdicional) +
                " and p.situacao=" + (PedidoConfig.LiberarPedido ? (int)Pedido.SituacaoPedido.ConfirmadoLiberacao : (int)Pedido.SituacaoPedido.Confirmado) +
                " and (select count(*) from pedido_espelho where idPedido=p.idPedido)=0 " + buscarRevenda + " ORDER BY p.IdPedido Asc";

            return objPersistence.LoadData(sql, GetParam(nomeCli, codCliente, null, null, null, null, dataIni, dataFim, null, null, null)).ToArray();
        }

        #endregion

        #region Busca pedidos para Liberação

        private string SqlLiberacao(uint idCliente, string nomeCliente, string idsPedidos, string dataIniEntrega, string dataFimEntrega,
            int situacaoProd, string tiposPedidos, int? idLoja, bool buscarDescontoFluxoParaLiberacao)
        {
            // O Left Join com funcionário deve ser left porque aconteceu de um pedido ter sido tirado pela web e 
            // ter ficado com idFunc=0

            var buscarOcs = OrdemCargaConfig.UsarControleOrdemCarga ? ", CAST((SELECT GROUP_CONCAT(idOrdemCarga) FROM pedido_ordem_carga WHERE idPedido = p.idPedido) as CHAR) as IdsOCs " : " ";

            var sql = $@"
                Select p.*, { ClienteDAO.Instance.GetNomeCliente("c") } as NomeCliente, c.Revenda as CliRevenda, f.Nome as NomeFunc, o.saldo as saldoObra, 
                    ({ ObraDAO.Instance.SqlPedidosAbertos("p.idObra", "p.idPedido", ObraDAO.TipoRetorno.TotalPedido) }) as totalPedidosAbertosObra, l.NomeFantasia as nomeLoja, 
                    fp.Descricao as FormaPagto, pe.Total as TotalEspelho, s.dataCad as dataEntrada,
                    s.usuCad as usuEntrada, cast(s.valorCreditoAoCriar as decimal(12,2)) as valorCreditoAoReceberSinal, 
                    cast(s.creditoGeradoCriar as decimal(12,2)) as creditoGeradoReceberSinal,
                    cast(s.creditoUtilizadoCriar as decimal(12,2)) as creditoUtilizadoReceberSinal, s.isPagtoAntecipado as pagamentoAntecipado,
                    { PCPConfig.UsarConferenciaFluxo && buscarDescontoFluxoParaLiberacao} as BuscarDescontoFluxoParaLiberacao,
                    c.ObsLiberacao as ObsLiberacaoCliente{ buscarOcs }
                From pedido p 
                    Left Join pedido_espelho pe On (p.idPedido=pe.idPedido) 
                    Inner Join cliente c On (p.idCli=c.id_Cli) 
                    Left Join funcionario f On (p.IdFunc=f.IdFunc) 
                    Inner Join loja l On (p.IdLoja = l.IdLoja) 
                    Left Join obra o On (p.idObra=o.idObra)
                    Left Join formapagto fp On (fp.IdFormaPagto=p.IdFormaPagto) 
                    Left Join sinal s On (p.idSinal=s.idSinal)
                Where p.situacao in ({ (int)Pedido.SituacaoPedido.ConfirmadoLiberacao }, { (int)Pedido.SituacaoPedido.LiberadoParcialmente }) 
                and p.tipoPedido<>{(int)Pedido.TipoPedidoEnum.Producao}";

            if (idCliente > 0)
                sql += " and p.idCli=" + idCliente;
            else if (!string.IsNullOrEmpty(nomeCliente))
            {
                var ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += " And c.id_Cli in (" + ids + ")";
            }

            if (!string.IsNullOrEmpty(idsPedidos))
                sql += " and p.idPedido in (" + idsPedidos + ")";

            if (!string.IsNullOrEmpty(dataIniEntrega))
                sql += " and p.dataEntrega>=?dataIniEntrega";

            if (!string.IsNullOrEmpty(dataFimEntrega))
                sql += " and p.dataEntrega<=?dataFimEntrega";

            if (situacaoProd > 0)
                sql += " And (p.situacaoProducao=" + situacaoProd +
                    (PedidoConfig.DadosPedido.BloquearItensTipoPedido ?
                        " Or (p.situacao=" + (uint)Pedido.SituacaoPedido.ConfirmadoLiberacao + " And p.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Revenda + "))" :
                        ")");

            if (tiposPedidos != null)
            {
                if (tiposPedidos == string.Empty) tiposPedidos = "0";
                sql += " and p.tipoPedido in (" + tiposPedidos + ")";
            }

            if (idLoja > 0)
                sql += " AND p.IdLoja=" + idLoja;

            if (string.IsNullOrEmpty(idsPedidos))
                sql += " Order By p.idPedido desc";
            else
            {
                sql += " Order By Case p.idPedido ";

                var cont = 100;
                foreach (var idPed in idsPedidos.Split(','))
                    sql += " When " + idPed + " then " + (cont--).ToString();

                sql += " else 0 end";
            }

            return sql;
        }

        /// <summary>
        /// Retorna uma string com os IDs dos pedidos de um cliente, retirando os pedidos em idsPedidosRem
        /// </summary>
        public string GetIdsPedidosForLiberacao(uint idCliente, string nomeCliente, string idsPedidosRem, string dataIniEntrega,
            string dataFimEntrega, int situacaoProd, string tiposPedidos, int? idLoja)
        {
            var itens = objPersistence.LoadResult(SqlLiberacao(idCliente, nomeCliente, null, dataIniEntrega, dataFimEntrega, situacaoProd, tiposPedidos, idLoja, false),
                new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"), new GDAParameter("?dataIniEntrega", DateTime.Parse(dataIniEntrega + " 00:00")),
                new GDAParameter("?dataFimEntrega", DateTime.Parse(dataFimEntrega + " 23:59"))).Select(f => f.GetUInt32(0))
                       .ToList(); ;

            var lstPedidosRem = new List<string>(idsPedidosRem.Split(','));

            var retorno = "";
            foreach (var i in itens)
                if (!lstPedidosRem.Contains(i.ToString()))
                    retorno += "," + i;

            return retorno.Length > 0 ? retorno.Substring(1) : "0";
        }

        /// <summary>
        /// Busca pedidos do cliente para liberação
        /// </summary>
        public Pedido[] GetForLiberacao(string idsPedidos)
        {
            if (string.IsNullOrEmpty(idsPedidos))
                return new Pedido[0];

            return objPersistence.LoadData(SqlLiberacao(0, null, idsPedidos, null, null, 0, null, null, true)).ToArray();
        }

        public Pedido[] GetForLiberacao(uint idCliente, uint idPedido)
        {
            if (idCliente == 0 && idPedido == 0)
                return new Pedido[0];

            return objPersistence.LoadData(SqlLiberacao(idCliente, null, idPedido.ToString(), null, null, 0, null, null, true)).ToArray();
        }

        #endregion

        #region Busca Total liberado de um pedido

        public float GetTotalLiberado(uint idPedido, string idsLiberarPedido)
        {
            if (idPedido == 0 && string.IsNullOrEmpty(idsLiberarPedido))
                return 0;

            var sql = @"
                Select " + SqlCampoTotalLiberacao(true, "valor", "p", "pe", "ap", "plp") + @"
                From pedido p 
                    Left Join pedido_espelho pe On (p.idPedido=pe.idPedido)
                    Inner Join produtos_pedido pp On (pp.idPedido=p.idPedido)
                    Left Join ambiente_pedido ap On (pp.idAmbientePedido=ap.idAmbientePedido)
                    Left Join produtos_liberar_pedido plp on (pp.idProdPed=plp.idProdPed)
                    Left Join liberarpedido lp on (plp.idLiberarPedido=lp.idLiberarPedido)
                where 1";

            if (!string.IsNullOrEmpty(idsLiberarPedido))
                sql += " And lp.idliberarpedido In (" + idsLiberarPedido + ")";

            if (idPedido > 0)
                sql += " And p.idPedido=" + idPedido;

            return ExecuteScalar<float>(sql);
        }

        #endregion

        #region Busca pedidos Liberados em uma determinada Liberação

        private string SqlByLiberacao(uint idPedido, string idsLiberarPedidos)
        {
            // O Left Join com funcionário deve ser left porque aconteceu de um pedido ter sido tirado pela web e 
            // ter ficado com idFunc=0
            var sql = @"
                Select p.*, " + ClienteDAO.Instance.GetNomeCliente("c") + @" as NomeCliente, c.Revenda as CliRevenda, f.Nome as NomeFunc, l.NomeFantasia as nomeLoja, 
                    fp.Descricao as FormaPagto, pe.total as totalEspelho, o.saldo as saldoObra, (" +
                    ObraDAO.Instance.SqlPedidosAbertos("p.idObra", "p.idPedido", ObraDAO.TipoRetorno.TotalPedido) + @") as totalPedidosAbertosObra, s.dataCad as dataEntrada,
                    s.usuCad as usuEntrada, cast(s.valorCreditoAoCriar as decimal(12,2)) as valorCreditoAoReceberSinal, cast(s.creditoGeradoCriar as decimal(12,2)) as creditoGeradoReceberSinal,
                    cast(s.creditoUtilizadoCriar as decimal(12,2)) as creditoUtilizadoReceberSinal,  (p.idPagamentoAntecipado > 0) as pagamentoAntecipado,
                    (SELECT r.codInterno FROM rota r WHERE r.idRota IN (Select rc.idRota From rota_cliente rc Where rc.idCliente=p.idCli)) As codRota
                From pedido p 
                    Inner Join cliente c On (p.idCli=c.id_Cli) 
                    Left Join funcionario f On (p.IdFunc=f.IdFunc) 
                    Inner Join loja l On (p.IdLoja = l.IdLoja) 
                    Left Join obra o On (p.idObra=o.idObra)
                    Left Join formapagto fp On (fp.IdFormaPagto=p.IdFormaPagto) 
                    Left Join pedido_espelho pe On (p.idPedido=pe.idPedido)
                    Left Join sinal s On (p.idSinal=s.idSinal)";

            if (idPedido > 0)
                sql += " Where p.idPedido=" + idPedido;
            else
                sql += " Where p.idPedido in (select idPedido from produtos_liberar_pedido where idLiberarPedido in (" + idsLiberarPedidos + "))";

            return sql + " order by p.idPedido asc";
        }

        /// <summary>
        /// Busca pedidos Liberados em uma determinada Liberação
        /// </summary>
        public Pedido[] GetByLiberacao(uint idLiberarPedido)
        {
            return GetByLiberacao(null, idLiberarPedido);
        }

        /// <summary>
        /// Busca pedidos Liberados em uma determinada Liberação
        /// </summary>
        public Pedido[] GetByLiberacao(GDASession session, uint idLiberarPedido)
        {
            var retorno = objPersistence.LoadData(session, SqlByLiberacao(0, idLiberarPedido.ToString())).ToArray();

            // Atualiza o campo desconto total
            foreach (var p in retorno)
            {
                if (PedidoEspelhoDAO.Instance.ExisteEspelho(session, p.IdPedido))
                    p.DescontoTotalPcp = true;
            }

            return retorno;
        }

        /// <summary>
        /// Busca pedidos Liberados em uma determinada Liberação
        /// </summary>
        public Pedido[] GetByLiberacoes(string idsLiberarPedidos)
        {
            return objPersistence.LoadData(SqlByLiberacao(0, idsLiberarPedidos)).ToArray();
        }

        public IList<uint> GetIdsByLiberacao(uint idLiberarPedido)
        {
            return GetIdsByLiberacao(null, idLiberarPedido);
        }

        public IList<uint> GetIdsByLiberacao(GDASession session, uint idLiberarPedido)
        {
            return objPersistence.LoadResult(session, SqlByLiberacao(0, idLiberarPedido.ToString()), null).Select(f => f.GetUInt32(0))
                       .ToList();
        }

        public IList<uint> GetIdsByLiberacoes(string idsLiberarPedidos)
        {
            return objPersistence.LoadResult(SqlByLiberacao(0, idsLiberarPedidos), null).Select(f => f.GetUInt32(0))
                       .ToList();
        }

        /// <summary>
        /// Retorna pedido com informações que serão usadas na liberação
        /// </summary>
        public Pedido GetElementForLiberacao(uint idPedido)
        {
            var pedido = objPersistence.LoadOneData(SqlByLiberacao(idPedido, null));

            return pedido;
        }

        #endregion

        #region Busca pedidos de um cliente

        /// <summary>
        /// Retorna os IDs dos pedidos de um cliente.
        /// </summary>
        public string GetIdsByCliente(uint idCli, string nomeCliente)
        {
            bool temFiltro;
            string filtroAdicional;

            return GetValoresCampo(Sql(0, 0, null, null, 0, idCli, nomeCliente, 0, null, 0, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, 0, false, false, 0, 0, 0, 0, 0, null, 0, 0, 0, "",
                true, out filtroAdicional, out temFiltro).Replace("?filtroAdicional?", filtroAdicional), "idPedido");
        }

        /// <summary>
        /// Retorna os IDs dos pedidos de uma impressão
        /// </summary>
        /// <returns></returns>
        public string GetIdsByImpressao(uint idImpressao)
        {
            var sql =
                @"SELECT DISTINCT pi.IdPedido
                FROM produto_impressao pi
                WHERE pi.idImpressao = ?idImpressao";

            var idsString =
                objPersistence.LoadResult(sql, new GDAParameter("?idImpressao", idImpressao))
                    .Select(f => f.GetString(0))
                    .ToList();

            var idsInt = new List<int>();

            if (!idsString.Any())
                return string.Empty;

            foreach (var id in idsString)
                if (id.StrParaIntNullable().GetValueOrDefault() > 0)
                    idsInt.Add(id.StrParaInt());

            if (!idsInt.Any())
                return string.Empty;

            return string.Join(",", Array.ConvertAll(idsInt.ToArray(), (x => x.ToString())));

        }

        #endregion

        #region Busca idPedido pelo orçamento

        /// <summary>
        /// Busca idPedido pelo orçamento
        /// </summary>
        /// <returns></returns>
        public uint GetIdPedidoByOrcamento(uint idOrcamento)
        {
            var sql = "Select idPedido From pedido Where situacao<>" + (int)Pedido.SituacaoPedido.Cancelado +
                " And idOrcamento=" + idOrcamento + " limit 1";

            object obj = objPersistence.ExecuteScalar(sql);

            var idPedido = obj != null && obj.ToString() != String.Empty ? Glass.Conversoes.StrParaUint(obj.ToString()) : 0;

            return idPedido;
        }

        #endregion

        #region Busca pedidos para Nota Fiscal

        private string SqlNfe(string idsPedidos, string idsLiberarPedidos, uint idCliente, string nomeCliente)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = Sql(0, 0, idsPedidos, idsLiberarPedidos, 0, idCliente, nomeCliente, 0, null, 0, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, 0, false, false, 0, 0, 0, 0, 0, null,
                0, 0, 0, "", true, out filtroAdicional, out temFiltro).Replace("?filtroAdicional?", filtroAdicional);

            var situacoes = (int)Pedido.SituacaoPedido.Confirmado + "," + (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + "," +
                (int)Pedido.SituacaoPedido.LiberadoParcialmente;

            if (FiscalConfig.PermitirGerarNotaPedidoConferido)
                situacoes += "," + (int)Pedido.SituacaoPedido.Conferido;

            sql += " and p.Situacao in (" + situacoes + ")";

            return sql;
        }

        /// <summary>
        /// Retorna os pedidos para geração da nota fiscal.
        /// </summary>
        public Pedido[] GetForNFe(string idsPedidos, string idsLiberarPedidos, uint idCliente, string nomeCliente)
        {
            if (string.IsNullOrEmpty(idsPedidos) && string.IsNullOrEmpty(idsLiberarPedidos) && idCliente == 0 && string.IsNullOrEmpty(nomeCliente))
                return new Pedido[0];

            return objPersistence.LoadData(SqlNfe(idsPedidos, idsLiberarPedidos, idCliente, nomeCliente),
                GetParam(nomeCliente, null, null, null, null, null, null, null, null, null, null, null)).ToArray();
        }

        /// <summary>
        /// Retorna os pedidos para geração da nota fiscal.
        /// </summary>
        public Pedido[] GetForNFe(uint idPedido, uint idLiberarPedido, uint idCliente, string nomeCliente)
        {
            if (idPedido == 0 && idLiberarPedido == 0 && idCliente == 0 && string.IsNullOrEmpty(nomeCliente))
                return new Pedido[0];

            return GetForNFe(idPedido.ToString(), idLiberarPedido.ToString(), idCliente, nomeCliente);
        }

        /// <summary>
        /// Retorna os IDs dos pedidos para NFe.
        /// </summary>
        public string GetIdsForNFe(uint idCliente, string nomeCliente)
        {
            var sql = "select distinct idPedido from (" + SqlNfe(null, null, idCliente, nomeCliente) + ") as temp";
            var ids = objPersistence.LoadResult(sql, GetParam(nomeCliente, null, null, null, null, null, null, null, null, null, null, null)).
                Select(f => f.GetUInt32(0)).ToList();

            return string.Join(",", Array.ConvertAll(ids.ToArray(), (
                x => x.ToString()
                )));
        }

        #endregion

        #region Volumes do pedido

        #region Busca pedidos para geração de volumes

        /// <summary>
        /// Retorna a sql para geração de volumes
        /// </summary>
        private string SqlForGeracaoVolume(uint idPedido, uint idCli, string nomeCli, uint idLoja, string codRota, string dataEntIni,
            string dataEntFim, string dataLibIni, string dataLibFim, string situacao, int tipoEntrega, uint idCliExterno,
            string nomeCliExterno, string codRotaExterna, bool selecionar)
        {
            var campos = @"p.*, c.nomeFantasia as NomeCliente, f.Nome as NomeFunc, l.NomeFantasia as nomeLoja,
                (SELECT r.codInterno FROM rota r WHERE r.idRota IN (Select rc.idRota From rota_cliente rc Where rc.idCliente=p.idCli)) As codRota, 
                CAST(SUM(pp.qtde) as SIGNED) as QuantidadePecasPedido, COALESCE(vpp.qtde, 0) as QtdePecasVolume, SUM(pp.TotM) as TotMVolume,
                SUM(pp.peso) as PesoVolume";

            var sql = @"
                SELECT " + campos + @"
                FROM pedido p
                    INNER JOIN produtos_pedido pp ON (p.idPedido = pp.idPedido)
                    INNER JOIN produto prod ON (pp.idProd = prod.idProd)
                    INNER JOIN cliente c On (p.idCli=c.id_Cli)
                    LEFT JOIN funcionario f On (p.idFunc=f.idFunc)
                    LEFT JOIN loja l On (p.IdLoja = l.IdLoja)
                    LEFT JOIN grupo_prod gp ON (prod.idGrupoProd = gp.idGrupoProd)
                    LEFT JOIN subgrupo_prod sgp ON (prod.idSubGrupoProd = sgp.idSubGrupoProd AND (sgp.PermitirItemRevendaNaVenda IS NULL OR sgp.PermitirItemRevendaNaVenda = 0))
                    LEFT JOIN (
                                    SELECT v1.idPedido, SUM(vpp1.qtde) as qtde
                                    FROM volume v1
	                                    INNER JOIN volume_produtos_pedido vpp1 ON (vpp1.idVolume = v1.idVolume)
                                    GROUP BY v1.idPedido
                             ) vpp ON (p.idPedido = vpp.idPedido)
                WHERE p.situacao IN(" + (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + @" {0}) 
                    AND COALESCE(sgp.GeraVolume, gp.GeraVolume, false) = true
                    AND COALESCE(sgp.TipoSubgrupo, 0) <> " + (int)TipoSubgrupoProd.ChapasVidro;

            sql = string.Format(sql, "," + (int)Pedido.SituacaoPedido.Confirmado + "," + (int)Pedido.SituacaoPedido.LiberadoParcialmente);

            if (OrdemCargaConfig.GerarVolumeApenasDePedidosEntrega)
                sql += " And p.tipoEntrega<>" + (int)Pedido.TipoEntregaPedido.Balcao;

            if (idPedido > 0)
                sql += " AND p.idPedido=" + idPedido;

            if (idCli > 0)
            {
                sql += " AND p.idcli=" + idCli;
            }
            else if (!string.IsNullOrEmpty(nomeCli))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCli, null, 0, null, null, null, null, 0);
                sql += " AND p.idCli IN(" + ids + ")";
            }

            if (idCliExterno > 0)
            {
                sql += " AND p.IdClienteExterno=" + idCliExterno;
            }
            else if (!string.IsNullOrEmpty(nomeCliExterno))
            {
                var ids = ClienteDAO.Instance.ObtemIdsClientesExternos(nomeCliExterno);

                if (!string.IsNullOrEmpty(ids))
                    sql += " AND p.IdClienteExterno IN(" + ids + ")";
            }

            if (idLoja > 0)
                sql += " AND p.idLoja=" + idLoja;

            if (!string.IsNullOrEmpty(dataEntIni))
                sql += " AND p.DataEntrega>=?dtEntIni";

            if (!string.IsNullOrEmpty(dataEntFim))
                sql += " AND p.DataEntrega<=?dtEntFim";

            if (!string.IsNullOrEmpty(dataLibIni))
                sql += " AND p.IdPedido IN (SELECT IdPedido FROM produtos_liberar_pedido WHERE IdLiberarPedido IN (SELECT IdLiberarPedido FROM liberarpedido WHERE DataLiberacao>=?dataLibIni))";

            if (!string.IsNullOrEmpty(dataLibFim))
                sql += " AND p.IdPedido IN (SELECT IdPedido FROM produtos_liberar_pedido WHERE IdLiberarPedido IN (SELECT IdLiberarPedido FROM liberarpedido WHERE DataLiberacao<=?dataLibFim))";

            if (!string.IsNullOrEmpty(codRota))
                sql += " And c.id_Cli In (Select idCliente From rota_cliente Where idRota In " +
                    "(Select idRota From rota where codInterno like ?codRota))";

            if (!string.IsNullOrEmpty(codRotaExterna))
            {
                var rotas = string.Join(",", codRotaExterna.Split(',').Select(f => "'" + f + "'").ToArray());
                sql += " AND p.RotaExterna IN (" + rotas + ")";
            }

            if (tipoEntrega > 0)
                sql += " AND p.tipoEntrega=" + tipoEntrega;

            if (!PCPConfig.UsarConferenciaFluxo)
                sql += " AND COALESCE(pp.InvisivelPedido, false) = false";
            else
                sql += " AND COALESCE(pp.InvisivelFluxo, false) = false";

            sql += " GROUP BY p.idpedido";

            situacao = "," + situacao + ",";
            var filtroSituacao = new List<string>();
            if (situacao != ",1,2,3,")
            {
                if (situacao.Contains(",1,"))
                    filtroSituacao.Add("QtdePecasVolume = 0");

                if (situacao.Contains(",2,"))
                    filtroSituacao.Add("(QtdePecasVolume > 0 AND QuantidadePecasPedido > QtdePecasVolume)");

                if (situacao.Contains(",3,"))
                    filtroSituacao.Add("QuantidadePecasPedido = QtdePecasVolume");
            }

            return @"
                SELECT " + (selecionar ? "*" : "COUNT(*)") + @"
                FROM (" + sql + ") as tmp " +
                (filtroSituacao.Count > 0 ? "WHERE " + string.Join(" OR ", filtroSituacao.ToArray()) : "");
        }

        /// <summary>
        /// Recupera os pedidos para gerar volume
        /// </summary>
        public Pedido[] GetForGeracaoVolume(uint idPedido, uint idCli, string nomeCli, uint idLoja, string codRota, string dataEntIni,
            string dataEntFim, string dataLibIni, string dataLibFim, string situacao, int tipoEntrega, uint idCliExterno,
            string nomeCliExterno, string idsRotasExternas, string sortExpression, int startRow, int pageSize)
        {
            var sql = SqlForGeracaoVolume(idPedido, idCli, nomeCli, idLoja, codRota, dataEntIni, dataEntFim, dataLibIni, dataLibFim,
                situacao, tipoEntrega, idCliExterno, nomeCliExterno, idsRotasExternas, true);

            if (string.IsNullOrEmpty(sortExpression))
                sql += " ORDER BY idPedido DESC";

            var pedidos = LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize,
                GetParametersVolume(dataEntIni, dataEntFim, dataLibIni, dataLibFim, codRota, idsRotasExternas)).ToArray();

            return pedidos;
        }

        /// <summary>
        /// Retorna a quantidade de itens da consulta
        /// </summary>
        public int GetForGeracaoVolumeCount(uint idPedido, uint idCli, string nomeCli, uint idLoja, string codRota, string dataEntIni,
            string dataEntFim, string dataLibIni, string dataLibFim, string situacao, int tipoEntrega, uint idCliExterno,
            string nomeCliExterno, string idsRotasExternas)
        {
            var sql = SqlForGeracaoVolume(idPedido, idCli, nomeCli, idLoja, codRota, dataEntIni, dataEntFim, dataLibIni, dataLibFim,
                situacao, tipoEntrega, idCliExterno, nomeCliExterno, idsRotasExternas, false);
            return objPersistence.ExecuteSqlQueryCount(sql, GetParametersVolume(dataEntIni, dataEntFim, dataLibIni, dataLibFim, codRota,
                idsRotasExternas));
        }

        public GDAParameter[] GetParametersVolume(string dtEntIni, string dtEntFim, string dataLibIni, string dataLibFim, string codRota,
            string codRotaExterna)
        {
            var parameters = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(dataLibIni))
                parameters.Add(new GDAParameter("?dataLibIni", DateTime.Parse(dataLibIni + " 00:00:00")));

            if (!string.IsNullOrEmpty(dataLibFim))
                parameters.Add(new GDAParameter("?dataLibFim", DateTime.Parse(dataLibFim + " 23:59:59")));

            if (!string.IsNullOrEmpty(dtEntIni))
                parameters.Add(new GDAParameter("?dtEntIni", DateTime.Parse(dtEntIni + " 00:00:00")));

            if (!string.IsNullOrEmpty(dtEntFim))
                parameters.Add(new GDAParameter("?dtEntFim", DateTime.Parse(dtEntFim + " 23:59:59")));

            if (!string.IsNullOrEmpty(codRota))
                parameters.Add(new GDAParameter("?codRota", codRota));

            if (!string.IsNullOrEmpty(codRotaExterna))
                parameters.Add(new GDAParameter("?codRotaExterna", codRotaExterna));

            return parameters.ToArray();
        }

        /// <summary>
        /// Recupera os pedidos para o relatório de Volume
        /// </summary>
        public Pedido[] GetForGeracaoVolumeRpt(uint idPedido, uint idCli, string nomeCli, uint idLoja, string codRota, string dataEntIni,
            string dataEntFim, string dataLibIni, string dataLibFim, string situacao, int tipoEntrega, uint idCliExterno,
            string nomeCliExterno, string codRotaExterna)
        {
            var sql = SqlForGeracaoVolume(idPedido, idCli, nomeCli, idLoja, codRota, dataEntIni, dataEntFim, dataLibIni, dataLibFim,
                situacao, tipoEntrega, idCliExterno, nomeCliExterno, codRotaExterna, true);
            sql += " ORDER BY idPedido DESC";

            var pedidos = objPersistence.LoadData(sql, GetParametersVolume(dataEntIni, dataEntFim, dataLibIni, dataLibFim, codRota,
                codRotaExterna)).ToList();

            return pedidos.ToArray();
        }

        #endregion

        /// <summary>
        /// Verifica se o pediu gerou todos os volumes.
        /// </summary>
        public bool GerouTodosVolumes(GDASession sessao, uint idPedido)
        {
            var sql = @"
                SELECT pp.idPedido, CAST(SUM(pp.qtde) as SIGNED) as QtdePecas, COALESCE(vpp.qtde, 0) as QtdePecasVolume
                FROM produtos_pedido pp
                    INNER JOIN produto prod ON (pp.idProd = prod.idProd)
                    LEFT JOIN grupo_prod gp ON (prod.idGrupoProd = gp.idGrupoProd)
                    LEFT JOIN subgrupo_prod sgp ON (prod.idSubGrupoProd = sgp.idSubGrupoProd AND (sgp.PermitirItemRevendaNaVenda IS NULL OR sgp.PermitirItemRevendaNaVenda = 0))
                    LEFT JOIN (
                                SELECT v1.idPedido, SUM(vpp1.qtde) as qtde
                                FROM volume v1
	                                INNER JOIN volume_produtos_pedido vpp1 ON (vpp1.idVolume = v1.idVolume)
                                WHERE v1.situacao<>" + (int)Volume.SituacaoVolume.Aberto + @"
                                GROUP BY v1.idPedido) vpp ON (pp.idPedido = vpp.idPedido)
                WHERE pp.idPedido=" + idPedido + @"
                    AND COALESCE(sgp.GeraVolume, gp.GeraVolume, false)=true
                    AND COALESCE(sgp.TipoSubgrupo, 0) <> " + (int)TipoSubgrupoProd.ChapasVidro;

            if (!PCPConfig.UsarConferenciaFluxo)
                sql += " AND COALESCE(pp.InvisivelPedido, false) = false";
            else
                sql += " AND COALESCE(pp.InvisivelFluxo, false) = false";

            sql += " GROUP BY pp.idPedido";

            var sqlDeveGerarVolume = "SELECT COUNT(idPedido) FROM (" + sql + ") as tmp";

            if (objPersistence.ExecuteSqlQueryCount(sessao, sqlDeveGerarVolume) == 0)
                return true;

            var sqlGerouTodosVolumes = "SELECT COUNT(*) FROM (" + sql + ") as tmp WHERE QtdePecas = QtdePecasVolume";

            return objPersistence.ExecuteSqlQueryCount(sessao, sqlGerouTodosVolumes) > 0;
        }

        /// <summary>
        /// Verifica se o pedido possuiu algum volume
        /// </summary>
        public bool TemVolume(uint idPedido)
        {
            return TemVolume(null, idPedido);
        }

        /// <summary>
        /// Verifica se o pedido possuiu algum volume
        /// </summary>
        public bool TemVolume(GDASession session, uint idPedido)
        {
            var sqlSemVolume = "SELECT COUNT(*) FROM volume WHERE idPedido=" + idPedido;

            return objPersistence.ExecuteSqlQueryCount(session, sqlSemVolume) > 0;
        }

        /// <summary>
        /// Verifica se há pedidos de produção ativos com referência do pedido passado
        /// </summary>
        public bool PedidoProducaoCorteAtivo(GDASession session, uint idPedido)
        {
            var sqlPedidoProducaoAtivo = "SELECT COUNT(*) FROM pedido WHERE SITUACAO <> 6 AND IDPEDIDOREVENDA = " + idPedido;

            return objPersistence.ExecuteSqlQueryCount(session, sqlPedidoProducaoAtivo) > 0;
        }

        /// <summary>
        /// Verifica se o pedido possuiu algum volume aberto
        /// </summary>
        public bool TemVolumeAberto(uint idPedido)
        {
            var sqlSemVolume = "SELECT COUNT(*) FROM volume WHERE idPedido=" + idPedido + " AND situacao=" + (int)Volume.SituacaoVolume.Aberto;

            return objPersistence.ExecuteSqlQueryCount(sqlSemVolume) > 0;
        }

        #endregion

        #region Ordem de Carga

        #region Sql

        /// <summary>
        /// Sql para recuperar ids dos pedidos que podem gerar OC.
        /// </summary>
        private string SqlIdsPedidosForOC(OrdemCarga.TipoOCEnum tipoOC, uint idCliente, string nomeCli, uint idRota, string idsRotas, uint idLoja,
            string dtEntPedidoIni, string dtEntPedidoFin, bool buscarTodos, string codRotasExternas, uint idCliExterno, string nomeCliExterno, bool fastDelivery, string obsLiberacao = "")
        {
            var filtro = "";

            var sql = @"
                SELECT cast(CONCAT(p.idCli, ';', p.idPedido, ';', rc.idRota, ';', COALESCE(p.obsLiberacao, '') <> '') as char)
                FROM pedido p
                    INNER JOIN produtos_pedido pp ON (p.idPedido = pp.idPedido)
                    INNER JOIN produto prod ON (pp.idprod = prod.idprod)
                    LEFT JOIN pedido_espelho pe ON (p.idPedido=pe.idPedido)
                    LEFT JOIN (" + SubgrupoProdDAO.Instance.SqlSubgrupoRevenda() + @"
                    ) as prodRevenda ON (prod.idGrupoProd = prodRevenda.idGrupoProd 
                        /* Chamado 16149.
                         * Não pode ser feito coalesce com prodRevenda.idSubgrupoProd, porque quando o produto não tem subgrupo
                         * associado, é buscado um subgrupo qualquer que atenda às condições informadas.
                        AND Coalesce(prod.idSubgrupoProd, prodRevenda.idSubgrupoProd) = prodRevenda.idSubgrupoProd)*/
                        AND Coalesce(prod.idSubgrupoProd, 0) = prodRevenda.idSubgrupoProd)
                    LEFT JOIN rota_cliente rc ON (p.idCli = rc.idCliente)
                    LEFT JOIN cliente c ON (p.idCli = c.id_cli)
                    LEFT JOIN grupo_prod gp ON (prod.idGrupoProd = gp.idGrupoProd)
                    LEFT JOIN subgrupo_prod sgp ON (prod.idSubGrupoProd = sgp.idSubGrupoProd)
                WHERE COALESCE(pp.invisivelFluxo, FALSE)=FALSE
                    AND if(prodRevenda.idSubgrupoProd is null,
                            pe.idPedido is not null AND pe.situacao not in (" +
                            (int)PedidoEspelho.SituacaoPedido.Aberto + "," +
                            (int)PedidoEspelho.SituacaoPedido.Cancelado + "," +
                            (int)PedidoEspelho.SituacaoPedido.Processando +
                            @"), true) {0}
                GROUP BY p.idPedido";

            var situacoesPedido = (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + ", " + (int)Pedido.SituacaoPedido.LiberadoParcialmente;
            var situacoesPedidoRevenda = (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + "," + (int)Pedido.SituacaoPedido.Confirmado + ", " + (int)Pedido.SituacaoPedido.LiberadoParcialmente;

            if (OrdemCargaConfig.DataEntregaBaseConsiderarPedidoParaOC != null)
                filtro += string.Format(" AND p.dataEntrega > '{0}'", OrdemCargaConfig.DataEntregaBaseConsiderarPedidoParaOC);

            filtro += " AND p.tipoEntrega <> " + (int)Pedido.TipoEntregaPedido.Balcao;
            filtro += string.Format(" AND IF(p.TipoPedido = {0}, coalesce(p.IdPedidoRevenda, 0) > 0, true)", (int)Pedido.TipoPedidoEnum.Producao);
            filtro += string.Format(" AND IF(p.TipoPedido = {0}, p.GerarPedidoProducaoCorte=false, true)", (int)Pedido.TipoPedidoEnum.Revenda);
            //49453 
            filtro += string.Format(" AND IF(p.TipoPedido = {0}, p.Situacao IN ({1}), p.situacao IN(" + situacoesPedido + "))", (int)Pedido.TipoPedidoEnum.Producao, situacoesPedidoRevenda);

            if (idRota > 0)
                filtro += " AND rc.idRota=" + idRota;
            else if (!string.IsNullOrEmpty(idsRotas))
                filtro += " AND rc.idRota IN (" + idsRotas + ")";

            if (!string.IsNullOrEmpty(dtEntPedidoIni))
                filtro += " AND p.DataEntrega>=?dtEntPedidoIni";

            if (!string.IsNullOrEmpty(dtEntPedidoFin))
                filtro += " AND p.DataEntrega<=?dtEntPedidoFin";

            if (idCliente > 0)
            {
                filtro += " AND p.idCli=" + idCliente;
            }
            else if (!string.IsNullOrEmpty(nomeCli))
            {
                var idsCli = ClienteDAO.Instance.GetIds(null, nomeCli, null, 0, null, null, null, null, 0);
                filtro += " AND p.idCli IN(" + idsCli + ")";
            }

            if (idCliExterno > 0)
            {
                filtro += " AND p.IdClienteExterno=" + idCliExterno;
            }
            else if (!string.IsNullOrEmpty(nomeCliExterno))
            {
                var ids = ClienteDAO.Instance.ObtemIdsClientesExternos(nomeCliExterno);

                if (!string.IsNullOrEmpty(ids))
                    filtro += " AND p.IdClienteExterno IN(" + ids + ")";
            }

            if (!string.IsNullOrEmpty(codRotasExternas))
            {
                var rotas = string.Join(",", codRotasExternas.Split(',').Select(f => "'" + f + "'").ToArray());
                filtro += " AND p.RotaExterna IN (" + rotas + ")";
            }

            if (fastDelivery)
                filtro += " AND p.FastDelivery";

            if (tipoOC == OrdemCarga.TipoOCEnum.Venda)
            {
                filtro += " AND p.idCli NOT IN (SELECT id_Cli FROM cliente WHERE SomenteOcTransferencia = 1)";
                filtro += " AND p.idLoja=" + idLoja;
                filtro += @" AND IF(p.deveTransferir, COALESCE((SELECT COUNT(*) 
                                                             FROM ordem_carga oc 
                                                                LEFT JOIN pedido_ordem_carga poc ON (oc.idOrdemCarga = poc.idOrdemCarga)
                                                             WHERE oc.tipoOrdemCarga=" + (int)OrdemCarga.TipoOCEnum.Transferencia + @"
                                                                AND poc.idPedido = p.idPedido), 0) > 0 
                                                    AND COALESCE((SELECT COUNT(*)
                                                                    FROM nota_fiscal nf
                                                                        INNER JOIN pedidos_nota_fiscal pnf ON (nf.idNf = pnf.idNf)
                                                                    WHERE nf.situacao = 2 
	                                                                AND nf.tipoDocumento = 2
	                                                                AND pnf.idPedido = p.idPedido), 0) > 0, p.situacaoProducao <> " + (int)Pedido.SituacaoProducaoEnum.Entregue + ")";

                filtro += @" AND COALESCE((SELECT COUNT(*) 
                                        FROM ordem_carga oc
                                            LEFT JOIN pedido_ordem_carga poc ON (oc.IdOrdemCarga = poc.idOrdemCarga)
                                        WHERE IF(p.OrdemCargaParcial, (oc.Situacao <> " + (int)OrdemCarga.SituacaoOCEnum.CarregadoParcialmente + "), 1) AND oc.tipoOrdemCarga=" + (int)OrdemCarga.TipoOCEnum.Venda +
                                            (buscarTodos ? " AND oc.situacao IN(" + (int)OrdemCarga.SituacaoOCEnum.Carregado + ","
                                                                     + (int)OrdemCarga.SituacaoOCEnum.PendenteCarregamento + ")" : "") + @"
                                            AND poc.idPedido = p.idPedido), 0)=0";

            }
            else if (tipoOC == OrdemCarga.TipoOCEnum.Transferencia)
            {
                filtro += " AND p.situacaoProducao <> " + (int)Pedido.SituacaoProducaoEnum.Entregue;
                filtro += " AND p.idLoja <>" + idLoja;
                filtro += " AND p.deveTransferir = TRUE";
                filtro += @" AND COALESCE((SELECT COUNT(*) 
                                        FROM ordem_carga oc
                                            LEFT JOIN pedido_ordem_carga poc ON (oc.IdOrdemCarga = poc.idOrdemCarga)
                                        WHERE oc.tipoOrdemCarga=" + (int)OrdemCarga.TipoOCEnum.Transferencia +
                                            (buscarTodos ? " AND oc.situacao IN(" + (int)OrdemCarga.SituacaoOCEnum.Carregado + ","
                                                                     + (int)OrdemCarga.SituacaoOCEnum.PendenteCarregamento + ")" : "") + @"
                                            AND poc.idPedido = p.idPedido), 0)=0";
            }

            if (!string.IsNullOrEmpty(obsLiberacao))
            {
                filtro += string.Format(" AND p.ObsLiberacao LIKE '{0}'", "%" + obsLiberacao + "%");
            }

            filtro += " AND IF(prodRevenda.idSubgrupoProd is not null AND COALESCE(sgp.geraVolume, gp.geraVolume, false)=false, prod.idgrupoProd = " +
                (int)Glass.Data.Model.NomeGrupoProd.Vidro + " AND sgp.produtosEstoque = true, true)";


            return string.Format(sql, filtro);
        }

        /// <summary>
        /// Sql para recuperar ids dos pedidos de uma OC
        /// </summary>
        private string SqlIdsPedidosForOC(uint idOC)
        {
            var campos = "DISTINCT(p.idPedido)";

            var sql = @"
                SELECT " + campos + @"
                FROM pedido p
                    INNER JOIN pedido_ordem_carga poc ON (p.IdPedido = poc.IdPedido)
                WHERE poc.IdOrdemCarga=" + idOC;

            return sql;
        }

        /// <summary>
        /// Sql para recuperar os pedidos para a OC
        /// </summary>
        private string SqlPedidosForOC(string idsPedidos, uint idOrdemCarga, bool ignorarGerados, bool selecionar)
        {
            var nomeCliente = ClienteDAO.Instance.GetNomeCliente("c");

            var sqlQtdeVolume = @"
                SELECT count(*) 
                FROM volume v
                    LEFT JOIN ordem_carga oc ON (v.IdOrdemCarga = oc.IdOrdemCarga)
                WHERE v.idPedido = p.idPedido";

            if (idOrdemCarga > 0)
                sqlQtdeVolume += " AND v.IdOrdemCarga = " + idOrdemCarga;
            if (ignorarGerados)
                sqlQtdeVolume += " AND (COALESCE(v.IdOrdemCarga, 0) = 0 OR oc.Situacao = " + (int)OrdemCarga.SituacaoOCEnum.Finalizado + ")";

            var campos = selecionar ? @"p.*, " + nomeCliente + @" as NomeCliente, l.NomeFantasia as NomeLoja, tmp.QtdePendente, tmp.TotMPendente, tmp.PesoPendente,
                CAST(COALESCE(tmp.peso, 0) as decimal(12,2)) as pesoOC, COALESCE(tmp.totM, 0) as totMOC, COALESCE(tmp.QtdePecasVidro, 0) as QtdePecasVidro,
                (SELECT CONCAT(r.codInterno,' - ',r.descricao) FROM rota r WHERE r.idRota IN (SELECT rc.idRota FROM rota_cliente rc WHERE rc.idCliente=c.id_Cli)) as codRota,
                c.Tel_Cont AS RptTelCont, c.Tel_Cel AS RptTelCel, c.Tel_Res AS RptTelRes, COALESCE(tmp.ValorTotal, 0) as ValorTotalOC,
                (" + sqlQtdeVolume + ") as QtdeVolume" : "COUNT(*)";

            var campoQtde = idOrdemCarga > 0 ? "IF(ic1.IdProdPed IS NULL, pp.qtde - COALESCE(ic.Qtde, 0), ic1.Qtde)" : ignorarGerados ? "pp.Qtde - COALESCE(ic.Qtde, 0)" : "pp.Qtde";

            var sql = @"
                SELECT " + campos + @"
                FROM pedido p
                    LEFT JOIN cliente c ON (p.idCli = c.id_cli)
                    LEFT JOIN loja l ON (p.idLoja = l.idLoja)
                    LEFT JOIN ( 
                        select idPedido, sum(qtdePecasVidro) as qtdePecasVidro, sum(qtdePendente) as qtdePendente,
                            SUM(peso / qtdePecasVidro * qtdePendente) as pesoPendente, 
                            sum(totM / qtdePecasVidro * qtdePendente) as totMPendente,
                            sum(peso) as peso, sum(totM) as totM, Sum(ValorTotal) as ValorTotal
                        from (

                            SELECT idProdPed, idPedido, qtde as qtdePecasVidro, qtdePendente, totM, peso, ValorTotal
                            FROM (
                                SELECT pp.idProdPed, pp.idPedido, {0} as Qtde, (pp.qtde - COALESCE(ppp.QtdePronto, 0)) as QtdePendente,
                                ((pp.TotM / pp.qtde) * ({0})) as TotM, ((pp.peso / pp.Qtde) * ({0})) as peso, (((pp.Total + pp.ValorIpi + pp.ValorIcms) / pp.qtde) * {0}) as ValorTotal
                                FROM produtos_pedido pp
                                INNER JOIN produto prod ON (pp.idProd = prod.idProd)
                                LEFT JOIN grupo_prod gp ON (prod.idGrupoProd = gp.idGrupoProd)
                                LEFT JOIN subgrupo_prod sgp ON (prod.idSubGrupoProd = sgp.idSubGrupoProd)
                                LEFT JOIN 
                                    (
			                                SELECT IdProdPed, count(*) as QtdePronto
			                                FROM produto_pedido_producao
			                                WHERE SituacaoProducao IN (" + (int)SituacaoProdutoProducao.Pronto + "," + (int)SituacaoProdutoProducao.Entregue + @")
			                                GROUP BY IdProdPed
                                    ) as ppp ON (pp.IdProdPedEsp = ppp.idProdPed)
	                                LEFT JOIN 
                                    (
		                                SELECT IdProdPed, count(*) as Qtde
		                                FROM item_carregamento
		                                WHERE COALESCE(IdProdPed, 0) > 0 AND idPedido IN (" + idsPedidos + @") AND IdOrdemCarga <> " + idOrdemCarga + @"
		                                GROUP BY IdProdPed
                                    ) as ic ON (ic.IdProdPed = pp.IdProdPed)
                                    LEFT JOIN 
                                    (
		                                SELECT IdProdPed, count(*) as Qtde
		                                FROM item_carregamento
		                                WHERE COALESCE(IdProdPed, 0) > 0 AND idPedido IN (" + idsPedidos + @") AND IdOrdemCarga = " + idOrdemCarga + @"
		                                GROUP BY IdProdPed
                                    ) as ic1 ON (ic1.IdProdPed = pp.IdProdPed)
                             WHERE pp.idPedido IN (" + idsPedidos + @")
                                AND COALESCE(sgp.GeraVolume, gp.GeraVolume, 0) = 0
                                    AND COALESCE(pp.InvisivelFluxo, 0) = 0
                                    AND COALESCE(sgp.produtosEstoque, 0) = 0
                                    AND pp.qtde > 0
                                    AND gp.idGrupoProd IN (" + (int)NomeGrupoProd.Vidro + "," + (int)NomeGrupoProd.MaoDeObra + @")
                                    AND pp.IdProdPedParent IS NULL
                                GROUP BY pp.idProdPed
                                HAVING Qtde > 0
                            ) as tmp1

                            UNION ALL SELECT pp.idProdPed, pp.idPedido, ({0}) as qtdePecasVidro, 0 as qtdePendente, ((pp.TotM / pp.qtde) * ({0})) as TotM, 
                                 ((pp.peso / pp.Qtde) * ({0})) as peso,
                                 (((pp.Total + pp.ValorIpi + pp.ValorIcms) / pp.qtde) * {0}) as ValorTotal
                            FROM produtos_pedido pp
                                INNER JOIN produto prod ON (pp.idProd = prod.idProd)
                                LEFT JOIN grupo_prod gp ON (prod.idGrupoProd = gp.idGrupoProd)
                                LEFT JOIN subgrupo_prod sgp ON (prod.idSubGrupoProd = sgp.idSubGrupoProd)
                                LEFT JOIN produto_pedido_producao ppp ON (pp.IDPRODPEDESP = ppp.idProdPed)
                                LEFT JOIN setor s ON (ppp.idSetor = s.idSetor)
                                LEFT JOIN 
                                    (
		                                SELECT IdProdPed, count(*) as Qtde
		                                FROM item_carregamento
		                                WHERE COALESCE(IdProdPed, 0) > 0 AND idPedido IN (" + idsPedidos + @") AND IdOrdemCarga <> " + idOrdemCarga + @"
		                                GROUP BY IdProdPed
                                    ) as ic ON (ic.IdProdPed = pp.IdProdPed)
                                    LEFT JOIN 
                                    (
		                                SELECT IdProdPed, count(*) as Qtde
		                                FROM item_carregamento
		                                WHERE COALESCE(IdProdPed, 0) > 0 AND idPedido IN (" + idsPedidos + @") AND IdOrdemCarga = " + idOrdemCarga + @"
		                                GROUP BY IdProdPed
                                    ) as ic1 ON (ic1.IdProdPed = pp.IdProdPed)
                             WHERE pp.idPedido IN (" + idsPedidos + @")
                                AND COALESCE(sgp.GeraVolume, gp.GeraVolume, 0) = 0
                                AND COALESCE(pp.InvisivelFluxo, 0) = 0
                                AND COALESCE(sgp.produtosEstoque, 0) = 1
                                AND pp.qtde > 0
                                AND gp.idGrupoProd IN (" + (int)NomeGrupoProd.Vidro + "," + (int)NomeGrupoProd.MaoDeObra + @")
                                AND pp.IdProdPedParent IS NULL
                            GROUP BY pp.idProdPed
                            HAVING qtdePecasVidro > 0
                            UNION ALL SELECT pp.idProdPed, pp.idPedido, 0 as qtdePecasVidro, 0 as qtdePendente, 0 as totM, (pp.peso / pp.qtde) * ({0}) as peso,
                                (((pp.Total + pp.ValorIpi + pp.ValorIcms) / pp.qtde) * {0}) as ValorTotal
                            FROM produtos_pedido pp
                                INNER JOIN produto prod ON (pp.idProd = prod.idProd)
                                LEFT JOIN grupo_prod gp ON (prod.idGrupoProd = gp.idGrupoProd)
                                LEFT JOIN subgrupo_prod sgp ON (prod.idSubGrupoProd = sgp.idSubGrupoProd)
                                LEFT JOIN 
                                (
		                            SELECT vpp.IdProdPed, SUM(vpp.Qtde) as Qtde
		                            FROM volume_produtos_pedido vpp
			                            INNER JOIN item_carregamento ic ON (vpp.IdVolume = ic.IdVolume)
		                            WHERE ic.idPedido IN (" + idsPedidos + @") AND IdOrdemCarga <> " + idOrdemCarga + @"
		                            GROUP BY vpp.IdProdPed
                                ) as ic ON (ic.IdProdPed = pp.IdProdPed)
                                LEFT JOIN 
                                (
		                            SELECT vpp.IdProdPed, SUM(vpp.Qtde) as Qtde
		                            FROM volume_produtos_pedido vpp
			                            INNER JOIN item_carregamento ic ON (vpp.IdVolume = ic.IdVolume)
		                            WHERE ic.idPedido IN (" + idsPedidos + @") AND IdOrdemCarga = " + idOrdemCarga + @"
		                            GROUP BY vpp.IdProdPed
                                ) as ic1 ON (ic1.IdProdPed = pp.IdProdPed)
                            WHERE pp.idPedido IN (" + idsPedidos + @")
                                AND COALESCE(sgp.GeraVolume, gp.GeraVolume, 0) = 1
                                AND COALESCE(pp.InvisivelFluxo, 0 ) = 0
                                AND pp.qtde > 0
                                AND pp.IdProdPedParent IS NULL
                            GROUP BY pp.idProdPed
                            HAVING peso > 0
                        ) as dados
                        group by idPedido
                    ) as tmp ON (p.idPedido = tmp.idPedido)
                WHERE p.idPedido in (" + idsPedidos + ")";

            sql = string.Format(sql, campoQtde);

            return sql;
        }

        #endregion

        #region Retorno de itens

        /// <summary>
        /// Recupera os ids dos pedidos para a OC
        /// </summary>
        public List<string> GetIdsPedidosForOC(OrdemCarga.TipoOCEnum tipoOC, uint idCliente, string nomeCli, uint idRota, string idsRotas, uint idLoja,
            string dtEntPedidoIni, string dtEntPedidoFin, bool buscarTodos, bool pedidosObs, string codRotasExternas, uint idCliExterno, string nomeCliExterno, bool fastDelivery, string obsLiberacao)
        {
            var retorno = ExecuteMultipleScalar<string>(SqlIdsPedidosForOC(tipoOC, idCliente, nomeCli, idRota, idsRotas, idLoja,
                dtEntPedidoIni, dtEntPedidoFin, buscarTodos, codRotasExternas, idCliExterno, nomeCliExterno, fastDelivery, obsLiberacao),
                GetParamsOC(dtEntPedidoIni, dtEntPedidoFin)).Where(f => f != null);

            var pedidos = new List<string>();

            foreach (var idCli in retorno.Select(c => c.Split(';')[0].StrParaUint()).Distinct())
            {
                var registro = retorno.Where(f => f.Split(';')[0].StrParaUint() == idCli);

                if (pedidosObs && registro.Count(f => f.Split(';')[3].StrParaInt() == 1) < 1)
                    continue;

                var pedido = "";

                pedido += registro.Select(f => f.Split(';')[0]).FirstOrDefault() + ";";
                pedido += registro.Select(f => f.Split(';')[2]).FirstOrDefault() + ";";
                pedido += string.Join(",", registro.Select(f => f.Split(';')[1]).ToArray());

                pedidos.Add(pedido);
            }

            return pedidos;
        }

        /// <summary>
        /// Recupera os ids dos pedidos para a OC
        /// </summary>
        public IList<string> GetIdsPedidosForOC(uint idOC)
        {
            return ExecuteMultipleScalar<string>(SqlIdsPedidosForOC(idOC));
        }

        /// <summary>
        /// Recupera os pedidos para a OC
        /// </summary>
        public List<Pedido> GetPedidosForOC(string idsPedidos, uint idOrdemCarga, bool ignorarGerados)
        {
            return objPersistence.LoadData(SqlPedidosForOC(idsPedidos, idOrdemCarga, ignorarGerados, true)).ToList();
        }

        /// <summary>
        /// Retorna o numero de pedidos que não geraram OC
        /// </summary>
        public int GetCountPedidosForOC(OrdemCarga.TipoOCEnum tipoOC, uint idCliente, uint idRota, uint idLoja,
            string dtEntPedidoIni, string dtEntPedidoFin, bool buscarTodos, bool pedidosObs,
            string codRotasExternas, uint idCliExterno, string nomeCliExterno, bool fastDelivery, string obsLiberacao)
        {
            var retorno = ExecuteMultipleScalar<string>(SqlIdsPedidosForOC(tipoOC, idCliente, null, idRota, null, idLoja,
                dtEntPedidoIni, dtEntPedidoFin, buscarTodos, codRotasExternas, idCliExterno, nomeCliExterno, fastDelivery, obsLiberacao),
                GetParamsOC(dtEntPedidoIni, dtEntPedidoFin));

            if (!pedidosObs)
                return retorno.Count;

            var count = 0;

            foreach (var idCli in retorno.Select(c => c.Split(';')[0].StrParaUint()).Distinct())
            {
                var registro = retorno.Where(f => f.Split(';')[0].StrParaUint() == idCli);

                if (pedidosObs && registro.Count(f => f.Split(';')[3].StrParaInt() == 1) < 1)
                    continue;

                count += registro.Count();
            }

            return count;
        }

        /// <summary>
        /// Parametros para a consulta da OC
        /// </summary>
        private GDAParameter[] GetParamsOC(string dtEntPedidoIni, string dtEntPedidoFin)
        {
            var lstParam = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(dtEntPedidoIni))
                lstParam.Add(new GDAParameter("?dtEntPedidoIni", DateTime.Parse(dtEntPedidoIni + " 00:00:00")));

            if (!string.IsNullOrEmpty(dtEntPedidoFin))
                lstParam.Add(new GDAParameter("?dtEntPedidoFin", DateTime.Parse(dtEntPedidoFin + " 23:59:59")));

            return lstParam.Count == 0 ? null : lstParam.ToArray();
        }

        /// <summary>
        /// Recupera os ids dos pedidos das ocs informadas
        /// </summary>
        public List<uint> GetIdsPedidosByOCs(GDASession sessao, string idsOCs)
        {
            var sql = @"
                SELECT DISTINCT(p.idPedido)
                FROM pedido p
                    INNER JOIN pedido_ordem_carga poc ON (p.idPedido = poc.idPedido)
                    INNER JOIN ordem_carga oc ON (poc.idOrdemCarga = oc.IdOrdemCarga)
                WHERE oc.IdOrdemCarga IN (" + idsOCs + ")";

            return ExecuteMultipleScalar<uint>(sessao, sql);
        }

        /// <summary>
        /// Recupera os ids dos pedidos das ocs informadas
        /// </summary>
        public List<uint> GetIdsPedidosByOCs(string idsOCs)
        {
            return GetIdsPedidosByOCs(null, idsOCs);
        }

        /// <summary>
        /// Obtem um lista de pedidos e o id do cliente importado
        /// </summary>
        public IList<KeyValuePair<uint?, string>> ObtemPedidosImportadosAgrupado(GDASession session, string idsPedidos)
        {
            var sql = @"
                        SELECT CONCAT(COALESCE(IdClienteExterno,0), ';', GROUP_CONCAT(idpedido))
                        FROM pedido
                        WHERE IdPedido IN (" + idsPedidos + @")
                        GROUP BY IdClienteExterno, ClienteExterno";

            var dados = ExecuteMultipleScalar<string>(session, sql);

            return dados.Where(f => !string.IsNullOrEmpty(f)).Select(f => new KeyValuePair<uint?, string>(f.Split(';')[0].StrParaUintNullable(), f.Split(';')[1])).ToList();
        }

        #endregion

        #region Recupera os pedidos da listagem de Ordens de Carga

        /// <summary>
        /// Sql para recuperar os pedidos para a OC
        /// </summary>
        public IEnumerable<PedidoTotaisOrdemCarga> ObterPedidosTotaisOrdensCarga(GDASession session, IEnumerable<int> idsOrdemCarga)
        {
            // Recupera os produtos de pedido da ordem de carga informada. 
            var produtosPedido = ProdutosPedidoDAO.Instance.ObterProdutosPedidoPelasOrdensDeCarga(session, idsOrdemCarga);
            produtosPedido = produtosPedido != null ? produtosPedido.ToList() : null;

            // Caso não haja retorno, sai do método.
            if (produtosPedido == null || produtosPedido.Count() == 0)
            {
                yield return new PedidoTotaisOrdemCarga();
                produtosPedido = new List<ProdutosPedido>();
            }

            // Recupera os itens de carregamento pelos ID's de produto de pedido recuperados pelo SQL.
            var itensCarregamento = ItemCarregamentoDAO.Instance.ObterItensCarregamentoPeloIdProdPed(session, produtosPedido.Select(f => (int)f.IdProdPed)).ToList();

            foreach (var produtosPedidoOrdemCarga in produtosPedido.GroupBy(f => f.IdOrdemCarga))
            {
                foreach (var pedidoTotalOrdemCarga in produtosPedidoOrdemCarga.Where(f => idsOrdemCarga.Contains(produtosPedidoOrdemCarga.Key)).Select(f =>
                {
                    // Obtém os itens do carregamento, da ordem de carga atual, da peça informada.
                    var itensCarregamentoMesmaOrdemCarga = itensCarregamento.Where(g => f.IdProdPed == g.IdProdPed && g.IdOrdemCarga == produtosPedidoOrdemCarga.Key).ToList();
                    // Obtém os itens do carregamento, das demais ordens de carga, da peça informada.
                    var itensCarregamentoOutrasOrdensCarga = itensCarregamento.Where(g => f.IdProdPed == g.IdProdPed && g.IdOrdemCarga != produtosPedidoOrdemCarga.Key).ToList();
                    // Calcula a quantidade do produto na ordem de carga.
                    var QtdeProdPed = itensCarregamentoMesmaOrdemCarga.Count > 0 ? itensCarregamentoMesmaOrdemCarga.Count :
                        itensCarregamentoOutrasOrdensCarga.Count > 0 ? f.Qtde - itensCarregamentoOutrasOrdensCarga.Count : f.Qtde;

                    // Recupera a quantidade de peças prontas do produto.
                    var quantidadePecasProntas = ProdutoPedidoProducaoDAO.Instance.ObterQuantidadePecasProntas(session, itensCarregamentoMesmaOrdemCarga
                        .Select(g => (int)g.IdProdPedProducao.GetValueOrDefault()).ToList());

                    // Recupera os totais do produto de pedido.
                    return new
                    {
                        IdPedido = f.IdPedido,
                        IdOrdemCarga = produtosPedidoOrdemCarga.Key,
                        QtdeTotal = QtdeProdPed,
                        // Caso a quantidade de volume seja maior que zero, o produto de pedido não é um vidro.
                        QtdePecasVidroTotal = f.QtdeVolume > 0 ? 0 : QtdeProdPed,
                        QtdePendenteTotal = QtdeProdPed - quantidadePecasProntas,
                        TotMTotal = (f.TotM / f.Qtde) * QtdeProdPed,
                        TotM2PendenteTotal = (f.TotM / f.Qtde) * (QtdeProdPed - quantidadePecasProntas),
                        PesoTotal = (f.Peso / f.Qtde) * QtdeProdPed,
                        PesoPendenteTotal = (f.Peso / f.Qtde) * (QtdeProdPed - quantidadePecasProntas),
                        ValorTotal = ((f.Total + f.ValorIpi + f.ValorIcms) / (decimal)f.Qtde) * (decimal)QtdeProdPed
                    };
                }).GroupBy(f => f.IdPedido))
                {
                    yield return new PedidoTotaisOrdemCarga(
                        GetElementByPrimaryKey(session, pedidoTotalOrdemCarga.Key),
                        produtosPedidoOrdemCarga.Key,
                        Math.Round(pedidoTotalOrdemCarga.Sum(f => f.QtdePecasVidroTotal), 2, MidpointRounding.AwayFromZero),
                        Math.Round(pedidoTotalOrdemCarga.Sum(f => f.QtdePendenteTotal), 2, MidpointRounding.AwayFromZero),
                        Math.Round(pedidoTotalOrdemCarga.Sum(f => f.TotMTotal), 2, MidpointRounding.AwayFromZero),
                        Math.Round(pedidoTotalOrdemCarga.Sum(f => f.TotM2PendenteTotal), 2, MidpointRounding.AwayFromZero),
                        Math.Round(pedidoTotalOrdemCarga.Sum(f => f.PesoTotal), 2, MidpointRounding.AwayFromZero),
                        Math.Round(pedidoTotalOrdemCarga.Sum(f => f.PesoPendenteTotal), 2, MidpointRounding.AwayFromZero),
                        Math.Round(pedidoTotalOrdemCarga.Sum(f => f.ValorTotal), 2, MidpointRounding.AwayFromZero));
                }
            }
        }

        #endregion

        #endregion

        #region Carregamento

        public uint? GetFormaPagto(GDASession sessao, uint idPedido)
        {
            return PedidoDAO.Instance.ObtemValorCampo<uint?>(sessao, "idFormaPagto", "idPedido=" + idPedido);
        }


        /// <summary>
        /// Recupera os ids dos pedidos de um carregamento
        /// </summary>
        public List<uint> GetIdsPedidosByCarregamento(GDASession session, uint idCarregamento)
        {
            var sql = @"
                SELECT DISTINCT(p.idPedido)
                FROM pedido p
                    INNER JOIN pedido_ordem_carga poc ON (p.idPedido = poc.idPedido)
                    INNER JOIN ordem_carga oc ON (poc.idOrdemCarga = oc.IdOrdemCarga)
                    INNER JOIN carregamento c ON (oc.idCarregamento = c.idCarregamento)
                WHERE c.idCarregamento =" + idCarregamento;

            return ExecuteMultipleScalar<uint>(session, sql);
        }

        /// <summary>
        /// Recupera os ids dos pedidos de um carregamento para gera a nf de transferencia
        /// </summary>
        public List<uint> GetIdsPedidosByCarregamentoParaNfTransferencia(uint idCarregamento)
        {
            var sql = @"
                SELECT DISTINCT(p.idPedido)
                FROM pedido p
                    INNER JOIN pedido_ordem_carga poc ON (p.idPedido = poc.idPedido)
                    INNER JOIN ordem_carga oc ON (poc.idOrdemCarga = oc.IdOrdemCarga)
                    INNER JOIN carregamento c ON (oc.idCarregamento = c.idCarregamento)
                WHERE c.idCarregamento =" + idCarregamento + " AND oc.TipoOrdemCarga=" + (int)OrdemCarga.TipoOCEnum.Transferencia;

            return ExecuteMultipleScalar<uint>(sql);
        }

        /// <summary>
        /// Verifica se um pedido possiu alguma peça que foi carregada
        /// </summary>
        public bool PossuiPecaCarregada(GDASession sessao, uint idPedido, uint idCarregamento)
        {
            var sql = @"
                SELECT COUNT(*)
                FROM item_carregamento ic
                WHERE ic.Carregado = true AND ic.idPedido=" + idPedido + " AND ic.idCarregamento=" + idCarregamento;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Verifica se um pedido possiu alguma peça que foi carregada
        /// </summary>
        public bool PossuiPecaCarregada(uint idPedido, uint idCarregamento)
        {
            return PossuiPecaCarregada(null, idPedido, idCarregamento);
        }

        /// <summary>
        /// Busca a placa é uf do veiculo do pedido utilizado no carregamento
        /// </summary>
        public KeyValuePair<string, string> ObtemVeiculoCarregamento(string idsPedidos)
        {
            var sql = @"
                SELECT CONCAT(v.Placa, ';', v.UfLicenc)
                FROM veiculo v
	                INNER JOIN carregamento c ON (v.Placa = c.Placa)
                    INNER JOIN ordem_carga oc ON (c.IdCarregamento = oc.IdCarregamento)
                    INNER JOIN pedido_ordem_carga poc ON (oc.IdOrdemCarga = poc.IdOrdemCarga)
                WHERE poc.IdPedido IN " + string.Format("({0})", idsPedidos) + " GROUP by v.Placa";

            var dados = ExecuteMultipleScalar<string>(sql);

            if (dados.Count == 0 || dados.Count > 1)
                return new KeyValuePair<string, string>();

            return new KeyValuePair<string, string>(dados[0].Split(';')[0], dados[0].Split(';')[1]);
        }

        /// <summary>
        /// Recupera os pedidos do cliente do carregamento informado que ainda não possuem carregamento.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idCarregamento"></param>
        /// <returns></returns>
        public List<Pedido> ObterPedidosProntosSemCarregamento(GDASession sessao, uint idCarregamento)
        {
            var sql = @"
                SELECT p.*, c.Id_cli as IdCli, c.Nome as NomeCliente
                FROM pedido p
	                LEFT JOIN cliente c ON (p.IdCli = c.Id_cli)
                    LEFT JOIN pedido_ordem_carga poc ON (poc.IdPedido = p.IdPedido)
                    LEFT JOIN ordem_carga oc ON (poc.IdOrdemCarga = oc.IdOrdemCarga)
                WHERE p.SituacaoProducao = " + (int)Pedido.SituacaoProducaoEnum.Pronto + @"
	                AND p.TipoEntrega = " + DataSources.Instance.GetTipoEntregaEntrega().GetValueOrDefault(0) + @"
                    AND coalesce(oc.IdCarregamento, 0) = 0
                    AND p.IdCli IN 
                    (
		                SELECT oc.IdCliente 
                        FROM ordem_carga oc
		                WHERE oc.IdCarregamento = ?idCarregamento
	                )
                ORDER BY p.DataEntrega, c.Nome";

            return objPersistence.LoadData(sessao, sql, new GDAParameter("?idCarregamento", idCarregamento));
        }

        /// <summary>
        /// Buscar os pedidos para a consulta produção
        /// </summary>
        public List<Pedido> ObterPedidosProducao(GDASession sessao, List<uint> idsPedido)
        {
            if (idsPedido?.Count(f => f > 0) == 0)
            {
                return new List<Pedido>();
            }

            var sql = @"
                SELECT p.*, c.Id_cli as IdCli, c.Nome as NomeCliente
                FROM pedido p
	                LEFT JOIN cliente c ON (p.IdCli = c.Id_cli)
                WHERE p.idPedido IN ({0})
                ORDER BY p.DataEntrega, c.Nome";

            return objPersistence.LoadData(sessao, string.Format(sql, string.Join(",", idsPedido)));
        }

        /// <summary>
        /// Busca os pedidos com peças disponiveis para leitura no setor informado no momento
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public List<Pedido> ObterPedidosPendentesLeitura(GDASession sessao, uint idSetor)
        {
            var sql = @"
                 SELECT  p.*, c.Id_cli as IdCli, c.Nome as NomeCliente
                FROM pedido p
	                LEFT JOIN cliente c On (p.IdCli = c.Id_Cli)
                    INNER JOIN produtos_pedido_espelho pp On (p.IdPedido = pp.IdPedido)
                    INNER JOIN produto_pedido_producao ppp On (ppp.IdProdPed = pp.IdProdPed)
				WHERE ppp.situacao in (" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + "," + (int)ProdutoPedidoProducao.SituacaoEnum.Perda + @")
                                    AND EXISTS
                                    (
                                        SELECT ppp1.*
                                        FROM produto_pedido_producao ppp1
	                                        INNER JOIN roteiro_producao_etiqueta rpe ON (rpe.IdProdPedProducao = ppp1.IdProdPedProducao)
                                        WHERE rpe.IdSetor = ?idSetor
	                                        AND ppp1.idProdPedProducao = ppp.idProdPedProducao
                                            AND ppp1.IdSetor =
                                                /* Se o setor filtrado for o primeiro setor do roteiro, busca somente as peças que estiverem no setor Impressão de Etiqueta. */
                                                IF (?idSetor =
                                                    (
    	                                                SELECT rpe.IdSetor
		                                                FROM produto_pedido_producao ppp2
			                                                INNER JOIN roteiro_producao_etiqueta rpe ON (rpe.IdProdPedProducao = ppp2.IdProdPedProducao)
    		                                                INNER JOIN setor s ON (rpe.IdSetor = s.IdSetor)
		                                                WHERE ppp2.IdProdPedProducao = ppp.IdProdPedProducao
                                                            AND ppp2.IdProdPedProducao IN (SELECT lp1.IdProdPedProducao FROM leitura_producao lp1)
		                                                ORDER BY s.NumSeq ASC
		                                                LIMIT 1
                                                    ), 1,
                                                    /* Senão, busca o próximo setor a ser lido no roteiro. */
                                                    (
    	                                                SELECT rpe.IdSetor
		                                                FROM produto_pedido_producao ppp2
			                                                INNER JOIN roteiro_producao_etiqueta rpe ON (rpe.IdProdPedProducao = ppp2.IdProdPedProducao)
    		                                                INNER JOIN setor s ON (rpe.IdSetor = s.IdSetor)
		                                                WHERE ppp2.IdProdPedProducao = ppp.IdProdPedProducao
			                                                AND s.NumSeq < (SELECT NumSeq FROM setor WHERE IdSetor = ?idSetor)
		                                                ORDER BY s.NumSeq DESC
		                                                LIMIT 1
                                                    ))
                                    )
					GROUP BY p.IdPedido
                    ORDER BY p.DataEntrega, c.Nome";

            return objPersistence.LoadData(sessao, sql, new GDAParameter("?idSetor", idSetor));
        }

        #endregion

        #region Lança uma ValidacaoPedidoFinanceiroException

        /// <summary>
        /// Lança uma ValidacaoPedidoFinanceiroException, se o funcionário não for Financeiro.
        /// </summary>
        private void LancarExceptionValidacaoPedidoFinanceiro(string mensagem, uint idPedido, bool finalizarPedido,
            string idsPedidos, ObservacaoFinalizacaoFinanceiro.MotivoEnum motivo)
        {
            if (FinanceiroConfig.PermitirFinalizacaoPedidoPeloFinanceiro && finalizarPedido)
            {
                if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
                    throw new ValidacaoPedidoFinanceiroException(mensagem, idPedido, idsPedidos, motivo);
            }
            // Chamado 13112.
            // A finalização do pedido pelo financeiro deveria estar separada da confirmação do pedido pelo financeiro.
            else if (FinanceiroConfig.PermitirConfirmacaoPedidoPeloFinanceiro && !finalizarPedido)
            {
                if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
                    throw new ValidacaoPedidoFinanceiroException(mensagem, idPedido, idsPedidos, motivo);
            }
            else
                throw new Exception(mensagem);
        }

        #endregion

        #region Permite que o pedido seja finalizado pelo Financeiro

        /// <summary>
        /// Disponibiliza o pedido para ser finalizado pelo financeiro.
        /// </summary>
        public void DisponibilizaFinalizacaoFinanceiro(GDASession sessao, uint idPedido, string mensagem)
        {
            var sql = @"
                UPDATE pedido SET
                    situacao=" + (int)Pedido.SituacaoPedido.AguardandoFinalizacaoFinanceiro + @",
                    idFuncFinalizarFinanc=" + UserInfo.GetUserInfo.CodUser + @"
                WHERE idPedido =" + idPedido;

            objPersistence.ExecuteCommand(sessao, sql);

            ObservacaoFinalizacaoFinanceiroDAO.Instance.InsereItem(sessao, idPedido, mensagem, ObservacaoFinalizacaoFinanceiro.TipoObs.Finalizacao);
        }

        /// <summary>
        /// Disponibiliza os pedidos para serem confirmados pelo financeiro.
        /// </summary>
        public void DisponibilizaConfirmacaoFinanceiro(GDASession sessao, string idsPedidos, string mensagem)
        {
            var sql = @"
                UPDATE pedido SET
                    situacao=" + (int)Pedido.SituacaoPedido.AguardandoConfirmacaoFinanceiro + @",
                    idFuncConfirmarFinanc=" + UserInfo.GetUserInfo.CodUser + @"
                WHERE idPedido IN(" + idsPedidos + ")";

            objPersistence.ExecuteCommand(sessao, sql);

            foreach (var idPedido in idsPedidos.Split(',').Select(f => f.StrParaUint()).ToList())
            {
                ObservacaoFinalizacaoFinanceiroDAO.Instance
                    .InsereItem(sessao, idPedido, mensagem, ObservacaoFinalizacaoFinanceiro.TipoObs.Confirmacao);
            }
        }

        #endregion

        #region Finalizar Pedido

        /// <summary>
        /// Criar pedido de produção com base no pedido de revenda
        /// </summary>
        public uint CriarPedidoProducaoPedidoRevenda(Pedido pedido)
        {
            var pedidoNovo = (Pedido)pedido.Clone();
            pedidoNovo.IdPedido = 0;
            pedidoNovo.TipoPedido = (int)Pedido.TipoPedidoEnum.Producao;
            pedidoNovo.Situacao = Pedido.SituacaoPedido.Ativo;
            pedidoNovo.IdPedidoRevenda = (int)pedido.IdPedido;
            pedidoNovo.GerarPedidoProducaoCorte = false;
            pedidoNovo.CodCliente = string.Format("({1}) Rev.{0}", pedido.IdPedido, pedidoNovo.CodCliente);

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();
                    Insert(transaction, pedidoNovo);
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

            return pedidoNovo.IdPedido;
        }

        public void VerificaCapacidadeProducaoSetor(uint idPedido, DateTime dataEntrega, float totM2Adicionar, uint idProcessoAdicionar)
        {
            VerificaCapacidadeProducaoSetor(null, idPedido, dataEntrega, totM2Adicionar, idProcessoAdicionar);
        }

        public void VerificaCapacidadeProducaoSetor(GDASession session, uint idPedido, DateTime dataEntrega, float totM2Adicionar, uint idProcessoAdicionar)
        {
            // Valida a capacidade de produção por setor através da data de fábrica do pedido:
            // só valida se a configuração estiver selecionada
            CapacidadeProducaoDAO.Instance.ValidaDataEntregaPedido(session, idPedido, dataEntrega, totM2Adicionar, idProcessoAdicionar);
        }

        /// <summary>
        /// Valida se o produto possui imagem caso o reteiro do mesmo obrigue a ter imagem
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        public void ValidarObrigatoriedadeDeImagemEmPecasAvulsas(GDA.GDASession sessao, int idPedido)
        {
            //Busca os produtos do pedido
            var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedido(sessao, (uint)idPedido, false, true);

            foreach (var prodPed in produtosPedido)
            {
                //Se o produto não tiver imagem e for do grupo vidro
                if (string.IsNullOrEmpty(prodPed.ImagemUrl) && prodPed.IsVidro == "true")
                {
                    //Se for peça de projeto não é necessario vincular imagem no mesmo
                    if (prodPed.IdPecaItemProj > 0)
                        continue;

                    if (prodPed.IdProcesso > 0)
                    {
                        var idRoteiroProducao = RoteiroProducaoDAO.Instance.ObtemValorCampo<int>("IdRoteiroProducao", "idProcesso=" + prodPed.IdProcesso);

                        /* Chamado 55108. */
                        if (idRoteiroProducao == 0)
                            return;

                        var roteiroProducao = RoteiroProducaoDAO.Instance.GetElementByPrimaryKey(idRoteiroProducao);

                        //Verifica se oi roteiro obriga ter imagem na peça
                        if (roteiroProducao.ObrigarAnexarImagemPecaAvulsa)
                        {
                            throw new Exception(string.Format("o produto {0} está em processo que necessita que o produto possua imagem", prodPed.DescrProduto));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Altera a situação do pedido para Conferido
        /// </summary>
        public void FinalizarPedidoComTransacao(uint idPedido, ref bool emConferencia, bool financeiro)
        {
            lock (_finalizarPedidoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        FinalizarPedido(transaction, idPedido, ref emConferencia, financeiro);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (ValidacaoPedidoFinanceiroException f)
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw f;
                    }
                    catch
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Altera a situação do pedido para Conferido
        /// </summary>
        public void FinalizarPedido(GDASession session, uint idPedido, ref bool emConferencia, bool financeiro)
        {
            // Atualiza o total do pedido para ter certeza que o valor está correto, evitando que ocorra novamente o problema no chamado 3202
            UpdateTotalPedido(session, idPedido);

            var pedido = GetElement(session, idPedido);
            var lstProd = ProdutosPedidoDAO.Instance.GetByPedidoLite(session, pedido.IdPedido).ToArray();
            var countProdPed = lstProd.Length;

            /* Chamado 50830. */
            if (pedido.IdLoja == 0)
                throw new Exception("Informe a loja do pedido antes de finalizá-lo.");

            var produtosSemBeneficiamentoObrigatorio = ProdutosPedidoDAO.Instance.VerificarBeneficiamentoObrigatorioAplicado(idPedido);

            if (!produtosSemBeneficiamentoObrigatorio.IsNullOrEmpty())
                throw new Exception(produtosSemBeneficiamentoObrigatorio);

            /* Chamado 57579. */
            var idsLojaSubgrupoProd = ProdutosPedidoDAO.Instance.ObterIdsLojaSubgrupoProdPeloPedido(session, (int)idPedido);

            if (idsLojaSubgrupoProd.Count > 0 && !idsLojaSubgrupoProd.Contains((int)pedido.IdLoja))
                throw new Exception("Não é possível finalizar esse pedido. A loja cadastrada para o subgrupo de um ou mais produtos é diferente da loja selecionada para o pedido.");

            /* Chamado 50830. */
            if (pedido.IdFunc == 0)
                throw new Exception("Informe o vendedor do pedido antes de finalizá-lo.");

            uint? idObra = PedidoConfig.DadosPedido.UsarControleNovoObra ? Instance.GetIdObra(idPedido) : null;
            if (idObra > 0)
            {
                if (ObraDAO.Instance.ObtemSituacao(session, idObra.Value) != Obra.SituacaoObra.Confirmada)
                    throw new Exception("A obra informada não esta confirmada.");

                // Valida apenas os pais dos produtos compostos.
                var lstProdSemComposicao = lstProd.Where(f => f.IdProdPedParent.GetValueOrDefault() == 0).ToList();
                foreach (var p in lstProdSemComposicao)
                {
                    // Verifica se o produto está na obra do pedido e se tem m² suficiente para adiciona-lo
                    var dadosProduto = "'" + p.DescrProduto + "'" + (p.IdAmbientePedido > 0 ? " (ambiente '" + p.Ambiente + "')" : "");
                    var retorno = ProdutoObraDAO.Instance.IsProdutoObra(session, idObra.Value, p.CodInterno);

                    if (!retorno.ProdutoValido)
                        throw new Exception("Não é possível inserir o produto " + dadosProduto + " no pedido. " + retorno.MensagemErro);

                    // Se o pedido tiver forma de pagamento Obra, não permite inserir produto com tipo subgrupo VidroLaminado ou VidroDuplo sem produto de composição.
                    var tipoSubgrupo = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(session, (int)p.IdProd);
                    if (tipoSubgrupo == TipoSubgrupoProd.VidroLaminado || tipoSubgrupo == TipoSubgrupoProd.VidroDuplo)
                    {
                        if (!ProdutoBaixaEstoqueDAO.Instance.TemProdutoBaixa(p.IdProd))
                            throw new Exception("Não é possível inserir produtos do tipo de subgrupo vidro duplo ou laminado sem produto de composição em seu cadastro.");
                    }
                }

                var saldoObra = ObraDAO.Instance.ObtemSaldoComPedConf(session, idObra.Value);

                if (saldoObra < pedido.Total)
                    /* Chamdao 22985. */
                    throw new Exception("O saldo da obra é menor que o valor deste pedido. Saldo da obra: " + saldoObra.ToString("C"));
            }

            // Atualiza o valor da obra no pedido (Chamado 12459)
            if (pedido.IdObra > 0)
            {
                if (PedidoConfig.DadosPedido.UsarControleNovoObra)
                {
                    /* Chamado 27503. */
                    var produtosObra = ProdutoObraDAO.Instance.GetByObra(session, (int)pedido.IdObra.Value);
                    var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedido(session, pedido.IdPedido);

                    foreach (var produtoObra in produtosObra)
                        if (produtosPedido.Any(
                            f =>
                            f.ValorVendido != produtoObra.ValorUnitario &&
                            f.IdProd == produtoObra.IdProd))
                            throw new Exception("Um ou mais produtos estão com o valor vendido diferente do valor unitário definido na obra.");
                }

                /* Chamado 19272. */
                if (ObraDAO.Instance.GetSaldo(session, pedido.IdObra.Value) < pedido.Total)
                    throw new Exception("Não é possível finalizar este pedido pois a obra não possui saldo suficiente.");

                // Atualiza o campo pagamento antecipado
                var valorPagamentoAntecipado = pedido.Total;
                objPersistence.ExecuteCommand(session, "update pedido set valorPagamentoAntecipado=?valor where idPedido=" + pedido.IdPedido,
                    new GDAParameter("?valor", valorPagamentoAntecipado));

                ObraDAO.Instance.AtualizaSaldo(session, pedido.IdObra.Value, false);

                pedido.ValorPagamentoAntecipado = valorPagamentoAntecipado;
            }

            // Garante que o pedido não seja finalizado sem a referência de um tipo de venda
            if (pedido.TipoVenda == null || pedido.TipoVenda == 0)
                throw new Exception("Selecione um Tipo de Venda antes de finalizar o pedido.");

            if (pedido.TipoPedido == 0)
                throw new Exception("Campo tipo pedido zerado.");

            if (pedido.MaoDeObra)
            {
                var ambientes = (pedido as IContainerCalculo).Ambientes.Obter().Cast<AmbientePedido>();
                foreach (var a in ambientes)
                    if (!AmbientePedidoDAO.Instance.PossuiProdutos(session, a.IdAmbientePedido))
                        throw new Exception("O vidro " + a.PecaVidro + " não possui mão-de-obra cadastrada. Cadastre alguma mão-de-obra ou remova o vidro para continuar.");
            }

            // Se não for sistema de liberação de pedido e o pedido for à vista e possuir sinal, não permite finalizá-lo
            if (!PedidoConfig.LiberarPedido && pedido.TipoVenda == (int)Pedido.TipoVendaPedido.AVista && pedido.ValorEntrada > 0)
                throw new Exception("Pedidos à vista não podem ter valor de entrada.");

            // Verifica se o pedido possui projetos não confirmados
            var projetosNaoConfirmados = string.Empty;
            if (!ItemProjetoDAO.Instance.ProjetosConfirmadosPedido(session, idPedido, ref projetosNaoConfirmados))
                throw new Exception("Os seguintes projetos não foram confirmados: " + projetosNaoConfirmados + ", confirme-os antes de finalizar o pedido.");

            // Verifica se o pedido possui cálculos de projeto com ambiente duplicado (Chamado 25137)
            var itemProjDupl = string.Join(",", ExecuteMultipleScalar<string>(session, string.Format(@"
                Select Concat('Ambiente: ', ip.Ambiente, ' no valor de R$', ip.Total)
                From ambiente_pedido ap
                    Inner Join item_projeto ip On (ap.IdItemProjeto=ip.IdItemProjeto)
                Where ap.idPedido={0}
                    And ap.idItemProjeto is not null 
                Group By ap.idItemProjeto 
                Having Count(*) > 1", idPedido)).ToArray());

            if (!string.IsNullOrEmpty(itemProjDupl))
                throw new Exception(string.Format("Alguns projetos estão com ambientes duplicados: {0}", itemProjDupl));

            /* Chamado 52139. */
            if (objPersistence.ExecuteSqlQueryCount(session, string.Format("SELECT COUNT(*) FROM ambiente_pedido WHERE IdPedido = {0} AND IdItemProjeto > 0", idPedido)) !=
                objPersistence.ExecuteSqlQueryCount(session, string.Format("SELECT COUNT(*) FROM item_projeto WHERE IdPedido = {0}", idPedido)))
                throw new Exception("Existem projetos calculados no pedido sem ambiente associado. Exclua os projetos sem ambiente e tente novamente.");

            // Verifica se algum Projeto Modelo utilizado no pedido está bloqueado para gerar novos pedidos.
            var modelosProjetoBloqueados = string.Empty;
            var idsProjetoModelo = ItemProjetoDAO.Instance.ObterIdsProjetoModeloPorPedido(session, idPedido);
            foreach (var id in idsProjetoModelo)
            {
                if (ProjetoModeloDAO.Instance.ObterSituacao(session, id) == ProjetoModelo.SituacaoEnum.Bloqueado)
                    modelosProjetoBloqueados += ProjetoModeloDAO.Instance.ObtemCodigo(session, id) + ", ";
            }
            if (!string.IsNullOrEmpty(modelosProjetoBloqueados))
                throw new Exception(string.Format("O(s) projeto(s) {0} esta(ão) bloqueado(s) para gerar novos pedidos. Selecione outro projeto para continuar.",
                    modelosProjetoBloqueados.Remove(modelosProjetoBloqueados.Length - 2, 2)));

            // Verifica se este pedido pode ter desconto
            if (PedidoConfig.Desconto.ImpedirDescontoSomativo && UserInfo.GetUserInfo.TipoUsuario != (uint)Utils.TipoFuncionario.Administrador &&
                pedido.Desconto > 0 && DescontoAcrescimoClienteDAO.Instance.ClientePossuiDesconto(session, pedido.IdCli, 0, null, idPedido, null))
            {
                if (pedido.Desconto > 0)
                    throw new Exception("O cliente já possui desconto por grupo/subgrupo, não é permitido lançar outro desconto no pedido.");

                var ambientesPedido = (pedido as IContainerCalculo).Ambientes.Obter().Cast<AmbientePedido>();

                string msg;
                foreach (var amb in ambientesPedido)
                    if (amb.Desconto > 0 && !AmbientePedidoDAO.Instance.ValidaDesconto(session, amb, out msg))
                        throw new Exception(msg + " Ambiente " + amb.Ambiente + ".");
            }

            // Se a empresa não usa liberação parcial de pedidos, não deve ter nenhuma liberação ativa para este pedido
            if (!Liberacao.DadosLiberacao.LiberarPedidoProdutos && !Liberacao.DadosLiberacao.LiberarPedidoAtrasadoParcialmente &&
                LiberarPedidoDAO.Instance.GetIdsLiberacaoAtivaByPedido(idPedido).Count > 0)
                throw new Exception("Este pedido já possui uma liberação ativa.");

            // Quando aplicável, verifica se os produtos do pedido existem em estoque
            if (pedido.TipoPedido != (int)Pedido.TipoPedidoEnum.Producao)
            {
                var pedidoReposicaoGarantia = pedido.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição || pedido.TipoVenda == (int)Pedido.TipoVendaPedido.Garantia;
                var pedidoMaoObraEspecial = pedido.TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObraEspecial;

                foreach (var prod in lstProd)
                {
                    float qtdProd = 0;
                    var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(session, (int)prod.IdProd);

                    // É necessário refazer o loop nos produtos do pedido para que caso tenha sido inserido o mesmo produto 2 ou mais vezes,
                    // seja somada a quantidade total inserida no pedido
                    foreach (var prod2 in lstProd)
                    {
                        // Soma somente produtos iguais ao produto do loop principal de produtos
                        if (prod.IdProd != prod2.IdProd)
                            continue;

                        if (tipoCalculo == (int)TipoCalculoGrupoProd.M2 || tipoCalculo == (int)TipoCalculoGrupoProd.M2Direto)
                            qtdProd += prod2.TotM;
                        else if (tipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 ||
                            tipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)TipoCalculoGrupoProd.MLAL6)
                            qtdProd += prod2.Qtde * prod2.Altura;
                        else
                            qtdProd += prod2.Qtde;
                    }

                    if (GrupoProdDAO.Instance.BloquearEstoque(session, (int)prod.IdGrupoProd, (int)prod.IdSubgrupoProd))
                    {
                        var estoque = ProdutoLojaDAO.Instance.GetEstoque(session, pedido.IdLoja, prod.IdProd, null, IsProducao(session, idPedido), false, true);

                        if (estoque < qtdProd)
                            throw new Exception("O produto " + prod.DescrProduto + " possui apenas " + estoque + " em estoque.");
                    }

                    // Verifica se o valor unitário do produto foi informado, pois pode acontecer do usuário inserir produtos zerados em 
                    // um pedido reposição/garantia e depois alterar o pedido para à vista/à prazo
                    if (!pedidoReposicaoGarantia && prod.ValorVendido == 0)
                        throw new Exception("O produto " + prod.DescrProduto + " não pode ter valor zerado.");

                    if (!pedidoReposicaoGarantia &&
                        pedido.TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObra &&
                        prod.EspessuraBenef == 0 && prod.AlturaBenef == 0 && prod.LarguraBenef == 0 && prod.Total == 0)
                        throw new Exception(
                            string.Format("Informe a altura, largura e espessura do beneficiamento {0}.", prod.DescrProduto));
                }
            }

            // Verifica se o tipo de entrega foi informado.
            if (pedido.TipoEntrega == null)
                throw new Exception("Informe o tipo de entrega do pedido.");

            // Verifica a data de entrega mínima
            if (pedido.DataEntrega == null)
                throw new Exception("A data de entrega não pode ser vazia.");

            DateTime dataMinima, dataFastDelivery;
            if (pedido.GeradoParceiro)
            {
                GetDataEntregaMinima(session, pedido.IdCli, idPedido, out dataMinima, out dataFastDelivery);
                pedido.DataEntrega = pedido.FastDelivery ? dataFastDelivery : dataMinima;
                objPersistence.ExecuteCommand(session, "Update pedido Set dataEntrega=?dataEntrega Where idPedido=" + idPedido, new GDAParameter("?dataEntrega", dataMinima));
            }

            // A verificação de bloquear ou não data de entrega mínima foi removida
            if (BloquearDataEntregaMinima(session, idPedido) && GetDataEntregaMinima(session, pedido.IdCli, idPedido, out dataMinima, out dataFastDelivery) && pedido.DataEntrega < dataMinima.Date)
                throw new Exception("A data de entrega não pode ser anterior a " + dataMinima.ToString("dd/MM/yyyy") + ".");

            if (pedido.DataEntrega < DateTime.Now.Date)
            {
                var mensagem = "A data de entrega não pode ser inferior à data de hoje.";

                if (financeiro)
                    mensagem = "É necessário negar a finalização para que o comercial calcule a data de entrega novamente.";

                throw new Exception(mensagem);
            }

            VerificaCapacidadeProducaoSetor(session, idPedido, pedido.DataEntrega.Value, 0, 0);

            // Se for cliente de rota, verifica se a data escolhida bate com o dia da rota
            uint? idRota = ClienteDAO.Instance.ObtemIdRota(session, pedido.IdCli);
            if (idRota > 0)
            {
                var diaSemanaRota = (DiasSemana)RotaDAO.Instance.ObtemValorCampo<uint>(session, "diasSemana", "idRota=" + idRota.Value);
                if (pedido.TipoEntrega != (int)Pedido.TipoEntregaPedido.Balcao && !pedido.FastDelivery &&
                    diaSemanaRota != DiasSemana.Nenhum && !RotaDAO.Instance.TemODia(pedido.DataEntrega.Value.DayOfWeek, diaSemanaRota) &&
                    !Config.PossuiPermissao(Config.FuncaoMenuPedido.IgnorarBloqueioDataEntrega))
                {
                    var rota = new Rota() { DiasSemana = diaSemanaRota };
                    throw new Exception("A data de entrega deste pedido deve ser no mesmo dia da rota deste cliente (" + rota.DescrDiaSemana + ").");
                }
            }

            // Verifica se o Pedido possui produtos
            if (countProdPed == 0)
                throw new Exception("Inclua pelo menos um produto no pedido para finalizá-lo.");
            // Se houver apenas um produto associado ao pedido e este contiver a palavra conferencia,
            // ao invés de finalizar o pedido, altera sua situação para em conferencia
            if (countProdPed == 1 && lstProd[0].DescrProduto.ToLower().Contains("conferencia"))
            {
                emConferencia = true;
                return;
            }
            // Verifica se o pedido contém produtos TOTAL ou PEDIDO EM CONFERÊNCIA
            if (lstProd.Length <= 2)
            {
                string descrProd;

                foreach (var p in lstProd)
                {
                    descrProd = p.DescrProduto;

                    if (!string.IsNullOrEmpty(descrProd) && (descrProd.Trim().ToLower() == "t o t a l" ||
                        descrProd.Trim().ToLower() == "total" || descrProd.Trim().ToLower() == "pedido em conferencia"))
                        throw new Exception("Inclua pelo menos um produto no pedido que não seja o produto TOTAL ou PEDIDO EM CONFERENCIA para finalizá-lo.");
                }
            }

            // Verifica os processos/aplicações dos produtos
            if (PedidoConfig.DadosPedido.ObrigarProcAplVidros && !Geral.SistemaLite)
            {
                var tipoCalcBenef = new List<int> { (int)TipoCalculoGrupoProd.M2, (int)TipoCalculoGrupoProd.M2Direto };
                var usarRoteiroProducao = Utils.GetSetores.Count(x => x.SetorPertenceARoteiro) > 0;

                if (pedido.TipoPedido != (int)Pedido.TipoPedidoEnum.MaoDeObra)
                {
                    foreach (var p in lstProd)
                    {
                        var isVidroBenef = Geral.UsarBeneficiamentosTodosOsGrupos || tipoCalcBenef.Contains(p.TipoCalc);
                        var descrProduto = p.DescrProduto + " (altura " + p.Altura + " largura " + p.Largura + " qtde " + p.Qtde + ")";

                        if (Glass.Data.DAL.GrupoProdDAO.Instance.IsVidro((int)p.IdGrupoProd) &&
                            (usarRoteiroProducao || isVidroBenef))
                        {
                            if (SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(session, (int)p.IdProd) == TipoSubgrupoProd.ChapasVidro)
                                continue;

                            if (p.IdAplicacao == null || p.IdAplicacao == 0)
                                throw new Exception("Informe a aplicação do produto '" + descrProduto + "'.");

                            if (p.IdProcesso == null || p.IdProcesso == 0)
                                throw new Exception("Informe o processo do produto '" + descrProduto + "'.");
                        }
                    }
                }
                else
                {
                    var ambientes = (pedido as IContainerCalculo).Ambientes.Obter().Cast<AmbientePedido>();

                    foreach (var a in ambientes)
                    {
                        var descrAmbiente = a.Ambiente + " (altura " + a.Altura + " largura " + a.Largura + " qtde " + a.Qtde + ")";

                        if (usarRoteiroProducao)
                        {
                            if (a.IdAplicacao == null)
                                throw new Exception("Informe a aplicação do ambiente '" + descrAmbiente + "'.");

                            if (a.IdProcesso == null)
                                throw new Exception("Informe o processo do ambiente '" + descrAmbiente + "'.");
                        }
                    }
                }
            }

            /* Chamado 56301. */
            if (lstProd.Any(f => f.IdSubgrupoProd == 0))
                throw new Exception(string.Format("Informe o subgrupo dos produtos {0} antes de finalizar o pedido.",
                    string.Join(", ", lstProd.Where(f => f.IdSubgrupoProd == 0).Select(f => f.CodInterno).Distinct().ToList())));

            ValidaTipoPedidoTipoProduto(session, pedido, lstProd);

            // Verifica se a data de entrega foi informada
            if (pedido.DataEntrega == null)
                throw new Exception("Informe a data de entrega do pedido.");

            if (!PedidoConfig.LiberarPedido && pedido.TipoVenda == (int)Pedido.TipoVendaPedido.APrazo)
            {
                var lstParc = ParcelasPedidoDAO.Instance.GetByPedido(session, pedido.IdPedido).ToArray();

                foreach (var p in lstParc)
                    if (p.Data == null || p.Data.Value.Year == 1)
                        throw new Exception("Informe a data de todas as parcelas.");
            }

            if (pedido.TipoVenda == (int)Pedido.TipoVendaPedido.APrazo && PedidoConfig.FormaPagamento.ExibirDatasParcelasPedido)
            {
                var parcelasNaoUsar = ParcelasNaoUsarDAO.Instance.ObterPeloCliente(session, (int)pedido.IdCli);

                if (parcelasNaoUsar.Count(f => f.IdParcela == pedido.IdParcela) > 0)
                    throw new Exception("A parcela do pedido não está disponível para o cliente. Selecione uma parcela no pedido antes de finalizá-lo.");
            }

            if (pedido.TipoPedido != (int)Pedido.TipoPedidoEnum.Producao && PedidoConfig.DadosPedido.ObrigarInformarPedidoCliente)
            {
                if (string.IsNullOrEmpty(pedido.CodCliente))
                    throw new Exception("Cadastre o cód. ped. cli antes de finalizar o pedido.");
            }

            // Verifica a medida dos vidros do pedido
            if (PedidoConfig.TamanhoVidro.UsarTamanhoMaximoVidro && !pedido.TemperaFora)
            {
                foreach (var p in lstProd)
                {
                    if (!GrupoProdDAO.Instance.IsVidro((int)p.IdGrupoProd))
                        continue;

                    if (p.Altura > PedidoConfig.TamanhoVidro.AlturaMaximaVidro)
                    {
                        if (p.Altura > PedidoConfig.TamanhoVidro.LarguraMaximaVidro || p.Largura > PedidoConfig.TamanhoVidro.AlturaMaximaVidro)
                            throw new Exception("O produto '" + p.DescrProduto + "' não pode ter altura maior que " + PedidoConfig.TamanhoVidro.AlturaMaximaVidro + ".");
                    }
                    else if (p.Largura > PedidoConfig.TamanhoVidro.LarguraMaximaVidro)
                    {
                        if (p.Altura > PedidoConfig.TamanhoVidro.LarguraMaximaVidro ||
                            p.Largura > PedidoConfig.TamanhoVidro.AlturaMaximaVidro)
                            throw new Exception("O produto '" + p.DescrProduto + "' não pode ter largura maior que " + PedidoConfig.TamanhoVidro.LarguraMaximaVidro + ".");
                    }
                }
            }

            if (Configuracoes.ComissaoConfig.ComissaoPorContasRecebidas && pedido.TipoVenda == (int)Pedido.TipoVendaPedido.Obra)
            {
                var idObraPed = GetIdObra(session, idPedido);

                if (idObraPed.GetValueOrDefault() > 0)
                {
                    var idLojaPed = ObtemIdLoja(session, idPedido);
                    var idFunc = ObraDAO.Instance.ObtemIdFunc(session, idObraPed.Value);
                    var idLojaFunc = FuncionarioDAO.Instance.ObtemIdLoja(session, idFunc);
                    var idCliente = ObraDAO.Instance.ObtemIdCliente(session, (int)idObraPed);
                    var idLojaCliente = ClienteDAO.Instance.ObtemIdLoja(session, (uint)idCliente);

                    if (Geral.ConsiderarLojaClientePedidoFluxoSistema && idLojaCliente > 0)
                    {
                        if (idLojaCliente != idLojaPed)
                            throw new Exception("A loja da obra informada é diferente da loja do pedido.");
                    }
                    else if (idLojaFunc != idLojaPed)
                        throw new Exception("A loja da obra informada é diferente da loja do pedido.");
                }
            }

            /* Chamado 56050. */
            if (!pedido.GeradoParceiro)
            {
                //Chamado 46533
                string msgDiasMinEntrega;
                var prodsDiasMinEntrega = lstProd.Where(f => f.IdAplicacao.GetValueOrDefault() > 0).Select(f => new KeyValuePair<int, uint>((int)f.IdProd, f.IdAplicacao.Value)).ToList();
                if (!EtiquetaAplicacaoDAO.Instance.VerificaPrazoEntregaAplicacao(session, prodsDiasMinEntrega, pedido.DataEntrega.GetValueOrDefault(), out msgDiasMinEntrega))
                    throw new Exception(msgDiasMinEntrega);
            }

            // Verifica se há pedidos atrasados que impedem a finalização deste pedido
            var numPedidosBloqueio = GetCountBloqueioEmissao(session, pedido.IdCli);
            if (numPedidosBloqueio > 0)
            {
                var dias = " há pelo menos " + PedidoConfig.NumeroDiasPedidoProntoAtrasado + " dias ";
                var inicio = numPedidosBloqueio > 1 ? "Os pedidos " : "O pedido ";
                var fim = numPedidosBloqueio > 1 ? " estão prontos" + dias + "e ainda não foram liberados" : " está pronto" + dias + "e ainda não foi liberado";

                LancarExceptionValidacaoPedidoFinanceiro("Não é possível finalizar esse pedido. " + inicio + GetIdsBloqueioEmissao(session, pedido.IdCli) +
                    fim + " para o cliente.", idPedido, true, null, ObservacaoFinalizacaoFinanceiro.MotivoEnum.Finalizacao);
            }

            // Verifica se o cliente está inativo
            if (ClienteDAO.Instance.GetSituacao(session, pedido.IdCli) != (int)SituacaoCliente.Ativo)
                LancarExceptionValidacaoPedidoFinanceiro("O cliente selecionado está inativo. Motivo: " +
                    ClienteDAO.Instance.ObtemValorCampo<string>(session, "obs", "id_Cli=" + pedido.IdCli), idPedido, true, null,
                    ObservacaoFinalizacaoFinanceiro.MotivoEnum.Finalizacao);

            // Bloqueia a forma de pagamento se o cliente não puder usá-la
            if (ParcelasDAO.Instance.GetCountByCliente(session, pedido.IdCli, ParcelasDAO.TipoConsulta.Todos) > 0)
            {
                if (ParcelasDAO.Instance.GetCountByCliente(session, pedido.IdCli, ParcelasDAO.TipoConsulta.Prazo) == 0 && pedido.TipoVenda == 2)
                    LancarExceptionValidacaoPedidoFinanceiro("O cliente " + pedido.IdCli + " - " + ClienteDAO.Instance.GetNome(session, pedido.IdCli) +
                        " não pode fazer compras à prazo.", idPedido, true, null, ObservacaoFinalizacaoFinanceiro.MotivoEnum.Finalizacao);
            }

            //Verifica se o cliente possui contas a receber vencidas se nao for garantia
            if ((ClienteDAO.Instance.ObtemValorCampo<bool>(session, "bloquearPedidoContaVencida", "id_Cli=" + pedido.IdCli)) &&
                ContasReceberDAO.Instance.ClientePossuiContasVencidas(session, pedido.IdCli) &&
                pedido.TipoVenda != (int)Pedido.TipoVendaPedido.Garantia)
            {
                LancarExceptionValidacaoPedidoFinanceiro("Cliente bloqueado. Motivo: Contas a receber em atraso.", idPedido, true,
                    null, ObservacaoFinalizacaoFinanceiro.MotivoEnum.Finalizacao);
            }

            // Verifica se pode liberar o epdido com base na rentailidade do mesmo
            if (Configuracoes.RentabilidadeConfig.ControlarFaixaRentabilidadeLiberacao &&
                RentabilidadeHelper.ObterVerificadorLiberacao<Pedido>().VerificarRequerLiberacao(session, pedido))
            {
                if (pedido.Situacao == Pedido.SituacaoPedido.AguardandoFinalizacaoFinanceiro)
                    throw new Exception("Você não possui permissão para liberar o pedido com base no percentual de rentabilidade.");

                LancarExceptionValidacaoPedidoFinanceiro("O percentual de rentabilidade do pedido deve ser verificado.", idPedido, true,
                    null, ObservacaoFinalizacaoFinanceiro.MotivoEnum.Finalizacao);
            }

            // Garante que o pedido possa ser finalizado
            var situacao = ObtemSituacao(session, idPedido);

            if (situacao != Pedido.SituacaoPedido.Ativo && situacao != Pedido.SituacaoPedido.AtivoConferencia &&
                situacao != Pedido.SituacaoPedido.EmConferencia && situacao != Pedido.SituacaoPedido.AguardandoFinalizacaoFinanceiro)
                throw new Exception("Apenas pedidos abertos podem ser finalizados.");

            var pagamentoAntesProducao = ClienteDAO.Instance.IsPagamentoAntesProducao(session, pedido.IdCli);
            var percSinalMinimo = ClienteDAO.Instance.GetPercMinSinalPedido(session, pedido.IdCli);
            var tipoPagto = ClienteDAO.Instance.ObtemValorCampo<uint?>(session, "tipoPagto", "id_Cli=" + pedido.IdCli);

            #region Calcula o sinal/parcelas do pedido

            var calculouSinal = false;
            // Comentado porque na Alternativa teria que ter calculado o sinal do pedido de revenda mas não foi calculado;
            if (percSinalMinimo > 0 && /*(pedido.Importado || ((pagamentoAntesProducao || pedido.TipoPedido == (int)Pedido.TipoPedidoEnum.Venda) && */
                ((pedido.ValorEntrada == 0 && pedido.ValorPagamentoAntecipado == 0) ||
                /* Chamado 15647.
                 * O valor de entrada foi calculado corretamente ao finalizar o pedido, porém, o usuário editou o pedido, alterou o valor de um produto,
                 * e consequentemente alterou o valor total do pedido. Ao finalizar o pedido, o valor de entrada não foi recalculado,
                 * resultando em um valor de entrada menor que o mínimo permitido para o cliente. */
                (pedido.ValorEntrada + pedido.ValorPagamentoAntecipado < Math.Round(pedido.Total * (decimal)(percSinalMinimo.Value / 100), 2))) &&
                (PedidoConfig.LiberarPedido || pedido.TipoVenda == (int)Pedido.TipoVendaPedido.APrazo))
            {
                pedido.ValorEntrada = Math.Round(pedido.Total * (decimal)(percSinalMinimo.Value / 100), 2);
                calculouSinal = true;
            }

            var calculouParc = false;
            if (PedidoConfig.FormaPagamento.ExibirDatasParcelasPedido && pedido.TipoVenda == (int)Pedido.TipoVendaPedido.APrazo &&
                (pedido.IdParcela > 0 || tipoPagto > 0) && (calculouSinal || pedido.Importado || ParcelasPedidoDAO.Instance.GetCount(session, idPedido) == 0))
            {
                RecalculaParcelas(session, ref pedido, TipoCalculoParcelas.Ambas);
                calculouParc = true;
            }
            // Chamado 10264, ao alterar o tipo de venda do pedido de prazo para reposição a parcela não foi removida do pedido.
            else if (pedido.TipoVenda != (int)Pedido.TipoVendaPedido.APrazo && ParcelasPedidoDAO.Instance.GetCount(session, idPedido) > 0)
                ParcelasPedidoDAO.Instance.DeleteFromPedido(session, pedido.IdPedido);

            if (calculouSinal || calculouParc)
                Update(session, pedido);

            /* Chamado 38360. */
            if (pedido.TipoVenda == (int)Pedido.TipoVendaPedido.APrazo)
            {
                /* Chamado 56647.
                 * Verifica se o pedido possui parcelas somente se ele não tiver sido pago totalmente antecipadamente. */
                if (Math.Round(pedido.ValorEntrada + pedido.ValorPagamentoAntecipado, 2) < Math.Round(pedido.Total, 2))
                {
                    var temParcelas = ExecuteScalar<int>(session, string.Format("SELECT COUNT(*) FROM parcelas_pedido WHERE IdPedido = {0}", idPedido)) > 0;

                    if (!temParcelas)
                        throw new Exception("Selecione uma parcela antes de finalizar o pedido ou altere o tipo de venda para À Vista caso ele tenha sido recebido.");
                }

                var temParcelasNegativas = ExecuteScalar<int>(session, string.Format("SELECT COUNT(*) FROM parcelas_pedido WHERE IdPedido = {0} AND Valor < 0", idPedido)) > 0;

                /* Chamado 47506. */
                if (temParcelasNegativas)
                    throw new Exception("Existem parcelas negativas informadas no pedido, edite-o, recalcule as parcelas e tente finalizá-lo novamente.");
            }

            /* Chamado 65135. */
            if (FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto && pedido.IdFormaPagto.GetValueOrDefault() == 0 &&
                (pedido.TipoVenda == (int)Pedido.TipoVendaPedido.AVista || pedido.TipoVenda == (int)Pedido.TipoVendaPedido.APrazo))
                throw new Exception("Não é possível finalizar o pedido, pois a forma de pagamento não foi selecionada.");

            #endregion

            // Verifica se o cliente deve pagar um percentual mínimo de sinal
            if (pedido.ValorPagamentoAntecipado == 0 && (pedido.TipoVenda == (int)Pedido.TipoVendaPedido.APrazo || (PedidoConfig.LiberarPedido && pedido.TipoVenda == (int)Pedido.TipoVendaPedido.AVista && (pagamentoAntesProducao || pedido.TipoPedido == (int)Pedido.TipoPedidoEnum.Venda))) && percSinalMinimo != null)
            {
                var valorMinimoSinal = Math.Round(pedido.Total * (decimal)(percSinalMinimo.Value / 100), 2);
                if (pedido.ValorEntrada < valorMinimoSinal)
                    LancarExceptionValidacaoPedidoFinanceiro("Esse cliente deve pagar um percentual mínimo de " +
                        percSinalMinimo + "% como sinal.\\nValor mínimo para o sinal: " + valorMinimoSinal.ToString("C"),
                        idPedido, true, null, ObservacaoFinalizacaoFinanceiro.MotivoEnum.Finalizacao);
            }

            // Verifica se o valor de entrada somado ao valor do pagamento antecipado ultrapassam o valor total do pedido, chamado 9875.
            if ((pedido.ValorEntrada + pedido.ValorPagamentoAntecipado) > pedido.Total)
            {
                throw new Exception("Não é possível finalizar o pedido. Motivo: " +
                    "O valor de entrada somado ao valor pago antecipadamente ultrapassa o valor total do pedido.");
                /* Chamado 22608. */
                /*LancarExceptionValidacaoPedidoFinanceiro("Não é possível finalizar o pedido. Motivo: " +
                    "O valor de entrada somado ao valor pago antecipadamente ultrapassa o valor total do pedido.",
                    idPedido, true, null, ObservacaoFinalizacaoFinanceiro.MotivoEnum.Finalizacao);*/
            }

            //Valida se todas os produtos do pedido não necessitam de imagem nelas
            ValidarObrigatoriedadeDeImagemEmPecasAvulsas(session, (int)idPedido);

            if (PedidoConfig.Comissao.PerComissaoPedido && pedido.PercentualComissao == 0)
            {
                var pedidoLog = GetElementByPrimaryKey(session, idPedido);
                var comissaoConfig = ComissaoConfigDAO.Instance.GetComissaoConfig(session, pedido.IdFunc);

                var percComissao = comissaoConfig.PercFaixaUm;

                if (PedidoConfig.Comissao.UsarComissaoPorTipoPedido)
                    percComissao = (float)comissaoConfig.ObterPercentualPorTipoPedido((Pedido.TipoPedidoEnum)pedido.TipoPedido);

                objPersistence.ExecuteCommand(session, "update pedido set PercentualComissao = ?p where idPedido = " + idPedido, new GDAParameter("?p", percComissao));

                LogAlteracaoDAO.Instance.LogPedido(session, pedidoLog, GetElementByPrimaryKey(session, pedido.IdPedido), LogAlteracaoDAO.SequenciaObjeto.Atual);
            }

            // Define se a situação do pedido será alterada para "Confirmado PCP"
            var marcarPedidoConfirmadoPCP = PedidoConfig.LiberarPedido &&
                (!PedidoConfig.DadosPedido.BloquearItensTipoPedido || pedido.TipoPedido == (int)Pedido.TipoPedidoEnum.Revenda) &&
                !ProdutosPedidoDAO.Instance.PossuiVidroCalcM2(session, idPedido);

            if (pedido.TipoPedido == (int)Pedido.TipoPedidoEnum.Producao)
            {
                if (!PedidoConfig.LiberarPedido || pedido.TemperaFora)
                    Instance.AlteraSituacao(session, pedido.IdPedido, Pedido.SituacaoPedido.Confirmado);
                else
                {
                    try
                    {
                        //PedidoDAO.Instance.AlteraSituacao(pedido.IdPedido, Pedido.SituacaoPedido.ConfirmadoLiberacao);
                        ConfirmarLiberacaoPedido(session, idPedido.ToString(), true);
                    }
                    catch (ValidacaoPedidoFinanceiroException f)
                    {
                        AlteraSituacao(session, pedido.IdPedido, Pedido.SituacaoPedido.Conferido);
                        throw f;
                    }
                    catch (Exception ex)
                    {
                        if (Geral.NaoVendeVidro())
                            throw ex;

                        AlteraSituacao(session, pedido.IdPedido, Pedido.SituacaoPedido.Conferido);
                    }
                }
            }
            // Se for venda à prazo            
            else if (pedido.TipoVenda == (int)Pedido.TipoVendaPedido.APrazo)
            {
                var valorPagamentoAntecipado = ObtemValorCampo<decimal>(session, "ValorPagamentoAntecipado", string.Format("IdPedido={0}", idPedido));

                // Não permite que consumidor final tire pedidos à prazo
                if (pedido.TipoVenda == (int)Pedido.TipoVendaPedido.APrazo && ClienteDAO.Instance.GetNome(session, pedido.IdCli).ToLower().Contains("consumidor final"))
                    throw new Exception("Não é permitido efetuar pedido à prazo para Consumidor Final.");

                // Calcula o valor de entrada + o valor à prazo
                var valorTotalPago = pedido.ValorEntrada;

                // Busca o valor das parcelas
                var lstParc = ParcelasPedidoDAO.Instance.GetByPedido(session, pedido.IdPedido).ToArray();

                /* Chamado 56647. */
                if (lstParc.Count() == 0)
                    valorTotalPago += pedido.ValorPagamentoAntecipado;
                else
                    foreach (var p in lstParc)
                        valorTotalPago += p.Valor;

                // Verifica se o valor total do pedido bate com o valorTotalPago e se o pagamento antecipado bate com o total do pedido
                if (Math.Round(pedido.Total, 2) != Math.Round(valorTotalPago, 2) && valorPagamentoAntecipado != pedido.Total)
                    throw new Exception("O valor total do pedido não bate com o valor do pagamento do mesmo. Valor Pedido: R$" + Math.Round(pedido.Total, 2) + " Valor Pago: R$" + Math.Round(valorTotalPago, 2));

                if (!pagamentoAntesProducao || !PedidoConfig.LiberarPedido ||
                    FinanceiroConfig.DebitosLimite.EmpresaConsideraPedidoConferidoLimite)
                    VerificaLimite(session, pedido, true);

                // Altera a situação do pedido
                if (pedido.Situacao != Pedido.SituacaoPedido.Confirmado)
                {
                    // Se for liberação e o pedido não possuir produtos do grupo vidro calculado por m², muda para Confirmado
                    if (marcarPedidoConfirmadoPCP)
                    {
                        try
                        {
                            ConfirmarLiberacaoPedido(session, idPedido.ToString(), true);
                        }
                        catch (ValidacaoPedidoFinanceiroException f)
                        {
                            AlteraSituacao(session, pedido.IdPedido, Pedido.SituacaoPedido.Conferido);
                            throw f;
                        }
                        catch
                        {
                            AlteraSituacao(session, pedido.IdPedido, Pedido.SituacaoPedido.Conferido);
                        }
                    }
                    else
                        AlteraSituacao(session, pedido.IdPedido, Pedido.SituacaoPedido.Conferido);
                }
            }
            else if (pedido.TipoVenda == (int)Pedido.TipoVendaPedido.AVista || pedido.TipoVenda == (int)Pedido.TipoVendaPedido.Obra ||
                pedido.TipoVenda == (int)Pedido.TipoVendaPedido.Funcionario)
            {
                // Altera a situação do pedido para Conferido
                if (pedido.Situacao != Pedido.SituacaoPedido.Confirmado)
                {
                    // Verifica se cliente possui limite disponível para realizar a compra, mesmo pedido à vista
                    if (FinanceiroConfig.DebitosLimite.EmpresaConsideraPedidoConferidoLimite &&
                        (pedido.TipoVenda != (int)Pedido.TipoVendaPedido.AVista ||
                        !PedidoConfig.EmpresaPermiteFinalizarPedidoAVistaSemVerificarLimite) &&
                        pedido.TipoVenda != (int)Pedido.TipoVendaPedido.Obra)
                        VerificaLimite(session, pedido, true);

                    // Se for liberação e o pedido não possuir produtos do grupo vidro calculado por m², muda para Confirmado
                    if (PedidoConfig.LiberarPedido && !ProdutosPedidoDAO.Instance.PossuiVidroCalcM2(session, idPedido))
                    {
                        try
                        {
                            //PedidoDAO.Instance.AlteraSituacao(pedido.IdPedido, Pedido.SituacaoPedido.ConfirmadoLiberacao);
                            ConfirmarLiberacaoPedido(session, idPedido.ToString(), true);
                        }
                        catch (ValidacaoPedidoFinanceiroException f)
                        {
                            AlteraSituacao(session, pedido.IdPedido, Pedido.SituacaoPedido.Conferido);
                            throw new ValidacaoPedidoFinanceiroException(f.Message, pedido.IdPedido, f.IdsPedidos, f.Motivo);
                        }
                        catch (Exception ex)
                        {
                            if (Geral.NaoVendeVidro())
                                throw ex;

                            AlteraSituacao(session, pedido.IdPedido, Pedido.SituacaoPedido.Conferido);
                        }
                    }
                    else
                        AlteraSituacao(session, pedido.IdPedido, Pedido.SituacaoPedido.Conferido);
                }
            }
            else if (pedido.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição)
            {
                if (UserInfo.GetUserInfo.TipoUsuario == (int)Utils.TipoFuncionario.Vendedor &&
                    !Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoGarantiaReposicao))
                    throw new Exception("Apenas o gerente pode emitir pedido de reposição.");

                #region Valida produtos reposição

                /* Chamado 12090.
                 * Aconteceu um caso em que o produto incluído em um pedido de reposição não existia no pedido original.
                 * Por isso, fizemos a verificação abaixo, que valida a identificação do produto e a quantidade a ser reposta.
                 */

                if (pedido.IdPedidoAnterior > 0)
                {
                    // Dicionário criado para salvar a identificação do produto e a quantidade total do mesmo no pedido original.
                    var dicProdQtdeOrig = new Dictionary<uint, float>();

                    // Salva no dicionário cada produto e sua quantidade total, inseridos no pedido original.
                    foreach (var prodPedOrig in ProdutosPedidoDAO.Instance.GetByPedidoLite(session, pedido.IdPedidoAnterior.GetValueOrDefault()))
                    {
                        if (!dicProdQtdeOrig.ContainsKey(prodPedOrig.IdProd))
                            dicProdQtdeOrig.Add(prodPedOrig.IdProd, prodPedOrig.Qtde);
                        else
                            dicProdQtdeOrig[prodPedOrig.IdProd] = prodPedOrig.Qtde;
                    }

                    // Verifica se os produtos do pedido de reposição existem no pedido original e, se existirem, valida a quantidade inserida.
                    foreach (var prod in lstProd)
                    {
                        if (!dicProdQtdeOrig.ContainsKey(prod.IdProd))
                            throw new Exception("Apenas produtos do pedido original podem ser inclusos no pedido de reposição. " +
                                "Remova o produto " + prod.CodInterno + " - " + prod.DescrProduto + ", deste pedido, e tente novamente.");
                        if (dicProdQtdeOrig[prod.IdProd] > prod.Qtde)
                            throw new Exception("Algum(ns) produto(s) está(ão) com a quantidade maior que a quantidade inserida no pedido" +
                                " original. Verifique os produtos e tente novamente.");
                    }
                }

                #endregion

                // Confirma o pedido
                ConfirmaGarantiaReposicao(session, pedido.IdPedido, financeiro);
            }
            else if (pedido.TipoVenda == (int)Pedido.TipoVendaPedido.Garantia)
            {
                if (UserInfo.GetUserInfo.TipoUsuario == (int)Utils.TipoFuncionario.Vendedor &&
                    !Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoGarantiaReposicao))
                    throw new Exception("Apenas o gerente pode emitir pedido de garantia.");

                // Confirma o pedido
                ConfirmaGarantiaReposicao(session, pedido.IdPedido, financeiro);
            }
            /*
            else if (pedido.TipoVenda == (int)Pedido.TipoVendaPedido.Obra)
            {
                Obra obra = ObraDAO.Instance.GetElementByPrimaryKey(pedido.IdObra.Value);

                if (obra.Saldo < pedido.Total)
                    throw new Exception("O valor do pedido é maior que o saldo da obra.");
            }
            */

            /* Chamado 22658. */
            if (pedido.TipoVenda == (int)Pedido.TipoVendaPedido.Obra)
            {
                if (pedido.Total != pedido.ValorPagamentoAntecipado)
                    throw new Exception("O valor do pagamento antecipado do pedido difere do total do mesmo." +
                        " Recalcule o pedido para que os valores fiquem corretos.");
            }

            // Salva a data e usuário de finalização
            var usuConf = UserInfo.GetUserInfo.CodUser;

            if (financeiro)
                usuConf = ObtemValorCampo<uint>(session, "idFuncFinalizarFinanc", "idPedido=" + idPedido);

            objPersistence.ExecuteCommand(session, "update pedido set dataFin=?data, usuFin=?usu where idPedido=" + idPedido,
                new GDAParameter("?data", DateTime.Now), new GDAParameter("?usu", usuConf));

            PedidoComissaoDAO.Instance.Create(session, pedido);

            //Movimenta o estoque da materia-prima para os produtos que forem vidro
            foreach (var p in lstProd)
            {
                if (ProdutoDAO.Instance.IsVidro(session, (int)p.IdProd))
                    MovMateriaPrimaDAO.Instance.MovimentaMateriaPrimaPedido(session, (int)p.IdProdPed, (decimal)p.TotM, MovEstoque.TipoMovEnum.Saida);
            }

            LogAlteracaoDAO.Instance.LogPedido(session, pedido, GetElementByPrimaryKey(session, pedido.IdPedido), LogAlteracaoDAO.SequenciaObjeto.Atual);
        }

        /// <summary>
        /// Verifica se os produtos são do mesmo tipo do pedido
        /// </summary>
        private void ValidaTipoPedidoTipoProduto(GDASession sessao, Pedido pedido, ProdutosPedido[] lstProd)
        {
            // Verifica se o pedido tem itens que não são permitidos pelo seu tipo
            if (PedidoConfig.DadosPedido.BloquearItensTipoPedido && lstProd != null)
            {
                var subGrupos = SubgrupoProdDAO.Instance.ObtemSubgrupos(sessao, lstProd.Where(f => f.IdSubgrupoProd > 0).Select(f => (int)f.IdSubgrupoProd).ToList()).Distinct();

                foreach (var p in lstProd.Where(f => f.IdProdPedParent.GetValueOrDefault() == 0).ToList())
                {
                    //Verifica se o produto é uma embalagem (Item de revenda que pode ser incluído em pedido de venda)
                    if (p.IdSubgrupoProd == 0 || subGrupos.Any(f => f.IdSubgrupoProd == p.IdSubgrupoProd && !f.PermitirItemRevendaNaVenda))
                    {
                        if (pedido.TipoPedido == (int)Pedido.TipoPedidoEnum.Venda &&
                            (p.IdGrupoProd != (uint)NomeGrupoProd.Vidro ||
                            (p.IdGrupoProd == (uint)NomeGrupoProd.Vidro && SubgrupoProdDAO.Instance.IsSubgrupoProducao(sessao, (int)p.IdGrupoProd, (int)p.IdSubgrupoProd))) &&
                            p.IdGrupoProd != (uint)NomeGrupoProd.MaoDeObra && p.IdProdPedParent.GetValueOrDefault(0) == 0)
                            throw new Exception("Não é possível incluir produtos de revenda em um pedido de venda.");

                        if (pedido.TipoPedido == (int)Pedido.TipoPedidoEnum.Revenda &&
                            ((p.IdGrupoProd == (uint)NomeGrupoProd.Vidro && !SubgrupoProdDAO.Instance.IsSubgrupoProducao(sessao, (int)p.IdGrupoProd, (int)p.IdSubgrupoProd)) ||
                            p.IdGrupoProd == (uint)NomeGrupoProd.MaoDeObra))
                            throw new Exception("Não é possível incluir produtos de venda em um pedido de revenda.");

                        // Impede que o pedido seja finalizado caso tenham sido inseridos produtos de cor e espessura diferentes.
                        if ((PedidoConfig.DadosPedido.BloquearItensCorEspessura && !LojaDAO.Instance.GetIgnorarBloquearItensCorEspessura(null, pedido.IdLoja)) &&
                            (pedido.TipoPedido == (int)Pedido.TipoPedidoEnum.Venda || pedido.TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObraEspecial))
                            if ((ProdutoDAO.Instance.ObtemIdCorVidro(sessao, (int)lstProd.FirstOrDefault(f => f.IdProdPedParent.GetValueOrDefault(0) == 0)?.IdProd) != ProdutoDAO.Instance.ObtemIdCorVidro(sessao, (int)p.IdProd) ||
                                ProdutoDAO.Instance.ObtemEspessura(sessao, (int)lstProd.FirstOrDefault(f => f.IdProdPedParent.GetValueOrDefault(0) == 0)?.IdProd) != ProdutoDAO.Instance.ObtemEspessura(sessao, (int)p.IdProd))
                                && p.IdProdPedParent.GetValueOrDefault() == 0)
                                throw new Exception("Todos produtos devem ter a mesma cor e espessura.");
                    }
                }
            }
        }

        public void VerificaLimite(Pedido pedido, bool finalizarPedido)
        {
            VerificaLimite(null, pedido, finalizarPedido);
        }

        public void VerificaLimite(GDASession session, Pedido pedido, bool finalizarPedido)
        {
            var limite = ClienteDAO.Instance.ObtemValorCampo<decimal>(session, "limite", "id_Cli=" + pedido.IdCli);
            var debitos = ContasReceberDAO.Instance.GetDebitos(session, pedido.IdCli, pedido.IdPedido.ToString());
            var totalJaPagoPedido = pedido.IdPagamentoAntecipado > 0 ? pedido.ValorPagamentoAntecipado : 0;
            totalJaPagoPedido += pedido.IdSinal > 0 ? pedido.ValorEntrada : 0;

            // Se o cliente possuir limite configurado e se o total de débitos mais o valor não pago do pedido ultrapassar esse limite,
            // lança excessão, não permitindo que o pedido seja finalizado/confirmado, a menos que o mesmo tenha sido pago 100%
            if (limite > 0 && (debitos + pedido.Total - totalJaPagoPedido > limite) && (pedido.Total - totalJaPagoPedido > 0))
            {
                var limiteDisp = limite - debitos;

                var mensagem =
                    string.Format(
                        @"O cliente não possui limite disponível para realizar esta compra. Contate o setor Financeiro.\n
                        Limite total: {0} Limite disponível: {1}
                        \nDébitos: {2}", limite.ToString("C"),
                        (limiteDisp > 0 ? limiteDisp : 0).ToString("C"), (debitos + pedido.Total - totalJaPagoPedido).ToString("C"));

                LancarExceptionValidacaoPedidoFinanceiro(mensagem,
                    pedido.IdPedido, finalizarPedido, null,
                    finalizarPedido ? ObservacaoFinalizacaoFinanceiro.MotivoEnum.Finalizacao :
                    ObservacaoFinalizacaoFinanceiro.MotivoEnum.Confirmacao);
            }
        }

        #endregion
    }
}
