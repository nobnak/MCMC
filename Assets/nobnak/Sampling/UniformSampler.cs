using UnityEngine;
using System.Collections;
using nobnak.Search;

namespace nobnak.Sampling {
    public class UniformSampler {
        public Mesh M { get; private set; }

        private float[] _cumWeights;

        public UniformSampler(Mesh m, float[] cumWeights) {
            this.M = m;
            this._cumWeights = cumWeights;
        }

        public void Sample(out Vector3 position, out Vector3 normal, out Vector2 uv) {
            var iWeight = _cumWeights.GreatestLowerBound(Random.value);
            SampleOnTriangle(3 * iWeight, out position, out normal, out uv);
        }

		public static UniformSampler Generate(GameObject g) {
			var mf = g.GetComponent<MeshFilter>();
			if (mf == null || mf.sharedMesh == null)
				return null;
			return Generate(mf.sharedMesh);
		}
        public static UniformSampler Generate(Mesh m) {
            var triangles = m.triangles;
            var vertices = m.vertices;
            var cumWeights = new float[triangles.Length / 3];
            var totalArea = 0f;
            for (var i = 0; i < cumWeights.Length; i++) {
                var iTriangle = 3 * i;
                var p0 = vertices[triangles[iTriangle]];
                var p1 = vertices[triangles[iTriangle + 1]];
                var p2 = vertices[triangles[iTriangle + 2]];
                var area = Vector3.Cross(p1 - p0, p2 - p0).magnitude * 0.5f;
                cumWeights[i] = totalArea;
                totalArea += area;
            }
            for (var i = 0; i < cumWeights.Length; i++)
                cumWeights[i] /= totalArea;

            return new UniformSampler(m, cumWeights);
        }

        public void SampleOnTriangle(int iTriangle, out Vector3 position, out Vector3 normal, out Vector2 uv) {
            var u = Random.value;
            var v = Random.value;
            var tr0 = M.triangles[iTriangle];
            var tr1 = M.triangles[iTriangle+1];
            var tr2 = M.triangles[iTriangle+2];
            var p0 = M.vertices[tr0];
            var p1 = M.vertices[tr1];
            var p2 = M.vertices[tr2];
            var n0 = M.normals[tr0];
            var n1 = M.normals[tr1];
            var n2 = M.normals[tr2];
			var uv0 = M.uv[tr0];
			var uv1 = M.uv[tr1];
			var uv2 = M.uv[tr2];
            if ((u + v) > 1f) {
                u = 1f - u;
                v = 1f - v;
            }
            var w = 1f - (u + v);
            position = w * p0 + u * p1 + v * p2;
            normal = w * n0 + u * n1 + v * n2;
			uv = w * uv0 + u * uv1 + v * uv2;
        }
    }
}