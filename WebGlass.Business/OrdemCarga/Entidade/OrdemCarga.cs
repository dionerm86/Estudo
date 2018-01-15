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

        public double PesoTotal
        {
            get
            {
                var pedidos = new List<Glass.Data.Model.Pedido>();

                foreach (var oc in Ocs)
                    pedidos.AddRange(oc.Pedidos);

                var peso = pedidos.Select(p => new { p.IdPedido, p.PesoOC }).Distinct();

                return Math.Round(peso.Sum(s => s.PesoOC), 2);
            }
        }

        #endregion
    }
}
