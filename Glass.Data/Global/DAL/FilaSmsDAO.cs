using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using Glass.Configuracoes;

namespace Glass.Data.DAL
{
    public sealed class FilaSmsDAO : BaseDAO<FilaSms, FilaSmsDAO>
    {
        //private FilaSmsDAO() { }

        #region Busca padrão

        private string Sql(bool selecionar, string nomeLoja, string dataEnvioIni, string dataEnvioFim,
            string dataCadIni, string dataCadFim, string destinatario, out bool temFiltro)
        {
            var campos = selecionar ? "f.*" : "count(*)";
            var where = String.Empty;

            var sql = "select " + campos + @"
                from fila_sms f
                where 1";

            if (!string.IsNullOrEmpty(nomeLoja))
                where += " and nomeLoja like ?nomeLoja";

            if (!string.IsNullOrEmpty(destinatario))
                where += " and celcliente like ?Destinatario";

            if (!string.IsNullOrEmpty(dataCadIni))
                where += " And f.datacad>=?dataCadIni";

            if (!string.IsNullOrEmpty(dataCadFim))
                where += " And f.datacad<=?dataCadFim";

            if (!string.IsNullOrEmpty(dataEnvioIni))
                where += " And f.dataenvio>=?dataEnvioIni";

            if (!string.IsNullOrEmpty(dataEnvioFim))
                where += " And f.dataenvio<=?dataEnvioFim";

            temFiltro = !String.IsNullOrEmpty(where);

            return sql + where;
        }

        public IList<FilaSms> GetList(string nomeLoja, string dataEnvioIni, string dataEnvioFim, string dataCadIni, string dataCadFim,
            string destinatario, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "f.dataEnvio desc";

            return LoadDataWithSortExpression(Sql(true, nomeLoja, dataEnvioIni, dataEnvioFim, dataCadIni, dataCadFim, destinatario, out temFiltro),
                sortExpression, startRow, pageSize, GetParam(nomeLoja, dataCadIni, dataCadFim, dataEnvioIni, dataEnvioFim, destinatario));
        }

        public int GetCount(string nomeLoja, string dataEnvioIni, string dataEnvioFim, string dataCadIni, string dataCadFim, string destinatario)
        {
            bool temFiltro;
            var sql = Sql(true, nomeLoja, dataEnvioIni, dataEnvioFim, dataCadIni, dataCadFim, destinatario, out temFiltro);
            
            return GetCountWithInfoPaging(sql, temFiltro, GetParam(nomeLoja, dataCadIni, dataCadFim, dataEnvioIni, dataEnvioFim, destinatario));
        }

        #endregion

        /// <summary>
        /// Verifica se a mensagem já foi enviada com base em parte da mesma
        /// </summary>
        public bool MensagemJaEnviada(string parteMsg)
        {
            return MensagemJaEnviada(null, parteMsg);
        }

        /// <summary>
        /// Verifica se a mensagem já foi enviada com base em parte da mesma
        /// </summary>
        public bool MensagemJaEnviada(GDASession session, string parteMsg)
        {
            return ExecuteScalar<bool>(session, "Select Count(*) > 0 From fila_sms Where mensagem like ?msg", new GDAParameter("?msg", "%" + parteMsg + "%"));
        }

        /// <summary>
        /// Retorna o próximo SMS que será enviado.
        /// </summary>
        /// <returns></returns>
        public FilaSms GetNext(bool apenasAdmin)
        {
            string sql = "select * from fila_sms where dataEnvio is null and numTentativas<=" + FilaSms.MAX_NUMERO_TENTATIVAS;
            if (apenasAdmin) sql += " and smsAdmin=true";
            sql += " order by idSms asc limit 1";

            List<FilaSms> sms = objPersistence.LoadData(sql);
            return sms.Count > 0 ? sms[0] : null;
        }

