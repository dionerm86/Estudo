using Glass.Data.DAL;

namespace Glass.Data.Model.Calculos
{
    class ClienteDTO : ICliente
    {
        public uint Id { get; set; }
        public bool Revenda { get; set; }

        internal ClienteDTO(uint id)
        {
            this.Id = id;
            this.Revenda = ClienteDAO.Instance.IsRevenda(id);
        }
    }
}
