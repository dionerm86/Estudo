using Colosoft;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Global.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio da Origem Troca/Desconto.
    /// </summary>
    public class OrigemTrocaDescontoFluxo : IOrigemTrocaDescontoFluxo, Entidades.IValidadorOrigemTrocaDesconto
    {
        /// <summary>
        /// Pesquisa as origens de Troca/Desconto.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.OrigemTrocaDescontoPesquisa> PesquisarOrigensTrocaDesconto()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.OrigemTrocaDesconto>("p")
                .Select("p.IdOrigemTrocaDesconto, p.Descricao, p.Situacao, ?qtde AS QtdeContasReceber")
                .Add("?qtde", SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ContasReceber>("cr")
                    .Where("IdOrigemDescontoAcrescimo=p.IdOrigemTrocaDesconto")
                    .Count())
                .OrderBy("Descricao")
                .ToVirtualResultLazy<Entidades.OrigemTrocaDescontoPesquisa>();
        }

        /// <summary>
        /// Recupera os descritores das origens de Troca/Desconto.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemOrigensTrocaDesconto()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.OrigemTrocaDesconto>()
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.OrigemTrocaDesconto>()
                .ToList();
        }

        /// <summary>
        /// Recupera a origem de Troca/Desconto.
        /// </summary>
        /// <param name="idOrigemTrocaDesconto"></param>
        /// <returns></returns>
        public Entidades.OrigemTrocaDesconto ObtemOrigemTrocaDesconto(int idOrigemTrocaDesconto)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.OrigemTrocaDesconto>()
                .Where("IdOrigemTrocaDesconto = ?id")
                .Add("?id", idOrigemTrocaDesconto)
                .ProcessLazyResult<Entidades.OrigemTrocaDesconto>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados da origem.
        /// </summary>
        /// <param name="origemTrocaDesconto"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarOrigemTrocaDesconto(Entidades.OrigemTrocaDesconto origemTrocaDesconto)
        {
            origemTrocaDesconto.Require("origemTrocaDesconto").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = origemTrocaDesconto.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados da origem.
        /// </summary>
        /// <param name="origemTrocaDesconto"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarOrigemTrocaDesconto(Entidades.OrigemTrocaDesconto origemTrocaDesconto)
        {
            origemTrocaDesconto.Require("origemTrocaDesconto").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = origemTrocaDesconto.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        /// <summary>
        /// Valida a origem de Troca/Devolução.
        /// </summary>
        /// <param name="origemTrocaDesconto"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IValidadorOrigemTrocaDesconto.ValidaExistencia(Entidades.OrigemTrocaDesconto origemTrocaDesconto)
        {
            var mensagens = new List<string>();

            // Handler para tratar o resultado da consulta de validação
            var tratarResultado = new Func<string, Colosoft.Query.QueryCallBack>(mensagem =>
               (sender, query, result) =>
               {
                   if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0 &&
                       !mensagens.Contains(mensagem))
                       mensagens.Add(mensagem);
               });

            var consulta = SourceContext.Instance.CreateMultiQuery();

            var adicionaConsulta = new Action<Type, string, string, char>((tipo, nomeCampo, nome, genero) =>
            {
                consulta.Add(SourceContext.Instance.CreateQuery()
                    .From(new Colosoft.Query.EntityInfo(tipo.FullName))
                    .Where(String.Format("{0}=?id", nomeCampo))
                    .Add("?id", origemTrocaDesconto.IdOrigemTrocaDesconto)
                    .Count(),

                    tratarResultado(String.Format(
                        "Esta origem de troca/desconto não pode ser excluída por possuir {0} relacionad{1}s à mesma.",
                        nome, genero)));
            });

            adicionaConsulta(typeof(Data.Model.ContasReceber), "IdOrigemDescontoAcrescimo", "contas a receber", 'a');
            adicionaConsulta(typeof(Data.Model.TrocaDevolucao), "IdOrigemTrocaDevolucao", "trocas/devoluções", 'a');

            consulta.Execute();

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }
    }
}
