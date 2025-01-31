﻿using CleanArchitecture.Application.Abstractions.Repositories;
using CleanArchitecture.Core.Abstractions.Entities;
using MockQueryable.Moq;

namespace CleanArchitecture.Core.Tests.Factories
{
    public static class MockRepositoryFactory
    {
        public static Mock<TRepository> Create<T, TRepository>(IEnumerable<T>? items = null)
            where T : AggregateRoot
            where TRepository : class, IRepository<T>
        {
            Mock<TRepository> repository = new();
            return Setup(repository, items);
        }

        public static Mock<IRepository<T>> Create<T>(IEnumerable<T>? items = null)
            where T : AggregateRoot
        {
            Mock<IRepository<T>> repository = new();
            return Setup(repository, items);
        }

        public static Mock<TRepository> Setup<T, TRepository>(Mock<TRepository> repository,
            IEnumerable<T>? items = null)
            where T : AggregateRoot
            where TRepository : class, IRepository<T>
        {
            if (items == null)
            {
                items = new List<T>();
            }

            repository.Setup(e => e.GetByIdAsync(It.IsAny<Guid>()))
                .Returns<Guid>(id => Task.FromResult(items.FirstOrDefault(e => e.Id == id)));
            repository.Setup(e => e.GetAll(It.IsAny<bool>()))
                .Returns(() => items.AsQueryable().BuildMockDbSet().Object);
            return repository;
        }
    }
}