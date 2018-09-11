using System;
using System.Collections.Generic;
using System.Linq;
using Glass.Data.Model.Cte;
using Glass.Data.Helper;
using System.Xml;
using GDA;
using Glass.Data.CTeUtils;
using System.IO;
using Glass.Data.Model;
using Glass.Data.EFD;
using Glass.Configuracoes;

namespace Glass.Data.DAL.CTe
{
    public sealed class ConhecimentoTransporteDAO : BaseDAO<ConhecimentoTransporte, ConhecimentoTransporteDAO>
    {
        //private ConhecimentoTransporteDAO() { }

        #region Busca padrão

        private string Sql(uint idCte, int idLoja, int numeroCte, string situacao, uint idCfop, int? formaPagto,
            int tipoEmissao, int? tipoCte, int? tipoServico, string dataEmiIni, string dataEmiFim, uint idTransportador,
            int ordenar, uint tipoDestinatario, uint idDestinatario, uint tipoRecebedor, uint idRecebedor, bool selecionar, bool isOrdenar)
        {
            return Sql(idCte, idLoja, numeroCte, situacao, idCfop, formaPagto, tipoEmissao, tipoCte,
                tipoServico, dataEmiIni, dataEmiFim, idTransportador, ordenar, 0, 0,
                tipoDestinatario, idDestinatario, tipoRecebedor, idRecebedor, selecionar, isOrdenar);
        }

        private string Sql(uint idCte, int idLoja, int numeroCte, string situacao, uint idCfop, int? formaPagto, int tipoEmissao, int? tipoCte,
            int? tipoServico, string dataEmiIni, string dataEmiFim, uint idTransportador, int ordenar, uint tipoRemetente, uint idRemetente,
            uint tipoDestinatario, uint idDestinatario, uint tipoRecebedor, uint idRecebedor, bool selecionar, bool isOrdenar)
        {
            string campos = selecionar ? "c.*, '$$$' as Criterio" : "count(*)";

            string sql = "Select " + campos + @"
                From conhecimento_transporte c " +
                (idLoja > 0 ? " INNER JOIN funcionario as func on c.usucad = func.idfunc AND func.IDLOJA =" + idLoja : "") +
                (idRemetente > 0 ? "INNER JOIN participante_cte pcrm ON(c.IdCte=pcrm.IdCte)" : "") +
                (idDestinatario > 0 ? "INNER JOIN participante_cte pcd ON(c.IdCte=pcd.IdCte)" : "") +
                (idRecebedor > 0 ? "INNER JOIN participante_cte pcr ON(c.IdCte=pcr.IdCte)" : "") +
                " Where 1 ";

            string criterio = "";

            if (idTransportador > 0)
            {
                sql += @" and c.IdCte In (
                    select part.idCte from participante_cte part 
                    where idCte=c.idCte and idTransportador=" + idTransportador + @" or part.idFornec in (
                        select idFornec from fornecedor where idFornec=part.idFornec and cpfCnpj=(
	                        select cpfCnpj from transportador where idTransportador=" + idTransportador + @"
                        )
                    ))";
            }

            if(idRemetente > 0)
            {
                switch (tipoRemetente)
                {
                    case 0:
                        sql += "AND (pcrm.TipoParticipante = 1 AND pcrm.IdLoja=" + idRemetente + ")";
                        break;
                    case 1:
                        sql += "AND (pcrm.TipoParticipante = 1 AND pcrm.IdFornec=" + idRemetente + ")";
                        break;
                    case 2:
                        sql += "AND (pcrm.TipoParticipante = 1 AND pcrm.IdCliente=" + idRemetente + ")";
                        break;
                    case 3:
                        sql += "AND (pcrm.TipoParticipante = 1 AND pcrm.IdTransportador=" + idRemetente + ")";
                        break;
                }
            }

            if(idDestinatario > 0)
            {
                switch (tipoDestinatario)
                {
                    case 0:
                        sql += "AND (pcd.TipoParticipante = 2 AND pcd.IdLoja=" + idDestinatario + ")";
                        break;
                    case 1:
                        sql += "AND (pcd.TipoParticipante = 2 AND pcd.IdFornec=" + idDestinatario + ")";
                        break;
                    case 2:
                        sql += "AND (pcd.TipoParticipante = 2 AND pcd.IdCliente=" + idDestinatario + ")";
                        break;
                    case 3:
                        sql += "AND (pcd.TipoParticipante = 2 AND pcd.IdTransportador=" + idDestinatario + ")";
                        break;
                }
            }

            if (idRecebedor > 0)
            {
                switch (tipoRecebedor)
                {
                    case 0:
                        sql += "AND (pcr.TipoParticipante = 4 AND pcr.IdLoja=" + idRecebedor + ")";
                        break;
                    case 1:
                        sql += "AND (pcr.TipoParticipante = 4 AND pcr.IdFornec=" + idRecebedor + ")";
                        break;
                    case 2:
                        sql += "AND (pcr.TipoParticipante = 4 AND pcr.IdCliente=" + idRecebedor + ")";
                        break;
                    case 3:
                        sql += "AND (pcr.TipoParticipante = 4 AND pcr.IdTransportador=" + idRecebedor + ")";
                        break;
                }
            }

            if (idCte > 0)
                sql += " And c.IDCTE=" + idCte;

            if (numeroCte > 0)
            {
                sql += " And c.NUMEROCTE=" + numeroCte;
                criterio += "Número CTe: " + numeroCte + "    ";
            }

            if (!String.IsNullOrEmpty(situacao))
            {
                sql += " And c.situacao in(" + situacao + ")";
                criterio += "Situação: ";

                string[] s = situacao.Split(',');

                foreach (string item in s)
                {
                    criterio += Enum.Parse(typeof(ConhecimentoTransporte.SituacaoEnum), item) + ", ";
                }

                criterio.Remove(criterio.LastIndexOf(','));
            }

            if (idCfop > 0)
            {
                sql += " And exists (select * from natureza_operacao where idNaturezaOperacao=c.idNaturezaOperacao and idCfop=" + idCfop + ")";
                criterio += "CFOP: " + CfopDAO.Instance.ObtemValorCampo<string>("codInterno", "idCfop=" + idCfop) + "    ";
            }

            if (formaPagto != null && formaPagto < 3)
            {
                sql += " And c.FORMAPAGTO=" + formaPagto;
                criterio += "Forma Pagamento: " + formaPagto;
            }

            if (tipoEmissao > 0)
            {
                sql += " And c.TIPOEMISSAO=" + tipoEmissao;
                criterio += "Tipo Emissão: " + tipoEmissao;
            }

            if (tipoCte != null && tipoCte < 4)
            {
                sql += " And c.TIPOCTE=" + tipoCte;
                criterio += "Tipo Cte: " + tipoCte;
            }

            if (tipoServico != null && tipoServico < 4)
            {
                sql += " And c.TIPOSERVICO=" + tipoServico;
                criterio += "Tipo Serviço: " + tipoServico;
            }

            if (!String.IsNullOrEmpty(dataEmiIni))
            {
                sql += " And coalesce(if(c.tipoDocumentoCte<>" + (int)ConhecimentoTransporte.TipoDocumentoCteEnum.Saida + ", c.dataEntradaSaida, null), c.DATAEMISSAO)>=?dataEmiIni";
                criterio += "Período emissão : " + dataEmiIni + "    ";
            }

            if (!String.IsNullOrEmpty(dataEmiFim))
            {
                sql += " And coalesce(if(c.tipoDocumentoCte<>" + (int)ConhecimentoTransporte.TipoDocumentoCteEnum.Saida + ", c.dataEntradaSaida, null), c.DATAEMISSAO)<=?dataEmiFim";

                if (!String.IsNullOrEmpty(dataEmiIni))
                    criterio += " até " + dataEmiIni + "    ";
                else
                    criterio += "Período emissão: até " + dataEmiFim + "    ";
            }

            if (isOrdenar)
            {
                switch (ordenar)
                {
                    case 0:
                        sql += " order by c.DataEmissao desc";
                        criterio += "Ordenar por: Data de emissão (descresc.)    ";
                        break;
                    case 1:
                        sql += " order by c.DataEmissao asc";
                        criterio += "Ordenar por: Data de emissão (cresc.)    ";
                        break;
                    case 2:
                        sql += " order by c.ValorTotal asc";
                        criterio += "Ordenar por: Valor Total (cresc.)    ";
                        break;
                    case 3:
                        sql += " order by c.ValorTotal desc";
                        criterio += "Ordenar por: Valor Total (descresc.)    ";
                        break;
                }
            }

            return sql.Replace("$$$", criterio);
        }

