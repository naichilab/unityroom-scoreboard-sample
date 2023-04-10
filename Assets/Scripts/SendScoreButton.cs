using System.Runtime.InteropServices;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SendScoreButton : MonoBehaviour
{
    
    void Start()
    {
        var button = GetComponent<Button>();

        button.OnClickAsObservable()
            .Subscribe(
                async _ =>
                {
                    var csrfToken = GetCsrfTokenInternal();
                    
                    WWWForm form = new WWWForm();
                    form.AddField("score", Random.Range(0,100));

                    using var request = UnityWebRequest.Post("/api/v1/scoreboards/5/scores",form);
                    request.SetRequestHeader("X-CSRF-TOKEN", csrfToken);

                    await request.SendWebRequest();
                    Debug.Log(request.responseCode);
                }
            )
            .AddTo(this);
    }

    [DllImport("__Internal")]
    private static extern string GetCsrfTokenInternal();
}
