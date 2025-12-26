using UnityEngine;

namespace MS.UsingExtensionMethods {

	public static class GameObjectExtensions {

		public static T GetOrAddComponent<T>(this GameObject go) where T : Component {
			T component = go.GetComponent<T>();
			if (component != null) return component;
			return go.AddComponent<T>();
		}

	}

}