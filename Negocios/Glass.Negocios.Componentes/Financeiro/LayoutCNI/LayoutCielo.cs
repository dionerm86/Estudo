using System;
using System.Collections.Generic;
using Glass.Financeiro.Negocios.Entidades;
using NPOI.SS.UserModel;
using Colosoft;
using Glass.Data.DAL;

namespace Glass.Financeiro.Negocios.Componentes.LayoutCNI
{
    public class LayoutCielo : IArquivoCNI
    {
        #region Variaveis Locais

        private ISheet _sheet;

        private int _index, _nunCardIndex, _dtVendaIndex, _valorIndex, _numParcIndex, _numEstabIndex;
        private int? _nsuIndex, _bandeiraIndex;

        #endregion

        #region Construtores

        public LayoutCielo(ISheet sheet)
        {
            _sheet = sheet;
        }

        #endregion

        public List<CartaoNaoIdentificado> Importar(int idArqCni)
        {
            var idOperadoraCartaoCielo = OperadoraCartaoDAO.Instance.ObterIdOperadoraPelaDescricao("Cielo");

            if (idOperadoraCartaoCielo == 0)
                throw new System.Exception("Não existe uma operadora de cartão cadastrada com a descrição Cielo.");

            var cnis = new List<CartaoNaoIdentificado>();

            CarregarDados();

            if (_nsuIndex.GetValueOrDefault(0) == 0)
                throw new Exception("A planilha informada não possui a coluna de identificação da transação (NSU / DOC)");

            if (_bandeiraIndex.GetValueOrDefault(0) == 0)
                throw new Exception("A planilha informada não possui a coluna de bandeira");

            for (int i = _index + 1; i <= _sheet.LastRowNum; i++)
            {
                var row = _sheet.GetRow(i);

                if (row == null)
                    continue;

                var bandeira = row.GetCell(_bandeiraIndex.Value).ToString();
                var descrNumPar = row.GetCell(_numParcIndex).ToString();
                var tipoVenda = "";
                var nsu = row.GetCell(_nsuIndex.Value).ToString();
                var numParc = "";

                var idxParc = descrNumPar.LastIndexOf('/');
                if (idxParc > -1)
                    numParc = descrNumPar.Substring(idxParc + 1, 1);

                if (descrNumPar.ToLower().Contains("débito"))
                    tipoVenda = "débito";

                var tipoCartao = TipoCartaoCreditoDAO.Instance.ObterTipoCartao(idOperadoraCartaoCielo, bandeira, tipoVenda);
                var idContaBanco = TipoCartaoCreditoDAO.Instance.GetContaBanco(tipoCartao.IdTipoCartao);

                if (string.IsNullOrEmpty(nsu))
                    throw new Exception("Não foi possivel importar. NSU não identificado no registro atual.");

                var cni = SourceContext.Instance.Create<CartaoNaoIdentificado>();

                cni.NumAutCartao = row.GetCell(_nsuIndex.Value).ToString();

                var ultDigCard = row.GetCell(_nunCardIndex).ToString();

                if (ultDigCard.Length > 4)
                    ultDigCard = ultDigCard.Substring(ultDigCard.Length - 4, 4);

                cni.UltimosDigitosCartao = ultDigCard;
                cni.DataVenda = row.GetCell(_dtVendaIndex).ToString().StrParaDate().Value;
                cni.Valor = row.GetCell(_valorIndex).ToString().StrParaDecimal();

                if (numParc != null)
                    cni.NumeroParcelas = numParc.StrParaInt();

                cni.NumeroEstabelecimento = row.GetCell(_numEstabIndex).ToString();
                cni.TipoCartao = (int)tipoCartao.IdTipoCartao;
                cni.IdContaBanco = (int)idContaBanco.GetValueOrDefault(0);
                cni.Importado = true;
                cni.IdArquivoCartaoNaoIdentificado = idArqCni;

                cnis.Add(cni);
            }

            return cnis;
        }

        public bool LayoutValido(ISheet sheet)
        {
            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);

                if (row == null)
                    continue;

                var cellValue = row.GetCell(0) == null ? "" : row.GetCell(0).ToString().ToLower();

                if (cellValue == "total" || cellValue.Contains("período"))
                    return true;
            }

            return false;
        }

        private void CarregarDados()
        {
            for (int i = 0; i <= _sheet.LastRowNum; i++)
            {
                var row = _sheet.GetRow(i);

                if (row == null)
                    continue;

                var cellCaption = row.GetCell(0) == null ? "" : row.GetCell(0).ToString().ToLower();
                var cellValue = row.GetCell(1) == null ? "" : row.GetCell(1).ToString().ToLower();

                switch (cellCaption)
                {
                    case "data da venda":
                    case "data prevista de pagamento":
                    case "data de Pagamento":
                        {
                            _index = i;
                            for (int j = 0; j <= row.LastCellNum; j++)
                            {
                                cellCaption = row.GetCell(j) == null ? "" : row.GetCell(j).ToString().ToLower();

                                switch (cellCaption)
                                {
                                    case "data da venda":
                                        {
                                            _dtVendaIndex = j;
                                            break;
                                        }
                                    case "estabelecimento":
                                        {
                                            _numEstabIndex = j;
                                            break;
                                        }
                                    case "bandeira":
                                        {
                                            _bandeiraIndex = j;
                                            break;
                                        }
                                    case "n° do cartão / tid":
                                        {
                                            _nunCardIndex = j;
                                            break;
                                        }

                                    case "nsu / doc":
                                        {
                                            _nsuIndex = j;
                                            break;
                                        }
                                    case "valor bruto de vendas (r$)":
                                        {
                                            _valorIndex = j;
                                            break;
                                        }
                                    case "descrição":
                                        {
                                            _numParcIndex = j;
                                            break;
                                        }
                                   

                                    default:
                                        break;
                                }
                            }
                            break;
                        }

                    default:
                        break;
                }
            }
        }
    }
}
