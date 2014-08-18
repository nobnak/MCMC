using UnityEngine;
using System.Collections;

public class Quad2Screen : MonoBehaviour {
	void Awake () {
		var size = 2f * Camera.main.orthographicSize;
		var aspect = Camera.main.aspect;
		transform.localScale = new Vector3(size * aspect, size, 1f);
	}
}
