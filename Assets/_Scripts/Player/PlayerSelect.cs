﻿namespace Player {

    using UnityEngine;
    using UnityEngine.EventSystems;

    using Constants;
    using Enum;
    using Helpers;
    using Unit;

    public class PlayerSelect : MonoBehaviour {
        #region VARIABLE
        private Camera _camera;
        private Player _controller;

        // SELECTION
        public HasHealthBase currentSelected;
        public HasHealthBase previousSelected;

        public Vector3 moveTo;
        public HasHealthBase toAttack;

        //public Selectable currentHover;
        //public Selectable previousHover;

        public bool selected = false;
        public bool selectedLock = false;

        // PHYSICS (RAY)
        private float _distance = 50.0f;

        private Ray _ray;
        private RaycastHit _hitInfo;
        #endregion

        #region UNITY
        private void Awake() {
            this._camera = this.GetComponent<Camera>() as Camera; 
        }

        private void Update() {
            this.UpdateSelect();
        }
        #endregion

        #region CLASS
        public void Init(Player player) {
            this._controller = player;
        }

        public void EndTurn() {
            if(this.currentSelected != null && this.currentSelected.uiComponent.showSelected)
                this.currentSelected.uiComponent.Hide();

            this.currentSelected = null;
            this.previousSelected = null;
        }

        private void UpdateSelect() {
            switch(this._controller.selectionState) {
                case SelectionState.FREE:
                    this.CastRayToWorld(GlobalSettings.LayerValues.groundLayer);
                    this.MouseSelect();
                    //this.HoverSelect();
                break;

                case SelectionState.RESEARCH:
                    Debug.Log("IN Research Mode");
                break;

                case SelectionState.UNIT_ATTACK:
                    this.AttackSelection();
                break;

                case SelectionState.UNIT_SPECIAL:
                    this.SpecialSelection();
                break;

                case SelectionState.UNIT_MOVE:
                    this.MoveToSelection();
                break;

                default:
                    this.CastRayToWorld(GlobalSettings.LayerValues.groundLayer);
                    this.MouseSelect();
                    //this.HoverSelect();
                break;
            }
        }

        private bool CastRayToWorld(LayerMask mask) {
            this._ray = this._camera.ScreenPointToRay(Input.mousePosition);

            Debug.DrawRay(this._ray.origin, this._ray.direction * this._distance, Color.yellow);

            if(EventSystem.current.IsPointerOverGameObject()) // check to see if our point iver on a UI object.
                return false;

            return Physics.Raycast(this._ray, out this._hitInfo, this._distance, ~(mask)); // ignores everything other than the mask
        }

        private void MouseSelect() {
            if(Input.GetMouseButtonUp(0)) {

                if(this.CastRayToWorld(GlobalSettings.LayerValues.groundLayer)) {
                    //Selectable tempSelect = this._hitInfo.transform.GetComponent<Selectable>() as Selectable;
                        this.SelectObject();
                } else {
                        this.DeSelectObject();
                }
            }
        }

        private void SelectObject() {
            if(this._hitInfo.transform.GetComponent<HasHealthBase>() != null) {
                var temp = this._hitInfo.transform.GetComponent<HasHealthBase>();

                if(temp.entityType == Enum.EntityType.UNIT) {
                    if(((UnitBase)temp).hasFinished)
                        return;
                }

                if(temp.controller.id == this._controller.id) {
                    if(this.currentSelected != null && !temp.Equals(this.currentSelected)) {
                        this.previousSelected = this.currentSelected;
                        this.previousSelected.uiComponent.Hide();
                    }

                    this.currentSelected = temp;
                    this.currentSelected.uiComponent.Display();

                    this.selected = true;
                }
            }
        }

        private void DeSelectObject() {
            if(!EventSystem.current.IsPointerOverGameObject()) {
                if(this.currentSelected != null) {
                    this.previousSelected = this.currentSelected;
                    this.currentSelected.uiComponent.Hide(); // Hide UI;
                    this.currentSelected = null;
                }
                this.selected = false;
            }
        }

        private void AttackSelection() {
            if(Input.GetMouseButtonUp(0)) {
                if(this.CastRayToWorld(GlobalSettings.LayerValues.groundLayer)) {
                    UnitBase unit = this.currentSelected as UnitBase;
                    UnitBase toAttack = this._hitInfo.transform.GetComponent<UnitBase>();
                    float distance = Vector3.Distance(toAttack.position, unit.position) - (unit.unitRadius - unit.radiusDrawer.width);

                    Debug.DrawLine(unit.position, toAttack.position, Color.blue, 20.0f);
                    Debug.Log("Selection Distance: " + distance);
                    Debug.Log("Unit Attack Radius: " + unit.attackRadius);
                    Debug.Log("Unit Radius: " + unit.unitRadius);

                    if(distance > unit.attackRadius) {
                        Debug.Log("Out Of Unit Attack Radius");
                        return;
                    } else {
                        if(toAttack.IsAlly(unit as IHasHealth)) {
                            Debug.Log("CAN NOT ATTACK ALLY");
                            return;
                        } else {
                            Debug.Log("Can Attack " + (toAttack != null ? toAttack.name : "doesn't exist"));
                            unit.Attack(toAttack as IHasHealth);
                            this.DebugText(unit, toAttack);
                        }
                    }
                }
            }
        }

        private void SpecialSelection() {
            if(this.currentSelected.entityType == EntityType.UNIT && this.currentSelected.GetType() == typeof(Cleric)) {
                Debug.Log("CLERIC HEAL SELECTED");
            }

            if(Input.GetMouseButtonUp(0)) {
                if(this.CastRayToWorld(GlobalSettings.LayerValues.groundLayer)) {
                    Cleric unit = this.currentSelected as Cleric;
                    UnitBase selection = this._hitInfo.transform.GetComponent<UnitBase>();
                    float distance = Vector3.Distance(selection.position, unit.position) - (unit.unitRadius - unit.radiusDrawer.width);

                    Debug.DrawLine(unit.position, selection.position, Color.blue, 20.0f);
                    Debug.Log("Selection Distance: " + distance);
                    Debug.Log("Unit Heal Radius: " + unit.healingRadius);
                    Debug.Log("Unit Radius: " + unit.unitRadius);

                    if(distance > unit.healingRadius) {
                        Debug.Log("Out Of Unit Heal Radius");
                        return;
                    } else {
                        if(selection.IsEnemy(unit as IHasHealth)) {
                            Debug.Log("CAN NOT HEAL ENEMY");
                            return;
                        } else {
                            Debug.Log("Can Heal " + (selection != null ? selection.name : "doesn't exist"));
                            unit.Heal(selection as IHasHealth);
                            this.DebugText(unit, selection);
                        }
                    }
                }
            }
        }

        private void MoveToSelection() {
            Debug.DrawRay(this._ray.origin, this._ray.direction * this._distance, Color.yellow);
            if(Input.GetMouseButtonUp(0)) {
                if(this.CastRayToWorld(GlobalSettings.LayerValues.unitLayer)) {
                    UnitBase unit = this.currentSelected as UnitBase;
                    float distance = Vector3.Distance(this._hitInfo.point, unit.position) - (unit.unitRadius - unit.radiusDrawer.width);
                    
                    Debug.DrawLine(unit.position, this._hitInfo.point, Color.blue, 20.0f);
                    Debug.Log("Selection Distance: " + distance.ToString());
                    Debug.Log("Unit Move Radius: " + (((UnitBase)this.currentSelected).moveRadius + ((UnitBase)this.currentSelected).unitRadius).ToString());
                    Debug.Log("Unit Radius: " + ((UnitBase)this.currentSelected).unitRadius);

                    if(distance > unit.moveRadius) {
                        Debug.Log("Out Of Unit Move Radius");
                        return;
                    }else {
                        Debug.Log("Can Move To");
                        unit.MoveTo(this._hitInfo.point);
                    }
                }
            }
        }

        private void DebugText(UnitBase unit, UnitBase selection) {
            float damage = unit.GetDamage();
            float extraDamage = 0.0f;
            string damagetext = damage.ToString() + "(" + "+" +extraDamage.ToString() + ")";

            if(unit.attackType == selection.weaknessType) {
                extraDamage = (damage * (selection.weaknessPercentage / 100.0f));
                damagetext = damage.ToString() + "(" + "+" + extraDamage.ToString() + ")";
            }
            if(unit.attackType == selection.resistanceType) {
                extraDamage = damage * (selection.resistancePercentage / 100.0f);
                damagetext = damage.ToString() + "(" + "-" + extraDamage.ToString() + ")";
            }

            string temp = "\r\n" + unit.name + " Attacking: " + selection.name + " For " + damagetext + " DAMAGe";

            this._controller.uiComponent.ChangeDebugText(temp);
        }

        // NOTE: instead of using this i've used the unity OnMouseEnter/Exit to simulate hovering over an object.
        /*private void HoverSelect() {
            if(this.CastRayToWorld()) {
                if(this._hitInfo.transform.GetComponent<Selectable>() != null) {
                    if(this.currentHover != null) {
                        this.previousHover = this.currentHover;
                        this.currentHover = this._hitInfo.transform.GetComponent<Selectable>() as Selectable;
                    } else {
                        this.currentHover = this._hitInfo.transform.GetComponent<Selectable>() as Selectable;
                    }
                }
            }else {
                if(this.currentHover != null)
                    this.previousHover = this.currentHover;
                this.currentHover = null;
            }
        }*/
        #endregion
    }
}