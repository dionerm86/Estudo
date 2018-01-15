using System;
using System.Collections.Generic;
using GDA;
using Glass.Data.Model;
using Glass.Data.Helper;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class TipoFuncDAO : BaseDAO<TipoFuncionario, TipoFuncDAO>
	{
        //private TipoFuncDAO() { }

        private string Sql(bool selecionar)
        {
            string campos = selecionar ? "*" : "Count(*)";

            string sql = "Select " + campos + " From tipo_func";

            return sql;
        }

        public IList<TipoFuncionario> GetList(string sortExpression, int startRow, int pageSize)
        {
            string filtro = String.IsNullOrEmpty(sortExpression) ? "Descricao" : sortExpression;

            return LoadDataWithSortExpression(Sql(true), filtro, startRow, pageSize);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(false));
        }

        public IList<TipoFuncionario> GetOrdered()
        {
            return GetOrdered(false, false);
        }

        public string GetDescricao(uint idTipoFunc)
        {
            return ObtemValorCampo<string>("descricao", "idTipoFunc=" + idTipoFunc);
        }

        public IList<TipoFuncionario> GetOrdered(bool incluirSetor, bool removerMarcadorProducaoSemSetor)
        {
            var sql = "Select * From tipo_func";
            var retorno = objPersistence.LoadData(sql).ToList();

            if (incluirSetor)
            {
                TipoFuncionario producao = retorno
                    .Where(t => t.IdTipoFuncionario == (uint)Utils.TipoFuncionario.MarcadorProducao).FirstOrDefault();

                if (producao != null)
                {
                    // Remove o marcador de produção
                    if (removerMarcadorProducaoSemSetor)
                        retorno.Remove(producao);

                    // Insere vários marcadores de produção
                    foreach (Setor s in Utils.GetSetores)
                    {
                        TipoFuncionario novo = new TipoFuncionario();
                        novo.IdTipoFuncionario = producao.IdTipoFuncionario;
                        novo.Descricao = producao.Descricao + ": " + s.Descricao;
                        novo.IdSetorProducao = s.IdSetor;

                        retorno.Add(novo);
                    }
                }
            }

            return retorno.OrderBy(t => t.Descricao).ToList();
        }

        public override int Delete(TipoFuncionario objDelete)
        {
            return DeleteByPrimaryKey((uint)objDelete.IdTipoFuncionario);
        }

        public override int DeleteByPrimaryKey(uint Key)
        {
            // Verifica se este tipo de funcionário está associado a algum funcionário
            if (CurrentPersistenceObject.ExecuteSqlQueryCount("Select Count(*) From funcionario Where idTipoFunc=" + Key) > 0)
                throw new Exception("Este tipo de funcionário não pode ser excluído pois há um ou mais funcionários associados ao mesmo.");

            return GDAOperations.Delete(new TipoFuncionario { IdTipoFuncionario = (int)Key });
        }
	}
}