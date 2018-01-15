using System;
using System.Collections.Generic;
using Glass.Data.NFeUtils;
using GDA;
using Glass.Data.Model;
using System.Xml;
using Glass.Data.Helper;
using System.IO;

namespace Glass.Data.DAL
{
    public sealed class CartaCorrecaoDAO : BaseDAO<CartaCorrecao, CartaCorrecaoDAO>
    {
        //private CartaCorrecaoDAO() { }

        #region Listagem padrão

        private string Sql(uint id, uint idNf, bool selecionar)
        {
            string campos = selecionar ? "*" : "Count(*)";

            string sql = "Select " + campos + " From carta_correcao Where 1 ";

            if (id > 0)
                sql += " And idcarta=" + id;

            if (idNf > 0)
                sql += " And idnf=" + idNf;

            return sql;
        }

        public CartaCorrecao GetElement(uint idCarta)
        {
            return objPersistence.LoadOneData(Sql(idCarta, 0, true));
        }

        public CartaCorrecao[] GetList(uint idNf, string sortExpression, int startRow, int pageSize)
        {
            return objPersistence.LoadDataWithSortExpression(Sql(0, idNf, true), new InfoSortExpression(sortExpression), new InfoPaging(startRow, pageSize), null).ToList().ToArray();
        }

        public IList<CartaCorrecao> GetList(uint idNf)
        {
            return objPersistence.LoadData(Sql(0, idNf, true)).ToList();
        }

