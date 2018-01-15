using System;

namespace Glass.UI.Web.Process
{
    /// <summary>
    /// Classe que serve como acesso ao ServiceLocator.
    /// </summary>
    public static class ServiceLocatorAccessor
    {
        /// <summary>
        /// Recupera a implementação para o tipo informado.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="contractName"></param>
        /// <returns></returns>
        public static object GetInstance(Type type, string contractName)
        {
            // Verifica se está sendo solicidade uma DAO.
            if (!type.IsAbstract && !type.IsInterface)
            {
                // Carrega a propriedade usada para recupera a instancia da DAO
                var instanceProperty = type.GetProperty("Instance",
                    System.Reflection.BindingFlags.FlattenHierarchy | 
                    System.Reflection.BindingFlags.Public | 
                    System.Reflection.BindingFlags.Static);

                if (instanceProperty != null)
                    try
                    {
                        return instanceProperty.GetValue(null, null);
                    }
                    catch (System.Reflection.TargetInvocationException ex)
                    {
                        throw ex.InnerException;
                    }
            }

            return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance(type, contractName);
        }
    }
}
