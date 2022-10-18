using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class VibrationManager : MonoBehaviour
{
    [SerializeField] ActionBasedController _leftHand;
    [SerializeField] ActionBasedController _rightHand;

    //singleton
    public static VibrationManager Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void VibrateTowardsDirection(Vector3 direction)
    {
        StartCoroutine(VibrateTowards(direction, _leftHand));
        StartCoroutine(VibrateTowards(direction, _rightHand));
    }

    public void StopVibrating()
    {
        StopAllCoroutines();
    }

    IEnumerator VibrateTowards(Vector3 pointToReach, ActionBasedController controller)
    {
        while (true)
        {
            Vector3 direction = pointToReach - controller.transform.position;
            float angle = (Mathf.Atan2(direction.z, direction.x) - Mathf.Atan2(controller.transform.forward.z, controller.transform.forward.x)) * Mathf.Rad2Deg;
            float distance = direction.magnitude;
            float amplitude = Mathf.Max(0.1f, 1 - distance / 10f);
            float duration = Mathf.Max(0.1f, 1 - Mathf.Abs(angle) / 180);
            controller.SendHapticImpulse(amplitude, duration);
            yield return new WaitForSeconds(1);
        }
    }
}
