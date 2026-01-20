using System.Reflection;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve, int levelOfDetail)
    {
        heightCurve = new AnimationCurve(heightCurve.keys);
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topleftX = (width - 1) / -2f;
        float topleftZ = (height - 1) / 2f;

        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerline = (width - 1) / meshSimplificationIncrement + 1;

        MeshData meshData = new MeshData(verticesPerline, verticesPerline);
        int vertexIndex = 0;

        for (int y = 0; y < height; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < width; x += meshSimplificationIncrement)
            {
                meshData.vertices[vertexIndex] = new Vector3(topleftX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topleftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticesPerline + 1, vertexIndex + verticesPerline);
                    meshData.AddTriangle(vertexIndex + verticesPerline + 1, vertexIndex, vertexIndex + 1);
                }
                vertexIndex++;
            }
        }

        return meshData; // CORRECTION: d�plac� ici, apr�s les boucles
    }

    public class MeshData
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uvs;
        int triangleIndex;

        // CORRECTION: constructeur � l'int�rieur de MeshData
        public MeshData(int meshWidth, int meshHeight)
        {
            vertices = new Vector3[meshWidth * meshHeight];
            uvs = new Vector2[meshWidth * meshHeight];
            triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
        }

        // CORRECTION: m�thodes � l'int�rieur de MeshData
        public void AddTriangle(int a, int b, int c)
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }

        public Mesh CreateMesh()
        {
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}