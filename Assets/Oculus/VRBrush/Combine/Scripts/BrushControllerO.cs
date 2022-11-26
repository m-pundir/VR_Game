using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Valve.VR;

public enum VR_CONTROLLER_TYPE
{
    pc,
    oculus,
    vive,
    none,
}

public class BrushControllerO : MonoBehaviour
{
    public VR_CONTROLLER_TYPE controllerType;
    private VRControllerTriggerValue m_Trigger;
    private LineRenderer rayRenderer;

    [Range(2, 30)]
    public int BrushAngleNum = 20;      //브러쉬 둥근 정도(클수록 둥글다 + 느려짐)
    [Range(0.001f, 0.05f)]
    public float m_BrushRadius = 0.01f;   //브러쉬 지름
    public float m_BrushRadiusMin = 0.001f;
    public float m_BrushRadiusMax = 0.05f;
    public bool CanDrawing = false;   //그리기 가능 여부
    public Material BrushMaterial;      //브러쉬 메터리얼
    public Transform tfBrushPoint; //브러쉬 드로잉 포인트 위치

    private Vector3 brush_PrePose;
    private float brush_MinDistance = 0.005f;

    [SerializeField]
    private BrushMesh current_brush = null;              //현재 브러쉬
    private Stack<BrushMesh> brush_stack;  //브러쉬 그린 내역 저장
    private Stack<BrushMesh> Undo_stack; //되돌리기 스택

    private const string parent_name = "BrushObject";     //부모 이름
    private const string stack_name = "BrushStack";

    private GameObject brush_Parent = null;
    private int itemCount = 0;
    private Vector3 minBoxColliderPosition;
    private Vector3 maxBoxColliderPosition;
    private Stack<Vector3> vMinBoxColliderPosition;
    private Stack<Vector3> vMaxBoxColliderPosition;
    private int drawingCount = 0;

    //vive
    //public SteamVR_Action_Boolean m_TrackPadClick;
    //public SteamVR_Action_Single m_Squeeze;
    //public SteamVR_Action_Boolean m_SnapTurnRight;
    //public SteamVR_Action_Boolean m_SnapTurnLeft;

