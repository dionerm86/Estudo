using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public sealed class EquipeDAO : BaseDAO<Equipe, EquipeDAO>
    {
        //private EquipeDAO() { }

        #region Listagem padrão

        private string Sql(uint idEquipe, bool selecionar)
        {
            string campos = selecionar ? "e.*, (Concat(v.Placa, ' ', v.Modelo, ' ', v.Cor, ' ', Cast(v.AnoFab as char))) as DescrVeiculo " : "Count(*)";
            
            string sql = "Select " + campos + " From equipe e " + 
                "Left Join veiculo v On (e.Placa=v.Placa) Where 1";

            if (idEquipe > 0)
                sql += " And IdEquipe=" + idEquipe;

            return sql;
        }

        public Equipe GetElement(uint idEquipe)
        {
            return objPersistence.LoadOneData(Sql(idEquipe, true));
        }

        public IList<Equipe> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql(0, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, false));
        }

        #endregion

        #region Listagem para relatório

        private string SqlRpt(int tipo, bool selecionar)
        {
            string campos = selecionar ? "e.*, (Concat(v.Placa, ' ', v.Modelo, ' ', v.Cor, ' ', Cast(v.AnoFab as char))) as DescrVeiculo, '$$$' as Criterio " : "Count(*)";
            string criterio = String.Empty;

            string sql = "Select " + campos + " From equipe e " +
                "Left Join veiculo v On (e.Placa=v.Placa) Where 1";

            if (tipo > 0)
            {
                sql += " And tipo=" + tipo;
                criterio += tipo == 1 ?  "Tipo: Comum    " : tipo == 2 ? "Tipo: Temperado    " : String.Empty;
            }

            return sql.Replace("$$$", criterio);
        }

        public IList<Equipe> GetRpt(int tipo)
        {
            return objPersistence.LoadData(SqlRpt(tipo, true)).ToList();
        }

        public IList<Equipe> GetListRpt(int tipo, string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(SqlRpt(tipo, true), sortExpression, startRow, pageSize, null);
        }

        public int GetCountRpt(int tipo)
        {
            return objPersistence.ExecuteSqlQueryCount(SqlRpt(tipo, false));
        }

        #endregion

        #region Busca Equipes ordenando pelo nome

        public Equipe[] GetOrdered()
        {
            string sql = "Select * From equipe Where situacao=" + (int)Equipe.SituacaoEnum.Ativa + " Order By nome";

            var lstEquipe = objPersistence.LoadData(sql).ToList();
            
            foreach (Equipe e in lstEquipe)
            {
                string nomeEquipe = e.Nome + ":";

                var lstFunc = FuncEquipeDAO.Instance.GetByEquipe(e.IdEquipe);

                foreach (FuncEquipe f in lstFunc)
                    nomeEquipe += " " + BibliotecaTexto.GetFirstName(f.NomeFunc) + ",";

                e.NomeEstendido = nomeEquipe.TrimEnd(',');
            }

            return lstEquipe.ToArray();
        }

        #endregion

        /// <summary>
        /// Busca as equipes que são do tipo do usuário logado. Por exemplo, se o usuário logado
        /// for supervisor de colocação temperado, busca apenas equipes de temperado.
        /// </summary>
        /// <returns></returns>
        public Equipe[] GetByTipo()
        {
            string sql = "Select * From equipe Where Situacao=" + (int)Equipe.SituacaoEnum.Ativa;

            LoginUsuario loginUsuario = UserInfo.GetUserInfo;

            bool instComum = Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoComum);
            bool instTemp = Config.PossuiPermissao(Config.FuncaoMenuInstalacao.ControleInstalacaoTemperado);
            
            if (instComum && !instTemp)
                sql += " And Tipo=" + (int)Equipe.TipoEquipeEnum.ColocacaoComum;
            else if (instTemp && !instComum)
                sql += " And Tipo=" + (int)Equipe.TipoEquipeEnum.ColocacaoTemperado;

            var lstEquipe = objPersistence.LoadData(sql + " Order By nome").ToList();
            
            foreach (Equipe e in lstEquipe)
            {
                string nomeEquipe = e.Nome + ":";

                var lstFunc = FuncEquipeDAO.Instance.GetByEquipe(e.IdEquipe);

                foreach (FuncEquipe f in lstFunc)
                    nomeEquipe += " " + BibliotecaTexto.GetFirstName(f.NomeFunc) + ",";

                e.NomeEstendido = nomeEquipe.TrimEnd(',');
            }

            return lstEquipe.ToArray();
        }

        public override int Delete(Equipe objDelete)
        {
            // Se a equipe possuir instalações associadas às mesmas, não permite exclusão, apenas inativa a mesma
            if (Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar("Select Count(*) From equipe_instalacao Where idEquipe=" + objDelete.IdEquipe).ToString()) > 0)
                return objPersistence.ExecuteCommand("Update equipe Set situacao=" + (int)Equipe.SituacaoEnum.Inativa + " Where idEquipe=" + objDelete.IdEquipe);
            else
                return base.Delete(objDelete);
        }

        public string ObtemNome(uint idEquipe)
        {
            return ObtemNome(null, idEquipe);
        }

        public string ObtemNome(GDASession session, uint idEquipe)
        {
            return ObtemValorCampo<string>(session, "nome", "idEquipe=" + idEquipe);
        }
	}
}