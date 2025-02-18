using System.Collections.Generic;
using _Scripts.Player;
using _Scripts.Weapon_Systems.Weapons_Logic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Scripts.Weapon_Systems.Weapons_UI
{
    public class WeaponUIManager : MonoBehaviour
    {
        [Header("- Dependencies")]
        [SerializeField] private WeaponHandler weaponHandler;
        [SerializeField] private WeaponManager weaponManager;
        private InputManager _inputManager;

        [Header("- Main Weapon Info")]
        [SerializeField] private CanvasGroup mainWeaponInfo;
        [SerializeField] private TextMeshProUGUI weaponNameText;
        [SerializeField] private TextMeshProUGUI currentAmmoText;
        [SerializeField] private TextMeshProUGUI reserveAmmoText;

        [Header("- Weapon Selection Menu")]
        [SerializeField] private CanvasGroup weaponSelectionPanel;
        [SerializeField] private Transform weaponListContent;
        [SerializeField] private GameObject weaponSelectionItemPrefab;
        [SerializeField] private float weaponSelectionShowTime = 0.5f;
        [SerializeField] private Color selectedWeaponColor = new Color(0f, 1f, 0.7f);

        [Header("- UI Animation")]
        [SerializeField] private float fadeSpeed = 5f;

        private List<GameObject> _weaponItems = new List<GameObject>();
        private float _weaponSelectionTimer;
        private bool _isWeaponMenuVisible;

        private void Start()
        {
            InitializeDependencies();
            SetWeaponSelectionVisibility(false);
            InitializeWeaponSelectionMenu();
        }

        private void InitializeDependencies()
        {
            _inputManager = FindFirstObjectByType<InputManager>();
            
            if (weaponHandler == null)
                weaponHandler = FindFirstObjectByType<WeaponHandler>();
            
            if (weaponManager == null)
                weaponManager = FindFirstObjectByType<WeaponManager>();
        }

        private void Update()
        {
            UpdateMainWeaponInfo();
            HandleWeaponSelectionDisplay();
        }

        private void UpdateMainWeaponInfo()
        {
            if (weaponHandler == null) return;

            WeaponData currentWeapon = weaponHandler.GetCurrentWeaponData();
            if (currentWeapon == null) return;

            // Get current ammo and reserve values.
            int currentAmmo = weaponHandler.GetCurrentAmmo();
            int reserveAmmo = weaponHandler.GetTotalAmmoLeft();

            // Update weapon name.
            if (weaponNameText != null)
                weaponNameText.text = currentWeapon.weaponName;
        
            // Update ammo display with clear formatting.
            if (currentAmmoText != null)
                currentAmmoText.text = currentAmmo.ToString();
        
            if (reserveAmmoText != null)
                reserveAmmoText.text = reserveAmmo.ToString();
            
        }

        private void UpdateWeaponNameDisplay(WeaponData currentWeapon)
        {
            if (weaponNameText != null)
                weaponNameText.text = currentWeapon.weaponName;
        }

        private void UpdateAmmoDisplay()
        {
            if (currentAmmoText != null)
                currentAmmoText.text = weaponHandler.GetCurrentAmmo().ToString();
                
            if (reserveAmmoText != null)
                reserveAmmoText.text = weaponHandler.GetTotalAmmoLeft().ToString();
        }

        private void HandleWeaponSelectionDisplay()
        {
            // Show selection UI when scrolling.
            if (Mathf.Abs(_inputManager.weaponScrollInput) > 0)
            {
                ShowWeaponSelection();
            }

            UpdateWeaponSelectionVisibility();
            UpdateSelectedWeaponHighlight();
        }

        private void InitializeWeaponSelectionMenu()
        {
            // Clear existing items.
            ClearExistingWeaponItems();

            // Create weapon selection items.
            for (int i = 0; i < weaponManager.GetWeaponCount(); i++)
            {
                WeaponData weaponData = weaponManager.GetWeaponAtIndex(i);
                if (weaponData == null) continue;

                GameObject item = Instantiate(weaponSelectionItemPrefab, weaponListContent);
                SetupWeaponSelectionItem(item, weaponData, i);
                _weaponItems.Add(item);
            }
        }

        private void ClearExistingWeaponItems()
        {
            foreach (var item in _weaponItems)
            {
                Destroy(item);
            }
            _weaponItems.Clear();
        }

        private void SetupWeaponSelectionItem(GameObject item, WeaponData weaponData, int index)
        {
            // Setup UI texts and icons.
            SetupWeaponNumber(item, index);
            SetupWeaponIcon(item, weaponData);
            SetupWeaponText(item, weaponData);
        }

        private void SetupWeaponNumber(GameObject item, int index)
        {
            var hotkeyText = item.transform.Find("Weapon Number")?.GetComponent<TextMeshProUGUI>();
            if (hotkeyText != null)
                hotkeyText.text = (index + 1).ToString();
        }

        private void SetupWeaponIcon(GameObject item, WeaponData weaponData)
        {
            var weaponIcon = item.transform.Find("Weapon Icon")?.GetComponent<Image>();
            if (weaponIcon != null)
            {
                weaponIcon.sprite = weaponData.weaponIcon;
                weaponIcon.color = weaponData.weaponIconTint;
                weaponIcon.enabled = weaponData.weaponIcon != null;
            }
        }

        private void SetupWeaponText(GameObject item, WeaponData weaponData)
        {
            var weaponNameText = item.transform.Find("Weapon Info/Weapon Name")?.GetComponent<TextMeshProUGUI>();
            var weaponTypeText = item.transform.Find("Weapon Info/Weapon Type")?.GetComponent<TextMeshProUGUI>();

            if (weaponNameText != null)
                weaponNameText.text = weaponData.weaponName;

            if (weaponTypeText != null)
                weaponTypeText.text = weaponData.weaponType.ToString();
        }

        private void ShowWeaponSelection()
        {
            SetWeaponSelectionVisibility(true);
            _weaponSelectionTimer = weaponSelectionShowTime;
        }

        private void UpdateWeaponSelectionVisibility()
        {
            if (weaponSelectionPanel == null) return;

            if (_weaponSelectionTimer > 0)
            {
                _weaponSelectionTimer -= Time.deltaTime;
                if (_weaponSelectionTimer <= 0)
                {
                    SetWeaponSelectionVisibility(false);
                }
            }

            // Smooth fade animation.
            float targetAlpha = _isWeaponMenuVisible ? 1f : 0f;
            weaponSelectionPanel.alpha = Mathf.Lerp(weaponSelectionPanel.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        }

        private void SetWeaponSelectionVisibility(bool visible)
        {
            _isWeaponMenuVisible = visible;
            weaponSelectionPanel.interactable = visible;
            weaponSelectionPanel.blocksRaycasts = visible;
        }
        
        private void UpdateSelectedWeaponHighlight()
        {
            int currentIndex = weaponManager.GetCurrentWeaponIndex();
        
            // Update highlight for each weapon item.
            for (int i = 0; i < _weaponItems.Count; i++)
            {
                var item = _weaponItems[i];
                var backgroundImage = item.GetComponent<Image>();
                var nameText = item.transform.Find("WeaponInfo/WeaponName")?.GetComponent<TextMeshProUGUI>();
            
                if (i == currentIndex)
                {
                    backgroundImage.color = new Color(selectedWeaponColor.r, selectedWeaponColor.g, selectedWeaponColor.b, 0.4f);
                    if (nameText != null) nameText.color = selectedWeaponColor;
                }
                else
                {
                    backgroundImage.color = new Color(0, 0, 0, 0.4f);
                    if (nameText != null) nameText.color = Color.white;
                }
            }
        }
    }
}