using System;
using System.Collections.Generic;
using Glass.Data.Model;
using GDA;
using Glass.Data.Helper;

namespace Glass.Data.DAL
{
    public sealed class MoldeDAO : BaseCadastroDAO<Molde, MoldeDAO>
    {
        //private MoldeDAO() { }

        #region Método de busca padrão

        private string Sql(uint idMolde, uint idPedido, string nomeCliente, bool selecionar)
        {
            string campos = selecionar ? @"m.*, p.EnderecoObra, p.BairroObra, p.CidadeObra, c.Nome as NomeCliente, 
                c.Tel_Cont as TelefoneCliente, p.dataEntrega as dataEntregaPedido, fv.Nome as FuncVend, fc.Nome as FuncCad" : 
                "Count(*)";

            string sql = @"
                select " + campos + @"
                from molde m
                    inner join pedido p on (m.idPedido=p.idPedido)
                    inner join cliente c on (p.idCli=c.id_cli)
                    inner join funcionario fv On (p.idFunc=fv.idFunc)
                    inner join funcionario fc On (m.usuCad=fc.idFunc)
                where 1";

            if (idMolde > 0)
                sql += " and m.idMolde=" + idMolde;
            else if (idPedido > 0)
                sql += " and m.idPedido=" + idPedido;
            else if (!String.IsNullOrEmpty(nomeCliente))
                sql += " and m.nomeCliente like ?nomeCliente";

            return sql;
        }

        private GDAParameter[] GetParams(string nomeCliente)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(nomeCliente))
                lst.Add(new GDAParameter("?nomeCliente", "%" + nomeCliente + "%"));

            return lst.ToArray();
        }

        public IList<Molde> GetList(uint idMolde, uint idOrcamento, string nomeCliente, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "idMolde DESC";
            return LoadDataWithSortExpression(Sql(idMolde, idOrcamento, nomeCliente, true), sortExpression, startRow, pageSize, GetParams(nomeCliente));
        }

        public int GetListCount(uint idMolde, uint idOrcamento, string nomeCliente)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(idMolde, idOrcamento, nomeCliente, false), GetParams(nomeCliente));
        }

        public Molde GetElement(uint idMolde)
        {
            return objPersistence.LoadOneData(Sql(idMolde, 0, null, true));
        }

        #endregion

        #region Métodos sobrescritos

        public override int Update(Molde objUpdate)
        {
            objUpdate.UsuAlt = (int)UserInfo.GetUserInfo.CodUser;
            objUpdate.DataAlt = DateTime.Now;

            return base.Update(objUpdate);
        }

        #endregion
    }
}
