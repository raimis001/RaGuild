using UnityEngine;
using System.Collections.Generic;
using System;


#region NODE

public class gridNode {
	public int x = 0;
	public int y = 0;
	public int ID = 0;
	public bool walkable = true;
	public gridNode parent = null;

	public int F = 0;
	public int H = 0;
	public int G = 0;

	//Use for faster look ups
	public int sortedIndex = -1;

	public gridNode(int indexX, int indexY, int idValue, bool w, gridNode p = null) {
		x = indexX;
		y = indexY;
		ID = idValue;
		walkable = w;
		parent = p;
		F = 0;
		G = 0;
		H = 0;
		//UnityEngine.Debug.Log("New node ID:" + ID.ToString());
	}
}
#endregion

#region Search node
public class NodeSearch : IComparable<NodeSearch> {
	private int id = 0;
	public int F = 0;
	public float Fv = 0F;

	public int ID {
		get {
			return id;
		}
		private set {
			this.id = value;
		}
	}

	public NodeSearch(int i, int f) {
		id = i;
		F = f;
	}

	public NodeSearch(int i, float f) {
		id = i;
		Fv = f;
	}


	public int CompareTo(NodeSearch b) {
		return this.F.CompareTo(b.F);
	}
}
#endregion

public class SortOnFvalue : IComparer<NodeSearch> {
	public int Compare(NodeSearch a, NodeSearch b) {
		if (a.F > b.F) return 1;
		else if (a.F < b.F) return -1;
		else return 0;
	}
}


public class gridPathfinder {

	public int HeuristicAggression = 0;
	public bool MoveDiagonal = false;

	static int _nodeID = 0;
	static int NodeID {
		get {
			return _nodeID++;
		}
	}

	public List<Vector3> Path = new List<Vector3>();

	Dictionary<string, gridNode> Grid = new Dictionary<string, gridNode>();

	int maxSearchRounds = 0;

	Dictionary<int,gridNode> openList = new Dictionary<int,gridNode>();
	Dictionary<int,gridNode> closedList = new Dictionary<int,gridNode>();
	gridNode endNode;
	gridNode currentNode;
	List<NodeSearch> sortedOpenList = new List<NodeSearch>();

	public void AddNode(int x, int y, bool walkable = true) {
		gridNode node = new gridNode(x, y, NodeID, walkable);
		Grid.Add(GridIndex(x,y), node);
	}

	string GridIndex(int x, int y) {
		return x.ToString() + ":" + y.ToString();
	}

	gridNode GetNode(int x, int y) {
		gridNode result = null;
		Grid.TryGetValue(GridIndex(x,y),out result);
		//if (result == null) result = new Node(x, y, 0, NodeID, 0, 0, false);
	
		return result;
	}

	public void FindPath(Vector3 startPos, Vector3 endPos) {
		//The list we returns when path is found
		Path.Clear();
		ClearTemp();
		bool endPosValid = true;

		//Find start and end nodes, if we cant return null and stop!
		gridNode startNode = FindClosestNode(startPos);
		endNode = FindClosestNode(endPos);

		if (startNode != null) {
			if (endNode == null) {
				endPosValid = false;
				endNode = FindEndNode(endPos);
				if (endNode == null) {
					//still no end node - we leave and sends an empty list
					maxSearchRounds = 0;
					//listMethod.Invoke(new List<Vector3>());
					print("Empty Openlist, closedList");
					return;
				}
			}

			//Clear lists if they are filled

			//Insert start node
			AddOpenList(startNode.ID,startNode);
			//sortedOpenList.Add(new NodeSearch(startNode.ID, startNode.F));
			BHInsertNode(new NodeSearch(startNode.ID, startNode.F));

			bool endLoop = false;

			while (!endLoop) {
				if (sortedOpenList.Count == 0) {
					print("Empty sortedOpenList");
					ClearTemp();
					return;
				}

				//Get lowest node and insert it into the closed list
				int id = BHGetLowest();
				//print("Loop " + id.ToString());
				currentNode = openList[id];
				AddClosedList(currentNode.ID,currentNode);
				AddOpenList(id,null);

				if (currentNode.ID == endNode.ID) {
					endLoop = true;
					continue;
				}

				//Now look at neighbours, check for unwalkable tiles, bounderies, open and closed listed nodes.

				NonDiagonalNeighborCheck();
			}


			while (currentNode.parent != null) {
				Path.Add(new Vector3(currentNode.x, currentNode.y));
				currentNode = currentNode.parent;
			}

			Path.Add(startPos);
			Path.Reverse();

			if (endPosValid) {
				Path.Add(endPos);
			}

			if (Path.Count > 2 && endPosValid) {
				//Now make sure we do not go backwards or go to long
				if (Vector3.Distance(Path[Path.Count - 1], Path[Path.Count - 3]) < Vector3.Distance(Path[Path.Count - 3], Path[Path.Count - 2])) {
					Path.RemoveAt(Path.Count - 2);
				}
				if (Vector3.Distance(Path[1], startPos) < Vector3.Distance(Path[0], Path[1])) {
					Path.RemoveAt(0);
				}
			}
			maxSearchRounds = 0;

		} else {
			maxSearchRounds = 0;
		}

		ClearTemp();

	}


