using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using UnityEngine;
using System.Collections.Concurrent;

namespace MenuChanger
{
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
        /// </summary>
        public static void BeginInvoke(Action a)
        {
            actions.Enqueue(a);
        }
    }
}
