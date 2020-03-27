using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using VovTech.UI;

namespace VovTech.Levels
{
    public class LostCityMap : Level
    {
        public override void Initialize()
        {
            TriggerFunctions = new Dictionary<string, System.Action>();
            TriggerFunctions["ShowStartTip"] = delegate
            {
                GameObject startTip = UIManager.Instance.BuildUIElement(
                new UITooltipStrategy("Pay attention", "Check your inventory for bonuses.\nGleb zdarova!",
                ColorTemplates.Get.BrightGreen), new Vector3(148.47f, 12f, -152.01f));
                startTip.GetComponent<UITip>().LookAtCamera(4);
                startTip.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            };
        }
    }
}