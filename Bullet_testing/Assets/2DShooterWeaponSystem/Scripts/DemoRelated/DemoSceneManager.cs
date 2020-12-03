using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Component that is used to manage and configure / display GUI for various demo scenes for the asset.
/// </summary>
public class DemoSceneManager : MonoBehaviour
{
    public Button nextScene;
    public Button previousScene;

    public Button presetSimple;
    public Button presetCrazySpread;
    public Button presetGatling;
    public Button presetShotgun;
    public Button presetWildFire;
    public Button presetThreeShot;
    public Button presetDualSpread;
    public Button presetImbaCannon;
    public Button presetShower;
    public Button presetDualAlternating;
    public Button presetDualMachineGun;
    public Button presetTarantula;
    public Button presetCircleSpray;
    public Button shooterHorizontal;
    public Button shooterVertical;
    public Button shooterFreeAim;
    public Button changeBulletColourButton;

    public Toggle showWeaponPresetsToggle;
    public Toggle showShooterTypeToggle;
    public Toggle showAdvancedCustomisationToggle;
    public Toggle autoFireToggle;
    public Toggle pingPongSpreadToggle;
    public Toggle circularFireModeToggle;
    public Toggle mirrorBulletsXToggle;

    public Slider bulletCountSlider;
    public Slider bulletRandomnessSlider;
    public Slider bulletSpacingSlider;
    public Slider bulletSpeedSlider;
    public Slider bulletSpreadSlider;
    public Slider bulletPingPongMaxSlider;
    public Slider bulletFireRateSlider;
    public Slider bulletWeaponXOffsetSlider;
    public Slider bulletWeaponYOffsetSlider;
    public Slider bulletRicochetChanceSlider;
    public Slider circularRotationRateSlider;

    public RectTransform weaponPresetsPanel;
    public RectTransform shooterTypePresetsPanel;
    public RectTransform advancedPanel;

    public Text demoSceneText1;
    public Text demoSceneText2;

    // Make sure this is hooked up to your player's WeaponSystem script for the demo GUI to work!
    public WeaponSystem playerWeaponSystemRef;
    public PlayerMovement playerMovementRef;

    private Color[] bulletColours = { new Color(1f, 0.9f, 0.36f, 1f), new Color(0f, 1f, 0.04f, 1f), new Color(0f, 0.95f, 1f, 1f), new Color(1f, 1f, 1f, 1f) };
    private int bulletColourCurrentIndex;

    private string loadedLevelName;

	void Start ()
	{
        loadedLevelName = SceneManager.GetActiveScene().name;
	    bulletColourCurrentIndex = 0;

	    nextScene.onClick.AddListener(NextDemoScene);
	    previousScene.onClick.AddListener(PreviousDemoScene);

        if (showWeaponPresetsToggle != null)
            showWeaponPresetsToggle.onValueChanged.AddListener(WeaponPresetToggleChanged);

	    if (showShooterTypeToggle != null)
	    {
	        showShooterTypeToggle.onValueChanged.AddListener(ShowShooterTypeToggleChanged);
            ShowShooterTypeToggleChanged(true);
        }

	    if (showAdvancedCustomisationToggle != null)
            showAdvancedCustomisationToggle.onValueChanged.AddListener(ShowAdvancedCustomisationToggleChanged);

        if (weaponPresetsPanel != null)
            WeaponPresetToggleChanged(loadedLevelName == "TopDownGunDemoScene");

        if (weaponPresetsPanel != null)
            SetupWeaponPresetButtons();

        if (shooterTypePresetsPanel != null)
            SetupShooterTypeButtons();

        if (advancedPanel != null)
            SetupAdvancedCustomisationControls();

        playerWeaponSystemRef.bulletColour = Color.white;

        // Set up some settings based on the demo scene loaded.
        if (loadedLevelName == "BulletDemoScene")
        {
            if (playerWeaponSystemRef != null)
            {
                ResetPlayerWeaponSystemToBasics(playerWeaponSystemRef);
                playerWeaponSystemRef.BulletPreset = WeaponSystem.BulletPresetType.CrazySpreadPingPong;
            }
        }
        else if (loadedLevelName == "TopDownGunDemoScene")
        {
            if (playerWeaponSystemRef != null)
            {
                ResetPlayerWeaponSystemToBasics(playerWeaponSystemRef);
                playerWeaponSystemRef.BulletPreset = WeaponSystem.BulletPresetType.Simple;

                playerWeaponSystemRef.bulletCount = 1;
                playerWeaponSystemRef.weaponFireRate = 0.15f;
                playerWeaponSystemRef.bulletSpacing = 1f;
                playerWeaponSystemRef.bulletSpread = 0.05f;
                playerWeaponSystemRef.bulletSpeed = 12f;
                playerWeaponSystemRef.bulletRandomness = 0.15f;
            }
        }

        playerWeaponSystemRef.WeaponConfigurationChanged += PlayerWeaponSystemRef_WeaponConfigurationChanged;

	    switch (loadedLevelName)
	    {
            case "BulletDemoScene":
                demoSceneText1.text = "Use the advanced customisation settings to the right to create custom bullet patterns and styles. The Shooter type setting orients the ship in a few different direction types";
                demoSceneText2.text = "Use WASD or arrow keys to move around. In FreeAim mode, use the mouse to aim the ship.";
                break;
            case "TopDownGunDemoScene":
	        case "WeaponConfigurationAndInventoryDemoScene":
	            demoSceneText1.text = "Left click to shoot, WASD or arrow keys to move. 'F' to toggle flashlight. " +
	                                  "Scroll mousewheel to change weapon. 'R' to reload. 'E' to reload one bullet " +
	                                  "at a time.";
	            demoSceneText2.text = "Use the unlimited ammo weapon and adjust sliders on right to try out your own configurations! " +
	                                  "Check the WeaponSystem.cs script for even more customisation settings";
	            break;
	        case "SpaceTopDownDemoScene":
	            demoSceneText1.text = "Scroll the mouse wheel to select different turrets on the space platform. Left click to shoot.";
	            demoSceneText2.text = "Each turret represents a different weapon configuration on the single weapon system configured in the demo scene.";
	            break;
	        default:
	            demoSceneText1.text = "Left click to shoot, WASD or arrow keys to move. 'F' to toggle flashlight. Checkboxes at top of the screen show/hide settings.";
	            demoSceneText2.text = "Check the WeaponSystem.cs script for even more customisation settings.";
	            break;
	    }
    }

