using GDA;
using Glass.Data.DAL;
using Sync.Utils.Boleto.Models;
using Sync.Utils.Boleto.Bancos;
using Sync.Utils.Boleto;
using Sync.Utils.Boleto.CodigoOcorrencia;
using System.Collections.Generic;
using System;

namespace Glass.Data.Model
{
    [PersistenceBaseDAO(typeof(DadosCnabDAO))]
    [PersistenceClass("dados_cnab")]
    public class DadosCnab : Colosoft.Data.BaseModel
    {
        #region Propiedades

        [PersistenceProperty("IdDadosCnab", PersistenceParameterType.IdentityKey)]
        public int IdDadosCnab { get; set; }

        [PersistenceProperty("IdArquivoRemessa")]
        [PersistenceForeignKey(typeof(ArquivoRemessa), "IdArquivoRemessa")]
        public int? IdArquivoRemessa { get; set; }

        [PersistenceProperty("ValorPadrao")]
        public bool ValorPadrao { get; set; }

        [PersistenceProperty("CodBanco")]
        public int CodBanco { get; set; }

        [PersistenceProperty("TipoCnab")]
        public int TipoCnab { get; set; }

        [PersistenceProperty("CodOcorrencia")]
        public int CodOcorrencia { get; set; }

        [PersistenceProperty("CodMovRemessa")]
        public int CodMovRemessa { get; set; }

        [PersistenceProperty("CodCarteira")]
        public int CodCarteira { get; set; }

        [PersistenceProperty("CodCadastramento")]
        public int CodCadastramento { get; set; }

        [PersistenceProperty("CodEspecieDocumento")]
        public int CodEspecieDocumento { get; set; }

        [PersistenceProperty("Aceite")]
        public int Aceite { get; set; }

        [PersistenceProperty("JurosMoraCod")]
        public int JurosMoraCod { get; set; }

        [PersistenceProperty("JurosMoraDias")]
        public int JurosMoraDias { get; set; }

        [PersistenceProperty("JurosMoraValor")]
        public decimal JurosMoraValor { get; set; }

        [PersistenceProperty("DescontoCod")]
        public int DescontoCod { get; set; }

        [PersistenceProperty("DescontoDias")]
        public int DescontoDias { get; set; }

        [PersistenceProperty("DescontoValor")]
        public decimal DescontoValor { get; set; }

        [PersistenceProperty("ProtestoCod")]
        public int ProtestoCod { get; set; }

        [PersistenceProperty("ProtestoDias")]
        public int ProtestoDias { get; set; }

        [PersistenceProperty("MultaCod")]
        public int MultaCod { get; set; }

        [PersistenceProperty("MultaValor")]
        public decimal MultaValor { get; set; }

        [PersistenceProperty("BaixaDevolucaoCod")]
        public int BaixaDevolucaoCod { get; set; }

        [PersistenceProperty("BaixaDevolucaoDias")]
        public int BaixaDevolucaoDias { get; set; }

        [PersistenceProperty("Iof")]
        public decimal Iof { get; set; }

        [PersistenceProperty("Abatimento")]
        public decimal Abatimento { get; set; }

        [PersistenceProperty("Instrucao1")]
        public int Instrucao1 { get; set; }

        [PersistenceProperty("Instrucao2")]
        public int Instrucao2 { get; set; }

        [PersistenceProperty("Mensagem")]
        public string Mensagem { get; set; }

        [PersistenceProperty("EmissaoBloqueto")]
        public int EmissaoBloqueto { get; set; }

        [PersistenceProperty("DistribuicaoBloqueto")]
        public int DistribuicaoBloqueto { get; set; }

        [PersistenceProperty("TipoDocumento")]
        public int TipoDocumento { get; set; }

        [PersistenceProperty("CodMoeda")]
        public int CodMoeda { get; set; }

        #endregion

        #region Propiedades de Suporte

