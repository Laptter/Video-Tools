using System;
using System.Collections.Generic;
using UnityEngine;

public class Managerment : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private Vector2[] uv;

    private int cols;
    private int rows;

    private int cellSize = 120;

    public Transform pointPrefab;
    private List<Transform> pointPool = new List<Transform>();
    private void OnEnable()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Plane";
        SetColAndRow();
        SetVerticles();
        SetTriangles();
        SetUVS();
        SetPlaneScale();
        CreatePointPool();
        ShowVertex();

    }

    private void CreatePointPool()
    {
        for (int i = 0; i < cols * rows; i++)
        {
            var point = Instantiate<Transform>(pointPrefab);
            pointPool.Add(point);
            point.SetParent(GameObject.Find("Canvas").transform);
            point.gameObject.SetActive(false);
        }
    }
    private void Update()
    {
        //ResetPosition();
        //SetVerticles();
        //SetTriangles();
        //SetUVS();
        SetPlaneScale();
        ShowVertex();
    }
    private void SetUVS()
    {
        if (uv != null)
        {
            Array.Clear(uv, 0, uv.Length);
        }

        uv = new Vector2[vertices.Length];
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                uv[row * cols + col].x = 1f * (cols - col - 1) / (cols - 1);
                uv[row * cols + col].y = 1f * (rows - row - 1) / (rows - 1);
            }
        }
        mesh.uv = uv;
    }

    private void SetColAndRow()
    {
        cols = Screen.width / cellSize;
        rows = Screen.height / cellSize;
    }

    private void SetVerticles()
    {
        if (vertices != null)
        {
            Array.Clear(vertices, 0, vertices.Length);
        }

        vertices = new Vector3[cols * rows];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                float x = 10f * col / (cols - 1) - 5f;
                float z = 10f * row / (rows - 1) - 5f;
                Vector3 point = new Vector3(x, 0, z) + transform.position;
                vertices[row * cols + col] = point;
            }
        }
        mesh.vertices = vertices;
    }


    private void SetVertexPoint(int incol, int inrow, Vector3 point)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (row == inrow && col == incol)
                {
                    vertices[row * cols + col] = point;
                }
            }
        }
        mesh.vertices = vertices;
    }


    private void ShowVertex()
    {
        if (vertices == null)
        {
            return;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 scale = transform.localScale;
            Quaternion rotation = transform.rotation;
            Vector3 newPos = new Vector3(vertices[i].x * scale.x, vertices[i].y * scale.y, vertices[i].z * scale.z);
            newPos = rotation * (newPos) + transform.position;
            var screenPos = Camera.main.WorldToScreenPoint(newPos);
            pointPool[i].position = screenPos;
            pointPool[i].gameObject.SetActive(true);
        }
    }



    private void SetTriangles()
    {
        if (triangles != null)
        {
            Array.Clear(triangles, 0, triangles.Length);
        }

        triangles = new int[(cols - 1) * (rows - 1) * 6];
        int length = (cols - 1) * (rows - 1);

        for (int i = 0; i < length; i++)
        {
            int index = -1;
            if ((i + 1) % cols != 0)
            {
                index = i * 6;
                triangles[index + 3] = triangles[index] = i;
                triangles[index + 1] = i + cols;
                triangles[index + 4] = triangles[index + 2] = i + cols + 1;
                triangles[index + 5] = i + 1;
            }
            else
            {
                index = i * 6;
                triangles[index + 3] = triangles[index] = length + i / cols;
                triangles[index + 1] = cols + length + i / cols;
                triangles[index + 4] = triangles[index + 2] = cols + 1 + length + i / cols;
                triangles[index + 5] = length + 1 + i / cols;
            }
        }

        mesh.triangles = triangles;
    }

    private void SetPlaneScale()
    {
        Camera cam = Camera.main;
        float pos = cam.nearClipPlane + 10.0f;
        transform.position = cam.transform.position + cam.transform.forward * pos;
        transform.LookAt(cam.transform);
        transform.Rotate(90.0f, 0.0f, 0.0f);
        float h = (Mathf.Tan(cam.fieldOfView * Mathf.Deg2Rad * 0.5f) * pos * 2f) / 10.0f;
        transform.localScale = new Vector3(h * cam.aspect, 1.0f, h);
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 scale = transform.localScale;
            Quaternion rotation = transform.rotation;
            Vector3 newPos = new Vector3(vertices[i].x * scale.x, vertices[i].y * scale.y, vertices[i].z * scale.z);
            newPos = rotation * (newPos) + transform.position;
            Gizmos.DrawSphere(newPos, 0.1f);
        }
    }


    private void ResetPosition()
    {
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;
    }


    private void OnDisable()
    {

    }
}
