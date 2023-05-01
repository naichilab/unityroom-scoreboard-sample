using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SendScoreButton : MonoBehaviour
{
    [SerializeField]
    private Button Button;

    void Start()
    {
        Button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        var boardNo = 1; //unityroomのスコアボード設定ページにある"ボードNo"を入力してください。
        var authenticationKey = "kr/2mTwV6XQsOPCFqpN3lw=="; //unityroomのスコアボード設定ページにある"認証用シークレット"を入力してください。

        //獲得スコア
        var score = Random.Range(0, 100);

        //スコア送信
        StartCoroutine(SendScore(boardNo, score, authenticationKey));
    }

    IEnumerator SendScore(
        int boardNo
        , float score
        , string authenticationKey
    )
    {
        if (IsEditor())
        {
            Debug.Log($"スコア送信 boardNo={boardNo} score={score} (unityroomにゲームをアップロードすると実際に送信されます)");
            yield break;
        }

        var url = $"/gameplay_api/v1/scoreboards/{boardNo}/scores";
        var scoreText = score.ToString(CultureInfo.InvariantCulture);
        var hmac = SHA256(scoreText, authenticationKey);
        var form = new WWWForm();
        form.AddField("score", scoreText);
        using var request = UnityWebRequest.Post(url, form);
        request.SetRequestHeader("X-Unityroom-Signature", hmac);
        yield return request.SendWebRequest();
        Debug.Log($"{request.responseCode}|{request.downloadHandler.text}");
    }

    private static string SHA256(string text, string key)
    {
        var utf8 = new UTF8Encoding(false); //BOM無しUTF-8
        var textBytes = utf8.GetBytes(text);
        var keyBytes = utf8.GetBytes(key);
        var sha256 = new System.Security.Cryptography.HMACSHA256(keyBytes);
        var hashBytes = sha256.ComputeHash(textBytes);

        //byte型配列を16進数に変換
        var hash = BitConverter.ToString(hashBytes).ToLower().Replace("-", "");
        return hash;
    }

    private bool IsEditor()
    {
#if UNITY_EDITOR
        return true;
#else
        return false;
#endif
    }
}