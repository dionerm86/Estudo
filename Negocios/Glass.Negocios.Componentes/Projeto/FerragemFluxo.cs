using Colosoft;
using System.Collections.Generic;
using System.Linq;
using CalcEngine;
using Glass.Data.DAL;

namespace Glass.Projeto.Negocios.Componentes
{

    public class FerragemFluxo : IFerragemFluxo, Entidades.IValidadorFabricanteFerragem, Entidades.IValidadorFerragem
    {
        #region Local Variables

        private FerragemCache _cache;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        /// <param name="cache"></param>
        public FerragemFluxo(FerragemCache cache)
        {
            _cache = cache;
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Cria uma instancia do sincronizador das ferragens.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private CADProject.Remote.Client.PartTemplateSynchronizer CriarSincronizador(CADProject.Remote.Client.IPartTemplateSynchronizerSource source)
        {
            var urlServicoCadProject = Configuracoes.ProjetoConfig.UrlServicoCadProject;

            var repository = new CADProject.Remote.Client.PartTemplateRepository(new System.Uri(urlServicoCadProject + "/Services/PartTemplateRepositoryService.svc"));

            return new CADProject.Remote.Client.PartTemplateSynchronizer(repository, source);
        }

        /// <summary>
        /// Método acionado quando a ferragem foi sincronizada com o CadProjet.
        /// </summary>
        /// <param name="ferragem"></param>
        private void FerragemSincronizada(Entidades.Ferragem ferragem)
        {

        }

        /// <summary>
        /// Método acionado quando ocorrer uma falha na sincronização da ferragem.
        /// </summary>
        /// <param name="ferragem"></param>
        /// <param name="mensagem"></param>
        private void FalhaSicronizacaoFerragem(Entidades.Ferragem ferragem, string mensagem)
        {

        }


        #endregion

        #region Ferragem

        /// <summary>
        /// Cria uma nova Instancia de Ferragem
        /// </summary>
        /// <returns></returns>
        public Entidades.Ferragem CriarFerragem()
        {
            return SourceContext.Instance.Create<Entidades.Ferragem>();
        }

        /// <summary>
        /// Pesquisa as Ferragens
        /// </summary>
        public IList<Entidades.FerragemPesquisa> PesquisarFerragem(string nomeFerragem, int idFabricanteFerragem, string codigo)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .Select("f.IdFerragem, f.Nome, f.Situacao, ff.Nome AS NomeFabricante, f.EstiloAncoragem, f.Altura, f.Largura, f.DataAlteracao")
                .From<Data.Model.Ferragem>("f")
                .LeftJoin<Data.Model.FabricanteFerragem>("f.IdFabricanteFerragem=ff.IdFabricanteFerragem", "ff")
                .GroupBy("f.IdFerragem");

            if (!string.IsNullOrWhiteSpace(nomeFerragem))
                consulta.WhereClause
                    .And("f.Nome LIKE ?nomeFerragem")
                    .Add("?nomeFerragem", nomeFerragem);

            if (idFabricanteFerragem > 0)
                consulta.WhereClause
                    .And("f.IdFabricanteFerragem=?idFabricanteFerragem")
                    .Add("?idFabricanteFerragem", idFabricanteFerragem);

            if (!string.IsNullOrEmpty(codigo))
                consulta.WhereClause
                    .And("f.IdFerragem IN ?idsFerragem")
                    .Add("?idsFerragem",
                        SourceContext.Instance.CreateQuery()
                            .Select("cf.IdFerragem")
                            .From<Data.Model.CodigoFerragem>("cf")
                            .Where("cf.Codigo LIKE ?codigo")
                            .Add("?codigo", string.Format("%{0}%", codigo)));

            // Essa Query é necessário para o count funcionar corretamente devido ao GroupBy.
            return SourceContext.Instance.CreateQuery()
                .From(consulta, "temp")
                .ToVirtualResult<Entidades.FerragemPesquisa>();
        }

