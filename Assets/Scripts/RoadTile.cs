using UnityEngine;
using System.Collections;

public class RoadTile : MonoBehaviour {

	[HideInInspector]
	public int X;

	[HideInInspector]
	public int Y;

	public string Index {
		get {
			return RoadMap.Index(X, Y);
		}
	}

	RoadMap Parent;

	int currentValue = 0;
	public int Value {
		set {
			currentValue = value;
			//Debug.Log(currentValue);
			GetComponent<SpriteRenderer>().sprite = Parent.TileSet[currentValue];
		}
	}

	public static RoadTile Create(int x, int y, RoadMap parent) {
		GameObject obj = Instantiate<GameObject>(Resources.Load<GameObject>("RoadTile"));
			obj.transform.SetParent(parent.transform);
			obj.transform.localScale = Vector3.one;
			obj.transform.position = new Vector3(x,y) * parent.transform.localScale.x * 0.5f;

		RoadTile tile = obj.GetComponent<RoadTile>();
			tile.X = x;
			tile.Y = y;
			tile.Parent = parent;

		return tile;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
