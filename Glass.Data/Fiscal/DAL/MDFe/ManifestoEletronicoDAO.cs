using GDA;
using Glass.Data.Helper;
using Glass.Data.MDFeUtils;
using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Glass.Data.DAL
{
    public class ManifestoEletronicoDAO : BaseDAO<ManifestoEletronico, ManifestoEletronicoDAO>
    {
        #region Busca padrão

        private string Sql(int idManifestoEletronico, int numeroManifestoEletronico, string situacoes, string uFInicio, string uFFim,
            string dataEmissaoIni, string dataEmissaoFim, int idLoja, int tipoContratante, int idContratante, bool selecionar)
        {
            var campos = selecionar ? "mdfe.*, se.NomeSeguradora" : "COUNT(*)";

            var sql = "SELECT " + campos + @"
                FROM manifesto_eletronico mdfe" +
                (idContratante > 0 ? " INNER JOIN participante_mdfe pmc ON(mdfe.IdManifestoEletronico=pmc.IdManifestoEletronico)" : "") +
                (idLoja > 0 ? " INNER JOIN participante_mdfe pme ON(mdfe.IdManifestoEletronico=pme.IdManifestoEletronico)" : "") +
                @" LEFT JOIN seguradora se ON (mdfe.IdSeguradora=se.IdSeguradora)
                WHERE 1 ";

            if (idManifestoEletronico > 0)
            {
                sql += " AND mdfe.IdManifestoEletronico=" + idManifestoEletronico;
            }

            if (numeroManifestoEletronico > 0)
            {
                sql += " AND mdfe.NumeroManifestoEletronico=" + numeroManifestoEletronico;
            }

            if (!string.IsNullOrWhiteSpace(situacoes))
            {
                sql += " AND mdfe.Situacao IN (" + situacoes + ")";
            }

            if (!string.IsNullOrWhiteSpace(uFInicio))
            {
                sql += " AND mdfe.UFInicio='" + uFInicio + "'";
            }

            if (!string.IsNullOrWhiteSpace(uFFim))
            {
                sql += " AND mdfe.UFFim='" + uFFim + "'";
            }

            if (!string.IsNullOrEmpty(dataEmissaoIni))
            {
                sql += " AND mdfe.DataEmissao>=?dataEmissaoIni";
                //criterio += "Período emissão : " + dataEmissaoIni + "    ";
            }

            if (!string.IsNullOrEmpty(dataEmissaoFim))
            {
                sql += " AND mdfe.DataEmissao<=?dataEmissaoFim";

                //if (!string.IsNullOrEmpty(dataEmissaoIni))
                //    criterio += " até " + dataEmissaoFim + "    ";
                //else
                //    criterio += "Período emissão: até " + dataEmissaoFim + "    ";
            }

            // Emitente sempre será a loja
            if (idLoja > 0)
            {
                sql += " AND (pme.TipoParticipante = 1 AND pme.IdLoja=" + idLoja + ")";
            }

            if (idContratante > 0)
            {
                switch (tipoContratante)
                {
                    case 0:
                        sql += " AND (pmc.TipoParticipante = 2 AND pmc.IdLoja=" + idContratante + ")";
                        break;
                    case 1:
                        sql += " AND (pmc.TipoParticipante = 2 AND pmc.IdFornecedor=" + idContratante + ")";
                        break;
                    case 2:
                        sql += " AND (pmc.TipoParticipante = 2 AND pmc.IdCliente=" + idContratante + ")";
                        break;
                    case 3:
                        sql += " AND (pmc.TipoParticipante = 2 AND pmc.IdTransportador=" + idContratante + ")";
                        break;
                }
            }

            return sql;
        }

        public IList<ManifestoEletronico> GetList(int numeroManifestoEletronico, string situacoes, string uFInicio, string uFFim,
            string dataEmissaoIni, string dataEmissaoFim, int idLoja, int tipoContratante, int idContratante, string sortExpression, int startRow, int pageSize)
        {
            sortExpression = string.IsNullOrEmpty(sortExpression) ? "mdfe.NumeroManifestoEletronico DESC" : sortExpression;

            return LoadDataWithSortExpression(Sql(0, numeroManifestoEletronico, situacoes, uFInicio, uFFim, dataEmissaoIni, dataEmissaoFim, idLoja, tipoContratante, idContratante, true),
                sortExpression, startRow, pageSize, GetParams(dataEmissaoIni, dataEmissaoFim));
        }

        public int GetCount(int numeroManifestoEletronico, string situacoes, string uFInicio, string uFFim,
            string dataEmissaoIni, string dataEmissaoFim, int idLoja, int tipoContratante, int idContratante)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, numeroManifestoEletronico, situacoes, uFInicio, uFFim, dataEmissaoIni, dataEmissaoFim, idLoja, tipoContratante, idContratante, false),
                GetParams(dataEmissaoIni, dataEmissaoFim));
        }

        private GDAParameter[] GetParams(string dataEmissaoIni, string dataEmissaoFim)
        {
            List<GDAParameter> lst = new List<GDAParameter>();

            if (!string.IsNullOrEmpty(dataEmissaoIni))
                lst.Add(new GDAParameter("?dataEmissaoIni", DateTime.Parse(dataEmissaoIni + " 00:00:00")));

            if (!string.IsNullOrEmpty(dataEmissaoFim))
                lst.Add(new GDAParameter("?dataEmissaoFim", DateTime.Parse(dataEmissaoFim + " 23:59:59")));

            return lst.ToArray();
        }

        public ManifestoEletronico ObterManifestoEletronicoPeloId(int idManifestoEletronico)
        {
            return objPersistence.LoadOneData(Sql(idManifestoEletronico, 0, null, null, null, null, null, 0, 0, 0, true));
        }

        public ManifestoEletronico ObterPelaChaveAcesso(string chaveAcesso)
        {
            return objPersistence.LoadOneData("SELECT * FROM manifesto_eletronico WHERE ChaveAcesso=?chaveAcesso",
                new GDAParameter("?chaveAcesso", chaveAcesso));
        }

        #endregion

        #region Métodos Sobrescritos

        public uint InsertComTransacao(ManifestoEletronico objInsert)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Insert(transaction, objInsert);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        public override uint Insert(GDASession session, ManifestoEletronico objInsert)
        {
            var idLoja = objInsert.Participantes.Where(f => f.TipoParticipante == TipoParticipanteEnum.Emitente).FirstOrDefault().IdLoja.GetValueOrDefault();
            var lojaEmitente = LojaDAO.Instance.GetElementByPrimaryKey(idLoja);
            var cidadeLojaEmitente = CidadeDAO.Instance.GetElementByPrimaryKey((uint)lojaEmitente.IdCidade);

            // Validações
            if (idLoja == 0)
                throw new Exception("Não foi informado um Emitente válido para esse MDFe");

            if (objInsert.Modelo == 0)
                throw new Exception("Não foi informado modelo para o MDFe");

            if (objInsert.Serie == 0)
                throw new Exception("Não foi informado série para o MDFe");

            if (objInsert.ValorCarga == 0)
                throw new Exception("Não foi informado valor total para o MDFe");

            if (objInsert.QuantidadeCarga == 0)
                throw new Exception("Não foi informado peso bruto para o MDFe");

            objInsert.DataCad = DateTime.Now;
            objInsert.Usucad = UserInfo.GetUserInfo.CodUser;
            objInsert.NumeroManifestoEletronico = ObterUltimoNumeroMDFe(session, idLoja, objInsert.Serie);
            objInsert.Situacao = SituacaoEnum.Aberto;
            objInsert.DataEmissao = DateTime.Now;
            objInsert.CodigoAleatorio = (objInsert.NumeroManifestoEletronico + (objInsert.TipoEmissao == TipoEmissao.Normal ? 10203040 : 9020304));
            objInsert.ChaveAcesso = ChaveDeAcesso(cidadeLojaEmitente.CodIbgeUf, objInsert.DataEmissao.ToString("yyMM"), LojaDAO.Instance.ObtemCnpj(session, (uint)idLoja), Glass.Data.MDFeUtils.ConfigMDFe.Modelo.ToString(),
                                objInsert.Serie.ToString().PadLeft(3, '0'), objInsert.NumeroManifestoEletronico.ToString(), ((int)objInsert.TipoEmissao).ToString(), objInsert.CodigoAleatorio.ToString());

            // Salva o MDFe
            var idManifestoEletronico = (int)base.Insert(session, objInsert);

            #region Filhos do MDFe

            //Salva Cidade Carga
            if (objInsert.CidadesCarga != null)
            {
                foreach (var cc in objInsert.CidadesCarga)
                {
                    cc.IdManifestoEletronico = idManifestoEletronico;
                    CidadeCargaMDFeDAO.Instance.Insert(session, cc);
                }
            }

            // Salva UF Percurso
            if (objInsert.UFsPercurso != null)
            {
                foreach (var uf in objInsert.UFsPercurso)
                {
                    uf.IdManifestoEletronico = idManifestoEletronico;
                    UFPercursoMDFeDAO.Instance.Insert(session, uf);
                }
            }

            // Salva Participante
            if (objInsert.Participantes != null)
            {
                foreach (var p in objInsert.Participantes)
                {
                    p.IdManifestoEletronico = idManifestoEletronico;
                    ParticipanteMDFeDAO.Instance.Insert(session, p);
                }
            }

            // Salva Rodoviario
            if (objInsert.Rodoviario != null)
            {
                objInsert.Rodoviario.IdManifestoEletronico = idManifestoEletronico;
                RodoviarioMDFeDAO.Instance.Insert(session, objInsert.Rodoviario);
            }

            //Salva Averbação Seguro
            if (objInsert.AverbacaoSeguro != null)
            {
                foreach (var avs in objInsert.AverbacaoSeguro)
                {
                    avs.IdManifestoEletronico = idManifestoEletronico;
                    AverbacaoSeguroMDFeDAO.Instance.Insert(session, avs);
                }
            }

            #endregion

            return (uint)idManifestoEletronico;
        }

        public int UpdateComTransacao(ManifestoEletronico objUpdate)
        {
            using (var transaction = new GDATransaction())
            {
                try
                {
                    transaction.BeginTransaction();

                    var retorno = Update(transaction, objUpdate);

                    transaction.Commit();
                    transaction.Close();

                    return retorno;
                }
                catch
                {
                    transaction.Rollback();
                    transaction.Close();
                    throw;
                }
            }
        }

        public override int Update(GDASession session, ManifestoEletronico objUpdate)
        {
            var idManifestoEletronico = objUpdate.IdManifestoEletronico;

            var idLoja = objUpdate.Participantes.Where(f => f.TipoParticipante == TipoParticipanteEnum.Emitente).FirstOrDefault().IdLoja.GetValueOrDefault();
            var lojaEmitente = LojaDAO.Instance.GetElementByPrimaryKey(idLoja);
            var cidadeLojaEmitente = CidadeDAO.Instance.GetElementByPrimaryKey((uint)lojaEmitente.IdCidade);

            // Validações
            if (idLoja == 0)
                throw new Exception("Não foi informado um Emitente válido para esse MDFe");

            if (objUpdate.Modelo == 0)
                throw new Exception("Não foi informado modelo para o MDFe");

            if (objUpdate.Serie == 0)
                throw new Exception("Não foi informado série para o MDFe");

            if (objUpdate.ValorCarga == 0)
                throw new Exception("Não foi informado valor total para o MDFe");

            if (objUpdate.QuantidadeCarga == 0)
                throw new Exception("Não foi informado peso bruto para o MDFe");

            objUpdate.ChaveAcesso = ChaveDeAcesso(cidadeLojaEmitente.CodIbgeUf, objUpdate.DataEmissao.ToString("yyMM"), LojaDAO.Instance.ObtemCnpj(session, (uint)idLoja), Glass.Data.MDFeUtils.ConfigMDFe.Modelo.ToString(),
                                objUpdate.Serie.ToString().PadLeft(3, '0'), objUpdate.NumeroManifestoEletronico.ToString(), ((int)objUpdate.TipoEmissao).ToString(), objUpdate.CodigoAleatorio.ToString());

            var resultado = base.Update(session, objUpdate);

            #region Filhos do MDFe

            //Salva Cidade Carga
            CidadeCargaMDFeDAO.Instance.DeletarPorIdManifestoEletronico(session, idManifestoEletronico);
            if (objUpdate.CidadesCarga != null)
            {
                foreach (var cc in objUpdate.CidadesCarga)
                {
                    cc.IdManifestoEletronico = idManifestoEletronico;
                    CidadeCargaMDFeDAO.Instance.Insert(session, cc);
                }
            }

            // Salva UF Percurso
            UFPercursoMDFeDAO.Instance.DeletarPorIdManifestoEletronico(session, idManifestoEletronico);
            if (objUpdate.UFsPercurso != null)
            {
                foreach (var uf in objUpdate.UFsPercurso)
                {
                    uf.IdManifestoEletronico = idManifestoEletronico;
                    UFPercursoMDFeDAO.Instance.Insert(session, uf);
                }
            }

            // Salva Participante
            ParticipanteMDFeDAO.Instance.DeletarPorIdManifestoEletronico(session, idManifestoEletronico);
            if (objUpdate.Participantes != null)
            {
                foreach (var p in objUpdate.Participantes)
                {
                    p.IdManifestoEletronico = idManifestoEletronico;
                    ParticipanteMDFeDAO.Instance.Insert(session, p);
                }
            }

            // Salva Rodoviario
            if (objUpdate.Rodoviario != null)
            {
                RodoviarioMDFeDAO.Instance.Update(session, objUpdate.Rodoviario);
            }

            //Salva Averbação Seguro
            AverbacaoSeguroMDFeDAO.Instance.DeletarPorIdManifestoEletronico(session, idManifestoEletronico);
            if (objUpdate.AverbacaoSeguro != null)
            {
                foreach (var avs in objUpdate.AverbacaoSeguro)
                {
                    avs.IdManifestoEletronico = idManifestoEletronico;
                    AverbacaoSeguroMDFeDAO.Instance.Insert(session, avs);
                }
            }

            #endregion

            return resultado;
        }

        public int AtualizaMotivoCancelamento(int idManifestoEletronico, string justificativa)
        {
            return objPersistence.ExecuteCommand("UPDATE manifesto_eletronico SET MotivoCancelamento=?justificativa WHERE IdManifestoEletronico=" + idManifestoEletronico,
                new GDAParameter("?justificativa", justificativa));
        }

        #endregion

        #region Obter Valor dos campos

        public int ObterUltimoNumeroMDFe(GDASession sessao, int idLoja, int serie)
        {
            var sql = @"SELECT COALESCE(MAX(mdfe.NumeroManifestoEletronico), 0) + 1
                FROM manifesto_eletronico mdfe
                INNER JOIN participante_mdfe pm ON (mdfe.IdManifestoEletronico=pm.IdManifestoEletronico)
                WHERE pm.IdLoja={0} AND mdfe.Serie={1}";

            return Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sessao, string.Format(sql, idLoja, serie)).ToString());
        }

        public string ObterChaveAcesso(GDASession sessao, int idManifestoEletronico)
        {
            return ObtemValorCampo<string>(sessao, "ChaveAcesso", "IdManifestoEletronico=" + idManifestoEletronico);
        }

        public TipoEmissao ObterTipoEmissao(GDASession sessao, int idManifestoEletronico)
        {
            return ObtemValorCampo<TipoEmissao>(sessao, "TipoEmissao", "IdManifestoEletronico=" + idManifestoEletronico);
        }

        public string ObterMotivoCancelamento(GDASession sessao, int idManifestoEletronico)
        {
            return ObtemValorCampo<string>(sessao, "MotivoCancelamento", "IdManifestoEletronico=" + idManifestoEletronico);
        }

        public SituacaoEnum ObterSituacaoManifestoEletronico(GDASession sessao, int idManifestoEletronico)
        {
            return ObtemValorCampo<SituacaoEnum>(sessao, "Situacao", "IdManifestoEletronico=" + idManifestoEletronico);
        }

        #endregion

        #region Gera um número de lote

        /// <summary>
        /// Atualiza o MDFe passado com um número de lote ainda não utilizado, retornado-o
        /// </summary>
        /// <param name="idManifestoEletronico"></param>
        /// <returns></returns>
        public int ObterNovoNumLote(int idManifestoEletronico)
        {
            // Verifica se esse MDFe já possui um número de lote
            string sql = "SELECT COALESCE(NUMLOTE, 0) FROM manifesto_eletronico WHERE IdManifestoEletronico=" + idManifestoEletronico;

            int numLote = Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sql).ToString());

            if (numLote > 0)
                return numLote;

            // Se este MDFe não possuir um número de lote, cria um novo número para o mesmo
            sql = "SELECT COALESCE(MAX(NUMLOTE)+1, 1) FROM manifesto_eletronico WHERE IdManifestoEletronico<>" + idManifestoEletronico;

            numLote = Glass.Conversoes.StrParaInt(objPersistence.ExecuteScalar(sql).ToString());

            objPersistence.ExecuteCommand("UPDATE manifesto_eletronico SET NUMLOTE=" + numLote + " WHERE IdManifestoEletronico=" + idManifestoEletronico);

            return numLote;
        }

        #endregion

        #region Altera valor dos campos

        /// <summary>
        /// Altera situação do MDFe
        /// </summary>
        public int AlteraSituacao(int idManifestoEletronico, SituacaoEnum situacao)
        {
            return AlteraSituacao(null, idManifestoEletronico, situacao);
        }

        /// <summary>
        /// Altera situação do MDFe
        /// </summary>
        public int AlteraSituacao(GDASession session, int idManifestoEletronico, SituacaoEnum situacao)
        {
            string sql = "UPDATE manifesto_eletronico SET Situacao=" + (int)situacao + " WHERE IdManifestoEletronico=" + idManifestoEletronico;

            return objPersistence.ExecuteCommand(session, sql);
        }

        public int AlteraTipoEmissao(GDASession session, int idManifestoEletronico, TipoEmissao tipoEmissao)
        {
            string sql = "UPDATE manifesto_eletronico SET TipoEmissao=" + (int)tipoEmissao + " WHERE IdManifestoEletronico=" + idManifestoEletronico;

            return objPersistence.ExecuteCommand(session, sql);
        }

        #endregion

        #region Retorno da emissão do MDFe

        /// <summary>
        /// Lote enviado com sucesso (salva número de recibo na tabela protocolo)
        /// </summary>
        /// <param name="idManifestoEletronico"></param>
        /// <param name="numRecibo"></param>
        public void SalvaReciboProtocoloAutorizacao(int idManifestoEletronico, string numRecibo)
        {
            var protocolo = ProtocoloMDFeDAO.Instance.GetElement(idManifestoEletronico, (int)ProtocoloMDFe.TipoProtocoloEnum.Autorizacao);
            // Adiciona na tabela protocolo, o número do recibo do lote do mdfe, para consultá-lo posteriormente
            if (protocolo != null)
            {
                ProtocoloMDFeDAO.Instance.Update(new ProtocoloMDFe
                {
                    DataCad = DateTime.Now,
                    IdManifestoEletronico = idManifestoEletronico,
                    NumRecibo = numRecibo,
                    TipoProtocolo = (int)ProtocoloMDFe.TipoProtocoloEnum.Autorizacao
                });
            }
            else if (!string.IsNullOrEmpty(numRecibo))
            {
                ProtocoloMDFeDAO.Instance.Insert(new ProtocoloMDFe
                {
                    DataCad = DateTime.Now,
                    IdManifestoEletronico = idManifestoEletronico,
                    NumRecibo = numRecibo,
                    TipoProtocolo = (int)ProtocoloMDFe.TipoProtocoloEnum.Autorizacao
                });
            }
        }

        /// <summary>
        /// Retorno da consulta de emissão do MDFe
        /// </summary>
        /// <param name="chaveAcesso"></param>
        /// <param name="xmlProt"></param>
        public string RetornoEmissaoMDFe(int idManifestoEletronico, XmlNode xmlProt)
        {
            return RetornoEmissaoMDFe(idManifestoEletronico, xmlProt, Utils.GetMDFeXmlPath);
        }

        /// <summary>
        /// Retorno da consulta de emissão do MDFe
        /// </summary>
        /// <param name="chaveAcesso"></param>
        /// <param name="xmlProt"></param>
        /// <param name="mdfePath"></param>
        public string RetornoEmissaoMDFe(int idManifestoEletronico, XmlNode xmlProt, string mdfePath)
        {
            var mdfe = ObterManifestoEletronicoPeloId(idManifestoEletronico);

            // Se o MDFe já tiver sido autorizado, não faz nada
            if (mdfe.Situacao == Glass.Data.Model.SituacaoEnum.Autorizado)
                return "MDFe já autorizado.";

            //Código do status da resposta
            var cStat = Conversoes.StrParaInt(xmlProt["infProt"]["cStat"].InnerXml);
            var xMotivo = xmlProt["infProt"]["xMotivo"].InnerXml;

            // Gera log do ocorrido
            LogMDFeDAO.Instance.NewLog(mdfe.IdManifestoEletronico, "Emissão", cStat, ConsultaSituacao.CustomizaMensagemRejeicao(mdfe.IdManifestoEletronico, xMotivo));

            // Atualiza número do protocolo de uso do MDFe
            if ((cStat == 100) && xmlProt["infProt"]["nProt"] != null)
            {
                AutorizacaoMDFe(mdfe, xmlProt, mdfePath);
                return xMotivo;
            }
            // Se o código de retorno da emissão for > 203, algum erro ocorreu, altera situação do MDFe para Falha ao Emitir
            else if (cStat >= 203)
            {
                if (mdfe.Situacao != SituacaoEnum.ContingenciaOffline)
                    AlteraSituacao(mdfe.IdManifestoEletronico, Glass.Data.Model.SituacaoEnum.FalhaEmitir);

                return xMotivo;
            }

            return xMotivo;
        }

        /// <summary>
        /// Procedimentos que devem ser realizados ao autorizar o MDFe
        /// </summary>
        /// <param name="mdfe"></param>
        /// <param name="xmlProt"></param>
        private void AutorizacaoMDFe(ManifestoEletronico mdfe, XmlNode xmlProt, string mdfePath)
        {
            // Salva protocolo de autorização
            ProtocoloMDFeDAO.Instance.Update(mdfe.IdManifestoEletronico, xmlProt["infProt"]["nProt"].InnerXml);

            LogMDFeDAO.Instance.NewLog(mdfe.IdManifestoEletronico, "Protocolo MDFe", 0, "Protocolo atualizado.");

            // Altera situação do MDFe para autorizado
            AlteraSituacao(mdfe.IdManifestoEletronico, SituacaoEnum.Autorizado);

            LogMDFeDAO.Instance.NewLog(mdfe.IdManifestoEletronico, "Situação", 0, "Situação atualizada.");

            #region Anexa protocolo de autorização ao arquivo XML do MDFe no servidor

            // Anexa protocolo de autorização ao MDFe
            var path = mdfePath + mdfe.ChaveAcesso + "-mdfe.xml";
            IncluiProtocoloXML(path, xmlProt);

            #endregion
        }

        #endregion

        #region Retorno da consulta do lote do MDFe

        public string RetornoConsSitMDFe(int idManifestoEletronico, XmlNode xmlRetConsSit)
        {
            if (xmlRetConsSit == null)
                throw new Exception("Falha ao comunicar com webservice da SEFAZ.");

            var mdfe = ManifestoEletronicoDAO.Instance.ObterManifestoEletronicoPeloId(idManifestoEletronico);

            // Se o MDFe já tiver sido autorizado, não faz nada
            if (mdfe.Situacao == SituacaoEnum.Autorizado)
                return string.Empty;

            //Código do status da resposta
            var cStat = Conversoes.StrParaInt(xmlRetConsSit["cStat"].InnerXml);
            var xMotivo = xmlRetConsSit["xMotivo"].InnerXml;

            // Gera log do ocorrido
            LogMDFeDAO.Instance.NewLog(mdfe.IdManifestoEletronico, "Consulta", cStat, xMotivo);

            // Atualiza número do protocolo de uso do MDFe
            if (cStat == 100)
                AutorizacaoMDFe(mdfe, xmlRetConsSit["protMDFe"], Utils.GetCteXmlPath);
            else if (cStat == 101)
                AlteraSituacao(mdfe.IdManifestoEletronico, SituacaoEnum.Cancelado);
            else if (cStat == 132)
                AlteraSituacao(mdfe.IdManifestoEletronico, SituacaoEnum.Encerrado);
            else if (cStat == 609) // MDFe já está Encerrado
                AlteraSituacao(mdfe.IdManifestoEletronico, SituacaoEnum.Encerrado);
            else if (cStat == 218) // MDFe já está Cancelado
                AlteraSituacao(mdfe.IdManifestoEletronico, SituacaoEnum.Cancelado);
            else if (cStat == 220) // MDFe já está autorizado há mais de 24 horas
                AlteraSituacao(mdfe.IdManifestoEletronico, SituacaoEnum.Autorizado);

            // Se o código de retorno da emissão for igual a 106, 137 ou >= 203, algum erro ocorreu, altera situação do MDFe para Falha ao Emitir
            else if (cStat == 106 || cStat == 137 || cStat >= 203)
            {
                if (mdfe.Situacao != SituacaoEnum.ContingenciaOffline)
                    AlteraSituacao(mdfe.IdManifestoEletronico, SituacaoEnum.FalhaEmitir);

                var msgErro = "Falha na consulta. ";
                // MDFe rejeitado
                if (cStat == 215 || cStat == 243 || cStat == 630)
                    msgErro += "Mensagem de consulta inválida. ";

                return msgErro + cStat + " - " + ConsultaSituacao.CustomizaMensagemRejeicao(mdfe.IdManifestoEletronico, xMotivo);
            }

            // Se a situação foi atualizada ou se nenhuma correspondencia foi encontrada, retorna a mensagem para o usuário
            return xMotivo;
        }

        #endregion

        #region Retorno do Evento do MDFe

        /// <summary>
        /// Retorno do Evento da MDFe, grava log e altera situação da MDFe
        /// </summary>
        /// <param name="idManifestoEletronico"></param>
        /// <param name="xmlRetEvento"></param>
        /// <param name="tipoEvento"></param>
        /// <returns></returns>
        public string RetornoEventoMDFe(int idManifestoEletronico, XmlNode xmlRetEvento, string tipoEvento)
        {
            SituacaoEnum situacaoFalha = SituacaoEnum.Falha;
            var descricaoEvento = string.Empty;
            switch (tipoEvento)
            {
                case "110111":
                    situacaoFalha = SituacaoEnum.FalhaCancelar;
                    descricaoEvento = "Cancelamento";
                    break;
                case "110112":
                    situacaoFalha = SituacaoEnum.FalhaEncerrar;
                    descricaoEvento = "Encerramento";
                    break;
            }

            // Se o xml de retorno for nulo, ocorreu alguma falha no processo
            if (xmlRetEvento == null)
            {
                LogMDFeDAO.Instance.NewLog(idManifestoEletronico, descricaoEvento, 1, "Falha no processo do MDFe. ");
                AlteraSituacao(idManifestoEletronico, situacaoFalha);
                throw new Exception("Servidor da SEFAZ não respondeu em tempo hábil, tente novamente.");
            }

            try
            {
                // Lê Xml de retorno do envio do lote                
                var cStat = Conversoes.StrParaInt(xmlRetEvento["infEvento"]["cStat"].InnerText);
                string xMotivo = xmlRetEvento["infEvento"]["xMotivo"].InnerText;

                // Insere o log do Evento deste MDFe
                LogMDFeDAO.Instance.NewLog(idManifestoEletronico, descricaoEvento, cStat, xMotivo);

                // Se o código de retorno for 101-Cancelamento de MDFe homologado, altera situação para cancelada
                if (cStat == 101)
                {
                    // Insere protocolo do Evento no MDFe
                    ProtocoloMDFeDAO.Instance.Insert(new ProtocoloMDFe
                    {
                        IdManifestoEletronico = idManifestoEletronico,
                        DataCad = DateTime.Now,
                        NumProtocolo = xmlRetEvento["infEvento"]["nProt"].InnerText,
                        TipoProtocolo = (int)ProtocoloMDFe.TipoProtocoloEnum.Cancelamento
                    });

                    // Altera situação do MDFe para Cancelado
                    AlteraSituacao(idManifestoEletronico, SituacaoEnum.Cancelado);
                    LogMDFeDAO.Instance.NewLog(idManifestoEletronico, "Situação", 0, "Situação atualizada para Cancelado.");

                    return xMotivo;
                }
                // Se o código de retorno for 132-Encerramento de MDFe homologado, altera situação para encerrado
                else if (cStat == 132)
                {
                    // Insere protocolo do Evento no MDFe
                    ProtocoloMDFeDAO.Instance.Insert(new ProtocoloMDFe
                    {
                        IdManifestoEletronico = idManifestoEletronico,
                        DataCad = DateTime.Now,
                        NumProtocolo = xmlRetEvento["infEvento"]["nProt"].InnerText,
                        TipoProtocolo = (int)ProtocoloMDFe.TipoProtocoloEnum.Encerramento
                    });

                    // Altera situação do MDFe para Cancelado
                    AlteraSituacao(idManifestoEletronico, SituacaoEnum.Encerrado);
                    LogMDFeDAO.Instance.NewLog(idManifestoEletronico, "Situação", 0, "Situação atualizada para Encerrado.");

                    return xMotivo;
                }
                // Evento registrado e vinculado ao MDF-e
                else if (cStat == 135)
                {
                    // Chama o evento de consulta situação para retornar a mensagem real do evento e atualizar a situação no MDFe
                    return ConsultaSituacao.ConsultaSitMDFe(idManifestoEletronico);
                }
                else if (cStat == 609) // MDFe já está Encerrado
                    AlteraSituacao(idManifestoEletronico, SituacaoEnum.Encerrado);
                else if (cStat == 218) // MDFe já está cancelado
                    AlteraSituacao(idManifestoEletronico, SituacaoEnum.Cancelado);
                else if (cStat == 220) // MDFe já está autorizado há mais de 24 horas
                    AlteraSituacao(idManifestoEletronico, SituacaoEnum.Autorizado);
                // Se o código de retorno da emissão for igual a 106, 137 ou >= 203, algum erro ocorreu, altera situação do MDFe para Falha ao Emitir
                else if (cStat == 106 || cStat == 137 || cStat >= 203)
                {
                    AlteraSituacao(idManifestoEletronico, SituacaoEnum.FalhaEmitir);
                    return string.Format("Falha no {0} do MDFe. {1}", descricaoEvento, xMotivo);
                }

                return xMotivo;
            }
            catch
            {
                LogMDFeDAO.Instance.NewLog(idManifestoEletronico, descricaoEvento, 1, "Falha ao processar retorno do MDFe. ");

                AlteraSituacao(idManifestoEletronico, situacaoFalha);

                throw new Exception("Falha ao processar retorno, tente novamente.");
            }
        }

        #endregion

        #region Retorno Consulta Não Encerrados

        public string RetornoConsultaNaoEncerrados(XmlNode xmlRetConsNaoEnc)
        {
            // Se o xml de retorno for nulo, ocorreu alguma falha no processo
            if (xmlRetConsNaoEnc == null)
            {
                throw new Exception("Servidor da SEFAZ não respondeu em tempo hábil, tente novamente.");
            }

            try
            {
                var cStat = Conversoes.StrParaInt(xmlRetConsNaoEnc["cStat"].InnerText);
                var xMotivo = xmlRetConsNaoEnc["xMotivo"].InnerText;

                // MDF-e não encerrados localizados
                if (cStat == 111)
                {
                    var chaveAcesso = string.Empty;
                    // Adiciona a mensagem do webService a string de resposta.
                    var retorno = xMotivo + Environment.NewLine;
                    XmlNodeList infMDFeList = ((XmlElement)xmlRetConsNaoEnc).GetElementsByTagName("infMDFe");

                    // Para cada MDFe
                    foreach (XmlNode infMDFe in infMDFeList)
                    {
                        chaveAcesso = infMDFe["chMDFe"].InnerText;
                        // Adiciona o número e a chave de acesso do MDFe não encerrado a string de resposta.
                        retorno += "Nº " + chaveAcesso.Substring(25, 9).TrimStart('0') + " Chave: " + chaveAcesso + Environment.NewLine;
                    }

                    return retorno;
                }
                // MDF-e não encerrados NÃO localizados
                else if (cStat == 112)
                {
                    return xMotivo;
                }

                return xMotivo;
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao processar retorno, tente novamente.", ex);
            }
        }

        #endregion

        #region Inclusão de protocolo de autorização do MDFe

        /// <summary>
        /// Inclui o protocolo de autorização no MDFe.
        /// </summary>
        /// <param name="path">O caminho do arquivo no servidor.</param>
        /// <param name="xmlProt">O XML que será adicionado ao fim do MDFe.</param>
        public void IncluiProtocoloXML(string path, XmlNode xmlProt)
        {
            if (!File.Exists(path))
                return;

            // Carrega o conteúdo do arquivo XML do MDFe
            string conteudoArquivoMDFe = "";
            using (FileStream arquivoMDFe = File.OpenRead(path))
            using (StreamReader textoArquivoMDFe = new StreamReader(arquivoMDFe))
                conteudoArquivoMDFe = textoArquivoMDFe.ReadToEnd();

            // Salva o texto do arquivo XML junto com o texto da autorização do MDFe
            conteudoArquivoMDFe = conteudoArquivoMDFe.Insert(conteudoArquivoMDFe.IndexOf("<Signature"), xmlProt.InnerXml);
            using (FileStream arquivoMDFe = File.OpenWrite(path))
            using (StreamWriter salvaArquivoMDFe = new StreamWriter(arquivoMDFe))
            {
                salvaArquivoMDFe.Write(conteudoArquivoMDFe);
                salvaArquivoMDFe.Flush();
            }
        }

        #endregion  

        #region Gerar XML do MDFe para emissão

        /// <summary>
        /// Gera XML do MDFe e envia para autorização.
        /// </summary>
        /// <param name="idManifestoEletronico"></param>
        /// <param name="preVisualizar"></param>
        /// <param name="offline"></param>
        /// <returns></returns>
        public string EmitirMDFe(int idManifestoEletronico, bool preVisualizar)
        {
            FilaOperacoes.ManifestoEletronico.AguardarVez();

            try
            {
                #region Permite o envio do lote mais de uma vez somente em um intervalo maior que 2 minutos

                /* Chamado 43375. */
                if (ExecuteScalar<bool>(string.Format(
                    @"SELECT COUNT(*)>0 FROM log_mdfe lm
                        WHERE lm.IdManifestoEletronico={0} AND
                            lm.Codigo=103 AND
                            lm.DataHora>=DATE_ADD(NOW(), INTERVAL - 2 MINUTE)", idManifestoEletronico)))
                    throw new Exception("Lote em processamento.");

                #endregion

                XmlDocument doc = GerarXmlMDFe(idManifestoEletronico, preVisualizar);

                #region Envia MDFe

                if (!preVisualizar)
                {
                    try
                    {
                        // Altera situação para processo de emissão
                        AlteraSituacao(idManifestoEletronico, SituacaoEnum.ProcessoEmissao);

                        // Envia MDFe para SEFAZ
                        return EnviaXML.EnviaMDFe(doc, idManifestoEletronico);
                    }
                    catch (Exception ex)
                    {
                        AlteraSituacao(idManifestoEletronico, SituacaoEnum.FalhaEmitir);
                        throw ex;
                    }
                }

                #endregion

                return string.Empty;
            }
            finally
            {
                FilaOperacoes.ManifestoEletronico.ProximoFila();
            }
        }

        /// <summary>
        /// Gera o XML do MDFe para impressão em contingência e altera a situação para Contingência Offline
        /// Depois de impresso o XML não pode mais ser alterado, pois os dados da impressão devem ser idênticos ao XML enviado a receita
        /// </summary>
        /// <param name="idManifestoEletronico"></param>
        public string ImprimirMDFeContingencia(int idManifestoEletronico)
        {
            FilaOperacoes.ManifestoEletronico.AguardarVez();

            try
            {
                AlteraTipoEmissao(null, idManifestoEletronico, TipoEmissao.Contingencia);

                if (ObterTipoEmissao(null, idManifestoEletronico) != TipoEmissao.Contingencia && ObterSituacaoManifestoEletronico(null, idManifestoEletronico) != SituacaoEnum.Aberto &&
                    ObterSituacaoManifestoEletronico(null, idManifestoEletronico) != SituacaoEnum.FalhaEmitir)
                    throw new Exception("Apenas MDF-e de contingência, em aberto ou com falha ao emitir pode ser impresso em contingência offline.");

                GerarXmlMDFe(idManifestoEletronico, false);

                // Gera log do ocorrido
                LogMDFeDAO.Instance.NewLog(idManifestoEletronico, "Contingência Offline", 0, "MDFe impresso em Contigência Offline.");

                // Altera situação para contingência offline
                AlteraSituacao(idManifestoEletronico, SituacaoEnum.ContingenciaOffline);

                return "MDFe impresso em contingência offline. Obrigatória a autorização em 168 horas após esta impressão.";
            }
            finally
            {
                FilaOperacoes.ManifestoEletronico.ProximoFila();
            }
        }

        /// <summary>
        /// Envia para autorização o XML do MDFe gerado anteriormente.
        /// </summary>
        /// <param name="idManifestoEletronico"></param>
        public string EmitirMDFeOffline(int idManifestoEletronico)
        {
            FilaOperacoes.ManifestoEletronico.AguardarVez();

            try
            {
                var chaveAcesso = ObterChaveAcesso(null, idManifestoEletronico);
                var context = System.Web.HttpContext.Current;

                // Verifica se o MDFe existe
                if (!File.Exists(Utils.GetMDFeXmlPathInternal(context) + chaveAcesso + "-mdfe.xml"))
                    throw new Exception("Arquivo do MDFe não encontrado.");

                // Carrega o arquivo XML
                XmlDocument xmlMDFe = new XmlDocument();
                xmlMDFe.Load(Utils.GetMDFeXmlPathInternal(context) + chaveAcesso + "-mdfe.xml");

                // Envia MDFe para SEFAZ
                return EnviaXML.EnviaMDFe(xmlMDFe, idManifestoEletronico);
            }
            finally
            {
                FilaOperacoes.ManifestoEletronico.ProximoFila();
            }
        }

        private XmlDocument GerarXmlMDFe(int idManifestoEletronico, bool preVisualizar)
        {
            var mdfe = ObterManifestoEletronicoPeloId(idManifestoEletronico);

            if (mdfe.Situacao == SituacaoEnum.ContingenciaOffline)
                throw new Exception("MDFe impresso em contingência não pode ser alterado.");

            var participanteEmitente = mdfe.Participantes.Where(f => f.TipoParticipante == TipoParticipanteEnum.Emitente).FirstOrDefault();
            if (participanteEmitente.IdLoja == null)
                throw new Exception("MDFe não possui Emitente.");
            var lojaEmitente = LojaDAO.Instance.GetElementByPrimaryKey((uint)participanteEmitente.IdLoja);
            var cidadeLojaEmitente = CidadeDAO.Instance.GetElementByPrimaryKey((uint)lojaEmitente.IdCidade);

            var contratante = mdfe.Participantes.Where(p => p.TipoParticipante == TipoParticipanteEnum.Contratante).FirstOrDefault();
            var seguradora = mdfe.IdSeguradora > 0 ? CTe.SeguradoraDAO.Instance.GetElement((uint)mdfe.IdSeguradora) : null;

            mdfe.DataEmissao = DateTime.Now;
            var dataInicioViagem = new DateTimeOffset(mdfe.DataInicioViagem);
            var dataEmissao = new DateTimeOffset(mdfe.DataEmissao);
            mdfe.ChaveAcesso = ChaveDeAcesso(cidadeLojaEmitente.CodIbgeUf, dataEmissao.ToString("yyMM"), lojaEmitente.Cnpj, Glass.Data.MDFeUtils.ConfigMDFe.Modelo.ToString(),
                mdfe.Serie.ToString().PadLeft(3, '0'), mdfe.NumeroManifestoEletronico.ToString(), ((int)mdfe.TipoEmissao).ToString(), mdfe.CodigoAleatorio.ToString());

            GDAOperations.Update(mdfe, "ChaveAcesso, DataEmissao");

            #region Gera XML

            XmlDocument doc = new XmlDocument();

            // Informações do MDF-e
            XmlElement MDFe = doc.CreateElement("MDFe");
            MDFe.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/mdfe");
            doc.AppendChild(MDFe);

            XmlElement infMDFe = doc.CreateElement("infMDFe");
            infMDFe.SetAttribute("versao", ConfigMDFe.VersaoMDFe);
            infMDFe.SetAttribute("Id", "MDFe" + mdfe.ChaveAcesso);
            MDFe.AppendChild(infMDFe);

            #region Identificação do MDF-e

            try
            {
                XmlElement ide = doc.CreateElement("ide");

                ManipulacaoXml.SetNode(doc, ide, "cUF", cidadeLojaEmitente.CodIbgeUf);
                ManipulacaoXml.SetNode(doc, ide, "tpAmb", ((int)ConfigMDFe.TipoAmbiente).ToString()); // 1-Produção 2-Homologação
                ManipulacaoXml.SetNode(doc, ide, "tpEmit", ((int)mdfe.TipoEmitente).ToString());
                if (mdfe.TipoTransportador != TipoTransportadorEnum.Nenhum)
                    ManipulacaoXml.SetNode(doc, ide, "tpTransp", ((int)mdfe.TipoTransportador).ToString());
                ManipulacaoXml.SetNode(doc, ide, "mod", mdfe.Modelo.ToString());
                ManipulacaoXml.SetNode(doc, ide, "serie", mdfe.Serie.ToString());
                ManipulacaoXml.SetNode(doc, ide, "nMDF", mdfe.NumeroManifestoEletronico.ToString());
                ManipulacaoXml.SetNode(doc, ide, "cMDF", mdfe.CodigoAleatorio.ToString());
                ManipulacaoXml.SetNode(doc, ide, "cDV", mdfe.ChaveAcesso[43].ToString());
                ManipulacaoXml.SetNode(doc, ide, "modal", ((int)mdfe.Modal).ToString());
                ManipulacaoXml.SetNode(doc, ide, "dhEmi", dataEmissao.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                ManipulacaoXml.SetNode(doc, ide, "tpEmis", ((int)mdfe.TipoEmissao).ToString());

                // Emissão com aplicativo do contribuinte;
                ManipulacaoXml.SetNode(doc, ide, "procEmi", "0");
                ManipulacaoXml.SetNode(doc, ide, "verProc", ConfigMDFe.VersaoMDFe);
                ManipulacaoXml.SetNode(doc, ide, "UFIni", mdfe.UFInicio);
                ManipulacaoXml.SetNode(doc, ide, "UFFim", mdfe.UFFim);

                // Minicipio de Carga
                Cidade cidadeCarga = null;
                foreach (var munCar in mdfe.CidadesCarga)
                {
                    cidadeCarga = CidadeDAO.Instance.GetElementByPrimaryKey(munCar.IdCidade);
                    XmlElement infMunCarrega = doc.CreateElement("infMunCarrega");
                    ManipulacaoXml.SetNode(doc, infMunCarrega, "cMunCarrega", cidadeCarga.CodUfMunicipio);
                    ManipulacaoXml.SetNode(doc, infMunCarrega, "xMunCarrega", cidadeCarga.NomeCidade);
                    ide.AppendChild(infMunCarrega);
                }

                // UF Percurso
                foreach (var uf in mdfe.UFsPercurso)
                {
                    XmlElement infPercurso = doc.CreateElement("infPercurso");
                    ManipulacaoXml.SetNode(doc, infPercurso, "UFPer", uf.UFPercurso);
                    ide.AppendChild(infPercurso);
                }

                // A DataInicioViagem deve ser maior que a DataEmissao
                // Se não for, não é obrigatorio informar DataInicioViagem
                if (dataInicioViagem > dataEmissao)
                    ManipulacaoXml.SetNode(doc, ide, "dhIniViagem", dataInicioViagem.ToString("yyyy-MM-ddTHH:mm:sszzz"));
                infMDFe.AppendChild(ide);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir Identificação do MDFe no XML.", ex);
            }

            #endregion

            #region Identificação do Emitente do Manifesto

            try
            {
                XmlElement emit = doc.CreateElement("emit");
                ManipulacaoXml.SetNode(doc, emit, "CNPJ", Formatacoes.TrataStringDocFiscal(lojaEmitente.Cnpj).PadLeft(14, '0'));
                ManipulacaoXml.SetNode(doc, emit, "IE", Formatacoes.TrataStringDocFiscal(lojaEmitente.InscEst.ToUpper()));
                if (ConfigMDFe.TipoAmbiente == ConfigMDFe.TipoAmbienteMDFe.Homologacao)
                    ManipulacaoXml.SetNode(doc, emit, "xNome", "CT-E EMITIDO EM AMBIENTE DE HOMOLOGACAO - SEM VALOR FISCAL");
                else
                    ManipulacaoXml.SetNode(doc, emit, "xNome", Formatacoes.TrataStringDocFiscal(lojaEmitente.RazaoSocial));

                if (!string.IsNullOrWhiteSpace(Formatacoes.TrataStringDocFiscal(lojaEmitente.NomeFantasia)))
                    ManipulacaoXml.SetNode(doc, emit, "xFant", Formatacoes.TrataStringDocFiscal(lojaEmitente.NomeFantasia));
                XmlElement enderEmit = doc.CreateElement("enderEmit");
                ManipulacaoXml.SetNode(doc, enderEmit, "xLgr", Formatacoes.TrataStringDocFiscal(lojaEmitente.Endereco));
                ManipulacaoXml.SetNode(doc, enderEmit, "nro", Formatacoes.TrataStringDocFiscal(lojaEmitente.Numero));

                if (!String.IsNullOrEmpty(Formatacoes.TrataStringDocFiscal(lojaEmitente.Compl)))
                    ManipulacaoXml.SetNode(doc, enderEmit, "xCpl", Formatacoes.TrataStringDocFiscal(lojaEmitente.Compl));

                ManipulacaoXml.SetNode(doc, enderEmit, "xBairro", Formatacoes.TrataStringDocFiscal(lojaEmitente.Bairro));
                ManipulacaoXml.SetNode(doc, enderEmit, "cMun", cidadeLojaEmitente.CodUfMunicipio);
                ManipulacaoXml.SetNode(doc, enderEmit, "xMun", Formatacoes.TrataStringDocFiscal(cidadeLojaEmitente.NomeCidade));
                if (!string.IsNullOrEmpty(lojaEmitente.Cep))
                    ManipulacaoXml.SetNode(doc, enderEmit, "CEP", Formatacoes.TrataStringDocFiscal(lojaEmitente.Cep));
                ManipulacaoXml.SetNode(doc, enderEmit, "UF", cidadeLojaEmitente.NomeUf);
                if (!string.IsNullOrEmpty(lojaEmitente.Telefone))
                    ManipulacaoXml.SetNode(doc, enderEmit, "fone", Formatacoes.TrataStringDocFiscal(lojaEmitente.Telefone, true));
                if (!string.IsNullOrEmpty(lojaEmitente.EmailFiscal))
                    ManipulacaoXml.SetNode(doc, enderEmit, "email", lojaEmitente.EmailFiscal);

                emit.AppendChild(enderEmit);
                infMDFe.AppendChild(emit);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir Identificação do Emitente do MDFe no XML.", ex);
            }

            #endregion

            #region Informações do modal

            try
            {
                XmlElement infModal = doc.CreateElement("infModal");

                switch (mdfe.Modal)
                {
                    case ModalEnum.Rodoviario:
                        infModal.SetAttribute("versaoModal", ConfigMDFe.VersaoModalRodoviario);
                        var rodo = GerarXMLModalRodoviario(ref doc, mdfe, lojaEmitente, contratante);
                        infModal.AppendChild(rodo);
                        infMDFe.AppendChild(infModal);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir Informações do modal do MDFe no XML.", ex);
            }

            #endregion

            #region Informações dos Documentos fiscais vinculados ao manifesto

            try
            {
                XmlElement infDoc = doc.CreateElement("infDoc");
                Cidade cidadeDescarga = null;
                NotaFiscal notaFiscal = null;
                Model.Cte.ConhecimentoTransporte conhecimentoTransporte = null;
                foreach (var munDescarga in mdfe.CidadesDescarga)
                {
                    cidadeDescarga = CidadeDAO.Instance.GetElementByPrimaryKey(munDescarga.IdCidade);
                    XmlElement infMunDescarga = doc.CreateElement("infMunDescarga");
                    ManipulacaoXml.SetNode(doc, infMunDescarga, "cMunDescarga", cidadeDescarga.CodUfMunicipio);
                    ManipulacaoXml.SetNode(doc, infMunDescarga, "xMunDescarga", cidadeDescarga.NomeCidade);
                    if(mdfe.TipoEmitente == TipoEmitenteEnum.TransportadorCargaPropria)
                    {
                        // Nota Fiscal
                        foreach (var nfe in munDescarga.NFesCidadeDescarga)
                        {
                            notaFiscal = NotaFiscalDAO.Instance.GetElementByPrimaryKey(nfe.IdNFe);
                            XmlElement infNFe = doc.CreateElement("infNFe");
                            ManipulacaoXml.SetNode(doc, infNFe, "chNFe", notaFiscal.ChaveAcesso);
                            //Se o tipo de emissão da NF-e informada for FS-DA, o campo SegCodBarra deverá ser informado.
                            if (notaFiscal.FormaEmissao == (int)NotaFiscal.TipoEmissao.ContingenciaFSDA)
                            {
                                ManipulacaoXml.SetNode(doc, infNFe, "SegCodBarra", notaFiscal.NumeroDocumentoFsda.ToString());
                            }
                            infMunDescarga.AppendChild(infNFe);
                        }
                    }
                    else
                    {
                        // Conhecimento de Transporte
                        foreach (var cte in munDescarga.CTesCidadeDescarga)
                        {
                            conhecimentoTransporte = CTe.ConhecimentoTransporteDAO.Instance.GetElementByPrimaryKey(cte.IdCTe);
                            XmlElement infCTe = doc.CreateElement("infCTe");
                            ManipulacaoXml.SetNode(doc, infCTe, "chCTe", conhecimentoTransporte.ChaveAcesso);
                            //Se o tipo de emissão do CT-e informada for FS-DA, o campo SegCodBarra deverá ser informado.
                            if (conhecimentoTransporte.TipoEmissao == (int)Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.ContingenciaFsda)
                            {
                                throw new Exception("O sistema não está configurado para trabalhar com CTe emitido em FS-DA");
                            }
                            infMunDescarga.AppendChild(infCTe);
                        }
                    }
                    infDoc.AppendChild(infMunDescarga);
                }
                infMDFe.AppendChild(infDoc);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir Documentos vinculados do MDFe no XML.", ex);
            }

            #endregion

            #region Informações de Seguro da Carga

            try
            {
                if (mdfe.IdSeguradora > 0 && !string.IsNullOrWhiteSpace(mdfe.NumeroApolice))
                {
                    XmlElement seg = doc.CreateElement("seg");

                    #region Informações do responsável pelo seguro da carga

                    XmlElement infResp = doc.CreateElement("infResp");
                    ManipulacaoXml.SetNode(doc, infResp, "respSeg", ((int)mdfe.ResponsavelSeguro).ToString());
                    if (mdfe.ResponsavelSeguro == ResponsavelEnum.Contratante)
                    {
                        if (contratante.IdLoja > 0)
                        {
                            var loja = LojaDAO.Instance.GetElement((uint)contratante.IdLoja.GetValueOrDefault());
                            ManipulacaoXml.SetNode(doc, infResp, "CNPJ", Formatacoes.TrataStringDocFiscal(loja.Cnpj).PadLeft(14, '0'));
                        }
                        else if (contratante.IdFornecedor > 0)
                        {
                            var fornec = FornecedorDAO.Instance.GetElement((uint)contratante.IdFornecedor.GetValueOrDefault());

                            if (fornec.TipoPessoa.ToUpper() == "J")
                            {
                                // Verifica se o CNPJ é válido
                                if (!Glass.Validacoes.ValidaCnpj(fornec.CpfCnpj))
                                    throw new Exception("O CNPJ do Fornecedor é inválido. Altere no cadastro de fornecedores.");
                            }
                            else
                            {
                                // Verifica se o CPF é válido
                                if (!Glass.Validacoes.ValidaCpf(fornec.CpfCnpj))
                                    throw new Exception("O CPF do Fornecedor é inválido. Altere no cadastro de fornecedores.");
                            }
                            
                            if (Formatacoes.TrataStringDocFiscal(fornec.CpfCnpj).Length == 11)
                                ManipulacaoXml.SetNode(doc, infResp, "CPF", Formatacoes.TrataStringDocFiscal(fornec.CpfCnpj).PadLeft(11, '0'));
                            else if (Formatacoes.TrataStringDocFiscal(fornec.CpfCnpj).Length == 14)
                                ManipulacaoXml.SetNode(doc, infResp, "CNPJ", Formatacoes.TrataStringDocFiscal(fornec.CpfCnpj).PadLeft(14, '0'));
                        }
                        else if (contratante.IdCliente > 0)
                        {
                            var cliente = ClienteDAO.Instance.GetElement((uint)contratante.IdCliente.GetValueOrDefault());

                            if (cliente.TipoPessoa.ToUpper() == "J")
                            {
                                // Verifica se o CNPJ é válido
                                if (!Glass.Validacoes.ValidaCnpj(cliente.CpfCnpj))
                                    throw new Exception("O CNPJ do Cliente é inválido. Altere no cadastro de clientes.");
                            }
                            else
                            {
                                // Verifica se o CPF é válido
                                if (!Glass.Validacoes.ValidaCpf(cliente.CpfCnpj))
                                    throw new Exception("O CPF do Cliente é inválido. Altere no cadastro de clientes.");
                            }

                            if (Formatacoes.TrataStringDocFiscal(cliente.CpfCnpj).Length == 11)
                                ManipulacaoXml.SetNode(doc, infResp, "CPF", Formatacoes.TrataStringDocFiscal(cliente.CpfCnpj).PadLeft(11, '0'));
                            else if (Formatacoes.TrataStringDocFiscal(cliente.CpfCnpj).Length == 14)
                                ManipulacaoXml.SetNode(doc, infResp, "CNPJ", Formatacoes.TrataStringDocFiscal(cliente.CpfCnpj).PadLeft(14, '0'));
                        }
                        else if (contratante.IdTransportador > 0)
                        {
                            var transp = TransportadorDAO.Instance.GetElement((uint)contratante.IdTransportador.GetValueOrDefault());
                            if (Formatacoes.TrataStringDocFiscal(transp.CpfCnpj).Length == 11)
                                ManipulacaoXml.SetNode(doc, infResp, "CPF", Formatacoes.TrataStringDocFiscal(transp.CpfCnpj).PadLeft(11, '0'));
                            else if (Formatacoes.TrataStringDocFiscal(transp.CpfCnpj).Length == 14)
                                ManipulacaoXml.SetNode(doc, infResp, "CNPJ", Formatacoes.TrataStringDocFiscal(transp.CpfCnpj).PadLeft(14, '0'));
                        }
                    }
                    seg.AppendChild(infResp);

                    #endregion

                    #region Informações da seguradora

                    XmlElement infSeg = doc.CreateElement("infSeg");
                    ManipulacaoXml.SetNode(doc, infSeg, "xSeg", Formatacoes.TrataStringDocFiscal(seguradora.NomeSeguradora));
                    ManipulacaoXml.SetNode(doc, infSeg, "CNPJ", Formatacoes.TrataStringDocFiscal(seguradora.CNPJ));

                    seg.AppendChild(infSeg);

                    #endregion

                    ManipulacaoXml.SetNode(doc, seg, "nApol", Formatacoes.TrataStringDocFiscal(mdfe.NumeroApolice));

                    //Informar as averbações do seguro 
                    foreach (var averbacaoSeguro in mdfe.AverbacaoSeguro)
                    {
                        ManipulacaoXml.SetNode(doc, seg, "nAver", Formatacoes.TrataStringDocFiscal(averbacaoSeguro.NumeroAverbacao));
                    }

                    infMDFe.AppendChild(seg);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir Informações de Seguro da Carga do MDFe no XML.", ex);
            }

            #endregion

            #region Totalizadores da carga transportada e seus documentos fiscais

            try
            {
                var quantidadeCTe = CidadeDescargaMDFeDAO.Instance.CountCTesAssociadoMDFe(null, mdfe.IdManifestoEletronico);
                var quantidadeNFe = CidadeDescargaMDFeDAO.Instance.CountNFesAssociadoMDFe(null, mdfe.IdManifestoEletronico);
                XmlElement tot = doc.CreateElement("tot");
                if (quantidadeCTe > 0)
                    ManipulacaoXml.SetNode(doc, tot, "qCTe", quantidadeCTe.ToString());
                if (quantidadeNFe > 0)
                    ManipulacaoXml.SetNode(doc, tot, "qNFe", quantidadeNFe.ToString());
                //ManipulacaoXml.SetNode(doc, tot, "qMDFe", "");
                ManipulacaoXml.SetNode(doc, tot, "vCarga", mdfe.ValorCarga.ToString().Replace(',', '.'));
                ManipulacaoXml.SetNode(doc, tot, "cUnid", ((int)mdfe.CodigoUnidade).ToString("00"));
                ManipulacaoXml.SetNode(doc, tot, "qCarga", mdfe.QuantidadeCarga.ToString().Replace(',', '.'));
                infMDFe.AppendChild(tot);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao inserir Totalizadores do MDFe no XML.", ex);
            }

            #endregion

            #region Lacres do MDF-e

            //Preechimento opcional para os modais Rodoviário e Ferroviário
            //XmlElement lacres = doc.CreateElement("lacres");

            //infMDFe.AppendChild(lacres);

            #endregion

            #region Autorizados para download do XML do MDF-e

            // não é necessário
            //XmlElement autXML = doc.CreateElement("autXML");

            //infMDFe.AppendChild(autXML);

            #endregion

            #region Informações Adicionais

            if (!string.IsNullOrWhiteSpace(mdfe.InformacoesAdicionaisFisco) || !string.IsNullOrWhiteSpace(mdfe.InformacoesComplementares))
            {
                XmlElement infAdic = doc.CreateElement("infAdic");
                if (!string.IsNullOrWhiteSpace(mdfe.InformacoesAdicionaisFisco))
                    ManipulacaoXml.SetNode(doc, infAdic, "infAdFisco", Formatacoes.TrataStringDocFiscal(mdfe.InformacoesAdicionaisFisco));
                if (!string.IsNullOrWhiteSpace(mdfe.InformacoesComplementares))
                    ManipulacaoXml.SetNode(doc, infAdic, "infCpl", Formatacoes.TrataStringDocFiscal(mdfe.InformacoesComplementares));
                infMDFe.AppendChild(infAdic);
            }

            #endregion

            #endregion

            #region Assina MDFe

            try
            {
                if (!preVisualizar)
                {
                    MemoryStream stream = new MemoryStream();
                    doc.Save(stream);

                    using (stream)
                    {
                        // Classe responsável por assinar o xml do MDFe
                        AssinaturaDigital AD = new AssinaturaDigital();

                        System.Security.Cryptography.X509Certificates.X509Certificate2 cert = NFeUtils.Certificado.GetCertificado((uint)lojaEmitente.IdLoja);

                        if (DateTime.Now > cert.NotAfter)
                            throw new Exception("O certificado digital cadastrado está vencido, insira um novo certificado para emitir este MDFe. Data Venc.: " + cert.GetExpirationDateString());

                        int resultado = AD.Assinar(ref doc, "infMDFe", cert);

                        if (resultado > 0)
                            throw new Exception(AD.mensagemResultado);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao assinar MDFe." + ex.Message);
            }

            #endregion

            #region Valida XML

            try
            {
                if (!preVisualizar)
                    ValidaXML.Validar(doc, ValidaXML.TipoArquivoXml.MDFe);
            }
            catch (Exception ex)
            {
                throw new Exception("XML inconsistente." + ex.Message);
            }

            #endregion

            #region Salva arquivo XML do MDFe

            try
            {
                string fileName = Glass.Data.Helper.Utils.GetMDFeXmlPath + doc["MDFe"]["infMDFe"].GetAttribute("Id").Remove(0, 4) + "-mdfe.xml";

                if (File.Exists(fileName))
                    File.Delete(fileName);

                doc.Save(fileName);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao salvar arquivo xml do MDFe. " + ex.Message);
            }

            #endregion

            return doc;
        }

        private XmlElement GerarXMLModalRodoviario(ref XmlDocument doc, ManifestoEletronico mdfe, Loja lojaEmitente, ParticipanteMDFe contratante)
        {
            var veiculoTracao = VeiculoDAO.Instance.GetElement(mdfe.Rodoviario.PlacaTracao);
            var proprietarioVeiculoVeiculo = CTe.ProprietarioVeiculo_VeiculoDAO.Instance.GetElement(veiculoTracao.Placa, 0);
            Model.Cte.ProprietarioVeiculo proprietarioVeiculoTracao = null;
            if (proprietarioVeiculoVeiculo != null)
                proprietarioVeiculoTracao = CTe.ProprietarioVeiculoDAO.Instance.GetElement(proprietarioVeiculoVeiculo.IdPropVeic);

            XmlElement rodo = doc.CreateElement("rodo");
            rodo.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/mdfe");

            #region Grupo de informações para Agência Reguladora

            if (mdfe.TipoEmitente == TipoEmitenteEnum.PrestadorServicoTrasporte ||
                mdfe.TipoEmitente == TipoEmitenteEnum.PrestadorServicoTrasporteCTeGlobalizado)
            {
                XmlElement infANTT = doc.CreateElement("infANTT");
                // O RNTRC é opcional e está dando erro ao tentar emitir.
                //if (!string.IsNullOrWhiteSpace(lojaEmitente.RNTRC))
                //    ManipulacaoXml.SetNode(doc, infANTT, "RNTRC", lojaEmitente.RNTRC);

                #region Dados do CIOT

                foreach (var ciotRodoviario in mdfe.Rodoviario.CiotRodoviario)
                {
                    XmlElement infCIOT = doc.CreateElement("infCIOT");
                    ManipulacaoXml.SetNode(doc, infCIOT, "CIOT", ciotRodoviario.CIOT.PadLeft(12, '0'));
                    if (ciotRodoviario.CPFCNPJCIOT.Count() == 14)
                        ManipulacaoXml.SetNode(doc, infCIOT, "CNPJ", ciotRodoviario.CPFCNPJCIOT);
                    else
                        ManipulacaoXml.SetNode(doc, infCIOT, "CPF", ciotRodoviario.CPFCNPJCIOT);

                    infANTT.AppendChild(infCIOT);
                }

                #endregion

                #region Informações de Vale Pedágio

                if (mdfe.Rodoviario.PedagioRodoviario.Count > 0)
                {
                    XmlElement valePed = doc.CreateElement("valePed");
                    foreach (var pedagioRodoviario in mdfe.Rodoviario.PedagioRodoviario)
                    {
                        var cNPJForn = FornecedorDAO.Instance.ObtemCpfCnpj((uint)pedagioRodoviario.IdFornecedor);
                        XmlElement disp = doc.CreateElement("disp");
                        ManipulacaoXml.SetNode(doc, disp, "CNPJForn", Formatacoes.TrataStringDocFiscal(cNPJForn));
                        // Responsável pelo pagamento do Vale-Pedágio
                        if (pedagioRodoviario.ResponsavelPedagio != ResponsavelEnum.Emitente)
                        {
                            if (contratante.IdLoja > 0)
                            {
                                var loja = LojaDAO.Instance.GetElement((uint)contratante.IdLoja.GetValueOrDefault());
                                ManipulacaoXml.SetNode(doc, disp, "CNPJPg", Formatacoes.TrataStringDocFiscal(loja.Cnpj).PadLeft(14, '0'));
                            }
                            else if (contratante.IdFornecedor > 0)
                            {
                                var fornec = FornecedorDAO.Instance.GetElement((uint)contratante.IdFornecedor.GetValueOrDefault());
                                if (Formatacoes.TrataStringDocFiscal(fornec.CpfCnpj).Length == 11)
                                    ManipulacaoXml.SetNode(doc, disp, "CPFPg", Formatacoes.TrataStringDocFiscal(fornec.CpfCnpj).PadLeft(11, '0'));
                                else if (Formatacoes.TrataStringDocFiscal(fornec.CpfCnpj).Length == 14)
                                    ManipulacaoXml.SetNode(doc, disp, "CNPJPg", Formatacoes.TrataStringDocFiscal(fornec.CpfCnpj).PadLeft(14, '0'));
                            }
                            else if (contratante.IdCliente > 0)
                            {
                                var cliente = ClienteDAO.Instance.GetElement((uint)contratante.IdCliente.GetValueOrDefault());
                                if (Formatacoes.TrataStringDocFiscal(cliente.CpfCnpj).Length == 11)
                                    ManipulacaoXml.SetNode(doc, disp, "CPFPg", Formatacoes.TrataStringDocFiscal(cliente.CpfCnpj).PadLeft(11, '0'));
                                else if (Formatacoes.TrataStringDocFiscal(cliente.CpfCnpj).Length == 14)
                                    ManipulacaoXml.SetNode(doc, disp, "CNPJPg", Formatacoes.TrataStringDocFiscal(cliente.CpfCnpj).PadLeft(14, '0'));
                            }
                            else if (contratante.IdTransportador > 0)
                            {
                                var transp = TransportadorDAO.Instance.GetElement((uint)contratante.IdTransportador.GetValueOrDefault());
                                if (Formatacoes.TrataStringDocFiscal(transp.CpfCnpj).Length == 11)
                                    ManipulacaoXml.SetNode(doc, disp, "CPFPg", Formatacoes.TrataStringDocFiscal(transp.CpfCnpj).PadLeft(11, '0'));
                                else if (Formatacoes.TrataStringDocFiscal(transp.CpfCnpj).Length == 14)
                                    ManipulacaoXml.SetNode(doc, disp, "CNPJPg", Formatacoes.TrataStringDocFiscal(transp.CpfCnpj).PadLeft(14, '0'));
                            }
                        }
                        ManipulacaoXml.SetNode(doc, disp, "nCompra", pedagioRodoviario.NumeroCompra);
                        ManipulacaoXml.SetNode(doc, disp, "vValePed", pedagioRodoviario.ValorValePedagio.ToString().Replace(',', '.'));

                        valePed.AppendChild(disp);
                    }
                    infANTT.AppendChild(valePed);
                }

                #endregion

                #region Grupo de informações dos contratantes do serviço de transporte

                XmlElement infContratante = doc.CreateElement("infContratante");
                if (contratante.IdLoja > 0)
                {
                    var loja = LojaDAO.Instance.GetElement((uint)contratante.IdLoja.GetValueOrDefault());
                    ManipulacaoXml.SetNode(doc, infContratante, "CNPJ", Formatacoes.TrataStringDocFiscal(loja.Cnpj).PadLeft(14, '0'));
                }
                else if (contratante.IdFornecedor > 0)
                {
                    var fornec = FornecedorDAO.Instance.GetElement((uint)contratante.IdFornecedor.GetValueOrDefault());
                    if (Formatacoes.TrataStringDocFiscal(fornec.CpfCnpj).Length == 11)
                        ManipulacaoXml.SetNode(doc, infContratante, "CPF", Formatacoes.TrataStringDocFiscal(fornec.CpfCnpj).PadLeft(11, '0'));
                    else if (Formatacoes.TrataStringDocFiscal(fornec.CpfCnpj).Length == 14)
                        ManipulacaoXml.SetNode(doc, infContratante, "CNPJ", Formatacoes.TrataStringDocFiscal(fornec.CpfCnpj).PadLeft(14, '0'));
                }
                else if (contratante.IdCliente > 0)
                {
                    var cliente = ClienteDAO.Instance.GetElement((uint)contratante.IdCliente.GetValueOrDefault());
                    if (Formatacoes.TrataStringDocFiscal(cliente.CpfCnpj).Length == 11)
                        ManipulacaoXml.SetNode(doc, infContratante, "CPF", Formatacoes.TrataStringDocFiscal(cliente.CpfCnpj).PadLeft(11, '0'));
                    else if (Formatacoes.TrataStringDocFiscal(cliente.CpfCnpj).Length == 14)
                        ManipulacaoXml.SetNode(doc, infContratante, "CNPJ", Formatacoes.TrataStringDocFiscal(cliente.CpfCnpj).PadLeft(14, '0'));
                }
                else if (contratante.IdTransportador > 0)
                {
                    var transp = TransportadorDAO.Instance.GetElement((uint)contratante.IdTransportador.GetValueOrDefault());
                    if (Formatacoes.TrataStringDocFiscal(transp.CpfCnpj).Length == 11)
                        ManipulacaoXml.SetNode(doc, infContratante, "CPF", Formatacoes.TrataStringDocFiscal(transp.CpfCnpj).PadLeft(11, '0'));
                    else if (Formatacoes.TrataStringDocFiscal(transp.CpfCnpj).Length == 14)
                        ManipulacaoXml.SetNode(doc, infContratante, "CNPJ", Formatacoes.TrataStringDocFiscal(transp.CpfCnpj).PadLeft(14, '0'));
                }
                infANTT.AppendChild(infContratante);

                #endregion

                rodo.AppendChild(infANTT);
            }

            #endregion

            #region Dados do Veículo com a Tração

            XmlElement veicTracao = doc.CreateElement("veicTracao");
            //ManipulacaoXml.SetNode(doc, veicTracao, "cInt", "");
            ManipulacaoXml.SetNode(doc, veicTracao, "placa", Formatacoes.TrataStringDocFiscal(veiculoTracao.Placa));
            if (!string.IsNullOrWhiteSpace(veiculoTracao.Renavam))
                ManipulacaoXml.SetNode(doc, veicTracao, "RENAVAM", veiculoTracao.Renavam);
            ManipulacaoXml.SetNode(doc, veicTracao, "tara", veiculoTracao.Tara.ToString());
            if (veiculoTracao.CapacidadeKg > 0)
                ManipulacaoXml.SetNode(doc, veicTracao, "capKG", veiculoTracao.CapacidadeKg.ToString());
            if (veiculoTracao.CapacidadeM3 > 0)
                ManipulacaoXml.SetNode(doc, veicTracao, "capM3", veiculoTracao.CapacidadeM3.ToString());

            #region Proprietários do Veículo. Só preenchido quando o veículo não pertencer à empresa emitente do MDF-e

            if (proprietarioVeiculoTracao != null &&
                Formatacoes.TrataStringDocFiscal(proprietarioVeiculoTracao.Cnpj) != Formatacoes.TrataStringDocFiscal(lojaEmitente.Cnpj))
            {
                XmlElement prop = doc.CreateElement("prop");
                if (!string.IsNullOrWhiteSpace(Formatacoes.TrataStringDocFiscal(proprietarioVeiculoTracao.Cpf)))
                    ManipulacaoXml.SetNode(doc, prop, "CPF", Formatacoes.TrataStringDocFiscal(proprietarioVeiculoTracao.Cpf));
                else
                    ManipulacaoXml.SetNode(doc, prop, "CNPJ", Formatacoes.TrataStringDocFiscal(proprietarioVeiculoTracao.Cnpj));

                ManipulacaoXml.SetNode(doc, veicTracao, "RNTRC", proprietarioVeiculoTracao.RNTRC);
                ManipulacaoXml.SetNode(doc, veicTracao, "xNome", proprietarioVeiculoTracao.Nome);
                ManipulacaoXml.SetNode(doc, veicTracao, "IE", proprietarioVeiculoTracao.IE);
                ManipulacaoXml.SetNode(doc, veicTracao, "UF", proprietarioVeiculoTracao.UF);
                ManipulacaoXml.SetNode(doc, veicTracao, "tpProp", proprietarioVeiculoTracao.TipoProp.ToString());

                veicTracao.AppendChild(prop);
            }

            #endregion

            #region Informações do(s) Condutor(s) do veículo

            foreach (var cond in mdfe.Rodoviario.CondutorVeiculo)
            {
                var condutorVeiculo = FuncionarioDAO.Instance.GetElement((uint)cond.IdCondutor);
                XmlElement condutor = doc.CreateElement("condutor");
                ManipulacaoXml.SetNode(doc, condutor, "xNome", condutorVeiculo.Nome);
                ManipulacaoXml.SetNode(doc, condutor, "CPF", Formatacoes.TrataStringDocFiscal(condutorVeiculo.Cpf));

                veicTracao.AppendChild(condutor);
            }

            #endregion

            ManipulacaoXml.SetNode(doc, veicTracao, "tpRod", veiculoTracao.TipoRodado.ToString("00"));
            ManipulacaoXml.SetNode(doc, veicTracao, "tpCar", veiculoTracao.TipoCarroceria.ToString("00"));
            ManipulacaoXml.SetNode(doc, veicTracao, "UF", veiculoTracao.UfLicenc);

            rodo.AppendChild(veicTracao);

            #endregion

            #region Dados dos reboques

            Veiculo veiculoReboque = null;
            Model.Cte.ProprietarioVeiculo_Veiculo proprietarioVeiculoVeiculoReboque = null;

            foreach (var veicReb in mdfe.Rodoviario.VeiculoRodoviario)
            {
                veiculoReboque = VeiculoDAO.Instance.GetElement(veicReb.Placa);

                XmlElement veicReboque = doc.CreateElement("veicReboque");
                //ManipulacaoXml.SetNode(doc, veicReboque, "cInt", "");
                ManipulacaoXml.SetNode(doc, veicReboque, "placa", Formatacoes.TrataStringDocFiscal(veiculoReboque.Placa));
                if (!string.IsNullOrWhiteSpace(veiculoReboque.Renavam))
                    ManipulacaoXml.SetNode(doc, veicReboque, "RENAVAM", veiculoReboque.Renavam);
                ManipulacaoXml.SetNode(doc, veicReboque, "tara", veiculoReboque.Tara.ToString());
                ManipulacaoXml.SetNode(doc, veicReboque, "capKG", veiculoReboque.CapacidadeKg.ToString());
                if (veiculoReboque.CapacidadeM3 > 0)
                    ManipulacaoXml.SetNode(doc, veicReboque, "capM3", veiculoReboque.CapacidadeM3.ToString());

                #region Proprietários do Veículo. Só preenchido quando o veículo não pertencer à empresa emitente do MDF-e

                proprietarioVeiculoVeiculoReboque = CTe.ProprietarioVeiculo_VeiculoDAO.Instance.GetElement(veiculoReboque.Placa, 0);
                Model.Cte.ProprietarioVeiculo proprietarioVeiculoReboque = null;
                if (proprietarioVeiculoVeiculoReboque != null)
                    proprietarioVeiculoReboque = CTe.ProprietarioVeiculoDAO.Instance.GetElement(proprietarioVeiculoVeiculoReboque.IdPropVeic);

                if (proprietarioVeiculoReboque != null &&
                Formatacoes.TrataStringDocFiscal(proprietarioVeiculoReboque.Cnpj) != Formatacoes.TrataStringDocFiscal(lojaEmitente.Cnpj))
                {
                    XmlElement prop = doc.CreateElement("prop");
                    if (!string.IsNullOrWhiteSpace(Formatacoes.TrataStringDocFiscal(proprietarioVeiculoReboque.Cpf)))
                        ManipulacaoXml.SetNode(doc, prop, "CPF", Formatacoes.TrataStringDocFiscal(proprietarioVeiculoReboque.Cpf).PadLeft(11, '0'));
                    else
                        ManipulacaoXml.SetNode(doc, prop, "CNPJ", Formatacoes.TrataStringDocFiscal(proprietarioVeiculoReboque.Cnpj).PadLeft(14, '0'));

                    ManipulacaoXml.SetNode(doc, prop, "RNTRC", proprietarioVeiculoReboque.RNTRC);
                    ManipulacaoXml.SetNode(doc, prop, "xNome", proprietarioVeiculoReboque.Nome);
                    ManipulacaoXml.SetNode(doc, prop, "IE", proprietarioVeiculoReboque.IE);
                    ManipulacaoXml.SetNode(doc, prop, "UF", proprietarioVeiculoReboque.UF);
                    ManipulacaoXml.SetNode(doc, prop, "tpProp", proprietarioVeiculoReboque.TipoProp.ToString());

                    veicReboque.AppendChild(prop);
                }

                #endregion

                ManipulacaoXml.SetNode(doc, veicReboque, "tpCar", veiculoReboque.TipoCarroceria.ToString("00"));
                ManipulacaoXml.SetNode(doc, veicReboque, "UF", veiculoReboque.UfLicenc);
                //ManipulacaoXml.SetNode(doc, veicReboque, "codAgPorto", "");

                rodo.AppendChild(veicReboque);
            }

            #endregion

            #region Lacres

            foreach (var lacRod in mdfe.Rodoviario.LacreRodoviario)
            {
                XmlElement lacRodo = doc.CreateElement("lacRodo");
                ManipulacaoXml.SetNode(doc, lacRodo, "nLacre", lacRod.Lacre);
                rodo.AppendChild(lacRodo);
            }

            #endregion

            return rodo;
        }

        #endregion

        #region Gerar XML do MDFe para cancelamento

        public XmlDocument GerarXmlMDFeCancelamento(string justificativa, ManifestoEletronico mdfe)
        {
            // Verifica se o MDFe já foi Cancelado
            if (mdfe.Situacao == SituacaoEnum.Cancelado)
                throw new Exception("Este MDFe já foi cancelado.");

            // Numero do Protocolo de Autorização do MDF - e a ser cancelado.
            var protocolo = ProtocoloMDFeDAO.Instance.GetElement(mdfe.IdManifestoEletronico, (int)ProtocoloMDFe.TipoProtocoloEnum.Autorizacao);

            // Apenas MDFe autorizado com falha no cancelamento ou em processo de cancelamento pode ser cancelado
            if (mdfe.Situacao != SituacaoEnum.Autorizado && mdfe.Situacao != SituacaoEnum.FalhaCancelar)
                throw new Exception("Apenas MDFe autorizado ou com falha no cancelamento pode ser cancelado.");

            // Se o MDFe estiver autorizado mas não possuir protocolo de autorização, não pode ser cancelado
            if (protocolo == null || string.IsNullOrWhiteSpace(protocolo.NumProtocolo))
                throw new Exception("Este MDFe não pode ser cancelado por não possuir protocolo de autorização.");

            var participanteEmitente = mdfe.Participantes.Where(f => f.TipoParticipante == TipoParticipanteEnum.Emitente).FirstOrDefault();
            if (participanteEmitente.IdLoja == null)
                throw new Exception("MDFe não possui Emitente.");
            var lojaEmitente = LojaDAO.Instance.GetElementByPrimaryKey((uint)participanteEmitente.IdLoja);

            #region Monta XML

            // XML do Evento de cancelamento
            XmlDocument xmlCanc = new XmlDocument();

            // Evento de Cancelamento
            XmlElement evCancMDFe = xmlCanc.CreateElement("evCancMDFe");

            ManipulacaoXml.SetNode(xmlCanc, evCancMDFe, "descEvento", "Cancelamento");
            ManipulacaoXml.SetNode(xmlCanc, evCancMDFe, "nProt", protocolo.NumProtocolo);
            ManipulacaoXml.SetNode(xmlCanc, evCancMDFe, "xJust", Formatacoes.TrataStringDocFiscal(justificativa));

            // Adiciona a parte especifica ao XML de Evento
            GerarXmlEvento(mdfe, ref xmlCanc, "110111", ConfigMDFe.VersaoEventoCancelamento, evCancMDFe, lojaEmitente);

            #endregion

            #region Assina XML

            try
            {
                AssinaturaDigital AD = new AssinaturaDigital();

                System.Security.Cryptography.X509Certificates.X509Certificate2 cert = NFeUtils.Certificado.GetCertificado((uint)lojaEmitente.IdLoja);

                if (DateTime.Now > cert.NotAfter)
                    throw new Exception("O certificado digital cadastrado está vencido, insira um novo certificado para emitir este MDFe. Data Venc.: " + cert.GetExpirationDateString());

                int resultado = AD.Assinar(ref xmlCanc, "infEvento", cert);

                if (resultado > 0)
                    throw new Exception(AD.mensagemResultado);
            }
            catch (Exception ex)
            {
                LogMDFeDAO.Instance.NewLog(mdfe.IdManifestoEletronico, "Cancelamento", 1, "Falha ao cancelar MDFe. " + ex.Message);

                throw new Exception("Falha ao assinar pedido de cancelamento." + ex.Message);
            }

            #endregion

            #region Valida XML

            //try
            //{
            //    ValidaXML.Validar(xmlCanc, ValidaXML.TipoArquivoXml.EvCancMDFe);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("XML inconsistente." + ex.Message);
            //}

            #endregion

            ManifestoEletronicoDAO.Instance.AlteraSituacao(mdfe.IdManifestoEletronico, SituacaoEnum.ProcessoCancelamento);

            // Atualiza MDFe informando o motivo do cancelamento
            ManifestoEletronicoDAO.Instance.AtualizaMotivoCancelamento(mdfe.IdManifestoEletronico, justificativa);

            return xmlCanc;
        }

        #endregion

        #region Gerar XML do MDFe para encerramento

        public XmlDocument GerarXmlMDFeEncerramento(ManifestoEletronico mdfe)
        {
            // Verifica se o MDFe já foi Encerrado
            if (mdfe.Situacao == SituacaoEnum.Encerrado)
                throw new Exception("Este MDFe já foi encerrado.");

            // Numero do Protocolo de Autorização do MDF-e a ser encerrado.
            var protocolo = ProtocoloMDFeDAO.Instance.GetElement(mdfe.IdManifestoEletronico, (int)ProtocoloMDFe.TipoProtocoloEnum.Autorizacao);

            // Apenas MDFe autorizado pode ser encerrado
            if (mdfe.Situacao != SituacaoEnum.Autorizado)
                throw new Exception("Apenas MDFe autorizado pode ser encerrado.");

            // Se o MDFe estiver autorizado mas não possuir protocolo de autorização, não pode ser encerrado
            if (mdfe.Situacao == SituacaoEnum.Autorizado && string.IsNullOrWhiteSpace(protocolo.NumProtocolo))
                throw new Exception("Este MDFe não pode ser encerrado por não possuir protocolo de autorização.");

            var participanteEmitente = mdfe.Participantes.Where(f => f.TipoParticipante == TipoParticipanteEnum.Emitente).FirstOrDefault();
            if (participanteEmitente.IdLoja == null)
                throw new Exception("MDFe não possui Emitente.");
            var lojaEmitente = LojaDAO.Instance.GetElementByPrimaryKey((uint)participanteEmitente.IdLoja);
            var cidadeLojaEmitente = CidadeDAO.Instance.GetElementByPrimaryKey((uint)lojaEmitente.IdCidade);

            #region Monta XML

            // XML do Evento de encerramento
            XmlDocument xmlEncerramento = new XmlDocument();

            // Evento de Encerramento
            XmlElement evEncMDFe = xmlEncerramento.CreateElement("evEncMDFe");
            
            ManipulacaoXml.SetNode(xmlEncerramento, evEncMDFe, "descEvento", "Encerramento");
            ManipulacaoXml.SetNode(xmlEncerramento, evEncMDFe, "nProt", protocolo.NumProtocolo);
            var dataEncerramento = DateTimeOffset.Now;
            ManipulacaoXml.SetNode(xmlEncerramento, evEncMDFe, "dtEnc", dataEncerramento.ToString("yyyy-MM-dd"));
            ManipulacaoXml.SetNode(xmlEncerramento, evEncMDFe, "cUF", cidadeLojaEmitente.CodIbgeUf);
            ManipulacaoXml.SetNode(xmlEncerramento, evEncMDFe, "cMun", cidadeLojaEmitente.CodUfMunicipio);

            // Adiciona a parte especifica ao XML de Evento
            GerarXmlEvento(mdfe, ref xmlEncerramento, "110112", ConfigMDFe.VersaoEventoEncerramento, evEncMDFe, lojaEmitente);

            #endregion

            #region Assina XML

            try
            {
                AssinaturaDigital AD = new AssinaturaDigital();

                System.Security.Cryptography.X509Certificates.X509Certificate2 cert = NFeUtils.Certificado.GetCertificado((uint)lojaEmitente.IdLoja);

                if (DateTime.Now > cert.NotAfter)
                    throw new Exception("O certificado digital cadastrado está vencido, insira um novo certificado para emitir este MDFe. Data Venc.: " + cert.GetExpirationDateString());

                int resultado = AD.Assinar(ref xmlEncerramento, "infEvento", cert);

                if (resultado > 0)
                    throw new Exception(AD.mensagemResultado);
            }
            catch (Exception ex)
            {
                LogMDFeDAO.Instance.NewLog(mdfe.IdManifestoEletronico, "Encerramento", 1, "Falha ao encerrar MDFe. " + ex.Message);

                throw new Exception("Falha ao assinar pedido de encerramento." + ex.Message);
            }

            #endregion

            #region Valida XML

            //try
            //{
            //    ValidaXML.Validar(xmlEncerramento, ValidaXML.TipoArquivoXml.EvEncMDFe);
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("XML inconsistente." + ex.Message);
            //}

            #endregion

            ManifestoEletronicoDAO.Instance.AlteraSituacao(mdfe.IdManifestoEletronico, SituacaoEnum.ProcessoEncerramento);

            return xmlEncerramento;
        }

        #endregion

        #region Gerar XML de Evento (Parte Geral)

        private void GerarXmlEvento(ManifestoEletronico mdfe, ref XmlDocument xmlEvento, string tipoEvento, string versaoEvento, XmlElement any, Loja lojaEmitente)
        {
            var cidadeLojaEmitente = CidadeDAO.Instance.GetElementByPrimaryKey((uint)lojaEmitente.IdCidade);

            #region Monta XML

            XmlNode declarationNode = xmlEvento.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlEvento.AppendChild(declarationNode);

            XmlElement eventoMDFe = xmlEvento.CreateElement("eventoMDFe");

            eventoMDFe.SetAttribute("versao", ConfigMDFe.VersaoEvento);
            eventoMDFe.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/mdfe");
            xmlEvento.AppendChild(eventoMDFe);

            XmlElement infEvento = xmlEvento.CreateElement("infEvento");

            //Identificador da TAG a ser assinada, a regra de formação do Id é: 
            //“ID” + tpEvento +  chave da MDF-e + nSeqEvento 
            infEvento.SetAttribute("Id", "ID" + tipoEvento + mdfe.ChaveAcesso + "1".PadLeft(2, '0'));
            eventoMDFe.AppendChild(infEvento);

            // Código do órgão de recepção do Evento. Utilizar a Tabela 
            // do IBGE, utilizar 90 para identificar o Ambiente Nacional. 
            XmlElement cOrgao = xmlEvento.CreateElement("cOrgao");
            cOrgao.InnerText = cidadeLojaEmitente.CodIbgeUf;
            infEvento.AppendChild(cOrgao);

            // Identificação do Ambiente
            // 1 - Produção 
            // 2 – Homologação 
            ManipulacaoXml.SetNode(xmlEvento, infEvento, "tpAmb", ((int)ConfigMDFe.TipoAmbiente).ToString());
            //Autor do evento
            ManipulacaoXml.SetNode(xmlEvento, infEvento, "CNPJ", Formatacoes.TrataStringDocFiscal(lojaEmitente.Cnpj));
            ManipulacaoXml.SetNode(xmlEvento, infEvento, "chMDFe", mdfe.ChaveAcesso);

            var dataEvento = new DateTimeOffset(DateTime.Now.AddMinutes(-2));
            ManipulacaoXml.SetNode(xmlEvento, infEvento, "dhEvento", dataEvento.ToString("yyyy-MM-ddTHH:mm:sszzz"));
            // Tipo do Evento: 
            // 110111 - Cancelamento
            // 110112 – Encerramento
            // 110114 – Inclusão de Condutor
            // 310620 - Registro de Passagem
            ManipulacaoXml.SetNode(xmlEvento, infEvento, "tpEvento", tipoEvento);
            ManipulacaoXml.SetNode(xmlEvento, infEvento, "nSeqEvento", "1");

            // Informações do evento específico.
            XmlElement detEvento = xmlEvento.CreateElement("detEvento");
            // Versão do leiaute específico do evento.
            detEvento.SetAttribute("versaoEvento", versaoEvento);
            infEvento.AppendChild(detEvento);

            // XML do evento
            // Insira neste local o XML específico do tipo de evento(cancelamento, encerramento, registro de passagem)
            detEvento.AppendChild(any);

            infEvento.AppendChild(detEvento);

            #endregion
        }

        #endregion

        #region Gerar XML MDFe Consulta Não Encerrados

        public XmlDocument GerarXmlMDFeConsultaNaoEncerrados(Loja lojaEmitente)
        {
            #region Monta XML

            // XML do Evento de Consulta Não Encerrados
            XmlDocument xmlConsMDFeNaoEnc = new XmlDocument();
            
            XmlNode declarationNode = xmlConsMDFeNaoEnc.CreateXmlDeclaration("1.0", "UTF-8", null);
            xmlConsMDFeNaoEnc.AppendChild(declarationNode);

            XmlElement consMDFeNaoEnc = xmlConsMDFeNaoEnc.CreateElement("consMDFeNaoEnc");

            consMDFeNaoEnc.SetAttribute("versao", ConfigMDFe.VersaoConsultaNaoEncerrado);
            consMDFeNaoEnc.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/mdfe");
            xmlConsMDFeNaoEnc.AppendChild(consMDFeNaoEnc);

            // Identificação do Ambiente
            // 1-Produção 2-Homologação
            ManipulacaoXml.SetNode(xmlConsMDFeNaoEnc, consMDFeNaoEnc, "tpAmb", ((int)ConfigMDFe.TipoAmbiente).ToString());
            ManipulacaoXml.SetNode(xmlConsMDFeNaoEnc, consMDFeNaoEnc, "xServ", "CONSULTAR NÃO ENCERRADOS");
            ManipulacaoXml.SetNode(xmlConsMDFeNaoEnc, consMDFeNaoEnc, "CNPJ", Formatacoes.TrataStringDocFiscal(lojaEmitente.Cnpj));

            #endregion

            return xmlConsMDFeNaoEnc;
        }

        #endregion

        #region Chave de Acesso MDFe

        /// <summary>
        /// Retorna chave de acesso do MDFe
        /// </summary>
        /// <param name="cUf">Código da UF do emitente do MDFe</param>
        /// <param name="AAMM">Ano e Mês de emissão do MDFe</param>
        /// <param name="CNPJ">CNPJ do emitente</param>
        /// <param name="mod">Modelo do Documento Fiscal</param>
        /// <param name="serie">Série do Documento Fiscal</param>
        /// <param name="nMDFe">Número do Documento Fiscal</param>
        /// <param name="cMDFe">Código Numérico que compõe a Chave de Acesso</param>
        public string ChaveDeAcesso(string cUf, string AAMM, string CNPJ, string mod, string serie, string nMDFe, string tpEmis, string cMDFe)
        {
            if (!Glass.Validacoes.ValidaCnpj(CNPJ))
                throw new Exception("CNPJ do emitente é inválido.");

            string chave = cUf + AAMM + CNPJ.Replace(".", "").Replace("/", "").Replace("-", "") + mod.PadLeft(2, '0') +
                serie.PadLeft(3, '0') + nMDFe.PadLeft(9, '0') + tpEmis + cMDFe.PadLeft(8, '0');

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
