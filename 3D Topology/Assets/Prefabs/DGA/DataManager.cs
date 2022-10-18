using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Parsing;

public class DataManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        JSONReader.Instance.GenerateEnvironment();
    }
}
