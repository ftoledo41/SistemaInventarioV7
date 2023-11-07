namespace SistemaInventarioV7.Modelos.ViewModels
{
    public class KardexInventarioVM
    {
        public Producto Producto { get; set; }

        public IEnumerable<KardexInventario> KardexInventarioLista { get; set; }

        public DateTime FechaInicio { get; set; }

        public DateTime FechaFinal { get; set; }
    }
}
