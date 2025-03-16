using UnityEngine;
using TMPro;
using RPG.Combat;
using RPG.Inventories;
using UnityEngine.UI;
using RPG.Abilities;

namespace RPG.UI.Inventories
{
    public class ItemTooltip : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI itemNameText;
        [SerializeField] Image itemIcon;
        [SerializeField] Image rarityImage;
        [SerializeField] TextMeshProUGUI itemDescriptionText;
        [SerializeField] TextMeshProUGUI itemInfoText;
        [SerializeField] TextMeshProUGUI itemRarityText;
        [SerializeField] RarityColor[] rarityColorConfig;

        public void Setup(InventoryItem item)
        {
            itemNameText.text = item.GetDisplayName();
            itemIcon.sprite = item.GetIcon();
            itemDescriptionText.text = item.GetDescription();
            itemInfoText.text = "";

            SetRarity(item);
            CheckIfWeapon(item);
            CheckIfStatsEquipableItem(item);
            CheckIfAbility(item);
        }

        [System.Serializable]
        struct RarityColor
        {
            public ItemRarity rarity;
            public Color color;
        }

        void SetRarity(InventoryItem item)
        {
            itemRarityText.text = $"{item.GetRarity()}";

            foreach(var config in rarityColorConfig)
            {
                if(config.rarity == item.GetRarity())
                {
                    rarityImage.color = config.color;
                    return;
                }
            }
        }

        void CheckIfWeapon(InventoryItem item)
        {
            var weapon = item as WeaponConfig;

            if (weapon != null)
            {
                itemInfoText.text += $"- Base Damage: {weapon.GetDamage()} HP\n";
                itemInfoText.text += $"- Bonus Damage: {weapon.GetPercentageBonus()}%\n";
                itemInfoText.text += $"- Range: {weapon.GetRange()}m\n";
            }
        }

        void CheckIfStatsEquipableItem(InventoryItem item)
        {
            var statsEquipableItem = item as StatsEquipableItem;

            if (statsEquipableItem != null)
            {
                if(statsEquipableItem.GetAdditiveModifiers() != null)
                {
                    foreach (var additiveModifier in statsEquipableItem.GetAdditiveModifiers())
                    {
                        itemInfoText.text += $"- {additiveModifier.stat}+{additiveModifier.value}\n";
                    }
                }

                if(statsEquipableItem.GetPercentageModifiers() != null)
                {
                    foreach (var percentageModifier in statsEquipableItem.GetPercentageModifiers())
                    {
                        itemInfoText.text += $"- {percentageModifier.stat}+{percentageModifier.value}%\n";
                    }
                }
            }
        }

        void CheckIfAbility(InventoryItem item)
        {
            Ability ability = item as Ability;

            if(ability != null)
            {
                foreach(var effect in ability.GetEffects())
                {
                    if(effect.GetEffectInfo() == null) 
                    {
                        continue;
                    }

                    foreach(var info in effect.GetEffectInfo())
                    {
                        itemInfoText.text += $"- {info}\n";
                    }
                }
            }
        }
    }
}
