using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Glass.Financeiro.Negocios.Entidades.Cheques
{
    public class Arquivo
    {
        #region Variaveis Locais

        private List<Item> _itens;

        #endregion

        #region Construtores

        public Arquivo(List<Item> itens)
        {
            _itens = itens;
        }

        #endregion

        #region Metodos Publicos

        /// <summary>
        /// Salva o arquivo
        /// </summary>
        public void Salvar(System.IO.Stream stream)
        {
            var writer = new System.IO.StreamWriter(stream);

            var nomeLoja = Glass.Data.DAL.LojaDAO.Instance.GetNome(_itens.First().IdLoja);

            writer.WriteLine(nomeLoja);

            for (int i = 0; i < _itens.Count; i++)
            {
                _itens[i].Serializar(writer);
            }

            writer.Flush();
        }

        #endregion
    }
}
