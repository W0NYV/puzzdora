using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace Puzzles
{
    public class GameManager : MonoBehaviour
    {
    
        public static PuzzleState currentState;

        void Start()
        {

            currentState = PuzzleState.Init;

            MessageBroker.Default.Receive<PuzzleState>()
                .Subscribe(x => currentState = x);

        }

    }

}

