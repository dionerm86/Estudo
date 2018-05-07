using Glass.Data.Model;

namespace Glass.Data.Test.Helper.DescontoAcrescimo
{
    class DadosClienteDTO : IDadosCliente
    {
        public bool CobrarAreaMinima
        {
            get { return false; }
        }

        public uint Id
        {
            get { return 1; }
        }

        public bool Revenda
        {
            get { return false; }
        }
    }
}
