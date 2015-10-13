using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class RoadMap : MonoBehaviour {

	public Sprite[] TileSet;

	public Transform Cursor;
	Dictionary<string, RoadTile> RoadGrid = new Dictionary<string, RoadTile>();

	private gridPathfinder Pathfind = new gridPathfinder();

	// Use this for initialization
	void Start () {
		//Debug.Log("Add event");
		//GUImain.OnCursorClick += OnRoadClick;
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnEnable() {
		GUImain.OnCursorClick += OnRoadClick;
	}
	void OnDisable() {
		GUImain.OnCursorClick -= OnRoadClick;
	}

	void OnRoadClick() {
		if (GUImain.DragObject == null || GUImain.DragObject.Resource.id != 5001)  return;
		Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition) / transform.localScale.x / 0.5f;
			mouse.x = Mathf.Round(mouse.x);
			mouse.y = Mathf.RoundToInt(mouse.y);
			Cursor.position = mouse * transform.localScale.x * 0.5f;

			AddRoad(mouse);
	}

	void AddRoad(Vector3 pos) {
		AddRoad((int)pos.x, (int)pos.y);
	}

	void AddRoad(int x, int y) {
		if (Tile(x,y) != null) return;

		RoadTile tile = RoadTile.Create(x, y, this);
		RoadGrid.Add(Index(x,y),tile);

		Pathfind.AddNode(x, y);

		Pathfind.FindPath(new Vector3(0,0),new Vector3(x,y));
		LineRenderer line = GetComponent<LineRenderer>();
		line.SetVertexCount(Pathfind.Path.Count);
		int i = 0;
		foreach (Vector3 key in Pathfind.Path) {
			line.SetPosition(i++, key * 0.5f);
		}
		

		tile.Value = TileValue(x, y);
		int xx, yy;
	
		xx = x + 0; yy = y - 1;
		tile = Tile(xx, yy); if (tile != null) tile.Value = TileValue(xx, yy);
		xx = x + 1; yy = y - 0;
		tile = Tile(xx, yy); if (tile != null) tile.Value = TileValue(xx, yy);
		xx = x + 0; yy = y + 1;
		tile = Tile(xx, yy); if (tile != null) tile.Value = TileValue(xx, yy);
		xx = x - 1; yy = y - 0;
		tile = Tile(xx, yy); if (tile != null) tile.Value = TileValue(xx, yy);

	}

	int TileValue(int x, int y) {
		
		int result = 0;

		result += Tile(x + 0, y - 1) == null ? 0 : 1;
		result += Tile(x + 1, y - 0) == null ? 0 : 2;
		result += Tile(x + 0, y + 1) == null ? 0 : 4;
		result += Tile(x - 1, y - 0) == null ? 0 : 8;

		return result;
	}

	RoadTile Tile(int x, int y) {
		RoadTile result = null;
		RoadGrid.TryGetValue(Index(x,y),out result);
		return result;
	}

	public static string Index(int x, int y) {
		return x.ToString() + ":" + y.ToString();
	}
}
