using System;
using System.Collections.Generic;
using System.Text;
using Glass.Data.Model;
using GDA;
using Glass.Data.EFD;
using Glass.Data.DAL.CTe;
using Glass.Data.Model.Cte;

namespace Glass.Data.DAL
{
    public sealed class AjusteBeneficioIncentivoDAO : BaseDAO<AjusteBeneficioIncentivo, AjusteBeneficioIncentivoDAO>
    {
        //private AjusteBeneficioIncentivoDAO() { }

        private string Sql(ConfigEFD.TipoImpostoEnum? tipoImposto, uint idAjBenInc, string uf, string codigo, 
            string descricao, bool selecionar, out string filtroAdicional)
        {
            StringBuilder sql = new StringBuilder("select ");
            sql.Append(selecionar ? "abi.*" : "count(*)");
            sql.AppendFormat(@"
                from ajuste_beneficio_incentivo abi
                where 1 {0}", FILTRO_ADICIONAL);

            StringBuilder sb = new StringBuilder(@" and date(coalesce(abi.dataInicio, ?data))<=date(?data) 
                and date(coalesce(abi.dataTermino, ?data))>=date(?data)");

            if (idAjBenInc > 0)
                sb.AppendFormat(" and abi.idAjBenInc={0}", idAjBenInc);

            if (!String.IsNullOrEmpty(uf))
                sb.Append(" and abi.uf=?uf");

            if (!String.IsNullOrEmpty(codigo))
                sb.Append(" and abi.codigo like ?codigo");

            if (!String.IsNullOrEmpty(descricao))
                sb.Append(" and abi.descricao like ?descricao");

            if (tipoImposto != null)
                sb.AppendFormat(" and abi.tipoImposto={0}", (int)tipoImposto.Value);

            filtroAdicional = sb.ToString();
            return sql.ToString();
        }

        private GDAParameter[] GetParams(string uf, string codigo, string descricao, DateTime dataBuscar)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(uf))
                lst.Add(new GDAParameter("?uf", uf));

            if (!String.IsNullOrEmpty(codigo))
                lst.Add(new GDAParameter("?codigo", "%" + codigo + "%"));

            if (!String.IsNullOrEmpty(descricao))
                lst.Add(new GDAParameter("?descricao", "%" + descricao + "%"));

            lst.Add(new GDAParameter("?data", dataBuscar));

            return lst.ToArray();
        }

        public IList<AjusteBeneficioIncentivo> GetForSelPopup(int tipoImposto, uint idNf, uint idCte)
        {
            uint idLoja = 0;

            if (idNf > 0)
                idLoja = NotaFiscalDAO.Instance.ObtemIdLoja(idNf);

            else if (idCte > 0)
            {
                bool saida = ConhecimentoTransporteDAO.Instance.ObtemTipoDocumentoCte(idCte) == Glass.Data.Model.Cte.ConhecimentoTransporte.TipoDocumentoCteEnum.Saida;

                var partCte = ParticipanteCteDAO.Instance.GetParticipanteByIdCte(idCte);
                var p = partCte.Find(y => y.IdLoja > 0 &&
                    ((saida && y.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Emitente) ||
                    (!saida && y.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Destinatario)));

                idLoja = p != null ? p.IdLoja.Value : 0;
            }

            if (idLoja == 0)
                return new List<AjusteBeneficioIncentivo>();

            return GetForSelPopup(tipoImposto, LojaDAO.Instance.GetUf(idLoja));
        }

        public IList<AjusteBeneficioIncentivo> GetForSelPopup(int tipoImposto, string uf)
        {
            string filtroAdicional;
            string sql = Sql((ConfigEFD.TipoImpostoEnum)tipoImposto, 0, uf, null, null, true, out filtroAdicional).
                Replace(FILTRO_ADICIONAL, filtroAdicional) + " order by codigo asc";

            return objPersistence.LoadData(sql, GetParams(uf, null, null, DateTime.Now)).ToList();
        }

        public GenericModel[] GetForRegistro1200(uint idLoja)
        {
            string uf = LojaDAO.Instance.GetUf(idLoja);

            string filtroAdicional;
            string sql = Sql(ConfigEFD.TipoImpostoEnum.ICMS, 0, uf, null, null, true, out filtroAdicional).
                Replace(FILTRO_ADICIONAL, filtroAdicional) + " and substr(abi.codigo, 4, 1)='9' order by codigo asc";

            List<AjusteBeneficioIncentivo> itens = objPersistence.LoadData(sql, GetParams(uf, null, null, DateTime.Now));

            List<GenericModel> retorno = new List<GenericModel>();
            foreach (AjusteBeneficioIncentivo i in itens)
                retorno.Add(new GenericModel(i.IdAjBenInc, i.CodigoDescricao));

            return retorno.ToArray();
        }

        public IList<AjusteBeneficioIncentivo> GetList(ConfigEFD.TipoImpostoEnum tipoImposto, string uf, string codigo, string descricao, 
            string sortExpression, int startRow, int pageSize)
        {
            string filtroAdicional;

            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "abi.codigo";
            return LoadDataWithSortExpression(Sql(tipoImposto, 0, uf, codigo, descricao, true, out filtroAdicional), sortExpression, startRow,
                pageSize, false, filtroAdicional, GetParams(uf, codigo, descricao, DateTime.Now));
        }

        public int GetCount(ConfigEFD.TipoImpostoEnum tipoImposto, string uf, string codigo, string descricao)
        {
            string filtroAdicional;
            return GetCountWithInfoPaging(Sql(tipoImposto, 0, uf, codigo, descricao, true, out filtroAdicional), false, filtroAdicional,
                GetParams(uf, codigo, descricao, DateTime.Now));
        }

        public List<AjusteBeneficioIncentivo> GetList(ConfigEFD.TipoImpostoEnum tipoImposto, string uf, DateTime data)
        {
            string filtroAdicional, sql = Sql(tipoImposto, 0, uf, null, null, true, out filtroAdicional);
            return objPersistence.LoadData(sql.Replace(FILTRO_ADICIONAL, filtroAdicional), GetParams(uf, null, null, data));
        }
    }
}
