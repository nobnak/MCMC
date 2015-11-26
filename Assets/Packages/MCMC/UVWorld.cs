using UnityEngine;
using System.Collections;

namespace MCMCSystem {
	[RequireComponent(typeof(MeshFilter))]
	public class UVWorld : MonoBehaviour {
		Mesh _mesh;
		Vector3[] _vertices;
		int[] _triangles;
		Vector2[] _uvs;
		Vector3[] _normals;
		Triangle2D[] _uvTris;

		void Start() {
			_mesh = GetComponent<MeshFilter>().sharedMesh;
			Init();
		}

		public void Init() {
			_triangles = _mesh.triangles;
			_vertices = _mesh.vertices;
			_normals = _mesh.normals;
			_uvs = _mesh.uv;

			var triangleCount = _triangles.Length / 3;
			_uvTris = new Triangle2D[triangleCount];
			for (var i = 0; i < triangleCount; i++) {
				Vector2 uva, uvb, uvc;
				TriangleUvs(i, out uva, out uvb, out uvc);
				_uvTris[i] = new Triangle2D(uva, uvb, uvc);
			}
		}
		public bool World(Vector2 uv, out Vector3 pos, out Vector3 normal) {
			for (var i = 0; i < _uvTris.Length; i++) {
				var uvTri = _uvTris[i];
				float s, t;
				if (uvTri.Solve(uv, out s, out t)) {
					var r = 1f - (s + t);
					pos = InterpolatePosition(i, s, t, r);
					normal = InterpolateNormal(i, s, t, r);
					return true;
				}
			}

			pos = default(Vector3);
			normal = default(Vector3);
			return false;
		}

		public void TriangleUvs(int i, out Vector2 uva, out Vector2 uvb, out Vector2 uvc) {
			var i3 = 3 * i;
			uva = _uvs [_triangles [i3]];
			uvb = _uvs [_triangles [i3 + 1]];
			uvc = _uvs [_triangles [i3 + 2]];
		}
		public void TriangleVertices(int i, out Vector3 va, out Vector3 vb, out Vector3 vc) {
			var i3 = 3 * i;
			va = _vertices [_triangles [i3]];
			vb = _vertices [_triangles [i3 + 1]];
			vc = _vertices [_triangles [i3 + 2]];
		}
		public void TriangleNormals(int i, out Vector3 na, out Vector3 nb, out Vector3 nc) {
			var i3 = 3 * i;
			na = _normals [_triangles [i3]];
			nb = _normals [_triangles [i3 + 1]];
			nc = _normals [_triangles [i3 + 2]];
		}

		public Vector3 InterpolatePosition (int i, float s, float t, float r) {
			Vector3 va, vb, vc;
			TriangleVertices(i, out va, out vb, out vc);
			return r * va + s * vb + t * vc;
		}
		public Vector3 InterpolateNormal(int i, float s, float t, float r) {
			Vector3 na, nb, nc;
			TriangleNormals (i, out na, out nb, out nc);
			return r * na + s * nb + t * nc;
		}

		public class Triangle2D {
			public Vector2 a;
			public Vector2 ab;
			public Vector2 ac;
			public float inv_det_abac;
			public bool valid;

			public Triangle2D(Vector2 a, Vector2 b, Vector2 c) {
				this.a = a;
				this.ab = b - a;
				this.ac = c - a;
				var det_abac = (ab.x * ac.y - ab.y * ac.x);
				this.inv_det_abac = 1f / det_abac;
				this.valid = (det_abac < -Mathf.Epsilon || Mathf.Epsilon < det_abac);
			}

			public bool Solve(Vector2 uv, out float s, out float t) {
				if (!valid) {
					s = 0f;
					t = 0f;
					return false;
				}

				var ap = uv - a;
				var det_apac = ap.x * ac.y - ap.y * ac.x;
				if ((s = det_apac * inv_det_abac) < 0f) {
					t = 0f;
					return false;
				}

				var det_abap = ab.x * ap.y - ab.y * ap.x;
				if ((t = det_abap * inv_det_abac) < 0f)
					return false;

				var r = 1f - (s + t);
				return r >= 0f;
			}
		}
	}
}
