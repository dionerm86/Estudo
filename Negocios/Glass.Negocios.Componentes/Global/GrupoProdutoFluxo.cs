using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.Global.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de grupos de produtos.
    /// </summary>
    public class GrupoProdutoFluxo : IGrupoProdutoFluxo, Entidades.IValidadorGrupoProd, Entidades.IValidadorSubgrupoProd
    {
        /// <summary>
        /// Retorna uma lista com os tipos de cálculo.
        /// </summary>
        /// <param name="exibirDecimal">Identifica se é para exibir os tipos de calculo com decimal.</param>
        /// <param name="notaFiscal">Identifica se é para exibir os tipos de calculo de nota fiscal.</param>
        /// <returns></returns>
        public IEnumerable<Data.Model.TipoCalculoGrupoProd> ObtemTiposCalculo(bool exibirDecimal, bool notaFiscal)
        {
            yield return Data.Model.TipoCalculoGrupoProd.Qtd;

            if (notaFiscal)
                yield return Data.Model.TipoCalculoGrupoProd.QtdM2;

            yield return Data.Model.TipoCalculoGrupoProd.M2;
            yield return Data.Model.TipoCalculoGrupoProd.M2Direto;
            yield return Data.Model.TipoCalculoGrupoProd.Perimetro;

            if (exibirDecimal)
                yield return Data.Model.TipoCalculoGrupoProd.QtdDecimal;

            yield return Data.Model.TipoCalculoGrupoProd.MLAL0;
            yield return Data.Model.TipoCalculoGrupoProd.MLAL05;
            yield return Data.Model.TipoCalculoGrupoProd.MLAL1;
            yield return Data.Model.TipoCalculoGrupoProd.MLAL6;
            yield return Data.Model.TipoCalculoGrupoProd.ML;
        }

        /// <summary>
        /// Recupera o tipo de calculo para o grupo de produto.
        /// </summary>
        /// <param name="idGrupoProd">Identificador do grupo de produtos.</param>
        /// <param name="idSubgrupoProd">Identificador do subgrupo de produtos.</param>
        /// <returns></returns>
        public Data.Model.TipoCalculoGrupoProd ObtemTipoCalculo(int idGrupoProd, int? idSubgrupoProd)
        {
            if (idSubgrupoProd.HasValue && idSubgrupoProd.Value > 0)
            {
                // Recuperao tipo de calculo do subgrupo
                var tipoCalcSubgrupo = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.SubgrupoProd>()
                    .Select("TipoCalculo")
                    .Where("IdSubgrupoProd=?idSubgrupoProd")
                    .Add("?idSubgrupoProd", idSubgrupoProd)
                    .Execute()
                    .Select(f => (Data.Model.TipoCalculoGrupoProd?)f.GetInt32(0))
                    .FirstOrDefault();

                if (tipoCalcSubgrupo.HasValue)
                    return tipoCalcSubgrupo.Value;
            }

            var tipoCalcGrupo = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.GrupoProd>()
                    .Select("TipoCalculo")
                    .Where("IdGrupoProd=?idGrupoProd")
                    .Add("?idGrupoProd", idGrupoProd)
                    .Execute()
                    .Select(f => (Data.Model.TipoCalculoGrupoProd?)f.GetInt32(0))
                    .FirstOrDefault();

            if (tipoCalcGrupo.HasValue)
                return tipoCalcGrupo.Value;

            return Data.Model.TipoCalculoGrupoProd.Qtd;

        }

        #region GrupoProd

        /// <summary>
        /// Pesquisa os grupos de produto do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.GrupoProd> PesquisarGruposProduto()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.GrupoProd>()
                .OrderBy("Descricao")
                .ToVirtualResultLazy<Entidades.GrupoProd>();
        }

        /// <summary>
        /// Recupera os descritores dos grupos de produto.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemGruposProduto()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.GrupoProd>()
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.GrupoProd>()
                .ToList();
        }

        /// <summary>
        /// Recupera os dados do grupo de produtos.
        /// </summary>
        /// <param name="idGrupoProd"></param>
        /// <returns></returns>
        public Entidades.GrupoProd ObtemGrupoProduto(int idGrupoProd)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.GrupoProd>()
                .Where("IdGrupoProd=?id")
                .Add("?id", idGrupoProd)
                .ProcessLazyResult<Entidades.GrupoProd>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Apaga os dados do grupo de produtos.
        /// </summary>
        /// <param name="grupoProd"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarGrupoProduto(Entidades.GrupoProd grupoProd)
        {
            grupoProd.Require("grupoProd").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = grupoProd.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados do grupo de produtos.
        /// </summary>
        /// <param name="grupoProd"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarGrupoProduto(Entidades.GrupoProd grupoProd)
        {
            grupoProd.Require("grupoProd").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = grupoProd.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #region Membros ValidadorGrupoProd

        /// <summary>
        /// Valida a atualização do grupo.
        /// </summary>
        IMessageFormattable[] Entidades.IValidadorGrupoProd.ValidaAtualizacao(Entidades.GrupoProd grupoProd)
        {
            if (!grupoProd.TipoCalculo.HasValue || !grupoProd.TipoCalculoNf.HasValue)
                return new IMessageFormattable[]
                {
                    "Informe o tipo de cálculo do grupo.".GetFormatter()
                };

            return new IMessageFormattable[0];
        }

        /// <summary>
        /// Valida a existema o grupo de produtos.
        /// </summary>
        /// <param name="grupoProd"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IValidadorGrupoProd.ValidaExistencia(Entidades.GrupoProd grupoProd)
        {
            var resultado = new List<IMessageFormattable>();

            SourceContext.Instance.CreateMultiQuery()
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Produto>()
                    .Where("IdGrupoProd=?id").Add("?id", grupoProd.IdGrupoProd)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            resultado.Add("Este grupo não pode ser excluído. Existem produtos relacionados ao mesmo.".GetFormatter());
                    })
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.SubgrupoProd>()
                    .Where("IdGrupoProd=?id").Add("?id", grupoProd.IdGrupoProd)
                    .Count(),
                    (sender, query, result) =>
                    {
                        if (result.Select(f => f.GetInt32(0)).FirstOrDefault() > 0)
                            resultado.Add("Este grupo não pode ser excluído. Existem subgrupos relacionados ao mesmo.".GetFormatter());
                    })
                .Execute();

            return resultado.ToArray();
        }

        #endregion

        #endregion

        #region SubgrupoProd

        /// <summary>
        /// Pesquisa os subgrupos de produtos.
        /// </summary>
        /// <param name="idGrupoProd">Identificador do grupo que será usado como filtro.</param>
        /// <returns></returns>
        public IList<Entidades.SubgrupoProdPesquisa> PesquisarSubgruposProduto(int? idGrupoProd)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.SubgrupoProd>("s")
                .InnerJoin<Data.Model.GrupoProd>("s.IdGrupoProd = g.IdGrupoProd", "g")
                .LeftJoin<Data.Model.Cliente>("s.IdCli = c.IdCli", "c")
                .OrderBy("Descricao")
                .Select(@"s.IdSubgrupoProd, s.IdGrupoProd, s.Descricao, s.TipoCalculo,
                          s.TipoCalculoNf, s.BloquearEstoque, s.NaoAlterarEstoque,
                          s.NaoAlterarEstoqueFiscal, s.ProdutosEstoque,
                          s.IsVidroTemperado, s.ExibirMensagemEstoque,
                          s.NumeroDiasMinimoEntrega, s.DiaSemanaEntrega,
                          s.GeraVolume, s.TipoSubgrupo, s.IdCli,
                          g.TipoCalculo AS TipoCalculoGrupo, g.TipoCalculoNf AS TipoCalculoNfGrupo, c.Nome as NomeCliente,
                          s.LiberarPendenteProducao, s.PermitirItemRevendaNaVenda, s.BloquearEcommerce");

            if (idGrupoProd.HasValue && idGrupoProd.Value > 0)
                consulta.WhereClause.And("IdGrupoProd=?idGrupoProd").Add("?idGrupoProd", idGrupoProd.Value);

            var retorno = consulta.ToVirtualResultLazy<Entidades.SubgrupoProdPesquisa>();

            if (retorno.Any())
            {
                var lojasAssociadas = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.SubgrupoProdLoja>("sgpl")
                    .LeftJoin<Data.Model.Loja>("l.IdLoja=sgpl.IdLoja", "l")
                    .Select("sgpl.IdSubgrupoProd, l.NomeFantasia, sgpl.IdLoja")
                    .Where(string.Format("IdSubgrupoProd IN ({0})", string.Join(", ", retorno.Select(f => f.IdSubgrupoProd))))
                    .Execute().Select(f => new
                    {
                        IdSubgrupoProd = f.GetInt32(0),
                        Loja = f.GetString(1),
                        IdLoja = f.GetInt32(2)
                    }).ToList();

                if (lojasAssociadas.Any())
                    foreach (var item in retorno)
                    {
                        item.Lojas = string.Join(",", lojasAssociadas.Where(f => f.IdSubgrupoProd == item.IdSubgrupoProd).Select(f => f.Loja));
                        item.IdsLojaAssociacao = lojasAssociadas.Where(f => f.IdSubgrupoProd == item.IdSubgrupoProd).Select(f => f.IdLoja).ToArray();
                    }
            }

            return retorno.ToList();
        }

        /// <summary>
        /// Recupera os descritores do subgrupos.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemSubgruposProduto()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.SubgrupoProd>()
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.SubgrupoProd>()
                .ToList();
        }

        /// <summary>
        /// Recupera os descritores dos subgrupos de produtos associados ao grupo informado.
        /// </summary>
        public IList<Colosoft.IEntityDescriptor> ObtemSubgruposProduto(int idGrupoProd)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.SubgrupoProd>()
                .Where("IdGrupoProd=?id")
                .Add("?id", idGrupoProd)
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.SubgrupoProd>()
                .ToList();
        }

        /// <summary>
        /// Recupera os descritores dos subgrupos de produtos do grupo vidro.
        /// </summary>
        public IList<Colosoft.IEntityDescriptor> ObterSubgruposVidro()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.SubgrupoProd>()
                .Where("IdGrupoProd=?idGrupoProd")
                .Add("?idGrupoProd", Data.Model.NomeGrupoProd.Vidro)
                .ProcessResultDescriptor<Entidades.SubgrupoProd>()
                .ToList();
        }
        
        /// <summary>
        /// Recupera os descritores dos subgrupos de produtos do grupo vidro e mão de obra, conforme solicitação do chamado 45497.
        /// </summary>
        public IList<IEntityDescriptor> ObterSubgruposClassificacaoRoteiroProducao()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.SubgrupoProd>()
                .Where("IdGrupoProd IN (?maoDeObra, ?vidro)")
                    .Add("?maoDeObra", Data.Model.NomeGrupoProd.MaoDeObra)
                    .Add("?vidro", Data.Model.NomeGrupoProd.Vidro)
                .ProcessResultDescriptor<Entidades.SubgrupoProd>()
                .ToList();
        }
        /// <summary>
        ///  Recupera os descritores dos subgrupos de produtos associados ao grupos informados.
        /// </summary>
        public IList<Colosoft.IEntityDescriptor> ObtemSubgruposProduto(string idsGrupoProds)
        {
            if (string.IsNullOrEmpty(idsGrupoProds))
                return new List<Colosoft.IEntityDescriptor>();

            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.SubgrupoProd>()
                .Where(string.Format("IdGrupoProd IN ({0})", idsGrupoProds))
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.SubgrupoProd>()
                .ToList();
        }

        /// <summary>
        /// Recupera os dados do subgrupo.
        /// </summary>
        public Entidades.SubgrupoProd ObtemSubgrupoProduto(int idSubgrupoProd)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.SubgrupoProd>()
                .Where("IdSubgrupoProd=?id")
                .Add("?id", idSubgrupoProd)
                .ProcessLazyResult<Entidades.SubgrupoProd>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados do subgrupo.
        /// </summary>
        public Colosoft.Business.SaveResult SalvarSubgrupoProduto(Entidades.SubgrupoProd subgrupoProduto)
        {
            subgrupoProduto.Require("subgrupoProduto").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = subgrupoProduto.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados do subgrupo.
        /// </summary>
        public Colosoft.Business.DeleteResult ApagarSubgrupoProduto(Entidades.SubgrupoProd subgrupoProduto)
        {
            subgrupoProduto.Require("subgrupoProduto").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = subgrupoProduto.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }
        
        #region Membros de IValidadorSubgrupoProd

        /// <summary>
        /// Valida a existencia do subgrupo de produtos.
        /// </summary>
        IMessageFormattable[] Entidades.IValidadorSubgrupoProd.ValidaExistencia(Entidades.SubgrupoProd subgrupoProd)
        {
            if (SourceContext.Instance.CreateQuery()
                .From<Data.Model.Produto>()
                .Where("IdSubgrupoProd=?id")
                .Add("?id", subgrupoProd.IdSubgrupoProd)
                .ExistsResult())
                return new IMessageFormattable[]
                {
                    "Este subgrupo não pode ser excluído. Existem produtos relacionados ao mesmo.".GetFormatter()
                };

            return new IMessageFormattable[0];
        }

        /// <summary>
        /// Valida a atualização do cliente.
        /// </summary>
        IMessageFormattable[] Entidades.IValidadorSubgrupoProd.ValidaAtualizacao(Entidades.SubgrupoProd subgrupoProd)
        {
            if (subgrupoProd.ProdutosEstoque && subgrupoProd.GeraVolume)
                return new[]
                {
                    string.Format("Não é possível gerar volume de produtos para estoque. O subgrupo não pode ser {0} com as opções Produtos para Estoque e Gera Volume.",
                        subgrupoProd.ExistsInStorage ? "salvo" : "inserido").GetFormatter()
                };

            /* Chamado 37536. */
            if (SourceContext.Instance.CreateQuery()
                .From<Data.Model.GrupoProd>("gp")
                .InnerJoin<Data.Model.DescontoAcrescimoCliente>("gp.IdGrupoProd=dac.IdGrupoProd", "dac")
                .Where("gp.IdGrupoProd=?idGrupoProd AND dac.IdSubgrupoProd IS NULL")
                .Add("?idGrupoProd", subgrupoProd.IdGrupoProd)
                .ExistsResult())
                return new IMessageFormattable[]
                {
                    ("Não é possível inserir subgrupo neste grupo, pois, o mesmo possui desconto/acréscimo configurado na tabela de cliente.").GetFormatter()
                };

            /* Chamado 37411. */
            if (subgrupoProd.ExistsInStorage && subgrupoProd.TipoSubgrupo == Data.Model.TipoSubgrupoProd.Modulado)
                if (SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Produto>("p")
                    .LeftJoin<Data.Model.ProdutoBaixaEstoque>("p.IdProd=pbe.IdProd", "pbe")
                    .Where("IdSubgrupoProd=?id AND pbe.IdProd IS NULL")
                    .Add("?id", subgrupoProd.IdSubgrupoProd)
                    .ExistsResult())
                    return new IMessageFormattable[]
                    {
                        ("Um ou mais produtos desse subgrupo não possuem matéria-prima associada. " +
                        "Todos os produtos associados ao subgrupo do tipo Modulado devem ter matéria-prima associada.").GetFormatter()
                    };

            /* Chamado 39865. */
            if (subgrupoProd.ProdutosEstoque && Data.DAL.GrupoProdDAO.Instance.IsVidro(subgrupoProd.IdGrupoProd) &&
                subgrupoProd.TipoCalculo != Data.Model.TipoCalculoGrupoProd.Qtd &&
                subgrupoProd.TipoCalculo != Data.Model.TipoCalculoGrupoProd.M2Direto)
                return new[]
                {
                    "Não é possível configurar um subgrupo do grupo Vidro com a opção produtos para estoque caso o tipo de cálculo não seja Quantidade ou M2 Direto.".GetFormatter()
                };

            /* Chamado 38721. */
            if (subgrupoProd.Descricao.IsNullOrEmpty())
                return new[]
                {
                    "Informe o nome do subgrupo.".GetFormatter()
                };

            if (subgrupoProd.ExistsInStorage && subgrupoProd.TipoSubgrupo == Data.Model.TipoSubgrupoProd.VidroLaminado)
            {
                if (SourceContext.Instance.CreateQuery()
                                    .From<Data.Model.Produto>("p")
                                    .LeftJoin<Data.Model.ProdutoBaixaEstoque>("p.IdProd=pbe.IdProd", "pbe")
                                    .Where("IdSubgrupoProd=?id AND pbe.IdProd IS NULL")
                                    .Add("?id", subgrupoProd.IdSubgrupoProd)
                                    .ExistsResult())
                    return new IMessageFormattable[]
                    {
                        ("Um ou mais produtos desse subgrupo não possuem matéria-prima associada. " +
                        "Todos os produtos associados ao subgrupo do tipo Laminado devem ter matéria-prima associada.").GetFormatter()
                    };
            }

            return new IMessageFormattable[0];
        }


        #endregion

        #endregion
    }
}
