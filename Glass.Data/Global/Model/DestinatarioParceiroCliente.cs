using GDA;

namespace Glass.Data.Model
{
    [PersistenceClass("destinatario_parceiro_cliente")]
    public class DestinatarioParceiroCliente : Colosoft.Data.BaseModel
    {
        #region Propriedades

        [PersistenceProperty("IDMENSAGEMPARCEIRO", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(MensagemParceiro), "IdMensagemParceiro")]
        public int IdMensagemParceiro { get; set; }

        [PersistenceProperty("ID_CLI", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Cliente), "IdCli")]
        public int IdCli { get; set; }

        [PersistenceProperty("LIDA")]
        public bool Lida { get; set; }

        #endregion

        #region Propriedades Estendidas

        [PersistenceProperty("NOMEDEST", DirectionParameter.InputOptional)]
        public string NomeDest { get; set; }

        #endregion
    }
}
