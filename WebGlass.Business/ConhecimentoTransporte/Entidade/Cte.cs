using System;
using System.Collections.Generic;
using Glass.Data.DAL;
using Glass.Data.DAL.CTe;

namespace WebGlass.Business.ConhecimentoTransporte.Entidade
{
    [Serializable]
    public class Cte
    {
        public enum TipoDocumentoCteEnum
        {
            Saida = Glass.Data.Model.Cte.ConhecimentoTransporte.TipoDocumentoCteEnum.Saida,
            EntradaTerceiros = Glass.Data.Model.Cte.ConhecimentoTransporte.TipoDocumentoCteEnum.EntradaTerceiros
        }

        private Glass.Data.Model.Cte.ConhecimentoTransporte _cte;

        #region Contrutores
      
        public Cte()
        {
            _cte = new Glass.Data.Model.Cte.ConhecimentoTransporte();
        }

        public Cte(Glass.Data.Model.Cte.ConhecimentoTransporte cte)
        {
            _cte = cte;
        }

        #endregion

        #region Propriedades

        public uint IdCte
        {
            get { return _cte.IdCte; }
            set { _cte.IdCte = value; }
        }

        public int TipoDocumentoCte
        {
            get { return _cte.TipoDocumentoCte; }
            set { _cte.TipoDocumentoCte = value; }
        }

        public uint IdNaturezaOperacao
        {
            get { return _cte.IdNaturezaOperacao; }
            set { _cte.IdNaturezaOperacao = value; }
        }

        public uint IdCfop
        {
            get { return _cte.IdCfop; }
        }

        public uint IdCidadeCte 
        {
            get { return _cte.IdCidadeCte; }
            set { _cte.IdCidadeCte = value; }
        }

        public uint IdCidadeInicio 
        {
            get { return _cte.IdCidadeInicio; }
            set { _cte.IdCidadeInicio = value; }
        }

        public uint IdCidadeFim 
        {
            get { return _cte.IdCidadeFim; }
            set { _cte.IdCidadeFim = value; }
        }

        //public uint IdCidadeOrigFrete 
        //{
        //    get { return _cte.IdCidadeOrigFrete; }
        //    set { _cte.IdCidadeOrigFrete = value; }
        //}

        //public uint IdCidadeDestFrete 
        //{
        //    get { return _cte.IdCidadeDestFrete; }
        //    set { _cte.IdCidadeDestFrete = value; }
        //}

        public uint? IdCteAnterior 
        {
            get { return _cte.IdCteAnterior; }
            set { _cte.IdCteAnterior = value; }
        }

        public int NumeroCte 
        {
            get { return _cte.NumeroCte; }
            set { _cte.NumeroCte = value; }
        }

        public string CodAleatorio 
        {
            get { return _cte.CodAleatorio; }
            set { _cte.CodAleatorio = value; }
        }
        
        public string Modelo 
        {
            get { return _cte.Modelo; }
            set { _cte.Modelo = value; }
        }

        public string Serie 
        {
            get { return _cte.Serie; }
            set { _cte.Serie = value; }
        }

        public DateTime DataEmissao 
        {
            get { return _cte.DataEmissao; }
            set { _cte.DataEmissao = value; }
        }

        public DateTime? DataEntradaSaida
        {
            get { return _cte.DataEntradaSaida; }
            set { _cte.DataEntradaSaida = value; }
        }

        public int TipoEmissao 
        {
            get { return _cte.TipoEmissao; }
            set { _cte.TipoEmissao = value; }
        }

        public string TipoEmissaoString
        {
            get
            {
                return TipoEmissao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.AutorizacaoSvcRs ? "Autorizacao SvcRs" :
                       TipoEmissao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.AutorizacaoSvcSp ? "Autorizacao SvcSp" :
                       TipoEmissao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.ContingenciaFsda ? "Contingencia Fsda" :
                       TipoEmissao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoEmissaoEnum.Normal ? "Normal" :
                    String.Empty;
            }
        }

