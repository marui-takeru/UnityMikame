using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManagerUAV : MonoBehaviour
{
    [SerializeField] GameObject[] fbx;
    public static string NextPanel;
    public GameObject LastFBXName;
    [SerializeField] GameObject ChangeScenePanel;
    [SerializeField] GameObject Minimap;
    public static int CurrentPanel;
    public static int CurrentScene;
    private bool judgePanel = false;
    public GameObject InitialFBX;
    public GameObject ChangeFBXWall;
    public GameObject[] objects;
    [SerializeField] GameObject[] Hint;
    public bool Hint_Bool = false;
    private string prevSceneName; 

    void Start()
    {

        // タグが "Untagged" または "Player" であるオブジェクトをリストから除外
        objects = GameObject.FindObjectsOfType<GameObject>();

        // ChangeScenePanelを非アクティブにする
        ChangeScenePanel.SetActive(false);

        // fbx番号を代入
        string sceneName = GlobalVariable.G_fbxNum;
        prevSceneName = RelativeSceneNumInformation.GetSceneName(SceneManager.GetActiveScene().name);
        prevSceneName = prevSceneName.Substring(0, 4);

        // 鳥瞰シーンから来た時の処理
        if (GlobalVariable.G_branch)
        {
            this.transform.position = GlobalVariable.G_PlayerPos;
            this.transform.position += new Vector3(0f, 100f, 0f);
        }
        else// ChangeSceneを渡ってきた時の処理
        {
            string activeSceneName = "_" + SceneManager.GetActiveScene().name;
            sceneName = RelativeSceneNumInformation.GetSceneNum(activeSceneName);
            // プレイヤーに初期角度を与える
            this.transform.eulerAngles = GlobalVariable.G_PlayerRot;
        }

        foreach (GameObject obj in objects)
        {
            if (obj.name.Contains(sceneName))
            {
                // 鳥瞰シーンのシーン名を含む場合
                if (obj.name.Contains(prevSceneName))
                {
                    obj.SetActive(true);

                    foreach (GameObject hintObject in Hint)
                    {
                        hintObject.SetActive(Hint_Bool);
                    }
                }
                // 鳥瞰シーンのシーン名を含まない場合
                else
                {
                    ChangeFBXWall = obj;
                    ChangeFBXWall.SetActive(false);
                }
            }
            // オブジェクト名の最後3文字が異なる場合、オブジェクトを非アクティブにする
            else if (obj.name.Contains(prevSceneName))
            {
                obj.SetActive(false);
            }
        }
    }

    void Update()
    {
        // "esc"キーが押されたら"Restart"シーンに強制移行
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Restart");
        }

        // "P"キーが押されたら"200"シーンに強制移行
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene("200");
        }

        // "M"キーでMinimapのオンオフを切り替える
        if (Input.GetKeyDown(KeyCode.M))
        {
            // Minimapの現在の状態を取得
            bool isMinimapActive = Minimap.activeSelf;

            // Minimapの状態を逆に切り替える
            Minimap.SetActive(!isMinimapActive);
        }

        // "H"キーでヒントのオンオフを切り替える
        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach (GameObject hintObject in Hint)
            {
                hintObject.SetActive(!Hint_Bool);
            }

            Hint_Bool = !Hint_Bool;
            GlobalVariable.G_HintObj = Hint_Bool;
        }

        if (judgePanel) // チェンジシーンパネルに当たった時起動する
        {
            // ゲームパットで緑のボタンが押されたら
            if (Input.GetButtonDown("Fire3"))
            {
                YES_Description();
                Debug.Log("Fire3");
            }
            if (Input.GetButtonDown("Fire1"))
            {
                YES_Description();
            }

            // ゲームパットで赤のボタンが押されたら
            if (Input.GetButtonDown("Fire2"))
            {
                NO_Description();
                Debug.Log("Fire2");
            }
        }
    }

    // トリガーコライダーとの衝突を処理
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ChangeFBXTag"))
        {
            // 衝突したトリガーコライダーの名前をNextPanelに格納する
            NextPanel = other.gameObject.name.Substring(0, 3);

            // 次のFBXを読み込む
            LoadNextFBX();
        }
    }

    private void LoadNextFBX()
    {
        foreach (GameObject obj in objects)
        {
            if (obj.name.Contains(NextPanel))
            {
                if (obj.name.Contains(prevSceneName))
                {
                    // 鳥瞰シーンのシーン名を含む場合
                    obj.SetActive(true);
                    Debug.Log(obj.name);
                }
                else
                {
                    // 鳥瞰シーンのシーン名を含まない場合
                    ChangeFBXWall = obj;
                    ChangeFBXWall.SetActive(false);
                }
            }
            // オブジェクト名の最後3文字が異なる場合、オブジェクトを非アクティブにする
            else if (obj.name.Contains(prevSceneName))
            {
                obj.SetActive(false);
            }
            else
            {
                obj.SetActive(true);
                ChangeScenePanel.SetActive(false);
            }
        }

        foreach (GameObject hintObject in Hint)
        {
            hintObject.SetActive(Hint_Bool);
        }
    }


    //次のSceneを読み込む準備
    public void OnControllerColliderHit(ControllerColliderHit collision)
    {
        //ChangeSceneTagがついたオブジェクトに衝突した時
        if (collision.gameObject.tag == "ChangeSceneTag")
        {
            //ChangeScenePanelをアクティブにする
            ChangeScenePanel.SetActive(true);

            judgePanel = true; // ゲームパットのボタンインプットを可能にする

            //衝突した壁の名前をCurrentPanelに格納する（壁には次のシーンのIDを既に振っておく）
            string CurrentPanel_1 = collision.gameObject.name;
            //念のため、string型で値を格納し、後でint型に変形させる
            CurrentPanel = int.Parse(CurrentPanel_1);

            //現在のシーンの名前をCurrentSceneに格納する
            string CurrentScene_1 = SceneManager.GetActiveScene().name;
            //念のため、string型で値を取得し、後でint型に変形させる
            CurrentScene = int.Parse(CurrentScene_1);

            // transformを取得
            Transform playerrot = this.transform;

            // グローバル変数に壁の回転角度を代入
            GlobalVariable.G_PlayerRot = playerrot.eulerAngles;

            GlobalVariable.G_flag = false;
        }
    }

    //ChangeScenePanelでYESが押下された時の処理
    public void YES_Description()
    {
        //シーン間の移動時間を加算　＊参照している値は、現在のシーンID、次に移動する場所のシーンID
        TimerManager.Timer += EachSceneDistanceInformation.SceneDistance[CurrentScene, CurrentPanel];

        //ChangeScenePanelを非アクティブにする
        ChangeScenePanel.SetActive(false);

        judgePanel = false; // ゲームパットのボタンインプットを無効にする

        GlobalVariable.G_branch = false;

        //次のSceneを読み込む
        SceneManager.LoadScene(CurrentPanel.ToString());
    }

    //ChangeScenePanelでNOが押下された時の処理
    public void NO_Description()
    {
        //ChangeScenePanelを非アクティブにする
        ChangeScenePanel.SetActive(false);
        //現在のシーンに戻る

        judgePanel = false; // ゲームパットのボタンインプットを無効にする
    }
}
