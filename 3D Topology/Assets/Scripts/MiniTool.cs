using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniTool : MonoBehaviour
{
    [SerializeField] InteractionTool _associatedTool;
    Vector3 _initialScale;
    public InteractionTool AssociatedTool
    {
        get => _associatedTool;
    }

    // Start is called before the first frame update
    void Start()
    {
        _initialScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartRotating()
    {
        StartCoroutine(IncreaseAndRotate());
    }

    IEnumerator IncreaseAndRotate()
    {
        bool shouldIncrease = true;
        float ellapsedTime = 0;
        while (true)
        {
            if (shouldIncrease)
            {
                transform.localScale += Time.deltaTime * _initialScale.x * Vector3.one;
                ellapsedTime += Time.deltaTime;
                if(transform.localScale.x > 2 * _initialScale.x)
                {
                    transform.localScale = 2 * _initialScale;
                    Debug.Log(ellapsedTime);
                    shouldIncrease = false;
                }
            }
            transform.Rotate(Vector3.up, Time.deltaTime * 180, Space.Self);
            yield return null;
        }
    }

    public void StartDecreasing()
    {
        StopAllCoroutines();
        StartCoroutine(Decrease());
    }

    IEnumerator Decrease()
    {
        while (transform.localScale.x > _initialScale.x)
        {
            transform.localScale -= Time.deltaTime * Vector3.one;
            yield return null;
        }
        transform.localScale = _initialScale;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        transform.localScale = _initialScale;
    }
}
