using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightBulb : MonoBehaviour
{
    [SerializeField] Light _light;
    public Color Color
    {
        get => _light.color;
        set => _light.color = value;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        StartCoroutine(Shine());
    }

    IEnumerator Shine()
    {
        _light.intensity = 0;
        while (_light.intensity < 1)
        {
            _light.intensity += Time.deltaTime;
            yield return null;
        }
        while (_light.intensity > Mathf.Epsilon)
        {
            _light.intensity -= Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
