using System.Collections.Generic;
using System.Linq;
using Colosoft;
using System;

namespace Glass.PCP.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio dos setores do sistema.
    /// </summary>
    public class SetorFluxo : ISetorFluxo,
        Entidades.IProvedorSetor
    {
        #region Setor

        /// <summary>
        /// Pequisa os setores do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.Setor> PesquisarSetores()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.Setor>()
                .OrderBy("NumeroSequencia")
                .ToVirtualResultLazy<Entidades.Setor>();
        }

        /// <summary>
        /// Recupera os descritores dos setores do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.SetorDescritor> ObtemSetores()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.Setor>()
                .OrderBy("NumeroSequencia")
                .Select("IdSetor, Descricao, Tipo")
                .ProcessResultDescriptor<Entidades.Setor>()
                .Select(f => (Entidades.SetorDescritor)f)
                .ToList();
        }

        /// <summary>
        /// Recupera os dados do setor.
        /// </summary>
        /// <param name="idSetor"></param>
        /// <returns></returns>
        public Entidades.Setor ObtemSetor(int idSetor)
        {
            return SourceContext.Instance.CreateQuery()
               .From<Glass.Data.Model.Setor>()
               .Where("IdSetor=?id")
               .Add("?id", idSetor)
               .ProcessLazyResult<Entidades.Setor>()
               .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados do setor.
        /// </summary>
        /// <param name="setor"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarSetor(Entidades.Setor setor)
        {
            setor.Require("setor").NotNull();

            if (!setor.ExistsInStorage)
            {
                var numeroSequencia = SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.Setor>()
                    .Select("MAX(NumeroSequencia)")
                    .Execute()
                    .Select(f => f.GetInt32(0))
                    .FirstOrDefault() + 1;

                // Gera um novo número de sequência para este novo setor
                setor.NumeroSequencia = numeroSequencia;
            }

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = setor.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        private IMessageFormattable[] ValidarExclusaoSetor(int idSetor)
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
                // Verifica se existem peças que foram inseridas neste setor.
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.LeituraProducao>()
                    .Where("IdSetor=?id")
                    .Add("?id", idSetor)
                    .Count(),
                    tratarResultado("Este Setor não pode ser excluído por haverem peças relacionadas ao mesmo."))

                // Verifica se existem reposições relacionadas ao setor.
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.DadosReposicao>()
                    .Where("IdSetorRepos=?id")
                    .Add("?id", idSetor)
                    .Count(),
                    tratarResultado("Este setor não pode ser excluído por haverem reposições relacionadas ao mesmo."))

                // Verifica se existem funcionários relacionados ao setor.
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.FuncionarioSetor>()
                    .Where("IdSetor=?id")
                    .Add("?id", idSetor)
                    .Count(),
                    tratarResultado("Este setor não pode ser excluído por haverem funcionários relacionados ao mesmo."))

                // Verifica se existem notificações relacionadas ao setor.
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.Notificacao>()
                    .Where("IdSetor=?id")
                    .Add("?id", idSetor)
                    .Count(),
                    tratarResultado("Este setor não pode ser excluído por haverem notificações relacionadas ao mesmo."))

                // Verifica se existem peças excluídas do sistema relacionadas ao setor.
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.PecasExcluidasSistema>()
                    .Where("IdSetor=?id")
                    .Add("?id", idSetor)
                    .Count(),
                    tratarResultado("Este setor não pode ser excluído por haverem peças excluídas do sistema associadas ao mesmo."))

                // Verifica se existem peças de pedidos em produção associadas ao setor.
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.ProdutoPedidoProducao>()
                    .Where("IdSetor=?id OR IdSetorRepos=?id")
                    .Add("?id", idSetor)
                    .Count(),
                    tratarResultado("Este setor não pode ser excluído por haverem peças em produção associadas ao mesmo."))

                // Verifica se existem etiquetas de roteiro de produção associadas ao setor.
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.RoteiroProducaoEtiqueta>()
                    .Where("IdSetor=?id")
                    .Add("?id", idSetor)
                    .Count(),
                    tratarResultado("Este setor não pode ser excluído por haverem etiquetas de roterios de produção associadas ao mesmo."))

                // Verifica se existem roteiros de produção associados ao setor.
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.RoteiroProducaoSetor>()
                    .Where("IdSetor=?id")
                    .Add("?id", idSetor)
                    .Count(),
                    tratarResultado("Este setor não pode ser excluído por haverem roteiros de produção associados ao mesmo."))

                // Verifica se existem beneficiamentos associados ao setor.
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.SetorBenef>()
                    .Where("IdSetor=?id")
                    .Add("?id", idSetor)
                    .Count(),
                    tratarResultado("Este setor não pode ser excluído por haverem beneficiamentos associados ao mesmo."))

                // Verifica se existem tipos de perda associados ao setor.
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.TipoPerda>()
                    .Where("IdSetor=?id")
                    .Add("?id", idSetor)
                    .Count(),
                    tratarResultado("Este setor não pode ser excluído por haverem tipos de perda associadas ao mesmo."))

                // Verifica se existe trocas/devoluções asssociadas ao setor.
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.TrocaDevolucao>()
                    .Where("IdSetor=?id")
                    .Add("?id", idSetor)
                    .Count(),
                    tratarResultado("Este setor não pode ser excluído por haverem trocas/devoluções associadas ao mesmo."))

                .Execute();

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        /// <summary>
        /// Apaga os dados do setor.
        /// </summary>
        /// <param name="setor"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarSetor(Entidades.Setor setor)
        {
            setor.Require("setor").NotNull();

            var mensagensValidacao = ValidarExclusaoSetor(setor.IdSetor);

            if (mensagensValidacao.Any())
            {
                return new Colosoft.Business.DeleteResult(false, mensagensValidacao.Join("\n"));
            }

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = setor.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        /// <summary>
        /// Método usado para alterar a posição do setor.
        /// </summary>
        /// <param name="idSetor"></param>
        /// <param name="acima"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult AlterarPosicao(int idSetor, bool acima)
        {
            var setor = SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.Setor>()
                .Where("IdSetor=?id").Add("?id", idSetor)
                .ProcessLazyResult<Entidades.Setor>()
                .FirstOrDefault();

            var maiorSequencia = SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.Setor>()
                .Select("MAX(NumeroSequencia)")
                .Execute()
                .Select(f => f.GetInt32(0)).FirstOrDefault();


            // Só troca de posição se houver algum setor abaixo/acima deste para ser trocado,
            // lembrando que a posição Impr. Etiqueta não pode ser trocada
            if (setor.NumeroSequencia == 1 || (acima && setor.NumeroSequencia == 2) ||
                (!acima && maiorSequencia == setor.NumeroSequencia))
                return new Colosoft.Business.SaveResult(true, null);


            var destino = SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.Setor>()
                .Where("NumeroSequencia" + (acima ? "<" : ">") + "?num")
                .Add("?num", setor.NumeroSequencia)
                .OrderBy("NumeroSequencia " + (acima ? "Desc" : "Asc"))
                .Take(1)
                .ProcessLazyResult<Entidades.Setor>()
                .FirstOrDefault();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var numSeqNovo = destino.NumeroSequencia;

                // Altera a posição do setor adjacente à este
                destino.NumeroSequencia = setor.NumeroSequencia;

                // Altera a posição deste setor
                setor.NumeroSequencia = numSeqNovo;

                var resultado = setor.Save(session);
                if (!resultado)
                    return resultado;

                resultado = destino.Save(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();

                // Recarrega listagem de setores
                //Utils.GetSetores = SetorDAO.Instance.GetOrdered();
            }
        }

        #endregion

        #region IProvedorSetor

        /// <summary>
        /// Valida a atualização dos dados do setor.
        /// </summary>
        /// <param name="setor"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IProvedorSetor.ValidaAtualizacao(Entidades.Setor setor)
        {
            if (setor.ExistsInStorage)
            {
                //Verifica Situação do setor antes de alterá-lo pois caso caso a alteração for somente no numseq o sistema não precisa verificar se está inativo novamente
                var setorAtual = SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.Setor>()
                    .Where("IdSetor=?id").Add("?id", setor.IdSetor)
                    .ProcessLazyResult<Entidades.Setor>()
                    .FirstOrDefault();

                if (setorAtual.Situacao != Situacao.Inativo && setor.Situacao == Situacao.Inativo)
                {
                    if (SourceContext.Instance.CreateQuery()
                        .From<Glass.Data.Model.FuncionarioSetor>("fs")
                        .Where("fs.IdSetor=?id")
                            .Add("?id", setor.IdSetor)
                        .GroupBy("fs.IdSetor")
                        .ExistsResult())
                        return new IMessageFormattable[]
                        {
                            (string.Format("O setor {0} não pode ser inativado, pois, ele está associado à funcionário(s) de produção. " +
                                "Para inativá-lo, desassocie o setor do funcionário.", setor.Descricao)).GetFormatter()
                        };

                    /* Chamado 51830. */
                    if (SourceContext.Instance.CreateQuery()
                        .From<Glass.Data.Model.ProdutoPedidoProducao>("ppp")
                        .Where("ppp.IdSetor=?id AND ppp.Situacao=?situacaoProdutoProducao")
                            .Add("?id", setor.IdSetor)
                            .Add("?situacaoProdutoProducao", (int)Glass.Data.Model.ProdutoPedidoProducao.SituacaoEnum.Producao)
                        .GroupBy("ppp.IdSetor")
                        .ExistsResult())
                        return new IMessageFormattable[]
                        {
                            (string.Format("O setor {0} não pode ser inativado, pois, ele está associado à etiqueta(s) de produção. " +
                                "Para inativá-lo, retire a(s) etiqueta(s) deste setor.", setor.Descricao)).GetFormatter()
                        };

                    /* Chamado 56684. */
                    if (SourceContext.Instance.CreateQuery()
                        .From<Glass.Data.Model.RoteiroProducaoSetor>("rps")
                        .Where("rps.IdSetor=?id")
                            .Add("?id", setor.IdSetor)
                        .GroupBy("rps.IdSetor")
                        .ExistsResult())
                        return new IMessageFormattable[]
                        {
                            (string.Format("O setor {0} não pode ser inativado, pois, ele está associado à roteiro(s) de produção. " +
                                "Para inativá-lo, retire este setor do(s) roteiro(s) de produção.", setor.Descricao)).GetFormatter()
                        };

                    /* Chamado 56684. */
                    if (SourceContext.Instance.CreateQuery()
                        .From<Glass.Data.Model.RoteiroProducaoEtiqueta>("rpe")
                        .Where("rpe.IdSetor=?id")
                            .Add("?id", setor.IdSetor)
                        .GroupBy("rpe.IdSetor")
                        .ExistsResult())
                        return new IMessageFormattable[]
                        {
                            (string.Format("O setor {0} não pode ser inativado, pois, existem etiquetas configuradas com este setor em seu roteiro de produção.", setor.Descricao)).GetFormatter()
                        };
                }
                else if (setor.Situacao == Situacao.Ativo)
                {
                    if (setor.ChangedProperties.Contains("Situacao") &&
                        setor.Tipo == Glass.Data.Model.TipoSetor.Entregue && SourceContext.Instance.CreateQuery()
                        .From<Glass.Data.Model.Setor>("s")
                        .Where("Situacao=?ativo AND Tipo=?entregue")
                        .Add("?ativo", Situacao.Ativo)
                        .Add("?entregue", Glass.Data.Model.TipoSetor.Entregue).ExistsResult())
                    {
                        return new IMessageFormattable[]
                        {
                            (string.Format("O setor {0} não pode ser ativado, pois, já existe um setor do tipo Entregue ativo. ", setor.Descricao)).GetFormatter()
                        };
                    }

                    if (setor.ChangedProperties.Contains("Situacao") &&
                        setor.Tipo == Glass.Data.Model.TipoSetor.ExpCarregamento && SourceContext.Instance.CreateQuery()
                        .From<Glass.Data.Model.Setor>("s")
                        .Where("Situacao=?ativo AND Tipo=?expCarregamento")
                        .Add("?ativo", Situacao.Ativo)
                        .Add("?expCarregamento", Glass.Data.Model.TipoSetor.ExpCarregamento).ExistsResult())
                    {
                        return new IMessageFormattable[]
                        {
                            (string.Format("O setor {0} não pode ser ativado, pois, já existe um setor do tipo Expedição Carregamento ativo. ", setor.Descricao)).GetFormatter()
                        };
                    }
                }
            }
            else
            {
                if (setor.Tipo == Glass.Data.Model.TipoSetor.Entregue && SourceContext.Instance.CreateQuery()
                        .From<Glass.Data.Model.Setor>("s")
                        .Where("Situacao=?ativo AND Tipo=?entregue")
                        .Add("?ativo", Situacao.Ativo)
                        .Add("?entregue", Glass.Data.Model.TipoSetor.Entregue).ExistsResult())
                {
                    return new IMessageFormattable[]
                    {
                            (string.Format("Não é possivel cadastrar o setor {0}, pois, já existe um setor do tipo Entregue ativo. ", setor.Descricao)).GetFormatter()
                    };
                }

                if (setor.Tipo == Glass.Data.Model.TipoSetor.ExpCarregamento && SourceContext.Instance.CreateQuery()
                    .From<Glass.Data.Model.Setor>("s")
                    .Where("Situacao=?ativo AND Tipo=?expCarregamento")
                    .Add("?ativo", Situacao.Ativo)
                    .Add("?expCarregamento", Glass.Data.Model.TipoSetor.ExpCarregamento).ExistsResult())
                {
                    return new IMessageFormattable[]
                    {
                            (string.Format("Não é possivel cadastrar o setor {0}, pois, já existe um setor do tipo Expedição Carregamento ativo. ", setor.Descricao)).GetFormatter()
                    };
                }
            }

            return new IMessageFormattable[0];
        }

        #endregion
    }
}
