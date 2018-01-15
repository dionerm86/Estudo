using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Data.Model;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ConciliacaoBancariaDAO : BaseCadastroDAO<ConciliacaoBancaria, ConciliacaoBancariaDAO>
    {
        //private ConciliacaoBancariaDAO() { }

        private string Sql(uint idConciliacaoBancaria, uint idContaBanco, bool selecionar, out string filtroAdicional)
        {
            StringBuilder sql = new StringBuilder("select "), fa = new StringBuilder();

            sql.AppendFormat(selecionar ? @"cb.*, concat(c.Nome, ' Agência: ', c.Agencia, 
                ' Conta: ', c.Conta) as descrContaBanco" : "count(*)");

            sql.AppendFormat(@"
                from conciliacao_bancaria cb
                    left join conta_banco c on (cb.idContaBanco=c.idContaBanco)
                where 1 {0}", FILTRO_ADICIONAL);

            if (idConciliacaoBancaria > 0)
                fa.AppendFormat(" and cb.idConciliacaoBancaria={0}", idConciliacaoBancaria);

            if (idContaBanco > 0)
                fa.AppendFormat(" and cb.idContaBanco={0}", idContaBanco);

            filtroAdicional = fa.ToString();
            return sql.ToString();
        }

        public ConciliacaoBancaria ObtemElemento(uint idConciliacaoBancaria)
        {
            string filtroAdicional;
            return objPersistence.LoadOneData(Sql(idConciliacaoBancaria, 0, true, out filtroAdicional).Replace(FILTRO_ADICIONAL, filtroAdicional));
        }

        public ConciliacaoBancaria[] ObtemListaConciliacoesBancarias(uint idContaBanco, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "cb.idConciliacaoBancaria desc";

            string filtroAdicional;
            return LoadDataWithSortExpression(Sql(0, idContaBanco, true, out filtroAdicional), sortExpression, startRow, 
                pageSize, false, filtroAdicional).ToArray();
        }

        public int ObtemNumeroConciliacoesBancarias(uint idContaBanco)
        {
            string filtroAdicional;
            return GetCountWithInfoPaging(Sql(0, idContaBanco, true, out filtroAdicional), false, filtroAdicional);
        }

        public uint ObtemContaBanco(uint idConciliacaoBancaria)
        {
            return ObtemValorCampo<uint>("idContaBanco", "idConciliacaoBancaria=" + idConciliacaoBancaria);
        }

        public KeyValuePair<DateTime?, DateTime> ObtemDatasConciliacao(uint idConciliacaoBancaria)
        {
            uint idContaBanco = ObtemContaBanco(idConciliacaoBancaria);
            var dataFim = ObtemValorCampo<DateTime>("dataConciliada", "idConciliacaoBancaria=" + idConciliacaoBancaria);
            var dataIni = ObtemValorCampo<DateTime?>("max(dataConciliada)", "situacao=1 and idContaBanco=" + idContaBanco + 
                " and dataConciliada<?data", new GDAParameter("?data", dataFim.Date));

            if (dataIni.HasValue)
                dataIni = dataIni.Value.AddDays(1);

            return new KeyValuePair<DateTime?, DateTime>(dataIni, dataFim);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idContaBanco"></param>
        /// <param name="dataConciliacao"></param>
        /// <returns></returns>
        public bool IsDataConciliada(uint idContaBanco, DateTime dataConciliacao)
        {
            return IsDataConciliada(null, idContaBanco, dataConciliacao);
        }

        public bool IsDataConciliada(GDASession sessao, uint idContaBanco, DateTime dataConciliacao)
        {
            var dataUltimaConciliacao = ObtemDataUltimaConciliacao(sessao, idContaBanco);

            if (dataUltimaConciliacao == null)
                return false;

            return dataConciliacao.Date <= dataUltimaConciliacao.Value.Date;
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idContaBanco"></param>
        /// <returns></returns>
        public DateTime? ObtemDataUltimaConciliacao(uint idContaBanco)
        {
            return ObtemDataUltimaConciliacao(null, idContaBanco);
        }

        public DateTime? ObtemDataUltimaConciliacao(GDASession sessao, uint idContaBanco)
        {
            return ObtemValorCampo<DateTime?>(sessao, "max(dataConciliada)", "situacao=1 and idContaBanco=" + idContaBanco);
        }

        public void VerificaDataConciliacao(uint idMovBanco)
        {
            VerificaDataConciliacao(null, idMovBanco);
        }

        public void VerificaDataConciliacao(GDASession sessao, uint idMovBanco)
        {
            DateTime dataMov = MovBancoDAO.Instance.ObtemValorCampo<DateTime>(sessao, "dataMov", "idMovBanco=" + idMovBanco);
            uint idContaBanco = MovBancoDAO.Instance.ObtemValorCampo<uint>(sessao, "idContaBanco", "idMovBanco=" + idMovBanco);

            if (idContaBanco == 0 && idMovBanco == 0)
                return;

            VerificaDataConciliacao(sessao, idContaBanco, dataMov);
        }

        /// <summary>
        /// (APAGAR: quando alterar para utilizar transação)
        /// </summary>
        /// <param name="idContaBanco"></param>
        /// <param name="dataVerificar"></param>
        public void VerificaDataConciliacao(uint idContaBanco, DateTime dataVerificar)
        {
            VerificaDataConciliacao(null, idContaBanco, dataVerificar);
        }

        public void VerificaDataConciliacao(GDASession sessao, uint idContaBanco, DateTime dataVerificar)
        {
            if (IsDataConciliada(sessao, idContaBanco, dataVerificar))
            {
                throw new ArgumentException(String.Format("A conta bancária {0} está conciliada na data {1:dd/MM/yyyy}. " +
                    "A última conciliação para esta conta bancária é do dia {2:dd/MM/yyyy}.",
                    ContaBancoDAO.Instance.GetDescricao(sessao, idContaBanco),
                    dataVerificar,
                    ObtemDataUltimaConciliacao(sessao, idContaBanco).Value));
            }
        }

        public void Cancelar(uint idConciliacaoBancaria, string motivo, bool manual)
        {
            var conciliacao = GetElementByPrimaryKey(idConciliacaoBancaria);
            LogCancelamentoDAO.Instance.LogConciliacaoBancaria(conciliacao, motivo, manual);

            objPersistence.ExecuteCommand("update conciliacao_bancaria set situacao=2 where idConciliacaoBancaria=" +
                idConciliacaoBancaria);
        }
    }
}
