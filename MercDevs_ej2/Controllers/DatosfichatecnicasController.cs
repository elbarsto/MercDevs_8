using System;
using System.Collections.Generic;
using System.IO; // Asegúrate de incluir esto para MemoryStream
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MercDevs_ej2.Models;
using iTextSharp.text.pdf;
using iTextSharp.text;

namespace MercDevs_ej2.Controllers
{
    public class DatosfichatecnicasController : Controller
    {
        private readonly MercyDeveloperContext _context;

        public DatosfichatecnicasController(MercyDeveloperContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> FichaTecnica(int? id)
        {
            var DatosFicha = _context.Datosfichatecnicas
                .Where(d => d.IdDatosFichaTecnica == id)
                .Include(d => d.RecepcionEquipo)
                .Include(d => d.Diagnosticosolucions);
            return View(await DatosFicha.ToListAsync());
        }

        public async Task<IActionResult> Inicio()
        {
            var mercydevsEjercicio2Context = _context.Datosfichatecnicas.Include(d => d.RecepcionEquipo);
            return View(await mercydevsEjercicio2Context.ToListAsync());
        }

        public async Task<IActionResult> Index(int id)
        {
            var fichaTecnica = await _context.Datosfichatecnicas
                .Include(d => d.RecepcionEquipo)
                .Include(d => d.Diagnosticosolucions)
                .Include(d => d.RecepcionEquipo.IdClienteNavigation)
                .Include(d => d.RecepcionEquipo.IdServicioNavigation)
                .FirstOrDefaultAsync(d => d.RecepcionEquipoId == id);

            if (fichaTecnica == null)
            {
                return RedirectToAction("Create", new { id });
            }

            return View(fichaTecnica);
        }

        public async Task<IActionResult> VerDatosFichaTecnicaPorRecepcion(int id)
        {
            var mercydevsEjercicio2Context = _context.Datosfichatecnicas
                .Where(d => d.RecepcionEquipoId == id)
                .Include(d => d.RecepcionEquipo);
            ViewData["IdRecepcionEquipo"] = id;
            return View(await mercydevsEjercicio2Context.ToListAsync());
        }

        public async Task<IActionResult> Diagnosticosolucionpordatosficha(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var verdiagnostico = await _context.Datosfichatecnicas
                .Include(r => r.Diagnosticosolucions)
                .Include(r => r.RecepcionEquipo)
                .Include(d => d.RecepcionEquipo.IdClienteNavigation)
                .FirstOrDefaultAsync(m => m.IdDatosFichaTecnica == id);
            if (verdiagnostico == null)
            {
                return NotFound();
            }

            return View(verdiagnostico);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datosfichatecnica = await _context.Datosfichatecnicas
                .Include(d => d.RecepcionEquipo)
                .FirstOrDefaultAsync(m => m.IdDatosFichaTecnica == id);
            if (datosfichatecnica == null)
            {
                return NotFound();
            }

            return View(datosfichatecnica);
        }

        public IActionResult Create(int? id)
        {
            ViewData["RecepcionEquipoId"] = new SelectList(_context.Recepcionequipos, "Id", "Id");
            ViewData["IdRecepcionEquipo"] = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int? id, [Bind("IdDatosFichaTecnica,FechaInicio,FechaFinalizacion,PobservacionesRecomendaciones,Soinstalado,SuiteOfficeInstalada,LectorPdfinstalado,NavegadorWebInstalado,AntivirusInstalado,RecepcionEquipoId")] Datosfichatecnica datosfichatecnica)
        {
            if (datosfichatecnica.FechaInicio != null)
            {
                datosfichatecnica.RecepcionEquipoId = Convert.ToInt32(id);
                _context.Add(datosfichatecnica);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Recepcionequipoes");
            }
            ViewData["RecepcionEquipoId"] = new SelectList(_context.Recepcionequipos, "Id", "Id", datosfichatecnica.RecepcionEquipoId);
            return View(datosfichatecnica);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datosfichatecnica = await _context.Datosfichatecnicas.FindAsync(id);
            if (datosfichatecnica == null)
            {
                return NotFound();
            }
            ViewData["RecepcionEquipoId"] = new SelectList(_context.Recepcionequipos, "Id", "Id", datosfichatecnica.RecepcionEquipoId);
            return View(datosfichatecnica);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdDatosFichaTecnica,FechaInicio,FechaFinalizacion,PobservacionesRecomendaciones,Soinstalado,SuiteOfficeInstalada,LectorPdfinstalado,NavegadorWebInstalado,AntivirusInstalado,RecepcionEquipoId")] Datosfichatecnica datosfichatecnica)
        {
            if (id != datosfichatecnica.IdDatosFichaTecnica)
            {
                return NotFound();
            }

            if (datosfichatecnica.FechaInicio != null)
            {
                try
                {
                    _context.Update(datosfichatecnica);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DatosfichatecnicaExists(datosfichatecnica.IdDatosFichaTecnica))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Inicio));
            }
            ViewData["RecepcionEquipoId"] = new SelectList(_context.Recepcionequipos, "Id", "Id", datosfichatecnica.RecepcionEquipoId);
            return View(datosfichatecnica);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var datosfichatecnica = await _context.Datosfichatecnicas
                .Include(d => d.RecepcionEquipo)
                .FirstOrDefaultAsync(m => m.IdDatosFichaTecnica == id);
            if (datosfichatecnica == null)
            {
                return NotFound();
            }

            return View(datosfichatecnica);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var datosfichatecnica = await _context.Datosfichatecnicas.FindAsync(id);
            if (datosfichatecnica != null)
            {
                _context.Datosfichatecnicas.Remove(datosfichatecnica);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DatosfichatecnicaExists(int id)
        {
            return _context.Datosfichatecnicas.Any(e => e.IdDatosFichaTecnica == id);
        }

        public async Task<IActionResult> GeneratePdf(int id)
        {
            var DatosfichaTecnica = await _context.Datosfichatecnicas
                .Include(d => d.RecepcionEquipo)
                .Include(d => d.RecepcionEquipo.IdClienteNavigation)
                .FirstOrDefaultAsync(d => d.IdDatosFichaTecnica == id);

            if (DatosfichaTecnica == null)
            {
                return NotFound();
            }

            using (var ms = new MemoryStream())
            {
                var document = new Document(PageSize.A4, 25, 25, 30, 30);
                var writer = PdfWriter.GetInstance(document, ms);
                document.Open();

                // Título del PDF
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                var normalFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                var boldFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);

                var titleParagraph = new Paragraph("Ficha Técnica", titleFont);
                titleParagraph.Alignment = Element.ALIGN_CENTER;
                document.Add(titleParagraph);
                document.Add(new Paragraph(" "));

                // Tabla 1: Información general
                var table1 = new PdfPTable(3);
                table1.WidthPercentage = 100;
                table1.AddCell(new PdfPCell(new Phrase($"ID Ficha: {DatosfichaTecnica.IdDatosFichaTecnica}", boldFont)) { Border = Rectangle.BOTTOM_BORDER });
                table1.AddCell(new PdfPCell(new Phrase($"ID Recepcion: {DatosfichaTecnica.RecepcionEquipo.Id}", boldFont)) { Border = Rectangle.BOTTOM_BORDER });
                table1.AddCell(new PdfPCell(new Phrase($"Fecha: {DatosfichaTecnica.FechaInicio?.ToString("dd/MM/yyyy")}", boldFont)) { Border = Rectangle.BOTTOM_BORDER });
                document.Add(table1);

                // Tabla 2: Datos del Usuario
                document.Add(new Paragraph("Datos del Usuario", boldFont));
                var table2 = new PdfPTable(3);
                table2.WidthPercentage = 100;
                table2.AddCell(new PdfPCell(new Phrase($"Nombre: {DatosfichaTecnica.RecepcionEquipo?.IdClienteNavigation.Nombre}", normalFont)) { Border = Rectangle.BOTTOM_BORDER });
                table2.AddCell(new PdfPCell(new Phrase($"Apellido: {DatosfichaTecnica.RecepcionEquipo?.IdClienteNavigation.Apellido}", normalFont)) { Border = Rectangle.BOTTOM_BORDER });
                table2.AddCell(new PdfPCell(new Phrase($"Celular: {DatosfichaTecnica.RecepcionEquipo?.IdClienteNavigation.Telefono}", normalFont)) { Border = Rectangle.BOTTOM_BORDER });
                table2.AddCell(new PdfPCell(new Phrase($"Correo: {DatosfichaTecnica.RecepcionEquipo?.IdClienteNavigation.Correo}", normalFont)) { Colspan = 2, Border = Rectangle.BOTTOM_BORDER });
                table2.AddCell(new PdfPCell(new Phrase($"Dirección: {DatosfichaTecnica.RecepcionEquipo?.IdClienteNavigation.Direccion}", normalFont)) { Border = Rectangle.BOTTOM_BORDER });
                document.Add(table2);

                // Tabla 3: Observaciones del Usuario
                document.Add(new Paragraph("Observaciones del Usuario", boldFont));
                var table3 = new PdfPTable(3);
                table3.WidthPercentage = 100;
                table3.AddCell(new PdfPCell(new Phrase("N°", boldFont)) { Border = Rectangle.BOTTOM_BORDER });
                table3.AddCell(new PdfPCell(new Phrase("Descripcion", boldFont)) { Border = Rectangle.BOTTOM_BORDER });
                table3.AddCell(new PdfPCell(new Phrase("Check", boldFont)) { Border = Rectangle.BOTTOM_BORDER });
                table3.AddCell(new PdfPCell(new Phrase("1", normalFont)) { Border = Rectangle.BOTTOM_BORDER });
                table3.AddCell(new PdfPCell(new Phrase($"{DatosfichaTecnica.PobservacionesRecomendaciones}", normalFont)) { Border = Rectangle.BOTTOM_BORDER });
                table3.AddCell(new PdfPCell(new Phrase("", normalFont)) { Border = Rectangle.BOTTOM_BORDER });
                table3.AddCell(new PdfPCell(new Phrase("2", normalFont)) { Border = Rectangle.BOTTOM_BORDER });
                table3.AddCell(new PdfPCell(new Phrase("", normalFont)) { Border = Rectangle.BOTTOM_BORDER });
                table3.AddCell(new PdfPCell(new Phrase("", normalFont)) { Border = Rectangle.BOTTOM_BORDER });
                table3.AddCell(new PdfPCell(new Phrase("3", normalFont)) { Border = Rectangle.BOTTOM_BORDER });
                table3.AddCell(new PdfPCell(new Phrase("", normalFont)) { Border = Rectangle.BOTTOM_BORDER });
                table3.AddCell(new PdfPCell(new Phrase("", normalFont)) { Border = Rectangle.BOTTOM_BORDER });
                document.Add(table3);

                // Añadir más tablas según sea necesario para cada sección del índice

                document.Close();
                writer.Close();

                return File(ms.ToArray(), "application/pdf", "FichaTecnica.pdf");
            }
        }

    }

}

