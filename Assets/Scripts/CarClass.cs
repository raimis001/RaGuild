using UnityEngine;
using System.Collections;

public class CarClass : MonoBehaviour {


	bool IsBreak = false;
	bool IsMove = false;

	float Speed = 2f;

	private LineRenderer lineRenderer;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}



	public void SetCarPath(Vector3 destination) {
		//FindPath(transform.position, destination);
		//StartCoroutine(MovePath());
	}

	IEnumerator MovePath() {
		if (IsMove) IsBreak = true;
		yield return null;

		IsMove = true;
		IsBreak = false;
		//Debug.Log("Start path nodes:" + Path.Count.ToString());
		/*
		while (Path.Count > 0) { 

			Vector3 path = Path[0];

			//float StartDistance = distance / 2f;
			//Vector3 startPostition = transform.position;

			Vector2 dist = Path[0] - transform.position;
			float angle = Mathf.Atan2(dist.y, dist.x) * Mathf.Rad2Deg + 180f;
			transform.rotation = Quaternion.Euler(0, 0, angle);

			float distance = Vector3.Distance(transform.position, path);
			while (distance > 0.01f) {
				//Debug.Log(distance);

				if (IsBreak) { IsMove = false; yield break; }

				//transform.position = Vector3.MoveTowards(transform.position, path, Speed * Time.smoothDeltaTime);
				transform.position = Vector3.Lerp(transform.position, path, Time.smoothDeltaTime * Speed / distance);
				yield return null;
				distance = Vector3.Distance(transform.position, path);
			}

			if (IsBreak) { IsMove = false; yield break; }
			transform.position = path;
			Path.RemoveAt(0);
			//Debug.Log("    remove node:" + Path.Count.ToString());
			yield return null;
		}
		*/
		transform.rotation = Quaternion.Euler(0, 0, 180);
		IsMove = false;

	}
}
