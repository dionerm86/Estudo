using Glass.Data.RelDAL;
using GDA;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(LinhaComponenteValorDAO))]
    public class LinhaComponenteValor
    {
        public string Nome1 { get; set; }
        public string Valor1 { get; set; }
        public string Nome2 { get; set; }
        public string Valor2 { get; set; }
        public string Nome3 { get; set; }
        public string Valor3 { get; set; }
        public string Nome4 { get; set; }
        public string Valor4 { get; set; }        
    }
}
