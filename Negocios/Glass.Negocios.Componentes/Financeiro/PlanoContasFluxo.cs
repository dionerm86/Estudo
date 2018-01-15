using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.Financeiro.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio dos planos de conta.
    /// </summary>
    public class PlanoContasFluxo : IPlanoContasFluxo, Entidades.IValidadorPlanoContas
    {
        #region Categoria de Contas

        /// <summary>
        /// Pesquisa as categorias de conta do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.CategoriaConta> PesquisarCategoriasConta()
        {
            var retorno =  SourceContext.Instance.CreateQuery()
                .From<Data.Model.CategoriaConta>()
                .OrderBy("NumeroSequencia")
                .ToVirtualResultLazy<Entidades.CategoriaConta>();

            return retorno;
        }

        /// <summary>
        /// Recupera os descritores das categorias de conta cadastradas no sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemCategoriasConta(bool ativas = false)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.CategoriaConta>()
                .OrderBy("Descricao");

            consulta
                .Where("Situacao = ?sit")
                .Add("?sit", Glass.Situacao.Ativo);

            return consulta.ProcessResultDescriptor<Entidades.CategoriaConta>()
                .ToList();
        }

        /// <summary>
        /// Recupera os descritores das categorias de conta sem as categorias do tipo Subtotal.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemCategoriasContaSemSubtotal()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.CategoriaConta>()
                .Where("Tipo<>?tipo")
                .Add("?tipo", Data.Model.TipoCategoriaConta.Subtotal)
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.CategoriaConta>()
                .ToList();
        }

        /// <summary>
        /// Recupera os dados da categoria de conta.
        /// </summary>
        /// <param name="idCategoriaConta">Identificador da categoria.</param>
        /// <returns></returns>
        public Entidades.CategoriaConta ObtemCategoriaConta(int idCategoriaConta)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.CategoriaConta>()
                .Where("IdCategoriaConta=?id")
                .Add("?id", idCategoriaConta)
                .ProcessResult<Entidades.CategoriaConta>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados da categoria de conta.
        /// </summary>
        /// <param name="categoriaConta"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarCategoriaConta(Entidades.CategoriaConta categoriaConta)
        {
            categoriaConta.Require("categoriaConta").NotNull();

            if (categoriaConta.IdCategoriaConta > 0)
                categoriaConta.DataModel.ExistsInStorage = true;

            // Verifica se um novo registro
            if (!categoriaConta.ExistsInStorage)
            {
                var ultimoNumeroSequencia = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.CategoriaConta>()
                    .Select("MAX(NumeroSequencia)")
                    .Execute()
                    .Select(f => f.GetInt32(0))
                    .FirstOrDefault();

                categoriaConta.NumeroSequencia = ultimoNumeroSequencia + 1;
            }

            using (var session = SourceContext.Instance.CreateSession())
            {
                Colosoft.Business.SaveResult resultado = null;

                // Verifica se é uma atualização
                if (categoriaConta.ExistsInStorage)
                {
                    if (categoriaConta.Tipo == null)
                    {
                        return new Colosoft.Business.SaveResult(false, "O tipo da categoria não pode ser vazio.".GetFormatter());
                    }

                    //Inativa os Grupos Associados a categoria caso a mesma estaja sendo inativada
                    if (categoriaConta.Situacao == Situacao.Inativo)
                    {
                        var grupos = SourceContext.Instance.CreateQuery()
                            .From<Data.Model.GrupoConta>()
                            .Where("IdCategoriaConta = " + categoriaConta.IdCategoriaConta)
                            .ProcessResult<Entidades.GrupoConta>();

                        foreach (var g in grupos)
                        {
                            var planos = SourceContext.Instance.CreateQuery()
                            .From<Data.Model.PlanoContas>()
                            .Where("IdGrupo = " + g.IdGrupo)
                            .ProcessResult<Entidades.PlanoContas>();

                            foreach (var p in planos)
                            {
                                p.Situacao = Situacao.Inativo;

                                if (!(resultado = p.Save(session)))
                                    return resultado;
                            }

                            g.Situacao = Situacao.Inativo;

                            if (!(resultado = g.Save(session)))
                                return resultado;
                        }
                    }

                    // Retira planos de contas de fornecedores que não sejam da categoria de débito
                    session.Update<Data.Model.Fornecedor>(new Data.Model.Fornecedor
                    {
                        // Limpa o campo da conta do fornecedor
                        IdConta = null
                    }, Colosoft.Query.ConditionalContainer.Parse("IdConta IN (?subConsulta)")
                        .Add("?subConsulta",
                            SourceContext.Instance.CreateQuery()
                                .From<Data.Model.PlanoContas>("p")
                                .Select("p.IdConta")
                                .InnerJoin<Data.Model.GrupoConta>("p.IdGrupo = g.IdGrupo", "g")
                                .LeftJoin<Data.Model.CategoriaConta>("g.IdCategoriaConta = c.IdCategoriaConta", "c")
                                .Where(
                                    "p.Situacao = ?situacao1 OR g.Situacao = ?situacao1 OR (c.Tipo <> ?tipo1 AND c.Tipo <> ?tipo2)")
                                .Add("?situacao1", Glass.Situacao.Inativo)
                                .Add("?situacao2", Glass.Situacao.Inativo)
                                .Add("?tipo1", Data.Model.TipoCategoriaConta.DespesaVariavel)
                                .Add("?tipo2", Data.Model.TipoCategoriaConta.DespesaFixa)),
                        "IdConta");
                }

                resultado = categoriaConta.Save(session);

                return !resultado ? resultado : session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga a categoria de conta.
        /// </summary>
        /// <param name="categoriaConta"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarCategoriaConta(Entidades.CategoriaConta categoriaConta)
        {
            categoriaConta.Require("categoriaConta").NotNull();

            // Verifica se esta categoria está sendo usada em algum grupo de conta
            if (SourceContext.Instance.CreateQuery()
                .From<Data.Model.GrupoConta>()
                .Where("IdCategoriaConta=?id")
                .Add("?id", categoriaConta.IdCategoriaConta)
                .ExistsResult())
                return new Colosoft.Business.DeleteResult(false, 
                    "Esta categoria não pode ser excluída por haver grupos de conta relacionadas à mesma.".GetFormatter());

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = categoriaConta.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        /// <summary>
        /// Altera a posição da categoria de conta.
        /// </summary>
        /// <param name="idCategoriaConta">Identificador da categoria que será movimentada.</param>
        /// <param name="moverParaCima">Identifica se é para mover para cima.</param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult AlterarPosicaoCategoriaConta(int idCategoriaConta, bool moverParaCima)
        {
            var categoriaConta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.CategoriaConta>()
                .Where("IdCategoriaConta=?id")
                .Add("?id", idCategoriaConta)
                .ProcessResult<Entidades.CategoriaConta>()
                .FirstOrDefault();


            using (var session = SourceContext.Instance.CreateSession())
            {
                // Altera a posição da categoria adjacente à esta
                session.Update<Data.Model.CategoriaConta>(
                    new Data.Model.CategoriaConta
                    {
                        NumeroSequencia = categoriaConta.NumeroSequencia + (moverParaCima ? 1 : -1)
                    }, Colosoft.Query.ConditionalContainer
                            .Parse("NumeroSequencia=?num")
                            .Add("?num", categoriaConta.NumeroSequencia + (moverParaCima ? -1 : 1)), 
                    "NumeroSequencia");


                // Altera a posição deste beneficiamento
                categoriaConta.NumeroSequencia += (moverParaCima ? -1 : 1);

                if (categoriaConta.NumeroSequencia < 0)
                    categoriaConta.NumeroSequencia = 0;

                categoriaConta.Save(session);

                return session.Execute(false).ToSaveResult();
            }
        }

        #endregion

        #region Grupo de Contas

        /// <summary>
        /// Pesquisa os grupos de conta do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.GrupoContaPesquisa> PesquisarGruposConta()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.GrupoConta>("gc")
                .Select("gc.IdGrupo, gc.IdCategoriaConta, c.Descricao AS Categoria, gc.Descricao, gc.Situacao, gc.PontoEquilibrio, gc.NumeroSequencia")
                .LeftJoin<Data.Model.CategoriaConta>("gc.IdCategoriaConta == c.IdCategoriaConta", "c")
                .OrderBy("NumeroSequencia")
                .ToVirtualResultLazy<Entidades.GrupoContaPesquisa>();
        }

        /// <summary>
        /// Pequisa os grupos de conta ignorando os ponto de equilibrio.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.GrupoContaPesquisa> PesquisarGruposContaIgnoraPE()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.GrupoConta>("gc")
                .Select("gc.IdGrupo, gc.IdCategoriaConta, c.Descricao AS Categoria, gc.Descricao, gc.Situacao, gc.PontoEquilibrio, gc.NumeroSequencia")
                .LeftJoin<Data.Model.CategoriaConta>("gc.IdCategoriaConta == c.IdCategoriaConta", "c")
                .OrderBy("NumeroSequencia")
                .Where("gc.PontoEquilibrio = ?pe").Add("?pe", false)
                .ToVirtualResultLazy<Entidades.GrupoContaPesquisa>();
        }

        /// <summary>
        /// Recupera os descritores dos grupos de conta do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemGruposConta()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.GrupoConta>()
                .Where(string.Format("IdGrupo NOT IN ({0})",
                    Glass.Data.Helper.UtilsPlanoConta.GetGruposExcluirFluxoSistema))
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.GrupoConta>()
                .ToList();
        }

        /// <summary>
        /// Recupera os descritores dos grupos de conta que podem ser selecionados para cadastro.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemGruposContaCadastro()
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.GrupoConta>()
                .Where(string.Format("IdGrupo NOT IN ({0})",
                    Glass.Data.Helper.UtilsPlanoConta.GetGruposSistema + "," +
                    Glass.Data.Helper.UtilsPlanoConta.GetGruposExcluirFluxoSistema))
                .OrderBy("Descricao");

            consulta
                .WhereClause
                .And("Situacao = ?sit")
                .Add("?sit", Glass.Situacao.Ativo);

            return consulta.ProcessResultDescriptor<Entidades.GrupoConta>()
                .ToList();
        }

        /// <summary>
        /// Recupera os grupos de contas associados com a categoria informada.
        /// </summary>
        /// <param name="idCategoriaConta">Identificador da categoria que será usada como filtro.</param>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemGruposContaPorCategoria(int idCategoriaConta)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.GrupoConta>()
                .OrderBy("Descricao");

            consulta.WhereClause
                    .And(string.Format("IdGrupo NOT IN ({0})", Glass.Data.Helper.UtilsPlanoConta.GetGruposExcluirFluxoSistema));

            if (idCategoriaConta > 0)
                consulta.WhereClause.And("IdCategoriaConta=?idCategoria")
                     .Add("?idCategoria", idCategoriaConta);

            return consulta.ProcessResultDescriptor<Entidades.GrupoConta>().ToList();

        }

        /// <summary>
        /// Recupera os dados do grupo de conta.
        /// </summary>
        /// <param name="idGrupo">Identificador do grupo.</param>
        /// <returns></returns>
        public Entidades.GrupoConta ObtemGrupoConta(int idGrupo)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.GrupoConta>()
                .Where("IdGrupo=?id")
                .Add("?id", idGrupo)
                .ProcessResult<Entidades.GrupoConta>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados do grupo de conta.
        /// </summary>
        /// <param name="grupoConta"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarGrupoConta(Entidades.GrupoConta grupoConta)
        {
            grupoConta.Require("grupoConta").NotNull();
            
            // Tratamento feito caso o identificador seja positivo e a instancia
            // esteja identifica como se não existisse no banco
            if (!grupoConta.ExistsInStorage && grupoConta.IdGrupo > 0)
                grupoConta.DataModel.ExistsInStorage = true;

            // Verifica se é um novo grupo
            if (!grupoConta.ExistsInStorage)
            {
                var idGrupo = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.GrupoConta>()
                    .Select("MAX(IdGrupo), COUNT(*)")
                    .Execute()
                    .Select(f => f.IsDBNull(0) ? 0 : f.GetInt32(0))
                    .FirstOrDefault() + 1;

                grupoConta.IdGrupo = idGrupo;
            }

            using (var session = SourceContext.Instance.CreateSession())
            {
                Colosoft.Business.SaveResult resultado = null;

                // Verifica se está atualizando o grupo de conta
                if (grupoConta.ExistsInStorage)
                {
                    //Inativa os planos de conta ssociados ao grupo caso o mesmo estaja sendo inativado
                    if (grupoConta.Situacao == Situacao.Inativo)
                    {
                        var planos = SourceContext.Instance.CreateQuery()
                            .From<Data.Model.PlanoContas>()
                            .Where("IdGrupo = " + grupoConta.IdGrupo)
                            .ProcessResult<Entidades.PlanoContas>();

                        foreach (var p in planos)
                        {
                            p.Situacao = Situacao.Inativo;

                            if (!(resultado = p.Save(session)))
                                return resultado;
                        }
                    }

                    // Retira planos de contas de fornecedores que não sejam da categoria de débito
                    session.Update<Data.Model.Fornecedor>(
                        new Data.Model.Fornecedor
                        {
                            IdConta = null
                        }, Colosoft.Query.ConditionalContainer
                            .Parse("IdConta IN (?subConsulta)")
                            .Add("?subConsulta",
                                SourceContext.Instance.CreateQuery()
                                    .From<Data.Model.PlanoContas>("p")
                                    .Select("p.IdConta")
                                    .InnerJoin<Data.Model.GrupoConta>("p.IdGrupo == g.IdGrupo", "g")
                                    .LeftJoin<Data.Model.CategoriaConta>("g.IdCategoriaConta == c.IdCategoriaConta", "c")
                                    .Where("p.Situacao = ?situacao1 OR g.Situacao = ?situacao2 OR c.Tipo NOT IN (?tipo1, ?tipo2) OR g.IdCategoriaConta IS NULL")
                                    .Add("?situacao1", Situacao.Inativo)
                                    .Add("?situacao2", Situacao.Inativo)
                                    .Add("?tipo1", Glass.Data.Model.TipoCategoriaConta.DespesaVariavel)
                                    .Add("?tipo2", Glass.Data.Model.TipoCategoriaConta.DespesaFixa)
                                    ),
                        "IdConta");
                }
                            
                resultado = grupoConta.Save(session);

                return !resultado ? resultado : session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga os dados do grupo de conta.
        /// </summary>
        /// <param name="grupoConta"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarGrupoConta(Entidades.GrupoConta grupoConta)
        {
            grupoConta.Require("grupoConta").NotNull();

            // Verifica se esta categoria está sendo usada em algum grupo de conta
            if (SourceContext.Instance.CreateQuery()
                .From<Data.Model.PlanoContas>()
                .Where("IdGrupo=?idGrupo")
                .Add("?idGrupo", grupoConta.IdGrupo)
                .ExistsResult())
                return new Colosoft.Business.DeleteResult(false, "Este grupo não pode ser excluído por haver planos de conta relacionadas ao mesmo.".GetFormatter());

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = grupoConta.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        /// <summary>
        /// Altera a posição do grupo de conta.
        /// </summary>
        /// <param name="idGrupo">Identificador do grupo de conta.</param>
        /// <param name="moverParaCima">Identifica se é para mover o grupo para cima.</param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult AlterarPosicaoGrupoConta(int idGrupo, bool moverParaCima)
        {
            var grupoConta = ObtemGrupoConta(idGrupo);

            if (grupoConta == null)
                return new Colosoft.Business.SaveResult(false, "O grupo não foi encontrado".GetFormatter());

            using (var session = SourceContext.Instance.CreateSession())
            {
                // Altera a posição do grupo adjacente à esta
                session.Update<Data.Model.GrupoConta>(
                    new Data.Model.GrupoConta
                    {
                        NumeroSequencia = grupoConta.NumeroSequencia
                    }, Colosoft.Query.ConditionalContainer
                               .Parse("NumeroSequencia=?numSeq")
                               .Add("?numSeq", grupoConta.NumeroSequencia + (moverParaCima ? -1 : 1)),
                    "NumeroSequencia");

                // Altera a posição deste grupo
                session.Update<Data.Model.GrupoConta>(
                    new Data.Model.GrupoConta
                    {
                        NumeroSequencia = grupoConta.NumeroSequencia + (moverParaCima ? -1 : 1)
                    }, Colosoft.Query.ConditionalContainer
                               .Parse("IdGrupo=?id")
                               .Add("?id", idGrupo),
                    "NumeroSequencia");

                return session.Execute(false).ToSaveResult();
                        
            }
        }

        #endregion

        #region Plano de Contas

        /// <summary>
        /// Pesquisa os planos de contas.
        /// </summary>
        /// <param name="idGrupo">Identificador do grupo para filtro.</param>
        /// <param name="situacao">Situação dos planos.</param>
        /// <returns></returns>
        public IList<Entidades.PlanoContasPesquisa> PesquisarPlanosContas(int idGrupo, Glass.Situacao? situacao)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.PlanoContas>("pc")
                .Select("pc.IdConta, pc.IdContaGrupo, pc.IdGrupo, pc.Descricao, pc.ExibirDre, pc.Situacao, gc.Descricao AS Grupo, gc.IdCategoriaConta, cc.Descricao AS Categoria, cc.Tipo AS CategoriaTipo")
                .InnerJoin<Data.Model.GrupoConta>("pc.IdGrupo = gc.IdGrupo", "gc")
                .LeftJoin<Data.Model.CategoriaConta>("gc.IdCategoriaConta = cc.IdCategoriaConta", "cc")
                .OrderBy("CategoriaTipo, Grupo, Descricao");

            if (idGrupo > 0)
                consulta.WhereClause.And("IdGrupo=?idGrupo").Add("?idGrupo", idGrupo);

            if (situacao != null)
                consulta.WhereClause.And("Situacao=?situacao").Add("?situacao", situacao.GetValueOrDefault());

            return consulta.ToVirtualResult<Entidades.PlanoContasPesquisa>();
        }

        /// <summary>
        /// Recupera os descritores dos planos de contas.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.PlanoContasDescritor> ObtemPlanosContas()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.PlanoContas>()
                .InnerJoin<Data.Model.GrupoConta>("p.IdGrupo = gc.IdGrupo", "gc")
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.PlanoContas>()
                .Select(f => (Entidades.PlanoContasDescritor)f)
                .ToList();
        }

        /// <summary>
        /// Recupera os dados do plano de contas.
        /// </summary>
        /// <param name="idConta"></param>
        /// <returns></returns>
        public Entidades.PlanoContas ObtemPlanoContas(int idConta)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.PlanoContas>()
                .Where("IdConta=?id")
                .Add("?id", idConta)
                .ProcessResult<Entidades.PlanoContas>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Recupera os planos de contas vinculados a um plano de conta contabil
        /// </summary>
        /// <param name="idPlanoContabil"></param>
        /// <returns></returns>
        public IList<Entidades.PlanoContas> ObterPlanosContasVincularPlanoContas(int idContaContabil)
        {
            var consulta = SourceContext.Instance.CreateQuery()
               .From<Data.Model.PlanoContas>("")
               .Where("IdContaContabil = " + idContaContabil);

            return consulta.ToVirtualResult<Entidades.PlanoContas>();
        }

        /// <summary>
        /// Recupera o identificador do plano de contas.
        /// </summary>
        /// <param name="idContaGrupo">Identificador do plano dentro da grupo.</param>
        /// <param name="idGrupo">Identificador do grupo.</param>
        /// <returns></returns>
        public int ObtemIdPlanoContas(int idContaGrupo, int idGrupo)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.PlanoContas>()
                .Where("IdGrupo=?idGrupo AND IdContaGrupo=?idContaGrupo")
                .Add("?idGrupo", idGrupo)
                .Add("?idContaGrupo", idContaGrupo)
                .Select("IdConta")
                .Execute()
                .Select(f => f.GetInt32(0))
                .FirstOrDefault();
        }

        /// <summary>
        /// Verifica se o plano de contas está em uso.
        /// </summary>
        /// <param name="idConta"></param>
        /// <param name="usarConfiguracao">Identifica se este método está sendo chamado da tela de configurações.</param>
        /// <returns></returns>
        public bool PlanoContasEmUso(int idConta, bool usarConfiguracao)
        {
            var quantidadeUso = 0;
            var callBack = new Colosoft.Query.QueryCallBack((sender, query, result) =>
                {
                    // Soma quantidade de registro que utiliza
                    quantidadeUso += result.Select(f => f.GetInt32(0)).FirstOrDefault();
                });

            var multiConsulta = SourceContext.Instance.CreateMultiQuery()
                .Add(SourceContext.Instance.CreateQuery()
                        .From<Data.Model.CaixaGeral>()
                        .Where("IdConta=?id").Add("?id", idConta)
                        .Count(), callBack)

                .Add(SourceContext.Instance.CreateQuery()
                        .From<Data.Model.MovBanco>()
                        .Where("IdConta=?id").Add("?id", idConta)
                        .Count(), callBack)

                .Add(SourceContext.Instance.CreateQuery()
                        .From<Data.Model.MovFunc>()
                        .Where("IdConta=?id").Add("?id", idConta)
                        .Count(), callBack)

                .Add(SourceContext.Instance.CreateQuery()
                        .From<Data.Model.CaixaDiario>()
                        .Where("IdConta=?id").Add("?id", idConta)
                        .Count(), callBack)

                .Add(SourceContext.Instance.CreateQuery()
                        .From<Data.Model.CustoFixo>()
                        .Where("IdConta=?id").Add("?id", idConta)
                        .Count(), callBack)

                .Add(SourceContext.Instance.CreateQuery()
                        .From<Data.Model.NotaFiscal>()
                        .Where("IdConta=?id").Add("?id", idConta)
                        .Count(), callBack)

                .Add(SourceContext.Instance.CreateQuery()
                        .From<Data.Model.Fornecedor>()
                        .Where("IdConta=?id").Add("?id", idConta)
                        .Count(), callBack)

                .Add(SourceContext.Instance.CreateQuery()
                        .From<Data.Model.ContasReceber>()
                        .Where("IdConta=?id").Add("?id", idConta)
                        .Count(), callBack)

                .Add(SourceContext.Instance.CreateQuery()
                        .From<Data.Model.ContasPagar>()
                        .Where("IdConta=?id").Add("?id", idConta)
                        .Count(), callBack)

                .Add(SourceContext.Instance.CreateQuery()
                        .From<Data.Model.Compra>()
                        .Where("IdConta=?id").Add("?id", idConta)
                        .Count(), callBack)

                .Add(SourceContext.Instance.CreateQuery()
                        .From<Data.Model.ImpostoServ>()
                        .Where("IdConta=?id").Add("?id", idConta)
                        .Count(), callBack);

            if (!usarConfiguracao)
            {
                multiConsulta.Add(
                    SourceContext.Instance.CreateQuery()
                        .From<Data.Model.ConfiguracaoLoja>()
                        .Where("ValorInteiro=?id AND IdConfig IN (?ids)")
                        .Add("?id", idConta)
                        .Add("?ids", new Glass.Data.Helper.Config.ConfigEnum[]
                        {
                            Glass.Data.Helper.Config.ConfigEnum.PlanoContaTaxaAntecip,
                            Glass.Data.Helper.Config.ConfigEnum.PlanoContaComissao,
                            Glass.Data.Helper.Config.ConfigEnum.PlanoContaJurosAntecip,
                            Glass.Data.Helper.Config.ConfigEnum.PlanoContaIOFAntecip,
                            Glass.Data.Helper.Config.ConfigEnum.PlanoContaJurosReceb,
                            Glass.Data.Helper.Config.ConfigEnum.PlanoContaMultaReceb,
                            Glass.Data.Helper.Config.ConfigEnum.PlanoContaJurosPagto,
                            Glass.Data.Helper.Config.ConfigEnum.PlanoContaMultaPagto,
                            Glass.Data.Helper.Config.ConfigEnum.PlanoContaEstornoJurosReceb,
                            Glass.Data.Helper.Config.ConfigEnum.PlanoContaEstornoMultaReceb,
                            Glass.Data.Helper.Config.ConfigEnum.PlanoContaEstornoJurosPagto,
                            Glass.Data.Helper.Config.ConfigEnum.PlanoContaEstornoMultaPagto
                        }));
            }


            multiConsulta.Execute();

            return quantidadeUso > 0; 
        }

        /// <summary>
        /// Salva os dados do plano de contas.
        /// </summary>
        /// <param name="planoContas"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarPlanoContas(Entidades.PlanoContas planoContas)
        {
            planoContas.Require("planoContas").NotNull();

            // Tratamento feito caso o identificador seja positivo e a instancia
            // esteja identifica como se não existisse no banco
            if (!planoContas.ExistsInStorage && planoContas.IdConta > 0)
                planoContas.DataModel.ExistsInStorage = true;

            // Verifica se é um novo plano de contas
            if (!planoContas.ExistsInStorage)
            {
                var idContaGrupo = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.PlanoContas>()
                    .Select("MAX(IdContaGrupo), COUNT(*)")
                    .Where("IdGrupo=?idGrupo")
                    .Add("?idGrupo", planoContas.IdGrupo)
                    .Execute()
                    .Select(f => f.IsDBNull(0) ? 0 : f.GetInt32(0))
                    .FirstOrDefault() + 1;

                planoContas.IdContaGrupo = idContaGrupo;
            }

            //Se um plano contabil for informado verifica se ja nao a um outro vinculo
            if (planoContas.IdContaContabil.GetValueOrDefault(0) > 0)
            {
                var planoAntigo = ObtemPlanoContas(planoContas.IdConta);

                if(planoAntigo.IdContaContabil.GetValueOrDefault(0)> 0 && planoAntigo.IdContaContabil.Value != planoContas.IdContaContabil.Value)
                    return new Colosoft.Business.SaveResult(false, "O plano de contas informado já esta vinculado a um plano de contas contábil".GetFormatter());
            }
         
            using (var session = SourceContext.Instance.CreateSession())
            {
                // Verifica se está atualizando o grupo de conta
                if (planoContas.ExistsInStorage)
                {
                    // Retira planos de contas de fornecedores que não sejam da categoria de débito
                    session.Update<Data.Model.Fornecedor>(
                        new Data.Model.Fornecedor
                        {
                            IdConta = null
                        }, Colosoft.Query.ConditionalContainer
                            .Parse("IdConta IN (?subConsulta)")
                            .Add("?subConsulta",
                                SourceContext.Instance.CreateQuery()
                                    .From<Data.Model.PlanoContas>("p")
                                    .Select("p.IdConta")
                                    .InnerJoin<Data.Model.GrupoConta>("p.IdGrupo == g.IdGrupo", "g")
                                    .LeftJoin<Data.Model.CategoriaConta>("g.IdCategoriaConta == c.IdCategoriaConta", "c")
                                    .Where("p.Situacao = ?situacao1 OR g.Situacao = ?situacao2 OR c.Tipo NOT IN (?tipo1, ?tipo2) OR g.IdCategoriaConta IS NULL")
                                    .Add("?situacao1", Situacao.Inativo)
                                    .Add("?situacao2", Situacao.Inativo)
                                    .Add("?tipo1", Glass.Data.Model.TipoCategoriaConta.DespesaVariavel)
                                    .Add("?tipo2", Glass.Data.Model.TipoCategoriaConta.DespesaFixa)
                                    ),
                        "IdConta");
                }

                var resultado = planoContas.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }
       
        /// <summary>
        /// Apaga o plano de contas.
        /// </summary>
        /// <param name="planoContas"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarPlanoContas(Entidades.PlanoContas planoContas)
        {
            planoContas.Require("planoContas").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = planoContas.Delete(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        #region Membros de IValidadorPlanoContas

        /// <summary>
        /// Valida a existencia do plano de contas.
        /// </summary>
        /// <param name="planoContas"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IValidadorPlanoContas.ValidaExistencia(Entidades.PlanoContas planoContas)
        {
            if (PlanoContasEmUso(planoContas.IdConta, false))
                return new IMessageFormattable[]
                {
                    "Este plano de conta não pode ser excluído. Existem registros relacionados ao mesmo.".GetFormatter()
                };

            return new IMessageFormattable[0];
        }

        #endregion

        #endregion
    }
}
