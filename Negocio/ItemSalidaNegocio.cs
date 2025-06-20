using Dominio;
using Microsoft.EntityFrameworkCore;

namespace Negocio
{
    public class ItemSalidaNegocio
    {
        private readonly AppDbContext _context;

        public ItemSalidaNegocio(AppDbContext context)
        {
            _context = context;
        }

        public List<ItemSalida> ObtenerTodos()
        {
            return _context.ItemsSalida.Include(i => i.NotaSalida).ToList();
        }

        public ItemSalida ObtenerPorId(int id)
        {
            return _context.ItemsSalida.Include(i => i.NotaSalida)
                                       .FirstOrDefault(i => i.Id == id);
        }

        public void Crear(ItemSalida item)
        {
            _context.ItemsSalida.Add(item);
            _context.SaveChanges();
        }

        public void Actualizar(ItemSalida item)
        {
            _context.ItemsSalida.Update(item);
            _context.SaveChanges();
        }

        public void Eliminar(int id)
        {
            var item = _context.ItemsSalida.Find(id);
            if (item != null)
            {
                _context.ItemsSalida.Remove(item);
                _context.SaveChanges();
            }
        }

        public List<ItemSalida> ObtenerPorNotaSalidaId(int notaId)
        {
            return _context.ItemsSalida
                .Where(i => i.NotaSalidaId == notaId)
                .ToList();
        }
    }
}
