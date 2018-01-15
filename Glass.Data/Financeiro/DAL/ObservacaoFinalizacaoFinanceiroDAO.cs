using System;
using System.Collections.Generic;
using System.Text;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class ObservacaoFinalizacaoFinanceiroDAO : BaseDAO<ObservacaoFinalizacaoFinanceiro, ObservacaoFinalizacaoFinanceiroDAO>
    {
        //private ObservacaoFinalizacaoFinanceiroDAO() { }

        private string Sql(uint idPedido, uint idFuncCad, string dataCadIni, string dataCadFim, 
            string motivo, bool selecionar, out string filtroAdicional)
        {
            StringBuilder sql = new StringBuilder("select "), fa = new StringBuilder(), c = new StringBuilder();

            sql.Append(selecionar ? "off.*, f.nome as nomeFuncCad, '$$$' as criterio" : "count(*)");

            sql.AppendFormat(@"
                from observacao_finalizacao_financeiro off
                    inner join funcionario f on (off.idFuncCad=f.idFunc)
                where motivo <> " + (int)ObservacaoFinalizacaoFinanceiro.MotivoEnum.Aberto + " {0}", FILTRO_ADICIONAL);

            if (idPedido > 0)
            {
                fa.AppendFormat(" and off.idPedido={0}", idPedido);
                c.AppendFormat("Pedido: {0}    ", idPedido);
            }

            if (idFuncCad > 0)
            {
                fa.AppendFormat(" and off.idFuncCad={0}", idFuncCad);
                c.AppendFormat("Funcionário: {0}    ", FuncionarioDAO.Instance.GetNome(idFuncCad));
            }

            if (!String.IsNullOrEmpty(dataCadIni))
            {
                fa.Append(" and off.dataCad>=?dataCadIni");
                c.AppendFormat("Data início: {0}    ", dataCadIni);
            }

            if (!String.IsNullOrEmpty(dataCadFim))
            {
                fa.Append(" and off.dataCad<=?dataCadFim");
                c.AppendFormat("Data término: {0}    ", dataCadFim);
            }

            if (!String.IsNullOrEmpty(motivo))
            {
                fa.AppendFormat(" and off.motivo in ({0})", motivo);

                var temp = new ObservacaoFinalizacaoFinanceiro();
                string descrMotivo = "";
                foreach (var m in motivo.Split(','))
                {
                    temp.Motivo = (ObservacaoFinalizacaoFinanceiro.MotivoEnum)Glass.Conversoes.StrParaInt(m);
                    descrMotivo += temp.DescrMotivo + ", ";
                }

                c.AppendFormat("Motivo: {0}    ", descrMotivo.TrimEnd(',', ' '));
            }

            filtroAdicional = fa.ToString();
            return sql.ToString().Replace("$$$", c.ToString());
        }

        private GDAParameter[] GetParams(string dataCadIni, string dataCadFim)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataCadIni))
                lst.Add(new GDAParameter("?dataCadIni", DateTime.Parse(dataCadIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataCadFim))
                lst.Add(new GDAParameter("?dataCadFim", DateTime.Parse(dataCadFim + " 23:59:59")));

            return lst.ToArray();
        }

        public IList<ObservacaoFinalizacaoFinanceiro> ObtemObservacoesFinalizacao(uint idPedido, uint idFuncCad, string dataCadIni, 
            string dataCadFim, string motivo, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "off.dataCad desc";

            string filtroAdicional, sql = Sql(idPedido, idFuncCad, dataCadIni, dataCadFim, motivo, true, out filtroAdicional);

            return LoadDataWithSortExpression(sql, sortExpression, startRow, pageSize, false, 
                filtroAdicional, GetParams(dataCadIni, dataCadFim));
        }

        public int ObtemNumeroObservacoesFinalizacao(uint idPedido, uint idFuncCad, string dataCadIni, string dataCadFim, string motivo)
        {
            string filtroAdicional, sql = Sql(idPedido, idFuncCad, dataCadIni, dataCadFim, motivo, true, out filtroAdicional);

            return GetCountWithInfoPaging(sql, false, filtroAdicional, GetParams(dataCadIni, dataCadFim));
        }

        public IList<ObservacaoFinalizacaoFinanceiro> ObtemParaRelatorio(uint idPedido, uint idFuncCad, string dataCadIni, string dataCadFim, string motivo)
        {
            string filtroAdicional, sql = Sql(idPedido, idFuncCad, dataCadIni, dataCadFim, motivo, true, out filtroAdicional).
                Replace(FILTRO_ADICIONAL, filtroAdicional);

            return objPersistence.LoadData(sql, GetParams(dataCadIni, dataCadFim)).ToList();
        }

        private int GetNumSeq(uint idPedido)
        {
            return ObtemValorCampo<int>("coalesce(max(numSeq), 0) + 1", "idPedido=" + idPedido);
        }

        public uint InsereItem(uint idPedido, string motivo, ObservacaoFinalizacaoFinanceiro.TipoObs tipoObs)
        {
            ObservacaoFinalizacaoFinanceiro novo = new ObservacaoFinalizacaoFinanceiro()
            {
                IdPedido = idPedido,
                Motivo = ObservacaoFinalizacaoFinanceiro.MotivoEnum.Aberto,
                NumSeq = GetNumSeq(idPedido)
            };

            if (tipoObs == ObservacaoFinalizacaoFinanceiro.TipoObs.Finalizacao)
                novo.MotivoErroFinalizarFinanc = motivo;
            else
                novo.MotivoErroConfirmarFinanc = motivo;

            return Insert(novo);
        }

        public void AtualizaItem(uint idPedido, string observacao, ObservacaoFinalizacaoFinanceiro.MotivoEnum motivo)
        {
            var sql = @"
                SELECT * 
                FROM observacao_finalizacao_financeiro
                WHERE idPedido=" + idPedido + @"
                    AND motivo=" + (int)Glass.Data.Model.ObservacaoFinalizacaoFinanceiro.MotivoEnum.Aberto;

            var obsFinalizacaoFinanc = objPersistence.LoadOneData(sql);

            if (obsFinalizacaoFinanc == null)
                throw new Exception("Observação da finalização do financeiro não foi encontrada.");

            obsFinalizacaoFinanc.Observacao = observacao;
            obsFinalizacaoFinanc.Motivo = motivo;
            obsFinalizacaoFinanc.IdFuncCad = UserInfo.GetUserInfo.CodUser;
            obsFinalizacaoFinanc.DataCad = DateTime.Now;

            Update(obsFinalizacaoFinanc);
        }
    }
}
