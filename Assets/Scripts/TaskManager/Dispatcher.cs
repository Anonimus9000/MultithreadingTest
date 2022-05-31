using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TaskManager
{
    public class Dispatcher : MonoBehaviour
    {
        private static readonly Queue<Action> Actions = new Queue<Action>();

        private void Update()
        {
            if(Actions.Count > 0)
            {
                Actions.Dequeue().Invoke();
            }
        }

        public IEnumerator InvokeCoroutine(IEnumerator coroutine)
        {
            yield return coroutine;
        }

        public static void Invoke<T>(Action callback)
        {
            Actions.Enqueue(callback);
        }

        public static void InvokeInMainThread(Action callback)
        {
            Actions.Enqueue(callback);
        }

        public static void Log(string message, string stacktrace = "")
        {
            Actions.Enqueue(() => Debug.Log(message + "\n" + stacktrace));
        }
    }
}
