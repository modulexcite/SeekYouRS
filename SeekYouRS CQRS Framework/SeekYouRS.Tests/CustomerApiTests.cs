using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SeekYouRS.Tests.TestObjects;
using SeekYouRS.Tests.TestObjects.Commands;
using SeekYouRS.Tests.TestObjects.Events;
using SeekYouRS.Tests.TestObjects.Handler;
using SeekYouRS.Tests.TestObjects.Models;
using SeekYouRS.Tests.TestObjects.Queries;

namespace SeekYouRS.Tests
{
    [TestFixture]
    internal class CustomerApiTests
    {
        [Test]
        public void TestToCreateCustomerAndReadIt()
        {
            var aggregateStore = new InMemoryAggregateStore();
            var readModel = new InMemoryReadModel();

            var commands = new CustomerCommands(aggregateStore);
            var queries = new CustomerQueries(readModel);

            var api = new CustomerContext(commands, queries);
            var id = Guid.NewGuid();

            api.Process(new CreateCustomer{Id = id, Name = "My Customer"});

            var customerModel = api.Retrieve<CustomerModel>(new GetCustomer {Id = id});

            customerModel.Name.ShouldBeEquivalentTo("My Customer");
        }

        [Test]
        public void TestToCreateAndChangeKundeViaApi()
        {
            var aggregateStore = new InMemoryAggregateStore();
            var queriesStore = new InMemoryReadModel();

            var reciever = new CustomerCommands(aggregateStore);
            var queries = new CustomerQueries(queriesStore);

            var api = new CustomerContext(reciever, queries);
            var id = Guid.NewGuid();

            api.Process(new CreateCustomer { Id = id, Name = "My Customer" });
            api.Process(new ChangeCustomer { Id = id, Name = "New Name" });

            var customer = api.Retrieve<CustomerModel>(new GetCustomer
                {
                    Id = id
                });

            customer.Name.ShouldBeEquivalentTo("New Name");
        }

        [Test]
        public void TestToCreateTwoCustomerAndChangeOneOfThem()
        {
            var aggregateStore = new InMemoryAggregateStore();
            var modelsStore = new InMemoryReadModel();

            var reciever = new CustomerCommands(aggregateStore);
            var queries = new CustomerQueries(modelsStore);

            var api = new CustomerContext(reciever, queries);

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            api.Process(new CreateCustomer { Id = id1, Name = "Customer One" });
            api.Process(new CreateCustomer { Id = id2, Name = "Customer Two" });

            api.Process(new ChangeCustomer { Id = id2, Name = "Customer Two Changed" });

            var customer = api.Retrieve<CustomerModel>(new GetCustomer
            {
                Id = id2
            });

            customer.Name.ShouldBeEquivalentTo("Customer Two Changed");

        }

        [Test]
        public void TestToRemoveACustomer()
        {
            var aggregateStore = new InMemoryAggregateStore();
            var modelsStore = new InMemoryReadModel();

            var reciever = new CustomerCommands(aggregateStore);
            var queries = new CustomerQueries(modelsStore);

            var api = new CustomerContext(reciever, queries);

            var id = Guid.NewGuid();

            api.Process(new CreateCustomer { Id = id, Name = "Customer To Remove" });

            var customer = api.Retrieve<CustomerModel>(new GetCustomer
            {
                Id = id
            });

            customer.Should().NotBeNull();

            api.Process(new RemoveCustomer { Id = id });

            customer = api.Retrieve<CustomerModel>(new GetCustomer
            {
                Id = id
            });

            customer.Should().BeNull();
        }

        [Test]
        public void TestToGetAListOfCustomers()
        {
            var aggregateStore = new InMemoryAggregateStore();
            var modelsStore = new InMemoryReadModel();

            var reciever = new CustomerCommands(aggregateStore);
            var queries = new CustomerQueries(modelsStore);

            var api = new CustomerContext(reciever, queries);

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();

            api.Process(new CreateCustomer { Id = id1, Name = "Customer One" });
            api.Process(new CreateCustomer { Id = id2, Name = "Customer Two" });

            var customers = api.Retrieve<IEnumerable<CustomerModel>>(new GetAllCustomers()).ToList();

            customers.Count().ShouldBeEquivalentTo(2);

            customers.Should().Contain(c => c.Id == id1);
            customers.Should().Contain(c => c.Id == id2);

        }
    }
}