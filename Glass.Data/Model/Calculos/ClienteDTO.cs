using System;
using Glass.Data.DAL;

namespace Glass.Data.Model.Calculos
{
    class ClienteDTO : ICliente
    {
        private readonly Func<uint> idContainer;

        private uint id = 0;
        private bool revenda = false;
        private bool cobrarAreaMinima = false;

        public uint Id
        {
            get
            {
                VerificaAtualizacaoIdCliente();
                return id;
            }
        }

        public bool Revenda
        {
            get
            {
                VerificaAtualizacaoIdCliente();
                return revenda;
            }
            set
            {
                revenda = value;
            }
        }

        public bool CobrarAreaMinima
        {
            get
            {
                VerificaAtualizacaoIdCliente();
                return cobrarAreaMinima;
            }
        }

        internal ClienteDTO(Func<uint> id)
        {
            idContainer = id;
        }

        private void VerificaAtualizacaoIdCliente()
        {
            if (idContainer() != id)
            {
                id = idContainer();

                revenda = id > 0
                    && ClienteDAO.Instance.IsRevenda(id);

                cobrarAreaMinima = id > 0
                    && TipoClienteDAO.Instance.CobrarAreaMinima(id);
            }
        }
    }
}
