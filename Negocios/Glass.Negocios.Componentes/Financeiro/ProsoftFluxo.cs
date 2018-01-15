using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Financeiro.Negocios.Entidades.Prosoft;
using Glass.Data.Helper;
using Glass.Data.DAL;

namespace Glass.Financeiro.Negocios.Componentes
{
    public class ProsoftFluxo : IProsoftFluxo
    {
        public Arquivo GerarArquivoPagas(int? idCompra, int? numNfPedido, int? idCustoFixo, int? idImpServ, int? idComissao, decimal? valorPagtoIni, decimal? valorPagtoFim, DateTime? dataVencIni,
            DateTime? dataVencFim, DateTime? dataPagtoIni, DateTime? dataPagtoFim, int? idLoja, int? idFornec, string nomeFornec, int? idFormaPagto, int? idConta, bool jurosMulta, string observacao)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ContasPagar>("cp")
                    .LeftJoin<Data.Model.Fornecedor>("cp.IdFornec = f.IdFornec", "f")
                    .LeftJoin<Data.Model.NotaFiscal>("cp.IdNf = nf.IdNf", "nf")
                    .LeftJoin<Data.Model.Compra>("cp.IdCompra = cmp.IdCompra", "cmp")
                    .LeftJoin<Data.Model.ImpostoServ>("cp.IdImpostoServ = i.IdImpostoServ", "i")
                    .LeftJoin<Data.Model.PlanoContas>("cp.IdConta = pc.IdConta", "pc")
                    .LeftJoin<Data.Model.PlanoContaContabil>("pc.IdContaContabil = pcc.IdContaContabil", "pcc");

            consulta
                .Where("cp.Paga = 1 AND IsNull(cp.Renegociada, 0) = 0")
                .Select("cp.DataPagto as DataLiquidacao, cp.IdContaPg as IdConta, nf.NumeroNFe, f.CpfCnpj, cp.ValorPago as Valor, cp.Obs, " + (int)Item.TipoArquivoEnum.Pagar + @" as TipoArquivo")
                .OrderBy("cp.DataPagto desc");

            #region Filtros

            if (idCompra > 0)
                consulta.WhereClause
                    .And("cp.IdCompra=" + idCompra);

            if (numNfPedido > 0)
                consulta.WhereClause
                   .And("cmp.Nf=?Nf or nf.NumeroNFe=?Nf or i.Nf=?Nf")
                   .Add("Nf", numNfPedido);

            if (idCustoFixo > 0)
                consulta.WhereClause
                    .And("cp.IdCustoFixo=" + idCustoFixo);

            if (idImpServ > 0)
                consulta.WhereClause
                    .And("cp.IdImpostoServ=" + idImpServ);

            if (idComissao > 0)
                consulta.WhereClause
                    .And("cp.IdComissao=" + idComissao);

            if (valorPagtoIni > 0)
                consulta.WhereClause
                    .And("cp.ValorPago >= ?valorPagtoIni")
                    .Add("valorPagtoIni", valorPagtoIni);

            if (valorPagtoFim > 0)
                consulta.WhereClause
                    .And("cp.ValorPago <= ?valorPagtoFim")
                    .Add("valorPagtoFim", valorPagtoIni);

            if (dataVencIni.HasValue)
                consulta.WhereClause
                    .And("cp.DataVenc >= ?dataVencIni")
                    .Add("dataVencIni", dataVencIni.Value.Date);

            if (dataVencFim.HasValue)
                consulta.WhereClause
                    .And("cp.DataVenc <= ?dataVencFim")
                    .Add("dataVencFim", dataVencFim.Value.Date.AddDays(1).AddMinutes(-1));

            if (dataPagtoIni.HasValue)
                consulta.WhereClause
                    .And("cp.DataPagto >= ?dataPagtoIni")
                    .Add("dataPagtoIni", dataPagtoIni.Value.Date);

            if (dataPagtoFim.HasValue)
                consulta.WhereClause
                    .And("cp.DataPagto <= ?dataPagtoFim")
                    .Add("dataPagtoFim", dataPagtoFim.Value.Date.AddDays(1).AddMinutes(-1));

            if (idLoja > 0)
                consulta.WhereClause
                    .And("cp.IdLoja=" + idLoja);

            if (idConta > 0)
                consulta.WhereClause
                    .And("cp.IdConta=" + idConta);

            if (idFormaPagto > 0)
                consulta.WhereClause
                    .And("cp.IdFormaPagto=" + idFormaPagto);

            if (idFornec > 0)
                consulta.WhereClause
                    .And("cp.IdFornec = " + idFornec);
            else if (!string.IsNullOrEmpty(nomeFornec))
                consulta.WhereClause
                    .And("f.Nome LIKE ?nomeFornec")
                    .Add("?nomeFornec", string.Format("%{0}%", nomeFornec));

            if (jurosMulta)
                consulta.WhereClause
                    .And("(cp.Juros > 0 OR cp.Multa > 0)");

            if (!string.IsNullOrEmpty(observacao))
                consulta.WhereClause
                    .And("cp.Observacao LIKE ?observacao")
                    .Add("?observacao", string.Format("%{0}%", observacao));

            #endregion

            var registros = consulta.Execute<Item>().ToList();

            if (registros.Count == 0)
                return null;

            #region Busca as formas pagto.

            var pagtos = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Pagto>("pg")
                    .InnerJoin<Data.Model.ContasPagar>("pg.IdPagto = cp.IdPagto", "cp")
                .Where(string.Format("cp.IdContaPg IN ({0})", string.Join(",", registros.Select(f => f.IdConta.ToString()).ToArray())))
                .Select("cp.IdPagto, cp.IdContaPg, LEAST(cp.ValorPago, cp.ValorVenc) as ValorContaPg, pg.ValorPago, (cp.Juros + cp.Multa) as Juros")
                .Execute()
                .Select(f => new
                {
                    IdPagto = f.GetInt32("IdPagto"),
                    IdContaPg = f.GetInt32("IdContaPg"),
                    ValorContaPg = f.GetDecimal("ValorContaPg"),
                    ValorPagto = f.GetDecimal("ValorPago"),
                    Juros = f.GetDecimal("Juros")
                }).ToList();