        public int GetCount(uint idNf)
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, idNf, false), null);
        }

        public int GetCount()
        {
            return objPersistence.ExecuteSqlQueryCount(Sql(0, 0, false), null);
        }

        //public int GetIdLote()
        //{
        //    string sql = "select max(IdLote) from carta_correcao";
        //    object obj = objPersistence.ExecuteScalar(sql);
        //    if (obj != System.DBNull.Value)
        //        return Convert.ToInt32(obj) + 1;
        //    else
        //        return 1;
        //}

        #endregion

        #region Insere a carta de correção

        public override uint Insert(CartaCorrecao objInsert)
        {
            string chaveAcessoNf = NotaFiscalDAO.Instance.ObtemChaveAcesso(objInsert.IdNf);
            uint idCidade = LojaDAO.Instance.ObtemValorCampo<uint>("idCidade", "idLoja=" + NotaFiscalDAO.Instance.ObtemIdLoja(objInsert.IdNf));
            string cpfCnpj = LojaDAO.Instance.ObtemValorCampo<string>("cnpj", "idLoja=" + NotaFiscalDAO.Instance.ObtemIdLoja(objInsert.IdNf));
            string codIbgeUf = CidadeDAO.Instance.ObtemValorCampo<string>("codIbgeUf", "idCidade=" + idCidade);

            objInsert.NumeroSequencialEvento = ObtemNumSeqEvento(objInsert.IdNf);
            objInsert.IdInfEvento = "ID" + objInsert.TipoEvento.ToString() + chaveAcessoNf + objInsert.NumeroSequencialEvento.ToString().PadLeft(2, '0');
            objInsert.Orgao = Convert.ToUInt32(codIbgeUf);
            objInsert.TipoAmbiente = ((int)ConfigNFe.TipoAmbiente);
            objInsert.ChaveNFe = chaveAcessoNf;
            objInsert.Situacao = CartaCorrecao.SituacaoEnum.Ativa;
            objInsert.Correcao = Formatacoes.TrataTextoDocFiscal(objInsert.Correcao);

            objInsert.CNPJ = cpfCnpj.Replace(".", String.Empty).Replace("-", String.Empty).Replace("/", String.Empty);

            return objPersistence.Insert(objInsert);
        }

        public override int Update(CartaCorrecao objUpdate)
        {
            objUpdate.Correcao = Formatacoes.TrataStringDocFiscal(objUpdate.Correcao);

 	        return base.Update(objUpdate);
        }

        #endregion

        /// <summary>
        /// Retorna o número sequencial do evento a ser usado na carta de correção
        /// </summary>
        /// <param name="idNf"></param>
        /// <returns></returns>
        public int ObtemNumSeqEvento(uint idNf)
        {
            string sql = "Select Coalesce(Max(numeroSequencialEvento), 0) + 1 From carta_correcao Where idNf=" + idNf;

            return ExecuteScalar<int>(sql);
        }

        /// <summary>
        /// Retorna o número da nota fiscal da carta de correção.
        /// </summary>
        public int ObterIdNotaFiscal(int idCarta)
        {
            return ObtemValorCampo<int>("IdNf", string.Format("IdCarta={0}", idCarta));
        }

        public int AtualizaSituacao(uint idCarta, uint situacao)
        {
            string sql = "update carta_correcao set Situacao = ?sit where idCarta=?id";

            return Convert.ToInt32(objPersistence.ExecuteScalar(sql, new GDAParameter("?sit", situacao), new GDAParameter("?id", idCarta)));
        }

        public void SalvaProtocolo(uint idCarta, string protocolo)
        {
            objPersistence.ExecuteCommand("update carta_correcao set Protocolo = ?p where idCarta=" + idCarta, new GDAParameter("?p", protocolo));
        }

        // TODO TESTE GerarCartaCorrecao
        private XmlDocument GeraXmlCartaCorrecao(uint idCarta, bool preVisualizar)
        {
            CartaCorrecao obj = GetElement(idCarta);

            NotaFiscal nf = NotaFiscalDAO.Instance.GetElement(obj.IdNf);

            XmlDocument doc = new XmlDocument();

            XmlNode declarationNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(declarationNode);

            XmlElement evento = doc.CreateElement("evento");
            evento.SetAttribute("versao", "1.00");
            evento.SetAttribute("xmlns", "http://www.portalfiscal.inf.br/nfe");
            doc.AppendChild(evento);

            XmlElement infEvento = doc.CreateElement("infEvento");

            //Identificador da TAG a ser assinada, a regra de formação 
            //do Id é: 
            //“ID” + tpEvento +  chave da NF-e + nSeqEvento 
            infEvento.SetAttribute("Id", obj.IdInfEvento);
            evento.AppendChild(infEvento);

            // Código do órgão de recepção do Evento. Utilizar a Tabela 
            // do IBGE, utilizar 90 para identificar o Ambiente Nacional. 
            XmlElement cOrgao = doc.CreateElement("cOrgao");
            cOrgao.InnerText = Configuracoes.FiscalConfig.NotaFiscalConfig.ContingenciaNFe == DataSources.TipoContingenciaNFe.NaoUtilizar ? obj.Orgao.ToString() : "90";
            infEvento.AppendChild(cOrgao);

            // Identificação do Amb
            // 1 - Produção 
            // 2 – Homologação 
            XmlElement tpAmb = doc.CreateElement("tpAmb");
            tpAmb.InnerText = obj.TipoAmbiente.ToString();
            infEvento.AppendChild(tpAmb);

            if (!string.IsNullOrEmpty(obj.CPF))
            {
                XmlElement CPF = doc.CreateElement("CPF");
                CPF.InnerText = obj.CPF.ToString().PadLeft(11, '0');
                infEvento.AppendChild(CPF);
            }
            else if (!string.IsNullOrEmpty(obj.CNPJ))
            {
                XmlElement CNPJ = doc.CreateElement("CNPJ");
                CNPJ.InnerText = obj.CNPJ.ToString().PadLeft(14, '0');
                infEvento.AppendChild(CNPJ);
            }

            XmlElement chNFe = doc.CreateElement("chNFe");
            chNFe.InnerText = obj.ChaveNFe;
            infEvento.AppendChild(chNFe);

            XmlElement dhEvento = doc.CreateElement("dhEvento");
            dhEvento.InnerText = DateTime.Now.AddMinutes(-10).ToString("yyyy-MM-ddTHH:mm:ssK");
            infEvento.AppendChild(dhEvento);

            //Código do de evento = 110110 
            XmlElement tpEvento = doc.CreateElement("tpEvento");
            tpEvento.InnerText = obj.TipoEvento.ToString();
            infEvento.AppendChild(tpEvento);

            XmlElement nSeqEvento = doc.CreateElement("nSeqEvento");
            nSeqEvento.InnerText = obj.NumeroSequencialEvento.ToString();
            infEvento.AppendChild(nSeqEvento);

            XmlElement verEvento = doc.CreateElement("verEvento");
            verEvento.InnerText = obj.VersaoEvento;
            infEvento.AppendChild(verEvento);

            XmlElement detEvento = doc.CreateElement("detEvento");
            detEvento.SetAttribute("versao", "1.00");

            ManipulacaoXml.SetNode(doc, detEvento, "descEvento", obj.DescricaoEvento);
            ManipulacaoXml.SetNode(doc, detEvento, "xCorrecao", obj.Correcao);
            ManipulacaoXml.SetNode(doc, detEvento, "xCondUso", obj.CondicaoUso);

            infEvento.AppendChild(detEvento);

            #region Assina CCe

            try
            {
                if (!preVisualizar)
                {
                    MemoryStream stream = new MemoryStream();
                    doc.Save(stream);

                    using (stream)
                    {
                        // Classe responsável por assinar o xml da NFe
                        AssinaturaDigital AD = new AssinaturaDigital();

                        System.Security.Cryptography.X509Certificates.X509Certificate2 cert = Certificado.GetCertificado(nf.IdLoja.Value);

                        if (DateTime.Now > cert.NotAfter)
                            throw new Exception("O certificado digital cadastrado está vencido, insira um novo certificado para emitir esta nota. Data Venc.: " + cert.GetExpirationDateString());

                        int resultado = AD.Assinar(ref doc, "infEvento", cert);

                        if (resultado > 0)
                            throw new Exception(AD.mensagemResultado);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao assinar Carta de Correção." + ex.Message);
            }

            #endregion

            #region Valida XML

            try
            {
                if (!preVisualizar)
                    ValidaXML.Validar(doc, ValidaXML.TipoArquivoXml.CCe);
            }
            catch (Exception ex)
            {
                throw new Exception("XML inconsistente." + ex.Message);
            }

            #endregion

            #region Salva arquivo XML da CCe

            try
            {
                string fileName = Utils.GetCartaCorrecaoXmlPath + doc["evento"]["infEvento"].GetAttribute("Id").Remove(0, 3) + "-cce.xml";

                if (File.Exists(fileName))
                    File.Delete(fileName);

                doc.Save(fileName);
            }
            catch (Exception ex)
            {
                throw new Exception("Falha ao salvar arquivo xml da CCe. " + ex.Message);
            }

            #endregion

            return doc;
        }

        public string EmitirCce(uint idCarta, bool preVisualizar)
        {
            XmlDocument doc = GeraXmlCartaCorrecao(idCarta, preVisualizar);
            string retorno = String.Empty;
            try
            {
                if (!preVisualizar)
                {
                    retorno = EnviaXML.EnviaCCe(doc, idCarta);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retorno;
        }

        public Dictionary<string, string> ObterRetorno(uint idCarta)
        {
            var carta = CartaCorrecaoDAO.Instance.GetElement(idCarta);
            var fileName = Utils.GetCartaCorrecaoXmlPath + idCarta.ToString().PadLeft(9, '0') + "-cce.xml";
            var xml = new XmlDocument();
            var dados = new Dictionary<string, string>();

            using (StreamReader r = new StreamReader(fileName))
            {
                xml.LoadXml(r.ReadToEnd());

                dados.Add("Correcao", carta.Correcao);
                dados.Add("TipoAmbiente", xml["retEnvEvento"]["retEvento"]["infEvento"]["tpAmb"].InnerText);
                dados.Add("VersaoAplicativo", xml["retEnvEvento"]["retEvento"]["infEvento"]["verAplic"].InnerText);
                dados.Add("Orgao", xml["retEnvEvento"]["retEvento"]["infEvento"]["cOrgao"].InnerText);
                dados.Add("Status", xml["retEnvEvento"]["retEvento"]["infEvento"]["cStat"].InnerText);
                dados.Add("Motivo", xml["retEnvEvento"]["retEvento"]["infEvento"]["xMotivo"].InnerText);
                dados.Add("ChaveNFE", xml["retEnvEvento"]["retEvento"]["infEvento"]["chNFe"].InnerText);
                dados.Add("TipoEvento", xml["retEnvEvento"]["retEvento"]["infEvento"]["tpEvento"].InnerText);
                dados.Add("NumSeqEvento", xml["retEnvEvento"]["retEvento"]["infEvento"]["nSeqEvento"].InnerText);
                dados.Add("DataRegistro", xml["retEnvEvento"]["retEvento"]["infEvento"]["dhRegEvento"].InnerText);
                dados.Add("NumProtocolo", xml["retEnvEvento"]["retEvento"]["infEvento"]["nProt"].InnerText);

                if (xml["retEnvEvento"]["retEvento"]["infEvento"]["xEvento"] != null)
                    dados.Add("Evento", xml["retEnvEvento"]["retEvento"]["infEvento"]["xEvento"].InnerText);
                else
                    dados.Add("Evento", ".");

                if (xml["retEnvEvento"]["retEvento"]["infEvento"]["CNPJDest"] != null)
                    dados.Add("CNPJDest", xml["retEnvEvento"]["retEvento"]["infEvento"]["CNPJDest"].InnerText);
                else
                    dados.Add("CNPJDest", ".");
            }

            return dados;
        }
    }
}