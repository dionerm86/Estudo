using System;
using Glass.Data.DAL;
using Glass.Comum.Cache;

namespace Glass.Data.Model.Calculos
{
    class ClienteDTO : BaseCalculoDTO, ICliente
    {
        #region Classe privada

        private class DadosCliente
        {
            public bool Revenda;
            public bool CobrarAreaMinima;
        }

        #endregion

        private static readonly CacheMemoria<DadosCliente, uint> cacheDadosCliente;

        private readonly Func<uint> idContainer;
        private uint id;
        private Lazy<DadosCliente> dadosCliente;

        static ClienteDTO()
        {
            cacheDadosCliente = new CacheMemoria<DadosCliente, uint>("dadosCliente");
        }

        internal ClienteDTO(Func<uint> id)
        {
            idContainer = id;
        }

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
                return dadosCliente.Value.Revenda;
            }
            set
            {
                dadosCliente.Value.Revenda = value;
            }
        }

        public bool CobrarAreaMinima
        {
            get
            {
                VerificaAtualizacaoIdCliente();
                return dadosCliente.Value.CobrarAreaMinima;
            }
        }

        private void VerificaAtualizacaoIdCliente()
        {
            if (dadosCliente == null || idContainer() != id)
            {
                id = idContainer();
                dadosCliente = ObterDadosCliente(id);
            }
        }

        private Lazy<DadosCliente> ObterDadosCliente(uint id)
        {
            Func<DadosCliente> recuperarBanco = () =>
                new DadosCliente()
                {
                    Revenda = id > 0 && ClienteDAO.Instance.IsRevenda(id),
                    CobrarAreaMinima = id > 0 && TipoClienteDAO.Instance.CobrarAreaMinima(id)
                };

            return ObterUsandoCache(
                cacheDadosCliente,
                id,
                recuperarBanco
            );
        }
    }
}
