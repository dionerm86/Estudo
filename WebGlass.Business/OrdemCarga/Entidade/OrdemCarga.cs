using System;
using System.Collections.Generic;
using System.Linq;

namespace WebGlass.Business.OrdemCarga.Entidade
{
    public class OrdemCarga
    {
        #region Variaveis Locais

        private List<Glass.Data.Model.OrdemCarga> _ocs;

        #endregion

        #region Construtores

        public OrdemCarga()
            : this(new List<Glass.Data.Model.OrdemCarga>())
        {
        }

        internal OrdemCarga(List<Glass.Data.Model.OrdemCarga> ocs)
        {
            _ocs = ocs;
        }

        #endregion

        #region Propiedades

        public List<Glass.Data.Model.OrdemCarga> Ocs
        {
            get { return _ocs; }
            set { _ocs = value; }
        }

        public uint IdCliente
        {
            get
            {
                if (_ocs.Count == 0)
                    return 0;

                return _ocs[0].IdCliente;
            }
        }

        public string IdNomeCliente 
        { 
            get 
            {
                if (_ocs.Count == 0)
                    return "";

                return _ocs[0].IdNomeCliente;
            }
        }

        /// <summary>
        /// Peso total da ordem de carga.
        /// </summary>
        public double PesoTotal { get { return Ocs.Sum(f => f.PedidosTotaisOrdemCarga.Sum(g => f.Peso)); } }

        #endregion
    }
}
