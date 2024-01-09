using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using practiquesIEI.Entities;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Windows.Forms;
using System.Reflection;
using OpenQA.Selenium.Support.UI;
using System.Globalization;
using System.Threading.Tasks;
using System.Drawing;
using apiIEI.Entities;

namespace apiIEI.Extractors
{
    public class CVextractor
    {

        public static string eliminados;
        public static string reparados;
        public static int inserts;
        public static async Task<ExtractionResult> LoadJsonDataIntoDatabase(string jsonData)
        {
            ExtractionResult result = new ExtractionResult(); // Crear una instancia de la clase ExtractionResult
            eliminados = "";
            reparados = "";
            inserts = 0;
            try
            { // Deserializar JSON a una lista de objetos dinámicos
                List<dynamic> dynamicDataList = JsonConvert.DeserializeObject<List<dynamic>>(jsonData);
                List<centro_educativo> ListaCentros = new List<centro_educativo>();
                foreach (var dynamicData in dynamicDataList)
                {
                    centro_educativo centro = JsonACentro(dynamicData);
                    ListaCentros.Add(centro);
                    provincia provincia = new provincia();

                    if (centro != null)
                    {
                        provincia.codigo = centro.cod_postal.ToString().Substring(0,2);
                        if (dynamicData.PROVINCIA != null)
                        {
                            switch (dynamicData.PROVINCIA.ToString())
                            {
                                case "ALICANTE/ALACANT": provincia.nombre = "Alicante"; break;
                                case "CASTELLÓN/CASTELLÓ": provincia.nombre = "Castellón"; break;
                                case "VALENCIA/VALÈNCIA": provincia.nombre = "Valencia"; break;
                            }
                            
                        }
                        else { provincia = null; }
                    }
                    else { provincia = null; }

                    if (provincia != null)
                    {
                        ConexionBD.ConexionBD.insertProvincia(provincia);
                    }

                    localidad localidad = new localidad();

                    if (centro != null && provincia != null)
                    {
                        localidad.codigo = centro.cod_postal.ToString().Substring(2,3);
                        centro.loc_codigo = localidad.codigo;
                        localidad.prov_codigo = provincia.codigo;
                        if (dynamicData.LOCALIDAD != null)
                        {
                            localidad.nombre = dynamicData.LOCALIDAD;
                            centro.loc_nombre = dynamicData.LOCALIDAD;
                        }
                        else { localidad = null; }
                    }
                    else { localidad = null; }

                    if (provincia != null)
                    {
                        ConexionBD.ConexionBD.insertLocalidad(localidad);
                    }

                }
                foreach (var centro in ListaCentros)
                {
                    if (centro != null)
                    {
                        //Se inserta el centro en la BD y se suma en el recuento de centros introducidos 
                        if (await ConexionBD.ConexionBD.insertCentro(centro))
                        {
                            inserts++;
                        }
                        else
                        {
                            eliminados += $"(Comunitat Valenciana, {centro.nombre}, {centro.loc_nombre}, Ya existe en la base de datos)\r\n";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al convertir el JSON a objetos: {ex.Message}"); 
            }
            result.Eliminados = eliminados;
            result.Reparados = reparados;
            result.Inserts = inserts;
            return result;
        }
        static centro_educativo JsonACentro(dynamic dynamicData)
        {
            try
            {
                centro_educativo centro = new centro_educativo();
                //nombre del centro
                if (dynamicData.DENOMINACION != null)
                {
                    centro.nombre = dynamicData.DENOMINACION;
                }
                else
                {
                    return null;
                }
                //direccion
                if (dynamicData.TIPO_VIA != null && dynamicData.DIRECCION != null && dynamicData.NUMERO != null)
                {
                    string tipoVia = dynamicData.TIPO_VIA;
                    string direccion = dynamicData.DIRECCION;
                    string numero = dynamicData.NUMERO;
                    // Concatenar los valores en una sola cadena para la columna "Direccion"
                    centro.direccion = $"{tipoVia} {direccion} {numero}";
                }
                else
                {
                    eliminados += $"(Comunitat Valenciana, {centro.nombre}, {dynamicData.LOCALIDAD}, No tiene la dirección del centro)\r\n";
                    return null;
                }
                // codigo postal
                if (dynamicData.CODIGO_POSTAL != null && (dynamicData.CODIGO_POSTAL.ToString().Length == 5 || dynamicData.CODIGO_POSTAL.ToString().Length == 4))
                {
                    if (dynamicData.CODIGO_POSTAL.ToString().Length == 4)
                    {
                        centro.cod_postal = '0' + dynamicData.CODIGO_POSTAL.ToString();
                        reparados += $"(Comunitat Valenciana, {centro.nombre}, {dynamicData.LOCALIDAD}, El código postal contiene 4 dígitos, Se ha añadido un 0 delante)\r\n";

                    }
                    else { centro.cod_postal = dynamicData.CODIGO_POSTAL; }
                }
                else
                {
                    eliminados += $"(Comunitat Valenciana, {centro.nombre}, {dynamicData.LOCALIDAD}, El código postal no contiene 5 dígitos)\r\n";
                    return null;
                }

                //telefono
                if (dynamicData.TELEFONO.ToString().Length == 9 || dynamicData.TELEFONO.ToString().Length == 0)
                {
                    if (dynamicData.TELEFONO.ToString().Length == 0) { centro.telefono = 0; }
                    else { centro.telefono = dynamicData.TELEFONO; }
                }
                else
                {
                    eliminados += $"(Comunitat Valenciana, {centro.nombre}, {dynamicData.LOCALIDAD}, El número de télefono tiene menos de 9 dígitos)\r\n";
                    return null;
                }
                //descripcion
                centro.descripcion = dynamicData.URL_ES.ToString();


               //tipo de centro
                string regimen = dynamicData.REGIMEN;
                switch (regimen)
                {
                    case "PÚB.": centro.tipo = tipo_centro.Público; break;
                    case "PRIV.": centro.tipo = tipo_centro.Privado; break;
                    case "PRIV. CONC.": centro.tipo = tipo_centro.Concertado; break;
                    case "OTROS": centro.tipo = tipo_centro.Otros; break;
                    default:
                        eliminados += $"(Comunitat Valenciana, {centro.nombre}, {dynamicData.LOCALIDAD}, El tipo de centro es erroneo)\r\n";
                        return null;

                }
                GetLatitudyLongitud(centro.direccion+","+ centro.cod_postal, centro);


                return centro;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener los datos para el centro: {ex.Message}");
                return null;
            }
        }

        public static void GetLatitudyLongitud(string direccion, centro_educativo centro)
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--headless", "--disable-gpu", "--no-sandbox", "--disable-software-rasterizer", "--disable-dev-shm-usage", "--disable-extensions", "--disable-notifications");
            options.AddArguments("--silent", "--disable-logging", "--log-level=3", "--log-file=log.txt");

            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService();
            driverService.HideCommandPromptWindow = true;  // Esto oculta la ventana de consola

            // Iniciar el navegador Chrome
            using (var driver = new ChromeDriver(driverService, options))
            {
                try
                {
                    // Navegar a la URL de Nominatim para obtener la latitud y longitud
                    driver.Navigate().GoToUrl($"https://nominatim.openstreetmap.org/search?format=json&q={direccion}");

                    // Esperar a que la página se cargue completamente
                    WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                    wait.Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));

                    // Obtener la respuesta en formato JSON
                    string responseJson = driver.FindElement(By.TagName("pre")).Text;

                    // Deserializar la respuesta JSON
                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseJson);

                    // Verificar si se obtuvo alguna información de geolocalización
                    if (jsonResponse.Count > 0)
                    {
                        // Obtener las coordenadas (latitud y longitud)
                        centro.latitud = jsonResponse[0].lat;
                        centro.longitud = jsonResponse[0].lon;
                    }
                    else
                    {
                        // Manejar el caso en el que no se encuentre la geolocalización
                        centro.latitud = null;
                        centro.longitud = null;
                        Console.WriteLine("No se ha podido obtener la geolocalización");
                    }
                }
                catch (Exception ex)
                {
                    // Manejar cualquier excepción que pueda ocurrir durante la obtención de la geolocalización
                    centro.latitud = null;
                    centro.longitud = null;
                    Console.WriteLine($"Error al obtener la geolocalización: {ex.Message}");
                }
                finally
                {
                    // Cerrar el navegador
                    driver.Quit();
                }
            }
        }

    }
}


