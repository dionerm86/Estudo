using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace Glass.Api.Host.Controllers
{
    /// <summary>
    /// Home.
    /// </summary>
    public class IndexController : ApiController
    {
        /// <summary>
        /// Index.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Get()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            return Ok(new
            {
                ProductName = fvi.ProductName,
                ProductVersion = fvi.ProductVersion
            });
        }
    }
}