using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonMeshGenerator : MonoBehaviour
{
    public static Mesh GenerateHexagonMesh(float size)
    {
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[7];
        int[] triangles = new int[18];

        // 중심점
        vertices[0] = Vector3.zero;

        // 6개의 꼭짓점 계산
        for (int i = 0; i < 6; i++)
        {
            float angle_deg = 60 * i - 30;
            float angle_rad = Mathf.Deg2Rad * angle_deg;
            vertices[i + 1] = new Vector3(size * Mathf.Cos(angle_rad), 0, size * Mathf.Sin(angle_rad));
        }

        // 삼각형 인덱스 순서 수정 (시계 방향)
        for (int i = 0; i < 6; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 2 <= 6 ? i + 2 : 1;
            triangles[i * 3 + 2] = i + 1;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
