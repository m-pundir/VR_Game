using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class BrushManager : MonoBehaviour
{
    [Range(2, 30)]
    public int BrushAngleNum = 20;      //브러쉬 둥근 정도(클수록 둥글다 + 느려짐)
    [Range(0.001f, 0.05f)]
    private float m_BrushRadius = 0.01f;   //브러쉬 지름
    private float m_BrushRadiusMin = 0.001f;
    private float m_BrushRadiusMax = 0.05f;
    public bool CanDrawing = false;   //그리기 가능 여부
    public bool ColorPickerUIUsing = false;
    public bool m_UseGravity = true;
    public bool m_useBloom = true;

    private Vector3 brush_PrePose;
    private float brush_MinDistance = 0.005f;

    private BrushMesh current_brush = null;              //현재 브러쉬
    private Stack<BrushMesh> brush_stack;  //브러쉬 그린 내역 저장
    private Stack<BrushMesh> Undo_stack; //되돌리기 스택
    public Material DefaultBrushMaterial;      //브러쉬 메터리얼
    public Material BloomBrushMaterial;

    private GameObject brush_Parent = null;
    private const string parent_name = "BrushObject";     //부모 이름
    private const string stack_name = "BrushStack";

    private Vector3 minBoxColliderPosition;
    private Vector3 maxBoxColliderPosition;
    private Stack<Vector3> vMinBoxColliderPosition;
    private Stack<Vector3> vMaxBoxColliderPosition;

    private int itemCount = 0;
    private int drawingCount = 0;
    private void Awake()
    {
        brush_stack = new Stack<BrushMesh>();
        Undo_stack = new Stack<BrushMesh>();
        vMinBoxColliderPosition = new Stack<Vector3>();
        vMaxBoxColliderPosition = new Stack<Vector3>();
        CreateBrushStackParent_C();

        m_UseGravity = true;
        LayerManager.instance.AddLayerMask("brushItem");
    }


    public void DrawingBrush(Vector3 vDrawingPosition, float brushRadius)
    {
        //브러쉬 없으면 초기화
        InitBrush(vDrawingPosition, brushRadius);

        //그리기
        if (CanDrawing)
        {
            //박스 콜리더 계산을 위한 드로잉 카운트
            float dis = Vector3.Distance(vDrawingPosition, brush_PrePose);
            if (dis < brush_MinDistance)
                return;

            drawingCount++;
            current_brush.AddMesh(vDrawingPosition, brushRadius);
            SetBoxColliderPivot(vDrawingPosition);
            brush_PrePose = vDrawingPosition;
            
        }
        else
        {
            EndBrush();
            
        }
            
    }

    private void InitBrush(Vector3 initPosition, float brushRadius)
    {
        if (CanDrawing) return;
        //if (current_brush != null) return;
        Material brushMaterial = null;

        if(m_useBloom)
        {
            brushMaterial = BloomBrushMaterial;
        }
        else
        {
            brushMaterial = DefaultBrushMaterial;
        }

        if (brushMaterial == null) return;

        //진동!
        //Debug.Log("InitBrush");
        //브러쉬 생성
        GameObject newBrushObj = new GameObject(stack_name + brush_stack.Count);
        //부모 설정
        newBrushObj.transform.SetParent(brush_Parent.transform);

        //컴포넌트 추가
        newBrushObj.AddComponent<BrushMesh>();
        current_brush = newBrushObj.GetComponent<BrushMesh>();
        current_brush.CreateBrushMesh(
            brushMaterial,
            stack_name + brush_stack.Count,
            initPosition,
            BrushAngleNum,
            brushRadius);


        //첫번째 브러쉬 위치로 브러쉬 최소값과 최대값 위치 입력
        if (minBoxColliderPosition == Vector3.zero && maxBoxColliderPosition == Vector3.zero)
        {
            minBoxColliderPosition = initPosition;
            maxBoxColliderPosition = initPosition;
        }
        else
        {
            SetBoxColliderPivot(initPosition);
        }

        //스택에 넣기
        brush_stack.Push(current_brush);
        CanDrawing = true;
        brush_PrePose = initPosition;
    }

    public void EndBrush()
    {
        PushBoxCollider();
        current_brush = null;
    }

    private void SetBoxColliderPivot(Vector3 pos)
    {
        if (minBoxColliderPosition == Vector3.zero && maxBoxColliderPosition == Vector3.zero)
        {
            minBoxColliderPosition = brush_PrePose;
            maxBoxColliderPosition = brush_PrePose;
        }


        if (minBoxColliderPosition.x > pos.x)
        {
            minBoxColliderPosition.x = pos.x;
        }
        if (minBoxColliderPosition.y > pos.y)
        {
            minBoxColliderPosition.y = pos.y;
        }
        if (minBoxColliderPosition.z > pos.z)
        {
            minBoxColliderPosition.z = pos.z;
        }

        if (maxBoxColliderPosition.x < pos.x)
        {
            maxBoxColliderPosition.x = pos.x;
        }
        if (maxBoxColliderPosition.y < pos.y)
        {
            maxBoxColliderPosition.y = pos.y;
        }
        if (maxBoxColliderPosition.z < pos.z)
        {
            maxBoxColliderPosition.z = pos.z;
        }

        if (drawingCount >= 5)
        {
            PushBoxCollider();
        }
    }

    private void PushBoxCollider()
    {
        if (!CanDrawing)
            return;

        vMinBoxColliderPosition.Push(minBoxColliderPosition);
        vMaxBoxColliderPosition.Push(maxBoxColliderPosition);
        minBoxColliderPosition = Vector3.zero;
        maxBoxColliderPosition = Vector3.zero;
        drawingCount = 0;
    }

    public void DeleteAllDrawingStack()
    {
        int stackCount = brush_Parent.transform.childCount;

        for (int i = 0; i < stackCount; i++)
        {
            Destroy(brush_Parent.transform.GetChild(i).gameObject);
        }

        brush_stack.Clear();
    }

    public void DrawingUndoFunction()
    {
        if (brush_stack.Count <= 0)
            return;


        BrushMesh undoStack = brush_stack.Pop();
        Undo_stack.Push(undoStack);
        undoStack.gameObject.SetActive(false);
    }

    public void DrawingRedoFunction()
    {
        if (Undo_stack.Count <= 0)
            return;

        BrushMesh redoStack = Undo_stack.Pop();
        brush_stack.Push(redoStack);
        redoStack.gameObject.SetActive(true);
    }

    public void UndoStackClear()
    {
        if (Undo_stack.Count < 0)
            return;

        while (Undo_stack.Count > 0)
        {
            BrushMesh pop = Undo_stack.Pop();
            Destroy(pop.gameObject);
        }
    }

    public void CreateBrushStackParent()
    {
        if (brush_Parent)
        {
            brush_Parent.GetComponent<Rigidbody>().useGravity = true;

            float sizeX = Mathf.Abs(maxBoxColliderPosition.x - minBoxColliderPosition.x);
            float sizeY = Mathf.Abs(maxBoxColliderPosition.y - minBoxColliderPosition.y);
            float sizeZ = Mathf.Abs(maxBoxColliderPosition.z - minBoxColliderPosition.z);

            float centerX = (maxBoxColliderPosition.x + minBoxColliderPosition.x) / 2;
            float centerY = (maxBoxColliderPosition.y + minBoxColliderPosition.y) / 2;
            float centerZ = (maxBoxColliderPosition.z + minBoxColliderPosition.z) / 2;

            BoxCollider brushBox = brush_Parent.GetComponent<BoxCollider>();
            brushBox.size = new Vector3(sizeX, sizeY, sizeZ);
            brushBox.center = new Vector3(centerX, centerY, centerZ);
            brushBox.isTrigger = false;

        }
        
        brush_Parent = new GameObject(parent_name + "_" + itemCount);
        brush_Parent.AddComponent<BoxCollider>();
        brush_Parent.GetComponent<BoxCollider>().isTrigger = true;


        Rigidbody bp = brush_Parent.AddComponent<Rigidbody>();
        bp.useGravity = false;

        minBoxColliderPosition = Vector3.zero;
        maxBoxColliderPosition = Vector3.zero;

        brush_stack.Clear();

        itemCount++;
        
    }

    public void CreateBrushStackParent_B()
    {
        if (brush_Parent)
        {
            brush_Parent.GetComponent<Rigidbody>().useGravity = m_UseGravity;

            BrushMeshMerge();
            brush_Parent.AddComponent<MeshCollider>();
            brush_Parent.GetComponent<MeshCollider>().convex = true;
            brush_Parent.GetComponent<MeshCollider>().isTrigger = true;

            //brush_Parent.AddComponent<PickUpItem>();
            //brush_Parent.GetComponent<PickUpItem>().gravitySettingValue = m_UseGravity;
        }
        /*
        brush_Parent = new GameObject(parent_name + "_" + itemCount);

        Rigidbody bp = brush_Parent.AddComponent<Rigidbody>();
        bp.useGravity = m_UseGravity;

        minBoxColliderPosition = Vector3.zero;
        maxBoxColliderPosition = Vector3.zero;

        brush_stack.Clear();

        itemCount++;
        */
    }

    public void CreateBrushStackParent_C()
    {
        if (brush_Parent)
        {
            brush_Parent.GetComponent<Rigidbody>().useGravity = m_UseGravity;
            GameObject boxColliderObj = new GameObject("BoxColliders");
            boxColliderObj.transform.SetParent(brush_Parent.transform);

            while (vMinBoxColliderPosition.Count > 0)
            {
                Vector3 min = vMinBoxColliderPosition.Pop();
                Vector3 max = vMaxBoxColliderPosition.Pop();

                float sizeX = Mathf.Abs(max.x - min.x);
                float sizeY = Mathf.Abs(max.y - min.y);
                float sizeZ = Mathf.Abs(max.z - min.z);

                if (sizeX < m_BrushRadius * 2)
                    sizeX = m_BrushRadius * 2;

                if (sizeY < m_BrushRadius * 2)
                    sizeY = m_BrushRadius * 2;
                
                if (sizeZ < m_BrushRadius * 2)
                    sizeZ = m_BrushRadius * 2;

                float centerX = (max.x + min.x) / 2;
                float centerY = (max.y + min.y) / 2;
                float centerZ = (max.z + min.z) / 2;

                BoxCollider brushBox = boxColliderObj.AddComponent<BoxCollider>();
                brushBox.size = new Vector3(sizeX, sizeY, sizeZ);
                brushBox.center = new Vector3(centerX, centerY, centerZ);
                brushBox.isTrigger = false;
            }

            vMinBoxColliderPosition.Clear();
            vMaxBoxColliderPosition.Clear();
        }

        brush_Parent = new GameObject(parent_name + "_" + itemCount);

        Rigidbody bp = brush_Parent.AddComponent<Rigidbody>();
        bp.useGravity = false;

        minBoxColliderPosition = Vector3.zero;
        maxBoxColliderPosition = Vector3.zero;

        brush_stack.Clear();

        itemCount++;
    }


    private void BrushMeshMerge()
    {
        MeshFilter meshFilter = brush_Parent.AddComponent<MeshFilter>();
        meshFilter.mesh.Clear();

        int stackCount = brush_stack.Count;

        MeshFilter[] meshFilters = new MeshFilter[stackCount];
        for (int i = 0; i < stackCount; i++)
        {
            meshFilters[i] = brush_stack.Pop().GetComponent<MeshFilter>();
        }

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int j = 0;
        int cj = 0;
        while (j < meshFilters.Length)
        {
            if (meshFilter != meshFilters[j])
            {
                combine[cj].mesh = meshFilters[j].sharedMesh;
                combine[cj].transform = meshFilters[j].transform.localToWorldMatrix;
                cj++;
            }
            //meshFilters[j].gameObject.SetActive(false);
            j++;
        }
        meshFilter.mesh.CombineMeshes(combine);
    }

    public void RadiusSizeUp()
    {
        m_BrushRadius += 0.001f;
        if (m_BrushRadius > m_BrushRadiusMax)
            m_BrushRadius = m_BrushRadiusMax;
    }

    public void RadiusSizeDown()
    {
        m_BrushRadius -= 0.001f;
        if (m_BrushRadius < m_BrushRadiusMin)
            m_BrushRadius = m_BrushRadiusMin;
    }

    public float GetRadius()
    {
        return m_BrushRadius;
    }
}
