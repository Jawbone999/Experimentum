using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public Camera cam;

    Func<Vector3> GetCamFollowPosition;

    public float cameraZoom;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.nearClipPlane = cameraZoom;
    }

    public void Setup(Func<Vector3> GetCamFollowPosition)
    {
        this.GetCamFollowPosition = GetCamFollowPosition;
    }

    void Update()
    {
        transform.position = GetCamFollowPosition() + (Vector3.forward * cameraZoom);
    }
}
