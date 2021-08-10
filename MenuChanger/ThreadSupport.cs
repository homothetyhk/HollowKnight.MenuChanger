using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Text;
using UnityEngine;

namespace MenuChanger
{
    public static class ThreadSupport
    {
        private static NonBouncer nb;

        internal static void Setup()
        {
            GameObject go = new GameObject();
            nb = go.AddComponent<NonBouncer>();
            GameObject.DontDestroyOnLoad(go);
        }

        private static IEnumerator Invoke(Action a)
        {
            yield return null;
            a?.Invoke();
        }

        public static void BeginInvoke(Action a)
        {
            nb.StartCoroutine(Invoke(a));
        }
    }
}
