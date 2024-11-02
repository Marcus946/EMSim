using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ControlStructs;
using System;

public class Electrostatics2DMain : MonoBehaviour
{
    public Camera mCamera;

    private VectorField mVectorField;
    private FieldLines mFieldLines;
    private List<PointCharge> mPointCharges;
    private int mSelectedChargeIndex = -1;
    private float mTimeSinceLastTick = 0.0f;
    private const float TicksPerSecond = 20.0f;

    void Start()
    {
        MemoryManager.Open();

        mPointCharges = new List<PointCharge>();

        Vector2 camBounds = new Vector2(mCamera.orthographicSize * mCamera.aspect, mCamera.orthographicSize);
        mVectorField = new VectorField(camBounds, Vector2.one);

        mFieldLines = new FieldLines(camBounds, 10, 0.1f);
    }

    void Update()
    {
        Vector2 mousePos = mCamera.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonUp(0)) {
            mSelectedChargeIndex = -1;
        }

        if (mSelectedChargeIndex != -1) {
            mPointCharges[mSelectedChargeIndex].mPosition = mousePos;
        }

        if (Input.GetMouseButtonDown(0) && mSelectedChargeIndex == -1) {
            for (int i = 0; i < mPointCharges.Count; i++) {
                // comparing square magnitudes is faster than using sqrt for actual distance
                Vector2 distance = (Vector2)mPointCharges[i].mPosition - mousePos;
                if (distance.sqrMagnitude <= PointCharge.Radius * PointCharge.Radius) {
                    mSelectedChargeIndex = i;
                    mPointCharges[mSelectedChargeIndex].mPosition = mousePos;
                }
            }
        }

        mTimeSinceLastTick += Time.deltaTime;

        if (mTimeSinceLastTick >= 1.0f / TicksPerSecond) {
            Tick();
        }

        // mph change controlstruct once it's used
        mVectorField.Update(new Electrostatics2D(), mPointCharges);

        mFieldLines.Update(mPointCharges);
    }

    void Tick()
    {
        byte[] buffer;
        bool success = MemoryManager.Read(out buffer, MemoryManager.BufferSize());

        if (success) {
            ReassignPointChargeGameObjects(PointCharge.DeserializeList(buffer));
        }

        WriteSimToSharedMemory();

        mTimeSinceLastTick = 0.0f;
    }

    void ReassignPointChargeGameObjects(
        List<PointCharge> newCharges)
    {
        int deltaChargeCount = newCharges.Count - mPointCharges.Count;

        if (deltaChargeCount < 0) {
            while (mPointCharges.Count > newCharges.Count) {
                mPointCharges[0].Destroy();
                mPointCharges.RemoveAt(0);
            }
        }
        else if (deltaChargeCount > 0) {
            mPointCharges.AddRange(newCharges.Skip(mPointCharges.Count));
        }

        // the first indexes common to newCharges and currentCharges are recycled.
        // this side of the simulation is responsible for controlling point charge positions,
        // so it doesn't listen to the shared memory on that
        for (int i = 0; i < Mathf.Min(mPointCharges.Count, newCharges.Count); i++) {
            if (!mPointCharges[i].GameObjectNull()) {
                newCharges[i].mPosition = mPointCharges[i].mPosition;
                mPointCharges[i].CopyAttributes(newCharges[i]);
            }
            else { 
                mPointCharges[i] = new PointCharge(newCharges[i].mChargeNC, newCharges[i].mPosition);
            }
        }
    }

    void WriteSimToSharedMemory()
    {
        byte[] buf = new byte[sizeof(int) + mPointCharges.Count * PointCharge.SizeOf() + Electrostatics2D.SizeOf()];

        BitConverter.GetBytes(mPointCharges.Count).CopyTo(buf, 0);

        for (int i = 0; i < mPointCharges.Count; i++) {
            byte[] test = mPointCharges[i].Serialize();
            mPointCharges[i].Serialize().CopyTo(buf, sizeof(int) + i * PointCharge.SizeOf());
        }

        MemoryManager.Write(buf);
    }
}
