using MercDevs_ej2.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks; // Agregar este using para manejar tareas asincrónicas

namespace MercDevs_ej2.Controllers
{
    [Authorize] // Requiere autenticación para acceder a cualquier acción del controlador
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MercyDeveloperContext _context;

        public HomeController(ILogger<HomeController> logger, MercyDeveloperContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var fichasTecnicas = await _context.Datosfichatecnicas
                .Include(d => d.RecepcionEquipo.IdClienteNavigation)
                .Include(d => d.RecepcionEquipo.IdServicioNavigation) // Incluir datos relacionados según sea necesario
                .Where(d => d.Estado == 1) // Filtrar por estado activo
                .ToListAsync();

            return View(fichasTecnicas);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Para cerrar sesión
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Ingresar", "Login");
        }
    }
}
