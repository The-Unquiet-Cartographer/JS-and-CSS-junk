using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using MS.UsingFileUtilities;

namespace MS.UsingMeshUtilities {

	public static class MeshUtilities {
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	//
	//	PROPERTIES
	//
	////////////////////////////////////////////////////////////////////////////////////////////////////
		public static Material defaultMaterial {get{
			if (Application.isEditor) return AssetDatabase.GetBuiltinExtraResource<Material>("Default-Material.mat");
			return Resources.GetBuiltinResource<Material>("Default-Material");
		}}
	
	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	//
	//	FUNCTIONS
	//
	////////////////////////////////////////////////////////////////////////////////////////////////////
		public static Mesh NewMesh (Vector3[] vertices, List<List<int>> triangles_bySubmesh, Vector3[] normals = null) {
			Mesh newMesh = new Mesh();
			newMesh.vertices = vertices;
			newMesh.subMeshCount = triangles_bySubmesh.Count;
			for (int i = 0; i < triangles_bySubmesh.Count; i++) {
				newMesh.SetTriangles(triangles_bySubmesh[i], i);
			}
			if (normals == null) newMesh.RecalculateNormals();
			else newMesh.normals = normals;
			return newMesh;
		}
	
		public static void CreateMeshAsset (Mesh _M, string name = "newMeshAsset") {
			AssetDatabase.CreateAsset(_M, "Assets/"+name+".asset");
		}
		public static void CreateMeshObject (Mesh M = null, Material[] materials = null, string name = "New Mesh Object", bool includeCollider = false) {
			GameObject go = new GameObject(name, typeof(MeshFilter), typeof(MeshRenderer));
			if (includeCollider) go.AddComponent(typeof(MeshCollider));
			if (M != null) {
				go.GetComponent<MeshFilter>().sharedMesh = M;
				if (materials == null) go.GetComponent<MeshRenderer>().material = defaultMaterial;
				else go.GetComponent<MeshRenderer>().sharedMaterials = materials;
				if (includeCollider) go.GetComponent<MeshCollider>().sharedMesh = M;
			}
			return;
		}
	
		public static Mesh GetMeshInSelection () {
			GameObject go = Selection.activeGameObject;
			if (go == null) { Debug.LogWarning("No GameObject in active selection!"); return null; }
			return GetMeshInGameObject(go);
		}
		public static Mesh GetMeshInGameObject (GameObject go) {
			if (go == null) { Debug.LogWarning("GameObject not found!"); return null; }
			MeshFilter mf = go.GetComponentInChildren<MeshFilter>();
			if (mf == null) { Debug.LogWarning("MeshFilter not found!"); return null; }
			Mesh M = mf.sharedMesh;
			if (M == null) { Debug.LogWarning("Mesh not found!"); return null; }
			return M;
		}
	
		public static Mesh CopyMesh (Mesh _M) {
			Mesh newMesh = new Mesh();
			newMesh.vertices = _M.vertices;
			newMesh.normals = _M.normals;
			newMesh.subMeshCount = _M.subMeshCount;
			for (int i = 0; i < _M.subMeshCount; i++) newMesh.SetTriangles(_M.GetTriangles(i), i);
			newMesh.tangents = _M.tangents;
			return newMesh;
		}
	
		public static List<List<int>> GetTrianglesBySubmesh (Mesh _mesh) {
			List<List<int>> trianglesBySubmesh = new List<List<int>>(_mesh.subMeshCount);
			for (int i = 0; i < _mesh.subMeshCount; i++) trianglesBySubmesh.Add(_mesh.GetTriangles(i).ToList());
			return trianglesBySubmesh;
		}



	////////////////////////////////////////////////////////////////////////////////////////////////////
	//
	//	UTILITY FUNCTIONS
	//
	////////////////////////////////////////////////////////////////////////////////////////////////////
		public static void InvertAllTriangles (Mesh _mesh) {
			for (int i = 0; i < _mesh.subMeshCount; i++) {
				int[] submesh = _mesh.GetTriangles(i);
				for (int j = 0; j < submesh.Length; j+=3) {
					int temp = submesh[j+1];
					submesh[j+1] = submesh[j+2];
					submesh[j+2] = temp;
				}
				_mesh.SetTriangles(submesh, i);
			}
		}

	
	
	////////////////////////////////////////////////////////////////////////////////////////////////////
	//
	//	MENU ITEMS - OPERATIONS (MESH UTILITIES)
	//
	////////////////////////////////////////////////////////////////////////////////////////////////////
	
