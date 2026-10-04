using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcStates : MonoBehaviour
{
    public enum NpcState
    {
        Default,
        Idle,
        Patrol,
        Wander,
        Talk
    }
    public NpcState currentState = NpcState.Idle;

    public PatrolNpc patrol;
    public WanderNpc wander;
    public TalkNpc talk;

    private NpcState _defaultState;

    void Start()
    {
        _defaultState = currentState;
        SwitchState(currentState);
    }

    public void SwitchState(NpcState newState)
    {
        currentState = newState;

        patrol.enabled = newState == NpcState.Patrol;
        wander.enabled = newState == NpcState.Wander;
        talk.enabled = newState == NpcState.Talk;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SwitchState(NpcState.Talk);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SwitchState(_defaultState);
        }
    }
}
