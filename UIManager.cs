using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    //Panelを格納する変数
    //インスペクターウィンドウからゲームオブジェクトを設定する
    [SerializeField] GameObject ChangeScenePanel;
    public static int CurrentPanel;
    public static int CurrentScene;
    private bool judgePanel = false;

    //アップデート関数が読み込まれる前の処理
    void Start()
    {
        //ChangeScenePanelを非アクティブにする
        ChangeScenePanel.SetActive(false);

        // プレイヤーに初期角度を与える
        this.transform.eulerAngles = GlobalVariable.G_PlayerRot;

        // flag変数がtrueの時-> シームレスなシーン移動の時
        if (GlobalVariable.G_flag)
        {
            // プレイヤーに初期ポジションも与える
            this.transform.position = GlobalVariable.G_PlayerPos;
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

        if (judgePanel) // チェンジシーンパネルに当たった時に起動する
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

    //ある特定の壁(tag=ChangeSceneTag)に衝突した時の処理
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

    // シームレスなシーン移動
    // ゲームタグ"DoubleDoor"に衝突した時の処理
    public void OnTriggerEnter(Collider other)
    {
        // DoubleDoorがついたオブジェクトに衝突した時（当たり判定はないため、すり抜ける）
        if (other.CompareTag("DoubleDoor"))
        {
            // 衝突した壁の名前をCurrentPanelに格納する（壁には次のシーンのIDを既に振っておく）
            string CurrentPanel_1 = other.gameObject.name;
            // 念のため、string型で値を格納し、後でint型に変形させる
            CurrentPanel = int.Parse(CurrentPanel_1);

            // 現在のシーンの名前をCurrentSceneに格納する
            string CurrentScene_1 = SceneManager.GetActiveScene().name;
            // 念のため、string型で値を取得し、後でint型に変形させる
            CurrentScene = int.Parse(CurrentScene_1);

            // transformを取得
            Transform playerrot = this.transform;

            // グローバル変数に壁の回転角度を代入
            GlobalVariable.G_PlayerRot = playerrot.eulerAngles;

            // プレイヤーのpositionをグローバル変数に格納
            Transform PlayerTrans = this.transform;
            GlobalVariable.G_PlayerPos = PlayerTrans.position;

            // フラグ変数をtrueにする
            GlobalVariable.G_flag = true;

            //次のSceneを読み込む
            SceneManager.LoadScene(CurrentPanel.ToString());
        }
    }
}
