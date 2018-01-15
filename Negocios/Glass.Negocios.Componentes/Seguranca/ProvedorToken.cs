using System;
using System.Collections.Generic;
using System.Linq;

namespace Glass.Negocios.Componentes.Seguranca
{
    /// <summary>
    /// Implementação do provedor de token.
    /// </summary>
    public class ProvedorToken : Colosoft.Security.ITokenProvider, Colosoft.Security.ITokenProviderExtension
    {
        #region Properties

        /// <summary>
        /// Tempo de vida do token.
        /// </summary>
        public static int TokenTimeout = 360;

        #endregion

        #region Events

        /// <summary>
        /// Evento acionado quando um token for inserido.
        /// </summary>
        public event Colosoft.Security.TokenInsertedHandle TokenInserted;

        #endregion

        /// <summary>
        /// Verifica se o token é válido.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Colosoft.Security.TokenConsultResult Check(string token)
        {
            return SourceContext.Instance.CreateQuery()
                .From<Data.Seguranca.Model.Token>("t")
                .LeftJoin<Data.Model.Funcionario>("t.IdFunc == f.IdFunc", "f")
                .LeftJoin<Data.Model.Cliente>("t.IdCli == c.IdCli", "c")
                .Where("Hash=?token AND Expira>?data")
                .Add("?token", token)
                .Add("?data", DateTime.Now)
                .Select("f.IdFunc, f.Nome AS NomeFunc, c.IdCli, c.Nome AS NomeCli")
                .Execute()
                .Select(f => new Colosoft.Security.TokenConsultResult
                {
                    Success = true,
                    UserId = f.IsDBNull("IdFunc") ? f["IdFunc"] : f["IdCli"],
                    UserName = f.IsDBNull("IdFunc") ? f["NomeFunc"] : f["NomeCli"]
                })
                .FirstOrDefault() ?? new Colosoft.Security.TokenConsultResult
                {
                    Success = false
                };
            throw new NotImplementedException();
        }

        /// <summary>
        /// Fecha o token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool Close(string token)
        {
            var instance = SourceContext.Instance.CreateQuery()
                .From<Data.Seguranca.Model.Token>()
                .Where("Hash=?hash")
                .Add("?hash", token)
                .Execute<Data.Seguranca.Model.Token>()
                .FirstOrDefault();

            if (instance != null)
            {
                using (var session = SourceContext.Instance.CreateSession())
                {
                    instance.Expira = DateTime.Now;
                    session.Update(instance, f => f.Expira);

                    var result = session.Execute(false);

                    if (result.Status == Colosoft.Data.ExecuteActionsResultStatus.Success)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Fecha todos os tokens do usuário.
        /// </summary>
        /// <param name="userId"></param>
        public void CloseUserTokens(int userId)
        {
            /*var tokens = SourceContext.Instance.CreateQuery()
                .From<Data.Seguranca.Model.Token>()
                .Where("UserId=?userId AND Expires > ?date")
                .Add("?userId", userId)
                .Add("?date", DateTime.Now)
                .Execute<Data.Models.Security.Token>()
                .ToList();


            using (var session = SourceContext.Instance.CreateSession())
            {
                foreach (var i in tokens)
                {
                    i.Expires = DateTime.Now;
                    session.Update(i, f => f.Expires);
                }

                session.Execute(false);
            }*/
        }

        public string Create(int size, char[] validChars)
        {
            return Guid.NewGuid().ToString();
        }

        public string Create(int size)
        {
            return Guid.NewGuid().ToString();
        }

        public string Create(char[] validChars)
        {
            return Guid.NewGuid().ToString();
        }

        public string Create()
        {
            return Guid.NewGuid().ToString();
        }

        public string GetToken(int userId)
        {
            return null;
        }

        /// <summary>
        /// Insere o token para o usuário.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool Insert(string token, int userId)
        {
            /*var subscriptions = SourceContext.Instance.CreateQuery()
                .From<Data.Models.Subscription>("s")
                .InnerJoin<Data.Models.SubscriptionUser>("s.SubscriptionId = su.SubscriptionId", "su")
                .Where("su.UserId=?userId && (s.ExpiredDate IS NULL || s.ExpiredDate > ?date)")
                .Add("?userId", userId)
                .Add("?date", DateTime.Now.Date)
                .Select("s.SubscriptionId")
                .Execute()
                .Select(f => f.GetInt32("SubscriptionId"))
                .ToArray();

            var instance = new Data.Models.Security.Token
            {
                UserId = userId,
                Hash = token,
                Expires = DateTime.Now.AddSeconds(TokenTimeout),
                CreatedDate = DateTime.Now,
                SubscriptionId = subscriptions.Length > 0 ? (int?)subscriptions.First() : null
            };

            using (var session = SourceContext.Instance.CreateSession())
            {
                session.Insert(instance);
                var result = session.Execute(false);

                if (result.Status == Colosoft.Data.ExecuteActionsResultStatus.Success)
                {
                    if (TokenInserted != null)
                        TokenInserted(this, new Colosoft.Security.TokenInsertedEventArgs(token, userId));

                    return true;
                }
            }

            return false;*/

            if (TokenInserted != null)
                TokenInserted(this, new Colosoft.Security.TokenInsertedEventArgs(token, userId));

            throw new NotImplementedException();
        }

        public void MarkMessageAsRead(IEnumerable<int> dispatcherIds)
        {
        }

        /// <summary>
        /// Nome do provedor de token.
        /// </summary>
        public string Name
        {
            get { return "Default"; }
        }

        /// <summary>
        /// Executa um ping para o token do usuário.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public Colosoft.Security.TokenPingResult Ping(string token)
        {
            // Recupera o token associado
            var instance = SourceContext.Instance.CreateQuery()
                .From<Data.Seguranca.Model.Token>()
                .Where("Hash=?hash AND Expira > ?data")
                .Add("?hash", token)
                .Add("?data", DateTime.Now)
                .Execute<Data.Seguranca.Model.Token>()
                .FirstOrDefault();

            if (instance == null)
                return new Colosoft.Security.TokenPingResult
                {
                    Status = Colosoft.Security.TokenPingResultStatus.InvalidToken
                };

            using (var session = SourceContext.Instance.CreateSession())
            {
                instance.Expira = DateTime.Now.AddSeconds(TokenTimeout);
                session.Update(instance, f => f.Expira);

                var result = session.Execute(false);

                if (result.Status != Colosoft.Data.ExecuteActionsResultStatus.Success)
                {
                    return new Colosoft.Security.TokenPingResult
                    {
                        Status = Colosoft.Security.TokenPingResultStatus.Error
                    };
                }
            }

            return new Colosoft.Security.TokenPingResult
            {
                Status = Colosoft.Security.TokenPingResultStatus.Success
            };
        }

        /// <summary>
        /// Define o perfil para o token.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="profileId"></param>
        /// <returns></returns>
        public Colosoft.Security.TokenSetProfileResult SetProfile(string token, int profileId)
        {
            return new Colosoft.Security.TokenSetProfileResult
            {
                Success = true
            };
        }
    }
}
