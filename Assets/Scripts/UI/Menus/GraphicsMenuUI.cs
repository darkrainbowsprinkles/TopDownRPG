using UnityEngine;
using RPG.Graphics;
using RPG.Utils;

namespace RPG.UI.Menus
{
    public class GraphicsMenuUI : MonoBehaviour
    {
        [SerializeField] GraphicsRowUI graphicsRowPrefab;
        [SerializeField] Transform listRoot;
        LazyValue<GraphicsManager> graphicsManager;

        void Start()
        {
            graphicsManager = new LazyValue<GraphicsManager>(() => FindObjectOfType<GraphicsManager>());
            graphicsManager.ForceInit();
            graphicsManager.value.onRestored += Redraw;
            Redraw();
        }

        void Redraw()
        {
            if(listRoot == null) return;

            foreach(Transform child in listRoot)
            {
                Destroy(child.gameObject);
            }

            foreach(var pair in graphicsManager.value.GetGraphicPairs())
            {
                var graphicsRowInstance = Instantiate(graphicsRowPrefab, listRoot);

                foreach(var innerPair in pair.Value)
                {
                    graphicsRowInstance.Setup(graphicsManager.value, pair.Key, innerPair.Key);
                }
            }
        }
    }
}
