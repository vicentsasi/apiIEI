using apiIEI.Entities;
using Newtonsoft.Json;
using practiquesIEI.Entities;

namespace apiIEI.Extractors
{
    public class CATextractor
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
            try {
                // Deserializar JSON a una lista de objetos dinámicos
                dynamic jsontext = JsonConvert.DeserializeObject<dynamic>(jsonData);
                List<centro_educativo> ListaCentros = new List<centro_educativo>();
                foreach (var row in jsontext.response.row.row)
                {
                    centro_educativo centro = JsonACentro(row);
                    ListaCentros.Add(centro);
                    provincia provincia = new provincia();

                    if (centro != null)
                    {
                        provincia.codigo = centro.cod_postal.ToString().Substring(0, 2);
                        if (provincia.codigo != "0")
                        {
                            switch (provincia.codigo) {
                                case "08": provincia.nombre = "Barcelona"; break;
                                case "17": provincia.nombre = "Girona"; break;
                                case "43": provincia.nombre = "Tarragona"; break;
                                case "25": provincia.nombre = "Lleida"; break;
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

                    if (centro != null)
                    {
                        localidad.codigo = row.codi_municipi_6_digits.ToString().Substring(2,4);
                        localidad.prov_codigo = provincia.codigo;
                        centro.loc_codigo = localidad.codigo;
                        if (row.nom_municipi != null)
                        {
                            localidad.nombre = row.nom_municipi;
                            centro.loc_nombre = localidad.nombre;
                        }
                        else { localidad = null; }
                    }
                    else { localidad = null; }

                    if (localidad != null)
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
                            eliminados += $"(Catalunya, {centro.nombre}, {centro.loc_nombre}, Ya existe en la base de datos)\r\n";
                        }
                    }
                }
                //rest.Add(inserts.ToString());
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
        public static centro_educativo JsonACentro(dynamic row)
        {
            try
            {
                centro_educativo centro = new centro_educativo();
                // Extraer propiedades específicas y construir las columnas que deseas
                //nombre
                if (row.denominaci_completa != null)
                {
                    centro.nombre = row.denominaci_completa;
                }
                else
                {
                    return null;
                }
                //Dirección
                if (row.adre_a != null)
                {
                    centro.direccion = row.adre_a;
                }
                else
                {
                    eliminados += $"(Catalunya, {centro.nombre}, {row.nom_municipi}, No tiene la dirección del centro)\r\n";
                    return null;
                }
                //Codigo Postal
                if (row.codi_postal != null && (row.codi_postal.ToString().Length == 5 || row.codi_postal.ToString().Length == 4))
                {
                    if (row.codi_postal.ToString().Length == 4)
                    {
                        centro.cod_postal = '0' + row.codi_postal.ToString();
                        reparados += $"(Catalunya, {centro.nombre}, {row.nom_municipi}, El código postal contiene 4 dígitos, Se ha añadido un 0 delante)\r\n";
                    }
                    else { centro.cod_postal = row.codi_postal; }
                }
                else
                {
                    eliminados += $"(Catalunya, {centro.nombre}, {row.nom_municipi}, No tiene codigo postal)\r\n";
                    return null;
                }

                //telefono
                centro.telefono = 0;
                //descripción
                centro.descripcion = row.estudis;

                string regimen = row.nom_naturalesa;
                if (!string.IsNullOrEmpty(regimen))
                {
                    switch (regimen)
                    {
                        case "Públic": centro.tipo = tipo_centro.Público; break;
                        case "Privat": centro.tipo = tipo_centro.Privado; break;
                       
                    }
                }
                else {
                    eliminados += $"(Catalunya, {centro.nombre}, {row.nom_municipi}, No tiene el tipo de centro)\r\n";
                    return null; }

                if (row.coordenades_geo_x != null)
                {
                    centro.longitud = row.coordenades_geo_x;
                }
                else
                {
                    eliminados += $"(Catalunya, {centro.nombre}, {row.nom_municipi}, No tiene las coordenadas de geolocalización)\r\n";
                    return null;
                }


                if (row.coordenades_geo_y != null)
                {
                    centro.latitud = row.coordenades_geo_y;
                }
                else
                {
                    eliminados += $"(Catalunya, {centro.nombre}, {row.nom_municipi}, No tiene las coordenadas de geolocalización)\r\n";
                    return null;
                }


                return centro;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al convertir el JSON a objeto centro: {ex.Message}");
                return null;
            }
        }
       

    }
}

