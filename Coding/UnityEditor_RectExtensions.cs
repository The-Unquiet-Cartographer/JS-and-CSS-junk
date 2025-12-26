using UnityEditor;

namespace MS.UsingEditorExtensions {

	public static class RectExtensions {

		public static Rect MoveHorizontal (this Rect r, float xShift) {
			return new Rect(r.x + xShift, r.y, r.width, r.height);
		}

		public static Rect MoveHorizontal (this Rect r, float xShift, float newWidth) {
			return new Rect(r.x + xShift, r.y, newWidth, r.height);
		}

		public static Rect MoveVertical (this Rect r, float yShift) {
			return new Rect(r.x, r.y + yShift, r.width, r.height);
		}

		public static Rect MoveVertical (this Rect r, float yShift, float newHeight) {
			return new Rect(r.x, r.y + yShift, r.width, newHeight);
		}

	}

}
