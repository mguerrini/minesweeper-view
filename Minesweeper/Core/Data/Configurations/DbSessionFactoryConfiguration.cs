namespace Minessweeper.CoreCore.Data.DbSessions.Configurations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class DbSessionFactoryConfiguration
    {
        [XmlAttribute("defaultSessionName")]
        public string DefaultSessionName { get; set; }
    }
}
