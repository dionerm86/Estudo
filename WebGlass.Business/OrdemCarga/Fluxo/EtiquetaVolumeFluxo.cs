using System;
using System.Collections.Generic;

namespace WebGlass.Business.OrdemCarga.Fluxo
{
    /// <summary>
    /// Fluxo de dados da etiqueta do volume
    /// </summary>
    public sealed class EtiquetaVolumeFluxo : BaseFluxo<EtiquetaVolumeFluxo>
    {
        private EtiquetaVolumeFluxo() { }

        #region Metodos

        /// <summary>
        /// Recupera a etiqueta do volume para impressão
        /// </summary>
        /// <param name="idVolume"></param>
        /// <returns></returns>
        public List<Entidade.EtiquetaVolume> GetForImpressao(uint idVolume)
        {
            var volume = Glass.Data.DAL.VolumeDAO.Instance.GetElement(idVolume);

            if (volume == null)
                throw new Exception("Volume não encontrado.");

            return new List<WebGlass.Business.OrdemCarga.Entidade.EtiquetaVolume>() 
            { 
                new Entidade.EtiquetaVolume(volume)
            };
        }

        #endregion
    }
}
