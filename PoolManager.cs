using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
	Dictionary<int,Queue<ObjectInstance>> poolDictionary = new Dictionary<int, Queue<ObjectInstance>>();

	static PoolManager _instance;

	public static PoolManager instance 
	{
		get{
			if(_instance == null)
			{
				_instance = FindObjectOfType<PoolManager> ();
			}
			return _instance;
		}
	}
	public void CreatePool (GameObject prefab, int poolSize)
	{
		int poolKey = prefab.GetInstanceID ();

		GameObject poolHolder = new GameObject (prefab.name + " pool");
		poolHolder.transform.parent = this.gameObject.transform;

		if(!poolDictionary.ContainsKey (poolKey))
		{
			poolDictionary.Add (poolKey, new Queue<ObjectInstance> ());

			for(int i = 0; i < poolSize; i++)
			{
				ObjectInstance newObject = new ObjectInstance(Instantiate (prefab) as GameObject);
				poolDictionary [poolKey].Enqueue (newObject);
				newObject.SetParent (poolHolder.transform);
			}
		}
	}

	public void ReuseObject (GameObject prefab, Vector3 pos, Quaternion rot) 
	{
		int poolkey = prefab.GetInstanceID ();

		if(poolDictionary.ContainsKey(poolkey))
		{
			ObjectInstance objectToReuse = poolDictionary [poolkey].Dequeue ();
			poolDictionary [poolkey].Enqueue (objectToReuse);

			objectToReuse.Reuse (pos, rot);
		}
	}

	public class ObjectInstance
	{
		GameObject go;
		Transform trans;

		bool hasPO;
		PoolObject po;

		public ObjectInstance (GameObject obj)
		{
			go = obj;
			trans = obj.transform;
			go.SetActive(false);

			if(go.GetComponent<PoolObject>())
			{
				hasPO = true;
				po = go.GetComponent<PoolObject>();
			}
		}

		public void Reuse(Vector3 pos, Quaternion rot)
		{
			if(hasPO)
			{
				po.OnObjectReuse ();
			}

			go.SetActive (true);
			go.transform.position = pos;
			go.transform.rotation = rot;
		}

		public void SetParent (Transform parent)
		{
			trans.parent = parent;		
		}
	}
}
