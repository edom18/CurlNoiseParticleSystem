using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace CurlNoiseParticleSystem.Utility
{
	static public class CurlParticleUtility
	{
        public const int MAX_VERTEX_NUM = 65534;

        /// <summary>
        /// パーリンノイズ用のグリッドを生成する
        /// </summary>
        /// <returns></returns>
        static public int[] CreateGrid(int seed)
        {
            Xorshift xorshift = new Xorshift((uint)seed);

            int[] p = new int[256];
            for (int i = 0; i < p.Length; i++)
            {
                p[i] = (int)Mathf.Floor(xorshift.Random() * 256);
            }

            int[] p2 = new int[512];
            for (int i = 0; i < p2.Length; i++)
            {
                p2[i] = p[i & 255];
            }

            return p2;
        }


        /// <summary>
        /// ベースの頂点からランダムにバラけてた点を得る
        /// </summary>
        /// <param name="baseVec">ベースとなる頂点</param>
        /// <returns>ランダムな値を足した新しい頂点</returns>
        static public Vector3 GetRandomVector(Vector3 baseVec, float range)
        {
            float x = baseVec.x + Random.Range(-range, range);
            float y = baseVec.y + Random.Range(-range, range);
            float z = baseVec.z + Random.Range(-range, range);
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// 指定した数分、マージしたメッシュを生成する
        /// </summary>
        /// <param name="mesh">元となるメッシュ</param>
        /// <param name="num">生成するメッシュ数</param>
        /// <returns>マージされたメッシュ</returns>
        static public Mesh CreateCombinedMesh(Mesh mesh, int num)
        {
            Assert.IsTrue(mesh.vertexCount * num <= MAX_VERTEX_NUM);

            int[] meshIndices = mesh.GetIndices(0);
            int indexNum = meshIndices.Length;

            // Buffer
            int[] indices = new int[num * indexNum];
            List<Vector2> uv0 = new List<Vector2>();
            List<Vector2> uv1 = new List<Vector2>();
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector4> tangents = new List<Vector4>();

            for (int id = 0; id < num; id++)
            {
                vertices.AddRange(mesh.vertices);
                normals.AddRange(mesh.normals);
                tangents.AddRange(mesh.tangents);
                uv0.AddRange(mesh.uv);

                // 各メッシュのIndexは、1つのモデルの頂点数 * ID分ずらす
                for (int n = 0; n < indexNum; n++)
                {
                    indices[id * indexNum + n] = id * mesh.vertexCount + meshIndices[n];
                }

                // 2番目のUVにIDを格納しておく
                for (int n = 0; n < mesh.uv.Length; n++)
                {
                    uv1.Add(new Vector2(id, id));
                }
            }

            Mesh combinedMesh = new Mesh();
            combinedMesh.SetVertices(vertices);
            combinedMesh.SetIndices(indices, MeshTopology.Triangles, 0);
            combinedMesh.SetNormals(normals);
            combinedMesh.RecalculateNormals();
            combinedMesh.SetTangents(tangents);
            combinedMesh.SetUVs(0, uv0);
            combinedMesh.SetUVs(1, uv1);

            combinedMesh.RecalculateBounds();
            Bounds bounds = new Bounds();
            bounds.center = combinedMesh.bounds.center;
            bounds.SetMinMax(Vector3.one * -100f, Vector3.one * 100f);
            combinedMesh.bounds = bounds;

            return combinedMesh;
        }
	}
}