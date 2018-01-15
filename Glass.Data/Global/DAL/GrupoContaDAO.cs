using System;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public sealed class GrupoContaDAO : BaseDAO<GrupoConta, GrupoContaDAO>
	{
        //private GrupoContaDAO() { }

        public string GetAllForSql()
        {
            string sql = "select cast(group_concat(idGrupo) as char) from grupo_conta where idGrupo not in (" + UtilsPlanoConta.GetGruposExcluirFluxoSistema + ")";
            return objPersistence.ExecuteScalar(sql).ToString();
        }

        #region Busca Ids dos grupos de conta

        /// <summary>
        /// Busca Ids dos grupos de conta.
        /// </summary>
        /// <param name="descricao"></param>
        /// <returns></returns>
        public string GetIds(string descricao)
        {
            string sql = "select idGrupo from grupo_conta where descricao like ?descricao";
            return GetValoresCampo(sql, "idGrupo", new GDAParameter("?descricao", "%" + descricao + "%"));
        }

        #endregion

        public override uint Insert(GrupoConta objInsert)
        {
            throw new NotSupportedException();
        }

        public override int Update(GrupoConta objUpdate)
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

        public override int Delete(GrupoConta objDelete)
        {
            throw new NotSupportedException();
        }

        public override int DeleteByPrimaryKey(int Key)
        {
            /*// Verifica se esta categoria está sendo usada em algum grupo de conta
            if (objPersistence.ExecuteSqlQueryCount("Select Count(*) From plano_contas Where idGrupo=" + Key) > 0)
                throw new Exception("Este grupo não pode ser excluído por haver planos de conta relacionadas ao mesmo.");

            return base.DeleteByPrimaryKey(Key);*/
            throw new NotSupportedException();

        }

        /*public int SetPontoEquilibrio(uint idGrupo, bool valor)
        {
            string sql = "update grupo_conta set pontoEquilibrio=" + valor + " where idGrupo=" + idGrupo;
            return objPersistence.ExecuteCommand(sql);
        }*/

        /// <summary>
        /// Troca posição do grupo de contas
        /// </summary>
        /// <param name="idGrupo"></param>
        /// <param name="acima"></param>
        public void ChangePosition(uint idGrupo, bool acima)
        {
            /*int numSeq = ObtemValorCampo<int>("numSeq", "idGrupo=" + idGrupo);

            // Altera a posição do grupo adjacente à esta
            objPersistence.ExecuteCommand("Update grupo_conta Set numSeq=numSeq" + (acima ? "+1" : "-1") +
                " Where numSeq=" + (numSeq + (acima ? -1 : 1)));

            // Altera a posição deste grupo
            objPersistence.ExecuteCommand("Update grupo_conta Set numSeq=numSeq" + (acima ? "-1" : "+1") +
                " Where idGrupo=" + idGrupo);*/
            throw new NotSupportedException();
        }
       
	}
}