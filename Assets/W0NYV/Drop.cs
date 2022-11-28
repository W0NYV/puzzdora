using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace Puzzles {

    public class Drop : MonoBehaviour
    {
        public DropType type;
        public bool canDelete = false;

        void Start()
        {

            var eventTrigger = this.gameObject.AddComponent<ObservableEventTrigger>();

            eventTrigger.OnPointerDownAsObservable()
                .Where(_ => GameManager.currentState == PuzzleState.Idle)
                .Subscribe(pointerEventData => {
                    PuzzleManager.isDragging = true;
                    PuzzleManager.draggedDrop = pointerEventData.pointerEnter;
                    MessageBroker.Default.Publish(PuzzleState.Moving);
                })
                .AddTo(this);

            eventTrigger.OnPointerUpAsObservable()
                .Where(_ => GameManager.currentState == PuzzleState.Moving)
                .Subscribe(pointerEventData => {
                    PuzzleManager.isDragging = false;
                    MessageBroker.Default.Publish(PuzzleState.Judging);
                })
                .AddTo(this);

            eventTrigger.OnPointerEnterAsObservable()
                .Where(_ => PuzzleManager.isDragging && GameManager.currentState == PuzzleState.Moving)
                .Subscribe(pointerEventData => Swap())
                .AddTo(this);
            
        }

        private void Swap() {

            float col = (transform.localPosition.y+200.0f)/100.0f;
            float row = (transform.localPosition.x+250.0f)/100.0f;

            float destCol = (PuzzleManager.draggedDrop.transform.localPosition.y+200.0f)/100.0f;
            float destRow = (PuzzleManager.draggedDrop.transform.localPosition.x+250.0f)/100.0f;

            GameObject tmp = PuzzleManager.CurrentDrops[(int)col, (int)row];

            transform.localPosition = PuzzleManager.dropLocations[(int)destCol, (int)destRow];
            PuzzleManager.draggedDrop.transform.localPosition = PuzzleManager.dropLocations[(int)col, (int)row];

            PuzzleManager.CurrentDrops[(int)col, (int)row] = PuzzleManager.CurrentDrops[(int)destCol, (int)destRow];
            PuzzleManager.CurrentDrops[(int)destCol, (int)destRow] = tmp;

        }
    }
}