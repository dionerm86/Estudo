using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;
using Glass.Data.DAL;
using System.IO;
using NPOI.XSSF.UserModel;
using Glass.Financeiro.Negocios.Entidades;

namespace Glass.Financeiro.Negocios.Componentes
{
    class CartaoNaoIdentificadoFluxo : ICartaoNaoIdentificadoFluxo, IProvedorCartaoNaoIdentificado, IValidadorCartaoNaoIdentificado
    {
        /// <summary>
        /// Recupera as informações do CNI para exibição na listagem/Relatório
        /// </summary>
        public IList<CartaoNaoIdentificadoPesquisa> PesquisarCartoesNaoIdentificados(int? idCartaoNaoIdentificado, 
            int? idContaBanco, decimal? valorInicio, decimal? valorFim, Data.Model.SituacaoCartaoNaoIdentificado? situacao, int? tipoCartao, 
            DateTime? dataCadInicio, DateTime? dataCadFim, DateTime? dataVendaInicio, DateTime? dataVendaFim, string nAutorizacao, string numEstabelecimento, 
            string ultimosDigitosCartao, int? codArquivo, DateTime? dataImportacao)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.CartaoNaoIdentificado>("cni")
                .LeftJoin<Data.Model.ContaBanco>("cni.IdContaBanco=cb.IdContaBanco", "cb")
                .LeftJoin<Data.Model.Funcionario>("cni.Usucad=f.IdFunc", "f")
                .LeftJoin<Data.Model.ArquivoCartaoNaoIdentificado>("cni.IdArquivoCartaoNaoIdentificado=acni.IdArquivoCartaoNaoIdentificado", "acni")
                .LeftJoin<Data.Model.TipoCartaoCredito>("cni.TipoCartao = tcc.IdTipoCartao", "tcc")
                .LeftJoin<Data.Model.BandeiraCartao>("tcc.Bandeira=bc.IdBandeiraCartao", "bc")
                .LeftJoin<Data.Model.OperadoraCartao>("tcc.Operadora=oc.IdOperadoraCartao", "oc")
                .Select(@"cni.IdCartaoNaoIdentificado, cb.Nome AS NomeBanco, cni.NumAutCartao, cb.Agencia, cb.Conta, cni.DataCad, 
                          f.Nome AS FuncionarioCadastro, cni.Valor, cni.TipoCartao, cni.Situacao, cni.Observacao, 
                          cni.IdAcerto, cni.IdContaR, cni.IdDevolucaoPagto, cni.IdLiberarPedido, cni.IdObra, 
                          cni.IdPedido, cni.IdSinal, cni.IdTrocaDevolucao, cni.IdAcertoCheque, cni.DataVenda, 
                          cni.NumeroEstabelecimento, cni.UltimosDigitosCartao, cni.Importado, cni.NumeroParcelas, 
                          cni.IdArquivoCartaoNaoIdentificado, tcc.Operadora, tcc.Bandeira, tcc.Tipo, bc.Descricao AS DescBandeira, oc.Descricao AS DescOperadora")
                .OrderBy("cni.IdCartaoNaoIdentificado DESC");

            if (idCartaoNaoIdentificado.GetValueOrDefault() > 0)
            {
                consulta.WhereClause
                    .And("IdCartaoNaoIdentificado=?idCartaoNaoIdentificado")
                    .Add("?idCartaoNaoIdentificado", idCartaoNaoIdentificado)
                    .AddDescription("Cod. CNI: " + idCartaoNaoIdentificado);
            }
            else
            {
                if (idContaBanco.GetValueOrDefault() > 0)
                    consulta.WhereClause
                        .And("cni.IdContaBanco=?idContaBanco")
                        .Add("?idContaBanco", idContaBanco)
                        .AddDescription("Conta Bancária: " + 
                            SourceContext.Instance.CreateQuery()
                                .From<Data.Model.ContaBanco>()
                                .Select("Nome")
                                .Where("IdContaBanco=?idContaBanco")
                                .Add("?idContaBanco", idContaBanco)
                                .Execute().Select(f => f.GetString(0))
                                .FirstOrDefault());

                if (valorInicio.GetValueOrDefault() > 0)
                    consulta.WhereClause
                        .And("cni.Valor>=?valorInicio")
                        .Add("?valorInicio", valorInicio)
                        .AddDescription("Valor inicial Busca: " + valorInicio);

                if (valorFim.GetValueOrDefault() > 0)
                    consulta.WhereClause
                        .And("cni.Valor<=?valorFim")
                        .Add("?valorFim", valorFim)
                        .AddDescription("Valor Final Busca: " + valorFim);

                if (situacao.GetValueOrDefault() > 0)
                    consulta.WhereClause
                        .And("cni.Situacao=?situacao")
                        .Add("?situacao", situacao)
                        .AddDescription("Situação: " + situacao.ToString());

                if (tipoCartao.GetValueOrDefault() > 0)
                    consulta.WhereClause
                        .And("cni.TipoCartao=?tipoCartao")
                        .Add("?tipoCartao", tipoCartao)
                        .AddDescription("Tipo Cartão: " + tipoCartao.ToString());

                if (dataCadInicio != null)
                    consulta.WhereClause
                        .And("cni.DataCad>=?dataCadInicio")
                        .Add("?dataCadInicio", dataCadInicio)
                        .AddDescription("Data inicial de cadastro: " + dataCadInicio);

                if (dataCadFim != null)
                    consulta.WhereClause
                        .And("cni.DataCad<=?dataCadFim")
                        .Add("?dataCadFim", dataCadFim.Value.AddDays(1).AddSeconds(-1))
                        .AddDescription("Data final de cadastro: " + dataCadFim.Value.AddDays(1).AddSeconds(-1));

                if (dataVendaInicio != null)
                    consulta.WhereClause
                        .And("cni.DataVenda>=?dataVendaInicio")
                        .Add("?dataVendaInicio", dataVendaInicio)
                        .AddDescription("Data inicial de venda: " + dataVendaInicio);

                if (dataVendaFim != null)
                    consulta.WhereClause
                        .And("cni.DataVenda<=?dataVendaFim")
                        .Add("?dataVendaFim", dataVendaFim.Value.AddDays(1).AddSeconds(-1))
                        .AddDescription("Data final de cadastro: " + dataVendaFim.Value.AddDays(1).AddSeconds(-1));

                if (!nAutorizacao.IsNullOrEmpty())
                    consulta.WhereClause
                      .And("cni.NumAutCartao=?nAutorizacao")
                      .Add("?nAutorizacao", nAutorizacao)
                      .AddDescription("Nº de Autorização: " + nAutorizacao);

                if (!numEstabelecimento.IsNullOrEmpty())
                    consulta.WhereClause
                        .And("cni.NumeroEstabelecimento=?numEstabelecimento")
                        .Add("?numEstabelecimento", numEstabelecimento)
                        .AddDescription("Nº Estabelecimento: " + numEstabelecimento);

                if (!ultimosDigitosCartao.IsNullOrEmpty())
                    consulta.WhereClause
                        .And("cni.UltimosDigitosCartao=?ultimosDigitosCartao")
                        .Add("?ultimosDigitosCartao", ultimosDigitosCartao)
                        .AddDescription("Últimos dígitos do cartão: " + ultimosDigitosCartao);

                if (codArquivo.GetValueOrDefault() > 0)
                    consulta.WhereClause
                        .And("cni.IdArquivoCartaoNaoIdentificado=?codArquivo")
                        .Add("?codArquivo", codArquivo)
                        .AddDescription("Cód. Arquivo Associado: " + codArquivo);

                if (dataImportacao != null)
                    consulta.WhereClause
                        .And("acni.DataCad>=?dataImportacao AND acni.DataCad<=?dataImportacaoFim")
                        .Add("?dataImportacao", dataImportacao)
                        .Add("dataImportacaoFim", dataImportacao.Value.AddDays(1).AddSeconds(-1))
                        .AddDescription("Data de importação: " + dataImportacao);
            }

            return consulta.ToVirtualResultLazy<CartaoNaoIdentificadoPesquisa>();
        }

