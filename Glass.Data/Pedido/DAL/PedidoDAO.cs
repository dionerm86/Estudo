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

    public sealed partial class PedidoDAO : BaseCadastroDAO<Pedido, PedidoDAO>
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
            string situacaoProd, string byVend, string vendas, string maoObra, string maoObraEspecial, string producao,
            string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim, string funcFinalizacao, uint idOrcamento,
            bool opcionais, bool infoPedidos, float altura, int largura, int numeroDiasDiferencaProntoLib, float valorDe, float valorAte,
            string tipo, int fastDelivery, int tipoVenda, int origemPedido, string obs, bool selecionar, out string filtroAdicional, out bool temFiltro, string obsLiberacao = "")
        {
            return Sql(idPedido, idLiberarPedido, idsPedido, idsLiberarPedidos, idLoja, idCli, nomeCli, idFunc, codCliente, idCidade,
                endereco, bairro, null, situacao, situacaoProd, byVend, vendas, maoObra, maoObraEspecial, producao, dataCadIni,
                dataCadFim, dataFinIni, dataFinFim, funcFinalizacao, idOrcamento, opcionais, infoPedidos, altura, largura,
                numeroDiasDiferencaProntoLib, valorDe, valorAte, tipo, fastDelivery, tipoVenda, origemPedido, obs, false, selecionar,
                out filtroAdicional, out temFiltro, obsLiberacao);
        }

        private string Sql(uint idPedido, uint idLiberarPedido, string idsPedido, string idsLiberarPedidos, uint idLoja, uint idCli,
            string nomeCli, uint idFunc, string codCliente, uint idCidade, string endereco, string bairro, string complemento,
            string situacao, string situacaoProd, string byVend, string vendas, string maoObra, string maoObraEspecial,
            string producao, string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim, string funcFinalizacao,
            uint idOrcamento, bool opcionais, bool infoPedidos, float altura, int largura, int numeroDiasDiferencaProntoLib,
            float valorDe, float valorAte, string tipo, int fastDelivery, int tipoVenda, int origemPedido, string obs, bool buscarPedidoProducao,
            bool selecionar, out string filtroAdicional, out bool temFiltro, string obsLiberacao = "")
        {
            return Sql(null, idPedido, idLiberarPedido, idsPedido, idsLiberarPedidos, idLoja, idCli, nomeCli, idFunc, codCliente, idCidade,
                endereco, bairro, complemento, situacao, situacaoProd, byVend, vendas, maoObra, maoObraEspecial, producao,
                dataCadIni, dataCadFim, dataFinIni, dataFinFim, funcFinalizacao, idOrcamento, opcionais, infoPedidos, altura, largura,
                numeroDiasDiferencaProntoLib, valorDe, valorAte, tipo, fastDelivery, tipoVenda, origemPedido, obs, buscarPedidoProducao,
                selecionar, out filtroAdicional, out temFiltro, obsLiberacao);
        }

        private string Sql(GDASession sessao, uint idPedido, uint idLiberarPedido, string idsPedido, string idsLiberarPedidos, uint idLoja,
            uint idCli, string nomeCli, uint idFunc, string codCliente, uint idCidade, string endereco, string bairro, string situacao,
            string situacaoProd, string byVend, string vendas, string maoObra, string maoObraEspecial, string producao,
            string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim, string funcFinalizacao, uint idOrcamento,
            bool opcionais, bool infoPedidos, float altura, int largura, int numeroDiasDiferencaProntoLib, float valorDe, float valorAte,
            string tipo, int fastDelivery, int tipoVenda, int origemPedido, string obs, bool selecionar, out string filtroAdicional, out bool temFiltro)
        {
            return Sql(sessao, idPedido, idLiberarPedido, idsPedido, idsLiberarPedidos, idLoja, idCli, nomeCli, idFunc, codCliente,
                idCidade, endereco, bairro, null, situacao, situacaoProd, byVend, vendas, maoObra, maoObraEspecial, producao, dataCadIni,
                dataCadFim, dataFinIni, dataFinFim, funcFinalizacao, idOrcamento, opcionais, infoPedidos, altura, largura,
                numeroDiasDiferencaProntoLib, valorDe, valorAte, tipo, fastDelivery, tipoVenda, origemPedido, obs, false, selecionar,
                out filtroAdicional, out temFiltro);
        }

        private string Sql(GDASession sessao, uint idPedido, uint idLiberarPedido, string idsPedido, string idsLiberarPedidos, uint idLoja,
            uint idCli, string nomeCli, uint idFunc, string codCliente, uint idCidade, string endereco, string bairro, string complemento,
            string situacao, string situacaoProd, string byVend, string vendas, string maoObra, string maoObraEspecial,
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
                idCidade, endereco, bairro, complemento, situacao, situacaoProd, byVend, vendas, maoObra, maoObraEspecial,
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
            string complemento, string situacao, string situacaoProd, string byVend, string vendas, string maoObra,
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
                    var ids = string.Join(",", ExecuteMultipleScalar<string>(string.Format(@"SELECT IdPedidoRevenda FROM Pedido WHERE IdPedido={0} AND IdPedidoRevenda>0 
                                           UNION ALL SELECT IdPedido FROM Pedido WHERE IdPedidoRevenda={0}", idPedido)));
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
                    string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, null, null, null, null, null, 0, true,
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
            string maoObra, string maoObraEspecial, string producao, uint idOrcamento, float altura, int largura,
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
                situacao, situacaoProd, byVend, null, maoObra, maoObraEspecial, producao, dataCadIni, dataCadFim, dataFinIni,
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
            string bairro, string situacao, string situacaoProd, string byVend, string maoObra, string maoObraEspecial,
            string producao, uint idOrcamento, float altura, int largura, int numeroDiasDiferencaProntoLib, float valorDe, float valorAte,
            string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim, string funcFinalizacao, string tipo,
            int tipoVenda, int fastDelivery, string sortExpression, int startRow, int pageSize)
        {
            return GetList(idPedido, idLoja, idCli, nomeCli, codCliente, idCidade, endereco, bairro, null, situacao, situacaoProd, byVend,
                maoObra, maoObraEspecial, producao, idOrcamento, altura, largura, numeroDiasDiferencaProntoLib, valorDe, valorAte,
                dataCadIni, dataCadFim, dataFinIni, dataFinFim, funcFinalizacao, tipo, fastDelivery, tipoVenda, 0, "", sortExpression,
                startRow, pageSize);
        }

        public Pedido[] GetList(uint idPedido, uint idLoja, uint idCli, string nomeCli, string codCliente, uint idCidade, string endereco,
            string bairro, string situacao, string situacaoProd, string byVend, string maoObra, string maoObraEspecial,
            string producao, uint idOrcamento, float altura, int largura, int numeroDiasDiferencaProntoLib, float valorDe, float valorAte,
            string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim, string funcFinalizacao, string tipo, string obsLiberacao,
            int fastDelivery, int tipoVenda, string obs, string sortExpression, int startRow, int pageSize)
        {
            return GetList(idPedido, idLoja, idCli, nomeCli, codCliente, idCidade, endereco, bairro, null, situacao, situacaoProd, byVend,
                maoObra, maoObraEspecial, producao, idOrcamento, altura, largura, numeroDiasDiferencaProntoLib, valorDe, valorAte,
                dataCadIni, dataCadFim, dataFinIni, dataFinFim, funcFinalizacao, tipo, fastDelivery, tipoVenda, 0, obs, sortExpression,
                startRow, pageSize, obsLiberacao);
        }

        public Pedido[] GetList(uint idPedido, uint idLoja, uint idCli, string nomeCli, string codCliente, uint idCidade, string endereco,
            string bairro, string complemento, string situacao, string situacaoProd, string byVend, string maoObra,
            string maoObraEspecial, string producao, uint idOrcamento, float altura, int largura, int numeroDiasDiferencaProntoLib,
            float valorDe, float valorAte, string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim,
            string funcFinalizacao, string tipo, int fastDelivery, int tipoVenda, int origemPedido, string obs, string sortExpression, int startRow,
            int pageSize, string obsLiberacao = "")
        {
            var filtro = string.IsNullOrEmpty(sortExpression) ? "p.IdPedido Desc" : string.Format("{0}, IdPedido DESC", sortExpression);

            bool temFiltro;
            string filtroAdicional;

            var sql = Sql(idPedido, 0, null, null, idLoja, idCli, nomeCli, 0, codCliente, idCidade, endereco, bairro, complemento, situacao,
                situacaoProd, byVend, null, maoObra, maoObraEspecial, producao, dataCadIni, dataCadFim, dataFinIni, dataFinFim,
                funcFinalizacao, idOrcamento, false, false, altura, largura, numeroDiasDiferencaProntoLib, valorDe, valorAte, tipo,
                fastDelivery, tipoVenda, origemPedido, obs, true, true, out filtroAdicional, out temFiltro, obsLiberacao)
                .Replace("?filtroAdicional?", temFiltro ? filtroAdicional : "");

            var ped = LoadDataWithSortExpression(sql, filtro, startRow, pageSize, temFiltro, filtroAdicional, GetParam(nomeCli, codCliente,
                endereco, bairro, complemento, situacao, situacaoProd, dataCadIni, dataCadFim, dataFinIni, dataFinFim, obs)).ToArray();

            AtualizaTemAlteracaoPcp(ref ped);
            return ped;
        }


        public int GetCount(uint idPedido, uint idLoja, uint idCli, string nomeCli, string codCliente, uint idCidade, string endereco,
            string bairro, string situacao, string situacaoProd, string byVend, string maoObra, string maoObraEspecial,
            string producao, uint idOrcamento, float altura, int largura, int numeroDiasDiferencaProntoLib, float valorDe, float valorAte,
            string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim, string funcFinalizacao, string tipo,
            int tipoVenda, int fastDelivery)
        {
            return GetCount(idPedido, idLoja, idCli, nomeCli, codCliente, idCidade, endereco, bairro, null, situacao, situacaoProd, byVend,
                maoObra, maoObraEspecial, producao, idOrcamento, altura, largura, numeroDiasDiferencaProntoLib, valorDe, valorAte,
                dataCadIni, dataCadFim, dataFinIni, dataFinFim, funcFinalizacao, tipo, fastDelivery, tipoVenda, 0, "");
        }


        public int GetCount(uint idPedido, uint idLoja, uint idCli, string nomeCli, string codCliente, uint idCidade, string endereco,
            string bairro, string situacao, string situacaoProd, string byVend, string maoObra, string maoObraEspecial,
            string producao, uint idOrcamento, float altura, int largura, int numeroDiasDiferencaProntoLib, float valorDe, float valorAte,
            string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim, string funcFinalizacao, string tipo,
            int fastDelivery, int tipoVenda, string obs, string obsLiberacao)
        {
            return GetCount(idPedido, idLoja, idCli, nomeCli, codCliente, idCidade, endereco, bairro, null, situacao, situacaoProd, byVend,
                maoObra, maoObraEspecial, producao, idOrcamento, altura, largura, numeroDiasDiferencaProntoLib, valorDe, valorAte,
                dataCadIni, dataCadFim, dataFinIni, dataFinFim, funcFinalizacao, tipo, fastDelivery, tipoVenda, 0, obs, obsLiberacao);
        }

        public int GetCount(uint idPedido, uint idLoja, uint idCli, string nomeCli, string codCliente, uint idCidade, string endereco,
            string bairro, string complemento, string situacao, string situacaoProd, string byVend, string maoObra,
            string maoObraEspecial, string producao, uint idOrcamento, float altura, int largura, int numeroDiasDiferencaProntoLib,
            float valorDe, float valorAte, string dataCadIni, string dataCadFim, string dataFinIni, string dataFinFim,
            string funcFinalizacao, string tipo, int fastDelivery, int tipoVenda, int origemPedido, string obs, string obsLiberacao = "")
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = Sql(idPedido, 0, null, null, idLoja, idCli, nomeCli, 0, codCliente, idCidade, endereco, bairro, complemento, situacao,
                situacaoProd, byVend, null, maoObra, maoObraEspecial, producao, dataCadIni, dataCadFim, dataFinIni, dataFinFim,
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
            string endereco, string bairro, string complemento, string byVend, string maoObra, string maoObraEspecial,
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
                null, byVend, null, maoObra, maoObraEspecial, producao, dataCadIni, dataCadFim, null, null, null, 0, false, false,
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

                if (string.IsNullOrEmpty(situacao) || vetSituacao.Contains(((int)Pedido.SituacaoPedido.Conferido).ToString()))
                {
                    //Busca apenas os pedidos que foram finalizados, pois ao finalizar o pedido o mesmo é alterado para conferido
                    //somente quando for filtrado a situação conferido/conferido Com
                    where += " Or (";

                    if (!string.IsNullOrEmpty(dataIni))
                    {
                        where += string.Format(" And {0}.DataFin>={1}", aliasPedido, nomeParamDataIni);
                    }

                    if (!string.IsNullOrEmpty(dataFim))
                    {
                        where += string.Format(" And {0}.DataFin<={1}", aliasPedido, nomeParamDataFim);
                    }

                    where += ")";
                }

                if (string.IsNullOrEmpty(situacao) ||
                    vetSituacao.Contains(((int)Pedido.SituacaoPedido.Ativo).ToString()) ||
                    vetSituacao.Contains(((int)Pedido.SituacaoPedido.AtivoConferencia).ToString()) ||
                    vetSituacao.Contains(((int)Pedido.SituacaoPedido.EmConferencia).ToString()))
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

            if (idOrcamento > 0)
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

        private string SqlVendasPedidos(float? altura, int? cidade, string codCliente, string codigoProduto, string comSemNF, bool countLimit, string dataFimEntrega,
            string dataFimInstalacao, string dataFimPedido, string dataFimPronto, string dataFimSituacao, string dataInicioEntrega, string dataInicioInstalacao, string dataInicioPedido,
            string dataInicioPronto, string dataInicioSituacao, int? desconto, string descricaoProduto, bool exibirProdutos, bool exibirRota, int? fastDelivery, out string filtroAdicional, int? idCarregamento,
            string idCliente, int? idFunc, int? idMedidor, int? idOC, int? idOrcamento, int? idPedido, string idsBenef, string idsGrupo, string idsPedidos, string idsRota, string idsSubgrupoProd,
            int? idVendAssoc, int? largura, LoginUsuario login, string loja, string nomeCliente, int? numeroDiasDiferencaProntoLib, string observacao, int? ordenacao, int? origemPedido,
            bool paraRelatorio, bool pedidosSemAnexos, string situacao, string situacaoProducao, out bool temFiltro, string tiposPedido, string tipoCliente, int? tipoEntrega,
            int? tipoFiscal, string tiposVenda, bool totaisListaPedidos, bool trazerPedCliVinculado, int? usuarioCadastro)
        {
            return SqlVendasPedidos(altura, cidade, codCliente, codigoProduto, comSemNF, countLimit, dataFimEntrega,
                dataFimInstalacao, dataFimPedido, dataFimPronto, dataFimSituacao, dataInicioEntrega, dataInicioInstalacao, dataInicioPedido,
                 dataInicioPronto, dataInicioSituacao, desconto, descricaoProduto, exibirProdutos, exibirRota, fastDelivery, out filtroAdicional, idCarregamento,
                 idCliente, idFunc, idMedidor, idOC, idOrcamento, idPedido, idsBenef, idsGrupo, idsPedidos, idsRota, idsSubgrupoProd,
                 idVendAssoc, largura, login, loja, nomeCliente, numeroDiasDiferencaProntoLib, observacao, ordenacao, origemPedido,
                 paraRelatorio, pedidosSemAnexos, situacao, situacaoProducao, out temFiltro, tiposPedido, tipoCliente, tipoEntrega,
                 tipoFiscal, tiposVenda, totaisListaPedidos, trazerPedCliVinculado, usuarioCadastro, null, null, null);
        }

        /// <summary>
        /// Retorna o SQL da tela de vendas de pedidos, aplicando os filtros de acordo com os parâmetros informados.
        /// </summary>
        private string SqlVendasPedidos(float? altura, int? cidade, string codCliente, string codigoProduto, string comSemNF, bool countLimit, string dataFimEntrega,
            string dataFimInstalacao, string dataFimPedido, string dataFimPronto, string dataFimSituacao, string dataInicioEntrega, string dataInicioInstalacao, string dataInicioPedido,
            string dataInicioPronto, string dataInicioSituacao, int? desconto, string descricaoProduto, bool exibirProdutos, bool exibirRota, int? fastDelivery, out string filtroAdicional, int? idCarregamento,
            string idCliente, int? idFunc, int? idMedidor, int? idOC, int? idOrcamento, int? idPedido, string idsBenef, string idsGrupo, string idsPedidos, string idsRota, string idsSubgrupoProd,
            int? idVendAssoc, int? largura, LoginUsuario login, string loja, string nomeCliente, int? numeroDiasDiferencaProntoLib, string observacao, int? ordenacao, int? origemPedido,
            bool paraRelatorio, bool pedidosSemAnexos, string situacao, string situacaoProducao, out bool temFiltro, string tiposPedido, string tipoCliente, int? tipoEntrega,
            int? tipoFiscal, string tiposVenda, bool totaisListaPedidos, bool trazerPedCliVinculado, int? usuarioCadastro, string bairro, string dataInicioMedicao, string dataFimMedicao)
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

            if (!observacao.IsNullOrEmpty())
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

            if (desconto == 1)
            {
                filtroAdicional += " AND (p.Desconto >1 OR p.IdPedido IN (SELECT DISTINCT IdPedido FROM ambiente_pedido WHERE Desconto >1))";
                whereDadosVendidos += " AND (ped.Desconto OR ped.IdPedido IN (SELECT DISTINCT IdPedido FROM ambiente_pedido WHERE Desconto > 1))";

                criterio += string.Format(formatoCriterio, desconto == 1 ? "Pedidos com desconto" : "Pedidos sem desconto", string.Empty);
            }

            if (desconto == 2)
            {
                filtroAdicional += " AND (p.Desconto = 0 AND p.IdPedido NOT IN (SELECT DISTINCT IdPedido FROM ambiente_pedido WHERE Desconto >0)" +
                                   " AND(p.IdPedido in (SELECT DISTINCT IdPedido FROM produtos_pedido WHERE VALORDESCONTOQTDE = 0)))";
                whereDadosVendidos += " AND (ped.Desconto = 0 AND ped.IdPedido NOT IN (SELECT DISTINCT IdPedido FROM ambiente_pedido WHERE Desconto >0)" +
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


            #region Data de medição do pedido

            if (!string.IsNullOrWhiteSpace(dataInicioMedicao) || !string.IsNullOrWhiteSpace(dataFimMedicao))
            {
                filtroAdicional += string.Format(" AND p.IdPedido IN (SELECT DISTINCT IdPedido FROM medicao WHERE {0} {1} {2})",
                    !string.IsNullOrWhiteSpace(dataInicioMedicao) ? "DataMedicao>=?dataInicioMedicao" : string.Empty,
                    !string.IsNullOrWhiteSpace(dataInicioMedicao) && !string.IsNullOrWhiteSpace(dataFimMedicao) ? "AND" : string.Empty,
                    !string.IsNullOrWhiteSpace(dataFimMedicao) ? "DataMedicao<=?dataFimMedicao" : string.Empty);

                criterio += string.Format("{0}{1}",
                    !string.IsNullOrWhiteSpace(dataInicioMedicao) ? string.Format(formatoCriterio, "Data Medição Início:", dataInicioMedicao) : string.Empty,
                    !string.IsNullOrWhiteSpace(dataFimMedicao) ? string.Format(formatoCriterio, "Data Medição Fim:", dataFimMedicao) : string.Empty);
            }

            #endregion

            if (!string.IsNullOrEmpty(bairro))
                filtroAdicional += " And (p.bairroObra like ?bairro Or c.bairro like ?bairro)";

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
            int? tipoEntrega, int? tipoFiscal, string tiposVenda, bool trazerPedCliVinculado, int? usuarioCadastro, string bairro, string dataInicioMedicao, string dataFimMedicao, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            string filtroAdicional;

            if (PedidoConfig.ListaVendasPedidosVaziaPorPadrao && FiltrosVazios(altura, cidade, codCliente, codigoProduto, dataFimEntrega, dataFimInstalacao,
                 dataFimPedido, dataFimPronto, dataFimSituacao, dataInicioEntrega, dataInicioInstalacao, dataInicioPedido, dataInicioPronto,
                 dataInicioSituacao, desconto, descricaoProduto, fastDelivery, idCarregamento,idCliente, idFunc, idMedidor, idOrcamento, idOC,
                 idPedido, idsBenef, idsGrupo, idsRota, idsSubgrupoProd, idVendAssoc, largura, loja, nomeCliente,
                 numeroDiasDiferencaProntoLib, origemPedido, situacao, situacaoProducao, tipoCliente,
                 tipoEntrega, tipoFiscal, usuarioCadastro, bairro, dataInicioMedicao, dataFimMedicao))
                return new List<Pedido>().ToArray();


            var sql = SqlVendasPedidos(altura, cidade, codCliente, codigoProduto, comSemNF, false, dataFimEntrega, dataFimInstalacao, dataFimPedido, dataFimPronto, dataFimSituacao,
                dataInicioEntrega, dataInicioInstalacao, dataInicioPedido, dataInicioPronto, dataInicioSituacao, desconto, descricaoProduto, exibirProdutos, false, fastDelivery, out filtroAdicional, idCarregamento,
                idCliente, idFunc, idMedidor, idOC, idOrcamento, idPedido, idsBenef, idsGrupo, null, idsRota, idsSubgrupoProd, idVendAssoc, largura, UserInfo.GetUserInfo, loja, nomeCliente,
                numeroDiasDiferencaProntoLib, observacao, ordenacao, origemPedido, false, pedidosSemAnexos, situacao, situacaoProducao, out temFiltro, tiposPedido, tipoCliente, tipoEntrega,
                tipoFiscal, tiposVenda, false, trazerPedCliVinculado, usuarioCadastro, bairro, dataInicioMedicao, dataFimMedicao).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : string.Empty);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, temFiltro, filtroAdicional, ObterParametrosFiltrosVendasPedidos(codCliente, codigoProduto, dataFimEntrega,
                dataFimPedido, dataFimInstalacao, dataFimPronto, dataFimSituacao, dataInicioEntrega, dataInicioInstalacao, dataInicioPedido, dataInicioPronto, dataInicioSituacao, descricaoProduto,
                nomeCliente, observacao, bairro, dataInicioMedicao, dataFimMedicao)).ToArray();
        }

        private bool FiltrosVazios(float? altura, int? cidade, string codCliente, string codigoProduto, string dataFimEntrega, string dataFimInstalacao,
            string dataFimPedido, string dataFimPronto, string dataFimSituacao, string dataInicioEntrega, string dataInicioInstalacao, string dataInicioPedido, string dataInicioPronto,
            string dataInicioSituacao, int? desconto, string descricaoProduto, int? fastDelivery, int? idCarregamento, string idCliente, int? idFunc, int? idMedidor, int? idOrcamento, int? idOC,
            int? idPedido, string idsBenef, string idsGrupo, string idsRota, string idsSubgrupoProd, int? idVendAssoc, int? largura, string loja, string nomeCliente,
            int? numeroDiasDiferencaProntoLib, int? origemPedido, string situacao, string situacaoProducao, string tipoCliente,
            int? tipoEntrega, int? tipoFiscal, int? usuarioCadastro, string bairro, string dataInicioMedicao, string dataFimMedicao)
        {
            return altura.GetValueOrDefault() == 0 && cidade.GetValueOrDefault() == 0 && string.IsNullOrEmpty(codCliente) && string.IsNullOrEmpty(codigoProduto) &&
                string.IsNullOrEmpty(dataFimEntrega) && string.IsNullOrEmpty(dataFimInstalacao) && string.IsNullOrEmpty(dataFimPedido) && string.IsNullOrEmpty(dataFimPronto) &&
                string.IsNullOrEmpty(dataFimSituacao) && string.IsNullOrEmpty(dataInicioEntrega) && string.IsNullOrEmpty(dataInicioInstalacao) && string.IsNullOrEmpty(dataInicioPedido) &&
                string.IsNullOrEmpty(dataInicioPronto) && string.IsNullOrEmpty(dataInicioSituacao) && desconto.GetValueOrDefault() == 0 && string.IsNullOrEmpty(descricaoProduto) &&
                fastDelivery.GetValueOrDefault() == 0 && idCarregamento.GetValueOrDefault() == 0 && string.IsNullOrEmpty(idCliente) && idFunc.GetValueOrDefault() == 0 && idMedidor.GetValueOrDefault() == 0 &&
                idOrcamento.GetValueOrDefault() == 0 && idOC.GetValueOrDefault() == 0 && idPedido.GetValueOrDefault() == 0 && string.IsNullOrEmpty(idsBenef) && string.IsNullOrEmpty(idsGrupo) &&
                string.IsNullOrEmpty(idsRota) && string.IsNullOrEmpty(idsSubgrupoProd) && idVendAssoc.GetValueOrDefault() == 0 && largura.GetValueOrDefault() == 0 && string.IsNullOrEmpty(loja) &&
                string.IsNullOrEmpty(nomeCliente) && numeroDiasDiferencaProntoLib.GetValueOrDefault() == 0 && origemPedido.GetValueOrDefault() == 0 && string.IsNullOrEmpty(situacao) &&
                string.IsNullOrEmpty(situacaoProducao) && string.IsNullOrEmpty(tipoCliente) && tipoEntrega.GetValueOrDefault() == 0 && tipoFiscal.GetValueOrDefault() == 0 && usuarioCadastro.GetValueOrDefault() == 0 &&
                string.IsNullOrEmpty(bairro) && string.IsNullOrEmpty(dataInicioMedicao) && string.IsNullOrEmpty(dataFimMedicao);
        }

        /// <summary>
        /// Retorna a quantidade de pedidos, com base nos parâmetros, para a paginação da listagem da tela de vendas de pedidos.
        /// </summary>
        public int PesquisarListaVendasPedidosCount(float? altura, int? cidade, string codCliente, string codigoProduto, string comSemNF, string dataFimEntrega, string dataFimInstalacao,
            string dataFimPedido, string dataFimPronto, string dataFimSituacao, string dataInicioEntrega, string dataInicioInstalacao, string dataInicioPedido, string dataInicioPronto,
            string dataInicioSituacao, int? desconto, string descricaoProduto, bool exibirProdutos, int? fastDelivery, int? idCarregamento, string idCliente, int? idFunc, int? idMedidor, int? idOrcamento, int? idOC,
            int? idPedido, string idsBenef, string idsGrupo, string idsRota, string idsSubgrupoProd, int? idVendAssoc, int? largura, string loja, string nomeCliente,
            int? numeroDiasDiferencaProntoLib, string observacao, int? ordenacao, int? origemPedido, bool pedidosSemAnexos, string situacao, string situacaoProducao, string tiposPedido, string tipoCliente,
            int? tipoEntrega, int? tipoFiscal, string tiposVenda, bool trazerPedCliVinculado, int? usuarioCadastro, string bairro, string dataInicioMedicao, string dataFimMedicao)
        {
            bool temFiltro;
            string filtroAdicional;

            if (PedidoConfig.ListaVendasPedidosVaziaPorPadrao && FiltrosVazios(altura, cidade, codCliente, codigoProduto, dataFimEntrega, dataFimInstalacao,
                 dataFimPedido, dataFimPronto, dataFimSituacao, dataInicioEntrega, dataInicioInstalacao, dataInicioPedido, dataInicioPronto,
                 dataInicioSituacao, desconto, descricaoProduto, fastDelivery, idCarregamento, idCliente, idFunc, idMedidor, idOrcamento, idOC,
                 idPedido, idsBenef, idsGrupo, idsRota, idsSubgrupoProd, idVendAssoc, largura, loja, nomeCliente,
                 numeroDiasDiferencaProntoLib, origemPedido, situacao, situacaoProducao, tipoCliente,
                 tipoEntrega, tipoFiscal, usuarioCadastro, bairro, dataInicioMedicao, dataFimMedicao))
            return 0;

            var sql = SqlVendasPedidos(altura, cidade, codCliente, codigoProduto, comSemNF, true, dataFimEntrega, dataFimInstalacao, dataFimPedido, dataFimPronto, dataFimSituacao, dataInicioEntrega,
            dataInicioInstalacao, dataInicioPedido, dataInicioPronto, dataInicioSituacao, desconto, descricaoProduto, exibirProdutos, false, fastDelivery, out filtroAdicional, idCarregamento, idCliente,
            idFunc, idMedidor, idOC, idOrcamento, idPedido, idsBenef, idsGrupo, null, idsRota, idsSubgrupoProd, idVendAssoc, largura, UserInfo.GetUserInfo, loja, nomeCliente,
             numeroDiasDiferencaProntoLib, observacao, ordenacao, origemPedido, false, pedidosSemAnexos, situacao, situacaoProducao, out temFiltro, tiposPedido, tipoCliente, tipoEntrega,
            tipoFiscal, tiposVenda, false, trazerPedCliVinculado, usuarioCadastro, bairro, dataInicioMedicao, dataFimMedicao).Replace("?filtroAdicional?", temFiltro ? filtroAdicional : string.Empty);

            return GetCountWithInfoPaging(sql, temFiltro, filtroAdicional, ObterParametrosFiltrosVendasPedidos(codCliente, codigoProduto, dataFimEntrega, dataFimPedido, dataFimInstalacao,
                dataFimPronto, dataFimSituacao, dataInicioEntrega, dataInicioInstalacao, dataInicioPedido, dataInicioPronto, dataInicioSituacao, descricaoProduto, nomeCliente, observacao,
                bairro, dataInicioMedicao, dataFimMedicao));
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
            string situacaoProducao, string tiposPedido, string tipoCliente, int? tipoEntrega, int? tipoFiscal, string tiposVenda, bool trazerPedCliVinculado, int? usuarioCadastro,
            string bairro, string dataInicioMedicao, string dataFimMedicao)
        {
            bool temFiltro;
            string filtroAdicional;

            var sql = SqlVendasPedidos(altura, cidade, codCliente, codigoProduto, comSemNF, false, dataFimEntrega, dataFimInstalacao, dataFimPedido, dataFimPronto, dataFimSituacao,
                dataInicioEntrega, dataInicioInstalacao, dataInicioPedido, dataInicioPronto, dataInicioSituacao, desconto, descricaoProduto, exibirProdutos, exibirRota, fastDelivery,
                out filtroAdicional, idCarregamento, idCliente, idFunc, idMedidor, idOC, idOrcamento, idPedido, idsBenef, idsGrupo, idsPedidos, idsRota, idsSubgrupoProd, idVendAssoc, largura, login, loja,
                nomeCliente, numeroDiasDiferencaProntoLib, observacao, ordenacao, origemPedido, true, pedidosSemAnexos, situacao, situacaoProducao, out temFiltro, tiposPedido, tipoCliente,
                tipoEntrega, tipoFiscal, tiposVenda, false, trazerPedCliVinculado, usuarioCadastro, bairro, dataInicioMedicao, dataFimMedicao).Replace("?filtroAdicional?", filtroAdicional);

            return objPersistence.LoadData(sql, ObterParametrosFiltrosVendasPedidos(codCliente, codigoProduto, dataFimEntrega, dataFimPedido, dataFimInstalacao, dataFimPronto, dataFimSituacao,
                dataInicioEntrega, dataInicioInstalacao, dataInicioPedido, dataInicioPronto, dataInicioSituacao, descricaoProduto, nomeCliente, observacao, bairro, dataInicioMedicao, dataFimMedicao)).ToArray();
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
            int? tipoEntrega, int? tipoFiscal, string tiposVenda, bool trazerPedCliVinculado, int? usuarioCadastro, string bairro, string dataInicioMedicao, string dataFimMedicao)
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
                string.IsNullOrEmpty(situacaoProducao) && string.IsNullOrEmpty(tipoCliente) && tipoEntrega.GetValueOrDefault() == 0 && tipoFiscal.GetValueOrDefault() == 0 && usuarioCadastro.GetValueOrDefault() == 0 &&
                string.IsNullOrEmpty(bairro) && string.IsNullOrEmpty(dataInicioMedicao) && string.IsNullOrEmpty(dataFimMedicao))
                return new TotalListaPedidos();

            // Passa o parâmetro totaisListaPedidos como true para recuperar somente os campos que serão somados.
            var sql = SqlVendasPedidos(altura, cidade, codCliente, codigoProduto, comSemNF, false, dataFimEntrega, dataFimInstalacao, dataFimPedido, dataFimPronto, dataFimSituacao,
                dataInicioEntrega, dataInicioInstalacao, dataInicioPedido, dataInicioPronto, dataInicioSituacao, desconto, descricaoProduto, exibirProdutos, false, fastDelivery, out filtroAdicional, idCarregamento,
                idCliente, idFunc, idMedidor, idOC, idOrcamento, idPedido, idsBenef, idsGrupo, null, idsRota, idsSubgrupoProd, idVendAssoc, largura, UserInfo.GetUserInfo, loja, nomeCliente,
                 numeroDiasDiferencaProntoLib, observacao, ordenacao, origemPedido, false, pedidosSemAnexos, situacao, situacaoProducao, out temFiltro, tiposPedido, tipoCliente, tipoEntrega,
                tipoFiscal, tiposVenda, true, trazerPedCliVinculado, usuarioCadastro, bairro, dataInicioMedicao, dataFimMedicao).Replace("?filtroAdicional?", filtroAdicional);

            // Soma somente os campos que serão exibidos na tela.
            // TotalReal: total do pedido comercial, mesmo exibido na listagem e relatório de vendas pedidos.
            sql = string.Format("SELECT CAST(CONCAT(SUM(TotalReal), ';', SUM(TotM), ';', SUM(Peso)) AS CHAR) FROM ({0}) AS Temp", sql);

            return new TotalListaPedidos(ExecuteScalar<string>(sql, ObterParametrosFiltrosVendasPedidos(codCliente, codigoProduto, dataFimEntrega, dataFimPedido, dataFimInstalacao, dataFimPronto,
                dataFimSituacao, dataInicioEntrega, dataInicioInstalacao, dataInicioPedido, dataInicioPronto, dataInicioSituacao, descricaoProduto, nomeCliente, observacao, bairro, dataInicioMedicao, dataFimMedicao)));
        }

        #endregion

        #region Parâmetros

        private GDAParameter[] ObterParametrosFiltrosVendasPedidos(string codCliente, string codigoProduto, string dataFimEntrega, string dataFimPedido, string dataFimInstalacao,
            string dataFimPronto, string dataFimSituacao, string dataInicioEntrega, string dataInicioInstalacao, string dataInicioPedido, string dataInicioPronto, string dataInicioSituacao,
            string descricaoProduto, string nomeCliente, string observacao)
        {
            return ObterParametrosFiltrosVendasPedidos(codCliente, codigoProduto, dataFimEntrega, dataFimPedido, dataFimInstalacao,
             dataFimPronto, dataFimSituacao, dataInicioEntrega, dataInicioInstalacao, dataInicioPedido, dataInicioPronto, dataInicioSituacao,
             descricaoProduto, nomeCliente, observacao, null, null, null);
        }

        /// <summary>
        /// Retorna os parâmetros que devem ser substituídos no SQL, com base nos filtros informados.
        /// </summary>
        private GDAParameter[] ObterParametrosFiltrosVendasPedidos(string codCliente, string codigoProduto, string dataFimEntrega, string dataFimPedido, string dataFimInstalacao,
            string dataFimPronto, string dataFimSituacao, string dataInicioEntrega, string dataInicioInstalacao, string dataInicioPedido, string dataInicioPronto, string dataInicioSituacao,
            string descricaoProduto, string nomeCliente, string observacao, string bairro, string dataInicioMedicao, string dataFimMedicao)
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

            if (!string.IsNullOrWhiteSpace(bairro))
                parametros.Add(new GDAParameter("?bairro", "%" + bairro + "%"));

            if (!string.IsNullOrWhiteSpace(dataInicioMedicao))
                parametros.Add(new GDAParameter("?dataInicioMedicao", DateTime.Parse(dataInicioMedicao + " 00:00")));

            if (!string.IsNullOrWhiteSpace(dataFimMedicao))
                parametros.Add(new GDAParameter("?dataFimMedicao", DateTime.Parse(dataFimMedicao + " 23:59")));

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
    }
}