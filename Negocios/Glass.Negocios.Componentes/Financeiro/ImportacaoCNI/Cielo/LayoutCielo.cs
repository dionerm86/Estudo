using System.Collections.Generic;
using Glass.Financeiro.Negocios.Entidades;
using Colosoft;
using System.IO;
using Glass.Data.DAL;
using System.Text.RegularExpressions;

namespace Glass.Financeiro.Negocios.Componentes.LayoutCNI.Cielo
{
    public class LayoutCielo : IArquivoCNI
    {
        #region Variaveis Locais

        private Stream _stream;
        private string _extensao;
        private string[] _arquivo;

        #endregion

        #region Construtores

        public LayoutCielo(Stream stream, string extensao)
        {
            _stream = stream;
            _extensao = extensao;
        }

        #endregion

        public List<CartaoNaoIdentificado> Importar(int idArqCni)
        {
            var idOperadoraCartaoCielo = OperadoraCartaoDAO.Instance.ObterIdOperadoraPelaDescricao("Cielo");

            if (idOperadoraCartaoCielo == 0)
                throw new System.Exception("Não existe uma operadora de cartão cadastrada com a descrição Cielo.");

            var lstCni = new List<CartaoNaoIdentificado>();

            for (int i = 0; i < _arquivo.Length; i++)
            {
                var reg1 = _arquivo[i];

                if (string.IsNullOrWhiteSpace(reg1) || reg1[0] != '1' || (reg1.Substring(18, 2).Trim() != "" && reg1.Substring(18, 2) != "01"))
                    continue;

                var regs2 = new List<string>();
                var indexReg2 = i + 1;
                while (_arquivo[indexReg2][0] == '2')
                {
                    regs2.Add(_arquivo[indexReg2]);
                    indexReg2++;
                }

                for (int j = 0; j < regs2.Count; j++)
                {
                    var reg2 = regs2[j];
                    var cni = SourceContext.Instance.Create<CartaoNaoIdentificado>();
                    var ultimosDigitosCartao = 0;
                    
                    /* Chamado 66288. */
                    if (!System.Int32.TryParse(reg2.Substring((reg2.LastIndexOf('*') + 1), 4), out ultimosDigitosCartao))
                        throw new System.Exception(string.Format("Não foi possível recuperar os últimos dígitos de um dos cartões do arquivo de importação. Valor recuperado na linha {0}, entre as colunas 31 e 34: {1}.",
                            // Esta lógica recupera a linha exata, do arquivo, que está sendo lida no momento.
                            // "indexReg2" possui a última linha do grupo de registros iniciados com "2", no arquivo.
                            // "j" representa a linha atual, dentro do grupo de registros iniciados com "2".
                            // "regs2.Count" representa a quantidade de linhas dentro do grupo de registros iniciados com "2".
                            // Ex.: indexReg2 = 10; regs2.Count = 6; j = 4; (10 - (6 - 4) + 1) == 9; (estamos na linha 9 do arquivo).
                            (indexReg2 - (regs2.Count - j) + 1), ultimosDigitosCartao));

                    cni.NumAutCartao = reg2.Substring(92, 6);
                    cni.UltimosDigitosCartao = ultimosDigitosCartao.ToString();
                    cni.DataVenda = new System.DateTime(reg2.Substring(37, 4).StrParaInt(), reg2.Substring(41, 2).StrParaInt(), reg2.Substring(43, 2).StrParaInt());

                    var numParcela = reg2.Substring(61, 2).StrParaInt();
                    cni.NumeroParcelas = numParcela == 0 ? 1 : numParcela;

                    //Se for venda a vista o campo para pegar o valor é diferente
                    var valorIndex = numParcela == 0 ? 46 : 113;
                    cni.Valor = reg2.Substring(valorIndex, 13).StrParaDecimal() / 100m;

                    cni.NumeroEstabelecimento = reg2.Substring(1, 10);

                    var bandeira = reg1.Substring(184, 3).StrParaInt();
                    var codProduto = reg1.Substring(232, 3).StrParaInt();
                    var tipoVenda = ObterTipoVenda(codProduto);

                    var tipoCartao = TipoCartaoCreditoDAO.Instance.ObterTipoCartao(idOperadoraCartaoCielo, bandeira, tipoVenda);
                    var idContaBanco = TipoCartaoCreditoDAO.Instance.GetContaBanco((uint)tipoCartao.IdTipoCartao);

                    cni.TipoCartao = (int)tipoCartao.IdTipoCartao;
                    cni.IdContaBanco = (int)idContaBanco.GetValueOrDefault(0);
                    cni.Importado = true;
                    cni.IdArquivoCartaoNaoIdentificado = idArqCni;

                    lstCni.Add(cni);
                }
            }

            return lstCni;
        }

        public bool LayoutValido()
        {
            _stream.Position = 0;

            using (var reader = new StreamReader(_stream))
            {
                var arquivo = reader.ReadToEnd();
                _arquivo = Regex.Split(arquivo, "\r\n");

                return _arquivo[0].Substring(48, 2).ToLower().Equals("3p");
            }
        }

        private Data.Model.TipoCartaoEnum ObterTipoVenda(int codProduto)
        {
            //Manual Cielo - Tabela IV  Código do Produto 
            var vendaDebito = new List<int>(){ 11, 14, 17, 18, 22, 23, 25, 36, 41, 71, 94, 97 };

            if (vendaDebito.Contains(codProduto))
                return Data.Model.TipoCartaoEnum.Debito;
            else
                return Data.Model.TipoCartaoEnum.Credito;
        }
    }
}
