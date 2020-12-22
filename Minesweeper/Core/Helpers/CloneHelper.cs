namespace Minesweeper.Core.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Runtime.Serialization;

    public class CloneHelper
    {
        public static object BinaryFormatterDeepClone(object original)
        {
            if (original == null)
                return null;

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Context = new StreamingContext(StreamingContextStates.Clone);
                formatter.Serialize(stream, original);
                stream.Position = 0;
                return formatter.Deserialize(stream);
            }
        }

        public static T BinaryFormatterDeepClone<T>(T original)
        {
            if (original == null)
                return default(T);

            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Context = new StreamingContext(StreamingContextStates.Clone);
                formatter.Serialize(stream, original);
                stream.Position = 0;
                return (T) formatter.Deserialize(stream);
            }
        }
    }
}
