using System;
using System.Collections.Generic;
using UnityEngine;

public class PointCharge
{
    private GameObject mGameObject;

    public float mChargeNC;

    public Vector3 mPosition
    {
        get { return _mPosition; }
        set {
            if (mGameObject != null) {
                mGameObject.transform.position = value;
            }
            _mPosition = value;
        }
    }

    private Vector3 _mPosition;

    public const float Radius = 0.25f;

    public PointCharge(
        float chargeNC,
        Vector3 pos,
        bool createGameObject = true)
    {
        if (createGameObject) {
            mGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mGameObject.transform.localScale = 2.0f * Radius * Vector3.one;
        } 
        else {
            mGameObject = null;
        }

        mChargeNC = chargeNC;
        mPosition = pos;
    }

    ~PointCharge()
    {
    }

    public byte[] Serialize()
    {
        byte[] buf = new byte[SizeOf()];

        byte[] temp = BitConverter.GetBytes(mChargeNC);

        temp.CopyTo(buf, 0);

        for (int i = 0; i < 3; /*i++ done in buf.CopyTo*/) {
            temp = BitConverter.GetBytes(mPosition[i]);
            temp.CopyTo(buf, (++i) * sizeof(float));
        }

        return buf;
    }

    public static int SizeOf()
    {
        return 4 * sizeof(float);
    }

    public static List<PointCharge> DeserializeList(
        byte[] buffer)
    {
        List<PointCharge> charges = new List<PointCharge>();

        int bytesRead = 0;

        int pointChargeCount = BitConverter.ToInt32(buffer, 0);
        bytesRead += 4;

        for (int i = 0; i < pointChargeCount; i++) {
            float chargeNC = BitConverter.ToSingle(buffer, bytesRead);
            bytesRead += 4;

            Vector3 pos;
            pos.x = BitConverter.ToSingle(buffer, bytesRead);
            bytesRead += 4;

            pos.y = BitConverter.ToSingle(buffer, bytesRead);
            bytesRead += 4;

            pos.z = BitConverter.ToSingle(buffer, bytesRead);
            bytesRead += 4;

            charges.Add(new PointCharge(chargeNC, pos, false));
        }

        return charges;
    }

    public void CopyAttributes(
        PointCharge other)
    {
        mChargeNC = other.mChargeNC;
        mPosition = other.mPosition;
    }

    public bool GameObjectNull()
    {
        return mGameObject == null;
    }

    public void DestroyGameObject()
    {
        GameObject.Destroy(mGameObject);
        mGameObject = null;
    }
}