	//
	//	SAVE AS .OBJ ASSET
	//
		[MenuItem("Mesh Utilities/Write selection to .OBJ file", priority = 998)]
		private static void SelectionToObjFile () {
			Mesh M = GetMeshInSelection();
			if (M == null) return;
		//Stringify vertices
			string str_vertices = "", str_normals = "";//, str_uvs = "";
			{
				Vector3[] _v = M.vertices, _n = M.normals;
				//Vector2[] _uv = M.uv;
				for (int i = 0; i < M.vertexCount; i++) {
					str_vertices += "v "+_v[i].x+" "+_v[i].y+" "+_v[i].z+"\n";
					str_normals += "vn "+_n[i].x+" "+_n[i].y+" "+_n[i].z+"\n";
					//str_uvs += "uv "+_uv[i].x+" "+_uv[i].y+" "+_uv[i].z+"\n";
				}
			}
		//stringify faces
			List<string> str_submeshes = new List<string>();
			for (int i = 0; i < M.subMeshCount; i++) {
				str_submeshes.Add("g [Submesh "+i+"]");
				int[] submesh = M.GetTriangles(i);
				for (int j = 0; j < submesh.Length; j+=3) {
					int tvi0 = submesh[j]+1;
					int tvi1 = submesh[j+1]+1;
					int tvi2 = submesh[j+2]+1;
					str_submeshes[i] += "\nf "+tvi0+"/"+tvi0+" "+tvi1+"/"+tvi1+" "+tvi2+"/"+tvi2;
				}
			}
		//Compile all data
			string fileData = "o ["+Selection.activeGameObject.name+"]\n\n"+str_vertices+"\n"+str_normals+"\n";//*str_uvs+"\n";
			foreach (string s in str_submeshes) fileData += s+"\n";
			Debug.Log(fileData);
			FileUtilities.File_WriteContents("Assets/"+Selection.activeGameObject.name+" "+FileUtilities.CreateSaveID()+".obj", fileData);
		}
	
	}
	
	
		
////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	MENU ITEMS - WINDOWS (MESH UTILITIES)
//
////////////////////////////////////////////////////////////////////////////////////////////////////

//
//	CREATE NEW MESH / EDIT SELECTION
//
/*
*	See "Window_MeshEditor.cs"
*	Priority 0
*/



//
//	SAVE AS MESH ASSET (SCALED)
//
	public class Window_SaveScaledMesh : EditorWindow {
		[MenuItem("Mesh Utilities/Save selection as new Mesh asset...", priority = 10)]
		public static void ShowWindow () {
			GetWindow<Window_SaveScaledMesh>("Save Mesh");
		}
	
		GameObject meshObject;
		string overrideName = "";
		public Vector3 scale = Vector3.one;
	
		void Awake () {
			meshObject = Selection.activeGameObject;
		}
	
		void OnGUI () {
			EditorGUIUtility.wideMode = true;
			EditorGUIUtility.labelWidth = 96;
			meshObject = EditorGUILayout.ObjectField("Mesh Object", meshObject, typeof(GameObject), true) as GameObject;
			overrideName = EditorGUILayout.TextField("Override name", overrideName);
			scale = EditorGUILayout.Vector3Field("Scale", scale);
			if (meshObject == null) GUI.enabled = false;
			if (GUILayout.Button("Save new asset")) {
				Mesh M = MeshUtilities.GetMeshInGameObject(meshObject);
				if (M != null) {
					Mesh _M = MeshUtilities.CopyMesh(M);
					Vector3[] _vertices = _M.vertices;
					for (int i = 0; i < _vertices.Length; i++) {
						_vertices[i].Scale(scale);
					}
					_M.vertices = _vertices;
					string str_scale = "";
					if (scale.x == scale.y && scale.x == scale.z) str_scale = " scale="+scale.x;
					else {
						if (scale.x != 1) str_scale += " x="+scale.x;
						if (scale.y != 1) str_scale += " y="+scale.y;
						if (scale.z != 1) str_scale += " z="+scale.z;
					}
					MeshUtilities.CreateMeshAsset(_M, (overrideName == "" ? meshObject.name : overrideName)+""+str_scale+" "+FileUtilities.CreateSaveID());
				}
			}
			GUI.enabled = true;
		}
	}
	
	
	
//
//	COMBINE MESHES
//
/*
*	See "Window_CombineMeshes.cs"
*	Priority 20
*/



//
//	INVERT TRIANGLES
//
	public class Window_InvertTriangleTool : EditorWindow {
		[MenuItem("Mesh Utilities/Invert triangles tool...", priority = 800)]
		public static void ShowWindow () {
			GetWindow<Window_InvertTriangleTool>("Invert triangles");
		}
	
		GameObject meshObject;
		Mesh mesh;
	