        public int TipoCte 
        {
            get { return _cte.TipoCte; }
            set { _cte.TipoCte = value; }
        }

        public string TipoCteString
        {
            get
            {
                return TipoCte == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoCteEnum.AnulacaoValores ? "Anulacao Valores" :
                       TipoCte == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoCteEnum.ComplementoValores ? "Complemento Valores" :
                       TipoCte == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoCteEnum.Normal ? "Normal" :
                       TipoCte == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoCteEnum.Substituto ? "Substituto" :
                    String.Empty;
            }
        }

        public string ChaveAcesso 
        {
            get { return _cte.ChaveAcesso; }
            set { _cte.ChaveAcesso = value; }
        }

        public int TipoServico 
        {
            get { return _cte.TipoServico; }
            set { _cte.TipoServico = value; }
        }

        public string TipoServicoString
        {
            get
            {
                return TipoServico == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoServicoEnum.Normal ? "Normal" :
                       TipoServico == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoServicoEnum.Redespacho ? "Redespacho" :
                       TipoServico == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoServicoEnum.RedespachoIntermediario ? "RedespachoIntermediario" :
                       TipoServico == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoServicoEnum.Subcontratacao ? "Subcontratacao" :
                    String.Empty;
            }
        }

        public bool Retirada 
        {
            get { return _cte.Retirada; }
            set { _cte.Retirada = value; }
        }

        public string DetalhesRetirada 
        {
            get { return _cte.DetalhesRetirada; }
            set { _cte.DetalhesRetirada = value; }
        }

        public decimal ValorTotal 
        {
            get { return _cte.ValorTotal; }
            set { _cte.ValorTotal = value; }
        }

        public decimal ValorReceber 
        {
            get { return _cte.ValorReceber; }
            set { _cte.ValorReceber = value; }
        }

        public string InformAdicionais 
        {
            get { return _cte.InformAdicionais; }
            set { _cte.InformAdicionais = value; }
        }

        public int Situacao 
        {
            get { return _cte.Situacao; }
            set { _cte.Situacao = value; }
        }

        public bool CentroCustoCompleto
        {
            get { return _cte.CentroCustoCompleto; }
        }

        public bool ExibirCentroCusto
        {
            get { return _cte.ExibirCentroCusto; }
        }

        public string SituacaoString
        {
            get
            {
                return Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Aberto ? "Aberto" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Autorizado ? "Autorizado" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.NaoEmitido ? "Não emitido" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Cancelado ? "Cancelado" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Inutilizado ? "Inutilizado" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Denegado ? "Denegado" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.ProcessoEmissao ? "Processo de emissão" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.ProcessoCancelamento ? "Processo de cancelamento" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.ProcessoInutilizacao ? "Processo de inutilização" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaEmitir ? "Falha ao emitir" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaCancelar ? "Falha ao cancelar" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaInutilizar ? "Falha ao inutilizar" :
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FinalizadoTerceiros ? "Finalizado" :
                    String.Empty;
            }
        }

        public string MotivoCanc { get; set; }

        public string MotivoInut { get; set; }

        public int NumeroLote { get; set; }

        public CobrancaCte ObjCobrancaCte { get; set; }

        public List<VeiculoCte> ObjVeiculoCte { get; set; }

        public SeguroCte ObjSeguroCte { get; set; }

        public EntregaCte ObjEntregaCte { get; set; }

        public List<ComponenteValorCte> ObjComponenteValorCte { get; set; }

        public InfoCte ObjInfoCte { get; set; }

        public List<ImpostoCte> ObjImpostoCte { get; set; }

        public ConhecimentoTransporteRodoviario ObjConhecimentoTransporteRodoviario { get; set; }

        public List<ParticipanteCte> ObjParticipanteCte { get; set; }

        public ComplCte ObjComplCte { get; set; }

        public EfdCte ObjEfdCte { get; set; }

        public Glass.Data.Model.Cfop Cfop { get; set; }

