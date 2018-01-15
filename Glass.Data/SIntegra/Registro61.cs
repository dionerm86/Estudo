using System;
using Glass.Data.Model;
using Glass.Data.DAL;

namespace Glass.Data.SIntegra
{
    internal class Registro61 : Registro
    {
        #region Campos Privados

        private NotaFiscal _nf;
        private Loja _loja;
        private NotaFiscal[] _listaNotas = null;

        #endregion

        #region Construtores

        /// <summary>
        /// Construtor da classe.
        /// </summary>
        /// <param name="nf">A nota fiscal usada no registro.</param>
        public Registro61(NotaFiscal nf)
        {
            _nf = nf;
            _loja = LojaDAO.Instance.GetElement(nf.IdLoja.Value);
        }

        #endregion

        #region Propriedades

        public override int Tipo
        {
            get { return 61; }
        }

        public DateTime DataEmissao
        {
            get { return _nf.DataEmissao; }
        }

        public string Modelo
        {
            get { return _nf.Modelo; }
        }

        public string Serie
        {
            get { return _nf.Serie; }
        }

        public int Subserie
        {
            get { return 0; }
        }

        public uint NumeroInicialOrdem
        {
            get
            {
                if (_listaNotas == null || _listaNotas.Length == 0)
                    _listaNotas = RecuperaNotasMesmoDia(_nf);

                return _listaNotas.Length > 0 ? _listaNotas[0].IdNf : _nf.IdNf;
            }
        }

        public uint NumeroFinalOrdem
        {
            get
            {
                if (_listaNotas == null || _listaNotas.Length == 0)
                    _listaNotas = RecuperaNotasMesmoDia(_nf);

                return _listaNotas.Length > 0 ? _listaNotas[_listaNotas.Length - 1].IdNf : _nf.IdNf;
            }
        }

        public decimal ValorTotal
        {
            get { return _nf.TotalNota; }
        }

        public decimal BaseCalcICMS
        {
            get
            {
                if (_listaNotas == null || _listaNotas.Length == 0)
                    _listaNotas = RecuperaNotasMesmoDia(_nf);

                decimal retorno = 0;
                foreach (NotaFiscal nf in _listaNotas)
                    retorno += nf.BcIcms;

                return retorno;
            }
        }

        public decimal ValorICMS
        {
            get
            {
                if (_listaNotas == null || _listaNotas.Length == 0)
                    _listaNotas = RecuperaNotasMesmoDia(_nf);

                decimal retorno = 0;
                foreach (NotaFiscal nf in _listaNotas)
                    retorno += nf.Valoricms;

                return retorno;
            }
        }

        public float ValorIsencao
        {
            get { return 0; }
        }

        public float ValorOutras
        {
            get { return 0; }
        }

        public double AliquotaICMS
        {
            get { return (double)(_nf.Valoricms / (_nf.TotalNota > 0 ? _nf.TotalNota : 1)) * 100; }
        }

        #endregion

        #region Métodos Privados

        /// <summary>
        /// Recupera uma lista das notas fiscais geradas no mesmo dia e com o mesmo modelo e número de série.
        /// </summary>
        /// <param name="nf"></param>
        /// <returns></returns>
        private static NotaFiscal[] RecuperaNotasMesmoDia(NotaFiscal nf)
        {
            return NotaFiscalDAO.Instance.GetByDiaModeloSerie(nf.DataEmissao, nf.Modelo, nf.Serie);
        }

        #endregion

        #region Propriedades de Suporte

        /// <summary>
        /// Retorna o texto do registro do tipo 61 para uso do SIntegra.
        /// </summary>
        /// <returns>Uma string com os dados formatados para uso do SIntegra.</returns>
        public override string ToString()
        {
            // Formata os dados para o retorno do método
            string n01 = Tipo.ToString();
            string n02 = "".PadRight(14);
            string n03 = "".PadRight(14);
            string n04 = FormatData(DataEmissao);
            string n05 = Modelo.PadLeft(2, '0');
            string n06 = Serie.ToString().PadRight(3);
            string n07 = Subserie.ToString().PadRight(2);
            string n08 = NumeroInicialOrdem.ToString().PadLeft(6, '0');
            string n09 = NumeroFinalOrdem.ToString().PadLeft(6, '0');
            string n10 = ValorTotal.ToString("0##########.#0").Remove(11, 1);
            string n11 = BaseCalcICMS.ToString("0##########.#0").Remove(11, 1);
            string n12 = ValorICMS.ToString("0#########.#0").Remove(10, 1);
            string n13 = ValorIsencao.ToString("0##########.#0").Remove(11, 1);
            string n14 = ValorOutras.ToString("0##########.#0").Remove(11, 1);
            string n15 = AliquotaICMS.ToString("0#.#0").Remove(2, 1);
            string n16 = "".PadRight(1);

            // Retorna os dados formatados
            return n01 + n02 + n03 + n04 + n05 + n06 + n07 + n08 + n09 + n10 +
                n11 + n12 + n13 + n14 + n15 + n16;
        }

        #endregion
    }
}