        /// <summary>
        /// Marca o SMS como último a ser enviado.
        /// Utilizado em caso de erro na fila de SMS.
        /// </summary>
        /// <param name="idSms"></param>
        public void SetLast(uint idSms, string resultDescr)
        {
            // Incrementa o número de tentativas
            objPersistence.ExecuteCommand("update fila_sms set numTentativas=coalesce(numTentativas,0)+1 where idSms=" + idSms);
            if (ObtemValorCampo<int>("numTentativas", "idSms=" + idSms) > FilaSms.MAX_NUMERO_TENTATIVAS)
                return;

            uint id = ExecuteScalar<uint>("select max(idSms)+1 from fila_sms");
            if (id == (idSms + 1))
                return;

            string sql = @"update fila_sms set idSms={0} {1} where idSms=" + idSms + @"; 
                alter table fila_sms auto_increment={0}";

            sql = string.Format(sql, id, resultDescr != string.Empty ? ", descricaoResultado = '" + resultDescr + "'" : string.Empty);

            objPersistence.ExecuteCommand(sql);
        }

        /// <summary>
        /// Indica que o SMS foi enviado.
        /// </summary>
        public void IndicaEnvio(bool enviada, uint idSms, int result, string resultDescr)
        {
            var sql = "update fila_sms set descricaoResultado=?descr, codResultado=" + result;

            if (enviada)
                sql += ", dataEnvio=now()";

            sql += " where idSms=" + idSms;

            resultDescr = resultDescr?.Length > 100 ? resultDescr.Substring(0, 100) : resultDescr;

            objPersistence.ExecuteCommand(sql, new GDAParameter("?descr", resultDescr));
        }

        private GDAParameter[] GetParam(string nomeLoja, string dataCadIni, string dataCadFim, string dataEnvioIni, string dataEnvioFim, string destinatario)
        {
            List<GDAParameter> lstParam = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataCadIni))
                lstParam.Add(new GDAParameter("?dataCadIni", DateTime.Parse(dataCadIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataCadFim))
                lstParam.Add(new GDAParameter("?dataCadFim", DateTime.Parse(dataCadFim + " 23:59:59")));

            if (!String.IsNullOrEmpty(dataEnvioIni))
                lstParam.Add(new GDAParameter("?dataEnvioIni", DateTime.Parse(dataEnvioIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataEnvioFim))
                lstParam.Add(new GDAParameter("?dataEnvioFim", DateTime.Parse(dataEnvioFim + " 23:59:59")));

            if (!String.IsNullOrEmpty(destinatario))
                lstParam.Add(new GDAParameter("?Destinatario", "%" + destinatario + "%"));

            if (!String.IsNullOrEmpty(nomeLoja))
                lstParam.Add(new GDAParameter("?nomeLoja", "%" + nomeLoja + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }

        #region SMS para administradores

        private static DateTime? _dataNaoEnviar;

        /// <summary>
        /// Verifica se os SMS para administradores podem ser enviados.
        /// </summary>
        /// <returns></returns>
        public bool PodeEnviarSmsAdmin()
        {
            // Apenas para empresas de liberação
            if (!PedidoConfig.LiberarPedido || _dataNaoEnviar == DateTime.Now)
                return false;

            if (PCPConfig.EmailSMS.HorariosEnvioEmailSmsAdmin.Count == 1)
            {
                if (ExecuteScalar<bool>("Select Count(*) > 0 From fila_sms Where smsAdmin And Date(dataCad)=Date(Now())"))
                    return false;
            }
            else
            {
                var deveEnviar = false;

                // Define se o email deverá ser enviado ou não, primeiramente verificando se já está na hora de enviá-lo 
                // e depois verificando se algum sms já foi enviado nesta hora.
                foreach (DateTime d in PCPConfig.EmailSMS.HorariosEnvioEmailSmsAdmin)
                    if (d.Hour == DateTime.Now.Hour)
                    {
                        if (ExecuteScalar<bool>("Select Count(*) > 0 From fila_sms Where smsAdmin And Date(dataCad)=Date(Now()) And Hour(DataCad)=" + d.Hour))
                            return false;

                        deveEnviar = true;
                    }

                if (!deveEnviar)
                    return false;
            }

            var sql = "select count(*) from funcionario where telCel is not null and idTipoFunc=" + (int)Utils.TipoFuncionario.Administrador;
            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        /// <summary>
        /// Indica que não será enviado SMS hoje.
        /// </summary>
        public void MarcaNaoEnviar()
        {
            _dataNaoEnviar = DateTime.Now;
        }

        #endregion
    }
}
