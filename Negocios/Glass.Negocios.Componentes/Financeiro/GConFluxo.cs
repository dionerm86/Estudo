using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Helper;

namespace Glass.Financeiro.Negocios.Componentes
{
    internal class Pgto : ICloneable
    {
        public int Id { get; set; }

        public int CodBanco { get; set; }

        public string Conta { get; set; }

        public Glass.Data.Model.Pagto.FormaPagto FormaPagto { get; set; }

        public decimal Valor { get; set; }

        public decimal Total { get; set; }

        public decimal CreditoGerado { get; set; }

        public int IdContaBanco { get; set; }

        public object Clone()
        {
           return (Pgto)this.MemberwiseClone();
        }
    }

    internal class Pagtos
    {
        public int IdContaPg { get; set; }

        public Data.Model.Pagto.FormaPagto FormaPagto { get; set; }

        public decimal ValorMov { get; set; }

        public int CodBanco { get; set; }
    }

    public class GConFluxo : IGConFluxo
    {
        public Entidades.GCon.Arquivo GerarArquivoRecebidas(int? idPedido, int? idLiberarPedido, int? idAcerto, int? idAcertoParcial, int? idTrocaDevolucao, int? numeroNFe,
            int? idLoja, int? idCli, int? idFunc, int? idFuncRecebido, int? tipoEntrega, string nomeCli, DateTime? dtIniVenc, DateTime? dtFimVenc, DateTime? dtIniRec,
            DateTime? dtFimRec, DateTime? dataIniCad, DateTime? dataFimCad, int? idFormaPagto, int? idTipoBoleto, decimal? precoInicial, decimal? precoFinal, int? idContaBancoRecebimento, bool? renegociadas, int? idComissionado,
            int? idRota, string obs, int? numArqRemessa, int? idVendedorObra, bool refObra, int? contasCnab, bool contasVinculadas)
        {
            #region Variaveis Locais

            var planosContaCredito = UtilsPlanoConta.GetLstCredito(3);
            var planosContaEstornoCredito = UtilsPlanoConta.GetLstCredito(4);
            var planosDesconsiderarCxGeral = UtilsPlanoConta.PlanosContaDesconsiderarCxGeral;

            #endregion

            #region Consulta

            var consulta = SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.ContasReceber>("cr")
                    .LeftJoin<Glass.Data.Model.Cliente>("cr.IdCliente = c.IdCli", "c")
                    .LeftJoin<Glass.Data.Model.NotaFiscal>("cr.IdNf = nf.IdNf", "nf")
                    .LeftJoin<Glass.Data.Model.Pedido>("cr.IdPedido = p.IdPedido", "p")
                    .LeftJoin<Glass.Data.Model.LiberarPedido>("cr.IdLiberarPedido = lp.IdLiberarPedido", "lp")
                .GroupBy("cr.IdContaR");

            if (idFunc > 0)
            {
                consulta
                    .LeftJoin<Glass.Data.Model.ProdutosLiberarPedido>("lp.IdLiberarPedido = plp.IdLiberarPedido", "plp")
                    .LeftJoin<Glass.Data.Model.Pedido>("plp.IdPedido = plib.IdPedido", "plib");
            }

            consulta
                .Where("cr.Recebida = 1 AND (cr.TipoConta & ?tipoConta) = ?tipoConta").Add("?tipoConta", (byte)Glass.Data.Model.ContasReceber.TipoContaEnum.Contabil)
                .Select("cr.IdContaR as IdConta, " + (int)Entidades.GCon.Item.TipoArquivoEnum.Receber + @" as TipoArquivo, cr.DataRec as DataLiquidacao,
                            cr.ValorRec as Valor, cr.IdCliente, c.Nome as RazaoSocial, c.CpfCnpj, c.RgEscinst as InscEstadual, c.TipoPessoa,
                            cr.NumParc as Parcela, nf.NumeroNFe, " + (int)Entidades.GCon.Item.TipoRegistroEnum.Recebimento + @" as TipoRegistro,
                            cr.Desconto, cr.Acrescimo, cr.Juros, cr.IdAcerto, cr.IdSinal")
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
                        .Where("IdPedido = ?idPedido").Add("?idPedido",idPedido)
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

            if (idFormaPagto.GetValueOrDefault() > 0 && idFormaPagto.Value != (uint)Data.Model.Pagto.FormaPagto.Permuta)
            {
                var pagtoContasReceber = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.PagtoContasReceber>()
                    .Where("IdFormaPagto = ?idFormaPagto").Add("?idFormaPagto", idFormaPagto.Value)
                    .SelectDistinct("IdContaR");

                consulta.WhereClause
                    .And("cr.IdContaR IN (?pagtoContasReceber)")
                    .Add("?pagtoContasReceber", pagtoContasReceber);
            }
            else
            {
                var pagtoContasReceberPermuta = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.PagtoContasReceber>()
                    .Where("IdFormaPagto = ?idFormaPagto").Add("?idFormaPagto", (uint)Data.Model.Pagto.FormaPagto.Permuta)
                    .SelectDistinct("IdContaR");

                consulta.WhereClause
                    .And("cr.IdContaR NOT IN (?pagtoContasReceberPermuta)")
                    .Add("?pagtoContasReceberPermuta", pagtoContasReceberPermuta);
            }

            if (precoInicial > 0)
                consulta.WhereClause
                        .And("cr.ValorRec >= ?precoInicial").Add("?precoInicial", precoInicial);

            if (precoFinal > 0)
                consulta.WhereClause
                        .And("cr.ValorRec <= ?precoFinal").Add("?precoFinal", precoFinal);

            if (idContaBancoRecebimento > 0)
            {
                consulta
                    .LeftJoin<Data.Model.PagtoContasReceber>("cr.IdContaR = pcr.IdContaR", "pcr");

                consulta.WhereClause
                    .And("pcr.IdContaBanco = ?idContaBancoRecebimento")
                    .Add("?idContaBancoRecebimento", idContaBancoRecebimento);
            }

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
                            .Where("IdRota = ?idRota").Add("?idRota",idRota)
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

            var registros = consulta.Execute<Entidades.GCon.Item>().ToList();

            if (registros.Count == 0)
                return null;

            #region Busca as formas pagto.

            var lstMovBanco = SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.MovBanco>("mv")
                .InnerJoin<Glass.Data.Model.ContaBanco>("mv.IdContaBanco = cb.IdContaBanco", "cb")
                .Where(string.Format("mv.IdContaR IN ({0}) AND mv.TipoMov = 1", string.Join(",", registros.Select(f => f.IdConta.ToString()).ToArray())))
                .Select("mv.IdConta, mv.IdContaR, mv.ValorMov, mv.Juros, cb.CodBanco, mv.TipoMov")
                .OrderBy("DATE_FORMAT(mv.DataCad, '%Y-%m-%d %H%i') DESC, mv.IdMovBanco DESC")
                .Execute()
                .Select(f => new
                {
                    IdConta = f.GetInt32(0),
                    IdContaR = f.GetInt32(1),
                    ValorMov = f.GetDecimal(2),
                    Juros = f.GetDecimal(3),
                    CodBanco = f.GetInt32(4),
                    TipoMov = f.GetInt32(5)
                }).ToList();

            var lstMovBancoSaida = SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.MovBanco>("mv")
                .InnerJoin<Glass.Data.Model.ContaBanco>("mv.IdContaBanco = cb.IdContaBanco", "cb")
                .Where(string.Format("mv.IdContaR IN ({0}) AND mv.TipoMov = 2", string.Join(",", registros.Select(f => f.IdConta.ToString()).ToArray())))
                .Select("mv.IdConta, mv.IdContaR, mv.ValorMov, mv.Juros, cb.CodBanco, mv.TipoMov")
                .Execute()
                .Select(f => new
                {
                    IdConta = f.GetInt32(0),
                    IdContaR = f.GetInt32(1),
                    ValorMov = f.GetDecimal(2),
                    Juros = f.GetDecimal(3),
                    CodBanco = f.GetInt32(4)
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

            var lstCxDiarioEstorno = SourceContext.Instance.CreateQuery()
                .From<Data.Model.CaixaDiario>("cx")
                .Where(string.Format("cx.IdContaR IN ({0}) AND cx.IdConta NOT IN ({1}) AND cx.TipoMov = 2", string.Join(",", registros.Select(f => f.IdConta.ToString()).ToArray()), planosDesconsiderarCxGeral))
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

            var lstCxGeralEstorno = SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.CaixaGeral>("cx")
                .Where(string.Format("cx.IdContaR IN ({0}) AND cx.IdConta NOT IN ({1}) AND cx.TipoMov = 2", string.Join(",", registros.Select(f => f.IdConta.ToString()).ToArray()), planosDesconsiderarCxGeral))
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
                .Select("dni.IdContaR,dni.IdContaBanco,cb.CodBanco,dni.ValorMov,dni.IdAcerto,dni.IdSinal")
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
                .Select("pa.IdAcerto as Id, pa.IdFormaPagto as FormaPagto, pa.ValorPagto as Valor, cb.CodBanco, tmp.TotalVec as Total, a.CreditoGeradoCriar as CreditoGerado")
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
                .Select("ps.IdSinal as Id, ps.IdFormaPagto as FormaPagto, ps.ValorPagto as Valor, cb.CodBanco, tmp.TotalVec as Total, s.CreditoGeradoCriar as CreditoGerado")
                .GroupBy("s.IdSinal, ps.IdFormaPagto, ps.ValorPagto, ps.IdContaBanco")
                .Execute<Pgto>().ToList();

            #endregion

            #endregion

            #region Formas de Pagto.

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
                            if (a.FormaPagto == Glass.Data.Model.Pagto.FormaPagto.Permuta)
                                continue;

                            var valorRec = a.Valor * percConta / 100;

                            if (a.FormaPagto == Glass.Data.Model.Pagto.FormaPagto.Dinheiro || a.FormaPagto == Glass.Data.Model.Pagto.FormaPagto.ChequeProprio || 
                                a.FormaPagto == Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro)
                            {
                                item.ValorDinheiroCheque += valorRec;
                            }
                            else if (a.FormaPagto == Glass.Data.Model.Pagto.FormaPagto.Boleto || a.FormaPagto == Glass.Data.Model.Pagto.FormaPagto.Deposito || 
                                a.FormaPagto == Glass.Data.Model.Pagto.FormaPagto.DepositoNaoIdentificado)
                            {
                                var codigoBanco = a.CodBanco;

                                /* Chamado 28906. */
                                if (codigoBanco == 0)
                                {
                                    codigoBanco =
                                        SourceContext.Instance.CreateQuery()
                                            .From<Data.Model.DepositoNaoIdentificado>("dni")
                                            .InnerJoin<Data.Model.ContaBanco>("dni.IdContaBanco=cb.IdContaBanco", "cb")
                                            .Where("dni.IdAcerto = ?idAcerto AND dni.ValorMov = ?valorMov")
                                                .Add("?idAcerto", item.IdAcerto.Value)
                                                .Add("?valorMov", a.Valor)
                                            .Select("cb.CodBanco")
                                            .Execute()
                                            .FirstOrDefault()
                                            .GetInt32(0);
                                }

                                if (codigoBanco == 1)
                                    item.ValorBancoBrasil += valorRec;
                                else if (codigoBanco == 41)
                                    item.ValorBanrisul += valorRec;
                                else if (codigoBanco == 748)
                                    item.ValorSicredi += valorRec;
                                else if (codigoBanco == 237)
                                    item.ValorBradesco += valorRec;
                            }
                            else if (a.FormaPagto == Data.Model.Pagto.FormaPagto.Credito)
                            {
                                item.ValorAdiantadosRecebimento += valorRec;
                            }
                        }
                    }
                }

                #endregion

                #region Sinal

                else if (item.IdSinal.GetValueOrDefault(0) > 0)
                {
                    var pagtosSinal = lstPagtoSinal.Where(f => f.Id == item.IdSinal && f.FormaPagto != Data.Model.Pagto.FormaPagto.Permuta).Select(f => (Pgto)f.Clone()).ToList();

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

                            if (a.FormaPagto == Glass.Data.Model.Pagto.FormaPagto.Dinheiro || a.FormaPagto == Glass.Data.Model.Pagto.FormaPagto.ChequeProprio ||
                                a.FormaPagto == Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro)
                            {
                                item.ValorDinheiroCheque += valorRec;
                            }
                            else if (a.FormaPagto == Glass.Data.Model.Pagto.FormaPagto.Boleto || a.FormaPagto == Glass.Data.Model.Pagto.FormaPagto.Deposito ||
                                a.FormaPagto == Glass.Data.Model.Pagto.FormaPagto.DepositoNaoIdentificado)
                            {
                                if (a.CodBanco == 1)
                                    item.ValorBancoBrasil += valorRec;
                                else if (a.CodBanco == 41)
                                    item.ValorBanrisul += valorRec;
                                else if (a.CodBanco == 748)
                                    item.ValorSicredi += valorRec;
                                else if (a.CodBanco == 237)
                                    item.ValorBradesco += valorRec;
                            }
                            else if (a.FormaPagto == Data.Model.Pagto.FormaPagto.Credito)
                            {
                                item.ValorAdiantadosRecebimento += valorRec;
                            }
                        }
                    }
                }

