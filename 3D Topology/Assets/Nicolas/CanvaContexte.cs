using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvaContexte : MonoBehaviour
{
    public Slider sliderLenght;
    public Text txtLenght;

    public Slider sliderStart;
    public Text txtStart;


    // Start is called before the first frame update
    void Start()
    {
        sliderStart.onValueChanged.AddListener(delegate { UpdateCanva(); });
        sliderLenght.onValueChanged.AddListener(delegate { UpdateCanva(); });
        UpdateCanva();
    }

    private void UpdateCanva()
    {
        txtLenght.text = sliderLenght.value.ToString() + " min";
        txtStart.text = sliderStart.value.ToString() + " min";
    }
}
