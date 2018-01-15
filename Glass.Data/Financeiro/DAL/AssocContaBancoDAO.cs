using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class AssocContaBancoDAO : BaseDAO<AssocContaBanco, AssocContaBancoDAO>
    {
        //private AssocContaBancoDAO() { }

        private AssocContaBanco GetContaBanco(uint idTipoCartao, uint idTipoBoleto, uint idLoja)
        {
            string sql = "select * from assoc_conta_banco where idLoja=" + idLoja;
            if (idTipoCartao > 0)
                sql += " and idTipoCartao=" + idTipoCartao;

            if (idTipoBoleto > 0)
                sql += " and idTipoBoleto=" + idTipoBoleto;

            var consulta = objPersistence.LoadData(sql).ToList();

            if (consulta.Count > 0)
                return consulta[0];
            else
            {
                AssocContaBanco retorno = new AssocContaBanco();
                retorno.IdLoja = idLoja;
                retorno.IdTipoCartao = idTipoCartao > 0 ? (uint?)idTipoCartao : null;
                retorno.IdTipoBoleto = idTipoBoleto > 0 ? (uint?)idTipoBoleto : null;

                return retorno;
            }
        }

        public AssocContaBanco GetContaBancoCartao(uint idTipoCartao, uint idLoja)
        {
            return GetContaBanco(idTipoCartao, 0, idLoja);
        }

        public void AtualizarTipoCartao(uint idTipoCartao, uint? idContaBanco, bool bloquearContaBanco, uint idLoja)
        {
            AssocContaBanco item = GetContaBanco(idTipoCartao, 0, idLoja);

            if (idContaBanco > 0)
            {
                item.IdContaBanco = idContaBanco.Value;
                item.BloquearContaBanco = bloquearContaBanco;

                InsertOrUpdate(item);
            }
            else if (item.IdAssocContaBanco > 0)
                DeleteByPrimaryKey(item.IdAssocContaBanco);
        }

        public IList<AssocContaBanco> GetByLoja(uint idLoja)
        {
            return objPersistence.LoadData("select * from assoc_conta_banco where idLoja=" + idLoja).ToList();
        }

        #region Métodos sobrescritos

        public override void InsertOrUpdate(AssocContaBanco objUpdate)
        {
            if (objUpdate.IdAssocContaBanco > 0)
            {
                objPersistence.ExecuteCommand("update assoc_conta_banco set idTipoCartao=" + (objUpdate.IdTipoCartao > 0 ? objUpdate.IdTipoCartao.ToString() : "null") +
                    ", idTipoBoleto=" + (objUpdate.IdTipoBoleto > 0 ? objUpdate.IdTipoBoleto.ToString() : "null") + ", idContaBanco=" +
                    (objUpdate.IdContaBanco > 0 ? objUpdate.IdContaBanco.ToString() : "null") + ", bloquearContaBanco=" +
                    objUpdate.BloquearContaBanco.ToString().ToLower() + " where idAssocContaBanco=" + objUpdate.IdAssocContaBanco);
            }
            else
            {
                objPersistence.ExecuteCommand("insert into assoc_conta_banco (idTipoCartao, idTipoBoleto, idContaBanco, bloquearContaBanco, idLoja) values (" +
                    (objUpdate.IdTipoCartao > 0 ? objUpdate.IdTipoCartao.ToString() : "null") + ", " +
                    (objUpdate.IdTipoBoleto > 0 ? objUpdate.IdTipoBoleto.ToString() : "null") + ", " +
                    (objUpdate.IdContaBanco > 0 ? objUpdate.IdContaBanco.ToString() : "null") + ", " +
                    objUpdate.BloquearContaBanco.ToString().ToLower() + ", " + objUpdate.IdLoja + ")");
            }
        }

        #endregion
    }
}
