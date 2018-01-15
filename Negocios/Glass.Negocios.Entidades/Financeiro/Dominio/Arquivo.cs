using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Financeiro.Negocios.Entidades.Dominio
{
    public class Arquivo
    {
        #region Variaveis Locais

        private List<R6000> _itens;

        #endregion

        #region Propiedades

        /// <summary>
        /// Itens do arquivo
        /// </summary>
        public List<R6000> Itens
        {
            get
            {
                if (_itens == null)
                    _itens = new List<R6000>();

                return _itens;
            }
            set
            {
                _itens = value;
            }
        }

        #endregion

        #region Metodos Publicos

        /// <summary>
        /// Salva o arquivo Gcon
        /// </summary>
        /// <param name="stream"></param>
        public void Salvar(System.IO.Stream stream)
        {
            var writer = new System.IO.StreamWriter(stream);

            var r100 = new R000(Itens[0].Cnpj);

            r100.Serializar(writer);

            for (int i = 0; i < Itens.Count; i++)
                Itens[i].Serializar(writer);

            writer.Flush();
        }

        #endregion
    }
}
