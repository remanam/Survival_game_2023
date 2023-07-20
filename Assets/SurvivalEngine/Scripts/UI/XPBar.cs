using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalEngine
{

    /// <summary>
    /// Bar that shows level and XP
    /// </summary>

    [RequireComponent(typeof(ProgressBar))]
    public class XPBar : MonoBehaviour
    {
        public string level_id;
        public Text level_txt;

        private PlayerUI parent_ui;
        private ProgressBar bar;

        void Awake()
        {
            parent_ui = GetComponentInParent<PlayerUI>();
            bar = GetComponent<ProgressBar>();
        }

        void Update()
        {
            PlayerCharacter character = parent_ui.GetPlayer();
            if (character != null)
            {
                int level = character.Attributes.GetLevel(level_id);
                int xp = character.Attributes.GetXP(level_id);
                int xp_max = xp;

                LevelData next = LevelData.GetLevel(level_id, level + 1);
                if (next != null)
                    xp_max = Mathf.Max(xp, next.xp_required);

                bar.SetMax(xp_max);
                bar.SetValue(xp);

                if (level_txt != null)
                    level_txt.text = "Level " + level.ToString();
            }
        }
    }

}