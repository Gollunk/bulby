using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SBK.Pathfinding;

/// <summary>
/// Tiled grid script.
/// </summary>
public class TiledGridScript : MonoBehaviour {

	const bool DEBUG_MODE = true;
	const float RAYCAST_FREE_OFFSET = 0.5f;
	const float SCAN_REFRESHDELAY = 1f;

	public int tiling;
	public float tileSize;
	private TiledGrid grid;
	private float scanTrigger = 0f;

	/// <summary>
	/// Gets the grid.
	/// </summary>
	/// <value>The grid.</value>
	public TiledGrid Grid { 
		get{ return this.grid; }
	}

	/// <summary>
	/// Awake this instance.
	/// </summary>
	void Awake ()
	{
		HashSet<int> obstacleSet = this.ScanForObstacles();
		this.grid = new TiledGrid(this.tiling, obstacleSet);		
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update ()
	{
		// raffraichissement des obstacles de la zone
		/*
		this.scanTrigger += Time.deltaTime;
		if(this.scanTrigger >= SCAN_REFRESHDELAY)
		{
			this.scanTrigger = 0f;
			this.grid.ObstacleSet = this.ScanForObstacles();
		}
		*/
	}

	/// <summary>
	/// Scans for obstacles.
	/// </summary>
	/// <returns>The for obstacles.</returns>
	private HashSet<int> ScanForObstacles()
	{
		HashSet<int> obstacleSet = new HashSet<int>();
		for(int i=0; i<this.tiling; i++)
		{
			for(int j=0; j<this.tiling; j++)
			{
				Vector3 localOrigin;
				localOrigin.x = (j * (1/(float)this.tiling)) -0.5f + 1/(float)(2 * this.tiling);
				localOrigin.z = (i * (1/(float)this.tiling)) -0.5f + 1/(float)(2 * this.tiling);
				localOrigin.y = 0f;

				int layerMask = 1 << 10;
				layerMask = ~layerMask;
				Vector3 origin = this.transform.TransformPoint(localOrigin);
				Vector3 direction = -this.transform.up;
				float raycastDistance = ((this.tiling * this.tileSize) / 2) - RAYCAST_FREE_OFFSET;

				if(Physics.Raycast(origin, direction, raycastDistance, layerMask))
				{
					if(DEBUG_MODE) Debug.DrawRay(origin, direction * raycastDistance, Color.red, 10.0f);
					int index = i * this.tiling + j;
					obstacleSet.Add(index);

				}	else{
					if(DEBUG_MODE) Debug.DrawRay(origin, direction * raycastDistance, Color.green, 10.0f);
				}
			}
		}	
		return obstacleSet;
	}
}
