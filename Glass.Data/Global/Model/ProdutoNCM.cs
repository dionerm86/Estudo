using GDA;

namespace Glass.Data.Model
{
    [PersistenceClass("produto_ncm")]
    public class ProdutoNCM : Colosoft.Data.BaseModel
    {
        [PersistenceProperty("IdProd", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Produto), "IdProd")]
        public int IdProd { get; set; }

        [PersistenceProperty("Idloja", PersistenceParameterType.Key)]
        [PersistenceForeignKey(typeof(Loja), "Idloja")]
        public int IdLoja { get; set; }

        [PersistenceProperty("NCM")]
        public string NCM { get; set; }
    }
}
    