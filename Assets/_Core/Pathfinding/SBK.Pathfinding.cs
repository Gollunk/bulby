using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SBK.Collections;

namespace SBK.Pathfinding
{
	/// <summary>
	/// Tiled grid.
	/// </summary>
	public class TiledGrid
	{
		private int tiling; // nombre de tiles par coté
		private HashSet<int> obstacleSet = new HashSet<int>(); // ensemble des indexs de tile contenant un obstacle

		#region Getters and Setters

		public HashSet<int> ObstacleSet { get{ return this.obstacleSet; } set{this.obstacleSet = value;}}
		public int Tiling { get{ return this.tiling; }}

		/// <summary>
		/// Gets the node number.
		/// </summary>
		/// <value>The node number.</value>
		public int NodeNumber { 
			get{ return this.tiling * this.tiling; }
		}

		#endregion

		/// <summary>
		/// Initializes a new instance of the <see cref="SBK.Pathfinding.TiledGrid"/> class.
		/// </summary>
		/// <param name="_tiling">_tiling.</param>
		/// <param name="_obstacles">_obstacles.</param>
		public TiledGrid(int _tiling, HashSet<int> _obstacles)
		{
			this.tiling = _tiling;
			this.obstacleSet = _obstacles;
		}

		/// <summary>
		/// Gets the index tile by position and tiling.
		/// </summary>
		/// <returns>The index tile by position and tiling</returns>
		/// <param name="_localPosition">_local position.</param>
		/// <param name="_tiling">_tiling.</param>
		public int GetIndexTileByPosition(Vector3 _localPosition)
		{
			float ratio_position_x = _localPosition.x + 0.5f; // 0.5f correspond à la correction par rapport au repère local de Unity [-0.5, 0.5]
			float ratio_position_z = _localPosition.z + 0.5f;  // 0.5f correspond à la correction par rapport au repère local de Unity [-0.5, 0.5]
			int index_x = (int) Mathf.Floor(this.tiling * ratio_position_x);
			int index_z = (int) Mathf.Floor(this.tiling * ratio_position_z);
			int index = index_z * this.tiling + index_x;
			return index;
		}
		
		/// <summary>
		/// Gets the adjacent node indexes.
		/// </summary>
		/// <returns>The adjacent node indexes.</returns>
		/// <param name="_nodeIndex">_node index.</param>
		public HashSet <int> GetAdjacentNodeIndexes (int _nodeIndex)
		{
			HashSet <int> l = new HashSet<int>();
			bool diagonal_up = true;
			bool diagonal_down = true;
			bool diagonal_left = true;
			bool diagonal_right = true;
			int index;

			// HAUT
			index = _nodeIndex - this.tiling;
			if((index >= 0) && !this.obstacleSet.Contains(index))
			{
				l.Add(index);
			}	else diagonal_up = false;

			// BAS
			index = _nodeIndex + this.tiling;
			if((index < NodeNumber) && !this.obstacleSet.Contains(index))
			{
				l.Add(index);
			}
			else diagonal_down = false;

			// GAUCHE
			index = _nodeIndex - 1;
			if((_nodeIndex %this.tiling != 0) && !this.obstacleSet.Contains(index))
			{
				l.Add(index);
			}
			else diagonal_left = false;

			// DROITE
			index = _nodeIndex + 1;
			if((_nodeIndex %this.tiling != (this.tiling-1)) && !this.obstacleSet.Contains(index))
			{
				l.Add(index);
			}
			else diagonal_right = false;

			// DIAGONALES

			// haut-gauche / gauche-haut
			if (diagonal_up != false && diagonal_left != false)
			{
				if(_nodeIndex %this.tiling != 0 && _nodeIndex >= this.tiling)
				{
					index = _nodeIndex - this.tiling - 1;
					if(!this.obstacleSet.Contains(index)){
						l.Add(index);
					}
				}
			}
			// bas-gauche / gauche-bas
			if (diagonal_down != false && diagonal_left != false)
			{
				if(_nodeIndex %this.tiling != 0 && _nodeIndex < (NodeNumber - this.tiling))
				{
					index = _nodeIndex + this.tiling - 1;
					if(!this.obstacleSet.Contains(index)){
						l.Add(index);
					}
				}
			}
			// haut-droite / droite-haut
			if (diagonal_up != false && diagonal_right != false)
			{
				if(_nodeIndex %this.tiling != (this.tiling-1) && _nodeIndex >= this.tiling)
				{
					index = _nodeIndex - this.tiling + 1;
					if(!this.obstacleSet.Contains(index)){
						l.Add(index);
					}
				}
			}
			// bas-droite / droite-bas
			if (diagonal_down != false && diagonal_right != false)
			{
				if(_nodeIndex %this.tiling != (this.tiling-1) && _nodeIndex < (NodeNumber - this.tiling))
				{
					index = _nodeIndex + this.tiling + 1;
					if(!this.obstacleSet.Contains(index)){
						l.Add(index);
					}
				}
			}

			return l;
		}
	}

