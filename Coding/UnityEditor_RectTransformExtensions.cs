using UnityEditor;

namespace MS.UsingEditorExtensions {

	public static class RectTransformExtensions {

		public static RectTransform SetX (this RectTransform rt, float x) {
			rt.anchoredPosition = new Vector2(x, rt.anchoredPosition.y);
			return rt;
		}
		public static RectTransform SetY (this RectTransform rt, float y) {
			rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);
			return rt;
		}

	}

}