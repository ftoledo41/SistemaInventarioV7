using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using SistemaInventarioV7.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventarioV7.Modelos;
using SistemaInventarioV7.Utilidades;

namespace SistemaInventarioV7.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriaController : Controller
    {
        //Con esto ya podemos utilizar todo lo que envuelve a la unidad de trabajo
        private readonly IUnidadTrabajo _unidadTrabajo;
        public CategoriaController(IUnidadTrabajo unidadTrabajo)
        {
            _unidadTrabajo = unidadTrabajo;
        }
        public IActionResult Index()
        {
            return View();
        }
        //? es por si es nulo es un registro nuevo; caso contrario lo actualiza
        public async Task<IActionResult> UpSert(int? id)
        {
            Categoria categoria = new Categoria();
            if(id == null) 
            {
                categoria.Estado = true;
                return View(categoria);
            }
            categoria = await _unidadTrabajo.Categoria.Obtener(id.GetValueOrDefault());
            if(categoria == null)
            {
                return NotFound();
            }
            return View(categoria);            
        }

        [HttpPost]
        //ValidateAntiForgeryToken Sirve para la evitar la falsificacion de solicitudes de un sitio de
        //otra pagina que intenta cargar registros en nuestra pagina
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Upsert(Categoria categoria)
        {
            if (ModelState.IsValid)
            {
                if(categoria.Id == 0)
                {
                    await _unidadTrabajo.Categoria.Agregar(categoria);
                    TempData[DS.Exitosa] = "Categoria creada exitosamente";
                }
                else
                {
                    _unidadTrabajo.Categoria.Actualizar(categoria);
                    TempData[DS.Exitosa] = "Categoria actualizada exitosamente";
                }
                await _unidadTrabajo.Guardar();
                return RedirectToAction(nameof(Index));
            }
            TempData[DS.Error] = "Error al grabar Categoria";
            return View(categoria);
        }

        #region API
        //Es API porque llamamos a algo externo para realizar la accion, en este caso, llamamos a código JavaScript
        [HttpGet]
        // IActionResult devuelve tambien objetos de tipo json
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Categoria.ObtenerTodos();
            //data guardara todo lo obtenido
            return Json(new { data = todos });
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var categoriaDb = await _unidadTrabajo.Categoria.Obtener(id);
            if(categoriaDb == null)
            {
                return Json(new { success = false, message = "Error al borrar Categoria" });
            }
            _unidadTrabajo.Categoria.Remover(categoriaDb);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Categoria borrada exitosamente" });
        }

        [ActionName("ValidarNombre")]
        public async Task<IActionResult> ValidarNombre(string nombre, int id=0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Categoria.ObtenerTodos();
            if (id == 0)
            {
                valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
            }
                
            else
            {
                valor = lista.Any(b => b.Nombre.ToLower().Trim() == nombre.ToLower().Trim() && b.Id != id);
            }                

            if (valor)
            {
                return Json(new { data = true });
            }
            return Json(new { data = false });
        }

        #endregion
    }
}