        private GDAParameter[] GetParams(string dataEmiIni, string dataEmiFim)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!String.IsNullOrEmpty(dataEmiIni))
                lst.Add(new GDAParameter("?dataEmiIni", DateTime.Parse(dataEmiIni + " 00:00:00")));

            if (!String.IsNullOrEmpty(dataEmiFim))
                lst.Add(new GDAParameter("?dataEmiFim", DateTime.Parse(dataEmiFim + " 23:59:59")));

            return lst.ToArray();
        }

        public ConhecimentoTransporte GetElement(uint idCte)
        {
            return GetElement(null, idCte);
        }

        public ConhecimentoTransporte GetElement(GDASession session, uint idCte)
        {
            try
            {
                return objPersistence.LoadOneData(session, Sql(idCte, 0, 0, null, 0, null, 0, null, null, null, null, 0, 0, 0, 0, 0, 0, true, false));
            }
            catch
            {
                return new ConhecimentoTransporte();
            }
        }

        public IList<ConhecimentoTransporte> GetList(int numeroCte, int idLoja, string situacao, uint idCfop, int? formaPagto, int tipoEmissao, int? tipoCte,
            int? tipoServico, string dataEmiIni, string dataEmiFim, uint idTransportador, int ordenar, uint tipoRemetente, uint idRemetente,
            uint tipoDestinatario, uint idDestinatario, uint tipoRecebedor, uint idRecebedor, string sortExpression, int startRow, int pageSize)
        {
            var isOrdenar = true;
            if (!string.IsNullOrEmpty(sortExpression))
                isOrdenar = false;

            return LoadDataWithSortExpression(Sql(0, idLoja, numeroCte, situacao, idCfop, formaPagto, tipoEmissao, tipoCte, tipoServico, dataEmiIni,
                dataEmiFim, idTransportador, ordenar, tipoRemetente, idRemetente, tipoDestinatario, idDestinatario, tipoRecebedor, idRecebedor, true, isOrdenar),
                sortExpression, startRow, pageSize, GetParams(dataEmiIni, dataEmiFim));
        }

        public int GetCount(int numeroCte, int idLoja, string situacao, uint idCfop, int? formaPagto, int tipoEmissao, int? tipoCte,
            int? tipoServico, string dataEmiIni, string dataEmiFim, uint idTransportador, int ordenar, uint tipoRemetente, uint idRemetente,
            uint tipoDestinatario, uint idDestinatario, uint tipoRecebedor, uint idRecebedor)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, idLoja, numeroCte, situacao, idCfop, formaPagto, tipoEmissao, tipoCte, tipoServico,
                dataEmiIni, dataEmiFim, idTransportador, ordenar, tipoRemetente, idRemetente, tipoDestinatario, idDestinatario, tipoRecebedor, idRecebedor, false, true),
                GetParams(dataEmiIni, dataEmiFim));
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, 0, 0, null, 0, null, 0, null, null, null, null, 0, 0, 0, 0, 0, 0, false, false));
        }

        public List<ConhecimentoTransporte> GetForSintegra(uint idLoja, string dataIni, string dataFim, bool canceladas, bool apenasEntradaTerceiros)
        {
            string sql = Sql(0, 0, 0, (int)ConhecimentoTransporte.SituacaoEnum.Autorizado + "," +
                (int)ConhecimentoTransporte.SituacaoEnum.FinalizadoTerceiros + (canceladas ? ", " +
                (int)ConhecimentoTransporte.SituacaoEnum.Cancelado : ""),
                0, null, 0, null, null, dataIni, dataFim, 0, 0, 0, 0, 0, 0, true, false);

            if (apenasEntradaTerceiros)
                sql += " and c.tipoDocumentoCte=" + (int)ConhecimentoTransporte.TipoDocumentoCteEnum.EntradaTerceiros;

            // Filtra pela loja selecionada
            sql += " And exists (select * from participante_cte where idCte=c.idCte and tipoParticipante=if(c.tipoDocumentoCte=" +
                (int)ConhecimentoTransporte.TipoDocumentoCteEnum.Saida + ", " + (int)ParticipanteCte.TipoParticipanteEnum.Emitente + ", " +
                (int)ParticipanteCte.TipoParticipanteEnum.Destinatario + ") and idLoja=" + idLoja + ")";

            return objPersistence.LoadData(sql, GetParams(dataIni, dataFim));
        }

        public ConhecimentoTransporte[] GetForEFD(string idsLojas, DateTime inicio, DateTime fim, string situacoes)
        {
            string dataIni = inicio.ToString("dd/MM/yyyy");
            string dataFim = fim.ToString("dd/MM/yyyy");
            string sql = Sql(0, 0, 0, situacoes, 0, null, 0, null, null, dataIni, dataFim, 0, 0, 0, 0, 0, 0, true, false);

            // Filtra pela loja selecionada
            if (!String.IsNullOrEmpty(idsLojas) && idsLojas != "0")
                sql += " And exists (select * from participante_cte where idCte=c.idCte and tipoParticipante=if(c.tipoDocumentoCte=" + 
                    (int)ConhecimentoTransporte.TipoDocumentoCteEnum.Saida + ", " + (int)ParticipanteCte.TipoParticipanteEnum.Emitente + ", " + 
                    (int)ParticipanteCte.TipoParticipanteEnum.Destinatario + ") and idLoja in (" + idsLojas + "))";

            return objPersistence.LoadData(sql, GetParams(dataIni, dataFim)).ToArray();
        }

        public IList<ConhecimentoTransporte> GetListForRpt(int numeroCte, int idLoja, string situacao, uint idCfop, int? formaPagto, int tipoEmissao, int? tipoCte,
            int? tipoServico, string dataEmiIni, string dataEmiFim, uint idTransportador, int ordenar, uint tipoRemetente, uint idRemetente,
            uint tipoDestinatario, uint idDestinatario, uint tipoRecebedor, uint idRecebedor)
        {
            return objPersistence.LoadData(Sql(0, idLoja, numeroCte, situacao, idCfop, formaPagto, tipoEmissao, tipoCte, tipoServico, dataEmiIni,
                dataEmiFim, idTransportador, ordenar, tipoRemetente, idRemetente, tipoDestinatario, idDestinatario, tipoRecebedor, idRecebedor, true, true),
                GetParams(dataEmiIni, dataEmiFim)).ToList();
        }

        public IList<uint> ObterIdCtePeloIdContaR(uint idContaR, bool apenasAutorizados)
        {
            string sql = @"
                SELECT cte.IdCte from conhecimento_transporte cte
                    Inner Join contas_receber cr ON (cte.IdCte = cr.IdCte)
                 WHERE " + (apenasAutorizados ? "cte.Situacao = " + (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Autorizado : "1") +
                $" AND cr.idContaR = {idContaR} ";

            return ExecuteMultipleScalar<uint>(sql);
        }

        #endregion

        public override uint Insert(ConhecimentoTransporte objInsert)
        {
            return Insert(null, objInsert);
        }

        public override uint Insert(GDASession session, ConhecimentoTransporte objInsert)
        {
            objInsert.Usucad = UserInfo.GetUserInfo.CodUser;
            objInsert.DataCad = DateTime.Now;
            uint idCte = base.Insert(session, objInsert);

            return idCte;
        }        

        /// <summary>
        /// Atualiza tabela do cte com motivo do cancelamento, quando for o caso;
        /// </summary>
        /// <param name="idCte"></param>
        /// <param name="justificativa"></param>
        /// <returns></returns>
        public int UpdateMotivoCanc(uint idCte, string justificativa)
        {
            return objPersistence.ExecuteCommand("Update conhecimento_transporte set MOTIVOCANC=?motivo Where IDCTE=" + idCte,
                new GDAParameter("?motivo", justificativa));
        }

        #region Altera situação

        /// <summary>
        /// Altera situação do CTe
        /// </summary>
        public int AlteraSituacao(uint idCte, ConhecimentoTransporte.SituacaoEnum situacao)
        {
            return AlteraSituacao(null, idCte, situacao);
        }

        /// <summary>
        /// Altera situação do CTe
        /// </summary>
        public int AlteraSituacao(GDASession session, uint idCte, ConhecimentoTransporte.SituacaoEnum situacao)
        {
            string sql = "Update conhecimento_transporte set situacao=" + (int)situacao + " Where idCte=" + idCte;

            return objPersistence.ExecuteCommand(session, sql);
        }

        #endregion

        #region Retorna dados do CTe

        public uint ObtemNumeroCte(uint idCte)
        {
            return ObtemValorCampo<uint>("numeroCte", "idCte=" + idCte);
        }

        /// <summary>
        /// Retorna números de chave de acesso a partir do idCte
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public string ObtemChaveAcesso(uint idCte)
        {
            string sql = "Select CHAVEACESSO From conhecimento_transporte Where idCte=" + idCte;

            object obj = objPersistence.ExecuteScalar(sql);

            if (obj == null || String.IsNullOrEmpty(obj.ToString()))
                return String.Empty;

            return obj.ToString();
        }

        /// <summary>
        /// Obtém motivo de cancelamento do CTe
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public string ObtemMotivoCanc(uint idCte)
        {
            return ObtemValorCampo<string>("motivoCanc", "idCte=" + idCte);
        }

        /// <summary>
        /// Obtém Motivo de inutilização do CTe
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public string ObtemMotivoInut(uint idCte)
        {
            return ObtemValorCampo<string>("motivoInut", "idCte=" + idCte);
        }

        /// <summary>
        /// Retorna forma de emissão da NFe
        /// 1-Normal
        /// 5-Contingência FSDA
        /// 7-Autorização pela SVC-RS
        /// 8-Autorização pela SVC-SP  
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public int ObtemFormaEmissao(uint idCte)
        {
            string sql = "Select TIPOEMISSAO From conhecimento_transporte Where idCte=" + idCte;

            object obj = objPersistence.ExecuteScalar(sql);

            if (obj == null || String.IsNullOrEmpty(obj.ToString()) || obj.ToString() == "0")
                return 1;

            return Glass.Conversoes.StrParaInt(obj.ToString());
        }

        public ConhecimentoTransporte.TipoDocumentoCteEnum ObtemTipoDocumentoCte(uint idCte)
        {
            return ObtemValorCampo<ConhecimentoTransporte.TipoDocumentoCteEnum>("tipoDocumentoCte", "idCte=" + idCte);
        }

        public ConhecimentoTransporte.SituacaoEnum ObtemSituacaoCte(GDASession session, uint idCte)
        {
            return ObtemValorCampo<ConhecimentoTransporte.SituacaoEnum>(session, "Situacao", "idCte=" + idCte);
        }

        public List<uint> ObtemIdCteByNumero(uint numCte)
        {
            return ExecuteMultipleScalar<uint>("SELECT idCte FROM conhecimento_transporte WHERE numeroCte=" + numCte);
        }

        public bool CteCadastrado(GDASession sessao)
        {
            var dataAtual = DateTime.Now.AddSeconds(-15);
            var ultimaInsercao = ExecuteScalar<DateTime>(sessao, @"SELECT MAX(DataCad) FROM conhecimento_transporte cte");

            return ultimaInsercao > dataAtual;
        }

        #endregion

        #region Gera um número de lote

        /// <summary>
        /// Atualiza o Cte passado com um número de lote ainda não utilizado, retornado-o
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public int GetNewNumLote(uint idCte)
        {
            // Verifica se esse CTe já possui um número de lote
            string sql = "Select Coalesce(NUMLOTE, 0) From conhecimento_transporte Where idCte=" + idCte;

            int numLote = Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sql).ToString());

            if (numLote > 0)
                return numLote;

            // Se este CTe não possuir um número de lote, cria um novo número para o mesmo
            sql = "Select Coalesce(Max(NUMLOTE)+1, 1) From conhecimento_transporte Where idCte<>" + idCte;

            numLote = Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sql).ToString());

            objPersistence.ExecuteCommand("Update conhecimento_transporte Set NUMLOTE=" + numLote + " Where idCte=" + idCte);

            return numLote;
        }

        #endregion

        #region Retorna próxima numeração do CTe

        public int GetUltimoNumeroCte(uint idLoja, int serie, int tipoEmissao)
        {
            return GetUltimoNumeroCte(null, idLoja, serie, tipoEmissao);
        }

        /// <summary>
        /// Retorna o próximo número de CTe a ser utilizado
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public int GetUltimoNumeroCte(GDASession sessao, uint idLoja, int serie, int tipoEmissao)
        {
            string sql = null;

            sql = @"select Coalesce(Max(NUMEROCTE) + 1, 1) From conhecimento_transporte con                            
                  inner join participante_cte part ON(con.IDCTE=part.IDCTE)                            
                  Where part.idLoja=" + idLoja + " And con.serie=" + serie +
                  " AND con.TipoDocumentoCte NOT IN(" + (int)ConhecimentoTransporte.TipoDocumentoCteEnum.EntradaTerceiros + ")";

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sessao, sql).ToString());
        }

        #endregion
        
        #region Retorno da emissão do CT-e

        /// <summary>
        /// Lote enviado com sucesso (salva número de recibo na tabela protocolo)
        /// </summary>
        /// <param name="idCte"></param>
        /// <param name="numRecibo"></param>
        public void RetornoEnvioLote(uint idCte, string numRecibo)
        {
            var protocolo = ProtocoloCteDAO.Instance.GetElement(idCte, (int)ProtocoloCte.TipoProtocoloEnum.Autorizacao);
            // Adiciona na tabela protocolo, o número do recibo do lote do cte, para consultá-lo posteriormente
            if (protocolo != null)
            {
                ProtocoloCteDAO.Instance.Update(new ProtocoloCte
                {
                    DataCad = DateTime.Now,
                    IdCte = idCte,
                    NumRecibo = numRecibo,
                    TipoProtocolo = (int)ProtocoloCte.TipoProtocoloEnum.Autorizacao
                });                
            }
            else if(!String.IsNullOrEmpty(numRecibo))
                ProtocoloCteDAO.Instance.Insert(new ProtocoloCte
                {
                    DataCad = DateTime.Now,
                    IdCte = idCte,                    
                    NumRecibo = numRecibo,
                    TipoProtocolo = (int)ProtocoloCte.TipoProtocoloEnum.Autorizacao
                });                

            AlteraSituacao(idCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.ProcessoEmissao);
        }                

        /// <summary>
        /// Retorno da consulta de emissão do CTe
        /// </summary>
        /// <param name="chaveAcesso"></param>
        /// <param name="xmlProt"></param>
        public void RetornoEmissaoCte(uint idCte, XmlNode xmlProt)
        {
            RetornoEmissaoCte(idCte, xmlProt, Utils.GetCteXmlPath);
        }

        /// <summary>
        /// Retorno da consulta de emissão do CTe
        /// </summary>
        /// <param name="chaveAcesso"></param>
        /// <param name="xmlProt"></param>
        /// <param name="ctePath"></param>
        public void RetornoEmissaoCte(uint idCte, XmlNode xmlProt, string ctePath)
        {
            ConhecimentoTransporte cte = GetElement(idCte);

            // Se o CTe já tiver sido autorizado, não faz nada
            if (cte.Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Autorizado)
                return;

            //Código do status da resposta
            string cStat = xmlProt["infProt"]["cStat"].InnerXml;

            // Gera log do ocorrido
            Glass.Data.DAL.CTe.LogCteDAO.Instance.NewLog(cte.IdCte, "Emissão", Glass.Conversoes.StrParaInt(cStat), ConsultaSituacao.CustomizaMensagemRejeicao(cte.IdCte, xmlProt["infProt"]["xMotivo"].InnerXml));

            // Atualiza número do protocolo de uso do CTe
            if ((cStat == "100" || cStat == "150") && xmlProt["infProt"]["nProt"] != null)
                AutorizacaoCTe(cte, xmlProt, ctePath);
            // CTe denegado
            else if (cStat == "301" || cStat == "302" || cStat == "110" || cStat == "205")
            {
                // Salva protocolo de denegação de uso
                if (xmlProt["infProt"]["nProt"] != null)
                    ProtocoloCteDAO.Instance.Update(cte.IdCte, xmlProt["infProt"]["nProt"].InnerXml);

                // Altera situação da NFe para denegada
                AlteraSituacao(cte.IdCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Denegado);
            }
            // Se o código de retorno da emissão for > 105, algum erro ocorreu, altera situação da NF para Falha ao Emitir
            else if (Glass.Conversoes.StrParaInt(cStat) > 105)
                AlteraSituacao(cte.IdCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaEmitir);

            #region Anexa protocolo de autorização ao arquivo XML do CT-e no servidor

            // Anexa protocolo de autorização ao CT-e se código for 100-Autorizado para Uso
            if (cStat == "100" || cStat == "150")
            {
                string path = ctePath + cte.ChaveAcesso + "-cte.xml";
                IncluiProtocoloXML(path, xmlProt);
            }

            #endregion
        }

        /// <summary>
        /// Procedimentos que devem ser realizados ao autorizar o CTe
        /// </summary>
        /// <param name="cte"></param>
        /// <param name="xmlProt"></param>
        private void AutorizacaoCTe(ConhecimentoTransporte cte, XmlNode xmlProt, string ctePath)
        {
            // Salva protocolo de autorização
            ProtocoloCteDAO.Instance.Update(cte.IdCte, xmlProt["infProt"]["nProt"].InnerXml);

            LogCteDAO.Instance.NewLog(cte.IdCte, "Protocolo CT-e", 0, "Protocolo atualizado.");

            // Altera situação do CTe para autorizado
            AlteraSituacao(cte.IdCte, ConhecimentoTransporte.SituacaoEnum.Autorizado);

            LogCteDAO.Instance.NewLog(cte.IdCte, "Situação", 0, "Situação atualizada.");

            #region Gera a conta à receber

            try
            {
                if (cte.GerarContasReceber &&
                    cte.Participantes.Any(f => f.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Emitente &&
                    f.IdLoja.GetValueOrDefault() > 0) &&
                    cte.Participantes.Any(f => f.Tomador && f.IdCliente > 0))
                {
                    /* Chamado 32799.
                     * Gera a conta a receber do CT-e somente se não tiver sido gerada. */
                    if (ContasReceberDAO.Instance.ObtemValorCampo<bool>("COUNT(*) = 0", string.Format("IdCte={0}", cte.IdCte)))
                    {
                        LogCteDAO.Instance.NewLog(cte.IdCte, "Conta a receber", 0, "Iniciando a geração da conta a receber.");

                        var participantes = cte.Participantes
                            .FirstOrDefault(f => f.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Emitente);
                        if (participantes != null)
                        {
                            if (ContasReceberDAO.Instance.Insert(
                                new ContasReceber()
                                {
                                    IdCte = (int)cte.IdCte,
                                    IdLoja = participantes.IdLoja.GetValueOrDefault(),
                                    NumParc = 1,
                                    NumParcMax = 1,
                                    DataVec = cte.CobrancaDuplCte.DataVenc.GetValueOrDefault(),
                                    ValorVec = cte.CobrancaDuplCte.ValorDupl,
                                    Usucad = UserInfo.GetUserInfo.CodUser,
                                    DataCad = DateTime.Now,
                                    IsParcelaCartao = false,
                                    IdContaRCartao = null,
                                    IdCliente = cte.Participantes != null ? cte.Participantes.FirstOrDefault(f => f.Tomador).IdCliente.GetValueOrDefault() : 0,
                                    IdConta = UtilsPlanoConta.GetPlanoPrazo((uint)Pagto.FormaPagto.Prazo),
                                    TipoConta = (byte)ContasReceber.TipoContaEnum.NaoContabil,
                                }) > 0)

                                LogCteDAO.Instance.NewLog(cte.IdCte, "Conta a receber", 0, "Conta(s) a receber gerada(s) com sucesso.");
                        }
                        else
                            LogCteDAO.Instance.NewLog(cte.IdCte, "Conta a receber", 0, "Falha ao gerar conta a receber, não foi possível recuperar os participantes.");
                    }
                }
                else
                    LogCteDAO.Instance.NewLog(cte.IdCte, "Conta a receber", 0, "Não foi gerada, o CT-e não deve gerar contas a receber.");
            }
            catch (Exception ex)
            {
                /* Chamado 22280.
                 * Ajustei manualmente a situação do chamado e criei este log para salvar
                 * no CT-e o erro que ocorreu ao gerar a conta a receber. */
                LogCteDAO.Instance.NewLog(cte.IdCte, "Conta a receber", 0, string.Format("Falha ao gerar contas a receber: {0}.", ex.Message));
            }

            #endregion

            #region Anexa protocolo de autorização ao arquivo XML do CT-e no servidor

            // Anexa protocolo de autorização ao CT-e
            string path = ctePath + cte.ChaveAcesso + "-cte.xml";
            IncluiProtocoloXML(path, xmlProt);

            #endregion
        }

        #endregion

        #region Retorno de inutilização de numeração do CT-e

        /// <summary>
        /// Retorno de inutilização de numeração do CT-e, grava log e altera situação do CT-e
        /// </summary>
        /// <param name="idCte"></param>
        /// <param name="justificativa"></param>
        /// <param name="xmlRetInut"></param>
        public void RetornoInutilizacaoCTe(uint idCte, string justificativa, XmlNode xmlRetInut)
        {
            // Se o xml de retorno for nulo, ocorreu alguma falha no processo
            if (xmlRetInut == null)
            {
                LogCteDAO.Instance.NewLog(idCte, "Inutilização", 1, "Falha ao inutilizar CTe. ");

                ConhecimentoTransporteDAO.Instance.AlteraSituacao(idCte, ConhecimentoTransporte.SituacaoEnum.FalhaInutilizar);

                throw new Exception("Servidor da SEFAZ não respondeu em tempo hábil, tente novamente.");
            }

            int cod = Glass.Conversoes.StrParaInt(xmlRetInut["infInut"]["cStat"].InnerXml);

            // Insere o log de cancelamento deste CTe
            LogCteDAO.Instance.NewLog(idCte, "Inutilização", cod, xmlRetInut["infInut"]["xMotivo"].InnerXml);

            // Se o código de retorno for 102-Inutilização de número homologado, altera situação para inutilizado
            if (cod == 102)
            {
                // Insere protocolo de inutilização no CTe
                var protocoloCteInut = new ProtocoloCte
                {
                    DataCad = DateTime.Now,
                    IdCte = idCte,
                    NumProtocolo = xmlRetInut["infInut"]["nProt"].InnerXml,
                    NumRecibo = "0",
                    TipoProtocolo = (int)ProtocoloCte.TipoProtocoloEnum.Inutilizacao
                };

                ProtocoloCteDAO.Instance.Insert(protocoloCteInut);
                
                AlteraSituacao(idCte, ConhecimentoTransporte.SituacaoEnum.Inutilizado);
            }
            else if (cod == 206 || cod == 256) // CT-e já está inutilizada
                AlteraSituacao(idCte, ConhecimentoTransporte.SituacaoEnum.Inutilizado);
            else if (cod == 218 || cod == 420) // CT-e já está cancelada
                AlteraSituacao(idCte, ConhecimentoTransporte.SituacaoEnum.Cancelado);
            else if (cod == 220) // CT-e já está autorizada há mais de 7 dias
                AlteraSituacao(idCte, ConhecimentoTransporte.SituacaoEnum.Autorizado);
            else if (cod == 110 || cod == 301 || cod == 302 || cod == 205)
                AlteraSituacao(idCte, ConhecimentoTransporte.SituacaoEnum.Denegado);
            // Se o código de retorno for > 105, algum erro ocorreu, altera situação do CTe para Falha ao Inutilizar
            else if (cod > 105)
                AlteraSituacao(idCte, ConhecimentoTransporte.SituacaoEnum.FalhaInutilizar);
        }

        #endregion

        #region Inclusão de protocolo de recebimento do CT-e

        /// <summary>
        /// Inclui o protocolo de recebimento no CT-e.
        /// </summary>
        /// <param name="path">O caminho do arquivo no servidor.</param>
        /// <param name="xmlProt">O XML que será adicionado ao fim do CT-e.</param>
        public void IncluiProtocoloXML(string path, XmlNode xmlProt)
        {
            if (!File.Exists(path))
                return;

            // Carrega o conteúdo do arquivo XML do CTe
            string conteudoArquivoCTe = "";
            using (FileStream arquivoCTe = File.OpenRead(path))
            using (StreamReader textoArquivoCTe = new StreamReader(arquivoCTe))
                conteudoArquivoCTe = textoArquivoCTe.ReadToEnd();

            // Salva o texto do arquivo XML junto com o texto da autorização do CT-e
            conteudoArquivoCTe = conteudoArquivoCTe.Insert(conteudoArquivoCTe.IndexOf("<Signature"), xmlProt.InnerXml);
            using (FileStream arquivoCTe = File.OpenWrite(path))
            using (StreamWriter salvaArquivoCTe = new StreamWriter(arquivoCTe))
            {
                salvaArquivoCTe.Write(conteudoArquivoCTe);
                salvaArquivoCTe.Flush();
            }
        }

        #endregion          

        #region Obtém CTe pela chave de acesso

        /// <summary>
        /// Obtém CTe pela chave de acesso
        /// </summary>
        /// <param name="chaveAcesso"></param>
        /// <returns></returns>
        public ConhecimentoTransporte GetByChaveAcesso(string chaveAcesso)
        {
            var ctes = new GDA.Sql.Query("chaveacesso=?chave")
                .Add("?chave", chaveAcesso)
                .ToList<ConhecimentoTransporte>();
                //objPersistence.LoadData(sql,
                //new GDAParameter[] { new GDAParameter("?chAcesso", chaveAcesso.Replace("CTe", "")) });

            if (ctes.Count == 0)
                throw new Exception("Não há nenhum CTe cadastrado com a chave de acesso informada.");
            else if (ctes.Count > 1)
                throw new Exception("Há mais de um CTe cadastrado com a chave de acesso informada.");

            return ctes[0];
        }

        #endregion

        #region Retorno da consulta do lote do CTe

        public void RetornoConsSitCTe(uint idCte, XmlNode xmlRetConsSit)
        {
            ConhecimentoTransporte cte = GetElement(idCte);

            // Se o CTe já tiver sido autorizado, não faz nada
            if (cte.Situacao == (int)ConhecimentoTransporte.SituacaoEnum.Autorizado)
                return;

            //Código do status da resposta
            string cStat = xmlRetConsSit["cStat"].InnerXml;

            // Gera log do ocorrido
            LogCteDAO.Instance.NewLog(cte.IdCte, "Consulta", Glass.Conversoes.StrParaInt(cStat), xmlRetConsSit["xMotivo"].InnerXml);

            // Atualiza número do protocolo de uso do CTe
            if (cStat == "100" || cStat == "150")
                AutorizacaoCTe(cte, xmlRetConsSit["protCTe"], Utils.GetCteXmlPath);
            else if (cStat == "206" || cStat == "256") // CT-e já está inutilizado
                AlteraSituacao(cte.IdCte, ConhecimentoTransporte.SituacaoEnum.Inutilizado);
            else if (cStat == "218" || cStat == "420") // CT-e já está cancelado
                AlteraSituacao(cte.IdCte, ConhecimentoTransporte.SituacaoEnum.Cancelado);
            else if (cStat == "220") // CT-e já está autorizado há mais de 7 dias
                AlteraSituacao(cte.IdCte, ConhecimentoTransporte.SituacaoEnum.Autorizado);
            // CTe denegado
            else if (cStat == "301" || cStat == "302" || cStat == "110" || cStat == "205")
            {
                // Salva protocolo de denegação de uso
                if (xmlRetConsSit["protCTe"] != null)
                    objPersistence.ExecuteCommand("Update protocolo_cte set NUMPROTOCOLO=?numProt Where IDCTE" + cte.IdCte, 
                    new GDAParameter[] { new GDAParameter("?numProt", xmlRetConsSit["protCTe"]["infProt"]["nProt"].InnerXml) });

                // Altera situação da CTe para denegado
                AlteraSituacao(cte.IdCte, ConhecimentoTransporte.SituacaoEnum.Denegado);
            }
            // Se o código de retorno da emissão for > 105, algum erro ocorreu, altera situação do CTe para Falha ao Emitir
            else if (Convert.ToInt32(cStat) > 105)
                AlteraSituacao(cte.IdCte, ConhecimentoTransporte.SituacaoEnum.FalhaEmitir);
        }

        #endregion

        #region Retorno de cancelamento do CT-e

        /// <summary>
        /// Retorno do cancelamento da CT-e, grava log e altera situação da CT-e
        /// </summary>
        /// <param name="idCTe"></param>
        /// <param name="justificativa"></param>
        /// <param name="xmlRetCanc"></param>
        public string RetornoEvtCancelamentoCTe(uint idCte, string justificativa, XmlNode xmlRetCanc)
        {
            // Se o xml de retorno for nulo, ocorreu alguma falha no processo
            if (xmlRetCanc == null)
            {
                LogCteDAO.Instance.NewLog(idCte, "Cancelamento", 1, "Falha ao cancelar CTe. ");

                AlteraSituacao(idCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaCancelar);

                throw new Exception("Servidor da SEFAZ não respondeu em tempo hábil, tente novamente.");
            }

            try
            {
                // Lê Xml de retorno do envio do lote                
                int statusProcessamento = Glass.Conversoes.StrParaInt(xmlRetCanc["infEvento"]["cStat"].InnerText);
                string respostaProcessamento = xmlRetCanc["infEvento"]["xMotivo"].InnerText;

                // Insere o log de cancelamento deste Cte
                LogCteDAO.Instance.NewLog(idCte, "Cancelamento", statusProcessamento, respostaProcessamento);

                // Se o código de retorno for 135 ou 136-Cancelamento de CT-e homologado, altera situação para cancelada
                // e estorna produtos no estoque fiscal
                if (statusProcessamento == 135 || statusProcessamento == 136)
                {
                    // Insere protocolo de cancelamento no CTe
                    ProtocoloCteDAO.Instance.Insert(new ProtocoloCte
                    {
                        IdCte = idCte,
                        DataCad = DateTime.Now,
                        NumProtocolo = xmlRetCanc["infEvento"]["nProt"].InnerText,
                        TipoProtocolo = (int)Glass.Data.Model.Cte.ProtocoloCte.TipoProtocoloEnum.Cancelamento
                    });

                    objPersistence.ExecuteCommand("DELETE FROM contas_receber WHERE recebida = 0 AND IdCte = " + idCte);

                    AlteraSituacao(idCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Cancelado);
                }
                else if (statusProcessamento == 206 || statusProcessamento == 256) // CT-e já está inutilizado
                    AlteraSituacao(idCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Inutilizado);
                else if (statusProcessamento == 218 || statusProcessamento == 420) // CT-e já está cancelado
                    AlteraSituacao(idCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Cancelado);
                else if (statusProcessamento == 220) // CT-e já está autorizado há mais de 7 dias
                    AlteraSituacao(idCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Autorizado);
                else if (statusProcessamento == 110 || statusProcessamento == 301 || statusProcessamento == 302 || statusProcessamento == 205)
                    AlteraSituacao(idCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Denegado);
                // Altera situação do CTe para Falha ao Cancelar
                else
                    AlteraSituacao(idCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaCancelar);

                if (statusProcessamento == 135 || statusProcessamento == 136)
                    return "Cancelamento efetuado.";
                else
                    return "Falha ao cancelar CTe. " + respostaProcessamento;

            }
            catch
            {
                LogCteDAO.Instance.NewLog(idCte, "Cancelamento", 1, "Falha ao cancelar CTe. ");

                AlteraSituacao(idCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaCancelar);

                throw new Exception("Falha ao processar retorno, tente novamente.");
            }
        }

        #endregion

        #region Gerar XML do CT-e para Inutilização

        /// <summary>
        /// Inutilização de CTe
        /// </summary>
        /// <param name="idCte"></param>
        /// <param name="justificativa"></param>
        /// <returns></returns>
        public XmlDocument InutilizarCTeXml(uint idCte, string justificativa)
        {
            ConhecimentoTransporte cte = GetElement(idCte);

            if (cte.Situacao == (int)ConhecimentoTransporte.SituacaoEnum.Inutilizado)
                throw new Exception("A numeração deste cte já foi inutilizada.");

            // Apenas CTe aberto, não emitido, com falha na inutilização ou em processo de inutilização pode ser inutilizado
            if (cte.Situacao != (int)ConhecimentoTransporte.SituacaoEnum.Aberto && cte.Situacao != (int)ConhecimentoTransporte.SituacaoEnum.FalhaInutilizar &&
                cte.Situacao != (int)ConhecimentoTransporte.SituacaoEnum.FalhaEmitir && cte.Situacao != (int)ConhecimentoTransporte.SituacaoEnum.NaoEmitido &&
                cte.Situacao != (int)ConhecimentoTransporte.SituacaoEnum.ProcessoInutilizacao)
                throw new Exception("Apenas Cte aberto, não emitido ou com falha ao inutilizar pode ser inutilizado.");

            //Se houver uma conta recebida não pode inutilizar o CT-e
            if(ContasReceberDAO.Instance.CTeTemContaRecebida((int)idCte))
                throw new Exception("Não é possível inutilizar este CT-e, pois o mesmo possui uma conta recebida.");

            #region Monta XML

            string cnpj = string.Empty;
            string codUf = string.Empty;
            int ? idLoja = null;

            var participanteEmitente = ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(idCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente);

            if (participanteEmitente == null)
                throw new Exception("O emitente não foi informado.");

            if (participanteEmitente.IdCliente > 0)
            {
                var cliente = ClienteDAO.Instance.GetElement(participanteEmitente.IdCliente.GetValueOrDefault());
                cnpj = cliente.CpfCnpj;
                codUf = cliente.CodIbgeUf;
                idLoja = (int)cliente.IdLoja.GetValueOrDefault();
            }
            else if (participanteEmitente.IdLoja > 0)
            {
                var loja = LojaDAO.Instance.GetElement(participanteEmitente.IdLoja.GetValueOrDefault());
                cnpj = loja.Cnpj;
                codUf = loja.CodUf;
                idLoja = loja.IdLoja;
            }
            else if (participanteEmitente.IdTransportador > 0)
            {
                var transportador = TransportadorDAO.Instance.GetElement(participanteEmitente.IdTransportador.GetValueOrDefault());
                cnpj = transportador.CpfCnpj;
                codUf = transportador.CodIbgeUf;
            }

            if (String.IsNullOrEmpty(codUf))
                throw new Exception("UF do emitente é não pode ser nula");
            
            // Monta o atributo ID, necessário para identificar esse pedido de inutilização            
            string idInut = Formatacoes.TrataStringDocFiscal("ID" + codUf + cnpj + cte.Modelo.PadLeft(2, '0') +
                cte.Serie.ToString().PadLeft(3, '0') + cte.NumeroCte.ToString().PadLeft(9, '0') + cte.NumeroCte.ToString().PadLeft(9, '0'));

            XmlDocument xmlInut = new XmlDocument();
            XmlNode declarationNode = xmlInut.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlInut.AppendChild(declarationNode);

            XmlElement inutCTe = xmlInut.CreateElement("inutCTe");
            inutCTe.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/cte");
            inutCTe.SetAttribute("versao", ConfigCTe.VersaoInutilizacao);
            xmlInut.AppendChild(inutCTe);

            XmlElement infInut = xmlInut.CreateElement("infInut");
            infInut.SetAttribute("Id", idInut);
            inutCTe.AppendChild(infInut);
            ManipulacaoXml.SetNode(xmlInut, infInut, "tpAmb", ((int)ConfigCTe.TipoAmbiente).ToString());
            ManipulacaoXml.SetNode(xmlInut, infInut, "xServ", "INUTILIZAR");
            ManipulacaoXml.SetNode(xmlInut, infInut, "cUF", codUf.ToString());
            ManipulacaoXml.SetNode(xmlInut, infInut, "ano", DateTime.Now.ToString("yy"));
            ManipulacaoXml.SetNode(xmlInut, infInut, "CNPJ", Formatacoes.TrataStringDocFiscal(cnpj).PadLeft(14, '0'));
            ManipulacaoXml.SetNode(xmlInut, infInut, "mod", cte.Modelo);
            ManipulacaoXml.SetNode(xmlInut, infInut, "serie", cte.Serie.ToString());
            ManipulacaoXml.SetNode(xmlInut, infInut, "nCTIni", cte.NumeroCte.ToString());
            ManipulacaoXml.SetNode(xmlInut, infInut, "nCTFin", cte.NumeroCte.ToString());
            ManipulacaoXml.SetNode(xmlInut, infInut, "xJust", Formatacoes.TrataStringDocFiscal(justificativa));

            #endregion

            #region Assina XML

            try
            {
                //// Classe responsável por assinar o xml da NFe
                //AssinaturaDigital AD = new AssinaturaDigital();

                //System.Security.Cryptography.X509Certificates.X509Certificate2 cert = Certificado.BuscaNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false);

                //int resultado = AD.Assinar(ref xmlInut, "infInut", Certificado.GetCertificado(emitente.IdLoja));

                //if (resultado > 0)
                //    throw new Exception(AD.mensagemResultado);


                // Classe responsável por assinar o xml do CTe
                AssinaturaDigital AD = new AssinaturaDigital();

                System.Security.Cryptography.X509Certificates.X509Certificate2 cert = Certificado.GetCertificado((uint)idLoja.GetValueOrDefault((int)UserInfo.GetUserInfo.IdLoja));
                //System.Security.Cryptography.X509Certificates.X509Certificate2 cert = Certificado.BuscaNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false);

                if (DateTime.Now > cert.NotAfter)
                    throw new Exception("O certificado digital cadastrado está vencido, insira um novo certificado para emitir este CTe. Data Venc.: " + cert.GetExpirationDateString());

                int resultado = AD.Assinar(ref xmlInut, "infInut", cert);

                if (resultado > 0)
                    throw new Exception(AD.mensagemResultado);
            }
            catch (Exception ex)
            {
                LogCteDAO.Instance.NewLog(idCte, "Inutilização", 1, "Falha ao inutilizar CTe. " + ex.Message);

                throw new Exception("Falha ao assinar pedido de inutilização." + ex.Message);
            }

            #endregion

            #region Valida XML

            try
            {
                ValidaXML.Validar(xmlInut, ValidaXML.TipoArquivoXml.InutCTe);
            }
            catch (Exception ex)
            {
                throw new Exception("XML inconsistente." + ex.Message);
            }

            #endregion

            ConhecimentoTransporteDAO.Instance.AlteraSituacao(idCte, ConhecimentoTransporte.SituacaoEnum.ProcessoInutilizacao);

            // Atualiza cte informando o motivo da inutilização
            objPersistence.ExecuteCommand("Update conhecimento_transporte set motivoInut=?motivo Where idCte=" + idCte,
                new GDAParameter("?motivo", justificativa));

            return xmlInut;
        }

        #endregion

        #region Gerar XML dao CT-e para cancelamento

        /// <summary>
        /// Cancelamento de CTe
        /// </summary>
        /// <param name="idCte"></param>
        /// <returns></returns>
        public XmlDocument CancelarCTeXmlEvt(string justificativa, Glass.Data.Model.Cte.ConhecimentoTransporte cte)
        {
            var protocolo = ProtocoloCteDAO.Instance.GetElement(cte.IdCte, (int)ProtocoloCte.TipoProtocoloEnum.Autorizacao);
            if (cte.Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Cancelado)
                throw new Exception("Este cte já foi cancelado.");

            // Apenas cte autorizado com falha no cancelamento ou em processo de cancelamento pode ser cancelado
            if (cte.Situacao != (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Autorizado && cte.Situacao
                != (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaCancelar
                && cte.Situacao != (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.ProcessoCancelamento)
                throw new Exception("Apenas CTe autorizado ou com falha no cancelamento pode ser cancelado.");

            // Se o CTe estiver autorizado mas não possuir protocolo de autorização, não pode ser cancelado
            if (cte.Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Autorizado
                && String.IsNullOrEmpty(protocolo.NumProtocolo))
                throw new Exception("Este CTe não pode ser cancelado por não possuir protocolo de autorização.");

            #region Monta XML

            Loja emitente = LojaDAO.Instance.GetElement(
                ParticipanteCteDAO.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)ParticipanteCte.TipoParticipanteEnum.Emitente).IdLoja.Value
                );
            string codUf = emitente.CodUf;

            XmlDocument xmlCanc = new XmlDocument();
            XmlNode declarationNode = xmlCanc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlCanc.AppendChild(declarationNode);

            XmlElement evento = xmlCanc.CreateElement("eventoCTe");

            evento.SetAttribute("versao", "3.00");
            evento.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/cte");
            xmlCanc.AppendChild(evento);

            XmlElement infEvento = xmlCanc.CreateElement("infEvento");

            //Identificador da TAG a ser assinada, a regra de formação 
            //do Id é: 
            //“ID” + tpEvento +  chave da NF-e + nSeqEvento 
            infEvento.SetAttribute("Id", "ID110111" + cte.ChaveAcesso + "1".PadLeft(2, '0'));
            evento.AppendChild(infEvento);

            // Código do órgão de recepção do Evento. Utilizar a Tabela 
            // do IBGE, utilizar 90 para identificar o Ambiente Nacional. 
            XmlElement cOrgao = xmlCanc.CreateElement("cOrgao");
            uint idCidade = LojaDAO.Instance.ObtemValorCampo<uint>("idCidade", "idLoja=" + emitente.IdLoja);
            string codIbgeUf = CidadeDAO.Instance.ObtemValorCampo<string>("codIbgeUf", "idCidade=" + idCidade);
            cOrgao.InnerText = codIbgeUf;
            infEvento.AppendChild(cOrgao);

            // Identificação do Amb
            // 1 - Produção 
            // 2 – Homologação 
            XmlElement tpAmb = xmlCanc.CreateElement("tpAmb");
            tpAmb.InnerText = ((int)ConfigCTe.TipoAmbiente).ToString();
            infEvento.AppendChild(tpAmb);

            //Autor do evento
            XmlElement CNPJ = xmlCanc.CreateElement("CNPJ");
            CNPJ.InnerText = LojaDAO.Instance.ObtemValorCampo<string>
                ("cnpj", "idLoja=" + emitente.IdLoja)
                .Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty); ;
            infEvento.AppendChild(CNPJ);

            XmlElement chNFe = xmlCanc.CreateElement("chCTe");
            chNFe.InnerText = cte.ChaveAcesso;
            infEvento.AppendChild(chNFe);

            XmlElement dhEvento = xmlCanc.CreateElement("dhEvento");
            dhEvento.InnerText = DateTime.Now.AddMinutes(-2).ToString("yyyy-MM-ddTHH:mm:sszzz");
            infEvento.AppendChild(dhEvento);

            //Código do de evento = 110111
            XmlElement tpEvento = xmlCanc.CreateElement("tpEvento");
            tpEvento.InnerText = "110111";
            infEvento.AppendChild(tpEvento);

            XmlElement nSeqEvento = xmlCanc.CreateElement("nSeqEvento");
            nSeqEvento.InnerText = "1";
            infEvento.AppendChild(nSeqEvento);

            XmlElement detEvento = xmlCanc.CreateElement("detEvento");
            detEvento.SetAttribute("versaoEvento", "3.00");
            infEvento.AppendChild(detEvento);

            XmlElement evCancCTe = xmlCanc.CreateElement("evCancCTe");
            detEvento.AppendChild(evCancCTe);

            ManipulacaoXml.SetNode(xmlCanc, evCancCTe, "descEvento", "Cancelamento");
            ManipulacaoXml.SetNode(xmlCanc, evCancCTe, "nProt", protocolo.NumProtocolo);
            ManipulacaoXml.SetNode(xmlCanc, evCancCTe, "xJust", Formatacoes.TrataStringDocFiscal(justificativa));

            infEvento.AppendChild(detEvento);

            #endregion

            #region Assina XML

            try
            {
                // Classe responsável por assinar o xml da NFe
                //AssinaturaDigital AD = new AssinaturaDigital();

                //int resultado = AD.Assinar(ref xmlCanc, "infEvento", Certificado.GetCertificado(idLoja));

                //if (resultado > 0)
                //    throw new Exception(AD.mensagemResultado);

                AssinaturaDigital AD = new AssinaturaDigital();

                System.Security.Cryptography.X509Certificates.X509Certificate2 cert = Certificado.GetCertificado((uint)emitente.IdLoja);
                //System.Security.Cryptography.X509Certificates.X509Certificate2 cert = Certificado.BuscaNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false);

                if (DateTime.Now > cert.NotAfter)
                    throw new Exception("O certificado digital cadastrado está vencido, insira um novo certificado para emitir este CTe. Data Venc.: " + cert.GetExpirationDateString());

                int resultado = AD.Assinar(ref xmlCanc, "infEvento", cert);

                if (resultado > 0)
                    throw new Exception(AD.mensagemResultado);
            }
            catch (Exception ex)
            {
                Glass.Data.DAL.CTe.LogCteDAO.Instance.NewLog(cte.IdCte, "Cancelamento", 1, "Falha ao cancelar CTe. " + ex.Message);

                throw new Exception("Falha ao assinar pedido de cancelamento." + ex.Message);
            }

            #endregion

            #region Valida XML

            try
            {
                ValidaXML.Validar(xmlCanc, ValidaXML.TipoArquivoXml.CancCTe);
            }
            catch (Exception ex)
            {
                throw new Exception("XML inconsistente." + ex.Message);
            }

            #endregion

            ConhecimentoTransporteDAO.Instance.AlteraSituacao(cte.IdCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.ProcessoCancelamento);

            // Atualiza cte informando o motivo do cancelamento
            ConhecimentoTransporteDAO.Instance.UpdateMotivoCanc(cte.IdCte, justificativa);

            return xmlCanc;
        }

        #endregion

        #region Gerar XML da CT-e para emissão

        /// <summary>
        /// Gera o XML do CTe.
        /// </summary>
        /// <param name="idCte"></param>
        /// <param name="preVisualizar"></param>
        /// <returns></returns>
        private XmlDocument GerarXmlCte(uint idCte, bool preVisualizar)
        {
            ConhecimentoTransporte cte = GetElement(idCte);

            // Verifica se Cte pode ser emitido
            if (cte.Situacao != (int)ConhecimentoTransporte.SituacaoEnum.Aberto &&
                cte.Situacao == (int)ConhecimentoTransporte.SituacaoEnum.NaoEmitido &&
                cte.Situacao == (int)ConhecimentoTransporte.SituacaoEnum.FalhaEmitir)
                throw new Exception("Apenas Cte nas situações: Aberta, Não Emitida e Falha ao emitir podem ser emitidos.");

            /* Chamado 39720. */
            if (cte.TipoDocumentoCte == (int)ConhecimentoTransporte.TipoDocumentoCteEnum.Saida && cte.GerarContasReceber)
            {
                var cobrancaDuplicataCTe = CobrancaDuplCteDAO.Instance.GetElement(idCte);

                if (cobrancaDuplicataCTe == null || cobrancaDuplicataCTe.ValorDupl == 0 || !cobrancaDuplicataCTe.DataVenc.HasValue)
                    throw new Exception("Para gerar a conta a receber do CTe é necessário informar o valor da duplicata e a data de vencimento da mesma.");
            }

            var possuiChaveAcessoInformada = ExecuteScalar<bool>($@"SELECT COUNT(*)>0 FROM chave_acesso_cte cac
                        WHERE cac.IdCte={idCte}");

            if (cte.TipoServico != (int)ConhecimentoTransporte.TipoServicoEnum.RedespachoIntermediario && (NotaFiscalCteDAO.Instance.GetCount(cte.IdCte) == 0 && !possuiChaveAcessoInformada))
                throw new Exception($"As informações dos documentos transportados pelo CT-e são obrigatórias para o tipo de serviço: {cte.TipoServicoString}. Informe pelo menos uma Nota Fiscal.");

            #region Gera XML

            XmlDocument doc = new XmlDocument();
            //XmlNode declarationNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            //doc.AppendChild(declarationNode);

            XmlElement CTe = doc.CreateElement("CTe");
            CTe.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/cte");
            doc.AppendChild(CTe);

            var participantes = ParticipanteCteDAO.Instance.GetParticipanteByIdCte(idCte);
            var participanteEmitente = participantes.Where(f => f.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Emitente).First();
            var loja = LojaDAO.Instance.GetElement(participanteEmitente.IdLoja.Value);
            var cidadeEmitente = CidadeDAO.Instance.GetElementByPrimaryKey(loja.IdCidade.GetValueOrDefault());

            cte.DataEmissao = DateTime.Now;
            cte.ChaveAcesso = ChaveDeAcesso(cidadeEmitente.CodIbgeUf, cte.DataEmissao.ToString("yyMM"), loja.Cnpj, Glass.Data.CTeUtils.ConfigCTe.Modelo,
                cte.Serie.ToString().PadLeft(3, '0'), cte.NumeroCte.ToString(), cte.TipoEmissao.ToString(), cte.CodAleatorio);

            GDAOperations.Update(cte, "ChaveAcesso, DataEmissao");

            XmlElement infCte = doc.CreateElement("infCte");
            infCte.SetAttribute("versao", ConfigCTe.VersaoCte);
            infCte.SetAttribute("Id", "CTe" + cte.ChaveAcesso);            
            CTe.AppendChild(infCte);

            #region Informações do CTe

            var idCidadeLoja = loja.IdCidade != null ? loja.IdCidade.ToString() : "0";
            if(idCidadeLoja == "0")
                throw new Exception("Emitente não possui cidade em seu cadastro.");
            var cidadeLoja = CidadeDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idCidadeLoja));

            try
            {
                // Identificação do CTe
                XmlElement ide = doc.CreateElement("ide");

                ManipulacaoXml.SetNode(doc, ide, "cUF", cidadeLoja.CodIbgeUf);
                ManipulacaoXml.SetNode(doc, ide, "cCT", cte.CodAleatorio.PadLeft(8, '0'));
                var cfop = CfopDAO.Instance.GetCfop(cte.IdCfop);
                ManipulacaoXml.SetNode(doc, ide, "CFOP", cfop.CodInterno.PadLeft(4, '0'));
                ManipulacaoXml.SetNode(doc, ide, "natOp", Formatacoes.TrataStringDocFiscal(cfop.CodInterno + "-" + cfop.Descricao));
                ManipulacaoXml.SetNode(doc, ide, "mod", cte.Modelo);
                                                   
                ManipulacaoXml.SetNode(doc, ide, "serie", Glass.Conversoes.StrParaInt(cte.Serie).ToString());
                ManipulacaoXml.SetNode(doc, ide, "nCT", cte.NumeroCte.ToString());
                var dataEmissao = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:sszzz");
                ManipulacaoXml.SetNode(doc, ide, "dhEmi", dataEmissao);

                ManipulacaoXml.SetNode(doc, ide, "tpImp", "1");

                if(FiscalConfig.ConhecimentoTransporte.ContingenciaCTe == DataSources.TipoContingenciaCTe.SVC)
                    ManipulacaoXml.SetNode(doc, ide, "tpEmis", cte.TipoEmissao.ToString());
                else
                    ManipulacaoXml.SetNode(doc, ide, "tpEmis", cte.TipoEmissao.ToString());

                ManipulacaoXml.SetNode(doc, ide, "cDV", cte.ChaveAcesso[43].ToString());
                ManipulacaoXml.SetNode(doc, ide, "tpAmb", ((int)ConfigCTe.TipoAmbiente).ToString()); // 1-Produção 2-Homologação
                ManipulacaoXml.SetNode(doc, ide, "tpCTe", cte.TipoCte.ToString());
                ManipulacaoXml.SetNode(doc, ide, "procEmi", "0"); // Emissão de CT-e com aplicativo do contribuinte                                
                ManipulacaoXml.SetNode(doc, ide, "verProc", ConfigCTe.VersaoCte);

                ManipulacaoXml.SetNode(doc, ide, "cMunEnv", cidadeLoja.CodUfMunicipio);
                ManipulacaoXml.SetNode(doc, ide, "xMunEnv", Formatacoes.TrataStringDocFiscal(cidadeLoja.NomeCidade));
                ManipulacaoXml.SetNode(doc, ide, "UFEnv", cidadeLoja.NomeUf);

                ManipulacaoXml.SetNode(doc, ide, "modal", "01");
                ManipulacaoXml.SetNode(doc, ide, "tpServ", cte.TipoServico.ToString());

                var cidadeMunIni = CidadeDAO.Instance.GetElementByPrimaryKey(cte.IdCidadeInicio);
                ManipulacaoXml.SetNode(doc, ide, "cMunIni", cidadeMunIni.CodUfMunicipio);
                ManipulacaoXml.SetNode(doc, ide, "xMunIni", cidadeMunIni.NomeCidade);
                ManipulacaoXml.SetNode(doc, ide, "UFIni", cidadeMunIni.NomeUf);

                var cidadeMunFim = CidadeDAO.Instance.GetElementByPrimaryKey(cte.IdCidadeFim);
                ManipulacaoXml.SetNode(doc, ide, "cMunFim", cidadeMunFim.CodUfMunicipio);
                ManipulacaoXml.SetNode(doc, ide, "xMunFim", cidadeMunFim.NomeCidade);
                ManipulacaoXml.SetNode(doc, ide, "UFFim", cidadeMunFim.NomeUf);

                ManipulacaoXml.SetNode(doc, ide, "retira", cte.Retirada ? "1" : "0");
                if (!string.IsNullOrEmpty(cte.DetalhesRetirada))
                    ManipulacaoXml.SetNode(doc, ide, "xDetRetira", cte.DetalhesRetirada);

                var participanteTomador = participantes.Where(f => f.Tomador).FirstOrDefault();

                Cliente clienteTomador = participanteTomador.IdCliente > 0 ? ClienteDAO.Instance.GetElementByPrimaryKey((int)participanteTomador.IdCliente) : null;
                Fornecedor fornecedorTomador = participanteTomador.IdFornec > 0 ? FornecedorDAO.Instance.GetElementByPrimaryKey((int)participanteTomador.IdFornec) : null;
                Loja lojaTomador = participanteTomador.IdLoja > 0 ? LojaDAO.Instance.GetElementByPrimaryKey((int)participanteTomador.IdLoja) : null;
                var pj = clienteTomador != null ? clienteTomador.TipoPessoa.ToUpper() == "J" : fornecedorTomador != null ? fornecedorTomador.TipoPessoa.ToUpper() == "J" : loja != null;
                var produtorRural = clienteTomador != null ? clienteTomador.ProdutorRural : fornecedorTomador != null ? fornecedorTomador.ProdutorRural : false;
                var inscricaoEstadualTomador = clienteTomador != null ? clienteTomador.RgEscinst : fornecedorTomador != null ? fornecedorTomador.RgInscEst : lojaTomador != null ? lojaTomador.InscEst : string.Empty;
                Cidade cidadeFornec;
                var indicadorIEDestinatario = NotaFiscalDAO.Instance.ObterIndicadorIE(null, cte, clienteTomador, fornecedorTomador, out cidadeFornec);
                
                if (indicadorIEDestinatario == null && loja != null)
                {
                    if (string.IsNullOrEmpty(loja.InscEst) || loja.InscEst.ToLower().Contains("isento"))
                        indicadorIEDestinatario = IndicadorIEDestinatario.ContribuinteIsento;
                    else if (Validacoes.ValidaIE(loja.Uf, loja.InscEst))
                        indicadorIEDestinatario = IndicadorIEDestinatario.ContribuinteICMS;
                }

                if (indicadorIEDestinatario != null)
                    ManipulacaoXml.SetNode(doc, ide, "indIEToma", ((int)indicadorIEDestinatario.Value).ToString());

                XmlElement toma3 = doc.CreateElement("toma3");

                var tipoTomador = participanteTomador.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Remetente ? "0" :
                    (participanteTomador.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Expedidor ? "1" :
                    (participanteTomador.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Recebedor ? "2" : "3"));
                ManipulacaoXml.SetNode(doc, toma3, "toma", tipoTomador);
                /*
                if (indicadorIEDestinatario == IndicadorIEDestinatario.ContribuinteICMS)
                {
                    if (pj || produtorRural)
                    {
                        if (string.IsNullOrEmpty(inscricaoEstadualTomador))
                            throw new Exception("Informe a inscrição estadual do cliente.");

                        ManipulacaoXml.SetNode(doc, toma3, "IE", Formatacoes.TrataStringDocFiscal(inscricaoEstadualTomador.ToUpper()));
                    }
                    else
                        ManipulacaoXml.SetNode(doc, toma3, "IE", string.Empty);
                }
                else if (indicadorIEDestinatario == IndicadorIEDestinatario.ContribuinteIsento && inscricaoEstadualTomador != null && inscricaoEstadualTomador.ToLower() == "isento" && (pj || produtorRural))
                    ManipulacaoXml.SetNode(doc, toma3, "IE", string.Empty);
                else if (indicadorIEDestinatario == IndicadorIEDestinatario.NaoContribuinte && !string.IsNullOrEmpty(inscricaoEstadualTomador) && (pj || produtorRural))
                    ManipulacaoXml.SetNode(doc, toma3, "IE", Formatacoes.TrataStringDocFiscal(inscricaoEstadualTomador.ToUpper()));*/

                ide.AppendChild(toma3);

                DateTime? dataContingencia = null;

                if (dataContingencia == null)
                    dataContingencia = DateTime.Now;

                //ManipulacaoXml.SetNode(doc, ide, "dhCont", dataContingencia.Value.ToString("yyyy-MM-ddTHH:mm:ss"));
                //ManipulacaoXml.SetNode(doc, ide, "xJust", "bla bla bla bla bla bla bla bla bla bla bla");                               

                infCte.AppendChild(ide);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir informação do CTe no XML.", ex);
            }

            #endregion

            #region Dados complementares do CTe para fins operacionais ou comerciais

            try
            {
                if (!string.IsNullOrEmpty(cte.InformAdicionais))
                {
                    XmlElement compl = doc.CreateElement("compl");
                    ManipulacaoXml.SetNode(doc, compl, "xObs", Formatacoes.TrataTextoDocFiscal(cte.InformAdicionais));
                    infCte.AppendChild(compl);
                }
            }
            catch (Exception ex)
            {

                throw new Exception("Falha ao inserir informação do CTe no XML.", ex);;
            }

            #endregion

            #region Informações do Emitente

            try
            {
                // Emitente                

                XmlElement emit = doc.CreateElement("emit");
                ManipulacaoXml.SetNode(doc, emit, "CNPJ", Formatacoes.TrataStringDocFiscal(loja.Cnpj).PadLeft(14, '0'));
                ManipulacaoXml.SetNode(doc, emit, "IE", Formatacoes.TrataStringDocFiscal(loja.InscEst.ToUpper()));
                if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Homologacao)
                    ManipulacaoXml.SetNode(doc, emit, "xNome", "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                else
                    ManipulacaoXml.SetNode(doc, emit, "xNome", Formatacoes.TrataStringDocFiscal(loja.RazaoSocial));
                //ManipulacaoXml.SetNode(doc, emit, "xFant", Formatacoes.TrataStringDocFiscal(loja.NomeFantasia));
                XmlElement enderEmit = doc.CreateElement("enderEmit");
                ManipulacaoXml.SetNode(doc, enderEmit, "xLgr", Formatacoes.TrataStringDocFiscal(loja.Endereco));
                ManipulacaoXml.SetNode(doc, enderEmit, "nro", Formatacoes.TrataStringDocFiscal(loja.Numero));

                if (!String.IsNullOrEmpty(Formatacoes.TrataStringDocFiscal(loja.Compl)))
                    ManipulacaoXml.SetNode(doc, enderEmit, "xCpl", Formatacoes.TrataStringDocFiscal(loja.Compl));

                ManipulacaoXml.SetNode(doc, enderEmit, "xBairro", Formatacoes.TrataStringDocFiscal(loja.Bairro));
                ManipulacaoXml.SetNode(doc, enderEmit, "cMun", cidadeLoja.CodUfMunicipio);
                ManipulacaoXml.SetNode(doc, enderEmit, "xMun", Formatacoes.TrataStringDocFiscal(cidadeLoja.NomeCidade));
                if (!string.IsNullOrEmpty(loja.Cep))
                    ManipulacaoXml.SetNode(doc, enderEmit, "CEP", Formatacoes.TrataStringDocFiscal(loja.Cep));
                ManipulacaoXml.SetNode(doc, enderEmit, "UF", cidadeLoja.NomeUf);
                if (!string.IsNullOrEmpty(loja.Telefone))
                    ManipulacaoXml.SetNode(doc, enderEmit, "fone", Formatacoes.TrataStringDocFiscal(loja.Telefone, true));

                emit.AppendChild(enderEmit);

                infCte.AppendChild(emit);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir dados do emitente no XML.", ex);
            }

            #endregion

            #region Informações do Remetente

            try
            {
                var participanteRemetente = participantes.Where(f => f.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Remetente).First();

                XmlElement rem = doc.CreateElement("rem");

                if (participanteRemetente.IdLoja > 0)
                {
                    var lojaRemetente = LojaDAO.Instance.GetElement(participanteRemetente.IdLoja.Value);

                    var idCidadeRemetente = lojaRemetente.IdCidade != null ? lojaRemetente.IdCidade.ToString() : "0";
                    if (idCidadeRemetente == "0")
                        throw new Exception("Remetente não possui cidade em seu cadastro.");
                    var cidadeRemetente = CidadeDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idCidadeRemetente));                   

                    if (string.IsNullOrEmpty(lojaRemetente.Cnpj))
                        throw new Exception("Remetente não possui CNPJ cadastrado");
                    ManipulacaoXml.SetNode(doc, rem, "CNPJ", Formatacoes.TrataStringDocFiscal(lojaRemetente.Cnpj).PadLeft(14, '0'));

                    if (string.IsNullOrEmpty(lojaRemetente.InscEst))
                        throw new Exception("Remetente não possui inscrição estadual cadastrada");
                    ManipulacaoXml.SetNode(doc, rem, "IE", Formatacoes.TrataStringDocFiscal(lojaRemetente.InscEst.ToUpper()));

                    if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Homologacao)
                        ManipulacaoXml.SetNode(doc, rem, "xNome", "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                    else
                        ManipulacaoXml.SetNode(doc, rem, "xNome", Formatacoes.TrataStringDocFiscal(lojaRemetente.RazaoSocial));
                    
                    XmlElement enderReme = doc.CreateElement("enderReme");

                    if (string.IsNullOrEmpty(lojaRemetente.Endereco))
                        throw new Exception("Remetente não possui endereço cadastrado");
                    ManipulacaoXml.SetNode(doc, enderReme, "xLgr", Formatacoes.TrataStringDocFiscal(lojaRemetente.Endereco));

                    if (string.IsNullOrEmpty(lojaRemetente.Numero))
                        throw new Exception("Remetente não possui número cadastrado");
                    ManipulacaoXml.SetNode(doc, enderReme, "nro", Formatacoes.TrataStringDocFiscal(lojaRemetente.Numero));

                    if (!String.IsNullOrEmpty(Formatacoes.TrataStringDocFiscal(lojaRemetente.Compl)))
                        ManipulacaoXml.SetNode(doc, enderReme, "xCpl", Formatacoes.TrataStringDocFiscal(lojaRemetente.Compl));

                    if (string.IsNullOrEmpty(lojaRemetente.Bairro))
                        throw new Exception("Remetente não possui bairro cadastrado");
                    ManipulacaoXml.SetNode(doc, enderReme, "xBairro", Formatacoes.TrataStringDocFiscal(lojaRemetente.Bairro));
                                        
                    ManipulacaoXml.SetNode(doc, enderReme, "cMun", cidadeRemetente.CodUfMunicipio);

                    ManipulacaoXml.SetNode(doc, enderReme, "xMun", Formatacoes.TrataStringDocFiscal(cidadeRemetente.NomeCidade));

                    if (!string.IsNullOrEmpty(loja.Cep))
                        ManipulacaoXml.SetNode(doc, enderReme, "CEP", Formatacoes.TrataStringDocFiscal(lojaRemetente.Cep));

                    if (string.IsNullOrEmpty(lojaRemetente.Uf))
                        throw new Exception("Remetente não possui UF cadastrado");
                    ManipulacaoXml.SetNode(doc, enderReme, "UF", cidadeRemetente.NomeUf);

                    rem.AppendChild(enderReme);
                }
                else if (participanteRemetente.IdCliente > 0)
                {                    
                    var remetente = ClienteDAO.Instance.GetElement(participanteRemetente.IdCliente.Value);
                    var pj = remetente.TipoPessoa.ToUpper() == "J";

                    var idCidadeRemetente = remetente.IdCidade != null ? remetente.IdCidade.ToString() : "0";

                    if (idCidadeRemetente == "0")
                        throw new Exception("Remetente não possui cidade em seu cadastro.");
                    var cidadeRemetente = CidadeDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idCidadeRemetente));                    

                    var numDocumento = Formatacoes.TrataStringDocFiscal(remetente.CpfCnpj);

                    if(numDocumento.Length == 11)
                        ManipulacaoXml.SetNode(doc, rem, "CPF", Formatacoes.TrataStringDocFiscal(remetente.CpfCnpj).PadLeft(11, '0'));
                    else if(numDocumento.Length == 14)
                        ManipulacaoXml.SetNode(doc, rem, "CNPJ", Formatacoes.TrataStringDocFiscal(remetente.CpfCnpj).PadLeft(14, '0'));

                    if(string.IsNullOrEmpty(remetente.RgEscinst) || remetente.RgEscinst.ToLower().Contains("isento"))
                    {
                        ManipulacaoXml.SetNode(doc, rem, "IE", "ISENTO");
                    }
                    else if (pj || remetente.ProdutorRural)
                    {
                        if (String.IsNullOrEmpty(remetente.RgEscinst))
                            throw new Exception("Informe a inscrição estadual do cliente.");
                        ManipulacaoXml.SetNode(doc, rem, "IE", Formatacoes.TrataStringDocFiscal(remetente.RgEscinst));
                    }

                    if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Homologacao)
                        ManipulacaoXml.SetNode(doc, rem, "xNome", "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                    else
                        ManipulacaoXml.SetNode(doc, rem, "xNome", Formatacoes.TrataStringDocFiscal(remetente.Nome));
                    
                    XmlElement enderReme = doc.CreateElement("enderReme");

                    ManipulacaoXml.SetNode(doc, enderReme, "xLgr", Formatacoes.TrataStringDocFiscal(remetente.Endereco));

                    ManipulacaoXml.SetNode(doc, enderReme, "nro", Formatacoes.TrataStringDocFiscal(remetente.Numero));

                    if (!String.IsNullOrEmpty(Formatacoes.TrataStringDocFiscal(remetente.Compl)))
                        ManipulacaoXml.SetNode(doc, enderReme, "xCpl", Formatacoes.TrataStringDocFiscal(remetente.Compl));

                    ManipulacaoXml.SetNode(doc, enderReme, "xBairro", Formatacoes.TrataStringDocFiscal(remetente.Bairro));

                    ManipulacaoXml.SetNode(doc, enderReme, "cMun", cidadeRemetente.CodUfMunicipio);

                    ManipulacaoXml.SetNode(doc, enderReme, "xMun", Formatacoes.TrataStringDocFiscal(cidadeRemetente.NomeCidade));

                    if (!string.IsNullOrEmpty(loja.Cep))
                        ManipulacaoXml.SetNode(doc, enderReme, "CEP", Formatacoes.TrataStringDocFiscal(remetente.Cep));

                    ManipulacaoXml.SetNode(doc, enderReme, "UF", cidadeRemetente.NomeUf);

                    rem.AppendChild(enderReme);
                }
                else if (participanteRemetente.IdFornec > 0)
                {
                    var remetente = FornecedorDAO.Instance.GetElement(participanteRemetente.IdFornec.Value);

                    var pj = remetente.TipoPessoa.ToUpper() == "J";

                    var idCidadeRemetente = remetente.IdCidade != null ? remetente.IdCidade.ToString() : "0";

                    if (idCidadeRemetente == "0")
                        throw new Exception("Remetente não possui cidade em seu cadastro.");
                    var cidadeRemetente = CidadeDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idCidadeRemetente));                    

                    var numDocumento = Formatacoes.TrataStringDocFiscal(remetente.CpfCnpj);

                    if (numDocumento.Length == 11)
                        ManipulacaoXml.SetNode(doc, rem, "CPF", Formatacoes.TrataStringDocFiscal(remetente.CpfCnpj).PadLeft(11, '0'));
                    else if (numDocumento.Length == 14)
                        ManipulacaoXml.SetNode(doc, rem, "CNPJ", Formatacoes.TrataStringDocFiscal(remetente.CpfCnpj).PadLeft(14, '0'));

                    if (string.IsNullOrEmpty(remetente.RgInscEst) || remetente.RgInscEst.ToLower().Contains("isento"))
                    {
                        ManipulacaoXml.SetNode(doc, rem, "IE", "ISENTO");
                    }
                    else if (pj || remetente.ProdutorRural)
                    {
                        if (String.IsNullOrEmpty(remetente.RgInscEst))
                            throw new Exception("Informe a inscrição estadual do cliente.");
                        ManipulacaoXml.SetNode(doc, rem, "IE", Formatacoes.TrataStringDocFiscal(remetente.RgInscEst));
                    }

                    if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Homologacao)
                        ManipulacaoXml.SetNode(doc, rem, "xNome", "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                    else
                        ManipulacaoXml.SetNode(doc, rem, "xNome", Formatacoes.TrataStringDocFiscal(remetente.Razaosocial));
                    
                    XmlElement enderReme = doc.CreateElement("enderReme");

                    ManipulacaoXml.SetNode(doc, enderReme, "xLgr", Formatacoes.TrataStringDocFiscal(remetente.Endereco));

                    ManipulacaoXml.SetNode(doc, enderReme, "nro", Formatacoes.TrataStringDocFiscal(remetente.Numero));

                    if (!String.IsNullOrEmpty(Formatacoes.TrataStringDocFiscal(remetente.Compl)))
                        ManipulacaoXml.SetNode(doc, enderReme, "xCpl", Formatacoes.TrataStringDocFiscal(remetente.Compl));

                    ManipulacaoXml.SetNode(doc, enderReme, "xBairro", Formatacoes.TrataStringDocFiscal(remetente.Bairro));

                    ManipulacaoXml.SetNode(doc, enderReme, "cMun", cidadeRemetente.CodUfMunicipio);

                    ManipulacaoXml.SetNode(doc, enderReme, "xMun", Formatacoes.TrataStringDocFiscal(cidadeRemetente.NomeCidade));

                    if (!string.IsNullOrEmpty(loja.Cep))
                        ManipulacaoXml.SetNode(doc, enderReme, "CEP", Formatacoes.TrataStringDocFiscal(remetente.Cep));

                    ManipulacaoXml.SetNode(doc, enderReme, "UF", cidadeRemetente.NomeUf);

                    rem.AppendChild(enderReme);
                }
                else if (participanteRemetente.IdTransportador > 0)
                {
                    var remetente = TransportadorDAO.Instance.GetElement(participanteRemetente.IdTransportador.Value);

                    var idCidadeRemetente = remetente.IdCidade != null ? remetente.IdCidade.ToString() : "0";

                    if (idCidadeRemetente == "0")
                        throw new Exception("Remetente não possui cidade em seu cadastro.");

                    var cidadeRemetente = CidadeDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idCidadeRemetente));

                    if (string.IsNullOrEmpty(remetente.CpfCnpj))
                        throw new Exception("Remetente não possui cpf ou cnpj cadastrado");

                    var numDocumento = Formatacoes.TrataStringDocFiscal(remetente.CpfCnpj);

                    if (numDocumento.Length == 11)
                        ManipulacaoXml.SetNode(doc, rem, "CPF", Formatacoes.TrataStringDocFiscal(remetente.CpfCnpj).PadLeft(11, '0'));
                    else if (numDocumento.Length == 14)
                        ManipulacaoXml.SetNode(doc, rem, "CNPJ", Formatacoes.TrataStringDocFiscal(remetente.CpfCnpj).PadLeft(14, '0'));

                    if (string.IsNullOrEmpty(remetente.InscEst))
                        throw new Exception("Remetente não possui inscrição estadual cadastrada");

                    ManipulacaoXml.SetNode(doc, rem, "IE", remetente.InscEst);

                    if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Homologacao)
                        ManipulacaoXml.SetNode(doc, rem, "xNome", "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                    else
                    {
                        if (string.IsNullOrEmpty(remetente.Nome))
                            throw new Exception("Remetente não possui nome cadastrado");
                        ManipulacaoXml.SetNode(doc, rem, "xNome", Formatacoes.TrataStringDocFiscal(remetente.Nome));
                    }
                    
                    XmlElement enderReme = doc.CreateElement("enderReme");

                    if (string.IsNullOrEmpty(remetente.Endereco))
                        throw new Exception("Remetente não possui endereço cadastrado");
                    ManipulacaoXml.SetNode(doc, enderReme, "xLgr", Formatacoes.TrataStringDocFiscal(remetente.Endereco));

                    if (string.IsNullOrEmpty(remetente.Numero))
                        throw new Exception("Remetente não possui número cadastrado");
                    ManipulacaoXml.SetNode(doc, enderReme, "nro", Formatacoes.TrataStringDocFiscal(remetente.Numero));                    

                    if (string.IsNullOrEmpty(remetente.Bairro))
                        throw new Exception("Remetente não possui bairro cadastrado");
                    ManipulacaoXml.SetNode(doc, enderReme, "xBairro", Formatacoes.TrataStringDocFiscal(remetente.Bairro));

                    if (string.IsNullOrEmpty(cidadeRemetente.CodUfMunicipio))
                        throw new Exception("Remetente não possui código do município cadastrado");
                    ManipulacaoXml.SetNode(doc, enderReme, "cMun", cidadeRemetente.CodUfMunicipio);

                    if (string.IsNullOrEmpty(cidadeRemetente.NomeCidade))
                        throw new Exception("Remetente não possui nome cidade cadastrado");
                    ManipulacaoXml.SetNode(doc, enderReme, "xMun", Formatacoes.TrataStringDocFiscal(cidadeRemetente.NomeCidade));

                    if (!string.IsNullOrEmpty(loja.Cep))
                        ManipulacaoXml.SetNode(doc, enderReme, "CEP", Formatacoes.TrataStringDocFiscal(remetente.Cep));

                    if (string.IsNullOrEmpty(cidadeRemetente.NomeUf))
                        throw new Exception("Remetente não possui UF cadastrado");
                    ManipulacaoXml.SetNode(doc, enderReme, "UF", cidadeRemetente.NomeUf);

                    rem.AppendChild(enderReme);
                }                                

                infCte.AppendChild(rem);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir dados do remetente no XML.", ex);
            }

            #endregion

            #region Informações do Expedidor

            try
            {
                var participanteExpedidor = participantes.Where(f => f.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Expedidor).FirstOrDefault();
                if (participanteExpedidor != null)
                {
                    XmlElement exped = doc.CreateElement("exped");

                    if (participanteExpedidor.IdCliente > 0)
                    {
                        //throw new Exception("Expedidor não possui inscrição estadual");

                        var expedidor = ClienteDAO.Instance.GetElement(participanteExpedidor.IdCliente.Value);
                        var pj = expedidor.TipoPessoa.ToUpper() == "J";
                        var idCidadeExpedidor = expedidor.IdCidade != null ? expedidor.IdCidade.ToString() : "0";
                        var cidadeExpedidor = CidadeDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idCidadeExpedidor));

                        var numDocumento = Formatacoes.TrataStringDocFiscal(expedidor.CpfCnpj);

                        if (numDocumento.Length == 11)
                            ManipulacaoXml.SetNode(doc, exped, "CPF", Formatacoes.TrataStringDocFiscal(expedidor.CpfCnpj).PadLeft(11, '0'));
                        else if (numDocumento.Length == 14)
                            ManipulacaoXml.SetNode(doc, exped, "CNPJ", Formatacoes.TrataStringDocFiscal(expedidor.CpfCnpj).PadLeft(14, '0'));

                        if (string.IsNullOrEmpty(expedidor.RgEscinst) || expedidor.RgEscinst.ToLower().Contains("isento"))
                        {
                            ManipulacaoXml.SetNode(doc, exped, "IE", "ISENTO");
                        }
                        else if (pj || expedidor.ProdutorRural)
                        {
                            if (String.IsNullOrEmpty(expedidor.RgEscinst))
                                throw new Exception("Informe a inscrição estadual do expedidor.");
                            ManipulacaoXml.SetNode(doc, exped, "IE", Formatacoes.TrataStringDocFiscal(expedidor.RgEscinst));
                        }
                        
                        if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Homologacao)
                            ManipulacaoXml.SetNode(doc, exped, "xNome", "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                        else
                            ManipulacaoXml.SetNode(doc, exped, "xNome", Formatacoes.TrataStringDocFiscal(expedidor.Nome));

                        XmlElement enderExped = doc.CreateElement("enderExped");
                        ManipulacaoXml.SetNode(doc, enderExped, "xLgr", Formatacoes.TrataStringDocFiscal(expedidor.Endereco));
                        ManipulacaoXml.SetNode(doc, enderExped, "nro", Formatacoes.TrataStringDocFiscal(expedidor.Numero));

                        if (!String.IsNullOrEmpty(Formatacoes.TrataStringDocFiscal(expedidor.Compl)))
                            ManipulacaoXml.SetNode(doc, enderExped, "xCpl", Formatacoes.TrataStringDocFiscal(expedidor.Compl));

                        ManipulacaoXml.SetNode(doc, enderExped, "xBairro", Formatacoes.TrataStringDocFiscal(expedidor.Bairro));
                        ManipulacaoXml.SetNode(doc, enderExped, "cMun", cidadeExpedidor.CodUfMunicipio);
                        ManipulacaoXml.SetNode(doc, enderExped, "xMun", Formatacoes.TrataStringDocFiscal(cidadeExpedidor.NomeCidade));
                        if (!string.IsNullOrEmpty(loja.Cep))
                            ManipulacaoXml.SetNode(doc, enderExped, "CEP", Formatacoes.TrataStringDocFiscal(expedidor.Cep));
                        ManipulacaoXml.SetNode(doc, enderExped, "UF", cidadeExpedidor.NomeUf);

                        exped.AppendChild(enderExped);

                        infCte.AppendChild(exped);
                    }
                    else if (participanteExpedidor.IdLoja > 0)
                    {
                        var expedidor = LojaDAO.Instance.GetElement(participanteExpedidor.IdLoja.Value);
                        var idcidadeExpedidor = expedidor.IdCidade != null ? expedidor.IdCidade.ToString() : "0";
                        var cidadeExpedidor = CidadeDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idcidadeExpedidor));

                        ManipulacaoXml.SetNode(doc, exped, "CNPJ", Formatacoes.TrataStringDocFiscal(expedidor.Cnpj).PadLeft(14, '0'));

                        if (string.IsNullOrEmpty(expedidor.InscEst))
                            throw new Exception("Expedidor não possui inscrição estadual");
                        ManipulacaoXml.SetNode(doc, exped, "IE", expedidor.InscEst);

                        if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Homologacao)
                            ManipulacaoXml.SetNode(doc, exped, "xNome", "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                        else
                            ManipulacaoXml.SetNode(doc, exped, "xNome", Formatacoes.TrataStringDocFiscal(expedidor.RazaoSocial));

                        XmlElement enderExped = doc.CreateElement("enderExped");
                        ManipulacaoXml.SetNode(doc, enderExped, "xLgr", Formatacoes.TrataStringDocFiscal(expedidor.Endereco));
                        ManipulacaoXml.SetNode(doc, enderExped, "nro", Formatacoes.TrataStringDocFiscal(expedidor.Numero));

                        if (!String.IsNullOrEmpty(Formatacoes.TrataStringDocFiscal(expedidor.Compl)))
                            ManipulacaoXml.SetNode(doc, enderExped, "xCpl", Formatacoes.TrataStringDocFiscal(expedidor.Compl));

                        ManipulacaoXml.SetNode(doc, enderExped, "xBairro", Formatacoes.TrataStringDocFiscal(expedidor.Bairro));
                        ManipulacaoXml.SetNode(doc, enderExped, "cMun", cidadeExpedidor.CodUfMunicipio);
                        ManipulacaoXml.SetNode(doc, enderExped, "xMun", Formatacoes.TrataStringDocFiscal(cidadeExpedidor.NomeCidade));
                        if (!string.IsNullOrEmpty(loja.Cep))
                            ManipulacaoXml.SetNode(doc, enderExped, "CEP", Formatacoes.TrataStringDocFiscal(expedidor.Cep));
                        ManipulacaoXml.SetNode(doc, enderExped, "UF", cidadeExpedidor.NomeUf);

                        exped.AppendChild(enderExped);

                        infCte.AppendChild(exped);
                    }
                    else if (participanteExpedidor.IdFornec > 0)
                    {
                        //throw new Exception("Expedidor não possui inscrição estadual");
                        
                        var expedidor = FornecedorDAO.Instance.GetElement(participanteExpedidor.IdFornec.Value);

                        var pj = expedidor.TipoPessoa.ToUpper() == "J";

                        var idCidadeExpedidor = expedidor.IdCidade != null ? expedidor.IdCidade.ToString() : "0";
                        var cidadeExpedidor = CidadeDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idCidadeExpedidor));

                        var numDocumento = Formatacoes.TrataStringDocFiscal(expedidor.CpfCnpj);

                        if (numDocumento.Length == 11)
                            ManipulacaoXml.SetNode(doc, exped, "CPF", Formatacoes.TrataStringDocFiscal(expedidor.CpfCnpj).PadLeft(11, '0'));
                        else if (numDocumento.Length == 14)
                            ManipulacaoXml.SetNode(doc, exped, "CNPJ", Formatacoes.TrataStringDocFiscal(expedidor.CpfCnpj).PadLeft(14, '0'));

                        if (string.IsNullOrEmpty(expedidor.RgInscEst) || expedidor.RgInscEst.ToLower().Contains("isento"))
                        {
                            ManipulacaoXml.SetNode(doc, exped, "IE", "ISENTO");
                        }
                        else if (pj || expedidor.ProdutorRural)
                        {
                            if (String.IsNullOrEmpty(expedidor.RgInscEst))
                                throw new Exception("Informe a inscrição estadual do expedidor.");
                            ManipulacaoXml.SetNode(doc, exped, "IE", Formatacoes.TrataStringDocFiscal(expedidor.RgInscEst));
                        }

                        if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Homologacao)
                            ManipulacaoXml.SetNode(doc, exped, "xNome", "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                        else
                            ManipulacaoXml.SetNode(doc, exped, "xNome", Formatacoes.TrataStringDocFiscal(expedidor.Razaosocial));

                        XmlElement enderExped = doc.CreateElement("enderExped");
                        ManipulacaoXml.SetNode(doc, enderExped, "xLgr", Formatacoes.TrataStringDocFiscal(expedidor.Endereco));
                        ManipulacaoXml.SetNode(doc, enderExped, "nro", Formatacoes.TrataStringDocFiscal(expedidor.Numero));

                        if (!String.IsNullOrEmpty(Formatacoes.TrataStringDocFiscal(expedidor.Compl)))
                            ManipulacaoXml.SetNode(doc, enderExped, "xCpl", Formatacoes.TrataStringDocFiscal(expedidor.Compl));

                        ManipulacaoXml.SetNode(doc, enderExped, "xBairro", Formatacoes.TrataStringDocFiscal(expedidor.Bairro));
                        ManipulacaoXml.SetNode(doc, enderExped, "cMun", cidadeExpedidor.CodUfMunicipio);
                        ManipulacaoXml.SetNode(doc, enderExped, "xMun", Formatacoes.TrataStringDocFiscal(cidadeExpedidor.NomeCidade));
                        if (!string.IsNullOrEmpty(loja.Cep))
                            ManipulacaoXml.SetNode(doc, enderExped, "CEP", Formatacoes.TrataStringDocFiscal(expedidor.Cep));
                        ManipulacaoXml.SetNode(doc, enderExped, "UF", cidadeExpedidor.NomeUf);

                        exped.AppendChild(enderExped);

                        infCte.AppendChild(exped);
                    }
                    else if (participanteExpedidor.IdTransportador > 0)
                    {
                        var expedidor = TransportadorDAO.Instance.GetElement(participanteExpedidor.IdTransportador.Value);
                        var idCidadeExpedidor = expedidor.IdCidade != null ? expedidor.IdCidade.ToString() : "0";
                        var cidadeExpedidor = CidadeDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idCidadeExpedidor));

                        var numDocumento = Formatacoes.TrataStringDocFiscal(expedidor.CpfCnpj);

                        if (numDocumento.Length == 11)
                            ManipulacaoXml.SetNode(doc, exped, "CPF", Formatacoes.TrataStringDocFiscal(expedidor.CpfCnpj).PadLeft(11, '0'));
                        else if (numDocumento.Length == 14)
                            ManipulacaoXml.SetNode(doc, exped, "CNPJ", Formatacoes.TrataStringDocFiscal(expedidor.CpfCnpj).PadLeft(14, '0'));

                        if (string.IsNullOrEmpty(expedidor.InscEst))
                            throw new Exception("Expedidor não possui inscrição estadual");
                        ManipulacaoXml.SetNode(doc, exped, "IE", expedidor.InscEst);

                        if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Homologacao)
                            ManipulacaoXml.SetNode(doc, exped, "xNome", "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                        else
                            ManipulacaoXml.SetNode(doc, exped, "xNome", Formatacoes.TrataStringDocFiscal(expedidor.Nome));

                        XmlElement enderExped = doc.CreateElement("enderExped");
                        ManipulacaoXml.SetNode(doc, enderExped, "xLgr", Formatacoes.TrataStringDocFiscal(expedidor.Endereco));
                        ManipulacaoXml.SetNode(doc, enderExped, "nro", Formatacoes.TrataStringDocFiscal(expedidor.Numero));

                        ManipulacaoXml.SetNode(doc, enderExped, "xBairro", Formatacoes.TrataStringDocFiscal(expedidor.Bairro));
                        ManipulacaoXml.SetNode(doc, enderExped, "cMun", cidadeExpedidor.CodUfMunicipio);
                        ManipulacaoXml.SetNode(doc, enderExped, "xMun", Formatacoes.TrataStringDocFiscal(cidadeExpedidor.NomeCidade));
                        if (!string.IsNullOrEmpty(loja.Cep))
                            ManipulacaoXml.SetNode(doc, enderExped, "CEP", Formatacoes.TrataStringDocFiscal(expedidor.Cep));
                        ManipulacaoXml.SetNode(doc, enderExped, "UF", cidadeExpedidor.NomeUf);

                        exped.AppendChild(enderExped);

                        infCte.AppendChild(exped);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir dados do remetente no XML.", ex);
            }

            #endregion

            #region Informações do Recebedor

            try
            {
                var participanteRecebedor = participantes.Where(f => f.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Recebedor).FirstOrDefault();
                if (participanteRecebedor != null)
                {
                    XmlElement receb = doc.CreateElement("receb");

                    if (participanteRecebedor.IdCliente > 0)
                    {
                        var recebedor = ClienteDAO.Instance.GetElement(participanteRecebedor.IdCliente.Value);

                        var pj = recebedor.TipoPessoa.ToUpper() == "J";

                        var idCidadeRecebedor = recebedor.IdCidade != null ? recebedor.IdCidade.ToString() : "0";
                        var cidadeRecebedor = CidadeDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idCidadeRecebedor));

                        var numDocumento = Formatacoes.TrataStringDocFiscal(recebedor.CpfCnpj);

                        if (numDocumento.Length == 11)
                            ManipulacaoXml.SetNode(doc, receb, "CPF", Formatacoes.TrataStringDocFiscal(recebedor.CpfCnpj).PadLeft(11, '0'));
                        else if (numDocumento.Length == 14)
                            ManipulacaoXml.SetNode(doc, receb, "CNPJ", Formatacoes.TrataStringDocFiscal(recebedor.CpfCnpj).PadLeft(14, '0'));

                        if (string.IsNullOrEmpty(recebedor.RgEscinst) || recebedor.RgEscinst.ToLower().Contains("isento"))
                        {
                            ManipulacaoXml.SetNode(doc, receb, "IE", "ISENTO");
                        }
                        else if (pj || recebedor.ProdutorRural)
                        {
                            if (String.IsNullOrEmpty(recebedor.RgEscinst))
                                throw new Exception("Informe a inscrição estadual do recebedor.");
                            ManipulacaoXml.SetNode(doc, receb, "IE", Formatacoes.TrataStringDocFiscal(recebedor.RgEscinst));
                        }

                        if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Homologacao)
                            ManipulacaoXml.SetNode(doc, receb, "xNome", "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                        else
                            ManipulacaoXml.SetNode(doc, receb, "xNome", Formatacoes.TrataStringDocFiscal(recebedor.Nome));

                        XmlElement enderReceb = doc.CreateElement("enderReceb");
                        ManipulacaoXml.SetNode(doc, enderReceb, "xLgr", Formatacoes.TrataStringDocFiscal(recebedor.Endereco));
                        ManipulacaoXml.SetNode(doc, enderReceb, "nro", Formatacoes.TrataStringDocFiscal(recebedor.Numero));

                        if (!String.IsNullOrEmpty(Formatacoes.TrataStringDocFiscal(recebedor.Compl)))
                            ManipulacaoXml.SetNode(doc, enderReceb, "xCpl", Formatacoes.TrataStringDocFiscal(recebedor.Compl));

                        ManipulacaoXml.SetNode(doc, enderReceb, "xBairro", Formatacoes.TrataStringDocFiscal(recebedor.Bairro));
                        ManipulacaoXml.SetNode(doc, enderReceb, "cMun", cidadeRecebedor.CodUfMunicipio);
                        ManipulacaoXml.SetNode(doc, enderReceb, "xMun", Formatacoes.TrataStringDocFiscal(cidadeRecebedor.NomeCidade));
                        if (!string.IsNullOrEmpty(loja.Cep))
                            ManipulacaoXml.SetNode(doc, enderReceb, "CEP", Formatacoes.TrataStringDocFiscal(recebedor.Cep));
                        ManipulacaoXml.SetNode(doc, enderReceb, "UF", cidadeRecebedor.NomeUf);

                        receb.AppendChild(enderReceb);

                        infCte.AppendChild(receb);
                    }
                    else if (participanteRecebedor.IdLoja > 0)
                    {
                        var recebedor = LojaDAO.Instance.GetElement(participanteRecebedor.IdLoja.Value);
                        var idcidadeRecebedor = recebedor.IdCidade != null ? recebedor.IdCidade.ToString() : "0";
                        var cidadeRecebedor = CidadeDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idcidadeRecebedor));

                        ManipulacaoXml.SetNode(doc, receb, "CNPJ", Formatacoes.TrataStringDocFiscal(recebedor.Cnpj).PadLeft(14, '0'));

                        if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Homologacao)
                            ManipulacaoXml.SetNode(doc, receb, "xNome", "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                        else
                            ManipulacaoXml.SetNode(doc, receb, "xNome", Formatacoes.TrataStringDocFiscal(recebedor.RazaoSocial));

                        XmlElement enderReceb = doc.CreateElement("enderReceb");
                        ManipulacaoXml.SetNode(doc, enderReceb, "xLgr", Formatacoes.TrataStringDocFiscal(recebedor.Endereco));
                        ManipulacaoXml.SetNode(doc, enderReceb, "nro", Formatacoes.TrataStringDocFiscal(recebedor.Numero));

                        if (!String.IsNullOrEmpty(Formatacoes.TrataStringDocFiscal(recebedor.Compl)))
                            ManipulacaoXml.SetNode(doc, enderReceb, "xCpl", Formatacoes.TrataStringDocFiscal(recebedor.Compl));

                        ManipulacaoXml.SetNode(doc, enderReceb, "xBairro", Formatacoes.TrataStringDocFiscal(recebedor.Bairro));
                        ManipulacaoXml.SetNode(doc, enderReceb, "cMun", cidadeRecebedor.CodUfMunicipio);
                        ManipulacaoXml.SetNode(doc, enderReceb, "xMun", Formatacoes.TrataStringDocFiscal(cidadeRecebedor.NomeCidade));
                        if (!string.IsNullOrEmpty(loja.Cep))
                            ManipulacaoXml.SetNode(doc, enderReceb, "CEP", Formatacoes.TrataStringDocFiscal(recebedor.Cep));
                        ManipulacaoXml.SetNode(doc, enderReceb, "UF", cidadeRecebedor.NomeUf);

                        receb.AppendChild(enderReceb);

                        infCte.AppendChild(receb);
                    }
                    else if (participanteRecebedor.IdFornec > 0)
                    {
                        var recebedor = FornecedorDAO.Instance.GetElement(participanteRecebedor.IdFornec.Value);

                        var pj = recebedor.TipoPessoa.ToUpper() == "J";

                        var idCidadeRecebedor = recebedor.IdCidade != null ? recebedor.IdCidade.ToString() : "0";
                        var cidadeRecebedor = CidadeDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idCidadeRecebedor));

                        var numDocumento = Formatacoes.TrataStringDocFiscal(recebedor.CpfCnpj);

                        if (numDocumento.Length == 11)
                            ManipulacaoXml.SetNode(doc, receb, "CPF", Formatacoes.TrataStringDocFiscal(recebedor.CpfCnpj).PadLeft(11, '0'));
                        else if (numDocumento.Length == 14)
                            ManipulacaoXml.SetNode(doc, receb, "CNPJ", Formatacoes.TrataStringDocFiscal(recebedor.CpfCnpj).PadLeft(14, '0'));

                        if (string.IsNullOrEmpty(recebedor.RgInscEst) || recebedor.RgInscEst.ToLower().Contains("isento"))
                        {
                            ManipulacaoXml.SetNode(doc, receb, "IE", "ISENTO");
                        }
                        else if (pj || recebedor.ProdutorRural)
                        {
                            if (String.IsNullOrEmpty(recebedor.RgInscEst))
                                throw new Exception("Informe a inscrição estadual do recebedor.");
                            ManipulacaoXml.SetNode(doc, receb, "IE", Formatacoes.TrataStringDocFiscal(recebedor.RgInscEst));
                        }

                        if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Homologacao)
                            ManipulacaoXml.SetNode(doc, receb, "xNome", "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                        else
                            ManipulacaoXml.SetNode(doc, receb, "xNome", Formatacoes.TrataStringDocFiscal(recebedor.Razaosocial));

                        XmlElement enderReceb = doc.CreateElement("enderReceb");
                        ManipulacaoXml.SetNode(doc, enderReceb, "xLgr", Formatacoes.TrataStringDocFiscal(recebedor.Endereco));
                        ManipulacaoXml.SetNode(doc, enderReceb, "nro", Formatacoes.TrataStringDocFiscal(recebedor.Numero));

                        if (!String.IsNullOrEmpty(Formatacoes.TrataStringDocFiscal(recebedor.Compl)))
                            ManipulacaoXml.SetNode(doc, enderReceb, "xCpl", Formatacoes.TrataStringDocFiscal(recebedor.Compl));

                        ManipulacaoXml.SetNode(doc, enderReceb, "xBairro", Formatacoes.TrataStringDocFiscal(recebedor.Bairro));
                        ManipulacaoXml.SetNode(doc, enderReceb, "cMun", cidadeRecebedor.CodUfMunicipio);
                        ManipulacaoXml.SetNode(doc, enderReceb, "xMun", Formatacoes.TrataStringDocFiscal(cidadeRecebedor.NomeCidade));
                        if (!string.IsNullOrEmpty(loja.Cep))
                            ManipulacaoXml.SetNode(doc, enderReceb, "CEP", Formatacoes.TrataStringDocFiscal(recebedor.Cep));
                        ManipulacaoXml.SetNode(doc, enderReceb, "UF", cidadeRecebedor.NomeUf);

                        receb.AppendChild(enderReceb);

                        infCte.AppendChild(receb);
                    }
                    else if (participanteRecebedor.IdTransportador > 0)
                    {
                        var recebedor = TransportadorDAO.Instance.GetElement(participanteRecebedor.IdTransportador.Value);
                        var idCidadeRecebedor = recebedor.IdCidade != null ? recebedor.IdCidade.ToString() : "0";
                        var cidadeRecebedor = CidadeDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idCidadeRecebedor));

                        var numDocumento = Formatacoes.TrataStringDocFiscal(recebedor.CpfCnpj);

                        if (numDocumento.Length == 11)
                            ManipulacaoXml.SetNode(doc, receb, "CPF", Formatacoes.TrataStringDocFiscal(recebedor.CpfCnpj).PadLeft(11, '0'));
                        else if (numDocumento.Length == 14)
                            ManipulacaoXml.SetNode(doc, receb, "CNPJ", Formatacoes.TrataStringDocFiscal(recebedor.CpfCnpj).PadLeft(14, '0'));

                        if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Homologacao)
                            ManipulacaoXml.SetNode(doc, receb, "xNome", "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                        else
                            ManipulacaoXml.SetNode(doc, receb, "xNome", Formatacoes.TrataStringDocFiscal(recebedor.Nome));

                        XmlElement enderReceb = doc.CreateElement("enderReceb");
                        ManipulacaoXml.SetNode(doc, enderReceb, "xLgr", Formatacoes.TrataStringDocFiscal(recebedor.Endereco));
                        ManipulacaoXml.SetNode(doc, enderReceb, "nro", Formatacoes.TrataStringDocFiscal(recebedor.Numero));

                        ManipulacaoXml.SetNode(doc, enderReceb, "xBairro", Formatacoes.TrataStringDocFiscal(recebedor.Bairro));
                        ManipulacaoXml.SetNode(doc, enderReceb, "cMun", cidadeRecebedor.CodUfMunicipio);
                        ManipulacaoXml.SetNode(doc, enderReceb, "xMun", Formatacoes.TrataStringDocFiscal(cidadeRecebedor.NomeCidade));
                        if (!string.IsNullOrEmpty(loja.Cep))
                            ManipulacaoXml.SetNode(doc, enderReceb, "CEP", Formatacoes.TrataStringDocFiscal(recebedor.Cep));
                        ManipulacaoXml.SetNode(doc, enderReceb, "UF", cidadeRecebedor.NomeUf);

                        receb.AppendChild(enderReceb);

                        infCte.AppendChild(receb);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir dados do remetente no XML.", ex);
            }

            #endregion

            #region Informações do Destinatário

            try
            {
                var participanteDestinatario = participantes.Where(f => f.TipoParticipante == ParticipanteCte.TipoParticipanteEnum.Destinatario).First();
                XmlElement dest = doc.CreateElement("dest");

                if (participanteDestinatario.IdCliente > 0)
                {
                    var clienteDest = ClienteDAO.Instance.GetElement(participanteDestinatario.IdCliente.Value);

                    var pj = clienteDest.TipoPessoa.ToUpper() == "J";

                    var idCidadeCliente = clienteDest.IdCidade != null ? clienteDest.IdCidade.ToString() : "0";
                    var cidadeCliente = CidadeDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idCidadeCliente));

                    var numDocumento = Formatacoes.TrataStringDocFiscal(clienteDest.CpfCnpj);

                    if (numDocumento.Length == 11)
                        ManipulacaoXml.SetNode(doc, dest, "CPF", Formatacoes.TrataStringDocFiscal(clienteDest.CpfCnpj).PadLeft(11, '0'));
                    else if (numDocumento.Length == 14)
                        ManipulacaoXml.SetNode(doc, dest, "CNPJ", Formatacoes.TrataStringDocFiscal(clienteDest.CpfCnpj).PadLeft(14, '0'));

                    if (string.IsNullOrEmpty(clienteDest.RgEscinst) || clienteDest.RgEscinst.ToLower().Contains("isento"))
                    {
                        ManipulacaoXml.SetNode(doc, dest, "IE", "ISENTO");
                    }
                    else if (pj || clienteDest.ProdutorRural)
                    {
                        if (String.IsNullOrEmpty(clienteDest.RgEscinst))
                            throw new Exception("Informe a inscrição estadual do cliente.");
                        ManipulacaoXml.SetNode(doc, dest, "IE", Formatacoes.TrataStringDocFiscal(clienteDest.RgEscinst));
                    }
                    
                    if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Homologacao)
                        ManipulacaoXml.SetNode(doc, dest, "xNome", "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                    else
                        ManipulacaoXml.SetNode(doc, dest, "xNome", Formatacoes.TrataStringDocFiscal(clienteDest.Nome));
                    
                    XmlElement enderDest = doc.CreateElement("enderDest");
                    ManipulacaoXml.SetNode(doc, enderDest, "xLgr", Formatacoes.TrataStringDocFiscal(clienteDest.Endereco));
                    ManipulacaoXml.SetNode(doc, enderDest, "nro", Formatacoes.TrataStringDocFiscal(clienteDest.Numero));

                    if (!String.IsNullOrEmpty(Formatacoes.TrataStringDocFiscal(clienteDest.Compl)))
                        ManipulacaoXml.SetNode(doc, enderDest, "xCpl", Formatacoes.TrataStringDocFiscal(clienteDest.Compl));

                    ManipulacaoXml.SetNode(doc, enderDest, "xBairro", Formatacoes.TrataStringDocFiscal(clienteDest.Bairro));
                    ManipulacaoXml.SetNode(doc, enderDest, "cMun", cidadeCliente.CodUfMunicipio);
                    ManipulacaoXml.SetNode(doc, enderDest, "xMun", Formatacoes.TrataStringDocFiscal(cidadeCliente.NomeCidade));
                    if (!string.IsNullOrEmpty(loja.Cep))
                        ManipulacaoXml.SetNode(doc, enderDest, "CEP", Formatacoes.TrataStringDocFiscal(clienteDest.Cep));
                    ManipulacaoXml.SetNode(doc, enderDest, "UF", cidadeCliente.NomeUf);

                    dest.AppendChild(enderDest);

                    infCte.AppendChild(dest);
                }
                else if (participanteDestinatario.IdLoja > 0)
                {
                    var destinatario = LojaDAO.Instance.GetElement(participanteDestinatario.IdLoja.Value);
                    var idCidadeDestinatario = destinatario.IdCidade != null ? destinatario.IdCidade.ToString() : "0";
                    var cidadeDestinatario = CidadeDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idCidadeDestinatario));

                    ManipulacaoXml.SetNode(doc, dest, "CNPJ", Formatacoes.TrataStringDocFiscal(destinatario.Cnpj).PadLeft(14, '0'));
                    
                    if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Homologacao)
                        ManipulacaoXml.SetNode(doc, dest, "xNome", "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                    else
                        ManipulacaoXml.SetNode(doc, dest, "xNome", Formatacoes.TrataStringDocFiscal(destinatario.RazaoSocial));
                    
                    XmlElement enderDest = doc.CreateElement("enderDest");
                    ManipulacaoXml.SetNode(doc, enderDest, "xLgr", Formatacoes.TrataStringDocFiscal(destinatario.Endereco));
                    ManipulacaoXml.SetNode(doc, enderDest, "nro", Formatacoes.TrataStringDocFiscal(destinatario.Numero));

                    if (!String.IsNullOrEmpty(Formatacoes.TrataStringDocFiscal(destinatario.Compl)))
                        ManipulacaoXml.SetNode(doc, enderDest, "xCpl", Formatacoes.TrataStringDocFiscal(destinatario.Compl));

                    ManipulacaoXml.SetNode(doc, enderDest, "xBairro", Formatacoes.TrataStringDocFiscal(destinatario.Bairro));
                    ManipulacaoXml.SetNode(doc, enderDest, "cMun", cidadeDestinatario.CodUfMunicipio);
                    ManipulacaoXml.SetNode(doc, enderDest, "xMun", Formatacoes.TrataStringDocFiscal(cidadeDestinatario.NomeCidade));
                    if (!string.IsNullOrEmpty(loja.Cep))
                        ManipulacaoXml.SetNode(doc, enderDest, "CEP", Formatacoes.TrataStringDocFiscal(destinatario.Cep));
                    ManipulacaoXml.SetNode(doc, enderDest, "UF", cidadeDestinatario.NomeUf);

                    dest.AppendChild(enderDest);

                    infCte.AppendChild(dest);
                }
                else if (participanteDestinatario.IdFornec > 0)
                {
                    var destinatario = FornecedorDAO.Instance.GetElement(participanteDestinatario.IdFornec.Value);

                    var pj = destinatario.TipoPessoa.ToUpper() == "J";

                    var idCidadeDestinatario = destinatario.IdCidade != null ? destinatario.IdCidade.ToString() : "0";
                    var cidadeDestinatario = CidadeDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idCidadeDestinatario));

                    var numDocumento = Formatacoes.TrataStringDocFiscal(destinatario.CpfCnpj);

                    if (numDocumento.Length == 11)
                        ManipulacaoXml.SetNode(doc, dest, "CPF", Formatacoes.TrataStringDocFiscal(destinatario.CpfCnpj).PadLeft(11, '0'));
                    else if (numDocumento.Length == 14)
                        ManipulacaoXml.SetNode(doc, dest, "CNPJ", Formatacoes.TrataStringDocFiscal(destinatario.CpfCnpj).PadLeft(14, '0'));

                    if (string.IsNullOrEmpty(destinatario.RgInscEst) || destinatario.RgInscEst.ToLower().Contains("isento"))
                    {
                        ManipulacaoXml.SetNode(doc, dest, "IE", "ISENTO");
                    }
                    else if (pj || destinatario.ProdutorRural)
                    {
                        if (String.IsNullOrEmpty(destinatario.RgInscEst))
                            throw new Exception("Informe a inscrição estadual do cliente.");
                        ManipulacaoXml.SetNode(doc, dest, "IE", Formatacoes.TrataStringDocFiscal(destinatario.RgInscEst));
                    }

                    if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Homologacao)
                        ManipulacaoXml.SetNode(doc, dest, "xNome", "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                    else
                        ManipulacaoXml.SetNode(doc, dest, "xNome", Formatacoes.TrataStringDocFiscal(destinatario.Razaosocial));

                    XmlElement enderDest = doc.CreateElement("enderDest");
                    ManipulacaoXml.SetNode(doc, enderDest, "xLgr", Formatacoes.TrataStringDocFiscal(destinatario.Endereco));
                    ManipulacaoXml.SetNode(doc, enderDest, "nro", Formatacoes.TrataStringDocFiscal(destinatario.Numero));

                    if (!String.IsNullOrEmpty(Formatacoes.TrataStringDocFiscal(destinatario.Compl)))
                        ManipulacaoXml.SetNode(doc, enderDest, "xCpl", Formatacoes.TrataStringDocFiscal(destinatario.Compl));

                    ManipulacaoXml.SetNode(doc, enderDest, "xBairro", Formatacoes.TrataStringDocFiscal(destinatario.Bairro));
                    ManipulacaoXml.SetNode(doc, enderDest, "cMun", cidadeDestinatario.CodUfMunicipio);
                    ManipulacaoXml.SetNode(doc, enderDest, "xMun", Formatacoes.TrataStringDocFiscal(cidadeDestinatario.NomeCidade));
                    if (!string.IsNullOrEmpty(loja.Cep))
                        ManipulacaoXml.SetNode(doc, enderDest, "CEP", Formatacoes.TrataStringDocFiscal(destinatario.Cep));
                    ManipulacaoXml.SetNode(doc, enderDest, "UF", cidadeDestinatario.NomeUf);

                    dest.AppendChild(enderDest);

                    infCte.AppendChild(dest);
                }
                else if (participanteDestinatario.IdTransportador > 0)
                {
                    var destinatario = TransportadorDAO.Instance.GetElement(participanteDestinatario.IdTransportador.Value);
                    var idCidadeDestinatario = destinatario.IdCidade != null ? destinatario.IdCidade.ToString() : "0";
                    var cidadeDestinatario = CidadeDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idCidadeDestinatario));

                    var numDocumento = Formatacoes.TrataStringDocFiscal(destinatario.CpfCnpj);

                    if (numDocumento.Length == 11)
                        ManipulacaoXml.SetNode(doc, dest, "CPF", Formatacoes.TrataStringDocFiscal(destinatario.CpfCnpj).PadLeft(11, '0'));
                    else if (numDocumento.Length == 14)
                        ManipulacaoXml.SetNode(doc, dest, "CNPJ", Formatacoes.TrataStringDocFiscal(destinatario.CpfCnpj).PadLeft(14, '0'));

                    if (ConfigCTe.TipoAmbiente == ConfigCTe.TipoAmbienteCte.Homologacao)
                        ManipulacaoXml.SetNode(doc, dest, "xNome", "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                    else
                        ManipulacaoXml.SetNode(doc, dest, "xNome", Formatacoes.TrataStringDocFiscal(destinatario.Nome));

                    XmlElement enderDest = doc.CreateElement("enderDest");
                    ManipulacaoXml.SetNode(doc, enderDest, "xLgr", Formatacoes.TrataStringDocFiscal(destinatario.Endereco));
                    ManipulacaoXml.SetNode(doc, enderDest, "nro", Formatacoes.TrataStringDocFiscal(destinatario.Numero));
                    
                    ManipulacaoXml.SetNode(doc, enderDest, "xBairro", Formatacoes.TrataStringDocFiscal(destinatario.Bairro));
                    ManipulacaoXml.SetNode(doc, enderDest, "cMun", cidadeDestinatario.CodUfMunicipio);
                    ManipulacaoXml.SetNode(doc, enderDest, "xMun", Formatacoes.TrataStringDocFiscal(cidadeDestinatario.NomeCidade));
                    if (!string.IsNullOrEmpty(loja.Cep))
                        ManipulacaoXml.SetNode(doc, enderDest, "CEP", Formatacoes.TrataStringDocFiscal(destinatario.Cep));
                    ManipulacaoXml.SetNode(doc, enderDest, "UF", cidadeDestinatario.NomeUf);

                    dest.AppendChild(enderDest);

                    infCte.AppendChild(dest);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir dados do remetente no XML.", ex);
            }

            #endregion
            
            #region Valores da Prestação de Serviço

            try
            {
                XmlElement vPrest = doc.CreateElement("vPrest");
                ManipulacaoXml.SetNode(doc, vPrest, "vTPrest", cte.ValorTotal.ToString().Replace(',','.'));
                ManipulacaoXml.SetNode(doc, vPrest, "vRec", cte.ValorReceber.ToString().Replace(',', '.'));
                foreach (var i in ComponenteValorCteDAO.Instance.GetComponentesByIdCte(cte.IdCte))
                {
                    XmlElement Comp = doc.CreateElement("Comp");
                    ManipulacaoXml.SetNode(doc, Comp, "xNome", i.NomeComponente);
                    ManipulacaoXml.SetNode(doc, Comp, "vComp", i.ValorComponente.ToString());
                    vPrest.AppendChild(Comp);
                }                                               
                infCte.AppendChild(vPrest);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir dados do valor da prestação do serviço no XML.", ex);
            }

            #endregion

            #region Informações relativas aos impostos

            try
            {
                var objIcms = ImpostoCteDAO.Instance.GetElement(idCte, DataSourcesEFD.TipoImpostoEnum.Icms);

                if (objIcms != null)
                {
                    XmlElement imp = doc.CreateElement("imp");

                    XmlElement ICMS = doc.CreateElement("ICMS");
                    if (objIcms.Cst == "00")
                    {
                        XmlElement ICMS00 = doc.CreateElement("ICMS00");
                        ManipulacaoXml.SetNode(doc, ICMS00, "CST", objIcms.Cst.ToString());
                        ManipulacaoXml.SetNode(doc, ICMS00, "vBC", Formatacoes.TrataValorDecimal(objIcms.BaseCalc, 2));
                        ManipulacaoXml.SetNode(doc, ICMS00, "pICMS", Formatacoes.TrataValorDecimal(decimal.Parse(objIcms.Aliquota.ToString()), 2));
                        ManipulacaoXml.SetNode(doc, ICMS00, "vICMS", Formatacoes.TrataValorDecimal(objIcms.Valor, 2));

                        ICMS.AppendChild(ICMS00);
                    }
                    else if (objIcms.Cst == "20")
                    {
                        XmlElement ICMS20 = doc.CreateElement("ICMS20");
                        ManipulacaoXml.SetNode(doc, ICMS20, "CST", objIcms.Cst.ToString());
                        ManipulacaoXml.SetNode(doc, ICMS20, "pRedBC", Formatacoes.TrataValorDecimal(decimal.Parse(objIcms.PercRedBaseCalc.ToString()), 2));
                        ManipulacaoXml.SetNode(doc, ICMS20, "vBC", Formatacoes.TrataValorDecimal(objIcms.BaseCalc, 2));
                        ManipulacaoXml.SetNode(doc, ICMS20, "pICMS", Formatacoes.TrataValorDecimal(decimal.Parse(objIcms.Aliquota.ToString()), 2));
                        ManipulacaoXml.SetNode(doc, ICMS20, "vICMS", Formatacoes.TrataValorDecimal(objIcms.Valor, 2));

                        ICMS.AppendChild(ICMS20);
                    }
                    else if (objIcms.Cst == "40" || objIcms.Cst == "41" || objIcms.Cst == "51")
                    {
                        XmlElement ICMS45 = doc.CreateElement("ICMS45");
                        ManipulacaoXml.SetNode(doc, ICMS45, "CST", objIcms.Cst.ToString());

                        ICMS.AppendChild(ICMS45);
                    }
                    else if (objIcms.Cst == "60")
                    {
                        XmlElement ICMS60 = doc.CreateElement("ICMS60");
                        ManipulacaoXml.SetNode(doc, ICMS60, "CST", objIcms.Cst.ToString());
                        ManipulacaoXml.SetNode(doc, ICMS60, "vBCSTRet", Formatacoes.TrataValorDecimal(objIcms.BaseCalcStRetido, 2));
                        ManipulacaoXml.SetNode(doc, ICMS60, "vICMSSTRet", Formatacoes.TrataValorDecimal(objIcms.ValorStRetido, 2));
                        ManipulacaoXml.SetNode(doc, ICMS60, "pICMSSTRet", Formatacoes.TrataValorDecimal(decimal.Parse(objIcms.AliquotaStRetido.ToString()), 2));
                        ManipulacaoXml.SetNode(doc, ICMS60, "vCred", Formatacoes.TrataValorDecimal(objIcms.ValorCred, 2));

                        ICMS.AppendChild(ICMS60);
                    }
                    else if (objIcms.Cst == "90")
                    {
                        XmlElement ICMS90 = doc.CreateElement("ICMS90");
                        ManipulacaoXml.SetNode(doc, ICMS90, "CST", objIcms.Cst.ToString());
                        ManipulacaoXml.SetNode(doc, ICMS90, "pRedBC", Formatacoes.TrataValorDecimal(decimal.Parse(objIcms.PercRedBaseCalc.ToString()), 2));
                        ManipulacaoXml.SetNode(doc, ICMS90, "vBC", Formatacoes.TrataValorDecimal(objIcms.BaseCalc, 2));
                        ManipulacaoXml.SetNode(doc, ICMS90, "pICMS", Formatacoes.TrataValorDecimal(decimal.Parse(objIcms.Aliquota.ToString()), 2));
                        ManipulacaoXml.SetNode(doc, ICMS90, "vICMS", Formatacoes.TrataValorDecimal(objIcms.Valor, 2));
                        ManipulacaoXml.SetNode(doc, ICMS90, "vCred", Formatacoes.TrataValorDecimal(objIcms.ValorCred, 2));

                        ICMS.AppendChild(ICMS90);
                    }
                    imp.AppendChild(ICMS);
                    infCte.AppendChild(imp);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir dados dos impostos no XML.", ex);
            }

            #endregion

            #region Grupo de informações do CT-e Normal e Substituto

            try
            {
                XmlElement infCTeNorm = doc.CreateElement("infCTeNorm");
                XmlElement infCarga = doc.CreateElement("infCarga");

                var objInfoCte = InfoCteDAO.Instance.GetElement(idCte);

                if (objInfoCte != null)
                {
                    if (!string.IsNullOrEmpty(objInfoCte.ValorCarga.ToString()))
                        ManipulacaoXml.SetNode(doc, infCarga, "vCarga", Formatacoes.TrataValorDecimal(objInfoCte.ValorCarga, 2));

                    if (String.IsNullOrEmpty(objInfoCte.ProdutoPredominante))
                        throw new Exception("Produto não informado no CTe.");

                    ManipulacaoXml.SetNode(doc, infCarga, "proPred",
                        objInfoCte.ProdutoPredominante.Length > 60 ? objInfoCte.ProdutoPredominante.Substring(0, 60) : objInfoCte.ProdutoPredominante);

                    if (!string.IsNullOrEmpty(objInfoCte.OutrasCaract.ToString()))
                        ManipulacaoXml.SetNode(doc, infCarga, "xOutCat", objInfoCte.OutrasCaract.ToString());
                }

                var objInfoCargaCte = InfoCargaCteDAO.Instance.GetList(idCte);
                foreach (var i in objInfoCargaCte)
                {
                    XmlElement infQ = doc.CreateElement("infQ");

                    ManipulacaoXml.SetNode(doc, infQ, "cUnid", "0" + i.TipoUnidade.ToString());
                    ManipulacaoXml.SetNode(doc, infQ, "tpMed", i.TipoMedida.ToString());

                    ManipulacaoXml.SetNode(doc, infQ, "qCarga", Formatacoes.TrataValorDecimal((decimal)i.Quantidade, 4));

                    infCarga.AppendChild(infQ);
                }

                infCTeNorm.AppendChild(infCarga);

                if (!FiscalConfig.ConhecimentoTransporte.ExibirGridNotaFiscal)
                {
                    var chaves = new GDA.Sql.Query("IdCte=?idCte")
                        .Add("?idCte", idCte)
                        .ToList<Glass.Data.Model.Cte.ChaveAcessoCte>();

                    XmlElement infDoc = doc.CreateElement("infDoc");

                    foreach (var i in chaves)
                    {                        
                        XmlElement infNFe = doc.CreateElement("infNFe");

                        ManipulacaoXml.SetNode(doc, infNFe, "chave", i.ChaveAcesso);

                        infDoc.AppendChild(infNFe);                        
                    }

                    infCTeNorm.AppendChild(infDoc);
                }
                else
                {
                    XmlElement infDoc = doc.CreateElement("infDoc");

                    foreach (var i in NotaFiscalCteDAO.Instance.GetList(cte.IdCte))
                    {
                        var nota = NotaFiscalDAO.Instance.GetElement(i.IdNf);                        

                        if (nota.Modelo == "55")
                        {
                            XmlElement infNFe = doc.CreateElement("infNFe");

                            ManipulacaoXml.SetNode(doc, infNFe, "chave", nota.ChaveAcesso);

                            infDoc.AppendChild(infNFe);
                        }
                        else
                        {
                            XmlElement infNf = doc.CreateElement("infNF");

                            ManipulacaoXml.SetNode(doc, infNf, "mod", "01");
                            ManipulacaoXml.SetNode(doc, infNf, "serie", nota.Serie);
                            ManipulacaoXml.SetNode(doc, infNf, "nDoc", nota.NumeroNFe.ToString());
                            if (!string.IsNullOrEmpty(nota.DataEmissao.ToString()))
                                ManipulacaoXml.SetNode(doc, infNf, "dEmi", nota.DataEmissao.ToString("yyyy-MM-dd"));
                            ManipulacaoXml.SetNode(doc, infNf, "vBC", Formatacoes.TrataValorDecimal(nota.BcIcms, 2));
                            ManipulacaoXml.SetNode(doc, infNf, "vICMS", Formatacoes.TrataValorDecimal(nota.Valoricms, 2));
                            ManipulacaoXml.SetNode(doc, infNf, "vBCST", Formatacoes.TrataValorDecimal(nota.BcIcmsSt, 2));
                            ManipulacaoXml.SetNode(doc, infNf, "vST", Formatacoes.TrataValorDecimal(nota.ValorIcmsSt, 2));
                            ManipulacaoXml.SetNode(doc, infNf, "vProd", Formatacoes.TrataValorDecimal(nota.TotalProd, 2));
                            ManipulacaoXml.SetNode(doc, infNf, "vNF", Formatacoes.TrataValorDecimal(nota.TotalNota, 2));
                            ManipulacaoXml.SetNode(doc, infNf, "nCFOP", "1206");

                            infDoc.AppendChild(infNf);
                        }                        
                    }

                    if (cte.TipoServico != (int)ConhecimentoTransporte.TipoServicoEnum.RedespachoIntermediario || infDoc.ChildNodes.Count > 0)
                        infCTeNorm.AppendChild(infDoc);
                }

                #region Informações do Rodoviário

                try
                {
                    XmlElement infModal = doc.CreateElement("infModal");
                    infModal.SetAttribute("versaoModal", ConfigCTe.VersaoModalRod);

                    XmlElement rodo = doc.CreateElement("rodo");

                    var objCteRod = ConhecimentoTransporteRodoviarioDAO.Instance.GetElement(idCte);

                    if (objCteRod != null)
                    {
                        if (string.IsNullOrEmpty(loja.RNTRC))
                            throw new Exception("O campo RNTRC deve ser preenchido no cadastro da loja " + loja.NomeFantasia);

                        ManipulacaoXml.SetNode(doc, rodo, "RNTRC", loja.RNTRC.PadLeft(8, '0'));
                    }

                    var objOrdemColetaCteRod = OrdemColetaCteRodDAO.Instance.GetOrdensColetaCte(idCte);
                    foreach (var i in objOrdemColetaCteRod)
                    {
                        var transportador = TransportadorDAO.Instance.GetElement(i.IdTransportador);

                        XmlElement occ = doc.CreateElement("occ");

                        if (!string.IsNullOrEmpty(i.Serie))
                            ManipulacaoXml.SetNode(doc, occ, "serie", i.Serie);

                        ManipulacaoXml.SetNode(doc, occ, "nOcc", i.Numero.ToString());
                        if(i.DataEmissao != null)
                            ManipulacaoXml.SetNode(doc, occ, "dEmi", Convert.ToDateTime(i.DataEmissao).ToString("yyyy-MM-dd"));
                        
                        XmlElement emiOcc = doc.CreateElement("emiOcc");

                        ManipulacaoXml.SetNode(doc, emiOcc, "CNPJ", Formatacoes.TrataStringDocFiscal(transportador.CpfCnpj).PadLeft(14, '0'));
                        ManipulacaoXml.SetNode(doc, emiOcc, "IE", transportador.InscEst);
                        ManipulacaoXml.SetNode(doc, emiOcc, "UF", transportador.Uf);

                        if (!string.IsNullOrEmpty(transportador.Telefone))
                            ManipulacaoXml.SetNode(doc, emiOcc, "fone", Formatacoes.TrataStringDocFiscal(transportador.Telefone, true));

                        occ.AppendChild(emiOcc);
                        rodo.AppendChild(occ);
                    }

                    var objValePedagioCteRod = ValePedagioCteRodDAO.Instance.GetValesPedagioCte(idCte);
                    foreach (var i in objValePedagioCteRod)
                    {
                        var fornecedor = FornecedorDAO.Instance.GetElement(i.IdFornec);

                        XmlElement valePed = doc.CreateElement("valePed");

                        ManipulacaoXml.SetNode(doc, valePed, "CNPJForn", Formatacoes.TrataStringDocFiscal(fornecedor.CpfCnpj).PadLeft(14, '0'));
                        ManipulacaoXml.SetNode(doc, valePed, "nCompra", i.NumeroCompra);

                        rodo.AppendChild(valePed);
                    }

                    var objLacreCteRod = LacreCteRodDAO.Instance.GetLacresByIdCte(idCte);
                    foreach (var i in objLacreCteRod)
                    {
                        XmlElement lacRodo = doc.CreateElement("lacRodo");
                        ManipulacaoXml.SetNode(doc, lacRodo, "nLacre", i.NumeroLacre);

                        rodo.AppendChild(lacRodo);
                    }
                    
                    infModal.AppendChild(rodo);
                    infCTeNorm.AppendChild(infModal);
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        string.Format("Falha ao inserir dados do modal rodoviário no XML. {0}",
                            ex.Message != null ? ex.Message : string.Empty), ex);
                }

                #endregion

                infCte.AppendChild(infCTeNorm);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir dados dos CT-e Normal e Substituto no XML.", ex);
            }

            #endregion

            #endregion

            #region Assina CTe

            try
            {
                if (!preVisualizar)
                {
                    MemoryStream stream = new MemoryStream();
                    doc.Save(stream);

                    using (stream)
                    {
                        // Classe responsável por assinar o xml do CTe
                        AssinaturaDigital AD = new AssinaturaDigital();

                        System.Security.Cryptography.X509Certificates.X509Certificate2 cert = Certificado.GetCertificado((uint)loja.IdLoja);
                        //System.Security.Cryptography.X509Certificates.X509Certificate2 cert = Certificado.BuscaNome("ADALBERTO FERREIRA DE SOUZA:07598913000109's AC FENACON Certisign RFB G2 ID", false);

                        if (DateTime.Now > cert.NotAfter)
                            throw new Exception("O certificado digital cadastrado está vencido, insira um novo certificado para emitir este CTe. Data Venc.: " + cert.GetExpirationDateString());

                        int resultado = AD.Assinar(ref doc, "infCte", cert);

                        if (resultado > 0)
                            throw new Exception(AD.mensagemResultado);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao assinar CTe." + ex.Message);
            }

            #endregion

            #region Valida XML

            try
            {
                if (!preVisualizar)
                    ValidaXML.Validar(doc, ValidaXML.TipoArquivoXml.CTe);
            }
            catch (Exception ex)
            {
                throw new Exception("XML inconsistente." + ex.Message);
            }

            #endregion

            #region Salva arquivo XML do CTe

            try
            {
                string fileName = Utils.GetCteXmlPath + doc["CTe"]["infCte"].GetAttribute("Id").Remove(0, 3) + "-cte.xml";

                if (File.Exists(fileName))
                    File.Delete(fileName);

                doc.Save(fileName);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao salvar arquivo xml do CTe. " + ex.Message);
            }

            #endregion

            return doc;
        }

        /// <summary>
        /// Cria XML do CTe para ser assinado e enviado para o webservice da receita
        /// </summary>
        /// <param name="idNf"></param>
        public XmlDocument EmitirCTe(uint idCte, bool preVisualizar)
        {
            FilaOperacoes.ConhecimentoTransporte.AguardarVez();

            try
            {
                #region Permite o envio do lote mais de uma vez somente em um intervalo maior que 2 minutos

                /* Chamado 43375. */
                if (ExecuteScalar<bool>(string.Format(
                    @"SELECT COUNT(*)>0 FROM log_cte lc
                        WHERE lc.IdCte={0} AND
                            lc.Codigo=103 AND
                            lc.DataHora>=DATE_ADD(NOW(), INTERVAL - 2 MINUTE)", idCte)))
                    throw new Exception("Lote em processamento.");

                #endregion

                XmlDocument doc = GerarXmlCte(idCte, preVisualizar);

                #region Envia CTE

                if (!preVisualizar)
                {
                    try
                    {
                        // Altera situação para processo de emissão
                        AlteraSituacao(idCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.ProcessoEmissao);

                        // Atualiza campo tipoAmbiente da NFe
                        //objPersistence.ExecuteCommand("Update nota_fiscal set tipoAmbiente=" + (int)ConfigNFe.TipoAmbiente + " where idNf=" + idNf);

                        // Envia CTe para SEFAZ
                        EnviaXML.EnviaCTe(doc, idCte);
                    }
                    catch (Exception ex)
                    {
                        // Altera situação para falha ao emitir
                        AlteraSituacao(idCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaEmitir);

                        throw ex;
                    }
                }

                #endregion

                return doc;
            }
            finally
            {
                FilaOperacoes.ConhecimentoTransporte.ProximoFila();
            }
        }

        #endregion

        #region Chave de Acesso CT-e

        /// <summary>
        /// Retorna chave de acesso do CTe
        /// </summary>
        /// <param name="cUf">Código da UF do emitente do CTe</param>
        /// <param name="aamm">Ano e Mês de emissão do CTe</param>
        /// <param name="cnpj">CNPJ do emitente</param>
        /// <param name="mod">Modelo do Documento Fiscal</param>
        /// <param name="serie">Série do Documento Fiscal</param>
        /// <param name="nCte">Número do Documento Fiscal</param>
        /// <param name="cCte">Código Numérico que compõe a Chave de Acesso</param>
        public string ChaveDeAcesso(string cUf, string aamm, string cnpj, string mod, string serie, string nCte, string tpEmis, string cCte)
        {
            if (!Glass.Validacoes.ValidaCnpj(cnpj))
                throw new Exception("CNPJ do emitente é inválido.");

            string chave = cUf + aamm + cnpj.Replace(".", "").Replace("/", "").Replace("-", "") + mod.PadLeft(2, '0') +
                serie.PadLeft(3, '0') + nCte.PadLeft(9, '0') + tpEmis + cCte.PadLeft(8, '0');

            if (chave.Length != 43)
                throw new Exception("Parâmetros da chave de acesso incorretos.");

            return chave + CalculaDV(chave, 4);
        }

        /// <summary>
        /// Calcula o dígito verificador para uma sequência numérica.
        /// </summary>
        /// <param name="textoCalcular"></param>
        /// <param name="pesoInicial"></param>
        /// <returns></returns>
        internal int CalculaDV(string textoCalcular, int pesoInicial)
        {
            int peso = pesoInicial, ponderacao = 0;

            for (int i = 0; i < textoCalcular.Length; i++)
            {
                ponderacao += Glass.Conversoes.StrParaInt(textoCalcular[i].ToString()) * peso--;

                if (peso == 1)
                    peso = 9;
            }

            // Calcula o resto da divisão da ponderação por 11
            int restoDiv = (ponderacao % 11);

            // Se o restoDiv for 0 ou 1, o dígito deverá ser 0
            return 11 - (restoDiv == 0 || restoDiv == 1 ? 11 : restoDiv);
        }

        #endregion
    }
}
