using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using Aspose.Cells;
using CsvHelper;
using CsvHelper.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace practiquesIEI.Wrappers
{
    public class CsvWrapper
    {
        public static string ConvertCsvToJson(string csvFileName)
        {
            try {
                string dataFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fuentes de datos");
                string csvFilePath = Path.Combine(dataFolderPath, csvFileName);
                string csvContent = File.ReadAllText(csvFilePath);
                string[] csvLines = csvContent.Split('\n');

                List<Dictionary<string, string>> jsonList = new List<Dictionary<string, string>>();

                // Suponiendo que la primera línea del archivo CSV contiene los encabezados
                string[] headers = csvLines[0].Split(';');

                for (int i = 1; i < csvLines.Length; i++)
                {
                    string[] values = csvLines[i].Split(';');
                    Dictionary<string, string> jsonEntry = new Dictionary<string, string>();

                    for (int j = 0; j < headers.Length && j < values.Length; j++)
                    {
                        jsonEntry[headers[j]] = values[j];
                    }

                    jsonList.Add(jsonEntry);
                }
                return JsonConvert.SerializeObject(jsonList, Formatting.Indented);
            }
            
            catch(Exception ex){
                Console.WriteLine($"Error al convertir CSV a JSON: {ex.Message}");
                return null;
            }
        }
    }
    
}
    



