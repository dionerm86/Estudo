using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.Helper;
using GDA;
using Glass.Configuracoes;
using System.Linq;

namespace Glass.Data.DAL
{
    public sealed class FilaEmailDAO : BaseDAO<FilaEmail, FilaEmailDAO>
    {
        //private FilaEmailDAO() { }

        #region Busca padrão

        private string Sql(bool selecionar, uint idLoja, string dataCadIni, string dataCadFim, string dataEnvioIni, string dataEnvioFim,
            string assunto, string destinatario, out bool temFiltro)
        {
            var campos = selecionar ? "f.*, coalesce(l.nomeFantasia, l.razaoSocial) as nomeLoja" : "count(*)";
            var where = String.Empty;

            var sql = "select " + campos + @"
                from fila_email f
                    left join loja l on (f.idLoja=l.idLoja)
                where 1";

            if (idLoja > 0)
                where += " and l.idLoja=" + idLoja;
            
            if (!string.IsNullOrEmpty(assunto))
                where += " and assunto like ?assunto";

            if (!string.IsNullOrEmpty(destinatario))
                where += " and emaildestinatario like ?Destinatario";

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

        public IList<FilaEmail> GetList(uint idLoja, string dataEnvioIni, string dataEnvioFim,
            string dataCadIni, string dataCadFim, string assunto, string destinatario, string sortExpression, int startRow, int pageSize)
        {
            bool temFiltro;
            sortExpression = !String.IsNullOrEmpty(sortExpression) ? sortExpression : "f.dataEnvio desc";

            return LoadDataWithSortExpression(Sql(true, idLoja, dataCadIni, dataCadFim, dataEnvioIni, dataEnvioFim, assunto, destinatario, out temFiltro), 
                sortExpression, startRow, pageSize, GetParam(dataCadIni, dataCadFim, dataEnvioIni, dataEnvioFim, assunto, destinatario));
        }

        public int GetCount(uint idLoja, string dataEnvioIni, string dataEnvioFim, string dataCadIni, string dataCadFim, string assunto, string destinatario)
        {
            bool temFiltro;
            var sql = Sql(true, idLoja, dataCadIni, dataCadFim, dataEnvioIni, dataEnvioFim, assunto, destinatario, out temFiltro);

            return GetCountWithInfoPaging(sql, temFiltro, GetParam(dataCadIni, dataCadFim, dataEnvioIni, dataEnvioFim, assunto, destinatario));
        }

        #endregion

        /// <summary>
        /// Obtém o número de tentativas de envio do e-mail.
        /// </summary>
        private int ObterNumeroTentativasEnvio(GDASession session, int idEmail)
        {
            return ObtemValorCampo<int?>("NumTentativas", $"IdEmail = { idEmail }").GetValueOrDefault();
        }

        /// <summary>
        /// Retorna o próximo e-mail que será enviado.
        /// </summary>
        /// <returns></returns>
        public FilaEmail GetNext()
        {
            var sql = $"SELECT * FROM fila_email WHERE NumTentativas < { FilaEmail.MAX_NUMERO_TENTATIVAS } AND DataEnvio IS NULL ORDER BY IdEmail ASC LIMIT 1;";
            var email = objPersistence.LoadData(sql)?.ToList();

            return email?.Count() > 0 ? email[0] : null;
        }

        /// <summary>
        /// Marca o e-mail como último a ser enviado.
        /// Utilizado em caso de erro na fila de e-mail.
        /// </summary>
        public void SetLast(int idEmail)
        {
            var numeroTentativas = ObterNumeroTentativasEnvio(null, idEmail);
            // Incrementa o número de tentativas
            objPersistence.ExecuteCommand($"UPDATE fila_email SET NumTentativas = { numeroTentativas + 1 } WHERE IdEmail = { idEmail };");
        }

        /// <summary>
        /// Indica que o e-mail foi enviado.
        /// </summary>
        /// <param name="idEmail"></param>
        public void IndicaEnvio(uint idEmail)
        {
            string sql = "update fila_email set dataEnvio=now() where idEmail=" + idEmail;
            objPersistence.ExecuteCommand(sql);
        }

        /// <summary>
        /// Verifica pela mensagem se email foi enviado
        /// </summary>
        public bool EmailEnviado(GDASession session, string assunto, string mensagem)
        {
            return ExecuteScalar<bool>(session, "Select Count(*)>0 From fila_email Where assunto=?assunto And mensagem like ?mensagem",
                new GDAParameter("?assunto", assunto), new GDAParameter("?mensagem", "%" + mensagem + "%"));
        }

        public override int Delete(FilaEmail objDelete)
        {
            return DeleteByPrimaryKey(objDelete.IdEmail);
        }

        public override int DeleteByPrimaryKey(uint key)
        {
            //Busca os ids dos anexos
            var idsAnexos = ExecuteMultipleScalar<int>("select IdAnexoEmail from anexo_email where idemail=" + key).ToArray();

            //Apaga da pasta Boletos o boleto do e-mail em questão
            AnexoEmailDAO.Instance.ApagarBoletosAnexos(idsAnexos);

            AnexoEmailDAO.Instance.DeleteByEmail(key);
            return GDAOperations.Delete(new FilaEmail { IdEmail = key });
        }

        private GDAParameter[] GetParam(string dataCadIni, string dataCadFim, string dataEnvioIni, string dataEnvioFim, string assunto, string destinatario)
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

            if (!String.IsNullOrEmpty(assunto))
                lstParam.Add(new GDAParameter("?Assunto", "%" + assunto + "%"));

            if (!String.IsNullOrEmpty(destinatario))
                lstParam.Add(new GDAParameter("?Destinatario", "%" + destinatario + "%"));

            return lstParam.Count > 0 ? lstParam.ToArray() : null;
        }
        
        #region Email para administradores

        private static DateTime? _dataNaoEnviar;

        /// <summary>
        /// Verifica se os e-mails para administradores podem ser enviados.
        /// </summary>
        /// <returns></returns>
        public bool PodeEnviarEmailAdmin()
        {
            // Apenas para empresas de liberação
            if (!PedidoConfig.LiberarPedido || _dataNaoEnviar == DateTime.Now)
                return false;

            if (PCPConfig.EmailSMS.HorariosEnvioEmailSmsAdmin.Count == 1)
            {
                if (ExecuteScalar<bool>("Select Count(*) > 0 From fila_email Where emailAdmin And Date(dataCad)=Date(Now())"))
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
                        if (ExecuteScalar<bool>("Select Count(*) > 0 From fila_email Where emailAdmin And Date(dataCad)=Date(Now()) And Hour(DataCad)=" + d.Hour))
                            return false;

                        deveEnviar = true;
                    }

                if (!deveEnviar)
                    return false;
            }

            var sql = "Select Count(*) From funcionario Where email Is Not Null And idTipoFunc=" + (int)Utils.TipoFuncionario.Administrador;
            return objPersistence.ExecuteSqlQueryCount(sql) > 0;
        }

        /// <summary>
        /// Indica que não será enviado email hoje.
        /// </summary>
        public void MarcaNaoEnviar()
        {
            _dataNaoEnviar = DateTime.Now;
        }

        #endregion
    }
}