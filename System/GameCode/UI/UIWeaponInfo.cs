using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace VovTech.UI
{
    /// <summary>
    /// Weapon info. Appears when player stands next to some weapon.
    /// </summary>
    public class UIWeaponInfo : UIEntity
    {
        public TextMeshProUGUI Name;
        public TextMeshProUGUI Type;
        public GameObject PrefixesContainer;
        public TextMeshProUGUI Dps;
        public Image AmmoTypeIcon;
        public TextMeshProUGUI AmmoType;
        public Image AmmoIcon;
        public TextMeshProUGUI Ammo;
        public GameObject Separator;

        private GameObject prefixGo;
        private GameObject attachedObject;

        private void Awake()
        {
            prefixGo = Resources.Load<GameObject>("UI/WeaponPrefix");
            vanishAnimation = delegate
            {
                transform.DOScale(0, 0.5f).OnComplete(() => { Destroy(gameObject); });
            };
        }

        /// <summary>
        /// Populate this with weapon info.
        /// </summary>
        /// <param name="weapon"></param>
        public void PopulateWithInfo(Weapon weapon)
        {
            WeaponInfo settings = weapon.Settings;
            Name.text = settings.Name;
            Type.text = settings.WeaponType.ToString();
            if (weapon.Prefixes.Count > 0)
            {
                Separator.gameObject.SetActive(true);
                foreach (StatAffector affector in weapon.Prefixes)
                {
                    UIWeaponPrefix prefix = Instantiate(prefixGo, PrefixesContainer.transform).GetComponent<UIWeaponPrefix>();
                    prefix.PopulateWithInfo(affector);
                }
            }
            Dps.text = Mathf.RoundToInt(weapon.Settings.Projectile.Heatlh / weapon.Settings.ShotInterval).ToString();
        }

        private void Update()
        {
            if(attachedObject != null)
                transform.position = attachedObject.transform.position + new Vector3(0, 2, 0);
        }

        public void Attach(GameObject target)
        {
            attachedObject = target;
        }
    }
}