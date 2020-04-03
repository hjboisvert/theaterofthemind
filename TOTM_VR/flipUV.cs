using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
public class flipUV : MonoBehaviour
{

    void Start()
    {
        MeshFilter filter = GetComponent(typeof(MeshFilter)) as MeshFilter;
        if (filter != null)
        {
            Mesh mesh = filter.mesh;
            Vector2[] uv = mesh.uv;
            for (int c = 0; c < uv.Length; c++)
            {
                uv[c].x = 1.0f - uv[c].x;
            }
            mesh.uv = uv;
        }
    }
}