        public string EmitenteCte
        {
            get { return _cte.EmitenteCte; }
        }

        public string RemetenteCte
        {
            get { return _cte.RemetenteCte; }
        }

        public string DestinatarioCte
        {
            get { return _cte.DestinatarioCte; }
        }

        public string ExpedidorCte
        {
            get { return _cte.expedidorCte; }
        }

        public string RecebedorCte
        {
            get { return _cte.RecebedorCte; }
        }

        public bool GerarContasReceber
        {
            get
            {
                return _cte.GerarContasReceber;
            }
            set
            {
                _cte.GerarContasReceber = value;
            }
        }

        #endregion

        #region Propriedades de suporte

        public string ChaveAcessoExibir
        {
            get { return Glass.Formatacoes.MascaraChaveAcessoCTe(ChaveAcesso); }
        }

        public string CodigoNaturezaOperacao
        {
            get { return NaturezaOperacaoDAO.Instance.ObtemCodigoCompleto(IdNaturezaOperacao); }
        }

        public string CodigoCfop
        {
            get { return CfopDAO.Instance.ObtemCodInterno(IdCfop); }
        }

        public string DescricaoCfop
        {
            get { return CfopDAO.Instance.GetDescricao((uint)IdCfop); }
        }

        public string DescricaoTipoDocumentoCte
        {
            get
            {
                switch (_cte.TipoDocumentoCte)
                {
                    case (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoDocumentoCteEnum.Saida: return "Saída";
                    case (int)Glass.Data.Model.Cte.ConhecimentoTransporte.TipoDocumentoCteEnum.EntradaTerceiros: return "Entrada (terceiros)";
                    default: return String.Empty;
                }
            }
        }

        public bool ConsSitVisible
        {
            get
            {
                return Situacao == 7 || Situacao == 8 || Situacao == 9 || Situacao == 10 || 
                    Situacao == 11 || Situacao == 12;
            }
        }

        public bool PrintDacteVisible
        {
            get
            {
                var protocolo = Glass.Data.DAL.CTe.ProtocoloCteDAO.Instance.GetElement(IdCte, (int)Glass.Data.Model.Cte.ProtocoloCte.TipoProtocoloEnum.Autorizacao);
                var numProtocolo = protocolo != null ? protocolo.NumProtocolo : "";
                return Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Autorizado || ((Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Cancelado ||
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FalhaCancelar) && !String.IsNullOrEmpty(numProtocolo));
            }
        }

        public bool PrintCteTercVisible
        {
            get { return Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FinalizadoTerceiros; }
        }
        
        public bool BaixarXmlVisible
        {
            get
            {
                return PrintDacteVisible;
            }
        }

        public bool ExibirReabrir
        {
            get { return Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.FinalizadoTerceiros; }
        }

        public bool ExibirDocRef
        {
            get { return TipoDocumentoCte == (int)TipoDocumentoCteEnum.EntradaTerceiros /*&& !String.IsNullOrEmpty(_infCompl) */; }
        }

        public bool ExibirExcluir
        {
            get
            {
                return TipoDocumentoCte == (int)TipoDocumentoCteEnum.EntradaTerceiros &&
                    Situacao == (int)Glass.Data.Model.Cte.ConhecimentoTransporte.SituacaoEnum.Aberto;
            }
        }

        public string NomeCidadeCte
        {
            get { return CidadeDAO.Instance.GetNome(IdCidadeCte); }
        }

        public string NomeCidadeInicio
        {
            get { return CidadeDAO.Instance.GetNome(IdCidadeInicio); }
        }

        public string NomeCidadeFim
        {
            get { return CidadeDAO.Instance.GetNome(IdCidadeFim); }
        }

        public uint? NumeroCteAnterior
        {
            get { return IdCteAnterior > 0 ? ConhecimentoTransporteDAO.Instance.ObtemNumeroCte(IdCteAnterior.Value) : (uint?)null; }
        }

        #endregion
    }
}
