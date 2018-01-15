using GDA;
using Glass.Data.RelDAL;
using Glass.Configuracoes;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(MetragemDAO))]
    [PersistenceClass("metragem")]
    public class Metragem
    {
        #region Propriedades

        [PersistenceProperty("IdPedido")]
        public uint IdPedido { get; set; }

        [PersistenceProperty("NumPeca")]
        public string NumPeca { get; set; }

        [PersistenceProperty("Cor")]
        public string Cor { get; set; }

        [PersistenceProperty("Espessura")]
        public float Espessura { get; set; }

        [PersistenceProperty("Altura")]
        public double Altura { get; set; }

        [PersistenceProperty("Largura")]
        public int Largura { get; set; }

        [PersistenceProperty("TotM2")]
        public double TotM2 { get; set; }

        [PersistenceProperty("Obs")]
        public string Obs { get; set; }

        [PersistenceProperty("Criterio")]
        public string Criterio { get; set; }

        #endregion

        #region Propiedades Estendidas

        [PersistenceProperty("IdCliente", DirectionParameter.InputOptional)]
        public uint IdCliente { get; set; }

        [PersistenceProperty("NomeCliente", DirectionParameter.InputOptional)]
        public string NomeCliente { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string Medidas
        {
            get { return PedidoConfig.EmpresaTrabalhaAlturaLargura ? Altura + " x " + Largura : Largura + " x " + Altura; }
        }

        public string IdNomeCliente { get { return IdCliente + " - " + NomeCliente; } }

        #endregion
    }
}