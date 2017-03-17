using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace XamlingCore.Portable.Messages.XamlingMessenger
{
    public class XMessenger
    {
        public static XMessenger Default { get; }


        private Dictionary<Type, List<ActionRegistration>> Registrations { get; }
        private ManualResetEvent Event { get; }

        static XMessenger()
        {
            Default = new XMessenger();
        }

        public XMessenger()
        {
            Registrations = new Dictionary<Type, List<ActionRegistration>>();
            Event = new ManualResetEvent(true);
        }

        public void Register<T>(object registrant, Action<object> callback) where T : XMessage
        {
            var registration = new ActionRegistration(registrant, callback);

            var t = typeof(T);
            Event.WaitOne();
            try
            {
                Get(t).Add(registration);
            }
            finally
            {
                Event.Set();
            }
        }

        public void Register<T>(object registrant, Action callback) where T : XMessage
        {
            var registration = new ActionRegistration(registrant, callback);

            var t = typeof(T);
            Event.WaitOne();
            try
            {
                Get(t).Add(registration);
            }
            finally
            {
                Event.Set();
            }
        }

        public bool IsRegistered(object t)
        {
            return Registrations.Select(item => item.Value).Any(v => v.Any(vItem => vItem.Registrant == t));
        }

        private List<ActionRegistration> Get(Type t)
        {
            if (!Registrations.ContainsKey(t))
            {
                Registrations.Add(t, new List<ActionRegistration>());
            }

            var l = Registrations[t];

            if (l != null) return l;

            l = new List<ActionRegistration>();
            Registrations[t] = l;

            return l;
        }

        public void Send<T>(T message) where T : XMessage
        {
            var t = message.GetType();

            if (!Registrations.ContainsKey(t)) //nothing has registered for this message
            {
                return;
            }

            var callList = Registrations[t];

            if (callList == null) return;

            Event.WaitOne();
            try
            {
                //tries to call the message, if it fails then it removes that message subscription. 
                var itemsToRemove = (from item in callList let callResult = item.Action(message) where !callResult select item).ToList();

                foreach (var item in itemsToRemove)
                {
                    callList.Remove(item);
                }
            }
            finally
            {
                Event.Set();
            }
        }

        public void Unregister(object registrant)
        {
            Event.WaitOne();
            try
            {
                var removeList = (from item in Registrations from action in item.Value where action.Registrant == registrant select action).ToList();
                foreach (var item in removeList)
                {
                    var closeItem = item;
                    closeItem.Dispose();
                    foreach (var registration in Registrations.Where(r => r.Value.Contains(closeItem)).ToList())
                    {
                        registration.Value?.Remove(closeItem);
                    }
                }
            }
            finally
            {
                Event.Set();
            }
        }

        protected class ActionRegistration : IDisposable
        {
            public ActionRegistration(object registrant, Action<object> callback)
            {
                CallbackReference = callback;
                Registrant = registrant;
            }

            public ActionRegistration(object registrant, Action callback)
            {
                CallbackReferencePlain = callback;
                Registrant = registrant;
            }

            public bool Action(object message)
            {
                try
                {
                    if (CallbackReference == null && CallbackReferencePlain == null)
                    {
                        return false;
                    }

                    Task.Run(() =>
                    {
                        CallbackReference?.Invoke(message);
                        CallbackReferencePlain?.Invoke();
                    });
                    return true;
                }
                catch
                {
                }
                return false;
            }

            public Action<object> CallbackReference { get; private set; }

            public Action CallbackReferencePlain { get; }

            public object Registrant { get; private set; }

            public void Dispose()
            {
                CallbackReference = null;
                Registrant = null;
            }
        }
    }

    public static class XMessengerRegisterExtension
    {
        public static void Register<T>(this object registrant, Action<object> callback) where T : XMessage
        {
            XMessenger.Default.Register<T>(registrant, callback);
        }

        public static void Register<T>(this object registrant, Action callback) where T : XMessage
        {
            XMessenger.Default.Register<T>(registrant, callback);
        }
    }
}
