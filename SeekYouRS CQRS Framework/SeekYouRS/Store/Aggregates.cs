﻿using System;
using System.Linq;

namespace SeekYouRS.Store
{
	public sealed class Aggregates : IStoreAggregates
	{
		readonly IAmAnAggregatesUnitOfWork _unitOfWork;

		public event Action<AggregateEvent> AggregateHasChanged;

		public Aggregates(IAmAnAggregatesUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public void Save<TAggregate>(TAggregate aggregate) where TAggregate : Aggregate
		{
			_unitOfWork.Save(aggregate.Changes);

			if (AggregateHasChanged != null)
				foreach (var e in aggregate.Changes)
					AggregateHasChanged(e);

			aggregate.History = aggregate.History.Concat(aggregate.Changes).ToList();
			aggregate.Changes.Clear();
		}

		public TAggregate GetAggregate<TAggregate>(Guid id) where TAggregate : Aggregate, new()
		{
			var aggregateHistory = _unitOfWork.GetEventsBy(id).ToList();
			var aggregate = new TAggregate
				{
					History = aggregateHistory
				};
			return aggregate;
		}
	}
}