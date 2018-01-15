using System;

namespace Glass.Data.Helper
{
    [Serializable()]
    public class RetalhoProducaoAuxiliar
    {
        public RetalhoProducaoAuxiliar(uint id, decimal altura, decimal largura, int quantidade, string observacao)
        {
            IdRetalhoProducao = id;
            Altura = altura;
            Largura = largura;
            Quantidade = quantidade;
            Observacao = observacao;
        }

        public uint IdRetalhoProducao { get; set; }

        public uint IdProdPed { get; set; }

        public uint IdProd { get; set; }

        public decimal Altura { get; set; }

        public decimal Largura { get; set; }

        public int Quantidade { get; set; }

        public string Observacao { get; set; }
    }
}
