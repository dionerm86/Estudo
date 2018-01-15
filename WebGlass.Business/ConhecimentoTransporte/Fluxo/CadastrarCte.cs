using System;
using System.Linq;
using Glass.Data.Helper;
using System.Xml;
using Glass.Data.DAL;
using Glass.Configuracoes;
using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public class CadastrarCte : BaseFluxo<CadastrarCte>
    {
        private CadastrarCte() { }

        private static object _cadastrarCte = new object();

        /// <summary>
        /// insere cte
        /// </summary>
        public uint Insert(Entidade.Cte cte)
        {
            lock (_cadastrarCte)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var idCte = new uint();

                        var emitente = cte.ObjParticipanteCte.Where(c => c.TipoParticipante == Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Emitente).FirstOrDefault();

                        if (emitente == null)
                            throw new Exception("O emitente não foi informado.");

                        var idCidade = emitente.IdLoja > 0 ? LojaDAO.Instance.ObtemIdCidade(transaction, emitente.IdLoja.Value) :
                            emitente.IdCliente > 0 ? ClienteDAO.Instance.ObtemIdCidade(transaction, emitente.IdCliente.Value) :
                            emitente.IdFornec > 0 ? FornecedorDAO.Instance.ObtemIdCidade(transaction, (int)emitente.IdFornec.Value) :
                            emitente.IdTransportador > 0 ? (uint?)TransportadorDAO.Instance.GetElementByPrimaryKey(transaction, emitente.IdTransportador.Value).IdCidade : (uint?)null;

                        if (!idCidade.HasValue)
                            throw new Exception("Não foi possível recuperar a cidade do emitente.");

                        var cidadeEmitente = CidadeDAO.Instance.GetElementByPrimaryKey(transaction, idCidade.Value);

                        if (Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.CteCadastrado(transaction))
                            throw new Exception("CTe cadastrado há poucos segundos, Tente novamente.");

                        cte.TipoEmissao = cte.TipoEmissao > 0 ? cte.TipoEmissao : 1;

                        if (cte.TipoDocumentoCte == (int)Entidade.Cte.TipoDocumentoCteEnum.Saida)
                        {
                            var idLoja = emitente.IdLoja;

                            if (cte.TipoEmissao != (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.AutorizacaoSvcRs &&
                                cte.TipoEmissao != (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.AutorizacaoSvcSp)
                                cte.NumeroCte = BuscarCte.Instance.GetUltimoNumeroCte(transaction, idLoja.Value, Glass.Conversoes.StrParaInt(cte.Serie), (int)cte.TipoEmissao);

                            cte.DataEmissao = DateTime.Now;
                            cte.CodAleatorio = (cte.NumeroCte + (cte.TipoEmissao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.Normal ? 10203040 : 9020304)).ToString();
                            cte.ChaveAcesso = ChaveDeAcesso(cidadeEmitente.CodIbgeUf, cte.DataEmissao.ToString("yyMM"), LojaDAO.Instance.ObtemCnpj(transaction, idLoja.Value), Glass.Data.CTeUtils.ConfigCTe.Modelo,
                                cte.Serie.ToString().PadLeft(3, '0'), cte.NumeroCte.ToString(), cte.TipoEmissao.ToString(), cte.CodAleatorio);
                        }
                        else if (cte.TipoDocumentoCte == (int)Entidade.Cte.TipoDocumentoCteEnum.EntradaTerceiros)
                            if (!string.IsNullOrEmpty(cte.ChaveAcesso) && cte.ChaveAcesso.Length > 0 && cte.ChaveAcesso.Length != 44)
                                throw new Exception("A chave de acesso deve ter 44 caracteres.");

                        cte.Situacao = (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Aberto;

                        idCte = Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.Insert(transaction, Convert(cte));

                        cte.ObjCobrancaCte.IdCte = cte.ObjEntregaCte.IdCte = cte.ObjInfoCte.IdCte =
                        cte.ObjConhecimentoTransporteRodoviario.IdCte = cte.ObjSeguroCte.IdCte =
                        cte.ObjComplCte.IdCte = cte.ObjComplCte.ObjComplPassagemCte.IdCte = cte.ObjEfdCte.IdCte = idCte;

                        foreach (var i in cte.ObjParticipanteCte)
                            i.IdCte = idCte;

                        foreach (var i in cte.ObjComponenteValorCte)
                            i.IdCte = idCte;

                        foreach (var i in cte.ObjVeiculoCte)
                            i.IdCte = idCte;

                        foreach (var i in cte.ObjImpostoCte)
                            i.IdCte = idCte;

                        CadastrarCobrancaCte.Instance.Insert(transaction, cte.ObjCobrancaCte);
                        CadastrarVeiculoCte.Instance.Insert(transaction, cte.ObjVeiculoCte);
                        if (cte.ObjSeguroCte.IdSeguradora > 0)
                            CadastrarSeguroCte.Instance.Insert(transaction, cte.ObjSeguroCte);
                        CadastrarEntregaCte.Instance.Insert(transaction, cte.ObjEntregaCte);
                        CadastrarComponenteValorCte.Instance.Insert(transaction, cte.ObjComponenteValorCte);
                        CadastrarInfoCte.Instance.Insert(transaction, cte.ObjInfoCte);
                        CadastrarConhecimentoTransporteRodoviario.Instance.Insert(transaction, cte.ObjConhecimentoTransporteRodoviario);
                        CadastrarParticipanteCte.Instance.Insert(transaction, cte.ObjParticipanteCte);
                        CadastrarComplCte.Instance.Insert(transaction, cte.ObjComplCte);
                        CadastrarImpostoCte.Instance.Insert(transaction, cte.ObjImpostoCte);
                        CadastrarEfdCte.Instance.Insert(transaction, cte.ObjEfdCte);

                        transaction.Commit();
                        transaction.Close();

                        return idCte;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException("CadastrarCTE(Insert).", ex);
                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Erro no cadastro de CTe.", ex));
                    }
                }
            }
        }

        /// <summary>
        /// atualiza dados do cte
        /// </summary>
        public void Update(Entidade.Cte cte)
        {
            lock(_cadastrarCte)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var situacaoCTe = Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.ObtemSituacaoCte(transaction, cte.IdCte);

                        if (situacaoCTe == Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Autorizado ||
                            situacaoCTe == Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FinalizadoTerceiros)
                            throw new Exception(string.Format("O CTE de número {0} já foi {1}.", cte.NumeroCte,
                                situacaoCTe == Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Autorizado ? "autorizado" : "finalizado"));

                        var emitente = cte.ObjParticipanteCte.Where(c => c.TipoParticipante == Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Emitente).FirstOrDefault();
                      
                        if (emitente == null)
                            throw new Exception("O emitente não foi informado.");

                        if (cte.TipoDocumentoCte == (int)Entidade.Cte.TipoDocumentoCteEnum.Saida)
                        {
                            var idCidade = emitente.IdLoja > 0 ? LojaDAO.Instance.ObtemIdCidade(transaction, emitente.IdLoja.Value) :
                                emitente.IdCliente > 0 ? ClienteDAO.Instance.ObtemIdCidade(transaction, emitente.IdCliente.Value) :
                                emitente.IdFornec > 0 ? FornecedorDAO.Instance.ObtemIdCidade(transaction, (int)emitente.IdFornec.Value) :
                                emitente.IdTransportador > 0 ? (uint?)TransportadorDAO.Instance.GetElementByPrimaryKey(transaction, emitente.IdTransportador.Value).IdCidade : (uint?)null;

                            if (!idCidade.HasValue)
                                throw new Exception("Não foi possível recuperar a cidade do emitente.");

                            var cidadeEmitente = CidadeDAO.Instance.GetElementByPrimaryKey(transaction, idCidade.Value);
                            var idLoja = emitente.IdLoja;

                            cte.ChaveAcesso = ChaveDeAcesso(cidadeEmitente.CodIbgeUf, cte.DataEmissao.ToString("yyMM"), LojaDAO.Instance.ObtemCnpj(transaction, idLoja.Value),
                                Glass.Data.CTeUtils.ConfigCTe.Modelo, cte.Serie.ToString().PadLeft(3, '0'), cte.NumeroCte.ToString(),
                                cte.TipoEmissao.ToString(), cte.CodAleatorio);
                        }
                        else if (cte.TipoDocumentoCte == (int)Entidade.Cte.TipoDocumentoCteEnum.EntradaTerceiros)
                            if (!string.IsNullOrEmpty(cte.ChaveAcesso) && cte.ChaveAcesso.Length > 0 && cte.ChaveAcesso.Length != 44)
                                throw new Exception("A chave de acesso deve ter 44 caracteres.");

                        cte.Situacao = (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Aberto;

                        var id = Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.Update(transaction, Convert(cte));

                        cte.ObjCobrancaCte.IdCte = cte.ObjEntregaCte.IdCte = cte.ObjInfoCte.IdCte = cte.ObjEfdCte.IdCte =
                        cte.ObjConhecimentoTransporteRodoviario.IdCte = cte.ObjSeguroCte.IdCte =
                        cte.ObjComplCte.IdCte = cte.ObjComplCte.ObjComplPassagemCte.IdCte = cte.IdCte;

                        foreach (var i in cte.ObjParticipanteCte)
                            i.IdCte = cte.IdCte;

                        foreach (var i in cte.ObjComponenteValorCte)
                            i.IdCte = cte.IdCte;

                        foreach (var i in cte.ObjVeiculoCte)
                            i.IdCte = cte.IdCte;

                        foreach (var i in cte.ObjImpostoCte)
                            i.IdCte = cte.IdCte;

                        CadastrarCobrancaCte.Instance.Update(transaction, cte.ObjCobrancaCte);
                        CadastrarVeiculoCte.Instance.Update(transaction, cte.ObjVeiculoCte);
                        CadastrarSeguroCte.Instance.Update(transaction, cte.ObjSeguroCte);
                        CadastrarEntregaCte.Instance.Update(transaction, cte.ObjEntregaCte);

                        if (cte.ObjComponenteValorCte.Count() > 0 && cte.ObjComponenteValorCte.Select(f => !string.IsNullOrEmpty(f.NomeComponente)
                            && f.ValorComponente != 0).FirstOrDefault())
                            CadastrarComponenteValorCte.Instance.Update(transaction, cte.ObjComponenteValorCte);

                        CadastrarConhecimentoTransporteRodoviario.Instance.Update(transaction, cte.ObjConhecimentoTransporteRodoviario);
                        CadastrarParticipanteCte.Instance.Update(transaction, cte.ObjParticipanteCte);
                        CadastrarComplCte.Instance.Update(transaction, cte.ObjComplCte);
                        CadastrarImpostoCte.Instance.Update(transaction, cte.ObjImpostoCte);
                        CadastrarInfoCte.Instance.Update(transaction, cte.ObjInfoCte);
                        CadastrarEfdCte.Instance.Update(transaction, cte.ObjEfdCte);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        transaction.Close();

                        ErroDAO.Instance.InserirFromException("CadastrarCTE(Update).", ex);
                        throw new Exception(Glass.MensagemAlerta.FormatErrorMsg("Erro ao atualizar CTe.", ex));
                    }
                }
            }
        }

        /// <summary>
        /// Atualiza motivo de cancelamento do cte
        /// </summary>
        /// <param name="idCte"></param>
        /// <param name="justificativa"></param>
        /// <returns></returns>
        public int UpdateMotivoCanc(uint idCte, string justificativa)
        {
            return Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.UpdateMotivoCanc(idCte, justificativa);
        }

        /// <summary>
        /// altera situação do cte
        /// </summary>
        /// <param name="idCte"></param>
        /// <param name="situacao"></param>
        /// <returns></returns>
        public int AlteraSituacao(uint idCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum situacao)
        {
            using (Glass.Data.DAL.CTe.ConhecimentoTransporteDAO dao = Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance)
            {
                return dao.AlteraSituacao(idCte, situacao);
            }
        }

        /// <summary>
        /// cria chave de acesso
        /// </summary>
        /// <param name="cUf"></param>
        /// <param name="aamm"></param>
        /// <param name="cnpj"></param>
        /// <param name="mod"></param>
        /// <param name="serie"></param>
        /// <param name="nCte"></param>
        /// <param name="tpEmis"></param>
        /// <param name="cCte"></param>
        /// <returns></returns>
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
        /// calcula dígito verificador do cte
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

        /// <summary>
        /// Converte dados da entidade para a model
        /// </summary>
        /// <param name="cte"></param>
        /// <returns></returns>
        internal Glass.Data.Model.Cte.ConhecimentoTransporte Convert(WebGlass.Business.ConhecimentoTransporte.Entidade.Cte cte)
        {
            return new Glass.Data.Model.Cte.ConhecimentoTransporte
            {
                IdCte = cte.IdCte,
                ChaveAcesso = cte.ChaveAcesso,
                CodAleatorio = cte.CodAleatorio,
                DataEmissao = cte.DataEmissao,
                DataEntradaSaida = cte.DataEntradaSaida,
                DetalhesRetirada = cte.DetalhesRetirada,
                IdNaturezaOperacao = cte.IdNaturezaOperacao,
                IdCidadeCte = cte.IdCidadeCte,
                //IdCidadeDestFrete = cte.IdCidadeDestFrete,
                IdCidadeFim = cte.IdCidadeFim,
                IdCidadeInicio = cte.IdCidadeInicio,
                //IdCidadeOrigFrete = cte.IdCidadeOrigFrete,
                IdCteAnterior = cte.IdCteAnterior,
                InformAdicionais = cte.InformAdicionais,
                Modelo = cte.Modelo,
                NumeroCte = cte.NumeroCte,
                Retirada = cte.Retirada,
                Serie = cte.Serie,
                TipoCte = cte.TipoCte,
                TipoEmissao = cte.TipoEmissao,
                TipoServico = cte.TipoServico,
                ValorReceber = cte.ValorReceber,
                ValorTotal = cte.ValorTotal,
                Situacao = cte.Situacao,
                TipoDocumentoCte = cte.TipoDocumentoCte,
                GerarContasReceber = cte.GerarContasReceber
            };
        }

        #region Cria cte em contingência

        /// <summary>
        /// Cria cte em contingência a partir de outro cte
        /// </summary>
        /// <param name="idCte"></param>
        public uint CriaCTeContingencia(WebGlass.Business.ConhecimentoTransporte.Entidade.Cte cte)
        {
            uint idCte = 0;

            if (FiscalConfig.ConhecimentoTransporte.ContingenciaCTe == DataSources.TipoContingenciaCTe.SVC && cte.TipoEmissao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.Normal)
            {
                var idLoja = Fluxo.BuscarParticipanteCte.Instance.GetParticipanteByIdCteTipo(cte.IdCte, (int)Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Emitente).IdLoja;
                var emitente = Glass.Data.DAL.LojaDAO.Instance.GetElement(idLoja.Value);
                var idCidadeLojaEmitente = emitente.IdCidade != null ? emitente.IdCidade.ToString() : "0";
                var cidadeEmitente = Glass.Data.DAL.CidadeDAO.Instance.GetElementByPrimaryKey(Glass.Conversoes.StrParaUint(idCidadeLojaEmitente));

                switch (cidadeEmitente.NomeUf)
                {
                    case "AP":
                    case "MT":
                    case "MS":
                    case "PE":
                    case "RR":
                    case "SP":
                        cte.TipoEmissao = (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.AutorizacaoSvcRs; break;
                    case "AC":
                    case "AL":
                    case "AM":
                    case "BA":
                    case "CE":
                    case "DF":
                    case "ES":
                    case "GO":
                    case "MA":
                    case "MG":
                    case "PA":
                    case "PB":
                    case "PI":
                    case "PR":
                    case "RJ":
                    case "RN":
                    case "RO":
                    case "RS":
                    case "SC":
                    case "SE":
                    case "TO":
                        cte.TipoEmissao = (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.AutorizacaoSvcSp; break;
                }

                cte.NumeroCte = GetUltimoNumeroCte(idLoja.Value, Glass.Conversoes.StrParaInt(cte.Serie), cte.TipoEmissao);

                cte.CodAleatorio = (cte.NumeroCte + (cte.TipoEmissao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.Normal ? 10203040 : 9020304)).ToString();                

                cte.ChaveAcesso = ChaveDeAcesso(cidadeEmitente.CodIbgeUf, cte.DataEmissao.ToString("yyMM"), emitente.Cnpj, Glass.Data.CTeUtils.ConfigCTe.Modelo,
                    cte.Serie.ToString().PadLeft(3, '0'), cte.NumeroCte.ToString(), cte.TipoEmissao.ToString(), cte.CodAleatorio);

                idCte = Fluxo.CadastrarCte.Instance.Insert(cte);

                // Insere notas fiscais no cte
                foreach (var nfCte in Fluxo.BuscarNotaFiscalCte.Instance.GetList(cte.IdCte, "", 0, 0))
                {
                    nfCte.IdCte = idCte;
                    Fluxo.CadastrarNotaFiscalCte.Instance.Insert(nfCte);
                }

            }

            return idCte;
        }

        #endregion

        #region Retorna próxima numeração do CTe

        /// <summary>
        /// Retorna o próximo número de CTe a ser utilizado
        /// </summary>
        /// <param name="idLoja"></param>
        /// <returns></returns>
        public int GetUltimoNumeroCte(uint idLoja, int serie, int tipoEmissao)
        {
            return Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.GetUltimoNumeroCte(idLoja, serie, tipoEmissao);
        }

        #endregion

        #region Obtém CTe pela chave de acesso

        /// <summary>
        /// Obtém cte pela chave de acesso
        /// </summary>
        /// <param name="chaveAcesso"></param>
        /// <returns></returns>
        public Entidade.Cte GetByChaveAcesso(string chaveAcesso)
        {
            return BuscarCte.Instance.GetCte(Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.GetByChaveAcesso(chaveAcesso).IdCte);
        }

        #endregion

        /// <summary>
        /// Emite cte
        /// </summary>
        /// <param name="idCte"></param>
        /// <param name="preVisualizar"></param>
        /// <returns></returns>
        public XmlDocument EmitirCTe(uint idCte, bool preVisualizar)
        {
            return Glass.Data.DAL.CTe.ConhecimentoTransporteDAO.Instance.EmitirCTe(idCte, preVisualizar);
        }
    }
}
