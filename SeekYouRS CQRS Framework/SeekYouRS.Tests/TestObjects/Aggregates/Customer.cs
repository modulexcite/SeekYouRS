using System;
using System.Threading;
using SeekYouRS.BaseComponents;
using SeekYouRS.Tests.TestObjects.Events;

namespace SeekYouRS.Tests.TestObjects.Aggregates
{
	internal class Customer : Aggregate
	{
		public string Name
		{
			get
			{
				var removed = FromHistory<CustomerRemoved>();
				if (removed != null)
					return null;

				var lastChange = FromHistory<CustomerChanged>();
				
				return lastChange != null 
					? lastChange.Name 
					: FromHistory<CustomerCreated>().Name;
			}
		}

		public override Guid Id {
			get
			{
				var removed = FromHistory<CustomerRemoved>();
				return removed != null ? Guid.Empty : FromHistory<CustomerCreated>().Id;
			}
		}

		public void Create(Guid id, string name)
		{
			ApplyChanges(new CustomerCreated { Id = id, Name = name });
		}

		public void Change(string neuerName)
		{
			ApplyChanges(new CustomerChanged { Id = this.Id, Name = neuerName });
		}

		public void Remove()
		{
			ApplyChanges(new CustomerRemoved{Id = this.Id});
		}

		public void RaiseUnhandledEvent()
		{
			ApplyChanges(new UnhandlesEventRaised());
		}

		public String GetStringResult(string expectedResult)
		{
			return expectedResult;
		}

		public int GetIntResult(int expectedResult)
		{
			return expectedResult;
		}

		public void StartLongRunningProcess(int milliseconds)
		{
			Thread.Sleep(milliseconds);
		}
	}
}