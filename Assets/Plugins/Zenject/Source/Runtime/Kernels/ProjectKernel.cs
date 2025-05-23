#if !NOT_UNITY3D

using System.Collections.Generic;
using System.Linq;
using ModestTree;
using UnityEngine.SceneManagement;

namespace Zenject
{
    public class ProjectKernel : MonoKernel
    {
        [Inject]
        ZenjectSettings _settings = null;

        [Inject]
        SceneContextRegistry _contextRegistry = null;

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        // manually unload the scenes in the reverse order they were loaded before going to
        // the new scene, if you require a predictable destruction order.  Or you can always use
        // ZenjectSceneLoader which will do this for you
        public void OnApplicationQuit()
        {
            if (_settings.EnsureDeterministicDestructionOrderOnApplicationQuit)
            {
                DestroyEverythingInOrder();
            }
        }

        public void DestroyEverythingInOrder()
        {
            ForceUnloadAllScenes(true);

            // Destroy project context after all scenes
            Assert.That(!IsDestroyed);
            DestroyImmediate(gameObject);
            Assert.That(IsDestroyed);
        }

        public void ForceUnloadAllScenes(bool immediate = false)
        {
            // OnApplicationQuit should always be called before OnDestroy
            // (Unless it is destroyed manually)
            Assert.That(!IsDestroyed);

            var sceneOrder = new List<Scene>();

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                sceneOrder.Add(SceneManager.GetSceneAt(i));
            }

            // Destroy the scene contexts from bottom to top
            // Since this is the reverse order that they were loaded in
            foreach (var sceneContext in _contextRegistry.SceneContexts.OrderByDescending(x => sceneOrder.IndexOf(x.gameObject.scene)).ToList())
            {
                if (immediate)
                {
                    DestroyImmediate(sceneContext.gameObject);
                }
                else
                {
                    Destroy(sceneContext.gameObject);
                }
            }
        }
    }
}

#endif
