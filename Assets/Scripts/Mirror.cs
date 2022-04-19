using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirror : MonoBehaviour
{
    public Camera TrackingCamera;
    public Camera ReflectionCamera;
    public Transform Specular;
    public Transform Frame;
    public float Size = 10;

    void Update()
    {
        var diff = transform.position - TrackingCamera.transform.position;
        var normal = transform.forward;
        var reflection = diff + 2 * (Vector3.Dot(-diff, normal)) * normal;
        ReflectionCamera.transform.position = transform.position - reflection;

        ReflectionCamera.transform.LookAt(Specular.position);

        var distance = Vector3.Distance(transform.position, ReflectionCamera.transform.position);
        ReflectionCamera.nearClipPlane = distance * .9f;

        Frame.localScale = new Vector3(Size, Size, 1);
        var angle = Vector3.Angle(-transform.forward, ReflectionCamera.transform.forward);
        var specularSize = Size / Mathf.Abs(Mathf.Cos(angle * Mathf.Deg2Rad)) * 2f;
        Specular.localScale = new Vector3(-specularSize, specularSize, 1);

        ReflectionCamera.fieldOfView = 2 * Mathf.Atan(specularSize / (2 * distance)) * Mathf.Rad2Deg;

        Specular.LookAt(diff);
    }
}
