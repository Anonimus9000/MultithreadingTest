using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = System.Random;

namespace TaskManager
{
    public class Benchmark : MonoBehaviour
    {
        [SerializeField] private int iterationCount;
        [SerializeField] private TextMeshProUGUI mainThreadTime;
        [SerializeField] private TextMeshProUGUI otherThreadText;

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(1);
            MainThreadBench();
            OtherThreadBench();
        }

        private void OtherThreadBench()
        {
            TaskController.InvokeAsync(BenchmarkGetTime,
                benchTime =>
                {
                    Debug.Log("time = " + benchTime);
                    otherThreadText.text = benchTime.ToString();
                });
        }
        
        private long BenchmarkGetTime()
        {
            var stopwatch = new Stopwatch(); 
            stopwatch.Start();
            var result = 0;
            var random = new Random();
            for (var i = 0; i < iterationCount; i++)
            {
                result += random.Next(-100, 100);
            }

            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }

        private void MainThreadBench()
        {
            var benchmarkGetTime = BenchmarkGetTime();
            Debug.Log("time = " + benchmarkGetTime);
            mainThreadTime.text = benchmarkGetTime.ToString();
        }
    }
}
