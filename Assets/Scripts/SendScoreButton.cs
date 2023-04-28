using System;
using System.Runtime.InteropServices;
using System.Text;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SendScoreButton : MonoBehaviour
{
    
    void Start()
    {
        var button = GetComponent<Button>();
        
        button.OnClickAsObservable()
            .Subscribe(
                async _ =>
                {
                    await SendScore(Random.Range(0, 100));
                }
            )
            .AddTo(this);
    }

    async UniTask SendScore(float score)
    {
        WWWForm form = new WWWForm();
        form.AddField("score", score.ToString());

        using var request = UnityWebRequest.Post("http://219.play.lvh.me:3000/api/v1/scoreboards/1/scores",form);
        request.SetRequestHeader("X-Unityroom-Timestamp", GetUnixTime().ToString());
        request.SetRequestHeader("X-Unityroom-Signature", "1udfwegfuhqwdfqe");
        request.SetRequestHeader("X-Unityroom-Id", SystemInfo.deviceUniqueIdentifier);

        await request.SendWebRequest();
        Debug.Log(request.downloadHandler.text);
    }

    private static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

    public static long GetUnixTime()
    {
        return (long)(DateTime.Now - UnixEpoch).TotalSeconds;
    }

    // UnixTimeからDateTimeへ変換
    public static DateTime GetDateTime(long unixTime)
    {
        return UnixEpoch.AddSeconds(unixTime);
    }
}
