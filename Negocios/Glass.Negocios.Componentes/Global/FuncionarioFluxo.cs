using System;
using System.Collections.Generic;
using System.Linq;
using Colosoft;
using Glass.Data.Helper;
using Glass.Global.Negocios.Entidades;

namespace Glass.Global.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio dos funcionários.
    /// </summary>
    public class FuncionarioFluxo : 
        Negocios.IFuncionarioFluxo,
        Entidades.IProvedorFuncionario
    {
        #region Funcionário

        /// <summary>
        /// Recupera os funcionários ativos que são vendedores ou estão 
        /// associados como vendedores para os clientes.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObterFuncionariosGraficoOrcamentoVenda()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Funcionario>("f")
                .Where("(Situacao=?situacao AND (IdTipoFunc=?tipoFunc OR IdFunc IN ?subFunc)) OR EXISTS (?sqlFuncCliente)")
                    .Add("?situacao", Situacao.Ativo)
                    .Add("?tipoFunc", Data.Helper.Utils.TipoFuncionario.Vendedor)
                    .Add("?sqlFuncCliente", SourceContext.Instance.CreateQuery()
                        .From<Data.Model.Cliente>()
                        .Where("IdFunc=f.IdFunc")
                        .Select("IdCli"))
                    .Add("?subFunc",
                        SourceContext.Instance.CreateQuery()
                            .From<Data.Model.ConfigFuncaoFunc>()
                            .Select("IdFunc")
                            .Where("IdFuncaoMenu=?idFuncaoMenu")
                            .Add("?idFuncaoMenu", Config.ObterIdFuncaoMenu(Config.FuncaoMenuOrcamento.EmitirOrcamento)))
                .OrderBy("Nome")
                .ProcessResultDescriptor<Entidades.Funcionario>()
                .ToList();
        }

        /// <summary>
        /// Recupera os funcionários ativos que são vendedores ou estão 
        /// associados como vendedores para os clientes.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObterFuncionariosAtivosAssociadosAClientes()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Funcionario>("f")
                .Where("(Situacao=?situacao) OR EXISTS (?sqlFuncCliente)")
                    .Add("?situacao", Situacao.Ativo)
                    .Add("?sqlFuncCliente", SourceContext.Instance.CreateQuery()
                        .From<Data.Model.Cliente>()
                        .Where("IdFunc=f.IdFunc")
                        .Select("IdCli"))
                .OrderBy("Nome")
                .ProcessResultDescriptor<Entidades.Funcionario>()
                .ToList();
        }

        /// <summary>
        /// Recupera os descritores dos funcionários ativos
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemFuncionariosAtivos()
        {
            return SourceContext.Instance.CreateQuery()
             .From<Data.Model.Funcionario>()
             .Where("Situacao=?situacao")
             .Add("?situacao", Glass.Situacao.Ativo)
             .OrderBy("Nome")
             .ProcessResultDescriptor<Entidades.Funcionario>()
             .ToList();
        }

        /// <summary>
        /// Cria uma nova instancia do funcionario.
        /// </summary>
        /// <returns></returns>
        public Entidades.Funcionario CriarFuncionario()
        {
            return SourceContext.Instance.Create<Entidades.Funcionario>();
        }

        /// <summary>
        /// Pesquisa os funcionários.
        /// </summary>
        /// <param name="idLoja">Identificador da loja.</param>
        /// <param name="nomeFuncionario">Nome do funcionário que será usado no filtro.</param>
        /// <param name="situacao">Situação do funcionário.</param>
        /// <param name="apenasRegistrados">Identifica se é para recupera apenas registrados.</param>
        /// <param name="idTipoFunc">Identificador do tipo de funcionário.</param>
        /// <param name="idSetor"></param>
        /// <param name="dataNascInicio"></param>
        /// <param name="dataNascFim"></param>
        /// <returns></returns>
        public IList<Entidades.FuncionarioPesquisa> PesquisarFuncionarios(
            int? idLoja, string nomeFuncionario, Glass.Situacao? situacao,
            bool apenasRegistrados, int? idTipoFunc, int? idSetor,
            DateTime? dataNascInicio, DateTime? dataNascFim)
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Funcionario>("f")
                .InnerJoin<Data.Model.Loja>("f.IdLoja = l.IdLoja", "l")
                .InnerJoin<Data.Model.TipoFuncionario>("f.IdTipoFunc = tf.IdTipoFuncionario", "tf")
                .OrderBy("Nome")
                .Select(
                    @"f.IdFunc, f.IdLoja, f.IdTipoFunc, f.Cpf, f.Rg, f.TelRes, f.TelCel, f.Nome,
                    f.Funcao, f.Salario, f.DataEnt, f.DataNasc, ISNULL(l.NomeFantasia, l.RazaoSocial) AS Loja,
                    tf.Descricao AS TipoFuncionario, f.AdminSync");

            var whereClause = consulta.WhereClause;

            if (idLoja.HasValue && idLoja.Value > 0)
                whereClause
                    .And("f.IdLoja=?idLoja")
                    .Add("?idLoja", idLoja.Value)
                    .AddDescription(() => 
                        string.Format("Loja: {0}",
                            SourceContext.Instance.CreateQuery()
                            .From<Data.Model.Loja>()
                            .Where("IdLoja=?id")
                            .Add("?id", idLoja.Value)
                            .Select("NomeFantasia, RazaoSocial")
                            .Execute()
                            .Select(f => string.IsNullOrEmpty(f.GetString(0)) ? f.GetString(1) : f.GetString(0))
                            .FirstOrDefault()));

            if (!string.IsNullOrEmpty(nomeFuncionario))
                whereClause
                    .And("f.Nome LIKE ?nomeFunc")
                    .Add("?nomeFunc", string.Format("%{0}%", nomeFuncionario))
                    .AddDescription(string.Format("Funcionário: {0}", nomeFuncionario));

            if (situacao.HasValue)
                whereClause
                    .And("f.Situacao=?situacao")
                    .Add("?situacao", situacao.Value)
                    .AddDescription(string.Format("Situação: {0}", situacao.Translate()));

            if (apenasRegistrados)
                whereClause
                    .And("f.Registrado=1")
                    .AddDescription("Apenas registrados");

            if (idTipoFunc.HasValue && idTipoFunc.Value > 0)
                whereClause
                    .And("f.IdTipoFunc=?idTipoFunc")
                    .Add("?idTipoFunc", idTipoFunc.Value)
                    .AddDescription(() =>
                        string.Format("Tipo funcionário: {0}",
                            SourceContext.Instance.CreateQuery()
                            .From<Data.Model.TipoFuncionario>()
                            .Where("IdTipoFuncionario=?id")
                            .Add("?id", idTipoFunc.Value)
                            .Select("Descricao")
                            .Execute()
                            .Select(f => f.GetString(0))
                            .FirstOrDefault()));

            if (idSetor.HasValue && idSetor.Value > 0)
            {
                whereClause
                    .And("f.IdFunc IN (?setores)")
                    .Add("?setores",
                        SourceContext.Instance.CreateQuery()
                        .From<Data.Model.FuncionarioSetor>("fs")
                        .Select("fs.IdFunc")
                        .Where("fs.IdSetor=?idSetor")
                        .Add("?idSetor", idSetor))
                    .AddDescription(() =>
                        string.Format("Setor: {0}",
                            SourceContext.Instance.CreateQuery()
                            .From<Data.Model.Setor>()
                            .Where("IdSetor=?id")
                            .Add("?id", idSetor.Value)
                            .Select("Descricao")
                            .Execute()
                            .Select(f => f.GetString(0))
                            .FirstOrDefault()));
            }

            if (dataNascInicio > DateTime.MinValue)
                whereClause
                    .And("f.DataNasc >= ?dataNascInicio")
                    .Add("?dataNascInicio", dataNascInicio.Value.Date)
                    .AddDescription(string.Format("Data de Nasc. Inicial: {0:dd/MM/yyyy}", dataNascInicio.Value));

            if (dataNascFim > DateTime.MinValue)
                whereClause
                    .And("f.DataNasc <= ?dataNascFim")
                    .Add("?dataNascFim", dataNascFim.Value.Date.AddDays(1).AddSeconds(-1))
                    .AddDescription(string.Format("Data de Nasc. Fim: {0:dd/MM/yyyy}", dataNascFim.Value));

            return consulta.ToVirtualResult<Entidades.FuncionarioPesquisa>();
        }

        /// <summary>
        /// Recupera os descritores dos funcionários que são vendedores.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemVendedores()
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Funcionario>()
                .OrderBy("Nome")
                .Where("Situacao=?situacao")
                .Add("?situacao", Glass.Situacao.Ativo);

            consulta.WhereClause
                .And("(IdFunc IN ?subFunc OR IdFunc=?atual)")
                .Add("?subFunc",
                    SourceContext.Instance.CreateQuery()
                        .From<Data.Model.ConfigFuncaoFunc>()
                        .Select("IdFunc")
                        .Where("IdFuncaoMenu=?idFuncaoMenu")
                        .Add("?idFuncaoMenu", Config.ObterIdFuncaoMenu(Config.FuncaoMenuPedido.EmitirPedido)))
                .Add("?atual",
                    Glass.Data.Helper.UserInfo.GetUserInfo != null ?
                    Glass.Data.Helper.UserInfo.GetUserInfo.CodUser : 0);

            return consulta.ProcessResultDescriptor<Entidades.Funcionario>().ToList();
        }

        /// <summary>
        /// Recupera os dados dos funcionarios pelos nomes informados.
        /// </summary>
        /// <param name="nomes"></param>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemFuncionarios(IEnumerable<string> nomes)
        {
            nomes.Require("nomes").NotNull();

            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Funcionario>();

            var enumerator = nomes.GetEnumerator();

            if (!enumerator.MoveNext())
                return new List<Colosoft.IEntityDescriptor>();

            var count = 0;

            do
            {
                consulta.WhereClause
                    .And(string.Format("Nome LIKE ?nome{0}", count))
                    .Add(string.Format("?nome{0}", count++), string.Format("%{0}%", enumerator.Current));

            } while (enumerator.MoveNext());

            return consulta.ProcessResultDescriptor<Entidades.Funcionario>().ToList();
        }

        /// <summary>
        /// Recupera os dados do funcionário.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        public Entidades.Funcionario ObtemFuncionario(int idFunc)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Funcionario>()
                .Where("IdFunc=?id")
                .Add("?id", idFunc)
                .ProcessLazyResult<Entidades.Funcionario>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Recupera descritores de vários funcionários
        /// </summary>
        /// <param name="idsFunc"></param>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemFuncionario(IEnumerable<int> idsFunc)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Funcionario>()
                .Where(String.Format("IdFunc In ({0})", String.Join(",", idsFunc.ToArray())))
                .ProcessResultDescriptor<Entidades.Funcionario>()
                .ToList();
        }

        /// <summary>
        /// Recupera os descritores dos funcionários que devem aparecer no filtro de funcionário da listagem de sugestão de cliente.
        /// </summary>
        public IList<IEntityDescriptor> ObterFuncionariosSugestao()
        {
            var consulta = SourceContext.Instance.CreateQuery()
                .From<Data.Model.Funcionario>()
                .OrderBy("Nome")
                .Where("Situacao=?situacao")
                .Add("?situacao", Situacao.Ativo);

            consulta.WhereClause
                .And("(IdFunc IN ?subFunc OR IdFunc IN ?funcionariosSugestao)")
                .Add("?subFunc",
                    SourceContext.Instance.CreateQuery()
                    .Select("IdFunc")
                    .From<Data.Model.ConfigFuncaoFunc>()
                    .Where("IdFuncaoMenu=?idFuncaoMenu")
                    .Add("?idFuncaoMenu", Config.ObterIdFuncaoMenu(Config.FuncaoMenuCadastro.CadastrarSugestoesClientes)))
                .Add("?funcionariosSugestao",
                    SourceContext.Instance.CreateQuery()
                    .SelectDistinct("Usucad")
                    .From<Data.Model.SugestaoCliente>());

            return consulta.ProcessResultDescriptor<Entidades.Funcionario>().ToList();
        }

        /// <summary>
        /// Salva os dados do funcionário.
        /// </summary>
        /// <param name="funcionario"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarFuncionario(Entidades.Funcionario funcionario)
        {
            funcionario.Require("funcionario").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = funcionario.Save(session);
                if (!resultado)
                    return resultado;

                resultado = session.Execute(false).ToSaveResult();

                if (resultado)
                    Glass.Data.Helper.Config.ResetModulosUsuario(funcionario.IdFunc);

                return resultado;
            }
        }

        /// <summary>
        /// Apaga os dados do funcionário.
        /// </summary>
        /// <param name="funcionario"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarFuncionario(Entidades.Funcionario funcionario)
        {
            funcionario.Require("funcionario").NotNull();

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = funcionario.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }

        /// <summary>
        /// Valida a atualização dos dados do funcionário.
        /// </summary>
        /// <param name="funcionario"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IProvedorFuncionario.ValidaAtualizacao(Entidades.Funcionario funcionario)
        {
            if (funcionario.ExistsInStorage)
            {
                var original = (Entidades.Funcionario)funcionario.Instance;

                if (funcionario.AdminSync && !UserInfo.GetUserInfo.IsAdminSync)
                {
                    return new IMessageFormattable[]
                    {
                        ("Não é possível efetuar alterações no funcionário").GetFormatter()
                    };
                }

                if ((original.IdTipoFunc == (int)Utils.TipoFuncionario.InstaladorComum ||
                    original.IdTipoFunc == (int)Utils.TipoFuncionario.InstaladorTemperado ||
                    original.IdTipoFunc == (int)Utils.TipoFuncionario.MotoristaInstalador) &&
                    funcionario.IdTipoFunc != (uint)Utils.TipoFuncionario.InstaladorComum &&
                    funcionario.IdTipoFunc != (uint)Utils.TipoFuncionario.InstaladorTemperado &&
                    funcionario.IdTipoFunc != (uint)Utils.TipoFuncionario.MotoristaInstalador &&
                    SourceContext.Instance.CreateQuery()
                    .From<Data.Model.FuncEquipe>()
                    .Where("IdFunc=?id")
                    .Add("?id", funcionario.IdFunc)
                    .ExistsResult())
                {
                    return new IMessageFormattable[]
                    {
                        ("Não é possível alterar o tipo de um funcionário instalador que pertence a uma equipe de instalação. " +
                         "Para alterar seu tipo para um tipo que não seja instalador remova o funcionário da equipe antes.").GetFormatter()
                    };
                }

                if (funcionario.Situacao == Situacao.Inativo && SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Cliente>()
                    .Where("IdFuncAtendente=?id")
                    .Add("?id", funcionario.IdFunc)
                    .ExistsResult())
                {
                    return new IMessageFormattable[]
                    {
                        ("Não é possível Inativar o funcionário pois ele está vinculado como atendente a pelo menos um Cliente.").GetFormatter()
                    };
                }

                if (funcionario.IdTipoFunc == (int)Utils.TipoFuncionario.MarcadorProducao && (funcionario.FuncionarioSetores == null || funcionario.FuncionarioSetores.Count() == 0))
                {
                    return new IMessageFormattable[]
                    {
                        ("Selecione ao menos um setor que este funcionário terá acesso.").GetFormatter()
                    };
                }

                // Caso o funcionário tenha permissão de emitir pedidos/orçamento é necessário verificar se o tipo de funcionário esteja sendo alterado
                // para um tipo de funcionario que dê permissão de emitir pedidos/orçamentos, é necessário verificar também se a situação está sendo alterada.
                if (original.Vendedor && !funcionario.Vendedor)
                {
                    if (SourceContext.Instance.CreateQuery()
                        .From<Data.Model.Pedido>()
                        .Where("IdFunc=?id AND Situacao NOT IN (?situacao1, ?situacao2)")
                        .Add("?id", funcionario.IdFunc)
                        .Add("?situacao1", Data.Model.Pedido.SituacaoPedido.Cancelado)
                        .Add("?situacao2", Data.Model.Pedido.SituacaoPedido.Confirmado)
                        .ExistsResult())
                        return new IMessageFormattable[]
                        {
                            ("Este funcionário está associado à pedidos que não estão liberados ou cancelados, desassocie-o " +
                            "destes pedidos antes de alterar seu tipo de funcionário ou sua situação.").GetFormatter()
                        };

                    if (SourceContext.Instance.CreateQuery()
                        .From<Data.Model.Orcamento>()
                        .Where("IdFunc=?id AND Situacao NOT IN (?situacao1, ?situacao2)")
                        .Add("?id", funcionario.IdFunc)
                        .Add("?situacao1", Data.Model.Orcamento.SituacaoOrcamento.Negociado)
                        .Add("?situacao2", Data.Model.Orcamento.SituacaoOrcamento.NaoNegociado)
                        .ExistsResult())
                        return new IMessageFormattable[]
                        {
                            ("Este funcionário está associado à orçamentos em aberto, desassocie-o " +
                            "destes orçamentos antes de alterar seu tipo de funcionário ou sua situação.").GetFormatter()
                        };
                }

            }

            // Reseta lista de funcionários logados para aplicar alteração
            UserInfo.ZeraListaUsuariosLogados();

            return new IMessageFormattable[0];
        }

        /// <summary>
        /// Valida a existencia do funcionário,.
        /// </summary>
        /// <param name="funcionario"></param>
        /// <returns></returns>
        IMessageFormattable[] Entidades.IProvedorFuncionario.ValidaExistencia(Entidades.Funcionario funcionario)
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
                // Verifica se o funcionário possui orçamentos relacionados à seu id
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Orcamento>()
                    .Where("IdFuncionario=?id")
                    .Add("?id", funcionario.IdFunc)
                    .Count(),
                    tratarResultado("Este funcionário não pode ser excluído por possuir orçamentos relacionados ao mesmo. Para impedir seu login no sistema, inative-o."))
                // Verifica se o funcionário possui pedidos relacionados à seu id
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Pedido>()
                    .Where("IdFunc=?id OR UsuConf=?id")
                    .Add("?id", funcionario.IdFunc)
                    .Count(),
                    tratarResultado("Este funcionário não pode ser excluído por possuir pedidos relacionados ao mesmo. Para impedir seu login no sistema, inative-o."))
                // Verifica se o funcionário possui medições relacionados à seu id
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Medicao>()
                    .Where("IdFunc=?id OR IdFuncConf=?id OR IdFuncMed=?id")
                    .Add("?id", funcionario.IdFunc)
                    .Count(),
                    tratarResultado("Este funcionário não pode ser excluído por possuir medições relacionadas ao mesmo. Para impedir seu login no sistema, inative-o."))
                // Verifica se o funcionário possuir instalações relacionadas à seu id
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.FuncEquipe>()
                    .Where("Idfunc=?id")
                    .Add("?id", funcionario.IdFunc)
                    .Count(),
                    tratarResultado("Este funcionário não pode ser excluído por fazer parte de uma equipe de instalação. Para impedir seu login no sistema, inative-o."))
                // Verifica se o funcionário possuir projetos relacionados à seu id
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Projeto>()
                    .Where("IdFunc=?id")
                    .Add("?id", funcionario.IdFunc)
                    .Count(),
                    tratarResultado("Este funcionário não pode ser excluído por possuir projetos relacionados ao mesmo. Para impedir seu login no sistema, inative-o."))
                // Verifica se o funcionário possuir leituras na produção relacionados à seu id
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.LeituraProducao>()
                    .Where("IdFuncLeitura=?id")
                    .Add("?id", funcionario.IdFunc)
                    .Count(),
                    tratarResultado("Este funcionário não pode ser excluído por possuir leituras na produção relacionados ao mesmo. Para impedir seu login no sistema, inative-o."))
                // Verifica se o funcionário possuir leituras na produção relacionados à seu id
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.MovBanco>()
                    .Where("Usucad=?id")
                    .Add("?id", funcionario.IdFunc)
                    .Count(),
                    tratarResultado("Este funcionário não pode ser excluído por possuir movimentações bancárias relacionadas ao mesmo. Para impedir seu login no sistema, inative-o."))
                .Add(SourceContext.Instance.CreateQuery()
                    .From<Data.Model.Cliente>()
                    .Where("IdFunc=?id")
                    .Add("?id", funcionario.IdFunc)
                    .Count(),
                    tratarResultado("Este funcionário não pode ser excluído por possuir clientes vinculado ao mesmo. Para impedir seu login no sistema, inative-o."))
                .Execute();

            return mensagens.Select(f => f.GetFormatter()).ToArray();
        }

        /// <summary>
        /// Retorna os menus que a empresa tem acesso
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public IList<Menu> ObterMenusEmpresa(int idLoja)
        {
            return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IMenuFluxo>().ObterMenusPorConfig(idLoja);
        }

        #endregion

        #region Tipo de Funcionário

        /// <summary>
        /// Recupera os tipos de funcionarios do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Entidades.TipoFuncionario> PesquisarTiposFuncionario()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.TipoFuncionario>()
                .OrderBy("Descricao")
                .ToVirtualResult<Entidades.TipoFuncionario>();
        }

        /// <summary>
        /// Recupera os tipos de funcionarios cadastrados no sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemTiposFuncionario()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.TipoFuncionario>()
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.TipoFuncionario>()
                .ToList();
        }

        /// <summary>
        /// Recupera os tipos de funcionarios cadastrados no sistema para serem usados no ControleUsuarios.aspx
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemTiposFuncionarioParaControleUsuarios()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.TipoFuncionario>()
                .Where(string.Format("IdTipoFuncionario Not In ({0},{1})", (int)Utils.TipoFuncionario.MarcadorProducao, (int)Utils.TipoFuncionario.SupervisorProducao))
                .OrderBy("Descricao")
                .ProcessResultDescriptor<Entidades.TipoFuncionario>()
                .ToList();
        }

        /// <summary>
        /// Recupera os tipos de funcionario.
        /// </summary>
        /// <param name="incluirSetor">Identifica se é para incluir os setores no resultado.</param>
        /// <param name="removerMarcadorProducaoSemSetor">Identifica se é para remover o marcador de produção sem setor.</param>
        /// <returns></returns>
        public IList<Entidades.TipoFuncSetor> ObtemTiposFuncionarioSetor(bool incluirSetor, bool removerMarcadorProducaoSemSetor)
        {
            var retorno = SourceContext.Instance.CreateQuery()
                .From<Data.Model.TipoFuncionario>()
                .Execute<Data.Model.TipoFuncionario>()
                .Select(f => new Entidades.TipoFuncSetor
                    {
                        IdTipoFunc = f.IdTipoFuncionario,
                        Descricao = f.Descricao
                    }).ToList();

            if (incluirSetor)
            {
                var producao = retorno
                    .Where(t => t.IdTipoFunc == (uint)Data.Helper.Utils.TipoFuncionario.MarcadorProducao).FirstOrDefault();

                if (producao != null)
                {
                    // Remove o marcador de produção
                    if (removerMarcadorProducaoSemSetor)
                        retorno.Remove(producao);

                    var setores = SourceContext.Instance.CreateQuery()
                        .From<Data.Model.Setor>()
                        .Select<Data.Model.Setor>(f => f.IdSetor, f => f.Descricao)
                        .Execute()
                        .Select(f => 
                            new 
                            { 
                                IdSetor = f.GetInt32("IdSetor"), 
                                Descricao = f.GetString("Descricao") 
                            });

                    // Insere vários marcadores de produção
                    foreach (var s in setores)
                    {
                        var novo = new Entidades.TipoFuncSetor();
                        novo.IdTipoFunc = producao.IdTipoFunc;
                        novo.Descricao = producao.Descricao + ": " + s.Descricao;
                        novo.IdSetorProducao = s.IdSetor;

                        retorno.Add(novo);
                    }
                }
            }

            return retorno.OrderBy(t => t.Descricao).ToList();
        }

        /// <summary>
        /// Recupera o tipo de funcionário pelo identificador informado.
        /// </summary>
        /// <param name="idTipoFuncionario"></param>
        /// <returns></returns>
        public Entidades.TipoFuncionario ObtemTipoFuncionario(int idTipoFuncionario)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.TipoFuncionario>()
                .Where("IdTipoFuncionario=?id")
                .Add("?id", idTipoFuncionario)
                .ProcessLazyResult<Entidades.TipoFuncionario>()
                .FirstOrDefault();
        }

        /// <summary>
        /// Salva os dados da entidade.
        /// </summary>
        /// <param name="tipoFuncionario"></param>
        /// <returns></returns>
        public Colosoft.Business.SaveResult SalvarTipoFuncionario(Entidades.TipoFuncionario tipoFuncionario)
        {
            tipoFuncionario.Require("tipoFuncionario").NotNull();
            
            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = tipoFuncionario.Save(session);

                if (!resultado)
                    return resultado;

                return session.Execute(false).ToSaveResult();
            }
        }

        /// <summary>
        /// Apaga o tipo de funcionario.
        /// </summary>
        /// <param name="idTipoFuncionario"></param>
        /// <returns></returns>
        public Colosoft.Business.DeleteResult ApagarTipoFuncionario(int idTipoFuncionario)
        {
            var tipoFunc = ObtemTipoFuncionario(idTipoFuncionario);
            if (tipoFunc == null)
                return new Colosoft.Business.DeleteResult(false, "Tipo de funcionário não encontrado".GetFormatter());

            if (SourceContext.Instance.CreateQuery()
                .From<Data.Model.Funcionario>()
                .Where("IdTipoFunc=?id")
                .Add("?id", idTipoFuncionario)
                .ExistsResult())
                return new Colosoft.Business.DeleteResult(false, "Este tipo de funcionário está associado a algum funcionário".GetFormatter());

            using (var session = SourceContext.Instance.CreateSession())
            {
                var resultado = tipoFunc.Delete(session);
                if (!resultado)
                    return resultado;

                return session.Execute(false).ToDeleteResult();
            }
        }
        
        #endregion

        #region FuncModulo

        /// <summary>
        /// Recupera os descritores do módulos do funcionário.
        /// </summary>
        /// <param name="idFunc">Identificador do funcionário.</param>
        /// <returns></returns>
        public IList<Entidades.FuncModuloPesquisa> ObtemModulosFuncionario(int idFunc)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Model.Modulo>("m")
                .LeftJoin<Data.Model.FuncModulo>("m.IdModulo=fm.IdModulo AND fm.IdFunc=?id", "fm")
                .Select("?id AS IdFunc, m.IdModulo, m.Descricao AS Modulo, m.Grupo AS GrupoModulo, fm.Permitir")
                .OrderBy("GrupoModulo, Modulo")
                .Add("?id", idFunc)
                .Execute<Entidades.FuncModuloPesquisa>()
                .ToList();
        }

        #endregion
       
    }
}
