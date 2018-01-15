using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using Glass.Configuracoes;
using Glass.Data.DAL.CTe;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class PlanoContasDAO : BaseDAO<PlanoContas, PlanoContasDAO>
	{
        //private PlanoContasDAO() { }

        #region Listagem padrão de planos de conta

        private string SqlList(uint idGrupo, string descricao, int situacao, bool selecionar)
        {
            string campos = selecionar ? "p.*, g.Descricao as DescrGrupo, cat.Descricao as DescrCategoria" : "Count(*)";

            string sql = @"
                Select " + campos + @" From plano_contas p 
                    Inner Join grupo_conta g On (p.IdGrupo=g.IdGrupo) 
                    Left Join categoria_conta cat On (cat.IdCategoriaConta=g.IdCategoriaConta) 
                Where p.idGrupo not in (" + UtilsPlanoConta.GetGruposExcluirFluxoSistema + ")";

            if (idGrupo > 0)
                sql += " And p.idGrupo=" + idGrupo;

            if (!String.IsNullOrEmpty(descricao))
                sql += " And p.descricao like ?descricao";

            if (situacao > 0)
                sql += " And p.situacao=" + situacao;

            return sql;
        }

        public IList<PlanoContas> GetList(uint idGrupo, int situacao, string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal(idGrupo, situacao) == 0)
                return new PlanoContas[] { new PlanoContas() };

            sortExpression = String.IsNullOrEmpty(sortExpression) ? "cat.NumSeq, g.NumSeq, g.Descricao, p.Descricao" : sortExpression;

            return LoadDataWithSortExpression(SqlList(idGrupo, null, situacao, true), sortExpression, startRow, pageSize, null);
        }

        public IList<PlanoContas> GetForRpt(uint idGrupo, Situacao? situacao)
        {
            var sit = situacao == null ? 0 : (int)situacao;

            if (GetCountReal(idGrupo, sit) == 0)
                return new PlanoContas[] { new PlanoContas() };

            return objPersistence.LoadData(SqlList(idGrupo, null, (int)sit, true) + " Order By cat.NumSeq, g.NumSeq, g.Descricao, p.Descricao").ToList();
        }

        public int GetCountReal(uint idGrupo, int situacao)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(idGrupo, null, situacao, false), null);
        }

        public int GetCount(uint idGrupo, int situacao)
        {
            int count = objPersistence.ExecuteSqlQueryCount(SqlList(idGrupo, null, situacao, false), null);

            return count == 0 ? 1 : count;
        }

        #endregion
        
        #region Listagem para tela de seleção

        public IList<PlanoContas> GetListSel(uint idGrupo, string descricao, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = String.IsNullOrEmpty(sortExpression) ? "g.Descricao, p.Descricao" : sortExpression;

            return LoadDataWithSortExpression(SqlList(idGrupo, descricao, (int)PlanoContas.SituacaoEnum.Ativo, true), sortExpression, startRow, pageSize, GetParam(descricao));
        }

        public int GetCountSel(uint idGrupo, string descricao)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(idGrupo, descricao, (int)PlanoContas.SituacaoEnum.Ativo, false), GetParam(descricao));
        }

        private GDAParameter[] GetParam(string descricao)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(descricao))
                lstParam.Add(new GDAParameter("?descricao", "%" + descricao + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #endregion

        #region Retorna o idConta pelo idGrupo e idContaGrupo

        /// <summary>
        /// Retorna a chave da tabela correspondente ao Grupo e ao Plano de contas passado
        /// </summary>
        /// <param name="idContaGrupo"></param>
        /// <param name="idGrupo"></param>
        /// <returns></returns>
        public uint GetId(uint idContaGrupo, uint idGrupo)
        {
            object idConta = objPersistence.ExecuteScalar("Select idConta From plano_contas Where IdContaGrupo=" + idContaGrupo + " And IdGrupo=" + idGrupo);

            if ((idConta == null || idConta.ToString() == "0" || idContaGrupo.ToString().Trim() == string.Empty) && idGrupo != 8)
            {
                var mensagem = string.Format("Plano de contas não existe. Detalhes: IdContaGrupo: {0} - IdGrupo: {1}", idContaGrupo, idGrupo);

                ErroDAO.Instance.InserirFromException(mensagem, new Exception(mensagem));

                throw new Exception(mensagem);
            }

            if (idConta == null)
                return 0;

            return Glass.Conversoes.StrParaUint(idConta.ToString());
        }

        #endregion

        #region Busca Ids dos planos de conta

        /// <summary>
        /// Busca Ids dos planos de conta.
        /// </summary>
        /// <param name="descricao"></param>
        /// <returns></returns>
        public string GetIds(string descricao)
        {
            string sql = "select idConta from plano_contas where descricao like ?descricao";
            return GetValoresCampo(sql, "idConta", new GDAParameter("?descricao", "%" + descricao + "%"));
        }

        #endregion

        #region Retorna o plano de conta pelo idConta

        /// <summary>
        /// Retorna o plano de conta pelo idConta
        /// </summary>
        /// <param name="idConta"></param>
        /// <returns></returns>
        public PlanoContas GetByIdConta(uint idConta)
        {
            string sql = @"
                Select p.*, g.Descricao as DescrGrupo 
                From plano_contas p
                    Inner Join grupo_conta g On (p.idGrupo=g.idGrupo)
                Where p.IdConta=" + idConta;

            return objPersistence.LoadOneData(sql);
        }

        #endregion

        #region Verifica se o plano de conta passado está sendo usado

        /// <summary>
        /// Verifica se o plano de conta passado está sendo usado
        /// </summary>
        /// <param name="idConta"></param>
        /// <param name="config">Identifica se este método está sendo chamado da tela de configurações</param>
        /// <returns></returns>
        public bool EstaSendoUsado(uint idConta, bool config)
        {
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From caixa_geral Where idConta=" + idConta) > 0)
                return true;

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From mov_banco Where idConta=" + idConta) > 0)
                return true;

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From mov_func Where idConta=" + idConta) > 0)
                return true;

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From caixa_diario Where idConta=" + idConta) > 0)
                return true;

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From custo_fixo Where idConta=" + idConta) > 0)
                return true;

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From nota_fiscal Where idConta=" + idConta) > 0)
                return true;

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From fornecedor Where idConta=" + idConta) > 0)
                return true;

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From contas_receber Where idConta=" + idConta) > 0)
                return true;

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From contas_pagar Where idConta=" + idConta) > 0)
                return true;

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From compra Where idConta=" + idConta) > 0)
                return true;

            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From imposto_serv Where idConta=" + idConta) > 0)
                return true;

            if (!config && objPersistence.ExecuteSqlQueryCount("Select Count(*) From config_loja Where valorInteiro=" + idConta +
                " And idConfig in (" + (int)Config.ConfigEnum.PlanoContaTaxaAntecip + "," + (int)Config.ConfigEnum.PlanoContaComissao + "," +
                (int)Config.ConfigEnum.PlanoContaJurosAntecip + "," + (int)Config.ConfigEnum.PlanoContaIOFAntecip + "," +
                (int)Config.ConfigEnum.PlanoContaJurosReceb + "," + (int)Config.ConfigEnum.PlanoContaMultaReceb + "," +
                (int)Config.ConfigEnum.PlanoContaJurosPagto + "," + (int)Config.ConfigEnum.PlanoContaMultaPagto + "," +
                (int)Config.ConfigEnum.PlanoContaEstornoJurosReceb + "," + (int)Config.ConfigEnum.PlanoContaEstornoMultaReceb + "," +
                (int)Config.ConfigEnum.PlanoContaEstornoJurosPagto + "," + (int)Config.ConfigEnum.PlanoContaEstornoMultaPagto + ")") > 0)
                return true;

            return false;
        }

        #endregion

        #region Busca planos de conta de acordo com o tipo

        /// <summary>
        /// SQL de Plano de Contas por tipo
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="idGrupo"></param>
        /// <param name="descricao"></param>
        /// <param name="selecionar"></param>
        /// <returns></returns>
        private string SqlPlanoContasPorTipo(int tipo, uint idGrupo, string descricao, bool selecionar)
        {
            var campos = selecionar ? " p.*, g.Descricao as DescrGrupo, cat.Descricao as DescrCategoria" : " Count(*)";
            var sql = "Select" + campos + @" From plano_contas p
                Inner Join grupo_conta g On (p.IdGrupo=g.IdGrupo)
                Left Join categoria_conta cat On (g.IdCategoriaConta=cat.IdCategoriaConta)
                Where p.Situacao=" + (int)PlanoContas.SituacaoEnum.Ativo + " And g.Situacao=" + (int)GrupoConta.SituacaoEnum.Ativo +
                " And g.idGrupo Not In (" + UtilsPlanoConta.GetGruposSistema + "," + UtilsPlanoConta.GetGruposExcluirFluxoSistema + @")";

            // Busca apenas movimentações que estejam em categorias de débito
            if (FinanceiroConfig.PlanoContaBloquearEntradaSaida)
                sql += " And cat.Tipo In (" + (tipo == 1 ? ((int)TipoCategoriaConta.Receita).ToString() :
                    (int)TipoCategoriaConta.DespesaVariavel + "," + (int)TipoCategoriaConta.DespesaFixa) + ")";

            if (idGrupo > 0)
                sql += " And p.idGrupo=" + idGrupo;

            if (!string.IsNullOrEmpty(descricao))
                sql += " And p.descricao like ?descricao";

            return sql;
        }

        /// <summary>
        /// Retorna todos os planos de contas exceto os de controle interno do sistema, filtrando por entrada/saída
        /// </summary>
        /// <param name="tipo">1-Crébito, 2-Débito</param>
        /// <returns></returns>
        public IList<PlanoContas> GetPlanoContas(int tipo)
        {
            var sql = SqlPlanoContasPorTipo(tipo, 0, null, true);

            if (!ProjetoConfig.InverterExibicaoPlanoConta)
                sql += " Order By g.Descricao, p.Descricao";
            else
                sql += " Order By p.Descricao, g.Descricao";

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Retorna todos os planos de contas exceto os de controle interno do sistema, filtrando por entrada/saída, idGrupo e Descricao
        /// </summary>
        /// <param name="tipo">1-Crébito, 2-Débito</param>
        /// <param name="idGrupo"></param>
        /// <param name="descricao"></param>
        /// <param name="sortExpression"></param>
        /// <param name="startRow"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public IList<PlanoContas> GetPlanoContasPeloTipo(int tipo, uint idGrupo, string descricao, string sortExpression, int startRow, int pageSize)
        {
            var sql = SqlPlanoContasPorTipo(tipo, idGrupo, descricao, true);

            if (!ProjetoConfig.InverterExibicaoPlanoConta)
                sql += " Order By g.Descricao, p.Descricao";
            else
                sql += " Order By p.Descricao, g.Descricao";

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, GetParam(descricao));
        }

        /// <summary>
        /// Retorna o Count do plano de conta por tipo
        /// </summary>
        /// <param name="tipo"></param>
        /// <param name="idGrupo"></param>
        /// <param name="descricao"></param>
        /// <returns></returns>
        public int GetCountPlanoContasPeloTipo(int tipo, uint idGrupo, string descricao)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlPlanoContasPorTipo(tipo, idGrupo, descricao, false), GetParam(descricao));
        }

        /// <summary>
        /// Retorna todos os planos de contas exceto os de controle interno do sistema, filtrando por entrada/saída
        /// </summary>
        /// <param name="tipo">1-Crébito, 2-Débito</param>
        /// <returns></returns>
        public string GetIdsPlanoContas(int tipo)
        {
            string sql = @"
                Select p.IdConta
                From plano_contas p 
                    Inner Join grupo_conta g On (p.IdGrupo=g.IdGrupo)
                    Left Join categoria_conta cat On (g.IdCategoriaConta=cat.IdCategoriaConta)
                Where p.Situacao=" + (int)PlanoContas.SituacaoEnum.Ativo + " And g.Situacao=" + (int)GrupoConta.SituacaoEnum.Ativo +
                " And g.idGrupo Not In (" + UtilsPlanoConta.GetGruposSistema + "," + UtilsPlanoConta.GetGruposExcluirFluxoSistema + @")";

            // Busca apenas movimentações que estejam em categorias de débito
            if (FinanceiroConfig.PlanoContaBloquearEntradaSaida)
                sql += " And cat.Tipo In (" + (tipo == 1 ? ((int)TipoCategoriaConta.Receita).ToString() :
                    (int)TipoCategoriaConta.DespesaVariavel + "," + (int)TipoCategoriaConta.DespesaFixa) + ") ";

            if (!ProjetoConfig.InverterExibicaoPlanoConta)
                sql += "Order By g.Descricao, p.Descricao";
            else
                sql += "Order By p.Descricao, g.Descricao";

            var contas =  ExecuteMultipleScalar<int>(sql);

            return string.Join(",", contas.Select(f => f.ToString()).ToArray());
        }

        /// <summary>
        /// Retorna todos os planos de contas utilizados na compra
        /// </summary>
        public IList<PlanoContas> GetPlanoContasCompra()
        {
            string sql = @"Select p.*, g.Descricao as DescrGrupo 
                From plano_contas p Inner Join grupo_conta g On (p.IdGrupo=g.IdGrupo) 
                Left Join categoria_conta cat On (g.IdCategoriaConta=cat.IdCategoriaConta) 
                Where p.Situacao=" + (int)PlanoContas.SituacaoEnum.Ativo + " And g.Situacao=" + (int)GrupoConta.SituacaoEnum.Ativo + 
                " And g.IdGrupo Not In (" + UtilsPlanoConta.GetGruposSistema + "," + UtilsPlanoConta.GetGruposExcluirFluxoSistema + ") ";

            // Busca apenas movimentações que estejam em categorias de débito
            if (FinanceiroConfig.PlanoContaBloquearEntradaSaida)
                sql += "And cat.Tipo In (" + (int)TipoCategoriaConta.DespesaFixa + "," + (int)TipoCategoriaConta.DespesaVariavel + ") ";
            
            sql += " Order By ";

            if (!ProjetoConfig.InverterExibicaoPlanoConta)
                sql += "g.Descricao, p.Descricao";
            else
                sql += "p.Descricao, g.Descricao";

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Retorna todos os planos de contas exceto os de controle interno do sistema, filtrando pela nota fiscal
        /// busca o idConta referenciado na NFe mesmo se estiver inativo.
        /// </summary>
        /// <param name="idNf">Id da nota fiscal</param>
        /// <returns></returns>
        private IList<PlanoContas> RecuperaIncluindoPlanoContas(uint idConta)
        {
            string sql = @"Select p.*, g.Descricao as DescrGrupo, cat.Descricao as DescrCategoria 
                From plano_contas p Inner Join grupo_conta g On (p.IdGrupo=g.IdGrupo)
                Left Join categoria_conta cat On (g.IdCategoriaConta=cat.IdCategoriaConta)
                Where (p.Situacao=" + (int)PlanoContas.SituacaoEnum.Ativo + " And g.Situacao=" + (int)GrupoConta.SituacaoEnum.Ativo +
                " And g.idGrupo Not In (" + UtilsPlanoConta.GetGruposSistema + "," + UtilsPlanoConta.GetGruposExcluirFluxoSistema + @"))
                Or p.idConta=" + idConta;

            // Busca apenas movimentações que estejam em categorias de débito
            if (FinanceiroConfig.PlanoContaBloquearEntradaSaida)
                sql += " And cat.Tipo In (" + (int)TipoCategoriaConta.DespesaVariavel + "," + (int)TipoCategoriaConta.DespesaFixa + ") ";

            if (!ProjetoConfig.InverterExibicaoPlanoConta)
                sql += " Order By g.Descricao, p.Descricao";
            else
                sql += " Order By p.Descricao, g.Descricao";

            return objPersistence.LoadData(sql).ToList();
        }

        /// <summary>
        /// Retorna todos os planos de contas exceto os de controle interno do sistema, filtrando pela nota fiscal
        /// busca o idConta referenciado na NFe mesmo se estiver inativo.
        /// </summary>
        /// <param name="idNf">Id da nota fiscal</param>
        /// <returns></returns>
        public IList<PlanoContas> GetPlanoContasNf(uint idNf)
        {
            uint idConta = NotaFiscalDAO.Instance.ObtemValorCampo<uint>("idConta", "idNf=" + idNf);
            return RecuperaIncluindoPlanoContas(idConta);
        }

        /// <summary>
        /// Retorna todos os planos de contas exceto os de controle interno do sistema, filtrando pela nota fiscal
        /// busca o idConta referenciado no CTe mesmo se estiver inativo.
        /// </summary>
        /// <param name="idCte">Id do CTe</param>
        /// <returns></returns>
        public IList<PlanoContas> GetPlanoContasCte(uint idCte)
        {
            uint idConta = CobrancaCteDAO.Instance.ObtemValorCampo<uint>("idConta", "idCte=" + idCte);
            return RecuperaIncluindoPlanoContas(idConta);
        }

        /// <summary>
        /// Busca planos de conta pelo grupo conta
        /// </summary>
        /// <param name="idGrupoConta"></param>
        /// <returns></returns>
        public PlanoContas[] GetByGrupo(int idGrupoConta)
        {
            string sql = string.Format("Select * From plano_contas Where situacao={0}", (int)Situacao.Ativo);

            if (idGrupoConta > 0)
                sql += " And idGrupo=" + idGrupoConta;

            List<PlanoContas> lst = objPersistence.LoadData(sql + " Order By Descricao");

            PlanoContas plano = new PlanoContas();
            plano.Descricao = "Todos";
            plano.IdConta = 0;

            lst.Insert(0, plano);

            return lst.ToArray();
        }

        #endregion

        #region Atualiza Grupo

        /// <summary>
        /// Atualiza o grupo de produto
        /// </summary>
        /// <param name="planoConta"></param>
        public void AtualizaGrupo(PlanoContas planoConta)
        {
            string sql = "Update plano_contas Set idGrupo=" + planoConta.IdGrupo + ", situacao=" + planoConta.Situacao +
                ", descricao=?descricao Where idConta=" + planoConta.IdConta;

            objPersistence.ExecuteCommand(sql, new GDAParameter("?descricao", planoConta.Descricao));
        }

        #endregion

        #region Insere novo plano de contas

        /// <summary>
        /// Retorna um incremento do último idConta do grupo passado
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <returns></returns>
        public uint CreateGrupoId(uint idGrupo)
        {
            string sql = "select Coalesce(max(idcontagrupo), 0)+1 from plano_contas where idgrupo=" + idGrupo;

            return Glass.Conversoes.StrParaUint(objPersistence.ExecuteScalar(sql).ToString());
        }

        public override uint Insert(PlanoContas objInsert)
        {
            /*objInsert.IdContaGrupo = CreateGrupoId(objInsert.IdGrupo);

            return base.Insert(objInsert);*/
            throw new NotSupportedException();
        }

        #endregion

        #region Busca a descrição do plano de contas

        /// <summary>
        /// Busca a descrição do plano de contas.
        /// </summary>
        public string GetDescricao(uint idConta, bool incluirDescrGrupo)
        {
            return GetDescricao(null, idConta, incluirDescrGrupo);
        }

        /// <summary>
        /// Busca a descrição do plano de contas.
        /// </summary>
        public string GetDescricao(GDASession session, uint idConta, bool incluirDescrGrupo)
        {
            string sql = !incluirDescrGrupo ? @"select descricao from plano_contas where idConta=" + idConta :
                @"select concat(g.descricao, ' - ', p.descricao) from plano_contas p left join grupo_conta g on (p.idGrupo=g.idGrupo) where p.idConta=" + idConta;

            var retorno = objPersistence.ExecuteScalar(session, sql);

            return retorno != null ? retorno.ToString() : string.Empty;
        }

        #endregion

        #region Busca a situação do plano de contas

        /// <summary>
        /// Busca a situação do plano de contas.
        /// </summary>
        public PlanoContas.SituacaoEnum ObterSituacao(GDASession session, int idConta)
        {
            var sql = "SELECT situacao FROM plano_contas WHERE IdConta=" + idConta;

            var retorno = objPersistence.ExecuteScalar(session, sql);

            return retorno != null ? (PlanoContas.SituacaoEnum)(retorno.ToString().StrParaInt()) : PlanoContas.SituacaoEnum.Inativo;
        }

        #endregion

        #region Métodos Sobrescritos

        public override int Update(PlanoContas objUpdate)
        {
            // Retira planos de contas de fornecedores que não sejam da categoria de débito
            /*string sql = @"
                Update fornecedor set idconta=null 
                Where idconta in (
                    Select idconta From plano_contas p 
                        Inner Join grupo_conta g On (p.IdGrupo=g.IdGrupo) 
                        Left Join categoria_conta c On (g.idcategoriaconta=c.idcategoriaconta) 
                    Where p.situacao=" + (int)PlanoContas.SituacaoEnum.Inativo + " or g.situacao=" + (int)GrupoConta.SituacaoEnum.Inativo +
                    " or c.tipo not in(" + (int)TipoCategoriaConta.DespesaVariavel + ", " + (int)TipoCategoriaConta.DespesaFixa + @") or g.idCategoriaConta is null
                );";

            objPersistence.ExecuteCommand(sql);

            return base.Update(objUpdate);*/
            throw new NotSupportedException();
        }

        public override int Delete(PlanoContas objDelete)
        {
            throw new NotSupportedException();
            //return DeleteByPrimaryKey(objDelete.IdConta);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            /*if (EstaSendoUsado(Key, false))
                throw new Exception("Este plano de conta não pode ser excluído. Existem registros relacionados ao mesmo.");

            return base.DeleteByPrimaryKey(Key);*/

            throw new NotSupportedException();
        }

        #endregion
    }
}