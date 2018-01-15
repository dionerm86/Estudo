using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;
using Colosoft.Query;

namespace Glass.Global.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio dos beneficiamentos.
    /// </summary>
    public class BeneficiamentoFluxo :
        IBeneficiamentoFluxo, Entidades.IValidadorBenefConfig
    {
        #region BenefConfig

        /// <summary>
        /// Pesquisa os beneficiamentos.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.BenefConfigPesquisa> PesquisarConfiguracoesBeneficiamento()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.BenefConfig>("bc")
                .LeftJoin<Data.Model.EtiquetaAplicacao>("bc.IdAplicacao = ea.IdAplicacao", "ea")
                .LeftJoin<Data.Model.EtiquetaProcesso>("bc.IdProcesso = ep.IdProcesso", "ep")
                .Where("IdParent IS NULL")
                .OrderBy("NumSeq")
                .Select(@"bc.IdBenefConfig, bc.Nome, bc.Descricao, bc.TipoControle, bc.TipoCalculo,
                          bc.IdAplicacao, bc.IdProcesso, bc.CobrancaOpcional, bc.NaoExibirEtiqueta,
                          bc.Situacao, bc.TipoBenef, ea.CodInterno AS CodAplicacao, 
                          ep.CodInterno AS CodProcesso")
                .ToVirtualResult<Entidades.BenefConfigPesquisa>();
        }

        /// <summary>
        /// Recupera os descritores dos beneficiamentos.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemBenefConfig()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.BenefConfig>()
                .OrderBy("Nome")
                .ProcessResultDescriptor<Entidades.BenefConfig>()
                .ToList();
        }

        /// <summary>
        /// Recupera os descritores dos beneficiamentos.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemBenefConfigAtivos()
        {
            var dados = SourceContext.Instance.CreateQuery()
                .From<Data.Model.BenefConfig>("bc")
                    .LeftJoin<Data.Model.BenefConfig>("bc.IdParent = bcp.IdBenefConfig", "bcp")
                .Where("bc.Situacao = ?sit").Add("?sit", Situacao.Ativo)
                .Select(@"bc.IdBenefConfig,
                        CASE
                            WHEN bc.IdParent > 0 THEN CONCAT(bcp.Descricao, ' - ', bc.Descricao)
                            ELSE bc.Descricao
                        END")
                .Execute()
                .Select(f => new Colosoft.EntityDescriptor()
                {
                    Id = f.GetInt32(0),
                    Name = f.GetString(1)
                })
                .OrderBy(f=>f.Name)
                .ToList<Colosoft.IEntityDescriptor>();

            return dados;


            //return SourceContext.Instance.CreateQuery()
            //    .From<Data.Model.BenefConfig>()
            //    .OrderBy("Nome")
            //    .Where("Situacao = ?sit").Add("?sit", Situacao.Ativo)
            //    .ProcessResultDescriptor<Entidades.BenefConfig>()
            //    .ToList();
        }

        /// <summary>
        /// Recupera os dados da configuração do beneficiamento.
        /// </summary>
        /// <param name="idBenefConfig"></param>
        /// <returns></returns>
        public Entidades.BenefConfig ObtemBenefConfig(int idBenefConfig)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.BenefConfig>()
                .Where("IdBenefConfig=?id")
                .Add("?id", idBenefConfig)
                .ProcessLazyResult<Entidades.BenefConfig>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva as configurações do beneficiamento.
        /// </summary>
        /// <param name="benefConfig"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarBenefConfig(Entidades.BenefConfig benefConfig)
        {
            benefConfig.Require("benefConfig").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = benefConfig.Save(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados do beneficiamento.
        /// </summary>
        /// <param name="benefConfig"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarBenefConfig(Entidades.BenefConfig benefConfig)
        {
            benefConfig.Require("benefConfig").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = benefConfig.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        /// <summary>
        /// Altera a posição da configuração do beneficiamento.
        /// </summary>
        /// <param name="idBenefConfig">Identificador da configuração.</param>
        /// <param name="paraCima">Identifica se é para mover para cima.</param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult AlterarPosicaoBenefConfig(int idBenefConfig, bool paraCima)
        {
            var benefConfig = SourceContext.Instance.CreateQuery()
                .From<Data.Model.BenefConfig>()
                .Where("IdBenefConfig=?id")
                .Add("?id", idBenefConfig)
                .ProcessLazyResult<Entidades.BenefConfig>()
                .FirstOrDefault();

            if (benefConfig != null)
            {
                var adjacente = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.BenefConfig>()
                    .Where("NumSeq=?numSeq")
                    .Add("?numSeq", (benefConfig.NumSeq + (paraCima ? -1 : 1)))
                    .ProcessLazyResult<Entidades.BenefConfig>()
                    .FirstOrDefault();


                if (adjacente != null)
                {
                    // Altera a posição do beneficiamento adjacente à este
                    adjacente.NumSeq += (paraCima ? 1 : -1);

                    // Altera a posição deste beneficiamento
                    benefConfig.NumSeq += (paraCima ? -1 : 1);

                    using (var session = SourceContext.Instance.CreateSession())
                    {
                        var resultado = adjacente.Save(session);
                        if (!resultado) return resultado;

                        resultado = benefConfig.Save(session);
                        if (!resultado) return resultado;

                        return session.Execute(false).ToSaveResult();
                    }
                }
            }

            return new Colosoft.Business.SaveResult(true, null);
        }

        /// <summary>
        /// Valida a atualização da configuração do beneficiamento.
        /// </summary>
        /// <param name="benefConfig"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IValidadorBenefConfig.ValidaAtualizacao(Entidades.BenefConfig benefConfig)
        {
            if (!benefConfig.IdParent.HasValue && (benefConfig.ChangedProperties.Contains("Descricao") || !benefConfig.ExistsInStorage))
            {
                // Não permite que sejam inseridos beneficiamentos com o mesmo nome
                var consulta = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.BenefConfig>()
                    .Where("(Nome=?nome OR Descricao=?descricao) AND IdParent IS NULL AND Situacao=?situacao")
                    .Add("?nome", benefConfig.Nome)
                    .Add("?descricao", benefConfig.Descricao)
                    .Add("?situacao", Glass.Situacao.Ativo);

                if (benefConfig.ExistsInStorage)
                    consulta.WhereClause
                        .And("IdBenefConfig<>?idBenefConfig")
                        .Add("?idBenefConfig", benefConfig.IdBenefConfig);

                if (consulta.ExistsResult())
                    return new IMessageFormattable[]
                {
                    "Já foi inserido um beneficiamento com este nome/descrição.".GetFormatter()
                };
            }

            return new IMessageFormattable[0];
        }

        /// <summary>
        /// Recupera o numero da sequencia para o beneficiamento informado.
        /// </summary>
        /// <param name="benefConfig"></param>
        /// <returns></returns>
        int Entidades.IValidadorBenefConfig.ObtemNumeroSequencia(Entidades.BenefConfig benefConfig)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.BenefConfig>()
                .Select("MAX(NumSeq)")
                .Execute()
                .Select(f => f.GetInt32(0))
                .FirstOrDefault() + 1;
        }

        /// <summary>
        /// Verifica se a configuração do beneficiamento está em uso
        /// em alguma parte do sistema.
        /// </summary>
        /// <param name="benefConfig"></param>
        /// <returns></returns>
        bool Entidades.IValidadorBenefConfig.EmUso(Entidades.BenefConfig benefConfig)
        {
            var models = new Type[]
            {
                typeof(Glass.Data.Model.MaterialProjetoBenef),
                typeof(Glass.Data.Model.ProdutosCompraBenef),
                typeof(Glass.Data.Model.ProdutoOrcamentoBenef),
                typeof(Glass.Data.Model.ProdutoPedidoBenef),
                typeof(Glass.Data.Model.ProdutoPedidoEspelhoBenef),
                typeof(Glass.Data.Model.ProdutoBenef),
                typeof(Glass.Data.Model.ProdutoTrocaDevolucaoBenef),
                typeof(Glass.Data.Model.PecaModeloBenef),
                typeof(Glass.Data.Model.PecaItemProjBenef),
                typeof(Glass.Data.Model.ProdutoTrocadoBenef)
            };

            var qtdeReferencias = 0;

            var consultaReferencias = SourceContext.Instance.CreateMultiQuery();

            // Cria as consultas para as models
            foreach (var i in models)
                consultaReferencias.Add(SourceContext.Instance.CreateQuery()
                    .From(new Colosoft.Query.EntityInfo(i.FullName, "t"))
                    .InnerJoin<Data.Model.BenefConfig>("t.IdBenefConfig = bc.IdBenefConfig", "bc")
                    .Where("t.IdBenefConfig=?id OR bc.IdParent=?id")
                    .Add("?id", benefConfig.IdBenefConfig)
                    .Count(), (sender, query, result) =>
                        qtdeReferencias += result.Select(f => f.GetInt32(0)).FirstOrDefault());


            consultaReferencias.Execute();

            // Verifica se este beneficiamento ou seus filhos estão sendo usados em alguma tabela
            return (qtdeReferencias > 0);
        }
        
        #endregion

        #region BenefConfigPreco
        
        /// <summary>
        /// Pesquisa os preços padrão dos beneficiamentos.
        /// </summary>
        /// <param name="descricao">Descricação do beneficiamento que será filtrado.</param>
        /// <returns></returns>
        public IList<Entidades.BenefConfigPrecoPadrao> PesquisarPrecosPadraoBeneficiamentos(string descricao)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.BenefConfigPreco>("bcp")
                .LeftJoin<Data.Model.BenefConfig>("bcp.IdBenefConfig=bc.IdBenefConfig", "bc")
                .LeftJoin<Data.Model.BenefConfig>("bc.IdParent=bc2.IdBenefConfig", "bc2")
                .Select(
                    @"bcp.IdBenefConfigPreco, bcp.IdBenefConfig, bcp.IdSubgrupoProd,
                      bc.Descricao AS DescricaoBenef, bc2.Descricao AS DescricaoBenefPai,
                      bcp.IdCorVidro, bcp.Espessura, bcp.ValorAtacado, bcp.ValorBalcao, 
                      bcp.ValorObra, bcp.Custo, bc.TipoCalculo")
                .OrderBy("DescricaoBenefPai, DescricaoBenef, Espessura, IdSubgrupoProd, IdCorVidro")
                .Where("ISNULL(bc2.Situacao, bc.Situacao)=?situacao AND IdSubgrupoProd IS NULL AND IdCorVidro IS NULL")
                .Add("?situacao", Glass.Situacao.Ativo);

            if (!string.IsNullOrEmpty(descricao))
                consulta.WhereClause
                    .And("(bc.Descricao LIKE ?descricao OR bc2.Descricao LIKE ?descricao) ")
                    .Add("?descricao", string.Format("%{0}%", descricao));

            var subPrecos = new List<Entidades.BenefConfigPrecoPadrao>();

            consulta.BeginSubQuery((sender, e) =>
                {
                    var bindStrategy = TypeBindStrategyCache.GetItem(typeof(Entidades.BenefConfigPrecoPadrao), null);

                    // Carrega os sub preços
                    foreach (var record in e.Result)
                    {
                        object preco = new Entidades.BenefConfigPrecoPadrao();
                        bindStrategy.Bind(record, BindStrategyMode.All, ref preco);
                        subPrecos.Add((Entidades.BenefConfigPrecoPadrao)preco);
                    }

                }, (sender, e) =>
                {
                    throw e.Result.Error;
                })
                .From<Data.Model.BenefConfigPreco>("bcp")
                .LeftJoin<Data.Model.BenefConfig>("bcp.IdBenefConfig=bc.IdBenefConfig", "bc")
                .LeftJoin<Data.Model.BenefConfig>("bc.IdParent=bc2.IdBenefConfig", "bc2")
                .LeftJoin<Data.Model.SubgrupoProd>("bcp.IdSubgrupoProd=sp.IdSubgrupoProd", "sp")
                .LeftJoin<Data.Model.CorVidro>("bcp.IdCorVidro=c.IdCorVidro", "c")
                .Select(
                    @"bcp.IdBenefConfigPreco, bcp.IdBenefConfig, bc.Descricao, 
                      bcp.IdSubgrupoProd, sp.Descricao AS SubgrupoProd,
                      bc.Descricao AS DescricaoBenef, bc2.Descricao AS DescricaoBenefPai,
                      bcp.IdCorVidro, c.Descricao AS CorVidro, bcp.Espessura, bcp.ValorAtacado, 
                      bcp.ValorBalcao, bcp.ValorObra, bcp.Custo, bc.TipoCalculo")
                .OrderBy("Espessura, IdSubgrupoProd, IdCorVidro, DescricaoBenef, DescricaoBenefPai")
                .Where(@"IdBenefConfig=?idBenefConfig AND ISNULL(bc2.Situacao, bc.Situacao)=?situacao AND Espessura=?espessura AND
                    (IdSubgrupoProd IS NOT NULL OR IdCorVidro IS NOT NULL)")
                .Add("?idBenefConfig", new ReferenceParameter("IdBenefConfig"))
                .Add("?espessura", new ReferenceParameter("Espessura"))
                .Add("?situacao", Glass.Situacao.Ativo)
                .EndSubQuery();

            var resultado = consulta.ToVirtualResult<Entidades.BenefConfigPrecoPadrao>();

            resultado.DataPageLoaded += (sender, e) =>
                {
                    // Carrega os subpreços para a página de dados carregada
                    foreach (var i in e.Page)
                        foreach (var j in subPrecos.Where(f =>
                            f.IdBenefConfig == i.IdBenefConfig))
                            i.Precos.Add(j);

                    subPrecos.Clear();
                };

            return resultado;
        }

        /// <summary>
        /// Pesquisa os preços padrão dos beneficiamentos.
        /// </summary>
        /// <param name="descricao">Descricação do beneficiamento que será filtrado.</param>
        /// <returns></returns>
        public IList<Entidades.BenefConfigPrecoPesquisa> PesquisarResumoPrecosPadraoBeneficiamentos(string descricao)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.BenefConfigPreco>("bcp")
                .LeftJoin<Data.Model.BenefConfig>("bcp.IdBenefConfig=bc.IdBenefConfig", "bc")
                .LeftJoin<Data.Model.BenefConfig>("bc.IdParent=bc2.IdBenefConfig", "bc2")
                .Select(
                    @"bcp.IdBenefConfigPreco, bcp.IdBenefConfig,
                      bc.Descricao AS DescricaoBenef, bc2.Descricao AS DescricaoBenefPai,
                      bcp.Espessura, bcp.ValorAtacado, bcp.ValorBalcao, 
                      bcp.ValorObra, bcp.Custo")
                .OrderBy("DescricaoBenefPai, DescricaoBenef, Espessura, bcp.IdSubgrupoProd, IdCorVidro")
                .Where("ISNULL(bc2.Situacao, bc.Situacao)=?situacao AND bcp.IdSubgrupoProd IS NULL AND bcp.IdCorVidro IS NULL")
                .Add("?situacao", Glass.Situacao.Ativo);

            if (!string.IsNullOrEmpty(descricao))
                consulta.WhereClause
                    .And("(bc.Descricao LIKE ?descricao OR bc2.Descricao LIKE ?descricao) ")
                    .Add("?descricao", string.Format("%{0}%", descricao));

            return consulta.ToVirtualResult<Entidades.BenefConfigPrecoPesquisa>();
        }

        /// <summary>
        /// Recupera os preços das configurações de beneficiamentos pelos
        /// identificadores informados.
        /// </summary>
        /// <param name="idsBenefConfigPreco"></param>
        /// <returns></returns>
        public IEnumerable<Entidades.BenefConfigPreco> ObtemPrecosBeneficiamento(IEnumerable<int> idsBenefConfigPreco)
        {
            int count = 0;
            var ids = new List<string>();
            var parts = new List<string[]>();

            foreach (var i in idsBenefConfigPreco)
            {
                ids.Add(i.ToString());

                if (count == 200)
                {
                    count = 0;
                    parts.Add(ids.ToArray());
                    ids.Clear();
                }
                else
                    count++;
            }

            if (count > 0)
                parts.Add(ids.ToArray());

            foreach(var i in parts)
                foreach (var j in SourceContext.Instance.CreateQuery()
                            .From<Data.Model.BenefConfigPreco>()
                            .Where(string.Format("IdBenefConfigPreco IN ({0})",
                                string.Join(",", i)))
                            .ProcessLazyResult<Entidades.BenefConfigPreco>())
                {
                    yield return j;
                }
            
        }

        /// <summary>
        /// Recupera o preço do beneficiamento.
        /// </summary>
        /// <param name="idBenefConfigPreco"></param>
        /// <returns></returns>
        public Entidades.BenefConfigPreco ObtemPrecoBeneficiamento(int idBenefConfigPreco)
        {
            return SourceContext.Instance.CreateQuery()
                    .From<Data.Model.BenefConfigPreco>()
                    .Where("IdBenefConfigPreco=?id")
                    .Add("?id", idBenefConfigPreco)
                    .ProcessLazyResult<Entidades.BenefConfigPreco>()
                    .FirstOrDefault();
        }

        /// <summary>
        /// Salva o preço do beneficiamento.
        /// </summary>
        /// <param name="preco"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarPrecoBeneficiamento(Entidades.BenefConfigPreco preco)
        {
            preco.Require("preco").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                Colosoft.Business.SaveResult resultado = null;
                if (!(resultado = preco.Save(session)))
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Salva os preços dos beneficiamentos.
        /// </summary>
        /// <param name="precos">Preços que serão atualizados.</param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarPrecosBeneficiamentos(IEnumerable<Entidades.BenefConfigPreco> precos)
        {
            precos.Require("precos").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                Colosoft.Business.SaveResult resultado = null;

                foreach (var i in precos)
                    if (!(resultado = i.Save(session)))
                        return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        #endregion
    }
}
