namespace SubirArchivos_API_Rest_Full.Models
{
    public class Documento
    {
        public int IdDocumento { get; set; }
        public string Descripcion { get; set; }
        public string Ruta { get; set; }

        public IFormFile Archivo { get; set; }//permitirá almacenar el archivo que se envia a traves de la API
    }
}
