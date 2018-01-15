using Glass.Financeiro.Negocios.Entidades;
using System.IO;
using System.Collections.Generic;
using Glass.Financeiro.Negocios.Componentes.LayoutCNI.Rede;
using Glass.Financeiro.Negocios.Componentes.LayoutCNI.Cielo;

namespace Glass.Financeiro.Negocios.Componentes.LayoutCNI
{
    public class Importador
    {
        #region Variaveis Locais

        private Stream _arquivo;
        private string _extensao;
        private IArquivoCNI _arquivoCni;

        #endregion

        #region Construtores

        public Importador(Stream stream, string extensao)
        {
            _arquivo = stream;
            _extensao = extensao;
        }

        #endregion

        #region Métodos Publicos

        public List<CartaoNaoIdentificado> Importar(int idArqCni)
        {
            return _arquivoCni.Importar(idArqCni);
        }

        public bool Valido(out string msgErro)
        {
            var layoutRede = new LayoutRede(_arquivo, _extensao);
            var layoutCielo = new LayoutCielo(_arquivo, _extensao);

            if (layoutRede.LayoutValido())
                _arquivoCni = layoutRede;
            else if (layoutCielo.LayoutValido())
                _arquivoCni = layoutCielo;

            if (_arquivoCni == null)
            {
                msgErro = "Nenhum layout cadastrado corresponde a planilha informada.";
                return false;
            }

            msgErro = "";
            return true;
        }

        #endregion
    }
}
