using SurvivalEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace SurvivalEngine
{

    /// <summary>
    /// Panel that contain the detailed information of a single construction and it's stages's crafts
    /// </summary>
    public class ConstructionPanel : UIPanel
    {

        public Text title;
        public Text desc;
        public Button craft_btn;

        public Text stage_text;

        public ItemSlot[] craft_slots;


        private PlayerUI parent_ui;
        private Construction construction;

        private float update_timer = 0f;

        private static List<ConstructionPanel> panel_list = new List<ConstructionPanel>();

        private static ConstructionPanel _instance;

        protected override void Awake()
        {
            base.Awake();
            panel_list.Add(this);
            parent_ui = GetComponentInParent<PlayerUI>();
            _instance = this;

            if (parent_ui == null)
                Debug.LogError("Warning: Missing PlayerUI script as parent of " + gameObject.name);
        }

        protected override void Start()
        {
            base.Start();
            PlayerControlsMouse.Get().onClick += (Vector3) => { CancelSelection(); };
        }

        private void OnDestroy()
        {
            panel_list.Remove(this);
        }


        protected override void Update()
        {
            base.Update();

            update_timer += Time.deltaTime;
            if (update_timer > 0.5f)
            {
                update_timer = 0f;
                SlowUpdate();
            }
        }

        public void SetVisible()
        {
            Show();
            RefreshPanel();
        }

        private void SlowUpdate()
        {
            if (construction != null && IsVisible())
            {
                RefreshPanel();
            }
        }

        private void RefreshPanel()
        {
            foreach (ItemSlot slot in craft_slots)
                slot.Hide();

            title.text = construction.GetName();

            

            PlayerCharacter player = parent_ui.GetPlayer();
            if (construction.IsFinalStage() == false)
            {
                

                CraftCostData cost = construction.data.stage_craft_list[construction.current_stage - 1].GetCraftCost();
                int index = 0;
                foreach (KeyValuePair<ItemData, int> pair in cost.craft_items)
                {
                    if (index < craft_slots.Length)
                    {
                        ItemSlot slot = craft_slots[index];
                        slot.SetSlot(pair.Key, pair.Value, false);
                        slot.SetFilter(player.Inventory.HasItem(pair.Key, pair.Value) ? 0 : 2);
                        slot.ShowTitle();
                    }
                    index++;
                }

                foreach (KeyValuePair<GroupData, int> pair in cost.craft_fillers)
                {
                    if (index < craft_slots.Length)
                    {
                        ItemSlot slot = craft_slots[index];
                        slot.SetSlotCustom(pair.Key.icon, pair.Key.title, pair.Value, false);
                        slot.SetFilter(player.Inventory.HasItemInGroup(pair.Key, pair.Value) ? 0 : 2);
                        slot.ShowTitle();
                    }
                    index++;
                }

                foreach (KeyValuePair<CraftData, int> pair in cost.craft_requirements)
                {
                    if (index < craft_slots.Length)
                    {
                        ItemSlot slot = craft_slots[index];
                        slot.SetSlot(pair.Key, pair.Value, false);
                        slot.SetFilter(player.Crafting.CountRequirements(pair.Key) >= pair.Value ? 0 : 2);
                        slot.ShowTitle();
                    }
                    index++;
                }

                if (index < craft_slots.Length)
                {
                    ItemSlot slot = craft_slots[index];
                    if (cost.craft_near != null)
                    {
                        slot.SetSlotCustom(cost.craft_near.icon, cost.craft_near.title, 1, false);
                        bool isnear = player.IsNearGroup(cost.craft_near) || player.EquipData.HasItemInGroup(cost.craft_near);
                        slot.SetFilter(isnear ? 0 : 2);
                        slot.ShowTitle();
                    }
                }
            }

            craft_btn.interactable = player.Crafting.CanCraftStage(construction, construction.current_stage);

            if (construction.IsFinalStage() == true)
                craft_btn.gameObject.SetActive(false);
        }

        public void OnClickCraft()
        {
            PlayerCharacter player = parent_ui.GetPlayer();

            if (player.Crafting.CanCraftStage(construction, construction.current_stage) && construction.current_stage < construction.stage_count)
            {
                player.Crafting.BuildConstructionStage(construction, construction.current_stage);
                construction.ChangeMeshStage();

                RefreshPanel();

                Hide();

                craft_btn.interactable = false;
/*                if (construction.current_stage == construction.stage_count)
                {
                    craft_btn.enabled = false;
                }*/

                construction = null;


            }
        }
        public void SetConstruction(Construction construction)
        {
            this.construction = construction;
            name = construction.GetName();
            if (construction.IsFinalStage() == false)
                stage_text.text = "stage " + construction.current_stage.ToString() + "/" + (construction.stage_count - 1).ToString();
            else
                stage_text.text = "";

        }

        public void CancelSelection()
        {
            base.Hide();

        }

        public static ConstructionPanel Get()
        {
            return _instance;
        }
    }


}

