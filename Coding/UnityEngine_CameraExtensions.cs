using UnityEngine;

namespace MS.UsingExtensionMethods {

	public static class CameraExtensions {

		public static float Dot(this Camera cam, Vector3 dir) {
			return Vector3.Dot(cam.transform.forward, dir);
		}

	}

}