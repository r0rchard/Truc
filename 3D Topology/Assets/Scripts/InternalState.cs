using EnvironmentGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InternalState : MonoBehaviour
{
    [SerializeField] GameObject _memoryColumn;
    [SerializeField] GameObject _CPUColumn;
    [SerializeField] GameObject _diskColumn;
    [SerializeField] GameObject _networkColumn;
    [SerializeField] HostNode _host;

    float _memoryUsage = 0;
    public float MemoryUsage
    {
        get => _memoryUsage;
        set
        {
            _memoryUsage = value;
        }
    }
    float _CPUUsage = 0;
    public float CPUUsage
    {
        get => _CPUUsage;
        set
        {
            _CPUUsage = value;
        }
    }
    float _diskUsage = 0;
    public float DiskUsage
    {
        get => _diskUsage;
        set
        {
            _diskUsage = value;
        }
    }
    float _networkUsage = 0;
    public float NetworkUsage
    {
        get => _networkUsage;
        set
        {
            _networkUsage = value;
        }
    }

    public string Identifier
    {
        get=> _host.Identifier;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PositionColumn(GameObject column, float value)
    {
        column.transform.localScale = new Vector3(1, value, 1);
        column.transform.position = new Vector3(column.transform.position.x, value / 2f, column.transform.position.z);
    }

    public void PositionColumns()
    {
        PositionColumn(_memoryColumn, _memoryUsage);
        PositionColumn(_diskColumn, _diskUsage);
        PositionColumn(_networkColumn, _networkUsage);
        PositionColumn(_CPUColumn, _CPUUsage);
    }
}
