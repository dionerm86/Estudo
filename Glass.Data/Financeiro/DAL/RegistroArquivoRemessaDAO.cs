using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Sync.Utils;
using Sync.Utils.Boleto.CodigoOcorrencia;
using GDA;

namespace Glass.Data.DAL
{
    public sealed class RegistroArquivoRemessaDAO : BaseDAO<RegistroArquivoRemessa, RegistroArquivoRemessaDAO>
    {
        #region Recupera a lista de registro de importações de uma conta a receber \ recebida

        private string Sql(uint idContaR, string idsContasR)
        {
            string campos = "rar.*, cb.codBanco as CodBanco";

            string sql = @"
            SELECT " + campos + @"
            FROM registro_arquivo_remessa rar
                INNER JOIN contas_receber cr ON (rar.idContaR = cr.idContaR)
                INNER JOIN arquivo_remessa ar ON (rar.idArquivoRemessa = ar.idArquivoRemessa)
                LEFT JOIN conta_banco cb ON (rar.idContaBanco = cb.idContaBanco)
            WHERE ar.situacao=" + (int)ArquivoRemessa.SituacaoEnum.Ativo + @"
                AND ar.tipo=" + (int)ArquivoRemessa.TipoEnum.Retorno;

            if (idContaR > 0)
                sql += " AND rar.idContaR=" + idContaR;
            else if(!string.IsNullOrEmpty(idsContasR))
                sql += " AND rar.idContaR IN(" + idsContasR + ")";

            return sql;
        }

        public IList<Glass.Data.Model.RegistroArquivoRemessa> GetListRegistros(uint idContaR)
        {
            return objPersistence.LoadData(Sql(idContaR, null)).ToList();
        }

        public IList<Glass.Data.Model.RegistroArquivoRemessa> GetListRegistros(string idsContasR)
        {
            return objPersistence.LoadData(Sql(0, idsContasR)).ToList();
        }

        #endregion

        #region Insere os registros

        /// <summary>
        /// Insere os registros do retorno do cnab 400
        /// </summary>
        /// <param name="idArquivoRemessa"></param>
        /// <param name="idContaR"></param>
        /// <param name="idContaBanco"></param>
        /// <param name="d"></param>
        public void InsertRegistroRetornoCnab(GDASession sessao, uint idArquivoRemessa, uint idContaR, uint idContaBanco, DateTime dataCredito, int codigoOcorrencia,
            string nossoNumero, string usoEmpresa, decimal valorPago, decimal juros, decimal jurosMora, string numeroDocumento, int codBanco)
        {
            var reg = new RegistroArquivoRemessa()
            {
                IdArquivoRemessa = idArquivoRemessa,
                IdContaR = idContaR,
                DataOcorrencia = dataCredito,
                CodOcorrencia = codigoOcorrencia,
                NossoNumero = nossoNumero,
                NumeroDocumento = numeroDocumento,
                UsoEmpresa = usoEmpresa,
                ValorRecebido = valorPago,
                Juros = juros,
                Multa = jurosMora,
                IdContaBanco = idContaBanco
            };


            if ((codBanco == (int)CodigoBanco.Sicredi && (codigoOcorrencia == (int)CodOcorrenciaSicredi.ConfirmaçãoRecebimentoInstrucaoProtesto || codigoOcorrencia == (int)CodOcorrenciaSicredi.EntradaTituloCartorio ||
                codigoOcorrencia == (int)CodOcorrenciaSicredi.LiquidacaoCartorio)) ||
                (codBanco == (int)CodigoBanco.BancoBrasil && (codigoOcorrencia == (int)CodigoOcorrenciaBancoBrasil.EntradaCartorio || codigoOcorrencia == (int)CodigoOcorrenciaBancoBrasil.ConfirmacaoProtesto ||
                codigoOcorrencia == (int)CodigoOcorrenciaBancoBrasil.Protestado)))
            {
                reg.Protestado = true;
            }

            if (codBanco == (int)CodigoBanco.Sicredi && (codigoOcorrencia == (int)CodOcorrenciaSicredi.EntradaConfirmada || 
                codigoOcorrencia == (int)CodOcorrenciaSicredi.ConfirmaçãoRecebimentoInstrucaoProtesto || codigoOcorrencia == (int)CodOcorrenciaSicredi.EntradaTituloCartorio))
            {
                reg.DataOcorrencia = DateTime.Now.Date;
            }                

            RegistroArquivoRemessaDAO.Instance.Insert(sessao, reg);
        }

        #endregion
    }
}
