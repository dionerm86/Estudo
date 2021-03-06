﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Glass.Financeiro.Negocios.Entidades;
using NPOI.SS.UserModel;
using Colosoft;
using Glass.Data.DAL;

namespace Glass.Financeiro.Negocios.Componentes.LayoutCNI
{
    public class LayoutRede : IArquivoCNI
    {
        #region Variaveis Locais

        private ISheet _sheet;
        private string _bandeira = "";
        private string _tipoVenda = "";

        private Glass.Data.Model.TipoCartaoCredito _tipoCartao;
        private uint? _idContaBanco;

        private int _index, _nsuIndex, _nunCardIndex, _dtVendaIndex, _valorIndex, _numParcIndex, _numEstabIndex;

        #endregion

        #region Construtores

        public LayoutRede(ISheet sheet)
        {
            _sheet = sheet;
        }

        #endregion

        public List<CartaoNaoIdentificado> Importar(int idArqCni)
        {
            var cnis = new List<CartaoNaoIdentificado>();

            CarregarDados();

            for (int i = _index + 1; i <= _sheet.LastRowNum; i++)
            {
                var row = _sheet.GetRow(i);

                if (row == null)
                    continue;

                var cni = SourceContext.Instance.Create<CartaoNaoIdentificado>();

                cni.NumAutCartao = row.GetCell(_nsuIndex).ToString();
                cni.UltimosDigitosCartao = row.GetCell(_nunCardIndex).ToString();
                cni.DataVenda = row.GetCell(_dtVendaIndex).ToString().StrParaDate().Value;
                cni.Valor = row.GetCell(_valorIndex).ToString().StrParaDecimal();

                if (row.GetCell(_numParcIndex) != null)
                    cni.NumeroParcelas = row.GetCell(_numParcIndex).ToString().StrParaInt();

                cni.NumeroEstabelecimento = row.GetCell(_numEstabIndex).ToString();
                cni.TipoCartao = (int)_tipoCartao.IdTipoCartao;
                cni.IdContaBanco = (int)_idContaBanco.GetValueOrDefault(0);
                cni.Importado = true;
                cni.IdArquivoCartaoNaoIdentificado = idArqCni;

                cnis.Add(cni);
            }

            return cnis;
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
                    case "bandeira":
                        {
                            _bandeira = cellValue;
                            break;
                        }
                    case "tipo de venda":
                        {
                            _tipoVenda = cellValue;
                            break;
                        }
                    case "tid":
                        {
                            _index = i;
                            for (int j = 0; j <= row.LastCellNum; j++)
                            {
                                cellCaption = row.GetCell(j) == null ? "" : row.GetCell(j).ToString().ToLower();

                                switch (cellCaption)
                                {
                                    case "nº do comprovante de venda (nsu)":
                                    case "nº do comprovantede venda (nsu)":
                                        {
                                            _nsuIndex = j;
                                            break;
                                        }
                                    case "nº cartão(últimos 4 dig.)":
                                        {
                                            _nunCardIndex = j;
                                            break;
                                        }
                                    case "data da venda":
                                        {
                                            _dtVendaIndex = j;
                                            break;
                                        }
                                    case "valor bruto":
                                        {
                                            _valorIndex = j;
                                            break;
                                        }
                                    case "qtde de parcelas":
                                        {
                                            _numParcIndex = j;
                                            break;
                                        }
                                    case "nº estabelec.":
                                        {
                                            _numEstabIndex = j;
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

            _tipoCartao = TipoCartaoCreditoDAO.Instance.ObterTipoCartao(Data.Model.OperadoraCartaoEnum.Rede, _bandeira, _tipoVenda);
            _idContaBanco = TipoCartaoCreditoDAO.Instance.GetContaBanco(_tipoCartao.IdTipoCartao);
        }

        public bool LayoutValido(ISheet sheet)
        {
            for (int i = 0; i <= sheet.LastRowNum; i++)
            {
                var row = sheet.GetRow(i);

                if (row == null)
                    continue;

                var cellValue = row.GetCell(0) == null ? "" : row.GetCell(0).ToString().ToLower();

                if (cellValue == "bandeira")
                    return true;
            }

            return false;
        }
    }
}
