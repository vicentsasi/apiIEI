using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using CsvHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace practiquesIEI.Wrappers
{
    public class XmlWrapper
    {

        public static string ConvertXmlToJson(string xmlFileName)
        {
            try
            {
                //string dataFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Fuentes de datos");
                //string xmlFilePath = Path.Combine(dataFolderPath, xmlFileName);
                // Cargar el contenido del archivo XML
                string xmlContent = File.ReadAllText(xmlFileName);

                // Crear un documento XML
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(xmlContent);

                // Convertir el documento XML a JSON
                string jsonText = JsonConvert.SerializeXmlNode(xmlDoc);

                return jsonText;
            }
            catch (Exception ex)
            {
                // Manejar cualquier excepción que pueda ocurrir durante el proceso de conversión
                Console.WriteLine($"Error al convertir XML a JSON: {ex.Message}");
                return null;
            }
        }


    }
}

