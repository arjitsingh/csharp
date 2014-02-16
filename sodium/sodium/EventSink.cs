namespace sodium
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class EventSink<TEvent> : Event<TEvent>
    {
        public void Send(TEvent evt)
        {
            Transaction.Run(new EventSinkRunner<TEvent>(this, evt));
        }

        public void Send(Transaction transaction, TEvent evt)
        {
            if (!Firings.Any())
                transaction.Last(new Runnable(() => Firings.Clear()));
            Firings.Add(evt);

            var listeners = new List<ITransactionHandler<TEvent>>(this.Listeners);

            foreach (var action in listeners)
            {
                try
                {
                    action.Run(transaction, evt);
                }
                catch (Exception t)
                {
                    Console.WriteLine("{0}", t);
                }
            }
        }
    }
}