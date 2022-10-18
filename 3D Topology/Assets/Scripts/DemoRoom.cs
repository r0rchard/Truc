using ModelSWaT;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DemoRoom : MonoBehaviour
{
    Sensor _sensor;
    [SerializeField] Transform _spawnPoint;
    [SerializeField] DemoRoomDoor _door;
    [SerializeField] Alarm _alarm1;
    [SerializeField] Alarm _alarm2;
    [SerializeField] Image _blackScreen;

    public DemoRoomDoor Door
    {
        get => _door;
    }
    public Transform SpawnPoint
    {
        get => _spawnPoint;
    }

    // Start is called before the first frame update
    void Start()
    {
        float[] data = new float[33000];
        for (int i = 0; i < data.Length; i++)
        {
            if (i % 2800 == 0 && i != 0)
            {
                for (int j = 0; j < 50; j++)
                {
                    data[i - j] = Random.Range(5f, 6f);
                }
            }
            else
            {
                data[i] = Random.Range(0f, 1f);
            }
        }
        _sensor = new Sensor(name, data);

        if (PhotonNetwork.IsMasterClient)
        {
            baseHelice helix = PhotonNetwork.InstantiateRoomObject("NetworkHelix", transform.position + Vector3.up, Quaternion.Euler(0, 180, 0)).GetComponentInChildren<baseHelice>();
            helix.sensor = _sensor;
        }
        else
        {
            StartCoroutine(WaitForHelix());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitForHelix()
    {
        baseHelice helix = FindObjectOfType<baseHelice>();
        while(helix == null)
        {
            yield return null;
            helix = FindObjectOfType<baseHelice>();
        }
        Debug.Log(_sensor.Values);
        helix.sensor = _sensor;
    }

    public void Alarm()
    {
        _alarm1.gameObject.SetActive(true);
        _alarm2.gameObject.SetActive(true);
        StartCoroutine(BlackScreen());
    }

    IEnumerator BlackScreen()
    {
        yield return new WaitForSeconds(4);
        while (_blackScreen.color.a < 1)
        {
            _blackScreen.color = new Color(0, 0, 0, _blackScreen.color.a + Time.deltaTime / 3f);
            yield return null;
        }
    }
}