        /// <summary>
        /// Recupera o Cartão não identificado com base no Id
        /// </summary>
        public CartaoNaoIdentificado ObterCartaoNaoIdentificado(int idCartaoNaoIdentificado)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.CartaoNaoIdentificado>()
                .Where("IdCartaoNaoIdentificado=?idCartaoNaoIdentificado")
                .Add("?idCartaoNaoIdentificado", idCartaoNaoIdentificado)
                .ProcessLazyResult<Entidades.CartaoNaoIdentificado>()
                .FirstOrDefault();
        }             

        /// <summary>
        /// Recupera a referência do deposito não identificado.
        /// </summary>
        string IProvedorCartaoNaoIdentificado.ObterReferencia(Entidades.ICartaoNaoIdentificado cartaoNaoIdentificado)
        {
            var referencias = new List<string>();

            if (cartaoNaoIdentificado.IdAcerto.HasValue)
                referencias.Add(string.Format("Acerto: {0}", cartaoNaoIdentificado.IdAcerto));

            if (cartaoNaoIdentificado.IdContaR.HasValue)
            {
                var contaReceber = ContasReceberDAO.Instance.GetByIdContaR(null, (uint)cartaoNaoIdentificado.IdContaR);

                if (contaReceber != null)
                    referencias.Add(contaReceber.Referencia);
            }

            if (cartaoNaoIdentificado.IdDevolucaoPagto.HasValue)
                referencias.Add(string.Format("Devolução pagto.: {0}", cartaoNaoIdentificado.IdDevolucaoPagto));

            if (cartaoNaoIdentificado.IdLiberarPedido.HasValue)
                referencias.Add(string.Format("Liberação pedido: {0}", cartaoNaoIdentificado.IdLiberarPedido));

            if (cartaoNaoIdentificado.IdObra.HasValue)
                referencias.Add(string.Format("Obra: {0}", cartaoNaoIdentificado.IdObra));

            if (cartaoNaoIdentificado.IdPedido.HasValue)
                referencias.Add(string.Format("Pedido: {0}", cartaoNaoIdentificado.IdPedido));

            if (cartaoNaoIdentificado.IdSinal.HasValue)
                referencias.Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Sinal>()
                    .Select(@"CASE 
                                   WHEN IsPagtoAntecipado 
                                   THEN CONCAT('Pagto. Antecipado: ', IdSinal) 
                                   ELSE CONCAT('Sinal: ', IdSinal) 
                               END AS Referencia")
                    .Where("IdSinal=?id")
                    .Add("?id", cartaoNaoIdentificado.IdSinal)
                    .Execute()
                    .Select(f => f.GetString(0))
                    .FirstOrDefault());

            if (cartaoNaoIdentificado.IdTrocaDevolucao.HasValue)
            {
                var tipo = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.TrocaDevolucao>()
                    .Where("IdTrocaDevolucao=?id")
                    .Add("?id", cartaoNaoIdentificado.IdTrocaDevolucao)
                    .Select("Tipo")
                    .Execute()
                    .Select(f => (Data.Model.TrocaDevolucao.TipoTrocaDev)f.GetInt32(0))
                    .First();

                referencias.Add(string.Format("{0}: {1}", tipo.Translate().Format(), cartaoNaoIdentificado.IdTrocaDevolucao));
            }

            if (cartaoNaoIdentificado.IdAcertoCheque.HasValue)
                referencias.Add(string.Format("Acerto Cheque: {0}", cartaoNaoIdentificado.IdAcertoCheque));

            return string.Join(", ", referencias);
        }

