using System;
using System.Collections;
using UnityEngine;

public class Zoomer : MonoBehaviour
{
    [Range(1, 100)] [SerializeField] private float zoomAmount;
    [SerializeField] private float cameraMoveFactor;
    [SerializeField] private float EndEffectorRepresentationScaleFactor;
    [Tooltip("Leave at 0 to keep the end effector touching the terrain")]
    [SerializeField] private float EndEffectorRepresentationYMoveFactor;

    [SerializeField] private KeyCode zoomInKey;
    [SerializeField] private KeyCode zoomOutKey;

    private int zoomDirection = 1;
    private bool isZooming;
    private Coroutine zoomCoroutine;
    
    private void OnEnable()
    {
        InputHandler.Instance.OnKeyDownEvent += OnKeyPress;
        InputHandler.Instance.OnKeyUpEvent += OnKeyRelease;
    }

    private void OnDisable()
    {
        InputHandler.Instance.OnKeyDownEvent -= OnKeyPress;
        InputHandler.Instance.OnKeyUpEvent -= OnKeyRelease;
    }

    private void OnKeyPress(KeyCode key)
    {
        if (isZooming) return;
        if (key == zoomInKey)
        {
            zoomDirection = 1;
        }
        else if (key == zoomOutKey)
        {
            zoomDirection = -1;
        }
        zoomCoroutine = StartCoroutine(ZoomCoroutine());
        isZooming = true;
    }

    private void OnKeyRelease(KeyCode key)
    {
        if (!isZooming) return;
        StopCoroutine(zoomCoroutine);
        isZooming = false;
    }

    private IEnumerator ZoomCoroutine()
    {
        while (true)
        {
            Zoom();
            yield return null;
        }
    }

    private void Zoom()
    {
        //Disable End Effector Collider
        //Move Camera 
        //Scale End Effector
        //Move End Effector to appropriate Y value
    }
}
