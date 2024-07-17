using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using SubirArchivos_API_Rest_Full.Models;
namespace SubirArchivos_API_Rest_Full.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentoController : ControllerBase
    {
        private readonly string _rutaServidor;
        private readonly string _cadenaSql;
        public DocumentoController(IConfiguration config)
        {
            _rutaServidor = config.GetSection("Configuracion").GetSection("RutaServidor").Value;
            _cadenaSql = config.GetConnectionString("CadenaSQL");
        }

        [HttpPost]
        [Route("Subir")]
        [DisableRequestSizeLimit, RequestFormLimits(MultipartBodyLengthLimit =int.MaxValue, ValueLengthLimit = int.MaxValue)]
        public IActionResult Subir([FromForm] Documento request)
        {
            string rutaDocumento = Path.Combine(_rutaServidor, request.Archivo.FileName);//Servidor con el nombre con el cual será guardado el documento
            try
            {
                using (FileStream newFile = System.IO.File.Create(rutaDocumento))
                {
                    request.Archivo.CopyTo(newFile);
                    newFile.Flush();//EL documento va a ser guardado en nuestra ruta de serv
                }
                using(var conexion = new SqlConnection(_cadenaSql))
                {
                    conexion.Open();
                    var cmd = new SqlCommand("sp_guardar_documento",conexion);
                    cmd.Parameters.AddWithValue("descripcion", request.Descripcion);
                    cmd.Parameters.AddWithValue("ruta", rutaDocumento);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.ExecuteNonQuery();

                }
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "documento guardado" });
            }
            catch (Exception error)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = error.Message});

            }
        }
    }
}
