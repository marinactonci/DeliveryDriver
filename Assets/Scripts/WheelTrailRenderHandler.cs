using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelTrailRenderHandler : MonoBehaviour
{
    Driver driver;
    TrailRenderer trailRenderer;

    void Awake() {
        driver = GetComponentInParent<Driver>();
        trailRenderer = GetComponent<TrailRenderer>();

        trailRenderer.emitting = false;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if(driver.isTireScreething(out float lateralVelocity, out bool isBraking)){
            trailRenderer.emitting = true;
        }
        else{
            trailRenderer.emitting = false;
        }
    }
}