        /// <summary>
        /// Recupera se o registro pode ter seu valor alterado
        /// </summary>
        bool IProvedorCartaoNaoIdentificado.EditarValor(int idCartaoNaoIdentificado)
        {
            return !SourceContext.Instance.CreateQuery()
                .From<Data.Model.ContasReceber>()
                .Where("IdCartaoNaoIdentificado=?idCartaoNaoIdentificado AND Recebida=1")
                .Add("?idCartaoNaoIdentificado", idCartaoNaoIdentificado)
                .ExistsResult();
        }

        /// <summary>
        /// Salva a instância do cartão não identificado
        /// </summary>
        public Colosoft.Business.SaveResult SalvarCartaoNaoIdentificado(Entidades.CartaoNaoIdentificado cartaoNaoIdentificado)
        {
            cartaoNaoIdentificado.Require("cartaoNaoIdentificado").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                Colosoft.Business.SaveResult retorno;

                if (!(retorno = cartaoNaoIdentificado.Save(session)))
                    return retorno;

                retorno = session.Execute(false).ToSaveResult();

                return retorno;
            }
        }

        /// <summary>
        /// Salva a instância do cartão não identificado
        /// </summary>
        public Colosoft.Business.DeleteResult ApagarCartaoNaoIdentificado(CartaoNaoIdentificado cartaoNaoIdentificado)
        {
            cartaoNaoIdentificado.Require("cni").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                Colosoft.Business.DeleteResult retorno;

                if (!(retorno = cartaoNaoIdentificado.Delete(session)))
                    return retorno;

                retorno = session.Execute(false).ToDeleteResult();

                return retorno;
            }
        }

        /// <summary>
        /// Cria uma nova instância de CNI
        /// </summary>
        /// <returns></returns>
        public CartaoNaoIdentificado CriarCartaoNaoIdentificado()
        {
            return SourceContext.Instance.Create<CartaoNaoIdentificado>();
        }

        /// <summary>
        /// Recupera os Ids das parcelas geradas para um CNI
        /// </summary>
        public IList<int> PesquisarIdsParcelasCNI(int idCNI)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.ContasReceber>()
                .Select("IdContaR")
                .Where("IdCartaoNaoIdentificado=?idCNI")
                .Add("?idCNI", idCNI)
                .Execute()
                .Select(f => f.GetInt32(0)).ToList();
        }

        /// <summary>
        /// Verifica se o cni passado pode ser inserido no banco
        /// </summary>
        public bool VerificarPodeInserir(string numAutCartao, int tipoCartao, out string msgErro)
        {
            if (SourceContext.Instance.CreateQuery()
                .From<Data.Model.CartaoNaoIdentificado>()
                .Where("NumAutCartao=?numAutCartao AND TipoCartao=?tipoCartao AND Situacao<>?situacao")
                .Add("?numAutCartao", numAutCartao)
                .Add("?tipoCartao", tipoCartao)
                .Add("?situacao", Data.Model.SituacaoCartaoNaoIdentificado.Cancelado)
                .ExistsResult())
            {
                msgErro = "Já existe um cartão não identificado com o mesmo número de autorização inserido.";
                return false;
            }

            if (SourceContext.Instance.CreateQuery()
                .From<Data.Model.PagtoContasReceber>()
                .Where("NumAutCartao=?numAutCartao AND IdTipoCartao=?tipoCartao")
                .Add("?numAutCartao", numAutCartao)
                .Add("?tipoCartao", tipoCartao)
                .ExistsResult())
            {
                msgErro = "Já existe um recebimento de conta com o mesmo número de autorização inserido.";
                return false;
            }
            
            if (SourceContext.Instance.CreateQuery()
                .From<Data.Model.PagtoAcerto>("pa")
                    .InnerJoin<Data.Model.Acerto>("pa.IdAcerto=a.IdAcerto", "a")
                .Where("pa.NumAutCartao=?numAutCartao AND pa.IdTipoCartao=?tipoCartao AND a.Situacao<>?situacao")
                .Add("?numAutCartao", numAutCartao)
                .Add("?tipoCartao", tipoCartao)
                .Add("?situacao", Data.Model.Acerto.SituacaoEnum.Cancelado)
                .ExistsResult())
            {
                msgErro = "Já existe um acerto com o mesmo número de autorização inserido.";
                return false;
            }

            if (SourceContext.Instance.CreateQuery()
                .From<Data.Model.PagtoAcertoCheque>("pac")
                    .InnerJoin<Data.Model.AcertoCheque>("pac.IdAcertoCheque=ac.IdAcertoCheque", "ac")
                .Where("pac.NumAutCartao=?numAutCartao AND pac.IdTipoCartao=?tipoCartao AND ac.Situacao<>?situacao")
                .Add("?numAutCartao", numAutCartao)
                .Add("?tipoCartao", tipoCartao)
                .Add("?situacao", Data.Model.AcertoCheque.SituacaoEnum.Cancelado)
                .ExistsResult())
            {
                msgErro = "Já existe um acerto de cheque com o mesmo número de autorização inserido.";
                return false;
            }

            if (SourceContext.Instance.CreateQuery()
                .From<Data.Model.PagtoAntecipacaoFornecedor>("paf")
                    .InnerJoin<Data.Model.AntecipacaoFornecedor>("paf.IdAntecipFornec=af.IdAntecipFornec", "af")
                .Where("paf.NumAutCartao=?numAutCartao AND paf.IdTipoCartao=?tipoCartao AND af.Situacao<>?situacao")
                .Add("?numAutCartao", numAutCartao)
                .Add("?tipoCartao", tipoCartao)
                .Add("?situacao", Data.Model.AntecipacaoFornecedor.SituacaoAntecipFornec.Cancelada)
                .ExistsResult())
            {
                msgErro = "Já existe um pagto. antecip. de fornecedor com o mesmo número de autorização inserido.";
                return false;
            }

            if (SourceContext.Instance.CreateQuery()
                .From<Data.Model.PagtoLiberarPedido>("plp")
                    .InnerJoin<Data.Model.LiberarPedido>("plp.IdLiberarPedido=lp.IdLiberarPedido", "lp")
                .Where("plp.NumAutCartao=?numAutCartao AND plp.IdTipoCartao=?tipoCartao AND lp.Situacao<>?situacao")
                .Add("?numAutCartao", numAutCartao)
                .Add("?tipoCartao", tipoCartao)
                .Add("?situacao", Data.Model.LiberarPedido.SituacaoLiberarPedido.Cancelado)
                .ExistsResult())
            {
                msgErro = "Já existe uma liberação de pedido com o mesmo número de autorização inserido.";
                return false;
            }

            if (SourceContext.Instance.CreateQuery()
                .From<Data.Model.PagtoTrocaDevolucao>("ptd")
                    .InnerJoin<Data.Model.TrocaDevolucao>("ptd.IdTrocaDevolucao=td.IdTrocaDevolucao", "td")
                .Where("ptd.NumAutCartao=?numAutCartao AND ptd.IdTipoCartao=?tipoCartao AND td.Situacao<>?situacao")
                .Add("?numAutCartao", numAutCartao)
                .Add("?tipoCartao", tipoCartao)
                .Add("?situacao", Data.Model.TrocaDevolucao.SituacaoTrocaDev.Cancelada)
                .ExistsResult())
            {
                msgErro = "Já existe uma troca/devolução com o mesmo número de autorização inserido.";
                return false;
            }

            if (SourceContext.Instance.CreateQuery()
                .From<Data.Model.PagtoSinal>("ps")
                    .InnerJoin<Data.Model.Sinal>("ps.IdSinal=s.IdSinal", "s")
                .Where("ps.NumAutCartao=?numAutCartao AND ps.IdTipoCartao=?tipoCartao AND s.Situacao<>?situacao")
                .Add("?numAutCartao", numAutCartao)
                .Add("?tipoCartao", tipoCartao)
                .Add("?situacao", Data.Model.Sinal.SituacaoEnum.Cancelado)
                .ExistsResult())
            {
                msgErro = "Já existe um sinal com o mesmo número de autorização inserido.";
                return false;
            }

            if (SourceContext.Instance.CreateQuery()
                .From<Data.Model.PagtoObra>("po")
                    .InnerJoin<Data.Model.Obra>("po.IdObra=o.IdObra", "o")
                .Where("po.NumAutCartao=?numAutCartao AND po.IdTipoCartao=?tipoCartao AND o.Situacao<>?situacao")
                .Add("?numAutCartao", numAutCartao)
                .Add("?tipoCartao", tipoCartao)
                .Add("?situacao", Data.Model.Obra.SituacaoObra.Cancelada)
                .ExistsResult())
            {
                msgErro = "Já existe uma obra com o mesmo número de autorização inserido.";
                return false;
            }

            msgErro = "";
            return true;
        }

        /// <summary>
        /// Recupera todos os CNIs de débito que estão gerados sem movimentações bancárias.
        /// </summary>
        public IList<CartaoNaoIdentificado> ObterCartoesNaoIdentificadosDebitoSemMovimentacao()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.CartaoNaoIdentificado>()
                .Where("IdCartaoNaoIdentificado NOT IN (?sqlCNI) AND Situacao <> ?sitCancelado AND TipoCartao IN (?sqlTipoCartao)")
                .Add("?sqlCNI",
                    SourceContext.Instance.CreateQuery()
                        .From<Data.Model.MovBanco>()
                        .SelectDistinct("IdCartaoNaoIdentificado")
                        .Where("IdCartaoNaoIdentificado > 0"))
                .Add("?sitCancelado", Data.Model.SituacaoCartaoNaoIdentificado.Cancelado)
                .Add("?sqlTipoCartao", 
                    SourceContext.Instance.CreateQuery()
                        .From<Data.Model.TipoCartaoCredito>()
                        .Select("IdTipoCartao")
                        .Where("Tipo = 1"))
                .ToVirtualResultLazy<CartaoNaoIdentificado>();
        }
    }
}
