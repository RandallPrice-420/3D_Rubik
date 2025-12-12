using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Cubelet : MonoBehaviour
{
    public bool inPlay;
    public CubeletDirection direction;
    public CubeletColors color;

}
