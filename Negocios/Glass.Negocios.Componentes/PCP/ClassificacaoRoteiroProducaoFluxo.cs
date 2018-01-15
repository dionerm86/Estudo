using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.PCP.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de nogócio da classificação do roteiro da produção 
    /// </summary>
    public class ClassificacaoRoteiroProducaoFluxo : IClassificacaoRoteiroProducaoFluxo, Entidades.IValidadorClassificacaoRoteiroProducao
    {
        /// <summary>
        /// Salva os dados da classificação
        /// </summary>
        /// <param name="classificacao"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarClassificacao(Entidades.ClassificacaoRoteiroProducao classificacao)
        {
            classificacao.Require("classificacao").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = classificacao.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados da Classificação.
        /// </summary>
        /// <param name="classificacao"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarClassificacao(Entidades.ClassificacaoRoteiroProducao classificacao)
        {
            classificacao.Require("classificacao").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = classificacao.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        /// <summary>
        /// Pequisa as classificações de roteiro do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.ClassificacaoRoteiroProducao> PesquisarClassificacao()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Glass.PCP.Data.Model.ClassificacaoRoteiroProducao>()
                .ToVirtualResultLazy<Entidades.ClassificacaoRoteiroProducao>();
        }

        /// <summary>
        /// Recupera os descritores das classificações de roteiro do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<IEntityDescriptor> ObtemClassificacao()
        {
            return SourceContext.Instance.CreateQuery()
               .From<Glass.PCP.Data.Model.ClassificacaoRoteiroProducao>()
               .OrderBy("Descricao")
               .ProcessResultDescriptor<Entidades.ClassificacaoRoteiroProducao>()
               .ToList();
        }

        /// <summary>
        /// Recupera os dados da classificação de roteiro.
        /// </summary>
        /// <param name="idClassificacao"></param>
        /// <returns></returns>
        public Entidades.ClassificacaoRoteiroProducao ObtemClassificacao(int idClassificacaoRoteiroProducao)
        {
            return SourceContext.Instance.CreateQuery()
               .From<Glass.PCP.Data.Model.ClassificacaoRoteiroProducao>()
               .Where("IdClassificacaoRoteiroProducao=?id")
               .Add("?id", idClassificacaoRoteiroProducao)
               .ProcessLazyResult<Entidades.ClassificacaoRoteiroProducao>()
               .FirstOrDefault();
        }

        /// <summary>
        /// Obtem a capacidade diária padrão de uma classificação
        /// </summary>
        /// <param name="idClassificacao"></param>
        /// <returns></returns>
        public int ObtemCapacidadeDiaria(int idClassificacao)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Glass.PCP.Data.Model.ClassificacaoRoteiroProducao>()
               .Where("IdClassificacaoRoteiroProducao=?id")
               .Add("?id", idClassificacao)
               .Select("CapacidadeDiaria")
               .Execute()
               .Select(f => f.GetInt32(0))
               .FirstOrDefault();
        }

        /// <summary>
        /// Obtem a descricao de uma classificação
        /// </summary>
        /// <param name="idClassificacao"></param>
        /// <returns></returns>
        public string ObtemDescricao(int idClassificacao)
        {
            return SourceContext.Instance.CreateQuery()
               .From<Glass.PCP.Data.Model.ClassificacaoRoteiroProducao>("crp")
               .Where("crp.IdClassificacaoRoteiroProducao=?id")
               .Add("?id", idClassificacao)
               .Select("crp.Descricao as Descricao")
               .Execute()
               .Select(f=>f.GetString(0))
               .FirstOrDefault();
        }

        /// <summary>
        ///  Obtem a descricao de uma classificação
        /// </summary>
        /// <param name="idsClassificacoes"></param>
        /// <returns></returns>
        public List<Entidades.ClassificacaoRoteiroProducao> ObtemClassificacoes(string idsClassificacoes)
        {
            return !String.IsNullOrEmpty(idsClassificacoes) ? SourceContext.Instance.CreateQuery()
               .From<Glass.PCP.Data.Model.ClassificacaoRoteiroProducao>("crp")
               .Where("crp.IdClassificacaoRoteiroProducao IN ("+idsClassificacoes+")")
               .ProcessResult<Entidades.ClassificacaoRoteiroProducao>()
               .ToList() : new List<Entidades.ClassificacaoRoteiroProducao>();
        }

        #region IValidadorClassificacaoRoteiroProducao Members

        /// <summary>
        /// Valida a existencia do dados da cor do vidro.
        /// </summary>
        /// <returns></returns>
        public IMessageFormattable[] ValidaExistencia(Entidades.ClassificacaoRoteiroProducao classificacao)
        {
            var mensagens = new List<string>();

            // Handler para criar a consulta padrão da existencia do registro.
            var criarConsulta = new Func<Type, Colosoft.Query.Queryable>(type =>
                SourceContext.Instance.CreateQuery()
                .From(new Colosoft.Query.EntityInfo(type.FullName))
                .Count()
                .Where("IdClassificacaoRoteiroProducao=?id")
                .Add("?id", classificacao.IdClassificacaoRoteiroProducao));

            // Handler para tratar o resultado da consulta de validação.
            var tratarResultado = new Func<string, Colosoft.Query.QueryCallBack>(mensagem =>
               (sender, query, result) =>
               {
                   if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0 &&
                       !mensagens.Contains(mensagem))
                       mensagens.Add(mensagem);
               });
            
            var consultas = SourceContext.Instance.CreateMultiQuery()

                // Verifica se a classificação está associada a algum subgrupo de produto.
                .Add(criarConsulta(typeof(Glass.Data.Model.ClassificacaoSubgrupo)),
                    tratarResultado("Esta classificação não pode ser excluída pois existem subgrupos associados a ela. Desassocie-os e tente novamente."))

                // Verifica se a classificação está associada a algum roteiro de produção.
                .Add(criarConsulta(typeof(Glass.Data.Model.RoteiroProducao)),
                    tratarResultado("Esta classificação não pode ser excluída pois existem roteiros associados a ela. Desassocie-os e tente novamente."));

            consultas.Execute();

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        #endregion
    }
}