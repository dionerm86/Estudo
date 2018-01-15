using GDA;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(EntregaPorRotaDAO))]
    public class EntregaPorRota
    {
        #region Propriedades

        [PersistenceProperty("IdLiberacao", DirectionParameter.InputOptional)]
        public uint IdLiberacao { get; set; }

        [PersistenceProperty("NomeCliente", DirectionParameter.InputOptional)]
        public string Cliente { get; set; }

        [PersistenceProperty("NomeCidade", DirectionParameter.InputOptional)]
        public string Cidade { get; set; }

        [PersistenceProperty("Qtde", DirectionParameter.InputOptional)]
        public decimal Qtde { get; set; }

        [PersistenceProperty("Peso", DirectionParameter.InputOptional)]
        public double Peso { get; set; }

        [PersistenceProperty("Valor", DirectionParameter.InputOptional)]
        public decimal Valor { get; set; }

        [PersistenceProperty("IdProdPed", DirectionParameter.InputOptional)]
        public uint IdProdPed { get; set; }

        [PersistenceProperty("IdsLiberacao", DirectionParameter.InputOptional)]
        public string IdsLiberacao { get; set; }

        [PersistenceProperty("IdsPedido", DirectionParameter.InputOptional)]
        public string IdsPedido { get; set; }

        #endregion
    }
}