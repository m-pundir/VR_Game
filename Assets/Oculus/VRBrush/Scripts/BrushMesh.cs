using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushMesh : MonoBehaviour
{
    /*
    //
    private Mesh mesh = new Mesh();
    private GameObject mesh_obj;

    //
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> tri_indexes = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    */
    //
    private Mesh mesh;
    private GameObject mesh_obj;

    //mesh properties
    private List<Vector3> vertices;
    private List<int> tri_indexes;
    private List<Vector2> uvs;

    //set value
    private int angle_num = 8;
    private float radius = 0.2f;
    private const float min_dist = 0.005f;

    //min/max value
    private const int angle_min = 3, angle_max = 30;
    private const float radius_min = 0.001f, radius_max = 500.0f;

    //record value
    private bool first_mesh = true;
    private Vector3 prev_pose0 = Vector3.zero;
    private Vector3 prev_pose1 = Vector3.zero;
    private Vector3 prev_normal = Vector3.zero;

    //constructor
    public void CreateBrushMesh(Material _matrial, string _name, Vector3 _pose, int _angle_num = 8, float _radius = 0.2f)
    {
        //create mash
        mesh = new Mesh();
        mesh.name = _name;

        vertices = new List<Vector3>();
        tri_indexes = new List<int>();
        uvs = new List<Vector2>();

        //object initialization
        mesh_obj = this.gameObject;
        mesh_obj.transform.position = Vector3.zero;
        mesh_obj.transform.rotation = Quaternion.identity;
        mesh_obj.transform.localScale = Vector3.one;
        mesh_obj.AddComponent<MeshFilter>().mesh = mesh;
        mesh_obj.AddComponent<MeshRenderer>().sharedMaterial = new Material(_matrial);

        //Reset Values ​​- Max/Min Limits
        angle_num = Mathf.Clamp(_angle_num, angle_min, angle_max);
        radius = Mathf.Clamp(_radius, radius_min, radius_max);

        //Initialize point and triangle indices

        AddVertex();
        AddVertex();
        AddTriangleIndex();
        AddFace();

        //Initialize value 2

        prev_pose0 = _pose;
        prev_pose1 = _pose;

        ApplyMesh();
    }

    //return object

    public GameObject GetMeshObj()
    {
        return mesh_obj;
    }

    //
    public Mesh GetMesh()
    {
        return mesh;
    }

    //Whether to add mash

    public bool IsDrawing()
    {
        return !first_mesh;
    }

    //addmesh
    public void AddMesh(Vector3 _pose, float _brushRadius)
    {
        //Run X if the distance from the previous value is less than a certain distance
        float distance = Vector3.Distance(_pose, prev_pose0);
        if (distance < min_dist) return;

        //add face
        AddFace();

        //radius
        radius = Mathf.Clamp(_brushRadius, radius_min, radius_max);

        // for calculation
        Vector3 normal = Vector3.zero;
        Vector3 direction = Vector3.zero;

        // if this is the first mesh
        if (first_mesh)
        {
            //calculate the angle
            direction = _pose - prev_pose0;
            float angleUp = Vector3.Angle(direction, Vector3.up);

            //calculate normal
            if (angleUp < 10.0f || angleUp > 170.0f)
                normal = Vector3.Cross(direction, Vector3.right);
            else
                normal = Vector3.Cross(direction, Vector3.up);

            normal = normal.normalized;
            prev_normal = normal;
        }
        else
        {
            //calculate normal
            Vector3 prev_perp = Vector3.Cross(prev_pose0 - prev_pose1, prev_normal);
            normal = Vector3.Cross(prev_perp, _pose - prev_pose0).normalized;
        }

        //update vertex
        if (first_mesh)
            UpdateVertex(4, prev_pose0);

        UpdateVertex(1, _pose);
        UpdateVertex(2, _pose, _pose - prev_pose0, normal);
        UpdateVertex(3, prev_pose0, _pose - prev_pose1, prev_normal);

        //save the previous value

        prev_pose1 = prev_pose0;
        prev_pose0 = _pose;
        prev_normal = normal;

        ApplyMesh();

        first_mesh = false;
    }

    #region Position of point and plane, index calculation

    // Add face = Add point + triangle index
    private void AddFace()
    {
        AddVertex();
        AddTriangleIndex();
    }

    // add dot
    private void AddVertex()
    {
        for (int i = 0; i < angle_num; i++)
        {
            vertices.Add(Vector3.zero);
            uvs.Add(new Vector2(i / (angle_num - 1.0f), 0f));
        }
    }

    // add triangle index
    private void AddTriangleIndex()
    {
        int last_index = vertices.Count - 1;
        for (int i = 0; i < angle_num; i++)
        {
            int index_0 = last_index - i;
            int index_1 = last_index - ((i + 1) % angle_num);

            tri_indexes.Add(index_0);
            tri_indexes.Add(index_1 - angle_num);
            tri_indexes.Add(index_0 - angle_num);

            tri_indexes.Add(index_0);
            tri_indexes.Add(index_1);
            tri_indexes.Add(index_1 - angle_num);
        }
    }

    //update vertex value
    // index from the end / position / direction / normal value / size
    private void UpdateVertex(int _face_index, Vector3 _pose, Vector3 _dir, Vector3 _normal)
    {
        Vector3 dir = _dir.normalized;
        Vector3 normal = _normal.normalized;

        //find the number of faces from the end
        int start_index = vertices.Count - (angle_num * _face_index);
        for (int i = 0; i < angle_num; i++)
        {
            // Calculate point position
            float angle = 360.0f * (i / (float)angle_num);
            Quaternion rotator = Quaternion.AngleAxis(angle, dir);
            Vector3 result_anlge = rotator * normal * radius;
            // add dot
            vertices[start_index + i] = _pose + result_anlge;
        }
    }
    private void UpdateVertex(int _face_index, Vector3 _pose)
    {
        //find the number of faces from the end
        int start_index = vertices.Count - (angle_num * _face_index);
        // add dot
        for (int i = 0; i < angle_num; i++)
            vertices[start_index + i] = _pose;

    }
    #endregion

    #region mash related
    private void ApplyMesh()
    {
        if (mesh != null)
        {
            mesh.SetVertices(vertices);
            mesh.SetUVs(0, uvs);
            mesh.SetIndices(tri_indexes.ToArray(), MeshTopology.Triangles, 0);
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }
    }

    public void UpdateMesh()
    {
        if (mesh != null)
        {
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }
    }
    #endregion
}
