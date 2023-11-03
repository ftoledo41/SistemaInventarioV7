using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SistemaInventarioV7.AccesoDatos.Repositorio.IRepositorio
{
    //al agregar <T> where T : class indicamos que sera una clase generica
    public interface IRepositorio<T> where T : class
    {
        Task<T> Obtener(int id);
        Task<IEnumerable<T>> ObtenerTodos(
            Expression<Func<T, bool>> filtro=null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBY = null,
            string IncluirPropiedades = null,
            bool isTracking = true
            );

        Task<T> ObtenerPrimero(
            Expression<Func<T, bool>> filtro = null,
            string IncluirPropiedades = null,
            bool isTracking = true
            );

        Task Agregar(T entidad);
        void Remover(T entidad);
        void RemoverRango(IEnumerable<T> entidad);

    }
}
