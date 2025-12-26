using UnityEngine;
using System.Collections.Generic;

public struct Triangle {
	public Vector3 ptA, ptB, ptC;
	public Triangle (Vector3 ptA, Vector3 ptB, Vector3 ptC) {
		this.ptA = ptA; this.ptB = ptB; this.ptC = ptC;
	}
	public Triangle (Vector3[] _meshVertices, int[] _meshTriangles, int triangleIndex0) {
		this.ptA = _meshVertices[_meshTriangles[triangleIndex0]];
		this.ptB = _meshVertices[_meshTriangles[triangleIndex0+1]];
		this.ptC = _meshVertices[_meshTriangles[triangleIndex0+2]];
	}

	public struct Edge {
		public Vector3 ptA, ptB;
		public Edge (Vector3 ptA, Vector3 ptB) {
			this.ptA = ptA;
			this.ptB = ptB;
		}
	}

//PROPERTIES
	public Vector3 normal {get{return CalculateNormal(ptA, ptB, ptC);}}

	public Vector3 centroid {get{return CalculateCentroid(ptA, ptB, ptC);}}

//FUNCTIONS

	public bool ContainsPt(Vector3 findPt) {
		return ContainsPt(findPt, ptA, ptB, ptC);
	}

	public bool SharesEdge (Triangle other) {
		if (this.ptA == other.ptA) {
			if (this.ptB == other.ptB) return true;
			if (this.ptB == other.ptC) return true;
			if (this.ptC == other.ptB) return true;
			if (this.ptC == other.ptC) return true;
		}
		if (this.ptB == other.ptA) {
			if (this.ptA == other.ptB) return true;
			if (this.ptA == other.ptC) return true;
			if (this.ptC == other.ptB) return true;
			if (this.ptC == other.ptC) return true;
		}
		if (this.ptC == other.ptA) {
			if (this.ptA == other.ptB) return true;
			if (this.ptA == other.ptC) return true;
			if (this.ptB == other.ptB) return true;
			if (this.ptB == other.ptC) return true;
		}
		return false;
	}
/// <summary>
/// Returns a string describing which pts are shared. ABBC would mean that this.A == other.B, and this.B == other.C.
/// </summary>
	public bool SharesEdge (Triangle other, out string pts) {
		if (this.ptA == other.ptA) {
			pts = "AA";
			if (this.ptB == other.ptB) {pts += "BB"; return true;}
			if (this.ptB == other.ptC) {pts += "BC"; return true;}
			if (this.ptC == other.ptB) {pts += "CB"; return true;}
			if (this.ptC == other.ptC) {pts += "CC"; return true;}
		}
		if (this.ptB == other.ptA) {
			pts = "BA";
			if (this.ptA == other.ptB) {pts += "AB"; return true;}
			if (this.ptA == other.ptC) {pts += "AC"; return true;}
			if (this.ptC == other.ptB) {pts += "CB"; return true;}
			if (this.ptC == other.ptC) {pts += "CC"; return true;}
		}
		if (this.ptC == other.ptA) {
			pts = "CA";
			if (this.ptA == other.ptB) {pts += "AB"; return true;}
			if (this.ptA == other.ptC) {pts += "AC"; return true;}
			if (this.ptB == other.ptB) {pts += "BB"; return true;}
			if (this.ptB == other.ptC) {pts += "BC"; return true;}
		}
		pts = "";
		return false;
	}

//STATIC FUNCTIONS
	public static Vector3 CalculateNormal (Vector3 ptA, Vector3 ptB, Vector3 ptC) {
		return Vector3.Cross(ptB - ptA, ptC - ptB).normalized;
	}

	public static Vector3 CalculateCentroid (Vector3 ptA, Vector3 ptB, Vector3 ptC) {
		return (ptA + ptB + ptC) * 0.3333333f;
	}

	public static bool ContainsPt (Vector3 findPt, Vector3 ptA, Vector3 ptB, Vector3 ptC) {
		if (findPt == ptA || findPt == ptB || findPt == ptC) return true;

		Vector3 normal = CalculateNormal(ptA, ptB, ptC);
	//Is pt within angle A?
		{
			Vector3 AB = ptB-ptA;
			Vector3 AC = ptC-ptA;
			Vector3 AV = findPt-ptA;
			float a_to_b = Vector3.SignedAngle(AB,AC, normal);
			float a_to_v = Vector3.SignedAngle(AB,AV, normal);
			if (a_to_v != Mathf.Clamp(a_to_v, 0, a_to_b)) return false;
		}
	//Is pt within angle B?
		{
			Vector3 BC = ptC-ptB;
			Vector3 BA = ptA-ptB;
			Vector3 BV = findPt-ptB;
			float b_to_c = Vector3.SignedAngle(BC,BA, normal);
			float b_to_v = Vector3.SignedAngle(BC,BV, normal);
			if (b_to_v != Mathf.Clamp(b_to_v, 0, b_to_c)) return false;
		}
	//Is pt within angle C?
		{
			Vector3 CA = ptA-ptC;
			Vector3 CB = ptB-ptC;
			Vector3 CV = findPt-ptC;
			float c_to_a = Vector3.SignedAngle(CA,CB, normal);
			float c_to_v = Vector3.SignedAngle(CA,CV, normal);
			if (c_to_v != Mathf.Clamp(c_to_v, 0, c_to_a)) return false;
		}
		return true;
	}

	public static List<Triangle> TrianglesFromMesh (Mesh mesh) {
		List<Triangle> l = new List<Triangle>(mesh.triangles.Length/3);
		for (int i = 0; i < mesh.triangles.Length; i+=3) {
			l.Add(new Triangle(
				mesh.vertices[mesh.triangles[i]],
				mesh.vertices[mesh.triangles[i+1]],
				mesh.vertices[mesh.triangles[i+2]]
			));
		}
		return l;
	}

}
