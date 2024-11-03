using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoddnessTear : BaseItem
{
    private ItemAttribute manaItemAttribute = new ItemAttribute();

    public override void InitItemSkill()
    {
        foreach (ItemAttribute iAttribute in ItemAttributes)
        {
            if (iAttribute.ItemAttributeType == ItemAttributeType.Mana)
            {
                manaItemAttribute = iAttribute;
            }

            iAttribute.InitItemAttributeValue();
        }
    }

    public override void ResetItem()
    {
        base.ResetItem();

        manaItemAttribute.InitItemAttributeValue();
    }
}
