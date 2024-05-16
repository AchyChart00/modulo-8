namespace WebApiAutores.DTOs
{
    public class PaginacionDTO
    {
        public int Pagina { get; set; } = 1;
        private int recordsPorPagina { get; set; } = 10;
        private readonly int CantidadMaximaPorPagina = 50;

        public int RecordsPorPagina
        {
            get
            {
                return recordsPorPagina;
            }

            set
            {
                recordsPorPagina = value > CantidadMaximaPorPagina ? CantidadMaximaPorPagina : value;
            }
        }
    }
}
