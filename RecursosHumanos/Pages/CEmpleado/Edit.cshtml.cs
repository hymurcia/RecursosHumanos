﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecursosHumanos.DAL;
using RecursosHumanos.Models;

namespace RecursosHumanos.Pages.CEmpleado
{
    public class EditModel : PageModel
    {
        private readonly RecursosHumanos.DAL.Db _context;

        public EditModel(RecursosHumanos.DAL.Db context)
        {
            _context = context;
        }

        [BindProperty]
        public Empleado Empleado { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var empleado =  await _context.Empleado.FirstOrDefaultAsync(m => m.Cedula == id);
            if (empleado == null)
            {
                return NotFound();
            }
            Empleado = empleado;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Empleado).State = EntityState.Modified;

            try
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    if (ex.ToString().Contains("The database operation was expected to "))
                    {
                        TempData["Mensaje"] = "No se puede cambiar la Cedula de este usuario al tener relacion con otras tablas";
                        return Page();
                    }
                    if (ex.ToString().Contains("FK__empleado__id_dep"))
                    {
                        TempData["Mensaje"] = "No existe Departamento con el Id ingresado";
                        return Page();
                    }
                    if (ex.ToString().Contains("FK__empleado__id_rol"))
                    {
                        TempData["Mensaje"] = "No existe Rol con el Id ingresado";
                        return Page();
                    }
                    if (ex.ToString().Contains("FK__empleado__id_usu"))
                    {
                        TempData["Mensaje"] = "No existe Usuario con el Id ingresado";
                        return Page();
                    }
                    else
                    {
                        TempData["Mensaje"] = ex;
                        return Page();
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmpleadoExists(Empleado.Cedula))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool EmpleadoExists(long id)
        {
            return _context.Empleado.Any(e => e.Cedula == id);
        }
    }
}
