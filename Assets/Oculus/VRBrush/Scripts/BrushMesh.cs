using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushMesh : MonoBehaviour
{
    /*
    //매쉬
    private Mesh mesh = new Mesh();
    private GameObject mesh_obj;

    //매쉬 속성
    private List<Vector3> vertices = new List<Vector3>();
    private List<int> tri_indexes = new List<int>();
    private List<Vector2> uvs = new List<Vector2>();
    */
    //매쉬
    private Mesh mesh;
    private GameObject mesh_obj;

    //매쉬 속성
    private List<Vector3> vertices;
    private List<int> tri_indexes;
    private List<Vector2> uvs;

    //설정 값
    private int angle_num = 8;
    private float radius = 0.2f;
    private const float min_dist = 0.005f;

    //최소/최대값
    private const int angle_min = 3, angle_max = 30;
    private const float radius_min = 0.001f, radius_max = 500.0f;

    //기록 값
    private bool first_mesh = true;
    private Vector3 prev_pose0 = Vector3.zero;
    private Vector3 prev_pose1 = Vector3.zero;
    private Vector3 prev_normal = Vector3.zero;

    //생성자
    public void CreateBrushMesh(Material _matrial, string _name, Vector3 _pose, int _angle_num = 8, float _radius = 0.2f)
    {
        //매쉬 생성
        mesh = new Mesh();
        mesh.name = _name;

        vertices = new List<Vector3>();
        tri_indexes = new List<int>();
        uvs = new List<Vector2>();

        //오브젝트 초기화
        mesh_obj = this.gameObject;
        mesh_obj.transform.position = Vector3.zero;
        mesh_obj.transform.rotation = Quaternion.identity;
        mesh_obj.transform.localScale = Vector3.one;
        mesh_obj.AddComponent<MeshFilter>().mesh = mesh;
        mesh_obj.AddComponent<MeshRenderer>().sharedMaterial = new Material(_matrial);

        //값 초기화 - 최대/최소 제한
        angle_num = Mathf.Clamp(_angle_num, angle_min, angle_max);
        radius = Mathf.Clamp(_radius, radius_min, radius_max);

        //점 및 삼각형 인덱스 초기화
        AddVertex();
        AddVertex();
        AddTriangleIndex();
        AddFace();

        //값 초기화2
        prev_pose0 = _pose;
        prev_pose1 = _pose;

        ApplyMesh();
    }

    //오브젝트 반환
    public GameObject GetMeshObj()
    {
        return mesh_obj;
    }

    //매쉬 반환
    public Mesh GetMesh()
    {
        return mesh;
    }

    //매쉬 추가 여부
    public bool IsDrawing()
    {
        return !first_mesh;
    }

    //매쉬 추가
    public void AddMesh(Vector3 _pose, float _brushRadius)
    {
        //이전값과의 거리가 일정 거리 이하이면 실행X
        float distance = Vector3.Distance(_pose, prev_pose0);
        if (distance < min_dist) return;

        //면 추가
        AddFace();

        //반지름
        radius = Mathf.Clamp(_brushRadius, radius_min, radius_max);

        //계산용
        Vector3 normal = Vector3.zero;
        Vector3 direction = Vector3.zero;

        //첫 매쉬일 경우
        if (first_mesh)
        {
            //각도 계산
            direction = _pose - prev_pose0;
            float angleUp = Vector3.Angle(direction, Vector3.up);

            //노말 계산
            if (angleUp < 10.0f || angleUp > 170.0f)
                normal = Vector3.Cross(direction, Vector3.right);
            else
                normal = Vector3.Cross(direction, Vector3.up);

            normal = normal.normalized;
            prev_normal = normal;
        }
        //그 외
        else
        {
            //노말 계산
            Vector3 prev_perp = Vector3.Cross(prev_pose0 - prev_pose1, prev_normal);
            normal = Vector3.Cross(prev_perp, _pose - prev_pose0).normalized;
        }

        //Vertex 갱신
        if (first_mesh)
            UpdateVertex(4, prev_pose0);

        UpdateVertex(1, _pose);
        UpdateVertex(2, _pose, _pose - prev_pose0, normal);
        UpdateVertex(3, prev_pose0, _pose - prev_pose1, prev_normal);

        //이전 값 저장
        prev_pose1 = prev_pose0;
        prev_pose0 = _pose;
        prev_normal = normal;

        //매쉬 적용
        ApplyMesh();

        first_mesh = false;
    }

    #region 점 및 면의 위치, index 계산
    //면 추가 = 점 + 삼각형 index 추가
    private void AddFace()
    {
        AddVertex();
        AddTriangleIndex();
    }

    //점 추가
    private void AddVertex()
    {
        for (int i = 0; i < angle_num; i++)
        {
            vertices.Add(Vector3.zero);
            uvs.Add(new Vector2(i / (angle_num - 1.0f), 0f));
        }
    }

    //삼각형 index 추가
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

    //Vertex값 갱신
    //끝에서 몇번째 인덱스인지 / 위치 / 방향 / 노말값 / 크기
    private void UpdateVertex(int _face_index, Vector3 _pose, Vector3 _dir, Vector3 _normal)
    {
        Vector3 dir = _dir.normalized;
        Vector3 normal = _normal.normalized;

        //끝에서 몇번째 면인지 찾기
        int start_index = vertices.Count - (angle_num * _face_index);
        for (int i = 0; i < angle_num; i++)
        {
            //점 위치 계산
            float angle = 360.0f * (i / (float)angle_num);
            Quaternion rotator = Quaternion.AngleAxis(angle, dir);
            Vector3 result_anlge = rotator * normal * radius;
            //점 추가
            vertices[start_index + i] = _pose + result_anlge;
        }
    }
    private void UpdateVertex(int _face_index, Vector3 _pose)
    {
        //끝에서 몇번째 면인지 찾기
        int start_index = vertices.Count - (angle_num * _face_index);
        //점 추가
        for (int i = 0; i < angle_num; i++)
            vertices[start_index + i] = _pose;

    }
    #endregion

    #region 매쉬 관련
    //매쉬 적용
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

    //매쉬 갱신
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
