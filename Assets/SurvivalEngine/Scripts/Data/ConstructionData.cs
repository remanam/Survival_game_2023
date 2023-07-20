using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

namespace SurvivalEngine
{
    /// <summary>
    /// Data file for Constructions
    /// </summary>



    [System.Serializable]
    public class CraftStageCostData
    {
        public Dictionary<ItemData, int> craft_stage_items = new Dictionary<ItemData, int>();
        public Dictionary<ConstructionData, int> craft_stage_requirements = new Dictionary<ConstructionData, int>();
        public GroupData craft_near;
        public Dictionary<GroupData, int> craft_fillers = new Dictionary<GroupData, int>();
    }

    [Serializable]
    public class StageCraftList
    {
        public ItemData[] stage_craftings; //list of items Items needed to build stage of this construction
    }

    [CreateAssetMenu(fileName = "ConstructionData", menuName = "SurvivalEngine/ConstructionData", order = 4)]
    public class ConstructionData : CraftData
    {
        [Header("--- ConstructionData ------------------")]

        public GameObject construction_prefab; //Prefab spawned when the construction is built

        [Header("Building Costs")]

        public GroupData[] craft_stage_fillers; //Items needed to craft this (but that can be any item in group)
        public CraftData[] craft_stage_requirements; //What needs to be built before you can craft this

        public StageCraftList[] stageCraftList; // contains crafts for all consctruction stages

        [Header("Ref Data")]
        public ItemData take_item_data; //For constructions that can be picked (trap, lure) what is the matching item

        [Header("Stats")]
        public DurabilityType durability_type;
        public float durability;

        private static List<ConstructionData> construction_data = new List<ConstructionData>();


    public bool HasDurability()
        {
            return durability_type != DurabilityType.None && durability >= 0.1f;
        }

        public static new void Load(string folder = "")
        {
            construction_data.Clear();
            construction_data.AddRange(Resources.LoadAll<ConstructionData>(folder));
        }

        public new static ConstructionData Get(string construction_id)
        {
            foreach (ConstructionData item in construction_data)
            {
                if (item.id == construction_id)
                    return item;
            }
            return null;
        }

        public new static List<ConstructionData> GetAll()
        {
            return construction_data;
        }

        public CraftStageCostData GetCraftStageCost(int stage_number)
        {
            CraftStageCostData cost = new CraftStageCostData();

            for (int i = 0; i < stageCraftList.Length; i++)
            {

            }

            foreach (ItemData item in stageCraftList[stage_number].stage_craftings)
            {
                if (cost.craft_stage_items.ContainsKey(item) == false)
                    cost.craft_stage_items[item] = 1;
                else
                    cost.craft_stage_items[item] += 1;
            }


            foreach (ConstructionData cdata in craft_stage_requirements)
            {
                if (cost.craft_stage_requirements.ContainsKey(cdata) == false)
                    cost.craft_stage_requirements[cdata] = 1;
                else
                    cost.craft_stage_requirements[cdata] += 1;
            }

            if (craft_near != null)
                cost.craft_near = craft_near;

            return cost;
        }


    }
}
