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
    public sealed class LiberarPedidoDAO : BaseDAO<LiberarPedido, LiberarPedidoDAO>
    {
        //private LiberarPedidoDAO() { }
        
        #region Listagem de Libera��es

        private string Sql(uint idLiberarPedido, uint idPedido, int? numeroNfe, uint idFunc, uint idCli, string nomeCli,
            int liberacaoNf, string dataIni, string dataFim, int situacao, uint idLoja, string dataIniCanc, string dataFimCanc,bool selecionar, out bool temFiltro)
        {
            return Sql(null, idLiberarPedido, idPedido, numeroNfe, idFunc, idCli, nomeCli, liberacaoNf, dataIni, dataFim,
                situacao, idLoja, dataIniCanc, dataFimCanc, selecionar, out temFiltro);
        }

        private string Sql(GDASession session, uint idLiberarPedido, uint idPedido, int? numeroNfe, uint idFunc, uint idCli, string nomeCli,
            int liberacaoNf, string dataIni, string dataFim, int situacao, uint idLoja, string dataIniCanc, string dataFimCanc, bool selecionar, out bool temFiltro)
        {
            temFiltro = false;

            var nomeCliente = Liberacao.DadosLiberacao.UsarRelatorioLiberacao4Vias ? "c.Nome" : ClienteDAO.Instance.GetNomeCliente("c");
            var campos = selecionar ? "lp.*, " + nomeCliente + @" as NomeCliente, c.NomeFantasia as NomeClienteFantasia, f.Nome as NomeFunc, " +
                (Liberacao.Impostos.CalcularIcmsLiberacao ? "(" + SqlIcms("lp.idLiberarPedido") + ") as ValorIcms, " : "") + "(select cast(group_concat(distinct if(plp1.idFormaPagto=" +
                (uint)Glass.Data.Model.Pagto.FormaPagto.Obra + ", 'Obra', if(plp1.idFormaPagto=" + (uint)Glass.Data.Model.Pagto.FormaPagto.Credito + 
                ", 'Cr�dito', fp.descricao)) SEPARATOR ', ') as char) as DescrFormaPagto from pagto_liberar_pedido plp1 " +
                @"Left Join formapagto fp on (plp1.idFormaPagto=fp.idFormaPagto) where plp1.idLiberarPedido=lp.idLiberarPedido) as DescrFormaPagto, 
                (select count(*) from rota_cliente where idCliente=c.id_Cli)>0 as isClienteRota, c.Endereco, c.Numero, c.Bairro, c.IdCidade, c.Cep, c.Compl,
                c.Tel_Cont as Telefone, '$$$' as criterio" : "Count(*)";

            var criterio = String.Empty;

            var sql = "Select " + campos + @" From liberarpedido lp 
                Left Join cliente c On (lp.idCliente=c.id_Cli) 
                Left Join funcionario f On (lp.IdFunc=f.IdFunc) 
                Where lp.Situacao <> " + (int)LiberarPedido.SituacaoLiberarPedido.Processando + " ";

            if (idLiberarPedido > 0)
            {
                sql += " And lp.IdLiberarPedido=" + idLiberarPedido;
                criterio += "Libera��o: " + idLiberarPedido + "    ";
                temFiltro = true;
            }
            
            if (idCli > 0)
            {
                sql += " And lp.IdCliente=" + idCli;
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(session, idCli) + "    ";
                temFiltro = true;
            }
            else if (!String.IsNullOrEmpty(nomeCli))
            {
                var ids = ClienteDAO.Instance.GetIds(session, null, nomeCli, null, 0, null, null, null, null, 0);
                sql += " And c.id_Cli in (" + ids + ")";
                criterio += "Cliente: " + nomeCli + "    ";
                temFiltro = true;
            }

            if (idPedido > 0)
            {
                sql += " And lp.IdLiberarPedido In (Select idLiberarPedido From produtos_liberar_pedido Where idPedido=" + idPedido + ")";
                criterio += "Pedido: " + idPedido + "    ";
                temFiltro = true;
            }

            if (numeroNfe.GetValueOrDefault() > 0)
            {
                sql += @" And lp.idLiberarPedido In (Select idLiberarPedido From pedidos_nota_fiscal Where idNf In
                    (Select idNf From nota_fiscal Where numeroNfe=" + numeroNfe + "))";
                criterio += "Nota Fiscal: " + numeroNfe + "    ";
                temFiltro = true;
            }

            if (idFunc > 0)
            {
                sql += " And lp.IdFunc=" + idFunc;
                criterio += "Liberado por: " + FuncionarioDAO.Instance.GetNome(session, idFunc) + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " And lp.dataLiberacao>=?dataIni";
                criterio += "Data In�cio: " + dataIni + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " And lp.dataLiberacao<=?dataFim";
                criterio += "Data Fim: " + dataFim + "    ";
                temFiltro = true;
            }

            if (situacao > 0)
            {
                sql += " and lp.situacao=" + situacao;
                var temp = new LiberarPedido {Situacao = situacao};
                criterio += "Situa��o: " + temp.DescrSituacao + "    ";
                temFiltro = true;
            }

            if (idLoja > 0)
            {
                sql += " AND f.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(session, idLoja);
                temFiltro = true;
            }

            if (liberacaoNf > 0)
            {
                sql += " AND lp.idLiberarPedido " + (liberacaoNf == 1 ? "" : "NOT") +
                    " IN (SELECT idLiberarPedido FROM pedidos_nota_fiscal WHERE idLiberarPedido IS NOT NULL)";
                criterio += "Apenas libera��es " + (liberacaoNf == 1 ? "com" : "sem") + " nota fiscal    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataIniCanc))
            {
                sql += " And lp.DataCanc>=?dataIniCanc";
                criterio += "Data In�cio do Cancelamento: " + dataIniCanc + "    ";
                temFiltro = true;
            }

            if (!String.IsNullOrEmpty(dataFimCanc))
            {
                sql += " And lp.DataCanc<=?dataFimCanc";
                criterio += "Data Fim do Cancelamento: " + dataFimCanc + "    ";
                temFiltro = true;
            }

            return sql.Replace("$$$", criterio);
        }

        public IList<LiberarPedido> GetList(uint idLiberarPedido, uint idPedido, int? numeroNfe, uint idFunc, uint idCli, string nomeCli,
            int liberacaoNf, string dataIni, string dataFim, int situacao, uint idLoja, string dataIniCanc, string dataFimCanc, string sortExpression, int startRow, int pageSize)
        {
            string filtro = String.IsNullOrEmpty(sortExpression) ? "lp.DataLiberacao Desc" : sortExpression;

            bool temFiltro;
            return LoadDataWithSortExpression(Sql(idLiberarPedido, idPedido, numeroNfe, idFunc, idCli, nomeCli, liberacaoNf,
                dataIni, dataFim, situacao, idLoja, dataIniCanc, dataFimCanc, true, out temFiltro), filtro, startRow, pageSize, temFiltro, GetParam(nomeCli, dataIni, dataFim, dataIniCanc, dataFimCanc));
        }

        public LiberarPedido[] GetForRpt(uint idLiberarPedido, uint idPedido, int? numeroNfe, uint idFunc, uint idCli, string nomeCli,
            int liberacaoNf, string dataIni, string dataFim, int situacao, uint idLoja, string dataIniCanc, string dataFimCanc)
        {
            bool temFiltro;
            return objPersistence.LoadData(Sql(idLiberarPedido, idPedido, numeroNfe, idFunc, idCli, nomeCli, liberacaoNf,
                dataIni, dataFim, situacao, idLoja, dataIniCanc, dataFimCanc, true, out temFiltro) + " Order By lp.DataLiberacao Desc",
                GetParam(nomeCli, dataIni, dataFim, dataIniCanc, dataFimCanc)).ToArray();
        }

        public int GetCount(uint idLiberarPedido, uint idPedido, int? numeroNfe, uint idFunc, uint idCli, string nomeCli,
            int liberacaoNf, string dataIni, string dataFim, int situacao, uint idLoja, string dataIniCanc, string dataFimCanc)
        {
            bool temFiltro;
            return GetCountWithInfoPaging(Sql(idLiberarPedido, idPedido, numeroNfe, idFunc, idCli, nomeCli, liberacaoNf,
                dataIni, dataFim, situacao, idLoja, dataIniCanc, dataFimCanc, true, out temFiltro), temFiltro, null, GetParam(nomeCli, dataIni, dataFim, dataIniCanc, dataFimCanc));
        }

        public LiberarPedido GetElement(uint idLiberarPedido)
        {
            return GetElement(null, idLiberarPedido);
        }

        public LiberarPedido GetElement(GDASession session, uint idLiberarPedido)
        {
            bool temFiltro;
            LiberarPedido liberar = objPersistence.LoadOneData(session, Sql(session, idLiberarPedido, 0, null, 0, 0, null, 0, null, null, 0, 0, null, null, true, out temFiltro));
            liberar = liberar ?? new LiberarPedido();

            string descrFormasPagto = "";
            bool formaPagtoParc = false; // Identifica se a forma de pagto de alguma parcela foi informada.
            string entrada = "";

            #region Pega parcelas

            if (liberar.TipoPagto == (int)LiberarPedido.TipoPagtoEnum.AVista)
            {
                var formasPagto = PagtoLiberarPedidoDAO.Instance.GetByLiberacao(session, idLiberarPedido).ToArray();
                foreach (var plp in formasPagto)
                    if (plp.DescrFormaPagto.Trim() != "")
                    {
                        descrFormasPagto += ", " + plp.DescrFormaPagto.Trim() + " " + plp.ValorPagto.ToString("C");
                        formaPagtoParc = !String.IsNullOrEmpty(plp.DescrFormaPagto);
                    }
            }
            else if (/* Chamado 45471. */liberar.TipoPagto == 0 || liberar.TipoPagto == (int)LiberarPedido.TipoPagtoEnum.APrazo)
            {
                bool exibirDescrParc = liberar.IdParcela > 0 && FinanceiroConfig.DadosLiberacao.ExibirDescricaoParcelaLiberacao;

                var parcelas = ContasReceberDAO.Instance.GetByLiberacaoPedido(session, idLiberarPedido, true).ToArray();

                if (!exibirDescrParc && parcelas.Length > 0) // Se for pagamento � prazo cheque, n�o tem parcelas geradas
                    descrFormasPagto = "  " + parcelas.Length + " parcela(s): ";
                else if (liberar.IdParcela != null)
                    descrFormasPagto = "  " + ParcelasDAO.Instance.ObtemValorCampo<string>(session, "descricao", "idParcela=" + liberar.IdParcela);

                var descrParcelas = "";
                if (!exibirDescrParc)
                    for (int i = 0; i < parcelas.Length; i++)
                        descrParcelas += ", " + (i + 1) + "� " + parcelas[i].ValorVec.ToString("C") + " / " + parcelas[i].DataVec.ToString("dd/MM/yyyy");

                descrFormasPagto += descrParcelas != "" ? descrParcelas.Substring(1) : "";

                var pagtos = PagtoLiberarPedidoDAO.Instance.GetByLiberacao(session, idLiberarPedido).ToArray();

                if (pagtos.Length > 1)
                {
                    for (int i = 1; i < pagtos.Length; i++)
                        entrada += ", " + pagtos[i].DescrFormaPagto + " " + pagtos[i].ValorPagto.ToString("C");

                    entrada = entrada != "" ? "Entrada: " + entrada.Substring(2) + ", " : "";
                }

                decimal credito = liberar.ValorCreditoEntrada;
                //if (credito > 0)
                /* Chamado 47929. */
                if (credito > 0 && !entrada.Contains("Cr�dito"))
                    entrada += (entrada == "" ? "Entrada: " : "") + "Cr�dito " + credito.ToString("C") + ", ";
            }

            liberar.DescricaoPagto = entrada + liberar.DescrTipoPagto +
                (!formaPagtoParc && !String.IsNullOrEmpty(liberar.DescrFormaPagto) && (liberar.DescrTipoPagto.IndexOf(liberar.DescrFormaPagto, StringComparison.Ordinal) == -1 || liberar.DescrFormaPagto == null) ? " - " + liberar.DescrFormaPagto : "") + 
                (descrFormasPagto != "" ? " - " + descrFormasPagto.Substring(2) : "");

            // Se na descri��o da parcela possuir a descri��o "na entrega", exibe s� esta informa��o no campo de parcelas
            if (descrFormasPagto.ToLower().Contains("na entrega"))
                liberar.DescricaoPagto = descrFormasPagto.Trim();

            #endregion

            liberar.IsLiberacaoParcial = IsLiberacaoParcial(idLiberarPedido);
            return liberar;
        }

        private GDAParameter[] GetParam(string nomeCli, string dataIni, string dataFim, string dataIniCanc, string dataFimCanc)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeCli))
                lstParam.Add(new GDAParameter("?nomeCli", "%" + nomeCli + "%"));

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", (dataIni.Length == 10 ? DateTime.Parse(dataIni + " 00:00") : DateTime.Parse(dataIni))));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", (dataFim.Length == 10 ? DateTime.Parse(dataFim + " 23:59:59") : DateTime.Parse(dataFim))));

            if (!String.IsNullOrEmpty(dataIniCanc))
                lstParam.Add(new GDAParameter("?dataIniCanc", (dataIniCanc.Length == 10 ? DateTime.Parse(dataIniCanc + " 00:00") : DateTime.Parse(dataIniCanc))));

            if (!String.IsNullOrEmpty(dataFimCanc))
                lstParam.Add(new GDAParameter("?dataFimCanc", (dataFimCanc.Length == 10 ? DateTime.Parse(dataFimCanc + " 23:59:59") : DateTime.Parse(dataFimCanc))));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        public IList<LiberarPedido> GetByString(string idsLiberarPedido)
        {
            if (idsLiberarPedido == null || String.IsNullOrEmpty(idsLiberarPedido.Trim(',')))
                return new List<LiberarPedido>();

            bool temFiltro;
            string sql = Sql(0, 0, null, 0, 0, null, 0, null, null, 0, 0,null,null, true, out temFiltro);

            sql += " and lp.idLiberarPedido in (" + idsLiberarPedido.Trim(',') + ")";

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Libera��o de pedido � vista

        /// <summary>
        /// Cria a libera��o � vista, de um ou mais pedidos.
        /// </summary>
        public int CriarLiberacaoAVista(decimal acrescimo, bool caixaDiario, decimal creditoUtilizado, IEnumerable<string> dadosChequesRecebimento, bool descontarComissao, decimal desconto,
            bool gerarCredito, int idCliente, IEnumerable<int> idsCartaoNaoIdentificado, IEnumerable<int> idsContaBanco, IEnumerable<int> idsDepositoNaoIdentificado, IEnumerable<int> idsFormaPagamento,
            IEnumerable<int> idsPedido, IEnumerable<int> idsProdutoPedido, IEnumerable<int> idsProdutoPedidoProducao, IEnumerable<int> idsTipoCartao, IEnumerable<string> numerosAutorizacaoCartao,
            string numeroAutorizacaoConstrucard, IEnumerable<float> quantidadesLiberar, IEnumerable<int> quantidadesParcelaCartao, int tipoAcrescimo, int tipoDesconto, decimal totalPagar,
            bool utilizarCredito, IEnumerable<decimal> valoresPagos, decimal valorUtilizadoObra)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    // Cria a libera��o de pedidos � vista.
                    var idLiberarPedido = CriarPreLiberacaoAVista(transaction, acrescimo, caixaDiario, creditoUtilizado, dadosChequesRecebimento, descontarComissao, desconto, gerarCredito, idCliente,
                        idsCartaoNaoIdentificado, idsContaBanco, idsDepositoNaoIdentificado, idsFormaPagamento, idsPedido, idsProdutoPedido, idsProdutoPedidoProducao, idsTipoCartao,
                        numerosAutorizacaoCartao, numeroAutorizacaoConstrucard, quantidadesLiberar, quantidadesParcelaCartao, tipoAcrescimo, tipoDesconto, totalPagar, utilizarCredito, valoresPagos,
                        valorUtilizadoObra);

                    // Finaliza a libera��o criada acima, gerando movimenta��o no caixa, conta banc�ria, estoque etc.
                    FinalizarPreLiberacaoAVista(transaction, idLiberarPedido);

                    transaction.Commit();
                    transaction.Close();

                    return idLiberarPedido;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("CriarLiberacaoAVista - IDs pedido: {0}.", idsPedido), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao criar libera��o do(s) pedido(s).", ex));
                }
            }
        }

        /// <summary>
        /// Cria a pr� libera��o de pedido � vista, efetuando todas as valida��es necess�rias.
        /// </summary>
        public int CriarPreLiberacaoAVistaComTransacao(decimal acrescimo, bool caixaDiario, decimal creditoUtilizado, IEnumerable<string> dadosChequesRecebimento, bool descontarComissao, decimal desconto,
            bool gerarCredito, int idCliente, IEnumerable<int> idsCartaoNaoIdentificado, IEnumerable<int> idsContaBanco, IEnumerable<int> idsDepositoNaoIdentificado, IEnumerable<int> idsFormaPagamento,
            IEnumerable<int> idsPedido, IEnumerable<int> idsProdutoPedido, IEnumerable<int> idsProdutoPedidoProducao, IEnumerable<int> idsTipoCartao, IEnumerable<string> numerosAutorizacaoCartao,
            string numeroAutorizacaoConstrucard, IEnumerable<float> quantidadesLiberar, IEnumerable<int> quantidadesParcelaCartao, int tipoAcrescimo, int tipoDesconto, decimal totalPagar,
            bool utilizarCredito, IEnumerable<decimal> valoresPagos, decimal valorUtilizadoObra)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    // Cria a libera��o de pedidos � vista.
                    var idLiberarPedido = CriarPreLiberacaoAVista(transaction, acrescimo, caixaDiario, creditoUtilizado, dadosChequesRecebimento, descontarComissao, desconto, gerarCredito, idCliente,
                        idsCartaoNaoIdentificado, idsContaBanco, idsDepositoNaoIdentificado, idsFormaPagamento, idsPedido, idsProdutoPedido, idsProdutoPedidoProducao, idsTipoCartao,
                        numerosAutorizacaoCartao, numeroAutorizacaoConstrucard, quantidadesLiberar, quantidadesParcelaCartao, tipoAcrescimo, tipoDesconto, totalPagar, utilizarCredito, valoresPagos,
                        valorUtilizadoObra);

                    TransacaoCapptaTefDAO.Instance.Insert(transaction, new TransacaoCapptaTef()
                    {
                        IdReferencia = idLiberarPedido,
                        TipoRecebimento = UtilsFinanceiro.TipoReceb.LiberacaoAVista
                    });

                    transaction.Commit();
                    transaction.Close();

                    return idLiberarPedido;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("CriarPreLiberacaoAVista - IDs pedido: {0}.", idsPedido), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao criar pr� libera��o do(s) pedido(s).", ex));
                }
            }
        }

        /// <summary>
        /// Cria a pr� libera��o de pedido � vista, efetuando todas as valida��es necess�rias.
        /// </summary>
        private int CriarPreLiberacaoAVista(GDASession session, decimal acrescimo, bool caixaDiario, decimal creditoUtilizado, IEnumerable<string> dadosChequesRecebimento, bool descontarComissao,
            decimal desconto, bool gerarCredito, int idCliente, IEnumerable<int> idsCartaoNaoIdentificado, IEnumerable<int> idsContaBanco, IEnumerable<int> idsDepositoNaoIdentificado,
            IEnumerable<int> idsFormaPagamento, IEnumerable<int> idsPedido, IEnumerable<int> idsProdutoPedido, IEnumerable<int> idsProdutoPedidoProducao, IEnumerable<int> idsTipoCartao,
            IEnumerable<string> numerosAutorizacaoCartao, string numeroAutorizacaoConstrucard, IEnumerable<float> quantidadesLiberar, IEnumerable<int> quantidadesParcelaCartao, int tipoAcrescimo,
            int tipoDesconto, decimal totalPagar, bool utilizarCredito, IEnumerable<decimal> valoresPagos, decimal valorUtilizadoObra)
        {
            #region Declara��o de vari�veis

            var usuarioLogado = UserInfo.GetUserInfo;
            var idLoja = 0;
            var idComissionado = 0;
            var contadorPagamento = 0;
            decimal totalPago = 0;
            decimal acrescimoAplicar = 0;
            decimal descontoAplicar = 0;

            #endregion

            #region Recupera��o da loja

            // Recupera a loja do primeiro pedido liberado.
            // Caso a empresa trabalhe com comiss�o de contas recebidas ou a loja do cliente tenha que ser considerada no fluxo do sistema, considera a loja do pedido,
            // sen�o, considera a loja do funcion�rio que est� liberando os pedidos.
            idLoja = Configuracoes.ComissaoConfig.ComissaoPorContasRecebidas || Geral.ConsiderarLojaClientePedidoFluxoSistema ?
                ((int)PedidoDAO.Instance.ObtemIdLoja(session, idsPedido != null && idsPedido.Count() > 0 ? (uint)idsPedido.ElementAt(0) : 0)) : (int)usuarioLogado.IdLoja;

            #endregion

            #region C�lculo dos totais da libera��o

            if (totalPagar != 0)
            {
                descontoAplicar = tipoDesconto == 1 ? desconto : desconto / totalPagar * 100;
                acrescimoAplicar = tipoAcrescimo == 1 ? acrescimo : acrescimo / totalPagar * 100;
                totalPagar = Math.Round(totalPagar * ((100 - descontoAplicar + acrescimoAplicar) / 100), 2);
            }
            else
            {
                // Se o total a ser pago for 0 (por ter pago antecipado por exemplo), ir� somar o acr�scimo e subtrair o desconto direto no
                // total a pagar desde que ambos sejam calculados por R$ e n�o por %, neste �ltimo caso, nada ser� calculado.
                descontoAplicar = tipoDesconto == 1 ? 0 : desconto;
                acrescimoAplicar = tipoAcrescimo == 1 ? 0 : acrescimo;
                totalPagar = acrescimoAplicar - descontoAplicar;
            }

            totalPago = valoresPagos.Sum(f => f);
            totalPagar -= valorUtilizadoObra;

            // Se for pago com cr�dito, soma o mesmo ao totalPago.
            if (creditoUtilizado > 0)
            {
                totalPago += creditoUtilizado;
            }

            if (descontarComissao)
            {
                totalPago += UtilsFinanceiro.GetValorComissao(session, string.Join(",", idsPedido), "Pedido");
            }

            // Se o total a ser pago for menor que 0, gera cr�dito sobre esse valor.
            if (totalPagar < 0)
            {
                totalPago -= totalPagar;
                totalPagar = 0;
                gerarCredito = true;
            }

            // Ignora os juros dos cart�es ao calcular o valor pago/a pagar.
            totalPago -= UtilsFinanceiro.GetJurosCartoes(session, (uint)idLoja, valoresPagos.ToArray(), idsFormaPagamento.Select(f => (uint)f).ToArray(),
                idsTipoCartao.Select(f => (uint)f).ToArray(), quantidadesParcelaCartao.Select(f => (uint)f).ToArray());

            #endregion

            #region Recupera��o do comissionado

            if (descontarComissao)
            {
                var pedidosParaComissao = UtilsFinanceiro.GetPedidosForComissao(session, string.Join(",", idsPedido), "Pedido");
                idComissionado = pedidosParaComissao != null && pedidosParaComissao.Count() > 0 ? (int)pedidosParaComissao[0].IdComissionado : 0;
            }

            #endregion

            #region Valida��o da libera��o

            // Verifica se a libera��o de pedidos pode ser efetuada.
            ValidarLiberarPedidoAVista(session, caixaDiario, gerarCredito, idCliente, idComissionado, idLoja, idsCartaoNaoIdentificado, idsContaBanco, idsFormaPagamento, idsPedido, idsProdutoPedido,
                quantidadesLiberar, totalPagar, totalPago, valoresPagos, valorUtilizadoObra);

            #endregion

            #region Cria��o da libera��o

            // Cadastra a libera��o antes da sess�o e na situa��o cancelada para resolver a seguinte situa��o:
            // Durante o processamento desta libera��o a pessoa pode imprimir a mesma por outra tela, o problema � que 
            // caso ocorra algum problema, a transa��o vai desfazer tudo, quando for feita uma nova libera��o, 
            // ela vai pegar o n�mero dessa, fazendo com que pare�a existir duas libera��es diferentes com o mesmo n�mero
            var liberarPedido = new LiberarPedido
            {
                IdCliente = (uint)idCliente,
                IdLojaRecebimento = idLoja,
                Situacao = (int)LiberarPedido.SituacaoLiberarPedido.Processando,
                ValorCreditoAoLiberar = ClienteDAO.Instance.GetCredito(session, (uint)idCliente),
                IdFunc = usuarioLogado.CodUser,
                TipoPagto = (int)LiberarPedido.TipoPagtoEnum.AVista,
                NumAutConstrucard = numeroAutorizacaoConstrucard,
                DataLiberacao = DateTime.Now,
                Total = totalPagar + valorUtilizadoObra,
                TipoDesconto = tipoDesconto,
                Desconto = desconto,
                TipoAcrescimo = tipoAcrescimo,
                Acrescimo = acrescimo,
                TotalPagar = totalPagar,
                TotalPago = totalPago,
                CreditoUtilizado = creditoUtilizado,
                DescontarComissao = descontarComissao,
                RecebimentoCaixaDiario = caixaDiario,
                RecebimentoGerarCredito = gerarCredito
            };

            liberarPedido.IdLiberarPedido = Insert(session, liberarPedido);

            #endregion

            #region Gera��o dos produtos da libera��o de pedidos

            // Deve ser feito antes de chamar o m�todo de recebimento, pois este v�nculo � usado l�
            for (var i = 0; i < idsProdutoPedido.Count(); i++)
            {
                var idProdPedProducao = idsProdutoPedidoProducao.ElementAtOrDefault(i) > 0 ? (uint)idsProdutoPedidoProducao.ElementAt(i) : (uint?)null;

                var produtoLiberarPedido = new ProdutosLiberarPedido
                {
                    IdLiberarPedido = liberarPedido.IdLiberarPedido,
                    IdPedido = ProdutosPedidoDAO.Instance.ObtemIdPedido(session, (uint)idsProdutoPedido.ElementAtOrDefault(i)),
                    IdProdPed = (uint)idsProdutoPedido.ElementAtOrDefault(i),
                    Qtde = quantidadesLiberar.ElementAtOrDefault(i),
                    QtdeCalc = quantidadesLiberar.ElementAtOrDefault(i),
                    IdProdPedProducao = idProdPedProducao,
                    ValorIcms = ProdutosLiberarPedidoDAO.Instance.GetValorIcmsForLiberacao(session, (uint)idCliente, (uint)idsProdutoPedido.ElementAtOrDefault(i),
                        quantidadesLiberar.ElementAtOrDefault(i)),
                    ValorIpi = ProdutosLiberarPedidoDAO.Instance.GetValorIpiForLiberacao(session, (uint)idCliente, (uint)idsProdutoPedido.ElementAtOrDefault(i),
                        quantidadesLiberar.ElementAtOrDefault(i))
                };

                ProdutosLiberarPedidoDAO.Instance.Insert(session, produtoLiberarPedido);
            }

            #endregion            

            #region Cadastro das formas de pagamento

            // Cadastro dos cheques utilizados no pagamento da libera��o.
            ChequesLiberarPedidoDAO.Instance.InserirPelaString(session, liberarPedido, dadosChequesRecebimento);
            // Garante que n�o haver� chaves duplicadas para esta libera��o.
            PagtoLiberarPedidoDAO.Instance.DeleteByLiberacao(session, liberarPedido.IdLiberarPedido);

            for (var i = 0; i < idsFormaPagamento.Count(); i++)
            {
                if (idsFormaPagamento.ElementAtOrDefault(i) == 0 || valoresPagos.ElementAtOrDefault(i) == 0)
                {
                    continue;
                }

                if (idsFormaPagamento.Count() > i && idsFormaPagamento.ElementAt(i) == (int)Pagto.FormaPagto.CartaoNaoIdentificado)
                {
                    var cartoesNaoIdentificado = CartaoNaoIdentificadoDAO.Instance.ObterPeloId(session, idsCartaoNaoIdentificado.Select(f => (uint)f).ToArray());

                    foreach (var cartaoNaoIdentificado in cartoesNaoIdentificado)
                    {
                        var pagamentoLiberarPedido = new PagtoLiberarPedido
                        {
                            IdLiberarPedido = liberarPedido.IdLiberarPedido,
                            NumFormaPagto = ++contadorPagamento,
                            IdFormaPagto = (uint)idsFormaPagamento.ElementAt(i),
                            IdContaBanco = idsContaBanco.ElementAtOrDefault(i) > 0 ? idsContaBanco.ElementAt(i) : (int?)null,
                            IdCartaoNaoIdentificado = cartaoNaoIdentificado.IdCartaoNaoIdentificado,
                            IdTipoCartao = (uint)cartaoNaoIdentificado.TipoCartao,
                            ValorPagto = cartaoNaoIdentificado.Valor,
                            NumAutCartao = cartaoNaoIdentificado.NumAutCartao
                        };

                        PagtoLiberarPedidoDAO.Instance.Insert(session, pagamentoLiberarPedido);
                    }
                }
                else
                {
                    var pagamentoLiberarPedido = new PagtoLiberarPedido
                    {
                        IdLiberarPedido = liberarPedido.IdLiberarPedido,
                        NumFormaPagto = ++contadorPagamento,
                        IdFormaPagto = (uint)idsFormaPagamento.ElementAt(i),
                        IdContaBanco = idsContaBanco.ElementAtOrDefault(i) > 0 ? idsContaBanco.ElementAt(i) : (int?)null,
                        IdDepositoNaoIdentificado = idsDepositoNaoIdentificado.ElementAtOrDefault(i) > 0 ? idsDepositoNaoIdentificado.ElementAt(i) : (int?)null,
                        IdTipoCartao = idsTipoCartao.ElementAtOrDefault(i) > 0 ? (uint)idsTipoCartao.ElementAt(i) : (uint?)null,
                        ValorPagto = valoresPagos.ElementAtOrDefault(i),
                        NumAutCartao = !string.IsNullOrWhiteSpace(numerosAutorizacaoCartao.ElementAtOrDefault(i)) ? numerosAutorizacaoCartao.ElementAt(i) : null
                    };

                    PagtoLiberarPedidoDAO.Instance.Insert(session, pagamentoLiberarPedido);
                }
            }

            #region Recebimento com obra

            if (valorUtilizadoObra > 0)
            {
                var novo = new PagtoLiberarPedido
                {
                    IdLiberarPedido = liberarPedido.IdLiberarPedido,
                    NumFormaPagto = idsFormaPagamento.Count() + 1,
                    IdFormaPagto = (uint)Pagto.FormaPagto.Obra,
                    IdTipoCartao = null,
                    ValorPagto = valorUtilizadoObra
                };

                PagtoLiberarPedidoDAO.Instance.Insert(session, novo);
            }

            #endregion

            #region Recebimento com cr�dito

            if (creditoUtilizado > 0)
            {
                var novo = new PagtoLiberarPedido
                {
                    IdLiberarPedido = liberarPedido.IdLiberarPedido,
                    NumFormaPagto = ++contadorPagamento,
                    IdFormaPagto = (uint)Pagto.FormaPagto.Credito,
                    IdTipoCartao = null,
                    ValorPagto = creditoUtilizado
                };

                PagtoLiberarPedidoDAO.Instance.Insert(session, novo);
            }

            #endregion

            #endregion

            #region Atualiza��o da formas de pagamento dos pedidos

            // Atualiza as formas de pagamento dos pedidos somente se pelo menos uma forma de pagamento tiver sido informada.
            if (idsFormaPagamento.Any(f => f > 0))
            {
                string sqlFormaPagto;

                // Atualiza forma de pagamento de acordo com aquela que foi escolhida pelo caixa, a menos que seja dinheiro, pois neste caso
                // pedidos � vista n�o possuem forma de pagamento e pedidos � prazo n�o podem ter a forma pagto dinheiro.
                if (idsFormaPagamento.ElementAtOrDefault(0) != (int)Pagto.FormaPagto.Dinheiro)
                {
                    sqlFormaPagto = string.Format("UPDATE pedido SET IdFormaPagto={0}", idsFormaPagamento.ElementAt(0));

                    if (idsFormaPagamento.Count() > 1 && idsFormaPagamento.ElementAt(1) > 0)
                    {
                        sqlFormaPagto += string.Format(", IdFormaPagto2={0}", idsFormaPagamento.ElementAt(1));
                    }

                    objPersistence.ExecuteCommand(session, string.Format("{0} WHERE TipoVenda={1} AND IdPedido IN ({2})", sqlFormaPagto, (int)Pedido.TipoVendaPedido.APrazo, string.Join(",", idsPedido)));
                }

                // Atualiza tipo de cart�o de acordo com aquele que foi escolhido pelo caixa.
                if ((uint)Pagto.FormaPagto.Cartao == idsFormaPagamento.ElementAt(0))
                {
                    objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido SET IdTipoCartao={0} WHERE IdPedido IN ({1});", idsTipoCartao.ElementAt(0), string.Join(",", idsPedido)));
                }

                if (idsFormaPagamento.Count() > 1 && (uint)Pagto.FormaPagto.Cartao == idsFormaPagamento.ElementAt(1))
                {
                    objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido SET IdTipoCartao2={0} WHERE IdPedido IN ({1});", idsTipoCartao.ElementAt(1), string.Join(",", idsPedido)));
                }
            }

            #endregion

            #region Atualiza��o dos dados do pedido

            foreach (var idPedido in idsPedido)
            {
                #region Declara��o de vari�veis

                var entrada = PedidoDAO.Instance.ObtemValorEntrada(session, (uint)idPedido);
                var idSinal = (int)PedidoDAO.Instance.ObtemIdSinal(session, (uint)idPedido).GetValueOrDefault();
                var idPagamentoAntecipado = (int)PedidoDAO.Instance.ObtemIdPagamentoAntecipado(session, (uint)idPedido).GetValueOrDefault();
                var pedidoTemSinalNaoPago = entrada > 0 && idSinal == 0 && idPagamentoAntecipado == 0;
                var pedidoEstaLiberado = IsPedidoLiberado(session, (uint)idPedido);
                var situacao = (!Liberacao.DadosLiberacao.LiberarPedidoProdutos && !Liberacao.DadosLiberacao.LiberarPedidoAtrasadoParcialmente) || pedidoEstaLiberado ?
                    Pedido.SituacaoPedido.Confirmado : Pedido.SituacaoPedido.LiberadoParcialmente;
                var dataEntregaPedido = PedidoDAO.Instance.ObtemDataEntrega(session, (uint)idPedido);

                #endregion

                #region Atualiza��o dos dados do pedido

                if (pedidoTemSinalNaoPago)
                {
                    objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido SET ValorEntrada = NULL WHERE IdPedido = {0}", (uint)idPedido));
                }

                PedidoDAO.Instance.AlteraSituacao(session, (uint)idPedido, situacao);

                #endregion

                #region Inser��o do log de altera��o do pedido

                // Salva um loga da altera��o da situa��o do pedido.
                var logData = new LogAlteracao();
                logData.Tabela = (int)LogAlteracao.TabelaAlteracao.Pedido;
                logData.IdRegistroAlt = idPedido;
                logData.NumEvento = LogAlteracaoDAO.Instance.GetNumEvento(session, LogAlteracao.TabelaAlteracao.Pedido, idPedido);
                logData.Campo = "Situac�o";
                logData.DataAlt = DateTime.Now;
                logData.IdFuncAlt = usuarioLogado.CodUser;
                logData.ValorAnterior = "Confirmado PCP";
                logData.ValorAtual = "Liberado";
                logData.Referencia = LogAlteracao.GetReferencia(session, (int)LogAlteracao.TabelaAlteracao.Pedido, (uint)idPedido);

                LogAlteracaoDAO.Instance.Insert(session, logData);

                #endregion

                #region Gera��o da instala��o do pedido

                // Gera instala��o para o pedido.
                PedidoDAO.Instance.GerarInstalacao(session, (uint)idPedido, dataEntregaPedido);

                #endregion
            }

            #region Atualiza��o de estoque

            // Atualiza o estoque dos produtos antes da finaliza��o da pr� libera��o, para evitar que outra libera��o seja feita com os mesmos produtos e ocorra bloqueio de estoque ao finalizar as libera��es.
            AtualizaEstoque(session, liberarPedido.IdLiberarPedido, (uint)idCliente, string.Join(",", idsPedido), idsProdutoPedido.Select(f => (uint)f).ToArray(), quantidadesLiberar.ToArray(), tipoAcrescimo);

            #endregion

            // Atualiza a data da �ltima compra do cliente.
            ClienteDAO.Instance.AtualizaUltimaCompra(session, (uint)idCliente);

            // Atualiza a data da �ltima libera��o dos pedidos desta libera��o, para exibir corretamente no pedido
            objPersistence.ExecuteCommand(session, string.Format("UPDATE PEDIDO SET IdLiberarPedido={0}, NumAutConstrucard=?numAutConst WHERE IdPedido IN ({1})",
                liberarPedido.IdLiberarPedido, string.Join(",", idsPedido)), new GDAParameter("?numAutConst", numeroAutorizacaoConstrucard));

            #endregion

            #region Altera��o da situa��o, dos pedidos de revenda, para entregue

            if (FinanceiroConfig.DadosLiberacao.MarcaPedidoRevendaEntregueAoLiberar && Liberacao.Estoque.SaidaEstoqueBoxLiberar && PedidoConfig.DadosPedido.BloquearItensTipoPedido)
            {
                foreach (var idPedido in idsPedido.Where(f => f > 0))
                {
                    var situacaoPedido = PedidoDAO.Instance.ObtemSituacao(session, (uint)idPedido);
                    var tipoPedido = PedidoDAO.Instance.ObtemValorCampo<int>(session, "TipoPedido", string.Format("IdPedido={0}", idPedido));

                    if (situacaoPedido == Pedido.SituacaoPedido.Confirmado && tipoPedido == (int)Pedido.TipoPedidoEnum.Revenda)
                    {
                        PedidoDAO.Instance.AlteraSituacaoProducao(session, (uint)idPedido, Pedido.SituacaoProducaoEnum.Entregue);
                    }
                }
            }

            #endregion

            return (int)liberarPedido.IdLiberarPedido;
        }

        /// <summary>
        /// Valida a cria��o da libera��o do(s) pedido(s).
        /// </summary>
        private void ValidarLiberarPedidoAVista(GDASession session, bool caixaDiario, bool gerarCredito, int idCliente, int idComissionado, int idLoja, IEnumerable<int> idsCartaoNaoIdentificado,
            IEnumerable<int> idsContaBanco, IEnumerable<int> idsFormaPagamento, IEnumerable<int> idsPedido, IEnumerable<int> idsProdutoPedido, IEnumerable<float> quantidadesLiberar,
            decimal totalPagar, decimal totalPago, IEnumerable<decimal> valoresPagos, decimal valorUtilizadoObra)
        {
            #region Declara��o de vari�veis

            // Vari�vel criada para retornar a mensagem de valida��o de sinal e pagamento antecipado dos pedidos.
            var mensagemSinalPagamentoAntecipado = string.Empty;
            // Vari�vel criada para retornar a mensagem de valida��o de estoque dos pedidos.
            var mensagemEstoque = string.Empty;
            var pedidosPossuemSinalPagamentoAntecipadoAReceber = !PedidoDAO.Instance.VerificaSinalPagamentoReceber(session, string.Join(",", idsPedido), out mensagemSinalPagamentoAntecipado);
            var pedidosPossuemEstoque = PedidoPossuiEstoque(session, idsProdutoPedido.Select(f => (uint)f).ToArray(), quantidadesLiberar.ToArray(), out mensagemEstoque);
            var indiceCheques = UtilsFinanceiro.IndexFormaPagto(Pagto.FormaPagto.ChequeProprio, idsFormaPagamento.Select(f => (uint)f).ToArray());
            var idSetorEntregue = (int)Utils.ObtemIdSetorEntregue().GetValueOrDefault();
            var idsPedidoLiberadosParcialmente = ExecuteMultipleScalar<int>(session, string.Format("SELECT COUNT(*) FROM pedido WHERE IdPedido IN ({0}) AND Situacao IN ({1})",
                string.Join(",", idsPedido), (int)Pedido.SituacaoPedido.LiberadoParcialmente));
            var idsPedidoNaoConfirmadosLiberacaoOuLiberadosParcialmente = ExecuteMultipleScalar<int>(session,
                string.Format("SELECT p.IdPedido FROM pedido p WHERE p.IdPedido IN ({0}) AND p.Situacao NOT IN ({1}, {2})", string.Join(",", idsPedido), (int)Pedido.SituacaoPedido.ConfirmadoLiberacao,
                    (int)Pedido.SituacaoPedido.LiberadoParcialmente));
            var idsPedidoRevendaSemProducaoConfirmadaLiberada = PedidoDAO.Instance.ObterIdsPedidoRevendaSemProducaoConfirmadaLiberada(session,
                string.Join(",", idsPedido))?.Split(',').Select(f => f.StrParaInt());

            #endregion

            #region Verifica��o das permiss�es do funcion�rio

            // Verifica se o funcion�rio possui permiss�o para liberar pedidos.
            if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) && !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
            {
                throw new Exception("Voc� n�o tem permiss�o para liberar pedidos.");
            }

            #endregion

            #region Valida��es do recebimento

            UtilsFinanceiro.ValidarRecebimento(session, caixaDiario, idCliente, idLoja, idsCartaoNaoIdentificado, idsContaBanco, idsFormaPagamento, gerarCredito, 0, false,
                UtilsFinanceiro.TipoReceb.LiberacaoAVista, totalPago, totalPagar);

            #endregion

            #region Verifica��es dos dados dos pedidos

            // Verifica se foram informados pedidos que devem ser liberados.
            if (idsPedido == null || !idsPedido.Any(f => f > 0))
            {
                throw new Exception("Informe os pedidos que devem ser liberados.");
            }

            // Verifica se a loja foi recuperada corretamente.
            if (idLoja == 0)
            {
                throw new Exception("N�o foi poss�vel recuperar a loja do(s) pedido(s) liberado(s).");
            }

            // Verifica se h� estoque dispon�vel para os produtos sendo liberados.
            if (!pedidosPossuemEstoque)
            {
                throw new Exception(mensagemEstoque);
            }

            // Verifica se todos os pedidos est�o na situa��o ConfirmadoLiberacao ou Liberado Parcialmente.
            if (idsPedidoNaoConfirmadosLiberacaoOuLiberadosParcialmente != null && idsPedidoNaoConfirmadosLiberacaoOuLiberadosParcialmente.Count() > 0)
            {
                throw new Exception(string.Format("Os pedidos {0} precisam estar na situa��o Confirmado PCP para que sejam liberados.",
                    string.Join(", ", idsPedidoNaoConfirmadosLiberacaoOuLiberadosParcialmente)));
            }

            // Verifica se existem pedidos de revenda que n�o possuem pedido de produ��o confirmado ou liberado.
            if (idsPedidoRevendaSemProducaoConfirmadaLiberada != null && idsPedidoRevendaSemProducaoConfirmadaLiberada.Count() > 0)
            {
                throw new Exception(string.Format("Os pedidos {0} est�o vinculados a um pedido de produ��o que ainda n�o foram confirmados.",
                    string.Join(", ", idsPedidoRevendaSemProducaoConfirmadaLiberada)));
            }

            // Verifica se os pedidos podem ser liberados.
            foreach (var idPedido in idsPedido)
            {
                #region Declara��o de vari�veis

                Pedido.TipoEntregaPedido tipoEntregaPedidoComparar = 0;
                var tipoPedido = PedidoDAO.Instance.GetTipoPedido(session, (uint)idPedido);
                var valorEntradaPedido = PedidoDAO.Instance.ObtemValorEntrada(session, (uint)idPedido);
                var idClientePedido = (int)PedidoDAO.Instance.GetIdCliente(session, (uint)idPedido);
                var idSinal = (int)PedidoDAO.Instance.ObtemIdSinal(session, (uint)idPedido).GetValueOrDefault();
                var idPagamentoAntecipado = (int)PedidoDAO.Instance.ObtemIdPagamentoAntecipado(session, (uint)idPedido).GetValueOrDefault();
                var idObra = 0;
                var pedidoTemSinalNaoPago = valorEntradaPedido > 0 && idSinal == 0 && idPagamentoAntecipado == 0;

                #endregion

                #region Verifica��es de produ��o

                // Garante que apenas pedidos finalizados sejam liberados se a empresa n�o controlar produ��o
                if (!PCPConfig.ControlarProducao)
                {
                    // Vari�vel criada para salvar se o pedido possui ou n�o espelho (PCP).
                    var pedidoPossuiEspelho = PedidoEspelhoDAO.Instance.ExisteEspelho(session, (uint)idPedido);
                    // Vari�vel criada para salvar a situa��o do pedido.
                    var situacaoPedido = PedidoEspelhoDAO.Instance.ObtemSituacao(session, (uint)idPedido);

                    // Verifica se o pedido possui espelho e se ele n�o est� finalizado.
                    if (pedidoPossuiEspelho && situacaoPedido != PedidoEspelho.SituacaoPedido.Finalizado)
                    {
                        throw new Exception(string.Format("O pedido {0} deve estar finalizado no PCP para ser liberado.", idPedido));
                    }
                }
                // Caso o controle novo de expedi��o balc�o esteja ativo, n�o permite liberar pedidos com tipos de entraga diferentes.
                else if (PCPConfig.UsarNovoControleExpBalcao)
                {
                    var tipoEntregaPedidoAtual = (Pedido.TipoEntregaPedido)PedidoDAO.Instance.ObtemTipoEntrega(session, (uint)idPedido);

                    // Recupera o tipo de entrega do pedido.
                    if (tipoEntregaPedidoComparar == 0)
                    {
                        tipoEntregaPedidoComparar = tipoEntregaPedidoAtual;
                    }
                    // Caso o tipo de entrega, recuperado, seja diferente do tipo de entrega do pedido atual, bloqueia a libera��o do pedido.
                    else if (tipoEntregaPedidoComparar != tipoEntregaPedidoAtual)
                    {
                        throw new Exception("A libera��o n�o pode conter pedidos com tipos de entrega diferentes.");
                    }
                }

                #endregion

                #region Verifica��es de pagamento

                // Verifica se o pedido est� para receber sinal e n�o recebeu, e essa situa��o est� bloqueada por configura��o.
                if (!PedidoConfig.NaoObrigarPagamentoAntesProducaoParaPedidoRevenda(tipoPedido) && pedidoTemSinalNaoPago)
                {
                    throw new Exception(string.Format("O pedido {0} tem um sinal de {1} a receber.", idPedido, valorEntradaPedido));
                }

                /* Chamado 65135.
                 * Caso a configura��o UsarControleDescontoFormaPagamentoDadosProduto esteja habilitada,
                 * impede que o pedido seja liberado com formas de pagamento que n�o foram selecionadas no pedido. */
                if (FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto)
                {
                    var tipoVenda = PedidoDAO.Instance.ObtemTipoVenda(session, (uint)idPedido);
                    var idFormaPagtoPedido = PedidoDAO.Instance.ObtemFormaPagto(session, (uint)idPedido);

                    if (tipoVenda != (int)Data.Model.Pedido.TipoVendaPedido.AVista || !idsFormaPagamento.Any(f => f > 0) || idsFormaPagamento.Any(f => f > 0 && f != idFormaPagtoPedido))
                    {
                        throw new Exception("N�o � permitido liberar os pedidos com uma forma de pagamento diferente da forma de pagamento definida no cadastro deles.");
                    }
                }

                #endregion

                #region Verifica��es do cliente

                // O cliente do pedido deve ser o mesmo cliente da libera��o.
                if (idCliente != idClientePedido)
                {
                    throw new Exception(string.Format("O cliente do pedido {0} � diferente do cliente da libera��o.", idPedido));
                }

                #endregion

                #region Atualiza o saldo da obra

                if (valorUtilizadoObra > 0)
                {
                    idObra = (int)PedidoDAO.Instance.GetIdObra(session, (uint)idPedido).GetValueOrDefault();

                    if (idObra > 0)
                    {
                        ObraDAO.Instance.AtualizaSaldo(session, (uint)idObra, caixaDiario);
                    }
                }

                #endregion
            }

            #endregion

            #region Verifica��o da situa��o das pe�as

            // Caso tenha algum pedido liberado parcialmente sendo liberado, verifica se as pe�as que est�o sendo liberadas j� foram 
            // liberadas anteriormente e se o valor da libera��o � o mesmo. No chamado 6823 um idProdPed de qtd 2 estava bloqueando a libera��o
            // porque uma das pe�as foi liberada parcialmente e ao liberar a outra o sistema bloqueava dizendo que as pe�as j� haviam sido liberadas,
            // para resolver inclu�mos um filtro pela data da libera��o, caso a libera��o tenha sido feita h� mais de 5 minutos ent�o a pe�a pode ser liberada,
            // pois esse tratamento foi feito para evitar que a libera��o seja feita mais de uma vez ao clicar no bot�o de liberar o pedido.
            if (idsPedidoLiberadosParcialmente != null && idsPedidoLiberadosParcialmente.Count() > 0)
            {
                var sqlPecasLiberadas = string.Format(@"SELECT COUNT(*) > 0
                    FROM produtos_liberar_pedido plp 
                        INNER JOIN liberarpedido lp ON (plp.IdLiberarPedido = lp.IdLiberarPedido)
                    WHERE lp.Situacao={0} AND IdProdPed=?idProdPed AND Qtde=?qtde AND QtdeCalc=?qtdeCalc
                        AND lp.DataLiberacao > DATE_ADD(NOW(), INTERVAL -5 MINUTE);", (int)LiberarPedido.SituacaoLiberarPedido.Liberado);
                var naoLiberado = false;

                for (var i = 0; i < idsProdutoPedido.Count(); i++)
                {
                    if (!ExecuteScalar<bool>(session, sqlPecasLiberadas,
                        new GDAParameter("?idProdPed", idsProdutoPedido.ElementAtOrDefault(i)),
                        new GDAParameter("?qtde", quantidadesLiberar.ElementAtOrDefault(i)),
                        new GDAParameter("?qtdeCalc", quantidadesLiberar.ElementAtOrDefault(i))))
                    {
                        naoLiberado = true;
                        break;
                    }
                }

                if (!naoLiberado)
                {
                    throw new Exception("As pe�as destes pedidos j� foram liberadas.");
                }
            }

            #endregion

            #region Verifica��o das informa��es de pagamento

            // Verifica se o pedido possui sinal ou pagamento antecipado a receber.
            if (!PCPConfig.TelaPedidoPcp.PermitirFinalizarComDiferencaEPagtoAntecip && pedidosPossuemSinalPagamentoAntecipadoAReceber)
            {
                throw new Exception(string.Format("Falha ao liberar pedidos. Erro: {0}", mensagemSinalPagamentoAntecipado));
            }

            // Verifica se foram informados cheques na libera��o do pedido e se eles foram recuperados no par�metro valoresPagos.
            if (indiceCheques > -1 && valoresPagos.ElementAtOrDefault(indiceCheques) == 0)
            {
                throw new Exception("Cadastre o(s) cheque(s).");
            }

            // Se o valor for inferior ao que deve ser pago, e o restante do pagto for gerarCredito, lan�a exce��o.
            if (gerarCredito && Math.Round(totalPago, 2) < Math.Round(totalPagar, 2))
            {
                throw new Exception(string.Format("Total a ser pago n�o confere com valor pago. Total a ser pago: {0} Valor pago: {1}.", totalPagar.ToString("C"), totalPago.ToString("C")));
            }
            // Se o total a ser pago for diferente do valor pago, considerando que n�o � para gerar cr�dito.
            else if (!gerarCredito && Math.Round(totalPagar, 2) != Math.Round(totalPago, 2))
            {
                throw new Exception(string.Format("Total a ser pago n�o confere com valor pago. Total a ser pago: {0} Valor pago: {1}.", totalPagar.ToString("C"), totalPago.ToString("C")));
            }

            #endregion

            #region Verifica��o de comiss�o

            // Apenas administrador, financeiro geral e financeiro pagto podem gerar comiss�es.
            if (idComissionado > 0 && !Config.PossuiPermissao(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento))
            {
                throw new Exception("Voc� n�o tem permiss�o para gerar comiss�es");
            }

            #endregion
        }

        /// <summary>
        /// Finaliza a pr� libera��o de pedidos � vista.
        /// </summary>
        public void FinalizarPreLiberacaoAVistaComTransacao(int idLiberarPedido)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    // Finaliza a libera��o criada acima, gerando movimenta��o no caixa, conta banc�ria, estoque etc.
                    FinalizarPreLiberacaoAVista(transaction, idLiberarPedido);

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("FinalizarPreLiberacaoAVista - ID libera��o: {0}.", idLiberarPedido), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao finalizar pr� libera��o do(s) pedido(s).", ex));
                }
            }
        }

        /// <summary>
        /// Finaliza a pr� libera��o de pedidos � vista.
        /// </summary>
        public void FinalizarPreLiberacaoAVista(GDASession session, int idLiberarPedido)
        {
            #region Declara��o de vari�veis

            var liberarPedido = GetElementByPrimaryKey(session, (uint)idLiberarPedido);
            // Vari�vel criada para salvar o retorno do recebimento da libera��o.
            UtilsFinanceiro.DadosRecebimento retorno = null;
            decimal saldoDevedor = 0;
            decimal saldoCredito = 0;
            // Vari�vel criada para salvar os IDs dos pedidos da libera��o.
            var idsPedido = IdsPedidos(session, idLiberarPedido.ToString()).Split(',').Select(f => f.StrParaInt());
            var produtosLiberarPedido = ProdutosLiberarPedidoDAO.Instance.GetByLiberarPedido(session, (uint)idLiberarPedido, false);
            var idComissionado = 0;
            var dataInicioGerarComissao = DateTime.Now;
            var dataFimGerarComissao = new DateTime();
            var idsPedidoLiberado = new List<int>();
            var sqlAtualizarLiberarPedido = string.Empty;
            var idLojaRecebimento = (uint)liberarPedido.IdLojaRecebimento.GetValueOrDefault();
            var totalPagar = liberarPedido.TotalPagar.GetValueOrDefault();
            var totalPago = liberarPedido.TotalPago.GetValueOrDefault();
            var descontarComissao = liberarPedido.DescontarComissao.GetValueOrDefault();
            var recebimentoGerarCredito = liberarPedido.RecebimentoGerarCredito.GetValueOrDefault();
            var recebimentoCaixaDiario = liberarPedido.RecebimentoCaixaDiario.GetValueOrDefault();
            // Recupera os cheques que foram selecionados no momento do recebimento da libera��o.
            var chequesRecebimento = ChequesLiberarPedidoDAO.Instance.ObterStringChequesPelaLiberacao(session, idLiberarPedido);
            var pagamentosLiberarPedido = PagtoLiberarPedidoDAO.Instance.GetByLiberacao(session, (uint)idLiberarPedido);
            // Vari�veis criadas para recuperar os dados do pagamento da libera��o.
            var idsCartaoNaoIdentificado = new List<int?>();
            var idsContaBanco = new List<int?>();
            var idsDepositoNaoIdentificado = new List<int?>();
            var idsFormaPagamento = new List<int>();
            var idsTipoCartao = new List<int?>();
            var numerosAutorizacaoCartao = new List<string>();
            var quantidadesParcelaCartao = new List<int?>();
            var valoresRecebimento = new List<decimal>();
            var numeroParcelaContaReceber = 0;

            #endregion

            #region Recupera��o dos dados de recebimento da libera��o

            if (pagamentosLiberarPedido.Any(f => f.IdFormaPagto != (uint)Pagto.FormaPagto.Credito && f.IdFormaPagto != (uint)Pagto.FormaPagto.Obra))
            {
                foreach (var pagamentoLiberarPedido in pagamentosLiberarPedido.Where(f => f.IdFormaPagto != (uint)Pagto.FormaPagto.Credito && f.IdFormaPagto != (uint)Pagto.FormaPagto.Obra)
                    .OrderBy(f => f.NumFormaPagto))
                {
                    idsCartaoNaoIdentificado.Add(pagamentoLiberarPedido.IdCartaoNaoIdentificado.GetValueOrDefault());
                    idsContaBanco.Add(pagamentoLiberarPedido.IdContaBanco.GetValueOrDefault());
                    idsDepositoNaoIdentificado.Add(pagamentoLiberarPedido.IdDepositoNaoIdentificado.GetValueOrDefault());
                    idsFormaPagamento.Add((int)pagamentoLiberarPedido.IdFormaPagto);
                    idsTipoCartao.Add(((int?)pagamentoLiberarPedido.IdTipoCartao).GetValueOrDefault());
                    numerosAutorizacaoCartao.Add(pagamentoLiberarPedido.NumAutCartao);
                    quantidadesParcelaCartao.Add(pagamentoLiberarPedido.QuantidadeParcelaCartao.GetValueOrDefault());
                    valoresRecebimento.Add(pagamentoLiberarPedido.ValorPagto);
                }
            }

            #endregion

            #region Recebimento da libera��o

            // Mesmo que o totalPagar seja 0 (zero), deve entrar neste m�todo, pois caso o totalPago tenha valor, 
            // ter� que ser gerado cr�dito para o cliente (caso tenha pago um sinal maior que o valor do pedido por exemplo).
            retorno = UtilsFinanceiro.Receber(session, idLojaRecebimento, null, null, liberarPedido, null, null, null, null, null, null, null, string.Join(",", idsPedido), liberarPedido.IdCliente, 0,
                null, DateTime.Now.ToString("dd/MM/yyyy"), totalPagar > 0 ? totalPagar : 0, totalPago, valoresRecebimento.ToArray(), idsFormaPagamento.Select(f => (uint)f).ToArray(),
                idsContaBanco.Select(f => (uint)f).ToArray(), idsDepositoNaoIdentificado.Select(f => (uint)f).ToArray(), idsCartaoNaoIdentificado.Select(f => (uint)f).ToArray(),
                idsTipoCartao.Select(f => (uint)f).ToArray(), null, null, 0, false, recebimentoGerarCredito, liberarPedido.CreditoUtilizado, liberarPedido.NumAutConstrucard, recebimentoCaixaDiario,
                quantidadesParcelaCartao.Select(f => (uint)f).ToArray(), chequesRecebimento, descontarComissao, UtilsFinanceiro.TipoReceb.LiberacaoAVista);

            if (retorno.ex != null)
            {
                throw retorno.ex;
            }

            #endregion

            #region Atualiza��o dos dados da libera��o

            // Atualiza o cr�dito gerado da libera��o.
            objPersistence.ExecuteCommand(session, string.Format("UPDATE liberarpedido SET CreditoGerado=?creditoGerado WHERE IdLiberarPedido={0}", idLiberarPedido),
                new GDAParameter("?creditoGerado", retorno.creditoGerado));

            liberarPedido.CreditoGerado = retorno.creditoGerado;

            #endregion

            #region Gera��o das contas recebidas

            //Gera uma conta recebida para cada tipo de pagamento
            // Se for pago com cr�dito, gera a conta recebida do credito
            if (liberarPedido.CreditoUtilizado > 0)
            {
                var idContaR = ContasReceberDAO.Instance.Insert(session, new ContasReceber()
                {
                    IdLoja = idLojaRecebimento,
                    IdLiberarPedido = (uint)idLiberarPedido,
                    IdFormaPagto = null,
                    IdConta = UtilsPlanoConta.GetPlanoVista((uint)Pagto.FormaPagto.Credito),
                    Recebida = true,
                    ValorVec = liberarPedido.CreditoUtilizado,
                    ValorRec = liberarPedido.CreditoUtilizado,
                    DataVec = DateTime.Now,
                    DataRec = DateTime.Now,
                    DataCad = DateTime.Now,
                    IdCliente = liberarPedido.IdCliente,
                    UsuRec = liberarPedido.IdFunc,
                    Renegociada = false,
                    NumParc = 1,
                    NumParcMax = 1
                });

                #region Salva o pagamento da conta

                var pagamentoContaReceber = new PagtoContasReceber
                {
                    IdContaR = idContaR,
                    IdFormaPagto = (uint)Pagto.FormaPagto.Credito,
                    ValorPagto = liberarPedido.CreditoUtilizado
                };

                PagtoContasReceberDAO.Instance.Insert(session, pagamentoContaReceber);

                #endregion
            }

            for (var i = 0; i < idsFormaPagamento.Count(); i++)
            {
                if (idsFormaPagamento.ElementAtOrDefault(i) == 0 || valoresRecebimento.ElementAtOrDefault(i) == 0)
                {
                    continue;
                }

                var idContaR = ContasReceberDAO.Instance.Insert(session, new ContasReceber()
                {
                    IdLoja = idLojaRecebimento,
                    IdLiberarPedido = (uint)idLiberarPedido,
                    IdFormaPagto = (uint)idsFormaPagamento.ElementAt(i),
                    IdConta = UtilsPlanoConta.GetPlanoVista((uint)idsFormaPagamento.ElementAt(i)),
                    Recebida = true,
                    ValorVec = valoresRecebimento.ElementAt(i),
                    ValorRec = valoresRecebimento.ElementAt(i),
                    DataVec = DateTime.Now,
                    DataRec = DateTime.Now,
                    DataCad = DateTime.Now,
                    IdCliente = liberarPedido.IdCliente,
                    UsuRec = liberarPedido.IdFunc,
                    Renegociada = false,
                    NumParc = 1,
                    NumParcMax = 1,
                    IdFuncComissaoRec = liberarPedido.IdCliente > 0 ? (int?)ClienteDAO.Instance.ObtemIdFunc(session, liberarPedido.IdCliente) : null
                });

                if (idsFormaPagamento.ElementAt(i) == (uint)Pagto.FormaPagto.Cartao)
                {
                    numeroParcelaContaReceber = ContasReceberDAO.Instance.AtualizarReferenciaContasCartao((GDATransaction)session, retorno, quantidadesParcelaCartao.Select(f => f.GetValueOrDefault()),
                        numeroParcelaContaReceber, i, idContaR);
                }

                #region Salva o pagamento da conta

                if (idsFormaPagamento.Count() > i && idsFormaPagamento.ElementAt(i) == (int)Pagto.FormaPagto.CartaoNaoIdentificado)
                {
                    var cartoesNaoIdentificado = CartaoNaoIdentificadoDAO.Instance.ObterPeloId(session, idsCartaoNaoIdentificado.Select(f => (uint)f).ToArray());

                    foreach (var cartaoNaoIdentificado in cartoesNaoIdentificado)
                    {
                        var pagamentoContaReceber = new PagtoContasReceber
                        {
                            IdContaR = idContaR,
                            IdFormaPagto = (uint)idsFormaPagamento.ElementAt(i),
                            ValorPagto = valoresRecebimento.ElementAt(i),
                            IdContaBanco = (uint)cartaoNaoIdentificado.IdContaBanco,
                            IdCartaoNaoIdentificado = cartaoNaoIdentificado.IdCartaoNaoIdentificado,
                            IdTipoCartao = (uint)cartaoNaoIdentificado.TipoCartao,
                            NumAutCartao = cartaoNaoIdentificado.NumAutCartao
                        };

                        PagtoContasReceberDAO.Instance.Insert(session, pagamentoContaReceber);
                    }
                }
                else
                {
                    var pagamentoContaReceber = new PagtoContasReceber
                    {
                        IdContaR = idContaR,
                        IdFormaPagto = (uint)idsFormaPagamento.ElementAt(i),
                        ValorPagto = valoresRecebimento.ElementAt(i),
                        IdContaBanco = idsFormaPagamento.ElementAt(i) != (uint)Pagto.FormaPagto.Dinheiro && idsContaBanco.ElementAtOrDefault(i) > 0 ? (uint?)idsContaBanco.ElementAt(i) : null,
                        IdTipoCartao = idsTipoCartao.ElementAtOrDefault(i) > 0 ? (uint?)idsTipoCartao.ElementAt(i) : null,
                        IdDepositoNaoIdentificado = idsDepositoNaoIdentificado.ElementAtOrDefault(i) > 0 ? (uint?)idsDepositoNaoIdentificado.ElementAt(i) : null,
                        QuantidadeParcelaCartao = quantidadesParcelaCartao.ElementAtOrDefault(i) > 0 ? (int?)quantidadesParcelaCartao.ElementAt(i) : null,
                        NumAutCartao = !string.IsNullOrWhiteSpace(numerosAutorizacaoCartao.ElementAtOrDefault(i)) ? numerosAutorizacaoCartao.ElementAt(i) : null
                    };

                    PagtoContasReceberDAO.Instance.Insert(session, pagamentoContaReceber);
                }

                #endregion
            }

            #endregion

            #region Gera��o da comiss�o dos pedidos

            if (descontarComissao)
            {
                var pedidosParaComissao = UtilsFinanceiro.GetPedidosForComissao(session, string.Join(",", idsPedido), "Pedido");

                foreach (var pedidoParaComissao in pedidosParaComissao)
                {
                    idComissionado = (int)pedidoParaComissao.IdComissionado;

                    if (pedidoParaComissao.DataConf != null)
                    {
                        if (pedidoParaComissao.DataConf.Value < dataInicioGerarComissao)
                        {
                            dataInicioGerarComissao = pedidoParaComissao.DataConf.Value;
                        }

                        if (pedidoParaComissao.DataConf.Value > dataFimGerarComissao)
                        {
                            dataFimGerarComissao = pedidoParaComissao.DataConf.Value;
                        }
                    }
                }

                if (dataFimGerarComissao < dataInicioGerarComissao)
                {
                    dataFimGerarComissao = dataInicioGerarComissao;
                }

                if (idComissionado > 0 && pedidosParaComissao != null && pedidosParaComissao.Count() > 0)
                {
                    ComissaoDAO.Instance.GerarComissao(session, Pedido.TipoComissao.Comissionado, pedidosParaComissao[0].IdComissionado.Value, string.Join(",", idsPedido),
                        dataInicioGerarComissao.ToString(CultureInfo.InvariantCulture), dataFimGerarComissao.ToString(CultureInfo.InvariantCulture), 0, null);
                }
            }

            #endregion

            #region Altera��o da situa��o dos pedidos de revenda

            if (FinanceiroConfig.DadosLiberacao.MarcaPedidoRevendaEntregueAoLiberar && Liberacao.Estoque.SaidaEstoqueBoxLiberar && PedidoConfig.DadosPedido.BloquearItensTipoPedido)
            {
                foreach (var idPedido in idsPedido.Where(f => f > 0))
                {
                    var situacaoPedido = PedidoDAO.Instance.ObtemSituacao(session, (uint)idPedido);
                    var tipoPedido = PedidoDAO.Instance.ObtemValorCampo<int>(session, "TipoPedido", string.Format("IdPedido={0}", idPedido));

                    if (situacaoPedido == Pedido.SituacaoPedido.Confirmado && tipoPedido == (int)Pedido.TipoPedidoEnum.Revenda)
                    {
                        PedidoDAO.Instance.AlteraSituacaoProducao(session, (uint)idPedido, Pedido.SituacaoProducaoEnum.Entregue);
                    }
                }
            }

            #endregion

            #region Atualiza��o do carregamento

            idsPedidoLiberado = PedidoDAO.Instance.GetIdsByLiberacao(session, (uint)idLiberarPedido)?.Select(f => (int)f).ToList() ?? new List<int>();

            if (idsPedidoLiberado.Any())
            {
                CarregamentoDAO.Instance.AlterarSituacaoFaturamentoCarregamentos(session, idsPedidoLiberado.Select(f => (uint)f));
            }

            // Atualiza o carregamento e as ocs parciais se houver.
            CarregamentoDAO.Instance.AtualizaCarregamentoParcial(session, produtosLiberarPedido.Select(f => f.IdProdPed).ToArray());

            #endregion

            #region Atualiza��o de saldo do cliente e da libera��o

            // Atualiza o total comprado pelo cliente.
            ClienteDAO.Instance.AtualizaTotalComprado(session, liberarPedido.IdCliente);

            ClienteDAO.Instance.ObterSaldoDevedor(session, liberarPedido.IdCliente, out saldoDevedor, out saldoCredito);

            // Atualiza a situa��o, saldo devedor e saldo de cr�dito do cliente, na libera��o.
            sqlAtualizarLiberarPedido = @" UPDATE liberarpedido
                    SET Situacao = {0}, SaldoDevedor = ?saldoDevedor, SaldoCredito = ?saldoCredito
                WHERE IdLiberarPedido = {1}";

            objPersistence.ExecuteCommand(session, string.Format(sqlAtualizarLiberarPedido, (int)LiberarPedido.SituacaoLiberarPedido.Liberado, idLiberarPedido),
                new GDAParameter("?saldoDevedor", saldoDevedor), new GDAParameter("?saldoCredito", saldoCredito));

            #endregion

            // Envia o e-mail.
            Email.EnviaEmailLiberacao(session, (uint)idLiberarPedido);
        }

        /// <summary>
        /// Cancela a pr� libera��o � vista.
        /// </summary>
        public void CancelarPreLiberacaoAVistaComTransacao(bool cancelamentoErroCapptaTef, DateTime dataEstornoBanco, int idLiberarPedido, string observacao)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    CancelarPreLiberacaoAVista(transaction, dataEstornoBanco, idLiberarPedido, observacao);

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("CancelarPreLiberacaoAVista - ID libera��o: {0}.", idLiberarPedido), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao cancelar pr� libera��o do(s) pedido(s).", ex));
                }
            }
        }

        /// <summary>
        /// Cancela a pr� libera��o � vista.
        /// </summary>
        public void CancelarPreLiberacaoAVista(GDASession session, DateTime dataEstornoBanco, int idLiberarPedido, string observacao)
        {
            #region Declara��o de vari�veis

            var usuarioLogado = UserInfo.GetUserInfo;
            var idsPedidoLiberacao = new List<int>();
            var liberacaoPedido = GetElementByPrimaryKey(session, idLiberarPedido);
            var liberacaoPossuiNotaFiscalAtiva = PossuiNotaFiscalAtiva(session, (uint)idLiberarPedido);
            var liberacaoPossuiTrocaDevolucaoAtiva = TrocaDevolucaoDAO.Instance.ExistsByLiberacao(session, (uint)idLiberarPedido);
            var liberacaoPossuiInstalacaoAtiva = objPersistence.ExecuteSqlQueryCount(session,
                string.Format(@"SELECT COUNT(*) FROM instalacao
                    WHERE (IdOrdemInstalacao IS NULL OR IdOrdemInstalacao=0) AND Situacao<>{0}
                        AND IdPedido IN (SELECT IdPedido FROM produtos_liberar_pedido WHERE IdLiberarPedido={1} AND QtdeCalc>0);", (int)Instalacao.SituacaoInst.Cancelada, idLiberarPedido)) > 0;
            var liberacaoPossuiVolumeExpedido = objPersistence.ExecuteSqlQueryCount(session,
                string.Format(@"SELECT COUNT(*) FROM volume v
                        INNER JOIN produtos_liberar_pedido plp ON (v.IdPedido = plp.IdPedido)
                        LEFT JOIN item_carregamento ic ON (v.IdVolume = ic.IdVolume)
                    WHERE (v.DataSaidaExpedicao IS NOT NULL OR ic.DataLeitura IS NOT NULL) AND plp.IdLiberarPedido={0} AND plp.Qtdecalc > 0", idLiberarPedido)) > 0;
            var liberacaoPossuiPecasExpedidas = objPersistence.ExecuteSqlQueryCount(session,
                string.Format(@"SELECT COUNT(*) FROM produto_impressao pi
                    WHERE pi.IdPedidoExpedicao IN (SELECT IdPedido FROM produtos_liberar_pedido WHERE IdLiberarPedido={0})
                    
                    UNION ALL
                        
                    SELECT COUNT(*) FROM produto_pedido_producao ppp
                    WHERE ppp.Situacao={1} AND ppp.SituacaoProducao={2} AND ppp.IdPedidoExpedicao IN (SELECT IdPedido FROM produtos_liberar_pedido WHERE IdLiberarPedido={0})

                    UNION ALL

                    SELECT COUNT(*) From produto_pedido_producao ppp 
                        INNER JOIN produtos_pedido pp ON (ppp.IdProdPed=pp.IdProdPedEsp)
                        INNER JOIN produtos_liberar_pedido plp ON (pp.IdProdPed=plp.IdProdPed)
                    WHERE ppp.Situacao={1} AND ppp.SituacaoProducao={2} AND plp.IdLiberarPedido={0}",
                    idLiberarPedido, (int)ProdutoPedidoProducao.SituacaoEnum.Producao, (int)SituacaoProdutoProducao.Entregue)) > 0;
            // Recupera os dados da sa�da de estoque da libera��o e seus produtos
            var saidaEstoque = SaidaEstoqueDAO.Instance.GetByLiberacao(session, (uint)idLiberarPedido);
            var produtosSaidaEstoque = saidaEstoque != null ? ProdutoSaidaEstoqueDAO.Instance.GetForRpt(session, saidaEstoque.IdSaidaEstoque).ToArray() : null;
            // Recupera os produtos da libera��o
            var produtosLiberarPedido = ProdutosLiberarPedidoDAO.Instance.GetByLiberarPedido(session, (uint)idLiberarPedido, false);
            var idsProdQtdeReserva = new Dictionary<int, Dictionary<int, float>>();
            var idsProdQtdeLiberacao = new Dictionary<int, Dictionary<int, float>>();

            #endregion

            #region Valida��es dos dados da libera��o

            /* Chamado 39231. */
            if (liberacaoPedido == null || idLiberarPedido == 0)
            {
                throw new Exception("N�o foi poss�vel recuperar a libera��o para efetuar o cancelamento. Tente novamente.");
            }

            // Verifica se a libera��o do pedido j� foi cancelada
            if (liberacaoPedido.Situacao == (int)LiberarPedido.SituacaoLiberarPedido.Cancelado)
            {
                throw new Exception("Libera��o j� cancelada.");
            }

            // Verifica se h� separa��o de valores e se h� notas fiscais ativas para a libera��o.
            if (liberacaoPossuiNotaFiscalAtiva && FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber)
            {
                throw new Exception("Esta libera��o possui uma ou mais notas fiscais n�o canceladas/inutilizadas, cancele essa(s) nota(s) para cancelar a libera��o.");
            }

            // Verifica se esta libera��o j� foi expedida na produ��o.
            if (liberacaoPossuiPecasExpedidas)
            {
                throw new Exception("Esta libera��o possui pe�as que j� foram marcadas como entregue. Verifique na produ��o a possibilidade de retir�-las desta situa��o.");
            }

            // Verifica se algum pedido desta libera��o possui troca/devolu��o n�o canceladas
            if (liberacaoPossuiTrocaDevolucaoAtiva)
            {
                throw new Exception("Um ou mais pedidos desta libera��o possuem troca/devolu��o, cancele-as antes de cancelar esta libera��o.");
            }

            //verifica se existe algum volume dos pedidos dessa libera��o que j� tenha sido expedido
            if (liberacaoPossuiVolumeExpedido && ProducaoConfig.ExpedirSomentePedidosLiberadosNoCarregamento)
            {
                throw new Exception("Um ou mais pedidos desta libera��o possuem volume(s) expedidos, estorne o(s) itens antes de cancelar esta libera��o.");
            }

            /* Chamado 53132.
                * Impede que a libera��o seja cancelada caso existam instala��es n�o canceladas para um ou mais pedidos dela. */
            if (liberacaoPossuiInstalacaoAtiva)
            {
                throw new Exception("Um ou mais pedidos desta libera��o possuem instala��es geradas, cancele as instala��es antes de cancelar esta libera��o.");
            }

            #endregion

            #region Atualiza��o dos produtos da libera��o

            objPersistence.ExecuteCommand(session, string.Format("UPDATE produtos_liberar_pedido SET QtdeCalc=0 WHERE IdLiberarPedido={0}", idLiberarPedido));

            #endregion

            #region Atualiza��o da situa��o da libera��o

            // Marca a libera��o como cancelada
            liberacaoPedido.Situacao = (int)LiberarPedido.SituacaoLiberarPedido.Cancelado;
            liberacaoPedido.IdFuncCanc = usuarioLogado.CodUser;
            liberacaoPedido.ObsCanc = observacao;
            liberacaoPedido.DataCanc = DateTime.Now;
            Update(session, liberacaoPedido);

            #endregion

            #region Atualiza��o do status dos pedidos da libera��o

            idsPedidoLiberacao.AddRange(ExecuteMultipleScalar<int>(session, string.Format("SELECT DISTINCT CAST(IdPedido AS CHAR) FROM produtos_liberar_pedido WHERE IdLiberarPedido={0}",
                idLiberarPedido))?.Where(f => f > 0).ToList() ?? new List<int>());

            if ((idsPedidoLiberacao?.Count()).GetValueOrDefault() > 0)
            {
                var sqlAtualizarDadosPedido = string.Format(@"UPDATE pedido p SET IdLiberarPedido=NULL, NumAutConstrucard=NULL, Situacao = IF(
                        (SELECT COUNT(*) FROM liberarpedido lp 
                            INNER JOIN produtos_liberar_pedido plp ON (plp.IdLiberarPedido=lp.IdLiberarPedido) 
                        WHERE plp.IdPedido=p.IdPedido AND lp.Situacao={0}) > 1, {1}, {2})
                    WHERE p.IdPedido IN ({3});",
                    (int)LiberarPedido.SituacaoLiberarPedido.Liberado, (int)Pedido.SituacaoPedido.LiberadoParcialmente, (int)Pedido.SituacaoPedido.ConfirmadoLiberacao,
                    string.Join(",", idsPedidoLiberacao.Where(f => f > 0)));

                objPersistence.ExecuteCommand(session, sqlAtualizarDadosPedido);

                //Percorre os pedidos da libera��o e salva log da mudan�a da situa��o
                foreach (var idPedidoLiberacao in idsPedidoLiberacao)
                {
                    var logAlteracao = new LogAlteracao();
                    logAlteracao.Tabela = (int)LogAlteracao.TabelaAlteracao.Pedido;
                    logAlteracao.IdRegistroAlt = idPedidoLiberacao;
                    logAlteracao.NumEvento = LogAlteracaoDAO.Instance.GetNumEvento(session, LogAlteracao.TabelaAlteracao.Pedido, idPedidoLiberacao);
                    logAlteracao.Campo = "Situac�o";
                    logAlteracao.DataAlt = DateTime.Now;
                    logAlteracao.IdFuncAlt = usuarioLogado.CodUser;
                    logAlteracao.ValorAnterior = "Liberado";
                    logAlteracao.ValorAtual = "Confirmado PCP";
                    logAlteracao.Referencia = LogAlteracao.GetReferencia(session, (int)LogAlteracao.TabelaAlteracao.Pedido, (uint)idPedidoLiberacao);

                    LogAlteracaoDAO.Instance.Insert(session, logAlteracao);
                }
            }

            #endregion

            #region Atualiza��o da reserva/libera��o dos produtos liberados

            #region C�lculo da reserva/libera��o de cada produto

            foreach (var produtoLiberarPedido in produtosLiberarPedido)
            {
                #region Declara��o de vari�veis

                var idLojaEstoque = (int)PedidoDAO.Instance.ObtemIdLoja(session, produtoLiberarPedido.IdPedido);
                // Tenta achar o produto da sa�da de estoque referente ao produto da libera��o.
                var produtoSaidaEstoque = saidaEstoque == null || produtosSaidaEstoque == null || produtosSaidaEstoque.Length == 0 ? null :
                    Array.Find(produtosSaidaEstoque, find => find.IdProdPed == produtoLiberarPedido.IdProdPed);
                var quantidadeEstorno = produtoSaidaEstoque != null ? (int)produtoSaidaEstoque.QtdeSaida : produtoLiberarPedido.Qtde;
                // Verifica o tipo de c�lculo do produto.
                var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(session, (int)produtoLiberarPedido.IdProd);
                // Verifica o tipo de c�lculo do produto.
                var m2Calc = Global.CalculosFluxo.ArredondaM2(session, produtoLiberarPedido.LarguraProd, (int)produtoLiberarPedido.AlturaProd, quantidadeEstorno, 0, produtoLiberarPedido.Redondo, 0,
                    tipoCalculo != (int)TipoCalculoGrupoProd.M2Direto);
                var m2 = tipoCalculo == (int)TipoCalculoGrupoProd.M2 || tipoCalculo == (int)TipoCalculoGrupoProd.M2Direto;
                var qtdEstornoEstoque = quantidadeEstorno;
                var transferencia = PedidoDAO.Instance.ObtemDeveTransferir(session, produtoLiberarPedido.IdPedido);
                var subGrupoVolume = SubgrupoProdDAO.Instance.IsSubgrupoGeraVolume(session, produtoLiberarPedido.IdGrupoProd, produtoLiberarPedido.IdSubgrupoProd.GetValueOrDefault());
                var entregaBalcao = PedidoDAO.Instance.ObtemTipoEntrega(session, produtoLiberarPedido.IdPedido) == (int)Pedido.TipoEntregaPedido.Balcao;
                var volumeApenasDePedidosEntrega = OrdemCargaConfig.GerarVolumeApenasDePedidosEntrega;
                var naoVolume = volumeApenasDePedidosEntrega ? entregaBalcao || !subGrupoVolume : !subGrupoVolume;
                var pedidoGerarProducaoParaCorte = PedidoDAO.Instance.GerarPedidoProducaoCorte(session, produtoLiberarPedido.IdPedido);
                var pedidoPossuiVolumeExpedido = false;
                float altura = 0;

                #endregion

                if (tipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 || tipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 ||
                    tipoCalculo == (int)TipoCalculoGrupoProd.MLAL6 || tipoCalculo == (int)TipoCalculoGrupoProd.ML)
                {
                    altura = ProdutosPedidoDAO.Instance.ObtemValorCampo<float>(session, "Altura", string.Format("IdProdPed={0}", produtoLiberarPedido.IdProdPed));
                    qtdEstornoEstoque = quantidadeEstorno * altura;
                }

                /* Chamado 54238.
                 * Caso o volume tenha sido expedido, o estoque e a reserva/libera��o n�o podem ser alterados, pois, a baixa j� ocorreu na expedi��o dele. */
                foreach (var volume in VolumeDAO.Instance.ObterPeloPedido(session, (int)produtoLiberarPedido.IdPedido))
                {
                    if (VolumeDAO.Instance.TemExpedicao(session, volume.IdVolume))
                    {
                        pedidoPossuiVolumeExpedido = true;
                        break;
                    }
                }

                if (!pedidoGerarProducaoParaCorte && !pedidoPossuiVolumeExpedido)
                {
                    if ((((Liberacao.Estoque.SaidaEstoqueAoLiberarPedido && (!GrupoProdDAO.Instance.IsVidro((int)produtoLiberarPedido.IdGrupoProd) || !PCPConfig.ControlarProducao)) ||
                        (Liberacao.Estoque.SaidaEstoqueBoxLiberar && GrupoProdDAO.Instance.IsVidro((int)produtoLiberarPedido.IdGrupoProd) &&
                        SubgrupoProdDAO.Instance.IsSubgrupoProducao(session, (int)produtoLiberarPedido.IdGrupoProd, (int?)produtoLiberarPedido.IdSubgrupoProd))) && naoVolume) || transferencia)
                    {
                        // Estorna a sa�da dada neste produto, se o pedido n�o tiver que transferir
                        if (!transferencia)
                        {
                            ProdutosPedidoDAO.Instance.MarcarSaida(session, produtoLiberarPedido.IdProdPed, quantidadeEstorno * -1, 0, System.Reflection.MethodBase.GetCurrentMethod().Name, string.Empty);
                        }

                        // Credita o estoque
                        MovEstoqueDAO.Instance.CreditaEstoqueLiberacao(session, produtoLiberarPedido.IdProd, (uint)idLojaEstoque, (uint)idLiberarPedido, produtoLiberarPedido.IdPedido,
                            produtoLiberarPedido.IdProdLiberarPedido, (decimal)(m2 ? m2Calc : qtdEstornoEstoque));
                    }
                    else
                    {
                        #region Salva dados para alterar o campo LIBERACAO do produto loja

                        // Salva o produto e a quantidade dele que deve entrar da coluna LIBERACAO.
                        if (!idsProdQtdeLiberacao.ContainsKey(idLojaEstoque))
                        {
                            idsProdQtdeLiberacao.Add(idLojaEstoque, new Dictionary<int, float> { { (int)produtoLiberarPedido.IdProd, m2 ? m2Calc : qtdEstornoEstoque } });
                        }
                        else if (!idsProdQtdeLiberacao[idLojaEstoque].ContainsKey((int)produtoLiberarPedido.IdProd))
                        {
                            idsProdQtdeLiberacao[idLojaEstoque].Add((int)produtoLiberarPedido.IdProd, m2 ? m2Calc : qtdEstornoEstoque);
                        }
                        else
                        {
                            idsProdQtdeLiberacao[idLojaEstoque][(int)produtoLiberarPedido.IdProd] += m2 ? m2Calc : qtdEstornoEstoque;
                        }

                        #endregion
                    }

                    #region Salva dados para alterar o campo RESERVA do produto loja

                    // Salva o produto e a quantidade dele que deve sair da coluna RESERVA.
                    if (!idsProdQtdeReserva.ContainsKey(idLojaEstoque))
                    {
                        idsProdQtdeReserva.Add(idLojaEstoque, new Dictionary<int, float> { { (int)produtoLiberarPedido.IdProd, m2 ? m2Calc : qtdEstornoEstoque } });
                    }
                    else if (!idsProdQtdeReserva[idLojaEstoque].ContainsKey((int)produtoLiberarPedido.IdProd))
                    {
                        idsProdQtdeReserva[idLojaEstoque].Add((int)produtoLiberarPedido.IdProd, m2 ? m2Calc : qtdEstornoEstoque);
                    }
                    else
                    {
                        idsProdQtdeReserva[idLojaEstoque][(int)produtoLiberarPedido.IdProd] += m2 ? m2Calc : qtdEstornoEstoque;
                    }

                    #endregion
                }
            }

            #endregion

            #region Atualiza��o dos totais de reserva/libera��o dos produtos

            if (produtosLiberarPedido != null && produtosLiberarPedido.Length > 0)
            {
                // Ajusta o campo RESERVA do produto loja.
                foreach (var idLojaReserva in idsProdQtdeReserva.Keys)
                {
                    if (idsProdQtdeReserva[idLojaReserva].Count > 0)
                    {
                        ProdutoLojaDAO.Instance.ColocarReserva(session, idLojaReserva, idsProdQtdeReserva[idLojaReserva], null, (int)idLiberarPedido, null, null, null, null, null,
                            "LiberarPedidoDAO - CancelarLiberacao");
                    }
                }

                // Ajusta o campo LIBERACAO do produto loja.
                foreach (var idLojaLiberacao in idsProdQtdeLiberacao.Keys)
                {
                    if (idsProdQtdeLiberacao[idLojaLiberacao].Count > 0)
                    {
                        ProdutoLojaDAO.Instance.TirarLiberacao(session, idLojaLiberacao, idsProdQtdeLiberacao[idLojaLiberacao], null, (int)idLiberarPedido, null, null, null, null, null,
                            "LiberarPedidoDAO - CancelarLiberacao");
                    }
                }
            }

            #endregion

            #endregion

            #region Atualiza��o do saldo das obras

            foreach (var id in IdsPedidos(session, idLiberarPedido.ToString()).Split(','))
            {
                if (string.IsNullOrEmpty(id))
                {
                    continue;
                }

                var idObra = PedidoDAO.Instance.GetIdObra(session, id.StrParaUint());

                if (idObra > 0)
                {
                    ObraDAO.Instance.AtualizaSaldo(session, null, idObra.Value, false, false);
                }
            }

            #endregion

            #region Atualiza��o da situa��o dos pedidos de revenda

            if (FinanceiroConfig.DadosLiberacao.MarcaPedidoRevendaEntregueAoLiberar && Liberacao.Estoque.SaidaEstoqueBoxLiberar && PedidoConfig.DadosPedido.BloquearItensTipoPedido)
            {
                objPersistence.ExecuteCommand(session, string.Format(@"UPDATE pedido SET SituacaoProducao={0}
                    WHERE TipoPedido={1} AND SituacaoProducao={2} AND IdPedido IN (SELECT IdPedido FROM produtos_liberar_pedido WHERE IdLiberarPedido={3})",
                    (int)Pedido.SituacaoProducaoEnum.NaoEntregue, (int)Pedido.TipoPedidoEnum.Revenda, (int)Pedido.SituacaoProducaoEnum.Entregue, idLiberarPedido));
            }

            #endregion

            LogCancelamentoDAO.Instance.LogLiberarPedido(session, liberacaoPedido,
                observacao.Substring(observacao.ToLower().IndexOf("motivo do cancelamento: ", StringComparison.Ordinal) + "motivo do cancelamento: ".Length), true);
        }

        #endregion

        #region Liberar Pedido � Prazo

        /// <summary>
        /// Cancela a libera��o de um pedido.
        /// </summary>
        public uint CriarLiberacaoAPrazo(uint idCliente, string idsPedido, uint[] idsProdutosPedido, uint?[] idsProdutoPedidoProducao,
            float[] qtdeLiberar, decimal totalASerPago, int numParcelas, int[] diasParcelas, decimal[] valoresParcelas, uint? idParcela,
            bool receberEntrada, uint[] formasPagto, uint[] tiposCartao, decimal[] valoresPagos, uint[] idContasBanco,
            uint[] depositoNaoIdentificado, uint[] cartaoNaoIdentificado, bool utilizarCredito, decimal creditoUtilizado, string numAutConstrucard, bool cxDiario,
            bool descontarComissao, uint[] parcelasCartao, int tipoDesconto, decimal desconto, int tipoAcrescimo, decimal acrescimo,
            uint formaPagtoPrazo, decimal valorUtilizadoObra, string chequesPagto, string[] numAutCartao)
        {
            FilaOperacoes.LiberarPedido.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var idLiberarPedido = CriarLiberacaoAPrazo(transaction, idCliente, idsPedido, idsProdutosPedido,
                        idsProdutoPedidoProducao, qtdeLiberar, totalASerPago, numParcelas, diasParcelas, valoresParcelas, idParcela,
                        receberEntrada, formasPagto, tiposCartao, valoresPagos, idContasBanco, depositoNaoIdentificado, cartaoNaoIdentificado,
                        utilizarCredito, creditoUtilizado, numAutConstrucard, cxDiario, descontarComissao, parcelasCartao,
                        tipoDesconto, desconto, tipoAcrescimo, acrescimo, formaPagtoPrazo, valorUtilizadoObra, chequesPagto,
                        numAutCartao);

                    transaction.Commit();
                    transaction.Close();

                    return idLiberarPedido;
                }
                catch (Exception ex)
                {
                    ErroDAO.Instance.InserirFromException(string.Format("CriarLiberacaoAPrazo - IDs pedido: {0}", idsPedido), ex);

                    transaction.Rollback();
                    transaction.Close();

                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao criar libera��o do(s) pedido(s).", ex));
                }
                finally
                {
                    FilaOperacoes.LiberarPedido.ProximoFila();
                }
            }
        }

        /// <summary>
        /// Realiza libera��o de pedidos � prazo
        /// </summary>
        public uint CriarLiberacaoAPrazo(GDASession session, uint idCliente, string idsPedido, uint[] idsProdutosPedido,
            uint?[] idsProdutoPedidoProducao, float[] qtdeLiberar, decimal totalASerPago, int numParcelas, int[] diasParcelas,
            decimal[] valoresParcelas, uint? idParcela, bool receberEntrada, uint[] formasPagto, uint[] tiposCartao,
            decimal[] valoresPagos, uint[] idContasBanco, uint[] depositoNaoIdentificado, uint[] cartaoNaoIdentificado, bool utilizarCredito, decimal creditoUtilizado,
            string numAutConstrucard, bool cxDiario, bool descontarComissao, uint[] parcelasCartao, int tipoDesconto, decimal desconto,
            int tipoAcrescimo, decimal acrescimo, uint formaPagtoPrazo, decimal valorUtilizadoObra, string chequesPagto,
            string[] numAutCartao)
        {
            uint idLiberarPedido = 0;

            // #69907 - Altera a OBS do pedido para bloquear qualquer outra altera��o na tabela fora dessa transa��o
            var idPedidoTemp = Array.ConvertAll(idsPedido.Trim(',').Split(','), x => x.StrParaUint())[0];
            var obsPedido = PedidoDAO.Instance.ObtemObs(session, idPedidoTemp);
            objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido SET obs='Liberando Pedido' WHERE IdPedido={0}", idPedidoTemp));

            LoginUsuario login = UserInfo.GetUserInfo;
            var tipoFunc = login.TipoUsuario;
            if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) &&
                !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
                throw new Exception("Voc� n�o tem permiss�o para liberar pedidos.");

            // Garante que apenas pedidos finalizados sejam liberados se a empresa n�o controlar produ��o
            if (!PCPConfig.ControlarProducao)
                foreach (var idPedido in Array.ConvertAll(idsPedido.Trim(',').Split(','), x => x.StrParaUint()))
                    if (PedidoEspelhoDAO.Instance.ExisteEspelho(session, idPedido) && PedidoEspelhoDAO.Instance.ObtemSituacao(session, idPedido) != PedidoEspelho.SituacaoPedido.Finalizado)
                        throw new Exception("O pedido " + idPedido + " deve estar finalizado no PCP para ser liberado.");

            // Verifica se todos os pedidos est�o na situa��o ConfirmadoLiberacao
            if (objPersistence.ExecuteSqlQueryCount(session, "Select Count(*) From pedido Where idPedido In (" + idsPedido + ") " +
                "And situacao not in (" + (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + ", " + (int)Pedido.SituacaoPedido.LiberadoParcialmente + ")") > 0)
                throw new Exception("Alguns pedidos selecionados j� foram liberados.");

            //Chamado 46600
            var idsPedidoSemProducao = PedidoDAO.Instance.ObterIdsPedidoRevendaSemProducaoConfirmadaLiberada(session, idsPedido);
            if (!string.IsNullOrEmpty(idsPedidoSemProducao))
                throw new Exception("Os pedidos: " + idsPedidoSemProducao + " est�o vinculados a um pedido de produ��o que ainda n�o foram confirmados.");

            // Caso tenha algum pedido liberado parcialmente sendo liberado, verifica se as pe�as que est�o sendo liberadas j� foram 
            // liberadas anteriormente e se o valor da libera��o � o mesmo. No chamado 6823 um idProdPed de qtd 2 estava bloqueando a libera��o
            // porque uma das pe�as foi liberada parcialmente e ao liberar a outra o sistema bloqueava dizendo que as pe�as j� haviam sido liberadas,
            // para resolver inclu�mos um filtro pela data da libera��o, caso a libera��o tenha sido feita h� mais de 5 minutos ent�o a pe�a pode ser liberada,
            // pois esse tratamento foi feito para evitar que a libera��o seja feita mais de uma vez ao clicar no bot�o de liberar o pedido.
            if (objPersistence.ExecuteSqlQueryCount(session, "Select Count(*) From pedido Where idPedido In (" + idsPedido + ") " +
                "And situacao in (" + (int)Pedido.SituacaoPedido.LiberadoParcialmente + ")") > 0)
            {
                string sql = @"
                Select Count(*)>0
                From produtos_liberar_pedido plp 
                    Inner Join liberarpedido lp On (plp.idLiberarPedido=lp.idLiberarPedido)
                Where lp.situacao=" + (int)LiberarPedido.SituacaoLiberarPedido.Liberado + @" 
                    And idProdPed=?idProdPed And qtde=?qtde And qtdeCalc=?qtdeCalc And
                    lp.dataLiberacao > Date_Add(Now(), Interval -5 Minute)";

                bool naoLiberado = false;
                for (int i = 0; i < idsProdutosPedido.Length; i++)
                    if (!ExecuteScalar<bool>(session, sql, new GDAParameter("?idProdPed", idsProdutosPedido[i]), new GDAParameter("?qtde", qtdeLiberar[i]),
                        new GDAParameter("?qtdeCalc", qtdeLiberar[i])))
                    {
                        naoLiberado = true;
                        break;
                    }

                if (!naoLiberado)
                    throw new Exception("As pe�as destes pedidos j� foram liberadas.");
            }

            string mensagem;
            if (!PedidoDAO.Instance.VerificaSinalPagamentoReceber(session, idsPedido, out mensagem))
                throw new Exception("Falha ao liberar pedidos. Erro: " + mensagem);

            // Verifica se cliente possui limite dispon�vel para liberar os pedidos, desde que os mesmos j� n�o estejam debitando do limite
            var debitosCliente = ContasReceberDAO.Instance.GetDebitos(session, idCliente, idsPedido);
            var limiteCliente = ClienteDAO.Instance.ObtemValorCampo<decimal>(session, "limite", "id_Cli=" + idCliente);
            if (!FinanceiroConfig.DebitosLimite.EmpresaConsideraPedidoConferidoLimite && totalASerPago > 0 && limiteCliente > 0 && ((debitosCliente + totalASerPago) - creditoUtilizado) > limiteCliente)
                throw new Exception("O cliente n�o possui limite dispon�vel para realizar esta compra. Limite dispon�vel: " +
                    (limiteCliente - debitosCliente).ToString("C") + " Limite necess�rio: " + totalASerPago.ToString("C"));

            // Verifica se foi informado no cadastro de clientes quantas parcelas o mesmo divide suas compras
            if (ParcelasDAO.Instance.GetCountByCliente(session, idCliente, ParcelasDAO.TipoConsulta.Prazo) == 0 && totalASerPago > 0)
                throw new Exception("Informe no cadastro de clientes quantas parcelas devem ser geradas para este cliente.");

            decimal totalPago = 0;

            List<Cheques> lstChequesInseridos = new List<Cheques>();

            if (receberEntrada)
            {
                foreach (decimal valor in valoresPagos)
                    totalPago += valor;

                int indexCheques = UtilsFinanceiro.IndexFormaPagto(Glass.Data.Model.Pagto.FormaPagto.ChequeProprio, formasPagto);
                if (indexCheques > -1 && valoresPagos[indexCheques] == 0)
                    throw new Exception("Cadastre o(s) cheque(s).");

                // Se for pago com cr�dito, soma o mesmo ao totalPago
                if (creditoUtilizado > 0)
                    totalPago += creditoUtilizado;
            }

            decimal descontoAplicar = totalASerPago > 0 ? tipoDesconto == 1 ? desconto : desconto / totalASerPago * 100 : 0;
            decimal acrescimoAplicar = totalASerPago > 0 ? tipoAcrescimo == 1 ? acrescimo : acrescimo / totalASerPago * 100 : 0;
            decimal valorPagar = totalASerPago * ((100 - descontoAplicar + acrescimoAplicar) / 100);

            if (PedidoConfig.Desconto.DescontoMaximoLiberacao < descontoAplicar)
                throw new Exception("O desconto dado n�o � permitido, desconto aplicado: " + descontoAplicar + "% desconto m�ximo permitido: " + PedidoConfig.Desconto.DescontoMaximoLiberacao + "%");

            decimal totalPagarPrazo = 0;
            foreach (decimal vp in valoresParcelas)
                totalPagarPrazo += vp;

            if (valorPagar > 0 && totalPagarPrazo == 0 && !receberEntrada &&
                /* Chamado 15029.
                    * Caso todos os pedidos tenham sido cadastrados com o tipo de venda Obra, ent�o a forma de pagamento n�o deve ser
                    * solicitada, pois, o valor do pedido foi descontado no valor da obra. */
                objPersistence.ExecuteSqlQueryCount(session, @"
                SELECT COUNT(*) FROM pedido p
                WHERE p.IdPedido In (" + idsPedido + ") " +
                        "AND p.TipoVenda NOT IN (" + (int)Pedido.TipoVendaPedido.Obra + ")") > 0)
                throw new Exception("Defina as parcelas.");

            if (totalPagarPrazo < 0)
                throw new Exception("N�o � poss�vel liberar � prazo valores negativos.");

            /* Chamado 26155. */
            if (receberEntrada &&
                valorPagar > totalPago + totalPagarPrazo)
                throw new Exception(
                    string.Format(
                        "Valor pago � inferior ao total a ser pago. Total pago: {0} Total a ser pago: {1}",
                        totalPago.ToString("C"), valorPagar.ToString("C")));

            // Verifica se h� estoque dispon�vel para os produtos sendo liberados
            if (!PedidoPossuiEstoque(session, idsProdutosPedido, qtdeLiberar, out mensagem))
                throw new Exception(mensagem);

            uint idLoja = 0;

            // Caso o controle novo de expedi��o balc�o esteja ativo, n�o permite liberar pedidos com tipos de entraga diferentes.
            if (PCPConfig.UsarNovoControleExpBalcao)
            {
                Pedido.TipoEntregaPedido tipoEntregaPedido = 0;

                foreach (var id in idsPedido.TrimEnd(' ').TrimStart(' ').TrimStart(',').TrimEnd(',').Split(','))
                {
                    var idLojaPedido = PedidoDAO.Instance.ObtemIdLoja(session, id.StrParaUint());

                    if (idLoja == 0)
                        idLoja = idLojaPedido;
                    
                    if (tipoEntregaPedido == 0)
                        tipoEntregaPedido = (Pedido.TipoEntregaPedido)PedidoDAO.Instance.ObtemTipoEntrega(session, id.StrParaUint());
                    else if (tipoEntregaPedido != (Pedido.TipoEntregaPedido)PedidoDAO.Instance.ObtemTipoEntrega(session, id.StrParaUint()))
                        throw new Exception("A libera��o n�o pode conter pedidos com tipos de entrega diferentes.");
                }
            }
            /* Chamado 52405. */
            else
            {
                // Recupera o id do primeiro pedido liberado.
                var idPrimeiroPedidoLiberado = !string.IsNullOrEmpty(idsPedido) ?
                    idsPedido.TrimEnd(' ').TrimStart(' ').TrimStart(',').TrimEnd(',').Split(',')[0].StrParaUint() : idsPedido.StrParaUint();

                // Recupera a loja do primeiro pedido liberado.
                idLoja = PedidoDAO.Instance.ObtemIdLoja(session, idPrimeiroPedidoLiberado);
            }

            /* Chamado 52405. */
            idLoja = Configuracoes.ComissaoConfig.ComissaoPorContasRecebidas || Geral.ConsiderarLojaClientePedidoFluxoSistema ? idLoja : UserInfo.GetUserInfo.IdLoja;

            if (idLoja == 0)
                throw new Exception("N�o foi poss�vel recuperar a loja do(s) pedido(s) liberado(s).");

            var sinalReceber = "";

            foreach (var id in idsPedido.TrimEnd(' ').TrimStart(' ').TrimStart(',').TrimEnd(',').Split(','))
            {
                //Verifica se o pedido est� para receber sinal e n�o recebeu
                var tipoPedido = PedidoDAO.Instance.GetTipoPedido(id.StrParaUint());
                var entrada = PedidoDAO.Instance.ObtemValorEntrada(id.StrParaUint());
                var idSinal = PedidoDAO.Instance.ObtemIdSinal(id.StrParaUint());
                var idPagamentoAntecipado = PedidoDAO.Instance.ObtemIdPagamentoAntecipado(id.StrParaUint());
                var idClientePedido = PedidoDAO.Instance.GetIdCliente(session, id.StrParaUint());

                if (entrada > 0 && idSinal.GetValueOrDefault() == 0 && idPagamentoAntecipado.GetValueOrDefault() == 0)
                    sinalReceber = sinalReceber + string.Format("O pedido {0} tem um sinal de {1} a receber. ", id, entrada);
                
                /* Chamado 56137. */
                if (idCliente != idClientePedido)
                    throw new Exception(string.Format("O cliente do pedido {0} � diferente do cliente da libera��o.", id));
            }

            if (!string.IsNullOrEmpty(sinalReceber))
                throw new Exception(sinalReceber);

            // Verifica se o cliente possui a parcela passada
            var parcelas = ParcelasDAO.Instance.GetByCliente(session, idCliente, ParcelasDAO.TipoConsulta.Todos);
            if (idParcela != null && parcelas.All(f => f.IdParcela != idParcela))
                throw new Exception(string.Format("O cliente n�o possui a parcela escolhida. Cliente: {0} | Parcela: {1}",
                    ClienteDAO.Instance.GetNome(session, idCliente), ParcelasDAO.Instance.ObtemDescricao(session, idParcela.Value)));

            // Verifica se o cliente possui a forma de pagto passada
            var formasPagtoCliente = FormaPagtoDAO.Instance.GetByCliente(session, idCliente);
            if (formaPagtoPrazo > 0 && formasPagtoCliente.All(f => f.IdFormaPagto != formaPagtoPrazo))
                throw new Exception(string.Format("O cliente n�o possui a forma de pagamento escolhida. Cliente: {0} | Forma Pagto: {1}",
                    ClienteDAO.Instance.GetNome(session, idCliente), FormaPagtoDAO.Instance.GetDescricao(session, formaPagtoPrazo)));

            UtilsFinanceiro.DadosRecebimento retorno = null;

            // Cadastra a libera��o antes da sess�o e na situa��o cancelada para resolver a seguinte situa��o:
            // Durante o processamento desta libera��o a pessoa pode imprimir a mesma por outra tela, o problema � que 
            // caso ocorra algum problema, a transa��o vai desfazer tudo, quando for feita uma nova libera��o, 
            // ela vai pegar o n�mero dessa, fazendo com que pare�a existir duas libera��es diferentes com o mesmo n�mero
            LiberarPedido liberaPed = new LiberarPedido
            {
                IdCliente = idCliente,
                Situacao = (int)LiberarPedido.SituacaoLiberarPedido.Processando
            };

            liberaPed.IdLiberarPedido = Insert(session, liberaPed);

            idLiberarPedido = liberaPed.IdLiberarPedido;
            liberaPed.ValorCreditoAoLiberar = ClienteDAO.Instance.GetCredito(session, idCliente);

            // Garante que n�o haver� chaves duplicadas para esta libera��o
            PagtoLiberarPedidoDAO.Instance.DeleteByLiberacao(session, idLiberarPedido);

            #region Cadastra as formas de pagamento

            int numPagto = 0;

            if (formaPagtoPrazo > 0)
            {
                PagtoLiberarPedido prazo = new PagtoLiberarPedido
                {
                    IdLiberarPedido = idLiberarPedido,
                    NumFormaPagto = ++numPagto,
                    IdFormaPagto = formaPagtoPrazo,
                    ValorPagto = totalPagarPrazo + liberaPed.CreditoGerado
                };

                PagtoLiberarPedidoDAO.Instance.Insert(session, prazo);
            }

            if (valorUtilizadoObra > 0)
            {
                PagtoLiberarPedido novo = new PagtoLiberarPedido
                {
                    IdLiberarPedido = idLiberarPedido,
                    NumFormaPagto = ++numPagto,
                    IdFormaPagto = (uint)Pagto.FormaPagto.Obra,
                    IdTipoCartao = null,
                    ValorPagto = valorUtilizadoObra
                };

                PagtoLiberarPedidoDAO.Instance.Insert(session, novo);
            }

            #endregion

            #region Recebimento da entrada

            if (receberEntrada && totalPago > 0)
            {
                // Ignora os juros dos cart�es ao calcular o valor pago/a pagar
                totalPago -= UtilsFinanceiro.GetJurosCartoes(session, UserInfo.GetUserInfo.IdLoja, valoresPagos, formasPagto, tiposCartao, parcelasCartao);

                retorno = UtilsFinanceiro.Receber(session, UserInfo.GetUserInfo.IdLoja, null, null, liberaPed, null, null, null, null, null, null, null,
                    string.Join(",", idsProdutosPedido.Where(f => f > 0).Select(f => ProdutosPedidoDAO.Instance.ObtemIdPedido(session, f))), idCliente, 0, null,
                    DateTime.Now.ToString("dd/MM/yyyy"), valorPagar, totalPago, valoresPagos, formasPagto, idContasBanco, depositoNaoIdentificado, cartaoNaoIdentificado, tiposCartao,
                    null, null, 0, false, false, creditoUtilizado, numAutConstrucard, cxDiario, parcelasCartao, chequesPagto, descontarComissao,
                    UtilsFinanceiro.TipoReceb.SinalLiberacao);

                if (retorno.ex != null)
                    throw retorno.ex;

                liberaPed.CreditoGerado = retorno.creditoGerado;
                liberaPed.CreditoUtilizado = creditoUtilizado;

                lstChequesInseridos = retorno.lstChequesInseridos;

                #region Atualiza formas de pagamento

                // Atualiza as formas de pagamento dos pedidos somente se o controle de desconto por forma de pagamento e dados do produto estiver desabilitado,
                // pois, a empresa que utiliza esse controle, libera somente pedidos com formas de pagamento iguais.
                if (!FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto)
                {
                    var atualizar = false;

                    foreach (uint fp in formasPagto)
                        if (fp != 0)
                        {
                            atualizar = true;
                            break;
                        }

                    if (atualizar)
                    {
                        var sqlFormaPagto = string.Empty;

                        // Atualiza forma de pagamento de acordo com aquela que foi escolhida pelo caixa.
                        if (formasPagto[0] != (int)Pagto.FormaPagto.Dinheiro)
                        {
                            sqlFormaPagto = string.Format("UPDATE pedido SET IdFormaPagto={0}", formasPagto[0]);

                            if (formasPagto.Length > 1 && formasPagto[1] > 0)
                                sqlFormaPagto += string.Format(", IdFormaPagto2={0}", formasPagto[1]);

                            objPersistence.ExecuteCommand(session, string.Format("{0} WHERE TipoVenda={1} AND IdPedido IN ({2});", sqlFormaPagto, (int)Pedido.TipoVendaPedido.APrazo, idsPedido));
                        }

                        // Atualiza tipo de cart�o de acordo com aquele que foi escolhido pelo caixa
                        if ((uint)Pagto.FormaPagto.Cartao == formasPagto[0])
                            objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido SET IdTipoCartao={0} WHERE IdPedido IN ({1});", tiposCartao[0], idsPedido));

                        if (formasPagto.Length > 1 && (uint)Pagto.FormaPagto.Cartao == formasPagto[1])
                            objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido SET IdTipoCartao2={0} WHERE IdPedido IN ({1});", tiposCartao[1], idsPedido));
                    }
                }

                #endregion

                #region Salva as formas de pagamento na tabela

                for (int i = 0; i < valoresPagos.Length; i++)
                    if (valoresPagos[i] > 0)
                    {
                        PagtoLiberarPedido novo = new PagtoLiberarPedido
                        {
                            IdLiberarPedido = idLiberarPedido,
                            IdFormaPagto = formasPagto[i],
                            NumFormaPagto = ++numPagto,
                            ValorPagto = valoresPagos[i],
                            IdTipoCartao = tiposCartao[i] > 0 ? (uint?)tiposCartao[i] : null,
                            NumAutCartao = numAutCartao[i]
                        };

                        PagtoLiberarPedidoDAO.Instance.Insert(session, novo);
                    }

                if (creditoUtilizado > 0)
                {
                    PagtoLiberarPedido novo = new PagtoLiberarPedido
                    {
                        IdLiberarPedido = idLiberarPedido,
                        NumFormaPagto = ++numPagto,
                        IdFormaPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.Credito,
                        IdTipoCartao = null,
                        ValorPagto = creditoUtilizado
                    };

                    PagtoLiberarPedidoDAO.Instance.Insert(session, novo);
                }

                #endregion

                #region Gera contas recebidas

                //Gera uma conta recebida para cada tipo de pagamento
                // Se for pago com cr�dito, gera a conta recebida do credito
                if (creditoUtilizado > 0)
                {
                    var idContaR = ContasReceberDAO.Instance.Insert(session, new ContasReceber()
                    {
                        IdLoja = idLoja,
                        IdLiberarPedido = idLiberarPedido,
                        IdFormaPagto = null,
                        IdConta = UtilsPlanoConta.GetPlanoSinal((uint)Pagto.FormaPagto.Credito),
                        Recebida = true,
                        ValorVec = creditoUtilizado,
                        ValorRec = creditoUtilizado,
                        DataVec = DateTime.Now,
                        DataRec = DateTime.Now,
                        DataCad = DateTime.Now,
                        IdCliente = idCliente,
                        UsuRec = login.CodUser,
                        Renegociada = false,
                        NumParc = 1,
                        NumParcMax = 1,
                        IdFuncComissaoRec = idCliente > 0 ? (int?)ClienteDAO.Instance.ObtemIdFunc(idCliente) : null
                });

                    #region Salva o pagamento da conta

                    var pagto = new PagtoContasReceber
                    {
                        IdContaR = idContaR,
                        IdFormaPagto = (uint)Pagto.FormaPagto.Credito,
                        ValorPagto = creditoUtilizado
                    };

                    PagtoContasReceberDAO.Instance.Insert(session, pagto);

                    #endregion
                }

                for (int i = 0; i < formasPagto.Length; i++)
                {
                    if (formasPagto[i] == 0 || valoresPagos[i] == 0)
                        continue;

                    var idContaR = ContasReceberDAO.Instance.Insert(session, new ContasReceber()
                    {
                        IdLoja = idLoja,
                        IdLiberarPedido = idLiberarPedido,
                        IdFormaPagto = formasPagto[i],
                        IdConta = UtilsPlanoConta.GetPlanoSinal(formasPagto[i]),
                        Recebida = true,
                        ValorVec = valoresPagos[i],
                        ValorRec = valoresPagos[i],
                        DataVec = DateTime.Now,
                        DataRec = DateTime.Now,
                        DataCad = DateTime.Now,
                        IdCliente = idCliente,
                        UsuRec = login.CodUser,
                        Renegociada = false,
                        NumParc = 1,
                        NumParcMax = 1,
                        IdFuncComissaoRec = idCliente > 0 ? (int?)ClienteDAO.Instance.ObtemIdFunc(idCliente) : null
                });

                    #region Salva o pagamento da conta

                    var pagto = new PagtoContasReceber
                    {
                        IdContaR = idContaR,
                        IdFormaPagto = formasPagto[i],
                        ValorPagto = valoresPagos[i],
                        IdContaBanco =
                            formasPagto[i] != (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro &&
                            idContasBanco[i] > 0
                                ? (uint?)idContasBanco[i]
                                : null,
                        IdTipoCartao = tiposCartao[i] > 0 ? (uint?)tiposCartao[i] : null,
                        IdDepositoNaoIdentificado =
                            depositoNaoIdentificado[i] > 0 ? (uint?)depositoNaoIdentificado[i] : null,
                        NumAutCartao = numAutCartao[i]
                    };

                    PagtoContasReceberDAO.Instance.Insert(session, pagto);

                    #endregion
                }
                
                #endregion
            }

            #endregion

            #region Salva na tabela os produtos liberados

            for (int i = 0; i < idsProdutosPedido.Length; i++)
            {
                ProdutosLiberarPedido novo = new ProdutosLiberarPedido
                {
                    IdLiberarPedido = idLiberarPedido,
                    IdPedido = ProdutosPedidoDAO.Instance.ObtemIdPedido(session, idsProdutosPedido[i]),
                    IdProdPed = idsProdutosPedido[i],
                    Qtde = qtdeLiberar[i],
                    QtdeCalc = qtdeLiberar[i],
                    IdProdPedProducao = idsProdutoPedidoProducao[i],
                    ValorIcms =
                        ProdutosLiberarPedidoDAO.Instance.GetValorIcmsForLiberacao(session, idCliente,
                            idsProdutosPedido[i], qtdeLiberar[i]),
                    ValorIpi =
                        ProdutosLiberarPedidoDAO.Instance.GetValorIpiForLiberacao(session, idCliente,
                            idsProdutosPedido[i], qtdeLiberar[i])
                };

                ProdutosLiberarPedidoDAO.Instance.Insert(session, novo);
            }

            #endregion

            #region Gera contas a receber para esta libera��o

            // Exclui todas as contas a receber do pedido antes de gerar as que ser�o geradas abaixo
            ContasReceberDAO.Instance.DeleteByLiberarPedido(session, idLiberarPedido, false);
            DateTime dataVenc = DateTime.Now;

            int numParc = 1;
            int diasParcelaAnterior = 0;

            for (int i = 0; i < numParcelas; i++)
            {
                if (valoresParcelas[i] == 0)
                    continue;

                if (diasParcelas.Count() > i)
                {
                    dataVenc = dataVenc.AddDays(diasParcelas[i] - diasParcelaAnterior);
                    diasParcelaAnterior = diasParcelas[i];
                }

                ContasReceber conta = new ContasReceber
                {
                    IdLoja = idLoja,
                    IdCliente = idCliente,
                    IdLiberarPedido = idLiberarPedido,
                    DataVec = dataVenc,
                    ValorVec = valoresParcelas[i],
                    IdConta = UtilsPlanoConta.GetPlanoPrazo(formaPagtoPrazo),
                    NumParc = numParc++,
                    NumParcMax = numParcelas,
                    IdFormaPagto = formaPagtoPrazo,
                    IdFuncComissaoRec = idCliente > 0 ? (int?)ClienteDAO.Instance.ObtemIdFunc(idCliente) : null
                };

                if (ContemPedidosReposicao(session, idLiberarPedido))
                    conta.TipoConta = (byte)((ContasReceber.TipoContaEnum)conta.TipoConta | ContasReceber.TipoContaEnum.Reposicao);

                if (ContasReceberDAO.Instance.Insert(session, conta) == 0)
                    throw new Exception("Conta a Receber n�o foi inserida.");
            }

            #endregion

            #region Altera situa��o dos pedidos para Liberado (Confirmado) e Gera instala��es

            foreach (string p in idsPedido.TrimEnd(',').Split(','))
            {
                var idPedido = p.StrParaUint();
                var situacao = (!Liberacao.DadosLiberacao.LiberarPedidoProdutos && !Liberacao.DadosLiberacao.LiberarPedidoAtrasadoParcialmente) || IsPedidoLiberado(session, idPedido) ?
                    Pedido.SituacaoPedido.Confirmado : Pedido.SituacaoPedido.LiberadoParcialmente;
                
                /* Chamado 65135.
                 * Caso a configura��o UsarControleDescontoFormaPagamentoDadosProduto esteja habilitada,
                 * impede que o pedido seja liberado com formas de pagamento que n�o foram selecionadas no pedido.
                 * Nesse caso, impede o recebimento da entrada no ato da libera��o do pedido. */
                if (FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto)
                {
                    var tipoVenda = PedidoDAO.Instance.ObtemTipoVenda(session, idPedido);
                    var idFormaPagtoPedido = PedidoDAO.Instance.ObtemFormaPagto(session, idPedido);

                    if (tipoVenda != (int)Data.Model.Pedido.TipoVendaPedido.APrazo || formaPagtoPrazo != idFormaPagtoPedido)
                        throw new Exception("N�o � permitido liberar os pedidos com uma forma de pagamento diferente da forma de pagamento definida no cadastro deles.");
                }

                PedidoDAO.Instance.AlteraSituacao(session, idPedido, situacao);

                ///Salva log da altera��o da situa��o do pedido
                var logData = new LogAlteracao();
                logData.Tabela = (int)LogAlteracao.TabelaAlteracao.Pedido;
                logData.IdRegistroAlt = (int)idPedido;
                logData.NumEvento = LogAlteracaoDAO.Instance.GetNumEvento(session, LogAlteracao.TabelaAlteracao.Pedido, (int)idPedido);
                logData.Campo = "Situac�o";
                logData.DataAlt = DateTime.Now;
                logData.IdFuncAlt = UserInfo.GetUserInfo != null ? UserInfo.GetUserInfo.CodUser : 0;
                logData.ValorAnterior = "Confirmado PCP";
                logData.ValorAtual = "Liberado";
                logData.Referencia = LogAlteracao.GetReferencia(session, (int)LogAlteracao.TabelaAlteracao.Pedido, idPedido);

                LogAlteracaoDAO.Instance.Insert(session, logData);

                // Gera instala��o para o pedido
                PedidoDAO.Instance.GerarInstalacao(session, idPedido, PedidoDAO.Instance.ObtemDataEntrega(session, idPedido));
            }

            // Atualiza a data da �ltima compra do cliente.
            ClienteDAO.Instance.AtualizaUltimaCompra(session, idCliente);

            // Atualiza a data da �ltima libera��o dos pedidos desta libera��o, para exibir corretamente no pedido
            objPersistence.ExecuteCommand(session, "update pedido set idLiberarPedido=" + idLiberarPedido +
                ", numAutConstrucard=?numAutConst where idPedido in (" + idsPedido + ")", new GDAParameter("?numAutConst", numAutConstrucard));

            #endregion

            #region Atualiza a libera��o de pedido

            liberaPed.IdCliente = idCliente;
            liberaPed.IdFunc = login.CodUser;
            liberaPed.TipoPagto = (int)LiberarPedido.TipoPagtoEnum.APrazo;
            liberaPed.DataLiberacao = DateTime.Now;
            liberaPed.Total = totalPagarPrazo + totalPago + valorUtilizadoObra;
            liberaPed.TipoDesconto = tipoDesconto;
            liberaPed.Desconto = desconto;
            liberaPed.TipoAcrescimo = tipoAcrescimo;
            liberaPed.Acrescimo = acrescimo;
            liberaPed.IdParcela = idParcela;

            Update(session, liberaPed);

            #endregion

            #region Gera a comiss�o dos pedidos

            if (descontarComissao)
            {
                uint idComissionado = 0;
                DateTime dataInicio = DateTime.Now, dataFim = new DateTime();

                foreach (Pedido ped in UtilsFinanceiro.GetPedidosForComissao(session, idsPedido, "Pedido"))
                {
                    idComissionado = ped.IdComissionado.Value;

                    if (ped.DataConf != null)
                    {
                        if (ped.DataConf.Value < dataInicio)
                            dataInicio = ped.DataConf.Value;
                        if (ped.DataConf.Value > dataFim)
                            dataFim = ped.DataConf.Value;
                    }
                }

                if (dataFim < dataInicio)
                    dataFim = dataInicio;

                if (idComissionado > 0)
                    ComissaoDAO.Instance.GerarComissao(session, Pedido.TipoComissao.Comissionado, idComissionado, idsPedido, dataInicio.ToString(CultureInfo.InvariantCulture), dataFim.ToString(CultureInfo.InvariantCulture), 0, null);
            }

            #endregion

            #region Atualiza o estoque

            AtualizaEstoque(session, idLiberarPedido, idCliente, idsPedido, idsProdutosPedido, qtdeLiberar, tipoAcrescimo);

            #endregion

            #region Atualiza o saldo das obras

            if (valorUtilizadoObra > 0)
            {
                foreach (var id in IdsPedidos(session, idLiberarPedido.ToString()).Split(','))
                {
                    if (string.IsNullOrEmpty(id))
                        continue;

                    uint? idObra = PedidoDAO.Instance.GetIdObra(session, id.StrParaUint());
                    if (idObra > 0)
                        ObraDAO.Instance.AtualizaSaldo(session, idObra.Value, cxDiario);
                }
            }

            #endregion

            #region Altera a situa��o para entregue de pedidos de revenda

            if (FinanceiroConfig.DadosLiberacao.MarcaPedidoRevendaEntregueAoLiberar &&
                Liberacao.Estoque.SaidaEstoqueBoxLiberar && PedidoConfig.DadosPedido.BloquearItensTipoPedido)
                foreach (var s in idsPedido.Split(','))
                {
                    if (string.IsNullOrEmpty(s))
                        continue;

                    var idPedido = s.StrParaUint();

                    if (PedidoDAO.Instance.ObtemSituacao(session, idPedido) == Pedido.SituacaoPedido.Confirmado &&
                        PedidoDAO.Instance.ObtemValorCampo<int>(session, "tipoPedido", "idPedido=" + idPedido) == (int)Pedido.TipoPedidoEnum.Revenda)
                        PedidoDAO.Instance.AlteraSituacaoProducao(session, idPedido, Pedido.SituacaoProducaoEnum.Entregue);
                }

            #endregion

            #region Carregamento parcial

            //Atualiza o carregamento e as ocs parciais se houver
            CarregamentoDAO.Instance.AtualizaCarregamentoParcial(session, idsProdutosPedido);

            #endregion

            // Envia o e-mail
            Email.EnviaEmailLiberacao(session, idLiberarPedido);

            // Atualiza o total comprado pelo cliente
            ClienteDAO.Instance.AtualizaTotalComprado(session, idCliente);

            var idsPedidoLiberado = PedidoDAO.Instance.GetIdsByLiberacao(idLiberarPedido);
            if (idsPedidoLiberado.Any())
                CarregamentoDAO.Instance.AlterarSituacaoFaturamentoCarregamentos(session, idsPedidoLiberado);

            #region Calcula o saldo devedor

            decimal saldoDevedor;
            decimal saldoCredito;

            ClienteDAO.Instance.ObterSaldoDevedor(session, idCliente, out saldoDevedor, out saldoCredito);

            #endregion

            //Chamado 46526
            var sqlUpdate = @"
            UPDATE liberarpedido
            SET situacao = {0}, SaldoDevedor = ?saldoDevedor, SaldoCredito = ?saldoCredito
            WHERE IdLiberarPedido = {1}";
            objPersistence.ExecuteCommand(session, string.Format(sqlUpdate, (int)LiberarPedido.SituacaoLiberarPedido.Liberado, idLiberarPedido),
                new GDAParameter("?saldoDevedor", saldoDevedor), new GDAParameter("?saldoCredito", saldoCredito));

            // #69907 - Ao final da transa��o volta a situa��o original do pedido
            objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido SET obs=?obs WHERE IdPedido={0}", idPedidoTemp), new GDAParameter("?obs", obsPedido));

            return idLiberarPedido;
        }

        #endregion

        #region Liberar Pedido Garantia/Reposi��o

        /// <summary>
        /// Libera��o de pedido de Garantia/Reposi��o
        /// </summary>
        public uint CriarLiberacaoGarantiaReposicao(uint idCliente, string idsPedido, uint[] idsProdutosPedido,
            uint?[] idsProdutoPedidoProducao, float[] qtdeLiberar)
        {
            FilaOperacoes.LiberarPedido.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    uint idLiberarPedido;

                    LoginUsuario login = UserInfo.GetUserInfo;
                    var tipoFunc = login.TipoUsuario;
                    if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) &&
                        !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
                        throw new Exception("Voc� n�o tem permiss�o para liberar pedidos.");

                    // Garante que apenas pedidos finalizados sejam liberados se a empresa n�o controlar produ��o
                    if (!PCPConfig.ControlarProducao)
                        foreach (uint idPedido in Array.ConvertAll(idsPedido.Trim(',').Split(','), x => x.StrParaUint()))
                            if (PedidoEspelhoDAO.Instance.ExisteEspelho(transaction, idPedido) && PedidoEspelhoDAO.Instance.ObtemSituacao(transaction, idPedido) != PedidoEspelho.SituacaoPedido.Finalizado)
                                throw new Exception("O pedido " + idPedido + " deve estar finalizado no PCP para ser liberado.");

                    // Verifica se todos os pedidos est�o na situa��o ConfirmadoLiberacao
                    if (objPersistence.ExecuteSqlQueryCount(transaction, "Select Count(*) From pedido Where idPedido In (" + idsPedido.Trim(',') + ") " +
                        "And situacao not in (" + (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + ", " + (int)Pedido.SituacaoPedido.LiberadoParcialmente + ")") > 0)
                        throw new Exception("Alguns pedidos selecionados j� foram liberados.");

                    //Chamado 46600
                    var idsPedidoSemProducao = PedidoDAO.Instance.ObterIdsPedidoRevendaSemProducaoConfirmadaLiberada(transaction, idsPedido);
                    if (!string.IsNullOrEmpty(idsPedidoSemProducao))
                        throw new Exception("Os pedidos: " + idsPedidoSemProducao + " est�o vinculados a um pedido de produ��o que ainda n�o foram confirmados.");

                    // Caso tenha algum pedido liberado parcialmente sendo liberado, verifica se as pe�as que est�o sendo liberadas j� foram 
                    // liberadas anteriormente e se o valor da libera��o � o mesmo. No chamado 6823 um idProdPed de qtd 2 estava bloqueando a libera��o
                    // porque uma das pe�as foi liberada parcialmente e ao liberar a outra o sistema bloqueava dizendo que as pe�as j� haviam sido liberadas,
                    // para resolver inclu�mos um filtro pela data da libera��o, caso a libera��o tenha sido feita h� mais de 5 minutos ent�o a pe�a pode ser liberada,
                    // pois esse tratamento foi feito para evitar que a libera��o seja feita mais de uma vez ao clicar no bot�o de liberar o pedido.
                    if (objPersistence.ExecuteSqlQueryCount(transaction, "Select Count(*) From pedido Where idPedido In (" + idsPedido + ") " +
                        "And situacao in (" + (int)Pedido.SituacaoPedido.LiberadoParcialmente + ")") > 0)
                    {
                        var sql =
                            @"Select Count(*)>0
                            From produtos_liberar_pedido plp 
                                Inner Join liberarpedido lp On (plp.idLiberarPedido=lp.idLiberarPedido)
                            Where lp.situacao=" + (int)LiberarPedido.SituacaoLiberarPedido.Liberado + @" 
                                And idProdPed=?idProdPed And qtde=?qtde And qtdeCalc=?qtdeCalc And
                                lp.dataLiberacao > Date_Add(Now(), Interval -5 Minute)";

                        var naoLiberado = false;
                        for (int i = 0; i < idsProdutosPedido.Length; i++)
                            if (!ExecuteScalar<bool>(transaction, sql, new GDAParameter("?idProdPed", idsProdutosPedido[i]), new GDAParameter("?qtde", qtdeLiberar[i]),
                                new GDAParameter("?qtdeCalc", qtdeLiberar[i])))
                            {
                                naoLiberado = true;
                                break;
                            }

                        if (!naoLiberado)
                            throw new Exception("As pe�as destes pedidos j� foram liberadas.");
                    }

                    // Verifica se h� estoque dispon�vel para os produtos sendo liberados
                    string mensagem;
                    if (!PedidoPossuiEstoque(transaction, idsProdutosPedido, qtdeLiberar, out mensagem))
                        throw new Exception(mensagem);

                    LiberarPedido liberaPed = new LiberarPedido
                    {
                        IdCliente = idCliente,
                        Situacao = (int)LiberarPedido.SituacaoLiberarPedido.Processando
                    };

                    liberaPed.IdLiberarPedido = Insert(transaction, liberaPed);

                    idLiberarPedido = liberaPed.IdLiberarPedido;

                    liberaPed.ValorCreditoAoLiberar = ClienteDAO.Instance.GetCredito(transaction, idCliente);

                    // Garante que n�o haver� chaves duplicadas para esta libera��o
                    PagtoLiberarPedidoDAO.Instance.DeleteByLiberacao(transaction, idLiberarPedido);

                    #region Salva na tabela os produtos liberados

                    for (var i = 0; i < idsProdutosPedido.Length; i++)
                    {
                        var novo = new ProdutosLiberarPedido
                        {
                            IdLiberarPedido = idLiberarPedido,
                            IdPedido = ProdutosPedidoDAO.Instance.ObtemIdPedido(transaction, idsProdutosPedido[i]),
                            IdProdPed = idsProdutosPedido[i],
                            Qtde = qtdeLiberar[i],
                            QtdeCalc = qtdeLiberar[i],
                            IdProdPedProducao = idsProdutoPedidoProducao[i],
                            ValorIcms =
                                ProdutosLiberarPedidoDAO.Instance.GetValorIcmsForLiberacao(transaction, idCliente,
                                    idsProdutosPedido[i], qtdeLiberar[i]),
                            ValorIpi =
                                ProdutosLiberarPedidoDAO.Instance.GetValorIpiForLiberacao(transaction, idCliente,
                                    idsProdutosPedido[i], qtdeLiberar[i])
                        };

                        ProdutosLiberarPedidoDAO.Instance.Insert(transaction, novo);
                    }

                    #endregion

                    #region Altera situa��o dos pedidos para Liberado (Confirmado)

                    foreach (var p in idsPedido.TrimEnd(',').Split(','))
                    {
                        var idPedido = p.StrParaUint();
                        var idClientePedido = PedidoDAO.Instance.GetIdCliente(transaction, idPedido);

                        /* Chamado 56137. */
                        if (idCliente != idClientePedido)
                            throw new Exception(string.Format("O cliente do pedido {0} � diferente do cliente da libera��o.", idPedido));

                        Pedido.SituacaoPedido situacao = (!Liberacao.DadosLiberacao.LiberarPedidoProdutos &&
                            !Liberacao.DadosLiberacao.LiberarPedidoAtrasadoParcialmente) ||
                            IsPedidoLiberado(transaction, idPedido) ? Pedido.SituacaoPedido.Confirmado : Pedido.SituacaoPedido.LiberadoParcialmente;

                        PedidoDAO.Instance.AlteraSituacao(transaction, idPedido, situacao);

                        // Gera instala��o para o pedido
                        PedidoDAO.Instance.GerarInstalacao(transaction, idPedido, PedidoDAO.Instance.ObtemDataEntrega(transaction, idPedido));
                    }

                    // Atualiza a data da �ltima libera��o dos pedidos desta libera��o, para exibir corretamente no pedido
                    objPersistence.ExecuteCommand(transaction, "update pedido set idLiberarPedido=" + idLiberarPedido +
                        " where idPedido in (" + idsPedido + ")", null);

                    #endregion

                    #region Atualiza a libera��o de pedido

                    liberaPed.IdFunc = login.CodUser;
                    liberaPed.IdCliente = idCliente;
                    liberaPed.TipoPagto = PedidoDAO.Instance.IsPedidoReposicao(transaction, idsPedido) ? (int)LiberarPedido.TipoPagtoEnum.Reposicao :
                        (int)LiberarPedido.TipoPagtoEnum.Garantia;
                    liberaPed.DataLiberacao = DateTime.Now;
                    liberaPed.Total = 0;
                    liberaPed.TipoDesconto = 1;
                    liberaPed.Desconto = 0;
                    liberaPed.TipoAcrescimo = 1;
                    liberaPed.Acrescimo = 0;

                    Update(transaction, liberaPed);

                    #endregion

                    #region Atualiza o estoque

                    AtualizaEstoque(transaction, idLiberarPedido, idCliente, idsPedido, idsProdutosPedido, qtdeLiberar, 0);

                    #endregion

                    #region Altera a situa��o para entregue de pedidos de revenda

                    if (FinanceiroConfig.DadosLiberacao.MarcaPedidoRevendaEntregueAoLiberar &&
                        Liberacao.Estoque.SaidaEstoqueBoxLiberar && PedidoConfig.DadosPedido.BloquearItensTipoPedido)
                        foreach (string s in idsPedido.Split(','))
                        {
                            if (String.IsNullOrEmpty(s))
                                continue;

                            uint idPedido = s.StrParaUint();

                            if (PedidoDAO.Instance.ObtemSituacao(transaction, idPedido) == Pedido.SituacaoPedido.Confirmado &&
                                PedidoDAO.Instance.ObtemValorCampo<int>(transaction, "tipoPedido", "idPedido=" + idPedido) == (int)Pedido.TipoPedidoEnum.Revenda)
                                PedidoDAO.Instance.AlteraSituacaoProducao(transaction, idPedido, Pedido.SituacaoProducaoEnum.Entregue);
                        }

                    #endregion

                    #region Carregamento parcial

                    //Atualiza o carregamento e as ocs parciais se houver
                    CarregamentoDAO.Instance.AtualizaCarregamentoParcial(transaction, idsProdutosPedido);

                    #endregion

                    // Envia o e-mail
                    Email.EnviaEmailLiberacao(transaction, idLiberarPedido);

                    // Atualiza o total comprado pelo cliente
                    ClienteDAO.Instance.AtualizaTotalComprado(transaction, idCliente);

                    #region Calcula o saldo devedor

                    decimal saldoDevedor;
                    decimal saldoCredito;

                    ClienteDAO.Instance.ObterSaldoDevedor(transaction, idCliente, out saldoDevedor, out saldoCredito);

                    #endregion

                    //Chamado 46526
                    var sqlUpdate = @"
                    UPDATE liberarpedido
                    SET situacao = {0}, SaldoDevedor = ?saldoDevedor, SaldoCredito = ?saldoCredito
                    WHERE IdLiberarPedido = {1}";
                    objPersistence.ExecuteCommand(transaction, string.Format(sqlUpdate, (int)LiberarPedido.SituacaoLiberarPedido.Liberado, idLiberarPedido),
                        new GDAParameter("?saldoDevedor", saldoDevedor), new GDAParameter("?saldoCredito", saldoCredito));

                    transaction.Commit();
                    transaction.Close();

                    return idLiberarPedido;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("CriarLiberacaoGarantiaReposicao - IDs pedido: {0}", idsPedido), ex);

                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao liberar pedidos.", ex));
                }
                finally
                {
                    FilaOperacoes.LiberarPedido.ProximoFila();
                }
            }
        }

        #endregion

        #region Liberar Pedido Funcion�rio

        /// <summary>
        /// Libera��o de pedido de Funcion�rio
        /// </summary>
        public uint CriarLiberacaoPedidoFuncionario(uint idCliente, string idsPedido, uint[] idsProdutosPedido,
            uint?[] idsProdutoPedidoProducao, float[] qtdeLiberar)
        {
            FilaOperacoes.LiberarPedido.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    uint idLiberarPedido;

                    LoginUsuario login = UserInfo.GetUserInfo;
                    var tipoFunc = login.TipoUsuario;
                    if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) &&
                        !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
                        throw new Exception("Voc� n�o tem permiss�o para liberar pedidos.");

                    // Garante que apenas pedidos finalizados sejam liberados se a empresa n�o controlar produ��o
                    if (!PCPConfig.ControlarProducao)
                        foreach (var idPedido in Array.ConvertAll(idsPedido.Trim(',').Split(','), x => x.StrParaUint()))
                            if (PedidoEspelhoDAO.Instance.ExisteEspelho(transaction, idPedido) && PedidoEspelhoDAO.Instance.ObtemSituacao(transaction, idPedido) != PedidoEspelho.SituacaoPedido.Finalizado)
                                throw new Exception("O pedido " + idPedido + " deve estar finalizado no PCP para ser liberado.");

                    // Verifica se todos os pedidos est�o na situa��o ConfirmadoLiberacao
                    if (objPersistence.ExecuteSqlQueryCount(transaction, "Select Count(*) From pedido Where idPedido In (" + idsPedido.Trim(',') + ") " +
                        "And situacao not in (" + (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + ", " + (int)Pedido.SituacaoPedido.LiberadoParcialmente + ")") > 0)
                        throw new Exception("Alguns pedidos selecionados j� foram liberados.");

                    //Chamado 46600
                    var idsPedidoSemProducao = PedidoDAO.Instance.ObterIdsPedidoRevendaSemProducaoConfirmadaLiberada(transaction, idsPedido);
                    if (!string.IsNullOrEmpty(idsPedidoSemProducao))
                        throw new Exception("Os pedidos: " + idsPedidoSemProducao + " est�o vinculados a um pedido de produ��o que ainda n�o foram confirmados.");

                    // Verifica se o funcion�rio foi associado � todos os pedidos
                    if (ExecuteScalar<bool>(transaction, "Select Count(*)>0 From pedido Where idFuncVenda is null And idPedido in (" + idsPedido.Trim(',') + ")"))
                        throw new Exception("Um ou mais pedidos est�o sem funcion�rio de venda associado.");

                    // Verifica se h� estoque dispon�vel para os produtos sendo liberados
                    string mensagem;
                    if (!PedidoPossuiEstoque(transaction, idsProdutosPedido, qtdeLiberar, out mensagem))
                        throw new Exception(mensagem);

                    var liberaPed = new LiberarPedido
                    {
                        IdCliente = idCliente,
                        Situacao = (int)LiberarPedido.SituacaoLiberarPedido.Processando
                    };

                    liberaPed.IdLiberarPedido = Insert(transaction, liberaPed);

                    idLiberarPedido = liberaPed.IdLiberarPedido;

                    //liberaPed.ValorCreditoAoLiberar = ClienteDAO.Instance.GetCredito(idCliente);

                    // Garante que n�o haver� chaves duplicadas para esta libera��o
                    PagtoLiberarPedidoDAO.Instance.DeleteByLiberacao(transaction, idLiberarPedido);

                    #region Salva na tabela os produtos liberados

                    for (var i = 0; i < idsProdutosPedido.Length; i++)
                    {
                        var novo = new ProdutosLiberarPedido
                        {
                            IdLiberarPedido = idLiberarPedido,
                            IdPedido = ProdutosPedidoDAO.Instance.ObtemIdPedido(transaction, idsProdutosPedido[i]),
                            IdProdPed = idsProdutosPedido[i],
                            Qtde = qtdeLiberar[i],
                            QtdeCalc = qtdeLiberar[i],
                            IdProdPedProducao = idsProdutoPedidoProducao[i],
                            ValorIcms =
                                ProdutosLiberarPedidoDAO.Instance.GetValorIcmsForLiberacao(transaction, idCliente,
                                    idsProdutosPedido[i], qtdeLiberar[i]),
                            ValorIpi =
                                ProdutosLiberarPedidoDAO.Instance.GetValorIpiForLiberacao(transaction, idCliente,
                                    idsProdutosPedido[i], qtdeLiberar[i])
                        };

                        ProdutosLiberarPedidoDAO.Instance.Insert(transaction, novo);
                    }

                    #endregion

                    #region Altera situa��o dos pedidos para Liberado (Confirmado)

                    foreach (var p in idsPedido.TrimEnd(',').Split(','))
                    {
                        var idPedido = p.StrParaUint();
                        var idClientePedido = PedidoDAO.Instance.GetIdCliente(transaction, idPedido);

                        /* Chamado 56137. */
                        if (idCliente != idClientePedido)
                            throw new Exception(string.Format("O cliente do pedido {0} � diferente do cliente da libera��o.", idPedido));

                        Pedido.SituacaoPedido situacao = (!Liberacao.DadosLiberacao.LiberarPedidoProdutos &&
                            !Liberacao.DadosLiberacao.LiberarPedidoAtrasadoParcialmente) ||
                            IsPedidoLiberado(transaction, idPedido) ? Pedido.SituacaoPedido.Confirmado : Pedido.SituacaoPedido.LiberadoParcialmente;

                        PedidoDAO.Instance.AlteraSituacao(transaction, idPedido, situacao);

                        // Gera instala��o para o pedido
                        PedidoDAO.Instance.GerarInstalacao(transaction, idPedido, PedidoDAO.Instance.ObtemDataEntrega(transaction, idPedido));
                    }

                    #endregion

                    #region Atualiza a libera��o de pedido

                    liberaPed.IdFunc = login.CodUser;
                    liberaPed.IdCliente = idCliente;
                    liberaPed.TipoPagto = (int)LiberarPedido.TipoPagtoEnum.Funcionario;
                    liberaPed.DataLiberacao = DateTime.Now;
                    liberaPed.Total = 0;
                    liberaPed.TipoDesconto = 1;
                    liberaPed.Desconto = 0;
                    liberaPed.TipoAcrescimo = 1;
                    liberaPed.Acrescimo = 0;

                    Update(transaction, liberaPed);

                    #endregion

                    #region Atualiza o estoque

                    AtualizaEstoque(transaction, idLiberarPedido, idCliente, idsPedido, idsProdutosPedido, qtdeLiberar, 0);

                    #endregion

                    #region Altera a situa��o para entregue de pedidos de revenda

                    if (FinanceiroConfig.DadosLiberacao.MarcaPedidoRevendaEntregueAoLiberar &&
                        Liberacao.Estoque.SaidaEstoqueBoxLiberar && PedidoConfig.DadosPedido.BloquearItensTipoPedido)
                        foreach (var s in idsPedido.Split(','))
                        {
                            if (string.IsNullOrEmpty(s))
                                continue;

                            uint idPedido = s.StrParaUint();

                            if (PedidoDAO.Instance.ObtemSituacao(transaction, idPedido) == Pedido.SituacaoPedido.Confirmado &&
                                PedidoDAO.Instance.ObtemValorCampo<int>(transaction, "tipoPedido", "idPedido=" + idPedido) == (int)Pedido.TipoPedidoEnum.Revenda)
                                PedidoDAO.Instance.AlteraSituacaoProducao(transaction, idPedido, Pedido.SituacaoProducaoEnum.Entregue);
                        }

                    #endregion

                    #region Carregamento parcial

                    //Atualiza o carregamento e as ocs parciais se houver
                    CarregamentoDAO.Instance.AtualizaCarregamentoParcial(transaction, idsProdutosPedido);

                    #endregion

                    // Atualiza o total comprado pelo funcion�rio
                    // ClienteDAO.Instance.AtualizaTotalComprado(idCliente);

                    // Envia o e-mail
                    Email.EnviaEmailLiberacao(transaction, idLiberarPedido);

                    //Faz a movimenta��o do funcion�rio
                    foreach (var idPedido in idsPedido.TrimEnd(',').Split(','))
                    {
                        var idPed = idPedido.StrParaUint();
                        if (idPed > 0)
                        {
                            var idFuncVenda = PedidoDAO.Instance.ObtemIdFuncVenda(transaction, idPed);
                            if (idFuncVenda > 0)
                            {
                                var total = PedidoDAO.Instance.GetTotal(transaction, idPed);
                                MovFuncDAO.Instance.MovimentarPedido(transaction, idFuncVenda.Value, idPed,
                                    UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.FuncRecebimento), 2, total, "");
                            }
                        }
                    }

                    #region Calcula o saldo devedor

                    decimal saldoDevedor;
                    decimal saldoCredito;

                    ClienteDAO.Instance.ObterSaldoDevedor(transaction, idCliente, out saldoDevedor, out saldoCredito);

                    #endregion

                    //Chamado 46526
                    var sqlUpdate = @"
                    UPDATE liberarpedido
                    SET situacao = {0}, SaldoDevedor = ?saldoDevedor, SaldoCredito = ?saldoCredito
                    WHERE IdLiberarPedido = {1}";
                    objPersistence.ExecuteCommand(transaction, string.Format(sqlUpdate, (int)LiberarPedido.SituacaoLiberarPedido.Liberado, idLiberarPedido),
                        new GDAParameter("?saldoDevedor", saldoDevedor), new GDAParameter("?saldoCredito", saldoCredito));

                    transaction.Commit();
                    transaction.Close();

                    return idLiberarPedido;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao liberar pedidos.", ex));
                }
                finally
                {
                    FilaOperacoes.LiberarPedido.ProximoFila();
                }
            }
        }

        #endregion

        #region Atualiza estoque

        private void AtualizaEstoque(GDASession sessao, uint idLiberarPedido, uint idCliente, string idsPedido, uint[] idsProdutosPedido, float[] qtdeLiberar,
            int tipoAcrescimo)
        {
            var idsProdQtdeReserva = new Dictionary<int, Dictionary<int, float>>();
            var idsProdQtdeLiberacao = new Dictionary<int, Dictionary<int, float>>();

            for (var i = 0; i < idsProdutosPedido.Length; i++)
            {
                var prodPed = ProdutosPedidoDAO.Instance.GetElementFluxoLite(sessao, idsProdutosPedido[i]);
                var idLojaPedido = PedidoDAO.Instance.ObtemIdLoja(sessao, prodPed.IdPedido);
                var idLoja = idLojaPedido > 0 ? idLojaPedido : UserInfo.GetUserInfo.IdLoja;
                var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(sessao, (int)prodPed.IdProd);

                // Remove a pe�a da reserva e a coloca na libera��o
                var m2Calc = Global.CalculosFluxo.ArredondaM2(sessao, prodPed.Largura, (int)prodPed.Altura, qtdeLiberar[i], 0, prodPed.Redondo, 0,
                    tipoCalculo != (int)TipoCalculoGrupoProd.M2Direto);

                var areaMinimaProd = ProdutoDAO.Instance.ObtemAreaMinima(sessao, (int)prodPed.IdProd);

                var m2CalcAreaMinima = Global.CalculosFluxo.CalcM2Calculo(sessao, idCliente, (int)prodPed.Altura, prodPed.Largura,
                    qtdeLiberar[i], (int)prodPed.IdProd, prodPed.Redondo, prodPed.Beneficiamentos.CountAreaMinimaSession(sessao), areaMinimaProd, false,
                    prodPed.Espessura, true);

                var m2 = tipoCalculo == (int)TipoCalculoGrupoProd.M2 || tipoCalculo == (int)TipoCalculoGrupoProd.M2Direto;
                var qtdSaidaEstoque = qtdeLiberar[i];

                if (tipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 ||
                    tipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)TipoCalculoGrupoProd.MLAL6 ||
                    tipoCalculo == (int)TipoCalculoGrupoProd.ML)
                    qtdSaidaEstoque *= prodPed.Altura;

                //Verifica se o pedido que esta sendo liberado deve ser tranferido.
                var transferencia = PedidoDAO.Instance.ObtemDeveTransferir(sessao, prodPed.IdPedido);

                var saidaNaoVidro = Liberacao.Estoque.SaidaEstoqueAoLiberarPedido && (!GrupoProdDAO.Instance.IsVidro((int)prodPed.IdGrupoProd) || !PCPConfig.ControlarProducao);
                var saidaBox = Liberacao.Estoque.SaidaEstoqueBoxLiberar && GrupoProdDAO.Instance.IsVidro((int)prodPed.IdGrupoProd) && SubgrupoProdDAO.Instance.IsSubgrupoProducao(sessao, (int)prodPed.IdGrupoProd, (int?)prodPed.IdSubgrupoProd);
                var subGrupoVolume = SubgrupoProdDAO.Instance.IsSubgrupoGeraVolume(sessao, prodPed.IdGrupoProd, prodPed.IdSubgrupoProd);
                var entregaBalcao = PedidoDAO.Instance.ObtemTipoEntrega(sessao, prodPed.IdPedido) == (int)Pedido.TipoEntregaPedido.Balcao;
                var volumeApenasDePedidosEntrega = OrdemCargaConfig.GerarVolumeApenasDePedidosEntrega;

                var naoVolume = volumeApenasDePedidosEntrega ? entregaBalcao || !subGrupoVolume : !subGrupoVolume;
                /* Chamado 54054. */
                var pedidoGerarProducaoParaCorte = PedidoDAO.Instance.GerarPedidoProducaoCorte(sessao, prodPed.IdPedido);
                var pedidoPossuiVolumeExpedido = false;

                /* Chamado 54238.
                 * Caso o volume tenha sido expedido, o estoque e a reserva/libera��o n�o podem ser alterados, pois, a baixa j� ocorreu na expedi��o dele. */
                foreach (var volume in VolumeDAO.Instance.ObterPeloPedido(sessao, (int)prodPed.IdPedido))
                    if (VolumeDAO.Instance.TemExpedicao(sessao, volume.IdVolume))
                    {
                        pedidoPossuiVolumeExpedido = true;
                        break;
                    }

                /* Chamado 64689. */
                if (!pedidoPossuiVolumeExpedido && !pedidoGerarProducaoParaCorte)
                {
                    if (((saidaNaoVidro || saidaBox) && naoVolume) || transferencia)
                    {
                        // Marca quantos produtos do pedido foi marcado como sa�da, se o pedido n�o tiver que transferir
                        if (!transferencia)
                        {
                            var idSaidaEstoque = SaidaEstoqueDAO.Instance.GetNewSaidaEstoque(sessao, idLoja, null, idLiberarPedido, null, false);

                            ProdutosPedidoDAO.Instance.MarcarSaida(sessao, prodPed.IdProdPed, qtdeLiberar[i], idSaidaEstoque, System.Reflection.MethodBase.GetCurrentMethod().Name, string.Empty);
                        }

                        MovEstoqueDAO.Instance.BaixaEstoqueLiberacao(sessao, prodPed.IdProd, idLoja, idLiberarPedido,
                            prodPed.IdPedido, ProdutosLiberarPedidoDAO.Instance.ObtemIdProdLiberarPedido(sessao, idLiberarPedido,
                            prodPed.IdProdPed), m2 ? (decimal)m2Calc : (decimal)qtdSaidaEstoque, m2 ? (decimal)m2CalcAreaMinima : 0);
                    }
                    else
                    {
                        #region Salva dados para alterar o campo LIBERACAO do produto loja

                        // Salva o produto e a quantidade dele que deve entrar da coluna LIBERACAO.
                        if (!idsProdQtdeLiberacao.ContainsKey((int)idLoja))
                            idsProdQtdeLiberacao.Add((int)idLoja, new Dictionary<int, float> { { (int)prodPed.IdProd, m2 ? m2Calc : qtdSaidaEstoque } });
                        else if (!idsProdQtdeLiberacao[(int)idLoja].ContainsKey((int)prodPed.IdProd))
                            idsProdQtdeLiberacao[(int)idLoja].Add((int)prodPed.IdProd, m2 ? m2Calc : qtdSaidaEstoque);
                        else
                            idsProdQtdeLiberacao[(int)idLoja][(int)prodPed.IdProd] += m2 ? m2Calc : qtdSaidaEstoque;

                        #endregion
                    }

                    #region Salva dados para alterar o campo RESERVA do produto loja

                    // Salva o produto e a quantidade dele que deve sair da coluna RESERVA.
                    if (!idsProdQtdeReserva.ContainsKey((int)idLoja))
                        idsProdQtdeReserva.Add((int)idLoja, new Dictionary<int, float> { { (int)prodPed.IdProd, m2 ? m2Calc : qtdSaidaEstoque } });
                    else if (!idsProdQtdeReserva[(int)idLoja].ContainsKey((int)prodPed.IdProd))
                        idsProdQtdeReserva[(int)idLoja].Add((int)prodPed.IdProd, m2 ? m2Calc : qtdSaidaEstoque);
                    else
                        idsProdQtdeReserva[(int)idLoja][(int)prodPed.IdProd] += m2 ? m2Calc : qtdSaidaEstoque;

                    #endregion
                }
            }

            // Ajusta o campo RESERVA do produto loja.
            foreach (var idLojaReserva in idsProdQtdeReserva.Keys)
                if (idsProdQtdeReserva[idLojaReserva].Count > 0)
                    ProdutoLojaDAO.Instance.TirarReserva(sessao, idLojaReserva, idsProdQtdeReserva[idLojaReserva], null,
                        (int)idLiberarPedido, null, null, null, null, null, "LiberarPedidoDAO - AtualizaEstoque");

            // Ajusta o campo LIBERACAO do produto loja.
            foreach (var idLojaLiberacao in idsProdQtdeLiberacao.Keys)
                if (idsProdQtdeLiberacao[idLojaLiberacao].Count > 0)
                    ProdutoLojaDAO.Instance.ColocarLiberacao(sessao, idLojaLiberacao, idsProdQtdeLiberacao[idLojaLiberacao], null,
                        (int)idLiberarPedido, null, null, null, null, null, "LiberarPedidoDAO - AtualizaEstoque");
        }

        #endregion

        #region Cancelar libera��o

        /// <summary>
        /// Cancela a libera��o de um pedido.
        /// </summary>
        public void CancelarLiberacao(uint idLiberarPedido, string obs, DateTime dataEstornoBanco, bool cancelamentoErroCapptaTef, bool gerarCredito)
        {
            FilaOperacoes.LiberarPedido.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    CancelarLiberacao(transaction, idLiberarPedido, obs, dataEstornoBanco, cancelamentoErroCapptaTef, gerarCredito);

                    transaction.Commit();
                    transaction.Close();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Close();

                    ErroDAO.Instance.InserirFromException(string.Format("CancelarLiberacao - ID: {0}", idLiberarPedido), ex);
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao cancelar libera��o do pedido.", ex));
                }
                finally
                {
                    FilaOperacoes.LiberarPedido.ProximoFila();
                }
            }
        }

        /// <summary>
        /// Cancela a libera��o de um pedido.
        /// </summary>
        public void CancelarLiberacao(GDASession session, uint idLiberarPedido, string obs, DateTime dataEstornoBanco,
            bool cancelamentoErroCapptaTef, bool gerarCredito)
        {
            Pedido[] lstPedidos;
            LiberarPedido liberacaoPedido = null;

            liberacaoPedido = GetElement(session, idLiberarPedido);

            /* Chamado 39231. */
            if (liberacaoPedido == null || idLiberarPedido == 0)
                throw new Exception("N�o foi poss�vel recuperar a libera��o para efetuar o cancelamento. Tente novamente.");

            // Verifica se a libera��o do pedido j� foi cancelada
            if (liberacaoPedido.Situacao == (int)LiberarPedido.SituacaoLiberarPedido.Cancelado)
                throw new Exception("Libera��o j� cancelada.");

            // Verifica se esta libera��o � a prazo e se suas parcelas j� foram pagas
            if (liberacaoPedido.TipoPagto == (int)LiberarPedido.TipoPagtoEnum.APrazo &&
                ContasReceberDAO.Instance.ContaRecebidaLiberacao(session, idLiberarPedido))
                throw new Exception("Esta libera��o possui parcelas j� recebidas ou renegociadas, cancele o recebimento/renegocia��es antes de cancelar a libera��o.");

            // Verifica se h� separa��o de valores e se h� notas fiscais ativas para a libera��o
            if (FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber && PossuiNotaFiscalAtiva(session, idLiberarPedido))
                throw new Exception("Esta libera��o possui uma ou mais notas fiscais n�o canceladas/inutilizadas, cancele essa(s) nota(s) para cancelar a libera��o.");

            // Verifica se esta libera��o j� foi expedida na produ��o
            if (objPersistence.ExecuteSqlQueryCount(session,
                    @"Select Count(*) From produto_pedido_producao ppp 
                        inner join produtos_pedido pp on (ppp.idProdPed=pp.idProdPedEsp)
                        inner join produtos_liberar_pedido plp on (pp.idProdPed=plp.idProdPed)
                    Where ppp.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @" 
                        and ppp.situacaoProducao=" + (int)SituacaoProdutoProducao.Entregue +
                        @" and plp.idLiberarPedido=" + idLiberarPedido) > 0)
            {
                throw new Exception("Esta libera��o possui pe�as que j� foram marcadas como entregue. Verifique na produ��o a possibilidade de retir�-las desta situa��o.");
            }

            // Verifica se esta libera��o j� foi expedida na produ��o (pedidos de revenda)
            if (objPersistence.ExecuteSqlQueryCount(session,
                    @"Select Count(*) From produto_pedido_producao ppp 
                    Where ppp.situacao=" + (int)ProdutoPedidoProducao.SituacaoEnum.Producao + @" 
                        and ppp.situacaoProducao=" + (int)SituacaoProdutoProducao.Entregue +
                        @" and ppp.idPedidoExpedicao In 
                            (Select idPedido From produtos_liberar_pedido Where idLiberarPedido=" + idLiberarPedido + ")") > 0)
            {
                throw new Exception("Esta libera��o possui pe�as que j� foram marcadas como entregue. Verifique na produ��o a possibilidade de retir�-las desta situa��o.");
            }

            // Verifica se esta libera��o j� foi expedida na produ��o (pedidos de revenda)
            if (objPersistence.ExecuteSqlQueryCount(session,
                    @"Select Count(*) From produto_impressao pi 
                    Where pi.idPedidoExpedicao In 
                        (Select idPedido From produtos_liberar_pedido Where idLiberarPedido=" + idLiberarPedido + ")") > 0)
            {
                throw new Exception("Esta libera��o possui pe�as que j� foram marcadas como entregue. Verifique na produ��o a possibilidade de retir�-las desta situa��o.");
            }

            // Verifica se algum pedido dessa libera��o j� tem a comiss�o paga
            if (objPersistence.ExecuteSqlQueryCount(session,
                    @"select count(*) from comissao_pedido cp inner join comissao c On (cp.idComissao=c.idComissao) 
                    where cp.idPedido in (select idPedido from produtos_liberar_pedido where idLiberarPedido=" +
                    idLiberarPedido + ") And c.dataCad>?dataLib",
                new GDAParameter("?dataLib", liberacaoPedido.DataLiberacao)) > 0)
                throw new Exception(
                    "Esta libera��o possui um ou mais pedidos cuja comiss�o j� foi paga. Cancele-as para continuar.");

            // Verifica se algum pedido desta libera��o possui troca/devolu��o n�o canceladas
            if (TrocaDevolucaoDAO.Instance.ExistsByLiberacao(session, idLiberarPedido))
                throw new Exception("Um ou mais pedidos desta libera��o possuem troca/devolu��o, cancele-as antes de cancelar esta libera��o.");

            //verifica se existe algum volume dos pedidos dessa libera��o que j� tenha sido expedido
            if (objPersistence.ExecuteSqlQueryCount(session,
                @"select count(*) from volume v inner join produtos_liberar_pedido plp on (v.IdPedido = plp.IdPedido)
                                                 left join item_carregamento ic on (v.IdVolume = ic.IdVolume)
                             where (v.datasaidaexpedicao IS NOT NULL OR ic.DataLeitura IS NOT NULL) and plp.idliberarpedido=" + idLiberarPedido + "  and plp.qtdecalc > 0") > 0 && ProducaoConfig.ExpedirSomentePedidosLiberadosNoCarregamento)
                throw new Exception("Um ou mais pedidos desta libera��o possuem volume(s) expedidos, estorne o(s) itens antes de cancelar esta libera��o.");

            /* Chamado 53132.
             * Apaga as instala��es em aberto geradas pela libera��o para os pedidos dela. */
            objPersistence.ExecuteCommand(session, string.Format(@"DELETE FROM instalacao WHERE (IdOrdemInstalacao IS NULL OR IdOrdemInstalacao=0) AND Situacao={0}
                AND IdPedido IN (SELECT IdPedido FROM produtos_liberar_pedido WHERE IdLiberarPedido={1} AND QtdeCalc>0);", (int)Instalacao.SituacaoInst.Aberta, idLiberarPedido));

            /* Chamado 53132.
             * Impede que a libera��o seja cancelada caso existam instala��es n�o canceladas para um ou mais pedidos dela. */
            if (objPersistence.ExecuteSqlQueryCount(session, string.Format(@"SELECT COUNT(*) FROM instalacao WHERE (IdOrdemInstalacao IS NULL OR IdOrdemInstalacao=0) AND Situacao<>{0}
                AND IdPedido IN (SELECT IdPedido FROM produtos_liberar_pedido WHERE IdLiberarPedido={1} AND QtdeCalc>0);", (int)Instalacao.SituacaoInst.Cancelada, idLiberarPedido)) > 0)
                throw new Exception("Um ou mais pedidos desta libera��o possuem instala��es geradas, cancele as instala��es antes de cancelar esta libera��o.");

            /* Chamado 70095. */
            var contasReceberLiberacao = ContasReceberDAO.Instance.GetByLiberacaoPedido(session, liberacaoPedido.IdLiberarPedido, false);
            foreach (var contasReceber in contasReceberLiberacao)
            {
                if (liberacaoPedido.TipoPagto != (int)LiberarPedido.TipoPagtoEnum.AVista && contasReceber.Recebida == true)
                {
                    if (contasReceber.IdFormaPagto > 0 && UtilsPlanoConta.GetPlanoSinal(contasReceber.IdFormaPagto.Value) != contasReceber.IdConta)
                        throw new Exception("A libera��o possui contas a receber/recebidas associadas � ela. Cancele os recebimentos antes de efetuar o cancelamento da libera��o.");
                }
            }

            UtilsFinanceiro.CancelaRecebimento(session, UtilsFinanceiro.TipoReceb.LiberacaoAVista, null,
                null, liberacaoPedido, null, null, 0,
                null, null, null, null, dataEstornoBanco, cancelamentoErroCapptaTef, gerarCredito);

            /* Chamado 64417. */
            //if (ExecuteScalar<bool>(session, string.Format("SELECT COUNT(*)>0 FROM contas_receber WHERE IdLiberarPedido={0}", idLiberarPedido)))
            //    throw new Exception("A libera��o possui contas a receber/recebidas associadas � ela. Cancele os recebimentos antes de efetuar o cancelamento da libera��o.");

            lstPedidos = PedidoDAO.Instance.GetByLiberacao(session, idLiberarPedido);            

            #region Remove os produtos da libera��o

            /* Chamado 39231. */
            if (objPersistence.ExecuteSqlQueryCount(session,
                string.Format("SELECT COUNT(*) FROM produtos_liberar_pedido WHERE QtdeCalc IS NOT NULL AND QtdeCalc>0 AND IdLiberarPedido={0}", idLiberarPedido)) == 0)
                throw new Exception("N�o foi poss�vel recuperar os produtos da libera��o. Tente novamente, caso o problema persista entre em contato com o suporte WebGlass.");

            objPersistence.ExecuteCommand(session,
                string.Format("UPDATE produtos_liberar_pedido SET QtdeCalc=0 WHERE IdLiberarPedido={0}", idLiberarPedido));

            #endregion

            #region Atualiza a situa��o da libera��o

            // Marca a libera��o como cancelada
            liberacaoPedido.Situacao = (int)LiberarPedido.SituacaoLiberarPedido.Cancelado;
            liberacaoPedido.IdFuncCanc = UserInfo.GetUserInfo.CodUser;
            liberacaoPedido.ObsCanc = obs;
            liberacaoPedido.DataCanc = DateTime.Now;
            Update(session, liberacaoPedido);

            #endregion

            #region Atualiza os status dos pedidos da libera��o

            var idsPedido = string.Join(",", ExecuteMultipleScalar<string>(session,
                "select distinct cast(idPedido as char) from produtos_liberar_pedido where idLiberarPedido=" +
                idLiberarPedido).ToArray());

            if (!string.IsNullOrEmpty(idsPedido))
            {
                var sqlSit = @"
                                Update pedido p set IdLiberarPedido=NULL, numAutConstrucard=null, situacao=
                                    if((
                                        select count(*) 
                                        from liberarpedido lp 
                                            inner join produtos_liberar_pedido plp on (plp.idLiberarPedido=lp.idLiberarPedido) 
                                        where plp.idPedido=p.idPedido 
                                            and lp.situacao=" + (int)LiberarPedido.SituacaoLiberarPedido.Liberado +
                             ") > 1, " +
                             (int)Pedido.SituacaoPedido.LiberadoParcialmente + ", " +
                             (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + ") " +
                             "Where p.idPedido in (" + idsPedido + ")";

                objPersistence.ExecuteCommand(session, sqlSit);

                //Percorre os pedidos da libera��o e salva log da mudan�a da situa��o
                foreach (var idPedido in idsPedido.Split(','))
                {
                    var logData = new LogAlteracao();
                    logData.Tabela = (int)LogAlteracao.TabelaAlteracao.Pedido;
                    logData.IdRegistroAlt = idPedido.StrParaInt();
                    logData.NumEvento = LogAlteracaoDAO.Instance.GetNumEvento(session, LogAlteracao.TabelaAlteracao.Pedido, idPedido.StrParaInt());
                    logData.Campo = "Situac�o";
                    logData.DataAlt = DateTime.Now;
                    logData.IdFuncAlt = UserInfo.GetUserInfo != null ? UserInfo.GetUserInfo.CodUser : 0;
                    logData.ValorAnterior = "Liberado";
                    logData.ValorAtual = "Confirmado PCP";
                    logData.Referencia = LogAlteracao.GetReferencia(session, (int)LogAlteracao.TabelaAlteracao.Pedido, idPedido.StrParaUint());

                    LogAlteracaoDAO.Instance.Insert(session, logData);
                }
            }

            #endregion

            #region Atualiza o IdTipoCartao do pedido

            /* Chamado 35522.
             * Caso a libera��o tenha sido recebida com cart�o de d�bito, ao cancel�-la deve-se alterar o cart�o para cr�dito no pedido.
             * Isso porque no pedido s�o exibidos somente cart�es de cr�dito,
             * e ao receber a libera��o com cart�o de d�bito o tipo do cart�o do pedido � atualizado automaticamente. */
            if (!string.IsNullOrEmpty(idsPedido))
                objPersistence.ExecuteCommand(session,
                    string.Format(
                        @"UPDATE pedido p SET IdTipoCartao=
                            IF(IdTipoCartao IS NULL, NULL,
                                IF(IdTipoCartao=2, 1,
                                    IF(IdTipoCartao=4, 3,
                                        IF(IdTipoCartao=6, 5, NULL))))
                        WHERE p.IdPedido IN ({0})", idsPedido));

            #endregion

            #region Volta pe�a para a reserva e tira da libera��o

            // Recupera os dados da sa�da de estoque da libera��o e seus produtos
            var saida = SaidaEstoqueDAO.Instance.GetByLiberacao(session, idLiberarPedido);
            var lstProdSaida = saida != null ? ProdutoSaidaEstoqueDAO.Instance.GetForRpt(session, saida.IdSaidaEstoque).ToArray() : null;

            // Recupera os produtos da libera��o
            var lstProd = ProdutosLiberarPedidoDAO.Instance.GetByLiberarPedido(session, idLiberarPedido, false);
            var idsProdQtdeReserva = new Dictionary<int, Dictionary<int, float>>();
            var idsProdQtdeLiberacao = new Dictionary<int, Dictionary<int, float>>();

            foreach (var prod in lstProd)
            {
                var idLoja = (int)PedidoDAO.Instance.ObtemIdLoja(session, prod.IdPedido);

                // Tenta achar o produto da sa�da de estoque referente ao produto da libera��o
                var pse = saida == null || lstProdSaida == null || lstProdSaida.Length == 0 ? null :
                    Array.Find(lstProdSaida, find => find.IdProdPed == prod.IdProdPed);

                var qtdEstorno = pse != null ? (int)pse.QtdeSaida : prod.Qtde;

                // Verifica o tipo de c�lculo do produto
                var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(session, (int)prod.IdProd);

                // Verifica o tipo de c�lculo do produto
                var m2Calc = Global.CalculosFluxo.ArredondaM2(session, prod.LarguraProd,
                    (int)prod.AlturaProd, qtdEstorno, 0, prod.Redondo, 0,
                    tipoCalculo != (int)TipoCalculoGrupoProd.M2Direto);

                var m2 = tipoCalculo == (int)TipoCalculoGrupoProd.M2 ||
                          tipoCalculo == (int)TipoCalculoGrupoProd.M2Direto;

                var qtdEstornoEstoque = qtdEstorno;

                if (tipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 ||
                    tipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 ||
                    tipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 ||
                    tipoCalculo == (int)TipoCalculoGrupoProd.MLAL6 ||
                    tipoCalculo == (int)TipoCalculoGrupoProd.ML)
                {
                    var altura = ProdutosPedidoDAO.Instance.ObtemValorCampo<float>(session, "altura",
                        "idProdPed=" + prod.IdProdPed);
                    qtdEstornoEstoque = qtdEstorno * altura;
                }

                var transferencia = PedidoDAO.Instance.ObtemDeveTransferir(session, prod.IdPedido);
                var subGrupoVolume = SubgrupoProdDAO.Instance.IsSubgrupoGeraVolume(session, prod.IdGrupoProd, prod.IdSubgrupoProd.GetValueOrDefault());
                var entregaBalcao = PedidoDAO.Instance.ObtemTipoEntrega(session, prod.IdPedido) == (int)Pedido.TipoEntregaPedido.Balcao;
                var volumeApenasDePedidosEntrega = OrdemCargaConfig.GerarVolumeApenasDePedidosEntrega;

                /* Chamado 24240. */
                var naoVolume = volumeApenasDePedidosEntrega ? entregaBalcao || !subGrupoVolume : !subGrupoVolume;
                /* Chamado 54054. */
                var pedidoGerarProducaoParaCorte = PedidoDAO.Instance.GerarPedidoProducaoCorte(session, prod.IdPedido);
                var pedidoPossuiVolumeExpedido = false;

                /* Chamado 54238.
                 * Caso o volume tenha sido expedido, o estoque e a reserva/libera��o n�o podem ser alterados, pois, a baixa j� ocorreu na expedi��o dele. */
                foreach (var volume in VolumeDAO.Instance.ObterPeloPedido(session, (int)prod.IdPedido))
                    if (VolumeDAO.Instance.TemExpedicao(session, volume.IdVolume))
                    {
                        pedidoPossuiVolumeExpedido = true;
                        break;
                    }

                if (!pedidoGerarProducaoParaCorte && !pedidoPossuiVolumeExpedido)
                {
                    if ((((Liberacao.Estoque.SaidaEstoqueAoLiberarPedido && (!GrupoProdDAO.Instance.IsVidro((int)prod.IdGrupoProd) ||
                        !PCPConfig.ControlarProducao)) || (Liberacao.Estoque.SaidaEstoqueBoxLiberar && GrupoProdDAO.Instance.IsVidro((int)prod.IdGrupoProd) &&
                        SubgrupoProdDAO.Instance.IsSubgrupoProducao(session, (int)prod.IdGrupoProd, (int?)prod.IdSubgrupoProd))) && naoVolume) || transferencia)
                    {
                        // Estorna a sa�da dada neste produto, se o pedido n�o tiver que transferir
                        if (!transferencia)
                            ProdutosPedidoDAO.Instance.MarcarSaida(session, prod.IdProdPed, qtdEstorno * -1, 0, System.Reflection.MethodBase.GetCurrentMethod().Name, string.Empty);

                        // Credita o estoque
                        MovEstoqueDAO.Instance.CreditaEstoqueLiberacao(session, prod.IdProd, (uint)idLoja, idLiberarPedido, prod.IdPedido,
                            prod.IdProdLiberarPedido, (decimal)(m2 ? m2Calc : qtdEstornoEstoque));
                    }
                    else
                    {
                        #region Salva dados para alterar o campo LIBERACAO do produto loja

                        // Salva o produto e a quantidade dele que deve entrar da coluna LIBERACAO.
                        if (!idsProdQtdeLiberacao.ContainsKey(idLoja))
                            idsProdQtdeLiberacao.Add(idLoja, new Dictionary<int, float> { { (int)prod.IdProd, m2 ? m2Calc : qtdEstornoEstoque } });
                        else if (!idsProdQtdeLiberacao[idLoja].ContainsKey((int)prod.IdProd))
                            idsProdQtdeLiberacao[idLoja].Add((int)prod.IdProd, m2 ? m2Calc : qtdEstornoEstoque);
                        else
                            idsProdQtdeLiberacao[idLoja][(int)prod.IdProd] += m2 ? m2Calc : qtdEstornoEstoque;

                        #endregion
                    }

                    #region Salva dados para alterar o campo RESERVA do produto loja

                        // Salva o produto e a quantidade dele que deve sair da coluna RESERVA.
                        if (!idsProdQtdeReserva.ContainsKey(idLoja))
                            idsProdQtdeReserva.Add(idLoja, new Dictionary<int, float> { { (int)prod.IdProd, m2 ? m2Calc : qtdEstornoEstoque } });
                        else if (!idsProdQtdeReserva[idLoja].ContainsKey((int)prod.IdProd))
                            idsProdQtdeReserva[idLoja].Add((int)prod.IdProd, m2 ? m2Calc : qtdEstornoEstoque);
                        else
                            idsProdQtdeReserva[idLoja][(int)prod.IdProd] += m2 ? m2Calc : qtdEstornoEstoque;

                        #endregion
                }
            }

            if (lstProd != null && lstProd.Length > 0)
            {
                // Ajusta o campo RESERVA do produto loja.
                foreach (var idLojaReserva in idsProdQtdeReserva.Keys)
                    if (idsProdQtdeReserva[idLojaReserva].Count > 0)
                        ProdutoLojaDAO.Instance.ColocarReserva(session, idLojaReserva, idsProdQtdeReserva[idLojaReserva], null,
                            (int)idLiberarPedido, null, null, null, null, null, "LiberarPedidoDAO - CancelarLiberacao");

                // Ajusta o campo LIBERACAO do produto loja.
                foreach (var idLojaLiberacao in idsProdQtdeLiberacao.Keys)
                    if (idsProdQtdeLiberacao[idLojaLiberacao].Count > 0)
                        ProdutoLojaDAO.Instance.TirarLiberacao(session, idLojaLiberacao, idsProdQtdeLiberacao[idLojaLiberacao], null,
                            (int)idLiberarPedido, null, null, null, null, null, "LiberarPedidoDAO - CancelarLiberacao");
            }

            #endregion

            #region Atualiza o saldo das obras

            foreach (var id in IdsPedidos(session, idLiberarPedido.ToString()).Split(','))
            {
                if (string.IsNullOrEmpty(id))
                    continue;

                var idObra = PedidoDAO.Instance.GetIdObra(session, id.StrParaUint());
                if (idObra > 0)
                    ObraDAO.Instance.AtualizaSaldo(session, null, idObra.Value, false, false);
            }

            #endregion

            #region Altera a situa��o para n�o entregue de pedidos de revenda

            if (FinanceiroConfig.DadosLiberacao.MarcaPedidoRevendaEntregueAoLiberar &&
                Liberacao.Estoque.SaidaEstoqueBoxLiberar && PedidoConfig.DadosPedido.BloquearItensTipoPedido)
                objPersistence.ExecuteCommand(session, @"
                                Update pedido set situacaoProducao=" + (int)Pedido.SituacaoProducaoEnum.NaoEntregue +
                                                      @" 
                                Where tipoPedido=" + (int)Pedido.TipoPedidoEnum.Revenda + @" 
                                    And situacaoProducao=" + (int)Pedido.SituacaoProducaoEnum.Entregue + @"
                                    And idPedido In (Select idPedido From produtos_liberar_pedido Where idLiberarPedido=" +
                                                      idLiberarPedido + ")"
                    );

            #endregion

            // Verifica se os pedidos est�o entregues
            if (ProducaoConfig.ExpedirSomentePedidosLiberados)
            {
                var pedidosEntregues = ExecuteMultipleScalar<string>(session, @"Select idPedido From pedido Where situacaoProducao=" + (int)Pedido.SituacaoProducaoEnum.Entregue + @"
                                        And idPedido In (Select idPedido From produtos_liberar_pedido Where idLiberarPedido=" + idLiberarPedido + ")");

                if (pedidosEntregues.Count() > 0)
                    throw new Exception("O(s) pedido(s) " + string.Join(",", pedidosEntregues) + " esta(�o) entregue(s).");
            }

            // Estorna movimenta��es do funcion�rio
            foreach (var ped in lstPedidos)
                if (ped.VendidoFuncionario)
                    PedidoDAO.Instance.EstornaMovFunc(session, ped.IdPedido, ped.IdFuncVenda.Value);

            /* Chamado 39395. */
            if (objPersistence.ExecuteSqlQueryCount(session,
                string.Format("SELECT COUNT(*) FROM produtos_liberar_pedido WHERE QtdeCalc IS NOT NULL AND QtdeCalc>0 AND IdLiberarPedido={0}", idLiberarPedido)) > 0)
                throw new Exception("N�o foi poss�vel cancelar os produtos da libera��o. Tente novamente, caso o problema persista entre em contato com o suporte WebGlass.");

            var idsPedidoLiberado = PedidoDAO.Instance.GetIdsByLiberacao(session, idLiberarPedido);
            if (idsPedidoLiberado.Any())
                CarregamentoDAO.Instance.AlterarSituacaoFaturamentoCarregamentos(session, idsPedidoLiberado);

            LogCancelamentoDAO.Instance.LogLiberarPedido(session, liberacaoPedido,
                obs.Substring(obs.ToLower().IndexOf("motivo do cancelamento: ", StringComparison.Ordinal) +
                "motivo do cancelamento: ".Length), true);
        }

        #endregion

        #region Verifica se produtos do pedido sendo liberado possui estoque

        /// <summary>
        /// Verifica se produtos do pedido sendo liberado possui estoque
        /// </summary>
        public bool PedidoPossuiEstoque(uint[] idsProdutosPedido, float[] qtdeLiberar, out string mensagem)
        {
            return PedidoPossuiEstoque(null, idsProdutosPedido, qtdeLiberar, out mensagem);
        }

        /// <summary>
        /// Verifica se produtos do pedido sendo liberado possui estoque
        /// </summary>
        public bool PedidoPossuiEstoque(GDASession session, uint[] idsProdutosPedido, float[] qtdeLiberar, out string mensagem)
        {
            mensagem = String.Empty;

            List<uint> lstIdPedidos = new List<uint>();
            Dictionary<uint, float> dicProduto = new Dictionary<uint, float>();
            var idLoja = 0;

            // Soma a quantidade a ser liberada dos produtos iguais
            for (int i = 0; i < idsProdutosPedido.Length; i++)
            {
                // Verifica se a loja deste produto_pedido � a mesma do foreach mais acima
                uint idPedido = ProdutosPedidoDAO.Instance.ObtemIdPedido(session, idsProdutosPedido[i]);
                idLoja = (int)PedidoDAO.Instance.ObtemIdLoja(session, idPedido);

                uint idProd = ProdutosPedidoDAO.Instance.ObtemIdProd(session, idsProdutosPedido[i]);

                var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(session, (int)idProd);
                var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(session, (int)idProd);
                var subGrupoVolume = SubgrupoProdDAO.Instance.IsSubgrupoGeraVolume(session, (uint)idGrupoProd, ((uint?)idSubgrupoProd).GetValueOrDefault());
                var entregaBalcao = PedidoDAO.Instance.ObtemTipoEntrega(session, idPedido) == (int)Pedido.TipoEntregaPedido.Balcao;
                var volumeApenasDePedidosEntrega = OrdemCargaConfig.GerarVolumeApenasDePedidosEntrega;

                var naoVolume = volumeApenasDePedidosEntrega ? entregaBalcao || !subGrupoVolume : !subGrupoVolume;

                /* Chamado 46018.
                 * Caso o estoque do produto tenha que ser baixado atrav�s de um volume e ele j� tiver sido expedido, n�o verifica o estoque dele.
                 * A configura��o UsarControleOrdemCarga � utilizada somente para constatar que n�o ser�o permitidas libera��es parciais. */
                if (OrdemCargaConfig.UsarControleOrdemCarga && !naoVolume && ProdutosPedidoDAO.Instance.ObtemQtdSaida(idsProdutosPedido[i]) == qtdeLiberar[i])
                    continue;

                int tipoCalculo = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session, (int)idProd);
                float qtdALiberar = qtdeLiberar[i];

                if (tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto)
                {
                    float totM2 = ProdutosPedidoDAO.Instance.ObtemTotM(session, idsProdutosPedido[i]);
                    float qtde = ProdutosPedidoDAO.Instance.ObtemQtde(session, idsProdutosPedido[i]);
                    qtdALiberar = (totM2 / qtde) * qtdALiberar;
                }
                else if (tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                    tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 ||
                    tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
                {
                    float altura = ProdutosPedidoDAO.Instance.ObtemValorCampo<float>(session, "altura", "idProdPed=" + idsProdutosPedido[i]);
                    qtdALiberar = qtdALiberar * altura;
                }
                else
                {
                    // Subtrai a quantidade de pe�as j� entregues, observando se o produto j� foi inserido no dicion�rio ou se o pedido
                    // do mesmo j� foi inserido na listagem
                    if (PedidoConfig.LiberarPedido && PedidoDAO.Instance.GetTipoPedido(session, idPedido) == Pedido.TipoPedidoEnum.Revenda &&
                        (!dicProduto.ContainsKey(idProd) || !lstIdPedidos.Contains(idPedido)))
                        qtdALiberar -= ProdutoPedidoProducaoDAO.Instance.ObtemQtdLiberadaRevenda(session, idPedido, idProd);
                }

                // Salva os pedidos j� inseridos pra fazer o controle de quantidae de pe�as de estoque j� expedidas acima
                if (!lstIdPedidos.Contains(idPedido))
                    lstIdPedidos.Add(idPedido);

                if (!dicProduto.ContainsKey(idProd))
                    dicProduto.Add(idProd, qtdALiberar);
                else
                    dicProduto[idProd] += qtdALiberar;
            }

            // Verifica se h� estoque dispon�vel para os produtos sendo liberados
            foreach (KeyValuePair<uint, float> item in dicProduto)
            {
                var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(session, (int)item.Key);
                var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(session, (int)item.Key);
                var qtdLiberar = item.Value;

                if (GrupoProdDAO.Instance.BloquearEstoque(session, idGrupoProd, idSubgrupoProd))
                {
                    var tipoCalculo = idSubgrupoProd.GetValueOrDefault() == 0 ?
                        GrupoProdDAO.Instance.ObtemTipoCalculo(session, idGrupoProd, false) :
                        SubgrupoProdDAO.Instance.ObtemTipoCalculo(session, idSubgrupoProd.Value, false);
                    var tipoCalculoM2 =
                        tipoCalculo == (int)TipoCalculoGrupoProd.M2 ||
                            tipoCalculo == (int)TipoCalculoGrupoProd.M2Direto;
                    var produtoVidro = ProdutoDAO.Instance.IsVidro(session, (int)item.Key);

                    /* Chamado 21996. */
                    //float qtdEstoqueReal = ProdutoLojaDAO.Instance.GetEstoque((uint)loja.IdLoja, item.Key, null, true);
                    /* Chamado 22931. */
                    /*float qtdEstoqueReal =
                        ProdutoLojaDAO.Instance.GetEstoque((uint)loja.IdLoja, item.Key, produtoVidro && tipoCalculoM2);*/
                    float qtdEstoqueReal = ProdutoLojaDAO.Instance.GetEstoque(session, (uint)idLoja, item.Key, null, false, true, produtoVidro && tipoCalculoM2);

                    // Verifica se o estoque real � suficiente para liberar a pe�a,
                    // esta situa��o ocorre somente quando o controle de estoque n�o est� bloqueando.
                    if (qtdEstoqueReal < qtdLiberar)
                    {
                        mensagem = "A libera��o n�o pode ser realizada pois o produto " +
                            ProdutoDAO.Instance.GetCodInterno(session, (int)item.Key) + " - " +
                            ProdutoDAO.Instance.ObtemDescricao(session, (int)item.Key) + " n�o possui estoque dispon�vel.";

                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region Verifica se a libera��o existe

        /// <summary>
        /// Verifica se a libera��o existe
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public bool LiberacaoExists(uint idLiberarPedido)
        {
            string sql = "Select Count(*) From liberarpedido Where idLiberarPedido=" + idLiberarPedido;

            return objPersistence.ExecuteSqlQueryCount(sql, null) > 0;
        }

        #endregion

        #region Verifica se a libera��o est� aberta

        /// <summary>
        /// Verifica se a libera��o est� aberta.
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public bool IsLiberacaoAberta(uint idLiberarPedido)
        {
            return objPersistence.ExecuteSqlQueryCount("select count(*) from liberarpedido where idLiberarPedido=" + idLiberarPedido +
                " and situacao=" + (int)LiberarPedido.SituacaoLiberarPedido.Liberado) > 0;
        }

        #endregion

        #region Verifica se a libera��o � � vista

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transa��o)
        /// Verifica se a libera��o � � vista.
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public bool IsLiberacaoAVista(uint idLiberarPedido)
        {
            return IsLiberacaoAVista(null, idLiberarPedido);
        }

        /// <summary>
        /// Verifica se a libera��o � � vista.
        /// </summary>
        public bool IsLiberacaoAVista(GDASession sessao, uint idLiberarPedido)
        {
            return objPersistence.ExecuteSqlQueryCount(sessao, "select count(*) from liberarpedido where idLiberarPedido=" + idLiberarPedido +
                " and tipoPagto=" + (int)LiberarPedido.TipoPagtoEnum.AVista) > 0;
        }

        #endregion

        #region Verifica se um pedido est� liberado

        /// <summary>
        /// Retorna o comando SQL executado para verificar se um pedido est� liberado.
        /// </summary>
        private string SqlPedidoLiberado(uint idPedido, string idLiberarPedido)
        {
            // Se a quantidade de produtos no pedido for menor ou igual � quantidade de produtos liberados,
            string sql = @"
                select coalesce(min(pp.qtde<=plp.qtde),0)
                from (
	                select idPedido, idLiberarPedido, sum(qtde) as qtde
	                from produtos_liberar_pedido
                    where qtdeCalc>0 {1}
                    group by idPedido" + (!String.IsNullOrEmpty(idLiberarPedido) ? ", idLiberarPedido" : "") + @"
                ) as plp, (
                    select pp.idPedido, sum(if(ped.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + 
                        @", coalesce(ape.qtde*ppe.qtde, ap.qtde*pp.qtde), pp.qtde)) as qtde
                    from produtos_pedido pp
                        left join ambiente_pedido ap on (pp.idAmbientePedido=ap.idAmbientePedido)
                        left join produtos_pedido_espelho ppe on (pp.idProdPedEsp=ppe.idProdPed)
                        left join ambiente_pedido_espelho ape on (ppe.idAmbientePedido=ape.idAmbientePedido)
                        inner join pedido ped on (pp.idPedido=ped.idPedido)
                    where pp.IdProdPedParent IS NULL
                        and (pp.invisivel{0}=false or pp.invisivel{0} is null) {2}
                    group by pp.idPedido
                ) as pp
                where plp.idPedido=pp.idPedido";

            string where1 = "", where2 = "";
            if (idPedido > 0)
            {
                where1 += " and idPedido=" + idPedido;
                where2 += " and pp.idPedido=" + idPedido;
            }

            if (!String.IsNullOrEmpty(idLiberarPedido))
            {
                where1 += " and idLiberarPedido in (" + idLiberarPedido.TrimEnd(',') + ")";
                where2 += " and pp.idPedido in (select * from (select distinct idPedido from produtos_liberar_pedido " +
                    "where idLiberarPedido in (" + idLiberarPedido.TrimEnd(',') + ")) as temp)";

                if (PCPConfig.UsarConferenciaFluxo)
                    where2 += " and (pp.idProdPedEsp is null or ppe.idProdPed is not null)";
            }

            return String.Format(sql, PCPConfig.UsarConferenciaFluxo ? "Fluxo" : "Pedido", where1, where2);
        }

        /// <summary>
        /// Verifica se um pedido j� foi liberado totalmente.
        /// </summary>
        public bool IsPedidoLiberado(GDASession sessao, uint idPedido)
        {
            idPedido = PedidoReposicaoDAO.Instance.GetPedidoOriginal(sessao, idPedido);

            if (PedidoDAO.Instance.ObtemSituacao(sessao, idPedido) == Pedido.SituacaoPedido.Confirmado)
                return true;

            /* Chamado 46495. */
            var produtosNaoLiberados = ProdutosPedidoDAO.Instance.GetForLiberacao(sessao, idPedido.ToString(), false);

            return produtosNaoLiberados == null || produtosNaoLiberados.Count() == 0;
        }

        #endregion

        #region Verifica se um produto de um pedido est� liberado

        /// (APAGAR: quando alterar para utilizar transa��o)
        /// <summary>
        /// Verifica se um produto est� liberado (usado para libera��es parciais).
        /// </summary>
        public bool IsProdutoPedidoLiberado(uint? idProdPed, uint? idProdPedEsp, uint idProdPedProducao)
        {
            return IsProdutoPedidoLiberado(null, idProdPed, idProdPedEsp, idProdPedProducao);
        }

        /// <summary>
        /// Verifica se um produto est� liberado (usado para libera��es parciais).
        /// </summary>
        public bool IsProdutoPedidoLiberado(GDASession sessao, uint? idProdPed, uint? idProdPedEsp, uint idProdPedProducao)
        {
            string sql;

            // N�o deve verificar se a pe�a quebrada est� liberada, deve verificar se a pe�a nova foi liberada, pois pode acontecer
            // de liberar a pe�a nova sem que a mesma tenha sido liberada.

            //idProdPed = PedidoReposicaoDAO.Instance.GetProdPedEspOriginal(idProdPed);

            //// Se pedido estiver liberado retorna true
            //string sql = "select count(*) from pedido where idPedido=(select idPedido from produtos_pedido_espelho where idProdPed=" + idProdPed + 
            //    ") and situacao=" + (int)Pedido.SituacaoPedido.Confirmado;

            //if (objPersistence.ExecuteSqlQueryCount(sql) > 0)
            //    return true;

            // Se a etiqueta estiver liberada retorna true
            if (idProdPedProducao > 0)
            {
                sql = "select count(*) from produtos_liberar_pedido where qtdeCalc>0 and idProdPedProducao=" + idProdPedProducao;
                if (objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0)
                    return true;
            }

            string numEtiqueta = ProdutoPedidoProducaoDAO.Instance.ObtemEtiqueta(sessao, idProdPedProducao);
            int item = !String.IsNullOrEmpty(numEtiqueta) ? numEtiqueta.Split('-', '.', '/')[2].StrParaInt() : 0;

            string itemEtiqueta = String.Format("cast(substr({0}, 1, instr({0}, '/') - 1) as signed)",
                "substr(coalesce(ppp.numEtiquetaCanc, ppp.numEtiqueta), instr(coalesce(ppp.numEtiquetaCanc, ppp.numEtiqueta), '.') + 1)");

            sql = @"select sum(qtdeCalc) from (
                select distinct plp.* from produtos_liberar_pedido plp
                inner join produtos_pedido pp on (plp.idProdPed=pp.idProdPed)
                left join produto_pedido_producao ppp on (pp.idProdPedEsp=ppp.idProdPed and 
                    (ppp.idProdPedProducao is null or ppp.idProdPedProducao=plp.idProdPedProducao))
                where (plp.idProdPedProducao is null or " + itemEtiqueta + "<" + item +
                ") and pp.idProdPed" + (idProdPed > 0 ? "=" + idProdPed : "Esp=" + idProdPedEsp) + ") as temp";

            return ExecuteScalar<int>(sessao, sql) >= item && numEtiqueta != null;
        }

        #endregion

        #region Verifica se um pedido pode ser cancelado

        /// <summary>
        /// Verifica se h� algum produto liberado para o pedido.
        /// </summary>
        /// <param name="idPedido">O id do pedido.</param>
        /// <returns></returns>
        public bool PodeCancelarPedido(uint idPedido)
        {
            return PodeCancelarPedido(null, idPedido);
        }

        /// <summary>
        /// Verifica se h� algum produto liberado para o pedido.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idPedido">O id do pedido.</param>
        /// <returns></returns>
        public bool PodeCancelarPedido(GDASession session, uint idPedido)
        {
            if (!PedidoConfig.LiberarPedido)
                return true;

            string sql = "select count(*) from liberarpedido lp " + 
                "Inner Join produtos_liberar_pedido plp On (plp.idLiberarPedido=lp.idLiberarPedido) " +
                "Where plp.idPedido=" + idPedido + " And lp.situacao=" + (int)LiberarPedido.SituacaoLiberarPedido.Liberado;

            return objPersistence.ExecuteScalar(session, sql).ToString().StrParaInt() == 0;
        }

        #endregion

        #region C�lculo de ICMS de uma libera��o

        private string SqlIcms(string idLiberarPedido)
        {
            string campos = String.IsNullOrEmpty(idLiberarPedido) ? "lp1.idLiberarPedido, " : "";
            string where = !String.IsNullOrEmpty(idLiberarPedido) ? "where lp1.idLiberarPedido=" + idLiberarPedido : "";

            return "select " + campos + @"cast(coalesce(round(sum((coalesce(if(p.valorIcms>0 Or (p.valorIcms>0 And c.cobrarIcmsSt),pp.ValorIcms,0),0)/pp.qtde)*
                    plp.qtdeCalc), 2), 0) as decimal(12,2)) as ValorIcms 
                from liberarpedido lp1
                    inner join cliente c On (lp1.idCliente=c.id_Cli)
                    left join produtos_liberar_pedido plp on (lp1.idLiberarPedido=plp.idLiberarPedido) 
                    left join produtos_pedido pp on (plp.idProdPed=pp.idProdPed)
                    left join pedido p On (pp.idPedido=p.idPedido)
                " + where + @"
                group by lp1.idLiberarPedido";
        }
        
        /// <summary>
        /// Retorna o valor do ICMS de uma libera��o.
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public decimal GetValorIcms(uint idLiberarPedido)
        {
            return ExecuteScalar<decimal>(SqlIcms(idLiberarPedido.ToString()));
        }

        #endregion

        #region Recupera o ID de uma libera��o para uma lista de pedidos

        /// <summary>
        /// Recupera o ID de uma libera��o para uma lista de pedidos.
        /// </summary>
        public uint? GetIdLiberacao(string idsPedidos)
        {
            return GetIdLiberacao(null, idsPedidos);
        }

        /// <summary>
        /// Recupera o ID de uma libera��o para uma lista de pedidos.
        /// </summary>
        public uint? GetIdLiberacao(GDASession session, string idsPedidos)
        {
            if (idsPedidos.Length == 0)
                return null;

            if (objPersistence.ExecuteSqlQueryCount(session, "select count(distinct plp.idLiberarPedido) from produtos_liberar_pedido plp " +
                "Inner Join liberarpedido lp On (plp.idLiberarPedido=lp.idLiberarPedido) where lp.situacao=" + (int)LiberarPedido.SituacaoLiberarPedido.Liberado + 
                " And idPedido in (" + idsPedidos + ")") > 1)
                return null;

            object retorno = objPersistence.ExecuteScalar(session, "select plp.idLiberarPedido from produtos_liberar_pedido plp " +
                "Inner Join liberarpedido lp On (plp.idLiberarPedido=lp.idLiberarPedido) where lp.situacao=" + (int)LiberarPedido.SituacaoLiberarPedido.Liberado + 
                " And idPedido in (" + idsPedidos + ")");

            return retorno != null && retorno.ToString() != "" ? retorno.ToString().StrParaUint() : 0;
        }

        #endregion

        #region Recupera o ID de uma libera��o para a impress�o de boletos

        /// <summary>
        /// Recupera o ID de uma libera��o para a impress�o de boletos.
        /// </summary>
        public int? ObterIdLiberarPedidoParaImpressaoBoletoNFe(int idPedido, int idNf)
        {
            if (idPedido == 0 || idNf == 0)
                return null;

            object retorno = objPersistence.ExecuteScalar(string.Format(@"SELECT plp.IdLiberarPedido FROM produtos_liberar_pedido plp
                    INNER JOIN liberarpedido lp ON (plp.IdLiberarPedido=lp.IdLiberarPedido)
                    INNER JOIN pedidos_nota_fiscal pnf ON (plp.IdLiberarPedido=lp.IdLiberarPedido)
                WHERE lp.Situacao={0} AND plp.IdPedido={1} AND pnf.IdNf={2}", (int)LiberarPedido.SituacaoLiberarPedido.Liberado, idPedido, idNf));

            return retorno != null && retorno.ToString() != string.Empty ? retorno.ToString().StrParaInt() : 0;
        }

        #endregion

        #region Recupera os pedidos de uma lista de libera��es

        /// <summary>
        /// Recupera os pedidos de uma lista de libera��es.
        /// </summary>
        public string IdsPedidos(GDASession sessao, string idsLiberacoes)
        {
            if (String.IsNullOrEmpty(idsLiberacoes))
                return String.Empty;

            object retorno = objPersistence.ExecuteScalar(sessao, "select cast(group_concat(distinct idPedido SEPARATOR ', ') as char) from " +
                "produtos_liberar_pedido where idLiberarPedido in (" + idsLiberacoes + ")");

            return retorno != null ? retorno.ToString() : String.Empty;
        }

        #endregion

        #region Recupera uma lista de libera��es que cont�m um pedido

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transa��o)
        /// Recupera uma lista de libera��es que cont�m um pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public IList<uint> GetIdsLiberacaoByPedido(uint idPedido)
        {
            return GetIdsLiberacaoByPedido(null, idPedido);
        }

        /// <summary>
        /// Recupera uma lista de libera��es que cont�m um pedido.
        /// </summary>
        public IList<uint> GetIdsLiberacaoByPedido(GDASession sessao, uint idPedido)
        {
            string sql = "select * from liberarpedido where idLiberarPedido in (select idLiberarPedido from produtos_liberar_pedido where idPedido=" + idPedido + ")";
            return objPersistence.LoadResult(sessao, sql, null).Select(f => f.GetUInt32(0))
                       .ToList();
        }

        /// <summary>
        /// Recupera uma lista de libera��es que cont�m um pedido.
        /// </summary>
        public IList<uint> GetIdsLiberacaoAtivaByPedido(GDASession sessao, uint idPedido)
        {
            string sql = "select * from liberarpedido " +
                            "where idLiberarPedido in (select idLiberarPedido from produtos_liberar_pedido where idPedido=" + idPedido + ")" +
                            " and situacao=" + (int)LiberarPedido.SituacaoLiberarPedido.Liberado;
            return objPersistence.LoadResult(sessao, sql, null).Select(f => f.GetUInt32(0))
                       .ToList();
        }

        /// <summary>
        /// Recupera uma lista de libera��es que cont�m um pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public IList<uint> GetIdsLiberacaoAtivaByPedido(uint idPedido)
        {
            return GetIdsLiberacaoAtivaByPedido(null, idPedido);
        }


        #endregion

        #region Verifica se uma libera��o foi parcial

        /// <summary>
        /// Verifica se em uma libera��o houve alguma libera��o parcial de pedido.
        /// </summary>
        public bool IsLiberacaoParcial(uint idLiberarPedido)
        {
            return IsLiberacaoParcial(null, idLiberarPedido);
        }

        /// <summary>
        /// Verifica se em uma libera��o houve alguma libera��o parcial de pedido.
        /// </summary>
        public bool IsLiberacaoParcial(GDASession session, uint idLiberarPedido)
        {
            string idsPedido = String.Empty;
            foreach (uint id in PedidoDAO.Instance.GetIdsByLiberacao(session, idLiberarPedido))
                idsPedido += id + ",";
            idsPedido = idsPedido.TrimEnd(',');

            if (string.IsNullOrEmpty(idsPedido) || string.IsNullOrWhiteSpace(idsPedido))
                return false;

            // Antes de executar o sql abaixo, verifica pedido a pedido se foram liberados em mais de uma libera��o 
            // e se est�o na situa��o "Liberado", pois se possuirem apenas uma libera��o e estiverem confirmados,
            // quer dizer que n�o foram liberados parcialmente, a id�ia � retirar esta verifica��o depois de fazer o pedido
            // m�o de obra funcionar igual pedido de venda e revenda.
            if (ExecuteScalar<bool>(session, @"
                Select Count(*)>0 From pedido 
                Where idPedido In (" + idsPedido + @")
                    And situacao=" + (int)Pedido.SituacaoPedido.LiberadoParcialmente))
                return true;
            else if (ExecuteScalar<bool>(session, @"
                Select Count(*)=1 From liberarpedido
                Where idLiberarPedido In (
                    Select idLiberarPedido From produtos_liberar_pedido 
                    Where idPedido In (" + idsPedido + @") 
                        And qtdeCalc>0
                )
                    And situacao=" + (int)LiberarPedido.SituacaoLiberarPedido.Liberado))
                return false;

            return ExecuteScalar<int>(session, SqlPedidoLiberado(0, idLiberarPedido.ToString())) == 0;
        }

        /// <summary>
        /// Verifica se um pedido foi liberado parcialmente em uma libera��o de pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idsLiberarPedidos"></param>
        /// <returns></returns>
        public bool IsPedidoLiberadoParcialmente(uint idPedido, string idsLiberarPedidos)
        {
            return ExecuteScalar<int>(SqlPedidoLiberado(idPedido, idsLiberarPedidos)) == 0;
        }

        #endregion

        #region Obt�m dados da libera��o

        /// <summary>
        /// Recupera o ID do cliente de uma libera��o.
        /// </summary>
        public uint GetIdCliente(uint idLiberarPedido)
        {
            return GetIdCliente(null, idLiberarPedido);
        }

        /// <summary>
        /// Recupera o ID do cliente de uma libera��o.
        /// </summary>
        public uint GetIdCliente(GDASession session, uint idLiberarPedido)
        {
            string sql = "select idCliente from liberarpedido where idLiberarPedido=" + idLiberarPedido;
            object retorno = objPersistence.ExecuteScalar(session, sql);
            return retorno != null && retorno != DBNull.Value && retorno.ToString() != "" ? retorno.ToString().StrParaUint() : 0;
        }

        public DateTime ObtemDataLiberacao(uint idLiberarPedido)
        {
            return ObtemValorCampo<DateTime>("dataLiberacao", "idLiberarPedido=" + idLiberarPedido);
        }

        /// <summary>
        /// Recupera a data da primeira libera��o do pedido.
        /// </summary>
        public DateTime? ObterDataPrimeiraLiberacaoPedido(GDASession session, uint idPedido)
        {
            if (idPedido == 0)
                return null;

            return ExecuteScalar<DateTime?>(session, string.Format(@"SELECT lp.DataLiberacao FROM liberarpedido lp
	                INNER JOIN produtos_liberar_pedido plp ON (lp.IdLiberarPedido=plp.IdLiberarPedido)
                WHERE plp.IdPedido={0} AND lp.Situacao={1} ORDER BY lp.DataLiberacao LIMIT 1;", idPedido, (int)LiberarPedido.SituacaoLiberarPedido.Liberado));
        }

        public int ObtemTipoPagto(uint idLiberarPedido)
        {
            return ObtemTipoPagto(null, idLiberarPedido);
        }

        public int ObtemTipoPagto(GDASession session, uint idLiberarPedido)
        {
            return ObtemValorCampo<int>(session, "tipoPagto", "IdLiberarPedido=" + idLiberarPedido);
        }

        public uint ObtemIdLoja(uint idLiberarPedido)
        {
            return ExecuteScalar<uint>(@"select f.idLoja from liberarpedido lp 
                inner join funcionario f on (lp.idFunc=f.idFunc) where lp.idLiberarPedido=" + idLiberarPedido);
        }

        public LiberarPedido.SituacaoLiberarPedido ObterSituacao(GDASession session, int idLiberarPedido)
        {
            return ObtemValorCampo<LiberarPedido.SituacaoLiberarPedido>(session, "Situacao", "IdLiberarPedido=" + idLiberarPedido);
        }

        /// <summary>
        /// Obt�m as lojas das libera��es
        /// </summary>
        public string ObtemIdsLojas(string idsLiberarPedidos)
        {
            if (string.IsNullOrWhiteSpace(idsLiberarPedidos))
                return string.Empty;

            var sql = string.Format(@"select distinct f.IdLoja as IdLoja from liberarpedido lp 
                inner join funcionario f on (lp.idFunc=f.idFunc) where lp.idLiberarPedido in ({0})", idsLiberarPedidos);

            var resultado = string.Empty;

            foreach (var record in this.CurrentPersistenceObject.LoadResult(sql, null))
            {
                resultado += record["IdLoja"].ToString() + ",";
            }

            return resultado.TrimEnd(',');
        }

        public int ObterIdNf(GDASession sessao, int idLiberarPedido)
        {
            return ExecuteScalar<int>(sessao, "SELECT pnf.IdNf FROM pedidos_nota_fiscal pnf WHERE pnf.IdLiberarPedido = " + idLiberarPedido);
        }

        #endregion

        #region Recupera descontos de libera��es

        /// <summary>
        /// Recupera descontos de libera��es
        /// </summary>
        /// <param name="idsLiberacao"></param>
        /// <returns></returns>
        public decimal GetDescontos(string idsLiberacao)
        {
            if (String.IsNullOrEmpty(idsLiberacao))
                return 0;

            string sql = "Select Sum(Coalesce(desconto, 0)) From liberarpedido Where idLiberarPedido In (" + idsLiberacao.Trim(',') + ")";
            return ExecuteScalar<decimal>(sql);
        }

        #endregion

        #region Exibir nota promiss�ria?

        /// <summary>
        /// A nota promiss�ria deve ser exibida?
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public bool ExibirNotaPromissoria(uint idLiberarPedido)
        {
            int tipoPagto = ObtemValorCampo<int>("tipoPagto", "idLiberarPedido=" + idLiberarPedido);
            int situacao = ObtemValorCampo<int>("situacao", "idLiberarPedido=" + idLiberarPedido);
            return ExibirNotaPromissoria(tipoPagto, situacao);
        }

        internal bool ExibirNotaPromissoria(int tipoPagto, int situacao)
        {
            return PedidoConfig.LiberarPedido && 
                FinanceiroConfig.DadosLiberacao.NumeroViasNotaPromissoria > 0 && 
                tipoPagto == (int)LiberarPedido.TipoPagtoEnum.APrazo && 
                situacao == (int)LiberarPedido.SituacaoLiberarPedido.Liberado && 
                Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento);
        }

        #endregion

        #region Tem nota fiscal gerada?
        /// <summary>
        /// Verifica se a libera��o possui nota fiscal gerada.
        /// </summary>
        /// <param name="idLiberarPedido"></param>
        /// <returns></returns>
        public bool TemNfe(uint idLiberarPedido)
        {
            return objPersistence.ExecuteSqlQueryCount("Select Count(*) From pedidos_nota_fiscal Where idLiberarPedido=" + idLiberarPedido) > 0;
        }

        #endregion

        #region Recupera o Total de Pedidos Liberados

        public string GetTotalLiberado(string dtIni, string dtFim)
        {
            string sql = @"Select sum(lp.total) From liberarpedido lp 
                Where 1  And lp.dataLiberacao>=?dataIni 
                And lp.dataLiberacao<=?dataFim and lp.situacao=1;";

            return objPersistence.ExecuteScalar(sql, new GDAParameter("?dataIni", DateTime.Parse(dtIni + " 00:00:00")), new GDAParameter("?dataFim", DateTime.Parse(dtFim + " 23:59:59"))).ToString();
        }

        public string GetTotalCustoProdutoLiberado(string dtIni, string dtFim)
        {
            string sql = @"Select sum(p.CustoCompra) From liberarpedido lp 
                inner join produtos_liberar_pedido pl on(lp.IdLiberarPedido=pl.IdLiberarPedido)
                inner join produtos_pedido pp on(pl.IdProdPed=pp.IdProdPed)
                inner join produto p on(p.IdProd=pp.IdProd) 
                Where 1  And lp.dataLiberacao>=?dataIni 
                And lp.dataLiberacao<=?dataFim and lp.situacao=1;";

            return objPersistence.ExecuteScalar(sql, new GDAParameter("?dataIni", DateTime.Parse(dtIni + " 00:00:00")), new GDAParameter("?dataFim", DateTime.Parse(dtFim + " 23:59:59"))).ToString();
        }

        public float GetTotalLiberado(string idsLiberacoes)
        {
            if (string.IsNullOrEmpty(idsLiberacoes))
                return 0;

            string sql = @"select sum(lp.total) from liberarpedido lp
                            where lp.idliberarpedido in (" + idsLiberacoes + ") and lp.situacao=1;";

            return Convert.ToSingle(objPersistence.ExecuteScalar(sql));
        }

        #endregion

        #region Verifica se a libera��o possui pedidos de reposi��o

        public bool ContemPedidosReposicao(GDASession sessao, uint idLiberarPedido)
        {
            string idsPedidos = ProdutosLiberarPedidoDAO.Instance.GetValoresCampo(sessao,
                "select idPedido from produtos_liberar_pedido where idLiberarPedido=" + idLiberarPedido,
                "idPedido", ",");

            return PedidoDAO.Instance.ContemPedidoReposicao(sessao, idsPedidos);
        }

        #endregion

        #region Verifica se h� nota fiscal n�o cancelada/inutilizada para uma libera��o

        /// <summary>
        /// Verifica se h� nota fiscal n�o cancelada/inutilizada para uma libera��o.
        /// </summary>
        public bool PossuiNotaFiscalAtiva(uint idLiberarPedido)
        {
            return PossuiNotaFiscalAtiva(null, idLiberarPedido);
        }

        /// <summary>
        /// Verifica se h� nota fiscal n�o cancelada/inutilizada para uma libera��o.
        /// </summary>
        public bool PossuiNotaFiscalAtiva(GDASession session, uint idLiberarPedido)
        {
            // Verifica se h� alguma nota fiscal ativa para a libera��o passada
            string sql = @"select count(*) from nota_fiscal nf
	            inner join pedidos_nota_fiscal pnf on (nf.idNf=pnf.idNf)
            where pnf.idLiberarPedido=" + idLiberarPedido + " and nf.situacao not in (" +
                (int)NotaFiscal.SituacaoEnum.Cancelada + "," + (int)NotaFiscal.SituacaoEnum.Denegada + "," + (int)NotaFiscal.SituacaoEnum.Inutilizada + ")";

            if (objPersistence.ExecuteSqlQueryCount(session, sql) == 0)
                return false;

            // Caso n�o tenha uma nota fiscal ativa, verifica se a mesma possui refer�ncia de conta a receber
            return ExecuteScalar<bool>(session, @"
                Select Count(*)>0 From contas_receber cr
                    inner join pedidos_nota_fiscal pnf on (cr.idNf=pnf.idNf)
                where pnf.idLiberarPedido=" + idLiberarPedido);
        }

        #endregion

        #region M�todos sobrescritos

        public override uint Insert(GDASession session, LiberarPedido objInsert)
        {
            objInsert.IdFunc = UserInfo.GetUserInfo.CodUser;
            objInsert.DataLiberacao = DateTime.Now;

            return base.Insert(session, objInsert);
        }

        public override uint Insert(LiberarPedido objInsert)
        {
            return Insert(null, objInsert);
        }

        #endregion

        #region Valida para expedi��o

        /// <summary>
        /// Valida se uma libera��o pode ser expedida.
        /// </summary>
        /// <param name="idLiberacao"></param>
        public void ValidaLiberacaoParaExpedicaoBalcao(uint idLiberacao)
        {
            if (!LiberacaoExists(idLiberacao))
                throw new Exception("A libera��o informada n�o foi encontrada.");

            if (!IsLiberacaoAberta(idLiberacao))
                throw new Exception("A libera��o informada esta cancelada.");

            //Verifica se a libera��o possui apenas pedidos do tipo entrega balc�o
            var sql = @"
                SELECT count(*)
                FROM liberarPedido lp
	                INNER JOIN produtos_liberar_pedido plp ON (lp.IdLiberarPedido = plp.IdLiberarPedido)
	                INNER JOIN pedido p ON (p.IdPedido = plp.IdPedido)
                WHERE p.tipoEntrega <> " + (int)Pedido.TipoEntregaPedido.Balcao + @" 
	                AND lp.IdLiberarPedido = " + idLiberacao;

            if (objPersistence.ExecuteSqlQueryCount(sql) > 0)
                throw new Exception("A libera��o informada n�o possui apenas pedidos do tipo entrega balc�o.");
        }

        /// <summary>
        /// Verifica se o pedido informado faz parte da libera��o informada.
        /// </summary>
        public bool VerificaPedidoLiberacao(GDASession session, int idLiberacao, int idPedido)
        {
            var sql = @"
                SELECT count(*)
                FROM produtos_liberar_pedido
                WHERE IdPedido = " + idPedido + " AND IdLiberarPedido=" + idLiberacao;

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        #endregion

        #region API

        /// <summary>
        /// Verifica se houve libera��o de pedido apos a data informada
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="dataInicial"></param>
        /// <returns></returns>
        public bool TeveLiberacaoPosterior(GDASession sessao, DateTime dataInicial)
        {
            var sql = @"
                SELECT COUNT(*)
                FROM liberarPedido
                WHERE DataLiberacao > ?dtIni";

            return objPersistence.ExecuteSqlQueryCount(sessao, sql, new GDAParameter("?dtIni", dataInicial)) > 0;
        }

        #endregion
    }
}