		void Awake () {
			meshObject = Selection.activeGameObject;
			if (meshObject != null) mesh = MeshUtilities.GetMeshInGameObject(meshObject);
		}
	
		void OnGUI () {
			meshObject = EditorGUILayout.ObjectField("Mesh Object", meshObject, typeof(GameObject), true) as GameObject;
			if (meshObject != null) mesh = MeshUtilities.GetMeshInGameObject(meshObject);
			else GUI.enabled = false;
			if (GUILayout.Button("Invert All")) {
				MeshUtilities.InvertAllTriangles(mesh);
				mesh.RecalculateNormals();
			}
			GUI.enabled = true;
		}
	}
	
	
	
//
//	LOG MESH DATA
//
	public class Window_LogMeshData : EditorWindow {
		[MenuItem("Mesh Utilities/Log Mesh data...", priority = 999)]
		public static void ShowWindow () {
			GetWindow<Window_LogMeshData>("Log Mesh data");
		}
	
		GameObject meshObject;
		bool
			vertices_local,
			vertices_world,
			normals_local,
			normals_world,
			submeshCount,
			triangleIndices,
			uvs
		;
	
		void Awake () {
			meshObject = Selection.activeGameObject;
		}
	
		void OnGUI () {
			meshObject = EditorGUILayout.ObjectField("Mesh Object", meshObject, typeof(GameObject), true) as GameObject;
	
			EditorGUILayout.BeginHorizontal();
			vertices_local = EditorGUILayout.Toggle("Vertices (local)", vertices_local);
			vertices_world = EditorGUILayout.Toggle("Vertices (world)", vertices_world);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			normals_local = EditorGUILayout.Toggle("Normals (local)", normals_local);
			normals_world = EditorGUILayout.Toggle("Normals (world)", normals_world);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			submeshCount = EditorGUILayout.Toggle("Submesh count", submeshCount);
			triangleIndices = EditorGUILayout.Toggle("Triangle indices", triangleIndices);
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			uvs = EditorGUILayout.Toggle("UVs", uvs);
			EditorGUILayout.EndHorizontal();
	
			if (meshObject == null) GUI.enabled = false;
			if (GUILayout.Button("Log Mesh data")) {
				Vector3[] _vertices;
				Vector3[] _normals;
				int[][] triangles_bySubmesh;
				Vector2[] _uvs;
	
				Mesh M = MeshUtilities.GetMeshInGameObject(meshObject);
				string str = "";
	
				if (vertices_local) {
					_vertices = M.vertices;
					str += "<b>Vertices (local):</b>";
					for (int i = 0; i < _vertices.Length; i++) str += "\n"+i+".\t"+_vertices[i];
					str += "\n\n";
				}
	
				if (vertices_world) {
					_vertices = M.vertices;
					str += "<b>Vertices (world):</b>";
					for (int i = 0; i < _vertices.Length; i++) str += "\n"+i+".\t"+meshObject.transform.TransformPoint(_vertices[i]);
					str += "\n\n";
				}
	
	
				if (normals_local) {
					_normals = M.normals;
					str += "<b>Normals (local):</b>";
					for (int i = 0; i < _normals.Length; i++) str += "\n"+i+".\t"+_normals[i];
					str += "\n\n";
				}
	
				if (normals_world) {
					_normals = M.normals;
					str += "<b>Normals (world):</b>";
					for (int i = 0; i < _normals.Length; i++) str += "\n"+i+".\t"+meshObject.transform.TransformVector(_normals[i]);
					str += "\n\n";
				}

				if (submeshCount) {
					str += "<b>Submesh count:</b> "+M.subMeshCount+"\n\n";
				}

				if (triangleIndices) {
					str += "<b>Triangle Indices:</b>";
					triangles_bySubmesh = new int[M.subMeshCount][];
					for (int i = 0; i < M.subMeshCount; i++) {
						str += "\n<b>Submesh "+i+"</b>";
						triangles_bySubmesh[i] = M.GetTriangles(i);
						int numOfTris = triangles_bySubmesh[i].Length/3;
						for (int j = 0; j < numOfTris; j++) {
							int j3 = j*3;
							str += "\n"+j+". ("+j3+")\t"+triangles_bySubmesh[i][j3]+", "+triangles_bySubmesh[i][j3+1]+", "+triangles_bySubmesh[i][j3+2];
						}
					}
					str += "\n\n";
				}

				if (uvs) {
					_uvs = M.uv;
					str += "<b>UVs:</b>";
					for (int i = 0; i < _uvs.Length; i++) str += "\n"+i+".\t"+_uvs[i];
					str += "\n\n";
				}

				str.Remove(str.Length-2);
				Debug.Log(str);
			}
			GUI.enabled = true;
		}
	}

}