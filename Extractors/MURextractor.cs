using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using practiquesIEI.Entities;
using OpenQA.Selenium.Support.UI;
using System.Threading.Tasks;
using apiIEI.Entities;

namespace apiIEI.Extractors
{
    public class MURextractor
    {
        public static string eliminados;
        public static string reparados;
        public static int inserts;
        public static async Task<ExtractionResult> LoadJsonDataIntoDatabase(string jsonFilePath )
        {
            ExtractionResult result = new ExtractionResult(); // Crear una instancia de la clase ExtractionResult
            eliminados = "";
            reparados = "";
            inserts = 0;
            try
            {
                // Deserializar JSON a una lista de objetos dinámicos
                List<dynamic> dynamicDataList = JsonConvert.DeserializeObject<List<dynamic>>(jsonFilePath);
                List<centro_educativo> ListaCentros = new List<centro_educativo>();
                foreach (var dynamicData in dynamicDataList)
                {
                    centro_educativo centro = JsonACentro(dynamicData);
                    ListaCentros.Add(centro);
                    //Crear la provincia 
                    provincia provincia = new provincia();
                    if (centro != null)
                    {
                        provincia.codigo = "30";
                        provincia.nombre = "Múrcia";
                        ConexionBD.ConexionBD.insertProvincia(provincia);
                    }
                    //Crear localidad
                    localidad localidad = new localidad();
                    if (centro != null)
                    {
                        if (dynamicData.loccen != null && (dynamicData.cpcen.ToString().Length == 6 || dynamicData.cpcen.ToString().Length == 5))
                        {
                            localidad.codigo = dynamicData.cpcen.ToString().Substring(2, 3);
                            localidad.nombre = dynamicData.loccen;
                            centro.loc_codigo = localidad.codigo;
                            centro.loc_nombre = localidad.nombre;
                        }
                        else
                        {
                            localidad = null;
                        }
                    }
                    else { localidad = null; }
                    if (localidad != null)
                    {
                        localidad.prov_codigo = provincia.codigo;
                        ConexionBD.ConexionBD.insertLocalidad(localidad);
                    }

                }
                foreach (var centro in ListaCentros)
                {
                    if (centro != null)
                    {
                        if (await ConexionBD.ConexionBD.insertCentro(centro))
                        {
                            inserts++;
                        }
                        else
                        {
                            eliminados += $"(Múrcia, {centro.nombre}, {centro.loc_nombre}, Ya existe en la base de datos)\r\n";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                
            }
            result.Eliminados = eliminados;
            result.Reparados = reparados;
            result.Inserts = inserts;
            return result;
        }

        public static centro_educativo JsonACentro(dynamic dynamicData)
        {
            centro_educativo centro = new centro_educativo();
            //nombre del centro
            if (dynamicData.dencen != null)
            {
                centro.nombre = dynamicData.denCorta + " " + dynamicData.dencen;
            }
            else
            {
                return null;
            }
            //codigo postal
            if (dynamicData.cpcen.ToString().Length == 5)
            {
                centro.cod_postal = dynamicData.cpcen.ToString();
            }
            else
            {
                eliminados += $"(Múrcia, {centro.nombre}, {dynamicData.loccen}, No tiene codigo postal)\r\n";
                return null;
            }
            //telefono
            if (dynamicData.telcen == null) { centro.telefono = 0; }
            if (dynamicData.telcen.ToString().Length == 9 )
            {
                centro.telefono = dynamicData.telcen;
            }
            else
            {
                
                eliminados += $"(Múrcia, {centro.nombre}, {dynamicData.loccen}, El número de télefono tiene menos de 9 dígitos)\r\n";
                return null;
            }
            //direccion
            

            if (!string.IsNullOrEmpty(dynamicData.domcen.ToString()))
            {
                centro.direccion = dynamicData.domcen;
            }
            else
            {
                eliminados += $"(Múrcia, {centro.nombre}, {dynamicData.loccen}, No tiene dirección)\r\n";
                return null;
            }
            //descripcion
            centro.descripcion = dynamicData.presentacionCorta;
            //latitud
            if (dynamicData["geo-referencia"]["lat"] != null)
            {
                centro.latitud = dynamicData["geo-referencia"]["lat"].ToString().Replace(",", ".");
            }
            else
            {
                eliminados += $"(Múrcia, {centro.nombre}, {dynamicData.loccen}, No tiene las coordenadas geográficas(Latitud))\r\n";
                return null;
            }
            //longitud
            if (dynamicData["geo-referencia"]["lon"] != null)
            {
                centro.longitud = dynamicData["geo-referencia"]["lon"].ToString().Replace(",", ".");
            }
            else
            {
                eliminados += $"(Múrcia, {centro.nombre}, {dynamicData.loccen}, No tiene las coordenadas geográficas(Longitud))\r\n";
                return null;
            }

            //tipo de centro
            if (dynamicData.titularidad != null)
            {
                string tipo = dynamicData.titularidad;
                switch (tipo)
                {
                    case "P":
                        centro.tipo = tipo_centro.Público;
                        break;
                    case "N":
                        centro.tipo = tipo_centro.Privado;
                        break;
                    case "C":
                        centro.tipo = tipo_centro.Concertado;
                        break;
                    default:
                        return null;
                }
            }
            else { return null; }
            return centro;

        }


    }
}