    private void PlayerWeaponSystemRef_WeaponConfigurationChanged(Transform gunPointTransform, int weaponConfigIndex)
    {
        SetCurrentWeaponSystemValues();
    }

    private void SetupAdvancedCustomisationControls()
    {
        bulletCountSlider.onValueChanged.AddListener(AdvancedSliderBulletCountChangedHandler);
        bulletRandomnessSlider.onValueChanged.AddListener(AdvancedSliderBulletRandomnessChangedHandler);
        bulletSpacingSlider.onValueChanged.AddListener(AdvancedSliderBulletSpacingChangedHandler);
        bulletSpeedSlider.onValueChanged.AddListener(AdvancedSliderBulletSpeedChangedHandler);
        bulletSpreadSlider.onValueChanged.AddListener(AdvancedSliderBulletSpreadChangedHandler);
        bulletPingPongMaxSlider.onValueChanged.AddListener(AdvancedSliderBulletPingPongMaxChangedHandler);
        bulletFireRateSlider.onValueChanged.AddListener(AdvancedSliderBulletFireRateChangedHandler);
        bulletWeaponXOffsetSlider.onValueChanged.AddListener(AdvancedSliderWeaponXOffsetChangedHandler);
        bulletWeaponYOffsetSlider.onValueChanged.AddListener(AdvancedSliderWeaponYOffsetChangedHandler);
        bulletRicochetChanceSlider.onValueChanged.AddListener(AdvancedSliderBulletRicochetChanceChangedHandler);
        circularRotationRateSlider.onValueChanged.AddListener(AdvancedSliderCircularRotationRateChangedHandler);

        autoFireToggle.onValueChanged.AddListener(AdvancedAutoFireToggleChangedHandler);
        pingPongSpreadToggle.onValueChanged.AddListener(AdvancedPingPongSpreadToggleChangedHandler);
        mirrorBulletsXToggle.onValueChanged.AddListener(AdvancedMirrorBulletsXToggleChangedHandler);
        circularFireModeToggle.onValueChanged.AddListener(AdvancedCircularFireToggleChangedHandler);

        changeBulletColourButton.onClick.AddListener(ChangeBulletColourHandler);

        SetCurrentWeaponSystemValues();
    }