	private void ClearTemp() {
		openList.Clear();
		closedList.Clear();
		sortedOpenList.Clear();
	}
	private gridNode FindClosestNode(Vector3 pos) {
		int x = (int)pos.x; 
		int y = (int)pos.y;

		gridNode n = GetNode(x, y);

		if (n != null && n.walkable) {
			return new gridNode(x, y, n.ID, n.walkable);
		} else {
			//If we get a non walkable tile, then look around its neightbours
			for (int i = y - 1; i < y + 2; i++) {
				for (int j = x - 1; j < x + 2; j++) {
						n = GetNode(j, i);
						if (n != null && n.walkable) {
							return new gridNode(j, i, n.ID, n.walkable);
						}
				}
			}
			return null;
		}
	}

	private gridNode FindEndNode(Vector3 pos) {
		int x = (int)pos.x; 
		int y = (int)pos.y;

		gridNode closestNode = GetNode(x, y);
		if (closestNode == null) closestNode = new gridNode(x, y, NodeID, false);

		List<gridNode> walkableNodes = new List<gridNode>();

		int turns = 1;

		while (walkableNodes.Count < 1 && maxSearchRounds < (int)10) {
			walkableNodes = EndNodeNeighbourCheck(x, y, turns);
			turns++;
			maxSearchRounds++;
		}

		if (walkableNodes.Count > 0) {//If we found some walkable tiles we will then return the nearest
        
			int lowestDist = 99999999;
			gridNode n = null;

			foreach (gridNode node in walkableNodes) {
				int i = GetHeuristics(closestNode, node);
				if (i < lowestDist) {
					lowestDist = i;
					n = node;
				}
			}
			return new gridNode(n.x, n.y, n.ID, n.walkable);
		}

		return null;
	}

	private List<gridNode> EndNodeNeighbourCheck(int x, int z, int r) {
		List<gridNode> nodes = new List<gridNode>();

		for (int i = z - r; i < z + r + 1; i++) {
			for (int j = x - r; j < x + r + 1; j++) {
				gridNode n = GetNode(j, i);
				if (n != null && n.walkable) nodes.Add(n);
			}
		}

		return nodes;
	}


	private void NonDiagonalNeighborCheck(bool diagonal = false) {
		int x = currentNode.x;
		int y = currentNode.y;
		for (int i = y - 1; i < y + 2; i++) {
			for (int j = x - 1; j < x + 2; j++) {
				if (i == y && j == x) continue; //Dont check for the current node.
				if (!diagonal && GetMovementCost(x, y, j, i) >= 14) continue; //Check that we are not moving diagonal
				gridNode n = GetNode(j, i);
				if (n == null || !n.walkable) continue; //Check the node is walkable
				if (OnClosedList(n.ID)) continue; //We do not recheck anything on the closed list
				
				//If it is not on the open list then add it to
				if (!OnOpenList(n.ID)) {
					gridNode addNode = new gridNode(n.x, n.y, n.ID, n.walkable, currentNode);
						addNode.H = GetHeuristics(n.x, n.y);
						addNode.G = GetMovementCost(x, y, j, i) + currentNode.G;
						addNode.F = addNode.H + addNode.G;
						//Insert on open list
						AddOpenList(addNode.ID,addNode);
						//Insert on sorted list
						BHInsertNode(new NodeSearch(addNode.ID, addNode.F));
				} else {
					///If it is on openlist then check if the new paths movement cost is lower
					gridNode nn = GetNodeFromOpenList(n.ID);
					if (currentNode.G + GetMovementCost(x, y, j, i) < openList[n.ID].G) {
						nn.parent = currentNode;
						nn.G = currentNode.G + GetMovementCost(x, y, j, i);
						nn.F = nn.G + nn.H;
						BHSortNode(nn.ID, nn.F);
					}
				}


			}//endfor j
		}//Endfor i
		
	}

	private void ChangeFValue(int id, int F) {
		foreach (NodeSearch ns in sortedOpenList) {
			if (ns.ID == id) {
				ns.F = F;
			}
		}
	}

	//Check if a Node is already on the openList
	private bool OnOpenList(int id) {
		return openList.ContainsKey(id) && openList[id] != null; 
		//return (openList[id] != null) ? true : false;
	}
	private gridNode GetNodeFromOpenList(int id) {
		gridNode result = null;
		openList.TryGetValue(id, out result);
		return result;
		//return (openList[id] != null) ? openList[id] : null;
	}
	private void AddOpenList(int id, gridNode node) {
		if (openList.ContainsKey(id)) 
			openList[id] = node;
			else openList.Add(id, node);
	}

	//Check if a Node is already on the closedList
	private bool OnClosedList(int id) {
		return closedList.ContainsKey(id) && closedList[id] != null;
		//return (closedList[id] != null) ? true : false;
	}
	private void AddClosedList(int id, gridNode node) {
		if (closedList.ContainsKey(id))
			closedList[id] = node;
			else closedList.Add(id, node);
	}

