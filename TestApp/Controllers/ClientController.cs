using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestApp.ServiceReference1;

namespace TestApp.Controllers
{
    public class ClientController : Controller
    {

        //Ejemplos de almacenamiento de cache, para mejorar rendimiento en la vista donde se muestra cantidades grandes informacion
        private const string CacheKeyClientes = "ClientesCache";
        private const string CacheKeyDirecciones = "DireccionesCache";
        private const int CacheDurationMiliSegundos = 30; 

        // GET: Client
        public ActionResult Index()
        {
            // Obtener datos de caché
            var clientes = HttpRuntime.Cache[CacheKeyClientes] as List<vw_clientes>;
            var direcciones = HttpRuntime.Cache[CacheKeyDirecciones] as List<vw_direccion>;

            // Si no están en caché, obtener de la fuente de datos y almacenar en caché
            if (clientes == null || direcciones == null)
            {
                using (var client = new ClienteClient())
                {
                    clientes = client.ObtenerClientes().ToList();
                    direcciones = client.ObtenerDireccion().ToList();

                    // Almacenar en caché
                    HttpRuntime.Cache.Insert(CacheKeyClientes, clientes, null, DateTime.Now.AddMilliseconds(CacheDurationMiliSegundos), TimeSpan.Zero);
                    HttpRuntime.Cache.Insert(CacheKeyDirecciones, direcciones, null, DateTime.Now.AddMilliseconds(CacheDurationMiliSegundos), TimeSpan.Zero);
                }
            }

            // Pasar datos a la vista
            ViewBag.List = clientes;
            ViewBag.cbdireccion = direcciones;

            return View();
        }

        public ActionResult Modal()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AccionCliente(FormCollection frm)
        {
            try
            {
                var id = frm["id"];
                var txtIdentificacion = frm["txtidentificacion"].Trim();
                var cbTipo = frm["cbtipo"];
                var txtPrimerNombre = frm["txtprimer_nombre"].Trim();
                var txtSegundoNombre = frm["txtsegundo_nombre"].Trim();
                var txtPrimerApellido = frm["txtprimer_apellido"].Trim();
                var txtSegundoApellido = frm["txtsegundo_apellido"].Trim();
                var cbDireccion = frm["cbdireccion"];

                using (var client = new ClienteClient())
                {
                    if (string.IsNullOrWhiteSpace(id))
                    {
                        client.Alta(txtIdentificacion, cbTipo, txtPrimerNombre, txtSegundoNombre, txtPrimerApellido, txtSegundoApellido, int.Parse(cbDireccion));
                    }
                    else
                    {
                        client.Modificar(int.Parse(id), txtIdentificacion, cbTipo, txtPrimerNombre, txtSegundoNombre, txtPrimerApellido, txtSegundoApellido, int.Parse(cbDireccion));
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Manejo de errores
                ViewBag.ErrorMessage = "Error al procesar la solicitud. Inténtelo nuevamente." + ex.Message;
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult BajaCliente(string id)
        {
            try
            {
                using (var client = new ClienteClient())
                {
                    client.Baja(int.Parse(id));
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Manejo de errores
                ViewBag.ErrorMessage = "Error al dar de baja el cliente. Inténtelo nuevamente." + ex.Message;
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
