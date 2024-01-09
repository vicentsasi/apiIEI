using apiIEI.Extractors;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Crmf;
using practiquesIEI.Wrappers;

[ApiController]
[Route("api/[controller]")]
public class ExtractorController : ControllerBase
{
    [HttpPost("cv")]
    public async Task<IActionResult> ExtractCV()
    {
        try
        {
            string csvFileName = "CV.csv";
            string csvFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Fuentes de datos", csvFileName);
            string jsonFromCsv = CsvWrapper.ConvertCsvToJson(csvFilePath);

            ExtractionResult result =  await CVextractor.LoadJsonDataIntoDatabase(jsonFromCsv);
            if (result.ErrorMessage != null)
            {
                return BadRequest($"Error en la extracción para CVextractor: {result.ErrorMessage}");
            }

            return Ok($"Extracción exitosa para CVextractor\nEliminados: {result.Eliminados}\nReparados: {result.Reparados}\nInserts: {result.Inserts}");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error en la extracción para CVextractor: {ex.Message}");
        }
    }

    [HttpPost("cat")]
    public async Task<IActionResult> ExtractCAT()
    {
        try
        {
            string xmlFileName = "CAT.xml";
            string xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Fuentes de datos", xmlFileName);
            string jsonFromJsonFile = XmlWrapper.ConvertXmlToJson(xmlFilePath);

            ExtractionResult result =  await CATextractor.LoadJsonDataIntoDatabase(jsonFromJsonFile);
            if (result.ErrorMessage != null)
            {
                return BadRequest($"Error en la extracción para CATEXTRAXTOR: {result.ErrorMessage}");
            }

            return Ok($"Extracción exitosa para CATextractor\nEliminados: {result.Eliminados}\nReparados: {result.Reparados}\nInserts: {result.Inserts}");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error en la extracción para CATextractor: {ex.Message}");
        }
    }

    [HttpPost("mur")]
    public async Task<IActionResult> ExtractMUR()
    {
        try
        {
            string jsonFileName = "MUR.json";
            string jsonFilePath = Path.Combine(Directory.GetCurrentDirectory(), "Fuentes de datos", jsonFileName);
            string jsonFromXml = JsonWrapper.ConvertToJson(jsonFilePath);
            ExtractionResult result = await MURextractor.LoadJsonDataIntoDatabase(jsonFromXml);

            if (result.ErrorMessage != null)
            {
                return BadRequest($"Error en la extracción para MURextractor: {result.ErrorMessage}");
            }

            return Ok($"Extracción exitosa para MURextractor\nEliminados: {result.Eliminados}\nReparados: {result.Reparados}\nInserts: {result.Inserts}");


        }
        catch (Exception ex)
        {
            return BadRequest($"Error en la extracción para MURextractor: {ex.Message}");
        }
    }

    [HttpGet("getAllCentros")]
    public async Task<IActionResult> GetAllCentros()
    {
        try
        {
            await ConexionBD.Conectar();  // Asegúrate de que la conexión esté establecida antes de llamar a los métodos.
            var centros = await ConexionBD.getAllCentros();

            if (centros != null)
            {
                return Ok(centros);
            }
            else
            {
                return NotFound("No se encontraron centros educativos.");
            }
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener centros educativos: {ex.Message}");
        }
    }

    [HttpGet("findCentros")]
    public async Task<IActionResult> FindCentros([FromQuery] string loc, [FromQuery] string tipo, [FromQuery] string prov, [FromQuery] string cp)
    {
        try
        {
            await ConexionBD.Conectar();  // Asegúrate de que la conexión esté establecida antes de llamar a los métodos.
            var centros = await ConexionBD.FindCentros(loc, tipo, prov, cp);

            if (centros != null)
            {
                return Ok(centros);
            }
            else
            {
                return NotFound("No se encontraron centros educativos que coincidan con los criterios de búsqueda.");
            }
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al buscar centros educativos: {ex.Message}");
        }
    }

    [HttpGet("borrar")]
    public async Task<IActionResult> borrarAllCentros()
    {
        try
        {
            await ConexionBD.Conectar();  // Asegúrate de que la conexión esté establecida antes de llamar a los métodos.
             await ConexionBD.BorrarCentros();
            return Ok("Centros Borrados");



        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener centros educativos: {ex.Message}");
        }
    }
}


