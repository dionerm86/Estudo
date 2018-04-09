using Glass.Data.Model;

namespace Glass.Data.Test.Helper.DescontoAcrescimo
{
    class ClienteDTO : ICliente
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