	/// <summary>
	/// Node path
	/// Todo : Idéalement, externaliser les éléments propres à Unity tel que Vector3, Vector2, etc.
	/// </summary>
	public class NodePath : IComparable
	{	
		private NodePath parent;
		private int nodeIndex;
		private float heuristic;
		private float score;
		private float cost;
		private Vector2 origin;

		public NodePath Parent { get{ return this.parent; } set{ this.parent = value; }}
		public int NodeIndex { get{ return this.nodeIndex; }}
		public float Score { get{ return this.score; }}
		public Vector2 Origin { get{ return this.origin; }}

		/// <summary>
		/// Origine du node (position locale par rapport au parent).
		/// </summary>
		/// <param name="_nodeIndex">_node index.</param>
		/// <param name="_parent">_parent.</param>
		public NodePath (int _nodeIndex, int _tiling, NodePath _parent=null) 
		{
			this.nodeIndex = _nodeIndex;
			this.parent = _parent;

			// calcul de l'origine
			Vector2 ori = new Vector2();
			int index_x = _nodeIndex %_tiling;
			int index_y = ((_nodeIndex - (_nodeIndex %_tiling)) / _tiling);
			ori.x = (index_x * (1/(float)_tiling)) -0.5f + 1/(float)(2 * _tiling);
			ori.y = (index_y * (1/(float)_tiling)) -0.5f + 1/(float)(2 * _tiling);
			this.origin = ori;
		}

		/// <summary>
		/// Gets the parents.
		/// </summary>
		/// <returns>The parents.</returns>
		public List<NodePath> GetParents ()
		{
			List<NodePath> parents = new List<NodePath> ();
			NodePath currentParent = this.parent;
			while(currentParent != null)
			{
				parents.Add(currentParent);
				currentParent = currentParent.Parent;
			}
			parents.Reverse();
			return parents;
		}

		/// <summary>
		/// Calculs the properties of the node path
		/// </summary>
		/// <param name="tiling">Tiling.</param>
		/// <param name="startOrigin">Start origin.</param>
		/// <param name="endOrigin">End origin.</param>
		public void CalculProperties (Vector2 startOrigin, Vector2 endOrigin)
		{
			this.cost = Math.Abs(this.origin.x - startOrigin.x) + Math.Abs(this.origin.y - startOrigin.y);
			this.heuristic = Math.Abs(this.origin.x - endOrigin.x) + Math.Abs(this.origin.y - endOrigin.y);
			this.score = this.cost + this.heuristic;
		}

		#region IComparable Members

		/// <summary>CompareTo</summary>
		/// <param name="obj">L'objet à comparer</param>
		public int CompareTo(object obj)
		{
			return(this.score.CompareTo(((NodePath)obj).Score));
		}
		
		#endregion
	}

	/// <summary>
	/// Pathfinder.
	/// </summary>
	public class Pathfinder 
	{
		private TiledGrid grid;
		private Heap openList = new Heap(); // stocke les nodes éligibles
		private Heap closedList = new Heap(); // stocke les nodes vérifiés et non-éligibles
		private HashSet<int> openIndexes = new HashSet<int>(); // stocke les indexs des nodes présents dans openList
		private HashSet<int> closedIndexes = new HashSet<int>(); // stocke les indexs des nodes présents dans closedList

		#region Constructeurs

		/// <summary>
		/// Initializes a new instance of the <see cref="SBK.Pathfinding.Pathfinder"/> class.
		/// </summary>
		/// <param name="_tiling">_tiling.</param>
		public Pathfinder (TiledGrid _grid) {
			this.grid = _grid;
		}

		#endregion

		#region Traitements sur les listes Open et Closed

