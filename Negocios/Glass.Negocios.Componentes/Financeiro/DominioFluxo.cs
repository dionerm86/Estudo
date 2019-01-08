using Glass.Configuracoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Financeiro.Negocios.Componentes
{
    public class DominioFluxo : IDominioFluxo
    {
        #region Métodos Privados

        /// <summary>
        /// Aplica o filtro dos tipos de conta.
        /// </summary>
        private void AplicarFiltroTipoConta(
            Colosoft.Query.ConditionalContainer whereClause, string aliasContasReceber, IEnumerable<int> tiposConta)
        {
            if (tiposConta.Any())
            {
                var index = 0;

                var clauses = new Colosoft.Query.ConditionalContainer();
                var tiposConta2 = tiposConta.ToArray();

                foreach (var tipoConta in tiposConta2)
                    clauses
                        .Or(string.Format("({0}.TipoConta & ?tipoConta{1})=?tipoConta{1}", aliasContasReceber, index))
                        .Add(string.Format("?tipoConta{0}", index++), tipoConta);


                whereClause
                    .And(clauses);
            }
        }

        #endregion

        public Entidades.Dominio.Arquivo GerarArquivoRecebidas(int? idPedido, int? idLiberarPedido, int? idAcerto, int? idAcertoParcial, int? idTrocaDevolucao,
            int? numeroNFe, int? idLoja, int? idFunc, int? idFuncRecebido, int? idCli, int? tipoEntrega,
            string nomeCli, DateTime? dtIniVenc, DateTime? dtFimVenc, DateTime? dtIniRec, DateTime? dtFimRec,
            DateTime? dataIniCad, DateTime? dataFimCad,
            int? idFormaPagto, int? idTipoBoleto, decimal? precoInicial, decimal? precoFinal, int? idContaBancoRecebimento, bool? renegociadas,
            bool? recebida, int? idComissionado, int? idRota, string obs, int? ordenacao, IEnumerable<int> tipoContaContabil,
            int? numArqRemessa, bool refObra, int? contasCnab, int? idVendedorAssociado, int? idVendedorObra, int? idComissao, int? numCte,
            bool protestadas, bool contasVinculadas)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                 .From<Data.Model.ContasReceber>("c")
                 .LeftJoin<Data.Model.Pedido>("c.IdPedido=p.IdPedido", "p")
                 .LeftJoin<Data.Model.Cliente>("c.IdCliente=cli.IdCli", "cli")
                 .LeftJoin<Data.Model.PlanoContas>("c.IdConta=pl.IdConta", "pl")
                 .LeftJoin<Data.Model.Funcionario>("c.UsuRec=f.IdFunc", "f")
                 .LeftJoin<Data.Model.AntecipContaRec>("c.IdAntecipContaRec=a.IdAntecipContaRec", "a")
                 .LeftJoin<Data.Model.ComissaoContasReceber>("c.IdContaR = ccr.IdContaR", "ccr")
                 .LeftJoin<Data.Model.Loja>("c.IdLoja = l.IdLoja", "l")
                 .LeftJoin<Data.Model.PlanoContaContabil>("cli.IdContaContabil = pcc.IdContaContabil", "pcc")
                 .Where("c.ValorVec>0 AND (c.IsParcelaCartao=0 OR c.IsParcelaCartao IS NULL)")
                .GroupBy("cr.IdContaR");

            consulta
                .Select(@"replace(replace(replace(replace(l.Cnpj, '.', ''), ' ', ''), '-', ''), '/', '') AS Cnpj, c.DataCad, c.ValorVec as ValorLancamento,
                        pcc.CodInterno as ContaContabilReceber, c.IdContaR, c.Recebida, c.IdLiberarPedido, c.IdAcerto, c.NumParc,
                        c.NumParcMax, c.IdCliente, cli.NomeFantasia AS NomeFantasiaCliente, cli.Nome AS NomeBaseCliente,
                        a.Valor AS ValorAntecipacao, c.DataVec AS Vencimento, a.Data AS DataAntecip, c.DataRec AS Recebimento,
                        a.Situacao AS SituacaoAntecipacao, a.Obs AS ObsAntecipacao, c.IdNf, c.IdAcertoParcial, c.IdAntecipContaRec, c.IdDevolucaoPagto, 
                        c.IdPedido, c.IdObra, c.IdTrocaDevolucao, c.IdSinal, c.Obs,
                        c.IdAcertoCheque, c.IdEncontroContas, pl.Descricao AS PlanoContas, c.MotivoDescontoAcresc, 
                        c.Desconto, c.Acrescimo, c.TipoConta, f.Nome AS Funcionario, c.ValorRec,
                        c.ValorVec, c.Renegociada, c.IsParcelaCartao, p.PercentualComissao,
                        c.NumeroArquivoRemessaCnab, c.Juros, c.Multa, c.DataPrimNeg, c.NumeroDocumentoCnab, c.IdContaR as IdConta");

            if (PedidoConfig.LiberarPedido && FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber)
            {
                consulta.Projection
                    .Add(@"(CASE WHEN cli.PercReducaoNFe < 100 AND c.IdLiberarPedido IS NOT NULL THEN
                        IF(((?subPedidosNfCount)=0), 1, 0) ELSE 0 END)", "LiberacaoNaoPossuiNotaFiscalGerada");

                consulta.WhereClause.Add("?subPedidosNfCount", SourceContext.Instance.CreateQuery()
                    .From<Data.Model.PedidosNotaFiscal>("pnf")
                    .InnerJoin<Data.Model.NotaFiscal>("pnf.IdNf=nf.IdNf", "nf")
                    .Select("Count(IdPedidoNf)")
                    .Where("pnf.IdLiberarPedido=c.IdLiberarPedido AND nf.Situacao NOT IN (?nfCancelada, ?nfDenegada, ?nfInutilizada)")
                    .Add("?nfCancelada", Sync.Fiscal.Enumeracao.NFe.Situacao.Cancelada)
                    .Add("?nfDenegada", Sync.Fiscal.Enumeracao.NFe.Situacao.Denegada)
                    .Add("?nfInutilizada", Sync.Fiscal.Enumeracao.NFe.Situacao.Inutilizada));
            }

            if (recebida.GetValueOrDefault())
                consulta.WhereClause
                    .And("c.Recebida=1");
            else
            {
                if (recebida.HasValue)
                    consulta.WhereClause
                        .And("(c.Recebida=0 OR c.Recebida IS NULL)");

                consulta.WhereClause
                    .And("IdAntecipContaRec IS NULL");
            }

            if (FinanceiroConfig.FinanceiroRec.ExibirCnab)
            {
                var consultaRegArq = SourceContext.Instance.CreateQuery()
                    .Select("rar.IdContaR, MIN(rar.IdContaBanco) AS IdContaBanco, MAX(rar.Protestado) AS protestado")
                    .From<Data.Model.RegistroArquivoRemessa>("rar")
                    .InnerJoin<Data.Model.ArquivoRemessa>("rar.IdArquivoRemessa = ar.IdArquivoRemessa", "ar")
                    .Where("ar.Situacao <> ?situacao")
                    .Add("?situacao", Data.Model.ArquivoRemessa.SituacaoEnum.Cancelado)
                    .GroupBy("IdContaR");

                consulta
                    .LeftJoin(consultaRegArq, "rar.IdContaR=c.IdContaR", "rar");
            }

            if (PedidoConfig.LiberarPedido)
            {
                consulta
                    .LeftJoin<Data.Model.LiberarPedido>("c.IdLiberarPedido=lp.IdLiberarPedido", "lp");

                if (idFunc > 0)
                    consulta
                        .LeftJoin<Data.Model.ProdutosLiberarPedido>("lp.IdLiberarPedido=plp.IdLiberarPedido", "plp")
                        .LeftJoin<Data.Model.Pedido>("plp.IdPedido=plib.IdPedido", "plib");
            }

            if (idPedido > 0)
            {
                var filtro = new Colosoft.Query.ConditionalContainer();

                var pedido = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Pedido>()
                    .Where("IdPedido=?id")
                    .Add("?id", idPedido)
                    .Select("IdPedido, IdSinal, IdPagamentoAntecipado")
                    .Execute()
                    .Select(f => new
                    {
                        IdPedido = f.GetInt32("IdPedido"),
                        IdSinal = (int?)f["IdSinal"],
                        IdPagamentoAntecipado = (int?)f["IdPagamentoAntecipado"]
                    })
                    .FirstOrDefault();

                var idSinal = pedido != null ? pedido.IdSinal : null;
                var idPagamentoAntecipado = pedido != null ? pedido.IdPagamentoAntecipado : null;

                var idsLiberacoes = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.LiberarPedido>("lp")
                    .InnerJoin<Data.Model.ProdutosLiberarPedido>("lp.IdLiberarPedido=plp.IdLiberarPedido", "plp")
                    .Where("lp.Situacao=?liberado AND plp.IdPedido=?idPedido")
                    .Add("?liberado", Glass.Data.Model.LiberarPedido.SituacaoLiberarPedido.Liberado)
                    .Add("?idPedido", idPedido)
                    .SelectDistinct("lp.IdLiberarPedido")
                    .Execute()
                    .Select(f => f.GetInt32(0))
                    .ToList();

                if (recebida.GetValueOrDefault(true))
                {
                    if (PedidoConfig.LiberarPedido)
                    {
                        var consultaAcertos = SourceContext.Instance.CreateQuery()
                            .From<Data.Model.ContasReceber>()
                            .Where("IdAcerto IS NOT NULL")
                            .SelectDistinct("IdAcerto");

                        if (idsLiberacoes.Any())
                            consultaAcertos.WhereClause
                                .And(string.Format("IdLiberarPedido IN ({0})", string.Join(",", idsLiberacoes)));

                        var idsAcerto = consultaAcertos
                            .Execute()
                            .Select(f => f.GetInt32(0))
                            .ToArray();

                        // Os parênteses abaixo devem ficar separados ") )" para que caso passe no TrimEnd(')') logo abaixo remova somente um deles
                        if (idsAcerto.Any())
                        {
                            var container = new Colosoft.Query.ConditionalContainer();
                            if (idsLiberacoes.Any())
                                container
                                    .And(string.Format("IdLiberarPedido IN ({0})", string.Join(",", idsLiberacoes)));

                            container
                                .Or(string.Format("c.IdAcerto IN ({0})", string.Join(",", idsLiberacoes)));

                            filtro.And(container);
                        }
                    }
                    else
                    {
                        var idsAcerto = SourceContext.Instance.CreateQuery()
                            .From<Data.Model.ContasReceber>()
                            .Where("IdAcerto IS NOT NULL AND IdPedido=?idPedido")
                            .Add("?idPedido", idPedido)
                            .SelectDistinct("IdAcerto")
                            .Execute()
                            .Select(f => f.GetInt32(0))
                            .ToArray();

                        // Os parênteses abaixo devem ficar separados ") )" para que caso passe no TrimEnd(')') logo abaixo remova somente um deles
                        if (idsAcerto.Any())
                            filtro
                                .And(string.Format("(c.IdPedido=?idPedido OR c.IdAcerto IN ({0}))", string.Join(",", idsAcerto)));
                    }
                }

                if (idSinal > 0)
                {
                    if (filtro.Any())
                        filtro = Colosoft.Query.ConditionalContainer.Parse("c.IdSinal=?idSinal")
                            .Or(filtro)
                            .Add("?idSinal", idSinal);
                    else
                        filtro
                            .And("c.IdSinal=?idSinal")
                            .Add("?idSinal", idSinal);
                }

                if (idPagamentoAntecipado > 0)
                {
                    if (filtro.Any())
                        filtro = Colosoft.Query.ConditionalContainer.Parse("c.IdSinal=?idPagamentoAntecipado")
                            .Or(filtro)
                            .Add("?idPagamentoAntecipado", idPagamentoAntecipado);
                    else
                        filtro
                            .And("c.IdSinal=?idPagamentoAntecipado")
                            .Add("?idPagamentoAntecipado", idPagamentoAntecipado);
                }

                if (!filtro.Any())
                {
                    if (PedidoConfig.LiberarPedido)
                    {
                        var container = new Colosoft.Query.ConditionalContainer();

                        if (idsLiberacoes.Any())
                            container
                                .And(string.Format("c.IdLiberarPedido IN ({0})", string.Join(",", idsLiberacoes)));

                        container
                            .Or("c.IdLiberarPedido IN ?subPedidosNf1")
                            .Add("?subPedidosNf1", SourceContext.Instance.CreateQuery()
                                .From<Data.Model.PedidosNotaFiscal>()
                                .Where("IdPedido=?idPedido")
                                .SelectDistinct("IdLiberarPedido"))
                            .Or("c.IdNf IN ?subPedidosNf2")
                            .Add("?subPedidosNf2", SourceContext.Instance.CreateQuery()
                                .From<Data.Model.PedidosNotaFiscal>()
                                .Where("IdPedido=?idPedido")
                                .SelectDistinct("IdNf"));

                        consulta.WhereClause
                            .And(container);
                    }
                    else
                        consulta.WhereClause
                            .And("c.IdPedido=?idPedido");
                }
                else
                {
                    consulta.WhereClause
                        .And(filtro
                            .Or("c.IdPedido=?idPedido OR c.IdNf IN ?subPedidosNf")
                            .Add("?subPedidosNf", SourceContext.Instance.CreateQuery()
                                .From<Data.Model.PedidosNotaFiscal>()
                                .Where("IdPedido=?idPedido")
                                .SelectDistinct("IdNf")));
                }

                consulta.WhereClause
                    .Add("?idPedido", idPedido);
            }

            if (idLiberarPedido > 0)
                consulta.WhereClause
                    .And("(c.IdLiberarPedido=?idLiberarPedido OR c.IdNf IN ?subPedidosNfGeral)")
                    .Add("?idLiberarPedido", idLiberarPedido)
                    .Add("?subPedidosNfGeral", SourceContext.Instance.CreateQuery()
                        .From<Data.Model.PedidosNotaFiscal>()
                        .Where("IdLiberarPedido=?idLiberarPedido")
                        .Select("IdNf"));

            if (idAcerto > 0)
                consulta.WhereClause
                    .And("c.IdAcerto=?idAcerto")
                    .Add("?idAcerto", idAcerto);

            if (idAcertoParcial > 0)
                consulta.WhereClause
                    .And("c.IdAcertoParcial=?idAcertoParcial")
                    .Add("?idAcertoParcial", idAcertoParcial);

            if (idTrocaDevolucao > 0)
                consulta.WhereClause
                    .And("c.IdTrocaDevolucao=?idTrocaDevolucao")
                    .Add("?idTrocaDevolucao", idTrocaDevolucao);

            if (contasVinculadas && idCli > 0)
                consulta.WhereClause
                    .And("(c.IdCliente IN ?subClientesVinculo OR c.IdCliente=?idCli)")
                    .Add("?subClientesVinculo", SourceContext.Instance.CreateQuery()
                        .From<Data.Model.ClienteVinculo>()
                        .Where("IdCliente=?idCli")
                        .Select("IdClienteVinculo"));

            else if (idCli > 0)
                consulta.WhereClause
                    .And("c.IdCliente=?idCli")
                    .Add("?idCli", idCli);

            else if (!string.IsNullOrEmpty(nomeCli))
                consulta.WhereClause
                   .And("(cli.Nome LIKE ?nomeCli OR cli.NomeFantasia LIKE ?nomeCli)")
                   .Add("?nomeCli", string.Format("%{0}%", nomeCli));

            if (tipoEntrega > 0)
                consulta.WhereClause
                    .And("c.TipoEntrega=?tipoEntrega")
                    .Add("?tipoEntrega", tipoEntrega);

            if (idLoja > 0)
                consulta.WhereClause
                    .And("c.IdLoja=?idLoja")
                    .Add("?idLoja", idLoja);

            if (idFuncRecebido > 0)
                consulta.WhereClause
                    .And("f.IdFunc=?idFuncRecebido")
                    .Add("?idFuncRecebido", idFuncRecebido);

            if (idComissionado > 0)
            {
                if (PedidoConfig.LiberarPedido)
                    consulta.WhereClause
                        .And("c.IdLiberarPedido IN ?subProdutosLiberarPedidoComissionado")
                        .Add("?subProdutosLiberarPedidoComissionado", SourceContext.Instance.CreateQuery()
                            .From<Data.Model.ProdutosLiberarPedido>("plp1")
                            .InnerJoin<Data.Model.Pedido>("plp1.IdPedido=p1.IdPedido", "p1")
                            .Where("p1.IdComissionado=?idComissionado")
                            .Add("?idComissionado", idComissionado)
                            .SelectDistinct("plp1.IdLiberarPedido"));

                else
                    consulta.WhereClause
                        .And("p.IdComissionado=?idComissionado")
                        .Add("?idComissionado", idComissionado);
            }

            if (idFunc > 0)
            {
                var container = new Colosoft.Query.ConditionalContainer();

                container.And(PedidoConfig.LiberarPedido ? "plib.IdFunc=?idFunc" : "p.IdFunc=?idFunc");

                container
                    .Or("c.IdSinal IN ?subSinal OR c.IdNf IN ?subPedidosNfSinal")
                    .Add("?subSinal", SourceContext.Instance.CreateQuery()
                        .From<Data.Model.Pedido>("p1")
                        .Where("ISNULL(p1.IdSinal, p1.IdPagamentoAntecipado) > 0  AND p1.IdFunc=?idFunc")
                        .Select("ISNULL(p1.IdSinal, p1.IdPagamentoAntecipado)"))
                    .Add("?subPedidosNfSinal", SourceContext.Instance.CreateQuery()
                        .From<Data.Model.PedidosNotaFiscal>("pnf")
                        .InnerJoin<Data.Model.Pedido>("pnf.IdPedido=ped.IdPedido", "ped")
                        .Where("ped.IdFunc=?idFunc")
                        .Select("pnf.IdNf"));
            }

            if (idVendedorObra > 0)
            {
                consulta.WhereClause
                    .And("c.IdObra IN (?fObra)")
                    .Add("?fObra",
                            SourceContext.Instance.CreateQuery()
                            .From<Data.Model.Obra>("o")
                            .Where("o.IdFunc = ?idFunc AND (o.GerarCredito IS NULL OR o.GerarCredito=0)")
                            .Add("?idFunc", idVendedorObra)
                            .SelectDistinct("o.IdObra"));
            }

            if (dtIniVenc.HasValue)
                consulta.WhereClause
                    .And("c.DataVec>=?dtIni")
                    .Add("?dtIni", dtIniVenc.Value.Date)
                    .AddDescription(string.Format("Data venc. início: {0:dd/MM/yyyy}    ", dtIniVenc.Value.Date));

            if (dtFimVenc.HasValue)
                consulta.WhereClause
                    .And("c.DataVec<=?dtFim")
                    .Add("?dtFim", dtFimVenc.Value.Date.AddDays(1).AddSeconds(-1))
                    .AddDescription(string.Format("Data venc. fim: {0:dd/MM/yyyy}    ", dtFimVenc.Value.Date));

            if (dataIniCad.HasValue)
                consulta.WhereClause
                    .And("c.DataCad>=?dataIni")
                    .Add("?dataIni", dataIniCad.Value.Date)
                    .AddDescription(string.Format("Data cad. início: {0:dd/MM/yyyy}    ", dataIniCad.Value.Date));

            if (dataFimCad.HasValue)
                consulta.WhereClause
                    .And("c.DataCad<=?dataFim")
                    .Add("?dataFim", dataFimCad.Value.Date.AddDays(1).AddSeconds(-1))
                    .AddDescription(string.Format("Data cad. fim: {0:dd/MM/yyyy}    ", dataFimCad.Value.Date));

            if (dtIniRec.HasValue)
                consulta.WhereClause
                    .And("c.DataRec>=?dtIniRec")
                    .Add("?dtIniRec", dtIniRec.Value.Date)
                    .AddDescription(string.Format("Data início rec.: {0:dd/MM/yyyy}    ", dtIniRec.Value.Date));

            if (dtFimRec.HasValue)
                consulta.WhereClause
                    .And("c.DataRec<=?dtFimRec")
                    .Add("?dtFimRec", dtFimRec.Value.Date.AddDays(1).AddSeconds(-1))
                    .AddDescription(string.Format("Data fim rec.: {0:dd/MM/yyyy}    ", dtFimRec.Value.Date));

            if (precoInicial > 0)
            {
                if (!recebida.HasValue)
                    consulta.WhereClause
                        .And("ISNULL(c.ValorRec, c.ValorVec)>=?precoInicial");
                else if (recebida.Value)
                    consulta.WhereClause
                        .And("c.ValorRec>=?precoInicial");
                else if (!recebida.Value)
                    consulta.WhereClause
                        .And("c.ValorVec>=?precoInicial");

                consulta.WhereClause
                    .Add("?precoInicial", precoInicial)
                    .AddDescription(string.Format("A partir de: {0:C}     ", precoInicial));
            }

            if (precoFinal > 0)
            {
                if (!recebida.HasValue)
                    consulta.WhereClause
                        .And("ISNULL(c.ValorRec, c.ValorVec)<=?precoFinal");
                else if (recebida.Value)
                    consulta.WhereClause
                        .And("c.ValorRec<=?precoFinal");
                else if (!recebida.Value)
                    consulta.WhereClause
                        .And("c.ValorVec<=?precoFinal");

                consulta.WhereClause
                    .Add("?precoFinal", precoFinal)
                    .AddDescription(string.Format("Até: {0:C}     ", precoFinal));
            }

            if (idContaBancoRecebimento > 0)
            {
                consulta
                    .LeftJoin<Data.Model.PagtoContasReceber>("c.IdContaR = pcr.IdContaR", "pcr");

                consulta.WhereClause
                    .And("pcr.IdContaBanco = ?idContaBancoRecebimento")
                    .Add("?idContaBancoRecebimento", idContaBancoRecebimento);
            }

            if (idFormaPagto > 0)
            {
                var pagtoContasReceber = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.PagtoContasReceber>()
                    .Select("IdContaR")
                    .Where("IdFormaPagto=?id")
                    .Add("?id", idFormaPagto);

                consulta.WhereClause
                    .And("c.IdContaR In (?subConsulta)")
                    .Add("?subConsulta", pagtoContasReceber);
            }

            if (renegociadas.HasValue && (!recebida.HasValue || !recebida.Value))
            {
                if (!renegociadas.Value)
                    consulta.WhereClause
                        .And("(c.Renegociada=0 OR c.Renegociada IS NULL OR c.ValorRec>0)");
                else
                    consulta.WhereClause
                        .And("c.Renegociada=1");
            }
            else if (renegociadas.GetValueOrDefault(false) && recebida.GetValueOrDefault(false))
            {
                consulta.WhereClause
                    .And("c.Renegociada=1");
            }
            else if (renegociadas.HasValue && !renegociadas.Value)
                consulta.WhereClause
                    .And("(c.Renegociada=0 OR c.Renegociada IS NULL OR c.ValorRec>0)");

            if (idRota > 0)
                consulta.WhereClause
                    .And("c.IdCliente IN (?subRotas)")
                    .Add("?subRotas", SourceContext.Instance.CreateQuery()
                        .From<Data.Model.RotaCliente>()
                        .Where("IdRota=?idRota")
                        .Add("?idRota", idRota)
                        .Select("IdCliente"));

            if (!string.IsNullOrEmpty(obs))
                consulta.WhereClause
                    .And("c.Obs LIKE ?obs")
                    .Add("?obs", string.Format("%{0}%", obs));

            if (tipoContaContabil != null && tipoContaContabil.Any())
                AplicarFiltroTipoConta(consulta.WhereClause, "c", tipoContaContabil);

            if (numArqRemessa > 0)
                consulta.WhereClause
                    .And("c.NumeroArquivoRemessaCnab=?numArqRemessa")
                    .Add("?numArqRemessa", numArqRemessa);

            if (!refObra)
                consulta.WhereClause
                    .And("c.IdObra IS NULL")
                    .AddDescription("Sem referência para obra");

            if (idVendedorAssociado > 0)
                consulta.WhereClause
                    .And("cli.IdFunc=?idVendedorAssociado")
                    .Add("?idVendedorAssociado", idVendedorAssociado);


            if (protestadas)
                consulta.Having("ISNULL(MAX(rar.Protestado),0)=1");

            if (idComissao.GetValueOrDefault(0) > 0)
                consulta.WhereClause
                    .And("c.IdContaR IN (?idContaRComissao)")
                    .Add("?idContaRComissao", SourceContext.Instance.CreateQuery()
                            .From<Glass.Data.Model.ComissaoContasReceber>()
                            .Where("IdComissao = " + idComissao.Value)
                            .Select("IdContaR"));

            if (contasCnab > 0)
                switch (contasCnab)
                {
                    case 1:
                        consulta.WhereClause
                            .And("c.NumeroArquivoRemessaCnab IS NULL");
                        break;
                    case 3:
                        consulta.WhereClause
                            .And("c.NumeroArquivoRemessaCnab IS NOT NULL");
                        break;
                }

            if (numCte.GetValueOrDefault(0) > 0)
            {
                consulta.WhereClause
                   .And("c.IdCte IN (?idsCte)")
                       .Add("?idsCte",
                            SourceContext.Instance.CreateQuery()
                            .From<Data.Model.Cte.ConhecimentoTransporte>()
                            .Where("NumeroCte = " + numCte)
                            .Select("IdCte"));
            }

            if (numeroNFe > 0)
            {

                consulta.WhereClause
                    .And("c.IdNF IN ?subPedidosNf10")
                    .Add("?subPedidosNf10", SourceContext.Instance.CreateQuery()
                        .From<Data.Model.PedidosNotaFiscal>("pnf")
                        .InnerJoin<Data.Model.NotaFiscal>("pnf.IdNf=nf.IdNF", "nf")
                        .Where("nf.NumeroNFe=?numeroNFe")
                        .Add("?numeroNFe", numeroNFe)
                        .Select("pnf.IdNf"));
            }

            var registros = consulta.Execute<Entidades.Dominio.Conta>().ToList();

            if (registros.Count == 0)
                return null;

            foreach (var r in registros)
                r.ReferenciaCompleta = ObterReferenciaReceber(r);
            
            var arquivo = new Entidades.Dominio.Arquivo() { Itens = registros.Select(f => (Entidades.Dominio.R6000)f).ToList() };

            return arquivo;
        }

        /// <summary>
        /// Pesquisa contas pagas.
        /// </summary>
        public Entidades.Dominio.Arquivo GerarArquivoPagas(int? idContaPg, int? idCompra, string nf, int? idLoja, int? idCustoFixo, int? idImpostoServ, int? idFornec, string nomeFornec, int? formaPagto,
            DateTime? dataInicioCadastro, DateTime? dataFimCadastro, DateTime? dtIniPago, DateTime? dtFimPago, DateTime? dtIniVenc, DateTime? dtFimVenc, decimal? valorInicial, decimal? valorFinal,
            int? tipo, bool comissao, bool renegociadas, string planoConta, bool custoFixo, bool exibirAPagar, int? idComissao, int? numeroCte, string observacao)
        {
            var nomeFornecBD = Configuracoes.FinanceiroConfig.FinanceiroPagto.ExibirRazaoSocialContasPagarPagas ?
                "f.Razaosocial, ISNULL(f.Nomefantasia" :
                "f.Nomefantasia, ISNULL(f.Razaosocial";

            var nomeFornecComis = string.Format(@"ISNULL({0}, concat(ISNULL(fCom.Nome, ISNULL(cCom.Nome, iCom.Nome)),
                ' - ', CONCAT(YEAR(c.DataCad), '-', MONTH(c.DataCad), '-', DAY(c.DataCad)))))", nomeFornecBD);

            var nomeFornecComisSemData = string.Format("ISNULL({0}, ISNULL(fCom.Nome, ISNULL(cCom.Nome, iCom.Nome))))", nomeFornecBD);

            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ContasPagar>("c")
                .LeftJoin<Data.Model.ImpostoServ>("c.IdImpostoServ=i.IdImpostoServ", "i")
                .LeftJoin<Data.Model.NotaFiscal>("c.IdNf = nf.IdNf", "nf")
                .LeftJoin<Data.Model.Compra>("c.IdCompra = cmp.IdCompra", "cmp")
                .LeftJoin<Data.Model.Fornecedor>("c.IdFornec = f.IdFornec", "f")
                .LeftJoin<Data.Model.PlanoContas>("c.IdConta = pl.IdConta", "pl")
                .LeftJoin<Data.Model.GrupoConta>("pl.IdGrupo = g.IdGrupo", "g")
                .LeftJoin<Data.Model.FormaPagto>("c.IdFormaPagto = fp.IdFormaPagto", "fp")
                .LeftJoin<Data.Model.Pagto>("c.IdPagto = pag.IdPagto", "pag")
                .LeftJoin<Data.Model.Funcionario>("pag.IdFuncPagto = fPag.IdFunc", "fPag")
                .LeftJoin<Data.Model.Comissao>("c.IdComissao = com.IdComissao", "com")
                .LeftJoin<Data.Model.Funcionario>("com.IdFunc = fCom.IdFunc", "fCom")
                .LeftJoin<Data.Model.Comissionado>("com.IdComissionado = cCom.IdComissionado", "cCom")
                .LeftJoin<Data.Model.Funcionario>("com.IdInstalador = iCom.IdFunc", "iCom")
                .LeftJoin<Data.Model.Transportador>("c.IdTransportador = t.IdTransportador", "t")
                .LeftJoin<Data.Model.CustoFixo>("c.IdCustoFixo = cf.IdCustoFixo", "cf")
                .LeftJoin<Data.Model.Loja>("c.IdLoja = l.IdLoja", "l")
                .LeftJoin<Data.Model.PlanoContaContabil>("f.IdContaContabil = pcc.IdContaContabil", "pcc")
                .Select(@"c.IdContaPg, c.IdPagtoRestante, c.IdCompra, c.IdSinalCompra, c.IdPagto, c.IdNf, c.IdImpostoServ, c.IdAntecipFornec, 
                    c.IdEncontroContas, c.IdCte, c.NumBoleto, c.ValorVenc, c.ValorPago, c.DataVenc, c.IdCustoFixo, c.IdFornec,
                    c.IdComissao, c.Paga, c.DataCad, c.DataPagto, c.Juros, c.Multa, c.Renegociada, c.IdTransportador, c.NumParc,
                    c.NumParcMax, c.Obs, ISNULL(t.NomeFantasia, t.Nome) as NomeTransportador, fp.Descricao as DescricaoFormaPagto,
                    cmp.Obs as obsCompra, CONCAT(g.Descricao, ' - ', pl.Descricao) as DescricaoPlanoConta, i.Obs as ObsImpostoServ,
                    fPag.Nome AS NomeFuncionarioPagamento, ?nomeFornecComisSemData as NomeFornecSemData, ?nomeFornecComis as NomeFornec,
                    cf.Descricao as DescricaoCustoFixo, l.Cnpj, c.ValorVenc as ValorLancamento, pcc.CodInterno as ContaContabilReceber, c.IdContaPg as IdConta")
                    .Add("?nomeFornecComisSemData",  nomeFornecComisSemData)
                    .Add("?nomeFornecComis",  nomeFornecComis)
                .OrderBy("c.DataVenc ASC");

            #region Filtros

            if (!exibirAPagar)
                consulta.WhereClause
                    .And("Paga=?paga")
                    .Add("?paga", 1);

            if (idContaPg > 0)
                consulta.WhereClause
                    .And("c.IdContaPg=" + idContaPg);

            if (idCompra > 0)
                consulta.WhereClause
                    .And("c.IdCompra=?idCompra")
                    .Add("?idCompra", idCompra);

            else if (idFornec > 0)
                consulta.WhereClause
                    .And("c.IdFornec=?idFornec")
                    .Add("?idFornec", idFornec);

            else if (!string.IsNullOrEmpty(nomeFornec))
                consulta.WhereClause
                    .And(string.Format("{0} LIKE ?nomeFornec OR f.Razaosocial LIKE ?nomeFornec", nomeFornecComisSemData))
                    .Add("?nomeFornec", string.Format("%{0}%", nomeFornec));

            if (numeroCte.GetValueOrDefault() > 0)
                consulta.WhereClause
                    .And("c.IdCte IN (?sqlIdsCte)")
                    .Add("?sqlIdsCte", SourceContext.Instance.CreateQuery()
                        .From<Data.Model.Cte.ConhecimentoTransporte>()
                        .Select("IdCte")
                        .Where("NumeroCte=?numeroCte")
                        .Add("?numeroCte", numeroCte));

            if (!string.IsNullOrEmpty(nf))
                consulta.WhereClause
                    .And("(cmp.Nf=?nf OR nf.NumeroNfe=?nf OR i.Nf=?nf)")
                    .Add("?nf", nf);

            if (dataInicioCadastro.HasValue)
                consulta.WhereClause
                    .And("c.DataCad>=?dataInicioCadastro")
                    .Add("?dataInicioCadastro", dataInicioCadastro);

            if (dataFimCadastro.HasValue)
                consulta.WhereClause
                    .And("c.DataCad<=?dataFimCadastro")
                    .Add("?dataFimCadastro", dataFimCadastro.Value.AddDays(1).AddSeconds(-1));


            if (dtIniPago.HasValue)
                consulta.WhereClause
                    .And("c.DataPagto>=?dataPagtoIni")
                    .Add("?dataPagtoIni", dtIniPago.Value);

            if (dtFimPago.HasValue)
                consulta.WhereClause
                    .And("c.DataPagto<=?dataPagtoFim")
                    .Add("?dataPagtoFim", dtFimPago.Value);

            if (dtIniVenc.HasValue)
                consulta.WhereClause
                    .And("c.DataVenc>=?dataVencIni")
                    .Add("?dataVencIni", dtIniVenc.Value);

            if (dtFimVenc.HasValue)
                consulta.WhereClause
                    .And("c.DataVenc<=?dataVencFim")
                    .Add("?dataVencFim", dtFimVenc.Value);

            if (idLoja > 0)
                consulta.WhereClause
                    .And("c.IdLoja=?idLoja")
                    .Add("?idLoja", idLoja);

            if (idCustoFixo > 0)
                consulta.WhereClause
                    .And("c.IdCustoFixo=?idCustoFixo")
                    .Add("?idCustoFixo", idCustoFixo);

            if (idImpostoServ > 0)
                consulta.WhereClause
                    .And("c.IdImpostoServ=?idImpostoServ")
                    .Add("?idImpostoServ", idImpostoServ);

            if (formaPagto > 0)
                consulta.WhereClause
                    .And("c.IdFormaPagto=?idFormaPagto")
                    .Add("?idFormaPagto", formaPagto);

            if (valorInicial > 0)
                consulta.WhereClause
                    .And("c.ValorPago>=?valorPagoInicial")
                    .Add("?valorPagoInicial", valorInicial);

            if (valorFinal > 0)
                consulta.WhereClause
                    .And("c.ValorPago<=?valorPagoFinal")
                    .Add("?valorPagoFinal", valorFinal);

            if (tipo == 1)
                consulta.WhereClause
                    .And("c.Contabil = 1");
            else if (tipo > 0)
                consulta.WhereClause
                    .And("(c.Contabil IS NULL OR c.Contabil = 0)");

            if (comissao)
                consulta.WhereClause
                    .And("c.IdComissao IS NOT NULL");

            if (!renegociadas)
                consulta.WhereClause
                    .And("(c.Renegociada IS NULL OR c.Renegociada = 0)");

            if (!string.IsNullOrEmpty(planoConta))
                consulta.WhereClause
                    .And("(pl.Descricao LIKE ?planoConta OR g.Descricao LIKE ?planoConta OR cc.Descricao LIKE ?planoConta)")
                    .Add("?planoConta", string.Format("%{0}%", planoConta));

            if (custoFixo)
                consulta.WhereClause
                    .And("c.IdCustoFixo IS NOT NULL");

            if (idComissao > 0)
                consulta.WhereClause
                    .And("c.IdComissao=?idComissao");
 
            if (!string.IsNullOrEmpty(observacao))
                consulta.WhereClause
                    .And("c.Obs LIKE ?observacao")
                    .Add("?observacao", string.Format("%{0}%", observacao));

            #endregion

            var registros = consulta.Execute<Entidades.Dominio.Conta>().ToList();

            if (registros.Count == 0)
                return null;

            foreach (var r in registros)
                r.ReferenciaCompleta = ObterReferenciaPagar(r);

            var arquivo = new Entidades.Dominio.Arquivo() { Itens = registros.Select(f => (Entidades.Dominio.R6000)f).ToList() };

            return arquivo;
        }

        private string ObterReferenciaReceber(Entidades.Dominio.Conta contaReceber)
        {
            string retorno = "";

            if (contaReceber.IdNf.HasValue)
            {
                var numeroNfe = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.NotaFiscal>()
                    .Where("IdNf=?id")
                    .Add("?id", contaReceber.IdNf)
                    .Select("NumeroNFe")
                    .Execute()
                    .Select(f => f.GetString(0))
                    .FirstOrDefault();

                retorno += string.Format(" NF-e: {0} ", numeroNfe);
            }
            else
            {
                var consulta = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ContasReceber>("cr")
                    .Select("cr.IdContaR, MIN(nfContaReceber.NumeroNFe) AS NumeroNFe")
                    .Where("cr.IdContaR=?idContaR")
                    .Add("?idContaR", contaReceber.IdConta);

                CriarConsultaNumeroNFeContarReceber(consulta, "cr");
                consulta.GroupBy("IdContaR");

                var numeros = consulta.Execute()
                    .Select(f => new
                    {
                        IdContaR = f.GetInt32("IdContaR"),
                        NumeroNFe = f.GetInt32("NumeroNFe")
                    })
                    .ToList();

                if (numeros != null && numeros.Any())
                    retorno += string.Format(" NF-e: {0} ", string.Join(", ", numeros.Select(f => f.NumeroNFe).FirstOrDefault()));
            }

            if (contaReceber.IdAcerto.HasValue)
                retorno += string.Format(" Acerto: {0} ", contaReceber.IdAcerto);

            if (contaReceber.IdAcertoParcial.HasValue)
                retorno += string.Format(" Acerto parcial: {0} ", contaReceber.IdAcertoParcial);

            if (contaReceber.IdAntecipContaRec.HasValue)
                retorno += string.Format(" Antecip. Boleto: {0} ", contaReceber.IdAntecipContaRec);

            if (contaReceber.IdDevolucaoPagto.HasValue)
                retorno += string.Format(" Devolução Pagto: {0} ", contaReceber.IdDevolucaoPagto);

            if (contaReceber.IdLiberarPedido.HasValue)
                retorno += " Liberação" + (contaReceber.IdConta == null && contaReceber.IdConta == 0 &&
                    contaReceber.IdPedido == null ? " " + contaReceber.DescricaoNumeroParcelas + ": " : ": ") +
                    contaReceber.IdLiberarPedido + " ";

            if (contaReceber.IdObra.HasValue)
                retorno += string.Format(" Obra: {0} ", contaReceber.IdObra.Value);

            if (contaReceber.IdPedido.HasValue)
                retorno += string.Format(" Pedido: {0} ", contaReceber.IdPedido);

            if (contaReceber.IdTrocaDevolucao.HasValue)
                retorno += string.Format(" Troca/Devolução: {0} ", contaReceber.IdTrocaDevolucao);

            if (contaReceber.IdSinal.HasValue)
            {
                // Recupera os identificadores dos pedidos associados com o sinal
                var idsPedido = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Pedido>()
                    .Where("IdSinal=?id")
                    .Add("?id", contaReceber.IdSinal)
                    .Select("IdPedido")
                    .Execute()
                    .Select(f => f.GetInt32(0))
                    .ToList();

                //var sinal = SourceContext.Instance.CreateQuery()
                //    .From<Data.Model.Sinal>()
                //    .Where("IdSinal=?idSinal")
                //    .Add("?idSinal", contaReceber.IdSinal)
                //    .ProcessLazyResult<Entidades.Sinal>()
                //    .FirstOrDefault();

                //retorno += string.Format("{0} {1}{2} ", sinal.Referencia, idsPedido.Any() ? "Pedido(s): " : "", string.Join(", ", idsPedido));
            }

            if (contaReceber.IdLiberarPedido > 0 &&
                Glass.Configuracoes.FinanceiroConfig.FinanceiroRec.ExibirIdPedidoComLiberacaoContasRec)
                retorno += string.Format("Pedido(s): {0} ",
                    string.Join(",", SourceContext.Instance.CreateQuery()
                        .From<Data.Model.ProdutosLiberarPedido>()
                        .Where("IdLiberarPedido=?id")
                        .Add("?id", contaReceber.IdLiberarPedido)
                        .Select("IdPedido")
                        .Execute()
                        .Select(f => f.GetInt32(0))));

            //if (contaReceber.NumCheque > 0)
            //    retorno += string.Format("Cheque: {0} ", contaReceber.NumCheque);

            if (contaReceber.IdAcertoCheque.HasValue)
                retorno += string.Format(" Acerto de Cheque: {0} ", contaReceber.IdAcertoCheque);

            if (contaReceber.IdEncontroContas.HasValue)
                retorno += string.Format(" Encontro de Contas a Pagar/Receber: {0} ", contaReceber.IdEncontroContas);

            if (contaReceber.IdCte.HasValue)
            {
                var numeroCte = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Cte.ConhecimentoTransporte>()
                    .Where("IdCte=?id")
                    .Add("?id", contaReceber.IdCte)
                    .Select("NumeroCte")
                    .Execute()
                    .Select(f => f.GetInt32(0))
                    .FirstOrDefault();

                retorno += string.Format("CT-e: {0} ", numeroCte);
            }

            var possuiDepositoNaoIdentificado = SourceContext.Instance.CreateQuery()
                .From<Data.Model.DepositoNaoIdentificado>()
                .Where("IdContaR=?id")
                .Add("?id", contaReceber.IdConta)
                .ExistsResult();

            if (possuiDepositoNaoIdentificado)
                retorno += contaReceber.NomeCliente;

            return retorno;
        }

        /// <summary>
        /// Recupera a referência da conta a pagar.
        /// </summary>
        /// <param name="contaPagar"></param>
        /// <returns></returns>
        private string ObterReferenciaPagar(Entidades.Dominio.Conta contaPagar)
        {
            string refer = string.Empty;

            if (contaPagar.IdCompra > 0)
                refer += string.Format("Compra: {0} ", contaPagar.IdCompra);

            if (contaPagar.IdSinalCompra > 0)
                refer += string.Format("Sinal da Compra: {0} ", contaPagar.IdSinalCompra);

            if (contaPagar.IdPagto > 0)
                refer += string.Format("Pagto: {0} ", contaPagar.IdPagto);

            var numeroNf = contaPagar.IdNf.HasValue ?
                SourceContext.Instance.CreateQuery()
                    .From<Data.Model.NotaFiscal>()
                    .Where("IdNf=?id")
                    .Add("?id", contaPagar.IdNf)
                    .Select("NumeroNFe")
                    .Execute()
                    .Select(f => (int?)f.GetInt32(0))
                    .FirstOrDefault() : null;

            var nf = new Lazy<string>(() =>
                contaPagar.IdCompra.HasValue ?
                    SourceContext.Instance.CreateQuery()
                        .From<Data.Model.Compra>()
                        .Where("IdCompra=?id")
                        .Add("?id", contaPagar.IdCompra)
                        .Select("Nf")
                        .Execute()
                        .Select(f => f.GetString(0))
                        .FirstOrDefault() : null);

            var numNfGeral = numeroNf > 0 ? numeroNf.ToString() : nf.Value;

            if (!string.IsNullOrEmpty(numNfGeral))
                refer += string.Format("NF: {0} ", numNfGeral);

            if (contaPagar.IdCustoFixo > 0)
                refer += string.Format("Custo Fixo: {0} ", contaPagar.IdCustoFixo);

            if (contaPagar.IdImpostoServ > 0)
                refer += string.Format("Imposto/Serv.: {0} ", contaPagar.IdImpostoServ);

            if (contaPagar.IdAntecipFornec > 0)
                refer += string.Format("Antecip. Pagto. Fornecedor: {0} ", contaPagar.IdAntecipFornec);

            if (contaPagar.IdEncontroContas > 0)
                refer += string.Format("Encontro de Contas a Pagar/Receber: {0} ", contaPagar.IdEncontroContas);

            if (contaPagar.IdCreditoFornecedor > 0)
                refer += string.Format("Créd. Fornec.: {0} ", contaPagar.IdCreditoFornecedor);

            if (contaPagar.IdCte > 0)
            {
                var numereCte = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Cte.ConhecimentoTransporte>()
                    .Where("IdCte=?id")
                    .Add("?id", contaPagar.IdCte)
                    .Select("NumeroCte")
                    .Execute()
                    .Select(f => f.GetInt32(0))
                    .FirstOrDefault();

                refer += string.Format("CT-e: {0} ", numereCte);
            }

            if (!string.IsNullOrEmpty(contaPagar.NumBoleto))
                refer += string.Format("Cheque/Boleto: {0} ", contaPagar.NumBoleto);

            if (contaPagar.IdComissao.GetValueOrDefault(0) > 0)
                refer += string.Format("Comissão: {0} ", contaPagar.IdComissao.Value);

            return refer;
        }

        /// <summary>
        /// Cria a consulta para recupera o número da nota fiscal da conta a receber.
        /// </summary>
        /// <param name="contasReceberConsulta"></param>
        /// <param name="aliasContasReceber"></param>
        /// <returns></returns>
        private Colosoft.Query.QueryInfo CriarConsultaNumeroNFeContarReceber(Colosoft.Query.Queryable contasReceberConsulta, string aliasContasReceber)
        {
            var subconsultaNf = SourceContext.Instance.CreateQuery()
                .From<Data.Model.NotaFiscal>("nf")
                .InnerJoin<Data.Model.PedidosNotaFiscal>("nf.IdNf=pnf.IdNf", "pnf");

            if (FinanceiroConfig.SepararValoresFiscaisEReaisContasReceber)
            {
                if (contasReceberConsulta != null)
                    contasReceberConsulta.InnerJoin(
                        subconsultaNf
                            .Select("nf.IdNf, nf.NumeroNFe")
                            .GroupBy("nf.IdNf, nf.NumeroNFe"),
                        string.Format("{0}.IdNf=nfContaReceber.IdNf", aliasContasReceber), "nfContaReceber");
                else
                    subconsultaNf
                        .Select("nf.IdNf, nf.NumeroNFe")
                        .Where(string.Format("{0}.IdNf=nf.IdNf", aliasContasReceber));

            }
            else if (!PedidoConfig.LiberarPedido)
            {
                if (contasReceberConsulta != null)
                    contasReceberConsulta.InnerJoin(
                        subconsultaNf
                            .Select("pnf.IdPedido, nf.NumeroNFe")
                            .GroupBy("pnf.IdPedido, nf.NumeroNFe"),
                        string.Format("{0}.IdPedido=nfContaReceber.IdPedido", aliasContasReceber), "nfContaReceber");
                else
                    subconsultaNf
                        .Select("pnf.IdPedido, nf.NumeroNFe")
                        .Where(string.Format("{0}.IdPedido=pnf.IdPedido", aliasContasReceber));
            }
            else
            {
                subconsultaNf
                    .InnerJoin<Data.Model.ProdutosLiberarPedido>("pnf.IdPedido=plp.IdPedido", "plp");

                if (contasReceberConsulta != null)
                    contasReceberConsulta.InnerJoin(
                        subconsultaNf
                            .Select("plp.IdLiberarPedido, nf.NumeroNFe")
                            .GroupBy("plp.IdLiberarPedido, nf.NumeroNFe"),
                        string.Format("{0}.IdLiberarPedido=nfContaReceber.IdLiberarPedido", aliasContasReceber), "nfContaReceber");
                else
                    subconsultaNf
                        .Select("plp.IdLiberarPedido, nf.NumeroNFe")
                        .Where(string.Format("{0}.IdLiberarPedido=plp.IdLiberarPedido", aliasContasReceber));

            }

            if (contasReceberConsulta != null)
                return contasReceberConsulta.JoinEntities.Last().SubQuery;

            return null;
        }
    }
}
