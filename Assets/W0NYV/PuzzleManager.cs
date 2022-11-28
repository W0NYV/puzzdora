using UnityEngine;
using UniRx;

namespace Puzzles {
    
    public class PuzzleManager : MonoBehaviour
    {

        public static bool isDragging = false;
        public static GameObject draggedDrop;
        public static GameObject[,] CurrentDrops = new GameObject[5, 6];
        public static Vector3[,] dropLocations = new Vector3[5, 6];

        [SerializeField] private GameManager gameManager;

        bool HasDeleted = false;

        void Start()
        {

            gameManager.ObserveEveryValueChanged(GameManager => GameManager.currentState)
                .Where(value => value == PuzzleState.Judging)
                .Delay(System.TimeSpan.FromSeconds(0.25f))
                .Subscribe(value => {
                    HasDeleted = false;
                    JudgeDrops();
                    DeleteDrops();

                    if(HasDeleted) {
                        MessageBroker.Default.Publish(PuzzleState.Shifting);
                    } else {
                        MessageBroker.Default.Publish(PuzzleState.Idle);
                    }
                })
                .AddTo(this);

            gameManager.ObserveEveryValueChanged(GameManager => GameManager.currentState)
                .Where(value => value == PuzzleState.Shifting)
                .Delay(System.TimeSpan.FromSeconds(0.25f))
                .Subscribe(value => {
                    ShiftDrops();
                    MessageBroker.Default.Publish(PuzzleState.Clearing);
                })
                .AddTo(this);
        }

        void ShiftDrops() {

            for(int row = 0; row < 6; row++) {

                for(int col = 0; col < 5; col++) {
                    
                    if(CurrentDrops[col,row] == null) {

                        for(int i = 1; i <= 4-col; i++) {

                            if(CurrentDrops[col+i,row] != null) {

                                CurrentDrops[col+i,row].transform.localPosition = dropLocations[col,row]; 
                                CurrentDrops[col,row] = CurrentDrops[col+i,row];
                                CurrentDrops[col+i,row] = null;

                                break;
                            }

                        }
                    }

                }
            }

        }

        void JudgeDrops() {

            for(int row = 0; row < 6; row++) {

                for(int col = 0; col < 5; col++) {
                    //縦の判定-----------------------
                    int aboveDropNum = 4-col;

                    if(2 <= aboveDropNum) {

                        int matchNum = 0;

                        for(int i = 1; i <= aboveDropNum; i++) {
                            if(CurrentDrops[col, row].GetComponent<Drop>().type == CurrentDrops[col+i, row].GetComponent<Drop>().type) {
                                matchNum++;
                            } else {
                                break;
                            }
                        }

                        if(2 <= matchNum) {
                            for(int i = 0; i <= matchNum; i++) CurrentDrops[col+i, row].GetComponent<Drop>().canDelete = true;
                        }
                    }

                    //横の判定-----------------------
                    int rightDropNum = 5-row;
                    
                    if(2 <= rightDropNum) {
                        int matchNum = 0;
                        for(int i = 1; i <= rightDropNum; i++) {
                            if(CurrentDrops[col, row].GetComponent<Drop>().type == CurrentDrops[col, row+i].GetComponent<Drop>().type) {
                                matchNum++;
                            } else {
                                break;
                            }
                        }

                        if(2 <= matchNum) {
                            for(int i = 0; i <= matchNum; i++) CurrentDrops[col, row+i].GetComponent<Drop>().canDelete = true;
                        }
                    }
                }
            }
        }

        void DeleteDrops() {

            for(int i = 0; i < 5; i++) {
                for(int j = 0; j < 6; j++) {

                    if(CurrentDrops[i, j].GetComponent<Drop>().canDelete) {
                        Destroy(CurrentDrops[i, j]);
                        HasDeleted = true;
                    }
                }
            }

        } 


    }
}
