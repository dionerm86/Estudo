using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.PCP.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de perda.
    /// </summary>
    public class PerdaFluxo : IPerdaFluxo, Entidades.IValidadorTipoPerda, Entidades.IValidadorSubtipoPerda
    {
        #region Tipo Perda

        /// <summary>
        /// Pesquisa os tipos de perda.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.TipoPerdaPesquisa> PesquisarTiposPerda()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.TipoPerda>("tp")
                .LeftJoin<Glass.Data.Model.Setor>("tp.IdSetor = s.IdSetor", "s")
                .Select("tp.IdTipoPerda, tp.Descricao, tp.IdSetor, s.Descricao AS Setor, tp.Situacao, tp.ExibirPainelProducao")
                .OrderBy("Descricao")
                .ToVirtualResultLazy<Entidades.TipoPerdaPesquisa>();
        }

        /// <summary>
        /// Recupera os dados do tipo de perda.
        /// </summary>
        /// <param name="idTipoPerda"></param>
        /// <returns></returns>
        public Entidades.TipoPerda ObtemTipoPerda(int idTipoPerda)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.TipoPerda>()
                .Where("IdTipoPerda=?idTipoPerda")
                .Add("?idTipoPerda", idTipoPerda)
                .ProcessLazyResult<Entidades.TipoPerda>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Obtem os tipos de perda disponíveis para um setor específico
        /// </summary>
        public List<Colosoft.IEntityDescriptor> ObterPeloSetor(int idSetor)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.TipoPerda>("tp")
                .LeftJoin<Glass.Data.Model.Setor>("tp.IdSetor=s.IdSetor", "s")
                .Where("tp.Situacao=?situacao")
                .Add("?situacao", Glass.Data.Model.SituacaoTipoPerda.Ativo)
                .OrderBy("Descricao");

            if (idSetor > 0)
                consulta.WhereClause
                .And("s.IdSetor=?idSetor OR ISNULL(s.IdSetor, 0) = 0")
                .Add("?idSetor", idSetor);

            return consulta
             .ProcessResultDescriptor<Entidades.TipoPerda>().ToList();
        }

        /// <summary>
        /// Salva o tipo de perda.
        /// </summary>
        /// <param name="tipoPerda">Instancia com os dado que serão salvos.</param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarTipoPerda(Entidades.TipoPerda tipoPerda)
        {
            tipoPerda.Require("tipoPerda").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = tipoPerda.Save(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados do tipo de perda.
        /// </summary>
        /// <param name="tipoPerda"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarTipoPerda(Entidades.TipoPerda tipoPerda)
        {
            tipoPerda.Require("tipoPerda").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = tipoPerda.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #endregion

        #region Subtipo Perda

        /// <summary>
        /// Pesquisa os subtipos de perda associados com a perda informada.
        /// </summary>
        /// <param name="idTipoPerda">Identificador da perda pai.</param>
        /// <returns></returns>
        public IList<Entidades.SubtipoPerda> PesquisarSubtiposPerda(int idTipoPerda)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.SubtipoPerda>()
                .Where("IdTipoPerda=?idTipoPerda")
                .Add("?idTipoPerda", idTipoPerda)
                .ToVirtualResultLazy<Entidades.SubtipoPerda>();
        }

        /// <summary>
        /// Recupera os descritores dos subtipos de perda associados com o tipo da perda.
        /// </summary>
        /// <param name="idTipoPerda">Identificador do tipo da perda.</param>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemSubtiposPerda(int idTipoPerda)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.SubtipoPerda>()
                .Where("IdTipoPerda=?idTipoPerda")
                .Add("?idTipoPerda", idTipoPerda)
                .ProcessResultDescriptor<Entidades.SubtipoPerda>()
                .ToList();
        }

        /// <summary>
        /// Recupera os dados do subtipo de perda.
        /// </summary>
        /// <param name="idSubtipoPerda">Identificador do subtipo.</param>
        /// <returns></returns>
        public Entidades.SubtipoPerda ObtemSubtipoPerda(int idSubtipoPerda)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.SubtipoPerda>()
                .Where("IdSubtipoPerda=?idSubtipoPerda")
                .Add("?idSubtipoPerda", idSubtipoPerda)
                .ProcessLazyResult<Entidades.SubtipoPerda>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva o subtipo de perda.
        /// </summary>
        /// <param name="subtipoPerda"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarSubtipoPerda(Entidades.SubtipoPerda subtipoPerda)
        {
            subtipoPerda.Require("subtipoPerda").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = subtipoPerda.Save(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga o subtipo de perda.
        /// </summary>
        /// <param name="subtipoPerda"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarSubtipoPerda(Entidades.SubtipoPerda subtipoPerda)
        {
            subtipoPerda.Require("subtipoPerda").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = subtipoPerda.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #endregion

        #region IValidadorTipoPerda Members

        /// <summary>
        /// Valida a existencia do tipo de perda.
        /// </summary>
        /// <param name="tipoPerda"></param>
        /// <returns></returns>
        public IMessageFormattable[] ValidaExistencia(Entidades.TipoPerda tipoPerda)
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
                    .Add("?id", tipoPerda.IdTipoPerda)
                    .Count(),

                    tratarResultado(String.Format(
                        "Este tipo de perda não pode ser excluído por possuir {0} relacionad{1}s ao mesmo.",
                        nome, genero)));
            });

            adicionaConsulta(typeof(Glass.Data.Model.TrocaDevolucao), "IdTipoPerda", "trocas/devoluções", 'o');
            adicionaConsulta(typeof(Glass.Data.Model.PerdaChapaVidro), "IdTipoPerda", "perdas de chapa de vidro", 'a');
            adicionaConsulta(typeof(Glass.Data.Model.SubtipoPerda), "IdTipoPerda", "subtipos de perda", 'o');
            adicionaConsulta(typeof(Glass.Data.Model.DadosReposicao), "TipoPerdaRepos", "reposições de peça anteriores", 'a');
            adicionaConsulta(typeof(Glass.Data.Model.ProdutoPedidoProducao), "TipoPerda", "reposições de peça (pedido)", 'a');
            adicionaConsulta(typeof(Glass.Data.Model.ProdutoPedidoProducao), "TipoPerdaRepos", "reposições de peça", 'a');

            consulta.Execute();

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        #endregion

        #region IValidadorSubtipoPerda Members

        /// <summary>
        /// Valida a existencia do subtipo de perda.
        /// </summary>
        /// <param name="subtipoPerda"></param>
        /// <returns></returns>
        public IMessageFormattable[] ValidaExistencia(Entidades.SubtipoPerda subtipoPerda)
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
                    .Add("?id", subtipoPerda.IdSubtipoPerda)
                    .Count(),

                    tratarResultado(String.Format(
                        "Este subtipo de perda não pode ser excluído por possuir {0} relacionad{1}s ao mesmo.",
                        nome, genero)));
            });

            adicionaConsulta(typeof(Glass.Data.Model.TrocaDevolucao), "IdSubtipoPerda", "trocas/devoluções", 'o');
            adicionaConsulta(typeof(Glass.Data.Model.PerdaChapaVidro), "IdSubTipoPerda", "perdas de chapa de vidro", 'a');
            adicionaConsulta(typeof(Glass.Data.Model.DadosReposicao), "IdSubtipoPerdaRepos", "reposições de peça anteriores", 'a');
            adicionaConsulta(typeof(Glass.Data.Model.ProdutoPedidoProducao), "IdSubtipoPerda", "reposições de peça (pedido)", 'a');
            adicionaConsulta(typeof(Glass.Data.Model.ProdutoPedidoProducao), "IdSubtipoPerdaRepos", "reposições de peça", 'a');

            consulta.Execute();

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        #endregion
    }
}
