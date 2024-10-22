namespace TiendaOnline.Models
{
    public class ApiResponseModel<T>
    {
        public string Mensaje { get; set; }
        public T Response { get; set; }
    }
}