        /// <summary>
        /// Recupera os dados de Ferragem.
        /// </summary>
        public Entidades.Ferragem ObterFerragem(int idFerragem)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Ferragem>()
                .Where("IdFerragem=?id")
                .Add("?id", idFerragem)
                .ProcessLazyResult<Entidades.Ferragem>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Recupera os dados de Ferragem.
        /// </summary>
        public Entidades.Ferragem ObterFerragem(string nomeFerragem)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Ferragem>()
                .Where("Nome=?nome")
                .Add("?nome", nomeFerragem)
                .ProcessLazyResult<Entidades.Ferragem>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Altera a situação da Ferragem
        /// </summary>
        /// <param name="ferragem"></param>
        public Colosoft.Business.SaveResult AtivarInativarFerragem(Entidades.Ferragem ferragem)
        {
            // Altera a situação da ferragem.
            ferragem.Situacao = ferragem.Situacao == Situacao.Ativo ? Situacao.Inativo : Situacao.Ativo;

            return SalvarFerragem(ferragem);
        }

        /// <summary>
        /// Salva os dados da ferragem.
        /// Os dados devem ser atualizados primeiramente no CadProject, para evitar que uma ferragem fique correta no WebGlass e incorreta nele.
        /// </summary>
        public Colosoft.Business.SaveResult SalvarFerragem(Entidades.Ferragem ferragem)
        {
            ferragem.Require("cliente").NotNull();

            var ferragens = this.ObterFerragem(ferragem.IdFerragem);

            var retornoAtualizacao = string.Empty;

            // Se for inserção adiciona a situação.
            if (!ferragem.ExistsInStorage)
            {
                ferragem.Situacao = Situacao.Ativo;
            }

            if (ferragem.Situacao == Situacao.Ativo)
            {
                this._cache.Atualizar(ferragem);
            }
            else
            {
                this._cache.Apagar(ferragem);
            }

            var repositorio = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<Entidades.IFerragemRepositorioCalcPackage>();
            var source = new PartTemplateSynchronizerSource(repositorio);
            source.Atualizar(ferragem, this.FerragemSincronizada, (f, mensagem) => { retornoAtualizacao = mensagem; });

            var sincronizador = this.CriarSincronizador(source);

            //Executa a sincronização
            sincronizador.Synchronize();

            /* Chamado 65883. */
            if (!string.IsNullOrEmpty(retornoAtualizacao))
            {
                return new Colosoft.Business.SaveResult(
                    false,
                    string.Format(
                        "Falha ao atualizar a ferragem no CadProject. Erro: {0}.",
                        retornoAtualizacao).GetFormatter());
            }

            Colosoft.Business.SaveResult resultado = null;

            using (var session = SourceContext.Instance.CreateSession())
            {
                resultado = ferragem.Save(session);

                if (!resultado)
                {
                    return resultado;
                }

                resultado = session.Execute(false).ToSaveResult();
            }

            /* Chamado 65883. */
            if (!resultado)
            {
                source.Apagar(ferragem, this.FerragemSincronizada, (f, mensagem) => { retornoAtualizacao = mensagem; });
                sincronizador.Synchronize();

                return new Colosoft.Business.SaveResult(
                false,
                string.Format(
                "Falha ao atualizar a ferragem no WebGlass. Erro: {0}.",
                resultado.Message.ToString()).GetFormatter());
            }

            LogAlteracaoDAO.Instance.LogFerragem(ferragem.DataModel, LogAlteracaoDAO.SequenciaObjeto.Atual);

            return resultado;
        }

