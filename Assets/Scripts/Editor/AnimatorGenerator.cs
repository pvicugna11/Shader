using System;
using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;
using System.Collections.Generic;
using Cinemachine.Editor;

public class AnimatorGenerator : EditorWindow
{
    [SerializeField] private AnimatorController _animatorController;
    [SerializeField] private List<AnimationClip> _animationClips = new List<AnimationClip>();
    [SerializeField] private float playTime;

    private SerializedObject _so;
    private SerializedProperty _animationClipsProperty;

    [MenuItem("Window/Animator Generator")]
    public static void ShowWindow()
    {
        var window = GetWindow<AnimatorGenerator>("Animator Generator");
    }
    
    private void OnEnable()
    {
        // SerializedObjectのインスタンス
        _so = new SerializedObject(this);
        _animationClipsProperty = _so.FindProperty("_animationClips");
    }
    
    private void OnInspectorUpdate()
    {
        // SerializedObjectを更新
        _so.Update();
    }

    private void OnGUI()
    {
        // Animator Controller
        EditorGUILayout.LabelField("Animator Controller", EditorStyles.boldLabel);
        _animatorController = (AnimatorController)EditorGUILayout.ObjectField("Animator Controller", _animatorController, typeof(AnimatorController), false);

        EditorGUILayout.Space();
        
        // Animation Clips
        EditorGUILayout.PropertyField(_animationClipsProperty, true);
        _so.ApplyModifiedProperties();

        EditorGUILayout.Space();
        
        // 各アニメーションの再生時間
        playTime = EditorGUILayout.FloatField("Play Time", playTime);

        EditorGUILayout.Space();
        
        // アニメーションを作成
        if (GUILayout.Button("Generate States"))
        {
            if(_animatorController != null && _animationClips.Count > 0)
            {
                GenerateAnimatorStates();
            }
        }
    }

    private void GenerateAnimatorStates()
    {
        var rootStateMachine = _animatorController.layers[0].stateMachine;
        
        // 初期化
        DeleteAllAnimatorStates(rootStateMachine);
        
        // アニメーション作成
        AnimatorState previousState = null;
        for (int i = 0; i < _animationClips.Count; i++)
        {
            AnimatorState state = rootStateMachine.AddState(_animationClips[i].name);
            state.motion = _animationClips[i];

            if (previousState != null)
            {
                AnimatorStateTransition transition = previousState.AddTransition(state);
                transition.duration = 0;
                transition.exitTime = playTime;
                transition.hasExitTime = true;
            }

            previousState = state;
        }

        // Loop back to the first state
        if (previousState != null && _animationClips.Count > 1)
        {
            AnimatorStateTransition transition = previousState.AddTransition(rootStateMachine.states[0].state);
            transition.duration = 0;
            transition.exitTime = playTime;
            transition.hasExitTime = true;
        }
    }
    
    // Animator StateのすべてのStateを削除する
    private void DeleteAllAnimatorStates(AnimatorStateMachine stateMachine)
    {
        foreach (var state in stateMachine.states)
        {
            stateMachine.RemoveState(state.state);
        }
    }
}

