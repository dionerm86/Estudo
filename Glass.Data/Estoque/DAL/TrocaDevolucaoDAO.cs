using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class TrocaDevolucaoDAO : BaseDAO<TrocaDevolucao, TrocaDevolucaoDAO>
    {
        //private TrocaDevolucaoDAO() { }

        #region Busca padrão

        private string Sql(uint idTrocaDevolucao, uint idPedido, int tipo, int situacao, uint idCliente, string nomeCliente,
            string idsFunc, string idsFuncionarioAssociadoCliente, string dataIni, string dataFim, uint idProduto,
            int alturaMin, int alturaMax, int larguraMin, int larguraMax, uint idOrigemTrocaDevolucao, uint idTipoPerda, int idSetor, string tipoPedido, int idGrupo, int idSubgrupo,
            bool selecionar)
        {
            return Sql(null, idTrocaDevolucao, idPedido, tipo, situacao, idCliente, nomeCliente, idsFunc, idsFuncionarioAssociadoCliente, dataIni,
                dataFim, idProduto, alturaMin, alturaMax, larguraMin, larguraMax, idOrigemTrocaDevolucao, idTipoPerda, idSetor, tipoPedido, idGrupo, idSubgrupo, selecionar);
        }

        private string Sql(GDASession session, uint idTrocaDevolucao, uint idPedido, int tipo, int situacao, uint idCliente, string nomeCliente,
            string idsFunc, string idsFuncionarioAssociadoCliente, string dataIni, string dataFim, uint idProduto,
            int alturaMin, int alturaMax, int larguraMin, int larguraMax, uint idOrigemTrocaDevolucao, uint idTipoPerda, int idSetor, string tipoPedido, int idGrupo, int idSubgrupo,
            bool selecionar)
        {
            string criterio = "";
            string campos = selecionar ? @"td.*, f.nome as nomeFunc, c.revenda as cliRevenda, c.nome as nomeCliente,
                p.tipoEntrega as tipoEntregaPedido, c.credito as creditoCliente, '$$$' as criterio, c.endereco as enderecoCliente, 
                c.numero as numeroCliente, c.compl as complCliente, c.bairro as bairroCliente, cid.nomeCidade as cidadeCliente,
                cid.nomeUf as ufCliente, c.cep as cepCliente, concat(c.tel_Res, ' / ', c.tel_Cel, ' / ', c.tel_Cont) as telContCliente,
                CAST(coalesce(p.idLoja, f.idLoja) as UNSIGNED) as idLoja, otd.descricao as DescrOrigemTrocaDevolucao,
                c.IdFunc As IdFuncionarioAssociadoCliente, fc.Nome AS NomeFuncionarioAssociadoCliente, s.Descricao as Setor,
                l.nomeFantasia as loja, f2.nome as nomeUsuCad,
                (SELECT sum(qtde) FROM produto_trocado WHERE IdTrocaDevolucao = td.IdTrocaDevolucao) as QtdePecas"
                : "count(*)";

            string sql = @"
                select " + campos + @"
                from troca_devolucao td
                    left join funcionario f on (td.idFunc=f.idFunc)
                    left join pedido p on (td.idPedido=p.idPedido)
                    left join cliente c on (td.idCliente=c.id_Cli)
                    left join funcionario fc on (c.IdFunc=fc.IdFunc)
                    left join funcionario f2 on (td.UsuCad=f2.IdFunc)
                    left join cidade cid on (c.idCidade=cid.idCidade)
                    left join origem_troca_desconto otd on (td.idOrigemTrocaDevolucao = otd.idOrigemTrocaDesconto)
                    LEFT JOIN setor s ON (td.IdSetor = s.IdSetor) 
                    LEFT JOIN loja l ON (p.IdLoja = l.IdLoja)
                where 1";

            TrocaDevolucao temp = new TrocaDevolucao();

            if (idTrocaDevolucao > 0)
            {
                sql += " and td.idTrocaDevolucao=" + idTrocaDevolucao;
                criterio += "Código: " + idTrocaDevolucao + "    ";
            }

            if (idPedido > 0)
            {
                sql += " and td.idPedido=" + idPedido;
                criterio += "Pedido: " + idPedido + "    ";
            }

            if (!string.IsNullOrEmpty(tipoPedido))
            {
                sql += string.Format(" AND p.TipoPedido IN ({0})", tipoPedido);
                var tiposPedido = tipoPedido.Split(',').Select(f => Colosoft.Translator.Translate(((Glass.Data.Model.Pedido.TipoPedidoEnum)f.StrParaInt())).ToString()).ToList();
                criterio += string.Format("Tipo Pedido: {0}", string.Join(", ", tiposPedido.ToArray())) + "   ";
            }

            if (tipo > 0)
            {
                sql += " and td.tipo=" + tipo;
                temp.Tipo = tipo;
                criterio += "Tipo: " + temp.DescrTipo + "    ";
            }

            if (situacao > 0)
            {
                sql += " and td.situacao=" + situacao;
                temp.Situacao = situacao;
                criterio += "Situação: " + temp.DescrSituacao + "    ";
            }

            if (idCliente > 0)
            {
                sql += " and td.idCliente=" + idCliente;
                criterio += "Cliente: " + ClienteDAO.Instance.GetNome(session, idCliente) + "    ";
            }
            else if (!String.IsNullOrEmpty(nomeCliente))
            {
                string ids = ClienteDAO.Instance.GetIds(session, null, nomeCliente, null, 0, null, null, null, null, 0);
                sql += " And c.id_Cli in (" + ids + ")";
                criterio += "Cliente: " + nomeCliente + "    ";
            }

            if (!String.IsNullOrEmpty(idsFunc) && idsFunc != "0")
            {
                sql += String.Format(" and td.idFunc IN ({0})", idsFunc);
                var criterioFunc = String.Empty;

                foreach (var id in idsFunc.Split(','))
                    criterioFunc += FuncionarioDAO.Instance.GetNome(session, Glass.Conversoes.StrParaUint(id)) + "    ";

                criterio += "Funcionário: " + criterioFunc;
            }

            if (!String.IsNullOrEmpty(idsFuncionarioAssociadoCliente) && idsFuncionarioAssociadoCliente != "0")
            {
                sql += String.Format(" and c.idFunc IN ({0})", idsFuncionarioAssociadoCliente);
                var criterioFunc = String.Empty;

                foreach (var id in idsFuncionarioAssociadoCliente.Split(','))
                    criterioFunc += FuncionarioDAO.Instance.GetNome(session, Glass.Conversoes.StrParaUint(id)) + "    ";

                criterio += "Funcionário Assoc. Cli.: " + criterioFunc;
            }

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " and td.dataTroca>=?dataIni";
                criterio += (!String.IsNullOrEmpty(dataFim) ? "Período: de " + dataIni : "Período: a partir de " + dataIni) + "    ";
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " and td.dataTroca<=?dataFim";
                criterio += (!String.IsNullOrEmpty(dataIni) ? " até " + dataFim : "Período: até " + dataFim) + "    ";
            }

            if (idProduto > 0)
            {
                sql += " and td.idTrocaDevolucao in (select pt.IDTROCADEVOLUCAO from produto_trocado pt where pt.IDPROD=" + idProduto + ")";
                criterio += "Produto: " + ProdutoDAO.Instance.GetElement(session, idProduto).Descricao + "    ";
            }

            string whereProd = "";

            if (alturaMin > 0)
            {
                whereProd += " and altura>=" + alturaMin;
                criterio += "Altura mínima: " + alturaMin + "    ";
            }

            if (alturaMax > 0)
            {
                whereProd += " and altura<=" + alturaMax;
                criterio += "Altura máxima: " + alturaMax + "    ";
            }

            if (larguraMin > 0)
            {
                whereProd += " and largura>=" + larguraMin;
                criterio += "Largura mínima: " + larguraMin + "    ";
            }

            if (larguraMax > 0)
            {
                whereProd += " and largura<=" + larguraMax;
                criterio += "Largura máxima: " + larguraMax + "    ";
            }

            if (idGrupo > 0)
            {
                whereProd += " AND idProd IN (SELECT IdProd FROM produto WHERE idGrupoProd =" + idGrupo + ")";
                criterio += "Grupo Prod.: " + GrupoProdDAO.Instance.GetDescricao(idGrupo) + "    ";
            }

            if (idSubgrupo > 0)
            {
                whereProd += " AND idProd IN (SELECT idProd FROM produto WHERE idSubgrupoProd =" + idSubgrupo + ")";
                criterio += "Subgrupo Prod.: " + SubgrupoProdDAO.Instance.GetDescricao(idSubgrupo) + "    ";
            }

            if (idOrigemTrocaDevolucao > 0)
            {
                sql += " AND td.idOrigemTrocaDevolucao=" + idOrigemTrocaDevolucao;
                criterio += "Origem Troca / Devolução: " + OrigemTrocaDescontoDAO.Instance.ObtemDescricao(session, idOrigemTrocaDevolucao) + "    ";
            }

            if (!String.IsNullOrEmpty(whereProd))
                sql += String.Format(@" and (exists (select * from produto_trocado where idTrocaDevolucao=td.idTrocaDevolucao {0})
                    or exists (select * from produto_troca_dev where idTrocaDevolucao=td.idTrocaDevolucao {0}))", whereProd);

            if (idTipoPerda > 0)
            {
                sql += " AND td.idTipoPerda = " + idTipoPerda;
                criterio += "Tipo Perda: " + TipoPerdaDAO.Instance.GetNome(idTipoPerda) + "  ";
            }

            if (idSetor > 0)
            {
                sql += " AND td.idSetor = " + idSetor;
                criterio += "Setor: " + SetorDAO.Instance.ObtemNomeSetor((uint)idSetor) + "  ";
            }

            sql = sql.Replace("$$$", criterio);

            return sql;
        }

        private GDAParameter[] GetParams(string nomeCliente, string dataIni, string dataFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeCliente))
                lstParam.Add(new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"));

            if (!String.IsNullOrEmpty(dataIni))
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        public IList<TrocaDevolucao> GetList(uint idTrocaDevolucao, uint idPedido, int tipo, int situacao,
            uint idCliente, string nomeCliente, string idsFuncionario, string idsFuncionarioAssociadoCliente, string dataIni, string dataFim, uint idProduto,
            int alturaMin, int alturaMax, int larguraMin, int larguraMax, uint idOrigemTrocaDevolucao, uint idTipoPerda, int idSetor, string tipoPedido, int idGrupo, int idSubgrupo,
            string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? "td.idTrocaDevolucao desc" : sortExpression;

            return LoadDataWithSortExpression(Sql(idTrocaDevolucao, idPedido, tipo, situacao, idCliente, nomeCliente,
                idsFuncionario, idsFuncionarioAssociadoCliente, dataIni, dataFim, idProduto, alturaMin, alturaMax, larguraMin, larguraMax, idOrigemTrocaDevolucao, idTipoPerda, idSetor, tipoPedido, idGrupo, idSubgrupo,
                true), sort, startRow, pageSize, GetParams(nomeCliente, dataIni, dataFim));
        }

        public int GetCount(uint idTrocaDevolucao, uint idPedido, int tipo, int situacao, uint idCliente, string nomeCliente, string idsFuncionario,
            string idsFuncionarioAssociadoCliente, string dataIni, string dataFim, uint idProduto, int alturaMin, int alturaMax,
            int larguraMin, int larguraMax, uint idOrigemTrocaDevolucao, uint idTipoPerda, int idSetor, string tipoPedido, int idGrupo, int idSubgrupo)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idTrocaDevolucao, idPedido, tipo, situacao, idCliente, nomeCliente, idsFuncionario,
                idsFuncionarioAssociadoCliente, dataIni, dataFim, idProduto, alturaMin, alturaMax, larguraMin, larguraMax, idOrigemTrocaDevolucao, idTipoPerda, idSetor,
                tipoPedido, idGrupo, idSubgrupo, false),
                GetParams(nomeCliente, dataIni, dataFim));
        }

        public IList<TrocaDevolucao> GetForRpt(uint idTrocaDevolucao, uint idPedido, int tipo, int situacao, uint idCliente,
            string nomeCliente, string idsFunc, string idsFuncionarioAssociadoCliente, string dataIni, string dataFim,
            uint idProduto, int alturaMin, int alturaMax, int larguraMin, int larguraMax, uint idOrigemTrocaDevolucao, uint idTipoPerda, int idSetor,
            string tipoPedido, int idGrupo, int idSubgrupo)
        {
            return objPersistence.LoadData(Sql(idTrocaDevolucao, idPedido, tipo, situacao, idCliente, nomeCliente, idsFunc, idsFuncionarioAssociadoCliente,
                dataIni, dataFim, idProduto, alturaMin, alturaMax, larguraMin, larguraMax, idOrigemTrocaDevolucao, idTipoPerda, idSetor, tipoPedido, idGrupo, idSubgrupo, true) +
                "  order by td.idTrocaDevolucao desc",
                GetParams(nomeCliente, dataIni, dataFim)).ToList();
        }

        public TrocaDevolucao GetElement(uint idTrocaDevolucao)
        {
            return GetElement(null, idTrocaDevolucao);
        }

        public TrocaDevolucao GetElement(GDASession session, uint idTrocaDevolucao)
        {
            return objPersistence.LoadOneData(session, Sql(session, idTrocaDevolucao, 0, 0, 0, 0, null, null, null, null, null, 0, 0, 0, 0, 0, 0, 0, 0, null, 0, 0, true));
        }

        #endregion

        #region Verifica se há valor a ser pago na troca

        /// <summary>
        /// Verifica se há valor a ser pago na troca.
        /// </summary>
        /// <param name="idTrocaDevolucao"></param>
        /// <returns></returns>
        public bool TemValorExcedente(uint idTrocaDevolucao)
        {
            return TemValorExcedente(null, idTrocaDevolucao);
        }

        /// <summary>
        /// Verifica se há valor a ser pago na troca.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idTrocaDevolucao"></param>
        /// <returns></returns>
        public bool TemValorExcedente(GDASession session, uint idTrocaDevolucao)
        {
            return objPersistence.ExecuteSqlQueryCount(session,
                "select count(*) from troca_devolucao where valorExcedente>0 and idTrocaDevolucao=" + idTrocaDevolucao) > 0;
        }

        #endregion

        #region Altera o estoque

        private void AlteraEstoqueFinalizar(uint idTrocaDevolucao)
        {
            AlteraEstoqueFinalizar(null, idTrocaDevolucao);
        }

        private void AlteraEstoqueFinalizar(GDASession session, uint idTrocaDevolucao)
        {
            // Credita o estoque dos produtos trocados/devolvidos
            foreach (ProdutoTrocado pt in ProdutoTrocadoDAO.Instance.GetByTrocaDevolucao(session, idTrocaDevolucao))
            {
                bool m2 = pt.TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || pt.TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;

                int tipoCalculo = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session, (int)pt.IdProd);
                float qtdCredito = pt.Qtde;

                if (tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                    tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 ||
                    tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
                    qtdCredito *= pt.Altura;

                if (pt.ComDefeito)
                    ProdutoLojaDAO.Instance.CreditaDefeito(session, pt.IdProd, UserInfo.GetUserInfo.IdLoja, m2 ? pt.TotM : qtdCredito);
                else if (pt.AlterarEstoque)
                    MovEstoqueDAO.Instance.CreditaEstoqueTrocaDevolucao(session, pt.IdProd, UserInfo.GetUserInfo.IdLoja, idTrocaDevolucao, null,
                        pt.IdProdTrocado, (decimal)(m2 ? pt.TotM : qtdCredito));
            }

            if (!ObtemUsarPedidoReposicao(session, idTrocaDevolucao))
            {
                // Baixa o estoque dos produtos novos
                foreach (ProdutoTrocaDevolucao ptd in ProdutoTrocaDevolucaoDAO.Instance.GetByTrocaDevolucao(session, idTrocaDevolucao))
                {
                    if (!ptd.AlterarEstoque)
                        continue;

                    bool m2 = ptd.TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || ptd.TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;

                    int tipoCalculo = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session, (int)ptd.IdProd);
                    float qtdSaida = ptd.Qtde;

                    if (tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                        tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 ||
                        tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
                        qtdSaida *= ptd.Altura;

                    MovEstoqueDAO.Instance.BaixaEstoqueTrocaDevolucao(session, ptd.IdProd, UserInfo.GetUserInfo.IdLoja, idTrocaDevolucao,
                        ptd.IdProdTrocaDev, null, (decimal)(m2 ? ptd.TotM : qtdSaida));
                }
            }
        }

        private void AlteraEstoqueCancelar(uint idTrocaDevolucao)
        {
            AlteraEstoqueCancelar(null, idTrocaDevolucao);
        }

        private void AlteraEstoqueCancelar(GDASession session, uint idTrocaDevolucao)
        {
            // Baixa o estoque dos produtos trocados/devolvidos
            foreach (ProdutoTrocado pt in ProdutoTrocadoDAO.Instance.GetByTrocaDevolucao(session, idTrocaDevolucao))
            {
                bool m2 = pt.TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || pt.TipoCalc == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;

                int tipoCalculo = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session, (int)pt.IdProd);
                float qtdSaida = pt.Qtde;

                if (tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                    tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 ||
                    tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
                    qtdSaida *= pt.Altura;

                if (pt.ComDefeito)
                    ProdutoLojaDAO.Instance.BaixaDefeito(session, pt.IdProd, UserInfo.GetUserInfo.IdLoja, m2 ? pt.TotM : qtdSaida);
                else if (pt.AlterarEstoque)
                    MovEstoqueDAO.Instance.BaixaEstoqueTrocaDevolucao(session, pt.IdProd, UserInfo.GetUserInfo.IdLoja, idTrocaDevolucao, null,
                        pt.IdProdTrocado, (decimal)(m2 ? pt.TotM : qtdSaida));
            }

            if (!ObtemUsarPedidoReposicao(session, idTrocaDevolucao))
            {
                // Credita o estoque dos produtos novos
                foreach (ProdutoTrocaDevolucao ptd in ProdutoTrocaDevolucaoDAO.Instance.GetByTrocaDevolucao(session, idTrocaDevolucao))
                {
                    if (!ptd.AlterarEstoque)
                        continue;

                    int tipoCalculo = Glass.Data.DAL.GrupoProdDAO.Instance.TipoCalculo(session, (int)ptd.IdProd);
                    float qtdCredito = ptd.Qtde;

                    if (tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL0 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL05 ||
                        tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL1 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.MLAL6 ||
                        tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.ML)
                        qtdCredito *= ptd.Altura;

                    bool m2 = tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2 || tipoCalculo == (int)Glass.Data.Model.TipoCalculoGrupoProd.M2Direto;
                    MovEstoqueDAO.Instance.CreditaEstoqueTrocaDevolucao(session, ptd.IdProd, UserInfo.GetUserInfo.IdLoja, idTrocaDevolucao,
                        ptd.IdProdTrocaDev, null, (decimal)(m2 ? ptd.TotM : qtdCredito));
                }
            }
        }

        #endregion

        #region Finalizar a troca

        private IDictionary<uint, decimal> ValorPagtoAntecipadoPedido(TrocaDevolucao troca)
        {
            return ValorPagtoAntecipadoPedido(null, troca);
        }

        private IDictionary<uint, decimal> ValorPagtoAntecipadoPedido(GDASession session, TrocaDevolucao troca)
        {
            var retorno = new Dictionary<uint, decimal>();

            var idsProdutosPedidos = ExecuteMultipleScalar<string>(session,
                @"select concat(pp.idPedido, ',', group_concat(concat(ptd.idProdPed, ',', ptd.qtde)))
                from produtos_pedido pp inner join produto_troca_dev ptd on (pp.idProdPed=ptd.idProdPed) 
                where ptd.idTrocaDevolucao=" + troca.IdTrocaDevolucao + " group by pp.idPedido");

            foreach (var idPP in idsProdutosPedidos)
            {
                if (String.IsNullOrEmpty(idPP))
                    continue;

                var ids = Array.ConvertAll(idPP.Split(','), x => Glass.Conversoes.StrParaFloat(x));

                var idPedido = (uint)ids[0];
                var total = 0M;

                for (int i = 1; i < ids.Length; i += 2)
                {
                    var idProdPed = (uint)ids[i];
                    var qtde = ids[i + 1];

                    var totProd = ProdutosPedidoDAO.Instance.ObtemValorCampo<decimal>(session,
                        "total+coalesce(valorBenef,0)", "idProdPed=" + idProdPed);

                    var qtdeProd = ProdutosPedidoDAO.Instance.ObtemValorCampo<float>(session,
                        "qtde", "idProdPed=" + idProdPed);

                    total += totProd / (decimal)qtdeProd * (decimal)qtde;
                }

                retorno.Add(idPedido, total * (troca.CreditoGerado / troca.CreditoGeradoMax));
            }

            return retorno;
        }

        private static readonly object _finalizarLock = new object();

        /// <summary>
        /// Finaliza a troca/devolução gerando crédito.
        /// </summary>
        public string Finalizar(uint idTrocaDevolucao, bool cxDiario)
        {
            lock (_finalizarLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        if (!Config.PossuiPermissao(Config.FuncaoMenuEstoque.FinalizarTrocaDevolucao))
                            throw new Exception("Você não tem permissão para finalizar troca/devolução.");

                        uint idContaGerada = 0;
                        TrocaDevolucao troca = GetElement(transaction, idTrocaDevolucao);

                        if (troca.Situacao == (int)TrocaDevolucao.SituacaoTrocaDev.Finalizada)
                            throw new Exception("Esta troca/devolução já foi finalizada.");

                        if (troca.Situacao == (int)TrocaDevolucao.SituacaoTrocaDev.Cancelada)
                            throw new Exception("Esta troca/devolução está cancelada.");

                        uint idMovimentacao = 0;
                        decimal creditoAtual = ClienteDAO.Instance.GetCredito(transaction, troca.IdCliente);
                        bool isReposicaoGarantia = false;

                        if (troca.IdCliente == 0)
                            throw new Exception("Informe o cliente desta troca antes de finalizá-la");

                        var produtos = ProdutoTrocadoDAO.Instance.GetByTrocaDevolucao(transaction, idTrocaDevolucao);

                        // Chamado 30847: Verifica se o produto foi calculado corretamente
                        foreach (var prodTroca in produtos)
                        {
                            if (prodTroca.IdProdPed.GetValueOrDefault() == 0)
                                continue;

                            var prodPed = ProdutosPedidoDAO.Instance.GetElementByPrimaryKey(transaction, prodTroca.IdProdPed.Value);

                            // Se a quantidade do produto trocado for diferente do produto do pedido, o valor total dos dois não pode ser igual
                            if (prodTroca.Qtde != prodPed.Qtde && prodPed.Total == prodTroca.Total)
                                throw new Exception("Falha no cálculo do produto trocado, remova-o e insira novamente na devolução.");
                        }

                        foreach (var p in produtos.Where(f => !string.IsNullOrEmpty(f.Etiquetas)))
                        {
                            foreach (var etq in p.Etiquetas.Split('|'))
                            {
                                var idProdPedProducao = etq.Split(';')[1].StrParaUint();
                                objPersistence.ExecuteCommand(transaction, "UPDATE produto_pedido_producao SET TrocadoDevolvido=1 WHERE IdProdPedProducao=" + idProdPedProducao);
                            }
                        }

                        // Se for pedido de reposição ou garantia não permite gerar/utilizar crédito
                        if (troca.IdPedido > 0)
                        {
                            isReposicaoGarantia = PedidoDAO.Instance.IsPedidoGarantia(transaction, troca.IdPedido.Value.ToString()) ||
                                (PedidoDAO.Instance.IsPedidoReposicao(transaction, troca.IdPedido.Value.ToString()) &&
                                !Liberacao.TelaLiberacao.CobrarPedidoReposicao);

                            if (isReposicaoGarantia)
                            {
                                troca.CreditoGerado = 0;
                                troca.CreditoGeradoFinalizar = 0;
                                troca.CreditoGeradoMax = 0;
                                troca.CreditoUtilizadoFinalizar = 0;
                                troca.ValorCreditoAoFinalizar = 0;
                                troca.ValorExcedente = 0;
                                creditoAtual = 0;

                                UpdateBase(transaction, troca);
                            }
                        }

                        try
                        {
                            #region Indica o valor do pagamento antecipado

                            if (troca.UsarPedidoReposicao && troca.CreditoGerado > 0)
                            {
                                foreach (var dados in ValorPagtoAntecipadoPedido(transaction, troca))
                                {
                                    objPersistence.ExecuteCommand(transaction, @"update pedido set valorPagamentoAntecipado=
                                        coalesce(valorPagamentoAntecipado, 0)+?valor where idPedido=" + dados.Key,
                                        new GDAParameter("?valor", dados.Value));
                                }
                            }

                            #endregion

                            #region Gera o crédito para o cliente

                            else if (troca.CreditoGerado > 0)
                            {
                                if (ClienteDAO.Instance.GetNome(transaction, troca.IdCliente).ToLower().Contains("consumidor final"))
                                    throw new Exception("Não é possível finalizar uma troca/devolução que gere crédito para consumidor final.");

                                //Gera o debito da comissão
                                DebitoComissaoDAO.Instance.AtualizaDebitoPedidoTrocaDevolucao(transaction, (int)troca.IdPedido, troca.IdTrocaDevolucao);

                                ClienteDAO.Instance.CreditaCredito(transaction, troca.IdCliente, troca.CreditoGerado);

                                if (cxDiario)
                                {
                                    idMovimentacao = CaixaDiarioDAO.Instance.MovCxTrocaDev(transaction, UserInfo.GetUserInfo.IdLoja, troca.IdCliente, troca.IdTrocaDevolucao,
                                        troca.IdPedido, 1, troca.CreditoGerado, 0, UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado), null, null, false);
                                }
                                else
                                {
                                    idMovimentacao = CaixaGeralDAO.Instance.MovCxTrocaDev(transaction, troca.IdTrocaDevolucao, troca.IdPedido, troca.IdCliente,
                                        UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.CreditoVendaGerado), 1, troca.CreditoGerado, 0, null, false, null, null);
                                }
                            }

                            #endregion

                            #region Gera conta a receber de valor excedente

                            else if (troca.ValorExcedente > 0)
                            {
                                // Gera a conta a receber
                                ContasReceber conta = new ContasReceber();
                                conta.IdLoja = UserInfo.GetUserInfo.IdLoja;
                                conta.IdTrocaDevolucao = idTrocaDevolucao;
                                conta.IdConta = UtilsPlanoConta.GetPlanoConta(UtilsPlanoConta.PlanoContas.TrocaDevolucao);
                                conta.ValorVec = troca.ValorExcedente;
                                conta.DataVec = DateTime.Now;
                                conta.IdCliente = troca.IdCliente;
                                conta.NumParc = 1;
                                conta.NumParcMax = 1;
                                conta.IdFuncComissaoRec = conta.IdCliente > 0 ? (int?)ClienteDAO.Instance.ObtemIdFunc(conta.IdCliente) : null;

                                idContaGerada = ContasReceberDAO.Instance.Insert(transaction, conta);
                            }

                            #endregion

                            // Altera o estoque
                            AlteraEstoqueFinalizar(transaction, idTrocaDevolucao);

                            // Atualiza a situação
                            objPersistence.ExecuteCommand(transaction, "update troca_devolucao set valorCreditoAoFinalizar=" + creditoAtual.ToString().Replace(",", ".") +
                                ", creditoGeradoFinalizar=0, creditoUtilizadoFinalizar=0, situacao=" + (int)TrocaDevolucao.SituacaoTrocaDev.Finalizada +
                                " where idTrocaDevolucao=" + idTrocaDevolucao);

                            // Chamado 47351
                            //Se tiver informado as etiquetas de box, volta a situação das mesmas para que possam ser usadas novamente
                            AtualizaEtiquetasBox(transaction, troca, produtos);
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao finalizar troca/devolução.", ex));
                        }

                        transaction.Commit();
                        transaction.Close();

                        return "Troca/devolução finalizada." + (idContaGerada > 0 ? " Foi gerada uma conta a receber para o pagamento dessa troca." : "");
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
        /// Finaliza a troca/devolução com pagamento de valor excedente.
        /// </summary>
        public string Finalizar(uint idTrocaDevolucao, decimal[] valoresReceb, uint[] formasPagto, uint[] contasBanco, uint[] depositoNaoIdentificado, uint[] cartaoNaoIdentificado,
            uint[] tiposCartao, uint[] tiposBoleto, decimal[] txAntecip, decimal juros, bool recebParcial, bool gerarCredito, decimal creditoUtilizado,
            string numAutConstrucard, bool cxDiario, uint[] numParcCartoes, string chequesPagto, string[] numAutCartao)
        {
            lock(_finalizarLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var msg = string.Empty;

                        if (!Config.PossuiPermissao(Config.FuncaoMenuEstoque.FinalizarTrocaDevolucao))
                            throw new Exception("Você não tem permissão para finalizar troca/devolução.");

                        if (!TemValorExcedente(transaction, idTrocaDevolucao))
                            throw new Exception("Não é possível finalizar essa troca sem valor excedente.");

                        UtilsFinanceiro.DadosRecebimento retorno = null;
                        TrocaDevolucao troca = GetElement(transaction, idTrocaDevolucao);

                        if (troca.Situacao == (int)TrocaDevolucao.SituacaoTrocaDev.Finalizada)
                            throw new Exception("Esta troca/devolução já foi finalizada.");

                        if (troca.Situacao == (int)TrocaDevolucao.SituacaoTrocaDev.Cancelada)
                            throw new Exception("Esta troca/devolução está cancelada.");

                        decimal totalPago = 0;
                        foreach (decimal valor in valoresReceb)
                            totalPago += valor;

                        // Se for pago com crédito, soma o mesmo ao totalPago
                        if (creditoUtilizado > 0)
                            totalPago += creditoUtilizado;

                        // Ignora os juros dos cartões ao calcular o valor pago/a pagar
                        totalPago -= UtilsFinanceiro.GetJurosCartoes(transaction, UserInfo.GetUserInfo.IdLoja, valoresReceb, formasPagto, tiposCartao, numParcCartoes);

                        // Verifica se a forma de pagamento foi selecionada, apenas se o crédito não cobrir todo valor da conta com juros
                        if (formasPagto.Length == 0 && Math.Round(creditoUtilizado, 2) < Math.Round(troca.ValorExcedente + juros, 2))
                            throw new Exception("Informe a forma de pagamento.");

                        if (UtilsFinanceiro.ContemFormaPagto(Pagto.FormaPagto.ChequeProprio, formasPagto) && string.IsNullOrEmpty(chequesPagto))
                            throw new Exception("Cadastre o(s) cheque(s) referente(s) ao pagamento da conta.");

                        // Mesmo se for recebimento parcial, não é permitido receber valor maior do que o valor da conta
                        if (recebParcial)
                        {
                            if (totalPago - juros > troca.ValorExcedente)
                                throw new Exception("Valor pago excede o valor da conta.");
                        }
                        // Se o valor for inferior ao que deve ser pago, e o restante do pagto for gerarCredito, lança exceção
                        else if (gerarCredito && Math.Round(totalPago - juros, 2) < Math.Round(troca.ValorExcedente, 2))
                            throw new Exception("Total a ser pago não confere com valor pago. Total a ser pago: " + Math.Round(troca.ValorExcedente + juros, 2).ToString("C") + " Valor pago: " + totalPago.ToString("C"));
                        // Se o total a ser pago for diferente do valor pago, considerando que não é para gerar crédito
                        else if (!gerarCredito && Math.Round(totalPago - juros, 2) != Math.Round(troca.ValorExcedente, 2))
                            throw new Exception("Total a ser pago não confere com valor pago. Total a ser pago: " + Math.Round(troca.ValorExcedente + juros, 2).ToString("C") + " Valor pago: " + totalPago.ToString("C"));

                        uint tipoFunc = UserInfo.GetUserInfo.TipoUsuario;

                        // Se não for caixa diário ou financeiro, não pode receber sinal
                        if (!Config.PossuiPermissao(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) &&
                            !Config.PossuiPermissao(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento))
                            throw new Exception("Você não tem permissão para receber contas.");

                        try
                        {
                            decimal creditoAtual = ClienteDAO.Instance.GetCredito(transaction, troca.IdCliente);

                            retorno = UtilsFinanceiro.Receber(transaction, UserInfo.GetUserInfo.IdLoja, null, null, null, null, null, null, troca, null,
                                null, null, null, troca.IdCliente, 0, null, DateTime.Now.ToString(), troca.ValorExcedente, totalPago, valoresReceb, formasPagto,
                                contasBanco, depositoNaoIdentificado, cartaoNaoIdentificado, tiposCartao, tiposBoleto, txAntecip, juros, recebParcial, gerarCredito, creditoUtilizado, numAutConstrucard,
                                cxDiario, numParcCartoes, chequesPagto, false, UtilsFinanceiro.TipoReceb.TrocaDevolucao);

                            if (retorno.ex != null)
                                throw retorno.ex;

                            #region Cadastra as formas de pagamento

                            int numPagto = 1;

                            for (int i = 0; i < valoresReceb.Length; i++)
                                if (valoresReceb[i] > 0 && formasPagto[i] > 0)
                                {
                                    if (formasPagto.Length > i && formasPagto[i] == (int)Pagto.FormaPagto.CartaoNaoIdentificado)
                                    {
                                        var CNIs = CartaoNaoIdentificadoDAO.Instance.ObterPeloId(cartaoNaoIdentificado);
                                        
                                        foreach (var cni in CNIs)
                                        {
                                            PagtoTrocaDevolucao pagto = new PagtoTrocaDevolucao();
                                            pagto.IdTrocaDevolucao = idTrocaDevolucao;
                                            pagto.NumFormaPagto = numPagto++;
                                            pagto.ValorPago = cni.Valor;
                                            pagto.IdFormaPagto = formasPagto[i];
                                            pagto.IdTipoCartao = (uint)cni.TipoCartao;
                                            pagto.NumAutCartao = cni.NumAutCartao;                                            

                                            PagtoTrocaDevolucaoDAO.Instance.Insert(transaction, pagto);
                                        }
                                    }
                                    else
                                    {
                                        PagtoTrocaDevolucao pagto = new PagtoTrocaDevolucao();
                                        pagto.IdTrocaDevolucao = idTrocaDevolucao;
                                        pagto.NumFormaPagto = numPagto++;
                                        pagto.ValorPago = valoresReceb[i];
                                        pagto.IdFormaPagto = formasPagto[i];
                                        pagto.IdTipoCartao = tiposCartao[i] > 0 ? (uint?)tiposCartao[i] : null;
                                        pagto.NumAutCartao = numAutCartao[i];

                                        PagtoTrocaDevolucaoDAO.Instance.Insert(transaction, pagto);
                                    }
                                }

                            if (creditoUtilizado > 0)
                                PagtoTrocaDevolucaoDAO.Instance.Insert(transaction, new PagtoTrocaDevolucao()
                                {
                                    IdTrocaDevolucao = idTrocaDevolucao,
                                    NumFormaPagto = numPagto++,
                                    ValorPago = creditoUtilizado,
                                    IdFormaPagto = (uint)Pagto.FormaPagto.Credito
                                });

                            #endregion

                            // Altera o estoque
                            AlteraEstoqueFinalizar(transaction, idTrocaDevolucao);

                            // Atualiza a situação
                            objPersistence.ExecuteCommand(transaction, "update troca_devolucao set valorCreditoAoFinalizar=" + creditoAtual.ToString().Replace(",", ".") +
                                ", creditoGeradoFinalizar=" + retorno.creditoGerado.ToString().Replace(",", ".") + ", creditoUtilizadoFinalizar=" +
                                creditoUtilizado.ToString().Replace(",", ".") + ", situacao=" + (int)TrocaDevolucao.SituacaoTrocaDev.Finalizada +
                                " where idTrocaDevolucao=" + idTrocaDevolucao);

                            // Chamado 47351
                            //Se tiver informado as etiquetas de box, volta a situação das mesmas para que possam ser usadas novamente
                            var produtos = ProdutoTrocadoDAO.Instance.GetByTrocaDevolucao(transaction, idTrocaDevolucao);
                            AtualizaEtiquetasBox(transaction, troca, produtos);

                            foreach (var p in produtos.Where(f => !string.IsNullOrEmpty(f.Etiquetas)))
                            {
                                foreach (var etq in p.Etiquetas.Split('|'))
                                {
                                    var idProdPedProducao = etq.Split(';')[1].StrParaUint();
                                    objPersistence.ExecuteCommand(transaction, "UPDATE produto_pedido_producao SET TrocadoDevolvido=1 WHERE IdProdPedProducao=" + idProdPedProducao);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao finalizar troca/devolução.", ex));
                        }

                        msg = "Valor recebido. ";

                        if (retorno != null)
                        {
                            if (retorno.creditoGerado > 0)
                            {
                                //Gera o debito da comissão
                                DebitoComissaoDAO.Instance.AtualizaDebitoPedidoTrocaDevolucao(transaction, (int)troca.IdPedido, troca.IdTrocaDevolucao);

                                msg += "Foi gerado " + retorno.creditoGerado.ToString("C") + " de crédito para o cliente. ";
                            }

                            if (retorno.creditoDebitado)
                                msg += "Foi utilizado " + creditoUtilizado.ToString("C") + " de crédito do cliente, restando " +
                                    ClienteDAO.Instance.GetCredito(transaction, troca.IdCliente).ToString("C") + " de crédito. ";
                        }

                        transaction.Commit();
                        transaction.Close();

                        return msg;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao finalizar troca/devolução.", ex));
                    }
                }
            }
        }

        #endregion

        #region Cancela a troca

        private static readonly object _cancelarLock = new object();

        /// <summary>
        /// Cancela uma troca/devolução.
        /// </summary>
        public void Cancelar(uint idTrocaDevolucao, string obs, DateTime dataEstornoBanco)
        {
            lock(_cancelarLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        TrocaDevolucao trocaDevolucao = GetElement(transaction, idTrocaDevolucao);

                        // Verifica se a troca/devolução pode ser cancelada
                        if (trocaDevolucao.Situacao == (int)TrocaDevolucao.SituacaoTrocaDev.Cancelada)
                            throw new Exception("Troca/devolução já está cancelada.");

                        // Verifica se a troca/devolução possui alguma conta recebida
                        if (ContasReceberDAO.Instance.ExecuteScalar<bool>(transaction,
                            "Select Count(*) From contas_receber Where recebida And idTrocaDevolucao=" + idTrocaDevolucao))
                            throw new Exception("Esta troca/devolução possui uma conta recebida, cancele o recebimento da mesma antes de cancelar esta troca/devolução");

                        var podeCancelar = DebitoComissaoDAO.Instance.VerificaCancelarTrocaDevolucao((uint)trocaDevolucao.IdPedido, trocaDevolucao.IdTrocaDevolucao);

                        if (!podeCancelar)
                            throw new Exception("Não é possível cancelar esta troca/devolução porque o débito de comissão gerado já foi quitado em uma comissão.");

                        DebitoComissaoDAO.Instance.CancelarTrocaDevolucao((uint)trocaDevolucao.IdPedido, trocaDevolucao.IdTrocaDevolucao);

                        var produtos = ProdutoTrocadoDAO.Instance.GetByTrocaDevolucao(transaction, idTrocaDevolucao);

                        if (trocaDevolucao.Situacao == (int)TrocaDevolucao.SituacaoTrocaDev.Finalizada)
                        {
                            foreach (var p in produtos.Where(f => !string.IsNullOrEmpty(f.Etiquetas)))
                            {
                                foreach (var etq in p.Etiquetas.Split('|'))
                                {
                                    var e = etq.Split(';')[0];
                                    var idProdPedProducao = etq.Split(';')[1].StrParaUint();
                                    var situacaoEtq = ProdutoPedidoProducaoDAO.Instance.ObtemSituacaoProducao(transaction, etq);
                                    if (situacaoEtq == SituacaoProdutoProducao.Entregue)
                                        throw new Exception("Não é possivel cancelar essa troca/devolução, pois a etiqueta " + e + " vinculada anteriormente a esse pedido foi entegue em outro pedido.");
                                }
                            }
                        }

                        if (!trocaDevolucao.UsarPedidoReposicao)
                        {
                            try
                            {
                                UtilsFinanceiro.CancelaRecebimento(transaction, UtilsFinanceiro.TipoReceb.TrocaDevolucao, null, null,
                                    null, null, null, 0, null, trocaDevolucao, null, null, dataEstornoBanco, false, false);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Falha ao cancelar troca/devolução.", ex));
                            }
                        }

                        if (trocaDevolucao.Situacao == (int)TrocaDevolucao.SituacaoTrocaDev.Finalizada)
                        {
                            // Altera o estoque
                            AlteraEstoqueCancelar(transaction, idTrocaDevolucao);

                            if (trocaDevolucao.UsarPedidoReposicao)
                            {
                                foreach (var dados in ValorPagtoAntecipadoPedido(transaction, trocaDevolucao))
                                {
                                    objPersistence.ExecuteCommand(transaction, @"update pedido set valorPagamentoAntecipado=
                                        greatest(coalesce(valorPagamentoAntecipado, 0)-?valor, 0) where idPedido=" + dados.Key,
                                        new GDAParameter("?valor", dados.Value));
                                }
                            }

                            foreach (var p in produtos.Where(f => !string.IsNullOrEmpty(f.Etiquetas)))
                            {
                                foreach (var etq in p.Etiquetas.Split('|'))
                                {
                                    var e = etq.Split(';')[0];
                                    var idProdPedProducao = etq.Split(';')[1].StrParaUint();
                                    var idSetor = etq.Split(';')[2].StrParaUint();
                                    var idFuncLeitura = etq.Split(';')[3].StrParaUint();
                                    var dataLeitura = etq.Split(';')[4].StrParaDate();
                                    var idCavalete = etq.Split(';')[5].StrParaInt();
                                    var idItemCarregamento = etq.Split(';')[6].StrParaUint();

                                    var tipoSetor = SetorDAO.Instance.ObtemTipoSetor(idSetor);

                                    ProdutoPedidoProducaoDAO.Instance.AtualizaSituacao(transaction, idFuncLeitura, null, e, idSetor,
                                        false, false, null, null, null, trocaDevolucao.IdPedido, 0, null, null, false, null, true, 0);

                                    var leitura = LeituraProducaoDAO.Instance.ObtemUltimaLeitura(transaction, idProdPedProducao);
                                    objPersistence.ExecuteCommand(transaction, "UPDATE leitura_producao SET DataLeitura= ?dt, IdCavalete = " + idCavalete + " WHERE IdLeituraProd = "
                                        + leitura.IdLeituraProd, new GDAParameter("?dt", dataLeitura));

                                    if (tipoSetor == TipoSetor.ExpCarregamento)
                                        objPersistence.ExecuteCommand(transaction, "UPDATE item_carregamento SET IdProdPedProducao = "+ idProdPedProducao + " WHERE IdItemCarregamento = " + idItemCarregamento);

                                    objPersistence.ExecuteCommand(transaction, "UPDATE produto_pedido_producao SET TrocadoDevolvido=0 WHERE IdProdPedProducao=" + idProdPedProducao);
                                }
                            }
                        }

                        LogCancelamentoDAO.Instance.LogTrocaDevolucao(transaction, trocaDevolucao,
                            obs.Substring(obs.ToLower().IndexOf("motivo do cancelamento: ") + "motivo do cancelamento: ".Length), true);

                        // Atualiza a situação para cancelada
                        trocaDevolucao.Situacao = (int)TrocaDevolucao.SituacaoTrocaDev.Cancelada;
                        trocaDevolucao.Obs = obs;
                        trocaDevolucao.IdFuncCanc = UserInfo.GetUserInfo.CodUser;
                        Update(transaction, trocaDevolucao);

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

        #region Verifica se a troca/devolução é relativa a um pedido

        /// <summary>
        /// Verifica se a troca/devolução é relativa a um pedido.
        /// </summary>
        /// <param name="idTrocaDevolucao"></param>
        /// <returns></returns>
        public bool TemPedido(uint idTrocaDevolucao)
        {
            string sql = "select count(*) from troca_devolucao where idPedido is not null and idTrocaDevolucao=" + idTrocaDevolucao;
            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        #endregion

        #region Verifica se o tipo de uma troca/devolução

        /// <summary>
        /// Verifica se é uma troca.
        /// </summary>
        /// <param name="idTrocaDevolucao"></param>
        /// <returns></returns>
        public bool IsTroca(uint idTrocaDevolucao)
        {
            return objPersistence.ExecuteSqlQueryCount("select count(*) from troca_devolucao where tipo=" +
                (int)TrocaDevolucao.TipoTrocaDev.Troca + " and idTrocaDevolucao=" + idTrocaDevolucao) > 0;
        }

        /// <summary>
        /// Verifica se é uma devolução.
        /// </summary>
        public bool IsDevolucao(uint idTrocaDevolucao)
        {
            return IsDevolucao(null, idTrocaDevolucao);
        }

        /// <summary>
        /// Verifica se é uma devolução.
        /// </summary>
        public bool IsDevolucao(GDASession session, uint idTrocaDevolucao)
        {
            return objPersistence.ExecuteSqlQueryCount(session, "select count(*) from troca_devolucao where tipo=" +
                (int)TrocaDevolucao.TipoTrocaDev.Devolucao + " and idTrocaDevolucao=" + idTrocaDevolucao) > 0;
        }

        #endregion

        #region Atualiza o total da troca/devolução

        /// <summary>
        /// Atualiza os totais da troca/devolução.
        /// </summary>
        /// <param name="idTrocaDevolucao"></param>
        public void UpdateTotaisTrocaDev(uint idTrocaDevolucao)
        {
            UpdateTotaisTrocaDev(null, idTrocaDevolucao);
        }

        /// <summary>
        /// Atualiza os totais da troca/devolução.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idTrocaDevolucao"></param>
        public void UpdateTotaisTrocaDev(GDASession session, uint idTrocaDevolucao)
        {
            UpdateTotaisTrocaDev(session, idTrocaDevolucao, true);
        }

        /// <summary>
        /// Atualiza os totais da troca/devolução.
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idTrocaDevolucao"></param>
        /// <param name="alterarCreditoMaximo"></param>
        public void UpdateTotaisTrocaDev(GDASession session, uint idTrocaDevolucao, bool alterarCreditoGerado)
        {
            string totalTrocado, totalTroca;

            var idPedido = ObtemValorCampo<uint?>(session, "idPedido", "idTrocaDevolucao=" + idTrocaDevolucao);

            var isMoEspecial = idPedido.GetValueOrDefault(0) > 0 &&
                PedidoDAO.Instance.GetTipoPedido(session, idPedido.Value) == Pedido.TipoPedidoEnum.MaoDeObraEspecial;

            /*// Chamado 16807: Mudamos a forma de calcular o crédito gerado para evitar o problema deste chamado, 
            // no qual o usuário inseria um produto, mudava a quantidade dele para menos e em seguida para mais fazendo com que o crédito gerado
            // não fosse recalculado para cima, devido à condição que existia abaixo ", creditoGerado=least(creditoGerado, creditoGeradoMax)"
            if (!isMoEspecial)
                objPersistence.ExecuteCommand(@"
                    update troca_devolucao td
                        set creditoGerado=(
                            Select Sum(total+coalesce(valorBenef,0)) 
                            From produto_trocado pt 
                            Where pt.idTrocaDevolucao=td.idTrocaDevolucao
                        ) 
                    where creditoGeradoMax>0 and idTrocaDevolucao=" + idTrocaDevolucao);*/

            if (!ObtemUsarPedidoReposicao(session, idTrocaDevolucao))
            {
                totalTrocado =
                    @"SELECT COALESCE(SUM(pt.Total + COALESCE(pt.ValorBenef,0)),0)
                    FROM produto_trocado pt
                    WHERE pt.IdTrocaDevolucao=td.IdTrocaDevolucao";

                totalTroca =
                    @"SELECT COALESCE(SUM(ptd.Total + COALESCE(ptd.ValorBenef,0)),0)
                    FROM produto_troca_dev ptd
                    WHERE ptd.IdTrocaDevolucao=td.IdTrocaDevolucao";
            }
            else
            {
                totalTrocado =
                    @"SELECT COALESCE(SUM(ptd.Total + COALESCE(ptd.ValorBenef,0)),0)
                    FROM produto_troca_dev ptd
                    WHERE ptd.IdTrocaDevolucao=td.IdTrocaDevolucao";

                totalTroca = "0";
            }

            var sql =
                @"UPDATE troca_devolucao td
                SET ValorExcedente=GREATEST(0, (" + totalTroca + ")-(" + totalTrocado + ")), " +
                (alterarCreditoGerado ? "CreditoGerado=GREATEST(0, (" + totalTrocado + ")-(" + totalTroca + ")), " : "") +
                    "CreditoGeradoMax=GREATEST(0, (" + totalTrocado + ")-(" + totalTroca + @"))
                WHERE IdTrocaDevolucao=" + idTrocaDevolucao;

            objPersistence.ExecuteCommand(session, sql);
        }

        #endregion

        #region Verifica se a troca/devolução pode ser editada

        /// <summary>
        /// Verifica se a troca/devolução pode ser editada.
        /// </summary>
        /// <param name="idTrocaDevolucao"></param>
        /// <returns></returns>
        public bool PodeEditar(uint idTrocaDevolucao)
        {
            return objPersistence.ExecuteSqlQueryCount("select count(*) from troca_devolucao where situacao=" + (int)TrocaDevolucao.SituacaoTrocaDev.Aberta +
                " and idTrocaDevolucao=" + idTrocaDevolucao) > 0;
        }

        #endregion

        #region Verifica se há trocas/devoluções para um pedido/liberação

        /// <summary>
        /// Verifica se há trocas/devoluções em aberto para um pedido.
        /// </summary>
        public bool ExistsByPedido(uint idPedido)
        {
            return ExistsByPedido(null, idPedido);
        }

        /// <summary>
        /// Verifica se há trocas/devoluções em aberto para um pedido.
        /// </summary>
        public bool ExistsByPedido(GDASession session, uint idPedido)
        {
            return objPersistence.ExecuteSqlQueryCount(session, "select count(*) from troca_devolucao where idPedido=" + idPedido +
                " And situacao<>" + (int)TrocaDevolucao.SituacaoTrocaDev.Cancelada) > 0;
        }

        /// <summary>
        /// Verifica se há trocas/devoluções em aberto para a liberação passada
        /// </summary>
        public bool ExistsByLiberacao(uint idLiberarPedido)
        {
            return ExistsByLiberacao(null, idLiberarPedido);
        }

        /// <summary>
        /// Verifica se há trocas/devoluções em aberto para a liberação passada
        /// </summary>
        public bool ExistsByLiberacao(GDASession session, uint idLiberarPedido)
        {
            return objPersistence.ExecuteSqlQueryCount(session,
                "select count(*) from troca_devolucao where situacao<>" + (int)TrocaDevolucao.SituacaoTrocaDev.Cancelada +
                " And idPedido in (select distinct idPedido from produtos_liberar_pedido where idLiberarPedido=" + idLiberarPedido + ")") > 0;
        }

        #endregion

        #region Obtem valores dos campos

        public uint ObtemIdCliente(uint idTrocaDevolucao)
        {
            return ObtemIdCliente(null, idTrocaDevolucao);
        }

        public uint ObtemIdCliente(GDASession session, uint idTrocaDevolucao)
        {
            return ObtemValorCampo<uint>(session, "idCliente", "idTrocaDevolucao=" + idTrocaDevolucao);
        }

        public int ObtemTipo(uint idTrocaDevolucao)
        {
            return ObtemValorCampo<int>("tipo", "idTrocaDevolucao=" + idTrocaDevolucao);
        }

        public uint ObtemIdTrocaDevolucaoPorPedido(GDASession session, uint idPedido)
        {
            return ObtemValorCampo<uint>("idTrocaDevolucao", "idPedido=" + idPedido + " And situacao<>" + (int)TrocaDevolucao.SituacaoTrocaDev.Cancelada);
        }

        public string ObtemIdTrocaDevPorPedido(uint idPedido)
        {
            return ObtemValorCampo<string>("Cast(group_concat(idTrocaDevolucao) as char)", "idPedido=" + idPedido + " And situacao<>" + (int)TrocaDevolucao.SituacaoTrocaDev.Cancelada);
        }

        public bool ObtemUsarPedidoReposicao(uint idTrocaDevolucao)
        {
            return ObtemUsarPedidoReposicao(null, idTrocaDevolucao);
        }

        public bool ObtemUsarPedidoReposicao(GDASession session, uint idTrocaDevolucao)
        {
            return ObtemValorCampo<bool>(session, "usarPedidoReposicao", "idTrocaDevolucao=" + idTrocaDevolucao);
        }

        public uint? ObtemIdTipoPerda(uint idTrocaDevolucao)
        {
            return ObtemValorCampo<uint?>("IdTipoPerda", "idTrocaDevolucao=" + idTrocaDevolucao);
        }

        public int? ObterIdPedido(GDASession session, int idTrocaDevolucao)
        {
            return ObtemValorCampo<int?>(session, "IdPedido", "IdTrocaDevolucao=" + idTrocaDevolucao);
        }

        public decimal ObterCreditoGerado(GDASession session, uint IdTrocaDevolucao)
        {
            return ObtemValorCampo<decimal>(session, "CreditoGerado", "IdTrocaDevolucao=" + IdTrocaDevolucao);
        }

        #endregion

        #region Métodos sobrescritos

        private static readonly object _insertLock = new object();

        public uint InsertComTransacao(TrocaDevolucao objInsert)
        {
            lock(_insertLock)
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

        public override uint Insert(TrocaDevolucao objInsert)
        {
            return Insert(null, objInsert);
        }

        public override uint Insert(GDASession session, TrocaDevolucao objInsert)
        {
            objInsert.DataTroca = DateTime.Now;
            objInsert.Situacao = (int)TrocaDevolucao.SituacaoTrocaDev.Aberta;
            objInsert.UsuCad = (int)UserInfo.GetUserInfo.CodUser;

            // Se o pedido tiver sido informado mas o cliente não, busca o idCliente.
            if (objInsert.IdPedido > 0 && objInsert.IdCliente == 0)
                objInsert.IdCliente = PedidoDAO.Instance.ObtemIdCliente(session, objInsert.IdPedido.Value);

            if (ClienteDAO.Instance.GetNome(session, objInsert.IdCliente).ToLower().Contains("consumidor"))
                throw new Exception("Não é possível gerar troca/devolução para Consumidor Final.");

            if (objInsert.DataErro != null && objInsert.DataErro.Value.AddDays(+1) < PedidoDAO.Instance.ObtemDataPedido(session, objInsert.IdPedido.Value))
                throw new Exception("A data do erro não pode ser inferior a data do pedido.");

            //Valida se pode realizar a troca de acordo com o prazo maximo permitido
            ValidaDataMaximaTrocaDev(session, objInsert.IdPedido.GetValueOrDefault(0), objInsert.DataTroca);

            return base.Insert(session, objInsert);
        }

        public int UpdateBase(TrocaDevolucao objUpdate)
        {
            return UpdateBase(null, objUpdate);
        }

        public int UpdateBase(GDASession session, TrocaDevolucao objUpdate)
        {
            return base.Update(session, objUpdate);
        }

        public override int Update(TrocaDevolucao objUpdate)
        {
            return Update(null, objUpdate);
        }

        private static readonly object _updateLock = new object();

        public int UpdateComTransacao(TrocaDevolucao objUpdate)
        {
            lock(_updateLock)
            {
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
                }
            }
        }

        public override int Update(GDASession session, TrocaDevolucao objUpdate)
        {
            if (objUpdate.Tipo == (int)TrocaDevolucao.TipoTrocaDev.Devolucao)
                ProdutoTrocaDevolucaoDAO.Instance.DeleteByTrocaDevolucao(session, objUpdate.IdTrocaDevolucao);

            if(objUpdate.CreditoGerado > objUpdate.CreditoGeradoMax)
                throw new Exception("Não é possível gerar crédito maior que o valor dos produtos");

            if (objUpdate.ValorExcedente > 0)
                objUpdate.CreditoGerado = 0;

            // Se o pedido tiver sido informado mas o cliente não, busca o idCliente.
            if (objUpdate.IdPedido > 0 && objUpdate.IdCliente == 0)
                objUpdate.IdCliente = PedidoDAO.Instance.ObtemIdCliente(session, objUpdate.IdPedido.Value);

            if (ClienteDAO.Instance.GetNome(session, objUpdate.IdCliente).ToLower().Contains("consumidor"))
                throw new Exception("Não é possível gerar troca/devolução para Consumidor Final.");

            if (objUpdate.DataErro != null && objUpdate.DataErro.Value.AddDays(+1) < PedidoDAO.Instance.ObtemDataPedido(session, objUpdate.IdPedido.Value))
                throw new Exception("A data do erro não pode ser inferior a data do pedido.");

            //Valida se pode realizar a troca de acordo com o prazo maximo permitido
            ValidaDataMaximaTrocaDev(session, objUpdate.IdPedido.GetValueOrDefault(0), objUpdate.DataTroca);

            LogAlteracaoDAO.Instance.LogTrocaDev(session, objUpdate);

            int retorno = base.Update(session, objUpdate);
            /* Chamado 18064. */
            //UpdateTotaisTrocaDev(session, objUpdate.IdTrocaDevolucao);
            UpdateTotaisTrocaDev(session, objUpdate.IdTrocaDevolucao, false);

            return retorno;
        }

        #endregion

        #region Atualiza etiquetas de box

        /// <summary>
        /// Volta a situação das etiquetas para que possam ser usadas novamente
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="troca"></param>
        /// <param name="produtos"></param>
        private void AtualizaEtiquetasBox(GDATransaction transaction, TrocaDevolucao troca, IList<ProdutoTrocado> produtos)
        {
            foreach (var p in produtos.Where(f => !string.IsNullOrEmpty(f.Etiquetas)))
            {
                var lstEtq = p.Etiquetas.Split('|');

                if (lstEtq.Count() > p.Qtde)
                    throw new Exception("A quantidade de etiquetas informadas é maior que a quantidade selecionada. Insira o produto novamente.");

                foreach (var etq in lstEtq)
                {
                    var idProdPedProducao = etq.Split(';')[1].StrParaUint();
                    var e = etq.Split(';')[0];
                    var tipoSetor = SetorDAO.Instance.ObtemTipoSetor(etq.Split(';')[2].StrParaUint());
                    var idProdPedEtq = ProdutoPedidoProducaoDAO.Instance.ObtemIdProdPed(transaction, idProdPedProducao);
                    var idProdEtq = ProdutosPedidoEspelhoDAO.Instance.ObtemIdProd(transaction, idProdPedEtq);
                    var idPedExp = ProdutoPedidoProducaoDAO.Instance.ObtemIdPedidoExpedicao(idProdPedProducao);
                    var situacaoEtq = ProdutoPedidoProducaoDAO.Instance.ObtemSituacaoProducao(transaction, idProdPedProducao);

                    if (situacaoEtq != SituacaoProdutoProducao.Entregue)
                        throw new Exception("A etiqueta " + e + " ainda não foi entregue. Insira o produto novamente");

                    if (idPedExp.GetValueOrDefault(0) != troca.IdPedido)
                        throw new Exception("A etiqueta " + e + " não esta vinculada ao pedido da troca/devolução. Insira o produto novamente");

                    if (idProdEtq != p.IdProd)
                        throw new Exception("O produto da etiqueta " + e + " é diferente do produto selecionado. Insira o produto novamente");

                    ProdutoPedidoProducaoDAO.Instance.RetiraPecaSituacao(transaction, idProdPedProducao, null, true);

                    if (tipoSetor == TipoSetor.ExpCarregamento)
                    {
                        var idItemCarregamento = ItemCarregamentoDAO.Instance.ObtemIdItemCarregamento(transaction, idProdPedProducao);

                        objPersistence.ExecuteCommand(transaction, "UPDATE item_carregamento SET IdProdPedProducao = null WHERE IdItemCarregamento = " + idItemCarregamento);

                        EstornoItemCarregamentoDAO.Instance.Insert(transaction,
                            new EstornoItemCarregamento()
                            {
                                IdItemCarregamento = idItemCarregamento,
                                Motivo = "Troca/Devolução: " + troca.IdTrocaDevolucao
                            });
                    }
                }
            }
        }

        #endregion

        #region Valida a data maxima permitida para efetuar troca/devolução

        /// <summary>
        /// Valida a data maxima permitida para efetuar troca/devolução
        /// </summary>
        /// <param name="session"></param>
        /// <param name="idPedido"></param>
        /// <param name="dataTroca"></param>
        private void ValidaDataMaximaTrocaDev(GDASession session, uint idPedido, DateTime dataTroca)
        {
            if (!PedidoConfig.LiberarPedido)
                return;

            var tipoPedido = PedidoDAO.Instance.ObterTipoPedido(session, idPedido);
            var prazos = FinanceiroConfig.PrazoMaxDiaUtilRealizarTrocaDev;

            if (!prazos.ContainsKey(tipoPedido))
                return;

            var prazoMaximo = prazos[tipoPedido];

            if (prazoMaximo == 0)
                return;

            var idLiberarPedido = LiberarPedidoDAO.Instance.GetIdsLiberacaoAtivaByPedido(session, idPedido);
            var dataLiberacao = LiberarPedidoDAO.Instance.ObtemDataLiberacao(idLiberarPedido[0]);

            if (dataLiberacao.AddDays(prazoMaximo).Date < dataTroca.Date)
                throw new Exception("O pedido informado ultrapassa a data máxima permitida para troca/devolução");
        }

        #endregion
    }
}
