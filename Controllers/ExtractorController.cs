using apiIEI.ConexionBD;
using apiIEI.Entities;
using apiIEI.Extractors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Crmf;
using practiquesIEI.Entities;

[ApiController]
[Route("api/[controller]")]
public class ExtractorController : ControllerBase
{
    [HttpPost("cv")]
    public async Task<IActionResult> ExtractCV()
    {
        try
        {
            string jsonFromCsv = "";
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var apiUrl = "https://localhost:7267/api/WrapperCsv/CsvToJson";
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        jsonFromCsv = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        Console.WriteLine($"Error en la llamada a la API. C�digo de estado: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            ExtractionResult result =  await CVextractor.LoadJsonDataIntoDatabase(jsonFromCsv);
            if (result.ErrorMessage != null)
            {
                return BadRequest($"Error en la extracci�n para CVextractor: {result.ErrorMessage}");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error en la extracci�n para CVextractor: {ex.Message}");
        }
    }

    [HttpPost("cat")]
    public async Task<IActionResult> ExtractCAT()
    {
        try
        {
            string jsonFromXmlFile = "";
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var apiUrl = "https://localhost:7250/api/WrapperXML/XmlToJson";
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        jsonFromXmlFile = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        Console.WriteLine($"Error en la llamada a la API. C�digo de estado: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            ExtractionResult result =  await CATextractor.LoadJsonDataIntoDatabase(jsonFromXmlFile);
            if (result.ErrorMessage != null)
            {
                return BadRequest($"Error en la extracci�n para CATEXTRAXTOR: {result.ErrorMessage}");
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error en la extracci�n para CATextractor: {ex.Message}");
        }
    }

    [HttpPost("mur")]
    public async Task<IActionResult> ExtractMUR()
    {
        try
        {
            string jsonFromJsonFile = "";
            using (var httpClient = new HttpClient())
            {
                try
                {
                    var apiUrl = "https://localhost:7197/api/WrapperJson/JsonToJson";
                    HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                    if (response.IsSuccessStatusCode)
                    {
                        jsonFromJsonFile = await response.Content.ReadAsStringAsync();
                    }
                    else
                    {
                        Console.WriteLine($"Error en la llamada a la API. C�digo de estado: {response.StatusCode}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            ExtractionResult result = await MURextractor.LoadJsonDataIntoDatabase(jsonFromJsonFile);

            if (result.ErrorMessage != null)
            {
                return BadRequest($"Error en la extracci�n para MURextractor: {result.ErrorMessage}");
            }

            return Ok(result);


        }
        catch (Exception ex)
        {
            return BadRequest($"Error en la extracci�n para MURextractor: {ex.Message}");
        }
    }

    

    [HttpPost("borrar")]
    public async Task<IActionResult> borrarAllCentros()
    {
        try
        {
            await ConexionBD.BorrarCentros();
            return Ok("Centros Borrados");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener centros educativos: {ex.Message}");
        }
    }
}