        public string DescInstrucoes
        {
            get
            {
                var descr = "";

                if (((Sync.Utils.CodigoBanco)CodBanco) != Sync.Utils.CodigoBanco.Sicredi) {

                    if (Instrucao1 > 0)
                        descr += new Sync.Utils.Boleto.Instrucoes.Instrucao((TipoArquivo)TipoCnab, CodBanco, Instrucao1, JurosMoraDias, JurosMoraValor,
                            DescontoDias, DescontoValor, ProtestoDias, MultaValor, BaixaDevolucaoDias)
                            .Descricao + Environment.NewLine;

                    if (Instrucao2 > 0)
                        descr += new Sync.Utils.Boleto.Instrucoes.Instrucao((TipoArquivo)TipoCnab, CodBanco, Instrucao2, JurosMoraDias, JurosMoraValor,
                           DescontoDias, DescontoValor, ProtestoDias, MultaValor, BaixaDevolucaoDias)
                           .Descricao + Environment.NewLine;
                }

                if (!string.IsNullOrEmpty(Mensagem))
                    descr += Mensagem;


                return descr.ToUpper();
            }
        }

        #endregion

        #region Conversão Explicita para o Boleto

        public static implicit operator Boletos(DadosCnab d)
        {
            var boletos = new Boletos();

            boletos.Aceite = d.Aceite.ToString();

            boletos.BaixaDevolucao = new Sync.Utils.Boleto.Models.BaixaDevolucao()
            {
                Codigo = d.BaixaDevolucaoCod,
                Prazo = d.BaixaDevolucaoDias
            };

            boletos.Banco = new Banco(d.CodBanco);

            boletos.CaracteristicaCobranca = new CaracteristicaCobranca()
            {
                Cadastramento = (Cadastramento)d.CodCadastramento,
                Carteira = new Sync.Utils.Boleto.Carteiras.Carteira(d.CodBanco, d.CodCarteira),
                DistribuicaoBloqueto = d.DistribuicaoBloqueto,
                EmissaoBloqueto = d.EmissaoBloqueto,
                TipoDocumento = (TipoDocumento)d.TipoDocumento
            };

            boletos.CodigoBanco = d.CodBanco;
            boletos.CodigoMoeda = (CodigoMoeda)d.CodMoeda;
            boletos.CodigoMovimentoRemessa = (CodigoMovimentoRemessa)d.CodMovRemessa;
            boletos.CodigoOcorrencia = (CodigoOcorrencia)d.CodOcorrencia;
            boletos.DadosDebito = null;

            boletos.Desconto = new Desconto()
            {
                Codigo = d.DescontoCod,
                Dias = d.DescontoDias,
                Valor = d.DescontoValor
            };

            boletos.EspecieTitulo = new Sync.Utils.Boleto.EspecieDocumentos.EspecieDocumento((TipoArquivo)d.TipoCnab, d.CodBanco, d.CodEspecieDocumento);

            boletos.JurosMora = new JurosMora()
            {
                Codigo = d.JurosMoraCod,
                Dias = d.JurosMoraDias,
                Valor = d.JurosMoraValor
            };

            var bancosMulta = new List<int>()
            {
                (int)Sync.Utils.CodigoBanco.Bradesco,
                (int)Sync.Utils.CodigoBanco.Sicoob,
                (int)Sync.Utils.CodigoBanco.Sicredi,
                (int)Sync.Utils.CodigoBanco.Santander
            };

            boletos.Multa = !bancosMulta.Contains(d.CodBanco) ? null : new Multa()
            {
                Codigo = d.MultaCod,
                Valor = d.MultaValor
            };

            boletos.Protesto = new Protesto()
            {
                Codigo = d.ProtestoCod,
                Prazo = d.ProtestoDias
            };

            boletos.TipoCnab = d.TipoCnab;
            boletos.ValorAbatimento = d.Abatimento;
            boletos.ValorIOF = d.Iof;

            return boletos;
        }

        #endregion
    }
}
