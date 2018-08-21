using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NobreakTSSharaDDDWeb.Domain.Entities;

namespace NobreakTSSharaDDDWeb.Domain.Interface.Services
{
    public interface INobreakService
    {
        Nobreak Adicionar(Nobreak entity);
        void Atualizar(Nobreak entity);
        IEnumerable<Nobreak> ObterTodos();
        Nobreak ObterPorId(int id);
        IEnumerable<Nobreak> Buscar(Expression<Func<Nobreak, bool>> predicado);
        void Remover(Nobreak entity);
    }
}
