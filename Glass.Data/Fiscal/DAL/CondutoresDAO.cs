using GDA;
using Glass.Data.DAL;
using Glass.Data.Model;
using System.Collections.Generic;

namespace Glass.Data.DAL
{
    public sealed class CondutoresDAO : BaseDAO<Condutores, CondutoresDAO>
    {
        /// <summary>
        /// Recupera a listagem de condutores.
        /// </summary>
        public IList<Condutores> GetList()
        {
            return objPersistence.LoadData("select * from condutores").ToList();
        }

        public override uint Insert(GDASession sessao, Condutores objInsert)
        {
            if (objPersistence.ExecuteSqlQueryCount(sessao, $"SELECT * FROM condutores WHERE CPF=?cpf", new GDA.GDAParameter("?cpf", objInsert.Cpf)) > 0)
            {
                throw new System.Exception("Já existe um condutor cadastrado com o mesmo Cpf");
            }

            return base.Insert(sessao, objInsert);
        }

        public override int Update(GDASession sessao, Condutores objUpdate)
        {
            if (objPersistence.ExecuteSqlQueryCount(sessao, $"SELECT * FROM condutores WHERE IDCONDUTOR!=?idCondutor AND CPF=?cpf",
                new GDA.GDAParameter("?idCondutor", objUpdate.IdCondutor), new GDA.GDAParameter("?cpf", objUpdate.Cpf)) > 0)
            {
                throw new System.Exception("Já existe um condutor cadastrado com o mesmo Cpf");
            }

            return base.Update(sessao, objUpdate);
        }

        public override int DeleteByPrimaryKey(GDASession sessao, int idCondutor)
        {
            if (objPersistence.ExecuteSqlQueryCount(sessao, $"SELECT * FROM condutor_veiculo_mdfe WHERE IDCONDUTOR!=?idCondutor",
                new GDA.GDAParameter("?idCondutor", idCondutor)) > 0)
            {
                throw new System.Exception("O Condutor está associado a um MDFe.");
            }

            return base.DeleteByPrimaryKey(sessao, idCondutor);
        }
    }
}
