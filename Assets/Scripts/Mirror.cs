using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    public Camera TrackingCamera;
    public Camera ReflectionCamera;
    public Transform Specular;
    public float Size = 10;

    void Update()
    {
        Specular.localScale = new Vector3(-Size, Size, 1);

        var diff = transform.position - TrackingCamera.transform.position;
        var normal = transform.forward;
        var reflection = diff + 2 * (Vector3.Dot(-diff, normal)) * normal;
        ReflectionCamera.transform.position = transform.position - reflection;

        ReflectionCamera.transform.LookAt(Specular.position);

        var distance = Vector3.Distance(transform.position, ReflectionCamera.transform.position);
        ReflectionCamera.nearClipPlane = distance * 0.9f;

        ReflectionCamera.fieldOfView = 2 * Mathf.Atan(Size / (2 * distance)) * Mathf.Rad2Deg;

        Specular.LookAt(diff);
    }
}