    private void SetCurrentWeaponSystemValues()
    {
        if (advancedPanel == null) return;

        bulletCountSlider.value = playerWeaponSystemRef.bulletCount;
        bulletRandomnessSlider.value = playerWeaponSystemRef.bulletRandomness;
        bulletSpacingSlider.value = playerWeaponSystemRef.bulletSpacing;
        bulletSpeedSlider.value = playerWeaponSystemRef.bulletSpeed;
        bulletSpreadSlider.value = playerWeaponSystemRef.bulletSpread;
        bulletPingPongMaxSlider.value = playerWeaponSystemRef.bulletSpreadPingPongMax;
        bulletFireRateSlider.value = playerWeaponSystemRef.weaponFireRate;
        bulletWeaponXOffsetSlider.value = playerWeaponSystemRef.weaponXOffset;
        bulletWeaponYOffsetSlider.value = playerWeaponSystemRef.weaponYOffset;
        bulletRicochetChanceSlider.value = playerWeaponSystemRef.ricochetChancePercent;
        circularRotationRateSlider.value = playerWeaponSystemRef.circularFireRotationRate;

        autoFireToggle.isOn = playerWeaponSystemRef.autoFire;
        pingPongSpreadToggle.isOn = playerWeaponSystemRef.pingPongSpread;
        mirrorBulletsXToggle.isOn = playerWeaponSystemRef.mirrorX;
        circularFireModeToggle.isOn = playerWeaponSystemRef.circularFireMode;
    }

    private void ChangeBulletColourHandler()
    {
        bulletColourCurrentIndex++;

        if (bulletColourCurrentIndex > bulletColours.Length - 1)
            bulletColourCurrentIndex = 0;

        playerWeaponSystemRef.bulletColour = bulletColours[bulletColourCurrentIndex];
    }

    private void AdvancedSliderBulletCountChangedHandler(float value)
    {
        playerWeaponSystemRef.bulletCount = (int) value;
    }

    private void AdvancedSliderBulletRandomnessChangedHandler(float value)
    {
        playerWeaponSystemRef.bulletRandomness = value;
    }

    private void AdvancedSliderBulletSpacingChangedHandler(float value)
    {
        playerWeaponSystemRef.bulletSpacing = value;
    }

    private void AdvancedSliderBulletSpeedChangedHandler(float value)
    {
        playerWeaponSystemRef.bulletSpeed = value;
    }

    private void AdvancedSliderBulletSpreadChangedHandler(float value)
    {
        playerWeaponSystemRef.bulletSpread = value;
    }

    private void AdvancedSliderBulletPingPongMaxChangedHandler(float value)
    {
        playerWeaponSystemRef.bulletSpreadPingPongMax = value;
    }

    private void AdvancedSliderBulletFireRateChangedHandler(float value)
    {
        playerWeaponSystemRef.weaponFireRate = value;
    }

    private void AdvancedSliderWeaponXOffsetChangedHandler(float value)
    {
        playerWeaponSystemRef.weaponXOffset = value;
    }

    private void AdvancedSliderWeaponYOffsetChangedHandler(float value)
    {
        playerWeaponSystemRef.weaponYOffset = value;
    }

    private void AdvancedSliderBulletRicochetChanceChangedHandler(float value)
    {
        playerWeaponSystemRef.ricochetChancePercent = value;
    }

    private void AdvancedSliderCircularRotationRateChangedHandler(float value)
    {
        playerWeaponSystemRef.circularFireRotationRate = value;
    }

    private void AdvancedAutoFireToggleChangedHandler(bool value)
    {
        playerWeaponSystemRef.autoFire = value;
    }

    private void AdvancedPingPongSpreadToggleChangedHandler(bool value)
    {
        playerWeaponSystemRef.pingPongSpread = value;
    }

    private void AdvancedCircularFireToggleChangedHandler(bool value)
    {
        playerWeaponSystemRef.circularFireMode = value;
    }

    private void AdvancedMirrorBulletsXToggleChangedHandler(bool value)
    {
        playerWeaponSystemRef.mirrorX = value;
    }

    private void SetupWeaponPresetButtons()
    {
        presetSimple.onClick.AddListener(PresetSimple);
        presetCircleSpray.onClick.AddListener(PresetCircleSpray);
        presetCrazySpread.onClick.AddListener(PresetCrazySpread);
        presetDualAlternating.onClick.AddListener(PresetDualAlternating);
        presetDualMachineGun.onClick.AddListener(PresetDualMachineGun);
        presetDualSpread.onClick.AddListener(PresetDualSpread);
        presetGatling.onClick.AddListener(PresetGatling);
        presetImbaCannon.onClick.AddListener(PresetImbaCannon);
        presetShotgun.onClick.AddListener(PresetShotgun);
        presetShower.onClick.AddListener(PresetShower);
        presetTarantula.onClick.AddListener(PresetTarantula);
        presetThreeShot.onClick.AddListener(PresetThreeShot);
        presetWildFire.onClick.AddListener(PresetWildFire);
    }

    private void SetupShooterTypeButtons()
    {
        shooterHorizontal.onClick.AddListener(SetShooterPresetTypeHorizontal);
        shooterVertical.onClick.AddListener(SetShooterPresetTypeVertical);
        shooterFreeAim.onClick.AddListener(SetShooterPresetTypeFreeAim);
    }

