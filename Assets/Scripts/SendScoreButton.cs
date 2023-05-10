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
    [SerializeField]
    private int BoardNo;

    void Start()
    {
        Button.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        //獲得スコア
        var score = Random.Range(0f, 100f);
        var authenticationKey
            = "Dts7JwXskWwF371olNr4I2VR6MS7vSE48o2g9Rl1c3piSH76cP2ZCSBruVhyjyzttmKAV46wy2kU/5tTA4qmoQ==";

        //スコア送信
        StartCoroutine(SendScore(BoardNo, score, authenticationKey));
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

        // 送信するスコアを文字列に変換しておく
        var scoreText = score.ToString(CultureInfo.InvariantCulture);

        // 現在のUNIXTIMEを取得
        var unixtime = GetCurrentUnixTime()
            .ToString();

        // 認証用のHMAC(Hash-based Message Authentication Code)を計算する
        var hmacDataText = $"{unixtime}:{scoreText}";
        var hmac = GetHmacSha256(hmacDataText, authenticationKey);

        // APIリクエストを送信する
        // スコアはFormDataとして付与する
        // 認証用の情報はリクエストヘッダーに付与する
        var uri = $"/gameplay_api/v1/scoreboards/{boardNo}/scores";
        var form = new WWWForm();
        form.AddField("score", scoreText);
        using var request = UnityWebRequest.Post(uri, form);
        request.SetRequestHeader("X-Unityroom-Signature", hmac);
        request.SetRequestHeader("X-Unityroom-Timestamp", unixtime);
        yield return request.SendWebRequest();

        //結果をログに表示
        Debug.Log($"{request.responseCode}|{request.downloadHandler.text}");
    }

    /// <summary>
    /// SHA256を用いてHMACを計算する
    /// </summary>
    /// <param name="dataText">計算対象文字列</param>
    /// <param name="base64AuthenticationKey">HMACSHA256に用いるキー（base64）</param>
    /// <returns>HMAC SHA-256</returns>
    private static string GetHmacSha256(string dataText, string base64AuthenticationKey)
    {
        var dataBytes = new UTF8Encoding(false).GetBytes(dataText); // BOM無しUTF-8のbyte配列に変換
        var keyBytes = Convert.FromBase64String(base64AuthenticationKey); // base64の認証用キーをデコードしてbyte配列に変換
        var sha256 = new System.Security.Cryptography.HMACSHA256(keyBytes); // ハッシュ関数はSHA256を用いる
        var hmacBytes = sha256.ComputeHash(dataBytes); // ハッシュ値を計算
        var hmacText = BitConverter.ToString(hmacBytes)
            .ToLower()
            .Replace("-", ""); // byte型配列を16進数の文字列に変換
        return hmacText;
    }

    /// <summary>
    /// 現在時刻のUnixTimeを返す
    /// </summary>
    /// <returns></returns>
    public static int GetCurrentUnixTime()
    {
        return (int)(DateTime.UtcNow.Subtract(DateTime.UnixEpoch)).TotalSeconds;
    }

    /// <summary>
    /// Unityエディタで実行中かどうか
    /// </summary>
    /// <returns></returns>
    private bool IsEditor()
    {
#if UNITY_EDITOR
        return true;
#else
        return false;
#endif
    }
}