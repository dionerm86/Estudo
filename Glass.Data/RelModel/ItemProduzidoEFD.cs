using GDA;
using Glass.Data.RelDAL;
using System;
using System.Collections.Generic;
using Sync.Fiscal.EFD.Entidade;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(ItemProduzidoEFDDAO))]
    public class ItemProduzidoEFD : IItemProduzido
    {
        #region Membros de IItemProduzido

        public DateTime InicioProducao { get; set; }

        public DateTime? FinalProducao { get; set; }

        [PersistenceProperty("NumEtiqueta")]
        public string NumEtiqueta { get; set; }

        [PersistenceProperty("IdProd")]
        public int CodigoProduto { get; set; }

        [PersistenceProperty("QtdeProduzida")]
        public decimal QtdeProduzida { get; set; }

        public IEnumerable<IInsumoConsumido> Insumos { get; set; }

        #endregion

        [PersistenceProperty("IdProdPedProducao")]
        public int IdProdPedProducao { get; set; }

        [PersistenceProperty("DataLeitura")]
        public DateTime DataLeitura { get; set; }

        [PersistenceProperty("Altura")]
        public float Altura { get; set; }

        [PersistenceProperty("Largura")]
        public int Largura { get; set; }

        [PersistenceProperty("InicioProducao")]
        public bool InicioProd { get; set; }

        [PersistenceProperty("FinalProducao")]
        public bool FinalProd { get; set; }

        [PersistenceProperty("Insumo")]
        public bool UsoEnsumo { get; set; }
    }

    public class InsumoConsumidoEFD : IInsumoConsumido
    {
        public int CodigoProduto { get; set; }

        public DateTime DataSaidaEstoque { get; set; }

        public decimal QtdeConsumida { get; set; }        
    }
}
