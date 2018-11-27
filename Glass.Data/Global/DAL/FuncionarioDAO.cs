using GDA;
using Glass.Configuracoes;
using Glass.Data.Helper;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Glass.Data.DAL
{
    public sealed class FuncionarioDAO : BaseCadastroDAO<Funcionario, FuncionarioDAO>
    {
        //private FuncionarioDAO() { }

        #region Autentica usuário

        /// <summary>
        /// Autentica usuário pelo login e senha
        /// </summary>
        /// <param name="login"></param>
        /// <param name="senha"></param>
        /// <returns></returns>
        public LoginUsuario Autenticacao(string login, string senha)
        {
            var sql = "Select IDFUNC From funcionario Where Login=?login And Senha=?senha And situacao=" + (int)Situacao.Ativo;
            object idFunc;

            var param = new GDAParameter[2];
            param.SetValue(new GDAParameter("?login", login), 0);
            param.SetValue(new GDAParameter("?senha", senha), 1);

            try
            {
                idFunc = objPersistence.ExecuteScalar(sql, param);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao autenticar usuário. Erro: " + ex.Message);
            }

            if (idFunc == null)
            {
                throw new Exception("Usuário ou senha inválidos.");
            }

            var tipoFunc = ObtemIdTipoFunc(null, idFunc.ToString().StrParaUint());

            // Verifica se funcionário pode logar no sistema pelo dia e hora atuais
            // Regra aplicada somente para os que não forem gerentes
            if (tipoFunc != (uint)Utils.TipoFuncionario.Administrador)
            {
                // Verifica se o usuário possui a permissão de login no sistema a qualquer momento
                if (Config.PossuiPermissao(idFunc.ToString().StrParaInt(), Config.FuncaoMenuCadastro.EfetuarLoginQualquerMomento))
                {
                    return GetLogin(idFunc.ToString().StrParaInt());
                }

                // Usuários da produção podem logar no sistema a qualquer hora
                if (tipoFunc == (uint)Utils.TipoFuncionario.MarcadorProducao)
                {
                    return GetLogin(idFunc.ToString().StrParaInt());
                }

                DateTime horarioInicioLogin;
                DateTime horarioFimLogin;

                if (!DateTime.TryParse(DateTime.Now.ToString(string.Format("dd/MM/yyyy {0}", Geral.HorarioInicioLogin)), out horarioInicioLogin) ||
                    !DateTime.TryParse(DateTime.Now.ToString(string.Format("dd/MM/yyyy {0}", Geral.HorarioFimLogin)), out horarioFimLogin))
                {
                    throw new InvalidOperationException("Não foi possível recuperar a configuração do horário de inicio e fim de login.");
                }

                if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday) // Domingo
                {
                    throw new Exception("Não é permitido logar no sistema aos domingos.");
                }
                else if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday && (DateTime.Now.Hour < 6 || DateTime.Now.Hour >= 14)) // Sábado
                {
                    throw new Exception("Não é permitido logar no sistema neste horário no sábado.");
                }
                else if (DateTime.Now < horarioInicioLogin || DateTime.Now > horarioFimLogin) // Configuração.
                {
                    var excecao = new Exception($"Não é permitido logar no sistema neste horário.");

                    ErroDAO.Instance.InserirFromException($"Autenticacao - Inicio: { horarioInicioLogin } | Fim: { horarioFimLogin }", excecao);
                    throw excecao;
                }
            }

            return GetLogin(idFunc.ToString().StrParaInt());
        }

        #endregion

        #region Retorna o nome do funcionário

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        public string GetNome(uint idFunc)
        {
            return GetNome(null, idFunc);
        }

        public string GetNome(GDASession sessao, uint idFunc)
        {
            string sql = "Select nome From funcionario Where idFunc=" + idFunc;

            object nome = objPersistence.ExecuteScalar(sessao, sql);

            return nome != null ? nome.ToString().Replace("'", "") : String.Empty;
        }

        #endregion

        #region Retorna classe login do usuário

        /// <summary>
        /// Retorna informações do usuário logado
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public LoginUsuario GetLogin(int idFuncionario)
        {
            return GetLogin(null, idFuncionario);
        }

        /// <summary>
        /// Retorna informações do usuário logado
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idUser"></param>
        /// <returns></returns>
        public LoginUsuario GetLogin(GDASession sessao, int idFuncionario)
        {
            string sql = @"
                Select f.*, tf.Descricao as descrTipoFunc, l.NomeFantasia as NomeLoja
                From funcionario f
                    Left Join tipo_func tf On (f.idTipoFunc=tf.idTipoFunc)
                    Left Join loja l On (f.idLoja=l.idLoja)
                Where f.idFunc=" + idFuncionario;

            try
            {
                Funcionario func = objPersistence.LoadOneData(sessao, sql);

                if (func != null)
                {
                    LoginUsuario login = new LoginUsuario();

                    login.CodUser = (uint)func.IdFunc;
                    login.Nome = func.Nome;
                    login.TipoUsuario = (uint)func.IdTipoFunc;
                    login.IdLoja = (uint)func.IdLoja;
                    login.DescrTipoFunc = func.DescrTipoFunc;
                    login.NomeLoja = func.NomeLoja;

                    uint idCidade = LojaDAO.Instance.ObtemValorCampo<uint>(sessao, "idCidade", "idLoja=" + func.IdLoja);
                    login.UfLoja = CidadeDAO.Instance.ObtemValorCampo<string>(sessao, "nomeUf", "idCidade=" + idCidade);

                    return login;
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Registro não encontrado"))
                    throw ex;

                ErroDAO.Instance.InserirFromException("Falha ao recuperar login.", ex);
            }

            return null;
        }

        #endregion

        #region Retorna todos os funcionários

        private string Sql(uint idFunc, string idLoja, string nomeFunc, int situacao, bool apenasRegistrados, uint idTipoFunc, uint? idSetor,
            string dtNascIni, string dtNascFim, bool selecionar)
        {
            string campos = selecionar ? "f.*, tf.Descricao as descrTipoFunc, l.NomeFantasia as NomeLoja, '$$$' as Criterio" : "Count(*)";

            string criterio = String.Empty;

            string sql = "Select " + campos + @" From funcionario f
                Left Join tipo_func tf On (f.idTipoFunc=tf.idTipoFunc)
                Left Join loja l On (f.idLoja=l.idLoja)
                Where 1";

            if (idFunc > 0)
            {
                sql += " and f.idFunc=" + idFunc;
                criterio += "Funcionário: " + GetNome(idFunc) + "    ";
            }

            if (idLoja != "Todas" && idLoja != "0")
            {
                sql += " And f.idLoja=" + idLoja;
                criterio += "Loja: " + LojaDAO.Instance.GetNome(Glass.Conversoes.StrParaUint(idLoja)) + "    ";
            }

            if (!String.IsNullOrEmpty(nomeFunc))
            {
                sql += " And Nome Like ?nome";
                criterio += "Funcionário: " + nomeFunc + "    ";
            }

            if (situacao > 0)
            {
                sql += " And f.situacao=" + situacao;
                criterio += "Situação: " + (situacao == 1 ? "Ativo" : situacao == 2 ? "Inativo" : String.Empty) + "    ";
            }

            if (apenasRegistrados)
            {
                sql += " And f.registrado=true";
                criterio += "Apenas registrados    ";
            }

            if (idTipoFunc > 0)
            {
                sql += " and f.idTipoFunc=" + idTipoFunc;
                criterio += "Tipo: " + TipoFuncDAO.Instance.GetDescricao(idTipoFunc) + "    ";
            }

            if (idSetor > 0)
            {
                sql += " and concat(f.idFunc, " + idSetor + ") in (select concat(idFunc, idSetor) from funcionario_setor)";
                criterio += "Setor: " + Utils.ObtemSetor(idSetor.Value).Descricao + "    ";
            }

            if (!string.IsNullOrEmpty(dtNascIni))
            {
                sql += " and date(f.DataNasc) >= ?dtNascIni";
                criterio += "Data de Nasc. Inicial: " + dtNascIni + "    ";
            }

            if (!string.IsNullOrEmpty(dtNascFim))
            {
                sql += " and date(f.DataNasc) <= ?dtNascFim";
                criterio += "Data de Nasc. Final: " + dtNascFim + "    ";
            }

            return sql.Replace("$$$", criterio);
        }

        private void CarregaSetoresFunc(ref List<Funcionario> func)
        {
            CarregaSetoresFunc(null, ref func);
        }

        private void CarregaSetoresFunc(GDASession sessao, ref List<Funcionario> func)
        {
            using (var dao = FuncionarioSetorDAO.Instance)
                foreach (var f in func)
                    f.SetoresFunc = dao.GetDescricaoSetores(sessao, (uint)f.IdFunc);
        }

        public IList<Funcionario> GetList(string idLoja, string nomeFunc, int situacao, bool apenasRegistrados,
            uint idTipoFunc, uint? idSetor, string dtNascIni, string dtNascFim, string sortExpression, int startRow, int pageSize)
        {
            string sort = String.IsNullOrEmpty(sortExpression) ? "Nome" : sortExpression;
            var itens = objPersistence.LoadDataWithSortExpression(Sql(0, idLoja, nomeFunc, situacao, apenasRegistrados, idTipoFunc, idSetor,
                dtNascIni, dtNascFim, true), new InfoSortExpression(sort),
                new InfoPaging(startRow, pageSize), GetParam(nomeFunc, dtNascIni, dtNascFim)).ToList();

            CarregaSetoresFunc(ref itens);
            return itens;
        }

        public int GetCount(string idLoja, string nomeFunc, int situacao, bool apenasRegistrados,
            uint idTipoFunc, uint? idSetor, string dtNascIni, string dtNascFim)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, idLoja, nomeFunc, situacao, apenasRegistrados, idTipoFunc,
                idSetor, dtNascIni, dtNascFim, false), GetParam(nomeFunc, dtNascIni, dtNascFim));
        }

        public Funcionario GetElement(uint idFunc)
        {
            return GetElement(null, idFunc);
        }

        public Funcionario GetElement(GDASession sessao, uint idFunc)
        {
            var itens = objPersistence.LoadData(sessao, Sql(idFunc, "0", null, 0, false, 0, null, null, null, true)).ToList();
            CarregaSetoresFunc(sessao, ref itens);
            return itens.Count > 0 ? itens[0] : null;
        }

        public IList<Funcionario> GetForRpt(string idLoja, string nomeFunc, int situacao, bool apenasRegistrados, uint idTipoFunc,
            uint? idSetor, string dtNascIni, string dtNascFim)
        {
            string filtro = " Order By Nome";

            var itens = objPersistence.LoadData(Sql(0, idLoja, nomeFunc, situacao, apenasRegistrados, idTipoFunc, idSetor,
                dtNascIni, dtNascFim, true) + filtro, GetParam(nomeFunc, dtNascIni, dtNascFim)).ToList();

            CarregaSetoresFunc(ref itens);
            return itens;
        }

        public GDAParameter[] GetParam(string nomeFunc, string dtNascIni, string dtNascFim)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dtNascIni))
                lstParam.Add(new GDAParameter("?dtNascIni", DateTime.Parse(dtNascIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dtNascFim))
                lstParam.Add(new GDAParameter("?dtNascFim", DateTime.Parse(dtNascFim + " 23:59:59")));

            if (!String.IsNullOrEmpty(nomeFunc))
                lstParam.Add(new GDAParameter("?nome", "%" + nomeFunc + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Retorna todos os funcionários para filtro

        /// <summary>
        /// Retorna todos os funcionários para filtro
        /// </summary>
        /// <returns></returns>
        public IList<Funcionario> GetOrdered()
        {
            string sql = "Select * From funcionario Where situacao=" + (int)Situacao.Ativo + " Order By nome";

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Retorna todos os funcionários para tela de associação de vendedor com cliente
        /// </summary>
        /// <returns></returns>
        public IList<Funcionario> GetAtivosAssociadosCliente()
        {
            string sql = "Select * From funcionario Where (situacao=" + (int)Situacao.Ativo +
                " And idTipoFunc=" + (int)Utils.TipoFuncionario.Vendedor + ") Or idFunc In (Select idFunc from cliente) Order By nome";

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Retorna funcionários logados no momento

        public LoginUsuario[] GetLogados()
        {
            List<LoginUsuario> lstLogados = UserInfo._usuario;
            List<LoginUsuario> lstLogadosRetorno = new List<LoginUsuario>();

            foreach (LoginUsuario login in lstLogados)
                if (login.UltimaAtividade >= DateTime.Now.AddMinutes(-20))
                    lstLogadosRetorno.Add(login);

            return lstLogadosRetorno.ToArray();
        }

        #endregion

        #region Busca funcionários que finalizaram pedidos

        public IList<Funcionario> GetFuncFin()
        {
            var sql = @"Select f.*, coalesce(l.nomeFantasia, l.razaoSocial) As nomeLoja, tf.descricao As descrTipoFunc
                From funcionario f
					Left Join pedido p ON (p.usuFin=f.idFunc)
                    Left Join loja l ON (f.idLoja=l.idLoja)
                    Left Join tipo_func tf ON (f.idTipoFunc=tf.idTipoFunc)
                Where f.situacao=" + (int)Situacao.Ativo +
                    " And p.usuFin > 0 Group By f.idFunc";

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Busca funcionários que tiram pedidos

        public IList<Funcionario> GetVendedoresDropAssociaCliente()
        {
            var itens = new List<Funcionario>();

            itens.Add(new Funcionario
            {
                IdFunc = int.MaxValue,
                Nome = "Sem Vendedor Associado"
            });

            itens.AddRange(GetVendedores(null));
            return itens;
        }

        public IList<Funcionario> GetVendedores()
        {
            return GetVendedores(null);
        }

        public IList<Funcionario> GetVendedores(string nome)
        {
            string sql = @"Select f.*, coalesce(l.nomeFantasia, l.razaoSocial) as nomeLoja, tf.descricao as descrTipoFunc
                From funcionario f
                    Left Join loja l On (f.idLoja=l.idLoja)
                    Left Join tipo_func tf On (f.idTipoFunc=tf.idTipoFunc)
                Where f.situacao=" + (int)Situacao.Ativo;

            if (!String.IsNullOrEmpty(nome))
                sql += " And f.Nome like ?nome";

            sql += " and (f.idFunc in (select idFunc from config_funcao_func where idFuncaoMenu=" +
                Config.ObterIdFuncaoMenu(Config.FuncaoMenuPedido.EmitirPedido) + ")" +
                " Or f.idFunc=" + UserInfo.GetUserInfo.CodUser +
                " OR f.IdTipoFunc= " + (int)Utils.TipoFuncionario.Vendedor + ") Order By f.Nome";

            return objPersistence.LoadData(sql, new GDAParameter("?nome", "%" + nome + "%")).ToList();
        }


        //Busca os funcionários que tiram pedidos e também os que são Aux. Escritório e Medição
        public IList<Funcionario> GetVendedoresEMedicao()
        {
            string sql = @"Select f.*, coalesce(l.nomeFantasia, l.razaoSocial) as nomeLoja, tf.descricao as descrTipoFunc
                From funcionario f
                    Left Join loja l On (f.idLoja=l.idLoja)
                    Left Join tipo_func tf On (f.idTipoFunc=tf.idTipoFunc)
                Where f.situacao=" + (int)Situacao.Ativo;

            sql += " and (f.idFunc in (select idFunc from config_funcao_func where idFuncaoMenu In (" +
                Config.ObterIdFuncaoMenu(Config.FuncaoMenuPedido.EmitirPedido) + "," + Config.ObterIdFuncaoMenu(Config.FuncaoMenuMedicao.EfetuarMedicao) + "))" +
                " Or f.idFunc=" + UserInfo.GetUserInfo.CodUser + ") Order By f.Nome";

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        public IList<Funcionario> GetMotoristas(string nome)
        {
            string sql = @"Select f.*, coalesce(l.nomeFantasia, l.razaoSocial) as nomeLoja, tf.descricao as descrTipoFunc
                From funcionario f
                    Left Join loja l On (f.idLoja=l.idLoja)
                    Left Join tipo_func tf On (f.idTipoFunc=tf.idTipoFunc)
                Where f.situacao=" + (int)Situacao.Ativo;

            if (!String.IsNullOrEmpty(nome))
                sql += " And f.Nome like ?nome";

            sql += " and (f.IDTIPOFUNC=" + (uint)Utils.TipoFuncionario.MotoristaInstalador +
                " Or f.IDTIPOFUNC=" + (int)Utils.TipoFuncionario.MotoristaMedidor + " Or tf.descricao like 'motorista%') Order By f.Nome";

            return objPersistence.LoadData(sql, new GDAParameter("?nome", "%" + nome + "%")).ToList();
        }

        public int GetMenorAnoCadastro()
        {
            var menorAno = ExecuteScalar<int>("Select Year(Min(DataCad)) From pedido Where DataCad is not null and Year(datacad)>2001");

            if (menorAno > 0)
                return menorAno;
            else
                return DateTime.Now.AddYears(-2).Year;
        }

        #region Busca funcionários para gerar/consultar comissão

        public Funcionario[] GetVendedoresComissao()
        {
            return GetVendedoresComissao(false);
        }

        public Funcionario[] GetVendedoresComissao(bool incluirInstaladores)
        {
            var todos = new Funcionario();
            todos.IdFunc = 0;
            todos.Nome = "Todos";

            var sql = string.Format("SELECT * FROM funcionario WHERE Situacao={0} AND (IdFunc IN (SELECT IdFunc FROM config_funcao_func WHERE IdFuncaoMenu={1}){2}) ORDER BY Nome",
                (int)Situacao.Ativo, Config.ObterIdFuncaoMenu(Config.FuncaoMenuPedido.EmitirPedido), incluirInstaladores && Geral.ControleInstalacao ?
                    string.Format(" OR IdFunc IN (SELECT IdFunc FROM config_funcao_func WHERE IdFuncaoMenu IN ({0},{1})) OR IdTipoFunc IN ({2},{3},{4})",
                        Config.ObterIdFuncaoMenu(Config.FuncaoMenuInstalacao.ControleInstalacaoComum), Config.ObterIdFuncaoMenu(Config.FuncaoMenuInstalacao.ControleInstalacaoTemperado),
                        (int)Utils.TipoFuncionario.InstaladorComum, (int)Utils.TipoFuncionario.InstaladorTemperado, (int)Utils.TipoFuncionario.MotoristaInstalador) : string.Empty);

            var retorno = objPersistence.LoadData(sql).ToList();
            retorno.Insert(0, todos);

            return retorno.ToArray();
        }

        /// <summary>
        /// Busca funcionarios associados aos clientes.
        /// </summary>
        /// <returns>
        /// Retorna um array de funcionários associados aos clientes.
        /// </returns>
        public Funcionario[] ObterVendedoresAssociadosCliente()
        {
            string sql = $@"
               SELECT *
               FROM funcionario f
                   INNER JOIN cliente c ON (f.IdFunc = c.IdFunc)
               WHERE f.situacao = {(int)Situacao.Ativo}
               GROUP BY f.IdFunc
               ORDER BY f.Nome";

            return objPersistence.LoadData(sql).ToArray();
        }

        #endregion

        #region Busca funcionários que tiram pedidos por loja (para filtro, com opção "TODOS")

        public Funcionario[] GetVendedoresByLoja(uint idLoja)
        {
            string sql = "Select * From funcionario Where (idFunc in (select idFunc from config_funcao_func where idFuncaoMenu=" + Config.ObterIdFuncaoMenu(Config.FuncaoMenuPedido.EmitirPedido) + ")" +
                " OR IdFunc IN (SELECT IdFunc FROM config_funcao_func WHERE IdFuncaoMenu=" + Config.ObterIdFuncaoMenu(Config.FuncaoMenuOrcamento.EmitirOrcamento) + ")" +
                " OR IdFunc IN (SELECT DISTINCT IdFunc FROM pedido) OR IdFunc IN (SELECT DISTINCT IdFunc FROM orcamento)) And situacao=" + (int)Situacao.Ativo;

            if (idLoja > 0) // Se não for Todas
                sql += " And idLoja=" + idLoja;

            sql += " Order By Nome";

            List<Funcionario> lst = objPersistence.LoadData(sql).ToList();

            Funcionario func = new Funcionario();
            func.IdFunc = 0;
            func.Nome = "TODOS";
            lst.Insert(0, func);

            return lst.ToArray();
        }

        #endregion

        #region Busca funcionários que tiram pedidos por loja

        public IList<Funcionario> GetVendedoresAuxGer(uint idLoja)
        {
            string sql = "Select * From funcionario Where (idFunc in (select idFunc from config_funcao_func where idFuncaoMenu=" + Config.ObterIdFuncaoMenu(Config.FuncaoMenuPedido.EmitirPedido) + ")" +
                ") And situacao=" + (int)Situacao.Ativo;

            if (idLoja > 0) // Se não for Todas
                sql += " And idLoja=" + idLoja;

            sql += " Order By Nome";

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Busca funcionários que tiram pedidos por loja e que tenham vendido algo no período passado

        /// <summary>
        /// Busca funcionários que tiram pedidos por loja e que tenham vendido mais de R$1000,00 no período passado
        /// </summary>
        public Funcionario[] GetVendedoresComVendas(uint idLoja, string dataIni, string dataFim)
        {
            return GetVendedoresComVendas(idLoja, false, dataIni, dataFim);
        }

        /// <summary>
        /// Busca funcionários que tiram pedidos por loja e que tenham vendido mais de R$1000,00 no período passado
        /// </summary>
        public Funcionario[] GetVendedoresComVendas(uint idLoja, bool funcCliente, string dataIni, string dataFim)
        {
            return GetVendedoresComVendas(idLoja, funcCliente, dataIni, dataFim, 15, 500, true);
        }

        /// <summary>
        /// Busca funcionários que tiram pedidos por loja e que tenham vendido mais de R$1000,00 no período passado
        /// </summary>
        public Funcionario[] GetVendedoresComVendas(uint idLoja, bool funcCliente, string dataIni, string dataFim, int quantidadeRegistros,
            decimal valorVendidoMinimoNoPeriodo, bool buscarSomenteFuncionarioAtivo)
        {
            string join = funcCliente ? "inner join cliente c on (f.idFunc=c.idFunc) inner join pedido p on (c.id_Cli=p.idCli)" :
                "inner join pedido p on (f.idfunc=p.idfunc)";

            var sql =
                string.Format("SELECT f.* FROM funcionario f {0} WHERE 1 {1}", join,
                    buscarSomenteFuncionarioAtivo ? string.Format("AND f.Situacao={0}", (int)Situacao.Ativo) : string.Empty);

            if (idLoja > 0) // Se não for Todas
                sql += " And f.idLoja=" + idLoja;

            sql += " And p.situacao=" + (int)Pedido.SituacaoPedido.Confirmado + " And p.TipoVenda<>" +
                (int)Pedido.TipoVendaPedido.Garantia + " And p.TipoVenda<>" + (int)Pedido.TipoVendaPedido.Reposição;

            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
            {
                sql += " And p.dataConf>=?dataIni";
                lstParam.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));
            }

            if (!String.IsNullOrEmpty(dataFim))
            {
                sql += " And p.dataConf<=?dataFim";
                lstParam.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59")));
            }

            sql += string.Format(" GROUP BY f.IdFunc HAVING SUM(p.Total) > {0} ORDER BY SUM(p.Total) DESC, Nome LIMIT 0,{1}",
                valorVendidoMinimoNoPeriodo, quantidadeRegistros);

            List<Funcionario> lst = objPersistence.LoadData(sql, lstParam.Count > 0 ? lstParam.ToArray() : null).ToList();

            Funcionario func = new Funcionario();
            func.IdFunc = 0;
            func.Nome = "TODOS";
            lst.Insert(0, func);

            return lst.ToArray();
        }

        #endregion

        #region Busca funcionários que fazem Orçamentos

        public IList<Funcionario> GetVendedoresOrca(uint idOrcamento)
        {
            string sql = "Select * From funcionario Where (idFunc in (select idFunc from config_funcao_func where idFuncaoMenu=" + Config.ObterIdFuncaoMenu(Config.FuncaoMenuOrcamento.EmitirOrcamento) + ")" +
                ") And situacao=" + (int)Situacao.Ativo;

            if (idOrcamento > 0)
                sql += " Or idFunc In (Select IdFunc From orcamento Where idOrcamento=" + idOrcamento + ")";

            sql += " Order By Nome";

            return objPersistence.LoadData(sql).ToList();
        }

        public IList<Funcionario> GetVendedoresOrcamento(GDASession sessao, int? idVendedor)
        {
            string sql = "Select * From funcionario Where (idFunc in (select idFunc from config_funcao_func where idFuncaoMenu=" + Config.ObterIdFuncaoMenu(Config.FuncaoMenuOrcamento.EmitirOrcamento) + ")" +
                ") And situacao=" + (int)Situacao.Ativo;

            if (idVendedor > 0)
                sql += " Or idFunc=" + idVendedor;

            sql += " Order By Nome";

            return objPersistence.LoadData(sessao, sql).ToList();
        }

        #endregion

        #region Busca funcionários que fazem Liberações

        /// <summary>
        /// Busca funcionários que já fizeram alguma liberação
        /// </summary>
        /// <returns></returns>
        public IList<Funcionario> GetFuncLiberacao()
        {
            string sql = "Select * From funcionario Where situacao=" + (int)Situacao.Ativo +
                " and idFunc In (Select distinct idFunc From liberarpedido) Order By Nome";

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Busca Medidores/Motoristas Medidores

        public IList<Funcionario> GetMedidores(string sortExpression, int startRow, int pageSize)
        {
            string sql = "Select * From funcionario Where situacao=" + (int)Situacao.Ativo + " And (idtipofunc=" + (int)Utils.TipoFuncionario.MotoristaMedidor +
                " Or idFunc In (select idFunc from config_funcao_func where idFuncaoMenu=" + Config.ObterIdFuncaoMenu(Config.FuncaoMenuMedicao.Medidor) + "))" +
                " Order By Nome asc";

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, null);
        }

        public IList<Funcionario> GetMedidores()
        {
            return GetMedidores(null, 0, int.MaxValue);
        }

        public int GetMedidoresCount()
        {
            string sql = "Select Count(*) From funcionario Where situacao=" + (int)Situacao.Ativo + " And (idtipofunc=" + (int)Utils.TipoFuncionario.MotoristaMedidor +
                " Or idFunc In (select idFunc from config_funcao_func where idFuncaoMenu=" + Config.ObterIdFuncaoMenu(Config.FuncaoMenuMedicao.Medidor) + "))";

            return objPersistence.ExecuteSqlQueryCount(sql, null);
        }

        #endregion

        #region Busca Financeiros

        /// <summary>
        /// Busca funcionarios financeiro e financeiro pagto.
        /// Se funcionário logado for financeiro, só busca financeiro,
        /// Se funcionário logado for financeiro pagto, só busc financeiro pagto
        /// </summary>
        /// <returns></returns>
        public IList<Funcionario> GetFinanceiros()
        {
            // Nesta função estava verificando qual é o tipo de funcionário do usuário logado para retornar os que são financeiro,
            // mas não pode ser assim

            string sql = "Select * From funcionario Where situacao=" + (int)Situacao.Ativo;

            sql += " And (idFunc In (Select idFunc From config_funcao_func Where IdFuncaoMenu In (" +
                Config.ObterIdFuncaoMenu(Config.FuncaoMenuFinanceiro.ControleFinanceiroRecebimento) + "," + Config.ObterIdFuncaoMenu(Config.FuncaoMenuFinanceiroPagto.ControleFinanceiroPagamento) + ")))";

            return objPersistence.LoadData(sql + " Order By Nome").ToList();
        }

        #endregion

        #region Busca Caixa Diario

        public Funcionario[] GetCaixaDiario()
        {
            var tempFinanc = GetFinanceiros().ToArray();

            string sql = @"
                Select * From funcionario
                Where situacao=" + (int)Situacao.Ativo + @"
                    And (idFunc In (Select IdFunc from config_funcao_func Where IdFuncaoMenu=" + Config.ObterIdFuncaoMenu(Config.FuncaoMenuCaixaDiario.ControleCaixaDiario) + "))";

            if (tempFinanc.Select(f => f.IdFunc).Count() > 0)
                sql += "And idFunc Not In (" + String.Join(",", tempFinanc.Select(f => f.IdFunc)) + ")";

            var tempDiario = objPersistence.LoadData(sql).ToList();

            var tempLista = new List<Funcionario>();
            tempLista.AddRange(tempFinanc);
            tempLista.AddRange(tempDiario);

            return tempLista.OrderBy(f => f.Nome).ToArray();
        }

        #endregion

        #region Busca Conferentes

        public IList<Funcionario> GetConferentesPCP()
        {
            var sql = @"
                SELECT f.*
                FROM funcionario f
                    INNER JOIN config_funcao_func cff ON (f.IdFunc = cff.IdFunc)
                    INNER JOIN funcao_menu fm ON (cff.IdFuncaoMenu = fm.IdFuncaoMenu)
                    INNER JOIN config_menu_func cmf ON (cff.IdFunc = cmf.IdFunc AND fm.IdMenu = cmf.IdMenu)
                WHERE Situacao = {0}
                    AND fm.IdFuncaoMenu IN ({1})
                GROUP BY f.IdFunc";

            var idFuncaoMenu = Config.ObterIdFuncaoMenu(Config.FuncaoMenuPCP.GerarConferenciaPedido) + ", " + Config.ObterIdFuncaoMenu(Config.FuncaoMenuPCP.ImprimirEtiquetasMaoDeObra);

            sql = string.Format(sql, (int)Situacao.Ativo, idFuncaoMenu);

            return objPersistence.LoadData(sql).ToList();
        }

        public IList<Funcionario> GetConferentes(string sortExpression, int startRow, int pageSize)
        {
            string sql = "Select * From funcionario Where situacao=" + (int)Situacao.Ativo +
                " And (idFunc In (select idFunc from config_funcao_func where idFuncaoMenu=" + Config.ObterIdFuncaoMenu(Config.FuncaoMenuConferencia.Conferente) + "))";

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, null);
        }

        public int GetConferentesCount()
        {
            string sql = "Select Count(*) From funcionario Where situacao=" + (int)Situacao.Ativo +
                " And (idFunc In (select idFunc from config_funcao_func where idFuncaoMenu=" + Config.ObterIdFuncaoMenu(Config.FuncaoMenuConferencia.Conferente) + "))";

            return objPersistence.ExecuteSqlQueryCount(sql, null);
        }

        #endregion
        #region Busca Colocadores

        private string SqlColocador(bool selecionar)
        {
            var campos = selecionar ? "f.*, t.Descricao as DescrTipoFunc" : "Count(*)";

            var sql = @"
                Select " + campos + @"
                From funcionario f
                    Left Join tipo_func t On (f.IdTipoFunc=t.IdTipoFunc)
                Where f.situacao=" + (int)Situacao.Ativo + @"
                    And (
                        f.idFunc In (select idFunc from config_funcao_func where idFuncaoMenu In (" + Config.ObterIdFuncaoMenu(Config.FuncaoMenuInstalacao.ControleInstalacaoComum) + "," + Config.ObterIdFuncaoMenu(Config.FuncaoMenuInstalacao.ControleInstalacaoTemperado) + @"))
                        Or f.idtipofunc=" + (int)Utils.TipoFuncionario.MotoristaInstalador + @"
                        Or f.idtipofunc=" + (int)Utils.TipoFuncionario.InstaladorComum + @"
                        Or f.idTipoFunc=" + (int)Utils.TipoFuncionario.InstaladorTemperado +
                    ")";

            return sql;
        }

        public IList<Funcionario> GetColocadores()
        {
            return objPersistence.LoadData(SqlColocador(true)).ToList();
        }

        public IList<Funcionario> GetColocadores(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(SqlColocador(true), sortExpression, startRow, pageSize);
        }

        public int GetColocadoresCount()
        {
            return objPersistence.ExecuteSqlQueryCount(SqlColocador(false), null);
        }

        #endregion

        #region Busca para Comissão

        private IList<Funcionario> GetForComissao(Pedido.TipoComissao tipo, string dataIni, string dataFim)
        {
            if (tipo == Pedido.TipoComissao.Instalador)
                return GetColocadores();

            string sql, ids = PedidoDAO.Instance.GetPedidosIdForComissao(tipo, 0, dataIni, dataFim);

            if (Configuracoes.ComissaoConfig.UsarPercComissaoCliente)
            {
                sql = @"select p.idFunc from pedido p
                    inner join produtos_liberar_pedido plp on (p.idPedido=plp.idPedido)
                    inner join liberarpedido lp on (plp.idLiberarPedido=lp.idLiberarPedido)
                    inner join cliente c on (p.idCli=c.id_Cli)
                    where c.percComissaoFunc > 0";

                if (!string.IsNullOrEmpty(dataIni))
                    sql += !PedidoConfig.LiberarPedido ? " and p.DataConf>=?dataIni" : " and lp.DataLiberacao>=?dataIni";

                if (!string.IsNullOrEmpty(dataFim))
                    sql += !PedidoConfig.LiberarPedido ? " and p.DataConf<=?dataFim" : " and lp.DataLiberacao<=?dataFim";

                ids += ("," + GetValoresCampo(sql, "idFunc", PedidoDAO.Instance.GetParamComissao(dataIni, dataFim))).Trim(',');
            }

            if (PedidoConfig.Comissao.PerComissaoPedido && tipo == Pedido.TipoComissao.Funcionario)
            {
                sql = @"select p.idFunc from pedido p
                    inner join produtos_liberar_pedido plp on (p.idPedido=plp.idPedido)
                    inner join liberarpedido lp on (plp.idLiberarPedido=lp.idLiberarPedido)
                    where p.percentualComissao > 0";

                if (!string.IsNullOrEmpty(dataIni))
                    sql += !PedidoConfig.LiberarPedido ? " and p.DataConf>=?dataIni" : " and lp.DataLiberacao>=?dataIni";

                if (!string.IsNullOrEmpty(dataFim))
                    sql += !PedidoConfig.LiberarPedido ? " and p.DataConf<=?dataFim" : " and lp.DataLiberacao<=?dataFim";

                ids += ("," + GetValoresCampo(sql, "idFunc", PedidoDAO.Instance.GetParamComissao(dataIni, dataFim))).TrimEnd(',');
            }

            if (ids.Length == 0)
                return new Funcionario[0];

            sql = "select * from funcionario where situacao=" + (int)Situacao.Ativo + " and idFunc in (" + ids.Trim(',') + ") order by Nome";
            return objPersistence.LoadData(sql).ToList();
        }

        private IList<Funcionario> GetByComissao(Pedido.TipoComissao tipo)
        {
            var campo = tipo == Pedido.TipoComissao.Funcionario ? "idFunc" : "idInstalador";
            var sql = "select * from funcionario where situacao=" + (int)Situacao.Ativo + " and idFunc in (select distinct " +
                campo + " from comissao where " + campo + " is not null) order by Nome";

            return objPersistence.LoadData(sql).ToList();
        }

        public IList<Funcionario> GetVendedoresForComissao(string dataIni, string dataFim)
        {
            return GetForComissao(Pedido.TipoComissao.Funcionario, dataIni, dataFim);
        }

        public IList<Funcionario> GetColocadoresForComissao(string dataIni, string dataFim)
        {
            return GetForComissao(Pedido.TipoComissao.Instalador, dataIni, dataFim);
        }

        public IList<Funcionario> GetGerentesForComissao()
        {
            var sql = "SELECT * FROM funcionario WHERE IdFunc IN (SELECT IdFuncionario FROM comissao_config_gerente)";
            return objPersistence.LoadData(sql).ToList();
        }

        public IList<Funcionario> GetVendedoresByComissao()
        {
            return GetByComissao(Pedido.TipoComissao.Funcionario);
        }

        public IList<Funcionario> GetColocadoresByComissao()
        {
            return GetByComissao(Pedido.TipoComissao.Instalador);
        }

        public IList<Funcionario> GetVendedorForComissaoContasReceber()
        {
            var sql = @"
                    SELECT f.*
                    FROM funcionario f
                        INNER JOIN cliente c ON (c.IdFunc = f.IdFunc)
                        INNER JOIN contas_receber cr ON (c.Id_Cli = cr.IdCliente)
                    WHERE f.Situacao = " + (int)Situacao.Ativo + @"
                        AND cr.Recebida = true
                    GROUP BY f.IdFunc
                    ORDER BY f.Nome
                ";

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Busca funcionários da produção

        /// <summary>
        /// Busca todos os funcionários de produção.
        /// </summary>
        /// <returns></returns>
        public IList<Funcionario> GetProducao()
        {
            return GetProducao(false);
        }

        /// <summary>
        /// Busca todos os funcionários de produção.
        /// </summary>
        /// <returns></returns>
        public IList<Funcionario> GetProducao(bool apenasFuncPerda)
        {
            string filtro = "select distinct idFunc from " + (!apenasFuncPerda ? "funcionario_setor" :
                "config_funcao_func where IdFuncaoMenu=" + Config.ObterIdFuncaoMenu(Config.FuncaoMenuPedido.ReposicaoDePeca));

            string sql = "select * from funcionario where idFunc in (" + filtro + ") order by nome";
            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Busca todos os funcionário da produção ativos
        /// </summary>
        /// <returns></returns>
        public IList<Funcionario> GetProducaoAtivos()
        {
            var sql = "select * from funcionario where idFunc in (select distinct idFunc from funcionario_setor) and situacao=" +
                (int)Situacao.Ativo + " order by nome";

            return objPersistence.LoadData(sql).ToList();
        }

        public Funcionario[] GetProducaoFiltro(uint idSetor)
        {
            List<Funcionario> lstFunc = new List<Funcionario>(GetProducao(idSetor));

            Funcionario func = new Funcionario();
            func.IdFunc = 0;
            func.Nome = "Todos";
            lstFunc.Insert(0, func);

            return lstFunc.ToArray();
        }

        /// <summary>
        /// Busca funcionários da produção.
        /// </summary>
        /// <returns></returns>
        public IList<Funcionario> GetProducao(uint idSetor)
        {
            if (idSetor == 0)
                return new List<Funcionario>();

            var idsFuncImprEtiq = string.Empty;
            var idsFuncLeitura = string.Empty;

            if (idSetor == 1)
                idsFuncImprEtiq = string.Join(",", ExecuteMultipleScalar<string>("SELECT DISTINCT IdFunc FROM impressao_etiqueta"));
            else
            {
                idsFuncLeitura = string.Join(",", LeituraProducaoDAO.Instance.ObterIdsFuncLeituraSetor(null, (int)idSetor));
            }

            var sql = string.Format("SELECT IdFunc, Nome FROM funcionario WHERE IdFunc IN (SELECT IdFunc FROM funcionario_setor WHERE IdSetor={0}) {1} {2} ORDER BY Nome", idSetor, "{0}", "{1}");

            sql = string.Format(sql, !string.IsNullOrEmpty(idsFuncImprEtiq) ? string.Format(" OR IdFunc IN ({0})", idsFuncImprEtiq) : string.Empty,
                /* Chamado 52471. */
                !string.IsNullOrEmpty(idsFuncLeitura) ? string.Format(" OR IdFunc IN ({0})", idsFuncLeitura) : string.Empty);

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Verifica se o usuário atual é o administrador da Sync

        internal string SqlAdminSync(string idFunc, bool selecionar)
        {
            string sql = "select " + (selecionar ? "idFunc" : "count(*)") + " from funcionario where adminSync=true and " +
                "idTipoFunc=" + (int)Utils.TipoFuncionario.Administrador;

            if (!String.IsNullOrEmpty(idFunc))
                sql += " and idFunc=" + idFunc;

            return sql;
        }

        /// <summary>
        /// Verifica se o usuário atual é o administrador da Sync.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <returns></returns>
        public bool IsAdminSync(uint idFunc)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlAdminSync(idFunc.ToString(), false)) > 0;
        }

        #endregion

        #region Verifica se o usuário é um vendedor

        /// <summary>
        /// Verifica se o usuário é vendedor.
        /// </summary>
        /// <param name="idFunc"></param>
        public bool IsVendedor(uint idFunc, uint tipoFunc)
        {
            return Config.PossuiPermissao((int)idFunc, Config.FuncaoMenuPedido.EmitirPedido);
        }

        #endregion

        #region Vale de funcionário

        /// <summary>
        /// Retorna os funcionários com débito (usado na tela de quitar débito de funcionário).
        /// </summary>
        /// <returns></returns>
        public IList<Funcionario> GetFuncionariosDebito()
        {
            string sql = Sql(0, "Todas", null, 0, false, 0, null, null, null, true);
            sql += " and idFunc in (select distinct idFunc from mov_func where saldo<0 group by idFunc order by idMovFunc desc)";

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        #region Obtém data de atraso ao tirar pedido do funcionário

        /// <summary>
        /// Atualiza o número de dias de atraso para um funcionário.
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="numDiasAtraso"></param>
        public void AtrasarFunc(uint idFunc, int numDiasAtraso)
        {
            if (numDiasAtraso == 0 && !PossuiDiasAtraso(idFunc))
                return;

            Funcionario f = GetElementByPrimaryKey(idFunc);
            f.NumDiasAtrasarPedido = numDiasAtraso;

            LogAlteracaoDAO.Instance.LogFuncionario(f, LogAlteracaoDAO.SequenciaObjeto.Novo);
            base.Update(f);
        }

        /// <summary>
        /// Verifica se o cliente possui dias de atraso de pedido
        /// </summary>
        public bool PossuiDiasAtraso(uint idFunc)
        {
            return PossuiDiasAtraso(null, idFunc);
        }

        /// <summary>
        /// Verifica se o cliente possui dias de atraso de pedido
        /// </summary>
        public bool PossuiDiasAtraso(GDASession session, uint idFunc)
        {
            string sql = "Select Count(*) From funcionario Where numDiasAtrasarPedido>0 And idFunc=" + idFunc;

            return objPersistence.ExecuteSqlQueryCount(session, sql) > 0;
        }

        /// <summary>
        /// Obtém data de atraso ao tirar pedido do funcionário
        /// </summary>
        public DateTime ObtemDataAtraso(uint idFunc)
        {
            return ObtemDataAtraso(null, idFunc);
        }

        /// <summary>
        /// Obtém data de atraso ao tirar pedido do funcionário
        /// </summary>
        public DateTime ObtemDataAtraso(GDASession session, uint idFunc)
        {
            string sql = "Select Coalesce(numDiasAtrasarPedido, 0) From funcionario Where idFunc=" + idFunc;
            int diasAtraso = ExecuteScalar<int>(session, sql);

            int num = 0;
            DateTime retorno = DateTime.Now;

            while (num++ < diasAtraso)
            {
                retorno = retorno.AddDays(-1);
                while (!FuncoesData.DiaUtil(retorno))
                    retorno = retorno.AddDays(-1);
            }

            return retorno;
        }

        #endregion

        #region Obtem dados do funcionário

        public uint ObtemIdLoja(uint idFunc)
        {
            return ObtemIdLoja(null, idFunc);
        }

        public uint ObtemIdLoja(GDASession session, uint idFunc)
        {
            return ObtemValorCampo<uint>(session, "idLoja", "idFunc=" + idFunc);
        }

        public string ObtemTipoPedido(uint idFunc)
        {
            return ObtemValorCampo<string>("tipoPedido", "idFunc=" + idFunc);
        }

        public Situacao ObtemSituacao(uint idFunc)
        {
            return (Situacao)ObtemValorCampo<int>("situacao", "idFunc=" + idFunc);
        }

        public uint ObtemIdTipoFunc(GDASession sessao, uint idFunc)
        {
            return ObtemValorCampo<uint>(sessao, "idTipoFunc", "idFunc=" + idFunc);
        }

        public uint ObtemIdTipoFunc(uint idFunc)
        {
            return ObtemIdTipoFunc(null, idFunc);
        }

        public string ObtemTelCel(uint idFunc)
        {
            return ObtemValorCampo<string>("TelCel", "idFunc=" + idFunc);
        }

        public int ObtemNumeroPdv(uint idFunc)
        {
            return ObtemValorCampo<int>("NumeroPdv", "idFunc=" + idFunc);
        }

        public bool ObtemHabilitarChat(uint idFunc)
        {
            return ObtemHabilitarChat(null, idFunc);
        }

        public bool ObtemHabilitarChat(GDASession sessao, uint idFunc)
        {
            return ObtemValorCampo<bool>(sessao, "HabilitarChat", "idFunc=" + idFunc);
        }

        public bool ObtemHabilitarControleUsuarios(uint idFunc)
        {
            return ObtemHabilitarControleUsuarios(null, idFunc);
        }

        public bool ObtemHabilitarControleUsuarios(GDASession sessao, uint idFunc)
        {
            return ObtemValorCampo<bool>(sessao, "HabilitarControleUsuarios", "idFunc=" + idFunc);
        }

        #endregion

        #region Busca funcionários usando filtros

        public string GetIds(string nome)
        {
            string sql = @"
                SELECT idFunc
                FROM funcionario f
                WHERE f.nome LIKE ?nome";


            var ids = ExecuteMultipleScalar<string>(sql, GetParam(nome, null, null));

            if (ids.Count == 0)
                return "0";

            return string.Join(",", ids.Select(f => f).ToArray());
        }

        public List<uint> GetListIds(string nome)
        {
            string sql = @"
                SELECT idFunc
                FROM funcionario f
                WHERE f.nome LIKE ?nome";


            return ExecuteMultipleScalar<uint>(sql, GetParam(nome, null, null));
        }

        #endregion

        #region Busca administradores

        /// <summary>
        /// Retorna o id do AdminSync
        /// </summary>
        /// <returns></returns>
        public int ObterIdAdminSync()
        {
            var lstAdmin = objPersistence.LoadData(string.Format(
                "select * from funcionario where idTipoFunc={0} and situacao={1} and adminSync=1",
                (uint)Utils.TipoFuncionario.Administrador,
                (int)Situacao.Ativo)).ToList();

            if (lstAdmin.Count() > 0)
                return lstAdmin[0].IdFunc;

            return 0;
        }

        /// <summary>
        /// Busca administradores do sistema.
        /// </summary>
        /// <returns></returns>
        public GenericModel[] GetAdministradoresConfig()
        {
            var admin = GetAdministradores(false);
            return admin.Select(x => new GenericModel(x.IdFunc, x.Nome)).OrderBy(x => x.Descr).ToArray();
        }

        /// <summary>
        /// Busca administradores do sistema.
        /// </summary>
        /// <returns></returns>
        public IList<Funcionario> GetAdministradores(bool incluirAdminSync)
        {
            string sql = "select * from funcionario where idTipoFunc=" + (uint)Utils.TipoFuncionario.Administrador +
                " and situacao=" + (int)Situacao.Ativo;

            if (!incluirAdminSync)
                sql += " and coalesce(adminSync,false)=false";

            return objPersistence.LoadData(sql).ToList();
        }

        #endregion

        /// <summary>
        /// Altera a senha do funcionário
        /// </summary>
        /// <param name="idFunc"></param>
        /// <param name="senha"></param>
        public void AlteraSenha(uint idFunc, string senha)
        {
            objPersistence.ExecuteCommand("Update funcionario Set senha=?senha Where idFunc=" + idFunc, new GDAParameter("?senha", senha));
        }

        /// <summary>
        /// Busca funcionários através de vários ids.
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public IList<Funcionario> GetByString(string ids)
        {
            string sql = "select * from funcionario where idFunc in (" + ids.Trim(',') + ")";
            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Recupera o e-mail do funcionario.
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        public string GetEmail(uint idFunc)
        {
            string sql = "select email from funcionario where idfunc=" + idFunc;
            object retorno = objPersistence.ExecuteScalar(sql);
            return retorno != null && retorno != DBNull.Value ? retorno.ToString() : null;
        }

        #region Métodos Sobrescritos

        public override uint Insert(Funcionario objInsert)
        {
            /*objInsert.TipoPedido = "1,2";

            if (!String.IsNullOrEmpty(objInsert.EstCivil))
            {
                // Faz com que o estado civil seja salvo sempre com a primeira letra maiúscula e todas as outras minúsculas.
                var estadoCivil = objInsert.EstCivil.Substring(1, objInsert.EstCivil.Length - 1);
                objInsert.EstCivil = objInsert.EstCivil.Substring(0, 1).ToUpper() + estadoCivil.ToLower();
            }

            objInsert.IdFunc = (int)base.Insert(objInsert);
            return (uint)objInsert.IdFunc;*/
            throw new NotSupportedException();
        }

        public override int Update(Funcionario objUpdate)
        {
            /*var tipoFunc = ObtemValorCampo<int>("idTipoFunc", "idFunc=" + objUpdate.IdFunc);
            var situacao = ObtemSituacao((uint)objUpdate.IdFunc);

            if (!String.IsNullOrEmpty(objUpdate.EstCivil))
            {
                // Faz com que o estado civil seja salvo sempre com a primeira letra maiúscula e todas as outras minúsculas.
                var estadoCivil = objUpdate.EstCivil.Substring(1, objUpdate.EstCivil.Length - 1);
                objUpdate.EstCivil = objUpdate.EstCivil.Substring(0, 1).ToUpper() + estadoCivil.ToLower();
            }

            if (UserInfo.GetUserInfo.TipoUsuario != (uint)Utils.TipoFuncionario.Administrador &&
                UserInfo.GetUserInfo.IsAdminSync)
            {
                // Se qualquer um que não seja Administrador tentar alterar qualquer funcionário para administrador, impede
                if (objUpdate.IdTipoFunc == (int)Utils.TipoFuncionario.Administrador)
                    objUpdate.IdTipoFunc = tipoFunc;

                // Se qualquer um que não seja Administrador tentar mudar o cargo de um administrador para qualquer outro, impede.
                if (tipoFunc == (int)Utils.TipoFuncionario.Administrador)
                    objUpdate.IdTipoFunc = (int)Utils.TipoFuncionario.Administrador;
            }

            //Se o funciário era marcador de produção e passou a ser de outro tipo
            //remove os registros da tabela funconario_setor
            if (tipoFunc == (int)Utils.TipoFuncionario.MarcadorProducao &&
                objUpdate.IdTipoFunc != (int)Utils.TipoFuncionario.MarcadorProducao)
            {
                FuncionarioSetorDAO.Instance.DeleteByIdFunc((uint)objUpdate.IdFunc);
            }

            if ((tipoFunc == (int)Utils.TipoFuncionario.InstaladorComum ||
                tipoFunc == (int)Utils.TipoFuncionario.InstaladorTemperado ||
                tipoFunc == (int)Utils.TipoFuncionario.MotoristaInstalador) &&
                objUpdate.IdTipoFunc != (uint)Utils.TipoFuncionario.InstaladorComum &&
                objUpdate.IdTipoFunc != (uint)Utils.TipoFuncionario.InstaladorTemperado &&
                objUpdate.IdTipoFunc != (uint)Utils.TipoFuncionario.MotoristaInstalador &&
                objPersistence.ExecuteSqlQueryCount("select count(*) from func_equipe where idFunc=" + objUpdate.IdFunc) > 0)
            {
                throw new Exception("Não é possível alterar o tipo de um funcionário instalador que pertence a uma equipe de instalação. " +
                    "Para alterar seu tipo para um tipo que não seja instalador remova o funcionário da equipe antes.");
            }

            // Caso o funcionário tenha permissão de emitir pedidos/orçamento é necessário verificar se o tipo de funcionário esteja sendo alterado
            // para um tipo de funcionario que dê permissão de emitir pedidos/orçamentos, é necessário verificar também se a situação está sendo alterada.
            if (IsVendedor((uint)objUpdate.IdFunc, (uint)tipoFunc) && !IsVendedor((uint)objUpdate.IdFunc, (uint)objUpdate.IdTipoFunc))
            {
                if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From pedido Where idFunc=" + objUpdate.IdFunc +
                    " And situacao Not In (" + (int)Pedido.SituacaoPedido.Cancelado + "," + (int)Pedido.SituacaoPedido.Confirmado + ")") > 0)
                    throw new Exception("Este funcionário está associado à pedidos que não estão liberados ou cancelados, desassocie-o " +
                        "destes pedidos antes de alterar seu tipo de funcionário ou sua situação.");

                if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From orcamento Where idFunc=" + objUpdate.IdFunc +
                    " And situacao Not In (" + (int)Orcamento.SituacaoOrcamento.Negociado + "," +
                    (int)Orcamento.SituacaoOrcamento.NaoNegociado + ")") > 0)
                    throw new Exception("Este funcionário está associado à orçamentos em aberto, desassocie-o " +
                        "destes orçamentos antes de alterar seu tipo de funcionário ou sua situação.");
            }

            // Reseta lista de funcionários logados para aplicar alteração
            UserInfo.ZeraListaUsuariosLogados();

            objUpdate.SetoresFunc = FuncionarioSetorDAO.Instance.GetDescricaoSetores((uint)objUpdate.IdFunc);
            LogAlteracaoDAO.Instance.LogFuncionario(objUpdate, LogAlteracaoDAO.SequenciaObjeto.Novo);

            // Recupera a senha direto do BD, pois caso o usuário tenha alterado a senha dentro do cadastro de funcionário
            // (no botão "Alterar Senha"), ao atualizar o cadastro do funcionário a senha alterada é mantida e não a que está no viewstate
            objUpdate.Senha = ObtemValorCampo<string>("Senha", "idFunc=" + objUpdate.IdFunc);

            return base.Update(objUpdate);*/
            throw new NotSupportedException();
        }

        public override int Delete(Funcionario objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdFunc);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            if (Key == 0)
                return 0;

            // Verifica se o funcionário possui orçamentos relacionados à seu id
            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar("Select count(*) From orcamento Where idFunc=" + Key).ToString()) > 0)
                throw new Exception("Este funcionário não pode ser excluído por possuir orçamentos relacionados ao mesmo. Para impedir seu login no sistema, inative-o.");

            // Verifica se o funcionário possui pedidos relacionados à seu id
            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar("Select count(*) From pedido Where idFunc=" + Key + " Or usuConf=" + Key).ToString()) > 0)
                throw new Exception("Este funcionário não pode ser excluído por possuir pedidos relacionados ao mesmo. Para impedir seu login no sistema, inative-o.");

            // Verifica se o funcionário possui medições relacionados à seu id
            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar("Select count(*) From medicao Where idFunc=" + Key + " Or idFuncConf=" + Key + " Or idFuncMed=" + Key).ToString()) > 0)
                throw new Exception("Este funcionário não pode ser excluído por possuir medições relacionadas ao mesmo. Para impedir seu login no sistema, inative-o.");

            // Verifica se o funcionário possuir instalações relacionadas à seu id
            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar("Select count(*) From func_equipe Where idFunc=" + Key).ToString()) > 0)
                throw new Exception("Este funcionário não pode ser excluído por fazer parte de uma equipe de instalação. Para impedir seu login no sistema, inative-o.");

            // Verifica se o funcionário possuir projetos relacionados à seu id
            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar("Select count(*) From projeto Where idFunc=" + Key).ToString()) > 0)
                throw new Exception("Este funcionário não pode ser excluído por possuir projetos relacionados ao mesmo. Para impedir seu login no sistema, inative-o.");

            // Verifica se o funcionário possuir leituras na produção relacionados à seu id
            if (LeituraProducaoDAO.Instance.VerificarFuncionarioPossuiLeitura(null, (int)Key))
                throw new Exception("Este funcionário não pode ser excluído por possuir leituras na produção relacionados ao mesmo. Para impedir seu login no sistema, inative-o.");

            // Verifica se o funcionário possui ordens de carga relacionados à seu id
            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar("Select count(*) From ordem_carga Where UsuCad=" + Key).ToString()) > 0)
                throw new Exception("Este funcionário não pode ser excluído por possuir ordens de carga relacionados ao mesmo. Para impedir seu login no sistema, inative-o.");

            // Verifica se o funcionário possui movimentações bancárias relacionadas à seu id
            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar("Select count(*) From mov_banco Where UsuCad=" + Key).ToString()) > 0)
                throw new Exception("Este funcionário não pode ser excluído por possuir movimentações bancárias relacionadas ao mesmo. Para impedir seu login no sistema, inative-o.");

            LogAlteracaoDAO.Instance.ApagaLogFuncionario(Key);
            return base.DeleteByPrimaryKey(Key);
        }

        #endregion
    }
}
