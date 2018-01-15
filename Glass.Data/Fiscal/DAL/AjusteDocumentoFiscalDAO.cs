using System.Collections.Generic;
using System.Text;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class AjusteDocumentoFiscalDAO : BaseDAO<AjusteDocumentoFiscal, AjusteDocumentoFiscalDAO>
    {
        //private AjusteDocumentoFiscalDAO() { }

        private string Sql(uint idNf, uint idCte, bool selecionar)
        {
            StringBuilder sql = new StringBuilder("select ");

            sql.Append(selecionar ? @"adf.*, p.codInterno as codInternoProd, abi.codigo as codigoAjuste,
                abi.descricao as descricaoAjuste, olf.descricao as descricaoObsLancFiscal" : "count(*)");

            sql.Append(@"
                from ajuste_documento_fiscal adf
                    left join ajuste_beneficio_incentivo abi on (adf.idAjBenInc=abi.idAjBenInc)
                    left join produto p on (adf.idProd=p.idProd)
                    left join obs_lanc_fiscal olf on (adf.idObsLancFiscal=olf.idObsLancFiscal)
                where 1");

            if (idNf > 0)
                sql.AppendFormat(" and adf.idNf={0} and coalesce(adf.idCte, 0)=0", idNf);

            else if (idCte > 0)
                sql.AppendFormat(" and adf.idCte={0} and coalesce(adf.idNf, 0)=0", idCte);

            return sql.ToString();
        }

        public IList<AjusteDocumentoFiscal> ObtemPorNf(uint idNf)
        {
            var itens = objPersistence.LoadData(Sql(idNf, 0, true)).ToList();
            if (itens.Count == 0)
                itens.Add(new AjusteDocumentoFiscal());

            return itens;
        }

        public IList<AjusteDocumentoFiscal> ObtemPorCte(uint idCte)
        {
            var itens = objPersistence.LoadData(Sql(0, idCte, true)).ToList();
            if (itens.Count == 0)
                itens.Add(new AjusteDocumentoFiscal());

            return itens;
        }
    }
}
