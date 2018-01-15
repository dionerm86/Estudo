using System;
using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class FeriadoDAO : BaseCadastroDAO<Feriado, FeriadoDAO>
    {
        //private FeriadoDAO() { }

        private string SqlList(bool selecionar)
        {
            string campos = selecionar ? "*" : "Count(*)";

            string sql = "Select " + campos + " From feriado";

            return sql;
        }

        public IList<Feriado> GetList(string sortExpression, int startRow, int pageSize)
        {
            if (GetCountReal() == 0)
            {
                var lst = new List<Feriado>();
                lst.Add(new Feriado());
                return lst.ToArray();
            }

            string sort = String.IsNullOrEmpty(sortExpression) ? " Order By mes asc, dia asc" : "";
                        
            return LoadDataWithSortExpression(SqlList(true) + sort, sortExpression, startRow, pageSize, null);
        }

        public int GetCountReal()
        {
            return objPersistence.ExecuteSqlQueryCount(SqlList(false), null);
        }

        public int GetCount()
        {
            int count = objPersistence.ExecuteSqlQueryCount(SqlList(false), null);

            return count == 0 ? 1 : count;
        }
               
        public Feriado GetByIdFeriado(int idFeriado)
        {
            List<Feriado> lst = objPersistence.LoadData("Select * From feriado Where IdFeriado=" + idFeriado);

            return lst.Count > 0 ? lst[0] : null;
        }

        /// <summary>
        /// Verifica se a data passada é um feriado
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool IsFeriado(DateTime data)
        {
            string sql = "Select Count(*) From feriado Where dia=" + data.Day + " And mes=" + data.Month;

            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        /// <summary>
        /// Verifica se a data passada é um dia útil
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool IsDiaUtil(DateTime data)
        {
            return (data.DayOfWeek != DayOfWeek.Saturday || Configuracoes.Geral.ConsiderarSabadoDiaUtil) &&
                (data.DayOfWeek != DayOfWeek.Sunday || Configuracoes.Geral.ConsiderarDomingoDiaUtil) &&
                !FeriadoDAO.Instance.IsFeriado(data);
        }

        /// <summary>
        /// Retorna a data com os dias úteis aplicados
        /// </summary>
        /// <param name="data"></param>
        /// <param name="diasUteis"></param>
        /// <returns></returns>
        public DateTime GetDataDiasUteis(DateTime data, long diasUteis)
        {
            sbyte diasSomar = (sbyte)(diasUteis > 0 ? 1 : -1);
            diasUteis = Math.Abs(diasUteis);

            long contador = 0;

            while (contador < diasUteis)
            {
                data = data.AddDays(diasSomar);

                if (IsDiaUtil(data))
                    contador++;
            }

            return data;
        }
    }
}