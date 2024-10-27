using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class VectorField
{
    private class Arrow
    {
        private static readonly Vector3[] mVertexTemplate = new Vector3[]
        {
            /* Origin is the center of the line on the tail.
             *                2
             * 0              |\ 
             * |--------------- \
             * |              1  \ 3
             * |              5  /
             * |--------------- /
             * 6              |/
             * Not to scale   4
             */               
            new Vector3(0.0f, 0.5f, 0.0f),  new Vector3(1.0f, 0.5f, 0.0f),
            new Vector3(1.0f, 1.0f, 0.0f),  new Vector3(2.0f, 0.0f, 0.0f),
            new Vector3(1.0f, -1.0f, 0.0f), new Vector3(1.0f, -0.5f, 0.0f),
            new Vector3(0.0f, -0.5f, 0.0f)
        };

        private static readonly int[] mIndices = { 0, 1, 6, 1, 5, 6, 2, 3, 4 };

        public Mesh mMesh;
        public ArrowSizeStruct mArrowSizes;
        public Vector2 mPos;

        public struct ArrowSizeStruct
        {
            public float mTailWidth;
            public float mTailLength;
            public float mHeadWidth;
            public float mHeadLength;

            public ArrowSizeStruct(float tailWidth, float tailLength, float headWidth, float headLength)
            {
                mTailWidth = tailWidth;
                mTailLength = tailLength;
                mHeadWidth = headWidth;
                mHeadLength = headLength;
            }
        }

        public Arrow(float magnitude, Vector2 pos)
        {
            mMesh = new Mesh();
            // used as temp until Update()
            mMesh.vertices = mVertexTemplate;
            mMesh.triangles = mIndices;

            mPos = pos;

            Update(magnitude, 0.0f);

            Color32[] colors = new Color32[mMesh.vertices.Length];
            Array.Fill(colors, new Color32(0, 0, 0, 255));
            mMesh.SetColors(colors);
        }

        public void Update(float magnitude, float angleRad)
        {
            int[] headIndices = new int[3] { 2, 3, 4 };

            Vector3[] tempVertices = new Vector3[mVertexTemplate.Length];

            mArrowSizes = MagnitudeToArrowSize(magnitude);

            Vector3 pos = mPos;

            for (int i = 0; i < mVertexTemplate.Length; i++) {
                Vector3 vertex = mVertexTemplate[i];

                // the order of the following operations is important - scale, translate, rotate.
                // if not a head index, must be a tail
                if (headIndices.Contains(i)) {
                    vertex.x *= mArrowSizes.mHeadLength;
                    vertex.y *= mArrowSizes.mHeadWidth;
                }
                else {
                    vertex.x *= mArrowSizes.mTailLength;
                    vertex.y *= mArrowSizes.mTailWidth;
                }

                vertex += pos;

                vertex = RotateVertex(vertex, angleRad);

                tempVertices[i] = vertex;
            }

            mMesh.vertices = tempVertices;
        }

        public static Mesh CombineAllMeshes(Arrow[] arrows)
        {
            CombineInstance[] combine = new CombineInstance[arrows.Length];

            for (int i = 0; i < combine.Length; i++) {
                combine[i].mesh = arrows[i].mMesh;
            }

            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combine, true, false);

            return mesh;
        }

        private Vector2 RotateVertex(Vector2 vertex, float angleRad)
        {
            vertex -= mPos;

            float r = MathF.Sqrt((vertex.x * vertex.x) + (vertex.y * vertex.y));
            float newAngle = Mathf.Atan2(vertex.y, vertex.x) + angleRad;

            vertex.y = r * MathF.Sin(newAngle);
            vertex.x = r * MathF.Cos(newAngle);

            return vertex + mPos;
        }

        private ArrowSizeStruct MagnitudeToArrowSize(float magnitude)
        {
            return new(
                0.1f * magnitude,
                0.2f * magnitude,
                0.15f * magnitude,
                0.2f * magnitude);
        }
    }

    public float mDirectionRad = Mathf.PI;
    public float mMagnitude = 0;

    private Arrow[] mArrows;
    private GameObject mObject;

    public VectorField(Vector2 camOrigin, Vector2 camBounds, Vector2 vectorDensity)
    {
        int columnCnt = Mathf.CeilToInt(2 * camBounds.x * vectorDensity.x);
        int rowCnt = Mathf.CeilToInt(2 * camBounds.y * vectorDensity.y);
        int arrowCnt = columnCnt * rowCnt;

        Vector2 spacing = vectorDensity;
        spacing.x /= 1.0f;
        spacing.y /= 1.0f;

        mArrows = new Arrow[arrowCnt];

        for (int column = 0; column < columnCnt; column++) {
            for (int row = 0; row < rowCnt; row++) {
                float x = spacing.x * (column + 0.5f) - camBounds.x;
                float y = spacing.y * (row + 0.5f) - camBounds.y;

                mArrows[rowCnt * column + row] = new Arrow(1.0f, new Vector2(x, y));
            }
        }

        mObject = new GameObject("VectorFieldMeshHost");

        Mesh totalMesh = Arrow.CombineAllMeshes(mArrows);
        AttachMeshToHostObject(totalMesh);
    }

    public void Update(
        ControlStructs.Electrostatics2D controls,
        List<PointCharge> pointCharges = null)
    {
        float magnitude = 1.5f - Mathf.Sin(2.0f * Time.time);
        // float angle = Time.time % (2.0f * MathF.PI);

        for (int i = 0; i < mArrows.Length; i++) {
            mArrows[i].Update(magnitude, 0.0f);
        }

        Mesh totalMesh = Arrow.CombineAllMeshes(mArrows);
        AttachMeshToHostObject(totalMesh);
    }

    private void AttachMeshToHostObject(Mesh mesh)
    {
        if (mObject == null) {
            mObject = new GameObject("VectorFieldMeshHost");
        }
        if (mObject.GetComponent<MeshRenderer>() == null) {
            MeshRenderer meshRenderer = mObject.AddComponent<MeshRenderer>();
            meshRenderer.material = new Material(Shader.Find("Standard"));
        }
        MeshFilter meshFilter;
        if ((meshFilter = mObject.GetComponent<MeshFilter>()) == null) {
            meshFilter = mObject.AddComponent<MeshFilter>();
        }
        meshFilter.mesh = mesh;
    }
}
