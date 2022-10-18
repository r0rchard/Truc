using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToolSelector : MonoBehaviour
{
    [SerializeField] List<MiniTool> _miniTools;
    MiniTool _selectedTool;
    [SerializeField] InputActionReference _selectToolActionReference;


    // Start is called before the first frame update
    void Start()
    {
        _selectToolActionReference.action.performed += MakeMiniToolsAppear;
        _selectToolActionReference.action.canceled += SelectTool;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MakeMiniToolsAppear(InputAction.CallbackContext obj)
    {
        for(int i = 0; i < _miniTools.Count; i++)
        {
            MiniTool miniTool = _miniTools[i];
            float angle = (180f - i * 180f / _miniTools.Count) * Mathf.Deg2Rad;
            miniTool.transform.position = transform.position + 0.1f * (Mathf.Cos(angle) * transform.right + Mathf.Sin(angle) * transform.forward);
            miniTool.transform.parent = null;
            miniTool.transform.forward = transform.forward;
            miniTool.gameObject.SetActive(true);
        }
    }

    void SelectTool(InputAction.CallbackContext obj)
    {
        if (_selectedTool)
        {
            for (int i = 0; i <_miniTools.Count; i++)
            {
                MiniTool miniTool = _miniTools[i];
                if (_selectedTool == miniTool)
                {
                    miniTool.AssociatedTool.gameObject.SetActive(true);
                    GetComponentInChildren<DummyToolSelector>().NetworkSelectTool(i);
                }
                else
                {
                    miniTool.AssociatedTool.gameObject.SetActive(false);
                }
            }
        }
        foreach(MiniTool miniTool in _miniTools)
        {
            miniTool.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        MiniTool miniTool = other.GetComponent<MiniTool>();
        if (miniTool)
        {
            miniTool.StartRotating();
            if (_selectedTool)
            {
                _selectedTool.StartDecreasing();
            }
            _selectedTool = miniTool;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        MiniTool miniTool = other.GetComponent<MiniTool>();
        if (miniTool)
        {
            miniTool.StartDecreasing();
            if(miniTool == _selectedTool)
            {
                _selectedTool = null;
            }
        }
    }
}
