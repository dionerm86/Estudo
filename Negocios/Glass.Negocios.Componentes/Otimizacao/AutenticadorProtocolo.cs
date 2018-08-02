using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Glass.Otimizacao.Negocios.Componentes
{
    /// <summary>
    /// Representa o autenticador do protocolo.
    /// </summary>
    public class AutenticadorProtocolo : eCutter.IAutenticadorProtocolo
    {
        #region Métodos

        /// <summary>
        /// Realiza a autenticação.
        /// </summary>
        /// <param name="usuario"></param>
        /// <param name="senha"></param>
        /// <returns></returns>
        public eCutter.AutenticacaoProtocolo Autenticar(string usuario, string senha)
        {
            try
            {
                var resultado = Data.DAL.FuncionarioDAO.Instance.Autenticacao(usuario, senha);

                return new eCutter.AutenticacaoProtocolo
                {
                    Sucesso = resultado != null,
                    Usuario = resultado?.CodUser.ToString()
                };
            }
            catch (Exception ex)
            {
                return new eCutter.AutenticacaoProtocolo
                {
                    Sucesso = false,
                    Mensagem = ex.Message
                };
            }
        }

        #endregion
    }
}
