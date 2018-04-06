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

namespace Glass.Data.DAL
{
    #region Pedido e totais da Ordem de Carga

    /// <summary>
    /// Classe criada para recuperar a somatória de pedidos, peso, M2 e valor da ordem de carga.
    /// </summary>
    public class PedidoTotaisOrdemCarga
    {
        #region Construtores

        /// <summary>
        /// Construtor com todas as propriedades vazias.
        /// </summary>
        internal PedidoTotaisOrdemCarga()
        {
            Pedido = new Pedido();
            IdOrdemCarga = 0;
            QtdePecasVidro = 0;
            QtdePendente = 0;
            TotM = 0;
            TotM2Pendente = 0;
            Peso = 0;
            PesoPendente = 0;
            ValorTotal = 0;
        }

        /// <summary>
        /// Construtor com todas as propriedades vazias, exceto a propriedade IdOrdemCarga.
        /// </summary>
        internal PedidoTotaisOrdemCarga(int idOrdemCarga)
        {
            Pedido = new Pedido();
            IdOrdemCarga = IdOrdemCarga;
            QtdePecasVidro = 0;
            QtdePendente = 0;
            TotM = 0;
            TotM2Pendente = 0;
            Peso = 0;
            PesoPendente = 0;
            ValorTotal = 0;
        }

        /// <summary>
        /// Construtor completo.
        /// </summary>
        internal PedidoTotaisOrdemCarga(Pedido pedido, int idOrdemCarga, double qtdePecasVidro, double qtdePendente, double totM, double totM2Pendente, double peso, double pesoPendente, decimal valorTotal)
        {
            Pedido = pedido ?? new Pedido();
            Pedido.PesoOC = pedido.IdPedido > 0 ? peso : 0;
            IdOrdemCarga = idOrdemCarga;
            QtdePecasVidro = Math.Round(qtdePecasVidro, 2, MidpointRounding.AwayFromZero);
            QtdePendente = Math.Round(qtdePendente, 2, MidpointRounding.AwayFromZero);
            TotM = Math.Round(totM, 2, MidpointRounding.AwayFromZero);
            TotM2Pendente = Math.Round(totM2Pendente, 2, MidpointRounding.AwayFromZero);
            Peso = peso;
            PesoPendente = Math.Round(pesoPendente, 2, MidpointRounding.AwayFromZero);
            ValorTotal = Math.Round(valorTotal, 2, MidpointRounding.AwayFromZero);
        }

        #endregion

        #region Propriedades

        /// <summary>
        /// Pedido.
        /// </summary>
        public Pedido Pedido { get; set; }

        /// <summary>
        /// Id da ordem de carga
        /// </summary>
        public int IdOrdemCarga { get; set; }

        /// <summary>
        /// Quantidade de peças de vidro da OC.
        /// </summary>
        public double QtdePecasVidro { get; set; }

        /// <summary>
        /// Quantidade de peças pendentes da OC.
        /// </summary>
        public double QtdePendente { get; set; }

        /// <summary>
        /// Total de metro quadrado das peças da OC.
        /// </summary>
        public double TotM { get; set; }

        /// <summary>
        /// Total de metro quadrado pendente das peças da OC.
        /// </summary>
        public double TotM2Pendente { get; set; }

        /// <summary>
        /// Peso total das peças da OC.
        /// </summary>
        public double Peso { get; set; }

        /// <summary>
        /// Peso total pendente das peças da OC.
        /// </summary>
        public double PesoPendente { get; set; }

        /// <summary>
        /// Valor total das peças da OC.
        /// </summary>
        public decimal ValorTotal { get; set; }

        #endregion
    }

    #endregion

    public sealed class PedidoDAO : BaseCadastroDAO<Pedido, PedidoDAO>
    {
        private static object _gerarPedidoLock = new object();
        private static object _inserirPedidoLock = new object();
        private static object _finalizarPedidoLock = new object();

        #region Classe de suporte

        public class TotalListaPedidos
        {
            private readonly double _total, _totM, _peso;

            public double Total
            {
                get { return _total; }
            }

            public double TotM
            {
                get { return _totM; }
            }

            public double Peso
            {
                get { return _peso; }
            }
 
            internal TotalListaPedidos()
            {
                _total = 0;
                _totM = 0;
                _peso = 0;
            }

            internal TotalListaPedidos(string retornoSql)
            {
                if (string.IsNullOrEmpty(retornoSql) || retornoSql.IndexOf(';') == -1)
                    return;

                var dados = retornoSql.Split(';');
                
                _total = Math.Round((double)dados[0].StrParaDecimal(), 2);
                _totM = Math.Round((double)dados[1].StrParaDecimal(), 2);
                _peso = Math.Round((double)dados[2].StrParaDecimal(), 2);
            }
        }

        #endregion

        #region Listagem de Pedidos Padrão

        private string Sql(uint idPedido, uint idLiberarPedido, string idsPedido, string idsLiberarPedidos, uint idLoja, uint idCli,
            string nomeCli, uint idFunc, string codCliente, uint idCidade, string endereco, string bairro, string situacao,
            string situacaoProd, string byVend, string byConf, string vendas, string maoObra, string maoObraEspecial, string producao,
            string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim, string funcFinalizacao, uint idOrcamento,
            bool opcionais, bool infoPedidos, float altura, int largura, int numeroDiasDiferencaProntoLib, float valorDe, float valorAte,
            string tipo, int fastDelivery, int tipoVenda, int origemPedido, string obs, bool selecionar, out string filtroAdicional, out bool temFiltro, string obsLiberacao = "")
        {
            return Sql(idPedido, idLiberarPedido, idsPedido, idsLiberarPedidos, idLoja, idCli, nomeCli, idFunc, codCliente, idCidade,
                endereco, bairro, null, situacao, situacaoProd, byVend, byConf, vendas, maoObra, maoObraEspecial, producao, dataCadIni,
                dataCadFim, dataFinIni, dataFinFim, funcFinalizacao, idOrcamento, opcionais, infoPedidos, altura, largura,
                numeroDiasDiferencaProntoLib, valorDe, valorAte, tipo, fastDelivery, tipoVenda, origemPedido, obs, false, selecionar,
                out filtroAdicional, out temFiltro, obsLiberacao);
        }

        private string Sql(uint idPedido, uint idLiberarPedido, string idsPedido, string idsLiberarPedidos, uint idLoja, uint idCli,
            string nomeCli, uint idFunc, string codCliente, uint idCidade, string endereco, string bairro, string complemento,
            string situacao, string situacaoProd, string byVend, string byConf, string vendas, string maoObra, string maoObraEspecial,
            string producao, string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim, string funcFinalizacao,
            uint idOrcamento, bool opcionais, bool infoPedidos, float altura, int largura, int numeroDiasDiferencaProntoLib,
            float valorDe, float valorAte, string tipo, int fastDelivery, int tipoVenda, int origemPedido, string obs, bool buscarPedidoProducao,
            bool selecionar, out string filtroAdicional, out bool temFiltro, string obsLiberacao = "")
        {
            return Sql(null, idPedido, idLiberarPedido, idsPedido, idsLiberarPedidos, idLoja, idCli, nomeCli, idFunc, codCliente, idCidade,
                endereco, bairro, complemento, situacao, situacaoProd, byVend, byConf, vendas, maoObra, maoObraEspecial, producao,
                dataCadIni, dataCadFim, dataFinIni, dataFinFim, funcFinalizacao, idOrcamento, opcionais, infoPedidos, altura, largura,
                numeroDiasDiferencaProntoLib, valorDe, valorAte, tipo, fastDelivery, tipoVenda, origemPedido, obs, buscarPedidoProducao,
                selecionar, out filtroAdicional, out temFiltro, obsLiberacao);
        }

        private string Sql(GDASession sessao, uint idPedido, uint idLiberarPedido, string idsPedido, string idsLiberarPedidos, uint idLoja,
            uint idCli, string nomeCli, uint idFunc, string codCliente, uint idCidade, string endereco, string bairro, string situacao,
            string situacaoProd, string byVend, string byConf, string vendas, string maoObra, string maoObraEspecial, string producao,
            string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim, string funcFinalizacao, uint idOrcamento,
            bool opcionais, bool infoPedidos, float altura, int largura, int numeroDiasDiferencaProntoLib, float valorDe, float valorAte,
            string tipo, int fastDelivery, int tipoVenda, int origemPedido, string obs, bool selecionar, out string filtroAdicional, out bool temFiltro)
        {
            return Sql(sessao, idPedido, idLiberarPedido, idsPedido, idsLiberarPedidos, idLoja, idCli, nomeCli, idFunc, codCliente,
                idCidade, endereco, bairro, null, situacao, situacaoProd, byVend, byConf, vendas, maoObra, maoObraEspecial, producao, dataCadIni,
                dataCadFim, dataFinIni, dataFinFim, funcFinalizacao, idOrcamento, opcionais, infoPedidos, altura, largura,
                numeroDiasDiferencaProntoLib, valorDe, valorAte, tipo, fastDelivery, tipoVenda, origemPedido, obs, false, selecionar,
                out filtroAdicional, out temFiltro);
        }
        
        private string Sql(GDASession sessao, uint idPedido, uint idLiberarPedido, string idsPedido, string idsLiberarPedidos, uint idLoja,
            uint idCli, string nomeCli, uint idFunc, string codCliente, uint idCidade, string endereco, string bairro, string complemento,
            string situacao, string situacaoProd, string byVend, string byConf, string vendas, string maoObra, string maoObraEspecial,
            string producao, string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim, string funcFinalizacao,
            uint idOrcamento, bool opcionais, bool infoPedidos, float altura, int largura, int numeroDiasDiferencaProntoLib, float valorDe,
            float valorAte, string tipo, int fastDelivery, int tipoVenda, int origemPedido, string obs, bool buscarPedidoProducao, bool selecionar,
            out string filtroAdicional, out bool temFiltro, string obsLiberacao = "")
        {
            temFiltro = false;

            var campos = new StringBuilder();
            if (!selecionar)
                campos.Append("Count(*)");
            else
            {
                campos.Append("p.*, ");
                campos.Append(ClienteDAO.Instance.GetNomeCliente("c"));
                campos.Append(@" as NomeCliente, c.Revenda as CliRevenda, f.Nome as NomeFunc, c.idFunc as idFuncCliente,
                    med.Nome as NomeMedidor, c.Tel_Cont as rptTelCont, c.Tel_Res as rptTelRes, lp.dataLiberacao as dataLiberacao,
                    c.Tel_Cel as rptTelCel, l.NomeFantasia as nomeLoja, fp.Descricao as FormaPagto, o.Descricao as DescrObra,
                    o.Saldo as SaldoObra, ent.Nome as NomeUsuEntrada, (select cast(group_concat(distinct idItemProjeto) as char)
                    as idItensProjeto from produtos_pedido where idPedido=p.idPedido) as idItensProjeto, fv.nome as NomeFuncVenda,
                    pe.total as totalEspelho, (pe.idPedido is not null) as temEspelho, s.dataCad as dataEntrada,
                    s.usuCad as usuEntrada, cast(s.valorCreditoAoCriar as decimal(12,2)) as valorCreditoAoReceberSinal,
                    cast(s.creditoGeradoCriar as decimal(12,2)) as creditoGeradoReceberSinal, c.Cpf_Cnpj AS CpfCnpjCliente,
                    cast(s.creditoUtilizadoCriar as decimal(12,2)) as creditoUtilizadoReceberSinal, p.idPagamentoAntecipado>0 as pagamentoAntecipado,
                    c.pagamentoAntesProducao as clientePagaAntecipado, c.percSinalMin as percSinalMinCliente,
                    CAST( CONCAT(r.codInterno, ' - ', r.descricao) as char) as RptRotaCliente, ff.nome as nomeUsuFin, fc.nome as nomeUsuConf,
                    fl.nome as nomeUsuLib, if(" +
                    (FinanceiroConfig.PermitirConfirmacaoPedidoPeloFinanceiro || FinanceiroConfig.PermitirFinalizacaoPedidoPeloFinanceiro) + @",
                    (select count(*) from observacao_finalizacao_financeiro where idPedido=p.idPedido)>0, false) as exibirFinalizacoesFinanceiro,
                    CAST((SELECT GROUP_CONCAT(idOrdemCarga) FROM pedido_ordem_carga WHERE idPedido = p.idPedido) as CHAR) as IdsOCs,
                    transp.Nome AS NomeTransportador");
            }

            if (selecionar && opcionais)
                campos.Append(@", cm.Nome as NomeComissionado, c.Endereco as Endereco, c.numero as Numero,
                    c.compl as Compl, c.bairro as Bairro, cid.nomeCidade as Cidade, cid.nomeUf as Uf");

            return Sql(sessao, idPedido, idLiberarPedido, idsPedido, idsLiberarPedidos, idLoja, idCli, nomeCli, idFunc, codCliente,
                idCidade, endereco, bairro, complemento, situacao, situacaoProd, byVend, byConf, vendas, maoObra, maoObraEspecial,
                producao, dataCadIni, dataCadFim, dataFinIni, dataFinFim, funcFinalizacao, idOrcamento, opcionais, infoPedidos, altura,
                largura, numeroDiasDiferencaProntoLib, valorDe, valorAte, tipo, fastDelivery, tipoVenda, campos.ToString(), origemPedido, obs,
                buscarPedidoProducao, out filtroAdicional, ref temFiltro, obsLiberacao);
        }

        /// <summary>
        /// Constrói a consulta sql para pesquisar os pedidos do sistema.
        /// Nesse método é informada a projeção da colunas que devem ser recuperadas.
        /// </summary>
        private string Sql(GDASession sessao, uint idPedido, uint idLiberarPedido, string idsPedido, string idsLiberarPedidos,
            uint idLoja, uint idCli, string nomeCli, uint idFunc, string codCliente, uint idCidade, string endereco, string bairro,
            string complemento, string situacao, string situacaoProd, string byVend, string byConf, string vendas, string maoObra,
            string maoObraEspecial, string producao, string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim,
            string funcFinalizacao, uint idOrcamento, bool opcionais, bool infoPedidos, float altura, int largura,
            int numeroDiasDiferencaProntoLib, float valorDe, float valorAte, string tipo, int fastDelivery, int tipoVenda, string projecao,
            int origemPedido, string obs, bool buscarPedidoProducao, out string filtroAdicional, ref bool temFiltro, string obsLiberacao = "")
        {
            var sql = new StringBuilder("Select ");
            sql.Append(projecao);
            sql.Append(@" From pedido p
                Inner Join cliente c On (p.idCli=c.id_Cli)
                Left Join pedido_espelho pe on (p.idPedido=pe.idPedido)
                Left Join funcionario f On (p.IdFunc=f.IdFunc)
                Left Join funcionario fv On (p.IdFuncVenda=fv.IdFunc)
                Left Join loja l On (p.IdLoja = l.IdLoja)
                Left Join funcionario med On (p.IdMedidor=med.IdFunc)
                Left Join liberarpedido lp on (p.idLiberarPedido=lp.idLiberarPedido)
                Left Join sinal s on (p.idSinal=s.idSinal)
                Left Join funcionario ent On (s.UsuCad=ent.idFunc) 
                LEFT JOIN rota_cliente rc ON (c.id_cli = rc.idCliente)
                LEFT JOIN rota r ON (rc.idRota = r.idRota)
                Left Join funcionario ff On (p.usuFin=ff.idFunc)
                Left Join funcionario fc On (p.usuConf=fc.idFunc)
                Left Join funcionario fl On (lp.idFunc=fl.idFunc)
                Left Join transportador transp On (p.IdTransportador=transp.IdTransportador)");

            if (opcionais)
                sql.Append(@"Left Join comissionado cm On (p.idComissionado=cm.idComissionado)
                    Left Join cidade cid On (c.idCidade=cid.idCidade) ");

            if (infoPedidos)
                sql.Append(@"Left Join produtos_pedido pp on (p.idPedido=pp.idPedido)
                    Left Join produto prod on (pp.idProd=prod.idProd)
                    Left Join subgrupo_prod s1 on (prod.idSubgrupoProd=s1.idSubgrupoProd) ");

            sql.AppendFormat(@"Left Join obra o On (p.IdObra=o.IdObra)
                Left Join formapagto fp On (fp.IdFormaPagto=p.IdFormaPagto) 
                Where 1 {0}", FILTRO_ADICIONAL);

            var fa = new StringBuilder();

            if (idPedido > 0)
            {
                var idsPedidoFiltro = idPedido.ToString();

                /* Chamado 45419.
                 * O pedido de produção deve ser buscado, inicialmente, somente na listagem de pedidos.
                 * O erro do chamado aconteceu ao abrir a nota verde na listagem de pedidos, ao invés de exibir o pedido de revenda
                 * o sistema exibiu o pedido de produção. */
                if (buscarPedidoProducao)
                {
                    // Otimização necessária para deixar o sistema mais rápido, select in no SQL é extremamente pesado e deve ser evitado ao máximo
                    var ids = string.Join(",", ExecuteMultipleScalar<string>(string.Format("SELECT IdPedidoRevenda FROM Pedido WHERE IdPedido={0} AND IdPedidoRevenda>0", idPedido)));
                    if (!string.IsNullOrEmpty(ids))
                        idsPedidoFiltro += "," + ids;
                }

                temFiltro = true;
                sql.AppendFormat(" And p.IdPedido IN ({0})", idsPedidoFiltro);

            }
            else if (idLiberarPedido > 0)
            {
                fa.Append(" and p.idPedido in (select idPedido from produtos_liberar_pedido where idLiberarPedido=");
                fa.Append(idLiberarPedido);
                fa.Append(")");
            }
            else if (!string.IsNullOrEmpty(idsPedido))
            {
                fa.Append(" and p.IdPedido in (");
                fa.Append(idsPedido);
                fa.Append(")");
            }
            else if (!string.IsNullOrEmpty(idsLiberarPedidos))
            {
                fa.Append(" and p.idPedido in (select idPedido from produtos_liberar_pedido where idLiberarPedido in (");
                fa.Append(idsLiberarPedidos);
                fa.Append("))");
            }
            else if (idCli > 0)
            {
                fa.Append(" And p.IdCli=");
                fa.Append(idCli);
            }
            else if (!string.IsNullOrEmpty(nomeCli))
            {
                var ids = ClienteDAO.Instance.GetIds(sessao, null, nomeCli, null, 0, null, null, null, null, 0);
                fa.Append(" And p.idCli in (");
                fa.Append(ids);
                fa.Append(")");
            }

            if (idLoja > 0)
            {
                fa.Append(" And p.idLoja=");
                fa.Append(idLoja);
            }

            if (idFunc > 0)
            {
                fa.Append(" And p.idFunc=");
                fa.Append(idFunc);
            }

            if (!string.IsNullOrEmpty(codCliente))
            {
                fa.Append(" And (p.CodCliente like ?codCliente");

                fa.Append(")");
            }

            if (!string.IsNullOrEmpty(situacao) && situacao != "0")
                fa.Append(" And p.situacao In (?situacao)");

            if (!string.IsNullOrEmpty(situacaoProd) && situacaoProd != "0")
                fa.Append(" And (p.situacaoProducao In (?situacaoProd)" + (situacaoProd.StrParaInt() == (int)Pedido.SituacaoProducaoEnum.Pronto && PedidoConfig.DadosPedido.BloquearItensTipoPedido ?
                     " Or (p.situacao=" + (uint)Pedido.SituacaoPedido.ConfirmadoLiberacao + " And p.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Revenda + "))" : ")"));

            if (idCidade > 0)
                fa.Append(" And c.idCidade=" + idCidade);

            if (!string.IsNullOrEmpty(endereco))
                fa.Append(" And (p.enderecoObra like ?endereco Or c.endereco like ?endereco)");

            if (!string.IsNullOrEmpty(bairro))
                fa.Append(" And (p.bairroObra like ?bairro Or c.bairro like ?bairro)");

            if (!string.IsNullOrEmpty(complemento))
            {
                temFiltro = true;
                fa.Append(" AND c.Compl LIKE ?complemento");
            }

            if (!string.IsNullOrEmpty(tipo))
                fa.Append(" and p.tipoPedido in (" + tipo + ")");

            // Se o valor desta variável for 1, busca apenas pedidos ativo, ativo/conferência, conferência ou conferido do vendedor logado
            if (byVend == "1")
            {
                fa.Append(" And p.IdFunc=" + UserInfo.GetUserInfo.CodUser);

                if (!PedidoConfig.LiberarPedido && PedidoConfig.TelaListagem.ExibirApenasPedidosComercialVendedor)
                {
                    fa.Append(" And (p.Situacao=");
                    fa.Append((int)Glass.Data.Model.Pedido.SituacaoPedido.Ativo);
                    fa.Append(" Or p.Situacao=");
                    fa.Append((int)Glass.Data.Model.Pedido.SituacaoPedido.Conferido);
                    fa.Append(" Or p.Situacao=");
                    fa.Append((int)Glass.Data.Model.Pedido.SituacaoPedido.AtivoConferencia);
                    fa.Append(" Or p.Situacao=");
                    fa.Append((int)Glass.Data.Model.Pedido.SituacaoPedido.EmConferencia + ")");
                }
            }
            // Se byConf=1 busca apenas pedidos conferidos
            else if (byConf == "1")
            {
                fa.Append(" And p.Situacao=");
                fa.Append((int)Glass.Data.Model.Pedido.SituacaoPedido.Conferido);

                var login = UserInfo.GetUserInfo;

                // Se o caixa estiver logado, busca pedidos conferidos da sua loja apenas
                if (Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) && !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
                {
                    fa.Append(" And p.IdLoja=");
                    fa.Append(login.IdLoja);
                }
            }

            if (vendas == "1")
            {
                fa.Append(" and p.tipoPedido in (");
                fa.Append((int)Pedido.TipoPedidoEnum.Venda);
                fa.Append(",");
                fa.Append((int)Pedido.TipoPedidoEnum.Revenda);
                fa.Append(")");
            }
            else if (maoObra == "1")
            {
                fa.Append(" And p.tipoPedido=");
                fa.Append((int)Pedido.TipoPedidoEnum.MaoDeObra);
            }
            else if (maoObraEspecial == "1")
            {
                fa.Append(" And p.tipoPedido=");
                fa.Append((int)Pedido.TipoPedidoEnum.MaoDeObraEspecial);
            }
            else if (producao == "1")
            {
                fa.Append(" And p.tipoPedido=");
                fa.Append((int)Pedido.TipoPedidoEnum.Producao);
            }
            else if (producao == "0")
            {
                fa.Append(" and p.tipoPedido<>");
                fa.Append((int)Pedido.TipoPedidoEnum.Producao);
            }

            if (!string.IsNullOrEmpty(dataCadIni))
                fa.Append(" And p.dataPedido>=?dataCadIni");

            if (!string.IsNullOrEmpty(dataCadFim))
                fa.Append(" And p.dataPedido<=?dataCadFim");

            if (!string.IsNullOrEmpty(dataFinIni))
                fa.Append(" And p.dataFin>=?dataFinIni");

            if (!string.IsNullOrEmpty(dataFinFim))
                fa.Append(" And p.dataFin<=?dataFinFim");

            if (!string.IsNullOrEmpty(funcFinalizacao))
                fa.Append(" And p.usuFin In (" + funcFinalizacao + ")");

            if (valorDe > 0)
            {
                fa.Append(" and p.total >= ");
                fa.Append(valorDe.ToString().Replace(",", "."));
            }

            if (valorAte > 0)
            {
                fa.Append(" and p.total <= ");
                fa.Append(valorAte.ToString().Replace(",", "."));
            }

            if (idOrcamento > 0)
            {
                fa.Append(" and p.idOrcamento=");
                fa.Append(idOrcamento);
            }

            if (infoPedidos)
            {
                sql.Append(" and (pp.Invisivel{0}=false or pp.Invisivel{0} is null)");
                temFiltro = true;
            }

            if (altura > 0 && largura > 0)
            {
                if (infoPedidos)
                {
                    sql.Append(string.Format(" And pp.altura={0} And pp.largura={1}", altura.ToString().Replace(",", "."), largura));
                    temFiltro = true;
                }
                else
                {
                    sql.Append(string.Format(" and p.idPedido in (select idPedido from produtos_pedido where altura={0} and largura={1} and (Invisivel{2}=false or Invisivel{2} is null))",
                        altura.ToString().Replace(",", "."), largura, "{0}"));
                    temFiltro = true;
                }
            }
            else
            {
                if (altura > 0)
                {
                    if (infoPedidos)
                    {
                        sql.Append(" and pp.altura=");
                        sql.Append(altura.ToString().Replace(",", "."));
                        temFiltro = true;
                    }
                    else
                    {
                        sql.Append(" and p.idPedido in (select idPedido from produtos_pedido where altura=");
                        sql.Append(altura.ToString().Replace(",", "."));
                        sql.Append(" and (Invisivel{0}=false or Invisivel{0} is null))");
                        temFiltro = true;
                    }
                }

                if (largura > 0)
                {
                    if (infoPedidos)
                    {
                        sql.Append(" and pp.largura=");
                        sql.Append(largura);
                        temFiltro = true;
                    }
                    else
                    {
                        sql.Append(" and p.idPedido in (select idPedido from produtos_pedido where largura=");
                        sql.Append(largura);
                        sql.Append(" and (Invisivel{0}=false or Invisivel{0} is null))");
                        temFiltro = true;
                    }
                }
            }

            if (numeroDiasDiferencaProntoLib > 0)
            {
                sql.Append(" and datediff(lp.dataLiberacao, p.dataPronto)>=");
                sql.Append(numeroDiasDiferencaProntoLib);
                temFiltro = true;
            }

            if (fastDelivery > 0)
            {
                fa.Append(" and (FastDelivery" + (fastDelivery == 1 ? "=true" : "=false or FastDelivery is null") + ")");
            }

            if (tipoVenda > 0)
            {
                temFiltro = true;
                fa.Append(" And p.tipoVenda=");
                fa.Append(tipoVenda);
            }

            if (!string.IsNullOrEmpty(obs))
            {
                fa.Append(" AND p.obs like ?obs");
                temFiltro = true;
            }

            if (origemPedido > 0)
            {
                switch (origemPedido)
                {
                    case 1:
                        fa.Append(" AND COALESCE(Importado, 0) = 0 AND COALESCE(GeradoParceiro, 0) = 0");
                        temFiltro = true;
                        break;

                    case 2:
                        fa.Append(" AND COALESCE(GeradoParceiro, 0) = 1");
                        temFiltro = true;
                        break;

                    case 3:
                        fa.Append(" AND COALESCE(Importado, 0) = 1");
                        temFiltro = true;
                        break;
                }
            }

            if (!string.IsNullOrEmpty(obsLiberacao))
            {
                sql.Append(string.Format(" AND p.ObsLiberacao LIKE '{0}'", "%" + obsLiberacao + "%"));
            }

            filtroAdicional = fa.ToString();
            return string.Format(sql.ToString(), PCPConfig.UsarConferenciaFluxo ? "Fluxo" : "Pedido");
        }

        private void AtualizaSaldoPedidosAbertosObra(GDASession sessao, ref Pedido ped)
        {
            if (ped.IdObra > 0)
                ped.TotalPedidosAbertosObra = ExecuteScalar<decimal>(sessao, ObraDAO.Instance.SqlPedidosAbertos(ped.IdObra.ToString(),
                    ped.IdPedido.ToString(), ObraDAO.TipoRetorno.TotalPedido));
        }

        /// <summary>
        /// Atualiza o saldo da obra no pedido e no saldo geral da obra
        /// </summary>
        private void AtualizaSaldoObra(GDASession sessao, uint idPedido, Obra obraAtual, uint? idObra, decimal totalPedidoAtual,
            decimal totalPedidoAnterior, bool pedidoJaCalculadoNoSaldoDaObra)
        {
            if (idObra > 0)
            {
                var saldoDisponivelObra = ObraDAO.Instance.GetSaldo(sessao, idObra.Value);

                // Chamado 27270: Se o pedido já estiver considerado no saldo da obra, seu valor deve ser deduzido do saldo da obra que está no banco de dados,
                // para que atualize seu total de forma correta
                if (pedidoJaCalculadoNoSaldoDaObra)
                    saldoDisponivelObra += totalPedidoAnterior;

                // Atualiza o campo pagamento antecipado
                decimal valorPagamentoAntecipado = Math.Min(saldoDisponivelObra, totalPedidoAtual);
                objPersistence.ExecuteCommand(sessao, "update pedido set valorPagamentoAntecipado=?valor where idPedido=" + idPedido,
                    new GDAParameter("?valor", valorPagamentoAntecipado));

                ObraDAO.Instance.AtualizaSaldo(sessao, obraAtual, idObra.Value, false, false);
            }
        }

        private void AtualizaTemAlteracaoPcp(GDASession sessao, ref Pedido ped)
        {
            Pedido[] temp = { ped };
            AtualizaTemAlteracaoPcp(sessao, ref temp);
            ped = temp[0];
        }

        private void AtualizaTemAlteracaoPcp(ref Pedido[] ped)
        {
            AtualizaTemAlteracaoPcp(null, ref ped);
        }

        private void AtualizaTemAlteracaoPcp(GDASession sessao, ref Pedido[] ped)
        {
            if (ped.Length == 0)
                return;

            var sql = @"
                select p.idPedido
                from pedido p
                    inner join pedido_espelho pe on (p.idPedido=pe.idPedido)
                where p.idPedido in ({0}) and (p.total<>pe.total or p.tipoAcrescimo<>pe.tipoAcrescimo
                    or p.acrescimo<>pe.acrescimo or p.tipoDesconto<>pe.tipoDesconto or p.desconto<>pe.desconto)

                union all select distinct pp.idPedido
                from produtos_pedido pp
                    inner join produtos_pedido_espelho ppe on (pp.idPedido=ppe.idPedido)
                where pp.idPedido in ({0}) and coalesce(pp.invisivelPedido, false)=false and coalesce(ppe.invisivelFluxo, false)=false
                group by pp.idPedido
                having count(pp.idProdPed)<>count(ppe.idProdPed)";

            var ids = string.Empty;
            foreach (var p in ped)
                ids += p.IdPedido + ",";

            ids = GetValoresCampo(sessao, string.Format(sql, ids.TrimEnd(',')), "idPedido");

            var i = new List<string>(!string.IsNullOrEmpty(ids) ? ids.Split(',') : new string[] { });
            foreach (var p in ped)
                p.TemAlteracaoPcp = i.Contains(p.IdPedido.ToString());
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        public Pedido GetElement(uint idPedido)
        {
            return GetElement(null, idPedido);
        }

        public Pedido GetElement(GDASession sessao, uint idPedido)
        {
            try
            {
                bool temFiltro;
                string filtroAdicional;

                var sql = Sql(sessao, idPedido, 0, null, null, 0, 0, string.Empty, 0, null, 0, null, null, null, null,
                    string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, null, null, null, null, null, 0, true,
                    false, 0, 0, 0, 0, 0, null, 0, 0, 0, "", true, out filtroAdicional, out temFiltro);

                sql += "order by idpedido desc"; 

                var pedido = objPersistence.LoadOneData(sessao, sql.Replace("?filtroAdicional?", filtroAdicional));

                AtualizaTemAlteracaoPcp(sessao, ref pedido);

                AtualizaSaldoPedidosAbertosObra(sessao, ref pedido);

                #region Busca as parcelas do pedido

                var lstParc = ParcelasPedidoDAO.Instance.GetByPedido(sessao, idPedido).ToArray();

                var parcelas = lstParc.Length + " vez(es): ";

                pedido.ValoresParcelas = new decimal[lstParc.Length];
                pedido.DatasParcelas = new DateTime[lstParc.Length];

                for (var i = 0; i < lstParc.Length; i++)
                {
                    pedido.ValoresParcelas[i] = lstParc[i].Valor;
                    pedido.DatasParcelas[i] = lstParc[i].Data != null ? lstParc[i].Data.Value : new DateTime();
                    parcelas += lstParc[i].Valor.ToString("c") + "-" + (lstParc[i].Data != null ? lstParc[i].Data.Value.ToString("d") : "") + ",  ";
                }

                if (lstParc.Length > 0 && pedido.TipoVenda != (int)Pedido.TipoVendaPedido.AVista)
                    pedido.DescrParcelas = parcelas.TrimEnd(' ').TrimEnd(',');

                #endregion

                // Se for à Prazo e tiver recebido sinal
                if (pedido.TipoVenda == 2 && pedido.RecebeuSinal)
                {
                    DateTime dataSinal;
                    string nomeUsuEntrada;

                    if (pedido.DataEntrada != null)
                    {
                        dataSinal = pedido.DataEntrada.Value;
                        nomeUsuEntrada = pedido.NomeUsuEntrada;
                    }
                    else
                    {
                        var caixa = CaixaDiarioDAO.Instance.GetPedidoSinal(sessao, idPedido);
                        dataSinal = caixa.DataCad;
                        nomeUsuEntrada = caixa.DescrUsuCad;
                    }

                    pedido.ConfirmouRecebeuSinal = "R$ " + pedido.ValorEntrada.ToString("F2") + ", recebido por " + BibliotecaTexto.GetTwoFirstNames(nomeUsuEntrada) + " em " + dataSinal.ToString("dd/MM/yy") + ". ";
                }
                else if (pedido.Situacao == Pedido.SituacaoPedido.Cancelado)
                    pedido.Obs += "    Cancelado por " + FuncionarioDAO.Instance.GetNome(sessao, (uint)pedido.UsuCanc.Value) + " em " + pedido.DataCanc.Value.ToString("dd/MM/yyyy");

                // Verifica se o pedido possui conferência e se a mesma já foi finalizada, se tiver sido, não permite alterar o cliente
                // cadastrado no pedido.
                pedido.ClienteEnabled = true;

                if (PedidoConferenciaDAO.Instance.IsInConferencia(sessao, idPedido))
                    if (PedidoConferenciaDAO.Instance.ObtemValorCampo<PedidoConferencia.SituacaoConferencia>(sessao, "situacao", "idPedido=" + idPedido) == PedidoConferencia.SituacaoConferencia.Finalizada)
                        pedido.ClienteEnabled = false;

                return pedido;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Recupera a relação dos totais diários dos pedidos.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PedidoTotalDiario> ObtemTotaisDiarios(uint idPedido, uint idLoja, uint idCli, string nomeCli, string codCliente,
            uint idCidade, string endereco, string bairro, string complemento, string situacao, string situacaoProd, string byVend,
            string byConf, string maoObra, string maoObraEspecial, string producao, uint idOrcamento, float altura, int largura,
            int numeroDiasDiferencaProntoLib, float valorDe, float valorAte, string dataCadIni, string dataCadFim, string dataFinIni,
            string dataFinFim, string funcFinalizacao, string tipo, int fastDelivery, int origemPedido, string obs, string groupBy)
        {
            string groupByColumn = null;

            switch (groupBy)
            {
                case "DataPronto":
                    groupByColumn = "p.datapronto";
                    break;
                case "DataConf":
                    groupByColumn = "p.dataconf";
                    break;
                case "DataLiberacao":
                    groupByColumn = "lp.dataliberacao";
                    break;
                default:
                    yield break;
            }

            var temFiltro = false;
            string filtroAdicional;
            var projecao = string.Format("DATE_FORMAT({0}, '%Y/%m%/%d') AS Data, SUM(p.total) AS Total", groupByColumn);

            var sql = Sql(null, idPedido, 0, null, null, idLoja, idCli, nomeCli, 0, codCliente, idCidade, endereco, bairro, complemento,
                situacao, situacaoProd, byVend, byConf, null, maoObra, maoObraEspecial, producao, dataCadIni, dataCadFim, dataFinIni,
                dataFinFim, funcFinalizacao, idOrcamento, false, false, altura, largura, numeroDiasDiferencaProntoLib, valorDe, valorAte,
                tipo, fastDelivery, 0, projecao, origemPedido, obs, true, out filtroAdicional, ref temFiltro)
                .Replace("?filtroAdicional?", string.Format("{0} AND {1} IS NOT NULL", filtroAdicional, groupByColumn));

            // Agrupa pelo dia
            sql += string.Format(" GROUP BY DATE_FORMAT({0}, '%Y/%m%/%d') ORDER BY Data", groupByColumn);

            // Recupera os parametros dos filtros
            var parameters = GetParam(nomeCli, codCliente, endereco, bairro, complemento, situacao, situacaoProd, dataCadIni, dataCadFim,
                dataFinIni, dataFinFim, obs);

            foreach (var record in this.CurrentPersistenceObject.LoadResult(sql, parameters))
            {
                var data = DateTime.Parse(record["Data"]);
                var total = record.GetDecimal("Total");

                yield return new PedidoTotalDiario
                {
                    Data = data,
                    Total = total
                };
            }
        }

        public Pedido[] GetList(uint idPedido, uint idLoja, uint idCli, string nomeCli, string codCliente, uint idCidade, string endereco,
            string bairro, string situacao, string situacaoProd, string byVend, string byConf, string maoObra, string maoObraEspecial,
            string producao, uint idOrcamento, float altura, int largura, int numeroDiasDiferencaProntoLib, float valorDe, float valorAte,
            string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim, string funcFinalizacao, string tipo, 
            int tipoVenda, int fastDelivery, string sortExpression, int startRow, int pageSize)
        {
            return GetList(idPedido, idLoja, idCli, nomeCli, codCliente, idCidade, endereco, bairro, null, situacao, situacaoProd, byVend,
                byConf, maoObra, maoObraEspecial, producao, idOrcamento, altura, largura, numeroDiasDiferencaProntoLib, valorDe, valorAte,
                dataCadIni, dataCadFim, dataFinIni, dataFinFim, funcFinalizacao, tipo, fastDelivery, tipoVenda, 0, "", sortExpression,
                startRow, pageSize);
        }

        public Pedido[] GetList(uint idPedido, uint idLoja, uint idCli, string nomeCli, string codCliente, uint idCidade, string endereco,
            string bairro, string situacao, string situacaoProd, string byVend, string byConf, string maoObra, string maoObraEspecial,
            string producao, uint idOrcamento, float altura, int largura, int numeroDiasDiferencaProntoLib, float valorDe, float valorAte,
            string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim, string funcFinalizacao, string tipo, string obsLiberacao,
            int fastDelivery, int tipoVenda, string obs, string sortExpression, int startRow, int pageSize)
        {
            return GetList(idPedido, idLoja, idCli, nomeCli, codCliente, idCidade, endereco, bairro, null, situacao, situacaoProd, byVend,
                byConf, maoObra, maoObraEspecial, producao, idOrcamento, altura, largura, numeroDiasDiferencaProntoLib, valorDe, valorAte,
                dataCadIni, dataCadFim, dataFinIni, dataFinFim, funcFinalizacao, tipo, fastDelivery, tipoVenda, 0, obs, sortExpression,
                startRow, pageSize, obsLiberacao);
        }

        public Pedido[] GetList(uint idPedido, uint idLoja, uint idCli, string nomeCli, string codCliente, uint idCidade, string endereco,
            string bairro, string complemento, string situacao, string situacaoProd, string byVend, string byConf, string maoObra,
            string maoObraEspecial, string producao, uint idOrcamento, float altura, int largura, int numeroDiasDiferencaProntoLib,
            float valorDe, float valorAte, string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim,
            string funcFinalizacao, string tipo, int fastDelivery, int tipoVenda, int origemPedido, string obs, string sortExpression, int startRow,
            int pageSize, string obsLiberacao = "")
        {
            var filtro = string.IsNullOrEmpty(sortExpression) ? "p.IdPedido Desc" : string.Format("{0}, IdPedido DESC", sortExpression);

            bool temFiltro;
            string filtroAdicional;

            var sql = Sql(idPedido, 0, null, null, idLoja, idCli, nomeCli, 0, codCliente, idCidade, endereco, bairro, complemento, situacao,
                situacaoProd, byVend, byConf, null, maoObra, maoObraEspecial, producao, dataCadIni, dataCadFim, dataFinIni, dataFinFim,
                funcFinalizacao, idOrcamento, false, false, altura, largura, numeroDiasDiferencaProntoLib, valorDe, valorAte, tipo,
                fastDelivery, tipoVenda, origemPedido, obs, true, true, out filtroAdicional, out temFiltro, obsLiberacao)
                .Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            var ped = LoadDataWithSortExpression(sql, filtro, startRow, pageSize, temFiltro, filtroAdicional, GetParam(nomeCli, codCliente,
                endereco, bairro, complemento, situacao, situacaoProd, dataCadIni, dataCadFim, dataFinIni, dataFinFim, obs)).ToArray();

            AtualizaTemAlteracaoPcp(ref ped);
            return ped;
        }


        public int GetCount(uint idPedido, uint idLoja, uint idCli, string nomeCli, string codCliente, uint idCidade, string endereco,
            string bairro, string situacao, string situacaoProd, string byVend, string byConf, string maoObra, string maoObraEspecial,
            string producao, uint idOrcamento, float altura, int largura, int numeroDiasDiferencaProntoLib, float valorDe, float valorAte,
            string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim, string funcFinalizacao, string tipo,
            int tipoVenda, int fastDelivery)
        {
            return GetCount(idPedido, idLoja, idCli, nomeCli, codCliente, idCidade, endereco, bairro, null, situacao, situacaoProd, byVend,
                byConf, maoObra, maoObraEspecial, producao, idOrcamento, altura, largura, numeroDiasDiferencaProntoLib, valorDe, valorAte,
                dataCadIni, dataCadFim, dataFinIni, dataFinFim, funcFinalizacao, tipo, fastDelivery, tipoVenda, 0, "");
        }


        public int GetCount(uint idPedido, uint idLoja, uint idCli, string nomeCli, string codCliente, uint idCidade, string endereco,
            string bairro, string situacao, string situacaoProd, string byVend, string byConf, string maoObra, string maoObraEspecial,
            string producao, uint idOrcamento, float altura, int largura, int numeroDiasDiferencaProntoLib, float valorDe, float valorAte,
            string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim, string funcFinalizacao, string tipo,
            int fastDelivery, int tipoVenda, string obs, string obsLiberacao)
        {
            return GetCount(idPedido, idLoja, idCli, nomeCli, codCliente, idCidade, endereco, bairro, null, situacao, situacaoProd, byVend,
                byConf, maoObra, maoObraEspecial, producao, idOrcamento, altura, largura, numeroDiasDiferencaProntoLib, valorDe, valorAte,
                dataCadIni, dataCadFim, dataFinIni, dataFinFim, funcFinalizacao, tipo, fastDelivery, tipoVenda, 0, obs, obsLiberacao);
        }

        public int GetCount(uint idPedido, uint idLoja, uint idCli, string nomeCli, string codCliente, uint idCidade, string endereco,
            string bairro, string complemento, string situacao, string situacaoProd, string byVend, string byConf, string maoObra,
            string maoObraEspecial, string producao, uint idOrcamento, float altura, int largura, int numeroDiasDiferencaProntoLib,
            float valorDe, float valorAte, string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim,
            string funcFinalizacao, string tipo, int fastDelivery, int tipoVenda, int origemPedido, string obs, string obsLiberacao = "")
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = Sql(idPedido, 0, null, null, idLoja, idCli, nomeCli, 0, codCliente, idCidade, endereco, bairro, complemento, situacao,
                situacaoProd, byVend, byConf, null, maoObra, maoObraEspecial, producao, dataCadIni, dataCadFim, dataFinIni, dataFinFim,
                funcFinalizacao, idOrcamento, false, false, altura, largura, numeroDiasDiferencaProntoLib, valorDe, valorAte, tipo,
                fastDelivery, tipoVenda, origemPedido, obs, true, true, out filtroAdicional, out temFiltro, obsLiberacao)
                .Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, GetParam(nomeCli, codCliente, endereco, bairro, complemento,
                situacao, situacaoProd, dataCadIni, dataCadFim, dataFinIni, dataFinFim, obs));
        }

        private GDAParameter[] GetParam(string nomeCli, string codCliente, string endereco, string bairro, string situacao,
            string situacaoProd, string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim, string obs)
        {
            return GetParam(nomeCli, codCliente, endereco, bairro, null, situacao, situacaoProd, dataCadIni, dataCadFim, dataFinIni, dataFinFim, obs);
        }

        private GDAParameter[] GetParam(string nomeCli, string codCliente, string endereco, string bairro, string complemento,
            string situacao, string situacaoProd, string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim, string obs)
        {
            var lstParam = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(nomeCli))
                lstParam.Add(new GDAParameter("?nomeCli", "%" + nomeCli + "%"));

            if (!string.IsNullOrEmpty(codCliente))
                lstParam.Add(new GDAParameter("?codCliente", "%" + codCliente + "%"));

            if (!string.IsNullOrEmpty(endereco))
                lstParam.Add(new GDAParameter("?endereco", "%" + endereco + "%"));

            if (!string.IsNullOrEmpty(bairro))
                lstParam.Add(new GDAParameter("?bairro", "%" + bairro + "%"));

            if (!string.IsNullOrEmpty(complemento))
                lstParam.Add(new GDAParameter("?complemento", "%" + complemento + "%"));

            if (!string.IsNullOrEmpty(situacao) && situacao != "0")
                lstParam.Add(new GDAParameter("?situacao", situacao.Trim(',')));

            if (!string.IsNullOrEmpty(situacaoProd) && situacaoProd != "0")
                lstParam.Add(new GDAParameter("?situacaoProd", situacaoProd.Trim(',')));

            if (!string.IsNullOrEmpty(dataCadIni))
                lstParam.Add(new GDAParameter("?dataCadIni", DateTime.Parse(dataCadIni + " 00:00:00")));

            if (!string.IsNullOrEmpty(dataCadFim))
                lstParam.Add(new GDAParameter("?dataCadFim", DateTime.Parse(dataCadFim + " 23:59:59")));

            if (!string.IsNullOrEmpty(dataFinIni))
                lstParam.Add(new GDAParameter("?dataFinIni", DateTime.Parse(dataFinIni + " 00:00:00")));

            if (!string.IsNullOrEmpty(dataFinFim))
                lstParam.Add(new GDAParameter("?dataFinFim", DateTime.Parse(dataFinFim + " 23:59:59")));

            if (!string.IsNullOrEmpty(obs))
                lstParam.Add(new GDAParameter("?obs", "%" + obs + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        /// <summary>
        /// Retorna lista de pedido para serem impressos a partir da listagem comum de pedidos Listas/LstPedidos.aspx
        /// </summary>
        public Pedido[] GetListForRpt(uint idPedido, uint idLoja, uint idCli, string nomeCli, string codCliente, uint idCidade,
            string endereco, string bairro, string complemento, string byVend, string byConf, string maoObra, string maoObraEspecial,
            string producao, float altura, int largura, int numeroDiasDiferencaProntoLib, float valorDe, float valorAte, string dataCadIni,
            string dataCadFim, int fastDelivery, int tipoVenda, int origemPedido, string obs)
        {
            var criterio = string.Empty;

            if (idPedido > 0)
                criterio += "Pedido: " + idPedido + "    ";
            else if (idCli > 0)
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCli) + "    ";
            else if (!string.IsNullOrEmpty(nomeCli))
                criterio += "Cliente: " + nomeCli + "    ";

            if (idLoja > 0)
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";

            if (!string.IsNullOrEmpty(codCliente))
                criterio += "Cód. Ped. Cli.: " + codCliente + " ";

            if (idCidade > 0)
                criterio += "Cidade: " + CidadeDAO.Instance.GetNome(idCidade) + " - " + CidadeDAO.Instance.GetNomeUf(null, idCidade) + "    ";

            if (!string.IsNullOrEmpty(endereco))
                criterio += "Endereço: " + endereco + "    ";

            if (!string.IsNullOrEmpty(bairro))
                criterio += "Bairro: " + bairro + "    ";

            if (!string.IsNullOrEmpty(complemento))
                criterio += "Complemento: " + complemento + "    ";

            if (byVend == "1")
                criterio += "Meus Pedidos    ";

            if (byConf == "1")
                criterio += "Pedidos Conferidos    ";

            if (maoObra == "1")
                criterio += "Pedidos de mão de obra    ";

            if (maoObraEspecial == "1")
                criterio += "Pedidos de mão de obra especial    ";

            if (producao == "1")
                criterio += "Pedidos de produção    ";

            if (altura > 0)
                criterio += "Altura Produto: " + altura + "    ";

            if (largura > 0)
                criterio += "Largura Produto: " + largura + "    ";

            if (valorDe > 0)
                criterio += "Valor a partir de: " + valorDe + " ";

            if (valorAte > 0)
                criterio += "Valor até: " + valorAte + "    ";

            if (numeroDiasDiferencaProntoLib > 0)
                criterio += "Diferença dias entre Pedido Pronto e Liberado: " + numeroDiasDiferencaProntoLib + "    ";

            bool temFiltro;
            string filtroAdicional;

            var sql = Sql(idPedido, 0, null, null, idLoja, idCli, nomeCli, 0, codCliente, idCidade, endereco, bairro, complemento, null,
                null, byVend, byConf, null, maoObra, maoObraEspecial, producao, dataCadIni, dataCadFim, null, null, null, 0, false, false,
                altura, largura, numeroDiasDiferencaProntoLib, valorDe, valorAte, null, fastDelivery, tipoVenda, origemPedido, obs, true, true,
                out filtroAdicional, out temFiltro).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            var lstPed = objPersistence.LoadData(sql + filtroAdicional, GetParam(nomeCli, codCliente, endereco, bairro, complemento, null,
                null, dataCadIni, dataCadFim, null, null, obs)).ToArray();

            if (lstPed.Length > 0)
                lstPed[0].Criterio = criterio;

            return lstPed;
        }

        #endregion

        #region Relatório de Pedido

        public Pedido[] GetForRpt(string idsPedidos, bool forPcp)
        {
            return GetForRpt(idsPedidos, forPcp, UserInfo.GetUserInfo);
        }

        public Pedido[] GetForRpt(string idsPedidos, bool forPcp, LoginUsuario login)
        {
            var qtdPecas = PedidoConfig.LiberarPedido ? @"Select Cast(Sum(Coalesce(qtde, 0)) As Signed Integer) From produtos_pedido pp 
                Left Join produto p On (pp.idProd=p.idProd) Where idPedido=p.idPedido And Coalesce(invisivelPedido, False)=False 
                And p.idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro : "0";

            var sql = @"
                Select p.*, " + ClienteDAO.Instance.GetNomeCliente("c") + @" as NomeCliente, f.Nome as NomeFunc, f_ant.nome as nomeFuncPedidoAnterior, l.NomeFantasia as nomeLoja,
                    l.telefone as TelefoneLoja, l.cnpj as cnpjLoja, l.Site as EmailLoja, fp.Descricao as FormaPagto, c.Cpf_Cnpj, c.RG_ESCINST, c.Email as Email, c.Email as RptEmail,
                    c.Tel_Cont as rptTelCont, c.Tel_Res as rptTelRes, c.Tel_Cel as rptTelCel, c.ENDERECO, c.COMPL, c.Numero,
                    c.Contato as ContatoCliente, Concat(Coalesce(l.Endereco, ''), ', ', Coalesce(l.Bairro, ''), ' - ',
                    Coalesce(cidLoja.NomeCidade, ''), '/', Coalesce(cidLoja.NomeUf, ''), ' Cep: ', Coalesce(l.Cep, ''), ' Fone: ',
                    Coalesce(l.Telefone, '')) as DadosLoja, l.Endereco as EnderecoLoja, l.Compl as ComplLoja, l.Bairro as BairroLoja,
                    cidLoja.NomeCidade as CidadeLoja, cidLoja.NomeUf as UfLoja, l.Cep as CepLoja, c.BAIRRO, if(c.idCidade is null, c.cidade,
                    cid.NomeCidade) as Cidade, cid.NomeUf as uf, c.CEP, c.TIPO_PESSOA, med.Nome as NomeMedidor, ent.Nome as NomeUsuEntrada,(" + (qtdPecas) + @") as QtdePecas,
                    com.Nome as NomeComissionado, coalesce(pe.Total, p.total) as TotalEspelho, c.cpf_Cnpj as cpfCnpjCliente, c.rg_escInst as rgInscrEstadualCliente,
                    c.tipo_Pessoa as tipoPessoaCliente, c.pagamentoAntesProducao as ClientePagaAntecipado, s.dataCad as dataEntrada,
                    s.usuCad as usuEntrada, cast(s.valorCreditoAoCriar as decimal(12,2)) as valorCreditoAoReceberSinal, cast(s.creditoGeradoCriar as decimal(12,2)) as creditoGeradoReceberSinal,
                    cast(s.creditoUtilizadoCriar as decimal(12,2)) as creditoUtilizadoReceberSinal, s.isPagtoAntecipado as pagamentoAntecipado,
                    fc.Nome AS NomeFuncCliente,
                    COALESCE(pe.valorIpi, p.valorIpi) as ValorIpiEspelho, COALESCE(pe.ValorIcms, p.ValorIcms) as ValorIcmsEspelho, transp.Nome AS NomeTransportador
                From pedido p
                    Left Join pedido p_ant on (p.idPedidoAnterior=p_ant.idPedido)
                    Left Join pedido_espelho pe on (p.idPedido=pe.idPedido)
                    Left Join funcionario f_ant on (p_ant.idFunc=f_ant.idFunc)
                    Inner Join cliente c On (p.idCli=c.id_Cli)
                    LEFT JOIN funcionario fc ON (c.IdFunc=fc.IdFunc)
                    Left Join cidade cid On (cid.idCidade=c.idCidade)
                    Left Join funcionario f On (p.IdFunc=f.IdFunc)
                    Left Join funcionario med On (p.IdMedidor=med.IdFunc)
                    Left Join loja l On (p.IdLoja = l.IdLoja)
                    Left Join cidade cidLoja On (cidLoja.idCidade=l.idCidade)
                    Left Join formapagto fp On (fp.IdFormaPagto=p.IdFormaPagto)
                    Left Join comissionado com On (p.IdComissionado=com.IdComissionado)
                    left join sinal s on (p.idSinal=s.idSinal)
                    Left Join funcionario ent On (s.UsuCad=ent.IdFunc)
                    Left Join Transportador transp On (p.IdTransportador=transp.IdTransportador)
                Where p.IdPedido in (" + idsPedidos + ")";

            var pedidos = objPersistence.LoadData(sql).ToArray();

            foreach (var pedido in pedidos)
            {
                #region Busca quem confirmou pedido

                var confirmouRecebeuSinal = string.Empty;

                // Busca quem finalizou o pedido
                if (pedido.UsuFin > 0)
                {
                    confirmouRecebeuSinal += "Finalizado por " + BibliotecaTexto.GetTwoFirstNames(FuncionarioDAO.Instance.GetNome(pedido.UsuFin.Value));
                    confirmouRecebeuSinal += pedido.DataFin != null ? " no dia " + pedido.DataFin.Value.ToString("dd/MM/yy HH:mm") + ". " : ". ";
                }

                // Busca quem confirmou o pedido
                if (pedido.UsuConf > 0)
                {
                    confirmouRecebeuSinal += "Confirmado por " + BibliotecaTexto.GetTwoFirstNames(FuncionarioDAO.Instance.GetNome((uint)pedido.UsuConf.Value));
                    confirmouRecebeuSinal += pedido.DataConf != null ? " no dia " + pedido.DataConf.Value.ToString("dd/MM/yy HH:mm") + ". " : ". ";
                }

                #endregion

                #region Busca que recebeu sinal

                if ((pedido.TipoVenda == 2 || pedido.TipoVenda == 1) && pedido.RecebeuSinal)
                {
                    DateTime dataSinal;
                    string nomeUsuEntrada;

                    if (pedido.DataEntrada != null)
                    {
                        dataSinal = pedido.DataEntrada.Value;
                        nomeUsuEntrada = pedido.NomeUsuEntrada;
                    }
                    else
                    {
                        var caixa = CaixaDiarioDAO.Instance.GetPedidoSinal(pedido.IdPedido);
                        dataSinal = caixa.DataCad;
                        nomeUsuEntrada = caixa.DescrUsuCad;
                    }

                    confirmouRecebeuSinal += "Sinal recebido por " + nomeUsuEntrada + " no dia " + dataSinal.ToString("dd/MM/yy") + ". ";
                }

                #endregion

                #region Busca as parcelas do pedido

                if (pedido.TipoVenda == (int)Pedido.TipoVendaPedido.APrazo)
                {
                    var lstParc = ParcelasPedidoDAO.Instance.GetForRpt(pedido.IdPedido).ToArray();

                    var parcelas = lstParc.Length + " vez(es): ";

                    for (var i = 0; i < lstParc.Length; i++)
                    {
                        var valor = lstParc[i].Valor - lstParc[i].Desconto;
                        parcelas += valor.ToString("c") + "-" + (lstParc[i].Data != null ? lstParc[i].Data.Value.ToString("d") : "") + ",  ";
                    }

                    if (lstParc.Length > 0)
                        pedido.DescrParcelas = parcelas.TrimEnd(' ').TrimEnd(',');
                }

                #endregion

                pedido.ConfirmouRecebeuSinal = confirmouRecebeuSinal;

                // Se o pedido estiver cancelado, mostra quem cancelou.
                if (pedido.Situacao == Pedido.SituacaoPedido.Cancelado && pedido.UsuCanc > 0)
                    pedido.Obs += "    Cancelado por: " + FuncionarioDAO.Instance.GetNome((uint)pedido.UsuCanc.Value) + " em " + pedido.DataCanc.Value.ToString("dd/MM/yyyy");

                pedido.ImpressoPor = login != null ? login.Nome : string.Empty;

                if (forPcp && PedidoEspelhoDAO.Instance.ExisteEspelho(pedido.IdPedido))
                {
                    pedido.DescontoTotalPcp = true;
                    pedido.Obs += " " + PedidoEspelhoDAO.Instance.ObtemValorCampo<string>("obs", "idPedido=" + pedido.IdPedido);
                }
            }

            return pedidos;
        }

        #endregion

        #region Filtro de data por situação (para seleção de mais de uma situação)

        internal class DadosFiltroDataSituacao
        {
            public string Sql, Criterio;
            public GDAParameter[] Parametros;
        }

        internal DadosFiltroDataSituacao FiltroDataSituacao(string dataIni, string dataFim, string situacao, string nomeParamDataIni,
            string nomeParamDataFim, string aliasPedido, string aliasLiberarPedido, string complementoCriterio, bool calcularLiberados)
        {
            var lstParam = new List<GDAParameter>();
            string sql = "", criterio = "";

            if ((!string.IsNullOrEmpty(dataIni) || !string.IsNullOrEmpty(dataFim)))
            {
                var where = "";

                if (!string.IsNullOrEmpty(dataIni))
                    lstParam.Add(new GDAParameter(nomeParamDataIni, DateTime.Parse(dataIni + " 00:00")));

                if (!String.IsNullOrEmpty(dataFim))
                    lstParam.Add(new GDAParameter(nomeParamDataFim, DateTime.Parse(dataFim + " 23:59")));

                var vetSituacao = new List<string>(!string.IsNullOrEmpty(situacao) ? situacao.Split(',') : new string[0]);

                if (string.IsNullOrEmpty(situacao) ||
                    vetSituacao.Contains(((int)Pedido.SituacaoPedido.Ativo).ToString()) ||
                    vetSituacao.Contains(((int)Pedido.SituacaoPedido.AtivoConferencia).ToString()) ||
                    vetSituacao.Contains(((int)Pedido.SituacaoPedido.EmConferencia).ToString()) ||
                    vetSituacao.Contains(((int)Pedido.SituacaoPedido.Conferido).ToString()))
                {
                    where += " Or (";

                    if (!string.IsNullOrEmpty(dataIni))
                        where += " and " + aliasPedido + ".DataCad>=" + nomeParamDataIni;

                    if (!string.IsNullOrEmpty(dataFim))
                        where += " and " + aliasPedido + ".DataCad<=" + nomeParamDataFim;

                    where += ")";
                }

                if (string.IsNullOrEmpty(situacao) ||
                    vetSituacao.Contains(((int)Pedido.SituacaoPedido.Cancelado).ToString()))
                {
                    where += " Or (";

                    if (!string.IsNullOrEmpty(dataIni))
                        where += " And " + aliasPedido + ".DataCanc>=" + nomeParamDataIni;

                    if (!string.IsNullOrEmpty(dataFim))
                        where += " And " + aliasPedido + ".DataCanc<=" + nomeParamDataFim;

                    where += ")";
                }

                if (string.IsNullOrEmpty(situacao) ||
                    vetSituacao.Contains(((int)Pedido.SituacaoPedido.Confirmado).ToString()) ||
                    vetSituacao.Contains(((int)Pedido.SituacaoPedido.LiberadoParcialmente).ToString()))
                {
                    where += " Or (";

                    // Se for filtrar pela data da liberação, filtra pelos ids
                    //if (!string.IsNullOrEmpty(dataIni) && !string.IsNullOrEmpty(dataFim) && PedidoConfig.LiberarPedido && calcularLiberados)
                    /* Chamado 48140. */
                    if ((!string.IsNullOrEmpty(dataIni) || !string.IsNullOrEmpty(dataFim)) && PedidoConfig.LiberarPedido)
                    {
                        var sqlIdsLiberacao = "SELECT CAST(IdLiberarPedido AS CHAR) FROM liberarpedido WHERE 1 ";
                        var parametros = new List<GDAParameter>();

                        if (!string.IsNullOrEmpty(dataIni))
                        {
                            sqlIdsLiberacao += string.Format(" AND DataLiberacao>={0}", nomeParamDataIni);
                            parametros.Add(new GDAParameter(nomeParamDataIni, DateTime.Parse(dataIni)));
                        }

                        if (!string.IsNullOrEmpty(dataFim))
                        {
                            sqlIdsLiberacao += string.Format(" AND DataLiberacao<={0}", nomeParamDataFim);
                            parametros.Add(new GDAParameter(nomeParamDataFim, DateTime.Parse(dataFim + " 23:59")));
                        }

                        var idsLibPed = string.Join(",", ExecuteMultipleScalar<string>(sqlIdsLiberacao, parametros.ToArray()).ToArray());

                        if (!string.IsNullOrEmpty(idsLibPed))
                            where += string.Format("{0}.idLiberarPedido In ({1})", aliasLiberarPedido, idsLibPed);
                        else
                            where += "FALSE";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(dataIni))
                        {
                            if (!PedidoConfig.LiberarPedido || !calcularLiberados)
                                where += " And " + aliasPedido + ".DataConf>=" + nomeParamDataIni;
                            else
                                where += " And " + aliasLiberarPedido + ".dataLiberacao>=" + nomeParamDataIni;
                        }

                        if (!string.IsNullOrEmpty(dataFim))
                        {
                            if (!PedidoConfig.LiberarPedido || !calcularLiberados)
                                where += " And " + aliasPedido + ".DataConf<=" + nomeParamDataFim;
                            else
                                where += " And " + aliasLiberarPedido + ".dataLiberacao<=" + nomeParamDataFim;
                        }
                    }

                    where += ")";
                }

                if ((PedidoConfig.LiberarPedido && string.IsNullOrEmpty(situacao)) ||
                    vetSituacao.Contains(((int)Pedido.SituacaoPedido.ConfirmadoLiberacao).ToString()))
                {
                    where += " Or (";

                    if (!string.IsNullOrEmpty(dataIni))
                        where += " And " + aliasPedido + ".DataConf>=" + nomeParamDataIni;

                    if (!string.IsNullOrEmpty(dataFim))
                        where += " And " + aliasPedido + ".DataConf<=" + nomeParamDataFim;

                    where += ")";
                }

                if (!string.IsNullOrEmpty(where))
                {
                    where = where.Replace("( And ", "(").Replace("( and ", "(");
                    sql += " and (" + where.Substring(" Or ".Length) + ")";

                    criterio += "Data Início" + complementoCriterio + ": " + dataIni + "    ";
                    criterio += "Data Fim" + complementoCriterio + ": " + dataFim + "    ";
                }

                if (!where.Contains(nomeParamDataIni))
                    lstParam.Remove(lstParam.Find(f => f.ParameterName == nomeParamDataIni));

                if (!where.Contains(nomeParamDataFim))
                    lstParam.Remove(lstParam.Find(f => f.ParameterName == nomeParamDataFim));
            }

            var retorno = new DadosFiltroDataSituacao
            {
                Sql = sql,
                Criterio = criterio,
                Parametros = lstParam.ToArray()
            };

            return retorno;
        }

        #endregion

        #region Campos para liberação parcial

        private string SqlCampoCalcLiberacao(bool usarTotalCalc, string campoSemCalcPedido, string campoSemCalcPcp,
            bool usarAliasPedidoSemCalc, string campoCalcProd, string campoCalcBenef, string nomeCampo, string aliasPedido,
            string aliasPedidoEspelho, string aliasAmbientePedido, string aliasProdutosLiberarPedido, bool arredondarResultado)
        {
            var campoSemCalc = string.Format(PCPConfig.UsarConferenciaFluxo && PedidoConfig.LiberarPedido && !string.IsNullOrEmpty(aliasPedidoEspelho) && !string.IsNullOrEmpty(campoSemCalcPcp) ?
                "coalesce({0}.{1}, {2}.{3})" : usarAliasPedidoSemCalc ? "{2}.{3}" : "{3}", aliasPedidoEspelho, campoSemCalcPcp, aliasPedido, campoSemCalcPedido);

            if (!usarTotalCalc || !PedidoConfig.LiberarPedido)
                return campoSemCalc + " as " + nomeCampo;

            // Campo que corresponde ao valor do produto (total ou custo)
            var campoSomar =
                campoCalcBenef == "valor" ?
                    string.Format(@"((
                    pp1.{0} + coalesce(pp1.valorBenef, 0))) *
                    (1 + coalesce({1}.taxaFastDelivery / 100, 0))",
                    campoCalcProd,
                    aliasPedido) :

                    string.Format(@"((
                    pp1.{0} + coalesce((select sum({1}) from produto_pedido_benef where idProdPed=pp1.idProdPed), 0))) *
                    (1 + coalesce({2}.taxaFastDelivery / 100, 0))",
                    campoCalcProd,
                    campoCalcBenef,
                    aliasPedido);

            // Campo de retorno
            var campo = new StringBuilder();
            campo.AppendFormat("cast({1}(sum((select (({0}",
                campoSomar,
                arredondarResultado ? "round" : "");

            // Considera a quantidade de peças para pedidos mão-de-obra ou de empresas que utilizam a configuração de liberação
            // por produtos prontos
            //string campoMaoObraLibPronto = String.Format(" / (pp1.qtde * if({0}.tipoPedido={2}, coalesce({1}.qtde, 1), 1)) * sum(plp1.qtdeCalc)",
            var campoMaoObraLibPronto = string.Format(" / (if({0}.tipoPedido={2}, coalesce({1}.qtde * pp1.qtde, 1), pp1.qtde)) * sum(plp1.qtdeCalc)",
                aliasPedido,
                aliasAmbientePedido,
                (int)Pedido.TipoPedidoEnum.MaoDeObra);

            campo.Append(campoMaoObraLibPronto);

            //ICMS
            var icms = string.Format(" + if({0}.situacao in (" +
                (int)Pedido.SituacaoPedido.Confirmado + "," +
                (int)Pedido.SituacaoPedido.LiberadoParcialmente + "), coalesce(plp1.valorIcms, 0), coalesce(pp1.valorIcms, 0){1})",
                aliasPedido,
                campoMaoObraLibPronto);

            campo.Append(") " + icms);

            //IPI
            var ipi = string.Format(" + if({0}.situacao in (" +
                (int)Pedido.SituacaoPedido.Confirmado + "," +
                (int)Pedido.SituacaoPedido.LiberadoParcialmente + "), coalesce(plp1.valorIpi, 0), coalesce(pp1.valorIpi, 0){1})",
                aliasPedido,
                campoMaoObraLibPronto);

            campo.Append(ipi);

            // Ajusta o valor dos produtos se não ratear o desconto, exceto se for cálculo de custo
            if (!PedidoConfig.RatearDescontoProdutos && !campoSemCalcPedido.Contains("custo"))
            {
                campo.AppendFormat(" - if({0} > 0, if({1}=1, ((({2}){3})/*+{7}*/)*({0}/100), (((({2}){3})/({4}+{0}-{5}-{6}))*{0})), 0)",
                    string.Format("if (!coalesce(pp1.invisivelFluxo, false), coalesce({1}.desconto, {0}.desconto), {0}.desconto)", aliasPedido, aliasPedidoEspelho),
                    string.Format("if (!coalesce(pp1.invisivelFluxo, false), coalesce({1}.tipoDesconto, {0}.tipoDesconto), {0}.tipoDesconto)", aliasPedido, aliasPedidoEspelho),
                    campoSomar,
                    campoMaoObraLibPronto,
                    /* Chamado 16581.
                     * O percentual de fast delivery deve ser retirado do valor total do pedido antes que o percentual de desconto seja calculado. */
                    //String.Format("if(!coalesce(pp1.invisivelFluxo, false), coalesce({1}.total, {0}.total), {0}.total)", aliasPedido, aliasPedidoEspelho),
                    string.Format("if(!coalesce(pp1.invisivelFluxo, false), coalesce({1}.total, {0}.total), {0}.total) / (1 + COALESCE({0}.taxaFastDelivery / 100, 0))", aliasPedido, aliasPedidoEspelho),
                    string.Format("if(!coalesce(pp1.invisivelFluxo, false), coalesce({1}.valoricms, {0}.valoricms), {0}.valoricms)", aliasPedido, aliasPedidoEspelho),
                    string.Format("if(!coalesce(pp1.invisivelFluxo, false), coalesce({1}.valoripi, {0}.valoripi), {0}.valoripi)", aliasPedido, aliasPedidoEspelho),
                    icms + ipi);
            }

            // Tabelas da subquery
            campo.AppendFormat(@")
                from produtos_pedido pp1
                    inner join produtos_liberar_pedido plp1 on (pp1.idProdPed=plp1.idProdPed)
                    inner join liberarpedido lp1 on (plp1.idLiberarPedido=lp1.idLiberarPedido)
                    inner join cliente cli1 on (cli1.id_Cli=lp1.idCliente)
                where plp1.idProdLiberarPedido={0}.idProdLiberarPedido and plp1.qtdeCalc>0)){1}) as decimal(12,6))",
                aliasProdutosLiberarPedido,
                arredondarResultado ? ", 2" : "");

            return string.Format("coalesce({2}, {0}) as {1}",
                campoSemCalc,
                nomeCampo,
                campo.ToString());
        }

        /// <summary>
        /// Retorna o SQL do campo Total do pedido para usar nos relatórios considerando o valor liberado.
        /// </summary>
        public string SqlCampoTotalLiberacao(bool usarTotalCalc, string nomeCampoTotal, string aliasPedido,
            string aliasPcp, string aliasAmbientePedido, string aliasProdutosLiberarPedido)
        {
            return SqlCampoTotalLiberacao(usarTotalCalc, nomeCampoTotal, aliasPedido, aliasPcp,
                aliasAmbientePedido, aliasProdutosLiberarPedido, true);
        }

        /// <summary>
        /// Retorna o SQL do campo Total do pedido para usar nos relatórios considerando o valor liberado.
        /// </summary>
        internal string SqlCampoTotalLiberacao(bool usarTotalCalc, string nomeCampoTotal, string aliasPedido,
            string aliasPcp, string aliasAmbientePedido, string aliasProdutosLiberarPedido, bool arredondarResultado)
        {
            return SqlCampoCalcLiberacao(usarTotalCalc, "total", "total", true, "total", "valor", nomeCampoTotal,
                aliasPedido, aliasPcp, aliasAmbientePedido, aliasProdutosLiberarPedido, arredondarResultado);
        }

        /// <summary>
        /// Retorna o SQL do campo Total do pedido para usar nos relatórios considerando o valor liberado.
        /// </summary>
        internal string SqlCampoTotalLiberacao(bool usarTotalCalc, string campoSemCalc, string nomeCampoTotal, string aliasPedido,
            string aliasPcp, string aliasAmbientePedido, string aliasProdutosLiberarPedido, bool arredondarResultado)
        {
            return SqlCampoCalcLiberacao(usarTotalCalc, campoSemCalc, null, false, "total", "valor", nomeCampoTotal,
                aliasPedido, aliasPcp, aliasAmbientePedido, aliasProdutosLiberarPedido, arredondarResultado);
        }

        /// <summary>
        /// Retorna o SQL do campo Total do pedido para usar nos relatórios considerando o valor liberado.
        /// </summary>
        internal string SqlCampoCustoLiberacao(bool usarTotalCalc, string nomeCampoCusto, string aliasPedido,
            string aliasPcp, string aliasAmbientePedido, string aliasProdutosLiberarPedido)
        {
            return SqlCampoCustoLiberacao(usarTotalCalc, nomeCampoCusto, aliasPedido, aliasPcp, aliasAmbientePedido,
                aliasProdutosLiberarPedido, true);
        }

        /// <summary>
        /// Retorna o SQL do campo Total do pedido para usar nos relatórios considerando o valor liberado.
        /// </summary>
        internal string SqlCampoCustoLiberacao(bool usarTotalCalc, string nomeCampoCusto, string aliasPedido,
            string aliasPcp, string aliasAmbientePedido, string aliasProdutosLiberarPedido, bool arredondarResultado)
        {
            return SqlCampoCalcLiberacao(usarTotalCalc, "custoPedido", null, true, "custoProd", "custo", nomeCampoCusto,
                aliasPedido, aliasPcp, aliasAmbientePedido, aliasProdutosLiberarPedido, arredondarResultado);
        }

        /// <summary>
        /// Retorna o SQL do campo Total do pedido para usar nos relatórios considerando o valor liberado.
        /// </summary>
        internal string SqlCampoCustoLiberacao(bool usarTotalCalc, string campoSemCalc, string nomeCampoCusto, string aliasPedido,
            string aliasPcp, string aliasAmbientePedido, string aliasProdutosLiberarPedido, bool arredondarResultado)
        {
            return SqlCampoCalcLiberacao(usarTotalCalc, campoSemCalc, null, false, "custoProd", "custo", nomeCampoCusto,
                aliasPedido, aliasPcp, aliasAmbientePedido, aliasProdutosLiberarPedido, arredondarResultado);
        }

        #endregion

        #region Relatório de Pedidos por Situação

        internal string SqlRptSit(uint idPedido, string idsPedidos, uint idOrcamento, string codCliente, string idsRota, string idCliente, string nomeCliente,
            int tipoFiscal, string loja, string situacao, string dtIniSit, string dtFimSit, string dtIni, string dtFim, string dtIniEnt,
            string dtFimEnt, uint idFunc, uint idVendAssoc, string tipo, int tipoEntrega, int fastDelivery, int ordenacao,
            string situacaoProd, string tipoVenda, uint idGrupoProd, string idsSubgrupoProd, string idsBenef, bool exibirProdutos,
            bool exibirRota, bool pedidosSemAnexos, string dataIniPronto, string dataFimPronto, int numeroDiasDiferencaProntoLib,
            string dataIniInst, string dataFimInst, float altura, int largura, string codProd, string descrProd, string idsGrupos,
            string tipoCliente, bool trazerPedCliVinculado, int origemPedido, int desconto, bool selecionar, bool countLimit, bool forRpt,
            bool forGrafico, out bool temFiltro, out string filtroAdicional, int cidade, string comSemNF, int idMedidor,
            bool apenasLiberados, uint idOC, string nomeUsuCad, bool loginCliente, bool administrador, bool emitirGarantiaReposicao,
            bool emitirPedidoFuncionario)
        {
            temFiltro = false;
            filtroAdicional = "";
            var calcularLiberados = PedidoConfig.LiberarPedido;

            if (calcularLiberados && !string.IsNullOrEmpty(situacao))
                foreach (var s in situacao.Split(','))
                    if (s.StrParaInt() != (int)Pedido.SituacaoPedido.Confirmado && s.StrParaInt() != (int)Pedido.SituacaoPedido.LiberadoParcialmente)
                        calcularLiberados = false;

            var criterio = string.Empty;

            var itensProjeto = !selecionar || !PedidoConfig.LiberarPedido || countLimit ? "" :
                @"(SELECT CAST(GROUP_CONCAT(DISTINCT prodped.idItemProjeto) AS CHAR)
                    FROM produtos_pedido prodped WHERE prodped.idPedido=p.idPedido
                        AND (prodped.InvisivelFluxo IS NULL OR prodped.InvisivelFluxo=FALSE)) AS IdItensProjeto, ";

            var total = SqlCampoTotalLiberacao(selecionar && calcularLiberados && !countLimit, "total", "p", "pe", "ap", "plp");

            var custo = SqlCampoCustoLiberacao(PedidoConfig.LiberarPedido, "custoPedido", "p", "pe", "ap", "plp");

            /* Chamado 49970. */
            var camposFluxo = string.Format(@"CAST(COALESCE({0}.Total, {1}.Total) AS DECIMAL (12,2)) AS TotalReal,
                CAST(COALESCE({0}.Desconto, {1}.Desconto) AS DECIMAL (12,2)) AS Desconto,
                CAST(COALESCE({0}.TipoDesconto, {1}.TipoDesconto) AS DECIMAL (12,2)) AS TipoDesconto,
                CAST(COALESCE({0}.Acrescimo, {1}.Acrescimo) AS DECIMAL (12,2)) AS Acrescimo,
                CAST(COALESCE({0}.TipoAcrescimo, {1}.TipoAcrescimo) AS DECIMAL (12,2)) AS TipoAcrescimo,
                CAST(COALESCE({0}.Peso, {1}.Peso) AS DECIMAL (12,2)) AS Peso,
                CAST(COALESCE({0}.TotM, {1}.TotM) AS DECIMAL (12,2)) AS TotM,
                CAST(COALESCE({0}.AliquotaIpi, {1}.AliquotaIpi) AS DECIMAL (12,2)) AS AliquotaIpi,
                CAST(COALESCE({0}.ValorIpi, {1}.ValorIpi) AS DECIMAL (12,2)) AS ValorIpi,
                CAST(COALESCE({0}.AliquotaIcms, {1}.AliquotaIcms) AS DECIMAL (12,2)) AS AliquotaIcms,
                CAST(COALESCE({0}.ValorIcms, {1}.ValorIcms) AS DECIMAL (12,2)) AS ValorIcms, ", PCPConfig.UsarConferenciaFluxo ? "pe" : "p", PCPConfig.UsarConferenciaFluxo ? "p" : "pe");

            var campos = selecionar ?
                (!forGrafico ?
                camposFluxo +

                @"p.idPedido, p.idLoja, p.idFunc, p.idCli, p.idFormaPagto, p.idOrcamento, " + total + ", " +
                itensProjeto + @"p.prazoEntrega, p.tipoEntrega, p.tipoVenda, p.dataEntrega, p.valorEntrega, p.situacao, p.valorEntrada,
                p.dataCad, p.usuCad, p.numParc, p.obs, " + custo + @", p.dataConf, p.usuConf, p.dataCanc, p.usuCanc, p.enderecoObra, 
                p.bairroObra, p.cidadeObra, p.localObra, p.idFormaPagto2, p.idTipoCartao, p.idTipoCartao2, p.codCliente, p.numAutConstrucard, 
                p.idComissionado, p.percComissao, p.valorComissao, p.idPedidoAnterior, p.fastDelivery, p.dataPedido,
                p.idObra, p.idMedidor, p.taxaPrazo, p.tipoPedido, p.taxaFastDelivery, 
                p.temperaFora, p.situacaoProducao, p.idFuncVenda, p.dataEntregaOriginal, p.geradoParceiro, 
                " + ClienteDAO.Instance.GetNomeCliente("c") + @" as NomeCliente, f.Nome as NomeFunc, l.NomeFantasia as nomeLoja, l.telefone as TelefoneLoja, fp.Descricao as FormaPagto, 
                cid.nomeCidade as cidade, p.valorCreditoAoConfirmar, p.creditoGeradoConfirmar, p.creditoUtilizadoConfirmar, '$$$' as Criterio, 
                c.idFunc as idFuncCliente, fc.nome as nomeFuncCliente, p.idParcela, p.cepObra, p.idSinal, p.idPagamentoAntecipado,
                p.valorPagamentoAntecipado, p.dataPronto, p.obsLiberacao, p.idProjeto, p.idLiberarPedido, p.idFuncDesc, p.dataDesc,
                p.importado, c.email, p.percentualComissao,
                p.rotaExterna, p.clienteExterno, p.pedCliExterno, p.celCliExterno, p.totalPedidoExterno, p.deveTransferir, p.dataFin, p.usuFin" +

                (exibirRota ? @", (Select r.codInterno From rota r Where r.idRota In 
                    (Select rc.idRota From rota_cliente rc Where rc.idCliente=p.idCli)) As codRota" : "") :

                camposFluxo +
                "p.idLoja, p.idFunc, p.idCli, " + total + @", f.nome as nomeFunc, c.idFunc as idFuncCliente, fc.nome as nomeFuncCliente, 
                l.nomeFantasia as nomeLoja, p.tipoPedido, p.situacao, p.dataConf, com.idComissionado, com.nome as nomeComissionado, lp.dataLiberacao, 
                " + ClienteDAO.Instance.GetNomeCliente("c") + @" as nomeCliente, p.importado, 
                '$$$' as Criterio") : "Count(*)";

            var usarDadosVendidos = !loginCliente && selecionar && exibirProdutos && !countLimit;

            var whereDadosVendidos = "";
            var campoDadosVendidos = !usarDadosVendidos ? "" :
                ", trim(dv.dadosVidrosVendidos) as dadosVidrosVendidos";

            var dadosVendidos = !usarDadosVendidos ? "" :
                @"Left Join (
                    select idCli, idPedido, concat(cast(group_concat(concat('\n* ', codInterno, ' - ', descricao, ': Qtde ', qtde, '  Tot. m² ', 
                        totM2)) as char), rpad('', 100, ' ')) as dadosVidrosVendidos
                    from (
                        select ped.idCli, ped.idPedido, pp.idProd, p.codInterno, p.descricao, 
                            replace(cast(sum(pp.totM) as char), '.', ',') as totM2, cast(sum(pp.qtde) as signed) as qtde
                        from produtos_pedido pp
                            inner join pedido ped on (pp.idPedido=ped.idPedido)
                            inner join cliente cli on (ped.idCli=cli.id_cli)
                            inner join produto p on (pp.idProd=p.idProd)
                            left join produtos_liberar_pedido plp1 on (pp.idProdPed=plp1.idProdPed)
                            left join liberarpedido lp1 on (plp1.idLiberarPedido=lp1.idLiberarPedido)
                        where p.idGrupoProd=1 
                            and (invisivelFluxo IS NULL or invisivelFluxo=FALSE)
                            {0}
                        group by ped.idCli, pp.idProd
                    ) as temp
                    group by idCli
                ) as dv on (dv.idCli=p.idCli)";

            var sql = string.Format(@"
                SELECT {0}
                FROM pedido p 
                    INNER JOIN cliente c ON (p.IdCli=c.Id_Cli)
                    LEFT JOIN pedido_espelho pe ON (p.IdPedido=pe.IdPedido)
                    LEFT JOIN produtos_pedido pp ON (p.IdPedido=pp.IdPedido)
                    LEFT JOIN produtos_liberar_pedido plp ON (pp.IdProdPed=plp.IdProdPed)
                    LEFT JOIN liberarpedido lp ON (plp.IdLiberarPedido=lp.IdLiberarPedido AND COALESCE(lp.Situacao, 1)=1)
                    LEFT JOIN ambiente_pedido ap ON (pp.IdAmbientePedido=ap.IdAmbientePedido)
                    LEFT JOIN funcionario fc ON (c.IdFunc=fc.IdFunc)
                    LEFT JOIN cidade cid ON (c.IdCidade=cid.IdCidade) 
                    LEFT JOIN funcionario f ON (p.IdFunc=f.IdFunc) 
                    LEFT JOIN loja l ON (p.IdLoja = l.IdLoja) 
                    LEFT JOIN formapagto fp ON (fp.IdFormaPagto=p.IdFormaPagto)
                    LEFT JOIN comissionado com ON (p.IdComissionado=com.IdComissionado)
                    {1}
                WHERE 1 ?filtroAdicional?",
                    campos + campoDadosVendidos, dadosVendidos);

            if (cidade > 0)
            {
                sql += " And c.IdCidade=" + cidade;
                criterio += "Cidade: " + CidadeDAO.Instance.GetNome(Convert.ToUInt32(cidade)) + "    ";
                temFiltro = true;
            }

            if (idPedido > 0)
            {
                filtroAdicional += " And p.idPedido=" + idPedido;
                whereDadosVendidos += " and ped.idPedido=" + idPedido;
                criterio += "Pedido: " + idPedido + "    ";
            }
            else if (!string.IsNullOrEmpty(idsPedidos))
            {
                filtroAdicional += " And p.idPedido In (" + idsPedidos.TrimEnd(',') + ")";
                whereDadosVendidos += " And ped.idPedido In (" + idsPedidos.TrimEnd(',') + ")";
                criterio += "Pedidos selecionados    ";
            }

            if(idOrcamento > 0)
            {
                filtroAdicional += " And p.idOrcamento=" + idOrcamento;
                whereDadosVendidos += " And ped.idOrcamento=" + idOrcamento;
                criterio += "Orçamento: " + idOrcamento + "    ";
            }

            if (!string.IsNullOrEmpty(codCliente))
            {
                filtroAdicional += @"
                    AND (p.CodCliente LIKE ?codCliente OR p.IdPedido IN
                        (SELECT pp1.IdPedido FROM produtos_pedido pp1 WHERE pp1.PedCli LIKE ?codCliente
                            AND (pp1.InvisivelFluxo IS NULL OR pp1.InvisivelFluxo=FALSE)))";

                whereDadosVendidos += " And (ped.codCliente like ?codCliente Or pp.pedCli like ?codCliente)";
                criterio += "Pedido Cli.: " + codCliente + "    ";
            }

            if (!string.IsNullOrEmpty(idsRota))
            {
                filtroAdicional += string.Format(" AND p.IdCli IN (SELECT IdCliente FROM rota_cliente WHERE IdRota IN ({0}))", idsRota);
                whereDadosVendidos += string.Format(" AND ped.IdCli IN (SELECT IdCliente FROM rota_cliente WHERE IdRota IN ({0}))", idsRota);

                criterio += string.Format("Rota(s): {0}    ", RotaDAO.Instance.ObtemCodRotas(idsRota));
            }

            if (!string.IsNullOrEmpty(idCliente))
            {
                if (trazerPedCliVinculado)
                {
                    var idsVinculados = ClienteVinculoDAO.Instance.GetIdsVinculados(idCliente.StrParaUint());
                    if (!string.IsNullOrEmpty(idsVinculados))
                    {
                        filtroAdicional += " And p.idCli in (" + idCliente + "," + idsVinculados + ")";
                        whereDadosVendidos += " and ped.idCli in (" + idCliente + "," + idsVinculados + ")";
                        criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCliente.StrParaUint()) +
                            " e pedidos de clientes vinculados.    ";
                    }
                    else
                    {
                        filtroAdicional += " And p.IdCli=" + idCliente;
                        whereDadosVendidos += " and ped.idCli=" + idCliente;
                        criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCliente.StrParaUint()) + "    ";
                    }
                }
                else
                {
                    filtroAdicional += " And p.IdCli=" + idCliente;
                    whereDadosVendidos += " and ped.idCli=" + idCliente;
                    criterio += "Cliente: " + ClienteDAO.Instance.GetNome(idCliente.StrParaUint()) + "    ";
                }
            }
            else if (!string.IsNullOrEmpty(nomeCliente))
            {
                var ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);
                filtroAdicional += " And p.idCli in (" + ids + ")";
                whereDadosVendidos += " and ped.idCli in (" + ids + ")";
                criterio += "Cliente: " + nomeCliente + "    ";
            }

            if (tipoFiscal > 0)
            {
                filtroAdicional += " And c.tipoFiscal=" + tipoFiscal;
                whereDadosVendidos += " And cli.tipoFiscal=" + tipoFiscal;

                switch (tipoFiscal)
                {
                    case (int)TipoFiscalCliente.ConsumidorFinal: criterio += "Tipo Fiscal: Consumidor Final    "; break;
                    case (int)TipoFiscalCliente.Revenda: criterio += "Tipo Fiscal: Revenda    "; break;
                }
            }

            if (!string.IsNullOrEmpty(loja) && loja.ToLower() != "todas" && loja != "0")
            {
                filtroAdicional += " And p.IdLoja=" + loja;
                whereDadosVendidos += " and ped.idLoja=" + loja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(loja.StrParaUint()) + "    ";
            }

            if (!string.IsNullOrEmpty(situacao))
            {
                filtroAdicional += " And p.situacao In (" + situacao + ")";
                whereDadosVendidos += " And ped.situacao In (" + situacao + ")";
                criterio += "Situação: " + PedidoDAO.Instance.GetSituacaoPedido(situacao) + "    ";
            }

            if (desconto > 0)
            {
                filtroAdicional += " And (p.desconto" + (desconto == 1 ? ">0" : "=0") +
                    " Or p.idPedido In (Select idPedido From ambiente_pedido Where desconto" + (desconto == 1 ? ">0" : "=0") + "))";
                whereDadosVendidos += " And (ped.desconto" + (desconto == 1 ? ">0" : "=0") +
                    " Or ped.idPedido In (Select idPedido From ambiente_pedido Where desconto" + (desconto == 1 ? ">0" : "=0") + "))";
                criterio += desconto == 1 ? "Pedidos com desconto    " : "Pedidos sem desconto    ";
            }

            if (idMedidor > 0)
            {
                filtroAdicional += " And p.idMedidor=" + idMedidor.ToString();
                criterio += "Medidor: " + FuncionarioDAO.Instance.GetNome(idMedidor.ToString().StrParaUint()) + "      ";
            }

            #region Filtro por data de situação

            var filtro = FiltroDataSituacao(dtIniSit, dtFimSit, situacao, "?dtIniSit", "?dtFimSit", "p", "lp", " Sit.", calcularLiberados);
            sql += filtro.Sql;
            criterio += filtro.Criterio;
            whereDadosVendidos += FiltroDataSituacao(dtIniSit, dtFimSit, situacao, "?dtIniSit", "?dtFimSit", "ped", "lp1", " Sit.", true).Sql;
            temFiltro = temFiltro || filtro.Sql != "";

            #endregion

            #region Filtro por data do pedido e de entrega

            if (!string.IsNullOrEmpty(dtIni))
            {
                filtroAdicional += " And p.DataPedido>=?dtIni";
                whereDadosVendidos += " And ped.DataPedido>=?dtIni";
                criterio += "Data Início: " + dtIni + "    ";
            }

            if (!string.IsNullOrEmpty(dtFim))
            {
                filtroAdicional += " And p.DataPedido<=?dtFim";
                whereDadosVendidos += " And ped.DataPedido<=?dtFim";
                criterio += "Data Fim: " + dtFim + "    ";
            }

            if (!string.IsNullOrEmpty(dtIniEnt))
            {
                filtroAdicional += " And p.DataEntrega>=?dtIniEnt";
                whereDadosVendidos += " And ped.DataEntrega>=?dtIniEnt";
                criterio += "Data Início Entrega: " + dtIniEnt + "    ";
            }

            if (!string.IsNullOrEmpty(dtFimEnt))
            {
                filtroAdicional += " And p.DataEntrega<=?dtFimEnt";
                whereDadosVendidos += " And ped.DataEntrega<=?dtFimEnt";
                criterio += "Data Fim Entrega: " + dtFimEnt + "    ";
            }

            #endregion

            if (idFunc > 0)
            {
                filtroAdicional += " and p.idFunc=" + idFunc;
                whereDadosVendidos += " and ped.idFunc=" + idFunc;
                criterio += "Funcionário: " + FuncionarioDAO.Instance.GetNome(idFunc) + "    ";
            }

            if (idVendAssoc > 0)
            {
                sql += " and c.idFunc=" + idVendAssoc;
                whereDadosVendidos += " and cli.idFunc=" + idVendAssoc;
                criterio += "Vendedor associado: " + FuncionarioDAO.Instance.GetNome(idVendAssoc) + "    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(tipoCliente))
            {
                sql += " and c.idTipoCliente in (" + tipoCliente + ")";
                whereDadosVendidos += " and cli.idTipoCliente in (" + tipoCliente + ")";
                criterio += "Tipo de Cliente: " + TipoClienteDAO.Instance.GetNomes(tipoCliente) + "    ";
                temFiltro = true;
            }

            if (tipoEntrega > 0)
            {
                filtroAdicional += " and p.tipoEntrega=" + tipoEntrega;
                whereDadosVendidos += " and ped.tipoEntrega=" + tipoEntrega;
                criterio += "Tipo Entrega: " + Utils.GetDescrTipoEntrega(tipoEntrega) + "    ";
            }

            if (!string.IsNullOrEmpty(tipo))
            {
                filtroAdicional += " and p.tipoPedido in (" + tipo + ")";
                whereDadosVendidos += " and ped.tipoPedido in (" + tipo + ")";
                criterio += "Tipo: ";

                if (("," + tipo + ",").Contains(",1,"))
                    criterio += "Venda, ";

                if (("," + tipo + ",").Contains(",2,"))
                    criterio += "Revenda, ";

                if (("," + tipo + ",").Contains(",3,"))
                    criterio += "Mão de obra, ";

                if (("," + tipo + ",").Contains(",4,"))
                    criterio += "Produção, ";

                criterio = criterio.TrimEnd(',', ' ') + "    ";
            }

            if (fastDelivery > 0)
            {
                filtroAdicional += " and (FastDelivery" + (fastDelivery == 1 ? "=true" : "=false or FastDelivery is null") + ")";
                whereDadosVendidos += " and (FastDelivery" + (fastDelivery == 1 ? "=true" : "=false or FastDelivery is null") + ")";
                criterio += "Fast Delivery: " + (fastDelivery == 1 ? "Sim" : "Não");
            }

            if (!string.IsNullOrEmpty(situacaoProd))
            {
                filtroAdicional += " And p.situacaoProducao In (" + situacaoProd + ")";

                whereDadosVendidos += " And ped.situacaoProducao In (" + situacaoProd + ")";

                criterio += "Situação Prod.: " + GetSituacaoProdPedido(situacaoProd) + "    ";
            }

            if (!string.IsNullOrEmpty(tipoVenda))
            {
                filtroAdicional += " and p.tipoVenda in (" + tipoVenda + ")";
                whereDadosVendidos += " and ped.tipoVenda in (" + tipoVenda + ")";
                criterio += "Tipo de venda: ";

                foreach (var g in DataSources.Instance.GetTipoVenda(false, true, administrador, emitirGarantiaReposicao, emitirPedidoFuncionario))
                    if (("," + tipoVenda + ",").Contains("," + g.Id + ","))
                        criterio += g.Descr + ", ";

                criterio = criterio.TrimEnd(',', ' ') + "    ";
            }

            if (idGrupoProd > 0)
            {
                filtroAdicional += " and pp.idProd in (select idProd From produto where idGrupoProd=" + idGrupoProd + ")";
                whereDadosVendidos += " and p.idGrupoProd=" + idGrupoProd;
                criterio += "Grupo: " + GrupoProdDAO.Instance.GetDescricao((int)idGrupoProd) + "    ";
            }

            if (!string.IsNullOrEmpty(idsGrupos) && idsGrupos != "0")
            {
                filtroAdicional += " and pp.idProd in (select idProd From produto where idGrupoProd In (" + idsGrupos + "))";
                whereDadosVendidos += " and p.idGrupoProd in (" + idsGrupos + ")";

                criterio += "Grupos: ";

                foreach (var id in idsGrupos.Split(','))
                    criterio += GrupoProdDAO.Instance.GetDescricao(id.StrParaInt()) + ", ";

                criterio = criterio.TrimEnd(' ', ',') + "    ";
            }

            if (!string.IsNullOrEmpty(idsSubgrupoProd) && idsSubgrupoProd != "0")
            {
                filtroAdicional += @" 
                    AND p.IdPedido IN
                        (SELECT pp1.IdPedido FROM produtos_pedido pp1
                            INNER JOIN produto prod ON (pp1.IdProd=prod.IdProd)
                        WHERE prod.IdSubgrupoProd IN(" + idsSubgrupoProd + ") AND (pp1.InvisivelFluxo IS NULL OR pp1.InvisivelFluxo=FALSE))";

                whereDadosVendidos += " and p.idSubgrupoProd IN(" + idsSubgrupoProd + ")";

                foreach (var id in idsSubgrupoProd.Split(','))
                    criterio += SubgrupoProdDAO.Instance.GetDescricao(id.StrParaInt()) + ", ";

                criterio = criterio.TrimEnd(' ', ',') + "    ";
            }

            if (pedidosSemAnexos)
            {
                filtroAdicional += " and p.idPedido not in (Select idPedido from fotos_pedido)";
                whereDadosVendidos += " and ped.idPedido not in (Select idPedido from fotos_pedido)";
                criterio += "Pedidos sem anexos    ";
            }

            if (!string.IsNullOrEmpty(idsBenef))
            {
                var redondo = BenefConfigDAO.Instance.TemBenefRedondo(idsBenef) ? " or redondo=true" : "";
                filtroAdicional += @"
                    AND p.IdPedido IN
                        (SELECT pp1.IdPedido FROM produtos_pedido pp1 WHERE (pp1.InvisivelFluxo IS NULL OR pp1.InvisivelFluxo=FALSE) AND pp1.IdProdPed IN
                            (SELECT DISTINCT ppb1.IdProdPed FROM produto_pedido_benef ppb1
                            WHERE ppb1.idBenefConfig IN (" + idsBenef + "))" + redondo + ")";

                whereDadosVendidos += " and (pp.idProdPed in (" +
                    "select distinct idProdPed from produto_pedido_benef where idBenefConfig in (" + idsBenef + "))" + redondo + ")";

                criterio += "Beneficiamentos: " + BenefConfigDAO.Instance.GetDescrBenef(idsBenef) + "    ";
            }

            if (!string.IsNullOrEmpty(dataIniPronto))
            {
                filtroAdicional += " and p.dataPronto>=?dataIniPronto";
                criterio += "Data Pronto Início: " + dataIniPronto + "    ";
            }

            if (!string.IsNullOrEmpty(dataFimPronto))
            {
                filtroAdicional += " and p.dataPronto<=?dataFimPronto";
                criterio += "Data Pronto Fim: " + dataFimPronto + "    ";
            }

            if (numeroDiasDiferencaProntoLib > 0)
            {
                sql += " and datediff(min(lp.dataLiberacao), p.dataPronto)>=" + numeroDiasDiferencaProntoLib;
                criterio += "Diferença dias entre Pedido Pronto e Liberado: " + numeroDiasDiferencaProntoLib + "    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(dataIniInst))
            {
                filtroAdicional += " and p.idPedido in (select idPedido from instalacao where dataFinal>=?dataIniInst)";
                criterio += "Data Instalação Início: " + dataIniInst + "    ";
            }

            if (!string.IsNullOrEmpty(dataFimInst))
            {
                if (!string.IsNullOrEmpty(dataIniInst))
                    filtroAdicional = filtroAdicional.TrimEnd(')') + " and dataFinal<=?dataFimInst)";
                else
                    filtroAdicional += " and p.idPedido in (select idPedido from instalacao where dataFinal<=?dataFimInst)";

                criterio += "Data Instalação Fim: " + dataFimInst + "    ";
            }

            if (altura > 0)
            {
                filtroAdicional += @"
                    AND p.IdPedido IN
                        (SELECT pp1.IdPedido FROM produtos_pedido pp1
                        WHERE pp1.Altura=" + altura.ToString().Replace(",", ".") + " AND (pp1.InvisivelFluxo IS NULL OR pp1.InvisivelFluxo=FALSE))";

                criterio += "Altura Produto: " + altura + "    ";
            }

            if (largura > 0)
            {
                filtroAdicional += @"
                    AND p.IdPedido IN
                        (SELECT pp1.IdPedido FROM produtos_pedido pp1
                        WHERE pp1.Largura=" + largura.ToString().Replace(",", ".") + " AND (pp1.InvisivelFluxo IS NULL OR pp1.InvisivelFluxo=FALSE))";

                criterio += "Largura Produto: " + largura + "    ";
            }

            if (!string.IsNullOrEmpty(codProd))
            {
                var idProd = ProdutoDAO.Instance.ObtemIdProd(codProd);

                filtroAdicional += string.Format(
                    @" AND p.IdPedido IN 
                        (SELECT IdPedido FROM produtos_pedido pp1
                        WHERE pp1.idProd={0} AND (pp1.InvisivelFluxo IS NULL OR pp1.InvisivelFluxo=FALSE))",
                    idProd);

                criterio += "Produto: " + codProd + " - " + ProdutoDAO.Instance.GetDescrProduto(codProd) + "    ";
            }
            else if (!string.IsNullOrEmpty(descrProd))
            {
                filtroAdicional +=
                    @" AND p.IdPedido IN 
                        (SELECT pp1.IdPedido FROM produtos_pedido pp1
                            INNER JOIN produto p ON (pp1.IdProd=p.IdProd) 
                        WHERE p.Descricao LIKE ?descrProd AND (pp1.InvisivelFluxo IS NULL OR pp1.InvisivelFluxo=FALSE))";

                criterio += "Produto: " + descrProd;
            }

            if (origemPedido == 1)
            {
                filtroAdicional += " AND ((p.Importado IS NOT NULL AND p.Importado = 1) OR (p.GeradoParceiro IS NOT NULL AND p.GeradoParceiro = 1)) ";
                criterio += "Origem Pedido: ecommerce";
            }

            if (origemPedido == 2)
            {
                filtroAdicional += " AND ((p.Importado IS NULL OR p.Importado = 0) AND (p.GeradoParceiro IS NULL OR p.GeradoParceiro = 0)) ";
                criterio += "Origem Pedido: normal";
            }

            if (comSemNF == "1") // Com NF gerada
            {
                filtroAdicional += "AND p.IdPedido IN (SELECT * FROM (SELECT pnf1.IdPedido FROM pedidos_nota_fiscal pnf1 WHERE pnf1.IdPedido IS NOT NULL) AS comNf)";
                criterio += "Pedidos com nota fiscal gerada    ";
            }
            else if (comSemNF == "2") // Sem NF gerada
            {
                filtroAdicional += "AND p.IdPedido NOT IN (SELECT * FROM (SELECT pnf1.IdPedido FROM pedidos_nota_fiscal pnf1 WHERE pnf1.IdPedido IS NOT NULL) AS semNf)";
                criterio += "Pedidos sem nota fiscal gerada    ";
            }

            if (idOC > 0)
            {
                filtroAdicional += " AND EXISTS (select * from pedido_ordem_carga where idPedido=p.idPedido and idOrdemCarga=" + idOC + ")";
                criterio += "Ordem de Carga: " + idOC + "     ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(nomeUsuCad))
            {
                filtroAdicional += " AND p.usuCad IN (SELECT f.idFunc FROM funcionario f WHERE f.nome like ?nomeUsuCad)";
                criterio += "Usuário Cad.: " + nomeUsuCad.ToUpper() + "     ";
            }

            if (selecionar)
            {
                sql += " group by p.idPedido";

                if (forRpt)
                    switch (ordenacao)
                    {
                        case 1:
                            sql += " Order By DataEntrega Asc";
                            break;
                        case 2:
                            sql += " Order By DataEntrega Desc";
                            break;
                        default:
                            sql += " Order By DataPedido Desc, DataCad Desc";
                            break;
                    }
            }

            sql = string.Format(sql, whereDadosVendidos);
            return sql.Replace("$$$", criterio);
        }

        #endregion

        #region Listagem/Relatório de vendas de pedidos

        #region SQL

        /// <summary>
        /// Retorna o SQL da tela de vendas de pedidos, aplicando os filtros de acordo com os parâmetros informados.
        /// </summary>
        private string SqlVendasPedidos(float? altura, int? cidade, string codCliente, string codigoProduto, string comSemNF, bool countLimit, string dataFimEntrega,
            string dataFimInstalacao, string dataFimPedido, string dataFimPronto, string dataFimSituacao, string dataInicioEntrega, string dataInicioInstalacao, string dataInicioPedido,
            string dataInicioPronto, string dataInicioSituacao, int? desconto, string descricaoProduto, bool exibirProdutos, bool exibirRota, int? fastDelivery, out string filtroAdicional, int? idCarregamento,
            string idCliente, int? idFunc, int? idMedidor, int? idOC, int? idOrcamento, int? idPedido, string idsBenef, string idsGrupo, string idsPedidos, string idsRota, string idsSubgrupoProd,
            int? idVendAssoc, int? largura, LoginUsuario login, string loja, string nomeCliente, int? numeroDiasDiferencaProntoLib, string observacao, int? ordenacao, int? origemPedido,
            bool paraRelatorio, bool pedidosSemAnexos, string situacao, string situacaoProducao, out bool temFiltro, string tiposPedido, string tipoCliente, int? tipoEntrega,
            int? tipoFiscal, string tiposVenda, bool totaisListaPedidos, bool trazerPedCliVinculado, int? usuarioCadastro)
        {
            login = login ?? UserInfo.GetUserInfo;

            temFiltro = false;
            filtroAdicional = string.Empty;
            var criterio = string.Empty;
            var formatoCriterio = "{0} {1}    ";
            var filtroFluxoProdutoPedido = string.Format("(Invisivel{0} IS NULL OR Invisivel{0}=0)", PCPConfig.UsarConferenciaFluxo ? "Fluxo" : "Pedido");

            var emitirGarantiaReposicao = Config.PossuiPermissao((int)login.CodUser, Config.FuncaoMenuPedido.EmitirPedidoGarantiaReposicao);
            var emitirPedidoFuncionario = Config.PossuiPermissao((int)login.CodUser, Config.FuncaoMenuPedido.EmitirPedidoFuncionario);
            var administrador = login.IsAdministrador;
            var calcularLiberados = PedidoConfig.LiberarPedido;

            if (calcularLiberados && !string.IsNullOrEmpty(situacao))
                foreach (var s in situacao.Split(','))
                    if (s.StrParaInt() != (int)Pedido.SituacaoPedido.Confirmado && s.StrParaInt() != (int)Pedido.SituacaoPedido.LiberadoParcialmente)
                        calcularLiberados = false;

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

            var campos = string.Format(@"{0}, p.IdPedido, p.IdLoja, p.IdFunc, p.IdCli, p.IdFormaPagto, p.IdOrcamento, p.PrazoEntrega, p.TipoEntrega, p.TipoVenda, p.DataEntrega, p.ValorEntrega,
                p.Situacao, p.ValorEntrada, p.DataCad, p.UsuCad, p.NumParc, p.Obs, p.DataConf, p.UsuConf, p.DataCanc, p.UsuCanc, p.EnderecoObra, p.BairroObra, p.CidadeObra, p.LocalObra,
                p.IdFormaPagto2, p.IdTipoCartao, p.IdTipoCartao2, p.CodCliente, p.NumAutConstrucard, p.IdComissionado, p.PercComissao, p.ValorComissao, p.IdPedidoAnterior, p.FastDelivery,
                p.DataPedido, p.IdObra, p.IdMedidor, p.TaxaPrazo, p.TipoPedido, p.TaxaFastDelivery, p.TemperaFora, p.SituacaoProducao, p.IdFuncVenda, p.DataEntregaOriginal, p.GeradoParceiro,
                {1} AS NomeCliente, f.Nome AS NomeFunc, l.NomeFantasia AS NomeLoja, l.Telefone AS TelefoneLoja, fp.Descricao AS FormaPagto, cid.NomeCidade AS Cidade, p.ValorCreditoAoConfirmar,
                p.CreditoGeradoConfirmar, p.CreditoUtilizadoConfirmar, c.IdFunc AS IdFuncCliente, fc.Nome AS NomeFuncCliente, p.IdParcela, p.CepObra, p.IdSinal, p.idPagamentoAntecipado,
                p.ValorPagamentoAntecipado, p.DataPronto, p.ObsLiberacao, p.IdProjeto, p.IdLiberarPedido, p.IdFuncDesc, p.DataDesc, p.Importado, c.Email, p.PercentualComissao, p.RotaExterna,
                p.ClienteExterno, p.PedCliExterno, p.CelCliExterno, p.TotalPedidoExterno, p.DeveTransferir, p.DataFin, p.UsuFin {2}, '$$$' AS Criterio",
                camposFluxo, ClienteDAO.Instance.GetNomeCliente("c"),
                exibirRota ? ", (SELECT r.CodInterno FROM rota r WHERE r.IdRota IN (SELECT rc.IdRota FROM rota_cliente rc WHERE rc.IdCliente=p.IdCli)) AS CodRota" : string.Empty);

            var usarDadosVendidos = exibirProdutos && !countLimit;

            var whereDadosVendidos = string.Empty;
            var campoDadosVendidos = usarDadosVendidos ? ", TRIM(dv.DadosVidrosVendidos) AS DadosVidrosVendidos" : string.Empty;

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
                            LEFT JOIN produtos_liberar_pedido plp1 ON (pp.IdProdPed=plp1.IdProdPed)
                            LEFT JOIN liberarpedido lp1 ON (plp1.IdLiberarPedido=lp1.IdLiberarPedido)
                        WHERE p.IdGrupoProd=1 AND {0}
                            {1}
                        GROUP BY ped.IdCli, pp.IdProd
                    ) AS Temp
                    GROUP BY IdCli
                ) AS dv ON (dv.IdCli=p.IdCli)", filtroFluxoProdutoPedido, "{0}") : string.Empty;

            var sql = string.Format(@"
                SELECT {0}
                FROM pedido p 
                    INNER JOIN cliente c ON (p.IdCli=c.Id_Cli)
                    LEFT JOIN pedido_espelho pe ON (p.IdPedido=pe.IdPedido)
                    LEFT JOIN produtos_liberar_pedido plp ON (p.IdPedido=plp.IdPedido)
                    LEFT JOIN liberarpedido lp ON (plp.IdLiberarPedido=lp.IdLiberarPedido AND lp.Situacao IS NOT NULL AND lp.Situacao=1)
                    LEFT JOIN funcionario fc ON (c.IdFunc=fc.IdFunc)
                    LEFT JOIN cidade cid ON (c.IdCidade=cid.IdCidade) 
                    LEFT JOIN funcionario f ON (p.IdFunc=f.IdFunc) 
                    LEFT JOIN loja l ON (p.IdLoja = l.IdLoja)
                    LEFT JOIN formapagto fp ON (fp.IdFormaPagto=p.IdFormaPagto)
                    {1}
                WHERE 1 ?filtroAdicional?", totaisListaPedidos ? camposFluxo : string.Format("{0}{1}", campos, campoDadosVendidos), dadosVendidos);

            // Recupera o tipo de usuário
            uint tipoUsuario = UserInfo.GetUserInfo.TipoUsuario;

            if (PedidoConfig.DadosPedido.ListaApenasPedidosVendedor &&
                tipoUsuario == (uint)Data.Helper.Utils.TipoFuncionario.Vendedor)
            {
                filtroAdicional += string.Format(" AND (p.UsuCad={0} OR p.IdFunc={0})", UserInfo.GetUserInfo.CodUser);
                whereDadosVendidos += string.Format(" AND ped.UsuCad={0}", UserInfo.GetUserInfo.CodUser);

                criterio += string.Format(formatoCriterio, "Vendedor:", UserInfo.GetUserInfo.Nome);
            }

            if (cidade > 0)
            {
                sql += string.Format(" AND c.IdCidade={0}", cidade);
                criterio += string.Format(formatoCriterio, "Cidade:", CidadeDAO.Instance.GetNome(Convert.ToUInt32(cidade)));
                temFiltro = true;
            }

            if (idPedido > 0)
            {
                filtroAdicional += string.Format(" AND p.IdPedido={0}", idPedido);
                whereDadosVendidos += string.Format(" AND ped.IdPedido={0}", idPedido);

                criterio += string.Format(formatoCriterio, "Pedido:", idPedido);
            }
            else if (!string.IsNullOrWhiteSpace(idsPedidos))
            {
                filtroAdicional += string.Format(" AND p.IdPedido In ({0})", idsPedidos.TrimEnd(','));
                whereDadosVendidos += string.Format(" AND ped.IdPedido IN ({0})", idsPedidos.TrimEnd(','));

                criterio += string.Format(formatoCriterio, "Pedidos selecionados", string.Empty);
            }

            if (idOrcamento > 0)
            {
                filtroAdicional += string.Format(" AND p.IdOrcamento={0}", idOrcamento);
                whereDadosVendidos += string.Format(" AND ped.IdOrcamento={0}", idOrcamento);

                criterio += string.Format(formatoCriterio, "Orçamento:", idOrcamento);
            }

            if (!string.IsNullOrWhiteSpace(codCliente))
            {
                filtroAdicional += string.Format(@" AND (p.CodCliente LIKE ?codCliente OR p.IdPedido IN
                    (SELECT DISTINCT pp1.IdPedido FROM produtos_pedido pp1 WHERE pp1.PedCli LIKE ?codCliente AND {0}))", filtroFluxoProdutoPedido);
                whereDadosVendidos += " AND (ped.CodCliente LIKE ?codCliente OR pp.PedCli LIKE ?codCliente)";

                criterio += string.Format(formatoCriterio, "Pedido Cli.:", codCliente);
            }

            if (idCarregamento > 0)
            {
                filtroAdicional += string.Format(@" AND p.IdPedido in (Select Distinct(idpedido) from item_carregamento where idcarregamento = {0})", idCarregamento);
            }

            if(!observacao.IsNullOrEmpty())
                filtroAdicional += " AND p.Obs LIKE ?observacao";


            if (!string.IsNullOrWhiteSpace(idsRota))
            {
                filtroAdicional += string.Format(" AND p.IdCli IN (SELECT DISTINCT IdCliente FROM rota_cliente WHERE IdRota IN ({0}))", idsRota);
                whereDadosVendidos += string.Format(" AND ped.IdCli IN (SELECT DISTINCT IdCliente FROM rota_cliente WHERE IdRota IN ({0}))", idsRota);

                criterio += string.Format(formatoCriterio, "Rota(s):", RotaDAO.Instance.ObtemCodRotas(idsRota));
            }

            if (!string.IsNullOrWhiteSpace(idCliente))
            {
                if (trazerPedCliVinculado)
                {
                    var idsVinculados = ClienteVinculoDAO.Instance.GetIdsVinculados(idCliente.StrParaUint());

                    if (!string.IsNullOrWhiteSpace(idsVinculados))
                    {
                        filtroAdicional += string.Format(" AND p.IdCli IN ({0},{1})", idCliente, idsVinculados);
                        whereDadosVendidos += string.Format(" AND ped.IdCli IN ({0},{1})", idCliente, idsVinculados);

                        criterio += string.Format(formatoCriterio, "Cliente:", string.Format("{0} e pedidos de clientes vinculados.", ClienteDAO.Instance.GetNome(idCliente.StrParaUint())));
                    }
                    else
                    {
                        filtroAdicional += string.Format(" AND p.IdCli={0}", idCliente);
                        whereDadosVendidos += string.Format(" AND ped.IdCli={0}", idCliente);

                        criterio += string.Format(formatoCriterio, "Cliente:", ClienteDAO.Instance.GetNome(idCliente.StrParaUint()));
                    }
                }
                else
                {
                    filtroAdicional += string.Format(" AND p.IdCli={0}", idCliente);
                    whereDadosVendidos += string.Format(" AND ped.IdCli={0}", idCliente);

                    criterio += string.Format(formatoCriterio, "Cliente:", ClienteDAO.Instance.GetNome(idCliente.StrParaUint()));
                }
            }
            else if (!string.IsNullOrWhiteSpace(nomeCliente))
            {
                var ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, null, null, null, null, 0);

                filtroAdicional += string.Format(" AND p.IdCli IN ({0})", ids);
                whereDadosVendidos += string.Format(" AND ped.IdCli IN ({0})", ids);

                criterio += string.Format(formatoCriterio, "Cliente:", nomeCliente);
            }

            if (tipoFiscal > 0)
            {
                filtroAdicional += string.Format(" AND c.TipoFiscal={0}", tipoFiscal);
                whereDadosVendidos += string.Format(" AND cli.TipoFiscal={0}", tipoFiscal);

                switch (tipoFiscal)
                {
                    case (int)TipoFiscalCliente.ConsumidorFinal: criterio += string.Format(formatoCriterio, "Tipo Fiscal:", "Consumidor Final"); break;
                    case (int)TipoFiscalCliente.Revenda: criterio += string.Format(formatoCriterio, "Tipo Fiscal:", "Revenda"); break;
                }
            }

            if (!string.IsNullOrWhiteSpace(loja) && loja.ToLower() != "todas" && loja != "0")
            {
                filtroAdicional += string.Format(" AND p.IdLoja={0}", loja);
                whereDadosVendidos += string.Format(" AND ped.IdLoja={0}", loja);
                criterio += string.Format(formatoCriterio, "Loja:", LojaDAO.Instance.GetNome(loja.StrParaUint()));
            }

            if (!string.IsNullOrWhiteSpace(situacao))
            {
                filtroAdicional += string.Format(" AND p.Situacao IN ({0})", situacao);
                whereDadosVendidos += string.Format(" AND ped.Situacao IN ({0})", situacao);

                criterio += string.Format(formatoCriterio, "Situação:", GetSituacaoPedido(situacao));
            }

            if (desconto == 1 )
            {
                filtroAdicional += " AND (p.Desconto >1 OR p.IdPedido IN (SELECT DISTINCT IdPedido FROM ambiente_pedido WHERE Desconto >1))";
                whereDadosVendidos += " AND (ped.Desconto OR ped.IdPedido IN (SELECT DISTINCT IdPedido FROM ambiente_pedido WHERE Desconto > 1))";

                criterio += string.Format(formatoCriterio, desconto == 1 ? "Pedidos com desconto" : "Pedidos sem desconto", string.Empty);
            }

            if(desconto == 2)
            {
                filtroAdicional += " AND (p.Desconto = 0 AND p.IdPedido NOT IN (SELECT DISTINCT IdPedido FROM ambiente_pedido WHERE Desconto >0)" +
                                   " AND(p.IdPedido in (SELECT DISTINCT IdPedido FROM produtos_pedido WHERE VALORDESCONTOQTDE = 0)))";
                whereDadosVendidos += " AND (ped.Desconto = 0 AND ped.IdPedido NOT IN (SELECT DISTINCT IdPedido FROM ambiente_pedido WHERE Desconto >0)"+
                                      " AND(p.IdPedido in (SELECT DISTINCT IdPedido FROM produtos_pedido WHERE VALORDESCONTOQTDE = 0)))";

                criterio += string.Format(formatoCriterio, desconto == 1 ? "Pedidos com desconto" : "Pedidos sem desconto", string.Empty);
            }               
            
            if (idMedidor > 0)
            {
                filtroAdicional += string.Format(" AND p.IdMedidor={0}", idMedidor.ToString());

                criterio += string.Format(formatoCriterio, "Medidor:", FuncionarioDAO.Instance.GetNome(idMedidor.ToString().StrParaUint()));
            }

            #region Filtro por data de situação

            /* Chamado 56824.
             * Passa o alias do pedido como alias da liberação, pois, a tabela liberarpedido foi removida do SQL e as liberações são buscadas pelo campo IdLiberarPedido,
             * o pedido possui essa propriedade. */
            var filtro = FiltroDataSituacao(dataInicioSituacao, dataFimSituacao, situacao, "?dtIniSit", "?dtFimSit", "p", "lp", " Sit.", calcularLiberados);
            sql += filtro.Sql;
            criterio += filtro.Criterio;
            whereDadosVendidos += FiltroDataSituacao(dataInicioSituacao, dataFimSituacao, situacao, "?dtIniSit", "?dtFimSit", "ped", "lp1", " Sit.", true).Sql;
            temFiltro = temFiltro || filtro.Sql != "";

            #endregion

            #region Filtro por data do pedido e de entrega

            if (!string.IsNullOrWhiteSpace(dataInicioPedido))
            {
                filtroAdicional += " AND p.DataPedido>=?dtIni";
                whereDadosVendidos += " AND ped.DataPedido>=?dtIni";

                criterio += string.Format(formatoCriterio, "Data Início:", dataInicioPedido);
            }

            if (!string.IsNullOrWhiteSpace(dataFimPedido))
            {
                filtroAdicional += " AND p.DataPedido<=?dtFim";
                whereDadosVendidos += " AND ped.DataPedido<=?dtFim";

                criterio += string.Format(formatoCriterio, "Data Fim:", dataFimPedido);
            }

            if (!string.IsNullOrWhiteSpace(dataInicioEntrega))
            {
                filtroAdicional += " AND p.DataEntrega>=?dtIniEnt";
                whereDadosVendidos += " AND ped.DataEntrega>=?dtIniEnt";

                criterio += string.Format(formatoCriterio, "Data Início Entrega:", dataInicioEntrega);
            }

            if (!string.IsNullOrWhiteSpace(dataFimEntrega))
            {
                filtroAdicional += " AND p.DataEntrega<=?dtFimEnt";
                whereDadosVendidos += " AND ped.DataEntrega<=?dtFimEnt";
                criterio += string.Format(formatoCriterio, "Data Fim Entrega:", dataFimEntrega);
            }

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

            if (!string.IsNullOrWhiteSpace(tipoCliente))
            {
                sql += string.Format(" AND c.IdTipoCliente IN ({0})", tipoCliente);
                whereDadosVendidos += string.Format(" AND cli.IdTipoCliente IN ({0})", tipoCliente);

                criterio += string.Format(formatoCriterio, "Tipo de Cliente:", TipoClienteDAO.Instance.GetNomes(tipoCliente));
                temFiltro = true;
            }

            if (tipoEntrega > 0)
            {
                filtroAdicional += string.Format(" AND p.TipoEntrega={0}", tipoEntrega);
                whereDadosVendidos += string.Format(" AND ped.TipoEntrega={0}", tipoEntrega);

                criterio += string.Format(formatoCriterio, "Tipo Entrega:", Utils.GetDescrTipoEntrega(tipoEntrega));
            }

            if (!string.IsNullOrWhiteSpace(tiposPedido))
            {
                filtroAdicional += string.Format(" AND p.TipoPedido IN ({0})", tiposPedido);
                whereDadosVendidos += string.Format(" AND ped.TipoPedido IN ({0})", tiposPedido);
                var criterioTipoPedido = new List<string>();

                foreach (var tipoPedido in tiposPedido.Split(','))
                    switch (tipoPedido)
                    {
                        case "1": criterioTipoPedido.Add("Venda"); break;
                        case "2": criterioTipoPedido.Add("Revenda"); break;
                        case "3": criterioTipoPedido.Add("Mão de obra"); break;
                        case "4": criterioTipoPedido.Add("Produção"); break;
                    }

                criterio += string.Format(formatoCriterio, "Tipo de pedido:", string.Join(", ", criterioTipoPedido));
            }

            if (fastDelivery > 0)
            {
                filtroAdicional += string.Format(" AND (FastDelivery{0})", fastDelivery == 1 ? "=1" : "=0 OR FastDelivery IS NULL");
                whereDadosVendidos += string.Format(" AND (FastDelivery{0})", fastDelivery == 1 ? "=1" : "=0 OR FastDelivery IS NULL");

                criterio += string.Format(formatoCriterio, "Fast Delivery:", (fastDelivery == 1 ? "Sim" : "Não"));
            }

            if (!string.IsNullOrWhiteSpace(situacaoProducao))
            {
                filtroAdicional += string.Format(" AND p.SituacaoProducao IN ({0}) ", situacaoProducao);
                whereDadosVendidos += string.Format(" AND p.SituacaoProducao IN ({0}) ", situacaoProducao);

                criterio += string.Format(formatoCriterio, "Situação Prod.:", GetSituacaoProdPedido(situacaoProducao));
            }

            if (!string.IsNullOrWhiteSpace(tiposVenda))
            {
                filtroAdicional += string.Format(" AND p.TipoVenda IN ({0})", tiposVenda);
                whereDadosVendidos += string.Format(" AND ped.TipoVenda IN ({0})", tiposVenda);
                var criterioTipoVenda = new List<string>();
                var tiposVendaCadastrados = DataSources.Instance.GetTipoVenda(false, true, administrador, emitirGarantiaReposicao, emitirPedidoFuncionario);

                foreach (var tipoVenda in tiposVenda.Split(','))
                    criterioTipoVenda.Add(tiposVendaCadastrados.FirstOrDefault(f => f.Id.ToString() == tipoVenda).Descr);

                criterio += string.Format(formatoCriterio, "Tipo de venda:", string.Join(", ", criterioTipoVenda));
            }

            if (!string.IsNullOrWhiteSpace(idsGrupo) && idsGrupo != "0")
            {
                var criterioGrupo = new List<string>();

                filtroAdicional += string.Format(@" AND p.IdPedido IN
                    (SELECT DISTINCT pp1.IdPedido FROM produtos_pedido pp1
                        INNER JOIN produto prod ON (pp1.IdProd=prod.IdProd)
                    WHERE prod.IdGrupoProd IN ({0}) AND {1})", idsGrupo, filtroFluxoProdutoPedido);
                whereDadosVendidos += string.Format(" AND p.IdGrupoProd IN ({0})", idsGrupo);

                foreach (var idGrupo in idsGrupo.Split(','))
                    criterioGrupo.Add(GrupoProdDAO.Instance.GetDescricao(idGrupo.StrParaInt()));

                criterio += string.Format(formatoCriterio, "Grupo(s):", string.Join(", ", criterioGrupo));
            }

            if (!string.IsNullOrWhiteSpace(idsSubgrupoProd) && idsSubgrupoProd != "0")
            {
                var criterioSubgrupo = new List<string>();

                filtroAdicional += string.Format(@" AND p.IdPedido IN
                    (SELECT DISTINCT pp1.IdPedido FROM produtos_pedido pp1
                        INNER JOIN produto prod ON (pp1.IdProd=prod.IdProd)
                    WHERE prod.IdSubgrupoProd IN ({0}) AND {1})", idsSubgrupoProd, filtroFluxoProdutoPedido);
                whereDadosVendidos += string.Format(" AND p.IdSubgrupoProd IN ({0})", idsSubgrupoProd);

                foreach (var id in idsSubgrupoProd.Split(','))
                    criterioSubgrupo.Add(SubgrupoProdDAO.Instance.GetDescricao(id.StrParaInt()));

                criterio += string.Format(formatoCriterio, "Subgrupo(s):", string.Join(", ", criterioSubgrupo));
            }

            if (pedidosSemAnexos)
            {
                filtroAdicional += " AND p.IdPedido NOT IN (SELECT DISTINCT IdPedido FROM fotos_pedido)";
                whereDadosVendidos += " AND ped.IdPedido NOT IN (SELECT DISTINCT IdPedido from fotos_pedido)";

                criterio += string.Format(formatoCriterio, "Pedidos sem anexos", string.Empty);
            }

            if (!string.IsNullOrWhiteSpace(idsBenef))
            {
                var redondo = BenefConfigDAO.Instance.TemBenefRedondo(idsBenef) ? " OR Redondo=1" : string.Empty;

                filtroAdicional += string.Format(@" AND p.IdPedido IN
                    (SELECT DISTINCT pp1.IdPedido FROM produtos_pedido pp1 WHERE {2} AND pp1.IdProdPed IN
                        (SELECT DISTINCT ppb1.IdProdPed FROM produto_pedido_benef ppb1 WHERE ppb1.idBenefConfig IN ({0})) {1})", idsBenef, redondo, filtroFluxoProdutoPedido);
                whereDadosVendidos += string.Format(" AND (pp.IdProdPed IN (SELECT DISTINCT IdProdPed FROM produto_pedido_benef WHERE IdBenefConfig IN ({0})) {1})", idsBenef, redondo);

                criterio += string.Format(formatoCriterio, "Beneficiamentos:", BenefConfigDAO.Instance.GetDescrBenef(idsBenef));
            }

            if (!string.IsNullOrWhiteSpace(dataInicioPronto))
            {
                filtroAdicional += " AND p.DataPronto>=?dataIniPronto";
                criterio += string.Format(formatoCriterio, "Data Pronto Início:", dataInicioPronto);
            }

            if (!string.IsNullOrWhiteSpace(dataFimPronto))
            {
                filtroAdicional += " AND p.DataPronto<=?dataFimPronto";
                criterio += string.Format(formatoCriterio, "Data Pronto Fim:", dataFimPronto);
            }

            if (numeroDiasDiferencaProntoLib > 0)
            {
                sql += string.Format(" AND IdLiberarPedido IN (SELECT lp1.IdLiberarPedido FROM liberarpedido lp1 WHERE DATEDIFF(lp1.DataLiberacao, p.DataPronto)>={0})", numeroDiasDiferencaProntoLib);
                criterio += string.Format(formatoCriterio, "Diferença dias entre Pedido Pronto e Liberado:", numeroDiasDiferencaProntoLib);
                temFiltro = true;
            }

            #region Data de instalação do pedido

            if (!string.IsNullOrWhiteSpace(dataInicioInstalacao) || !string.IsNullOrWhiteSpace(dataFimInstalacao))
            {
                filtroAdicional += string.Format(" AND p.IdPedido IN (SELECT DISTINCT IdPedido FROM instalacao WHERE {0} {1} {2})",
                    !string.IsNullOrWhiteSpace(dataInicioInstalacao) ? "DataFinal>=?dataIniInst" : string.Empty,
                    !string.IsNullOrWhiteSpace(dataInicioInstalacao) && !string.IsNullOrWhiteSpace(dataFimInstalacao) ? "AND" : string.Empty,
                    !string.IsNullOrWhiteSpace(dataFimInstalacao) ? "DataFinal<=?dataFimInst" : string.Empty);

                criterio += string.Format("{0}{1}",
                    !string.IsNullOrWhiteSpace(dataInicioInstalacao) ? string.Format(formatoCriterio, "Data Instalação Início:", dataInicioInstalacao) : string.Empty,
                    !string.IsNullOrWhiteSpace(dataFimInstalacao) ? string.Format(formatoCriterio, "Data Instalação Fim:", dataFimInstalacao) : string.Empty);
            }

            #endregion

            if (altura > 0 || largura > 0)
            {
                filtroAdicional += string.Format(@" AND p.IdPedido IN
                    (SELECT DISTINCT pp1.IdPedido FROM produtos_pedido pp1 WHERE ({0} OR {1}) AND {2})",
                    altura > 0 ? string.Format("pp1.Altura={0}", altura.ToString().Replace(",", ".")) : "0",
                    largura > 0 ? string.Format("pp1.Largura={0}", largura.ToString().Replace(",", ".")) : "0", filtroFluxoProdutoPedido);

                criterio += string.Format("{0}{1}",
                    altura > 0 ? string.Format(formatoCriterio, "Altura produto:", altura) : string.Empty,
                    largura > 0 ? string.Format(formatoCriterio, "Largura produto:", largura) : string.Empty);
            }

            if (!string.IsNullOrWhiteSpace(codigoProduto))
            {
                var idProd = ProdutoDAO.Instance.ObtemIdProd(codigoProduto);

                filtroAdicional += string.Format(@" AND p.IdPedido IN 
                    (SELECT DISTINCT IdPedido FROM produtos_pedido pp1 WHERE pp1.IdProd={0} AND {1})", idProd, filtroFluxoProdutoPedido);

                criterio += string.Format(formatoCriterio, "Produto:", string.Format("{0} - {1}", codigoProduto, ProdutoDAO.Instance.GetDescrProduto(codigoProduto)));
            }
            else if (!string.IsNullOrWhiteSpace(descricaoProduto))
            {
                filtroAdicional += string.Format(@" AND p.IdPedido IN 
                    (SELECT DISTINCT pp1.IdPedido FROM produtos_pedido pp1
                        INNER JOIN produto p ON (pp1.IdProd=p.IdProd) 
                    WHERE p.Descricao LIKE ?descrProd AND {0})", filtroFluxoProdutoPedido);

                criterio += string.Format(formatoCriterio, "Produto:", descricaoProduto);
            }

            if (origemPedido == 1)
            {
                filtroAdicional += " AND (p.Importado OR p.GeradoParceiro)";
                criterio += string.Format(formatoCriterio, "Origem Pedido:", "E-Commerce");
            }
            else if (origemPedido == 2)
            {
                filtroAdicional += " AND (p.Importado = 0 AND p.GeradoParceiro = 0)";
                criterio += string.Format(formatoCriterio, "Origem Pedido:", "Normal");
            }

            if (comSemNF == "1") // Com NF gerada
            {
                filtroAdicional += " AND p.IdPedido IN (SELECT DISTINCT pnf1.IdPedido FROM pedidos_nota_fiscal pnf1 WHERE pnf1.IdPedido IS NOT NULL)";
                criterio += string.Format(formatoCriterio, "Pedidos com nota fiscal gerada", string.Empty);
            }
            else if (comSemNF == "2") // Sem NF gerada
            {
                filtroAdicional += " AND p.IdPedido NOT IN (SELECT DISTINCT pnf1.IdPedido FROM pedidos_nota_fiscal pnf1 WHERE pnf1.IdPedido IS NOT NULL)";
                criterio += string.Format(formatoCriterio, "Pedidos sem nota fiscal gerada", string.Empty);
            }

            if (idOC > 0)
            {
                filtroAdicional += string.Format(" AND EXISTS (SELECT IdPedido FROM pedido_ordem_carga WHERE IdPedido=p.IdPedido AND IdOrdemCarga={0})", idOC);
                criterio += string.Format(formatoCriterio, "Ordem de Carga:", idOC);
                temFiltro = true;
            }

            if (usuarioCadastro > 0)
            {
                filtroAdicional += " AND p.UsuCad = " + usuarioCadastro;
                criterio += string.Format(formatoCriterio, "Usuário Cadastro: ", FuncionarioDAO.Instance.GetNome((uint)usuarioCadastro));
            }

            sql += " GROUP BY p.IdPedido";

            switch (ordenacao)
            {
                case 1:
                    sql += " ORDER BY p.DataEntrega ASC";
                    break;
                case 2:
                    sql += " ORDER BY p.DataEntrega DESC";
                    break;
                default:
                    sql += " ORDER BY p.DataPedido DESC, p.DataCad DESC";
                    break;
            }

            sql = string.Format(sql, whereDadosVendidos);

            return sql.Replace("$$$", criterio);
        }

        #endregion

        #region Listagem

        /// <summary>
        /// Retorna uma lista de pedidos, com base nos parâmetros, para a listagem da tela de vendas de pedidos.
        /// </summary>
        public Pedido[] PesquisarListaVendasPedidos(float? altura, int? cidade, string codCliente, string codigoProduto, string comSemNF, string dataFimEntrega, string dataFimInstalacao,
            string dataFimPedido, string dataFimPronto, string dataFimSituacao, string dataInicioEntrega, string dataInicioInstalacao, string dataInicioPedido, string dataInicioPronto,
            string dataInicioSituacao, int? desconto, string descricaoProduto, bool exibirProdutos, int? fastDelivery, int? idCarregamento, string idCliente, int? idFunc, int? idMedidor, int? idOrcamento, int? idOC,
            int? idPedido, string idsBenef, string idsGrupo, string idsRota, string idsSubgrupoProd, int? idVendAssoc, int? largura, string loja, string nomeCliente,
            int? numeroDiasDiferencaProntoLib, string observacao, int? ordenacao, int? origemPedido, bool pedidosSemAnexos, string situacao, string situacaoProducao, string tiposPedido, string tipoCliente,
            int? tipoEntrega, int? tipoFiscal, string tiposVenda, bool trazerPedCliVinculado, int? usuarioCadastro, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlVendasPedidos(altura, cidade, codCliente, codigoProduto, comSemNF, false, dataFimEntrega, dataFimInstalacao, dataFimPedido, dataFimPronto, dataFimSituacao,
                dataInicioEntrega, dataInicioInstalacao, dataInicioPedido, dataInicioPronto, dataInicioSituacao, desconto, descricaoProduto, exibirProdutos, false, fastDelivery, out filtroAdicional,  idCarregamento,
                idCliente, idFunc, idMedidor, idOC, idOrcamento, idPedido, idsBenef, idsGrupo, null, idsRota, idsSubgrupoProd, idVendAssoc, largura, UserInfo.GetUserInfo, loja, nomeCliente,
                numeroDiasDiferencaProntoLib, observacao, ordenacao, origemPedido, false, pedidosSemAnexos, situacao, situacaoProducao, out temFiltro, tiposPedido, tipoCliente, tipoEntrega,
                tipoFiscal, tiposVenda, false, trazerPedCliVinculado, usuarioCadastro).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : string.Empty);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional, ObterParametrosFiltrosVendasPedidos(codCliente, codigoProduto, dataFimEntrega,
                dataFimPedido, dataFimInstalacao, dataFimPronto, dataFimSituacao, dataInicioEntrega, dataInicioInstalacao, dataInicioPedido, dataInicioPronto, dataInicioSituacao, descricaoProduto,
                nomeCliente, observacao)).ToArray();
        }

        /// <summary>
        /// Retorna a quantidade de pedidos, com base nos parâmetros, para a paginação da listagem da tela de vendas de pedidos.
        /// </summary>
        public int PesquisarListaVendasPedidosCount(float? altura, int? cidade, string codCliente, string codigoProduto, string comSemNF, string dataFimEntrega, string dataFimInstalacao,
            string dataFimPedido, string dataFimPronto, string dataFimSituacao, string dataInicioEntrega, string dataInicioInstalacao, string dataInicioPedido, string dataInicioPronto,
            string dataInicioSituacao, int? desconto, string descricaoProduto, bool exibirProdutos, int? fastDelivery, int? idCarregamento, string idCliente, int? idFunc, int? idMedidor, int? idOrcamento, int? idOC,
            int? idPedido, string idsBenef, string idsGrupo, string idsRota, string idsSubgrupoProd, int? idVendAssoc, int? largura, string loja, string nomeCliente, 
            int? numeroDiasDiferencaProntoLib, string observacao, int? ordenacao, int? origemPedido, bool pedidosSemAnexos, string situacao, string situacaoProducao, string tiposPedido, string tipoCliente,
            int? tipoEntrega, int? tipoFiscal, string tiposVenda, bool trazerPedCliVinculado, int? usuarioCadastro)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlVendasPedidos(altura, cidade, codCliente, codigoProduto, comSemNF, true, dataFimEntrega, dataFimInstalacao, dataFimPedido, dataFimPronto, dataFimSituacao, dataInicioEntrega,
                dataInicioInstalacao, dataInicioPedido, dataInicioPronto, dataInicioSituacao, desconto, descricaoProduto, exibirProdutos, false, fastDelivery, out filtroAdicional, idCarregamento, idCliente,
                idFunc, idMedidor, idOC, idOrcamento, idPedido, idsBenef, idsGrupo, null, idsRota, idsSubgrupoProd, idVendAssoc, largura, UserInfo.GetUserInfo, loja, nomeCliente,
                 numeroDiasDiferencaProntoLib,observacao, ordenacao, origemPedido, false, pedidosSemAnexos, situacao, situacaoProducao, out temFiltro, tiposPedido, tipoCliente, tipoEntrega,
                tipoFiscal, tiposVenda, false, trazerPedCliVinculado, usuarioCadastro).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : string.Empty);

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, ObterParametrosFiltrosVendasPedidos(codCliente, codigoProduto, dataFimEntrega, dataFimPedido, dataFimInstalacao,
                dataFimPronto, dataFimSituacao, dataInicioEntrega, dataInicioInstalacao, dataInicioPedido, dataInicioPronto, dataInicioSituacao, descricaoProduto, nomeCliente, observacao));
        }

        #endregion

        #region Relatório

        /// <summary>
        /// Retorna uma lista de pedidos, com base nos parâmetros, para os relatórios da listagem da tela de vendas de pedidos.
        /// </summary>
        public Pedido[] PesquisarRelatorioVendasPedidos(float? altura, int? cidade, string codCliente, string codigoProduto, string comSemNF, string dataFimEntrega, string dataFimInstalacao,
            string dataFimPedido, string dataFimPronto, string dataFimSituacao, string dataInicioEntrega, string dataInicioInstalacao, string dataInicioPedido, string dataInicioPronto,
            string dataInicioSituacao, int? desconto, string descricaoProduto, bool exibirProdutos, bool exibirRota, int? fastDelivery, int? idCarregamento, string idCliente, int? idFunc, int? idMedidor, int? idOC,
            int? idOrcamento, int? idPedido, string idsBenef, string idsGrupo, string idsPedidos, string idsRota, string idsSubgrupoProd, int? idVendAssoc, int? largura, LoginUsuario login,
            string loja, string nomeCliente, int? numeroDiasDiferencaProntoLib, string observacao, int? ordenacao, int? origemPedido, bool pedidosSemAnexos, string situacao,
            string situacaoProducao, string tiposPedido, string tipoCliente, int? tipoEntrega, int? tipoFiscal, string tiposVenda, bool trazerPedCliVinculado, int? usuarioCadastro)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlVendasPedidos(altura, cidade, codCliente, codigoProduto, comSemNF, false, dataFimEntrega, dataFimInstalacao, dataFimPedido, dataFimPronto, dataFimSituacao,
                dataInicioEntrega, dataInicioInstalacao, dataInicioPedido, dataInicioPronto, dataInicioSituacao, desconto, descricaoProduto, exibirProdutos, exibirRota, fastDelivery,
                out filtroAdicional, idCarregamento, idCliente, idFunc, idMedidor, idOC, idOrcamento, idPedido, idsBenef, idsGrupo, idsPedidos, idsRota, idsSubgrupoProd, idVendAssoc, largura, login, loja,
                nomeCliente, numeroDiasDiferencaProntoLib, observacao, ordenacao, origemPedido, true, pedidosSemAnexos, situacao, situacaoProducao, out temFiltro, tiposPedido, tipoCliente,
                tipoEntrega, tipoFiscal, tiposVenda, false, trazerPedCliVinculado, usuarioCadastro).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadData(sql, ObterParametrosFiltrosVendasPedidos(codCliente, codigoProduto, dataFimEntrega, dataFimPedido, dataFimInstalacao, dataFimPronto, dataFimSituacao,
                dataInicioEntrega, dataInicioInstalacao, dataInicioPedido, dataInicioPronto, dataInicioSituacao, descricaoProduto, nomeCliente, observacao)).ToArray();
        }

        #endregion

        #region Rodapé

        /// <summary>
        /// Chamado 49487.
        /// Retorna os totais dos pedidos para exibí-los no rodapé da listagem de vendas pedidos.
        /// </summary>
        public TotalListaPedidos ObterTotaisListaPedidos(float? altura, int? cidade, string codCliente, string codigoProduto, string comSemNF, string dataFimEntrega, string dataFimInstalacao,
            string dataFimPedido, string dataFimPronto, string dataFimSituacao, string dataInicioEntrega, string dataInicioInstalacao, string dataInicioPedido, string dataInicioPronto,
            string dataInicioSituacao, int? desconto, string descricaoProduto, bool exibirProdutos, int? fastDelivery, int? idCarregamento, string idCliente, int? idFunc, int? idMedidor, int? idOrcamento, int? idOC,
            int? idPedido, string idsBenef, string idsGrupo, string idsRota, string idsSubgrupoProd, int? idVendAssoc, int? largura, string loja, string nomeCliente,
            int? numeroDiasDiferencaProntoLib, string observacao, int? ordenacao, int? origemPedido, bool pedidosSemAnexos, string situacao, string situacaoProducao, string tiposPedido, string tipoCliente,
            int? tipoEntrega, int? tipoFiscal, string tiposVenda, bool trazerPedCliVinculado, int? usuarioCadastro)
        {
            bool temFiltro;
            string filtroAdicional;
 
            if (altura.GetValueOrDefault() == 0 && cidade.GetValueOrDefault() == 0 && string.IsNullOrEmpty(codCliente) && string.IsNullOrEmpty(codigoProduto) &&
                string.IsNullOrEmpty(dataFimEntrega) && string.IsNullOrEmpty(dataFimInstalacao) && string.IsNullOrEmpty(dataFimPedido) && string.IsNullOrEmpty(dataFimPronto) &&
                string.IsNullOrEmpty(dataFimSituacao) && string.IsNullOrEmpty(dataInicioEntrega) && string.IsNullOrEmpty(dataInicioInstalacao) && string.IsNullOrEmpty(dataInicioPedido) &&
                string.IsNullOrEmpty(dataInicioPronto) && string.IsNullOrEmpty(dataInicioSituacao) && desconto.GetValueOrDefault() == 0 && string.IsNullOrEmpty(descricaoProduto) &&
                fastDelivery.GetValueOrDefault() == 0 && idCarregamento.GetValueOrDefault() == 0 && string.IsNullOrEmpty(idCliente) && idFunc.GetValueOrDefault() == 0 && idMedidor.GetValueOrDefault() == 0 &&
                idOrcamento.GetValueOrDefault() == 0 && idOC.GetValueOrDefault() == 0 && idPedido.GetValueOrDefault() == 0 && string.IsNullOrEmpty(idsBenef) && string.IsNullOrEmpty(idsGrupo) &&
                string.IsNullOrEmpty(idsRota) && string.IsNullOrEmpty(idsSubgrupoProd) && idVendAssoc.GetValueOrDefault() == 0 && largura.GetValueOrDefault() == 0 && string.IsNullOrEmpty(loja) &&
                string.IsNullOrEmpty(nomeCliente) && numeroDiasDiferencaProntoLib.GetValueOrDefault() == 0 && origemPedido.GetValueOrDefault() == 0 && string.IsNullOrEmpty(situacao) &&
                string.IsNullOrEmpty(situacaoProducao) && string.IsNullOrEmpty(tipoCliente) && tipoEntrega.GetValueOrDefault() == 0 && tipoFiscal.GetValueOrDefault() == 0 && usuarioCadastro.GetValueOrDefault() == 0)
                return new TotalListaPedidos();

            // Passa o parâmetro totaisListaPedidos como true para recuperar somente os campos que serão somados.
            var sql = SqlVendasPedidos(altura, cidade, codCliente, codigoProduto, comSemNF, false, dataFimEntrega, dataFimInstalacao, dataFimPedido, dataFimPronto, dataFimSituacao,
                dataInicioEntrega, dataInicioInstalacao, dataInicioPedido, dataInicioPronto, dataInicioSituacao, desconto, descricaoProduto, exibirProdutos, false, fastDelivery, out filtroAdicional, idCarregamento,
                idCliente, idFunc, idMedidor, idOC, idOrcamento, idPedido, idsBenef, idsGrupo, null, idsRota, idsSubgrupoProd, idVendAssoc, largura, UserInfo.GetUserInfo, loja, nomeCliente,
                 numeroDiasDiferencaProntoLib, observacao, ordenacao, origemPedido, false, pedidosSemAnexos, situacao, situacaoProducao, out temFiltro, tiposPedido, tipoCliente, tipoEntrega,
                tipoFiscal, tiposVenda, true, trazerPedCliVinculado, usuarioCadastro).Replace("?filtroAdicional?", filtroAdicional);

            // Soma somente os campos que serão exibidos na tela.
            // TotalReal: total do pedido comercial, mesmo exibido na listagem e relatório de vendas pedidos.
            sql = string.Format("SELECT CAST(CONCAT(SUM(TotalReal), ';', SUM(TotM), ';', SUM(Peso)) AS CHAR) FROM ({0}) AS Temp", sql);

            return new TotalListaPedidos(ExecuteScalar<string>(sql, ObterParametrosFiltrosVendasPedidos(codCliente, codigoProduto, dataFimEntrega, dataFimPedido, dataFimInstalacao, dataFimPronto,
                dataFimSituacao, dataInicioEntrega, dataInicioInstalacao, dataInicioPedido, dataInicioPronto, dataInicioSituacao, descricaoProduto, nomeCliente, observacao)));
        }

        #endregion

        #region Parâmetros

        /// <summary>
        /// Retorna os parâmetros que devem ser substituídos no SQL, com base nos filtros informados.
        /// </summary>
        private GDAParameter[] ObterParametrosFiltrosVendasPedidos(string codCliente, string codigoProduto, string dataFimEntrega, string dataFimPedido, string dataFimInstalacao,
            string dataFimPronto, string dataFimSituacao, string dataInicioEntrega, string dataInicioInstalacao, string dataInicioPedido, string dataInicioPronto, string dataInicioSituacao,
            string descricaoProduto, string nomeCliente, string observacao)
        {
            var parametros = new List<GDAParameter>();

            if (!string.IsNullOrWhiteSpace(codCliente))
                parametros.Add(new GDAParameter("?codCliente", "%" + codCliente + "%"));

            if (string.IsNullOrWhiteSpace(codigoProduto) && !string.IsNullOrEmpty(descricaoProduto))
                parametros.Add(new GDAParameter("?descrProd", "%" + descricaoProduto + "%"));

            if (!string.IsNullOrWhiteSpace(dataFimEntrega))
                parametros.Add(new GDAParameter("?dtFimEnt", DateTime.Parse(dataFimEntrega + " 23:59")));

            if (!string.IsNullOrWhiteSpace(dataFimPedido))
                parametros.Add(new GDAParameter("?dtFim", DateTime.Parse(dataFimPedido + " 23:59")));

            if (!string.IsNullOrWhiteSpace(dataFimInstalacao))
                parametros.Add(new GDAParameter("?dataFimInst", DateTime.Parse(dataFimInstalacao + " 23:59")));

            if (!string.IsNullOrWhiteSpace(dataFimPronto))
                parametros.Add(new GDAParameter("?dataFimPronto", DateTime.Parse(dataFimPronto + " 23:59")));

            if (!string.IsNullOrWhiteSpace(dataFimSituacao))
                parametros.Add(new GDAParameter("?dtFimSit", DateTime.Parse(dataFimSituacao + " 23:59")));

            if (!string.IsNullOrWhiteSpace(dataInicioEntrega))
                parametros.Add(new GDAParameter("?dtIniEnt", DateTime.Parse(dataInicioEntrega + " 00:00")));

            if (!string.IsNullOrWhiteSpace(dataInicioInstalacao))
                parametros.Add(new GDAParameter("?dataIniInst", DateTime.Parse(dataInicioInstalacao + " 00:00")));

            if (!string.IsNullOrWhiteSpace(dataInicioPedido))
                parametros.Add(new GDAParameter("?dtIni", DateTime.Parse(dataInicioPedido + " 00:00")));

            if (!string.IsNullOrWhiteSpace(dataInicioPronto))
                parametros.Add(new GDAParameter("?dataIniPronto", DateTime.Parse(dataInicioPronto + " 00:00")));

            if (!string.IsNullOrWhiteSpace(dataInicioSituacao))
                parametros.Add(new GDAParameter("?dtIniSit", DateTime.Parse(dataInicioSituacao + " 00:00")));

            if (!string.IsNullOrWhiteSpace(nomeCliente))
                parametros.Add(new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"));          

            if (!string.IsNullOrWhiteSpace(observacao))
                parametros.Add(new GDAParameter("?observacao", "%" + observacao + "%"));

            return parametros.Count > 0 ? parametros.ToArray() : null;
        }

        #endregion

        #endregion

        #region Listagem/Relatório de vendas de pedidos simples

        #region SQL

        /// <summary>
        /// Retorna o SQL da tela de vendas de pedidos simples, aplicando os filtros de acordo com os parâmetros informados.
        /// </summary>
        private string SqlVendasPedidosSimples(string dataFimEntrega, string dataInicioEntrega, out string filtroAdicional, string idsRota, out bool temFiltro)
        {
            temFiltro = false;
            filtroAdicional = string.Empty;
            var criterio = string.Empty;
            var formatoCriterio = "{0} {1}    ";

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

            var campos = string.Format(@"{0}, p.IdPedido, p.IdLoja, p.IdFunc, p.IdCli, p.IdFormaPagto, p.IdOrcamento, p.PrazoEntrega, p.TipoEntrega, p.TipoVenda, p.DataEntrega, p.ValorEntrega,
                p.Situacao, p.ValorEntrada, p.DataCad, p.UsuCad, p.NumParc, p.Obs, p.DataConf, p.UsuConf, p.DataCanc, p.UsuCanc, p.EnderecoObra, p.BairroObra, p.CidadeObra, p.LocalObra,
                p.IdFormaPagto2, p.IdTipoCartao, p.IdTipoCartao2, p.CodCliente, p.NumAutConstrucard, p.IdComissionado, p.PercComissao, p.ValorComissao, p.IdPedidoAnterior, p.FastDelivery,
                p.DataPedido, p.IdObra, p.IdMedidor, p.TaxaPrazo, p.TipoPedido, p.TaxaFastDelivery, p.TemperaFora, p.SituacaoProducao, p.IdFuncVenda, p.DataEntregaOriginal, p.GeradoParceiro,
                {1} AS NomeCliente, f.Nome AS NomeFunc, l.NomeFantasia AS NomeLoja, l.Telefone AS TelefoneLoja, fp.Descricao AS FormaPagto, cid.NomeCidade AS Cidade, p.ValorCreditoAoConfirmar,
                p.CreditoGeradoConfirmar, p.CreditoUtilizadoConfirmar, c.IdFunc AS IdFuncCliente, fc.Nome AS NomeFuncCliente, p.IdParcela, p.CepObra, p.IdSinal, p.idPagamentoAntecipado,
                p.ValorPagamentoAntecipado, p.DataPronto, p.ObsLiberacao, p.IdProjeto, p.IdLiberarPedido, p.IdFuncDesc, p.DataDesc, p.Importado, c.Email, p.PercentualComissao, p.RotaExterna,
                p.ClienteExterno, p.PedCliExterno, p.CelCliExterno, p.TotalPedidoExterno, p.DeveTransferir, p.DataFin, p.UsuFin, '$$$' AS Criterio",
                camposFluxo, ClienteDAO.Instance.GetNomeCliente("c"));

            var sql = string.Format(@"
                SELECT {0}
                FROM pedido p 
                    INNER JOIN cliente c ON (p.IdCli=c.Id_Cli)
                    LEFT JOIN pedido_espelho pe ON (p.IdPedido=pe.IdPedido)
                    LEFT JOIN funcionario fc ON (c.IdFunc=fc.IdFunc)
                    LEFT JOIN cidade cid ON (c.IdCidade=cid.IdCidade) 
                    LEFT JOIN funcionario f ON (p.IdFunc=f.IdFunc) 
                    LEFT JOIN loja l ON (p.IdLoja = l.IdLoja)
                    LEFT JOIN formapagto fp ON (fp.IdFormaPagto=p.IdFormaPagto)
                WHERE p.TipoPedido IN (1,2,3) AND p.TipoVenda IN (1,2,5) ?filtroAdicional?", campos);

            criterio += "Tipo de pedido: Venda, Revenda, Mão de Obra    ";
            criterio += string.Format("Tipo de venda: {0}    ",
                string.Join(", ", DataSources.Instance.GetTipoVenda(false, true).Where(f => f.Id == 1 || f.Id == 2 || f.Id == 5).Select(f => f.Descr)));    

            if (!string.IsNullOrWhiteSpace(idsRota))
            {
                filtroAdicional += string.Format(" AND p.IdCli IN (SELECT DISTINCT IdCliente FROM rota_cliente WHERE IdRota IN ({0}))", idsRota);

                criterio += string.Format(formatoCriterio, "Rota(s):", RotaDAO.Instance.ObtemCodRotas(idsRota));
            }

            if (!string.IsNullOrWhiteSpace(dataInicioEntrega))
            {
                filtroAdicional += " AND p.DataEntrega>=?dtIniEnt";

                criterio += string.Format(formatoCriterio, "Data Início Entrega:", dataInicioEntrega);
            }

            if (!string.IsNullOrWhiteSpace(dataFimEntrega))
            {
                filtroAdicional += " AND p.DataEntrega<=?dtFimEnt";

                criterio += string.Format(formatoCriterio, "Data Fim Entrega:", dataFimEntrega);
            }

            sql += " GROUP BY p.IdPedido";
            
            return sql.Replace("$$$", criterio);
        }

        #endregion

        #region Listagem

        /// <summary>
        /// Retorna uma lista de pedidos, com base nos parâmetros, para a listagem da tela de vendas de pedidos simples.
        /// </summary>
        public Pedido[] PesquisarListaVendasPedidosSimples(string dataFimEntrega, string dataInicioEntrega, string idsRota, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;

            sortExpression = string.IsNullOrWhiteSpace(sortExpression) ? "p.DataPedido DESC, p.DataCad DESC" : sortExpression;

            var sql = SqlVendasPedidosSimples(dataFimEntrega, dataInicioEntrega, out filtroAdicional, idsRota, out temFiltro).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : string.Empty);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional,
                ObterParametrosFiltrosVendasPedidosSimples(dataFimEntrega, dataInicioEntrega)).ToArray();
        }

        /// <summary>
        /// Retorna a quantidade de pedidos, com base nos parâmetros, para a paginação da listagem da tela de vendas de pedidos simples.
        /// </summary>
        public int PesquisarListaVendasPedidosSimplesCount(string dataFimEntrega, string dataInicioEntrega, string idsRota)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlVendasPedidosSimples(dataFimEntrega, dataInicioEntrega, out filtroAdicional, idsRota, out temFiltro).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : string.Empty);

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, ObterParametrosFiltrosVendasPedidosSimples(dataFimEntrega, dataInicioEntrega));
        }

        #endregion

        #region Relatório

        /// <summary>
        /// Retorna uma lista de pedidos, com base nos parâmetros, para os relatórios da listagem da tela de vendas de pedidos simples.
        /// </summary>
        public Pedido[] PesquisarRelatorioVendasPedidosSimples(string dataFimEntrega, string dataInicioEntrega, string idsRota)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlVendasPedidosSimples(dataFimEntrega, dataInicioEntrega, out filtroAdicional, idsRota, out temFiltro).Replace("?filtroAdicional?", filtroAdicional);

            sql += " ORDER BY DataPedido DESC, DataCad DESC";

            return objPersistence.LoadData(sql, ObterParametrosFiltrosVendasPedidosSimples(dataFimEntrega, dataInicioEntrega)).ToArray();
        }

        #endregion

        #region Parâmetros

        /// <summary>
        /// Retorna os parâmetros que devem ser substituídos no SQL, com base nos filtros informados.
        /// </summary>
        private GDAParameter[] ObterParametrosFiltrosVendasPedidosSimples(string dataFimEntrega, string dataInicioEntrega)
        {
            var parametros = new List<GDAParameter>();
            
            if (!string.IsNullOrWhiteSpace(dataFimEntrega))
                parametros.Add(new GDAParameter("?dtFimEnt", DateTime.Parse(dataFimEntrega + " 23:59")));
            
            if (!string.IsNullOrWhiteSpace(dataInicioEntrega))
                parametros.Add(new GDAParameter("?dtIniEnt", DateTime.Parse(dataInicioEntrega + " 00:00")));

            return parametros.Count > 0 ? parametros.ToArray() : null;
        }

        #endregion

        #endregion
 
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

            if(idRota > 0)
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

        #region Relatório de pedidos à vista (Tipo Pagto.)

        /// <summary>
        /// Retorna os totais
        /// </summary>
        public string[] TotaisAVista(string idLoja, string idVendedor, uint idFormaPagto, uint tipoCartao, string dtIni, string dtFim)
        {
            var sql = @"Select Round(Sum(p.Total), 2) From pedido p 
                Inner Join cliente c On (p.idCli=c.id_Cli) 
                Inner Join funcionario f On (p.IdFunc=f.IdFunc) 
                Inner Join loja l On (p.IdLoja = l.IdLoja) 
                Where tipoVenda=1 And p.situacao=" + (int)Pedido.SituacaoPedido.Confirmado;

            if (!string.IsNullOrEmpty(idLoja) && idLoja != "0")
                sql += " And p.IdLoja=" + idLoja;

            if (idVendedor != "0")
                sql += " And p.IdFunc=" + idVendedor;

            if (idFormaPagto > 0)
                sql += " And p.IdFormaPagto=" + idFormaPagto;

            if (tipoCartao > 0)
                sql += " And p.IdTipoCartao=" + tipoCartao;

            if (!string.IsNullOrEmpty(dtIni))
                sql += " And p.DataConf>=?dtIni";

            if (!string.IsNullOrEmpty(dtFim))
                sql += " And p.DataConf<=?dtFim";

            object totalDinheiro = objPersistence.ExecuteScalar(sql + " And p.idFormaPagto=" + (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro, GetParamAVista(dtIni, dtFim));
            object totalCheque = objPersistence.ExecuteScalar(sql + " And p.idFormaPagto=" + (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio, GetParamAVista(dtIni, dtFim));
            object totalCartao = objPersistence.ExecuteScalar(sql + " And p.idFormaPagto=" + (uint)Glass.Data.Model.Pagto.FormaPagto.Cartao, GetParamAVista(dtIni, dtFim));
            object totalConstrucard = objPersistence.ExecuteScalar(sql + " And p.idFormaPagto=" + (uint)Glass.Data.Model.Pagto.FormaPagto.Construcard, GetParamAVista(dtIni, dtFim));
            object totalPermuta = objPersistence.ExecuteScalar(sql + " And p.idFormaPagto=" + (uint)Glass.Data.Model.Pagto.FormaPagto.Permuta, GetParamAVista(dtIni, dtFim));
            object totalDeposito = objPersistence.ExecuteScalar(sql + " And p.idFormaPagto=" + (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito, GetParamAVista(dtIni, dtFim));

            var lstTotais = new List<string>
            {
                totalDinheiro != null && totalDinheiro.ToString() != string.Empty ? totalDinheiro.ToString() : "0",
                totalCheque != null && totalCheque.ToString() != string.Empty ? totalCheque.ToString() : "0",
                totalCartao != null && totalCartao.ToString() != string.Empty ? totalCartao.ToString() : "0",
                totalConstrucard != null && totalConstrucard.ToString() != string.Empty
                    ? totalConstrucard.ToString()
                    : "0",
                totalPermuta != null && totalPermuta.ToString() != string.Empty ? totalPermuta.ToString() : "0",
                totalDeposito != null && totalDeposito.ToString() != string.Empty ? totalDeposito.ToString() : "0"
            };

            return lstTotais.ToArray();
        }

        private string SqlAVista(string idLoja, string idVendedor, uint idFormaPagto, uint tipoCartao, string dtIni, string dtFim, bool selecionar,
            out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = false;
            filtroAdicional = " and p.tipoPedido<>" + (int)Pedido.TipoPedidoEnum.Producao + " And (tipoVenda=1 OR lp.TipoPagto=1) And p.situacao=" + (int)Pedido.SituacaoPedido.Confirmado;

            string criterio = string.Empty, where = string.Empty;

            if (!string.IsNullOrEmpty(idLoja) && idLoja != "0")
            {
                where += " And p.IdLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja.StrParaUint()) + "    ";
                temFiltro = true;
            }
            else
                criterio += "Loja: Todas    ";

            if (idVendedor != "0")
            {
                where += " And p.IdFunc=" + idVendedor;
                criterio += "Vendedor: " + BibliotecaTexto.GetTwoFirstNames(FuncionarioDAO.Instance.GetNome(idVendedor.StrParaUint())) + "    ";
                temFiltro = true;
            }
            else
                criterio += "Vendedor: Todos    ";

            if (idFormaPagto > 0)
            {
                where += string.Format(@" And (p.IdFormaPagto={0} OR pglp.IdFormaPagto={0})", idFormaPagto);
                criterio += "Forma Pagto: " + FormaPagtoDAO.Instance.GetDescricao(idFormaPagto);

                if (tipoCartao > 0)
                {
                    where += " And p.IdTipoCartao=" + tipoCartao;
                    criterio += " " + TipoCartaoCreditoDAO.Instance.GetElementByPrimaryKey(tipoCartao).Descricao;
                }

                criterio += "    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(dtIni))
            {
                where += " And p.DataConf>=?dtIni";
                criterio += "Data Início: " + dtIni + "    ";
                temFiltro = true;
            }

            if (!string.IsNullOrEmpty(dtFim))
            {
                where += " And p.DataConf<=?dtFim";
                criterio += "Data Fim: " + dtFim + "    ";
                temFiltro = true;
            }

            var campos = selecionar ? "p.*, " + ClienteDAO.Instance.GetNomeCliente("c") + @" as NomeCliente, f.Nome as NomeFunc, 
                l.NomeFantasia as nomeLoja, '" + criterio + "' as Criterio" : "Count(*)";

            var sql = "Select " + campos + @" From pedido p 
                Inner Join cliente c On (p.idCli=c.id_Cli) 
                Inner Join funcionario f On (p.IdFunc=f.IdFunc) 
                Inner Join loja l On (p.IdLoja = l.IdLoja) 
                LEFT JOIN produtos_liberar_pedido plp ON (p.IdPedido=plp.IdPedido)
                LEFT JOIN liberarpedido lp ON (plp.IdLiberarPedido=lp.IdLiberarPedido)
                LEFT JOIN pagto_liberar_pedido pglp ON (plp.IdLiberarPedido=pglp.IdLiberarPedido)
                Where 1 ?filtroAdicional?" + where + " GROUP BY p.IdPedido";

            return sql;
        }

        public Pedido[] GetForRptAVista(string idLoja, string idVendedor, uint idFormaPagto, uint tipoCartao, string dtIni, string dtFim)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlAVista(idLoja, idVendedor, idFormaPagto, tipoCartao, dtIni, dtFim, true, out temFiltro, out filtroAdicional).
                Replace("?filtroAdicional?", filtroAdicional) + " Order By p.DataConf Desc";

            return objPersistence.LoadData(sql, GetParamAVista(dtIni, dtFim)).ToArray();
        }

        public IList<Pedido> GetListAVista(string idLoja, string idVendedor, uint idFormaPagto, uint tipoCartao, string dtIni, string dtFim, string sortExpression, int startRow, int pageSize)
        {
            var sort = (string.IsNullOrEmpty(sortExpression)) ? "p.DataConf Desc" : sortExpression;

            bool temFiltro;
            string filtroAdicional;

            var sql = SqlAVista(idLoja, idVendedor, idFormaPagto, tipoCartao, dtIni, dtFim, true, out temFiltro, out filtroAdicional).
                Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return LoadDataWithSortExpression(sql, sort, startRow, pageSize, temFiltro, filtroAdicional, GetParamAVista(dtIni, dtFim));
        }

        public int GetCountAVista(string idLoja, string idVendedor, uint idFormaPagto, uint tipoCartao, string dtIni, string dtFim)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlAVista(idLoja, idVendedor, idFormaPagto, tipoCartao, dtIni, dtFim, true, out temFiltro, out filtroAdicional).
                Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, GetParamAVista(dtIni, dtFim));
        }

        private GDAParameter[] GetParamAVista(string dtIni, string dtFim)
        {
            var lstParam = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(dtIni))
                lstParam.Add(new GDAParameter("?dtIni", DateTime.Parse(dtIni + " 00:00")));

            if (!string.IsNullOrEmpty(dtFim))
                lstParam.Add(new GDAParameter("?dtFim", DateTime.Parse(dtFim + " 23:59")));

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
                string.Empty, string.Empty, string.Empty, string.Empty, "2", null, null, null, null, null, 0, true, false, 0, 0, 0, 0, 0,
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

            var tipoPedidoStr = (revenda && !liberarPedido) || (revenda && liberarPedido && !PedidoConfig.TelaConfirmaPedidoLiberacao.ExibirPedidosVendaPopUpConfirmarPedido) ?
                ((int)Pedido.TipoPedidoEnum.Revenda).ToString() : null;

            if (tipoPedido > 0)
                tipoPedidoStr = tipoPedido.ToString();

            var sql = Sql(idPedido, 0, null, null, idLoja, idCli, nomeCli, idFunc, codCliente, 0, null, null, null, null, null, "1",
                String.Empty, String.Empty, String.Empty, "2", dataIni, dataFim, null, null, null, 0, true, false, 0, 0, 0, 0, 0,
                tipoPedidoStr, 0, 0, origemPedido, "", true, out filtroAdicional, out temFiltro).Replace("?filtroAdicional?", filtroAdicional);

            sortExpression = !sortExpression.IsNullOrEmpty() ? sortExpression : "IdPedido";

            return objPersistence.LoadDataWithSortExpression(sql, new InfoSortExpression(sortExpression), new InfoPaging(0,1500),  GetParam(nomeCli, codCliente, null, null, null, null, dataIni, dataFim, null, null, null)).ToArray();
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

            var sql = Sql(idPedido, 0, null, null, 0, idCli, nomeCli, 0, codCliente, 0, null, null, null, null, null, null, null, null, null, null, dataIni,
                dataFim, null, null, null, 0, true, false, 0, 0, 0, 0, 0, null, 0, 0, 0, "", true, out filtroAdicional, out temFiltro).Replace("?filtroAdicional?", filtroAdicional) +
                " and p.situacao=" + (PedidoConfig.LiberarPedido ? (int)Pedido.SituacaoPedido.ConfirmadoLiberacao : (int)Pedido.SituacaoPedido.Confirmado) +
                " and (select count(*) from pedido_espelho where idPedido=p.idPedido)=0 "+ buscarRevenda + " ORDER BY p.IdPedido Asc";

            return objPersistence.LoadData(sql, GetParam(nomeCli, codCliente, null, null, null, null, dataIni, dataFim, null, null, null)).ToArray();
        }

        #endregion

        #region Busca pedidos para Liberação

        private string SqlLiberacao(uint idCliente, string nomeCliente, string idsPedidos, string dataIniEntrega, string dataFimEntrega,
            int situacaoProd, string tiposPedidos, int? idLoja)
        {
            // O Left Join com funcionário deve ser left porque aconteceu de um pedido ter sido tirado pela web e 
            // ter ficado com idFunc=0

            var buscarOcs = OrdemCargaConfig.UsarControleOrdemCarga ? ", CAST((SELECT GROUP_CONCAT(idOrdemCarga) FROM pedido_ordem_carga WHERE idPedido = p.idPedido) as CHAR) as IdsOCs " : " ";

            var sql = @"
                Select p.*, " + ClienteDAO.Instance.GetNomeCliente("c") + @" as NomeCliente, c.Revenda as CliRevenda, f.Nome as NomeFunc, o.saldo as saldoObra, 
                    (" + ObraDAO.Instance.SqlPedidosAbertos("p.idObra", "p.idPedido", ObraDAO.TipoRetorno.TotalPedido) + @") as totalPedidosAbertosObra, l.NomeFantasia as nomeLoja, 
                    fp.Descricao as FormaPagto, pe.Total as TotalEspelho, s.dataCad as dataEntrada,
                    s.usuCad as usuEntrada, cast(s.valorCreditoAoCriar as decimal(12,2)) as valorCreditoAoReceberSinal, 
                    cast(s.creditoGeradoCriar as decimal(12,2)) as creditoGeradoReceberSinal,
                    cast(s.creditoUtilizadoCriar as decimal(12,2)) as creditoUtilizadoReceberSinal, s.isPagtoAntecipado as pagamentoAntecipado,
                    c.ObsLiberacao as ObsLiberacaoCliente"+ buscarOcs +
                @"From pedido p 
                    Left Join pedido_espelho pe On (p.idPedido=pe.idPedido) 
                    Inner Join cliente c On (p.idCli=c.id_Cli) 
                    Left Join funcionario f On (p.IdFunc=f.IdFunc) 
                    Inner Join loja l On (p.IdLoja = l.IdLoja) 
                    Left Join obra o On (p.idObra=o.idObra)
                    Left Join formapagto fp On (fp.IdFormaPagto=p.IdFormaPagto) 
                    Left Join sinal s On (p.idSinal=s.idSinal)
                Where p.situacao in (" + (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + ", " + (int)Pedido.SituacaoPedido.LiberadoParcialmente +
                    ") and p.tipoPedido<>" + (int)Pedido.TipoPedidoEnum.Producao;

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
            var itens = objPersistence.LoadResult(SqlLiberacao(idCliente, nomeCliente, null, dataIniEntrega, dataFimEntrega, situacaoProd, tiposPedidos,idLoja),
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

            return objPersistence.LoadData(SqlLiberacao(0, null, idsPedidos, null, null, 0, null, null)).ToArray();
        }

        public Pedido[] GetForLiberacao(uint idCliente, uint idPedido)
        {
            if (idCliente == 0 && idPedido == 0)
                return new Pedido[0];

            return objPersistence.LoadData(SqlLiberacao(idCliente, null, idPedido.ToString(), null, null, 0, null, null)).ToArray();
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
                null, null, null, null, null, null, null, null, null, null, null, 0, false, false, 0, 0, 0, 0, 0, null, 0, 0, 0, "",
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
                null, null, null, null, null, null, null, null, null, null, null, null, 0, false, false, 0, 0, 0, 0, 0, null,
                0, 0, 0, "", true, out filtroAdicional, out temFiltro).Replace("?filtroAdicional?", filtroAdicional);

            var situacoes = (int)Pedido.SituacaoPedido.Confirmado + "," + (int)Pedido.SituacaoPedido.ConfirmadoLiberacao + "," +
                (int)Pedido.SituacaoPedido.LiberadoParcialmente;

            if (FiscalConfig.PermitirGerarNotaPedidoConferido)
                situacoes += "," + (int)Pedido.SituacaoPedido.Conferido;

            sql += " and p.Situacao in (" + situacoes + ")"; //" and (select coalesce(count(*), 0) from pedidos_nota_fiscal pnf where p.IdPedido=pnf.IdPedido)=0";

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

        #region Busca pedidos para informações de produção

        /// <summary>
        /// Retorna os pedidos para informações de produção.
        /// </summary>
        public Pedido[] GetForInfoPedidos(string dataIni, string dataFim, uint idPedido, uint idCliente, string nomeCliente, int tipo, int fastDelivery)
        {
            var vendas = tipo == 1 ? "1" : string.Empty;
            var maoDeObra = tipo == 2 ? "1" : string.Empty;
            var producao = tipo == 3 ? "1" : string.Empty;
            var maoDeObraEspecial = tipo == 4 ? "1" : string.Empty;

            bool temFiltro;
            string filtroAdicional;

            var sql = Sql(idPedido, 0, null, null, 0, idCliente, nomeCliente, 0, null, 0, null, null, null, null, null, null, vendas, maoDeObra, maoDeObraEspecial,
                producao, null, null, null, null, null, 0, false, true, 0, 0, 0, 0, 0, null, 0, 0, 0, "", true, out filtroAdicional, out temFiltro).Replace("?filtroAdicional?", filtroAdicional);

            sql += " and p.DataEntrega>=?inicio And p.DataEntrega<=?fim and p.Situacao<>" + (int)Pedido.SituacaoPedido.Cancelado + @"
                and prod.idSubgrupoProd<>" + (int)Utils.SubgrupoProduto.LevesDefeitos + @" and if(p.tipoPedido=" + (int)Pedido.TipoPedidoEnum.Producao + @", true, 
                prod.idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @" and (s1.TipoCalculo<>" + (int)Glass.Data.Model.TipoCalculoGrupoProd.Qtd + @" || s1.TipoCalculo is null))
                and p.totM>0";

            if (fastDelivery > 0)
                sql += " and (p.FastDelivery" + (fastDelivery == 1 ? "=true" : "=false or p.FastDelivery is null") + ")";

            sql += " group by p.idPedido";

            return objPersistence.LoadData(sql, new GDAParameter("?inicio", DateTime.Parse(dataIni + " 00:00:00")),
                new GDAParameter("?fim", DateTime.Parse(dataFim + " 23:59:59")), new GDAParameter("?nomeCli", "%" + nomeCliente + "%")).ToArray();
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

            if(fastDelivery)
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
                                        WHERE IF(p.OrdemCargaParcial, (oc.Situacao <> "+ (int)OrdemCarga.SituacaoOCEnum.CarregadoParcialmente + "), 1) AND oc.tipoOrdemCarga=" + (int)OrdemCarga.TipoOCEnum.Venda +
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
        /// <param name="sessao"></param>
        /// <param name="idsPedidos"></param>
        /// <returns></returns>
        public List<Pedido> ObterPedidosProducao(GDASession sessao, List<uint> idsPedidos)
        {
            var sql = @"
                SELECT p.*, c.Id_cli as IdCli, c.Nome as NomeCliente
                FROM pedido p
	                LEFT JOIN cliente c ON (p.IdCli = c.Id_cli)
                WHERE p.idPedido IN ({0})
                ORDER BY p.DataEntrega, c.Nome";

            return objPersistence.LoadData(sessao, string.Format(sql, string.Join(",", idsPedidos)));
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
                    if(tipoSubgrupo == TipoSubgrupoProd.VidroLaminado || tipoSubgrupo == TipoSubgrupoProd.VidroDuplo)
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
                var ambientes = AmbientePedidoDAO.Instance.GetByPedido(session, idPedido).ToArray();
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
            foreach(var id in idsProjetoModelo)
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

                string msg;
                foreach (var amb in AmbientePedidoDAO.Instance.GetByPedido(session, idPedido))
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
                    var ambientes = AmbientePedidoDAO.Instance.GetByPedido(session, idPedido);

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

                if(PedidoConfig.Comissao.UsarComissaoPorTipoPedido)
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
                var subGrupos = SubgrupoProdDAO.Instance.ObtemSubgrupos(sessao, lstProd.Where(f=> f.IdSubgrupoProd > 0).Select(f => (int)f.IdSubgrupoProd).ToList()).Distinct();

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
                            if ((ProdutoDAO.Instance.ObtemIdCorVidro(sessao, (int)lstProd[0].IdProd) != ProdutoDAO.Instance.ObtemIdCorVidro(sessao, (int)p.IdProd) ||
                                ProdutoDAO.Instance.ObtemEspessura(sessao, (int)lstProd[0].IdProd) != ProdutoDAO.Instance.ObtemEspessura(sessao, (int)p.IdProd)) && p.IdProdPedParent.GetValueOrDefault() == 0)
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

        #region Confirmar Pedido

        private static readonly object _confirmarPedidoLock = new object();

        /// <summary>
        /// Confirma o pedido, gera contas a receber, dá baixa no estoque, 
        /// </summary>
        public string ConfirmarPedido(uint idPedido, uint[] formasPagto, uint[] tiposCartao, decimal[] valores, uint[] contasBanco, uint[] depositoNaoIdentificado,
            bool gerarCredito, decimal creditoUtilizado, string numAutConstrucard, uint[] numParcCartoes, string chequesPagto,
            bool descontarComissao, int tipoVendaObra, bool verificarParcelas, string[] numAutCartao)
        {
            lock (_confirmarPedidoLock)
            {
                FilaOperacoes.ConfirmarPedido.AguardarVez();

                using (var trans = new GDATransaction())
                {
                    try
                    {
                        trans.BeginTransaction();

                        Pedido ped = null;
                        string msg = string.Empty;
                        uint idCliente = PedidoDAO.Instance.ObtemValorCampo<uint>(trans, "idCli", "idPedido=" + idPedido);
                        int? tipoVenda = GetTipoVenda(trans, idPedido);
                        Pedido.TipoPedidoEnum tipoPedido = GetTipoPedido(trans, idPedido);
                        uint idFuncVenda = ObtemValorCampo<uint>(trans, "idFuncVenda", "idPedido=" + idPedido);
                        uint? idFormaPagto = ObtemValorCampo<uint?>(trans, "idFormaPagto", "idPedido=" + idPedido);
                        DateTime? dataEntrega = ObtemDataEntrega(trans, idPedido);
                        UtilsFinanceiro.DadosRecebimento retorno = null;
                        ProdutosPedido[] lstProdPed = null;

                        // Guarda id do caixa diario inserido pelo valor da entrada do pedido
                        List<uint> lstIdContaRecVista = new List<uint>();
                        List<uint> lstIdContaRecPrazo = new List<uint>();

                        decimal totalPedido = GetTotal(trans, idPedido);

                        decimal totalPago = 0;

                        // Se a empresa tiver permissão para trabalhar com caixa diário
                        if (Geral.ControleCaixaDiario)
                        {
                            if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario))
                                throw new Exception("Apenas o Caixa pode confirmar pedidos.");
                        }
                        else
                        {
                            if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) &&
                                !Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario))
                                throw new Exception("Você não tem permissão para confirmar pedidos.");
                        }

                        Pedido.SituacaoPedido situacao = ObtemSituacao(trans, idPedido);

                        // Se o pedido não estiver conferido ou não tiver produtos associados, não pode ser confirmado
                        if (situacao == Pedido.SituacaoPedido.Confirmado)
                            throw new Exception("Pedido já confirmado.");

                        if (situacao != Pedido.SituacaoPedido.Conferido)
                            throw new Exception("O pedido ainda não foi conferido, portanto não pode ser confirmado.");

                        if (TemSinalReceber(trans, idPedido))
                            LancarExceptionValidacaoPedidoFinanceiro("O pedido tem sinal a receber. Receba-o para confirmar o pedido.",
                                idPedido, false, null, ObservacaoFinalizacaoFinanceiro.MotivoEnum.Confirmacao);

                        else if (ProdutosPedidoDAO.Instance.GetCount(trans, idPedido, 0, false, 0) < 1)
                            throw new Exception("O pedido não pode ser confirmado por não haver nenhum Produto associado ao mesmo.");

                        // Verifica se o pedido já possui contas a receber/recebidas para impedir que o mesmo seja confirmado duas vezes
                        if (ObtemIdSinal(trans, idPedido) == null && ContasReceberDAO.Instance.ExistsByPedido(trans, idPedido))
                            throw new Exception("Este pedido não pode ser confirmado pois já possui contas a receber/recebidas");

                        // Verifica se a data de entrega é inferior a hoje
                        if (dataEntrega != null && dataEntrega.Value < DateTime.Parse(DateTime.Now.ToString("dd/MM/yyyy 00:00:00")))
                            throw new Exception("A data de entrega do pedido não pode ser inferior à hoje, data de entrega: " + dataEntrega.Value.ToString("dd/MM/yyyy"));

                        var valorEntrada = ObtemValorEntrada(trans, idPedido);
                        var idPagamentoAntecipado = ObtemValorCampo<int>(trans, "IdPagamentoAntecipado", string.Format("IdPedido={0}", idPedido));
                        var valorPagamentoAntecipado = ObtemValorCampo<decimal>(trans, "ValorPagamentoAntecipado", string.Format("IdPedido={0}", idPedido));
                        var idSinal = ObtemValorCampo<int>(trans, "IdSinal", string.Format("IdPedido={0}", idPedido));

                        /* Chamado 52558.
                         * Se o pedido tiver sido pago antecipadamente e o valor de entrada mais o valor pago não seja igual ao valor do pedido, bloqueia a confirmação dele. */
                        if (valorPagamentoAntecipado > 0 && (idSinal > 0 ? valorEntrada : 0) + valorPagamentoAntecipado != totalPedido)
                            throw new Exception("O valor de entrada somado ao valor pago antecipadamente ultrapassa o valor total do pedido.");
                        
                        // Quando aplicável, verifica se os produtos do pedido existem em estoque
                        if (tipoPedido != Pedido.TipoPedidoEnum.Producao)
                        {
                            var pedidoReposicaoGarantia = tipoVenda == (int)Pedido.TipoVendaPedido.Reposição || tipoVenda == (int)Pedido.TipoVendaPedido.Garantia;
                            var pedidoMaoObraEspecial = tipoPedido == Pedido.TipoPedidoEnum.MaoDeObraEspecial;
                            var lstProd = ProdutosPedidoDAO.Instance.GetByPedidoLite(trans, idPedido).ToArray();

                            foreach (var prod in lstProd)
                            {
                                var qtdProd = 0F;
                                var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(trans, (int)prod.IdProd);

                                // É necessário refazer o loop nos produtos do pedido para que caso tenha sido inserido o mesmo produto 2 ou mais vezes,
                                // seja somada a quantidade total inserida no pedido
                                foreach (var prod2 in lstProd)
                                {
                                    // Soma somente produtos iguais ao produto do loop principal de produtos
                                    if (prod.IdProd != prod2.IdProd)
                                        continue;

                                    if (tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 ||
                                        tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto)
                                        qtdProd += prod2.TotM;

                                    else if (tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 ||
                                        tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                                        tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 ||
                                        tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6)
                                        qtdProd += prod2.Qtde * prod2.Altura;

                                    else
                                        qtdProd += prod2.Qtde;
                                }

                                if (GrupoProdDAO.Instance.BloquearEstoque(trans, (int)prod.IdGrupoProd, (int)prod.IdSubgrupoProd))
                                {
                                    var estoque = ProdutoLojaDAO.Instance.GetEstoque(trans, ObtemIdLoja(trans, idPedido), prod.IdProd, null, IsProducao(trans, idPedido), false, true);

                                    if (estoque < qtdProd)
                                        throw new Exception("O produto " + prod.DescrProduto + " possui apenas " + estoque + " em estoque.");
                                }

                                // Verifica se o valor unitário do produto foi informado, pois pode acontecer do usuário inserir produtos zerados em 
                                // um pedido reposição/garantia e depois alterar o pedido para à vista/à prazo
                                if (!pedidoReposicaoGarantia && prod.ValorVendido == 0)
                                    throw new Exception("O produto " + prod.DescrProduto + " não pode ter valor zerado.");
                            }
                        }

                        ParcelasPedido[] lstParcPed;
                        lstProdPed = ProdutosPedidoDAO.Instance.GetByPedidoLite(trans, idPedido).ToArray();

                        // Se for venda para funcionário
                        if (idFuncVenda > 0 && tipoVenda == (int)Pedido.TipoVendaPedido.Funcionario)
                        {
                            #region Venda para funcionário

                            // Gera a movimentação no controle
                            MovFuncDAO.Instance.MovimentarPedido(trans, idFuncVenda, idPedido,
                                UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.FuncRecebimento), 2, totalPedido, null);

                            #endregion
                        }
                        // Se for venda à vista
                        else if (tipoVenda == (int)Glass.Data.Model.Pedido.TipoVendaPedido.AVista ||
                            (tipoVenda == (int)Pedido.TipoVendaPedido.Obra && tipoVendaObra == (int)Pedido.TipoVendaPedido.AVista))
                        {
                            #region Venda à vista

                            if (tipoVenda == (int)Pedido.TipoVendaPedido.Obra)
                                totalPago += ObraDAO.Instance.GetSaldo(trans, ObtemValorCampo<uint>(trans, "idObra", "idPedido=" + idPedido));

                            foreach (decimal valor in valores)
                                totalPago += valor;

                            if (UtilsFinanceiro.ContemFormaPagto(Pagto.FormaPagto.ChequeProprio, formasPagto) && String.IsNullOrEmpty(chequesPagto))
                                throw new Exception("Cadastre o(s) cheque(s) referente(s) ao pagamento do pedido.");

                            // Se for pago com crédito, soma o mesmo ao totalPago
                            if (creditoUtilizado > 0)
                                totalPago += creditoUtilizado;

                            // Ignora os juros dos cartões ao calcular o valor pago/a pagar
                            totalPago -= UtilsFinanceiro.GetJurosCartoes(trans, UserInfo.GetUserInfo.IdLoja, valores, formasPagto, tiposCartao, numParcCartoes);

                            // Se o valor for inferior ao que deve ser pago, e o restante do pagto for gerarCredito, lança exceção
                            if (gerarCredito && totalPago < totalPedido)
                                throw new Exception("Total a ser pago não confere com valor pago. Total a ser pago: " + totalPedido.ToString("C") + " Valor pago: " + totalPago.ToString("C"));
                            // Se o total a ser pago for diferente do valor pago, considerando que não é para gerar crédito
                            else if (!gerarCredito && Math.Round(totalPedido, 2) != Math.Round(totalPago, 2))
                                throw new Exception("Total a ser pago não confere com valor pago. Total a ser pago: " + totalPedido.ToString("C") + " Valor pago: " + totalPago.ToString("C"));

                            #region Atualiza formas de pagamento

                            bool possuiFormaPagto = false;

                            for (int i = 0; i < formasPagto.Length; i++)
                                if (formasPagto[i] > 0)
                                {
                                    possuiFormaPagto = true;
                                    break;
                                }

                            if (possuiFormaPagto)
                            {
                                // Atualiza forma de pagamento de acordo com aquela que foi escolhida pelo caixa.
                                if (formasPagto.Length > 0 && formasPagto[0] > 0)
                                    objPersistence.ExecuteCommand(trans,
                                        string.Format("UPDATE pedido SET IdFormaPagto={0} Where IdPedido={1}", formasPagto[0], idPedido));

                                if (formasPagto.Length > 1 && formasPagto[1] > 0)
                                    objPersistence.ExecuteCommand(trans,
                                        string.Format("UPDATE pedido SET IdFormaPagto2={0} Where IdPedido={1}", formasPagto[1], idPedido));

                                // Atualiza tipo de cartão de acordo com aquele que foi escolhido pelo caixa
                                if ((uint)Pagto.FormaPagto.Cartao == formasPagto[0])
                                    objPersistence.ExecuteCommand(trans,
                                        string.Format("UPDATE pedido SET IdTipoCartao={0} WHERE IdPedido={1}", tiposCartao[0], idPedido));

                                if (formasPagto.Length > 1 && (uint)Pagto.FormaPagto.Cartao == formasPagto[1])
                                    objPersistence.ExecuteCommand(trans,
                                        string.Format("UPDATE pedido SET IdTipoCartao2={0} WHERE IdPedido={1}", tiposCartao[1], idPedido));
                            }
                            else if (creditoUtilizado == totalPedido) // Foi pago com crédito
                            {
                                // Altera a forma de pagamento do pedido para crédito
                                objPersistence.ExecuteCommand(trans, "Update pedido set idFormaPagto=" + (uint)Pagto.FormaPagto.Credito + " Where IdPedido=" + idPedido);
                            }

                            #endregion

                            #endregion
                        }
                        // Se for venda à prazo
                        else if (tipoVenda == (int)Glass.Data.Model.Pedido.TipoVendaPedido.APrazo || (tipoVenda == (int)Pedido.TipoVendaPedido.Obra && tipoVendaObra == (int)Pedido.TipoVendaPedido.APrazo))
                        {
                            #region Venda à prazo
                            
                            // Se o pedido possuir sinal e o mesmo nao tiver sido recebido
                            // o pedido não poderá ser finalizado
                            if (valorEntrada > 0 && idSinal == 0)
                            {
                                var pedidoRecebido = false;

                                /* Chamado 37593. */
                                if (idPagamentoAntecipado > 0)
                                {
                                    if (valorPagamentoAntecipado == totalPedido)
                                    {
                                        pedidoRecebido = true;
                                        objPersistence.ExecuteCommand(trans, string.Format("UPDATE pedido SET ValorEntrada=NULL WHERE IdPedido={0}", idPedido));
                                    }
                                }

                                if (!pedidoRecebido)
                                    throw new Exception("Receba o sinal do pedido antes de confirmá-lo.");
                            }

                            if (idPagamentoAntecipado == 0 || valorPagamentoAntecipado != totalPedido)
                            {
                                // Calcula o valor de entrada + o valor à prazo
                                decimal valorTotalPago = valorEntrada;
                                if (tipoVenda == (int)Pedido.TipoVendaPedido.Obra)
                                    valorTotalPago += ObraDAO.Instance.GetSaldo(trans, ObtemValorCampo<uint>(trans, "idObra", "idPedido=" + idPedido));

                                // Busca o valor das parcelas e verifica se as parcelas possuem data igual ou maior que hoje (dia da confirmação)
                                var lstParc = ParcelasPedidoDAO.Instance.GetByPedido(trans, idPedido).ToArray();
                                foreach (ParcelasPedido p in lstParc)
                                {
                                    if (verificarParcelas && (p.Data != null && p.Data < DateTime.Now.AddDays(-1)))
                                        throw new Exception("A data de vencimento das parcelas do pedido deve ser igual ou maior que a data de hoje.");

                                    valorTotalPago += p.Valor;
                                }

                                // Verifica se o valor total do pedido bate com o valorTotalPago
                                if (Math.Round(totalPedido, 2) != Math.Round(valorTotalPago, 2))
                                    throw new Exception("O valor total do pedido não bate com o valor do pagamento do mesmo. Valor Pedido: " + Math.Round(totalPedido, 2).ToString("C") + " Valor Pago: " + Math.Round(valorTotalPago, 2).ToString("C"));
                                
                                // Verifica se cliente possui limite disponível para realizar a compra
                                decimal limite = ClienteDAO.Instance.ObtemValorCampo<decimal>(trans, "limite", "id_Cli=" + idCliente);

                                // Determina o valor que será somado aos débitos do cliente para verificar se ficará tudo dentro do limite
                                decimal valorAConsiderar = FinanceiroConfig.DebitosLimite.EmpresaConsideraPedidoConferidoLimite ? 0 : totalPedido - ObtemValorEntrada(trans, idPedido);

                                if (limite > 0 && ContasReceberDAO.Instance.GetDebitos(trans, idCliente, null) + valorAConsiderar > limite)
                                    LancarExceptionValidacaoPedidoFinanceiro("O cliente não possui limite disponível para realizar esta compra. Contate o setor Financeiro.",
                                        idPedido, false, null, ObservacaoFinalizacaoFinanceiro.MotivoEnum.Confirmacao);
                            }

                            #endregion
                        }

                        decimal creditoAtual = ClienteDAO.Instance.GetCredito(trans, idCliente);

                        GerarInstalacao(trans, idPedido, dataEntrega);

                        #region Gera contas a receber se for venda à prazo

                        try
                        {
                            var pagamentoAntecipado = ObtemIdPagamentoAntecipado(trans, idPedido);
                            if (tipoVenda == (int)Pedido.TipoVendaPedido.APrazo && pagamentoAntecipado.GetValueOrDefault() == 0)
                            {
                                ContasReceber conta;
                                lstParcPed = ParcelasPedidoDAO.Instance.GetByPedido(trans, idPedido).ToArray();

                                if (idFormaPagto == null)
                                    throw new Exception("A forma de pagamento não foi definida.");

                                // Exclui todas as contas a receber do pedido antes de gerar as que serão geradas abaixo
                                ContasReceberDAO.Instance.DeleteByPedido(trans, idPedido);

                                var numParc = ContasReceberDAO.Instance.ObtemNumParcPedido(trans, idPedido);

                                foreach (var p in lstParcPed)
                                {
                                    conta = new ContasReceber
                                    {
                                        IdLoja = UserInfo.GetUserInfo.IdLoja,
                                        IdCliente = idCliente,
                                        IdPedido = idPedido,
                                        DataVec = p.Data.Value,
                                        ValorVec = p.Valor,
                                        IdConta = UtilsPlanoConta.GetPlanoPrazo(idFormaPagto.Value),
                                        NumParc = numParc,
                                        IdFormaPagto = idFormaPagto.Value,
                                        IdFuncComissaoRec = idCliente > 0 ? (int?)ClienteDAO.Instance.ObtemIdFunc(idCliente) : null
                                };
                                    numParc++;
                                    conta.IdContaR = ContasReceberDAO.Instance.Insert(trans, conta);

                                    if (conta.IdContaR == 0)
                                        throw new Exception("Conta a Receber não foi inserida.");

                                    lstIdContaRecPrazo.Add(conta.IdContaR);
                                }

                                // Atualiza o número de parcelas do pedido
                                ContasReceberDAO.Instance.AtualizaNumParcPedido(trans, idPedido, numParc - 1);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao gerar contas a receber.", ex));
                        }

                        #endregion

                        ped = GetElementByPrimaryKey(trans, idPedido);

                        #region Gera movimentação no caixa diário ou no caixa geral e ou na conta bancária quando for venda à vista

                        try
                        {
                            if (tipoVenda == (int)Glass.Data.Model.Pedido.TipoVendaPedido.AVista || (tipoVenda == (int)Pedido.TipoVendaPedido.Obra && tipoVendaObra == (int)Pedido.TipoVendaPedido.AVista))
                            {
                                retorno = UtilsFinanceiro.Receber(trans, UserInfo.GetUserInfo.IdLoja, ped, null, null, null, null, null, null, null,
                                    null, null, null, idCliente, 0, null, DateTime.Now.ToString("dd/MM/yyyy"), totalPedido, totalPago, valores, formasPagto, contasBanco, depositoNaoIdentificado, new uint[] { }, tiposCartao,
                                    null, null, 0, false, gerarCredito, creditoUtilizado, numAutConstrucard, Geral.ControleCaixaDiario,
                                    numParcCartoes, chequesPagto, descontarComissao, UtilsFinanceiro.TipoReceb.PedidoAVista);

                                if (retorno.ex != null)
                                    throw retorno.ex;
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao inserir valor no caixa diário.", ex));
                        }

                        #endregion

                        ped.ValorCreditoAoConfirmar = creditoAtual;
                        ped.CreditoGeradoConfirmar = retorno != null ? retorno.creditoGerado : 0;
                        ped.CreditoUtilizadoConfirmar = creditoUtilizado;
                        UpdateBase(trans, ped);

                        #region Gera conta recebida se o pedido for à vista

                        if (!ped.VendidoFuncionario && (ped.TipoVenda == (int)Pedido.TipoVendaPedido.AVista || (ped.TipoVenda == (int)Pedido.TipoVendaPedido.Obra && tipoVendaObra == (int)Pedido.TipoVendaPedido.AVista)))
                        {
                            for (int i = 0; i < valores.Length; i++)
                            {
                                if (valores[i] <= 0)
                                    continue;

                                ContasReceber contaRecSinal = new ContasReceber
                                {
                                    IdLoja = UserInfo.GetUserInfo.IdLoja,
                                    IdPedido = ped.IdPedido,
                                    IdCliente = ped.IdCli,
                                    IdConta = UtilsPlanoConta.GetPlanoVista(formasPagto[i]),
                                    DataVec = DateTime.Now,
                                    ValorVec = valores[i],
                                    DataRec = DateTime.Now,
                                    ValorRec = valores[i],
                                    Recebida = true,
                                    UsuRec = UserInfo.GetUserInfo.CodUser,
                                    NumParc = 1,
                                    NumParcMax = 1,
                                    IdFuncComissaoRec = ped.IdCli > 0 ? (int?)ClienteDAO.Instance.ObtemIdFunc(ped.IdCli) : null
                            };

                                var idContaR = ContasReceberDAO.Instance.Insert(trans, contaRecSinal);

                                lstIdContaRecVista.Add(idContaR);

                                var pagto = new PagtoContasReceber
                                {
                                    IdContaR = idContaR,
                                    IdFormaPagto = formasPagto[i],
                                    IdContaBanco =
                                        formasPagto[i] != (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro &&
                                        contasBanco[i] > 0
                                            ? (uint?)contasBanco[i]
                                            : null,
                                    IdTipoCartao = tiposCartao[i] > 0 ? (uint?)tiposCartao[i] : null,
                                    IdDepositoNaoIdentificado =
                                        depositoNaoIdentificado[i] > 0 ? (uint?)depositoNaoIdentificado[i] : null,
                                    ValorPagto = valores[i],
                                    NumAutCartao = numAutCartao[i]
                                };

                                PagtoContasReceberDAO.Instance.Insert(trans, pagto);
                            }

                            if (creditoUtilizado > 0)
                            {
                                ContasReceber contaRecSinal = new ContasReceber();
                                contaRecSinal.IdLoja = UserInfo.GetUserInfo.IdLoja;
                                contaRecSinal.IdPedido = ped.IdPedido;
                                contaRecSinal.IdCliente = ped.IdCli;
                                contaRecSinal.IdConta = UtilsPlanoConta.GetPlanoVista((uint)Glass.Data.Model.Pagto.FormaPagto.Credito);
                                contaRecSinal.DataVec = DateTime.Now;
                                contaRecSinal.ValorVec = creditoUtilizado;
                                contaRecSinal.DataRec = DateTime.Now;
                                contaRecSinal.ValorRec = creditoUtilizado;
                                contaRecSinal.Recebida = true;
                                contaRecSinal.UsuRec = UserInfo.GetUserInfo.CodUser;
                                contaRecSinal.NumParc = 1;
                                contaRecSinal.NumParcMax = 1;

                                var idContaR = ContasReceberDAO.Instance.Insert(trans, contaRecSinal);

                                lstIdContaRecVista.Add(idContaR);

                                var pagto = new PagtoContasReceber();
                                pagto.IdContaR = idContaR;
                                pagto.IdFormaPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.Credito;
                                pagto.ValorPagto = creditoUtilizado;

                                PagtoContasReceberDAO.Instance.Insert(trans, pagto);
                            }
                        }

                        #endregion

                        #region Atualiza custo do pedido

                        try
                        {
                            UpdateCustoPedido(trans, idPedido);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao atualizar custo do pedido.", ex));
                        }

                        #endregion

                        #region Altera situação do pedido para Confirmado

                        try
                        {
                            ped.UsuConf = UserInfo.GetUserInfo.CodUser;
                            ped.DataConf = DateTime.Now;
                            ped.Situacao = Pedido.SituacaoPedido.Confirmado;

                            if (UtilsFinanceiro.ContemFormaPagto(Glass.Data.Model.Pagto.FormaPagto.Construcard, formasPagto) &&
                                ped.TipoVenda == (int)Pedido.TipoVendaPedido.AVista && !String.IsNullOrEmpty(numAutConstrucard))
                                ped.NumAutConstrucard = numAutConstrucard;

                            if (ped.TipoVenda != (int)Pedido.TipoVendaPedido.APrazo)
                            {
                                AlteraLiberarFinanc(trans, idPedido, true);
                                ped.LiberadoFinanc = true;
                            }

                            if (PedidoDAO.Instance.UpdateBase(trans, ped) == 0)
                                PedidoDAO.Instance.UpdateBase(trans, ped);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao atualizar situação do pedido.", ex));
                        }

                        #endregion

                        #region Coloca produtos na reserva no estoque da loja

                        try
                        {
                            uint idSaidaEstoque = 0;
                            if (FinanceiroConfig.Estoque.SaidaEstoqueAutomaticaAoConfirmar)
                                idSaidaEstoque = SaidaEstoqueDAO.Instance.GetNewSaidaEstoque(trans, ped.IdLoja, idPedido, null, null, false);

                            var idsProdQtde = new Dictionary<int, float>();

                            foreach (var p in lstProdPed)
                            {
                                var m2 = new List<int> { (int)TipoCalculoGrupoProd.M2, (int)TipoCalculoGrupoProd.M2Direto }.Contains(GrupoProdDAO.Instance.TipoCalculo(trans, (int)p.IdGrupoProd, (int)p.IdSubgrupoProd));

                                var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(trans, (int)p.IdProd);
                                var qtdSaida = p.Qtde - p.QtdSaida;

                                if (tipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 ||
                                    tipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)TipoCalculoGrupoProd.MLAL6 ||
                                    tipoCalculo == (int)TipoCalculoGrupoProd.ML)
                                    qtdSaida *= p.Altura;

                                // Salva produto e qtd de saída para executar apenas um sql de atualização de estoque
                                if (!idsProdQtde.ContainsKey((int)p.IdProd))
                                    idsProdQtde.Add((int)p.IdProd, m2 ? p.TotM : qtdSaida);
                                else
                                    idsProdQtde[(int)p.IdProd] += m2 ? p.TotM : qtdSaida;

                                if (FinanceiroConfig.Estoque.SaidaEstoqueAutomaticaAoConfirmar && qtdSaida > 0)
                                {
                                    ProdutosPedidoDAO.Instance.MarcarSaida(trans, p.IdProdPed, p.Qtde - p.QtdSaida, idSaidaEstoque);

                                    // Dá baixa no estoque da loja
                                    MovEstoqueDAO.Instance.BaixaEstoquePedido(trans, p.IdProd, ped.IdLoja, idPedido, p.IdProdPed,
                                        (decimal)(m2 ? p.TotM : qtdSaida), (decimal)(m2 ? p.TotM2Calc : 0), true, null);
                                }
                            }

                            if (!ped.Producao)
                            {
                                if (!FinanceiroConfig.Estoque.SaidaEstoqueAutomaticaAoConfirmar)
                                    ProdutoLojaDAO.Instance.ColocarReserva(trans, (int)ped.IdLoja, idsProdQtde, null, null, null, null,
                                        (int)idPedido, null, null, "PedidoDAO - ConfirmarPedido");
                                else
                                    MarcaPedidoEntregue(trans, idPedido);
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao dar baixa no estoque.", ex));
                        }

                        #endregion

                        #region Gera a comissão do pedido

                        if (descontarComissao)
                            ComissaoDAO.Instance.GerarComissao(trans, Pedido.TipoComissao.Comissionado, ped.IdComissionado.Value, ped.IdPedido.ToString(), ped.DataConf.Value.ToString(), ped.DataConf.Value.ToString(), 0, null);

                        #endregion

                        // Atualiza data da última compra para hoje
                        ClienteDAO.Instance.AtualizaUltimaCompra(trans, ped.IdCli);

                        // Atualiza o total comprado
                        ClienteDAO.Instance.AtualizaTotalComprado(trans, ped.IdCli);

                        msg = "Pedido confirmado. ";

                        if (retorno != null && retorno.creditoGerado > 0)
                            msg += "Foi gerado " + retorno.creditoGerado.ToString("C") + " de crédito para o cliente. ";

                        if (retorno != null && retorno.creditoDebitado)
                            msg += "Foi utilizado " + creditoUtilizado.ToString("C") + " de crédito do cliente, restando " +
                                ClienteDAO.Instance.GetCredito(trans, idCliente).ToString("C") + " de crédito. ";

                        trans.Commit();
                        trans.Close();

                        return msg;
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        trans.Close();
                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao confirmar pedido.", ex));
                    }
                    finally
                    {
                        FilaOperacoes.ConfirmarPedido.ProximoFila();
                    }
                }
            }
        }

        /// <summary>
        /// Marca o pedido como entregue após baixar o estoque do mesmo, se todos os produtos tiverem dado baixa e 
        /// se a empresa não trabalhar com produção de vidro ou não possuir vidros de produção no pedido
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        public void MarcaPedidoEntregue(GDASession sessao, uint idPedido)
        {
            if (!PossuiProdutosPendentesSaida(sessao, idPedido) &&
                (!PCPConfig.ControlarProducao || ((!PossuiVidrosEstoque(sessao, idPedido) || Geral.EmpresaSomenteRevendeBox)
                && !PossuiVidrosProducao(sessao, idPedido))))
                AlteraSituacaoProducao(sessao, idPedido, Pedido.SituacaoProducaoEnum.Entregue);
        }

        #endregion

        #region Gera Instalações para o pedido

        /// <summary>
        /// Gera Instalações para o pedido
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <param name="dataEntrega"></param>
        public void GerarInstalacao(GDASession sessao, uint idPedido, DateTime? dataEntrega)
        {
            try
            {
                if (!Geral.ControleInstalacao)
                    return;

                int tipoEntrega = ObtemTipoEntrega(sessao, idPedido);

                if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Comum || tipoEntrega == (int)Pedido.TipoEntregaPedido.Temperado ||
                    tipoEntrega == (int)Pedido.TipoEntregaPedido.ManutencaoTemperado || tipoEntrega == (int)Pedido.TipoEntregaPedido.Esquadria ||
                    tipoEntrega == (int)Pedido.TipoEntregaPedido.Entrega)
                {
                    bool comum = ContemTipo(sessao, idPedido, 1);
                    bool temperado = ContemTipo(sessao, idPedido, 2);
                    bool entrega = false;

                    // Se o tipo de entrega for esquadria, gera instalação temperado
                    if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Esquadria)
                    {
                        comum = false;
                        temperado = true;
                    }

                    // Se o tipo de entrega for entrega, gera instalação Entrega
                    else if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Entrega)
                    {
                        comum = false;
                        temperado = false;
                        entrega = true;
                        InstalacaoDAO.Instance.NovaInstalacao(sessao, idPedido, dataEntrega.Value, 4, false);
                    }

                    // Se tiver produtos temperado, gera instalação temperado
                    if (temperado)
                        InstalacaoDAO.Instance.NovaInstalacao(sessao, idPedido, dataEntrega.Value, 2, false);

                    // Se tiver produtos comum, gera instalação comum
                    if (comum)
                        InstalacaoDAO.Instance.NovaInstalacao(sessao, idPedido, dataEntrega.Value, 1, false);

                    // Se não tiver nenhum dos três, gera instalação pelo tipo de entrega escolhido
                    if (!comum && !temperado && !entrega)
                    {
                        if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Comum)
                            InstalacaoDAO.Instance.NovaInstalacao(sessao, idPedido, dataEntrega.Value, 1, false);
                        else
                            InstalacaoDAO.Instance.NovaInstalacao(sessao, idPedido, dataEntrega.Value, 2, false);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao gerar instalações para o pedido", ex));
            }
        }

        #endregion

        #region Confirmar Pedido Liberação

        static volatile object _confirmarLiberacaoPedidoLock = new object();

        /// <summary>
        /// Confirma pedido, utilizado apenas na Tempera
        /// </summary>
        public void ConfirmarLiberacaoPedidoComTransacao(string idsPedidos, out string idsPedidosOk, out string idsPedidosErro, bool finalizando, bool financeiro)
        {
            lock (_confirmarLiberacaoPedidoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        ConfirmarLiberacaoPedido(transaction, idsPedidos, out idsPedidosOk, out idsPedidosErro, finalizando, financeiro);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (ValidacaoPedidoFinanceiroException f)
                    {
                        //Caso a Exceção contenha "Os demais pedidos foram confirmados com sucesso"
                        //Executa o commit dos pedidos que foram confirmados
                        if (f.Message.Contains("demais pedidos"))
                            transaction.Commit();
                        else
                            transaction.Rollback();

                        transaction.Close();
                        throw f;
                    }
                    catch (Exception ex)
                    {
                        //Caso a Exceção contenha "Os demais pedidos foram confirmados com sucesso"
                        //Executa o commit dos pedidos que foram confirmados
                        if (ex.Message.Contains("demais pedidos"))
                            transaction.Commit();
                        else
                            transaction.Rollback();

                        transaction.Close();
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Confirma pedido, utilizado apenas na Tempera
        /// </summary>
        private void ConfirmarLiberacaoPedido(GDASession session, string idsPedidos, bool finalizando)
        {
            string temp, temp1;
            ConfirmarLiberacaoPedido(session, idsPedidos, out temp, out temp1, finalizando, false);
        }

        /// <summary>
        /// Confirma pedido, utilizado apenas na Tempera
        /// </summary>
        public void ConfirmarLiberacaoPedido(GDASession sessao, string idsPedidos, out string idsPedidosOk, out string idsPedidosErro, bool financeiro)
        {
            ConfirmarLiberacaoPedido(sessao, idsPedidos, out idsPedidosOk, out idsPedidosErro, false, financeiro);
        }

        /// <summary>
        /// Confirma pedido, utilizado apenas na Tempera
        /// </summary>
        public void ConfirmarLiberacaoPedido(GDASession sessao, string idsPedidos, out string idsPedidosOk, out string idsPedidosErro, bool finalizando, bool financeiro)
        {
            try
            {
                uint tipoUsuario = UserInfo.GetUserInfo.TipoUsuario;
                bool confApenasMaoDeObra = false;

                if (!finalizando)
                {
                    if (UserInfo.GetUserInfo.IdCliente == null || UserInfo.GetUserInfo.IdCliente == 0)
                        if (!Config.PossuiPermissao(Config.FuncaoMenuPedido.ConfirmarPedidoLiberacao))
                        {
                            // Verifica se o usuário pode imprimir pedidos de mão de obra
                            if (Config.PossuiPermissao(Config.FuncaoMenuPCP.ImprimirEtiquetasMaoDeObra))
                                confApenasMaoDeObra = true;
                            else
                                throw new Exception("Apenas o Gerente pode confirmar pedidos.");
                        }
                }

                Pedido[] pedidos = GetByString(sessao, idsPedidos);

                string idsClientes = ",";
                foreach (Pedido p in pedidos)
                {
                    if (!finalizando)
                    {
                        // Se o pedido não estiver conferido ou não tiver produtos associados, não pode ser confirmado
                        if (p.Situacao == Pedido.SituacaoPedido.ConfirmadoLiberacao)
                            throw new Exception("Pedido '" + p.IdPedido + "' já confirmado.");
                        else if (p.Situacao == Pedido.SituacaoPedido.Confirmado)
                            throw new Exception("Pedido '" + p.IdPedido + "' já liberado.");
                        else if (p.Situacao != Pedido.SituacaoPedido.Conferido && p.Situacao != Pedido.SituacaoPedido.AguardandoConfirmacaoFinanceiro)
                            throw new Exception("O pedido '" + p.IdPedido + "' ainda não foi conferido, portanto não pode ser confirmado.");

                        if (ProdutosPedidoDAO.Instance.GetCount(sessao, p.IdPedido, 0, false, 0) < 1)
                            throw new Exception("O pedido '" + p.IdPedido + "' não pode ser confirmado por não haver nenhum Produto associado ao mesmo.");

                        if (!p.MaoDeObra && confApenasMaoDeObra)
                            throw new Exception("Você pode confirmar apenas pedidos de mão de obra.");
                    }

                    // Salva o id dos clientes para a consulta do limite
                    idsClientes += !idsClientes.Contains("," + p.IdCli + ",") ? p.IdCli + "," : "";
                }

                var consideraPedidoConferido = FinanceiroConfig.DebitosLimite.EmpresaConsideraPedidoConferidoLimite;
                var naoVerificaPedidoAVista = PedidoConfig.EmpresaPermiteFinalizarPedidoAVistaSemVerificarLimite;

                // Verifica se a empresa considera pedidos conferidos (todos ou apenas à vista) no limite do cliente
                if (!consideraPedidoConferido || naoVerificaPedidoAVista)
                {
                    foreach (var id in idsClientes.TrimStart(',').TrimEnd(',').Split(','))
                    {
                        // Se a empresa não considera pedidos conferidos no limite, soma o total de todos os pedidos sendo confirmados,
                        // mas caso a empresa apenas não verifique o limite ao finalizar pedido à vista, puxa o total de todos os pedidos
                        // sendo confirmados que forem à vista e que não foi recebido antecipado
                        var whereTotal = !consideraPedidoConferido ?
                            "idCli=" + id + " And idPedido In (" + idsPedidos + ")" :
                            "idCli=" + id + " And idPedido In (" + idsPedidos + ") And (Coalesce(idPagamentoAntecipado, 0)=0 Or Coalesce(valorPagamentoAntecipado, 0)=0)  And tipoVenda=" + (int)Pedido.TipoVendaPedido.AVista;

                        // Quando a empresa considera pedido conferido no limite, os débitos do pedido já são buscados no método GetDebitos, por isso foi removido dos totais e do pagoAntecipado.
                        // Recupera o valor de todos os pedidos do cliente que estão sendo confirmados
                        var totaisPedidos = PedidoDAO.Instance.ObtemValorCampo<decimal>(sessao, "Sum(total)", !consideraPedidoConferido ? whereTotal : whereTotal + " And Situacao != " + (int)Pedido.SituacaoPedido.Conferido);

                        // Total pago antecipado
                        var totalPagoAntecipado = PedidoDAO.Instance.ObtemValorCampo<decimal>(sessao, "Sum(Coalesce(valorPagamentoAntecipado,0))",
                            !consideraPedidoConferido ? whereTotal : whereTotal + " And Situacao != " + (int)Pedido.SituacaoPedido.Conferido);

                        // Recupera os débitos do cliente
                        var debitos = ContasReceberDAO.Instance.GetDebitos(sessao, id.StrParaUint(), null);

                        // Recupera o limite do cliente
                        var limite = ClienteDAO.Instance.ObtemValorCampo<decimal>(sessao, "limite", "id_cli=" + id, null);

                        // Verifica se o total dos pedidos mais o total de débitos ultrapassa o limite do cliente, se sim é lançada uma exceção
                        if (limite > 0 && (totaisPedidos + debitos - totalPagoAntecipado) > limite &&
                            /* Chamado 45457. */
                            totaisPedidos - totalPagoAntecipado > 0)
                        {
                            // Passa somente os pedidos do cliente desta iteração
                            var idsPedidoCliente = string.Join(",", ExecuteMultipleScalar<string>(sessao, "Select Cast(idPedido as char) From pedido Where " + whereTotal).ToArray());

                            if (!string.IsNullOrWhiteSpace(idsPedidoCliente) && !idsPedidoCliente.Contains(","))
                            {
                                //Cria um log no pedido caso ocorra problema com o limite do cliente
                                // Feito pois ao finalizar pedido de revenda não é lancada uma exceção, sendo assim será salvo um log no pedido.
                                var logConfirmacao = new LogAlteracao();
                                logConfirmacao.Tabela = (int)LogAlteracao.TabelaAlteracao.Pedido;
                                logConfirmacao.IdRegistroAlt = idsPedidoCliente.StrParaInt();
                                logConfirmacao.NumEvento = LogAlteracaoDAO.Instance.GetNumEvento(sessao, LogAlteracao.TabelaAlteracao.Pedido, idsPedidoCliente.StrParaInt());
                                logConfirmacao.Campo = "Falha ao finalizar/Confirmar Pedido";
                                logConfirmacao.DataAlt = DateTime.Now;
                                logConfirmacao.IdFuncAlt = UserInfo.GetUserInfo != null ? UserInfo.GetUserInfo.CodUser : 0;
                                logConfirmacao.ValorAnterior = null;
                                logConfirmacao.ValorAtual = @"O cliente '" + id + " - " + ClienteDAO.Instance.ObtemValorCampo<string>(sessao, "nome", "id_cli=" + id) +
                                    " não possui limite suficiente. " + "\nLimite disponível: R$ " + limite + "\nLimite necessário: R$ " + (totaisPedidos + debitos);
                                logConfirmacao.Referencia = LogAlteracao.GetReferencia(sessao, (int)LogAlteracao.TabelaAlteracao.Pedido, idsPedidoCliente.StrParaUint());

                                //Salva o log no pedido
                                LogAlteracaoDAO.Instance.Insert(sessao, logConfirmacao);
                            }
                            LancarExceptionValidacaoPedidoFinanceiro("O cliente '" + id + " - " + ClienteDAO.Instance.ObtemValorCampo<string>(sessao, "nome", "id_cli=" + id) +
                                " não possui limite suficiente. " + "\nLimite disponível: R$ " + limite + "\nLimite necessário: R$ " + (totaisPedidos + debitos),
                                !string.IsNullOrWhiteSpace(idsPedidoCliente) && !idsPedidoCliente.Contains(",") ? idsPedidoCliente.StrParaUint() : 0, false, idsPedidoCliente,
                                ObservacaoFinalizacaoFinanceiro.MotivoEnum.Confirmacao);
                        }
                    }
                }

                List<uint> idPedidoOk = new List<uint>(), idPedidoErro = new List<uint>();
                var mensagem = "";
                var situacaoCliente = ClienteDAO.Instance.GetSituacao(pedidos[0].IdCli);

                // Se, bloquear confirmação de pedido com sinal à receber.
                if (PedidoConfig.ImpedirConfirmacaoPedidoPagamento && idPedidoOk.Count == 0 && 
                    !VerificaSinalPagamentoReceber(sessao, pedidos, out mensagem, out idPedidoOk, out idPedidoErro))
                {
                    idsPedidosOk = "";
                    idsPedidosErro = idsPedidos;

                    LancarExceptionValidacaoPedidoFinanceiro(mensagem, !string.IsNullOrWhiteSpace(idsPedidos) && !idsPedidos.Contains(",") ? idsPedidos.StrParaUint() : 0, false, idsPedidos,
                        ObservacaoFinalizacaoFinanceiro.MotivoEnum.Confirmacao);

                    // Permite que os pedidos sejam liberados pelo funcionário do Financeiro
                    idPedidoOk.Clear();
                    idPedidoOk.AddRange(idPedidoErro);
                    idPedidoErro.Clear();
                }
                // Se, pedido gerado pelo Parceiro e cliente Inativo ou Bloqueado.
                else if (FinanceiroConfig.ClienteInativoBloqueadoEmitirPedidoComConfirmacaoPeloFinanceiro &&
                    pedidos[0].GeradoParceiro && (situacaoCliente == (int)SituacaoCliente.Inativo || situacaoCliente == (int)SituacaoCliente.Bloqueado))
                {
                    idsPedidosOk = "";
                    idsPedidosErro = idsPedidos;
                    mensagem = "Pedido emitido no e-commerce por cliente inativo ou bloqueado";
                    LancarExceptionValidacaoPedidoFinanceiro(mensagem, !string.IsNullOrWhiteSpace(idsPedidos) && !idsPedidos.Contains(",") ? idsPedidos.StrParaUint() : 0, false, idsPedidos,
                        ObservacaoFinalizacaoFinanceiro.MotivoEnum.Confirmacao);

                    // Permite que os pedidos sejam liberados pelo funcionário do Financeiro
                    idPedidoOk.Clear();
                    idPedidoOk.AddRange(idPedidoErro);
                    idPedidoErro.Clear();
                }

                if (idPedidoErro.Count > 0)
                {
                    idsPedidosOk = string.Join(",", Array.ConvertAll<uint, string>(idPedidoOk.ToArray(), x => x.ToString()));
                    idsPedidosErro = string.Join(",", Array.ConvertAll<uint, string>(idPedidoErro.ToArray(), x => x.ToString()));

                    pedidos = GetByString(sessao, idsPedidosOk);
                }
                else
                {
                    idsPedidosOk = idsPedidos;
                    idsPedidosErro = "";
                }

                var idsProdQtde = new Dictionary<int, float>();

                // Se houver alteração neste método alterar também na confirmação de garantia/reposição
                #region Coloca produtos na reserva no estoque da loja

                var produtosPedidosEstoque = new Dictionary<uint, Dictionary<uint, float>>();

                try
                {
                    foreach (var idProdPed in ProdutosPedidoDAO.Instance.ObtemIdsPedidoExcetoProducao(sessao, idsPedidosOk))
                    {
                        var idProd = ProdutosPedidoDAO.Instance.ObtemIdProd(sessao, idProdPed);
                        var totM = ProdutosPedidoDAO.Instance.ObtemTotM(sessao, idProdPed);
                        var qtde = ProdutosPedidoDAO.Instance.ObtemQtde(sessao, idProdPed);
                        var idPedido = ProdutosPedidoDAO.Instance.ObtemIdPedido(sessao, idProdPed);
                        var idLoja = PedidoDAO.Instance.ObtemIdLoja(sessao, idPedido);
                        var idGrupo = ProdutoDAO.Instance.ObtemIdGrupoProd(sessao, (int)idProd);
                        var idSubGrupo = ProdutoDAO.Instance.ObtemIdSubgrupoProd(sessao, (int)idProd);
                        var alturaProd = ProdutosPedidoDAO.Instance.ObterAltura(sessao, (uint)idProdPed);

                        float qtdProd = 0;

                        var tipoCalc = GrupoProdDAO.Instance.TipoCalculo(sessao, (int)idProd);

                        if (tipoCalc == (int)TipoCalculoGrupoProd.M2 || tipoCalc == (int)TipoCalculoGrupoProd.M2Direto)
                            qtdProd += totM;
                        else if (tipoCalc == (int)TipoCalculoGrupoProd.MLAL0 || tipoCalc == (int)TipoCalculoGrupoProd.MLAL05 ||
                            tipoCalc == (int)TipoCalculoGrupoProd.MLAL1 || tipoCalc == (int)TipoCalculoGrupoProd.MLAL6)
                            qtdProd += qtde * alturaProd;
                        else
                            qtdProd += qtde;

                        if (!produtosPedidosEstoque.ContainsKey(idPedido))
                        {
                            produtosPedidosEstoque.Add(idPedido, new Dictionary<uint, float> { { idProd, qtdProd } });
                        }
                        else if (!produtosPedidosEstoque[idPedido].ContainsKey(idProd))
                        {
                            produtosPedidosEstoque[idPedido].Add(idProd, qtdProd);
                        }
                        else
                        {
                            produtosPedidosEstoque[idPedido][idProd] += qtdProd;
                        }

                        //Verifica se o produto possui estoque para inserir na reserva 
                        if (GrupoProdDAO.Instance.BloquearEstoque(sessao, (int)idGrupo, (int)idSubGrupo))
                        {
                            var estoque = ProdutoLojaDAO.Instance.GetEstoque(sessao, idLoja, idProd, null, false, false, true);

                            if (estoque < produtosPedidosEstoque[idPedido][idProd])
                            {
                                var descricaoProd = ProdutoDAO.Instance.ObtemDescricao((int)idProd);
                                throw new Exception("O produto " + descricaoProd + " possui apenas " + estoque + " em estoque.");
                            }
                        }

                        // Salva produto e qtd de saída para executar apenas um sql de atualização de estoque
                        if (!idsProdQtde.ContainsKey((int)idProd))
                            idsProdQtde.Add((int)idProd, produtosPedidosEstoque[idPedido][idProd]);
                        else
                            idsProdQtde[(int)idProd] += produtosPedidosEstoque[idPedido][idProd];
                    }
                }
                catch(Exception e)
                {
                    throw new Exception("Falha ao colocar produtos na reserva. ", e);
                }

                #endregion

                string sql;

                #region Altera situação do pedido para Confirmado Liberacao e atualiza o saldo da obra

                try
                {
                    /* Chamado 37030. */
                    foreach (var id in idsPedidosOk.Split(','))
                    {
                        // Recupera o pedido
                        var ped = pedidos.Where(f => f.IdPedido == id.StrParaUint()).FirstOrDefault();

                        // Atualiza para confirmado PCP 
                        AlteraSituacao(sessao, id.StrParaUint(), Pedido.SituacaoPedido.ConfirmadoLiberacao);

                        // Chamado 59179: Atualiza o saldo da obra (Deve ser feito neste momento)
                        AtualizaSaldoObra(sessao, ped.IdPedido, null, ped.IdObra, ped.Total, ped.Total, true);

                        /* Chamado 53136. */
                        LogAlteracaoDAO.Instance.LogPedido(sessao, ped, GetElementByPrimaryKey(sessao, ped.IdPedido), LogAlteracaoDAO.SequenciaObjeto.Atual);
                    }

                    sql = string.Format("UPDATE pedido SET UsuConf={0}, DataConf=NOW(), TaxaFastDelivery=IF(FastDelivery, {1}, NULL) WHERE IdPedido IN ({2})",
                        "{0}", PedidoConfig.Pedido_FastDelivery.TaxaFastDelivery.ToString().Replace(",", "."), "{1}");

                    if (financeiro)
                    {
                        foreach (var idPedido in idsPedidos.Split(','))
                        {
                            // Salva o usuário atual que estiver confirmando o pedido (Chamado 12100)
                            var usuConf = UserInfo.GetUserInfo.CodUser;//ObtemValorCampo<uint>("idFuncConfirmarFinanc", "idPedido=" + idPedido);
                            objPersistence.ExecuteCommand(sessao, string.Format(sql, usuConf, idPedido));
                        }
                    }
                    else
                    {
                        // Confirma somente os pedidos que estiverem ok
                        objPersistence.ExecuteCommand(sessao, string.Format(sql, UserInfo.GetUserInfo.CodUser, idsPedidosOk));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao atualizar situação dos pedidos.", ex));
                }

                #endregion

                // Coloca produtos na reserva do estoque da loja (Deve ser feito depois de alterar a situação do pedido)
                if (idsProdQtde.Count > 0)
                {
                    var idsLojaReserva = new List<int>();

                    foreach (var pedido in pedidos)
                        if (!idsLojaReserva.Contains((int)pedido.IdLoja))
                            idsLojaReserva.Add((int)pedido.IdLoja);

                    foreach (var idLojaReserva in idsLojaReserva)
                        ProdutoLojaDAO.Instance.ColocarReserva(sessao, idLojaReserva, idsProdQtde, null, null, null, null, null,
                            string.Join(",", pedidos.Select(f => f.IdPedido).ToList()), null, "PedidoDAO - ConfirmarLiberacaoPedido");
                }

                if (!string.IsNullOrEmpty(mensagem))
                    LancarExceptionValidacaoPedidoFinanceiro(mensagem + "\n\nOs demais pedidos foram confirmados com sucesso.",
                        !string.IsNullOrWhiteSpace(idsPedidosErro) && !idsPedidosErro.Contains(",") ? idsPedidosErro.StrParaUint() : 0, false, idsPedidosErro,
                        ObservacaoFinalizacaoFinanceiro.MotivoEnum.Confirmacao);
            }
            catch (ValidacaoPedidoFinanceiroException f)
            {
                throw f;
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException(string.Format("PedidoDAO - ConfirmarLiberacaoPedido. Pedidos: {0}", idsPedidos), ex);
                throw ex;
            }
        }

        #endregion

        #region Reabrir Pedido

        public bool PodeReabrir(uint idPedido)
        {
            return PodeReabrir(null, idPedido);
        }

        public bool PodeReabrir(GDASession session, uint idPedido)
        {
            var valorPagtoAntecipado = ObtemValorCampo<decimal>(session, "valorPagamentoAntecipado", "idPedido=" + idPedido);
            var situacao = ObtemSituacao(session, idPedido);
            var geradoParceiro = IsGeradoParceiro(session, idPedido);
            var idCli = ObtemIdCliente(session, idPedido);
            var temEspelho = PedidoEspelhoDAO.Instance.ExisteEspelho(session, idPedido);
            var possuiObraAssociada = GetIdObra(session, idPedido) > 0;
            var tipoPedido = GetTipoPedido(session, idPedido);
            var importado = IsPedidoImportado(session, idPedido);
            var recebeuSinal = ObtemValorCampo<bool>(session, "IdSinal > 0", "idPedido=" + idPedido);

            return PodeReabrir(session, idPedido, valorPagtoAntecipado, situacao, geradoParceiro, idCli, temEspelho, possuiObraAssociada, tipoPedido, importado, recebeuSinal);
        }

        public bool PodeReabrir(uint idPedido, decimal valorPagtoAntecipado, Pedido.SituacaoPedido situacao,
            bool geradoParceiro, uint idCli, bool temEspelho, bool possuiObraAssociada, Pedido.TipoPedidoEnum tipoPedido,
            bool importado, bool recebeuSinal)
        {
            return PodeReabrir(null, idPedido, valorPagtoAntecipado, situacao, geradoParceiro, idCli, temEspelho, possuiObraAssociada, tipoPedido, importado, recebeuSinal);
        }

        public bool PodeReabrir(GDASession session, uint idPedido, decimal valorPagtoAntecipado, Pedido.SituacaoPedido situacao, bool geradoParceiro,
            uint idCli, bool temEspelho, bool possuiObraAssociada, Pedido.TipoPedidoEnum tipoPedido, bool importado, bool recebeuSinal)
        {
            // Define que apenas administrador pode reabrir pedido
            var apenasAdminReabrePedido = PCPConfig.ReabrirPCPSomenteAdmin;
            // Define que todos usuários podem reabrir pedido confirmado PCP, exceto o vendedor (a menos que seja pedido de revenda)
            var apenasVendedorNaoReabrePedidoConfirmadoPCP = PedidoConfig.ReabrirPedidoConfirmadoPCPTodosMenosVendedor;

            /* Chamado 52903. */
            if (geradoParceiro)
            {
                if (!PedidoConfig.ParceiroPodeReabrirPedidoConferido && idCli == UserInfo.GetUserInfo.IdCliente && situacao == Pedido.SituacaoPedido.Conferido)
                    return false;
                else if (!PedidoConfig.PodeReabrirPedidoGeradoParceiro && idCli != UserInfo.GetUserInfo.IdCliente)
                    return false;
            }

            // Não deixa reabrir se recebeu sinal
            if (PedidoConfig.ReabrirPedidoNaoPermitidoComSinalRecebido && recebeuSinal)
                return false;
            
            return (((valorPagtoAntecipado == 0 || PedidoConfig.ReabrirPedidoComPagamentoAntecipado ||
                /* Chamado 16956 e 17824. */
                (possuiObraAssociada && ObraDAO.Instance.ObtemSituacao(session, GetIdObra(session, idPedido).Value) != Obra.SituacaoObra.Finalizada)) &&
                ((situacao == Pedido.SituacaoPedido.ConfirmadoLiberacao && !temEspelho) || situacao == Pedido.SituacaoPedido.Conferido))
                && (!apenasAdminReabrePedido || UserInfo.GetUserInfo.IsAdministrador)
                && (!apenasVendedorNaoReabrePedidoConfirmadoPCP || UserInfo.GetUserInfo.TipoUsuario != (uint)Utils.TipoFuncionario.Vendedor ||
                    situacao == Pedido.SituacaoPedido.Conferido || tipoPedido == Pedido.TipoPedidoEnum.Revenda)
                && (!OrdemCargaConfig.UsarControleOrdemCarga || !PedidoOrdemCargaDAO.Instance.PedidoTemOC(session, idPedido)))
                && !(tipoPedido == Pedido.TipoPedidoEnum.Revenda && importado)
                && !(tipoPedido == Pedido.TipoPedidoEnum.Revenda && PedidoExportacaoDAO.Instance.GetSituacaoExportacao(session, idPedido) == PedidoExportacao.SituacaoExportacaoEnum.Exportado)
                && !(tipoPedido == Pedido.TipoPedidoEnum.Revenda && Instance.PossuiImpressaoBox(session, idPedido));
        }

        private static object _reabrirPedido = new object();

        /// <summary>
        /// Reabre um pedido.
        /// </summary>
        public void Reabrir(uint idPedido)
        {
            lock (_reabrirPedido)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        // #69907 - Altera a OBS do pedido para bloquear qualquer outra alteração na tabela fora dessa transação
                        var obsPedido = ObtemObs(transaction, idPedido);
                        objPersistence.ExecuteCommand(transaction, string.Format("UPDATE pedido SET obs='Reabrindo pedido' WHERE IdPedido={0}", idPedido));

                        if (!PodeReabrir(transaction, idPedido))
                            throw new Exception("Não é possível reabrir esse pedido.");

                        /* Chamado 33940. */
                        if (objPersistence.ExecuteSqlQueryCount(transaction,
                            string.Format("SELECT COUNT(*) FROM produto_pedido_producao WHERE IdPedidoExpedicao={0}", idPedido)) > 0)
                            throw new Exception("Este pedido não pode ser reaberto porque uma ou mais peças foram entregues, volte a situação delas na produção e tente novamente.");

                        if (TemVolume(transaction, idPedido))
                            throw new Exception("Não é possível reabrir esse pedido, pois o mesmo possui volumes gerados.");

                        var nova = Pedido.SituacaoPedido.Ativo;
                        if (PedidoConferenciaDAO.Instance.IsInConferencia(transaction, idPedido) || Instance.ObtemIdSinal(transaction, idPedido) > 0)
                            nova = Pedido.SituacaoPedido.AtivoConferencia;

                        var pedido = GetElementByPrimaryKey(transaction, idPedido);

                        if(pedido.GerarPedidoProducaoCorte && Instance.PedidoProducaoCorteAtivo(transaction, idPedido))
                            throw new Exception("Não é possível reabrir esse pedido, pois o mesmo possui pedidos de produção em andamento. Cancele-os para reabrir este pedido.");

                        var situacao = ObtemSituacao(transaction, idPedido);
                        Instance.AlteraSituacao(transaction, idPedido, nova);

                        objPersistence.ExecuteCommand(transaction, "update pedido set dataFin=null, usuFin=null where idPedido=" + idPedido);

                        //Verifica se o  ValorPagamentoAntecipado é proveniente de uma Obra, então zera.
                        if (pedido.IdObra.GetValueOrDefault() > 0 && pedido.IdPagamentoAntecipado.GetValueOrDefault() == 0)
                            objPersistence.ExecuteCommand(transaction, "UPDATE pedido SET ValorPagamentoAntecipado=null WHERE IdPedido=" + idPedido);

                        var produtos = ProdutosPedidoDAO.Instance.GetByPedidoLite(transaction, idPedido);

                        //Movimenta o estoque da materia-prima
                        foreach (var p in produtos)
                        {
                            if (ProdutoDAO.Instance.IsVidro(transaction, (int)p.IdProd))
                                MovMateriaPrimaDAO.Instance.MovimentaMateriaPrimaPedido(transaction, (int)p.IdProdPed, (decimal)p.TotM, MovEstoque.TipoMovEnum.Entrada);
                        }

                        // Tira os produtos da reserva, se o pedido estivesse confirmado
                        if (situacao == Pedido.SituacaoPedido.ConfirmadoLiberacao)
                        {
                            try
                            {
                                var login = UserInfo.GetUserInfo;

                                var idsProdQtde = new Dictionary<int, float>();

                                // Pedido de produção não deve tirar nem colocar na reserva
                                if (pedido.TipoPedido != (int)Pedido.TipoPedidoEnum.Producao)
                                {
                                    foreach (var pp in ProdutosPedidoDAO.Instance.GetByPedidoLite(transaction, idPedido))
                                    {
                                        var tipoCalc = GrupoProdDAO.Instance.TipoCalculo(transaction, (int)pp.IdGrupoProd,
                                            (int)pp.IdSubgrupoProd);
                                        var m2 = tipoCalc == (int)TipoCalculoGrupoProd.M2 ||
                                            tipoCalc == (int)TipoCalculoGrupoProd.M2Direto;

                                        var qtdEstornoEstoque = pp.Qtde;

                                        if (tipoCalc == (int)TipoCalculoGrupoProd.MLAL0 ||
                                            tipoCalc == (int)TipoCalculoGrupoProd.MLAL05 ||
                                            tipoCalc == (int)TipoCalculoGrupoProd.MLAL1 ||
                                            tipoCalc == (int)TipoCalculoGrupoProd.MLAL6)
                                        {
                                            var altura = ProdutosPedidoDAO.Instance.ObtemValorCampo<float>(transaction, "altura",
                                                "idProdPed=" + pp.IdProdPed);
                                            qtdEstornoEstoque = pp.Qtde * altura;
                                        }

                                        // Salva produto e qtd de saída para executar apenas um sql de atualização de estoque
                                        if (!idsProdQtde.ContainsKey((int)pp.IdProd))
                                            idsProdQtde.Add((int)pp.IdProd, m2 ? pp.TotM : qtdEstornoEstoque);
                                        else
                                            idsProdQtde[(int)pp.IdProd] += m2 ? pp.TotM : qtdEstornoEstoque;
                                    }
                                }

                                /* Chamado 17824. */
                                // Zera o campo pagamento antecipado
                                //objPersistence.ExecuteCommand("update pedido set valorPagamentoAntecipado=0, dataConf=null, usuConf=null where idPedido=" + idPedido);
                                objPersistence.ExecuteCommand(transaction,
                                    "update pedido set dataConf=null, usuConf=null where idPedido=" + idPedido);

                                var idObra = ObtemValorCampo<uint>(transaction, "idObra", "idPedido=" + idPedido);
                                if (idObra > 0)
                                    ObraDAO.Instance.AtualizaSaldo(transaction, idObra, false);

                                ProdutoLojaDAO.Instance.TirarReserva(transaction, (int)ObtemIdLoja(transaction, idPedido), idsProdQtde,
                                    null, null, null, null, (int)idPedido, null, null, "PedidoDAO - Reabrir");
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Falha ao tirar produtos na reserva.", ex);
                            }
                        }

                        LogAlteracaoDAO.Instance.LogPedido(transaction, pedido, GetElementByPrimaryKey(transaction, pedido.IdPedido),
                            LogAlteracaoDAO.SequenciaObjeto.Atual);

                        // #69907 - Ao final da transação volta a situação original do pedido
                        objPersistence.ExecuteCommand(transaction, string.Format("UPDATE pedido SET obs=?obs WHERE IdPedido={0}", idPedido), new GDAParameter("?obs", obsPedido));

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

        #endregion

        #region Confirmar Pedido Obra

        /// <summary>
        /// Confirma pedido de obra
        /// </summary>
        /// <param name="idPedido"></param>
        public void ConfirmarPedidoObra(uint idPedido, bool obraPagouPedidoInteiro)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    uint tipoUsuario = UserInfo.GetUserInfo.TipoUsuario;

                    bool cxDiario = false;

                    // Se a empresa tiver permissão para trabalhar com caixa diário
                    if (Geral.ControleCaixaDiario)
                    {
                        if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario))
                            throw new Exception("Apenas o Caixa pode confirmar pedidos.");

                        cxDiario = true;
                    }
                    else
                    {
                        if (!Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) &&
                            !Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario))
                            throw new Exception("Você não tem permissão para confirmar pedidos.");
                    }

                    Pedido pedido = GetElementByPrimaryKey(transaction, idPedido);

                    if (pedido.IdObra == null || pedido.IdObra == 0)
                        throw new Exception("Associe uma obra à este pedido antes de confirmá-lo.");

                    //Obra obra = ObraDAO.Instance.GetElementByPrimaryKey(pedido.IdObra.Value);

                    if (obraPagouPedidoInteiro)
                    {
                        // Se o pedido não estiver conferido ou não tiver produtos associados, não pode ser confirmado
                        if (pedido.Situacao == Pedido.SituacaoPedido.Confirmado)
                            throw new Exception("Pedido já liberado.");

                        else if (pedido.Situacao != Pedido.SituacaoPedido.Conferido)
                            throw new Exception("O pedido ainda não foi conferido, portanto não pode ser confirmado.");

                        else if (ProdutosPedidoDAO.Instance.GetCount(idPedido, 0, false, 0) < 1)
                            throw new Exception("O pedido não pode ser confirmado por não haver nenhum Produto associado ao mesmo.");
                    }

                    /*
                    // Verifica se o saldo da obra é suficiente para confirmar este pedido
                    if (pedido.Total > obra.Saldo)
                        throw new Exception("O saldo da obra não é suficiente para cobrir o valor do pedido.");
                    */

                    #region Altera situação do pedido para Confirmado

                    try
                    {
                        pedido.UsuConf = UserInfo.GetUserInfo.CodUser;
                        pedido.DataConf = DateTime.Now;
                        pedido.Situacao = Pedido.SituacaoPedido.Confirmado;

                        if (PedidoDAO.Instance.UpdateBase(transaction, pedido) == 0)
                            PedidoDAO.Instance.UpdateBase(transaction, pedido);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao atualizar situação do pedido.", ex));
                    }

                    #endregion

                    #region Gera Instalações para o pedido

                    try
                    {
                        int tipoEntrega = ObtemTipoEntrega(transaction, idPedido);

                        if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Comum || tipoEntrega == (int)Pedido.TipoEntregaPedido.Temperado ||
                            tipoEntrega == (int)Pedido.TipoEntregaPedido.ManutencaoTemperado || tipoEntrega == (int)Pedido.TipoEntregaPedido.Esquadria ||
                            tipoEntrega == (int)Pedido.TipoEntregaPedido.Entrega)
                        {
                            bool comum = ContemTipo(transaction, idPedido, 1);
                            bool temperado = ContemTipo(transaction, idPedido, 2);
                            bool entrega = false;

                            // Se o tipo de entrega for esquadria, gera instalação temperado
                            if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Esquadria)
                            {
                                comum = false;
                                temperado = true;
                            }

                            // Se o tipo de entrega for entrega, gera instalação Entrega
                            else if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Entrega)
                            {
                                comum = false;
                                temperado = false;
                                entrega = true;

                                InstalacaoDAO.Instance.NovaInstalacao(transaction, idPedido, pedido.DataEntrega.Value, 4, false);
                            }

                            uint idInstTemperado = 0;
                            uint idInstComum = 0;

                            // Se tiver produtos temperado, gera instalação temperado
                            if (temperado)
                                idInstTemperado = InstalacaoDAO.Instance.NovaInstalacao(transaction, idPedido, pedido.DataEntrega.Value, 2, false);

                            // Se tiver produtos comum, gera instalação comum
                            if (comum)
                                idInstComum = InstalacaoDAO.Instance.NovaInstalacao(transaction, idPedido, pedido.DataEntrega.Value, 1, false);

                            // Se não tiver nenhum dos três, gera instalação pelo tipo de entrega escolhido
                            if (!comum && !temperado && !entrega)
                            {
                                if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Comum)
                                    InstalacaoDAO.Instance.NovaInstalacao(transaction, idPedido, pedido.DataEntrega.Value, 1, false);
                                else
                                    InstalacaoDAO.Instance.NovaInstalacao(transaction, idPedido, pedido.DataEntrega.Value, 2, false);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao gerar instalações para o pedido", ex));
                    }

                    #endregion

                    // Atualiza o saldo da obra
                    ObraDAO.Instance.AtualizaSaldo(transaction, pedido.IdObra.Value, cxDiario);

                    transaction.Commit();
                    transaction.Close();
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                }
            }
        }

        #endregion

        #region Verifica se o pedido possui vidro temperado/comum

        /// <summary>
        /// Verifica se o pedido possui vidro comum ou vidro temperado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="tipo">1-Comum, 2-Temperado</param>
        private bool ContemTipo(GDASession sessao, uint idPedido, int tipo)
        {
            var sql = @"Select Count(*) From produtos_pedido pp 
                Left Join produto p On (pp.IdProd=p.IdProd) 
                Where idPedido=" + idPedido;

            var subgruposMarcadosFiltro = Glass.Data.DAL.GrupoProdDAO.Instance.ObtemSubgruposMarcadosFiltro(sessao, 0);

            if (!String.IsNullOrEmpty(subgruposMarcadosFiltro))
            {
                if (tipo == 1)
                    sql += @" And (p.IdGrupoProd=1 And p.IdSubgrupoProd Not In (" + subgruposMarcadosFiltro + @"))";
                else if (tipo == 2)
                    sql += @" And (p.IdGrupoProd=1 And p.IdSubgrupoProd In (" + subgruposMarcadosFiltro + "))";
            }
            else
                return false;

            return objPersistence.ExecuteSqlQueryCount(sessao, sql, null) > 0;
        }

        #endregion

        #region Verifica se o pedido está em conferência

        /// <summary>
        /// Verifica se o pedido está em conferência
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool EstaEmConferencia(uint idPedido)
        {
            string sql = "Select Count(*) From pedido_conferencia where idPedido=" + idPedido +
                " And Situacao<>" + (int)PedidoConferencia.SituacaoConferencia.Finalizada;

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sql).ToString()) > 0;
        }

        #endregion

        #region Confirma pedido de garantia e de reposição

        static volatile object _confirmaGarantiaReposicaoLock = new object();

        /// <summary>
        /// Confirma pedidos de garantia e de reposição
        /// </summary>
        public void ConfirmaGarantiaReposicaoComTransacao(uint idPedido, bool financeiro)
        {
            lock (_confirmaGarantiaReposicaoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        ConfirmaGarantiaReposicao(transaction, idPedido, financeiro);

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
        /// Confirma pedidos de garantia e de reposição
        /// </summary>
        public void ConfirmaGarantiaReposicao(GDASession session, uint idPedido, bool financeiro)
        {
            var tipoEntrega = ObtemTipoEntrega(session, idPedido);
            DateTime? dataEntrega = ObtemDataEntrega(session, idPedido);
            var pedidoAtual = GetElementByPrimaryKey(session, idPedido);

            if (PedidoConfig.LiberarPedido)
            {
                // Se for liberação e o pedido não possuir produtos do grupo vidro calculado por m², muda para Confirmado
                if (PedidoConfig.LiberarPedido && !ProdutosPedidoDAO.Instance.PossuiVidroCalcM2(session, idPedido))
                {
                    AlteraSituacao(session, idPedido, Pedido.SituacaoPedido.ConfirmadoLiberacao);

                    // TODO: Colocar função para recalcular a reserva
                }
                else
                {
                    AlteraSituacao(session, idPedido, Pedido.SituacaoPedido.Conferido);

                    // Salva a data e usuário de finalização
                    var usuConf = UserInfo.GetUserInfo.CodUser;

                    if (financeiro)
                        usuConf = ObtemValorCampo<uint>(session, "idFuncFinalizarFinanc", "idPedido=" + idPedido);

                    objPersistence.ExecuteCommand(session, "update pedido set dataFin=?data, usuFin=?usu where idPedido=" + idPedido,
                        new GDAParameter("?data", DateTime.Now), new GDAParameter("?usu", usuConf));
                }
            }
            else
            {
                if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Comum ||
                    tipoEntrega == (int)Pedido.TipoEntregaPedido.Temperado ||
                    tipoEntrega == (int)Pedido.TipoEntregaPedido.ManutencaoTemperado ||
                    tipoEntrega == (int)Pedido.TipoEntregaPedido.Esquadria)
                {
                    var comum = ContemTipo(session, idPedido, 1);
                    var temperado = ContemTipo(session, idPedido, 2);

                    // Se for esquadria de alumínio, gera instalação temperado
                    if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Esquadria)
                    {
                        comum = false;
                        temperado = true;
                    }

                    // Se tiver produtos comum, gera instalação comum
                    if (comum)
                        InstalacaoDAO.Instance.NovaInstalacao(session, idPedido, dataEntrega.Value, 1, false);

                    // Se tiver produtos temperado, gera instalação temperado
                    if (temperado)
                        InstalacaoDAO.Instance.NovaInstalacao(session, idPedido, dataEntrega.Value, 2, false);

                    // Se não tiver nenhum dos dois, gera instalação pelo tipo de entrega escolhido
                    if (!comum && !temperado)
                    {
                        if (tipoEntrega == (int)Pedido.TipoEntregaPedido.Comum)
                            InstalacaoDAO.Instance.NovaInstalacao(session, idPedido, dataEntrega.Value, 1, false);
                        else
                            InstalacaoDAO.Instance.NovaInstalacao(session, idPedido, dataEntrega.Value, 2, false);
                    }
                }

                AlteraSituacao(session, idPedido, Pedido.SituacaoPedido.Confirmado);

                // Confirma/Libera o pedido
                objPersistence.ExecuteCommand(session, string.Format("Update pedido Set UsuConf=UsuCad, DataConf=?data Where idPedido={0}",
                    idPedido), new GDAParameter("?data", DateTime.Now));
            }

            #region Coloca produtos na reserva no estoque da loja

            /* Chamado 39942. */
            var idsProdQtde = new Dictionary<int, float>();

            try
            {
                if (GetTipoPedido(session, idPedido) != Pedido.TipoPedidoEnum.Producao)
                {
                    foreach (var prodPed in ProdutosPedidoDAO.Instance.GetByPedidoLite(session, idPedido))
                    {
                        var idProd = ProdutosPedidoDAO.Instance.ObtemIdProd(session, prodPed.IdProdPed);
                        var totM = ProdutosPedidoDAO.Instance.ObtemTotM(session, prodPed.IdProdPed);
                        var qtde = ProdutosPedidoDAO.Instance.ObtemQtde(session, prodPed.IdProdPed);

                        var tipoCalc = GrupoProdDAO.Instance.TipoCalculo(session, (int)idProd);
                        var m2 = tipoCalc == (int)TipoCalculoGrupoProd.M2 ||
                            tipoCalc == (int)TipoCalculoGrupoProd.M2Direto;

                        var qtdEstornoEstoque = qtde;

                        if (tipoCalc == (int)TipoCalculoGrupoProd.MLAL0 || tipoCalc == (int)TipoCalculoGrupoProd.MLAL05 ||
                            tipoCalc == (int)TipoCalculoGrupoProd.MLAL1 || tipoCalc == (int)TipoCalculoGrupoProd.MLAL6)
                        {
                            var altura = ProdutosPedidoDAO.Instance.ObtemValorCampo<float>(session, "altura", "idProdPed=" + prodPed.IdProdPed);
                            qtdEstornoEstoque = qtde * altura;
                        }

                        // Salva produto e qtd de saída para executar apenas um sql de atualização de estoque
                        if (!idsProdQtde.ContainsKey((int)idProd))
                            idsProdQtde.Add((int)idProd, m2 ? totM : qtdEstornoEstoque);
                        else
                            idsProdQtde[(int)idProd] += m2 ? totM : qtdEstornoEstoque;
                    }

                    // Coloca produtos na reserva do estoque da loja (Deve ser feito depois de alterar a situação do pedido)
                    if (idsProdQtde.Count > 0)
                        ProdutoLojaDAO.Instance.ColocarReserva(session, (int)ObtemIdLoja(session, idPedido), idsProdQtde, null, null, null,
                            null, (int)idPedido, null, null, "PedidoDAO - ConfirmarGarantiaReposicao");
                }
            }
            catch
            {
                throw new Exception("Falha ao colocar produtos na reserva.");
            }

            #endregion

            LogAlteracaoDAO.Instance.LogPedido(session, pedidoAtual, GetElementByPrimaryKey(session, idPedido), LogAlteracaoDAO.SequenciaObjeto.Atual);
        }

        #endregion

        #region Altera situação do pedido

        public int AlteraSituacao(uint idPedido, Pedido.SituacaoPedido situacao)
        {
            return AlteraSituacao(null, idPedido, situacao);
        }

        public int AlteraSituacao(GDASession sessao, uint idPedido, Pedido.SituacaoPedido situacao)
        {
            return objPersistence.ExecuteCommand(sessao, "Update pedido Set Situacao=" + (int)situacao + " Where idPedido=" + idPedido);
        }

        public int AlteraSituacaoProducao(Pedido pedido)
        {
            Pedido ped = GetElementByPrimaryKey(pedido.IdPedido);

            int retorno = AlteraSituacaoProducao(pedido.IdPedido, (Pedido.SituacaoProducaoEnum)pedido.SituacaoProducao);

            switch ((Pedido.SituacaoProducaoEnum)pedido.SituacaoProducao)
            {
                case Pedido.SituacaoProducaoEnum.Pendente:
                    AtualizaSituacaoProducao(pedido.IdPedido, SituacaoProdutoProducao.Pendente, DateTime.Now);
                    break;

                case Pedido.SituacaoProducaoEnum.Pronto:
                    AtualizaSituacaoProducao(pedido.IdPedido, SituacaoProdutoProducao.Pronto, DateTime.Now);
                    break;

                case Pedido.SituacaoProducaoEnum.Entregue:
                    AtualizaSituacaoProducao(pedido.IdPedido, SituacaoProdutoProducao.Entregue, DateTime.Now);
                    break;
            }

            LogAlteracaoDAO.Instance.LogPedido(ped, GetElementByPrimaryKey(pedido.IdPedido), LogAlteracaoDAO.SequenciaObjeto.Atual);
            return retorno;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="situacaoProducao"></param>
        /// <returns></returns>
        public int AlteraSituacaoProducao(uint idPedido, Pedido.SituacaoProducaoEnum situacaoProducao)
        {
            return AlteraSituacaoProducao(null, idPedido, situacaoProducao);
        }

        public int AlteraSituacaoProducao(GDASession sessao, uint idPedido, Pedido.SituacaoProducaoEnum situacaoProducao)
        {
            return objPersistence.ExecuteCommand(sessao, "Update pedido Set SituacaoProducao=" + (int)situacaoProducao + " Where idPedido=" + idPedido);
        }

        #endregion

        #region Altera o Status de Liberação para Entrega - Financeiro

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="liberar"></param>
        /// <returns></returns>
        public int AlteraLiberarFinanc(uint idPedido, bool liberar)
        {
            return AlteraLiberarFinanc(null, idPedido, liberar);
        }

        public int AlteraLiberarFinanc(GDASession sessao, uint idPedido, bool liberar)
        {
            Pedido ped = GetElementByPrimaryKey(idPedido);
            objPersistence.ExecuteCommand(sessao, "Update pedido set liberadoFinanc=" + liberar + " Where idPedido=" + idPedido);

            LogAlteracaoDAO.Instance.LogPedido(sessao, ped, GetElementByPrimaryKey(idPedido), LogAlteracaoDAO.SequenciaObjeto.Atual);

            return 1;
        }

        #endregion

        #region Cancelar Pedido

        static volatile object _cancelarPedidoLock = new object();

        /// <summary>
        /// Cancela o pedido, apagando contas a pagar, contas a receber,
        /// estornando produtos que foi dado baixa.
        /// </summary>
        public void CancelarPedidoComTransacao(uint idPedido, string motivoCancelamento, bool gerarCredito, bool gerarDebitoComissao,
            DateTime dataEstornoBanco)
        {
            lock (_cancelarPedidoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        CancelarPedido(transaction, idPedido, motivoCancelamento, gerarCredito, gerarDebitoComissao, dataEstornoBanco);                                               
                       
                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao cancelar pedido.", ex));
                    }
                }
            }
        }

        /// <summary>
        /// Cancela o pedido, apagando contas a pagar, contas a receber,
        /// estornando produtos que foi dado baixa.
        /// </summary>
        public void CancelarPedido(GDASession session, uint idPedido, string motivoCancelamento, bool gerarCredito,
            bool gerarDebitoComissao, DateTime dataEstornoBanco)
        {
            Pedido ped = GetElementByPrimaryKey(session, idPedido);
            var gerarCreditoObra = false;

            // Verifica se o caixa diário não foi fechado no dia anterior
            if (gerarCredito && Geral.ControleCaixaDiario &&
                !CaixaDiarioDAO.Instance.CaixaFechadoDiaAnterior(session, ped.IdLoja))
                throw new Exception("O caixa não foi fechado no último dia de trabalho.");

            // Verifica se o pedido já foi cancelado
            if (ped.Situacao == Pedido.SituacaoPedido.Cancelado)
                throw new Exception("Este pedido já foi cancelado.");

            /* Chamado 63742. */
            if (!Config.PossuiPermissao(Config.FuncaoMenuPedido.CancelarPedido))
                throw new Exception("O usuário logado não possui permissão para cancelar pedidos.");

            // Verifica se há trocas/devoluções em aberto para esse pedido
            if (TrocaDevolucaoDAO.Instance.ExistsByPedido(session, idPedido))
                throw new Exception("Cancele as trocas/devoluções relacionadas a esse pedido antes de cancelá-lo.");

            // Verifica se existe um acerto associado à este pedido
            if (!gerarCredito && AcertoDAO.Instance.ExisteAcerto(session, idPedido))
                throw new Exception(
                    "Existe um acerto associado à este pedido, cancele-o antes de cancelar o pedido.");

            // Verifica se alguma parcela deste pedido já foi recebida
            if (!gerarCredito && ContasReceberDAO.Instance.ExisteRecebida(session, idPedido, false) &&
                (ped.TipoVenda != (int)Pedido.TipoVendaPedido.AVista || PedidoConfig.LiberarPedido))
                throw new Exception(
                    "Existe uma conta recebida associada à este pedido, cancele-a antes de cancelar o pedido.");

            // Verifica se há liberações ativas para este pedido
            if (!gerarCredito && !LiberarPedidoDAO.Instance.PodeCancelarPedido(session, idPedido))
                throw new Exception(
                    "Ainda há liberações ativas para esse pedido. Cancele-as antes de cancelar o pedido.");

            // Verifica se o caixa já foi fechado
            if (Geral.ControleCaixaDiario &&
                CaixaDiarioDAO.Instance.CaixaFechadoPedido(session, idPedido))
                throw new Exception("O caixa já foi fechado.");

            // Verifica se o pedido possui sinal pago
            if (ped.RecebeuSinal && ped.Situacao != Pedido.SituacaoPedido.Confirmado)
                throw new Exception("Cancele o sinal deste pedido antes de cancelar o mesmo.");

            // Verifica se o pedido possui pagamento antecipado
            if (ped.IdPagamentoAntecipado > 0)
                throw new Exception("Cancele o pagamento antecipado deste pedido antes de cancelar o mesmo.");

            // Se o pedido estiver em uma obra já finalizada, não permite o cancelamento deste
            if (!gerarCredito && ped.IdObra > 0 &&
                ObraDAO.Instance.ObtemValorCampo<int>(session, "situacao", "idObra=" + ped.IdObra) == (int)Obra.SituacaoObra.Finalizada)
                /* Chamado 52310. */
                gerarCreditoObra = true;
                //throw new Exception("Este pedido está associado à uma obra já finalizada, cancele-a antes de cancelar este pedido.");

            // Chamado 14793: Não permite cancelar pedidos que possuam conferência gerada
            if (PedidoConfig.LiberarPedido && PedidoEspelhoDAO.Instance.ExisteEspelho(session, idPedido))
                throw new Exception("Não é possível cancelar pedidos que possuam conferência gerada.");

            // Verifica se já existe alguma impressão para este pedido, se tiver, não permite cancelamento
            if (ped.Situacao != Pedido.SituacaoPedido.Ativo &&
                ped.TipoVenda != (int)Pedido.TipoVendaPedido.Reposição &&
                ped.TipoVenda != (int)Pedido.TipoVendaPedido.Garantia && !ped.VendidoFuncionario)
            {
                if (objPersistence.ExecuteSqlQueryCount(session, @"Select Count(*) From produto_impressao pi
                        inner join impressao_etiqueta ie on (pi.idImpressao=ie.idImpressao)
                        Where pi.idPedido=" + idPedido + " and !coalesce(pi.cancelado,false) and ie.situacao=" +
                                                                 (int)
                                                                     ImpressaoEtiqueta.SituacaoImpressaoEtiqueta
                                                                         .Ativa) > 0)
                {
                    throw new Exception("Este pedido não pode ser cancelado pois já está em produção.\\n" +
                                        "Contacte o responsável pelo PCP para cancelar a produção desse pedido para que você possa cancelá-lo.");
                }
            }

            if (
                objPersistence.ExecuteSqlQueryCount(session,
                    "Select Count(*) From produto_pedido_producao Where idPedidoExpedicao=" + idPedido) > 0)
                throw new Exception(
                    "Este pedido possui peças entregues na produção, volte a situação dessas peças antes de cancelá-lo.");

            if (
                objPersistence.ExecuteSqlQueryCount(session,
                    "Select Count(*) From produto_impressao Where idPedidoExpedicao=" + idPedido) > 0)
                throw new Exception("Não é possível cancelar esse pedido porque há peças entregues na produção.");

            //Valida se o pedido ja tem OC se tiver não pode cancelar
            if (PedidoOrdemCargaDAO.Instance.PedidoTemOC(session, idPedido))
                throw new Exception("Não é possível cancelar esse pedido porque há uma OC vinculada.");

            if (TemVolume(session, idPedido))
                throw new Exception("Não é possível cancelar esse pedido porque há volumes gerados.");

            // Verifica se há comissão paga para esse pedido
            string idsComissoes = ComissaoPedidoDAO.Instance.IdsComissoesPagasPedido(session, idPedido);
            if (!gerarDebitoComissao && !String.IsNullOrEmpty(idsComissoes))
                throw new ComissaoGeradaException(idsComissoes, true);

            // Verifica se há comissão gerada para esse pedido
            else if (!gerarDebitoComissao && String.IsNullOrEmpty(idsComissoes) &&
                     ComissaoPedidoDAO.Instance.TemComissao(session, idPedido))
                throw new Exception(
                    "Não é possível cancelar esse pedido porque há comissões não pagas geradas para ele.");

            // Gera os débitos de comissão, se necessário
            else if (gerarDebitoComissao && !String.IsNullOrEmpty(idsComissoes))
                DebitoComissaoDAO.Instance.GeraDebito(session, idPedido, idsComissoes);

            if (!PedidoConfig.LiberarPedido &&
                ExecuteScalar<bool>(session,
                    "Select Count(*)>0 From cheques Where idDeposito>0 And idPedido=" + idPedido))
                throw new Exception("Este pedido possui cheques que já foram depositados, cancele ou retifique o depósito antes de cancelá-lo.");

            //verifica se há pedido de corte ativo associado
            if (ped.GerarPedidoProducaoCorte &&
                objPersistence.ExecuteSqlQueryCount(session, string.Format("SELECT COUNT(*) FROM pedido WHERE  IdPedidoRevenda={0} AND Situacao<>{1}",
                    ped.IdPedido, (int)Pedido.SituacaoPedido.Cancelado)) > 0)
                throw new Exception("Este pedido possui pedidos de produção associados ativos, para cancelar este pedido antes você deve cancelar os pedidos de produção ativos associados a ele.");
            var situacaoPed = ped.Situacao;

            // Salva o motivo do cancelamento
            //Concatena o Obs do pedido com o motivo do cancelamento
            ped.Obs = !String.IsNullOrEmpty(ped.Obs) ? ped.Obs + " " + motivoCancelamento : motivoCancelamento;
            // Se o tamanho do campo Obs exceder 1000 caracteres, salva apenas os 1000 primeiros, descartando o restante
            ped.Obs = ped.Obs.Length > 1000 ? ped.Obs.Substring(0, 1000) : ped.Obs;

            if (ped.Situacao == Pedido.SituacaoPedido.Ativo ||
                ped.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição ||
                ped.TipoVenda == (int)Pedido.TipoVendaPedido.Garantia || ped.VendidoFuncionario)
            {
                if (!gerarCredito)
                {
                    // Exclui cheques relacionados em aberto à este pedido e cancela os demais
                    ChequesDAO.Instance.DeleteByPedido(session, idPedido);
                }

                if (!PedidoConfig.LiberarPedido && ped.VendidoFuncionario)
                    EstornaMovFunc(session, idPedido, ped.IdFuncVenda.Value);

                // Salva a situação do pedido
                Pedido.SituacaoPedido situacaoAtual = ped.Situacao;

                #region Altera situação para cancelado

                try
                {
                    ped.DataCanc = DateTime.Now;
                    ped.UsuCanc = UserInfo.GetUserInfo.CodUser;
                    ped.Situacao = Pedido.SituacaoPedido.Cancelado;

                    PedidoDAO.Instance.UpdateBase(session, ped);
                }
                catch (Exception ex)
                {
                    string msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;

                    throw new Exception("Falha ao atualizar situação do pedido. Erro: " + msg);
                }

                #endregion

                #region Estorna/Tira da reserva ou liberação produtos deste pedido

                if (situacaoAtual == Pedido.SituacaoPedido.Confirmado ||
                    situacaoAtual == Pedido.SituacaoPedido.ConfirmadoLiberacao ||
                    situacaoAtual == Pedido.SituacaoPedido.LiberadoParcialmente)
                {
                    var idsProdQtde = new Dictionary<int, float>();

                    // Tira produtos da reserva ou estorna se já tiver dado baixa
                    foreach (var p in ProdutosPedidoDAO.Instance.GetByPedidoLite(session, idPedido))
                    {
                        var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(session,
                            (int)p.IdProd);

                        var m2 = tipoCalculo == (int)TipoCalculoGrupoProd.M2 ||
                                  tipoCalculo == (int)TipoCalculoGrupoProd.M2Direto;

                        var m2Saida = CalculosFluxo.ArredondaM2(session, (int)p.Largura,
                            (int)p.Altura, p.QtdSaida, (int)p.IdProd, p.Redondo, 0,
                            tipoCalculo != (int)TipoCalculoGrupoProd.M2Direto);

                        var qtdSaida = p.Qtde - p.QtdSaida;
                        var qtdCreditoEstoque = p.QtdSaida;

                        if (tipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 ||
                            tipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 ||
                            tipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 ||
                            tipoCalculo == (int)TipoCalculoGrupoProd.MLAL6 ||
                            tipoCalculo == (int)TipoCalculoGrupoProd.ML)
                        {
                            qtdSaida *= p.Altura;
                            qtdCreditoEstoque *= p.Altura;
                        }

                        // Salva produto e qtd de saída para executar apenas um sql de atualização de estoque
                        if (!PedidoDAO.Instance.IsProducao(session, idPedido))
                        {
                            if (!idsProdQtde.ContainsKey((int)p.IdProd))
                                idsProdQtde.Add((int)p.IdProd, m2 ? p.TotM - m2Saida : qtdSaida);
                            else
                                idsProdQtde[(int)p.IdProd] += m2 ? p.TotM - m2Saida : qtdSaida;
                        }

                        if (p.QtdSaida > 0)
                            MovEstoqueDAO.Instance.CreditaEstoquePedido(session, p.IdProd, ped.IdLoja, idPedido,
                                p.IdProdPed,
                                (decimal)(m2 ? m2Saida : qtdCreditoEstoque), true);

                        if (situacaoAtual == Pedido.SituacaoPedido.Confirmado && ProdutoDAO.Instance.IsVidro(session, (int)p.IdProd))
                            MovMateriaPrimaDAO.Instance.MovimentaMateriaPrimaPedido(null, (int)p.IdProdPed, (decimal)p.TotM, MovEstoque.TipoMovEnum.Entrada);
                    }

                    if (situacaoAtual == Pedido.SituacaoPedido.Confirmado)
                    {
                        if (PedidoConfig.LiberarPedido)
                            ProdutoLojaDAO.Instance.TirarLiberacao(session, (int)ped.IdLoja, idsProdQtde, null, null, null, null,
                                (int)idPedido, null, null, "PedidoDAO - CancelarPedido");
                        else
                            ProdutoLojaDAO.Instance.TirarReserva(session, (int)ped.IdLoja, idsProdQtde, null, null, null, null,
                                (int)idPedido, null, null, "PedidoDAO - CancelarPedido");
                    }
                    else
                        ProdutoLojaDAO.Instance.TirarReserva(session, (int)ped.IdLoja, idsProdQtde, null, null, null, null,
                                (int)idPedido, null, null, "PedidoDAO - CancelarPedido");
                }

                #endregion

                #region Volta a situação das etiquetas da reposição (tira da perda)

                if (PedidoReposicaoDAO.Instance.IsPedidoReposicao(session, idPedido))
                {
                    foreach (ProdutosPedido pp in ProdutosPedidoDAO.Instance.GetByPedidoLite(session, idPedido))
                        if (!string.IsNullOrEmpty(pp.NumEtiquetaRepos))
                            ProdutoPedidoProducaoDAO.Instance.VoltarPerdaProducao(session, pp.NumEtiquetaRepos, false);
                }

                #endregion
            }
            else // Se o pedido estiver confirmado e for a vista ou a prazo ou obra
            {
                List<ProdutosPedido> lstProdPed = new List<ProdutosPedido>();

                if (!gerarCredito)
                {
                    // Realiza os estornos/cancelamentos financeiros
                    UtilsFinanceiro.CancelaRecebimento(session, UtilsFinanceiro.TipoReceb.PedidoAVista,
                        ped, null, null, null, null, 0,
                        null, null, null, null, dataEstornoBanco, false, false);
                }
                else
                {
                    // Deve ser chamado ao gerar crédito.
                    // Exclui contas a receber que podem ter sido geradas para este pedido e que ainda não foi paga
                    ContasReceberDAO.Instance.DeleteByPedido(session, idPedido);
                }

                // Salva a situação do pedido
                Pedido.SituacaoPedido situacaoAtual = ped.Situacao;

                #region Altera situação para cancelado e atualização observação

                try
                {
                    ped.DataCanc = DateTime.Now;
                    ped.UsuCanc = UserInfo.GetUserInfo.CodUser;
                    ped.Situacao = Pedido.SituacaoPedido.Cancelado;                    

                    PedidoDAO.Instance.UpdateBase(session, ped);
                }
                catch (Exception ex)
                {
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao atualizar situação do pedido.", ex));
                }

                #endregion

                #region Cancela o pedido espelho

                try
                {
                    if (PedidoEspelhoDAO.Instance.ExisteEspelho(session, idPedido))
                        objPersistence.ExecuteCommand(session,
                            "update pedido_espelho set situacao=" + (int)PedidoEspelho.SituacaoPedido.Cancelado +
                            " where idPedido=" + idPedido);
                }
                catch (Exception ex)
                {
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao atualizar situação do pedido espelho.", ex));
                }

                #endregion

                #region Estorna produtos/Tira da reserva ou liberação

                try
                {
                    if (situacaoAtual == Pedido.SituacaoPedido.Confirmado ||
                        situacaoAtual == Pedido.SituacaoPedido.ConfirmadoLiberacao ||
                        situacaoAtual == Pedido.SituacaoPedido.LiberadoParcialmente)
                    {
                        // Estorna produtos ao estoque da loja
                        lstProdPed =
                            new List<ProdutosPedido>(ProdutosPedidoDAO.Instance.GetByPedidoLite(session,
                                idPedido, true));

                        if (ped.TipoPedido == (int)Pedido.TipoPedidoEnum.Producao)
                        {
                            // Tira produtos do estoque
                            foreach (ProdutosPedido p in lstProdPed)
                            {
                                // Busca a quantidade que foi dado baixa deste produto no estoque
                                int qtdBaixa = objPersistence.ExecuteSqlQueryCount(session,
                                    @"Select Count(*) From produto_pedido_producao 
                                        Where idProdPed In (Select idProdPed from produtos_pedido_espelho Where idPedido=" +
                                    idPedido + @" 
                                        And idProd=" + p.IdProd +
                                    ") And idSetor in (select idSetor from setor where forno)");

                                bool m2 =
                                    Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session,
                                        (int)p.IdGrupoProd, (int)p.IdSubgrupoProd) ==
                                    (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 ||
                                    Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session,
                                        (int)p.IdGrupoProd, (int)p.IdSubgrupoProd) ==
                                    (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;

                                Single m2Saida = Glass.Global.CalculosFluxo.ArredondaM2(session, (int)p.Largura,
                                    (int)p.Altura, qtdBaixa, (int)p.IdProd, p.Redondo, 0,
                                    Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session,
                                        (int)p.IdGrupoProd, (int)p.IdSubgrupoProd) !=
                                    (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto);

                                float areaMinimaProd = ProdutoDAO.Instance.ObtemAreaMinima(session,
                                    (int)p.IdProd);

                                uint idCliente = PedidoDAO.Instance.ObtemIdCliente(session, idPedido);

                                float m2CalcAreaMinima = Glass.Global.CalculosFluxo.CalcM2Calculo(session,
                                    idCliente, (int)p.Altura, p.Largura,
                                    qtdBaixa, (int)p.IdProd, p.Redondo,
                                    p.Beneficiamentos.CountAreaMinimaSession(session), areaMinimaProd, false,
                                    p.Espessura, true);

                                MovEstoqueDAO.Instance.BaixaEstoquePedido(session, p.IdProd, ped.IdLoja,
                                    idPedido, p.IdProdPed,
                                    (decimal)(m2 ? m2Saida : qtdBaixa), (decimal)(m2 ? m2CalcAreaMinima : 0),
                                    false, null);

                                MovEstoqueDAO.Instance.CreditaEstoquePedido(session, p.IdProd, ped.IdLoja,
                                    idPedido, p.IdProdPed,
                                    (decimal)(m2 ? m2Saida : qtdBaixa), true);
                            }
                        }
                        else
                        {
                            var idsProdQtde = new Dictionary<int, float>();

                            // Tira produtos da reserva ou estorna se já tiver dado baixa
                            foreach (var p in lstProdPed)
                            {
                                var tipoCalculo = GrupoProdDAO.Instance.TipoCalculo(session,
                                    (int)p.IdProd);

                                var m2 = tipoCalculo == (int)TipoCalculoGrupoProd.M2 ||
                                          tipoCalculo == (int)TipoCalculoGrupoProd.M2Direto;

                                var m2Saida = CalculosFluxo.ArredondaM2(session, (int)p.Largura,
                                    (int)p.Altura, p.QtdSaida, (int)p.IdProd, p.Redondo, 0,
                                    tipoCalculo != (int)TipoCalculoGrupoProd.M2Direto);

                                var qtdSaida = p.Qtde - p.QtdSaida;
                                var qtdCreditoEstoque = p.QtdSaida;

                                if (tipoCalculo == (int)TipoCalculoGrupoProd.MLAL0 ||
                                    tipoCalculo == (int)TipoCalculoGrupoProd.MLAL05 ||
                                    tipoCalculo == (int)TipoCalculoGrupoProd.MLAL1 ||
                                    tipoCalculo == (int)TipoCalculoGrupoProd.MLAL6 ||
                                    tipoCalculo == (int)TipoCalculoGrupoProd.ML)
                                {
                                    qtdSaida *= p.Altura;
                                    qtdCreditoEstoque *= p.Altura;
                                }

                                // Salva produto e qtd de saída para executar apenas um sql de atualização de estoque
                                if (!idsProdQtde.ContainsKey((int)p.IdProd))
                                    idsProdQtde.Add((int)p.IdProd, m2 ? p.TotM - m2Saida : qtdSaida);
                                else
                                    idsProdQtde[(int)p.IdProd] += m2 ? p.TotM - m2Saida : qtdSaida;

                                if (p.QtdSaida > 0)
                                    MovEstoqueDAO.Instance.CreditaEstoquePedido(session, p.IdProd, ped.IdLoja,
                                        idPedido, p.IdProdPed,
                                        (decimal)(m2 ? m2Saida : qtdCreditoEstoque), true);
                            }

                            if (situacaoAtual == Pedido.SituacaoPedido.Confirmado)
                            {
                                if (PedidoConfig.LiberarPedido)
                                    ProdutoLojaDAO.Instance.TirarLiberacao(session, (int)ped.IdLoja, idsProdQtde, null, null, null, null,
                                        (int)idPedido, null, null, "PedidoDAO - CancelarPedido");
                                else
                                    ProdutoLojaDAO.Instance.TirarReserva(session, (int)ped.IdLoja, idsProdQtde, null, null, null, null,
                                        (int)idPedido, null, null, "PedidoDAO - CancelarPedido");
                            }
                            else
                                ProdutoLojaDAO.Instance.TirarReserva(session, (int)ped.IdLoja, idsProdQtde, null, null, null, null,
                                        (int)idPedido, null, null, "PedidoDAO - CancelarPedido");
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(MensagemAlerta.FormatErrorMsg("Falha ao estornar produtos ao estoque da loja.", ex));
                }

                #endregion

                // Exclui contas recebida gerada por este pedido se for à vista
                ContasReceberDAO.Instance.DeleteByPedidoAVista(session, idPedido);
            }

            #region Movimenta a materia-prima

            if (situacaoPed == Pedido.SituacaoPedido.Conferido || situacaoPed == Pedido.SituacaoPedido.AguardandoFinalizacaoFinanceiro
                || situacaoPed == Pedido.SituacaoPedido.AguardandoConfirmacaoFinanceiro)
            {
                foreach (var p in ProdutosPedidoDAO.Instance.GetByPedidoLite(idPedido, true))
                {
                    if (ProdutoDAO.Instance.IsVidro(null, (int)p.IdProd))
                        MovMateriaPrimaDAO.Instance.MovimentaMateriaPrimaPedido(null, (int)p.IdProdPed, (decimal)p.TotM, MovEstoque.TipoMovEnum.Entrada);
                }
            }

            #endregion

            if (gerarCredito || gerarCreditoObra)
            {
                #region Gera o crédito para o cliente

                /* Chamado 52310. */
                var totalUsar = gerarCreditoObra ? ped.ValorPagamentoAntecipado : ped.Total;

                if (Geral.ControleCaixaDiario)
                {
                    CaixaDiarioDAO.Instance.MovCxPedido(session, ped.IdLoja, ped.IdCli, ped.IdPedido, 1, totalUsar, 0,
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado), null, "Cancelamento do pedido", false);
                }
                else
                {
                    CaixaGeralDAO.Instance.MovCxPedido(session, ped.IdPedido, ped.IdCli,
                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado),
                        1, totalUsar, 0, null, "Cancelamento do pedido", false, null);
                }

                #endregion

                ClienteDAO.Instance.CreditaCredito(session, ped.IdCli, totalUsar);
            }

            #region Atualiza o saldo da obra do pedido

            if (ped.IdObra > 0)
                ObraDAO.Instance.AtualizaSaldo(session, ped.IdObra.Value, false);

            #endregion

            #region Reabre o orçamento e o projeto

            try
            {
                if (ped.IdOrcamento != null)
                {
                    objPersistence.ExecuteCommand(session,
                        "Update orcamento Set idPedidoGerado=null where idOrcamento=" + ped.IdOrcamento);

                    // Remove os IDs dos produtos do pedido dos produtos do orçamento
                    objPersistence.ExecuteCommand(session, @"
                            update produtos_orcamento po
                                left join (
                                    select idProdParent, count(*) as num
                                    from produtos_orcamento
                                    where idOrcamento=" + ped.IdOrcamento + @" and idProdParent is not null
                                    group by idProdParent
                                ) as pc on (po.idProd=pc.idProdParent)
                            set po.idProdPed=null
                            where po.idOrcamento=" + ped.IdOrcamento + @" and (po.idProdPed=0
                                or (po.idItemProjeto is null and (
                                    po.idProdPed in (select idProdPed from produtos_pedido where idPedido=" + idPedido +
                                                           @")
                                    or po.idProdPed not in (select idProdPed from produtos_pedido)
                                    or po.idProdPed in (select idProdPed from produtos_pedido where idPedido in (
                                        select idPedido from pedido where situacao=" +
                                                           (int)Pedido.SituacaoPedido.Cancelado + @"))
                                )) or ((po.idItemProjeto is not null or coalesce(pc.num, 0)=0) and (
                                    po.idProdPed in (select idAmbientePedido from ambiente_pedido where idPedido=" +
                                                           idPedido + @")
                                    or po.idProdPed not in (select idAmbientePedido from ambiente_pedido)
                                    or po.idProdPed in (select idAmbientePedido from ambiente_pedido where idPedido in (
                                        select idPedido from pedido where situacao=" +
                                                           (int)Pedido.SituacaoPedido.Cancelado + @"))
                                )))");

                    if (OrcamentoConfig.NegociarParcialmente)
                    {
                        int situacao =
                            !OrcamentoDAO.Instance.IsNegociadoParcialmente(session, ped.IdOrcamento.Value)
                                ? (int)Orcamento.SituacaoOrcamento.Negociado
                                : (int)Orcamento.SituacaoOrcamento.NegociadoParcialmente;

                        objPersistence.ExecuteCommand(session,
                            "update orcamento set situacao=" + situacao + " where idOrcamento=" +
                            ped.IdOrcamento.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao reabrir o orçamento.", ex));
            }

            try
            {
                if (ped.IdProjeto != null)
                    ProjetoDAO.Instance.ReabrirProjeto(session, ped.IdProjeto.Value);
            }
            catch (Exception ex)
            {
                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao reabrir o projeto.", ex));
            }

            #endregion

            #region Cancela Instalações

            // Cancela as instalações do pedido, se houver
            var lstInst = InstalacaoDAO.Instance.GetByPedido(session, ped.IdPedido).ToArray();

            // Se a instalação estiver aberta, cancela
            foreach (Instalacao inst in lstInst)
                if (inst.Situacao == 1)
                    InstalacaoDAO.Instance.Cancelar(session, inst.IdInstalacao);

            #endregion

            #region Marca etiquetas como canceladas

            objPersistence.ExecuteCommand(session, @"
                    update produto_pedido_producao ppp
                        inner join produtos_pedido_espelho ppe on (ppp.idProdPed=ppe.idProdPed)
                    set ppp.situacao=" + (!ped.MaoDeObra
                ? (int)ProdutoPedidoProducao.SituacaoEnum.CanceladaVenda
                : (int)ProdutoPedidoProducao.SituacaoEnum.CanceladaMaoObra) + " where ppe.idPedido=" + idPedido);

            // Altera a situação de produção do pedido para Etiqueta Não Impressa.
            objPersistence.ExecuteCommand(session,
                string.Format("UPDATE pedido p SET p.SituacaoProducao={0} WHERE p.IdPedido={1}",
                    (int)Pedido.SituacaoProducaoEnum.NaoEntregue, idPedido));

            #endregion

            LogCancelamentoDAO.Instance.LogPedido(session, ped,
                motivoCancelamento.Substring(motivoCancelamento.ToLower().IndexOf("motivo do cancelamento: ") +
                              "motivo do cancelamento: ".Length), true);
        }

        /// <summary>
        /// Estorna movimentações de funcionários
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="idFuncVenda"></param>
        internal void EstornaMovFunc(GDASession sessao, uint idPedido, uint idFuncVenda)
        {
            foreach (MovFunc m in MovFuncDAO.Instance.GetByPedido(sessao, idPedido))
            {
                if (m.TipoMov == (int)MovFunc.TipoMovEnum.Entrada)
                    break;

                // Recupera o plano de conta
                uint idConta = m.IdConta;

                // Recupera a lista de planos de conta de sinal, à vista e à prazo
                List<string> sinal = new List<string>(UtilsPlanoConta.ListaEstornosSinalPedido().Split(','));
                List<string> vista = new List<string>(UtilsPlanoConta.ListaEstornosAVista().Split(','));
                List<string> prazo = new List<string>(UtilsPlanoConta.ListaEstornosAPrazo().Split(','));

                // Recupera o plano de conta de estorno, se possível
                if (sinal.Contains(idConta.ToString()))
                    idConta = UtilsPlanoConta.EstornoSinalPedido(m.IdConta);
                else if (vista.Contains(idConta.ToString()))
                    idConta = UtilsPlanoConta.EstornoAVista(m.IdConta);
                else if (prazo.Contains(idConta.ToString()))
                    idConta = UtilsPlanoConta.EstornoAPrazo(m.IdConta);

                // Efetua o estorno
                MovFuncDAO.Instance.MovimentarPedido(sessao, idFuncVenda, idPedido, idConta, 1, m.ValorMov, null);
            }
        }

        #endregion

        #region Busca o percentual de desconto do pedido

        /// <summary>
        /// Busca o percentual de desconto do pedido
        /// </summary>
        public float GetPercDesc(uint idPedido)
        {
            return GetPercDesc(null, idPedido);
        }

        /// <summary>
        /// Busca o percentual de desconto do pedido
        /// </summary>
        public float GetPercDesc(GDASession session, uint idPedido)
        {
            string sql = "Select Coalesce(if(tipoDesconto=1, desconto/100, desconto/(total+desconto)), 0) From pedido Where idPedido=" + idPedido;
            return ExecuteScalar<float>(session, sql);
        }

        #endregion

        #region Verifica se o desconto do pedido está dentro do permitido

        /// <summary>
        /// Verifica se o desconto do pedido está dentro do permitido
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool DescontoPermitido(uint idPedido)
        {
            return DescontoPermitido(null, idPedido);
        }

        /// <summary>
        /// Verifica se o desconto do pedido está dentro do permitido
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool DescontoPermitido(GDASession sessao, uint idPedido)
        {
            string somaDesconto = "(select sum(coalesce(valorDescontoQtde,0)" + (PedidoConfig.RatearDescontoProdutos ? "+coalesce(valorDesconto,0)+coalesce(valorDescontoProd,0)" :
                "") + ") from produtos_pedido where idPedido=p.idPedido)";

            uint idFunc = UserInfo.GetUserInfo.CodUser;
            if (Geral.ManterDescontoAdministrador)
                idFunc = ObtemIdFuncDesc(sessao, idPedido).GetValueOrDefault(idFunc);

            if (idFunc == 0)
                idFunc = ObtemIdFunc(sessao, idPedido);

            float descontoMaximoPermitido = PedidoConfig.Desconto.GetDescontoMaximoPedido(sessao, idFunc, (int)GetTipoVenda(sessao, idPedido));

            if (descontoMaximoPermitido == 100)
                return true;

            if (FinanceiroConfig.UsarDescontoEmParcela)
            {
                var idParcela = ObtemIdParcela(sessao, idPedido);
                if (idParcela.GetValueOrDefault(0) > 0)
                {
                    var desconto = ParcelasDAO.Instance.ObtemDesconto(sessao, idParcela.Value);
                    if (desconto == ObtemDescontoCalculado(sessao, idPedido))
                        return true;
                }
            }
            else if (FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto)
            {
                uint tipoVenda = (uint)ObtemTipoVenda(sessao, idPedido);
                uint? idFormaPagto = ObtemFormaPagto(sessao, idPedido);
                uint? idTipoCartao = ObtemTipoCartao(sessao, idPedido);
                uint? idParcela = ObtemIdParcela(sessao, idPedido);
                uint? idGrupoProd = null;
                uint? idSubgrupoProd = null;

                var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedido(sessao, idPedido);
                if (produtosPedido != null && produtosPedido.Count > 0)
                {
                    idGrupoProd = produtosPedido[0].IdGrupoProd;
                    idSubgrupoProd = produtosPedido[0].IdSubgrupoProd;
                }

                var desconto = DescontoFormaPagamentoDadosProdutoDAO.Instance.ObterDesconto(tipoVenda, idFormaPagto, idTipoCartao, idParcela, idGrupoProd, idSubgrupoProd);
                if (desconto == ObtemDescontoCalculado(sessao, idPedido))
                    return true;
            }

            string sql = "Select Count(*) from pedido p Where idPedido=" + idPedido + @" And (
                (tipoDesconto=1 And desconto<=" + descontoMaximoPermitido.ToString().Replace(",", ".") + @") Or
                (tipoDesconto=2 And Coalesce(round(desconto/(total+" + somaDesconto + (!PedidoConfig.RatearDescontoProdutos ? "+desconto" : "") + "),2),0)<=(" +
                descontoMaximoPermitido.ToString().Replace(",", ".") + @"/100))
            )";

            return ExecuteScalar<int>(sessao, sql) > 0;
        }

        private void RemoveDescontoNaoPermitido(uint idPedido)
        {
            RemoveDescontoNaoPermitido(null, idPedido);
        }

        private void RemoveDescontoNaoPermitido(GDASession sessao, uint idPedido)
        {
            // Chamado 23794: Caso passasse neste método, a busca por ambientes abaixo buscaria todos os ambientes e atualizaria todos os pedidos no sistema,
            // caso algum possuísse desconto acima do permitido, o mesmo seria desfeito.
            if (idPedido == 0)
                return;

            var pedido = GetElement(sessao, idPedido);

            if (pedido == null)
                return;

            // Remove o desconto dos produtos
            RemoveDesconto(sessao, idPedido);
            foreach (AmbientePedido ambiente in AmbientePedidoDAO.Instance.GetByPedido(sessao, idPedido))
                AmbientePedidoDAO.Instance.RemoveDesconto(sessao, ambiente.IdAmbientePedido);

            objPersistence.ExecuteCommand(sessao, @"
                Update pedido set desconto=0 
                Where idPedido=" + idPedido + @";
                Update pedido p set Total=Round((   
                    Select Sum(Total + coalesce(valorBenef, 0)) 
                    From produtos_pedido 
                    Where IdPedido=p.IdPedido 
                        And (InvisivelPedido = false or InvisivelPedido is null)), 2) 
                Where p.IdPedido=" + idPedido);

            // Chamado 21923: Não deve salvar log se pedido já estiver liberado, pois a alteração de desconto não será salva.
            if (pedido.Situacao != Pedido.SituacaoPedido.Confirmado)
            {
                Erro novo = new Erro();
                novo.UrlErro = "Desconto Pedido " + idPedido;
                novo.DataErro = DateTime.Now;
                novo.IdFuncErro = UserInfo.GetUserInfo.CodUser;
                novo.Mensagem = "Removido desconto do pedido " + idPedido;

                ErroDAO.Instance.Insert(novo);
            }
        }

        #endregion

        #region Fast Delivery

        public bool IsFastDelivery(uint idPedido)
        {
            return IsFastDelivery(null, idPedido);
        }

        public bool IsFastDelivery(GDASession sessao, uint idPedido)
        {
            string sql = "select Coalesce(FastDelivery, 0) from pedido where idPedido=" + idPedido;
            int retorno = Convert.ToInt32(objPersistence.ExecuteScalar(sessao, sql));
            return retorno == 1;
        }

        #endregion

        #region Verifica o tipo do pedido

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public Pedido.TipoPedidoEnum GetTipoPedido(uint idPedido)
        {
            return GetTipoPedido(null, idPedido);
        }

        public Pedido.TipoPedidoEnum GetTipoPedido(GDASession sessao, uint idPedido)
        {
            return ObtemValorCampo<Pedido.TipoPedidoEnum>(sessao, "tipoPedido", "idPedido=" + idPedido);
        }

        #region Verifica se pedido é Mão de Obra

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se pedido é mão de obra
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsMaoDeObra(uint idPedido)
        {
            return IsMaoDeObra(null, idPedido);
        }

        /// <summary>
        /// Verifica se pedido é mão de obra
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsMaoDeObra(GDASession sessao, uint idPedido)
        {
            return GetTipoPedido(sessao, idPedido) == Pedido.TipoPedidoEnum.MaoDeObra;
        }

        #endregion

        #region Verifica se pedido é Produção
        
        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se pedido é Produção
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsProducao(uint idPedido)
        {
            return IsProducao(null, idPedido);
        }

        /// <summary>
        /// Verifica se pedido é Produção
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsProducao(GDASession sessao, uint idPedido)
        {
            return GetTipoPedido(sessao, idPedido) == Pedido.TipoPedidoEnum.Producao;
        }

        #endregion

        #region Verifica se pedido é Venda
        
        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verific se pedido é Venda.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsVenda(uint idPedido)
        {
            return IsVenda(null, idPedido);
        }

        /// <summary>
        /// Verific se pedido é Venda.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsVenda(GDASession sessao, uint idPedido)
        {
            return !IsMaoDeObra(sessao, idPedido) && !IsProducao(sessao, idPedido);
        }

        #endregion

        #region Verifica se o pedido é do tipo Revenda

        /// <summary>
        /// Verifica se o pedido é do tipo Revenda.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsRevenda(uint idPedido)
        {
            return GetTipoPedido(idPedido) == Pedido.TipoPedidoEnum.Revenda;
        }

        #endregion

        #region Verifica se pedido é Mão de Obra Especial

        /// <summary>
        /// Verifica se pedido é mão de obra especial
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool IsMaoDeObraEspecial(uint idPedido)
        {
            return GetTipoPedido(idPedido) == Pedido.TipoPedidoEnum.MaoDeObraEspecial;
        }

        #endregion

        #region Verifica se os pedidos são de loja diferente

        /// <summary>
        /// Verifica se os pedidos são de loja diferente
        /// </summary>
        public bool PedidosLojasDiferentes(string idsPedidos)
        {
            var sql = string.Format("Select IdLoja from pedido where idPedido IN({0}) group by idloja", idsPedidos);
            return this.CurrentPersistenceObject.LoadResult(sql, null).Count() > 1;
        }

        #endregion

        #endregion

        #region Descrição do tipo de Entrega

        public string DescrTipoEntrega(int tipoEntrega)
        {
            switch (tipoEntrega)
            {
                case 1:
                    return "Balcão";
                case 2:
                    return "Colocação Comum";
                case 3:
                    return "Colocação Temperado";
                case 4:
                    return "Entrega";
                case 5:
                    return "Manutenção Temperado";
                case 6:
                    return "Colocação Esquadria";
                default:
                    return String.Empty;
            }
        }

        #endregion

        #region Verifica se o pedido possui vidros para produção/estoque

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se o pedido possui vidros para produção
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiVidrosProducao(uint idPedido)
        {
            return PossuiVidrosProducao(null, idPedido);
        }

        /// <summary>
        /// Verifica se o pedido possui vidros para produção
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiVidrosProducao(GDASession sessao, uint idPedido)
        {
            if (IsMaoDeObra(sessao, idPedido))
                return true;

            var sql = @"
                SELECT COUNT(*) FROM produtos_pedido pp 
                    INNER JOIN produto p On (pp.IdProd=p.IdProd)
                WHERE pp.IdPedido=" + idPedido + @"
                    AND p.IdGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @"  
                    AND (p.IdSubgrupoProd IN (
                        SELECT sgp.IdSubgrupoProd From subgrupo_prod sgp Where sgp.IdGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @" 
                            AND (sgp.ProdutosEstoque=FALSE OR sgp.ProdutosEstoque IS NULL))" +
                /* Chamado 16470.
                 * Produtos sem associação de subgrupo não estavam sendo considerados como vidros de produção,
                 * por isso, colocamos uma condição que irá verificar se o produto não tem subgrupo. */
                " OR COALESCE(p.IdSubgrupoProd, 0)=0)";
            /*string sql = @"
                Select Count(*) From produtos_pedido pp 
                    Inner Join produto p On (pp.idProd=p.idProd)
                Where pp.idPedido=" + idPedido + @"
                    And p.idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @"  
                    And p.idSubgrupoProd In (
                        Select idSubgrupoProd From subgrupo_prod Where idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @" 
                            And (produtosEstoque=false or produtosEstoque is null)
                    )";*/

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        /// <summary>
        /// Verifica se o pedido possui volumes
        /// </summary>
        public bool PossuiVolume(GDASession session, uint idPedido)
        {
            string sql = @"
                SELECT COUNT(*) 
                FROM produtos_pedido pp 
                    INNER JOIN produto p On (pp.idProd=p.idProd)
                    LEFT JOIN grupo_prod gp ON (p.idGrupoProd = gp.idGrupoProd)
                    LEFT JOIN subgrupo_prod sgp ON (p.idSubgrupoProd = sgp.idSubgrupoProd)
                WHERE pp.idPedido=" + idPedido + @"
                    AND COALESCE(sgp.geraVolume, gp.geraVolume, false) = true
                    AND COALESCE(pp.invisivelFluxo, false) = false";

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        /// <summary>
        /// Retorna a quantidade de vidros para retirada no estoque
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int ObtemQtdVidrosProducao(GDASession session, uint idPedido)
        {
            string sql = @"
                Select Sum(Qtde) From produtos_pedido pp 
                    Inner Join produto p On (pp.idProd=p.idProd)
                Where pp.idPedido=" + idPedido + @" and Coalesce(invisivelFluxo,false)=false
                    And p.idGrupoProd=1 
                    And p.idSubgrupoProd In (
                        Select idSubgrupoProd From subgrupo_prod Where idGrupoProd=" + (int)Glass.Data.Model.NomeGrupoProd.Vidro + @" 
                            And produtosEstoque=true
                    )";

            return ExecuteScalar<int>(session, sql);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se o pedido possui vidros para retirada no estoque
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiVidrosEstoque(uint idPedido)
        {
            return PossuiVidrosEstoque(null, idPedido);
        }

        /// <summary>
        /// Verifica se o pedido possui vidros para retirada no estoque
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiVidrosEstoque(GDASession sessao, uint idPedido)
        {
            if (IsMaoDeObra(sessao, idPedido))
                return true;

            return ObtemQtdVidrosProducao(sessao, idPedido) > 0;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Verifica se existe algum produto no pedido passado que ainda não foi marcada saída
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiProdutosPendentesSaida(uint idPedido)
        {
            return PossuiProdutosPendentesSaida(null, idPedido);
        }

        /// <summary>
        /// Verifica se existe algum produto no pedido passado que ainda não foi marcada saída
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PossuiProdutosPendentesSaida(GDASession sessao, uint idPedido)
        {
            string sql = @"
                Select Count(*) From produtos_pedido pp 
                Where pp.idPedido=" + idPedido + @"
                    And pp.qtde<>Coalesce(pp.qtdSaida, 0)";

            return objPersistence.ExecuteSqlQueryCount(sessao, sql) > 0;
        }

        #endregion

        #region Produção

        #region Pedido único para Corte

        /// <summary>
        /// Retorna o pedido para corte, se puder ser retornado
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public Pedido GetForCorte(uint idPedido, int situacao)
        {
            if (idPedido == 0 || situacao == 0)
                return null;
            else if (!PedidoExists(idPedido))
                throw new Exception("Não foi encontrado nenhum pedido com o número informado.");
            else
            {
                bool existsPedidoCorte = PedidoCorteDAO.Instance.ExistsByPedido(idPedido);

                if (situacao == (int)PedidoCorte.SituacaoEnum.Producao)
                {
                    // Se já existir um pedido corte para este pedido, verifica sua situação
                    if (existsPedidoCorte)
                    {
                        PedidoCorte pedidoCorte = PedidoCorteDAO.Instance.GetByIdPedido(idPedido);

                        switch (pedidoCorte.Situacao)
                        {
                            case (int)PedidoCorte.SituacaoEnum.Producao:
                                throw new Exception("Este Pedido já está em Produção.");
                            case (int)PedidoCorte.SituacaoEnum.Pronto:
                                throw new Exception("Este Pedido já está Pronto.");
                            case (int)PedidoCorte.SituacaoEnum.Entregue:
                                throw new Exception("Este Pedido já está Entregue.");
                        }
                    }
                }
                else if (situacao == (int)PedidoCorte.SituacaoEnum.Pronto)
                {
                    // Se não existir um pedido corte para este pedido
                    if (!existsPedidoCorte)
                        throw new Exception("Este pedido ainda não entrou em Produção.");

                    PedidoCorte pedidoCorte = PedidoCorteDAO.Instance.GetByIdPedido(idPedido);

                    switch (pedidoCorte.Situacao)
                    {
                        case (int)PedidoCorte.SituacaoEnum.Confirmado:
                            throw new Exception("Este Pedido ainda não entrou em Produção.");
                        case (int)PedidoCorte.SituacaoEnum.Pronto:
                            throw new Exception("Este Pedido já está Pronto.");
                        case (int)PedidoCorte.SituacaoEnum.Entregue:
                            throw new Exception("Este Pedido já está Entregue.");
                    }
                }
                else if (situacao == (int)PedidoCorte.SituacaoEnum.Entregue)
                {
                    // Se não existir um pedido corte para este pedido
                    if (!existsPedidoCorte)
                        throw new Exception("Este pedido ainda não entrou em Produção.");

                    PedidoCorte pedidoCorte = PedidoCorteDAO.Instance.GetByIdPedido(idPedido);

                    switch (pedidoCorte.Situacao)
                    {
                        case (int)PedidoCorte.SituacaoEnum.Confirmado:
                            throw new Exception("Este Pedido ainda não entrou em Produção.");
                        case (int)PedidoCorte.SituacaoEnum.Producao:
                            throw new Exception("Este Pedido ainda está em Produção, precisa estar Pronto para ser Entregue.");
                        case (int)PedidoCorte.SituacaoEnum.Entregue:
                            throw new Exception("Este Pedido já está Entregue.");
                    }
                }
            }

            bool temFiltro;
            string filtroAdicional;

            Pedido pedido = objPersistence.LoadOneData(Sql(idPedido, 0, null, null, 0, 0, null, 0, null, 0, null, null, null, null,
                String.Empty, String.Empty, String.Empty, String.Empty, String.Empty, null, null, null, null, null, null, 0, false,
                false, 0, 0, 0, 0, 0, null, 0, 0, 0, "", true, out filtroAdicional, out temFiltro).Replace("?filtroAdicional?", filtroAdicional));

            if (pedido.Situacao == Pedido.SituacaoPedido.Cancelado)
                throw new Exception("Este pedido foi cancelado.");

            #region Busca as parcelas do pedido

            var lstParc = ParcelasPedidoDAO.Instance.GetByPedido(idPedido).ToArray();

            string parcelas = lstParc.Length + " vez(es): ";

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

        #endregion

        #region Busca Pedidos pela situação do corte

        private string SqlCorte(uint idPedido, string dataIni, string dataFim, int situacao, bool selecionar,
            out bool temFiltro, out string dataPesq)
        {
            temFiltro = false;

            // Data que será utilizada para pesquisar e ordenar
            dataPesq = String.Empty;

            string campos = selecionar ? "p.*, pc.Situacao as SitProducao, " + ClienteDAO.Instance.GetNomeCliente("c") + @" as NomeCliente, '$$$' as Criterio" : "Count(*)";

            string criterio = String.Empty;

            string sql = "Select " + campos + @" From pedido p 
                Left Join cliente c On (p.IdCli=c.Id_Cli) 
                Inner Join pedido_corte pc On (p.IdPedido=pc.IdPedido) 
                Where 1 ";

            // Se nenhuma situação tiver sido especificada, não retorna nada
            if (situacao == 0)
            {
                temFiltro = true;
                return sql += " And 0>1";
            }
            else
            {
                sql += " And pc.Situacao=" + situacao;
                temFiltro = true;
            }

            LoginUsuario login = UserInfo.GetUserInfo;

            // Se a situação for 2 (Producao) ou 3 (Pronto) e não for gerente, busca so os pedidos do funcionário logado
            if (situacao == 2 || situacao == 3)
            {
                sql += " And pc.IdFuncProducao=" + login.CodUser;
                criterio += "Funcionário: " + FuncionarioDAO.Instance.GetNome(login.CodUser) + "    ";
                temFiltro = true;
            }

            // Descrição da situação que será filtrada por data
            string filtroData = String.Empty;

            // Verifica por qual Data será pesquisado (Producao, Pronto, Entregue)
            switch (situacao)
            {
                case 2:
                    dataPesq = "pc.DataProducao";
                    filtroData = "Produção"; break;
                case 3:
                    dataPesq = "pc.DataPronto";
                    filtroData = "Pronto"; break;
                case 4:
                    dataPesq = "pc.DataEntregue";
                    filtroData = "Entregue"; break;
            }

            if (idPedido > 0)
            {
                sql += " And p.idPedido=" + idPedido;
                criterio += "Pedido: " + idPedido + "    ";
                temFiltro = true;
            }
            else
            {
                if (!String.IsNullOrEmpty(dataIni))
                {
                    sql += " And " + dataPesq + ">=?dataIni";
                    criterio += "Data Início (" + filtroData + "): " + dataIni + "    ";
                    temFiltro = true;
                }

                if (!String.IsNullOrEmpty(dataFim))
                {
                    sql += " And " + dataPesq + "<=?dataFim";
                    criterio += "Data Fim (" + filtroData + "): " + dataFim + "    ";
                    temFiltro = true;
                }
            }

            sql = sql.Replace("$$$", criterio);

            return sql; // dataPesq != String.Empty ? (sql + " Order By " + dataPesq) : sql;
        }

        public Pedido[] GetForCorteRpt(uint idPedido, string dataIni, string dataFim, int situacao)
        {
            bool temFiltro;
            string dataPesq;
            return objPersistence.LoadData(SqlCorte(idPedido, dataIni, dataFim, situacao, true, out temFiltro, out dataPesq),
                GetParamCorte(dataIni, dataFim)).ToArray();
        }

        public IList<Pedido> GetForCorte(uint idPedido, string dataIni, string dataFim, int situacao, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string dataPesq;

            string sql = SqlCorte(idPedido, dataIni, dataFim, situacao, true, out temFiltro, out dataPesq);
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : dataPesq;

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, GetParamCorte(dataIni, dataFim));
        }

        public int GetCountCorte(uint idPedido, string dataIni, string dataFim, int situacao)
        {
            bool temFiltro;
            string dataPesq;

            string sql = SqlCorte(idPedido, dataIni, dataFim, situacao, true, out temFiltro, out dataPesq);

            return GetCountWithInfoPaging(sql, temFiltro, GetParamCorte(dataIni, dataFim));
        }

        private GDAParameter[] GetParamCorte(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Listagem de Pedidos de Corte Padrão

        private string SqlListCorte(uint idPedido, uint idCli, string nomeCli, bool selecionar, out bool temFiltro)
        {
            temFiltro = false;
            string campos = selecionar ? "p.*, " + ClienteDAO.Instance.GetNomeCliente("c") + @" as NomeCliente, c.Revenda as CliRevenda, f.Nome as NomeFunc, 
                pc.DataProducao, pc.DataEntregue, pc.DataPronto as DataProntoCorte, fprod.Nome as FuncProd, fe.Nome as FuncEntregue,  
                pc.Situacao as SitProducao, l.NomeFantasia as nomeLoja, fp.Descricao as FormaPagto" : "Count(*)";

            string sql = @"
                Select " + campos + @" From pedido p 
                Inner Join cliente c On (p.idCli=c.id_Cli) 
                Inner Join funcionario f On (p.IdFunc=f.IdFunc) 
                Inner Join loja l On (p.IdLoja = l.IdLoja) 
                Inner Join pedido_corte pc On (p.idPedido=pc.idPedido) 
                Left Join funcionario fprod On (pc.idFuncProducao=fprod.idFunc) 
                Left Join funcionario fe On (pc.idFuncEntregue=fe.idFunc) 
                Left Join formapagto fp On (fp.IdFormaPagto=p.IdFormaPagto) Where 1 ";

            if (idPedido > 0)
            {
                sql += " And p.IdPedido=" + idPedido;
                temFiltro = true;
            }
            else if (idCli > 0)
            {
                sql += " And IdCli=" + idCli;
                temFiltro = true;
            }
            else if (!String.IsNullOrEmpty(nomeCli))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCli, null, 0, null, null, null, null, 0);
                sql += " And idCli in (" + ids + ")";
                temFiltro = true;
            }

            return sql;
        }

        public IList<Pedido> GetListCorte(uint idPedido, uint idCli, string nomeCli, string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? "pc.DataProducao desc" : sortExpression;

            bool temFiltro;
            string sql = SqlListCorte(idPedido, idCli, nomeCli, true, out temFiltro);

            return LoadDataWithSortExpression(sql, sort, startRow, pageSize, temFiltro,
                GetParam(nomeCli, null, null, null, null, null, null, null, null, null, null, null));
        }

        public int GetCountListCorte(uint idPedido, uint idCli, string nomeCli)
        {
            bool temFiltro;
            string sql = SqlListCorte(idPedido, idCli, nomeCli, true, out temFiltro);

            return GetCountWithInfoPaging(sql, temFiltro,
                GetParam(nomeCli, null, null, null, null, null, null, null, null, null, null, null));
        }

        #endregion

        #region Retira pedido de alguma situação, voltando para a anterior

        public void VoltaSituacao(Pedido pedido)
        {
            PedidoCorte pedidoCorte = PedidoCorteDAO.Instance.GetByIdPedido(pedido.IdPedido);

            switch (pedidoCorte.Situacao)
            {
                case 2: // Produção, exclui pedido da tabela pedido_corte
                    PedidoCorteDAO.Instance.Delete(pedidoCorte); break;
                case 3: // Pronto, volta para produção
                    pedidoCorte.DataPronto = null;
                    pedidoCorte.Situacao = (int)PedidoCorte.SituacaoEnum.Producao;
                    PedidoCorteDAO.Instance.Update(pedidoCorte);
                    break;
                case 4: // Entregue, volta para pronto
                    pedidoCorte.IdFuncEntregue = null;
                    pedidoCorte.DataEntregue = null;
                    pedidoCorte.Situacao = (int)PedidoCorte.SituacaoEnum.Pronto;
                    PedidoCorteDAO.Instance.Update(pedidoCorte);
                    break;
            }
        }

        #endregion

        #endregion

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
                select cp.idPedido, " + (campo.IsNullOrEmpty() ? "" : campo + ", " ) + @" coalesce(sum(coalesce(cp.BaseCalcComissao, 0)), 0) as valor
                from comissao_pedido cp
                    left join comissao c on (cp.idcomissao=c.idComissao)
                where " + where + @"
                group by cp.idPedido" + (campo.IsNullOrEmpty() ? "" : ", " + campo.Trim(','));

            return sql;
        }

        public string SqlComissao(string idComissao, string idsPedidos, uint idPedido, Pedido.TipoComissao tipoFunc, uint idFunc,
            string dataIni, string dataFim, bool semComissao, bool incluirComissaoPaga, string campoIds, uint idLoja, string tiposvenda= "")
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
        public void UpdateTotalPedido(GDASession sessao, uint idPedido)
        {
            UpdateTotalPedido(sessao, idPedido, true);
        }
 
         /// <summary>
         /// Atualiza o valor total do pedido, somando os totais dos produtos relacionados à ele
         /// </summary>
        public void UpdateTotalPedido(GDASession sessao, uint idPedido, bool criarLogDeAlteracao)
        {
            UpdateTotalPedido(sessao, idPedido, false, false, false, criarLogDeAlteracao);
        }

        /// <summary>
        /// Atualiza o valor total do pedido, somando os totais dos produtos relacionados à ele
        /// </summary>
        internal void UpdateTotalPedido(GDASession sessao, uint idPedido, bool liberando, bool forcarAtualizacao, bool alterouDesconto)
        {
            UpdateTotalPedido(sessao, idPedido, liberando, forcarAtualizacao, alterouDesconto, true);
        }
            
        /// <summary>
        /// Atualiza o valor total do pedido, somando os totais dos produtos relacionados à ele
        /// </summary>
        internal void UpdateTotalPedido(GDASession sessao, uint idPedido, bool liberando, bool forcarAtualizacao, bool alterouDesconto,
            bool criarLogDeAlteracao)
        {
            // Verifica se o usuário está atualizando o total
            if (!_atualizando.ContainsKey(UserInfo.GetUserInfo.CodUser))
                _atualizando.Add(UserInfo.GetUserInfo.CodUser, false);

            if (!forcarAtualizacao && _atualizando[UserInfo.GetUserInfo.CodUser])
                return;

            try
            {
                var pedido = GetElementByPrimaryKey(sessao, idPedido);

                // Define que o usuário está atualizando o total
                _atualizando[UserInfo.GetUserInfo.CodUser] = true;

                // Atualiza o custo do pedido
                UpdateCustoPedido(sessao, idPedido);

                // Atualiza total do pedido
                string sql = "update pedido p set Total=(Select Sum(Total + coalesce(valorBenef, 0)) From produtos_pedido Where " +
                    "IdPedido=p.IdPedido and (InvisivelPedido = false or InvisivelPedido is null) AND IdProdPedParent IS NULL) where p.idPedido=" + idPedido;

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
                        "Where IdPedido=" + idPedido;

                    objPersistence.ExecuteCommand(sessao, sql);
                }

                // Verifica se o desconto dado no pedido é permitido, se não for, zera o desconto
                if (!liberando)
                {
                    if (!DescontoPermitido(sessao, idPedido))
                        RemoveDescontoNaoPermitido(sessao, idPedido);
                    else if (alterouDesconto)
                    {
                        int tipoDesconto = ObtemValorCampo<int>(sessao, "tipoDesconto", "idPedido=" + idPedido);
                        decimal percDesconto = ObtemValorCampo<decimal>(sessao, "desconto", "idPedido=" + idPedido);

                        uint idFuncDesc = ObtemIdFuncDesc(sessao, idPedido) ?? UserInfo.GetUserInfo.CodUser;

                        if (tipoDesconto == 2)
                            percDesconto = Pedido.GetValorPerc(1, tipoDesconto, percDesconto, GetTotalSemDesconto(sessao, idPedido,
                                GetTotal(sessao, idPedido)));

                        if (percDesconto > (decimal)PedidoConfig.Desconto.GetDescontoMaximoPedido(idFuncDesc, ObtemTipoVenda(idPedido)))
                            Email.EnviaEmailDescontoMaior(sessao, idPedido, 0, idFuncDesc, (float)percDesconto, PedidoConfig.Desconto.GetDescontoMaximoPedido(idFuncDesc, ObtemTipoVenda(idPedido)));
                    }
                }

                float percFastDelivery = 1;

                // Verifica se há taxa de urgência para o pedido
                if (PedidoConfig.Pedido_FastDelivery.FastDelivery && IsFastDelivery(sessao, idPedido))
                {
                    percFastDelivery = 1 + (PedidoConfig.Pedido_FastDelivery.TaxaFastDelivery / 100);
                    sql = "update pedido set taxaFastDelivery=?taxa, Total=Round(Total * " + percFastDelivery.ToString().Replace(',', '.') + ", 2) where IdPedido=" + idPedido;

                    objPersistence.ExecuteCommand(sessao, sql, new GDAParameter("?taxa", PedidoConfig.Pedido_FastDelivery.TaxaFastDelivery));
                }

                uint idLoja = ObtemIdLoja(sessao, idPedido);
                uint idCliente = ObtemIdCliente(sessao, idPedido);

                string descontoRateadoImpostos = "0";

                if (!PedidoConfig.RatearDescontoProdutos)
                {
                    var dadosAmbientes = AmbientePedidoDAO.Instance.GetByPedido(sessao, idPedido).
                        Select(x => new { x.IdAmbientePedido, x.TotalProdutos });

                    var formata = new Func<decimal, string>(x => x.ToString().Replace(".", "").Replace(",", "."));

                    decimal totalSemDesconto = GetTotalSemDesconto(sessao, idPedido, (GetTotal(sessao, idPedido) / (decimal)percFastDelivery));
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
                if (LojaDAO.Instance.ObtemCalculaIcmsPedido(sessao, idLoja) && ClienteDAO.Instance.IsCobrarIcmsSt(sessao, idCliente))
                {
                    var calcIcmsSt = CalculoIcmsStFactory.ObtemInstancia(sessao, (int)idLoja, (int)idCliente, null, null, null, null);

                    string idProd = "pp.idProd";
                    string total = "pp.Total + Coalesce(pp.ValorBenef, 0)";
                    string aliquotaIcmsSt = "pp.AliqIcms";

                    sql = @"
                        Update produtos_pedido pp
                            inner join pedido ped on (pp.idPedido=ped.idPedido)
                            left join ambiente_pedido ap on (pp.idAmbientePedido=ap.idAmbientePedido)
                        {0}
                        Where pp.idPedido=" + idPedido + " and (pp.InvisivelPedido = false or pp.InvisivelPedido is null) AND pp.IdProdPedParent IS NULL";

                    // Atualiza a Alíquota ICMSST somada ao FCPST com o ajuste do MVA e do IPI. Necessário porque na tela é recuperado e salvo o valor sem FCPST.
                    objPersistence.ExecuteCommand(sessao, string.Format(sql,
                        "set pp.AliqIcms=(" + calcIcmsSt.ObtemSqlAliquotaInternaIcmsSt(sessao, idProd, total, descontoRateadoImpostos, aliquotaIcmsSt, percFastDelivery.ToString().Replace(',', '.')) + @")"));
                    // Atualiza o valor do ICMSST calculado com a Alíquota recuperada anteriormente.
                    objPersistence.ExecuteCommand(sessao, string.Format(sql,
                        "set pp.ValorIcms=(" + calcIcmsSt.ObtemSqlValorIcmsSt(total, descontoRateadoImpostos, aliquotaIcmsSt, percFastDelivery.ToString().Replace(',', '.')) + @")"));

                    sql = "update pedido set AliquotaIcms=(select sum(coalesce(AliqIcms, 0)) from produtos_pedido where idPedido=" + idPedido + " and (InvisivelPedido = false or InvisivelPedido is null) AND IdProdPedParent IS NULL) / (select Greatest(count(*), 1) from produtos_pedido where idPedido=" + idPedido + " and AliqIcms>0 and (InvisivelPedido = false or InvisivelPedido is null) AND IdProdPedParent IS NULL) where idPedido=" + idPedido;
                    objPersistence.ExecuteCommand(sessao, sql);

                    sql = "update pedido set ValorIcms=(select sum(coalesce(ValorIcms, 0)) from produtos_pedido where IdPedido=" + idPedido + " and (InvisivelPedido = false or InvisivelPedido is null) AND IdProdPedParent IS NULL), Total=(Total + ValorIcms) where idPedido=" + idPedido;
                    objPersistence.ExecuteCommand(sessao, sql);
                }
                else
                {
                    sql = "update produtos_pedido pp set AliqIcms=0, ValorIcms=0 where idPedido=" + idPedido + " and (InvisivelPedido = false or InvisivelPedido is null) AND IdProdPedParent IS NULL";
                    objPersistence.ExecuteCommand(sessao, sql);

                    sql = "update pedido set AliquotaIcms=0, ValorIcms=0 where idPedido=" + idPedido;
                    objPersistence.ExecuteCommand(sessao, sql);
                }

                // Calcula o valor do IPI do pedido
                if (LojaDAO.Instance.ObtemCalculaIpiPedido(sessao, idLoja) && ClienteDAO.Instance.IsCobrarIpi(sessao, idCliente))
                {
                    sql = @"
                        Update produtos_pedido pp
                            inner join pedido ped on (pp.idPedido=ped.idPedido)
                            left join ambiente_pedido ap on (pp.idAmbientePedido=ap.idAmbientePedido) 
                        {0}
                        Where pp.idPedido=" + idPedido + " and (pp.InvisivelPedido = false or pp.InvisivelPedido is null) AND pp.IdProdPedParent IS NULL";

                    objPersistence.ExecuteCommand(sessao, string.Format(sql,
                        "SET pp.AliquotaIpi=Round((select aliqIpi from produto where idProd=pp.idProd), 2)"));

                    objPersistence.ExecuteCommand(sessao, string.Format(sql,
                        "SET pp.ValorIpi=(((pp.Total + Coalesce(pp.ValorBenef, 0) - " + descontoRateadoImpostos + @") * " + percFastDelivery.ToString().Replace(',', '.') + @")  * (Coalesce(pp.AliquotaIpi, 0) / 100))"));

                    sql = "update pedido set AliquotaIpi=Round((select sum(coalesce(AliquotaIpi, 0)) from produtos_pedido where idPedido=" + idPedido + " and (InvisivelPedido = false or InvisivelPedido is null) AND IdProdPedParent IS NULL) / (select Greatest(count(*), 1) from produtos_pedido where idPedido=" + idPedido + " and AliquotaIpi>0 and (InvisivelPedido = false or InvisivelPedido is null) AND IdProdPedParent IS NULL), 2) where idPedido=" + idPedido;
                    objPersistence.ExecuteCommand(sessao, sql);

                    sql = "update pedido set ValorIpi=Round((select sum(coalesce(ValorIpi, 0)) from produtos_pedido where IdPedido=" + idPedido + " and (InvisivelPedido = false or InvisivelPedido is null) AND IdProdPedParent IS NULL), 2), Total=(Total + ValorIpi) where idPedido=" + idPedido;
                    objPersistence.ExecuteCommand(sessao, sql);
                }
                else
                {
                    sql = "update produtos_pedido pp set AliquotaIpi=0, ValorIpi=0 where idPedido=" + idPedido + " and (InvisivelPedido = false or InvisivelPedido is null) AND IdProdPedParent IS NULL";
                    objPersistence.ExecuteCommand(sessao, sql);

                    sql = "update pedido set AliquotaIpi=0, ValorIpi=0 where idPedido=" + idPedido;
                    objPersistence.ExecuteCommand(sessao, sql);
                }

                // Atualiza o campo ValorComissao
                sql = @"update pedido set valorComissao=total*coalesce(percComissao,0)/100 where idPedido=" + idPedido;
                objPersistence.ExecuteCommand(sessao, sql);

                // Atualiza peso e total de m²
                AtualizaTotM(sessao, idPedido, false);
                AtualizaPeso(sessao, idPedido);

                // Aplica taxa de juros no pedido
                string taxaPrazo = "0";
                objPersistence.ExecuteCommand(sessao, "update pedido set taxaPrazo=" + taxaPrazo + ", Total=Round(Total*(1+(taxaPrazo/100)), 2) where IdPedido=" + idPedido);

                //Aplica o frete no pedido
                objPersistence.ExecuteCommand(sessao, "UPDATE pedido SET Total = COALESCE(Total, 0) + ValorEntrega WHERE IdPedido=" + idPedido);

                // Se for parceiro, gera parcelas do pedido
                if (ObtemValorCampo<bool>(sessao, "geradoParceiro", "idPedido=" + idPedido))
                {
                    Pedido ped = GetElementByPrimaryKey(sessao, idPedido);
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

                if (criarLogDeAlteracao)
                    LogAlteracaoDAO.Instance.LogPedido(sessao, pedido, GetElementByPrimaryKey(sessao, idPedido), LogAlteracaoDAO.SequenciaObjeto.Atual);
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

            return objPersistence.LoadData(sessao, Sql(0, 0, idsPedido, null, 0, 0, null, 0, null, 0, null, null, null, null, null, null, null, null,
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
                retorno = retorno.FindAll(new Predicate<Pedido>(delegate(Pedido x)
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

        #region Comissão

        #region Aplica a comissão no valor dos produtos

        /// <summary>
        /// Aplica um percentual de comissão sobre o valor dos produtos do pedido.
        /// </summary>
        public void AplicaComissao(GDASession session, uint idPedido, float percComissao)
        {
            AplicaComissao(session, idPedido, percComissao, (ProdutosPedido)null);
        }

        /// <summary>
        /// Aplica um percentual de comissão sobre o valor dos produtos do pedido.
        /// </summary>
        public void AplicaComissao(GDASession sessao, uint idPedido, float percComissao, ProdutosPedido produtoPedido)
        {
            AplicaComissao(sessao, idPedido, percComissao, null, produtoPedido);
        }

        /// <summary>
        /// Aplica um percentual de comissão sobre o valor dos produtos do pedido.
        /// </summary>
        public void AplicaComissao(GDASession sessao, uint idPedido, float percComissao, int? idAmbientePedido)
        {
            AplicaComissao(sessao, idPedido, percComissao, idAmbientePedido, null);
        }

        /// <summary>
        /// Aplica um percentual de comissão sobre o valor dos produtos do pedido.
        /// </summary>
        public void AplicaComissao(GDASession sessao, uint idPedido, float percComissao, int? idAmbientePedido,
            ProdutosPedido produtoPedido)
        {
            if (!PedidoConfig.Comissao.ComissaoAlteraValor)
                return;

            var atualizarDados = false;

            try
            {
                var produtos = ProdutosPedidoDAO.Instance.GetByPedidoLite(sessao, idPedido, true).ToArray();
                atualizarDados = DescontoAcrescimo.Instance.AplicaComissao(sessao, percComissao, produtos, (int?)idPedido, null, null);

                if (atualizarDados)
                    foreach (var prod in produtos)
                    {
                        ProdutosPedidoDAO.Instance.UpdateBase(sessao, prod);
                        ProdutosPedidoDAO.Instance.AtualizaBenef(sessao, prod.IdProdPed, prod.Beneficiamentos);
                    }
            }
            finally
            {
                if (atualizarDados)
                    UpdateTotalPedido(sessao, idPedido);
            }
        }

        #endregion

        #region Remove a comissão no valor dos produtos

        /// <summary>
        /// Remove comissão no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        public void RemoveComissao(GDASession sessao, uint idPedido, ProdutosPedido produtoPedido)
        {
            float percComissao = RecuperaPercComissao(sessao, idPedido);
            RemoveComissao(sessao, idPedido, percComissao, produtoPedido);
        }

        /// <summary>
        /// Remove comissão no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        private void RemoveComissao(GDASession sessao, uint idPedido, float percComissao)
        {
            RemoveComissao(sessao, idPedido, percComissao, null, null);
        }

        /// <summary>
        /// Remove comissão no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        private void RemoveComissao(GDASession sessao, uint idPedido, float percComissao,
            ProdutosPedido produtoPedido)
        {
            RemoveComissao(sessao, idPedido, percComissao, null, produtoPedido);
        }

        /// <summary>
        /// Remove comissão no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        private void RemoveComissao(GDASession sessao, uint idPedido, float percComissao, int? idAmbientePedido)
        {
            RemoveComissao(sessao, idPedido, percComissao, idAmbientePedido, null);
        }

        /// <summary>
        /// Remove comissão no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        private void RemoveComissao(GDASession sessao, uint idPedido, float percComissao, int? idAmbientePedido,
            ProdutosPedido produtoPedido)
        {
            var atualizarDados = false;

            try
            {
                var produtos = ProdutosPedidoDAO.Instance.GetByPedidoLite(sessao, idPedido, true).ToArray();
                atualizarDados = DescontoAcrescimo.Instance.RemoveComissao(sessao, percComissao, produtos, (int?)idPedido, null, null);

                if (atualizarDados)
                    foreach (var prod in produtos)
                    {
                        ProdutosPedidoDAO.Instance.UpdateBase(sessao, prod);
                        ProdutosPedidoDAO.Instance.AtualizaBenef(sessao, prod.IdProdPed, prod.Beneficiamentos);
                    }
            }
            finally
            {
                if (atualizarDados)
                    UpdateTotalPedido(sessao, idPedido);
            }
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
        public void AplicaAcrescimo(GDASession sessao, uint idPedido, int tipoAcrescimo, decimal acrescimo, bool atualizarClone)
        {
            AplicaAcrescimo(sessao, idPedido, tipoAcrescimo, acrescimo, atualizarClone, (ProdutosPedido)null);
        }

        /// <summary>
        /// Aplica acréscimo no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        public void AplicaAcrescimo(GDASession sessao, uint idPedido, int tipoAcrescimo, decimal acrescimo, bool atualizarClone, ProdutosPedido produtoPedido)
        {
            AplicaAcrescimo(sessao, idPedido, tipoAcrescimo, acrescimo, atualizarClone, null, produtoPedido);
        }

        /// <summary>
        /// Aplica acréscimo no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        public void AplicaAcrescimo(GDASession sessao, uint idPedido, int tipoAcrescimo, decimal acrescimo, bool atualizarClone, int? idAmbientePedido)
        {
            AplicaAcrescimo(sessao, idPedido, tipoAcrescimo, acrescimo, atualizarClone, idAmbientePedido, null);
        }

        /// <summary>
        /// Aplica acréscimo no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        public void AplicaAcrescimo(GDASession sessao, uint idPedido, int tipoAcrescimo, decimal acrescimo, bool atualizarClone, int? idAmbientePedido, ProdutosPedido prodPed)
        {
            var atualizarDados = false;

            try
            {
                var produtos = ProdutosPedidoDAO.Instance.GetByPedidoLite(sessao, idPedido, atualizarClone, true).ToArray();
                atualizarDados = DescontoAcrescimo.Instance.AplicaAcrescimo(sessao, tipoAcrescimo, acrescimo, produtos.ToArray(), (int?)idPedido, null, null);

                if (atualizarDados)
                    foreach (var produtoPedido in produtos)
                    {
                        ProdutosPedidoDAO.Instance.Update(sessao, produtoPedido, false, false, false);
                        ProdutosPedidoDAO.Instance.AtualizaBenef(sessao, produtoPedido.IdProdPed, produtoPedido.Beneficiamentos);
                    }
            }
            finally
            {
                if (atualizarDados)
                    UpdateTotalPedido(sessao, idPedido);
            }
        }

        #endregion

        #region Remove acréscimo no valor dos produtos

        /// <summary>
        /// Remove acréscimo no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        public void RemoveAcrescimo(GDASession sessao, uint idPedido, ProdutosPedido produtoPedido)
        {
            int tipoAcrescimo = ObtemValorCampo<int>(sessao, "tipoAcrescimo", "idPedido=" + idPedido);
            decimal acrescimo = ObtemValorCampo<decimal>(sessao, "acrescimo", "idPedido=" + idPedido);
            RemoveAcrescimo(sessao, idPedido, tipoAcrescimo, acrescimo, produtoPedido);
        }

        /// <summary>
        /// Remove acréscimo no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        private void RemoveAcrescimo(GDASession sessao, uint idPedido, int tipoAcrescimo, decimal acrescimo, ProdutosPedido produtoPedido)
        {
            RemoveAcrescimo(sessao, idPedido, tipoAcrescimo, acrescimo, null, produtoPedido);
        }

        /// <summary>
        /// Remove acréscimo no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        private void RemoveAcrescimo(GDASession sessao, uint idPedido, int tipoAcrescimo, decimal acrescimo, int? idAmbientePedido)
        {
            RemoveAcrescimo(sessao, idPedido, tipoAcrescimo, acrescimo, idAmbientePedido, null);
        }

        /// <summary>
        /// Remove acréscimo no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        private void RemoveAcrescimo(GDASession sessao, uint idPedido, int tipoAcrescimo, decimal acrescimo, int? idAmbientePedido, ProdutosPedido prodPed)
        {
            var atualizarDados = false;

            try
            {
                var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedidoLite(sessao, idPedido, true).ToArray();
                atualizarDados = DescontoAcrescimo.Instance.RemoveAcrescimo(sessao, tipoAcrescimo, acrescimo, produtosPedido, (int?)idPedido, null, null);

                if (atualizarDados)
                    foreach (var produtoPedido in produtosPedido)
                    {
                        ProdutosPedidoDAO.Instance.Update(sessao, produtoPedido, false, false, false);
                        ProdutosPedidoDAO.Instance.AtualizaBenef(sessao, produtoPedido.IdProdPed, produtoPedido.Beneficiamentos);
                    }
            }
            finally
            {
                if (atualizarDados)
                    UpdateTotalPedido(sessao, idPedido);
            }
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
        public void AplicaDesconto(GDASession sessao, uint idPedido, int tipoDesconto, decimal desconto, bool manterFuncDesc)
        {
            AplicaDesconto(sessao, idPedido, tipoDesconto, desconto, manterFuncDesc, true);
        }

        /// <summary>
        /// Aplica desconto no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        public void AplicaDesconto(GDASession sessao, uint idPedido, int tipoDesconto, decimal desconto, bool manterFuncDesc,
            bool criarLogDeAlteracao)
        {
            AplicaDesconto(sessao, idPedido, tipoDesconto, desconto, manterFuncDesc, (ProdutosPedido)null, criarLogDeAlteracao);
        }

        /// <summary>
        /// Aplica desconto no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        public void AplicaDesconto(GDASession sessao, uint idPedido, int tipoDesconto, decimal desconto, bool manterFuncDesc,
            ProdutosPedido produtoPedido)
        {
            AplicaDesconto(sessao, idPedido, tipoDesconto, desconto, manterFuncDesc, produtoPedido, true);
        }

        /// <summary>
        /// Aplica desconto no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        public void AplicaDesconto(GDASession sessao, uint idPedido, int tipoDesconto, decimal desconto, bool manterFuncDesc,
            ProdutosPedido produtoPedido, bool criarLogDeAlteracao)
        {
            AplicaDesconto(sessao, idPedido, tipoDesconto, desconto, manterFuncDesc, null, produtoPedido, criarLogDeAlteracao);
        }

        /// <summary>
        /// Aplica desconto no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        public void AplicaDesconto(GDASession sessao, uint idPedido, int tipoDesconto, decimal desconto, bool manterFuncDesc,
            int? idAmbientePedido, bool criarLogDeAlteracao)
        {
            AplicaDesconto(sessao, idPedido, tipoDesconto, desconto, manterFuncDesc, idAmbientePedido, null, criarLogDeAlteracao);
        }

        /// <summary>
        /// Aplica desconto no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        public void AplicaDesconto(GDASession sessao, uint idPedido, int tipoDesconto, decimal desconto, bool manterFuncDesc,
            int? idAmbientePedido, ProdutosPedido produtoPedido, bool criarLogDeAlteracao)
        {
            var atualizarDados = false;

            try
            {
                var produtos = ProdutosPedidoDAO.Instance.GetByPedidoLite(sessao, idPedido, true).ToArray();
                atualizarDados = DescontoAcrescimo.Instance.AplicaDesconto(sessao, tipoDesconto, desconto, produtos, (int?)idPedido, null, null);

                if (atualizarDados)
                    foreach (var prod in produtos)
                    {
                        ProdutosPedidoDAO.Instance.UpdateBase(sessao, prod);
                        ProdutosPedidoDAO.Instance.AtualizaBenef(sessao, prod.IdProdPed, prod.Beneficiamentos);
                    }

                // A data do desconto não pode ser alterada caso o pedido esteja sendo gerado pelo orçamento.
                if (!manterFuncDesc)
                    objPersistence.ExecuteCommand(sessao, "update pedido set idFuncDesc=?f, dataDesc=?d where idPedido=" + idPedido,
                        new GDAParameter("?f", UserInfo.GetUserInfo.CodUser), new GDAParameter("?d", DateTime.Now));
            }
            finally
            {
                if (atualizarDados)
                    UpdateTotalPedido(sessao, idPedido, criarLogDeAlteracao);
            }
        }

        #endregion

        #region Remove desconto no valor dos produtos

        /// <summary>
        /// Remove acréscimo no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        public void RemoveDesconto(GDASession sessao, uint idPedido)
        {
            RemoveDesconto(sessao, idPedido, null);
        }

        /// <summary>
        /// Remove acréscimo no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        public void RemoveDesconto(GDASession sessao, uint idPedido, ProdutosPedido produtoPedido)
        {
            int tipoDesconto = ObtemValorCampo<int>(sessao, "tipoDesconto", "idPedido=" + idPedido);
            decimal desconto = ObtemValorCampo<decimal>(sessao, "desconto", "idPedido=" + idPedido);
            RemoveDesconto(sessao, idPedido, tipoDesconto, desconto, produtoPedido);
        }

        /// <summary>
        /// Remove acréscimo no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        private void RemoveDesconto(GDASession sessao, uint idPedido, int tipoDesconto, decimal desconto, ProdutosPedido produtoPedido)
        {
            RemoveDesconto(sessao, idPedido, tipoDesconto, desconto, null, produtoPedido);
        }

        /// <summary>
        /// Remove acréscimo no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        private void RemoveDesconto(GDASession sessao, uint idPedido, int tipoDesconto, decimal desconto, int? idAmbientePedido)
        {
            RemoveDesconto(sessao, idPedido, tipoDesconto, desconto, idAmbientePedido, null);
        }

        /// <summary>
        /// Remove acréscimo no valor dos produtos e consequentemente no valor do pedido
        /// </summary>
        private void RemoveDesconto(GDASession sessao, uint idPedido, int tipoDesconto, decimal desconto, int? idAmbientePedido,
            ProdutosPedido produtoPedido)
        {
            var atualizarDados = false;

            try
            {
                var produtos = ProdutosPedidoDAO.Instance.GetByPedidoLite(sessao, idPedido, true).ToArray();
                atualizarDados = DescontoAcrescimo.Instance.RemoveDesconto(sessao, tipoDesconto, desconto, produtos, (int?)idPedido, null, null);

                if (atualizarDados)
                    foreach (var prod in produtos)
                    {
                        ProdutosPedidoDAO.Instance.UpdateBase(sessao, prod);
                        ProdutosPedidoDAO.Instance.AtualizaBenef(sessao, prod.IdProdPed, prod.Beneficiamentos);
                    }
            }
            finally
            {
                if (atualizarDados)
                    UpdateTotalPedido(sessao, idPedido);
            }
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
                    situacao = !PedidoEspelhoDAO.Instance.ExisteEspelho(sessao, idPedido) ? Pedido.SituacaoProducaoEnum.NaoEntregue : Pedido.SituacaoProducaoEnum.Pendente;
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
                        AlteraSituacao(sessao, idPedido, Pedido.SituacaoPedido.Confirmado);
                    else if (ObtemSituacao(sessao, idPedido) != Pedido.SituacaoPedido.ConfirmadoLiberacao)
                        AlteraSituacao(sessao, idPedido, Pedido.SituacaoPedido.ConfirmadoLiberacao);
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
                    uint idMaterial = MaterialItemProjetoDAO.Instance.InsertBase(session, mip);

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

                uint idMaterial = MaterialItemProjetoDAO.Instance.InsertBase(session, mip);

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

        #endregion

        #region Data de entrega mínima do pedido

        /// <summary>
        /// Verifica se a data de entrega de um pedido deve ser bloqueada.
        /// </summary>
        public bool BloquearDataEntregaMinima(uint? idPedido)
        {
            return BloquearDataEntregaMinima(null, idPedido);
        }

        /// <summary>
        /// Verifica se a data de entrega de um pedido deve ser bloqueada.
        /// </summary>
        public bool BloquearDataEntregaMinima(GDASession session, uint? idPedido)
        {
            var tipoPedido = idPedido > 0 ? GetTipoPedido(session, idPedido.Value) : Pedido.TipoPedidoEnum.Producao;

            // Comentado: não bloqueia a data de entrega mínima se o pedido for de revenda
            //int configDias = tipoPedido == Pedido.TipoPedidoEnum.Venda ? PedidoConfig.DataEntrega.NumeroDiasUteisDataEntregaPedido :
            //    tipoPedido == Pedido.TipoPedidoEnum.Revenda ? PedidoConfig.DataEntrega.NumeroDiasUteisDataEntregaPedidoRevenda : 0;

            var configDias = tipoPedido == Pedido.TipoPedidoEnum.Venda ? PedidoConfig.DataEntrega.NumeroDiasUteisDataEntregaPedido : 0;

            int? tipoEntrega = idPedido > 0 ? (int?)ObtemTipoEntrega(session, idPedido.Value) : null;
            if (tipoEntrega > 0 && PedidoConfig.DiasMinimosEntregaTipo.Keys.Contains((Pedido.TipoEntregaPedido)tipoEntrega.Value))
                configDias = Math.Max(configDias, PedidoConfig.DiasMinimosEntregaTipo[(Pedido.TipoEntregaPedido)tipoEntrega.Value]);

            return configDias > 0 &&
                !Config.PossuiPermissao(Config.FuncaoMenuPedido.IgnorarBloqueioDataEntrega) &&
                (!(idPedido > 0) || !IsFastDelivery(idPedido.Value));
        }

        /// <summary>
        /// Altera a data de entrega do pedido para a data mínima.
        /// </summary>
        public void SetDataEntregaMinima(GDASession session, uint idPedido)
        {
            DateTime dataEntrega, dataFastDelivery;

            if (GetDataEntregaMinima(session, ObtemIdCliente(session, idPedido), idPedido, (int)GetTipoPedido(session, idPedido), ObtemTipoEntrega(session, idPedido),
                out dataEntrega, out dataFastDelivery))
                objPersistence.ExecuteCommand(session, "update pedido set dataEntrega=?data where idPedido=" + idPedido,
                    new GDAParameter("?data", !IsFastDelivery(session, idPedido) ? dataEntrega : dataFastDelivery));
        }

        internal DateTime GetDataEntregaMinimaFinal(uint idPedido, DateTime dataEntregaAtual, bool fastDelivery = false)
        {
            return GetDataEntregaMinimaFinal(null, idPedido, dataEntregaAtual, fastDelivery);
        }

        internal DateTime GetDataEntregaMinimaFinal(GDASession session, uint idPedido, DateTime dataEntregaAtual, bool fastDelivery = false)
        {
            DateTime data1, data2;
            bool desabilitar;

            uint idCliente = PedidoDAO.Instance.ObtemIdCliente(session, idPedido);
            int? tipoPedido = (int?)GetTipoPedido(session, idPedido);
            int? tipoEntrega = (int?)ObtemTipoEntrega(session, idPedido);
            DateTime dataPedido = GetDataPedido(session, idPedido);

            var dataComparar = !PedidoDAO.Instance.GetDataEntregaMinima(session, idCliente, idPedido, tipoPedido, tipoEntrega,
                dataPedido, out data1, out data2, out desabilitar, 0, fastDelivery) ? dataEntregaAtual : !fastDelivery ? data1 : data2;

            return dataEntregaAtual.Date >= dataComparar.Date ? dataEntregaAtual : dataComparar;
        }

        /// <summary>
        /// Recupera a data de entrega mínima para um pedido.
        /// </summary>
        public bool GetDataEntregaMinima(uint idCli, uint? idPedido, out DateTime dataEntregaMinima,
            out DateTime dataFastDelivery)
        {
            return GetDataEntregaMinima(null, idCli, idPedido, out dataEntregaMinima, out dataFastDelivery);
        }

        public bool GetDataEntregaMinima(GDASession session, uint idCli, uint? idPedido, out DateTime dataEntregaMinima, out DateTime dataFastDelivery)
        {
            int? tipoPedido = idPedido > 0 ? (int?)GetTipoPedido(session, idPedido.Value) : null;
            int? tipoEntrega = idPedido > 0 ? (int?)ObtemTipoEntrega(session, idPedido.Value) : null;
            return GetDataEntregaMinima(session, idCli, idPedido, tipoPedido, tipoEntrega, out dataEntregaMinima, out dataFastDelivery);
        }

        /// <summary>
        /// Recupera a data de entrega mínima para um pedido.
        /// </summary>
        public bool GetDataEntregaMinima(uint idCli, uint? idPedido, int? tipoPedido, int? tipoEntrega, out DateTime dataEntregaMinima, out DateTime dataFastDelivery)
        {
            return GetDataEntregaMinima((GDASession)null, idCli, idPedido, tipoPedido, tipoEntrega, out dataEntregaMinima, out dataFastDelivery);
        }

        /// <summary>
        /// Recupera a data de entrega mínima para um pedido.
        /// </summary>
        public bool GetDataEntregaMinima(GDASession session, uint idCli, uint? idPedido, int? tipoPedido, int? tipoEntrega, out DateTime dataEntregaMinima, out DateTime dataFastDelivery)
        {
            return GetDataEntregaMinima(session, idCli, idPedido, tipoPedido, tipoEntrega, out dataEntregaMinima, out dataFastDelivery, 0);
        }

        /// <summary>
        /// Recupera a data de entrega mínima para um pedido.
        /// </summary>
        public bool GetDataEntregaMinima(uint idCli, uint? idPedido, int? tipoPedido, int? tipoEntrega, out DateTime dataEntregaMinima, out DateTime dataFastDelivery, int numeroDiasUteisMinimoNaoConfig)
        {
            return GetDataEntregaMinima(null, idCli, idPedido, tipoPedido, tipoEntrega, out dataEntregaMinima, out dataFastDelivery, numeroDiasUteisMinimoNaoConfig);
        }

        /// <summary>
        /// Recupera a data de entrega mínima para um pedido.
        /// </summary>
        public bool GetDataEntregaMinima(GDASession session, uint idCli, uint? idPedido, int? tipoPedido, int? tipoEntrega, out DateTime dataEntregaMinima, out DateTime dataFastDelivery, int numeroDiasUteisMinimoNaoConfig)
        {
            bool temp;
            DateTime? dataBase = idPedido > 0 ? (DateTime?)GetDataPedido(session, idPedido.Value) : null;

            if (dataBase != null && dataBase.Value.Date < DateTime.Now.Date)
                dataBase = DateTime.Now;

            return GetDataEntregaMinima(session, idCli, idPedido, tipoPedido, tipoEntrega, dataBase, out dataEntregaMinima, out dataFastDelivery, out temp, numeroDiasUteisMinimoNaoConfig);
        }

        /// <summary>
        /// Recupera a data de entrega mínima para um pedido.
        /// </summary>
        public bool GetDataEntregaMinima(uint idCli, uint? idPedido, int? tipoPedido, int? tipoEntrega, DateTime? dataBase, out DateTime dataEntregaMinima, out DateTime dataFastDelivery, out bool desabilitarCampo)
        {
            return GetDataEntregaMinima(null, idCli, idPedido, tipoPedido, tipoEntrega, dataBase, out dataEntregaMinima, out dataFastDelivery, out desabilitarCampo);
        }

        /// <summary>
        /// Recupera a data de entrega mínima para um pedido.
        /// </summary>
        public bool GetDataEntregaMinima(GDASession session, uint idCli, uint? idPedido, int? tipoPedido, int? tipoEntrega, DateTime? dataBase, out DateTime dataEntregaMinima, out DateTime dataFastDelivery, out bool desabilitarCampo)
        {
            return GetDataEntregaMinima(session, idCli, idPedido, tipoPedido, tipoEntrega, dataBase, out dataEntregaMinima, out dataFastDelivery, out desabilitarCampo, 0);
        }

        /// <summary>
        /// Recupera a data de entrega mínima para um pedido.
        /// </summary>
        public bool GetDataEntregaMinima(uint idCli, uint? idPedido, int? tipoPedido, int? tipoEntrega,
            DateTime? dataBase, out DateTime dataEntregaMinima, out DateTime dataFastDelivery, out bool desabilitarCampo,
            int numeroDiasUteisMinimoNaoConfig, bool fastDelivery = false)
        {
            return GetDataEntregaMinima(null, idCli, idPedido, tipoPedido, tipoEntrega, dataBase, out dataEntregaMinima,
                out dataFastDelivery, out desabilitarCampo, numeroDiasUteisMinimoNaoConfig, fastDelivery);
        }

        /// <summary>
        /// Recupera a data de entrega mínima para um pedido.
        /// </summary>
        public bool GetDataEntregaMinima(GDASession session, uint idCli, uint? idPedido, int? tipoPedido, int? tipoEntrega, DateTime? dataBase,
            out DateTime dataEntregaMinima, out DateTime dataFastDelivery, out bool desabilitarCampo,int numeroDiasUteisMinimoNaoConfig, bool fastDelivery = false)
        {
            try
            {
                /* Chamado 47744. */
                if (UserInfo.GetUserInfo == null || (UserInfo.GetUserInfo.CodUser == 0 && UserInfo.GetUserInfo.IdCliente == 0))
                    throw new Exception("Não foi possível recuperar o login do usuário.");

                if (dataBase == null || dataBase.Value.Ticks == 0)
                    dataBase = DateTime.Now;

                DateTime? dataRota = RotaDAO.Instance.GetDataRota(session, idCli, dataBase.Value.Date);

                dataEntregaMinima = dataBase.Value.Date;
                dataFastDelivery = dataBase.Value.Date;
                desabilitarCampo = PedidoConfig.DataEntrega.BloquearDataEntregaPedidoVendedor && !Config.PossuiPermissao(Config.FuncaoMenuPedido.IgnorarBloqueioDataEntrega)
                    && !UserInfo.GetUserInfo.IsAdministrador;

                tipoPedido = tipoPedido != null ? tipoPedido : idPedido > 0 ? (int?)GetTipoPedido(session, idPedido.Value) : null;
                int numeroDiasUteisMinimoConfig = tipoPedido == (int)Pedido.TipoPedidoEnum.Revenda ? PedidoConfig.DataEntrega.NumeroDiasUteisDataEntregaPedidoRevenda :
                    tipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObra ? PedidoConfig.DataEntrega.NumeroDiasUteisDataEntregaPedidoMaoDeObra :
                    PedidoConfig.DataEntrega.NumeroDiasUteisDataEntregaPedido;

                if (tipoEntrega != null)
                {
                    var existeTipo = PedidoConfig.DiasMinimosEntregaTipo.ContainsKey((Pedido.TipoEntregaPedido)tipoEntrega.Value);

                    numeroDiasUteisMinimoConfig = Math.Max(numeroDiasUteisMinimoConfig,
                        existeTipo ? PedidoConfig.DiasMinimosEntregaTipo[(Pedido.TipoEntregaPedido)tipoEntrega.Value] : 0);
                }

                int numeroDiasSomar = numeroDiasUteisMinimoConfig > 0 ? numeroDiasUteisMinimoConfig : numeroDiasUteisMinimoNaoConfig;
                var considerouDiasUteisSubgrupo = false;
                int? diaSemanaEntrega = null;

                #region Busca a data de entrega mínima de acordo com os produtos do pedido

                if (idPedido > 0)
                {
                    var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedido(session, idPedido.Value);

                    // Etiqueta Processo
                    var diasDataEntregaProcesso = 0;
                    foreach (var pp in produtosPedido.Where(f => f.IdProcesso > 0).ToList())
                        diasDataEntregaProcesso = Math.Max(diasDataEntregaProcesso, EtiquetaProcessoDAO.Instance.ObterNumeroDiasUteisDataEntrega(session, pp.IdProcesso.Value));
                    // Considera a data maior entre a data das configurações e da data do processo.
                    numeroDiasSomar = Math.Max(numeroDiasSomar, diasDataEntregaProcesso);

                    // Subgrupo produto.
                    Dictionary<uint, KeyValuePair<int?, int?>> subgrupos = new Dictionary<uint, KeyValuePair<int?, int?>>();
                    foreach (ProdutosPedido pp in produtosPedido)
                        if (pp.IdSubgrupoProd > 0 && !subgrupos.ContainsKey(pp.IdSubgrupoProd))
                        {
                            subgrupos.Add(pp.IdSubgrupoProd, new KeyValuePair<int?, int?>(
                                SubgrupoProdDAO.Instance.ObtemValorCampo<int?>(session, "numeroDiasMinimoEntrega", "idSubgrupoProd=" + pp.IdSubgrupoProd),
                                SubgrupoProdDAO.Instance.ObtemValorCampo<int?>(session, "diaSemanaEntrega", "idSubgrupoProd=" + pp.IdSubgrupoProd)
                            ));
                        }

                    uint idSubgrupoMaiorPrazo = 0;
                    foreach (uint key in subgrupos.Keys)
                        if (subgrupos[key].Key > 0 || subgrupos[key].Value != null)
                            if (idSubgrupoMaiorPrazo == 0 || subgrupos[key].Key > subgrupos[idSubgrupoMaiorPrazo].Key)
                                idSubgrupoMaiorPrazo = key;

                    if (idSubgrupoMaiorPrazo > 0)
                    {
                        /* Chamado 54042. */
                        considerouDiasUteisSubgrupo = subgrupos[idSubgrupoMaiorPrazo].Key.GetValueOrDefault() > numeroDiasSomar;

                        numeroDiasSomar = Math.Max(numeroDiasSomar, subgrupos[idSubgrupoMaiorPrazo].Key.GetValueOrDefault());
                        diaSemanaEntrega = subgrupos[idSubgrupoMaiorPrazo].Value;
                        desabilitarCampo = desabilitarCampo && tipoPedido == (int)Pedido.TipoPedidoEnum.Venda;
                    }
                }

                #endregion

                // Se tiver permissão de ignorar bloqueios na data de entrega, não desabilita o campo
                desabilitarCampo = desabilitarCampo && !Config.PossuiPermissao<Config.FuncaoMenuPedido>(Config.FuncaoMenuPedido.IgnorarBloqueioDataEntrega);

                // Calcula a data do fast delivery
                int j = 0;
                while (j < PedidoConfig.Pedido_FastDelivery.PrazoEntregaFastDelivery)
                {
                    dataFastDelivery = dataFastDelivery.AddDays(1);
                    while (!dataFastDelivery.DiaUtil())
                        dataFastDelivery = dataFastDelivery.AddDays(1);

                    j++;
                }

                if(PedidoConfig.Pedido_FastDelivery.ConsiderarTurnoFastDelivery && DateTime.Now.Hour >= 12 && DateTime.Now.Minute >= 1)
                {
                    dataFastDelivery = dataFastDelivery.AddDays(1);
                    while (!dataFastDelivery.DiaUtil())
                        dataFastDelivery = dataFastDelivery.AddDays(1);
                }

                var m2Pedido = idPedido > 0 ? ProdutosPedidoDAO.Instance.GetTotalM2ByPedido(session, idPedido.Value) : 0;

                // Calcula o fast delivery somente se o pedido for fast delivery (10651)
                if (PedidoConfig.Pedido_FastDelivery.FastDelivery && idPedido > 0 && (fastDelivery == true || IsFastDelivery(session, idPedido.Value)))
                {
                    // Se a metragem do pedido for maior que o total diário do fast delivery e se o pedido for importado, apenas não calcula a data do fast delivery
                    // chamado (67439)
                    if (m2Pedido > PedidoConfig.Pedido_FastDelivery.M2MaximoFastDelivery && !IsPedidoImportado(session, idPedido.Value))
                        dataFastDelivery = ProdutosPedidoDAO.Instance.GetFastDeliveryDay(session, idPedido.Value, dataFastDelivery, m2Pedido, false).GetValueOrDefault(dataFastDelivery);
                }

                if (numeroDiasSomar > 0)
                {
                    int i = 0;

                    while (i < numeroDiasSomar)
                    {
                        dataEntregaMinima = dataEntregaMinima.AddDays(1);
                        while (!dataEntregaMinima.DiaUtil())
                            dataEntregaMinima = dataEntregaMinima.AddDays(1);

                        i++;
                    }

                    if (diaSemanaEntrega != null)
                    {
                        while ((int)dataEntregaMinima.DayOfWeek != diaSemanaEntrega.Value)
                            dataEntregaMinima = dataEntregaMinima.AddDays(1);

                        if (diaSemanaEntrega != (int)DayOfWeek.Saturday && diaSemanaEntrega != (int)DayOfWeek.Sunday)
                            while (!dataEntregaMinima.DiaUtil())
                                dataEntregaMinima = dataEntregaMinima.AddDays(7);
                    }
                }

                bool valido = false;

                while (!valido)
                {
                    valido = true;
                    
                    // Recalcula a data de entrega para que caia em um dia da rota
                    if (dataRota != null && ((idPedido == null && tipoEntrega.GetValueOrDefault(0) != (uint)Pedido.TipoEntregaPedido.Balcao) ||
                        (idPedido > 0 && (tipoEntrega ?? ObtemTipoEntrega(session, idPedido.Value)) != (int)Pedido.TipoEntregaPedido.Balcao)))
                    {
                        if (dataRota < dataEntregaMinima)
                            dataRota = RotaDAO.Instance.GetDataRota(session, idCli, dataEntregaMinima, !considerouDiasUteisSubgrupo);

                        valido = dataRota.Value.Date == dataEntregaMinima.Date;

                        dataEntregaMinima = dataRota.Value.Date;
                    }
                }

                return numeroDiasSomar > 0 || (dataRota != null && dataRota.Value.Date == dataEntregaMinima.Date);
            }
            catch (Exception ex)
            {
                ErroDAO.Instance.InserirFromException("CalculoDataMinima", ex);
                desabilitarCampo = false;
                dataEntregaMinima = dataBase ?? DateTime.Now;
                dataFastDelivery = dataEntregaMinima;
                return false;
            }
        }

        /// <summary>
        /// Recalcula a data de entrega do pedido baseando-se na data passada e atualiza o pedido
        /// </summary>
        public void RecalcularEAtualizarDataEntregaPedido(GDASession session, uint idPedido, DateTime? dataBase, out bool enviarMensagem)
        {
            uint idCliente = PedidoDAO.Instance.ObtemIdCliente(session, idPedido);
            int? tipoPedido = (int?)GetTipoPedido(session, idPedido);
            int? tipoEntrega = (int?)ObtemTipoEntrega(session, idPedido);
            DateTime dataEntregaMinima, dataFastDelivery;
            bool desabilitarCampo;
            var fastDelivey = IsFastDelivery(session, idPedido);
            enviarMensagem = false;

            GetDataEntregaMinima(session, idCliente, idPedido, tipoPedido, tipoEntrega, dataBase, out dataEntregaMinima,
                out dataFastDelivery, out desabilitarCampo, 0, fastDelivey);

            if (dataFastDelivery != DateTime.MinValue && dataEntregaMinima != DateTime.MinValue)
            {
                var dataEntregaAtual = PedidoDAO.Instance.ObtemDataEntrega(session, idPedido);

                if (fastDelivey)
                {
                    if (dataFastDelivery != dataEntregaAtual)
                    {
                        AtualizarDataEntrega(session, (int)idPedido, dataFastDelivery);
                        enviarMensagem = true;
                    }
                }
                else
                {
                    if (dataEntregaMinima != dataEntregaAtual)
                    {
                        AtualizarDataEntrega(session, (int)idPedido, dataEntregaMinima);
                        enviarMensagem = true;
                    }
                }
            }
        }
        #endregion

        #region Altera o desconto do pedido

        private static object _updateDescontoLock = new object();

        public void UpdateDescontoComTransacao(Pedido objUpdate)
        {
            lock(_updateDescontoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        UpdateDesconto(transaction, objUpdate, true);

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
        
        internal void UpdateDesconto(GDASession session, Pedido objUpdate, bool atualizarPedido)
        {
            FilaOperacoes.AtualizarPedido.AguardarVez();

            try
            {
                Pedido ped = GetElementByPrimaryKey(session, objUpdate.IdPedido);
                if (PedidoConfig.LiberarPedido && ped.Situacao == Pedido.SituacaoPedido.Confirmado)
                    throw new Exception("Esse pedido já foi liberado.");

                // Verifica se o desconto que já exista no pedido pode ser mantido pelo usuário que está atualizando o pedido, 
                // tendo em vista que o mesmo possa ter sido lançado por um administrador
                if (objUpdate.Desconto == ped.Desconto && ped.Desconto > 0 && !DescontoPermitido(session, objUpdate.IdPedido))
                    throw new Exception("O desconto lançado anteriormente está acima do permitido para este login.");

                if (objUpdate.IdObra.GetValueOrDefault() > 0 && ObraDAO.Instance.ObtemSituacao(session, objUpdate.IdObra.Value) != Obra.SituacaoObra.Confirmada)
                    throw new Exception("A obra informada não está confirmada.");

                // Não pode modificar o tipo de venda para obra, pois além de não inserir o valor do pagamento antecipado do pedido
                // teria que fazer todas as validações que faz quando o pedido é de obra ao inserir produtos no pedido.
                if (ped.TipoVenda != (int)Pedido.TipoVendaPedido.Obra && objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.Obra &&
                    PedidoConfig.DadosPedido.UsarControleNovoObra)
                    throw new Exception("Não é possível alterar o tipo de venda para obra, é necessário definir o tipo de venda obra antes de inserir produtos no pedido.");
                                
                // Chamado 12792. Não pode modificar o tipo de obra para outro tipo.
                if (ped.TipoVenda == (int)Pedido.TipoVendaPedido.Obra && objUpdate.TipoVenda != (int)Pedido.TipoVendaPedido.Obra &&
                    PedidoConfig.DadosPedido.UsarControleNovoObra)
                    throw new Exception("Não é possível alterar o tipo de venda, como o tipo de venda é Obra o mesmo pode ser alterado somente se o pedido estiver sem produtos.");

                if (objUpdate.IdObra > 0 && objUpdate.Desconto > 0 && PedidoConfig.DadosPedido.UsarControleNovoObra)
                    throw new Exception("Não é permitido lançar desconto em pedidos de obra, pois o valor do m² já foi definido com o cliente.");
                
                /* Chamado 65135. */
                if (FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto && objUpdate.IdFormaPagto.GetValueOrDefault() == 0 &&
                    (objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.AVista || objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.APrazo))
                    throw new Exception("Não é possível atualizar os dados do pedido, pois a forma de pagamento não foi selecionada.");

                if (objUpdate.IdObra > 0 && objUpdate.Acrescimo > 0 && PedidoConfig.DadosPedido.UsarControleNovoObra)
                    throw new Exception("Não é permitido lançar acréscimo em pedidos de obra, pois o valor do m² já foi definido com o cliente.");
                // Valida a alteração no tipo de entrega
                if (ped.TipoEntrega != objUpdate.TipoEntrega)
                {
                    if (OrdemCargaConfig.UsarControleOrdemCarga)
                        if (ped.TipoEntrega == DataSources.Instance.GetTipoEntregaEntrega() &&
                            (TemVolume(session, objUpdate.IdPedido) || PedidoOrdemCargaDAO.Instance.PedidoTemOC(session, objUpdate.IdPedido)))
                            throw new Exception("Não é possível alterar o tipo de entrega desse pedido pois ele já possui volume ou OC gerados. Cancele-os antes de prosseguir.");

                    /* Chamado 48370. */
                    if (ObterIdsPedidoProducaoPeloIdPedidoRevenda(session, (int)objUpdate.IdPedido).Count > 0)
                        objPersistence.ExecuteCommand(session, "UPDATE pedido SET TipoEntrega=?tipoEntrega WHERE IdPedidoRevenda=?idPedidoRevenda",
                            new GDAParameter("?tipoEntrega", objUpdate.TipoEntrega), new GDAParameter("?idPedidoRevenda", objUpdate.IdPedido));
                }
 
                /* Chamado 43090. */
                if (objUpdate.ValorEntrada < 0)
                    throw new Exception("O valor de entrada do pedido não pode ser negativo. Edite o pedido, atualize os valores e tente novamente.");

                if (ped.IdLoja != objUpdate.IdLoja)
                {
                    var idsLojaSubgrupoProd = ProdutosPedidoDAO.Instance.ObterIdsLojaSubgrupoProdPeloPedido(session, (int)objUpdate.IdPedido);
                    
                    if (idsLojaSubgrupoProd.Count > 0 && !idsLojaSubgrupoProd.Contains((int)objUpdate.IdLoja))
                        throw new Exception("Não é possível alterar a loja deste pedido, a loja cadastrada para o subgrupo de um ou mais produtos é diferente da loja selecionada para o pedido.");

                    var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedido(objUpdate.IdPedido);

                    foreach (var prodPed in produtosPedido)
                    {
                        if (GrupoProdDAO.Instance.BloquearEstoque(session, (int)prodPed.IdGrupoProd, (int)prodPed.IdSubgrupoProd))
                        {
                            var estoque = ProdutoLojaDAO.Instance.GetEstoque(session, objUpdate.IdLoja, prodPed.IdProd, null, IsProducao(session, objUpdate.IdPedido), false, true);

                            if (estoque < produtosPedido.Where(f => f.IdProd == prodPed.IdProd).Sum(f => f.Qtde))
                                throw new Exception("O produto " + prodPed.DescrProduto + " possui apenas " + estoque + " em estoque na loja selecionada.");
                        }
                    }
                }

                if ((objUpdate.TipoVenda != ped.TipoVenda || objUpdate.IdFormaPagto != ped.IdFormaPagto || objUpdate.IdParcela != ped.IdParcela) && PedidoConfig.UsarTabelaDescontoAcrescimoPedidoAVista)
                {
                    foreach (var prodPed in ProdutosPedidoDAO.Instance.ObtemProdPedInstByPedido(session, objUpdate.IdPedido))
                    {
                        objPersistence.ExecuteCommand(session, string.Format("update pedido set tipovenda ={0} where idpedido ={1}", objUpdate.TipoVenda, objUpdate.IdPedido));

                        ProdutosPedidoDAO.Instance.RecalcularValores(session, prodPed, objUpdate.IdCli, objUpdate.TipoEntrega.Value, true, (Pedido.TipoVendaPedido)objUpdate.TipoVenda);

                        ProdutosPedidoDAO.Instance.Update(session, prodPed);
                    }
                }

                if (Glass.Configuracoes.ComissaoConfig.ComissaoPorContasRecebidas &&
                    ped.TipoVenda == (int)Pedido.TipoVendaPedido.Obra &&
                    ped.IdObra.GetValueOrDefault() > 0)
                {
                    var idFunc = ObraDAO.Instance.ObtemIdFunc(session, ped.IdObra.Value);
                    var idLojaFunc = FuncionarioDAO.Instance.ObtemIdLoja(session, idFunc);
                    var idCliente = ObraDAO.Instance.ObtemIdCliente(session, (int)ped.IdObra.Value);
                    var idLojaCliente = ClienteDAO.Instance.ObtemIdLoja(session, (uint)idCliente);

                    if (Geral.ConsiderarLojaClientePedidoFluxoSistema && idLojaCliente > 0)
                    {
                        if (idLojaCliente != ped.IdLoja)
                            throw new Exception("A loja da obra informada é diferente da loja do pedido.");
                    }
                    else if (idLojaFunc != ped.IdLoja)
                        throw new Exception("A loja da obra informada é diferente da loja do pedido.");
                }

                // Verifica se este pedido pode ter desconto.
                if (PedidoConfig.Desconto.ImpedirDescontoSomativo && UserInfo.GetUserInfo.TipoUsuario != (uint)Utils.TipoFuncionario.Administrador &&
                    objUpdate.Desconto > 0 && DescontoAcrescimoClienteDAO.Instance.ClientePossuiDesconto(session, ped.IdCli, 0, null, objUpdate.IdPedido, null))
                    throw new Exception("O cliente já possui desconto por grupo/subgrupo, não é permitido lançar outro desconto.");

                /* Chamado 36463.
                 * Em sistemas de confirmação a data de entrega não é exibida na tela e é passada ao método vazia. */
                if (!PedidoConfig.LiberarPedido && !objUpdate.DataEntrega.HasValue)
                    objUpdate.DataEntrega = ped.DataEntrega;

                // Pega a data de entrega da fábrica
                DateTime dataFabrica = PedidoEspelhoDAO.Instance.CalculaDataFabrica(objUpdate.DataEntrega.Value);
                var existeEspelho = PedidoEspelhoDAO.Instance.ExisteEspelho(session, objUpdate.IdPedido);
                var atualizarSomenteDataEntrega = !Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarPedidoConfirmadoListaPedidos) &&
                    !Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarPedidoCofirmadoListaPedidosVendedor) &&
                    Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarDataEntregaPedidoListaPedidos);

                #region Data entrega

                // Altera a data de entrega e data de fábrica da conferência
                if (objUpdate.DataEntrega != null && objUpdate.DataEntrega != ped.DataEntrega)
                {
                    if (!existeEspelho)
                        VerificaCapacidadeProducaoSetor(session, objUpdate.IdPedido, objUpdate.DataEntrega.Value, 0, 0);
                    else
                        PedidoEspelhoDAO.Instance.VerificaCapacidadeProducaoSetor(session, objUpdate.IdPedido, dataFabrica, 0, 0);

                    // Atualiza a data de entrega do pedido
                    objPersistence.ExecuteCommand(session, "Update pedido set dataEntrega=?dataEntrega Where idpedido=" + objUpdate.IdPedido, new GDAParameter("?dataEntrega", objUpdate.DataEntrega.Value));

                    // Salva os dados do Log
                    /* Chamado 53869.
                     * Caso o pedido não seja atualizado, o log não é gerado.
                     * Caso somente a data de entrega seja atualizada, o método é parado ao final dessa region. */
                    if (atualizarSomenteDataEntrega || !atualizarPedido)
                        LogAlteracaoDAO.Instance.LogPedido(session, ped, GetElementByPrimaryKey(session, ped.IdPedido), LogAlteracaoDAO.SequenciaObjeto.Atual);

                    // Atualiza a data de entrega da fábrica
                    objPersistence.ExecuteCommand(session, "Update pedido_espelho Set dataFabrica=?dataFabrica Where idPedido=" + objUpdate.IdPedido, new GDAParameter("?dataFabrica", dataFabrica));

                    if (existeEspelho)
                    {
                        // Atualiza a data de entrega da fábrica.
                        objPersistence.ExecuteCommand(session, "UPDATE pedido_espelho SET DataFabrica=?dataFabrica WHERE IdPedido=" + objUpdate.IdPedido, new GDAParameter("?dataFabrica", dataFabrica));
                        LogAlteracaoDAO.Instance.LogPedidoEspelho(session, PedidoEspelhoDAO.Instance.GetElementByPrimaryKey(session, ped.IdPedido), LogAlteracaoDAO.SequenciaObjeto.Novo);
                    }
                }

                // Altera somente a data de entrega.
                if (atualizarSomenteDataEntrega)
                    return;

                #endregion

                if (ped.DeveTransferir != objUpdate.DeveTransferir && PedidoOrdemCargaDAO.Instance.PedidoTemOC(session, ped.IdPedido))
                {
                    throw new Exception("Não é possível alterar a opção Deve Transferir desse pedido pois ele já esta vinculado a uma OC.");
                }

                if (ped.IdLoja != objUpdate.IdLoja && PedidoOrdemCargaDAO.Instance.PedidoTemOC(session, ped.IdPedido))
                {
                    throw new Exception("Não é possível alterar a loja desse pedido pois ele já esta vinculado a uma OC.");
                }

                bool desconto = ((!atualizarPedido && objUpdate.Desconto > 0) ||
                    (atualizarPedido && (ped.TipoDesconto != objUpdate.TipoDesconto || ped.Desconto != objUpdate.Desconto))) && PedidoConfig.LiberarPedido;
                bool acrescimo = ((!atualizarPedido && objUpdate.Acrescimo > 0) ||
                    (atualizarPedido && (ped.TipoAcrescimo != objUpdate.TipoAcrescimo || ped.Acrescimo != objUpdate.Acrescimo))) && PedidoConfig.LiberarPedido;
                
                // Recupera a obra ainda com as informações do pedido, para que o log dela seja criado corretamente.
                var obraAtual = ped.IdObra > 0 ? ObraDAO.Instance.GetElement(session, ped.IdObra.Value) : null;
                var obraNova = objUpdate.IdObra > 0 ? ObraDAO.Instance.GetElement(session, objUpdate.IdObra.Value) : null;

                var isPedidoProducao = PedidoDAO.Instance.IsProducao(objUpdate.IdPedido);

                var idPedidoRevenda = ObterIdPedidoRevenda(null, (int)objUpdate.IdPedido);

                // Caso o tipo de entrega tenha sido alterado e o pedido seja de produção vinculado a um de revenda atualiza o tipo entrega
                //do pedido de revenda tambem, caso o mesmo não esteja liberado
                if (objUpdate.TipoEntrega != (int)ObtemTipoEntrega(null, objUpdate.IdPedido) && isPedidoProducao && idPedidoRevenda > 0)
                {
                    var situacaoPedidoRevenda = PedidoDAO.Instance.ObtemSituacao((uint)idPedidoRevenda.Value);

                    if (situacaoPedidoRevenda == Pedido.SituacaoPedido.Confirmado || situacaoPedidoRevenda == Pedido.SituacaoPedido.LiberadoParcialmente)
                    {
                        throw new Exception("Não é possível alterar o tipo de entrega desse pedido pois o pedido de revenda já está liberado");
                    }

                    //Atualiza o tipo de entrega do pedido de revenda
                    objPersistence.ExecuteCommand(session, string.Format("Update pedido Set TipoEntrega={0} Where idPedido={1}", (int)objUpdate.TipoEntrega, idPedidoRevenda.Value));
                }

               

                if (atualizarPedido)
                {
                    if (desconto)
                    {
                        RemoveDesconto(session, objUpdate.IdPedido, ped.TipoDesconto, ped.Desconto, null, null);
                        AplicaDesconto(session, objUpdate.IdPedido, objUpdate.TipoDesconto, objUpdate.Desconto, false, false);
                    }

                    if (acrescimo)
                    {
                        RemoveAcrescimo(session, objUpdate.IdPedido, ped.TipoAcrescimo, ped.Acrescimo, null, null);
                        AplicaAcrescimo(session, objUpdate.IdPedido, objUpdate.TipoAcrescimo, objUpdate.Acrescimo, false);
                    }

                    if (objUpdate.Desconto != ped.Desconto && PedidoConfig.Desconto.GetDescontoMaximoPedido(session, UserInfo.GetUserInfo.CodUser, (int)objUpdate.TipoVenda) != 100)
                    {
                        objUpdate.IdFuncDesc = null;
                        objPersistence.ExecuteCommand(session, "Update pedido Set idFuncDesc=Null Where idPedido=" + objUpdate.IdPedido);
                    }

                    if (!DescontoPermitido(session, objUpdate.IdPedido))
                    {
                        RemoveDesconto(session, objUpdate.IdPedido);
                        objUpdate.Desconto = 0;
                    }

                    objUpdate.ObsLiberacao = String.IsNullOrEmpty(objUpdate.ObsLiberacao) ? null :
                        objUpdate.ObsLiberacao.Length > 300 ? objUpdate.ObsLiberacao.Substring(0, 297) + "..." : objUpdate.ObsLiberacao;
                    
                    // Estas duas primeiras condições vão de acordo com a detail view selecionada para ser alterada no popup de desconto de pedido,
                    // alterando-as deve alterar nesta tela também (Utils/DescontoPedido.aspx.cs)
                    bool alterarSomenteObsDesconto =
                        (UserInfo.GetUserInfo.TipoUsuario == (uint)Utils.TipoFuncionario.Vendedor &&
                        !Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarPedidoCofirmadoListaPedidosVendedor)) ||

                        (PedidoConfig.DescontoPedidoVendedorUmProduto && UserInfo.GetUserInfo.CodUser == objUpdate.IdFunc &&
                        UserInfo.GetUserInfo.TipoUsuario == (uint)Utils.TipoFuncionario.Vendedor &&
                        !Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarPedidoConfirmadoListaPedidos) &&
                        !Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarPedidoCofirmadoListaPedidosVendedor));

                    bool alterarVendedor =
                        Config.PossuiPermissao(Config.FuncaoMenuPedido.AlterarVendedorPedido);

                    //Busca o idCliente do pedido
                    var idClientePedido = GetIdCliente(objUpdate.IdPedido);

                    // Verifica se o código do pedido do cliente já foi cadastrado
                    if (objUpdate.TipoVenda < 3)
                        CodigoClienteUsado(session, objUpdate.IdPedido, idClientePedido, objUpdate.CodCliente, false);

                    string sql =
                        !PedidoConfig.LiberarPedido ?
                            "update pedido set obs=?obs, percentualComissao=?percentualComissao Where idPedido=" + objUpdate.IdPedido :

                        !alterarSomenteObsDesconto ?
                            @"update pedido set tipoDesconto=?tipoDesc, desconto=?desc,
                            tipoAcrescimo=?tipoAcr, acrescimo=?acr, obs=?obs, tipoVenda=?tipoVenda, fastDelivery=?fast, 
                            idParcela=?idParc, numParc=?numParc, valorEntrada=?entrada, idFormaPagto=?fp, idTipoCartao=?tp, obsLiberacao=?ol, 
                            idFunc=?idFunc, idFuncVenda=?idFuncVenda, tipoEntrega=?tipoEntrega, percentualComissao=?percentualComissao, idObra=?o, 
                            deveTransferir=?deveTransferir, idLoja=?idLoja, CodCliente=?codCliente, OrdemCargaParcial=?ocParcial Where idPedido=" + objUpdate.IdPedido :

                        /* Chamado 22610. */
                        alterarVendedor ?
                            @"Update pedido SET IdFunc=?idFunc, TipoDesconto=?tipoDesc, Desconto=?desc, Obs=?obs, ObsLiberacao=?ol WHERE IdPedido=" + objUpdate.IdPedido :

                            @"update pedido set tipoDesconto=?tipoDesc, desconto=?desc, obs=?obs, obsLiberacao=?ol where idPedido=" + objUpdate.IdPedido;

                    if (!alterarSomenteObsDesconto && objUpdate.TipoVenda.GetValueOrDefault() == 0)
                    {
                        // Se o tipo de venda for nulo então o tipo de venda não estava disponível para ser selecionado na tela, sendo assim o tipo de venda deve ser mantido.
                        if (objUpdate.TipoVenda == null)
                        { 
                            objUpdate.TipoVenda = ObtemTipoVenda(session, objUpdate.IdPedido);
                            
                            //Chamado 37640 : se o pedido tiver aberto em uma segunda tela de alterações o sistema estava mantendo o idobra e atualizando em cima de outra atualização. essa alteração impede que isso ocorra
                            if (objUpdate.TipoVenda != (int)Pedido.TipoVendaPedido.Obra)
                                objUpdate.IdObra = null;
                        }

                        if (objUpdate.TipoVenda.GetValueOrDefault() == 0)
                            throw new Exception("Informe o tipo de venda");
                    }
                    
                    //Se o pagamento era obra e nao é mais remove as referencias da obra
                    if (ped.TipoVenda == (int)Pedido.TipoVendaPedido.Obra && objUpdate.TipoVenda != (int)Pedido.TipoVendaPedido.Obra)
                    {
                        objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido SET IdObra=NULL, ValorPagamentoAntecipado=NULL WHERE IdPedido={0}", objUpdate.IdPedido));

                        /* Chamado 51738.
                         * O ID da obra estava sendo removido do pedido através do SQL acima, mas no SQL abaixo ele era adicionado novamente. */
                        objUpdate.IdObra = null;

                        if (ped.IdObra > 0)
                            ObraDAO.Instance.AtualizaSaldo(session, obraAtual, obraAtual.IdObra, false, false);
                    }

                    if(ped.IdTransportador != objUpdate.IdTransportador)
                        objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido SET IdTransportador={0} WHERE IdPedido={1}", objUpdate.IdTransportador, objUpdate.IdPedido));

                    // Atualiza os dados do pedido
                    objPersistence.ExecuteCommand(session, sql,
                        new GDAParameter("?tipoDesc", objUpdate.TipoDesconto),
                        new GDAParameter("?desc", objUpdate.Desconto),
                        new GDAParameter("?tipoAcr", objUpdate.TipoAcrescimo),
                        new GDAParameter("?acr", objUpdate.Acrescimo),
                        new GDAParameter("?obs", objUpdate.Obs),
                        new GDAParameter("?tipoVenda", objUpdate.TipoVenda),
                        new GDAParameter("?idFunc", objUpdate.IdFunc),
                        new GDAParameter("?tipoEntrega", objUpdate.TipoEntrega),
                        new GDAParameter("?fast", objUpdate.FastDelivery),
                        new GDAParameter("?idParc", objUpdate.IdParcela),
                        new GDAParameter("?numParc", objUpdate.NumParc),
                        new GDAParameter("?entrada", objUpdate.ValorEntrada),
                        new GDAParameter("?fp", objUpdate.IdFormaPagto),
                        new GDAParameter("?tp", objUpdate.IdTipoCartao),
                        new GDAParameter("?ol", objUpdate.ObsLiberacao),
                        new GDAParameter("?percentualComissao", objUpdate.PercentualComissao),
                        new GDAParameter("?o",  objUpdate.IdObra),
                        new GDAParameter("?deveTransferir", objUpdate.DeveTransferir),
                        new GDAParameter("?idFuncVenda", objUpdate.IdFuncVenda),
                        new GDAParameter("?idLoja", objUpdate.IdLoja),
                        new GDAParameter("?codCliente", objUpdate.CodCliente),
                        new GDAParameter("?ocParcial", objUpdate.OrdemCargaParcial));

                    if (!objUpdate.FastDelivery)
                        objPersistence.ExecuteCommand(session, "update pedido set taxaFastDelivery=0 Where idPedido=" + objUpdate.IdPedido);
                    
                    if (ped.IdLoja != objUpdate.IdLoja)
                    {
                        if (!ped.Producao)
                        {
                            var idsProdQtde = new Dictionary<int, float>();

                            foreach (var prodPed in ProdutosPedidoDAO.Instance.GetByPedido(session, ped.IdPedido))
                            {
                                var tipoCalc = GrupoProdDAO.Instance.TipoCalculo(session, (int)prodPed.IdProd);
                                var m2 = tipoCalc == (int)TipoCalculoGrupoProd.M2 ||
                                    tipoCalc == (int)TipoCalculoGrupoProd.M2Direto;

                                var qtdEstornoEstoque = prodPed.Qtde;

                                if (tipoCalc == (int)TipoCalculoGrupoProd.MLAL0 || tipoCalc == (int)TipoCalculoGrupoProd.MLAL05 ||
                                    tipoCalc == (int)TipoCalculoGrupoProd.MLAL1 || tipoCalc == (int)TipoCalculoGrupoProd.MLAL6)
                                {
                                    qtdEstornoEstoque = prodPed.Qtde * prodPed.Altura;
                                }

                                // Salva produto e qtd de saída para executar apenas um sql de atualização de estoque
                           
                                if (!idsProdQtde.ContainsKey((int)prodPed.IdProd))
                                    idsProdQtde.Add((int)prodPed.IdProd, m2 ? prodPed.TotM : qtdEstornoEstoque);
                                else
                                    idsProdQtde[(int)prodPed.IdProd] += m2 ? prodPed.TotM : qtdEstornoEstoque;
                            
                            }

                            ProdutoLojaDAO.Instance.ColocarReserva(session, (int)objUpdate.IdLoja, idsProdQtde, null, null, null, null,
                                (int)ped.IdPedido, null, null, "PedidoDAO - UpdateDesconto");
                            ProdutoLojaDAO.Instance.TirarReserva(session, (int)ped.IdLoja, idsProdQtde, null, null, null, null,
                                (int)ped.IdPedido, null, null, "PedidoDAO - UpdateDesconto");
                        }
                    }
                        
                    // Salva os dados do Log
                    LogAlteracaoDAO.Instance.LogPedido(session, ped, GetElementByPrimaryKey(session, ped.IdPedido), LogAlteracaoDAO.SequenciaObjeto.Atual);
                }

                //Atualiza o valor do frete
                objPersistence.ExecuteCommand(session, "Update pedido Set ValorEntrega=?ValorEntrega Where idPedido=" + objUpdate.IdPedido, new GDAParameter("?ValorEntrega", objUpdate.ValorEntrega));

                UpdateTotalPedido(session, objUpdate.IdPedido, false, false, desconto);

                // Atualiza os dados do pedido espelho
                if (existeEspelho)
                {
                    if (desconto)
                    {
                        PedidoEspelhoDAO.Instance.RemoveDesconto(session, objUpdate.IdPedido);
                        PedidoEspelhoDAO.Instance.AplicaDesconto(session, objUpdate.IdPedido, objUpdate.TipoDesconto, objUpdate.Desconto);
                    }

                    if (acrescimo)
                    {
                        PedidoEspelhoDAO.Instance.RemoveAcrescimo(session, objUpdate.IdPedido);

                        // Aplica acréscimo nos clones, desde que existam (MUITO IMPORTANTE APLICAR ANTES DE APLICAR NO ESPELHO LOGO ABAIXO, 
                        // porque o "BENEFICIAMENTOS" da model de pedidos está buscando os beneficiamentos do produtos_pedido_espelho, que da forma 
                        // como está agora ainda não tem acréscimo aplicado)
                        if (ExecuteScalar<bool>(session, "Select Count(*)>0 From produtos_pedido where idPedido=" + objUpdate.IdPedido + " And (invisivelfluxo=1 Or invisivelpedido=1)"))
                            AplicaAcrescimo(session, objUpdate.IdPedido, objUpdate.TipoAcrescimo, objUpdate.Acrescimo, true);

                        PedidoEspelhoDAO.Instance.AplicaAcrescimo(session, objUpdate.IdPedido, objUpdate.TipoAcrescimo, objUpdate.Acrescimo);
                    }

                    objPersistence.ExecuteCommand(session, @"update pedido_espelho set tipoDesconto=?tipo, desconto=?desc, 
                    tipoAcrescimo=?tipoAcr, acrescimo=?acr where idPedido=" + objUpdate.IdPedido,
                        new GDAParameter("?tipo", objUpdate.TipoDesconto), new GDAParameter("?desc", objUpdate.Desconto),
                        new GDAParameter("?tipoAcr", objUpdate.TipoAcrescimo), new GDAParameter("?acr", objUpdate.Acrescimo));

                    //Atualiza o valor do frete
                    objPersistence.ExecuteCommand(session, "Update pedido_espelho Set ValorEntrega=?ValorEntrega Where idPedido=" + objUpdate.IdPedido,
                            new GDAParameter("?ValorEntrega", objUpdate.ValorEntrega));

                    PedidoEspelhoDAO.Instance.UpdateTotalPedido(session, objUpdate.IdPedido);
                }

                var totalPedidoAtual = GetTotal(session, objUpdate.IdPedido);

                if (objUpdate.ValoresParcelas != null)
                {
                    /* Chamado 37155. */
                    objUpdate.Total = totalPedidoAtual;
                    RecalculaParcelas(session, ref objUpdate, TipoCalculoParcelas.Valor);
                    SalvarParcelas(session, objUpdate);
                }

                // O saldo da obra sempre deve ser atualizado, pois, caso seja aplicado um desconto no pedido o valor total será recalculado
                // e, da mesma forma, o valor do pagamento antecipado também deve ser recalculado.
                // Se o pedido passou a ser de obra, atualiza os dados do mesmo.
                // Verifica se o tipo de venda do pedido foi alterado para obra nesta atualização.
                if (ped.TipoVenda != (int)Pedido.TipoVendaPedido.Obra && objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.Obra)
                    AtualizaSaldoObra(session, objUpdate.IdPedido, obraNova, obraNova.IdObra, totalPedidoAtual, 0, false);
                else if (objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.Obra)
                {
                    /* Chamado 51738.
                     * Verifica se a obra associada ao pedido foi alterada. */
                    if (ped.IdObra > 0 && objUpdate.IdObra > 0 && ped.IdObra != objUpdate.IdObra)
                    {
                        // Atualiza o saldo da obra atual para que os valores fiquem corretos.
                        ObraDAO.Instance.AtualizaSaldo(session, obraAtual, obraAtual.IdObra, false, false);
                        // Atualiza o saldo da obra nova para que os valores fiquem corretos.
                        AtualizaSaldoObra(session, objUpdate.IdPedido, obraNova, obraNova.IdObra, totalPedidoAtual, 0, false);
                    }
                    /* Chamado 63233. */
                    else
                        AtualizaSaldoObra(session, objUpdate.IdPedido, obraNova, obraNova.IdObra, totalPedidoAtual, ped.Total, true);
                }
            }
            finally
            {
                FilaOperacoes.AtualizarPedido.ProximoFila();
            }
        }

        #endregion

        #region Recupera o ID da obra de um pedido

        public uint? GetIdObra(uint idPedido)
        {
            return GetIdObra(null, idPedido);
        }

        /// <summary>
        /// Recupera o ID da obra de um pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public uint? GetIdObra(GDASession sessao, uint idPedido)
        {
            object retorno = objPersistence.ExecuteScalar(sessao, "select idObra from pedido where idPedido=" + idPedido);
            return retorno != null && retorno != DBNull.Value && retorno.ToString() != "" ? (uint?)Glass.Conversoes.StrParaUint(retorno.ToString()) : null;
        }

        #endregion

        #region Recupera o ID do cliente de um pedido

        /// <summary>
        /// Recupera o ID do cliente de um pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public uint GetIdCliente(uint idPedido)
        {
            return GetIdCliente(null, idPedido);
        }

        /// <summary>
        /// Recupera o ID do cliente de um pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public uint GetIdCliente(GDASession sessao, uint idPedido)
        {
            object retorno = objPersistence.ExecuteScalar(sessao, "select coalesce(idCli,0) from pedido where idPedido=" + idPedido);
            return retorno != null && retorno != DBNull.Value && retorno.ToString() != "" ? Glass.Conversoes.StrParaUint(retorno.ToString()) : 0;
        }

        #endregion

        #region Altera os dados do pedido de um parceiro

        /// <summary>
        /// Altera os dados do pedido de um parceiro.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <param name="codPedCli"></param>
        /// <param name="valorEntrada"></param>
        /// <param name="obs"></param>
        public void UpdateParceiro(uint idPedido, string codPedCli, string valorEntrada, string obs, string obsLib, int? idTransportador)
        {
            UpdateParceiro(null, idPedido, codPedCli, valorEntrada, obs, obsLib, idTransportador);
        }

        /// <summary>
        /// Altera os dados do pedido de um parceiro.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idPedido"></param>
        /// <param name="codPedCli"></param>
        /// <param name="valorEntrada"></param>
        /// <param name="obs"></param>
        public void UpdateParceiro(GDASession sessao, uint idPedido, string codPedCli, string valorEntrada, string obs, string obsLib, int? idTransportador)
        {
            string sql = "update pedido set codCliente=?codPedCli, obs=?obs, ObsLiberacao=?obsLib{0}, IdTransportador=?idTransp where idPedido=" + idPedido;

            var lstParam = new List<GDAParameter>();
            lstParam.Add(new GDAParameter("?codPedCli", codPedCli));
            lstParam.Add(new GDAParameter("?obs", obs));
            lstParam.Add(new GDAParameter("?obsLib", obsLib));
            lstParam.Add(new GDAParameter("?idTransp", idTransportador));

            if (!String.IsNullOrEmpty(valorEntrada))
            {
                sql = String.Format(sql, ", valorEntrada=?valorEntrada");
                lstParam.Add(new GDAParameter("?valorEntrada", valorEntrada));
            }
            else
                sql = String.Format(sql, String.Empty);

            objPersistence.ExecuteCommand(sessao, sql, lstParam.ToArray());
        }

        /// <summary>
        /// Gera parcelas padrão do cliente parceiro
        /// </summary>
        /// <param name="ped"></param>
        public void GeraParcelaParceiro(ref Pedido ped)
        {
            GeraParcelaParceiro(null, ref ped);
        }

        /// <summary>
        /// Gera parcelas padrão do cliente parceiro
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="ped"></param>
        public void GeraParcelaParceiro(GDASession sessao, ref Pedido ped)
        {
            Parcelas parc = ParcelasDAO.Instance.GetPadraoCliente(sessao, ped.IdCli);

            if (parc != null && parc.NumParcelas > 0)
            {
                ped.NumParc = parc.NumParcelas;
                ped.ValoresParcelas = new decimal[parc.NumParcelas];
                ped.DatasParcelas = new DateTime[parc.NumParcelas];

                decimal valorParc = Math.Round((ped.Total - ped.ValorEntrada) / parc.NumParcelas, 2);
                decimal soma = 0;
                for (int i = 0; i < parc.NumParcelas; i++)
                {
                    ped.ValoresParcelas[i] = i < parc.NumParcelas - 1 ? valorParc : (ped.Total - ped.ValorEntrada) - soma;
                    ped.DatasParcelas[i] = DateTime.Now.AddDays(parc.NumeroDias[i]);
                    soma += valorParc;
                }

                ParcelasPedidoDAO.Instance.DeleteFromPedido(sessao, ped.IdPedido);

                if (ped.ValoresParcelas.Length > 0 && ped.ValoresParcelas[0] > 0)
                    for (int i = 0; i < ped.NumParc; i++)
                    {
                        ParcelasPedido parcela = new ParcelasPedido();
                        parcela.IdPedido = ped.IdPedido;
                        parcela.Valor = ped.ValoresParcelas[i];
                        parcela.Data = ped.DatasParcelas[i];
                        ParcelasPedidoDAO.Instance.Insert(sessao, parcela);
                    }
            }
        }

        #endregion

        #region Verifica Código Cliente

        /// <summary>
        /// Verifica se o código do cliente passado já foi usado em outro pedido que não esteja cancelado
        /// </summary>
        /// <returns></returns>
        public bool CodigoClienteUsado(GDASession sessao, uint idPedido, uint idCliente, string codCliente, bool verificarSempre)
        {
            Pedido.TipoPedidoEnum tipoPedido = GetTipoPedido(sessao, idPedido);
            if (tipoPedido == Pedido.TipoPedidoEnum.Producao)
                return false;

            string sqlVerifica = "Select Count(*) From pedido Where codCliente=?codCliente And situacao<>" +
                (int)Pedido.SituacaoPedido.Cancelado + " And idCli=" + idCliente + (idPedido > 0 ? " And idPedido<>" + idPedido : "");

            string sqlBuscaPedido = "Select idPedido From pedido Where codCliente=?codCliente And idCli=" + idCliente + " limit 1";

            if ((verificarSempre || PedidoConfig.CodigoClienteUsado) && !String.IsNullOrEmpty(codCliente) &&
                Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sessao, sqlVerifica, new GDAParameter("?codCliente", codCliente)).ToString()) > 0)
            {
                if (!verificarSempre)
                {
                    uint idPedidoUsado = Glass.Conversoes.StrParaUint(objPersistence.ExecuteScalar(sessao, sqlBuscaPedido,
                        new GDAParameter("?codCliente", codCliente)).ToString());

                    throw new Exception("O código: " + codCliente + " de pedido do cliente já foi utilizado no pedido " + idPedidoUsado + ".");
                }
                else
                    return true;
            }

            return false;
        }

        #endregion

        #region Recupera o tipo de venda do pedido

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Recupera o tipo de venda do pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int? GetTipoVenda(uint idPedido)
        {
            return GetTipoVenda(null, idPedido);
        }

        /// <summary>
        /// Recupera o tipo de venda do pedido.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public int? GetTipoVenda(GDASession sessao, uint idPedido)
        {
            string sql = "select tipoVenda from pedido where idPedido=" + idPedido;
            object retorno = objPersistence.ExecuteScalar(sessao, sql);
            return retorno != null && retorno != DBNull.Value && retorno.ToString() != "" ? (int?)Glass.Conversoes.StrParaInt(retorno.ToString()) : null;
        }

        #endregion

        #region Verifica se o pedido tem sinal/pagamento antecipado a receber

        /// <summary>
        /// Verifica se o pedido tem sinal/pagamento antecipado a receber.
        /// </summary>
        /// <param name="ped"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool VerificaSinalPagamentoReceber(Pedido ped, out string mensagemErro)
        {
            return VerificaSinalPagamentoReceber(null, ped, out mensagemErro);
        }

        /// <summary>
        /// Verifica se o pedido tem sinal/pagamento antecipado a receber.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="ped"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool VerificaSinalPagamentoReceber(GDASession sessao, Pedido ped, out string mensagemErro)
        {
            var totalPedido = PedidoEspelhoDAO.Instance.ObtemTotal(sessao, ped.IdPedido);
            mensagemErro = string.Empty;

            if (totalPedido == 0)
                totalPedido = ped.Total;

            // Se for reposição ou garantia, não deve verificar se possui sinal ou pagto antecipado pendente.
            if (ped.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição || ped.TipoVenda == (int)Pedido.TipoVendaPedido.Garantia ||
                // Verifica se o pagamento antes da produção deverá ser efetuado de acordo com o tipo do pedido e configuração interna.
                PedidoConfig.NaoObrigarPagamentoAntesProducaoParaPedidoRevenda((Pedido.TipoPedidoEnum)ped.TipoPedido))
                return true;

            /* Chamado 48262. */
            if (ped.TipoPedido == (int)Pedido.TipoPedidoEnum.Producao)
                return true;

            // Verifica se o pedido tem sinal a receber
            if (TemSinalReceber(sessao, ped.IdPedido))
            {
                mensagemErro = "O pedido " + ped.IdPedido + " tem um sinal de " + ped.ValorEntrada.ToString("C") + " a receber.";
                return false;
            }
            // Verifica se o pedido tem pagamento antecipado a receber, desde que já não tenha recebido sinal 
            // (Alterado para ser um ou outro, conforme era na versão 4.1, a pedido da Dekor)
            else if (!ped.RecebeuSinal && TemPagamentoAntecipadoReceber(sessao, ped.IdPedido))
            {
                mensagemErro = "O pedido " + ped.IdPedido + " deve ser pago antecipadamente.";
                return false;
            }
            // Chamado 15570: Verifica se o pedido foi totalmente pago antecipado, caso o cliente esteja obrigado a pagar
            else if (ped.IdPagamentoAntecipado > 0 && ClienteDAO.Instance.IsPagamentoAntesProducao(ped.IdCli) && 
                totalPedido > ped.ValorPagamentoAntecipado + (ped.IdSinal > 0 ? ped.ValorEntrada : 0))
            {
                mensagemErro = "O pedido " + ped.IdPedido + " deve ser totalmente pago antecipadamente, apenas uma parte do mesmo foi paga.";
                return false;
            }

            return true;
        }

        /// <summary>
        /// Verifica se os pedidos tem sinal/pagamento antecipado a receber.
        /// </summary>
        /// <param name="idsPedidos"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool VerificaSinalPagamentoReceber(string idsPedidos, out string mensagemErro)
        {
            return VerificaSinalPagamentoReceber(null, idsPedidos, out mensagemErro);
        }

        /// <summary>
        /// Verifica se os pedidos tem sinal/pagamento antecipado a receber.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idsPedidos"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool VerificaSinalPagamentoReceber(GDASession sessao, string idsPedidos, out string mensagemErro)
        {
            return VerificaSinalPagamentoReceber(sessao, GetByString(null, idsPedidos), out mensagemErro);
        }

        /// <summary>
        /// Verifica se os pedidos tem sinal/pagamento antecipado a receber.
        /// </summary>
        /// <param name="pedidos"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool VerificaSinalPagamentoReceber(IEnumerable<Pedido> pedidos, out string mensagemErro)
        {
            return VerificaSinalPagamentoReceber(null, pedidos, out mensagemErro);
        }

        /// <summary>
        /// Verifica se os pedidos tem sinal/pagamento antecipado a receber.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="pedidos"></param>
        /// <param name="mensagemErro"></param>
        /// <returns></returns>
        public bool VerificaSinalPagamentoReceber(GDASession sessao, IEnumerable<Pedido> pedidos, out string mensagemErro)
        {
            List<uint> temp1 = new List<uint>(), temp2 = new List<uint>();
            return VerificaSinalPagamentoReceber(sessao, pedidos, out mensagemErro, out temp1, out temp2);
        }

        /// <summary>
        /// Verifica se os pedidos tem sinal/pagamento antecipado a receber.
        /// </summary>
        /// <param name="pedidos"></param>
        /// <param name="mensagemErro"></param>
        /// <param name="idsPedidosOk"></param>
        /// <param name="idsPedidosErro"></param>
        /// <returns></returns>
        public bool VerificaSinalPagamentoReceber(IEnumerable<Pedido> pedidos, out string mensagemErro,
            out List<uint> idsPedidosOk, out List<uint> idsPedidosErro)
        {
            return VerificaSinalPagamentoReceber(null, pedidos, out mensagemErro, out idsPedidosOk, out idsPedidosErro);
        }

        /// <summary>
        /// Verifica se os pedidos tem sinal/pagamento antecipado a receber.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="pedidos"></param>
        /// <param name="mensagemErro"></param>
        /// <param name="idsPedidosOk"></param>
        /// <param name="idsPedidosErro"></param>
        /// <returns></returns>
        public bool VerificaSinalPagamentoReceber(GDASession sessao, IEnumerable<Pedido> pedidos, out string mensagemErro,
            out List<uint> idsPedidosOk, out List<uint> idsPedidosErro)
        {
            string idsSinal = "";
            string idsPagtoAntecipado = "";
            idsPedidosOk = new List<uint>();
            idsPedidosErro = new List<uint>();

            // Verifica, em cada pedido, se há sinal/pagamento antecipado a receber
            foreach (Pedido p in pedidos)
            {
                string erro = "";
                if (!VerificaSinalPagamentoReceber(sessao, p, out erro))
                {
                    if (erro.IndexOf("sinal") > -1)
                        idsSinal += p.IdPedido + ", ";
                    else
                        idsPagtoAntecipado += p.IdPedido + ", ";

                    idsPedidosErro.Add(p.IdPedido);
                }
                else
                    idsPedidosOk.Add(p.IdPedido);
            }

            idsSinal = idsSinal.TrimEnd(',', ' ');
            idsPagtoAntecipado = idsPagtoAntecipado.TrimEnd(',', ' ');

            mensagemErro = "";
            if (!String.IsNullOrEmpty(idsSinal))
            {
                bool plural = idsSinal.IndexOf(',') > -1;
                mensagemErro += String.Format("O{1} pedido{1} {0} tem sinal a receber.\n",
                    idsSinal, plural ? "s" : "");
            }

            if (!String.IsNullOrEmpty(idsPagtoAntecipado))
            {
                bool plural = idsPagtoAntecipado.IndexOf(',') > -1;
                mensagemErro += String.Format("O{1} pedido{1} {0} deve{2} ser pago{1} antecipadamente.\n",
                    idsPagtoAntecipado, plural ? "s" : "", plural ? "m" : "");
            }

            mensagemErro = mensagemErro.TrimEnd('\n');
            return String.IsNullOrEmpty(mensagemErro);
        }

        #endregion

        #region Verifica o número de pedidos atrasados (bloqueio de emissão de pedido)

        private string SqlBloqueioEmissao(uint idCliente, uint idPedido, bool getClientes, bool selecionar)
        {
            string campos = getClientes ? "ped.idCli" : selecionar ? "ped.idPedido" : "count(distinct ped.idPedido)";

            string sql = "select " + campos + @"
                from pedido ped
                    " + (getClientes ? "inner join cliente c on (ped.idCli=c.id_Cli)" : "") + @"
                where date(ped.dataEntrega)<date(now())
                    and ped.situacao not in (" + (int)Pedido.SituacaoPedido.Cancelado + "," + (int)Pedido.SituacaoPedido.Confirmado + @") 
                    and ped.tipoPedido not in (" + (int)Pedido.TipoPedidoEnum.Producao + @")
                    and ped.dataPronto<date_sub(now(), interval " + PedidoConfig.NumeroDiasPedidoProntoAtrasado + @" day)";

            if (!getClientes)
            {
                if (idCliente > 0)
                {
                    // Não retorna nenhum pedido para bloqueio se o pedido possuir pagamento antecipado já pago
                    sql += " and ped.idSinal is null and coalesce(ped.valorPagamentoAntecipado,0)=0 and ped.idCli=" + idCliente;
                }

                if (idPedido > 0)
                    sql += " and ped.idPedido=" + idPedido;
            }
            else
                sql += " and lower(c.nome)<>'consumidor final' and c.situacao=" + (int)SituacaoCliente.Ativo;

            return sql;
        }

        public int GetCountBloqueioEmissao(uint idCliente)
        {
            return GetCountBloqueioEmissao(null, idCliente);
        }

        public int GetCountBloqueioEmissao(GDASession session, uint idCliente)
        {
            return PedidoConfig.NumeroDiasPedidoProntoAtrasado == 0 ||
                ClienteDAO.Instance.IgnorarBloqueioPedidoPronto(session, idCliente) ? 0 :
                objPersistence.ExecuteSqlQueryCount(session, SqlBloqueioEmissao(idCliente, 0, false, false));
        }

        public string GetIdsBloqueioEmissao(uint idCliente)
        {
            return GetIdsBloqueioEmissao(null, idCliente);
        }

        public string GetIdsBloqueioEmissao(GDASession session, uint idCliente)
        {
            return PedidoConfig.NumeroDiasPedidoProntoAtrasado == 0 ||
                ClienteDAO.Instance.IgnorarBloqueioPedidoPronto(session, idCliente) ? "" :
                GetValoresCampo(session, SqlBloqueioEmissao(idCliente, 0, false, true), "idPedido");
        }

        public uint[] GetClientesAtrasados()
        {
            if (PedidoConfig.NumeroDiasPedidoProntoAtrasado == 0)
                return new uint[0];

            string temp = GetValoresCampo(SqlBloqueioEmissao(0, 0, true, true), "idCli");

            if (String.IsNullOrEmpty(temp))
                return new uint[0];

            List<uint> retorno = new List<uint>();
            foreach (string s in temp.Split(','))
            {
                uint i;
                if (uint.TryParse(s, out i) && !ClienteDAO.Instance.IgnorarBloqueioPedidoPronto(i))
                    retorno.Add(i);
            }

            return retorno.ToArray();
        }

        public Pedido[] GetPedidosBloqueioEmissaoByCliente(uint idCliente)
        {
            if (idCliente > 0)
            {
                string idsPedidos = GetIdsBloqueioEmissao(idCliente);

                if (string.IsNullOrEmpty(idsPedidos))
                    idsPedidos = "0";

                bool temFiltro;
                string filtroAdicional;

                return objPersistence.LoadData(Sql(0, 0, idsPedidos, null, 0, idCliente, null, 0, null, 0, null, null, null, null, null, null,
                    null, null, null, null, null, null, null, null, null, 0, false, false, 0, 0, 0, 0, 0, null, 0, 0, 0, "", true, out filtroAdicional,
                    out temFiltro).Replace("?filtroAdicional?", filtroAdicional)).ToArray();
            }
            else
            {
                return new Pedido[0];
            }
        }

        #endregion

        #region Verifica se o pedido está atrasado pela data de entrega

        /// <summary>
        /// Verifica se o pedido está atrasado pela data de entrega.
        /// </summary>
        public bool IsPedidoAtrasado(uint idPedido, bool forLiberacao)
        {
            return IsPedidoAtrasado(null, idPedido, forLiberacao);
        }

        /// <summary>
        /// Verifica se o pedido está atrasado pela data de entrega.
        /// </summary>
        public bool IsPedidoAtrasado(GDASession session, uint idPedido, bool forLiberacao)
        {
            if (forLiberacao && !Liberacao.DadosLiberacao.LiberarPedidoAtrasadoParcialmente)
                return false;

            string sql = "select count(*) from pedido where dataEntrega<=date(now()) and idPedido=" + idPedido;
            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        #endregion

        #region Verifica se o pedido possui parcela cadastrada

        /// <summary>
        /// Verifica se o pedido possui parcela cadastrada
        /// </summary>
        /// <param name="idsPedido"></param>
        /// <returns></returns>
        public bool PossuiParcela(uint idPedido)
        {
            string sql = "Select Count(*) From parcelas_pedido Where idPedido=" + idPedido;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Recupera os pedidos de um sinal

        /// <summary>
        /// Recupera os pedidos de um sinal.
        /// </summary>
        /// <param name="idSinal"></param>
        /// <param name="pagtoAntecipado"></param>
        /// <returns></returns>
        public Pedido[] GetBySinal(uint idSinal, bool pagtoAntecipado)
        {
            return GetBySinal(null, idSinal, pagtoAntecipado);
        }

        /// <summary>
        /// Recupera os pedidos de um sinal.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idSinal"></param>
        /// <param name="pagtoAntecipado"></param>
        /// <returns></returns>
        public Pedido[] GetBySinal(GDASession session, uint idSinal, bool pagtoAntecipado)
        {
            return GetBySinal(session, idSinal, pagtoAntecipado, false);
        }

        /// <summary>
        /// Recupera os pedidos de um sinal.
        /// </summary>
        /// <param name="idSinal"></param>
        /// <param name="pagtoAntecipado"></param>
        /// <param name="retificacao"></param>
        /// <returns></returns>
        public Pedido[] GetBySinal(uint idSinal, bool pagtoAntecipado, bool retificacao)
        {
            return GetBySinal(null, idSinal, pagtoAntecipado, retificacao);
        }

        /// <summary>
        /// Recupera os pedidos de um sinal.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idSinal"></param>
        /// <param name="pagtoAntecipado"></param>
        /// <param name="retificacao"></param>
        /// <returns></returns>
        public Pedido[] GetBySinal(GDASession session, uint idSinal, bool pagtoAntecipado, bool retificacao)
        {
            bool buscarReais = !SinalDAO.Instance.Exists(session, idSinal) ||
                SinalDAO.Instance.ObtemValorCampo<Sinal.SituacaoEnum>(session, "situacao", "idSinal=" + idSinal) != Sinal.SituacaoEnum.Cancelado;

            string sql = @"
                select p.*, pe.total as totalEspelho, " + ClienteDAO.Instance.GetNomeCliente("c") + @" as nomeCliente, s.dataCad as dataEntrada, s.usuCad as usuEntrada, 
                    cast(s.valorCreditoAoCriar as decimal(12,2)) as valorCreditoAoReceberSinal, cast(s.creditoGeradoCriar as decimal(12,2)) as creditoGeradoReceberSinal, 
                    cast(s.creditoUtilizadoCriar as decimal(12,2)) as creditoUtilizadoReceberSinal, s.isPagtoAntecipado as pagamentoAntecipado,
                    f.nome as nomeFunc, fc.nome as nomeFuncCliente
                from pedido p
                    left join pedido_espelho pe on (p.idPedido=pe.idPedido)
                    left join cliente c on (p.idCli=c.id_Cli)
                    left join sinal s on (p.idSinal=s.idSinal)
                    left join funcionario f on (p.idFunc=f.idFunc)
                    left join funcionario fc on (c.idFunc=fc.idFunc)
                where 1";

            if (retificacao)
                sql += " And p.situacao<>" + (int)Pedido.SituacaoPedido.Confirmado;

            if (buscarReais)
                sql += (pagtoAntecipado ? " And p.idPagamentoAntecipado=" : " And p.idSinal=") + idSinal;
            else
            {
                string idsPedidosR = SinalDAO.Instance.ObtemValorCampo<string>(session, "idsPedidosR", "idSinal=" + idSinal);
                sql += " and p.idPedido in (" + (!String.IsNullOrEmpty(idsPedidosR) ? idsPedidosR : "0") + ")";
            }

            return objPersistence.LoadData(session, sql).ToArray();
        }

        #endregion

        #region Obtém os Valores Totais

        public string GetTotais(uint idPedido, uint idLoja, uint idCli, string nomeCli, string codCliente, string endereco,
            string bairro, string situacao, string situacaoProd, string byVend, string byConf, string maoObra, string maoObraEspecial, string producao,
            uint idOrcamento, float altura, int largura, string nomeColunaTotal)
        {
            bool temFiltro;
            string filtroAdicional;

            string sqlTotal = "select sum(temp." + nomeColunaTotal + ") from (" + Sql(idPedido, 0, null, null, idLoja, idCli, nomeCli, 0,
                codCliente, 0, endereco, bairro, situacao, situacaoProd, byVend, byConf, null, maoObra, maoObraEspecial, producao, null, null,
                null, null, null, idOrcamento, false, false, altura, largura, 0, 0, 0, null, 0, 0, 0, "", true, out filtroAdicional, out temFiltro).
                Replace("?filtroAdicional?", filtroAdicional) + " ) as temp";

            return ExecuteScalar<string>(sqlTotal, GetParam(nomeCli, codCliente, endereco, bairro, situacao, situacaoProd, null, null, null, null, ""));
        }

        #endregion

        #region Obtém o total dos pedidos de um período - Gráfico

        //public string GetTotalPedidos(uint idLoja, uint idFunc, string dataIni, string dataFim)
        //{
        //    string dataInicial = DateTime.Parse(dataIni).ToString("yyyy/MM/dd");
        //    string dataFinal = DateTime.Parse(dataFim).ToString("yyyy/MM/dd");

        //    string sqltotal = "select sum(temp.Total) from (" + Sql(0, 0, null, null, idLoja, 0, null, idFunc, null,
        //        null, null, null, null, null, null, null, null, null, null, null, 0, false, false, 0,
        //        0, true) + " And p.Datacad between '" + dataInicial + "' And '" + dataFinal + "') as temp";

        //    Object retorno = objPersistence.ExecuteScalar(sqltotal);

        //    return retorno.ToString();
        //}

        public string GetTotalPedidos(uint idLoja, int tipoFunc, uint idVendedor, string dataIni, string dataFim)
        {
            string data = PedidoConfig.LiberarPedido ? "DataLiberacao" : "DataConf";

            string campos = @"p.idLoja, p.idFunc" + (tipoFunc == 0 ? "" : "Cli") + @" as idFunc, cast(Sum(p.Total) as decimal(12,2)) as TotalVenda, NomeFunc" + (tipoFunc == 0 ? "" : "Cli") +
                @" as NomeVendedor, NomeLoja, (Right(Concat('0', Cast(Month(p." + data + ") as char), '/', Cast(Year(p." + data + @") as char)), 7)) as DataVenda, 
                '$$$' as Criterio";

            string criterio = String.Empty;

            bool temFiltro;
            string filtroAdicional;

            string sql = @"
                Select " + campos + @" 
                From (" + PedidoDAO.Instance.SqlLucr("0", "0", (int)Pedido.SituacaoPedido.Confirmado, dataIni, dataFim, 0, 0, true,
                    out temFiltro, out filtroAdicional).Replace("?filtroAdicional?", filtroAdicional) + @") as p
                Where 1";

            if (idLoja > 0)
            {
                sql += " And p.IdLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(idLoja) + "    ";
            }

            if (idVendedor > 0)
            {
                sql += " And p.idFunc" + (tipoFunc == 0 ? "" : "Cli") + "=" + idVendedor;
                criterio += (tipoFunc == 0 ? "Emissor" : "Vendedor") + ": " + BibliotecaTexto.GetTwoFirstNames(FuncionarioDAO.Instance.GetNome(idVendedor)) + "    ";
            }

            if (!String.IsNullOrEmpty(dataIni))
                criterio += "Data Início: " + dataIni + "    ";

            if (!String.IsNullOrEmpty(dataFim))
                criterio += "Data Fim: " + dataFim + "    ";

            sql = "Select sum(temp.TotalVenda) from (" + sql + ") as temp";

            Object retorno = objPersistence.ExecuteScalar(sql.Replace("$$$", criterio), GetParams(dataIni, dataFim));

            return retorno.ToString();
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim)
        {
            List<GDAParameter> lstParams = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lstParams.Add(new GDAParameter("?dtIni", DateTime.Parse(dataIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParams.Add(new GDAParameter("?dtFim", DateTime.Parse(dataFim + " 23:59:59")));

            return lstParams.ToArray();
        }

        #endregion

        #region Verifica se o pedido pode ser exportado

        /// <summary>
        /// Verifica se o pedido pode ser exportado.
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool PodeExportar(uint idPedido)
        {
            var pcpFinalizado = GetTipoPedido(idPedido) == Pedido.TipoPedidoEnum.Revenda ? true :
                PedidoEspelhoDAO.Instance.ObtemSituacao(idPedido) == PedidoEspelho.SituacaoPedido.Finalizado ||
                PedidoEspelhoDAO.Instance.ObtemSituacao(idPedido) == PedidoEspelho.SituacaoPedido.Impresso;
    
            bool pode = PedidoDAO.Instance.IsPedidoConfirmadoLiberado(idPedido) && pcpFinalizado &&
                 PedidoExportacaoDAO.Instance.PodeExportar(idPedido);

             return pode;
        }

        #endregion

        #region Atualiza os campos TotM do Pedido e Pedido_Espelho

        /// <summary>
        /// Atualiza os campos TotM do Pedido e Pedido_Espelho.
        /// </summary>
        public void AtualizaTotM(uint idPedido)
        {
            AtualizaTotM(idPedido, null);
        }

        /// <summary>
        /// Atualiza os campos TotM do Pedido e Pedido_Espelho.
        /// </summary>
        public void AtualizaTotM(uint idPedido, bool? pcp)
        {
            AtualizaTotM(null, idPedido, pcp);
        }

        /// <summary>
        /// Atualiza os campos TotM do Pedido e Pedido_Espelho.
        /// </summary>
        /// <param name="pcp">NULL: Ambos; true: PCP; false: Pedido</param>
        internal void AtualizaTotM(GDASession sessao, uint idPedido, bool? pcp)
        {
            string campo = "Round(pp.totM" + (PedidoConfig.RelatorioPedido.ExibirM2CalcRelatorio ? "2Calc" :
                "*if(p1.tipoPedido=" + (int)Pedido.TipoPedidoEnum.MaoDeObra + " and ap.idAmbientePedido is not null, ap.qtde, 1)") + ", 2)";

            string sql = "";

            // Usa left join para calcular o totM, porque caso não tenha nenhum produto associado à este pedido o valor seja preenchido com 0 (zero)
            if (!pcp.GetValueOrDefault(false))
                sql += @"
                    update pedido p
	                    left join (
    	                    select pp.idPedido, sum(coalesce({0},0)) as totM
                            from produtos_pedido pp
                                left join ambiente_pedido ap on (pp.idAmbientePedido=ap.idAmbientePedido)
                                inner join pedido p1 on (pp.idPedido=p1.idPedido)
    	                    where coalesce(pp.invisivelPedido, false)=false And coalesce(pp.IdProdPedParent, 0) = 0 {2}
                            group by pp.idPedido
                        ) pp on (p.idPedido=pp.idPedido)
                    set p.totM=pp.totM
                    where 1 {1};";

            // Usa left join para calcular o totM, porque caso não tenha nenhum produto associado à este pedido o valor seja preenchido com 0 (zero)
            if (pcp.GetValueOrDefault(true))
                sql += @"
                    update pedido_espelho p
	                    left join (
    	                    select pp.idPedido, sum(coalesce({0},0)) as totM
                            from produtos_pedido_espelho pp
                                left join ambiente_pedido_espelho ap on (pp.idAmbientePedido=ap.idAmbientePedido)
                                inner join pedido p1 on (pp.idPedido=p1.idPedido)
    	                    where coalesce(pp.invisivelFluxo, false)=false And coalesce(pp.IdProdPedParent, 0) = 0 {2}
                            group by pp.idPedido
                        ) pp on (p.idPedido=pp.idPedido)
                    set p.totM=pp.totM
                    where 1 {1}";

            string where = "", whereInt = "";

            if (idPedido > 0)
            {
                where += " and p.idPedido=" + idPedido;
                whereInt += " and pp.idPedido=" + idPedido;
            }

            objPersistence.ExecuteCommand(sessao, String.Format(sql, campo, where, whereInt));
        }

        #endregion

        #region Recupera a data do último recebimento dos pedidos

        /// <summary>
        /// Recupera a data da última alteração, data de recebimento de sinal ou pagamento antecipado dos pedidos.
        /// </summary>
        public DateTime? ObterDataUltimaAlteracaoPedidoRecebimentoSinalouPagamentoAntecipado(GDASession session, string idsPedidos)
        {
            // Recupera a data da última alteração feita nos pedidos.
            var dataUltimaAlteracao = ObterDataUltimaAlteracaoPedido(session, idsPedidos);
            // Recupera a data do último recebimento de sinal ou pagamento antecipado dos pedidos.
            var dataUltimoRecebimentoSinalOuPagamentoAntecipado = ObterDataRecebimentoSinalOuPagamentoAntecipado(session, idsPedidos);

            // Caso nenhuma data seja retornada, sai do método retornando um valor nulo.
            if (!dataUltimaAlteracao.HasValue && !dataUltimoRecebimentoSinalOuPagamentoAntecipado.HasValue)
                return null;

            // Seta um valor mínimo nas variáveis que estiverem nulas.
            var primeiraData = dataUltimaAlteracao.GetValueOrDefault(DateTime.MinValue);
            var segundaData = dataUltimoRecebimentoSinalOuPagamentoAntecipado.GetValueOrDefault(DateTime.MinValue);

            // Verifica se as datas são iguais.
            if (DateTime.Compare(primeiraData, segundaData) == 0)
                return primeiraData;

            // Retorna a maior data dentre as 2 recuperadas.
            return DateTime.Compare(primeiraData, segundaData) > 0 ? primeiraData : DateTime.Compare(segundaData, primeiraData) > 0 ? segundaData : (DateTime?)null;
        }

        /// <summary>
        /// Recupera a data da última alteração dos pedidos.
        /// </summary>
        public DateTime? ObterDataUltimaAlteracaoPedido(GDASession session, string idsPedidos)
        {
            if (string.IsNullOrWhiteSpace(idsPedidos))
                return null;

            var sql = string.Format("SELECT MAX(DataAlt) AS Data FROM log_alteracao WHERE IdRegistroAlt IN ({0}) AND Tabela={1}", idsPedidos, (int)LogAlteracao.TabelaAlteracao.Pedido);

            return ExecuteScalar<DateTime?>(session, sql);
        }

        /// <summary>
        /// Recupera a data de recebimento dos sinais e dos pagamentos antecipados dos pedidos.
        /// </summary>
        public DateTime? ObterDataRecebimentoSinalOuPagamentoAntecipado(GDASession session, string idsPedidos)
        {
            if (string.IsNullOrWhiteSpace(idsPedidos))
                return null;

            var sql = string.Format(@"SELECT MAX(Data) FROM (
                    SELECT MAX(s.DataCad) AS Data FROM sinal s
                        INNER JOIN pedido p ON (s.IdSinal=p.IdSinal)
                    WHERE p.IdPedido IN ({0})
                    
                    UNION SELECT MAX(s.DataCad) AS Data FROM sinal s
                        INNER JOIN pedido p ON (s.IdSinal=p.IdPagamentoAntecipado)
                    WHERE p.IdPedido IN ({0})
                ) AS temp", idsPedidos);

            return ExecuteScalar<DateTime?>(session, sql);
        }

        /// <summary>
        /// Recupera a data de cancelamento dos sinais e pagamentos antecipados informados.
        /// </summary>
        public DateTime? ObterDataCancelamentoSinalOuPagamentoAntecipado(GDASession session, string idsSinais, string idsPagtoAntecip)
        {
            if (string.IsNullOrWhiteSpace(idsSinais))
                return null;

            if (!string.IsNullOrWhiteSpace(idsPagtoAntecip))
                idsSinais += string.Format(",{0}", idsPagtoAntecip);

            var sql = string.Format("SELECT MAX(DataCanc) AS Data FROM log_cancelamento WHERE IdRegistroCanc IN ({0}) AND Tabela={1}", idsSinais, (int)LogCancelamento.TabelaCancelamento.Sinal);

            return ExecuteScalar<DateTime?>(session, sql);
        }

        #endregion

        #region Exibir Nota Promissória?

        /// <summary>
        /// Exibir Nota Promissória?
        /// </summary>
        /// <param name="idPedido"></param>
        /// <returns></returns>
        public bool ExibirNotaPromissoria(uint idPedido)
        {
            int tipoVenda = ObtemTipoVenda(idPedido);
            Pedido.SituacaoPedido situacao = ObtemSituacao(idPedido);
            return ExibirNotaPromissoria(tipoVenda, situacao);
        }

        internal bool ExibirNotaPromissoria(int tipoVenda, Pedido.SituacaoPedido situacao)
        {
            return !PedidoConfig.LiberarPedido && FinanceiroConfig.DadosLiberacao.NumeroViasNotaPromissoria > 0 && tipoVenda == (int)Pedido.TipoVendaPedido.APrazo &&
                situacao == Pedido.SituacaoPedido.Confirmado && Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento);
        }

        #endregion

        #region Remove e aplica comissão, desconto e acréscimo

        #region Remover
        
        /// <summary>
        /// Remove comissão, desconto e acréscimo.
        /// </summary>
        public void RemoveComissaoDescontoAcrescimo(GDASession sessao, uint idPedido)
        {
            RemoveComissaoDescontoAcrescimo(sessao, idPedido, null);
        }

        /// <summary>
        /// Remove comissão, desconto e acréscimo.
        /// </summary>
        public void RemoveComissaoDescontoAcrescimo(GDASession sessao, uint idPedido, int? idAmbientePedido)
        {
            var ambientesPedido = AmbientePedidoDAO.Instance.GetByPedido(sessao, idPedido).Where(f => f.Acrescimo > 0).ToList();

            /* Chamado 62763. */
            foreach (var ambientePedido in ambientesPedido)
                AmbientePedidoDAO.Instance.RemoveAcrescimo(sessao, ambientePedido.IdAmbientePedido);

            var percComissao = RecuperaPercComissao(sessao, idPedido);
            RemoveComissao(sessao, idPedido, percComissao, idAmbientePedido);

            var tipoAcrescimo = ObtemValorCampo<int>(sessao, "tipoAcrescimo", "idPedido=" + idPedido);
            var acrescimo = ObtemValorCampo<decimal>(sessao, "acrescimo", "idPedido=" + idPedido);
            RemoveAcrescimo(sessao, idPedido, tipoAcrescimo, acrescimo, idAmbientePedido);
            
            var tipoDesconto = ObtemValorCampo<int>(sessao, "tipoDesconto", "idPedido=" + idPedido);
            var desconto = ObtemValorCampo<decimal>(sessao, "desconto", "idPedido=" + idPedido);
            RemoveDesconto(sessao, idPedido, tipoDesconto, desconto, idAmbientePedido);

            objPersistence.ExecuteCommand(sessao, @"update pedido set percComissao=0, desconto=0,
                acrescimo=0 where idPedido=" + idPedido);
        }

        /// <summary>
        /// Remove comissão, desconto e acréscimo.
        /// </summary>
        internal void RemoveComissaoDescontoAcrescimo(GDASession sessao, Pedido antigo, Pedido novo)
        {
            var ambientesPedido = AmbientePedidoDAO.Instance.GetByPedido(sessao, novo.IdPedido).Where(f => f.Acrescimo > 0).ToList();

            /* Chamado 62763. */
            foreach (var ambientePedido in ambientesPedido)
                AmbientePedidoDAO.Instance.RemoveAcrescimo(sessao, ambientePedido.IdAmbientePedido);

            var alteraComissao = antigo.PercComissao != novo.PercComissao;
            var alteraAcrescimo = antigo.Acrescimo != novo.Acrescimo || antigo.TipoAcrescimo != novo.TipoAcrescimo;
            var alteraDesconto = antigo.Desconto != novo.Desconto || antigo.TipoDesconto != novo.TipoDesconto;

            // Remove o valor da comissão nos produtos e no pedido
            if (alteraComissao)
                RemoveComissao(sessao, novo.IdPedido, antigo.PercComissao);

            // Remove o acréscimo do pedido
            if (alteraAcrescimo)
                RemoveAcrescimo(sessao, novo.IdPedido, antigo.TipoAcrescimo, antigo.Acrescimo, null, null);

            // Remove o desconto do pedido
            if (alteraDesconto)
                RemoveDesconto(sessao, novo.IdPedido, antigo.TipoDesconto, antigo.Desconto, null, null);
        }

        #endregion

        #region Aplicar

        public void AplicaComissaoDescontoAcrescimo(GDASession sessao, uint idPedido, uint? idComissionado, float percComissao,
            int tipoAcrescimo, decimal acrescimo, int tipoDesconto, decimal desconto, bool manterFuncDesc)
        {
            AplicaComissaoDescontoAcrescimo(sessao, idPedido, idComissionado, percComissao, tipoAcrescimo, acrescimo, tipoDesconto,
                desconto, null, manterFuncDesc);
        }

        public void AplicaComissaoDescontoAcrescimo(GDASession sessao, uint idPedido, uint? idComissionado, float percComissao,
            int tipoAcrescimo, decimal acrescimo, int tipoDesconto, decimal desconto, int? idAmbientePedido, bool manterFuncDesc)
        {
            var ambientesPedido = AmbientePedidoDAO.Instance.GetByPedido(sessao, idPedido).Where(f => f.Acrescimo > 0).ToList();

            if (idComissionado > 0)
                objPersistence.ExecuteCommand(sessao, "update pedido set idComissionado=" + idComissionado +
                    " where idPedido=" + idPedido);

            AplicaAcrescimo(sessao, idPedido, tipoAcrescimo, acrescimo, false, idAmbientePedido);
            AplicaDesconto(sessao, idPedido, tipoDesconto, desconto, manterFuncDesc, idAmbientePedido, true);
            AplicaComissao(sessao, idPedido, percComissao, idAmbientePedido);

            objPersistence.ExecuteCommand(sessao, @"update pedido set percComissao=?pc, tipoDesconto=?td, desconto=?d,
                tipoAcrescimo=?ta, acrescimo=?a where idPedido=" + idPedido, new GDAParameter("?pc", percComissao),
                new GDAParameter("?td", tipoDesconto), new GDAParameter("?d", desconto),
                new GDAParameter("?ta", tipoAcrescimo), new GDAParameter("?a", acrescimo));

            /* Chamado 62763. */
            foreach (var ambientePedido in ambientesPedido)
                AmbientePedidoDAO.Instance.AplicaAcrescimo(sessao, ambientePedido.IdAmbientePedido, ambientePedido.TipoAcrescimo, ambientePedido.Acrescimo);
        }

        /// <summary>
        /// Aplica comissão, desconto e acréscimo em uma ordem pré-estabelecida.
        /// </summary>
        internal void AplicaComissaoDescontoAcrescimo(GDASession sessao, Pedido antigo, Pedido novo)
        {
            var ambientesPedido = AmbientePedidoDAO.Instance.GetByPedido(sessao, novo.IdPedido).Where(f => f.Acrescimo > 0).ToList();

            var alteraDesconto = antigo.Desconto != novo.Desconto || antigo.TipoDesconto != novo.TipoDesconto;
            var alteraComissao = antigo.PercComissao != novo.PercComissao;
            var alteraAcrescimo = antigo.Acrescimo != novo.Acrescimo || antigo.TipoAcrescimo != novo.TipoAcrescimo;

            // Remove o acréscimo do pedido
            if (alteraAcrescimo)
                AplicaAcrescimo(sessao, novo.IdPedido, novo.TipoAcrescimo, novo.Acrescimo, false);

            // Remove o desconto do pedido
            if (alteraDesconto)
                AplicaDesconto(sessao, novo.IdPedido, novo.TipoDesconto, novo.Desconto, false);

            // Remove o valor da comissão nos produtos e no pedido
            if (alteraComissao)
                AplicaComissao(sessao, novo.IdPedido, novo.PercComissao);

            /* Chamado 62763. */
            foreach (var ambientePedido in ambientesPedido)
                AmbientePedidoDAO.Instance.AplicaAcrescimo(sessao, ambientePedido.IdAmbientePedido, ambientePedido.TipoAcrescimo, ambientePedido.Acrescimo);
        }

        #endregion

        #endregion

        #region Verifica se o pedido foi gerado por um parceiro

        /// <summary>
        /// Verifica se o pedido foi gerado por um parceiro.
        /// </summary>
        public bool IsGeradoParceiro(uint idPedido)
        {
            return IsGeradoParceiro(null, idPedido);
        }

        /// <summary>
        /// Verifica se o pedido foi gerado por um parceiro.
        /// </summary>
        public bool IsGeradoParceiro(GDASession session, uint idPedido)
        {
            return ObtemValorCampo<bool>(session, "geradoParceiro", "idPedido=" + idPedido);
        }

        #endregion

        #region Métodos sobrescritos

        public enum TipoCalculoParcelas
        {
            Ambas,
            Data,
            Valor
        }

        /// <summary>
        /// Recalcula as parcelas do pedido.
        /// </summary>
        /// <param name="objPedido"></param>
        public void RecalculaParcelas(GDASession sessao, ref Pedido objPedido, TipoCalculoParcelas tipoCalculo)
        {
            uint tipoPagtoCli = ClienteDAO.Instance.ObtemValorCampo<uint>(sessao, "tipoPagto", "id_Cli=" + objPedido.IdCli);
            uint idParc = objPedido.IdParcela.GetValueOrDefault(tipoPagtoCli);

            if (idParc == 0)
                return;

            int numParcelas = ParcelasDAO.Instance.ObtemValorCampo<int>(sessao, "numParcelas", "idParcela=" + idParc);

            if (numParcelas == 0)
                return;

            string[] numDias = ParcelasDAO.Instance.ObtemValorCampo<string>(sessao, "dias", "idParcela=" + idParc).Split(',');

            objPedido.IdParcela = idParc;
            objPedido.NumParc = numParcelas;

            decimal totalParc = objPedido.Total - (objPedido.ValorEntrada + objPedido.ValorPagamentoAntecipado);
            decimal valorParc = Math.Round(totalParc / objPedido.NumParc, 2);

            decimal[] valoresParc = new decimal[objPedido.NumParc];
            DateTime[] datasParc = new DateTime[objPedido.NumParc];

            decimal somaParc = 0;
            for (int i = 0; i < objPedido.NumParc; i++)
            {
                if (i < (objPedido.NumParc - 1))
                {
                    valoresParc[i] = valorParc;
                    somaParc += valorParc;
                }
                else
                    valoresParc[i] = Math.Round(totalParc - somaParc, 2);

                datasParc[i] = DateTime.Now.AddDays(Glass.Conversoes.StrParaInt(numDias[i]));
            }

            if (tipoCalculo != TipoCalculoParcelas.Data || objPedido.ValoresParcelas.Length < valoresParc.Length)
                objPedido.ValoresParcelas = valoresParc;

            if (tipoCalculo != TipoCalculoParcelas.Valor || objPedido.DatasParcelas.Length < datasParc.Length)
                objPedido.DatasParcelas = datasParc;
        }

        /// <summary>
        /// Salva as parcelas do pedido.
        /// </summary>
        private void SalvarParcelas(GDASession session, Pedido objPedido)
        {
            // Se for venda à vista exclui as parcelas
            if (objPedido.TipoVenda == 1)
                ParcelasPedidoDAO.Instance.DeleteFromPedido(session, objPedido.IdPedido);

            // Se for venda à prazo, salva as parcelas
            else if (objPedido.TipoVenda == 2)
            {
                ParcelasPedidoDAO.Instance.DeleteFromPedido(session, objPedido.IdPedido);

                ParcelasPedido parcela = new ParcelasPedido();
                parcela.IdPedido = objPedido.IdPedido;

                if (objPedido.ValoresParcelas == null)
                    throw new Exception("Informe as parcelas do pedido");

                if (objPedido.ValoresParcelas.Length > 0 && objPedido.ValoresParcelas[0] > 0)
                    for (int i = 0; i < objPedido.NumParc; i++)
                    {
                        // Chamado 35806. Caso o índice seja maior que a quantidade de itens dentro das variáveis "ValoresParcelas" ou
                        // "DatasParcelas", o loop deve ser finalizado.
                        if (objPedido.ValoresParcelas.Count() == i || objPedido.DatasParcelas.Count() == i)
                            break;

                        parcela.Valor = objPedido.ValoresParcelas[i];
                        parcela.Data = objPedido.DatasParcelas[i];
                        ParcelasPedidoDAO.Instance.Insert(session, parcela);
                    }
            }
        }

        public override uint Insert(Pedido objInsert)
        {
            lock (_inserirPedidoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var retorno = Insert(transaction, objInsert);

                        transaction.Commit();
                        transaction.Close();

                        return retorno;
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

        public override uint Insert(GDASession session, Pedido objInsert)
        {
            uint idPedido = 0;

            if (objInsert.TipoPedido == 0)
                throw new Exception("Não foi informado o Tipo do pedido.");

            if (UserInfo.GetUserInfo.TipoUsuario == (int)Utils.TipoFuncionario.Vendedor &&
            !Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoGarantiaReposicao))
            {
                if (objInsert.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição &&
                    !Config.PossuiPermissao(Config.FuncaoMenuPedido.GerarReposicao))
                    throw new Exception("Apenas o gerente pode emitir pedido de reposição.");
                else if (objInsert.TipoVenda == (int)Pedido.TipoVendaPedido.Garantia)
                    throw new Exception("Apenas o gerente pode emitir pedido de garantia.");
            }

            if (objInsert.TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObraEspecial)
            {
                if (!ClienteDAO.Instance.ObtemValorCampo<bool>(session, "controlarEstoqueVidros", "id_Cli=" + objInsert.IdCli))
                    throw new Exception("O cliente deve controlar estoque para ser utilizado em um pedido de mão-de-obra especial.");
            }

            if (objInsert.IdObra.GetValueOrDefault(0) > 0 && ObraDAO.Instance.ObtemSituacao(session, objInsert.IdObra.Value) != Obra.SituacaoObra.Confirmada)
                throw new Exception("A obra informada não esta confirmada.");

            // Caso o usuário não tenha permissão de alterar o vendedor no pedido, força a associação do vendedor associado ao cliente neste pedido
            uint? idFunc = ClienteDAO.Instance.ObtemIdFunc(session, objInsert.IdCli);
            if ((objInsert.Importado || !objInsert.SelVendEnabled) && idFunc > 0 && PedidoConfig.DadosPedido.BuscarVendedorEmitirPedido)
                objInsert.IdFunc = idFunc.Value;

            if (objInsert.IdOrcamento == 0)
                objInsert.IdOrcamento = null;

            if (!PedidoConfig.Comissao.AlterarPercComissionado)
            {
                if (objInsert.IdComissionado > 0)
                    objInsert.PercComissao = ComissionadoDAO.Instance.ObtemValorCampo<float>(session, "percentual", "idComissionado=" + objInsert.IdComissionado.Value);
                else
                    objInsert.PercComissao = 0;
            }

            // Verifica se o id do orcamento informado existe
            if (objInsert.IdOrcamento != null)
                if (OrcamentoDAO.Instance.GetCount(session, objInsert.IdOrcamento.Value) < 1)
                    throw new Exception("O Orcamento informado no pedido não existe.");

            if (objInsert.IdFormaPagto == 0)
                objInsert.IdFormaPagto = (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio;

            if (objInsert.TipoVenda == null || objInsert.TipoVenda == 0)
                objInsert.TipoVenda = (int)Pedido.TipoVendaPedido.AVista;

            // Se o pedido for à vista, não é necessário informar a forma de pagamento
            if (objInsert.TipoVenda == (int)Pedido.TipoVendaPedido.AVista)
                objInsert.IdFormaPagto = null;

            //Verifica se o cliente possui contas a receber vencidas se nao for garantia
            if (!FinanceiroConfig.PermitirFinalizacaoPedidoPeloFinanceiro && (ClienteDAO.Instance.ObtemValorCampo<bool>(session, "bloquearPedidoContaVencida", "id_Cli=" + objInsert.IdCli)) &&
                ContasReceberDAO.Instance.ClientePossuiContasVencidas(session, objInsert.IdCli) && objInsert.TipoVenda != (int)Pedido.TipoVendaPedido.Garantia)
                throw new Exception("Cliente bloqueado. Motivo: Contas a receber em atraso.");

            // Verifica se o código do pedido do cliente já foi cadastrado
            if (objInsert.TipoVenda < 3)
                CodigoClienteUsado(session, objInsert.IdPedido, objInsert.IdCli, objInsert.CodCliente, false);

            // Obtém a data de atraso deste funcionário, se houver
            if (FuncionarioDAO.Instance.PossuiDiasAtraso(session, objInsert.Usucad))
                objInsert.DataPedido = FuncionarioDAO.Instance.ObtemDataAtraso(session, objInsert.Usucad);

            decimal descontoPadraoAVista = PedidoConfig.Desconto.DescontoPadraoPedidoAVista;
            if (descontoPadraoAVista > 0)
            {
                uint? tipoPagto = ClienteDAO.Instance.ObtemValorCampo<uint?>(session, "tipoPagto", "id_Cli=" + objInsert.IdCli);

                bool podeTerDesconto = !(PedidoConfig.Desconto.ImpedirDescontoSomativo &&
                    UserInfo.GetUserInfo.TipoUsuario != (uint)Utils.TipoFuncionario.Administrador &&
                        DescontoAcrescimoClienteDAO.Instance.ClientePossuiDesconto(session, objInsert.IdCli, 0, null, 0, null));

                if (podeTerDesconto && (objInsert.TipoVenda == (int)Pedido.TipoVendaPedido.AVista || (tipoPagto > 0 &&
                        ParcelasDAO.Instance.ObtemValorCampo<string>(session, "descricao", "idParcela=" + tipoPagto).ToLower().Contains("na entrega"))))
                {
                    objInsert.TipoDesconto = 1;
                    objInsert.Desconto = descontoPadraoAVista;
                }
            }

            idPedido = InsertBase(session, objInsert);

            #region Data de entrega

            AtualizarDataEntregaCalculada(session, objInsert, idPedido);

            #endregion

            // Se o pedido não for mão de obra e se a empresa trabalha com ambiente no pedido, insere um ambiente balcão de uma vez, se o tipo de entrega for balcão
            if (objInsert.TipoPedido != (int)Pedido.TipoPedidoEnum.MaoDeObra &&
                PedidoConfig.DadosPedido.AmbientePedido && objInsert.TipoEntrega == (int)Pedido.TipoEntregaPedido.Balcao &&
                objInsert.IdProjeto == null && !objInsert.FromOrcamentoRapido && objInsert.IdPedidoAnterior == null)
            {
                AmbientePedido ambiente = new AmbientePedido();
                ambiente.Ambiente = "Balcão";
                ambiente.Descricao = "Balcão";
                ambiente.IdPedido = idPedido;
                ambiente.Qtde = 1;
                AmbientePedidoDAO.Instance.Insert(session, ambiente);
            }

            return idPedido;
        }

        public override int Update(Pedido objUpdate)
        {
            FilaOperacoes.AtualizarPedido.AguardarVez();

            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Update(transaction, objUpdate);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
                finally
                {
                    FilaOperacoes.AtualizarPedido.ProximoFila();
                }
            }
        }

        public override int Update(GDASession session, Pedido objUpdate)
        {
            if (objUpdate.TipoPedido == 0)
                throw new Exception("Não foi informado o Tipo do pedido.");

            if (UserInfo.GetUserInfo.TipoUsuario == (int)Utils.TipoFuncionario.Vendedor &&
                !Config.PossuiPermissao(Config.FuncaoMenuPedido.EmitirPedidoGarantiaReposicao))
            {
                if (objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição && !Config.PossuiPermissao(Config.FuncaoMenuPedido.GerarReposicao))
                    throw new Exception("Apenas o gerente pode emitir pedido de reposição.");
                else if (objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.Garantia)
                    throw new Exception("Apenas o gerente pode emitir pedido de garantia.");
            }

            //Se o pedido ja tiver oc gerada não pode alterar a data de entrega
            if (PedidoOrdemCargaDAO.Instance.PedidoTemOC(session, objUpdate.IdPedido) &&
                objUpdate.DataEntrega != ObtemDataEntrega(session, objUpdate.IdPedido))
            {
                throw new Exception("O pedido já possui OC gerada, não é possível alterar a data de entrega.");
            }

            if (objUpdate.TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObraEspecial)
            {
                if (!ClienteDAO.Instance.ObtemValorCampo<bool>(session, "controlarEstoqueVidros", "id_Cli=" + objUpdate.IdCli))
                    throw new Exception("O cliente deve controlar estoque para ser utilizado em um pedido de mão-de-obra especial.");
            }

            if (objUpdate.IdObra.GetValueOrDefault(0) > 0 && ObraDAO.Instance.ObtemSituacao(session, objUpdate.IdObra.Value) != Obra.SituacaoObra.Confirmada)
                throw new Exception("A obra informada não esta confirmada.");

            if (objUpdate.IdObra > 0 && objUpdate.Desconto > 0 && PedidoConfig.DadosPedido.UsarControleNovoObra)
                throw new Exception("Não é permitido lançar desconto em pedidos de obra, pois o valor do m² já foi definido com o cliente.");

            if (objUpdate.IdObra > 0 && objUpdate.Acrescimo > 0 && PedidoConfig.DadosPedido.UsarControleNovoObra)
                throw new Exception("Não é permitido lançar acréscimo em pedidos de obra, pois o valor do m² já foi definido com o cliente.");

            var produtosPedido = ProdutosPedidoDAO.Instance.GetByPedido(session, objUpdate.IdPedido);

            //se o pedido tiver marcado com fast delivery e se tiver valida as aplicações dos produtos
            if (objUpdate.FastDelivery)
            {
                foreach (var produtoPedido in produtosPedido.Where(f => f.IdAplicacao > 0))
                {
                    if (EtiquetaAplicacaoDAO.Instance.NaoPermitirFastDelivery(session, produtoPedido.IdAplicacao.Value))
                    {
                        var codInternoAplicacao = EtiquetaAplicacaoDAO.Instance.ObtemCodInterno(session, produtoPedido.IdAplicacao.Value);

                        throw new Exception(string.Format("Erro|O produto {0} tem a aplicacao {1} e esta aplicacao não permite fast delivery.", produtoPedido.DescrProduto, codInternoAplicacao));
                    }
                }
            }

            var idLojaOriginal = ObtemIdLoja(session, objUpdate.IdPedido);

            if (idLojaOriginal > 0 && idLojaOriginal != objUpdate.IdLoja)
            {
                /* Chamado 53901. */
                if (!PedidoConfig.AlterarLojaPedido)
                    throw new Exception("Não é permitido alterar a loja do pedido.");

                foreach (var prodPed in produtosPedido)
                {
                    //Esse produto não pode ser utilizado, pois a loja do seu subgrupo é diferente da loja do pedido.
                    var idLojaSubgrupoProd = SubgrupoProdDAO.Instance.ObterIdLojaPeloProduto(null, (int)prodPed.IdProd);
                    if (idLojaSubgrupoProd.GetValueOrDefault(0) > 0 && idLojaSubgrupoProd.Value != objUpdate.IdLoja)
                        throw new Exception("Não é possível alterar a loja deste pedido, a loja cadastrada para o subgrupo de um ou mais produtos é diferente da loja selecionada para o pedido.");

                    if (GrupoProdDAO.Instance.BloquearEstoque(session, (int)prodPed.IdGrupoProd, (int)prodPed.IdSubgrupoProd))
                    {
                        var estoque = ProdutoLojaDAO.Instance.GetEstoque(session, objUpdate.IdLoja, prodPed.IdProd, null, IsProducao(session, objUpdate.IdPedido), false, true);

                        if (estoque < produtosPedido.Where(f => f.IdProd == prodPed.IdProd).Sum(f => f.Qtde))
                            throw new Exception("O produto " + prodPed.DescrProduto + " possui apenas " + estoque + " em estoque na loja selecionada.");
                    }
                }
            }

            // Caso o usuário não tenha permissão de alterar o vendedor no pedido, força a associação do vendedor associado ao cliente neste pedido
            uint? idFunc = ClienteDAO.Instance.ObtemIdFunc(session, objUpdate.IdCli);
            if ((objUpdate.Importado || !objUpdate.SelVendEnabled) && idFunc > 0 && PedidoConfig.DadosPedido.BuscarVendedorEmitirPedido)
                objUpdate.IdFunc = idFunc.Value;

            if (objUpdate.IdOrcamento == 0)
                objUpdate.IdOrcamento = null;
                
            if (!PedidoConfig.Comissao.AlterarPercComissionado)
            {
                if (objUpdate.IdComissionado > 0)
                    objUpdate.PercComissao = ComissionadoDAO.Instance.ObtemValorCampo<float>(session, "percentual", "idComissionado=" + objUpdate.IdComissionado.Value);
                else
                    objUpdate.PercComissao = 0;
            }

            // Verifica se o id do orcamento informado existe
            if (objUpdate.IdOrcamento != null)
                if (OrcamentoDAO.Instance.GetCount(session, objUpdate.IdOrcamento.Value) < 1)
                    throw new Exception("O Orcamento informado no pedido não existe.");

            // Verifica se este orçamento pode ter desconto
            if (PedidoConfig.Desconto.ImpedirDescontoSomativo && UserInfo.GetUserInfo.TipoUsuario != (uint)Utils.TipoFuncionario.Administrador &&
                objUpdate.Desconto > 0 && DescontoAcrescimoClienteDAO.Instance.ClientePossuiDesconto(session, objUpdate.IdCli, 0, null, objUpdate.IdPedido, null))
                throw new Exception("O cliente já possui desconto por grupo/subgrupo, não é permitido lançar outro desconto.");
 
            /* Chamado 43090. */
            if (objUpdate.ValorEntrada < 0)
                throw new Exception("O valor de entrada do pedido não pode ser negativo. Edite o pedido, atualize os valores e tente novamente.");

            // Verifica se o código do pedido do cliente já foi cadastrado
            if (objUpdate.TipoVenda < 3)
                CodigoClienteUsado(session, objUpdate.IdPedido, objUpdate.IdCli, objUpdate.CodCliente, false);

            // Busca o pedido antes dele ser atualizado para verificar se o tipo de entrega foi alterado
            Pedido ped = GetElementByPrimaryKey(session, objUpdate.IdPedido);
            bool isClienteRevenda = ClienteDAO.Instance.IsRevenda(session, objUpdate.IdCli);

            /* Chamado 28687. */
            objUpdate.Importado = ped.Importado;

            /* Chamado 22806. */
            if (objUpdate.Desconto != ped.Desconto &&
                ped.Situacao != Pedido.SituacaoPedido.Ativo &&
                ped.Situacao != Pedido.SituacaoPedido.AtivoConferencia)
                throw new Exception("Não é possível alterar o desconto deste pedido, ele não está ativo.");
 
            /* Chamado 28243. */
            if ((objUpdate.Desconto > 0 || objUpdate.Acrescimo > 0) &&
                ProdutosPedidoDAO.Instance.CountInPedido(session, objUpdate.IdPedido) == 0)
                throw new Exception("Não é possível definir o percentual/valor de desconto/acréscimo no pedido caso o mesmo não possua produtos.");

            if (objUpdate.Situacao == Pedido.SituacaoPedido.Ativo)
            {
                if (ped.Situacao == Pedido.SituacaoPedido.ConfirmadoLiberacao)
                    throw new Exception("Não é possível mudar a situação do pedido de confirmado para ativo, feche a tela e tente realizar a operação novamente.");
                else if (ped.Situacao == Pedido.SituacaoPedido.Conferido)
                    throw new Exception("Não é possível mudar a situação do pedido de conferido para ativo, feche a tela e tente realizar a operação novamente.");
            }

            // Não permite atualizar o pedido se já estiver confirmado/liberado.
            if (ped.Situacao == Pedido.SituacaoPedido.Confirmado)
                throw new Exception(string.Format("Não é possível alterar dados do pedido depois de {0}.", PedidoConfig.LiberarPedido ? "liberado" : "confirmado"));

            // Caso o pedido tenha pagamento antecipado e o usuário esteja tentando alterar o cliente, não permite
            if (objUpdate.IdCli != ped.IdCli && TemPagamentoAntecipadoRecebido(session, objUpdate.IdPedido))
                throw new Exception("O cliente deste pedido não pode ser alterado pois já existe um pagamento antecipado para este pedido.");

            // Verifica se o desconto que já exista no pedido pode ser mantido pelo usuário que está atualizando o pedido, 
            // tendo em vista que o mesmo possa ter sido lançado por um administrador
            if (objUpdate.Desconto == ped.Desconto && ped.Desconto > 0 && !DescontoPermitido(session, objUpdate.IdPedido))
                throw new Exception("O desconto lançado anteriormente está acima do permitido para este login.");

            // Não permite que pedido de garantia e reposição tenham sinal
            if (objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.Garantia || objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição)
            {
                if (ped.RecebeuSinal)
                    throw new Exception("Não é possivel alterar o tipo de venda para Garantia ou Reposição pois este pedido já possui sinal recebido.");

                objUpdate.ValorEntrada = 0;
                objUpdate.ValorPagamentoAntecipado = 0;
            }

            if (objUpdate.IdCli != ped.IdCli && (ped.IdSinal > 0 || ped.IdPagamentoAntecipado > 0))
                throw new Exception("Não é possível alterar o cliente deste pedido pois o mesmo já possui sinal/pagamento antecipado recebido.");

            // Se tiver sido lançado 100% de desconto no pedido, altera o desconto para reais, porque o desconto ficando 100%,
            // a propriedade DescontoTotal fica incorreta e na liberação também
            if (objUpdate.Desconto == 100 && objUpdate.TipoDesconto == 1)
            {
                objUpdate.Desconto = objUpdate.TotalSemDesconto;
                objUpdate.TipoDesconto = 2;
            }

            if (!PedidoConfig.LiberarPedido && objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.AVista && (ped.IdSinal > 0 || ped.IdPagamentoAntecipado > 0))
                throw new Exception("Este pedido teve um " + (ped.IdSinal > 0 ? "sinal" : "pagto. antecipado") + " recebido, não é possivel alterá-lo para à vista.");

            if (ped.ValorEntrada != objUpdate.ValorEntrada && ped.ValorEntrada > 0 && ped.RecebeuSinal)
                throw new Exception("O sinal deste pedido já foi recebido, não é possível alterar o valor de entrada.");

            #region Atualiza preço dos produtos do pedido

            // Se o tipo de entrega estiver sendo alterado, se o cliente não for revenda e se o novo tipo
            // de entrega for colocação comum ou temperado, OU se o cliente tiver sido alterado e este novo cliente não for revenda,
            // atualiza o valor dos itens do pedido se estiverem abaixo do permitido, a menos que seja obra e a empresa use o controle novo de obra
            if ((objUpdate.IdObra.GetValueOrDefault() == 0 || !PedidoConfig.DadosPedido.UsarControleNovoObra) && 
                (ped.TipoEntrega != objUpdate.TipoEntrega || ped.IdCli != objUpdate.IdCli || ped.TipoVenda != objUpdate.TipoVenda))
            {
                #region Declaração de variáveis

                int tipoDesconto, tipoAcrescimo;
                decimal desconto, acrescimo;
                float percComissao;
                uint? idComissionado;

                #endregion

                #region Atualização dos dados do pedido

                // Atualiza o tipo de venda e a parcela do pedido, para que o desconto à vista da tabela do cliente seja recuperado corretamente.
                objPersistence.ExecuteCommand(session, string.Format("UPDATE pedido SET TipoVenda={0} WHERE IdPedido={2}", objUpdate.TipoVenda,
                    objUpdate.IdParcela > 0 ? string.Format(", IdParcela={0}", objUpdate.IdParcela) : string.Empty, objUpdate.IdPedido));

                #endregion

                ObtemDadosComissaoDescontoAcrescimo(session, objUpdate.IdPedido, out tipoDesconto, out desconto, out tipoAcrescimo, out acrescimo,
                    out percComissao, out idComissionado);

                RemoveComissaoDescontoAcrescimo(session, objUpdate.IdPedido);

                // Marca os projetos como não conferido, pois é mais complicado recalcular os projetos
                foreach (ItemProjeto ip in ItemProjetoDAO.Instance.GetByPedido(session, objUpdate.IdPedido))
                {
                    uint? idAmbientePedido = AmbientePedidoDAO.Instance.GetIdByItemProjeto(session, ip.IdItemProjeto);
                    ProdutosPedidoDAO.Instance.InsereAtualizaProdProj(session, objUpdate.IdPedido, idAmbientePedido, ip, false);
                    ItemProjetoDAO.Instance.CalculoNaoConferido(session, ip.IdItemProjeto);
                }

                var lstProd = ProdutosPedidoDAO.Instance.GetByPedido(session, objUpdate.IdPedido).ToArray();
                bool originalCobraAreaMinima = TipoClienteDAO.Instance.CobrarAreaMinima(session, ped.IdCli);
                bool novoCobraAreaMinima = TipoClienteDAO.Instance.CobrarAreaMinima(session, objUpdate.IdCli);

                // Para cada item do pedido
                foreach (ProdutosPedido prodPed in lstProd)
                {
                    bool mudou = originalCobraAreaMinima != novoCobraAreaMinima;

                    if (objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição)
                    {
                        mudou = true;
                        prodPed.ValorVendido = ProdutoDAO.Instance.GetValorTabela(session, (int)prodPed.IdProd, ped.TipoEntrega, objUpdate.IdCli, isClienteRevenda, true, prodPed.PercDescontoQtde, (int)objUpdate.IdPedido, null, null);
                        prodPed.ValorAcrescimoCliente = 0;
                        prodPed.ValorDescontoCliente = 0;
                    }
                    else
                    {
                        decimal valorAtacado = ProdutoDAO.Instance.GetValorTabela(session, (int)prodPed.IdProd, (int)Pedido.TipoEntregaPedido.Balcao, objUpdate.IdCli, true, false, prodPed.PercDescontoQtde, (int)objUpdate.IdPedido, null, null);
                        decimal valorBalcao = ProdutoDAO.Instance.GetValorTabela(session, (int)prodPed.IdProd, (int)Pedido.TipoEntregaPedido.Balcao, objUpdate.IdCli, isClienteRevenda, false, prodPed.PercDescontoQtde, (int)objUpdate.IdPedido, null, null);
                        decimal valorObra = ProdutoDAO.Instance.GetValorTabela(session, (int)prodPed.IdProd, (int)Pedido.TipoEntregaPedido.Comum, objUpdate.IdCli, isClienteRevenda, false, prodPed.PercDescontoQtde, (int)objUpdate.IdPedido, null, null);

                        // Se o cliente é revenda
                        if (isClienteRevenda && (prodPed.ValorVendido < valorAtacado || ped.IdCli != objUpdate.IdCli))
                        {
                            mudou = true;
                            prodPed.ValorVendido = valorAtacado;
                            DescontoAcrescimo.Instance.DiferencaCliente(session, prodPed, objUpdate.IdCli, objUpdate.TipoEntrega, false, (int)objUpdate.IdPedido, null, null);
                        }

                        // Se o tipo de entrega for balcão, traz preço de balcão
                        else if (objUpdate.TipoEntrega == (int)Pedido.TipoEntregaPedido.Balcao)
                        {
                            mudou = true;
                            prodPed.ValorVendido = valorBalcao;
                            DescontoAcrescimo.Instance.DiferencaCliente(session, prodPed, objUpdate.IdCli, (int)Pedido.TipoEntregaPedido.Balcao, false, (int)objUpdate.IdPedido, null, null);
                        }

                        // Se o tipo de entrega for entrega, traz preço de obra
                        else if (objUpdate.TipoEntrega == (int)Pedido.TipoEntregaPedido.Entrega ||
                            objUpdate.TipoEntrega == (int)Pedido.TipoEntregaPedido.Temperado)
                        {
                            mudou = true;
                            prodPed.ValorVendido = valorObra;
                            DescontoAcrescimo.Instance.DiferencaCliente(session, prodPed, objUpdate.IdCli, (int)Pedido.TipoEntregaPedido.Entrega, false, (int)objUpdate.IdPedido, null, null);
                        }

                        // Verifica se o valor é permitido, se não for atualiza o valor para o mínimo
                        else if (prodPed.ValorVendido < valorObra)
                        {
                            mudou = true;
                            prodPed.ValorVendido = valorObra;
                            DescontoAcrescimo.Instance.DiferencaCliente(session, prodPed, objUpdate.IdCli, (int)Pedido.TipoEntregaPedido.Comum, false, (int)objUpdate.IdPedido, null, null);
                        }
                    }

                    if (mudou)
                    {
                        ProdutosPedidoDAO.Instance.RecalcularValores(session, prodPed, objUpdate.IdCli,
                            objUpdate.TipoEntrega.GetValueOrDefault((int)Pedido.TipoEntregaPedido.Balcao),
                            false, (Pedido.TipoVendaPedido?)objUpdate.TipoVenda);

                        ProdutosPedidoDAO.Instance.UpdateBase(session, prodPed, false);
                    }
                }

                AplicaComissaoDescontoAcrescimo(session, objUpdate.IdPedido, idComissionado, percComissao, tipoAcrescimo, acrescimo,
                    tipoDesconto, desconto, Geral.ManterDescontoAdministrador);
            }

            #endregion

            // Marca recebeu sinal e situação de acordo com o que está no banco
            objUpdate.IdPagamentoAntecipado = ped.IdPagamentoAntecipado;
            objUpdate.IdSinal = ped.IdSinal;
            objUpdate.ValorPagamentoAntecipado = ped.ValorPagamentoAntecipado;
            objUpdate.Situacao = ped.Situacao;

            // Salva as parcelas do pedido
            SalvarParcelas(session, objUpdate);

            if (objUpdate.TipoVenda == null || objUpdate.TipoVenda == 0)
                objUpdate.TipoVenda = (int)Pedido.TipoVendaPedido.AVista;
 
            // Se o controle de desconto por forma de pagamento estiver desabilitado e o pedido for à vista, não é necessário informar a forma de pagamento.
            if (!FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto && objUpdate.TipoVenda == (int)Pedido.TipoVendaPedido.AVista)
                objUpdate.IdFormaPagto = null;

            //Verifica se o cliente possui contas a receber vencidas se nao for garantia
            if (ped.IdCli != objUpdate.IdCli && !FinanceiroConfig.PermitirFinalizacaoPedidoPeloFinanceiro &&
                (ClienteDAO.Instance.ObtemValorCampo<bool>(session, "bloquearPedidoContaVencida", "id_Cli=" + objUpdate.IdCli)) &&
                ContasReceberDAO.Instance.ClientePossuiContasVencidas(session, objUpdate.IdCli) && objUpdate.TipoVenda != (int)Pedido.TipoVendaPedido.Garantia)
                LancarExceptionValidacaoPedidoFinanceiro("Cliente bloqueado. Motivo: Contas a receber em atraso.", objUpdate.IdPedido, true, null, ObservacaoFinalizacaoFinanceiro.MotivoEnum.Finalizacao);

            // Remove o idObra caso não seja mais obra o tipo de venda
            var atualizarValorObra = false;
            if (objUpdate.TipoVenda != (int)Pedido.TipoVendaPedido.Obra)
            {
                objUpdate.IdObra = null;
                objUpdate.ValorPagamentoAntecipado = 0;

                // Se o pagamento era obra e nao é mais atualiza o saldo da mesma
                if (ped.TipoVenda == (int)Pedido.TipoVendaPedido.Obra)
                    atualizarValorObra = true;
            }

            // Se o comissionado não tiver sido informado, zera percComissao e valorComissao
            if (objUpdate.IdComissionado == null)
            {
                objUpdate.PercComissao = 0;
                objUpdate.ValorComissao = 0;
            }

            if (ped.Situacao == Pedido.SituacaoPedido.Confirmado)
                objUpdate.Situacao = Pedido.SituacaoPedido.Confirmado;

            objUpdate.IdProjeto = ped.IdProjeto;

            if (ped.BloquearDescontoAcrescimoAtualizar)
            {
                if (ped.Acrescimo != objUpdate.Acrescimo)
                    objUpdate.Acrescimo = ped.Acrescimo;

                if (ped.TipoAcrescimo != objUpdate.TipoAcrescimo)
                    objUpdate.TipoAcrescimo = ped.TipoAcrescimo;

                if (ped.Desconto != objUpdate.Desconto)
                    objUpdate.Desconto = ped.Desconto;

                if (ped.TipoDesconto != objUpdate.TipoDesconto)
                    objUpdate.TipoDesconto = ped.TipoDesconto;
            }

            // Recupera data e hora de cadastro da DataPedido
            if (objUpdate.DataPedido.Hour == 0)
                objUpdate.DataPedido = objUpdate.DataPedido.AddHours(ped.DataCad.Hour).AddMinutes(ped.DataCad.Minute).AddSeconds(ped.DataCad.Second);
                
            int retorno = UpdateBase(session, objUpdate);

            if (atualizarValorObra)
                ObraDAO.Instance.AtualizaSaldo(session, ped.IdObra.Value, false);

            // Remove e aplica comissão, desconto e acréscimo 
            RemoveComissaoDescontoAcrescimo(session, ped, objUpdate);
            AplicaComissaoDescontoAcrescimo(session, ped, objUpdate);

            UpdateTotalPedido(session, objUpdate.IdPedido, false, true,
                ped.Desconto != objUpdate.Desconto || ped.TipoDesconto != objUpdate.TipoDesconto);

            objUpdate.Total = GetTotal(session, objUpdate.IdPedido);

            // Atualiza a tabela com o valor da comissão
            if (objUpdate.Total > 0)
                PedidoComissaoDAO.Instance.Create(session, objUpdate);

            if (ped.IdCli != objUpdate.IdCli)
            {
                var parcelasCliente = ParcelasDAO.Instance.GetByCliente(session, objUpdate.IdCli, ParcelasDAO.TipoConsulta.Prazo);

                if (parcelasCliente != null && parcelasCliente.Count > 0 && parcelasCliente.All(f => f.IdParcela != (int)objUpdate.IdParcela.GetValueOrDefault()))
                    objUpdate.IdParcela = (uint)parcelasCliente[0].IdParcela;
                /* Chamado 54857. */
                else
                    objUpdate.IdParcela = null;
            }

            if ((objUpdate.FastDelivery != ped.FastDelivery &&
                objUpdate.Total != ParcelasPedidoDAO.Instance.ObtemTotalPorPedido(session, ped.IdPedido)) ||
                ped.IdCli != objUpdate.IdCli)
            {
                RecalculaParcelas(session, ref objUpdate, TipoCalculoParcelas.Valor);
                SalvarParcelas(session, objUpdate);
            }

            return retorno;
        }

        /// <summary>
        /// Método utilizado apenas para gerar pedido pelo projeto
        /// </summary>
        public uint InsertBase(Pedido objInsert)
        {
            return InsertBase(null, objInsert);
        }

        /// <summary>
        /// Método utilizado apenas para gerar pedido pelo projeto
        /// </summary>
        /// <param name="objInsert"></param>
        /// <returns></returns>
        public uint InsertBase(GDASession sessao, Pedido objInsert)
        {
            objInsert.Usucad = UserInfo.GetUserInfo.CodUser;
            objInsert.DataCad = DateTime.Now;

            if (objInsert.DataPedido.Hour == 0)
                objInsert.DataPedido = objInsert.DataPedido.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute).AddSeconds(DateTime.Now.Second);

            objInsert.IdPedido = base.Insert(sessao, objInsert);

            // Associa textos de pedidos padrões
            TextoPedidoDAO.Instance.AssociaTextoPedidoPadrao(sessao, objInsert.IdPedido);

            return objInsert.IdPedido;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// Método utilizado apenas para confirmar e cancelar pedido
        /// </summary>
        /// <param name="objUpdate"></param>
        public int UpdateBase(Pedido objUpdate)
        {
            return UpdateBase(null, objUpdate);
        }

        /// <summary>
        /// Método utilizado apenas para confirmar e cancelar pedido
        /// </summary>
        /// <param name="objUpdate"></param>
        public int UpdateBase(GDASession sessao, Pedido objUpdate)
        {
            /* Chamado 43090. */
            if (objUpdate.ValorEntrada < 0)
                throw new Exception("O valor de entrada do pedido não pode ser negativo. Edite o pedido, atualize os valores e tente novamente.");

            Pedido ped = GetElement(sessao, objUpdate.IdPedido);

            // Chamado 18844: Recupera o valor no banco do pagto antecipado deste pedido
            objUpdate.ValorPagamentoAntecipado = ped.ValorPagamentoAntecipado;

            // Chamado 24273: Ao remover a obra, o pedido continuava com o valorpagamentoantecipado pree
            if (objUpdate.IdObra.GetValueOrDefault() == 0 && objUpdate.IdPagamentoAntecipado.GetValueOrDefault() == 0)
                objUpdate.ValorPagamentoAntecipado = 0;

            int retorno = base.Update(sessao, objUpdate);

            LogAlteracaoDAO.Instance.LogPedido(sessao, ped, GetElement(objUpdate.IdPedido), LogAlteracaoDAO.SequenciaObjeto.Atual);

            //UpdateTotalPedido(objUpdate.IdPedido, objUpdate.Desconto);
            //PedidoComissaoDAO.Instance.Create(objUpdate);

            return retorno;
        }

        #endregion

        #region Busca pedidos para exportação

        public Pedido[] GetForPedidoExportar(uint idPedido, uint idCli, string nomeCli, string codCliente, string dataIni, string dataFim)
        {
            bool temFiltro;
            string filtroAdicional;

            // Situações de pedidos que são permitidas para exportação
            string situacoes = ((int)Pedido.SituacaoPedido.ConfirmadoLiberacao).ToString();

            if (!PedidoConfig.ExportacaoPedido.EsconderPedidosLiberados)
                situacoes += "," + (int)Pedido.SituacaoPedido.Confirmado;

            string sql = Sql(idPedido, 0, null, null, 0, idCli, nomeCli, 0, codCliente, 0, null, null, null, null,
                null, null, null, null, null, null, dataIni, dataFim, null, null, null, 0, false, false, 0, 0, 0,
                0, 0, null, 0, 0, 0, "", true, out filtroAdicional, out temFiltro);

            // Busca apenas pedidos nas situações confirmado e confirmado liberação (usando GDAParameter não funciona)
            filtroAdicional += " And p.situacao In (" + situacoes + ")";

            // Só busca pedidos que foram gerados PCP e que não esteja em aberto
            if (Geral.ControlePCP)
            {
                filtroAdicional += " And IF(pe.IdPedido is not null, pe.Situacao in (" + (int)PedidoEspelho.SituacaoPedido.Finalizado + "," +
                    (int)PedidoEspelho.SituacaoPedido.Impresso + "," + (int)PedidoEspelho.SituacaoPedido.ImpressoComum + "), 0)";
            }

            // Só busca pedidos não exportados
            filtroAdicional += String.Format(" and coalesce((" + PedidoExportacaoDAO.Instance.SqlSituacaoExportacao("p.idPedido") + "),{0})={0}",
                (int)PedidoExportacao.SituacaoExportacaoEnum.Cancelado);

            /* Chamado 55675. */
            filtroAdicional += string.Format(" AND IF((p.TipoPedido={0} AND p.TipoEntrega={1}) OR p.TipoPedido={2}, 1, 0)",
                (int)Pedido.TipoPedidoEnum.Revenda, (int)Pedido.TipoEntregaPedido.Entrega, (int)Pedido.TipoPedidoEnum.Venda);

            Pedido[] model = objPersistence.LoadData(sql.Replace(FILTRO_ADICIONAL, filtroAdicional) + " order by p.idPedido",
                GetParam(nomeCli, codCliente, null, null, null, null, dataIni, dataFim, null, null, "")).ToArray();

            return model;
        }

        #endregion

        #region Busca pedidos para finalização/confirmação Financeiro

        private string SqlFinalizarFinanc(uint idPedido, string codCliente, uint idCliente, string nomeCliente,
            uint idOrcamento, string endereco, string bairro, string dataPedidoIni, string dataPedidoFim,
            uint idLoja, int situacao, float alturaProd, int larguraProd, bool selecionar, out bool temFiltro,
            out string filtroAdicional)
        {
            StringBuilder sql = new StringBuilder("select ");
            var criterio = new StringBuilder();

            sql.Append(selecionar ? @"p.*, coalesce(l.nomeFantasia, l.razaoSocial) as NomeLoja,
                f.Nome as NomeFunc, (select cast(group_concat(distinct idItemProjeto) as char)
                from produtos_pedido where idPedido=p.idPedido) as idItensProjeto, tmp.motivoErroFinalizarFinanc, tmp.motivoErroConfirmarFinanc,
                '$$$' as criterio" : "count(*)");

            if (selecionar)
            {
                sql.Append(FinanceiroConfig.TelaFinalizacaoFinanceiro.ExibirRazaoSocial ?
                    ", COALESCE(cli.nome, cli.NomeFantasia) as NomeCliente" : ", COALESCE(cli.NomeFantasia, cli.nome) as NomeCliente");
            }

            sql.AppendFormat(@"
                from pedido p
                INNER JOIN cliente cli ON (p.idCli = cli.id_cli)
                LEFT JOIN loja l ON (p.idLoja = l.idLoja)
                LEFT JOIN funcionario f ON (p.idFunc = f.idFunc)
                LEFT JOIN (
                    SELECT * FROM (
                        SELECT idPedido, motivoErroFinalizarFinanc, motivoErroConfirmarFinanc
                        FROM observacao_finalizacao_financeiro
                        WHERE motivo=" + (int)ObservacaoFinalizacaoFinanceiro.MotivoEnum.Aberto + @" 
                        ORDER BY idObsFinanc DESC
                    ) as o
                    GROUP BY idPedido
                ) as tmp ON (p.idPedido = tmp.idPedido)
                where 1 {0}", FILTRO_ADICIONAL);

            temFiltro = false;
            StringBuilder fa = new StringBuilder(" and p.situacao in (" +
                (int)Pedido.SituacaoPedido.AguardandoConfirmacaoFinanceiro + "," +
                (int)Pedido.SituacaoPedido.AguardandoFinalizacaoFinanceiro + ")");

            if (idPedido > 0)
            {
                fa.AppendFormat(" and p.idPedido={0}", idPedido);
                criterio.Append("Pedido: " + idPedido + "     ");
            }

            if (!String.IsNullOrEmpty(codCliente))
                fa.Append(" and p.codCliente like ?codCliente");

            if (idCliente > 0)
            {
                fa.AppendFormat(" and p.idCli={0}", idCliente);
                criterio.Append("Cód Cliente: " + idCliente + "     ");
            }
            else if (!String.IsNullOrEmpty(nomeCliente) || !String.IsNullOrEmpty(endereco) || !String.IsNullOrEmpty(bairro))
            {
                string ids = ClienteDAO.Instance.GetIds(null, nomeCliente, null, 0, endereco, bairro, null, null, 0);
                fa.AppendFormat(" and p.idCli in ({0})", ids);
                criterio.Append("Cliente: " + nomeCliente + "     ");
            }

            if (idOrcamento > 0)
            {
                fa.AppendFormat(" and p.idOrcamento={0}", idOrcamento);
                criterio.Append("Orçamento: " + idOrcamento + "     ");
            }

            if (!String.IsNullOrEmpty(dataPedidoIni))
            {
                fa.Append(" and p.dataPedido>=?dataPedidoIni");
                criterio.Append("Data Cad. Ini: " + dataPedidoIni + "     ");
            }

            if (!String.IsNullOrEmpty(dataPedidoFim))
            {
                fa.Append(" and p.dataPedido<=?dataPedidoFim");
                criterio.Append("Data Cad. Fim: " + dataPedidoFim + "     ");
            }

            if (idLoja > 0)
            {
                fa.AppendFormat(" and p.idLoja={0}", idLoja);
                criterio.Append("Loja: " + LojaDAO.Instance.GetNome(idLoja) + "     ");
            }

            if (situacao > 0)
            {
                fa.AppendFormat(" and p.situacao={0}", situacao);
                var p = new Pedido() { Situacao = (Pedido.SituacaoPedido)situacao };
                criterio.Append("Situacao: " + p.DescrSituacaoPedido + "     ");
            }

            if (alturaProd > 0)
            {
                fa.Append(" and p.idPedido in (select * from (select idPedido from produtos_pedido where altura=?alturaProd) as temp)");
                criterio.Append("Altura Prod.: " + alturaProd + "     ");
            }

            if (larguraProd > 0)
            {
                fa.AppendFormat(" and p.idPedido in (select * from (select idPedido from produtos_pedido where largura={0}) as temp)", larguraProd);
                criterio.Append("Largura Prod.: " + larguraProd + "     ");
            }

            filtroAdicional = fa.ToString();
            return sql.ToString().Replace("$$$", criterio.ToString());
        }

        private GDAParameter[] GetParamFinalizarFinanc(string codCliente, string dataPedidoIni,
            string dataPedidoFim, float alturaProd)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(codCliente))
                lst.Add(new GDAParameter("?codCliente", "%" + codCliente + "%"));

            if (!String.IsNullOrEmpty(dataPedidoIni))
                lst.Add(new GDAParameter("?dataPedidoIni", DateTime.Parse(dataPedidoIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataPedidoFim))
                lst.Add(new GDAParameter("?dataPedidoFim", DateTime.Parse(dataPedidoFim + " 23:59:59")));

            if (alturaProd > 0)
                lst.Add(new GDAParameter("?alturaProd", alturaProd));

            return lst.ToArray();
        }

        public IList<Pedido> ObtemItensFinalizarFinanceiro(uint idPedido, string codCliente, uint idCliente, string nomeCliente,
            uint idOrcamento, string endereco, string bairro, string dataPedidoIni, string dataPedidoFim,
            uint idLoja, int situacao, float alturaProd, int larguraProd, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "p.idPedido desc";

            bool temFiltro;
            string filtroAdicional, sql = SqlFinalizarFinanc(idPedido, codCliente, idCliente, nomeCliente, idOrcamento,
                endereco, bairro, dataPedidoIni, dataPedidoFim, idLoja, situacao, alturaProd, larguraProd, true,
                out temFiltro, out filtroAdicional);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional,
                GetParamFinalizarFinanc(codCliente, dataPedidoIni, dataPedidoFim, alturaProd));
        }

        public int ObtemNumeroItensFinalizarFinanceiro(uint idPedido, string codCliente, uint idCliente, string nomeCliente,
            uint idOrcamento, string endereco, string bairro, string dataPedidoIni, string dataPedidoFim,
            uint idLoja, int situacao, float alturaProd, int larguraProd)
        {
            bool temFiltro;
            string filtroAdicional, sql = SqlFinalizarFinanc(idPedido, codCliente, idCliente, nomeCliente, idOrcamento,
                endereco, bairro, dataPedidoIni, dataPedidoFim, idLoja, situacao, alturaProd, larguraProd, true,
                out temFiltro, out filtroAdicional);

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, GetParamFinalizarFinanc(codCliente,
                dataPedidoIni, dataPedidoFim, alturaProd));
        }

        public IList<Pedido> ObtemItensFinalizarFinanceiroRpt(uint idPedido, string codCliente, uint idCliente, string nomeCliente,
            uint idOrcamento, string endereco, string bairro, string dataPedidoIni, string dataPedidoFim,
            uint idLoja, int situacao, float alturaProd, int larguraProd)
        {
            bool temFiltro;
            string filtroAdicional, sql = SqlFinalizarFinanc(idPedido, codCliente, idCliente, nomeCliente, idOrcamento,
                endereco, bairro, dataPedidoIni, dataPedidoFim, idLoja, situacao, alturaProd, larguraProd, true,
                out temFiltro, out filtroAdicional);

            return objPersistence.LoadData(sql.Replace(FILTRO_ADICIONAL, filtroAdicional),
                GetParamFinalizarFinanc(codCliente, dataPedidoIni, dataPedidoFim, alturaProd)).OrderByDescending(f => f.IdPedido).ToList();
        }


        #endregion

        public Pedido[] GetForEtiquetas(string idsProdPed, string idsAmbPed)
        {
            string sql = "select * from pedido where false";

            if (!String.IsNullOrEmpty(idsProdPed) && idsProdPed != "0")
                sql += " or idPedido in (select distinct idPedido from produtos_pedido_espelho where idProdPed in (" + idsProdPed + "))";

            if (!String.IsNullOrEmpty(idsAmbPed) && idsAmbPed != "0")
                sql += " or idPedido in (select distinct idPedido from ambiente_pedido_espelho where idAmbientePedido in (" + idsAmbPed + "))";

            return objPersistence.LoadData(sql).ToArray();
        }

        public float ObtemPercComissaoAdmin(uint idPedido)
        {
            return ObtemValorCampo<float>("PercentualComissao", "IdPedido=" + idPedido);
        }

        public int ObtemQuantidadePecas(GDASession session, uint idPedido)
        {
            if (!PedidoConfig.LiberarPedido)
                return 0;

            var sql = string.Format(@"SELECT CAST(SUM(COALESCE(Qtde, 0)) AS SIGNED INTEGER) FROM produtos_pedido pp 
                    LEFT JOIN produto p ON (pp.IdProd=p.IdProd)
                WHERE IdPedido=?id AND (Invisivel{0} IS NULL OR Invisivel{0}=0) AND p.IdGrupoProd={1}",
                PCPConfig.UsarConferenciaFluxo ? "Fluxo" : "Pedido", (int)NomeGrupoProd.Vidro);

            return ExecuteScalar<int>(session, sql, new GDAParameter("?id", idPedido));
        }

        #region Gerar Pedido

        /// <summary>
        /// Gera um pedido a partir do orçamento passado
        /// </summary>
        public uint GerarPedido(uint idOrcamento)
        {
            var login = UserInfo.GetUserInfo;

            lock (_gerarPedidoLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        #region Declaração de variáveis

                        // Busca o orçamento.
                        var orcamento = OrcamentoDAO.Instance.GetElementByPrimaryKey(transaction, idOrcamento);
                        // Produtos do orçamento.
                        var produtosOrcamento = ProdutosOrcamentoDAO.Instance.GetByOrcamento(transaction, idOrcamento, false);
                        // Verifica se existe algum pedido, gerado através do orçamento atual, que não esteja cancelado, nesse caso, o orçamento não pode gerar um novo pedido.
                        var idPedidoNaoCanceladoAssociadoOrcamento = ExecuteScalar<int?>(transaction, string.Format("SELECT IdPedido FROM pedido WHERE Situacao<>{0} AND IdOrcamento={1}",
                            (int)Pedido.SituacaoPedido.Cancelado, orcamento.IdOrcamento));
                        // Recupera a medição mais recente do orçamento.
                        var idMedicaoMaisRecente = !string.IsNullOrWhiteSpace(orcamento.IdsMedicao) ?
                            orcamento.IdsMedicao.Split(',').Select(f => f.StrParaInt()).Where(f => f > 0).OrderByDescending(f => f).First() : 0;
                        // Verifica se o cliente possui desconto.
                        var clientePossuiDesconto = DescontoAcrescimoClienteDAO.Instance.ClientePossuiDesconto(transaction, orcamento.IdCliente.Value, idOrcamento, null, 0, null);
                        // Verifica se o cliente poossui contas vencidas.
                        var clientePossuiContasVencidas = ContasReceberDAO.Instance.ClientePossuiContasVencidas(transaction, orcamento.IdCliente.Value);
                        // Recupera a aituação atual do cliente.
                        var situacaoCliente = ClienteDAO.Instance.GetSituacao(transaction, orcamento.IdCliente.Value);
                        // Verifica se o cliente deve ser bloqueado caso existam contas vencidas.
                        var clienteBloquearContaVencida = ClienteDAO.Instance.ObtemValorCampo<bool>(transaction, "bloquearPedidoContaVencida", string.Format("Id_Cli={0}", orcamento.IdCliente));
                        // Recupera o funcionario associado ao cliente.
                        var idVendCliente = ClienteDAO.Instance.ObtemIdFunc(transaction, orcamento.IdCliente.Value);
                        // Recupera a parcela padrão do cliente.
                        var tipoPagto = ClienteDAO.Instance.ObtemTipoPagto(transaction, orcamento.IdCliente.Value);

                        DateTime dataEntrega, dataFastDelivery;
                        uint idPedido = 0;
                        uint idProdPed = 0;

                        #endregion

                        #region Validações

                        // Verifica se ao menos um produto do orçamento foi marcado para gerar pedido (Negociar?).
                        if (OrcamentoConfig.NegociarParcialmente && !produtosOrcamento.Any(f => f.IdProdPed.GetValueOrDefault() == 0 && f.Negociar))
                            throw new Exception("Selecione pelo menos 1 produto para ser negociado.");

                        // Verifica se o vendedor do orçamento foi selecionado.
                        if (orcamento.IdFuncionario.GetValueOrDefault() == 0)
                            throw new Exception("Selecione um vendedor para este orçamento antes de gerar pedido.");

                        // Verifica se o tipo do orçamento foi selecionado.
                        if (orcamento.TipoOrcamento.GetValueOrDefault() == 0)
                            throw new Exception("Selecione o tipo do orçamento.");

                        // Verifica se o cliente foi informado.
                        if (orcamento.IdCliente == null || orcamento.IdCliente == 0)
                            throw new Exception("Cadastre o cliente informado no orçamento antes de gerar pedido.");

                        // Impede a geração do pedido caso o cliente não esteja ativo.
                        if (situacaoCliente != (int)SituacaoCliente.Ativo)
                            throw new Exception("O cliente não está ativo.");

                        // Verifica se o cliente possui contas a receber vencidas se nao for garantia.
                        if (!FinanceiroConfig.PermitirFinalizacaoPedidoPeloFinanceiro && clienteBloquearContaVencida && clientePossuiContasVencidas)
                            throw new Exception("Cliente bloqueado. Motivo: Contas a receber em atraso.");

                        // Verifica se este orçamento pode ter desconto.
                        if (PedidoConfig.Desconto.ImpedirDescontoSomativo && clientePossuiDesconto && orcamento.Desconto > 0 && !login.IsAdministrador)
                            throw new Exception("O cliente já possui desconto por grupo/subgrupo, não é permitido lançar outro desconto.");

                        // Verifica se já foi gerado um pedido para este orçamento.
                        if (orcamento.Situacao != (int)Orcamento.SituacaoOrcamento.NegociadoParcialmente && idPedidoNaoCanceladoAssociadoOrcamento.GetValueOrDefault() > 0)
                            throw new Exception(string.Format("Já foi gerado um pedido para este orçamento. Número do pedido: {0}.", idPedidoNaoCanceladoAssociadoOrcamento));

                        // Verifica se existem produtos no orçamento.
                        if (ExecuteScalar<bool>(transaction, string.Format("SELECT COUNT(*)=0 FROM produtos_orcamento WHERE IdOrcamento={0}", idOrcamento)))
                            throw new Exception("Insira pelo menos um item neste orçamento antes de gerar pedido.");

                        /* Chamado 56301. */
                        if (produtosOrcamento.Any(f => f.IdProduto > 0 && f.IdSubgrupoProd.GetValueOrDefault() == 0))
                            throw new Exception(string.Format("Informe o subgrupo dos produtos {0} antes de gerar o pedido.",
                                string.Join(", ", produtosOrcamento.Where(f => f.IdProduto > 0 && f.IdSubgrupoProd == 0).Select(f => f.CodInterno).Distinct().ToList())));

                        #endregion

                        #region Bloqueio itens tipo pedido

                        if (orcamento.TipoOrcamento != null && PedidoConfig.DadosPedido.BloquearItensTipoPedido)
                        {
                            var idProdutoComparar = 0;
                            var idCorVidroProdutoComparar = 0;
                            float espessuraProdutoComparar = 0;
                            var lojaBloqueaItensCorEspessura = LojaDAO.Instance.GetIgnorarBloquearItensCorEspessura(transaction, orcamento.IdLoja.GetValueOrDefault());
                            var materiaisVidroMesmaCorEspessura = MaterialItemProjetoDAO.Instance.VidrosMesmaCorEspessura(transaction, idOrcamento);

                            // Impede que o pedido seja gerado com produtos de cor e espessura diferentes. (Materiais de projeto)
                            if ((PedidoConfig.DadosPedido.BloquearItensCorEspessura && !lojaBloqueaItensCorEspessura) && orcamento.TipoOrcamento == (int)Orcamento.TipoOrcamentoEnum.Venda &&
                                !materiaisVidroMesmaCorEspessura)
                                throw new Exception("Não é possível incluir produtos de cor e espessura diferentes.");

                            foreach (var po in produtosOrcamento.Where(f => f.IdProduto > 0))
                            {
                                // Não negocia os produtos já negociados ou que não serão negociados
                                if (OrcamentoConfig.NegociarParcialmente && (po.IdProdPed != null || !po.Negociar))
                                    continue;

                                if (idProdutoComparar == 0)
                                {
                                    idProdutoComparar = (int)po.IdProduto.Value;
                                    idCorVidroProdutoComparar = ProdutoDAO.Instance.ObtemIdCorVidro(transaction, idProdutoComparar).GetValueOrDefault();
                                    espessuraProdutoComparar = ProdutoDAO.Instance.ObtemEspessura(transaction, idProdutoComparar);
                                }

                                var idCorVidro = ProdutoDAO.Instance.ObtemIdCorVidro(transaction, (int)po.IdProduto.Value);
                                var espessura = ProdutoDAO.Instance.ObtemEspessura(transaction, (int)po.IdProduto.Value);
                                var idGrupoProd = ProdutoDAO.Instance.ObtemIdGrupoProd(transaction, (int)po.IdProduto.Value);
                                var idSubgrupoProd = ProdutoDAO.Instance.ObtemIdSubgrupoProd(transaction, (int)po.IdProduto.Value);
                                var subgrupoProducao = SubgrupoProdDAO.Instance.IsSubgrupoProducao(transaction, idGrupoProd, idSubgrupoProd);

                                if (orcamento.TipoOrcamento == (int)Orcamento.TipoOrcamentoEnum.Venda && idGrupoProd != (uint)NomeGrupoProd.MaoDeObra &&
                                    (idGrupoProd != (uint)NomeGrupoProd.Vidro || (idGrupoProd == (uint)NomeGrupoProd.Vidro && subgrupoProducao)))
                                {
                                    throw new Exception("Não é possível incluir produtos de revenda em um pedido de venda.");
                                }
                                if (orcamento.TipoOrcamento == (int)Orcamento.TipoOrcamentoEnum.Revenda &&
                                    ((idGrupoProd == (uint)NomeGrupoProd.Vidro && !subgrupoProducao) || idGrupoProd == (uint)NomeGrupoProd.MaoDeObra))
                                {
                                    throw new Exception("Não é possível incluir produtos de venda em um pedido de revenda.");
                                }
                                // Impede que o pedido seja gerado com produtos de cor e espessura diferentes.
                                else if ((PedidoConfig.DadosPedido.BloquearItensCorEspessura && !lojaBloqueaItensCorEspessura) && orcamento.TipoOrcamento == (int)Orcamento.TipoOrcamentoEnum.Venda &&
                                    idGrupoProd == (uint)NomeGrupoProd.Vidro && (idCorVidro != idCorVidroProdutoComparar || espessura != espessuraProdutoComparar))
                                {
                                    throw new Exception("Não é possível incluir produtos de cor e espessura diferentes.");
                                }
                            }
                        }

                        #endregion

                        #region Insere o pedido

                        var pedido = new Pedido
                        {
                            IdLoja = orcamento.IdLoja > 0 ? orcamento.IdLoja.Value : login.IdLoja,
                            IdFunc = PedidoConfig.DadosPedido.BuscarVendedorEmitirPedido && idVendCliente > 0 ? idVendCliente.Value : login.CodUser,
                            IdCli = orcamento.IdCliente.Value,
                            IdOrcamento = idOrcamento,
                            IdProjeto = orcamento.IdProjeto,
                            TipoEntrega = orcamento.TipoEntrega,
                            TipoVenda = orcamento.TipoVenda,
                            Situacao = Pedido.SituacaoPedido.Ativo,
                            DataPedido = DateTime.Now,
                            FromOrcamentoRapido = true,
                            CustoPedido = orcamento.Custo,
                            Total = orcamento.Total,
                            EnderecoObra = orcamento.EnderecoObra,
                            BairroObra = orcamento.BairroObra,
                            CidadeObra = orcamento.CidadeObra,
                            CepObra = orcamento.CepObra,
                            Obs = orcamento.Obs,
                            GerarPedidoProducaoCorte = false,
                            TipoPedido = orcamento.TipoOrcamento.GetValueOrDefault((int)Pedido.TipoPedidoEnum.Venda),
                            ValorEntrega = orcamento.ValorEntrega,
                            NumParc = orcamento.NumParc,
                            IdParcela = orcamento.IdParcela,
                            PrazoEntrega = orcamento.PrazoEntrega,
                            DataEntrega = (GetDataEntregaMinima(transaction, orcamento.IdCliente.Value, null, orcamento.TipoOrcamento.GetValueOrDefault((int)Pedido.TipoPedidoEnum.Venda), orcamento.TipoEntrega,
                                out dataEntrega, out dataFastDelivery) ?
                                dataEntrega : RotaDAO.Instance.GetDataRota(transaction, orcamento.IdCliente.Value, orcamento.DataEntrega != null ? orcamento.DataEntrega.Value : DateTime.Now)) ?? orcamento.DataEntrega,
                            IdMedidor = idMedicaoMaisRecente > 0 ? MedicaoDAO.Instance.GetMedidor(transaction, (uint)idMedicaoMaisRecente) : null,
                            PercentualComissao = PedidoConfig.Comissao.PerComissaoPedido ? ClienteDAO.Instance.ObtemPercentualComissao(transaction, orcamento.IdCliente.Value) : 0,

                            // Chamado 68242: Insere o acréscimo no pedido, para que ao removê-lo e adicioná-lo novamente logo abaixo 
                            // nos métodos "RemoveComissaoDescontoAcrescimo" e "AplicaComissaoDescontoAcrescimo", não seja aplicado novamente sem que seja removido
                            // (que é o que acontece se não preenhcer o acréscimo e tipo acréscimo neste momento)
                            Acrescimo = orcamento.Acrescimo,
                            TipoAcrescimo = orcamento.TipoAcrescimo
                        };

                        if (tipoPagto > 0)
                        {
                            var parcelaPadrao = ParcelasDAO.Instance.GetElementByPrimaryKey(transaction, tipoPagto.Value);

                            if (parcelaPadrao != null && parcelaPadrao.NumParcelas > 0)
                                pedido.TipoVenda = (int)Pedido.TipoVendaPedido.APrazo;
                        }

                        idPedido = InsertBase(transaction, pedido);

                        if (idPedido == 0)
                            throw new Exception("Inserção do pedido retornou 0.");

                        // Insere o id do pedido no campo idPedidoGerado do orçamento
                        objPersistence.ExecuteCommand(transaction, string.Format("UPDATE orcamento SET IdPedidoGerado={0} WHERE IdOrcamento={1}", idPedido, idOrcamento));

                        #endregion

                        // Se a empresa não trabalha com venda de vidro, a forma de gerar pedido é diferente
                        if (Geral.NaoVendeVidro())
                        {
                            #region Inserção de produtos para empresas que NÃO VENDEM vidro

                            foreach (var po in produtosOrcamento)
                            {
                                // O custo do produto de orçamento é atualizado somente se o cliente estiver inserido no orçamento, 
                                // para certificar que o custo inserido no pedido será o valor correto é necessário atualizar novamente
                                decimal custoProdTemp = 0, totalTemp = 0;
                                float alturaTemp = 0, totM2Temp = 0;
                                decimal valorProd = po.ValorProd != null ? po.ValorProd.Value : 0;

                                // Não negocia os produtos já negociados ou que não serão negociados
                                if (OrcamentoConfig.NegociarParcialmente && (po.IdProdPed != null || !po.Negociar))
                                    continue;

                                var prodPed = new ProdutosPedido
                                {
                                    IdPedido = idPedido,
                                    IdProd = po.IdProduto.Value,
                                    Qtde = po.Qtde.Value,
                                    TotM = po.TotM,
                                    TotM2Calc = po.TotMCalc,
                                    Altura = po.AlturaCalc,
                                    AlturaReal = po.Altura,
                                    Largura = po.Largura,
                                    CustoProd = po.Custo,
                                    AliqIcms = po.AliquotaIcms,
                                    ValorIcms = po.ValorIcms,
                                    AliqIpi = po.AliquotaIpi,
                                    ValorIpi = po.ValorIpi,
                                    Redondo = po.Redondo,
                                    ValorTabelaOrcamento = po.ValorTabela,
                                    ValorTabelaPedido = ProdutoDAO.Instance.GetValorTabela(transaction, (int)po.IdProduto.Value, pedido.TipoEntrega, pedido.IdCli, false, false, po.PercDescontoQtde,
                                        (int?)idPedido, null, null),
                                    TipoCalculoUsadoOrcamento = po.TipoCalculoUsado,
                                    TipoCalculoUsadoPedido = GrupoProdDAO.Instance.TipoCalculo(transaction, (int)po.IdProduto.Value),
                                    PercDescontoQtde = po.PercDescontoQtde,
                                    ValorDescontoQtde = po.ValorDescontoQtde,
                                    ValorDescontoCliente = po.ValorDescontoCliente,
                                    ValorAcrescimoCliente = po.ValorAcrescimoCliente,
                                    ValorUnitarioBruto = po.ValorUnitarioBruto,
                                    TotalBruto = po.TotalBruto,
                                    IdProcesso = po.IdProcesso,
                                    IdAplicacao = po.IdAplicacao
                                };

                                ProdutoDAO.Instance.CalcTotaisItemProd(transaction, pedido.IdCli, (int)prodPed.IdProd, po.Largura, prodPed.Qtde, prodPed.QtdeAmbiente, valorProd, po.Espessura,
                                    po.Redondo, 2, false, ref custoProdTemp, ref alturaTemp, ref totM2Temp, ref totalTemp, false, po.Beneficiamentos.CountAreaMinimaSession(transaction));

                                prodPed.CustoProd = custoProdTemp > 0 ? custoProdTemp : po.Custo;

                                // O valor vendido e o total devem ser preenchidos, assim como os outros campos abaixo, 
                                // caso contrário o valor deste produto ficaria zerado ou incorreto no pedido, antes,
                                // todos os campos abaixo estavam sendo preenchidos apenas se a opção PedidoConfig.DadosPedido.AlterarValorUnitarioProduto fosse true
                                prodPed.ValorVendido = valorProd;
                                prodPed.Total = po.Total.Value;
                                prodPed.ValorAcrescimo = po.ValorAcrescimo + (PedidoConfig.DadosPedido.AmbientePedido ? 0 : po.ValorAcrescimoProd);
                                prodPed.ValorDesconto = po.ValorDesconto + (PedidoConfig.DadosPedido.AmbientePedido ? 0 : po.ValorDescontoProd);
                                prodPed.ValorAcrescimoProd = !PedidoConfig.DadosPedido.AmbientePedido ? 0 : po.ValorAcrescimoProd;
                                prodPed.ValorDescontoProd = !PedidoConfig.DadosPedido.AmbientePedido ? 0 : po.ValorDescontoProd;
                                prodPed.ValorComissao = PedidoConfig.Comissao.ComissaoPedido ? po.ValorComissao : 0;
                                prodPed.RemoverDescontoQtde = true;
                                idProdPed = ProdutosPedidoDAO.Instance.InsertBase(transaction, prodPed);

                                if (idProdPed == 0)
                                    throw new Exception("Inserção do produto do pedido retornou 0.");

                                // Atualiza o produto, indicando o produto do pedido que foi gerado
                                if (idProdPed > 0)
                                    objPersistence.ExecuteCommand(transaction, string.Format("UPDATE produtos_orcamento SET IdProdPed={0} WHERE IdProd={1}", idProdPed, po.IdProd));
                            }

                            #endregion
                        }
                        else
                        {
                            #region Inserção de produtos para empresas que VENDEM vidro

                            var pedidoReposicaoGarantia = pedido.TipoVenda == (int)Pedido.TipoVendaPedido.Reposição || pedido.TipoVenda == (int)Pedido.TipoVendaPedido.Garantia;
                            var pedidoMaoObraEspecial = pedido.TipoPedido == (int)Pedido.TipoPedidoEnum.MaoDeObraEspecial;

                            foreach (var po in produtosOrcamento)
                            {
                                // Não negocia os produtos já negociados ou que não serão negociados
                                if (OrcamentoConfig.NegociarParcialmente && (po.IdProdPed != null || !po.Negociar))
                                    continue;

                                uint? idAmbiente = null; // Ambiente do pedido
                                var itensProjetoId = new Dictionary<uint, uint>();

                                // Cria um ambiente se a empresa trabalha com ambiente no pedido ou 
                                // se o produto do orçamento for um cálculo de projeto
                                if (PedidoConfig.DadosPedido.AmbientePedido || po.IdItemProjeto > 0)
                                {
                                    // Se o produto for um cálculo de projeto, faz uma cópia para o pedido
                                    if (po.IdItemProjeto > 0 && !itensProjetoId.ContainsKey(po.IdItemProjeto.Value))
                                    {
                                        var idItemProjeto = ClonaItemProjeto(transaction, po.IdItemProjeto.Value, idPedido);
                                        itensProjetoId.Add(po.IdItemProjeto.Value, idItemProjeto);
                                    }

                                    var ambiente = new AmbientePedido
                                    {
                                        IdPedido = idPedido,
                                        Ambiente = po.Ambiente,
                                        Descricao = po.Descricao,
                                        IdItemProjeto = po.IdItemProjeto != null ? (uint?)itensProjetoId[po.IdItemProjeto.Value] : null
                                    };

                                    // Na Center Box/Mega Temper, a impressão do pedido é igual do orçamento, portanto, 
                                    // precisa mostrar a quantidade na impressão do pedido 
                                    if (!PedidoConfig.RelatorioPedido.ExibirItensProdutosPedido)
                                        ambiente.Qtde = (int)po.Qtde;

                                    idAmbiente = AmbientePedidoDAO.Instance.Insert(transaction, ambiente);

                                    // Correção Mega Temper
                                    // Insere os produtos de projeto através do método específico
                                    if (ambiente.IdItemProjeto > 0)
                                    {
                                        var itemProjeto = ItemProjetoDAO.Instance.GetElement(transaction, ambiente.IdItemProjeto.Value);

                                        ProdutosPedidoDAO.Instance.InsereAtualizaProdProjSemAtualizarTotalPedido(transaction, idPedido, idAmbiente, itemProjeto, true);
                                    }

                                    // Atualiza os dados de desconto/acréscimo do ambiente.
                                    objPersistence.ExecuteCommand(transaction, string.Format(@"UPDATE ambiente_pedido SET TipoDesconto=?tipoDesconto, Desconto=?desconto, TipoAcrescimo=?tipoAcrescimo,
                                        Acrescimo=?acrescimo, Descricao=?descricao WHERE IdAmbientePedido={0}", idAmbiente), new GDAParameter("?tipoDesconto", po.TipoDesconto),
                                        new GDAParameter("?desconto", po.Desconto), new GDAParameter("?tipoAcrescimo", po.TipoAcrescimo), new GDAParameter("?acrescimo", po.Acrescimo),
                                        new GDAParameter("?descricao", po.Descricao));

                                    idProdPed = idAmbiente.Value;
                                }

                                // Adiciona os itens internos como os produtos do pedido
                                if (po.TemItensProdutoSession(transaction))
                                {
                                    foreach (var poChild in ProdutosOrcamentoDAO.Instance.GetByProdutoOrcamento(transaction, po.IdProd))
                                    {
                                        // O custo do produto de orçamento é atualizado somente se o cliente estiver inserido no orçamento, 
                                        // para certificar que o custo inserido no pedido será o valor correto é necessário atualizar novamente
                                        decimal custoProdTemp = 0, totalTemp = 0;
                                        float alturaTemp = 0, totM2Temp = 0;
                                        decimal valorProd = poChild.ValorProd != null ? po.ValorProd.Value : 0;

                                        var prodPed = new ProdutosPedido
                                        {
                                            IdPedido = idPedido,
                                            IdAmbientePedido = idAmbiente,
                                            IdItemProjeto = poChild.IdItemProjeto != null ? (uint?)itensProjetoId[poChild.IdItemProjeto.Value] : null,
                                            IdProd = poChild.IdProduto != null ? poChild.IdProduto.Value : 0,
                                            Qtde = poChild.Qtde != null ? poChild.Qtde.Value : 0,
                                            TotM = poChild.TotM,
                                            TotM2Calc = poChild.TotMCalc,
                                            Altura = poChild.AlturaCalc,
                                            AlturaReal = poChild.Altura,
                                            Largura = poChild.Largura,
                                            Espessura = poChild.Espessura > 0 ? poChild.Espessura : poChild.IdProduto > 0 ?
                                            ProdutoDAO.Instance.ObtemEspessura(transaction, (int)poChild.IdProduto.Value) : 0
                                        };

                                        ProdutoDAO.Instance.CalcTotaisItemProd(transaction, pedido.IdCli, (int)prodPed.IdProd, po.Largura, prodPed.Qtde, prodPed.QtdeAmbiente, valorProd,
                                            poChild.Espessura, poChild.Redondo, 2, false, ref custoProdTemp, ref alturaTemp, ref totM2Temp,
                                            ref totalTemp, false, poChild.Beneficiamentos.CountAreaMinimaSession(transaction));

                                        prodPed.CustoProd = custoProdTemp > 0 ? custoProdTemp : poChild.Custo;
                                        prodPed.AliqIcms = poChild.AliquotaIcms;
                                        prodPed.ValorIcms = poChild.ValorIcms;
                                        prodPed.AliqIpi = poChild.AliquotaIpi;
                                        prodPed.ValorIpi = poChild.ValorIpi;
                                        prodPed.Redondo = poChild.Redondo;
                                        prodPed.ValorTabelaOrcamento = poChild.ValorTabela;
                                        prodPed.ValorTabelaPedido = ProdutoDAO.Instance.GetValorTabela(transaction, (int)prodPed.IdProd, pedido.TipoEntrega, pedido.IdCli, false, false,
                                            poChild.PercDescontoQtde, (int)prodPed.IdPedido, null, null);
                                        prodPed.TipoCalculoUsadoOrcamento = poChild.TipoCalculoUsado;
                                        prodPed.TipoCalculoUsadoPedido = GrupoProdDAO.Instance.TipoCalculo(transaction, (int)prodPed.IdProd);
                                        prodPed.PercDescontoQtde = poChild.PercDescontoQtde;
                                        prodPed.ValorDescontoQtde = poChild.ValorDescontoQtde;
                                        prodPed.ValorDescontoCliente = poChild.ValorDescontoCliente;
                                        prodPed.ValorAcrescimoCliente = poChild.ValorAcrescimoCliente;
                                        prodPed.Beneficiamentos = poChild.Beneficiamentos;
                                        prodPed.ValorUnitarioBruto = poChild.ValorUnitarioBruto;
                                        prodPed.TotalBruto = poChild.TotalBruto;
                                        prodPed.CustoProd = poChild.Custo;
                                        prodPed.IdProcesso = poChild.IdProcesso;
                                        prodPed.IdAplicacao = poChild.IdAplicacao;
                                        // O valor vendido e o total devem ser preenchidos, assim como os outros campos abaixo, 
                                        // caso contrário o valor deste produto ficaria zerado ou incorreto no pedido, antes,
                                        // todos os campos abaixo estavam sendo preenchidos apenas se a opção PedidoConfig.DadosPedido.AlterarValorUnitarioProduto fosse true
                                        prodPed.ValorVendido = poChild.ValorProd != null ? poChild.ValorProd.Value : 0;
                                        prodPed.Total = poChild.Total != null ? poChild.Total.Value : 0;
                                        prodPed.ValorAcrescimo = poChild.ValorAcrescimo + (PedidoConfig.DadosPedido.AmbientePedido ? 0 : poChild.ValorAcrescimoProd);
                                        prodPed.ValorDesconto = poChild.ValorDesconto + (PedidoConfig.DadosPedido.AmbientePedido ? 0 : poChild.ValorDescontoProd);
                                        prodPed.ValorAcrescimoProd = !PedidoConfig.DadosPedido.AmbientePedido ? 0 : poChild.ValorAcrescimoProd;
                                        prodPed.ValorDescontoProd = !PedidoConfig.DadosPedido.AmbientePedido ? 0 : poChild.ValorDescontoProd;
                                        prodPed.ValorComissao = PedidoConfig.Comissao.ComissaoPedido ? poChild.ValorComissao : 0;
                                        prodPed.RemoverDescontoQtde = true;

                                        // Verifica se o valor unitário do produto foi informado, pois pode acontecer do usuário inserir produtos zerados
                                        // o que não é permitido em pedidos que não são de produção, reposição ou garantia.
                                        if (!pedidoReposicaoGarantia && prodPed.ValorVendido == 0)
                                            throw new Exception(string.Format("O produto {0} não pode ter valor zerado.", ProdutoDAO.Instance.ObtemDescricao(transaction, (int)prodPed.IdProd)));

                                        idProdPed = ProdutosPedidoDAO.Instance.InsertBase(transaction, prodPed);

                                        if (idProdPed == 0)
                                            throw new Exception("Inserção do produto do pedido retornou 0.");

                                        //Caso o produto seja do subgrupo de tipo laminado, insere os filhos
                                        var tipoSubgrupoProd = SubgrupoProdDAO.Instance.ObtemTipoSubgrupo(transaction, (int)prodPed.IdProd);

                                        if (tipoSubgrupoProd == TipoSubgrupoProd.VidroLaminado || tipoSubgrupoProd == TipoSubgrupoProd.VidroDuplo)
                                        {
                                            var tipoEntrega = ObtemTipoEntrega(transaction, prodPed.IdPedido);

                                            foreach (var p in ProdutoBaixaEstoqueDAO.Instance.GetByProd(transaction, prodPed.IdProd, false))
                                            {
                                                ProdutosPedidoDAO.Instance.Insert(transaction, new ProdutosPedido()
                                                {
                                                    IdProdPedParent = prodPed.IdProdPed,
                                                    IdProd = (uint)p.IdProdBaixa,
                                                    IdProcesso = (uint)p.IdProcesso,
                                                    IdAplicacao = (uint)p.IdAplicacao,
                                                    IdPedido = prodPed.IdPedido,
                                                    IdAmbientePedido = prodPed.IdAmbientePedido,
                                                    Qtde = p.Qtde,
                                                    Altura = p.Altura > 0 ? p.Altura : prodPed.Altura,
                                                    Largura = p.Largura > 0 ? p.Largura : prodPed.Largura,
                                                    ValorVendido = ProdutoDAO.Instance.GetValorTabela(transaction, p.IdProdBaixa, tipoEntrega, prodPed.IdCliente, false, false, 0, (int)prodPed.IdPedido, null, null),
                                                }, false, true);
                                            }
                                        }

                                    }
                                }

                                // Atualiza o produto, indicando o produto do pedido que foi gerado
                                if (idProdPed > 0)
                                    objPersistence.ExecuteCommand(transaction, string.Format("UPDATE produtos_orcamento SET IdProdPed={0} WHERE IdProd={1}", idProdPed, po.IdProd));
                            }

                            #endregion
                        }

                        // Finaliza o projeto
                        if (orcamento.IdProjeto != null)
                            ProjetoDAO.Instance.Finaliza(transaction, orcamento.IdProjeto.Value);

                        if (OrcamentoConfig.NegociarParcialmente)
                        {
                            var situacao = OrcamentoDAO.Instance.IsNegociadoParcialmente(transaction, idOrcamento) ?
                                (int)Orcamento.SituacaoOrcamento.NegociadoParcialmente : (int)Orcamento.SituacaoOrcamento.Negociado;

                            objPersistence.ExecuteCommand(transaction, string.Format("UPDATE orcamento SET Situacao={0} WHERE IdOrcamento={1}", situacao, idOrcamento));
                        }

                        if (!PedidoConfig.DadosPedido.AlterarValorUnitarioProduto)
                        {
                            // Atualiza o pedido, recalculando os valores dos produtos.
                            pedido = GetElementByPrimaryKey(transaction, idPedido);
                            pedido.TipoEntrega = orcamento.TipoEntrega;
                            pedido.ValoresParcelas = new decimal[] { pedido.Total };
                            pedido.DatasParcelas = new DateTime[] { DateTime.Now };

                            Update(transaction, pedido);

                            // Marca novamente os projetos como conferido.
                            foreach (var item in ItemProjetoDAO.Instance.GetByPedido(transaction, idPedido))
                                ItemProjetoDAO.Instance.CalculoConferido(transaction, item.IdItemProjeto);
                        }

                        #region Comissão/Desconto/Acréscimo

                        // Remove o percentual de comissão
                        objPersistence.ExecuteCommand(transaction, string.Format("UPDATE pedido SET PercComissao=0 WHERE IdPedido={0}", idPedido));

                        // Salva no pedido o funcionário que aplicou o desconto no orçamento.
                        if (orcamento.Desconto > 0)
                        {
                            var idFuncDesc = OrcamentoDAO.Instance.ObtemIdFuncDesc(transaction, orcamento.IdOrcamento);

                            /* Chamado 29245. */
                            if (idFuncDesc.GetValueOrDefault() == 0)
                                idFuncDesc = UserInfo.GetUserInfo.CodUser;

                            objPersistence.ExecuteCommand(transaction, string.Format("UPDATE pedido SET IdFuncDesc={0} WHERE IdPedido={1}", idFuncDesc, idPedido));
                        }

                        RemoveComissaoDescontoAcrescimo(transaction, idPedido);

                        AplicaComissaoDescontoAcrescimo(transaction, idPedido, orcamento.IdComissionado, orcamento.PercComissao, orcamento.TipoAcrescimo, orcamento.Acrescimo, orcamento.TipoDesconto,
                            orcamento.Desconto, Geral.ManterDescontoAdministrador);

                        foreach (var a in AmbientePedidoDAO.Instance.GetByPedido(transaction, idPedido))
                        {
                            AmbientePedidoDAO.Instance.RemoveAcrescimo(transaction, a.IdAmbientePedido);
                            AmbientePedidoDAO.Instance.RemoveDesconto(transaction, a.IdAmbientePedido);

                            if (a.Acrescimo > 0)
                                AmbientePedidoDAO.Instance.AplicaAcrescimo(transaction, a.IdAmbientePedido, a.TipoAcrescimo, a.Acrescimo);

                            if (a.Desconto > 0)
                                AmbientePedidoDAO.Instance.AplicaDesconto(transaction, a.IdAmbientePedido, a.TipoDesconto, a.Desconto);
                        }

                        objPersistence.ExecuteCommand(transaction, string.Format(@"UPDATE pedido SET TipoDesconto=?tipoDesconto, Desconto=?desconto, TipoAcrescimo=?tipoAcrescimo, Acrescimo=?acrescimo
                            WHERE IdPedido={0}", idPedido), new GDAParameter("?tipoDesconto", orcamento.TipoDesconto), new GDAParameter("?desconto", orcamento.Desconto),
                            new GDAParameter("?tipoAcrescimo", orcamento.TipoAcrescimo), new GDAParameter("?acrescimo", orcamento.Acrescimo));

                        UpdateTotalPedido(transaction, idPedido);

                        #endregion

                        /* Cancela o pedido se o total do mesmo não coincidir com o total do orçamento (Margem de erro de R$0,50)
                         * Teve que ser retirado para confirmação porque na vidrália aconteceu do pedido 162677 ter sido gerado PCP com um valor diferente
                         * Teve que ser retirado da tempera de Vespasiano porque lá pedido original tem dois valores, à vista e à prazo, porém na conferência
                         * só o à vista (taxa à prazo). */
                        var totalPedido = GetTotal(transaction, idPedido);
                        var totalOrcamento = OrcamentoDAO.Instance.GetTotal(transaction, idOrcamento);

                        if ((!OrcamentoConfig.NegociarParcialmente || !OrcamentoDAO.Instance.PossuiPedidoGerado(idOrcamento)) &&
                            PedidoConfig.LiberarPedido && (totalPedido > totalOrcamento + (decimal)0.5 || totalPedido < totalOrcamento - (decimal)0.5))
                            throw new Exception("O pedido não poderá ser gerado, houve alguma modificação nos valores dos produtos ou no cadastro do cliente, recalcule o orçamento e tente gerar o pedido novamente.");

                        transaction.Commit();
                        transaction.Close();

                        return idPedido;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        #region Salva um log de alteração no orçamento com a falha

                        /* Chamado 48759. */
                        var logOrcamento = new LogAlteracao();
                        logOrcamento.Campo = "Geração pedido";
                        logOrcamento.DataAlt = DateTime.Now;
                        logOrcamento.IdFuncAlt = login.CodUser;
                        logOrcamento.IdRegistroAlt = (int)idOrcamento;
                        logOrcamento.NumEvento = 1;
                        logOrcamento.Referencia = "Orçamento: " + idOrcamento;
                        logOrcamento.Tabela = (int)LogAlteracao.TabelaAlteracao.Orcamento;
                        logOrcamento.ValorAtual = ex.Message != null ? ex.Message :
                            ex.InnerException != null && !string.IsNullOrEmpty(ex.InnerException.Message) ? ex.InnerException.Message :
                            "Não foi possível recuperar o motivo da falha.";

                        LogAlteracaoDAO.Instance.Insert(logOrcamento);

                        #endregion

                        throw ex;
                    }
                }
            }
        }

        #endregion

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
        /// Define a visibilidade dos ícones do relatório de pedidos.
        /// </summary>
        /// <returns></returns>
        public static bool ExibirRelatorioPedido(uint idPedido)
        {
            if (!PedidoConfig.RelatorioPedido.SoImprimirPedidoConfirmado ||
                UserInfo.GetUserInfo.TipoUsuario == (uint)Utils.TipoFuncionario.Administrador)
                return true;

            return PedidoDAO.Instance.IsPedidoConfirmadoLiberado(idPedido);
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

        #region Buscar pedidos prontos e não entregues

        public bool ExistemPedidosProntosNaoEntreguesPeriodo(int qtdeDias)
        {
            string sql = "SELECT * FROM pedido WHERE SituacaoProducao = 3 AND NOW() > DATE_ADD(DATAPRONTO, INTERVAL " + qtdeDias + " DAY) ";

            return objPersistence.LoadData(sql).Count() > 0;
        }

        #endregion

        #region Verifica se todos os produtos do pedido podem ser liberados sem estarem prontos na produção

        /// <summary>
        /// Verifica se todos os produtos do pedido podem ser liberados sem estarem prontos na produção
        /// </summary>
        public bool ProdutosPodemLiberarProducaoPendente(GDASession session, uint idPedido)
        {
            var sql = @"
                SELECT COUNT(*)
                FROM produtos_pedido_espelho ppe
                    INNER JOIN produto prod ON (ppe.IdProd = prod.IdProd)
                    INNER JOIN subgrupo_prod sgp ON (prod.IdSubgrupoProd = sgp.IdSubGrupoProd)
                WHERE COALESCE(ppe.InvisivelFluxo, 0) = 0 
                    AND COALESCE(sgp.LiberarPendenteProducao, 0) = 0
                    AND ppe.IdPedido = " + idPedido;

            return objPersistence.ExecuteSqlQueryCount(session, sql) == 0;
        }

        #endregion

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

        #region API

        /// <summary>
    /// Busca os dados de vendas por pedidos para API
    /// </summary>
    /// <returns>1 - Tipo de Pedido, 2 - Data, 3 - Valor</returns>
        public List<Tuple<int, string, DateTime, decimal>> PesquisarVendasPedido()
        {
            var ini = DateTime.Now.AddDays(-15);
            var fim = DateTime.Now;

            //var ini = new DateTime(2015, 10, 6);
            //var fim = new DateTime(2015, 10, 19);

            var sql = @"
                SELECT CONCAT(TipoPedido, ';', Date(DataCad), ';', cast(SUM(Total) as char))
                FROM pedido
                WHERE
                    Situacao <> " + (int)Pedido.SituacaoPedido.Cancelado + @"
		                AND DATE(dataCad) >= ?dtIni
		                AND DATE(dataCad) <= ?dtFim
                        AND TipoPedido <> " + (int)Pedido.TipoPedidoEnum.Producao + @"
                        AND TipoVenda NOT IN (" + (int)Pedido.TipoVendaPedido.Garantia + @", " + (int)Pedido.TipoVendaPedido.Reposição + @")
                GROUP BY TipoPedido, DATE(DataCad)
                HAVING SUM(Total) > 0
                ORDER BY DataCad Desc";

            var dados = ExecuteMultipleScalar<string>(sql, new GDAParameter("dtIni", ini), new GDAParameter("dtFim", fim));

            var retorno = new List<Tuple<int, string, DateTime, decimal>>();

            foreach (var d in dados)
            {
                var str = d.Split(';');

                var tipoPedidoStr = Colosoft.Translator.Translate(((Pedido.TipoPedidoEnum)str[0].StrParaInt())).Format();

                retorno.Add(new Tuple<int, string, DateTime, decimal>(str[0].StrParaInt(), tipoPedidoStr, DateTime.Parse(str[1]), str[2].StrParaDecimal()));
            }

            return retorno;
        }

        /// <summary>
        /// Verifica se houve cadastro de pedido apos a data informada
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="dataInicial"></param>
        /// <returns></returns>
        public bool TeveCadPosterior(GDASession sessao, DateTime dataInicial)
        {
            var sql = @"
                SELECT COUNT(*)
                FROM pedido
                WHERE DataCad > ?dtIni";

            return objPersistence.ExecuteSqlQueryCount(sessao, sql, new GDAParameter("?dtIni", dataInicial)) > 0;
        }

        #endregion

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

        #region Valida Pedido para Liberação

        public string ValidaPedidoLiberacao(GDASession session, uint idPedido, int? tipoVenda, int? idFormaPagto, bool cxDiario, List<uint> idsPedido)
        {
            try
            {
                // Verifica se o pedido existe
                if (!PedidoExists(session, idPedido))
                    return "false|Não existe pedido com esse número.";

                var idCliente = ObtemIdCliente(session, idPedido);
                var tipoEntrega = ObtemTipoEntrega(session, idPedido);
                var isReposicaoGarantia = IsPedidoGarantia(session, idPedido.ToString()) || IsPedidoReposicao(session, idPedido.ToString());
                var existeEspelho = PedidoEspelhoDAO.Instance.ExisteEspelho(session, idPedido);
                var idLoja = ObtemIdLoja(session, idPedido);
                var naoIgnorar = !LojaDAO.Instance.GetIgnorarLiberarProdutosProntos(session, idLoja);
                var idObra = GetIdObra(session, idPedido);

                // Verifica se o cliente está ativo
                if (ClienteDAO.Instance.GetSituacao(session, idCliente) != (int)SituacaoCliente.Ativo)
                    return "false|O cliente desse pedido está inativo.";

                // Verifica se o pedido possui funcionário
                if (string.IsNullOrEmpty(ObtemNomeFuncResp(session, idPedido)))
                    return "false|Este pedido não possui nenhum funcionário associado ao mesmo.";

                #region Validações da situação do pedido

                // Verifica se o pedido já foi liberado
                if (IsPedidoLiberado(session, idPedido))
                {
                    var idLiberacao = LiberarPedidoDAO.Instance.GetIdLiberacao(session, idPedido.ToString());
                    return string.Format("false|Este pedido já foi liberado.{0}", idLiberacao > 0 ? string.Format(" Número da liberação: {0}.", idLiberacao.Value) : string.Empty);
                }
                
                if (!IsPedidoConfirmado(session, idPedido))
                {
                    var retorno = "false|Esse pedido ainda não foi confirmado.";

                    if (!isReposicaoGarantia && TemSinalReceber(session, idPedido))
                        retorno += " Pedido possui sinal a receber.";
                    else if (!isReposicaoGarantia && TemPagamentoAntecipadoReceber(session, idPedido))
                        retorno += " Pedido possui pagamento antecipado a receber.";

                    return retorno;
                }

                #endregion

                #region Validações do pedido espelho

                if (existeEspelho)
                {
                    if (PedidoEspelhoDAO.Instance.ObtemSituacao(session, idPedido) == PedidoEspelho.SituacaoPedido.Aberto)
                        return "false|A conferência deste pedido deve estar finalizada ou impressa para poder liberá-lo.";

                    if (!PCPConfig.ControlarProducao && PedidoEspelhoDAO.Instance.ObtemSituacao(session, idPedido) != PedidoEspelho.SituacaoPedido.Finalizado)
                        return "false|Este pedido deve estar finalizado no PCP para ser liberado.";

                    // Verifica se o desconto lançado no pedido é o mesmo no pedido original e no PCP
                    if (ObterDesconto(session, (int)idPedido) != PedidoEspelhoDAO.Instance.ObterDesconto(session, (int)idPedido))
                        return "false|O desconto lançado no pedido original está diferente do desconto lançado no pedido espelho, retire o mesmo e lançe-o novamente no ícone ($) na lista de pedidos.";

                    // Verifica se o desconto lançado no pedido é o mesmo no pedido original e no PCP
                    if (ObterAcrescimo(session, (int)idPedido) != PedidoEspelhoDAO.Instance.ObterAcrescimo(session, (int)idPedido))
                        return "false|O acréscimo lançado no pedido original está diferente do acréscimo lançado no pedido espelho, retire o mesmo e lançe-o novamente no ícone ($) na lista de pedidos.";
                }
                else
                {
                    if (PCPConfig.ControlarProducao && PCPConfig.ImpedirLiberacaoPedidoSemPCP && (PossuiVidrosProducao(session, idPedido) || IsMaoDeObra(session, idPedido)))
                        return "false|Este pedido deve passar no PCP antes de ser liberado.";
                }

                #endregion

                #region Validações do pagamento antecipado

                // Verifica se o pagto antecipado do pedido é válido
                if (ObtemIdPagamentoAntecipado(session, idPedido) > 0 && ObtemValorPagtoAntecipado(session, idPedido) == 0)
                    return "false|O pedido possui pagamento antecipado mas o valor recebido está zerado, será necessário receber o valor novamente.";

                #endregion

                #region Validações da produção do pedido

                if ((Liberacao.DadosLiberacao.LiberarProdutosProntos && naoIgnorar) && !Liberacao.DadosLiberacao.LiberarPedidoProdutos && PossuiVidrosProducao(session, idPedido) &&
                    !IsPedidoAtrasado(session, idPedido, true) && !ProdutosPodemLiberarProducaoPendente(session, idPedido))
                {
                    var situacaoProducao = ObtemSituacaoProducao(session, idPedido);

                    if ((situacaoProducao == (int)Pedido.SituacaoProducaoEnum.Pendente || situacaoProducao == (int)Pedido.SituacaoProducaoEnum.NaoEntregue) &&
                        (!Liberacao.DadosLiberacao.LiberarClienteRota || !RotaDAO.Instance.ClientePossuiRota(session, idCliente)))
                        return "false|Algumas peças deste pedido ainda não estão prontas.";
                }

                #endregion

                #region Validações da Ordem de Carga

                if (OrdemCargaConfig.UsarControleOrdemCarga && tipoEntrega == DataSources.Instance.GetTipoEntregaEntrega())
                {
                    if ((ObtemDeveTransferir(session, idPedido) && !PedidoOrdemCargaDAO.Instance.PedidoTemOC(session, OrdemCarga.TipoOCEnum.Venda, idPedido)) ||
                        !PedidoOrdemCargaDAO.Instance.PedidoTemOC(session, idPedido) && !GerarPedidoProducaoCorte(session, idPedido) &&
                        (PossuiVidrosProducao(session, idPedido) || PossuiVidrosEstoque(session, idPedido) || PossuiVolume(session, idPedido)))
                        return "false|Este pedido deve ter uma OC venda gerada para ser liberado.";
                }

                #endregion

                #region Validações das informações de forma de pagamento do pedido

                if (!isReposicaoGarantia)
                {
                    if (Liberacao.DadosLiberacao.BloquearLiberacaoDadosPedido && tipoVenda.GetValueOrDefault() > 0 && GetTipoVenda(session, idPedido) != tipoVenda)
                        return string.Format("false|Este pedido não foi vendido como '{0}'.", Pedido.GetDescrTipoVenda(tipoVenda));

                    /* Chamado 65135. */
                    if (FinanceiroConfig.UsarControleDescontoFormaPagamentoDadosProduto && idFormaPagto.GetValueOrDefault() > 0 && ObtemFormaPagto(session, idPedido) != idFormaPagto)
                        throw new Exception("Como o controle de desconto por forma de pagamento e dados do produto está habilitado, não é possível liberar pedidos com formas de pagamento diferentes.");
                }

                #region Parcelas

                // Se a empresa não libera pedidos com parcelas diferentes então as parcelas devem ser validadas.
                if (Liberacao.BloquearLiberacaoParcelasDiferentes && idsPedido != null && idsPedido.Count > 0)
                {
                    var lstIdsParcela = new List<uint>();

                    foreach (var id in idsPedido)
                    {
                        var idParcela = ObtemIdParcela(session, id).GetValueOrDefault();

                        if (lstIdsParcela.Count == 0)
                            lstIdsParcela.Add(idParcela);

                        if (lstIdsParcela.Count > 0 && !lstIdsParcela.Contains(idParcela))
                            return "false|Somente pedidos com a mesma parcela podem ser liberados.";
                    }
                }

                #endregion

                #endregion

                // Se for pedido de obra, recalcula o saldo da mesma, em alguns casos o saldo já havia debitado o valor do pedido antes de ser liberado.
                if (idObra > 0)
                    ObraDAO.Instance.AtualizaSaldo(session, null, idObra.Value, cxDiario, false);

                return string.Format("true|{0}", idCliente);
            }
            catch (Exception ex)
            {
                return MensagemAlerta.FormatErrorMsg("false|", ex);
            }
        }

        #endregion

        #region App

        /// <summary>
        /// Finaliza o projeto do aplicativo criando o pedido e finalizando-o em seguida
        /// </summary>
        /// <param name="projeto">Dados do projeto que será finalizado.</param>
        /// <param name="imagens">Imagens do projeto.</param>
        public void FinalizarProjetoGerarPedidoApp(IProjeto projeto, IEnumerable<IImagemProjeto> imagens)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var rota = RotaDAO.Instance.GetByCliente(transaction, UserInfo.GetUserInfo.IdCliente.GetValueOrDefault());

                    var pedido = new Pedido();
                    pedido.IdCli = UserInfo.GetUserInfo.IdCliente.GetValueOrDefault();
                    pedido.TipoEntrega = rota != null && rota.EntregaBalcao ? (int)Pedido.TipoEntregaPedido.Balcao : projeto.IdTipoEntrega;
                    pedido.IdFunc = ClienteDAO.Instance.ObtemIdFunc(transaction, pedido.IdCli).GetValueOrDefault(0);
                    pedido.Situacao = Pedido.SituacaoPedido.Ativo;
                    pedido.DataPedido = DateTime.Now;
                    pedido.AliquotaIcms = 0.12f;
                    pedido.CustoPedido = projeto.CustoTotal;
                    pedido.CodCliente = projeto.Pedido;
                    pedido.Obs = projeto.Obs;
                    pedido.TipoPedido =  (int)Pedido.TipoPedidoEnum.Venda;
                    pedido.TipoVenda = (int)Pedido.TipoVendaPedido.AVista;
                    pedido.IdLoja = UserInfo.GetUserInfo.IdLoja;
                    pedido.GeradoParceiro = true;
                    
                    #region Define as informações de pagamento do pedido

                    // Recupera a parcela padrão do cliente.
                    var tipoPagto = ClienteDAO.Instance.ObtemTipoPagto(transaction, pedido.IdCli);

                    if (tipoPagto > 0)
                    {
                        var parcelaPadrao = ParcelasDAO.Instance.GetElementByPrimaryKey(transaction, tipoPagto.Value);

                        // Caso a parcela padrão seja uma parcela à prazo, altera o tipo de venda do pedido para À Prazo.
                        if (parcelaPadrao != null && parcelaPadrao.NumParcelas > 0)
                            pedido.TipoVenda = (int)Pedido.TipoVendaPedido.APrazo;
                    }

                    // Recupera a forma de pagamento padrão do cliente.
                    var idFormaPagtoCliente = ClienteDAO.Instance.ObtemIdFormaPagto(transaction, pedido.IdCli);

                    if (idFormaPagtoCliente > 0)
                    {
                        // Recupera as formas de pagamento disponíveis, para o cliente, de acordo com o tipo de venda do pedido.
                        var formasPagamento = FormaPagtoDAO.Instance.GetForPedido(transaction, (int)pedido.IdCli, pedido.TipoVenda.GetValueOrDefault());

                        // Caso a forma de pagamento, padrão do cliente, esteja presente nas opções de forma de pagamento do pedido, seleciona ela por padrão.
                        if (formasPagamento != null && formasPagamento.Count > 0 && formasPagamento.Select(f => f.IdFormaPagto).Contains(idFormaPagtoCliente))
                            pedido.IdFormaPagto = idFormaPagtoCliente;
                    }

                    #endregion

                    var idPedido = Insert(transaction, pedido);
                    if (idPedido == 0)
                        throw new Exception("Inserção do pedido retornou 0.");

                    var idObra = PedidoConfig.DadosPedido.UsarControleNovoObra ? PedidoDAO.Instance.GetIdObra(idPedido) : null;

                    var retornoValidacao = string.Empty;
                    
                    foreach (var ip in projeto.Itens)
                    {
                        var projetoModelo = ProjetoModeloDAO.Instance.GetElementByPrimaryKey(transaction, ip.IdProjetoModelo);

                        #region Cria projetos 

                        var itemProjeto = ItemProjetoDAO.Instance.NovoItemProjetoVazio(transaction, null, 
                            null, null, idPedido, null, null, null, ip.IdProjetoModelo, ip.EspessuraVidro,
                            ip.IdCorVidro, 0, 0, true, ip.MedidaExata, true);

                        //// Salva as medidas da área de instalação, com a referência do item de projeto.
                        //var medidasItemProjeto = SalvarMedidasAreaInstalacao;

                        var medidasItemProjeto = SalvarMedidasAreaInstalacaoApp(transaction, projetoModelo, itemProjeto, ip.Medidas);
                        itemProjeto.Qtde = medidasItemProjeto.Where(f => f.IdMedidaProjeto == 1).FirstOrDefault().Valor;

                        // Busca as peças deste item, que serão utilizadas nas expressões
                        var pecasItemProjeto = PecaItemProjetoDAO.Instance.GetByItemProjeto(transaction, itemProjeto.IdItemProjeto, itemProjeto.IdProjetoModelo);

                        var pecasProjetoModelo = ObterPecasProjetoModeloApi(transaction, ip, pecasItemProjeto);
                       
                        // Calcula as medidas das peças do projeto.
                        var lstPecaModelo = UtilsProjeto.CalcularMedidasPecas(transaction, itemProjeto, projetoModelo, pecasProjetoModelo, medidasItemProjeto, true, false,
                            out retornoValidacao);

                        // Insere Peças na tabela peca_item_projeto
                        PecaItemProjetoDAO.Instance.InsertFromPecaModelo(transaction, itemProjeto, ref lstPecaModelo);

                        // Insere as peças de vidro apenas se todas as Peça Projeto Modelo tiver idProd
                        var inserirPecasVidro = !lstPecaModelo.Any(f => f.IdProd == 0);
                        if (inserirPecasVidro)
                            // Insere Peças na tabela material_item_projeto
                            MaterialItemProjetoDAO.Instance.InserePecasVidro(transaction, idObra, UserInfo.GetUserInfo.IdCliente, 
                                projeto.IdTipoEntrega, itemProjeto, projetoModelo, lstPecaModelo);


                        // Atualiza qtds dos materiais apenas
                        MaterialItemProjetoDAO.Instance.AtualizaQtd(transaction, idObra, UserInfo.GetUserInfo.IdCliente, 
                            projeto.IdTipoEntrega, itemProjeto, projetoModelo);

                        ItemProjetoDAO.Instance.UpdateTotalItemProjeto(transaction, itemProjeto.IdItemProjeto);

                        uint? idProjeto = ItemProjetoDAO.Instance.GetIdProjeto(transaction, itemProjeto.IdItemProjeto);
                        uint? idOrcamento = ItemProjetoDAO.Instance.GetIdOrcamento(transaction, itemProjeto.IdItemProjeto);

                        if (idProjeto > 0)
                            ProjetoDAO.Instance.UpdateTotalProjeto(transaction, idProjeto.Value);
                        else if (idOrcamento > 0)
                        {
                            uint idProd = ProdutosOrcamentoDAO.Instance.ObtemIdProdutoPorIdItemProjeto(transaction, itemProjeto.IdItemProjeto);
                            if (idProd > 0)
                                ProdutosOrcamentoDAO.Instance.UpdateTotaisProdutoOrcamento(transaction, idProd);

                            OrcamentoDAO.Instance.UpdateTotaisOrcamento(transaction, idOrcamento.Value);
                        }

                        #endregion

                        #region Finaliza projeto

                        itemProjeto = ItemProjetoDAO.Instance.GetElementByPrimaryKey(itemProjeto.IdItemProjeto);

                        uint idAmbienteNovo;

                        retornoValidacao = string.Empty;

                        /* Chamado 48676. */
                        if (itemProjeto == null)
                            throw new Exception("Não foi possível recuperar o projeto. Atualize a tela e confirme o projeto novamente.");


                        // Atualiza o campo ambiente no itemProjeto
                        ItemProjetoDAO.Instance.AtualizaAmbiente(transaction, itemProjeto.IdItemProjeto, ip.Ambiente);
                        itemProjeto.Ambiente = ip.Ambiente;

                        var lstPecas = PecaItemProjetoDAO.Instance.GetByItemProjeto(transaction, itemProjeto.IdItemProjeto, itemProjeto.IdProjetoModelo);

                        var idGrupoModelo = ProjetoModeloDAO.Instance.ObtemGrupoModelo(transaction, itemProjeto.IdProjetoModelo);
                        var codigoGrupoModelo = GrupoModeloDAO.Instance.ObtemDescricao(transaction, idGrupoModelo);

                        if (lstPecas.Count == 0 &&
                            ProjetoModeloDAO.Instance.ObtemCodigo(transaction, itemProjeto.IdProjetoModelo) != "OTR01" &&
                            /* Chamado 22588. */
                            !codigoGrupoModelo.ToLower().Contains("esquadria"))
                            throw new Exception("Informe as peças de vidro.");

                        // Verifica se as peças do item projeto estão de acordo com os materiais do mesmo. Chamado 9673.
                        foreach (var peca in lstPecas)
                        {
                            /* Chamado 63058. */
                            if (peca.Qtde == 0 || peca.IdProd == 0)
                                continue;

                            var material = MaterialItemProjetoDAO.Instance.GetMaterialByPeca(transaction, peca.IdPecaItemProj);

                            // Se a Peça Item Projeto não tiver IdProd, é porque não foi calculado os vidros.
                            if (peca.IdProd > 0 && (material == null || material.Qtde != peca.Qtde))
                            {
                                var ex = new Exception("Calcule as medidas novamente. Os materias do projeto não conferem com as peças do mesmo.");
                                ErroDAO.Instance.InserirFromException("CadProjetoAvulso.aspx", ex);
                                throw ex;
                            }
                            else if (peca.Altura == 0 || peca.Largura == 0)
                                throw new Exception(
                                    string.Format("A {0} da peça {1} está zerada. Informe o valor desta medida e confirme o projeto novamente.",
                                        peca.Altura == 0 ? "Altura" : "Largura", peca.CodInterno));

                            /* Chamado 24308. */
                            objPersistence.ExecuteCommand(transaction, string.Format("UPDATE peca_item_projeto SET ImagemEditada=0 WHERE IdPecaItemProj={0};", peca.IdPecaItemProj));
                        }    

                        if (idPedido > 0)
                        {
                            /* Chamado 52637.
                             * Remove e aplica acréscimo/desconto/comissão no pedido somente uma vez.
                             * Antes essa atualização estava demorando muito porque era feita para cada ambiente. */
                            #region Remove acréscimo/desconto/comissão do pedido

                            var idsAmbientePedido = new List<uint>();
                            var idComissionado = new uint?();
                            var percComissao = new float();
                            var tipoAcrescimo = new int();
                            var tipoDesconto = new int();
                            var acrescimo = new decimal();
                            var desconto = new decimal();

                            PedidoDAO.Instance.ObtemDadosComissaoDescontoAcrescimo(transaction, idPedido, out tipoDesconto,
                                out desconto, out tipoAcrescimo, out acrescimo, out percComissao, out idComissionado);

                            // Remove acréscimo, desconto e comissão.
                            objPersistence.ExecuteCommand(transaction, "UPDATE PEDIDO SET IdComissionado=NULL WHERE IdPedido=" + idPedido);
                            PedidoDAO.Instance.RemoveComissaoDescontoAcrescimo(transaction, idPedido);

                            #endregion

                            var ambiente = AmbientePedidoDAO.Instance.GetIdByItemProjeto(itemProjeto.IdItemProjeto);

                            idAmbienteNovo = ProdutosPedidoDAO.Instance.InsereAtualizaProdProj(transaction, idPedido, ambiente, itemProjeto, false, false, true);

                            #region Aplica acréscimo/desconto/comissão do pedido

                            // Aplica acréscimo, desconto e comissão.
                            PedidoDAO.Instance.AplicaComissaoDescontoAcrescimo(transaction, idPedido, idComissionado, percComissao,
                                tipoAcrescimo, acrescimo, tipoDesconto, desconto, Geral.ManterDescontoAdministrador);

                            // Aplica acréscimo e desconto no ambiente.
                            if (OrcamentoConfig.Desconto.DescontoAcrescimoItensOrcamento && idsAmbientePedido.Count > 0)
                                foreach (var idAmbPed in idsAmbientePedido)
                                {
                                    var acrescimoAmbiente = AmbientePedidoDAO.Instance.ObterAcrescimo(transaction, idAmbPed);
                                    var descontoAmbiente = AmbientePedidoDAO.Instance.ObterAcrescimo(transaction, idAmbPed);

                                    if (acrescimoAmbiente > 0)
                                        AmbientePedidoDAO.Instance.AplicaAcrescimo(transaction, idAmbPed, AmbientePedidoDAO.Instance.ObterTipoAcrescimo(transaction, idAmbPed), acrescimoAmbiente);

                                    if (descontoAmbiente > 0)
                                        AmbientePedidoDAO.Instance.AplicaDesconto(transaction, idAmbPed, AmbientePedidoDAO.Instance.ObterTipoDesconto(transaction, idAmbPed), descontoAmbiente);
                                }

                            // Atualiza o total do pedido.
                            PedidoDAO.Instance.UpdateTotalPedido(transaction, idPedido);

                            #endregion
                        }


                        // Marca que cálculo de projeto foi conferido
                        if (idPedido > 0)
                        {
                            // Verifica se todas as medidas de instalação foram inseridas
                            if (!itemProjeto.MedidaExata && itemProjeto.IdCorVidro > 0 && MedidaProjetoModeloDAO.Instance.GetByProjetoModelo(transaction, itemProjeto.IdProjetoModelo, false).Count >
                                MedidaItemProjetoDAO.Instance.GetListByItemProjeto(transaction, itemProjeto.IdItemProjeto).Count && ProjetoModeloDAO.Instance.ObtemCodigo(transaction, itemProjeto.IdProjetoModelo) != "OTR01")
                                throw new Exception("Falha ao inserir medidas, confirme o projeto novamente.");

                            ItemProjetoDAO.Instance.CalculoConferido(transaction, itemProjeto.IdItemProjeto);
                        }

                        #endregion
                    }

                    UpdateTotalPedido(transaction, idPedido);

                    pedido = GetElementByPrimaryKey(transaction, idPedido);

                    if (Math.Round(projeto.Total, 2) != Math.Round(pedido.Total, 2))
                        throw new Exception(
                            string.Format(Globalizacao.Cultura.CulturaSistema, "Erro ao gerar pedido. Valor do pedido difere do valor do projeto. (Pedido: {0:C}, Projeto: {1:C}",
                                pedido.Total, projeto.Total));

                    var emConferencia = false;

                    if (!imagens.Any())
                    {
                        FinalizarPedido(transaction, idPedido, ref emConferencia, false);

                        //PedidoEspelhoDAO.Instance.GeraEspelho(transaction, idPedido);
                        //PedidoEspelhoDAO.Instance.FinalizarPedido(transaction, idPedido);
                    }

                    foreach (var imagem in imagens)
                    {
                        using (var stream = imagem.Abrir())
                        {
                            var content = new byte[stream.Length];
                            stream.Read(content, 0, content.Length);

                            Anexo.InserirAnexo(IFoto.TipoFoto.Pedido, idPedido, content, imagem.Nome, imagem.Descricao);
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


        public IEnumerable<IMedidaItemProjeto> SalvarMedidasAreaInstalacaoApp(GDASession session, ProjetoModelo projetoModelo,
            ItemProjeto itemProjeto, IEnumerable<IMedidaItemProjeto> medidasItemProjeto)
        {
            // Insere a quantidade
            var medidaQtd = new MedidaItemProjeto();
            medidaQtd.IdItemProjeto = itemProjeto.IdItemProjeto;
            medidaQtd.IdMedidaProjeto = 1;
            medidaQtd.Valor = medidasItemProjeto.Where(f => f.IdMedidaProjeto == 1).FirstOrDefault().Valor;
            MedidaItemProjetoDAO.Instance.Insert(session, medidaQtd);

            #region Salva as medidas da área de instalação

            foreach (MedidaProjetoModelo mpm in MedidaProjetoModeloDAO.Instance.GetByProjetoModelo(session, itemProjeto.IdProjetoModelo, false))
            {
                var med = medidasItemProjeto.FirstOrDefault(f => f.IdMedidaProjeto == (uint)mpm.IdMedidaProjeto);
                var alturaBox = itemProjeto.EspessuraVidro == 6 ? ProjetoConfig.AlturaPadraoProjetoBox6mm : ProjetoConfig.AlturaPadraoProjetoBoxAcima6mm;

                // Não insere a medida QTD, pois já foi inserida no código acima
                if (mpm.IdMedidaProjeto != 1)
                    MedidaItemProjetoDAO.Instance.InsereMedida(session, itemProjeto.IdItemProjeto, mpm.IdMedidaProjeto, med.Valor);
            }
   
            #endregion

            return MedidaItemProjetoDAO.Instance.GetListByItemProjeto(itemProjeto.IdItemProjeto);
        }

        /// <summary>
        /// Retorna Lista de PecasModeloProjeto a partir da tbPecasModelo
        /// </summary>
        public static List<PecaProjetoModelo> ObterPecasProjetoModeloApi(GDASession session, IItemProjeto itemProjeto, IEnumerable<PecaItemProjeto> pecasItemProjeto)
        {
            // Busca as peças deste item, que serão utilizadas nas expressões

            var pecasProjetoModelo = new List<PecaProjetoModelo>();

            foreach (var pecaApp in itemProjeto.Pecas)
            {
                var peca = pecasItemProjeto.First(f => f.IdPecaProjMod == pecaApp.IdPecaProjMod);
                var pecaModelo = PecaProjetoModeloDAO.Instance.GetElementByPrimaryKey(session, pecaApp.IdPecaProjMod);
               
                pecasProjetoModelo.Add(new PecaProjetoModelo
                {
                    IdPecaProjMod = pecaApp.IdPecaProjMod,
                    IdPecaItemProj = peca.IdPecaItemProj,
                    IdProd = pecaApp.IdProd.GetValueOrDefault(),
                    Qtde = pecaApp.Qtde,
                    Obs = pecaApp.Obs,
                    Largura = pecaApp.Largura,
                    Altura = pecaApp.Altura,
                    CalculoAltura = pecaModelo.CalculoAltura,
                    CalculoLargura = pecaModelo.CalculoLargura,
                    CalculoQtde = pecaModelo.CalculoQtde,
                    Item = pecaApp.Item,
                    Tipo = pecaApp.Tipo
                });
            }

            return pecasProjetoModelo;
        }

        #endregion
    }
}