        /// <summary>
        /// Apaga os dados da Ferragem.
        /// Os dados devem ser apagados primeiramente no CadProject, para evitar que uma ferragem exista nele e não exista no WebGlass.
        /// </summary>
        public Colosoft.Business.DeleteResult ApagarFerragem(Entidades.Ferragem ferragem)
        {
            ferragem.Require("ferragem").NotNull();

            #region Atualiza os dados da ferragem no WebGlass

            Colosoft.Business.DeleteResult resultado = null;

            using (var session = SourceContext.Instance.CreateSession())
            {
                resultado = ferragem.Delete(session);
                if (!resultado)
                    return resultado;

                resultado = session.Execute().ToDeleteResult();
            }

            /* Chamado 65883. */
            if (!resultado)
                return new Colosoft.Business.DeleteResult(false, string.Format("Falha ao apagar a ferragem no WebGlass. Erro: {0}.",
                    resultado.Message.ToString()).GetFormatter());

            #endregion

            #region Atualiza os dados da ferragem no CadProject

            var retornoDelecao = string.Empty;
            _cache.Apagar(ferragem);

            var repositorio = Microsoft.Practices.ServiceLocation.ServiceLocator
                .Current.GetInstance<Entidades.IFerragemRepositorioCalcPackage>();

            var source = new PartTemplateSynchronizerSource(repositorio);
            source.Apagar(ferragem, f => { }, (f, mensagem) => { retornoDelecao = mensagem; });

            /* Chamado 65883. */
            if (!string.IsNullOrEmpty(retornoDelecao))
                return new Colosoft.Business.DeleteResult(false, string.Format("Falha ao apagar a ferragem no CadProject. Erro: {0}. IMPORTANTE: a ferragem foi apagada no WebGlass.", retornoDelecao).GetFormatter());

            var sincronizador = CriarSincronizador(source);

            // Executa a sincronização
            sincronizador.Synchronize();

            #endregion

            return resultado;
        }

        public IMessageFormattable[] ValidarAtualizacao(Entidades.Ferragem ferragem)
        {
            var mensagens = new List<string>();

            // Handler para criar a consulta padrão da existencia do registro
            var consulta = SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Ferragem>()
                    .Where("Nome=?nome")
                    .Add("?nome", ferragem.Nome);

            if (ferragem.ExistsInStorage)
            {
                consulta.WhereClause
                    .And("IdFerragem <> ?idFerragem")
                    .Add("?idFerragem", ferragem.IdFerragem);
            }

            if (consulta.ExistsResult())
            {
                mensagens.Add("Já existe uma ferragem cadastrada com esse nome.");
            }

            if (!Data.Helper.UserInfo.GetUserInfo.IsAdminSync && (ferragem.Constantes.Where(f => f.ChangedProperties.Contains("Nome")).HasItems() ||
                ferragem.ChangedProperties.Contains("MedidasEstaticas") || ferragem.ChangedProperties.Contains("PodeEspelhar") ||
                ferragem.ChangedProperties.Contains("PodeRotacionar") || ferragem.ChangedProperties.Contains("EstiloAncoragem")))
            {
                mensagens.Add("Você não tem permissão para alterar os valores. Pode Espelhar, Pode Rotacionar, Estilo Acoragem e Nome da Constante");
            }

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        #endregion

        #region Fabricante Ferragem

        /// <summary>
        /// Pesquisa os Fabricantes de Ferragem
        /// </summary>
        /// <param name="nome"></param>
        /// <param name="sitio"></param>
        /// <returns></returns>
        public IList<Entidades.FabricanteFerragemPesquisa> PesquisarFabricanteFerragem(string nome, string sitio)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .Select("ff.IdFabricanteFerragem, ff.Nome, ff.Sitio")
                .From<Data.Model.FabricanteFerragem>("ff")
                .OrderBy("ff.Nome");

            if (!string.IsNullOrEmpty(nome))
                consulta.WhereClause
                    .And("Nome LIKE ?nome")
                    .Add("?nome", nome);

            if (!string.IsNullOrEmpty(sitio))
                consulta.WhereClause
                    .And("Sitio LIKE ?sitio")
                    .Add("?sitio", sitio);

            return consulta.ToVirtualResult<Entidades.FabricanteFerragemPesquisa>();
        }

