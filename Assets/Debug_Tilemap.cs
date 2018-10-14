using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Debug_Tilemap : MonoBehaviour {

    Tilemap tilemap;

	// Use this for initialization
	void Start () {
        this.tilemap = GetComponent<Tilemap>();


        Vector3Int v = new Vector3Int(1, 1, 0);
        this.tilemap.SetTile(v, null);

        Vector3Int v2 = new Vector3Int(0, -5, 0);
        this.tilemap.SetTile(v2, null);

        //Debug.Log(this.tilemap.GetTile(v));
    }

    // Update is called once per frame
    void Update () {

	}
}
