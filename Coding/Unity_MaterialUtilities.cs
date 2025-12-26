using UnityEngine;
using UnityEditor;
using MS.UsingFileUtilities;

namespace MS.UsingMaterialUtilities {

	public static class MaterialUtilities {

		[MenuItem("Assets/Create/Material from image", priority = 30)]
		public static void CreateMaterialFromImage () {
			if (!FileUtilities.SelectedFileInProjectFolder()) return;
			for (int i = 0; i < Selection.objects.Length; i++) {
				string fullPath = AssetDatabase.GetAssetPath(Selection.objects[i]);
				string folderPath, fileName, fileExtension;
				FileUtilities.SplitPathComponents(fullPath, out folderPath, out fileName, out fileExtension);
				if (
					fileExtension != ".png"
				&&	fileExtension != ".bmp"
				&&	fileExtension != ".jpeg"
				) {
					Debug.Log(fileName+fileExtension+" has an invalid extension. Valid extensions are: .png .bmp .jpeg");
					continue;
				}
				Texture T = AssetDatabase.LoadAssetAtPath<Texture>(fullPath) as Texture;
				Material newMat = new Material(Shader.Find("Standard"));
				newMat.mainTexture = T;
				newMat.SetFloat("_Glossiness", 0);
				newMat.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
				newMat.SetInt("_SpecularHighlights", 0);
				newMat.EnableKeyword("_GLOSSYREFLECTIONS_OFF");
				newMat.SetInt("_GlossyReflections", 0);
			/*
			*	https://forum.unity.com/threads/set-smoothness-of-material-in-script.381247/
			*	https://discussions.unity.com/t/reflection-and-specular-highlights-how-to-turn-on-off-help-pls/173114
			*/
				AssetDatabase.CreateAsset(newMat, folderPath+"/"+fileName+".mat");
			}
		}

		public static Material defaultMaterial {get{
			if (Application.isEditor) return AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
			return Resources.GetBuiltinResource<Material>("Default-Material");
		}}

		public static Material LoadMaterial (string materialName) {
			materialName = materialName.Split('.')[0];
			Material mat = Resources.Load<Material>("Materials/"+materialName);															//<== Must not end with any extension
			if (mat != null) return mat;
			materialName += ".mat";
			mat = AssetDatabase.LoadAssetAtPath<Material>(FileUtilities.FileUtilities.Asset_GetPath(materialName)) as Material;			//<== Must end with .mat
			if (mat != null) return mat;
			Debug.LogWarning("Material "+materialName+" not found!");
			return MaterialUtilities.defaultMaterial;
		}

	}

}