        /// <summary>
        /// Recupera os dados do Fabricante de Ferragem.
        /// </summary>
        /// <param name="idFabricanteFerragem"></param>
        public Entidades.FabricanteFerragem ObterFabricanteFerragem(int idFabricanteFerragem)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.FabricanteFerragem>()
                .Where("IdFabricanteFerragem=?id")
                .Add("?id", idFabricanteFerragem)
                .ProcessLazyResult<Entidades.FabricanteFerragem>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Recupera os descritores dos fabricantes de ferragem.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObterFabricantesFerragem()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.FabricanteFerragem>()
                .OrderBy("Nome")
                .ProcessResultDescriptor<Entidades.FabricanteFerragem>()
                .ToList();
        }

        /// <summary>
        /// Valida a atualização do Fabricante de Ferragem.
        /// </summary>
        public Colosoft.Business.OperationResult ValidarAtualizacao(Entidades.FabricanteFerragem fabricanteFerragem)
        {
            // Verifica se já existe fabricante com nome informado
            if (SourceContext.Instance.CreateQuery()
                .From<Data.Model.FabricanteFerragem>()
                .Where("Nome=?nome AND IdFabricanteFerragem <> ?id")
                .Add("?nome", fabricanteFerragem.Nome)
                .Add("?id", fabricanteFerragem.IdFabricanteFerragem)
                .ExistsResult())
            {
                return new Colosoft.Business.OperationResult(false, $"Já existe um fabricante cadastrado com o nome {fabricanteFerragem.Nome}.".GetFormatter());
            }

            return new Colosoft.Business.OperationResult(true, null);
        }

        /// <summary>
        /// Verifica se o fabricante está sendo utilizado no cadastro de ferragem.
        /// </summary>
        public Colosoft.Business.OperationResult ValidarExclusao(Entidades.FabricanteFerragem fabricanteFerragem)
        {
            if (SourceContext.Instance.CreateQuery()
                .From<Data.Model.Ferragem>("f")
                .Where("IdFabricanteFerragem=?idFabricanteFerragem")
                .Add("?idFabricanteFerragem", fabricanteFerragem.IdFabricanteFerragem)
                .ExistsResult())
                return new Colosoft.Business.OperationResult(false, "Esse fabricante de ferragem está sendo utilizado no cadastro de ferragem.".GetFormatter());

            return new Colosoft.Business.OperationResult(true, null);
        }

        /// <summary>
        /// Salva os dados do Fabricante de Ferragem
        /// </summary>
        public Colosoft.Business.SaveResult SalvarFabricanteFerragem(Entidades.FabricanteFerragem fabricanteFerragem)
        {
            fabricanteFerragem.Require("fabricanteFerragem").NotNull();
            Colosoft.Business.SaveResult resultado = null;

            using (var session = SourceContext.Instance.CreateSession())
            {
                resultado = fabricanteFerragem.Save(session);
                if (!resultado)
                    return resultado;

                resultado = session.Execute().ToSaveResult();
            }

            _cache.Atualizar(fabricanteFerragem);

            return resultado;
        }

        /// <summary>
        /// Apaga os dados do Fabricante de Ferragem.
        /// </summary>
        /// <param name="fabricanteFerragem"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarFabricanteFerragem(Entidades.FabricanteFerragem fabricanteFerragem)
        {
            fabricanteFerragem.Require("fabricanteFerragem").NotNull();

            var fabricanteEmUso = SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.Ferragem>("f")
                .Where("f.IdFabricanteFerragem=?idFabricanteFerragem")
                .Add("?idFabricanteFerragem", fabricanteFerragem.IdFabricanteFerragem)
                .ExistsResult();

            if (fabricanteEmUso)
                return new Colosoft.Business.DeleteResult(false, string.Format("O fabricante ferragem não pode ser excluído pois está associado à ferragens.").GetFormatter());

            Colosoft.Business.DeleteResult resultado = null;

            using (var session = SourceContext.Instance.CreateSession())
            {
                resultado = fabricanteFerragem.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute().ToDeleteResult();
            }

            return resultado;
        }

        #endregion
    }
}
