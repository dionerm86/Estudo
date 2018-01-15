using System;

namespace Glass.Data.RelModel
{
    [Serializable]
    public class ControleMvaProdutoPorUf
    {
        public string UfOrigem { get; set; }
        public string UfDestino { get; set; }
        public float MvaOriginal { get; set; }
        public float MvaSimples { get; set; }
    }
}
