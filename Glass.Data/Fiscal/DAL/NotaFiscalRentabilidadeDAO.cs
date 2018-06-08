using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    /// <summary>
    /// Representa o objeto de acesso a dados da rentabilidade da nota fiscal.
    /// </summary>
    public class NotaFiscalRentabilidadeDAO : BaseDAO<NotaFiscalRentabilidade, NotaFiscalRentabilidadeDAO>
    {
        #region Métodos Públicos

        /// <summary>
        /// Recupera os registros de rentabilidade pela nota fiscal informada.
        /// </summary>
        /// <param name="sessao"></param>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public IList<NotaFiscalRentabilidade> ObterPorPedido(GDA.GDASession sessao, uint idNf)
        {
            return objPersistence.LoadData(sessao, "SELECT * FROM nota_fiscal_rentabilidade WHERE IdNf=?id",
                new GDA.GDAParameter("?id", idNf))
                .Select(f =>
                {
                    f.ExistsInStorage = true;
                    return f;
                }).ToList();
        }

        #endregion
    }
}
