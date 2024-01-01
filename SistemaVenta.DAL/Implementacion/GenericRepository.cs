﻿using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.BDContext;
using SistemaVenta.DAL.Interfaces;

namespace SistemaVenta.DAL.Implementacion
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        public readonly DbventaContext _dbContext;

        public GenericRepository(DbventaContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IQueryable<TEntity>> Consultar(Expression<Func<TEntity, bool>> filtro = null)
        {
            IQueryable<TEntity> queryEntidad = filtro == null? _dbContext.Set<TEntity>() : _dbContext.Set<TEntity>().Where(filtro);
            return queryEntidad;
        }

        public async Task<TEntity> Crear(TEntity entidad)
        {
            try
            {
                _dbContext.Set<TEntity>().Add(entidad);
                await _dbContext.SaveChangesAsync();
                return entidad;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Editar(TEntity entidad)
        {
            try
            {
                _dbContext.Set<TEntity>().Update(entidad);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(TEntity entidad)
        {
            try
            {
                _dbContext.Set<TEntity>().Remove(entidad);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                throw;
            }
        }

        public async Task<TEntity> Obtener(Expression<Func<TEntity, bool>> filtro)
        {
            try
            {
                TEntity entidad = await _dbContext.Set<TEntity>().FirstOrDefaultAsync(filtro);
                return entidad;
            }
            catch
            {
                throw;
            }
        }
    }
}