            var formaPagtos = SourceContext.Instance.CreateQuery()
                .From<Data.Model.PagtoPagto>("pagto")
                .Where(string.Format("pagto.IdPagto IN ({0})", string.Join(",", pagtos.Select(f => f.IdPagto.ToString()).ToArray())))
                .Select("pagto.IdPagto, pagto.IdFormaPagto, pagto.ValorPagto, pagto.IdContaBanco")
                .Execute()
                .Select(f => new
                {
                    IdPagto = f.GetInt32("IdPagto"),
                    FormaPagto = (Data.Model.Pagto.FormaPagto)f.GetInt32("IdFormaPagto"),
                    ValorPagto = f.GetDecimal("ValorPagto"),
                    CodBanco = f.GetInt32("IdContaBanco")
                }).ToList();

            pagtos = pagtos.Select(f => new
            {
                IdPagto = f.IdPagto,
                IdContaPg = f.IdContaPg,
                ValorContaPg = f.ValorContaPg + f.Juros,
                ValorPagto = f.ValorPagto + formaPagtos.Where(x => x.IdPagto == f.IdPagto
                && x.FormaPagto == Data.Model.Pagto.FormaPagto.Credito).Sum(x => x.ValorPagto),
                Juros = f.Juros
            }).ToList();

            var lstPagtos = new List<Pagtos>();

            foreach (var p in pagtos)
            {
                var formas = formaPagtos.Where(f => f.IdPagto == p.IdPagto).ToList();

                foreach (var f in formas)
                {
                    lstPagtos.Add(new Pagtos()
                    {
                        IdContaPg = p.IdContaPg,
                        FormaPagto = f.FormaPagto,
                        ValorMov = p.ValorContaPg * 100 / p.ValorPagto / 100 * f.ValorPagto,
                        CodBanco = f.CodBanco
                    });
                }
            }

            #endregion

            #region Formas de Pagto.

            var novosItens = new List<Item>();

            for (int i = 0; i < registros.Count; i++)
            {
                var novoItem = registros[i];
                novoItem.TipoContabil = Item.TipoContabilEnum.Outro;

                /* Chamado 37263.
                 * Na versão migrada existe um método no provedor de contas a pagar que recupera a referência da conta. */
                if (novoItem.IdConta > 0)
                    novoItem.Obs = string.Format("‘IMP. DEB N/CTA REF LIQ NF(s) No(s).’ {0}",
                        ContasPagarDAO.Instance.GetElementByPrimaryKey(novoItem.IdConta).Referencia);

                foreach (var pg in lstPagtos.Where(f => f.IdContaPg == registros[i].IdConta))
                {
                    switch (pg.FormaPagto)
                    {
                        case Glass.Data.Model.Pagto.FormaPagto.Dinheiro:
                            {
                                novoItem.Valor = pg.ValorMov;
                                novoItem.TipoContabil = Item.TipoContabilEnum.Dinheiro;
                                break;
                            }
                        case Glass.Data.Model.Pagto.FormaPagto.ChequeProprio:
                        case Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro:
                            {
                                novoItem.Valor = pg.ValorMov;
                                novoItem.TipoContabil = Item.TipoContabilEnum.Cheque;
                                break;
                            }
                        case Glass.Data.Model.Pagto.FormaPagto.Credito:
                            {
                                novoItem.Valor = pg.ValorMov;
                                novoItem.TipoContabil = Item.TipoContabilEnum.Credito;
                                break;
                            }
                        case Glass.Data.Model.Pagto.FormaPagto.Boleto:
                        case Glass.Data.Model.Pagto.FormaPagto.Deposito:
                        case Glass.Data.Model.Pagto.FormaPagto.DepositoNaoIdentificado:
                            {
                                var banco = Glass.Data.DAL.ContaBancoDAO.Instance.GetElement((uint)pg.CodBanco);

                                if (banco == null)
                                {
                                    novoItem.Valor = pg.ValorMov;
                                    novoItem.TipoContabil = Item.TipoContabilEnum.Outro;
                                    break;
                                }

                                if (banco.CodBanco.GetValueOrDefault(0) == 104 && banco.Conta == "2875-6")
                                {
                                    novoItem.Valor = pg.ValorMov;
                                    novoItem.TipoContabil = Item.TipoContabilEnum.Caixa2875;
                                }
                                else if (banco.CodBanco.GetValueOrDefault(0) == 33 && banco.Conta == "13002999-1")
                                {
                                    novoItem.Valor = pg.ValorMov;
                                    novoItem.TipoContabil = Item.TipoContabilEnum.Santander002999;
                                }
                                else if (banco.CodBanco.GetValueOrDefault(0) == 4 && banco.Conta == "29053-4")
                                {
                                    novoItem.Valor = pg.ValorMov;
                                    novoItem.TipoContabil = Item.TipoContabilEnum.BancoNordeste29053;
                                }
                                else if (banco.CodBanco.GetValueOrDefault(0) == 4 && banco.Conta == "030869-7")
                                {
                                    novoItem.Valor = pg.ValorMov;
                                    novoItem.TipoContabil = Item.TipoContabilEnum.BancoNordeste30869;
                                }
                                else if (banco.CodBanco.GetValueOrDefault(0) == 4 && banco.Conta == "031244-9")
                                {
                                    novoItem.Valor = pg.ValorMov;
                                    novoItem.TipoContabil = Item.TipoContabilEnum.BancoNordeste31244;
                                }
                                else if (banco.CodBanco.GetValueOrDefault(0) == 4 && banco.Conta == "037942-0")
                                {
                                    novoItem.Valor = pg.ValorMov;
                                    novoItem.TipoContabil = Item.TipoContabilEnum.BancoNordeste37942;
                                }
                                else
                                {
                                    novoItem.Valor = pg.ValorMov;
                                    novoItem.TipoContabil = Item.TipoContabilEnum.Outro;
                                }

                                break;
                            }
                        default:
                            {
                                novoItem.Valor = pg.ValorMov;
                                novoItem.TipoContabil = Item.TipoContabilEnum.Outro;
                                break;
                            }
                    }
                    novosItens.Add(novoItem);
                }

                if (lstPagtos.Where(f => f.IdContaPg == registros[i].IdConta).Count() == 0)
                    novosItens.Add(novoItem);
            }

            #endregion

            var arquivo = new Arquivo() { Itens = novosItens };

            return arquivo;
        }

