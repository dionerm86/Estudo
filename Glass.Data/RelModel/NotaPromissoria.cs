using System;
using System.Collections.Generic;
using Glass.Data.Model;
using Glass.Data.DAL;
using Glass.Data.RelDAL;
using GDA;

namespace Glass.Data.RelModel
{
    [PersistenceBaseDAO(typeof(NotaPromissoriaDAO))]
    public class NotaPromissoria
    {
        #region Propriedades

        public int NumeroParc { get; set; }

        public int NumeroParcMax { get; set; }

        public decimal ValorFatura { get; set; }

        public decimal Valor { get; set; }

        public decimal Juros { get; set; }

        public DateTime DataVenc { get; set; }

        public uint IdLoja { get; set; }

        public uint IdCliente { get; set; }

        public uint? IdPedido { get; set; }

        public uint? IdLiberarPedido { get; set; }

        public uint? IdAcerto { get; set; }

        #endregion

        #region Propriedades Estendidas

        private Loja _loja = null;

        protected Loja DadosLoja
        {
            get
            {
                if (_loja == null && IdLoja > 0)
                    _loja = LojaDAO.Instance.GetElement(IdLoja);

                return _loja != null ? _loja : new Loja();
            }
        }

        private Cliente _cliente = null;

        protected Cliente DadosCliente
        {
            get
            {
                if (_cliente == null && IdCliente > 0)
                    _cliente = ClienteDAO.Instance.GetElement(IdCliente);

                return _cliente != null ? _cliente : new Cliente();
            }
        }

        #endregion

        #region Propriedades de Suporte

        public uint NumeroNotaProm
        {
            get { return IdPedido > 0 ? IdPedido.Value : IdLiberarPedido > 0 ? IdLiberarPedido.Value : IdAcerto > 0 ? IdAcerto.Value : 0; }
        }

        public string ValorExtenso
        {
            get { return Formatacoes.ValorExtenso(Valor.ToString("0.00")); }
        }
 
        public string JurosExtenso
        {
            get { return Formatacoes.ValorExtenso(Juros.ToString("0.00")); }
        }

        public string NomeLoja
        {
            get { return DadosLoja.NomeFantasia; }
        }

        public string RazaoSocialLoja
        {
            get { return DadosLoja.RazaoSocial; }
        }

        public string FoneFaxLoja
        {
            get { return DadosLoja.Telefone + " " + DadosLoja.Fax; }
        }

        public string CnpjLoja
        {
            get { return DadosLoja.Cnpj; }
        }

        public string InscEstLoja
        {
            get { return DadosLoja.InscEst; }
        }

        public string EmailLoja
        {
            get { return DadosLoja.EmailContato; }
        }

        public string SiteLoja
        {
            get { return DadosLoja.Site; }
        }

        public string EnderecoLoja
        {
            get { return DadosLoja.DescrEndereco; }
        }

        public string NomeCliente
        {
            get { return DadosCliente.Nome; }
        }

        public string CnpjCpfCliente
        {
            get { return DadosCliente.CpfCnpj; }
        }

        public string RgInscEstClient
        {
            get { return DadosCliente.RgEscinst; }
        }

        public string EnderecoCompletoCliente
        {
            get { return DadosCliente.EnderecoCompleto + " CEP: " + DadosCliente.Cep; }
        }

        public string EnderecoCliente
        {
            get { return DadosCliente.Endereco; }
        }

        public string NumeroEnderecoCliente
        {
            get { return DadosCliente.Numero; }
        }

        public string ComplementoEnderecoCliente
        {
            get { return DadosCliente.Compl; }
        }

        public string NomeCidadeCliente
        {
            get { return DadosCliente.Cidade; }
        }

        public string BairroCliente
        {
            get { return DadosCliente.Bairro; }
        }

        public string UfCliente
        {
            get { return DadosCliente.Uf; }
        }

        public string CepCliente
        {
            get { return DadosCliente.Cep; }
        }

        public string EnderecoCobCliente
        {
            get 
            {
                if (DadosCliente.EnderecoCompletoCobranca.Trim() != "- /")
                    return DadosCliente.EnderecoCompletoCobranca + " CEP: " + DadosCliente.CepCobranca;
                else
                    return "";
            }
        }

        public string NumeroNFe
        {
            get
            {
                var numerosNfe = new List<string>();

                if (IdLiberarPedido > 0 || IdPedido > 0)
                {
                    var numeroNfe = NotaFiscalDAO.Instance.ObtemNumNfePedidoLiberacao(IdLiberarPedido, IdPedido, true);

                    if (!string.IsNullOrEmpty(numeroNfe))
                        numerosNfe.Add(numeroNfe);
                }
                else if (IdAcerto > 0)
                {
                    foreach (var contaReceber in ContasReceberDAO.Instance.GetByAcerto(null, IdAcerto.Value, false))
                    {
                        var numeroNfe = string.Empty;

                        if (contaReceber.IdPedido > 0 || contaReceber.IdLiberarPedido > 0)
                        {
                            numeroNfe = NotaFiscalDAO.Instance.ObtemNumNfePedidoLiberacao(contaReceber.IdLiberarPedido, contaReceber.IdPedido, true);

                            if (!string.IsNullOrEmpty(numeroNfe) && !numerosNfe.Contains(numeroNfe))
                                numerosNfe.Add(numeroNfe);
                        }
                    }
                }

                if (numerosNfe.Count == 0)
                    numerosNfe.Add("SR");

                return string.Join(",", numerosNfe);
            }
        }

        public string TelefoneCliente
        {
            get { return DadosCliente.Telefone; }
        }

        public string NomeCidade
        {
            get { return DadosLoja.Cidade + "/" + DadosLoja.Uf; }
        }

        #endregion
    }
}