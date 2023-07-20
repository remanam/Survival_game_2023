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

        private void Start()
        {
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

        public void Set(Selectable target, Construction construction)
        {
            Show();
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

            PlayerCharacter player = parent_ui.GetPlayer();

        }

        public void OnClickCraft()
        {
            PlayerCharacter player = parent_ui.GetPlayer();

            if (player.Crafting.CanCraft(construction.data, construction.current_stage))
            {
                player.Crafting.BuildStagedConstruction(construction.data, construction.current_stage);

                craft_btn.interactable = false;
                Hide();
            }
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

