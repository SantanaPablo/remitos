using System.Collections.Generic;
using System.Linq;
using Dominio;
using Microsoft.EntityFrameworkCore;

namespace Negocio
{
    public class RemitoNegocio
    {
        private readonly AppDbContext _context;

        public RemitoNegocio(AppDbContext context)
        {
            _context = context;
        }

        public List<Remito> ObtenerTodos()
        {
            return _context.Remitos.Include(r => r.Items).ToList();
        }

        public Remito ObtenerPorId(int id)
        {
            return _context.Remitos.Include(r => r.Items)
                                   .FirstOrDefault(r => r.Id == id);
        }

        public void Crear(Remito remito)
        {
            _context.Remitos.Add(remito);
            _context.SaveChanges();
        }

        public void Actualizar(Remito remito)
        {
            _context.Remitos.Update(remito);
            _context.SaveChanges();
        }

        public void Eliminar(int id)
        {
            var remito = _context.Remitos.Find(id);
            if (remito != null)
            {
                _context.Remitos.Remove(remito);
                _context.SaveChanges();
            }
        }


    }
}

