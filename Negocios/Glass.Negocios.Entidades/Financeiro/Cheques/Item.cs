using Glass.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Financeiro.Negocios.Entidades.Cheques
{
    /// <summary>
    /// Separação de informações do CMC7 do cheque.
    /// </summary>
    public class Cmc7
    {
        public List<string> _numCmc7;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="Cmc7"/>.
        /// </summary>
        /// <param name="numCmc7">número bruto do CMC7</param>
        public Cmc7(string numCmc7)
        {
            if (string.IsNullOrEmpty(numCmc7))
                throw new Exception("CMC7 do cheque não encontrado.");

            _numCmc7 = numCmc7.Replace('<', '|').Replace('>', '|').Split('|').ToList();
        }

        /// <summary>
        /// Obtém o código do banco.
        /// </summary>
        public string Banco => _numCmc7[0].Substring(0, 3);

        /// <summary>
        /// Obtém o código de compensação.
        /// </summary>
        public string CodCompensacao => _numCmc7[1].Substring(0, 3);

        /// <summary>
        /// Obtém o dígito verificador 2 do CMC7.
        /// </summary>
        public string DigitoVerificador2 => _numCmc7[0].Last().ToString();

        /// <summary>
        /// Obtém a tipificação do CMC7.
        /// </summary>
        public string Tipificacao => _numCmc7[1].Last().ToString();

        /// <summary>
        /// Obtém o dígito verificador 1 do CMC7.
        /// </summary>
        public string DigitoVerificador1 => _numCmc7[2].First().ToString();

        /// <summary>
        /// Obtém o dígito verificador 3 do CMC7.
        /// </summary>
        public string DigitoVerificador3 => _numCmc7[2].Last().ToString();
    }

    /// <summary>
    /// Item de exportação de cheque.
    /// </summary>
    public class Item
    {
        private Data.Model.Cheques _cheque;
        private Cmc7 _cmc7;

        /// <summary>
        /// Inicia uma nova instância da classe <see cref="Item"/>.
        /// </summary>
        /// <param name="cheque">Model do cheque</param>
        public Item(Data.Model.Cheques cheque)
        {
            _cheque = cheque;
            _cmc7 = new Cmc7(cheque.Cmc7);
        }

        /// <summary>
        /// Obtém o Id da loja associada ao cheque.
        /// Apenas para recuperação de loja do arquivo.
        /// </summary>
        public uint IdLoja => _cheque.IdLoja;

        /// <summary>
        /// Obtém o Id da liberação associado ao cheque.
        /// Apenas para Geração das informações da fatura.
        /// </summary>
        public uint? IdLiberarPedido => _cheque.IdLiberarPedido;

        /// <summary>
        /// Obtém o Id do pedido associado ao cheque.
        /// Apenas para Geração das informações da fatura.
        /// </summary>
        public uint? IdPedido => _cheque.IdPedido;

        /// <summary>
        /// Obtém o Id do acerto associado ao cheque.
        /// Apenas para Geração das informações da fatura.
        /// </summary>
        public uint? IdAcerto => _cheque.IdAcerto;

        /// <summary>
        /// Obtém o banco do cheque.
        /// posição inicial 1.
        /// posição final 3.
        /// Tamanho 3.
        /// </summary>
        public string Banco => _cmc7.Banco;

        /// <summary>
        /// Obtém a agência do cheque.
        /// posição inicial 4.
        /// posição final 8.
        /// Tamanho 5.
        /// </summary>
        public string Agencia => _cheque.Agencia;

        /// <summary>
        /// Obtém a Conta do cheque.
        /// posição inicial 9.
        /// posição final 18.
        /// Tamanho 10.
        /// </summary>
        public string Conta => _cheque.Conta;

        /// <summary>
        /// Obtém o número do cheque.
        /// posição inicial 19.
        /// posição final 24.
        /// Tamanho 6.
        /// </summary>
        public int Num => _cheque.Num;

        /// <summary>
        /// Obtém o valor do cheque.
        /// posição inicial 25.
        /// posição final 37.
        /// Tamanho 13 (11+2).
        /// </summary>
        public decimal Valor => _cheque.Valor;

        /// <summary>
        /// Obtém a data de cadastro do cheque.
        /// posição inicial 38.
        /// posição final 47.
        /// Tamanho 10.
        /// </summary>
        public DateTime DataCad => _cheque.DataCad;

        /// <summary>
        /// Obtém a data de cadastro do cheque.
        /// posição inicial 48.
        /// posição final 61.
        /// Tamanho 14.
        /// </summary>
        public string CpfCnpj => _cheque.CpfCnpj;

        /// <summary>
        /// Obtém a UF do cheque.
        /// posição inicial 62.
        /// posição final 63.
        /// Tamanho 2.
        /// </summary>
        public string UF
        {
            get
            {
                var cidadeUf = Data.DAL.ClienteDAO.Instance.ObtemCidadeUf(_cheque.IdCliente.GetValueOrDefault());

                if (string.IsNullOrEmpty(cidadeUf))
                    return string.Empty;

                return cidadeUf.Split('/')[1];
            }
        }

        /// <summary>
        /// Obtém a data de vencimento do cheque.
        /// posição inicial 64.
        /// posição final 73.
        /// Tamanho 10.
        /// </summary>
        public DateTime? DataVenc => _cheque.DataVenc;

        /// <summary>
        /// Obtém a data de vencimento do cheque.
        /// posição inicial 74.
        /// posição final 83.
        /// Tamanho 10.
        /// </summary>
        public DateTime? DataVencUtil => _cheque.DataVenc;

        /// <summary>
        /// Obtém a data de vencimento do cheque.
        /// posição inicial 84.
        /// posição final 93.
        /// Tamanho 10.
        /// </summary>
        public DateTime? DataVencOriginal => _cheque.DataVenc;

        /// <summary>
        /// Obtém a data de vencimento do cheque.
        /// posição inicial 94.
        /// posição final 103.
        /// Tamanho 10.
        /// </summary>
        public DateTime? DataVencUtilOriginal => _cheque.DataVenc;

        /// <summary>
        /// Obtém o código de compensação do cheque.
        /// posição inicial 104.
        /// posição final 106.
        /// Tamanho 3.
        /// </summary>
        public string CodCompensacao => _cmc7.CodCompensacao;

        /// <summary>
        /// Obtém Digito verificador 2 do cheque.
        /// posição inicial 107.
        /// posição final 107.
        /// Tamanho 1.
        /// </summary>
        public string DigitoVerificador2 => _cmc7.DigitoVerificador2;

        /// <summary>
        /// Obtém a tipificação do cheque.
        /// posição inicial 108.
        /// posição final 108.
        /// Tamanho 1.
        /// </summary>
        public string Tipificacao => _cmc7.Tipificacao;

        /// <summary>
        /// Obtém o dígito verificador 1 do cheque.
        /// posição inicial 109.
        /// posição final 109.
        /// Tamanho 1.
        /// </summary>
        public string DigitoVerificador1 => _cmc7.DigitoVerificador1;

        /// <summary>
        /// Obtém o dígito verificador 3 do cheque.
        /// posição inicial 110.
        /// posição final 110.
        /// Tamanho 1.
        /// </summary>
        public string DigitoVerificador3 => _cmc7.DigitoVerificador3;

        /// <summary>
        /// Obtém Praça de compensação do cheque.
        /// posição inicial 111.
        /// posição final 113.
        /// Tamanho 3.
        /// </summary>
        public string PracaCompensacao => "006";

        /// <summary>
        /// Obtém o Emitente do cheque.
        /// posição inicial 147.
        /// posição final 176.
        /// Tamanho 30.
        /// </summary>
        public string Emitente => Glass.Data.DAL.ClienteDAO.Instance.GetNome(_cheque.IdCliente.GetValueOrDefault());

        /// <summary>
        /// Obtém ou define a fatura do cheque.
        /// posição inicial 189.
        /// posição final 202.
        /// Tamanho 14.
        /// </summary>
        public uint Fatura { get; set; }

        /// <summary>
        /// Obtém ou define o valor da fatura do cheque.
        /// posição inicial 203.
        /// posição final 217.
        /// Tamanho 14.
        /// </summary>
        public decimal ValorFatura { get; set; }

        /// <summary>
        /// Obtém ou define a chave de acesso da fatura do cheque.
        /// posição inicial 218.
        /// posição final 261.
        /// Tamanho 44.
        /// </summary>
        public string ChaveDanfe { get; set; }

        /// <summary>
        /// Obtém ou define a série da fatura do cheque.
        /// posição inicial 262.
        /// posição final 264.
        /// Tamanho 3.
        /// </summary>
        public string SerieFatura { get; set; }

        #region Métodos Públicos

        /// <summary>
        /// Serializa os dados do cheque.
        /// </summary>
        /// <param name="writer">TextWriter.</param>
        public void Serializar(System.IO.TextWriter writer)
        {
            var retorno = new StringBuilder();
            retorno.Append(Banco);
            retorno.Append(Agencia.PadLeft(5, '0'));
            retorno.Append(Conta.Replace("-", string.Empty).PadLeft(10, '0'));
            retorno.Append(Num.ToString().PadLeft(6, '0'));
            retorno.Append(Valor.ToString().Replace(".", string.Empty).Replace(",", string.Empty).PadLeft(13, '0'));
            retorno.Append(DataCad.ToShortDateString());
            retorno.Append(CpfCnpj.LimpaCpfCnpj().PadLeft(14, '0'));
            retorno.Append(UF);
            retorno.Append(DataVenc.GetValueOrDefault().ToShortDateString());
            retorno.Append(DataVencUtil.GetValueOrDefault().ToShortDateString());
            retorno.Append(DataVencOriginal.GetValueOrDefault().ToShortDateString());
            retorno.Append(DataVencUtilOriginal.GetValueOrDefault().ToShortDateString());
            retorno.Append(CodCompensacao);
            retorno.Append(DigitoVerificador2);
            retorno.Append(Tipificacao);
            retorno.Append(DigitoVerificador1);
            retorno.Append(DigitoVerificador3);
            retorno.Append(PracaCompensacao);
            retorno.Append(string.Empty.PadLeft(33, ' '));

            if (Emitente.Count() > 30)
                retorno.Append(Emitente.Substring(0, 30));

            retorno.Append(Emitente.PadLeft(30, ' '));

            retorno.Append(string.Empty.PadLeft(12, ' '));
            retorno.Append(Fatura.ToString().PadLeft(14, '0'));
            retorno.Append(ValorFatura.ToString().Replace(".", string.Empty).Replace(",", string.Empty).PadLeft(15, '0'));
            retorno.Append(ChaveDanfe);
            retorno.Append(SerieFatura);

            writer.WriteLine(retorno);
        }

        #endregion

    }
}
