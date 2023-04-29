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
        var scoreBoardId = 1;
        var authenticationKey = "kr/2mTwV6XQsOPCFqpN3lw==";
        var score = Random.Range(0, 100);

        //スコア送信
        StartCoroutine(SendScore(scoreBoardId, score, authenticationKey));
    }

    IEnumerator SendScore(
        int scoreboardId
        , float score
        , string authenticationKey
    )
    {
        if (IsEditor())
        {
            Debug.Log($"スコア送信 scoreboardId={scoreboardId} score={score} (unityroomにゲームをアップロードすると実際に送信されます)");
            yield break;
        }

        var url = $"/api/v1/scoreboards/{scoreboardId}/scores";
        var scoreText = score.ToString(CultureInfo.InvariantCulture);
        var hmac = SHA1(scoreText, authenticationKey);
        var form = new WWWForm();
        form.AddField("score", scoreText);
        using var request = UnityWebRequest.Post(url, form);
        request.SetRequestHeader("X-Unityroom-Signature", hmac);
        yield return request.SendWebRequest();
        var log
            = $"{(request.result == UnityWebRequest.Result.Success ? request.responseCode.ToString() : request.error)} : {request.downloadHandler.text}";
        Debug.Log(log);
    }

    private static string SHA1(string text, string key)
    {
        var utf8 = new UTF8Encoding();
        var planeBytes = utf8.GetBytes(text);
        var keyBytes = utf8.GetBytes(key);
        var sha1 = new System.Security.Cryptography.HMACSHA1(keyBytes);
        var hashBytes = sha1.ComputeHash(planeBytes);
        return hashBytes.Aggregate("", (current, b) => current + $"{b,0:x2}");
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