        public Arquivo GerarArquivoRecebidas(int? idPedido, int? idLiberarPedido, int? idAcerto, int? idAcertoParcial,
            int? idTrocaDevolucao, int? numeroNFe, int? idLoja, int? idCli, int? idFunc, int? idFuncRecebido, int? tipoEntrega,
            string nomeCli, DateTime? dtIniVenc, DateTime? dtFimVenc, DateTime? dtIniRec, DateTime? dtFimRec, DateTime? dataIniCad,
            DateTime? dataFimCad, int? idFormaPagto, int? idTipoBoleto, decimal? precoInicial, decimal? precoFinal, bool? renegociadas,
            int? idComissionado, int? idRota, string obs, int? numArqRemessa, int? idVendedorObra, bool refObra, int? contasCnab, bool contasVinculadas)
        {
            #region Variaveis Locais

            var planosContaCredito = UtilsPlanoConta.GetLstCredito(3);
            var planosDesconsiderarCxGeral = UtilsPlanoConta.PlanosContaDesconsiderarCxGeral;

            #endregion

            #region Consulta

            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ContasReceber>("cr")
                    .LeftJoin<Data.Model.Cliente>("cr.IdCliente = c.IdCli", "c")
                    .LeftJoin<Data.Model.NotaFiscal>("cr.IdNf = nf.IdNf", "nf")
                    .LeftJoin<Data.Model.Pedido>("cr.IdPedido = p.IdPedido", "p")
                    .LeftJoin<Data.Model.LiberarPedido>("cr.IdLiberarPedido = lp.IdLiberarPedido", "lp");

            if (idFunc > 0)
            {
                consulta
                    .LeftJoin<Data.Model.ProdutosLiberarPedido>("lp.IdLiberarPedido = plp.IdLiberarPedido", "plp")
                    .LeftJoin<Data.Model.Pedido>("plp.IdPedido = plib.IdPedido", "plib");
            }

            consulta
                .Where("cr.Recebida = 1")
                //.Select("cr.IdContaR as IdConta, " + (int)Entidades.GCon.Item.TipoArquivoEnum.Receber + @" as TipoArquivo, cr.DataRec as DataLiquidacao,
                //                cr.ValorRec as Valor, cr.IdCliente, c.Nome as RazaoSocial, c.CpfCnpj, c.RgEscinst as InscEstadual, c.TipoPessoa,
                //                cr.NumParc as Parcela, nf.NumeroNFe, " + (int)Entidades.GCon.Item.TipoRegistroEnum.Recebimento + @" as TipoRegistro,
                //                cr.Desconto, cr.Acrescimo, cr.Juros, cr.IdAcerto, cr.IdSinal")
                .Select("cr.DataRec as DataLiquidacao, cr.IdContaR as IdConta, nf.NumeroNFe, c.CpfCnpj, cr.ValorRec as Valor, cr.Obs, " + (int)Item.TipoArquivoEnum.Receber + @" as TipoArquivo, cr.IdAcerto, cr.IdSinal")
                .OrderBy("cr.DataRec desc");

            #region Filtros

            if (idPedido.GetValueOrDefault(0) > 0)
            {
                var idSinal = SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.Pedido>()
                    .Where("IdPedido = ?idPedido").Add("?idPedido", idPedido)
                    .Select("IsNull(IdSinal, IdPagamentoAntecipado)")
                    .Execute()
                    .Select(f => f.GetInt32(0)).FirstOrDefault();

                var acertos = SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.ContasReceber>()
                    .Where("IdAcerto IS NOT NULL AND IdLiberarPedido IN (?idLp)")
                    .Add("?idLp",
                        SourceContext.Instance.CreateQuery()
                        .From<Glass.Data.Model.ProdutosLiberarPedido>()
                        .Where("IdPedido = ?idPedido").Add("?idPedido", idPedido)
                        .SelectDistinct("IdLiberarPedido"))
                    .SelectDistinct("IdAcerto")
                    .Execute()
                    .Select(f => f.GetString(0)).ToList();

                string filtro = "";

                var fPedido = SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.Pedido>()
                    .Where("IdPedido = ?idPedido")
                    .SelectDistinct("IdLiberarPedido");

                if (acertos.Count > 0)
                    filtro = "(cr.IdLiberarPedido In (?fPedido) Or cr.IdAcerto In (" + String.Join(",", acertos.ToArray()) + ") )";

                if (idSinal > 0)
                    filtro = (String.IsNullOrEmpty(filtro) ? " (" : filtro.TrimEnd(')') + " Or ") + "cr.IdSinal=" + idSinal + ")";

                var fpnf = SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.PedidosNotaFiscal>()
                    .Where("IdPedido = ?idPedido");

                filtro = String.IsNullOrEmpty(filtro) ?
                    @"(cr.IdLiberarPedido In (?fPNf) Or cr.IdNf In (?fPnf2))" : filtro.TrimEnd(')') + " Or cr.IdPedido = ?idPedido Or cr.IdNf In (?fpnf2))";

                consulta.WhereClause
                    .And(filtro)
                    .Add("?fpnf", fpnf.SelectDistinct("IdLiberarPedido"))
                    .Add("?fpnf2", fpnf.SelectDistinct("IdNf"))
                    .Add("?fPedido", fPedido)
                    .Add("?idPedido", idPedido);
            }

            if (idLiberarPedido.GetValueOrDefault(0) > 0)
            {
                consulta.WhereClause
                    .And("(cr.IdLiberarPedido = ?idLiberarPedido OR cr.IdNf IN (?idsNf))")
                    .Add("?idLiberarPedido", idLiberarPedido)
                    .Add("?idsNf",
                            SourceContext.Instance.CreateQuery()
                            .From<Glass.Data.Model.PedidosNotaFiscal>()
                            .Where("IdLiberarPedido = ?idLiberarPedido")
                            .SelectDistinct("IdNf")
                        );
            }

            if (idAcerto > 0)
                consulta.WhereClause
                    .And("cr.IdAcerto=" + idAcerto);

            if (idAcertoParcial > 0)
                consulta.WhereClause
                    .And("cr.IdAcertoParcial=" + idAcertoParcial);

            if (idTrocaDevolucao > 0)
                consulta.WhereClause
                    .And("cr.IdTrocaDevolucao=" + idTrocaDevolucao);

            if (numeroNFe.GetValueOrDefault(0) > 0)
            {
                consulta.WhereClause
                    .And("cr.IdNf IN(?idNf)")
                    .Add("?idNf",
                        SourceContext.Instance.CreateQuery()
                        .From<Glass.Data.Model.NotaFiscal>()
                        .Where("NumeroNFe = ?numNfe").Add("?numNfe", numeroNFe)
                        .SelectDistinct("IdNf"));
            }
            else if (idFunc.GetValueOrDefault(0) > 0)
            {
                consulta
                    .GroupBy("cr.IdContaR");
            }

            if (idLoja > 0)
                consulta.WhereClause
                    .And("cr.IdLoja=" + idLoja);

            if (contasVinculadas && idCli > 0)
                consulta.WhereClause
                    .And("(cr.IdCliente IN ?subClientesVinculo OR cr.IdCliente=?idCli)")
                    .Add("?subClientesVinculo", SourceContext.Instance.CreateQuery()
                        .From<Data.Model.ClienteVinculo>()
                        .Where("IdCliente=?idCli")
                        .Add("?idCli", idCli.Value)
                        .Select("IdClienteVinculo"))
                    .Add("?idCli", idCli.Value);
            if (contasVinculadas && idCli.GetValueOrDefault() > 0)
                consulta.WhereClause
                    .And("cr.IdCliente = ?idCli").Add("?idCli", idCli.Value);
            else if (idCli.GetValueOrDefault() > 0)
                consulta.WhereClause
                    .And("cr.IdCliente = ?idCli").Add("?idCli", idCli.Value);
            else if (!string.IsNullOrEmpty(nomeCli))
            {
                var ids = Glass.Data.DAL.ClienteDAO.Instance.GetIds(null, nomeCli, null, 0, null, null, null, null, 0);
                consulta.WhereClause
                    .And(string.Format("cr.IdCliente IN ({0})", ids));
            }

            if (idFunc.GetValueOrDefault(0) > 0)
            {
                consulta.WhereClause
                    .And(@"(plib.IdFunc = ?idFunc OR cr.IdSinal IN (?fSinal) OR cr.IdNf IN (?fNf))")
                    .Add("?idFunc", idFunc)
                    .Add("?fSinal",
                            SourceContext.Instance.CreateQuery()
                            .From<Glass.Data.Model.Pedido>()
                            .Where("IsNull(IdSinal, IdPagamentoAntecipado) > 0 AND IdFunc = ?idFunc")
                            .Select("IsNull(IdSinal, IdPagamentoAntecipado)"))
                    .Add("?fNf",
                            SourceContext.Instance.CreateQuery()
                            .From<Glass.Data.Model.PedidosNotaFiscal>("pnf1")
                                .LeftJoin<Glass.Data.Model.Pedido>("pnf1.IdPedido = ped1.IdPedido", "ped1")
                            .Where("ped1.IdFunc = ?idFunc")
                            .SelectDistinct("pnf1.IdNf"));
            }

            if (idVendedorObra > 0)
            {
                consulta.WhereClause
                    .And("cr.IdObra IN (?fObra)")
                    .Add("?fObra",
                            SourceContext.Instance.CreateQuery()
                            .From<Data.Model.Obra>("o")
                            .Where("o.IdFunc = ?idFunc AND (o.GerarCredito IS NULL OR o.GerarCredito=0)")
                            .Add("?idFunc", idVendedorObra)
                            .SelectDistinct("o.IdObra"));
            }

            if (tipoEntrega > 0)
                consulta.WhereClause
                    .And("p.TipoEntrega = ?tipoEntrega").Add("?tipoEntrega", tipoEntrega);

            if (idFuncRecebido > 0)
                consulta.WhereClause
                    .And("cr.UsuRec=" + idFuncRecebido);

            if (dtIniVenc.HasValue)
                consulta.WhereClause
                    .And("cr.DataVec >= ?dtVecIni")
                    .Add("?dtVecIni", dtIniVenc.Value);

            if (dtFimVenc.HasValue)
                consulta.WhereClause.
                    And("cr.DataVec <= ?dtVecFim")
                    .Add("?dtVecFim", dtFimVenc.Value.AddHours(23).AddMinutes(59).AddSeconds(59));

            if (dtIniRec.HasValue)
                consulta.WhereClause
                    .And("cr.DataRec >= ?dtRecIni")
                    .Add("?dtRecIni", dtIniRec);

            if (dtFimRec.HasValue)
                consulta.WhereClause.
                    And("cr.DataRec <= ?dtRecFim")
                    .Add("?dtRecFim", dtFimRec.Value.AddHours(23).AddMinutes(59).AddSeconds(59));

            if (dataIniCad.HasValue)
                consulta.WhereClause
                    .And("cr.DataCad >= ?dataCadIni")
                    .Add("?dataCadIni", dataIniCad);

            if (dataFimCad.HasValue)
                consulta.WhereClause.
                    And("cr.DataCad <= ?dataCadFim")
                    .Add("?dataCadFim", dataFimCad.Value.AddHours(23).AddMinutes(59).AddSeconds(59));

            if (idFormaPagto.GetValueOrDefault(0) > 0)
            {
                string idsContas = "";

                if (idFormaPagto.Value == (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto)
                {
                    if (idTipoBoleto.GetValueOrDefault(0) > 0)
                        idsContas = UtilsPlanoConta.GetPlanoRecebTipoBoleto((uint)idTipoBoleto).ToString();
                    else
                        idsContas = UtilsPlanoConta.ContasTodosTiposBoleto();
                }
                else
                    idsContas = UtilsPlanoConta.ContasTodasPorTipo((Glass.Data.Model.Pagto.FormaPagto)idFormaPagto);

                var consAcerto = SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.PagtoAcerto>()
                    .Where("IdFormaPagto = ?idFormaPagto").Add("?idFormaPagto", idFormaPagto)
                    .SelectDistinct("IdAcerto");

                var consAcertoCheque = SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.PagtoAcertoCheque>()
                    .Where("IdFormaPagto = ?idFormaPagto").Add("?idFormaPagto", idFormaPagto)
                    .SelectDistinct("IdAcertoCheque");

                var consLiberarPedido = SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.PagtoLiberarPedido>()
                    .Where("IdFormaPagto = ?idFormaPagto").Add("?idFormaPagto", idFormaPagto)
                    .SelectDistinct("IdLiberarPedido");

                var consObra = SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.PagtoObra>()
                    .Where("IdFormaPagto = ?idFormaPagto").Add("?idFormaPagto", idFormaPagto)
                    .SelectDistinct("IdObra");

                var consSinal = SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.PagtoSinal>()
                    .Where("IdFormaPagto = ?idFormaPagto").Add("?idFormaPagto", idFormaPagto)
                    .SelectDistinct("IdSinal");

                var consTrocaDev = SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.PagtoTrocaDevolucao>()
                    .Where("IdFormaPagto = ?idFormaPagto").Add("?idFormaPagto", idFormaPagto)
                    .SelectDistinct("IdTrocaDevolucao");

                consulta.WhereClause
                    .And(@"cr.IdConta IN (" + idsContas + @") OR (cr.IdAcerto > 0 AND cr.IdAcerto IN (?consAcerto)) OR (cr.IdAcertoCheque > 0 AND cr.IdAcertoCheque IN (?consAcertoCheque)) OR
                            (cr.IdLiberarPedido > 0 AND cr.IdLiberarPedido IN (?consLiberarPedido)) OR (cr.IdObra > 0 AND cr.IdObra IN (?consObra)) OR
                            (cr.IdSinal > 0 AND cr.IdSinal IN (?consSinal)) OR (cr.IdTrocaDevolucao > 0 AND cr.IdTrocaDevolucao IN (?consTrocaDev))")
                   .Add("?consAcerto", consAcerto).Add("?consAcertoCheque", consAcertoCheque).Add("?consLiberarPedido", consLiberarPedido)
                   .Add("?consObra", consObra).Add("?consSinal", consSinal).Add("?consTrocaDev", consTrocaDev);
            }

            if (precoInicial > 0)
                consulta.WhereClause
                        .And("cr.ValorRec >= ?precoInicial").Add("?precoInicial", precoInicial);

            if (precoFinal > 0)
                consulta.WhereClause
                        .And("cr.ValorRec <= ?precoFinal").Add("?precoFinal", precoFinal);

            if (renegociadas != null)
            {
                if (!renegociadas.Value)
                    consulta.WhereClause
                        .And("(IsNull(cr.Renegociada, 0) = 0 OR cr.ValorRec > 0)");
                else
                    consulta.WhereClause
                        .And("cr.Renegociada=1");
            }

            if (idComissionado.GetValueOrDefault(0) > 0)
            {
                consulta.WhereClause
                    .And("cr.IdLiberarPedido IN (?idsComis)")
                    .Add("?idsComis",
                           SourceContext.Instance.CreateQuery()
                           .From<Glass.Data.Model.ProdutosLiberarPedido>()
                           .Where("IdPedido IN (?idsPedidoComis)")
                           .Add("?idsPedidoComis",
                                   SourceContext.Instance.CreateQuery()
                                   .From<Glass.Data.Model.Pedido>()
                                   .Where("IdComissionado = ?idComis").Add("?idComis", idComissionado)
                                   .SelectDistinct("IdPedido")
                               )
                           .SelectDistinct("IdLiberarPedido")
                       );
            }

            if (idRota.GetValueOrDefault(0) > 0)
            {
                consulta.WhereClause
                    .And("c.IdCli IN (?idsCliRota)")
                    .Add("?idsCliRota",
                            SourceContext.Instance.CreateQuery()
                            .From<Glass.Data.Model.RotaCliente>()
                            .Where("IdRota = ?idRota").Add("?idRota", idRota)
                            .SelectDistinct("IdCliente")
                        );
            }

            if (!string.IsNullOrEmpty(obs))
                consulta.WhereClause
                    .And("cr.Obs like ?obs").Add("?obs", "%" + obs + "%");

            if (numArqRemessa > 0)
                consulta.WhereClause
                    .And("cr.NumeroArquivoRemessaCnab=" + numArqRemessa);

            if (!refObra)
                consulta.WhereClause
                    .And("cr.IdObra IS NULL");

            if (contasCnab > 0)
            {
                switch (contasCnab)
                {
                    case 1:
                        consulta.WhereClause
                            .And("cr.NumeroArquivoRemessaCnab IS NULL");
                        break;
                    case 3:
                        consulta.WhereClause
                            .And("cr.NumeroArquivoRemessaCnab IS NOT NULL");
                        break;
                }
            }

            #endregion

            var registros = consulta.Execute<Item>().ToList();

            if (registros.Count == 0)
                return null;

            #region Busca as formas pagto.

            var lstMovBanco = SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.MovBanco>("mv")
                .InnerJoin<Glass.Data.Model.ContaBanco>("mv.IdContaBanco = cb.IdContaBanco", "cb")
                .Where(string.Format("mv.IdContaR IN ({0})", string.Join(",", registros.Select(f => f.IdConta.ToString()).ToArray())))
                .Select("mv.IdConta, mv.IdContaR, mv.ValorMov, mv.Juros, cb.IdContaBanco")
                .Execute()
                .Select(f => new
                {
                    IdConta = f.GetInt32(0),
                    IdContaR = f.GetInt32(1),
                    ValorMov = f.GetDecimal(2),
                    Juros = f.GetDecimal(3),
                    IdContaBanco = f.GetInt32(4)
                }).ToList();

            var lstCxDiario = SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.CaixaDiario>("cx")
                .Where(string.Format("cx.IdContaR IN ({0}) AND cx.IdConta NOT IN ({1}) AND cx.TipoMov = 1", string.Join(",", registros.Select(f => f.IdConta.ToString()).ToArray()), planosDesconsiderarCxGeral))
                .Select("cx.IdConta, cx.IdContaR, cx.Valor, cx.Juros")
                .Execute()
                .Select(f => new
                {
                    IdConta = f.GetInt32(0),
                    IdContaR = f.GetInt32(1),
                    ValorMov = f.GetDecimal(2),
                    Juros = f.GetDecimal(3)
                }).ToList();

            var lstCxGeral = SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.CaixaGeral>("cx")
                .Where(string.Format("cx.IdContaR IN ({0}) AND cx.IdConta NOT IN ({1}) AND cx.TipoMov = 1", string.Join(",", registros.Select(f => f.IdConta.ToString()).ToArray()), planosDesconsiderarCxGeral))
                .Select("cx.IdConta, cx.IdContaR, cx.ValorMov, cx.Juros")
                .Execute()
                .Select(f => new
                {
                    IdConta = f.GetInt32(0),
                    IdContaR = f.GetInt32(1),
                    ValorMov = f.GetDecimal(2),
                    Juros = f.GetDecimal(3)
                }).ToList();

            var lstDepositoNaoIdentificado = SourceContext.Instance.CreateQuery()
                .From<Data.Model.DepositoNaoIdentificado>("dni")
                    .InnerJoin<Data.Model.ContaBanco>("dni.IdContaBanco=cb.IdContaBanco", "cb")
                .Where(string.Format("dni.IdContaR IN ({0})", string.Join(",", registros.Select(f => f.IdConta.ToString()).ToArray())))
                .Select("dni.IdContaR, dni.IdContaBanco, cb.CodBanco, cb.Agencia, dni.ValorMov, dni.IdAcerto, dni.IdSinal")
                .Execute()
                .Select(f => new
                {
                    IdContaR = f.GetInt32(0),
                    IdContaBanco = f.GetInt32(1),
                    CodBanco = f.GetInt32(2),
                    ValorMov = f.GetDecimal(3),
                    IdAcerto = f.GetInt32(4),
                    IdSinal = f.GetInt32(5)
                }).ToList();

            var lstPagtoAcerto = SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.ContasReceber>("cr")
                    .InnerJoin<Glass.Data.Model.Acerto>("a.IdAcerto = cr.IdAcerto", "a")
                    .LeftJoin<Glass.Data.Model.PagtoAcerto>("pa.IdAcerto = a.IdAcerto", "pa")
                    .LeftJoin<Glass.Data.Model.ContaBanco>("pa.IdContaBanco = cb.IdContaBanco", "cb")
                    .LeftJoin
                    (
                        SourceContext.Instance.CreateQuery()
                        .From<Glass.Data.Model.PagtoAcerto>()
                        .GroupBy("IdAcerto")
                        .Select("IdAcerto, SUM(ValorPagto) as TotalVec"),
                        "tmp.IdAcerto = a.IdAcerto", "tmp"
                    )
                .Where(string.Format("cr.IdContaR IN ({0})", string.Join(",", registros.Select(f => f.IdConta.ToString()).ToArray())))
                .Select("pa.IdAcerto as Id, pa.IdFormaPagto as FormaPagto, pa.ValorPagto as Valor, cb.IdContaBanco, tmp.TotalVec as Total, a.CreditoGeradoCriar as CreditoGerado")
                .GroupBy("a.IdAcerto, pa.IdFormaPagto, pa.ValorPagto, pa.IdContaBanco")
                .Execute<Pgto>().ToList();

            var lstPagtoSinal = SourceContext.Instance.CreateQuery()
                .From<Data.Model.ContasReceber>("cr")
                    .InnerJoin<Data.Model.Sinal>("s.IdSinal = cr.IdSinal", "s")
                    .LeftJoin<Data.Model.PagtoSinal>("ps.IdSinal = s.IdSinal", "ps")
                    .LeftJoin<Data.Model.ContaBanco>("ps.IdContaBanco = cb.IdContaBanco", "cb")
                    .LeftJoin
                    (
                        SourceContext.Instance.CreateQuery()
                        .From<Data.Model.PagtoSinal>()
                        .GroupBy("IdSinal")
                        .Select("IdSinal, SUM(ValorPagto) as TotalVec"),
                        "tmp.IdSinal = s.IdSinal", "tmp"
                    )
                .Where(string.Format("cr.IdContaR IN ({0})", string.Join(",", registros.Select(f => f.IdConta.ToString()).ToArray())))
                .Select("ps.IdSinal as Id, ps.IdFormaPagto as FormaPagto, ps.ValorPagto as Valor, cb.IdContaBanco, tmp.TotalVec as Total, s.CreditoGeradoCriar as CreditoGerado")
                .GroupBy("s.IdSinal, ps.IdFormaPagto, ps.ValorPagto, ps.IdContaBanco")
                .Execute<Pgto>().ToList();

            #endregion

            #endregion

            #region Formas de Pagto.

            var novosItens = new List<Item>();

            foreach (var item in registros)
            {
                #region Acerto

                if (item.IdAcerto.GetValueOrDefault(0) > 0)
                {
                    var pagtosAcertos = lstPagtoAcerto.Where(f => f.Id == item.IdAcerto).Select(f => (Pgto)f.Clone()).ToList();

                    if (pagtosAcertos.Count > 0)
                    {
                        if (pagtosAcertos[0].CreditoGerado > 0)
                        {
                            var percCreditoGerado = pagtosAcertos[0].CreditoGerado * 100 / pagtosAcertos[0].Total;
                            foreach (var a in pagtosAcertos)
                                a.Valor -= a.Valor * percCreditoGerado / 100;
                        }

                        var totalAcerto = pagtosAcertos.Sum(f => f.Valor);
                        var percConta = item.Valor * 100 / totalAcerto;

                        foreach (var a in pagtosAcertos)
                        {
                            var valorRec = a.Valor * percConta / 100;

                            if (valorRec <= 0)
                                continue;

                            var novoItem = (Item)item.Clone();
                            novoItem.TipoContabil = Item.TipoContabilEnum.Outro;
                            item.Valor = valorRec;

                            if (a.FormaPagto == Data.Model.Pagto.FormaPagto.Dinheiro)
                                novoItem.TipoContabil = Item.TipoContabilEnum.Dinheiro;
                            else if (a.FormaPagto == Data.Model.Pagto.FormaPagto.ChequeProprio || a.FormaPagto == Data.Model.Pagto.FormaPagto.ChequeTerceiro)
                                novoItem.TipoContabil = Item.TipoContabilEnum.Dinheiro;
                            else if (a.FormaPagto == Data.Model.Pagto.FormaPagto.Credito)
                                novoItem.TipoContabil = Item.TipoContabilEnum.Credito;
                            else if (a.FormaPagto == Data.Model.Pagto.FormaPagto.Cartao)
                                novoItem.TipoContabil = Item.TipoContabilEnum.Cartao;
                            else if (a.FormaPagto == Glass.Data.Model.Pagto.FormaPagto.Boleto || a.FormaPagto == Glass.Data.Model.Pagto.FormaPagto.Deposito ||
                                a.FormaPagto == Glass.Data.Model.Pagto.FormaPagto.DepositoNaoIdentificado)
                            {
                                var banco = Data.DAL.ContaBancoDAO.Instance.GetElement((uint)a.IdContaBanco);

                                var codigoBanco = banco.CodBanco;
                                var contaBanco = banco.Conta;

                                if (codigoBanco == 0)
                                {
                                    var dadosBanco =
                                        SourceContext.Instance.CreateQuery()
                                            .From<Data.Model.DepositoNaoIdentificado>("dni")
                                            .InnerJoin<Data.Model.ContaBanco>("dni.IdContaBanco=cb.IdContaBanco", "cb")
                                            .Where("dni.IdAcerto = ?idAcerto AND dni.ValorMov = ?valorMov")
                                                .Add("?idAcerto", item.IdAcerto.Value)
                                                .Add("?valorMov", a.Valor)
                                            .Select("CONCAT(cb.CodBanco, ';', cb.Conta)")
                                            .Execute()
                                            .FirstOrDefault()
                                            .GetString(0)
                                            .Split(';');

                                    codigoBanco = dadosBanco[0].StrParaInt();
                                    contaBanco = dadosBanco[1];
                                }

                                if (codigoBanco == 104 && contaBanco == "2875-6")
                                    novoItem.TipoContabil = Item.TipoContabilEnum.Caixa2875;
                                else if (codigoBanco == 33 && contaBanco == "13002999-1")
                                    novoItem.TipoContabil = Item.TipoContabilEnum.Santander002999;
                                else if (codigoBanco == 4 && contaBanco == "29053-4")
                                    novoItem.TipoContabil = Item.TipoContabilEnum.BancoNordeste29053;
                                else if (codigoBanco == 4 && contaBanco == "030869-7")
                                    novoItem.TipoContabil = Item.TipoContabilEnum.BancoNordeste30869;
                                else if (codigoBanco == 4 && contaBanco == "031244-9")
                                    novoItem.TipoContabil = Item.TipoContabilEnum.BancoNordeste31244;
                                else if (codigoBanco == 4 && contaBanco == "037942-0")
                                    novoItem.TipoContabil = Item.TipoContabilEnum.BancoNordeste37942;
                            }
                            novosItens.Add(novoItem);
                        }
                    }
                }

                #endregion

                #region Sinal

                else if (item.IdSinal.GetValueOrDefault(0) > 0)
                {
                    var pagtosSinal = lstPagtoSinal.Where(f => f.Id == item.IdSinal).Select(f => (Pgto)f.Clone()).ToList();

                    if (pagtosSinal.Count > 0)
                    {
                        if (pagtosSinal[0].CreditoGerado > 0)
                        {
                            var percCreditoGerado = pagtosSinal[0].CreditoGerado * 100 / pagtosSinal[0].Total;
                            foreach (var a in pagtosSinal)
                                a.Valor -= a.Valor * percCreditoGerado / 100;
                        }

                        var totalAcerto = pagtosSinal.Sum(f => f.Valor);
                        var percConta = item.Valor * 100 / totalAcerto;

                        foreach (var a in pagtosSinal)
                        {
                            var valorRec = a.Valor * percConta / 100;

                            if (valorRec <= 0)
                                continue;

                            var novoItem = (Item)item.Clone();
                            novoItem.TipoContabil = Item.TipoContabilEnum.Outro;
                            novoItem.Valor = valorRec;

                            if (a.FormaPagto == Data.Model.Pagto.FormaPagto.Dinheiro)
                                novoItem.TipoContabil = Item.TipoContabilEnum.Dinheiro;
                            else if (a.FormaPagto == Data.Model.Pagto.FormaPagto.ChequeProprio || a.FormaPagto == Data.Model.Pagto.FormaPagto.ChequeTerceiro)
                                novoItem.TipoContabil = Item.TipoContabilEnum.Dinheiro;
                            else if (a.FormaPagto == Data.Model.Pagto.FormaPagto.Credito)
                                novoItem.TipoContabil = Item.TipoContabilEnum.Credito;
                            else if (a.FormaPagto == Data.Model.Pagto.FormaPagto.Cartao)
                                novoItem.TipoContabil = Item.TipoContabilEnum.Cartao;
                            else if (a.FormaPagto == Glass.Data.Model.Pagto.FormaPagto.Boleto || a.FormaPagto == Glass.Data.Model.Pagto.FormaPagto.Deposito ||
                                a.FormaPagto == Glass.Data.Model.Pagto.FormaPagto.DepositoNaoIdentificado)
                            {
                                var banco = Data.DAL.ContaBancoDAO.Instance.GetElement((uint)a.IdContaBanco);

                                if (banco == null)
                                    continue;

                                if (banco.CodBanco.GetValueOrDefault(0) == 104 && banco.Conta == "2875-6")
                                    novoItem.TipoContabil = Item.TipoContabilEnum.Caixa2875;
                                else if (banco.CodBanco.GetValueOrDefault(0) == 33 && banco.Conta == "13002999-1")
                                    novoItem.TipoContabil = Item.TipoContabilEnum.Santander002999;
                                else if (banco.CodBanco.GetValueOrDefault(0) == 4 && banco.Conta == "29053-4")
                                    novoItem.TipoContabil = Item.TipoContabilEnum.BancoNordeste29053;
                                else if (banco.CodBanco.GetValueOrDefault(0) == 4 && banco.Conta == "030869-7")
                                    novoItem.TipoContabil = Item.TipoContabilEnum.BancoNordeste30869;
                                else if (banco.CodBanco.GetValueOrDefault(0) == 4 && banco.Conta == "031244-9")
                                    novoItem.TipoContabil = Item.TipoContabilEnum.BancoNordeste31244;
                                else if (banco.CodBanco.GetValueOrDefault(0) == 4 && banco.Conta == "037942-0")
                                    novoItem.TipoContabil = Item.TipoContabilEnum.BancoNordeste37942;
                            }

                            novosItens.Add(novoItem);
                        }
                    }
                }

                #endregion

                #region Cx Diario / Cx Geral / Mov Banco

                else
                {
                    var cxGeral = lstCxGeral.Where(f => f.IdContaR == item.IdConta).ToList();
                    var cxDiario = lstCxDiario.Where(f => f.IdContaR == item.IdConta).ToList();
                    var mbs = lstMovBanco.Where(f => f.IdContaR == item.IdConta).ToList();
                    var depositosNaoIdentificados = lstDepositoNaoIdentificado.Where(f => f.IdContaR == item.IdConta).ToList();

                    #region Cx Diario

                    var novoItem = (Item)item.Clone();
                    novoItem.TipoContabil = Item.TipoContabilEnum.Outro;

                    foreach (var cxG in cxGeral)
                    {
                        var formaPgto = UtilsPlanoConta.GetFormaPagtoByIdConta(Convert.ToUInt32(cxG.IdConta));
                        var idFPagto = formaPgto != null ? formaPgto.IdFormaPagto.GetValueOrDefault(0) : 0;

                        if (cxG.ValorMov <= 0)
                            continue;

                        novoItem.Valor = cxG.ValorMov;

                        if (idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro)
                            novoItem.TipoContabil = Item.TipoContabilEnum.Dinheiro;
                        else if(idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio || idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro)
                            novoItem.TipoContabil = Item.TipoContabilEnum.Dinheiro;
                        else if (planosContaCredito.Contains(Convert.ToUInt32(cxG.IdConta)))
                            novoItem.TipoContabil = Item.TipoContabilEnum.Credito;
                        else if (idFPagto == (uint)Data.Model.Pagto.FormaPagto.Cartao)
                            novoItem.TipoContabil = Item.TipoContabilEnum.Cartao;

                        novosItens.Add(novoItem);
                    }

                    #endregion

                    #region Cx Geral

                    novoItem = (Item)item.Clone();
                    novoItem.TipoContabil = Item.TipoContabilEnum.Outro;

                    foreach (var cxD in cxDiario)
                    {
                        var formaPgto = UtilsPlanoConta.GetFormaPagtoByIdConta(Convert.ToUInt32(cxD.IdConta));
                        var idFPagto = formaPgto != null ? formaPgto.IdFormaPagto.GetValueOrDefault(0) : 0;

                        if (cxD.ValorMov <= 0)
                            continue;

                        novoItem.Valor = cxD.ValorMov;

                        if (idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro)
                            novoItem.TipoContabil = Item.TipoContabilEnum.Dinheiro;
                        else if (idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio || idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro)
                            novoItem.TipoContabil = Item.TipoContabilEnum.Dinheiro;
                        else if (planosContaCredito.Contains(Convert.ToUInt32(cxD.IdConta)))
                            novoItem.TipoContabil = Item.TipoContabilEnum.Credito;
                        else if (idFPagto == (uint)Data.Model.Pagto.FormaPagto.Cartao)
                            novoItem.TipoContabil = Item.TipoContabilEnum.Cartao;

                        novosItens.Add(novoItem);
                    }

                    #endregion

                    #region Mov Banco

                    novoItem = (Item)item.Clone();
                    novoItem.TipoContabil = Item.TipoContabilEnum.Outro;

                    foreach (var mb in mbs)
                    {
                        var formaPgto = UtilsPlanoConta.GetFormaPagtoByIdConta(Convert.ToUInt32(mb.IdConta));
                        var idFPagto = formaPgto != null ? formaPgto.IdFormaPagto.GetValueOrDefault(0) : 0;

                        var juros = mb.Juros > 0 ? mb.Juros / mbs.Count : 0;
                        var valor = mb.ValorMov - juros;

                        if (valor <= 0)
                            continue;

                        novoItem.Valor = valor;

                        if (idFPagto == (uint)Data.Model.Pagto.FormaPagto.Boleto || idFPagto == (uint)Data.Model.Pagto.FormaPagto.Deposito || idFPagto == (uint)Data.Model.Pagto.FormaPagto.DepositoNaoIdentificado)
                        {
                            var banco = Data.DAL.ContaBancoDAO.Instance.GetElement((uint)mb.IdContaBanco);

                            if (banco == null)
                                continue;

                            if (banco.CodBanco.GetValueOrDefault(0) == 104 && banco.Conta == "2875-6")
                                novoItem.TipoContabil = Item.TipoContabilEnum.Caixa2875;
                            else if (banco.CodBanco.GetValueOrDefault(0) == 33 && banco.Conta == "13002999-1")
                                novoItem.TipoContabil = Item.TipoContabilEnum.Santander002999;
                            else if (banco.CodBanco.GetValueOrDefault(0) == 4 && banco.Conta == "29053-4")
                                novoItem.TipoContabil = Item.TipoContabilEnum.BancoNordeste29053;
                            else if (banco.CodBanco.GetValueOrDefault(0) == 4 && banco.Conta == "030869-7")
                                novoItem.TipoContabil = Item.TipoContabilEnum.BancoNordeste30869;
                            else if (banco.CodBanco.GetValueOrDefault(0) == 4 && banco.Conta == "031244-9")
                                novoItem.TipoContabil = Item.TipoContabilEnum.BancoNordeste31244;
                            else if (banco.CodBanco.GetValueOrDefault(0) == 4 && banco.Conta == "037942-0")
                                novoItem.TipoContabil = Item.TipoContabilEnum.BancoNordeste37942;
                        }

                        novosItens.Add(novoItem);
                    }

                    #endregion

                    #region Dep. Não Identificado

                    novoItem = (Item)item.Clone();
                    novoItem.TipoContabil = Item.TipoContabilEnum.Outro;

                    foreach (var dni in depositosNaoIdentificados)
                    {
                        if (dni.ValorMov <= 0)
                            continue;

                        novoItem.Valor = dni.ValorMov;

                        var banco = Data.DAL.ContaBancoDAO.Instance.GetElement((uint)dni.IdContaBanco);

                        if (banco == null)
                            continue;

                        if (banco.CodBanco.GetValueOrDefault(0) == 104 && banco.Conta == "2875-6")
                            novoItem.TipoContabil = Item.TipoContabilEnum.Caixa2875;
                        else if (banco.CodBanco.GetValueOrDefault(0) == 33 && banco.Conta == "13002999-1")
                            novoItem.TipoContabil = Item.TipoContabilEnum.Santander002999;
                        else if (banco.CodBanco.GetValueOrDefault(0) == 4 && banco.Conta == "29053-4")
                            novoItem.TipoContabil = Item.TipoContabilEnum.BancoNordeste29053;
                        else if (banco.CodBanco.GetValueOrDefault(0) == 4 && banco.Conta == "030869-7")
                            novoItem.TipoContabil = Item.TipoContabilEnum.BancoNordeste30869;
                        else if (banco.CodBanco.GetValueOrDefault(0) == 4 && banco.Conta == "031244-9")
                            novoItem.TipoContabil = Item.TipoContabilEnum.BancoNordeste31244;
                        else if (banco.CodBanco.GetValueOrDefault(0) == 4 && banco.Conta == "037942-0")
                            novoItem.TipoContabil = Item.TipoContabilEnum.BancoNordeste37942;

                        novosItens.Add(novoItem);
                    }

                    #endregion
                }

                #endregion
            }

            #endregion

            var arquivo = new Arquivo() { Itens = novosItens };

            return arquivo;
        }
    }
}
