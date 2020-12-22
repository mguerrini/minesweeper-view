

namespace Minesweeper.Core.Xml.Exceptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable]
    public class XmlDataSerializerException : Exception
    {
        public XmlDataSerializerException(){}
        
        public XmlDataSerializerException(string message) : base(message){}

        public XmlDataSerializerException(string message, Exception inner) : base(message, inner) { }

        protected XmlDataSerializerException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        #region -- override GetObjectData --

        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        #endregion    
    }
}
