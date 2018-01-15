using System;
using System.Linq;
using Glass.Data.DAL;
using Glass.Data.DAL.CTe;
using Glass;
using GDA;

namespace WebGlass.Business.ConhecimentoTransporte.Fluxo
{
    public sealed class FinalizarCte : BaseFluxo<FinalizarCte>
    {
        private FinalizarCte() { }

        private static object _finalizarCTeLock = new object();

        public void Finalizar(uint idCte)
        {
            lock (_finalizarCTeLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        Entidade.Cte cte = BuscarCte.Instance.GetCte(transaction, idCte);

                        #region Valida dados do CT-e

                        // TODO: Fazer validações do tipo: CNPJ/Insc Est do transportador, 
                        // codigo NCM preenchido automaticamente entre outros.

                        if (cte.NumeroCte == 0)
                            throw new Exception("Informe o número do conhecimento de transporte.");

                        if (string.IsNullOrEmpty(cte.Modelo))
                            throw new Exception("Informe o modelo do conhecimento de transporte.");

                        if (cte.IdNaturezaOperacao == 0)
                            throw new Exception("Selecione a natureza de operação do conhecimento de transporte.");

                        // Verifica se o CFOP selecionado é de nota fiscal de saída
                        if (!CfopDAO.Instance.IsCfopEntrada(transaction, (int)cte.IdCfop))
                            throw new Exception("O CFOP informado no conhecimento de transporte não é um CFOP de entrada.");

                        // Verifica se o CTE já foi finalizado.
                        if (cte.Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Autorizado ||
                            cte.Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FinalizadoTerceiros)
                            throw new Exception("O CTE de número " + cte.NumeroCte + " já foi " +
                                (cte.Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Autorizado ? "autorizado" : "finalizado."));

                        //Verifica se o cfop selecionado corresponde a uf do emitente e destinatario
                        var codCfop = Glass.Conversoes.StrParaInt(CfopDAO.Instance.ObtemCodInterno(transaction, cte.IdCfop)[0].ToString());
                        var ufOrigem = CidadeDAO.Instance.GetNomeUf(transaction, cte.IdCidadeInicio);
                        var ufDestino = CidadeDAO.Instance.GetNomeUf(transaction, cte.IdCidadeFim);

                        if (ufOrigem.ToLower() == ufDestino.ToLower() && codCfop != 1)
                            throw new Exception("O CFOP informado não corresponde a um CFOP de entrada dentro do estado.");
                        else if (ufOrigem.ToLower() != ufDestino.ToLower() && codCfop != 2)
                            throw new Exception("O CFOP informado não corresponde a um CFOP de entrada fora do estado.");
                        else if (ufOrigem.ToLower() == "ex" && codCfop != 3)
                            throw new Exception("O CFOP informado não corresponde a um CFOP de entrada fora do país.");

                        if (!string.IsNullOrEmpty(cte.ChaveAcesso))
                        {
                            var cnpjChaveAcesso = cte.ChaveAcesso.Substring(6, 14);
                            var emitente = cte.ObjParticipanteCte.FirstOrDefault(x => x.TipoParticipante == Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Emitente);

                            var cpfCnpjEmitente = emitente == null ? null :
                                emitente.IdLoja > 0 ? LojaDAO.Instance.ObtemCnpj(transaction, emitente.IdLoja.Value) :
                                emitente.IdCliente > 0 ? ClienteDAO.Instance.ObtemCpfCnpj(transaction, emitente.IdCliente.Value) :
                                emitente.IdFornec > 0 ? FornecedorDAO.Instance.ObtemCpfCnpj(transaction, emitente.IdFornec.Value) :
                                emitente.IdTransportador > 0 ? TransportadorDAO.Instance.GetElementByPrimaryKey(transaction, emitente.IdTransportador.Value).CpfCnpj : null;

                            if (cpfCnpjEmitente != null && Formatacoes.LimpaCpfCnpj(cpfCnpjEmitente) != cnpjChaveAcesso)
                                throw new Exception("O CNPJ do Emitente não é o mesmo informado na chave de acesso.");
                        }

                        #endregion

                        if (cte.TipoDocumentoCte != (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoDocumentoCteEnum.EntradaTerceiros)
                        {
                            var notas = NotaFiscalCteDAO.Instance.GetCount(transaction, idCte);
                            if (notas == 0)
                                throw new Exception("Selecione ao menos uma nota fiscal que está vinculada ao CT-e.");
                        }

                        GerarContasPagar(transaction, cte);

                        // Altera a situação do CTe
                        ConhecimentoTransporteDAO.Instance.AlteraSituacao(transaction, idCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FinalizadoTerceiros);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw;
                    }
                }
            }
        }

