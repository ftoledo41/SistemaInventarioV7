using Microsoft.AspNetCore.Mvc;
using SistemaInventarioV7.AccesoDatos.Repositorio.IRepositorio;
using SistemaInventarioV7.Modelos;
using SistemaInventarioV7.Modelos.ViewModels;
using SistemaInventarioV7.Utilidades;

namespace SistemaInventarioV7.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductoController : Controller
    {
        //Con esto ya podemos utilizar todo lo que envuelve a la unidad de trabajo
        private readonly IUnidadTrabajo _unidadTrabajo;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductoController(IUnidadTrabajo unidadTrabajo, IWebHostEnvironment webHostEnvironment)
        {
            _unidadTrabajo = unidadTrabajo;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        //? es por si es nulo es un registro nuevo; caso contrario lo actualiza
        public async Task<IActionResult> UpSert(int? id)
        {

            ProductoVM productoVM = new ProductoVM()
            {
                Producto = new Producto(),
                CategoriaLista = _unidadTrabajo.Producto.ObtenerTodosDropDownList("Categoria"),
                MarcaLista = _unidadTrabajo.Producto.ObtenerTodosDropDownList("Marca"),
                PadreLista = _unidadTrabajo.Producto.ObtenerTodosDropDownList("Producto")
            };

            if (id == null)
            {
                productoVM.Producto.Estado = true;
                return View(productoVM);
            }
            else
            {
                productoVM.Producto = await _unidadTrabajo.Producto.Obtener(id.GetValueOrDefault());
                if (productoVM.Producto == null)
                {
                    return NotFound();
                }
                return View(productoVM);
            }                      
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductoVM productoVM)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                string webRootPatch = _webHostEnvironment.WebRootPath;

                if(productoVM.Producto.Id ==0) {

                    string upload = webRootPatch + DS.ImagenRuta;
                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);

                    using (var fileStrem = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStrem);
                    }
                    productoVM.Producto.ImagenUrl = fileName + extension;
                    await _unidadTrabajo.Producto.Agregar(productoVM.Producto);
                }
                else
                {
                    var objProducto = await _unidadTrabajo.Producto.ObtenerPrimero(p => p.Id == productoVM.Producto.Id, isTracking:false);
                    if (files.Count > 0)
                    {
                        string upload =webRootPatch + DS.ImagenRuta;
                        string fileName = Guid.NewGuid().ToString();
                        string extension = Path.GetExtension(files[0].FileName);

                        var anteriorFile = Path.Combine(upload, fileName + extension);
                        if (System.IO.File.Exists(anteriorFile))
                        {
                            System.IO.File.Delete(anteriorFile);
                        }
                        using (var fileStrem = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                        {
                            files[0].CopyTo(fileStrem);
                        }
                        productoVM.Producto.ImagenUrl = fileName + extension;
                    }
                    else
                    {
                        productoVM.Producto.ImagenUrl = objProducto.ImagenUrl;
                    }
                    _unidadTrabajo.Producto.Actualizar(productoVM.Producto);
                }

                TempData[DS.Exitosa] = "Transaccion Exitosa!";
                await _unidadTrabajo.Guardar();
                return RedirectToAction("Index");
            }
            productoVM.CategoriaLista = _unidadTrabajo.Producto.ObtenerTodosDropDownList("Categoria");
            productoVM.MarcaLista = _unidadTrabajo.Producto.ObtenerTodosDropDownList("Marca");
            productoVM.PadreLista = _unidadTrabajo.Producto.ObtenerTodosDropDownList("Producto");
            return View(productoVM);


        }

        #region API
        //Es API porque llamamos a algo externo para realizar la accion, en este caso, llamamos a código JavaScript
        [HttpGet]
        // IActionResult devuelve tambien objetos de tipo json
        //IncluirPropiedades:"Categoria,Marca" para que a parte de los productos
        //Tambien traiga las cattegorias y la marcas
        public async Task<IActionResult> ObtenerTodos()
        {
            var todos = await _unidadTrabajo.Producto.ObtenerTodos(IncluirPropiedades:"Categoria,Marca");
            //data guardara todo lo obtenido
            return Json(new { data = todos });
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var productoDb = await _unidadTrabajo.Producto.Obtener(id);
            if(productoDb == null)
            {
                return Json(new { success = false, message = "Error al borrar Producto" });
            }

            // Remover imagen
            string upload = _webHostEnvironment.WebRootPath + DS.ImagenRuta;
            var anteriorFile = Path.Combine(upload, productoDb.ImagenUrl);
            if (System.IO.File.Exists(anteriorFile))
            {
                System.IO.File.Delete(anteriorFile);
            }

            _unidadTrabajo.Producto.Remover(productoDb);
            await _unidadTrabajo.Guardar();
            return Json(new { success = true, message = "Producto borrado exitosamente" });
        }

        [ActionName("ValidarSerie")]
        public async Task<IActionResult> ValidarSerie(string serie, int id=0)
        {
            bool valor = false;
            var lista = await _unidadTrabajo.Producto.ObtenerTodos();
            if (id == 0)
            {
                valor = lista.Any(b => b.NumeroSerie.ToLower().Trim() == serie.ToLower().Trim());
            }
                
            else
            {
                valor = lista.Any(b => b.NumeroSerie.ToLower().Trim() == serie.ToLower().Trim() && b.Id != id);
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
