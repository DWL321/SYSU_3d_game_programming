using UnityEngine;

public class LookAtCamera : MonoBehaviour {

	void Update () {
		this.transform.rotation = Quaternion.LookRotation(new Vector3(0, 0, 1));
	}
}