using System;
using System.Collections.Generic;
using System.Globalization;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Linq;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class ContasReceberDAO : BaseCadastroDAO<ContasReceber, ContasReceberDAO>
    {
        //private ContasReceberDAO() { }

        private static readonly object _quitarParcCartaoLock = new object();
        private static readonly object _cancelarRecebimentorParcCartaoLock = new object();

        #region Enumerators

        public enum TipoDebito
        {
            Todos,
            ChequesTotal,
            ContasAReceberTotal,
            ContasAReceberAntecipadas,
            PedidosEmAberto,
            ChequesEmAberto,
            ChequesDevolvidos,
            ChequesProtestados,
        }

        #endregion

        #region Busca Contas a Receber

        internal string SqlBuscarNF(string aliasContasReceber, bool selecionar, uint numeroNFe,
            bool apenasContasNf, bool forcarVirgula)
        {
            return SqlBuscarNF(null, aliasContasReceber, selecionar, numeroNFe, apenasContasNf, forcarVirgula);
        }

        internal string SqlBuscarNF(GDASession session, string aliasContasReceber, bool selecionar, uint numeroNFe,
            bool apenasContasNf, bool forcarVirgula)
        {
            string campo;
            return SqlBuscarNF(session, aliasContasReceber, selecionar, numeroNFe, apenasContasNf, forcarVirgula, false, out campo);
        }

        internal string SqlBuscarNF(string aliasContasReceber, bool selecionar, uint numeroNFe,
            bool apenasContasNf, bool forcarVirgula, bool usarJoin, out string campoContasReceber)
        {
            return SqlBuscarNF(aliasContasReceber, selecionar, numeroNFe, apenasContasNf, forcarVirgula,
                usarJoin, 0, null, out campoContasReceber);
        }

        internal string SqlBuscarNF(string aliasContasReceber, bool selecionar, uint numeroNFe, bool apenasContasNf,
            bool forcarVirgula, bool usarJoin, int idLoja, string modelo, out string campoContasReceber)
        {
            return SqlBuscarNF(null, aliasContasReceber, selecionar, numeroNFe, apenasContasNf,
                forcarVirgula, usarJoin, idLoja, modelo, out campoContasReceber);
        }

        internal string SqlBuscarNF(GDASession session, string aliasContasReceber, bool selecionar, uint numeroNFe,
            bool apenasContasNf, bool forcarVirgula, bool usarJoin, out string campoContasReceber)
        {
            return SqlBuscarNF(session, aliasContasReceber, selecionar, numeroNFe, apenasContasNf, forcarVirgula,
                usarJoin, 0, null, out campoContasReceber);
        }

        internal string SqlBuscarNF(GDASession session, string aliasContasReceber, bool selecionar, uint numeroNFe,
            bool apenasContasNf, bool forcarVirgula, bool usarJoin, int idLoja, string modelo, out string campoContasReceber)
        {
            campoContasReceber = String.Empty;

            // Busca o numeroNFe
            if (selecionar || numeroNFe > 0 || apenasContasNf)
            {
                string campo, where;
                string retorno = @"Select {1} Cast(group_concat(distinct numeroNFe separator ', ') as char) as numeroNfe
                    From nota_fiscal nf inner join pedidos_nota_fiscal pnf on (nf.idNf=pnf.idNf) ";

                if (FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber)
                {
                    campo = "nf.idNf";
                    campoContasReceber = "idNf";
                    where = "Where 1" + (!usarJoin ? " and nf.idNf={0}.idNf" : "");
                }
                else if (!PedidoConfig.LiberarPedido)
                {
                    campo = "pnf.idPedido";
                    campoContasReceber = "idPedido";
                    where = "Where 1" + (!usarJoin ? " and pnf.idPedido={0}.idPedido" : "");
                }
                else
                {
                    retorno += "inner join produtos_liberar_pedido plp on (pnf.idPedido=plp.idPedido)";
                    campo = "plp.idLiberarPedido";
                    campoContasReceber = "idLiberarPedido";
                    where = "Where 1" + (!usarJoin ? " AND nf.situacao <> " + (int)NotaFiscal.SituacaoEnum.Cancelada + " and plp.idLiberarPedido={0}.idLiberarPedido" : "");
                }

                if (usarJoin && numeroNFe > 0)
                {
                    var idsNf = string.Join(",", NotaFiscalDAO.Instance.ExecuteMultipleScalar<string>(session,
                        string.Format("Select Cast(idNf as char) From nota_fiscal Where numeroNfe={0}{1}{2} AND situacao <> " + (int)NotaFiscal.SituacaoEnum.Cancelada, numeroNFe,
                            idLoja > 0 ? " AND IdLoja=" + idLoja : string.Empty, !string.IsNullOrEmpty(modelo) ? " AND modelo=?modelo" : string.Empty),
                        new GDAParameter("?modelo", modelo)));
                    where += " And nf.idNf In (" + (string.IsNullOrEmpty(idsNf) ? "0" : idsNf) + ")";
                }

                retorno += where + (!usarJoin ? "" : " group  by " + campo);
                retorno = String.Format(retorno, aliasContasReceber, usarJoin ? campo + " as " + campoContasReceber + "," : String.Empty);

                if (!usarJoin)
                    return (!selecionar && (numeroNFe > 0 || apenasContasNf) && !forcarVirgula ? "(" : ", (") +
                        retorno + ") as NumeroNFe";
                else
                    return retorno;
            }
            else
                return "";
        }

        /// <summary>
        /// Recupera contas receber referentes ao cartão nao identificado
        /// </summary>
        public IList<ContasReceber> RecuperarContaspeloIdCartaoNaoIdentificado(GDASession sessao, int idCartaoNaoIdentificado)
        {
            return objPersistence.LoadData(sessao, "SELECT * FROM contas_receber WHERE IdCartaoNaoIdentificado="+ idCartaoNaoIdentificado).ToList();
        }

        internal string SqlCampoDescricaoContaContabil(string aliasContasReceber)
        {
            return String.Format("concat(if(({0}.tipoConta & {1})={1}, '{2}', " +
                "if(({0}.tipoConta & {5})={5}, '{6}', '{3}')), " +
                "if(({0}.tipoConta & {4})={4}, ', Reposição', ''))",

                aliasContasReceber,
                (byte)ContasReceber.TipoContaEnum.Contabil,
                FinanceiroConfig.ContasPagarReceber.DescricaoContaContabil,
                FinanceiroConfig.ContasPagarReceber.DescricaoContaNaoContabil,
                (byte)ContasReceber.TipoContaEnum.Reposicao,
                (byte)ContasReceber.TipoContaEnum.CupomFiscal,
                FinanceiroConfig.ContasPagarReceber.DescricaoContaCupomFiscal);
        }

        private string FiltroTipoConta(string aliasContasReceber, string tipoContasBuscar, out string criterio)
        {
            string retorno = String.Empty;
            criterio = String.Empty;

            if (!String.IsNullOrEmpty(tipoContasBuscar))
            {
                var contas = ObtemTiposContas();
                var ft = new List<string>();
                var c = new List<string>();

                foreach (var t in tipoContasBuscar.Split(','))
                    if (!String.IsNullOrEmpty(t))
                    {
                        ft.Add(String.Format("({0}.tipoConta & {1})={1}", "{0}", t));
                        c.Add((contas.FirstOrDefault(x => x.Id.ToString() == t) ?? new GenericModel(null, String.Empty)).Descr);
                    }

                retorno = String.Format(" and ({0})", String.Join(" or ", ft.ToArray()));
                criterio = "Tipos de Conta: " + String.Join(", ", c.Where(x => !String.IsNullOrEmpty(x)).ToArray()) + "    ";
            }

            return string.Format(retorno, aliasContasReceber);
        }

        private string SqlAReceber(uint idContaR, uint idPedido, uint idLiberarPedido, uint idAcerto, uint idTrocaDevolucao, uint numeroNFe,
            uint idLoja, bool lojaCliente, uint idCli, uint idFunc, uint tipoEntrega, string nomeCli, uint idContaBanco, string dtIni, string dtFim,
            string dtIniLib, string dtFimLib, string dataCadIni, string dataCadFim, string dtIniAntecip, string dtFimAntecip,
            string sitAntecip, Single precoInicial, Single precoFinal, bool returnAll, bool exibirContasVinculadas, uint idFormaPagto, int situacaoPedido,
            uint filtroContasAntecipadas, bool simples, bool incluirParcCartao, int contasRenegociadas, bool apenasNf, string agrupar,
            int contasCnab, string idsRotas, string obs, string tipoContasBuscar, string tipoContaContabil, bool selecionar, bool buscarContasValorZerado,
            uint numArqRemessa, bool refObra, int protestadas, uint idContaBancoCnab, int numCte, out bool temFiltro, out string filtroAdicional)
        {
            return SqlAReceber(idContaR, idPedido, idLiberarPedido, idAcerto, idTrocaDevolucao, numeroNFe,
                idLoja, lojaCliente, idCli, idFunc, tipoEntrega, nomeCli, idContaBanco, dtIni, dtFim,
                dtIniLib, dtFimLib, dataCadIni, dataCadFim, dtIniAntecip, dtFimAntecip,
                sitAntecip, precoInicial, precoFinal, returnAll, exibirContasVinculadas, new[] { idFormaPagto }, situacaoPedido,
                filtroContasAntecipadas, simples, incluirParcCartao, contasRenegociadas, apenasNf, agrupar,
                contasCnab, idsRotas, obs, tipoContasBuscar, tipoContaContabil, selecionar, buscarContasValorZerado,
                numArqRemessa, refObra, protestadas, idContaBancoCnab, numCte, out temFiltro, out filtroAdicional);
        }

        private string SqlAReceber(uint idContaR, uint idPedido, uint idLiberarPedido, uint idAcerto, uint idTrocaDevolucao, uint numeroNFe,
            uint idLoja, bool lojaCliente, uint idCli, uint idFunc, uint tipoEntrega, string nomeCli, uint idContaBanco, string dtIni, string dtFim,
            string dtIniLib, string dtFimLib, string dataCadIni, string dataCadFim, string dtIniAntecip, string dtFimAntecip,
            string sitAntecip, Single precoInicial, Single precoFinal, bool returnAll, bool exibirContasVinculadas, uint[] idsFormaPagto, int situacaoPedido,
            uint filtroContasAntecipadas, bool simples, bool incluirParcCartao, int contasRenegociadas, bool apenasNf, string agrupar,
            int contasCnab, string idsRotas, string obs, string tipoContasBuscar, string tipoContaContabil, bool selecionar, bool buscarContasValorZerado,
            uint numArqRemessa, bool refObra, int protestadas, uint idContaBancoCnab, int numCte, out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = false;
            filtroAdicional = !buscarContasValorZerado ? " and valorVec>0" : "";

            bool having = numeroNFe > 0 || apenasNf || (!String.IsNullOrEmpty(tipoContasBuscar) && tipoContasBuscar != "1,2,3");

            string campoTipoContaContabil = SqlCampoDescricaoContaContabil("c");
            string campos = selecionar ? @"c.*, cli.Nome as NomeCli, cli.Credito as CreditoCliente, a.data as DataAntecip,
                Coalesce(Tel_Cont, Tel_Cel, Tel_Res) as telCliente, pl.Descricao as DescrPlanoConta, '$$$' as criterio," +
                campoTipoContaContabil + "  as descricaoContaContabil, cli.limite as limiteCliente" :
                "Count(*) as contagem" + (having ? ", c.id" + (PedidoConfig.LiberarPedido ? "Liberar" : "") + "Pedido" : "");

            if (FinanceiroConfig.FinanceiroRec.ExibirCnab && selecionar)
                campos += ", (COALESCE(rar.protestado, FALSE) OR COALESCE(c.Juridico, FALSE)) as protestado, rar.idContaBanco as idContaBancoCnab";

            // Se for agrupar por comissionado, busca o nome dos mesmos
            if (("," + agrupar + ",").Contains(",4,"))
                campos += ", Coalesce(com.Nome, 'Sem comissionado') as NomeComissionado";

            // Busca pedidos da liberação
            if (selecionar && Glass.Configuracoes.PedidoConfig.LiberarPedido)
            {
                var codCliente = FinanceiroConfig.RelatorioContasReceber.ExibirPedCli ?
                    "COALESCE(concat(cast(ped.idPedido as char), ' (',ped.codCliente, ')'),ped.idPedido)" : "ped.idPedido";

                campos += @", lp.dataLiberacao, (
                    select Cast(group_concat(distinct "+ codCliente + @" separator ', ') as char)
                    from produtos_liberar_pedido plp
                    inner join pedido ped on (plp.idpedido = ped.idpedido)
                    where plp.idliberarpedido=c.idliberarpedido

                    /* Este Group By e Order By devem ter 2 espaços entre as duas palavras, pois, ao utilizar o método LoadDataWithSortExpression
                       é verificado a primeira ocorrência de 'Group By' e é inserido o Where antes desta ocorrência */

                    group  by plp.idLiberarPedido
                    order  by plp.idPedido) as pedidosLiberacao";

                    campos += @", if(cli.percReducaoNfe < 100 and c.idLiberarPedido is not null, (
                        select count(*) from pedidos_nota_fiscal pnf
                        inner join nota_fiscal nf on (pnf.idNf=nf.idNf)
                        where pnf.idLiberarPedido=c.idLiberarPedido
                        and nf.situacao not in (" + (int)NotaFiscal.SituacaoEnum.Cancelada + "," + (int)NotaFiscal.SituacaoEnum.Denegada + "," +
                        (int)NotaFiscal.SituacaoEnum.Inutilizada + "))=0, null) as liberacaoNaoPossuiNotaFiscalGerada";
            }

            else if (selecionar && !PedidoConfig.LiberarPedido)
            {
                campos += @", if(cli.percReducaoNfe < 100 and c.idPedido is not null, (
                        select count(*) from pedidos_nota_fiscal pnf
                        inner join nota_fiscal nf on (pnf.idNf=nf.idNf)
                        where pnf.idPedido=c.idPedido
                        and nf.situacao not in (" + (int)NotaFiscal.SituacaoEnum.Cancelada + "," + (int)NotaFiscal.SituacaoEnum.Denegada + "," +
                        (int)NotaFiscal.SituacaoEnum.Inutilizada + "))=0, null) as pedidoNaoPossuiNotaFiscalGerada";
            }

            campos += SqlBuscarNF("c", selecionar, numeroNFe, apenasNf, true);

            string criterio = String.Empty;
            string sql = "Select " + campos + " From contas_receber c ";

            if (!PedidoConfig.LiberarPedido && (tipoEntrega > 0 || idLoja > 0 || situacaoPedido > 0 || idFunc > 0 || ("," + agrupar + ",").Contains(",4,")))
                sql += "Left Join pedido p On (c.IdPedido=p.idPedido) ";

            if (PedidoConfig.LiberarPedido)
                sql += @"Left Join liberarpedido lp On (c.idLiberarPedido=lp.idLiberarPedido) ";

            if (("," + agrupar + ",").Contains(",4,"))
            {
                if (PedidoConfig.LiberarPedido)
                    sql += @"
                        Left Join produtos_liberar_pedido plp On (c.idLiberarPedido=plp.idLiberarPedido)
                        Left Join pedido p On (plp.idPedido=p.idPedido)
                        Left Join comissionado com On (p.idComissionado=com.idComissionado)";
                else
                    sql += "Left Join comissionado com On (p.idComissionado=com.idComissionado)";
            }

            if (FinanceiroConfig.FinanceiroRec.ExibirCnab)
            {
                sql += @"
                    LEFT JOIN
                    (
                        SELECT rar.idContaR, rar.idContaBanco, max(rar.protestado) AS protestado
                        FROM registro_arquivo_remessa rar
                            INNER JOIN arquivo_remessa ar ON (rar.idArquivoRemessa = ar.idArquivoRemessa)
                        WHERE ar.situacao <> " + (int)ArquivoRemessa.SituacaoEnum.Cancelado + @"
                        GROUP BY idContaR
                    ) as rar ON (rar.idContaR = c.idContaR)";
            }

            sql += @"
                Left Join cliente cli On (c.idCliente=cli.id_Cli)
                Left Join plano_contas pl On (c.IdConta=pl.IdConta)
                Left Join antecip_conta_rec a On (c.idAntecipContaRec=a.idAntecipContaRec)
                Where 1 ?filtroAdicional?";

            if (!incluirParcCartao)
                filtroAdicional += " And (c.isParcelaCartao=false or c.isParcelaCartao is null)";
            else
                criterio += "Incluir parcelas de cartão    ";

            if (contasCnab > 0)
            {
                switch (contasCnab)
                {
                    case 1:
                        filtroAdicional += " And numArquivoRemessaCnab Is Null";
                        criterio += "Não incluir contas de arquivo de remessa    ";
                        break;
                    case 2:
                        criterio += "Incluir contas de arquivo de remessa    ";
                        break;
                    case 3:
                        filtroAdicional += " And numArquivoRemessaCnab Is Not Null";
                        criterio += "Somente contas de arquivo de remessa    ";
                        break;
                }
            }

            if (FinanceiroConfig.FinanceiroRec.ExibirCnab)
            {
                if(protestadas == 1)
                {
                    filtroAdicional += " AND (COALESCE(rar.protestado, 0) = 1 OR COALESCE(c.Juridico, 0) = 1)";
                    criterio += "Somente contas em jurídico/cartório  ";
                }
                else if(protestadas == 2)
                {
                    filtroAdicional += " AND COALESCE(rar.protestado, 0) = 0 AND COALESCE(c.Juridico, 0) = 0";
                    criterio += "Não incluir contas em jurídico/cartório  ";
                }
            }

            if (idContaBancoCnab > 0)
            {
                filtroAdicional += " AND rar.idContaBanco = "+idContaBancoCnab;
                criterio += "Banco do Arquivo Remessa: " + ContaBancoDAO.Instance.ObtemNome(idContaBancoCnab) + " ";
            }

            if (contasRenegociadas > 0)
            {
                switch (contasRenegociadas)
                {
                    case 1:
                        criterio += "Incluir contas renegociadas    ";
                        break;
                    case 2:
                        filtroAdicional += " AND c.DataPrimNeg IS NOT NULL";
                        criterio += "Apenas contas renegociadas    ";
                        break;
                    case 3:
                        filtroAdicional += " AND c.DataPrimNeg IS NULL";
                        criterio += "Não incluir contas renegociadas    ";
                        break;
                }
            }

            //0 - Exceto Contas Antecipadas
            //1 - Incluir Contas Antecipadas
            //2 - Apenas Contas Antecipadas
            if (filtroContasAntecipadas == 0)
                filtroAdicional += " And c.idAntecipContaRec Is Null";
            else if (filtroContasAntecipadas == 1)
            {
                filtroAdicional += (sitAntecip == "" || sitAntecip == "0" ? " And recebida=false" : "");
                criterio += "Incluir contas antecipadas    ";
            }
            else if (filtroContasAntecipadas == 2)
            {
                filtroAdicional += " And c.idAntecipContaRec Is Not Null " + (String.IsNullOrEmpty(sitAntecip) ? " And recebida=false" : "");
                criterio += "Apenas contas antecipadas    ";
            }

            string where = String.Empty;

            if (idContaR > 0)
            {
                filtroAdicional += " AND c.idContaR=" + idContaR;
                criterio += "Conta à receber: " + idContaR;
            }

            if (idPedido > 0)
            {
                filtroAdicional += !PedidoConfig.LiberarPedido ? " And c.IdPedido=" + idPedido :
                    " And (c.IdPedido=" + idPedido + " Or c.IdLiberarPedido in (select distinct idLiberarPedido from produtos_liberar_pedido where idPedido=" + idPedido + @")
                    OR c.IdNf IN (SELECT DISTINCT idNf FROM pedidos_nota_fiscal WHERE idPedido=" + idPedido + "))";
                criterio += "Pedido: " + idPedido + "    ";
            }

            if (idLiberarPedido > 0)
            {
                if (FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber)
                    filtroAdicional += " AND (c.IdLiberarPedido=" + idLiberarPedido + @"
                        OR c.idNf IN (SELECT idNf
                                        FROM pedidos_nota_fiscal
                                        WHERE idLiberarPedido=" + idLiberarPedido + "))";
                else
                    filtroAdicional += " And c.IdLiberarPedido=" + idLiberarPedido;

                criterio += "Liberação: " + idLiberarPedido + "    ";
            }

            if (idAcerto > 0)
            {
                filtroAdicional += " and (c.idAcerto=" + idAcerto + " Or c.idAcertoParcial=" + idAcerto + ")";
                criterio += "Acerto: " + idAcerto + "    ";
            }

            if (idTrocaDevolucao > 0)
            {
                filtroAdicional += " And c.idTrocaDevolucao=" + idTrocaDevolucao;
                criterio += "Troca/Devolução: " + idTrocaDevolucao;
            }

            if (exibirContasVinculadas && idCli > 0)
            {
                filtroAdicional += " And (c.idCliente In (Select idClienteVinculo From cliente_vinculo Where idCliente=" + idCli + ") Or cli.id_Cli=" + idCli + ")";
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCli) + " e clientes vinculados    ";
            }
            else if (idCli > 0)
            {
                filtroAdicional += " And c.IdCliente=" + idCli;
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCli) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeCli))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCli, null, 0, null, null, null, null, 0);
                filtroAdicional += " And c.idCliente in (" + ids + ")";
                criterio += "Cliente: " + nomeCli + "    ";
            }

            if (!String.IsNullOrEmpty(idsRotas))
            {
                filtroAdicional += " And cli.id_cli in (Select idCliente From rota_cliente Where idRota IN(" + idsRotas + "))";
                criterio += "Rota: " + RotaDAO.Instance.ObtemValorCampo<string>("GROUP_CONCAT(codInterno)", "idRota IN(" + idsRotas + ")") + "    ";
            }

            if (!String.IsNullOrEmpty(dtIni))
            {
                filtroAdicional += " And DATAVEC>=?dtIni";
                criterio += "Data venc. início: " + dtIni + "    ";
            }

            if (!String.IsNullOrEmpty(dtFim))
            {
                filtroAdicional += " And DATAVEC<=?dtFim";
                criterio += "Data venc. fim: " + dtFim + "    ";
            }

            if (PedidoConfig.LiberarPedido)
            {
                // Filtro por troca/devolução feito para NRC
                if (!String.IsNullOrEmpty(dtIniLib))
                {
                    where += " and (lp.dataLiberacao>=?dtIniLib Or (c.idTrocaDevolucao > 0 and c.DataCad>=?dtIniLib))";
                    criterio += "Data lib. início: " + dtIniLib + "    ";
                    temFiltro = true;
                }

                if (!String.IsNullOrEmpty(dtFimLib))
                {
                    where += " and (lp.dataLiberacao<=?dtFimLib Or (c.idTrocaDevolucao > 0 and c.DataCad<=?dtFimLib))";
                    criterio += "Data lib. fim: " + dtFimLib + "    ";
                    temFiltro = true;
                }
            }

            if (!String.IsNullOrEmpty(dataCadIni))
            {
                filtroAdicional += " and c.DataCad>=?dataCadIni";
                criterio += "Período Cad.: " + (!String.IsNullOrEmpty(dataCadFim) ? "de " + dataCadIni : " a partir de " + dataCadIni + "    ");
            }

            if (!String.IsNullOrEmpty(dataCadFim))
            {
                filtroAdicional += " and c.DataCad<=?dataCadFim";
                criterio += (!String.IsNullOrEmpty(dataCadIni) ? " até " + dataCadFim : "Período Cad.: até " + dataCadFim) + "    ";
            }

            if (!String.IsNullOrEmpty(dtIniAntecip))
            {
                where += " And a.Data>=?dtIniAntecip";
                criterio += !String.IsNullOrEmpty(dtFimAntecip) ? "Período Antecip: " + dtIniAntecip + " a " + dtFimAntecip + "    " : "Data Antecip.: a partir de " + dtIniAntecip + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dtFimAntecip))
            {
                where += " And a.Data<=?dtFimAntecip";
                criterio += !String.IsNullOrEmpty(dtIniAntecip) ? "" : "Data Antecip: até " + dtFimAntecip + "    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(obs))
            {
                sql += " and c.obs like ?obs";
                criterio += "Obs.: " + obs + "    ";
                temFiltro = true;
            }

            if (idContaBanco > 0)
            {
                where += " And a.idContaBanco=" + idContaBanco;
                criterio += "Conta Bancária " + ContaBancoDAO.Instance.GetElement(idContaBanco).Descricao + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(sitAntecip) && sitAntecip != "0")
            {
                filtroAdicional += sitAntecip == "1" ? " And recebida=false" : " And recebida=true";
                criterio += sitAntecip == "1" ? "Apenas boletos não quitados    " : "Apenas boletos quitados    ";
            }
            else
                filtroAdicional += " And coalesce(recebida,false)=false";

            if (idLoja > 0)
            {
                if (!lojaCliente)
                    filtroAdicional += " And c.idLoja=" + idLoja;
                else
                {
                    sql += " And cli.id_Loja=" + idLoja;
                    temFiltro = true;
                }

                criterio += "Loja" + (lojaCliente ? " do cliente" : "") + ": " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (idFunc > 0)
            {
                if (PedidoConfig.LiberarPedido)
                    filtroAdicional += @" And c.idLiberarPedido In (Select idLiberarPedido From produtos_liberar_pedido Where idPedido In
                        (Select idPedido From pedido Where idFunc=" + idFunc + "))";
                else
                {
                    where += " And p.idFunc=" + idFunc;
                    temFiltro = true;
                }

                criterio += "Vendedor: " + FuncionarioDAO.Instance.GetNome(idFunc) + "    ";
            }

            if (tipoEntrega > 0)
            {
                if (PedidoConfig.LiberarPedido)
                    filtroAdicional += @" And c.idLiberarPedido In (Select idLiberarPedido From produtos_liberar_pedido Where idPedido In
                        (Select idPedido From pedido Where tipoEntrega=" + tipoEntrega + "))";
                else
                {
                    where += " And p.TipoEntrega=" + tipoEntrega;
                    temFiltro = true;
                }

                criterio += "Tipo Entrega: " + Utils.GetDescrTipoEntrega((int)tipoEntrega) + "    ";
            }

            if (precoInicial > 0)
            {
                filtroAdicional += " And c.valorVec>=" + precoInicial.ToString().Replace(',', '.');
                criterio += precoFinal > 0 ? "Valor Boleto: " + precoInicial + " até " + precoFinal + "    " : "Valor Boleto: a partir de " + precoInicial + "    ";
            }

            if (precoFinal > 0)
            {
                filtroAdicional += " And c.valorVec<=" + precoFinal.ToString().Replace(',', '.');
                criterio += precoInicial > 0 ? "" : "Valor Boleto: até " + precoFinal + "    ";
            }

            if (idsFormaPagto != null && idsFormaPagto.Any(f => f != 0))
            {
                if (idsFormaPagto.Contains((uint)Pagto.FormaPagto.Boleto))
                    filtroAdicional += " And c.idConta In (" + UtilsPlanoConta.ContasTodosTiposBoleto() + ")";
                else
                    filtroAdicional += " And c.idConta in (" + string.Join(", ", idsFormaPagto.Select(f => UtilsPlanoConta.ContasTodasPorTipo((Glass.Data.Model.Pagto.FormaPagto)f))) + ")";

                criterio += "Forma Pagto.: " + string.Join(", ", idsFormaPagto.Select(f => PagtoDAO.Instance.GetDescrFormaPagto(f)));
            }

            if (situacaoPedido > 0)
            {
                if (PedidoConfig.LiberarPedido)
                    filtroAdicional += @" And c.idLiberarPedido In (Select idLiberarPedido From produtos_liberar_pedido Where idPedido In
                        (Select idPedido From pedido Where situacaoProducao=" + situacaoPedido + "))";
                else
                {
                    where += " and p.situacaoProducao=" + situacaoPedido;
                    temFiltro = true;
                }

                foreach (GenericModel m in DataSources.Instance.GetSituacaoProducao())
                    if (m.Id == situacaoPedido)
                    {
                        criterio += "Situação do pedido: " + m.Descr + "    ";
                        break;
                    }
            }

            if (!String.IsNullOrEmpty(tipoContaContabil))
            {
                string c;
                filtroAdicional += FiltroTipoConta("c", tipoContaContabil, out c);
                criterio += c;
            }

            if (numArqRemessa > 0)
            {
                filtroAdicional += " And c.numArquivoRemessaCnab=" + numArqRemessa;
                criterio += "Núm. Arquivo Remessa: " + numArqRemessa + "    ";
            }

            if (!refObra)
            {
                filtroAdicional += " And c.IdObra IS NULL";
                criterio += "Sem referência para obra";
            }

            if (numCte > 0)
            {
                var idsCte = string.Join(",", Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.ObtemIdCteByNumero((uint)numCte).Select(f => f.ToString()).ToArray());

                if (String.IsNullOrEmpty(idsCte) || String.IsNullOrEmpty(idsCte.Trim(',')))
                    idsCte = "0";

                filtroAdicional += " AND c.idCte IN (" + idsCte + ")";
                criterio += " Num. CT-e:" + numCte + "    ";
            }

            // Se não for para retornar todos e nenhum filtro tiver sido especificado, não retorna nenhum registro
            if (!returnAll && where == String.Empty && filtroAdicional == String.Empty)
                where = " And 0>1";

            sql += where;

            if (having)
            {
                if (!selecionar)
                    sql += " group by " + (numeroNFe > 0 || apenasNf ? "c.id" + (PedidoConfig.LiberarPedido ? "Liberar" : "") + "Pedido" : "c.idContaR");

                sql += " having 1";

                if (numeroNFe > 0)
                {
                    sql += " and concat(',', numeroNFe, ',') like '%," + numeroNFe + ",%'";
                    criterio += "Número NFe: " + numeroNFe + "    ";
                }

                if (apenasNf)
                {
                    sql += " and length(coalesce(numeroNFe, '')) > 0";
                    criterio += "Apenas contas com núm. NF    ";
                }

                if (!String.IsNullOrEmpty(tipoContasBuscar) && tipoContasBuscar != "1,2,3" && PedidoConfig.LiberarPedido)
                {
                    List<string> itensBuscar = new List<string>(tipoContasBuscar.Split(','));

                    string filtroBuscar = "";

                    if (itensBuscar.Contains("1"))
                        filtroBuscar += " or !liberacaoNaoPossuiNotaFiscalGerada";

                    if (itensBuscar.Contains("2"))
                        filtroBuscar += " or liberacaoNaoPossuiNotaFiscalGerada";

                    if (itensBuscar.Contains("3"))
                        filtroBuscar += " or liberacaoNaoPossuiNotaFiscalGerada is null";

                    sql += " and (" + filtroBuscar.Substring(4) + ")";
                }

                else if (!String.IsNullOrEmpty(tipoContasBuscar) && tipoContasBuscar != "1,2,3" && !PedidoConfig.LiberarPedido)
                {
                    List<string> itensBuscar = new List<string>(tipoContasBuscar.Split(','));

                    string filtroBuscar = "";

                    if (itensBuscar.Contains("1"))
                        filtroBuscar += " or !pedidoNaoPossuiNotaFiscalGerada";

                    if (itensBuscar.Contains("2"))
                        filtroBuscar += " or pedidoNaoPossuiNotaFiscalGerada";

                    if (itensBuscar.Contains("3"))
                        filtroBuscar += " or pedidoNaoPossuiNotaFiscalGerada is null";

                    sql += " and (" + filtroBuscar.Substring(4) + ")";
                }

                if (!selecionar)
                    sql = "select coalesce(sum(contagem),0) from (" + sql + ") as temp";

                // Ignora a otimização de SQL se houver Having
                temFiltro = true;
            }

            return sql.Replace("$$$", criterio);
        }

        /// <summary>
        /// Busca as contas a receber que ainda não foram recebidas
        /// </summary>
        public IList<ContasReceber> GetNaoRecebidas(uint idContaR, uint idPedido, uint idLiberarPedido, uint idAcerto, uint idTrocaDevolucao,
            uint numeroNFe, uint idLoja, bool lojaCliente, uint idCli, uint idFunc, uint tipoEntrega, string nomeCli, string dtIni, string dtFim,
            string dtIniLib, string dtFimLib, string dataCadIni, string dataCadFim, float precoInicial, float precoFinal, uint[] idsFormaPagto,
            int situacaoPedido, bool incluirParcCartao, int contasRenegociadas, bool apenasNf, uint filtroContasAntecipadas, int sort, int contasCnab,
            string idsRotas, string obs, string tipoContasBuscar, string tipoContaContabil, uint numArqRemessa, bool refObra, int protestadas,
            uint idContaBanco, bool exibirContasVinculadas, int numCte, string sortExpression, int startRow, int pageSize)
        {
            if (String.IsNullOrEmpty(sortExpression))
                switch (sort)
                {
                    case 1: // Data Venc.
                        /* Chamado 15983.
                         * É necessário ordenar também pelo id da conta a receber para que as contas sejam exibidas corretamente na tela,
                         * para evitar problemas na exibição quando a ordenação escolhida trouxer registros iguais. */
                        //sortExpression = "c.DataVec Asc, c.IdContaR Asc"; break;
                        sortExpression = "c.DataVec Asc, c.IdContaR Asc"; break;
                    case 2: // Cliente
                        sortExpression = "cli.Nome asc, c.IdContaR Asc"; break;
                    case 3: // Valor
                        sortExpression = "c.ValorVec asc, c.IdContaR Asc"; break;
                    default:
                        sortExpression = "c.DataVec Desc, c.IdContaR Asc"; break;
                }

            bool temFiltro;
            string filtroAdicional;

            string sql = SqlAReceber(idContaR, idPedido, idLiberarPedido, idAcerto, idTrocaDevolucao, numeroNFe, idLoja, lojaCliente,
                idCli, idFunc, tipoEntrega, nomeCli, 0, dtIni, dtFim, dtIniLib, dtFimLib, dataCadIni, dataCadFim, null, null, "",
                precoInicial, precoFinal, true, exibirContasVinculadas, idsFormaPagto, situacaoPedido, filtroContasAntecipadas, true, incluirParcCartao,
                contasRenegociadas, apenasNf, "", contasCnab, idsRotas, obs, tipoContasBuscar, tipoContaContabil, true, false, numArqRemessa, refObra,
                protestadas, idContaBanco, numCte, out temFiltro, out filtroAdicional);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional,
                 GetParam(nomeCli, dtIni, dtFim, dtIniLib, dtFimLib, null, null, dataCadIni, dataCadFim, obs, null, null, null, null));
        }

        public int GetNaoRecebidasCount(uint idContaR, uint idPedido, uint idLiberarPedido, uint idAcerto, uint idTrocaDevolucao, uint numeroNFe,
            uint idLoja, bool lojaCliente, uint idCli, uint idFunc, uint tipoEntrega, string nomeCli, string dtIni, string dtFim, string dtIniLib,
            string dtFimLib, string dataCadIni, string dataCadFim, float precoInicial, float precoFinal, uint[] idsFormaPagto,
            int situacaoPedido, bool incluirParcCartao, int contasRenegociadas, bool apenasNf, uint filtroContasAntecipadas, int sort,
            int contasCnab, string idsRotas, string obs, string tipoContasBuscar, string tipoContaContabil, uint numArqRemessa, bool refObra,
            bool exibirContasVinculadas, int protestadas, uint idContaBanco, int numCte)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlAReceber(idContaR, idPedido, idLiberarPedido, idAcerto, idTrocaDevolucao, numeroNFe, idLoja, lojaCliente,
                idCli, idFunc, tipoEntrega, nomeCli, 0, dtIni, dtFim, dtIniLib, dtFimLib, dataCadIni, dataCadFim, null, null, "",
                precoInicial, precoFinal, true, exibirContasVinculadas, idsFormaPagto, situacaoPedido, filtroContasAntecipadas, true,
                incluirParcCartao, contasRenegociadas, apenasNf, "", contasCnab, idsRotas, obs, tipoContasBuscar, tipoContaContabil, true,
                false, numArqRemessa, refObra, protestadas, idContaBanco, numCte, out temFiltro, out filtroAdicional);

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, GetParam(nomeCli, dtIni, dtFim, dtIniLib, dtFimLib,
                 null, null, dataCadIni, dataCadFim, obs, null, null, null, null));
        }

        /// <summary>
        /// Busca as contas a receber que ainda não foram recebidas
        /// </summary>
        public IList<ContasReceber> GetNaoRecebidasRpt(uint idContaR, uint idPedido, uint idLiberarPedido, uint idAcerto, uint idTrocaDevolucao,
            uint numeroNFe, uint idLoja, bool lojaCliente, uint idCli, uint idFunc, uint tipoEntrega, string nomeCli, string dtIni, string dtFim,
            string dtIniLib, string dtFimLib, string dataCadIni, string dataCadFim, float precoInicial, float precoFinal, uint[] idsFormaPagto,
            int situacaoPedido, bool incluirParcCartao, int contasRenegociadas, bool apenasNf, uint filtroContasAntecipadas, string agrupar,
            int sort, int contasCnab, string idsRotas, string obs, string tipoContasBuscar, string tipoContaContabil, uint numArqRemessa,
            bool refObra, bool exibirContasVinculadas, int protestadas, uint idContaBanco, int numCte)
        {
            string sortExpression;
            switch (sort)
            {
                case 1: // Data Venc.
                    sortExpression = " order by DataVec Asc"; break;
                case 2: // Cliente
                    sortExpression = " order by cli.Nome asc"; break;
                case 3: // Valor
                    sortExpression = " order by ValorVec asc"; break;
                default:
                    sortExpression = " order by DataVec Desc"; break;
            }

            bool temFiltro;
            string filtroAdicional;

            string sql = SqlAReceber(idContaR, idPedido, idLiberarPedido, idAcerto, idTrocaDevolucao, numeroNFe, idLoja, lojaCliente,
                idCli, idFunc, tipoEntrega, nomeCli, 0, dtIni, dtFim, dtIniLib, dtFimLib, dataCadIni, dataCadFim, null, null, "",
                precoInicial, precoFinal, true, exibirContasVinculadas, idsFormaPagto, situacaoPedido, filtroContasAntecipadas, true,
                incluirParcCartao, contasRenegociadas, apenasNf, agrupar, contasCnab, idsRotas, obs, tipoContasBuscar, tipoContaContabil,
                true, false, numArqRemessa, refObra, protestadas, idContaBanco, numCte, out temFiltro, out filtroAdicional).
                Replace(FILTRO_ADICIONAL, filtroAdicional);

            return objPersistence.LoadData(sql + sortExpression,
                GetParam(nomeCli, dtIni, dtFim, dtIniLib, dtFimLib, null, null, dataCadIni, dataCadFim, obs, null, null, null, null)).ToList();
        }

        /// <summary>
        /// Busca contas antecipadas, recebidas ou não
        /// </summary>
        public IList<ContasReceber> GetAntecip(uint idCli, string nomeCli, string dtIniAntecip, string dtFimAntecip, int sitAntecip,
            uint idContaBanco, float valorInicial, float valorFinal, string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? "c.DataVec Asc" : sortExpression;

            bool temFiltro;
            string filtroAdicional;

            string sql = SqlAReceber(0, 0, 0, 0, 0, 0, 0, false, idCli, 0, 0, nomeCli, idContaBanco, null, null, null, null, null, null,
                dtIniAntecip, dtFimAntecip, sitAntecip.ToString(), valorInicial, valorFinal, true, false, 0, 0, 2, true, false, 0, false, "", 0, null, null, null, null,
                true, false, 0, true, 0, 0, 0, out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return LoadDataWithSortExpression(sql, sort, startRow, pageSize, temFiltro, filtroAdicional,
                GetParam(nomeCli, null, null, null, null, dtIniAntecip, dtFimAntecip, null, null, null, null, null, null, null));
        }

        public int GetAntecipCount(uint idCli, string nomeCli, string dtIniAntecip, string dtFimAntecip, int sitAntecip,
            uint idContaBanco, float valorInicial, float valorFinal)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlAReceber(0, 0, 0, 0, 0, 0, 0, false, idCli, 0, 0, nomeCli, idContaBanco, null, null, null, null, null, null,
                dtIniAntecip, dtFimAntecip, sitAntecip.ToString(), valorInicial, valorFinal, true, false, 0, 0, 2, true, false, 0, false, "", 0,
                null, null, null, null, true, false, 0, true, 0, 0, 0, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional,
                GetParam(nomeCli, null, null, null, null, dtIniAntecip, dtFimAntecip, null, null, null, null, null, null, null));
        }

        public IList<ContasReceber> GetAntecipForRpt(uint idCli, string nomeCli, string dtIniAntecip, string dtFimAntecip, int sitAntecip,
            uint idContaBanco, float valorInicial, float valorFinal)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlAReceber(0, 0, 0, 0, 0, 0, 0, false, idCli, 0, 0, nomeCli, idContaBanco, null, null, null, null, null, null,
                dtIniAntecip, dtFimAntecip, sitAntecip.ToString(), valorInicial, valorFinal, true, false, 0, 0, 2, true, false, 0, false, "", 0, null, null, null, null,
                true, false, 0, true, 0, 0, 0, out temFiltro, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional);

            return objPersistence.LoadData(sql + " Order By c.DataVec Asc",
                GetParam(nomeCli, null, null, null, null, dtIniAntecip, dtFimAntecip, null, null, null, null, null, null, null)).ToList();
        }

        /// <summary>
        /// Busca débitos do cliente
        /// </summary>
        /// <returns></returns>
        public IList<ContasReceber> GetDebitos(uint idPedido, uint idLiberarPedido, uint idCli, string nomeCli, string sortExpression, int startRow, int pageSize)
        {
            string filtro = String.IsNullOrEmpty(sortExpression) ? "c.DataVec Asc" : sortExpression;

            bool temFiltro;
            string filtroAdicional;

            string sql = SqlAReceber(0, idPedido, idLiberarPedido, 0, 0, 0, 0, false, idCli, 0, 0, nomeCli, 0, null, null,
                null, null, null, null, null, null, "", 0, 0, false, false, 0, 0, 0, false, false, 0, false, "", 2, null, null, null, null, true,
                false, 0, true, 0, 0, 0, out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return LoadDataWithSortExpression(sql, filtro, startRow, pageSize, temFiltro, filtroAdicional,
                GetParam(nomeCli, null, null, null, null, null, null, null, null, null, null, null, null, null));
        }

        public int GetDebitosCount(uint idPedido, uint idLiberarPedido, uint idCli, string nomeCli)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlAReceber(0, idPedido, idLiberarPedido, 0, 0, 0, 0, false, idCli, 0, 0, nomeCli, 0, null, null,
                null, null, null, null, null, null, "", 0, 0, false, false, 0, 0, 0, false, false, 0, false, "", 2, null, null, null, null,
                true, false, 0, true, 0, 0, 0, out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, GetParam(nomeCli, null, null, null, null, null, null, null, null, null, null, null, null, null));
        }

        /// <summary>
        /// Busca as contas a receber que ainda não foram recebidas para a tela de Efetuar Acerto
        /// </summary>
        /// <returns></returns>
        public IList<ContasReceber> GetForEfetuarAcerto(uint idPedido, uint idLiberarPedido, uint idAcerto, uint numeroNFe,
            uint idLoja, uint idCli, string nomeCli, string dtIni, string dtFim, uint idFormaPagto, bool contasVinculadas,
            string tipoContaContabil, bool naoBuscarReneg, int ordenar, bool buscarContasValorZerado, int numeroCTe,
            uint idTrocaDevolucao, string observacao, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;

            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : (ordenar == 2 ? "c.dataVec Asc, c.idContaR Asc" :
                ordenar == 1 ? "c.idCliente Asc, c.idContaR Asc" : ordenar == 3 ? "c.idPedido Asc, c.idContaR Asc" : "c.valorVec Asc, c.idContaR Asc");

            string sql = SqlAReceber(0, idPedido, idLiberarPedido, idAcerto, idTrocaDevolucao, numeroNFe, idLoja, false, idCli, 0, 0, nomeCli, 0, dtIni, dtFim,
                null, null, null, null, null, null, "", 0, 0, false, contasVinculadas, idFormaPagto, 0, 0, false, false, naoBuscarReneg ? 3 : 0,
                false, "", 0, null, observacao, null, tipoContaContabil, true, buscarContasValorZerado, 0, true, 0, 0, numeroCTe,
                out temFiltro, out filtroAdicional);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional,
                GetParam(nomeCli, dtIni, dtFim, null, null, null, null, null, null, observacao, tipoContaContabil, null, null, null));
        }

        /// <summary>
        /// Busca as contas a receber que ainda não foram recebidas para a tela de Efetuar Acerto
        /// </summary>
        /// <returns></returns>
        public int GetForEfetuarAcertoCount(uint idPedido, uint idLiberarPedido, uint idAcerto, uint numeroNFe,
            uint idLoja, uint idCli, string nomeCli, string dtIni, string dtFim, uint idFormaPagto, bool contasVinculadas,
            string tipoContaContabil, bool naoBuscarReneg, int ordenar, bool buscarContasValorZerado, int numeroCTe,
            uint idTrocaDevolucao, string observacao)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlAReceber(0, idPedido, idLiberarPedido, idAcerto, idTrocaDevolucao, numeroNFe, idLoja, false, idCli, 0, 0, nomeCli, 0, dtIni, dtFim,
                null, null, null, null, null, null, "", 0, 0, false, contasVinculadas, idFormaPagto, 0, 0, false, false, naoBuscarReneg ? 3 : 0,
                false, "", 0, null, observacao, null, tipoContaContabil, true, buscarContasValorZerado, 0, true, 0, 0, numeroCTe,
                out temFiltro, out filtroAdicional);

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional,
                GetParam(nomeCli, dtIni, dtFim, null, null, null, null, null, null, observacao, tipoContaContabil, null, null, null));
        }

        public ContasReceber GetByIdContaR(uint idContaR)
        {
            return GetByIdContaR(null, idContaR);
        }

        public ContasReceber GetByIdContaR(GDASession session, uint idContaR)
        {
            string sql = @"select c.*, cli.nome as nomeCli, f.nome as nomeFunc
                from contas_receber c
                    left join cliente cli on (c.idCliente=cli.id_Cli)
                    left join funcionario f on (c.usuRec=f.idFunc)
                where idContaR=" + idContaR;

            return objPersistence.LoadOneData(session, sql);
        }

        public ContasReceber GetElement(uint idContaR)
        {
            return GetElement(null, idContaR);
        }

        public ContasReceber GetElement(GDASession session, uint idContaR)
        {
            var retorno = GetByIdContaR(session, idContaR);
            var formaPagto = string.Empty;

            #region Pagto. Contas Receber

            var pagtosContasReceber = PagtoContasReceberDAO.Instance.ObtemPagtos(session, idContaR);

            if (pagtosContasReceber.Any(f => f.IdFormaPagto == 14))
            {
                string where = "idContaR=" + idContaR;

                var idsCNI = CartaoNaoIdentificadoDAO.Instance.GetIdsCartaoNaoIdentificado(session,
                    ObtemValorCampo<uint?>(session, "idPedido", where),
                    ObtemValorCampo<uint?>(session, "idLiberarPedido", where),
                    ObtemValorCampo<uint?>(session, "idAcerto", where),
                    idContaR, ObtemValorCampo<uint?>(session, "idObra", where),
                    ObtemValorCampo<uint?>(session, "idSinal", where),
                    ObtemValorCampo<uint?>(session, "idTrocaDevolucao", where), null,
                    ObtemValorCampo<uint?>(session, "idAcertoCheque", where));

                foreach (var idCNI in idsCNI.Split(','))
                    formaPagto += " Cartão R$" + CartaoNaoIdentificadoDAO.Instance.GetValorCartaoNaoIdentificado(session, Glass.Conversoes.StrParaUint(idCNI));
            }

            if (pagtosContasReceber != null)
            {
                formaPagto =
                    string.Join(", ", pagtosContasReceber.Select(f =>
                        // Descrição forma de pagamento.
                        FormaPagtoDAO.Instance.GetDescricao(session, f.IdFormaPagto) +
                        // Tipo de cartão.
                        (f.IdTipoCartao > 0 ? " " + TipoCartaoCreditoDAO.Instance.ObterDescricao(session, (int)f.IdTipoCartao) : string.Empty) +
                        // Número de parcelas
                        (f.IdFormaPagto == 5 && TipoCartaoCreditoDAO.Instance.ObterTipoCartao(session, (int)f.IdTipoCartao) == TipoCartaoEnum.Debito ? " " :
                        (f.IdFormaPagto == 5 && ObterNumParcMaxContaR(session, f.IdContaR) > 0 ? " " + ObterNumParcMaxContaR(session, f.IdContaR) + " parcela(s) " : string.Empty)) +
                        // Valor.
                        f.ValorPagto.ToString("C") +
                        // Descrição conta bancária.
                        (f.IdContaBanco > 0 ? " " + ContaBancoDAO.Instance.GetDescricao(session, (uint)f.IdContaBanco) : string.Empty)).ToList());

                if (formaPagto != null)
                {
                    if (retorno.IdSinal > 0)
                    {
                        var numParcMax = ObtemValorCampo<int>(session, "NumParcMax", $"IdContaRRef={ retorno.IdContaR }");
                        formaPagto += $" - { (numParcMax > 0 ? $" { numParcMax } parcela(s)" : string.Empty) }";
                    }

                    retorno.FormaPagto = formaPagto;

                    return retorno;
                }
            }

            #endregion

            var cxDiario = CaixaDiarioDAO.Instance.GetByContaRec(session, idContaR);
            var cxGeral = CaixaGeralDAO.Instance.GetByContaRec(session, idContaR);

            if (cxDiario.Length == 0 && cxGeral.Length == 0 && retorno.IdLiberarPedido > 0)
            {
                cxDiario = CaixaDiarioDAO.Instance.GetByLiberacao(session, retorno.IdLiberarPedido.Value);
                cxGeral = CaixaGeralDAO.Instance.GetByLiberacao(session, retorno.IdLiberarPedido.Value);
            }

            foreach (MovBanco mov in MovBancoDAO.Instance.GetByContaRec(session, idContaR))
            {
                if (mov.TipoMov == 2)
                    break;

                string descrMov = UtilsPlanoConta.GetDescrFormaPagtoByIdConta(mov.IdConta);
                var idTipoCartao = UtilsPlanoConta.ObterTipoCartaoPorConta(mov.IdConta);

                if (idTipoCartao > 0)
                    descrMov += " " + TipoCartaoCreditoDAO.Instance.ObterDescricao(session, (int)idTipoCartao);

                formaPagto += descrMov + " " + mov.ValorMov.ToString("C") + " " +
                    ContaBancoDAO.Instance.GetDescricao(session, mov.IdContaBanco) + ", ";
            }

            if (cxDiario.Length > 0)
                foreach (CaixaDiario cx in cxDiario)
                {
                    if (cx.TipoMov == 2)
                        continue;

                    string descrMov = UtilsPlanoConta.GetDescrFormaPagtoByIdConta(cx.IdConta);
                    var idTipoCartao = UtilsPlanoConta.ObterTipoCartaoPorConta(cx.IdConta);

                    if (idTipoCartao > 0)
                        descrMov += " " + TipoCartaoCreditoDAO.Instance.ObterDescricao(session, (int)idTipoCartao);

                    if (String.IsNullOrEmpty(descrMov))
                        descrMov = PlanoContasDAO.Instance.GetDescricao(session, cx.IdConta, false);

                    string formaPagtoStr = descrMov + " " + cx.Valor.ToString("C") + ", ";

                    if (!formaPagto.Contains(formaPagtoStr))
                        formaPagto += formaPagtoStr;
                }
            else
                foreach (CaixaGeral cx in cxGeral)
                {
                    if (cx.TipoMov == 2)
                        continue;

                    string descrMov = UtilsPlanoConta.GetDescrFormaPagtoByIdConta(cx.IdConta);
                    var idTipoCartao = UtilsPlanoConta.ObterTipoCartaoPorConta(cx.IdConta);

                    if (idTipoCartao > 0)
                        descrMov += " " + TipoCartaoCreditoDAO.Instance.ObterDescricao(session, (int)idTipoCartao);

                    if (String.IsNullOrEmpty(descrMov))
                        descrMov = PlanoContasDAO.Instance.GetDescricao(session, cx.IdConta, false);

                    string formaPagtoStr = descrMov + " " + cx.ValorMov.ToString("C") + ", ";

                    if (!formaPagto.Contains(formaPagtoStr.TrimEnd(',', ' ')))
                        formaPagto += formaPagtoStr;
                }

            if (retorno.IdSinal > 0)
                formaPagto = SinalDAO.Instance.ObtemFormaPagto(session, retorno.IdSinal.Value);

            retorno.FormaPagto = formaPagto;

            return retorno;
        }

        /// <summary>
        /// Busca contas a receber que possuírem os ids separados por "," passado no parâmetro
        /// </summary>
        public ContasReceber[] GetByPks(GDASession sessao, string pks)
        {
            return objPersistence.LoadData(sessao, "Select distinct c.* From contas_receber c Where c.IdContaR In (" + pks.TrimEnd(' ').TrimStart(' ').TrimStart(',').TrimEnd(',') + ")").ToList().ToArray();
        }

        /// <summary>
        /// Verifica se as contas a receber quitadas em um acerto pertencem ao mesmo pedido
        /// </summary>
        public bool ContasRecMesmoPedido(GDASession sessao, string pks)
        {
            return objPersistence.ExecuteSqlQueryCount(sessao, "Select Count(*) From (Select count(*) From contas_receber c Where c.IdContaR In (" +
                pks.TrimEnd(' ').TrimStart(' ').TrimStart(',').TrimEnd(',') + ") group by c.idPedido) as tbl", null) == 1;
        }

        #endregion

        #region Busca Contas A Receber/Recebidas para relatório

        private string SqlRpt(uint idPedido, uint idLiberarPedido, uint idAcerto, uint idAcertoParcial, uint idTrocaDevolucao, uint numeroNFe,
            uint idLoja, uint idFunc, uint idFuncRecebido, uint idCli, uint tipoEntrega, string nomeCli, string dtIniVenc, string dtFimVenc,
            string dtIniRec, string dtFimRec, string dataIniCad, string dataFimCad, string dtIniLib, string dtFimLib, string idsFormaPagto, uint idTipoBoleto, Single precoInicial,
            Single precoFinal, bool? recebida, uint idComissionado, uint idRota, string obs, int sort, bool? renegociadas, string tipoContaContabil,
            bool returnAll, uint numArqRemessa, bool refObra, int contasCnab, int idVendedorAssociado, int idVendedorObra, int idComissao, int idSinalPedido, int numCte,
            bool protestadas, bool contasVinculadas, string tipoContasBuscar, string numAutCartao, int? idContaBancoRecebimento, bool relatorio, bool selecionar, out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = false;
            filtroAdicional = " And c.valorVec > 0 AND (c.isParcelaCartao=false OR c.isParcelaCartao IS NULL)";

            bool having = numeroNFe > 0 || (!String.IsNullOrEmpty(tipoContasBuscar) && tipoContasBuscar != "1,2,3");

            if (recebida.GetValueOrDefault(false))
                filtroAdicional += " AND (c.Recebida = true)";
            else
            {
                if (recebida.HasValue)
                    filtroAdicional += " AND (c.Recebida = FALSE OR c.Recebida IS NULL)";

                filtroAdicional += " AND idAntecipContaRec IS NULL";
            }

            string criterio = String.Empty;
            string where = String.Empty;

            var campos = string.Empty;

            campos = selecionar ? @"c.idContaR, c.idCliente, c.idFormaPagto, c.idPedido, c.idAntecipContaRec, c.idConta, c.dataVec, c.DestinoRec,
                c.valorVec, c.dataRec, c.valorRec, c.juros, c.recebida, c.usuRec, c.idAcerto, c.numParc, c.desconto, c.motivoDescontoAcresc,
                c.idFuncDescAcresc, c.numAutConstrucard, c.dataDescAcresc, c.idLiberarPedido, c.idContaBanco, c.idAcertoParcial, c.obs,
                c.idObra, c.dataPrimNeg, cli.Nome as NomeCli, pl.Descricao as DescrPlanoConta, f.Nome as NomeFunc, '$$$' as Criterio,
                    c.NumParcMax, cli.Credito as CreditoCliente, c.renegociada, c.idTrocaDevolucao, c.dataCad, c.multa, c.idDevolucaoPagto,
                    c.isParcelaCartao, c.IdContaRCartao, c.acrescimo, c.valorJurosCartao, c.tipoRecebimentoParcCartao, c.idSinal, c.usuCad, c.idAcertoCheque,
                    c.idEncontroContas, c.idNf, c.numArquivoRemessaCnab, c.numeroDocumentoCnab, p.PercentualComissao,
                    cli.cpf_cnpj as CpfCnpjCliente, cli.rg_escInst as InscEstadualCliente, GROUP_CONCAT(COALESCE(fp.descricao, '')) as FormaPagto, GROUP_CONCAT(COALESCE(pcr.NumAutCartao, '')) as NumAutCartao, ccr.IdComissao," +
                    SqlCampoDescricaoContaContabil("c") + " as descricaoContaContabil" : "Count(*) as contagem" +
                    (numeroNFe > 0 ? ", c.id" + (PedidoConfig.LiberarPedido ? "Liberar" : "") + "Pedido" : "");

            if (FinanceiroConfig.FinanceiroRec.ExibirCnab && selecionar)
                campos += ", COALESCE(rar.protestado, 0) as protestado, rar.idContaBanco as idContaBancoCnab";

            // Busca pedidos da liberação
            if (selecionar && protestadas && FinanceiroConfig.RelatorioContasRecebidas.ExibirPedidos)
            {
                campos += @", (
                    select Cast(group_concat(distinct idPedido separator ', ') as char)
                    from produtos_liberar_pedido plp
                    where plp.idliberarpedido=c.idliberarpedido
                    group by idLiberarPedido
                    order by idLiberarPedido, idPedido) as pedidosLiberacao";
            }

            if (selecionar && PedidoConfig.LiberarPedido)
            {
                campos += @", if(cli.percReducaoNfe < 100 and c.idLiberarPedido is not null, (
                        select count(*) from pedidos_nota_fiscal pnf
                        inner join nota_fiscal nf on (pnf.idNf=nf.idNf)
                        where pnf.idLiberarPedido=c.idLiberarPedido
                        and nf.situacao not in (" + (int)NotaFiscal.SituacaoEnum.Cancelada + "," + (int)NotaFiscal.SituacaoEnum.Denegada + "," +
                        (int)NotaFiscal.SituacaoEnum.Inutilizada + "))=0, null) as liberacaoNaoPossuiNotaFiscalGerada";
            }

            if (selecionar && !PedidoConfig.LiberarPedido)
            {
                campos += @", if(cli.percReducaoNfe < 100 and c.idPedido is not null, (
                        select count(*) from pedidos_nota_fiscal pnf
                        inner join nota_fiscal nf on (pnf.idNf=nf.idNf)
                        where pnf.idPedido=c.idPedido
                        and nf.situacao not in (" + (int)NotaFiscal.SituacaoEnum.Cancelada + "," + (int)NotaFiscal.SituacaoEnum.Denegada + "," +
                        (int)NotaFiscal.SituacaoEnum.Inutilizada + "))=0, null) as pedidoNaoPossuiNotaFiscalGerada";
            }

            campos += SqlBuscarNF("c", selecionar, numeroNFe, false, true);

            string sql = @"
                Select " + campos + @" From contas_receber c
                    Left Join pedido p On (c.IdPedido=p.idPedido) ";

            if (PedidoConfig.LiberarPedido)
            {
                sql += "Left Join liberarpedido lp On (c.idLiberarPedido=lp.idLiberarPedido) ";

                if (idFunc > 0)
                    sql += @"
                        Left Join produtos_liberar_pedido plp On (lp.idLiberarPedido=plp.idLiberarPedido)
                        Left Join pedido plib On (plp.idPedido=plib.idPedido)";
            }

            if (FinanceiroConfig.FinanceiroRec.ExibirCnab)
            {
                sql += @"
                    LEFT JOIN
                    (
                        SELECT rar.idContaR, rar.idContaBanco, max(rar.protestado) AS protestado
                        FROM registro_arquivo_remessa rar
                            INNER JOIN arquivo_remessa ar ON (rar.idArquivoRemessa = ar.idArquivoRemessa)
                        WHERE ar.situacao <> " + (int)ArquivoRemessa.SituacaoEnum.Cancelado + @"
                        GROUP BY idContaR
                    ) as rar ON (rar.idContaR = c.idContaR)";
            }

            sql += @"
                    Left Join cliente cli On (c.idCliente=cli.id_Cli)
                    Left Join plano_contas pl On (c.IdConta=pl.IdConta)
                    Left Join funcionario f On (c.UsuRec=f.IdFunc)
                    LEFT JOIN comissao_contas_receber ccr ON (c.IdContaR = ccr.IdContaR)
                    LEFT JOIN pagto_contas_receber pcr ON (pcr.IdContaR = c.IdContaR)
                    LEFT JOIN formapagto fp ON (pcr.IdFormaPagto = fp.IdFormaPagto)
                Where 1 ?filtroAdicional?";

            if (idPedido > 0)
            {
                uint? idSinal = PedidoDAO.Instance.ObtemIdSinal(null, idPedido);
                uint? idPagamentoAntecipado = PedidoDAO.Instance.ObtemIdPagamentoAntecipado(null, idPedido);

                var filtro = "";

                var idsLiberacao = String.Join(",", LiberarPedidoDAO.Instance.GetIdsLiberacaoAtivaByPedido(idPedido));
                if (String.IsNullOrEmpty(idsLiberacao))
                    idsLiberacao = "0";

                if (recebida.GetValueOrDefault(true))
                {
                    if (PedidoConfig.LiberarPedido)
                    {
                        IList<string> lstIdAcerto = ExecuteMultipleScalar<string>(@"Select idAcerto From contas_receber Where idAcerto Is Not Null And
                            idLiberarPedido In (" + idsLiberacao + ")");

                        // Os parênteses abaixo devem ficar separados ") )" para que caso passe no TrimEnd(')') logo abaixo remova somente um deles
                        if (lstIdAcerto.Count > 0)
                            filtro = " And (c.idLiberarPedido In (" + idsLiberacao + ") Or c.idAcerto In (" + String.Join(",", lstIdAcerto.ToArray()) + ") )";
                    }
                    else
                    {
                        IList<string> lstIdAcerto = ExecuteMultipleScalar<string>(@"Select idAcerto From contas_receber Where idAcerto Is Not Null And
                            idPedido=" + idPedido);

                        // Os parênteses abaixo devem ficar separados ") )" para que caso passe no TrimEnd(')') logo abaixo remova somente um deles
                        if (lstIdAcerto.Count > 0)
                            filtro = " And (c.idPedido=" + idPedido + " Or c.idAcerto In (" + String.Join(",", lstIdAcerto.ToArray()) + ") )";
                    }
                }

                if (idSinal.GetValueOrDefault() > 0)
                    filtro = (string.IsNullOrEmpty(filtro) ? " And (" : filtro.TrimEnd(')') + " Or ") + "c.idSinal=" + idSinal + ")";

                if (idPagamentoAntecipado.GetValueOrDefault() > 0)
                    filtro = (string.IsNullOrEmpty(filtro) ? " And (" : filtro.TrimEnd(')') + " Or ") + "c.idSinal=" + idPagamentoAntecipado + ")";

                var filtroLiberacao = string.Format(@" {0} (c.IdLiberarPedido IN ({1}) OR c.IdLiberarPedido IN (SELECT pnf.IdLiberarPedido FROM pedidos_nota_fiscal pnf WHERE pnf.IdPedido={2})
                    OR c.IdNf IN (SELECT pnf.IdNf FROM pedidos_nota_fiscal pnf WHERE pnf.IdPedido={2}))", "{0}", idsLiberacao, idPedido);

                if (string.IsNullOrEmpty(filtro))
                {
                    if (PedidoConfig.LiberarPedido)
                        filtroAdicional += string.Format(filtroLiberacao, "AND");
                    else
                        filtroAdicional += " And c.idPedido=" + idPedido;
                }
                else
                    filtroAdicional += string.Format("{0} {1} OR c.IdPedido={2} OR c.IdNf IN (SELECT pnf.IdNf FROM pedidos_nota_fiscal pnf WHERE pnf.IdPedido={2}))",
                        filtro.TrimEnd(')'), PedidoConfig.LiberarPedido ? string.Format(filtroLiberacao, "OR") : string.Empty, idPedido);

                criterio += "Pedido N.º " + idPedido + "    ";
            }

            if (idLiberarPedido > 0)
            {
                filtroAdicional += " and (c.IdLiberarPedido=" + idLiberarPedido +
                    " Or c.idNf In (Select pnf.idNf From pedidos_nota_fiscal pnf Where pnf.idLiberarPedido=" + idLiberarPedido + "))";
                criterio += "Liberação N.º " + idLiberarPedido + "    ";
            }

            if (idAcerto > 0)
            {
                filtroAdicional += " and c.idAcerto=" + idAcerto;
                criterio += "Acerto N.º " + idAcerto + "    ";
            }

            if (idAcertoParcial > 0)
            {
                filtroAdicional += " and c.idAcertoParcial=" + idAcertoParcial;
                criterio += "Acerto parcial N.º " + idAcertoParcial + "    ";
            }

            if (idTrocaDevolucao > 0)
            {
                filtroAdicional += " And c.idTrocaDevolucao=" + idTrocaDevolucao;
                criterio += "Troca/Devolução N.º " + idTrocaDevolucao;
            }

            if (idCli > 0 && contasVinculadas)
            {
                filtroAdicional +=
                    string.Format(
                        " AND (c.IdCliente IN (SELECT IdClienteVinculo FROM cliente_vinculo WHERE IdCliente={0}) OR cli.id_cli={0})",
                        idCli);
                criterio += string.Format("Cliente: {0} e clientes vinculados    ", ClienteDAO.Instance.GetNome(idCli));
            }
            else if (idCli > 0)
            {
                filtroAdicional += string.Format(" And c.IdCliente={0}", idCli);
                criterio += string.Format("Cliente: {0}    ", ClienteDAO.Instance.GetNome(idCli));
            }
            else if (!string.IsNullOrEmpty(nomeCli))
            {
                var ids = ClienteDAO.Instance.GetIds(null, nomeCli, null, 0, null, null, null, null, 0);
                filtroAdicional += string.Format(" AND c.IdCliente IN ({0})", ids);
                criterio += string.Format("Cliente: {0}    ", nomeCli);
            }

            if (tipoEntrega > 0)
            {
                where += " And p.TipoEntrega=" + tipoEntrega;
                switch (tipoEntrega)
                {
                    case 1:
                        criterio += "Tipo Entrega: Balcão    "; break;
                    case 2:
                        criterio += "Tipo Entrega: Colocação Comum    "; break;
                    case 3:
                        criterio += "Tipo Entrega: Colocação Temperado    "; break;
                    case 4:
                        criterio += "Tipo Entrega: Entrega    "; break;
                    case 5:
                        criterio += "Tipo Entrega: Manutenção Temperado    "; break;
                    case 6:
                        criterio += "Tipo Entrega: Colocação Esquadria    "; break;
                }

                temFiltro = true;
            }

            if (idLoja > 0)
            {
                where += " And c.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (idFuncRecebido > 0)
            {
                where += "  And f.idFunc=" + idFuncRecebido;
                temFiltro = true;
                criterio += "Recebido Por: " + FuncionarioDAO.Instance.GetNome(idFuncRecebido) + "    ";
            }

            if (idComissionado > 0)
            {
                if (PedidoConfig.LiberarPedido)
                    filtroAdicional += @" And c.idLiberarPedido In (Select idLiberarPedido From produtos_liberar_pedido Where idPedido In
                        (Select idPedido From pedido Where idComissionado=" + idComissionado + "))";
                else
                {
                    where += " And p.idComissionado=" + idComissionado;
                    temFiltro = true;
                }

                criterio += "Comissionado: " + ComissionadoDAO.Instance.ObtemValorCampo<string>("Nome", "idComissionado=" + idComissionado) + "    ";
            }

            if (idFunc > 0)
            {
                where += " And (" + (PedidoConfig.LiberarPedido ? "plib.idFunc=" : "p.idFunc=") + idFunc +
                    @" Or c.idSinal In (Select Coalesce(p1.idSinal, p1.idPagamentoAntecipado) from pedido p1
                    Where Coalesce(p1.idSinal, p1.idPagamentoAntecipado, 0) > 0 And p1.idFunc=" + idFunc + @")
                Or c.idNf In (Select pnf.idnf From pedidos_nota_fiscal pnf
                        Left Join pedido ped ON (pnf.idpedido=ped.idpedido)
                    Where ped.idFunc=" + idFunc + "))";

                temFiltro = true;
                criterio += "Vendedor: " + FuncionarioDAO.Instance.GetNome(idFunc) + "    ";
            }

            if (!String.IsNullOrEmpty(dtIniVenc))
            {
                filtroAdicional += " And DATAVEC>=?dtIniVenc";
                criterio += "Data Início Venc.: " + dtIniVenc + "    ";
            }

            if (!String.IsNullOrEmpty(dtFimVenc))
            {
                filtroAdicional += " And DATAVEC<=?dtFimVenc";
                criterio += "Data Fim Venc.: " + dtFimVenc + "    ";
            }

            if (!String.IsNullOrEmpty(dtIniRec))
            {
                filtroAdicional += " And DATAREC>=?dtIniRec";
                criterio += "Data Início Rec.: " + dtIniRec + "    ";
            }

            if (!String.IsNullOrEmpty(dtFimRec))
            {
                filtroAdicional += " And DATAREC<=?dtFimRec";
                criterio += "Data Fim Rec.: " + dtFimRec + "    ";
            }

            if (!String.IsNullOrEmpty(dataIniCad))
            {
                filtroAdicional += " And c.DataCad>=?dataIniCad";
                criterio += "Data Início Cad.: " + dataIniCad + "    ";
            }

            if (!String.IsNullOrEmpty(dataFimCad))
            {
                filtroAdicional += " And c.DataCad<=?dataFimCad";
                criterio += "Data Fim Cad.: " + dataFimCad + "    ";
            }

            if (PedidoConfig.LiberarPedido)
            {
                if (!String.IsNullOrEmpty(dtIniLib))
                {
                    where += " And lp.DataLiberacao>=?dtIniLib";
                    criterio += "Data Início Liberação: " + dtIniLib + "    ";
                    temFiltro = true;
                }

                if (!String.IsNullOrEmpty(dtFimLib))
                {
                    where += " And lp.DataLiberacao<=?dtFimLib";
                    criterio += "Data Fim Liberação: " + dtFimLib + "    ";
                    temFiltro = true;
                }
            }

            if (precoInicial > 0)
            {
                filtroAdicional += " And c." + (!recebida.HasValue ? "coalesce(valorRec,valorVec)" : recebida.Value ? "valorRec" : "valorVec") + ">=" + precoInicial.ToString().Replace(',', '.');
                criterio += "A partir de: " + precoInicial.ToString("C") + "    ";
            }

            if (precoFinal > 0)
            {
                filtroAdicional += " And c." + (!recebida.HasValue ? "coalesce(valorRec,valorVec)" : recebida.Value ? "valorRec" : "valorVec") + "<=" + precoFinal.ToString().Replace(',', '.');
                criterio += "Até: " + precoFinal.ToString("C") + "    ";
            }

            if (!string.IsNullOrWhiteSpace(idsFormaPagto) && idsFormaPagto != "0")
            {
                filtroAdicional += string.Format(" AND pcr.IdFormaPagto IN ({0})", idsFormaPagto);
                //criterio += "Forma Pagto.: " + UtilsPlanoConta.GetFormaPagtoFromPlanoReceb(UtilsPlanoConta.GetPlanoReceb(idFormaPagto));
            }

            if (renegociadas != null && (!recebida.HasValue || !recebida.Value))
            {
                if (!renegociadas.Value)
                    filtroAdicional += " and (renegociada=false Or renegociada is null Or valorRec>0)";
                else
                {
                    filtroAdicional += " and renegociada=true";
                    criterio += "Exibindo apenas contas renegociadas    ";
                }
            }
            else if (renegociadas.GetValueOrDefault(false) && recebida.GetValueOrDefault(false))
            {
                filtroAdicional += " and renegociada=true";
                criterio += "Exibindo apenas contas renegociadas    ";
            }
            else if (renegociadas.HasValue && !renegociadas.Value)
                filtroAdicional += " and (renegociada=false Or renegociada is null Or valorRec>0)";

            if (idRota > 0)
            {
                filtroAdicional += " And cli.id_cli in (Select idCliente From rota_cliente Where idRota=" + idRota + ")";
                criterio += "Rota: " + RotaDAO.Instance.ObtemValorCampo<string>("codInterno", "idRota=" + idRota) + "    ";
            }

            if (!string.IsNullOrEmpty(obs))
            {
                filtroAdicional += " And c.obs like ?obs";
                criterio += "Obs.: " + obs + "    ";
            }

            if (!String.IsNullOrEmpty(tipoContaContabil))
            {
                string c;
                filtroAdicional += FiltroTipoConta("c", tipoContaContabil, out c);
                criterio += c;
            }

            if (numArqRemessa > 0)
            {
                filtroAdicional += " And c.numArquivoRemessaCnab=" + numArqRemessa;
                criterio += "Núm. Arquivo Remessa: " + numArqRemessa + "    ";
            }

            if (!string.IsNullOrEmpty(numAutCartao))
            {
                filtroAdicional += " And pcr.numAutCartao=" + "'"+numAutCartao+"'";
                criterio += "Núm. Autorização do Cartão: " + numAutCartao + "    ";
            }

            if (!refObra)
            {
                filtroAdicional += " And c.IdObra IS NULL";
                criterio += "Sem referência para obra    ";
            }

            if (idVendedorAssociado > 0)
            {
                var idsCliente = string.Join(",", ClienteDAO.Instance.ExecuteMultipleScalar<string>(string.Format("Select id_Cli From cliente Where idFunc={0}", idVendedorAssociado)));
                if (string.IsNullOrEmpty(idsCliente))
                    idsCliente = "0";

                filtroAdicional += string.Format(" AND c.IdCliente In ({0})", idsCliente);
                criterio += "Vendedor Associado: " + FuncionarioDAO.Instance.GetNome((uint)idVendedorAssociado) + "    ";
            }

            if (idVendedorObra > 0)
            {
                filtroAdicional += string.Format(" AND c.IdObra IN (SELECT IdObra FROM obra WHERE (GerarCredito IS NULL OR GerarCredito=0) AND IdFunc={0})", idVendedorObra);
                criterio += "Vendedor Obra: " + FuncionarioDAO.Instance.GetNome((uint)idVendedorObra) + "    ";
            }

            if (FinanceiroConfig.FinanceiroRec.ExibirCnab && protestadas)
            {
                where += " AND COALESCE(rar.protestado, 0) = 1";
                criterio += "Somente contas protestadas  ";
                temFiltro = true;
            }

            if (contasCnab > 0)
            {
                switch (contasCnab)
                {
                    case 1:
                        filtroAdicional += " And numArquivoRemessaCnab Is Null";
                        criterio += "Não incluir contas de arquivo de remessa    ";
                        break;
                    case 2:
                        criterio += "Incluir contas de arquivo de remessa    ";
                        break;
                    case 3:
                        filtroAdicional += " And numArquivoRemessaCnab Is Not Null";
                        criterio += "Somente contas de arquivo de remessa    ";
                        break;
                }
            }

            if (idContaBancoRecebimento > 0)
            {
                where += $" AND pcr.IdContaBanco = {idContaBancoRecebimento}";
                temFiltro = true;

                var descricaoContaBancoRecebimento = ContaBancoDAO.Instance.GetDescricao(null, (uint)idContaBancoRecebimento);
                criterio += $"Conta bancária recebimento: {descricaoContaBancoRecebimento}    ";
            }

            if (idComissao > 0)
            {
                filtroAdicional += " AND ccr.IdComissao=" + idComissao;
                criterio += "Comissão: " + idComissao + "     ";
            }

            if (idSinalPedido > 0)
            {
                filtroAdicional += " AND c.IdSinal = " + idSinalPedido;
                criterio += "Sinal / Pagto. Antecipado: " + idSinalPedido + "     ";
            }

            if (numCte > 0)
            {
                var idsCte = string.Join(",", Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.ObtemIdCteByNumero((uint)numCte).Select(f => f.ToString()).ToArray());

                if (String.IsNullOrEmpty(idsCte) || String.IsNullOrEmpty(idsCte.Trim(',')))
                    idsCte = "0";

                filtroAdicional += " AND c.idCte IN (" + idsCte + ")";
                criterio += " Num. CT-e:" + numCte + "    ";
            }

            // Se não for para retornar todos e nenhum filtro tiver sido especificado, não retorna nenhum registro
            if (!returnAll && where == String.Empty)
            {
                where = " And 0>1";
                temFiltro = false;
            }

            sql = sql.Replace("$$$", criterio) + where;

            if (!recebida.GetValueOrDefault(true))
                switch (sort)
                {
                    case 1: // Data Venc.
                        sql += " Order By DataVec Asc"; break;
                    case 2: // Cliente
                        sql += " Order By cli.Nome"; break;
                    case 3: // Valor
                        sql += " Order By ValorVec "; break;
                    default:
                        sql += " Order By DataVec Desc"; break;
                }

            if (FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber)
            {
                if (numeroNFe > 0)
                {
                    if (!selecionar)
                        sql += " group by c.id" + (PedidoConfig.LiberarPedido ? "Liberar" : "") + "Pedido";

                    sql += @" And c.idNf In (Select pnf.idNf From pedidos_nota_fiscal pnf Where pnf.idNf In
                    (Select nf.idNf From nota_fiscal nf Where nf.numeroNfe=" + numeroNFe + @"))
                    GROUP by c.IdContaR";

                    criterio += "Número NFe: " + numeroNFe + "    ";
                    temFiltro = true;
                }
                else
                    sql += " group by c.idContaR";
            }
            else
            {
                if (having)
                {
                    if (!selecionar)
                        sql += " group by " + (numeroNFe > 0 ? "c.id" + (PedidoConfig.LiberarPedido ? "Liberar" : "") + "Pedido" : "c.idContaR");
                    else
                        sql += " Group By c.IdContaR";

                    sql += " having 1";

                    if (numeroNFe > 0)
                    {
                        sql += " and concat(',', numeroNFe, ',') like '%," + numeroNFe + ",%'";
                        criterio += "Número NFe: " + numeroNFe + "    ";
                    }

                    // Ignora a otimização de SQL se houver Having
                    temFiltro = true;
                }
                else
                    sql += " group by c.idContaR";
            }

            if (!String.IsNullOrEmpty(tipoContasBuscar) && tipoContasBuscar != "1,2,3" && PedidoConfig.LiberarPedido)
            {
                List<string> itensBuscar = new List<string>(tipoContasBuscar.Split(','));

                string filtroBuscar = "";

                if (itensBuscar.Contains("1"))
                    filtroBuscar += " or !liberacaoNaoPossuiNotaFiscalGerada";

                if (itensBuscar.Contains("2"))
                    filtroBuscar += " or liberacaoNaoPossuiNotaFiscalGerada";

                if (itensBuscar.Contains("3"))
                    filtroBuscar += " or liberacaoNaoPossuiNotaFiscalGerada is null";

                sql += " and (" + filtroBuscar.Substring(4) + ")";
            }

            else if (!String.IsNullOrEmpty(tipoContasBuscar) && tipoContasBuscar != "1,2,3" && !PedidoConfig.LiberarPedido)
            {
                List<string> itensBuscar = new List<string>(tipoContasBuscar.Split(','));

                string filtroBuscar = "";

                if (itensBuscar.Contains("1"))
                    filtroBuscar += " or !pedidoNaoPossuiNotaFiscalGerada";

                if (itensBuscar.Contains("2"))
                    filtroBuscar += " or pedidoNaoPossuiNotaFiscalGerada";

                if (itensBuscar.Contains("3"))
                    filtroBuscar += " or pedidoNaoPossuiNotaFiscalGerada is null";

                sql += " and (" + filtroBuscar.Substring(4) + ")";
            }

            if (!selecionar)
            {
                sql = $"SELECT SUM(contagem) FROM ({sql}) AS temp";
            }

            return sql;
        }

        public ContasReceber[] GetForRpt(uint idPedido, uint idLiberarPedido, uint idAcerto, uint idAcertoParcial, uint idTrocaDevolucao,
            uint numeroNFe, uint idLoja, uint idCli, uint idFunc, uint idFuncRecebido, uint tipoEntrega, string nomeCli, string dtIniVenc,
            string dtFimVenc, string dtIniRec, string dtFimRec, string dataIniCad, string dataFimCad, string dtIniLib, string dtFimLib, string idsFormaPagto, uint idTipoBoleto,
            Single precoInicial, Single precoFinal, int sort, bool? renegociadas, bool? recebida, uint idComissionado,
            uint idRota, string obs, string tipoContaContabil, uint numArqRemessa, bool refObra, int contasCnab, int idVendedorAssociado, int idVendedorObra, int idComissao, int idSinal,
            int numCte, bool protestadas, bool contasVinculadas, string tipoContasBuscar, string numAutCartao, int? idContaBancoRecebimento)
        {
            bool temFiltro;
            string filtroAdicional;

            string sortExpression = " Order By " +
                (sort == 2 ? "cli.Nome Asc" :
                sort == 3 ? "c.ValorVec Asc" :
                sort == 4 ? "numeroNfe Desc" :
                "c.DataVec Desc");

            string sql = SqlRpt(idPedido, idLiberarPedido, idAcerto, idAcertoParcial, idTrocaDevolucao, numeroNFe, idLoja, idFunc, idFuncRecebido,
                idCli, tipoEntrega, nomeCli, dtIniVenc, dtFimVenc, dtIniRec, dtFimRec, dataIniCad, dataFimCad, dtIniLib, dtFimLib, idsFormaPagto, idTipoBoleto, precoInicial,
                precoFinal, recebida, idComissionado, idRota, obs, 0, renegociadas, tipoContaContabil, true, numArqRemessa, refObra, contasCnab, idVendedorAssociado, idVendedorObra, idComissao, idSinal, numCte,
                protestadas, contasVinculadas, tipoContasBuscar, numAutCartao, idContaBancoRecebimento, true, true, out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            List<ContasReceber> lst = objPersistence.LoadData(sql + sortExpression, GetParamRpt(nomeCli, dtIniVenc, dtFimVenc, dtIniRec, dtFimRec,
                dataIniCad, dataFimCad, dtIniLib, dtFimLib, null, null, obs));

            return lst.ToArray();
        }

        public ContasReceber[] GetForListRpt(uint idPedido, uint idLiberarPedido, uint idAcerto, uint idAcertoParcial, uint idTrocaDevolucao,
            uint numeroNFe, uint idLoja, uint idFunc, uint idFuncRecebido, uint idCli, uint tipoEntrega, string nomeCli, string dtIniVenc,
            string dtFimVenc, string dtIniRec, string dtFimRec, string dataIniCad, string dataFimCad, string idsFormaPagto, uint idTipoBoleto, float precoInicial, float precoFinal,
            bool? renegociadas, bool? recebida, uint idComissionado, uint idRota, string obs, int ordenacao, string tipoContaContabil,
            uint numArqRemessa, bool refObra, int contasCnab, int idVendedorAssociado, int idVendedorObra, int idComissao, int idSinal, int numCte,
            bool protestadas, bool contasVinculadas, string tipoContasBuscar, string numAutCartao, int? idContaBancoRecebimento, string sortExpression, int startRow, int pageSize)
        {
            var ordenacaoSql = !string.IsNullOrEmpty(sortExpression) ? string.Empty :
                ordenacao == 2 ? " ORDER BY cli.Nome Asc, c.idContaR Desc" :
                ordenacao == 3 ? " ORDER BY c.ValorVec Asc, c.idContaR Desc" :
                ordenacao == 4 ? " ORDER BY numeroNfe Desc, c.idContaR Desc" :
                " ORDER BY c.DataVec Desc, c.idContaR Desc";

            bool temFiltro;
            string filtroAdicional;

            string sql = SqlRpt(idPedido, idLiberarPedido, idAcerto, idAcertoParcial, idTrocaDevolucao, numeroNFe, idLoja, idFunc, idFuncRecebido,
                idCli, tipoEntrega, nomeCli, dtIniVenc, dtFimVenc, dtIniRec, dtFimRec, dataIniCad, dataFimCad, null, null, idsFormaPagto, idTipoBoleto, precoInicial, precoFinal,
                recebida, idComissionado, idRota, obs, 0, renegociadas, tipoContaContabil, true, numArqRemessa, refObra, contasCnab, idVendedorAssociado,
                idVendedorObra, idComissao, idSinal, numCte, protestadas, contasVinculadas, tipoContasBuscar, numAutCartao, idContaBancoRecebimento, false, true, out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            var lst = objPersistence.LoadDataWithSortExpression(sql + ordenacaoSql, new InfoSortExpression(sortExpression), new InfoPaging(startRow, pageSize),
                GetParamRpt(nomeCli, dtIniVenc, dtFimVenc, dtIniRec, dtFimRec, dataIniCad, dataFimCad, null, null, null, null, obs));

            return lst.ToArray();
        }

        public int GetRptCount(uint idPedido, uint idLiberarPedido, uint idAcerto, uint idAcertoParcial, uint idTrocaDevolucao,
            uint numeroNFe, uint idLoja, uint idFunc, uint idFuncRecebido, uint idCli, uint tipoEntrega, string nomeCli, string dtIniVenc,
            string dtFimVenc, string dtIniRec, string dtFimRec, string dataIniCad, string dataFimCad, string idsFormaPagto, uint idTipoBoleto, float precoInicial, float precoFinal,
            bool? renegociadas, bool? recebida, uint idComissionado, uint idRota, string obs, int ordenacao, string tipoContaContabil,
            uint numArqRemessa, bool refObra, int contasCnab, int idVendedorAssociado, int idVendedorObra, int idComissao, int idSinal, int numCte,
            bool protestadas, bool contasVinculadas, string tipoContasBuscar, string numAutCartao, int? idContaBancoRecebimento)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlRpt(idPedido, idLiberarPedido, idAcerto, idAcertoParcial, idTrocaDevolucao, numeroNFe, idLoja, idFunc, idFuncRecebido,
                idCli, tipoEntrega, nomeCli, dtIniVenc, dtFimVenc, dtIniRec, dtFimRec, dataIniCad, dataFimCad, null, null, idsFormaPagto, idTipoBoleto, precoInicial, precoFinal,
                recebida, idComissionado, idRota, obs, ordenacao, renegociadas, tipoContaContabil, true, numArqRemessa, refObra, contasCnab, idVendedorAssociado,
                idVendedorObra, idComissao, idSinal, numCte, protestadas, contasVinculadas, tipoContasBuscar, numAutCartao, idContaBancoRecebimento,  false, false, out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.ExecuteSqlQueryCount(sql, GetParamRpt(nomeCli, dtIniVenc, dtFimVenc, dtIniRec, dtFimRec, dataIniCad, dataFimCad, null, null, null, null, obs));
        }

        /// <summary>
        /// Preenche a localização de cada conta recebida da lista
        /// </summary>
        public void PreencheLocalizacao(GDASession session, ref ContasReceber[] lst)
        {
            var sqlContaRec = @"SELECT COALESCE(if(mb.IdMovBanco>0, Concat('Banco: ', COALESCE(cb.Nome, ''), ' Conta: ', COALESCE(cb.Conta, '')), if(cx.IdCaixaGeral>0, ' Cx. Geral', if(cd.IdCaixaDiario>0, ' Cx. Diário', ' '))), ' ')
                FROM contas_receber c
                    LEFT JOIN caixa_geral cx ON (c.IdContaR=cx.IdContaR)
                    LEFT JOIN caixa_diario cd ON (c.IdContaR=cd.IdContaR)
                    LEFT JOIN mov_banco mb ON (c.IdContaR=mb.IdContaR)
                    LEFT JOIN conta_banco cb ON (mb.IdContaBanco=cb.IdContaBanco)
                WHERE c.IdContaR=?id
                ORDER BY COALESCE(cx.IdCaixaGeral, 0), COALESCE(cd.IdCaixaDiario, 0), COALESCE(mb.IdMovBanco, 0) DESC LIMIT 1";

            var sqlSinal = @"SELECT COALESCE(if(mb.IdMovBanco>0, Concat('Banco: ', COALESCE(cb.Nome, ''), ' Conta: ', COALESCE(cb.Conta, '')), if(cx.IdCaixaGeral>0, ' Cx. Geral', if(cd.IdCaixaDiario>0, ' Cx. Diário', ' '))), ' ')
                FROM contas_receber c
                    LEFT JOIN caixa_geral cx ON (c.IdSinal=cx.IdSinal)
                    LEFT JOIN caixa_diario cd ON (c.IdSinal=cd.IdSinal)
                    LEFT JOIN mov_banco mb ON (c.IdSinal=mb.IdSinal)
                    LEFT JOIN conta_banco cb ON (mb.IdContaBanco=cb.IdContaBanco)
                WHERE c.IdSinal=?id
                ORDER BY COALESCE(cx.IdCaixaGeral, 0), COALESCE(cd.IdCaixaDiario, 0), COALESCE(mb.IdMovBanco, 0) DESC LIMIT 1";

            var sqlAcerto = @"SELECT COALESCE(if(mb.IdMovBanco>0, Concat('Banco: ', COALESCE(cb.Nome, ''), ' Conta: ', COALESCE(cb.Conta, '')), if(cx.IdCaixaGeral>0, ' Cx. Geral', if(cd.IdCaixaDiario>0, ' Cx. Diário', ' '))), ' ')
                FROM contas_receber c
                    LEFT JOIN caixa_geral cx ON (c.IdAcerto=cx.IdAcerto)
                    LEFT JOIN caixa_diario cd ON (c.IdAcerto=cd.IdAcerto)
                    LEFT JOIN mov_banco mb ON (c.IdAcerto=mb.IdAcerto)
                    LEFT JOIN conta_banco cb ON (mb.IdContaBanco=cb.IdContaBanco)
                WHERE c.IdAcerto=?id
                ORDER BY COALESCE(cx.IdCaixaGeral, 0), COALESCE(cd.IdCaixaDiario, 0), COALESCE(mb.IdMovBanco, 0) DESC LIMIT 1";

            var sqlAcertoParcial = @"SELECT COALESCE(if(mb.IdMovBanco>0, Concat('Banco: ', COALESCE(cb.Nome, ''), ' Conta: ', COALESCE(cb.Conta, '')), if(cx.IdCaixaGeral>0, ' Cx. Geral', if(cd.IdCaixaDiario>0, ' Cx. Diário', ' '))), ' ')
                FROM contas_receber c
                    LEFT JOIN caixa_geral cx ON (c.IdAcertoParcial=cx.IdAcerto)
                    LEFT JOIN caixa_diario cd ON (c.IdAcertoParcial=cd.IdAcerto)
                    LEFT JOIN mov_banco mb ON (c.IdAcertoParcial=mb.IdAcerto)
                    LEFT JOIN conta_banco cb ON (mb.IdContaBanco=cb.IdContaBanco)
                WHERE c.IdAcertoParcial=?id
                ORDER BY COALESCE(cx.IdCaixaGeral, 0), COALESCE(cd.IdCaixaDiario, 0), COALESCE(mb.IdMovBanco, 0) DESC LIMIT 1";

            var sqlPedido = @"SELECT COALESCE(if(mb.IdMovBanco>0, Concat('Banco: ', COALESCE(cb.Nome, ''), ' Conta: ', COALESCE(cb.Conta, '')), if(cx.IdCaixaGeral>0, ' Cx. Geral', if(cd.IdCaixaDiario>0, ' Cx. Diário', ' '))), ' ')
                FROM contas_receber c
                    LEFT JOIN caixa_geral cx ON (c.IdPedido=cx.IdPedido)
                    LEFT JOIN caixa_diario cd ON (c.IdPedido=cd.IdPedido)
                    LEFT JOIN mov_banco mb ON (c.IdPedido=mb.IdPedido)
                    LEFT JOIN conta_banco cb ON (mb.IdContaBanco=cb.IdContaBanco)
                WHERE c.IdPedido=?id
                ORDER BY COALESCE(cx.IdCaixaGeral, 0), COALESCE(cd.IdCaixaDiario, 0), COALESCE(mb.IdMovBanco, 0) DESC LIMIT 1";

            var sqlLiberacao = @"select Referencia from
                    (select IdCaixaDiario AS ID, 'Cx. Diário' AS Referencia
                    from caixa_diario
                    where idliberarpedido = ?id
	                union all
	                select IdCaixaGeral AS ID, 'Cx. Geral' AS Referencia
	                from caixa_geral
	                where idliberarpedido = ?id
	                union all
	                select IdMovBanco AS ID, Concat('Banco: ', COALESCE(cb.Nome, ''), ' Conta: ', COALESCE(cb.Conta, '')) AS Referencia
                    from mov_banco mb
		                LEFT JOIN conta_banco cb ON (mb.IdContaBanco = cb.IdContaBanco)
                    where idliberarpedido = ?id) as temp
                order by ID DESC";

            var sqlObra = @"SELECT COALESCE(IF(mb.IdMovBanco>0, CONCAT('Banco: ', COALESCE(cb.Nome, ''), ' Conta: ', COALESCE(cb.Conta, '')), IF(cx.IdCaixaGeral>0, ' Cx. Geral', IF(cd.IdCaixaDiario>0, ' Cx. Diário', ' '))), ' ')
                FROM contas_receber c
                    LEFT JOIN caixa_geral cx ON (c.IdObra=cx.IdObra AND c.IdConta=cx.IdConta AND c.ValorRec=cx.ValorMov)
                    LEFT JOIN caixa_diario cd ON (c.IdObra=cd.IdObra AND c.IdConta=cd.IdConta AND c.ValorRec=cd.Valor)
                    LEFT JOIN mov_banco mb ON (c.IdObra=mb.IdObra AND c.IdConta=mb.IdConta AND c.ValorRec=mb.ValorMov)
                    LEFT JOIN conta_banco cb ON (mb.IdContaBanco=cb.IdContaBanco)
                WHERE c.IdObra=?id AND c.IdFormaPagto=?fp
                ORDER BY COALESCE(cx.IdCaixaGeral, 0), COALESCE(cd.IdCaixaDiario, 0), COALESCE(mb.IdMovBanco, 0) DESC LIMIT 1";

            var sqlAntecip = @"SELECT COALESCE(IF(mb.IdMovBanco>0, CONCAT('Banco: ', COALESCE(cb.Nome, ''), ' Conta: ', COALESCE(cb.Conta, '')), ' ') , '')
                FROM contas_receber c
                    LEFT JOIN mov_banco mb ON (c.IdAntecipContaRec=mb.IdAntecipContaRec)
                    LEFT JOIN conta_banco cb ON (mb.IdContaBanco=cb.IdContaBanco)
                WHERE c.IdAntecipContaRec=?id
                ORDER BY COALESCE(mb.IdMovBanco, 0) DESC LIMIT 1";

            object obj = null;

            foreach (var cr in lst)
            {
                if (!string.IsNullOrWhiteSpace(cr.DestinoRec))
                    continue;

                #region Conta a receber

                /* Chamado 52415. */
                /* Chamado 19832: Deve vir antes do acerto parcial*/
                if (cr.IdContaR > 0)
                {
                    obj = objPersistence.ExecuteScalar(sqlContaRec.Replace("?id", cr.IdContaR.ToString()));

                    if (obj != null && !string.IsNullOrWhiteSpace(obj.ToString()))
                    {
                        cr.DestinoRec = obj.ToString();
                    }
                }

                #endregion

                #region Acerto

                if (string.IsNullOrWhiteSpace(cr.DestinoRec) && cr.IdAcerto > 0)
                {
                    obj = objPersistence.ExecuteScalar(sqlAcerto.Replace("?id", cr.IdAcerto.ToString()));

                    if (obj != null && !string.IsNullOrWhiteSpace(obj.ToString()))
                    {
                        cr.DestinoRec = obj.ToString();
                    }
                }

                #endregion

                #region Conta antecipada

                if (string.IsNullOrWhiteSpace(cr.DestinoRec) && cr.IdAntecipContaRec > 0)
                {
                    obj = objPersistence.ExecuteScalar(sqlAntecip.Replace("?id", cr.IdAntecipContaRec.ToString()));

                    if (!String.IsNullOrEmpty(obj.ToString()) && !String.IsNullOrWhiteSpace(obj.ToString()))
                    {
                        cr.DestinoRec = obj.ToString();
                    }
                }

                #endregion

                #region Liberação

                if (string.IsNullOrWhiteSpace(cr.DestinoRec) && cr.IdLiberarPedido > 0)
                {
                    obj = objPersistence.ExecuteScalar(sqlLiberacao.Replace("?id", cr.IdLiberarPedido.ToString()));

                    if (obj != null && !string.IsNullOrWhiteSpace(obj.ToString()))
                    {
                        cr.DestinoRec = obj.ToString();
                    }
                }

                #endregion

                #region Pedido à vista

                if (string.IsNullOrWhiteSpace(cr.DestinoRec) && cr.IdPedido > 0)
                {
                    obj = objPersistence.ExecuteScalar(sqlPedido.Replace("?id", cr.IdPedido.ToString()));

                    if (obj != null && !string.IsNullOrWhiteSpace(obj.ToString()))
                    {
                        cr.DestinoRec = obj.ToString();
                    }
                }

                #endregion

                #region Sinal

                if (string.IsNullOrWhiteSpace(cr.DestinoRec) && cr.IdSinal > 0)
                {
                    obj = objPersistence.ExecuteScalar(sqlSinal.Replace("?id", cr.IdSinal.ToString()));

                    if (obj != null && !string.IsNullOrWhiteSpace(obj.ToString()))
                    {
                        cr.DestinoRec = obj.ToString();
                    }
                }

                #endregion

                #region Obra

                if (string.IsNullOrWhiteSpace(cr.DestinoRec) && cr.IdObra > 0)
                {
                    obj = objPersistence.ExecuteScalar(sqlObra.Replace("?id", cr.IdObra.ToString()).Replace("?fp", cr.IdFormaPagto.GetValueOrDefault((uint)Pagto.FormaPagto.Prazo).ToString()));

                    if (obj != null && !string.IsNullOrWhiteSpace(obj.ToString()))
                    {
                        cr.DestinoRec = obj.ToString();
                    }
                }

                #endregion

                #region Acerto parcial

                /* Chamado 16739. */
                if (string.IsNullOrWhiteSpace(cr.DestinoRec) && cr.IdAcertoParcial > 0)
                {
                    obj = objPersistence.ExecuteScalar(sqlAcertoParcial.Replace("?id", cr.IdAcertoParcial.ToString()));

                    if (obj != null && !string.IsNullOrWhiteSpace(obj.ToString()))
                    {
                        cr.DestinoRec = obj.ToString();
                    }
                }

                #endregion

                #region Conta renegociada

                if (string.IsNullOrWhiteSpace(cr.DestinoRec) && cr.Renegociada)
                    cr.DestinoRec = "Renegociada";

                #endregion

                objPersistence.ExecuteCommand(session, "update contas_receber set DestinoRec=?obj where idContar=" + cr.IdContaR,
                        new GDAParameter("?obj", cr.DestinoRec));
            }
        }

        #endregion

        #region Copia a referência de uma conta a receber para a outra

        private void CopiaReferencias(GDASession session, ContasReceber original, ref ContasReceber nova)
        {
            nova.IdAcerto = original.IdAcerto;
            nova.IdAcertoParcial = original.IdAcertoParcial;
            nova.IdAntecipContaRec = original.IdAntecipContaRec;
            nova.IdDevolucaoPagto = original.IdDevolucaoPagto;
            nova.IdLiberarPedido = original.IdLiberarPedido;
            nova.IdObra = original.IdObra;
            nova.IdPedido = original.IdPedido;
            nova.IdTrocaDevolucao = original.IdTrocaDevolucao;
            nova.IdSinal = original.IdSinal;
            nova.NumCheque = original.NumCheque;
            nova.IdAcertoCheque = original.IdAcertoCheque;
            nova.IdEncontroContas = original.IdEncontroContas;
            nova.IdCte = original.IdCte;

            // Deve recuperar este campo direto no banco, pois como ele está como "input" na model este valor não vem preenchido,
            // no método que chamou esse deverá fazer um update nesta conta a receber, após receber a mesma
            nova.IdNf = ObtemValorCampo<uint?>(session, "IdNf", string.Format("IdContaR={0}", original.IdContaR));
        }

        #endregion

        #region Recebimento de conta

        /// <summary>
        /// Efetua o recebimento da conta a receber.
        /// </summary>
        public string ReceberContaComTransacao(bool caixaDiario, decimal creditoUtilizado, IEnumerable<string> dadosChequesRecebimento, DateTime dataRecebimento, bool descontarComissao, bool gerarCredito,
            int idContaR, int? idPedido, IEnumerable<int> idsCartaoNaoIdentificado, IEnumerable<int> idsContaBanco, IEnumerable<int> idsDepositoNaoIdentificado, IEnumerable<int> idsFormaPagamento,
            IEnumerable<int> idsTipoCartao, decimal juros, IEnumerable<string> numerosAutorizacaoCartao, string numeroAutorizacaoConstrucard, IEnumerable<int> quantidadesParcelaCartao,
            bool recebimentoParcial, IEnumerable<decimal> taxasAntecipacao, IEnumerable<int> tiposBoleto, IEnumerable<decimal> valoresRecebimento)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    CriarPreRecebimentoConta(transaction, caixaDiario, creditoUtilizado, dadosChequesRecebimento, dataRecebimento, descontarComissao, gerarCredito, idContaR, idPedido,
                        idsCartaoNaoIdentificado, idsContaBanco, idsDepositoNaoIdentificado, idsFormaPagamento, idsTipoCartao, juros, numerosAutorizacaoCartao, numeroAutorizacaoConstrucard,
                        quantidadesParcelaCartao, recebimentoParcial, taxasAntecipacao, tiposBoleto, valoresRecebimento);

                    var retorno = FinalizarPreRecebimentoConta(transaction, idContaR);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("ReceberConta - ID conta receber: {0}.", idContaR), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao receber conta.", ex));
                }
            }
        }

        /// <summary>
        /// Efetua o recebimento da conta a receber.
        /// </summary>
        public string ReceberConta(GDASession session, bool caixaDiario, decimal creditoUtilizado, IEnumerable<string> dadosChequesRecebimento, DateTime dataRecebimento, bool descontarComissao,
            bool gerarCredito, int idContaR, int? idPedido, IEnumerable<int> idsCartaoNaoIdentificado, IEnumerable<int> idsContaBanco, IEnumerable<int> idsDepositoNaoIdentificado,
            IEnumerable<int> idsFormaPagamento, IEnumerable<int> idsTipoCartao, decimal juros, IEnumerable<string> numerosAutorizacaoCartao, string numeroAutorizacaoConstrucard,
            IEnumerable<int> quantidadesParcelaCartao, bool recebimentoParcial, IEnumerable<decimal> taxasAntecipacao, IEnumerable<int> tiposBoleto, IEnumerable<decimal> valoresRecebimento)
        {
            CriarPreRecebimentoConta(session, caixaDiario, creditoUtilizado, dadosChequesRecebimento, dataRecebimento, descontarComissao, gerarCredito, idContaR, idPedido,
                idsCartaoNaoIdentificado, idsContaBanco, idsDepositoNaoIdentificado, idsFormaPagamento, idsTipoCartao, juros, numerosAutorizacaoCartao, numeroAutorizacaoConstrucard,
                quantidadesParcelaCartao, recebimentoParcial, taxasAntecipacao, tiposBoleto, valoresRecebimento);

            return FinalizarPreRecebimentoConta(session, idContaR);
        }

        /// <summary>
        /// Cria o pré recebimento da conta a receber.
        /// </summary>
        public void CriarPreRecebimentoContaComTransacao(bool caixaDiario, decimal creditoUtilizado, IEnumerable<string> dadosChequesRecebimento, DateTime dataRecebimento, bool descontarComissao,
            bool gerarCredito, int idContaR, int? idPedido, IEnumerable<int> idsCartaoNaoIdentificado, IEnumerable<int> idsContaBanco, IEnumerable<int> idsDepositoNaoIdentificado,
            IEnumerable<int> idsFormaPagamento, IEnumerable<int> idsTipoCartao, decimal juros, IEnumerable<string> numerosAutorizacaoCartao, string numeroAutorizacaoConstrucard,
            IEnumerable<int> quantidadesParcelaCartao, bool recebimentoParcial, IEnumerable<decimal> taxasAntecipacao, IEnumerable<int> tiposBoleto, IEnumerable<decimal> valoresRecebimento)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    CriarPreRecebimentoConta(transaction, caixaDiario, creditoUtilizado, dadosChequesRecebimento, dataRecebimento, descontarComissao, gerarCredito, idContaR, idPedido,
                        idsCartaoNaoIdentificado, idsContaBanco, idsDepositoNaoIdentificado, idsFormaPagamento, idsTipoCartao, juros, numerosAutorizacaoCartao, numeroAutorizacaoConstrucard,
                        quantidadesParcelaCartao, recebimentoParcial, taxasAntecipacao, tiposBoleto, valoresRecebimento);

                    TransacaoCapptaTefDAO.Instance.Insert(transaction, new TransacaoCapptaTef()
                    {
                        IdReferencia = idContaR,
                        TipoRecebimento = UtilsFinanceiro.TipoReceb.ContaReceber
                    });

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("CriarPreRecebimentoContaComTransacao - ID conta receber: {0}.", idContaR), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao receber conta.", ex));
                }
            }
        }

        /// <summary>
        /// Recupera a conta e atualiza com os dados inseridos para o recebimento
        /// </summary>
        /// <returns></returns>
        public ContasReceber PrepararContaRecebimento(GDASession session, bool caixaDiario, decimal creditoUtilizado, DateTime dataRecebimento, bool descontarComissao,
            bool gerarCredito, int idContaR, IEnumerable<int> idsFormaPagamento, IEnumerable<int> idsTipoCartao, decimal juros, string numeroAutorizacaoConstrucard,
            IEnumerable<int> quantidadesParcelaCartao, bool recebimentoParcial,  IEnumerable<decimal> valoresRecebimento)
        {
            #region Declaração de variáveis

            var usuarioLogado = UserInfo.GetUserInfo;
            var contaReceber = GetElementByPrimaryKey(session, idContaR);
            var pedido = contaReceber.IdPedido > 0 ? PedidoDAO.Instance.GetElementByPrimaryKey(session, contaReceber.IdPedido.Value) : new Pedido();
            decimal totalPago = 0;

            #endregion

            #region Cálculo dos totais da conta a receber

            totalPago += valoresRecebimento.Sum(f => f) + (creditoUtilizado > 0 ? creditoUtilizado : 0);

            if (descontarComissao)
            {
                pedido.ComissaoFuncionario = Pedido.TipoComissao.Comissionado;
                totalPago += pedido.ValorComissaoPagar;
            }

            // Ignora os juros dos cartões ao calcular o valor pago/a pagar.
            totalPago -= UtilsFinanceiro.GetJurosCartoes(session, usuarioLogado.IdLoja, valoresRecebimento.ToArray(), idsFormaPagamento.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(),
                idsTipoCartao.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(), quantidadesParcelaCartao.Select(f => ((uint?)f).GetValueOrDefault()).ToArray());

            #endregion

            #region Recuperação de dados da conta a receber

            // Atualiza esta conta a receber.
            contaReceber.UsuRec = usuarioLogado.CodUser;
            contaReceber.ValorRec = totalPago - juros;
            contaReceber.IdConta = totalPago == creditoUtilizado || idsFormaPagamento.ElementAtOrDefault(0) == 0 ? UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecPrazoCredito) :
                idsFormaPagamento.ElementAtOrDefault(0) != (uint)Pagto.FormaPagto.Cartao ? UtilsPlanoConta.GetPlanoReceb((uint)idsFormaPagamento.ElementAtOrDefault(0)) :
                UtilsPlanoConta.GetPlanoRecebTipoCartao((uint)idsTipoCartao.ElementAtOrDefault(0));
            contaReceber.DataRec = dataRecebimento;
            contaReceber.Juros = juros;
            contaReceber.NumAutConstrucard = numeroAutorizacaoConstrucard;
            contaReceber.TotalPago = totalPago;
            contaReceber.IdLojaRecebimento = (int)usuarioLogado.IdLoja;
            contaReceber.DescontarComissao = descontarComissao;
            contaReceber.RecebimentoParcial = recebimentoParcial;
            contaReceber.RecebimentoCaixaDiario = caixaDiario;
            contaReceber.RecebimentoGerarCredito = gerarCredito;

            #endregion

            return contaReceber;
        }

        /// <summary>
        /// Cria o pré recebimento da conta a receber.
        /// </summary>
        public void CriarPreRecebimentoConta(GDASession session, bool caixaDiario, decimal creditoUtilizado, IEnumerable<string> dadosChequesRecebimento, DateTime dataRecebimento, bool descontarComissao,
            bool gerarCredito, int idContaR, int? idPedido, IEnumerable<int> idsCartaoNaoIdentificado, IEnumerable<int> idsContaBanco, IEnumerable<int> idsDepositoNaoIdentificado,
            IEnumerable<int> idsFormaPagamento, IEnumerable<int> idsTipoCartao, decimal juros, IEnumerable<string> numerosAutorizacaoCartao, string numeroAutorizacaoConstrucard,
            IEnumerable<int> quantidadesParcelaCartao, bool recebimentoParcial, IEnumerable<decimal> taxasAntecipacao, IEnumerable<int> tiposBoleto, IEnumerable<decimal> valoresRecebimento)
        {
            var contaReceber = PrepararContaRecebimento(session, caixaDiario, creditoUtilizado, dataRecebimento,
                descontarComissao, gerarCredito, idContaR, idsFormaPagamento, idsTipoCartao, juros,
                numeroAutorizacaoConstrucard, quantidadesParcelaCartao, recebimentoParcial, valoresRecebimento);

            #region Validações do recebimento da conta

            ValidarRecebimentoConta(session, contaReceber, creditoUtilizado, dadosChequesRecebimento, idsCartaoNaoIdentificado, idsContaBanco, idsDepositoNaoIdentificado, idsFormaPagamento, idsTipoCartao,
                numerosAutorizacaoCartao, numeroAutorizacaoConstrucard, quantidadesParcelaCartao, taxasAntecipacao, tiposBoleto, valoresRecebimento);

            #endregion

            #region Atualização da conta a receber

            contaReceber.Recebida = true;
            Update(session, contaReceber);

            #endregion

            #region Inserção dos pagamentos da conta a receber

            ChequesContasReceberDAO.Instance.InserirPelaString(session, contaReceber, dadosChequesRecebimento);
            PagtoContasReceberDAO.Instance.DeleteByIdContaR(session, contaReceber.IdContaR);

            // Salva as formas de pagamento.
            for (var i = 0; i < valoresRecebimento.Count(); i++)
            {
                if (idsFormaPagamento.ElementAtOrDefault(i) == 0 || valoresRecebimento.ElementAtOrDefault(i) == 0)
                {
                    continue;
                }

                if (idsFormaPagamento.ElementAt(i) == (int)Pagto.FormaPagto.CartaoNaoIdentificado)
                {
                    var cartoesNaoIdentificado = CartaoNaoIdentificadoDAO.Instance.ObterPeloId(session, idsCartaoNaoIdentificado.Select(f => ((uint?)f).GetValueOrDefault()).ToArray());

                    foreach (var cartaoNaoIdentificado in cartoesNaoIdentificado)
                    {
                        var pagamentoContaReceber = new PagtoContasReceber();
                        pagamentoContaReceber.IdContaR = contaReceber.IdContaR;
                        pagamentoContaReceber.IdFormaPagto = (uint)idsFormaPagamento.ElementAt(i);
                        pagamentoContaReceber.IdContaBanco = (uint)cartaoNaoIdentificado.IdContaBanco;
                        pagamentoContaReceber.IdCartaoNaoIdentificado = cartaoNaoIdentificado.IdCartaoNaoIdentificado;
                        pagamentoContaReceber.IdTipoCartao = (uint)cartaoNaoIdentificado.TipoCartao;
                        pagamentoContaReceber.ValorPagto = cartaoNaoIdentificado.Valor;
                        pagamentoContaReceber.NumAutCartao = cartaoNaoIdentificado.NumAutCartao;

                        PagtoContasReceberDAO.Instance.Insert(session, pagamentoContaReceber);
                    }
                }
                else
                {
                    var pagamentoContaReceber = new PagtoContasReceber();
                    pagamentoContaReceber.IdContaR = contaReceber.IdContaR;
                    pagamentoContaReceber.IdFormaPagto = (uint)idsFormaPagamento.ElementAt(i);
                    pagamentoContaReceber.ValorPagto = valoresRecebimento.ElementAt(i);
                    pagamentoContaReceber.IdContaBanco = idsFormaPagamento.ElementAt(i) != (uint)Pagto.FormaPagto.Dinheiro && idsContaBanco.ElementAtOrDefault(i) > 0 ? (uint?)idsContaBanco.ElementAt(i) : null;
                    pagamentoContaReceber.IdTipoCartao = idsTipoCartao.ElementAtOrDefault(i) > 0 ? (uint?)idsTipoCartao.ElementAt(i) : null;
                    pagamentoContaReceber.IdDepositoNaoIdentificado = idsDepositoNaoIdentificado.ElementAtOrDefault(i) > 0 ? (uint?)idsDepositoNaoIdentificado.ElementAt(i) : null;
                    pagamentoContaReceber.QuantidadeParcelaCartao = pagamentoContaReceber.IdTipoCartao > 0 && quantidadesParcelaCartao.ElementAtOrDefault(i) > 0 ? (int?)quantidadesParcelaCartao.ElementAt(i) : null;
                    pagamentoContaReceber.TaxaAntecipacao = taxasAntecipacao.ElementAtOrDefault(i) > 0 ? (int?)taxasAntecipacao.ElementAt(i) : null;
                    pagamentoContaReceber.TipoBoleto = tiposBoleto.ElementAtOrDefault(i) > 0 ? (int?)tiposBoleto.ElementAt(i) : null;
                    pagamentoContaReceber.NumAutCartao = !string.IsNullOrWhiteSpace(numerosAutorizacaoCartao.ElementAtOrDefault(i)) ? numerosAutorizacaoCartao.ElementAt(i) : null;

                    PagtoContasReceberDAO.Instance.Insert(session, pagamentoContaReceber);
                }
            }

            if (creditoUtilizado > 0)
            {
                var pagamentoContaReceber = new PagtoContasReceber();
                pagamentoContaReceber.IdContaR = contaReceber.IdContaR;
                pagamentoContaReceber.IdFormaPagto = (uint)Pagto.FormaPagto.Credito;
                pagamentoContaReceber.ValorPagto = creditoUtilizado;

                PagtoContasReceberDAO.Instance.Insert(session, pagamentoContaReceber);
            }

            #endregion
        }

        /// <summary>
        /// Valida o recebimento da conta a receber.
        /// </summary>
        public void ValidarRecebimentoConta(GDASession session, ContasReceber contaReceber, decimal creditoUtilizado, IEnumerable<string> dadosChequesRecebimento,
            IEnumerable<int> idsCartaoNaoIdentificado, IEnumerable<int> idsContaBanco, IEnumerable<int> idsDepositoNaoIdentificado, IEnumerable<int> idsFormaPagamento, IEnumerable<int> idsTipoCartao,
            IEnumerable<string> numerosAutorizacaoCartao, string numeroAutorizacaoConstrucard, IEnumerable<int> quantidadesParcelaCartao, IEnumerable<decimal> taxasAntecipacao,
            IEnumerable<int> tiposBoleto, IEnumerable<decimal> valoresRecebimento)
        {
            #region Declaração de variáveis

            var usuarioLogado = UserInfo.GetUserInfo;
            var pedido = contaReceber.IdPedido > 0 ? PedidoDAO.Instance.GetElementByPrimaryKey(session, contaReceber.IdPedido.Value) : new Pedido();
            var liberacao = contaReceber.IdLiberarPedido > 0 ? LiberarPedidoDAO.Instance.GetElementByPrimaryKey(session, contaReceber.IdLiberarPedido.Value) : new LiberarPedido();
            var idCliente = contaReceber.IdCliente > 0 ? contaReceber.IdCliente : contaReceber.IdPedido != null ? pedido.IdCli : contaReceber.IdLiberarPedido > 0 ? liberacao.IdCliente : 0;
            decimal totalPago = 0;

            #endregion

            #region Recuperação do valor total pago

            totalPago = valoresRecebimento.Sum(f => f) + (creditoUtilizado > 0 ? creditoUtilizado : 0);

            if (contaReceber.DescontarComissao.GetValueOrDefault())
            {
                pedido.ComissaoFuncionario = Pedido.TipoComissao.Comissionado;
                totalPago += pedido.ValorComissaoPagar;
            }

            // Ignora os juros dos cartões ao calcular o valor pago/a pagar.
            totalPago -= UtilsFinanceiro.GetJurosCartoes(session, usuarioLogado.IdLoja, valoresRecebimento.ToArray(), idsFormaPagamento.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(),
                idsTipoCartao.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(), quantidadesParcelaCartao.Select(f => ((uint?)f).GetValueOrDefault()).ToArray());

            #endregion

            #region Validações da conta a receber

            // Não prossegue caso a conta esteja recebida.
            if (contaReceber.Recebida)
            {
                throw new Exception("Esta conta já foi recebida.");
            }

            // Verifica se a conta a receber existe.
            if (!Exists(session, contaReceber))
            {
                throw new Exception("Está conta não existe.");
            }

            // Chamado 45154.
            if (contaReceber.IdAcerto > 0 && contaReceber.IdAcertoParcial > 0)
            {
                throw new Exception("Esta conta possui referência de acerto e acerto parcial, portanto, efetue o recebimento dela através de um acerto.");
            }

            #endregion

            #region Validações de permissão

            // Se não for caixa diário ou financeiro, não pode receber sinal.
            if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) && !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
            {
                throw new Exception("Você não tem permissão para receber contas.");
            }

            // Apenas administrador, financeiro geral e financeiro pagto podem gerar comissões.
            if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento))
            {
                throw new Exception("Você não tem permissão para gerar comissões");
            }

            #endregion

            #region Validações dos totais do recebimento

            // Mesmo se for recebimento parcial, não é permitido receber valor maior do que o valor da conta. Além disso, não é permitido receber uma conta com o valor a receber zerado.
            if (contaReceber.RecebimentoParcial.GetValueOrDefault())
            {
                if (totalPago - contaReceber.Juros > contaReceber.ValorVec)
                {
                    throw new Exception("Valor pago excede o valor da conta.");
                }

                // Chamado 40478.
                if ((contaReceber.ValorVec + contaReceber.Juros) - totalPago == 0)
                {
                    throw new Exception("A conta a receber gerada pelo recebimento parcial não pode estar zerada.");
                }
            }
            // Se o valor for inferior ao que deve ser pago, e o restante do pagto for gerarCredito, lança exceção.
            else if (contaReceber.RecebimentoGerarCredito.GetValueOrDefault() && Math.Round(totalPago - contaReceber.Juros, 2) < Math.Round(contaReceber.ValorVec, 2))
            {
                throw new Exception(string.Format("Total a ser pago não confere com valor pago. Total a ser pago: {0} Valor pago: {1}.",
                    Math.Round(contaReceber.ValorVec + contaReceber.Juros, 2).ToString("C"), totalPago.ToString("C")));
            }
            // Se o total a ser pago for diferente do valor pago, considerando que não é para gerar crédito.
            else if (!contaReceber.RecebimentoGerarCredito.GetValueOrDefault() && Math.Round(totalPago - contaReceber.Juros, 2) != Math.Round(contaReceber.ValorVec, 2))
            {
                throw new Exception(string.Format("Total a ser pago não confere com valor pago. Total a ser pago: {0} Valor pago: {1}.",
                    Math.Round(contaReceber.ValorVec + contaReceber.Juros, 2).ToString("C"), totalPago.ToString("C")));
            }

            // Se o valor pago for menor que o valor de juros, lança exceção.
            if (Math.Round(contaReceber.Juros, 2) > Math.Round(totalPago, 2))
            {
                throw new Exception(string.Format("O valor de juros não pode ser maior que o total pago. Total pago: {0} Valor de juros: {1}.",
                    Math.Round(totalPago, 2).ToString("C"), Math.Round(contaReceber.Juros, 2).ToString("C")));
            }

            #endregion

            #region Validações dos dados de recebimento

            // Verifica se a forma de pagamento foi selecionada, apenas se o crédito não cobrir todo valor da conta com juros.
            if (idsFormaPagamento.Count() == 0 && Math.Round(creditoUtilizado, 2) < Math.Round(contaReceber.ValorVec + contaReceber.Juros, 2))
            {
                throw new Exception("Informe a forma de pagamento.");
            }

            if (UtilsFinanceiro.ContemFormaPagto(Pagto.FormaPagto.ChequeProprio, idsFormaPagamento.Select(f => ((uint?)f).GetValueOrDefault()).ToArray()) &&
                !(dadosChequesRecebimento?.Any(f => !string.IsNullOrWhiteSpace(f)) ?? false))
            {
                throw new Exception("Cadastre o(s) cheque(s) referente(s) ao pagamento da conta.");
            }

            UtilsFinanceiro.ValidarRecebimento(session, contaReceber.RecebimentoCaixaDiario.GetValueOrDefault(), (int?)idCliente, (int)usuarioLogado.IdLoja, idsCartaoNaoIdentificado, idsContaBanco,
                idsFormaPagamento, contaReceber.RecebimentoGerarCredito.GetValueOrDefault(), contaReceber.Juros, contaReceber.RecebimentoParcial.GetValueOrDefault(),
                UtilsFinanceiro.TipoReceb.ContaReceber, totalPago, contaReceber.ValorVec);

            #endregion
        }

        /// <summary>
        /// Finaliza o pré recebimento da conta a receber.
        /// </summary>
        public string FinalizarPreRecebimentoContaComTransacao(int idContaR)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = FinalizarPreRecebimentoConta(transaction, idContaR);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("FinalizarPreRecebimentoContaComTransacao - ID conta receber: {0}.", idContaR), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao finalizar o recebimento da conta.", ex));
                }
            }
        }

        /// <summary>
        /// Finaliza o pré recebimento da conta a receber.
        /// </summary>
        public string FinalizarPreRecebimentoConta(GDASession session, int idContaR)
        {
            #region Declaração de variáveis

            UtilsFinanceiro.DadosRecebimento retorno = null;
            var contaReceber = GetElementByPrimaryKey(session, idContaR);
            var pedido = contaReceber.IdPedido > 0 ? PedidoDAO.Instance.GetElementByPrimaryKey(session, contaReceber.IdPedido.Value) : new Pedido();
            var liberacao = contaReceber.IdLiberarPedido > 0 ? LiberarPedidoDAO.Instance.GetElementByPrimaryKey(session, contaReceber.IdLiberarPedido.Value) : new LiberarPedido();
            var idCliente = contaReceber.IdCliente > 0 ? contaReceber.IdCliente : contaReceber.IdPedido != null ? pedido.IdCli : contaReceber.IdLiberarPedido > 0 ? liberacao.IdCliente : 0;
            var idContaAntigo = contaReceber.IdConta;
            var totalPago = contaReceber.TotalPago.GetValueOrDefault();
            var descontarComissao = contaReceber.DescontarComissao.GetValueOrDefault();
            var recebimentoCaixaDiario = contaReceber.RecebimentoCaixaDiario.GetValueOrDefault();
            var recebimentoParcial = contaReceber.RecebimentoParcial.GetValueOrDefault();
            var recebimentoGerarCredito = contaReceber.RecebimentoGerarCredito.GetValueOrDefault();
            var chequesRecebimento = ChequesContasReceberDAO.Instance.ObterStringChequesPelaContaReceber(session, idContaR);
            var pagamentosContaReceber = PagtoContasReceberDAO.Instance.ObtemPagtos(session, (uint)idContaR);
            // Variáveis criadas para recuperar os dados do pagamento da conta a receber.
            var idsCartaoNaoIdentificado = new List<int?>();
            var idsContaBanco = new List<int?>();
            var idsDepositoNaoIdentificado = new List<int?>();
            var idsFormaPagamento = new List<int>();
            var idsTipoCartao = new List<int?>();
            var quantidadesParcelaCartao = new List<int?>();
            var taxasAntecipacao = new List<decimal?>();
            var tiposBoleto = new List<int?>();
            var valoresRecebimento = new List<decimal>();
            decimal creditoUtilizado = 0;
            var numeroParcelaContaReceber = 0;

            #endregion

            #region Recuperação dos dados de recebimento da conta a receber

            if (pagamentosContaReceber.Any(f => f.IdFormaPagto != (uint)Pagto.FormaPagto.Credito && f.IdFormaPagto != (uint)Pagto.FormaPagto.Obra))
            {
                foreach (var pagamentoContaReceber in pagamentosContaReceber.Where(f => f.IdFormaPagto != (uint)Pagto.FormaPagto.Credito && f.IdFormaPagto != (uint)Pagto.FormaPagto.Obra))
                {
                    idsCartaoNaoIdentificado.Add(pagamentoContaReceber.IdCartaoNaoIdentificado.GetValueOrDefault());
                    idsContaBanco.Add((int)pagamentoContaReceber.IdContaBanco.GetValueOrDefault());
                    idsDepositoNaoIdentificado.Add((int?)pagamentoContaReceber.IdDepositoNaoIdentificado.GetValueOrDefault());
                    idsFormaPagamento.Add((int)pagamentoContaReceber.IdFormaPagto);
                    idsTipoCartao.Add((int?)pagamentoContaReceber.IdTipoCartao);
                    quantidadesParcelaCartao.Add(pagamentoContaReceber.QuantidadeParcelaCartao.GetValueOrDefault());
                    taxasAntecipacao.Add(pagamentoContaReceber.TaxaAntecipacao);
                    tiposBoleto.Add(pagamentoContaReceber.TipoBoleto);
                    valoresRecebimento.Add(pagamentoContaReceber.ValorPagto);
                }
            }

            if (pagamentosContaReceber.Any(f => f.IdFormaPagto == (uint)Pagto.FormaPagto.Credito))
            {
                creditoUtilizado = pagamentosContaReceber.FirstOrDefault(f => f.IdFormaPagto == (uint)Pagto.FormaPagto.Credito)?.ValorPagto ?? 0;
            }

            #endregion

            #region Recebimento da conta a receber

            // Faz o recebimento desta conta.
            retorno = UtilsFinanceiro.Receber(session, (uint)contaReceber.IdLojaRecebimento, pedido, null, null, contaReceber, null, null, null, null, null, null, null, idCliente, 0, null,
                contaReceber.DataRec.GetValueOrDefault(DateTime.Now).ToString("dd/MM/yyyy"), contaReceber.ValorVec, totalPago, valoresRecebimento.ToArray(),
                idsFormaPagamento.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(), idsContaBanco.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(),
                idsDepositoNaoIdentificado.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(), idsCartaoNaoIdentificado.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(),
                idsTipoCartao.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(), tiposBoleto.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(),
                taxasAntecipacao.Select(f => f.GetValueOrDefault()).ToArray(), contaReceber.Juros, recebimentoParcial, recebimentoGerarCredito, creditoUtilizado, contaReceber.NumAutConstrucard,
                recebimentoCaixaDiario, quantidadesParcelaCartao.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(), chequesRecebimento, descontarComissao, UtilsFinanceiro.TipoReceb.ContaReceber);

            if (retorno.ex != null)
            {
                throw retorno.ex;
            }

            for (var i = 0; i < idsFormaPagamento.Count; i++)
            {
                if (idsFormaPagamento.ElementAt(i) == (uint)Pagto.FormaPagto.Cartao)
                {
                    numeroParcelaContaReceber = AtualizarReferenciaContasCartao(session, retorno, quantidadesParcelaCartao.Select(f => f.GetValueOrDefault()), numeroParcelaContaReceber, i,
                        contaReceber.IdContaR);
                }
            }

            #endregion

            #region Atualização dos dados do pedido

            // Libera o pedido para entrega somente se for empresa do tipo Comércio (Confirmação).
            if (!PedidoConfig.LiberarPedido && contaReceber.IdPedido > 0)
            {
                // Verifica se o pedido possui alguma conta a receber que ainda não foi recebida.
                if (!GetByPedido(session, contaReceber.IdPedido.Value, false, false)?.Any(f => !f.Recebida) ?? false)
                {
                    PedidoDAO.Instance.AlteraLiberarFinanc(session, contaReceber.IdPedido.Value, true);
                }
            }

            #endregion

            #region Geração da conta restante gerada pelo recebimento parcial

            // Se for recebimento parcial
            if (contaReceber.RecebimentoParcial.GetValueOrDefault())
            {
                /* Chamado 40478. */
                if ((contaReceber.ValorVec + contaReceber.Juros) - contaReceber.TotalPago.GetValueOrDefault() == 0)
                {
                    throw new Exception("A conta a receber gerada pelo recebimento parcial não pode estar zerada.");
                }

                // Insere outra parcela contendo o valor que deverá ser recebido
                var contaReceberRestante = new ContasReceber();
                contaReceberRestante.IdLoja = contaReceber.IdLoja;
                contaReceberRestante.ValorVec = (contaReceber.ValorVec + contaReceber.Juros) - contaReceber.TotalPago.GetValueOrDefault();
                contaReceberRestante.DataVec = contaReceber.DataVec;
                contaReceberRestante.Recebida = false;
                contaReceberRestante.Obs = contaReceber.Obs;
                contaReceberRestante.NumParc = contaReceber.NumParc;
                contaReceberRestante.NumParcMax = contaReceber.NumParcMax;
                contaReceberRestante.IdFormaPagto = contaReceber.IdFormaPagto;
                contaReceberRestante.DataPrimNeg = contaReceber.DataPrimNeg;
                contaReceberRestante.TipoConta = contaReceber.TipoConta;
                contaReceberRestante.IdConta = idContaAntigo ?? contaReceber.IdConta;
                contaReceberRestante.IdCliente = contaReceber.IdCliente;
                contaReceberRestante.IdFormaPagto = contaReceber.IdFormaPagto;
                CopiaReferencias(session, contaReceber, ref contaReceberRestante);
                retorno.idContaParcial = InsertBase(session, contaReceberRestante);

                // Atualiza a referência do idNf, pois como ele é "input", não é salvo
                objPersistence.ExecuteCommand(session, string.Format("UPDATE contas_receber SET IdNf={0} WHERE IdContaR={1};",
                    contaReceberRestante.IdNf == null ? "Null" : contaReceberRestante.IdNf.ToString(), retorno.idContaParcial));
            }

            #endregion

            #region Geração da comissão dos pedidos

            try
            {
                if (contaReceber.DescontarComissao.GetValueOrDefault())
                {
                    ComissaoDAO.Instance.GerarComissao(session, Pedido.TipoComissao.Comissionado, pedido.IdComissionado.Value, pedido.IdPedido.ToString(), pedido.DataConf.Value.ToString(), pedido.DataConf.Value.ToString(), 0, null);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao gerar comissão do comissionado.", ex));
            }

            #endregion

            #region Montagem da mensagem de retorno

            var mensagemRetorno = "Valor recebido. ";

            if (retorno != null)
            {
                if (retorno.creditoGerado > 0)
                {
                    mensagemRetorno += string.Format("Foi gerado {0} de crédito para o cliente. ", retorno.creditoGerado.ToString("C"));
                }

                if (retorno.creditoDebitado)
                {
                    mensagemRetorno += string.Format("Foi utilizado {0} de crédito do cliente, restando {1} de crédito. ",
                        creditoUtilizado.ToString("C"), ClienteDAO.Instance.GetCredito(idCliente).ToString("C"));
                }
            }

            #endregion

            var contasReceber = new[] { contaReceber };
            PreencheLocalizacao(session, ref contasReceber);

            return mensagemRetorno;
        }

        /// <summary>
        /// Cancela o pré recebimento da conta a receber. Caso a conta tenha sido recebida, este método não pode ser utilizado.
        /// </summary>
        public void CancelarPreRecebimentoContaComTransacao(DateTime dataEstornoBanco, int idContaR, string motivo)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    CancelarPreRecebimentoConta(transaction, dataEstornoBanco, idContaR, motivo);

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("CancelarPreRecebimentoContaComTransacao - ID conta receber: {0}.", idContaR), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao cancelar o pré recebimento da conta.", ex));
                }
            }
        }

        /// <summary>
        /// Cancela o pré recebimento da conta a receber. Caso a conta tenha sido recebida, este método não pode ser utilizado.
        /// </summary>
        public void CancelarPreRecebimentoConta(GDASession session, DateTime dataEstornoBanco, int idContaR, string motivo)
        {
            #region Declaração de variáveis

            // Busca a conta a receber.
            var contaReceber = GetElement(session, (uint)idContaR);

            #endregion

            #region Validações de permissão

            // Apenas financeiro e caixa diário podem cancelar contas recebidas.
            if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) && !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
            {
                throw new Exception("Você não tem permissão para cancelar contas recebidas.");
            }

            #endregion

            #region Validações dos dados da conta a receber

            // Verifica se esta conta já foi cancelada.
            if (!contaReceber.Recebida)
            {
                throw new Exception("Esta conta já foi cancelada.");
            }

            // contaReceber se esta conta pertence a um acerto.
            if (contaReceber.IdAcerto > 0)
            {
                throw new Exception(string.Format("Este recebimento pertence ao acerto n.º {0}, cancele-o para cancelar esta conta.", contaReceber.IdAcerto));
            }

            // Verifica se esta conta pertence a uma obra e se a obra é à vista.
            if (contaReceber.IdObra.GetValueOrDefault() > 0 && ObraDAO.Instance.ObtemTipoPagto(session, contaReceber.IdObra.Value) == (int)Obra.TipoPagtoObra.AVista)
            {
                throw new Exception(string.Format("Este recebimento pertence ao crédito/pagamento antecipado n.º {0}, cancele-o para cancelar esta conta.", contaReceber.IdObra));
            }

            if (contaReceber.IdPedido > 0 && !contaReceber.ValorExcedentePCP && PedidoDAO.Instance.ObtemTipoVenda(session, contaReceber.IdPedido.Value) == (int)Pedido.TipoVendaPedido.AVista)
            {
                throw new Exception("Esta conta recebida não pode ser cancelada, pois a mesma foi gerada a partir de um pedido à vista, é necessário cancelar o pedido.");
            }

            if (contaReceber.IdLiberarPedido > 0 && LiberarPedidoDAO.Instance.IsLiberacaoAVista(session, contaReceber.IdLiberarPedido.Value))
            {
                throw new Exception("Esta conta recebida não pode ser cancelada pois a mesma foi gerada a partir de uma liberação de pedido à vista.");
            }

            if (contaReceber.IdLiberarPedido > 0 && contaReceber.IdFormaPagto > 0 && contaReceber.IdFormaPagto != (uint)Pagto.FormaPagto.Prazo &&
                UtilsPlanoConta.GetPlanoSinal(contaReceber.IdFormaPagto.Value) == contaReceber.IdConta)
            {
                throw new Exception("Esta conta recebida não pode ser cancelada pois a mesma foi gerada a partir de um pagamento de entrada de uma liberação de pedidos à prazo.");
            }

            #endregion

            #region Remoção dos dados de pagamento da conta a receber

            // Apaga o pagto da conta recebida.
            PagtoContasReceberDAO.Instance.DeleteByIdContaR(session, (uint)idContaR);
            // Exclui a referência dos cheques utilizados no recebimento da conta.
            ChequesContasReceberDAO.Instance.ExcluirPelaContaReceber(session, idContaR);

            #endregion

            #region Atualização dos dados da conta a receber

            // Volta esta conta a receber para não recebida.
            contaReceber.Recebida = false;
            contaReceber.UsuRec = null;
            contaReceber.DataRec = null;
            contaReceber.ValorRec = 0;
            contaReceber.Juros = 0;
            contaReceber.NumAutConstrucard = null;
            contaReceber.IdLojaRecebimento = null;
            contaReceber.DescontarComissao = null;
            contaReceber.RecebimentoParcial = null;
            contaReceber.RecebimentoCaixaDiario = null;
            contaReceber.RecebimentoGerarCredito = null;

            Update(session, contaReceber);

            #endregion

            LogCancelamentoDAO.Instance.LogContaReceber(contaReceber, "Cancelamento do recebimento", true);
        }

        #endregion

        #region Recebimento de acerto

        /// <summary>
        /// Cria o acerto e recebe as contas dele.
        /// </summary>
        public string ReceberAcerto(bool caixaDiario, decimal creditoUtilizado, IEnumerable<string> dadosChequesRecebimento, DateTime dataRecebimento, bool descontarComissao, bool gerarCredito,
            int idCliente, IEnumerable<int> idsCartaoNaoIdentificado, IEnumerable<int> idsContaBanco, IEnumerable<int> idsContaReceber, IEnumerable<int> idsDepositoNaoIdentificado,
            IEnumerable<int> idsFormaPagamento, IEnumerable<int> idsTipoCartao, decimal juros, IEnumerable<string> numerosAutorizacaoCartao, string numeroAutorizacaoConstrucard, string observacao,
            IEnumerable<int> quantidadesParcelasCartao, bool recebimentoParcial, IEnumerable<decimal> taxasAntecipacao, IEnumerable<int> tiposBoleto, decimal totalPagar,
            IEnumerable<decimal> valoresRecebimento)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var idAcerto = CriarPreRecebimentoAcerto(transaction, caixaDiario, creditoUtilizado, dadosChequesRecebimento, dataRecebimento, descontarComissao, gerarCredito, idCliente,
                        idsCartaoNaoIdentificado, idsContaBanco, idsContaReceber, idsDepositoNaoIdentificado, idsFormaPagamento, idsTipoCartao, juros, numerosAutorizacaoCartao,
                        numeroAutorizacaoConstrucard, observacao, quantidadesParcelasCartao, recebimentoParcial, taxasAntecipacao, tiposBoleto, totalPagar, valoresRecebimento);

                    var retorno = FinalizarPreRecebimentoAcerto(transaction, idAcerto);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("ReceberAcerto - IDs conta receber: {0}.", string.Join(",", idsContaReceber ?? new List<int>())), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao receber contas.", ex));
                }
            }
        }

        /// <summary>
        /// Cria o pré recebimento do acerto.
        /// </summary>
        public int CriarPreRecebimentoAcertoComTransacao(bool caixaDiario, decimal creditoUtilizado, IEnumerable<string> dadosChequesRecebimento, DateTime dataRecebimento, bool descontarComissao,
            bool gerarCredito, int idCliente, IEnumerable<int> idsCartaoNaoIdentificado, IEnumerable<int> idsContaBanco, IEnumerable<int> idsContaReceber, IEnumerable<int> idsDepositoNaoIdentificado,
            IEnumerable<int> idsFormaPagamento, IEnumerable<int> idsTipoCartao, decimal juros, IEnumerable<string> numerosAutorizacaoCartao, string numeroAutorizacaoConstrucard, string observacao,
            IEnumerable<int> quantidadesParcelasCartao, bool recebimentoParcial, IEnumerable<decimal> taxasAntecipacao, IEnumerable<int> tiposBoleto, decimal totalPagar,
            IEnumerable<decimal> valoresRecebimento)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = CriarPreRecebimentoAcerto(transaction, caixaDiario, creditoUtilizado, dadosChequesRecebimento, dataRecebimento, descontarComissao, gerarCredito, idCliente,
                        idsCartaoNaoIdentificado, idsContaBanco, idsContaReceber, idsDepositoNaoIdentificado, idsFormaPagamento, idsTipoCartao, juros, numerosAutorizacaoCartao,
                        numeroAutorizacaoConstrucard, observacao, quantidadesParcelasCartao, recebimentoParcial, taxasAntecipacao, tiposBoleto, totalPagar, valoresRecebimento);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("CriarPreRecebimentoAcertoComTransacao - IDs conta receber: {0}.", string.Join(",", idsContaReceber ?? new List<int>())), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao receber contas.", ex));
                }
            }
        }

        /// <summary>
        /// Cria o pré recebimento do acerto.
        /// </summary>
        public int CriarPreRecebimentoAcerto(GDASession session, bool caixaDiario, decimal creditoUtilizado, IEnumerable<string> dadosChequesRecebimento, DateTime dataRecebimento,
            bool descontarComissao, bool gerarCredito, int idCliente, IEnumerable<int> idsCartaoNaoIdentificado, IEnumerable<int> idsContaBanco, IEnumerable<int> idsContaReceber,
            IEnumerable<int> idsDepositoNaoIdentificado, IEnumerable<int> idsFormaPagamento, IEnumerable<int> idsTipoCartao, decimal juros, IEnumerable<string> numerosAutorizacaoCartao,
            string numeroAutorizacaoConstrucard, string observacao, IEnumerable<int> quantidadesParcelasCartao, bool recebimentoParcial, IEnumerable<decimal> taxasAntecipacao,
            IEnumerable<int> tiposBoleto, decimal totalPagar, IEnumerable<decimal> valoresRecebimento)
        {
            #region Declaração de variáveis

            var acerto = new Acerto((uint)idCliente);
            var usuarioLogado = UserInfo.GetUserInfo;
            var contadorPagamento = 0;
            decimal totalPago = 0;

            #endregion

            #region Cálculo dos totais do acerto

            totalPago = valoresRecebimento.Sum(f => f) + (creditoUtilizado > 0 ? creditoUtilizado : 0);

            if (descontarComissao)
            {
                totalPago += UtilsFinanceiro.GetValorComissao(session, string.Join(",", idsContaReceber), "ContasReceber");
            }

            // Ignora os juros dos cartões ao calcular o valor pago/a pagar.
            totalPago -= UtilsFinanceiro.GetJurosCartoes(session, usuarioLogado.IdLoja, valoresRecebimento.ToArray(), idsFormaPagamento.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(),
                idsTipoCartao.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(), quantidadesParcelasCartao.Select(f => ((uint?)f).GetValueOrDefault()).ToArray());

            #endregion

            #region Atualização dos dados do acerto

            acerto.DataCad = DateTime.Now;
            acerto.UsuCad = usuarioLogado.CodUser;
            acerto.ValorCreditoAoCriar = ClienteDAO.Instance.GetCredito(session, acerto.IdCli);
            acerto.Obs = observacao;
            acerto.NumAutConstrucard = string.IsNullOrWhiteSpace(numeroAutorizacaoConstrucard) ? acerto.NumAutConstrucard : numeroAutorizacaoConstrucard;
            acerto.TotalPagar = totalPagar;
            acerto.TotalPago = totalPago;
            acerto.JurosRecebimento = juros;
            acerto.DataRecebimento = dataRecebimento;
            acerto.IdLojaRecebimento = (int)usuarioLogado.IdLoja;
            acerto.DescontarComissao = descontarComissao;
            acerto.RecebimentoParcial = recebimentoParcial;
            acerto.RecebimentoCaixaDiario = caixaDiario;
            acerto.RecebimentoGerarCredito = gerarCredito;

            #endregion

            #region Validações do recebimento

            ValidarRecebimentoAcerto(session, acerto, creditoUtilizado, idsCartaoNaoIdentificado, idsContaBanco, idsContaReceber, idsFormaPagamento, idsTipoCartao, quantidadesParcelasCartao,
                valoresRecebimento);

            #endregion

            #region Inserção do acerto

            acerto.Situacao = (int)Acerto.SituacaoEnum.Processando;
            acerto.IdAcerto = AcertoDAO.Instance.Insert(session, acerto);

            #endregion

            #region Atualização das contas a receber

            objPersistence.ExecuteCommand(session, string.Format("UPDATE contas_receber SET IdAcerto={0} WHERE IdContaR IN ({1});", acerto.IdAcerto, string.Join(",", idsContaReceber)));

            #endregion

            #region Inserção dos pagamentos do acerto

            ChequesAcertoDAO.Instance.InserirPelaString(session, acerto, dadosChequesRecebimento);

            try
            {
                // Insere as formas de pagamento.
                for (var i = 0; i < valoresRecebimento.Count(); i++)
                {
                    if (idsFormaPagamento.ElementAtOrDefault(i) == 0 || valoresRecebimento.ElementAtOrDefault(i) == 0)
                    {
                        continue;
                    }

                    if (idsFormaPagamento.Count() > i && idsFormaPagamento.ElementAtOrDefault(i) == (int)Pagto.FormaPagto.CartaoNaoIdentificado)
                    {
                        var cartoesNaoIdentificado = CartaoNaoIdentificadoDAO.Instance.ObterPeloId(session, idsCartaoNaoIdentificado.Select(f => ((uint?)f).GetValueOrDefault()).ToArray());

                        foreach (var cartaoNaoIdentificado in cartoesNaoIdentificado)
                        {
                            var dadosPagamento = new PagtoAcerto();
                            dadosPagamento.IdAcerto = acerto.IdAcerto;
                            dadosPagamento.NumFormaPagto = ++contadorPagamento;
                            dadosPagamento.IdFormaPagto = (uint)idsFormaPagamento.ElementAt(i);
                            dadosPagamento.ValorPagto = cartaoNaoIdentificado.Valor;
                            dadosPagamento.IdContaBanco = (uint)cartaoNaoIdentificado.IdContaBanco;
                            dadosPagamento.IdCartaoNaoIdentificado = cartaoNaoIdentificado.IdCartaoNaoIdentificado;
                            dadosPagamento.IdTipoCartao = (uint)cartaoNaoIdentificado.TipoCartao;
                            dadosPagamento.NumAutCartao = cartaoNaoIdentificado.NumAutCartao;

                            PagtoAcertoDAO.Instance.Insert(session, dadosPagamento);
                        }
                    }
                    else if (idsFormaPagamento.Count() > i && idsDepositoNaoIdentificado.ElementAtOrDefault(i) > 0)
                    {
                        var idContabancoDepositoNaoIdentificado = DepositoNaoIdentificadoDAO.Instance.ObtemIdContaBanco(session, idsDepositoNaoIdentificado.ElementAtOrDefault(i));

                        PagtoAcerto dadosPagamento = new PagtoAcerto();
                        dadosPagamento.IdAcerto = acerto.IdAcerto;
                        dadosPagamento.NumFormaPagto = ++contadorPagamento;
                        dadosPagamento.IdFormaPagto = (uint)idsFormaPagamento.ElementAt(i);
                        dadosPagamento.IdTipoCartao = idsTipoCartao.ElementAtOrDefault(i) > 0 ? (uint)idsTipoCartao.ElementAt(i) : (uint?)null;
                        dadosPagamento.ValorPagto = valoresRecebimento.ElementAt(i);
                        dadosPagamento.IdContaBanco = (uint)idContabancoDepositoNaoIdentificado;

                        PagtoAcertoDAO.Instance.Insert(session, dadosPagamento);
                    }
                    else
                    {
                        var pagamentoAcerto = new PagtoAcerto();
                        pagamentoAcerto.IdAcerto = acerto.IdAcerto;
                        pagamentoAcerto.NumFormaPagto = ++contadorPagamento;
                        pagamentoAcerto.IdFormaPagto = (uint)idsFormaPagamento.ElementAt(i);
                        pagamentoAcerto.ValorPagto = valoresRecebimento.ElementAt(i);
                        pagamentoAcerto.IdContaBanco = idsContaBanco.ElementAtOrDefault(i) > 0 ? (uint)idsContaBanco.ElementAt(i) : (uint?)null;
                        pagamentoAcerto.IdDepositoNaoIdentificado = idsDepositoNaoIdentificado.ElementAtOrDefault(i) > 0 ? idsDepositoNaoIdentificado.ElementAt(i) : (int?)null;
                        pagamentoAcerto.IdTipoCartao = idsTipoCartao.ElementAtOrDefault(i) > 0 ? (uint)idsTipoCartao.ElementAt(i) : (uint?)null;
                        pagamentoAcerto.QuantidadeParcelaCartao = pagamentoAcerto.IdTipoCartao > 0 && quantidadesParcelasCartao.ElementAtOrDefault(i) > 0 ? quantidadesParcelasCartao.ElementAt(i) : (int?)null;
                        pagamentoAcerto.TaxaAntecipacao = taxasAntecipacao.ElementAtOrDefault(i) > 0 ? taxasAntecipacao.ElementAt(i) : (decimal?)null;
                        pagamentoAcerto.TipoBoleto = tiposBoleto.ElementAtOrDefault(i) > 0 ? tiposBoleto.ElementAt(i) : (int?)null;
                        pagamentoAcerto.NumAutCartao = !string.IsNullOrWhiteSpace(numerosAutorizacaoCartao.ElementAtOrDefault(i)) ? numerosAutorizacaoCartao.ElementAt(i) : null;

                        PagtoAcertoDAO.Instance.Insert(session, pagamentoAcerto);
                    }
                }

                if (creditoUtilizado > 0)
                {
                    var dadosPagamento = new PagtoAcerto();
                    dadosPagamento.IdAcerto = acerto.IdAcerto;
                    dadosPagamento.NumFormaPagto = ++contadorPagamento;
                    dadosPagamento.IdFormaPagto = (uint)Pagto.FormaPagto.Credito;
                    dadosPagamento.ValorPagto = creditoUtilizado;

                    PagtoAcertoDAO.Instance.Insert(session, dadosPagamento);
                }
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("Falha ao salvar pagto do acerto.", ex);
                throw ex;
            }

            #endregion

            return (int)acerto.IdAcerto;
        }

        /// <summary>
        /// Valida o pré recebimento do acerto.
        /// </summary>
        private void ValidarRecebimentoAcerto(GDASession session, Acerto acerto, decimal creditoUtilizado, IEnumerable<int> idsCartaoNaoIdentificado, IEnumerable<int> idsContaBanco,
            IEnumerable<int> idsContaReceber, IEnumerable<int> idsFormaPagamento, IEnumerable<int> idsTipoCartao, IEnumerable<int> quantidadesParcelasCartao, IEnumerable<decimal> valoresRecebimento)
        {
            #region Declaração de variáveis

            Pedido[] pedidosParaComissao;
            var contasReceber = GetByPks(session, string.Join(",", idsContaReceber));
            var idLojaContaReceberComparar = 0;
            var jurosRecebimento = acerto.JurosRecebimento.GetValueOrDefault();
            var totalPagar = acerto.TotalPagar.GetValueOrDefault();
            var totalPago = acerto.TotalPago.GetValueOrDefault();
            var idLojaRecebimento = acerto.IdLojaRecebimento.GetValueOrDefault();
            var recebimentoGerarCredito = acerto.RecebimentoGerarCredito.GetValueOrDefault();
            var recebimentoCaixaDiario = acerto.RecebimentoCaixaDiario.GetValueOrDefault();
            var recebimentoParcial = acerto.RecebimentoParcial.GetValueOrDefault();

            #endregion

            #region Validações do cliente

            // Chamado: 29294 - Gerou um acerto sem cliente.
            if (acerto.IdCli <= 0)
            {
                throw new Exception("Nenhum cliente foi informado");
            }

            #endregion

            #region Validações de comissão

            if (acerto.DescontarComissao.GetValueOrDefault())
            {
                pedidosParaComissao = UtilsFinanceiro.GetPedidosForComissao(session, string.Join(",", idsContaReceber), "ContasReceber");

                // Apenas administrador, financeiro geral e financeiro pagto podem gerar comissões
                if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento) && (pedidosParaComissao?.Any(f => f.IdComissionado > 0) ?? false))
                {
                    throw new Exception("Você não tem permissão para gerar comissões.");
                }
            }

            #endregion

            #region Validações de permissão

            // Se não for caixa diário ou financeiro, não pode receber sinal
            if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) && !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
            {
                throw new Exception("Você não tem permissão para receber contas.");
            }

            #endregion

            #region Validações dos totais e das formas de pagamento

            // Verifica se a forma de pagamento foi selecionada.
            if (Math.Round(creditoUtilizado, 2, MidpointRounding.AwayFromZero) < Math.Round(totalPagar + jurosRecebimento, 2, MidpointRounding.AwayFromZero) && idsFormaPagamento.Count() == 0)
            {
                throw new Exception("Informe a forma de pagamento.");
            }

            // Mesmo se for recebimento parcial, não é permitido receber valor maior do que o valor a ser pago.
            if (recebimentoParcial)
            {
                if (Math.Round(totalPago - jurosRecebimento, 2, MidpointRounding.AwayFromZero) > Math.Round(totalPagar, 2, MidpointRounding.AwayFromZero))
                    throw new Exception("Valor pago excede o valor do acerto.");
            }
            // Se o valor for inferior ao que deve ser pago, e o restante do pagto for gerarCredito, lança exceção.
            else if (recebimentoGerarCredito && Math.Round(totalPago - jurosRecebimento, 2, MidpointRounding.AwayFromZero) < Math.Round(totalPagar, 2, MidpointRounding.AwayFromZero))
            {
                throw new Exception(string.Format("Total a ser pago não confere com valor pago. Total a ser pago: {0} Valor pago: {1}.",
                    Math.Round(totalPagar + jurosRecebimento, 2, MidpointRounding.AwayFromZero).ToString("C"), totalPago.ToString("C")));
            }
            // Se o total a ser pago for diferente do valor pago, considerando que não é para gerar crédito.
            else if (!recebimentoGerarCredito && Math.Round(totalPago - jurosRecebimento, 2, MidpointRounding.AwayFromZero) != Math.Round(totalPagar, 2, MidpointRounding.AwayFromZero))
            {
                throw new Exception(string.Format("Total a ser pago não confere com valor pago. Total a ser pago: {0} Valor pago: {1}.",
                    Math.Round(totalPagar + jurosRecebimento, 2, MidpointRounding.AwayFromZero).ToString("C"), totalPago.ToString("C")));
            }

            #endregion

            #region Validações do recebimento das contas do acerto

            // Verifica se as contas já foram recebidas.
            if (contasReceber.Any(f => f.Recebida))
            {
                throw new Exception("Uma das contas a receber já foi recebida.");
            }

            // Chamado 16618 (Extremamente importante): Valida se total de contas a receber bate com o total a ser pago vindo da tela.
            if (!acerto.RecebimentoParcial.GetValueOrDefault() && Math.Round(contasReceber.Sum(f => f.ValorVec), 2) != Math.Round(totalPagar, 2))
            {
                throw new Exception("Tela expirada, faça login novamente no sistema.");
            }

            /* Chamado 50083. */
            if (FinanceiroConfig.ContasReceber.UtilizarControleContaReceberJuridico &&
                (contasReceber.Count() != contasReceber.Count(f => f.Juridico) && contasReceber.Count() != contasReceber.Count(f => !f.Juridico)))
            {
                throw new Exception("Todas as contas devem estar marcadas como Jurídico/Cartório ou nenhuma delas deve estar marcada como Jurídico/Cartório para efetuar o acerto.");
            }

            UtilsFinanceiro.ValidarRecebimento(session, recebimentoCaixaDiario, (int)acerto.IdCli, idLojaRecebimento, idsCartaoNaoIdentificado, idsContaBanco, idsFormaPagamento,
                recebimentoGerarCredito, jurosRecebimento, recebimentoParcial, UtilsFinanceiro.TipoReceb.Acerto, totalPago, totalPagar);

            #endregion

            #region Validações das lojas das contas a receber

            // Se usar o controle de comissão de contas recebidas não pode fazer acerto parcial de contas de lojas diferentes.
            if (recebimentoParcial && ComissaoDAO.Instance.VerificarComissaoContasRecebidas())
            {
                foreach (var idContaReceber in idsContaReceber)
                {
                    var idLojaContaReceber = ObtemIdLoja(session, (uint)idContaReceber);

                    if (idLojaContaReceberComparar == 0)
                    {
                        idLojaContaReceberComparar = (int)idLojaContaReceber;
                    }
                    else if (idLojaContaReceberComparar != idLojaContaReceber)
                    {
                        throw new Exception("Não é possivel fazer o recebimento parcial de contas de lojas diferentes.");
                    }
                }
            }

            #endregion

            #region Validações das contas a receber

            if (contasReceber.Count() != idsContaReceber.Count())
            {
                throw new Exception("Uma das contas a receber inserida não existe mais, possivelmente foi renegociada por outra pessoa.");
            }

            // Chamados 14293 e 14365.
            // Foram feitos acertos com contas a receber de clientes diferentes, que não possuíam vínculo. A verificação abaixo evitará que isso ocorra novamente.
            if (contasReceber.Any(f => f.IdCliente != acerto.IdCli))
            {
                foreach (var contaReceber in contasReceber.Where(f => f.IdCliente != acerto.IdCli))
                {
                    var idsClienteVinculado = ClienteVinculoDAO.Instance.GetIdsVinculados(session, acerto.IdCli);
                    var possuiVinculo = string.IsNullOrWhiteSpace(idsClienteVinculado) ? false : idsClienteVinculado.Split(',').Any(f => f.StrParaUintNullable().GetValueOrDefault() == contaReceber.IdCliente);

                    if (!possuiVinculo)
                    {
                        throw new Exception("Não é possível efetuar acerto de contas a receber de clientes diferentes que não estão vinculados.");
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// Finaliza o pré recebimento do acerto.
        /// </summary>
        public string FinalizarPreRecebimentoAcertoComTransacao(int idAcerto)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = FinalizarPreRecebimentoAcerto(transaction, idAcerto);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("FinalizarPreRecebimentoAcerto - ID acerto: {0}.", idAcerto), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao finalizar o recebimento das contas.", ex));
                }
            }
        }

        /// <summary>
        /// Finaliza o pré recebimento do acerto.
        /// </summary>
        public string FinalizarPreRecebimentoAcerto(GDASession session, int idAcerto)
        {
            #region Declaração de variáveis

            UtilsFinanceiro.DadosRecebimento retorno = null;
            var acerto = AcertoDAO.Instance.GetElementByPrimaryKey(session, idAcerto);
            var usuarioLogado = UserInfo.GetUserInfo;
            var contasReceber = GetByAcerto(session, (uint)idAcerto, false);
            var contadorPagamento = 0;
            var totalPagar = acerto.TotalPagar.GetValueOrDefault();
            var totalPago = acerto.TotalPago.GetValueOrDefault();
            var jurosRecebimento = acerto.JurosRecebimento.GetValueOrDefault();
            var dataRecebimento = acerto.DataRecebimento.GetValueOrDefault(DateTime.Now).ToString("dd/MM/yyyy");
            var descontarComissao = acerto.DescontarComissao.GetValueOrDefault();
            var recebimentoCaixaDiario = acerto.RecebimentoCaixaDiario.GetValueOrDefault();
            var recebimentoGerarCredito = acerto.RecebimentoGerarCredito.GetValueOrDefault();
            var recebimentoParcial = acerto.RecebimentoParcial.GetValueOrDefault();
            // Recupera os cheques que foram selecionados no momento do recebimento da liberação.
            var chequesRecebimento = ChequesAcertoDAO.Instance.ObterStringChequesPeloAcerto(session, idAcerto);
            var pagamentosAcerto = PagtoAcertoDAO.Instance.GetByAcerto(session, (uint)idAcerto);
            // Variáveis criadas para recuperar os dados do pagamento do acerto.
            var idsCartaoNaoIdentificado = new List<int?>();
            var idsContaBanco = new List<int?>();
            var idsDepositoNaoIdentificado = new List<int?>();
            var idsFormaPagamento = new List<int>();
            var idsTipoCartao = new List<int?>();
            var quantidadesParcelaCartao = new List<int?>();
            var taxasAntecipacao = new List<decimal?>();
            var tiposBoleto = new List<int?>();
            var valoresRecebimento = new List<decimal>();
            decimal creditoUtilizado = 0;

            #endregion

            #region Recuperação dos dados de recebimento da liberação

            if (pagamentosAcerto.Any(f => f.IdFormaPagto != (uint)Pagto.FormaPagto.Credito && f.IdFormaPagto != (uint)Pagto.FormaPagto.Obra))
            {
                foreach (var pagamentoAcerto in pagamentosAcerto.Where(f => f.IdFormaPagto != (uint)Pagto.FormaPagto.Credito && f.IdFormaPagto != (uint)Pagto.FormaPagto.Obra)
                    .OrderBy(f => f.NumFormaPagto))
                {
                    idsCartaoNaoIdentificado.Add(pagamentoAcerto.IdCartaoNaoIdentificado.GetValueOrDefault());
                    idsContaBanco.Add((int)pagamentoAcerto.IdContaBanco.GetValueOrDefault());
                    idsDepositoNaoIdentificado.Add(pagamentoAcerto.IdDepositoNaoIdentificado.GetValueOrDefault());
                    idsFormaPagamento.Add((int)pagamentoAcerto.IdFormaPagto);
                    idsTipoCartao.Add((int?)pagamentoAcerto.IdTipoCartao);
                    quantidadesParcelaCartao.Add(pagamentoAcerto.QuantidadeParcelaCartao.GetValueOrDefault());
                    taxasAntecipacao.Add(pagamentoAcerto.TaxaAntecipacao);
                    tiposBoleto.Add(pagamentoAcerto.TipoBoleto);
                    valoresRecebimento.Add(pagamentoAcerto.ValorPagto);
                }
            }

            if (pagamentosAcerto.Any(f => f.IdFormaPagto == (uint)Pagto.FormaPagto.Credito))
            {
                creditoUtilizado = (pagamentosAcerto.FirstOrDefault(f => f.IdFormaPagto == (uint)Pagto.FormaPagto.Credito)?.ValorPagto).GetValueOrDefault();
            }

            #endregion

            #region Validações do pré recebimento do acerto

            if (contasReceber == null || contasReceber.Count() == 0)
            {
                throw new Exception("Não foi possível recuperar as contas a receber para efetuar o acerto. Tente novamente. Caso esta mensagem persista, entre em contato com o suporte do software WebGlass.");
            }

            #endregion

            #region Recebimento das contas

            retorno = UtilsFinanceiro.Receber(session, (uint)acerto.IdLojaRecebimento, null, null, null, null, acerto, contasReceber.ToArray(), null,
                string.Join(",", contasReceber.Select(f => f.IdContaR).ToList()), null, null, null, acerto.IdCli, 0, null, dataRecebimento, totalPagar, totalPago, valoresRecebimento.ToArray(),
                idsFormaPagamento.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(), idsContaBanco.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(),
                idsDepositoNaoIdentificado.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(), idsCartaoNaoIdentificado.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(),
                idsTipoCartao.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(), tiposBoleto.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(),
                taxasAntecipacao.Select(f => f.GetValueOrDefault()).ToArray(), jurosRecebimento, recebimentoParcial, recebimentoGerarCredito, creditoUtilizado, acerto.NumAutConstrucard,
                recebimentoCaixaDiario, quantidadesParcelaCartao.Select(f => ((uint?)f).GetValueOrDefault()).ToArray(), chequesRecebimento, descontarComissao, UtilsFinanceiro.TipoReceb.Acerto);

            if (retorno.ex != null)
            {
                throw retorno.ex;
            }

            #endregion

            #region Atualização dos dados do acerto

            acerto.Situacao = (int)Acerto.SituacaoEnum.Aberto;
            acerto.CreditoUtilizadoCriar = creditoUtilizado;
            acerto.CreditoGeradoCriar = retorno.creditoGerado;

            AcertoDAO.Instance.Update(session, acerto);

            #endregion

            #region Inserção dos pagamentos das contas a receber e geração das contas restantes

            // Utilizado para controlar qual forma de pagto será utilizada para receber conta.
            contadorPagamento = -1;

            if (!recebimentoParcial)
            {
                #region Recebimento integral

                foreach (var contaReceber in contasReceber?.Where(f => !f.Recebida).ToList())
                {
                    // Seleciona a próxima forma de pagamento válida
                    if (idsFormaPagamento.Count() > 0)
                    {
                        while (idsFormaPagamento.ElementAtOrDefault(++contadorPagamento % idsFormaPagamento.Count()) == 0)
                        {
                            if (contadorPagamento >= idsFormaPagamento.Count())
                            {
                                contadorPagamento = -1;
                                break;
                            }
                        }
                    }

                    if (contadorPagamento > -1)
                    {
                        contadorPagamento = contadorPagamento % idsFormaPagamento.Count();
                    }

                    // Atualiza esta conta a receber.
                    contaReceber.UsuRec = usuarioLogado.CodUser;
                    contaReceber.ValorRec = contaReceber.ValorVec;
                    contaReceber.Recebida = true;
                    contaReceber.IdConta = contadorPagamento > -1 ?
                        (uint?)(idsFormaPagamento.ElementAtOrDefault(contadorPagamento) == (uint)Pagto.FormaPagto.Cartao ?
                            UtilsPlanoConta.GetPlanoRecebTipoCartao((uint?)idsTipoCartao.ElementAtOrDefault(contadorPagamento) ?? 0) :
                            idsFormaPagamento.ElementAtOrDefault(contadorPagamento) == (uint)Pagto.FormaPagto.Boleto ?
                                UtilsPlanoConta.GetPlanoRecebTipoBoleto((uint?)tiposBoleto.ElementAtOrDefault(contadorPagamento) ?? 0) :
                                UtilsPlanoConta.GetPlanoReceb((uint?)idsFormaPagamento.ElementAtOrDefault(contadorPagamento) ?? 0)) :
                                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecPrazoCredito);
                    contaReceber.IdAcerto = acerto.IdAcerto;
                    contaReceber.DataRec = acerto.DataRecebimento.GetValueOrDefault(DateTime.Now);

                    if (contaReceber.DataRec.Value.Hour == 0)
                    {
                        contaReceber.DataRec.Value.AddHours(DateTime.Now.Hour);
                        contaReceber.DataRec.Value.AddMinutes(DateTime.Now.Minute);
                        contaReceber.DataRec.Value.AddSeconds(DateTime.Now.Hour);
                    }

                    Update(session, contaReceber);
                }

                #endregion
            }
            else
            {
                #region Recebimento parcial

                var contasReceberParcial = new List<ContasReceber>(contasReceber);
                // Verifica até quando parcelas poderão ser quitadas.
                var totalPagoRestante = totalPago - jurosRecebimento;
                // Define que já foi tudo pago e que as próximas contas da iteração devem ser removidas da listagem, para gerar os pagtos de cada conta a receber do acerto corretamente.
                var removerProximas = false;

                // Ordena as contas recebidas de forma a quitar primeiro as mais antigas.
                contasReceberParcial.Sort(new Comparison<ContasReceber>(delegate (ContasReceber x, ContasReceber y)
                {
                    return Comparer<DateTime>.Default.Compare(x.DataVec, y.DataVec);
                }));

                foreach (var contaReceberParcial in contasReceberParcial.Where(f => !f.Recebida).ToList())
                {
                    var idContaAntiga = contaReceberParcial.IdConta;

                    if (removerProximas)
                    {
                        contasReceberParcial.Remove(contaReceberParcial);
                        continue;
                    }

                    // Seleciona a próxima forma de pagamento válida.
                    if (idsFormaPagamento.Count() > 0)
                    {
                        while (idsFormaPagamento[++contadorPagamento % idsFormaPagamento.Count()] == 0)
                        {
                            if (contadorPagamento >= idsFormaPagamento.Count())
                            {
                                contadorPagamento = -1;
                                break;
                            }
                        }
                    }

                    if (contadorPagamento > -1)
                    {
                        contadorPagamento = contadorPagamento % idsFormaPagamento.Count();
                    }

                    // Atualiza esta conta a receber. Se o total pago pelo cliente restante, for menor que o valor desta conta, recebe apenas este restante.
                    contaReceberParcial.ValorRec = totalPagoRestante > contaReceberParcial.ValorVec ? contaReceberParcial.ValorVec : totalPagoRestante;
                    contaReceberParcial.UsuRec = usuarioLogado.CodUser;
                    contaReceberParcial.Recebida = true;
                    contaReceberParcial.IdConta = contadorPagamento > -1 ? (uint?)(idsFormaPagamento.ElementAtOrDefault(contadorPagamento) == (uint)Pagto.FormaPagto.Cartao ?
                        UtilsPlanoConta.GetPlanoRecebTipoCartao(((uint?)idsTipoCartao.ElementAtOrDefault(contadorPagamento)).GetValueOrDefault()) :
                        idsFormaPagamento.ElementAtOrDefault(contadorPagamento) == (uint)Pagto.FormaPagto.Boleto ?
                            UtilsPlanoConta.GetPlanoRecebTipoBoleto(((uint?)tiposBoleto.ElementAtOrDefault(contadorPagamento)).GetValueOrDefault()) :
                            UtilsPlanoConta.GetPlanoReceb(((uint?)idsFormaPagamento.ElementAtOrDefault(contadorPagamento)).GetValueOrDefault())) :
                            UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecPrazoCredito);
                    contaReceberParcial.IdAcerto = acerto.IdAcerto;
                    contaReceberParcial.DataRec = acerto.DataRecebimento.GetValueOrDefault(DateTime.Now);

                    if (contaReceberParcial.DataRec.Value.Hour == 0)
                    {
                        contaReceberParcial.DataRec.Value.AddHours(DateTime.Now.Hour);
                        contaReceberParcial.DataRec.Value.AddMinutes(DateTime.Now.Minute);
                        contaReceberParcial.DataRec.Value.AddSeconds(DateTime.Now.Hour);
                    }

                    Update(session, contaReceberParcial);

                    // Se o valor restante não der para pagar a conta toda, recebe parcial
                    // e sai do loop, para não receber mais nenhuma conta
                    if (totalPagoRestante < contaReceberParcial.ValorVec)
                    {
                        // Insere outra parcela contendo o valor restante que deverá ser recebido
                        var contaReceberRestante = new ContasReceber();
                        contaReceberRestante.IdLoja = contaReceberParcial.IdLoja;
                        contaReceberRestante.ValorVec = contaReceberParcial.ValorVec - contaReceberParcial.ValorRec;
                        contaReceberRestante.DataVec = contaReceberParcial.DataVec;
                        contaReceberRestante.Recebida = false;

                        /* Chamado 50083. */
                        if (FinanceiroConfig.ContasReceber.UtilizarControleContaReceberJuridico)
                        {
                            contaReceberRestante.Juridico = contasReceber.Count(f => !f.Juridico) == 0;
                        }

                        contaReceberRestante.IdPedido = contaReceberParcial.IdPedido;
                        // Silmara pediu para sair o pedido no restante do acerto
                        contaReceberRestante.IdLiberarPedido = contaReceberParcial.IdLiberarPedido;
                        contaReceberRestante.IdCliente = acerto.IdCli;
                        contaReceberRestante.IdFormaPagto = contaReceberParcial.IdFormaPagto;
                        contaReceberRestante.IdAcerto = contaReceberParcial.IdAcertoParcial;
                        contaReceberRestante.IdAcertoParcial = acerto.IdAcerto;
                        contaReceberRestante.IdConta = idContaAntiga;
                        contaReceberRestante.IdFormaPagto = contaReceberParcial.IdFormaPagto;
                        contaReceberRestante.DataPrimNeg = contaReceberParcial.DataPrimNeg;
                        contaReceberRestante.TipoConta = contasReceber.FirstOrDefault(f => f.TipoConta > 0)?.TipoConta ?? new byte();
                        contaReceberRestante.IdFuncComissaoRec = contasReceber.First().IdFuncComissaoRec;
                        retorno.idContaParcial = Insert(session, contaReceberRestante);

                        removerProximas = true;
                        continue;
                    }

                    // Debita valor desta conta do total que o cliente pagou
                    totalPagoRestante -= contaReceberParcial.ValorVec;
                }

                // Necessário para inserir corretamente os pagtos de cada conta a receber abaixo
                contasReceber = contasReceberParcial.ToArray();

                #endregion
            }

            #endregion

            #region Inserção dos pagamentos das contas do acerto

            try
            {
                // ATENÇÃO: Chamado 30337: Deve ser feito depois de remover as contas a receber que não puderam ser pagas (considerando que o acerto tenha sido parcial)
                // pois esta função insere pagto para todas as contas a receber informadas na tela.
                var valoresRecebimentoRateio = (decimal[])valoresRecebimento.ToArray().Clone();
                var creditoUtilizadoRateio = creditoUtilizado;

                // Salva as formas de pagto de cada conta do acerto
                for (var i = 0; i < contasReceber.Count(); i++)
                {
                    var contaReceber = contasReceber[i];
                    var ultimaConta = i + 1 == contasReceber.Count();
                    // Obtém o percentual que esta conta representa em todo o acerto.
                    var percentual = contasReceber.Count() == 1 ? 100 : (contaReceber.ValorVec * 100) / (recebimentoParcial ? acerto.TotalPago : acerto.TotalPagar);

                    // Salva as formas de pagto usadas no acerto rateando pelas contas quitadas
                    for (var j = 0; j < valoresRecebimento.Count(); j++)
                    {
                        if (idsFormaPagamento.ElementAtOrDefault(j) == 0 || valoresRecebimento.ElementAtOrDefault(j) == 0)
                        {
                            continue;
                        }

                        var pagamentoContaReceber = new PagtoContasReceber();
                        var valorPagamento = (valoresRecebimento.ElementAtOrDefault(j) * percentual) / 100;

                        pagamentoContaReceber.IdContaR = contaReceber.IdContaR;
                        pagamentoContaReceber.IdFormaPagto = (uint)idsFormaPagamento[j];
                        pagamentoContaReceber.IdContaBanco = idsFormaPagamento[j] != (uint)Pagto.FormaPagto.Dinheiro && idsContaBanco.ElementAtOrDefault(j) > 0 ? (uint?)idsContaBanco.ElementAt(j) : null;
                        pagamentoContaReceber.IdTipoCartao = idsTipoCartao.ElementAtOrDefault(j) > 0 ? (uint)idsTipoCartao.ElementAt(j) : (uint?)null;
                        pagamentoContaReceber.IdDepositoNaoIdentificado = idsDepositoNaoIdentificado.ElementAtOrDefault(j) > 0 ? (uint)idsDepositoNaoIdentificado.ElementAt(j) : (uint?)null;
                        // Se for a última conta, salva o valor restante do pagto.
                        pagamentoContaReceber.ValorPagto = ultimaConta ? valoresRecebimentoRateio[j] : valorPagamento.GetValueOrDefault();
                        valoresRecebimentoRateio[j] -= valorPagamento.GetValueOrDefault();

                        PagtoContasReceberDAO.Instance.Insert(session, pagamentoContaReceber);
                    }

                    if (acerto.CreditoUtilizadoCriar > 0)
                    {
                        var pagamentoContaReceber = new PagtoContasReceber();
                        var valorPagamento = (creditoUtilizado * percentual) / 100;

                        pagamentoContaReceber.IdContaR = contaReceber.IdContaR;
                        pagamentoContaReceber.IdFormaPagto = (uint)Pagto.FormaPagto.Credito;
                        pagamentoContaReceber.ValorPagto = ultimaConta ? creditoUtilizadoRateio : valorPagamento.GetValueOrDefault();
                        creditoUtilizadoRateio -= valorPagamento.GetValueOrDefault();

                        PagtoContasReceberDAO.Instance.Insert(session, pagamentoContaReceber);
                    }
                }
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("Falha ao salvar pagto das contas do acerto.", ex);
                throw ex;
            }

            #endregion

            #region Atualização de dados do pedido

            // Libera o pedido para entrega somente se for empresa do tipo Comércio (Confirmação).
            if (!PedidoConfig.LiberarPedido)
            {
                // Recupera o id de cada pedido recebido no acerto.
                var idsPedido = ObtemIdsPedido(session, acerto.IdAcerto, false)?.Split(',')?.Where(f => f.StrParaIntNullable() > 0).Select(f => f.StrParaInt()).ToList() ?? new List<int>();

                foreach (var idPedido in idsPedido)
                {
                    // Verifica se o pedido possui alguma conta a receber que ainda não foi recebida.
                    if (!GetByPedido(session, (uint)idPedido, false, false)?.Any(f => !f.Recebida) ?? false)
                    {
                        PedidoDAO.Instance.AlteraLiberarFinanc(session, (uint)idPedido, true);
                    }
                }
            }

            #endregion

            #region Geração da comissão dos pedidos

            try
            {
                if (descontarComissao)
                {
                    var idComissionado = 0;
                    var idsPedido = new List<int>();
                    var pedidosParaComissao = UtilsFinanceiro.GetPedidosForComissao(session, string.Join(",", contasReceber.Select(f => f.IdContaR).ToList()), "ContasReceber");
                    DateTime dataInicio = DateTime.Now, dataFim = new DateTime();

                    foreach (var pedidoParaComissao in pedidosParaComissao)
                    {
                        idComissionado = (int)pedidoParaComissao.IdComissionado.Value;
                        idsPedido.Add((int)pedidoParaComissao.IdPedido);

                        if (pedidoParaComissao.DataConf != null)
                        {
                            if (pedidoParaComissao.DataConf.Value < dataInicio)
                            {
                                dataInicio = pedidoParaComissao.DataConf.Value;
                            }

                            if (pedidoParaComissao.DataConf.Value > dataFim)
                            {
                                dataFim = pedidoParaComissao.DataConf.Value;
                            }
                        }
                    }

                    if (dataFim < dataInicio)
                    {
                        dataFim = dataInicio;
                    }

                    if (idComissionado > 0)
                    {
                        ComissaoDAO.Instance.GerarComissao(session, Pedido.TipoComissao.Comissionado, (uint)idComissionado, string.Join(",", idsPedido), dataInicio.ToString(), dataFim.ToString(), 0, null);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao gerar comissão dos pedidos.", ex));
            }

            #endregion

            #region Montagem da mensagem de retorno

            var mensagemRetorno = "Valor recebido. Contas Quitadas. ";

            if (retorno != null)
            {
                if (retorno.creditoGerado > 0)
                {
                    mensagemRetorno += string.Format("Foi gerado {0} de crédito para o cliente. ", retorno.creditoGerado.ToString("C"));
                }

                if (retorno.creditoDebitado)
                {
                    mensagemRetorno += string.Format("Foi utilizado {0} de crédito do cliente, restando {1} de crédito. ",
                        creditoUtilizado.ToString("C"), ClienteDAO.Instance.GetCredito(session, acerto.IdCli).ToString("C"));
                }
            }

            #endregion

            return string.Format("{0}\t{1}", mensagemRetorno, acerto.IdAcerto);
        }

        /// <summary>
        /// Cancela o pré recebimento do acerto.
        /// </summary>
        public void CancelarPreRecebimentoAcertoComTransacao(DateTime dataEstornoBanco, int idAcerto, string motivo)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    CancelarPreRecebimentoAcerto(transaction, dataEstornoBanco, idAcerto, motivo);

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("CancelarPreRecebimentoAcerto - ID acerto: {0}.", idAcerto), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao cancelar o pré recebimento das contas.", ex));
                }
            }
        }

        /// <summary>
        /// Cancela o pré recebimento do acerto.
        /// </summary>
        public void CancelarPreRecebimentoAcerto(GDASession session, DateTime dataEstornoBanco, int idAcerto, string motivo)
        {
            #region Declaração de variáveis

            var acerto = AcertoDAO.Instance.GetAcertoDetails(session, (uint)idAcerto);
            var contasReceber = GetByAcerto(session, (uint)idAcerto, false);
            var idsContasR = new List<int>();
            var valoresRecebimento = new List<decimal>();

            #endregion

            #region Validações do cancelamento do pré recebimento

            // Apenas financeiro e caixa diário podem cancelar contas recebidas.
            if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) && !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
            {
                throw new Exception("Você não tem permissão para cancelar acertos.");
            }

            #endregion

            #region Atualização dos dados do acerto

            foreach (var contaReceber in contasReceber)
            {
                idsContasR.Add((int)contaReceber.IdContaR);
                valoresRecebimento.Add(contaReceber.ValorRec);
            }

            acerto.Situacao = (int)Acerto.SituacaoEnum.Cancelado;
            acerto.IdsContasR = string.Join(",", idsContasR);
            acerto.ValoresR = string.Join(",", valoresRecebimento.Select(f => f.ToString(CultureInfo.InvariantCulture).Replace(".", "").Replace(",", ".")).ToList());

            AcertoDAO.Instance.Update(session, acerto);

            #endregion

            #region Atualização das contas a receber

            objPersistence.ExecuteCommand(session, string.Format("UPDATE contas_receber SET IdAcerto=NULL WHERE IdAcerto={0};", idAcerto));

            #endregion

            LogCancelamentoDAO.Instance.LogAcerto(session, acerto, motivo, true);
        }

        #endregion

        #region Recebimento de conta antecipada

        /// <summary>
        /// Recebimento de conta antecipada
        /// </summary>
        public void ReceberContaAntecipadaComTransacao(uint idContaR, string data)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    ReceberContaAntecipada(transaction, idContaR, data);

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

        public void ValidarReceberContaAntecipada(string data, ref DateTime dataValida)
        {
            /* Chamado 57329. */
            if (!string.IsNullOrWhiteSpace(data) && !DateTime.TryParse(data, out dataValida))
                throw new Exception("Data de recebimento inválida. Informe a data correta.");
        }

        /// <summary>
        /// Recebimento de conta antecipada
        /// </summary>
        public void ReceberContaAntecipada(GDASession sessao, uint idContaR, string data)
        {
            var dataValida = DateTime.Now;

            ValidarReceberContaAntecipada(data, ref dataValida);

            // Atualiza esta conta a receber
            ContasReceber conta = GetElementByPrimaryKey(sessao, idContaR);
            conta.UsuRec = UserInfo.GetUserInfo.CodUser;
            conta.ValorRec = conta.ValorVec;
            conta.Recebida = true;
            conta.IdConta = UtilsPlanoConta.GetPlanoReceb((uint)Glass.Data.Model.Pagto.FormaPagto.Boleto);
            conta.DataRec = dataValida;
            Update(sessao, conta);

            #region Salva o pagamento da conta

            var lstPagtoContasR = new List<uint>();

            try
            {
                var idAntecip = ObtemValorCampo<int>(sessao, "IdAntecipContaRec", "IdContaR = " + idContaR);
                var idContaBanco = AntecipContaRecDAO.Instance.ObtemValorCampo<uint>(sessao, "IdContaBanco", "IdAntecipContaRec = " + idAntecip);

                var pagto = new PagtoContasReceber();

                pagto.IdContaR = conta.IdContaR;
                pagto.IdFormaPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto;
                pagto.IdContaBanco = idContaBanco;
                pagto.ValorPagto = conta.ValorVec;

                lstPagtoContasR.Add(PagtoContasReceberDAO.Instance.Insert(sessao, pagto));
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("Falha ao salvar pagto das contas.", ex);
                throw ex;
            }

            #endregion
        }

        #endregion

        #region Cancelamento conta Simples

        /// <summary>
        /// Cancela conta individual
        /// </summary>
        public void CancelarConta(uint idContaR, string motivo, DateTime dataEstornoBanco, bool cancelamentoErroTef, bool gerarCredito)
        {
            FilaOperacoes.CancelarConta.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    // Apenas financeiro e caixa diário podem cancelar contas recebidas
                    if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) &&
                        !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) &&
                         !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.CancelarRecebimentos))
                        throw new Exception("Você não tem permissão para cancelar contas recebidas, contacte o administrador");

                    // Busca a conta a receber
                    ContasReceber contaRec = GetElement(transaction, idContaR);

                    // Verifica se esta conta já foi recebida
                    if (!contaRec.Recebida)
                        throw new Exception("Esta conta já foi cancelada.");

                    // Verifica se esta conta pertence a um acerto
                    if (contaRec.IdAcerto > 0)
                        throw new Exception("Este recebimento pertence ao acerto n.º " + contaRec.IdAcerto +
                                            ", cancele-o para cancelar esta conta.");

                    // Verifica se esta conta pertence a uma obra e se a obra é à vista.
                    if (contaRec.IdObra.GetValueOrDefault() > 0)
                        if (ObraDAO.Instance.ObtemTipoPagto(transaction, contaRec.IdObra.Value) == (int)Obra.TipoPagtoObra.AVista)
                            throw new Exception("Este recebimento pertence ao crédito/pagamento antecipado n.º " +
                                                contaRec.IdObra + ", cancele-o para cancelar esta conta.");

                    if (contaRec.IdPedido > 0 && !contaRec.ValorExcedentePCP &&
                        PedidoDAO.Instance.ObtemTipoVenda(transaction, contaRec.IdPedido.Value) == (int)Pedido.TipoVendaPedido.AVista)
                        throw new Exception(
                            "Esta conta recebida não pode ser cancelada, pois a mesma foi gerada a partir de um pedido à vista, é necessário cancelar o pedido.");

                    if (contaRec.IdLiberarPedido > 0 &&
                        LiberarPedidoDAO.Instance.IsLiberacaoAVista(transaction, contaRec.IdLiberarPedido.Value))
                        throw new Exception(
                            "Esta conta recebida não pode ser cancelada pois a mesma foi gerada a partir de uma liberação de pedido à vista.");

                    // No sistema do Comércio (Confirmação) o recebimento de uma conta poderá ser cancelado somente se o pedido não tiver sido liberado para a entrega.
                    if (!PedidoConfig.LiberarPedido && FinanceiroConfig.UsarControleLiberarFinanc && contaRec.IdPedido > 0 &&
                        PedidoDAO.Instance.ObtemValorCampo<bool>(transaction, "LiberadoFinanc", "idPedido=" + contaRec.IdPedido))
                        throw new Exception(
                            "Esta conta foi liberada para a entrega. Desfaça a liberação antes de cancelar o recebimento.");

                    if (contaRec.IdLiberarPedido > 0 && contaRec.IdFormaPagto > 0 &&
                        contaRec.IdFormaPagto != (uint)Glass.Data.Model.Pagto.FormaPagto.Prazo &&
                        UtilsPlanoConta.GetPlanoSinal(contaRec.IdFormaPagto.Value) == contaRec.IdConta)
                        throw new Exception(
                            "Esta conta recebida não pode ser cancelada pois a mesma foi gerada a partir de um pagamento de entrada de uma liberação de pedidos à prazo.");


                    if(ExecuteScalar<bool>(transaction, "Select Count(*)>0 From cheques c Where c.IdContaR=" + idContaR + " And Situacao > 1"))
                        throw new Exception(@"Um ou mais cheques recebidos já foram utilizados em outras transações, cancele ou retifique as transações dos cheques antes de cancelar esta conta recebida.");

                    // Se esta conta tiver sido gerada por um sinal, cancela o mesmo
                    if (contaRec.IdSinal > 0)
                    {
                        try
                        {
                            SinalDAO.Instance.Cancelar(transaction, contaRec.IdSinal.Value, null, false, false, motivo, dataEstornoBanco, cancelamentoErroTef, gerarCredito);

                            transaction.Commit();
                            transaction.Close();

                            return;
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao cancelar conta recebida.", ex));
                        }
                    }

                    Pedido pedido = null;
                    LiberarPedido liberacao = null;

                    if (contaRec.IdPedido != null)
                        pedido = PedidoDAO.Instance.GetElementByPrimaryKey(transaction, contaRec.IdPedido.Value);

                    if (contaRec.IdLiberarPedido != null)
                        liberacao = LiberarPedidoDAO.Instance.GetElement(transaction, contaRec.IdLiberarPedido.Value);

                    UtilsFinanceiro.CancelaRecebimento(transaction, UtilsFinanceiro.TipoReceb.ContaReceber,
                        pedido, null, liberacao, contaRec, null, 0,
                        null, null, null, null, dataEstornoBanco, cancelamentoErroTef, gerarCredito);

                    // Se tiver sido recebida parcialmente, apaga conta gerada
                    if (contaRec.ValorRec < (contaRec.ValorVec + contaRec.Juros))
                    {
                        List<ContasReceber> lstDel = objPersistence.LoadData(transaction,
                            "Select * from contas_receber Where idContaR In " +
                            "(Select idContaR From contas_receber Where 1" +
                            (contaRec.IdPedido != null
                                ? " And idPedido=" + contaRec.IdPedido
                                : contaRec.IdLiberarPedido != null
                                    ? " And idLiberarPedido=" + contaRec.IdLiberarPedido
                                    : contaRec.IdAcerto != null
                                        ? " And idAcerto=" + contaRec.IdAcerto
                                        : contaRec.IdAcertoParcial != null
                                            ? " And idAcertoParcial=" + contaRec.IdAcertoParcial
                                            : contaRec.IdNf != null ? " And idNf=" + contaRec.IdNf : "") +
                            " And recebida=0 And Round(valorVec, 2)=" +
                            (Math.Round((contaRec.ValorVec + contaRec.Juros) - contaRec.ValorRec, 2)).ToString()
                                .Replace(',', '.') + ")").ToList();

                        if (lstDel.Count > 0)
                            DeleteByPrimaryKey(transaction, lstDel[0].IdContaR);
                        else // Troca o valor venc pelo valor rec, caso o acerto seja parcial
                            contaRec.ValorVec = contaRec.ValorRec;
                    }

                    //Apaga o pagto da conta recebida
                    PagtoContasReceberDAO.Instance.DeleteByIdContaR(transaction, idContaR);

                    // Volta esta conta a receber para não recebida
                    contaRec.Recebida = false;
                    contaRec.UsuRec = null;
                    contaRec.DataRec = null;
                    contaRec.ValorRec = 0;
                    contaRec.Juros = 0;
                    Update(transaction, contaRec);

                    ChequesContasReceberDAO.Instance.ExcluirPelaContaReceber(transaction, (int)idContaR);

                    LogCancelamentoDAO.Instance.LogContaReceber(transaction, contaRec, "Cancelamento do recebimento", true);

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao cancelar conta recebida.", ex));
                }
                finally
                {
                    FilaOperacoes.CancelarConta.ProximoFila();
                }
            }
        }

        #endregion

        #region Cancelamento de Acerto

        private static readonly object _cancelarAcertoLock = new object();

        /// <summary>
        /// Cancela acerto
        /// </summary>
        public void CancelarAcerto(uint idAcerto, string motivo, DateTime dataEstornoBanco, bool cancelamentoErroTef, bool gerarCredito)
        {
            lock (_cancelarAcertoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        CancelarAcerto(transaction, idAcerto, motivo, dataEstornoBanco, cancelamentoErroTef, gerarCredito);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException(string.Format("CancelarAcerto - ID: {0}", idAcerto), ex);
                        throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao cancelar acerto.", ex));
                    }
                }
            }
        }

        /// <summary>
        /// Cancela acerto
        /// </summary>
        public void CancelarAcerto(GDASession session, uint idAcerto, string motivo, DateTime dataEstornoBanco, bool cancelamentoErroTef, bool gerarCredito)
        {
            // Apenas financeiro e caixa diário podem cancelar contas recebidas
            if ((!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) &&
                !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento)) ||
                !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.CancelarRecebimentos))
                throw new Exception("Você não tem permissão para cancelar acertos, contacte o administrador");

            // No sistema do Comércio (Confirmação) o acerto poderá ser cancelado somente se nenhum pedido recebido no mesmo tenha sido liberado para a entrega.
            if (!PedidoConfig.LiberarPedido && FinanceiroConfig.UsarControleLiberarFinanc)
            {
                var pedidosLiberados = "";
                var idsPedido = Instance.ObtemIdsPedido(session, idAcerto, false).Split(',');

                for (var i = 0; i < idsPedido.Length; i++)
                {
                    var liberadoFinanceiro = PedidoDAO.Instance.ObtemValorCampo<bool>(session, "LiberadoFinanc", "idPedido=" + idsPedido[i]);
                    if (liberadoFinanceiro)
                        pedidosLiberados += idsPedido[i] + ",";
                }

                if (!string.IsNullOrEmpty(pedidosLiberados))
                    throw new Exception("O(s) Pedido(s) " + pedidosLiberados.TrimEnd(',') + " está(ão) liberado(s) para entrega. Desfaça a liberação antes de cancelar este acerto.");
            }

            // Verifica se alguma conta parcial desse acerto já foi paga
            if (objPersistence.ExecuteSqlQueryCount(session, "select count(*) from contas_receber where recebida=true and valorRec>0 and idAcertoParcial=" + idAcerto) > 0)
                throw new Exception("Uma conta desse acerto já foi recebida. Cancele o recebimento para cancelar esse acerto.");

            // Verifica se alguma conta parcial desse acerto foi renegociada novamente
            if (objPersistence.ExecuteSqlQueryCount(session, "select count(*) from contas_receber where recebida=true and idAcertoParcial=" + idAcerto) > 0)
                throw new Exception("Uma conta desse acerto foi renegociada novamente. Cancele a renegociação para cancelar esse acerto.");

            if (ExecuteScalar<bool>(session, "Select Count(*)>0 From cheques c Where c.IdAcerto=" + idAcerto + " And Situacao > 1"))
                throw new Exception(@"Um ou mais cheques recebidos já foram utilizados em outras transações, cancele ou retifique as transações dos cheques antes de cancelar este acerto.");

            var acerto = AcertoDAO.Instance.GetAcertoDetails(session, idAcerto);

            // Verifica se algum dos cheques recebidos foram depositados, se tiverem sido, obriga o usuáio a cancelar o depósito antes
            var lstCheques = ChequesDAO.Instance.GetByAcerto(session, idAcerto);
            foreach (var c in lstCheques)
            {
                if (c.IdDeposito > 0)
                    throw new Exception("Um dos cheques deste acerto já foi depositado, cancele ou altere o depósito nº " + c.IdDeposito.Value + " antes de cancelar este acerto.");

                if (c.Situacao == (int)Cheques.SituacaoCheque.Compensado)
                {
                    var idPagto = PagtoChequeDAO.Instance.GetPagtoByCheque(session, c.IdCheque);
                    if (idPagto > 0)
                        throw new Exception("Um dos cheques deste acerto foi usado em um pagamento, cancele ou altere o pagto. nº " + idPagto + " antes de cancelar este acerto.");
                }
            }

            // Exclui conta gerada por recebimento parcial, se houver
            foreach (var conta in objPersistence.LoadData(session, "Select * From contas_receber Where recebida=false And idAcertoParcial=" + idAcerto).ToList())
                Delete(session, conta);

            var lstContas = GetByAcerto(session, idAcerto, false);

            UtilsFinanceiro.CancelaRecebimento(session, UtilsFinanceiro.TipoReceb.Acerto, null, null, null, null, acerto, 0, null, null, null, null,
                dataEstornoBanco, cancelamentoErroTef, gerarCredito);

            string idsContasR = "", valoresR = "", idsChequesR = "";
            foreach (var c in lstContas)
            {
                idsContasR += c.IdContaR + ",";
                valoresR += c.ValorRec.ToString(CultureInfo.InvariantCulture).Replace(".", "").Replace(",", ".") + ",";
            }

            foreach (var c in lstCheques)
                idsChequesR += c.IdCheque + ",";

            // Volta contas recebidas por este acerto para em aberto
            objPersistence.ExecuteCommand(session,
                @"Update contas_receber set idAcerto=null, recebida=false, usuRec=null, dataRec=null,
                    valorRec=null, juros=0, renegociada=false Where idAcerto=" + idAcerto);

            //Remove o pagamento das contas recebidas
            if (lstContas.Count > 0)
            {
                var idsContasPagto = string.Join(",", lstContas.Select(f => f.IdContaR.ToString()).ToArray());
                PagtoContasReceberDAO.Instance.DeleteByIdContaR(session, idsContasPagto);
            }

            acerto.Situacao = (int)Acerto.SituacaoEnum.Cancelado;
            acerto.IdsContasR = idsContasR.TrimEnd(',');
            acerto.IdsChequesR = idsChequesR.TrimEnd(',');
            acerto.ValoresR = valoresR.TrimEnd(',');

            AcertoDAO.Instance.Update(session, acerto);

            LogCancelamentoDAO.Instance.LogAcerto(session, acerto, motivo, true);
        }

        #endregion

        #region Cancelamento de conta antecipada

        /// <summary>
        /// Cancela conta antecipada
        /// </summary>
        public void CancelarContaAntecipada(uint idContaR)
        {
            //Apaga o pagto da conta recebida
            PagtoContasReceberDAO.Instance.DeleteByIdContaR(idContaR);

            // Atualiza esta conta a receber
            ContasReceber conta = GetElementByPrimaryKey(idContaR);
            conta.UsuRec = null;
            conta.ValorRec = 0;
            conta.Recebida = false;
            conta.DataRec = null;
            Update(conta);
        }

        #endregion

        #region Atualiza cheques com idPedido

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Atualiza cheques com idPedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idCheque"></param>
        public void AtualizaChequeIdPedido(uint idPedido, uint idCheque)
        {
            AtualizaChequeIdPedido(null, idPedido, idCheque);
        }

        /// <summary>
        /// Atualiza cheques com idPedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idCheque"></param>
        public void AtualizaChequeIdPedido(GDASession sessao, uint idPedido, uint idCheque)
        {
            objPersistence.ExecuteCommand(sessao, "Update cheques Set idPedido=" + idPedido + " Where idCheque=" + idCheque);
        }

        #endregion

        #region Renegociar parcelas

        /// <summary>
        /// Renegocia parcela, gerando outras parcelas
        /// </summary>
        public void RenegociarParcela(uint idPedido, uint idContaR, uint idFormaPagto, int numParc, string parcelas, decimal multa)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    // Busca a conta a receber
                    ContasReceber conta = GetElementByPrimaryKey(transaction, idContaR);

                    if (conta.Recebida)
                        throw new Exception("Esta conta já foi recebida.");

                    // Chamado 12935. Contas a receber foram renegociadas após a geração do arquivo CNAB, por isso, ao importar
                    // o arquivo de retorno o sistema duplicou a conta a receber informada no chamado.
                    if (conta.IdArquivoRemessa.GetValueOrDefault() > 0)
                        throw new Exception("Não é possível renegociar contas a receber que possuem CNAB gerado.");

                    /* Chamado 40057. */
                    if (conta.Acrescimo > 0 || conta.Desconto > 0)
                        throw new Exception("Não é possível renegociar contas a receber que possuem desconto/acréscimo aplicado.");

                    List<ContasReceber> lstContaReceber = new List<ContasReceber>();

                    decimal total = 0;
                    decimal somaMulta = 0;

                    string[] vetParc = parcelas.TrimEnd('|').Split('|');

                    for (int i = 0; i < numParc; i++)
                    {
                        string[] parc = vetParc[i].Split(';');

                        decimal valorParc = Conversoes.StrParaDecimal(parc[0]);
                        decimal juros = Conversoes.StrParaDecimal(parc[2]);

                        decimal multaItem = 0;

                        if (i < (numParc - 1))
                        {
                            multaItem = Math.Round((valorParc / conta.ValorVec) * multa, 2);
                            somaMulta += multaItem;
                        }
                        else
                        {
                            multaItem = multa - somaMulta;
                        }

                        DateTime dataValida;
                        if (!DateTime.TryParse(parc[1], out dataValida))
                        {
                            throw new Exception($"A data informada é invalida. Data informada {parc[1]}");
                        }

                        ContasReceber contaRec = new ContasReceber();
                        contaRec.IdLoja = conta.IdLoja;
                        contaRec.IdCliente = conta.IdCliente;
                        CopiaReferencias(transaction, conta, ref contaRec);
                        contaRec.IdConta = UtilsPlanoConta.GetPlanoPrazo(idFormaPagto);
                        contaRec.Recebida = false;
                        contaRec.ValorVec = valorParc;
                        contaRec.DataVec = DateTime.Parse(parc[1]);
                        contaRec.DataPrimNeg = conta.DataPrimNeg != null ? conta.DataPrimNeg : conta.DataVec;
                        contaRec.IdFormaPagto = idFormaPagto; // Pega a forma de pagamento ao renegociar a parcela
                        contaRec.Renegociada = true;
                        contaRec.Juridico = conta.Juridico;
                        contaRec.Juros = juros;
                        contaRec.Multa = multaItem;
                        contaRec.TipoConta = conta.TipoConta;
                        contaRec.IdFuncComissaoRec = conta.IdFuncComissaoRec;

                        // Caso seja apenas uma parcela, irá manter a parcela e o máximo de parcelas, caso a conta tenha sido renegociada
                        // em mais de uma parcela, aumentar o número máximo de parcelas e continua a numeração do numParc a partir do numParc desta
                        // parcela sendo renegociada
                        contaRec.NumParc = conta.NumParc + i;
                        contaRec.NumParcMax = conta.NumParcMax + (numParc - 1);

                        lstContaReceber.Add(contaRec);

                        total += contaRec.ValorVec;
                    }

                    if (Math.Round(total, 2) != Math.Round(conta.ValorVec, 2))
                        throw new Exception("O valor informado nas parcelas está diferente do valor da conta sendo renegociada. Valor da Conta: " + conta.ValorVec.ToString("C") + " Valor das Parcelas: " + total.ToString("C"));

                    foreach (ContasReceber c in lstContaReceber)
                    {
                        c.IdContaR = Insert(transaction, c);

                        // Atualiza a referência do idNf, pois como ele é "input", não é salvo, mantém a datacad da conta original
                        objPersistence.ExecuteCommand(transaction, "Update contas_receber Set dataCad=?dataCad, idNf=" + (c.IdNf == null ? "Null" : c.IdNf.ToString()) +
                            " Where idContaR=" + c.IdContaR, new GDAParameter("?dataCad", conta.DataCad));
                    }

                    // Exclui esta conta a receber
                    DeleteByPrimaryKey(transaction, idContaR);

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException("RenegociarConta", ex);

                    throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao renegociar conta.", ex));
                }
            }
        }

        /// <summary>
        /// Renegocia parcelas, gerando outras parcelas
        /// </summary>
        public void RenegociarParcela(uint idCliente, string idsContasR, uint idFormaPagto, int numParc, string parcelas, decimal multa,
            decimal creditoUtilizado, string obs)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    //Chamado: 29294 - Gerou um acerto sem cliente
                    if (idCliente <= 0)
                        throw new Exception("Nenhum cliente foi informado");

                    /* Chamado 46262. */
                    if (ComissaoDAO.Instance.VerificarComissaoContasRecebidas() && (Configuracoes.FinanceiroConfig.FinanceiroPagto.SubtrairICMSCalculoComissao
                        || Configuracoes.ComissaoConfig.TotalParaComissao != Configuracoes.ComissaoConfig.TotalComissaoEnum.TotalSemImpostos))
                        throw new Exception(@"Não é possível renegociar contas caso a configuração Comissão por contas recebidas e
                                Subtrair ICMS do cálculo da comissão de pedido ou Define qual total será usado no cálculo da comissão esteja habilitada.");

                    // Busca a contas a receber que estão sendo renegociadas
                    var contas = GetByPks(transaction, idsContasR);

                    if (ComissaoDAO.Instance.VerificarComissaoContasRecebidas() && contas.Select(f => f.IdFuncComissaoRec).Distinct().Count() > 1)
                        throw new Exception("Não é possivel renegociar contas em que o funcionário a receber comissão sejam diferentes");

                    var idsLiberacao = string.Join(",", contas.Where(f => f.IdLiberarPedido > 0).Select(f => f.IdLiberarPedido).Distinct());

                    //Busca todos os Ids nota fiscal de acordo com a liberação das contas
                    var idsNotas = ExecuteMultipleScalar<int>(string.Format("Select distinct(IdNf) from pedidos_nota_fiscal where idLiberarPedido in ({0})", idsLiberacao.Count() > 0 ? idsLiberacao : "0"));

                    idsNotas.AddRange(contas.Where(f => f.IdNf > 0).Select(f => (int)f.IdNf).Distinct());

                    var possuiReferenciaDeNota = idsLiberacao.Count() == 0 && idsNotas.Count() == 1 ? true : false;

                    //Verifica se todas as contas tem referencia de nota fiscal
                    foreach (var idLiberarPedido in idsLiberacao.Split(','))
                    {
                        if (!string.IsNullOrWhiteSpace(idLiberarPedido))
                            possuiReferenciaDeNota = ExecuteScalar<int>("Select count(idnf) from pedidos_nota_fiscal where idliberarPedido=" + idLiberarPedido) > 0;

                        else if (!possuiReferenciaDeNota)
                            break;
                    }

                    /* Chamado 53850. */
                    if (FinanceiroConfig.ContasReceber.UtilizarControleContaReceberJuridico &&
                        (contas.Count() != contas.Count(f => f.Juridico) && contas.Count() != contas.Count(f => !f.Juridico)))
                        throw new Exception("Todas as contas devem estar marcadas como Jurídico/Cartório ou nenhuma delas deve estar marcada como Jurídico/Cartório para renegociá-las.");

                    decimal totalContas = 0;
                    DateTime? dataPrimNeg = contas[0].DataPrimNeg != null ? contas[0].DataPrimNeg.Value : contas[0].DataVec;

                    // Se todas as contas forem do mesmo pedido, mantém idPedido
                    uint? idPedido = (contas[0].IdPedido > 0 ? contas[0].IdPedido : null);

                    // Se todas as contas forem da mesma loja, mantém idLoja
                    uint idLoja = contas[0].IdLoja;
                    byte? tipoConta = null;

                    for (int i = 0; i < contas.Length; i++)
                    {
                        // Chamado 12935. Contas a receber foram renegociadas após a geração do arquivo CNAB, por isso, ao importar
                        // o arquivo de retorno o sistema duplicou a conta a receber informada no chamado.
                        if (!string.IsNullOrEmpty(contas[i].NumeroDocumentoCnab) || contas[i].NumeroArquivoRemessaCnab.GetValueOrDefault() > 0)
                            throw new Exception("Não é possível renegociar contas a receber que possuem CNAB gerado.");

                        if (tipoConta == null)
                            tipoConta = contas[i].TipoConta;

                        totalContas += contas[i].ValorVec;
                        dataPrimNeg = dataPrimNeg != null ? (contas[i].DataPrimNeg != null ? new DateTime(Math.Min(dataPrimNeg.Value.Ticks,
                            contas[i].DataPrimNeg.Value.Ticks)) : new DateTime(Math.Min(dataPrimNeg.Value.Ticks, contas[i].DataVec.Ticks))) :
                            contas[i].DataVec;

                        // Se todas as contas forem do mesmo pedido, mantém idPedido
                        if (contas[i].IdPedido != idPedido)
                            idPedido = null;

                        // Se todas as contas forem da mesma loja, mantém idLoja
                        if (contas[i].IdLoja != idLoja)
                            idLoja = 0;

                        if (contas[i].Recebida)
                            throw new Exception("Uma dessas contas já foi recebida.");

                        // Chamado 12935. Contas a receber foram renegociadas após a geração do arquivo CNAB, por isso, ao importar
                        // o arquivo de retorno o sistema duplicou a conta a receber informada no chamado.
                        if (contas[i].IdArquivoRemessa.GetValueOrDefault() > 0)
                            throw new Exception("Não é possível renegociar contas a receber que possuem CNAB gerado.");
                    }

                    List<ContasReceber> lstContaReceber = new List<ContasReceber>();

                    uint idConta = UtilsPlanoConta.GetPlanoPrazo(idFormaPagto);

                    Acerto acerto = new Acerto(idCliente);
                    acerto.DataCad = DateTime.Now;
                    acerto.UsuCad = UserInfo.GetUserInfo.CodUser;
                    acerto.ValorCreditoAoCriar = ClienteDAO.Instance.GetCredito(transaction, acerto.IdCli);
                    acerto.Obs = obs;
                    acerto.IdAcerto = AcertoDAO.Instance.Insert(transaction, acerto);

                    // Chamado 13151. Ocorreu um problema que fez com que a conta a receber ficasse sem referências, pode ser
                    // que o código do acerto não foi gerado por algum motivo e por isso colocamos esta verificação.
                    if (acerto == null || acerto.IdAcerto == 0)
                        throw new Exception("Falha ao efetuar a renegociação das contas. Tente novamente.");

                    decimal total = 0;
                    decimal somaMulta = 0;

                    string[] vetParc = parcelas.TrimEnd('|').Split('|');

                    for (int i = 0; i < numParc; i++)
                    {
                        string[] parc = vetParc[i].Split(';');

                        decimal valorParc = Conversoes.StrParaDecimal(parc[0]);
                        decimal juros = Conversoes.StrParaDecimal(parc[2]);

                        decimal multaItem = 0;

                        if (i < (numParc - 1))
                        {
                            multaItem = Math.Round((valorParc / totalContas) * multa, 2);
                            somaMulta += multaItem;
                        }
                        else
                        {
                            multaItem = multa - somaMulta;
                        }

                        if (String.IsNullOrEmpty(parc[1]))
                            throw new Exception("Informe a data das parcelas.");

                        ContasReceber contaRec = new ContasReceber();
                        contaRec.IdLoja = idLoja > 0 ? idLoja : FuncionarioDAO.Instance.ObtemIdLoja(transaction, acerto.UsuCad);
                        contaRec.IdCliente = acerto.IdCli;
                        contaRec.IdAcertoParcial = acerto.IdAcerto;
                        contaRec.IdPedido = idPedido;
                        contaRec.IdConta = idConta;
                        contaRec.Recebida = false;
                        contaRec.NumParc = i + 1;
                        contaRec.NumParcMax = numParc;
                        contaRec.ValorVec = valorParc;
                        contaRec.DataVec = DateTime.Parse(parc[1]);
                        contaRec.DataPrimNeg = dataPrimNeg.Value;
                        contaRec.Renegociada = true;
                        //Se todas contas tiverem referencia de nota fiscal e for a mesma nota fiscal, atribui o identificador na conta gerada
                        contaRec.IdNf = idsNotas.Distinct().Count() > 1 || !possuiReferenciaDeNota ? null : (uint?)idsNotas[0];
                        contaRec.IdFuncComissaoRec = contas[0]?.IdFuncComissaoRec; // Caso tenha IdFuncComissaoRec preenche na nova conta (Pega só pra primeira conta pois não é possivel receber de contas com IdFuncComissaoRec diferentes)

                        /* Chamado 50083. */
                        if (FinanceiroConfig.ContasReceber.UtilizarControleContaReceberJuridico)
                            contaRec.Juridico = contas.Count(f => !f.Juridico) == 0;

                        contaRec.Juros = juros;
                        contaRec.Multa = multaItem;
                        contaRec.TipoConta = tipoConta.GetValueOrDefault();
                        contaRec.IdFormaPagto = idFormaPagto;

                        lstContaReceber.Add(contaRec);

                        total += contaRec.ValorVec;
                    }

                    if (Math.Round(total, 2) != Math.Round(totalContas - creditoUtilizado, 2))
                        throw new Exception("O valor informado nas parcelas está diferente do valor das contas sendo renegociadas. Valor das Contas: " + totalContas.ToString("C") + " Valor das Parcelas: " + total.ToString("C"));

                    foreach (ContasReceber c in lstContaReceber)
                        c.IdContaR = Insert(transaction, c);

                    // Coloca essas contas a receber como pagas
                    for (int i = 0; i < contas.Length; i++)
                    {
                        contas[i].IdAcerto = acerto.IdAcerto;
                        contas[i].Recebida = true;
                        contas[i].DataRec = DateTime.Now;
                        contas[i].UsuRec = UserInfo.GetUserInfo.CodUser;
                        contas[i].Renegociada = true;
                        Update(transaction, contas[i]);
                    }

                    // Gera a movimentação no caixa do crédito
                    if (creditoUtilizado > 0)
                    {
                        idConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.RecPrazoCredito);

                        if (Geral.ControleCaixaDiario && UserInfo.GetUserInfo.IsCaixaDiario)
                            CaixaDiarioDAO.Instance.MovCxAcerto(transaction, UserInfo.GetUserInfo.IdLoja, acerto.IdCli, acerto.IdAcerto, 1, creditoUtilizado, 0, idConta, null, 0, null, false);
                        else
                            CaixaGeralDAO.Instance.MovCxAcerto(transaction, acerto.IdAcerto, acerto.IdCli, idConta, 1, creditoUtilizado, 0, null, 0, false, null, null);

                        ClienteDAO.Instance.DebitaCredito(transaction, acerto.IdCli, creditoUtilizado);
                    }

                    acerto.CreditoUtilizadoCriar = creditoUtilizado;
                    AcertoDAO.Instance.Update(transaction, acerto);

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao renegociar conta.", ex));
                }
            }
        }

        #endregion

        #region Exclui contas a receber em aberto

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Exclui todos as contas a receber do pedido passado
        /// </summary>
        /// <param name="idPedido"></param>
        public void DeleteByPedido(GDASession sessao, uint idPedido)
        {
            foreach (ContasReceber c in objPersistence.LoadData(sessao, "select * from contas_receber where recebida=false and idPedido=" + idPedido).ToList())
            {
                LogCancelamentoDAO.Instance.LogContaReceber(sessao, c, "Cancelamento do Pedido " + idPedido, false);
                Delete(sessao, c);
            }
        }

        /// <summary>
        /// Exclui todos as contas a receber do pedido passado
        /// </summary>
        /// <param name="idPedido"></param>
        public void DeleteByPedidoAVista(GDASession sessao, uint idPedido)
        {
            foreach (ContasReceber c in objPersistence.LoadData(sessao, "Select * from contas_receber Where idPedido=" + idPedido +
                " And recebida=true And idConta In (" + UtilsPlanoConta.ContasAVista() + ")").ToList())
            {
                LogCancelamentoDAO.Instance.LogContaReceber(sessao, c, "Cancelamento do Pedido " + idPedido, false);
                Delete(sessao, c);
            }
        }

        /// <summary>
        /// Exclui todos as contas a receber da obra passada
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idObra"></param>
        public void DeleteByObra(GDASession sessao, uint idObra)
        {
            foreach (ContasReceber c in objPersistence.LoadData(sessao, "SELECT * FROM contas_receber WHERE (Recebida=false OR (SELECT tipoPagto=" +
                (int)Obra.TipoPagtoObra.AVista + " FROM obra WHERE idObra=" + idObra + ")) AND idObra=" + idObra).ToList())
                LogCancelamentoDAO.Instance.LogContaReceber(sessao, c, "Cancelamento da Obra " + idObra, false);

            // Exclui as contas a receber ou recebidas caso a obra seja à vista
            foreach (var conta in objPersistence.LoadData(sessao, "Select * From contas_receber Where (Recebida=false Or (Select tipoPagto=" + (int)Obra.TipoPagtoObra.AVista +
                " From obra Where idObra=" + idObra + ")) And idObra=" + idObra).ToList())
                Delete(sessao, conta);
        }

        /// <summary>
        /// Exclui todos as contas a receber da troca/devolução passada
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idTrocaDevolucao"></param>
        public void DeleteByTrocaDevolucao(GDASession session, uint idTrocaDevolucao)
        {
            foreach (ContasReceber c in objPersistence.LoadData(session, "select * from contas_receber where recebida=false and idTrocaDevolucao=" + idTrocaDevolucao).ToList())
            {
                LogCancelamentoDAO.Instance.LogContaReceber(session, c, "Cancelamento da Troca/Devolução " + idTrocaDevolucao, false);
                Delete(session, c);
            }
        }

        /// <summary>
        /// Exclui todos as contas a receber da devolução de pagamento passada
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idDevolucaoPagto"></param>
        public void DeleteByDevolucaoPagto(GDASession session, uint idDevolucaoPagto)
        {
            foreach (ContasReceber c in objPersistence.LoadData(session, "select * from contas_receber where recebida=false and idDevolucaoPagto=" + idDevolucaoPagto).ToList())
            {
                LogCancelamentoDAO.Instance.LogContaReceber(session, c, "Cancelamento da Devolução de Pagamento " + idDevolucaoPagto, false);
                Delete(session, c);
            }
        }

        /// <summary>
        /// Exclui todos as contas a receber do cartão não identificado.
        /// </summary>
        public void DeleteByCartaoNaoIdentificado(GDASession session, int idCartaoNaoIdentificado)
        {
            foreach (var c in objPersistence.LoadData(session, "SELECT * FROM contas_receber WHERE Recebida=0 AND IdCartaoNaoIdentificado=" + idCartaoNaoIdentificado).ToList())
            {
                LogCancelamentoDAO.Instance.LogContaReceber(session, c, "Cancelamento do Cartão Não Identificado " + idCartaoNaoIdentificado, false);
                Delete(session, c);
            }
        }

        /// <summary>
        /// Exclui todos as contas a receber da liberação de pedidos passada
        /// </summary>
        public void DeleteByLiberarPedido(GDASession sessao, uint idLiberarPedido)
        {
            DeleteByLiberarPedido(sessao, idLiberarPedido, true);
        }

        /// <summary>
        /// Exclui todos as contas a receber da liberação de pedidos passada
        /// </summary>
        public void DeleteByLiberarPedido(GDASession sessao, uint idLiberarPedido, bool apagarParcelaCartao)
        {
            foreach (ContasReceber c in objPersistence.LoadData(sessao, "select * from contas_receber where Coalesce(recebida, false)=false and idLiberarPedido=" + idLiberarPedido).ToList())
            {
                /* Chamado 34329. */
                if (!apagarParcelaCartao && c.IsParcelaCartao)
                    continue;

                LogCancelamentoDAO.Instance.LogContaReceber(sessao, c, "Cancelamento da Liberação de Pedido " + idLiberarPedido, false);
                Delete(sessao, c);
            }
        }

        #endregion

        /// <summary>
        /// Exclui todas as contas a receber de um contro de contas
        /// </summary>
        public void DeleteByEncontroContas(GDASession session, uint idEncontroContas)
        {
            foreach (var conta in objPersistence.LoadData(session, "Select * From contas_receber WHERE COALESCE(Recebida,false)=false AND idEncontroContas=" + idEncontroContas).ToList())
                Delete(session, conta);
        }

        #region Busca as contas a receber de um pedido

        /// <summary>
        /// Busca contas a receber de um pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="emAberto"></param>
        /// <param name="antecipadas">Busca contas antecipadas</param>
        /// <returns></returns>
        public IList<ContasReceber> GetByPedido(GDASession sessao, uint idPedido, bool emAberto, bool antecipadas)
        {
            string sql = "Select c.*, cli.Nome as NomeCli, pl.Descricao as DescrPlanoConta From contas_receber c " +
                "Left Join cliente cli On (c.IdCliente=cli.id_Cli) " +
                "Left Join plano_contas pl On (c.IdConta=pl.IdConta) " +
                "Where 1 And (c.isParcelaCartao=false or c.isParcelaCartao is null)";

            if (idPedido > 0)
                sql += " And c.idPedido=" + idPedido;
            else
                sql += " And 0>1";

            if (emAberto)
                sql += " And Recebida <> 1";

            if (!antecipadas)
                sql += " And Coalesce(idAntecipContaRec,0)=0";

            sql += " Order By DataVec Asc";

            return objPersistence.LoadData(sessao, sql).ToList();
        }

        #endregion

        #region Busca as contas a receber de um sinal

        /// <summary>
        /// Busca contas a receber de um sinal.
        /// </summary>
        public IList<ContasReceber> ObterPeloSinal(GDASession session, int idSinal)
        {
            var sql =
                string.Format(@"SELECT c.* FROM contas_receber c
                    WHERE IdSinal={0} AND (c.IsParcelaCartao=0 OR c.IsParcelaCartao IS NULL) ORDER BY DataVec", idSinal);

            return objPersistence.LoadData(session, sql).ToList();
        }

        #endregion

        #region Verifica se o cliente possui contas vencidas e não pagas

        /// <summary>
        /// Busca contas a receber de um pedido
        /// </summary>
        public bool ClientePossuiContasVencidas(uint idCliente)
        {
            return ClientePossuiContasVencidas(null, idCliente);
        }

        /// <summary>
        /// Busca contas a receber de um pedido
        /// </summary>
        public bool ClientePossuiContasVencidas(GDASession session, uint idCliente)
        {
            var sql = @"
                Select Count(*) From contas_receber c
                Where AddDate(datavec, interval " + FinanceiroConfig.NumeroDiasContaRecAtrasada + @" day)<now()
                    And Recebida <> 1
                    And (c.isParcelaCartao=false or c.isParcelaCartao is null)
                    And valorVec>0
                    And idCliente=" + idCliente;

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        #endregion

        #region Busca as contas a receber/recebidas de um pedido ou liberação

        /// <summary>
        /// Busca contas a receber de um pedido
        /// </summary>
        public IList<ContasReceber> GetByPedidoLiberacao(int tipoBusca, uint idPedidoLiberacao, string idsContasR)
        {
            string sql = "Select c.*, cli.Nome as NomeCli, pl.Descricao as DescrPlanoConta From contas_receber c " +
                "Left Join cliente cli On (c.IdCliente=cli.id_Cli) " +
                "Left Join plano_contas pl On (c.IdConta=pl.IdConta) " +
                "Where (c.isParcelaCartao=false or c.isParcelaCartao is null)";

            bool temFiltros = false;
            if (tipoBusca == 0 && idPedidoLiberacao > 0)
            {
                temFiltros = true;

                if (!PedidoConfig.LiberarPedido)
                    sql += " And c.idPedido=" + idPedidoLiberacao;
                else
                    sql += " And c.idLiberarPedido=" + idPedidoLiberacao;
            }

            if (tipoBusca == 1 && !String.IsNullOrEmpty(idsContasR))
            {
                temFiltros = true;
                sql += " and idContaR in (" + idsContasR.Trim(',') + ")";
            }

            if (!temFiltros)
                sql += " and false";

            sql += " Order By DataVec Asc";

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Busca as contas a receber de um pedido ou liberação

        /// <summary>
        /// Busca contas a receber de um pedido ou liberação.
        /// </summary>
        public IList<ContasReceber> PesquisarContasAReceberPeloPedidoLiberacao(int tipoBusca, uint idPedidoLiberacao, string idsContasR)
        {
            if ((tipoBusca == 0 && idPedidoLiberacao == 0) || (tipoBusca == 1 && string.IsNullOrWhiteSpace(idsContasR)))
                return new List<ContasReceber>();

            var sql = @"SELECT c.*, cli.Nome AS NomeCli, pl.Descricao AS DescrPlanoConta FROM contas_receber c
                    LEFT JOIN cliente cli ON (c.IdCliente=cli.Id_Cli)
                    LEFT JOIN plano_contas pl ON (c.IdConta=pl.IdConta)
                WHERE Recebida <> 1 AND (c.IsParcelaCartao=0 OR c.IsParcelaCartao IS NULL)";

            if (tipoBusca == 0 && idPedidoLiberacao > 0)
            {
                if (!PedidoConfig.LiberarPedido)
                    sql += string.Format(" AND c.IdPedido={0}", idPedidoLiberacao);
                else
                    sql += string.Format(" AND c.IdLiberarPedido={0}", idPedidoLiberacao);
            }

            if (tipoBusca == 1 && !string.IsNullOrEmpty(idsContasR))
                sql += string.Format(" AND IdContaR IN ({0})", idsContasR.Trim(','));

            sql += " ORDER BY DataVec ASC";

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Busca contas recebidas em um acerto

        public IList<ContasReceber> GetByAcerto(GDASession sessao, uint idAcerto, bool soRecebidas)
        {
            bool buscarReais = !AcertoDAO.Instance.Exists(sessao, idAcerto) ||
                AcertoDAO.Instance.ObtemValorCampo<Acerto.SituacaoEnum>(sessao, "situacao", "idAcerto=" + idAcerto) != Acerto.SituacaoEnum.Cancelado;

            // Este SQL fica mais otimizado buscando os pedidos da liberação da forma como está
            string sql = @"
                Select c.*, cli.Nome as NomeCli, (
		                select Cast(group_concat(distinct concat(p.idPedido, if(p.codCliente<>'' And p.codCliente is not NULL,
                            Concat(' (', p.codcliente, ')'), '')) separator ', ') as char) as pedidosLiberacao
                        from produtos_liberar_pedido plp
       		                inner join pedido p On (plp.idPedido=p.idPedido)
		                Where c.idLiberarPedido=plp.idLiberarPedido
                        group by plp.idLiberarPedido
                        order by plp.idLiberarPedido, plp.idPedido
                    ) as pedidosLiberacao" + SqlBuscarNF("c", true, 0, false, false) + @"
                From contas_receber c
                    Left Join cliente cli on (c.IdCliente=cli.Id_Cli)
                Where (c.isParcelaCartao=false or c.isParcelaCartao is null)";

            if (buscarReais)
                sql += " and c.idAcerto=" + idAcerto;
            else
            {
                string idsContasR = AcertoDAO.Instance.ObtemValorCampo<string>(sessao, "idsContasR", "idAcerto=" + idAcerto);
                sql += " and c.idContaR in (" + (!String.IsNullOrEmpty(idsContasR) ? idsContasR : "0") + ")";
            }

            if (soRecebidas)
                sql += " and c.recebida=true";

            return objPersistence.LoadData(sessao, sql).ToList();
        }

        /// <summary>
        /// Busca contas renegociadas em um acerto
        /// </summary>
        public IList<ContasReceber> GetRenegByAcerto(uint idAcerto, bool soRecebidas)
        {
            return GetRenegByAcerto(null, idAcerto, soRecebidas);
        }

        /// <summary>
        /// Busca contas renegociadas em um acerto
        /// </summary>
        public IList<ContasReceber> GetRenegByAcerto(GDASession session, uint idAcerto, bool soRecebidas)
        {
            string sql = @"
                Select c.*, cli.Nome as NomeCli" + SqlBuscarNF(session, "c", true, 0, false, false) + @"
                From contas_receber c
                    Left Join cliente cli on (c.IdCliente=cli.Id_Cli)
                Where c.idAcertoParcial=" + idAcerto + @"
                    And (c.isParcelaCartao=false or c.isParcelaCartao is null)";

            if (soRecebidas)
                sql += " and c.recebida=true";

            return objPersistence.LoadData(session, sql).ToList();
        }

        public int GetCountRenegByAcerto(uint idAcerto, bool soRecebidas)
        {
            string sql = @"
                Select count(*) From contas_receber c
                    Left Join cliente cli on (c.IdCliente=cli.Id_Cli)
                Where c.idAcertoParcial=" + idAcerto + @"
                    And (c.isParcelaCartao=false or c.isParcelaCartao is null)";

            if (soRecebidas)
                sql += " and c.recebida=true";

            return objPersistence.ExecuteSqlQueryCount(sql);
        }

        /// <summary>
        /// Recupera as contas a receber de uma nota fiscal
        /// </summary>
        public IList<ContasReceber> GetByNf(uint idNf)
        {
            return GetByNf(null, idNf);
        }

        /// <summary>
        /// Recupera as contas a receber de uma nota fiscal
        /// </summary>
        public IList<ContasReceber> GetByNf(GDASession session, uint idNf)
        {
            string sql = @"
                SELECT cr.*, c.nome as NomeCli
                FROM contas_receber cr
                    INNER JOIN cliente c ON (cr.idCliente = c.id_cli)
                WHERE cr.idNf=" + idNf;

            return objPersistence.LoadData(session, sql).ToList();
        }

        /// <summary>
        /// Retorna se o acerto possui conta juridico
        /// </summary>
        /// <param name="idAcerto"></param>
        /// <returns></returns>
        public bool AcertoPossuiContasJuridico(GDASession sessao, int idAcerto)
        {
            return ExecuteScalar<int>(sessao, string.Format("SELECT COUNT(*) FROM contas_receber WHERE idAcerto={0} AND (Renegociada IS NULL OR Renegociada=0) AND Juridico=1", idAcerto)) > 0;
        }

        #endregion

        #region Busca contas recebidas de uma liberação

        public IList<ContasReceber> GetRecebidasByLiberacao(uint idLiberacao)
        {
            string sql = @"
                SELECT cr.*
                FROM contas_receber cr
                WHERE recebida = true AND cr.idLiberarPedido = " + idLiberacao;

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Busca contas recebidas de um sinal

        /// <summary>
        /// Busca contas recebidas de um sinal
        /// </summary>
        /// <param name="idSinal"></param>
        /// <returns></returns>
        public IList<ContasReceber> GetRecebidasBySinal(uint idSinal)
        {
            return GetRecebidasBySinal((GDASession)null, idSinal);
        }

        /// <summary>
        /// Busca contas recebidas de um sinal
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idSinal"></param>
        /// <returns></returns>
        public IList<ContasReceber> GetRecebidasBySinal(GDASession session, uint idSinal)
        {
            string sql = @"
                SELECT cr.*
                FROM contas_receber cr
                WHERE COALESCE(recebida, 0) = 1 AND cr.idSinal = " + idSinal;

            return objPersistence.LoadData(session, sql).ToList();
        }

        #endregion

        #region busca contas recebidas de uma obra

        /// <summary>
        /// Busca contas recebidas de uma obra
        /// </summary>
        /// <param name="idSinal"></param>
        /// <returns></returns>
        public IList<ContasReceber> GetRecebidasByObra(uint idObra)
        {
            string sql = @"
                SELECT cr.*
                FROM contas_receber cr
                WHERE COALESCE(recebida, 0) = 1 AND cr.idObra = " + idObra;

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Busca contas recebidas antecipadas

        /// <summary>
        /// Busca contas recebidas antecipadas. Utilizado na tela CadRetificarAntecipContaRec.aspx.
        /// </summary>
        public IList<ContasReceber> GetByAntecipacao(uint idAntecipContaRec)
        {
            return GetByAntecipacao(null, idAntecipContaRec);
        }

        /// <summary>
        /// Busca contas recebidas antecipadas
        /// </summary>
        public IList<ContasReceber> GetByAntecipacao(GDASession session, uint idAntecipContaRec)
        {
            string sql = @"Select c.*, cli.Nome as NomeCli" +
                SqlBuscarNF(session, "c", true, 0, false, true) + @" From contas_receber c
                Left Join cliente cli on (c.IdCliente=cli.Id_Cli)
                Where c.idAntecipContaRec=" + idAntecipContaRec;

            return objPersistence.LoadData(session, sql).ToList();
        }

        public bool PodeRetificarAntecipacao(GDASession session, uint idAntecipContaRec)
        {
            var sql = @"
                SELECT COUNT(*)
                FROM contas_receber
                WHERE Recebida = 1 AND IdAntecipContaRec=" + idAntecipContaRec;

            return objPersistence.ExecuteSqlQueryCount(session, sql) == 0;
        }

        #endregion

        #region Busca histórico de um cliente

        private string SqlHist(uint idCliente, uint idPedido, string dataIniVenc, string dataFimVenc, string dataIniRec, string dataFimRec,
            string dataIniCad, string dataFimCad, float vIniVenc, float vFinVenc, float vIniRec, float vFinRec, bool emAberto, bool recEmDia,
            bool recComAtraso, bool buscarParcCartao, int contasRenegociadas, bool buscaPedRepoGarantia, bool buscarChequeDevolvido,
            string sort, bool selecionar, out bool temFiltro)
        {
            temFiltro = false;
            string totalEmAberto = GetHistValor(1, idCliente, dataIniVenc, dataFimVenc, dataIniRec, dataFimRec, dataIniCad, dataFimCad,
                vIniVenc, vFinVenc, vIniRec, vFinRec, emAberto, recEmDia, recComAtraso, buscarParcCartao, contasRenegociadas, buscarChequeDevolvido).ToString().Replace(',', '.') + " + 0.001";
            string totalEmDia = GetHistValor(2, idCliente, dataIniVenc, dataFimVenc, dataIniRec, dataFimRec, dataIniCad, dataFimCad,
                vIniVenc, vFinVenc, vIniRec, vFinRec, emAberto, recEmDia, recComAtraso, buscarParcCartao, contasRenegociadas, buscarChequeDevolvido).ToString().Replace(',', '.') + " + 0.001";
            string totalComAtraso = GetHistValor(3, idCliente, dataIniVenc, dataFimVenc, dataIniRec, dataFimRec, dataIniCad, dataFimCad,
                vIniVenc, vFinVenc, vIniRec, vFinRec, emAberto, recEmDia, recComAtraso, buscarParcCartao, contasRenegociadas, buscarChequeDevolvido).ToString().Replace(',', '.') + " + 0.001";

            string campos = selecionar ? @"c.idContaR, c.idPedido, c.idAntecipContaRec, c.idConta, c.dataVec, c.valorVec, c.dataRec, c.DestinoRec,
                c.valorRec, c.juros, c.recebida, c.usuRec, c.idAcerto, c.numParc, c.desconto, c.motivoDescontoAcresc, c.idFuncDescAcresc,
                c.numAutConstrucard, c.dataDescAcresc, c.idLiberarPedido, c.idContaBanco, c.idAcertoParcial, c.obs, c.idObra, c.dataPrimNeg,
                c.idCliente, cli.Nome as NomeCli, pl.Descricao as DescrPlanoConta, c.idFormaPagto, f.Nome as NomeFunc, cast(" +
                totalEmAberto + " as decimal(10,2)) as totalEmAberto, cast(" + totalEmDia + " as decimal(10,2)) as totalRecEmDia, cast(" +
                totalComAtraso + @" as decimal(10,2)) as totalRecComAtraso, '$$$' as Criterio, c.NumParcMax, c.idTrocaDevolucao, c.renegociada,
                c.dataCad, c.multa, c.idDevolucaoPagto, c.isParcelaCartao, c.IdContaRCartao, c.acrescimo, c.valorJurosCartao, c.tipoRecebimentoParcCartao,
                c.idSinal, c.usuCad, c.idAcertoCheque, 0 As numCheque, c.numArquivoRemessaCnab, c.numeroDocumentoCnab, c.idEncontroContas, c.idNf,
                if(c.idConta in (" + UtilsPlanoConta.ContasCredito(3) + "), 0, c.valorRec) as valorRecSemCredito, cli.Credito AS CreditoCliente" :
                "Count(*) as contagem";

            string sql = @"
                Select " + campos + @" From contas_receber c
                    Left Join cliente cli On (c.idCliente=cli.id_Cli)
                    Left Join plano_contas pl On (c.IdConta=pl.IdConta)
                    Left Join funcionario f On (c.UsuRec=f.IdFunc)
                    Where ValorVec>0 &where";

            string criterio = String.Empty;
            string where = String.Empty;

            if (idCliente > 0)
            {
                where += " And c.IdCliente=" + idCliente;
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCliente) + "    ";
                temFiltro = true;
            }

            if (idPedido > 0)
            {
                where += " And c.IdPedido=" + idPedido;
                criterio += "Pedido: " + idPedido + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataIniVenc))
            {
                where += " And DATAVEC>=?dtIniVenc";
                criterio += "Data venc.: " + dataIniVenc + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataFimVenc))
            {
                where += " And DATAVEC<=?dtFimVenc";

                if (!String.IsNullOrEmpty(dataIniVenc))
                    criterio += " até " + dataFimVenc + "    ";
                else
                    criterio += "Data venc.: até " + dataFimVenc + "    ";

                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataIniRec))
            {
                where += " And DATAREC>=?dtIniRec";
                criterio += "Data rec.: " + dataIniRec + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataFimRec))
            {
                where += " And DATAREC<=?dtFimRec";

                if (!String.IsNullOrEmpty(dataIniRec))
                    criterio += " até " + dataFimRec + "    ";
                else
                    criterio += "Data rec.: até " + dataFimRec + "    ";

                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataIniCad))
            {
                where += " And c.DATACAD>=?dtIniCad";
                criterio += "Data cad.: " + dataIniCad + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataFimCad))
            {
                where += " And c.DATACAD<=?dtFimCad";

                if (!String.IsNullOrEmpty(dataIniCad))
                    criterio += " até " + dataFimCad + "    ";
                else
                    criterio += "Data cad.: até " + dataFimCad + "    ";

                temFiltro = true;
            }

            if (vIniVenc > 0)
            {
                where += " And c.valorVec>=" + vIniVenc.ToString().Replace(',', '.');
                criterio += "Valor venc.: " + vIniVenc.ToString("C") + "    ";
                temFiltro = true;
            }

            if (vFinVenc > 0)
            {
                where += " And c.valorVec<=" + vFinVenc.ToString().Replace(',', '.');

                if (vIniVenc > 0)
                    criterio += "Até: " + vFinVenc.ToString("C") + "    ";
                else
                    criterio += "Valor venc.: até " + vFinVenc.ToString("C") + "    ";

                temFiltro = true;
            }

            if (vIniRec > 0)
            {
                where += " And c.valorRec>=" + vIniRec.ToString().Replace(',', '.');
                criterio += "Valor rec.: " + vIniRec.ToString("C") + "    ";
                temFiltro = true;
            }

            if (vFinRec > 0)
            {
                where += " And c.valorRec<=" + vFinRec.ToString().Replace(',', '.');

                if (vIniRec > 0)
                    criterio += "Até: " + vFinRec.ToString("C") + "    ";
                else
                    criterio += "Valor rec.: até " + vFinRec.ToString("C") + "    ";

                temFiltro = true;
            }

            if (!emAberto)
            {
                where += " And recebida=1";
                criterio += "Contas recebidas    ";
                temFiltro = true;
            }

            if (recEmDia && !recComAtraso)
            {
                where += " And (c.dataRec<=c.dataVec Or c.dataRec is null)";
                criterio += "Recebidas em dia    ";
                temFiltro = true;
            }
            else if (!recEmDia && recComAtraso)
            {
                where += " And (c.dataRec>c.dataVec Or c.dataRec is null)";
                criterio += "Recebidas com atraso    ";
                temFiltro = true;
            }
            else if (emAberto && !recEmDia && !recComAtraso)
            {
                where += " And (recebida=0 Or recebida is null)";
                criterio += "Contas em aberto    ";
                temFiltro = true;
            }

            if (!buscarParcCartao)
            {
                where += " And c.isParcelaCartao=false";
                temFiltro = true;
            }

            if (contasRenegociadas > 0)
            {
                temFiltro = true;

                switch (contasRenegociadas)
                {
                    case 1:
                        criterio += "Incluir contas renegociadas    ";
                        break;
                    case 2:
                        where += " AND c.Renegociada IS NOT NULL AND c.Renegociada AND c.ValorRec=0";
                        criterio += "Apenas contas renegociadas    ";
                        break;
                    case 3:
                        where += " AND IF(c.Renegociada IS NOT NULL AND c.Renegociada, c.ValorRec > 0, true)";
                        criterio += "Não incluir contas renegociadas    ";
                        break;
                }
            }

            if (buscaPedRepoGarantia)
            {
                sql += " UNION Select ";

                string camposUnion = selecionar ? @"null as idContaR, ped.idPedido, null as idAntecipContaRec, null as idConta,
                    ped.dataEntrega as dataVec, ped.total as valorVec, null as dataRec,
                    null as valorRec, null as juros, null as recebida, null as usuRec, null as idAcerto, ped.numParc,
                    ped.desconto, null as motivoDescontoAcresc, null as idFuncDescAcresc,
                    ped.numAutConstrucard, null as dataDescAcresc, ped.idLiberarPedido, null as idContaBanco, null as idAcertoParcial,
                    ped.obs, ped.idObra, null as dataPrimNeg,
                    ped.idCli as idCliente, cl.Nome as NomeCli, 'Reposição/Garantia' as DescrPlanoConta, ped.idFormaPagto, fu.Nome as NomeFunc, cast(" +
                    totalEmAberto + " as decimal(10,2)) as totalEmAberto, cast(" + totalEmDia + " as decimal(10,2)) as totalRecEmDia, cast(" +
                    totalComAtraso + @" as decimal(10,2)) as totalRecComAtraso, null as Criterio, null as NumParcMax, null as idTrocaDevolucao,
                    null as renegociada, ped.dataCad, null as multa, null as idDevolucaoPagto, null as isParcelaCartao, NULL AS IdContaRCartao, ped.acrescimo,
                    null as valorJurosCartao, null as tipoRecebimentoParcCartao, ped.idSinal, ped.usuCad,
                    null as idAcertoCheque, 0 As numCheque, null as numArquivoRemessaCnab, null as numeroDocumentoCnab, cl.Credito AS CreditoCliente" : "Count(*) as contagem";

                sql += camposUnion + @" from pedido ped
                Left Join cliente cl On (ped.idCli=cl.id_Cli)
                Left Join funcionario fu On (ped.idFunc=fu.IdFunc) ";

                sql += " WHERE (ped.TipoVenda=3 Or ped.TipoVenda=4) ";
                criterio += "Incluir pedidos de reposição/garantia.    ";
                temFiltro = true;

                if (idCliente > 0)
                {
                    sql += " And ped.idCli=" + idCliente;
                }

                if (idPedido > 0)
                {
                    sql += " And ped.IdPedido=" + idPedido;
                }

                if (!String.IsNullOrEmpty(dataIniCad))
                {
                    sql += " And ped.dataCad>=?dtIniCad";
                }

                if (!String.IsNullOrEmpty(dataFimCad))
                {
                    sql += " And ped.dataCad<=?dtFimCad";
                }
            }
            // Busca os cheques devolvidos do cliente.
            if (buscarChequeDevolvido)
            {
                sql += " Union Select ";

                string camposUnion = selecionar ? @"Null As idContaR, Null As idPedido, Null As idAntecipContaRec, Null As idConta,
                    ch.dataVenc As dataVec, ch.valor As valorVec, ac.dataAcerto As dataRec, iac.valorReceb As valorRec, Null As juros, Null As recebida, f.idFunc As usuRec,
                    Null As idAcerto, Null As numParc, 0 As desconto, Null As motivoDescontoAcresc, Null As idFuncDescAcresc, Null As numAutConstrucard,
                    Null As dataDescAcresc, Null As idLiberarPedido, Null As idContaBanco, Null As idAcertoParcial, ch.obs, Null As idObra, Null As dataPrimNeg,
                    cli.id_cli As idCliente, cli.nome as nomeCli, 'Cheque Devolvido' as DescrPlanoConta, Null As idFormaPagto, f.nome As nomeFunc,
                    Cast(" + totalEmAberto + @" As Decimal(10,2)) As totalEmAberto, Cast(" + totalEmDia + @" As Decimal(10,2)) As totalRecEmDia,
                    Cast(" + totalComAtraso + @" As Decimal(10,2)) As totalRecComAtraso, Null As criterio, Null As numParcMax, Null As idTrocaDevolucao,
                    Null As renegociada, ch.dataCad, Null As multa, Null As idDevolucaoPagto, Null As isParcelaCartao, NULL AS IdContaRCartao, Null As acrescimo, Null As valorJurosCartao,
                    Null As tipoRecebimentoParcCartao, Null As idSinal, Null As usuCad, ac.idAcertoCheque, ch.num As numCheque, Null As numArquivoRemessaCnab,
                    Null As numeroDocumentoCnab, Null As idEncontroContas, Null As idNf, Null As valorRecSemCredito, cli.Credito AS CreditoCliente" : "Count(*) As contagem";

                sql += camposUnion + @" From cheques ch
                    Left Join cliente cli On (ch.idCliente=cli.id_Cli)
                    Left Join item_acerto_cheque iac On (ch.idCheque=iac.idCheque)
                    Left Join acerto_cheque ac On (iac.idAcertoCheque=ac.idAcertoCheque)
                    Left Join funcionario f On (ac.idFunc=f.idFunc) Where ch.situacao=" + (int)Cheques.SituacaoCheque.Devolvido;

                if (idCliente > 0)
                    sql += " And ch.idCliente=" + idCliente;

                if (vIniVenc > 0)
                    sql += " And ch.valor>=" + vIniVenc.ToString().Replace(',', '.');

                if (vFinVenc > 0)
                    sql += " And ch.valor<=" + vFinVenc.ToString().Replace(',', '.');

                if (vIniRec > 0)
                    sql += " And iac.valorReceb>=" + vIniRec.ToString().Replace(',', '.');

                if (vFinRec > 0)
                    sql += " And iac.valorReceb<=" + vFinRec.ToString().Replace(',', '.');

                if (!String.IsNullOrEmpty(dataIniCad))
                    sql += " And ch.dataCad>=?dtIniCad";

                if (!String.IsNullOrEmpty(dataFimCad))
                    sql += " And ch.dataCad<=?dtFimCad";

                if (!String.IsNullOrEmpty(dataIniVenc))
                    sql += " And ch.dataVenc>=?dtIniVenc";

                if (!String.IsNullOrEmpty(dataFimVenc))
                    sql += " And ch.dataVenc<=?dtFimvenc";

                if (!String.IsNullOrEmpty(dataIniRec))
                    sql += " And ac.dataAcerto>=?dtIniRec";

                if (!String.IsNullOrEmpty(dataFimRec))
                    sql += " And ac.dataAcerto<=?dtFimRec";

                criterio += "Incluir cheques devolvidos    ";
                temFiltro = true;
            }

            sql = sql.Replace("&where", where);

            return sql.Replace("$$$", criterio);
        }

        /// <summary>
        /// Retorna o valor das contas do cliente por situação
        /// </summary>
        /// <param name="tipoValor">1-Em aberto, 2-Rec em dia, 3-Rec com atraso</param>
        public decimal GetHistValor(int tipoValor, uint idCliente, string dataIniVenc, string dataFimVenc, string dataIniRec,
            string dataFimRec, string dataIniCad, string dataFimCad, float vIniVenc, float vFinVenc, float vIniRec, float vFinRec,
            bool emAberto, bool recEmDia, bool recComAtraso, bool buscarParcCartao, int contasRenegociadas, bool buscarChequeDevolvido)
        {
            string sql = "Select " + (tipoValor != 1 ? "c.valorRec" : "c.valorVec") + @" As valor
                From contas_receber c ";

            sql += tipoValor == 1 ? "Where (c.recebida=0 Or c.recebida Is Null)" :
                tipoValor == 2 ? "Where c.dataRec<=c.dataVec " :
                tipoValor == 3 ? "Where c.dataRec>c.dataVec " : "Where 1";

            if (idCliente > 0)
                sql += " And c.IdCliente=" + idCliente;

            if (!String.IsNullOrEmpty(dataIniVenc))
                sql += " And c.dataVec>=?dtIniVenc";

            if (!String.IsNullOrEmpty(dataFimVenc))
                sql += " And c.dataVec<=?dtFimVenc";

            if (!String.IsNullOrEmpty(dataIniRec))
                sql += " And c.dataRec>=?dtIniRec";

            if (!String.IsNullOrEmpty(dataFimRec))
                sql += " And c.dataRec<=?dtFimRec";

            if (!String.IsNullOrEmpty(dataIniCad))
                sql += " And c.dataCad>=?dtIniCad";

            if (!String.IsNullOrEmpty(dataFimCad))
                sql += " And c.dataCad<=?dtFimCad";

            if (vIniVenc > 0)
                sql += " And c.valorVec>=" + vIniVenc.ToString().Replace(',', '.');

            if (vFinVenc > 0)
                sql += " And c.valorVec<=" + vFinVenc.ToString().Replace(',', '.');

            if (vIniRec > 0)
                sql += " And c.valorRec>=" + vIniRec.ToString().Replace(',', '.');

            if (vFinRec > 0)
                sql += " And c.valorRec<=" + vFinRec.ToString().Replace(',', '.');

            if (!emAberto)
                sql += " And c.recebida=1";

            if (recEmDia && !recComAtraso)
                sql += " And (c.dataRec<=c.dataVec Or c.dataRec is null)";
            else if (!recEmDia && recComAtraso)
                sql += " And (c.dataRec>c.dataVec Or c.dataRec is null)";
            else if (emAberto && !recEmDia && !recComAtraso)
                sql += " And (c.recebida=0 Or c.recebida Is Null)";

            if (!buscarParcCartao)
                sql += " And c.isParcelaCartao=False";

            if (contasRenegociadas > 1)
            {
                switch (contasRenegociadas)
                {
                    case 2:
                        sql += " AND c.Renegociada IS NOT NULL AND c.Renegociada AND c.ValorRec=0";
                        break;
                    case 3:
                        sql += " AND IF(c.Renegociada IS NOT NULL AND c.Renegociada, c.ValorRec > 0, true)";
                        break;
                }
            }

            // Busca o total de cheques devolvidos do cliente,
            // se o tipo de valor for em aberto então busca o total do cheque menos o que foi recebido, senão
            // se o tipo de valor for o que foi recebido em dia então busca o valor que foi recebido antes da data de vencimento do cheque, senão
            // busca o valor que foi recebido após a data de vencimento do cheque.
            if (buscarChequeDevolvido)
            {
                sql += @" Union All Select " + (tipoValor == 1 ? "Cast((ch.valor - ch.valorReceb) As Decimal(10,2))" :
                    tipoValor == 2 ? "If(ch.dataVenc >= ac.dataAcerto, iac.valorReceb, 0)" :
                    "If(ch.dataVenc < ac.dataAcerto, iac.valorReceb, 0)") + @" As valor
                    From cheques ch
                        Left Join item_acerto_cheque iac On (ch.idCheque=iac.idCheque)
                        Left Join acerto_cheque ac On (iac.idAcertoCheque=ac.idAcertoCheque)
                    Where ch.situacao=" + (int)Cheques.SituacaoCheque.Devolvido;

                if (idCliente > 0)
                    sql += " And ch.idCliente=" + idCliente;

                if (vIniVenc > 0)
                    sql += " And ch.valor>=" + vIniVenc.ToString().Replace(',', '.');

                if (vFinVenc > 0)
                    sql += " And ch.valor<=" + vFinVenc.ToString().Replace(',', '.');

                if (vIniRec > 0)
                    sql += " And iac.valorReceb>=" + vIniRec.ToString().Replace(',', '.');

                if (vFinRec > 0)
                    sql += " And iac.valorReceb<=" + vFinRec.ToString().Replace(',', '.');

                if (!String.IsNullOrEmpty(dataIniCad))
                    sql += " And ch.dataCad>=?dtIniCad";

                if (!String.IsNullOrEmpty(dataFimCad))
                    sql += " And ch.dataCad<=?dtFimCad";

                if (!String.IsNullOrEmpty(dataIniVenc))
                    sql += " And ch.dataVenc>=?dtIniVenc";

                if (!String.IsNullOrEmpty(dataFimVenc))
                    sql += " And ch.dataVenc<=?dtFimvenc";

                if (!String.IsNullOrEmpty(dataIniRec))
                    sql += " And ac.dataAcerto>=?dtIniRec";

                if (!String.IsNullOrEmpty(dataFimRec))
                    sql += " And ac.dataAcerto<=?dtFimRec";
            }

            sql = "Select Sum(temp.valor) From (" + sql + ") As temp";
            return ExecuteScalar<decimal>(sql, GetParamHist(dataIniVenc, dataFimVenc, dataIniRec, dataFimRec, dataIniCad, dataFimCad));
        }

        public IList<ContasReceber> GetForRptHist(uint idCliente, uint idPedido, string dataIniVenc, string dataFimVenc, string dataIniRec,
            string dataFimRec, string dataIniCad, string dataFimCad, float vIniVenc, float vFinVenc, float vIniRec, float vFinRec,
            bool emAberto, bool recEmDia, bool recComAtraso, bool buscarParcCartao, int contasRenegociadas, bool buscaPedRepoGarantia,
            bool buscarChequedevolvido, string sort)
        {
            bool temFiltro;
            var retorno = objPersistence.LoadData(SqlHist(idCliente, idPedido, dataIniVenc, dataFimVenc, dataIniRec, dataFimRec, dataIniCad, dataFimCad,
                vIniVenc, vFinVenc, vIniRec, vFinRec, emAberto, recEmDia, recComAtraso, buscarParcCartao, contasRenegociadas, buscaPedRepoGarantia, buscarChequedevolvido,
                sort, true, out temFiltro), GetParamHist(dataIniVenc, dataFimVenc, dataIniRec, dataFimRec, dataIniCad, dataFimCad)).ToArray();

            PreencheReferenciaPedidoECreditoHistoricoCliente(ref retorno);
            return retorno;
        }

        public ContasReceber[] GetListHist(uint idCliente, uint idPedido, string dataIniVenc, string dataFimVenc, string dataIniRec,
            string dataFimRec, string dataIniCad, string dataFimCad, float vIniVenc, float vFinVenc, float vIniRec, float vFinRec,
            bool emAberto, bool recEmDia, bool recComAtraso, bool buscarParcCartao, int contasRenegociadas, bool buscaPedRepoGarantia,
            bool buscarChequeDevolvido, string sort, string sortExpression, int startRow, int pageSize)
        {
            if (!String.IsNullOrEmpty(sortExpression))
                sort = "0";
            else
                switch (sort)
                {
                    case "1": // Vencimento
                        sortExpression = "DataVec desc"; break;
                    case "2": // Recebimento
                        sortExpression = " DataRec desc"; break;
                    case "3": // Situação (Em aberto, Recebida em dia, Recebida com atraso)
                        sortExpression = " Recebida, (DataRec>DataVec Or DataRec is null), DataVec desc"; break;
                }

            bool temFiltro;
            var lst = ((List<ContasReceber>)LoadDataWithSortExpression(SqlHist(idCliente, idPedido, dataIniVenc, dataFimVenc, dataIniRec, dataFimRec,  dataIniCad, dataFimCad,
                vIniVenc, vFinVenc, vIniRec, vFinRec, emAberto, recEmDia, recComAtraso, buscarParcCartao, contasRenegociadas, buscaPedRepoGarantia, buscarChequeDevolvido,
                sort, true, out temFiltro), sortExpression, startRow, pageSize, temFiltro, GetParamHist(dataIniVenc, dataFimVenc, dataIniRec, dataFimRec, dataIniCad, dataFimCad))).ToArray();

            return lst;
        }

        public int GetCountHist(uint idCliente, uint idPedido, string dataIniVenc, string dataFimVenc, string dataIniRec, string dataFimRec,
            string dataIniCad, string dataFimCad, float vIniVenc, float vFinVenc, float vIniRec, float vFinRec, bool emAberto,
            bool recEmDia, bool recComAtraso, bool buscarParcCartao, int contasRenegociadas, bool buscaPedRepoGarantia,
            bool buscarChequeDevolvido, string sort)
        {
            bool temFiltro;
            return GetCountWithInfoPaging(SqlHist(idCliente, idPedido, dataIniVenc, dataFimVenc, dataIniRec, dataFimRec, dataIniCad, dataFimCad,
                vIniVenc, vFinVenc, vIniRec, vFinRec, emAberto, recEmDia, recComAtraso, buscarParcCartao, contasRenegociadas, buscaPedRepoGarantia,
                buscarChequeDevolvido, sort, true, out temFiltro), temFiltro, null, GetParamHist(dataIniVenc, dataFimVenc, dataIniRec, dataFimRec, dataIniCad, dataFimCad));
        }

        private GDAParameter[] GetParamHist(string dataIniVenc, string dataFimVenc, string dataIniRec, string dataFimRec,
            string dataIniCad, string dataFimCad)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIniVenc))
                lstParam.Add(new GDAParameter("?dtIniVenc", DateTime.Parse(dataIniVenc + " 00:00")));

            if (!String.IsNullOrEmpty(dataFimVenc))
                lstParam.Add(new GDAParameter("?dtFimVenc", DateTime.Parse(dataFimVenc + " 23:59")));

            if (!String.IsNullOrEmpty(dataIniRec))
                lstParam.Add(new GDAParameter("?dtIniRec", DateTime.Parse(dataIniRec + " 00:00")));

            if (!String.IsNullOrEmpty(dataFimRec))
                lstParam.Add(new GDAParameter("?dtFimRec", DateTime.Parse(dataFimRec + " 23:59")));

            if (!String.IsNullOrEmpty(dataIniCad))
                lstParam.Add(new GDAParameter("?dtIniCad", DateTime.Parse(dataIniCad + " 00:00")));

            if (!String.IsNullOrEmpty(dataFimCad))
                lstParam.Add(new GDAParameter("?dtFimCad", DateTime.Parse(dataFimCad + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        /// <summary>
        /// Preenche a referência dos pedidos de cada conta recebida da lista.
        /// </summary>
        public void PreencheReferenciaPedidoECreditoHistoricoCliente(ref ContasReceber[] contasReceber)
        {
            var idsCliente = new List<int>();

            foreach (var contaReceber in contasReceber)
            {
                if (contaReceber.IdCliente > 0 && !idsCliente.Contains((int)contaReceber.IdCliente))
                    idsCliente.Add((int)contaReceber.IdCliente);

                var referencia = string.Empty;

                if (contaReceber.IdPedido > 0)
                {
                    var codCliente = PedidoDAO.Instance.ObtemValorCampo<string>("CodCliente", string.Format("idPedido={0}", contaReceber.IdPedido));
                    referencia = string.Format("{0} ({1})", contaReceber.IdPedido, codCliente);
                }
                else if (contaReceber.IdSinal > 0)
                {
                    var idsPedido = PedidoDAO.Instance.ObtemIdsPeloSinal(contaReceber.IdSinal.Value);

                    if (!string.IsNullOrEmpty(idsPedido))
                        foreach (var idPedido in idsPedido.Split(','))
                        {
                            var codCliente = PedidoDAO.Instance.ObtemValorCampo<string>("CodCliente", string.Format("idPedido={0}",
                                idPedido.Contains("...") ? idPedido.Replace("...","") : idPedido));
                            referencia += string.Format("{0} ({1}), ", idPedido, codCliente);
                        }
                }
                else if (contaReceber.IdAcerto > 0)
                {
                    var idsPedido = PedidoDAO.Instance.ObterIdsPedidoPeloAcerto(null, (int)contaReceber.IdAcerto.Value);

                    if (string.IsNullOrEmpty(idsPedido))
                    {
                        var idsLiberarPedido = LiberarPedidoDAO.Instance.ObterIdsLiberarPedidoPeloAcerto(null, (int)contaReceber.IdAcerto.Value);

                        if (!string.IsNullOrEmpty(idsLiberarPedido))
                        {
                            idsPedido = LiberarPedidoDAO.Instance.IdsPedidos(null, idsLiberarPedido);

                            if (!string.IsNullOrEmpty(idsPedido))
                                foreach (var idPedido in idsPedido.Split(','))
                                {
                                    var codCliente = PedidoDAO.Instance.ObtemValorCampo<string>("CodCliente", string.Format("idPedido={0}", idPedido));
                                    referencia += string.Format("{0} ({1}), ", idPedido, codCliente);
                                }
                        }
                    }
                    else
                        foreach (var idPedido in idsPedido.Split(','))
                        {
                            var codCliente = PedidoDAO.Instance.ObtemValorCampo<string>("CodCliente", string.Format("idPedido={0}", idPedido));
                            referencia += string.Format("{0} ({1}), ", idPedido, codCliente);
                        }
                }
                else if (contaReceber.IdLiberarPedido > 0)
                {
                    var idsPedido = LiberarPedidoDAO.Instance.IdsPedidos(null, contaReceber.IdLiberarPedido.Value.ToString());

                    if (!string.IsNullOrEmpty(idsPedido))
                        foreach (var idPedido in idsPedido.Split(','))
                        {
                            var codCliente = PedidoDAO.Instance.ObtemValorCampo<string>("CodCliente", string.Format("idPedido={0}", idPedido));
                            referencia += string.Format("{0} ({1}), ", idPedido, codCliente);
                        }
                }

                if (!string.IsNullOrEmpty(referencia))
                    contaReceber.ReferenciaPedidoHistoricoCliente = string.Format("Pedido(s): {0}", referencia.TrimEnd(' ').TrimEnd(','));
            }

            foreach (var idCliente in idsCliente)
                contasReceber[0].TotalCreditoCliente += ClienteDAO.Instance.GetCredito((uint)idCliente);
        }

        #endregion

        #region Busca contas a receber que foram dados descontos

        private string SqlContaComDesconto(uint idPedido, uint idLiberarPedido, uint idLoja, uint idCliente, string nomeCli, uint idFunc, decimal valorIniAcres,
            decimal valorFimAcres, decimal valorIniDesc, decimal valorFimDesc, string dataIni, string dataFim, string dataDescIni, string dataDescFim,
            uint idOrigemDesconto, bool selecionar, out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = false;
            filtroAdicional = " And (cr.desconto > 0 Or cr.acrescimo > 0)";

            string campos = selecionar ? "Distinct cr.*, cli.Nome as NomeCli, f.Nome as nomeFuncDesc, " +
                (!PedidoConfig.LiberarPedido ? "p.tipoEntrega as TipoEntrega, fp.Nome as nomeFunc, " : "") +
                "pl.Descricao as DescrPlanoConta, otd.descricao as DescrOrigemDescontoAcrescimo, '$$$' as Criterio" : "Count(*)";

            string criterio = String.Empty;

            string sql = "Select " + campos + @" From contas_receber cr
                Left Join cliente cli On (cr.IdCliente=cli.id_Cli)
                Left Join funcionario f On (f.idFunc=cr.idFuncDescAcresc)
                left join origem_troca_desconto otd on (cr.idOrigemDescontoAcrescimo = otd.idOrigemTrocaDesconto)" +
                (PedidoConfig.LiberarPedido ?
                @"Left Join liberarpedido lp On (cr.idLiberarPedido=lp.idLiberarPedido)
                Left Join produtos_liberar_pedido plp On (lp.idLiberarpedido=plp.idLiberarPedido)
                Left Join pedido p On (plp.idPedido=p.idPedido)" :
                "Left Join pedido p On (cr.idPedido=p.idPedido)") +
                @"Left Join funcionario fp On (p.idFunc=fp.idFunc)
                Left Join plano_contas pl On (cr.IdConta=pl.IdConta) Where 1 ?filtroAdicional?";

            if (idPedido > 0)
            {
                sql += " and cr.idPedido=" + idPedido;
                criterio += "Pedido: " + idPedido + "    ";
                temFiltro = true;
            }

            if (idLiberarPedido > 0)
            {
                if (FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber)
                    sql += " AND (cr.IdLiberarPedido=" + idLiberarPedido + @"
                        OR cr.idNf IN (SELECT idNf
                                        FROM pedidos_nota_fiscal
                                        WHERE idLiberarPedido=" + idLiberarPedido + "))";
                else
                    sql += " And cr.IdLiberarPedido=" + idLiberarPedido;

                criterio += "Liberação: " + idLiberarPedido + "    ";
                temFiltro = true;
            }

            if (idLoja > 0)
            {
                sql += string.Format(" AND cr.IdLoja={0}", idLoja);
                criterio += string.Format("Loja: {0}    ", LojaDAO.Instance.GetNome(idLoja));
            }

            if (idCliente > 0)
            {
                sql += " and cr.idCliente=" + idCliente;
                criterio += "Cliente: " + idCliente + "   ";
                temFiltro = true;
            }
            else if (!String.IsNullOrEmpty(nomeCli))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCli, null, 0, null, null, null, null, 0);
                sql += " And cr.idCliente in (" + ids + ")";
                criterio += "Clientes: " + ids + "   ";
                temFiltro = true;
            }

            if (idFunc > 0)
            {
                sql += " And fp.idFunc=" + idFunc;
                criterio += "Funcionário: " + FuncionarioDAO.Instance.GetNome(idFunc) + "    ";
                temFiltro = true;
            }

            if (valorIniAcres > 0)
            {
                sql += " And cr.acrescimo >= " + valorIniAcres.ToString().Replace(",", ".");
                criterio += "Acréscimo a partir de: " + valorIniAcres.ToString("C") + "    ";
                temFiltro = true;
            }

            if (valorFimAcres > 0)
            {
                sql += " And cr.acrescimo <= " + valorFimAcres.ToString().Replace(",", ".");
                criterio += "Acréscimo até: " + valorFimAcres.ToString("C") + "    ";
                temFiltro = true;
            }

            if (valorIniDesc > 0)
            {
                sql += " And cr.desconto >= " + valorIniDesc.ToString().Replace(",", ".");
                criterio += "Desconto a partir de: " + valorIniDesc.ToString("C") + "    ";
                temFiltro = true;
            }

            if (valorFimDesc > 0)
            {
                sql += " And cr.desconto <= " + valorFimDesc.ToString().Replace(",", ".");
                criterio += "Desconto até: " + valorFimDesc.ToString("C") + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " And cr.dataVec>=?dataIni";
                criterio += "Data Inicial: " + dataIni + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " And cr.dataVec<=?dataFim";
                criterio += "Data Final: " + dataFim + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataDescIni))
            {
                sql += " And cr.dataDescAcresc>=?dataDescIni";
                criterio += "Data Inicial: " + dataDescIni + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataDescFim))
            {
                sql += " And cr.dataDescAcresc<=?dataDescFim";
                criterio += "Data Final: " + dataDescFim + "    ";
                temFiltro = true;
            }

            if (idOrigemDesconto > 0)
            {
                sql += " AND cr.idOrigemDescontoAcrescimo=" + idOrigemDesconto;
                criterio += "Origem do Desconto/Acréscimo: " + OrigemTrocaDescontoDAO.Instance.ObtemDescricao(idOrigemDesconto) + "  ";
                temFiltro = true;
            }

            return sql.Replace("$$$", criterio);
        }

        public IList<ContasReceber> GetListContaComDesconto(uint idPedido, uint idLiberarPedido, uint idLoja, uint idCliente, string nomeCli, uint idFunc, decimal valorIniAcres,
            decimal valorFimAcres, decimal valorIniDesc, decimal valorFimDesc, string dataIni, string dataFim, string dataDescIni, string dataDescFim,
            uint idOrigemDesconto, string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? " cr.dataVec Desc" : sortExpression;

            bool temFiltro;
            string filtroAdicional;

            string sql = SqlContaComDesconto(idPedido, idLiberarPedido, idLoja, idCliente, nomeCli, idFunc, valorIniAcres, valorFimAcres, valorIniDesc, valorFimDesc,
                dataIni, dataFim, dataDescIni, dataDescFim, idOrigemDesconto, true, out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return LoadDataWithSortExpression(sql, sort, startRow, pageSize, temFiltro, filtroAdicional,
                GetParamContaComDesconto(dataIni, dataFim, dataDescIni, dataDescFim));
        }

        public int GetCountContaComDesconto(uint idPedido, uint idLiberarPedido, uint idLoja, uint idCliente, string nomeCli, uint idFunc, decimal valorIniAcres,
            decimal valorFimAcres, decimal valorIniDesc, decimal valorFimDesc, string dataIni, string dataFim, string dataDescIni, string dataDescFim,
            uint idOrigemDesconto)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlContaComDesconto(idPedido, idLiberarPedido, idLoja, idCliente, nomeCli, idFunc, valorIniAcres, valorFimAcres, valorIniDesc, valorFimDesc,
                dataIni, dataFim, dataDescIni, dataDescFim, idOrigemDesconto, true, out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, GetParamContaComDesconto(dataIni, dataFim, dataDescIni, dataDescFim));
        }

        public IList<ContasReceber> GetListContaComDescontoRpt(uint idPedido, uint idLiberarPedido, uint idLoja, uint idCliente, string nomeCli, uint idFunc, decimal valorIniAcres,
            decimal valorFimAcres, decimal valorIniDesc, decimal valorFimDesc, string dataIni, string dataFim, string dataDescIni, string dataDescFim,
            uint idOrigemDesconto)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlContaComDesconto(idPedido, idLiberarPedido, idLoja, idCliente, nomeCli, idFunc, valorIniAcres, valorFimAcres, valorIniDesc, valorFimDesc,
                dataIni, dataFim, dataDescIni, dataDescFim, idOrigemDesconto, true, out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional) + " Order By cr.dataVec Desc";

            return objPersistence.LoadData(sql, GetParamContaComDesconto(dataIni, dataFim, dataDescIni, dataDescFim)).ToList();
        }

        private GDAParameter[] GetParamContaComDesconto(string dataIni, string dataFim, string dataDescIni, string dataDescFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            if (!String.IsNullOrEmpty(dataDescIni))
                lstParam.Add(new GDAParameter("?dataDescIni", DateTime.Parse(dataDescIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataDescFim))
                lstParam.Add(new GDAParameter("?dataDescFim", DateTime.Parse(dataDescFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Retorna parâmetros

        private GDAParameter[] GetParam(string nomeCli, string dtIni, string dtFim, string dtIniLib, string dtFimLib,
            string dtIniAntecip, string dtFimAntecip, string dtCadIni, string dtCadFim, string obs, string tipoContaContabil,
            string numAutCartao, string numEstabCartao, string ultDigCartao)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeCli))
                lstParam.Add(new GDAParameter("?nomeCli", "%" + nomeCli + "%"));

            if (!String.IsNullOrEmpty(dtIni))
                lstParam.Add(new GDAParameter("?dtIni", DateTime.Parse(dtIni + " 00:00")));

            if (!String.IsNullOrEmpty(dtFim))
                lstParam.Add(new GDAParameter("?dtFim", DateTime.Parse(dtFim + " 23:59")));

            if (!String.IsNullOrEmpty(dtIniLib))
                lstParam.Add(new GDAParameter("?dtIniLib", DateTime.Parse(dtIniLib + " 00:00")));

            if (!String.IsNullOrEmpty(dtFimLib))
                lstParam.Add(new GDAParameter("?dtFimLib", DateTime.Parse(dtFimLib + " 23:59")));

            if (!String.IsNullOrEmpty(dtIniAntecip))
                lstParam.Add(new GDAParameter("?dtIniAntecip", DateTime.Parse(dtIniAntecip + " 00:00")));

            if (!String.IsNullOrEmpty(dtFimAntecip))
                lstParam.Add(new GDAParameter("?dtFimAntecip", DateTime.Parse(dtFimAntecip + " 23:59")));

            if (!String.IsNullOrEmpty(dtCadIni))
                lstParam.Add(new GDAParameter("?dataCadIni", DateTime.Parse(dtCadIni + " 00:00")));

            if (!String.IsNullOrEmpty(dtCadFim))
                lstParam.Add(new GDAParameter("?dataCadFim", DateTime.Parse(dtCadFim + " 23:59")));

            if (!string.IsNullOrEmpty(obs))
                lstParam.Add(new GDAParameter("?obs", "%" + obs + "%"));

            if (!String.IsNullOrEmpty(tipoContaContabil))
                lstParam.Add(new GDAParameter("?tipoContaContabil", tipoContaContabil));

            if (!string.IsNullOrEmpty(numAutCartao))
                lstParam.Add(new GDAParameter("?numAutCartao", numAutCartao));

            if (!string.IsNullOrEmpty(numEstabCartao))
                lstParam.Add(new GDAParameter("?numEstabCartao", numEstabCartao));

            if (!string.IsNullOrEmpty(ultDigCartao))
                lstParam.Add(new GDAParameter("?ultDigCartao", ultDigCartao));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        private GDAParameter[] GetParamRpt(string nomeCli, string dtIniVenc, string dtFimVenc, string dtIniRec, string dtFimRec,
            string dataIniCad, string dataFimCad, string dtIniLib, string dtFimLib, string dtIniAntecip, string dtFimAntecip, string obs)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeCli))
                lstParam.Add(new GDAParameter("?nomeCli", "%" + nomeCli + "%"));

            if (!String.IsNullOrEmpty(dtIniVenc))
                lstParam.Add(new GDAParameter("?dtIniVenc", DateTime.Parse(dtIniVenc + " 00:00")));

            if (!String.IsNullOrEmpty(dtFimVenc))
                lstParam.Add(new GDAParameter("?dtFimVenc", DateTime.Parse(dtFimVenc + " 23:59")));

            if (!String.IsNullOrEmpty(dtIniRec))
                lstParam.Add(new GDAParameter("?dtIniRec", DateTime.Parse(dtIniRec + " 00:00")));

            if (!String.IsNullOrEmpty(dtFimRec))
                lstParam.Add(new GDAParameter("?dtFimRec", DateTime.Parse(dtFimRec + " 23:59")));

            if (!String.IsNullOrEmpty(dataIniCad))
                lstParam.Add(new GDAParameter("?dataIniCad", DateTime.Parse(dataIniCad + " 00:00")));

            if (!String.IsNullOrEmpty(dataFimCad))
                lstParam.Add(new GDAParameter("?dataFimCad", DateTime.Parse(dataFimCad + " 23:59")));

            if (!String.IsNullOrEmpty(dtIniLib))
                lstParam.Add(new GDAParameter("?dtIniLib", DateTime.Parse(dtIniLib + " 00:00")));

            if (!String.IsNullOrEmpty(dtFimLib))
                lstParam.Add(new GDAParameter("?dtFimLib", DateTime.Parse(dtFimLib + " 23:59")));

            if (!String.IsNullOrEmpty(dtIniAntecip))
                lstParam.Add(new GDAParameter("?dtIniAntecip", DateTime.Parse(dtIniAntecip + " 00:00")));

            if (!String.IsNullOrEmpty(dtFimAntecip))
                lstParam.Add(new GDAParameter("?dtFimAntecip", DateTime.Parse(dtFimAntecip + " 23:59")));

            if (!string.IsNullOrEmpty(obs))
                lstParam.Add(new GDAParameter("?obs", "%" + obs + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Desconta conta a receber

        private static object _descontaAcrescentaContaReceberLock = new object();

        /// <summary>
        /// Marca desconto em parcela
        /// </summary>
        public void DescontaAcrescentaContaReceber(uint idContaR, decimal desconto, decimal acrescimo, uint? idOrigem, string motivo)
        {
            lock (_descontaAcrescentaContaReceberLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var contaReceberAtual = GetElementByPrimaryKey(transaction, idContaR);

                        var idPedido = ObtemValorCampo<uint?>(transaction, "idPedido", "idContaR=" + idContaR);
                        var recebida = ObtemValorCampo<bool>(transaction, "recebida", "idContaR=" + idContaR);

                        if (idOrigem.GetValueOrDefault() > 0 &&
                            OrigemTrocaDescontoDAO.Instance.GetElementByPrimaryKey(transaction, idOrigem.Value) == null)
                            throw new Exception("Origem inexistente.");

                        if (recebida)
                            throw new Exception("Esta conta já foi recebida. Não é possível aplicar o Desconto/Acréscimo");

                        var sql = "Update contas_receber set valorVec=(valorVec+Coalesce(desconto,0)-coalesce(acrescimo,0))-?desconto+?acrescimo Where idContaR=" + idContaR +
                            "; Update contas_receber set desconto=?desconto, acrescimo=?acrescimo, motivoDescontoAcresc=?motivo, idFuncDescAcresc=" + UserInfo.GetUserInfo.CodUser +
                            ", dataDescAcresc=now(), idOrigemDescontoAcrescimo=?idOrigem Where idContaR=" + idContaR;

                        var lstParam = new List<GDAParameter>();
                        lstParam.Add(new GDAParameter("?desconto", desconto));
                        lstParam.Add(new GDAParameter("?acrescimo", acrescimo));
                        lstParam.Add(new GDAParameter("?motivo", motivo));
                        lstParam.Add(new GDAParameter("?idOrigem", idOrigem));

                        objPersistence.ExecuteCommand(transaction, sql, lstParam.ToArray());

                        var contaReceberNova = GetElementByPrimaryKey(transaction, idContaR);

                        LogAlteracaoDAO.Instance.LogContaReceber(contaReceberAtual, contaReceberNova);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();
                        ErroDAO.Instance.InserirFromException(string.Format("DescontaAcrescentaContaReceber - IdContaR: {0}", idContaR), ex);
                        throw ex;
                    }
                }
            }
        }

        #endregion

        #region Obtém dados da Conta a Receber

        /// <summary>
        /// Obtém os ids das parcelas de cartão associadas à conta a receber/recebida.
        /// </summary>
        public List<int> ObterIdsContaRParcCartaoPeloIdContaR(GDASession session, int idContaR)
        {
            var idsContarRParcCartao = ExecuteMultipleScalar<int>(session, string.Format("SELECT DISTINCT IdContaR From contas_receber WHERE IdContaRCartao={0}", idContaR));

            return idsContarRParcCartao != null && idsContarRParcCartao.Count > 0 ? idsContarRParcCartao : new List<int>();
        }

        public List<uint> ObterIdsContaRPeloIdArquivoQuitacaoParcelaCartao(GDASession sessao, int idArquivoQuitacaoParcelaCartao)
        {
            return ExecuteMultipleScalar<uint>(sessao, "SELECT IdContaR FROM contas_receber WHERE IdArquivoQuitacaoParcelaCartao=" + idArquivoQuitacaoParcelaCartao);
        }

        /// <summary>
        /// Obtém o tipo de conta a receber.
        /// </summary>
        /// <param name="idContaR"></param>
        /// <returns></returns>
        public string ObtemTipoConta(uint idContaR)
        {
            return ExecuteScalar<string>("select " + SqlCampoDescricaoContaContabil("c") + @"
                from contas_receber c where c.idContaR=" + idContaR);
        }

        /// <summary>
        /// Obtém o idLIberar pedido da conta a receber.
        /// </summary>
        public int? ObterIdLiberarPedido(GDASession session, int idContaR)
        {
            return ExecuteScalar<int?>(session, string.Format("SELECT c.IdLiberarPedido FROM contas_receber c WHERE c.IdContaR={0}", idContaR));
        }

        /// <summary>
        /// Obtém os ids dos acertos associados às contas a receber informadas, separados por vírgula.
        /// <param name="idsContaR">Ids das contas a receber</param>
        /// <returns></returns>
        /// </summary>
        public string ObtemIdsAcerto(string idsContaR)
        {
            IList<string> idsAcerto = ExecuteMultipleScalar<string>("Select Distinct idAcerto From contas_receber Where idContaR In (" + idsContaR + ")", null);

            return String.Join(",", idsAcerto.ToArray());
        }

        /// <summary>
        /// Obtém o ID do acerto parcial associado à conta a receber.
        /// </summary>
        public int? ObterIdAcertoParcial(int idContaR)
        {
            return ObtemValorCampo<int?>("IdAcertoParcial", string.Format("IdContaR={0}", idContaR));
        }

        /// <summary>
        /// Obtem o id do cliente da conta
        /// </summary>
        /// <param name="idContaR"></param>
        /// <returns></returns>
        public uint ObtemIdCliente(uint idContaR)
        {
            return ObtemValorCampo<uint>("idCliente", "idContaR=" + idContaR);
        }

        public int? ObterIdObra(GDASession sessao, int idContaR)
        {
            return ObtemValorCampo<int?>(sessao, "IdObra", "IdContaR=" + idContaR);
        }

        #endregion

        #region Obtém contas a receber da NF-e

        public IList<uint> ObtemPelaNfe(uint idNf)
        {
            var numeroNfe = NotaFiscalDAO.Instance.ObtemNumeroNf(null, idNf);
            var idLoja = NotaFiscalDAO.Instance.ObtemIdLoja(null, idNf);
            var modelo = NotaFiscalDAO.Instance.ObterModelo(null, idNf);

            // Tenta encontrar primeiro usando o idNf, caso não encontre (não tenha feito separação), tenta pelo número da nota fiscal
            var ids = ExecuteMultipleScalar<uint>($@"
                SELECT cr.idContaR
                FROM contas_receber cr
                WHERE idNf = {idNf} AND cr.dataCad <> COALESCE(cr.dataRec, now())");

            if (ids.Count > 0)
            {
                return ids;
            }

            var campo = string.Empty;
            var sqlNf = SqlBuscarNF("cr", true, numeroNfe, true, true, true, (int)idLoja, modelo, out campo);

            // A condição "cr.dataCad<>cr.dataRec" foi criada para que contas de crédito não apareçam para gerar boleto
            var sql = $@"
                SELECT cr.idContaR
                FROM contas_receber cr
                    INNER JOIN (
                        {sqlNf}
                    ) AS nf ON (cr.{campo} = nf.{campo})
                WHERE CONCAT(', ', numeroNFe, ',') LIKE '%, {numeroNfe},%' AND cr.dataCad <> COALESCE(cr.dataRec, now())
                    AND (cr.recebimentoParcial IS NULL OR cr.recebimentoParcial = FALSE);";

            return ExecuteMultipleScalar<uint>(sql);
        }

        public IList<uint> ObterIdContaRPeloIdCte(uint idCte)
        {
            return ExecuteMultipleScalar<uint>($@"SELECT cr.idContaR
                FROM contas_receber cr
                WHERE IdCte={ idCte } and cr.DataCad <> COALESCE(cr.DataRec, now())");
        }

        public bool NfeTemContasReceber(uint idNf)
        {
            var numeroNfe = NotaFiscalDAO.Instance.ObtemNumeroNf(null, idNf);

            string campo, sql = @"
                select count(*) from (
                    select cr.idContaR
                    from contas_receber cr
                        inner join (
                            " + SqlBuscarNF("cr", true, numeroNfe, true, true, true, out campo) + @"
                        ) as nf on (cr." + campo + "=nf." + campo + @")
                    where !coalesce(recebida, false) and numeroNFe=" + numeroNfe + @"
                ) as temp";

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Obtém idPedido / idLiberarPedido

        /// <summary>
        /// Obtém os ids pedidos relacionados ao acerto passado
        /// </summary>
        /// <param name="idAcerto"></param>
        /// <returns></returns>
        // Chamado 13294.
        // Incluímos a variável "relatorio" para saber se o método está sendo chamado através de uma listagem, relatório ou não.
        // Pois, na listagem de acertos, por exemplo, não são exibidos todos os pedidos associados ao acerto, porém, em
        // vários outros lugares onde este método é chamado devem ser buscadas todas as referências.
        // private string ObtemIds(uint idAcerto, string campo)
        private string ObtemIds(GDASession sessao, uint idAcerto, string campo, bool relatorio)
        {
            // Foi retirada a opção idAcertoParcial para otimizar o comando
            string ids = ExecuteScalar<string>(sessao, @"select cast(group_concat(distinct " + campo + @" separator ',') as char)
                from contas_receber where idAcerto=" + idAcerto/* + " or idAcertoParcial=" + idAcerto*/);

            if (ids == null)
                return "";

            if (relatorio)
            {
                string[] vetIds = ids.Split(',');

                string[] retorno = new string[Math.Min(3, vetIds.Length)];
                Array.Copy(vetIds, retorno, retorno.Length);

                return String.Join(", ", retorno) + (vetIds.Length > 3 ? "..." : "");
            }
            else
                return ids;
        }

        /// <summary>
        /// Obtém os ids pedidos relacionados ao acerto passado
        /// </summary>
        /// <param name="idAcerto"></param>
        /// <returns></returns>
        public string ObtemIdsPedido(GDASession sessao, uint idAcerto, bool relatorio)
        {
            return ObtemIds(sessao, idAcerto, "idPedido", relatorio);
        }

        /// <summary>
        /// Obtém os ids liberação de pedidos relacionados ao acerto passado
        /// </summary>
        public string ObtemIdsLiberarPedido(GDASession session, uint idAcerto, bool relatorio)
        {
            return ObtemIds(session, idAcerto, "idLiberarPedido", relatorio);
        }

        public decimal ObtemValorVec(GDASession session, uint idContaR)
        {
            return ObtemValorCampo<decimal>(session, "valorVec", "idContaR=" + idContaR);
        }

        public uint ObtemIdLoja(GDASession session, uint idContaR)
        {
            return ObtemValorCampo<uint>(session, "idLoja", "idContaR=" + idContaR);
        }

        /// <summary>
        /// Busca os IDs das contas recebidas no acerto.
        /// </summary>
        public string ObterIdsContasRPeloAcerto(GDASession sessao, int idAcerto)
        {
            var idsContaR = ExecuteMultipleScalar<int>(sessao, $"SELECT DISTINCT(IdContaR) FROM contas_receber WHERE IdAcerto={ idAcerto }");

            return string.Join(",", idsContaR?.Where(f => f > 0)?.ToList() ?? new List<int>());
        }

        #endregion

        #region Total de desconto de um pedido

        /// <summary>
        /// Retorna o total de desconto em um pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public decimal GetTotalDescontoParcelas(uint idPedido)
        {
            string sql = "select coalesce(sum(coalesce(desconto,0)),0) from contas_receber where idPedido=" + idPedido;
            return ExecuteScalar<decimal>(sql);
        }

        #endregion

        #region Exclui conta a receber de valor excedente de pedido

        /// <summary>
        /// Exclui conta a receber de valor excedente de pedido
        /// </summary>
        public void ExcluiExcedentePedido(GDASession session, uint idPedido)
        {
            /* Chamado 34740. */
            if (ContaRecebidaValorExcedentePedido(session, (int)idPedido))
                throw new Exception("A conta do valor excedente do pedido foi recebida. " +
                    "Cancele o recebimento antes de gerar o valor excedente novamente ou cancelar o pedido em conferência.");

            foreach (var conta in objPersistence.LoadData(session, "Select * from contas_receber Where idPedido=" + idPedido +
                " And idConta=" + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ValorExcedente)).ToList())
                Delete(session, conta);
        }

        /// <summary>
        /// Verifica se a conta do valor excedente do pedido foi recebida.
        /// </summary>
        private bool ContaRecebidaValorExcedentePedido(GDASession session, int idPedido)
        {
            return objPersistence.ExecuteSqlQueryCount(session,
                string.Format(@"SELECT COUNT(*) FROM contas_receber cr
                    WHERE cr.IdPedido={0} AND cr.Recebida=1 AND cr.ValorExcedentePCP=1", idPedido)) > 0;
        }

        #endregion

        #region Retorna o valor de débitos do cliente

        #region SQL

        private string SqlDebitos(uint idCliente, uint idPedido, uint idLiberarPedido, string idsPedido, string idsContasR,
            string idsCheques, string buscarItens, TipoDebito tipoDebito, int tipoBuscaData, string dataIni, string dataFim, bool selecionar)
        {
            if (String.IsNullOrEmpty(buscarItens))
                buscarItens = "1,2,3,4,5";

            List<string> itensBuscar = new List<string>(buscarItens.Split(','));

            string camposVazios = @", null as idformapagto, null as idAcerto, null as idAcertoParcial, NULL AS IdObra, null as idConta,
                null as idFuncDescAcresc, null as idContaBanco, null as valorRec, null as dataRec, null as dataPrimNeg, null as Juros,
                false as Recebida, null as usuRec, null as numParc, null as numParcMax, null as Desconto, null as motivoDescontoAcresc,
                null as dataDescAcresc, null as numAutConstrucard, null as Obs, null as idTrocaDevolucao, null as multa,
                null as idDevolucaoPagto, null as isParcelaCartao, NULL AS IdContaRCartao, null as acrescimo, null as valorJurosCartao, null as tipoRecebimentoParcCartao, null as usuCad,
                null as idAcertoCheque, null as numArquivoRemessaCnab, null as numeroDocumentoCnab";

            string sql = "";

            // Busca débitos de contas a receber
            if (itensBuscar.Contains("1") && (tipoDebito == TipoDebito.Todos || tipoDebito == TipoDebito.ContasAReceberTotal || tipoDebito == TipoDebito.ContasAReceberAntecipadas))
            {
                string camposCr = @"c.idContaR, c.idNf, c.idEncontroContas" + SqlBuscarNF("c", true, 0, false, false) + @", null as numCheque,
                    c.idCliente as idCliente, cast(concat(pc.Descricao, if(c.idAntecipContaRec is not null, concat(' (Antecipação: ',
                    c.idAntecipContaRec, ')'), '')) as char) as DescrPlanoConta, c.valorVec as valorVec, c.idPedido, c.idLiberarPedido, c.dataVec, c.renegociada,
                    c.dataCad, c.idSinal, c.idAntecipContaRec" +
                    /* Chamado 52394.
                     * Caso seja incluso algum item no Replace, o texto deverá ser incluído com letras minúsculas. */
                    camposVazios.ToLower()
                        .Replace("null as numparc, null as numparcmax", "c.numparc as numparc, c.numparcmax as numparcmax")
                        .Replace("null as idacertoparcial", "c.idacertoparcial")
                        .Replace("null as idobra", "c.idobra");

                sql = "select " + camposCr + @"
                    From contas_receber c
                        left join plano_contas pc on (c.idConta=pc.idConta)
                    where !coalesce(recebida,false) and c.valorVec>0 and c.idCliente=?idCliente" +
                        (!String.IsNullOrEmpty(idsContasR) ? " and c.idContaR not in (" + idsContasR.Trim(',') + ")" : "");

                sql += " And (c.isParcelaCartao=false or c.isParcelaCartao is null)";

                if (tipoDebito == TipoDebito.ContasAReceberAntecipadas)
                    sql += " and c.idAntecipContaRec is not null";
            }

            // Busca débitos de sinais de pedidos
            if ((itensBuscar.Contains("1") || itensBuscar.Contains("2") || itensBuscar.Contains("3") || itensBuscar.Contains("4")) &&
                (tipoDebito == TipoDebito.Todos || tipoDebito == TipoDebito.PedidosEmAberto))
            {
                string camposPedidoSinal = @"null as idContaR, null as idNf, null as idEncontroContas, null as numeroNFe, null as numCheque,
                    p.idCli as idCliente, 'Sinal do pedido' as DescrPlanoConta, p.valorEntrada as valorVec,
                    p.idPedido, p.idLiberarPedido, null as dataVec, false as renegociada, p.dataCad, p.idSinal, null as idAntecipContaRec" + camposVazios;

                if (!String.IsNullOrEmpty(sql))
                    sql += " union ";

                sql += "select " + camposPedidoSinal + @"
                    from pedido p
                    where p.idSinal is null and p.idCli=?idCliente and p.valorEntrada>0 " +
                        (!String.IsNullOrEmpty(idsPedido) ? " and p.idPedido Not In (" + idsPedido.Trim(',') + ")" : "") + @"
                        and p.tipoVenda in (" + (int)Pedido.TipoVendaPedido.APrazo + "," + (int)Pedido.TipoVendaPedido.AVista + @")
                        and p.situacao<>" + (int)Pedido.SituacaoPedido.Cancelado + @"
                        AND p.TipoPedido<>" + (int)Pedido.TipoPedidoEnum.Producao + @"
                        and idPagamentoAntecipado is null ";
            }

            //Busca os pedidos conferidos
            if ((itensBuscar.Contains("2") || itensBuscar.Contains("3")) &&
                (FinanceiroConfig.DebitosLimite.EmpresaConsideraPedidoAtivoLimite || FinanceiroConfig.DebitosLimite.EmpresaConsideraPedidoConferidoLimite) &&
                (tipoDebito == TipoDebito.Todos || tipoDebito == TipoDebito.PedidosEmAberto))
            {
                if (!String.IsNullOrEmpty(sql))
                    sql += " union ";

                string situacaoPedido = ((itensBuscar.Contains("3") && FinanceiroConfig.DebitosLimite.EmpresaConsideraPedidoConferidoLimite
                    ? (int)Pedido.SituacaoPedido.Conferido + "," + (int)Pedido.SituacaoPedido.LiberadoParcialmente + "," : string.Empty) +
                    (itensBuscar.Contains("2") && FinanceiroConfig.DebitosLimite.EmpresaConsideraPedidoAtivoLimite ? (int)Pedido.SituacaoPedido.Ativo + "," +
                    (int)Pedido.SituacaoPedido.AtivoConferencia : string.Empty)).Trim(',');

                if (String.IsNullOrEmpty(situacaoPedido))
                    situacaoPedido = "0";

                // Sempre subtrai o valor do sinal do pedido, pois o valor de entrada já é calculado no item acima, já o valor do pagto antecipado
                // é calculado somente se for recebido, não retorna o idSinal para que não apareça na referência na listagem
                string camposPedidosConferidos = @"null as idContaR, null as idNf, null as idEncontroContas, null as numeroNFe, null as numCheque,
                    p.idCli as idCliente, Cast(Concat('Pedido ', if(p.situacao=1, 'ativo',
                    'conferido')) as char) as descrPlanoConta, cast((p.total-(Coalesce(if(/*p.idSinal is not null and */coalesce(s.isPagtoAntecipado,false)=false,
                    p.valorEntrada, 0), 0) + Coalesce(if(p.idPagamentoAntecipado is not null, p.valorPagamentoAntecipado, 0), 0))) as decimal(12,2)) as valorVec,
                    p.idPedido, null as idLiberarPedido, null as dataVenc, false as renegociada, p.dataCad, null as idSinal, null as idAntecipContaRec" + camposVazios;

                sql += @"
                    select " + camposPedidosConferidos + @" from pedido p
                        left join sinal s on (p.idSinal=s.idSinal)
                    where p.idCli=?idCliente " +
                        (!String.IsNullOrEmpty(idsPedido) ? " and p.idPedido Not In (" + idsPedido.Trim(',') + ")" : "") + @"
                        and p.situacao In (" + situacaoPedido + @")
                        and p.tipoVenda in (" + (int)Pedido.TipoVendaPedido.APrazo + "," + (int)Pedido.TipoVendaPedido.AVista + @")
                        AND p.TipoPedido<>" + (int)Pedido.TipoPedidoEnum.Producao + @"
                    having valorVec>0";
            }

            // Se for liberação, busca débitos de pedidos confirmados liberação,
            // não retorna o idSinal e o idliberarpedido para que não apareça na referência na listagem
            if (PedidoConfig.LiberarPedido && (itensBuscar.Contains("3") || itensBuscar.Contains("4")) &&
                (tipoDebito == TipoDebito.Todos || tipoDebito == TipoDebito.PedidosEmAberto))
            {
                if (!String.IsNullOrEmpty(sql))
                    sql += " union ";

                string situacaoBusca = ((itensBuscar.Contains("4") ? (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + "," + (int)Pedido.SituacaoPedido.LiberadoParcialmente : "")).Trim(',');

                if (String.IsNullOrEmpty(situacaoBusca))
                    situacaoBusca = "0";

                string camposPedidoConfLib = @"null as idContaR, null as idNf, null as idEncontroContas, null as numeroNFe, null as numCheque,
                    p.idCli as idCliente, 'Pedido confirmado' as DescrPlanoConta,
                    cast((Coalesce(pe.total,p.total)-(Coalesce(if((p.valorEntrada > 0 Or p.idSinal is not null) and coalesce(s.isPagtoAntecipado,false)=false, p.valorEntrada, 0), 0)+
                    Coalesce(if(p.idPagamentoAntecipado is not null, p.valorPagamentoAntecipado, 0), 0)))-
                    coalesce(round((select sum(pp.total/pp.qtde*plp.qtdeCalc) from produtos_pedido pp inner join produtos_liberar_pedido plp on
                    (pp.idProdPed=plp.idProdPed) where plp.idPedido=p.idPedido and plp.qtdeCalc>0),2),0) as decimal(12,2)) as valorVec,
                    p.idPedido, null as idLiberarPedido, p.dataCad as dataVec, false as renegociada, if(p.situacao=" +
                    (int)Pedido.SituacaoPedido.LiberadoParcialmente + @", lp.DataLiberacao, if(p.situacao=" +
                    (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + @", p.dataConf, p.dataCad)) as dataCad, null as idSinal, null as idAntecipContaRec" + camposVazios;

                sql += " select " + camposPedidoConfLib + @"
                    From pedido p
                        left join sinal s on (p.idSinal=s.idSinal)
                        left join liberarpedido lp on (p.idLiberarPedido=lp.idLiberarPedido)
                        left join pedido_espelho pe On (p.idPedido=pe.idPedido)
                    where p.situacao In (" + situacaoBusca + ") and p.idCli=?idCliente " +
                        (!String.IsNullOrEmpty(idsPedido) ? " and p.idPedido Not In (" + idsPedido.Trim(',') + ")" : "") + @"
                        and p.tipoVenda in (" + (int)Pedido.TipoVendaPedido.APrazo + "," + (int)Pedido.TipoVendaPedido.AVista + @")
                        AND p.TipoPedido<>" + (int)Pedido.TipoPedidoEnum.Producao + @"
                    having valorVec>0";
            }

            // Busca débitos de cheques
            if (itensBuscar != null && itensBuscar.Contains("5") && FinanceiroConfig.DebitosLimite.EmpresaConsideraChequeLimite &&
                /*(login.IdCliente == 0 || login.IdCliente == null) &&*/
                (tipoDebito == TipoDebito.Todos || tipoDebito == TipoDebito.ChequesTotal || tipoDebito == TipoDebito.ChequesEmAberto ||
                tipoDebito == TipoDebito.ChequesDevolvidos || tipoDebito == TipoDebito.ChequesProtestados))
            {
                if (!String.IsNullOrEmpty(sql))
                    sql += " union ";

                string camposCheques = @"null as idContaR, null as idNf, null as idEncontroContas, null as numeroNFe, c.num as numCheque,
                    c.idCliente as idCliente, cast(concat('Cheque ',
                    if(c.situacao=" + (int)Cheques.SituacaoCheque.EmAberto + @", 'em aberto',
                    if(c.situacao=" + (int)Cheques.SituacaoCheque.Devolvido + @", 'devolvido',
                    if(c.situacao=" + (int)Cheques.SituacaoCheque.Protestado + @", 'protestado',
                    if(c.situacao=" + (int)Cheques.SituacaoCheque.Trocado + @", 'trocado',
                    if(c.situacao=" + (int)Cheques.SituacaoCheque.Compensado + @", 'compensado (não vencido)', ''))))), ' Banco/Ag./Conta ', c.Banco,
                    '/', c.Conta, '/', c.Agencia) as char) as DescrPlanoConta,
                    (c.valor - coalesce(c.valorReceb, 0)) as valorVec, null as idPedido, null as idLiberarPedido, dataVenc as dataVec, false as renegociada,
                    c.dataCad, null as idSinal, null as idAntecipContaRec" + camposVazios;

                // Verifica quais situações de cheques serão consideradas
                string situacoes = String.Empty;
                switch (tipoDebito)
                {
                    case TipoDebito.Todos:
                    case TipoDebito.ChequesTotal:
                        situacoes = (int)Cheques.SituacaoCheque.EmAberto + ", " + (int)Cheques.SituacaoCheque.Devolvido + ", " +
                            (int)Cheques.SituacaoCheque.Trocado + ", " + (int)Cheques.SituacaoCheque.Protestado; break;
                    case TipoDebito.ChequesEmAberto:
                        situacoes = (int)Cheques.SituacaoCheque.EmAberto + "," + (int)Cheques.SituacaoCheque.Trocado; break;
                    case TipoDebito.ChequesDevolvidos:
                        situacoes = ((int)Cheques.SituacaoCheque.Devolvido).ToString(); break;
                    case TipoDebito.ChequesProtestados:
                        situacoes = ((int)Cheques.SituacaoCheque.Protestado).ToString(); break;
                }

                // No caso da vidrometro, o cheque deverá continuar debitando do limite até 10 dias depois de seu vencimento, para as outras empresas,
                // o cheque não deverá constar no limite até 2 dias depois de seu vencimento
                var diasConsiderarChequeCompensado = FinanceiroConfig.FinanceiroRec.DiasConsiderarChequeCompensado;

                // Considera no limite os cheques de terceiro em aberto e devolvido ou compensado e não vencidos e o restante
                // a ser pago de cheques trocados que não foram totalmente quitados
                sql += @"
                    select " + camposCheques + @" from cheques c
                    where c.idCliente=?idCliente and (c.valor - coalesce(c.valorReceb, 0)) > 0
                        and (c.situacao in (" + situacoes + ")" +
                            (tipoDebito == TipoDebito.Todos || tipoDebito == TipoDebito.ChequesTotal || tipoDebito == TipoDebito.ChequesEmAberto ?
                                "Or (" + (FinanceiroConfig.DebitosLimite.ConsiderarChequeDepositadoVencidoNoLimite ? "" : "c.idDeposito is null And") + @"
                                    c.situacao=" + (int)Cheques.SituacaoCheque.Compensado + @" And dataVenc>date_add(now(), INTERVAL " + diasConsiderarChequeCompensado +" DAY))" :
                            String.Empty) + @"
                        )" +
                        (!String.IsNullOrEmpty(idsCheques) ? " and c.idCheque not in (" + idsCheques.Trim(',') + ")" : "");
            }

            string criterio = "";
            string campos = selecionar ? "temp.*, c.nome as NomeCli, c.Credito as CreditoCliente, c.cpf_cnpj As CpfCnpjCliente, c.limite As LimiteCliente, '$$$' as Criterio" :
                "count(*)";

            if (idCliente > 0)
            {
                sql = sql.Replace("?idCliente", idCliente.ToString());
                criterio = "Cliente: " + idCliente + " - " + ClienteDAO.Instance.GetNome(idCliente) + "    ";
            }
            else
            {
                while (sql.Contains("?idCliente"))
                {
                    int index = sql.IndexOf("?idCliente");
                    string inicio = sql.Substring(0, index);
                    sql = inicio + inicio.Substring(inicio.LastIndexOf(" ") + 1).TrimEnd('=') + sql.Substring(index + 10);
                }
            }

            if (String.IsNullOrEmpty(sql))
                return String.Empty;

            sql = "select " + campos + @"
                from (" + sql + @") as temp
                    left join cliente c on (temp.idCliente=c.id_cli)
                where 1";

            if (idPedido > 0)
            {
                sql += " and temp.idPedido=" + idPedido;
                criterio += "Pedido: " + idPedido + "    ";
            }

            if (idLiberarPedido > 0)
            {
                sql += " and temp.idLiberarPedido=" + idLiberarPedido;
                criterio += "Liberação: " + idLiberarPedido + "    ";
            }

            string campoData = tipoBuscaData == 1 ? "dataCad" : "dataVec";
            string descrData = tipoBuscaData == 1 ? "Débito" : "Venc.";

            if (tipoBuscaData > 0 && !String.IsNullOrEmpty(dataIni))
            {
                sql += " and temp." + campoData + ">=?dataIni";
                criterio += "Data " + descrData + " Início: " + dataIni + "    ";
            }

            if (tipoBuscaData > 0 && !String.IsNullOrEmpty(dataFim))
            {
                sql += " and temp." + campoData + "<=?dataFim";
                criterio += "Data " + descrData + " Fim: " + dataFim + "    ";
            }

            string naoBuscar = "Não buscar ";

            if (!itensBuscar.Contains("1"))
                naoBuscar += "contas a receber, ";

            if (!itensBuscar.Contains("2") && FinanceiroConfig.DebitosLimite.EmpresaConsideraPedidoAtivoLimite)
                naoBuscar += "pedidos abertos, ";

            if (!itensBuscar.Contains("3") && (FinanceiroConfig.DebitosLimite.EmpresaConsideraPedidoAtivoLimite || Glass.Configuracoes.PedidoConfig.LiberarPedido))
                naoBuscar += "pedidos conferidos, ";

            if (!itensBuscar.Contains("4") && Glass.Configuracoes.PedidoConfig.LiberarPedido)
                naoBuscar += "pedidos confirmados, ";

            if (!itensBuscar.Contains("5") && FinanceiroConfig.DebitosLimite.EmpresaConsideraChequeLimite)
                naoBuscar += "cheques em aberto, ";

            if (naoBuscar.Length > 11)
                criterio += naoBuscar.TrimEnd(' ', ',') + "    ";

            sql = sql.Replace("$$$", criterio.Trim());

            return sql;
        }

        private GDAParameter[] GetDebitosParams(string dataIni, string dataFim)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lst.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lst.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lst.ToArray();
        }

        #endregion

        /// <summary>
        /// Retorna a lista de débitos do cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idPedido"></param>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public IList<ContasReceber> GetDebitosRpt(uint idCliente, uint idPedido, uint idLiberarPedido, string buscarItens,
            int tipoBuscaData, string dataIni, string dataFim, int ordenar)
        {
            // Busca débitos do cliente logado
            if (idCliente == 0 && UserInfo.GetUserInfo.IdCliente > 0)
                idCliente = UserInfo.GetUserInfo.IdCliente.Value;

            string sql = SqlDebitos(idCliente, idPedido, idLiberarPedido, null, null, null, buscarItens, TipoDebito.Todos, tipoBuscaData,
                dataIni, dataFim, true);

            switch (ordenar)
            {
                case 1:
                    sql += " Order By c.Nome"; break;
                case 2:
                    sql += " Order By ValorVec Desc"; break;
                case 3:
                    sql += " Order By idPedido"; break;
                case 4:
                    sql += " Order By idLiberarPedido"; break;
            }

            ContasReceber[] lstContas = objPersistence.LoadData(sql, GetDebitosParams(dataIni, dataFim)).ToArray();

            if (FinanceiroConfig.ContasReceber.ExibirPedidosLiberacaoDebitos)
                foreach (ContasReceber cr in lstContas)
                    if (cr.IdLiberarPedido > 0)
                        cr.PedidosLiberacao = PedidoDAO.Instance.ObtemValorCampo<string>(
                            "cast(group_concat(idPedido) as char)", "idPedido is not null and idLiberarPedido=" + cr.IdLiberarPedido);

            return lstContas;
        }

        /// <summary>
        /// Retorna a lista de débitos do cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idPedido"></param>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public IList<ContasReceber> GetDebitosList(uint idCliente, uint idPedido, uint idLiberarPedido, string buscarItens,
            int ordenar, int tipoBuscaData, string dataIni, string dataFim, string sortExpression, int startRow, int pageSize)
        {
            // Busca débitos do cliente logado
            if (idCliente == 0 && UserInfo.GetUserInfo.IdCliente > 0)
                idCliente = UserInfo.GetUserInfo.IdCliente.Value;

            switch (ordenar)
            {
                case 1:
                    sortExpression = "c.Nome"; break;
                case 2:
                    sortExpression = "ValorVec Desc"; break;
                case 3:
                    sortExpression = "idPedido"; break;
                case 4:
                    sortExpression = "idLiberarPedido"; break;
                case 5:
                    sortExpression = "DataVec Desc"; break;
                case 6:
                    sortExpression = "DataVec ASC"; break;
            }

            return LoadDataWithSortExpression(SqlDebitos(idCliente, idPedido, idLiberarPedido, null, null, null, buscarItens, TipoDebito.Todos,
                tipoBuscaData, dataIni, dataFim, true), sortExpression, startRow, pageSize, GetDebitosParams(dataIni, dataFim));
        }

        public int GetDebitosCount(uint idCliente, uint idPedido, uint idLiberarPedido, string buscarItens,
            int ordenar, int tipoBuscaData, string dataIni, string dataFim)
        {
            // Busca débitos do cliente logado
            if (idCliente == 0 && UserInfo.GetUserInfo.IdCliente > 0)
                idCliente = UserInfo.GetUserInfo.IdCliente.Value;

            return objPersistence.ExecuteSqlQueryCount(SqlDebitos(idCliente, idPedido, idLiberarPedido, null, null, null,
                buscarItens, TipoDebito.Todos, tipoBuscaData, dataIni, dataFim, false), GetDebitosParams(dataIni, dataFim));
        }

        /// <summary>
        /// Retorna a lista de débitos do cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idPedido"></param>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public IList<ContasReceber> GetDebitosListParceiros(uint idCliente, uint idPedido, uint idLiberarPedido, bool buscarChequesEmAberto,
            int ordenar, string sortExpression, int startRow, int pageSize)
        {
            // Busca débitos do cliente logado
            if (UserInfo.GetUserInfo.IdCliente > 0)
                idCliente = UserInfo.GetUserInfo.IdCliente.Value;
            else
                return new ContasReceber[0];

            switch (ordenar)
            {
                case 1:
                    sortExpression = "c.Nome"; break;
                case 2:
                    sortExpression = "ValorVec Desc"; break;
            }

            return LoadDataWithSortExpression(SqlDebitos(idCliente, idPedido, idLiberarPedido, null, null, null,
                null, TipoDebito.Todos, 0, null, null, true), sortExpression, startRow, pageSize);
        }

        public int GetDebitosCountParceiros(uint idCliente, uint idPedido, uint idLiberarPedido, bool buscarChequesEmAberto,
            int ordenar)
        {
            // Busca débitos do cliente logado
            if (UserInfo.GetUserInfo.IdCliente > 0)
                idCliente = UserInfo.GetUserInfo.IdCliente.Value;
            else
                return 0;

            if ((idCliente == 0 && idPedido == 0 && idLiberarPedido == 0) && !UserInfo.GetUserInfo.IsAdministrador)
                return 0;

            return objPersistence.ExecuteSqlQueryCount(SqlDebitos(idCliente, idPedido, idLiberarPedido, null, null, null,
                null, TipoDebito.Todos, 0, null, null, false));
        }

        /// <summary>
        /// Retorna os débitos do cliente passado pelo tipo
        /// </summary>
        public decimal GetDebitosByTipo(uint idCliente, TipoDebito tipoDebito)
        {
            return GetDebitosByTipo(null, idCliente, tipoDebito);
        }

        /// <summary>
        /// Retorna os débitos do cliente passado pelo tipo
        /// </summary>
        public decimal GetDebitosByTipo(GDASession session, uint idCliente, TipoDebito tipoDebito)
        {
            if (idCliente == 0)
                return 0;

            string sql = SqlDebitos(idCliente, 0, 0, null, null, null, null, tipoDebito, 0, null, null, true);
            return String.IsNullOrEmpty(sql) ? 0 :
                ExecuteScalar<decimal>(session, "select sum(valorVec) from (" + sql + ") as temp");
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna o valor do débito do cliente com a empresa, descontando o crédito que o mesmo tiver
        /// </summary>
        public decimal GetDebitos(uint idCliente, string idsPedidoLib)
        {
            return GetDebitos(null, idCliente, idsPedidoLib);
        }

        /// <summary>
        /// Retorna o valor do débito do cliente com a empresa, descontando o crédito que o mesmo tiver
        /// </summary>
        public decimal GetDebitos(GDASession sessao, uint idCliente, string idsPedidoLib)
        {
            if (idCliente == 0)
                return 0;

            return GetDebitos(sessao, idCliente, idsPedidoLib, null, null);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna o valor do débito do cliente com a empresa, descontando o crédito que o mesmo tiver
        /// </summary>
        public decimal GetDebitos(uint idCliente, string idsPedidoLib, string idsContasR, string idsChequesR)
        {
            return GetDebitos(null, idCliente, idsPedidoLib, idsContasR, idsChequesR);
        }

        /// <summary>
        /// Retorna o valor do débito do cliente com a empresa, descontando o crédito que o mesmo tiver
        /// </summary>
        public decimal GetDebitos(GDASession sessao, uint idCliente, string idsPedidoLib, string idsContasR, string idsChequesR)
        {
            if (idCliente == 0) return 0;

            string sql = String.Empty;

            try
            {
                sql = "select sum(valorVec) from (" + SqlDebitos(idCliente, 0, 0, idsPedidoLib, idsContasR, idsChequesR, null,
                    TipoDebito.Todos, 0, null, null, true) + ") as temp";

                decimal debitos = ExecuteScalar<decimal>(sessao, sql);
                decimal creditoCliente = ClienteDAO.Instance.GetCredito(sessao, idCliente);

                return creditoCliente >= debitos ? 0 : debitos - creditoCliente;
            }
            catch (Exception ex)
            {
                Exception ex1 = new Exception("Falha ao recuperar débitos. Sql: " + sql);
                ErroDAO.Instance.InserirFromException("Débitos", ex1);

                throw ex;
            }
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna o valor do débito de cheques do cliente com a empresa.
        /// </summary>
        public decimal GetDebitosCheque(uint idCliente, string idsPedidosLib, string idsContasR, string idsChequesR)
        {
            return GetDebitosCheque(null, idCliente, idsPedidosLib, idsContasR, idsChequesR);
        }

        /// <summary>
        /// Retorna o valor do débito de cheques do cliente com a empresa.
        /// </summary>
        public decimal GetDebitosCheque(GDASession sessao, uint idCliente, string idsPedidosLib, string idsContasR, string idsChequesR)
        {
            if (idCliente == 0)
                return 0;

            string sql = "select sum(if(instr(descrPlanoConta, 'Cheque Banco/Ag./Conta/Num. ')>0, valorVec, 0)) from (" +
                SqlDebitos(idCliente, 0, 0, idsPedidosLib, idsContasR, idsChequesR, null, TipoDebito.Todos, 0, null, null, true) + ") as temp";

            return ExecuteScalar<decimal>(sessao, sql);
        }

        public decimal ObterValorParaSaldoDevedor(GDASession sessao, uint idCliente)
        {
            var sql = @"
                SELECT SUM(ValorVec)
                FROM contas_receber
                WHERE COALESCE(IsParcelaCartao, 0) = 0
                    AND Recebida = 0
                    AND IdCliente = " + idCliente;

            return ExecuteScalar<decimal>(sessao, sql);
        }

        #endregion

        #region Parcelas do cartão

        private string SqlParcCartao(uint idPedido, uint idLiberarPedido, uint idAcerto, uint idLoja, uint idCli, uint tipoEntrega,
            string nomeCli, string dtIni, string dtFim, uint idAcertoCheque, bool returnAll, bool vinculadas, uint tipoCartao, bool agrupar,
            bool selecionar, bool recebidas, string dtCadIni, string dtCadFim,string nCNI,
            decimal valorIni, decimal valorFim, TipoCartaoEnum tipoRecbCartao, string numAutCartao, string numEstabCartao, string ultDigCartao,
            out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = agrupar;
            filtroAdicional = " And c.isParcelaCartao=true";
            filtroAdicional += recebidas ? " and c.recebida=true" : " and coalesce(c.recebida, false)=false";

            string criterio = "";
            string campos = selecionar || agrupar ?
                @"c.*, cli.Nome as NomeCli, pl.Descricao as DescrPlanoConta,
                CONCAT(jpc.Juros,'%') AS TaxaJuros, CONCAT(oc.DESCRICAO, ' ', bc.DESCRICAO, ' ', (CASE tcc.TIPO WHEN 1 THEN 'Débito' ELSE 'Crédito' END)   ) AS DescricaoCartao,
                concat(coalesce(cb.nome, ''), ' (Ag. ', coalesce(cb.agencia, ''), ' Conta ', coalesce(cb.conta, ''), ')') as contaBanco, tcc.IdTipoCartao AS tipoCartao, '$$$' as criterio" : "Count(*)";

            string where = String.Empty;

            if (!agrupar)
            {
                if (idPedido > 0)
                {
                    uint? idSinal = PedidoDAO.Instance.ObtemIdSinalOuPagtoAntecipado(null, idPedido);
                    string filtro = "";

                    if (PedidoConfig.LiberarPedido)
                    {
                        IList<string> lstIdAcerto = ExecuteMultipleScalar<string>(@"Select idAcerto From contas_receber Where idAcerto Is Not Null And
                            idLiberarPedido In (Select idLiberarPedido From produtos_liberar_pedido Where idPedido=" + idPedido + ")");

                        // Os parênteses abaixo devem ficar separados ") )" para que caso passe no TrimEnd(')') logo abaixo remova somente um deles
                        if (lstIdAcerto.Count > 0)
                            filtro = " And (c.idLiberarPedido In (Select idLiberarPedido From pedido Where idPedido=" + idPedido + ") Or c.idAcerto In (" + String.Join(",", lstIdAcerto.ToArray()) + ") )";
                    }
                    else
                    {
                        IList<string> lstIdAcerto = ExecuteMultipleScalar<string>(@"Select idAcerto From contas_receber Where idAcerto Is Not Null And
                            idPedido=" + idPedido);

                        // Os parênteses abaixo devem ficar separados ") )" para que caso passe no TrimEnd(')') logo abaixo remova somente um deles
                        if (lstIdAcerto.Count > 0)
                            filtro = " And (c.idPedido=" + idPedido + " Or c.idAcerto In (" + String.Join(",", lstIdAcerto.ToArray()) + ") )";
                    }

                    if (idSinal > 0)
                        filtro = (String.IsNullOrEmpty(filtro) ? " And (" : filtro.TrimEnd(')') + " Or ") + "c.idSinal=" + idSinal + ")";

                    filtroAdicional += String.IsNullOrEmpty(filtro) ?
                        (PedidoConfig.LiberarPedido ? " And c.idLiberarPedido In (Select idLiberarPedido From pedido Where idPedido=" + idPedido + ")" :
                        " And c.idPedido=" + idPedido) : filtro.TrimEnd(')') + " Or c.idPedido=" + idPedido + ")"; ;

                    criterio += "Pedido: " + idPedido + "    ";
                    temFiltro = true;
                }
                else if (idAcerto > 0)
                {
                    where += " And c.idAcerto=" + idAcerto;
                    criterio += "Acerto: " + idAcerto + "    ";
                    temFiltro = true;
                }
                else if (idLiberarPedido > 0)
                {
                    where += " And c.idLiberarPedido=" + idLiberarPedido;
                    criterio += "Liberação: " + idLiberarPedido + "    ";
                    temFiltro = true;
                }
                else if (vinculadas)
                {
                    where += " And (cli.id_Cli In (Select idClienteVinculo From cliente_vinculo Where idCliente=" + idCli + ") Or cli.id_Cli=" + idCli + ")";
                    criterio += "Incluir clientes vinculados    ";
                    temFiltro = true;
                }
                else if (idCli > 0)
                {
                    where += " And cli.Id_Cli=" + idCli;
                    criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCli) + "    ";
                    temFiltro = true;
                }
                else if (!String.IsNullOrEmpty(nomeCli))
                {
                    string ids = ClienteDAO.Instance.GetIds(null, nomeCli, null, 0, null, null, null, null, 0);
                    where += " And cli.id_Cli in (" + ids + ")";
                    criterio += "Cliente: " + nomeCli;
                    temFiltro = true;
                }

                else if (idAcertoCheque > 0)
                {
                    where += " And c.idAcertoCheque=" + idAcertoCheque;
                    criterio += "Acerto Cheque: " + idAcertoCheque + "    ";
                    temFiltro = true;
                }
                else if (!string.IsNullOrEmpty(nCNI))
                {
                    where += " And c.IdCartaoNaoIdentificado=" + nCNI;
                    criterio += "Nº CNI: " + nCNI + "    ";
                    temFiltro = true;
                }

                if (tipoEntrega > 0)
                {
                    where += PedidoConfig.LiberarPedido ?
                        " And c.idLiberarPedido In (Select idLiberarPedido From pedido Where tipoEntrega=" + tipoEntrega + ")" :
                        " And p.TipoEntrega=" + tipoEntrega;
                    criterio += "Tipo Entrega: " + Utils.GetDescrTipoEntrega((int)tipoEntrega) + "    ";
                    temFiltro = true;
                }

                if (valorIni > 0)
                {
                    filtroAdicional += " And c.valorVec>=" + valorIni.ToString().Replace(',', '.');
                    criterio += valorFim > 0 ? "Valor: " + valorIni + " até " + valorFim + "    " : "Valor a partir de " + valorIni + "    ";
                    temFiltro = true;
                }

                if (valorFim > 0)
                {
                    filtroAdicional += " And c.valorVec<=" + valorFim.ToString().Replace(',', '.');
                    criterio += valorIni > 0 ? "" : "Valor até " + valorFim + "    ";
                    temFiltro = true;
                }

                if (tipoRecbCartao > 0)
                {
                    where += " and c.idConta in (" + UtilsPlanoConta.ContasTipoCartao(tipoRecbCartao) + ")";
                    criterio += "Tipo de Recebimento de Cartão: " + Colosoft.Translator.Translate(tipoRecbCartao).Format();
                    temFiltro = true;
                }

                if (!string.IsNullOrEmpty(numAutCartao))
                {
                    where += @" AND (cni.NumAutCartao=?numAutCartao OR
                    EXISTS (SELECT NumAutCartao FROM pagto_sinal ps WHERE c.IdSinal=ps.IdSinal AND NumAutCartao=?numAutCartao
                        UNION SELECT NumAutCartao FROM pagto_acerto pa WHERE c.IdAcerto=pa.IdAcerto AND NumAutCartao=?numAutCartao
                        UNION SELECT NumAutCartao FROM pagto_acerto_cheque pac WHERE c.IdAcertoCheque=pac.IdAcertoCheque AND NumAutCartao=?numAutCartao
                        UNION SELECT NumAutCartao FROM pagto_contas_receber pcr WHERE c.IdContarCartao=pcr.IdContaR AND NumAutCartao=?numAutCartao
                        UNION SELECT NumAutCartao FROM pagto_troca_dev ptd WHERE c.IdTrocaDevolucao=ptd.IdTrocaDevolucao AND NumAutCartao=?numAutCartao
                        UNION SELECT NumAutCartao FROM pagto_liberar_pedido plp WHERE c.IdLiberarPedido=plp.IdLiberarPedido AND NumAutCartao=?numAutCartao
                        UNION SELECT NumAut FROM pagto_nota_fiscal pnf WHERE c.IdNf=pnf.IdNf AND NumAut=?numAutCartao
                        UNION SELECT NumAutCartao FROM pagto_obra po WHERE c.IdObra=po.IdObra AND NumAutCartao=?numAutCartao))";
                    criterio += "Num. de Autorização: " + numAutCartao + "    ";
                    temFiltro = true;
                }

                if (!string.IsNullOrEmpty(numEstabCartao))
                {
                    where += " AND cni.NumeroEstabelecimento=?numEstabCartao";
                    criterio += "Num. do Estabelecimento: " + numEstabCartao + "    ";
                    temFiltro = true;
                }

                if (!string.IsNullOrEmpty(ultDigCartao))
                {
                    where += " AND cni.UltimosDigitosCartao=?ultDigCartao";
                    criterio += "Ultimos digitos do cartão: " + ultDigCartao + "    ";
                    temFiltro = true;
                }
            }

            if (tipoCartao > 0)
            {
                where += " and c.idConta in (" + UtilsPlanoConta.ContasTipoCartao(tipoCartao) + ")";
                criterio += "Tipo Cartão: " + TipoCartaoCreditoDAO.Instance.ObterDescricao(null, (int)tipoCartao) + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dtIni))
            {
                where += " And DATAVEC>=?dtIni";
                criterio += "Data início: " + dtIni + "    ";
                temFiltro = true;
            }

            if (!agrupar && !String.IsNullOrEmpty(dtFim))
            {
                where += " And DATAVEC<=?dtFim";
                criterio += "Data término: " + dtFim + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dtCadIni))
            {
                where += " And c.DATACAD>=?dataCadIni";
                criterio += "Data Cad. início: " + dtCadIni + "    ";
                temFiltro = true;
            }

            if (!agrupar && !String.IsNullOrEmpty(dtCadFim))
            {
                where += " And c.DATACAD<=?dataCadFim";
                criterio += "Data Cad. término: " + dtCadFim + "    ";
                temFiltro = true;
            }

            if (idLoja > 0)
            {
                filtroAdicional += " And c.IdLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            // Se não for para retornar todos e nenhum filtro tiver sido especificado, não retorna nenhum registro
            if (!returnAll && where == String.Empty)
            {
                where = " And 0>1";
                filtroAdicional = " And 0>1";
            }

            string sql = "Select " + campos + @" From contas_receber c
                Left Join conta_banco cb On (c.idContaBanco=cb.idContaBanco)
                Left Join plano_contas pl On (c.IdConta=pl.IdConta)
                Left Join cliente cli On (c.idCliente=cli.id_Cli)
                Left Join pedido p On (c.IdPedido=p.idPedido)
                LEFT JOIN cartao_nao_identificado cni ON (c.IdCartaoNaoIdentificado=cni.IdCartaoNaoIdentificado)
                LEFT JOIN tipo_cartao_credito tcc on (c.IDCONTA=tcc.IDCONTAVISTA OR c.IDCONTA=tcc.IDCONTARECPRAZO  OR c.IDCONTA=tcc.IDCONTAENTRADA )
                LEFT JOIN juros_parcela_cartao jpc on (jpc.IDJUROSPARCELA=(CASE
																		        (SELECT COUNT(*) FROM juros_parcela_cartao WHERE IDTIPOCARTAO = tcc.IDTIPOCARTAO AND NUMPARC = c.NUMPARCMAX AND IDLOJA =c.IdLoja)
																		        WHEN 0
																		        THEN (SELECT MIN(IDJUROSPARCELA) AS IDJUROSPARCELA FROM juros_parcela_cartao WHERE IDTIPOCARTAO = tcc.IdTipoCartao AND NUMPARC = c.NUMPARCMAX)
																		        ELSE (SELECT IDJUROSPARCELA FROM juros_parcela_cartao WHERE IDTIPOCARTAO = tcc.IDTIPOCARTAO  AND NUMPARC = c.NUMPARCMAX AND IDLOJA = c.IdLoja)
																	        END))

                LEFT JOIN operadora_cartao oc on (tcc.OPERADORA=oc.IDOPERADORACARTAO)
                LEFT JOIN bandeira_cartao bc on (tcc.Bandeira=bc.IDBANDEIRACARTAO)
                Where 1 ?filtroAdicional? " + where;

            if (agrupar)
            {
                var camposAgrupar = @"c.*, cast(sum(c.valorVec) as decimal(12,2)) as valorAgrupado,
                    cast(group_concat(c.idContaR) as char) as idsContas, coalesce(l.nomeFantasia, l.razaoSocial) as nomeLoja";

                sql = "SELECT " + camposAgrupar +
                    " FROM (" + sql + @") as c
                        LEFT JOIN loja l on (c.idLoja = l.idLoja) " +
                    "GROUP BY " + (PedidoConfig.LiberarPedido ? "" : "c.idLoja,") + "date(c.dataVec), c.tipoCartao, c.IdConta";
            }

            if (!agrupar)
                sql = sql + " Group By c.IdContaR";

            if (!selecionar)
                sql = "select count(*) from (" + sql + ") as temp";

            return sql.Replace("$$$", criterio);
        }

        /// <summary>
        /// Busca as parcelas de cartão a receber que ainda não foram recebidas
        /// </summary>
        /// <returns></returns>
        public IList<ContasReceber> GetParcCartao(uint idPedido, uint idLiberarPedido, uint idAcerto, uint idLoja, uint idCli,
            uint tipoEntrega, string nomeCli, string dtIni, string dtFim,
            uint tipoCartao, uint idAcertoCheque, bool agrupar, bool recebidas, string dtCadIni, string dtCadFim, string nCNI,
            decimal valorIni, decimal valorFim, TipoCartaoEnum tipoRecbCartao, string numAutCartao, string numEstabCartao, string ultDigCartao,
            string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? "c.DataVec Asc" : sortExpression;

            bool temFiltro;
            string filtroAdicional;

            string sql = SqlParcCartao(idPedido, idLiberarPedido, idAcerto, idLoja, idCli, tipoEntrega, nomeCli, dtIni, dtFim,
                idAcertoCheque, true, false, tipoCartao, agrupar, true, recebidas, dtCadIni, dtCadFim, nCNI, valorIni, valorFim,
                tipoRecbCartao, numAutCartao, numEstabCartao, ultDigCartao,
                out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadDataWithSortExpression(sql, new InfoSortExpression(sortExpression),
                new InfoPaging(startRow, pageSize), GetParam(nomeCli, dtIni, dtFim, null, null, null, null, dtCadIni, dtCadFim, null, null,
                numAutCartao, numEstabCartao, ultDigCartao)).ToList();
        }

        public int GetParcCartaoCount(uint idPedido, uint idLiberarPedido, uint idAcerto, uint idLoja, uint idCli, uint tipoEntrega, string nomeCli, string dtIni,
            string dtFim, uint tipoCartao, uint idAcertoCheque, bool agrupar, bool recebidas, string dtCadIni, string dtCadFim, string nCNI,
            decimal valorIni, decimal valorFim, TipoCartaoEnum tipoRecbCartao, string numAutCartao, string numEstabCartao, string ultDigCartao)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlParcCartao(idPedido, idLiberarPedido, idAcerto, idLoja, idCli, tipoEntrega, nomeCli, dtIni, dtFim, idAcertoCheque, true, false,
                tipoCartao, agrupar, false, recebidas, dtCadIni, dtCadFim, nCNI, valorIni, valorFim, tipoRecbCartao, numAutCartao, numEstabCartao, ultDigCartao,
                out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.ExecuteSqlQueryCount(sql, GetParam(nomeCli, dtIni, dtFim, null, null, null, null, dtCadIni, dtCadFim, null, null,
                numAutCartao, numEstabCartao, ultDigCartao));
        }

        /// <summary>
        /// Busca as parcelas de cartão a receber que ainda não foram recebidas
        /// </summary>
        /// <returns></returns>
        public IList<ContasReceber> GetParcCartaoRpt(uint idPedido, uint idLiberarPedido, uint idAcerto, uint idLoja, uint idCli, uint tipoEntrega, string nomeCli, string dtIni,
            string dtFim, uint tipoCartao, uint idAcertoCheque, bool agrupar, bool recebidas, string dtCadIni, string dtCadFim, string nCNI,
            decimal valorIni, decimal valorFim, TipoCartaoEnum tipoRecbCartao, string numAutCartao, string numEstabCartao, string ultDigCartao)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = SqlParcCartao(idPedido, idLiberarPedido, idAcerto, idLoja, idCli, tipoEntrega, nomeCli, dtIni, dtFim,idAcertoCheque, true, false,
                tipoCartao, agrupar, true, recebidas, dtCadIni, dtCadFim, nCNI, valorIni, valorFim, tipoRecbCartao, numAutCartao, numEstabCartao, ultDigCartao,
                out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadData(sql + " order by c.DataVec Asc", GetParam(nomeCli, dtIni, dtFim, null, null, null, null, dtCadIni, dtCadFim, null, null,
                numAutCartao, numEstabCartao, ultDigCartao)).ToList();
        }

        public void QuitarVariasParcCartao(string idContasR, uint idContaBanco, string data, bool isCaixaDiario)
        {
            foreach (string id in idContasR.Split(','))
                QuitarParcCartao(Glass.Conversoes.StrParaUint(id), idContaBanco, data, isCaixaDiario);
        }

        /// <summary>
        /// Quita as parcelas de cartão em aberto com base no arquivo importado, e gera apenas uma movimentação bancária
        /// </summary>
        /// <param name="idArquivoQuitacaoParcelaCartao"></param>
        /// <param name="contasQuitar"></param>
        /// <param name="data"></param>
        /// <param name="isCaixaDiario"></param>
        public void QuitarVariasParcCartao(int idArquivoQuitacaoParcelaCartao, List<KeyValuePair<uint, uint>> contasQuitar, string data, bool isCaixaDiario)
        {
            lock (_quitarParcCartaoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        // Se não houver contas a receber
                        if (contasQuitar == null || contasQuitar.Count == 0)
                            throw new Exception("Não há nenhuma conta a receber para ser quitada, verifique se já foram quitadas.");

                        var contasReceber = GetByPks(
                            transaction,
                            string.Join(", ", contasQuitar.Select(c => c.Value)));

                        var falhas = string.Join(
                            Environment.NewLine,
                            contasReceber.Where(c => c.Recebida).Select(c => $"Cód.:{c.IdContaR} - Ref.:{c.Referencia}"));

                        if (!string.IsNullOrWhiteSpace(falhas))
                        {
                            throw new Exception($"Contas já recebidas:{Environment.NewLine}{falhas}");
                        }

                        decimal valorTotalJurosCartao = 0;
                        decimal valorTotalMov = 0;
                        var tipoMov = 0;
                        var tipoMovJuros = 0;
                        // Define a data da movimentação
                        var dataMov = data != DateTime.Now.ToString("dd/MM/yyyy") ? DateTime.Parse(data + " 23:00") : DateTime.Now;

                        // Recupera a lista de IdsContaBanco Distintos.
                        var idsContaBanco = (contasQuitar.Select(b => b.Key)).Distinct();

                        // Gera uma movimentação por conta bancária distinta
                        foreach (var idContaBanco in idsContaBanco)
                        {
                            // Zera os valores das movimentações bancárias para realizar outra.
                            valorTotalJurosCartao = 0;
                            valorTotalMov = 0;

                            // Gera uma movimentação por Conta Receber onde o IdContaBanco for igual ao IdContaBanco da Iteração atual.
                            foreach (var idContaR in contasQuitar.Where(b => b.Key == idContaBanco).Select(r => r.Value))
                            {
                                // Recupera os dados da conta a receber (parcela do cartão)
                                var conta = contasReceber
                                    .Where(c => c.IdContaR == idContaR)
                                    .First();

                                // Necessário para recuperar as contas ao cancelar arquivo.
                                conta.IdArquivoQuitacaoParcelaCartao = idArquivoQuitacaoParcelaCartao;

                                // Se for devolução de pagamento, o tipo de movimentação deve ser saída, pois está sendo devolvido um valor para o cliente,
                                // caso contrário será entrada.
                                tipoMov = conta.IdDevolucaoPagto > 0 ? 2 : 1;
                                tipoMovJuros = tipoMov == 1 ? 2 : 1;

                                /* Chamado 25083. */
                                if (!FinanceiroConfig.Cartao.CartaoMovimentaCxGeralDiario)
                                {
                                    MovimentaCaixaGeralDiario(transaction, isCaixaDiario, conta, dataMov, tipoMov);
                                }

                                if (FinanceiroConfig.FinanceiroRec.SelecionarContaBancoQuitarParcCartao)
                                    conta.IdContaBanco = idContaBanco;

                                // Gera a movimentação bancária
                                if (conta.IdContaBanco > 0)
                                {
                                    // Calcula o valor dos juros
                                    valorTotalJurosCartao += conta.ValorJurosCartao;
                                    valorTotalMov += conta.ValorVec + (!FinanceiroConfig.Cartao.CobrarJurosCartaoCliente ? conta.ValorJurosCartao : 0);
                                }

                                MarcaContaComoRecebida(transaction, conta, dataMov);
                            }

                            // Gera uma unica movimentação bancaria para todas as parcelas recebidas.
                            GerarMovimentacaoBancariaArquivoQuitacaoParcelaCartao(transaction, idArquivoQuitacaoParcelaCartao, idContaBanco,
                                FinanceiroConfig.PlanoContaQuitacaoParcelaCartao, dataMov, tipoMov, valorTotalMov);
                            //Gera a movimentação de juros
                            if (valorTotalJurosCartao > 0)
                            {
                                GerarMovimentacaoBancariaArquivoQuitacaoParcelaCartao(transaction, idArquivoQuitacaoParcelaCartao, idContaBanco,
                                    FinanceiroConfig.PlanoContaJurosCartao, dataMov, tipoMovJuros, valorTotalJurosCartao);
                            }
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
        }

        public void QuitarParcCartao(uint idContaR, uint idContaBanco, string data, bool isCaixaDiario)
        {
            lock (_quitarParcCartaoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        // Recupera os dados da conta a receber (parcela do cartão)
                        var conta = GetElementByPrimaryKey(transaction, idContaR);

                        if (conta == null)
                            throw new Exception("Não há conta a receber com o id informado, verifique se o mesma já foi quitada.");

                        if (conta.Recebida)
                            throw new Exception("Esta parcela já foi quitada. Ref.:" + conta.Referencia);

                        // Define a data da movimentação
                        var dataMov = data != DateTime.Now.ToString("dd/MM/yyyy") ? DateTime.Parse(data + " 23:00") : DateTime.Now;

                        // Se for devolução de pagamento, o tipo de movimentação deve ser saída, pois está sendo devolvido um valor para o cliente,
                        // caso contrário será entrada.
                        var tipoMov = conta.IdDevolucaoPagto > 0 ? 2 : 1;
                        var tipoMovJuros = tipoMov == 1 ? 2 : 1;

                        /* Chamado 25083. */
                        if (!FinanceiroConfig.Cartao.CartaoMovimentaCxGeralDiario)
                        {
                            MovimentaCaixaGeralDiario(transaction, isCaixaDiario, conta, dataMov, tipoMov);
                        }

                        if (FinanceiroConfig.FinanceiroRec.SelecionarContaBancoQuitarParcCartao)
                            conta.IdContaBanco = idContaBanco;

                        // Gera a movimentação bancária
                        if (conta.IdContaBanco > 0)
                        {
                            // Calcula o valor dos juros
                            var valorJurosCartao = conta.ValorJurosCartao;
                            var valor = conta.ValorVec + (!FinanceiroConfig.Cartao.CobrarJurosCartaoCliente ? valorJurosCartao : 0);

                            // Gera a movimentação bancária
                            GerarMovimentacaoBancariaContaReceber(transaction, idContaR, conta, conta.IdConta.Value, dataMov, tipoMov, valor, conta.Obs);

                            // Gera a movimentação de juros
                            if (valorJurosCartao > 0)
                            {
                                GerarMovimentacaoBancariaContaReceber(transaction, idContaR, conta, FinanceiroConfig.PlanoContaJurosCartao, dataMov, tipoMovJuros, valorJurosCartao, conta.Obs);
                            }
                        }

                        MarcaContaComoRecebida(transaction, conta, dataMov);

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
        }

        /// <summary>
        /// Gera a movimentação bancaria pelo Arquivo Quitação Parcela Cartão
        /// </summary>
        private void GerarMovimentacaoBancariaArquivoQuitacaoParcelaCartao(GDASession sessao, int idArquivoQuitacaoParcelaCartao, uint idContaBanco,
            uint planoConta, DateTime dataMov, int tipoMov, decimal valor)
        {
            // Gera a movimentação bancária
            ContaBancoDAO.Instance.MovContaContaR(sessao, idContaBanco, planoConta,
                (int)UserInfo.GetUserInfo.IdLoja, idArquivoQuitacaoParcelaCartao, tipoMov, valor, dataMov, null);
        }

        /// <summary>
        /// Gera a movimentação bancaria pela conta a receber.
        /// </summary>
        private void GerarMovimentacaoBancariaContaReceber(GDASession sessao, uint idContaR, ContasReceber conta, uint planoConta,
            DateTime dataMov, int tipoMov, decimal valor, string obs)
        {
            /* Chamado 64406. */
            // Gera a movimentação bancária
            ContaBancoDAO.Instance.MovContaContaR(sessao, conta.IdContaBanco.Value, planoConta, (int)UserInfo.GetUserInfo.IdLoja, null, null, idContaR, null,
                conta.IdCliente, tipoMov, valor, dataMov, (uint?)conta.IdCartaoNaoIdentificado, obs);
        }

        private void MovimentaCaixaGeralDiario(GDASession sessao, bool isCaixaDiario, ContasReceber conta, DateTime dataMov, int tipoMov)
        {
            /* Chamado 17940. */
            // Gera a movimentação no Cx Geral
            if (!isCaixaDiario)
            {
                var idCxGeral = CaixaGeralDAO.Instance.MovCxContaRec(sessao, conta.IdPedido, conta.IdLiberarPedido,
                    conta.IdContaR, conta.IdCliente, conta.IdConta.Value,
                    tipoMov, conta.ValorVec, 0, null, 0, false,
                    conta.IdContaBanco > 0 ? (DateTime?)dataMov : null, null);

                if (conta.IdCartaoNaoIdentificado > 0)
                    CaixaGeralDAO.Instance.AssociarCaixaGeralIdCartaoNaoIdentificado(sessao, idCxGeral, (uint)conta.IdCartaoNaoIdentificado);
            }
            else
            {
                var idCxDiario = CaixaDiarioDAO.Instance.MovCxContaRec(sessao, UserInfo.GetUserInfo.IdLoja, conta.IdCliente,
                    conta.IdPedido, conta.IdLiberarPedido, conta.IdContaR,
                    tipoMov, conta.ValorVec, 0, conta.IdConta.Value, null, 0, null, true);

                if (conta.IdCartaoNaoIdentificado > 0)
                    CaixaDiarioDAO.Instance.AssociarCaixaDiarioIdCartaoNaoIdentificado(sessao, idCxDiario, (uint)conta.IdCartaoNaoIdentificado);
            }
        }

        private void MarcaContaComoRecebida(GDASession sessao, ContasReceber conta, DateTime dataMov)
        {
            // Indica que a conta foi recebida
            conta.Recebida = true;
            conta.DataRec = dataMov;
            Update(sessao, conta);
        }

        public void CancelarRecebimentoParcCartao(uint idContaR, int idArquivoQuitacaoParcelaCartao, bool estornarMovimentacaoBancaria, DateTime? dataEstornoBanco, string obs)
        {
            lock (_cancelarRecebimentorParcCartaoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        // Recupera os dados da conta a receber (parcela do cartão)
                        ContasReceber conta = GetElementByPrimaryKey(transaction, idContaR);

                        /* Chamado 64406. */
                        if (!conta.Recebida)
                            throw new Exception("O recebimento desta parcela de cartão já foi cancelado.");

                        string motivo = "Cancelamento de recebimento de parcela de cartão";
                        LogCancelamentoDAO.Instance.LogContaReceber(transaction, conta, motivo, true);

                        // Indica que a conta foi cancelada
                        objPersistence.ExecuteCommand(transaction, "Update contas_receber Set recebida=false Where idContaR=" + idContaR);

                        /* Chamado 25083. */
                        if (!FinanceiroConfig.Cartao.CartaoMovimentaCxGeralDiario)
                        {
                            /* Chamado 17940. */
                            // Gera a movimentação no Cx Geral
                            CaixaGeralDAO.Instance.MovCxContaRec(transaction, conta.IdPedido, conta.IdLiberarPedido, conta.IdContaR,
                                conta.IdCliente, UtilsPlanoConta.EstornoAPrazo(conta.IdConta.Value),
                                2, conta.ValorVec, 0, null, 0, false, null, null);
                        }

                        if (conta.IdContaBanco > 0)
                        {
                            // Recupera as movimentações bancárias.
                            var movimentacoesBancarias = MovBancoDAO.Instance.GetByContaRec(transaction, idContaR, idArquivoQuitacaoParcelaCartao);
                            var idsContaR = new List<uint>(Array.ConvertAll(movimentacoesBancarias, x => x.IdMovBanco));

                            if (idsContaR.Count > 0)
                            {
                                idsContaR.Sort();

                                /* Chamado 46840. */
                                if (estornarMovimentacaoBancaria && dataEstornoBanco.HasValue)
                                {
                                    // Verifica a conciliação bancária.
                                    ConciliacaoBancariaDAO.Instance.VerificaDataConciliacao(transaction, conta.IdContaBanco.Value, dataEstornoBanco.Value);

                                    // Percorre as movimentações bancárias da parcela de cartão para estorná-las.
                                    foreach (var m in movimentacoesBancarias)
                                    {
                                        // Se a movimentação for de saída, quer dizer que movimentações anteriores à essas
                                        // já foram estornadas anteriormente, uma vez que a lista está ordenada em ordem decrescente,
                                        // por isso, para o loop neste momento
                                        if (m.TipoMov == 2)
                                        {
                                            // Se for juros de venda cartão continua
                                            if (m.IdConta == FinanceiroConfig.PlanoContaJurosCartao)
                                            {
                                                // O tipo da movimentação será sempre o inverso, na movimentação de estorno.
                                                if (m.IdArquivoQuitacaoParcelaCartao > 0)
                                                {
                                                    // Estorna a movimentação de juros de QuitacaoParcelaCartao
                                                    ContaBancoDAO.Instance.MovContaContaR(transaction, conta.IdContaBanco.Value, FinanceiroConfig.PlanoContaEstornoJurosCartao, m.IdLoja,
                                                        conta.IdArquivoQuitacaoParcelaCartao, m.TipoMov == 1 ? 2 : 1, m.ValorMov, dataEstornoBanco.Value, obs);
                                                }
                                                else
                                                {
                                                    ContaBancoDAO.Instance.MovContaContaR(transaction, conta.IdContaBanco.Value, FinanceiroConfig.PlanoContaEstornoJurosCartao, m.IdLoja, null, null,
                                                        m.IdContaR, null, m.IdCliente.GetValueOrDefault(), m.TipoMov == 1 ? 2 : 1, m.ValorMov, 0, dataEstornoBanco.Value, obs);
                                                }
                                                continue;
                                            }
                                            else if (m.IdConta == UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.JurosVendaConstrucard))
                                            {
                                                // O tipo da movimentação será sempre o inverso, na movimentação de estorno.
                                                ContaBancoDAO.Instance.MovContaContaR(transaction, conta.IdContaBanco.Value,
                                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EstornoJurosVendaConstrucard), m.IdLoja, null, null, m.IdContaR, null,
                                                    m.IdCliente.GetValueOrDefault(), m.TipoMov == 1 ? 2 : 1, m.ValorMov, 0, dataEstornoBanco.Value, obs);

                                                continue;
                                            }
                                            else
                                                break;
                                        }

                                        uint idContaEstorno = 0;

                                        // Recupera o ID conta de estorno.
                                        try
                                        {
                                            // Chamado 50011
                                            if (m.IdSinal > 0)
                                                idContaEstorno = UtilsPlanoConta.EstornoSinalPedido(m.IdConta);
                                            // Define o Plano de conta de estorno para mov de QuitacaoParcelaCartao
                                            else if (m.IdArquivoQuitacaoParcelaCartao > 0)
                                                idContaEstorno = FinanceiroConfig.PlanoContaEstornoQuitacaoParcelaCartao;
                                            else
                                                idContaEstorno = UtilsPlanoConta.EstornoAPrazo(m.IdConta);
                                        }
                                        catch
                                        {
                                            throw new Exception(string.Format("Não foi possível recuperar a conta de estorno da conta: {0}. " +
                                                "Caso a conta esteja inativa, ative-a e tente novamente. Sendo uma conta referente à juros, ela deve ser associada " +
                                                "através menu Configurações para que a conta de estorno possa ser recuperada e o cancelamento possa ser feito.",
                                                PlanoContasDAO.Instance.GetDescricao(transaction, m.IdConta, true)));
                                        }

                                        // O tipo da movimentação será sempre o inverso, na movimentação de estorno.
                                        if (m.IdArquivoQuitacaoParcelaCartao > 0)
                                        {
                                            // Estorna a movimentação de QuitacaoParcelaCartao
                                            ContaBancoDAO.Instance.MovContaContaR(transaction, conta.IdContaBanco.Value, idContaEstorno, m.IdLoja,
                                                conta.IdArquivoQuitacaoParcelaCartao, m.TipoMov == 1 ? 2 : 1, m.ValorMov, dataEstornoBanco.Value, obs);
                                        }
                                        else
                                        {
                                            ContaBancoDAO.Instance.MovContaContaR(transaction, conta.IdContaBanco.Value, idContaEstorno, m.IdLoja, null, null, m.IdContaR, null,
                                                m.IdCliente.GetValueOrDefault(), m.TipoMov == 1 ? 2 : 1, m.ValorMov, 0, dataEstornoBanco.Value, obs);
                                        }
                                    }
                                }
                                else
                                {
                                    // Apaga as movimentações bancárias
                                    MovBanco movAnterior = MovBancoDAO.Instance.ObtemMovAnterior(transaction, idsContaR[0]);

                                    string ids = string.Join(",", Array.ConvertAll(idsContaR.ToArray(), x => x.ToString()));

                                    // Corrige saldo.
                                    objPersistence.ExecuteCommand(transaction, "UPDATE mov_banco SET ValorMov=0 WHERE IdMovBanco IN (" + ids + ")");

                                    if (movAnterior != null)
                                        MovBancoDAO.Instance.CorrigeSaldo(transaction, movAnterior.IdMovBanco, idsContaR[0]);

                                    MovBancoDAO.Instance.DeleteByPKs(transaction, ids, motivo);
                                }
                            }
                        }

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
        }

        #endregion

        #region Contas geradas/recebidas por período

        private string SqlGeradasReceb(uint idFunc, string dataIni, string dataFim, uint idLoja, bool geradas, out GDAParameter[] lstParam)
        {
            List<GDAParameter> param = new List<GDAParameter>();
            string campoRetorno = geradas ? "valorVec" : "valorRec";
            string campoData = geradas ? "dataCad" : "dataRec";

            string sql = @"
                select coalesce(sum(" + campoRetorno + @"),0)
                from contas_receber
                where Coalesce(isParcelaCartao, false)=false ";

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " and " + campoData + ">=?dataIni";
                param.Add(new GDAParameter("?dataIni", (dataIni.Length == 10 ? DateTime.Parse(dataIni = dataIni + " 00:00") : DateTime.Parse(dataIni))));
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " and " + campoData + "<=?dataFim";
                param.Add(new GDAParameter("?dataFim", (dataFim.Length == 10 ? DateTime.Parse(dataFim = dataFim + " 23:59:59") : DateTime.Parse(dataFim))));
            }
            if (idLoja > 0)
                sql += " AND idLoja=" + idLoja;

            lstParam = param.ToArray();
            return sql;
        }

        /// <summary>
        /// Retorna o valor total gerado para um período.
        /// </summary>
        /// <returns></returns>
        public decimal GetTotalGeradasPeriodo(uint idFunc, string dataIni, string dataFim, uint idLoja)
        {
            GDAParameter[] p;
            return ExecuteScalar<decimal>(SqlGeradasReceb(idFunc, dataIni, dataFim, idLoja, true, out p), p);
        }

        /// <summary>
        /// Retorna o valor total já recebido para um período.
        /// </summary>
        /// <returns></returns>
        public decimal GetTotalRecebidasPeriodo(uint idFunc, string dataIni, string dataFim, uint idLoja)
        {
            GDAParameter[] p;
            return ExecuteScalar<decimal>(SqlGeradasReceb(idFunc, dataIni, dataFim, idLoja, false, out p), p);
        }

        public decimal GetTotalRecebidasPeriodo(string dataIni, string dataFim, string tipoContaContabil, uint idLoja)
        {
            string sql = @"
                SELECT COALESCE(SUM(c.valorRec), 0)
                FROM contas_receber c
                WHERE recebida = true";

            var lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(tipoContaContabil))
            {
                string c;
                sql += FiltroTipoConta("c", tipoContaContabil, out c);
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " AND c.dataRec >=?dataIni";
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni)));
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                if (dataFim.Length <= 10)
                    dataFim += " 23:59:59";

                sql += " AND c.dataRec <=?dataFim";
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim)));
            }

            if (idLoja > 0)
                sql += " AND c.idLoja=" + idLoja;

            return ExecuteScalar<decimal>(sql, lstParam.ToArray());
        }

        #endregion

        #region Atualiza observação e data de vencimento da conta

        /// <summary>
        /// Atualiza a observação e a data de vencimento da conta a receber
        /// </summary>
        /// <param name="contaRec"></param>
        /// <returns></returns>
        public int AtualizaObsDataVec(ContasReceber contaRec)
        {
            var contaReceberAntiga = GetElementByPrimaryKey(contaRec.IdContaR);

            List<GDAParameter> lstParam = new List<GDAParameter>();
            lstParam.Add(new GDAParameter("?obs", contaRec.Obs));

            string sql = "Update contas_receber set obs=?obs";

            if (contaRec.DataVec.Year > 1)
            {
                sql += ", dataVec=?dataVec";
                lstParam.Add(new GDAParameter("?dataVec", contaRec.DataVec));
            }

            sql += " Where idContaR=" + contaRec.IdContaR;

            var temp = objPersistence.ExecuteCommand(sql, lstParam.ToArray());

            var contaReceberNova = GetElementByPrimaryKey(contaRec.IdContaR);
            LogAlteracaoDAO.Instance.LogContaReceber(contaReceberAntiga, contaReceberNova);

            return temp;
        }

        #endregion

        #region Busca contas a receber de uma liberação de pedido

        /// <summary>
        /// Retorna as contas a receber de uma liberação de pedido.
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <param name="removerNumParcIguais">Define que caso alguma conta a receber tenha o mesmo numParc, apenas a primeira seja adicionada, e que não busque parcelas de liberação ou entrada de crédito</param>
        /// <returns></returns>
        public IList<ContasReceber> GetByLiberacaoPedido(uint idLiberarPedido, bool removerNumParcIguaisECredito)
        {
            return GetByLiberacaoPedido(null, idLiberarPedido, removerNumParcIguaisECredito);
        }

        /// <summary>
        /// Retorna as contas a receber de uma liberação de pedido.
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <param name="removerNumParcIguais">Define que caso alguma conta a receber tenha o mesmo numParc, apenas a primeira seja adicionada, e que não busque parcelas de liberação ou entrada de crédito</param>
        /// <returns></returns>
        public IList<ContasReceber> GetByLiberacaoPedido(GDASession sessao, uint idLiberarPedido, bool removerNumParcIguaisECredito)
        {
            string sql = @"
                Select c.*, cli.Nome as NomeCli, pl.Descricao as DescrPlanoConta From contas_receber c
                    Left Join cliente cli On (c.IdCliente=cli.id_Cli)
                    Left Join plano_contas pl On (c.IdConta=pl.IdConta)
                Where (c.isParcelaCartao=false or c.isParcelaCartao is null)
                    And idAcertoParcial is null";

            // O filtro "And idAcertoParcial is null" foi colocado para que ao imprimir liberação que tenha parcela renegociada
            // não busque a parcela restante da renegociação como se fosse parcela da liberação

            if (idLiberarPedido > 0)
                sql += " And c.idLiberarPedido=" + idLiberarPedido;
            else
                sql += " And 0>1";

            if (removerNumParcIguaisECredito)
                sql += " And coalesce(c.idConta, 0) Not In (" + UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.EntradaCredito) + "," +
                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.VistaCredito) + "," +
                    UtilsPlanoConta.ContasSinalPedido() + ")";

            sql += " Order By DataVec Asc";

            if (!removerNumParcIguaisECredito)
                return objPersistence.LoadData(sql).ToList();

            var lstContasRecRetorno = new List<ContasReceber>();
            var numParcAdic = new List<int>();

            foreach (var parc in objPersistence.LoadData(sessao, sql).ToList())
            {
                // Caso a conta a receber tenha sido recebida parcialmente, não adiciona nas parcelas da liberação
                // (Ao receber parcial, a conta restante fica com o mesmo número de parcela da parcela original)
                if (numParcAdic.Contains(parc.NumParc))
                {
                    lstContasRecRetorno.FirstOrDefault(f => f.NumParc == parc.NumParc).ValorVec = parc.ValorVec;
                    continue;
                }

                lstContasRecRetorno.Add(parc);

                numParcAdic.Add(parc.NumParc);
            }

            return lstContasRecRetorno.ToArray();
        }

        #endregion

        #region Verifica se existe alguma conta recebida de uma liberação

        /// <summary>
        /// Verifica se existe alguma conta recebida de cartão associada à esta liberação
        /// </summary>
        /// <param name="idObra"></param>
        /// <returns></returns>
        public bool ExisteParcCartaoRecebidaLiberacao(uint idLiberarPedido)
        {
            string sql = "Select Count(*) From contas_receber Where recebida=true And isParcelaCartao=true And idLiberarPedido=" + idLiberarPedido;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        /// <summary>
        /// Verifica se existe alguma conta recebida de uma liberação que não seja referente à entrada
        /// </summary>
        public bool ContaRecebidaLiberacao(uint idLiberarPedido)
        {
            return ContaRecebidaLiberacao(null, idLiberarPedido);
        }

        /// <summary>
        /// Verifica se existe alguma conta recebida de uma liberação que não seja referente à entrada
        /// </summary>
        public bool ContaRecebidaLiberacao(GDASession session, uint idLiberarPedido)
        {
            string sql = "Select Count(*) From contas_receber Where recebida=true And Coalesce(idConta, 0) Not In (" +
                UtilsPlanoConta.ContasSinalPedido() + ") And idLiberarPedido=" + idLiberarPedido;

            if (objPersistence.ExecuteSqlQueryCount(session, sql, null) > 0)
                return true;

            // Verifica se as contas possuem acerto parcial em aberto
            foreach (var contas in GetByLiberacaoPedido(session, idLiberarPedido, false))
                if (contas.IdAcertoParcial > 0 &&
                    AcertoDAO.Instance.ObtemValorCampo<int>(session, "Situacao", "idAcerto=" + contas.IdAcertoParcial) == (int)Acerto.SituacaoEnum.Aberto)
                    return true;

            return false;
        }

        /// <summary>
        /// Verifica se existe alguma conta a receber de uma liberação.
        /// </summary>
        public bool ExisteReceberLiberacao(uint idLiberarPedido)
        {
            var sql =
                string.Format(@"SELECT COUNT(*) FROM contas_receber
                WHERE (Recebida IS NULL OR Recebida=0) AND IdLiberarPedido={0}", idLiberarPedido);

            return objPersistence.ExecuteSqlQueryCount(sql, null) > 0;
        }

        public bool ContaAntecipada(GDASession sessao, uint idContaR)
        {
            string sql = @"
                SELECT count(*)
                FROM contas_receber cr
                INNER JOIN antecip_conta_rec acr ON (cr.idAntecipContaRec = acr.idAntecipContaRec)
                WHERE cr.idAntecipContaRec is not null
                    AND acr.situacao=" + (int)AntecipContaRec.SituacaoEnum.Finalizada + @"
                    AND cr.idContaR=" + idContaR;

            return ExecuteScalar<int>(sessao, sql) > 0;
        }

        #endregion

        #region Verifica se há alguma conta recebida/a receber no pedido passado

        /// <summary>
        /// Verifica se há alguma conta recebida no pedido passado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public IDictionary<uint, bool> ExisteRecebida(string idsPedidos, bool paraComissao)
        {
            return ExisteRecebida(null, idsPedidos, paraComissao);
        }

        /// <summary>
        /// Verifica se há alguma conta recebida no pedido passado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public IDictionary<uint, bool> ExisteRecebida(GDASession session, string idsPedidos, bool paraComissao)
        {
            var sql = String.Format(@"
                SELECT CAST(CONCAT(" +
                    (Glass.Configuracoes.PedidoConfig.LiberarPedido ? "COALESCE(plp.idPedido, pnf.idpedido)" : "COALESCE(cr.idPedido, pnf.idpedido)") +
                    @", ',', COUNT(*)) AS CHAR)
                FROM contas_receber cr
                    " + (Glass.Configuracoes.PedidoConfig.LiberarPedido ?
                    "LEFT JOIN produtos_liberar_pedido plp ON (cr.IdLiberarPedido = plp.IdLiberarPedido)" : "") + @"
                    LEFT JOIN pedidos_nota_fiscal pnf ON (cr.IdNf = pnf.IdNf)
                WHERE cr.Recebida IS NOT NULL AND cr.Recebida = TRUE" +
                    (paraComissao ? "" : " AND cr.IdConta NOT IN (" + UtilsPlanoConta.ContasSinalPedido() + "," + UtilsPlanoConta.ContasAVista() + ")") +
                    (Glass.Configuracoes.PedidoConfig.LiberarPedido ?
                        " AND (plp.IdPedido IN (" + idsPedidos + ") {0}) GROUP BY COALESCE(plp.IdPedido, pnf.IdPedido)" :
                        " AND (cr.IdPedido IN (" + idsPedidos + ") {0}) GROUP BY COALESCE(cr.IdPedido, pnf.IdPedido)"),
                "OR pnf.IdPedido IN (" + idsPedidos + ")");

            /*AND (plp.IdPedido IN () OR pnf.IdPedido IN ())
                GROUP BY pnf.IdPedido;";*/

            var dicionario = new Dictionary<uint, bool>();
            foreach (var item in ExecuteMultipleScalar<string>(session, sql))
            {
                if (String.IsNullOrEmpty(item))
                    continue;

                int[] dados = Array.ConvertAll(item.Split(','), x => Glass.Conversoes.StrParaInt(x));
                dicionario.Add((uint)dados[0], dados[1] > 0);
            }

            return dicionario;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se há alguma conta recebida no pedido passado
        /// </summary>
        public bool ExisteRecebida(uint idPedido)
        {
            return ExisteRecebida(null, idPedido);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se há alguma conta recebida no pedido passado
        /// </summary>
        public bool ExisteRecebida(GDASession session, uint idPedido)
        {
            return ExisteRecebida(session, idPedido, false);
        }

        /// <summary>
        /// Verifica se há alguma conta recebida no pedido passado
        /// </summary>
        public bool ExisteRecebida(GDASession sessao, uint idPedido, bool recAVista)
        {
            var sql = @"
                Select Count(*) From contas_receber cr
                    " + (PedidoConfig.LiberarPedido ?
                    "Left Join produtos_liberar_pedido plp ON (cr.idLiberarPedido=plp.idLiberarPedido)"
                    : "") + @"
                    Left Join pedidos_nota_fiscal pnf ON (cr.idNf=pnf.idNf)
                Where Coalesce(cr.recebida, False)=True
                    And cr.idConta Not In (" + UtilsPlanoConta.ContasSinalPedido() + "," + UtilsPlanoConta.ContasAVista() + ")" +
                    (PedidoConfig.LiberarPedido ? " And (plp.idPedido=" + idPedido + " Or pnf.idPedido=" + idPedido + @")Group By cr.idContaR" :
                    " And (cr.idPedido=" + idPedido + " Or pnf.idPedido=" + idPedido + ")");

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Verifica se há alguma conta a receber no pedido passado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool ExisteEmAberto(uint idPedido)
        {
            var sql = @"
                Select Count(*) From contas_receber cr
                    " + (PedidoConfig.LiberarPedido ?
                    "Left Join produtos_liberar_pedido plp ON (cr.idLiberarPedido=plp.idLiberarPedido)"
                    : "") + @"
                Where Coalesce(cr.recebida, False)=False
                    And cr.idConta Not In (" + UtilsPlanoConta.ContasSinalPedido() + "," + UtilsPlanoConta.ContasAVista() + ")" +
                    (PedidoConfig.LiberarPedido ? " And plp.idPedido=" + idPedido + " Group By cr.idContaR" :
                    " And cr.idPedido=" + idPedido);

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Verifica se há alguma conta recebida na obra passada

        /// <summary>
        /// Verifica se há alguma parcela de cartão recebida na obra passada
        /// </summary>
        /// <param name="idObra"></param>
        /// <returns></returns>
        public bool ExisteParcCartaoRecebidaObra(uint idObra)
        {
            return ExisteParcCartaoRecebidaObra(null, idObra);
        }

        /// <summary>
        /// Verifica se há alguma parcela de cartão recebida na obra passada
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idObra"></param>
        /// <returns></returns>
        public bool ExisteParcCartaoRecebidaObra(GDASession sessao, uint idObra)
        {
            string sql = "Select Count(*) From contas_receber Where recebida=true And isParcelaCartao=true And idObra=" + idObra;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Verifica se há alguma conta recebida na obra passada
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool ExisteRecebidaObra(uint idObra)
        {
            return ExisteRecebidaObra(null, idObra);
        }

        /// <summary>
        /// Verifica se há alguma conta recebida na obra passada
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool ExisteRecebidaObra(GDASession sessao, uint idObra)
        {
            string sql = "Select Count(*) From contas_receber Where recebida=true And idObra=" + idObra;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Existe movimentação por pedido?

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Existe movimentação por pedido?
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool ExistsByPedido(uint idPedido)
        {
            return ExistsByPedido(null, idPedido);
        }

        /// <summary>
        /// Existe movimentação por pedido?
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool ExistsByPedido(GDASession sessao, uint idPedido)
        {
            return objPersistence.ExecuteSqlQueryCount(sessao, "select count(*) from contas_receber where idPedido=" + idPedido) > 0;
        }

        #endregion

        #region Recupera o desconto em parcelas

        /// <summary>
        /// Recupera o valor do desconto nas parcelas de um pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public decimal GetDescontoParcelas(uint idPedido)
        {
            return ExecuteScalar<decimal>("select sum(coalesce(desconto,0)) from contas_receber where idPedido=" + idPedido);
        }

        #endregion

        #region Recupera o valor total de vencimento das contas a receber passadas

        /// <summary>
        /// Recupera o valor total de vencimento das contas a receber passadas.
        /// </summary>
        /// <param name="idsContasR"></param>
        /// <returns></returns>
        public decimal GetTotalVenc(string idsContasR)
        {
            string sql = "select coalesce(sum(valorVec),0) from contas_receber where idContaR in (" + idsContasR + ")";
            return ExecuteScalar<decimal>(sql);
        }

        #endregion

        #region Recupera o texto usado no recibo de liberação

        /// <summary>
        /// Recupera o texto usado no recibo de liberação.
        /// </summary>
        /// <param name="idsContasR"></param>
        /// <returns></returns>
        public string GetForRecibo(string idsContasR)
        {
            string retorno = "";

            var parcelas = GetByPks(null, idsContasR);
            foreach (ContasReceber c in parcelas)
                retorno += ", " + c.ValorVec.ToString("C") + " com vencimento em " + c.DataVec.ToString("dd/MM/yyyy");

            return retorno.Length > 0 ? retorno.Substring(2) : "";
        }

        #endregion

        #region Exclui parcelas cartão

        public void DeleteParcCartaoByPedido(uint idPedido)
        {
            DeleteParcCartaoByPedido(null, idPedido);
        }

        public void DeleteParcCartaoByPedido(GDASession session, uint idPedido)
        {
            foreach (ContasReceber c in objPersistence.LoadData(session, "select * from contas_receber where isParcelaCartao=true and " +
                "idAcerto is null and idSinal is null and idPedido=" + idPedido).ToList())
            {
                LogCancelamentoDAO.Instance.LogContaReceber(session, c, "Cancelamento do Pedido " + idPedido, false);
                Delete(session, c);
            }
        }

        public void DeleteParcCartaoBySinal(GDASession sessao, uint idSinal)
        {
            foreach (ContasReceber c in objPersistence.LoadData(sessao, "Select * from contas_receber Where idSinal=" + idSinal +
                " And (recebida=true Or isParcelaCartao) And idConta In (" + UtilsPlanoConta.ContasSinalPedido() + "," +
                UtilsPlanoConta.ContasTipoPagto(Glass.Data.Model.Pagto.FormaPagto.Cartao) + ")").ToList())
            {
                LogCancelamentoDAO.Instance.LogContaReceber(sessao, c, "Cancelamento do Sinal " + idSinal, false);
                Delete(sessao, c);
            }
        }

        public void DeleteParcCartaoByLiberacao(GDASession sessao, uint idLiberarPedido)
        {
            foreach (ContasReceber c in objPersistence.LoadData(sessao, "select * from contas_receber where isParcelaCartao=true and " +
                "idLiberarPedido=" + idLiberarPedido).ToList())
            {
                LogCancelamentoDAO.Instance.LogContaReceber(sessao, c, "Cancelamento da Liberação de Pedido " + idLiberarPedido, false);
                Delete(sessao, c);
            }
        }

        public void DeleteParcCartaoByContaR(GDASession sessao, uint idContaR)
        {
            var conta = GetElementByPrimaryKey(sessao, idContaR);
            var idsContaRParcCartao = ObterIdsContaRParcCartaoPeloIdContaR(sessao, (int)idContaR);

            if (idsContaRParcCartao.Count == 0)
            {
                /* Chamado 57969. */
                if (conta.IdPedido == 0 && conta.IdLiberarPedido == 0 && conta.IdAcerto == 0 && conta.IdAcertoParcial == 0 && conta.IdTrocaDevolucao == 0 && conta.IdDevolucaoPagto == 0 && conta.IdObra == 0)
                    throw new Exception("Não foi possível cancelar as parcelas de cartão associadas à conta a receber. A referência não foi encontrada. Entre em contato com o suporte do software WebGlass.");

                string sql = " and idCliente=" + conta.IdCliente;
                sql += " and idFormaPagto" + (conta.IdFormaPagto != null ? "=" + conta.IdFormaPagto : " is null");
                sql += " and idPedido" + (conta.IdPedido != null ? "=" + conta.IdPedido : " is null");
                sql += " and idLiberarPedido" + (conta.IdLiberarPedido != null ? "=" + conta.IdLiberarPedido : " is null");
                sql += " and idAcerto" + (conta.IdAcerto != null ? "=" + conta.IdAcerto : " is null");
                sql += " and idAcertoParcial" + (conta.IdAcertoParcial != null ? "=" + conta.IdAcertoParcial : " is null");
                sql += " and idTrocaDevolucao" + (conta.IdTrocaDevolucao != null ? "=" + conta.IdTrocaDevolucao : " is null");
                sql += " and idDevolucaoPagto" + (conta.IdDevolucaoPagto != null ? "=" + conta.IdDevolucaoPagto : " is null");
                sql += " AND IdObra" + (conta.IdObra > 0 ? "=" + conta.IdObra : " IS NULL");

                foreach (ContasReceber c in objPersistence.LoadData(sessao, "select * from contas_receber where (isParcelaCartao=true OR IdContaRCartao > 0) " + sql).ToList())
                {
                    LogCancelamentoDAO.Instance.LogContaReceber(sessao, c, "Cancelamento da Conta a Receber " + idContaR, false);
                    Delete(sessao, c);
                }
            }
            /* Chamado 57969. */
            else
                foreach (var idContaRParcCartao in idsContaRParcCartao)
                    DeleteByPrimaryKey(sessao, (uint)idContaRParcCartao, false);
        }

        public void DeleteParcCartaoByAcerto(GDASession sessao, uint idAcerto)
        {
            foreach (ContasReceber c in objPersistence.LoadData(sessao, "select * from contas_receber where isParcelaCartao=true and idAcerto=" + idAcerto).ToList())
            {
                LogCancelamentoDAO.Instance.LogContaReceber(sessao, c, "Cancelamento do Acerto " + idAcerto, false);
                Delete(sessao, c);
            }
        }

        public void DeleteParcCartaoByAcertoCheque(GDASession session, uint idAcertoCheque)
        {
            foreach (ContasReceber c in objPersistence.LoadData(session, "select * from contas_receber where isParcelaCartao=true and idAcertoCheque=" + idAcertoCheque).ToList())
            {
                LogCancelamentoDAO.Instance.LogContaReceber(session, c, "Cancelamento do Acerto do Cheque " + idAcertoCheque, false);
                Delete(session, c);
            }
        }

        public void DeleteParcCartaoByObra(GDASession sessao, uint idObra)
        {
            foreach (ContasReceber c in objPersistence.LoadData(sessao, "select * from contas_receber where isParcelaCartao=true and idObra=" + idObra).ToList())
            {
                LogCancelamentoDAO.Instance.LogContaReceber(sessao, c, "Cancelamento da Obra " + idObra, false);
                Delete(sessao, c);
            }
        }

        public void DeleteParcCartaoByTrocaDevolucao(GDASession session, uint idTrocaDevolucao)
        {
            foreach (ContasReceber c in objPersistence.LoadData(session, "select * from contas_receber where isParcelaCartao=true and idTrocaDevolucao=" + idTrocaDevolucao).ToList())
            {
                LogCancelamentoDAO.Instance.LogContaReceber(session, c, "Cancelamento da Troca/Devolução " + idTrocaDevolucao, false);
                Delete(session, c);
            }
        }

        public void DeleteParcCartaoByDevolucaoPagto(GDASession session, uint idDevolucaoPagto)
        {
            foreach (ContasReceber c in objPersistence.LoadData(session, "select * from contas_receber where isParcelaCartao=true and idDevolucaoPagto=" + idDevolucaoPagto).ToList())
            {
                LogCancelamentoDAO.Instance.LogContaReceber(session, c, "Cancelamento da Devolução de Pagamento " + idDevolucaoPagto, false);
                Delete(session, c);
            }
        }

        public void DeleteParcCartaoByCartaoNaoIdentificado(GDASession session, int idCartaoNaoIdentificado)
        {
            foreach (var c in objPersistence.LoadData(session, "SELECT * FROM contas_receber WHERE IsParcelaCartao=1 AND IdCartaoNaoIdentificado=" + idCartaoNaoIdentificado).ToList())
            {
                LogCancelamentoDAO.Instance.LogContaReceber(session, c, "Cancelamento do Cartão Não Identificado " + idCartaoNaoIdentificado, false);
                Delete(session, c);
            }
        }

        #endregion

        #region Verifica se o registro existe

        /// <summary>
        /// Verifica se o registro existe.
        /// </summary>
        public new bool Exists(GDASession session, uint idContaR)
        {
            return objPersistence.ExecuteSqlQueryCount(session, "select count(*) from contas_receber where idContaR=" + idContaR) > 0;
        }

        public int ObtemIdArquivoRemessa(int idContaR)
        {
            var sql = @"SELECT r.IdArquivoRemessa
                FROM contas_receber cr
                    INNER JOIN arquivo_remessa r ON (cr.NumArquivoRemessaCnab = NumRemessa)
                WHERE cr.IdContaR = " + idContaR;

            return ExecuteScalar<int>(sql);
        }

        #endregion

        #region Arquivo Remessa

        /// <summary>
        /// Busca o Id da conta a receber pelo número do documento CNAB.
        /// </summary>
        public uint GetIdByNumeroDocumentoCnab(int codbanco, string numeroDocumento, string nossoNumero, string usoEmpresa, out string numDocCnab)
        {
            return GetIdByNumeroDocumentoCnab(null, codbanco, numeroDocumento, nossoNumero, usoEmpresa, out numDocCnab);
        }

        public List<ContasReceber> GetByIdCartaoNaoIdentificado(GDASession session, uint idCartaoNaoIdentificado)
        {
            return objPersistence.LoadData(session, "select * from contas_receber where IdCartaoNaoIdentificado="+ idCartaoNaoIdentificado);
        }

        /// <summary>
        /// Busca o Id da conta a receber pelo número do documento CNAB.
        /// </summary>
        public uint GetIdByNumeroDocumentoCnab(GDASession session, int codbanco, string numeroDocumento, string nossoNumero, string usoEmpresa, out string numDocCnab)
        {
            numeroDocumento = numeroDocumento.Trim();
            nossoNumero = nossoNumero.Trim();
            usoEmpresa = usoEmpresa.Trim();

            uint idContaR = 0;

            if (codbanco == (int)Sync.Utils.CodigoBanco.Sicredi)
            {
                idContaR = ObtemValorCampo<uint>(session, "idContaR", "numeroDocumentoCnab=?numDoc", new GDAParameter("?numDoc", nossoNumero));

                // Feito para resolver o chamado 14174
                if (idContaR == 0 && nossoNumero.Length > 2)
                {
                    var nossoNumeroCorrigido = "14" + nossoNumero.Substring(2);
                    idContaR = ObtemValorCampo<uint>(session, "idContaR", "numeroDocumentoCnab=?numDoc", new GDAParameter("?numDoc", nossoNumeroCorrigido));
                }

                // Feito para resolver o chamado 14262
                if (idContaR == 0 && nossoNumero.Length > 2)
                {
                    var nossoNumeroCorrigido = "01" + nossoNumero.Substring(2);
                    idContaR = ObtemValorCampo<uint>(session, "idContaR", "numeroDocumentoCnab=?numDoc", new GDAParameter("?numDoc", nossoNumeroCorrigido));
                }

                // Feito para resolver o chamado 14262
                if (idContaR == 0 && nossoNumero.Length > 2)
                {
                    var nossoNumeroCorrigido = "15" + nossoNumero.Substring(2);
                    idContaR = ObtemValorCampo<uint>(session, "idContaR", "numeroDocumentoCnab=?numDoc", new GDAParameter("?numDoc", nossoNumeroCorrigido));
                }

                numDocCnab = nossoNumero;
            }
            else if (codbanco == (int)Sync.Utils.CodigoBanco.Itau)
            {
                idContaR = ObtemValorCampo<uint>(session, "idContaR", "numeroDocumentoCnab=?numDoc", new GDAParameter("?numDoc", usoEmpresa));
                numDocCnab = usoEmpresa;

                if (idContaR == 0 && !string.IsNullOrEmpty(usoEmpresa))
                {
                    usoEmpresa = usoEmpresa.Insert(usoEmpresa.Length - 1, "-");
                    idContaR = ObtemValorCampo<uint>(session, "idContaR", "numeroDocumentoCnab=?numDoc", new GDAParameter("?numDoc", usoEmpresa));
                    numDocCnab = usoEmpresa;
                }

                if (idContaR == 0)
                {
                    idContaR = ObtemValorCampo<uint>(session, "idContaR", "numeroDocumentoCnab=?numDoc", new GDAParameter("?numDoc", nossoNumero));
                    numDocCnab = nossoNumero;
                }

                /* Chamado 61196.
                 * Como agora é possível renegociar contas a receber que estão associadas à arquivos de remessa, não é possível buscar as contas pelo número do documento,
                 * pois essa informação pode estar duplicada no arquivo após uma renegociação. Inclusive, foi o problema resolvido através do chamado citado.
                if (idContaR == 0 && !string.IsNullOrEmpty(numeroDocumento))
                {
                    numDocCnab = numeroDocumento;
                    idContaR = BuscaIdContaRPeloNumeroDoc(session, numeroDocumento);
                }*/
            }
            else if (codbanco == (int)Sync.Utils.CodigoBanco.BancoBrasil)
            {
                // Chamado 12433. A variável "numeroDocumento" estava diferente do campo número documento na tabela de conta a receber.
                // Por isso, a conta não estava sendo buscada e não era marcada como recebida ao importar o arquivo de remessa.
                // A variável "usoEmpresa" contém o número do documento que é salvo no banco de dados, porém, com informações a mais,
                // Verificamos no banco de dados que o número do documento possui 10 caracteres, tratamos a variável para recuperar
                // os dez caracteres necessários para se buscar a conta a receber.
                if(string.IsNullOrEmpty(usoEmpresa))
                    throw new Exception("Não foi possível encontrar o número do documento passado.");

                idContaR = ObtemValorCampo<uint>(session, "idContaR", "numeroDocumentoCnab=?numDoc",
                        new GDAParameter("?numDoc", usoEmpresa.Substring(usoEmpresa.Length - 10)));

                // Caso o id da conta a receber seja igual a zero, significa que a conta não foi recuperada, então, é feita uma tentativa
                // de recuperar a conta pelo variável "numeroDocumento", que, normalmente (pelo menos no caso do chamado 12433),
                // é diferente da informação salva no banco de dados.
                if (idContaR == 0)
                    idContaR = ObtemValorCampo<uint>(session, "idContaR", "numeroDocumentoCnab=?numDoc", new GDAParameter("?numDoc", numeroDocumento));

                numDocCnab = numeroDocumento;
            }
            else if (codbanco == (int)Sync.Utils.CodigoBanco.Sicoob)
            {
                var num = nossoNumero.Substring(0, nossoNumero.Length - 1).StrParaInt();
                idContaR = ObtemValorCampo<uint>(session, "idContaR", "numeroDocumentoCnab=" + num);
                numDocCnab = num.ToString();
            }
            else if (codbanco == (int)Sync.Utils.CodigoBanco.Bradesco)
            {
                idContaR = ObtemValorCampo<uint>(session, "idContaR", "numeroDocumentoCnab=?numDoc", new GDAParameter("?numDoc", usoEmpresa));

                if (idContaR == 0)
                    idContaR = ObtemValorCampo<uint>(session, "idContaR", "numeroDocumentoCnab=?numDoc", new GDAParameter("?numDoc", numeroDocumento));

                numDocCnab = numeroDocumento;
            }
            else if (codbanco == (int)Sync.Utils.CodigoBanco.CaixaEconomicaFederal)
            {
                idContaR = ObtemValorCampo<uint>(session, "idContaR", "numeroDocumentoCnab=?numDoc", new GDAParameter("?numDoc", numeroDocumento));
                numDocCnab = numeroDocumento;

                if(idContaR == 0 && !string.IsNullOrEmpty(usoEmpresa))
                {
                    idContaR = ObtemValorCampo<uint>(session, "idContaR", "numeroDocumentoCnab=?numDoc",
                        new GDAParameter("?numDoc", usoEmpresa.Substring(usoEmpresa.Length > 10 ? -10 : 0)));
                }
            }
            else
            {
                idContaR = ObtemValorCampo<uint>(session, "idContaR", "numeroDocumentoCnab=?numDoc", new GDAParameter("?numDoc", numeroDocumento));
                numDocCnab = numeroDocumento;
            }

            if (idContaR == 0)
                throw new Exception("Não foi possível encontrar a conta a receber para o documento " + numDocCnab + ".");

            return idContaR;
        }

        private uint BuscaIdContaRPeloNumeroDoc(string numeroDocumento)
        {
            return BuscaIdContaRPeloNumeroDoc(null, numeroDocumento);
        }

        private uint BuscaIdContaRPeloNumeroDoc(GDASession session, string numeroDocumento)
        {
            uint idContaR = 0;

            var numDocTemp = numeroDocumento.Trim();
            if (numDocTemp.Length == 0)
                throw new Exception("Número de documento está em branco. Não é possível identificar a conta a receber para o documento " + numeroDocumento + ".");

            var parcelas = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'X', 'Z' };

            char parcela = numDocTemp.Substring(numDocTemp.Length - 1).ToUpper()[0];
            var numParcela = Array.IndexOf(parcelas, parcela) + 1;
            var idContaNf = Glass.Conversoes.StrParaUint(numDocTemp.Substring(0, numDocTemp.Length - 1));
            var notas = NotaFiscalDAO.Instance.GetByNumeroNFe(session, idContaNf, (int)NotaFiscal.TipoDoc.Saída);

            if (notas.Count() == 1)
            {
                var contasR = GetByNf(session, notas[0].IdNf);
                idContaR = contasR.Where(f => f.NumParc == numParcela).Select(f => f.IdContaR).FirstOrDefault();
            }

            if (idContaR == 0)
            {
                var conta = GetByIdContaR(session, idContaNf);
                if (conta != null && conta.NumParc == numParcela)
                    idContaR = conta.IdContaR;
            }

            return idContaR;
        }

        public void ValidarPagaByCnab(GDASession sessao, string numeroDocumentoCnab, uint idContaR,
            DateTime dataRec, decimal valorRec, decimal jurosMulta)
        {
            if (valorRec + jurosMulta <= 0)
            {
                throw new Exception(string.Format("Não há valor pago para o boleto {0}.", numeroDocumentoCnab));
            }

            if (!Exists(sessao, idContaR))
            {
                throw new Exception(string.Format("Boleto não encontrado: {0}.", numeroDocumentoCnab));
            }

            if (dataRec == DateTime.Parse("01/01/0001 00:00:00"))
            {
                throw new Exception(string.Format("Data de recebimento não informada no boleto {0}.", numeroDocumentoCnab));
            }
        }

        /// <summary>
        /// Marca uma conta como recebida através da importação do CNAB.
        /// </summary>
        public void PagaByCnab(GDASession sessao, string numeroDocumentoCnab, uint idContaR, DateTime dataRec, decimal valorRec,
            decimal jurosMulta, uint idContaBanco, bool caixaDiario, ref int contadorDataUnica)
        {
            try
            {
                FilaOperacoes.RecebimentosGerais.AguardarVez();

                var valorReceber = ObtemValorCampo<decimal>(sessao, "ValorVec", string.Format("IdContaR={0}", idContaR));
                /* Chamado 28317. */
                var juros = ObtemValorCampo<decimal>(sessao, "Juros", string.Format("IdContaR={0}", idContaR));
                var vazio = new List<int>();

                ReceberConta(sessao, caixaDiario, 0, new List<string>(), dataRec, false, false, (int)idContaR, null, vazio, new List<int> { (int)idContaBanco }, vazio,
                    new List<int> { (int)Pagto.FormaPagto.Boleto }, vazio, jurosMulta == 0 ? juros : jurosMulta, new List<string> { string.Empty }, null, vazio, valorRec - jurosMulta < valorReceber,
                    new List<decimal> { 0 }, vazio, new List<decimal> { valorRec });

                // Atualiza o campo DataUnica para evitar o índice BLOQUEIO_DUPLICIDADE.
                var sqlAtualizarDataUnica = $@"UPDATE caixa_geral
                    SET DataUnica = IF(INSTR(DataUnica, '_0') > 0,
                        REPLACE(DataUnica, '_0', CONCAT('_', {contadorDataUnica})),
                        CONCAT('_', {contadorDataUnica++}))
                    WHERE IdContaR = {idContaR};";
                objPersistence.ExecuteCommand(sessao, sqlAtualizarDataUnica);
            }
            finally
            {
                FilaOperacoes.RecebimentosGerais.ProximoFila();
            }
        }

        /// <summary>
        /// Método que recupera os Identificadores dos planos de conta que
        /// devem ser considerados como a prazo para a geração do CNAB.
        /// </summary>
        /// <returns>Retorna uma lista de inteiros com os identificadores dos planos de conta.</returns>
        public List<uint> ObterPlanosContaConsiderarPrazoParaCnab()
        {
            return new List<uint>
            {
                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.ParcelamentoObra),
                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.PrazoCartao)
            };
        }

        /// <summary>
        /// Recupera as contas a receber para geração do arquivo do CNAB.
        /// </summary>
        public IList<ContasReceber> GetForCnab(int tipoPeriodo, string dataIni, string dataFim, string tiposConta, int tipoContaSemSeparacao, string formasPagto,
            uint idCli, string nomeCli, uint idLoja, int idContaBancoCliente, int idContaBancoContaReceber, string idsContas, bool incluirContasAcertoParcial, bool incluirContasAntecipacaoBoleto)
        {
            string idsFormasPagto = "";

            var formasPagtoArray = formasPagto
                .Split(',')
                .Select(x => (Pagto.FormaPagto)x.StrParaUint());

            var tipoPeriodoConsiderar = tipoPeriodo == 0
                ? "c.dataVec"
                : "c.dataCad";

            foreach (var f in formasPagtoArray)
            {
                if (f == Pagto.FormaPagto.Boleto)
                {
                    idsFormasPagto += $",{UtilsPlanoConta.ContasTodosTiposBoleto()}";
                }
                if (f == Pagto.FormaPagto.Prazo)
                {
                    idsFormasPagto += $",{ UtilsPlanoConta.ContasTodasPorTipo(f)},{string.Join(",", ObterPlanosContaConsiderarPrazoParaCnab().ToArray())}";
                }
                else
                {
                    idsFormasPagto += $",{UtilsPlanoConta.ContasTodasPorTipo(f)}";
                }
            }

            var criterio = string.Empty;

            var sql = $@"
                SELECT c.*, CONCAT(cli.id_cli, ' - ', cli.nome) as nomeCli, {SqlCampoDescricaoContaContabil("c")} as descricaoContaContabil, l.nomeFantasia as NomeLoja
                FROM contas_receber c
                    INNER JOIN cliente cli ON (c.idCliente=cli.id_Cli)
                    LEFT JOIN loja l ON (c.idLoja = l.idLoja)
                    LEFT JOIN boleto_impresso bi ON (c.IdContaR = bi.IdContaR)
                WHERE COALESCE(c.isParcelaCartao, 0) = 0
                    AND coalesce(recebida, 0) = 0
                    AND numeroDocumentoCnab IS NULL
                    AND numArquivoRemessaCnab IS NULL";

            if (!incluirContasAcertoParcial && string.IsNullOrWhiteSpace(idsContas))
            {
                sql += " AND c.IdAcertoParcial IS NULL";
            }

            if (!incluirContasAntecipacaoBoleto && string.IsNullOrWhiteSpace(idsContas))
            {
                sql += " AND c.IdAntecipContaRec IS NULL";
            }

            if (!string.IsNullOrEmpty(dataIni))
            {
                sql += $" AND {tipoPeriodoConsiderar} >= ?dataIni";
            }

            if (!string.IsNullOrWhiteSpace(dataFim))
            {
                sql += $" AND {tipoPeriodoConsiderar} <= ?dataFim";
            }

            if (string.IsNullOrWhiteSpace(tiposConta))
            {
                sql += !string.IsNullOrWhiteSpace(idsContas)
                    ? string.Empty
                    : " AND 0";
            }
            else
            {
                sql += FiltroTipoConta(
                    "c",
                    tiposConta,
                    out criterio);
            }

            if (!string.IsNullOrWhiteSpace(idsFormasPagto.Trim(',')))
            {
                sql += $" AND c.idConta IN ({idsFormasPagto.Trim(',')})";
            }

            if (idCli > 0)
            {
                sql += $" AND cli.Id_Cli = {idCli}";
            }
            else if (!String.IsNullOrWhiteSpace(nomeCli))
            {
                string ids = ClienteDAO.Instance.GetIds(
                    null,
                    nomeCli,
                    null,
                    0,
                    null,
                    null,
                    null,
                    null,
                    0);

                sql += $" AND cli.Id_Cli IN ({ids})";
            }

            if (idLoja > 0)
            {
                sql += $" AND c.IdLoja={idLoja}";
            }

            if (idContaBancoCliente > 0)
            {
                sql += $" AND cli.IdContaBanco = {idContaBancoCliente}";
            }

            if (idContaBancoContaReceber > 0)
            {
                sql += $" AND bi.IdContaBanco = {idContaBancoContaReceber}";
            }

            if (!string.IsNullOrWhiteSpace(idsContas) && !string.IsNullOrWhiteSpace(idsContas.Trim(',')))
            {
                sql += $" AND c.IdContaR IN ({idsContas.Trim(',')})";
            }

            sql += " ORDER BY dataVec";

            var retorno =  objPersistence.LoadData(sql, new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")),
                new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59"))).ToList();

            if (tipoContaSemSeparacao > 0)
            {
                foreach (var conta in retorno.ToList())
                {
                    var possuiNota = NotaFiscalDAO.Instance.ObtemIdNfByContaR(
                        conta.IdContaR,
                        true)
                        .Count() > 0;

                    if (tipoContaSemSeparacao == 1 && !possuiNota)
                    {
                        retorno.Remove(conta);
                    }
                    else if (tipoContaSemSeparacao == 2 && possuiNota)
                    {
                        retorno.Remove(conta);
                    }
                }
            }

            return retorno;
        }

        /// <summary>
        /// Verifica se foi gerado arquivo remessa para a conta
        /// </summary>
        /// <param name="idContaR"></param>
        /// <returns></returns>
        public bool TemCnabGerado(uint idContaR)
        {
            var numeroArquivoRemessaCnab = ObtemValorCampo<uint>("numArquivoRemessaCnab", "idContaR=" + idContaR);
            var numeroDocumentoCnab = ObtemValorCampo<string>("NumeroDocumentoCnab", "idContaR=" + idContaR);

            return numeroArquivoRemessaCnab > 0 || !string.IsNullOrEmpty(numeroDocumentoCnab);
        }

        /// <summary>
        /// Verifica se foi cobrado a tarifa de uso de boleto para a conta informada.
        /// </summary>
        /// <param name="idContaR"></param>
        /// <returns></returns>
        public bool CobrouTarifaBoleto(uint idContaR)
        {
            return ObtemValorCampo<bool>("TarifaBoleto", "idContaR=" + idContaR);
        }

        /// <summary>
        /// Verifica se foi cobrado a tarifa de protesto de boleto para a conta informada.
        /// </summary>
        /// <param name="idContaR"></param>
        /// <returns></returns>
        public bool CobrouTarifaProtesto(uint idContaR)
        {
            return ObtemValorCampo<bool>("TarifaProtesto", "idContaR=" + idContaR);
        }

        /// <summary>
        /// Marca as contas informados como ja recebido a tarifa para uso do boleto
        /// </summary>
        /// <param name="idsContasR"></param>
        public void MarcaCobradoTarifaBoleto(string idsContasR)
        {
            objPersistence.ExecuteCommand(string.Format("UPDATE contas_receber set TarifaBoleto = 1 WHERE IdContaR IN ({0})", idsContasR));
        }

        /// <summary>
        /// Marca as contas informados como ja recebido a tarifa para protesto do boleto
        /// </summary>
        /// <param name="idsContasR"></param>
        public void MarcaCobradoTarifaProtesto(string idsContasR)
        {
            objPersistence.ExecuteCommand(string.Format("UPDATE contas_receber set TarifaProtesto = 1 WHERE IdContaR IN ({0})", idsContasR));
        }

        /// <summary>
        /// Obtem as contas a receber para retificar o arquivo remessa.
        /// </summary>
        /// <param name="idArquivoRemessa">Identificador do arquivo de remessa.</param>
        /// <param name="idCli">Identificador do cliente.</param>
        /// <param name="nomeCli">Nome do cliente.</param>
        /// <returns>Contas a receber para retificar associadas ao arquivo de remessa.</returns>
        public IList<ContasReceber> ObterContasReceberParaRetificarArquivoRemessa(int idArquivoRemessa, int idCli, string nomeCli)
        {
            if (idArquivoRemessa == 0 && idCli == 0 && string.IsNullOrWhiteSpace(nomeCli))
            {
                return new List<ContasReceber>();
            }

            var sql = $@"
                SELECT c.*, CONCAT(cli.id_cli, ' - ', cli.nome) as nomeCli, {this.SqlCampoDescricaoContaContabil("c")} as descricaoContaContabil, l.nomeFantasia as NomeLoja
                FROM contas_receber c
                    INNER JOIN cliente cli ON (c.idCliente=cli.id_Cli)
                    LEFT JOIN loja l ON (c.idLoja = l.idLoja)
                WHERE (c.Recebida = null OR c.Recebida = 0)";

            if (idArquivoRemessa > 0)
            {
                sql += " AND idArquivoRemessa=" + idArquivoRemessa;
            }
            else
            {
                sql += " AND idArquivoRemessa > 0";
            }

            if (idCli > 0)
            {
                sql += " AND cli.ID_CLI=" + idCli;
            }
            else if (!string.IsNullOrEmpty(nomeCli))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCli, null, 0, null, null, null, null, 0);
                sql += " AND cli.ID_CLI in (" + ids + ")";
            }

            sql += " ORDER BY dataVec";

            var retorno = this.objPersistence.LoadData(sql).ToList();

            return retorno;
        }

        public void MarcarJuridico(int idContaR, bool juridico)
        {
            objPersistence.ExecuteCommand("UPDATE contas_receber SET Juridico = " + juridico + " WHERE IdContaR = " + idContaR);

            LogAlteracaoDAO.Instance.Insert(new LogAlteracao()
            {
                Tabela = (int)LogAlteracao.TabelaAlteracao.ContasReceber,
                IdRegistroAlt = idContaR,
                Campo = "Jurídico/Cartório",
                DataAlt = DateTime.Now,
                IdFuncAlt = UserInfo.GetUserInfo.CodUser,
                ValorAtual = juridico ? "Sim" : "Não"
            });
        }

        #endregion

        #region Recupera campos específicos da conta a receber

        /// <summary>
        /// Insere todas as contas a receber na tabela contas_receber_data_base.
        /// </summary>
        public void InserirContasReceberNoHistoricoDeContasAReceber()
        {
            objPersistence.ExecuteCommand(@"INSERT INTO contas_receber_data_base (IDCONTAR, IDCLIENTE, DATACAD, DATAVEC, VALORVEC, DATACOPIA)
                (SELECT c.IdContaR, c.IdCliente, c.DataCad, c.DataVec, c.ValorVec, NOW()
                FROM contas_receber c
                WHERE c.Recebida = 0
                ORDER BY c.DataVec)");
        }

        #endregion

        #region Retorna o número de parcelas do pedido acrescentando 1

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Retorna o número de parcelas do pedido acrescentando 1
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int ObtemNumParcPedido(uint idPedido)
        {
            return ObtemNumParcPedido(null, idPedido);
        }

        /// <summary>
        /// Retorna o número de parcelas do pedido acrescentando 1
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int ObtemNumParcPedido(GDASession sessao, uint idPedido)
        {
            string sql = "Select Coalesce(Max(NumParc) + 1, 1) From contas_receber Where idPedido=" + idPedido;

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sessao, sql).ToString());
        }

        public int ObterNumParcMaxObra(GDASession sessao, uint idObra)
        {
            return ObtemValorCampo<int>(sessao, "MAX(NumParcMax)", "COALESCE(IsParcelaCartao, 0)=1 AND IdObra=" + idObra);
        }

        public int ObterNumParcMaxAcerto(GDASession sessao, uint idAcerto)
        {
            return ObtemValorCampo<int>(sessao, "MAX(NumParcMax)", "COALESCE(IsParcelaCartao, 0)=1 AND (idAcerto=" + idAcerto + " OR IdAcertoParcial=" + idAcerto + ")");
        }

        public int ObterNumParcMaxLiberacao(GDASession sessao, uint idLiberacao)
        {
            return ObtemValorCampo<int>(sessao, "NumParcMax", "COALESCE(IsParcelaCartao, 0)=1 AND IdLiberarPedido=" + idLiberacao);
        }

        public int ObterNumParcMaxContaR(GDASession sessao, uint idContaR)
        {
            var contaR = GetByIdContaR(sessao, idContaR);

            // Retorna o NumParcMax da conta receber referente a Parcela Cartão do acerto
            if (contaR.IdAcerto.GetValueOrDefault() > 0)
            {
                return ObterNumParcMaxAcerto(sessao, contaR.IdAcerto.Value);
            }
            // Retorna o NumParcMax da conta receber referente a Parcela Cartão do acerto parcial
            else if (contaR.IdAcertoParcial.GetValueOrDefault() > 0)
            {
                return ObterNumParcMaxAcerto(sessao, contaR.IdAcertoParcial.Value);
            }
            // Retorna o NumParcMax da conta receber referente a Parcela Cartão da obra
            else if (contaR.IdObra.GetValueOrDefault() > 0)
            {
                return ObterNumParcMaxObra(sessao, contaR.IdObra.Value);
            }
            // Retorna o NumParcMax da conta receber referente a Parcela Cartão da liberação
            else if (contaR.IdLiberarPedido.GetValueOrDefault() > 0)
            {
                return ObterNumParcMaxLiberacao(sessao, contaR.IdLiberarPedido.Value);
            }
            // Retorna o NumParcMax da conta receber buscada
            else
            {
                return ObtemValorCampo<int>(sessao, "MAX(NumParcMax)", "COALESCE(IsParcelaCartao, 0)=1 AND idContaR=" + idContaR);
            }
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Atualiza o número de parcelas do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="numParc"></param>
        public void AtualizaNumParcPedido(uint idPedido, int numParc)
        {
            AtualizaNumParcPedido(null, idPedido, numParc);
        }

        /// <summary>
        /// Atualiza o número de parcelas do pedido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="numParc"></param>
        public void AtualizaNumParcPedido(GDASession sessao, uint idPedido, int numParc)
        {
            objPersistence.ExecuteCommand(sessao, "update contas_receber set numParcMax=" + numParc + " where idPedido=" + idPedido);
        }

        #endregion

        #region Retorna o total de débitos do cliente

        /// <summary>
        /// Obtém o total de débitos do cliente.
        /// </summary>
        /// <param name="idCliente"></param>
        public decimal ObtemTotalDebitosCliente(uint idCliente)
        {
            var sql = "Select Sum(ValorVec) From (" +
                SqlDebitos(idCliente, 0, 0, null, null, null, null, TipoDebito.Todos,
                0, null, null, true) + ") As temp";

            return ExecuteScalar<decimal>(sql);
        }

        #endregion

        #region Recupera a referência de uma conta a receber

        public string GetReferencia(uint idContaR)
        {
            return GetReferencia(null, idContaR);
        }

        public string GetReferencia(GDASession session, uint idContaR)
        {
            string where = "idContaR=" + idContaR;

            ContasReceber cr = new ContasReceber();
            cr.IdNf = ObtemValorCampo<uint?>(session, "idNf", where);
            cr.IdAcerto = ObtemValorCampo<uint?>(session, "idAcerto", where);
            cr.IdAcertoParcial = ObtemValorCampo<uint?>(session, "idAcertoParcial", where);
            cr.IdAntecipContaRec = ObtemValorCampo<uint?>(session, "idAntecipContaRec", where);
            cr.IdDevolucaoPagto = ObtemValorCampo<uint?>(session, "idDevolucaoPagto", where);
            cr.IdLiberarPedido = ObtemValorCampo<uint?>(session, "idLiberarPedido", where);
            cr.IdObra = ObtemValorCampo<uint?>(session, "idObra", where);
            cr.IdPedido = ObtemValorCampo<uint?>(session, "idPedido", where);
            cr.IdTrocaDevolucao = ObtemValorCampo<uint?>(session, "idTrocaDevolucao", where);
            cr.IdSinal = ObtemValorCampo<uint?>(session, "idSinal", where);
            cr.IdAcertoCheque = ObtemValorCampo<uint?>(session, "idAcertoCheque", where);
            cr.IdEncontroContas = ObtemValorCampo<uint?>(session, "idEncontroContas", where);
            cr.NumParcMax = ObtemValorCampo<int>(session, "NumParcMax", where);
            cr.NumParc = ObtemValorCampo<int>(session, "NumParc", where);
            cr.IdContaRRef = ObtemValorCampo<int>(session, "IdContaRRef", where);

            return cr.Referencia;
        }

        #endregion

        #region Tipos de contas a receber

        public GenericModel[] ObtemTiposContas()
        {
            List<GenericModel> lst = new List<GenericModel>();

            lst.Add(new GenericModel((int)ContasReceber.TipoContaEnum.Contabil, FinanceiroConfig.ContasPagarReceber.DescricaoContaContabil));
            lst.Add(new GenericModel((int)ContasReceber.TipoContaEnum.NaoContabil, FinanceiroConfig.ContasPagarReceber.DescricaoContaNaoContabil));

            if (FinanceiroConfig.ContasPagarReceber.DescricaoContaNaoContabil != FinanceiroConfig.ContasPagarReceber.DescricaoContaCupomFiscal)
                lst.Add(new GenericModel((int)ContasReceber.TipoContaEnum.CupomFiscal, FinanceiroConfig.ContasPagarReceber.DescricaoContaCupomFiscal));

            lst.Add(new GenericModel((int)ContasReceber.TipoContaEnum.Reposicao, "Reposição"));

            return lst.ToArray();
        }

        #endregion

        #region Métodos sobrescritos

        private string GetWhere(object item)
        {
            return item == null ? " is null" : "=" + item.ToString();
        }

        public new uint InsertBase(GDASession sessao, ContasReceber objInsert)
        {
            return base.Insert(sessao, objInsert);
        }

        public uint InsertExecScript(ContasReceber objInsert)
        {
            string sql = @"select idContaR from contas_receber where idCliente=?idCliente and date(dataVec)=date(?dataVenc)
                and valorVec=?valorVenc and numParc=?numParc and tipoConta=?tipoConta and idLoja=?idLoja";

            var idContaR = ExecuteScalar<uint?>(sql, new GDAParameter("?idCliente", objInsert.IdCliente),
                new GDAParameter("?dataVenc", objInsert.DataVec), new GDAParameter("?valorVenc", objInsert.ValorVec),
                new GDAParameter("?numParc", objInsert.NumParc), new GDAParameter("?tipoConta", objInsert.TipoConta),
                new GDAParameter("?idLoja", objInsert.IdLoja));

            if (idContaR.GetValueOrDefault(0) == 0)
                return Insert(null, objInsert);
            else
                throw new Exception("Conta já inserida. Cliente:" + objInsert.IdCliente);
        }

        public override int Delete(GDASession session, ContasReceber objDelete)
        {
            return DeleteByPrimaryKey(session, objDelete.IdContaR, false);
        }

        public override int Delete(ContasReceber objDelete)
        {
            return DeleteByPrimaryKey(null, objDelete.IdContaR, true);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public override int DeleteByPrimaryKey(uint Key)
        {
            return DeleteByPrimaryKey(null, Key, false);
        }

        public override int DeleteByPrimaryKey(GDASession sessao, uint Key)
        {
            return DeleteByPrimaryKey(sessao, Key, false);
        }

        private int DeleteByPrimaryKey(GDASession sessao, uint Key, bool manual)
        {
            if (!Exists(sessao, Key))
                return 0;

            ContasReceber objDelete = GetElementByPrimaryKey(sessao, Key);

            // Não permite excluir contas a receber que possuam referência no caixa geral
            if (objPersistence.ExecuteSqlQueryCount(sessao, "Select Count(*) From caixa_geral Where idContaR=" + Key) > 0)
            {
                // Caso tenha pedido, apaga o IdContaR e salva o IdPedido apenas.
                if (objDelete.IdPedido > 0)
                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE caixa_geral SET IdContaR=null, IdPedido={0} WHERE IdContaR={1}", objDelete.IdPedido, Key));
                // Caso tenha acerto, apaga o IdContaR e salva o IdAcerto apenas.
                else if (objDelete.IdAcerto > 0)
                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE caixa_geral SET IdContaR=NULL, IdAcerto={0} WHERE IdContaR={1}", objDelete.IdAcerto, Key));
                // Caso tenha acerto parcial, apaga o IdContaR e salva o IdAcerto apenas (não existe o campo IdAcertoParcial no caixa geral).
                else if (objDelete.IdAcertoParcial > 0)
                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE caixa_geral SET IdContaR=NULL, IdAcerto={0} WHERE IdContaR={1}", objDelete.IdAcertoParcial, Key));
                // Caso tenha obra, apaga o IdContaR e salva o IdObra apenas.
                else if (objDelete.IdObra > 0)
                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE caixa_geral SET IdContaR=null, IdObra={0} WHERE IdContaR={1}", objDelete.IdObra, Key));
                // Caso tenha liberação, apaga o IdContaR e salva o IdLiberarPedido apenas.
                else if (objDelete.IdLiberarPedido > 0)
                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE caixa_geral SET IdContaR=null, IdLiberarPedido={0} WHERE IdContaR={1}", objDelete.IdLiberarPedido, Key));
                // Caso tenha troca/devolução, apaga o IdContaR e salva o IdTrocaDevolucao apenas.
                else if (objDelete.IdTrocaDevolucao > 0)
                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE caixa_geral SET IdContaR=null, IdTrocaDevolucao={0} WHERE IdContaR={1}", objDelete.IdTrocaDevolucao, Key));
                // Caso tenha Cartão Não Identificado, apaga o IdContaR e salva o IdCartaoNaoIdentificado apenas.
                else if (objDelete.IdCartaoNaoIdentificado > 0)
                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE caixa_geral SET IdContaR=null, IdCartaoNaoIdentificado={0} WHERE IdContaR={1}", objDelete.IdCartaoNaoIdentificado, Key));
                // Caso tenha Sinal/Pagamento antecipado, apaga o IdContaR e salva o IdSinal apenas.
                else if (objDelete.IdSinal > 0)
                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE caixa_geral SET IdContaR=null, IdSinal={0} WHERE IdContaR={1}", objDelete.IdSinal, Key));
                else
                    throw new Exception("Esta conta a receber não pode ser excluída pois possui referência no caixa geral.");
            }

            // Não permite excluir contas a receber que possuam referência na conta bancária
            if (objPersistence.ExecuteSqlQueryCount(sessao, "Select Count(*) From mov_banco Where idContaR=" + Key) > 0)
            {
                // Caso tenha acerto, apaga o IdContaR e salva o IdAcerto/IdAcertoParcial apenas.
                if (objDelete.IdAcerto > 0 || objDelete.IdAcertoParcial > 0)
                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE mov_banco SET IdContaR=NULL, IdAcerto={0} WHERE IdContaR={1}", objDelete.IdAcerto ?? objDelete.IdAcertoParcial, Key));
                // Caso tenha acerto de cheque, apaga o IdContaR e salva o IdAcertoCheque apenas.
                else if (objDelete.IdAcertoCheque > 0)
                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE mov_banco SET IdContaR=NULL, IdAcertoCheque={0} WHERE IdContaR={1}", objDelete.IdAcertoCheque, Key));
                // Caso tenha sinal ou pagamento antecipado, apaga o IdContaR e salva o IdSinal apenas.
                else if (objDelete.IdSinal > 0)
                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE mov_banco SET IdContaR=NULL, IdSinal={0} WHERE IdContaR={1}", objDelete.IdSinal, Key));
                // Caso tenha obra, apaga o IdContaR e salva o IdObra apenas.
                else if (objDelete.IdObra > 0)
                {
                    /* Chamado 63242. */
                    if (objDelete.IdContaRCartao > 0)
                        objPersistence.ExecuteCommand(sessao, string.Format("UPDATE mov_banco SET IdContaR={0} WHERE IdContaR={1}", objDelete.IdContaRCartao, Key));
                    else
                        objPersistence.ExecuteCommand(sessao, string.Format("UPDATE mov_banco SET IdContaR=NULL, IdObra={0} WHERE IdContaR={1}", objDelete.IdObra, Key));
                }
                // Caso tenha devolução de pagamento, apaga o IdContaR e salva o IdDevolucaoPagto apenas.
                else if (objDelete.IdDevolucaoPagto > 0)
                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE mov_banco SET IdContaR=NULL, IdDevolucaoPagto={0} WHERE IdContaR={1}", objDelete.IdDevolucaoPagto, Key));
                // Caso tenha Cartão Não Identificado, apaga o IdContaR e salva o IdCartaoNaoIdentificado apenas.
                else if (objDelete.IdCartaoNaoIdentificado > 0)
                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE mov_banco SET IdContaR=null, IdCartaoNaoIdentificado={0} WHERE IdContaR={1}", objDelete.IdCartaoNaoIdentificado, Key));
                // Caso tenha Liberação, apaga o IdContaR e salva o IdLiberarPedido apenas.
                else if (objDelete.IdLiberarPedido > 0)
                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE mov_banco SET IdContaR=null, IdLiberarPedido={0} WHERE IdContaR={1}", objDelete.IdLiberarPedido, Key));
                // Caso tenha troca/devolução, apaga o IdContaR e salva o IdTrocaDevolucao apenas.
                else if (objDelete.IdTrocaDevolucao > 0)
                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE mov_banco SET IdContaR=null, IdTrocaDevolucao={0} WHERE IdContaR={1}", objDelete.IdTrocaDevolucao, Key));
                // Caso tenha pedido, apaga o IdContaR e salva o IdPedido apenas.
                else if (objDelete.IdPedido > 0)
                    objPersistence.ExecuteCommand(sessao, string.Format("UPDATE mov_banco SET IdContaR=null, IdPedido={0} WHERE IdContaR={1}", objDelete.IdPedido, Key));
                else
                    throw new Exception("Esta conta a receber não pode ser excluída pois possui referência na conta bancária.");
            }

            var retorno = 0;

            PagtoContasReceberDAO.Instance.DeleteByIdContaR(sessao, Key);

            retorno = base.DeleteByPrimaryKey(sessao, Key);

            LogCancelamentoDAO.Instance.LogContaReceber(sessao, objDelete, null, manual);

            return retorno;
        }

        #endregion

        #region Busca contas a receber \ recebidas para o registro de importação do arquivo remessa

        private string SqlForRegistroImportacao(uint idContaR, uint idPedido, uint idLiberarPedido, uint idAcerto, uint idAcertoParcial, uint idTrocaDevolucao,
            uint numNFe, uint idCli, string nomeCli, uint idLoja, bool lojaCliente, uint numArqRemessa, uint idFormaPagto, string obs,
            string dtIniVenc, string dtFimVenc, string dtIniRec, string dtFimRec, decimal valorVecIni, decimal valorVecFim, decimal valorRecIni,
            decimal valorRecFim, int recebida, int codOcorrencia, string nossoNumero, string numDoc, string usoEmpresa, int idContaBanco)
        {
            string campos = "cr.*, cli.Nome as NomeCli, pl.Descricao as DescrPlanoConta, '$$$' as criterios";

            campos += SqlBuscarNF("cr", true, numNFe, false, true);

            string sql = @"
            SELECT " + campos + @"
            FROM contas_receber cr
                LEFT JOIN cliente cli ON (cr.idCliente = cli.id_cli)
                LEFT JOIN plano_contas pl ON (cr.IdConta = pl.IdConta)
                LEFT JOIN arquivo_remessa ar ON (cr.IdArquivoRemessa = ar.IdArquivoRemessa)
                {0}
            WHERE (cr.numArquivoRemessaCnab IS NOT NULL OR cr.numeroDocumentoCnab IS NOT NULL)";

            string criterio = "";

            if (idContaR > 0)
            {
                sql += " AND cr.idContaR=" + idContaR;
                criterio += "Conta à receber: " + idContaR + "    ";
            }

            if (idPedido > 0)
            {
                sql += !PedidoConfig.LiberarPedido ? " And cr.IdPedido=" + idPedido :
                    " And (cr.IdPedido=" + idPedido + " Or cr.IdLiberarPedido in (select distinct idLiberarPedido from produtos_liberar_pedido where idPedido=" + idPedido + "))";
                criterio += "Pedido: " + idPedido + "    ";
            }

            if (idLiberarPedido > 0)
            {
                if (FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber)
                    sql += " AND (cr.IdLiberarPedido=" + idLiberarPedido + @"
                        OR cr.idNf IN (SELECT idNf
                                        FROM pedidos_nota_fiscal
                                        WHERE idLiberarPedido=" + idLiberarPedido + "))";
                else
                    sql += " And cr.IdLiberarPedido=" + idLiberarPedido;

                criterio += "Liberação: " + idLiberarPedido + "    ";
            }

            if (idAcerto > 0)
            {
                sql += " and (cr.idAcerto=" + idAcerto + " Or cr.idAcertoParcial=" + idAcerto + ")";
                criterio += "Acerto: " + idAcerto + "    ";
            }

            if (idTrocaDevolucao > 0)
            {
                sql += " And cr.idTrocaDevolucao=" + idTrocaDevolucao;
                criterio += "Troca/Devolução: " + idTrocaDevolucao;
            }

            if (idAcertoParcial > 0)
            {
                sql += " and cr.idAcertoParcial=" + idAcertoParcial;
                criterio += "Acerto parcial N.º " + idAcertoParcial + "    ";
            }

            if (idCli > 0)
            {
                sql += " And cr.IdCliente=" + idCli;
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCli) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeCli))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCli, null, 0, null, null, null, null, 0);
                sql += " And cr.idCliente in (" + ids + ")";
                criterio += "Cliente: " + nomeCli + "    ";
            }

            if (idLoja > 0)
            {
                if (!lojaCliente)
                    sql += " And cr.idLoja=" + idLoja;
                else
                    sql += " And cli.id_Loja=" + idLoja;

                criterio += "Loja" + (lojaCliente ? " do cliente" : "") + ": " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (numArqRemessa > 0)
            {
                sql += " And cr.numArquivoRemessaCnab=" + numArqRemessa;
                criterio += "Núm. Arquivo Remessa: " + numArqRemessa + "    ";
            }

            if (idFormaPagto > 0)
            {
                if (idFormaPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto)
                    sql += " And cr.idConta In (" + UtilsPlanoConta.ContasTodosTiposBoleto() + ")";
                else
                    sql += " And cr.idConta in (" + UtilsPlanoConta.ContasTodasPorTipo((Glass.Data.Model.Pagto.FormaPagto)idFormaPagto) + ")";

                criterio += "Forma Pagto.: " + PagtoDAO.Instance.GetDescrFormaPagto(idFormaPagto);
            }

            if (!string.IsNullOrEmpty(obs))
            {
                sql += " and cr.obs like ?obs";
                criterio += "Obs.: " + obs + "    ";
            }

            if (!String.IsNullOrEmpty(dtIniVenc))
            {
                sql += " And cr.DATAVEC>=?dtIniVenc";
                criterio += "Data Início Venc.: " + dtIniVenc + "    ";
            }

            if (!String.IsNullOrEmpty(dtFimVenc))
            {
                sql += " And cr.DATAVEC<=?dtFimVenc";
                criterio += "Data Fim Venc.: " + dtFimVenc + "    ";
            }

            if (!String.IsNullOrEmpty(dtIniRec))
            {
                sql += " And cr.DATAREC>=?dtIniRec";
                criterio += "Data Início Rec.: " + dtIniRec + "    ";
            }

            if (!String.IsNullOrEmpty(dtFimRec))
            {
                sql += " And cr.DATAREC<=?dtFimRec";
                criterio += "Data Fim Rec.: " + dtFimRec + "    ";
            }

            if (valorVecIni > 0)
            {
                sql += " And cr.valorVec >= " + valorVecIni.ToString().Replace(',', '.');
                criterio += "Venc. a partir de: " + valorVecIni.ToString("C") + "    ";
            }

            if (valorVecFim > 0)
            {
                sql += " And cr.valorVec <=" + valorVecFim.ToString().Replace(',', '.');
                criterio += (valorVecFim > 0 ? "" : "Venc. ") + "Até: " + valorVecFim.ToString("C") + "    ";
            }

            if (valorRecIni > 0)
            {
                sql += " And cr.valorRec >= " + valorRecIni.ToString().Replace(',', '.');
                criterio += "Receb. a partir de: " + valorRecIni.ToString("C") + "    ";
            }

            if (valorRecFim > 0)
            {
                sql += " And cr.valorRec <=" + valorRecFim.ToString().Replace(',', '.');
                criterio += (valorRecIni > 0 ? "" : "Receb. ") + "Até: " + valorRecFim.ToString("C") + "    ";
            }

            if (recebida > 0)
            {
                if (recebida == 1)
                    sql += " AND cr.recebida=true";
                else if (recebida == 2)
                    sql += " AND cr.recebida=false";
            }

            var filtroRegistro = false;

            if (codOcorrencia > 0)
            {
                sql += " AND rar.codOcorrencia=" + codOcorrencia;
                filtroRegistro = true;
            }

            if (!string.IsNullOrEmpty(nossoNumero))
            {
                sql += " AND rar.nossoNumero like ?nossoNumero";
                filtroRegistro = true;
            }

            if (!string.IsNullOrEmpty(numDoc))
            {
                sql += " AND rar.numeroDocumento like ?numDoc";
                filtroRegistro = true;
            }

            if (!string.IsNullOrEmpty(usoEmpresa))
            {
                sql += " AND rar.usoEmpresa like ?usoEmpresa";
                filtroRegistro = true;
            }

            if (idContaBanco > 0)
            {
                sql += " AND ar.IdContaBanco = " + idContaBanco;
                criterio += "Conta Bancária: " + ContaBancoDAO.Instance.GetDescricao((uint)idContaBanco);
                filtroRegistro = true;
            }

            sql = string.Format(sql, filtroRegistro ? "INNER JOIN registro_arquivo_remessa rar ON (cr.idContaR = rar.idContaR)" : "");

            sql += " GROUP BY cr.idContaR";

            if (numNFe > 0)
            {
                sql += " having 1";
                sql += " and concat(',', numeroNFe, ',') like '%," + numNFe + ",%'";
                criterio += "Número NFe: " + numNFe + "    ";
            }

            return (sql + " ORDER BY cr.dataRec Desc").Replace("$$$", criterio);
        }

        public IList<ContasReceber> GetListRegistroArquivoRemessa(uint idContaR, uint idPedido, uint idLiberarPedido, uint idAcerto, uint idAcertoParcial,
            uint idTrocaDevolucao, uint numNFe, uint idCli, string nomeCli, uint idLoja, bool lojaCliente, uint numArqRemessa, uint idFormaPagto,
            string obs, string dtIniVenc, string dtFimVenc, string dtIniRec, string dtFimRec, decimal valorVecIni, decimal valorVecFim,
            decimal valorRecIni, decimal valorRecFim, int recebida, int codOcorrencia, string nossoNumero, string numDoc, string usoEmpresa, int idContaBanco,
            string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(SqlForRegistroImportacao(idContaR, idPedido, idLiberarPedido, idAcerto, idAcertoParcial, idTrocaDevolucao,
                numNFe, idCli, nomeCli, idLoja, lojaCliente, numArqRemessa, idFormaPagto, obs, dtIniVenc, dtFimVenc, dtIniRec, dtFimRec,
                valorVecIni, valorVecFim, valorRecIni, valorRecFim, recebida, codOcorrencia, nossoNumero, numDoc, usoEmpresa, idContaBanco),
                sortExpression, startRow, pageSize, GetParamRegistroArqRemessa(nomeCli, dtIniVenc, dtFimVenc, dtIniRec, dtFimRec, obs,
                nossoNumero, numDoc, usoEmpresa));
        }

        public int GetListRegistroArquivoRemessaCount(uint idContaR, uint idPedido, uint idLiberarPedido, uint idAcerto, uint idAcertoParcial,
            uint idTrocaDevolucao, uint numNFe, uint idCli, string nomeCli, uint idLoja, bool lojaCliente, uint numArqRemessa, uint idFormaPagto,
            string obs, string dtIniVenc, string dtFimVenc, string dtIniRec, string dtFimRec, decimal valorVecIni, decimal valorVecFim,
            decimal valorRecIni, decimal valorRecFim, int recebida, int codOcorrencia, string nossoNumero, string numDoc, string usoEmpresa, int idContaBanco)
        {
            return GetCountWithInfoPaging(SqlForRegistroImportacao(idContaR, idPedido, idLiberarPedido, idAcerto, idAcertoParcial, idTrocaDevolucao,
                numNFe, idCli, nomeCli, idLoja, lojaCliente, numArqRemessa, idFormaPagto, obs, dtIniVenc, dtFimVenc, dtIniRec, dtFimRec,
                valorVecIni, valorVecFim, valorRecIni, valorRecFim, recebida, codOcorrencia, nossoNumero, numDoc, usoEmpresa, idContaBanco),
                GetParamRegistroArqRemessa(nomeCli, dtIniVenc, dtFimVenc, dtIniRec, dtFimRec, obs, nossoNumero, numDoc, usoEmpresa));
        }

        public IList<ContasReceber> GetListRegistroArquivoRemessaForRpt(uint idContaR, uint idPedido, uint idLiberarPedido, uint idAcerto, uint idAcertoParcial,
           uint idTrocaDevolucao, uint numNFe, uint idCli, string nomeCli, uint idLoja, bool lojaCliente, uint numArqRemessa, uint idFormaPagto,
           string obs, string dtIniVenc, string dtFimVenc, string dtIniRec, string dtFimRec, decimal valorVecIni, decimal valorVecFim,
           decimal valorRecIni, decimal valorRecFim, int recebida, int codOcorrencia, string nossoNumero, string numDoc, string usoEmpresa, int idContaBanco)
        {
            return objPersistence.LoadData(SqlForRegistroImportacao(idContaR, idPedido, idLiberarPedido, idAcerto, idAcertoParcial, idTrocaDevolucao,
                numNFe, idCli, nomeCli, idLoja, lojaCliente, numArqRemessa, idFormaPagto, obs, dtIniVenc, dtFimVenc, dtIniRec, dtFimRec,
                valorVecIni, valorVecFim, valorRecIni, valorRecFim, recebida, codOcorrencia, nossoNumero, numDoc, usoEmpresa, idContaBanco),
                GetParamRegistroArqRemessa(nomeCli, dtIniVenc, dtFimVenc, dtIniRec, dtFimRec, obs,
                nossoNumero, numDoc, usoEmpresa)).ToList();
        }

        private GDAParameter[] GetParamRegistroArqRemessa(string nomeCli, string dtIniVenc, string dtFimVenc, string dtIniRec, string dtFimRec,
            string obs, string nossoNumero, string numDoc, string usoEmpresa)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeCli))
                lstParam.Add(new GDAParameter("?nomeCli", "%" + nomeCli + "%"));

            if (!String.IsNullOrEmpty(dtIniVenc))
                lstParam.Add(new GDAParameter("?dtIniVenc", DateTime.Parse(dtIniVenc + " 00:00")));

            if (!String.IsNullOrEmpty(dtFimVenc))
                lstParam.Add(new GDAParameter("?dtFimVenc", DateTime.Parse(dtFimVenc + " 23:59")));

            if (!String.IsNullOrEmpty(dtIniRec))
                lstParam.Add(new GDAParameter("?dtIniRec", DateTime.Parse(dtIniRec + " 00:00")));

            if (!String.IsNullOrEmpty(dtFimRec))
                lstParam.Add(new GDAParameter("?dtFimRec", DateTime.Parse(dtFimRec + " 23:59")));

            if (!string.IsNullOrEmpty(obs))
                lstParam.Add(new GDAParameter("?obs", "%" + obs + "%"));

            if (!string.IsNullOrEmpty(nossoNumero))
                lstParam.Add(new GDAParameter("?nossoNumero", "%" + nossoNumero + "%"));

            if (!string.IsNullOrEmpty(numDoc))
                lstParam.Add(new GDAParameter("?numDoc", "%" + numDoc + "%"));

            if (!string.IsNullOrEmpty(usoEmpresa))
                lstParam.Add(new GDAParameter("?usoEmpresa", "%" + usoEmpresa + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Busca contas recebidas para o ajuste da tabela pagto_contas_receber

        public IList<ContasReceber> ObtemContasParaAjustePagtoContasReceber(DateTime dataInicioRecebimento, DateTime dataFimRecebimento)
        {
            var sql = string.Format(@"
                SELECT *
                FROM contas_receber cr
                WHERE (cr.Recebida IS NOT NULL AND cr.Recebida = 1) AND cr.DataRec>='{0}' AND cr.DataRec<='{1}'",
                dataInicioRecebimento.ToString("yyyy-MM-dd 00:00:00"),
                dataFimRecebimento.ToString("yyyy-MM-dd 23:59:59"));

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Verifica de existem boletos vencendo no dia

        public bool BoletosVencimentoHoje()
        {
            return objPersistence.ExecuteSqlQueryCount("Select Count(*) From contas_receber Where idformapagto=" +
                (int)Pagto.FormaPagto.Boleto + " AND DATE_FORMAT(datavec,'%d/%m/%Y') = '" + DateTime.Now.ToShortDateString() + "'") > 0;
        }

        #endregion

        #region Busca contas recebidas para gerar comissão

        private string SqlContasRecebidasParaComissao(uint idFunc, int idLoja, string tipoContaContabil, string dataIni, string dataFim,
            string idsContasR, uint idComissao, string dataRecIni, string dataRecFim, bool paraRelatorio, ref List<GDAParameter> lstParams)
        {
            var campoTipoContaContabil = SqlCampoDescricaoContaContabil("cr");

            var tabelasFuncionarioComissao =
                paraRelatorio ?
                    @"INNER JOIN comissao com ON (ccr.IdComissao=com.IdComissao)
                    INNER JOIN funcionario f ON (com.IdFunc = f.IdFunc)" :
                    "INNER JOIN funcionario f ON (cr.IdFuncComissaoRec = f.IdFunc)";

            var sql = string.Format(@"
                SELECT cr.*, c.Nome as NomeCli, {0} as descricaoContaContabil, cr.IdFuncComissaoRec as IdFuncComissao, l.NomeFantasia as NomeLoja,
                    (
                	    COALESCE(((LEAST(cr.ValorVec, cr.ValorRec) * 100 / nf.TotalNota) / 100 * nf.ValorIPI), 0) +
                	    COALESCE(((LEAST(cr.ValorVec, cr.ValorRec) * 100 / nf.TotalNota) / 100 * nf.ValorICMSST), 0) +
                	    COALESCE(((LEAST(cr.ValorVec, cr.ValorRec) * 100 / SUM(nf2.TotalNota)) / 100 * SUM(nf2.ValorIPI)), 0) +
                	    COALESCE(((LEAST(cr.ValorVec, cr.ValorRec) * 100 / SUM(nf2.TotalNota)) / 100 * SUM(nf2.ValorICMSST)), 0)
                    ) as ValorImpostos,
                    (
                	    LEAST(cr.ValorVec, cr.ValorRec) - (COALESCE(((LEAST(cr.ValorVec, cr.ValorRec) * 100 / nf.TotalNota) / 100 * nf.ValorIPI), 0) +
                	    COALESCE(((LEAST(cr.ValorVec, cr.ValorRec) * 100 / nf.TotalNota) / 100 * nf.ValorICMSST), 0) +
                	    COALESCE(((LEAST(cr.ValorVec, cr.ValorRec) * 100 / SUM(nf2.TotalNota)) / 100 * SUM(nf2.ValorIPI)), 0) +
                	    COALESCE(((LEAST(cr.ValorVec, cr.ValorRec) * 100 / SUM(nf2.TotalNota)) / 100 * SUM(nf2.ValorICMSST)), 0))
                    ) as ValorBaseCalcComissao
                FROM contas_receber cr
                    LEFT JOIN cliente c ON (c.Id_Cli = cr.IdCliente)
                    LEFT JOIN comissao_contas_receber ccr ON (cr.IdContaR = ccr.IdContaR)
                    {1}
                    LEFT JOIN nota_fiscal nf ON (cr.IdNf = nf.IdNf)
                    LEFT JOIN loja l ON (cr.IdLoja = l.IdLoja)
                    LEFT JOIN pedido p ON ((p.IdSinal IS NOT NULL OR p.IdPagamentoAntecipado IS NOT NULL) AND cr.IdSinal = COALESCE(p.IdSinal, p.IdPagamentoAntecipado))
    				LEFT JOIN pedidos_nota_fiscal pnf ON (p.IdPedido = pnf.IdPedido)
    				LEFT JOIN nota_fiscal nf2 ON (pnf.IdNf = nf2.IdNf AND cr.IdNf IS NULL)
                    LEFT JOIN obra o ON (cr.IdObra = o.IdObra AND o.GerarCredito = 1)
                WHERE cr.Recebida = true AND cr.ValorRec > 0
                    AND (IsParcelaCartao IS NULL OR IsParcelaCartao=0)
                    AND o.IdObra IS NULL
                    AND IF(p.IdPedido IS NOT NULL, p.Situacao = {2} AND p.SituacaoProducao = {3} , 1)
                    {4}
                    AND cr.IdFuncComissaoRec = {5}", campoTipoContaContabil, tabelasFuncionarioComissao, (int)Pedido.SituacaoPedido.Confirmado,
                    (int)Pedido.SituacaoProducaoEnum.Entregue, paraRelatorio ? string.Empty : string.Format("AND f.Situacao={0}", (int)Situacao.Ativo), idFunc);

            if (idLoja > 0)
                sql += " AND cr.IdLoja = " + idLoja;

            if (!string.IsNullOrEmpty(dataIni))
            {
                sql += " AND cr.DataCad >= ?dataIni";
                lstParams.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));
            }

            if (!string.IsNullOrEmpty(dataFim))
            {
                sql += " AND cr.DataCad <= ?dataFim";
                lstParams.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));
            }

            if (!string.IsNullOrEmpty(dataRecIni))
            {
                sql += " AND cr.DataRec >= ?dataRecIni";
                lstParams.Add(new GDAParameter("?dataRecIni", DateTime.Parse(dataRecIni + " 00:00")));
            }

            if (!string.IsNullOrEmpty(dataRecFim))
            {
                sql += " AND cr.DataRec <= ?dataRecFim";
                lstParams.Add(new GDAParameter("?dataRecFim", DateTime.Parse(dataRecFim + " 23:59")));
            }

            if (!String.IsNullOrEmpty(tipoContaContabil))
            {
                string c;
                sql += FiltroTipoConta("cr", tipoContaContabil, out c);
            }

            if (!string.IsNullOrEmpty(idsContasR))
            {
                sql += " AND cr.IdContaR IN(" + idsContasR + ")";
            }

            if (idComissao > 0)
            {
                sql += " AND ccr.idComissao = " + idComissao;
            }
            else
            {
                sql += " AND ccr.IdComissaoContasReceber IS NULL";
            }

            sql += " GROUP BY cr.IdContaR";

            return sql;
        }

        /// <summary>
        /// Busca contas recebidas para gerar comissão
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="idLoja"></param>
        /// <param name="tipoContaContabil"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <param name="idsContasR"></param>
        /// <returns></returns>
        private IList<ContasReceber> ObtemContasRecebidasParaComissao(GDASession sessao, uint idFunc, int idLoja, string tipoContaContabil,
            string dataIni, string dataFim, string idsContasR, string dataRecIni, string dataRecFim)
        {
            if (idFunc == 0)
                throw new Exception("Nenhum funcionário foi informado.");

            var lstParams = new List<GDAParameter>();

            var sql = SqlContasRecebidasParaComissao(idFunc, idLoja, tipoContaContabil, dataIni, dataFim, idsContasR, 0, dataRecIni, dataRecFim, false, ref lstParams);

            var contas = objPersistence.LoadData(sessao, sql, lstParams.ToArray()).ToList();

            if (contas.Count == 0)
                return new List<ContasReceber>();

            var baseCalcComissao = contas.Sum(f => f.ValorBaseCalcComissao);
            var percComissao = ComissaoConfigDAO.Instance.GetComissaoPercContasRecebidas(sessao, baseCalcComissao, idFunc);

            foreach (var c in contas)
                c.ValorComissao = c.ValorBaseCalcComissao * percComissao;

            return contas;
        }

        /// <summary>
        /// Busca contas recebidas para gerar comissão
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="idLoja"></param>
        /// <param name="tipoContaContabil"></param>
        /// <param name="dataIni"></param>
        /// <param name="dataFim"></param>
        /// <returns></returns>
        public IList<ContasReceber> ObtemContasRecebidasParaComissao(GDASession sessao, uint idFunc, int idLoja, string tipoContaContabil, string dataIni, string dataFim, string dataRecIni, string dataRecFim)
        {
            return ObtemContasRecebidasParaComissao(sessao, idFunc, idLoja, tipoContaContabil, dataIni, dataFim, null, dataRecIni, dataRecFim);
        }

        /// <summary>
        /// Busca contas recebidas para gerar comissão
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="idsContasR"></param>
        /// <returns></returns>
        public IList<ContasReceber> ObtemContasRecebidasParaComissao(GDASession sessao, uint idFunc, string idsContasR)
        {
            return ObtemContasRecebidasParaComissao(sessao, idFunc, 0, null, null, null, idsContasR, null, null);
        }

        /// <summary>
        /// Buscas as contas recebidas de uma comissão
        /// </summary>
        public IList<ContasReceber> GetContasRecebidasByComissao(uint idComissao, uint idFunc, bool paraRelatorio)
        {
            if (idComissao == 0 || idFunc == 0)
                return new List<ContasReceber>();

            var lstParams = new List<GDAParameter>();

            var sql = SqlContasRecebidasParaComissao(idFunc, 0, null, null, null, null, idComissao, null, null, paraRelatorio, ref lstParams);

            var contas = objPersistence.LoadData(sql, lstParams.ToArray()).ToList();

            if (contas.Count == 0)
                return new List<ContasReceber>();

            var baseCalcComissao = contas.Sum(f => f.ValorBaseCalcComissao);
            var percComissao = ComissaoConfigDAO.Instance.GetComissaoPercContasRecebidas(null, baseCalcComissao, idFunc);

            foreach (var c in contas)
                c.ValorComissao = c.ValorBaseCalcComissao * percComissao;

            return contas;
        }

        #endregion

        #region Busca contas recebidas para ajuste pagto_contas_receber

        /// <summary>
        /// Busca contas recebidas para ajuste dos registros da tabela pagto_contas_receber.
        /// </summary>
        public IList<ContasReceber> ObterContasRecebidasParaAjustePagtoContasReceber()
        {
            var sql = @"SELECT * FROM contas_receber cr
                    LEFT JOIN acerto a ON (COALESCE(cr.IdAcerto, cr.IdAcertoParcial)=a.IdAcerto)
                WHERE a.DataCad >= '2016-12-09 00:00:00' AND (cr.IdAcerto IS NOT NULL OR cr.IdAcertoParcial IS NOT NULL) AND cr.Recebida;";

            var contas = objPersistence.LoadData(sql).ToList();

            if (contas.Count == 0)
                return new List<ContasReceber>();

            return contas;
        }

        #endregion

        #region CT-e

        /// <summary>
        /// Verifica se o CT-e informado possui uma conta recebida.
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public bool CTeTemContaRecebida(int idCte)
        {
            return objPersistence.ExecuteSqlQueryCount(@"
                SELECT COUNT(*)
                FROM contas_receber
                WHERE recebida = 1
                    AND IdCte = " + idCte) > 0;
        }

        /// <summary>
        /// Verifica se o CT-e informado possui uma conta recebida.
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public bool CTeTemContaReceber(int idCte)
        {
            return objPersistence.ExecuteSqlQueryCount(@"
                SELECT COUNT(*)
                FROM contas_receber
                WHERE recebida = 0
                    AND IdCte = " + idCte) > 0;
        }

        #endregion

        #region Contas a receber de cartão referenciadas

        /// <summary>
        /// Atualiza o campo IdContaRRef das parcelas de cartão.
        /// </summary>
        public int AtualizarReferenciaContasCartao(GDASession session, UtilsFinanceiro.DadosRecebimento retorno, IEnumerable<int> quantidadesParcelaCartao, int numeroParcelaContaPagar, int posRecebimento, uint idContaR)
        {
            for (var j = 0; j < quantidadesParcelaCartao.ElementAtOrDefault(posRecebimento); j++)
            {
                if (retorno.idParcCartao.Count > 0 && retorno.idParcCartao.Count() > numeroParcelaContaPagar)
                {
                    objPersistence.ExecuteCommand(session, string.Format("UPDATE contas_receber SET IdContaRRef={0} WHERE IdContaR={1}", idContaR, retorno.idParcCartao[numeroParcelaContaPagar]));
                    numeroParcelaContaPagar++;
                }
            }

            return numeroParcelaContaPagar;
        }

        #endregion

        #region CRUD

        /// <summary>
        /// Atualiza a conta a receber.
        /// </summary>
        /// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
        /// <returns>Número de linhas afetadas.</returns>
        public override int Update(ContasReceber objUpdate)
        {
            return Update(null, objUpdate);
        }

        /// <summary>
        /// Atualiza a conta a receber.
        /// </summary>
        /// <param name="transaction">Sessão utilizada para a execução do comando.</param>
        /// <param name="objUpdate">Objeto com os dados a serem atualizados.</param>
        /// <returns>Número de linhas afetadas.</returns>
        public override int Update(GDASession transaction, ContasReceber objUpdate)
        {
            var atualRecebida = ObtemValorCampo<bool>(transaction, "recebida", "idContaR=" + objUpdate.IdContaR);

            if (!atualRecebida
                && objUpdate.Recebida
                && objUpdate.UsuRec == null
                && !objUpdate.IsParcelaCartao
                && !objUpdate.Renegociada
                && objUpdate.ValorRec == 0)
            {
                throw new InvalidOperationException("Não é possível efetuar um recebimento sem um usuário referenciado ou valor zerado.");
            }

            if (atualRecebida && !objUpdate.Recebida)
            {
                objUpdate.DestinoRec = null;
            }

            return base.Update(transaction, objUpdate);
        }

        #endregion

    }
}
