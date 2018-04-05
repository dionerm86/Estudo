using System;
using Glass.Data.DAL;

namespace Glass.Data.Model.Calculos
{
    class ClienteDTO : ICliente
    {
        public uint Id { get; set; }
        public bool Revenda { get; set; }
        public bool CobrarAreaMinima { get; set; }

        internal ClienteDTO(uint id)
        {
            Id = id;
            Revenda = ClienteDAO.Instance.IsRevenda(id);
            CobrarAreaMinima = TipoClienteDAO.Instance.CobrarAreaMinima(id);
        }
    }
}
