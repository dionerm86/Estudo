﻿using System.Collections.Generic;
using GDA;
using Glass.Data.Model.Cte;

namespace Glass.Data.DAL.CTe
{
    public sealed class ProprietarioVeiculoDAO : BaseDAO<ProprietarioVeiculo, ProprietarioVeiculoDAO>
    {
        //private ProprietarioVeiculoDAO() { }

        #region Busca padrão

        private string Sql(string placa, uint idProprietario, bool selecionar)
        {
            string sql = "Select * From proprietario_veiculo Where 1";

            if(!selecionar)
                sql = "Select count(*) From proprietario_veiculo Where 1";

            if (!string.IsNullOrEmpty(placa))
                sql += " And Placa='" + placa + "'";

            if (idProprietario > 0)
                sql += " And IDPROPVEIC=" + idProprietario;

            return sql;
        }

        public ProprietarioVeiculo GetElement(uint idProprietario)
        {
            try
            {
                return objPersistence.LoadOneData(Sql("", idProprietario, true));
            }
            catch
            {
                return new ProprietarioVeiculo();
            }
        }

        public IList<ProprietarioVeiculo> GetList(string sortExpression, int startRow, int pageSize)
        {
            return LoadDataWithSortExpression(Sql("", 0, true), sortExpression, startRow, pageSize, null);
        }

        public List<ProprietarioVeiculo> GetList(string placa, uint idPropVeiculo)
        {
            return objPersistence.LoadData(Sql(placa, idPropVeiculo, true));
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql("", 0, false), null);
        }

        #endregion

        public override uint Insert(ProprietarioVeiculo objInsert)
        {
            uint idPropVeiculo = base.Insert(objInsert);            

            return idPropVeiculo;
        }

        public override int Delete(ProprietarioVeiculo objDelete)
        {
            if (ExecuteScalar<int>("Select COUNT(*) FROM proprietario_veiculo_veiculo WHERE IdPropVeic=" + objDelete.IdPropVeic) > 0)
                throw new System.Exception("Este proprietário está associado à uma veículo e não pode ser excluído");

            return base.Delete(objDelete);
        }

        /// <summary>
        /// Exclui um proprietário de veículo.
        /// </summary>
        /// <param name="sessao">A transação atual.</param>
        /// <param name="key">O identificador do proprietário de veículo que será excluido.</param>
        /// <returns>Um número inteiro contendo o número de linhas afetadas pela execução do sql.</returns>
        public override int DeleteByPrimaryKey(GDASession sessao, int key)
        {
            if (ExecuteScalar<int>("Select COUNT(*) FROM proprietario_veiculo_veiculo WHERE IdPropVeic=" + key) > 0)
                throw new System.Exception("Este proprietário está associado à uma veículo e não pode ser excluído");

            return base.DeleteByPrimaryKey(sessao, key);
        }

        public override int Update(ProprietarioVeiculo objUpdate)
        {
            return base.Update(objUpdate);
        }
    }
}
