using UnityEngine;
using System.Collections;
public class ForCamera : MonoBehaviour
{
    
    private Transform target;
    public float rotSpeed = 1.5f;
    private float _rotY = 0;
    private float _rotX = 0;

    void LateUpdate()
    {   
        _rotY += Input.GetAxis("Mouse Y") * rotSpeed * 3;
        _rotX += Input.GetAxis("Mouse X") * rotSpeed * 3;
        Quaternion rotation = Quaternion.Euler(-_rotY, _rotX, 0);
        transform.localRotation = rotation;
       // transform.RotateAround(transform.parent, new Vector3)
      
 }
}