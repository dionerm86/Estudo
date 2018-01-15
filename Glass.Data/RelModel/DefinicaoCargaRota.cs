using GDA;
using Glass.Data.Helper;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(DefinicaoCargaRotaDAO))]
    public class DefinicaoCargaRota
    {
        #region Propriedades

        [PersistenceProperty("Indice", DirectionParameter.InputOptional)]
        public long Indice { get; set; }

        [PersistenceProperty("IdCliente", DirectionParameter.InputOptional)]
        public uint IdCliente { get; set; }

        private string _nomeCliente;

        [PersistenceProperty("NomeCliente", DirectionParameter.InputOptional)]
        public string NomeCliente { get { return IdCliente + " - " + _nomeCliente; } set { _nomeCliente = value; } }

        [PersistenceProperty("TotalM2", DirectionParameter.InputOptional)]
        public decimal TotalM2 { get; set; }

        [PersistenceProperty("Peso", DirectionParameter.InputOptional)]
        public decimal Peso { get; set; }

        [PersistenceProperty("Pronto", DirectionParameter.InputOptional)]
        public decimal ProntoM2 { get; set; }
 
        [PersistenceProperty("Entregue", DirectionParameter.InputOptional)]
        public decimal EntregueM2 { get; set; }

        [PersistenceProperty("Pendente", DirectionParameter.InputOptional)]
        public decimal PendenteM2 { get; set; }

        [PersistenceProperty("Pedidos", DirectionParameter.InputOptional)]
        public string Pedidos { get; set; }

        [PersistenceProperty("EtiquetaNaoImpressa", DirectionParameter.InputOptional)]
        public decimal EtiquetaNaoImpressaM2 { get { return TotalM2 - ProntoM2 - PendenteM2 - EntregueM2; } }

        #endregion

        #region Propriedades de Suporte

        public bool AlteraLinha
        {
            get
            {
                return UserInfo.GetUserInfo.TipoUsuario != (uint)Utils.TipoFuncionario.Vendedor;
            }
        }

        #endregion
    }
}