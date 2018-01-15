using System.Collections.Generic;

namespace Glass.Financeiro.Negocios.Entidades.GCon
{
    public class Arquivo
    {
        #region Variaveis Locais

        private List<Item> _itens;

        #endregion

        #region Propiedades

        /// <summary>
        /// Itens do arquivo
        /// </summary>
        public List<Item> Itens
        {
            get
            {
                if (_itens == null)
                    _itens = new List<Item>();

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

            for (int i = 0; i < Itens.Count; i++)
            {
                Itens[i].Serializar(writer, i + 1);
            }

            writer.Flush();
        }

        #endregion
    }
}
