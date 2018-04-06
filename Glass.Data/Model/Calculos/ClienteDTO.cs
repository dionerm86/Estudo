using System;
using Glass.Data.DAL;

namespace Glass.Data.Model.Calculos
{
    class ClienteDTO : ICliente
    {
        private readonly Func<uint> idContainer;
        private uint id;

        public uint Id
        {
            get
            {
                if (idContainer() != id)
                {
                    id = idContainer();
                }

                return id;
            }
            set
            {
                id = value;

                Revenda = value > 0
                    ? ClienteDAO.Instance.IsRevenda(value)
                    : false;

                CobrarAreaMinima = value > 0
                    ? TipoClienteDAO.Instance.CobrarAreaMinima(value)
                    : false;
            }
        }

        public bool Revenda { get; set; }
        public bool CobrarAreaMinima { get; set; }

        internal ClienteDTO(Func<uint> id)
        {
            idContainer = id;
            Id = id();
        }
    }
}
