using System;
using System.Collections.Generic;
using System.Text;
using Glass.Data.Model;

namespace Glass.Data.SIntegra
{
    internal class Registro90 : Registro
    {
        #region Campos Privados

        private Loja _loja;
        private Registro[] _registros;
        Dictionary<int, int> totais;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="loja">O objeto que contém os dados da loja.</param>
        /// <param name="registros">Os registros que serão usados para gerar o registro 90.</param>
        public Registro90(Loja loja, params Registro[] registros)
        {
            _loja = loja;
            _registros = registros;
            totais = new Dictionary<int, int>();

            foreach (Registro r in _registros)
            {
                if (!totais.ContainsKey(r.Tipo))
                    totais.Add(r.Tipo, 1);
                else
                    totais[r.Tipo]++;
            }
        }

        #endregion

        #region Propriedades

        public override int Tipo
        {
            get { return 90; }
        }

        public string CNPJ
        {
            get { return NotNullString(_loja.Cnpj).Replace(".", "").Replace("-", "").Replace("/", ""); }
        }

        public string InscEstadual
        {
            get { return NotNullString(_loja.InscEst); }
        }

        public Dictionary<int, int> TotaisTipoRegistro
        {
            get { return totais; }
        }

        public int TotalGeral
        {
            get
            {
                int totalRegistros = 0;
                foreach (int tipo in TotaisTipoRegistro.Keys)
                    totalRegistros += TotaisTipoRegistro[tipo];

                return 2 + totalRegistros + TotalRegistros90;
            }
        }

        public int TotalRegistros90
        {
            get { return (int)Math.Ceiling((double)(TotaisTipoRegistro.Keys.Count + 1) / 8); }
        }

        #endregion

        #region Propriedades de Suporte

        /// <summary>
        /// Retorna o texto do registro do tipo 90 para uso do SIntegra.
        /// </summary>
        /// <returns>Uma string com os dados formatados para uso do SIntegra.</returns>
        public override string ToString()
        {
            // Formata os dados para gerar a string do cabeçalho da resposta
            string n01 = Tipo.ToString().PadLeft(2, '0');
            string n02 = CNPJ.PadLeft(14, '0');
            string n03 = InscEstadual.PadRight(14);
            string[] n04 = new string[TotaisTipoRegistro.Keys.Count + 1];
            string n05 = TotalRegistros90.ToString();

            // Coloca os totais de registros na variável n04
            int j = 0;
            foreach (int tipo in TotaisTipoRegistro.Keys)
                n04[j++] = tipo.ToString().PadLeft(2, '0') + TotaisTipoRegistro[tipo].ToString().PadLeft(8, '0');
            n04[j] = "99" + TotalGeral.ToString().PadLeft(8, '0');

            // Formata as linhas dos registros 90
            string[] linhas = new string[TotalRegistros90];
            for (int i = 0; i < linhas.Length; i++)
            {
                string n04Linha = "";
                for (j = i * 8; j < (i + 1) * 8; j++)
                {
                    if (j >= n04.Length)
                        break;

                    n04Linha += n04[j];
                }

                linhas[i] = n01 + n02 + n03 + n04Linha.PadRight(95) + n05;
            }

            // Retorna os dados formatados
            StringBuilder retorno = new StringBuilder();
            foreach (string s in linhas)
                retorno.AppendLine(s);
            return retorno.ToString();
        }

        #endregion
    }
}