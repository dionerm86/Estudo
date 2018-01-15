using GDA;
using Glass.Data.Model;
using Glass.Data.RelDAL;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(GraficoOrcamentosDAO))]
    public class GraficoOrcamentos
    {
        #region Propriedades

        [PersistenceProperty("IDLOJA")]
        public uint IdLoja { get; set; }

        [PersistenceProperty("IDFUNC")]
        public uint IdFunc { get; set; }

        [PersistenceProperty("TOTALVENDA")]
        public decimal TotalVenda { get; set; }

        [PersistenceProperty("NOMELOJA")]
        public string NomeLoja { get; set; }

        private string _nomeVendedor;

        [PersistenceProperty("NOMEVENDEDOR")]
        public string NomeVendedor
        {
            get { return BibliotecaTexto.GetTwoFirstNames(_nomeVendedor); }
            set { _nomeVendedor = value; }
        }

        [PersistenceProperty("DATAVENDA")]
        public string DataVenda { get; set; }

        [PersistenceProperty("SITUACAO")]
        public int Situacao { get; set; }
                
        [PersistenceProperty("CRITERIO")]
        public string Criterio { get; set; }

        #endregion

        #region Propriedades de Suporte

        public string DescrSituacao
        {
            get { return Orcamento.GetDescrSituacao(Situacao); }
        }

        #endregion
    }

    public class GraficoOrcamentosImagem
    {
        public byte[] Buffer { get; set; }
    }
}