    private void Awake()
    {
        rayRenderer = this.GetComponent<LineRenderer>();
        if (rayRenderer)
        {
            rayRenderer.startWidth = 0.001f;
            rayRenderer.endWidth = 0.001f;
        }

        brush_stack = new Stack<BrushMesh>();
        Undo_stack = new Stack<BrushMesh>();
        vMinBoxColliderPosition = new Stack<Vector3>();
        vMaxBoxColliderPosition = new Stack<Vector3>();
        CreateBrushStackParent_C();

        GameObject goBrushPoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        goBrushPoint.name = "BrushPoint";
        goBrushPoint.transform.SetParent(this.transform);
        tfBrushPoint = goBrushPoint.transform;

        goBrushPoint.transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        goBrushPoint.transform.localPosition = new Vector3(0f, 0f, 0.0625f);

        m_Trigger = this.GetComponent<VRControllerTriggerValue>();

        //Debug.Log("Loaded VR Device : " + UnityEngine.XR.XRSettings.loadedDeviceName);
        //Device Detecting if Oculus
        if(UnityEngine.XR.XRSettings.loadedDeviceName == "Oculus")
        {
            controllerType = VR_CONTROLLER_TYPE.oculus;
        }
        else
        {
            //OpenVr(Vive)
            controllerType = VR_CONTROLLER_TYPE.vive;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //pc
        if(controllerType == VR_CONTROLLER_TYPE.pc)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);


            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                if (Input.GetMouseButton(0))
                {
                    if (hit.transform.GetComponent<HueTexture>())
                    {
                        HueTexture hue = hit.transform.GetComponent<HueTexture>();
                        hue.HueUpdate(hit.point);
                    }
                    else if (hit.transform.GetComponent<SatTexture>())
                    {
                        SatTexture sat = hit.transform.GetComponent<SatTexture>();
                        sat.SatUpdate(hit.point);
                    }
                }
            }
            else
            {
                if(Input.GetMouseButton(0))
                {
                    Vector3 vMouse = Input.mousePosition;
                    vMouse.z = 1f;
                    Vector3 vPos = Camera.main.ScreenToWorldPoint(vMouse);
                    DrawingBrush(vPos, m_BrushRadius);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    CanDrawing = false;
                    EndBrush();
                }
            }
        }
        //Oculus or Vive
        else if(controllerType == VR_CONTROLLER_TYPE.oculus || controllerType == VR_CONTROLLER_TYPE.vive)
        {
            RaycastHit hit;
            Ray ray = new Ray(this.transform.position, this.transform.forward);

            int layerMask = 1 << LayerMask.NameToLayer("VRBrushUI");

            m_Trigger.SetTriggerValue(0f);

            if (controllerType == VR_CONTROLLER_TYPE.oculus)
                m_Trigger.SetTriggerValue(OVRInput.Get(OVRInput.Axis1D.SecondaryIndexTrigger));
            //else if (controllerType == VR_CONTROLLER_TYPE.vive)
            //    m_Trigger.SetTriggerValue(m_Squeeze.GetAxis(SteamVR_Input_Sources.RightHand));

            if (Physics.Raycast(ray, out hit, 50f, layerMask))
            {
                rayRenderer.enabled = true;
                rayRenderer.SetPosition(0, this.transform.position);
                rayRenderer.SetPosition(1, hit.point);
            }
            else
            {
                rayRenderer.enabled = false;

                if (m_Trigger.GetTriggerValue() > 0.1f)
                {
                    if (Undo_stack.Count > 0)
                        UndoStackClear();

                    float brushRadius = m_BrushRadius * m_Trigger.GetTriggerValue();
                    DrawingBrush(tfBrushPoint.position, brushRadius);
                }
                else if(m_Trigger.GetTriggerValue() <= 0f)
                {
                    EndBrush();
                    CanDrawing = false;
                }
            }

            Vector2 secondaryThumbstickValue = Vector2.zero;

            if (controllerType == VR_CONTROLLER_TYPE.oculus)
                secondaryThumbstickValue = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
            else if (controllerType == VR_CONTROLLER_TYPE.vive)
            {
                //if (m_SnapTurnRight.GetState(SteamVR_Input_Sources.RightHand))
                //    secondaryThumbstickValue = new Vector2(1f, 0);
                //else if (m_SnapTurnLeft.GetState(SteamVR_Input_Sources.RightHand))
                //    secondaryThumbstickValue = new Vector2(-1f, 0);
            }


            if(secondaryThumbstickValue.x > 0)
            {
                m_BrushRadius += 0.001f;
                if (m_BrushRadius > m_BrushRadiusMax)
                    m_BrushRadius = m_BrushRadiusMax;

                tfBrushPoint.transform.localScale = new Vector3(m_BrushRadius * 2, m_BrushRadius * 2, m_BrushRadius * 2);
            }
            else if(secondaryThumbstickValue.x < 0)
            {
                m_BrushRadius -= 0.001f;
                if (m_BrushRadius < m_BrushRadiusMin)
                    m_BrushRadius = m_BrushRadiusMin;

                tfBrushPoint.transform.localScale = new Vector3(m_BrushRadius * 2, m_BrushRadius * 2, m_BrushRadius * 2);
            }
        }
    }
    private void InitBrush(Vector3 initPosition, float brushRadius)
    {
        if (CanDrawing) return;
        //if (current_brush != null) return;
        if (BrushMaterial == null) return;

        //진동!
        Debug.Log("InitBrush");
        //브러쉬 생성
        GameObject newBrushObj = new GameObject(stack_name + brush_stack.Count);
        //부모 설정
        newBrushObj.transform.SetParent(brush_Parent.transform);

        //컴포넌트 추가
        newBrushObj.AddComponent<BrushMesh>();
        current_brush = newBrushObj.GetComponent<BrushMesh>();
        current_brush.CreateBrushMesh(
            BrushMaterial,
            stack_name + brush_stack.Count,
            initPosition,
            BrushAngleNum,
            brushRadius);


        //첫번째 브러쉬 위치로 브러쉬 최소값과 최대값 위치 입력
        if(minBoxColliderPosition == Vector3.zero && maxBoxColliderPosition == Vector3.zero)
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

    private void DrawingBrush(Vector3 vDrawingPosition, float brushRadius)
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
            EndBrush();
    }

    private void EndBrush()
    {
        PushBoxCollider();
        current_brush = null;
    }

    public void CreateBrushStackParent()
    {
        if(brush_Parent)
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

        brush_Parent = new GameObject(parent_name + "_" +itemCount);
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
            brush_Parent.GetComponent<Rigidbody>().useGravity = true;

            BrushMeshMerge();
            brush_Parent.AddComponent<MeshCollider>();
            brush_Parent.GetComponent<MeshCollider>().convex = true;
        }

        brush_Parent = new GameObject(parent_name + "_" + itemCount);

        Rigidbody bp = brush_Parent.AddComponent<Rigidbody>();
        bp.useGravity = false;

        minBoxColliderPosition = Vector3.zero;
        maxBoxColliderPosition = Vector3.zero;

        brush_stack.Clear();

        itemCount++;
    }

    public void CreateBrushStackParent_C()
    {
        if (brush_Parent)
        {
            brush_Parent.GetComponent<Rigidbody>().useGravity = true;
            GameObject boxColliderObj = new GameObject("BoxColliders");
            boxColliderObj.transform.SetParent(brush_Parent.transform);

            while (vMinBoxColliderPosition.Count > 0)
            {
                Vector3 min = vMinBoxColliderPosition.Pop();
                Vector3 max = vMaxBoxColliderPosition.Pop();

                float sizeX = Mathf.Abs(max.x - min.x);
                float sizeY = Mathf.Abs(max.y - min.y);
                float sizeZ = Mathf.Abs(max.z - min.z);

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


    public void DeleteAllDrawingStack()
    {
        int stackCount = brush_Parent.transform.childCount;

        for(int i = 0; i < stackCount; i++)
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

    private void UndoStackClear()
    {
        while(Undo_stack.Count > 0)
        {
            BrushMesh pop = Undo_stack.Pop();
            Destroy(pop.gameObject);
        }
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
        if(minBoxColliderPosition.y > pos.y)
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

        if(drawingCount >= 5)
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

    private void BrushMeshMerge()
    {
        MeshFilter meshFilter = brush_Parent.AddComponent<MeshFilter>();
        meshFilter.mesh.Clear();

        int stackCount = brush_stack.Count;
        
        MeshFilter[] meshFilters = new MeshFilter[stackCount];
        for(int i = 0; i < stackCount; i++)
        {
            meshFilters[i] = brush_stack.Pop().GetComponent<MeshFilter>();
        }

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int j = 0;
        int cj = 0;
        while( j < meshFilters.Length)
        {
            if(meshFilter != meshFilters[j])
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

}
