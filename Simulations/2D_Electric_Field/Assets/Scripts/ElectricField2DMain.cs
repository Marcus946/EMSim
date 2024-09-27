using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ElectricField2DMain : MonoBehaviour
{
    public Camera mCamera;

    private VectorField mVectorField;

    void Start()
    {
        Vector2 camBounds = new Vector2(mCamera.orthographicSize * mCamera.aspect, mCamera.orthographicSize);
        // mph add actual density later
        mVectorField = new VectorField(mCamera.transform.position, camBounds, Vector2.one);
    }

    void Update()
    {
        mVectorField.Update();
    }
}
