using System.Collections.Generic;
using UnityEngine;

public class ElectrostaticCalculations {
    public const float CoulombsConstant = 8.99e9f;
    public const float NCToCoulombs = 1e-9f;

    public static Vector3 ElectricField(
        ref List<PointCharge> charges,
        Vector3 position)
    {
        Vector3 totalField = Vector3.zero;

        for (int i = 0; i < charges.Count; i++) {
            Vector3 directionVec = position - charges[i].mPosition;
            
            totalField += CoulombsConstant * charges[i].mChargeNC
                * NCToCoulombs / directionVec.sqrMagnitude * directionVec.normalized;
        }

        return totalField;
    }
}
