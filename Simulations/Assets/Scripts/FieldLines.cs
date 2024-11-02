using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class FieldLines {
    public class FieldLine
    {
        private GameObject mGameObject;
        private LineRenderer mLine;
        private readonly float mResolution;

        public FieldLine(
            Vector3 start,
            float resolution)
        {
            mGameObject = new GameObject("LineObject");

            mLine = mGameObject.AddComponent<LineRenderer>();

            mLine.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            mLine.startWidth = mLine.endWidth = 0.1f;
            mLine.startColor = mLine.endColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);

            mResolution = resolution;

            // WalkLine(start, ref charges, resolution);
            mLine.positionCount = 2;
            mLine.SetPositions(new Vector3[] { Vector3.one * Random.value, Vector3.one * Random.value });
        }

        public void Destroy()
        {
            GameObject.Destroy(mLine);
            mLine = null;
            GameObject.Destroy(mGameObject);
            mGameObject = null;
        }

        public void WalkLine(
            Vector3 start,
            ref List<PointCharge> charges,
            float segmentLength,
            Vector2 camBounds)
        {
            mLine.positionCount = 1;
            mLine.SetPosition(0, start);

            int positionIndex = 0;
            Vector3 newPoint = Vector2.zero;
            Vector3 currentPoint = start;
            do {
                newPoint = currentPoint +
                    ElectrostaticCalculations.ElectricField(ref charges, currentPoint).normalized * segmentLength;

                positionIndex++;
                mLine.positionCount = positionIndex + 1;
                mLine.SetPosition(positionIndex, newPoint);

                currentPoint = newPoint;
            }
            while (!EndLine(ref charges, newPoint, camBounds) && positionIndex < 1000);
        }

        // end line if it is out of bounds or touching a point charge
        public bool EndLine(
            ref List<PointCharge> charges,
            Vector3 newPoint,
            Vector2 camBounds)
        {
            bool boundedX = Mathf.Abs(newPoint.x) > camBounds.x;
            bool boundedY = Mathf.Abs(newPoint.y) > camBounds.y;

            bool touchingCharge = false;
            float sqrRadius = PointCharge.Radius * PointCharge.Radius;
            for (int i = 0; i < charges.Count; i++) {
                if ((newPoint - charges[i].mPosition).sqrMagnitude <= sqrRadius) {
                    touchingCharge = true;
                    break;
                }
            }

            return boundedX || boundedY || touchingCharge;
        }
    }

    private List<FieldLine> mFieldLines;
    private readonly int mLinesPerCharge;
    private readonly Vector2 mCamBounds;
    private readonly float mSegmentLength;

    public FieldLines(
        Vector2 camBounds,
        int linesPerCharge,
        float segmentLength)
    {
        mFieldLines = new List<FieldLine>();

        mCamBounds = camBounds;
        mLinesPerCharge = linesPerCharge;
        mSegmentLength = segmentLength;
    }

    public void Update(
        List<PointCharge> charges)
    {
        int deltaLineCount = charges.Count * mLinesPerCharge - mFieldLines.Count;

        if (deltaLineCount > 0) {
            for (int i = 0; i < deltaLineCount; i++) {
                mFieldLines.Add(new FieldLine(Vector2.zero, 1.0f));
            }
        }
        else if (deltaLineCount < 0) {
            for (int i = 0; i < Mathf.Abs(deltaLineCount); i++) {
                mFieldLines[0].Destroy();
                mFieldLines.RemoveAt(0);
            }
        }

        for (int charge = 0; charge < charges.Count; charge++) {
            for (int line = 0; line < mLinesPerCharge; line++) {
                float angle = 2.0f * Mathf.PI * line / mLinesPerCharge;

                Vector3 start = charges[charge].mPosition +
                    new Vector3(PointCharge.Radius * Mathf.Cos(angle), PointCharge.Radius * Mathf.Sin(angle), 0.0f);

                mFieldLines[charge * mLinesPerCharge + line].WalkLine(start, ref charges, mSegmentLength, mCamBounds);
            }
        }
    }
}
