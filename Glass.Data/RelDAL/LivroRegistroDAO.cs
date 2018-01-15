using System;
using Glass.Data.DAL;
using Glass.Data.RelModel;
using Glass.Data.Helper;

namespace Glass.Data.RelDAL
{
    public sealed class LivroRegistroDAO : BaseDAO<LivroRegistro, LivroRegistroDAO>
    {
        //private LivroRegistroDAO() { }

        internal string SqlCampoDataEntrada(string alias, bool nf)
        {
            string format = nf ? (!Configuracoes.FiscalConfig.Relatorio.UsarDataCadNotaLivroRegistro ?
                "COALESCE({0}.DataSaidaEnt, {0}.DataEmissao)" :
                "COALESCE({0}.DataSaidaEnt, {0}.DataCad)") :
                // Chamado 13426.
                // "{0}.DataSaidaEnt";
                "COALESCE({0}.DataEntradaSaida, {0}.DataEmissao)";

            return String.Format(format, alias);
        }

        public LivroRegistro ObterDadosLivroRegistro(int idLoja)
        {
            string sql =
                @"SELECT l.NomeFantasia AS Nome, l.Endereco, l.Bairro, l.Cep, c.NomeCidade AS Cidade, c.NomeUf AS Estado,
                    l.InscEst AS InscEstadual, l.CNPJ
                FROM loja l
                    LEFT JOIN cidade c ON(c.IdCidade=l.IdCidade)
                WHERE l.IdLoja=" + idLoja;

            return objPersistence.LoadOneData(sql);
        }
    }
}
