using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] private float rotateSpeed;
    [SerializeField] private Vector3 rotateDir;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotateSpeed * rotateDir * Time.deltaTime);
    }
}
