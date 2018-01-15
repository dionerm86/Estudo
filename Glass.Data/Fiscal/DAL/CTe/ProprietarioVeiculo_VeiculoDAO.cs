using System.Collections.Generic;
using Glass.Data.Model.Cte;

namespace Glass.Data.DAL.CTe
{
    public sealed class ProprietarioVeiculo_VeiculoDAO : BaseDAO<ProprietarioVeiculo_Veiculo, ProprietarioVeiculo_VeiculoDAO>
    {
        //private ProprietarioVeiculo_VeiculoDAO() { }

        #region Busca padrão

        private string Sql(string placa, uint idProprietario, bool selecionar)
        {
            string sql = "Select * From proprietario_veiculo_veiculo as prop Where 1";

            if(!selecionar)
                sql = "Select count(*) From proprietario_veiculo_veiculo as prop Where 1";

            if (!string.IsNullOrEmpty(placa))
                sql += " And Placa='" + placa + "'";

            if (idProprietario > 0)
                sql += " And IDPROPVEIC=" + idProprietario;
            
            return sql;
        }

        public ProprietarioVeiculo_Veiculo GetElement(string placa, uint idProprietario)
        {
            try
            {
                return objPersistence.LoadOneData(Sql(placa, idProprietario, true));
            }
            catch
            {
                return new ProprietarioVeiculo_Veiculo();
            }
        }

        public IList<ProprietarioVeiculo_Veiculo> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql("", 0, true), sortExpression, startRow, pageSize, null);
        }

        public List<ProprietarioVeiculo_Veiculo> GetList(string placa, uint idProprietario)
        {
            return objPersistence.LoadData(Sql(placa, idProprietario, true)).ToList();
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql("", 0, false), null);
        }

        #endregion

        public override uint Insert(ProprietarioVeiculo_Veiculo objInsert)
        {
            uint idPropVeiculo = base.Insert(objInsert);

            return idPropVeiculo;
        }

        public new uint Update(ProprietarioVeiculo_Veiculo objUpdate)
        {
            var id = Delete(objUpdate);
            return base.Insert(objUpdate);
        }
    }
}