        private static object _reabrirCTeLock = new object();

        public void Reabrir(uint idCte)
        {
            lock (_reabrirCTeLock)
            {
                using (var transaction = new GDATransaction())
                {
                    try
                    {
                        transaction.BeginTransaction();

                        var cte = BuscarCte.Instance.GetCte(transaction, idCte);

                        if (cte.Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Aberto)
                            throw new Exception(string.Format("O CTe {0} já foi reaberto.", cte.NumeroCte));

                        ApagarContasPagar(transaction, idCte);

                        // Altera a situação do CTe
                        ConhecimentoTransporteDAO.Instance.AlteraSituacao(transaction, idCte, Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Aberto);

                        transaction.Commit();
                        transaction.Close();
                    }
                    catch
                    {
                        transaction.Rollback();
                        transaction.Close();
                        throw;
                    }
                }
            }
        }

        private void GerarContasPagar(GDASession session, Entidade.Cte cte)
        {
            if (!cte.ObjCobrancaCte.GerarContasPagar)
                return;

            var duplicatas = cte.ObjCobrancaCte.ObjCobrancaDuplCte.Where(x => x.DataVenc.HasValue).ToList();

            if (cte.ObjCobrancaCte.ValorLiquidoFatura == 0)
                throw new Exception("O Valor Líquido Fatura não foi informado.");

            if (!cte.ObjCobrancaCte.GerarContasPagar || cte.ObjCobrancaCte.IdConta.GetValueOrDefault() == 0 || 
                cte.ObjCobrancaCte.ValorLiquidoFatura == 0 || duplicatas.Count == 0)
                return;

            var loja = cte.ObjParticipanteCte.FirstOrDefault(x =>
                x.IdLoja > 0 &&
                (x.TipoParticipante == Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Destinatario ||
                /* Chamado 46951. */
                x.TipoParticipante == Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Remetente));
            var tipoParticipante = cte.ObjParticipanteCte.FirstOrDefault
                (x => x.TipoParticipante == Glass.Data.Model.Cte.ParticipanteCte.TipoParticipanteEnum.Emitente);

            loja = loja ?? new Entidade.ParticipanteCte();
            tipoParticipante = tipoParticipante ?? new Entidade.ParticipanteCte();

            if (loja.IdLoja.GetValueOrDefault() == 0)
                throw new Exception("Não foi possível identificar a loja (destinatário) do CT-e.");

            if (tipoParticipante.IdFornec.GetValueOrDefault() == 0 && tipoParticipante.IdTransportador.GetValueOrDefault() == 0)
                throw new Exception("Não foi encontrado nenhum fornecedor/transportador associado ao CT-e.");

            int numParc = 1, numParcTotal = duplicatas.Count;

            foreach (var dupl in duplicatas)
            {
                var contasPagar = new Glass.Data.Model.ContasPagar()
                {
                    IdCte = cte.IdCte,
                    Contabil = true,
                    AVista = false,
                    DataVenc = dupl.DataVenc.Value,
                    IdConta = cte.ObjCobrancaCte.IdConta.Value,
                    IdLoja = loja.IdLoja.Value,
                    IdFornec = tipoParticipante.IdFornec > 0 ? (uint?)tipoParticipante.IdFornec : null,
                    IdTransportador = tipoParticipante.IdTransportador > 0 ? (uint?)tipoParticipante.IdTransportador : null,
                    NumBoleto = dupl.NumeroDupl,
                    NumParc = numParc++,
                    NumParcMax = numParcTotal,
                    Paga = false,
                    ValorVenc = dupl.ValorDupl
                };

                ContasPagarDAO.Instance.Insert(session, contasPagar);
            }
        }

        private void ApagarContasPagar(GDASession session, uint idCte)
        {
            if (ContasPagarDAO.Instance.ExistePagasCte(session, idCte))
                throw new Exception("Já existe pelo menos uma conta paga gerada por esse CT-e.");

            ContasPagarDAO.Instance.DeleteByCte(session, idCte);
        }
    }
}