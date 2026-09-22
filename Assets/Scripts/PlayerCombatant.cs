    using System.Collections.Generic;
    using TMPro;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class PlayerCombatant : Combatant
    {
        public GameObject characterMenu;
        public CombatManager combatManager;

        [SerializeField] private TMP_Text[] moveTexts;
        [SerializeField] protected int currentMP;

        private MoveData selectedMove;
        private bool selectingTarget = false;

        private PlayerCharacter playerCharacter;

        private void Awake()
        {
            combatManager = FindFirstObjectByType<CombatManager>();
        }

        public void Initialize(PlayerCharacter player)
        {
            playerCharacter = player;

            characterData = player.characterData;

            currentHP = characterData.maxHP;
            currentMP = characterData.maxMP;
        }
         
    
        private void Update()
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                SelectBasicAttack();
                return;
            }

            if (!Mouse.current.leftButton.wasPressedThisFrame)
                return;

            if (combatManager.GetCurrentPlayer() != this)
                return;

            if (!selectingTarget)
                return;

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);

            Collider2D hit = Physics2D.OverlapPoint(worldPosition);

            if (hit == null)
                return;

            PlayerCombatant ally = hit.GetComponent<PlayerCombatant>();

            if (ally != null)
            {
                SelectAlly(ally);
                return;
            }

            EnemyCombatant enemy = hit.GetComponent<EnemyCombatant>();

            if (enemy != null)
            {
                SelectEnemy(enemy);
            }
        }

        public void ShowMoves()
        {
            characterMenu.SetActive(true);

            for (int i = 0; i < moveTexts.Length; i++)
            {
                moveTexts[i].text = playerCharacter.moves[i].moveName;
            }
        }

        public void HideMoves()
        {
            characterMenu.SetActive(false);
        }

        public void SelectMove(int moveIndex)
        {
            if (combatManager.GetCurrentPlayer() != this)
                return;

            if (HasStatus(StatusEffect.Confusion))
            {
                SelectRandomAttack();
            }
            else
            {
                selectedMove = playerCharacter.moves[moveIndex];
            }

            if (selectedMove == null)
                return;

            if (currentMP < selectedMove.MPCost)
            {
                Debug.Log(
                    gameObject.name + " does not have enough MP for " +
                    selectedMove.moveName
                );

                selectedMove = null;
                ShowMoves();

                return;
            }

            Debug.Log(
                gameObject.name + " selected " +
                selectedMove.moveName
            );

            selectingTarget = true;

            HideMoves();

            if (selectedMove.targetType == MoveData.TargetType.SingleAlly ||
                selectedMove.targetType == MoveData.TargetType.AllAllies)
            {
                Debug.Log("Select an ally.");
            }
            else
            {
                Debug.Log("Select an enemy.");
            }

        }

        private void SelectRandomAttack()
        {
            List<MoveData> attackingMoves = new List<MoveData>();

            foreach (MoveData move in playerCharacter.moves)
            {
                if (move.moveType == MoveData.MoveType.Attack)
                {
                    attackingMoves.Add(move);
                }
            }

            if (attackingMoves.Count == 0)
            {
                Debug.Log(
                    gameObject.name +
                    " is Confused, but has no attack moves!"
                );

                return;
            }

            selectedMove = attackingMoves[
                Random.Range(0, attackingMoves.Count)
            ];

            Debug.Log(
                gameObject.name +
                " is Confused and randomly selected " +
                selectedMove.moveName
            );
        }

        public void SelectAlly(PlayerCombatant ally)
        {
            if (!selectingTarget)
                return;

            if (selectedMove.targetType != MoveData.TargetType.SingleAlly &&
                selectedMove.targetType != MoveData.TargetType.AllAllies)
                return;

            if (!combatManager.IsAlly(this, ally))
                return;

            Debug.Log(
                gameObject.name + " used " +
                selectedMove.moveName + " on " +
                ally.gameObject.name
            );

            if (selectedMove.targetType == MoveData.TargetType.SingleAlly)
            {
                ApplyMove(ally);
            }
            else if (selectedMove.targetType == MoveData.TargetType.AllAllies)
            {
                foreach (PlayerCombatant player in combatManager.playerParty)
                {
                    ApplyMove(player);
                }
            }

            FinishMove();
        }

        public void SelectEnemy(EnemyCombatant enemy)
        {
            if (!selectingTarget)
                return;

            if (selectedMove.targetType != MoveData.TargetType.SingleEnemy &&
                selectedMove.targetType != MoveData.TargetType.AllEnemies)
                return;

            if (!combatManager.IsEnemy(this, enemy))
                return;

            Debug.Log(
                gameObject.name + " used " +
                selectedMove.moveName + " on " +
                enemy.gameObject.name
            );

            if (selectedMove.targetType == MoveData.TargetType.SingleEnemy)
            {
                ApplyMove(enemy);
            }
            else if (selectedMove.targetType == MoveData.TargetType.AllEnemies)
            {
                foreach (EnemyCombatant target in combatManager.enemyParty)
                {
                    ApplyMove(target);
                }
            }

            FinishMove();
        }

        private void ApplyMove(Combatant target)
        {
            switch (selectedMove.moveType)
            {
                case MoveData.MoveType.Attack:

                    target.TakeDamage(this, selectedMove);

                    break;

                case MoveData.MoveType.Heal:

                    target.Heal(selectedMove.power);

                    break;

                case MoveData.MoveType.Buff:

                    target.ApplyStatModifier(
                        selectedMove.statTarget,
                        selectedMove.moveType
                    );

                    break;

                case MoveData.MoveType.Debuff:

                    target.ApplyStatModifier(
                        selectedMove.statTarget,
                        selectedMove.moveType
                    );

                    break;
            }
        }

        private void FinishMove()
        {
            currentMP -= selectedMove.MPCost;

            selectingTarget = false;
            selectedMove = null;

            ReduceModifierDurations();
            ProcessEndOfTurnStatus();

            combatManager.NextTurn();
        }

        public bool IsTargetingEnemy()
        {
            if (!selectingTarget || selectedMove == null)
                return false;

            return selectedMove.targetType == MoveData.TargetType.SingleEnemy ||
                   selectedMove.targetType == MoveData.TargetType.AllEnemies;
        }

        public void SelectBasicAttack()
        {
            if (combatManager.GetCurrentPlayer() != this)
                return;

            selectedMove = characterData.basicAttack;

            if (selectedMove == null)
            {
                Debug.LogWarning(gameObject.name + " has no basic attack assigned!");
                return;
            }

            Debug.Log(
                gameObject.name + " selected " +
                selectedMove.moveName
            );

            selectingTarget = true;

            HideMoves();

            if (selectedMove.targetType == MoveData.TargetType.SingleAlly ||
                selectedMove.targetType == MoveData.TargetType.AllAllies)
            {
                Debug.Log("Select an ally.");
            }
            else
            {
                Debug.Log("Select an enemy.");
            }
        }
    }