using System.Collections.Generic;
using Glass.Data.Model;

namespace Glass.Data.DAL
{
    public sealed class InfoAdicionalNfDAO : BaseDAO<InfoAdicionalNf, InfoAdicionalNfDAO>
    {
        //private InfoAdicionalNfDAO() { }

        public InfoAdicionalNf GetByNf(uint idNf)
        {
            string sql = @"select i.*, nf.modelo, nf.transporte, nf.TipoDocumento, 
                    pcc.codInterno as codInternoContaContabil
                from info_adicional_nf i
                    right join nota_fiscal nf on (i.idNf=nf.idNf)
                    left join plano_conta_contabil pcc on (i.idContaContabil=pcc.idContaContabil)
                where nf.idNf=" + idNf;

            List<InfoAdicionalNf> item = objPersistence.LoadData(sql);

            if (item.Count == 0)
            {
                item.Add(new InfoAdicionalNf());
                item[0].IdNf = idNf;
                item[0].ModeloNf = NotaFiscalDAO.Instance.ObtemValorCampo<string>("modelo", "idNf=" + idNf);
                item[0].IsNfTransporte = NotaFiscalDAO.Instance.IsTransporte(idNf);
            }

            item[0].IdNf = idNf;
            return item[0];
        }

        public override uint Insert(InfoAdicionalNf objInsert)
        {
            objInsert.CstCofins = objInsert.CstPis;
            return base.Insert(objInsert);
        }

        public override int Update(InfoAdicionalNf objUpdate)
        {
            objUpdate.CstCofins = objUpdate.CstPis;
            return base.Update(objUpdate);
        }
    }
}