		/// <summary>
		/// Adds the node to open list.
		/// </summary>
		/// <returns>The node to open list.</returns>
		/// <param name="_index">_index.</param>
		/// <param name="_parent">_parent.</param>
		private NodePath AddNodeToOpenList (int _index, NodePath _parent=null)
		{
			NodePath n = new NodePath(_index, this.grid.Tiling, _parent);
			this.openIndexes.Add(_index);
			this.openList.Push(n);
			return n;
		}

		/// <summary>
		/// Moves the node to closed list.
		/// </summary>
		/// <returns>The node to closed list.</returns>
		/// <param name="_nodePath">_node path.</param>
		private void MoveNodeToClosedList (NodePath _nodePath)
		{
			this.closedIndexes.Add(_nodePath.NodeIndex);
			this.closedList.Push(_nodePath);
			this.openIndexes.Remove(_nodePath.NodeIndex);
			this.openList.Remove(_nodePath);
		}

		#endregion

		#region Processus de recherche de chemin 

		/// <summary>
		/// Checks for intersection (obstacles) between two position using Bresenham's algorythm
		/// </summary>
		/// <returns><c>true</c>, if for intersection was checked, <c>false</c> otherwise.</returns>
		/// <param name="fromPosition">From position.</param>
		/// <param name="toPosition">To position.</param>
		public bool CheckForIntersection(Vector3 _fromPosition, Vector3 _toPosition)
		{
			// 0.5f correspond à la correction par rapport au repère local de Unity [-0.5, 0.5]
			int x0 = (int) Mathf.Floor(this.grid.Tiling * (_fromPosition.x + 0.5f));
			int y0 = (int) Mathf.Floor(this.grid.Tiling * (_fromPosition.z + 0.5f));
			int x1 = (int) Mathf.Floor(this.grid.Tiling * (_toPosition.x + 0.5f));
			int y1 = (int) Mathf.Floor(this.grid.Tiling * (_toPosition.z + 0.5f));
			
			bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
			if (steep){
				int t;
				t = x0; // swap x0 and y0
				x0 = y0;
				y0 = t;
				t = x1; // swap x1 and y1
				x1 = y1;
				y1 = t;
			}
			if (x0 > x1){
				int t;
				t = x0; // swap x0 and x1
				x0 = x1;
				x1 = t;
				t = y0; // swap y0 and y1
				y0 = y1;
				y1 = t;
			}
			int dx = x1 - x0;
			int dy = Math.Abs(y1 - y0);
			int error = dx / 2;
			int ystep = (y0 < y1) ? 1 : -1;
			int y = y0;
			for (int x = x0; x <= x1; x++)
			{
				int index;
				if(steep){	index = x * this.grid.Tiling + y; }	
				else{		index = y * this.grid.Tiling + x; }
				
				// Si intersection détéctée -> retourne true
				if (this.grid.ObstacleSet.Contains(index)){
					return true;
				}
				
				error = error - dy;
				if (error < 0){
					y += ystep;
					error += dx;
				}
			}
			
			// pas d'intesection détéctée -> retourne false
			return false;
		}

		private bool CheckForIntersection(int _fromIndex, int _toIndex)
		{
			int x0 = _fromIndex % this.grid.Tiling;
			int y0 = (int) Mathf.Floor(_fromIndex / this.grid.Tiling);
			int x1 = _toIndex % this.grid.Tiling;
			int y1 = (int) Mathf.Floor(_toIndex / this.grid.Tiling);
			
			bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
			if (steep){
				int t;
				t = x0; // swap x0 and y0
				x0 = y0;
				y0 = t;
				t = x1; // swap x1 and y1
				x1 = y1;
				y1 = t;
			}
			if (x0 > x1){
				int t;
				t = x0; // swap x0 and x1
				x0 = x1;
				x1 = t;
				t = y0; // swap y0 and y1
				y0 = y1;
				y1 = t;
			}
			int dx = x1 - x0;
			int dy = Math.Abs(y1 - y0);
			int error = dx / 2;
			int ystep = (y0 < y1) ? 1 : -1;
			int y = y0;
			for (int x = x0; x <= x1; x++)
			{
				int index;
				if(steep){	index = x * this.grid.Tiling + y; }	
				else{		index = y * this.grid.Tiling + x; }
				
				// Si intersection détéctée -> retourne true
				if (this.grid.ObstacleSet.Contains(index)){
					return true;
				}
				
				error = error - dy;
				if (error < 0){
					y += ystep;
					error += dx;
				}
			}
			
			// pas d'intesection détéctée -> retourne false
			return false;
		}

