using Glass.Data.DAL;
using Glass.Data.RelModel;

namespace Glass.Data.RelDAL
{
    public sealed class LivroRegistroEntradaDAO : BaseDAO<LivroRegistroEntrada, LivroRegistroEntradaDAO>
    {
        //private LivroRegistroEntradaDAO() { }

        public LivroRegistroEntrada ObterLivroRegistroEntrada(int idLoja)
        {
            string sql = @"SELECT l.NomeFantasia AS Nome, l.Endereco, l.Bairro, c.NomeCidade AS Cidade, c.NomeUf AS Estado,
                            l.InscEst AS InscEstadual, l.CNPJ
	                            FROM loja l
                            LEFT JOIN cidade c ON(c.IdCidade=l.IdCidade)
                            WHERE l.IdLoja=" + idLoja;

            return objPersistence.LoadOneData(sql);
        }
    }
}
