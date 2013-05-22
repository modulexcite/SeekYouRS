﻿using System;
using System.Collections.Generic;
using System.Linq;
using SeekYouRS.EventSource;

namespace SeekYouRS
{
    public abstract class Aggegate
    {
        protected Aggegate()
        {
            Changes = new List<Event>();
        }

        internal Guid Id { get; set; }

        internal IList<Event> Changes{ get; private set; }

        internal IEnumerable<Event> History { get; set; }

        internal void ApplyChanges<T>(T changeEvent) where T : class
        {
            Changes.Add(new EventBag<T>{EventData = changeEvent});
        }

        internal T FromHistory<T>() where T : new ()
        {
            var lastEventOfSearchedType = History.Concat(Changes).OfType<EventBag<T>>().LastOrDefault();
            return lastEventOfSearchedType == null 
                ? new T() 
                : lastEventOfSearchedType.EventData;
        }
    }
}