using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskManager
{
    public static class TaskController
    {
        public static async void InvokeAsync<T>(
            Func<T> otherThreadCallback, 
            Action<T> mainThreadCallback = null,
            Func<bool> otherThreadCallbackCondition = null, 
            Func<bool> mainThreadCallbackCondition = null,
            CancellationToken token = default)
        {
            var invokeAsync = InvokeAsync(otherThreadCallback, otherThreadCallbackCondition, token);

            while (!invokeAsync.IsCompleted)
            {
                if (invokeAsync.IsFaulted)
                {
                    throw new Exception("Thread is faulted | id: " + invokeAsync.Id);
                }
                
                token.ThrowIfCancellationRequested();
                await Task.Yield();
            }

            if (mainThreadCallback != null)
            {
                token.ThrowIfCancellationRequested();
                await InvokeInMainThread<T>(() => mainThreadCallback(invokeAsync.Result), mainThreadCallbackCondition, token);
            }
        }

        public static async void InvokeAsync(Action callbackInOtherThread, Action callbackInUnityThread = null, 
            CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();
            var task = Task.Run(callbackInOtherThread, token);

            while (!task.IsCompleted)
            {
                token.ThrowIfCancellationRequested();
                await Task.Yield();
            }
            
            Dispatcher.InvokeInMainThread(callbackInUnityThread);
        }
        
        private static async Task InvokeInMainThread<T>(Action action, Func<bool> condition = null, 
            CancellationToken token = default)
        {
            while (condition != null && !condition.Invoke())
            {
                token.ThrowIfCancellationRequested();
                await Task.Yield();
            }
            
            Dispatcher.Invoke<T>(() => action?.Invoke());
        }

        private static async Task<T> InvokeAsync<T>(Func<T> callback, Func<bool> condition = null,
            CancellationToken token = default)
        {
            while (condition != null && !condition.Invoke())
            {
                token.ThrowIfCancellationRequested();
                await Task.Yield();
            }

            var task = Task.Run(callback.Invoke, token);
            
            token.ThrowIfCancellationRequested();
            return await task;
        }
    }
}
