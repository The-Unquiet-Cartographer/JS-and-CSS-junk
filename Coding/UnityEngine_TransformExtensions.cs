using UnityEngine

namespace MS.UsingExtension Methods {

	public static class TransformExtensions {
	
		public static bool ContainsType<T> (this IEnumerable<Transform> transforms) {
			foreach (Transform t in transforms) {
				if (t.GetComponent<T>() != null) return true;
			}
			return false;
		}

		public static int GetDepth (this Transform t) {
			int depth = 0;
			while (t.parent != null) {
				depth++;
				t = t.parent;	
			}
			return depth;
		}
	
		public static IEnumerable<Transform> GetDirectChildren (this Transform t) {
			foreach (Transform child in t) {
				yield return child;
			}
		}
		public static IEnumerable<Transform> GetDirectChildren (this Transform t, System.Func<Transform, bool> predicate, bool includeThis = false) {
			if (includeThis && predicate(t)) yield return t;
			foreach (Transform child in t) {
				if (predicate(child)) yield return child;
			}
		}
		public static IEnumerable<T> GetDirectChildrenOfType<T> (this Transform t, bool includeThis = false) where T : Component {
			if (includeThis) {
				T component = t.GetComponent<T>();
				if (component != null) yield return component;
			}
			foreach (Transform child in t) {
				T component = child.GetComponent<T>();
				if (component != null) yield return component;
			}
		}

		public static Transform FindRecursive (this Transform t, System.Func<Transform, bool> predicate, bool includeThis = false) {
			if (includeThis && predicate(t)) return t;
			foreach (Transform child in t) {
				if (predicate(child)) return child;
				child.FindRecursive(predicate, false);
			}
			return null;
		}
		public static IEnumerable<Transform> FindAllRecursive (this Transform t, System.Func<Transform, bool> predicate, bool includeThis = false) {
			if (includeThis && predicate(t)) yield return t;
			foreach (Transform child in t) {
				foreach (Transform recursiveChild in child.FindAllRecursive(predicate, true)) yield return recursiveChild;
			}
		}
		public static IEnumerable<T> FindAllOfTypeRecursive<T> (this Transform t, bool includeThis = false) {
			if (includeThis) {
				T component = t.GetComponent<T>();
				if (component != null) yield return component;
			}
			foreach (Transform child in t) {
				foreach (T recursiveComponent in child.FindAllOfTypeRecursive<T>(true)) yield return recursiveComponent;
			}
		}

	}

}