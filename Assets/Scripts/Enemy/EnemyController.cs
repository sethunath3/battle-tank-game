﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Manager;

namespace Enemy
{
    public enum EnemyState { petrol, chase, coolDown }

    public class EnemyController
    {
        public EnemyView enemyView { get; private set; }
        public EnemyModel enemyModel { get; private set; }

        public event Action DestroyEnemy;
        private EnemyState enemyState = EnemyState.petrol;

        public EnemyState EnemyState { get { return enemyState; } }

        private EnemyState lastStateView, currentStateView;

        private EnemyData enemyData;

        public EnemyData EnemyData { get { return enemyData; } }

        public EnemyController(ScriptableObjEnemy scriptableObjEnemy, Vector3 position, int enemyIndex)
        {
            if(GameManager.Instance.currentState.gameStateType == StateMachine.GameStateType.Replay)
            {
                enemyData = new EnemyData();
                enemyData = EnemyManager.Instance.EnemyDatas[enemyIndex];
                for (int i = 0; i < enemyData.wayPoints.Count; i++)
                {
                    Debug.Log("[EnemyController] WayPoint" + i + " " + enemyData.wayPoints[i]);
                }
            }

            EnemyManager.Instance.AlertMode += GetAlerted;
            enemyModel = new EnemyModel();
            enemyModel.scriptableObj = scriptableObjEnemy;
            GameObject enemy = GameObject.Instantiate<GameObject>(enemyModel.scriptableObj.enemyView.gameObject);
            enemyView = enemy.GetComponent<EnemyView>();
            enemyView.SetEnemyController(this);
            enemy.transform.position = position;
            enemyModel.CurrentHealth = enemyModel.scriptableObj.health;
            enemyView.TargetDetected += SendAlert;
            enemyView.StateChangedEvent += ChangeState;
            enemyView.enemyIndex = enemyIndex;
            ChangeState(EnemyState.petrol);
        }

        public void TakeDamage(int value)
        {
            enemyModel.CurrentHealth -= value;
            if (enemyModel.CurrentHealth <= 0)
            {
                DestroyEnemy?.Invoke();
            }
        }

        public void ChangeState(EnemyState enemyState)
        {
            lastStateView = currentStateView;

            currentStateView = enemyState;

            if (currentStateView == EnemyState.petrol)
                enemyView.PetrolState.enabled = true;
            else if (currentStateView == EnemyState.chase)
                enemyView.ChaseState.enabled = true;
        }

        public void DestroyEnemyModel()
        {
            enemyView.TargetDetected -= SendAlert;
            enemyView.DestroyEnemyView();
            enemyModel = null;
        }

        public void RemoveAlertMode()
        {
            EnemyManager.Instance.AlertMode -= GetAlerted;
            Debug.Log("[EnemyController] AlertMode removed");
        }

        void GetAlerted(Vector3 position)
        {
            enemyView.FollowTarget(position);
        }

        void SendAlert(Vector3 position)
        {
            EnemyManager.Instance.AlertEnemies(position);
        }

        public int getScoreIncreaser()
        {
            return enemyModel.scriptableObj.scoreIncrease;
        }
    }
}