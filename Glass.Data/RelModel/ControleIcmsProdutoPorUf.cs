using System;

namespace Glass.Data.RelModel
{
    [Serializable]
    public class ControleIcmsProdutoPorUf
    {
        public int? TipoCliente { get; set; }
        public string UfOrigem { get; set; }
        public string UfDestino { get; set; }
        public float AliquotaIntraestadual { get; set; }
        public float AliquotaInterestadual { get; set; }
        public float AliquotaInternaDestinatario { get; set; }
    }
}