		/// <summary>
		/// Gets the simplified path using bresenham's line algorythm.
		/// </summary>
		/// <returns>The simplified path by bresenham.</returns>
		/// <param name="_startPosition">_start position.</param>
		/// <param name="_endPosition">_end position.</param>
		public List<NodePath> GetSimplifiedPathByBresenham (Vector3 _startPosition, Vector3 _endPosition)
		{
			int startIndex = this.grid.GetIndexTileByPosition(_startPosition);
			List<NodePath> path = this.FindPath (_startPosition, _endPosition);
			List<NodePath> simplifiedPath = new List<NodePath>();
			NodePath lastNode = null;
			int lastAvailableNodeIndex = 0;
			int i = 0;
			foreach(NodePath np in path)
			{
				if(i==0){
					lastAvailableNodeIndex = np.NodeIndex;
					//simplifiedPath.Add (np);

				}	else{
					if(this.CheckForIntersection(lastAvailableNodeIndex, np.NodeIndex)){
						lastAvailableNodeIndex = lastNode.NodeIndex;
						simplifiedPath.Add (lastNode);
					}
				}
				lastNode = np;
				i++;
			}
			return simplifiedPath;
		}

		/// <summary>
		/// Trouve un chemin simplifié entre 2 positions.
		///	Retourne uniquement les nodes dont la direction change.
		/// </summary>
		/// <returns>The simplified path.</returns>
		/// <param name="startPosition">Start position.</param>
		/// <param name="endPosition">End position.</param>
		public List<NodePath> GetSimplifiedPath (Vector3 startPosition, Vector3 endPosition)
		{
			List<NodePath> path = this.FindPath (startPosition, endPosition);
			List<NodePath> simplifiedPath = new List<NodePath>();
			NodePath lastNode = null;
			int lastDirectionValue = 0;
			int lastNodePathListIndex = path.Count -1;
			int directionValue = 0;
			int i = 0;

			foreach(NodePath np in path)
			{
				if(i!=0) // exclus le node de départ
				{
					directionValue = np.NodeIndex - lastNode.NodeIndex;
					if(directionValue != lastDirectionValue){
						simplifiedPath.Add (lastNode);
					}

					if(i==lastNodePathListIndex){
						simplifiedPath.Add (np);
					}	else{
						lastDirectionValue = directionValue;
					}
				}

				lastNode = np;
				i++;
			}

			return simplifiedPath;
		}

		/// <summary>
		/// Trouve un chemin entre 2 positions.
		///	Retourne l'ensemble des nodes constituants le chemin
		/// </summary>
		/// <returns>Liste d'objets NodePath correspondant au chemin trouvé</returns>
		/// <param name="startIndex">Start index.</param>
		/// <param name="endIndex">End index.</param>
		public List<NodePath> FindPath (Vector3 startPosition, Vector3 endPosition)
		{		
			List<NodePath> path = new List<NodePath>();

			// RAZ des Collections
			this.openList.Clear();
			this.closedList.Clear();
			this.openIndexes.Clear();
			this.closedIndexes.Clear();

			int startIndex = this.grid.GetIndexTileByPosition(startPosition);
			int endIndex = this.grid.GetIndexTileByPosition(endPosition);
			NodePath startNode = this.AddNodeToOpenList(startIndex);
			NodePath endNode = new NodePath(endIndex, this.grid.Tiling);
			//startNode.CalculProperties(startNode.Origin, endNode.Origin);
			//endNode.CalculProperties(startNode.Origin, endNode.Origin);

			NodePath currentNode = null;
			NodePath lastNode = null;

			while(this.openList.Count > 0)
			{
				lastNode = currentNode;
				currentNode = (NodePath)this.openList.Pop();
				if(currentNode.NodeIndex == endIndex)
				{	
					// chemin trouvé
					path = currentNode.GetParents();
					break;
				}
				else
				{
					this.MoveNodeToClosedList(currentNode);
					HashSet<int> adjacentNodeIndexes = this.grid.GetAdjacentNodeIndexes(currentNode.NodeIndex);
					foreach(int nodeIndex in adjacentNodeIndexes)
					{
						if(!this.openIndexes.Contains(nodeIndex) && !this.closedIndexes.Contains(nodeIndex))
						{
							NodePath newNode = this.AddNodeToOpenList(nodeIndex, currentNode);
							newNode.CalculProperties(startNode.Origin, endNode.Origin);
						}
					}
				}
			}

			return path;
		}

		#endregion
	}
}
