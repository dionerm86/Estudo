using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.Fiscal.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio do CTe.
    /// </summary>
    public class CTeFluxo : ICTeFluxo, Entidades.Cte.IValidadorChaveAcesso
    {
        #region Seguradora

        /// <summary>
        /// Pesquisa as seguradoras.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.Seguradora> PesquisarSeguradoras()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Cte.Seguradora>()
                .OrderBy("NomeSeguradora")
                .ToVirtualResultLazy<Entidades.Seguradora>();
        }

        /// <summary>
        /// Obtem os descritores das seguradoras.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemSeguradoras()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Cte.Seguradora>()
                .OrderBy("NomeSeguradora")
                .ProcessResultDescriptor<Entidades.Seguradora>()
                .ToList();
        }

        /// <summary>
        /// Recupera os dados da seguradora.
        /// </summary>
        /// <param name="idSeguradora"></param>
        /// <returns></returns>
        public Entidades.Seguradora ObtemSeguradora(int idSeguradora)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Cte.Seguradora>()
                .Where("IdSeguradora=?id")
                .Add("?id", idSeguradora)
                .ProcessLazyResult<Entidades.Seguradora>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados da seguradora.
        /// </summary>
        /// <param name="seguradora"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarSeguradora(Entidades.Seguradora seguradora)
        {
            seguradora.Require("seguradora").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = seguradora.Save(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados da seguradora.
        /// </summary>
        /// <param name="seguradora"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarSeguradora(Entidades.Seguradora seguradora)
        {
            seguradora.Require("seguradora").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = seguradora.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #endregion

        #region Chave de Acesso

        /// <summary>
        /// Pesquisa as chaves de acesso de um CT-e
        /// </summary>
        /// <param name="idCte"></param>
        /// <param name="pageSize"></param>
        /// <param name="sortExpression"></param>
        /// <returns></returns>
        public IList<Entidades.Cte.ChaveAcessoCte> PesquisarChavesAcesso(int idCte)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.Cte.ChaveAcessoCte>()
                .Select("IdChaveAcessoCte, IdCte, ChaveAcesso, PIN");

            if (idCte > 0)
                consulta.WhereClause
                    .And("IdCte=?id").Add("?id", idCte);

            return consulta.ToVirtualResult<Entidades.Cte.ChaveAcessoCte>();
        }

        /// <summary>
        /// Recupera os dados de uma chave de acesso.
        /// </summary>
        /// <param name="IdChaveAcessoCte"></param>
        /// <returns></returns>
        public Entidades.Cte.ChaveAcessoCte ObtemChaveAcesso(int IdChaveAcessoCte)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Cte.ChaveAcessoCte>()
                .Where("IdChaveAcessoCte=?id")
                .Add("?id", IdChaveAcessoCte)
                .ProcessLazyResult<Entidades.Cte.ChaveAcessoCte>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva a chave de acesso de um CT-e
        /// </summary>
        /// <param name="chaveAcessoCte"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarChaveAcesso(Entidades.Cte.ChaveAcessoCte chaveAcessoCte)
        {
            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = chaveAcessoCte.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga uma chave de acesso do CT-e do sistema
        /// </summary>
        /// <param name="idChaveAcessoCte"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarChaveAcesso(int idChaveAcessoCte)
        {
            var chaveAcesso = ObtemChaveAcesso(idChaveAcessoCte);

            if (chaveAcesso == null)
                return new Colosoft.Business.DeleteResult(false, "Chave de acesso não encontrada".GetFormatter());

            return ApagarChaveAcesso(chaveAcesso);
        }

        /// <summary>
        /// Apaga uma chave de acesso do CT-e do sistema
        /// </summary>
        /// <param name="chaveAcessoCte"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarChaveAcesso(Entidades.Cte.ChaveAcessoCte chaveAcessoCte)
        {
            chaveAcessoCte.Require("chaveAcessoCte").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = chaveAcessoCte.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #endregion

        #region Membros de IValidadorChaveAcesso

        /// <summary>
        /// Valida a atualização da chave de acesso.
        /// </summary>
        /// <param name="chaveAcessoCte"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.Cte.IValidadorChaveAcesso.ValidaAtualizacao(Entidades.Cte.ChaveAcessoCte chaveAcessoCte)
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

            SourceContext.Instance.CreateMultiQuery()
                //Verifica se o ct-e informado esta na situação aberto
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.Cte.ConhecimentoTransporte>()
                    .Where("IdCte=?id AND Situacao <> ?sit")
                    .Add("?sit", Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Aberto)
                    .Add("?id", chaveAcessoCte.IdCte)
                    .Count(),
                    tratarResultado("O CT-e informado não esta na situação aberto."))
                //Verifica se o ct-e informado é de saída
                .Add(SourceContext.Instance.CreateQuery()
                   .From<Glass.Data.Model.Cte.ConhecimentoTransporte>()
                    .Where("IdCte=?id AND TipoDocumentoCte <> ?tipo")
                    .Add("?tipo", Glass.Data.Model.Cte.ConhecimentoTransporte.TipoDocumentoCteEnum.Saida)
                    .Add("?id", chaveAcessoCte.IdCte)
                    .Count(),
                    tratarResultado("O CT-e informado não é de saída."))
                .Execute();

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        #endregion
    }
}
