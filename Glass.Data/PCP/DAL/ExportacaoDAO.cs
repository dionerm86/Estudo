using System;
using System.Collections.Generic;
using System.Text;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ExportacaoDAO : BaseDAO<Exportacao, ExportacaoDAO>
    {
        //private ExportacaoDAO() { }

        #region Busca padrão

        private string Sql(uint idExportacao, uint idPedido, int situacao, string dataIni, string dataFim, bool selecionar, 
            out bool temFiltro, out string filtroAdicional)
        {
            temFiltro = false;
            StringBuilder fa = new StringBuilder();

            StringBuilder sql = new StringBuilder("select ");
            sql.AppendFormat(selecionar ? "e.*, {0} as nomeFornec, func.nome as nomeFunc" : "count(*)", FornecedorDAO.Instance.GetNomeFornecedor("f"));
            
            sql.Append(@" from exportacao e
                left join fornecedor f on (e.idFornec=f.idFornec)
                left join funcionario func on (e.idFunc=func.idFunc)
                where 1 ");

            sql.Append(FILTRO_ADICIONAL);

            if (idExportacao > 0)
                fa.AppendFormat(" and e.idExportacao={0}", idExportacao);

            if (idPedido > 0)
                fa.AppendFormat(" and e.idExportacao in (select * from (select idExportacao from pedido_exportacao where idPedido={0}) as temp)", idPedido);

            if (situacao > 0)
                fa.AppendFormat(" and e.idExportacao in (select * from (select idExportacao from pedido_exportacao where situacaoExportacao={0}) as temp)", situacao);

            if (!String.IsNullOrEmpty(dataIni))
                fa.Append(" and e.dataExportacao>=?dataIni");

            if (!String.IsNullOrEmpty(dataFim))
                fa.Append(" and e.dataExportacao<=?dataFim");

            filtroAdicional = fa.ToString();
            return sql.ToString();
        }

        private GDAParameter[] GetParams(string dataIni, string dataFim)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataIni))
                lst.Add(new GDAParameter("?dataIni", DateTime.Parse(dataIni + " 00:00")));

            if (!String.IsNullOrEmpty(dataFim))
                lst.Add(new GDAParameter("?dataFim", DateTime.Parse(dataFim + " 23:59:59")));

            return lst.ToArray();
        }

        public IList<Exportacao> GetList(uint idExportacao, uint idPedido, int situacao, string dataIni, string dataFim, 
            string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "e.dataExportacao desc";

            bool temFiltro;
            string filtroAdicional;

            return LoadDataWithSortExpression(Sql(idExportacao, idPedido, situacao, dataIni, dataFim, true, out temFiltro, out filtroAdicional),
                sortExpression, startRow, pageSize, temFiltro, filtroAdicional, GetParams(dataIni, dataFim));
        }

        public int GetCount(uint idExportacao, uint idPedido, int situacao, string dataIni, string dataFim)
        {
            bool temFiltro;
            string filtroAdicional;

            return GetCountWithInfoPaging(Sql(idExportacao, idPedido, situacao, dataIni, dataFim, true, out temFiltro, out filtroAdicional),
                temFiltro, filtroAdicional, GetParams(dataIni, dataFim));
        }

        public Exportacao GetElement(uint idExportacao)
        {
            bool temFiltro;
            string filtroAdicional;

            string sql = Sql(idExportacao, 0, 0, null, null, true, out temFiltro, out filtroAdicional).
                Replace(FILTRO_ADICIONAL, filtroAdicional);

            List<Exportacao> item = objPersistence.LoadData(sql);
            return item.Count > 0 ? item[0] : null;
        }

        #endregion
    }
}
