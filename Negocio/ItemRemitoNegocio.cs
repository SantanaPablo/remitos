using System.Collections.Generic;
using System.Linq;
using Dominio;
using Microsoft.EntityFrameworkCore;

namespace Negocio
{
    public class ItemRemitoNegocio
    {
        private readonly AppDbContext _context;

        public ItemRemitoNegocio(AppDbContext context)
        {
            _context = context;
        }

        public List<ItemRemito> ObtenerTodos()
        {
            return _context.ItemsRemito.Include(i => i.Remito).ToList();
        }

        public ItemRemito ObtenerPorId(int id)
        {
            return _context.ItemsRemito.Include(i => i.Remito)
                                       .FirstOrDefault(i => i.Id == id);
        }

        public void Crear(ItemRemito item)
        {
            _context.ItemsRemito.Add(item);
            _context.SaveChanges();
        }

        public void Actualizar(ItemRemito item)
        {
            _context.ItemsRemito.Update(item);
            _context.SaveChanges();
        }

        public void Eliminar(int id)
        {
            var item = _context.ItemsRemito.Find(id);
            if (item != null)
            {
                _context.ItemsRemito.Remove(item);
                _context.SaveChanges();
            }
        }

        public List<ItemRemito> ObtenerPorRemitoId(int remitoId)
        {
            return _context.ItemsRemito
                .Where(i => i.id_remito == remitoId)
                .ToList();
        }
    }
}
