using UnityEngine;

[System.Serializable]
public struct IntPos{
	public int x,y,z;
	
	public IntPos(int ix, int iy, int iz){
        x=ix;
		y=iy;
		z=iz;
	}
	
	public Vector3 toVec{
		get{
			return new Vector3(x,y,z);
		}
	}
	
	public static IntPos zero = new IntPos(0,0,0);
	
	public static IntPos up = new IntPos(0,1,0);
	
	public static IntPos right = new IntPos(1,0,0);
	
	public static IntPos forward = new IntPos(0,0,1);
	
	public static IntPos one = new IntPos(1,1,1);
	
	//IntPos - IntPos operators
	public static IntPos operator +(IntPos i1,IntPos i2){
		return new IntPos(i1.x+i2.x,i1.y+i2.y,i1.z+i2.z);
	}
	public static IntPos operator -(IntPos i1,IntPos i2){
		return new IntPos(i1.x-i2.x,i1.y-i2.y,i1.z-i2.z);
	}
    public static IntPos operator -(IntPos i1) {
        return new IntPos(-i1.x, -i1.y, -i1.z);
    }
    public static IntPos operator *(IntPos i1,IntPos i2){
		return new IntPos(i1.x*i2.x,i1.y*i2.y,i1.z*i2.z);
	}
	public static IntPos operator /(IntPos i1,IntPos i2){
		return new IntPos(i1.x/i2.x,i1.y/i2.y,i1.z/i2.z);
	}
	public static bool operator ==(IntPos i1,IntPos i2){
		bool r = true;
		
		if(i1.x!=i2.x)
			r=false;
		if(i1.y!=i2.y)
			r=false;
		if(i1.z!=i2.z)
			r=false;
		
		return r;
	}
	public static bool operator !=(IntPos i1,IntPos i2){
		
		if(i1.x!=i2.x)
			return true;
		if(i1.y!=i2.y)
			return true;
		if(i1.z!=i2.z)
			return true;
		
		return false;
	}

    public bool isNegative{
		get{ 
			return x<0 || y<0 || z<0;
		}
	}

    public bool isContained(int min, int max) {
        if (x>=max||x<0)
            return false;
        if (y>=max||y<0)
            return false;
        if (z>=max||z<0)
            return false;
        return true;
    }
	
	//IntPos - Int Operators
	public static IntPos operator +(IntPos i1,int i2){
		return new IntPos(i1.x+i2,i1.y+i2,i1.z+i2);
	}
	public static IntPos operator -(IntPos i1,int i2){
		return new IntPos(i1.x-i2,i1.y-i2,i1.z-i2);
	}
	public static IntPos operator *(IntPos i1,int i2){
		return new IntPos(i1.x*i2,i1.y*i2,i1.z*i2);
	}
	public static IntPos operator /(IntPos i1,int i2){
		return new IntPos(Mathf.FloorToInt((float)i1.x/i2),Mathf.FloorToInt((float)i1.y/i2),Mathf.FloorToInt((float)i1.z/i2));
	}

    public static bool operator >(IntPos i1, int i2) {
        return i1.x>i2||i1.y>i2||i1.z>i2;
    }
    public static bool operator <(IntPos i1, int i2) {
        return i1.x<i2||i1.y<i2||i1.z<i2;
    }
    public static bool operator >=(IntPos i1, int i2) {
        return i1.x>=i2||i1.y>=i2||i1.z>=i2;
    }
    public static bool operator <=(IntPos i1, int i2) {
        return i1.x<=i2||i1.y<=i2||i1.z<=i2;
    }

    public IntPos normalized{
		get{
			return IntPos.Vector3ToIntPos(toVec*1.2f);
		}
	}

	public static float DistanceSqr(IntPos i1, IntPos i2){
		
		return (i1-i2).sqrMagnitude;
	}
	
	public float sqrMagnitude{
		get{
			return (x*x+y*y+z*z);
		}
	}
	
	public static IntPos Vector3ToIntPos(Vector3 input){
		return new IntPos(Mathf.FloorToInt(input.x),Mathf.FloorToInt(input.y),Mathf.FloorToInt(input.z));
	}
    public static IntPos Vector3ToIntPosCeil(Vector3 input) {
        return new IntPos(Mathf.CeilToInt(input.x), Mathf.CeilToInt(input.y), Mathf.CeilToInt(input.z));
    }
    public static IntPos Vector3ToIntPosRound(Vector3 input) {
        return new IntPos(Mathf.RoundToInt(input.x), Mathf.RoundToInt(input.y), Mathf.RoundToInt(input.z));
    }

    public override string ToString ()
	{
		return "IntPos(" + x + "," + y + "," + z + ")";
	}
	public IntPos ToFlatIntPos(){
		return new IntPos(x,0,y);
	}
	public IntPos ToFlatZIntpos(){
		return new IntPos(x,0,z);
	}
}