                #endregion

                #region Cx Diario / Cx Geral / Mov Banco

                else
                {
                    var cxGeral = lstCxGeral.Where(f => f.IdContaR == item.IdConta).ToList();
                    var cxGeralEstorno = lstCxGeralEstorno.Where(f => f.IdContaR == item.IdConta).ToList();
                    var cxDiario = lstCxDiario.Where(f => f.IdContaR == item.IdConta).ToList();
                    var cxDiarioEstorno = lstCxDiarioEstorno.Where(f => f.IdContaR == item.IdConta).ToList();
                    var mbs = lstMovBanco.Where(f => f.IdContaR == item.IdConta).ToList();
                    var mbsSaida = lstMovBancoSaida.Where(f => f.IdContaR == item.IdConta).ToList();
                    var depositosNaoIdentificados = lstDepositoNaoIdentificado.Where(f => f.IdContaR == item.IdConta).ToList();

                    foreach (var cxG in cxGeral)
                    {
                        var formaPgto = UtilsPlanoConta.GetFormaPagtoByIdConta(Convert.ToUInt32(cxG.IdConta));
                        var idFPagto = formaPgto != null ? formaPgto.IdFormaPagto.GetValueOrDefault(0) : 0;

                        if (idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Permuta)
                            continue;

                        if (idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro || idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio ||
                            idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro)
                        {
                            item.ValorDinheiroCheque += cxG.ValorMov;
                            item.ValorJurosDinheiroCheque += cxG.Juros;
                        }
                        else if (planosContaCredito.Contains(Convert.ToUInt32(cxG.IdConta)))
                        {
                            item.ValorAdiantadosRecebimento += cxG.ValorMov;
                            item.ValorJurosAdiantadosRecebimento += cxG.Juros;
                        }
                    }

                    foreach (var cxGE in cxGeralEstorno)
                    {
                        var formaPgto = UtilsPlanoConta.GetFormaPagtoByIdConta(Convert.ToUInt32(cxGE.IdConta));
                        var idFPagto = formaPgto != null ? formaPgto.IdFormaPagto.GetValueOrDefault(0) : 0;

                        if (idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Permuta)
                            continue;

                        if (idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro || idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio ||
                            idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro)
                        {
                            item.ValorDinheiroCheque -= cxGE.ValorMov;
                            item.ValorJurosDinheiroCheque -= cxGE.Juros;
                        }
                        else if (planosContaEstornoCredito.Contains(Convert.ToUInt32(cxGE.IdConta)))
                        {
                            item.ValorAdiantadosRecebimento -= cxGE.ValorMov;
                            item.ValorJurosAdiantadosRecebimento -= cxGE.Juros;
                        }
                    }

                    foreach (var cxD in cxDiario)
                    {
                        var formaPgto = UtilsPlanoConta.GetFormaPagtoByIdConta(Convert.ToUInt32(cxD.IdConta));
                        var idFPagto = formaPgto != null ? formaPgto.IdFormaPagto.GetValueOrDefault(0) : 0;

                        if (idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Permuta)
                            continue;

                        if (idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Dinheiro || idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeProprio ||
                            idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro)
                        {
                            item.ValorDinheiroCheque += cxD.ValorMov;
                            item.ValorJurosDinheiroCheque += cxD.Juros;
                        }
                        else if (planosContaCredito.Contains(Convert.ToUInt32(cxD.IdConta)))
                        {
                            item.ValorAdiantadosRecebimento += cxD.ValorMov;
                            item.ValorJurosAdiantadosRecebimento += cxD.Juros;
                        }
                    }

                    foreach (var cxD in cxDiarioEstorno)
                    {
                        var formaPgto = UtilsPlanoConta.GetFormaPagtoByIdConta(Convert.ToUInt32(cxD.IdConta));
                        var idFPagto = formaPgto != null ? formaPgto.IdFormaPagto.GetValueOrDefault(0) : 0;

                        if (idFPagto == (uint)Data.Model.Pagto.FormaPagto.Permuta)
                            continue;

                        if (idFPagto == (uint)Data.Model.Pagto.FormaPagto.Dinheiro || idFPagto == (uint)Data.Model.Pagto.FormaPagto.ChequeProprio ||
                            idFPagto == (uint)Data.Model.Pagto.FormaPagto.ChequeTerceiro)
                        {
                            item.ValorDinheiroCheque -= cxD.ValorMov;
                            item.ValorJurosDinheiroCheque -= cxD.Juros;
                        }
                        else if (planosContaEstornoCredito.Contains(Convert.ToUInt32(cxD.IdConta)))
                        {
                            item.ValorAdiantadosRecebimento -= cxD.ValorMov;
                            item.ValorJurosAdiantadosRecebimento -= cxD.Juros;
                        }
                    }

                    foreach (var mb in mbs)
                    {
                        var formaPgto = Glass.Data.Helper.UtilsPlanoConta.GetFormaPagtoByIdConta(Convert.ToUInt32(mb.IdConta));
                        var idFPagto = formaPgto != null ? formaPgto.IdFormaPagto.GetValueOrDefault(0) : 0;

                        if (idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Permuta)
                            continue;
 
                        /* Chamado 55569. */
                        if (mb.TipoMov == 2 && mb.IdConta != Configuracoes.FinanceiroConfig.PlanoContaJurosCartao)
                            break;

                        var juros = mb.Juros;

                        if (idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto || idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito ||
                            idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.DepositoNaoIdentificado)
                        {
                            if (mb.CodBanco == 1)
                            {
                                item.ValorBancoBrasil += mb.ValorMov - juros;
                                item.ValorJurosBancoBrasil += juros;
                            }
                            else if (mb.CodBanco == 41)
                            {
                                item.ValorBanrisul += mb.ValorMov - juros;
                                item.ValorJurosBanrisul += juros;
                            }
                            else if (mb.CodBanco == 748)
                            {
                                item.ValorSicredi += mb.ValorMov - juros;
                                item.ValorJurosSicredi += juros;
                            }
                            else if (mb.CodBanco == 237)
                            {
                                item.ValorBradesco += mb.ValorMov - juros;
                                item.ValorJurosBradesco += juros;
                            }
                        }
                    }

                    foreach (var mb in mbsSaida)
                    {
                        var formaPgto = Glass.Data.Helper.UtilsPlanoConta.GetFormaPagtoByIdConta(Convert.ToUInt32(mb.IdConta));
                        var idFPagto = formaPgto != null ? formaPgto.IdFormaPagto.GetValueOrDefault(0) : 0;

                        if (idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Permuta)
                            continue;

                        var juros = mb.Juros;

                        if (idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Boleto || idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.Deposito ||
                            idFPagto == (uint)Glass.Data.Model.Pagto.FormaPagto.DepositoNaoIdentificado)
                        {
                            if (mb.CodBanco == 1)
                            {
                                item.ValorBancoBrasil -= mb.ValorMov - juros;
                                item.ValorJurosBancoBrasil -= juros;
                            }
                            else if (mb.CodBanco == 41)
                            {
                                item.ValorBanrisul -= mb.ValorMov - juros;
                                item.ValorJurosBanrisul -= juros;
                            }
                            else if (mb.CodBanco == 748)
                            {
                                item.ValorSicredi -= mb.ValorMov - juros;
                                item.ValorJurosSicredi -= juros;
                            }
                            else if (mb.CodBanco == 237)
                            {
                                item.ValorBradesco -= mb.ValorMov - juros;
                                item.ValorJurosBradesco -= juros;
                            }
                        }
                    }

                    /* Chamado 27772. */
                    foreach (var depositoNaoIdentificado in depositosNaoIdentificados)
                    {
                        if (depositoNaoIdentificado.CodBanco == 1)
                            item.ValorBancoBrasil += depositoNaoIdentificado.ValorMov;
                        else if (depositoNaoIdentificado.CodBanco == 41)
                            item.ValorBanrisul += depositoNaoIdentificado.ValorMov;
                        else if (depositoNaoIdentificado.CodBanco == 748)
                            item.ValorSicredi += depositoNaoIdentificado.ValorMov;
                        else if (depositoNaoIdentificado.CodBanco == 237)
                            item.ValorBradesco += depositoNaoIdentificado.ValorMov;
                    }
                }

