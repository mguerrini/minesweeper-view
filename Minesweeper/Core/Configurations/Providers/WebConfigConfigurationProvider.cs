namespace Minesweeper.Core.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.ServiceModel;

    /// <summary>
    /// 
    /// </summary>
    public class WebConfigConfigurationProvider : AppConfigConfigurationProvider
    {
        #region -- Constructors --

        /// <summary>
        /// 
        /// </summary>
        public WebConfigConfigurationProvider()
        {
        }

        #endregion


        #region -- IConfigurationProvider Members --

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        public override void Load(string fileName)
        {
            //no hace nada....porque es web
        }

        /// <summary>
        /// 
        /// </summary>
        public override string ApplicationPath
        {
            get
            {
                if (HttpContext.Current != null)
                    return HttpContext.Current.Server.MapPath("~");
                else
                    return base.ApplicationPath;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override System.Configuration.Configuration DoGetConfigurationManager()
        {
            return System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
        }

        #endregion
    }
}
