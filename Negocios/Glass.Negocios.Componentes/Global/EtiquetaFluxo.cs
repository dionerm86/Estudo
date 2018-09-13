using System.Collections.Generic;
using System.Linq;
using Colosoft;
using GDA;

namespace Glass.Global.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de etiquetas.
    /// </summary>
    public class EtiquetaFluxo : 
        IEtiquetaFluxo, 
        Entidades.IValidadorEtiquetaProcesso,
        Entidades.IValidadorEtiquetaAplicacao
    {
        #region EtiquetaProcesso

        /// <summary>
        /// Pesquisa os processos de etiqueta.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.EtiquetaProcessoPesquisa> PesquisarEtiquetaProcessos()
        {
            var dados =  SourceContext.Instance.CreateQuery()
                .From<Data.Model.EtiquetaProcesso>("ep")
                .LeftJoin<Data.Model.EtiquetaAplicacao>("ep.IdAplicacao = ea.IdAplicacao", "ea")
                .Select(
                    @"ep.IdProcesso, ep.CodInterno, ep.IdAplicacao, ep.Descricao, ep.ForcarGerarSag,
                      ep.DestacarEtiqueta, ep.GerarFormaInexistente, ep.TipoProcesso,
                      ep.Situacao, ea.CodInterno AS CodInternoAplicacao,
                      ea.Descricao AS DescricaoAplicacao, ep.GerarArquivoDeMesa, ep.TipoPedido, ep.NumeroDiasUteisDataEntrega")
                .OrderBy("Descricao")
                .ToVirtualResult<Entidades.EtiquetaProcessoPesquisa>();

            dados.DataPageLoaded += (sender, e) =>
             {
                 foreach (var d in e.Page)
                 {
                     if (d.TipoPedido == null)
                         continue;

                     d.DescricaoTipoPedido = string.Join(", ", d.TipoPedido.Split(',')
                         .Select(f => ((Data.Model.Pedido.TipoPedidoEnum)f.StrParaInt()).Translate().Format()));
                 }
             };

            return dados;
        }

        /// <summary>
        /// Recupera os descritores dos EtiquetaProcessos.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemEtiquetaProcessos()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.EtiquetaProcesso>()
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.EtiquetaProcesso>()
                .ToList();
        }

        /// <summary>
        /// Recupera os dados do processo.
        /// </summary>
        /// <param name="idProcesso"></param>
        /// <returns></returns>
        public Entidades.EtiquetaProcesso ObtemEtiquetaProcesso(int idProcesso)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.EtiquetaProcesso>()
                .Where("IdProcesso=?id")
                .Add("?id", idProcesso)
                .ProcessResult<Entidades.EtiquetaProcesso>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados do EtiquetaProcesso.
        /// </summary>
        /// <param name="etiquetaProcesso"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarEtiquetaProcesso(Entidades.EtiquetaProcesso etiquetaProcesso)
        {
            etiquetaProcesso.Require("etiquetaProcesso").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                // Verifica se está atualizando a etiqueta
                if (etiquetaProcesso.ExistsInStorage &&
                    etiquetaProcesso.ChangedProperties.Contains("CodInterno"))
                {
                    // Atualiza a configuração para ficar igual à esta alteração
                    session.Update<Data.Model.ConfiguracaoLoja>(
                        new Data.Model.ConfiguracaoLoja
                        {
                            ValorTexto = etiquetaProcesso.CodInterno
                        },
                         Colosoft.Query.ConditionalContainer
                            .Parse("IdConfig IN (19, 21) AND ValorTexto IN (?texto)")
                            .Add("?texto",
                                SourceContext.Instance.CreateQuery()
                                    .From<Data.Model.EtiquetaProcesso>()
                                    .Select("CodInterno")
                                    .Where("IdProcesso=?idProcesso")
                                    .Add("?idProcesso", etiquetaProcesso.IdProcesso)),
                         "ValorTexto");
                }

                var resultado = etiquetaProcesso.Save(session);

                if (!resultado)
                    return resultado;

                if (etiquetaProcesso.ExistsInStorage)
                    Data.DAL.LogAlteracaoDAO.Instance.LogEtiquetaProcesso(ObtemEtiquetaProcesso(etiquetaProcesso.IdProcesso).DataModel, etiquetaProcesso.DataModel);

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apagar os dados do EtiquetaProcesso.
        /// </summary>
        /// <param name="etiquetaProcesso"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarEtiquetaProcesso(Entidades.EtiquetaProcesso etiquetaProcesso)
        {
            etiquetaProcesso.Require("etiquetaProcesso").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = etiquetaProcesso.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #region Membros de Entidades.IValidatorEtiquetaProcesso

        /// <summary>
        /// Valida a existencia do EtiquetaProcesso.
        /// </summary>
        /// <param name="etiquetaProcesso"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IValidadorEtiquetaProcesso.ValidaExistencia
            (Entidades.EtiquetaProcesso etiquetaProcesso)
        {
            var idProcesso = etiquetaProcesso.IdProcesso;
            var messages = new List<IMessageFormattable>();

            SourceContext.Instance.CreateMultiQuery()
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ProdutosPedido>()
                    .Where("IdProcesso=?id").Add("?id", idProcesso)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            messages.Add("Este Processo não pode ser excluído por haver pedidos relacionados ao mesmo.".GetFormatter());
                    })
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ProdutosPedidoEspelho>()
                    .Where("IdProcesso=?id").Add("?id", idProcesso)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            messages.Add("Este Processo não pode ser excluído por haver conferências relacionadas ao mesmo.".GetFormatter());
                    })
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.MaterialItemProjeto>()
                    .Where("IdProcesso=?id").Add("?id", idProcesso)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            messages.Add("Este Processo não pode ser excluído por haver projetos relacionados ao mesmo.".GetFormatter());
                    })
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.AmbientePedido>()
                    .Where("IdProcesso=?id").Add("?id", idProcesso)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            messages.Add("Este Processo não pode ser excluído por haver pedidos de mão de obra relacionados ao mesmo.".GetFormatter());
                    })
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.AmbientePedidoEspelho>()
                    .Where("IdProcesso=?id").Add("?id", idProcesso)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            messages.Add("Este Processo não pode ser excluído por haver pedidos de mão de obra relacionados ao mesmo.".GetFormatter());
                    })
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.BenefConfig>()
                    .Where("IdProcesso=?id").Add("?id", idProcesso)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            messages.Add("Este Processo não pode ser excluído por haver beneficiamentos relacionados ao mesmo.".GetFormatter());
                    })
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ProdutoTrocaDevolucao>()
                    .Where("IdProcesso=?id").Add("?id", idProcesso)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            messages.Add("Este Processo não pode ser excluído por haver trocas relacionadas ao mesmo.".GetFormatter());
                    })
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ProdutoTrocado>()
                    .Where("IdProcesso=?id").Add("?id", idProcesso)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            messages.Add("Este Processo não pode ser excluído por haver trocas relacionadas ao mesmo.".GetFormatter());
                    })
                .Execute();

            return messages.ToArray();
        }

        #endregion

        #endregion

        #region EtiquetaAplicacao

        /// <summary>
        /// Pesquisa as aplicações de etiqueta.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.EtiquetaAplicacao> PesquisarEtiquetaAplicacoes()
        {
            var dados= SourceContext.Instance.CreateQuery()
                .From<Data.Model.EtiquetaAplicacao>()
                .OrderBy("Descricao")
                .ToVirtualResult<Entidades.EtiquetaAplicacao>();

            dados.DataPageLoaded += (sender, e) =>
            {
                foreach (var d in e.Page)
                {
                    if (d.TipoPedido == null)
                        continue;

                    d.DescricaoTipoPedido = string.Join(", ", d.TipoPedido.Split(',')
                        .Select(f => ((Data.Model.Pedido.TipoPedidoEnum)f.StrParaInt()).Translate().Format()));
                }
            };

            return dados;
        }

        /// <summary>
        /// Recupera os descritores das EtiquetaAplicacao.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemEtiquetaAplicacoes()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.EtiquetaAplicacao>()
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.EtiquetaAplicacao>()
                .ToList();
        }

        /// <summary>
        /// Recupera os dados da EtiquetaAplicação.
        /// </summary>
        /// <param name="idAplicacao"></param>
        /// <returns></returns>
        public Entidades.EtiquetaAplicacao ObtemEtiquetaAplicacao(int idAplicacao)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.EtiquetaAplicacao>()
                .Where("IdAplicacao=?id")
                .Add("?id", idAplicacao)
                .ProcessResult<Entidades.EtiquetaAplicacao>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados da EtiquetaAplicacao.
        /// </summary>
        /// <param name="etiquetaAplicacao"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarEtiquetaAplicacao(Entidades.EtiquetaAplicacao etiquetaAplicacao)
        {
            etiquetaAplicacao.Require("etiquetaAplicacao").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                // Verifica se está atualizando a etiqueta
                if (etiquetaAplicacao.ExistsInStorage &&
                    etiquetaAplicacao.ChangedProperties.Contains("CodInterno"))
                {
                    // Atualiza a configuração para ficar igual à esta alteração
                    session.Update<Data.Model.ConfiguracaoLoja>(
                        new Data.Model.ConfiguracaoLoja
                        {
                            ValorTexto = etiquetaAplicacao.CodInterno
                        },
                         Colosoft.Query.ConditionalContainer
                            .Parse("IdConfig IN (20, 22) AND ValorTexto IN (?texto)")
                            .Add("?texto",
                                SourceContext.Instance.CreateQuery()
                                    .From<Data.Model.EtiquetaAplicacao>()
                                    .Select("CodInterno")
                                    .Where("IdAplicacao=?idAplicacao")
                                    .Add("?idAplicacao", etiquetaAplicacao.IdAplicacao)),
                         "ValorTexto");
                }

                var resultado = etiquetaAplicacao.Save(session);

                if (!resultado)
                    return resultado;

                //Se já existir no banco, insere um log de alteração da mesma
                if (etiquetaAplicacao.ExistsInStorage)
                    Data.DAL.LogAlteracaoDAO.Instance.LogEtiquetaAplicacao(ObtemEtiquetaAplicacao(etiquetaAplicacao.IdAplicacao).DataModel, etiquetaAplicacao.DataModel);

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados da EtiquetaAplicacao.
        /// </summary>
        /// <param name="etiquetaAplicacao"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarEtiquetaAplicacao(Entidades.EtiquetaAplicacao etiquetaAplicacao)
        {
            etiquetaAplicacao.Require("etiquetaAplicacao").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = etiquetaAplicacao.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #region Membros de Entidades.IValidadorEtiquetaAplicacao

        /// <summary>
        /// Valida a existencia da EtiquetaAplicacao.
        /// </summary>
        /// <param name="etiquetaAplicacao"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IValidadorEtiquetaAplicacao.ValidaExistencia
            (Entidades.EtiquetaAplicacao etiquetaAplicacao)
        {
            var idAplicacao = etiquetaAplicacao.IdAplicacao;
            var messages = new List<IMessageFormattable>();

            SourceContext.Instance.CreateMultiQuery()
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.EtiquetaProcesso>()
                    .Where("IdAplicacao=?id").Add("?id", idAplicacao)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            messages.Add("Esta Aplicação não pode ser excluída por haver processos relacionados à mesma.".GetFormatter());
                    })
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ProdutosPedido>()
                    .Where("IdAplicacao=?id").Add("?id", idAplicacao)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            messages.Add("Esta Aplicação não pode ser excluída por haver pedidos relacionados à mesma.".GetFormatter());
                    })
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ProdutosPedidoEspelho>()
                    .Where("IdAplicacao=?id").Add("?id", idAplicacao)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            messages.Add("Esta Aplicação não pode ser excluída por haver conferências relacionadas à mesma.".GetFormatter());
                    })
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.MaterialItemProjeto>()
                    .Where("IdAplicacao=?id").Add("?id", idAplicacao)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            messages.Add("Esta Aplicação não pode ser excluída por haver projetos relacionados à mesma.".GetFormatter());
                    })
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.AmbientePedido>()
                    .Where("IdAplicacao=?id").Add("?id", idAplicacao)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            messages.Add("Esta Aplicação não pode ser excluída por haver pedidos de mão de obra relacionados à mesma.".GetFormatter());
                    })
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.AmbientePedidoEspelho>()
                    .Where("IdAplicacao=?id").Add("?id", idAplicacao)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            messages.Add("Esta Aplicação não pode ser excluída por haver pedidos de mão de obra relacionados à mesma.".GetFormatter());
                    })
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.BenefConfig>()
                    .Where("IdAplicacao=?id").Add("?id", idAplicacao)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            messages.Add("Esta Aplicação não pode ser excluída por haver beneficiamentos relacionados à mesma.".GetFormatter());
                    })
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ProdutoTrocaDevolucao>()
                    .Where("IdAplicacao=?id").Add("?id", idAplicacao)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            messages.Add("Esta Aplicação não pode ser excluída por haver trocas relacionadas à mesma.".GetFormatter());
                    })
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.ProdutoTrocado>()
                    .Where("IdAplicacao=?id").Add("?id", idAplicacao)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            messages.Add("Esta Aplicação não pode ser excluída por haver trocas relacionadas à mesma.".GetFormatter());
                    })
                .Execute();

            return messages.ToArray();
        }

        #endregion

        #endregion
    }
}
