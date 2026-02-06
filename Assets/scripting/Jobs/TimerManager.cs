using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Burst;
/// <summary>
/// Besoin de Timers et Chronos ? De creer des coolDown, des temps de Cast? Bref manipuler le temps... c'est ce que fait ce script. 
/// </summary>
public class TimerManager : MonoBehaviour
{
    public static TimerManager Instance;
    private const int MAX_TIMERS = 1024;
    
    private NativeArray<float> _times;
    private NativeArray<float> _directions;
    private NativeArray<bool> _isRunning;
    
    private System.Collections.Generic.Stack<int> _pool = new System.Collections.Generic.Stack<int>();
    private JobHandle _handle;

    void Awake()
    {
        Instance = this;
        _times = new NativeArray<float>(MAX_TIMERS, Allocator.Persistent);
        _directions = new NativeArray<float>(MAX_TIMERS, Allocator.Persistent);
        _isRunning = new NativeArray<bool>(MAX_TIMERS, Allocator.Persistent);

        for (int i = MAX_TIMERS - 1; i >= 0; i--) _pool.Push(i);
    }

    public static int Create(float duration, bool isCountdown = true)
    {
        if (Instance._pool.Count == 0) return -1;
        int id = Instance._pool.Pop();
        Instance._times[id] = duration;
        Instance._directions[id] = isCountdown ? -1f : 1f;
        Instance._isRunning[id] = true;
        return id;
    }

    public static float GetValue(int id) => (id >= 0) ? Instance._times[id] : 0;

    [BurstCompile]
    struct TimerJob : IJobParallelFor
    {
        public float dt;
        public NativeArray<float> times;
        public NativeArray<bool> running;
        [ReadOnly] public NativeArray<float> dirs;

        public void Execute(int i)
        {
            if (!running[i]) return;

            float next = times[i] + (dt * dirs[i]);

            // Si minuteur et qu'on touche 0
            if (dirs[i] < 0 && next <= 0)
            {
                times[i] = 0;
                running[i] = false; // Auto-stop
            }
            else
            {
                times[i] = next;
            }
        }
    }

    void Update()
    {
        _handle = new TimerJob {
            dt = Time.deltaTime,
            times = _times,
            running = _isRunning,
            dirs = _directions
        }.Schedule(MAX_TIMERS, 64);
    }

    void LateUpdate() => _handle.Complete();

    void OnDestroy()
    {
        if (_times.IsCreated) _times.Dispose();
        if (_directions.IsCreated) _directions.Dispose();
        if (_isRunning.IsCreated) _isRunning.Dispose();
    }
}