    private void SetShooterPresetTypeHorizontal()
    {
        playerWeaponSystemRef.shooterDirectionType = WeaponSystem.ShooterType.Horizonal;
        playerMovementRef.playerMovementType = PlayerMovement.PlayerMovementType.Normal;
    }

    private void SetShooterPresetTypeVertical()
    {
        playerWeaponSystemRef.shooterDirectionType = WeaponSystem.ShooterType.Vertical;
        playerMovementRef.playerMovementType = PlayerMovement.PlayerMovementType.Normal;
    }

    private void SetShooterPresetTypeFreeAim()
    {
        playerWeaponSystemRef.shooterDirectionType = WeaponSystem.ShooterType.FreeAim;
        playerMovementRef.playerMovementType = PlayerMovement.PlayerMovementType.FreeAim;
    }

    private void WeaponPresetToggleChanged(bool value)
    {
        foreach (Transform t in weaponPresetsPanel.transform)
        {
            if (t.gameObject == null) continue;
            t.gameObject.SetActive(value);
            foreach (Transform ct in t.transform)
            {
                ct.gameObject.SetActive(value);
            }
        }
    }

    private void ShowShooterTypeToggleChanged(bool value)
    {
        if (loadedLevelName != "EnemyTurretDemoScene")
        {
            foreach (Transform t in shooterTypePresetsPanel.transform)
            {
                if (t.gameObject == null) continue;
                t.gameObject.SetActive(value);
                foreach (Transform ct in t.transform)
                {
                    ct.gameObject.SetActive(value);
                }
            }
        }
    }

    private void ShowAdvancedCustomisationToggleChanged(bool value)
    {
        foreach (Transform t in advancedPanel.transform)
        {
            if (t.gameObject == null) continue;
            t.gameObject.SetActive(value);
            foreach (Transform ct in t.transform)
            {
                ct.gameObject.SetActive(value);
            }
        }
    }

    private void ResetPlayerWeaponSystemToBasics(WeaponSystem weaponSystem)
    {
        weaponSystem.usesMagazines = false;
        weaponSystem.limitedAmmo = false;
    }

    private static void PreviousDemoScene()
    {
        var currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentLevelIndex > 0)
        {
            SceneManager.LoadScene((currentLevelIndex - 1));
        }
        else
        {
            SceneManager.LoadScene((SceneManager.sceneCountInBuildSettings - 1));
        }
    }

    private static void NextDemoScene()
    {
        var currentLevelIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentLevelIndex < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(currentLevelIndex + 1);
        }
        else
        {
            SceneManager.LoadScene(0);
        }
    }

    private void PresetSimple()
    {
        playerWeaponSystemRef.BulletPreset = WeaponSystem.BulletPresetType.Simple;
    }
    private void PresetCrazySpread()
    {
        playerWeaponSystemRef.BulletPreset = WeaponSystem.BulletPresetType.CrazySpreadPingPong;
    }
    private void PresetGatling()
    {
        playerWeaponSystemRef.BulletPreset = WeaponSystem.BulletPresetType.GatlingGun;
    }

    private void PresetShotgun()
    {
        playerWeaponSystemRef.BulletPreset = WeaponSystem.BulletPresetType.Shotgun;
    }

    private void PresetWildFire()
    {
        playerWeaponSystemRef.BulletPreset = WeaponSystem.BulletPresetType.WildFire;
    }

    private void PresetThreeShot()
    {
        playerWeaponSystemRef.BulletPreset = WeaponSystem.BulletPresetType.ThreeShotSpread;
    }

    private void PresetDualSpread()
    {
        playerWeaponSystemRef.BulletPreset = WeaponSystem.BulletPresetType.DualSpread;
    }

    private void PresetImbaCannon()
    {
        playerWeaponSystemRef.BulletPreset = WeaponSystem.BulletPresetType.ImbaCannon;
    }

    private void PresetShower()
    {
        playerWeaponSystemRef.BulletPreset = WeaponSystem.BulletPresetType.Shower;
    }

    private void PresetDualAlternating()
    {
        playerWeaponSystemRef.BulletPreset = WeaponSystem.BulletPresetType.DualAlternating;
    }

    private void PresetDualMachineGun()
    {
        playerWeaponSystemRef.BulletPreset = WeaponSystem.BulletPresetType.DualMachineGun;
    }

    private void PresetTarantula()
    {
        playerWeaponSystemRef.BulletPreset = WeaponSystem.BulletPresetType.Tarantula;
    }

    private void PresetCircleSpray()
    {
        playerWeaponSystemRef.BulletPreset = WeaponSystem.BulletPresetType.CircleSpray;
    }
}
