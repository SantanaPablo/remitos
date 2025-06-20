using Dominio;
using Microsoft.EntityFrameworkCore;

namespace Negocio
{
    public class NotaSalidaNegocio
    {
        private readonly AppDbContext _context;

        public NotaSalidaNegocio(AppDbContext context)
        {
            _context = context;
        }

        public List<NotaSalida> ObtenerTodos()
        {
            return _context.NotasSalida.Include(n => n.Autorizante).ToList();
        }

        public NotaSalida ObtenerPorId(int id)
        {
            return _context.NotasSalida
                .Include(n => n.Autorizante)
                .Include(n => n.Items)
                .FirstOrDefault(n => n.Id == id);
        }

        public void Crear(NotaSalida nota)
        {
            _context.NotasSalida.Add(nota);
            _context.SaveChanges();
        }

        public void Actualizar(NotaSalida nota)
        {
            _context.NotasSalida.Update(nota);
            _context.SaveChanges();
        }

        public void Eliminar(int id)
        {
            var nota = _context.NotasSalida.Find(id);
            if (nota != null)
            {
                _context.NotasSalida.Remove(nota);
                _context.SaveChanges();
            }
        }
    }
}

