using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorScheme : MonoBehaviour
{
    public List<Color> Colors;

    //singleton
    public static ColorScheme Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;
    }

    public Color GetPlayerColor(int actorNumber)
    {
        return Colors[actorNumber - 1 % Colors.Count];
    }
}
