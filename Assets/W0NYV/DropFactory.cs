using UnityEngine;
using UniRx;

namespace Puzzles {

    public class DropFactory : MonoBehaviour
    {

        [SerializeField] private GameObject[] drops;
        [SerializeField] private GameObject canvas;
        [SerializeField] private GameManager gameManager;

        void Start()
        {
            gameManager.ObserveEveryValueChanged(GameManager => GameManager.currentState)
                .Where(value => value == PuzzleState.Init)
                .Subscribe(value => {
                    Initialize();
                })
                .AddTo(this);
            
            gameManager.ObserveEveryValueChanged(GameManager => GameManager.currentState)
                .Where(value => value == PuzzleState.Clearing)
                .Delay(System.TimeSpan.FromSeconds(0.5f))
                .Subscribe(value => {
                    CreateDrops();
                    MessageBroker.Default.Publish(PuzzleState.Judging);
                })
                .AddTo(this);
        }

        private void Initialize() {
            
            for(int i = 0; i < 6; i++) {
                for(int j = 0; j < 5; j++) {
                    GameObject o = Instantiate(drops[Random.Range(0, drops.Length)], new Vector3(100*i - 250, 100*j - 200, 0), Quaternion.identity);
                    o.transform.SetParent(canvas.transform, false);
                    PuzzleManager.CurrentDrops[j, i] = o;
                    PuzzleManager.dropLocations[j, i] = new Vector3(100*i - 250, 100*j - 200, 0);
                }
            }

            MessageBroker.Default.Publish(PuzzleState.Idle);

        }

        void CreateDrops() {
            for(int i = 0; i < 6; i++) {
                for(int j = 0; j < 5; j++) {
                    if(PuzzleManager.CurrentDrops[j, i] == null) CreateDrop(PuzzleManager.dropLocations[j,i], j, i);
                }
            }
        }

        void CreateDrop(Vector3 pos, int colIndex, int rowIndex) {
            GameObject o = Instantiate(drops[Random.Range(0, drops.Length)], pos, Quaternion.identity);
            o.transform.SetParent(canvas.transform, false);
            PuzzleManager.CurrentDrops[colIndex, rowIndex] = o;
        }

    }
}