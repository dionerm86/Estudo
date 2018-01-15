using GDA;
using Glass.Data.RelDAL;
using Glass.Data.Helper;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(CapacidadeProducaoPedidoDAO)),
    PersistenceClass("capacidade_producao_pedido")]
    public class CapacidadeProducaoPedido
    {
        public uint IdSetor { get; set; }

        public uint IdPedido { get; set; }

        public decimal TotM { get; set; }

        public string Criterio { get; set; }

        public string NomeSetor
        {
            get { return Utils.ObtemSetor(IdSetor).Descricao; }
        }

        public string SiglaSetor
        {
            get { return Utils.ObtemSetor(IdSetor).Sigla; }
        }

        public int NumSeqSetor
        {
            get { return Utils.ObtemSetor(IdSetor).NumeroSequencia; }
        }
    }
}
