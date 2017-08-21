using System.Collections;
using System.Collections.Generic;
using ProcedualLevels.Models;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ProcedualLevels.Views
{
    public class SceneHelper
    {
        public static IObservable<Unit> ChangeSceneAsync(string sceneName)
        {
            var subject = new Subject<Unit>();

            SceneManager.LoadScene("Root");
            SceneManager.activeSceneChanged += (prev, next) =>
            {
                subject.OnNext(Unit.Default);
                subject.OnCompleted();
            };

            return subject;
        }

        public static IObservable<IAdventureView> ChangeToRootScene()
        {
            return Observable.FromCoroutine<IAdventureView>(observer => SetAdventureUp(observer));            
        }

        private static IEnumerator SetAdventureUp(IObserver<IAdventureView> result)
        {
            yield return ChangeSceneAsync("Root").ToYieldInstruction();
            
            var viewPrefab = Resources.Load<Views.GameManager>("Prefabs/Manager/GameManager");
            IAdventureView view = Object.Instantiate(viewPrefab);

            yield return Observable.NextFrame();
            result.OnNext(view);
            result.OnCompleted();
        }
    }
}