using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.Financeiro.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de parcelas.
    /// </summary>
    public class ParcelasFluxo :
        IParcelasFluxo, Entidades.IValidadorParcelas, Entidades.IProvedorParcelasNaoUsar
    {
        #region Parcelas

        /// <summary>
        /// Pesquisa as parcelas.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.Parcelas> PesquisarParcelas()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Parcelas>()
                .OrderBy("Descricao")
                .ToVirtualResultLazy<Entidades.Parcelas>();
        }

        /// <summary>
        /// Recupera os descritores das parcelas.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemParcelas()
        {
            return SourceContext.Instance.CreateQuery()
               .From<Data.Model.Parcelas>()
               .OrderBy("Descricao")
               .Where("Situacao=?ativo")
               .Add("?ativo", Situacao.Ativo)
               .ProcessResultDescriptor<Entidades.Parcelas>()
               .ToList();
        }

        /// <summary>
        /// Recupera os dados da parcela.
        /// </summary>
        /// <param name="idParcela"></param>
        /// <returns></returns>
        public Entidades.Parcelas ObtemParcela(int idParcela)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Parcelas>()
                .Where("IdParcela=?idParcela")
                .Add("?idParcela", idParcela)
                .ProcessLazyResult<Entidades.Parcelas>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados da parcela.
        /// </summary>
        /// <param name="parcela"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarParcela(Entidades.Parcelas parcela)
        {
            parcela.Require("parcela").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = parcela.Save(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados da parcela.
        /// </summary>
        /// <param name="parcela"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarParcela(Entidades.Parcelas parcela)
        {
            parcela.Require("parcela").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = parcela.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        /// <summary>
        /// Valida a existencia da parcela.
        /// </summary>
        /// <param name="parcelas"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IValidadorParcelas.ValidaExistencia(Entidades.Parcelas parcelas)
        {
            var mensagens = new List<string>();

            // Handler para tratar o resultado da consulta de validação
            var tratarResultado = new Func<string, Colosoft.Query.QueryCallBack>(mensagem =>
               (sender, query, result) =>
               {
                   if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0 &&
                       !mensagens.Contains(mensagem))
                       mensagens.Add(mensagem );
               });

            var consulta = SourceContext.Instance.CreateMultiQuery();

            var adicionaConsulta = new Action<Type, string, string, char>((tipo, nomeCampo, nome, genero) =>
            {
            consulta.Add(SourceContext.Instance.CreateQuery()
                .From(new Colosoft.Query.EntityInfo(tipo.FullName))
                .Where(String.Format("{0}=?id", nomeCampo))
                .Add("?id", parcelas.IdParcela)
                .Count(),

                    tratarResultado(String.Format(
                        "Esta parcela não pode ser excluída por possuir {0} relacionad{1}s à mesma. Referencia: {2}.",  
                        nome, genero, nomeCampo)));
            });

            adicionaConsulta(typeof(Data.Model.Cliente), "TipoPagto", "clientes", 'o');
            adicionaConsulta(typeof(Data.Model.Fornecedor), "TipoPagto", "fornecedores", 'o');
            adicionaConsulta(typeof(Data.Model.LiberarPedido), "IdParcela", "liberações de pedidos", 'a');
            adicionaConsulta(typeof(Data.Model.ProdutoFornecedorCotacaoCompra), "IdParcela", "cotações de compras", 'a');
            adicionaConsulta(typeof(Data.Model.Pedido), "IdParcela", "pedidos", 'o');

            consulta.Execute();

            #region Parcelas não usar cliente

            var quantidadeParcelasCliente =
                SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ParcelasNaoUsar>()
                    .Where("IdParcela=?idParcela AND IdCliente IS NOT NULL")
                    .Add("?idParcela", parcelas.IdParcela)
                    .Execute().Count();

            var quantidadeCliente =
                SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Cliente>()
                    .Execute().Count();

            if (quantidadeParcelasCliente != quantidadeCliente)
                tratarResultado("Esta parcela não pode ser excluída por possuir restrições de parcela por cliente relacionados à mesma.");

            #endregion

            #region Parcelas não usar fornecedor

            var quantidadeParcelasFornecedor =
                SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ParcelasNaoUsar>()
                    .Where("IdParcela=?idParcela AND IdFornecedor IS NOT NULL")
                    .Add("?idParcela", parcelas.IdParcela)
                    .Execute().Count();

            var quantidadeFornecedor =
                SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Fornecedor>()
                    .Execute().Count();

            if (quantidadeParcelasFornecedor != quantidadeFornecedor)
                tratarResultado("Esta parcela não pode ser excluída por possuir restrições de parcela por fornecedor relacionados à mesma.");

            #endregion

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        /// <summary>
        /// Recupera a identificação da parcela;
        /// </summary>
        /// <param name="idParcela"></param>
        /// <returns></returns>
        string Entidades.IProvedorParcelasNaoUsar.ObtemIdentificacao(int idParcela)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Parcelas>()
                .Where("IdParcela=?id")
                .Add("?id", idParcela)
                .Select("Descricao")
                .Execute()
                .Select(f => f.GetString(0))
                .FirstOrDefault();
        }


        #endregion

        #region Membros de IProvedorParcelasNaoUsar

        /// <summary>
        /// Crias as parcelas que não devem ser usadas associadas a parcela informada.
        /// </summary>
        /// <param name="parcela"></param>
        /// <returns></returns>
        IEnumerable<Entidades.ParcelasNaoUsar> Entidades.IProvedorParcelasNaoUsar.CriarParcelasNaoUsar(Entidades.Parcelas parcela)
        {
            var resultado = new List<Entidades.ParcelasNaoUsar>();

            SourceContext.Instance.CreateMultiQuery()
                // Consulta os clientes para criar a associação
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Cliente>()
                    .Select("IdCli"),
                    (sender, query, result) =>
                        resultado.AddRange(result.Select(f =>
                            new Entidades.ParcelasNaoUsar
                            {
                                IdParcela = parcela.IdParcela,
                                IdCliente = f.GetInt32(0)
                            })))
                // Consulta os fornecedores para criar a associação
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Fornecedor>()
                    .Select("IdFornec"),
                    (sender, query, result) =>
                        resultado.AddRange(result.Select(f =>
                            new Entidades.ParcelasNaoUsar
                            {
                                IdParcela = parcela.IdParcela,
                                IdFornecedor = f.GetInt32(0)
                            })))
                .Execute();

            return resultado;
        }

        #endregion

        #region Membros de Validador da Parcela
        IMessageFormattable[] Entidades.IValidadorParcelas.ValidaSituacao(int idParcela)
        {
            var parcela = ObtemParcela(idParcela);

            var idCliente =  SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ParcelasNaoUsar>()
                    .Where("IdParcela=?idParcela AND IdCliente IS NOT NULL")
                    .Add("?idParcela", idParcela)
                    .Select("IdCliente")
                    .Execute()
                    .Select(f => f.GetInt32(0)).ToList();

            if (idCliente.HasItems() && idCliente.FirstOrDefault() > 0)
            {
                var cliente = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Cliente>()
                    .Where(string.Format("IdCli NOT IN ({0}) AND Situacao=?situacao", string.Join(",", idCliente)))
                    .Add("?situacao", Situacao.Ativo)
                    .Select("IdCli")
                    .Execute()
                    .Select(f => f.GetInt32(0)).ToList();

                if (cliente.HasItems() && cliente.FirstOrDefault() > 0)
                    return new IMessageFormattable[]
               {
                    string.Format("Esta Parecela não pode ser inativada pois existe clientes usando a mesma. Desassocie a parcela dos clientes antes de inativá-la." +
                    "Código dos Clientes: {0}.", string.Join(", ", cliente)).GetFormatter()
               };
            }
            else
            {
                return new IMessageFormattable[]
                {
                    "Todos os clientes possuem essa parcela. Desassocia-a de todos os clientes antes de inativá-la.".GetFormatter()
                };
            }
            return new IMessageFormattable[0];

        }

        #endregion
    }
}
