using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARManager : MonoBehaviour
{
    public ARSession arSession;
    public ARCameraManager arCamManager;

    public void StopARCamera()
    {
        if (arCamManager != null && arCamManager.enabled)
        {
            arCamManager.enabled = false;
            Debug.Log("AR Camera Stop");
        }

        if (arSession != null && arSession.enabled)
        {
            arSession.Reset();
            arSession.enabled = false;
            Debug.Log("AR Session Stop");
        }
    }
}
