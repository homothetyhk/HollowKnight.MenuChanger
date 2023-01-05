using System.Collections.Concurrent;
using System.Threading;

namespace MenuChanger
{
    public class ThreadSupportException : Exception
    {
        public ThreadSupportException() : base() { }
        public ThreadSupportException(string message) : base(message) { }
        public ThreadSupportException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary>
    /// Utility which helps side threads interact with the main Unity thread.
    /// </summary>
    public class ThreadSupport : MonoBehaviour
    {

        private static readonly ConcurrentQueue<Action> actions = new();
        private static ThreadSupport instance;

        internal static void Setup()
        {
            GameObject go = new();
            instance = go.AddComponent<ThreadSupport>();
            DontDestroyOnLoad(go);
        }

        public void Update()
        {
            while (!actions.IsEmpty && actions.TryDequeue(out Action action))
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    LogError($"Error during ThreadSupport invocation:\n{e}");
                }
            }
        }

        /// <summary>
        /// Enqueues the action to be invoked during the next Unity update.
        /// <br/>If the action raises an exception, the exception is caught and logged.
        /// </summary>
        public static void BeginInvoke(Action a)
        {
            actions.Enqueue(a);
        }

        /// <summary>
        /// Enqueues the action to be invoked during the next Unity update. Blocks the requesting thread until the action has been invoked.
        /// <br/>If the action raises an exception, the exception is caught and rethrown on the requester's thread as a <see cref="ThreadSupportException"/>.
        /// </summary>
        /// <exception cref="ThreadSupportException"></exception>
        public static void BlockUntilInvoked(Action a)
        {
            EventWaitHandle h = new(false, EventResetMode.AutoReset);
            Exception? error = null;
            BeginInvoke(() =>
            {
                try
                {
                    a.Invoke();
                }
                catch (Exception e)
                {
                    error = e;
                }
                h.Set();
            });
            h.WaitOne();
            if (error is not null) 
                throw new ThreadSupportException("An exception was thrown during a ThreadSupport invocation", error);
        }

        /// <summary>
        /// Enqueues the function to be invoked during the next Unity update. Blocks the requesting thread until the function has been invoked, and returns its result.
        /// <br/>If the function raises an exception, the exception is caught and rethrown on the requester's thread as a <see cref="ThreadSupportException"/>.
        /// </summary>
        /// <exception cref="ThreadSupportException"></exception>
        public static T BlockUntilInvoked<T>(Func<T> f)
        {
            EventWaitHandle h = new(false, EventResetMode.AutoReset);
            T result = default;
            Exception? error = null;
            BeginInvoke(() =>
            {
                try
                {
                    result = f.Invoke();
                }
                catch (Exception e)
                {
                    error = e;
                }
                h.Set();
            });
            h.WaitOne();
            if (error is not null) 
                throw new ThreadSupportException("An exception was thrown during a ThreadSupport invocation", error);
            return result;
        }
    }
}
