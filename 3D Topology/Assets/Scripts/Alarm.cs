using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm : MonoBehaviour
{
    [SerializeField] GameObject _lightHolder;
    AudioSource _source;
    float _initialHeight;

    // Start is called before the first frame update
    void Start()
    {
        _source = GetComponent<AudioSource>();
        _initialHeight = transform.position.y;
        StartCoroutine(RingAlarm());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator RingAlarm()
    {
        _source.Play();
        float ellapsedTime = 0f;
        while (ellapsedTime < 5f)
        {
            ellapsedTime += Time.deltaTime;
            if (transform.position.y > _initialHeight - 0.2f)
            {
                transform.position -= new Vector3(0, Time.deltaTime, 0);
            }
            if (_source.volume < 1)
            {
                _source.volume += Time.deltaTime / 4f;
            }
            _lightHolder.transform.Rotate(new Vector3(0, 1, 0), Time.deltaTime * 90f, Space.Self);
            yield return null;
        }
    }
}
