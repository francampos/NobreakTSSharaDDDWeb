using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NobreakTSSharaDDDWeb.Domain.Entities;
using NobreakTSSharaDDDWeb.Domain.Interface;
using NobreakTSSharaDDDWeb.Domain.Interface.Services;

namespace NobreakTSSharaDDDWeb.Domain.Services
{
    public class NobreakService : INobreakService
    {
        private readonly INobreakRepository _nobreakRepository;

        public NobreakService(INobreakRepository nobreakRepository)
        {
            _nobreakRepository = nobreakRepository;
        }

        public Nobreak Adicionar(Nobreak entity)
        {
            return _nobreakRepository.Adicionar(entity);
        }

        public void Atualizar(Nobreak entity)
        {
            _nobreakRepository.Atualizar(entity);
        }

        public IEnumerable<Nobreak> Buscar(Expression<Func<Nobreak, bool>> predicado)
        {
            return _nobreakRepository.Buscar(predicado);
        }

        public Nobreak ObterPorId(int id)
        {
            return _nobreakRepository.ObterPorId(id);
        }

        public IEnumerable<Nobreak> ObterTodos()
        {
            return _nobreakRepository.ObterTodos();
        }

        public void Remover(Nobreak entity)
        {
            _nobreakRepository.Remover(entity);
        }
    }
}
