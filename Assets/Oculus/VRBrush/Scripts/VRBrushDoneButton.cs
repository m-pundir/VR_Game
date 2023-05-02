using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VRBrushDoneButton : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    private VRControllerTriggerValue VRController;
    private BrushManager m_BrushManager;

    private Button btn;
    private bool isPressButton = false;

    public Transform ONE, TWO, THREE, FOUR;

    float time = 0f;
    float timeDelay = 2f;

    private void Start()
    {
        time = 0f;
        timeDelay = 2f;

        isPressButton = false;

        VRController = FindObjectOfType<VRControllerTriggerValue>();
        m_BrushManager = VRController.gameObject.GetComponent<BrushManager>();
        btn = this.GetComponent<Button>();
    }
    
    private void Update()
    {
        if (m_BrushManager.CanDrawing == true)
            return;
        /*
        time += 1f * Time.deltaTime;
        if (time < timeDelay)
        {
            return;
        }
        */

        ray = new Ray(VRController.transform.position, VRController.transform.forward);

        if (this.GetComponent<BoxCollider>().Raycast(ray, out hit, 50f))
        {
            btn.image.color = Color.yellow;
            if (VRController.GetTriggerValue() > 0.1f)
            {
                isPressButton = true;
                btn.image.color = Color.green;
            }
            else if (isPressButton)
            {
                time = 0f;

                isPressButton = false;
                m_BrushManager.CreateBrushStackParent_B();
                m_BrushManager.CreateBrushStackParent_C();
                btn.image.color = Color.white;

                StartCoroutine(UpdateMessageWithDelay());
            }
        }
        else
        {
            isPressButton = false;
            btn.image.color = Color.white;
        }
    }

    private IEnumerator UpdateMessageWithDelay()
    {
        yield return new WaitForSeconds(0.25f);
        //here we switch turns.
        //first team1, then team 2,
        //then team 1 again and then team 2
        if (m_BrushManager.itemCount == 1)
        {
            VRGameManager.instance.Team1Drawings.Add(GameObject.Find("BrushObject_0"));
            VRGameManager.instance.Team1Drawings[0].transform.SetParent(ONE);
            VRGameManager.instance.Team1Drawings[0].transform.localPosition = new Vector3(0, 0, 0);
            VRGameManager.instance.SetGameState(GameState.TeamTwoDrawing);
        }
        if (m_BrushManager.itemCount == 2)
        {
            VRGameManager.instance.Team2Drawings.Add(GameObject.Find("BrushObject_1"));
            VRGameManager.instance.Team2Drawings[0].transform.SetParent(TWO);
            VRGameManager.instance.Team2Drawings[0].transform.localPosition = new Vector3(0, 0, 0);
            VRGameManager.instance.SetGameState(GameState.TeamOneDrawing);
        }
        if (m_BrushManager.itemCount == 3)
        {
            VRGameManager.instance.Team1Drawings.Add(GameObject.Find("BrushObject_2"));
            VRGameManager.instance.Team1Drawings[1].transform.SetParent(THREE);
            VRGameManager.instance.Team1Drawings[1].transform.localPosition = new Vector3(0, 0, 0);
            VRGameManager.instance.SetGameState(GameState.TeamTwoDrawing);
        }
        if (m_BrushManager.itemCount == 4)
        {
            VRGameManager.instance.Team2Drawings.Add(GameObject.Find("BrushObject_3"));
            VRGameManager.instance.Team2Drawings[1].transform.SetParent(FOUR);
            VRGameManager.instance.Team2Drawings[1].transform.localPosition = new Vector3(0, 0, 0);
            //VRGameManager.instance.SetGameState(GameState.TeamTwoDrawing);
        }
        VRUIManager.instance.UpdateMessage();
        m_BrushManager.FinalStep();

        if (m_BrushManager.itemCount >4)
        {
            VRGameManager.instance.SetGameState(GameState.Results);
            SceneManager.LoadScene(VRScenes.Results.ToString());
        }


    }
}
