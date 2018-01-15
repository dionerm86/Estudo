using System.Collections.Generic;
using System.Linq;
using Colosoft;

namespace Glass.PCP.Negocios.Componentes
{
    /// <summary>
    /// Implementação do fluxo de negócio das mesas de corte.
    /// </summary>
    public class MesaCorteFluxo : IMesaCorteFluxo
    {
        #region ArquivoMesaCorte

        /// <summary>
        /// Recupera os descritores dos arquivos de mesa de corte.
        /// </summary>
        /// <returns></returns>
        public IList<Colosoft.IEntityDescriptor> ObtemArquivosMesaCorte()
        {
            return SourceContext.Instance.CreateQuery()
                .From<Glass.Data.Model.ArquivoMesaCorte>()
                .ProcessResultDescriptor<Entidades.ArquivoMesaCorte>()
                .ToList();
        }

        #endregion
    }
}
