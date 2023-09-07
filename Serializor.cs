using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace HistoricalTimeLineCreator
{
    /// <summary>
    /// Class for serializing history as binary
    /// NOTE! THE BINARY SERIALIZOR IS OBSOLETE
    /// 
    /// Author:
    /// Tobias Lenander
    /// </summary>
    public static class Serializor
    {
        public static void SaveHistoryAsBinary(HistorySerializable history, string fileName)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Create))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                #pragma warning disable SYSLIB0011 // Type or member is obsolete
                binaryFormatter.Serialize(fileStream, history);
                #pragma warning restore SYSLIB0011 // Type or member is obsolete
            }
        }

        public static HistorySerializable LoadHistoryFromBinary(string fileName)
        {
            using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();

                #pragma warning disable SYSLIB0011 // Type or member is obsolete
                var fileData = binaryFormatter.Deserialize(fileStream);
                #pragma warning restore SYSLIB0011 // Type or member is obsolete

                return (HistorySerializable)fileData;
            }
        }
    }
}
