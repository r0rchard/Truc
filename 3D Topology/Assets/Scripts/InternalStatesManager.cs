using Parsing.CyberRangeSerialization;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class InternalStatesManager : MonoBehaviour
{
    //singleton
    public static InternalStatesManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;
    }

    private Dictionary<string, InternalState> _states = new Dictionary<string, InternalState>();
    public Dictionary<string, InternalState> States
    {
        get => _states;
    }

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(UpdateStates());
        StartCoroutine(FakeStates());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator UpdateStates()
    {
        while (true)
        {
            Thread thread = new Thread(delegate ()
            {
                foreach(HostsSpecsAsset hostAsset in CyberRangeInterface.Instance.GetAllHosts())
                {
                    InternalState state;
                    if (_states.TryGetValue(hostAsset.identifier, out state))
                    {
                        state.CPUUsage = hostAsset.metrics.cpu_usage / hostAsset.metrics.cpu_capacity;
                        state.MemoryUsage = hostAsset.metrics.memory_usage / hostAsset.metrics.memory_capacity;
                    }
                }
            });
            thread.Start();
            yield return new WaitUntil(() => !thread.IsAlive);
            foreach(InternalState state in _states.Values)
            {
                if (state.isActiveAndEnabled)
                {
                    state.PositionColumns();
                }
            }
            yield return null;
        }
    }

    IEnumerator FakeStates()
    {
        foreach(InternalState state in _states.Values)
        {
            state.MemoryUsage = Random.Range(1f, 2f);
            state.CPUUsage = Random.Range(1f, 2f);
            state.DiskUsage = Random.Range(1f, 2f);
            state.NetworkUsage = Random.Range(1f, 2f);
        }
        while (true)
        {
            foreach (InternalState state in _states.Values)
            {
                state.MemoryUsage += Random.Range(-Time.deltaTime, Time.deltaTime);
                state.MemoryUsage = Mathf.Max(state.MemoryUsage, 0f);
                state.CPUUsage += Random.Range(-Time.deltaTime, Time.deltaTime);
                state.CPUUsage = Mathf.Max(state.CPUUsage, 0f);
                state.DiskUsage += Random.Range(-Time.deltaTime, Time.deltaTime);
                state.DiskUsage = Mathf.Max(state.DiskUsage, 0f);
                state.NetworkUsage += Random.Range(-Time.deltaTime, Time.deltaTime);
                state.NetworkUsage = Mathf.Max(state.NetworkUsage, 0f);
                state.PositionColumns();
            }
            yield return null;
        }
    }
}