                #endregion
            }

            #endregion

            var erros = registros.Where(f => f.diff != 0).ToList();

            CriarTotalizadores(ref registros);

            var creditos = registros.Where(f => f.TipoConta == "\"C \";").Sum(f => Math.Round(f.Valor + f.Desconto, 2));
            var debitos = registros.Where(f => f.TipoConta == "\"D \";").Sum(f => Math.Round(f.Valor + f.Desconto, 2));
            var diff = Math.Abs(creditos - debitos);

            if (diff > 0.1m)
                throw new Exception("Falha ao gerar arquivo GCON. Entre em contato com suporte WebGlass. Erro 1");

            var arquivo = new Entidades.GCon.Arquivo() { Itens = registros };

            return arquivo;
        }

        public Entidades.GCon.Arquivo GerarArquivoPagas(int? idContaPg, int? idCompra, int? numNfPedido, int? idCustoFixo, int? idImpServ, int? idComissao, decimal? valorPagtoIni,
            decimal? valorPagtoFim, DateTime? dataCadIni, DateTime? dataCadFim, DateTime? dataVencIni, DateTime? dataVencFim, DateTime? dataPagtoIni, DateTime? dataPagtoFim, int? idLoja,
            int? idFornec, string nomeFornec, int? idFormaPagto, int? idConta, bool jurosMulta, string observacao)
        {
            #region Consulta

            var consulta = SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.ContasPagar>("cp")
                    .LeftJoin<Glass.Data.Model.Fornecedor>("cp.IdFornec = f.IdFornec", "f")
                    .LeftJoin<Glass.Data.Model.NotaFiscal>("cp.IdNf = nf.IdNf", "nf")
                    .LeftJoin<Glass.Data.Model.Compra>("cp.IdCompra = cmp.IdCompra", "cmp")
                    .LeftJoin<Glass.Data.Model.ImpostoServ>("cp.IdImpostoServ = i.IdImpostoServ", "i")
                    .LeftJoin<Data.Model.PlanoContas>("cp.IdConta = pc.IdConta", "pc")
                    .LeftJoin<Data.Model.PlanoContaContabil>("pc.IdContaContabil = pcc.IdContaContabil", "pcc");

            consulta
                .Where("cp.Paga = 1 AND cp.Contabil = 1 AND IsNull(cp.Renegociada, 0) = 0")
                .Select("cp.IdContaPg as IdConta, " + (int)Entidades.GCon.Item.TipoArquivoEnum.Pagar + @" as TipoArquivo, cp.DataPagto as DataLiquidacao,
                                LEAST(cp.ValorPago, cp.ValorVenc) as Valor, cp.IdFornec as IdCliente, f.Razaosocial as RazaoSocial, f.CpfCnpj, f.RgInscEst as InscEstadual,
                                cp.NumParc as Parcela, nf.NumeroNFe, " + (int)Entidades.GCon.Item.TipoRegistroEnum.Pagamento + @" as TipoRegistro,
                                cp.Desconto, cp.AcrescimoParc as Acrescimo, (cp.Juros + cp.Multa) as Juros, pcc.CodInterno as TipoContabil")
                .OrderBy("cp.DataPagto desc");

            #region Filtros

            if (idContaPg > 0)
                consulta.WhereClause
                    .And("cp.IdContaPg=" + idContaPg);

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

            if (dataCadIni > DateTime.MinValue)
                consulta.WhereClause
                    .And("cp.DataCad >= ?dataCadIni")
                    .Add("dataCadIni", dataCadIni.Value.Date);

            if (dataCadFim > DateTime.MinValue)
                consulta.WhereClause
                    .And("cp.DataCad <= ?dataCadFim")
                    .Add("dataCadFim", dataCadFim.Value.Date.AddDays(1).AddMinutes(-1));

            if (dataVencIni > DateTime.MinValue)
                consulta.WhereClause
                    .And("cp.DataVenc >= ?dataVencIni")
                    .Add("dataVencIni", dataVencIni.Value.Date);

            if (dataVencFim > DateTime.MinValue)
                consulta.WhereClause
                    .And("cp.DataVenc <= ?dataVencFim")
                    .Add("dataVencFim", dataVencFim.Value.Date.AddDays(1).AddMinutes(-1));

            if (dataPagtoIni > DateTime.MinValue)
                consulta.WhereClause
                    .And("cp.DataPagto >= ?dataPagtoIni")
                    .Add("dataPagtoIni", dataPagtoIni.Value.Date);

            if (dataPagtoFim > DateTime.MinValue)
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

            var registros = consulta.Execute<Entidades.GCon.Item>().ToList();

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

            //Remove as contas que tiveram recebimento com conta bancaria pessoal
            //Deve ser gerado apenas das contas BRASIL - MODELO, SICREDI - MODELO, BANRISUL - MODELO, SICREDI - MXM
            var contasRemover = lstPagtos
                .Where(f => f.CodBanco != 0 && f.CodBanco != 40 && f.CodBanco != 41 && f.CodBanco != 42 && f.CodBanco != 43)
                .Select(f => f.IdContaPg)
                .GroupBy(f => f)
                .Select(f => f.Key)
                .ToList();

            registros = registros.Where(f => !contasRemover.Contains(f.IdConta)).ToList();
            lstPagtos = lstPagtos.Where(f => !contasRemover.Contains(f.IdContaPg)).ToList();

            #endregion

            #region Formas de Pagto.

            foreach (var item in registros)
            {
                foreach (var pg in lstPagtos.Where(f => f.IdContaPg == item.IdConta))
                {
                    switch (pg.FormaPagto)
                    {
                        case Glass.Data.Model.Pagto.FormaPagto.Dinheiro:
                        case Glass.Data.Model.Pagto.FormaPagto.ChequeProprio:
                        case Glass.Data.Model.Pagto.FormaPagto.ChequeTerceiro:
                            {
                                item.ValorDinheiroCheque += pg.ValorMov;
                                break;
                            }
                        case Glass.Data.Model.Pagto.FormaPagto.Credito:
                            {
                                item.ValorAdiantadosRecebimento += pg.ValorMov;
                                break;
                            }
                        case Glass.Data.Model.Pagto.FormaPagto.Boleto:
                        case Glass.Data.Model.Pagto.FormaPagto.Deposito:
                        case Glass.Data.Model.Pagto.FormaPagto.DepositoNaoIdentificado:
                            {
                                var banco = Glass.Data.DAL.ContaBancoDAO.Instance.GetElement((uint)pg.CodBanco);

                                if (banco == null)
                                    break;

                                switch (banco.CodBanco.GetValueOrDefault(0))
                                {
                                    case 1:
                                        item.ValorBancoBrasil += pg.ValorMov;
                                        break;
                                    case 41:
                                        item.ValorBanrisul += pg.ValorMov;
                                        break;
                                    case 748:
                                        item.ValorSicredi += pg.ValorMov;
                                        break;
                                    case 237:
                                        item.ValorBradesco += pg.ValorMov;
                                        break;
                                }

                                break;
                            }
                    }
                }
            }

            #endregion

            CriarTotalizadores(ref registros);

            var creditos = registros.Where(f => f.TipoConta == "\"C \";").Sum(f => Math.Round(f.Valor + f.Desconto, 2));
            var debitos = registros.Where(f => f.TipoConta == "\"D \";").Sum(f => Math.Round(f.Valor + f.Desconto, 2));
            var diff = Math.Abs(creditos - debitos);

            if (diff > 0.1m)
                throw new Exception("Falha ao gerar arquivo GCON. Entre em contato com suporte WebGlass. Erro 1");

            var arquivo = new Entidades.GCon.Arquivo() { Itens = registros };

            return arquivo;
        }

        #region Métodos Privados

        /// <summary>
        /// Método que cria os totalizadores
        /// </summary>
        /// <param name="itens"></param>
        private void CriarTotalizadores(ref List<Entidades.GCon.Item> itens)
        {
            var itensProcessados = new List<Entidades.GCon.Item>();

            if (itens.Count < 1)
                return;

            var receber = itens[0].TipoArquivo == Entidades.GCon.Item.TipoArquivoEnum.Receber;
            var datas = itens.GroupBy(f => f.DataLiquidacao.Date).Select(f => f.Key).OrderBy(f => f).ToList();

            foreach (var d in datas)
            {
                var i = itens.Where(f => f.DataLiquidacao.Date == d).ToList();

                #region Totalizadores

                #region Dinheiro e Cheque

                var valorDineiroCheque = i.Sum(f => f.ValorDinheiroCheque);

                if (valorDineiroCheque > 0)
                    itensProcessados.Add(new Entidades.GCon.Item()
                    {
                        DataLiquidacao = d,
                        TipoArquivo = receber ? Entidades.GCon.Item.TipoArquivoEnum.Receber : Entidades.GCon.Item.TipoArquivoEnum.Pagar,
                        TipoRegistro = receber ? Entidades.GCon.Item.TipoRegistroEnum.TotalRecebimento : Entidades.GCon.Item.TipoRegistroEnum.TotalPagamento,
                        TipoContabil = ((int)Entidades.GCon.Item.TipoContabilEnum.ChequesDinheiro).ToString(),
                        Valor = valorDineiroCheque,

                    });

                #endregion

                #region Crédito

                var valorAdiantadosRecebimento = i.Sum(f => f.ValorAdiantadosRecebimento);

                if (valorAdiantadosRecebimento > 0)
                    itensProcessados.Add(new Entidades.GCon.Item()
                    {
                        DataLiquidacao = d,
                        TipoArquivo = receber ? Entidades.GCon.Item.TipoArquivoEnum.Receber : Entidades.GCon.Item.TipoArquivoEnum.Pagar,
                        TipoRegistro = receber ? Entidades.GCon.Item.TipoRegistroEnum.TotalRecebimento : Entidades.GCon.Item.TipoRegistroEnum.TotalPagamento,
                        TipoContabil = ((int)Entidades.GCon.Item.TipoContabilEnum.AdiantadosRecebimento).ToString(),
                        Valor = valorAdiantadosRecebimento,

                    });

                #endregion

                #region Deposito e Boleto

                #region Banco do Brasil

                var valorBancoBrasil = i.Sum(f => f.ValorBancoBrasil);
                var jurosBancoBrasil = i.Sum(f => f.ValorJurosBancoBrasil);

                if (valorBancoBrasil > 0 || jurosBancoBrasil > 0)
                    itensProcessados.Add(new Entidades.GCon.Item()
                    {
                        DataLiquidacao = d,
                        TipoArquivo = receber ? Entidades.GCon.Item.TipoArquivoEnum.Receber : Entidades.GCon.Item.TipoArquivoEnum.Pagar,
                        TipoRegistro = receber ? Entidades.GCon.Item.TipoRegistroEnum.TotalRecebimento : Entidades.GCon.Item.TipoRegistroEnum.TotalPagamento,
                        TipoContabil = ((int)Entidades.GCon.Item.TipoContabilEnum.BancoBrasil).ToString(),
                        Valor = valorBancoBrasil + jurosBancoBrasil,

                    });

                #endregion

                #region Banco Banrisul

                var valorBanrisul = i.Sum(f => f.ValorBanrisul);
                var jurosBanrisul = i.Sum(f => f.ValorJurosBanrisul);

                if (valorBanrisul > 0 || jurosBanrisul > 0)
                    itensProcessados.Add(new Entidades.GCon.Item()
                    {
                        DataLiquidacao = d,
                        TipoArquivo = receber ? Entidades.GCon.Item.TipoArquivoEnum.Receber : Entidades.GCon.Item.TipoArquivoEnum.Pagar,
                        TipoRegistro = receber ? Entidades.GCon.Item.TipoRegistroEnum.TotalRecebimento : Entidades.GCon.Item.TipoRegistroEnum.TotalPagamento,
                        TipoContabil = ((int)Entidades.GCon.Item.TipoContabilEnum.Banrisul).ToString(),
                        Valor = valorBanrisul + jurosBanrisul,

                    });

                #endregion

                #region Banco Sicredi

                var valorSicredi = i.Sum(f => f.ValorSicredi);
                var jurosSicredi = i.Sum(f => f.ValorJurosSicredi);

                if (valorSicredi > 0 || jurosSicredi > 0)
                    itensProcessados.Add(new Entidades.GCon.Item()
                    {
                        DataLiquidacao = d,
                        TipoArquivo = receber ? Entidades.GCon.Item.TipoArquivoEnum.Receber : Entidades.GCon.Item.TipoArquivoEnum.Pagar,
                        TipoRegistro = receber ? Entidades.GCon.Item.TipoRegistroEnum.TotalRecebimento : Entidades.GCon.Item.TipoRegistroEnum.TotalPagamento,
                        TipoContabil = ((int)Entidades.GCon.Item.TipoContabilEnum.Sicredi).ToString(),
                        Valor = valorSicredi + jurosSicredi,

                    });

                #endregion

                #region Banco Bradesco

                var valorBradesco = i.Sum(f => f.ValorBradesco);
                var jurosBradesco = i.Sum(f => f.ValorJurosBradesco);

                if (valorBradesco > 0 || jurosBradesco > 0)
                    itensProcessados.Add(new Entidades.GCon.Item()
                    {
                        DataLiquidacao = d,
                        TipoArquivo = receber ? Entidades.GCon.Item.TipoArquivoEnum.Receber : Entidades.GCon.Item.TipoArquivoEnum.Pagar,
                        TipoRegistro = receber ? Entidades.GCon.Item.TipoRegistroEnum.TotalRecebimento : Entidades.GCon.Item.TipoRegistroEnum.TotalPagamento,
                        TipoContabil = ((int)Entidades.GCon.Item.TipoContabilEnum.Bradesco).ToString(),
                        Valor = valorBradesco + jurosBradesco,

                    });

                #endregion

                #endregion

                #endregion

                foreach (var it in i)
                {
                    itensProcessados.Add(it);

                    #region Juros

                    if (it.Juros > 0)
                    {
                        itensProcessados.Add(new Entidades.GCon.Item()
                        {
                            IdConta = it.IdConta,
                            TipoArquivo = it.TipoArquivo,
                            DataLiquidacao = it.DataLiquidacao,
                            TipoContabil = ((int)(receber ? Entidades.GCon.Item.TipoContabilEnum.JurosRecebidos : Entidades.GCon.Item.TipoContabilEnum.JurosPagos)).ToString(),
                            Valor = it.Juros,
                            IdCliente = it.IdCliente,
                            RazaoSocial = it.RazaoSocial,
                            CpfCnpj = it.CpfCnpj,
                            InscEstadual = it.InscEstadual,
                            TipoRegistro = Entidades.GCon.Item.TipoRegistroEnum.Juros,
                            NumeroNFe = it.NumeroNFe,
                            Parcela = it.Parcela
                        });
                    }

                    #endregion

                    #region Desconto

                    if (it.Desconto > 0)
                    {
                        itensProcessados.Add(new Entidades.GCon.Item()
                        {
                            IdConta = it.IdConta,
                            TipoArquivo = it.TipoArquivo,
                            DataLiquidacao = it.DataLiquidacao,
                            TipoContabil = ((int)(receber ? Entidades.GCon.Item.TipoContabilEnum.DescontosConcedidos : Entidades.GCon.Item.TipoContabilEnum.DescontosObtidos)).ToString(),
                            Valor = it.Desconto,
                            IdCliente = it.IdCliente,
                            RazaoSocial = it.RazaoSocial,
                            CpfCnpj = it.CpfCnpj,
                            InscEstadual = it.InscEstadual,
                            TipoRegistro = Entidades.GCon.Item.TipoRegistroEnum.Desconto,
                            NumeroNFe = it.NumeroNFe,
                            Parcela = it.Parcela
                        });
                    }

                    #endregion
                }
            }

            itens = new List<Entidades.GCon.Item>(itensProcessados);
        }

        #endregion
    }
}