	private int GetMovementCost(int x, int y, int j, int i) {
		//Moving straight or diagonal?
		return (x != j && y != i) ? 14 : 10;
	}



	
	private int GetHeuristics(int x, int y) {
		//Make sure heuristic aggression is not less then 0!
		int HA = (HeuristicAggression < 0) ? 0 : HeuristicAggression;
		return (int)(Mathf.Abs(x - endNode.x) * (10F + (10F * HA))) + (int)(Mathf.Abs(y - endNode.y) * (10F + (10F * HA)));
	}

	private int GetHeuristics(gridNode a, gridNode b) {
		//Make sure heuristic aggression is not less then 0!
		int HA = (HeuristicAggression < 0) ? 0 : HeuristicAggression;
		return (int)(Mathf.Abs(a.x - b.x) * (10F + (10F * HA))) + (int)(Mathf.Abs(a.y - b.y) * (10F + (10F * HA)));
	}


	#region Binary_Heap (min)

	private void BHInsertNode(NodeSearch ns) {
		//We use index 0 as the root!
		if (sortedOpenList.Count == 0) {
			sortedOpenList.Add(ns);
			openList[ns.ID].sortedIndex = 0;
			return;
		}

		sortedOpenList.Add(ns);
		bool canMoveFurther = true;
		int index = sortedOpenList.Count - 1;
		openList[ns.ID].sortedIndex = index;

		while (canMoveFurther) {
			int parent = Mathf.FloorToInt((index - 1) / 2);

			if (index == 0) //We are the root
            {
				canMoveFurther = false;
				openList[sortedOpenList[index].ID].sortedIndex = 0;
			} else {
				if (sortedOpenList[index].F < sortedOpenList[parent].F) {
					NodeSearch s = sortedOpenList[parent];
					sortedOpenList[parent] = sortedOpenList[index];
					sortedOpenList[index] = s;

					//Save sortedlist index's for faster look up
					openList[sortedOpenList[index].ID].sortedIndex = index;
					openList[sortedOpenList[parent].ID].sortedIndex = parent;

					//Reset index to parent ID
					index = parent;
				} else {
					canMoveFurther = false;
				}
			}
		}
	}

	private void BHSortNode(int id, int F) {
		bool canMoveFurther = true;
		int index = openList[id].sortedIndex;
		sortedOpenList[index].F = F;

		while (canMoveFurther) {
			int parent = Mathf.FloorToInt((index - 1) / 2);

			if (index == 0) //We are the root
            {
				canMoveFurther = false;
				openList[sortedOpenList[index].ID].sortedIndex = 0;
			} else {
				if (sortedOpenList[index].F < sortedOpenList[parent].F) {
					NodeSearch s = sortedOpenList[parent];
					sortedOpenList[parent] = sortedOpenList[index];
					sortedOpenList[index] = s;

					//Save sortedlist index's for faster look up
					openList[sortedOpenList[index].ID].sortedIndex = index;
					openList[sortedOpenList[parent].ID].sortedIndex = parent;

					//Reset index to parent ID
					index = parent;
				} else {
					canMoveFurther = false;
				}
			}
		}
	}

	private int BHGetLowest() {

		if (sortedOpenList.Count == 1) //Remember 0 is our root
        {
			int ID = sortedOpenList[0].ID;
			sortedOpenList.RemoveAt(0);
			return ID;
		} else if (sortedOpenList.Count > 1) {
			//save lowest not, take our leaf as root, and remove it! Then switch through children to find right place.
			int ID = sortedOpenList[0].ID;
			sortedOpenList[0] = sortedOpenList[sortedOpenList.Count - 1];
			sortedOpenList.RemoveAt(sortedOpenList.Count - 1);
			openList[sortedOpenList[0].ID].sortedIndex = 0;

			int index = 0;
			bool canMoveFurther = true;
			//Sort the list before returning the ID
			while (canMoveFurther) {
				int child1 = (index * 2) + 1;
				int child2 = (index * 2) + 2;
				int switchIndex = -1;

				if (child1 < sortedOpenList.Count) {
					switchIndex = child1;
				} else {
					break;
				}
				if (child2 < sortedOpenList.Count) {
					if (sortedOpenList[child2].F < sortedOpenList[child1].F) {
						switchIndex = child2;
					}
				}
				if (sortedOpenList[index].F > sortedOpenList[switchIndex].F) {
					NodeSearch ns = sortedOpenList[index];
					sortedOpenList[index] = sortedOpenList[switchIndex];
					sortedOpenList[switchIndex] = ns;

					//Save sortedlist index's for faster look up
					openList[sortedOpenList[index].ID].sortedIndex = index;
					openList[sortedOpenList[switchIndex].ID].sortedIndex = switchIndex;

					//Switch around idnex
					index = switchIndex;
				} else {
					break;
				}
			}
			return ID;

		} else {
			return -1;
		}
	}

	#endregion

	#region Tools
	void print(string message) {
		//Debug.Log(message);
	}
	#endregion
}
